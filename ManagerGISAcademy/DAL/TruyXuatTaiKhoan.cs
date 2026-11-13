using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace ManagerGISAcademy.DAL
{
    // =====================================================
    // DTO
    // =====================================================

    /// <summary>Nguoi dung dang dang nhap (khong giu mat khau).</summary>
    public sealed class TaiKhoanDangNhap
    {
        public long Id { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string VaiTro { get; set; }
        public string DonViCongTac { get; set; }
        public DateTime? LanDangNhapTruoc { get; set; }
        public string TenVaiTro { get { return TruyXuatTaiKhoan.TenVaiTro(VaiTro); } }
        public bool LaQuanTri { get { return VaiTro == "gis_quan_tri"; } }

        /// <summary>Chu viet tat cho avatar: 2 chu cai dau cua ho + ten.</summary>
        public string ChuVietTat
        {
            get
            {
                string[] t = (HoTen ?? TenDangNhap ?? "?").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (t.Length == 0) return "?";
                string s = t.Length == 1 ? t[0].Substring(0, Math.Min(2, t[0].Length)) : t[0].Substring(0, 1) + t[t.Length - 1].Substring(0, 1);
                return s.ToUpper(CultureInfo.GetCultureInfo("vi-VN"));
            }
        }
    }

    public sealed class KetQuaTaiKhoan
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; }
        public string VaiTro { get; set; }
        public string TrangThai { get; set; }
        public TaiKhoanDangNhap TaiKhoan { get; set; }
    }

    /// <summary>Dong danh sach tai khoan cho man quan tri.</summary>
    public sealed class DongTaiKhoan
    {
        public long Id { get; set; }
        public string TenDangNhap { get; set; }
        public string HoTen { get; set; }
        public string DonViCongTac { get; set; }
        public string Email { get; set; }
        public string VaiTro { get; set; }
        public string TrangThai { get; set; }
        public string LyDoDangKy { get; set; }
        public DateTime NgayTao { get; set; }
        public DateTime? LanDangNhapCuoi { get; set; }
        public DateTime? KhoaDen { get; set; }
        public string NguoiDuyet { get; set; }

        public string TenVaiTro { get { return TruyXuatTaiKhoan.TenVaiTro(VaiTro); } }
        public string TrangThaiHienThi
        {
            get
            {
                if (TrangThai == "CHO_DUYET") return "Chờ duyệt";
                if (TrangThai == "KHOA") return "Đã khóa";
                return KhoaDen.HasValue ? "Khóa tạm" : "Hoạt động";
            }
        }
        public string NgayTaoHienThi { get { return NgayTao.ToString("dd/MM/yyyy HH:mm"); } }
        public string DangNhapCuoiHienThi { get { return LanDangNhapCuoi.HasValue ? LanDangNhapCuoi.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa"; } }
    }

    public sealed class DongDangNhap
    {
        public DateTime ThoiDiem { get; set; }
        public string TenDangNhap { get; set; }
        public string KetQua { get; set; }
        public string GhiChu { get; set; }
        public string ThoiDiemHienThi { get { return ThoiDiem.ToString("dd/MM HH:mm:ss"); } }
        public string KetQuaHienThi
        {
            get
            {
                switch (KetQua)
                {
                    case "THANH_CONG": return "Đăng nhập";
                    case "DANG_XUAT": return "Đăng xuất";
                    case "SAI_MAT_KHAU": return "Sai mật khẩu";
                    case "SAI_TEN": return "Sai tên";
                    case "KHOA_TAM": return "Khóa tạm 15′";
                    case "DANG_KHOA_TAM": return "Đang khóa tạm";
                    case "DANG_KY": return "Đăng ký";
                    case "CHO_DUYET": return "Chưa được duyệt";
                    case "BI_KHOA": return "Tài khoản bị khóa";
                    case "DUYET_DANG_KY": return "Duyệt đăng ký";
                    case "TU_CHOI_DANG_KY": return "Từ chối đăng ký";
                    case "PHAN_QUYEN": return "Phân quyền";
                    case "DOI_MAT_KHAU": return "Đổi mật khẩu";
                    case "DAT_LAI_MAT_KHAU": return "Đặt lại mật khẩu";
                    default: return KetQua;
                }
            }
        }
    }

    // =====================================================
    // PHIEN DANG NHAP (toan ung dung)
    // =====================================================

    public static class PhienDangNhap
    {
        public static TaiKhoanDangNhap HienTai { get; private set; }
        public static bool DaDangNhap { get { return HienTai != null; } }
        /// <summary>CSDL da co script 89 (bang tai khoan + vai tro). false = chay nhu truoc, khong dang nhap.</summary>
        public static bool HeThongSanSang { get; set; }
        public static event Action DaThayDoi;

        public static void Gan(TaiKhoanDangNhap tk)
        {
            HienTai = tk;
            KetNoiCoSoDuLieu.TaiKhoanPhien = tk == null ? null : tk.TenDangNhap;
            KetNoiCoSoDuLieu.VaiTroPhien = tk == null ? (HeThongSanSang ? "gis_xem" : null) : tk.VaiTro;
            Action h = DaThayDoi;
            if (h != null) h();
        }
    }

    // =====================================================
    // DAL
    // =====================================================

    public sealed class TruyXuatTaiKhoan
    {
        public static string TenVaiTro(string vaiTro)
        {
            switch (vaiTro)
            {
                case "gis_quan_tri": return "Quản trị";
                case "gis_bien_tap": return "Biên tập";
                case "gis_xem": return "Chỉ xem";
                default: return string.IsNullOrEmpty(vaiTro) ? "Khách" : vaiTro;
            }
        }

        /// <summary>Kiem tra CSDL da chay script 89 (goi khi chua SET ROLE).</summary>
        public static bool KiemTraSanSang()
        {
            const string sql = @"SELECT to_regprocedure('ql_he_thong.fn_dang_nhap(text,text)') IS NOT NULL
                                    AND EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'gis_xem')
                                    AND pg_has_role(current_user, 'gis_xem', 'MEMBER')";
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
                return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public bool CoQuanTriHoatDong()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_co_quan_tri_hoat_dong()"))
                return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public KetQuaTaiKhoan DangKy(string ten, string hoTen, string matKhau, string donVi, string email, string lyDo)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ok, thong_bao, vai_tro, trang_thai FROM ql_he_thong.fn_dang_ky(@ten, @ho, @mk, @dv, @em, @ld)"))
            {
                ThamSo(cmd, "@ten", ten); ThamSo(cmd, "@ho", hoTen); ThamSo(cmd, "@mk", matKhau);
                ThamSo(cmd, "@dv", donVi); ThamSo(cmd, "@em", email); ThamSo(cmd, "@ld", lyDo);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return new KetQuaTaiKhoan { ThongBao = "Không nhận được kết quả từ CSDL." };
                    return new KetQuaTaiKhoan
                    {
                        ThanhCong = rd.GetBoolean(0),
                        ThongBao = Chuoi(rd, 1),
                        VaiTro = Chuoi(rd, 2),
                        TrangThai = Chuoi(rd, 3)
                    };
                }
            }
        }

        public KetQuaTaiKhoan DangNhap(string ten, string matKhau)
        {
            const string sql = @"SELECT ok, thong_bao, id, ten_dang_nhap, ho_ten, vai_tro, trang_thai, don_vi_cong_tac, lan_dang_nhap_truoc
                                 FROM ql_he_thong.fn_dang_nhap(@ten, @mk)";
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            {
                ThamSo(cmd, "@ten", ten); ThamSo(cmd, "@mk", matKhau);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return new KetQuaTaiKhoan { ThongBao = "Không nhận được kết quả từ CSDL." };
                    KetQuaTaiKhoan kq = new KetQuaTaiKhoan { ThanhCong = rd.GetBoolean(0), ThongBao = Chuoi(rd, 1) };
                    if (kq.ThanhCong)
                    {
                        kq.TaiKhoan = new TaiKhoanDangNhap
                        {
                            Id = Convert.ToInt64(rd.GetValue(2)),
                            TenDangNhap = Chuoi(rd, 3),
                            HoTen = Chuoi(rd, 4),
                            VaiTro = Chuoi(rd, 5),
                            DonViCongTac = Chuoi(rd, 7),
                            LanDangNhapTruoc = rd.IsDBNull(8) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(8))
                        };
                        kq.VaiTro = kq.TaiKhoan.VaiTro;
                    }
                    return kq;
                }
            }
        }

        public void DangXuat(string ten)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_dang_xuat(@ten)"))
            {
                ThamSo(cmd, "@ten", ten);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Tra ve "OK" hoac thong bao loi.</summary>
        public string DoiMatKhau(string ten, string cu, string moi)
        {
            return ChuoiKetQua("SELECT ql_he_thong.fn_doi_mat_khau(@a, @b, @c)", ten, cu, moi);
        }

        public List<DongTaiKhoan> LayDanhSach()
        {
            List<DongTaiKhoan> ds = new List<DongTaiKhoan>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT * FROM ql_he_thong.fn_ds_tai_khoan()"))
            using (DbDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    ds.Add(new DongTaiKhoan
                    {
                        Id = Convert.ToInt64(rd.GetValue(0)),
                        TenDangNhap = Chuoi(rd, 1),
                        HoTen = Chuoi(rd, 2),
                        DonViCongTac = Chuoi(rd, 3),
                        Email = Chuoi(rd, 4),
                        VaiTro = Chuoi(rd, 5),
                        TrangThai = Chuoi(rd, 6),
                        LyDoDangKy = Chuoi(rd, 7),
                        NgayTao = Convert.ToDateTime(rd.GetValue(8)),
                        LanDangNhapCuoi = rd.IsDBNull(9) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(9)),
                        KhoaDen = rd.IsDBNull(10) ? (DateTime?)null : Convert.ToDateTime(rd.GetValue(10)),
                        NguoiDuyet = Chuoi(rd, 11)
                    });
                }
            }
            return ds;
        }

        public string CapNhat(long id, string vaiTro, string trangThai)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_cap_nhat_tai_khoan(@id, @vt, @tt)"))
            {
                ThamSoSo(cmd, "@id", id); ThamSo(cmd, "@vt", vaiTro); ThamSo(cmd, "@tt", trangThai);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public string TuChoi(long id)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_tu_choi_dang_ky(@id)"))
            {
                ThamSoSo(cmd, "@id", id);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public string DatLaiMatKhau(long id, string moi)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_dat_lai_mat_khau(@id, @mk)"))
            {
                ThamSoSo(cmd, "@id", id); ThamSo(cmd, "@mk", moi);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public List<DongDangNhap> NhatKyDangNhap(int gioiHan)
        {
            List<DongDangNhap> ds = new List<DongDangNhap>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT * FROM ql_he_thong.fn_nhat_ky_dang_nhap(@n)"))
            {
                DbParameter p = cmd.CreateParameter(); p.ParameterName = "@n"; p.DbType = DbType.Int32; p.Value = gioiHan; cmd.Parameters.Add(p);
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        ds.Add(new DongDangNhap
                        {
                            ThoiDiem = Convert.ToDateTime(rd.GetValue(0)),
                            TenDangNhap = Chuoi(rd, 1),
                            KetQua = Chuoi(rd, 2),
                            GhiChu = Chuoi(rd, 3)
                        });
            }
            return ds;
        }

        // ---------- tien ich ----------

        private static string ChuoiKetQua(string sql, string a, string b, string c)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            {
                ThamSo(cmd, "@a", a); ThamSo(cmd, "@b", b); ThamSo(cmd, "@c", c);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private static DbCommand Lenh(DbConnection cn, string sql)
        {
            DbCommand cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 20;
            return cmd;
        }

        private static void ThamSo(DbCommand cmd, string ten, string giaTri)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = ten;
            p.DbType = DbType.String;
            p.Value = string.IsNullOrWhiteSpace(giaTri) ? (object)DBNull.Value : giaTri;
            cmd.Parameters.Add(p);
        }

        private static void ThamSoSo(DbCommand cmd, string ten, long giaTri)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = ten;
            p.DbType = DbType.Int64;
            p.Value = giaTri;
            cmd.Parameters.Add(p);
        }

        private static string Chuoi(DbDataReader rd, int i) { return rd.IsDBNull(i) ? "" : Convert.ToString(rd.GetValue(i)); }
    }
}
