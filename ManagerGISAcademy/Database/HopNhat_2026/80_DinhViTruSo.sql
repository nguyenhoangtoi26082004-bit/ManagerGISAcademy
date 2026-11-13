-- ================================================================
-- 80_DinhViTruSo.sql
-- Chuc nang "Dinh vi lai tru so": ung dung goi HAM nay thay vi
-- ghi thang vao bang vi_tri_don_vi.
--
--   ql_don_vi.fn_quy_doi_toa_do(srid, e, n)
--       -> quy doi 1 diem sang WGS84 / UTM 48N / VN-2000 (TM-3 105°)
--          + phuong/xa chua diem (de xem truoc tren hop thoai).
--   ql_don_vi.fn_dinh_vi_tru_so(...)
--       -> kiem quyen nguoi goi, tu cap nhat xa/phuong theo vi tri moi,
--          them vi tri moi (vi tri cu thanh lich su), ghi nhat ky.
--
-- Chay bang tai khoan postgres tren CSDL HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

-- ================================================================
-- 1. QUY DOI TOA DO (chi doc)
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_quy_doi_toa_do(
    p_srid INTEGER,
    p_e    DOUBLE PRECISION,
    p_n    DOUBLE PRECISION)
RETURNS TABLE (
    kinh_do      DOUBLE PRECISION,
    vi_do        DOUBLE PRECISION,
    e_32648      DOUBLE PRECISION,
    n_32648      DOUBLE PRECISION,
    x_vn2000     DOUBLE PRECISION,   -- quy uoc VN: X = Bac
    y_vn2000     DOUBLE PRECISION,   -- quy uoc VN: Y = Dong
    ten_xa_phuong TEXT,
    trong_ha_noi BOOLEAN)
LANGUAGE plpgsql
STABLE
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
#variable_conflict use_column
DECLARE
    v_diem geometry;
BEGIN
    IF p_srid NOT IN (4326, 4756, 5897, 3405, 32648) THEN
        RAISE EXCEPTION 'Hệ tọa độ EPSG:% chưa được hỗ trợ.', p_srid;
    END IF;

    -- Kiem tra mien gia tri truoc khi chieu (phat hien dao truc).
    IF p_srid IN (4326, 4756) THEN
        IF NOT (p_e BETWEEN 104.8 AND 106.5 AND p_n BETWEEN 20.2 AND 21.8) THEN
            RAISE EXCEPTION
                'Kinh độ %, vĩ độ % ngoài vùng Hà Nội. Có thể đã nhập ngược.',
                p_e, p_n;
        END IF;
    ELSIF NOT (p_e BETWEEN 400000 AND 700000 AND p_n BETWEEN 2200000 AND 2450000) THEN
        RAISE EXCEPTION
            'Tọa độ phẳng E=%, N=% ngoài vùng Hà Nội. '
            'Quy ước VN-2000: X là tọa độ Bắc, Y là tọa độ Đông.',
            p_e, p_n;
    END IF;

    v_diem := ST_Transform(ST_SetSRID(ST_MakePoint(p_e, p_n), p_srid), 32648);

    RETURN QUERY
    SELECT
        ST_X(ST_Transform(v_diem, 4326)),
        ST_Y(ST_Transform(v_diem, 4326)),
        ST_X(v_diem),
        ST_Y(v_diem),
        ST_Y(ST_Transform(v_diem, 5897)),
        ST_X(ST_Transform(v_diem, 5897)),
        (SELECT CASE x.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END
                    || x.ten_xa_phuong
           FROM ql_don_vi.dm_xa_phuong x
          WHERE ST_Covers(x.geom, v_diem)
          ORDER BY x.id
          LIMIT 1),
        ST_DWithin(v_diem, ql_don_vi.fn_ranh_gioi_ha_noi(), 100);
END;
$f$;

-- ================================================================
-- 2. DINH VI LAI TRU SO
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_dinh_vi_tru_so(
    p_ma_don_vi    TEXT,
    p_srid         INTEGER,
    p_e            DOUBLE PRECISION,
    p_n            DOUBLE PRECISION,
    p_phuong_phap  TEXT,
    p_do_chinh_xac NUMERIC,
    p_nguon        TEXT,
    p_nguoi        TEXT,
    p_ghi_chu      TEXT)
RETURNS TABLE (
    vi_tri_id      BIGINT,
    ten_xa_phuong  TEXT,
    da_doi_xa      BOOLEAN,
    do_chinh_xac_m NUMERIC,
    khoang_dich_m  DOUBLE PRECISION)
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
#variable_conflict use_column
DECLARE
    v_dv       RECORD;
    v_diem     geometry;
    v_xa_id    BIGINT;
    v_xa_ten   TEXT;
    v_doi_xa   BOOLEAN;
    v_id       BIGINT;
    v_dcx      NUMERIC;
    v_dich     DOUBLE PRECISION;
    v_cu       JSONB;
