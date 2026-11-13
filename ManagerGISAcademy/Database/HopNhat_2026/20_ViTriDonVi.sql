-- ================================================================
-- 20_ViTriDonVi.sql
-- CSDL: HQTCSDL  |  Schema: ql_don_vi
--
-- Tach VI TRI ra bang rieng co LICH SU va SIEU DU LIEU DO DAC:
--   - toa do goc dung nhu khi do/nhap (WGS84, VN-2000, UTM),
--   - phuong phap dinh vi, do chinh xac (m), nguon, nguoi xac minh,
--   - moi don vi chi co 1 vi tri hien hanh, vi tri cu duoc giu lai.
--
-- Hinh hoc luu tru thong nhat: EPSG:32648 (WGS84 / UTM 48N, met),
-- dung cho tinh khoang cach/vung dem va trung CRS ban do cua ung dung.
--
-- He VN-2000 cho Ha Noi: kinh tuyen truc 105°00', mui 3°, k = 0.9999
--   -> EPSG:5897 "VN-2000 / TM-3 zone 482" (truc E, N).
-- LUU Y QUY UOC VIET NAM: X = toa do BAC (N), Y = toa do DONG (E).
-- Bang nay luu theo ten cot ro nghia: e_goc (Dong/kinh do),
-- n_goc (Bac/vi do) de tranh dao truc.
--
-- Chay sau 10_DanhMucChuanHoa.sql, bang tai khoan postgres.
-- ================================================================

BEGIN;

SET LOCAL search_path = ql_don_vi, public;

-- ================================================================
-- 1. HAM LAY RANH GIOI HA NOI (EPSG:32648)
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_ranh_gioi_ha_noi()
RETURNS geometry
LANGUAGE sql
STABLE
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
    SELECT COALESCE(
        (SELECT ST_Transform(geom, 32648)
           FROM ql_don_vi.dia_gioi_hanh_chinh
          WHERE cap_hanh_chinh = 'thanh_pho'
            AND geom IS NOT NULL
          LIMIT 1),
        (SELECT ST_Transform(ST_Union(geom), 32648)
           FROM ql_don_vi.ranh_gioi_ha_noi_raw
          WHERE geom IS NOT NULL)
    );
$f$;

-- ================================================================
-- 2. BANG VI TRI DON VI
-- ================================================================

CREATE TABLE IF NOT EXISTS ql_don_vi.vi_tri_don_vi
(
    id             BIGSERIAL PRIMARY KEY,
    don_vi_id      BIGINT NOT NULL
                   REFERENCES ql_don_vi.don_vi_quan_doi (id)
                   ON DELETE CASCADE,

    -- Toa do goc, dung nhu so do / so hoa.
    srid_goc       INTEGER          NOT NULL,
    e_goc          DOUBLE PRECISION NOT NULL,  -- kinh do hoac toa do Dong (Y kieu VN)
    n_goc          DOUBLE PRECISION NOT NULL,  -- vi do hoac toa do Bac (X kieu VN)

    -- Hinh hoc luu tru, do trigger tinh tu toa do goc.
    geom           geometry(Point, 32648) NOT NULL,

    phuong_phap    VARCHAR(50)  NOT NULL
                   REFERENCES ql_don_vi.dm_phuong_phap_dinh_vi (ma),
    do_chinh_xac_m NUMERIC(10,2) NOT NULL,
    nguon          VARCHAR(1000) NOT NULL,
    nguoi_xac_minh VARCHAR(150),
    ngay_xac_dinh  DATE NOT NULL DEFAULT CURRENT_DATE,

    hieu_luc_tu    TIMESTAMPTZ NOT NULL DEFAULT now(),
    hieu_luc_den   TIMESTAMPTZ,
    la_hien_hanh   BOOLEAN     NOT NULL DEFAULT TRUE,
    ghi_chu        TEXT,

    CONSTRAINT ck_vt_srid_goc
        CHECK (srid_goc IN (4326, 4756, 5897, 3405, 32648)),
    CONSTRAINT ck_vt_do_chinh_xac
        CHECK (do_chinh_xac_m > 0),
    CONSTRAINT ck_vt_nguon
        CHECK (btrim(nguon) <> ''),
    CONSTRAINT ck_vt_ngay
        CHECK (ngay_xac_dinh <= CURRENT_DATE),
    CONSTRAINT ck_vt_hieu_luc
        CHECK (hieu_luc_den IS NULL OR hieu_luc_den >= hieu_luc_tu),
    CONSTRAINT ck_vt_hien_hanh_mo
        CHECK (NOT la_hien_hanh OR hieu_luc_den IS NULL),
    CONSTRAINT ck_vt_geom
        CHECK (NOT ST_IsEmpty(geom) AND ST_IsValid(geom))
);

