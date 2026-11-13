# 02. THIẾT KẾ CSDL POSTGRESQL + POSTGIS & KẾT NỐI C# NPGSQL
## Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)
### Đề tài: Quản lý & Tra cứu Đơn vị Quân đội trên Địa bàn TP Hà Nội

---

## 1. MỤC TIÊU MÔN HỌC & YÊU CẦU THIẾT KẾ CSDL KHÔNG GIAN
Trong môn học **Hệ quản trị CSDL Không gian**, mục tiêu cốt lõi là làm chủ các khái niệm:
- **Spatial Data Types (Kiểu dữ liệu không gian)**: `POINT`, `POLYGON`, `MULTIPOLYGON`.
- **Spatial Reference System Identifier (SRID)**: 
  - `SRID 32648`: Lưới chiếu phẳng UTM Zone 48N (VN-2000), đơn vị đo là **mét (m)** $\rightarrow$ Dùng cho tính toán cự ly thực tế, hỏa lực pháo binh và vùng đệm buffer không bị méo tỷ lệ.
  - `SRID 4326`: Hệ tọa độ trắc địa WGS-84, đơn vị là **độ (degrees)** $\rightarrow$ Dùng để tương thích hiển thị trên bản đồ nền MapWinGIS.
- **Spatial Indexing (Chỉ mục không gian GIST)**: Tối ưu hóa truy vấn không gian dựa trên cây R-Tree.
- **Spatial Operations (Các phép toán không gian OGC)**: `ST_DWithin`, `ST_Buffer`, `ST_Distance`, `ST_Contains`, `ST_AsText` (WKT).

---

## 2. KỊCH BẢN KHỞI TẠO CSDL KHÔNG GIAN (POSTGIS DDL)

### 2.1. Kích hoạt phần mở rộng PostGIS
```sql
-- Mở pgAdmin 4 -> Tạo database: hanoi_military_gis
-- Mở Query Tool và chạy:
CREATE EXTENSION IF NOT EXISTS postgis;
CREATE EXTENSION IF NOT EXISTS postgis_topology;
```

### 2.2. Bảng 1: Bảng Đơn vị Quân đội (`don_vi_quan_doi`)
```sql
DROP TABLE IF EXISTS don_vi_quan_doi CASCADE;

CREATE TABLE don_vi_quan_doi (
    -- Khóa chính & Định danh quân sự
    id SERIAL PRIMARY KEY,
    ma_don_vi VARCHAR(50) NOT NULL UNIQUE,          -- VD: BQP-HQ, HV-KTQS, BCH-BD...
    ten_don_vi VARCHAR(255) NOT NULL,               -- Tên đầy đủ
    ten_viet_tat VARCHAR(100),                      -- Tên viết tắt
    loai_don_vi VARCHAR(100) NOT NULL,              -- Cơ quan BQP, Sư đoàn, Lữ đoàn, Ban CHQS...
    quan_binh_chung VARCHAR(100) NOT NULL,          -- Lục quân, PK-KQ, Đặc công, Pháo binh...
    don_vi_cap_tren VARCHAR(255) NOT NULL,          -- Đơn vị chủ quản cấp trên

    -- Phân vùng hành chính
    quan_huyen VARCHAR(100) NOT NULL,               -- Ba Đình, Hoàn Kiếm, Cầu Giấy...
    dia_chi TEXT NOT NULL,                          -- Địa chỉ đóng quân chi tiết

    -- Thuộc tính tọa độ số trị (Numeric Attributes)
    vi_do DOUBLE PRECISION NOT NULL,                -- Latitude WGS84 (deg)
    kinh_do DOUBLE PRECISION NOT NULL,              -- Longitude WGS84 (deg)
    x_vn2000 DOUBLE PRECISION NOT NULL,             -- Easting UTM 48N (m)
    y_vn2000 DOUBLE PRECISION NOT NULL,             -- Northing UTM 48N (m)

    -- Thuộc tính tác chiến & chỉ huy
    chi_huy_truong VARCHAR(150),                    -- Cấp bậc + Họ tên
    chinh_uy VARCHAR(150),                          -- Chính ủy / Chính trị viên
    so_dien_thoai VARCHAR(50) NOT NULL,             -- Máy trực ban tác chiến
    trang_thai_sscd VARCHAR(100) DEFAULT 'Cấp 1 (Cao nhất)', -- Cấp 1, Cấp 2, Cấp 3, Huấn luyện
    do_mat VARCHAR(50) DEFAULT 'Mật',               -- Tuyệt mật, Tối mật, Mật, Nội bộ
    nhiem_vu_chinh TEXT,                            -- Chuỗi mô tả nhiệm vụ
    vu_khi_trang_bi TEXT,                           -- Chuỗi khí tài trang bị

    -- CỘT DỮ LIỆU HÌNH HỌC KHÔNG GIAN (SPATIAL GEOMETRY COLUMNS)
    -- 1. geom_4326: Hiển thị lên MapWinGIS (kinh độ, vĩ độ)
    geom_4326 geometry(Point, 4326),

    -- 2. geom_32648: Tính toán cự ly thực tế (mét)
    geom_32648 geometry(Point, 32648),

    ngay_cap_nhat TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### 2.3. Đánh Chỉ Mục Không Gian (Spatial Indexing)
Trong bài tập lớn môn CSDL Không gian, chỉ mục GIST là tiêu chí chấm điểm kỹ thuật bắt buộc:
```sql
-- Đánh chỉ mục GIST trên cột hình học 4326 và 32648
CREATE INDEX idx_donvi_geom_4326 ON don_vi_quan_doi USING GIST (geom_4326);
CREATE INDEX idx_donvi_geom_32648 ON don_vi_quan_doi USING GIST (geom_32648);

