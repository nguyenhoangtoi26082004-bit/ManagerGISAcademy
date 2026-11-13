-- ================================================================
-- 87_DinhTuyenPgRouting.sql
-- MANG DUONG BO + DINH TUYEN (pgRouting 3.6)
--
-- Nguon: ql_ban_do_nen.duong_osm (OpenStreetMap, ODbL; nap bang ogr2ogr
--        tu Overpass API, EPSG:32648).
-- B1. Cat moi tuyen OSM tai cac DINH DUNG CHUNG (nut giao) -> canh.
-- B2. Bang dinh (vertex) tu dau/cuoi canh; khoa = EWKB (bang nhau tuyet doi).
-- B3. Chi phi = thoi gian (giay) theo cap duong; 1 chieu -> reverse_cost = -1.
-- B4. Thanh phan lien thong (pgr_connectedComponents) -> chi "bat diem"
--     vao thanh phan lon nhat, tranh roi vao doan duong co lap.
-- Ham:
--   fn_dinh_tuyen(x1,y1,x2,y2)        duong ngan nhat theo THOI GIAN
--   fn_den_quan_y_gan_nhat(x,y)       co so quan y gan nhat theo MANG DUONG
--   fn_vung_tiep_can(x,y,phut[])      vung toi duoc trong N phut (isochrone)
-- Toa do vao/ra: EPSG:32648 (met).
-- Chay bang postgres tren HQTCSDL. Chay lai = dung lai mang.
-- ================================================================

SET client_encoding = 'UTF8';
SET work_mem = '256MB';

CREATE EXTENSION IF NOT EXISTS pgrouting;

BEGIN;

-- ---------------- B1. DINH CUA TUYEN + DEM SO LAN DUNG CHUNG ----------------
DROP TABLE IF EXISTS ql_ban_do_nen.tmp_diem;
CREATE UNLOGGED TABLE ql_ban_do_nen.tmp_diem AS
SELECT d.gid, (dp).path[1] AS i, ST_NPoints(d.geom) AS n, (dp).geom AS g, ST_AsEWKB((dp).geom) AS k
FROM ql_ban_do_nen.duong_osm d, ST_DumpPoints(d.geom) dp;

CREATE INDEX ON ql_ban_do_nen.tmp_diem (k);
CREATE INDEX ON ql_ban_do_nen.tmp_diem (gid, i);

DROP TABLE IF EXISTS ql_ban_do_nen.tmp_dung_chung;
CREATE UNLOGGED TABLE ql_ban_do_nen.tmp_dung_chung AS
SELECT k FROM ql_ban_do_nen.tmp_diem GROUP BY k HAVING count(*) > 1;
CREATE UNIQUE INDEX ON ql_ban_do_nen.tmp_dung_chung (k);

-- Nut = dau, cuoi, hoac dinh dung chung voi tuyen khac.
DROP TABLE IF EXISTS ql_ban_do_nen.tmp_nut;
CREATE UNLOGGED TABLE ql_ban_do_nen.tmp_nut AS
SELECT p.gid, p.i, lead(p.i) OVER (PARTITION BY p.gid ORDER BY p.i) AS j
FROM ql_ban_do_nen.tmp_diem p
WHERE p.i = 1 OR p.i = p.n OR EXISTS (SELECT 1 FROM ql_ban_do_nen.tmp_dung_chung c WHERE c.k = p.k);

-- ---------------- B1b. CANH ----------------
DROP TABLE IF EXISTS ql_ban_do_nen.canh CASCADE;
CREATE TABLE ql_ban_do_nen.canh AS
SELECT row_number() OVER ()::bigint AS id, z.gid, z.geom
FROM (
    SELECT n.gid, ST_MakeLine(p.g ORDER BY p.i) AS geom
    FROM ql_ban_do_nen.tmp_nut n
    JOIN ql_ban_do_nen.tmp_diem p ON p.gid = n.gid AND p.i BETWEEN n.i AND n.j
    WHERE n.j IS NOT NULL
    GROUP BY n.gid, n.i
) z
WHERE ST_NPoints(z.geom) >= 2 AND ST_Length(z.geom) > 0;

