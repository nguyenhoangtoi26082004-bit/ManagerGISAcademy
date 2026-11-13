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

        private void UiCapNhatTrang()
        {
            UiCanMenu();

            bool laBanDo =
                phanHeHienTai == "Bản đồ GIS";

            bool laPhanTich =
                phanHeHienTai == "Phân tích không gian";

            tblWorkspaceHeading.Visible = laPhanTich;
            tblWorkspaceHeading.Height = laPhanTich ? 58 : 0;

            if (laBanDo)
            {
                tblContent.Padding = Padding.Empty;

                pnlMapCard.Margin = Padding.Empty;
                pnlMapCardHeader.Visible = false;
                pnlMapCardHeader.Height = 0;
                lblMapFooter.Visible = false;
                lblMapFooter.Height = 0;

                UiDatBangLopNoi();
                UiDuaThongTinVaoChuGiai();
                UiDungHeaderNoiTrenBanDo();
                UiCanBangLop();

                UiHenCanCongCuBanDo();
                UiKiemTraKieuNhan();
            }
            else
            {
                UiTraHeaderVeMacDinh();

                tblContent.Padding =
                    new Padding(14, 12, 14, 14);

                foreach (Control c in pnlPhanHeHost.Controls)
                    UiTrangTriCay(c);
            }
        }

        private void UiKhiDoiHienThiBangLop(
            object sender,
            EventArgs e)
        {
            if (uiDangCapNhatBangLop ||
                phanHeHienTai != "Bản đồ GIS")
                return;

            uiBangLopMo = pnlRightCard.Visible;
            UiHenCapNhatTrang();
        }
        private void UiDatBangLopNoi()
        {
            if (uiDangCapNhatBangLop ||
                pnlRightCard == null ||
                pnlRightCard.IsDisposed ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed)
                return;

            uiDangCapNhatBangLop = true;

            tblContent.SuspendLayout();
            pnlMapCanvas.SuspendLayout();

            try
            {
                if (pnlRightCard.Parent != pnlMapCanvas)
                {
                    if (pnlRightCard.Parent != null)
                    {
                        pnlRightCard.Parent.Controls.Remove(
                            pnlRightCard);
                    }

                    pnlMapCanvas.Controls.Add(pnlRightCard);
                    pnlRightCard.Dock = DockStyle.None;

                    UiBoGoc(pnlRightCard, 12);
                }

                if (tblContent.ColumnStyles.Count >= 2)
                {
                    tblContent.ColumnStyles[0].SizeType =
                        SizeType.Percent;
                    tblContent.ColumnStyles[0].Width = 100F;

                    tblContent.ColumnStyles[1].SizeType =
                        SizeType.Absolute;
                    tblContent.ColumnStyles[1].Width = 0F;

                    if (pnlMapCard.Parent == tblContent)
                        tblContent.SetColumnSpan(pnlMapCard, 2);
                }

                pnlRightCard.BackColor = uiThe;
                pnlRightHeader.BackColor = uiThe;
                pnlRightHeader.Height = 48;

                lblRightTitle.Text = "LỚP BẢN ĐỒ";
                lblRightTitle.ForeColor = uiChu;
                lblRightTag.Visible = false;

                pnlRightCard.Visible = uiBangLopMo;
                UiCanBangLop();
            }
            finally
            {
                pnlMapCanvas.ResumeLayout(true);
                tblContent.ResumeLayout(true);
                uiDangCapNhatBangLop = false;
            }
        }

        private Bitmap UiLayAnhNutMoBangLop()
        {
            if (uiAnhNutMoBangLop != null)
                return uiAnhNutMoBangLop;

            string root =
                AppDomain.CurrentDomain.BaseDirectory;

            string[] cacTep =
            {
                Path.Combine(
                    root, "Resources", "Icons", "map.png"),

                Path.Combine(
                    root, "Icons", "map.png"),

                Path.GetFullPath(Path.Combine(
                    root, "..", "..",
                    "Resources", "Icons", "map.png")),

                Path.GetFullPath(Path.Combine(
                    root, "..", "..", "..",
                    "Resources", "Icons", "map.png"))
            };

            foreach (string tep in cacTep)
            {
                try
                {
                    if (!File.Exists(tep))
                        continue;

                    using (WinImage goc = WinImage.FromFile(tep))
                    {
                        Bitmap anh = new Bitmap(
                            24, 24,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                        using (Graphics g = Graphics.FromImage(anh))
                        {
                            g.Clear(Color.Transparent);
                            g.InterpolationMode =
                                InterpolationMode.HighQualityBicubic;
                            g.CompositingQuality =
                                CompositingQuality.HighQuality;
                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            float tyLe = Math.Min(
                                22F / Math.Max(1, goc.Width),
                                22F / Math.Max(1, goc.Height));

                            int rong = Math.Max(
                                1,
                                (int)Math.Round(goc.Width * tyLe));

                            int cao = Math.Max(
                                1,
                                (int)Math.Round(goc.Height * tyLe));

                            g.DrawImage(
                                goc,
                                new Rectangle(
                                    (24 - rong) / 2,
                                    (24 - cao) / 2,
                                    rong,
                                    cao));
                        }

                        uiAnhNutMoBangLop = anh;
                        return uiAnhNutMoBangLop;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[UI_MAP_ICON] " + ex.Message);
                }
            }

            uiAnhNutMoBangLop = new Bitmap(
                UiLayBieuTuong(
                    "map",
                    Color.FromArgb(0, 163, 121)));

            System.Diagnostics.Debug.WriteLine(
                "[UI_MAP_ICON] Khong tim thay Resources\\Icons\\map.png");

            return uiAnhNutMoBangLop;
        }

        private void UiTaoNutDongBangLop()
        {
            if (pnlRightCard == null ||
                pnlRightCard.IsDisposed ||
                pnlRightHeader == null ||
                pnlRightHeader.IsDisposed ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed)
                return;

            if (uiNutDongLop == null ||
                uiNutDongLop.IsDisposed)
            {
                uiNutDongLop = new Button
                {
                    Name = "uiNutDongLop",
                    Text = "×",
                    Size = new Size(29, 29),
                    BackColor = uiTheNhat,
                    ForeColor = uiChu,

                    Font = new Font(
                        "Segoe UI", 11F, FontStyle.Bold),

                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    TabStop = false,
                    UseVisualStyleBackColor = false
                };

                uiNutDongLop.FlatAppearance.BorderSize = 0;

                uiNutDongLop.Click += (s, e) =>
                {
                    uiBangLopMo = false;
                    pnlRightCard.Visible = false;

                    UiTaoNutDongBangLop();
                    UiDatZoomDuoiCongCu();
                    UiHenCapNhatTrang();
                };

                pnlRightHeader.Controls.Add(uiNutDongLop);
                UiLamDepNut(uiNutDongLop, 7);
            }

            uiNutDongLop.Location = new WinPoint(
                Math.Max(
                    0,
                    pnlRightHeader.ClientSize.Width - 39),
                9);

            uiNutDongLop.BringToFront();

            if (uiNutMoBangLop == null ||
                uiNutMoBangLop.IsDisposed)
            {
                uiNutMoBangLop = new Button
                {
                    Name = "uiNutMoBangLop",
                    Text = string.Empty,
                    Size = new Size(35, 35),
                    Image = UiLayAnhNutMoBangLop(),
                    ImageAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    TabStop = false,
                    UseVisualStyleBackColor = false,
                    AccessibleName = "Mo bang lop ban do"
                };

                uiNutMoBangLop.FlatAppearance.BorderSize = 0;
                pnlMapCanvas.Controls.Add(uiNutMoBangLop);

                UiLamDepNut(uiNutMoBangLop, 8);

                uiNutMoBangLop.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(235, 248, 255);

                uiNutMoBangLop.FlatAppearance.MouseDownBackColor =
                    Color.FromArgb(218, 241, 250);

                uiMeo.SetToolTip(
                    uiNutMoBangLop,
                    "Mo bang lop ban do va chu giai");

                uiNutMoBangLop.Click += (s, e) =>
                {
                    if (pnlRightCard == null ||
                        pnlRightCard.IsDisposed)
                        return;

                    uiBangLopMo = true;

                    if (pnlRightCard.Parent != pnlMapCanvas)
                        UiDatBangLopNoi();

                    pnlRightCard.Visible = true;
                    UiCanBangLop();
                    pnlRightCard.BringToFront();
                    UiDatZoomDuoiCongCu();
                    UiHenCapNhatTrang();
                };
            }

            int x =
                pnlMapTools != null && !pnlMapTools.IsDisposed
                    ? pnlMapTools.Left
                    : Math.Max(
                        6,
                        pnlMapCanvas.ClientSize.Width - 47);

            int y =
                pnlMapTools != null && !pnlMapTools.IsDisposed
                    ? pnlMapTools.Bottom + 9
                    : 241;

            uiNutMoBangLop.SetBounds(x, y, 35, 35);

            uiNutMoBangLop.Visible =
                phanHeHienTai == "Bản đồ GIS" &&
                !pnlRightCard.Visible;

            if (uiNutMoBangLop.Visible)
                uiNutMoBangLop.BringToFront();
        }

        private void UiCanBangLop()
        {
            if (pnlRightCard == null ||
        pnlRightCard.IsDisposed ||
        pnlMapCanvas == null ||
        pnlMapCanvas.IsDisposed ||
        pnlRightCard.Parent != pnlMapCanvas)
            {
                return;
            }

            // Chiều rộng bảng lớp.
            int w = Math.Min(
                315,
                Math.Max(
                    240,
                    pnlMapCanvas.ClientSize.Width - 100));

            const int dichSangTrai = 36;

            int left = Math.Max(
                8,
                pnlMapCanvas.ClientSize.Width -
                w - 17 - dichSangTrai);

            // Đặt bảng lớp thấp hơn thanh công cụ phía trên.
            // Trước đây top = 17 làm bảng che các nút header.
            int top = Math.Min(
                90,
                Math.Max(
                    10,
                    pnlMapCanvas.ClientSize.Height - 185));

            // Giữ khoảng trống cuối bảng.
            int height = Math.Max(
                170,
                pnlMapCanvas.ClientSize.Height - top - 10);

            pnlRightCard.SetBounds(
                left,
                top,
                w,
                height);

            UiTaoNutDongBangLop();

            if (pnlRightCard.Visible)
            {
                pnlRightCard.BringToFront();
            }
        }
    }
}