BEGIN
    -- 2.1. Quyen: nguoi goi (session_user) phai co quyen SUA ho so.
    IF NOT has_table_privilege(session_user, 'public.don_vi_quan_doi', 'UPDATE') THEN
        RAISE EXCEPTION 'Tài khoản % không có quyền định vị lại trụ sở.', session_user;
    END IF;

    IF NULLIF(btrim(p_nguon), '') IS NULL THEN
        RAISE EXCEPTION 'Phải ghi rõ nguồn vị trí (ảnh vệ tinh, máy GNSS, tài liệu...).';
    END IF;

    SELECT d.id, d.ma_don_vi, d.xa_phuong_id, d.geom,
           v.phuong_phap, v.do_chinh_xac_m
      INTO v_dv
      FROM ql_don_vi.don_vi_quan_doi d
      JOIN ql_don_vi.dm_trang_thai_quan_ly t
        ON t.ma = d.trang_thai_quan_ly AND t.con_hieu_luc
      LEFT JOIN ql_don_vi.vi_tri_don_vi v
        ON v.don_vi_id = d.id AND v.la_hien_hanh
     WHERE d.ma_don_vi = btrim(p_ma_don_vi);

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Không tìm thấy đơn vị đang quản lý có mã "%".', p_ma_don_vi;
    END IF;

    -- 2.2. Quy doi + kiem tra mien (dung chung ham 1).
    SELECT q.ten_xa_phuong INTO v_xa_ten
      FROM ql_don_vi.fn_quy_doi_toa_do(p_srid, p_e, p_n) q;

    v_diem := ST_Transform(ST_SetSRID(ST_MakePoint(p_e, p_n), p_srid), 32648);

    SELECT x.id INTO v_xa_id
      FROM ql_don_vi.dm_xa_phuong x
     WHERE ST_Covers(x.geom, v_diem)
     ORDER BY x.id
     LIMIT 1;

    IF v_xa_id IS NULL THEN
        RAISE EXCEPTION 'Vị trí không thuộc phường/xã nào của Hà Nội.';
    END IF;

    v_dich := ST_Distance(v_diem, v_dv.geom);

    -- 2.3. Vi tri moi la can cu: cap nhat xa/phuong TRUOC khi them
    --      vi tri (trigger vi_tri kiem tra diem thuoc xa khai bao).
    v_doi_xa := v_xa_id IS DISTINCT FROM v_dv.xa_phuong_id;

    IF v_doi_xa THEN
        UPDATE ql_don_vi.don_vi_quan_doi
           SET xa_phuong_id = v_xa_id,
               ngay_cap_nhat = CURRENT_TIMESTAMP
         WHERE id = v_dv.id;
    END IF;

    SELECT to_jsonb(u) INTO v_cu FROM public.don_vi_quan_doi u WHERE u.id = v_dv.id;

    INSERT INTO ql_don_vi.vi_tri_don_vi
        (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap,
         do_chinh_xac_m, nguon, nguoi_xac_minh, ghi_chu)
    VALUES
        (v_dv.id, p_srid, p_e, p_n, p_phuong_phap,
         p_do_chinh_xac, btrim(p_nguon),
         COALESCE(NULLIF(btrim(p_nguoi), ''), session_user),
         NULLIF(btrim(p_ghi_chu), ''))
    RETURNING id, vi_tri_don_vi.do_chinh_xac_m INTO v_id, v_dcx;

    -- 2.4. Nhat ky kieu ung dung (xem duoc o man "Nhat ky").
    INSERT INTO public.nhat_ky_don_vi_quan_doi
        (thao_tac, ma_don_vi, du_lieu_cu, du_lieu_moi, ung_dung)
    VALUES
        ('U', v_dv.ma_don_vi,
         jsonb_build_object(
             'vi_tri_phuong_phap', v_dv.phuong_phap,
             'vi_tri_do_chinh_xac_m', v_dv.do_chinh_xac_m,
             'kinh_do', round((v_cu ->> 'kinh_do')::numeric, 6),
             'vi_do',   round((v_cu ->> 'vi_do')::numeric, 6),
             'quan_huyen', v_cu ->> 'quan_huyen'),
         (SELECT jsonb_build_object(
                     'vi_tri_phuong_phap', p_phuong_phap,
                     'vi_tri_do_chinh_xac_m', v_dcx,
                     'kinh_do', round(u.kinh_do::numeric, 6),
                     'vi_do',   round(u.vi_do::numeric, 6),
                     'quan_huyen', u.quan_huyen,
                     'vi_tri_nguon', btrim(p_nguon))
            FROM public.don_vi_quan_doi u WHERE u.id = v_dv.id),
         current_setting('application_name', true));

    RETURN QUERY SELECT v_id, v_xa_ten, v_doi_xa, v_dcx, v_dich;
END;
$f$;

-- ================================================================
-- 3. QUYEN: ung dung chi duoc GOI HAM, khong ghi bang goc
-- ================================================================

REVOKE ALL ON FUNCTION ql_don_vi.fn_quy_doi_toa_do(INTEGER, DOUBLE PRECISION, DOUBLE PRECISION) FROM PUBLIC;
REVOKE ALL ON FUNCTION ql_don_vi.fn_dinh_vi_tru_so(TEXT, INTEGER, DOUBLE PRECISION, DOUBLE PRECISION, TEXT, NUMERIC, TEXT, TEXT, TEXT) FROM PUBLIC;

GRANT EXECUTE ON FUNCTION ql_don_vi.fn_quy_doi_toa_do(INTEGER, DOUBLE PRECISION, DOUBLE PRECISION) TO managergis_app;
GRANT EXECUTE ON FUNCTION ql_don_vi.fn_dinh_vi_tru_so(TEXT, INTEGER, DOUBLE PRECISION, DOUBLE PRECISION, TEXT, NUMERIC, TEXT, TEXT, TEXT) TO managergis_app;

COMMIT;