COMMENT ON TABLE ql_don_vi.vi_tri_don_vi IS
    'Vi tri tru so don vi theo thoi gian. Moi don vi toi da 1 vi tri '
    'hien hanh. Vi tri lich su khong duoc sua/xoa truc tiep.';

-- Moi don vi chi co 1 vi tri hien hanh.
CREATE UNIQUE INDEX IF NOT EXISTS ux_vt_mot_hien_hanh
    ON ql_don_vi.vi_tri_don_vi (don_vi_id)
    WHERE la_hien_hanh;

CREATE INDEX IF NOT EXISTS ix_vt_geom
    ON ql_don_vi.vi_tri_don_vi USING gist (geom);

CREATE INDEX IF NOT EXISTS ix_vt_don_vi
    ON ql_don_vi.vi_tri_don_vi (don_vi_id);

-- ================================================================
-- 3. TRIGGER KIEM TRA TRUOC KHI GHI VI TRI
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_vi_tri_truoc_ghi()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
DECLARE
    v_pp           ql_don_vi.dm_phuong_phap_dinh_vi%ROWTYPE;
    v_kiem_chung   BOOLEAN;
    v_xa_id        BIGINT;
    v_xa_geom      geometry;
    v_xa_ten       TEXT;
    v_xa_dung      TEXT;
    v_ha_noi       geometry;
