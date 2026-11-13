-- ================================================================
-- 89_TaiKhoanDangNhap.sql
-- TAI KHOAN NGUOI DUNG: DANG KY / DANG NHAP / PHAN QUYEN
--
-- * Mat khau bam bcrypt (pgcrypto: crypt + gen_salt('bf', 10)); CSDL
--   khong bao gio luu mat khau ro. Ung dung khong duoc doc bang truc tiep.
-- * Moi tai khoan gan 1 vai tro script 85: gis_quan_tri | gis_bien_tap | gis_xem.
--   Sau dang nhap, ung dung SET ROLE theo vai tro -> RLS + GRANT ap dung.
-- * Khach (chua dang nhap) = gis_xem.
-- * Khoi tao: KHONG co mat khau mac dinh. Tai khoan DANG KY DAU TIEN khi
--   chua co quan tri vien hoat dong se thanh Quan tri; cac dang ky sau o
--   trang thai CHO_DUYET, quan tri vien duyet va chon vai tro.
-- * Sai mat khau 5 lan -> khoa tam 15 phut. Moi lan dang nhap duoc ghi nhat ky.
-- * Ten tai khoan duoc gan vao phien (app.tai_khoan + application_name) ->
--   nhat ky thay doi va hang doi duyet ghi duoc NGUOI thao tac, khong chi vai tro.
-- Chay lai an toan.
-- ================================================================
BEGIN;

CREATE EXTENSION IF NOT EXISTS pgcrypto WITH SCHEMA public;
CREATE SCHEMA IF NOT EXISTS ql_he_thong;

CREATE TABLE IF NOT EXISTS ql_he_thong.tai_khoan (
    id                 bigserial PRIMARY KEY,
    ten_dang_nhap      varchar(40)  NOT NULL,
    ho_ten             varchar(120) NOT NULL,
    don_vi_cong_tac    varchar(200),
    email              varchar(120),
    mat_khau_bam       text         NOT NULL,
    vai_tro            varchar(30)  NOT NULL DEFAULT 'gis_xem',
    trang_thai         varchar(20)  NOT NULL DEFAULT 'CHO_DUYET',
    ly_do_dang_ky      text,
    so_lan_sai         int          NOT NULL DEFAULT 0,
    khoa_den           timestamptz,
    ngay_tao           timestamptz  NOT NULL DEFAULT now(),
    lan_dang_nhap_cuoi timestamptz,
    nguoi_duyet        varchar(40),
    ngay_duyet         timestamptz,
    CONSTRAINT ck_tk_ten     CHECK (ten_dang_nhap ~ '^[a-z0-9._]{4,40}$'),
    CONSTRAINT ck_tk_vai_tro CHECK (vai_tro IN ('gis_quan_tri', 'gis_bien_tap', 'gis_xem')),
    CONSTRAINT ck_tk_trang   CHECK (trang_thai IN ('CHO_DUYET', 'HOAT_DONG', 'KHOA')),
    CONSTRAINT ck_tk_email   CHECK (email IS NULL OR email ~ '^[^@\s]+@[^@\s]+\.[^@\s]+$')
);
CREATE UNIQUE INDEX IF NOT EXISTS ux_tai_khoan_ten ON ql_he_thong.tai_khoan (lower(ten_dang_nhap));

CREATE TABLE IF NOT EXISTS ql_he_thong.nhat_ky_dang_nhap (
    id            bigserial PRIMARY KEY,
    thoi_diem     timestamptz NOT NULL DEFAULT clock_timestamp(),
    ten_dang_nhap varchar(60),
    ket_qua       varchar(30) NOT NULL,
    ghi_chu       text
);
CREATE INDEX IF NOT EXISTS ix_nk_dang_nhap_tg ON ql_he_thong.nhat_ky_dang_nhap (thoi_diem DESC);

REVOKE ALL ON ql_he_thong.tai_khoan, ql_he_thong.nhat_ky_dang_nhap FROM PUBLIC;

-- Hang doi duyet: ghi TEN TAI KHOAN gui / duyet (ngoai vai tro).
ALTER TABLE ql_he_thong.yeu_cau_thay_doi
    ADD COLUMN IF NOT EXISTS tai_khoan_gui  varchar(40) DEFAULT nullif(current_setting('app.tai_khoan', true), ''),
    ADD COLUMN IF NOT EXISTS tai_khoan_duyet varchar(40);

