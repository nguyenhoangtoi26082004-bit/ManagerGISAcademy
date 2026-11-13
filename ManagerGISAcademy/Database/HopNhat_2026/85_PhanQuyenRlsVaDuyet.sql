-- ================================================================
-- 85_PhanQuyenRlsVaDuyet.sql
-- PHAN QUYEN THEO VAI TRO + ROW LEVEL SECURITY + QUY TRINH DUYET
--
-- Vai tro (NOLOGIN, managergis_app duoc SET ROLE sang):
--   gis_quan_tri : xem tat ca, sua/xoa truc tiep, DUYET yeu cau.
--   gis_bien_tap : xem den muc "Mật"; them/sua/dinh vi -> XEP HANG cho duyet.
--   gis_xem      : chi xem, den muc "Nội bộ".
--   (managergis_app khong SET ROLE = quan tri, giu tuong thich cu.)
--
-- RLS: bang ql_don_vi.don_vi_quan_doi / vi_tri_don_vi / ho_so_nghiep_vu /
--   khu_vuc_don_vi chi tra ve dong co do mat <= muc cua vai tro.
--   Cac view ung dung doc duoc chuyen sang security_invoker = true de RLS
--   ap dung theo nguoi goi (PostgreSQL 15).
--
-- Duyet: trigger INSTEAD OF fn_ho_so_ung_dung_ghi va ham fn_dinh_vi_tru_so
--   duoc va them khoi [DUYET_V1]: neu vai tro = gis_bien_tap -> ghi vao
--   ql_he_thong.yeu_cau_thay_doi va KHONG doi du lieu goc.
--   Quan tri goi fn_duyet_yeu_cau / fn_tu_choi_yeu_cau.
--
-- Chay bang postgres tren HQTCSDL. Chay lai an toan.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

CREATE SCHEMA IF NOT EXISTS ql_he_thong;

-- ----------------------------------------------------------------
-- 1. VAI TRO
-- ----------------------------------------------------------------
DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_quan_tri') THEN CREATE ROLE gis_quan_tri NOLOGIN; END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_bien_tap') THEN CREATE ROLE gis_bien_tap NOLOGIN; END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_xem')      THEN CREATE ROLE gis_xem NOLOGIN; END IF;
END $$;
COMMENT ON ROLE gis_quan_tri IS 'Quan tri: xem toan bo, sua truc tiep, duyet yeu cau.';
COMMENT ON ROLE gis_bien_tap IS 'Bien tap: xem den muc Mat, thay doi phai qua duyet.';
COMMENT ON ROLE gis_xem      IS 'Chi xem: den muc Noi bo.';

GRANT gis_quan_tri, gis_bien_tap, gis_xem TO managergis_app;

-- ----------------------------------------------------------------
-- 2. HAM VAI TRO / MUC DO MAT
-- ----------------------------------------------------------------
-- Vai tro HIEU LUC: gia tri SET ROLE cua phien (GUC "role" khong doi khi
-- vao ham SECURITY DEFINER) hoac session_user neu khong SET ROLE.
CREATE OR REPLACE FUNCTION ql_he_thong.fn_vai_tro_hien_hanh()
RETURNS TEXT LANGUAGE sql STABLE AS $f$
    SELECT CASE WHEN coalesce(current_setting('role', true), 'none') IN ('none', '')
                THEN session_user::text
                ELSE current_setting('role', true) END;
$f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_muc_do_mat(p_do_mat TEXT)
RETURNS INT LANGUAGE sql IMMUTABLE AS $f$
    SELECT CASE lower(btrim(coalesce(p_do_mat, '')))
        WHEN '' THEN 0 WHEN 'nội bộ' THEN 1 WHEN 'mật' THEN 2
        WHEN 'tối mật' THEN 3 WHEN 'tuyệt mật' THEN 4 ELSE 1 END;
$f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_muc_vai_tro(p_vai_tro TEXT)
RETURNS INT LANGUAGE sql IMMUTABLE AS $f$
    SELECT CASE p_vai_tro WHEN 'gis_xem' THEN 1 WHEN 'gis_bien_tap' THEN 2 ELSE 4 END;
$f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_duoc_xem_muc(p_vai_tro TEXT, p_do_mat TEXT)
RETURNS BOOLEAN LANGUAGE sql IMMUTABLE AS $f$
    SELECT ql_he_thong.fn_muc_do_mat(p_do_mat) <= ql_he_thong.fn_muc_vai_tro(p_vai_tro);
$f$;

