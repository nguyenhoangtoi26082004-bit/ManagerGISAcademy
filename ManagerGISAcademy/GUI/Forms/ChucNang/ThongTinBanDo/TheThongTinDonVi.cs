using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using ManagerGISAcademy.DTO;

using DonViQuanDoiBLL =
    ManagerGISAcademy.BLL.NghiepVuDonVi;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private void TtDv_ThemTrangThaiHoSo(
            DonViQuanDoi donVi,
            bool coToaDo)
        {
            Panel hang = new Panel
            {
                Width = 344,
                Height = 38,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 3)
            };

            Label a = new Label
            {
                Text = "● Hồ sơ đơn vị",
                Left = 0,
                Top = 3,
                Width = 108,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = TtDv_Mau(219, 250, 240),
                ForeColor = TtDv_Mau(0, 129, 109),
                Font = ttDvFontHuyHieuLight
            };

            Label b = new Label
            {
                Text = coToaDo
                    ? "⌖ Có tọa độ"
                    : "! Thiếu tọa độ",
                Left = 116,
                Top = 3,
                Width = 112,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = coToaDo
                    ? TtDv_Mau(225, 238, 255)
                    : TtDv_Mau(255, 238, 219),
                ForeColor = coToaDo
                    ? TtDv_Mau(31, 110, 203)
                    : TtDv_Mau(183, 110, 25),
                Font = ttDvFontHuyHieuLight
            };

            Label c = new Label
            {
                Text = "● " +
                    GiaTriHienThi(donVi.TrangThaiSSCD),
                Left = 235,
                Top = 3,
                Width = 111,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true,
                BackColor = TtDv_Mau(250, 239, 255),
                ForeColor = TtDv_Mau(126, 70, 183),
                Font = ttDvFontHuyHieuLight
            };

            TtDv_BoGoc(a, 8);
            TtDv_BoGoc(b, 8);
            TtDv_BoGoc(c, 8);

            meoThongTinDiem.SetToolTip(
                c,
                GiaTriHienThi(donVi.TrangThaiSSCD));

            hang.Controls.Add(a);
            hang.Controls.Add(b);
            hang.Controls.Add(c);

            flpThongTinDiem.Controls.Add(hang);
        }

        private void TtDv_ThemTieuDeNhom(
            string tieuDe)
        {
            Panel nhom = new Panel
            {
                Width = 344,
                Height = 37,
                Margin = new Padding(0, 7, 0, 1),
                BackColor = Color.Transparent
            };

            Panel vach = new Panel
            {
                Left = 0,
                Top = 12,
                Width = 4,
                Height = 14,
                BackColor = TtDv_Mau(12, 190, 170)
            };

            TtDv_BoGoc(vach, 2);

            Label ten = new Label
            {
                Text = tieuDe,
                Left = 12,
                Top = 6,
                Width = 305,
                Height = 27,
                ForeColor = TtDv_Mau(67, 111, 143),
                Font = ttDvFontNhomLight,
                TextAlign = ContentAlignment.MiddleLeft
            };

            nhom.Controls.Add(vach);
            nhom.Controls.Add(ten);

            flpThongTinDiem.Controls.Add(nhom);
        }

        private static readonly Color[] TtDv_MauNenThe =
        {
            Color.FromArgb(235, 251, 244),
            Color.FromArgb(235, 245, 255),
            Color.FromArgb(246, 239, 255),
            Color.FromArgb(255, 248, 231),
            Color.FromArgb(233, 250, 255)
        };

        private static readonly Color[] TtDv_MauVienThe =
        {
            Color.FromArgb(194, 240, 220),
            Color.FromArgb(203, 224, 255),
            Color.FromArgb(224, 207, 254),
            Color.FromArgb(247, 227, 185),
            Color.FromArgb(193, 234, 246)
        };

        private static readonly Color[] TtDv_MauNhanThe =
        {
            Color.FromArgb(17, 179, 146),
            Color.FromArgb(61, 142, 236),
            Color.FromArgb(148, 89, 228),
            Color.FromArgb(230, 162, 54),
            Color.FromArgb(32, 175, 210)
        };

        private Panel TtDv_TaoThe(
            string ten,
            object giaTri,
            int mau,
            int chieuRong,
            int chieuCao)
        {
            int loai =
                Math.Abs(mau) % TtDv_MauNenThe.Length;

            string noiDung =
                GiaTriHienThi(
                    Convert.ToString(giaTri));

            Panel the = new Panel
            {
                BackColor = TtDv_MauNenThe[loai],
                Width = chieuRong,
                Height = chieuCao,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            TtDv_BoGoc(the, 11);

            the.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (GraphicsPath p =
                    TtDv_DuongBoGoc(
                        new Rectangle(
                            0,
                            0,
                            Math.Max(1, the.Width - 1),
                            Math.Max(1, the.Height - 1)),
                        11))
                using (Pen vien =
                    new Pen(TtDv_MauVienThe[loai]))
                {
                    e.Graphics.DrawPath(vien, p);
                }

                using (SolidBrush butVachMau =
                    new SolidBrush(
                        TtDv_MauNhanThe[loai]))
                {
                    e.Graphics.FillRectangle(
                        butVachMau,
                        0,
                        12,
                        3,
                        Math.Max(2, the.Height - 24));
                }
            };

            Label nhan = new Label
            {
                Text = ten,
                Left = 12,
                Top = 8,
                Width = Math.Max(20, chieuRong - 22),
                Height = 18,
                ForeColor = TtDv_Mau(95, 128, 151),
                Font = ttDvFontNhanLight,
                AutoEllipsis = true
            };

            Label giaTriHienThi = new Label
            {
                Text = noiDung,
                Left = 12,
                Top = 28,
                Width = Math.Max(20, chieuRong - 24),
                Height = Math.Max(30, chieuCao - 34),
                ForeColor = TtDv_Mau(34, 64, 87),
                Font = ttDvFontGiaTriLight,
                AutoEllipsis = false
            };

            the.Controls.Add(nhan);
            the.Controls.Add(giaTriHienThi);

            the.Resize += (s, e) =>
            {
                nhan.Width = Math.Max(
                    20,
                    the.ClientSize.Width - 22);

                giaTriHienThi.Width = Math.Max(
                    20,
                    the.ClientSize.Width - 24);

                giaTriHienThi.Height = Math.Max(
                    30,
                    the.ClientSize.Height - 34);
            };

            meoThongTinDiem.SetToolTip(
                giaTriHienThi,
                noiDung);

            meoThongTinDiem.SetToolTip(nhan, ten);

            return the;
        }

        private void TtDv_ThemTheRong(
            string tieuDe,
            object giaTri,
            int mau)
        {
            string chuoi =
                GiaTriHienThi(
                    Convert.ToString(giaTri));

            int cao = Math.Max(
                71,
                Math.Min(
                    126,
                    55 + (chuoi.Length / 46) * 19));

            Panel the =
                TtDv_TaoThe(
                    tieuDe,
                    chuoi,
                    mau,
                    344,
                    cao);

            the.Margin = new Padding(0, 0, 0, 8);

            flpThongTinDiem.Controls.Add(the);
        }

        private void TtDv_ThemHaiThe(
            string tieuDe1,
            object giaTri1,
            int mau1,
            string tieuDe2,
            object giaTri2,
            int mau2)
        {
            string mot =
                GiaTriHienThi(
                    Convert.ToString(giaTri1));

            string hai =
                GiaTriHienThi(
                    Convert.ToString(giaTri2));

            int doDai = Math.Max(
                mot.Length,
                hai.Length);

            int cao = Math.Max(
                87,
                Math.Min(
                    141,
                    65 + (doDai / 23) * 17));

            TableLayoutPanel hang =
                new TableLayoutPanel
                {
                    ColumnCount = 2,
                    RowCount = 1,
                    Height = cao,
                    Width = 344,
                    BackColor = Color.Transparent,
                    Padding = Padding.Empty,
                    Margin = new Padding(0, 0, 0, 8)
                };

            hang.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            hang.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    50F));

            hang.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            Panel p1 =
                TtDv_TaoThe(
                    tieuDe1,
                    mot,
                    mau1,
                    165,
                    cao);

            Panel p2 =
                TtDv_TaoThe(
                    tieuDe2,
                    hai,
                    mau2,
                    165,
                    cao);

            p1.Dock = DockStyle.Fill;
            p2.Dock = DockStyle.Fill;

            p1.Margin = new Padding(0, 0, 5, 0);
            p2.Margin = new Padding(5, 0, 0, 0);

            hang.Controls.Add(p1, 0, 0);
            hang.Controls.Add(p2, 1, 0);

            flpThongTinDiem.Controls.Add(hang);
        }

        private void TtDv_XoaCacDongThongTin()
        {
            if (flpThongTinDiem == null ||
                flpThongTinDiem.IsDisposed)
                return;

            while (flpThongTinDiem.Controls.Count > 0)
            {
                Control cu =
                    flpThongTinDiem.Controls[0];

                flpThongTinDiem.Controls.Remove(cu);
                cu.Dispose();
            }

            flpThongTinDiem.AutoScrollPosition =
                System.Drawing.Point.Empty;
        }

        private void TtDv_CanChinhDongThongTin()
        {
            if (flpThongTinDiem == null ||
                flpThongTinDiem.IsDisposed)
                return;

            int rong = Math.Max(
                80,
                flpThongTinDiem.ClientSize.Width -
                flpThongTinDiem.Padding.Horizontal -
                SystemInformation.VerticalScrollBarWidth - 2);

            foreach (Control muc in
                flpThongTinDiem.Controls)
            {
                if (muc.Width != rong)
                    muc.Width = rong;
            }
        }

        private static string GiaTriHienThi(
            string giaTri)
        {
            return string.IsNullOrWhiteSpace(giaTri)
                ? "Chưa cập nhật"
                : giaTri.Trim();
        }

        private void GioiHanKhungThongTin()
        {
            if (ttDvCotHoSo == null ||
                ttDvCotHoSo.IsDisposed ||
                pnlThongTinDiem == null ||
                pnlThongTinDiem.IsDisposed ||
                pnlMapCard == null ||
                pnlMapCard.IsDisposed)
                return;

            int rong = Math.Min(
                436,
                Math.Max(
                    1,
                    pnlMapCard.ClientSize.Width - 300));

            if (ttDvCotHoSo.Width != rong)
                ttDvCotHoSo.Width = rong;

            const int cachThanhTrangThai = 10;
            int demDuoi = cachThanhTrangThai;

            if (statusStrip != null &&
                !statusStrip.IsDisposed &&
                statusStrip.Visible &&
                statusStrip.IsHandleCreated &&
                pnlMapCard.IsHandleCreated)
            {
                try
                {
                    System.Drawing.Point dauThanhTrangThai =
                        pnlMapCard.PointToClient(
                            statusStrip.PointToScreen(
                                System.Drawing.Point.Empty));

                    if (dauThanhTrangThai.Y > 0 &&
                        dauThanhTrangThai.Y <
                        pnlMapCard.ClientSize.Height)
                    {
                        demDuoi = Math.Max(
                            cachThanhTrangThai,
                            pnlMapCard.ClientSize.Height -
                            dauThanhTrangThai.Y +
                            cachThanhTrangThai);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Handle đang được tạo lại.
                }
            }

            Padding demMoi =
                new Padding(0, 0, 0, demDuoi);

            if (ttDvCotHoSo.Padding != demMoi)
                ttDvCotHoSo.Padding = demMoi;

            TtDv_CanChinhDongThongTin();
        }

        private void TtDv_SaoChepToaDo()
        {
            if (ttDvHoSoDangHienThi == null ||
                !DonViQuanDoiBLL.CoToaDoWgs84(
                    ttDvHoSoDangHienThi))
                return;

            try
            {
                Clipboard.SetText(
                    "WGS84 (EPSG:4326)\r\nKinh độ: " +
                    ttDvHoSoDangHienThi.KinhDo.ToString(
                        "F6",
                        CultureInfo.InvariantCulture) +
                    "\r\nVĩ độ: " +
                    ttDvHoSoDangHienThi.ViDo.ToString(
                        "F6",
                        CultureInfo.InvariantCulture));

                statusGeneral.Text =
                    "● Đã sao chép tọa độ WGS84 của đơn vị.";
            }
            catch (Exception ex)
            {
                statusGeneral.Text =
                    "● Không sao chép được tọa độ: " +
                    ex.Message;
            }
        }

        private void TtDv_DinhTamDonVi()
        {
            if (ttDvDiemDangHienThi == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            try
            {
                MapWinGIS.Extents cu =
                    banDoMapWinGIS.Extents as MapWinGIS.Extents;

                if (cu == null)
                {
                    throw new InvalidOperationException(
                        "Không đọc được phạm vi bản đồ.");
                }

                double rong = cu.xMax - cu.xMin;
                double cao = cu.yMax - cu.yMin;

                if (double.IsNaN(rong) ||
                    double.IsInfinity(rong) ||
                    rong <= 0d ||
                    double.IsNaN(cao) ||
                    double.IsInfinity(cao) ||
                    cao <= 0d)
                {
                    rong = cao = 2000d;
                }

                double x = ttDvDiemDangHienThi.X;
                double y = ttDvDiemDangHienThi.Y;

                MapWinGIS.Extents moi =
                    new MapWinGIS.Extents();

                moi.SetBounds(
                    x - rong / 2d,
                    y - cao / 2d,
                    0d,
                    x + rong / 2d,
                    y + cao / 2d,
                    0d);

                banDoMapWinGIS.Extents = moi;

                GioiHanKhungThongTin();

                statusGeneral.Text =
                    "● Đã đưa đơn vị về giữa bản đồ.";
            }
            catch (Exception ex)
            {
                statusGeneral.Text =
                    "● Không định tâm được đơn vị: " +
                    ex.Message;
            }
        }

        private void AnKhungThongTinDiem()
        {
            TtDv_DonHoSoDangHienThi(true);
        }

        private void TtDv_DonHoSoDangHienThi(
            bool dongCotHoSo)
        {
            ++ttDvPhienTraCuu;

            ttDvYeuCauDangCho = null;
            maDonViDangChon = null;
            ttDvHoSoDangHienThi = null;
            ttDvDiemDangHienThi = null;

            meoThongTinDiem.Active = false;

            if (pnlThongTinDiem != null &&
                !pnlThongTinDiem.IsDisposed)
            {
                if (dongCotHoSo)
                {
                    pnlThongTinDiem.Visible = false;

                    if (ttDvCotHoSo != null &&
                        !ttDvCotHoSo.IsDisposed)
                    {
                        ttDvCotHoSo.Visible = false;
                    }
                }

                lblTenDiem.Text =
                    "THÔNG TIN ĐƠN VỊ";

                meoThongTinDiem.SetToolTip(
                    lblTenDiem,
                    null);

                ttDvBtnHoSo.Enabled = false;
                ttDvBtnSaoChep.Enabled = false;
                ttDvBtnDinhTam.Enabled = false;

                flpThongTinDiem.SuspendLayout();

                try
                {
                    TtDv_XoaCacDongThongTin();
                }
                finally
                {
                    flpThongTinDiem.ResumeLayout(true);
                }
            }

            TtDv_DongMenuUngVien();
            TtDv_XoaDanhDau();
        }
    }
}