CREATE OR REPLACE FUNCTION ql_he_thong.fn_ghi_tai_khoan_duyet()
RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
    IF OLD.trang_thai = 'CHO_DUYET' AND NEW.trang_thai <> 'CHO_DUYET' THEN
        NEW.tai_khoan_duyet := nullif(current_setting('app.tai_khoan', true), '');
    END IF;
    RETURN NEW;
END $$;
DROP TRIGGER IF EXISTS trg_ghi_tai_khoan_duyet ON ql_he_thong.yeu_cau_thay_doi;
CREATE TRIGGER trg_ghi_tai_khoan_duyet BEFORE UPDATE ON ql_he_thong.yeu_cau_thay_doi
    FOR EACH ROW EXECUTE FUNCTION ql_he_thong.fn_ghi_tai_khoan_duyet();

-- ---------------------------------------------------------------
-- Tien ich
-- ---------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_kiem_mat_khau(p text)
RETURNS text LANGUAGE sql IMMUTABLE AS $$
    SELECT CASE
        WHEN p IS NULL OR length(p) < 8 THEN 'Mật khẩu phải có ít nhất 8 ký tự.'
        WHEN length(p) > 72 THEN 'Mật khẩu tối đa 72 ký tự.'
        WHEN p !~ '[A-Za-z]' OR p !~ '[0-9]' THEN 'Mật khẩu phải có cả chữ và số.'
        ELSE NULL END;
$$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_la_quan_tri()
RETURNS boolean LANGUAGE sql STABLE AS $$
    -- Superuser chi duoc tinh la quan tri khi KHONG SET ROLE (chay script/bao tri).
    SELECT ql_he_thong.fn_vai_tro_hien_hanh() = 'gis_quan_tri'
        OR (coalesce(current_setting('role', true), 'none') IN ('none', '')
            AND (SELECT rolsuper FROM pg_roles WHERE rolname = session_user));
$$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_co_quan_tri_hoat_dong()
RETURNS boolean LANGUAGE sql STABLE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
    SELECT EXISTS (SELECT 1 FROM ql_he_thong.tai_khoan WHERE vai_tro = 'gis_quan_tri' AND trang_thai = 'HOAT_DONG');
$$;

-- ---------------------------------------------------------------
-- Dang ky
-- ---------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_dang_ky(
    p_ten text, p_ho_ten text, p_mat_khau text,
    p_don_vi text DEFAULT NULL, p_email text DEFAULT NULL, p_ly_do text DEFAULT NULL)
RETURNS TABLE (ok boolean, thong_bao text, vai_tro text, trang_thai text)
LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, public, pg_temp AS $$
DECLARE
    v_ten text := lower(btrim(coalesce(p_ten, '')));
    v_loi text;
    v_dau_tien boolean;
