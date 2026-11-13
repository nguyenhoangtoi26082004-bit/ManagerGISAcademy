# 05. PHÂN CHIA NHIỆM VỤ NHÓM 4 NGƯỜI & QUY TRÌNH GITHUB
## Đồ Án: C# WinForms + MapWinGIS + PostgreSQL/PostGIS
### Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)

---

## 1. PHÂN CÔNG VAI TRÒ NHÓM 4 THÀNH VIÊN

Để đạt điểm tối đa môn Hệ quản trị CSDL Không gian, nhóm 4 người cần phân công đúng chuyên môn, mỗi thành viên chịu trách nhiệm một phần việc cụ thể:

```
                            [ THÀNH VIÊN 1 - TRƯỞNG NHÓM ]
                               GUI & TỔNG THỂ HỆ THỐNG
                           (WinForms, Controls, FormMain)
                                       │
       ┌───────────────────────────────┼───────────────────────────────┐
       ▼                               ▼                               ▼
[ THÀNH VIÊN 2 ]               [ THÀNH VIÊN 3 ]                [ THÀNH VIÊN 4 ]
  MAPWINGIS CHUYÊN SÂU          POSTGIS & TẦNG DAL             TESTER & BÁO CÁO
(Bản đồ, Lớp Shapefile,       (Database, Stored Procedure,     (Kiểm thử Topo, Viết Báo cáo
 Vẽ Buffer, Đo cự ly)         Chỉ mục GIST, Npgsql DAL)        Word & Slide PowerPoint)
```

---

### BẢNG CHI TIẾT CÔNG VIỆC CỦA TỪNG THÀNH VIÊN

| Thành viên | Vị trí đảm nhiệm | Nhiệm vụ cụ thể trong đồ án | Kết quả bàn giao (Deliverables) |
| :--- | :--- | :--- | :--- |
| **Thành viên 1**<br>*(Trưởng nhóm)* | **WinForms GUI Lead** | - Quản lý cấu trúc Solution Visual Studio (`.sln`, `.csproj`)<br>- Thiết kế FormMain: Thanh Toolbar bên trái, thanh tìm kiếm trên đỉnh, StatusStrip hiển thị tọa độ X/Y mét dưới đáy<br>- Thiết kế FormTraCuu, FormThemSuaDonVi, FormChiTietDonVi<br>- Tích hợp các module lại với nhau, quản lý GitHub Repository | - Mã nguồn thư mục `GUI/`<br>- Giao diện phần mềm chuẩn chỉ huy quân sự |
| **Thành viên 2** | **MapWinGIS Developer** | - Cài đặt MapWinGIS SDK, cấu hình điều khiển `axMap1`<br>- Lập trình nạp lớp Shapefile điểm các đơn vị quân đội lên bản đồ<br>- Lập trình sự kiện click chuột `PixelToProj` bắt tọa độ kinh/vĩ độ<br>- Lập trình vẽ vùng đệm Buffer (Polygon) và vẽ đường đo cự ly (Polyline) trên `axMap1` | - Mã nguồn thư mục `GIS/` (`MapWinGISHelper.cs`, `SpatialDrawer.cs`)<br>- Bản đồ tương tác mượt mà không lỗi |
| **Thành viên 3** | **PostGIS & DAL Developer** | - Cài đặt PostgreSQL 15 + PostGIS 3.3 trên máy tính<br>- Tạo bảng `don_vi_quan_doi` với cột `geom_32648` (UTM 48N) và `geom_4326`<br>- Tạo chỉ mục GIST, trigger tự động tính toán tọa độ<br>- Viết các hàm không gian: `ST_DWithin`, `ST_Buffer`, `ST_Distance`<br>- Viết tầng `DAL/` trong C# sử dụng thư viện Npgsql | - Tệp kịch bản `hanoi_military_gis_postgis.sql`<br>- Mã nguồn thư mục `DAL/` (`DbConnection.cs`, `DonViQuanDoiDAL.cs`) |
| **Thành viên 4** | **QA Tester & Báo cáo BTL** | - Kiểm thử toàn diện phần mềm: Rà soát lỗi kết nối CSDL, lỗi tọa độ ngoài biên giới Hà Nội<br>- Soạn thảo cuốn **Báo cáo thuyết minh đồ án** (Word 40 - 50 trang) chuẩn mẫu của học viện/khoa<br>- Thiết kế **Slide thuyết trình PowerPoint** (15 - 20 slide chuyên nghiệp)<br>- Chuẩn bị kịch bản Demo phân vai khi báo cáo trước giảng viên | - File Báo cáo Word + PDF hoàn chỉnh<br>- File Slide PowerPoint + Bộ kịch bản Demo |

---

## 2. QUY TRÌNH QUẢN LÝ MÃ NGUỒN QUA GITHUB CHO NHÓM

### 2.1. Cấu hình tệp `.gitignore` cho Visual Studio C# (Cực kỳ quan trọng)
Khi làm việc nhóm với Visual Studio, **tuyệt đối không đẩy thư mục `bin/`, `obj/`, `.vs/`** lên GitHub vì sẽ gây xung đột file nhị phân giữa các máy.

Tạo tệp `.gitignore` tại thư mục gốc của Solution:
```gitignore
## Visual Studio
.vs/
[Bb]in/
[Oo]bj/
*.user
*.suo
*.userosscache
*.sln.docstates

## Packages
packages/
*.nupkg

## MapWinGIS cache
*.mwsr
*.mwh
*.mwe
```

---

### 2.2. Chiến lược Phân nhánh (Git Branching Model)

