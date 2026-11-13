# 03. CẤU TRÚC XỬ LÝ DỮ LIỆU & GIS TRONG C# WINFORMS (BLL & DAL)
## Phân Tích Luồng Xử Lý Yêu Cầu Người Dùng & Nạp Dữ Liệu Lên MapWinGIS
### Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)

---

## 1. MÔ HÌNH XỬ LÝ YÊU CẦU TRONG ỨNG DỤNG WINFORMS DESKTOP
Trong mô hình ứng dụng WinForms kết hợp MapWinGIS và PostGIS, tầng "Backend" chính là sự kết hợp giữa **Tầng xử lý nghiệp vụ (BLL - Business Logic Layer)** và **Tầng truy xuất CSDL không gian (DAL - Data Access Layer)**:

```
[ NGƯỜI DÙNG TƯƠNG TÁC ]
  - Click chuột trên bản đồ axMap1
  - Chọn bán kính vùng đệm Buffer
  - Nhập từ khóa tìm kiếm trên FormTraCuu
  - Bấm nút "+ Thêm đơn vị"
           │
           ▼
┌────────────────────────────────────────────────────────┐
│     TẦNG GIAO DIỆN (GUI - WINDOWS FORMS)               │
│  - FormMain, FormTraCuu, FormThemSua, axMap1 Control   │
│  - Lắng nghe sự kiện: MouseDown, MouseMove, ButtonClick│
└──────────────────────────┬─────────────────────────────┘
                           │ Gửi DTO / Tham số
                           ▼
┌────────────────────────────────────────────────────────┐
│     TẦNG NGHIỆP VỤ (BLL / BUS - BUSINESS LOGIC LAYER)  │
│  - DonViQuanDoiBLL: Kiểm tra tính hợp lệ dữ liệu       │
│  - SpatialAnalysisBLL: Điều phối thuật toán GIS        │
│  - CoordinateConverter: Chuyển đổi WGS84 <=> VN-2000   │
└──────────────────────────┬─────────────────────────────┘
                           │ Gọi hàm truy xuất dữ liệu
                           ▼
┌────────────────────────────────────────────────────────┐
│     TẦNG TRUY XUẤT DỮ LIỆU (DAL / DAO LAYER)           │
│  - DbConnection (Npgsql Connection Pool)               │
│  - Thực thi Stored Procedure & SQL Không gian          │
└──────────────────────────┬─────────────────────────────┘
                           │ SQL ST_DWithin / ST_Buffer
                           ▼
┌────────────────────────────────────────────────────────┐
│     HỆ CSDL KHÔNG GIAN POSTGRESQL + POSTGIS (PORT 5432)│
│  - Bảng don_vi_quan_doi, quan_huyen_hanoi              │
│  - Tận dụng GIST Spatial Index trả về WKT / Coordinates│
└──────────────────────────┬─────────────────────────────┘
                           │ Trả về kết quả
                           ▼
┌────────────────────────────────────────────────────────┐
│     CÔNG CỤ KẾT XUẤT BẢN ĐỒ (MAPWINGIS ENGINE)         │
│  - Tạo đối tượng MapWinGIS.Shape (Point / Polygon)     │
│  - Nạp vào axMap1.AddLayer() & Vẽ vòng tròn Buffer     │
│  - Highlight điểm và ZoomToShape()                     │
└────────────────────────────────────────────────────────┘
```

---

## 2. QUY TRÌNH XỬ LÝ 5 YÊU CẦU NGƯỜI DÙNG CỐT LÕI

### YÊU CẦU 1: Khởi động FormMain & Nạp toàn bộ lớp điểm đơn vị lên MapWinGIS
1. **Sự kiện**: `FormMain_Load`.
2. **Xử lý BLL & DAL**:
   - `DonViQuanDoiBLL.LayTatCaDonVi()` gọi `DonViQuanDoiDAL.LayTatCaDonVi()`.
   - DAL thực thi query `SELECT *, ST_AsText(geom_4326) as wkt FROM don_vi_quan_doi;`.