-- Đánh chỉ mục B-tree trên thuộc tính thường để lọc kết hợp
CREATE INDEX idx_donvi_ma_dv ON don_vi_quan_doi (ma_don_vi);
CREATE INDEX idx_donvi_binh_chung ON don_vi_quan_doi (quan_binh_chung);
CREATE INDEX idx_donvi_quan_huyen ON don_vi_quan_doi (quan_huyen);
```

### 2.4. Trigger Tự Động Khởi Tạo Cột Hình Học (Auto-Compute Trigger)
```sql
CREATE OR REPLACE FUNCTION fn_trg_cap_nhat_hinh_hoc()
RETURNS TRIGGER AS $$
BEGIN
    -- Tạo geom_4326 từ kinh độ, vĩ độ
    NEW.geom_4326 := ST_SetSRID(ST_MakePoint(NEW.kinh_do, NEW.vi_do), 4326);

    -- Tự động chuyển đổi sang geom_32648 (VN-2000 UTM Zone 48N)
    NEW.geom_32648 := ST_Transform(NEW.geom_4326, 32648);

    -- Tự động trích xuất X, Y mét
    NEW.x_vn2000 := ROUND(ST_X(NEW.geom_32648)::numeric, 2);
    NEW.y_vn2000 := ROUND(ST_Y(NEW.geom_32648)::numeric, 2);

    NEW.ngay_cap_nhat := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_cap_nhat_geom ON don_vi_quan_doi;
