-- ================================================================
-- 00_TaoVaiTro.sql  –  chay MOT LAN tren may moi, TRUOC khi pg_restore.
-- Tao tai khoan ung dung + 3 vai tro phan quyen (RLS) ma CSDL su dung.
--
-- Cach chay (PowerShell, tai khoan postgres):
--   psql -U postgres -h 127.0.0.1 -p 5432 -v mat_khau_app="MatKhauCuaBan" -f 00_TaoVaiTro.sql
-- Mat khau nay chi nam tren may ban; KHONG commit len GitHub.
-- ================================================================
\set ON_ERROR_STOP on

\if :{?mat_khau_app}
\else
  \echo 'Hay truyen -v mat_khau_app="..." (it nhat 8 ky tu).'
  \quit
\endif
SELECT length(:'mat_khau_app') >= 8 AS du_dai \gset
\if :du_dai
\else
  \echo 'Mat khau phai co it nhat 8 ky tu.'
  \quit
\endif

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_quan_tri') THEN CREATE ROLE gis_quan_tri NOLOGIN; END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_bien_tap') THEN CREATE ROLE gis_bien_tap NOLOGIN; END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_xem')      THEN CREATE ROLE gis_xem NOLOGIN; END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'managergis_app') THEN CREATE ROLE managergis_app LOGIN; END IF;
END $$;

ALTER ROLE managergis_app WITH LOGIN PASSWORD :'mat_khau_app';
-- Ung dung dang nhap bang managergis_app roi SET ROLE theo vai tro cua tai khoan nguoi dung.
GRANT gis_quan_tri, gis_bien_tap, gis_xem TO managergis_app;

\echo 'Da tao vai tro. Buoc tiep: createdb + pg_restore (xem README).'
