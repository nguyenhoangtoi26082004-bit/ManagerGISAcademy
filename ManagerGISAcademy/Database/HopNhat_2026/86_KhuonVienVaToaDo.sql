-- ================================================================
-- 86_KhuonVienVaToaDo.sql
--   (1) Khuon vien doanh trai (da giac) trong ql_don_vi.khu_vuc_don_vi:
--       fn_luu_khuon_vien / fn_xoa_khuon_vien + view v_khuon_vien_don_vi.
--   (2) Kiem tra 1 toa do nhap (GNSS/CSV/KML) KHONG nem loi:
--       fn_kiem_tra_nhap_toa_do -> dung cho man "Nhap toa do hang loat".
--   (3) Lich su vi tri (dong thoi gian): fn_lich_su_vi_tri.
-- Chay bang postgres tren HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

-- ----------------------------------------------------------------
-- 1. KHUON VIEN
-- ----------------------------------------------------------------
CREATE OR REPLACE VIEW ql_don_vi.v_khuon_vien_don_vi
WITH (security_invoker = true) AS
SELECT k.id, d.ma_don_vi, d.ten_don_vi, k.ma_khu_vuc, k.ten_khu_vuc, k.loai_khu_vuc, k.ghi_chu,
       round(ST_Area(k.geom)::numeric, 1)      AS dien_tich_m2,
       round(ST_Perimeter(k.geom)::numeric, 1) AS chu_vi_m,
       ST_Covers(k.geom, d.geom)              AS chua_tru_so,
       k.ngay_cap_nhat,
       ST_AsText(k.geom)                      AS wkt
FROM ql_don_vi.khu_vuc_don_vi k
JOIN ql_don_vi.don_vi_quan_doi d ON d.id = k.don_vi_id
WHERE k.loai_khu_vuc = 'khuon_vien' AND k.trang_thai_quan_ly = 'dang_quan_ly';

CREATE OR REPLACE FUNCTION ql_don_vi.fn_luu_khuon_vien(
    p_ma_don_vi TEXT, p_wkt_32648 TEXT, p_ten TEXT, p_ghi_chu TEXT)
RETURNS TABLE (id BIGINT, dien_tich_m2 NUMERIC, chu_vi_m NUMERIC, chua_tru_so BOOLEAN)
LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
#variable_conflict use_column
DECLARE
    v_dv RECORD;
    v_g geometry;
    v_id BIGINT;
    v_hn geometry;
