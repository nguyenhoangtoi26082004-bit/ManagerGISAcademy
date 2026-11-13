
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using AxMapWinGIS;

using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using ManagerGISAcademy.GUI.Views;
using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Form chinh HanoiMilitaryGIS.
    /// C# 7.3 / .NET Framework 4.8 / WinForms / MapWinGIS.
    ///
    /// Tra cuu don vi duoc tich hop tren Ban do GIS.
    /// Muc Tra cuu rieng chi bi an khoi menu,
    /// khong xoa man hinh cu hoac du lieu.
    /// </summary>
    public partial class FormMain : Form
    {
        // ==================================================
        // 1. CAU HINH VA TRANG THAI
        // ==================================================

        private readonly Color mauMenu =
            Color.FromArgb(9, 17, 28);

        private readonly Color mauDangChon =
            Color.FromArgb(16, 185, 129);

        private const int EpsgBanDo = 32648;

        private string thongBaoKhoiTao;

        private string phanHeHienTai = "Bản đồ GIS";

        private bool daKiemTraKetNoi;
        private bool dangCapNhatLop;
        private bool dangThaoTacBanDo;

        private bool? daKetNoiPostGIS;

        private Button nutDangChon;
        private Button btnNapLop;

        private CheckedListBox danhSachLop;

        private Control dieuKhienBanDo;

        private System.Windows.Forms.Timer mainNavTheoDoi;

        private readonly Dictionary<string, Control> cacManHinh =
            new Dictionary<string, Control>(
                StringComparer.Ordinal);

        // ==================================================
        // 2. NGHIEP VU VA PHAN QUYEN
        // ==================================================

        private NghiepVuDonVi nghiepVuDonVi;

        private IPhanQuyenDonViQuanDoi chinhSachQuyen;

        private List<DonViQuanDoi> nativeBoNhoHoSo;

        private DateTime nativeLanTaiHoSo =
            DateTime.MinValue;

        private bool nativePhaiTaiLaiHoSo = true;

        private readonly TimeSpan nativeThoiHanBoNho =
            TimeSpan.FromSeconds(30);

        private Control nativeManHinhHoSoDangMo;

        private int nativePhienBanDuLieu;

        private readonly Dictionary<Control, int>
            nativePhienBanTrang =
            new Dictionary<Control, int>();

        // ==================================================
        // 3. BAN DO MAPWINGIS
        // ==================================================

        private AxMap banDoMapWinGIS;

        private TroGiupMapWinGIS ketNoiMapWinGIS;

        private readonly List<LopMapItem> cacLop =
            new List<LopMapItem>();

        // ==================================================
        // 4. PHAN TICH KHONG GIAN
        // ==================================================

        private int veVungDem = -1;
        private int veDiemKetQua = -1;
        private int veDiemDangChon = -1;

        private List<KetQuaPhanTichItem> cacDiemPhanTich =
            new List<KetQuaPhanTichItem>();

        // ==================================================
        // 5. TOI UU ZOOM
        // ==================================================

        private const int ZoomGomYeuCauMs = 110;

        private const double ZoomHeSoMoiBuoc = 0.86d;

        private const int ZoomGioiHanBuocCho = 24;

        private int soBuocZoomDangCho;

        private System.Windows.Forms.Timer henXuLyZoom;

        private DateTime lanZoomCuoi =
            DateTime.MinValue;

        private DateTime lanGhiPhamViCuoi =
            DateTime.MinValue;

        // ==================================================
        // 6. NHAN DIEN HE THONG
        // ==================================================

        private readonly Font mainFontTenNho =
            new Font(
                "Segoe UI",
                8.6F,
                FontStyle.Bold);

        private bool mainDaDangKyHeaderNho;
        private bool mainDaHenHeaderNho;

        private bool mainDaLuuBoCucDauTrang;

        private DockStyle mainDockHeaderGoc;

        private int mainCaoHeaderGoc;

        private DockStyle mainDockBrandGoc;

        private int mainRongBrandGoc;

        private Control mainChaBrandGoc;

        private bool mainDangHienTrangNghiepVu;

        // ==================================================
        // 7. TIM KIEM THONG MINH TREN BAN DO
        // ==================================================

        private System.Windows.Forms.Timer headerHenGoiY;

        private HeaderGoiYForm headerPopup;

        private Panel headerPopupNoiDung;

        private FlowLayoutPanel headerPopupDanhSach;

        private Label headerPopupThongKe;

        private readonly List<Panel> headerDongKetQua =
            new List<Panel>();

        private List<DonViQuanDoi> headerKetQua =
            new List<DonViQuanDoi>();

        private List<DonViQuanDoi> headerBoNho;

        private DateTime headerLanTai =
            DateTime.MinValue;

        private bool headerDangTai;
        private bool headerBoQuaThayDoi;

        private int headerChiSoChon;

        private bool headerConTimKiem;

        private HeaderGoiYOutsideClickFilter
            headerBoLocClickNgoai;

        private string headerTuKhoaDaHienThi =
            string.Empty;

        private readonly TimeSpan headerThoiHanCache =
            TimeSpan.FromSeconds(45);

        // ==================================================
        // 8. NHAT KY MAPWINGIS
        // ==================================================

        private readonly string tepNhatKyBanDo =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ManagerGISAcademy",
                "mapwingis-diagnostic.log");

        // ==================================================
        // 9. SU KIEN
        // ==================================================

        public event EventHandler YeuCauPhongToBanDo;

        public event EventHandler YeuCauThuNhoBanDo;

        public event EventHandler YeuCauToanCanhBanDo;

        public event EventHandler YeuCauThayDoiLop;

        // ==================================================
        // 10. TU KHOA TRA CUU
        // ==================================================

        public string TuKhoaTraCuu
        {
            get
            {
                if (txtQuickSearch == null ||
                    txtQuickSearch.IsDisposed)
                {
                    return string.Empty;
                }

                return (txtQuickSearch.Text ?? string.Empty)
                    .Trim();
            }
        }

        // ==================================================
        // 11. KHOI TAO FORM
        // ==================================================

        public FormMain()
        {
            InitializeComponent();

            if (DesignMode ||
                LicenseManager.UsageMode ==
                LicenseUsageMode.Designtime)
            {
                return;
            }

            KhoiTaoDanhSachLop();

            KhoiTaoNutNapLop();

            DangKySuKien();

            CapNhatTrangThaiPostGIS(null);

            BatDieuKhienLop(false);

            btnAddUnit.Enabled = false;

            // Ban do GIS la phan he mac dinh.
            ChonPhanHe(
                btnNavMap,
                "Bản đồ GIS");

            // Tra cuu da duoc tich hop tren ban do.
            // Khong xoa view cu de bao dam tuong thich.
            Main_AnMenuTraCuuRieng();
        }

        // ==================================================
        // 12. GAN NGHIEP VU
        // ==================================================

        public void GanNghiepVuDonVi(
            NghiepVuDonVi nghiepVu)
        {
            GanNghiepVuDonVi(
                nghiepVu,
                null);
        }

        public void GanNghiepVuDonVi(
            NghiepVuDonVi nghiepVu,
            IPhanQuyenDonViQuanDoi chinhSach)
        {
            if (nghiepVu == null)
            {
                throw new ArgumentNullException(
                    "nghiepVu");
            }

            if (!nghiepVu.DaCauHinh)
            {
                throw new InvalidOperationException(
                    "NghiepVuDonVi chưa được cấu hình DAL " +
                    "và chính sách phân quyền.");
            }

            nghiepVuDonVi = nghiepVu;

            chinhSachQuyen = chinhSach;

            btnAddUnit.Enabled =
                CoQuyen(ThaoTacDonViQuanDoi.Them);

            Native_DanhDauDuLieuCanTaiLai();

            TaiLaiDuLieuPhanHe();
        }

        public void GanThongBaoKhoiTao(
            string thongBao)
        {
            thongBaoKhoiTao = thongBao;

            btnAddUnit.Enabled = false;

            if (!string.IsNullOrWhiteSpace(thongBao))
            {
                statusGeneral.Text =
                    "● " + thongBao;
            }
        }

        // ==================================================
        // 13. KIEM TRA PHAN QUYEN
        // ==================================================

        private bool CoQuyen(
            ThaoTacDonViQuanDoi thaoTac,
            string maDonVi = null)
        {
            return nghiepVuDonVi != null &&
                   nghiepVuDonVi.DaCauHinh &&
                   chinhSachQuyen != null &&
                   chinhSachQuyen.DuocPhep(
                       thaoTac,
                       maDonVi ?? string.Empty);
        }

        // ==================================================
        // 14. TIM MAN HINH DA TAO
        // ==================================================

        private T TimManHinh<T>(string ten)
            where T : Control
        {
            Control giaTri;

            return cacManHinh.TryGetValue(
                ten,
                out giaTri)
                ? giaTri as T
                : null;
        }

        // ==================================================
        // 15. FORM LOAD
        // ==================================================

        private async void FormMain_Load(
            object sender,
            EventArgs e)
        {
            if (daKiemTraKetNoi)
                return;

            daKiemTraKetNoi = true;

            CapNhatTrangThaiPostGIS(null);

            lblConnectHint.Text =
                "Đang kiểm tra PostgreSQL/PostGIS...";

            Tuple<bool, string> ketQua;

            try
            {
                ketQua = await Task.Run(() =>
                {
                    string thongBao;

                    bool thanhCong =
                        KetNoiCoSoDuLieu.ThuKetNoi(
                            out thongBao);

                    return Tuple.Create(
                        thanhCong,
                        thongBao);
                });
            }
            catch (Exception ex)
            {
                ketQua = Tuple.Create(
                    false,
                    "Lỗi kiểm tra kết nối: " +
                    ex.Message);
            }

            if (IsDisposed ||
                Disposing ||
                !IsHandleCreated)
            {
                return;
            }

            CapNhatTrangThaiPostGIS(
                ketQua.Item1);

            lblConnectHint.Text = ketQua.Item1
                ? "Đã kết nối PostgreSQL/PostGIS."
                : "Không kết nối được: " +
                  ketQua.Item2;

            if (!ketQua.Item1)
            {
                statusGeneral.Text =
                    "● PostgreSQL/PostGIS không kết nối.";

                MessageBox.Show(
                    this,
                    ketQua.Item2,
                    "Lỗi kết nối PostgreSQL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                statusGeneral.Text =
                    "● PostgreSQL/PostGIS đã kết nối.";

                if (CoQuyen(
                    ThaoTacDonViQuanDoi.Xem) &&
                    cacManHinh.Count > 0)
                {
                    TaiLaiDuLieuPhanHe();
                }
            }

            if (!string.IsNullOrWhiteSpace(
                thongBaoKhoiTao))
            {
                statusGeneral.Text =
                    "● " + thongBaoKhoiTao;

                MessageBox.Show(
                    this,
                    thongBaoKhoiTao,
                    "Trạng thái nghiệp vụ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            CapNhatTongQuan();
        }

        // ==================================================
        // 16. FORM SHOWN
        // ==================================================

        private void FormMain_Shown(
            object sender,
            EventArgs e)
        {
            CanGiuaThongBao();

            GhiNhatKyBanDo(
                "FORM_SHOWN | 64bit=" +
                Environment.Is64BitProcess);

            KhoiTaoMapWinGIS();

            CanChinhToaDoGocDuoiPhai();

            Main_DangKyHeaderNho();

            // An trang thai PostGIS cu o sidebar.
            if (pnlSidebarBottom != null &&
                !pnlSidebarBottom.IsDisposed)
            {
                pnlSidebarBottom.Visible = false;

                if (pnlSidebarBottom.Parent != null)
                {
                    pnlSidebarBottom.Parent.Controls.Remove(
                        pnlSidebarBottom);
                }
            }

            Main_AnHaiCumChuDieuHuong(
                pnlSidebar);

            // An muc Tra cuu cu va thu gon sidebar.
            Main_AnMenuTraCuuRieng();

            Main_ThuGonKhoangTrongMenu();

            // Dam bao menu van gon khi resize.
            pnlSidebar.Resize += (s, a) =>
            {
                Main_AnMenuTraCuuRieng();
                Main_ThuGonKhoangTrongMenu();
            };

            flowNavigation.Resize += (s, a) =>
            {
                Main_AnMenuTraCuuRieng();
                Main_ThuGonKhoangTrongMenu();
            };

            // Neu menu duoc tao lai khi dang chay.
            flowNavigation.ControlAdded += (s, a) =>
            {
                Main_AnMenuTraCuuRieng();
            };
        }

        // ==================================================
        // 17. DONG FORM
        // ==================================================

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            Header_GiaiPhong();

            if (henXuLyZoom != null)
            {
                henXuLyZoom.Stop();

                henXuLyZoom.Tick -=
                    BanDo_XuLyZoomDangCho;

                henXuLyZoom.Dispose();
                henXuLyZoom = null;

                soBuocZoomDangCho = 0;
            }

            GhiNhatKyBanDo("FORM_CLOSED");

            if (mainNavTheoDoi != null)
            {
                mainNavTheoDoi.Stop();

                mainNavTheoDoi.Tick -=
                    MainNav_TickKhoiTaoWeb;

                mainNavTheoDoi.Dispose();
                mainNavTheoDoi = null;
            }

            XoaVePhanTich();

            if (banDoMapWinGIS != null &&
                !banDoMapWinGIS.IsDisposed)
            {
                try
                {
                    banDoMapWinGIS.ExtentsChanged -=
                        BanDo_ExtentsChanged;

                    banDoMapWinGIS.MouseMoveEvent -=
                        BanDo_CapNhatToaDoDuoi_MouseMove;
                }
                catch
                {
                    // ActiveX co the da giai phong.
                }
            }

            foreach (Control manHinh in cacManHinh.Values)
            {
                if (manHinh != null &&
                    !manHinh.IsDisposed)
                {
                    manHinh.Dispose();
                }
            }

            base.OnFormClosed(e);

            mainFontTenNho.Dispose();
        }

        // ==================================================
        // 18. AN MUC TRA CUU RIENG
        // ==================================================

        private void Main_AnMenuTraCuuRieng()
        {
            if (flowNavigation == null ||
                flowNavigation.IsDisposed)
            {
                return;
            }

            // Chi tim trong sidebar.
            // Khong anh huong thanh tim kiem tren ban do.
            foreach (Control c in
                Main_DuyetControl(flowNavigation))
            {
                string ten =
                    (c.Name ?? string.Empty)
                    .ToLowerInvariant();

                string nhan =
                    (c.Text ?? string.Empty).Trim();

                bool laMucCu =
                    nhan.IndexOf(
                        "Tra cứu đơn vị",
                        StringComparison.CurrentCultureIgnoreCase)
                    >= 0 ||

                    ten == "btnnavsearch" ||
                    ten == "btnnavtracuu" ||
                    ten == "btnnavlookup";

                if (!laMucCu)
                    continue;

                Control hang = c;

                // Neu menu item nam trong Panel,
                // an ca Panel de khong de khoang trong.
                while (hang.Parent != null &&
                       hang.Parent != flowNavigation)
                {
                    hang = hang.Parent;
                }

                if (hang == flowNavigation)
                    continue;

                hang.Visible = false;
                hang.TabStop = false;
            }
        }

        // ==================================================
        // 19. DUYET CAC CONTROL TRONG MENU
        // ==================================================

        private static IEnumerable<Control>
            Main_DuyetControl(Control cha)
        {
            foreach (Control con in cha.Controls)
            {
                yield return con;

                foreach (Control chau in
                    Main_DuyetControl(con))
                {
                    yield return chau;
                }
            }
        }

        // ==================================================
        // 20. TUONG THICH DESIGNER
        // ==================================================

        private void FormMain_Load_1(
            object sender,
            EventArgs e)
        {
            // Giu lai vi Designer co the dang tham chieu.
            // Khong dang ky them su kien Load tai day.
        }
    }
}
