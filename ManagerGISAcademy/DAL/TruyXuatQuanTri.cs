using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ManagerGISAcademy.DAL
{
    // =====================================================
    // DTO quan tri he thong
    // =====================================================

    public sealed class YeuCauThayDoi
    {
        public long Id { get; set; }
        public string Loai { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string DuLieuCu { get; set; }
        public string DuLieuMoi { get; set; }
        public string ThamSo { get; set; }
        public string NguoiGui { get; set; }
        public DateTime ThoiGianGui { get; set; }
        public string TrangThai { get; set; }
        public string NguoiDuyet { get; set; }
        public DateTime? ThoiGianDuyet { get; set; }
        public string GhiChuDuyet { get; set; }

        public string LoaiHienThi
        {
            get
            {
                switch (Loai)
                {
                    case "THEM": return "Thêm hồ sơ";
                    case "SUA": return "Sửa hồ sơ";
                    case "XOA": return "Xóa hồ sơ";
                    case "DINH_VI": return "Định vị lại";
                    default: return Loai;
                }
            }
        }

        public string TrangThaiHienThi
        {
            get
            {
                switch (TrangThai)
                {
                    case "CHO_DUYET": return "Chờ duyệt";
                    case "DA_DUYET": return "Đã duyệt";
                    case "TU_CHOI": return "Từ chối";
                    case "LOI": return "Lỗi áp dụng";
                    default: return TrangThai;
                }
            }
        }

        public string TaiKhoanGui { get; set; }
        public string NguoiGuiHienThi
        {
            get { return string.IsNullOrEmpty(TaiKhoanGui) ? TruyXuatQuanTri.TenVaiTro(NguoiGui) : TaiKhoanGui + " (" + TruyXuatQuanTri.TenVaiTro(NguoiGui) + ")"; }
        }
        public string ThoiGianGuiHienThi { get { return ThoiGianGui.ToString("dd/MM/yyyy HH:mm"); } }
    }

    public sealed class DongNhatKy
    {
        public string Nguon { get; set; }
        public long Id { get; set; }
        public DateTime ThoiDiem { get; set; }
        public string ThaoTac { get; set; }
        public string MaDonVi { get; set; }
        public string TaiKhoan { get; set; }
        public string UngDung { get; set; }
        public string Bang { get; set; }
        public string DuLieuCu { get; set; }
        public string DuLieuMoi { get; set; }
        public string ThoiDiemHienThi { get { return ThoiDiem.ToString("dd/MM/yyyy HH:mm:ss"); } }
    }

    public sealed class KetQuaKiemThu
    {
        public int Stt { get; set; }
        public string Nhom { get; set; }
        public string Ten { get; set; }
        public bool Dat { get; set; }
        public string ChiTiet { get; set; }
        public string KetQua { get { return Dat ? "Đạt" : "Không đạt"; } }
    }

    public sealed class KetQuaHieuNang
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public double MsCoChiMuc { get; set; }
        public double MsKhongChiMuc { get; set; }
        public bool DungChiMuc { get; set; }
        public string KeHoach { get; set; }
        public string NhanhHon
        {
            get
            {
                if (MsCoChiMuc <= 0) return "—";
                double k = MsKhongChiMuc / MsCoChiMuc;
                return k >= 1.05 ? "×" + k.ToString("0.0", CultureInfo.GetCultureInfo("vi-VN")) : "≈ như nhau";
            }
        }
    }

    public sealed class KichThuocBang
    {
        public string LuocDo { get; set; }
        public string TenBang { get; set; }
        public long SoDong { get; set; }
        public string DuLieu { get; set; }
        public string ChiMuc { get; set; }
        public string Tong { get; set; }
    }

    public sealed class ThongTinChiMuc
    {
        public string Bang { get; set; }
        public string TenChiMuc { get; set; }
        public string Kieu { get; set; }
        public string KichThuoc { get; set; }
        public long SoLanQuet { get; set; }
        public string DinhNghia { get; set; }
    }

    public sealed class TepSaoLuu
    {
        public string DuongDan { get; set; }
        public string Ten { get { return Path.GetFileName(DuongDan); } }
        public long KichThuocByte { get; set; }
        public DateTime ThoiGian { get; set; }
        public string KichThuoc { get { return (KichThuocByte / 1048576d).ToString("N2", CultureInfo.GetCultureInfo("vi-VN")) + " MB"; } }
        public string ThoiGianHienThi { get { return ThoiGian.ToString("dd/MM/yyyy HH:mm"); } }
    }

    // =====================================================
    // DAL
    // =====================================================

    public sealed class TruyXuatQuanTri
    {
        public static string TenVaiTro(string vaiTro)
        {
            switch (vaiTro)
            {
                case "gis_quan_tri": return "Quản trị";
                case "gis_bien_tap": return "Biên tập";
                case "gis_xem": return "Chỉ xem";
                case null: case "": return "—";
                default: return vaiTro + " (quản trị)";
            }
        }

        // ---------------- vai tro ----------------

        public string LayVaiTroHieuLuc()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_vai_tro_hien_hanh()"))
                return Convert.ToString(cmd.ExecuteScalar());
        }

        // ---------------- yeu cau duyet ----------------

        public List<YeuCauThayDoi> LayYeuCau(bool chiChoDuyet)
        {
            string sql = @"SELECT id, loai, ma_don_vi, ten_don_vi, du_lieu_cu::text, du_lieu_moi::text, tham_so::text,
                                  nguoi_gui, thoi_gian_gui, trang_thai, coalesce(tai_khoan_duyet, nguoi_duyet), thoi_gian_duyet, ghi_chu_duyet,
                                  tai_khoan_gui
                           FROM ql_he_thong.yeu_cau_thay_doi " +
                         (chiChoDuyet ? "WHERE trang_thai = 'CHO_DUYET' " : "") +
                         "ORDER BY (trang_thai = 'CHO_DUYET') DESC, thoi_gian_gui DESC LIMIT 500";
            List<YeuCauThayDoi> ds = new List<YeuCauThayDoi>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    ds.Add(new YeuCauThayDoi
                    {
                        Id = rd.GetInt64(0),
                        Loai = Chuoi(rd, 1),
                        MaDonVi = Chuoi(rd, 2),
                        TenDonVi = Chuoi(rd, 3),
                        DuLieuCu = Chuoi(rd, 4),
                        DuLieuMoi = Chuoi(rd, 5),
                        ThamSo = Chuoi(rd, 6),
                        NguoiGui = Chuoi(rd, 7),
                        ThoiGianGui = rd.GetDateTime(8),
                        TrangThai = Chuoi(rd, 9),
                        NguoiDuyet = Chuoi(rd, 10),
                        ThoiGianDuyet = rd.IsDBNull(11) ? (DateTime?)null : rd.GetDateTime(11),
                        GhiChuDuyet = Chuoi(rd, 12),
                        TaiKhoanGui = rd.FieldCount > 13 ? Chuoi(rd, 13) : ""
                    });
                }
            }
            return ds;
        }

        public int DemChoDuyet()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT count(*) FROM ql_he_thong.yeu_cau_thay_doi WHERE trang_thai = 'CHO_DUYET'"))
                return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public string Duyet(long id, string ghiChu)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_duyet_yeu_cau(@id, @gc)"))
            {
                ThamSo(cmd, "@id", DbType.Int64, id);
                ThamSo(cmd, "@gc", DbType.String, ghiChu);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        public string TuChoi(long id, string lyDo)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_tu_choi_yeu_cau(@id, @ld)"))
            {
                ThamSo(cmd, "@id", DbType.Int64, id);
                ThamSo(cmd, "@ld", DbType.String, lyDo);
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        // ---------------- nhat ky ----------------

        public List<DongNhatKy> LayNhatKy(int gioiHan)
        {
            const string sql = @"
                SELECT * FROM (
                    SELECT 'Hồ sơ (ứng dụng)' AS nguon, n.id, n.thoi_diem,
                           CASE n.thao_tac WHEN 'I' THEN 'Thêm' WHEN 'U' THEN 'Sửa' ELSE 'Xóa' END AS thao_tac,
                           n.ma_don_vi, n.tai_khoan_db, n.ung_dung, 'don_vi_quan_doi' AS bang,
                           n.du_lieu_cu::text, n.du_lieu_moi::text
                    FROM public.nhat_ky_don_vi_quan_doi n
                    UNION ALL
                    SELECT 'Bảng gốc (trigger)', k.id, k.thoi_diem,
                           CASE k.hanh_dong WHEN 'INSERT' THEN 'Thêm' WHEN 'UPDATE' THEN 'Sửa' WHEN 'DELETE' THEN 'Xóa' ELSE k.hanh_dong END,
                           d.ma_don_vi, k.tai_khoan_csdl, k.ung_dung, k.ten_bang, NULL, NULL
                    FROM ql_don_vi.nhat_ky_thay_doi k
                    LEFT JOIN ql_don_vi.vi_tri_don_vi v  ON k.ten_bang = 'vi_tri_don_vi'  AND v.id = k.id_ban_ghi
                    LEFT JOIN ql_don_vi.ho_so_nghiep_vu h ON k.ten_bang = 'ho_so_nghiep_vu' AND h.id = k.id_ban_ghi
                    LEFT JOIN ql_don_vi.khu_vuc_don_vi kv ON k.ten_bang = 'khu_vuc_don_vi' AND kv.id = k.id_ban_ghi
                    LEFT JOIN ql_don_vi.don_vi_quan_doi d ON d.id = CASE k.ten_bang
                        WHEN 'don_vi_quan_doi' THEN k.id_ban_ghi WHEN 'vi_tri_don_vi' THEN v.don_vi_id
                        WHEN 'ho_so_nghiep_vu' THEN h.don_vi_id WHEN 'khu_vuc_don_vi' THEN kv.don_vi_id END
                ) z
                ORDER BY thoi_diem DESC
                LIMIT @gh";
            List<DongNhatKy> ds = new List<DongNhatKy>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            {
                ThamSo(cmd, "@gh", DbType.Int32, gioiHan);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ds.Add(new DongNhatKy
                        {
                            Nguon = Chuoi(rd, 0),
                            Id = rd.GetInt64(1),
                            ThoiDiem = rd.GetDateTime(2),
                            ThaoTac = Chuoi(rd, 3),
                            MaDonVi = Chuoi(rd, 4),
                            TaiKhoan = Chuoi(rd, 5),
                            UngDung = Chuoi(rd, 6),
                            Bang = Chuoi(rd, 7),
                            DuLieuCu = Chuoi(rd, 8),
                            DuLieuMoi = Chuoi(rd, 9)
                        });
                    }
                }
            }
            return ds;
        }

        // ---------------- kiem thu + hieu nang ----------------

        public List<KetQuaKiemThu> ChayKiemThu()
        {
            List<KetQuaKiemThu> ds = new List<KetQuaKiemThu>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT stt, nhom, ten, dat, chi_tiet FROM ql_kiem_thu.fn_chay_kiem_thu()"))
            {
                cmd.CommandTimeout = 120;
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        ds.Add(new KetQuaKiemThu
                        {
                            Stt = Convert.ToInt32(rd.GetValue(0)),
                            Nhom = Chuoi(rd, 1),
                            Ten = Chuoi(rd, 2),
                            Dat = !rd.IsDBNull(3) && rd.GetBoolean(3),
                            ChiTiet = Chuoi(rd, 4)
                        });
            }
            return ds;
        }

        public List<KetQuaHieuNang> DoHieuNang()
        {
            List<KetQuaHieuNang> ds = new List<KetQuaHieuNang>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ten, mo_ta, ms_co_chi_muc, ms_khong_chi_muc, dung_chi_muc, ke_hoach FROM ql_he_thong.fn_do_hieu_nang()"))
            {
                cmd.CommandTimeout = 120;
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        ds.Add(new KetQuaHieuNang
                        {
                            Ten = Chuoi(rd, 0),
                            MoTa = Chuoi(rd, 1),
                            MsCoChiMuc = rd.IsDBNull(2) ? 0 : Convert.ToDouble(rd.GetValue(2)),
                            MsKhongChiMuc = rd.IsDBNull(3) ? 0 : Convert.ToDouble(rd.GetValue(3)),
                            DungChiMuc = !rd.IsDBNull(4) && rd.GetBoolean(4),
                            KeHoach = Chuoi(rd, 5)
                        });
            }
            return ds;
        }

        public List<KichThuocBang> LayKichThuocBang()
        {
            List<KichThuocBang> ds = new List<KichThuocBang>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT luoc_do, ten_bang, so_dong, du_lieu, chi_muc, tong FROM ql_he_thong.fn_kich_thuoc_bang()"))
            using (DbDataReader rd = cmd.ExecuteReader())
                while (rd.Read())
                    ds.Add(new KichThuocBang
                    {
                        LuocDo = Chuoi(rd, 0), TenBang = Chuoi(rd, 1),
                        SoDong = rd.IsDBNull(2) ? 0 : Math.Max(0, Convert.ToInt64(rd.GetValue(2))),
                        DuLieu = Chuoi(rd, 3), ChiMuc = Chuoi(rd, 4), Tong = Chuoi(rd, 5)
                    });
            return ds;
        }

        public List<ThongTinChiMuc> LayChiMuc()
        {
            List<ThongTinChiMuc> ds = new List<ThongTinChiMuc>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT luoc_do || '.' || ten_bang, ten_chi_muc, kieu, kich_thuoc, so_lan_quet, dinh_nghia FROM ql_he_thong.fn_danh_sach_chi_muc()"))
            using (DbDataReader rd = cmd.ExecuteReader())
                while (rd.Read())
                    ds.Add(new ThongTinChiMuc
                    {
                        Bang = Chuoi(rd, 0), TenChiMuc = Chuoi(rd, 1), Kieu = Chuoi(rd, 2).ToUpperInvariant(),
                        KichThuoc = Chuoi(rd, 3), SoLanQuet = rd.IsDBNull(4) ? 0 : Convert.ToInt64(rd.GetValue(4)),
                        DinhNghia = Chuoi(rd, 5)
                    });
            return ds;
        }

        public string LamMoiThongKe()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_he_thong.fn_lam_moi_thong_ke()"))
            {
                cmd.CommandTimeout = 120;
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        // ---------------- sao luu / khoi phuc (pg_dump / pg_restore) ----------------

        /// <summary>Tim thu muc bin PostgreSQL moi nhat co pg_dump.exe.</summary>
        public static string TimThuMucPgBin()
        {
            // Ung dung chay 32-bit (MapWinGIS x86) -> SpecialFolder.ProgramFiles tra ve "Program Files (x86)";
            // PostgreSQL 64-bit nam o ProgramW6432.
            string[] cacGoc =
            {
                Environment.GetEnvironmentVariable("ProgramW6432"),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
            };
            return cacGoc
                .Where(g => !string.IsNullOrEmpty(g))
                .Select(g => Path.Combine(g, "PostgreSQL"))
                .Where(Directory.Exists)
                .SelectMany(Directory.GetDirectories)
                .Select(d => new { d, so = SoPhienBan(Path.GetFileName(d)) })
                .Where(x => File.Exists(Path.Combine(x.d, "bin", "pg_dump.exe")))
                .OrderByDescending(x => x.so)
                .Select(x => Path.Combine(x.d, "bin"))
                .FirstOrDefault();
        }

        private static int SoPhienBan(string s)
        {
            int n;
            return int.TryParse(new string((s ?? "").TakeWhile(char.IsDigit).ToArray()), out n) ? n : 0;
        }

        /// <summary>Thong so ket noi lay tu chuoi ket noi ung dung; tai khoan sao luu = postgres (mac dinh) hoac bien moi truong.</summary>
        public static void LayThongSoMayChu(out string host, out string port, out string csdl, out string user)
        {
            DbConnectionStringBuilder b = new DbConnectionStringBuilder { ConnectionString = KetNoiCoSoDuLieu.LayChuoiKetNoi() };
            host = DocKhoa(b, "Host", "Server") ?? "127.0.0.1";
            port = DocKhoa(b, "Port") ?? "5432";
            csdl = DocKhoa(b, "Database") ?? "HQTCSDL";
            user = Environment.GetEnvironmentVariable("MANAGERGISACADEMY_BACKUP_USER");
            if (string.IsNullOrWhiteSpace(user))
                user = Environment.GetEnvironmentVariable("MANAGERGISACADEMY_BACKUP_USER", EnvironmentVariableTarget.User);
            if (string.IsNullOrWhiteSpace(user)) user = "postgres";
        }

        private static string DocKhoa(DbConnectionStringBuilder b, params string[] khoa)
        {
            foreach (string k in khoa)
            {
                object v;
                if (b.TryGetValue(k, out v) && v != null && v.ToString().Length > 0) return v.ToString();
            }
            return null;
        }

        public static List<TepSaoLuu> LietKeSaoLuu(string thuMuc)
        {
            if (string.IsNullOrWhiteSpace(thuMuc) || !Directory.Exists(thuMuc)) return new List<TepSaoLuu>();
            return new DirectoryInfo(thuMuc).GetFiles("*.backup")
                .OrderByDescending(f => f.LastWriteTime)
                .Select(f => new TepSaoLuu { DuongDan = f.FullName, KichThuocByte = f.Length, ThoiGian = f.LastWriteTime })
                .ToList();
        }

        /// <summary>Chay mot cong cu PostgreSQL (khong hien cua so), tra ve (ma thoat, stderr+stdout).</summary>
        public static Tuple<int, string> ChayCongCu(string tenExe, string thamSo, int choToiDaGiay)
        {
            string bin = TimThuMucPgBin();
            if (bin == null) throw new InvalidOperationException("Không tìm thấy thư mục PostgreSQL\\<phiên bản>\\bin có pg_dump.exe.");
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(bin, tenExe), thamSo)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8
            };
            psi.EnvironmentVariables["PGCLIENTENCODING"] = "UTF8";
            using (Process p = Process.Start(psi))
            {
                string ra = p.StandardOutput.ReadToEndAsync().Result;
                string loi = p.StandardError.ReadToEnd();
                if (!p.WaitForExit(choToiDaGiay * 1000))
                {
                    try { p.Kill(); } catch { }
                    throw new TimeoutException(tenExe + " chạy quá " + choToiDaGiay + " giây.");
                }
                return Tuple.Create(p.ExitCode, (loi + "\n" + ra).Trim());
            }
        }

        // ---------------- tien ich ----------------

        private static DbCommand Lenh(DbConnection cn, string sql)
        {
            DbCommand cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            return cmd;
        }

        private static void ThamSo(DbCommand cmd, string ten, DbType kieu, object giaTri)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = ten;
            p.DbType = kieu;
            p.Value = giaTri ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }

        private static string Chuoi(DbDataReader rd, int i)
        {
            return rd.IsDBNull(i) ? "" : Convert.ToString(rd.GetValue(i));
        }
    }
}
