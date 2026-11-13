# Hợp nhất CSDL đơn vị quân đội Hà Nội (2026)

## 1. Hiện trạng (khảo sát 12/11/2026, chỉ đọc)

| Nguồn | Số bản ghi | Vấn đề |
|---|---|---|
| `HQTCSDL.ql_don_vi.don_vi_quan_doi` | 43 | **Cả 43 điểm trùng khớp `ST_PointOnSurface` của xã/phường**, tức là điểm đại diện, không phải trụ sở. 30 đơn vị chồng lên 9 tọa độ (7 đơn vị chung 1 điểm ở Ba Đình). |
| `HQTCSDL.public.danh_muc_bchqs_cap_xa_2026` | 126 | Bảng rời, không có khóa ngoại tới bảng đơn vị; 126/126 "chưa xác minh". |
| `hanoi_military_gis.public.don_vi_quan_doi` | 51 | Theo địa giới cũ (28 Ban CHQS quận/huyện); ~20 bản ghi trùng với 43 đơn vị trên nhưng khác mã; cột `x_vn2000` thực chất là UTM 48N. |
| Logo | 1 ảnh | Mọi đơn vị cùng một ký hiệu. |

Các cột `muc_do_xac_minh`, `trang_thai_quan_ly` là VARCHAR tự do, không có danh mục.

## 2. Thiết kế mới (một nguồn duy nhất: `HQTCSDL.ql_don_vi`)

```
cap_don_vi ─┐                         ┌─ dm_muc_do_xac_minh
loai_don_vi ┼─► don_vi_quan_doi ◄─────┼─ dm_trang_thai_quan_ly
  └ bieu_tuong_ma ─► bieu_tuong ◄─────┘   (bieu_tuong_ma ghi đè)
dm_xa_phuong ─►      │ 1
                     ▼ n
               vi_tri_don_vi ─► dm_phuong_phap_dinh_vi
               (lịch sử, 1 vị trí hiện hành / đơn vị)
```

### Nguyên tắc trắc địa

* **Lưu trữ**: `geometry(Point, 32648)` – WGS84/UTM 48N (mét), trùng CRS bản đồ của ứng dụng.
* **Tọa độ gốc** giữ nguyên như khi đo: `srid_goc` ∈ {4326, 4756, **5897**, 3405, 32648}, cột `e_goc` (Đông/kinh độ), `n_goc` (Bắc/vĩ độ).
* **VN-2000 Hà Nội**: kinh tuyến trục 105°00', múi 3°, k = 0,9999 → **EPSG:5897**. Quy ước Việt Nam: **X = Bắc, Y = Đông** – view xuất `x_vn2000`, `y_vn2000` theo đúng quy ước này.
* **Siêu dữ liệu vị trí** bắt buộc: phương pháp, độ chính xác (m), nguồn, ngày xác định.

### Ràng buộc được CSDL tự kiểm (trigger)

1. Kinh/vĩ độ hoặc E/N phải thuộc miền Hà Nội → phát hiện **nhập ngược trục**.
2. Điểm phải nằm trong ranh giới Hà Nội (dung sai 100 m).
3. Điểm phải nằm trong **xã/phường khai báo** (dung sai 50 m); sai thì báo xã/phường mà điểm thực sự rơi vào.
4. Không khai báo độ chính xác tốt hơn khả năng của phương pháp (vd. GNSS cầm tay ≥ 5 m).
5. Hồ sơ "đã kiểm chứng" bắt buộc có vị trí thật, độ chính xác ≤ 10 m.
6. Mỗi đơn vị **một** vị trí hiện hành; thêm vị trí mới thì vị trí cũ tự đóng và **không được sửa/xóa**.
7. `don_vi_quan_doi.geom` chỉ là bản sao đồng bộ; sửa trực tiếp sẽ bị chặn.

### Hiển thị bản đồ

* `v_ban_do_don_vi`: 173 đơn vị (43 + 126 BCHQS + 4 từ CSDL cũ), kèm logo theo loại (`tep_anh`, `kich_thuoc_px`, `thu_tu_ve`), độ chính xác, tọa độ 3 hệ.
* `geom_hien_thi`: đơn vị trùng điểm được xếp vòng tròn quanh điểm gốc để logo không che nhau – **chỉ để vẽ**, không dùng đo đạc.
* `v_kiem_tra_vi_tri`, `v_thong_ke_vi_tri`: danh sách cần định vị lại.

