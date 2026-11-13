-- ================================================================
-- 84_HieuNangVaKiemThu.sql
-- (1) Chi muc + materialized view thong ke + ham do hieu nang
--     (EXPLAIN ANALYZE co / khong dung chi muc).
-- (2) Bo kiem thu CSDL tu dong (thay pgTAP - may chua cai pgTAP):
--     ql_kiem_thu.fn_chay_kiem_thu() -> moi dong = 1 ca kiem thu.
--     Ca kiem thu co ghi du lieu chay trong khoi BEGIN..EXCEPTION
--     va tu hoan tac (khong de lai dau vet).
-- (3) Ham doc kich thuoc bang / chi muc cho man "Quan tri he thong".
-- Chay bang postgres tren HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

CREATE SCHEMA IF NOT EXISTS ql_he_thong;
CREATE SCHEMA IF NOT EXISTS ql_kiem_thu;
COMMENT ON SCHEMA ql_he_thong IS 'Quan tri he thong: phan quyen, duyet thay doi, hieu nang.';
COMMENT ON SCHEMA ql_kiem_thu IS 'Bo kiem thu CSDL tu dong.';

-- ----------------------------------------------------------------
-- 1. CHI MUC (khong gian GiST + khoa ngoai + nhat ky)
-- ----------------------------------------------------------------
CREATE INDEX IF NOT EXISTS idx_dvqd_geom_gist     ON ql_don_vi.don_vi_quan_doi USING gist (geom);
CREATE INDEX IF NOT EXISTS idx_vtdv_geom_gist     ON ql_don_vi.vi_tri_don_vi   USING gist (geom);
CREATE INDEX IF NOT EXISTS idx_xa_phuong_geom_gist ON ql_don_vi.dm_xa_phuong   USING gist (geom);
CREATE INDEX IF NOT EXISTS idx_khu_vuc_geom_gist  ON ql_don_vi.khu_vuc_don_vi  USING gist (geom);
CREATE INDEX IF NOT EXISTS idx_giaothong_geom_gist ON public.giaothong         USING gist (geom);
CREATE INDEX IF NOT EXISTS idx_vtdv_hien_hanh     ON ql_don_vi.vi_tri_don_vi (don_vi_id) WHERE la_hien_hanh;
CREATE INDEX IF NOT EXISTS idx_vtdv_don_vi_tg     ON ql_don_vi.vi_tri_don_vi (don_vi_id, hieu_luc_tu DESC);
CREATE INDEX IF NOT EXISTS idx_dvqd_loai          ON ql_don_vi.don_vi_quan_doi (loai_don_vi_id);
CREATE INDEX IF NOT EXISTS idx_dvqd_xa            ON ql_don_vi.don_vi_quan_doi (xa_phuong_id);
CREATE INDEX IF NOT EXISTS idx_nktd_thoi_diem     ON ql_don_vi.nhat_ky_thay_doi (thoi_diem DESC);
CREATE INDEX IF NOT EXISTS idx_nktd_bang_ban_ghi  ON ql_don_vi.nhat_ky_thay_doi (ten_bang, id_ban_ghi);

-- ----------------------------------------------------------------
-- 2. MATERIALIZED VIEW thong ke (loai x phuong/xa)
-- ----------------------------------------------------------------
DROP MATERIALIZED VIEW IF EXISTS ql_he_thong.mv_thong_ke_loai_xa;
CREATE MATERIALIZED VIEW ql_he_thong.mv_thong_ke_loai_xa AS
SELECT l.ten_loai,
       x.ma_xa_phuong,
       x.ten_xa_phuong,
       count(*)                                   AS so_don_vi,
       count(*) FILTER (WHERE v.phuong_phap <> 'DAI_DIEN_DIA_BAN') AS so_vi_tri_thuc,
       now()                                      AS lam_moi_luc
FROM ql_don_vi.don_vi_quan_doi d
JOIN ql_don_vi.loai_don_vi l ON l.id = d.loai_don_vi_id
JOIN ql_don_vi.dm_xa_phuong x ON x.id = d.xa_phuong_id
JOIN ql_don_vi.dm_trang_thai_quan_ly t ON t.ma = d.trang_thai_quan_ly AND t.con_hieu_luc
LEFT JOIN ql_don_vi.vi_tri_don_vi v ON v.don_vi_id = d.id AND v.la_hien_hanh
GROUP BY l.ten_loai, x.ma_xa_phuong, x.ten_xa_phuong;
CREATE UNIQUE INDEX ux_mv_thong_ke_loai_xa ON ql_he_thong.mv_thong_ke_loai_xa (ten_loai, ma_xa_phuong);