-- Dung trong chinh sach RLS: doc do mat bang SECURITY DEFINER (tranh de quy RLS).
CREATE OR REPLACE FUNCTION ql_he_thong.fn_duoc_xem_don_vi(p_don_vi_id BIGINT)
RETURNS BOOLEAN LANGUAGE sql STABLE SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
    SELECT ql_he_thong.fn_duoc_xem_muc(
               ql_he_thong.fn_vai_tro_hien_hanh(),
               (SELECT h.do_mat FROM ql_don_vi.ho_so_nghiep_vu h WHERE h.don_vi_id = p_don_vi_id));
$f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_can_duyet()
RETURNS BOOLEAN LANGUAGE sql STABLE AS $f$
    SELECT ql_he_thong.fn_vai_tro_hien_hanh() = 'gis_bien_tap'
       AND coalesce(current_setting('ql.dang_duyet', true), '') <> '1';
$f$;

-- ----------------------------------------------------------------
-- 3. BANG YEU CAU THAY DOI
-- ----------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ql_he_thong.yeu_cau_thay_doi
(
    id              BIGSERIAL PRIMARY KEY,
    loai            VARCHAR(10) NOT NULL CHECK (loai IN ('THEM', 'SUA', 'XOA', 'DINH_VI')),
    ma_don_vi       VARCHAR(50) NOT NULL,
    ten_don_vi      VARCHAR(255),
    du_lieu_cu      JSONB,
    du_lieu_moi     JSONB,
    tham_so         JSONB,
    nguoi_gui       TEXT NOT NULL DEFAULT ql_he_thong.fn_vai_tro_hien_hanh(),
    thoi_gian_gui   TIMESTAMPTZ NOT NULL DEFAULT now(),
    trang_thai      VARCHAR(10) NOT NULL DEFAULT 'CHO_DUYET'
                    CHECK (trang_thai IN ('CHO_DUYET', 'DA_DUYET', 'TU_CHOI', 'LOI')),
    nguoi_duyet     TEXT,
    thoi_gian_duyet TIMESTAMPTZ,
    ghi_chu_duyet   TEXT,
    CONSTRAINT ck_yc_duyet_du CHECK (
        trang_thai = 'CHO_DUYET' OR (nguoi_duyet IS NOT NULL AND thoi_gian_duyet IS NOT NULL))
);
CREATE INDEX IF NOT EXISTS idx_yc_trang_thai ON ql_he_thong.yeu_cau_thay_doi (trang_thai, thoi_gian_gui DESC);

-- ----------------------------------------------------------------
-- 4. VA KHOI [DUYET_V1] VAO TRIGGER GHI QUA VIEW VA HAM DINH VI
-- ----------------------------------------------------------------
DO $$
DECLARE
    v_def TEXT;
    v_khoi TEXT;
