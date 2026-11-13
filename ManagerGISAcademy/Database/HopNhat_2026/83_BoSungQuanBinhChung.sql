-- ================================================================
-- 83_BoSungQuanBinhChung.sql
-- Bo sung truong "Quan/Binh chung" con trong cho 25 don vi (khong
-- tinh Ban CHQS xa). Gia tri suy ra tu DON VI CAP TREN da co trong
-- CSDL va dung lai bo tu vung san co (Phong khong - Khong quan,
-- Binh chung ..., Luc quan); don vi truc thuoc truc tiep BQP ghi
-- "Trực thuộc Bộ Quốc phòng".
--
-- KHONG bo sung so dien thoai: so truc ban don vi quan doi khong co
-- nguon cong khai tin cay -> de trong, ung dung bao "Canh bao".
--
-- Chi dien vao o DANG TRONG (khong ghi de du lieu da nhap).
-- Trigger nhat ky cua ho_so_nghiep_vu ghi lai moi thay doi.
-- Chay bang postgres tren HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

CREATE TEMP TABLE tmp_qbc (ma_don_vi TEXT PRIMARY KEY, quan_binh_chung TEXT NOT NULL, can_cu TEXT) ON COMMIT DROP;

INSERT INTO tmp_qbc VALUES
    ('HN-BTLBDBP-001',  'Bộ đội Biên phòng',            'Lực lượng Bộ đội Biên phòng'),
    ('HN-HVBP-001',     'Bộ đội Biên phòng',            'Cấp trên: BTL Bộ đội Biên phòng'),
    ('HN-BTLCSB-001',   'Cảnh sát biển',                'Lực lượng Cảnh sát biển Việt Nam'),
    ('HN-HVPKKQ-001',   'Phòng không - Không quân',     'Cấp trên: BTL Quân chủng PK-KQ'),
    ('HN-E218-001',     'Phòng không - Không quân',     'Cấp trên: Sư đoàn Phòng không 361'),
    ('HN-E236-001',     'Phòng không - Không quân',     'Cấp trên: Sư đoàn Phòng không 361'),
    ('HN-E257-001',     'Phòng không - Không quân',     'Cấp trên: Sư đoàn Phòng không 361'),
    ('HN-E293-001',     'Phòng không - Không quân',     'Cấp trên: Sư đoàn Phòng không 361'),
    ('HN-LD113-001',    'Binh chủng Đặc công',          'Cấp trên: BTL Binh chủng Đặc công'),
    ('HN-SQDC-001',     'Binh chủng Đặc công',          'Cấp trên: BTL Binh chủng Đặc công'),
    ('HN-LD239-001',    'Binh chủng Công binh',         'Cấp trên: BTL Binh chủng Công binh'),
    ('HN-LD45-001',     'Binh chủng Pháo binh',         'Cấp trên: BTL Binh chủng Pháo binh'),
    ('HN-SQPB-001',     'Binh chủng Pháo binh',         'Cấp trên: BTL Binh chủng Pháo binh'),
    ('HN-SQPH-001',     'Binh chủng Hóa học',           'Cấp trên: BTL Binh chủng Hóa học'),
    ('HN-SD308-001',    'Lục quân',                     'Sư đoàn bộ binh'),
    ('HN-E692-001',     'Lục quân',                     'Cấp trên: Sư đoàn Bộ binh 301'),
    ('HN-BTTM-001',     'Trực thuộc Bộ Quốc phòng',     'Cơ quan chiến lược'),
    ('HN-TCCNQP-001',   'Trực thuộc Bộ Quốc phòng',     'Tổng cục'),
    ('HN-TCCT-001',     'Trực thuộc Bộ Quốc phòng',     'Tổng cục'),
    ('HN-TCHCKT-001',   'Trực thuộc Bộ Quốc phòng',     'Tổng cục'),
    ('HN-TCTB-001',     'Trực thuộc Bộ Quốc phòng',     'Tổng cục'),
    ('HN-HVCT-001',     'Trực thuộc Bộ Quốc phòng',     'Cấp trên: Bộ Quốc phòng'),
    ('HN-HVKHQS-001',   'Trực thuộc Bộ Quốc phòng',     'Cấp trên: Bộ Quốc phòng'),
    ('HN-DHVHNTQD-001', 'Trực thuộc Bộ Quốc phòng',     'Cấp trên: Bộ Quốc phòng'),
    ('HN-SQCT-001',     'Trực thuộc Bộ Quốc phòng',     'Cấp trên: Bộ Quốc phòng');

INSERT INTO ql_don_vi.ho_so_nghiep_vu AS h (don_vi_id, quan_binh_chung)
SELECT d.id, t.quan_binh_chung
FROM tmp_qbc t
JOIN ql_don_vi.don_vi_quan_doi d ON d.ma_don_vi = t.ma_don_vi
ON CONFLICT (don_vi_id) DO UPDATE
   SET quan_binh_chung = EXCLUDED.quan_binh_chung,
       ngay_cap_nhat   = now()
 WHERE nullif(btrim(h.quan_binh_chung), '') IS NULL;

-- Bao cao: con bao nhieu don vi (ngoai Ban CHQS xa) thieu quan/binh chung.
DO $$
DECLARE n INT;
BEGIN
    SELECT count(*) INTO n
    FROM public.don_vi_quan_doi
    WHERE ma_don_vi NOT LIKE 'HN-BCHQS-%'
      AND nullif(btrim(quan_binh_chung), '') IS NULL;
    RAISE NOTICE 'Con thieu quan/binh chung: % don vi', n;
END $$;

COMMIT;