CREATE OR REPLACE FUNCTION ql_he_thong.fn_lam_moi_thong_ke()
RETURNS TEXT LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_he_thong, public AS $f$
DECLARE t0 timestamptz := clock_timestamp();
BEGIN
    REFRESH MATERIALIZED VIEW CONCURRENTLY ql_he_thong.mv_thong_ke_loai_xa;
    ANALYZE ql_don_vi.don_vi_quan_doi;
    ANALYZE ql_don_vi.vi_tri_don_vi;
    ANALYZE ql_don_vi.ho_so_nghiep_vu;
    RETURN 'Đã làm mới thống kê và ANALYZE trong ' ||
           round(extract(epoch FROM clock_timestamp() - t0) * 1000) || ' ms';
END $f$;

-- ----------------------------------------------------------------
-- 3. KICH THUOC BANG / CHI MUC
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_kich_thuoc_bang()
RETURNS TABLE (luoc_do TEXT, ten_bang TEXT, so_dong BIGINT, du_lieu TEXT, chi_muc TEXT, tong TEXT, tong_byte BIGINT)
LANGUAGE sql STABLE SECURITY DEFINER SET search_path = pg_catalog AS $f$
    SELECT n.nspname::text, c.relname::text, c.reltuples::bigint,
           pg_size_pretty(pg_table_size(c.oid)), pg_size_pretty(pg_indexes_size(c.oid)),
           pg_size_pretty(pg_total_relation_size(c.oid)), pg_total_relation_size(c.oid)
    FROM pg_class c JOIN pg_namespace n ON n.oid = c.relnamespace
    WHERE c.relkind IN ('r', 'm') AND n.nspname IN ('public', 'ql_don_vi', 'ql_he_thong', 'ql_ban_do_nen')
    ORDER BY pg_total_relation_size(c.oid) DESC;
$f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_danh_sach_chi_muc()
RETURNS TABLE (luoc_do TEXT, ten_bang TEXT, ten_chi_muc TEXT, kieu TEXT, kich_thuoc TEXT, so_lan_quet BIGINT, dinh_nghia TEXT)
LANGUAGE sql STABLE SECURITY DEFINER SET search_path = pg_catalog AS $f$
    SELECT s.schemaname::text, s.relname::text, s.indexrelname::text, am.amname::text,
           pg_size_pretty(pg_relation_size(s.indexrelid)), s.idx_scan, pg_get_indexdef(s.indexrelid)
    FROM pg_stat_user_indexes s
    JOIN pg_class ic ON ic.oid = s.indexrelid
    JOIN pg_am am ON am.oid = ic.relam
    WHERE s.schemaname IN ('public', 'ql_don_vi', 'ql_he_thong', 'ql_ban_do_nen')
    ORDER BY s.schemaname, s.relname, s.indexrelname;
$f$;

-- ----------------------------------------------------------------
-- 4. DO HIEU NANG: moi truy van chay EXPLAIN ANALYZE 2 lan
--    (binh thuong / tat quet chi muc) -> so sanh thoi gian.
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_do_hieu_nang()
RETURNS TABLE (ten TEXT, mo_ta TEXT, ms_co_chi_muc NUMERIC, ms_khong_chi_muc NUMERIC,
               dung_chi_muc BOOLEAN, ke_hoach TEXT)
LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE
    q RECORD;
    j1 JSON; j2 JSON;
    t1 NUMERIC; t2 NUMERIC;