BEGIN
    -- 4.1. Trigger INSTEAD OF cua view public.don_vi_quan_doi
    v_def := pg_get_functiondef('ql_don_vi.fn_ho_so_ung_dung_ghi()'::regprocedure);
    IF position('[DUYET_V1]' IN v_def) = 0 THEN
        v_khoi := E'\nBEGIN\n'
            || E'    -- [DUYET_V1] Bien tap vien: xep hang cho duyet, KHONG ghi du lieu goc.\n'
            || E'    IF ql_he_thong.fn_can_duyet() THEN\n'
            || E'        INSERT INTO ql_he_thong.yeu_cau_thay_doi (loai, ma_don_vi, ten_don_vi, du_lieu_cu, du_lieu_moi)\n'
            || E'        VALUES (CASE TG_OP WHEN ''INSERT'' THEN ''THEM'' WHEN ''UPDATE'' THEN ''SUA'' ELSE ''XOA'' END,\n'
            || E'                CASE WHEN TG_OP = ''DELETE'' THEN OLD.ma_don_vi ELSE NEW.ma_don_vi END,\n'
            || E'                CASE WHEN TG_OP = ''DELETE'' THEN OLD.ten_don_vi ELSE NEW.ten_don_vi END,\n'
            || E'                CASE WHEN TG_OP <> ''INSERT'' THEN to_jsonb(OLD) END,\n'
            || E'                CASE WHEN TG_OP <> ''DELETE'' THEN to_jsonb(NEW) END);\n'
            || E'        IF TG_OP = ''DELETE'' THEN RETURN OLD; END IF;\n'
            || E'        RETURN NEW;\n'
            || E'    END IF;\n';
        v_def := regexp_replace(v_def, E'\\nBEGIN\\n', v_khoi);
        EXECUTE v_def;
    END IF;

    -- 4.2. Ham dinh vi lai tru so
    v_def := pg_get_functiondef(
        'ql_don_vi.fn_dinh_vi_tru_so(text,integer,double precision,double precision,text,numeric,text,text,text)'::regprocedure);
    IF position('[DUYET_V1]' IN v_def) = 0 THEN
        -- Kiem tra quyen theo vai tro HIEU LUC (SET ROLE), khong theo session_user.
        v_def := replace(v_def,
            'has_table_privilege(session_user, ''public.don_vi_quan_doi'', ''UPDATE'')',
            'has_table_privilege(ql_he_thong.fn_vai_tro_hien_hanh(), ''public.don_vi_quan_doi'', ''UPDATE'')');
        v_khoi := E'\nBEGIN\n'
            || E'    -- [DUYET_V1] Bien tap vien: luu yeu cau dinh vi cho duyet.\n'
            || E'    IF ql_he_thong.fn_can_duyet() THEN\n'
            || E'        INSERT INTO ql_he_thong.yeu_cau_thay_doi (loai, ma_don_vi, ten_don_vi, du_lieu_cu, tham_so)\n'
            || E'        SELECT ''DINH_VI'', d.ma_don_vi, d.ten_don_vi,\n'
            || E'               jsonb_build_object(''kinh_do'', u.kinh_do, ''vi_do'', u.vi_do),\n'
            || E'               jsonb_build_object(''srid'', p_srid, ''e'', p_e, ''n'', p_n, ''phuong_phap'', p_phuong_phap,\n'
            || E'                   ''do_chinh_xac'', p_do_chinh_xac, ''nguon'', p_nguon, ''nguoi'', p_nguoi, ''ghi_chu'', p_ghi_chu)\n'
            || E'        FROM ql_don_vi.don_vi_quan_doi d LEFT JOIN public.don_vi_quan_doi u ON u.id = d.id\n'
            || E'        WHERE d.ma_don_vi = btrim(p_ma_don_vi)\n'
            || E'        RETURNING -id INTO v_id;\n'
            || E'        IF v_id IS NULL THEN RAISE EXCEPTION ''Không tìm thấy đơn vị "%".'', p_ma_don_vi; END IF;\n'
            || E'        RETURN QUERY SELECT v_id, ''CHỜ DUYỆT''::text, FALSE, p_do_chinh_xac, 0::double precision;\n'
            || E'        RETURN;\n'
            || E'    END IF;\n';
        v_def := regexp_replace(v_def, E'\\nBEGIN\\n', v_khoi);
        EXECUTE v_def;
    END IF;
END $$;

-- ----------------------------------------------------------------
-- 5. DUYET / TU CHOI
-- ----------------------------------------------------------------
CREATE OR REPLACE FUNCTION ql_he_thong.fn_duyet_yeu_cau(p_id BIGINT, p_ghi_chu TEXT DEFAULT NULL)
RETURNS TEXT LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE
    r ql_he_thong.yeu_cau_thay_doi%ROWTYPE;
    v_vt TEXT := ql_he_thong.fn_vai_tro_hien_hanh();
    v_cot TEXT;
    v_kq TEXT;
