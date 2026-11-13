-- ================================================================
-- 40_ViewBanDoVaKiemTra.sql
-- CSDL: HQTCSDL  |  Schema: ql_don_vi
--
--   1. v_don_vi_ban_do      : view CU, giu nguyen cot de ung dung
--                             hien tai khong bi dem trung BCHQS.
--   2. v_ban_do_don_vi      : view MOI, mot nguon duy nhat cho ban do
--                             (vi tri, do chinh xac, logo, toa do
--                             WGS84 / UTM 48N / VN-2000).
--   3. v_kiem_tra_vi_tri    : danh sach loi/canh bao vi tri.
--   4. v_thong_ke_vi_tri    : tong hop chat luong vi tri theo loai.
--
-- Chay sau 30_HopNhatDonVi.sql, bang tai khoan postgres.
-- ================================================================

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- ================================================================
-- 1. VIEW CU - GIU NGUYEN DANH SACH COT, LOAI BCHQS XA
--    (ung dung dang doc BCHQS tu v_bchqs_cap_xa_2026 rieng).
-- ================================================================

CREATE OR REPLACE VIEW ql_don_vi.v_don_vi_ban_do AS
SELECT dv.id,
       dv.ma_don_vi,
       dv.ten_don_vi,
       dv.ky_hieu_bien_so,
       cdv.ten_cap,
       ldv.ten_loai,
       xp.ma_xa_phuong,
       xp.ten_xa_phuong,
       xp.loai AS loai_xa_phuong,
       dv.tinh_thanh_pho,
       dv.dia_chi_hien_hanh,
       dv.nguon_tham_khao,
       dv.muc_do_xac_minh,
       dv.trang_thai_quan_ly,
       dv.geom
FROM ql_don_vi.don_vi_quan_doi dv
JOIN ql_don_vi.cap_don_vi   cdv ON cdv.id = dv.cap_don_vi_id
JOIN ql_don_vi.loai_don_vi  ldv ON ldv.id = dv.loai_don_vi_id
JOIN ql_don_vi.dm_xa_phuong xp  ON xp.id  = dv.xa_phuong_id
WHERE dv.tinh_thanh_pho::text = 'Thành phố Hà Nội'
  AND dv.geom IS NOT NULL
  AND ldv.ma_loai IS DISTINCT FROM 'BCHQS_XA';

-- ================================================================
-- 2. VIEW BAN DO THONG NHAT
--
-- geom          : vi tri luu tru (dung cho do dac, phan tich).
-- geom_hien_thi : CHI de ve. Neu nhieu don vi trung 1 diem
--                 (vd: cung diem dai dien xa/phuong), xep tren vong
--                 tron ban kinh r quanh diem goc de logo khong
--                 che nhau. Khong dung geom_hien_thi de do dac.
-- ================================================================

