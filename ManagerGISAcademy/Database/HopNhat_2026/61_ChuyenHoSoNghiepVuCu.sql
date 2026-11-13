-- ================================================================
-- 61_ChuyenHoSoNghiepVuCu.sql   (chay bang psql, CSDL HQTCSDL)
--
-- Chuyen thong tin nghiep vu cua CSDL cu hanoi_military_gis sang
-- ql_don_vi.ho_so_nghiep_vu cho 22 don vi da xac dinh tuong ung.
-- 28 Ban CHQS quan/huyen (cap huyen da ket thuc) KHONG chuyen.
--
-- Buoc 1 - xuat tu CSDL cu (chi doc):
--   psql -d hanoi_military_gis -c "\copy (SELECT ma_don_vi, ten_viet_tat,
--     quan_binh_chung, don_vi_cap_tren, chi_huy_truong, chinh_uy,
--     so_dien_thoai, trang_thai_sscd, do_mat, nhiem_vu_chinh,
--     vu_khi_trang_bi FROM public.don_vi_quan_doi) TO 'ho_so_cu.csv'
--     WITH (FORMAT csv, HEADER true, ENCODING 'UTF8')"
--
-- Buoc 2 - nap vao HQTCSDL:
--   psql -d HQTCSDL -f 61_ChuyenHoSoNghiepVuCu.sql < ho_so_cu.csv
-- ================================================================

\set ON_ERROR_STOP on
SET client_encoding = 'UTF8';

BEGIN;

CREATE TEMP TABLE tam_ho_so_cu
(
    ma_don_vi       TEXT,
    ten_viet_tat    TEXT,
    quan_binh_chung TEXT,
    don_vi_cap_tren TEXT,
    chi_huy_truong  TEXT,
    chinh_uy        TEXT,
    so_dien_thoai   TEXT,
    trang_thai_sscd TEXT,
    do_mat          TEXT,
    nhiem_vu_chinh  TEXT,
    vu_khi_trang_bi TEXT
) ON COMMIT DROP;

-- CSV dua vao qua stdin cua psql (xem Buoc 2): \copy khong nhan bien :'...'.
\copy tam_ho_so_cu FROM pstdin WITH (FORMAT csv, HEADER true, ENCODING 'UTF8')

-- Bang doi chieu ma cu -> ma moi (da kiem tra ten, dia ban).
CREATE TEMP TABLE tam_doi_chieu (ma_cu TEXT PRIMARY KEY, ma_moi TEXT NOT NULL)
ON COMMIT DROP;

INSERT INTO tam_doi_chieu VALUES
    ('BC-CB',     'HN-BCCB-001'),   ('BC-DC',     'HN-BCDC-001'),
    ('BC-HH',     'HN-BCHH-001'),   ('BC-PB',     'HN-BCPB-001'),
    ('BC-TTG',    'HN-BCTTG-001'),  ('BC-TTLL',   'HN-BCTTLL-001'),
    ('BTL-LANG',  'HN-BTLLANG-001'),('BTL-86',    'HN-BTL86-001'),
    ('BTL-TDO',   'HN-BTLTD-001'),  ('BQP-HQ',    'HN-BQP-001'),
    ('HV-HC',     'HN-HVHC-001'),   ('HV-KTQS',   'HN-HVKTQS-001'),
    ('HV-QY',     'HN-HVQY-001'),   ('HV-QP',     'HN-HVQP-001'),
    ('TSQ-LQ1',   'HN-SQLQ1-001'),  ('QC-PKKQ',   'HN-BTLPKKQ-001'),
    ('SD-BB-301', 'HN-SD301-001'),  ('SD-PK-361', 'HN-SD361-001'),
    ('BV-108',    'HN-BV108-001'),  ('BV-354',    'HN-BV354-001'),
    ('LU-144',    'HN-LD144-001'),  ('SB-HOALAC', 'HN-E916-001');

-- Ten viet tat: chi dien khi ban ghi moi con trong.
UPDATE ql_don_vi.don_vi_quan_doi d
SET ten_viet_tat = NULLIF(btrim(c.ten_viet_tat), '')
FROM tam_doi_chieu m
JOIN tam_ho_so_cu c ON c.ma_don_vi = m.ma_cu
WHERE d.ma_don_vi = m.ma_moi
  AND d.ten_viet_tat IS NULL
  AND NULLIF(btrim(c.ten_viet_tat), '') IS NOT NULL;

INSERT INTO ql_don_vi.ho_so_nghiep_vu
(
    don_vi_id, quan_binh_chung, don_vi_cap_tren, chi_huy_truong,
    chinh_uy, so_dien_thoai, trang_thai_sscd, do_mat,
    nhiem_vu_chinh, vu_khi_trang_bi
)
SELECT
    d.id,
    NULLIF(btrim(c.quan_binh_chung), ''),
    NULLIF(btrim(c.don_vi_cap_tren), ''),
    NULLIF(btrim(c.chi_huy_truong), ''),
    NULLIF(btrim(c.chinh_uy), ''),
    NULLIF(btrim(c.so_dien_thoai), ''),
    NULLIF(btrim(c.trang_thai_sscd), ''),
    NULLIF(btrim(c.do_mat), ''),
    NULLIF(btrim(c.nhiem_vu_chinh), ''),
    NULLIF(btrim(c.vu_khi_trang_bi), '')
FROM tam_doi_chieu m
JOIN tam_ho_so_cu c ON c.ma_don_vi = m.ma_cu
JOIN ql_don_vi.don_vi_quan_doi d ON d.ma_don_vi = m.ma_moi
ON CONFLICT (don_vi_id) DO NOTHING;

-- Ban CHQS xa: dua "don vi cap tren" tu danh muc vao ho so.
INSERT INTO ql_don_vi.ho_so_nghiep_vu (don_vi_id, don_vi_cap_tren)
SELECT d.id, NULLIF(btrim(b.don_vi_cap_tren), '')
FROM public.danh_muc_bchqs_cap_xa_2026 b
JOIN ql_don_vi.don_vi_quan_doi d
  ON d.ma_don_vi = 'HN-BCHQS-' || btrim(b.ma_dvhc)
ON CONFLICT (don_vi_id) DO NOTHING;

DO $kt$
DECLARE
    v_csv  INTEGER;
    v_khop INTEGER;
BEGIN
    SELECT count(*) INTO v_csv FROM tam_ho_so_cu;
    SELECT count(*) INTO v_khop
      FROM tam_doi_chieu m
      JOIN tam_ho_so_cu c ON c.ma_don_vi = m.ma_cu
      JOIN ql_don_vi.don_vi_quan_doi d ON d.ma_don_vi = m.ma_moi;

    IF v_csv <> 51 THEN
        RAISE EXCEPTION 'Tệp CSV có % dòng, mong đợi 51.', v_csv;
    END IF;
    IF v_khop <> 22 THEN
        RAISE EXCEPTION 'Chỉ khớp % / 22 đơn vị đối chiếu.', v_khop;
    END IF;

    RAISE NOTICE 'Đã chuyển hồ sơ nghiệp vụ cho % đơn vị.', v_khop;
END;
$kt$;

COMMIT;
