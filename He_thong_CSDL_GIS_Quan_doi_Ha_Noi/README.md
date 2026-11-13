# HỆ THỐNG CƠ SỞ DỮ LIỆU GIS QUẢN LÝ VÀ TRA CỨU ĐƠN VỊ QUÂN ĐỘI TRÊN ĐỊA BÀN HÀ NỘI

> **Đề tài:** "Xây cơ sở dữ liệu GIS tra cứu thông tin và quản lý các đơn vị quân đội trên địa bàn Hà Nội"  
> **Đơn vị thực hiện:** Học viện Kỹ thuật Quân sự (MTA) / Bộ môn Bản đồ - GIS & Viễn thám  
> **Hệ quy chiếu không gian chuẩn:** VN-2000 / UTM Zone 48N (EPSG:32648) & WGS-84 (EPSG:4326)

---

## 1. GIỚI THIỆU TỔNG QUAN

Hệ thống được phát triển nhằm mục đích số hóa, quản lý dữ liệu không gian và tra cứu thông tin của các đơn vị quân đội (cơ quan Bộ Quốc phòng, các quân chủng, binh chủng, sư đoàn, lữ đoàn, các học viện nhà trường, bệnh viện quân y và Ban Chỉ huy Quân sự 30 quận/huyện/thị xã) trên địa bàn thành phố Hà Nội.

### Các phân hệ chính:
1. **Bản đồ số tương tác GIS (Interactive GIS Tactical Map)**: Bản đồ vệ tinh Esri, Dark mode tác chiến, Topo quân sự, lớp phủ ranh giới 30 quận huyện, đo đạc cự ly tác chiến, tính bán kính vùng đệm phòng thủ.
2. **Tra cứu đa tiêu chí**: Tìm kiếm theo tên, mã định danh, quân binh chủng, cấp đơn vị, địa bàn hành chính, trạng thái sẵn sàng chiến đấu (SSCĐ).
3. **Quản trị dữ liệu không gian (Spatial CRUD)**: Thêm, sửa, xóa các thực thể điểm đóng quân; chuyển đổi tọa độ tự động hai chiều giữa WGS-84 và VN-2000 (m).
4. **Phân tích không gian (Spatial Analysis)**:
   - Phân tích Vùng đệm phòng thủ (*Buffer Zone* 3km - 35km).
   - Truy vấn đơn vị gần nhất (*K-Nearest Neighbors / K-NN*).
   - Phân tích 5 khu vực phòng thủ chiến lược (Trung tâm, Phía Tây, Phía Bắc, Phía Nam, Phía Đông).
5. **CSDL Không gian PostGIS**: Sinh tự động tệp SQL DDL, GIST Spatial Indexing và các câu truy vấn mẫu (`ST_DWithin`, `ST_Buffer`, `ST_Distance`).
6. **Thống kê & Báo cáo kỹ thuật**: Biểu đồ phân bổ lực lượng, xuất bản báo cáo quân sự chuẩn in ấn (PDF).
7. **Kiểm tra Topo & Dữ liệu (Topology & Quality Audit)**: Tự động rà soát điểm nằm ngoài ranh giới Hà Nội (Ray-Casting algorithm), trùng mã quân sự, thiếu thông tin trực ban.

---

## 2. YÊU CẦU HỆ THỐNG & CÔNG NGHỆ

- **Node.js**: Phiên bản 18 trở lên (Khuyến nghị Node.js 20 LTS)
- **Trình quản lý gói**: npm hoặc yarn
- **Ngôn ngữ**: TypeScript, React 19
- **Công nghệ GIS**: Leaflet 1.9+, Esri World Imagery, CartoDB Dark, OpenTopoMap
- **Cơ sở dữ liệu không gian**: PostgreSQL 14+ kết hợp PostGIS 3.2+ (tùy chọn kết nối)
- **Styling**: Tailwind CSS v4

---

## 3. HƯỚNG DẪN CÀI ĐẶT VÀ CHẠY DỰ ÁN TRÊN MÁY TÍNH

### Bước 1: Mở thư mục dự án
Giải nén tệp zip đã tải về và mở thư mục bằng Visual Studio Code (hoặc Terminal):

```bash
cd he-thong-gis-quan-doi-hanoi
```

### Bước 2: Cài đặt các thư viện phụ thuộc
Chạy lệnh sau trong Terminal để cài đặt toàn bộ dependencies:

```bash
npm install
```

### Bước 3: Chạy ứng dụng ở môi trường phát triển (Development)
Khởi động máy chủ phát triển cục bộ:

```bash
npm run dev
```

