
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Controls
{
    /// <summary>
    /// Dữ liệu hiển thị trong bảng chi tiết đơn vị.
    /// Chỉ truyền những trường mà người dùng hiện tại được phép xem.
    /// Kiểm tra quyền đọc/sửa dữ liệu thật phải được thực hiện ở BLL/DAL.
    /// </summary>
    public sealed class ThongTinDonViPanel
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenVietTat { get; set; }
        public string LoaiDonVi { get; set; }
        public string QuanBinhChung { get; set; }
        public string DonViCapTren { get; set; }
        public string QuanHuyen { get; set; }
        public string DiaChi { get; set; }
        public double? ViDo { get; set; }
        public double? KinhDo { get; set; }
        public double? XPhang { get; set; }
        public double? YPhang { get; set; }
        public string HeQuyChieuPhang { get; set; }
        public string ChiHuyTruong { get; set; }
        public string ChinhUy { get; set; }
        public string SoDienThoai { get; set; }
        public string TrangThaiSSCD { get; set; }
        public string DoMat { get; set; }
        public string NhiemVuChinh { get; set; }
        public string VuKhiTrangBi { get; set; }

        public bool CoToaDoWgs84HopLe
        {
            get
            {
                if (!ViDo.HasValue || !KinhDo.HasValue)
                    return false;

                double viDo = ViDo.Value;
                double kinhDo = KinhDo.Value;

                return !double.IsNaN(viDo) &&
                       !double.IsInfinity(viDo) &&
                       !double.IsNaN(kinhDo) &&
                       !double.IsInfinity(kinhDo) &&
                       viDo >= -90 && viDo <= 90 &&
                       kinhDo >= -180 && kinhDo <= 180 &&
                       !(viDo == 0 && kinhDo == 0);
            }
        }
    }

    /// <summary>
    /// Dữ liệu kèm theo yêu cầu từ UnitDetailPanel đến FormMain.
    /// </summary>
    public sealed class SuKienUnitDetailArgs : EventArgs
    {
        public ThongTinDonViPanel DonVi { get; private set; }

        public SuKienUnitDetailArgs(ThongTinDonViPanel donVi)
        {
            if (donVi == null)
                throw new ArgumentNullException("donVi");

            DonVi = donVi;
        }
    }

    /// <summary>
    /// Bảng chi tiết đơn vị gắn bên phải bản đồ.
    /// Chỉ hiển thị thông tin và phát sự kiện, không chạy SQL/MapWinGIS.
    /// Windows Forms .NET Framework 4.8 - C# 7.3.
    /// Giao diện tạo bằng code, không cần UnitDetailPanel.Designer.cs.
    /// </summary>
    public class BangChiTietDonVi : UserControl
    {
        // ============================================
        // 1. KHAI BÁO ĐIỀU KHIỂN
        // ============================================

        private readonly Panel pnlDau;
        private readonly Panel pnlNoiDung;
        private readonly Panel pnlChan;

        private readonly TableLayoutPanel bangNhom;
        private readonly TableLayoutPanel bangNut;

        private readonly Label lblTieuDe;
        private readonly Label lblTenDonVi;
        private readonly Label lblMaDonVi;
        private readonly Label lblTrangThaiSSCD;
        private readonly Label lblThongBao;

        private readonly Button btnXemHoSo;
        private readonly Button btnDinhVi;
        private readonly Button btnChinhSua;
        private readonly Button btnDong;

        private readonly ToolTip chuThich;

        private readonly Dictionary<string, Label> cacGiaTri;

        private ThongTinDonViPanel donViDangXem;

        private bool choPhepChinhSua;
        private bool choPhepDinhVi = true;

        // ============================================
        // 2. CÁC SỰ KIỆN GỬI CHO FORMMAIN
        // ============================================

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauXemHoSo;

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauHienThiTrenBanDo;

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauChinhSua;

        public event EventHandler YeuCauDong;

        // ============================================
        // 3. KHỞI TẠO GIAO DIỆN
        // ============================================

        public BangChiTietDonVi()
        {
            Name = "UnitDetailPanel";

            Dock = DockStyle.Right;
            Width = 385;

            MinimumSize = new Size(310, 350);

            BackColor = Color.FromArgb(17, 24, 39);
            ForeColor = Color.White;

            Font = new Font("Segoe UI", 9F);

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            chuThich = new ToolTip();

            cacGiaTri = new Dictionary<string, Label>(
                StringComparer.Ordinal);

            // ========================================
            // 3.1. PHẦN TIÊU ĐỀ
            // ========================================

            pnlDau = new Panel();

            pnlDau.Name = "pnlDau";
            pnlDau.Dock = DockStyle.Top;
            pnlDau.Height = 139;

            pnlDau.BackColor =
                Color.FromArgb(11, 19, 36);

            lblTieuDe = new Label();

            lblTieuDe.Name = "lblTieuDe";
            lblTieuDe.Text = "THÔNG TIN CHI TIẾT ĐƠN VỊ";

            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 33;

            lblTieuDe.Padding =
                new Padding(14, 9, 8, 0);

            lblTieuDe.ForeColor =
                Color.FromArgb(110, 231, 183);

            lblTieuDe.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold);

            lblTieuDe.AutoEllipsis = true;

            // Tên đơn vị đang xem
            lblTenDonVi = new Label();

            lblTenDonVi.Name = "lblTenDonVi";
            lblTenDonVi.Text = "CHƯA CHỌN ĐƠN VỊ";

            lblTenDonVi.Dock = DockStyle.Top;
            lblTenDonVi.Height = 43;

            lblTenDonVi.Padding =
                new Padding(14, 2, 10, 0);

            lblTenDonVi.ForeColor = Color.White;

            lblTenDonVi.Font = new Font(
                "Segoe UI",
                12F,
                FontStyle.Bold);

            lblTenDonVi.AutoEllipsis = true;

            // Mã đơn vị
            lblMaDonVi = new Label();

            lblMaDonVi.Name = "lblMaDonVi";
            lblMaDonVi.Text = "Mã đơn vị: —";

            lblMaDonVi.Dock = DockStyle.Top;
            lblMaDonVi.Height = 27;

            lblMaDonVi.Padding =
                new Padding(14, 2, 10, 0);

            lblMaDonVi.ForeColor =
                Color.FromArgb(203, 213, 225);

            lblMaDonVi.AutoEllipsis = true;

            // Trạng thái sẵn sàng chiến đấu
            lblTrangThaiSSCD = new Label();

            lblTrangThaiSSCD.Name = "lblTrangThaiSSCD";
            lblTrangThaiSSCD.Text =
                "Trạng thái: Chưa xác định";

            lblTrangThaiSSCD.Dock = DockStyle.Fill;

            lblTrangThaiSSCD.Padding =
                new Padding(14, 0, 10, 0);

            lblTrangThaiSSCD.ForeColor =
                Color.FromArgb(203, 213, 225);

            lblTrangThaiSSCD.TextAlign =
                ContentAlignment.MiddleLeft;

            lblTrangThaiSSCD.AutoEllipsis = true;

            pnlDau.Controls.Add(lblTrangThaiSSCD);
            pnlDau.Controls.Add(lblMaDonVi);
            pnlDau.Controls.Add(lblTenDonVi);
            pnlDau.Controls.Add(lblTieuDe);

            // ========================================
            // 3.2. VÙNG NỘI DUNG CÓ THANH CUỘN
            // ========================================

            pnlNoiDung = new Panel();

            pnlNoiDung.Name = "pnlNoiDung";
            pnlNoiDung.Dock = DockStyle.Fill;

            pnlNoiDung.AutoScroll = true;

            pnlNoiDung.BackColor =
                Color.FromArgb(17, 24, 39);

            pnlNoiDung.Padding =
                new Padding(8, 8, 8, 8);

            bangNhom = new TableLayoutPanel();

            bangNhom.Name = "bangNhom";
            bangNhom.Dock = DockStyle.Top;

            bangNhom.ColumnCount = 1;
            bangNhom.RowCount = 4;

            bangNhom.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            bangNhom.Margin = new Padding(0);
            bangNhom.Padding = new Padding(0);

            // ========================================
            // 3.3. NHÓM THÔNG TIN CHUNG
            // ========================================

            GroupBox grpChung = TaoNhom(
                "THÔNG TIN CHUNG",
                new string[]
                {
                    "Ma",
                    "Ten",
                    "VietTat",
                    "Loai",
                    "BinhChung",
                    "CapTren"
                },
                new string[]
                {
                    "Mã đơn vị",
                    "Tên đơn vị",
                    "Tên viết tắt",
                    "Loại đơn vị",
                    "Quân/Binh chủng",
                    "Đơn vị cấp trên"
                },
                43);

            // ========================================
            // 3.4. NHÓM VỊ TRÍ VÀ TỌA ĐỘ
            // ========================================

            GroupBox grpViTri = TaoNhom(
                "VỊ TRÍ VÀ TỌA ĐỘ",
                new string[]
                {
                    "DiaBan",
                    "DiaChi",
                    "ViDo",
                    "KinhDo",
                    "XPhang",
                    "YPhang",
                    "CRS"
                },
                new string[]
                {
                    "Địa bàn",
                    "Địa chỉ",
                    "Vĩ độ WGS84",
                    "Kinh độ WGS84",
                    "X phẳng (m)",
                    "Y phẳng (m)",
                    "CRS của X/Y"
                },
                43);

            // ========================================
            // 3.5. NHÓM CHỈ HUY VÀ TRẠNG THÁI
            // ========================================

            GroupBox grpChiHuy = TaoNhom(
                "CHỈ HUY VÀ TRẠNG THÁI",
                new string[]
                {
                    "ChiHuy",
                    "ChinhUy",
                    "DienThoai",
                    "SSCD",
                    "DoMat"
                },
                new string[]
                {
                    "Chỉ huy trưởng",
                    "Chính ủy",
                    "Điện thoại",
                    "Trạng thái SSCĐ",
                    "Độ mật"
                },
                43);

            // ========================================
            // 3.6. NHÓM NHIỆM VỤ VÀ TRANG BỊ
            // ========================================

            GroupBox grpNhiemVu = TaoNhom(
                "NHIỆM VỤ VÀ TRANG BỊ",
                new string[]
                {
                    "NhiemVu",
                    "TrangBi"
                },
                new string[]
                {
                    "Nhiệm vụ chính",
                    "Vũ khí, trang bị"
                },
                82);

            GroupBox[] cacNhom =
            {
                grpChung,
                grpViTri,
                grpChiHuy,
                grpNhiemVu
            };

            int tongCao = 0;

            for (int i = 0; i < cacNhom.Length; i++)
            {
                bangNhom.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute,
                        cacNhom[i].Height + 10));

                bangNhom.Controls.Add(
                    cacNhom[i],
                    0,
                    i);

                tongCao += cacNhom[i].Height + 10;
            }

            bangNhom.Height = tongCao;

            pnlNoiDung.Controls.Add(bangNhom);

            // ========================================
            // 3.7. THANH CHỨC NĂNG
            // ========================================

            pnlChan = new Panel();

            pnlChan.Name = "pnlChan";
            pnlChan.Dock = DockStyle.Bottom;
            pnlChan.Height = 139;

            pnlChan.BackColor =
                Color.FromArgb(15, 23, 42);

            pnlChan.Padding =
                new Padding(10, 8, 10, 8);

            bangNut = new TableLayoutPanel();

            bangNut.Name = "bangNut";
            bangNut.Dock = DockStyle.Fill;

            bangNut.ColumnCount = 2;
            bangNut.RowCount = 2;

            bangNut.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            bangNut.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            bangNut.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            bangNut.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            // ========================================
            // 3.8. BỐN NÚT CHỨC NĂNG
            // ========================================

            btnXemHoSo = TaoNut(
                "btnXemHoSo",
                "XEM HỒ SƠ",
                Color.FromArgb(30, 64, 175));

            btnDinhVi = TaoNut(
                "btnDinhVi",
                "ĐỊNH VỊ",
                Color.FromArgb(5, 150, 105));

            btnChinhSua = TaoNut(
                "btnChinhSua",
                "CHỈNH SỬA",
                Color.FromArgb(146, 64, 14));

            btnDong = TaoNut(
                "btnDong",
                "ĐÓNG BẢNG",
                Color.FromArgb(51, 65, 85));

            bangNut.Controls.Add(btnXemHoSo, 0, 0);
            bangNut.Controls.Add(btnDinhVi, 1, 0);
            bangNut.Controls.Add(btnChinhSua, 0, 1);
            bangNut.Controls.Add(btnDong, 1, 1);

            // ========================================
            // 3.9. THÔNG BÁO TRẠNG THÁI
            // ========================================

            lblThongBao = new Label();

            lblThongBao.Name = "lblThongBao";
            lblThongBao.Dock = DockStyle.Bottom;
            lblThongBao.Height = 27;

            lblThongBao.ForeColor =
                Color.FromArgb(148, 163, 184);

            lblThongBao.TextAlign =
                ContentAlignment.MiddleLeft;

            lblThongBao.AutoEllipsis = true;
            lblThongBao.Text = "Chưa chọn đối tượng.";

            pnlChan.Controls.Add(bangNut);
            pnlChan.Controls.Add(lblThongBao);

            // ========================================
            // 3.10. GHÉP CONTROL
            // ========================================

            Controls.Add(pnlNoiDung);
            Controls.Add(pnlChan);
            Controls.Add(pnlDau);

            // ========================================
            // 3.11. ĐĂNG KÝ SỰ KIỆN
            // ========================================

            btnXemHoSo.Click += BtnXemHoSo_Click;
            btnDinhVi.Click += BtnDinhVi_Click;
            btnChinhSua.Click += BtnChinhSua_Click;
            btnDong.Click += BtnDong_Click;

            XoaDuLieu();
        }

        // ============================================
        // 4. THUỘC TÍNH CÔNG KHAI
        // ============================================

        public ThongTinDonViPanel DuLieuDangXem
        {
            get
            {
                return donViDangXem;
            }
        }

        public string MaDonViDangXem
        {
            get
            {
                return donViDangXem == null
                    ? string.Empty
                    : donViDangXem.MaDonVi ?? string.Empty;
            }
        }

        public bool ChoPhepChinhSua
        {
            get
            {
                return choPhepChinhSua;
            }
            set
            {
                choPhepChinhSua = value;
                CapNhatNut();
            }
        }

        public bool ChoPhepDinhVi
        {
            get
            {
                return choPhepDinhVi;
            }
            set
            {
                choPhepDinhVi = value;
                CapNhatNut();
            }
        }

        // ============================================
        // 5. NHẬN DỮ LIỆU TỪ FORMMAIN / BLL
        // ============================================

        public void GanDuLieu(ThongTinDonViPanel duLieu)
        {
            if (duLieu == null ||
                string.IsNullOrWhiteSpace(duLieu.MaDonVi))
            {
                XoaDuLieu();
                return;
            }

            donViDangXem = duLieu;

            Visible = true;

            lblMaDonVi.Text =
                "Mã đơn vị: " + duLieu.MaDonVi.Trim();

            lblTenDonVi.Text =
                HienThi(duLieu.TenDonVi).ToUpperInvariant();

            lblTrangThaiSSCD.Text =
                "Trạng thái: " +
                HienThi(duLieu.TrangThaiSSCD);

            DoiMauTrangThai(duLieu.TrangThaiSSCD);

            // Thông tin chung
            GanNhan("Ma", duLieu.MaDonVi);
            GanNhan("Ten", duLieu.TenDonVi);
            GanNhan("VietTat", duLieu.TenVietTat);
            GanNhan("Loai", duLieu.LoaiDonVi);
            GanNhan("BinhChung", duLieu.QuanBinhChung);
            GanNhan("CapTren", duLieu.DonViCapTren);

            // Vị trí và tọa độ
            GanNhan("DiaBan", duLieu.QuanHuyen);
            GanNhan("DiaChi", duLieu.DiaChi);

            GanNhan(
                "ViDo",
                duLieu.CoToaDoWgs84HopLe
                    ? DinhDangSo(duLieu.ViDo, "0.########")
                    : "Chưa có");

            GanNhan(
                "KinhDo",
                duLieu.CoToaDoWgs84HopLe
                    ? DinhDangSo(duLieu.KinhDo, "0.########")
                    : "Chưa có");

            GanNhan(
                "XPhang",
                DinhDangSo(duLieu.XPhang, "0.##"));

            GanNhan(
                "YPhang",
                DinhDangSo(duLieu.YPhang, "0.##"));

            GanNhan("CRS", duLieu.HeQuyChieuPhang);

            // Chỉ huy và trạng thái
            GanNhan("ChiHuy", duLieu.ChiHuyTruong);
            GanNhan("ChinhUy", duLieu.ChinhUy);
            GanNhan("DienThoai", duLieu.SoDienThoai);
            GanNhan("SSCD", duLieu.TrangThaiSSCD);
            GanNhan("DoMat", duLieu.DoMat);

            // Nhiệm vụ và trang bị
            GanNhan("NhiemVu", duLieu.NhiemVuChinh);
            GanNhan("TrangBi", duLieu.VuKhiTrangBi);

            lblThongBao.Text =
                "Đang xem: " + duLieu.MaDonVi.Trim();

            CapNhatNut();
        }

        // ============================================
        // 6. HÀM TƯƠNG THÍCH VỚI FORM CHI TIẾT
        // ============================================

        /// <summary>
        /// Nhận 19 trường tương ứng với
        /// FormChiTietDonVi.GanDuLieuChiTiet().
        ///
        /// Chỉ cặp WGS84 (0, 0) được coi là thiếu tọa độ.
        /// CRS của X/Y chưa biết thì để trống.
        /// </summary>
        public void GanDuLieuChiTiet(
            string maDonVi,
            string tenDonVi,
            string tenVietTat,
            string loaiDonVi,
            string quanBinhChung,
            string donViCapTren,
            string quanHuyen,
            string diaChi,
            double viDo,
            double kinhDo,
            double xVn2000,
            double yVn2000,
            string chiHuyTruong,
            string chinhUy,
            string soDienThoai,
            string trangThaiSSCD,
            string doMat,
            string nhiemVuChinh,
            string vuKhiTrangBi)
        {
            GanDuLieu(new ThongTinDonViPanel
            {
                MaDonVi = maDonVi,
                TenDonVi = tenDonVi,
                TenVietTat = tenVietTat,
                LoaiDonVi = loaiDonVi,
                QuanBinhChung = quanBinhChung,
                DonViCapTren = donViCapTren,
                QuanHuyen = quanHuyen,
                DiaChi = diaChi,

                ViDo = viDo,
                KinhDo = kinhDo,

                XPhang = xVn2000 == 0
                    ? (double?)null
                    : xVn2000,

                YPhang = yVn2000 == 0
                    ? (double?)null
                    : yVn2000,

                ChiHuyTruong = chiHuyTruong,
                ChinhUy = chinhUy,
                SoDienThoai = soDienThoai,
                TrangThaiSSCD = trangThaiSSCD,
                DoMat = doMat,
                NhiemVuChinh = nhiemVuChinh,
                VuKhiTrangBi = vuKhiTrangBi
            });
        }

        // ============================================
        // 7. XÓA DỮ LIỆU HIỂN THỊ
        // ============================================

        public void XoaDuLieu()
        {
            donViDangXem = null;

            lblTenDonVi.Text =
                "CHƯA CHỌN ĐƠN VỊ";

            lblMaDonVi.Text =
                "Mã đơn vị: —";

            lblTrangThaiSSCD.Text =
                "Trạng thái: Chưa xác định";

            lblTrangThaiSSCD.ForeColor =
                Color.FromArgb(203, 213, 225);

            foreach (
                KeyValuePair<string, Label> item
                in cacGiaTri)
            {
                item.Value.Text = "—";

                chuThich.SetToolTip(
                    item.Value,
                    "");
            }

            lblThongBao.Text =
                "Chưa chọn đối tượng.";

            CapNhatNut();
        }

        // ============================================
        // 8. HÀM TẠO NHÓM THÔNG TIN
        // ============================================

        private GroupBox TaoNhom(
            string tieuDe,
            string[] maTruong,
            string[] tenTruong,
            int chieuCaoDong)
        {
            if (maTruong.Length != tenTruong.Length)
            {
                throw new ArgumentException(
                    "Số tên trường không khớp.");
            }

            GroupBox nhom = new GroupBox();

            nhom.Name = "grp" + maTruong[0];
            nhom.Text = "  " + tieuDe + "  ";

            nhom.Dock = DockStyle.Fill;

            nhom.Height =
                45 + maTruong.Length * chieuCaoDong;

            nhom.Margin =
                new Padding(3, 3, 3, 7);

            nhom.Padding =
                new Padding(7, 11, 7, 7);

            nhom.BackColor =
                Color.FromArgb(22, 31, 46);

            nhom.ForeColor =
                Color.FromArgb(110, 231, 183);

            nhom.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);

            TableLayoutPanel bang = new TableLayoutPanel();

            bang.Name = "bang" + maTruong[0];
            bang.Dock = DockStyle.Fill;

            bang.ColumnCount = 2;
            bang.RowCount = maTruong.Length;

            bang.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    126F));

            bang.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            for (int i = 0; i < maTruong.Length; i++)
            {
                bang.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute,
                        chieuCaoDong));

                // Nhãn tên trường
                Label nhan = new Label();

                nhan.Name =
                    "lblTen" + maTruong[i];

                nhan.Text = tenTruong[i];
                nhan.Dock = DockStyle.Fill;

                nhan.TextAlign =
                    ContentAlignment.MiddleLeft;

                nhan.ForeColor =
                    Color.FromArgb(148, 163, 184);

                nhan.Font =
                    new Font("Segoe UI", 8.5F);

                nhan.Margin =
                    new Padding(4, 3, 2, 3);

                // Nhãn giá trị
                Label giaTri = new Label();

                giaTri.Name =
                    "lblGiaTri" + maTruong[i];

                giaTri.Text = "—";
                giaTri.Dock = DockStyle.Fill;

                giaTri.AutoEllipsis =
                    chieuCaoDong < 60;

                giaTri.TextAlign =
                    ContentAlignment.MiddleLeft;

                giaTri.ForeColor = Color.White;

                giaTri.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        FontStyle.Regular);

                giaTri.Margin =
                    new Padding(3, 3, 3, 3);

                bang.Controls.Add(nhan, 0, i);
                bang.Controls.Add(giaTri, 1, i);

                cacGiaTri.Add(
                    maTruong[i],
                    giaTri);
            }

            nhom.Controls.Add(bang);

            return nhom;
        }

        // ============================================
        // 9. TẠO NÚT CHỨC NĂNG
        // ============================================

        private static Button TaoNut(
            string ten,
            string noiDung,
            Color mau)
        {
            Button nut = new Button();

            nut.Name = ten;
            nut.Text = noiDung;

            nut.Dock = DockStyle.Fill;

            nut.Margin =
                new Padding(4, 4, 4, 4);

            nut.BackColor = mau;
            nut.ForeColor = Color.White;

            nut.Font =
                new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold);

            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;

            nut.UseVisualStyleBackColor = false;
            nut.Cursor = Cursors.Hand;

            return nut;
        }

        // ============================================
        // 10. CẬP NHẬT GIÁ TRỊ HIỂN THỊ
        // ============================================

        private void GanNhan(
            string ma,
            string noiDung)
        {
            Label nhan;

            if (!cacGiaTri.TryGetValue(ma, out nhan))
            {
                return;
            }

            nhan.Text = HienThi(noiDung);

            chuThich.SetToolTip(
                nhan,
                noiDung ?? string.Empty);
        }

        private static string HienThi(
            string noiDung)
        {
            return string.IsNullOrWhiteSpace(noiDung)
                ? "—"
                : noiDung.Trim();
        }

        private static string DinhDangSo(
            double? so,
            string dinhDang)
        {
            if (!so.HasValue ||
                double.IsNaN(so.Value) ||
                double.IsInfinity(so.Value))
            {
                return "Chưa có";
            }

            return so.Value.ToString(
                dinhDang,
                CultureInfo.InvariantCulture);
        }

        // ============================================
        // 11. ĐỔI MÀU TRẠNG THÁI SSCĐ
        // ============================================

        private void DoiMauTrangThai(
            string trangThai)
        {
            string value =
                (trangThai ?? "").ToLowerInvariant();

            if (value.Contains("cấp 1"))
            {
                lblTrangThaiSSCD.ForeColor =
                    Color.FromArgb(253, 164, 175);
            }
            else if (value.Contains("cấp 2"))
            {
                lblTrangThaiSSCD.ForeColor =
                    Color.FromArgb(251, 191, 36);
            }
            else if (value.Contains("cấp 3"))
            {
                lblTrangThaiSSCD.ForeColor =
                    Color.FromArgb(147, 197, 253);
            }
            else if (value.Contains("bình thường"))
            {
                lblTrangThaiSSCD.ForeColor =
                    Color.FromArgb(110, 231, 183);
            }
            else
            {
                lblTrangThaiSSCD.ForeColor =
                    Color.FromArgb(203, 213, 225);
            }
        }

        // ============================================
        // 12. BẬT / TẮT CÁC NÚT CHỨC NĂNG
        // ============================================

        private void CapNhatNut()
        {
            bool coHoSo =
                donViDangXem != null &&
                !string.IsNullOrWhiteSpace(
                    donViDangXem.MaDonVi);

            btnXemHoSo.Enabled = coHoSo;

            btnDinhVi.Enabled =
                coHoSo &&
                choPhepDinhVi &&
                donViDangXem.CoToaDoWgs84HopLe;

            btnChinhSua.Enabled =
                coHoSo &&
                choPhepChinhSua;
        }

        // ============================================
        // 13. XỬ LÝ NÚT XEM HỒ SƠ
        // ============================================

        private void BtnXemHoSo_Click(
            object sender,
            EventArgs e)
        {
            PhatYeuCau(
                YeuCauXemHoSo,
                "xem hồ sơ đơn vị");
        }

        // ============================================
        // 14. XỬ LÝ NÚT ĐỊNH VỊ
        // ============================================

        private void BtnDinhVi_Click(
            object sender,
            EventArgs e)
        {
            if (donViDangXem == null ||
                !choPhepDinhVi ||
                !donViDangXem.CoToaDoWgs84HopLe)
            {
                return;
            }

            PhatYeuCau(
                YeuCauHienThiTrenBanDo,
                "định vị đơn vị");
        }

        // ============================================
        // 15. XỬ LÝ NÚT CHỈNH SỬA
        // ============================================

        private void BtnChinhSua_Click(
            object sender,
            EventArgs e)
        {
            if (!choPhepChinhSua)
            {
                return;
            }

            PhatYeuCau(
                YeuCauChinhSua,
                "chỉnh sửa đơn vị");
        }

        // ============================================
        // 16. XỬ LÝ NÚT ĐÓNG
        // ============================================

        private void BtnDong_Click(
            object sender,
            EventArgs e)
        {
            Visible = false;

            EventHandler handler = YeuCauDong;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        // ============================================
        // 17. GỬI YÊU CẦU ĐẾN FORMMAIN
        // ============================================

        private void PhatYeuCau(
            EventHandler<SuKienUnitDetailArgs> suKien,
            string tenChucNang)
        {
            if (donViDangXem == null)
            {
                return;
            }

            if (suKien == null)
            {
                lblThongBao.Text =
                    "Chưa kết nối: " + tenChucNang;

                MessageBox.Show(
                    this,
                    "Chức năng " +
                    tenChucNang +
                    " chưa được FormMain đăng ký xử lý.",
                    "Chưa tích hợp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                suKien(
                    this,
                    new SuKienUnitDetailArgs(
                        donViDangXem));

                lblThongBao.Text =
                    "Đã gửi yêu cầu: " + tenChucNang;
            }
            catch (Exception ex)
            {
                lblThongBao.Text =
                    "Không xử lý được yêu cầu.";

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Lỗi xử lý",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 18. GIẢI PHÓNG TÀI NGUYÊN
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
