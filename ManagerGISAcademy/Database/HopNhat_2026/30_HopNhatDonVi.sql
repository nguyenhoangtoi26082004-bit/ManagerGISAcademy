-- ================================================================
-- 30_HopNhatDonVi.sql
-- CSDL: HQTCSDL  |  Schema: ql_don_vi
--
-- Dua TAT CA don vi ve MOT bang ql_don_vi.don_vi_quan_doi:
--   A. 126 Ban CHQS xa/phuong (tu public.danh_muc_bchqs_cap_xa_2026)
--   B. 4 don vi chi co trong CSDL cu hanoi_military_gis
--      (BV 108, BV 354, Lu doan 144, Trung doan KQ 916).
--
-- KHONG dua vao:
--   - 28 Ban CHQS quan/huyen/thi xa: cap huyen da ket thuc
--     tu 01/7/2025, giu nguyen trong CSDL cu lam luu tru.
--   - Cac ban ghi cu trung voi 43 don vi da co
--     (vd: BC-CB = HN-BCCB-001, LQDTU = HN-HVKTQS-001,
--      BQP-HQ = HN-BQP-001, QC-PKKQ = HN-BTLPKKQ-001...).
--
-- Chay sau 20_ViTriDonVi.sql, bang tai khoan postgres.
-- ================================================================

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- ================================================================
-- A. 126 BAN CHQS XA/PHUONG
-- ================================================================

INSERT INTO ql_don_vi.don_vi_quan_doi
(
    ma_don_vi, ten_don_vi,
    cap_don_vi_id, loai_don_vi_id, xa_phuong_id,
    dia_chi_hien_hanh, nguon_tham_khao,
    muc_do_xac_minh, trang_thai_quan_ly, ghi_chu
)
SELECT
    'HN-BCHQS-' || btrim(b.ma_dvhc),
    btrim(b.ten_don_vi),
    (SELECT id FROM ql_don_vi.cap_don_vi  WHERE ma_cap  = 'CAP_XA'),
    (SELECT id FROM ql_don_vi.loai_don_vi WHERE ma_loai = 'BCHQS_XA'),
    a.xa_phuong_id,
    'CHƯA XÁC MINH',
    'Danh mục BCHQS cấp xã 2026 (public.danh_muc_bchqs_cap_xa_2026), '
        || 'mã ĐVHC ' || btrim(b.ma_dvhc),
    'chua_xac_minh',
    'dang_quan_ly',
    'Đơn vị cấp trên theo danh mục: ' || btrim(b.don_vi_cap_tren)
FROM public.danh_muc_bchqs_cap_xa_2026 AS b
JOIN ql_don_vi.v_anh_xa_phuong_2025 AS a
  ON btrim(a.ma_dvhc::text) = btrim(b.ma_dvhc::text)
ON CONFLICT (ma_don_vi) DO NOTHING;

-- Vi tri: diem dai dien trong ranh gioi xa/phuong (ghi ro ban chat).
INSERT INTO ql_don_vi.vi_tri_don_vi
    (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, nguon, ghi_chu)
SELECT
    dv.id,
    32648,
    ST_X(ST_PointOnSurface(ST_Transform(x.geom, 32648))),
    ST_Y(ST_PointOnSurface(ST_Transform(x.geom, 32648))),
    'DAI_DIEN_DIA_BAN',
    'ST_PointOnSurface ranh giới ' || x.ten_xa_phuong
        || ' (ql_don_vi.dm_xa_phuong).',
    'Cần số hóa vị trí trụ sở UBND/Ban CHQS xã/phường.'
FROM ql_don_vi.don_vi_quan_doi dv
JOIN ql_don_vi.loai_don_vi l  ON l.id = dv.loai_don_vi_id
JOIN ql_don_vi.dm_xa_phuong x ON x.id = dv.xa_phuong_id
WHERE l.ma_loai = 'BCHQS_XA'
  AND x.geom IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM ql_don_vi.vi_tri_don_vi v
                   WHERE v.don_vi_id = dv.id AND v.la_hien_hanh);

-- ================================================================
-- B. 4 DON VI CHI CO TRONG CSDL CU
--
-- xa_phuong_id KHONG nhap tay: suy ra bang phep giao khong gian
-- (ST_Covers) tu toa do, nen luon nhat quan voi vi tri.
-- Toa do cu chua kiem chung -> NHAP_TAY_CHUA_KIEM_CHUNG.
-- ================================================================

WITH nguon (ma_moi, ten, ten_cap, ten_loai, dia_chi,
            kinh_do, vi_do, ma_cu) AS
