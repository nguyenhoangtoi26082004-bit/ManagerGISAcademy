# 07. HƯỚNG DẪN NẠP TOÀN BỘ DỮ LIỆU ĐƠN VỊ VÀO C# WINFORMS MAPWINGIS
## Đề tài: Hệ thống CSDL GIS Quản lý & Tra cứu Đơn vị Quân đội Hà Nội
### Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)

---

## 1. TỔNG QUAN BỘ DỮ LIỆU ĐÃ XUẤT BẢN
Toàn bộ dữ liệu của **50 đơn vị quân sự thực tế** trên địa bàn 30 quận/huyện/thị xã thành phố Hà Nội đã được xuất bản và lưu trữ sẵn trong thư mục **`database/`** với 4 định dạng chuyên dụng:

```
database/
├── DonViQuanDoiSeedData.cs        # [DÙNG NGAY TRONG C#] Chứa List<DonViQuanDoi> 50 đơn vị có sẵn tọa độ
├── hanoi_military_gis_postgis.sql # [DÀNH CHO POSTGIS] Kịch bản SQL DDL + 50 câu lệnh INSERT không gian
├── hanoi_military_units.geojson   # [DÀNH CHO MAPWINGIS / QGIS] Chuẩn FeatureCollection WGS-84 RFC 7946
└── hanoi_military_units.csv       # [DÀNH CHO EXCEL / PGDMIN] Bảng số liệu chi tiết tọa độ VN2000 & WGS84
```

---

## 2. BA CÁCH NẠP DỮ LIỆU VÀO DỰ ÁN C# WINFORMS + MAPWINGIS

### CÁCH 1: Dùng trực tiếp tệp C# `DonViQuanDoiSeedData.cs` (Nhanh nhất - Chạy Offline không cần cài PostgreSQL)
Nếu bạn đang trong giai đoạn thiết kế giao diện WinForms và muốn bản đồ `axMap1` hiển thị ngay 50 điểm đơn vị quân đội mà chưa kịp cấu hình PostgreSQL:

1. Copy tệp `database/DonViQuanDoiSeedData.cs` vào thư mục `DTO/` trong Visual Studio Project của bạn.
2. Tại sự kiện `FormMain_Load` hoặc sự kiện bấm nút "Nạp bản đồ", gọi hàm sau:

