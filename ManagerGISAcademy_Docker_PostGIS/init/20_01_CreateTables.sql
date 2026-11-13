-- ================================================================
-- 01_CreateTables.sql | ManagerGISAcademy / HanoiMilitaryGIS
-- Tao bang don_vi_quan_doi va trigger dong bo hinh hoc.
-- Chay tren DUNG database da khai bao trong App.config.
-- PostgreSQL + PostGIS; khong xoa bang hay du lieu cu.
-- ================================================================

BEGIN;

-- Tai khoan tao extension can co quyen phu hop.
CREATE EXTENSION IF NOT EXISTS postgis;

CREATE TABLE IF NOT EXISTS public.don_vi_quan_doi
(
    id                 BIGSERIAL PRIMARY KEY,
    ma_don_vi          VARCHAR(50) NOT NULL UNIQUE,
    ten_don_vi         VARCHAR(255) NOT NULL,
    ten_viet_tat       VARCHAR(100),
    loai_don_vi        VARCHAR(100) NOT NULL,
    quan_binh_chung    VARCHAR(100) NOT NULL,
    don_vi_cap_tren    VARCHAR(255) NOT NULL,
    quan_huyen         VARCHAR(100) NOT NULL,
    dia_chi            TEXT NOT NULL,

    -- WGS84, EPSG:4326: kinh do (X), vi do (Y), don vi do.
    kinh_do            DOUBLE PRECISION NOT NULL,
    vi_do              DOUBLE PRECISION NOT NULL,

    -- Ten cot tu luoc do cu. GIA TRI DUOC TINH THEO EPSG:32648,
    -- tuc WGS84 / UTM zone 48N, khong phai datum VN-2000.
    x_vn2000           DOUBLE PRECISION NOT NULL,
    y_vn2000           DOUBLE PRECISION NOT NULL,

    chi_huy_truong     VARCHAR(150),
    chinh_uy           VARCHAR(150),
    so_dien_thoai      VARCHAR(50) NOT NULL,
    trang_thai_sscd    VARCHAR(100),
    do_mat             VARCHAR(50),
    nhiem_vu_chinh     TEXT,
    vu_khi_trang_bi    TEXT,

    geom_4326          geometry(Point, 4326),
    geom_32648         geometry(Point, 32648),
    ngay_cap_nhat      TIMESTAMP WITHOUT TIME ZONE NOT NULL
                       DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT ck_donvi_toa_do_utm48n
        CHECK (
            kinh_do BETWEEN 102.0 AND 108.0
            AND vi_do BETWEEN 0.0 AND 84.0
        )
);

-- Neu bang da ton tai nhung khong dung luoc do, dung tai day thay vi
-- im lang chay tiep roi gay loi o DAL. Khong tu dong sua/xoa du lieu.
DO $kiem_tra_bang$
DECLARE
    cot_thieu TEXT;
BEGIN
    SELECT string_agg(
        yeu_cau.ten,
        ', '
        ORDER BY yeu_cau.ten
    )
    INTO cot_thieu
    FROM unnest(ARRAY[
        'id',
        'ma_don_vi',
        'ten_don_vi',
        'ten_viet_tat',
        'loai_don_vi',
        'quan_binh_chung',
        'don_vi_cap_tren',
        'quan_huyen',
        'dia_chi',
        'kinh_do',
        'vi_do',
        'x_vn2000',
        'y_vn2000',
        'chi_huy_truong',
        'chinh_uy',
        'so_dien_thoai',
        'trang_thai_sscd',
        'do_mat',
        'nhiem_vu_chinh',
        'vu_khi_trang_bi',
        'geom_4326',
        'geom_32648',
        'ngay_cap_nhat'
    ]) AS yeu_cau(ten)
    WHERE NOT EXISTS (
        SELECT 1
        FROM information_schema.columns AS cot
        WHERE cot.table_schema = 'public'
          AND cot.table_name = 'don_vi_quan_doi'
          AND cot.column_name = yeu_cau.ten
    );

    IF cot_thieu IS NOT NULL THEN
        RAISE EXCEPTION
            'Bang public.don_vi_quan_doi da co nhung thieu cot: %. Can doi chieu va migration, khong DROP bang.',
            cot_thieu;
    END IF;

    -- Hai cot hinh hoc phai la Point voi SRID dung nhu DAL.
    IF NOT EXISTS (
        SELECT 1
        FROM geometry_columns
        WHERE f_table_schema = 'public'
          AND f_table_name = 'don_vi_quan_doi'
          AND f_geometry_column = 'geom_4326'
          AND type = 'POINT'
          AND srid = 4326
    )
    OR NOT EXISTS (
        SELECT 1
        FROM geometry_columns
        WHERE f_table_schema = 'public'
          AND f_table_name = 'don_vi_quan_doi'
          AND f_geometry_column = 'geom_32648'
          AND type = 'POINT'
          AND srid = 32648
    )
    THEN
        RAISE EXCEPTION
            'Hai cot geom_4326 va geom_32648 phai la geometry(Point,4326/32648). Kiem tra schema hien co.';
    END IF;
END;
$kiem_tra_bang$;

-- ================================================================
-- HAM DONG BO TOA DO VA HINH HOC
-- ================================================================

CREATE OR REPLACE FUNCTION public.fn_trg_cap_nhat_hinh_hoc()
RETURNS TRIGGER
LANGUAGE plpgsql
AS $dong_bo_hinh_hoc$
BEGIN

    -- Kiem tra kinh do va vi do WGS84.
    IF NEW.kinh_do IS NULL
       OR NEW.vi_do IS NULL
       OR NOT (NEW.kinh_do BETWEEN 102.0 AND 108.0)
       OR NOT (NEW.vi_do BETWEEN 0.0 AND 84.0)
    THEN
        RAISE EXCEPTION
            'Kinh do/vi do khong hop le cho UTM 48N (102..108 E; 0..84 N).';
    END IF;

    -- Tao hinh hoc EPSG:4326.
    NEW.geom_4326 := ST_SetSRID(
        ST_MakePoint(
            NEW.kinh_do,
            NEW.vi_do
        ),
        4326
    );

    -- Chuyen hinh hoc sang EPSG:32648.
    NEW.geom_32648 := ST_Transform(
        NEW.geom_4326,
        32648
    );

    -- Dong bo hai cot toa do phang.
    NEW.x_vn2000 := ST_X(NEW.geom_32648);
    NEW.y_vn2000 := ST_Y(NEW.geom_32648);

    -- Cap nhat thoi gian.
    NEW.ngay_cap_nhat := CURRENT_TIMESTAMP;

    RETURN NEW;
END;
$dong_bo_hinh_hoc$;

-- ================================================================
-- TRIGGER CAP NHAT HINH HOC
-- ================================================================

DROP TRIGGER IF EXISTS trg_cap_nhat_geom
ON public.don_vi_quan_doi;

CREATE TRIGGER trg_cap_nhat_geom
BEFORE INSERT OR UPDATE
ON public.don_vi_quan_doi
FOR EACH ROW
EXECUTE FUNCTION public.fn_trg_cap_nhat_hinh_hoc();

COMMIT;

-- ================================================================
-- LENH KIEM TRA (CHAY RIENG KHI CAN)
-- ================================================================

-- SELECT PostGIS_Version();

-- SELECT COUNT(*)
-- FROM public.don_vi_quan_doi;
