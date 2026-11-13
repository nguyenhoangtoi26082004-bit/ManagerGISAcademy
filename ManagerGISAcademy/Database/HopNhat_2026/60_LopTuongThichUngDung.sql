-- ================================================================
-- 60_LopTuongThichUngDung.sql
-- CSDL: HQTCSDL  |  Giai doan 3: ung dung doc/ghi TRUC TIEP HQTCSDL
--
-- Ky thuat: "VIEW TUONG THICH + INSTEAD OF TRIGGER".
--   Ung dung cu doc/ghi bang public.don_vi_quan_doi (cot phang:
--   loai_don_vi, quan_huyen, kinh_do, vi_do, chi_huy_truong...).
--   Thay vi viet lai toan bo DAL, ta tao VIEW cung ten, cung cot
--   tren mo hinh chuan hoa ql_don_vi; trigger INSTEAD OF chuyen
--   moi INSERT/UPDATE/DELETE thanh thao tac dung tren bang goc:
--     - loai don vi    -> loai_don_vi_id + cap_don_vi_id (danh muc)
--     - kinh do, vi do -> vi_tri_don_vi (co lich su, do chinh xac)
--     - xa/phuong      -> SUY RA tu toa do (ST_Covers), doi chieu
--                         voi lua chon tren bieu mau
--     - thong tin nghiep vu -> ho_so_nghiep_vu (1-1)
--     - XOA            -> xoa mem (trang thai luu_tru) + nhat ky
--
-- Them:
--   public.nhat_ky_don_vi_quan_doi (cung cau truc CSDL cu)
--   public.ranh_gioi_ha_noi        (view tren ranh_gioi_ha_noi_raw)
--
-- Chay sau 10 -> 50, bang tai khoan postgres. Mot giao dich.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- ================================================================
-- 1. CAP / LOAI BO SUNG VA CAP MAC DINH THEO LOAI
-- ================================================================

INSERT INTO ql_don_vi.cap_don_vi (ma_cap, ten_cap, thu_tu_hien_thi, mo_ta)
VALUES
    ('CAP_TIEU_DOAN', 'Cấp tiểu đoàn', 75, 'Tiểu đoàn và tương đương.'),
    ('CAP_KHAC',      'Cấp khác / chưa phân cấp', 95,
     'Cơ quan, đơn vị chưa xác định cấp; cần phân cấp lại.')
ON CONFLICT DO NOTHING;

-- Them cot TRUOC khi them loai moi: lan chay lai, cot da NOT NULL
-- (rang buoc NOT NULL duoc kiem truoc ON CONFLICT).
ALTER TABLE ql_don_vi.loai_don_vi
    ADD COLUMN IF NOT EXISTS cap_mac_dinh_id BIGINT
        REFERENCES ql_don_vi.cap_don_vi (id);

INSERT INTO ql_don_vi.loai_don_vi
    (ma_loai, ten_loai, mo_ta, bieu_tuong_ma, cap_mac_dinh_id)
SELECT m.ma_loai, m.ten_loai, m.mo_ta, 'MAC_DINH', c.id
FROM (VALUES
    ('TIEU_DOAN',   'Tiểu đoàn',   'Tiểu đoàn.',                       'CAP_TIEU_DOAN'),
    ('CO_QUAN',     'Cơ quan',     'Cơ quan chưa phân loại chi tiết.', 'CAP_KHAC'),
    ('DON_VI_KHAC', 'Đơn vị khác', 'Đơn vị chưa phân loại.',           'CAP_KHAC')
) AS m (ma_loai, ten_loai, mo_ta, ma_cap)
JOIN ql_don_vi.cap_don_vi c ON c.ma_cap = m.ma_cap
ON CONFLICT DO NOTHING;

-- Cap mac dinh = cap pho bien nhat cua cac don vi cung loai.
UPDATE ql_don_vi.loai_don_vi AS l
SET cap_mac_dinh_id = t.cap_don_vi_id
FROM (
    SELECT DISTINCT ON (loai_don_vi_id)
           loai_don_vi_id, cap_don_vi_id
    FROM ql_don_vi.don_vi_quan_doi
    GROUP BY loai_don_vi_id, cap_don_vi_id
    ORDER BY loai_don_vi_id, count(*) DESC
) AS t
WHERE l.id = t.loai_don_vi_id
  AND l.cap_mac_dinh_id IS NULL;

