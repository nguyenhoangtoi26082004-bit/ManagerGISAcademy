# 04. LỘ TRÌNH THỰC HIỆN BÀI TẬP LỚN C# WINFORMS + MAPWINGIS 100%
## Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)
### Đề tài: Xây Dựng CSDL GIS Tra Cứu & Quản Lý Đơn Vị Quân Đội Trên Địa Bàn Hà Nội

---

## 1. TỔNG QUAN TIẾN ĐỘ THỰC HIỆN ĐỀ TÀI (5 GIAI ĐOẠN)

Đề tài được chia thành **5 giai đoạn rõ ràng**, bám sát yêu cầu chấm điểm của môn học Hệ quản trị CSDL Không gian:

```
[GIAI ĐOẠN 1] THIẾT KẾ & KHỞI TẠO CSDL POSTGIS (Tuần 1 - 2)
  ├── Cài đặt PostgreSQL 15 + PostGIS 3.3
  ├── Tạo bảng don_vi_quan_doi với cột geom_32648 và geom_4326
  └── Đánh chỉ mục không gian GIST & Viết Stored Procedures
       │
       ▼
[GIAI ĐOẠN 2] CÀI ĐẶT MAPWINGIS & THIẾT KẾ FORM CHÍNH (Tuần 3)
  ├── Đăng ký MapWinGIS.ocx vào Visual Studio Toolbox
  ├── Thiết kế FormMain: axMap1, Toolbar, StatusStrip tọa độ X/Y
  └── Nạp lớp bản đồ ranh giới hành chính Hà Nội (Shapefile / Tile)
       │
       ▼
[GIAI ĐOẠN 3] LẬP TRÌNH TẦNG DAL NPGSQL & NẠP LỚP ĐIỂM (Tuần 4 - 5)
  ├── Cài đặt NuGet Npgsql, viết lớp DbConnection
  ├── Viết DonViQuanDoiDAL: SELECT và INSERT với ST_MakePoint
  └── Hiển thị danh sách điểm đơn vị lên axMap1 với icon quân sự
       │
       ▼
[GIAI ĐOẠN 4] LẬP TRÌNH CÁC CHỨC NĂNG PHÂN TÍCH KHÔNG GIAN (Tuần 6)
  ├── Chức năng 1: Phân tích Vùng đệm phòng thủ (ST_Buffer / ST_DWithin)
  ├── Chức năng 2: Tìm K đơn vị gần nhất (K-NN với toán tử <->)
  └── Chức năng 3: Tra cứu kết hợp thuộc tính & vị trí địa lý
       │
       ▼
[GIAI ĐOẠN 5] KIỂM TRA TOPO, VIẾT BÁO CÁO & BẢO VỆ BTL (Tuần 7 - 8)
  ├── Rà soát các điểm có tọa độ nằm ngoài ranh giới Hà Nội
  ├── Xuất file báo cáo thống kê lực lượng (PDF / Excel)
  └── Soạn thảo Báo cáo Word 40 trang & Thiết kế Slide thuyết trình
```

---

## 2. CHI TIẾT CÁC BƯỚC THỰC HIỆN TỪNG GIAI ĐOẠN

### GIAI ĐOẠN 1: THIẾT KẾ CSDL POSTGIS (Trọng tâm môn học)
- **Bước 1.1**: Cài đặt PostgreSQL và kích hoạt extension:
  ```sql
  CREATE EXTENSION postgis;
  CREATE EXTENSION postgis_topology;
  ```
- **Bước 1.2**: Chạy kịch bản SQL trong `database/hanoi_military_gis_postgis.sql` để tạo bảng `don_vi_quan_doi`.
- **Bước 1.3**: Kiểm tra chỉ mục không gian:
  ```sql
  EXPLAIN ANALYZE 
  SELECT * FROM don_vi_quan_doi 
  WHERE ST_DWithin(geom_32648, ST_SetSRID(ST_MakePoint(586000, 2326000), 32648), 5000);
  ```
  *(Đảm bảo PostgreSQL sử dụng `Bitmap Index Scan on idx_donvi_geom_32648` chứ không quét tuần tự)*.

---

### GIAI ĐOẠN 2: THIẾT KẾ GIAO DIỆN WINFORMS VỚI MAPWINGIS
*(Có thể tham khảo trực tiếp cách bố trí giao diện Dark Command Center từ bản mẫu)*
- **Bước 2.1**: Tạo Project mới: `C# Windows Forms App (.NET Framework 4.8 hoặc .NET 8)`.
- **Bước 2.2**: Kéo thả điều khiển `AxMapWinGIS.AxMap` vào trung tâm `FormMain`, đặt `Dock = Fill`.
- **Bước 2.3**: Thiết kế thanh công cụ bên trái (Toolstrip/Panel):
  - Phóng to (`ZoomIn`), Thu nhỏ (`ZoomOut`), Toàn cảnh (`ZoomToMaxExtents`).
  - Thước đo khoảng cách tác chiến (`CursorMode = cmMeasure`).
  - Bật/tắt chế độ chọn đối tượng (`cmIdentify`).
- **Bước 2.4**: Thiết kế thanh trạng thái `StatusStrip` ở đáy Form:
  - Bắt sự kiện `axMap1_MouseMove` để cập nhật liên tục:
    `"Tọa độ chuột: X = ... m, Y = ... m | Lat: ... , Lon: ... | Hệ quy chiếu: VN-2000 (EPSG:32648)"`.