CREATE OR REPLACE VIEW ql_don_vi.v_ban_do_don_vi AS
WITH co_so AS
(
    SELECT
        dv.id,
        dv.ma_don_vi,
        dv.ten_don_vi,
        dv.ten_viet_tat,
        dv.ky_hieu_bien_so,
        dv.don_vi_cap_tren_id,
        cdv.ma_cap,
        cdv.ten_cap,
        cdv.thu_tu_hien_thi,
        ldv.ma_loai,
        ldv.ten_loai,
        xp.ma_xa_phuong,
        xp.ten_xa_phuong,
        xp.loai               AS loai_xa_phuong,
        dv.dia_chi_hien_hanh,
        dv.trang_thai_quan_ly,
        dv.muc_do_xac_minh,
        m.ten                 AS ten_muc_do_xac_minh,
        m.da_kiem_chung,
        vt.phuong_phap,
        pp.ten                AS ten_phuong_phap,
        pp.la_vi_tri_thuc,
        vt.do_chinh_xac_m,
        vt.nguon              AS nguon_vi_tri,
        vt.ngay_xac_dinh,
        bt.ma                 AS bieu_tuong_ma,
        bt.tep_anh,
        bt.kich_thuoc_px,
        bt.thu_tu_ve,
        vt.geom,
        count(*) OVER w_cum   AS so_cung_vi_tri,
        row_number() OVER (w_cum ORDER BY bt.thu_tu_ve, dv.ma_don_vi)
                              AS thu_tu_trong_cum
    FROM ql_don_vi.don_vi_quan_doi dv
    JOIN ql_don_vi.cap_don_vi            cdv ON cdv.id = dv.cap_don_vi_id
    JOIN ql_don_vi.loai_don_vi           ldv ON ldv.id = dv.loai_don_vi_id
    JOIN ql_don_vi.dm_xa_phuong          xp  ON xp.id  = dv.xa_phuong_id
    JOIN ql_don_vi.dm_trang_thai_quan_ly tt  ON tt.ma  = dv.trang_thai_quan_ly
    JOIN ql_don_vi.dm_muc_do_xac_minh    m   ON m.ma   = dv.muc_do_xac_minh
    JOIN ql_don_vi.vi_tri_don_vi         vt  ON vt.don_vi_id = dv.id
                                            AND vt.la_hien_hanh
    JOIN ql_don_vi.dm_phuong_phap_dinh_vi pp ON pp.ma  = vt.phuong_phap
    JOIN ql_don_vi.bieu_tuong            bt
      ON bt.ma = COALESCE(dv.bieu_tuong_ma, ldv.bieu_tuong_ma, 'MAC_DINH')
    WHERE tt.con_hieu_luc
    WINDOW w_cum AS (PARTITION BY ST_AsBinary(ST_SnapToGrid(vt.geom, 1.0)))
)
SELECT
    c.id,
    c.ma_don_vi,
    c.ten_don_vi,
    c.ten_viet_tat,
    c.ky_hieu_bien_so,
    c.don_vi_cap_tren_id,
    c.ma_cap,
    c.ten_cap,
    c.thu_tu_hien_thi,
    c.ma_loai,
    c.ten_loai,
    c.ma_xa_phuong,
    c.ten_xa_phuong,
    c.loai_xa_phuong,
    c.dia_chi_hien_hanh,
    c.trang_thai_quan_ly,
    c.muc_do_xac_minh,
    c.ten_muc_do_xac_minh,
    c.da_kiem_chung,
    c.phuong_phap,
    c.ten_phuong_phap,
    c.la_vi_tri_thuc,
    c.do_chinh_xac_m,
    c.nguon_vi_tri,
    c.ngay_xac_dinh,
    c.bieu_tuong_ma,
    c.tep_anh,
    c.kich_thuoc_px,
    c.thu_tu_ve,
    c.so_cung_vi_tri,

    c.geom,

    CASE
        WHEN c.so_cung_vi_tri > 1 THEN
            ST_Translate(
                c.geom,
                (60 + 25 * c.so_cung_vi_tri)
                    * cos(2 * pi() * (c.thu_tu_trong_cum - 1)
                          / c.so_cung_vi_tri),
                (60 + 25 * c.so_cung_vi_tri)
                    * sin(2 * pi() * (c.thu_tu_trong_cum - 1)
                          / c.so_cung_vi_tri))
        ELSE c.geom
    END::geometry(Point, 32648)                    AS geom_hien_thi,

    -- WGS84 / UTM 48N (met) - CRS ban do ung dung.
    ST_X(c.geom)                                   AS x_32648,
    ST_Y(c.geom)                                   AS y_32648,

    -- WGS84 dia ly (do thap phan).
    ST_X(ST_Transform(c.geom, 4326))               AS kinh_do,
    ST_Y(ST_Transform(c.geom, 4326))               AS vi_do,

    -- VN-2000 / TM-3 KTT 105°00' (EPSG:5897), theo quy uoc Viet Nam:
    -- X = Bac (N), Y = Dong (E).
    ST_Y(ST_Transform(c.geom, 5897))               AS x_vn2000,
    ST_X(ST_Transform(c.geom, 5897))               AS y_vn2000
FROM co_so c;

COMMENT ON VIEW ql_don_vi.v_ban_do_don_vi IS
    'Nguon duy nhat cho ban do don vi (hop nhat 2026). '
    'geom_hien_thi chi de ve, khong dung de do dac.';