BEGIN
    -- 3.1. Vi tri lich su la bat bien.
    IF TG_OP = 'UPDATE' AND NOT OLD.la_hien_hanh THEN
        RAISE EXCEPTION
            'Vị trí lịch sử (id=%) không được sửa. Hãy thêm vị trí mới.',
            OLD.id;
    END IF;

    IF TG_OP = 'UPDATE' AND OLD.la_hien_hanh AND NOT NEW.la_hien_hanh THEN
        -- Chi dong hieu luc, khong doi noi dung toa do.
        IF (NEW.e_goc, NEW.n_goc, NEW.srid_goc, NEW.phuong_phap)
           IS DISTINCT FROM
           (OLD.e_goc, OLD.n_goc, OLD.srid_goc, OLD.phuong_phap) THEN
            RAISE EXCEPTION
                'Khi đóng hiệu lực vị trí không được đổi tọa độ.';
        END IF;
        NEW.hieu_luc_den := COALESCE(NEW.hieu_luc_den, now());
        RETURN NEW;
    END IF;

    -- 3.2. Phuong phap va do chinh xac.
    SELECT * INTO v_pp
      FROM ql_don_vi.dm_phuong_phap_dinh_vi
     WHERE ma = NEW.phuong_phap;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Phương pháp định vị "%" không có trong danh mục.',
            NEW.phuong_phap;
    END IF;

    NEW.do_chinh_xac_m := COALESCE(NEW.do_chinh_xac_m,
                                   v_pp.do_chinh_xac_danh_nghia_m);

    IF NEW.do_chinh_xac_m < v_pp.do_chinh_xac_danh_nghia_m THEN
        RAISE EXCEPTION
            'Phương pháp % chỉ đạt khoảng % m; không khai báo % m.',
            v_pp.ma, v_pp.do_chinh_xac_danh_nghia_m, NEW.do_chinh_xac_m;
    END IF;

    -- 3.3. Kiem tra mien gia tri theo he toa do goc (phat hien dao truc).
    IF NEW.srid_goc IN (4326, 4756) THEN
        IF NOT (NEW.e_goc BETWEEN 104.8 AND 106.5
                AND NEW.n_goc BETWEEN 20.2 AND 21.8) THEN
            RAISE EXCEPTION
                'Kinh độ %, vĩ độ % ngoài vùng Hà Nội. '
                'Có thể đã nhập ngược kinh độ/vĩ độ.',
                NEW.e_goc, NEW.n_goc;
        END IF;
    ELSE
        -- 5897 (TM-3 105°), 3405 / 32648 (UTM 48N): cung kinh tuyen 105°.
        IF NOT (NEW.e_goc BETWEEN 400000 AND 700000
                AND NEW.n_goc BETWEEN 2200000 AND 2450000) THEN
            RAISE EXCEPTION
                'Tọa độ phẳng E=%, N=% ngoài vùng Hà Nội. '
                'Lưu ý quy ước VN-2000: X là tọa độ Bắc (N), '
                'Y là tọa độ Đông (E).',
                NEW.e_goc, NEW.n_goc;
        END IF;
    END IF;

    -- 3.4. Tinh hinh hoc luu tru EPSG:32648 tu toa do goc.
    NEW.geom := ST_Transform(
                    ST_SetSRID(ST_MakePoint(NEW.e_goc, NEW.n_goc),
                               NEW.srid_goc),
                    32648);

    -- 3.5. Phai nam trong Ha Noi (dung sai 100 m do ranh gioi).
    v_ha_noi := ql_don_vi.fn_ranh_gioi_ha_noi();

    IF v_ha_noi IS NULL THEN
        RAISE EXCEPTION 'Chưa có ranh giới Hà Nội để kiểm tra vị trí.';
    END IF;

    IF NOT ST_DWithin(NEW.geom, v_ha_noi, 100) THEN
        RAISE EXCEPTION 'Vị trí nằm ngoài ranh giới Hà Nội.';
    END IF;

    -- 3.6. Phai nam trong xa/phuong da khai bao cua don vi
    --      (dung sai 50 m). Neu sai, goi y xa/phuong dung.
    SELECT dv.xa_phuong_id, x.geom, x.ten_xa_phuong
      INTO v_xa_id, v_xa_geom, v_xa_ten
      FROM ql_don_vi.don_vi_quan_doi dv
      JOIN ql_don_vi.dm_xa_phuong x ON x.id = dv.xa_phuong_id
     WHERE dv.id = NEW.don_vi_id;

    IF v_xa_geom IS NOT NULL
       AND NOT ST_DWithin(NEW.geom, ST_Transform(v_xa_geom, 32648), 50) THEN

        SELECT x.ten_xa_phuong INTO v_xa_dung
          FROM ql_don_vi.dm_xa_phuong x
         WHERE ST_Covers(ST_Transform(x.geom, 32648), NEW.geom)
         LIMIT 1;

        RAISE EXCEPTION
            'Vị trí không thuộc "%" như hồ sơ khai báo (điểm rơi vào "%"). '
            'Sửa xa_phuong_id hoặc kiểm tra lại tọa độ.',
            v_xa_ten, COALESCE(v_xa_dung, 'không xác định');
    END IF;

    -- 3.7. Ho so "da kiem chung" khong duoc dung diem dai dien.
    SELECT m.da_kiem_chung INTO v_kiem_chung
      FROM ql_don_vi.don_vi_quan_doi dv
      JOIN ql_don_vi.dm_muc_do_xac_minh m ON m.ma = dv.muc_do_xac_minh
     WHERE dv.id = NEW.don_vi_id;

    IF COALESCE(v_kiem_chung, FALSE)
       AND (NOT v_pp.la_vi_tri_thuc OR NEW.do_chinh_xac_m > 10) THEN
        RAISE EXCEPTION
            'Hồ sơ đã kiểm chứng cần vị trí thật, độ chính xác ≤ 10 m.';
    END IF;

    -- 3.8. Dong vi tri hien hanh cu (giu lam lich su).
    IF NEW.la_hien_hanh THEN
        NEW.hieu_luc_den := NULL;

        UPDATE ql_don_vi.vi_tri_don_vi
           SET la_hien_hanh = FALSE,
               hieu_luc_den = now()
         WHERE don_vi_id = NEW.don_vi_id
           AND la_hien_hanh
           AND id <> NEW.id;
    END IF;

    RETURN NEW;