UPDATE ql_don_vi.loai_don_vi AS l
SET cap_mac_dinh_id = c.id
FROM ql_don_vi.cap_don_vi AS c
WHERE l.cap_mac_dinh_id IS NULL
  AND c.ma_cap = CASE l.ma_loai
                     WHEN 'TIEU_DOAN' THEN 'CAP_TIEU_DOAN'
                     ELSE 'CAP_KHAC'
                 END;

DO $kt$
BEGIN
    IF EXISTS (SELECT 1 FROM ql_don_vi.loai_don_vi
               WHERE cap_mac_dinh_id IS NULL) THEN
        RAISE EXCEPTION 'Còn loại đơn vị chưa có cấp mặc định.';
    END IF;
END;
$kt$;

ALTER TABLE ql_don_vi.loai_don_vi
    ALTER COLUMN cap_mac_dinh_id SET NOT NULL;

-- ================================================================
-- 2. ANH XA TEN LOAI NHAP TU UNG DUNG -> DANH MUC
--    (bieu mau dung ten ngan: "Học viện", "Nhà trường"...)
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.anh_xa_loai_nhap
(
    ten_nhap VARCHAR(150) PRIMARY KEY,
    ma_loai  VARCHAR(50)  NOT NULL
             REFERENCES ql_don_vi.loai_don_vi (ma_loai),

    CONSTRAINT ck_axln_chuan_hoa
        CHECK (ten_nhap = lower(btrim(ten_nhap)))
);

INSERT INTO ql_don_vi.anh_xa_loai_nhap (ten_nhap, ma_loai)
VALUES
    -- Danh sach tren bieu mau UcNhapDonViNative.
    ('bộ tư lệnh',            'BO_TU_LENH'),
    ('học viện',              'HOC_VIEN'),
    ('nhà trường',            'TRUONG_SI_QUAN'),
    ('binh chủng',            'BINH_CHUNG'),
    ('lữ đoàn',               'LU_DOAN'),
    ('sư đoàn',               'SU_DOAN'),
    ('trung đoàn',            'TRUNG_DOAN'),
    ('tiểu đoàn',             'TIEU_DOAN'),
    ('ban chỉ huy quân sự',   'BCHQS_XA'),
    ('cơ quan',               'CO_QUAN'),
    ('đơn vị khác',           'DON_VI_KHAC'),
    -- Ten loai cua CSDL cu hanoi_military_gis.
    ('học viện - nhà trường', 'HOC_VIEN'),
    ('cơ quan bộ quốc phòng', 'CO_QUAN_CHIEN_LUOC'),
    ('quân chủng',            'BO_TU_LENH')
ON CONFLICT DO NOTHING;

-- Ten day du cua moi loai trong danh muc (khi sua ho so co san).
INSERT INTO ql_don_vi.anh_xa_loai_nhap (ten_nhap, ma_loai)
SELECT lower(btrim(ten_loai)), ma_loai
FROM ql_don_vi.loai_don_vi
ON CONFLICT DO NOTHING;

-- ================================================================
-- 3. HO SO NGHIEP VU (1-1 VOI DON VI)
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.ho_so_nghiep_vu
(
    id              BIGSERIAL PRIMARY KEY,
    don_vi_id       BIGINT NOT NULL UNIQUE
                    REFERENCES ql_don_vi.don_vi_quan_doi (id)
                    ON DELETE CASCADE,

    quan_binh_chung VARCHAR(100),
    don_vi_cap_tren VARCHAR(255),   -- ten/ma cap tren nhap tu do
    chi_huy_truong  VARCHAR(150),
    chinh_uy        VARCHAR(150),
    so_dien_thoai   VARCHAR(50),
    trang_thai_sscd VARCHAR(100),
    do_mat          VARCHAR(50),
    nhiem_vu_chinh  TEXT,
    vu_khi_trang_bi TEXT,
    ngay_cap_nhat   TIMESTAMPTZ NOT NULL DEFAULT now(),

    CONSTRAINT ck_hsnv_dien_thoai
        CHECK (so_dien_thoai IS NULL
               OR so_dien_thoai ~ '^[0-9+() .-]{6,30}$')
);