3. **Hiển thị MapWinGIS**:
   - Tạo một `MapWinGIS.Shapefile` kiểu điểm (`SHP_POINT`).
   - Duyệt từng đơn vị, tạo `MapWinGIS.Shape`, gán tọa độ `(KinhDo, ViDo)`.
   - Nạp vào `axMap1.AddLayer(sf, true)`.
   - Thiết lập Icon quân hiệu cho từng đơn vị theo Quân binh chủng.

#### Code mẫu triển khai nạp Shapefile điểm từ PostGIS lên MapWinGIS:
```csharp
// GUI/FormMain.cs
private int _layerDonViHandle = -1;

private void LoadLopDonViLenMap()
{
    var bll = new DonViQuanDoiBLL();
    List<DonViQuanDoi> list = bll.LayTatCaDonVi();

    // 1. Tạo Shapefile bộ nhớ kiểu POINT (WGS-84)
    var sf = new MapWinGIS.Shapefile();
    sf.CreateNewWithShapeID("", ShpfileType.SHP_POINT);
    sf.GeoProjection.SetWgs84();

    // 2. Thêm các trường thuộc tính vào Shapefile
    sf.EditAddField("MaDV", FieldType.STRING_FIELD, 0, 50);
    sf.EditAddField("TenDV", FieldType.STRING_FIELD, 0, 150);
    sf.EditAddField("BinhChung", FieldType.STRING_FIELD, 0, 50);

    // 3. Đọc danh sách từ CSDL PostGIS và thêm các Shape điểm
    for (int i = 0; i < list.Count; i++)
    {
        var dv = list[i];
        var shape = new MapWinGIS.Shape();
        shape.Create(ShpfileType.SHP_POINT);

        var pt = new MapWinGIS.Point();
        pt.x = dv.KinhDo; // Longitude
        pt.y = dv.ViDo;   // Latitude
        shape.AddPoint(pt);

        int shapeIndex = sf.EditAddShape(shape);
        sf.EditCellValue(0, shapeIndex, dv.MaDonVi);
        sf.EditCellValue(1, shapeIndex, dv.TenDonVi);
        sf.EditCellValue(2, shapeIndex, dv.QuanBinhChung);
    }

    // 4. Thiết lập kiểu hiển thị Marker quân sự
    sf.DefaultDrawingOptions.PointType = tkPointSymbolType.ptSymbolStandard;
    sf.DefaultDrawingOptions.PointSize = 12;
    sf.DefaultDrawingOptions.FillColor = 255; // Red color

    // 5. Thêm Layer vào bản đồ axMap1
    if (_layerDonViHandle != -1) axMap1.RemoveLayer(_layerDonViHandle);
    _layerDonViHandle = axMap1.AddLayer(sf, true);
    axMap1.Redraw();
}
```

---

### YÊU CẦU 2: Tra cứu đa tiêu chí & Nhấp đúp vào dòng để định vị trên bản đồ
1. **Sự kiện**: Người dùng nhập tên đơn vị, chọn Binh chủng và trạng thái SSCĐ trên `FormTraCuu`.
2. **Xử lý BLL & DAL**:
   - `DonViQuanDoiBLL.TraCuu(keyword, binhChung, quanHuyen, sscd)`.
   - DAL sinh câu truy vấn SQL động với mệnh đề `WHERE` tối ưu qua chỉ mục B-tree.
3. **Hiển thị**:
   - Đổ dữ liệu vào `DataGridView.DataSource`.
   - Khi người dùng click chọn 1 dòng $\rightarrow$ Lấy kinh/vĩ độ của đơn vị.
   - Gọi `axMap1.ZoomTo(lon, lat, 14)` để bản đồ tự động bay tới đơn vị và mở popup thông tin.

---