## 3. Thứ tự chạy (pgAdmin, tài khoản `postgres`, database `HQTCSDL`)

| File | Việc |
|---|---|
| `10_DanhMucChuanHoa.sql` | Danh mục, logo, cấp/loại mới, ràng buộc |
| `20_ViTriDonVi.sql` | Bảng vị trí + trigger + chuyển 43 điểm hiện có |
| `30_HopNhatDonVi.sql` | Thêm 126 BCHQS + 4 đơn vị cũ, tự kiểm số lượng |
| `40_ViewBanDoVaKiemTra.sql` | View bản đồ / kiểm tra / thống kê |
| `50_PhanQuyen.sql` | Quyền SELECT cho `managergis_app` |
| `60_LopTuongThichUngDung.sql` | View `public.don_vi_quan_doi` + trigger INSTEAD OF (giai đoạn 3) |
| `61_ChuyenHoSoNghiepVuCu.sql` | Chuyển hồ sơ nghiệp vụ từ CSDL cũ (đọc CSV qua stdin) |
| `70_` / `71_KichThuocKyHieu*.sql` | Kích thước ký hiệu bản đồ (v2 / v3 = 18 px) |
| `80_DinhViTruSo.sql` | Hàm `fn_quy_doi_toa_do`, `fn_dinh_vi_tru_so` cho chức năng "Định vị lại trụ sở" |
| `81_BanDoChuyenDe.sql` | Hàm `fn_ban_do_chuyen_de` (mật độ, xác minh, khoảng cách KNN tới cơ sở quân y, Voronoi) + view `v_co_so_quan_y` — ứng dụng đã bỏ chức năng này (13/11/2026), hàm để lại không dùng |
| `82_DinhViTheoOSM.sql` | Định vị lại 26 trụ sở theo khuôn viên OpenStreetMap + địa chỉ công bố (`ST_PointOnSurface`, sai số = bán kính tương đương khuôn viên, ≥ 50 m). Giữ lịch sử vị trí cũ; chạy lại an toàn |
| `83_BoSungQuanBinhChung.sql` | Bổ sung Quân/Binh chủng cho 25 đơn vị (chỉ ô đang trống) |
| `84_HieuNangVaKiemThu.sql` | Chỉ mục GiST/B-tree, MV `ql_he_thong.mv_thong_ke_loai_xa`, `fn_do_hieu_nang` (EXPLAIN có/không chỉ mục), bộ kiểm thử `ql_kiem_thu.fn_chay_kiem_thu()` (tự hoàn tác) |
| `85_PhanQuyenRlsVaDuyet.sql` | Vai trò `gis_quan_tri` / `gis_bien_tap` / `gis_xem`, RLS theo độ mật, hàng đợi duyệt `ql_he_thong.yeu_cau_thay_doi` + `fn_duyet_yeu_cau` / `fn_tu_choi_yeu_cau` |
| `86_KhuonVienVaToaDo.sql` | View `v_khuon_vien_don_vi`, `fn_luu_khuon_vien` / `fn_xoa_khuon_vien` (kiểm tra hình học, ≤ 2 km từ trụ sở), `fn_kiem_tra_nhap_toa_do`, `fn_lich_su_vi_tri` |
| `87_DinhTuyenPgRouting.sql` | Dựng mạng đường `ql_ban_do_nen.canh` / `dinh` từ OSM (cần nạp `duong_osm` trước), `fn_dinh_tuyen`, `fn_den_quan_y_gan_nhat`, `fn_vung_tiep_can`. Chạy lại = dựng lại mạng (~2 phút) |
| `88_DiaHinhDem.sql` | `fn_do_cao`, `fn_mat_cat` trên raster `ql_ban_do_nen.dem_srtm30` (nạp bằng raster2pgsql trước) |
| `90_KiemTraSauKhiChay.sql` | Đối chiếu (chỉ đọc) |
| `99_HoanTac.sql` | Hoàn tác toàn bộ |

Mỗi file là một giao dịch; lỗi ở đâu thì file đó tự hủy, không để dữ liệu dở dang.
Ứng dụng hiện tại vẫn chạy như cũ (view `v_don_vi_ban_do` giữ nguyên cột).

