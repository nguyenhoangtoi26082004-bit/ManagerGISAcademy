# 01. CẤU TRÚC GIẢI PHÁP C# WINFORMS + MAPWINGIS
## Đề tài: Hệ Thống CSDL GIS Quản Lý & Tra Cứu Đơn Vị Quân Đội Trên Địa Bàn Hà Nội
### Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)

---

## 1. MỤC TIÊU KIẾN TRÚC ỨNG DỤNG WINFORMS
Dự án được xây dựng dưới dạng ứng dụng máy tính **Desktop Application** sử dụng:
- **Ngôn ngữ**: C# (.NET Framework 4.8 hoặc .NET 6/8 Windows Forms).
- **Thư viện GIS**: **MapWinGIS ActiveX Control** (`AxMapWinGIS.AxMap`) để hiển thị bản đồ số, quản lý lớp Shapefile, bắt tọa độ con trỏ và vẽ vùng đệm không gian.
- **Hệ quản trị CSDL không gian**: **PostgreSQL 14+ kết hợp PostGIS 3.2+**.
- **Thư viện kết nối CSDL**: **Npgsql** (Data Provider chính thức cho PostgreSQL trên .NET).

Kiến trúc mã nguồn áp dụng mô hình **3 lớp phân tách (3-Tier Layered Architecture)**:
1. **GUI (Presentation Layer)**: Giao diện người dùng Windows Forms, hiển thị bản đồ `axMap1`, thanh công cụ Toolbar, bảng `DataGridView`.
2. **BLL / BUS (Business Logic Layer)**: Xử lý logic nghiệp vụ, tính toán trắc địa, chuyển đổi tọa độ VN-2000 $\Leftrightarrow$ WGS-84, điều phối bản đồ MapWinGIS.
3. **DAL / DAO (Data Access Layer)**: Mở kết nối `NpgsqlConnection`, thực thi các câu lệnh SQL không gian PostGIS (`ST_DWithin`, `ST_Buffer`, `ST_Distance`).
4. **DTO / Models**: Các lớp đối tượng dữ liệu đại diện cho thực thể đơn vị quân sự, quận/huyện, vùng đệm.

---

## 2. SƠ ĐỒ CÂY THƯ MỤC C# VISUAL STUDIO SOLUTION

