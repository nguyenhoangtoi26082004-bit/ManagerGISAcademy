using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // C# 7.3 / .NET Framework 4.8. Tái sử dụng danhSachLop và bộ chọn nền.
    public partial class FormMain
    {
        private bool daDangKySapXepLop;
        private bool daSapXepLop;
        private bool dangLamMoiGiaoDienLop;
        private TableLayoutPanel bangDanhSachLop;
        private FlowLayoutPanel khungTheLop;
        private Label lblTrangThaiDanhSachLop;
        private Label lblTongSoLopGiaoDien;
        private Button btnLamMoiDanhSachLop;
        private readonly ToolTip meoGiaoDienLop = new ToolTip();

        // Light Military GIS: nền sáng, tương phản rõ, màu chức năng riêng.
        private readonly Color nenGiaoDienLop = Color.FromArgb(235, 247, 255);
        private readonly Color nenTheLop = Color.FromArgb(236, 255, 247);
        private readonly Color mauChuPhuLop = Color.FromArgb(85, 111, 133);
        private readonly Color mauXanhNgocLop = Color.FromArgb(0, 153, 119);
        private readonly Color lp26MauChuChinh = Color.FromArgb(23, 57, 82);
        private readonly Color lp26MauXanhDuong = Color.FromArgb(27, 124, 196);
        private readonly Color lp26MauTim = Color.FromArgb(119, 81, 195);
        private readonly Color lp26MauCam = Color.FromArgb(190, 106, 27);

        private TextBox lp26TimLop;
        private CheckBox lp26ChiLopDangBat;
        private System.Windows.Forms.Timer lp26HenLoc;
        private bool lp26ChoLamMoi;
        private bool lp26DaGanSuKien;
        private int lp26SoLopTrongBoLoc;
        private int lp26HandleCanCuonDen = -1;
        private int lp26HandleThongTin = -1;

        private readonly Font lp26FontTen =
            new Font("Segoe UI", 8.5F, FontStyle.Bold);
        private readonly Font lp26FontMoTa =
            new Font("Segoe UI", 8F);
        private readonly Font lp26FontKyHieu =
            new Font("Segoe UI Symbol", 13F);

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (daDangKySapXepLop || IsDisposed || Disposing || DesignMode)
                return;

            daDangKySapXepLop = true;
            Application.Idle += FormMain_GiaoDienLop_KhiRanh;
            FormClosed += Lp26_KhiDongForm;
        }

        private void FormMain_GiaoDienLop_KhiRanh(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || daSapXepLop)
            {
                Application.Idle -= FormMain_GiaoDienLop_KhiRanh;
                return;
            }

            if (danhSachLop == null ||
                cboChonBanDoNen == null ||
                btnChonTepBanDoNen == null ||
                pnlRightBody == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated ||
                ketNoiMapWinGIS == null ||
                cboChonBanDoNen.Parent == null)
                return;

            Application.Idle -= FormMain_GiaoDienLop_KhiRanh;

            try
            {
                TaoGiaoDienDanhSachLopMoi();
                daSapXepLop = true;
                DangKyDongBoDanhSachLop();
                LamMoiDanhSachLop();

                GhiNhatKyBanDo(
                    "MILITARY_LAYER_PANEL_READY | search=true | zoom=true | reorder=true | saveOrder=true | layerInfo=true");
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("MILITARY_LAYER_PANEL_FAILED | " + ex);
                statusGeneral.Text =
                    "● Không tạo được bảng lớp: " + ex.Message;
            }
        }

        private void Lp26_KhiDongForm(object sender, FormClosedEventArgs e)
        {
            Application.Idle -= FormMain_GiaoDienLop_KhiRanh;

            if (lp26HenLoc != null)
            {
                lp26HenLoc.Stop();
                lp26HenLoc.Dispose();
                lp26HenLoc = null;
            }

            if (lp26DaGanSuKien)
            {
                btnNavMap.Click -= Lp26_HenDongBo;
                btnNapLop.Click -= Lp26_HenDongBo;
                cboChonBanDoNen.SelectedIndexChanged -= Lp26_HenDongBo;
                btnChonTepBanDoNen.Click -= Lp26_HenDongBo;
                danhSachLop.ItemCheck -= Lp26_KhiDoiDanhDau;
                pnlRightCard.VisibleChanged -= Lp26_KhiMoBangLop;

                try
                {
                    if (banDoMapWinGIS != null &&
                        !banDoMapWinGIS.IsDisposed)
                    {
                        banDoMapWinGIS.LayerAdded -= Lp26_KhiThemLop;
                        banDoMapWinGIS.LayerRemoved -= Lp26_KhiXoaLop;
                    }
                }
                catch (Exception)
                {
                    // ActiveX có thể đã được giải phóng.
                }
            }

            meoGiaoDienLop.Dispose();
            lp26FontTen.Dispose();
            lp26FontMoTa.Dispose();
            lp26FontKyHieu.Dispose();
        }

        private void TaoGiaoDienDanhSachLopMoi()
        {
            pnlRightBody.SuspendLayout();

            try
            {
                // Giữ các điều khiển đang sử dụng trước khi
                // hủy giao diện bảng lớp cũ.
                if (cboChonBanDoNen.Parent != null)
                {
                    cboChonBanDoNen.Parent.Controls.Remove(
                        cboChonBanDoNen);
                }

                if (btnChonTepBanDoNen.Parent != null)
                {
                    btnChonTepBanDoNen.Parent.Controls.Remove(
                        btnChonTepBanDoNen);
                }

                // QUAN TRỌNG:
                // Trước đây nút chẩn đoán không được giữ lại,
                // nên bị Dispose cùng với khung giao diện cũ.
                if (btnChanDoanRaster != null &&
                    !btnChanDoanRaster.IsDisposed &&
                    btnChanDoanRaster.Parent != null)
                {
                    btnChanDoanRaster.Parent.Controls.Remove(
                        btnChanDoanRaster);
                }

                if (danhSachLop.Parent != null)
                {
                    danhSachLop.Parent.Controls.Remove(
                        danhSachLop);
                }

                Control[] cacKhungCu =
                    new Control[pnlRightBody.Controls.Count];

                pnlRightBody.Controls.CopyTo(cacKhungCu, 0);
                pnlRightBody.Controls.Clear();

                foreach (Control khungCu in cacKhungCu)
                {
                    khungCu.Dispose();
                }

                pnlRightBody.BackColor = nenGiaoDienLop;
                pnlRightBody.AutoScroll = true;

                pnlRightBottom.Visible = false;
                pnlRightBottom.Height = 0;

                // Giữ danh sách lớp gốc để đồng bộ
                // MapWinGIS và các sự kiện hiện có.
                danhSachLop.Dock = DockStyle.None;
                danhSachLop.Visible = false;
                danhSachLop.TabStop = false;
                danhSachLop.Size = new Size(1, 1);
                danhSachLop.Location =
                    new System.Drawing.Point(-20, -20);

                pnlRightBody.Controls.Add(danhSachLop);

                // Một bố cục duy nhất quản lý khu vực lớp.
                bangDanhSachLop = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    Height = 442,
                    ColumnCount = 1,
                    RowCount = 7,
                    BackColor = nenGiaoDienLop,
                    Padding = new Padding(10, 6, 10, 6),
                    Margin = Padding.Empty
                };

                bangDanhSachLop.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Percent, 100F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 23F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 31F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 36F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 8F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 120F));

                bangDanhSachLop.RowStyles.Add(new RowStyle(SizeType.Absolute, 180F));

                bangDanhSachLop.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 27F));

                TaoTieuDeNenBanDo();
                TaoBoChonNenMoi();
                TaoNutChonAnhNenMoi();
                TaoDuongPhanCach();
                TaoTieuDeKhuVucLop();
                TaoKhungTheLop();
                TaoChanDanhSachLop();

                pnlRightBody.Controls.Add(bangDanhSachLop);
                bangDanhSachLop.BringToFront();
                ManagerGISAcademy.GUI.Views.NativeLightUi.ApplyRoundedFields(bangDanhSachLop);
                ManagerGISAcademy.GUI.Views.NativeLightUi.PolishControls(pnlRightCard);
            }
            finally
            {
                pnlRightBody.ResumeLayout(true);
            }
        }

        private void TaoTieuDeNenBanDo()
        {
            bangDanhSachLop.Controls.Add(new Label
            {
                Dock = DockStyle.Fill,
                Text = "BẢN ĐỒ NỀN",
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = lp26MauXanhDuong,
                Font = lp26FontTen
            }, 0, 0);
        }

        private void TaoBoChonNenMoi()
        {
            cboChonBanDoNen.Dock = DockStyle.Fill;
            cboChonBanDoNen.Margin = new Padding(0, 0, 0, 2);
            cboChonBanDoNen.BackColor = Color.FromArgb(224, 244, 255);
            cboChonBanDoNen.ForeColor = lp26MauChuChinh;
            cboChonBanDoNen.Font = lp26FontMoTa;
            cboChonBanDoNen.FlatStyle = FlatStyle.Flat;
            cboChonBanDoNen.DropDownStyle = ComboBoxStyle.DropDownList;

            bangDanhSachLop.Controls.Add(cboChonBanDoNen, 0, 1);

            meoGiaoDienLop.SetToolTip(
                cboChonBanDoNen,
                "Chọn nguồn nền vệ tinh, nền trực tuyến hoặc không có nền.");
        }

        private void TaoNutChonAnhNenMoi()
        {
            // Một hàng duy nhất chứa hai nút.
            TableLayoutPanel hangNutNen = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };

            hangNutNen.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 60F));

            hangNutNen.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 40F));

            hangNutNen.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            // =======================================
            // NÚT ĐỔI ẢNH VỆ TINH NGOẠI TUYẾN
            // =======================================

            btnChonTepBanDoNen.Dock = DockStyle.Fill;
            btnChonTepBanDoNen.Margin =
                new Padding(0, 3, 3, 3);

            btnChonTepBanDoNen.Text = "ĐỔI ẢNH .TIF";
            btnChonTepBanDoNen.BackColor =
                Color.FromArgb(16, 137, 168);

            btnChonTepBanDoNen.ForeColor = Color.White;
            btnChonTepBanDoNen.FlatStyle = FlatStyle.Flat;
            btnChonTepBanDoNen.FlatAppearance.BorderSize = 0;
            btnChonTepBanDoNen.Font = lp26FontTen;
            btnChonTepBanDoNen.Cursor = Cursors.Hand;

            // =======================================
            // NÚT CHẨN ĐOÁN BẢN ĐỒ NỀN
            // =======================================

            // Ưu tiên sử dụng nút cũ để giữ nguyên
            // sự kiện Click đã đăng ký trong BanDoNen.cs.
            if (btnChanDoanRaster == null ||
                btnChanDoanRaster.IsDisposed)
            {
                btnChanDoanRaster = new Button();

                btnChanDoanRaster.Click +=
                    (s, e) => ChanDoanAnhNen();
            }

            btnChanDoanRaster.Dock = DockStyle.Fill;
            btnChanDoanRaster.Margin =
                new Padding(3, 3, 0, 3);

            btnChanDoanRaster.Text = "CHẨN ĐOÁN";

            btnChanDoanRaster.BackColor =
                Color.FromArgb(17, 65, 60);

            btnChanDoanRaster.ForeColor =
                Color.FromArgb(197, 255, 231);

            btnChanDoanRaster.FlatStyle =
                FlatStyle.Flat;

            btnChanDoanRaster.FlatAppearance.BorderSize = 0;

            btnChanDoanRaster.Font = new Font(
                "Segoe UI",
                7.6F,
                FontStyle.Bold);

            btnChanDoanRaster.Cursor = Cursors.Hand;
            btnChanDoanRaster.TabStop = true;

            btnChanDoanRaster.AccessibleName =
                "Chẩn đoán bản đồ nền";

            // =======================================
            // THÊM VÀO BẢNG LỚP HIỆN CÓ
            // =======================================

            hangNutNen.Controls.Add(
                btnChonTepBanDoNen, 0, 0);

            hangNutNen.Controls.Add(
                btnChanDoanRaster, 1, 0);

            bangDanhSachLop.Controls.Add(
                hangNutNen, 0, 2);

            meoGiaoDienLop.SetToolTip(
                btnChanDoanRaster,
                "Kiểm tra provider, phép chiếu, cache và trạng thái tile.");

            meoGiaoDienLop.SetToolTip(
                btnChonTepBanDoNen,
                "Chọn ảnh vệ tinh GeoTIFF ngoại tuyến.");
        }

        private void TaoDuongPhanCach()
        {
            bangDanhSachLop.Controls.Add(new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(204, 225, 240)
            }, 0, 3);
        }

        private void TaoTieuDeKhuVucLop()
        {
            TableLayoutPanel khung = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Margin = Padding.Empty,
                BackColor = nenGiaoDienLop
            };

            khung.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            khung.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 29F));
            khung.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 31F));
            khung.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 25F));
            khung.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32F));

            Panel dau = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = nenGiaoDienLop
            };

            Label nhan = new Label
            {
                Text = "CÁC LỚP DỮ LIỆU",
                Dock = DockStyle.Fill,
                ForeColor = lp26MauChuChinh,
                Font = lp26FontTen,
                TextAlign = ContentAlignment.MiddleLeft
            };

            btnLamMoiDanhSachLop = Lp26_TaoNut("LÀM MỚI");
            btnLamMoiDanhSachLop.Dock = DockStyle.Right;
            btnLamMoiDanhSachLop.Width = 82;

            Lp26_DatMauNut(
                btnLamMoiDanhSachLop,
                Color.FromArgb(215, 250, 237),
                Color.FromArgb(0, 126, 96),
                Color.FromArgb(185, 240, 217));

            btnLamMoiDanhSachLop.Click +=
                (s, e) => LamMoiDanhSachLop();

            dau.Controls.Add(nhan);
            dau.Controls.Add(btnLamMoiDanhSachLop);

            TableLayoutPanel tim = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };

            tim.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));
            tim.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 30F));
            tim.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            lp26TimLop = new TextBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                ForeColor = lp26MauChuChinh,
                BorderStyle = BorderStyle.FixedSingle,
                Font = lp26FontMoTa,
                AccessibleName = "Tìm lớp theo tên"
            };

            Button xoa = Lp26_TaoNut("×");
            xoa.Dock = DockStyle.Fill;

            Lp26_DatMauNut(
                xoa,
                Color.FromArgb(232, 224, 255),
                lp26MauTim,
                Color.FromArgb(214, 200, 251));

            xoa.Click += (s, e) =>
            {
                lp26TimLop.Clear();
                lp26TimLop.Focus();
            };

            lp26HenLoc = new System.Windows.Forms.Timer
            {
                Interval = 180
            };

            lp26HenLoc.Tick += (s, e) =>
            {
                lp26HenLoc.Stop();

                if (!IsDisposed && !Disposing && daSapXepLop)
                {
                    VeLaiCacTheLop();
                    CapNhatThongKeDanhSachLop();
                }
            };

            lp26TimLop.TextChanged += (s, e) =>
            {
                if (lp26HenLoc == null)
                    return;

                lp26HenLoc.Stop();
                lp26HenLoc.Start();
            };

            lp26TimLop.KeyDown += (s, e) =>
            {
                if (e.KeyCode != Keys.Escape)
                    return;

                lp26TimLop.Clear();
                e.Handled = true;
                e.SuppressKeyPress = true;
            };

            lp26ChiLopDangBat = new CheckBox
            {
                Text = "Chỉ xem lớp đang bật",
                Dock = DockStyle.Fill,
                ForeColor = mauChuPhuLop,
                Font = lp26FontMoTa
            };

            lp26ChiLopDangBat.CheckedChanged += (s, e) =>
            {
                VeLaiCacTheLop();
                CapNhatThongKeDanhSachLop();
            };

            tim.Controls.Add(lp26TimLop, 0, 0);
            tim.Controls.Add(xoa, 1, 0);

            khung.Controls.Add(dau, 0, 0);
            khung.Controls.Add(tim, 0, 1);
            khung.Controls.Add(lp26ChiLopDangBat, 0, 2);

            TableLayoutPanel luuThuTu = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty
            };

            luuThuTu.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));
            luuThuTu.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));
            luuThuTu.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            Button luu = Lp26_TaoNut("LƯU THỨ TỰ");

            Lp26_DatMauNut(
                luu,
                Color.FromArgb(215, 239, 255),
                lp26MauXanhDuong,
                Color.FromArgb(188, 226, 251));

            luu.Dock = DockStyle.Fill;
            luu.Margin = new Padding(0, 2, 3, 2);
            luu.Click += (s, e) => Lp26_LuuThuTuLop();

            Button khoiPhuc = Lp26_TaoNut("KHÔI PHỤC");

            Lp26_DatMauNut(
                khoiPhuc,
                Color.FromArgb(255, 237, 209),
                lp26MauCam,
                Color.FromArgb(255, 220, 168));

            khoiPhuc.Dock = DockStyle.Fill;
            khoiPhuc.Margin = new Padding(3, 2, 0, 2);
            khoiPhuc.Click += (s, e) => Lp26_KhoiPhucThuTuLop();

            luuThuTu.Controls.Add(luu, 0, 0);
            luuThuTu.Controls.Add(khoiPhuc, 1, 0);
            khung.Controls.Add(luuThuTu, 0, 3);

            meoGiaoDienLop.SetToolTip(
                luu,
                "Ghi nhớ thứ tự toàn bộ lớp, kể cả lớp đang bị lọc hoặc đang ẩn.");

            meoGiaoDienLop.SetToolTip(
                khoiPhuc,
                "Áp dụng thứ tự đã lưu cho các lớp đang nạp trên bản đồ.");

            bangDanhSachLop.Controls.Add(khung, 0, 4);

            meoGiaoDienLop.SetToolTip(
                lp26TimLop,
                "Tìm theo tên lớp; chấp nhận có dấu hoặc không dấu.");

            meoGiaoDienLop.SetToolTip(
                xoa,
                "Xóa từ khóa tìm lớp.");
        }

        private Button Lp26_TaoNut(string noiDung)
        {
            Button nut = new Button
            {
                Text = noiDung,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = lp26MauChuChinh,
                Font = lp26FontTen,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 1, 0, 2)
            };

            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(218, 242, 255);
            nut.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(201, 232, 249);

            Lp26_BoGoc(nut, 7);
            return nut;
        }

        // Bo góc theo kích thước thực khi panel/nút được Resize.
        // Chỉ thay đổi hiển thị; không can thiệp các sự kiện MapWinGIS.
        private static void Lp26_BoGoc(Control control, int banKinh)
        {
            if (control == null) return;
            Action capNhat = () =>
            {
                if (control.IsDisposed || control.Width < 3 || control.Height < 3)
                    return;
                using (GraphicsPath path = Lp26_DuongBoGoc(
                    new Rectangle(0, 0, control.Width, control.Height), banKinh))
                {
                    Region cu = control.Region;
                    control.Region = new Region(path);
                    if (cu != null) cu.Dispose();
                }
            };
            control.SizeChanged += (s, e) => capNhat();
            capNhat();
        }

        private static GraphicsPath Lp26_DuongBoGoc(Rectangle r, int banKinh)
        {
            GraphicsPath path = new GraphicsPath();
            int d = Math.Max(2, Math.Min(2 * banKinh,
                Math.Min(r.Width, r.Height)));
            path.AddArc(r.Left, r.Top, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void Lp26_DatMauNut(Button nut, Color nen, Color chu, Color hover)
        {
            nut.BackColor = nen;
            nut.ForeColor = chu;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = hover;
        }
    }
}