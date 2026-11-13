
-- ================================================================
-- 03_SpatialFunctions.sql
-- ManagerGISAcademy / HanoiMilitaryGIS
--
-- Cac ham phan tich khong gian PostGIS.
-- Chay sau 01_CreateTables.sql va 02_CreateIndexes.sql.
--
-- Khong tao, sua hoac xoa ban ghi don vi.
-- Khong thay doi toa do da luu.
--
-- EPSG:4326  : WGS84, kinh do/vi do.
-- EPSG:32648 : WGS84 / UTM zone 48N, don vi met.
-- ================================================================

BEGIN;

-- ================================================================
-- 1. PHAN TICH VUNG DEM
-- Tim cac don vi trong ban kinh R met quanh don vi tam.
-- Khong tra lai chinh don vi tam.
-- ================================================================

CREATE OR REPLACE FUNCTION
public.sp_phan_tich_vung_dem
(
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

        ST_Distance(
            u.geom_32648,
            t.geom_32648
        )::DOUBLE PRECISION AS khoang_cach_met,

        ST_AsText(
            u.geom_4326
        )::TEXT AS geom_wkt

    FROM public.don_vi_quan_doi AS t

    JOIN public.don_vi_quan_doi AS u
        ON u.ma_don_vi <> t.ma_don_vi

    WHERE t.ma_don_vi = p_ma_don_vi

        AND t.geom_32648 IS NOT NULL
        AND u.geom_32648 IS NOT NULL

        AND p_ban_kinh_met > 0
        AND p_ban_kinh_met <= 35000

        AND ST_DWithin(
            u.geom_32648,
            t.geom_32648,
            p_ban_kinh_met
        )

    ORDER BY
        khoang_cach_met,
        u.ma_don_vi;

$phan_tich_vung_dem$;

-- ================================================================
-- 2. TIM K DON VI GAN NHAT
--
-- Dau vao:
-- p_vi_do   : vi do WGS84
-- p_kinh_do : kinh do WGS84
-- p_k       : so doi tuong can tim (1 - 100)
--
-- Su dung toan tu <-> de ho tro GIST KNN.
-- ================================================================

CREATE OR REPLACE FUNCTION
public.sp_tim_k_don_vi_gan_nhat
(
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

        (
            ST_Distance(
                u.geom_32648,

                ST_Transform(
                    ST_SetSRID(
                        ST_MakePoint(
                            p_kinh_do,
                            p_vi_do
                        ),
                        4326
                    ),
                    32648
                )
            ) / 1000.0
        )::DOUBLE PRECISION AS khoang_cach_km,

        ST_AsText(
            u.geom_4326
        )::TEXT AS geom_wkt

    FROM public.don_vi_quan_doi AS u

    WHERE u.geom_32648 IS NOT NULL

        AND p_k BETWEEN 1 AND 100

        AND p_kinh_do BETWEEN 102.0 AND 108.0

        AND p_vi_do BETWEEN 0.0 AND 84.0

    ORDER BY

        u.geom_32648 <->

        ST_Transform(
            ST_SetSRID(
                ST_MakePoint(
                    p_kinh_do,
                    p_vi_do
                ),
                4326
            ),
            32648
        ),

        u.ma_don_vi

    LIMIT p_k;

$tim_k_gan_nhat$;

-- ================================================================
-- 3. TINH KHOANG CACH GIUA HAI DON VI
--
-- Ket qua:
-- Khoang cach phang theo met.
-- Tra NULL neu khong tim thay hoac thieu hinh hoc.
-- ================================================================

CREATE OR REPLACE FUNCTION
public.fn_khoang_cach_hai_don_vi
(
    p_ma_a VARCHAR(50),
    p_ma_b VARCHAR(50)
)

RETURNS DOUBLE PRECISION

LANGUAGE sql
STABLE
SECURITY INVOKER

AS $khoang_cach_hai_don_vi$

    SELECT

        ST_Distance(
            a.geom_32648,
            b.geom_32648
        )::DOUBLE PRECISION

    FROM public.don_vi_quan_doi AS a

    JOIN public.don_vi_quan_doi AS b
        ON b.ma_don_vi = p_ma_b

    WHERE a.ma_don_vi = p_ma_a

        AND a.geom_32648 IS NOT NULL

        AND b.geom_32648 IS NOT NULL

    LIMIT 1;

$khoang_cach_hai_don_vi$;

-- ================================================================
-- 4. TAO VUNG DEM DANG WKT
--
-- Tao buffer bang met tren EPSG:32648.
-- Chuyen ve EPSG:4326 truoc khi xuat WKT.
--
-- Ket qua:
-- Polygon WKT hoac NULL neu khong co doi tuong.
-- ================================================================

CREATE OR REPLACE FUNCTION
public.fn_vung_dem_don_vi_wkt
(
    p_ma_don_vi VARCHAR(50),
    p_ban_kinh_met DOUBLE PRECISION
)

RETURNS TEXT

LANGUAGE sql
STABLE
SECURITY INVOKER

AS $vung_dem_wkt$

    SELECT

        ST_AsText(

            ST_Transform(

                ST_Buffer(
                    u.geom_32648,
                    p_ban_kinh_met,
                    8
                ),

                4326
            )
        )::TEXT

    FROM public.don_vi_quan_doi AS u

    WHERE u.ma_don_vi = p_ma_don_vi

        AND u.geom_32648 IS NOT NULL

        AND p_ban_kinh_met > 0

        AND p_ban_kinh_met <= 35000

    LIMIT 1;

$vung_dem_wkt$;

COMMIT;

-- ================================================================
-- 5. KIEM TRA CAC HAM DA TAO
-- Chay rieng khi can.
-- ================================================================

-- SELECT
--     p.proname AS ten_ham
-- FROM pg_proc AS p
-- JOIN pg_namespace AS n
--     ON n.oid = p.pronamespace
-- WHERE n.nspname = 'public'
--   AND p.proname IN
--   (
--       'sp_phan_tich_vung_dem',
--       'sp_tim_k_don_vi_gan_nhat',
--       'fn_khoang_cach_hai_don_vi',
--       'fn_vung_dem_don_vi_wkt'
--   )
-- ORDER BY p.proname;

-- ================================================================
-- 6. VI DU KIEM TRA KHONG GIAN
-- Chi su dung du lieu da duoc phep truy cap.
-- ================================================================

-- Tim 5 doi tuong gan mot toa do THU NGHIEM.
--
-- SELECT *
-- FROM public.sp_tim_k_don_vi_gan_nhat(
--     21.0278,
--     105.8342,
--     5
-- );

-- Tinh khoang cach hai ma mau (neu ton tai).
--
-- SELECT public.fn_khoang_cach_hai_don_vi(
--     'MA_THU_NGHIEM_A',
--     'MA_THU_NGHIEM_B'
-- );

-- Phan tich ban kinh 1000 met.
--
-- SELECT *
-- FROM public.sp_phan_tich_vung_dem(
--     'MA_THU_NGHIEM_A',
--     1000
-- );

-- Lay WKT vung dem.
--
-- SELECT public.fn_vung_dem_don_vi_wkt(
--     'MA_THU_NGHIEM_A',
--     1000
-- );
