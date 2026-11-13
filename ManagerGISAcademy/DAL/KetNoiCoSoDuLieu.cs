using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Xml;

using AdoConnection = System.Data.Common.DbConnection;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Quan ly ket noi PostgreSQL/PostGIS.
    /// C# 7.3 / .NET Framework 4.8 / Npgsql 4.1.14.
    ///
    /// PostgisHanoi:
    ///   Ket noi ho so GIS hien tai.
    ///
    /// PostgisDanhMucHaNoi:
    ///   Ket noi doc danh muc BCHQS trong HQTCSDL.
    ///
    /// Khong thay doi cac API dang duoc DAL cu su dung.
    /// </summary>
    public sealed class KetNoiCoSoDuLieu
    {
        private const string TenKetNoi = "PostgisHanoi";

        private const string BienMoiTruong =
            "MANAGERGISACADEMY_POSTGIS";

        private const string TenKetNoiDanhMuc =
            "PostgisDanhMucHaNoi";

        private const string BienMoiTruongDanhMuc =
            "MANAGERGISACADEMY_POSTGIS_DANHMUC";

        private const string DatabaseDanhMuc =
            "HQTCSDL";

        // CSDL ma ung dung doc/ghi ho so (tu giai doan 3).
        private const string TenCsdlUngDung =
            "HQTCSDL";

        // ==========================================
        // 0. DOC BIEN MOI TRUONG
        //
        // Uu tien bien cua TIEN TRINH. Neu khong co, doc thang
        // bien cap NGUOI DUNG (HKCU\Environment).
        //
        // Ly do: Windows chi truyen bien moi cho chuong trinh
        // mo SAU khi dat bien. Visual Studio mo tu truoc se
        // chay ung dung voi bo bien cu -> ung dung roi ve
        // App.config (mat khau mau) va bao loi.
        // ==========================================

        private static string DocBienMoiTruong(string ten)
        {
            string giaTri =
                Environment.GetEnvironmentVariable(ten);

            if (!string.IsNullOrWhiteSpace(giaTri))
                return giaTri;

            try
            {
                return Environment.GetEnvironmentVariable(
                    ten,
                    EnvironmentVariableTarget.User);
            }
            catch (System.Security.SecurityException)
            {
                return null;
            }
        }

        // ==========================================
        // 1. NGUON CAU HINH KET NOI CHINH
        // ==========================================

        public static string LayNguonCauHinh()
        {
            string giaTri =
                DocBienMoiTruong(
                    BienMoiTruong);

            return !string.IsNullOrWhiteSpace(giaTri)
                ? "biến môi trường " + BienMoiTruong
                : "App.config (" + TenKetNoi + ")";
        }

        // ==========================================
        // 2. KET NOI CHINH - GIU NGUYEN
        // ==========================================

        public static string LayChuoiKetNoi()
        {
            string chuoiMoiTruong =
                DocBienMoiTruong(
                    BienMoiTruong);

            if (!string.IsNullOrWhiteSpace(
                chuoiMoiTruong))
            {
                return chuoiMoiTruong.Trim();
            }

            string chuoi = DocChuoiTuAppConfig(
                TenKetNoi);

            if (string.IsNullOrWhiteSpace(chuoi))
            {
                throw new InvalidOperationException(
                    "Không tìm thấy chuỗi kết nối " +
                    TenKetNoi +
                    " trong App.config.");
            }

            KiemTraMatKhauMau(chuoi);

            return chuoi.Trim();
        }

        // ==========================================
        // 3. NGUON CAU HINH DANH MUC
        // ==========================================

        public static string LayNguonCauHinhDanhMuc()
        {
            string bienMoiTruong =
                DocBienMoiTruong(
                    BienMoiTruongDanhMuc);

            if (!string.IsNullOrWhiteSpace(
                bienMoiTruong))
            {
                return "biến môi trường " +
                       BienMoiTruongDanhMuc;
            }

            string cauHinh = DocChuoiTuAppConfig(
                TenKetNoiDanhMuc);

            if (!string.IsNullOrWhiteSpace(cauHinh))
            {
                return "App.config (" +
                       TenKetNoiDanhMuc + ")";
            }

            return "Kế thừa kết nối PostgisHanoi, " +
                   "chuyển database sang HQTCSDL";
        }

        // ==========================================
        // 4. CHUOI KET NOI DANH MUC HQTCSDL
        // ==========================================

        public static string LayChuoiKetNoiDanhMuc()
        {
            // Uu tien 1:
            // Bien moi truong rieng cho danh muc.

            string chuoi =
                DocBienMoiTruong(
                    BienMoiTruongDanhMuc);

            if (!string.IsNullOrWhiteSpace(chuoi))
            {
                KiemTraMatKhauMau(chuoi);
                return chuoi.Trim();
            }

            // Uu tien 2:
            // Connection string rieng trong App.config.

            chuoi = DocChuoiTuAppConfig(
                TenKetNoiDanhMuc);

            if (!string.IsNullOrWhiteSpace(chuoi))
            {
                KiemTraMatKhauMau(chuoi);
                return chuoi.Trim();
            }

            // Uu tien 3:
            // Dung cung host, port, username/password
            // voi PostgisHanoi, chi doi database.
            //
            // Cach nay chi thanh cong khi tai khoan
            // hien tai duoc cap quyen vao HQTCSDL.

            DbConnectionStringBuilder builder =
                new DbConnectionStringBuilder();

            builder.ConnectionString =
                LayChuoiKetNoi();

            builder["Database"] =
                DatabaseDanhMuc;

            return builder.ConnectionString;
        }

        // ==========================================
        // 5. DOC APP.CONFIG AN TOAN
        // ==========================================

        private static string DocChuoiTuAppConfig(
            string tenKetNoi)
        {
            string duongDan =
                AppDomain.CurrentDomain
                    .SetupInformation
                    .ConfigurationFile;

            if (string.IsNullOrWhiteSpace(duongDan) ||
                !File.Exists(duongDan))
            {
                return null;
            }

            XmlReaderSettings caiDat =
                new XmlReaderSettings
                {
                    DtdProcessing =
                        DtdProcessing.Prohibit,

                    XmlResolver = null
                };

            XmlDocument xml =
                new XmlDocument
                {
                    XmlResolver = null
                };

            using (XmlReader boDoc =
                XmlReader.Create(
                    duongDan,
                    caiDat))
            {
                xml.Load(boDoc);
            }

            XmlNode node = xml.SelectSingleNode(
                "/configuration/connectionStrings/add[@name='" +
                tenKetNoi +
                "']");

            if (node == null ||
                node.Attributes == null ||
                node.Attributes["connectionString"] == null)
            {
                return null;
            }

            return node.Attributes[
                "connectionString"].Value;
        }

        private static void KiemTraMatKhauMau(
            string chuoi)
        {
            if (string.IsNullOrWhiteSpace(chuoi))
            {
                throw new InvalidOperationException(
                    "Chuỗi kết nối PostgreSQL đang trống.");
            }

            if (chuoi.IndexOf(
                "THAY_MAT_KHAU_MOI_TREN_MAY",
                StringComparison.OrdinalIgnoreCase) >= 0)
            {
                throw new InvalidOperationException(
                    "App.config vẫn còn mật khẩu mẫu. " +
                    "Cần thiết lập mật khẩu thực tế.");
            }
        }

        // ==========================================
        // 6. NPGSQL FACTORY
        // ==========================================

        public static DbProviderFactory
            LayNhaCungCapNpgsql()
        {
            Type kieu;

            try
            {
                kieu = Type.GetType(
                    "Npgsql.NpgsqlFactory, Npgsql",
                    false);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "Không nạp được Npgsql. " +
                    "Kiểm tra NuGet Npgsql và " +
                    "các thư viện phụ thuộc.",
                    ex);
            }

            if (kieu == null)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy Npgsql.dll. " +
                    "Kiểm tra References/Npgsql " +
                    "và thư mục bin.");
            }

            object giaTri = null;

            FieldInfo truong = kieu.GetField(
                "Instance",
                BindingFlags.Public |
                BindingFlags.Static);

            if (truong != null)
            {
                giaTri = truong.GetValue(null);
            }

            if (giaTri == null)
            {
                PropertyInfo thuocTinh =
                    kieu.GetProperty(
                        "Instance",
                        BindingFlags.Public |
                        BindingFlags.Static);

                if (thuocTinh != null)
                {
                    giaTri = thuocTinh.GetValue(
                        null,
                        null);
                }
            }

            DbProviderFactory ketQua =
                giaTri as DbProviderFactory;

            if (ketQua == null)
            {
                throw new InvalidOperationException(
                    "Không lấy được " +
                    "NpgsqlFactory.Instance. " +
                    "Kiểm tra phiên bản Npgsql.");
            }

            return ketQua;
        }

        // ==========================================
        // 7. TAO KET NOI CHUNG
        // ==========================================

        private static AdoConnection TaoKetNoi(
            string chuoiKetNoi)
        {
            DbProviderFactory nhaCungCap =
                LayNhaCungCapNpgsql();

            AdoConnection ketNoi =
                nhaCungCap.CreateConnection();

            if (ketNoi == null)
            {
                throw new InvalidOperationException(
                    "Npgsql không khởi tạo được " +
                    "đối tượng kết nối PostgreSQL.");
            }

            try
            {
                ketNoi.ConnectionString =
                    chuoiKetNoi;

                ketNoi.Open();

                ApDungVaiTro(ketNoi);

                return ketNoi;
            }
            catch
            {
                ketNoi.Dispose();
                throw;
            }
        }

        // ==========================================
        // 7b. VAI TRO PHIEN LAM VIEC (SET ROLE + RLS)
        // ==========================================

        // Chi chap nhan dung 3 vai tro do script 85 tao (chong SQL injection).
        private static readonly string[] VaiTroHopLe =
            { "gis_quan_tri", "gis_bien_tap", "gis_xem" };

        private static volatile string vaiTroPhien;

        /// <summary>
        /// Vai tro PostgreSQL cho moi ket noi moi (null = vai tro goc cua tai
        /// khoan ung dung). PostgreSQL tu loc dong (RLS) va kiem tra GRANT theo
        /// vai tro nay; giao dien doc lai quyen tu has_table_privilege.
        /// </summary>
        public static string VaiTroPhien
        {
            get { return vaiTroPhien; }
            set
            {
                if (value != null && Array.IndexOf(VaiTroHopLe, value) < 0)
                    throw new ArgumentException("Vai trò không hợp lệ: " + value);
                vaiTroPhien = value;
            }
        }

        private static volatile string taiKhoanPhien;

        /// <summary>
        /// Ten dang nhap cua nguoi dung (null = khach). Gan vao phien PostgreSQL
        /// (app.tai_khoan + application_name) de nhat ky / hang doi duyet ghi duoc nguoi thao tac.
        /// </summary>
        public static string TaiKhoanPhien
        {
            get { return taiKhoanPhien; }
            set
            {
                if (value != null && !System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-z0-9._]{4,40}$"))
                    throw new ArgumentException("Tên đăng nhập không hợp lệ.");
                taiKhoanPhien = value;
            }
        }

        /// <summary>Gan vai tro + tai khoan phien cho ket noi vua mo (luon SET hoac RESET de khong dinh phien cu trong pool).</summary>
        public static void ApDungVaiTro(DbConnection ketNoi)
        {
            if (ketNoi == null || ketNoi.State != ConnectionState.Open) return;
            string vt = vaiTroPhien;
            string tk = taiKhoanPhien;
            using (DbCommand lenh = ketNoi.CreateCommand())
            {
                lenh.CommandText = vt == null ? "RESET ROLE" : "SET ROLE " + vt;
                lenh.CommandType = CommandType.Text;
                lenh.CommandTimeout = 10;
                lenh.ExecuteNonQuery();
            }
            using (DbCommand lenh = ketNoi.CreateCommand())
            {
                lenh.CommandText = "SELECT set_config('app.tai_khoan', @tk, false), set_config('application_name', @ud, false)";
                lenh.CommandType = CommandType.Text;
                lenh.CommandTimeout = 10;
                DbParameter p1 = lenh.CreateParameter(); p1.ParameterName = "@tk"; p1.DbType = DbType.String; p1.Value = tk ?? ""; lenh.Parameters.Add(p1);
                DbParameter p2 = lenh.CreateParameter(); p2.ParameterName = "@ud"; p2.DbType = DbType.String;
                p2.Value = tk == null ? "ManagerGISAcademy" : "ManagerGISAcademy:" + tk; lenh.Parameters.Add(p2);
                lenh.ExecuteNonQuery();
            }
        }

        // ==========================================
        // 8. MO KET NOI HO SO CHINH
        // ==========================================

        public static AdoConnection MoKetNoi()
        {
            return TaoKetNoi(
                LayChuoiKetNoi());
        }

        // ==========================================
        // 9. MO KET NOI HQTCSDL
        // ==========================================

        public static AdoConnection MoKetNoiDanhMuc()
        {
            AdoConnection ketNoi =
                TaoKetNoi(
                    LayChuoiKetNoiDanhMuc());

            // Bao ve viec doc nham database.
            // Khong can kiem tra tai khoan,
            // vi tai khoan co the duoc cap quyen
            // rieng cho danh muc.

            if (!string.Equals(
                ketNoi.Database,
                DatabaseDanhMuc,
                StringComparison.OrdinalIgnoreCase))
            {
                string databaseThucTe =
                    ketNoi.Database;

                ketNoi.Dispose();

                throw new InvalidOperationException(
                    "Kết nối danh mục đang trỏ tới " +
                    "database '" + databaseThucTe +
                    "', trong khi yêu cầu HQTCSDL. " +
                    "Kiểm tra PostgisDanhMucHaNoi.");
            }

            return ketNoi;
        }

        // ==========================================
        // 10. KIEM TRA KET NOI CHINH
        // ==========================================

        public static bool ThuKetNoi(
            out string thongBao)
        {
            try
            {
                using (AdoConnection ketNoi =
                    MoKetNoi())
                using (DbCommand lenh =
                    ketNoi.CreateCommand())
                {
                    lenh.CommandText = @"
                        SELECT
                            current_database(),
                            current_user,
                            inet_server_port(),
                            PostGIS_Version(),
                            (
                                SELECT COUNT(*)
                                FROM public.don_vi_quan_doi
                            )";

                    lenh.CommandType =
                        CommandType.Text;

                    lenh.CommandTimeout = 15;

                    using (DbDataReader doc =
                        lenh.ExecuteReader())
                    {
                        if (!doc.Read())
                        {
                            throw new InvalidOperationException(
                                "PostgreSQL không trả về " +
                                "kết quả kiểm tra.");
                        }

                        string tenCSDL =
                            Convert.ToString(
                                doc.GetValue(0));

                        string taiKhoan =
                            Convert.ToString(
                                doc.GetValue(1));

                        int cong =
                            Convert.ToInt32(
                                doc.GetValue(2));

                        string postgis =
                            Convert.ToString(
                                doc.GetValue(3));

                        long soDonVi =
                            Convert.ToInt64(
                                doc.GetValue(4));

                        // Giai doan 3: CSDL ung dung la HQTCSDL
                        // (hanoi_military_gis chi con luu tru).
                        if (!string.Equals(
                                tenCSDL,
                                TenCsdlUngDung,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            !string.Equals(
                                taiKhoan,
                                "managergis_app",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            cong != 5433)
                        {
                            thongBao =
                                "Đã kết nối PostgreSQL nhưng " +
                                "không đúng máy chủ ứng dụng. " +
                                "Nhận được: CSDL=" +
                                tenCSDL +
                                "; tài khoản=" +
                                taiKhoan +
                                "; cổng=" +
                                cong +
                                ". Cần: " + TenCsdlUngDung + " / " +
                                "managergis_app / 5433. " +
                                "Nguồn: " +
                                LayNguonCauHinh() +
                                ".";

                            return false;
                        }

                        thongBao =
                            "Kết nối thành công. " +
                            "CSDL: " + tenCSDL +
                            "; tài khoản: " + taiKhoan +
                            "; cổng: " + cong +
                            "; PostGIS: " + postgis +
                            "; tổng số đơn vị: " +
                            soDonVi +
                            ". Nguồn: " +
                            LayNguonCauHinh() +
                            ".";

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception goc = TimLoiGoc(ex);

                thongBao =
                    "Không kết nối/kiểm tra được " +
                    "PostgreSQL/PostGIS. Nguồn: " +
                    LayNguonCauHinh() +
                    ". " +
                    goc.GetType().Name +
                    ": " +
                    goc.Message;

                return false;
            }
        }

        // ==========================================
        // 11. KIEM TRA KET NOI DANH MUC HQTCSDL
        // ==========================================

        public static bool ThuKetNoiDanhMuc(
            out string thongBao)
        {
            try
            {
                using (AdoConnection ketNoi =
                    MoKetNoiDanhMuc())
                using (DbCommand lenh =
                    ketNoi.CreateCommand())
                {
                    lenh.CommandText = @"
                        SELECT
                            current_database(),
                            current_user,
                            inet_server_port(),
                            to_regclass(
                                'ql_don_vi.v_bchqs_cap_xa_2026'
                            )::text,
                            (
                                SELECT COUNT(*)
                                FROM ql_don_vi.v_bchqs_cap_xa_2026
                            )";

                    lenh.CommandType =
                        CommandType.Text;

                    lenh.CommandTimeout = 15;

                    using (DbDataReader doc =
                        lenh.ExecuteReader())
                    {
                        if (!doc.Read())
                        {
                            throw new InvalidOperationException(
                                "Không nhận được kết quả " +
                                "kiểm tra danh mục.");
                        }

                        string database =
                            Convert.ToString(
                                doc.GetValue(0));

                        string taiKhoan =
                            Convert.ToString(
                                doc.GetValue(1));

                        int cong =
                            Convert.ToInt32(
                                doc.GetValue(2));

                        string view =
                            doc.IsDBNull(3)
                                ? null
                                : Convert.ToString(
                                    doc.GetValue(3));

                        long soBanGhi =
                            Convert.ToInt64(
                                doc.GetValue(4));

                        if (string.IsNullOrWhiteSpace(
                            view))
                        {
                            thongBao =
                                "Đã kết nối HQTCSDL nhưng " +
                                "không tìm thấy View Ban CHQS.";

                            return false;
                        }

                        thongBao =
                            "Kết nối danh mục thành công. " +
                            "Database=" + database +
                            "; tài khoản=" + taiKhoan +
                            "; cổng=" + cong +
                            "; View=" + view +
                            "; số bản ghi=" + soBanGhi +
                            ". Nguồn: " +
                            LayNguonCauHinhDanhMuc() +
                            ".";

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Exception goc = TimLoiGoc(ex);

                thongBao =
                    "Không đọc được danh mục Ban CHQS " +
                    "trong HQTCSDL. Nguồn: " +
                    LayNguonCauHinhDanhMuc() +
                    ". " +
                    goc.GetType().Name +
                    ": " +
                    goc.Message;

                return false;
            }
        }

        private static Exception TimLoiGoc(
            Exception ex)
        {
            Exception goc = ex;
            int doSau = 0;

            while (goc.InnerException != null &&
                   doSau < 5)
            {
                goc = goc.InnerException;
                doSau++;
            }

            return goc;
        }
    }
}