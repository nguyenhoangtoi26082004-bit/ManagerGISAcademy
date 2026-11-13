-- ================================================================
-- 04_NhatKyVaRangBuoc.sql
-- ManagerGISAcademy / HanoiMilitaryGIS
--
-- Bo sung quy trinh nghiep vu o tang CSDL:
--   1. Nhat ky (audit) moi lan THEM / SUA / XOA ho so don vi.
--      Ung dung chi duoc DOC nhat ky, khong sua/xoa duoc.
--   2. Rang buoc toan ven cay to chuc: khong xoa don vi
--      dang la cap tren cua don vi khac.
--
-- Chay sau 01, 02, 03. Chay lai nhieu lan an toan.
-- Khong DROP bang, khong xoa ban ghi.
-- Can chay bang tai khoan chu so huu bang (vd: postgres).
-- ================================================================

-- File co ky tu tieng Viet (bang bo dau o muc 3).
-- Bat buoc dat de psql tren Windows khong doc sai ma.
SET client_encoding = 'UTF8';

BEGIN;

-- ================================================================
-- 1. BANG NHAT KY
-- Luu ban ghi cu / moi dang JSONB (bo 2 cot geometry vi da co
-- kinh_do, vi_do; trigger 01 tu dung lai hinh hoc khi khoi phuc).
-- ================================================================

CREATE TABLE IF NOT EXISTS public.nhat_ky_don_vi_quan_doi
(
    id BIGSERIAL PRIMARY KEY,

    thao_tac CHAR(1) NOT NULL
        CHECK (thao_tac IN ('I', 'U', 'D')),

    ma_don_vi VARCHAR(50) NOT NULL,

    du_lieu_cu JSONB,
    du_lieu_moi JSONB,

    -- session_user: tai khoan dang nhap that,
    -- khong bi thay doi boi SECURITY DEFINER.
    tai_khoan_db TEXT NOT NULL DEFAULT session_user,
    ung_dung TEXT,
    dia_chi_may INET DEFAULT inet_client_addr(),

    thoi_diem TIMESTAMPTZ NOT NULL DEFAULT clock_timestamp()
);

CREATE INDEX IF NOT EXISTS
    idx_nhat_ky_dv_ma_thoi_diem
ON public.nhat_ky_don_vi_quan_doi
(ma_don_vi, thoi_diem DESC);

-- ================================================================
-- 2. TRIGGER GHI NHAT KY
-- SECURITY DEFINER: tai khoan ung dung khong can quyen INSERT
-- tren bang nhat ky => khong tu ghi/xoa nhat ky gia duoc.
-- ================================================================

CREATE OR REPLACE FUNCTION
    public.fn_trg_ghi_nhat_ky_don_vi()

RETURNS TRIGGER
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog, public

AS $ghi_nhat_ky$
DECLARE
    v_cu  JSONB;
    v_moi JSONB;
    v_ma  VARCHAR(50);
BEGIN

    IF TG_OP = 'INSERT' THEN
        v_moi := to_jsonb(NEW) - 'geom_4326' - 'geom_32648';
        v_ma  := NEW.ma_don_vi;

    ELSIF TG_OP = 'UPDATE' THEN
        v_cu  := to_jsonb(OLD) - 'geom_4326' - 'geom_32648';
        v_moi := to_jsonb(NEW) - 'geom_4326' - 'geom_32648';
        v_ma  := NEW.ma_don_vi;

        -- Bo qua UPDATE khong doi du lieu thuc (chi doi ngay).
        IF (v_cu - 'ngay_cap_nhat') = (v_moi - 'ngay_cap_nhat') THEN
            RETURN NULL;
        END IF;

    ELSE
        v_cu := to_jsonb(OLD) - 'geom_4326' - 'geom_32648';
        v_ma := OLD.ma_don_vi;
    END IF;

    INSERT INTO public.nhat_ky_don_vi_quan_doi
    (
        thao_tac,
        ma_don_vi,
        du_lieu_cu,
        du_lieu_moi,
        tai_khoan_db,
        ung_dung
    )
    VALUES
    (
        left(TG_OP, 1),
        v_ma,
        v_cu,
        v_moi,
        session_user,
        current_setting('application_name', true)
    );

    RETURN NULL;   -- AFTER trigger, gia tri tra ve bi bo qua.

END;
$ghi_nhat_ky$;

DROP TRIGGER IF EXISTS trg_nhat_ky_don_vi
ON public.don_vi_quan_doi;

CREATE TRIGGER trg_nhat_ky_don_vi
AFTER INSERT OR UPDATE OR DELETE
ON public.don_vi_quan_doi
FOR EACH ROW
EXECUTE FUNCTION public.fn_trg_ghi_nhat_ky_don_vi();

-- ================================================================
-- 3. RANG BUOC: KHONG XOA DON VI CON CAP DUOI
-- Cot don_vi_cap_tren hien la chuoi tu do (ten hoac ma), nen so
-- khop voi ma / ten / ten viet tat sau khi chuan hoa: bo dau
-- tieng Viet, khong phan biet hoa thuong, gop khoang trang.
-- Cung quy tac voi NghiepVuDonVi.ChuanHoaTimKiem o BLL.
-- BLL da kiem tra truoc va bao loi than thien; day la lop bao ve
-- cuoi cung khi xoa bang SQL truc tiep.
-- ================================================================

-- Khong dung extension unaccent de khong phu thuoc quyen cai dat.
CREATE OR REPLACE FUNCTION
    public.fn_chuan_hoa_ten_don_vi(p TEXT)

