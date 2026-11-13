-- ================================================================
-- 82_DinhViTheoOSM.sql
-- Dinh vi lai tru so cac don vi co dia chi / khuon vien cong khai.
--
-- PHUONG PHAP (trac dia):
--   1. Doi chieu ten + dia chi cong bo voi khuon vien da so hoa tren
--      OpenStreetMap (Overpass API, du lieu ODbL, truy cap 13/11/2026).
--   2. Diem tru so = ST_PointOnSurface cua khuon vien (luon nam TRONG
--      khuon vien, ke ca khuon vien loi lom), tinh trong UTM 48N.
--   3. Do chinh xac ghi nhan = ban kinh tuong duong cua khuon vien
--      (sqrt(S/pi)), toi thieu 50 m (= do chinh xac danh nghia cua
--      MA_HOA_DIA_CHI, trigger fn_vi_tri_truoc_ghi kiem tra): day la sai so so voi cong
--      chinh / toa nha chi huy, KHONG phai sai so do dac.
--   4. Ghi qua ql_don_vi.fn_dinh_vi_tru_so -> giu lich su vi tri cu,
--      tu tinh lai phuong/xa (ST_Covers), ghi nhat ky.
--
-- Don vi KHONG co dia chi cong khai (co quan tinh bao, tac chien mang,
-- cac co quan trong khu Bo Quoc phong, su doan/trung doan chua co nguon)
-- duoc GIU NGUYEN la "diem dai dien" - khong suy doan vi tri.
--
-- Chay bang postgres tren HQTCSDL. Chay lai an toan (bo qua neu vi tri
-- hien hanh da trung trong 1 m).
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

CREATE TEMP TABLE tmp_vi_tri_osm (
    ma_don_vi    TEXT PRIMARY KEY,
    kinh_do      DOUBLE PRECISION NOT NULL,
    vi_do        DOUBLE PRECISION NOT NULL,
    do_chinh_xac NUMERIC NOT NULL,
    nguon        TEXT NOT NULL,
    dia_chi      TEXT
) ON COMMIT DROP;