ALTER TABLE ql_ban_do_nen.canh ADD PRIMARY KEY (id);
ALTER TABLE ql_ban_do_nen.canh
    ADD COLUMN source BIGINT, ADD COLUMN target BIGINT,
    ADD COLUMN highway TEXT, ADD COLUMN ten TEXT, ADD COLUMN mot_chieu TEXT,
    ADD COLUMN dai_m DOUBLE PRECISION, ADD COLUMN toc_do_kmh DOUBLE PRECISION,
    ADD COLUMN cost DOUBLE PRECISION, ADD COLUMN reverse_cost DOUBLE PRECISION;

UPDATE ql_ban_do_nen.canh c
   SET highway = o.highway, ten = o.name, mot_chieu = lower(coalesce(o.oneway, '')),
       dai_m = ST_Length(c.geom)
  FROM ql_ban_do_nen.duong_osm o WHERE o.gid = c.gid;

-- ---------------- B2. DINH ----------------
DROP TABLE IF EXISTS ql_ban_do_nen.dinh CASCADE;
CREATE TABLE ql_ban_do_nen.dinh AS
SELECT row_number() OVER ()::bigint AS id, k, ST_GeomFromEWKB(k)::geometry(Point, 32648) AS geom
FROM (SELECT ST_AsEWKB(ST_StartPoint(geom)) AS k FROM ql_ban_do_nen.canh
      UNION
      SELECT ST_AsEWKB(ST_EndPoint(geom)) FROM ql_ban_do_nen.canh) u;
ALTER TABLE ql_ban_do_nen.dinh ADD PRIMARY KEY (id);
CREATE UNIQUE INDEX ON ql_ban_do_nen.dinh (k);

UPDATE ql_ban_do_nen.canh c SET source = d.id FROM ql_ban_do_nen.dinh d WHERE d.k = ST_AsEWKB(ST_StartPoint(c.geom));
UPDATE ql_ban_do_nen.canh c SET target = d.id FROM ql_ban_do_nen.dinh d WHERE d.k = ST_AsEWKB(ST_EndPoint(c.geom));

-- ---------------- B3. CHI PHI (giay) ----------------
UPDATE ql_ban_do_nen.canh SET toc_do_kmh = CASE highway
    WHEN 'motorway' THEN 80 WHEN 'motorway_link' THEN 45 WHEN 'trunk' THEN 60 WHEN 'trunk_link' THEN 40
    WHEN 'primary' THEN 45 WHEN 'primary_link' THEN 35 WHEN 'secondary' THEN 40 WHEN 'secondary_link' THEN 30
    WHEN 'tertiary' THEN 35 WHEN 'tertiary_link' THEN 25 WHEN 'unclassified' THEN 30
    WHEN 'residential' THEN 22 WHEN 'living_street' THEN 12 ELSE 20 END;
UPDATE ql_ban_do_nen.canh SET
    cost = CASE WHEN mot_chieu = '-1' THEN -1 ELSE dai_m / (toc_do_kmh / 3.6) END,
    reverse_cost = CASE WHEN mot_chieu IN ('yes', 'true', '1') THEN -1
                        WHEN mot_chieu = '-1' THEN dai_m / (toc_do_kmh / 3.6)
                        ELSE dai_m / (toc_do_kmh / 3.6) END;

CREATE INDEX idx_canh_geom ON ql_ban_do_nen.canh USING gist (geom);
CREATE INDEX idx_canh_source ON ql_ban_do_nen.canh (source);
CREATE INDEX idx_canh_target ON ql_ban_do_nen.canh (target);
CREATE INDEX idx_dinh_geom ON ql_ban_do_nen.dinh USING gist (geom);

-- ---------------- B4. THANH PHAN LIEN THONG ----------------
ALTER TABLE ql_ban_do_nen.dinh ADD COLUMN thanh_phan BIGINT;
UPDATE ql_ban_do_nen.dinh d SET thanh_phan = c.component
FROM pgr_connectedComponents('SELECT id, source, target, dai_m AS cost, dai_m AS reverse_cost FROM ql_ban_do_nen.canh') c
WHERE c.node = d.id;

