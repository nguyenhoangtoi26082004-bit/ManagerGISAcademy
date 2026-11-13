
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Forms;

namespace ManagerGISAcademy
{
    /// <summary>
    /// Khoi dong khung giao dien truoc,
    /// kiem tra quyen PostGIS sau.
    ///
    /// Khong cho phep doc/ghi ho so truoc
    /// khi xac thuc quyen.
    ///
    /// C# 7.3 - .NET Framework 4.8.
    /// </summary>
    internal static class Program
    {
        private static readonly Stopwatch DongHoKhoiDong =
            Stopwatch.StartNew();

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormMain giaoDien;

            try
            {
                // Chi tao khung FormMain tai day.
                // Phan khoi tao nang trong FormMain
                // se duoc toi uu o cac buoc tiep theo.

                giaoDien = new FormMain();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "[STARTUP] CREATE_FORM_ERROR | " +
                    ex.GetType().Name);

                MessageBox.Show(
                    "Không tạo được cửa sổ chính. " +
                    "Kiểm tra thư viện WinForms/MapWinGIS " +
                    "và cấu hình dự án.",
                    "Lỗi khởi động",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            Debug.WriteLine(
                "[STARTUP] FORM_CREATED | ms=" +
                DongHoKhoiDong.ElapsedMilliseconds);

            // Dang ky su kien mot lan.
            // Khong mo ket noi CSDL truoc Application.Run.

            giaoDien.Shown += (sender, args) =>
            {
                Debug.WriteLine(
                    "[STARTUP] FORM_SHOWN | ms=" +
                    DongHoKhoiDong.ElapsedMilliseconds);

                if (giaoDien.IsDisposed ||
                    giaoDien.Disposing ||
                    !giaoDien.IsHandleCreated)
                {
                    return;
                }

                // Dua vao hang doi UI sau khi Form da hien thi.
                // Truy van PostgreSQL duoc thuc hien
                // bang Task.Run, khong block UI.

                giaoDien.BeginInvoke(
                    new MethodInvoker(() =>
                    {
                        if (giaoDien.IsDisposed ||
                            giaoDien.Disposing)
                        {
                            return;
                        }

                        Task tacVu =
                            KhoiTaoNghiepVuAsync(giaoDien);

                        GC.KeepAlive(tacVu);
                    }));
            };

            Application.Run(giaoDien);
        }

        // =====================================================
        // 1. KHOI TAO NGHIEP VU BAT DONG BO
        // =====================================================

        private static async Task KhoiTaoNghiepVuAsync(
            FormMain giaoDien)
        {
            Stopwatch dongHo = Stopwatch.StartNew();

            try
            {
                // Khong truy cap WinForms hay MapWinGIS
                // ben trong Task.Run.
                //
                // Chi doc quyen va tao cac doi tuong
                // DAL/BLL khong phu thuoc giao dien.

                Tuple<ChinhSachQuyenPostGIS,
                    NghiepVuDonVi> ketQua =
                    await Task.Run(() =>
                    {
                        // He thong tai khoan (script 89): chua dang nhap = KHACH (gis_xem).
                        try { PhienDangNhap.HeThongSanSang = TruyXuatTaiKhoan.KiemTraSanSang(); }
                        catch (Exception exTk)
                        {
                            PhienDangNhap.HeThongSanSang = false;
                            Debug.WriteLine("[STARTUP] ACCOUNT_CHECK_FAILED | " + exTk.Message);
                        }
                        if (PhienDangNhap.HeThongSanSang && !PhienDangNhap.DaDangNhap)
                            ManagerGISAcademy.DAL.KetNoiCoSoDuLieu.VaiTroPhien = "gis_xem";

                        ChinhSachQuyenPostGIS chinhSach =
                            ChinhSachQuyenPostGIS
                                .DocQuyenTuCoSoDuLieu();

                        if (!chinhSach.ChoPhepXem)
                        {
                            return Tuple.Create(
                                chinhSach,
                                (NghiepVuDonVi)null);
                        }

                        TruyXuatDonVi khoDuLieu =
                            new TruyXuatDonVi(
                                ManagerGISAcademy.DAL
                                    .KetNoiCoSoDuLieu
                                    .LayChuoiKetNoi());

                        NghiepVuDonVi nghiepVu =
                            new NghiepVuDonVi(
                                khoDuLieu,
                                chinhSach);

                        return Tuple.Create(
                            chinhSach,
                            nghiepVu);
                    });

                if (giaoDien.IsDisposed ||
                    giaoDien.Disposing ||
                    !giaoDien.IsHandleCreated)
                {
                    return;
                }

                // =================================================
                // 2. KHONG CO QUYEN DOC
                // =================================================

                if (ketQua.Item2 == null)
                {
                    // Khong gan BLL neu khong co quyen.
                    // Khong nap du lieu ho so.

                    giaoDien.GanThongBaoKhoiTao(
                        "Tài khoản PostgreSQL chưa có quyền " +
                        "SELECT trên public.don_vi_quan_doi. " +
                        "Các chức năng hồ sơ tiếp tục bị khóa.");

                    giaoDien.Text +=
                        " — Chưa đủ quyền truy cập dữ liệu";

                    Debug.WriteLine(
                        "[STARTUP] PERMISSION_DENIED");

                    return;
                }

                // =================================================
                // 3. GAN BLL TREN LUONG GIAO DIEN
                // =================================================

                giaoDien.GanNghiepVuDonVi(
                    ketQua.Item2,
                    ketQua.Item1);

                giaoDien.CapNhatTrangThaiTaiKhoan();

                Debug.WriteLine(
                    "[STARTUP] BLL_READY | ms=" +
                    dongHo.ElapsedMilliseconds);

                // =================================================
                // 4. NAP LOP DIEM SAU KHI CO QUYEN
                // =================================================

                // FormMain.PostGIS.OnShown co the chay
                // truoc khi BLL san sang.
                //
                // Vi vay lap lich nap lai diem mot lan
                // sau khi gan BLL thanh cong.
                //
                // COM MapWinGIS phai duoc goi tren UI thread.

                giaoDien.BeginInvoke(
                    new MethodInvoker(() =>
                    {
                        if (giaoDien.IsDisposed ||
                            giaoDien.Disposing)
                        {
                            return;
                        }

                        try
                        {
                            giaoDien.NapLaiLopDiemPostGIS();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(
                                "[STARTUP] " +
                                "INITIAL_MAP_POINTS_ERROR | " +
                                ex.GetType().Name);
                        }
                    }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "[STARTUP] DB_PERMISSION_ERROR | " +
                    ex.GetType().Name);

                if (!giaoDien.IsDisposed &&
                    !giaoDien.Disposing)
                {
                    // Khong dua chuoi ket noi
                    // hoac mat khau vao thong bao.

                    giaoDien.GanThongBaoKhoiTao(
                        "Chưa khởi tạo được quyền truy cập " +
                        "PostgreSQL. Hãy kiểm tra tài khoản " +
                        "CSDL, kết nối và vai trò ứng dụng. " +
                        "Chức năng hồ sơ vẫn bị khóa.");

                    giaoDien.Text +=
                        " — Nghiệp vụ PostGIS chưa sẵn sàng";
                }
            }
            finally
            {
                dongHo.Stop();

                Debug.WriteLine(
                    "[STARTUP] AUTH_FINISHED | ms=" +
                    dongHo.ElapsedMilliseconds);
            }
        }
    }

