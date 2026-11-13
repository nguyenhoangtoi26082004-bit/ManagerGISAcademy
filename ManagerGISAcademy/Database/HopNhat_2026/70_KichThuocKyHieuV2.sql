-- ================================================================
-- 70_KichThuocKyHieuV2.sql
-- Bo ky hieu v2 (12/11/2026): moi loai CUNG kich thuoc 22 px,
-- Ban CHQS xa/phuong (126 diem, cap thap nhat) 18 px.
-- Phan biet loai bang MAU + CHU, khong bang do lon.
-- thu_tu_ve giu nguyen (cap cao ve de len cap thap).
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

UPDATE ql_don_vi.bieu_tuong
SET kich_thuoc_px = CASE ma
                        WHEN 'BCHQS_XA' THEN 18
                        ELSE 22
                    END;

UPDATE ql_don_vi.bieu_tuong SET ten = 'Sư đoàn (f)'    WHERE ma = 'SU_DOAN';
UPDATE ql_don_vi.bieu_tuong SET ten = 'Trung đoàn (e)' WHERE ma = 'TRUNG_DOAN';

COMMIT;

-- Hoan tac (kich thuoc ban v1):
-- UPDATE ql_don_vi.bieu_tuong SET kich_thuoc_px = CASE ma
--   WHEN 'BQP' THEN 40 WHEN 'TONG_CUC' THEN 36 WHEN 'BO_TU_LENH' THEN 36
--   WHEN 'BINH_CHUNG' THEN 34 WHEN 'HOC_VIEN' THEN 32 WHEN 'TRUONG' THEN 30
--   WHEN 'BENH_VIEN' THEN 30 WHEN 'SU_DOAN' THEN 30 WHEN 'LU_DOAN' THEN 28
--   WHEN 'TRUNG_DOAN' THEN 28 WHEN 'BCHQS_XA' THEN 22 ELSE 24 END;
