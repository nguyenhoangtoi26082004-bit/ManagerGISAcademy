using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WinLabel = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Chu giai ban do GIS hien dai.
    ///
    /// - Nhan biet ranh gioi Ha Noi, quan/huyen.
    /// - Hien thi diem quan doi va anh ve tinh.
    /// - Bat/tat lop truc tiep tu chu giai.
    /// - Dong bo danh sach lop MapWinGIS.
    /// - Khong sua du lieu PostGIS.
    /// - Gop chu giai vao bang LOP BAN DO.
    ///
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {

        // =====================================================
        // 8. DEM DIEM DON VI POSTGIS
        // =====================================================

        private string LayThongTinSoDiem()
        {
            try
            {
                if (handleLopDiemPostGIS >= 0 &&
                    ketNoiMapWinGIS.CoLop(
                        handleLopDiemPostGIS))
                {
                    MapWinGIS.Shapefile shp =
                        banDoMapWinGIS.get_Shapefile(
                            handleLopDiemPostGIS);

                    if (shp != null)
                    {
                        return shp.NumShapes +
                            " điểm";
                    }
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "MAP_LEGEND_COUNT_ERROR | " +
                    ex.Message);
            }

            return "Lớp điểm";
        }

        // =====================================================
        // 9. TAO THE CHU GIAI
        // =====================================================
        private Panel TaoTheChuGiai(
            int handle,
            int loai,
            string ten,
            string moTa,
            Color mau,
            bool hienThi)
        {
            // Màu nền tùy loại lớp.

            Color mauNenThe = !hienThi
                ? Color.FromArgb(
                    241, 246, 250)
                : loai == CG_DON_VI
                    ? Color.FromArgb(
                        218, 255, 237)
                    : loai == CG_OSM
                        ? Color.FromArgb(
                            221, 241, 255)
                        : loai == CG_RASTER
                            ? Color.FromArgb(
                                255, 239, 213)
                            : loai == CG_RANH_TINH
                                ? Color.FromArgb(
                                    223, 255, 247)
                                : loai == CG_RANH_HUYEN
                                    ? Color.FromArgb(
                                        255, 249, 219)
                                    : Color.FromArgb(
                                        244, 234, 255);

            Color mauVienThe = !hienThi
                ? Color.FromArgb(
                    208, 224, 234)
                : loai == CG_DON_VI
                    ? Color.FromArgb(
                        123, 223, 181)
                    : loai == CG_OSM
                        ? Color.FromArgb(
                            133, 202, 246)
                        : loai == CG_RASTER
                            ? Color.FromArgb(
                                240, 190, 127)
                            : loai == CG_RANH_TINH
                                ? Color.FromArgb(
                                    126, 224, 199)
                                : loai == CG_RANH_HUYEN
                                    ? Color.FromArgb(
                                        238, 210, 133)
                                    : Color.FromArgb(
                                        197, 169, 241);

            Panel the = new Panel
            {
                Width = 262,
                Height = 62,

                Margin = new Padding(
                    0, 0, 0, 7),

                BackColor = mauNenThe
            };

            Panel vach = new Panel
            {
                Dock = DockStyle.Left,
                Width = 3,

                BackColor = hienThi
                    ? mau
                    : Color.FromArgb(
                        88, 106, 125)
            };

            Panel khungKyHieu = new Panel
            {
                Left = 11,
                Top = 12,

                Width = 38,
                Height = 38,

                BackColor = hienThi
                    ? Color.White
                    : Color.FromArgb(
                        228, 238, 245)
            };

            khungKyHieu.Paint += (s, e) =>
            {
                ChuGiai_VeKyHieu(
                    e.Graphics,
                    khungKyHieu.ClientRectangle,
                    loai,
                    hienThi
                        ? mau
                        : Color.FromArgb(
                            120, 133, 148));
            };

            WinLabel nhanTen = new WinLabel
            {
                Left = 58,
                Top = 10,

                Width = 161,
                Height = 21,

                Text = ten,
                AutoEllipsis = true,

                Font = new Font(
                    "Segoe UI",
                    8.6F,
                    FontStyle.Bold),

                ForeColor = hienThi
                    ? Color.FromArgb(
                        25, 67, 92)
                    : chuPhuChuGiai
            };

            WinLabel nhanMoTa = new WinLabel
            {
                Left = 58,
                Top = 33,

                Width = 161,
                Height = 19,

                Text = moTa,
                AutoEllipsis = true,

                Font = new Font(
                    "Segoe UI", 7.7F),

                ForeColor = chuPhuChuGiai
            };

            the.Controls.Add(vach);
            the.Controls.Add(khungKyHieu);
            the.Controls.Add(nhanTen);
            the.Controls.Add(nhanMoTa);

            // Lớp MapWinGIS có handle,
            // hỗ trợ bật/tắt trực tiếp.

            if (handle >= 0)
            {
                CheckBox batTat = new CheckBox
                {
                    Width = 19,
                    Height = 22,

                    Checked = hienThi,

                    CheckAlign =
                        ContentAlignment.MiddleCenter,

                    BackColor = mauNenThe,

                    ForeColor = xanhChuGiai,

                    Cursor = Cursors.Hand,

                    FlatStyle = FlatStyle.Flat,

                    TabStop = false
                };

                batTat.CheckedChanged += (s, e) =>
                {
                    if (dangVeChuGiai)
                        return;

                    ChuGiai_DoiHienThiLop(
                        handle,
                        batTat.Checked);
                };

                the.Controls.Add(batTat);

                the.Resize += (s, e) =>
                {
                    int rong = Math.Max(
                        55,
                        the.ClientSize.Width - 95);

                    nhanTen.Width = rong;
                    nhanMoTa.Width = rong;

                    batTat.Left =
                        Math.Max(
                            0,
                            the.ClientSize.Width - 29);

                    batTat.Top = 20;
                };
            }
            else
            {
                // OSM dùng bộ chọn nền riêng.

                WinLabel nhanNen = new WinLabel
                {
                    Width = 39,
                    Height = 18,

                    Text = "NỀN",

                    TextAlign =
                        ContentAlignment.MiddleCenter,

                    BackColor = Color.FromArgb(
                        201, 228, 253),

                    ForeColor = xanhChuGiai,

                    Font = new Font(
                        "Segoe UI",
                        7F,
                        FontStyle.Bold)
                };

                the.Controls.Add(nhanNen);

                the.Resize += (s, e) =>
                {
                    nhanTen.Width =
                        Math.Max(
                            55,
                            the.ClientSize.Width - 113);

                    nhanMoTa.Width =
                        nhanTen.Width;

                    nhanNen.Left =
                        Math.Max(
                            0,
                            the.ClientSize.Width - 46);

                    nhanNen.Top = 22;
                };
            }

            // Vẽ đường viền của thẻ.

            the.Paint += (s, e) =>
            {
                if (the.Width < 4 ||
                    the.Height < 4)
                {
                    return;
                }

                Rectangle r = new Rectangle(
                    1,
                    1,
                    the.Width - 3,
                    the.Height - 3);

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (GraphicsPath path =
                    ChuGiai_TaoDuongBoGoc(
                        r, 8))
                using (Pen pen =
                    new Pen(
                        mauVienThe, 1F))
                {
                    e.Graphics.DrawPath(
                        pen, path);
                }
            };

            return the;
        }

        // =====================================================
        // 10. VE KY HIEU RASTER / RANH GIOI / DON VI
        // =====================================================

        private void ChuGiai_VeKyHieu(
            Graphics g,
            Rectangle r,
            int loai,
            Color mau)
        {
            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            int giuaY =
                r.Height / 2;

            // Ranh giới hành chính.

            if (loai == CG_RANH_TINH ||
                loai == CG_RANH_HUYEN)
            {
                using (Pen pen = new Pen(
                    mau,
                    loai == CG_RANH_TINH
                        ? 2.5F
                        : 1.5F))
                {
                    pen.DashStyle =
                        loai == CG_RANH_TINH
                            ? DashStyle.Solid
                            : DashStyle.Dash;

                    g.DrawLine(
                        pen,
                        5,
                        giuaY,
                        r.Width - 5,
                        giuaY);
                }

                return;
            }

            // Ký hiệu ảnh raster.

            if (loai == CG_RASTER)
            {
                using (SolidBrush nen =
                    new SolidBrush(
                        Color.FromArgb(
                            45, 87, 65)))
                using (Pen vien =
                    new Pen(
                        mau, 1.5F))
                {
                    g.FillRectangle(
                        nen,
                        8,
                        8,
                        r.Width - 16,
                        r.Height - 16);

                    g.DrawRectangle(
                        vien,
                        8,
                        8,
                        r.Width - 16,
                        r.Height - 16);
                }

                return;
            }

            string kyHieu;

            switch (loai)
            {
                case CG_DON_VI:
                    kyHieu = "★";
                    break;

                case CG_OSM:
                    kyHieu = "≋";
                    break;

                default:
                    kyHieu = "◆";
                    break;
            }

            using (Font font = new Font(
                "Segoe UI Symbol",
                loai == CG_DON_VI
                    ? 20F
                    : 16F,
                FontStyle.Bold))
            {
                TextRenderer.DrawText(
                    g,
                    kyHieu,
                    font,
                    r,
                    mau,

                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.NoPadding);
            }
        }
    }
}