BEGIN
    FOR q IN SELECT * FROM (VALUES
        ('KNN 10 đơn vị gần Hồ Gươm', 'ORDER BY geom <-> điểm (GiST KNN)',
         'SELECT id FROM ql_don_vi.don_vi_quan_doi ORDER BY geom <-> ST_Transform(ST_SetSRID(ST_MakePoint(105.8523,21.0285),4326),32648) LIMIT 10'),
        ('Đơn vị trong bán kính 3 km', 'ST_DWithin dùng GiST',
         'SELECT count(*) FROM ql_don_vi.don_vi_quan_doi WHERE ST_DWithin(geom, ST_Transform(ST_SetSRID(ST_MakePoint(105.8523,21.0285),4326),32648), 3000)'),
        ('Phường/xã chứa một điểm', 'ST_Covers trên 126 đa giác',
         'SELECT ma_xa_phuong FROM ql_don_vi.dm_xa_phuong WHERE ST_Covers(geom, ST_Transform(ST_SetSRID(ST_MakePoint(105.8523,21.0285),4326),32648))'),
        ('Vị trí hiện hành của 1 đơn vị', 'Chỉ mục một phần WHERE la_hien_hanh',
         'SELECT * FROM ql_don_vi.vi_tri_don_vi WHERE don_vi_id = (SELECT min(id) FROM ql_don_vi.don_vi_quan_doi) AND la_hien_hanh'),
        ('Nhật ký 50 thay đổi gần nhất', 'B-tree thoi_diem DESC',
         'SELECT * FROM ql_don_vi.nhat_ky_thay_doi ORDER BY thoi_diem DESC LIMIT 50'),
        ('Thống kê loại × xã (MV)', 'Đọc materialized view thay vì GROUP BY',
         'SELECT ten_loai, sum(so_don_vi) FROM ql_he_thong.mv_thong_ke_loai_xa GROUP BY ten_loai')
    ) AS v(ten, mo_ta, sql) LOOP
        -- Lam nong bo dem 1 lan, sau do lay thoi gian NHO NHAT cua 3 lan do.
        EXECUTE 'EXPLAIN (ANALYZE, FORMAT JSON) ' || q.sql INTO j1;
        t1 := NULL;
        FOR i IN 1..3 LOOP
            EXECUTE 'EXPLAIN (ANALYZE, FORMAT JSON) ' || q.sql INTO j1;
            t1 := LEAST(coalesce(t1, 1e9), round(((j1 -> 0 ->> 'Execution Time')::numeric), 3));
        END LOOP;
        PERFORM set_config('enable_indexscan', 'off', true);
        PERFORM set_config('enable_bitmapscan', 'off', true);
        PERFORM set_config('enable_indexonlyscan', 'off', true);
        EXECUTE 'EXPLAIN (ANALYZE, FORMAT JSON) ' || q.sql INTO j2;
        t2 := NULL;
        FOR i IN 1..3 LOOP
            EXECUTE 'EXPLAIN (ANALYZE, FORMAT JSON) ' || q.sql INTO j2;
            t2 := LEAST(coalesce(t2, 1e9), round(((j2 -> 0 ->> 'Execution Time')::numeric), 3));
        END LOOP;
        PERFORM set_config('enable_indexscan', 'on', true);
        PERFORM set_config('enable_bitmapscan', 'on', true);
        PERFORM set_config('enable_indexonlyscan', 'on', true);
        ten := q.ten; mo_ta := q.mo_ta; ms_co_chi_muc := t1; ms_khong_chi_muc := t2;
        dung_chi_muc := (j1::text ~ '"Node Type": "(Index|Bitmap)');
        ke_hoach := (j1 -> 0 -> 'Plan' ->> 'Node Type');
        RETURN NEXT;
    END LOOP;
END $f$;

-- ----------------------------------------------------------------
-- 5. BO KIEM THU
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_kiem_thu.fn_chay_kiem_thu()
RETURNS TABLE (stt INT, nhom TEXT, ten TEXT, dat BOOLEAN, chi_tiet TEXT)
LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE
    n INT := 0;
    v_id BIGINT; v_cnt INT; v_cnt2 INT; v_txt TEXT; v_ma TEXT;
