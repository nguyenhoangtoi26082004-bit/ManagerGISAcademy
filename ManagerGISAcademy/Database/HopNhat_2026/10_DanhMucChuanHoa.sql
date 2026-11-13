-- ================================================================
-- 10_DanhMucChuanHoa.sql
-- CSDL: HQTCSDL  |  Schema: ql_don_vi  |  PostgreSQL 15 + PostGIS 3.4
--
-- Muc tieu:
--   1. Thay cac cot VARCHAR "tu do" bang danh muc co khoa ngoai.
--   2. Danh muc phuong phap dinh vi kem do chinh xac danh nghia.
--   3. Danh muc bieu tuong (logo) theo loai don vi.
--
-- Chay bang tai khoan chu so huu schema (postgres) trong pgAdmin.
-- Script chi THEM (additive), khong DROP bang, khong xoa ban ghi.
-- Chay lai nhieu lan van an toan (IF NOT EXISTS / ON CONFLICT).
-- ================================================================

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- ================================================================
-- 1. MUC DO XAC MINH HO SO
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.dm_muc_do_xac_minh
(
    ma            VARCHAR(50)  PRIMARY KEY,
    ten           VARCHAR(200) NOT NULL UNIQUE,
    thu_tu        INTEGER      NOT NULL DEFAULT 999,
    da_kiem_chung BOOLEAN      NOT NULL DEFAULT FALSE,

    CONSTRAINT ck_mdxm_ma CHECK (ma ~ '^[a-z0-9_]+$')
);

COMMENT ON TABLE ql_don_vi.dm_muc_do_xac_minh IS
    'Muc do xac minh ho so. da_kiem_chung = TRUE chi danh cho ho so '
    'da doi soat thuc dia hoac da duoc can bo co tham quyen phe duyet.';

INSERT INTO ql_don_vi.dm_muc_do_xac_minh (ma, ten, thu_tu, da_kiem_chung)
VALUES
    ('chua_xac_minh',            'Chưa xác minh',                          10, FALSE),
    ('tham_chieu_hanh_chinh',    'Tham chiếu đơn vị hành chính',           20, FALSE),
    ('tham_chieu_theo_khu_vuc',  'Tham chiếu theo khu vực',                30, FALSE),
    ('doi_chieu_nghi_quyet',     'Đối chiếu nghị quyết sắp xếp ĐVHC',      40, FALSE),
    ('doi_chieu_nguon_chinh_thuc','Đối chiếu nguồn công bố chính thức',    50, FALSE),
    ('da_kiem_chung_thuc_dia',   'Đã kiểm chứng thực địa',                 90, TRUE),
    ('da_phe_duyet',             'Đã phê duyệt',                          100, TRUE)
ON CONFLICT DO NOTHING;

-- ================================================================
-- 2. TRANG THAI QUAN LY
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.dm_trang_thai_quan_ly
(
    ma           VARCHAR(50)  PRIMARY KEY,
    ten          VARCHAR(200) NOT NULL UNIQUE,
    con_hieu_luc BOOLEAN      NOT NULL,

    CONSTRAINT ck_ttql_ma CHECK (ma ~ '^[a-z0-9_]+$')
);

INSERT INTO ql_don_vi.dm_trang_thai_quan_ly (ma, ten, con_hieu_luc)
VALUES
    ('dang_quan_ly', 'Đang quản lý',                   TRUE),
    ('tam_dung',     'Tạm dừng cập nhật',              TRUE),
    ('giai_the',     'Đã giải thể / sáp nhập',         FALSE),
    ('luu_tru',      'Lưu trữ lịch sử',                FALSE)
ON CONFLICT DO NOTHING;

-- ================================================================
-- 3. PHUONG PHAP DINH VI + DO CHINH XAC DANH NGHIA
--
-- do_chinh_xac_danh_nghia_m: sai so vi tri mat bang (m) toi thieu
-- co the khai bao voi phuong phap nay. Khong cho phep khai bao
-- "chinh xac hon" muc danh nghia (vd: GNSS cam tay ma ghi 0,1 m).
--
-- la_vi_tri_thuc = FALSE: diem chi mang tinh dai dien
-- (khong phai vi tri tru so that), ban do phai hien thi khac biet.
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.dm_phuong_phap_dinh_vi
(
    ma                         VARCHAR(50)   PRIMARY KEY,
    ten                        VARCHAR(200)  NOT NULL UNIQUE,
    do_chinh_xac_danh_nghia_m  NUMERIC(10,2) NOT NULL,
    la_vi_tri_thuc             BOOLEAN       NOT NULL,
    mo_ta                      TEXT,

    CONSTRAINT ck_ppdv_ma CHECK (ma ~ '^[A-Z0-9_]+$'),
    CONSTRAINT ck_ppdv_do_chinh_xac CHECK (do_chinh_xac_danh_nghia_m > 0)
);