### YÊU CẦU 3: Phân tích Vùng đệm phòng thủ (Buffer Analysis)
1. **Sự kiện**: Người dùng chọn một đơn vị (ví dụ `HV-KTQS`) và thiết lập bán kính $R = 10.000$ mét.
2. **Xử lý BLL & DAL**:
   - BLL gọi `SpatialAnalysisDAL.PhanTichVungDem("HV-KTQS", 10000)`.
   - DAL thực thi hàm PostGIS:
     ```sql
     -- Tạo hình học đa giác đường tròn bán kính 10.000m trong hệ UTM 48N
     SELECT ST_AsText(ST_Transform(ST_Buffer(geom_32648, 10000), 4326)) AS buffer_wkt
     FROM don_vi_quan_doi WHERE ma_don_vi = 'HV-KTQS';
     ```
3. **Hiển thị MapWinGIS**:
   - Nhận chuỗi `WKT POLYGON` từ CSDL PostGIS.
   - Dùng phương thức `shape.ImportFromWKT(wkt)` tạo đối tượng Polygon.
   - Đưa vào một Shapefile lớp đa giác (`SHP_POLYGON`) với độ trong suốt 30%, viền màu vàng cam.
   - Liệt kê các đơn vị khác nằm trong vùng đệm lên bảng kết quả kèm thời gian cơ động chi viện.

---

### YÊU CẦU 4: Bắt điểm trực tiếp trên bản đồ để Thêm Mới đơn vị
1. **Sự kiện**: Bật công cụ "Chấm tọa độ" $\rightarrow$ Click chuột trên `axMap1`.
2. **Xử lý trắc địa**:
   - Sự kiện `axMap1_MouseDown(sender, e)`:
   - Dùng `axMap1.PixelToProj(e.x, e.y, ref projX, ref projY)` lấy được Kinh độ/Vĩ độ WGS-84.
   - `CoordinateConverter.Wgs84ToUtm48N(lat, lon)` tự động tính ra $X, Y$ mét (VN-2000).
   - Truyền tọa độ này vào `FormThemSuaDonVi`.
3. **Lưu dữ liệu**:
   - Khi bấm "Lưu", DAL thực thi lệnh `INSERT` với `ST_SetSRID(ST_MakePoint(kinhdo, vido), 4326)`.
   - Nạp lại lớp bản đồ `LoadLopDonViLenMap()` để điểm mới xuất hiện ngay lập tức.

---

### YÊU CẦU 5: Tìm kiếm K đơn vị gần nhất (K-NN Query)
1. **Sự kiện**: Nhấp chuột phải vào 1 điểm trên bản đồ $\rightarrow$ Chọn "Tìm 5 đơn vị chi viện gần nhất".
2. **Xử lý DAL**:
   - Gửi tọa độ `(lon, lat)` vào stored procedure `sp_tim_k_don_vi_gan_nhat`.
   - PostGIS sử dụng toán tử `<->` trên chỉ mục GIST trả về Top 5 đơn vị nhanh nhất.
3. **Hiển thị**:
   - Vẽ đường nét đứt (Polyline) nối từ tâm điểm tới 5 đơn vị.
   - Bảng kết quả hiển thị cự ly trắc địa chính xác tính theo km và mét.

---

## 3. TỐI ƯU HÓA BỘ NHỚ KHI LÀM VIỆC VỚI MAPWINGIS
- Luôn gọi `axMap1.LockWindow(tkLockMode.lmLock)` trước khi nạp hàng loạt điểm và `axMap1.LockWindow(tkLockMode.lmUnlock)` sau khi nạp xong để tránh bản đồ bị giật / chớp màn hình.
- Dùng `sf.FastMode = true` đối với các Shapefile có nhiều đối tượng.
- Đóng kết nối `NpgsqlConnection` ngay sau khi đọc dữ liệu xong bằng khối lệnh `using (...)`.
