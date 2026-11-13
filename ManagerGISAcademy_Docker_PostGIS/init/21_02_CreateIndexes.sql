-- ================================================================
-- 02_CreateIndexes.sql | ManagerGISAcademy / HanoiMilitaryGIS
-- Chay sau 01_CreateTables.sql tren CUNG database.
-- Khong xoa ban ghi; co the chay lai bang IF NOT EXISTS.
-- ================================================================

BEGIN;

-- ================================================================
-- 1. CHI MUC KHONG GIAN GIST
-- ================================================================

-- Chi muc hinh hoc EPSG:32648.
-- Phuc vu ST_DWithin, ST_Distance va tim gan nhat (<->).
CREATE INDEX IF NOT EXISTS idx_donvi_geom_32648
ON public.don_vi_quan_doi
USING GIST (geom_32648);

-- Chi muc hinh hoc WGS84 EPSG:4326.
-- Phuc vu truy van khong gian theo toa do dia ly.
CREATE INDEX IF NOT EXISTS idx_donvi_geom_4326
ON public.don_vi_quan_doi
USING GIST (geom_4326);

-- ================================================================
-- 2. CHI MUC THUOC TINH
-- ================================================================

-- Tim kiem theo quan, binh chung.
CREATE INDEX IF NOT EXISTS idx_donvi_quan_binh_chung
ON public.don_vi_quan_doi (quan_binh_chung);

-- Tim kiem theo quan/huyen.
CREATE INDEX IF NOT EXISTS idx_donvi_quan_huyen
ON public.don_vi_quan_doi (quan_huyen);

-- Tim kiem theo loai don vi.
CREATE INDEX IF NOT EXISTS idx_donvi_loai_don_vi
ON public.don_vi_quan_doi (loai_don_vi);

-- ================================================================
-- 3. MA DON VI
-- ================================================================

-- Cot ma_don_vi da co rang buoc UNIQUE trong file 01.
-- PostgreSQL tu dong tao unique index cho cot nay.
-- Khong can tao them chi muc trung lap.

COMMIT;

-- ================================================================
-- 4. CAP NHAT THONG KE TRUY VAN
-- ================================================================

ANALYZE public.don_vi_quan_doi;

-- ================================================================
-- 5. LENH KIEM TRA (CHAY RIENG KHI CAN)
-- ================================================================

-- SELECT
--     indexname,
--     indexdef
-- FROM pg_indexes
-- WHERE schemaname = 'public'
--   AND tablename = 'don_vi_quan_doi'
-- ORDER BY indexname;