END;
$f$;

DROP TRIGGER IF EXISTS trg_vi_tri_truoc_ghi ON ql_don_vi.vi_tri_don_vi;
CREATE TRIGGER trg_vi_tri_truoc_ghi
    BEFORE INSERT OR UPDATE ON ql_don_vi.vi_tri_don_vi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_vi_tri_truoc_ghi();

-- ================================================================
-- 4. KHONG XOA TRUC TIEP VI TRI (chi xoa theo don vi - CASCADE)
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_vi_tri_cam_xoa()
RETURNS trigger
LANGUAGE plpgsql
AS $f$
BEGIN
    -- pg_trigger_depth() > 1: dang xoa day chuyen tu bang don vi.
    IF pg_trigger_depth() > 1 THEN
        RETURN OLD;
    END IF;

    RAISE EXCEPTION
        'Không xóa trực tiếp vị trí (id=%). Hãy thêm vị trí mới thay thế.',
        OLD.id;
END;
$f$;

DROP TRIGGER IF EXISTS trg_vi_tri_cam_xoa ON ql_don_vi.vi_tri_don_vi;
CREATE TRIGGER trg_vi_tri_cam_xoa
    BEFORE DELETE ON ql_don_vi.vi_tri_don_vi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_vi_tri_cam_xoa();

-- ================================================================
-- 5. DONG BO COT geom CU CUA BANG DON VI (tuong thich ung dung)
--
-- don_vi_quan_doi.geom tro thanh BAN SAO CHI DOC cua vi tri
-- hien hanh. Moi thay doi phai di qua vi_tri_don_vi.
-- ================================================================

CREATE OR REPLACE FUNCTION ql_don_vi.fn_vi_tri_dong_bo_don_vi()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
BEGIN
    IF NEW.la_hien_hanh THEN
        PERFORM set_config('ql_don_vi.dang_dong_bo_vi_tri', 'on', TRUE);

        UPDATE ql_don_vi.don_vi_quan_doi
           SET geom = NEW.geom,
               ngay_cap_nhat = CURRENT_TIMESTAMP
         WHERE id = NEW.don_vi_id
           AND geom IS DISTINCT FROM NEW.geom;

        PERFORM set_config('ql_don_vi.dang_dong_bo_vi_tri', 'off', TRUE);
    END IF;

    RETURN NULL;
END;
$f$;

DROP TRIGGER IF EXISTS trg_vi_tri_dong_bo ON ql_don_vi.vi_tri_don_vi;
CREATE TRIGGER trg_vi_tri_dong_bo
    AFTER INSERT OR UPDATE ON ql_don_vi.vi_tri_don_vi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_vi_tri_dong_bo_don_vi();

CREATE OR REPLACE FUNCTION ql_don_vi.fn_don_vi_khoa_geom()
RETURNS trigger
LANGUAGE plpgsql
AS $f$
BEGIN
    IF NEW.geom IS DISTINCT FROM OLD.geom
       AND COALESCE(current_setting('ql_don_vi.dang_dong_bo_vi_tri', TRUE),
                    'off') <> 'on' THEN
        RAISE EXCEPTION
            'Không sửa trực tiếp don_vi_quan_doi.geom. '
            'Hãy thêm bản ghi vào ql_don_vi.vi_tri_don_vi.';
    END IF;
    RETURN NEW;
END;
$f$;

DROP TRIGGER IF EXISTS trg_don_vi_khoa_geom ON ql_don_vi.don_vi_quan_doi;
CREATE TRIGGER trg_don_vi_khoa_geom
    BEFORE UPDATE OF geom ON ql_don_vi.don_vi_quan_doi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_don_vi_khoa_geom();

