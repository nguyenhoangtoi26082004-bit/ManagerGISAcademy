#!/bin/sh
# Khoi tao tai khoan ung dung mot lan tren volume moi.
# Khong in mat khau vao log; khong su dung root/superuser de ket noi ung dung.
set -eu
: "${APP_DB_USER:?Missing APP_DB_USER}"
: "${APP_DB_PASSWORD:?Missing APP_DB_PASSWORD}"

psql --set ON_ERROR_STOP=1 \
  --username "$POSTGRES_USER" \
  --dbname "$POSTGRES_DB" \
  --set "app_user=$APP_DB_USER" \
  --set "app_password=$APP_DB_PASSWORD" <<'SQL'
-- psql quotes identifiers and strings safely with :"name" and :'name'.
SELECT format('CREATE ROLE %I LOGIN PASSWORD %L', :'app_user', :'app_password')
WHERE NOT EXISTS (
  SELECT 1 FROM pg_catalog.pg_roles WHERE rolname = :'app_user'
)
\gexec

GRANT CONNECT ON DATABASE hanoi_military_gis TO :"app_user";
GRANT USAGE ON SCHEMA public TO :"app_user";
GRANT SELECT, INSERT, UPDATE, DELETE
  ON TABLE public.don_vi_quan_doi TO :"app_user";
GRANT USAGE, SELECT
  ON SEQUENCE public.don_vi_quan_doi_id_seq TO :"app_user";
GRANT SELECT
  ON TABLE public.nhat_ky_don_vi_quan_doi TO :"app_user";

-- Tai khoan ung dung khong phai superuser va khong co BYPASSRLS.
SELECT rolname, rolsuper, rolbypassrls, rolcanlogin
FROM pg_catalog.pg_roles WHERE rolname = :'app_user';
SQL