DROP TRIGGER IF EXISTS trg_nhat_ky_ho_so_nghiep_vu ON ql_don_vi.ho_so_nghiep_vu;
CREATE TRIGGER trg_nhat_ky_ho_so_nghiep_vu
    AFTER INSERT OR UPDATE OR DELETE ON ql_don_vi.ho_so_nghiep_vu
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_ghi_nhat_ky_v2();

-- ================================================================
-- 4. NHAT KY KIEU CU (ung dung doc qua TruyXuatNhatKy)
-- ================================================================

CREATE TABLE IF NOT EXISTS public.nhat_ky_don_vi_quan_doi
(
    id           BIGSERIAL PRIMARY KEY,
    thao_tac     CHAR(1) NOT NULL CHECK (thao_tac IN ('I', 'U', 'D')),
    ma_don_vi    VARCHAR(50) NOT NULL,
    du_lieu_cu   JSONB,
    du_lieu_moi  JSONB,
    tai_khoan_db TEXT NOT NULL DEFAULT session_user,
    ung_dung     TEXT,
    dia_chi_may  INET DEFAULT inet_client_addr(),
    thoi_diem    TIMESTAMPTZ NOT NULL DEFAULT clock_timestamp()
);

CREATE INDEX IF NOT EXISTS idx_nhat_ky_dv_ma_thoi_diem
    ON public.nhat_ky_don_vi_quan_doi (ma_don_vi, thoi_diem DESC);

-- ================================================================
-- 5. VIEW TUONG THICH public.don_vi_quan_doi
--
-- Ten cot giu nhu bang cu. Rieng:
--   quan_huyen  = "Phường X" / "Xã Y" (dia ban 2025, suy tu toa do)
--   x_vn2000,
--   y_vn2000    = GIU NGHIA CU: E, N theo EPSG:32648 (UTM 48N).
--                 VN-2000 that (EPSG:5897) xem v_ban_do_don_vi.
--   phien_ban   = khoa lac quan, doi khi bat ky bang goc nao doi.
-- ================================================================

CREATE OR REPLACE VIEW public.don_vi_quan_doi AS
SELECT
    dv.id,
    dv.ma_don_vi::varchar(50)                         AS ma_don_vi,
    dv.ten_don_vi::varchar(255)                       AS ten_don_vi,
    dv.ten_viet_tat::varchar(100)                     AS ten_viet_tat,
    l.ten_loai::varchar(100)                          AS loai_don_vi,
    hs.quan_binh_chung,
    COALESCE(ct.ten_don_vi, hs.don_vi_cap_tren)::varchar(255)
                                                      AS don_vi_cap_tren,
    (CASE x.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END
        || x.ten_xa_phuong)::varchar(100)             AS quan_huyen,
    dv.dia_chi_hien_hanh::text                        AS dia_chi,
    ST_X(ST_Transform(dv.geom, 4326))                 AS kinh_do,
    ST_Y(ST_Transform(dv.geom, 4326))                 AS vi_do,
    ST_X(dv.geom)                                     AS x_vn2000,
    ST_Y(dv.geom)                                     AS y_vn2000,
    hs.chi_huy_truong,
    hs.chinh_uy,
    hs.so_dien_thoai,
    hs.trang_thai_sscd,
    hs.do_mat,
    hs.nhiem_vu_chinh,
    hs.vu_khi_trang_bi,
    ST_Transform(dv.geom, 4326)::geometry(Point, 4326) AS geom_4326,
    dv.geom::geometry(Point, 32648)                   AS geom_32648,
    dv.ngay_cap_nhat,
    (dv.xmin::text || '.' || COALESCE(hs.xmin::text, '0'))
                                                      AS phien_ban
FROM ql_don_vi.don_vi_quan_doi        dv
JOIN ql_don_vi.loai_don_vi            l  ON l.id  = dv.loai_don_vi_id
JOIN ql_don_vi.dm_xa_phuong           x  ON x.id  = dv.xa_phuong_id
JOIN ql_don_vi.dm_trang_thai_quan_ly  tt ON tt.ma = dv.trang_thai_quan_ly
LEFT JOIN ql_don_vi.ho_so_nghiep_vu   hs ON hs.don_vi_id = dv.id
LEFT JOIN ql_don_vi.don_vi_quan_doi   ct ON ct.id = dv.don_vi_cap_tren_id
WHERE tt.con_hieu_luc
  AND dv.geom IS NOT NULL;