BEGIN
    IF ql_he_thong.fn_can_duyet() OR
       NOT has_table_privilege(ql_he_thong.fn_vai_tro_hien_hanh(), 'public.don_vi_quan_doi', 'UPDATE') THEN
        RAISE EXCEPTION 'Số hóa khuôn viên cần vai trò Quản trị (vai trò hiện tại: %).', ql_he_thong.fn_vai_tro_hien_hanh();
    END IF;
    SELECT d.id, d.ma_don_vi, d.ten_don_vi, d.geom INTO v_dv
    FROM ql_don_vi.don_vi_quan_doi d WHERE d.ma_don_vi = btrim(p_ma_don_vi);
    IF NOT FOUND THEN RAISE EXCEPTION 'Không tìm thấy đơn vị "%".', p_ma_don_vi; END IF;

    BEGIN
        v_g := ST_SetSRID(ST_GeomFromText(p_wkt_32648), 32648);
    EXCEPTION WHEN OTHERS THEN
        RAISE EXCEPTION 'Hình học không đọc được (WKT): %', SQLERRM;
    END;
    IF GeometryType(v_g) = 'MULTIPOLYGON' AND ST_NumGeometries(v_g) = 1 THEN v_g := ST_GeometryN(v_g, 1); END IF;
    IF GeometryType(v_g) <> 'POLYGON' THEN RAISE EXCEPTION 'Khuôn viên phải là MỘT đa giác (nhận %).', GeometryType(v_g); END IF;
    IF NOT ST_IsValid(v_g) THEN
        v_g := ST_MakeValid(v_g);
        IF GeometryType(v_g) <> 'POLYGON' THEN
            RAISE EXCEPTION 'Đa giác tự cắt, không sửa được thành 1 đa giác: %', ST_IsValidReason(ST_SetSRID(ST_GeomFromText(p_wkt_32648), 32648));
        END IF;
    END IF;
    v_g := ST_ForcePolygonCCW(v_g);
    IF ST_Area(v_g) < 100 THEN RAISE EXCEPTION 'Diện tích % m² quá nhỏ (tối thiểu 100 m²).', round(ST_Area(v_g)); END IF;
    IF ST_Area(v_g) > 2e7 THEN RAISE EXCEPTION 'Diện tích % km² quá lớn (tối đa 20 km²).', round((ST_Area(v_g) / 1e6)::numeric, 1); END IF;
    v_hn := ql_don_vi.fn_ranh_gioi_ha_noi();
    IF v_hn IS NOT NULL AND NOT ST_Intersects(v_hn, v_g) THEN RAISE EXCEPTION 'Khuôn viên nằm ngoài Hà Nội.'; END IF;
    IF ST_Distance(v_g, v_dv.geom) > 2000 THEN
        RAISE EXCEPTION 'Khuôn viên cách trụ sở % m (> 2 km) – kiểm tra lại đơn vị hoặc định vị lại trụ sở trước.', round(ST_Distance(v_g, v_dv.geom));
    END IF;

    -- Moi don vi 1 khuon vien dang quan ly: ban cu chuyen luu tru (giu lich su).
    UPDATE ql_don_vi.khu_vuc_don_vi k
       SET trang_thai_quan_ly = 'luu_tru', ngay_cap_nhat = now()
     WHERE k.don_vi_id = v_dv.id AND k.loai_khu_vuc = 'khuon_vien' AND k.trang_thai_quan_ly = 'dang_quan_ly';

    INSERT INTO ql_don_vi.khu_vuc_don_vi (don_vi_id, ma_khu_vuc, ten_khu_vuc, loai_khu_vuc, mo_ta, ghi_chu, geom)
    VALUES (v_dv.id, v_dv.ma_don_vi || '-KV-' || to_char(clock_timestamp(), 'YYYYMMDDHH24MISSMS'),
            coalesce(nullif(btrim(p_ten), ''), 'Khuôn viên ' || v_dv.ten_don_vi), 'khuon_vien',
            'Số hóa trên ảnh vệ tinh trong ManagerGISAcademy',
            coalesce(nullif(btrim(p_ghi_chu), ''), 'Không có ghi chú'), v_g)
    RETURNING khu_vuc_don_vi.id INTO v_id;

    RETURN QUERY SELECT v_id, round(ST_Area(v_g)::numeric, 1), round(ST_Perimeter(v_g)::numeric, 1), ST_Covers(v_g, v_dv.geom);
END $f$;

CREATE OR REPLACE FUNCTION ql_don_vi.fn_xoa_khuon_vien(p_ma_don_vi TEXT)
RETURNS INT LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE n INT;
BEGIN
    IF ql_he_thong.fn_can_duyet() OR
       NOT has_table_privilege(ql_he_thong.fn_vai_tro_hien_hanh(), 'public.don_vi_quan_doi', 'UPDATE') THEN
        RAISE EXCEPTION 'Xóa khuôn viên cần vai trò Quản trị.';
    END IF;
    UPDATE ql_don_vi.khu_vuc_don_vi k SET trang_thai_quan_ly = 'luu_tru', ngay_cap_nhat = now()
     FROM ql_don_vi.don_vi_quan_doi d
    WHERE d.id = k.don_vi_id AND d.ma_don_vi = btrim(p_ma_don_vi)
      AND k.loai_khu_vuc = 'khuon_vien' AND k.trang_thai_quan_ly = 'dang_quan_ly';
    GET DIAGNOSTICS n = ROW_COUNT;
    RETURN n;
END $f$;

-- ----------------------------------------------------------------
-- 2. KIEM TRA 1 TOA DO NHAP (khong nem loi)
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_don_vi.fn_kiem_tra_nhap_toa_do(
    p_ma_don_vi TEXT, p_srid INT, p_e DOUBLE PRECISION, p_n DOUBLE PRECISION)
RETURNS TABLE (hop_le BOOLEAN, thong_bao TEXT, kinh_do DOUBLE PRECISION, vi_do DOUBLE PRECISION,
               e_utm DOUBLE PRECISION, n_utm DOUBLE PRECISION, ten_xa_phuong TEXT,
               ten_don_vi TEXT, khoang_dich_m DOUBLE PRECISION)
LANGUAGE plpgsql STABLE SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE
    v_g geometry;
    v_dv RECORD;
    v_xa TEXT;