-- ================================================================
-- 3. VIEW KIEM TRA VI TRI
-- ================================================================

CREATE OR REPLACE VIEW ql_don_vi.v_kiem_tra_vi_tri AS

-- 3.1. Don vi chua co vi tri hien hanh.
SELECT dv.ma_don_vi, dv.ten_don_vi,
       'THIEU_VI_TRI'::text  AS loai_loi,
       'NGHIEM_TRONG'::text  AS muc_do,
       'Chưa có vị trí hiện hành.'::text AS mo_ta
FROM ql_don_vi.don_vi_quan_doi dv
WHERE NOT EXISTS (SELECT 1 FROM ql_don_vi.vi_tri_don_vi v
                   WHERE v.don_vi_id = dv.id AND v.la_hien_hanh)

UNION ALL

-- 3.2. Dang dung diem dai dien, chua phai vi tri tru so.
SELECT b.ma_don_vi, b.ten_don_vi,
       'DIEM_DAI_DIEN', 'CANH_BAO',
       'Đang dùng điểm đại diện của ' || b.ten_xa_phuong
           || ', chưa phải vị trí trụ sở.'
FROM ql_don_vi.v_ban_do_don_vi b
WHERE NOT b.la_vi_tri_thuc

UNION ALL

-- 3.3. Vi tri that nhung do chinh xac kem (> 50 m).
SELECT b.ma_don_vi, b.ten_don_vi,
       'DO_CHINH_XAC_THAP', 'CANH_BAO',
       'Độ chính xác ' || b.do_chinh_xac_m || ' m ('
           || b.ten_phuong_phap || ').'
FROM ql_don_vi.v_ban_do_don_vi b
WHERE b.la_vi_tri_thuc
  AND b.do_chinh_xac_m > 50

UNION ALL

-- 3.4. Trung toa do voi don vi khac (sai so 1 m).
SELECT b.ma_don_vi, b.ten_don_vi,
       'TRUNG_VI_TRI', 'CANH_BAO',
       'Trùng tọa độ với ' || (b.so_cung_vi_tri - 1)
           || ' đơn vị khác.'
FROM ql_don_vi.v_ban_do_don_vi b
WHERE b.so_cung_vi_tri > 1

UNION ALL

-- 3.5. Vi tri khong con nam trong xa/phuong khai bao
--      (xay ra neu sau nay sua xa_phuong_id).
SELECT dv.ma_don_vi, dv.ten_don_vi,
       'NGOAI_XA_KHAI_BAO', 'NGHIEM_TRONG',
       'Vị trí cách ranh giới ' || xp.ten_xa_phuong || ' '
           || round(ST_Distance(vt.geom,
                                ST_Transform(xp.geom, 32648))::numeric)
           || ' m.'
FROM ql_don_vi.don_vi_quan_doi dv
JOIN ql_don_vi.vi_tri_don_vi vt ON vt.don_vi_id = dv.id AND vt.la_hien_hanh
JOIN ql_don_vi.dm_xa_phuong  xp ON xp.id = dv.xa_phuong_id
WHERE xp.geom IS NOT NULL
  AND NOT ST_DWithin(vt.geom, ST_Transform(xp.geom, 32648), 50);

-- ================================================================
-- 4. THONG KE CHAT LUONG VI TRI
-- ================================================================

CREATE OR REPLACE VIEW ql_don_vi.v_thong_ke_vi_tri AS
SELECT
    b.ten_loai,
    count(*)                                         AS tong,
    count(*) FILTER (WHERE b.la_vi_tri_thuc
                       AND b.do_chinh_xac_m <= 10)   AS vi_tri_tot,
    count(*) FILTER (WHERE b.la_vi_tri_thuc
                       AND b.do_chinh_xac_m > 10)    AS vi_tri_can_do_lai,
    count(*) FILTER (WHERE NOT b.la_vi_tri_thuc)     AS diem_dai_dien,
    count(*) FILTER (WHERE b.da_kiem_chung)          AS da_kiem_chung
FROM ql_don_vi.v_ban_do_don_vi b
GROUP BY b.ten_loai;

COMMIT;