```
HanoiMilitaryGIS.sln                 # Solution Visual Studio
└── HanoiMilitaryGIS/                # Project C# Windows Forms (.csproj)
    ├── App.config                   # Chuỗi kết nối ConnectionString tới PostGIS
    ├── packages.config              # Quản lý NuGet (Npgsql, Dapper, NetTopologySuite)
    ├── Program.cs                   # Điểm khởi chạy ứng dụng (Main)
    │
    ├── DTO/                         # DATA TRANSFER OBJECTS (LỚP ĐỐI TƯỢNG)
    │   ├── DonViQuanDoi.cs          # Thực thể đơn vị: Mã, Tên, Binh chủng, Tọa độ X/Y, SSCĐ...
    │   ├── QuanHuyen.cs             # Thực thể 30 quận/huyện Hà Nội & vùng phòng thủ
    │   ├── VungDemConfig.cs         # Cấu hình bán kính vùng đệm Buffer (R = 5km, 10km...)
    │   └── TopologyIssue.cs         # Lớp chứa thông tin cảnh báo lỗi tọa độ/sai số
    │
    ├── DAL/                         # DATA ACCESS LAYER (TẦNG TRUY XUẤT POSTGIS)
    │   ├── DbConnection.cs          # Quản lý chuỗi kết nối và Pool kết nối PostgreSQL
    │   ├── DonViQuanDoiDAL.cs       # Thực thi SELECT, INSERT, UPDATE, DELETE bảng don_vi_quan_doi
    │   ├── QuanHuyenDAL.cs          # Truy vấn ranh giới đa giác quận huyện
    │   └── SpatialAnalysisDAL.cs    # Gọi các hàm PostGIS ST_DWithin, ST_Buffer, ST_Distance
    │
    ├── BLL/                         # BUSINESS LOGIC LAYER (TẦNG NGHIỆP VỤ & GIS)
    │   ├── DonViQuanDoiBLL.cs       # Kiểm tra hợp lệ dữ liệu, phân quyền cán bộ
    │   ├── SpatialAnalysisBLL.cs    # Xử lý logic Vùng đệm phòng thủ, tìm K đơn vị gần nhất
    │   ├── CoordinateConverter.cs   # Thuật toán chuyển đổi tọa độ WGS84 <=> VN-2000 UTM 48N
    │   └── TopologyCheckerBLL.cs    # Rà soát lỗi điểm nằm ngoài đa giác địa giới Hà Nội
    │
    ├── GIS/                         # MODULE TÍCH HỢP MAPWINGIS CHUYÊN BIỆT
    │   ├── MapWinGISHelper.cs       # Khởi tạo bản đồ, cấu hình Projection EPSG:32648 / 4326
    │   ├── LayerManager.cs          # Nạp/xóa/ẩn/hiện các lớp Shapefile điểm, đường, vùng
    │   ├── MarkerStyler.cs          # Thiết lập biểu tượng quân sự SVG/Icon cho từng Binh chủng
    │   └── SpatialDrawer.cs         # Vẽ đường đo cự ly (Polyline) và vòng tròn Buffer (Polygon)
    │
    ├── GUI/                         # PRESENTATION LAYER (GIAO DIỆN WINDOWS FORMS)
    │   ├── FormMain.cs              # Form chính: Chứa MapWinGIS axMap1, Toolbar, Status Bar
    │   ├── FormMain.Designer.cs
    │   ├── FormTraCuu.cs            # Form tra cứu đa tiêu chí (Binh chủng, Cấp bậc, SSCĐ)
    │   ├── FormTraCuu.Designer.cs
    │   ├── FormChiTietDonVi.cs      # Form xem chi tiết thông tin tác chiến của đơn vị
    │   ├── FormChiTietDonVi.Designer.cs
    │   ├── FormThemSuaDonVi.cs      # Form nhập liệu đơn vị mới kèm chuyển đổi tọa độ
    │   ├── FormThemSuaDonVi.Designer.cs
    │   ├── FormPhanTichVungDem.cs   # Form tham số Buffer bán kính phòng thủ (3-35km)
    │   ├── FormKiemTraTopo.cs       # Form rà soát dữ liệu không gian
    │   └── FormBaoCaoThongKe.cs     # Form in ấn / xuất báo cáo kỹ thuật quân sự
    │
    ├── Resources/                   # TÀI NGUYÊN HỆ THỐNG
    │   ├── Icons/                   # Icon quân hiệu Lục quân, Không quân, Đặc công, Pháo binh...
    │   ├── Shapes/                  # Tệp ranh giới Hà Nội (HaNoi_Border.shp, QuanHuyen.shp)
    │   └── MTA_Logo.png             # Logo Học viện Kỹ thuật Quân sự
    │
    └── Database/                    # KỊCH BẢN CSDL LƯU TRỮ TRONG PROJECT
        └── hanoi_military_gis_postgis.sql # Script khởi tạo bảng, chỉ mục và dữ liệu mẫu
```

---

## 3. CÁCH TÍCH HỢP MAPWINGIS ACTIVE-X CONTROL VÀO VISUAL STUDIO

### Bước 1: Cài đặt bộ MapWinGIS
1. Tải bộ cài đặt MapWinGIS x64 hoặc x86 (khuyến nghị bản **v5.2.4+**) từ trang chủ `https://www.mapwindow.org/` và cài đặt lên máy tính.
2. Bộ cài đặt sẽ tự động đăng ký thư viện COM `MapWinGIS.ocx` vào hệ thống Windows.