BEGIN
    IF v_vt IN ('gis_bien_tap', 'gis_xem') THEN
        RAISE EXCEPTION 'Chỉ quản trị viên được duyệt yêu cầu (vai trò hiện tại: %).', v_vt;
    END IF;
    SELECT * INTO r FROM ql_he_thong.yeu_cau_thay_doi WHERE id = p_id FOR UPDATE;
    IF NOT FOUND THEN RAISE EXCEPTION 'Không có yêu cầu #%.', p_id; END IF;
    IF r.trang_thai <> 'CHO_DUYET' THEN RAISE EXCEPTION 'Yêu cầu #% đã xử lý (%).', p_id, r.trang_thai; END IF;

    PERFORM set_config('ql.dang_duyet', '1', true);
    BEGIN
        IF r.loai = 'DINH_VI' THEN
            PERFORM ql_don_vi.fn_dinh_vi_tru_so(r.ma_don_vi,
                (r.tham_so ->> 'srid')::int, (r.tham_so ->> 'e')::float8, (r.tham_so ->> 'n')::float8,
                r.tham_so ->> 'phuong_phap', (r.tham_so ->> 'do_chinh_xac')::numeric,
                r.tham_so ->> 'nguon', coalesce(r.tham_so ->> 'nguoi', r.nguoi_gui),
                coalesce(r.tham_so ->> 'ghi_chu', '') || ' [duyệt yêu cầu #' || r.id || ']');
            v_kq := 'Đã định vị lại ' || r.ma_don_vi;
        ELSIF r.loai = 'SUA' THEN
            SELECT string_agg(quote_ident(column_name), ', ' ORDER BY ordinal_position) INTO v_cot
            FROM information_schema.columns
            WHERE table_schema = 'public' AND table_name = 'don_vi_quan_doi' AND column_name <> 'id';
            EXECUTE format('UPDATE public.don_vi_quan_doi SET (%s) = (SELECT %s FROM jsonb_populate_record(NULL::public.don_vi_quan_doi, $1)) WHERE id = $2',
                           v_cot, v_cot)
            USING r.du_lieu_moi, (r.du_lieu_cu ->> 'id')::bigint;
            v_kq := 'Đã cập nhật hồ sơ ' || r.ma_don_vi;
        ELSIF r.loai = 'THEM' THEN
            INSERT INTO public.don_vi_quan_doi
            SELECT * FROM jsonb_populate_record(NULL::public.don_vi_quan_doi, r.du_lieu_moi);
            v_kq := 'Đã thêm hồ sơ ' || r.ma_don_vi;
        ELSE
            DELETE FROM public.don_vi_quan_doi WHERE id = (r.du_lieu_cu ->> 'id')::bigint;
            v_kq := 'Đã chuyển lưu trữ ' || r.ma_don_vi;
        END IF;
        UPDATE ql_he_thong.yeu_cau_thay_doi
           SET trang_thai = 'DA_DUYET', nguoi_duyet = v_vt, thoi_gian_duyet = now(), ghi_chu_duyet = p_ghi_chu
         WHERE id = p_id;
    EXCEPTION WHEN OTHERS THEN
        PERFORM set_config('ql.dang_duyet', '0', true);
        UPDATE ql_he_thong.yeu_cau_thay_doi
           SET trang_thai = 'LOI', nguoi_duyet = v_vt, thoi_gian_duyet = now(),
               ghi_chu_duyet = 'Lỗi khi áp dụng: ' || SQLERRM
         WHERE id = p_id;
        RETURN 'LỖI: ' || SQLERRM;
    END;
    PERFORM set_config('ql.dang_duyet', '0', true);
    RETURN v_kq;
END $f$;

CREATE OR REPLACE FUNCTION ql_he_thong.fn_tu_choi_yeu_cau(p_id BIGINT, p_ly_do TEXT)
RETURNS TEXT LANGUAGE plpgsql SECURITY DEFINER SET search_path = ql_don_vi, public AS $f$
DECLARE v_vt TEXT := ql_he_thong.fn_vai_tro_hien_hanh();
BEGIN
    IF v_vt IN ('gis_bien_tap', 'gis_xem') THEN
        RAISE EXCEPTION 'Chỉ quản trị viên được từ chối yêu cầu.';
    END IF;
    IF nullif(btrim(p_ly_do), '') IS NULL THEN RAISE EXCEPTION 'Phải ghi lý do từ chối.'; END IF;
    UPDATE ql_he_thong.yeu_cau_thay_doi
       SET trang_thai = 'TU_CHOI', nguoi_duyet = v_vt, thoi_gian_duyet = now(), ghi_chu_duyet = p_ly_do
     WHERE id = p_id AND trang_thai = 'CHO_DUYET';
    IF NOT FOUND THEN RAISE EXCEPTION 'Yêu cầu #% không còn chờ duyệt.', p_id; END IF;
    RETURN 'Đã từ chối yêu cầu #' || p_id;
END $f$;

-- ----------------------------------------------------------------
-- 6. ROW LEVEL SECURITY
-- ----------------------------------------------------------------
ALTER TABLE ql_don_vi.don_vi_quan_doi ENABLE ROW LEVEL SECURITY;
ALTER TABLE ql_don_vi.vi_tri_don_vi   ENABLE ROW LEVEL SECURITY;
ALTER TABLE ql_don_vi.ho_so_nghiep_vu ENABLE ROW LEVEL SECURITY;
ALTER TABLE ql_don_vi.khu_vuc_don_vi  ENABLE ROW LEVEL SECURITY;

DROP POLICY IF EXISTS p_xem_theo_do_mat ON ql_don_vi.don_vi_quan_doi;
CREATE POLICY p_xem_theo_do_mat ON ql_don_vi.don_vi_quan_doi
    FOR SELECT USING (ql_he_thong.fn_duoc_xem_don_vi(id));
DROP POLICY IF EXISTS p_xem_theo_do_mat ON ql_don_vi.vi_tri_don_vi;
CREATE POLICY p_xem_theo_do_mat ON ql_don_vi.vi_tri_don_vi
    FOR SELECT USING (ql_he_thong.fn_duoc_xem_don_vi(don_vi_id));
