-- ================================================================
-- 71_KichThuocKyHieuV3.sql
-- Bo ky hieu v3 (13/11/2026): MOI loai cung mot co 18 px
-- (ke ca Ban CHQS). Phan biet loai bang MAU + MA, khong bang co.
-- Hoan tac ve v2: chay lai 70_KichThuocKyHieuV2.sql.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

UPDATE ql_don_vi.bieu_tuong SET kich_thuoc_px = 18;

COMMIT;