(
    VALUES
    ('HN-BV108-001', 'Bệnh viện Trung ương Quân đội 108',
     'Cấp bệnh viện/viện', 'Bệnh viện quân y',
     'Số 1 Trần Hưng Đạo, Thành phố Hà Nội',
     105.8612::float8, 21.0185::float8, 'BV-108'),

    ('HN-BV354-001', 'Bệnh viện Quân y 354',
     'Cấp bệnh viện/viện', 'Bệnh viện quân y',
     'Số 120 Đốc Ngữ, Thành phố Hà Nội',
     105.8110, 21.0420, 'BV-354'),

    ('HN-LD144-001', 'Lữ đoàn 144',
     'Cấp lữ đoàn', 'Lữ đoàn',
     'CHƯA XÁC MINH',
     105.8295, 21.0392, 'LU-144'),

    ('HN-E916-001', 'Trung đoàn Không quân 916',
     'Cấp trung đoàn', 'Trung đoàn',
     'CHƯA XÁC MINH',
     105.5340, 21.0385, 'SB-HOALAC')
),
co_xa AS
(
    SELECT n.*, x.id AS xa_phuong_id
    FROM nguon n
    JOIN ql_don_vi.dm_xa_phuong x
      ON ST_Covers(
            ST_Transform(x.geom, 32648),
            ST_Transform(ST_SetSRID(
                ST_MakePoint(n.kinh_do, n.vi_do), 4326), 32648))
)
INSERT INTO ql_don_vi.don_vi_quan_doi
(
    ma_don_vi, ten_don_vi,
    cap_don_vi_id, loai_don_vi_id, xa_phuong_id,
    dia_chi_hien_hanh, nguon_tham_khao,
    muc_do_xac_minh, trang_thai_quan_ly, ghi_chu
)
SELECT
    c.ma_moi, c.ten,
    cdv.id, ldv.id, c.xa_phuong_id,
    c.dia_chi,
    'Hồ sơ cũ hanoi_military_gis.public.don_vi_quan_doi, mã ' || c.ma_cu,
    'chua_xac_minh', 'dang_quan_ly',
    'Hợp nhất 2026 từ CSDL cũ.'
FROM co_xa c
JOIN ql_don_vi.cap_don_vi  cdv ON cdv.ten_cap  = c.ten_cap
JOIN ql_don_vi.loai_don_vi ldv ON ldv.ten_loai = c.ten_loai
ON CONFLICT (ma_don_vi) DO NOTHING;

INSERT INTO ql_don_vi.vi_tri_don_vi
    (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, nguon, ghi_chu)
SELECT dv.id, 4326, n.kinh_do, n.vi_do,
       'NHAP_TAY_CHUA_KIEM_CHUNG',
       'Tọa độ nhập tay trong CSDL cũ (mã ' || n.ma_cu || ').',
       'Cần số hóa lại tại cổng chính trên ảnh vệ tinh hoặc đo GNSS.'
FROM (VALUES
        ('HN-BV108-001', 105.8612::float8, 21.0185::float8, 'BV-108'),
        ('HN-BV354-001', 105.8110, 21.0420, 'BV-354'),
        ('HN-LD144-001', 105.8295, 21.0392, 'LU-144'),
        ('HN-E916-001',  105.5340, 21.0385, 'SB-HOALAC')
     ) AS n (ma, kinh_do, vi_do, ma_cu)
JOIN ql_don_vi.don_vi_quan_doi dv ON dv.ma_don_vi = n.ma
WHERE NOT EXISTS (SELECT 1 FROM ql_don_vi.vi_tri_don_vi v
                   WHERE v.don_vi_id = dv.id AND v.la_hien_hanh);

-- ================================================================
-- C. KIEM TRA KET QUA - SAI THI HUY TOAN BO GIAO DICH
-- ================================================================

DO $kt$
DECLARE
    v_bchqs   INTEGER;
    v_cu      INTEGER;
    v_thieu   INTEGER;
BEGIN
    SELECT count(*) INTO v_bchqs
      FROM ql_don_vi.don_vi_quan_doi dv
      JOIN ql_don_vi.loai_don_vi l ON l.id = dv.loai_don_vi_id
     WHERE l.ma_loai = 'BCHQS_XA';

    SELECT count(*) INTO v_cu
      FROM ql_don_vi.don_vi_quan_doi
     WHERE ma_don_vi IN ('HN-BV108-001', 'HN-BV354-001',
                         'HN-LD144-001', 'HN-E916-001');

    SELECT count(*) INTO v_thieu
      FROM ql_don_vi.don_vi_quan_doi dv
     WHERE NOT EXISTS (SELECT 1 FROM ql_don_vi.vi_tri_don_vi v
                        WHERE v.don_vi_id = dv.id AND v.la_hien_hanh);

    IF v_bchqs <> 126 THEN
        RAISE EXCEPTION 'Mong đợi 126 Ban CHQS xã/phường, thực tế %.', v_bchqs;
    END IF;

    IF v_cu <> 4 THEN
        RAISE EXCEPTION
            'Mong đợi 4 đơn vị từ CSDL cũ, thực tế % '
            '(kiểm tra tên cấp/loại hoặc tọa độ ngoài xã/phường).', v_cu;
    END IF;

    IF v_thieu > 0 THEN
        RAISE EXCEPTION '% đơn vị chưa có vị trí hiện hành.', v_thieu;
    END IF;

    RAISE NOTICE 'Hợp nhất xong: % BCHQS + % đơn vị cũ.', v_bchqs, v_cu;
END;
$kt$;

COMMIT;