BEGIN
    -- Ham noi bo: ghi ket qua
    -- (PL/pgSQL khong co ham long nhau -> lap mau RETURN NEXT).

    -- ===== A. RANG BUOC DU LIEU =====
    n := n + 1; stt := n; nhom := 'Ràng buộc'; ten := 'Độ chính xác vị trí phải > 0';
    BEGIN
        INSERT INTO vi_tri_don_vi (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, do_chinh_xac_m, nguon)
        SELECT id, 4326, 105.85, 21.03, 'GNSS_RTK', 0, 'kiem thu' FROM don_vi_quan_doi LIMIT 1;
        dat := false; chi_tiet := 'CSDL KHÔNG chặn độ chính xác = 0';
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN check_violation THEN dat := true; chi_tiet := 'Bị chặn đúng: ' || SQLERRM;
        WHEN raise_exception THEN IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := true; chi_tiet := 'Bị chặn: ' || SQLERRM; END IF;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Ràng buộc'; ten := 'Chỉ nhận hệ tọa độ gốc hợp lệ (4326/4756/5897/3405/32648)';
    BEGIN
        INSERT INTO vi_tri_don_vi (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, do_chinh_xac_m, nguon)
        SELECT id, 3857, 11780000, 2390000, 'GNSS_RTK', 1, 'kiem thu' FROM don_vi_quan_doi LIMIT 1;
        dat := false; chi_tiet := 'CSDL nhận SRID 3857';
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN check_violation THEN dat := true; chi_tiet := 'Bị chặn đúng: ' || SQLERRM;
        WHEN raise_exception THEN IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := true; chi_tiet := 'Bị chặn: ' || SQLERRM; END IF;
        WHEN OTHERS THEN dat := true; chi_tiet := 'Bị chặn: ' || SQLERRM;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Ràng buộc'; ten := 'Định dạng số điện thoại trực ban';
    BEGIN
        UPDATE ho_so_nghiep_vu SET so_dien_thoai = 'abc-xyz' WHERE id = (SELECT min(id) FROM ho_so_nghiep_vu);
        dat := false; chi_tiet := 'CSDL nhận số điện thoại sai định dạng';
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN check_violation THEN dat := true; chi_tiet := 'Bị chặn đúng: ' || SQLERRM;
        WHEN raise_exception THEN IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := true; chi_tiet := SQLERRM; END IF;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Ràng buộc'; ten := 'Mã đơn vị không trùng';
    SELECT count(*) INTO v_cnt FROM (SELECT ma_don_vi FROM don_vi_quan_doi GROUP BY ma_don_vi HAVING count(*) > 1) z;
    dat := v_cnt = 0; chi_tiet := v_cnt || ' mã bị trùng';
    RETURN NEXT;

    -- ===== B. TRIGGER =====
    n := n + 1; stt := n; nhom := 'Trigger'; ten := 'Không khai báo sai số tốt hơn phương pháp cho phép';
    BEGIN
        INSERT INTO vi_tri_don_vi (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, do_chinh_xac_m, nguon)
        SELECT d.id, 32648, ST_X(d.geom), ST_Y(d.geom), 'MA_HOA_DIA_CHI', 5, 'kiem thu'
        FROM don_vi_quan_doi d LIMIT 1;
        dat := false; chi_tiet := 'Trigger không chặn MA_HOA_DIA_CHI ±5 m';
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN raise_exception THEN
            IF SQLERRM = 'KT_HOAN_TAC' THEN NULL;
            ELSE dat := true; chi_tiet := 'Bị chặn đúng: ' || SQLERRM; END IF;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Trigger'; ten := 'Định vị lại: giữ lịch sử, đúng 1 vị trí hiện hành';
    BEGIN
        SELECT d.ma_don_vi, d.id INTO v_ma, v_id FROM don_vi_quan_doi d
        JOIN dm_trang_thai_quan_ly t ON t.ma = d.trang_thai_quan_ly AND t.con_hieu_luc
        ORDER BY d.id LIMIT 1;
        SELECT count(*) INTO v_cnt FROM vi_tri_don_vi WHERE don_vi_id = v_id;
        PERFORM set_config('ql.dang_duyet', '1', true);
        PERFORM ql_don_vi.fn_dinh_vi_tru_so(v_ma, 32648,
            (SELECT ST_X(geom) + 3 FROM don_vi_quan_doi WHERE id = v_id),
            (SELECT ST_Y(geom) FROM don_vi_quan_doi WHERE id = v_id),
            'GNSS_RTK', 0.05, 'kiem thu tu dong', 'kiem_thu', NULL);
        PERFORM set_config('ql.dang_duyet', '0', true);
        SELECT count(*) INTO v_cnt2 FROM vi_tri_don_vi WHERE don_vi_id = v_id;
        SELECT count(*) INTO v_txt FROM vi_tri_don_vi WHERE don_vi_id = v_id AND la_hien_hanh;
        dat := v_cnt2 = v_cnt + 1 AND v_txt::int = 1;
        chi_tiet := 'Số bản ghi vị trí ' || v_cnt || ' → ' || v_cnt2 || ', hiện hành: ' || v_txt;
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN raise_exception THEN
            IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := false; chi_tiet := SQLERRM; END IF;
        WHEN OTHERS THEN dat := false; chi_tiet := SQLERRM;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Trigger'; ten := 'Xóa qua view = xóa mềm (lưu trữ)';
    BEGIN
        SELECT d.id INTO v_id FROM public.don_vi_quan_doi d
        WHERE NOT EXISTS (SELECT 1 FROM ql_don_vi.don_vi_quan_doi c WHERE c.don_vi_cap_tren_id = d.id)
          AND NOT EXISTS (SELECT 1 FROM ql_don_vi.ho_so_nghiep_vu h WHERE lower(btrim(h.don_vi_cap_tren)) IN (lower(d.ma_don_vi), lower(d.ten_don_vi)))
        ORDER BY d.id DESC LIMIT 1;
        PERFORM set_config('ql.dang_duyet', '1', true);
        DELETE FROM public.don_vi_quan_doi WHERE id = v_id;
        PERFORM set_config('ql.dang_duyet', '0', true);
        SELECT trang_thai_quan_ly INTO v_txt FROM ql_don_vi.don_vi_quan_doi WHERE id = v_id;
        dat := v_txt = 'luu_tru';
        chi_tiet := 'Trạng thái sau xóa: ' || coalesce(v_txt, '(mất bản ghi!)');
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN raise_exception THEN
            IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := false; chi_tiet := SQLERRM; END IF;
        WHEN OTHERS THEN dat := false; chi_tiet := SQLERRM;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Trigger'; ten := 'Sửa hồ sơ được ghi nhật ký';
    BEGIN
        SELECT count(*) INTO v_cnt FROM ql_don_vi.nhat_ky_thay_doi;
        UPDATE ho_so_nghiep_vu SET trang_thai_sscd = coalesce(trang_thai_sscd, '') || ' '
        WHERE id = (SELECT min(id) FROM ho_so_nghiep_vu);
        SELECT count(*) INTO v_cnt2 FROM ql_don_vi.nhat_ky_thay_doi;
        dat := v_cnt2 > v_cnt; chi_tiet := 'Nhật ký ' || v_cnt || ' → ' || v_cnt2;
        RAISE EXCEPTION 'KT_HOAN_TAC';
    EXCEPTION
        WHEN raise_exception THEN
            IF SQLERRM <> 'KT_HOAN_TAC' THEN dat := false; chi_tiet := SQLERRM; END IF;
        WHEN OTHERS THEN dat := false; chi_tiet := SQLERRM;
    END;
    RETURN NEXT;

    -- ===== C. HAM KHONG GIAN =====
    n := n + 1; stt := n; nhom := 'Hàm không gian'; ten := 'Quy đổi tọa độ: Hồ Gươm thuộc phường Hoàn Kiếm';
    BEGIN
        SELECT q.ten_xa_phuong INTO v_txt FROM ql_don_vi.fn_quy_doi_toa_do(4326, 105.8523, 21.0285) q;
        dat := v_txt ILIKE '%Hoàn Kiếm%'; chi_tiet := 'Kết quả: ' || coalesce(v_txt, '(null)');
    EXCEPTION WHEN OTHERS THEN dat := false; chi_tiet := SQLERRM;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Hàm không gian'; ten := 'Tọa độ ngoài Hà Nội bị từ chối';
    BEGIN
        SELECT q.ten_xa_phuong INTO v_txt FROM ql_don_vi.fn_quy_doi_toa_do(4326, 106.6297, 10.8231) q;
        dat := false; chi_tiet := 'Nhận điểm TP.HCM: ' || coalesce(v_txt, '');
    EXCEPTION WHEN OTHERS THEN dat := true; chi_tiet := 'Bị chặn đúng: ' || SQLERRM;
    END;
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Dữ liệu'; ten := 'Mỗi đơn vị có đúng 1 vị trí hiện hành';
    SELECT count(*) INTO v_cnt FROM ql_don_vi.don_vi_quan_doi d
    WHERE (SELECT count(*) FROM vi_tri_don_vi v WHERE v.don_vi_id = d.id AND v.la_hien_hanh) <> 1;
    dat := v_cnt = 0; chi_tiet := v_cnt || ' đơn vị lệch';
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Dữ liệu'; ten := 'Vị trí thật nằm trong phường/xã khai báo';
    SELECT count(*) INTO v_cnt
    FROM ql_don_vi.don_vi_quan_doi d
    JOIN vi_tri_don_vi v ON v.don_vi_id = d.id AND v.la_hien_hanh AND v.phuong_phap <> 'DAI_DIEN_DIA_BAN'
    JOIN dm_xa_phuong x ON x.id = d.xa_phuong_id
    WHERE NOT ST_Covers(x.geom, v.geom);
    dat := v_cnt = 0; chi_tiet := v_cnt || ' vị trí nằm ngoài xã khai báo';
    RETURN NEXT;

    n := n + 1; stt := n; nhom := 'Dữ liệu'; ten := 'Hình học hợp lệ (ST_IsValid) ở ranh giới xã';
    SELECT count(*) INTO v_cnt FROM dm_xa_phuong WHERE NOT ST_IsValid(geom);
    dat := v_cnt = 0; chi_tiet := v_cnt || ' đa giác không hợp lệ';
    RETURN NEXT;

    -- ===== D. PHAN QUYEN (neu da chay script 85) =====
    IF to_regproc('ql_he_thong.fn_muc_vai_tro') IS NOT NULL THEN
        n := n + 1; stt := n; nhom := 'Phân quyền RLS'; ten := 'Vai trò Chỉ xem không thấy hồ sơ Tối mật';
        EXECUTE 'SELECT ql_he_thong.fn_duoc_xem_muc(''gis_xem'', ''Tối mật'')' INTO dat;
        dat := NOT dat; chi_tiet := CASE WHEN dat THEN 'Ẩn đúng' ELSE 'BỊ LỘ' END;
        RETURN NEXT;

        n := n + 1; stt := n; nhom := 'Phân quyền RLS'; ten := 'Biên tập thấy "Mật" nhưng không thấy "Tuyệt mật"';
        EXECUTE 'SELECT ql_he_thong.fn_duoc_xem_muc(''gis_bien_tap'', ''Mật'') AND NOT ql_he_thong.fn_duoc_xem_muc(''gis_bien_tap'', ''Tuyệt mật'')' INTO dat;
        chi_tiet := CASE WHEN dat THEN 'Đúng phạm vi' ELSE 'Sai phạm vi' END;
        RETURN NEXT;

        n := n + 1; stt := n; nhom := 'Phân quyền RLS'; ten := 'Quản trị thấy toàn bộ hồ sơ';
        EXECUTE 'SELECT ql_he_thong.fn_duoc_xem_muc(''gis_quan_tri'', ''Tuyệt mật'')' INTO dat;
        chi_tiet := CASE WHEN dat THEN 'Đúng' ELSE 'Thiếu quyền' END;
        RETURN NEXT;

        n := n + 1; stt := n; nhom := 'Phân quyền RLS'; ten := 'RLS đã bật trên bảng đơn vị, vị trí, hồ sơ';
        SELECT count(*) INTO v_cnt FROM pg_class
        WHERE oid IN ('ql_don_vi.don_vi_quan_doi'::regclass, 'ql_don_vi.vi_tri_don_vi'::regclass,
                      'ql_don_vi.ho_so_nghiep_vu'::regclass) AND relrowsecurity;
        dat := v_cnt = 3; chi_tiet := v_cnt || '/3 bảng bật RLS';
        RETURN NEXT;
    END IF;
END $f$;

-- ----------------------------------------------------------------
-- 6. QUYEN
-- ----------------------------------------------------------------
GRANT USAGE ON SCHEMA ql_he_thong, ql_kiem_thu TO managergis_app;
GRANT SELECT ON ql_he_thong.mv_thong_ke_loai_xa TO managergis_app;
GRANT EXECUTE ON FUNCTION ql_he_thong.fn_lam_moi_thong_ke(), ql_he_thong.fn_kich_thuoc_bang(),
    ql_he_thong.fn_danh_sach_chi_muc(), ql_he_thong.fn_do_hieu_nang(),
    ql_kiem_thu.fn_chay_kiem_thu() TO managergis_app;
GRANT SELECT ON ql_don_vi.nhat_ky_thay_doi TO managergis_app;

COMMIT;