INSERT INTO ql_don_vi.dm_phuong_phap_dinh_vi
    (ma, ten, do_chinh_xac_danh_nghia_m, la_vi_tri_thuc, mo_ta)
VALUES
    ('GNSS_RTK',
     'Đo GNSS RTK/CORS',                         0.05, TRUE,
     'Đo tại cổng/trụ sở, lưới CORS hoặc trạm base VN-2000.'),
    ('GNSS_CAM_TAY',
     'Máy GNSS cầm tay / điện thoại',            5.00, TRUE,
     'Lấy điểm tại cổng chính, ghi rõ thiết bị.'),
    ('SO_HOA_ANH',
     'Số hóa trên ảnh vệ tinh / ảnh hàng không', 3.00, TRUE,
     'Ảnh đã nắn chỉnh, ghi rõ nguồn ảnh và ngày chụp.'),
    ('SO_HOA_BAN_DO',
     'Số hóa bản đồ địa hình/địa chính',         5.00, TRUE,
     'Ghi rõ tờ bản đồ, tỷ lệ, hệ tọa độ gốc.'),
    ('MA_HOA_DIA_CHI',
     'Mã hóa địa chỉ (geocoding)',              50.00, TRUE,
     'Chỉ dùng cho cơ sở có địa chỉ công bố chính thức.'),
    ('NHAP_TAY_CHUA_KIEM_CHUNG',
     'Nhập tay, chưa kiểm chứng',              500.00, TRUE,
     'Dữ liệu cũ; phải đo hoặc số hóa lại.'),
    ('DAI_DIEN_DIA_BAN',
     'Điểm đại diện trong ranh giới xã/phường', 5000.00, FALSE,
     'ST_PointOnSurface của ranh giới xã/phường; KHÔNG phải vị trí thật.')
ON CONFLICT DO NOTHING;

-- ================================================================
-- 4. BIEU TUONG (LOGO) DON VI
--
-- Anh dat trong thu muc ung dung: Resources/Logos/<tep_anh>.
-- CSDL chi luu ten tep (khong luu BLOB) de anh duoc quan ly
-- cung ma nguon va de MapWinGIS nap nhanh.
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.bieu_tuong
(
    ma            VARCHAR(50)  PRIMARY KEY,
    ten           VARCHAR(200) NOT NULL,
    tep_anh       VARCHAR(120) NOT NULL UNIQUE,
    kich_thuoc_px INTEGER      NOT NULL DEFAULT 28,
    thu_tu_ve     INTEGER      NOT NULL DEFAULT 500,
    mo_ta         TEXT,

    CONSTRAINT ck_bt_ma   CHECK (ma ~ '^[A-Z0-9_]+$'),
    CONSTRAINT ck_bt_tep  CHECK (tep_anh ~ '^[a-z0-9_]+\.png$'),
    CONSTRAINT ck_bt_size CHECK (kich_thuoc_px BETWEEN 12 AND 96)
);

COMMENT ON COLUMN ql_don_vi.bieu_tuong.thu_tu_ve IS
    'So nho ve sau cung (nam tren). Don vi cap cao ve de len cap thap.';

INSERT INTO ql_don_vi.bieu_tuong (ma, ten, tep_anh, kich_thuoc_px, thu_tu_ve)
VALUES
    ('MAC_DINH',   'Ký hiệu đơn vị mặc định',   'mac_dinh.png',   24, 900),
    ('BQP',        'Cơ quan Bộ Quốc phòng',     'bqp.png',        40, 100),
    ('TONG_CUC',   'Tổng cục',                  'tong_cuc.png',   36, 150),
    ('BO_TU_LENH', 'Bộ Tư lệnh / Quân chủng',   'bo_tu_lenh.png', 36, 200),
    ('BINH_CHUNG', 'Binh chủng',                'binh_chung.png', 34, 250),
    ('HOC_VIEN',   'Học viện',                  'hoc_vien.png',   32, 300),
    ('TRUONG',     'Trường sĩ quan / đại học',  'truong.png',     30, 350),
    ('BENH_VIEN',  'Bệnh viện / viện quân y',   'benh_vien.png',  30, 380),
    ('SU_DOAN',    'Sư đoàn',                   'su_doan.png',    30, 400),
    ('LU_DOAN',    'Lữ đoàn',                   'lu_doan.png',    28, 450),
    ('TRUNG_DOAN', 'Trung đoàn',                'trung_doan.png', 28, 500),
    ('BCHQS_XA',   'Ban CHQS xã/phường',        'bchqs_xa.png',   22, 800)
ON CONFLICT DO NOTHING;

-- Logo mac dinh theo loai, va logo rieng (ghi de) theo tung don vi.
ALTER TABLE ql_don_vi.loai_don_vi
    ADD COLUMN IF NOT EXISTS bieu_tuong_ma VARCHAR(50)
        REFERENCES ql_don_vi.bieu_tuong (ma);

