-- ================================================================
-- 50_PhanQuyen.sql
-- CSDL: HQTCSDL
--
-- Tai khoan ung dung managergis_app: CHI DOC qua view.
-- Khong cap quyen tren bang goc (nguyen tac quyen toi thieu).
-- ================================================================

BEGIN;

GRANT USAGE ON SCHEMA ql_don_vi TO managergis_app;

GRANT SELECT ON
    ql_don_vi.v_don_vi_ban_do,
    ql_don_vi.v_ban_do_don_vi,
    ql_don_vi.v_kiem_tra_vi_tri,
    ql_don_vi.v_thong_ke_vi_tri,
    ql_don_vi.dm_muc_do_xac_minh,
    ql_don_vi.dm_trang_thai_quan_ly,
    ql_don_vi.dm_phuong_phap_dinh_vi,
    ql_don_vi.bieu_tuong
TO managergis_app;

-- ----------------------------------------------------------------
-- CHUA BAT: chi mo khi ung dung co chuc nang nhap vi tri.
-- Ung dung chi THEM vi tri moi; trigger tu dong dong vi tri cu.
-- ----------------------------------------------------------------
-- GRANT INSERT ON ql_don_vi.vi_tri_don_vi TO managergis_app;
-- GRANT USAGE ON SEQUENCE ql_don_vi.vi_tri_don_vi_id_seq TO managergis_app;

COMMIT;