RETURNS TEXT
LANGUAGE sql
IMMUTABLE
PARALLEL SAFE

AS $chuan_hoa$
    SELECT NULLIF(
        lower(
            regexp_replace(
                translate(
                    btrim(normalize(p, NFC)),
                    'àáãảạăằắẵẳặâầấẫẩậèéẽẻẹêềếễểệìíĩỉịòóõỏọôồốỗổộơờớỡởợùúũủụưừứữửựỳýỹỷỵđÀÁÃẢẠĂẰẮẴẲẶÂẦẤẪẨẬÈÉẼẺẸÊỀẾỄỂỆÌÍĨỈỊÒÓÕỎỌÔỒỐỖỔỘƠỜỚỠỞỢÙÚŨỦỤƯỪỨỮỬỰỲÝỸỶỴĐ',
                    'aaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyydaaaaaaaaaaaaaaaaaeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyd'
                ),
                '\s+', ' ', 'g'
            )
        ),
        ''
    );
$chuan_hoa$;

CREATE OR REPLACE FUNCTION
    public.fn_trg_chan_xoa_don_vi_co_cap_duoi()

RETURNS TRIGGER
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = pg_catalog, public

AS $chan_xoa$
DECLARE
    v_so INTEGER;
    v_ds TEXT;
BEGIN

    SELECT
        COUNT(*),
        string_agg(c.ma_don_vi, ', ' ORDER BY c.ma_don_vi)
    INTO v_so, v_ds
    FROM public.don_vi_quan_doi AS c
    WHERE c.ma_don_vi <> OLD.ma_don_vi
      AND public.fn_chuan_hoa_ten_don_vi(c.don_vi_cap_tren) IN
      (
          public.fn_chuan_hoa_ten_don_vi(OLD.ma_don_vi),
          public.fn_chuan_hoa_ten_don_vi(OLD.ten_don_vi),
          public.fn_chuan_hoa_ten_don_vi(OLD.ten_viet_tat)
      );

    IF v_so > 0 THEN
        RAISE EXCEPTION USING
            ERRCODE = 'foreign_key_violation',
            MESSAGE = format(
                'Khong the xoa don vi %s: con %s don vi truc thuoc (%s).',
                OLD.ma_don_vi, v_so, v_ds),
            HINT = 'Chuyen cac don vi truc thuoc sang cap tren khac truoc khi xoa.';
    END IF;

    RETURN OLD;

END;
$chan_xoa$;

DROP TRIGGER IF EXISTS trg_chan_xoa_co_cap_duoi
ON public.don_vi_quan_doi;

CREATE TRIGGER trg_chan_xoa_co_cap_duoi
BEFORE DELETE
ON public.don_vi_quan_doi
FOR EACH ROW
EXECUTE FUNCTION public.fn_trg_chan_xoa_don_vi_co_cap_duoi();

-- ================================================================
-- 3b. RANG BUOC SO DIEN THOAI
-- Cung luat voi BLL (LaSoDienThoaiHopLe): chi chu so, khoang trang,
-- + - . ( ) va co 6-15 chu so. Chan ca khi ghi bang SQL truc tiep.
-- Neu du lieu cu vi pham, ALTER se bao loi va ROLLBACK ca file.
-- ================================================================

DO $rang_buoc_sdt$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conrelid = 'public.don_vi_quan_doi'::regclass
          AND conname = 'ck_donvi_so_dien_thoai'
    ) THEN
        ALTER TABLE public.don_vi_quan_doi
        ADD CONSTRAINT ck_donvi_so_dien_thoai
        CHECK (
            so_dien_thoai ~ '^[0-9 +().-]+$'
            AND length(regexp_replace(so_dien_thoai, '[^0-9]', '', 'g'))
                BETWEEN 6 AND 15
        );
    END IF;
END;
$rang_buoc_sdt$;

-- ================================================================
-- 4. PHAN QUYEN
-- Ung dung chi DOC nhat ky. Neu role chua ton tai (vd: Docker
-- tao role o buoc 30 sau file nay) thi bo qua, buoc 30 se cap.
-- ================================================================

REVOKE ALL ON TABLE public.nhat_ky_don_vi_quan_doi FROM PUBLIC;

DO $cap_quyen$
BEGIN
    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'managergis_app') THEN
        GRANT SELECT ON TABLE public.nhat_ky_don_vi_quan_doi
            TO managergis_app;
    END IF;
END;
$cap_quyen$;

COMMIT;

-- ================================================================
-- 5. KIEM TRA / SU DUNG - CHAY RIENG KHI CAN
-- ================================================================

-- Lich su thay doi cua mot don vi:
-- SELECT id, thao_tac, tai_khoan_db, thoi_diem,
--        du_lieu_cu ->> 'ten_don_vi' AS ten_cu,
--        du_lieu_moi ->> 'ten_don_vi' AS ten_moi
-- FROM public.nhat_ky_don_vi_quan_doi
-- WHERE ma_don_vi = 'BTL-TDO'
-- ORDER BY thoi_diem DESC;

-- Khoi phuc mot don vi da bi xoa (tai khoan quan tri):
-- INSERT INTO public.don_vi_quan_doi
-- SELECT (jsonb_populate_record(
--            NULL::public.don_vi_quan_doi, n.du_lieu_cu)).*
-- FROM public.nhat_ky_don_vi_quan_doi AS n
-- WHERE n.id = <id_dong_nhat_ky_D>;
