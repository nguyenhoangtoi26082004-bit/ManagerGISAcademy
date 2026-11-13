// Phan tach tu FormMain.cs (GD5.7.6), dung partial class.
// Khong khai bao lop FormMain moi, khong thay doi FormMain.Designer.cs.
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
    public partial class FormMain
    {
        private void Main_AnHaiCumChuDieuHuong(Control goc)
        {
            if (goc == null || goc.IsDisposed)
                return;

            foreach (Control con in goc.Controls)
            {
                Label nhan = con as Label;

                if (nhan != null && !nhan.IsDisposed)
                {
                    string cu = nhan.Text ?? string.Empty;

                    string moi = cu
                        .Replace("ĐIỀU HƯỚNG", string.Empty)
                        .Replace("07 PHÂN HỆ", string.Empty);

                    if (!string.Equals(cu, moi, StringComparison.Ordinal))
                    {
                        string conLai = moi.Trim(
                            ' ', '\t', '\r', '\n',
                            '•', '·', '|', '-', '–', '—', ':');

                        nhan.Text = moi;

                        // An nhan neu chi con dau phan cach.
                        if (conLai.Length == 0)
                        {
                            nhan.Visible = false;
                            nhan.Height = 0;

                            Panel khung = nhan.Parent as Panel;

                            // Chi thu gon khung rieng cua tieu de.
                            // Khong an sidebar, logo hay khung menu chinh.
                            if (khung != null &&
                                khung != pnlSidebar &&
                                khung != pnlSidebarTop &&
                                khung != pnlBrand &&
                                khung != pnlSidebarBottom &&
                                khung.Controls.Cast<Control>().All(
                                    c => c == nhan || !c.Visible))
                            {
                                khung.Visible = false;
                                khung.Height = 0;
                            }
                        }
                    }
                }

                if (con.HasChildren)
                    Main_AnHaiCumChuDieuHuong(con);
            }
        }

        private void Main_ThuGonKhoangTrongMenu()
        {
            if (flowNavigation == null || flowNavigation.IsDisposed)
                return;

            // Chi giam khoang dem phia tren danh sach nut.
            Padding cu = flowNavigation.Padding;

            if (cu.Top != 2)
            {
                flowNavigation.Padding = new Padding(
                    cu.Left,
                    2,
                    cu.Right,
                    cu.Bottom);

                flowNavigation.PerformLayout();
            }
        }

        private void Main_DangKyHeaderNho()
        {
            if (mainDaDangKyHeaderNho || pnlBrand == null ||
                pnlHeader == null || pnlSidebar == null ||
                lblBrandTitle == null)
                return;

            mainDaDangKyHeaderNho = true;

            pnlBrand.SizeChanged +=
                (s, e) => Main_HenCanHeaderNho();

            pnlHeader.SizeChanged +=
                (s, e) => Main_HenCanHeaderNho();

            pnlSidebar.SizeChanged +=
                (s, e) => Main_HenCanHeaderNho();

            lblBrandTitle.FontChanged +=
                (s, e) => Main_HenCanHeaderNho();

            Main_HenCanHeaderNho();
        }

        private void Main_HenCanHeaderNho()
        {
            if (mainDaHenHeaderNho || IsDisposed || Disposing ||
                !IsHandleCreated)
                return;

            mainDaHenHeaderNho = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    mainDaHenHeaderNho = false;
                    Main_CanHeaderNho();
                }));
            }
            catch (InvalidOperationException)
            {
                mainDaHenHeaderNho = false;
            }
        }

        private void Main_CanHeaderNho()
        {
            if (IsDisposed || Disposing ||
                pnlBrand == null || pnlBrand.IsDisposed ||
                pnlSidebar == null || pnlSidebar.IsDisposed ||
                lblBrandTitle == null || lblBrandTitle.IsDisposed)
                return;

            if (pnlSidebar.Width < 210)
                return;

            const int coLogo = 56;
            const int xLogo = 12;
            const int xChu = 96;

            int chieuCao = pnlBrand.ClientSize.Height > 0
                ? pnlBrand.ClientSize.Height
                : 72;

            int yLogo = Math.Max(
                0,
                (chieuCao - coLogo) / 2);

            foreach (Control con in pnlBrand.Controls)
            {
                if (con is PictureBox &&
                    string.Equals(
                        con.Name,
                        "uiLogo",
                        StringComparison.Ordinal))
                {
                    con.SetBounds(
                        xLogo,
                        yLogo,
                        coLogo,
                        coLogo);

                    break;
                }
            }

            if (lblBrandLogo != null &&
                !lblBrandLogo.IsDisposed)
            {
                lblBrandLogo.SetBounds(
                    xLogo,
                    yLogo,
                    coLogo,
                    coLogo);
            }

            lblBrandTitle.AutoSize = false;

            if (!object.ReferenceEquals(
                lblBrandTitle.Font,
                mainFontTenNho))
            {
                lblBrandTitle.Font = mainFontTenNho;
            }

            lblBrandTitle.SetBounds(
                xChu,
                3,
                Math.Max(
                    110,
                    pnlBrand.ClientSize.Width - xChu - 10),
                65);
        }

        private void CanChinhToaDoGocDuoiPhai()
        {
            if (statusStrip == null || statusStrip.IsDisposed ||
                statusCoordinates == null || statusProjection == null ||
                statusGeneral == null || statusSpacer == null)
                return;

            statusStrip.SuspendLayout();

            try
            {
                if (statusStrip.Parent != this)
                {
                    if (statusStrip.Parent != null)
                        statusStrip.Parent.Controls.Remove(statusStrip);

                    Controls.Add(statusStrip);
                }

                statusStrip.Dock = DockStyle.Bottom;
                statusStrip.AutoSize = false;
                statusStrip.Height = 30;
                statusStrip.SizingGrip = false;

                statusStrip.BackColor =
                    Color.FromArgb(11, 25, 35);

                statusGeneral.Alignment =
                    ToolStripItemAlignment.Left;

                statusSpacer.Alignment =
                    ToolStripItemAlignment.Left;

                statusSpacer.Spring = true;

                statusCoordinates.Alignment =
                    ToolStripItemAlignment.Right;

                statusProjection.Alignment =
                    ToolStripItemAlignment.Right;

                statusCoordinates.TextAlign =
                    ContentAlignment.MiddleRight;

                statusProjection.TextAlign =
                    ContentAlignment.MiddleRight;

                statusCoordinates.ForeColor =
                    Color.FromArgb(20, 184, 166);

                statusProjection.ForeColor =
                    Color.FromArgb(148, 163, 184);

                statusCoordinates.Margin =
                    new Padding(8, 0, 12, 0);

                statusProjection.Margin =
                    new Padding(8, 0, 8, 0);

                statusCoordinates.Visible = true;
                statusProjection.Visible = true;

                statusStrip.Visible = true;
                statusStrip.BringToFront();
            }
            finally
            {
                statusStrip.ResumeLayout(true);
            }
        }

        // =====================================================
        // SUA LOI MAT LOGO KHI CHUYEN QUA LAI CAC PHAN HE
        // =====================================================

        private void Main_CapNhatBoCucToanTrang(
            bool hienHeaderBanDo)
        {
            if (pnlHeader == null || pnlHeader.IsDisposed ||
                pnlBrand == null || pnlBrand.IsDisposed ||
                pnlSidebarTop == null || pnlSidebarTop.IsDisposed ||
                pnlWorkspace == null || pnlWorkspace.IsDisposed ||
                tblHeaderActions == null || tblHeaderActions.IsDisposed)
                return;

            if (!mainDaLuuBoCucDauTrang)
            {
                mainDaLuuBoCucDauTrang = true;

                mainDockHeaderGoc = pnlHeader.Dock;

                mainCaoHeaderGoc = pnlHeader.Height > 0
                    ? pnlHeader.Height
                    : 72;

                // QUAN TRONG:
                // Khong lay pnlBrand.Parent lam parent goc.
                // Khi constructor chay, parent co the la pnlHeader.

                mainDockBrandGoc = DockStyle.Fill;

                mainRongBrandGoc = pnlSidebarTop.Width > 0
                    ? pnlSidebarTop.Width
                    : 316;

                mainChaBrandGoc = pnlSidebarTop;
            }

            bool trangNghiepVu = !hienHeaderBanDo;

            bool daDoiCheDo =
                mainDangHienTrangNghiepVu != trangNghiepVu;

            SuspendLayout();

            pnlHeader.SuspendLayout();
            pnlSidebarTop.SuspendLayout();
            pnlWorkspace.SuspendLayout();

            try
            {
                Header_DongGoiY();

                // Logo va ten he thong LUON o SidebarTop.
                // Khong chuyen ve Header khi quay lai ban do.

                if (!object.ReferenceEquals(
                    pnlBrand.Parent,
                    mainChaBrandGoc))
                {
                    if (pnlBrand.Parent != null)
                    {
                        pnlBrand.Parent.Controls.Remove(
                            pnlBrand);
                    }

                    mainChaBrandGoc.Controls.Add(pnlBrand);
                }

                // Bao dam khu vuc nhan dien khong bi an.

                pnlSidebarTop.Visible = true;

                if (pnlSidebarTop.Height < 72)
                    pnlSidebarTop.Height = 72;

                pnlBrand.Dock = mainDockBrandGoc;

                if (pnlSidebarTop.ClientSize.Width == 0)
                    pnlBrand.Width = mainRongBrandGoc;

                pnlBrand.Visible = true;
                pnlBrand.BringToFront();

                if (trangNghiepVu)
                {
                    // CAC TRANG NGHIEP VU:
                    // An Header ben phai.
                    // Khong an logo trong SidebarTop.

                    tblHeaderActions.Visible = false;

                    pnlHeader.Visible = false;
                    pnlHeader.Dock = DockStyle.None;
                    pnlHeader.Height = 0;

                    pnlWorkspace.Dock = DockStyle.Fill;
                }
                else
                {
                    // TRANG BAN DO:
                    // Khong dua pnlBrand ve pnlHeader.
                    // Header noi tren ban do duoc xu ly rieng.

                    pnlHeader.Height = mainCaoHeaderGoc;
                    pnlHeader.Dock = mainDockHeaderGoc;

                    tblHeaderActions.Visible = true;
                    pnlHeader.Visible = true;

                    pnlWorkspace.Dock = DockStyle.Fill;
                }

                mainDangHienTrangNghiepVu = trangNghiepVu;
            }
            finally
            {
                pnlWorkspace.ResumeLayout(true);
                pnlSidebarTop.ResumeLayout(true);
                pnlHeader.ResumeLayout(true);

                ResumeLayout(true);

                PerformLayout();
                pnlWorkspace.PerformLayout();

                // Can lai logo/ten sau khi chuyen phan he.
                // Chi goi khi giao dien moi da khoi tao.

                if (uiDaKhoiTao)
                    UiCanChuTieuDe();

                if (mainDaDangKyHeaderNho)
                    Main_HenCanHeaderNho();
            }

            GhiNhatKyBanDo(
                "WORKSPACE_FULL_HEIGHT | business=" +
                trangNghiepVu +
                " | modeChanged=" + daDoiCheDo +
                " | headerVisible=" + pnlHeader.Visible +
                " | headerDock=" + pnlHeader.Dock +
                " | workspaceTop=" + pnlWorkspace.Top +
                " | brandParent=" +
                (pnlBrand.Parent == null
                    ? "null"
                    : pnlBrand.Parent.Name));
        }

        private void Main_AnTieuDePhanTichBiTrung()
        {
            if (tblWorkspaceHeading != null &&
                !tblWorkspaceHeading.IsDisposed)
            {
                tblWorkspaceHeading.Visible = false;
                tblWorkspaceHeading.Height = 0;
            }

            if (lblPageTitle != null &&
                !lblPageTitle.IsDisposed)
            {
                lblPageTitle.Visible = false;
            }

            if (lblPageSubtitle != null &&
                !lblPageSubtitle.IsDisposed)
            {
                lblPageSubtitle.Visible = false;
            }

            if (lblStageBadge != null &&
                !lblStageBadge.IsDisposed)
            {
                lblStageBadge.Visible = false;
            }

            if (pnlWorkspace != null &&
                !pnlWorkspace.IsDisposed)
            {
                pnlWorkspace.PerformLayout();
            }
        }
    }
}