INSERT INTO tmp_vi_tri_osm VALUES
    ('HN-HVQP-001', 105.8018768, 21.0428547, 165, 'OpenStreetMap (ODbL) way/199956925 "Học viện Quốc phòng"', '93 Hoàng Quốc Việt, P. Nghĩa Đô'),
    ('HN-HVKTQS-001', 105.7856684, 21.0483736, 160, 'OpenStreetMap (ODbL) way/37770763 "Học viện Kĩ thuật Quân sự"', '236 Hoàng Quốc Việt, P. Nghĩa Đô'),
    ('HN-BCHH-001', 105.7868802, 21.0395155, 75, 'OpenStreetMap (ODbL) way/906824459 "Binh chủng Hóa học"', 'Khuôn viên BTL Hóa học, phố Phan Văn Trường, P. Cầu Giấy'),
    ('HN-BCTTG-001', 105.7797606, 21.0572845, 85, 'OpenStreetMap (ODbL) way/335145868 "Bộ Tư lệnh Tăng Thiết Giáp"', 'Khuôn viên BTL Tăng Thiết giáp, khu Cổ Nhuế, P. Đông Ngạc'),
    ('HN-BCCB-001', 105.8101255, 21.0393755, 75, 'OpenStreetMap (ODbL) way/549115329 "Bộ Tư lệnh Binh chủng Công binh"', 'Khuôn viên BTL Công binh, ngõ 6 phố Đội Nhân, P. Ngọc Hà'),
    ('HN-BCPB-001', 105.8090160, 21.0400968, 95, 'OpenStreetMap (ODbL) way/1287690845 "Bộ Tư lệnh Pháo binh - Tên lửa"', 'Khuôn viên BTL Pháo binh - Tên lửa, P. Ngọc Hà'),
    ('HN-BCTTLL-001', 105.8276090, 21.0308067, 65, 'OpenStreetMap (ODbL) way/560585022 "Bộ Tư lệnh Binh chủng Thông tin Liên lạc"', 'Số 1 Giang Văn Minh, P. Giảng Võ (hồ sơ mời thầu của BTL TTLL)'),
    ('HN-BTLBDBP-001', 105.8610930, 21.0197630, 50, 'OpenStreetMap (ODbL) way/977482364 "Bộ tư lệnh Bộ đội Biên phòng"', 'Số 4 Đinh Công Tráng / Trần Khánh Dư, P. Cửa Nam'),
    ('HN-BTLCSB-001', 105.7452631, 21.0115802, 50, 'OpenStreetMap (ODbL) way/942107829 "Bộ chỉ huy Cảnh sát biển Việt Nam"', 'Km6+825 Đại lộ Thăng Long, P. Xuân Phương'),
    ('HN-BTLLANG-001', 105.8341664, 21.0347129, 50, 'OpenStreetMap (ODbL) relation/21817964 "Bộ Tư lệnh Lăng Chủ tịch Hồ Chí Minh"', 'Số 2 Ông Ích Khiêm, P. Ba Đình'),
    ('HN-BTLPKKQ-001', 105.8331549, 20.9936043, 385, 'OpenStreetMap (ODbL) way/838560582 "Bộ Tư lệnh Quân chủng Không quân"', 'Khuôn viên Bộ Tư lệnh Quân chủng, đường Trường Chinh (cạnh BT PK-KQ 173C)'),
    ('HN-BTLTD-001', 105.7886567, 21.0111598, 105, 'OpenStreetMap (ODbL) relation/21364479 "Bộ Tư lệnh Thủ đô Hà Nội"', 'Khuôn viên Bộ Tư lệnh Thủ đô, P. Yên Hòa'),
    ('HN-BQP-001', 105.8414832, 21.0314030, 65, 'OpenStreetMap (ODbL) relation/21798564 "Tổng cục Công nghiệp Quốc phòng"', 'Số 7 Nguyễn Tri Phương, P. Ba Đình'),
    ('HN-TCCNQP-001', 105.7584861, 21.0088964, 120, 'OpenStreetMap (ODbL) way/1457384997 "Trụ sở Tổng cục Công nghiệp quốc phòng"', 'Trụ sở mới Km6+500 Đại lộ Thăng Long'),
    ('HN-HVBP-001', 105.4930385, 21.1135802, 200, 'OpenStreetMap (ODbL) way/951204738 "Học viện Biên phòng"', 'Phố Thanh Vị, P. Tùng Thiện (Sơn Tây)'),
    ('HN-HVCT-001', 105.7698572, 20.9738113, 230, 'OpenStreetMap (ODbL) way/942415474 "Học viện Chính trị"', '124 Ngô Quyền, P. Hà Đông'),
    ('HN-HVHC-001', 105.8762670, 21.0653482, 245, 'OpenStreetMap (ODbL) way/1175934342 "Học viện Hậu cần"', 'Đường Ngọc Thụy, P. Bồ Đề'),
    ('HN-HVKHQS-001', 105.8330932, 20.9878211, 125, 'OpenStreetMap (ODbL) way/1322067049 "Học viện Khoa học Quân sự"', '322E Lê Trọng Tấn (trụ sở chính)'),
    ('HN-HVPKKQ-001', 105.4828000, 21.0991215, 535, 'OpenStreetMap (ODbL) way/951420407 "Học viện Phòng không Không quân"', 'ĐT416, xã Đoài Phương (Sơn Tây)'),
    ('HN-HVQY-001', 105.7884888, 20.9665534, 150, 'OpenStreetMap (ODbL) way/838172055 "Học viện Quân y"', '160 Phùng Hưng, P. Hà Đông'),
    ('HN-DHVHNTQD-001', 105.8073565, 21.0172028, 50, 'OpenStreetMap (ODbL) way/906631345 "Trường Đại học Văn hóa Nghệ thuật Quân đội"', '101 Nguyễn Chí Thanh, P. Láng'),
    ('HN-SQLQ1-001', 105.5142371, 21.0565091, 550, 'OpenStreetMap (ODbL) way/870427259 "Trường Đại học Trần Quốc Tuấn"', 'QL21 - Đường Hồ Chí Minh, xã Đoài Phương'),
    ('HN-SQPB-001', 105.4810298, 21.1101913, 510, 'OpenStreetMap (ODbL) way/951420403 "Trường Sĩ quan Pháo binh"', 'Phố Thanh Vị, P. Tùng Thiện (Sơn Tây)'),
    ('HN-SD361-001', 105.8066299, 21.0084976, 120, 'OpenStreetMap (ODbL) way/1137757629 "Sư đoàn Phòng không 361"', '63A Lê Văn Lương'),
    ('HN-LD239-001', 105.9605111, 21.0415175, 95, 'OpenStreetMap (ODbL) way/1122073265 "Lữ đoàn Công binh 239"', 'Doanh trại theo OSM, khu vực Long Biên'),
    ('HN-E236-001', 105.9104901, 21.1262163, 140, 'OpenStreetMap (ODbL) way/1454442720 "Sư đoàn Phòng không 361 - Trung đoàn Tên lửa 236"', 'Doanh trại theo OSM (Sư đoàn 361 - Trung đoàn 236), đường Dục Tú');

DO $$
DECLARE
    r       RECORD;
    v_cu    geometry;
    v_moi   geometry;
    n_sua   INT := 0;
    n_bo    INT := 0;
BEGIN
    FOR r IN SELECT * FROM tmp_vi_tri_osm ORDER BY ma_don_vi LOOP
        SELECT b.geom INTO v_cu
        FROM ql_don_vi.v_ban_do_don_vi b WHERE b.ma_don_vi = r.ma_don_vi;
        IF NOT FOUND THEN
            RAISE WARNING 'Bo qua %: khong co trong v_ban_do_don_vi', r.ma_don_vi;
            CONTINUE;
        END IF;
        v_moi := ST_Transform(ST_SetSRID(ST_MakePoint(r.kinh_do, r.vi_do), 4326), 32648);
        IF ST_DWithin(v_cu, v_moi, 1.0) THEN
            n_bo := n_bo + 1;
            CONTINUE;
        END IF;
        PERFORM ql_don_vi.fn_dinh_vi_tru_so(
            r.ma_don_vi, 4326, r.kinh_do, r.vi_do,
            'MA_HOA_DIA_CHI', r.do_chinh_xac, r.nguon,
            'Script 82 (doi chieu OSM + dia chi cong bo)',
            'Đối chiếu: ' || COALESCE(r.dia_chi, '') ||
            '. Điểm = ST_PointOnSurface khuôn viên; dời ' ||
            round(ST_Distance(v_cu, v_moi))::text || ' m so với điểm cũ.');
        n_sua := n_sua + 1;
    END LOOP;
    RAISE NOTICE 'Dinh vi lai: % don vi, giu nguyen: %', n_sua, n_bo;
END $$;

COMMIT;