COMMENT ON VIEW public.don_vi_quan_doi IS
    'Lop tuong thich cho ung dung cu (giai doan 3). '
    'Ghi qua trigger INSTEAD OF fn_ho_so_ung_dung_ghi.';

-- ================================================================
-- 6. HAM HO TRO
-- ================================================================

-- Bo tien to "Phường"/"Xã", chu thuong, gop khoang trang.
CREATE OR REPLACE FUNCTION ql_don_vi.fn_chuan_hoa_dia_ban(p TEXT)
RETURNS TEXT
LANGUAGE sql
IMMUTABLE
AS $f$
    SELECT regexp_replace(
               regexp_replace(lower(btrim(coalesce(p, ''))),
                              '^(phường|phuong|xã|xa)\s+', ''),
               '\s+', ' ', 'g');
$f$;

-- Du lieu nhat ky: bo hinh hoc, phien ban va gia tri tinh lai.
CREATE OR REPLACE FUNCTION ql_don_vi.fn_json_nhat_ky(p JSONB)
RETURNS JSONB
LANGUAGE sql
IMMUTABLE
AS $f$
    SELECT CASE WHEN p IS NULL THEN NULL ELSE
        (p - 'geom_4326' - 'geom_32648' - 'phien_ban'
           - 'x_vn2000' - 'y_vn2000' - 'ngay_cap_nhat' - 'id')
        || jsonb_build_object(
               'kinh_do', round((p ->> 'kinh_do')::numeric, 6),
               'vi_do',   round((p ->> 'vi_do')::numeric, 6))
    END;
$f$;

-- ================================================================
-- 7. TRIGGER INSTEAD OF: GHI QUA VIEW
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_ho_so_ung_dung_ghi()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
DECLARE
    v_loai_id   BIGINT;
    v_cap_id    BIGINT;
    v_xa_id     BIGINT;
    v_xa_ten    TEXT;
    v_cap_tren  BIGINT;
    v_id        BIGINT;
    v_diem      geometry;
    v_doi_vt    BOOLEAN := FALSE;
    v_cu        JSONB;
    v_moi       JSONB;
    v_ds_loai   TEXT;
