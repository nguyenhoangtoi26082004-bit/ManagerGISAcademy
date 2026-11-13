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
        private void UiCanMenu()
        {
            if (uiDangCanMenu ||
                flowNavigation == null ||
                flowNavigation.IsDisposed ||
                pnlSidebar == null ||
                pnlSidebar.IsDisposed)
                return;

            uiDangCanMenu = true;

            try
            {
                Button[] nut =
                {
                    btnNavOverview,
                    btnNavMap,
                    btnNavSearch,
                    btnNavManage,
                    btnNavAnalysis,
                    btnNavReports,
                    btnNavValidate,
                    btnNavAdmin
                };

                string[] ten =
                {
                    "Tổng quan",
                    "Bản đồ GIS",
                    "Tra cứu đơn vị",
                    "Quản lý dữ liệu",
                    "Phân tích không gian",
                    "Thống kê, báo cáo",
                    "Kiểm tra dữ liệu",
                    "Quản trị hệ thống"
                };

                string[] icon =
                {
                    "dashboard",
                    "map",
                    "search",
                    "folder",
                    "analysis",
                    "chart",
                    "shield",
                    "server"
                };

                bool gon = pnlSidebar.Width < 210;

                flowNavigation.Padding = gon
                    ? new Padding(8, 8, 7, 3)
                    : new Padding(14, 8, 14, 3);

                int w = Math.Max(
                    34,
                    flowNavigation.ClientSize.Width -
                    flowNavigation.Padding.Horizontal - 3);

                for (int i = 0; i < nut.Length; i++)
                {
                    Button b = nut[i];

                    if (b == null || b.IsDisposed)
                        continue;

                    bool chon = b == nutDangChon;

                    Color mau = chon
                        ? Color.White
                        : Color.FromArgb(192, 211, 197);

                    b.Text = gon
                        ? string.Empty
                        : "    " + ten[i];

                    b.Image = UiLayBieuTuong(icon[i], mau);

                    b.ImageAlign = gon
                        ? ContentAlignment.MiddleCenter
                        : ContentAlignment.MiddleLeft;

                    b.TextImageRelation = gon
                        ? TextImageRelation.Overlay
                        : TextImageRelation.ImageBeforeText;

                    b.Width = w;
                    b.Height = 43;

                    b.Padding = gon
                        ? Padding.Empty
                        : new Padding(16, 0, 4, 0);

                    b.Margin = new Padding(0, 0, 0, 3);

                    b.TextAlign = gon
                        ? ContentAlignment.MiddleCenter
                        : ContentAlignment.MiddleLeft;

                    uiMeo.SetToolTip(b, ten[i]);

                    b.Font = chon
                        ? uiFontMenuChon
                        : uiFontMenu;

                    b.ForeColor = mau;
                    b.BackColor = chon
                        ? Color.FromArgb(22, 133, 103)
                        : uiThanh;

                    UiLamDepNut(b, 10);
                }
            }
            finally
            {
                uiDangCanMenu = false;
            }
        }

        private void UiTheoDoiPhanHe()
        {
            pnlPhanHeHost.ControlAdded += (s, e) =>
            {
                UiTrangTriCay(e.Control);
                UiHenCapNhatTrang();
            };

            foreach (Control c in pnlPhanHeHost.Controls)
                UiTrangTriCay(c);
        }

        private void UiTrangTriCay(Control goc)
        {
            if (goc == null ||
                goc.IsDisposed ||
                !uiDaTrangTri.Add(goc))
                return;

            if (goc is AxHost)
                return;

            // Trang thiet ke v2 tu dinh kieu: khong ghi de mau/bo goc/bang.
            if (string.Equals(goc.Tag as string, "v2", StringComparison.Ordinal))
                return;

            Button b = goc as Button;
            DataGridView bang = goc as DataGridView;
            TextBox text = goc as TextBox;

            if (b != null)
            {
                UiLamDepNut(b, 8);
            }
            else if (bang != null)
            {
                UiLamDepBang(bang);
            }
            else if (text != null)
            {
                if (!text.ReadOnly)
                    text.BackColor = uiThe;

                text.ForeColor = uiChu;
            }
            else if (goc is ComboBox ||
                     goc is NumericUpDown)
            {
                goc.BackColor = uiThe;
                goc.ForeColor = uiChu;
            }

            goc.ControlAdded +=
                (s, e) => UiTrangTriCay(e.Control);

            goc.Disposed +=
                (s, e) => uiDaTrangTri.Remove(goc);

            foreach (Control con in goc.Controls)
                UiTrangTriCay(con);
        }

        private void UiLamDepBang(DataGridView d)
        {
            d.EnableHeadersVisualStyles = false;
            d.BackgroundColor = uiThe;
            d.BorderStyle = BorderStyle.None;
            d.RowHeadersVisible = false;
            d.GridColor = uiVien;

            d.ColumnHeadersDefaultCellStyle.BackColor =
                uiTheNhat;

            d.ColumnHeadersDefaultCellStyle.ForeColor = uiChu;
            d.ColumnHeadersDefaultCellStyle.Font =
                uiFontMenuChon;

            d.DefaultCellStyle.BackColor = uiThe;
            d.DefaultCellStyle.ForeColor = uiChu;

            d.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(224, 244, 232);

            d.DefaultCellStyle.SelectionForeColor = uiChu;
            d.AlternatingRowsDefaultCellStyle.BackColor =
                uiTheNhat;

            d.RowTemplate.Height = 35;
        }

        private void UiLamDepNut(Button b, int radius)
        {
            if (b == null || b.IsDisposed)
                return;

            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.UseVisualStyleBackColor = false;
            b.Cursor = Cursors.Hand;

            b.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(39, 159, 123);

            b.FlatAppearance.MouseDownBackColor = uiXanh;

            UiBoGoc(b, radius);
        }

        private void UiBoGoc(Control c, int radius)
        {
            if (c == null || c.IsDisposed)
                return;

            UiApDungBoGoc(c, radius);

            if (!uiDaBoGoc.Add(c))
                return;

            c.Resize +=
                (s, e) => UiApDungBoGoc(c, radius);

            c.Disposed +=
                (s, e) => uiDaBoGoc.Remove(c);
        }

        private static void UiApDungBoGoc(
            Control c,
            int radius)
        {
            if (c == null ||
                c.IsDisposed ||
                c.Width < 3 ||
                c.Height < 3)
                return;

            int d = Math.Max(
                2,
                Math.Min(
                    radius * 2,
                    Math.Min(c.Width, c.Height)));

            Rectangle r = new Rectangle(
                0, 0, c.Width - 1, c.Height - 1);

            using (GraphicsPath p = new GraphicsPath())
            {
                p.AddArc(
                    r.Left, r.Top, d, d, 180, 90);

                p.AddArc(
                    r.Right - d, r.Top, d, d, 270, 90);

                p.AddArc(
                    r.Right - d, r.Bottom - d, d, d, 0, 90);

                p.AddArc(
                    r.Left, r.Bottom - d, d, d, 90, 90);

                p.CloseFigure();

                Region cu = c.Region;
                c.Region = new Region(p);

                if (cu != null)
                    cu.Dispose();
            }
        }
        private void UiTaoTheThongTinBanDo()
        {
            if (uiTheBanDo != null &&
                !uiTheBanDo.IsDisposed)
                return;

            uiTheBanDo = new Panel
            {
                Name = "uiTheBanDo",
                Size = new Size(292, 81),
                BackColor = uiThe
            };

            uiTieuDeBanDo = new Label
            {
                Text = "Bản đồ Hà Nội",
                Font = new Font(
                    "Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = uiChu,
                BackColor = Color.Transparent,
                Location = new WinPoint(15, 10),
                Size = new Size(166, 25)
            };

            uiMoTaBanDo = new Label
            {
                Text = "EPSG:32648",
                Font = new Font("Segoe UI", 8F),
                ForeColor = uiXanh,
                BackColor = Color.Transparent,
                Location = new WinPoint(190, 13),
                Size = new Size(94, 20)
            };

            uiNhanTuyChon = new Label
            {
                Text = "Nhãn đơn vị",
                Font = new Font(
                    "Segoe UI", 8F, FontStyle.Bold),
                ForeColor = uiChuPhu,
                BackColor = Color.Transparent,
                Location = new WinPoint(15, 48),
                Size = new Size(112, 21)
            };

            uiCheDoHienNhan = new ComboBox
            {
                Name = "uiCheDoHienNhan",
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8.5F),
                BackColor = uiTheNhat,
                ForeColor = uiChu,
                Location = new WinPoint(139, 44),
                Size = new Size(140, 26),
                IntegralHeight = false
            };

            uiCheDoHienNhan.Items.Add("Tự động");
            uiCheDoHienNhan.Items.Add("Hiện tất cả");
            uiCheDoHienNhan.SelectedIndex = 0;

            uiCheDoHienNhan.SelectedIndexChanged +=
                (s, e) => UiCapNhatCheDoNhanTheoYeuCau();

            uiMeo.SetToolTip(
                uiCheDoHienNhan,
                "Tự động: chỉ hiển thị nhãn khi tỷ lệ " +
                "khoảng 1:200.000 hoặc gần hơn.");

            uiTheBanDo.Controls.Add(uiTieuDeBanDo);
            uiTheBanDo.Controls.Add(uiMoTaBanDo);
            uiTheBanDo.Controls.Add(uiNhanTuyChon);
            uiTheBanDo.Controls.Add(uiCheDoHienNhan);

            UiBoGoc(uiTheBanDo, 10);
        }

        private void UiDuaThongTinVaoChuGiai()
        {
            if (uiTheBanDo == null ||
                uiTheBanDo.IsDisposed ||
                pnlNoiDungChuGiai == null ||
                pnlNoiDungChuGiai.IsDisposed ||
                flpCacLopChuGiai == null ||
                flpCacLopChuGiai.IsDisposed ||
                lblThongKeChuGiai == null ||
                lblThongKeChuGiai.IsDisposed)
                return;

            if (uiTheBanDo.Parent != pnlNoiDungChuGiai)
            {
                if (uiTheBanDo.Parent != null)
                    uiTheBanDo.Parent.Controls.Remove(uiTheBanDo);

                pnlNoiDungChuGiai.SuspendLayout();

                try
                {
                    flpCacLopChuGiai.Dock = DockStyle.None;
                    lblThongKeChuGiai.Dock = DockStyle.None;
                    uiTheBanDo.Dock = DockStyle.None;

                    pnlNoiDungChuGiai.Controls.Add(uiTheBanDo);

                    uiTheBanDo.BackColor =
                        Color.FromArgb(27, 49, 65);

                    uiTieuDeBanDo.ForeColor = Color.White;
                    uiMoTaBanDo.ForeColor =
                        Color.FromArgb(52, 211, 153);

                    uiNhanTuyChon.ForeColor =
                        Color.FromArgb(187, 213, 203);

                    uiCheDoHienNhan.BackColor = uiTheNhat;
                    uiCheDoHienNhan.ForeColor = uiChu;
                    uiTheBanDo.Height = 82;
                }
                finally
                {
                    pnlNoiDungChuGiai.ResumeLayout(true);
                }
            }

            if (!uiDaGanCanNoiDungChuGiai)
            {
                uiDaGanCanNoiDungChuGiai = true;

                pnlNoiDungChuGiai.Resize +=
                    UiKhiChuGiaiDoiKichThuoc;
            }

            UiCanNoiDungChuGiai();
        }

        private void UiKhiChuGiaiDoiKichThuoc(
            object sender,
            EventArgs e)
        {
            UiCanNoiDungChuGiai();
        }

        private void UiCanNoiDungChuGiai()
        {
            if (pnlNoiDungChuGiai == null ||
                pnlNoiDungChuGiai.IsDisposed ||
                uiTheBanDo == null ||
                uiTheBanDo.IsDisposed ||
                flpCacLopChuGiai == null ||
                flpCacLopChuGiai.IsDisposed ||
                lblThongKeChuGiai == null ||
                lblThongKeChuGiai.IsDisposed)
                return;

            int w = pnlNoiDungChuGiai.ClientSize.Width;
            int h = pnlNoiDungChuGiai.ClientSize.Height;

            if (w <= 0 || h <= 0)
                return;

            Padding p = pnlNoiDungChuGiai.Padding;
            int left = p.Left;
            int top = p.Top;
            int width = Math.Max(1, w - p.Horizontal);

            const int footerHeight = 34;
            const int cardHeight = 82;

            uiTheBanDo.SetBounds(
                left, top, width, cardHeight);

            uiCheDoHienNhan.Left =
                Math.Max(124, width - 148);

            uiCheDoHienNhan.Width =
                Math.Max(
                    110,
                    width - uiCheDoHienNhan.Left - 9);

            uiMoTaBanDo.Left =
                Math.Max(168, width - 98);

            uiMoTaBanDo.Width =
                Math.Max(
                    80,
                    width - uiMoTaBanDo.Left - 4);

            int listTop = top + cardHeight + 8;

            int footerTop = Math.Max(
                listTop,
                h - p.Bottom - footerHeight);

            lblThongKeChuGiai.SetBounds(
                left, footerTop, width, footerHeight);

            flpCacLopChuGiai.SetBounds(
                left,
                listTop,
                width,
                Math.Max(0, footerTop - listTop - 4));

            uiTheBanDo.BringToFront();
        }

        private void UiHenCapNhatTrang()
        {
            if (!IsHandleCreated ||
                IsDisposed ||
                Disposing ||
                uiChoCapNhatTrang)
                return;

            uiChoCapNhatTrang = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    uiChoCapNhatTrang = false;

                    if (!IsDisposed && !Disposing)
                        UiCapNhatTrang();
                }));
            }
            catch (InvalidOperationException)
            {
                uiChoCapNhatTrang = false;
            }
        }
    }
}