DROP POLICY IF EXISTS p_xem_theo_do_mat ON ql_don_vi.ho_so_nghiep_vu;
CREATE POLICY p_xem_theo_do_mat ON ql_don_vi.ho_so_nghiep_vu
    FOR SELECT USING (ql_he_thong.fn_duoc_xem_muc(ql_he_thong.fn_vai_tro_hien_hanh(), do_mat));
DROP POLICY IF EXISTS p_xem_theo_do_mat ON ql_don_vi.khu_vuc_don_vi;
CREATE POLICY p_xem_theo_do_mat ON ql_don_vi.khu_vuc_don_vi
    FOR SELECT USING (don_vi_id IS NULL OR ql_he_thong.fn_duoc_xem_don_vi(don_vi_id));

-- View ung dung doc: chay theo quyen NGUOI GOI de RLS co hieu luc.
DO $$
DECLARE v TEXT;
BEGIN
    FOREACH v IN ARRAY ARRAY[
        'public.don_vi_quan_doi', 'ql_don_vi.v_ban_do_don_vi', 'ql_don_vi.v_don_vi_ban_do',
        'ql_don_vi.v_kiem_tra_vi_tri', 'ql_don_vi.v_thong_ke_vi_tri', 'ql_don_vi.v_co_so_quan_y',
        'ql_don_vi.v_ho_so_don_vi', 'ql_don_vi.v_don_vi_tra_cuu', 'ql_don_vi.v_tong_quan_he_thong']
    LOOP
        IF to_regclass(v) IS NOT NULL THEN
            EXECUTE format('ALTER VIEW %s SET (security_invoker = true)', v);
        END IF;
    END LOOP;
END $$;

-- ----------------------------------------------------------------
-- 7. QUYEN (view security_invoker can SELECT tren bang goc)
-- ----------------------------------------------------------------
GRANT USAGE ON SCHEMA public, ql_don_vi, ql_he_thong, ql_kiem_thu TO gis_quan_tri, gis_bien_tap, gis_xem, managergis_app;
GRANT SELECT ON ALL TABLES IN SCHEMA ql_don_vi TO gis_quan_tri, gis_bien_tap, gis_xem, managergis_app;
GRANT SELECT ON ALL TABLES IN SCHEMA public    TO gis_quan_tri, gis_bien_tap, gis_xem, managergis_app;
GRANT SELECT ON ql_he_thong.yeu_cau_thay_doi, ql_he_thong.mv_thong_ke_loai_xa
      TO gis_quan_tri, gis_bien_tap, gis_xem, managergis_app;

GRANT INSERT, UPDATE ON public.don_vi_quan_doi TO gis_quan_tri, gis_bien_tap;
GRANT DELETE ON public.don_vi_quan_doi TO gis_quan_tri;

-- Sao quyen EXECUTE cua managergis_app sang 3 vai tro.
DO $$
DECLARE r RECORD;
BEGIN
    FOR r IN
        SELECT DISTINCT p.oid::regprocedure AS sig
        FROM pg_proc p JOIN pg_namespace n ON n.oid = p.pronamespace
        WHERE n.nspname IN ('public', 'ql_don_vi', 'ql_he_thong', 'ql_kiem_thu')
          AND has_function_privilege('managergis_app', p.oid, 'EXECUTE')
          AND p.prokind = 'f'
          AND NOT EXISTS (SELECT 1 FROM pg_depend d WHERE d.objid = p.oid AND d.deptype = 'e')
    LOOP
        EXECUTE format('GRANT EXECUTE ON FUNCTION %s TO gis_quan_tri, gis_bien_tap, gis_xem', r.sig);
    END LOOP;
END $$;

REVOKE ALL ON FUNCTION ql_he_thong.fn_duyet_yeu_cau(BIGINT, TEXT), ql_he_thong.fn_tu_choi_yeu_cau(BIGINT, TEXT) FROM PUBLIC;
GRANT EXECUTE ON FUNCTION ql_he_thong.fn_duyet_yeu_cau(BIGINT, TEXT), ql_he_thong.fn_tu_choi_yeu_cau(BIGINT, TEXT)
      TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;   -- ham tu kiem tra vai tro
GRANT EXECUTE ON FUNCTION ql_he_thong.fn_vai_tro_hien_hanh(), ql_he_thong.fn_muc_do_mat(TEXT),
      ql_he_thong.fn_muc_vai_tro(TEXT), ql_he_thong.fn_duoc_xem_muc(TEXT, TEXT),
      ql_he_thong.fn_duoc_xem_don_vi(BIGINT), ql_he_thong.fn_can_duyet()
      TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;

COMMIT;