BEGIN
    -- ------------------------------------------------------------
    -- 7.1. XOA = XOA MEM (giu lich su, vi tri, nhat ky)
    -- ------------------------------------------------------------
    IF TG_OP = 'DELETE' THEN
        IF EXISTS (
            SELECT 1
            FROM ql_don_vi.don_vi_quan_doi d
            JOIN ql_don_vi.dm_trang_thai_quan_ly t
              ON t.ma = d.trang_thai_quan_ly AND t.con_hieu_luc
            LEFT JOIN ql_don_vi.ho_so_nghiep_vu h ON h.don_vi_id = d.id
            WHERE d.id <> OLD.id
              AND (d.don_vi_cap_tren_id = OLD.id
                   OR lower(btrim(h.don_vi_cap_tren))
                      IN (lower(OLD.ma_don_vi), lower(OLD.ten_don_vi)))
        ) THEN
            RAISE EXCEPTION
                'Không xóa được "%": vẫn còn đơn vị cấp dưới trực thuộc.',
                OLD.ten_don_vi;
        END IF;

        UPDATE ql_don_vi.don_vi_quan_doi
           SET trang_thai_quan_ly = 'luu_tru',
               ngay_cap_nhat = CURRENT_TIMESTAMP
         WHERE id = OLD.id;

        INSERT INTO public.nhat_ky_don_vi_quan_doi
            (thao_tac, ma_don_vi, du_lieu_cu, du_lieu_moi, ung_dung)
        VALUES ('D', OLD.ma_don_vi,
                ql_don_vi.fn_json_nhat_ky(to_jsonb(OLD)), NULL,
                current_setting('application_name', true));

        RETURN OLD;
    END IF;

    -- ------------------------------------------------------------
    -- 7.2. KIEM TRA CHUNG CHO THEM / SUA
    -- ------------------------------------------------------------
    IF TG_OP = 'UPDATE' AND NEW.ma_don_vi IS DISTINCT FROM OLD.ma_don_vi THEN
        RAISE EXCEPTION 'Không được đổi mã đơn vị (%).', OLD.ma_don_vi;
    END IF;

    SELECT l.id, l.cap_mac_dinh_id
      INTO v_loai_id, v_cap_id
      FROM ql_don_vi.anh_xa_loai_nhap a
      JOIN ql_don_vi.loai_don_vi l ON l.ma_loai = a.ma_loai
     WHERE a.ten_nhap = lower(btrim(coalesce(NEW.loai_don_vi, '')));

    IF v_loai_id IS NULL THEN
        SELECT string_agg(ten_loai, ', ' ORDER BY ten_loai)
          INTO v_ds_loai FROM ql_don_vi.loai_don_vi;
        RAISE EXCEPTION
            'Loại đơn vị "%" không có trong danh mục. Chọn một trong: %.',
            NEW.loai_don_vi, v_ds_loai;
    END IF;

    IF NEW.kinh_do IS NULL OR NEW.vi_do IS NULL THEN
        RAISE EXCEPTION 'Hồ sơ phải có kinh độ, vĩ độ (WGS84).';
    END IF;

    -- Kiem tra mien gia tri TRUOC khi chieu toa do (phat hien dao truc).
    IF NOT (NEW.kinh_do BETWEEN 104.8 AND 106.5
            AND NEW.vi_do BETWEEN 20.2 AND 21.8) THEN
        RAISE EXCEPTION
            'Kinh độ %, vĩ độ % ngoài vùng Hà Nội. '
            'Có thể đã nhập ngược kinh độ và vĩ độ.',
            NEW.kinh_do, NEW.vi_do;
    END IF;

    v_diem := ST_Transform(
                  ST_SetSRID(ST_MakePoint(NEW.kinh_do, NEW.vi_do), 4326),
                  32648);

    IF TG_OP = 'UPDATE' THEN
        -- Bieu mau lam tron toa do (~0,1 m): duoi 0,5 m coi nhu khong doi,
        -- tranh ha cap diem da kiem chung moi lan luu.
        v_doi_vt := NOT ST_DWithin(v_diem, OLD.geom_32648, 0.5);
    END IF;

    -- Xa/phuong SUY RA tu toa do (vi tri hien hanh neu khong doi).
    SELECT x.id,
           CASE x.loai WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END
               || x.ten_xa_phuong
      INTO v_xa_id, v_xa_ten
      FROM ql_don_vi.dm_xa_phuong x
     WHERE ST_Covers(ST_Transform(x.geom, 32648),
                     CASE WHEN TG_OP = 'UPDATE' AND NOT v_doi_vt
                          THEN OLD.geom_32648 ELSE v_diem END)
     ORDER BY x.id
     LIMIT 1;

    IF v_xa_id IS NULL THEN
        RAISE EXCEPTION
            'Tọa độ (%, %) không thuộc phường/xã nào của Hà Nội.',
            NEW.kinh_do, NEW.vi_do;
    END IF;

    IF NULLIF(btrim(NEW.quan_huyen), '') IS NOT NULL
       AND ql_don_vi.fn_chuan_hoa_dia_ban(NEW.quan_huyen)
           <> ql_don_vi.fn_chuan_hoa_dia_ban(v_xa_ten) THEN
        RAISE EXCEPTION
            'Tọa độ nằm trong "%", nhưng hồ sơ chọn "%". '
            'Sửa lại phường/xã hoặc tọa độ.',
            v_xa_ten, NEW.quan_huyen;
    END IF;

    -- Cap tren: khop theo ma hoac ten (khong phan biet hoa thuong).
    SELECT d.id INTO v_cap_tren
      FROM ql_don_vi.don_vi_quan_doi d
     WHERE NULLIF(btrim(NEW.don_vi_cap_tren), '') IS NOT NULL
       AND lower(btrim(NEW.don_vi_cap_tren))
           IN (lower(d.ma_don_vi), lower(d.ten_don_vi))
       AND d.ma_don_vi IS DISTINCT FROM NEW.ma_don_vi
     LIMIT 1;

    -- ------------------------------------------------------------
    -- 7.3. THEM
    -- ------------------------------------------------------------
    IF TG_OP = 'INSERT' THEN
        INSERT INTO ql_don_vi.don_vi_quan_doi
        (
            ma_don_vi, ten_don_vi, ten_viet_tat,
            cap_don_vi_id, loai_don_vi_id, don_vi_cap_tren_id,
            xa_phuong_id, dia_chi_hien_hanh, nguon_tham_khao,
            muc_do_xac_minh, trang_thai_quan_ly, ghi_chu
        )
        VALUES
        (
            btrim(NEW.ma_don_vi), btrim(NEW.ten_don_vi),
            NULLIF(btrim(NEW.ten_viet_tat), ''),
            v_cap_id, v_loai_id, v_cap_tren,
            v_xa_id,
            COALESCE(NULLIF(btrim(NEW.dia_chi), ''), 'CHƯA XÁC MINH'),
            'Nhập từ ứng dụng ManagerGISAcademy',
            'chua_xac_minh', 'dang_quan_ly',
            'Tạo từ ứng dụng; cần xác minh vị trí.'
        )
        RETURNING id INTO v_id;

        INSERT INTO ql_don_vi.vi_tri_don_vi
            (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, nguon)
        VALUES
            (v_id, 4326, NEW.kinh_do, NEW.vi_do,
             'NHAP_TAY_CHUA_KIEM_CHUNG',
             'Nhập trên ứng dụng ManagerGISAcademy bởi ' || session_user);
    ELSE
    -- ------------------------------------------------------------
    -- 7.4. SUA
    -- ------------------------------------------------------------
        v_id := OLD.id;

        UPDATE ql_don_vi.don_vi_quan_doi
           SET ten_don_vi         = btrim(NEW.ten_don_vi),
               ten_viet_tat       = NULLIF(btrim(NEW.ten_viet_tat), ''),
               loai_don_vi_id     = v_loai_id,
               cap_don_vi_id      = CASE WHEN loai_don_vi_id = v_loai_id
                                         THEN cap_don_vi_id ELSE v_cap_id END,
               don_vi_cap_tren_id = v_cap_tren,
               xa_phuong_id       = v_xa_id,
               dia_chi_hien_hanh  = COALESCE(NULLIF(btrim(NEW.dia_chi), ''),
                                             'CHƯA XÁC MINH'),
               ngay_cap_nhat      = CURRENT_TIMESTAMP
         WHERE id = v_id;

        IF v_doi_vt THEN
            INSERT INTO ql_don_vi.vi_tri_don_vi
                (don_vi_id, srid_goc, e_goc, n_goc, phuong_phap, nguon)
            VALUES
                (v_id, 4326, NEW.kinh_do, NEW.vi_do,
                 'NHAP_TAY_CHUA_KIEM_CHUNG',
                 'Sửa trên ứng dụng ManagerGISAcademy bởi ' || session_user);
        END IF;
    END IF;

    -- ------------------------------------------------------------
    -- 7.5. HO SO NGHIEP VU (UPSERT)
    -- ------------------------------------------------------------
    INSERT INTO ql_don_vi.ho_so_nghiep_vu AS h
    (
        don_vi_id, quan_binh_chung, don_vi_cap_tren,
        chi_huy_truong, chinh_uy, so_dien_thoai,
        trang_thai_sscd, do_mat, nhiem_vu_chinh, vu_khi_trang_bi
    )
    VALUES
    (
        v_id,
        NULLIF(btrim(NEW.quan_binh_chung), ''),
        NULLIF(btrim(NEW.don_vi_cap_tren), ''),
        NULLIF(btrim(NEW.chi_huy_truong), ''),
        NULLIF(btrim(NEW.chinh_uy), ''),
        NULLIF(btrim(NEW.so_dien_thoai), ''),
        NULLIF(btrim(NEW.trang_thai_sscd), ''),
        NULLIF(btrim(NEW.do_mat), ''),
        NULLIF(btrim(NEW.nhiem_vu_chinh), ''),
        NULLIF(btrim(NEW.vu_khi_trang_bi), '')
    )
    ON CONFLICT (don_vi_id) DO UPDATE
       SET quan_binh_chung = EXCLUDED.quan_binh_chung,
           don_vi_cap_tren = EXCLUDED.don_vi_cap_tren,
           chi_huy_truong  = EXCLUDED.chi_huy_truong,
           chinh_uy        = EXCLUDED.chinh_uy,
           so_dien_thoai   = EXCLUDED.so_dien_thoai,
           trang_thai_sscd = EXCLUDED.trang_thai_sscd,
           do_mat          = EXCLUDED.do_mat,
           nhiem_vu_chinh  = EXCLUDED.nhiem_vu_chinh,
           vu_khi_trang_bi = EXCLUDED.vu_khi_trang_bi,
           ngay_cap_nhat   = now()
     WHERE (h.quan_binh_chung, h.don_vi_cap_tren, h.chi_huy_truong,
            h.chinh_uy, h.so_dien_thoai, h.trang_thai_sscd, h.do_mat,
            h.nhiem_vu_chinh, h.vu_khi_trang_bi)
           IS DISTINCT FROM
           (EXCLUDED.quan_binh_chung, EXCLUDED.don_vi_cap_tren,
            EXCLUDED.chi_huy_truong, EXCLUDED.chinh_uy,
            EXCLUDED.so_dien_thoai, EXCLUDED.trang_thai_sscd,
            EXCLUDED.do_mat, EXCLUDED.nhiem_vu_chinh,
            EXCLUDED.vu_khi_trang_bi);

    -- ------------------------------------------------------------
    -- 7.6. NHAT KY (bo qua lan luu khong doi du lieu)
    -- ------------------------------------------------------------
    SELECT to_jsonb(v) INTO v_moi
      FROM public.don_vi_quan_doi v WHERE v.id = v_id;

    v_moi := ql_don_vi.fn_json_nhat_ky(v_moi);
    v_cu  := CASE WHEN TG_OP = 'UPDATE'
                  THEN ql_don_vi.fn_json_nhat_ky(to_jsonb(OLD)) END;

    IF v_cu IS DISTINCT FROM v_moi THEN
        INSERT INTO public.nhat_ky_don_vi_quan_doi
            (thao_tac, ma_don_vi, du_lieu_cu, du_lieu_moi, ung_dung)
        VALUES (left(TG_OP, 1), btrim(NEW.ma_don_vi), v_cu, v_moi,
                current_setting('application_name', true));
    END IF;

    NEW.id := v_id;
    RETURN NEW;