CREATE TRIGGER trg_cap_nhat_geom
BEFORE INSERT OR UPDATE ON don_vi_quan_doi
FOR EACH ROW
EXECUTE FUNCTION fn_trg_cap_nhat_hinh_hoc();
```

---

## 3. CÁC STORED PROCEDURES / TRUY VẤN KHÔNG GIAN BẮT BUỘC TRONG ĐỒ ÁN

### 3.1. Phân tích Vùng đệm phòng thủ (Buffer Analysis)
Tìm tất cả đơn vị nằm trong bán kính $R$ mét quanh 1 vị trí sở chỉ huy và tạo vùng đệm Polygon:
```sql
CREATE OR REPLACE FUNCTION sp_phan_tich_vung_dem(
    p_ma_don_vi VARCHAR(50),
    p_ban_kinh_met DOUBLE PRECISION
)
RETURNS TABLE (
    ma_don_vi VARCHAR,
    ten_don_vi VARCHAR,
    quan_binh_chung VARCHAR,
    khoang_cach_met DOUBLE PRECISION,
    geom_wkt TEXT                                -- Xuất định dạng WKT để C# MapWinGIS nạp vào Shape
) AS $$
BEGIN
    RETURN QUERY
    WITH target AS (
        SELECT geom_32648 FROM don_vi_quan_doi WHERE don_vi_quan_doi.ma_don_vi = p_ma_don_vi LIMIT 1
    )
    SELECT 
        u.ma_don_vi,
        u.ten_don_vi,
        u.quan_binh_chung,
        ROUND(ST_Distance(u.geom_32648, t.geom_32648)::numeric, 2)::DOUBLE PRECISION,
        ST_AsText(u.geom_4326) AS geom_wkt
    FROM don_vi_quan_doi u, target t
    WHERE ST_DWithin(u.geom_32648, t.geom_32648, p_ban_kinh_met)
      AND u.ma_don_vi <> p_ma_don_vi
    ORDER BY khoang_cach_met ASC;
END;
$$ LANGUAGE plpgsql;
```

### 3.2. Truy vấn K-Nearest Neighbors (Tìm K đơn vị gần nhất)
Sử dụng toán tử khoảng cách `<->` của PostGIS tận dụng cây chỉ mục GIST:
```sql
CREATE OR REPLACE FUNCTION sp_tim_k_don_vi_gan_nhat(
    p_vi_do DOUBLE PRECISION,
    p_kinh_do DOUBLE PRECISION,
    p_k INT
)
RETURNS TABLE (
    ma_don_vi VARCHAR,
    ten_don_vi VARCHAR,
    quan_binh_chung VARCHAR,
    khoang_cach_km DOUBLE PRECISION,
    geom_wkt TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT 
        u.ma_don_vi,
        u.ten_don_vi,
        u.quan_binh_chung,
        ROUND((ST_Distance(u.geom_32648, ST_Transform(ST_SetSRID(ST_MakePoint(p_kinh_do, p_vi_do), 4326), 32648)) / 1000.0)::numeric, 2)::DOUBLE PRECISION,
        ST_AsText(u.geom_4326)
    FROM don_vi_quan_doi u
    ORDER BY u.geom_32648 <-> ST_Transform(ST_SetSRID(ST_MakePoint(p_kinh_do, p_vi_do), 4326), 32648)
    LIMIT p_k;
END;
$$ LANGUAGE plpgsql;
```

---

## 4. CODE MẪU TẦNG DAL C# KẾT NỐI QUA THƯ VIỆN NPGSQL

Cài đặt package NuGet trên Visual Studio:
```bash
Install-Package Npgsql
```

### 4.1. Lớp Quản lý Kết nối CSDL (`DAL/DbConnection.cs`)
```csharp
using Npgsql;
using System.Configuration;

namespace HanoiMilitaryGIS.DAL
{
    public class DbConnection
    {
        // Chuỗi kết nối lấy từ App.config
        private static string connStr = ConfigurationManager.ConnectionStrings["PostgisHanoi"].ConnectionString;

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connStr);
        }
    }
}
```

*Cấu hình trong `App.config`:*
```xml
<connectionStrings>
    <add name="PostgisHanoi" 
         connectionString="Host=localhost;Port=5432;Database=hanoi_military_gis;Username=postgres;Password=your_password;Timeout=15;" 
         providerName="Npgsql" />
</connectionStrings>
```

### 4.2. Lớp Truy Xuất Đơn Vị (`DAL/DonViQuanDoiDAL.cs`)
```csharp
using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using HanoiMilitaryGIS.DTO;

