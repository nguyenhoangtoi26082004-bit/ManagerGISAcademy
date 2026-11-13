using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

using AxMapWinGIS;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;
using MapWinGIS;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // GD5.11:
    // Chon toa do va tu dong doi chieu phuong/xa moi.
    //
    // Khong su dung ThemSuaDonVi cu.
    // Khong ghi PostGIS khi nhap ban do.
    // Khong tu thay thong tin nguoi dung da nhap.
    public partial class FormMain
    {
        private UcNhapDonViNative nativeBieuMauDangChonToaDo;

        private bool nativeDangChonToaDo;
        private bool nativeDaNhanChuotChonToaDo;

        private System.Drawing.Point nativeDiemNhanChuotChonToaDo;

        private Panel nativeThanhHuongDanChonToaDo;

        // ==========================================
        // THANH HUONG DAN CHON TOA DO
        // ==========================================

        private sealed class NativePickBanner : Panel
        {
            private readonly Label icon;
            private readonly Label title;
            private readonly Label note;
            private readonly Button cancel;

            public event Action YeuCauHuy;

            public NativePickBanner()
            {
                Name = "nativePickBanner";

                Height = 76;
                Width = 560;

                DoubleBuffered = true;
                ResizeRedraw = true;

                BackColor = Color.FromArgb(12, 57, 76);

                icon = new Label
                {
                    Text = "⌖",
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(10, 143, 124),
                    Font = new Font(
                        "Segoe UI Symbol",
                        22F,
                        FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Left = 13,
                    Top = 14,
                    Width = 49,
                    Height = 49
                };

                title = new Label
                {
                    Text = "NHẤP BẢN ĐỒ ĐỂ CHỌN TỌA ĐỘ HỒ SƠ",
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Font = new Font(
                        "Segoe UI",
                        9.8F,
                        FontStyle.Bold),
                    AutoEllipsis = true,
                    Left = 76,
                    Top = 15,
                    Height = 25
                };

                note = new Label
                {
                    Text = "Tự lấy WGS84 và đối chiếu Phường/Xã mới.",
                    ForeColor = Color.FromArgb(
                        207, 242, 239),
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 8F),
                    AutoEllipsis = true,
                    Left = 76,
                    Top = 41,
                    Height = 20
                };

                cancel = new Button
                {
                    Text = "×  HỦY CHỌN",
                    BackColor = Color.FromArgb(39, 79, 105),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font(
                        "Segoe UI",
                        8.5F,
                        FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Height = 36,
                    Width = 124,
                    Top = 20,
                    TabStop = false,
                    UseVisualStyleBackColor = false
                };

                cancel.FlatAppearance.BorderColor =
                    Color.FromArgb(137, 199, 205);

                cancel.FlatAppearance.BorderSize = 1;

                cancel.FlatAppearance.MouseOverBackColor =
                    Color.FromArgb(58, 108, 137);

                cancel.Click += (s, e) =>
                {
                    Action xuLy = YeuCauHuy;
                    if (xuLy != null)
                        xuLy();
                };

                Controls.Add(icon);
                Controls.Add(title);
                Controls.Add(note);
                Controls.Add(cancel);

                Resize += (s, e) => CanLaiNoiDung();

                CanLaiNoiDung();
            }

            private void CanLaiNoiDung()
            {
                if (Width < 10 || Height < 10)
                    return;

                cancel.Left = Math.Max(
                    165,
                    Width - cancel.Width - 13);

                int rongChu = Math.Max(
                    70,
                    cancel.Left - title.Left - 13);

                title.Width = rongChu;
                note.Width = rongChu;

                using (GraphicsPath p =
                    TaoDuongBoGoc(
                        new Rectangle(
                            0, 0, Width, Height),
                        16))
                {
                    Region cu = Region;
                    Region = new Region(p);

                    if (cu != null)
                        cu.Dispose();
                }

                using (GraphicsPath p =
                    TaoDuongBoGoc(
                        new Rectangle(
                            0,
                            0,
                            cancel.Width,
                            cancel.Height),
                        9))
                {
                    Region cu = cancel.Region;
                    cancel.Region = new Region(p);

                    if (cu != null)
                        cu.Dispose();
                }
            }

            private static GraphicsPath TaoDuongBoGoc(
                Rectangle r,
                int radius)
            {
                GraphicsPath p = new GraphicsPath();

                int d = Math.Max(
                    2,
                    Math.Min(
                        radius * 2,
                        Math.Min(r.Width, r.Height)));

                p.AddArc(
                    r.Left, r.Top,
                    d, d, 180, 90);

                p.AddArc(
                    r.Right - d, r.Top,
                    d, d, 270, 90);

                p.AddArc(
                    r.Right - d, r.Bottom - d,
                    d, d, 0, 90);

                p.AddArc(
                    r.Left, r.Bottom - d,
                    d, d, 90, 90);

                p.CloseFigure();

                return p;
            }

            protected override void OnPaintBackground(
                PaintEventArgs e)
            {
                if (Width < 2 || Height < 2)
                    return;

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                Rectangle r = new Rectangle(
                    0, 0, Width - 1, Height - 1);

                using (GraphicsPath p =
                    TaoDuongBoGoc(r, 16))
                using (LinearGradientBrush brush =
                    new LinearGradientBrush(
                        r,
                        Color.FromArgb(0, 129, 112),
                        Color.FromArgb(15, 43, 77),
                        LinearGradientMode.Horizontal))
                using (Pen vien = new Pen(
                    Color.FromArgb(77, 213, 195),
                    1.5F))
                {
                    e.Graphics.FillPath(brush, p);
                    e.Graphics.DrawPath(vien, p);
                }
            }
        }

        // ==========================================
        // BAT CHON TOA DO HO SO
        // ==========================================

        private void Native_BatChonViTriChoHoSo(
            UcNhapDonViNative trang)
        {
            if (trang == null ||
                trang.IsDisposed ||
                !object.ReferenceEquals(
                    nativeManHinhHoSoDangMo,
                    trang) ||
                nativeDangChonToaDo)
                return;

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated ||
                !daKhoiTaoThongTinBanDo)
            {
                MessageBox.Show(
                    this,
                    "Bản đồ GIS chưa sẵn sàng.",
                    "Chọn vị trí hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            nativeBieuMauDangChonToaDo = trang;
            nativeDangChonToaDo = true;
            nativeDaNhanChuotChonToaDo = false;

            try
            {
                // Tach UserControl truoc khi chuyen trang.
                // Giu nguyen du lieu chua luu.

                if (pnlPhanHeHost.Controls.Contains(trang))
                {
                    pnlPhanHeHost.Controls.Remove(trang);
                }

                nativeManHinhHoSoDangMo = null;

                ChonPhanHe(
                    btnNavMap,
                    "Bản đồ GIS");

                if (cheDoDoDac != 0)
                {
                    ThoatDoDacKhiChuyenCongCu();
                }

                DatCheDoBanDo(true);

                // Tam dung tra cuu diem thong thuong.
                dangOCheDoChonDiem = false;
                ttDvDangNhanChuot = false;

                banDoMapWinGIS.CursorMode =
                    tkCursorMode.cmNone;

                banDoMapWinGIS.SendMouseDown = true;
                banDoMapWinGIS.SendMouseUp = true;

                banDoMapWinGIS.MouseDownEvent -=
                    Native_BanDo_MouseDownChonToaDo;

                banDoMapWinGIS.MouseUpEvent -=
                    Native_BanDo_MouseUpChonToaDo;

                banDoMapWinGIS.MouseDownEvent +=
                    Native_BanDo_MouseDownChonToaDo;

                banDoMapWinGIS.MouseUpEvent +=
                    Native_BanDo_MouseUpChonToaDo;

                lblPageTitle.TextChanged -=
                    Native_KhiDoiPhanHeDangChonToaDo;

                lblPageTitle.TextChanged +=
                    Native_KhiDoiPhanHeDangChonToaDo;

                pnlMapCanvas.Resize -=
                    Native_CanThanhChonToaDo;

                pnlMapCanvas.Resize +=
                    Native_CanThanhChonToaDo;

                FormClosed -=
                    Native_FormDongKhiChonToaDo;

                FormClosed +=
                    Native_FormDongKhiChonToaDo;

                Native_TaoThanhChonToaDo();

                statusGeneral.Text =
                    "● Nhấp bản đồ để chọn tọa độ; " +
                    "hệ thống sẽ kiểm tra Phường/Xã mới.";
            }
            catch (Exception ex)
            {
                Native_KetThucChonToaDo(
                    false, 0d, 0d, true);

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không mở được chế độ chọn vị trí",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        private void Native_TaoThanhChonToaDo()
        {
            if (nativeThanhHuongDanChonToaDo != null)
                return;

            NativePickBanner thanh =
                new NativePickBanner();

            thanh.YeuCauHuy += () =>
                Native_KetThucChonToaDo(
                    false, 0d, 0d, true);

            nativeThanhHuongDanChonToaDo = thanh;

            pnlMapCanvas.Controls.Add(thanh);

            Native_CanThanhChonToaDo(
                this,
                EventArgs.Empty);

            thanh.BringToFront();
        }

        private void Native_CanThanhChonToaDo(
            object sender,
            EventArgs e)
        {
            Panel thanh =
                nativeThanhHuongDanChonToaDo;

            if (thanh == null ||
                thanh.IsDisposed ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed)
                return;

            int chieuRong =
                pnlMapCanvas.ClientSize.Width;

            int khoangBanDo = Math.Max(
                335,
                chieuRong - 344);

            int rongThanh = Math.Min(
                585,
                Math.Max(
                    300,
                    khoangBanDo - 20));

            rongThanh = Math.Min(
                rongThanh,
                Math.Max(
                    200,
                    chieuRong - 16));

            thanh.Width = rongThanh;

            thanh.Left = Math.Max(
                8,
                (khoangBanDo - rongThanh) / 2);

            thanh.Top = 62;

            thanh.BringToFront();
        }

        // ==========================================
        // XU LY CHUOT MAPWINGIS
        // ==========================================

        private void Native_BanDo_MouseDownChonToaDo(
            object sender,
            _DMapEvents_MouseDownEvent e)
        {
            nativeDaNhanChuotChonToaDo = false;

            if (!nativeDangChonToaDo ||
                e.button != 1 ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.CursorMode !=
                    tkCursorMode.cmNone)
                return;

            nativeDiemNhanChuotChonToaDo =
                new System.Drawing.Point(
                    e.x,
                    e.y);

            nativeDaNhanChuotChonToaDo = true;
        }

        private void Native_BanDo_MouseUpChonToaDo(
            object sender,
            _DMapEvents_MouseUpEvent e)
        {
            bool daNhan =
                nativeDaNhanChuotChonToaDo;

            nativeDaNhanChuotChonToaDo = false;

            if (!nativeDangChonToaDo ||
                !daNhan ||
                e.button != 1 ||
                IsDisposed ||
                Disposing ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            double dx =
                e.x - nativeDiemNhanChuotChonToaDo.X;

            double dy =
                e.y - nativeDiemNhanChuotChonToaDo.Y;

            if (dx * dx + dy * dy > 36d)
                return;

            try
            {
                double x = 0d;
                double y = 0d;

                double kinhDo = 0d;
                double viDo = 0d;

                banDoMapWinGIS.PixelToProj(
                    e.x,
                    e.y,
                    ref x,
                    ref y);

                bool chuyenDuoc =
                    banDoMapWinGIS.ProjToDegrees(
                        x,
                        y,
                        ref kinhDo,
                        ref viDo);

                if (!chuyenDuoc ||
                    double.IsNaN(kinhDo) ||
                    double.IsInfinity(kinhDo) ||
                    double.IsNaN(viDo) ||
                    double.IsInfinity(viDo) ||
                    kinhDo < -180d ||
                    kinhDo > 180d ||
                    viDo < -90d ||
                    viDo > 90d)
                {
                    statusGeneral.Text =
                        "● Không chuyển được tọa độ " +
                        "sang WGS84.";

                    return;
                }

                // Roi callback ActiveX truoc
                // khi dieu huong ve bieu mau.
                BeginInvoke(new MethodInvoker(() =>
                {
                    if (!IsDisposed &&
                        !Disposing &&
                        nativeDangChonToaDo)
                    {
                        Native_KetThucChonToaDo(
                            true,
                            kinhDo,
                            viDo,
                            true);
                    }
                }));
            }
            catch (Exception ex)
            {
                statusGeneral.Text =
                    "● Lỗi chọn tọa độ: " +
                    ex.Message;
            }
        }

        // ==========================================
        // DUNG CHON TOA DO
        // ==========================================

        private void Native_KhiDoiPhanHeDangChonToaDo(
            object sender,
            EventArgs e)
        {
            if (nativeDangChonToaDo &&
                lblPageTitle.Text != "Bản đồ GIS")
            {
                Native_KetThucChonToaDo(
                    false, 0d, 0d, false);
            }
        }

        private void Native_FormDongKhiChonToaDo(
            object sender,
            FormClosedEventArgs e)
        {
            Native_KetThucChonToaDo(
                false, 0d, 0d, false);
        }

        private void Native_KetThucChonToaDo(
            bool daChon,
            double kinhDo,
            double viDo,
            bool quayLaiBieuMau)
        {
            UcNhapDonViNative trang =
                nativeBieuMauDangChonToaDo;

            nativeBieuMauDangChonToaDo = null;
            nativeDangChonToaDo = false;
            nativeDaNhanChuotChonToaDo = false;

            if (lblPageTitle != null &&
                !lblPageTitle.IsDisposed)
            {
                lblPageTitle.TextChanged -=
                    Native_KhiDoiPhanHeDangChonToaDo;
            }

            if (pnlMapCanvas != null &&
                !pnlMapCanvas.IsDisposed)
            {
                pnlMapCanvas.Resize -=
                    Native_CanThanhChonToaDo;
            }

            FormClosed -=
                Native_FormDongKhiChonToaDo;

            if (banDoMapWinGIS != null &&
                !banDoMapWinGIS.IsDisposed)
            {
                try
                {
                    banDoMapWinGIS.MouseDownEvent -=
                        Native_BanDo_MouseDownChonToaDo;

                    banDoMapWinGIS.MouseUpEvent -=
                        Native_BanDo_MouseUpChonToaDo;

                    if (!IsDisposed && !Disposing)
                        DatCheDoBanDo(true);
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "UNIT_PICK_CLEANUP_FAILED | " +
                        ex.Message);
                }
            }

            Panel thanh =
                nativeThanhHuongDanChonToaDo;

            nativeThanhHuongDanChonToaDo = null;

            if (thanh != null && !thanh.IsDisposed)
                thanh.Dispose();

            if (trang == null || trang.IsDisposed)
                return;

            if (!quayLaiBieuMau ||
                IsDisposed ||
                Disposing)
            {
                trang.Dispose();
                return;
            }

            try
            {
                ChonPhanHe(
                    btnNavManage,
                    "Quản lý dữ liệu");

                if (daChon)
                {
                    trang.CapNhatToaDoWgs84(
                        kinhDo,
                        viDo);
                }

                Native_HienTrangHoSo(trang);

                if (daChon)
                {
                    statusGeneral.Text =
                        "● Đã nhận WGS84. " +
                        "Đang đối chiếu địa giới Phường/Xã...";

                    // Thuc hien truy van khong gian
                    // sau khi bieu mau da duoc hien lai.
                    Native_NhanDienPhuongXaAsync(
                        trang,
                        kinhDo,
                        viDo);
                }
                else
                {
                    statusGeneral.Text =
                        "● Đã hủy chọn vị trí; " +
                        "dữ liệu nhập vẫn được giữ nguyên.";
                }
            }
            catch (Exception ex)
            {
                if (!trang.IsDisposed)
                    trang.Dispose();

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không thể quay lại biểu mẫu",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // GD5.11 - TRA CUU PHUONG / XA
        // ==========================================

        private static Control Native_TimOPhuongXa(
            UcNhapDonViNative trang)
        {
            if (trang == null || trang.IsDisposed)
                return null;

            // Giao dien moi dung ComboBox.
            Control[] danhSach =
                trang.Controls.Find(
                    "chon_QuanHuyen",
                    true);

            if (danhSach.Length > 0)
                return danhSach[0];

            // Du phong cho giao dien nhap tay cu.
            danhSach =
                trang.Controls.Find(
                    "nhap_QuanHuyen",
                    true);

            return danhSach.Length > 0
                ? danhSach[0]
                : null;
        }

        private async void Native_NhanDienPhuongXaAsync(
            UcNhapDonViNative trang,
            double kinhDo,
            double viDo)
        {
            if (trang == null || trang.IsDisposed)
                return;

            Control oPhuongXa =
                Native_TimOPhuongXa(trang);

            if (oPhuongXa == null)
            {
                statusGeneral.Text =
                    "● Không tìm thấy ô Phường/Xã " +
                    "trên biểu mẫu.";
                return;
            }

            // Ghi lai gia tri ban dau.
            // Neu nguoi dung sua trong luc tra cuu,
            // khong ghi de len du lieu moi.
            string giaTriBanDau =
                (oPhuongXa.Text ?? "").Trim();

            try
            {
                KetQuaPhuongXa2025 ketQua =
                    await Task.Run(() =>
                    {
                        TruyXuatPhuongXa2025 dal =
                            new TruyXuatPhuongXa2025();

                        return dal.XacDinh(
                            kinhDo,
                            viDo);
                    });

                if (IsDisposed ||
                    Disposing ||
                    trang.IsDisposed ||
                    !object.ReferenceEquals(
                        nativeManHinhHoSoDangMo,
                        trang))
                    return;

                if (ketQua == null ||
                    !ketQua.TimThay)
                {
                    statusGeneral.Text =
                        "● Đã nhận WGS84. " +
                        (ketQua == null
                            ? "Không nhận được kết quả địa giới."
                            : ketQua.ThongBao);

                    return;
                }

                Control oHienTai =
                    Native_TimOPhuongXa(trang);

                if (oHienTai == null ||
                    oHienTai.IsDisposed)
                    return;

                string giaTriHienTai =
                    (oHienTai.Text ?? "").Trim();

                if (!string.Equals(
                    giaTriHienTai,
                    giaTriBanDau,
                    StringComparison.Ordinal))
                {
                    statusGeneral.Text =
                        "● Đã xác định " +
                        ketQua.TenPhuongXa +
                        " nhưng ô địa danh đã được chỉnh sửa; " +
                        "không tự ghi đè.";

                    return;
                }

                string tenMoi =
                    ketQua.TenPhuongXa;

                bool daGiong =
                    string.Equals(
                        giaTriHienTai,
                        tenMoi,
                        StringComparison.OrdinalIgnoreCase);

                if (!daGiong &&
                    !string.IsNullOrWhiteSpace(
                        giaTriHienTai))
                {
                    DialogResult chon =
                        MessageBox.Show(
                            this,
                            "Địa danh đang nhập:\n" +
                            giaTriHienTai +
                            "\n\nĐịa danh xác định từ " +
                            "ranh giới PostGIS:\n" +
                            tenMoi +
                            "\n\nAnh có muốn cập nhật " +
                            "theo địa danh trên bản đồ không?",
                            "Xác nhận Phường/Xã",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2);

                    if (chon != DialogResult.Yes)
                    {
                        statusGeneral.Text =
                            "● Đã giữ nguyên địa danh " +
                            "người dùng nhập.";

                        return;
                    }
                }

                if (!daGiong)
                    oHienTai.Text = tenMoi;

                statusGeneral.Text =
                    "● Đã đối chiếu " +
                    tenMoi +
                    " (" +
                    ketQua.MaPhuongXa +
                    ") từ dữ liệu ranh giới PostGIS. " +
                    "Hồ sơ chưa được lưu.";
            }
            catch (Exception ex)
            {
                if (!IsDisposed &&
                    !Disposing &&
                    !trang.IsDisposed &&
                    object.ReferenceEquals(
                        nativeManHinhHoSoDangMo,
                        trang))
                {
                    statusGeneral.Text =
                        "● Đã nhận WGS84, nhưng không " +
                        "tra cứu được Phường/Xã: " +
                        ex.Message;
                }
            }
        }
    }
}