END;
$f$;

DROP TRIGGER IF EXISTS trg_ho_so_ung_dung_ghi ON public.don_vi_quan_doi;
CREATE TRIGGER trg_ho_so_ung_dung_ghi
    INSTEAD OF INSERT OR UPDATE OR DELETE ON public.don_vi_quan_doi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_ho_so_ung_dung_ghi();

-- ================================================================
-- 8. RANH GIOI HA NOI CHO UNG DUNG
-- ================================================================

CREATE OR REPLACE VIEW public.ranh_gioi_ha_noi AS
SELECT
    r.id,
    ST_Multi(ST_Transform(r.geom, 32648))::geometry(MultiPolygon, 32648) AS geom,
    NULL::bigint            AS fid,
    r.ma_tinh, r.ten_tinh, r.sap_nhap, r.quy_mo, r.tru_so, r.loai,
    r.cap, r.stt,
    r.dtich_km2::float8     AS dtich_km2,
    r.dan_so,
    r.matdo_km2::float8     AS matdo_km2
FROM ql_don_vi.ranh_gioi_ha_noi_raw r;

-- ================================================================
-- 9. PHAN QUYEN: ung dung ghi QUA VIEW, khong cham bang goc
-- ================================================================

GRANT SELECT, INSERT, UPDATE, DELETE ON public.don_vi_quan_doi TO managergis_app;
GRANT SELECT ON public.nhat_ky_don_vi_quan_doi TO managergis_app;
GRANT SELECT ON public.ranh_gioi_ha_noi        TO managergis_app;
GRANT SELECT ON public.phuong_xa_hanoi_2025    TO managergis_app;

COMMIT;
