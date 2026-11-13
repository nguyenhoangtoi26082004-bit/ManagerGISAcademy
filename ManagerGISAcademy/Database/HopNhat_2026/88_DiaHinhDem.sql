-- ================================================================
-- 88_DiaHinhDem.sql
-- DIA HINH: DEM SRTM 1" (~30 m, NASA/USGS, nguon AWS Terrain Tiles)
-- ghep + chieu ve EPSG:32648, nap vao ql_ban_do_nen.dem_srtm30 bang:
--   raster2pgsql -s 32648 -t 128x128 -I -C -M -N -32768 \
--       HANOI_DEM_SRTM30_32648.tif ql_ban_do_nen.dem_srtm30 | psql ...
-- (chay TRUOC script nay; script chi tao ham).
--
--   fn_do_cao(x, y)                 do cao (m) tai 1 diem (noi suy song tuyen)
--   fn_mat_cat(x1,y1,x2,y2,n)       n diem cach deu tren doan thang -> mat cat
-- Tam nhin thong (LOS) tinh o ung dung tu mat cat, co hieu chinh do cong
-- Trai Dat + khuc xa (k = 0,13).
-- Toa do EPSG:32648. Chay bang postgres tren HQTCSDL.
-- ================================================================

SET client_encoding = 'UTF8';

BEGIN;

CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_do_cao(p_x DOUBLE PRECISION, p_y DOUBLE PRECISION)
RETURNS DOUBLE PRECISION LANGUAGE sql STABLE SECURITY DEFINER SET search_path = ql_ban_do_nen, public AS $f$
    SELECT ST_Value(r.rast, 1, ST_SetSRID(ST_MakePoint(p_x, p_y), 32648), true, 'bilinear')
    FROM ql_ban_do_nen.dem_srtm30 r
    WHERE ST_Intersects(r.rast, ST_SetSRID(ST_MakePoint(p_x, p_y), 32648))
    LIMIT 1;
$f$;

CREATE OR REPLACE FUNCTION ql_ban_do_nen.fn_mat_cat(
    p_x1 DOUBLE PRECISION, p_y1 DOUBLE PRECISION, p_x2 DOUBLE PRECISION, p_y2 DOUBLE PRECISION,
    p_so_diem INT DEFAULT 200)
RETURNS TABLE (stt INT, khoang_cach_m DOUBLE PRECISION, do_cao_m DOUBLE PRECISION, x DOUBLE PRECISION, y DOUBLE PRECISION)
LANGUAGE sql STABLE SECURITY DEFINER SET search_path = ql_ban_do_nen, public AS $f$
    WITH tham AS (
        SELECT greatest(2, least(coalesce(p_so_diem, 200), 2000)) AS n,
               sqrt(power(p_x2 - p_x1, 2) + power(p_y2 - p_y1, 2)) AS L),
    diem AS (
        SELECT i, (i::float8 / (tham.n - 1)) AS t, tham.L
        FROM tham, generate_series(0, (SELECT n FROM tham) - 1) i),
    toa AS (
        SELECT i, t * L AS d, p_x1 + (p_x2 - p_x1) * t AS px, p_y1 + (p_y2 - p_y1) * t AS py FROM diem)
    SELECT (toa.i + 1)::int, toa.d,
           (SELECT ST_Value(r.rast, 1, ST_SetSRID(ST_MakePoint(toa.px, toa.py), 32648), true, 'bilinear')
              FROM ql_ban_do_nen.dem_srtm30 r
             WHERE ST_Intersects(r.rast, ST_SetSRID(ST_MakePoint(toa.px, toa.py), 32648)) LIMIT 1),
           toa.px, toa.py
    FROM toa ORDER BY toa.i;
$f$;

GRANT SELECT ON ql_ban_do_nen.dem_srtm30 TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;
GRANT EXECUTE ON FUNCTION ql_ban_do_nen.fn_do_cao(DOUBLE PRECISION, DOUBLE PRECISION),
      ql_ban_do_nen.fn_mat_cat(DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, INT)
      TO managergis_app, gis_quan_tri, gis_bien_tap, gis_xem;

COMMIT;
