-- ================================================================
-- 81_BanDoChuyenDe.sql
-- Ban do chuyen de theo phuong/xa + vung phuc vu Voronoi.
--
--   ql_don_vi.fn_ban_do_chuyen_de(che_do)
--     'MAT_DO'         so don vi dang quan ly trong moi phuong/xa
--     'XAC_MINH'       ty le don vi da co VI TRI THAT (chat luong du lieu)
--     'KHOANG_CACH_BV' khoang cach (km, duong chim bay) tu tam phuong/xa
--                      toi benh vien quan y gan nhat - toan tu KNN <->
--     'VORONOI_BV'     vung phuc vu cua tung benh vien quan y
--                      (ST_VoronoiPolygons cat theo ranh gioi Ha Noi)
--
-- Tra ve: ma, ten, gia_tri, lop (so thu tu lop mau), nhan_lop
-- (chu giai) va wkt (EPSG:32648, don gian hoa 20 m; vong ngoai THUAN chieu
-- kim dong ho theo quy uoc Shapefile/MapWinGIS - ST_ForcePolygonCW).
-- PHAN LOP DUOC TINH TRONG SQL -> ung dung chi to mau theo "lop".
--
-- Chay bang postgres tren HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

-- Benh vien quan y dung cho phan tich: loai BENH_VIEN_QY va Hoc vien
-- Quan y (co Benh vien Quan y 103). Tach thanh view de de mo rong.
CREATE OR REPLACE VIEW ql_don_vi.v_co_so_quan_y AS
SELECT b.id, b.ma_don_vi, b.ten_don_vi, b.geom, b.la_vi_tri_thuc
FROM ql_don_vi.v_ban_do_don_vi b
WHERE b.ma_loai = 'BENH_VIEN_QY'
   OR b.ma_don_vi = 'HN-HVQY-001';

COMMENT ON VIEW ql_don_vi.v_co_so_quan_y IS
    'Co so quan y dung cho phan tich khoang cach / vung phuc vu.';

CREATE OR REPLACE FUNCTION ql_don_vi.fn_ban_do_chuyen_de(p_che_do TEXT)
RETURNS TABLE (
    ma        TEXT,
    ten       TEXT,
    gia_tri   DOUBLE PRECISION,
    lop       INTEGER,
    nhan_lop  TEXT,
    wkt       TEXT)
