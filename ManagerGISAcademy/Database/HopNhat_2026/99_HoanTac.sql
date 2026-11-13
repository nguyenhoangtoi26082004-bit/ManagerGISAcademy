-- ================================================================
-- 99_HoanTac.sql
-- Dua CSDL HQTCSDL ve trang thai TRUOC khi chay 10 -> 50.
-- Chi go nhung gi bo hop nhat da them; 43 don vi goc giu nguyen
-- (toa do cua chung khong doi trong qua trinh hop nhat).
-- ================================================================

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- 1. Tra view cu ve dinh nghia ban dau.
CREATE OR REPLACE VIEW ql_don_vi.v_don_vi_ban_do AS
SELECT dv.id, dv.ma_don_vi, dv.ten_don_vi, dv.ky_hieu_bien_so,
       cdv.ten_cap, ldv.ten_loai,
       xp.ma_xa_phuong, xp.ten_xa_phuong, xp.loai AS loai_xa_phuong,
       dv.tinh_thanh_pho, dv.dia_chi_hien_hanh, dv.nguon_tham_khao,
       dv.muc_do_xac_minh, dv.trang_thai_quan_ly, dv.geom
FROM ql_don_vi.don_vi_quan_doi dv
JOIN ql_don_vi.cap_don_vi   cdv ON cdv.id = dv.cap_don_vi_id
JOIN ql_don_vi.loai_don_vi  ldv ON ldv.id = dv.loai_don_vi_id
JOIN ql_don_vi.dm_xa_phuong xp  ON xp.id  = dv.xa_phuong_id
WHERE dv.tinh_thanh_pho::text = 'Thành phố Hà Nội'
  AND dv.geom IS NOT NULL;

-- 2. Go view moi.
DROP VIEW IF EXISTS ql_don_vi.v_thong_ke_vi_tri;
DROP VIEW IF EXISTS ql_don_vi.v_kiem_tra_vi_tri;
DROP VIEW IF EXISTS ql_don_vi.v_ban_do_don_vi;

-- 3. Xoa don vi da hop nhat (vi tri xoa day chuyen).
DELETE FROM ql_don_vi.don_vi_quan_doi
WHERE ma_don_vi LIKE 'HN-BCHQS-%'
   OR ma_don_vi IN ('HN-BV108-001', 'HN-BV354-001',
                    'HN-LD144-001', 'HN-E916-001');

-- 4. Go trigger, bang vi tri va ham.
DROP TRIGGER IF EXISTS trg_don_vi_khoa_geom         ON ql_don_vi.don_vi_quan_doi;
DROP TRIGGER IF EXISTS trg_don_vi_kiem_tra_xac_minh ON ql_don_vi.don_vi_quan_doi;

DROP TABLE IF EXISTS ql_don_vi.vi_tri_don_vi;

DROP FUNCTION IF EXISTS ql_don_vi.fn_vi_tri_truoc_ghi();
DROP FUNCTION IF EXISTS ql_don_vi.fn_vi_tri_cam_xoa();
DROP FUNCTION IF EXISTS ql_don_vi.fn_vi_tri_dong_bo_don_vi();
DROP FUNCTION IF EXISTS ql_don_vi.fn_don_vi_khoa_geom();
DROP FUNCTION IF EXISTS ql_don_vi.fn_don_vi_kiem_tra_xac_minh();
DROP FUNCTION IF EXISTS ql_don_vi.fn_ranh_gioi_ha_noi();

-- 5. Go rang buoc va cot moi tren bang don vi.
ALTER TABLE ql_don_vi.don_vi_quan_doi
    DROP CONSTRAINT IF EXISTS fk_dv_muc_do_xac_minh,
    DROP CONSTRAINT IF EXISTS fk_dv_trang_thai_quan_ly,
    DROP CONSTRAINT IF EXISTS ck_dv_ma_don_vi,
    DROP CONSTRAINT IF EXISTS ck_dv_ten_khong_rong,
    DROP CONSTRAINT IF EXISTS ck_dv_khong_tu_lam_cap_tren,
    DROP CONSTRAINT IF EXISTS ck_dv_ngay_xac_minh,
    DROP COLUMN IF EXISTS bieu_tuong_ma;

ALTER TABLE ql_don_vi.loai_don_vi
    DROP COLUMN IF EXISTS bieu_tuong_ma;

-- 6. Go cap/loai bo sung (luc nay khong con don vi nao dung).
DELETE FROM ql_don_vi.loai_don_vi WHERE ma_loai IN ('BENH_VIEN_QY', 'BCHQS_XA');
DELETE FROM ql_don_vi.cap_don_vi  WHERE ma_cap  IN ('CAP_BENH_VIEN', 'CAP_XA');

-- 7. Go danh muc.
DROP TABLE IF EXISTS ql_don_vi.bieu_tuong;
DROP TABLE IF EXISTS ql_don_vi.dm_phuong_phap_dinh_vi;
DROP TABLE IF EXISTS ql_don_vi.dm_trang_thai_quan_ly;
DROP TABLE IF EXISTS ql_don_vi.dm_muc_do_xac_minh;

COMMIT;