### Bước 2: Thêm MapWinGIS vào Toolbox Visual Studio
1. Mở Solution trên Visual Studio.
2. Mở chế độ Design của `FormMain.cs`.
3. Nhấp chuột phải vào cửa sổ **Toolbox** $\rightarrow$ Chọn **Choose Items...**.
4. Chọn thẻ **COM Components** $\rightarrow$ Tìm và tích chọn **MapWinGIS Control** (hoặc browse tới tệp `C:\dev\MapWinGIS\MapWinGIS.ocx`).
5. Bấm **OK**. Kéo thả biểu tượng **Map Control** vào giao diện form, đặt tên là `axMap1`.

---

## 4. CHI TIẾT CÁC LỚP MÃ NGUỒN CỐT LÕI

### 4.1. Lớp Đối tượng Đơn vị Quân sự (`DTO/DonViQuanDoi.cs`)
```csharp
namespace HanoiMilitaryGIS.DTO
{
    public class DonViQuanDoi
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenVietTat { get; set; }
        public string LoaiDonVi { get; set; }
        public string QuanBinhChung { get; set; }
        public string DonViCapTren { get; set; }
        public string QuanHuyen { get; set; }
        public string DiaChi { get; set; }
        
        // Tọa độ địa lý WGS-84 phục vụ hiển thị
        public double ViDo { get; set; }      // Latitude (deg)
        public double KinhDo { get; set; }    // Longitude (deg)
        
        // Tọa độ phẳng VN-2000 UTM Zone 48N (EPSG:32648) tính cự ly mét
        public double XVn2000 { get; set; }  // Easting (m)
        public double YVn2000 { get; set; }  // Northing (m)

        public string ChiHuyTruong { get; set; }
        public string SoDienThoai { get; set; }
        public string TrangThaiSSCD { get; set; }
        public string DoMat { get; set; }
        public string[] NhiemVuChinh { get; set; }
        public string[] VuKhiTrangBi { get; set; }
    }
}
```

### 4.2. Khởi tạo Bản đồ và Bắt Tọa độ Chuột (`GIS/MapWinGISHelper.cs` & `FormMain.cs`)
```csharp
using AxMapWinGIS;
using MapWinGIS;

namespace HanoiMilitaryGIS.GIS
{
    public class MapWinGISHelper
    {
        private AxMap _map;

        public MapWinGISHelper(AxMap map)
        {
            _map = map;
            ConfigureMap();
        }

        private void ConfigureMap()
        {
            // Thiết lập bản đồ nền vệ tinh OpenStreetMap / Esri
            _map.TileProvider = tkTileProvider.OpenStreetMap;
            _map.KnownExtents = tkKnownExtents.keVietnam;

            // Bật chế độ hiển thị thước tỷ lệ Scale bar
            _map.ShowScaleBar = true;
            _map.ScaleBarUnits = tkScaleBarUnits.Kilometers;

            // Thiết lập hệ quy chiếu WGS-84 EPSG:4326
            _map.GeoProjection.SetWgs84();
        }

        // Bắt tọa độ con trỏ chuột thời gian thực hiển thị lên StatusStrip
        public void HandleMouseMove(int x, int y, out double lon, out double lat)
        {
            double projX = 0, projY = 0;
            _map.PixelToProj(x, y, ref projX, ref projY);
            lon = projX;
            lat = projY;
        }
    }
}
```

---

## 5. NGUYÊN TẮC PHÁT TRIỂN TIẾP THEO CHO NHÓM
1. **Tách biệt code giao diện và code SQL**: Mọi câu lệnh SQL tương tác PostGIS phải viết bên `DAL/`, không viết trực tiếp trong sự kiện click của Button.
2. **Quản lý LayerHandle**: Khi nạp các lớp Shapefile vào `axMap1`, luôn lưu lại `int layerHandle` để có thể bật/tắt hoặc xóa lớp khi chuyển chế độ phân tích.
3. **Đồng bộ bảng dữ liệu Grid**: Khi người dùng click chọn 1 dòng trên `DataGridView`, gọi phương thức `axMap1.ZoomToShape()` để bản đồ tự động bay đến vị trí đơn vị đó.