- **`main`**: Nhánh chính thức nộp bài cho giảng viên.
- **`develop`**: Nhánh tích hợp chung của nhóm.
- **`feature/gui-winforms`**: Thành viên 1 làm việc.
- **`feature/mapwingis-layers`**: Thành viên 2 làm việc.
- **`feature/postgis-dal`**: Thành viên 3 làm việc.
- **`feature/testing-docs`**: Thành viên 4 làm việc.

```
[main] ──────────────────────────────────────────────────● (Bản nộp BTL)
          ▲                                              │
[develop] ┴─────────●────────────────────────●───────────┘ (Merge tuần)
                     ▲                        ▲
[feature]            │ (PR của TV1, TV2)      │ (PR của TV3, TV4)
  gui-winforms ──────┘                        │
  postgis-dal  ───────────────────────────────┘
```

---

## 3. CÁC LỆNH GIT TỪNG BƯỚC ĐỂ BẮT ĐẦU DỰ ÁN

### Bước 1: Trưởng nhóm tạo Solution Visual Studio và đẩy lên GitHub
```bash
# Di chuyển vào thư mục Solution C#
cd HanoiMilitaryGIS

# Khởi tạo Git
git init

# Thêm toàn bộ mã nguồn
git add .
git commit -m "feat: khoi tao Visual Studio Solution WinForms MapWinGIS PostGIS"

# Đẩy lên GitHub
git branch -M main
git remote add origin https://github.com/your-team/gis-quan-doi-hanoi-winforms.git
git push -u origin main

# Tạo nhánh develop
git checkout -b develop
git push -u origin develop
```

### Bước 2: Các thành viên Clone về máy tính
```bash
git clone https://github.com/your-team/gis-quan-doi-hanoi-winforms.git
cd gis-quan-doi-hanoi-winforms
git checkout develop
```
*Lưu ý*: Sau khi clone, mở Solution trên Visual Studio $\rightarrow$ Nhấp chuột phải vào Solution chọn **Restore NuGet Packages** để tự động tải lại thư viện Npgsql.

### Bước 3: Làm việc hàng ngày trên nhánh riêng
```bash
# Trước khi làm việc, luôn kéo mã mới nhất về:
git checkout develop
git pull origin develop

# Tạo nhánh tính năng của bạn (ví dụ Thành viên 3 làm PostGIS DAL):
git checkout -b feature/postgis-dal

# Sau khi viết code và test thành công:
git status
git add .
git commit -m "feat: viet xong ham DonViQuanDoiDAL lay danh sach theo vung dem ST_Buffer"
git push -u origin feature/postgis-dal
```

### Bước 4: Tạo Pull Request (PR) để Trưởng nhóm Review và Merge
1. Vào trang GitHub Repo của nhóm.
2. Bấm **New Pull Request** $\rightarrow$ Chọn merge từ nhánh của bạn vào `develop`.
3. Gửi link PR vào nhóm Zalo/Discord để Trưởng nhóm kiểm tra, build thử không bị lỗi thì bấm **Merge pull request**.

---

## 4. KỊCH BẢN PHÂN VAI KHI THUYẾT TRÌNH BẢO VỆ BÀI TẬP LỚN (15 PHÚT)

Khi lên báo cáo trước giảng viên môn Hệ quản trị CSDL Không gian, nhóm chia đều thời lượng như sau:

1. **Phút 01 - 03 (Trưởng nhóm / TV1)**:
   - Giới thiệu đề tài: "Xây dựng CSDL GIS tra cứu và quản lý các đơn vị quân đội trên địa bàn Hà Nội".
   - Trình bày kiến trúc 3 lớp C# WinForms kết hợp MapWinGIS và PostgreSQL/PostGIS.
   - Demo tổng quan giao diện FormMain và thanh công cụ.
2. **Phút 04 - 08 (Thành viên 3 - Phụ trách PostGIS)**:
   - Trình bày mô hình CSDL: Lược đồ bảng `don_vi_quan_doi`, giải thích ý nghĩa hệ tọa độ VN-2000 (SRID 32648) và WGS-84 (SRID 4326).
   - Giải thích vai trò của **Chỉ mục không gian GIST** và lệnh `EXPLAIN ANALYZE` chứng minh tốc độ truy vấn vượt trội.
   - Trình bày các hàm PostGIS đã lập trình: `ST_DWithin`, `ST_Buffer`, `ST_Distance` và toán tử `<->`.
3. **Phút 09 - 12 (Thành viên 2 - Phụ trách MapWinGIS)**:
   - Demo trực tiếp chức năng nạp lớp điểm đơn vị từ CSDL PostGIS lên bản đồ `axMap1`.
   - Demo chức năng **Phân tích Vùng đệm**: Bấm chọn Học viện Kỹ thuật Quân sự, chọn bán kính 10km $\rightarrow$ Hệ thống gọi PostGIS tạo Polygon và vẽ vòng tròn bao phủ lên bản đồ, liệt kê các đơn vị chi viện.
   - Demo chức năng **Tìm K đơn vị gần nhất**: Bấm một điểm bất kỳ trên bản đồ, tìm 5 đơn vị gần nhất và đo cự ly trắc địa.
4. **Phút 13 - 14 (Thành viên 4 - QA & Báo cáo)**:
   - Trình bày chức năng rà soát lỗi Topo (phát hiện tọa độ gõ nhầm nằm ngoài ranh giới Hà Nội).
   - Trình bày báo cáo thống kê cơ cấu lực lượng và kết luận đề tài.
5. **Phút 15 (Cả nhóm)**:
   - Trả lời câu hỏi vấn đáp chuyên môn của giảng viên về PostGIS và xử lý không gian.