BEGIN
    IF v_ten !~ '^[a-z0-9._]{4,40}$' THEN
        RETURN QUERY SELECT false, 'Tên đăng nhập 4–40 ký tự: chữ thường không dấu, số, dấu chấm hoặc gạch dưới.', NULL::text, NULL::text; RETURN;
    END IF;
    IF length(btrim(coalesce(p_ho_ten, ''))) < 3 THEN
        RETURN QUERY SELECT false, 'Họ tên phải có ít nhất 3 ký tự.', NULL::text, NULL::text; RETURN;
    END IF;
    v_loi := ql_he_thong.fn_kiem_mat_khau(p_mat_khau);
    IF v_loi IS NOT NULL THEN
        RETURN QUERY SELECT false, v_loi, NULL::text, NULL::text; RETURN;
    END IF;
    IF nullif(btrim(p_email), '') IS NOT NULL AND btrim(p_email) !~ '^[^@\s]+@[^@\s]+\.[^@\s]+$' THEN
        RETURN QUERY SELECT false, 'Email không hợp lệ.', NULL::text, NULL::text; RETURN;
    END IF;
    IF EXISTS (SELECT 1 FROM ql_he_thong.tai_khoan t WHERE lower(t.ten_dang_nhap) = v_ten) THEN
        RETURN QUERY SELECT false, 'Tên đăng nhập "' || v_ten || '" đã tồn tại.', NULL::text, NULL::text; RETURN;
    END IF;

    -- Khoa bang de 2 nguoi dang ky dong thoi khong cung thanh "quan tri dau tien".
    LOCK TABLE ql_he_thong.tai_khoan IN SHARE ROW EXCLUSIVE MODE;
    v_dau_tien := NOT EXISTS (SELECT 1 FROM ql_he_thong.tai_khoan t
                              WHERE t.vai_tro = 'gis_quan_tri' AND t.trang_thai = 'HOAT_DONG');

    INSERT INTO ql_he_thong.tai_khoan (ten_dang_nhap, ho_ten, don_vi_cong_tac, email, mat_khau_bam,
                                       vai_tro, trang_thai, ly_do_dang_ky, nguoi_duyet, ngay_duyet)
    VALUES (v_ten, btrim(p_ho_ten), nullif(btrim(p_don_vi), ''), nullif(btrim(p_email), ''),
            public.crypt(p_mat_khau, public.gen_salt('bf', 10)),
            CASE WHEN v_dau_tien THEN 'gis_quan_tri' ELSE 'gis_xem' END,
            CASE WHEN v_dau_tien THEN 'HOAT_DONG' ELSE 'CHO_DUYET' END,
            nullif(btrim(p_ly_do), ''),
            CASE WHEN v_dau_tien THEN 'he_thong' END,
            CASE WHEN v_dau_tien THEN now() END);

    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu)
    VALUES (v_ten, 'DANG_KY', CASE WHEN v_dau_tien THEN 'quản trị đầu tiên' ELSE 'chờ duyệt' END);

    IF v_dau_tien THEN
        RETURN QUERY SELECT true, 'Đã tạo tài khoản QUẢN TRỊ đầu tiên của hệ thống. Bạn có thể đăng nhập ngay.',
                            'gis_quan_tri'::text, 'HOAT_DONG'::text;
    ELSE
        RETURN QUERY SELECT true, 'Đã gửi đăng ký. Quản trị viên sẽ duyệt và cấp vai trò trước khi bạn đăng nhập được.',
                            'gis_xem'::text, 'CHO_DUYET'::text;
    END IF;
END $$;

-- ---------------------------------------------------------------
-- Dang nhap
-- ---------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_dang_nhap(p_ten text, p_mat_khau text)
RETURNS TABLE (ok boolean, thong_bao text, id bigint, ten_dang_nhap text, ho_ten text,
               vai_tro text, trang_thai text, don_vi_cong_tac text, lan_dang_nhap_truoc timestamptz)
LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, public, pg_temp AS $$
DECLARE
    v_ten text := lower(btrim(coalesce(p_ten, '')));
    r ql_he_thong.tai_khoan%ROWTYPE;
    v_sai int;
