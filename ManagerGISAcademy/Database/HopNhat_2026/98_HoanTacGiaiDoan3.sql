-- ================================================================
-- 98_HoanTacGiaiDoan3.sql
-- Go lop tuong thich (60, 61). Giu nguyen ket qua giai doan 1-2.
-- Ung dung phai tro lai CSDL hanoi_military_gis sau khi chay.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

DROP TRIGGER IF EXISTS trg_ho_so_ung_dung_ghi ON public.don_vi_quan_doi;
DROP VIEW IF EXISTS public.don_vi_quan_doi;
DROP VIEW IF EXISTS public.ranh_gioi_ha_noi;
DROP TABLE IF EXISTS public.nhat_ky_don_vi_quan_doi;

DROP FUNCTION IF EXISTS ql_don_vi.fn_ho_so_ung_dung_ghi();
DROP FUNCTION IF EXISTS ql_don_vi.fn_json_nhat_ky(JSONB);
DROP FUNCTION IF EXISTS ql_don_vi.fn_chuan_hoa_dia_ban(TEXT);

DROP TABLE IF EXISTS ql_don_vi.ho_so_nghiep_vu;
DROP TABLE IF EXISTS ql_don_vi.anh_xa_loai_nhap;

-- Don vi da xoa mem tu ung dung -> tra ve dang quan ly.
UPDATE ql_don_vi.don_vi_quan_doi
SET trang_thai_quan_ly = 'dang_quan_ly'
WHERE trang_thai_quan_ly = 'luu_tru'
  AND nguon_tham_khao <> 'Nhập từ ứng dụng ManagerGISAcademy';

-- Don vi tao moi tu ung dung (neu co) bi xoa han.
DELETE FROM ql_don_vi.don_vi_quan_doi
WHERE nguon_tham_khao = 'Nhập từ ứng dụng ManagerGISAcademy';

ALTER TABLE ql_don_vi.loai_don_vi DROP COLUMN IF EXISTS cap_mac_dinh_id;

DELETE FROM ql_don_vi.loai_don_vi
WHERE ma_loai IN ('TIEU_DOAN', 'CO_QUAN', 'DON_VI_KHAC')
  AND NOT EXISTS (SELECT 1 FROM ql_don_vi.don_vi_quan_doi d
                   WHERE d.loai_don_vi_id = loai_don_vi.id);

DELETE FROM ql_don_vi.cap_don_vi
WHERE ma_cap IN ('CAP_TIEU_DOAN', 'CAP_KHAC')
  AND NOT EXISTS (SELECT 1 FROM ql_don_vi.don_vi_quan_doi d
                   WHERE d.cap_don_vi_id = cap_don_vi.id);

COMMIT;