LANGUAGE plpgsql
STABLE
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
#variable_conflict use_column
BEGIN
    IF NOT has_table_privilege(session_user, 'public.don_vi_quan_doi', 'SELECT') THEN
        RAISE EXCEPTION 'Tài khoản % không có quyền xem dữ liệu đơn vị.', session_user;
    END IF;

    -- ------------------------------------------------------------
    -- 1. MAT DO DON VI THEO PHUONG/XA
    -- ------------------------------------------------------------
    IF p_che_do = 'MAT_DO' THEN
        RETURN QUERY
        WITH dem AS (
            SELECT x.id, x.ma_xa_phuong, x.ten_xa_phuong, x.loai, x.geom,
                   count(b.id) AS n
            FROM ql_don_vi.dm_xa_phuong x
            LEFT JOIN ql_don_vi.v_ban_do_don_vi b ON b.ma_xa_phuong = x.ma_xa_phuong
            GROUP BY x.id
        )
        SELECT d.ma_xa_phuong::text,
               (CASE d.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END || d.ten_xa_phuong)::text,
               d.n::float8,
               CASE WHEN d.n = 0 THEN 0 WHEN d.n = 1 THEN 1 WHEN d.n = 2 THEN 2
                    WHEN d.n <= 4 THEN 3 ELSE 4 END,
               CASE WHEN d.n = 0 THEN 'Không có đơn vị' WHEN d.n = 1 THEN '1 đơn vị'
                    WHEN d.n = 2 THEN '2 đơn vị' WHEN d.n <= 4 THEN '3 – 4 đơn vị'
                    ELSE '≥ 5 đơn vị' END,
               ST_AsText(ST_ForcePolygonCW(ST_SimplifyPreserveTopology(d.geom, 20)))
        FROM dem d;

    -- ------------------------------------------------------------
    -- 2. MUC DO XAC MINH VI TRI (chat luong du lieu khong gian)
    -- ------------------------------------------------------------
    ELSIF p_che_do = 'XAC_MINH' THEN
        RETURN QUERY
        WITH tk AS (
            SELECT x.ma_xa_phuong, x.ten_xa_phuong, x.loai, x.geom,
                   count(b.id) AS n,
                   count(b.id) FILTER (WHERE b.la_vi_tri_thuc) AS n_thuc
            FROM ql_don_vi.dm_xa_phuong x
            LEFT JOIN ql_don_vi.v_ban_do_don_vi b ON b.ma_xa_phuong = x.ma_xa_phuong
            GROUP BY x.id
        )
        SELECT t.ma_xa_phuong::text,
               (CASE t.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END || t.ten_xa_phuong)::text,
               CASE WHEN t.n = 0 THEN NULL ELSE round(100.0 * t.n_thuc / t.n, 1)::float8 END,
               CASE WHEN t.n = 0 THEN 0
                    WHEN t.n_thuc = 0 THEN 1
                    WHEN t.n_thuc < t.n / 2.0 THEN 2
                    WHEN t.n_thuc < t.n THEN 3
                    ELSE 4 END,
               CASE WHEN t.n = 0 THEN 'Không có đơn vị'
                    WHEN t.n_thuc = 0 THEN 'Chưa xác minh (0%)'
                    WHEN t.n_thuc < t.n / 2.0 THEN 'Dưới 50%'
                    WHEN t.n_thuc < t.n THEN '50% – dưới 100%'
                    ELSE 'Đã xác minh 100%' END,
               ST_AsText(ST_ForcePolygonCW(ST_SimplifyPreserveTopology(t.geom, 20)))
        FROM tk t;

    -- ------------------------------------------------------------
    -- 3. KHOANG CACH TOI CO SO QUAN Y GAN NHAT (KNN <->)
    -- ------------------------------------------------------------
    ELSIF p_che_do = 'KHOANG_CACH_BV' THEN
        RETURN QUERY
        SELECT x.ma_xa_phuong::text,
               (CASE x.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END || x.ten_xa_phuong)::text,
               round((g.kc / 1000.0)::numeric, 2)::float8,
               CASE WHEN g.kc < 3000 THEN 0 WHEN g.kc < 6000 THEN 1
                    WHEN g.kc < 12000 THEN 2 WHEN g.kc < 25000 THEN 3 ELSE 4 END,
               CASE WHEN g.kc < 3000 THEN 'Dưới 3 km' WHEN g.kc < 6000 THEN '3 – 6 km'
                    WHEN g.kc < 12000 THEN '6 – 12 km' WHEN g.kc < 25000 THEN '12 – 25 km'
                    ELSE 'Từ 25 km' END,
               ST_AsText(ST_ForcePolygonCW(ST_SimplifyPreserveTopology(x.geom, 20)))
        FROM ql_don_vi.dm_xa_phuong x
        CROSS JOIN LATERAL (
            SELECT ST_Distance(ST_PointOnSurface(x.geom), q.geom) AS kc
            FROM ql_don_vi.v_co_so_quan_y q
            ORDER BY q.geom <-> ST_PointOnSurface(x.geom)
            LIMIT 1
        ) g;

    -- ------------------------------------------------------------
    -- 4. VUNG PHUC VU VORONOI CUA CO SO QUAN Y
    -- ------------------------------------------------------------
    ELSIF p_che_do = 'VORONOI_BV' THEN
        RETURN QUERY
        WITH hn AS (
            SELECT ql_don_vi.fn_ranh_gioi_ha_noi() AS geom
        ),
        o AS (
            SELECT (ST_Dump(ST_VoronoiPolygons(
                        ST_Collect(q.geom), 0,
                        ST_Expand((SELECT geom FROM hn), 5000)))).geom AS geom
            FROM ql_don_vi.v_co_so_quan_y q
        ),
        vung AS (
            SELECT q.ma_don_vi, q.ten_don_vi,
                   ST_Intersection(o.geom, (SELECT geom FROM hn)) AS geom
            FROM o
            JOIN ql_don_vi.v_co_so_quan_y q ON ST_Contains(o.geom, q.geom)
        )
        SELECT v.ma_don_vi::text,
               v.ten_don_vi::text,
               round((ST_Area(v.geom) / 1e6)::numeric, 1)::float8,
               (row_number() OVER (ORDER BY v.ten_don_vi) - 1)::int,
               (v.ten_don_vi || ' · ' || round((ST_Area(v.geom) / 1e6)::numeric, 0) || ' km²')::text,
               ST_AsText(ST_ForcePolygonCW(ST_SimplifyPreserveTopology(v.geom, 20)))
        FROM vung v
        WHERE NOT ST_IsEmpty(v.geom);

    ELSE
        RAISE EXCEPTION 'Chế độ bản đồ chuyên đề "%" không hợp lệ.', p_che_do;
    END IF;
END;
$f$;

REVOKE ALL ON FUNCTION ql_don_vi.fn_ban_do_chuyen_de(TEXT) FROM PUBLIC;
GRANT EXECUTE ON FUNCTION ql_don_vi.fn_ban_do_chuyen_de(TEXT) TO managergis_app;
GRANT SELECT ON ql_don_vi.v_co_so_quan_y TO managergis_app;

COMMIT;
