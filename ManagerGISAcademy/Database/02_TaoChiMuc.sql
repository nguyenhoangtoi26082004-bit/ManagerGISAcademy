
-- ================================================================
-- 02_CreateIndexes.sql
-- ManagerGISAcademy / HanoiMilitaryGIS
--
-- Tao chi muc khong gian va thuoc tinh.
-- Chay sau 01_CreateTables.sql.
-- Khong xoa du lieu cu.
-- ================================================================

BEGIN;

-- ================================================================
-- 1. CHI MUC KHONG GIAN GIST
-- ================================================================

-- Hinh hoc EPSG:32648.
-- Phuc vu phan tich theo met va tim gan nhat.
CREATE INDEX IF NOT EXISTS
    idx_donvi_geom_32648
ON public.don_vi_quan_doi
USING GIST (geom_32648);

-- Hinh hoc WGS84, EPSG:4326.
CREATE INDEX IF NOT EXISTS
    idx_donvi_geom_4326
ON public.don_vi_quan_doi
USING GIST (geom_4326);

-- ================================================================
-- 2. CHI MUC THUOC TINH
-- ================================================================

-- Loc theo quan / binh chung.
CREATE INDEX IF NOT EXISTS
    idx_donvi_quan_binh_chung
ON public.don_vi_quan_doi
(quan_binh_chung);

-- Loc theo quan / huyen.
CREATE INDEX IF NOT EXISTS
    idx_donvi_quan_huyen
ON public.don_vi_quan_doi
(quan_huyen);

-- Loc theo loai don vi.
CREATE INDEX IF NOT EXISTS
    idx_donvi_loai_don_vi
ON public.don_vi_quan_doi
(loai_don_vi);

-- ================================================================
-- 3. MA DON VI
-- ================================================================

-- ma_don_vi da UNIQUE trong file 01.
-- PostgreSQL tu tao unique index.
-- Khong tao chi muc trung lap.

COMMIT;

-- ================================================================
-- 4. CAP NHAT THONG KE TRUY VAN
-- ================================================================

ANALYZE public.don_vi_quan_doi;

-- ================================================================
-- 5. KIEM TRA - CHAY RIENG KHI CAN
-- ================================================================

-- SELECT
--     indexname,
--     indexdef
-- FROM pg_indexes
-- WHERE schemaname = 'public'
--   AND tablename = 'don_vi_quan_doi'
-- ORDER BY indexname;
