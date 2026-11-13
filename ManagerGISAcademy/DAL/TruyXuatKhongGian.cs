
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Xml;
using ManagerGISAcademy.BLL;
using AdoConnection = System.Data.Common.DbConnection;
using AdoCommand = System.Data.Common.DbCommand;
using AdoDataReader = System.Data.Common.DbDataReader;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Truy vấn không gian trên bảng public.don_vi_quan_doi, gồm các cột
    /// geom_4326 và geom_32648 là Point với SRID tương ứng.
    ///
    /// KHÔNG phụ thuộc trực tiếp vào assembly System.Configuration hoặc
    /// namespace Npgsql ở thời điểm biên dịch. Lúc chạy vẫn cần Npgsql.dll,
    /// cấu hình kết nối PostgisHanoi và PostGIS trên máy chủ PostgreSQL.
    ///
    /// Dùng tài khoản CSDL hạn chế quyền và Row Level Security (nếu có).
    /// Không gọi phương thức DAL trực tiếp từ GUI để bỏ qua phân quyền BLL.
    /// Tương thích C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class TruyXuatKhongGian : ITruyXuatPhanTichKhongGian
    {
        private const string TenKetNoi = "PostgisHanoi";
        private const string TenBang = "public.don_vi_quan_doi";
        private const string CotKetQua = @"
            u.ma_don_vi, u.ten_don_vi, u.quan_binh_chung,
            u.kinh_do, u.vi_do, ST_AsText(u.geom_4326) AS wkt";

        private readonly string chuoiKetNoi;
        private readonly DbProviderFactory nhaCungCap;

        public TruyXuatKhongGian()
            : this(LayChuoiKetNoiMacDinh(), TaoNhaCungCapNpgsql())
        {
        }

        public TruyXuatKhongGian(string connectionString)
            : this(connectionString, TaoNhaCungCapNpgsql())
        {
        }

        /// <summary>
        /// Tiêm DbProviderFactory cho cấu hình hoặc kiểm thử riêng tầng DAL.
        /// </summary>
        public TruyXuatKhongGian(string connectionString, DbProviderFactory factory)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Chuỗi kết nối không được để trống.",
                    "connectionString");
            if (factory == null)
                throw new ArgumentNullException("factory");
            chuoiKetNoi = connectionString.Trim();
            nhaCungCap = factory;
        }

        /// <summary>
        /// ST_DWithin trên EPSG:32648 (đơn vị mét) sử dụng được GiST Index.
        /// Loại đơn vị trung tâm khỏi kết quả; kết quả sắp xếp gần đến xa.
        /// </summary>
        public IList<DonViTheoKhoangCach> TimDonViTrongBanKinh(
            string maDonViTam, double banKinhMet, int gioiHan)
        {
            string ma = KiemTraMa(maDonViTam);
            KiemTraBanKinh(banKinhMet);
            KiemTraGioiHan(gioiHan, SpatialAnalysisBLL.GioiHanTrongBanKinhToiDa);

            string sql = @"
                WITH tam AS (
                    SELECT geom_32648 AS geom
                    FROM " + TenBang + @"
                    WHERE ma_don_vi = @ma AND geom_32648 IS NOT NULL
                )
                SELECT " + CotKetQua + @",
                    ST_Distance(u.geom_32648, tam.geom) AS khoang_cach_met
                FROM " + TenBang + @" AS u CROSS JOIN tam
                WHERE u.ma_don_vi <> @ma
                  AND u.geom_32648 IS NOT NULL
                  AND ST_DWithin(u.geom_32648, tam.geom, @ban_kinh)
                ORDER BY khoang_cach_met, u.ma_don_vi
                LIMIT @gioi_han";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(lenh, "@ma", DbType.String, ma);
                ThemThamSo(lenh, "@ban_kinh", DbType.Double, banKinhMet);
                ThemThamSo(lenh, "@gioi_han", DbType.Int32, gioiHan);
                return DocDanhSach(lenh);
            }
        }

        /// <summary>
        /// K-nearest neighbor: phép toán <-> dùng chỉ mục GiST trên geom_32648.
        /// Điểm đầu vào là kinh độ/vĩ độ WGS84, được ST_Transform trong SQL.
        /// </summary>
        public IList<DonViTheoKhoangCach> TimKDonViGanNhat(
            double kinhDo, double viDo, int soLuong)
        {
            KiemTraToaDoWgs84(kinhDo, viDo);
            KiemTraGioiHan(soLuong, SpatialAnalysisBLL.SoLuongGanNhatToiDa);

            string sql = @"
                WITH diem AS (
                    SELECT ST_Transform(
                        ST_SetSRID(ST_MakePoint(@kinh_do, @vi_do), 4326),
                        32648) AS geom
                )
                SELECT " + CotKetQua + @",
                    ST_Distance(u.geom_32648, diem.geom) AS khoang_cach_met
                FROM " + TenBang + @" AS u CROSS JOIN diem
                WHERE u.geom_32648 IS NOT NULL
                ORDER BY u.geom_32648 <-> diem.geom, u.ma_don_vi
                LIMIT @so_luong";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(lenh, "@kinh_do", DbType.Double, kinhDo);
                ThemThamSo(lenh, "@vi_do", DbType.Double, viDo);
                ThemThamSo(lenh, "@so_luong", DbType.Int32, soLuong);
                return DocDanhSach(lenh);
            }
        }

        /// <summary>
        /// Tính khoảng cách phẳng theo mét. Nếu thiếu một trong hai đơn vị
        /// hoặc thiếu hình học, trả về null (không tự biến thành 0 mét).
        /// </summary>
        public double? TinhKhoangCachHaiDonVi(string maDonViA, string maDonViB)
        {
            string a = KiemTraMa(maDonViA);
            string b = KiemTraMa(maDonViB);
            string sql = @"
                SELECT ST_Distance(a.geom_32648, b.geom_32648)
                FROM " + TenBang + @" AS a
                JOIN " + TenBang + @" AS b ON b.ma_don_vi = @ma_b
                WHERE a.ma_don_vi = @ma_a
                  AND a.geom_32648 IS NOT NULL
                  AND b.geom_32648 IS NOT NULL
                LIMIT 1";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(lenh, "@ma_a", DbType.String, a);
                ThemThamSo(lenh, "@ma_b", DbType.String, b);
                object giaTri = lenh.ExecuteScalar();
                if (giaTri == null || giaTri == DBNull.Value)
                    return null;
                return Convert.ToDouble(giaTri);
            }
        }

        /// <summary>
        /// Tạo WKT polygon vùng đệm: Buffer trong CRS mét EPSG:32648;
        /// chuyển polygon sang EPSG:4326 để truyền tới MapWinGIS.
        /// Không sửa geometry trong CSDL.
        /// </summary>
        public string LayWktVungDem(string maDonViTam, double banKinhMet)
        {
            string ma = KiemTraMa(maDonViTam);
            KiemTraBanKinh(banKinhMet);
            string sql = @"
                SELECT ST_AsText(ST_Transform(
                    ST_Buffer(u.geom_32648, @ban_kinh, 8), 4326))
                FROM " + TenBang + @" AS u
                WHERE u.ma_don_vi = @ma
                  AND u.geom_32648 IS NOT NULL
                LIMIT 1";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(lenh, "@ma", DbType.String, ma);
                ThemThamSo(lenh, "@ban_kinh", DbType.Double, banKinhMet);
                object giaTri = lenh.ExecuteScalar();
                return giaTri == null || giaTri == DBNull.Value
                    ? null : Convert.ToString(giaTri);
            }
        }

        private static IList<DonViTheoKhoangCach> DocDanhSach(AdoCommand lenh)
        {
            List<DonViTheoKhoangCach> danhSach = new List<DonViTheoKhoangCach>();
            using (AdoDataReader doc = lenh.ExecuteReader())
            {
                int ma = doc.GetOrdinal("ma_don_vi");
                int ten = doc.GetOrdinal("ten_don_vi");
                int quanBinhChung = doc.GetOrdinal("quan_binh_chung");
                int kinhDo = doc.GetOrdinal("kinh_do");
                int viDo = doc.GetOrdinal("vi_do");
                int wkt = doc.GetOrdinal("wkt");
                int khoangCach = doc.GetOrdinal("khoang_cach_met");

                while (doc.Read())
                {
                    danhSach.Add(new DonViTheoKhoangCach
                    {
                        MaDonVi = LayChuoi(doc, ma),
                        TenDonVi = LayChuoi(doc, ten),
                        QuanBinhChung = LayChuoi(doc, quanBinhChung),
                        KinhDo = LaySo(doc, kinhDo),
                        ViDo = LaySo(doc, viDo),
                        Wkt4326 = LayChuoi(doc, wkt),
                        KhoangCachMet = LaySo(doc, khoangCach)
                    });
                }
            }
            return danhSach;
        }

        private static string LayChuoi(AdoDataReader doc, int chiSo)
        {
            return doc.IsDBNull(chiSo) ? null : Convert.ToString(doc.GetValue(chiSo));
        }

        private static double LaySo(AdoDataReader doc, int chiSo)
        {
            return doc.IsDBNull(chiSo) ? 0.0 : Convert.ToDouble(doc.GetValue(chiSo));
        }

        private static AdoCommand TaoLenh(AdoConnection ketNoi, string sql)
        {
            AdoCommand lenh = ketNoi.CreateCommand();
            if (lenh == null)
                throw new InvalidOperationException("Không tạo được lệnh truy vấn PostgreSQL.");
            lenh.CommandText = sql;
            lenh.CommandType = CommandType.Text;
            lenh.CommandTimeout = 30;
            return lenh;
        }

        private AdoConnection MoKetNoi()
        {
            AdoConnection ketNoi = nhaCungCap.CreateConnection();
            if (ketNoi == null)
                throw new InvalidOperationException("Npgsql không thể tạo kết nối PostgreSQL.");
            try
            {
                ketNoi.ConnectionString = chuoiKetNoi;
                ketNoi.Open();
                KetNoiCoSoDuLieu.ApDungVaiTro(ketNoi);
                return ketNoi;
            }
            catch
            {
                ketNoi.Dispose();
                throw;
            }
        }

        private static void ThemThamSo(AdoCommand lenh, string ten,
            DbType kieu, object giaTri)
        {
            DbParameter thamSo = lenh.CreateParameter();
            if (thamSo == null)
                throw new InvalidOperationException("Không tạo được tham số SQL: " + ten);
            thamSo.ParameterName = ten;
            thamSo.DbType = kieu;
            thamSo.Value = giaTri ?? DBNull.Value;
            lenh.Parameters.Add(thamSo);
        }

        /// <summary>
        /// Dùng chung một nguồn chuỗi kết nối với KetNoiCoSoDuLieu:
        /// ưu tiên biến môi trường MANAGERGISACADEMY_POSTGIS, sau đó App.config,
        /// và chặn mật khẩu mẫu. Không tự đọc App.config riêng ở đây nữa.
        /// </summary>
        private static string LayChuoiKetNoiMacDinh()
        {
            return KetNoiCoSoDuLieu.LayChuoiKetNoi();
        }

        private static DbProviderFactory TaoNhaCungCapNpgsql()
        {
            Type kieu = Type.GetType("Npgsql.NpgsqlFactory, Npgsql", false);
            if (kieu != null)
            {
                FieldInfo truong = kieu.GetField("Instance",
                    BindingFlags.Public | BindingFlags.Static);
                object instance = truong == null ? null : truong.GetValue(null);
                if (instance == null)
                {
                    PropertyInfo thuocTinh = kieu.GetProperty("Instance",
                        BindingFlags.Public | BindingFlags.Static);
                    if (thuocTinh != null)
                        instance = thuocTinh.GetValue(null, null);
                }
                DbProviderFactory factory = instance as DbProviderFactory;
                if (factory != null)
                    return factory;
            }
            throw new InvalidOperationException(
                "Không tìm thấy NpgsqlFactory. Hãy cài NuGet Npgsql tương thích .NET Framework 4.8.");
        }

        private static string KiemTraMa(string maDonVi)
        {
            if (string.IsNullOrWhiteSpace(maDonVi))
                throw new ArgumentException("Mã đơn vị không được trống.", "maDonVi");
            string ma = maDonVi.Trim();
            if (ma.Length > 50)
                throw new ArgumentOutOfRangeException("maDonVi", "Mã đơn vị tối đa 50 ký tự.");
            return ma;
        }

        private static void KiemTraBanKinh(double banKinhMet)
        {
            if (double.IsNaN(banKinhMet) || double.IsInfinity(banKinhMet) ||
                banKinhMet <= 0 || banKinhMet > SpatialAnalysisBLL.BanKinhToiDaMet)
                throw new ArgumentOutOfRangeException("banKinhMet",
                    "Bán kính phải lớn hơn 0 và không vượt quá 35.000 mét.");
        }

        private static void KiemTraGioiHan(int giaTri, int toiDa)
        {
            if (giaTri < 1 || giaTri > toiDa)
                throw new ArgumentOutOfRangeException("giaTri",
                    "Số kết quả không được nhỏ hơn 1 hoặc lớn hơn " + toiDa + ".");
        }

        private static void KiemTraToaDoWgs84(double kinhDo, double viDo)
        {
            if (double.IsNaN(kinhDo) || double.IsInfinity(kinhDo) ||
                double.IsNaN(viDo) || double.IsInfinity(viDo) ||
                kinhDo < 102 || kinhDo > 108 || viDo < 0 || viDo > 84)
                throw new ArgumentOutOfRangeException("kinhDo",
                    "Tọa độ ngoài phạm vi UTM 48 Bắc.");
        }
    }
}