```csharp
// GUI/FormMain.cs
using MapWinGIS;
using HanoiMilitaryGIS.DTO;
using System.Collections.Generic;

private int _layerDonViHandle = -1;

private void NapDuLieuTuSeedDataLenMapWinGIS()
{
    // 1. Lấy danh sách 50 đơn vị quân đội có sẵn tọa độ từ SeedData
    List<DonViQuanDoi> danhSach = DonViQuanDoiSeedData.LayDanhSachDonViMau();

    // 2. Tạo một Shapefile điểm trong bộ nhớ (Memory Shapefile)
    var sf = new MapWinGIS.Shapefile();
    sf.CreateNewWithShapeID("", ShpfileType.SHP_POINT);
    sf.GeoProjection.SetWgs84(); // Thiết lập chuẩn WGS-84

    // 3. Khai báo các cột thuộc tính
    sf.EditAddField("MaDV", FieldType.STRING_FIELD, 0, 50);
    sf.EditAddField("TenDV", FieldType.STRING_FIELD, 0, 150);
    sf.EditAddField("BinhChung", FieldType.STRING_FIELD, 0, 80);
    sf.EditAddField("QuanHuyen", FieldType.STRING_FIELD, 0, 80);
    sf.EditAddField("SDT", FieldType.STRING_FIELD, 0, 50);
    sf.EditAddField("SSCD", FieldType.STRING_FIELD, 0, 50);

    // 4. Lặp qua 50 đơn vị và tạo các Shape Point đưa vào Shapefile
    for (int i = 0; i < danhSach.Count; i++)
    {
        var dv = danhSach[i];
        
        var shape = new MapWinGIS.Shape();
        shape.Create(ShpfileType.SHP_POINT);

        var pt = new MapWinGIS.Point();
        pt.x = dv.KinhDo; // Longitude WGS84
        pt.y = dv.ViDo;   // Latitude WGS84
        shape.AddPoint(pt);

        int shapeIdx = sf.EditAddShape(shape);
        sf.EditCellValue(0, shapeIdx, dv.MaDonVi);
        sf.EditCellValue(1, shapeIdx, dv.TenDonVi);
        sf.EditCellValue(2, shapeIdx, dv.QuanBinhChung);
        sf.EditCellValue(3, shapeIdx, dv.QuanHuyen);
        sf.EditCellValue(4, shapeIdx, dv.SoDienThoai);
        sf.EditCellValue(5, shapeIdx, dv.TrangThaiSSCD);
    }

    // 5. Cấu hình hiển thị Marker quân sự
    sf.DefaultDrawingOptions.PointType = tkPointSymbolType.ptSymbolStandard;
    sf.DefaultDrawingOptions.PointSize = 12;
    sf.DefaultDrawingOptions.FillColor = (uint)System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);
    sf.DefaultDrawingOptions.LineColor = (uint)System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gold);
    sf.DefaultDrawingOptions.LineWidth = 2;

    // 6. Hiển thị nhãn tên viết tắt (Labels)
    sf.Labels.Generate("[TenDV]", tkLabelPositioning.lpBottom, true);
    sf.Labels.FontSize = 9;
    sf.Labels.FontColor = (uint)System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
    sf.Labels.HaloColor = (uint)System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
    sf.Labels.HaloSize = 1;

    // 7. Thêm Layer vào điều khiển axMap1
    if (_layerDonViHandle != -1) axMap1.RemoveLayer(_layerDonViHandle);
    _layerDonViHandle = axMap1.AddLayer(sf, true);

    // Zoom vừa vặn toàn bộ 50 đơn vị trên bản đồ Hà Nội
    axMap1.ZoomToLayer(_layerDonViHandle);
    axMap1.Redraw();

    // Đổ danh sách vào DataGridView để tra cứu
    dgvDanhSachDonVi.DataSource = danhSach;
}
```

---

### CÁCH 2: Nạp vào CSDL PostgreSQL/PostGIS và Truy vấn qua Npgsql (Chuẩn điểm tối đa)
Đây là phương án chuẩn nhất theo yêu cầu môn học **Hệ quản trị CSDL Không gian**:

1. **Bước 1**: Mở **pgAdmin 4** $\rightarrow$ Tạo Database: `hanoi_military_gis`.
2. **Bước 2**: Mở cửa sổ **Query Tool** trong pgAdmin.
3. **Bước 3**: Mở tệp `database/hanoi_military_gis_postgis.sql` $\rightarrow$ Nhấn **F5** (Execute).
   - Tệp SQL đã có sẵn câu lệnh tạo bảng `don_vi_quan_doi`.
   - Đánh chỉ mục không gian `GIST(geom_32648)` và `GIST(geom_4326)`.
   - Nạp đầy đủ 50 câu lệnh `INSERT INTO don_vi_quan_doi ... VALUES (..., ST_SetSRID(ST_MakePoint(...), 4326), ST_SetSRID(ST_MakePoint(...), 32648));`.
4. **Bước 4**: Trong ứng dụng C# WinForms, sử dụng tầng `DAL/DonViQuanDoiDAL.cs` (đã hướng dẫn ở file `02_POSTGIS_DATABASE_SYNC.md`) để đọc dữ liệu qua Npgsql:
```csharp
var dal = new DonViQuanDoiDAL();
List<DonViQuanDoi> list = dal.LayTatCaDonVi();
// Sau đó nạp lên MapWinGIS tương tự như Cách 1
```

---

### CÁCH 3: Mở trực tiếp tệp GeoJSON trong MapWinGIS qua OGR
MapWinGIS tích hợp sẵn thư viện **GDAL / OGR**, cho phép bạn mở trực tiếp tệp `database/hanoi_military_units.geojson`:

```csharp
private void MoFileGeoJsonLenMapWinGIS(string duongDanGeoJson)
{
    var ogrLayer = new MapWinGIS.OgrLayer();
    if (ogrLayer.OpenFrom(duongDanGeoJson))
    {
        int handle = axMap1.AddLayer(ogrLayer, true);
        axMap1.ZoomToLayer(handle);
        axMap1.Redraw();
        MessageBox.Show("Đã nạp thành công tệp GeoJSON lên MapWinGIS!");
    }
}
```

---

## 3. DANH MỤC TRÍCH DẪN MỘT SỐ ĐƠN VỊ TIÊU BIỂU TRONG CSDL

| STT | Mã số | Tên Đơn vị | Binh chủng | Quận / Huyện | Vĩ độ (Lat) | Kinh độ (Lng) | Tọa độ X (VN-2000 m) | Tọa độ Y (VN-2000 m) | Số máy trực ban |
| :---: | :--- | :--- | :--- | :--- | :---: | :---: | :---: | :---: | :---: |
| 1 | `BQP-HQ` | Sở Chỉ huy Bộ Quốc phòng | Lục quân | Q. Ba Đình | 21.0335 | 105.8402 | 587,314 | 2,326,242 | 069.533.111 |
| 2 | `HV-KTQS` | Học viện Kỹ thuật Quân sự (MTA) | Hậu cần - Kỹ thuật | Q. Bắc Từ Liêm | 21.0481 | 105.7845 | 581,515 | 2,327,823 | 069.515.200 |
| 3 | `BTL-TDO` | Bộ Tư lệnh Thủ đô Hà Nội | Lục quân | Q. Hà Đông | 20.9712 | 105.7745 | 580,551 | 2,319,308 | 069.576.102 |
| 4 | `QC-PKKQ` | Quân chủng Phòng không - Không quân | PK-KQ | Q. Thanh Xuân | 20.9998 | 105.8278 | 586,056 | 2,322,504 | 069.565.113 |
| 5 | `SD-PK-361` | Sư đoàn Phòng không 361 | PK-KQ | Q. Cầu Giấy | 21.0215 | 105.7878 | 581,883 | 2,324,880 | 024.3784.5512 |
| 6 | `SD-BB-301` | Sư đoàn Bộ binh 301 | Lục quân | H. Thanh Trì | 20.9465 | 105.8315 | 586,498 | 2,316,601 | 024.3861.8824 |
| 7 | `BTL-86` | Bộ Tư lệnh 86 (Tác chiến không gian mạng) | Tác chiến mạng | Q. Cầu Giấy | 21.0298 | 105.7865 | 581,739 | 2,325,798 | 069.599.186 |
| 8 | `BC-DC` | Binh chủng Đặc công | Đặc công | H. Thanh Trì | 20.9150 | 105.8520 | 588,655 | 2,313,127 | 069.588.312 |
| 9 | `BC-PB` | Binh chủng Pháo binh | Pháo binh | TX. Sơn Tây | 21.1395 | 105.5012 | 552,060 | 2,337,842 | 069.591.220 |
| 10| `SB-HOALAC`| Trung đoàn Không quân 916 (Sân bay Hòa Lạc) | PK-KQ | H. Thạch Thất | 21.0385 | 105.5340 | 555,528 | 2,326,691 | 024.3384.6114 |
| 11| `TSQ-LQ1` | Trường Sĩ quan Lục quân 1 | Lục quân | TX. Sơn Tây | 21.0780 | 105.5180 | 553,838 | 2,331,048 | 024.3383.9102 |
| 12| `BCH-BD` | Ban CHQS Quận Ba Đình | Lục quân | Q. Ba Đình | 21.0346 | 105.8166 | 584,860 | 2,326,346 | 024.3832.5512 |
| ...| *và 38 đơn vị khác bao phủ toàn bộ 30 Quận, Huyện, Thị xã của Thủ đô Hà Nội* | | | | | | | | |