namespace HanoiMilitaryGIS.DAL
{
    public class DonViQuanDoiDAL
    {
        // Lấy toàn bộ đơn vị kèm chuỗi WKT để nạp lên MapWinGIS
        public List<DonViQuanDoi> LayTatCaDonVi()
        {
            var list = new List<DonViQuanDoi>();
            string sql = "SELECT ma_don_vi, ten_don_vi, ten_viet_tat, loai_don_vi, quan_binh_chung, " +
                         "quan_huyen, dia_chi, vi_do, kinh_do, x_vn2000, y_vn2000, chi_huy_truong, " +
                         "so_dien_thoai, trang_thai_sscd, do_mat, ST_AsText(geom_4326) as wkt " +
                         "FROM don_vi_quan_doi ORDER BY ma_don_vi;";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DonViQuanDoi
                        {
                            MaDonVi = reader["ma_don_vi"].ToString(),
                            TenDonVi = reader["ten_don_vi"].ToString(),
                            TenVietTat = reader["ten_viet_tat"].ToString(),
                            LoaiDonVi = reader["loai_don_vi"].ToString(),
                            QuanBinhChung = reader["quan_binh_chung"].ToString(),
                            QuanHuyen = reader["quan_huyen"].ToString(),
                            DiaChi = reader["dia_chi"].ToString(),
                            ViDo = Convert.ToDouble(reader["vi_do"]),
                            KinhDo = Convert.ToDouble(reader["kinh_do"]),
                            XVn2000 = Convert.ToDouble(reader["x_vn2000"]),
                            YVn2000 = Convert.ToDouble(reader["y_vn2000"]),
                            ChiHuyTruong = reader["chi_huy_truong"].ToString(),
                            SoDienThoai = reader["so_dien_thoai"].ToString(),
                            TrangThaiSSCD = reader["trang_thai_sscd"].ToString(),
                            DoMat = reader["do_mat"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Thêm mới đơn vị với tọa độ PostGIS ST_SetSRID
        public bool ThemDonVi(DonViQuanDoi dv)
        {
            string sql = @"INSERT INTO don_vi_quan_doi 
                (ma_don_vi, ten_don_vi, ten_viet_tat, loai_don_vi, quan_binh_chung, don_vi_cap_tren,
                 quan_huyen, dia_chi, vi_do, kinh_do, x_vn2000, y_vn2000, chi_huy_truong, so_dien_thoai,
                 trang_thai_sscd, do_mat, geom_4326, geom_32648) 
                VALUES 
                (@ma, @ten, @ngan, @loai, @binhchung, @captren, @quanhuyen, @diachi, @vido, @kinhdo,
                 @x, @y, @chihuy, @sdt, @sscd, @domat,
                 ST_SetSRID(ST_MakePoint(@kinhdo, @vido), 4326),
                 ST_SetSRID(ST_MakePoint(@x, @y), 32648));";

            using (var conn = DbConnection.GetConnection())
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("ma", dv.MaDonVi);
                    cmd.Parameters.AddWithValue("ten", dv.TenDonVi);
                    cmd.Parameters.AddWithValue("ngan", dv.TenVietTat ?? "");
                    cmd.Parameters.AddWithValue("loai", dv.LoaiDonVi);
                    cmd.Parameters.AddWithValue("binhchung", dv.QuanBinhChung);
                    cmd.Parameters.AddWithValue("captren", dv.DonViCapTren ?? "Bộ Quốc phòng");
                    cmd.Parameters.AddWithValue("quanhuyen", dv.QuanHuyen);
                    cmd.Parameters.AddWithValue("diachi", dv.DiaChi);
                    cmd.Parameters.AddWithValue("vido", dv.ViDo);
                    cmd.Parameters.AddWithValue("kinhdo", dv.KinhDo);
                    cmd.Parameters.AddWithValue("x", dv.XVn2000);
                    cmd.Parameters.AddWithValue("y", dv.YVn2000);
                    cmd.Parameters.AddWithValue("chihuy", dv.ChiHuyTruong ?? "");
                    cmd.Parameters.AddWithValue("sdt", dv.SoDienThoai);
                    cmd.Parameters.AddWithValue("sscd", dv.TrangThaiSSCD);
                    cmd.Parameters.AddWithValue("domat", dv.DoMat);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
```
