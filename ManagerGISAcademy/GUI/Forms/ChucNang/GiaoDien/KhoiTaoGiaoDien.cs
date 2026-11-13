using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinImage = System.Drawing.Image;
using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private readonly Color uiNen = Color.FromArgb(244, 247, 245);
        private readonly Color uiThanh = Color.FromArgb(22, 53, 45);
        private readonly Color uiThe = Color.White;
        private readonly Color uiTheNhat = Color.FromArgb(246, 249, 247);
        private readonly Color uiVien = Color.FromArgb(218, 229, 221);
        private readonly Color uiChuPhu = Color.FromArgb(107, 128, 116);
        private readonly Color uiXanh = Color.FromArgb(16, 145, 111);
        private readonly Color uiXanhSang = Color.FromArgb(182, 224, 199);
        private readonly Color uiChu = Color.FromArgb(31, 56, 47);

        private readonly Font uiFontMenu = new Font("Segoe UI", 9.5F);
        private readonly Font uiFontMenuChon =
            new Font("Segoe UI", 9.5F, FontStyle.Bold);
        private readonly Font uiFontPhu = new Font("Segoe UI", 7.8F);
        private readonly Font uiFontTenHeThong =
            new Font("Segoe UI", 9.4F, FontStyle.Bold);
        private readonly Font uiFontTieuDeMotDong =
            new Font("Segoe UI", 9F, FontStyle.Bold);
        private readonly Font uiFontTieuDeHaiDong =
            new Font("Segoe UI", 8.6F, FontStyle.Bold);

        private readonly ToolTip uiMeo = new ToolTip();
        private readonly HashSet<Control> uiDaTrangTri =
            new HashSet<Control>();
        private readonly HashSet<Control> uiDaBoGoc =
            new HashSet<Control>();
        private readonly Dictionary<string, Bitmap> uiBieuTuong =
            new Dictionary<string, Bitmap>(StringComparer.Ordinal);

        private bool uiDaKhoiTao;
        private bool uiDangCanHeader;
        private bool uiDangCanMenu;
        private bool uiDangCanBanDo;
        private bool uiDangCapNhatBangLop;
        private bool uiChoCapNhatTrang;
        private bool uiDaKhoiTaoCongCu;
        private bool uiChoCanCongCu;
        private bool uiBangLopMo;
        private bool uiDaGanTheoDoiCongCu;
        private bool uiDaGanCanNoiDungChuGiai;
        private bool uiDaGanCanZoomDoc;
        private bool uiDangCanZoomDoc;

        private const int UiSidebarRongMo = 316;
        private const int UiSidebarRongGon = 82;
        private const int UiSidebarRongToiDa = 440;

        private int uiSidebarRongDaMo = UiSidebarRongMo;
        private bool uiSidebarDangKeo;
        private bool uiSidebarDangThuGon;
        private int uiSidebarXBatDauKeo;
        private int uiSidebarRongBatDauKeo;

        private Panel uiSidebarTayNam;
        private Button uiSidebarNutThuGon;

        private string uiCongCuDangChon = "info";

        private PictureBox uiLogo;
        private WinImage uiAnhLogo;
        private Button uiNutDongLop;
        private Button uiNutMoBangLop;
        private Bitmap uiAnhNutMoBangLop;

        private Panel uiTheBanDo;
        private Label uiTieuDeBanDo;
        private Label uiMoTaBanDo;
        private Label uiNhanTuyChon;
        private ComboBox uiCheDoHienNhan;

        private Panel uiVungPhai;
        private bool uiHeaderDangNoiTrenBanDo;
        private bool uiDangCanHeaderNoi;
        private Panel uiHopTimKiem;
        private Label uiGoiYTim;
        private System.Windows.Forms.Timer uiHenNhan;
        private bool uiChoVeLaiNhan;

        private int uiHandleNhanDaTao = -1;
        private int uiSoDiemDaTao = -1;
        private int uiSoNhanDaTao = -1;
        private int uiCheDoNhanDaApDung = -1;

        private const double UiMaxTyLeNhanTuDong = 200000d;

        private const string UiTieuDeDayDu =
            "HỆ THỐNG QUẢN LÝ ĐƠN VỊ QUÂN ĐỘI TRÊN ĐỊA BÀN HÀ NỘI";

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode ||
                LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                uiDaKhoiTao ||
                !Visible ||
                pnlHeader == null)
            {
                return;
            }

            uiDaKhoiTao = true;

            UiKhoiTaoGiaoDien();
            // Khoi tao sidebar moi sau khi giao dien goc san sang.
            Sidebar_KhoiTao();
            UiTheoDoiPhanHe();

            Application.Idle += UiChoCongCuBanDo;
            FormClosed += UiGiaiPhong;

            UiHenCapNhatTrang();
        }

        private void UiKhoiTaoGiaoDien()
        {
            SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlSidebar.SuspendLayout();

            try
            {
                BackColor = uiNen;
                ForeColor = uiChu;
                MinimumSize = new Size(1050, 650);

                pnlSidebar.Width = UiSidebarRongMo;
                pnlSidebar.BackColor = uiThanh;

                pnlSidebarTop.Height = 72;
                pnlSidebarTop.BackColor = uiThanh;
                pnlSidebarTop.Padding = Padding.Empty;

                pnlSidebarBottom.Height = 66;
                pnlSidebarBottom.BackColor =
                    Color.FromArgb(18, 44, 37);

                flowNavigation.BackColor = uiThanh;
                flowNavigation.Padding = new Padding(14, 8, 14, 3);

                lblSidebarTitle.Text = string.Empty;
                lblSidebarTitle.Visible = false;
                lblSidebarHint.Text = string.Empty;
                lblSidebarHint.Visible = false;

                lblSidebarVersion.Text = string.Empty;
                lblSidebarVersion.Visible = false;
                pnlSidebarBottom.Visible = false;
                pnlHeader.Height = 72;
                pnlHeader.BackColor = uiThe;

                if (pnlBrand.Parent != pnlSidebarTop)
                {
                    if (pnlBrand.Parent != null)
                    {
                        pnlBrand.Parent.Controls.Remove(pnlBrand);
                    }

                    pnlSidebarTop.Controls.Add(pnlBrand);
                }

                pnlBrand.Dock = DockStyle.Fill;
                pnlBrand.Margin = Padding.Empty;
                pnlBrand.Padding = Padding.Empty;
                pnlBrand.BackColor = uiThanh;
                pnlBrand.Paint += UiVeNenThuongHieu;
                pnlBrand.BringToFront();

                UiTachSidebarVaHeader();
                UiKhoiTaoSidebarCoTheKeo();

                tblHeaderActions.BackColor = uiThe;
                tblHeaderActions.Padding =
                    new Padding(6, 0, 12, 0);

                UiTaoLogo();
                UiCanChuTieuDe();

                // Giao dien Light Military GIS
                lblHeaderDbStatus.AutoSize = false;
                lblHeaderDbStatus.BackColor = Color.FromArgb(225, 250, 239);
                lblHeaderDbStatus.ForeColor = Color.FromArgb(7, 128, 99);
                lblHeaderDbStatus.Font =
                    new Font("Segoe UI", 8.2F, FontStyle.Bold);
                lblHeaderDbStatus.TextAlign = ContentAlignment.MiddleCenter;
                lblHeaderDbStatus.TextChanged +=
                    (s, a) => UiMauTrangThaiPostGIS();
                lblHeaderDbStatus.Paint += UiVeTrangThaiPostGIS;
                UiBoGoc(lblHeaderDbStatus, 19);
                UiMauTrangThaiPostGIS();

                btnAddUnit.Text = "THÊM ĐƠN VỊ";
                btnAddUnit.BackColor = Color.FromArgb(49, 139, 237);
                btnAddUnit.ForeColor = Color.White;
                btnAddUnit.Font =
                    new Font("Segoe UI", 8.4F, FontStyle.Bold);
                btnAddUnit.Margin = new Padding(6, 15, 0, 15);
                UiLamDepNut(btnAddUnit, 19);
                btnAddUnit.Paint += UiVeNutThemDonVi;

                UiTaoHopTimKiem();

                pnlWorkspace.BackColor = uiNen;
                pnlPhanHeHost.BackColor = uiNen;
                tblContent.BackColor = uiNen;
                tblContent.Padding = new Padding(12, 10, 12, 12);

                tblWorkspaceHeading.Visible = false;
                tblWorkspaceHeading.Height = 0;
                lblStageBadge.Visible = false;

                pnlMapCard.BackColor = uiThe;
                pnlMapCard.Margin = Padding.Empty;
                pnlMapCardHeader.Visible = false;
                pnlMapCardHeader.Height = 0;
                lblMapFooter.Visible = false;
                lblMapFooter.Height = 0;

                pnlMapCanvas.BackColor =
                    Color.FromArgb(238, 243, 240);

                statusStrip.BackColor = uiThe;
                statusStrip.ForeColor = uiChuPhu;
                statusStrip.Font = new Font("Segoe UI", 8F);
                statusStrip.SizingGrip = false;
                statusStrip.Padding = new Padding(8, 1, 8, 1);

                UiCanMenu();
                UiTaoTheThongTinBanDo();
                UiDatZoomDuoiCongCu();
            }
            finally
            {
                pnlSidebar.ResumeLayout(true);
                pnlHeader.ResumeLayout(true);
                ResumeLayout(true);
            }

            pnlHeader.Resize +=
                (s, a) => UiCanThanhHeader();

            pnlBrand.Resize +=
                (s, a) => UiCanChuTieuDe();

            pnlSidebar.Resize +=
                (s, a) => UiCanSidebarTheoChieuRong();

            flowNavigation.Resize +=
                (s, a) => UiCanMenu();

            pnlSidebarTop.Resize +=
                (s, a) => UiCanNutThuGonSidebar();

            pnlMapCanvas.Resize +=
                (s, a) =>
                {
                    UiCanHeaderNoiTrenBanDo();
                    UiHenCanCongCuBanDo();
                };

            pnlRightCard.VisibleChanged +=
                UiKhiDoiHienThiBangLop;

            lblPageTitle.TextChanged +=
                (s, a) => UiHenCapNhatTrang();

            UiCanThanhHeader();
        }

        private void UiTachSidebarVaHeader()
        {
            if (uiVungPhai != null && !uiVungPhai.IsDisposed)
            {
                return;
            }

            uiVungPhai = new Panel
            {
                Name = "uiVungPhai",
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = uiNen
            };

            uiVungPhai.SuspendLayout();
            try
            {
                pnlSidebar.Dock = DockStyle.Left;
                pnlSidebar.Width = UiSidebarRongMo;

                pnlHeader.Dock = DockStyle.Top;
                pnlHeader.Height = 72;

                pnlWorkspace.Dock = DockStyle.Fill;
                statusStrip.Dock = DockStyle.Bottom;

                Controls.Add(uiVungPhai);

                uiVungPhai.Controls.Add(pnlWorkspace);
                uiVungPhai.Controls.Add(pnlHeader);
                uiVungPhai.Controls.Add(statusStrip);

                uiVungPhai.Controls.SetChildIndex(pnlWorkspace, 0);
                uiVungPhai.Controls.SetChildIndex(pnlHeader, 1);
                uiVungPhai.Controls.SetChildIndex(statusStrip, 2);

                Controls.SetChildIndex(uiVungPhai, 0);
                Controls.SetChildIndex(pnlSidebar, 1);
            }
            finally
            {
                uiVungPhai.ResumeLayout(true);
            }
        }

        private void UiDungHeaderNoiTrenBanDo()
        {
            if (pnlMapCanvas == null || pnlMapCanvas.IsDisposed ||
                pnlHeader == null || pnlHeader.IsDisposed ||
                uiHopTimKiem == null || uiHopTimKiem.IsDisposed ||
                lblHeaderDbStatus == null || lblHeaderDbStatus.IsDisposed ||
                btnAddUnit == null || btnAddUnit.IsDisposed)
                return;

            if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed)
                banDoMapWinGIS.ShowZoomBar = false;

            if (!uiHeaderDangNoiTrenBanDo)
            {
                uiHeaderDangNoiTrenBanDo = true;

                uiHopTimKiem.Dock = DockStyle.None;
                uiHopTimKiem.Margin = Padding.Empty;
                uiHopTimKiem.BackColor = Color.White;
                pnlMapCanvas.Controls.Add(uiHopTimKiem);

                lblHeaderDbStatus.Dock = DockStyle.None;
                lblHeaderDbStatus.AutoSize = false;
                lblHeaderDbStatus.Margin = Padding.Empty;
                lblHeaderDbStatus.Padding = Padding.Empty;
                lblHeaderDbStatus.BackColor =
                    Color.FromArgb(225, 250, 239);
                pnlMapCanvas.Controls.Add(lblHeaderDbStatus);

                btnAddUnit.Dock = DockStyle.None;
                btnAddUnit.AutoSize = false;
                btnAddUnit.Margin = Padding.Empty;
                btnAddUnit.BackColor =
                    Color.FromArgb(49, 139, 237);
                btnAddUnit.ForeColor = Color.White;
                pnlMapCanvas.Controls.Add(btnAddUnit);

                txtQuickSearch.BackColor = Color.White;
                uiGoiYTim.BackColor = Color.White;

                UiBoGoc(uiHopTimKiem, 20);
                UiBoGoc(lblHeaderDbStatus, 19);
                UiBoGoc(btnAddUnit, 19);
            }

            pnlHeader.Visible = false;
            UiCanHeaderNoiTrenBanDo();
        }

        private void UiTraHeaderVeMacDinh()
        {
            if (!uiHeaderDangNoiTrenBanDo)
            {
                if (pnlHeader != null && !pnlHeader.IsDisposed)
                    pnlHeader.Visible = true;
                return;
            }

            if (tblHeaderActions == null || tblHeaderActions.IsDisposed ||
                uiHopTimKiem == null || uiHopTimKiem.IsDisposed)
                return;

            tblHeaderActions.SuspendLayout();

            try
            {
                tblHeaderActions.Controls.Add(uiHopTimKiem, 0, 0);
                tblHeaderActions.Controls.Add(lblHeaderDbStatus, 2, 0);
                tblHeaderActions.Controls.Add(btnAddUnit, 3, 0);

                uiHopTimKiem.Dock = DockStyle.Fill;
                uiHopTimKiem.Margin =
                    new Padding(0, 15, 0, 15);
                uiHopTimKiem.BackColor = Color.White;
                txtQuickSearch.BackColor = Color.White;
                uiGoiYTim.BackColor = Color.White;

                lblHeaderDbStatus.Dock = DockStyle.Fill;
                lblHeaderDbStatus.Margin =
                    new Padding(5, 15, 5, 15);
                lblHeaderDbStatus.Padding = Padding.Empty;
                lblHeaderDbStatus.BackColor =
                    Color.FromArgb(225, 250, 239);

                btnAddUnit.Dock = DockStyle.Fill;
                btnAddUnit.Margin =
                    new Padding(5, 15, 0, 15);
                btnAddUnit.BackColor =
                    Color.FromArgb(49, 139, 237);
            }
            finally
            {
                tblHeaderActions.ResumeLayout(true);
            }

            uiHeaderDangNoiTrenBanDo = false;
            pnlHeader.Visible = true;

            UiCanThanhHeader();
            UiCanHopTimMoi();
        }

        private void UiCanHeaderNoiTrenBanDo()
        {
            if (!uiHeaderDangNoiTrenBanDo ||
                uiDangCanHeaderNoi ||
                pnlMapCanvas == null || pnlMapCanvas.IsDisposed ||
                uiHopTimKiem == null || uiHopTimKiem.IsDisposed ||
                lblHeaderDbStatus == null || lblHeaderDbStatus.IsDisposed ||
                btnAddUnit == null || btnAddUnit.IsDisposed)
                return;

            int w = pnlMapCanvas.ClientSize.Width;
            if (w <= 0) return;

            uiDangCanHeaderNoi = true;

            try
            {
                const int le = 15;
                const int cach = 10;
                const int cao = 42;
                const int yTren = 11;
                const int rongPostGIS = 177;
                const int rongThem = 155;

                // Chua cho nut tai khoan (dang nhap / avatar) o goc phai.
                int choTk = tkThe != null && !tkThe.IsDisposed && tkThe.Visible ? TkRong + cach : 0;
                int xThem = Math.Max(le, w - le - rongThem - choTk);
                int xDb = xThem - cach - rongPostGIS;

                int rongTim;
                int yPhai;

                bool haiDong = xDb - le - cach < 215;

                if (haiDong)
                {
                    rongTim = Math.Max(140, w - 2 * le);
                    yPhai = yTren + cao + 8;
                }
                else
                {
                    rongTim = Math.Min(326, xDb - cach - le);
                    yPhai = yTren;
                }

                uiHopTimKiem.SetBounds(
                    le, yTren, rongTim, cao);

                lblHeaderDbStatus.SetBounds(
                    Math.Max(le, xDb),
                    yPhai, rongPostGIS, cao);

                btnAddUnit.SetBounds(
                    xThem, yPhai, rongThem, cao);

                if (banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed &&
                    banDoMapWinGIS.Parent == pnlMapCanvas)
                {
                    banDoMapWinGIS.SendToBack();
                }

                uiHopTimKiem.BringToFront();
                lblHeaderDbStatus.BringToFront();
                btnAddUnit.BringToFront();

                UiCanHopTimMoi();
                TaiKhoan_CanViTri();
            }
            finally
            {
                uiDangCanHeaderNoi = false;
            }
        }
    }
}