Sau khi chạy xong, mở trình duyệt web và truy cập địa chỉ:
```
http://localhost:3000
```
(hoặc cổng hiển thị trên Terminal của bạn).

### Bước 4: Đóng gói dự án để triển khai (Production Build)
```bash
npm run build
npm run preview
```

---

## 4. HƯỚNG DẪN NẠP DỮ LIỆU VÀO POSTGIS (TÙY CHỌN)

Dự án có sẵn tệp kịch bản `database/hanoi_military_gis_postgis.sql` (hoặc xuất từ giao diện của hệ thống).

Để nạp vào hệ quản trị CSDL PostgreSQL / PostGIS trên máy của bạn:

1. Mở **pgAdmin 4** (hoặc DBeaver).
2. Tạo cơ sở dữ liệu mới mang tên: `hanoi_military_gis`.
3. Mở **Query Tool** và mở tệp `hanoi_military_gis_postgis.sql`.
4. Nhấn **Execute (F5)** để khởi tạo:
   - Phần mở rộng `postgis` và `postgis_topology`.
   - Bảng `don_vi_quan_doi` với cột hình học `geom_32648` (EPSG:32648) và `geom_4326` (EPSG:4326).
   - Chỉ mục không gian `GIST`.
   - Toàn bộ dữ liệu các đơn vị quân đội trên địa bàn Thủ đô Hà Nội.

---

## 5. CẤU TRÚC THƯ MỤC NGUỒN

```
├── .env.example              # Biến môi trường mẫu
├── index.html                # Trang web chủ với Leaflet CSS & Google Fonts
├── metadata.json             # Khai báo thông tin dự án
├── package.json              # Danh sách thư viện phụ thuộc
├── tsconfig.json             # Cấu hình TypeScript
├── vite.config.ts            # Cấu hình Vite & Tailwind
├── README.md                 # Hướng dẫn chi tiết dự án
├── database/                 # Dữ liệu xuất bản PostGIS SQL, GeoJSON, CSV
│   ├── hanoi_military_gis_postgis.sql
│   ├── hanoi_military_units.geojson
│   └── hanoi_military_units.csv
└── src/
    ├── main.tsx              # Điểm khởi chạy React
    ├── App.tsx               # Component điều phối chính
    ├── index.css             # CSS toàn cục và styling Leaflet dark mode
    ├── types/
    │   └── gis.ts            # Khai báo kiểu dữ liệu quân sự và GIS
    ├── data/
    │   ├── hanoiDistricts.ts # Ranh giới và thông tin 30 Quận/Huyện Hà Nội
    │   └── militaryUnits.ts  # Bộ dữ liệu mẫu các đơn vị quân đội Hà Nội
    ├── utils/
    │   ├── coordinateConversion.ts # Chuyển đổi WGS84 <=> VN-2000 UTM 48N
    │   ├── gisCalculations.ts      # Vùng đệm, K-NN, Ray-casting topo
    │   ├── postgisExport.ts        # Bộ sinh mã PostGIS DDL & CSV
    │   └── projectZipExporter.ts   # Công cụ đóng gói toàn bộ mã nguồn (.zip)
    └── components/
        ├── Header.tsx        # Thanh tiêu đề, logo MTA, tra cứu nhanh, nút tải zip
        ├── Sidebar.tsx       # Menu 7 phân hệ chức năng
        ├── StatusBar.tsx     # Thanh trạng thái tọa độ X/Y mét thời gian thực
        ├── GisMap.tsx        # Bản đồ số tương tác chuyên dụng
        ├── UnitDetailDrawer.tsx # Bảng thông tin chi tiết đơn vị
        ├── DashboardView.tsx # Phân hệ Tổng quan
        ├── UnitSearchView.tsx# Phân hệ Tra cứu
        ├── DataManagementView.tsx # Phân hệ Quản lý dữ liệu
        ├── SpatialAnalysisView.tsx# Phân hệ Phân tích không gian
        ├── StatisticsView.tsx# Phân hệ Thống kê & Báo cáo
        ├── DataCheckerView.tsx # Phân hệ Kiểm tra dữ liệu Topo
        ├── UnitModal.tsx     # Biểu mẫu Thêm/Sửa đơn vị với quy đổi tọa độ
        └── PostgisModal.tsx  # Cửa sổ chi tiết kết nối & schema PostGIS
```

---

## 6. BẢN QUYỀN VÀ THÔNG TIN ĐỀ TÀI
- Phát triển phục vụ học tập, nghiên cứu khoa học và bảo vệ đề tài / luận văn ứng dụng Hệ thống thông tin địa lý (GIS) trong lĩnh vực Quốc phòng - An ninh.
