# Hệ thống quản lý đơn vị quân đội trên địa bàn Hà Nội (ManagerGISAcademy)

Bài tập lớn môn **Hệ quản trị CSDL không gian**: ứng dụng WinForms (.NET Framework 4.8 + MapWinGIS)
trên CSDL **PostgreSQL / PostGIS / pgRouting**.

## Chức năng chính

| Nhóm | Chức năng |
|---|---|
| Bản đồ GIS | 173 đơn vị (47 đơn vị + 126 Ban CHQS xã/phường), ký hiệu theo loại, nền OSM / ảnh vệ tinh, đo đạc, lưới tọa độ UTM km |
| Trắc địa | Định vị lại trụ sở, nhập tọa độ GNSS/CSV/KML/GPX hàng loạt (WGS84, VN‑2000, UTM), lịch sử vị trí, số hóa khuôn viên |
| Phân tích | Tìm đường, cơ sở quân y gần nhất, vùng tiếp cận 5/10/15 phút (pgRouting), mặt cắt địa hình và tầm nhìn (DEM SRTM 30 m) |
| Báo cáo | Thống kê và báo cáo PDF, in bản đồ A4/A3, phiếu hồ sơ đơn vị, xuất GeoJSON/KML/Shapefile/CSV |
| Kiểm tra dữ liệu | Lỗi vị trí, thiếu thông tin, chất lượng tọa độ (vị trí thật / điểm đại diện) |
| Quản trị | Đăng nhập / đăng ký, phân quyền theo vai trò + RLS theo độ mật, duyệt thay đổi, nhật ký, sao lưu/khôi phục, kiểm thử CSDL, đo hiệu năng |

Vai trò: **Quản trị** (toàn quyền, duyệt, phân quyền) · **Biên tập** (thêm/sửa → chờ duyệt) · **Chỉ xem** · **Khách** (chưa đăng nhập = chỉ xem).

## 1. Cài phần mềm

| Phần mềm | Ghi chú |
|---|---|
| Windows 10/11 | |
| Visual Studio 2022 | workload **.NET desktop development** (.NET Framework 4.8) |
| PostgreSQL 15 trở lên | Khi cài xong, mở **Stack Builder** → *Spatial Extensions* → **PostGIS 3.4+ Bundle** (đã kèm pgRouting và postgis_raster) |
| MapWinGIS **5.3 bản Win32 (x86)** | Tải từ trang Releases của dự án MapWinGIS trên GitHub, cài bằng quyền Administrator. Ứng dụng chạy 32‑bit nên phải dùng bản x86 |

## 2. Tạo CSDL

Mở **PowerShell** trong thư mục `DuLieu\CSDL` (thay `5432` nếu PostgreSQL của bạn dùng cổng khác; thêm thư mục `bin` của PostgreSQL vào PATH hoặc gõ đường dẫn đầy đủ):

```powershell
# 2.1 Tao tai khoan ung dung + 3 vai tro (tu dat mat khau cua ban, >= 8 ky tu)
psql -U postgres -h 127.0.0.1 -p 5432 -v mat_khau_app="MAT_KHAU_CUA_BAN" -f 00_TaoVaiTro.sql

# 2.2 Tao CSDL va khoi phuc du lieu (khoang 10 giay)
createdb -U postgres -h 127.0.0.1 -p 5432 HQTCSDL
pg_restore -U postgres -h 127.0.0.1 -p 5432 -d HQTCSDL HQTCSDL.backup

# 2.3 Dung mang duong cho chuc nang tim duong (khoang 2-3 phut)
psql -U postgres -h 127.0.0.1 -p 5432 -d HQTCSDL -f ..\..\ManagerGISAcademy\Database\HopNhat_2026\87_DinhTuyenPgRouting.sql
```

Kiểm tra nhanh: `psql -U postgres -p 5432 -d HQTCSDL -c "select count(*) from ql_kiem_thu.fn_chay_kiem_thu() where dat"` phải ra **17**.

## 3. Cấu hình kết nối (không ghi mật khẩu vào mã nguồn)

Ứng dụng đọc chuỗi kết nối từ **biến môi trường** `MANAGERGISACADEMY_POSTGIS`, chạy một lần:

```powershell
[Environment]::SetEnvironmentVariable('MANAGERGISACADEMY_POSTGIS', 'Host=127.0.0.1;Port=5432;Database=HQTCSDL;Username=managergis_app;Password=MAT_KHAU_CUA_BAN;Timeout=15;Command Timeout=30;', 'User')
```

Sau đó **đóng và mở lại** Visual Studio.

## 4. Build và chạy

1. Mở `ManagerGISAcademy\HanoiMilitaryGIS.sln` bằng Visual Studio 2022.
2. Chuột phải Solution → **Restore NuGet Packages** (Npgsql 4.1…).
3. Chọn **Debug**, nhấn **F5**.

Lần đầu chạy: bấm **ĐĂNG NHẬP** ở góc phải → thẻ **Đăng ký tài khoản**. Tài khoản đăng ký **đầu tiên** sẽ là **Quản trị**.
Những người đăng ký sau ở trạng thái *chờ duyệt*. Quản trị viên duyệt và cấp vai trò tại **Quản trị hệ thống → Tài khoản & phân quyền**.

## 5. Ảnh nền

* `DuLieu\Basemaps\HANOI_SATELLITE_32648.tif` (ảnh vệ tinh nhẹ, 13 MB) được ứng dụng tự tìm thấy.
* Bản 2 m (`HANOI_SATELLITE_2M_TILED_32648.tif`, ~275 MB) vượt giới hạn 100 MB của GitHub nên **không có trong repo**. Nếu có, đặt vào thư mục trên hoặc trỏ biến môi trường `MANAGERGISACADEMY_BASEMAPS` tới thư mục chứa nó.
* Không có ảnh nền thì ứng dụng dùng OpenStreetMap trực tuyến.

## 6. Cấu trúc thư mục

```
ManagerGISAcademy/                 Mã nguồn WinForms (GUI / BLL / DAL / DTO)
  Database/HopNhat_2026/           Script SQL theo giai đoạn (10 → 89) + README_HOP_NHAT.md
DuLieu/CSDL/00_TaoVaiTro.sql       Tạo vai trò trước khi khôi phục
DuLieu/CSDL/HQTCSDL.backup         Bản sao CSDL (pg_dump 15, định dạng custom)
DuLieu/Basemaps/                   Ảnh nền ngoại tuyến (EPSG:32648)
ManagerGISAcademy_Docker_PostGIS/  Cấu hình Docker cũ (giai đoạn 1); tự tạo tệp .env từ .env.example
He_thong_CSDL_GIS_Quan_doi_Ha_Noi/ Bản mẫu giao diện web + tài liệu thiết kế ban đầu
```

## 7. Hệ tọa độ

* Lưu trữ và hiển thị: **WGS 84 / UTM múi 48N (EPSG:32648)**.
* VN‑2000 Hà Nội: kinh tuyến trục 105°00′, múi 3°, k₀ = 0,9999 (**EPSG:5897**, X = Bắc, Y = Đông).

## 8. Lưu ý về dữ liệu

* Vị trí trụ sở lấy từ OpenStreetMap (ODbL) và địa chỉ công bố. 30/173 đơn vị có vị trí trụ sở thật; số còn lại là **điểm đại diện địa bàn**, được ghi rõ trong ứng dụng.
* Dữ liệu chỉ phục vụ học tập, không dùng cho mục đích tác nghiệp.
* Mật khẩu người dùng được băm **bcrypt** (pgcrypto) trong CSDL. Không commit mật khẩu, tệp `.env` hay chuỗi kết nối thật.
