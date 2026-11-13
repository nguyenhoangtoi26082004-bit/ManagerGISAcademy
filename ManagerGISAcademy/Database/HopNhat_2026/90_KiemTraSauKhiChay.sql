-- ================================================================
-- 90_KiemTraSauKhiChay.sql  (CHI DOC)
-- Chay sau 10 -> 50 de doi chieu ket qua.
-- ================================================================

-- 1. Tong so don vi tren ban do thong nhat. Mong doi: 43 + 126 + 4 = 173.
SELECT count(*) AS tong_don_vi FROM ql_don_vi.v_ban_do_don_vi;

-- 2. Theo loai va chat luong vi tri.
SELECT * FROM ql_don_vi.v_thong_ke_vi_tri ORDER BY tong DESC;

-- 3. Tong hop loi/canh bao.
SELECT loai_loi, muc_do, count(*)
FROM ql_don_vi.v_kiem_tra_vi_tri
GROUP BY 1, 2
ORDER BY 2, 3 DESC;

-- 4. View cu khong doi so luong voi ung dung hien tai.
--    Mong doi: 47 (43 cu + 4 don vi moi, khong gom BCHQS xa).
SELECT count(*) AS v_don_vi_ban_do FROM ql_don_vi.v_don_vi_ban_do;

-- 5. Thu rang buoc (mong doi: LOI "ngoai vung Ha Noi / dao truc").
-- BEGIN;
-- INSERT INTO ql_don_vi.vi_tri_don_vi
--     (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, nguon)
-- SELECT id, 4326, 21.03, 105.85, 'GNSS_CAM_TAY', 'thu dao truc'
-- FROM ql_don_vi.don_vi_quan_doi LIMIT 1;
-- ROLLBACK;