    // =========================================================
    // 5. CHINH SACH PHAN QUYEN POSTGRESQL
    // =========================================================

    /// <summary>
    /// Chinh sach duoc xac nhan tu quyen PostgreSQL.
    ///
    /// Yeu cau tai khoan ung dung it quyen,
    /// khong la superuser/BYPASSRLS.
    ///
    /// Phan quyen tung ho so can duoc dam bao
    /// boi PostgreSQL/RLS.
    /// </summary>
    internal sealed class ChinhSachQuyenPostGIS :
        IPhanQuyenDonViQuanDoi
    {
        public bool ChoPhepXem { get; private set; }
        public bool ChoPhepThem { get; private set; }
        public bool ChoPhepSua { get; private set; }
        public bool ChoPhepXoa { get; private set; }

        private ChinhSachQuyenPostGIS()
        {
        }

        public bool DuocPhep(
            ThaoTacDonViQuanDoi thaoTac,
            string maDonVi)
        {
            switch (thaoTac)
            {
                case ThaoTacDonViQuanDoi.Xem:
                    return ChoPhepXem;

                case ThaoTacDonViQuanDoi.Them:
                    return ChoPhepThem;

                case ThaoTacDonViQuanDoi.Sua:
                    return ChoPhepSua;

                case ThaoTacDonViQuanDoi.Xoa:
                    return ChoPhepXoa;

                default:
                    return false;
            }
        }

        // =====================================================
        // 6. DOC QUYEN TU CO SO DU LIEU
        // =====================================================

        public static ChinhSachQuyenPostGIS
            DocQuyenTuCoSoDuLieu()
        {
            using (System.Data.Common.DbConnection ketNoi =
                ManagerGISAcademy.DAL.KetNoiCoSoDuLieu
                    .MoKetNoi())
            using (DbCommand lenh = ketNoi.CreateCommand())
            {
                lenh.CommandText = @"
                    SELECT
                        r.rolsuper,
                        r.rolbypassrls,

                        has_table_privilege(
                            current_user,
                            'public.don_vi_quan_doi',
                            'SELECT'
                        ),

                        has_table_privilege(
                            current_user,
                            'public.don_vi_quan_doi',
                            'INSERT'
                        ),

                        has_table_privilege(
                            current_user,
                            'public.don_vi_quan_doi',
                            'UPDATE'
                        ),

                        has_table_privilege(
                            current_user,
                            'public.don_vi_quan_doi',
                            'DELETE'
                        )

                    FROM pg_catalog.pg_roles AS r
                    WHERE r.rolname = current_user";

                lenh.CommandType = CommandType.Text;
                lenh.CommandTimeout = 15;

                using (DbDataReader doc =
                    lenh.ExecuteReader())
                {
                    if (!doc.Read())
                    {
                        throw new UnauthorizedAccessException(
                            "Không xác định được vai trò PostgreSQL.");
                    }

                    if (doc.GetBoolean(0) ||
                        doc.GetBoolean(1))
                    {
                        throw new UnauthorizedAccessException(
                            "Không sử dụng tài khoản superuser " +
                            "hoặc BYPASSRLS cho nghiệp vụ. " +
                            "Hãy sử dụng tài khoản ứng dụng ít quyền.");
                    }

                    return new ChinhSachQuyenPostGIS
                    {
                        ChoPhepXem = doc.GetBoolean(2),
                        ChoPhepThem = doc.GetBoolean(3),
                        ChoPhepSua = doc.GetBoolean(4),
                        ChoPhepXoa = doc.GetBoolean(5)
                    };
                }
            }
        }
    }
}