### Nạp dữ liệu nền cho 87 / 88 (một lần)

Tệp nguồn ở `MapWinGIS_TOI\DuLieuNen\`. Mật khẩu **không** ghi vào lệnh (dùng `pgpass` hoặc trust cục bộ).

```powershell
# Mạng đường OSM (GeoJSON 4326) -> ql_ban_do_nen.duong_osm, rồi chạy 87_
$env:PROJ_LIB = "C:\Program Files\QGIS 3.44.6\share\proj"
ogr2ogr -f PostgreSQL "PG:host=127.0.0.1 port=5433 dbname=HQTCSDL user=postgres" duong_osm_hanoi.geojson -nln ql_ban_do_nen.duong_osm -t_srs EPSG:32648 -lco GEOMETRY_NAME=geom -lco FID=gid -overwrite
# (bảng đích phải là LineString 32648 có cột highway/name/oneway như script 87 đọc)
# DEM SRTM 30 m (đã chiếu 32648) -> ql_ban_do_nen.dem_srtm30, rồi chạy 88_
raster2pgsql -s 32648 -t 128x128 -I -C -M -N -32768 HANOI_DEM_SRTM30_32648.tif ql_ban_do_nen.dem_srtm30 | psql -h 127.0.0.1 -p 5433 -U postgres -d HQTCSDL
```

### Chức năng ứng dụng dùng các script trên (13/11/2026)

| Chức năng | Vị trí trong ứng dụng | CSDL |
|---|---|---|
| Nhập tọa độ GNSS / CSV / KML / GPX hàng loạt | Bản đồ → ⚒ → Trắc địa; Quản lý dữ liệu → ⚒ GIS | `fn_kiem_tra_nhap_toa_do`, `fn_dinh_vi_tru_so` |
| Lịch sử vị trí | ⚒ hoặc thẻ đơn vị → "Lịch sử vị trí" | `fn_lich_su_vi_tri` |
| Lưới tọa độ UTM km | ⚒ → Lưới tọa độ UTM 48N | (vẽ phía ứng dụng) |
| Số hóa khuôn viên | thẻ đơn vị → "Khuôn viên"; chuột phải = hoàn tất | `fn_luu_khuon_vien` |
| Tìm đường / quân y gần nhất / vùng 5-10-15 phút | ⚒ → Mạng đường bộ | script 87 |
| Mặt cắt & tầm nhìn thông | ⚒ → Địa hình | script 88 |
| Đổi vai trò, duyệt, nhật ký, sao lưu, kiểm thử, hiệu năng | menu "Quản trị hệ thống" | script 84, 85 |
| In bản đồ A4/A3, phiếu hồ sơ, xuất GeoJSON/KML/SHP/CSV | ⚒ → Xuất bản; thẻ đơn vị → "Phiếu hồ sơ" | VN-2000 qua `ST_Transform(…, 5897)` |

## 4. Định vị chính xác – quy trình

CSDL **không tự biết** vị trí thật; nó bảo đảm vị trí nhập vào nhất quán và
ghi rõ độ tin cậy. Vị trí thật cần lấy từ nguồn có thẩm quyền:

1. Cơ sở có địa chỉ công bố (học viện, bệnh viện, cơ quan): số hóa tại **cổng
   chính** trên ảnh vệ tinh đã nắn (`SO_HOA_ANH`, ~3 m) hoặc đo GNSS.
2. Ban CHQS xã/phường: số hóa trụ sở UBND/Ban CHQS theo danh mục địa chỉ của
   thành phố.
3. Đơn vị không công bố địa chỉ: chỉ nhập khi có tài liệu được phép sử dụng;
   nếu không, giữ điểm đại diện – bản đồ sẽ ghi rõ "điểm đại diện".

## 5. Giai đoạn 2 (sửa ứng dụng)

* Đọc một nguồn `v_ban_do_don_vi`, bỏ nguồn 51 điểm cũ (hết đếm trùng 220).
* Vẽ logo theo `tep_anh` từ `Resources/Logos/`, thiếu ảnh thì dùng `mac_dinh.png`.
* Điểm đại diện vẽ mờ/viền nét đứt; tooltip hiện phương pháp + độ chính xác.