BEGIN
    SELECT * INTO r FROM ql_he_thong.tai_khoan t WHERE lower(t.ten_dang_nhap) = v_ten FOR UPDATE;
    IF NOT FOUND THEN
        PERFORM public.crypt(coalesce(p_mat_khau, ''), public.gen_salt('bf', 10));   -- can bang thoi gian
        INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (left(v_ten, 60), 'SAI_TEN');
        RETURN QUERY SELECT false, 'Sai tên đăng nhập hoặc mật khẩu.', NULL::bigint, NULL::text, NULL::text,
                            NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        RETURN;
    END IF;

    IF r.khoa_den IS NOT NULL AND r.khoa_den > now() THEN
        INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (v_ten, 'DANG_KHOA_TAM');
        RETURN QUERY SELECT false, 'Tài khoản tạm khóa do nhập sai nhiều lần. Thử lại sau '
                            || to_char(r.khoa_den AT TIME ZONE 'Asia/Ho_Chi_Minh', 'HH24:MI') || '.',
                            NULL::bigint, NULL::text, NULL::text, NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        RETURN;
    END IF;

    IF r.mat_khau_bam <> public.crypt(coalesce(p_mat_khau, ''), r.mat_khau_bam) THEN
        v_sai := r.so_lan_sai + 1;
        IF v_sai >= 5 THEN
            UPDATE ql_he_thong.tai_khoan t SET so_lan_sai = 0, khoa_den = now() + interval '15 minutes' WHERE t.id = r.id;
            INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu) VALUES (v_ten, 'KHOA_TAM', 'sai 5 lần');
            RETURN QUERY SELECT false, 'Sai mật khẩu 5 lần – tài khoản bị khóa tạm 15 phút.',
                                NULL::bigint, NULL::text, NULL::text, NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        ELSE
            UPDATE ql_he_thong.tai_khoan t SET so_lan_sai = v_sai WHERE t.id = r.id;
            INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu) VALUES (v_ten, 'SAI_MAT_KHAU', 'lần ' || v_sai);
            RETURN QUERY SELECT false, 'Sai tên đăng nhập hoặc mật khẩu (còn ' || (5 - v_sai) || ' lần thử).',
                                NULL::bigint, NULL::text, NULL::text, NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        END IF;
        RETURN;
    END IF;

    IF r.trang_thai = 'CHO_DUYET' THEN
        INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (v_ten, 'CHO_DUYET');
        RETURN QUERY SELECT false, 'Tài khoản đang chờ quản trị viên duyệt.',
                            NULL::bigint, NULL::text, NULL::text, NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        RETURN;
    ELSIF r.trang_thai = 'KHOA' THEN
        INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (v_ten, 'BI_KHOA');
        RETURN QUERY SELECT false, 'Tài khoản đã bị khóa. Liên hệ quản trị viên.',
                            NULL::bigint, NULL::text, NULL::text, NULL::text, NULL::text, NULL::text, NULL::timestamptz;
        RETURN;
    END IF;

    UPDATE ql_he_thong.tai_khoan t SET so_lan_sai = 0, khoa_den = NULL, lan_dang_nhap_cuoi = now() WHERE t.id = r.id;
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (v_ten, 'THANH_CONG');
    RETURN QUERY SELECT true, 'Đăng nhập thành công.', r.id, r.ten_dang_nhap::text, r.ho_ten::text,
                        r.vai_tro::text, r.trang_thai::text, r.don_vi_cong_tac::text, r.lan_dang_nhap_cuoi;
END $$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_dang_xuat(p_ten text)
RETURNS void LANGUAGE sql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (left(lower(p_ten), 60), 'DANG_XUAT');
$$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_doi_mat_khau(p_ten text, p_cu text, p_moi text)
RETURNS text LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, public, pg_temp AS $$
DECLARE
    r ql_he_thong.tai_khoan%ROWTYPE;
    v_loi text;
BEGIN
    SELECT * INTO r FROM ql_he_thong.tai_khoan t WHERE lower(t.ten_dang_nhap) = lower(btrim(p_ten)) FOR UPDATE;
    IF NOT FOUND OR r.mat_khau_bam <> public.crypt(coalesce(p_cu, ''), r.mat_khau_bam) THEN
        RETURN 'Mật khẩu hiện tại không đúng.';
    END IF;
    v_loi := ql_he_thong.fn_kiem_mat_khau(p_moi);
    IF v_loi IS NOT NULL THEN RETURN v_loi; END IF;
    IF p_moi = p_cu THEN RETURN 'Mật khẩu mới phải khác mật khẩu cũ.'; END IF;
    UPDATE ql_he_thong.tai_khoan t SET mat_khau_bam = public.crypt(p_moi, public.gen_salt('bf', 10)) WHERE t.id = r.id;
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua) VALUES (r.ten_dang_nhap, 'DOI_MAT_KHAU');
    RETURN 'OK';
END $$;

-- ---------------------------------------------------------------
-- Quan tri tai khoan (chi vai tro gis_quan_tri)
-- ---------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_ds_tai_khoan()
RETURNS TABLE (id bigint, ten_dang_nhap text, ho_ten text, don_vi_cong_tac text, email text,
               vai_tro text, trang_thai text, ly_do_dang_ky text, ngay_tao timestamptz,
               lan_dang_nhap_cuoi timestamptz, khoa_den timestamptz, nguoi_duyet text)
