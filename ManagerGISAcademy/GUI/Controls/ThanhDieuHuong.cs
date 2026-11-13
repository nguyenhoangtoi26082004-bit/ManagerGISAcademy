
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Controls
{
    /// <summary>
    /// Bảy phân hệ của ứng dụng quản trị CSDL không gian.
    /// </summary>
    public enum PhanHeDieuHuong
    {
        TongQuan = 0,
        BanDoGIS = 1,
        TraCuuDonVi = 2,
        QuanLyDuLieu = 3,
        PhanTichKhongGian = 4,
        ThongKeBaoCao = 5,
        KiemTraDuLieu = 6
    }

    /// <summary>
    /// Dữ liệu gửi về FormMain khi người dùng chọn phân hệ.
    /// </summary>
    public sealed class SuKienDieuHuongArgs : EventArgs
    {
        public PhanHeDieuHuong PhanHe { get; private set; }
        public string TenPhanHe { get; private set; }

        public SuKienDieuHuongArgs(
            PhanHeDieuHuong phanHe,
            string tenPhanHe)
        {
            PhanHe = phanHe;
            TenPhanHe = tenPhanHe ?? string.Empty;
        }
    }

    /// <summary>
    /// Thanh điều hướng bên trái.
    /// Chỉ phát sự kiện, không mở Form, không truy vấn PostGIS
    /// và không gọi MapWinGIS trực tiếp.
    /// Tương thích C# 7.3 / Windows Forms .NET Framework 4.8.
    /// </summary>
    public class ThanhDieuHuong : UserControl
    {
        // ============================================
        // 1. KHAI BÁO ĐIỀU KHIỂN
        // ============================================

        private readonly Panel pnlDau;
        private readonly Panel pnlGiua;
        private readonly Panel pnlCuoi;

        private readonly TableLayoutPanel bangNut;

        private readonly Label lblDanhMuc;
        private readonly Label lblGoiY;
        private readonly Label lblThongTin;
        private readonly Label lblRutGon;

        private readonly Button btnThuGon;
        private readonly ToolTip chuThich;

        private readonly Dictionary<PhanHeDieuHuong, Button> cacNut;

        // ============================================
        // 2. MÀU SẮC GIAO DIỆN
        // ============================================

        private readonly Color mauNen =
            Color.FromArgb(17, 29, 46);

        private readonly Color mauNut =
            Color.FromArgb(17, 29, 46);

        private readonly Color mauDangChon =
            Color.FromArgb(34, 91, 76);

        private readonly Color mauChu =
            Color.FromArgb(207, 220, 232);

        private bool dangThuGon;

        private PhanHeDieuHuong phanHeDangChon;

        // ============================================
        // 3. SỰ KIỆN ĐIỀU HƯỚNG
        // ============================================

        public event EventHandler<SuKienDieuHuongArgs>
            YeuCauChuyenPhanHe;

        // ============================================
        // 4. KHỞI TẠO THANH ĐIỀU HƯỚNG
        // ============================================

        public ThanhDieuHuong()
        {
            Name = "NavigationSidebar";

            Dock = DockStyle.Left;
            Width = 234;
            MinimumSize = new Size(68, 0);

            BackColor = mauNen;
            Font = new Font("Segoe UI", 9F);

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            cacNut = new Dictionary<PhanHeDieuHuong, Button>();

            chuThich = new ToolTip();
            chuThich.InitialDelay = 350;
            chuThich.AutoPopDelay = 7000;

            // ========================================
            // 4.1. KHUNG ĐẦU THANH ĐIỀU HƯỚNG
            // ========================================

            pnlDau = new Panel();

            pnlDau.Name = "pnlDau";
            pnlDau.Dock = DockStyle.Top;
            pnlDau.Height = 99;

            pnlDau.BackColor =
                Color.FromArgb(14, 25, 41);

            lblDanhMuc = new Label();

            lblDanhMuc.Name = "lblDanhMuc";
            lblDanhMuc.Text = "DANH MỤC CHỨC NĂNG";

            lblDanhMuc.Location = new Point(17, 22);
            lblDanhMuc.Size = new Size(177, 25);

            lblDanhMuc.ForeColor = Color.White;

            lblDanhMuc.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold);

            lblDanhMuc.AutoEllipsis = true;

            lblGoiY = new Label();

            lblGoiY.Name = "lblGoiY";
            lblGoiY.Text = "Điều hướng hệ thống";

            lblGoiY.Location = new Point(17, 55);
            lblGoiY.Size = new Size(186, 23);

            lblGoiY.ForeColor =
                Color.FromArgb(148, 163, 184);

            lblGoiY.Font = new Font(
                "Segoe UI",
                8.5F);

            // ========================================
            // 4.2. NÚT THU GỌN
            // ========================================

            btnThuGon = new Button();

            btnThuGon.Name = "btnThuGon";
            btnThuGon.Text = "«";

            btnThuGon.Size = new Size(30, 30);
            btnThuGon.Location = new Point(196, 11);

            btnThuGon.BackColor =
                Color.FromArgb(30, 48, 67);

            btnThuGon.ForeColor = Color.White;

            btnThuGon.FlatStyle =
                FlatStyle.Flat;

            btnThuGon.FlatAppearance.BorderSize = 0;

            btnThuGon.Cursor = Cursors.Hand;
            btnThuGon.UseVisualStyleBackColor = false;

            btnThuGon.Click += BtnThuGon_Click;

            chuThich.SetToolTip(
                btnThuGon,
                "Thu gọn hoặc mở rộng thanh điều hướng");

            pnlDau.Controls.Add(lblDanhMuc);
            pnlDau.Controls.Add(lblGoiY);
            pnlDau.Controls.Add(btnThuGon);

            pnlDau.Resize += PnlDau_Resize;

            // ========================================
            // 4.3. KHUNG CHÂN THANH ĐIỀU HƯỚNG
            // ========================================

            pnlCuoi = new Panel();

            pnlCuoi.Name = "pnlCuoi";
            pnlCuoi.Dock = DockStyle.Bottom;
            pnlCuoi.Height = 79;

            pnlCuoi.BackColor =
                Color.FromArgb(13, 24, 39);

            lblThongTin = new Label();

            lblThongTin.Name = "lblThongTin";
            lblThongTin.Dock = DockStyle.Fill;

            lblThongTin.Padding =
                new Padding(17, 0, 5, 0);

            lblThongTin.Text =
                "HỆ QUẢN TRỊ CSDL KHÔNG GIAN\r\n" +
                "Giao diện thử nghiệm";

            lblThongTin.ForeColor =
                Color.FromArgb(148, 163, 184);

            lblThongTin.Font =
                new Font("Segoe UI", 8F);

            lblThongTin.TextAlign =
                ContentAlignment.MiddleLeft;

            lblRutGon = new Label();

            lblRutGon.Name = "lblRutGon";
            lblRutGon.Dock = DockStyle.Fill;

            lblRutGon.Text = "GIS";

            lblRutGon.Font = new Font(
                "Segoe UI",
                11F,
                FontStyle.Bold);

            lblRutGon.ForeColor =
                Color.FromArgb(110, 231, 183);

            lblRutGon.TextAlign =
                ContentAlignment.MiddleCenter;

            lblRutGon.Visible = false;

            pnlCuoi.Controls.Add(lblThongTin);
            pnlCuoi.Controls.Add(lblRutGon);

            // ========================================
            // 4.4. VÙNG DANH SÁCH CÓ THANH CUỘN
            // ========================================

            pnlGiua = new Panel();

            pnlGiua.Name = "pnlGiua";
            pnlGiua.Dock = DockStyle.Fill;

            pnlGiua.AutoScroll = true;
            pnlGiua.BackColor = mauNen;

            bangNut = new TableLayoutPanel();

            bangNut.Name = "bangNut";
            bangNut.Dock = DockStyle.Top;

            bangNut.Height = 384;
            bangNut.ColumnCount = 1;
            bangNut.RowCount = 7;

            bangNut.Padding =
                new Padding(8, 7, 8, 0);

            bangNut.BackColor = mauNen;

            bangNut.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            for (int i = 0; i < 7; i++)
            {
                bangNut.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute,
                        53F));
            }

            // ========================================
            // 4.5. TẠO BẢY NÚT CHỨC NĂNG
            // ========================================

            ThemNut(
                PhanHeDieuHuong.TongQuan,
                "Tổng quan",
                0);

            ThemNut(
                PhanHeDieuHuong.BanDoGIS,
                "Bản đồ GIS",
                1);

            ThemNut(
                PhanHeDieuHuong.TraCuuDonVi,
                "Tra cứu đơn vị",
                2);

            ThemNut(
                PhanHeDieuHuong.QuanLyDuLieu,
                "Quản lý dữ liệu",
                3);

            ThemNut(
                PhanHeDieuHuong.PhanTichKhongGian,
                "Phân tích không gian",
                4);

            ThemNut(
                PhanHeDieuHuong.ThongKeBaoCao,
                "Thống kê, báo cáo",
                5);

            ThemNut(
                PhanHeDieuHuong.KiemTraDuLieu,
                "Kiểm tra dữ liệu",
                6);

            pnlGiua.Controls.Add(bangNut);

            // ========================================
            // 4.6. GHÉP TOÀN BỘ SIDEBAR
            // ========================================

            Controls.Add(pnlGiua);
            Controls.Add(pnlCuoi);
            Controls.Add(pnlDau);

            // Mặc định chọn phân hệ Bản đồ GIS.
            ChonPhanHe(
                PhanHeDieuHuong.BanDoGIS);

            CapNhatGiaoDienThuGon();
        }

        // ============================================
        // 5. THUỘC TÍNH CÔNG KHAI
        // ============================================

        public PhanHeDieuHuong PhanHeDangChon
        {
            get
            {
                return phanHeDangChon;
            }
        }

        public bool DangThuGon
        {
            get
            {
                return dangThuGon;
            }
            set
            {
                if (dangThuGon == value)
                {
                    return;
                }

                dangThuGon = value;

                Width = dangThuGon ? 68 : 234;

                CapNhatGiaoDienThuGon();
            }
        }

        // ============================================
        // 6. ĐỒNG BỘ PHÂN HỆ ĐANG CHỌN
        // ============================================

        /// <summary>
        /// Được FormMain gọi để đồng bộ mục đang hiển thị.
        /// Không phát lại sự kiện điều hướng.
        /// </summary>
        public void ChonPhanHe(
            PhanHeDieuHuong phanHe)
        {
            if (!cacNut.ContainsKey(phanHe))
            {
                throw new ArgumentOutOfRangeException(
                    "phanHe");
            }

            phanHeDangChon = phanHe;

            foreach (
                KeyValuePair<PhanHeDieuHuong, Button> item
                in cacNut)
            {
                bool duocChon =
                    item.Key == phanHe;

                item.Value.BackColor =
                    duocChon ? mauDangChon : mauNut;

                item.Value.ForeColor =
                    duocChon ? Color.White : mauChu;

                item.Value.FlatAppearance.MouseOverBackColor =
                    duocChon
                        ? Color.FromArgb(40, 114, 93)
                        : Color.FromArgb(31, 52, 71);
            }
        }

        // ============================================
        // 7. CHO PHÉP / KHÓA PHÂN HỆ
        // ============================================

        /// <summary>
        /// Điều khiển khả năng chọn một phân hệ theo quyền.
        /// Kiểm tra phân quyền thực tế vẫn phải có ở BLL/DAL.
        /// </summary>
        public void ChoPhepPhanHe(
            PhanHeDieuHuong phanHe,
            bool choPhep)
        {
            Button nut;

            if (!cacNut.TryGetValue(
                phanHe,
                out nut))
            {
                throw new ArgumentOutOfRangeException(
                    "phanHe");
            }

            nut.Enabled = choPhep;
        }

        // ============================================
        // 8. TẠO NÚT ĐIỀU HƯỚNG
        // ============================================

        private void ThemNut(
            PhanHeDieuHuong phanHe,
            string ten,
            int viTri)
        {
            Button nut = new Button();

            nut.Name =
                "btn" + phanHe.ToString();

            nut.Tag = phanHe;
            nut.Text = "   " + ten;

            nut.AccessibleName = ten;

            nut.AccessibleDescription =
                "Chuyển tới phân hệ " + ten;

            nut.Dock = DockStyle.Fill;

            nut.Margin =
                new Padding(2, 4, 2, 4);

            nut.BackColor = mauNut;
            nut.ForeColor = mauChu;

            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;

            nut.UseVisualStyleBackColor = false;
            nut.Cursor = Cursors.Hand;

            nut.TextAlign =
                ContentAlignment.MiddleLeft;

            nut.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold);

            nut.Click += NutDieuHuong_Click;

            chuThich.SetToolTip(nut, ten);

            cacNut.Add(phanHe, nut);

            bangNut.Controls.Add(
                nut,
                0,
                viTri);
        }

        // ============================================
        // 9. XỬ LÝ KHI NGƯỜI DÙNG CHỌN PHÂN HỆ
        // ============================================

        private void NutDieuHuong_Click(
            object sender,
            EventArgs e)
        {
            Button nut = sender as Button;

            if (nut == null ||
                !nut.Enabled ||
                !(nut.Tag is PhanHeDieuHuong))
            {
                return;
            }

            PhanHeDieuHuong phanHe =
                (PhanHeDieuHuong)nut.Tag;

            EventHandler<SuKienDieuHuongArgs> suKien =
                YeuCauChuyenPhanHe;

            if (suKien == null)
            {
                MessageBox.Show(
                    this,
                    "Thanh điều hướng chưa được FormMain " +
                    "đăng ký xử lý.\n" +
                    "Chức năng chưa được mở.",
                    "Chưa tích hợp điều hướng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                suKien(
                    this,
                    new SuKienDieuHuongArgs(
                        phanHe,
                        nut.AccessibleName));

                ChonPhanHe(phanHe);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Không thể chuyển đến phân hệ " +
                    "đã chọn:\n" +
                    ex.Message,
                    "Lỗi điều hướng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 10. NÚT THU GỌN / MỞ RỘNG
        // ============================================

        private void BtnThuGon_Click(
            object sender,
            EventArgs e)
        {
            DangThuGon = !DangThuGon;
        }

        private void PnlDau_Resize(
            object sender,
            EventArgs e)
        {
            if (btnThuGon != null &&
                pnlDau != null)
            {
                btnThuGon.Left = Math.Max(
                    5,
                    pnlDau.ClientSize.Width - 39);
            }
        }

        // ============================================
        // 11. CẬP NHẬT GIAO DIỆN THU GỌN
        // ============================================

        private void CapNhatGiaoDienThuGon()
        {
            if (btnThuGon == null)
            {
                return;
            }

            lblDanhMuc.Visible = !dangThuGon;
            lblGoiY.Visible = !dangThuGon;
            lblThongTin.Visible = !dangThuGon;

            lblRutGon.Visible = dangThuGon;

            btnThuGon.Text =
                dangThuGon ? "»" : "«";

            btnThuGon.Left = Math.Max(
                5,
                pnlDau.ClientSize.Width - 39);

            foreach (
                KeyValuePair<PhanHeDieuHuong, Button> item
                in cacNut)
            {
                Button nut = item.Value;

                string ten = TenPhanHe(
                    item.Key);

                nut.Text = dangThuGon
                    ? KyHieuPhanHe(item.Key)
                    : "   " + ten;

                nut.TextAlign = dangThuGon
                    ? ContentAlignment.MiddleCenter
                    : ContentAlignment.MiddleLeft;
            }
        }

        // ============================================
        // 12. LẤY TÊN PHÂN HỆ
        // ============================================

        private static string TenPhanHe(
            PhanHeDieuHuong phanHe)
        {
            switch (phanHe)
            {
                case PhanHeDieuHuong.TongQuan:
                    return "Tổng quan";

                case PhanHeDieuHuong.BanDoGIS:
                    return "Bản đồ GIS";

                case PhanHeDieuHuong.TraCuuDonVi:
                    return "Tra cứu đơn vị";

                case PhanHeDieuHuong.QuanLyDuLieu:
                    return "Quản lý dữ liệu";

                case PhanHeDieuHuong.PhanTichKhongGian:
                    return "Phân tích không gian";

                case PhanHeDieuHuong.ThongKeBaoCao:
                    return "Thống kê, báo cáo";

                case PhanHeDieuHuong.KiemTraDuLieu:
                    return "Kiểm tra dữ liệu";

                default:
                    return "Không xác định";
            }
        }

        // ============================================
        // 13. KÝ HIỆU KHI THU GỌN
        // ============================================

        private static string KyHieuPhanHe(
            PhanHeDieuHuong phanHe)
        {
            switch (phanHe)
            {
                case PhanHeDieuHuong.TongQuan:
                    return "TQ";

                case PhanHeDieuHuong.BanDoGIS:
                    return "BĐ";

                case PhanHeDieuHuong.TraCuuDonVi:
                    return "TC";

                case PhanHeDieuHuong.QuanLyDuLieu:
                    return "QL";

                case PhanHeDieuHuong.PhanTichKhongGian:
                    return "PT";

                case PhanHeDieuHuong.ThongKeBaoCao:
                    return "BC";

                case PhanHeDieuHuong.KiemTraDuLieu:
                    return "KT";

                default:
                    return "?";
            }
        }

        // ============================================
        // 14. GIẢI PHÓNG TÀI NGUYÊN
        // ============================================

        protected override void Dispose(
            bool disposing)
        {
            if (disposing && chuThich != null)
            {
                chuThich.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
