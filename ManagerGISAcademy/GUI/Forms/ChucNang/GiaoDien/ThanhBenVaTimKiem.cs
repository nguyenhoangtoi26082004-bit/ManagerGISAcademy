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

        private void UiKhoiTaoSidebarCoTheKeo()
        {
            if (uiSidebarTayNam != null &&
                !uiSidebarTayNam.IsDisposed)
                return;

            uiSidebarTayNam = new Panel
            {
                Name = "uiSidebarTayNam",
                Dock = DockStyle.Right,
                Width = 8,
                Cursor = Cursors.SizeWE,
                BackColor = Color.FromArgb(36, 88, 71)
            };

            uiSidebarTayNam.MouseDown += UiSidebarBatDauKeo;
            uiSidebarTayNam.MouseMove += UiSidebarDangDiChuyen;
            uiSidebarTayNam.MouseUp += UiSidebarThaChuot;

            uiSidebarTayNam.DoubleClick +=
                (s, e) => UiDoiThuGonSidebar();

            uiMeo.SetToolTip(
                uiSidebarTayNam,
                "Giữ chuột trái và kéo để thay đổi chiều rộng; " +
                "nhấp đúp để thu gọn/mở ra");

            pnlSidebar.Controls.Add(uiSidebarTayNam);
            uiSidebarTayNam.BringToFront();

            uiSidebarNutThuGon = new Button
            {
                Text = "‹",
                Size = new Size(22, 27),
                BackColor = Color.FromArgb(30, 89, 69),
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI", 11F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };

            UiLamDepNut(uiSidebarNutThuGon, 7);

            uiSidebarNutThuGon.Click +=
                (s, e) => UiDoiThuGonSidebar();

            uiMeo.SetToolTip(
                uiSidebarNutThuGon,
                "Thu gọn / mở rộng thanh chức năng");

            pnlSidebarTop.Controls.Add(uiSidebarNutThuGon);
            uiSidebarNutThuGon.BringToFront();

            UiCanSidebarTheoChieuRong();
        }

        private void UiSidebarBatDauKeo(
            object sender,
            MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            uiSidebarDangKeo = true;
            uiSidebarXBatDauKeo = Cursor.Position.X;
            uiSidebarRongBatDauKeo = pnlSidebar.Width;

            uiSidebarTayNam.Capture = true;
        }

        private void UiSidebarDangDiChuyen(
            object sender,
            MouseEventArgs e)
        {
            if (!uiSidebarDangKeo ||
                pnlSidebar == null ||
                pnlSidebar.IsDisposed)
                return;

            int chenhLech =
                Cursor.Position.X - uiSidebarXBatDauKeo;

            int rong = Math.Max(
                UiSidebarRongGon,
                Math.Min(
                    UiSidebarRongToiDa,
                    uiSidebarRongBatDauKeo + chenhLech));

            if (pnlSidebar.Width != rong)
                pnlSidebar.Width = rong;
        }

        private void UiSidebarThaChuot(
            object sender,
            MouseEventArgs e)
        {
            if (!uiSidebarDangKeo)
                return;

            uiSidebarDangKeo = false;

            if (uiSidebarTayNam != null &&
                !uiSidebarTayNam.IsDisposed)
                uiSidebarTayNam.Capture = false;

            int rong = pnlSidebar.Width;

            if (rong < 135)
            {
                pnlSidebar.Width = UiSidebarRongGon;
            }
            else
            {
                pnlSidebar.Width = Math.Max(
                    210,
                    Math.Min(UiSidebarRongToiDa, rong));
                uiSidebarRongDaMo = pnlSidebar.Width;
            }

            UiCanSidebarTheoChieuRong();
        }
        private void UiDoiThuGonSidebar()
        {
            if (pnlSidebar == null || pnlSidebar.IsDisposed)
                return;

            bool dangGon = pnlSidebar.Width < 210;

            if (dangGon)
            {
                pnlSidebar.Width = Math.Max(
                    210,
                    Math.Min(UiSidebarRongToiDa, uiSidebarRongDaMo));
            }
            else
            {
                uiSidebarRongDaMo = pnlSidebar.Width;
                pnlSidebar.Width = UiSidebarRongGon;
            }

            UiCanSidebarTheoChieuRong();
        }

        private void UiCanNutThuGonSidebar()
        {
            if (uiSidebarNutThuGon == null ||
                uiSidebarNutThuGon.IsDisposed ||
                pnlSidebarTop == null ||
                pnlSidebarTop.IsDisposed)
                return;

            uiSidebarNutThuGon.Location = new WinPoint(
                Math.Max(3, pnlSidebarTop.ClientSize.Width - 24),
                Math.Max(
                    5,
                    (pnlSidebarTop.ClientSize.Height - 27) / 2));

            uiSidebarNutThuGon.Text =
                pnlSidebar.Width < 210 ? "›" : "‹";

            uiSidebarNutThuGon.BringToFront();
        }

        private void UiCanSidebarTheoChieuRong()
        {
            if (pnlSidebar == null || pnlSidebar.IsDisposed)
                return;

            uiSidebarDangThuGon = pnlSidebar.Width < 210;

            if (pnlSidebarBottom != null &&
                !pnlSidebarBottom.IsDisposed)
            {
                pnlSidebarBottom.Visible =
                    !uiSidebarDangThuGon;
            }

            UiCanNutThuGonSidebar();
            UiCanChuTieuDe();
            UiCanMenu();

            if (uiSidebarTayNam != null &&
                !uiSidebarTayNam.IsDisposed)
                uiSidebarTayNam.BringToFront();
        }

        private void UiMauTrangThaiPostGIS()
        {
            if (lblHeaderDbStatus == null ||
                lblHeaderDbStatus.IsDisposed)
                return;

            string t =
                lblHeaderDbStatus.Text ?? string.Empty;

            bool loi =
                t.IndexOf(
                    "Không kết nối",
                    StringComparison.OrdinalIgnoreCase) >= 0 ||
                t.IndexOf(
                    "Lỗi",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            bool ok = !loi &&
                t.IndexOf(
                    "Đã kết nối",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            Color mauChu = loi
                ? Color.FromArgb(185, 58, 74)
                : ok
                    ? Color.FromArgb(6, 120, 88)
                    : Color.FromArgb(165, 113, 37);

            Color mauNen = loi
                ? Color.FromArgb(255, 234, 237)
                : ok
                    ? Color.FromArgb(225, 250, 239)
                    : Color.FromArgb(255, 247, 226);

            if (lblHeaderDbStatus.ForeColor != mauChu)
                lblHeaderDbStatus.ForeColor = mauChu;

            if (lblHeaderDbStatus.BackColor != mauNen)
                lblHeaderDbStatus.BackColor = mauNen;

            lblHeaderDbStatus.Invalidate();
        }

        private static GraphicsPath UiDuongBoGoc(
            Rectangle r,
            int banKinh)
        {
            GraphicsPath path = new GraphicsPath();

            int d = Math.Min(
                banKinh * 2,
                Math.Min(r.Width, r.Height));

            if (d < 2)
            {
                path.AddRectangle(r);
                return path;
            }

            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);

            path.CloseFigure();
            return path;
        }

        private void UiVeHopTimKiem(
            object sender,
            PaintEventArgs e)
        {
            Panel p = sender as Panel;

            if (p == null ||
                p.Width < 4 ||
                p.Height < 4)
                return;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (GraphicsPath path = UiDuongBoGoc(
                new Rectangle(
                    0, 0, p.Width - 1, p.Height - 1), 19))
            using (Pen vien = new Pen(
                Color.FromArgb(177, 226, 216), 1.2F))
            {
                e.Graphics.DrawPath(vien, path);
            }

            // Kính lúp màu xanh ngọc.
            using (Pen pen = new Pen(
                Color.FromArgb(5, 172, 137), 2F))
            {
                e.Graphics.DrawEllipse(
                    pen, 17, (p.Height - 16) / 2, 11, 11);

                e.Graphics.DrawLine(
                    pen,
                    27,
                    (p.Height - 16) / 2 + 10,
                    32,
                    (p.Height - 16) / 2 + 15);
            }
        }

        private static void UiVeNenNutGradient(
            Button nut,
            PaintEventArgs e,
            Color dau,
            Color cuoi,
            int banKinh)
        {
            if (nut.Width < 4 || nut.Height < 4)
                return;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(
                0, 0, nut.Width, nut.Height);

            using (GraphicsPath path =
                UiDuongBoGoc(r, banKinh))
            using (LinearGradientBrush nen =
                new LinearGradientBrush(
                    r,
                    dau,
                    cuoi,
                    LinearGradientMode.Horizontal))
            {
                e.Graphics.FillPath(nen, path);
            }
        }

        private void UiVeNutTimKiem(
            object sender,
            PaintEventArgs e)
        {
            Button nut = sender as Button;
            if (nut == null)
                return;

            UiVeNenNutGradient(
                nut,
                e,
                Color.FromArgb(5, 178, 137),
                Color.FromArgb(29, 209, 163),
                16);

            TextRenderer.DrawText(
                e.Graphics,
                "TÌM",
                nut.Font,
                nut.ClientRectangle,
                Color.White,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);
        }

        private void UiVeNutThemDonVi(
            object sender,
            PaintEventArgs e)
        {
            Button nut = sender as Button;
            if (nut == null)
                return;

            UiVeNenNutGradient(
                nut,
                e,
                Color.FromArgb(38, 129, 234),
                Color.FromArgb(79, 165, 249),
                19);

            int cy = nut.Height / 2;

            using (Pen pen =
                new Pen(Color.White, 1.8F))
            {
                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                e.Graphics.DrawEllipse(
                    pen, 13, cy - 7, 14, 14);

                e.Graphics.DrawLine(pen, 17, cy, 23, cy);
                e.Graphics.DrawLine(pen, 20, cy - 3, 20, cy + 3);
            }

            Rectangle ten = new Rectangle(
                31, 0,
                Math.Max(20, nut.Width - 34),
                nut.Height);

            TextRenderer.DrawText(
                e.Graphics,
                "THÊM ĐƠN VỊ",
                nut.Font,
                ten,
                Color.White,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.NoPadding);
        }
        private void UiVeTrangThaiPostGIS(
    object sender,
    PaintEventArgs e)
        {
            Label nhan = sender as Label;

            if (nhan == null ||
                nhan.Width < 4 ||
                nhan.Height < 4)
                return;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            string raw = nhan.Text ?? string.Empty;

            bool loi =
                raw.IndexOf(
                    "Không kết nối",
                    StringComparison.OrdinalIgnoreCase) >= 0 ||
                raw.IndexOf(
                    "Lỗi",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            bool ok = !loi &&
                raw.IndexOf(
                    "Đã kết nối",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            Color nen = loi
                ? Color.FromArgb(255, 234, 237)
                : ok
                    ? Color.FromArgb(225, 250, 239)
                    : Color.FromArgb(255, 247, 226);

            Color vien = loi
                ? Color.FromArgb(248, 191, 199)
                : ok
                    ? Color.FromArgb(166, 226, 203)
                    : Color.FromArgb(237, 206, 146);

            Color chu = loi
                ? Color.FromArgb(173, 51, 69)
                : ok
                    ? Color.FromArgb(5, 120, 90)
                    : Color.FromArgb(165, 111, 36);

            Rectangle r = new Rectangle(
                0, 0, nhan.Width - 1, nhan.Height - 1);

            using (GraphicsPath path =
                UiDuongBoGoc(r, 19))
            using (SolidBrush brush = new SolidBrush(nen))
            using (Pen pen = new Pen(vien, 1.1F))
            {
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            }

            using (SolidBrush dot = new SolidBrush(chu))
            {
                e.Graphics.FillEllipse(
                    dot,
                    14,
                    nhan.Height / 2 - 4,
                    8,
                    8);
            }

            string ten = raw
                .Replace("●", "")
                .Replace("•", "")
                .Trim();

            if (string.IsNullOrWhiteSpace(ten))
                ten = "PostGIS";

            Rectangle textRect = new Rectangle(
                28, 0,
                Math.Max(25, nhan.Width - 34),
                nhan.Height);

            TextRenderer.DrawText(
                e.Graphics,
                ten,
                nhan.Font,
                textRect,
                chu,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPadding);
        }

        private void UiTaoHopTimKiem()
        {
            if (uiHopTimKiem != null &&
                !uiHopTimKiem.IsDisposed)
                return;

            if (txtQuickSearch.Parent != null)
            {
                txtQuickSearch.Parent.Controls.Remove(
                    txtQuickSearch);
            }

            if (btnQuickSearch.Parent != null)
            {
                btnQuickSearch.Parent.Controls.Remove(
                    btnQuickSearch);
            }

            uiHopTimKiem = new Panel
            {
                Name = "uiHopTimKiem",
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 15, 0, 15),
                BackColor = Color.White
            };

            UiBoGoc(uiHopTimKiem, 20);
            uiHopTimKiem.Paint += UiVeHopTimKiem;

            tblHeaderActions.Controls.Add(
                uiHopTimKiem, 0, 0);

            txtQuickSearch.BorderStyle = BorderStyle.None;
            txtQuickSearch.BackColor = Color.White;
            txtQuickSearch.ForeColor =
                Color.FromArgb(31, 66, 86);

            txtQuickSearch.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            txtQuickSearch.Dock = DockStyle.None;

            btnQuickSearch.Dock = DockStyle.None;
            btnQuickSearch.Text = "TÌM";
            btnQuickSearch.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            btnQuickSearch.BackColor =
                Color.FromArgb(6, 181, 138);

            btnQuickSearch.ForeColor = Color.White;

            UiLamDepNut(btnQuickSearch, 16);
            btnQuickSearch.Paint += UiVeNutTimKiem;

            uiGoiYTim = new Label
            {
                Text = "Tìm kiếm đơn vị, mã hồ sơ...",
                AutoSize = false,

                ForeColor = Color.FromArgb(
                    125, 155, 166),

                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.IBeam
            };

            uiGoiYTim.Click +=
                (s, a) => txtQuickSearch.Focus();

            txtQuickSearch.TextChanged +=
                (s, a) => UiCanHopTimMoi();

            txtQuickSearch.GotFocus +=
                (s, a) => UiCanHopTimMoi();

            txtQuickSearch.LostFocus +=
                (s, a) => UiCanHopTimMoi();

            uiHopTimKiem.Controls.Add(txtQuickSearch);
            uiHopTimKiem.Controls.Add(uiGoiYTim);
            uiHopTimKiem.Controls.Add(btnQuickSearch);

            uiHopTimKiem.Resize +=
                (s, a) => UiCanHopTimMoi();

            UiCanHopTimMoi();
        }

        private void UiCanHopTimMoi()
        {
            if (uiHopTimKiem == null ||
                uiHopTimKiem.IsDisposed ||
                txtQuickSearch == null ||
                txtQuickSearch.IsDisposed ||
                btnQuickSearch == null ||
                btnQuickSearch.IsDisposed ||
                uiGoiYTim == null ||
                uiGoiYTim.IsDisposed)
                return;

            int w = uiHopTimKiem.ClientSize.Width;
            int h = uiHopTimKiem.ClientSize.Height;

            if (w < 60 || h < 20)
                return;

            const int rongNut = 66;
            const int lePhai = 5;
            const int leText = 40;

            int caoText = Math.Min(
                25,
                Math.Max(
                    18, txtQuickSearch.PreferredHeight));

            int y = Math.Max(
                3, (h - caoText) / 2);

            int rongText = Math.Max(
                32,
                w - rongNut - leText - lePhai - 13);

            txtQuickSearch.SetBounds(
                leText, y, rongText, caoText);

            uiGoiYTim.SetBounds(
                leText, y, rongText, caoText);

            btnQuickSearch.SetBounds(
                Math.Max(10, w - rongNut - lePhai),
                5,
                rongNut,
                Math.Max(26, h - 10));

            uiGoiYTim.Visible =
                txtQuickSearch.TextLength == 0 &&
                !txtQuickSearch.Focused;

            if (uiGoiYTim.Visible)
                uiGoiYTim.BringToFront();

            btnQuickSearch.BringToFront();
            uiHopTimKiem.Invalidate();
        }
    }
}