LANGUAGE plpgsql STABLE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
BEGIN
    IF NOT ql_he_thong.fn_la_quan_tri() THEN
        RAISE EXCEPTION 'Chỉ quản trị viên được xem danh sách tài khoản.';
    END IF;
    RETURN QUERY
    SELECT t.id, t.ten_dang_nhap::text, t.ho_ten::text, t.don_vi_cong_tac::text, t.email::text,
           t.vai_tro::text, t.trang_thai::text, t.ly_do_dang_ky, t.ngay_tao, t.lan_dang_nhap_cuoi,
           CASE WHEN t.khoa_den > now() THEN t.khoa_den END, t.nguoi_duyet::text
    FROM ql_he_thong.tai_khoan t
    ORDER BY (t.trang_thai = 'CHO_DUYET') DESC, t.ngay_tao DESC;
END $$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_cap_nhat_tai_khoan(p_id bigint, p_vai_tro text, p_trang_thai text)
RETURNS text LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
DECLARE
    r ql_he_thong.tai_khoan%ROWTYPE;
    v_toi text := nullif(current_setting('app.tai_khoan', true), '');
BEGIN
    IF NOT ql_he_thong.fn_la_quan_tri() THEN RETURN 'Chỉ quản trị viên được phân quyền.'; END IF;
    IF p_vai_tro NOT IN ('gis_quan_tri', 'gis_bien_tap', 'gis_xem') THEN RETURN 'Vai trò không hợp lệ.'; END IF;
    IF p_trang_thai NOT IN ('HOAT_DONG', 'KHOA') THEN RETURN 'Trạng thái không hợp lệ.'; END IF;
    SELECT * INTO r FROM ql_he_thong.tai_khoan t WHERE t.id = p_id FOR UPDATE;
    IF NOT FOUND THEN RETURN 'Không tìm thấy tài khoản.'; END IF;
    IF v_toi IS NOT NULL AND lower(r.ten_dang_nhap) = lower(v_toi)
       AND (p_vai_tro <> r.vai_tro OR p_trang_thai <> 'HOAT_DONG') THEN
        RETURN 'Không tự hạ quyền hoặc tự khóa tài khoản đang đăng nhập.';
    END IF;
    IF r.vai_tro = 'gis_quan_tri' AND r.trang_thai = 'HOAT_DONG'
       AND (p_vai_tro <> 'gis_quan_tri' OR p_trang_thai <> 'HOAT_DONG')
       AND (SELECT count(*) FROM ql_he_thong.tai_khoan t WHERE t.vai_tro = 'gis_quan_tri' AND t.trang_thai = 'HOAT_DONG') <= 1 THEN
        RETURN 'Phải còn ít nhất một quản trị viên hoạt động.';
    END IF;
    UPDATE ql_he_thong.tai_khoan t
       SET vai_tro = p_vai_tro, trang_thai = p_trang_thai,
           so_lan_sai = CASE WHEN p_trang_thai = 'HOAT_DONG' THEN 0 ELSE t.so_lan_sai END,
           khoa_den   = CASE WHEN p_trang_thai = 'HOAT_DONG' THEN NULL ELSE t.khoa_den END,
           nguoi_duyet = CASE WHEN r.trang_thai = 'CHO_DUYET' THEN coalesce(v_toi, ql_he_thong.fn_vai_tro_hien_hanh()) ELSE t.nguoi_duyet END,
           ngay_duyet  = CASE WHEN r.trang_thai = 'CHO_DUYET' THEN now() ELSE t.ngay_duyet END
     WHERE t.id = p_id;
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu)
    VALUES (r.ten_dang_nhap, CASE WHEN r.trang_thai = 'CHO_DUYET' THEN 'DUYET_DANG_KY' ELSE 'PHAN_QUYEN' END,
            p_vai_tro || ' / ' || p_trang_thai || ' bởi ' || coalesce(v_toi, '?'));
    RETURN 'OK';
END $$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_tu_choi_dang_ky(p_id bigint)
RETURNS text LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
DECLARE v_ten text;
BEGIN
    IF NOT ql_he_thong.fn_la_quan_tri() THEN RETURN 'Chỉ quản trị viên được từ chối đăng ký.'; END IF;
    DELETE FROM ql_he_thong.tai_khoan t WHERE t.id = p_id AND t.trang_thai = 'CHO_DUYET' RETURNING t.ten_dang_nhap INTO v_ten;
    IF v_ten IS NULL THEN RETURN 'Chỉ từ chối được tài khoản đang chờ duyệt.'; END IF;
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu)
    VALUES (v_ten, 'TU_CHOI_DANG_KY', 'bởi ' || coalesce(nullif(current_setting('app.tai_khoan', true), ''), '?'));
    RETURN 'OK';