-- Khong cho nang ho so len "da kiem chung" khi vi tri chua dat.
CREATE OR REPLACE FUNCTION ql_don_vi.fn_don_vi_kiem_tra_xac_minh()
RETURNS trigger
LANGUAGE plpgsql
SECURITY DEFINER
SET search_path = ql_don_vi, public
AS $f$
DECLARE
    v_kiem_chung BOOLEAN;
    v_dat        BOOLEAN;
BEGIN
    SELECT da_kiem_chung INTO v_kiem_chung
      FROM ql_don_vi.dm_muc_do_xac_minh
     WHERE ma = NEW.muc_do_xac_minh;

    IF NOT COALESCE(v_kiem_chung, FALSE) THEN
        RETURN NEW;
    END IF;

    SELECT p.la_vi_tri_thuc AND v.do_chinh_xac_m <= 10
      INTO v_dat
      FROM ql_don_vi.vi_tri_don_vi v
      JOIN ql_don_vi.dm_phuong_phap_dinh_vi p ON p.ma = v.phuong_phap
     WHERE v.don_vi_id = NEW.id
       AND v.la_hien_hanh;

    IF NOT COALESCE(v_dat, FALSE) THEN
        RAISE EXCEPTION
            'Đơn vị % chưa có vị trí thật với độ chính xác ≤ 10 m; '
            'không thể đặt mức "%".',
            NEW.ma_don_vi, NEW.muc_do_xac_minh;
    END IF;

    RETURN NEW;
END;
$f$;

DROP TRIGGER IF EXISTS trg_don_vi_kiem_tra_xac_minh ON ql_don_vi.don_vi_quan_doi;
CREATE TRIGGER trg_don_vi_kiem_tra_xac_minh
    BEFORE UPDATE OF muc_do_xac_minh ON ql_don_vi.don_vi_quan_doi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_don_vi_kiem_tra_xac_minh();

-- Ghi nhat ky dung ham chung da co.
DROP TRIGGER IF EXISTS trg_nhat_ky_vi_tri ON ql_don_vi.vi_tri_don_vi;
CREATE TRIGGER trg_nhat_ky_vi_tri
    AFTER INSERT OR UPDATE OR DELETE ON ql_don_vi.vi_tri_don_vi
    FOR EACH ROW EXECUTE FUNCTION ql_don_vi.fn_ghi_nhat_ky_v2();

-- ================================================================
-- 6. CHUYEN VI TRI HIEN CO SANG BANG MOI (TRUNG THUC VE NGUON GOC)
--
-- Khao sat ngay 12/11/2026: ca 43 diem hien co TRUNG KHOP
-- ST_PointOnSurface cua xa/phuong khai bao, tuc la diem dai dien,
-- KHONG phai vi tri tru so. Ghi dung ban chat do.
-- ================================================================

INSERT INTO ql_don_vi.vi_tri_don_vi
    (don_vi_id, srid_goc, e_goc, n_goc,
     phuong_phap, nguon, ngay_xac_dinh, ghi_chu)
SELECT
    dv.id,
    32648,
    ST_X(ST_Transform(dv.geom, 32648)),
    ST_Y(ST_Transform(dv.geom, 32648)),
    CASE
        WHEN ST_DWithin(ST_Transform(dv.geom, 32648),
                        ST_PointOnSurface(ST_Transform(x.geom, 32648)), 1)
        THEN 'DAI_DIEN_DIA_BAN'
        ELSE 'NHAP_TAY_CHUA_KIEM_CHUNG'
    END,
    'Chuyển từ cột don_vi_quan_doi.geom (dữ liệu trước hợp nhất 2026).',
    dv.ngay_xac_minh,
    'Cần định vị lại tại trụ sở/cổng chính.'
FROM ql_don_vi.don_vi_quan_doi dv
LEFT JOIN ql_don_vi.dm_xa_phuong x ON x.id = dv.xa_phuong_id
WHERE dv.geom IS NOT NULL
  AND NOT EXISTS (SELECT 1 FROM ql_don_vi.vi_tri_don_vi v
                   WHERE v.don_vi_id = dv.id);

COMMIT;