ALTER TABLE ql_don_vi.don_vi_quan_doi
    ADD COLUMN IF NOT EXISTS bieu_tuong_ma VARCHAR(50)
        REFERENCES ql_don_vi.bieu_tuong (ma);

-- ================================================================
-- 5. CAP / LOAI DON VI BO SUNG
-- ================================================================

INSERT INTO ql_don_vi.cap_don_vi (ma_cap, ten_cap, thu_tu_hien_thi, mo_ta)
VALUES
    ('CAP_BENH_VIEN', 'Cấp bệnh viện/viện',  65,
     'Bệnh viện, viện nghiên cứu y học quân sự.'),
    ('CAP_XA',        'Cấp xã/phường',       90,
     'Ban Chỉ huy quân sự xã/phường sau sắp xếp ĐVHC 2025.')
ON CONFLICT DO NOTHING;

INSERT INTO ql_don_vi.loai_don_vi (ma_loai, ten_loai, mo_ta)
VALUES
    ('BENH_VIEN_QY', 'Bệnh viện quân y',            'Bệnh viện, viện quân y.'),
    ('BCHQS_XA',     'Ban Chỉ huy quân sự cấp xã',  'Ban CHQS xã/phường.')
ON CONFLICT DO NOTHING;

-- Gan logo mac dinh cho tung loai (theo ten loai dang co trong CSDL).
UPDATE ql_don_vi.loai_don_vi AS l
SET bieu_tuong_ma = m.bieu_tuong_ma
FROM (VALUES
    ('Cơ quan chiến lược/cấp Bộ Quốc phòng', 'BQP'),
    ('Tổng cục',                             'TONG_CUC'),
    ('Bộ tư lệnh/Bộ chỉ huy',                'BO_TU_LENH'),
    ('Binh chủng',                           'BINH_CHUNG'),
    ('Học viện quân đội',                    'HOC_VIEN'),
    ('Trường sĩ quan/đại học quân đội',      'TRUONG'),
    ('Bệnh viện quân y',                     'BENH_VIEN'),
    ('Sư đoàn',                              'SU_DOAN'),
    ('Lữ đoàn',                              'LU_DOAN'),
    ('Trung đoàn',                           'TRUNG_DOAN'),
    ('Ban Chỉ huy quân sự cấp xã',           'BCHQS_XA')
) AS m (ten_loai, bieu_tuong_ma)
WHERE l.ten_loai = m.ten_loai
  AND l.bieu_tuong_ma IS NULL;

-- ================================================================
-- 6. RANG BUOC CHAT CHE TREN BANG DON VI
--
-- NOT VALID + VALIDATE: them rang buoc khong khoa bang lau,
-- va bao loi ro rang neu du lieu cu vi pham.
-- ================================================================

DO $rb$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'fk_dv_muc_do_xac_minh') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT fk_dv_muc_do_xac_minh
            FOREIGN KEY (muc_do_xac_minh)
            REFERENCES ql_don_vi.dm_muc_do_xac_minh (ma) NOT VALID;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'fk_dv_trang_thai_quan_ly') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT fk_dv_trang_thai_quan_ly
            FOREIGN KEY (trang_thai_quan_ly)
            REFERENCES ql_don_vi.dm_trang_thai_quan_ly (ma) NOT VALID;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'ck_dv_ma_don_vi') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT ck_dv_ma_don_vi
            CHECK (ma_don_vi ~ '^[A-Z0-9]+(-[A-Z0-9]+)*$') NOT VALID;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'ck_dv_ten_khong_rong') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT ck_dv_ten_khong_rong
            CHECK (btrim(ten_don_vi) <> ''
                   AND ten_don_vi = btrim(ten_don_vi)) NOT VALID;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'ck_dv_khong_tu_lam_cap_tren') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT ck_dv_khong_tu_lam_cap_tren
            CHECK (don_vi_cap_tren_id IS NULL
                   OR don_vi_cap_tren_id <> id) NOT VALID;
    END IF;

    IF NOT EXISTS (SELECT 1 FROM pg_constraint
                   WHERE conname = 'ck_dv_ngay_xac_minh') THEN
        ALTER TABLE ql_don_vi.don_vi_quan_doi
            ADD CONSTRAINT ck_dv_ngay_xac_minh
            CHECK (ngay_xac_minh <= CURRENT_DATE) NOT VALID;
    END IF;
END;
$rb$;

ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT fk_dv_muc_do_xac_minh;
ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT fk_dv_trang_thai_quan_ly;
ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT ck_dv_ma_don_vi;
ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT ck_dv_ten_khong_rong;
ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT ck_dv_khong_tu_lam_cap_tren;
ALTER TABLE ql_don_vi.don_vi_quan_doi VALIDATE CONSTRAINT ck_dv_ngay_xac_minh;

COMMIT;