DROP TABLE IF EXISTS ql_ban_do_nen.tmp_diem, ql_ban_do_nen.tmp_dung_chung, ql_ban_do_nen.tmp_nut;

-- Dinh thuoc thanh phan lon nhat (de bat diem).
CREATE OR REPLACE VIEW ql_ban_do_nen.v_dinh_chinh AS
SELECT d.* FROM ql_ban_do_nen.dinh d
WHERE d.thanh_phan = (SELECT thanh_phan FROM ql_ban_do_nen.dinh GROUP BY thanh_phan ORDER BY count(*) DESC LIMIT 1);

CREATE TABLE IF NOT EXISTS ql_ban_do_nen.thong_tin_mang (khoa TEXT PRIMARY KEY, gia_tri TEXT);
DELETE FROM ql_ban_do_nen.thong_tin_mang;
INSERT INTO ql_ban_do_nen.thong_tin_mang
SELECT 'so_canh', count(*)::text FROM ql_ban_do_nen.canh
UNION ALL SELECT 'so_dinh', count(*)::text FROM ql_ban_do_nen.dinh
UNION ALL SELECT 'thanh_phan_chinh', (SELECT thanh_phan FROM ql_ban_do_nen.dinh GROUP BY thanh_phan ORDER BY count(*) DESC LIMIT 1)::text
UNION ALL SELECT 'nguon', 'OpenStreetMap (ODbL) qua Overpass API, ' || to_char(now(), 'DD/MM/YYYY');

-- ---------------- HAM BAT DIEM ----------------
CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_dinh_gan_nhat(p_x DOUBLE PRECISION, p_y DOUBLE PRECISION)
RETURNS BIGINT LANGUAGE sql STABLE AS $f$
    SELECT id FROM ql_ban_do_nen.v_dinh_chinh
    ORDER BY geom <-> ST_SetSRID(ST_MakePoint(p_x, p_y), 32648) LIMIT 1;
$f$;

-- ---------------- DINH TUYEN ----------------
CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_dinh_tuyen(
    p_x1 DOUBLE PRECISION, p_y1 DOUBLE PRECISION, p_x2 DOUBLE PRECISION, p_y2 DOUBLE PRECISION)
RETURNS TABLE (wkt TEXT, dai_m DOUBLE PRECISION, phut DOUBLE PRECISION, so_doan INT, ten_duong TEXT)
LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path = ql_ban_do_nen, public AS $f$
DECLARE
    v1 BIGINT := ql_ban_do_nen.fn_dinh_gan_nhat(p_x1, p_y1);
    v2 BIGINT := ql_ban_do_nen.fn_dinh_gan_nhat(p_x2, p_y2);
    v_kc DOUBLE PRECISION := sqrt(power(p_x2 - p_x1, 2) + power(p_y2 - p_y1, 2));
    v_hop TEXT;
    v_sql TEXT;
    v_n INT;
