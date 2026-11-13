-- ================================================================
-- 03_SpatialFunctions.sql | ManagerGISAcademy / HanoiMilitaryGIS
-- Chay SAU 01 va 02, trong cung database PostgreSQL/PostGIS.
-- SQL thuan (khong phai class C#). Tat ca ket qua do dai tinh theo met
-- trong EPSG:32648 (WGS84/UTM 48N); WKT hien thi la EPSG:4326.
-- Khong tao du lieu don vi gia, khong thay doi cac ban ghi.
-- ================================================================

BEGIN;

-- 1) Danh sach don vi trong ban kinh tu mot don vi tam.
-- Khong tra lai don vi tam, loai hang khong co hinh hoc.
CREATE OR REPLACE FUNCTION public.sp_phan_tich_vung_dem(
    p_ma_don_vi VARCHAR(50),
    p_ban_kinh_met DOUBLE PRECISION
)
RETURNS TABLE
(
    ma_don_vi VARCHAR,
    ten_don_vi VARCHAR,
    quan_binh_chung VARCHAR,
    khoang_cach_met DOUBLE PRECISION,
    geom_wkt TEXT
)
LANGUAGE sql
STABLE
SECURITY INVOKER
AS $phan_tich_vung_dem$
    SELECT
        u.ma_don_vi::VARCHAR,
        u.ten_don_vi::VARCHAR,
        u.quan_binh_chung::VARCHAR,
        ST_Distance(u.geom_32648, t.geom_32648)::DOUBLE PRECISION AS khoang_cach_met,
        ST_AsText(u.geom_4326)::TEXT
    FROM public.don_vi_quan_doi AS t
    JOIN public.don_vi_quan_doi AS u
      ON u.ma_don_vi <> t.ma_don_vi
    WHERE t.ma_don_vi = p_ma_don_vi
      AND t.geom_32648 IS NOT NULL
      AND u.geom_32648 IS NOT NULL
      AND p_ban_kinh_met > 0
      AND p_ban_kinh_met <= 35000
      AND ST_DWithin(u.geom_32648, t.geom_32648, p_ban_kinh_met)
    ORDER BY 4, u.ma_don_vi;
$phan_tich_vung_dem$;

-- 2) Tim K don vi gan nhat tu mot diem WGS84.
-- KNN (<->) su dung chi muc GIST tren geom_32648 neu planner lua chon.
-- Chu ky tham so giu giong tai lieu cu: vi do TRUOC, kinh do SAU.
CREATE OR REPLACE FUNCTION public.sp_tim_k_don_vi_gan_nhat(
    p_vi_do DOUBLE PRECISION,
    p_kinh_do DOUBLE PRECISION,
    p_k INTEGER
)
RETURNS TABLE
(
    ma_don_vi VARCHAR,
    ten_don_vi VARCHAR,
    quan_binh_chung VARCHAR,
    khoang_cach_km DOUBLE PRECISION,
    geom_wkt TEXT
)
LANGUAGE sql
STABLE
SECURITY INVOKER
AS $tim_k_gan_nhat$
    SELECT
        u.ma_don_vi::VARCHAR,
        u.ten_don_vi::VARCHAR,
        u.quan_binh_chung::VARCHAR,
        (ST_Distance(u.geom_32648,
            ST_Transform(ST_SetSRID(ST_MakePoint(p_kinh_do, p_vi_do), 4326), 32648))
            / 1000.0)::DOUBLE PRECISION,
        ST_AsText(u.geom_4326)::TEXT
    FROM public.don_vi_quan_doi AS u
    WHERE u.geom_32648 IS NOT NULL
      AND p_k BETWEEN 1 AND 100
      AND p_kinh_do BETWEEN 102.0 AND 108.0
      AND p_vi_do BETWEEN 0.0 AND 84.0
    ORDER BY u.geom_32648 <->
        ST_Transform(ST_SetSRID(ST_MakePoint(p_kinh_do, p_vi_do), 4326), 32648),
        u.ma_don_vi
    LIMIT p_k;
$tim_k_gan_nhat$;

-- 3) Tinh khoang cach thuc te theo phep chieu phang UTM (met).
-- Tra NULL neu ma khong ton tai hoac thieu hinh hoc.
CREATE OR REPLACE FUNCTION public.fn_khoang_cach_hai_don_vi(
    p_ma_a VARCHAR(50),
    p_ma_b VARCHAR(50)
)
RETURNS DOUBLE PRECISION
LANGUAGE sql
STABLE
SECURITY INVOKER
AS $khoang_cach_hai_don_vi$
    SELECT ST_Distance(a.geom_32648, b.geom_32648)::DOUBLE PRECISION
    FROM public.don_vi_quan_doi AS a
    JOIN public.don_vi_quan_doi AS b
      ON b.ma_don_vi = p_ma_b
    WHERE a.ma_don_vi = p_ma_a
      AND a.geom_32648 IS NOT NULL
      AND b.geom_32648 IS NOT NULL
    LIMIT 1;
$khoang_cach_hai_don_vi$;

-- 4) Tao polygon vung dem quanh mot don vi, xuat WKT EPSG:4326.
-- Buffer duoc tinh theo met truoc, roi moi chuyen sang WGS84.
CREATE OR REPLACE FUNCTION public.fn_vung_dem_don_vi_wkt(
    p_ma_don_vi VARCHAR(50),
    p_ban_kinh_met DOUBLE PRECISION
)
RETURNS TEXT
LANGUAGE sql
STABLE
SECURITY INVOKER
AS $vung_dem_wkt$
    SELECT ST_AsText(
        ST_Transform(ST_Buffer(u.geom_32648, p_ban_kinh_met, 8), 4326)
    )::TEXT
    FROM public.don_vi_quan_doi AS u
    WHERE u.ma_don_vi = p_ma_don_vi
      AND u.geom_32648 IS NOT NULL
      AND p_ban_kinh_met > 0
      AND p_ban_kinh_met <= 35000
    LIMIT 1;
$vung_dem_wkt$;

COMMIT;

-- CAC LENH KIEM TRA (khong tao/sua/xoa don vi):
-- SELECT proname FROM pg_proc p JOIN pg_namespace n ON n.oid = p.pronamespace
-- WHERE n.nspname = 'public' AND proname IN
-- ('sp_phan_tich_vung_dem','sp_tim_k_don_vi_gan_nhat',
--  'fn_khoang_cach_hai_don_vi','fn_vung_dem_don_vi_wkt');
-- SELECT * FROM public.sp_tim_k_don_vi_gan_nhat(21.0278, 105.8342, 5);
-- Vi du toa do tren CHI de kiem thu, khong phai dia chi don vi thuc.
-- SELECT public.fn_vung_dem_don_vi_wkt('MA_DON_VI_CO_THAT', 5000);
