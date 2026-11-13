# PostgreSQL + PostGIS Docker cho ManagerGISAcademy

## 1. Chuẩn bị Windows

- Mở Docker Desktop (Linux containers / WSL2) cho tới khi Engine đang chạy.
- Mở PowerShell trong thư mục có `compose.yaml`.
- Sao chép `.env.example` thành `.env`: `Copy-Item .env.example .env`
- Mở `.env` để nhập 2 mật khẩu mới, riêng biệt: `notepad .env`
- Không chia sẻ `.env`, không commit lên GitHub.

## 2. Tạo database

```powershell
docker compose up -d
docker compose ps
docker compose logs --tail=100 postgis
```

Container tên service `postgis`; image `postgis/postgis:15-3.4`.
Host 127.0.0.1, port 5434 (độc lập server cũ 5433). PostgreSQL nội bộ container ở port 5432.

Ở lần khởi tạo volume mới, Docker tự chạy lần lượt các script:
1. Script PostGIS tích hợp sẵn trong image.
2. `20_01_CreateTables.sql`.
3. `21_02_CreateIndexes.sql`.
4. `22_03_SpatialFunctions.sql` (phiên bản sửa lỗi từ ZIP người dùng, nơi bản cũ vô tình lặp lại nội dung 02).
5. `30_create_app_role.sh`: tạo role `managergis_app`, cấp quyền trên bảng và sequence.

## 3. Kiểm tra

```powershell
docker compose exec postgis psql -U postgres -d hanoi_military_gis -c "SELECT PostGIS_Version();"
docker compose exec postgis psql -U postgres -d hanoi_military_gis -c "SELECT COUNT(*) FROM public.don_vi_quan_doi;"
docker compose exec postgis psql -U postgres -d hanoi_military_gis -c "SELECT rolname,rolsuper,rolbypassrls FROM pg_roles WHERE rolname='managergis_app';"
```

## 4. Kết nối từ pgAdmin / WinForms

PostgreSQL server: `127.0.0.1`, port `5434`, database `hanoi_military_gis`, user `managergis_app`, password là `PG_APP_PASSWORD` trong `.env`.

Đặt connection string của `PostgisHanoi` trong App.config (không chia sẻ mật khẩu):

```xml
<connectionStrings>
  <add name="PostgisHanoi"
       connectionString="Host=127.0.0.1;Port=5434;Database=hanoi_military_gis;Username=managergis_app;Password=MAT_KHAU_UNG_DUNG;Timeout=15;Command Timeout=30;"
       providerName="Npgsql" />
</connectionStrings>
```

Nếu có ký tự `&` trong mật khẩu cần mã hóa XML thành `&amp;`. `DbConnection.cs` của dự án còn hỗ trợ biến môi trường `MANAGERGISACADEMY_POSTGIS`, ưu tiên hơn `App.config`; nếu đã đặt biến này thì phải đổi chuỗi kết nối trong biến môi trường.

## 5. Lưu ý quan trọng

- Tạo volume Docker không sao chép dữ liệu đã có ở PostgreSQL cũ `127.0.0.1:5433`. Muốn chuyển phải dump/restore riêng, có sao lưu trước.
- SQL chỉ tạo schema, chỉ mục và hàm không gian. Nó không tạo dữ liệu đơn vị mẫu nên tổng bản ghi ban đầu bằng 0.
- Docker init chạy chỉ khi volume trống. Thay `.env` hay file SQL sau lần chạy đầu không tự cập nhật DB đang tồn tại; hãy dùng migration và ALTER ROLE riêng. Không dùng `docker compose down -v` trên DB cần giữ.
- GRANT cấp quyền bảng cho tài khoản ứng dụng trong môi trường nghiên cứu nội bộ. Nếu dùng nhiều người dùng/độ mật khác nhau, cần bổ sung xác thực và RLS theo bản ghi/cột.
- Không thể thực thi Docker Desktop của máy Windows này từ môi trường hiện tại; cấu hình được kiểm tra tĩnh, cần chạy thực tế trên máy Windows của bạn.