BEGIN
    IF v1 IS NULL OR v2 IS NULL THEN RAISE EXCEPTION 'Chưa có mạng đường.'; END IF;
    IF v1 = v2 THEN
        wkt := ST_AsText(ST_MakeLine(ST_SetSRID(ST_MakePoint(p_x1, p_y1), 32648), ST_SetSRID(ST_MakePoint(p_x2, p_y2), 32648)));
        dai_m := v_kc; phut := v_kc / (20 / 3.6) / 60; so_doan := 0; ten_duong := '(rất gần)';
        RETURN NEXT; RETURN;
    END IF;
    -- Lan 1: chi xet canh trong hop bao (nhanh); khong co duong thi mo rong toan mang.
    FOR lan IN 1..2 LOOP
        v_hop := ST_AsEWKT(ST_Expand(ST_MakeEnvelope(least(p_x1, p_x2), least(p_y1, p_y2), greatest(p_x1, p_x2), greatest(p_y1, p_y2), 32648),
                                     greatest(3000, v_kc * 0.35)));
        v_sql := 'SELECT id, source, target, cost, reverse_cost FROM ql_ban_do_nen.canh' ||
                 CASE WHEN lan = 1 THEN ' WHERE geom && ST_GeomFromEWKT(' || quote_literal(v_hop) || ')' ELSE '' END;
        DROP TABLE IF EXISTS pg_temp.tuyen;
        CREATE TEMP TABLE tuyen ON COMMIT DROP AS
        SELECT r.seq, r.edge FROM pgr_dijkstra(v_sql, v1, v2, true) r WHERE r.edge > 0;
        GET DIAGNOSTICS v_n = ROW_COUNT;
        EXIT WHEN v_n > 0;
    END LOOP;
    IF v_n = 0 THEN RAISE EXCEPTION 'Không tìm được đường bộ nối hai điểm (mạng đường bị đứt).'; END IF;

    RETURN QUERY
    WITH c AS (SELECT t.seq, k.geom, k.dai_m AS d, k.dai_m / (k.toc_do_kmh / 3.6) AS s, k.ten
               FROM tuyen t JOIN ql_ban_do_nen.canh k ON k.id = t.edge),
         noi AS (SELECT ST_LineMerge(ST_Collect(
                     ARRAY[ST_MakeLine(ST_SetSRID(ST_MakePoint(p_x1, p_y1), 32648), (SELECT geom FROM ql_ban_do_nen.dinh WHERE id = v1))]
                     || array_agg(c.geom ORDER BY c.seq)
                     || ARRAY[ST_MakeLine((SELECT geom FROM ql_ban_do_nen.dinh WHERE id = v2), ST_SetSRID(ST_MakePoint(p_x2, p_y2), 32648))])) AS g,
                        sum(c.d) AS d, sum(c.s) AS s, count(*) AS n
                 FROM c),
         ten AS (SELECT string_agg(t, ' → ') AS ds FROM (
                    SELECT ten AS t, min(seq) m FROM c WHERE ten IS NOT NULL GROUP BY ten ORDER BY sum(d) DESC LIMIT 4) z)
    SELECT ST_AsText(noi.g), noi.d, noi.s / 60.0, noi.n::int, coalesce((SELECT ds FROM ten), '(đường không tên)')
    FROM noi;
END $f$;

-- ---------------- CO SO QUAN Y GAN NHAT (theo mang duong) ----------------
-- Chi xet 5 co so gan nhat theo duong chim bay + hop bao hep -> nhanh.
CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_den_quan_y_gan_nhat(p_x DOUBLE PRECISION, p_y DOUBLE PRECISION)
RETURNS TABLE (ma_don_vi TEXT, ten_don_vi TEXT, wkt TEXT, dai_m DOUBLE PRECISION, phut DOUBLE PRECISION)
LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path = ql_ban_do_nen, public AS $f$
DECLARE
    v1 BIGINT := ql_ban_do_nen.fn_dinh_gan_nhat(p_x, p_y);
    v_diem geometry := ST_SetSRID(ST_MakePoint(p_x, p_y), 32648);
    v_hop TEXT;
    r RECORD;
