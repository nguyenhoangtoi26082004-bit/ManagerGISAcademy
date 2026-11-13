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
        private bool daDangKyChuGiai;
        private bool daKhoiTaoChuGiai;
        private bool dangVeChuGiai;
        private bool dangChoCapNhatChuGiai;

        private bool chuGiaiThuGon = true;

        // Kết hợp chú giải vào phần cuối bảng LỚP BẢN ĐỒ.
        // Không còn nổi độc lập trên MapWinGIS.
        private bool cgDangCanBangGop;
        private bool cgDaDangKyCanBangGop;

        // GD5.4: dich tam ban do ve trai khi bang lop ben phai mo.
        private bool cg54DaDangKyDichTam;
        private bool cg54DaDichTam;
        private bool cg54DangChoDichTam;
        private double cg54SoPixelDaDich;

        private Panel pnlChuGiaiBanDo;
        private Panel pnlNoiDungChuGiai;
        private Panel pnlDauChuGiai;

        private FlowLayoutPanel flpCacLopChuGiai;

        private Button btnThuGonChuGiai;

        private WinLabel lblThongKeChuGiai;
        private WinLabel lblTieuDeChuGiai;
        private WinLabel lblSoLopChuGiai;

        private readonly Color nenChuGiai =
            Color.FromArgb(235, 247, 255);

        private readonly Color nenTheChuGiai =
            Color.FromArgb(225, 243, 255);

        private readonly Color xanhChuGiai =
            Color.FromArgb(0, 165, 127);

        private readonly Color vangChuGiai =
            Color.FromArgb(232, 151, 56);

        private readonly Color chuPhuChuGiai =
            Color.FromArgb(77, 107, 132);

        private readonly Color vienChuGiai =
            Color.FromArgb(161, 209, 230);

        private readonly Color mauRanhTinh =
            Color.FromArgb(0, 175, 146);

        private readonly Color mauRanhHuyen =
            Color.FromArgb(224, 162, 56);

        private readonly Color mauDiemDonVi =
            Color.FromArgb(0, 165, 127);

        // Mã loại ký hiệu.
        private const int CG_LOP_KHAC = 0;
        private const int CG_RANH_TINH = 1;
        private const int CG_RANH_HUYEN = 2;
        private const int CG_RASTER = 3;
        private const int CG_DON_VI = 4;
        private const int CG_OSM = 5;

        // =====================================================
        // 1. DANG KY KHOI TAO
        // =====================================================

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (daDangKyChuGiai || DesignMode)
                return;

            daDangKyChuGiai = true;

            Shown += FormMain_ChuGiai_Shown;
            FormClosed += FormMain_ChuGiai_FormClosed;
        }

        private void FormMain_ChuGiai_Shown(
            object sender,
            EventArgs e)
        {
            Application.Idle += FormMain_ChuGiai_KhiRanh;
        }

        private void FormMain_ChuGiai_KhiRanh(
            object sender,
            EventArgs e)
        {
            if (IsDisposed || Disposing)
            {
                Application.Idle -=
                    FormMain_ChuGiai_KhiRanh;
                return;
            }

            if (daKhoiTaoChuGiai)
            {
                Application.Idle -=
                    FormMain_ChuGiai_KhiRanh;
                return;
            }

            // Đợi bảng danh sách lớp hoàn thành.
            if (!daSapXepLop ||
                bangDanhSachLop == null ||
                bangDanhSachLop.IsDisposed ||
                pnlRightBody == null ||
                pnlRightBody.IsDisposed)
            {
                return;
            }

            if (pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null ||
                danhSachLop == null ||
                cboChonBanDoNen == null)
            {
                return;
            }

            Application.Idle -=
                FormMain_ChuGiai_KhiRanh;

            try
            {
                TaoBangChuGiaiBanDo();

                daKhoiTaoChuGiai = true;

                DangKyDongBoChuGiai();

                Cg54DangKyDichTam();

                CapNhatChuGiaiBanDo();

                GhiNhatKyBanDo(
                    "MAP_LEGEND_TWO_LEVEL_READY");
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "MAP_LEGEND_INIT_ERROR | " + ex);

                if (statusGeneral != null)
                {
                    statusGeneral.Text =
                        "● Lỗi chú giải: " + ex.Message;
                }
            }
        }

        private void FormMain_ChuGiai_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            Application.Idle -=
                FormMain_ChuGiai_KhiRanh;
        }

        // =====================================================
        // 2. TAO GIAO DIEN CHU GIAI
        // =====================================================

        private void TaoBangChuGiaiBanDo()
        {
            if (pnlChuGiaiBanDo != null &&
                !pnlChuGiaiBanDo.IsDisposed)
            {
                return;
            }

            pnlChuGiaiBanDo = new Panel
            {
                Name = "pnlChuGiaiBanDo",
                Width = 304,
                Height = 44,
                BackColor = nenChuGiai,
                BorderStyle = BorderStyle.None
            };

            pnlChuGiaiBanDo.Paint +=
                (s, e) => ChuGiai_VeKhung(e.Graphics);

            pnlChuGiaiBanDo.Resize +=
                (s, e) => ChuGiai_BoGocKhung();

            // ---------------------------------------------
            // Thanh tiêu đề chú giải.
            // ---------------------------------------------

            pnlDauChuGiai = new Panel
            {
                Name = "pnlDauChuGiai",
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(
                    217, 246, 242),
                Cursor = Cursors.Hand
            };

            Panel vachXanh = new Panel
            {
                Left = 0,
                Top = 10,
                Width = 3,
                Height = 24,
                BackColor = xanhChuGiai
            };

            lblTieuDeChuGiai = new WinLabel
            {
                Left = 13,
                Top = 0,
                Width = 170,
                Height = 44,

                Text = "CHÚ GIẢI BẢN ĐỒ",

                TextAlign =
                    ContentAlignment.MiddleLeft,

                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold),

                ForeColor = Color.FromArgb(
                    24, 73, 95),

                Cursor = Cursors.Hand
            };

            lblSoLopChuGiai = new WinLabel
            {
                Width = 58,
                Height = 24,
                Text = "0/0",

                TextAlign =
                    ContentAlignment.MiddleCenter,

                Font = new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Bold),

                BackColor = Color.FromArgb(
                    230, 220, 255),

                ForeColor = xanhChuGiai,

                Cursor = Cursors.Hand
            };

            btnThuGonChuGiai = new Button
            {
                Width = 30,
                Height = 28,
                Text = "+",

                FlatStyle = FlatStyle.Flat,

                BackColor = Color.FromArgb(
                    198, 228, 255),

                ForeColor = Color.FromArgb(
                    20, 110, 176),

                Font = new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Bold),

                Cursor = Cursors.Hand,
                TabStop = false,
                UseVisualStyleBackColor = false
            };

            btnThuGonChuGiai
                .FlatAppearance.BorderSize = 0;

            btnThuGonChuGiai
                .FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(155, 215, 250);

            // Sự kiện mở/đóng chú giải.
            btnThuGonChuGiai.Click +=
                (s, e) => DoiThuGonChuGiai();

            lblTieuDeChuGiai.Click +=
                (s, e) => DoiThuGonChuGiai();

            lblSoLopChuGiai.Click +=
                (s, e) => DoiThuGonChuGiai();

            pnlDauChuGiai.Click +=
                (s, e) => DoiThuGonChuGiai();

            pnlDauChuGiai.Controls.Add(
                vachXanh);

            pnlDauChuGiai.Controls.Add(
                lblTieuDeChuGiai);

            pnlDauChuGiai.Controls.Add(
                lblSoLopChuGiai);

            pnlDauChuGiai.Controls.Add(
                btnThuGonChuGiai);

            pnlDauChuGiai.Resize +=
                (s, e) => ChuGiai_CanTieuDe();

            // ---------------------------------------------
            // Vùng nội dung chú giải.
            // ---------------------------------------------

            pnlNoiDungChuGiai = new Panel
            {
                Name = "pnlNoiDungChuGiai",
                Dock = DockStyle.Fill,

                BackColor = nenChuGiai,

                Padding = new Padding(
                    11, 10, 11, 8),

                Visible = false
            };

            flpCacLopChuGiai = new FlowLayoutPanel
            {
                Name = "flpCacLopChuGiai",

                Dock = DockStyle.Fill,
                AutoScroll = true,

                FlowDirection =
                    FlowDirection.TopDown,

                WrapContents = false,

                BackColor = nenChuGiai,

                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            flpCacLopChuGiai.Resize +=
                (s, e) => CanRongTheChuGiai();

            lblThongKeChuGiai = new WinLabel
            {
                Dock = DockStyle.Bottom,
                Height = 34,

                TextAlign =
                    ContentAlignment.MiddleLeft,

                ForeColor = chuPhuChuGiai,

                Font = new Font(
                    "Segoe UI", 8F),

                BackColor = nenChuGiai,

                Padding = new Padding(
                    4, 5, 0, 0),

                Text = "Đang cập nhật lớp dữ liệu..."
            };

            pnlNoiDungChuGiai.Controls.Add(
                flpCacLopChuGiai);

            pnlNoiDungChuGiai.Controls.Add(
                lblThongKeChuGiai);

            pnlChuGiaiBanDo.Controls.Add(
                pnlNoiDungChuGiai);

            pnlChuGiaiBanDo.Controls.Add(
                pnlDauChuGiai);

            // ============================================
            // QUAN TRỌNG:
            // GỘP CHÚ GIẢI VÀO BẢNG LỚP BẢN ĐỒ.
            // ============================================

            // Không thêm vào pnlMapCanvas nữa.
            // Chú giải dùng chung pnlRightBody với danh sách lớp.

            pnlChuGiaiBanDo.Dock = DockStyle.None;

            pnlRightBody.Controls.Add(
                pnlChuGiaiBanDo);

            // Chuyển bảng lớp sang căn thủ công.
            // Phần trên: danh sách lớp.
            // Phần dưới: chú giải.

            bangDanhSachLop.Dock = DockStyle.None;

            bangDanhSachLop.Anchor =
                AnchorStyles.None;

            if (!cgDaDangKyCanBangGop)
            {
                cgDaDangKyCanBangGop = true;

                pnlRightBody.Resize +=
                    CgKhiBangGopDoiKichThuoc;

                pnlChuGiaiBanDo.LocationChanged +=
                    CgKhiChuGiaiBiDich;
            }

            bangDanhSachLop.BringToFront();

            pnlChuGiaiBanDo.BringToFront();

            ChuGiai_CanTieuDe();

            DatViTriChuGiai();
        }

        // =====================================================
        // 3. BO GOC VA VE VIEN
        // =====================================================

        private static GraphicsPath ChuGiai_TaoDuongBoGoc(
            Rectangle r,
            int banKinh)
        {
            GraphicsPath path =
                new GraphicsPath();

            int d = Math.Min(
                banKinh * 2,
                Math.Min(r.Width, r.Height));

            if (d < 3)
            {
                path.AddRectangle(r);
                return path;
            }

            path.AddArc(
                r.Left,
                r.Top,
                d,
                d,
                180,
                90);

            path.AddArc(
                r.Right - d,
                r.Top,
                d,
                d,
                270,
                90);

            path.AddArc(
                r.Right - d,
                r.Bottom - d,
                d,
                d,
                0,
                90);

            path.AddArc(
                r.Left,
                r.Bottom - d,
                d,
                d,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        private void ChuGiai_VeKhung(
            Graphics g)
        {
            if (pnlChuGiaiBanDo == null ||
                pnlChuGiaiBanDo.Width < 5 ||
                pnlChuGiaiBanDo.Height < 5)
            {
                return;
            }

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle r = new Rectangle(
                1,
                1,
                pnlChuGiaiBanDo.Width - 3,
                pnlChuGiaiBanDo.Height - 3);

            using (GraphicsPath path =
                ChuGiai_TaoDuongBoGoc(
                    r, 12))
            using (Pen pen =
                new Pen(vienChuGiai, 1F))
            {
                g.DrawPath(pen, path);
            }
        }

        private void ChuGiai_BoGocKhung()
        {
            if (pnlChuGiaiBanDo == null ||
                pnlChuGiaiBanDo.IsDisposed ||
                pnlChuGiaiBanDo.Width < 5 ||
                pnlChuGiaiBanDo.Height < 5)
            {
                return;
            }

            Rectangle r = new Rectangle(
                0,
                0,
                pnlChuGiaiBanDo.Width,
                pnlChuGiaiBanDo.Height);

            using (GraphicsPath path =
                ChuGiai_TaoDuongBoGoc(
                    r, 12))
            {
                Region cu =
                    pnlChuGiaiBanDo.Region;

                pnlChuGiaiBanDo.Region =
                    new Region(path);

                if (cu != null)
                    cu.Dispose();
            }

            pnlChuGiaiBanDo.Invalidate();
        }

        private void ChuGiai_CanTieuDe()
        {
            if (pnlDauChuGiai == null ||
                lblSoLopChuGiai == null ||
                btnThuGonChuGiai == null)
            {
                return;
            }

            int rong =
                pnlDauChuGiai.ClientSize.Width;

            btnThuGonChuGiai.Left =
                Math.Max(0, rong - 39);

            btnThuGonChuGiai.Top = 8;

            lblSoLopChuGiai.Left =
                Math.Max(100, rong - 105);

            lblSoLopChuGiai.Top = 10;

            lblTieuDeChuGiai.Width =
                Math.Max(
                    65,
                    lblSoLopChuGiai.Left - 18);
        }
    }
}