END $$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_dat_lai_mat_khau(p_id bigint, p_moi text)
RETURNS text LANGUAGE plpgsql VOLATILE SECURITY DEFINER
SET search_path = ql_he_thong, public, pg_temp AS $$
DECLARE v_loi text; v_ten text;
BEGIN
    IF NOT ql_he_thong.fn_la_quan_tri() THEN RETURN 'Chỉ quản trị viên được đặt lại mật khẩu.'; END IF;
    v_loi := ql_he_thong.fn_kiem_mat_khau(p_moi);
    IF v_loi IS NOT NULL THEN RETURN v_loi; END IF;
    UPDATE ql_he_thong.tai_khoan t
       SET mat_khau_bam = public.crypt(p_moi, public.gen_salt('bf', 10)), so_lan_sai = 0, khoa_den = NULL
     WHERE t.id = p_id RETURNING t.ten_dang_nhap INTO v_ten;
    IF v_ten IS NULL THEN RETURN 'Không tìm thấy tài khoản.'; END IF;
    INSERT INTO ql_he_thong.nhat_ky_dang_nhap (ten_dang_nhap, ket_qua, ghi_chu)
    VALUES (v_ten, 'DAT_LAI_MAT_KHAU', 'bởi ' || coalesce(nullif(current_setting('app.tai_khoan', true), ''), '?'));
    RETURN 'OK';
END $$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_nhat_ky_dang_nhap(p_gioi_han int DEFAULT 200)
RETURNS TABLE (thoi_diem timestamptz, ten_dang_nhap text, ket_qua text, ghi_chu text)
LANGUAGE plpgsql STABLE SECURITY DEFINER
SET search_path = ql_he_thong, pg_temp AS $$
BEGIN
    IF NOT ql_he_thong.fn_la_quan_tri() THEN
        RAISE EXCEPTION 'Chỉ quản trị viên được xem nhật ký đăng nhập.';
    END IF;
    RETURN QUERY SELECT n.thoi_diem, n.ten_dang_nhap::text, n.ket_qua::text, n.ghi_chu
                 FROM ql_he_thong.nhat_ky_dang_nhap n ORDER BY n.thoi_diem DESC LIMIT p_gioi_han;
END $$;

-- ---------------------------------------------------------------
-- Quyen thuc thi: moi vai tro ung dung goi duoc; ham quan tri tu kiem tra.
-- ---------------------------------------------------------------
DO $$
DECLARE f text; r text;
BEGIN
    FOREACH f IN ARRAY ARRAY[
        'fn_co_quan_tri_hoat_dong()', 'fn_dang_ky(text,text,text,text,text,text)', 'fn_dang_nhap(text,text)',
        'fn_dang_xuat(text)', 'fn_doi_mat_khau(text,text,text)', 'fn_ds_tai_khoan()',
        'fn_cap_nhat_tai_khoan(bigint,text,text)', 'fn_tu_choi_dang_ky(bigint)',
        'fn_dat_lai_mat_khau(bigint,text)', 'fn_nhat_ky_dang_nhap(int)', 'fn_la_quan_tri()', 'fn_kiem_mat_khau(text)']
    LOOP
        EXECUTE 'REVOKE ALL ON FUNCTION ql_he_thong.' || f || ' FROM PUBLIC';
        FOREACH r IN ARRAY ARRAY['managergis_app', 'gis_quan_tri', 'gis_bien_tap', 'gis_xem'] LOOP
            IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = r) THEN
                EXECUTE 'GRANT EXECUTE ON FUNCTION ql_he_thong.' || f || ' TO ' || r;
            END IF;
        END LOOP;
    END LOOP;
    FOREACH r IN ARRAY ARRAY['managergis_app', 'gis_quan_tri', 'gis_bien_tap', 'gis_xem'] LOOP
        IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = r) THEN
            EXECUTE 'GRANT USAGE ON SCHEMA ql_he_thong TO ' || r;
        END IF;
    END LOOP;
END $$;

COMMIT;
