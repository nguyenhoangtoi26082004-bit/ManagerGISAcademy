
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ManagerGISAcademy.DTO;
using AdoConnection = System.Data.Common.DbConnection;
using AdoCommand = System.Data.Common.DbCommand;
using AdoDataReader = System.Data.Common.DbDataReader;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Đọc danh sách địa bàn từ public.don_vi_quan_doi.quan_huyen.
    ///
    /// Bộ script CSDL đã chạy chỉ tạo bảng don_vi_quan_doi, chưa tạo
    /// bảng quan_huyen_hanoi. Không tự giả định bảng địa giới đã tồn tại.
    /// Khi có bảng Polygon/MultiPolygon, cần bổ sung chức năng đọc ranh giới
    /// bằng PostGIS và gắn đúng SRID; không tự tạo hình học từ tên địa bàn.
    ///
    /// Dùng DbConnection.MoKetNoi() đã có trong dự án, không tạo thêm
    /// tên kết nối mới hoặc dùng trực tiếp System.Configuration.
    /// Các câu lệnh SELECT là chỉ đọc và có tham số.
    ///
    /// Chỉ gọi qua BLL và áp dụng quyền truy cập ở CSDL (RLS/quyền SELECT).
    /// DAL không tự cấp quyền hay suy đoán quyền của người đang đăng nhập.
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class TruyXuatQuanHuyen
    {
        private const string BangDonVi = "public.don_vi_quan_doi";

        private const string DanhSachCoBan = @"
            SELECT MIN(BTRIM(u.quan_huyen)) AS ten_quan_huyen,
                   COUNT(*)::bigint AS so_luong_don_vi
            FROM public.don_vi_quan_doi AS u
            WHERE u.quan_huyen IS NOT NULL
              AND BTRIM(u.quan_huyen) <> ''";

        private readonly Func<AdoConnection> taoKetNoi;

        /// <summary>
        /// Dùng kết nối thực tế từ DAL/DbConnection.cs.
        /// Chuỗi kết nối PostgisHanoi sử dụng cổng do App.config khai báo.
        /// </summary>
        public TruyXuatQuanHuyen()
            : this(ManagerGISAcademy.DAL.KetNoiCoSoDuLieu.MoKetNoi)
        {
        }

        /// <summary>
        /// Cho phép cung cấp một hàm mở kết nối khi kiểm thử.
        /// Hàm phải trả kết nối đang mở; bên gọi DAL sẽ tự Dispose kết nối.
        /// </summary>
        public TruyXuatQuanHuyen(Func<AdoConnection> hamMoKetNoi)
        {
            if (hamMoKetNoi == null)
                throw new ArgumentNullException("hamMoKetNoi");

            taoKetNoi = hamMoKetNoi;
        }

        // ==========================================================
        // 1. LẤY DANH SÁCH QUẬN/HUYỆN
        // ==========================================================

        /// <summary>
        /// Lấy các tên quận/huyện khác nhau đang có trong dữ liệu đơn vị.
        /// Chưa có dữ liệu đơn vị thì trả về danh sách rỗng, không phải lỗi.
        /// Tên khác nhau chỉ bởi chữ hoa/thường sẽ được gom vào một nhóm.
        /// </summary>
        public List<QuanHuyen> LayTatCaQuanHuyen()
        {
            string sql = DanhSachCoBan + @"
                GROUP BY LOWER(BTRIM(u.quan_huyen))
                ORDER BY ten_quan_huyen";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                return DocDanhSach(lenh);
            }
        }

        // ==========================================================
        // 2. TÌM KIẾM QUẬN/HUYỆN
        // ==========================================================

        /// <summary>
        /// Tìm tên địa bàn theo chuỗi con, không phân biệt chữ hoa/thường.
        /// SQL không có tính năng bỏ dấu tiếng Việt; chức năng đó nếu cần
        /// sẽ được bổ sung ở BLL hoặc bằng chỉ mục/hàm CSDL phù hợp.
        /// </summary>
        public List<QuanHuyen> TimKiemQuanHuyen(
            string tuKhoa,
            int gioiHan = 200)
        {
            if (gioiHan < 1 || gioiHan > 1000)
                throw new ArgumentOutOfRangeException(
                    "gioiHan", "Giới hạn phải từ 1 đến 1000.");

            string sql = DanhSachCoBan + @"
                AND POSITION(LOWER(@tu_khoa)
                    IN LOWER(BTRIM(u.quan_huyen))) > 0
                GROUP BY LOWER(BTRIM(u.quan_huyen))
                ORDER BY ten_quan_huyen
                LIMIT @gioi_han";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(
                    lenh,
                    "@tu_khoa",
                    DbType.String,
                    (tuKhoa ?? string.Empty).Trim());

                ThemThamSo(
                    lenh,
                    "@gioi_han",
                    DbType.Int32,
                    gioiHan);

                return DocDanhSach(lenh);
            }
        }

        // ==========================================================
        // 3. LẤY QUẬN/HUYỆN THEO TÊN
        // ==========================================================

        /// <summary>
        /// Lấy thông tin tổng hợp của một địa bàn theo tên chính xác,
        /// không phân biệt chữ hoa/thường và khoảng trắng đầu/cuối.
        /// Trả null nếu chưa có bản ghi nào ở địa bàn đó.
        /// </summary>
        public QuanHuyen LayTheoTen(string tenQuanHuyen)
        {
            string ten = KiemTraTenQuanHuyen(tenQuanHuyen);

            string sql = DanhSachCoBan + @"
                AND LOWER(BTRIM(u.quan_huyen)) = LOWER(@ten)
                GROUP BY LOWER(BTRIM(u.quan_huyen))
                LIMIT 1";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(
                    lenh,
                    "@ten",
                    DbType.String,
                    ten);

                using (AdoDataReader doc = lenh.ExecuteReader())
                {
                    return doc.Read()
                        ? DocQuanHuyen(doc)
                        : null;
                }
            }
        }

        // ==========================================================
        // 4. ĐẾM BẢN GHI THEO QUẬN/HUYỆN
        // ==========================================================

        /// <summary>
        /// Đếm bản ghi tại một địa bàn theo tên.
        /// Trả về 0 nếu địa bàn chưa có bản ghi.
        /// Số lượng phụ thuộc quyền xem của tài khoản PostgreSQL hiện tại.
        /// </summary>
        public long DemDonViTheoQuanHuyen(string tenQuanHuyen)
        {
            string ten = KiemTraTenQuanHuyen(tenQuanHuyen);

            string sql = @"
                SELECT COUNT(*)::bigint
                FROM " + BangDonVi + @" AS u
                WHERE u.quan_huyen IS NOT NULL
                  AND LOWER(BTRIM(u.quan_huyen)) = LOWER(@ten)";

            using (AdoConnection ketNoi = MoKetNoi())
            using (AdoCommand lenh = TaoLenh(ketNoi, sql))
            {
                ThemThamSo(
                    lenh,
                    "@ten",
                    DbType.String,
                    ten);

                return Convert.ToInt64(lenh.ExecuteScalar());
            }
        }

        // ==========================================================
        // 5. MỞ KẾT NỐI POSTGRESQL
        // ==========================================================

        private AdoConnection MoKetNoi()
        {
            AdoConnection ketNoi = taoKetNoi();

            if (ketNoi == null)
                throw new InvalidOperationException(
                    "Hàm mở kết nối đã trả về null.");

            if (ketNoi.State != ConnectionState.Open)
            {
                ketNoi.Dispose();

                throw new InvalidOperationException(
                    "Kết nối được truyền vào QuanHuyenDAL chưa mở.");
            }

            return ketNoi;
        }

        // ==========================================================
        // 6. TẠO LỆNH SQL
        // ==========================================================

        private static AdoCommand TaoLenh(
            AdoConnection ketNoi,
            string sql)
        {
            AdoCommand lenh = ketNoi.CreateCommand();

            if (lenh == null)
                throw new InvalidOperationException(
                    "Không tạo được lệnh PostgreSQL.");

            lenh.CommandText = sql;
            lenh.CommandType = CommandType.Text;
            lenh.CommandTimeout = 30;

            return lenh;
        }

        // ==========================================================
        // 7. THÊM THAM SỐ SQL
        // ==========================================================

        private static void ThemThamSo(
            AdoCommand lenh,
            string ten,
            DbType kieu,
            object giaTri)
        {
            DbParameter thamSo = lenh.CreateParameter();

            if (thamSo == null)
                throw new InvalidOperationException(
                    "Không tạo được tham số SQL: " + ten);

            thamSo.ParameterName = ten;
            thamSo.DbType = kieu;
            thamSo.Value = giaTri ?? DBNull.Value;

            lenh.Parameters.Add(thamSo);
        }

        // ==========================================================
        // 8. ĐỌC DANH SÁCH KẾT QUẢ
        // ==========================================================

        private static List<QuanHuyen> DocDanhSach(AdoCommand lenh)
        {
            List<QuanHuyen> ketQua = new List<QuanHuyen>();

            using (AdoDataReader doc = lenh.ExecuteReader())
            {
                while (doc.Read())
                {
                    ketQua.Add(DocQuanHuyen(doc));
                }
            }

            return ketQua;
        }

        // ==========================================================
        // 9. CHUYỂN DỮ LIỆU TỪ POSTGRESQL SANG DTO
        // ==========================================================

        private static QuanHuyen DocQuanHuyen(AdoDataReader doc)
        {
            int cotTen = doc.GetOrdinal("ten_quan_huyen");
            int cotSoLuong = doc.GetOrdinal("so_luong_don_vi");

            return new QuanHuyen
            {
                TenQuanHuyen = doc.IsDBNull(cotTen)
                    ? null
                    : Convert.ToString(doc.GetValue(cotTen)),

                SoLuongDonVi = doc.IsDBNull(cotSoLuong)
                    ? 0L
                    : Convert.ToInt64(doc.GetValue(cotSoLuong))

                // CSDL hiện chưa có bảng địa giới; giữ null các thuộc tính:
                // MaQuanHuyen, TinhThanhPho, WktRanhGioi4326,
                // SridRanhGioi và DienTichKm2.
            };
        }

        // ==========================================================
        // 10. KIỂM TRA TÊN QUẬN/HUYỆN
        // ==========================================================

        private static string KiemTraTenQuanHuyen(
            string tenQuanHuyen)
        {
            if (string.IsNullOrWhiteSpace(tenQuanHuyen))
            {
                throw new ArgumentException(
                    "Tên quận/huyện không được để trống.",
                    "tenQuanHuyen");
            }

            string ten = tenQuanHuyen.Trim();

            if (ten.Length > 100)
            {
                throw new ArgumentOutOfRangeException(
                    "tenQuanHuyen",
                    "Tên quận/huyện tối đa 100 ký tự.");
            }

            return ten;
        }
    }
}