---

### GIAI ĐOẠN 3: KẾT NỐI DATABASE VÀ NẠP DỮ LIỆU ĐIỂM
- **Bước 3.1**: Cài đặt thư viện Npgsql qua NuGet Package Manager.
- **Bước 3.2**: Xây dựng lớp `DbConnection.cs` đọc chuỗi kết nối từ `App.config`.
- **Bước 3.3**: Lập trình đọc dữ liệu từ bảng `don_vi_quan_doi` và chuyển thành `MapWinGIS.Shapefile`:
  - Mỗi bản ghi chuyển thành một `Shape` kiểu `SHP_POINT`.
  - Thiết lập thuộc tính `Shapefile.Labels` để hiển thị tên viết tắt của đơn vị lên bản đồ.
- **Bước 3.4**: Bắt sự kiện click chuột vào một điểm trên bản đồ để mở Form xem chi tiết hồ sơ đơn vị tác chiến.

---

### GIAI ĐOẠN 4: LẬP TRÌNH CÁC PHÉP TOÁN KHÔNG GIAN POSTGIS
- **Bước 4.1: Chức năng Phân tích Vùng đệm (Buffer Analysis)**:
  - Thiết kế Form con `FormPhanTichVungDem.cs`.
  - Cho phép người dùng chọn đơn vị nguồn và nhập bán kính (ví dụ 5.000m, 10.000m).
  - Gửi truy vấn xuống PostGIS sử dụng `ST_Buffer` và `ST_DWithin`.
  - Nhận kết quả chuỗi `WKT POLYGON` từ CSDL, dùng `MapWinGIS.Shape.ImportFromWKT()` để vẽ đa giác tròn bán kính lên bản đồ `axMap1`.
  - Hiển thị danh sách các đơn vị khác nằm trong tầm bắn/hỏa lực lên `DataGridView`.
- **Bước 4.2: Chức năng Tìm K đơn vị gần nhất (K-NN)**:
  - Bấm điểm bất kỳ trên bản đồ $\rightarrow$ Gọi hàm `sp_tim_k_don_vi_gan_nhat`.
  - PostGIS dùng toán tử `<->` trả về Top $K$ đơn vị.
  - Vẽ đường thẳng nối từ tâm điểm đến các đơn vị và hiển thị cự ly (mét).

---

### GIAI ĐOẠN 5: KIỂM TRA DỮ LIỆU TOPO & BẢO VỆ ĐỀ TÀI
- **Bước 5.1: Kiểm tra lỗi không gian (Data & Topology Audit)**:
  - Viết câu truy vấn tìm các đơn vị bị gõ nhầm tọa độ nằm ngoài ranh giới Hà Nội:
    ```sql
    SELECT * FROM don_vi_quan_doi 
    WHERE vi_do NOT BETWEEN 20.55 AND 21.40 
       OR kinh_do NOT BETWEEN 105.25 AND 106.10;
    ```
  - Cảnh báo các đơn vị bị trùng mã quân sự hoặc trùng tọa độ đóng quân.
- **Bước 5.2: Xuất báo cáo thống kê**:
  - Xuất danh sách đơn vị ra file Excel (`.xlsx`) hoặc in ra bản báo cáo kỹ thuật.
- **Bước 5.3: Hoàn thiện báo cáo BTL**:
  - Báo cáo thuyết minh Word: Đầy đủ sơ đồ ERD, kiến trúc 3 lớp C#, giải thích các câu lệnh PostGIS, hình ảnh minh họa chạy phần mềm WinForms.
  - Slide báo cáo thuyết trình (PowerPoint 15-20 slide).

---

## 3. TIÊU CHÍ ĐÁNH GIÁ ĐIỂM CAO MÔN HỆ QUẢN TRỊ CSDL KHÔNG GIAN

| Tiêu chí đánh giá | Trọng số | Yêu cầu kỹ thuật cần đạt |
| :--- | :---: | :--- |
| **1. Thiết kế CSDL PostGIS chuẩn** | 30% | Bảng có cột hình học `geometry`, xác định đúng SRID (32648 & 4326), đánh chỉ mục GIST, có trigger tự động tính toán. |
| **2. Tương tác bản đồ MapWinGIS** | 25% | Hiển thị bản đồ nền, nạp lớp điểm đơn vị từ CSDL, bắt tọa độ chuột thời gian thực, zoom tới điểm khi click trên DataGrid. |
| **3. Phép toán không gian (Spatial Queries)** | 25% | Ứng dụng thành công `ST_DWithin`, `ST_Buffer`, `ST_Distance` và toán tử `<->`. Tính toán cự ly thực tế chính xác. |
| **4. Kiến trúc mã nguồn C# WinForms** | 10% | Phân tách 3 lớp rõ ràng (GUI - BLL - DAL), kết nối Npgsql an toàn, không viết SQL trực tiếp trên Form. |
| **5. Báo cáo thuyết minh & Thuyết trình** | 10% | Báo cáo Word trình bày khoa học, Slide súc tích, demo phần mềm trôi chảy, trả lời đúng câu hỏi vấn đáp. |