BEGIN
    DROP TABLE IF EXISTS pg_temp.ung_vien;
    CREATE TEMP TABLE ung_vien ON COMMIT DROP AS
    SELECT q.ma_don_vi::text AS ma, q.ten_don_vi::text AS ten, q.geom,
           ql_ban_do_nen.fn_dinh_gan_nhat(ST_X(q.geom), ST_Y(q.geom)) AS v
    FROM ql_don_vi.v_co_so_quan_y q ORDER BY q.geom <-> v_diem LIMIT 5;
    SELECT ST_AsEWKT(ST_Expand(ST_Envelope(ST_Collect(ARRAY(SELECT u.geom FROM ung_vien u) || v_diem)), 3000)) INTO v_hop;
    IF v_hop IS NULL THEN RAISE EXCEPTION 'Chưa có cơ sở quân y.'; END IF;
    SELECT u.ma, u.ten, ST_X(u.geom) AS x, ST_Y(u.geom) AS y, c.agg_cost INTO r
    FROM pgr_dijkstraCost(
            'SELECT id, source, target, cost, reverse_cost FROM ql_ban_do_nen.canh WHERE geom && ST_GeomFromEWKT(' || quote_literal(v_hop) || ')',
            v1, (SELECT array_agg(v) FROM ung_vien), true) c
    JOIN ung_vien u ON u.v = c.end_vid
    ORDER BY c.agg_cost LIMIT 1;
    IF r IS NULL THEN RAISE EXCEPTION 'Không tìm được cơ sở quân y nối mạng đường.'; END IF;
    RETURN QUERY SELECT r.ma, r.ten, t.wkt, t.dai_m, t.phut
                 FROM ql_ban_do_nen.fn_dinh_tuyen(p_x, p_y, r.x, r.y) t;
END $f$;

-- ---------------- VUNG TIEP CAN (isochrone) ----------------
-- Dinh toi duoc gom ve luoi 100 m truoc khi dung bao lom -> nhanh, muot.
CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_vung_tiep_can(p_x DOUBLE PRECISION, p_y DOUBLE PRECISION, p_phut INT[])
RETURNS TABLE (phut INT, wkt TEXT, dien_tich_km2 DOUBLE PRECISION)
LANGUAGE plpgsql STABLE SECURITY DEFINER SET search_path = ql_ban_do_nen, public AS $f$
DECLARE
    v1 BIGINT := ql_ban_do_nen.fn_dinh_gan_nhat(p_x, p_y);
    v_max INT := (SELECT max(m) FROM unnest(p_phut) m);
    v_hop TEXT := ST_AsEWKT(ST_Expand(ST_SetSRID(ST_MakePoint(p_x, p_y), 32648), v_max * 60 * 60 / 3.6));
BEGIN
    RETURN QUERY
    WITH dd AS (
        SELECT d.node, d.agg_cost
        FROM pgr_drivingDistance(
            'SELECT id, source, target, cost, reverse_cost FROM ql_ban_do_nen.canh WHERE geom && ST_GeomFromEWKT(' || quote_literal(v_hop) || ')',
            v1, v_max * 60, true) d),
    luoi AS (
        SELECT DISTINCT ON (o) ST_SnapToGrid(v.geom, 100) AS o, dd.agg_cost
        FROM dd JOIN ql_ban_do_nen.dinh v ON v.id = dd.node
        ORDER BY o, dd.agg_cost),
    muc AS (SELECT unnest(p_phut) AS m),
    vung AS (
        SELECT muc.m, ST_ConcaveHull(ST_Collect(luoi.o), 0.3) AS g
        FROM muc JOIN luoi ON luoi.agg_cost <= muc.m * 60
        GROUP BY muc.m)
    SELECT vung.m, ST_AsText(ST_ForcePolygonCW(ST_SimplifyPreserveTopology(ST_Buffer(vung.g, 60), 20))), ST_Area(vung.g) / 1e6
    FROM vung WHERE GeometryType(vung.g) IN ('POLYGON', 'MULTIPOLYGON')
    ORDER BY vung.m DESC;
END $f$;

GRANT USAGE ON SCHEMA ql_ban_do_nen TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;
GRANT SELECT ON ALL TABLES IN SCHEMA ql_ban_do_nen TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;
GRANT EXECUTE ON FUNCTION ql_ban_do_nen.fn_dinh_gan_nhat(DOUBLE PRECISION, DOUBLE PRECISION),
      ql_ban_do_nen.fn_dinh_tuyen(DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION),
      ql_ban_do_nen.fn_den_quan_y_gan_nhat(DOUBLE PRECISION, DOUBLE PRECISION),
      ql_ban_do_nen.fn_vung_tiep_can(DOUBLE PRECISION, DOUBLE PRECISION, INT[])
      TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;

COMMIT;

ANALYZE ql_ban_do_nen.canh;
ANALYZE ql_ban_do_nen.dinh;