BEGIN
    SELECT d.ten_don_vi, d.geom INTO v_dv FROM ql_don_vi.don_vi_quan_doi d
    JOIN ql_don_vi.dm_trang_thai_quan_ly t ON t.ma = d.trang_thai_quan_ly AND t.con_hieu_luc
    WHERE d.ma_don_vi = btrim(p_ma_don_vi);
    IF NOT FOUND THEN
        hop_le := FALSE; thong_bao := 'Không có đơn vị mã "' || coalesce(p_ma_don_vi, '') || '"';
        RETURN NEXT; RETURN;
    END IF;
    ten_don_vi := v_dv.ten_don_vi;
    BEGIN
        SELECT q.ten_xa_phuong INTO v_xa FROM ql_don_vi.fn_quy_doi_toa_do(p_srid, p_e, p_n) q;
    EXCEPTION WHEN OTHERS THEN
        hop_le := FALSE; thong_bao := SQLERRM; RETURN NEXT; RETURN;
    END;
    v_g := ST_Transform(ST_SetSRID(ST_MakePoint(p_e, p_n), p_srid), 32648);
    hop_le := TRUE;
    ten_xa_phuong := v_xa;
    e_utm := ST_X(v_g); n_utm := ST_Y(v_g);
    kinh_do := ST_X(ST_Transform(v_g, 4326)); vi_do := ST_Y(ST_Transform(v_g, 4326));
    khoang_dich_m := ST_Distance(v_g, v_dv.geom);
    thong_bao := CASE WHEN ST_Distance(v_g, v_dv.geom) > 5000 THEN 'Hợp lệ – dời xa ' || round(ST_Distance(v_g, v_dv.geom) / 1000) || ' km, kiểm tra lại'
                      ELSE 'Hợp lệ' END;
    RETURN NEXT;
END $f$;

-- ----------------------------------------------------------------
-- 3. LICH SU VI TRI (SECURITY INVOKER -> RLS ap dung)
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_don_vi.fn_lich_su_vi_tri(p_ma_don_vi TEXT)
RETURNS TABLE (stt INT, hieu_luc_tu TIMESTAMPTZ, hieu_luc_den TIMESTAMPTZ, la_hien_hanh BOOLEAN,
               phuong_phap TEXT, ten_phuong_phap TEXT, do_chinh_xac_m NUMERIC, la_vi_tri_thuc BOOLEAN,
               nguon TEXT, nguoi_xac_minh TEXT, ghi_chu TEXT, kinh_do DOUBLE PRECISION, vi_do DOUBLE PRECISION,
               e_utm DOUBLE PRECISION, n_utm DOUBLE PRECISION, cach_hien_hanh_m DOUBLE PRECISION)
LANGUAGE sql STABLE SET search_path = ql_don_vi, public AS $f$
    WITH ds AS (
        SELECT v.*, p.ten AS ten_pp, p.la_vi_tri_thuc AS thuc
        FROM ql_don_vi.vi_tri_don_vi v
        JOIN ql_don_vi.don_vi_quan_doi d ON d.id = v.don_vi_id
        LEFT JOIN ql_don_vi.dm_phuong_phap_dinh_vi p ON p.ma = v.phuong_phap
        WHERE d.ma_don_vi = btrim(p_ma_don_vi)
    ), hh AS (SELECT geom FROM ds WHERE la_hien_hanh LIMIT 1)
    SELECT (row_number() OVER (ORDER BY ds.hieu_luc_tu))::int, ds.hieu_luc_tu, ds.hieu_luc_den, ds.la_hien_hanh,
           ds.phuong_phap::text, ds.ten_pp::text, ds.do_chinh_xac_m, coalesce(ds.thuc, false),
           ds.nguon::text, ds.nguoi_xac_minh::text, ds.ghi_chu,
           ST_X(ST_Transform(ds.geom, 4326)), ST_Y(ST_Transform(ds.geom, 4326)), ST_X(ds.geom), ST_Y(ds.geom),
           ST_Distance(ds.geom, (SELECT geom FROM hh))
    FROM ds ORDER BY ds.hieu_luc_tu;
$f$;

GRANT SELECT ON ql_don_vi.v_khuon_vien_don_vi TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;
GRANT EXECUTE ON FUNCTION ql_don_vi.fn_luu_khuon_vien(TEXT, TEXT, TEXT, TEXT), ql_don_vi.fn_xoa_khuon_vien(TEXT),
      ql_don_vi.fn_kiem_tra_nhap_toa_do(TEXT, INT, DOUBLE PRECISION, DOUBLE PRECISION),
      ql_don_vi.fn_lich_su_vi_tri(TEXT)
      TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;

COMMIT;
