

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Views
{
    // Giu nguyen hop dong du lieu voi FormMain, BLL, DTO va PostGIS.
    // Null la chua co du lieu, khong phai gia tri 0.
    public sealed class DuLieuTongQuan
    {
        public long? TongSoDonVi { get; set; }
        public long? SoCoToaDo { get; set; }
        public long? SoThieuToaDo { get; set; }
        /// <summary>Vi tri hien hanh la tru so that (do/ma hoa dia chi), tu v_thong_ke_vi_tri.</summary>
        public int? SoViTriThat { get; set; }
        /// <summary>Vi tri hien hanh chi la diem dai dien dia ban (chua phai tru so).</summary>
        public int? SoDiemDaiDien { get; set; }
        public int? SoLopBanDo { get; set; }
        public bool? DaKetNoiPostGIS { get; set; }
        public string ChiTietKetNoi { get; set; }
        public string HeQuyChieu { get; set; }
        public string NguonDuLieu { get; set; }
        public DateTime? ThoiDiemCapNhat { get; set; }
        public IEnumerable<HoatDongTongQuanItem> HoatDongGanDay { get; set; }

        // Du lieu mo rong tu danh sach BLL de co cung bo cuc voi overview.html.
        public int? SoNhomLoaiDonVi { get; set; }
        public int? SoDiaBanCoHoSo { get; set; }
        public IEnumerable<NhomTongQuanItem> PhanBoTheoLoai { get; set; }
        public IEnumerable<NhomTongQuanItem> PhanBoTheoDiaBan { get; set; }
        public IEnumerable<HoSoNoiBatTongQuanItem> HoSoNoiBat { get; set; }
    }

    public sealed class NhomTongQuanItem
    {
        public string Ten { get; set; }
        public long SoLuong { get; set; }
    }

    public sealed class HoSoNoiBatTongQuanItem
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string LoaiDonVi { get; set; }
        public string DiaBan { get; set; }
    }

    // Danh sach hoat dong do BLL cung cap, da duoc phan quyen.
    public sealed class HoatDongTongQuanItem
    {
        public DateTime? ThoiDiem { get; set; }
        public string LoaiHoatDong { get; set; }
        public string NoiDung { get; set; }

        public string ThoiGianHienThi
        {
            get
            {
                return ThoiDiem.HasValue
                    ? ThoiDiem.Value.ToString("dd/MM HH:mm",
                        CultureInfo.GetCultureInfo("vi-VN"))
                    : "—";
            }
        }
    }

    /// <summary>
    /// Tong quan GIS phong cach Light Dashboard, 100% C# WinForms .NET 4.8.
    /// Khong SQL, khong HTML, khong WebView2, khong truy cap MapWinGIS truc tiep.
    /// Giu nguyen phuong thuc/su kien de FormMain hien tai hoat dong.
    /// </summary>
    public class GiaoDienTongQuan : UserControl
    {
        private static readonly Color Nen = Color.FromArgb(239, 248, 255);
        private static readonly Color Trang = Color.White;
        private static readonly Color Chu = Color.FromArgb(19, 55, 98);
        private static readonly Color ChuPhu = Color.FromArgb(103, 129, 157);
        private static readonly Color Vien = Color.FromArgb(204, 224, 244);
        private static readonly Color XanhNgoc = Color.FromArgb(0, 188, 151);
        private static readonly Color XanhDuong = Color.FromArgb(46, 132, 245);
        private static readonly Color Tim = Color.FromArgb(129, 89, 240);
        private static readonly Color Cam = Color.FromArgb(222, 132, 24);
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

        private readonly Panel pnlCuon;
        private readonly TableLayoutPanel tblTrang;
        private readonly Label lblTongDonVi;
        private readonly Label lblCoToaDo;
        private readonly Label lblThieuToaDo;
        private readonly Label lblSoLop;
        private readonly Label lblTyLe;
        private readonly Label lblThePostGIS;
        private readonly Label lblPostGIS;
        private readonly Label lblCRS;
        private readonly Label lblCapNhat;
        private readonly Label lblNguon;
        private readonly Label lblSoHoatDong;
        private readonly Label lblTrangThai;
        private readonly Label lblBieuDoTyLe;
        private readonly Label lblChuThichCo;
        private readonly Label lblChuThichThieu;
        private readonly Label lblTieuDeChatLuong;
        private readonly DonutChart bieuDo;
        private readonly FlowLayoutPanel flowPhanBo;
        private readonly FlowLayoutPanel flowHoSo;
        private readonly Button btnLoai;
        private readonly Button btnDiaBan;
        private readonly Label lblGhiChuPhanBo;
        private readonly Label lblTinhTrangHoSo;
        private List<NhomTongQuanItem> nhomLoai = new List<NhomTongQuanItem>();
        private List<NhomTongQuanItem> nhomDiaBan = new List<NhomTongQuanItem>();
        private bool dangXemDiaBan;
        private readonly Color[] mauBieuDo = {
            Color.FromArgb(0, 172, 135), Color.FromArgb(43, 137, 239),
            Color.FromArgb(137, 91, 226), Color.FromArgb(245, 157, 57),
            Color.FromArgb(0, 166, 190), Color.FromArgb(227, 103, 150),
            Color.FromArgb(114, 183, 56)
        };
        private readonly DataGridView dgvHoatDong;
        private readonly Button btnTaiLai;
        private readonly Button btnBanDo;
        private readonly Button btnTraCuu;
        private readonly Button btnQuanLy;
        private readonly Button btnPhanTich;
        private readonly Button btnThongKe;
        private readonly Button btnKiemTra;
        private readonly Button btnThemDonVi;
        private readonly ToolTip chuThich;

        private bool choPhepThemDonVi;
        private bool dangTai;
        private bool daNapDuLieu;
        private int soLanNap;

        public event EventHandler<SuKienDieuHuongArgs> YeuCauMoPhanHe;
        public event EventHandler YeuCauThemDonVi;
        public event EventHandler YeuCauTaiLaiTongQuan;
        public event EventHandler YeuCauKiemTraPostGIS;

        public GiaoDienTongQuan()
        {
            Name = "UcTongQuan";
            Dock = DockStyle.Fill;
            BackColor = Nen;
            ForeColor = Chu;
            Font = new Font("Segoe UI", 9F);
            DoubleBuffered = true;

            chuThich = new ToolTip { AutoPopDelay = 7000, InitialDelay = 350 };
            SuspendLayout();

            pnlCuon = new Panel
            {
                Name = "pnlTongQuanCuon",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Nen,
                Padding = new Padding(14, 13, 14, 12)
            };
            tblTrang = new TableLayoutPanel
            {
                Name = "tblTongQuanLight",
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 11,
                Height = 1470,
                BackColor = Nen,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            tblTrang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            int[] chieuCao = { 90, 58, 40, 248, 42, 362, 42, 284, 42, 254, 38 };
            foreach (int h in chieuCao)
                tblTrang.RowStyles.Add(new RowStyle(SizeType.Absolute, h));
            pnlCuon.Controls.Add(tblTrang);
            Controls.Add(pnlCuon);

            pnlCuon.Resize += (s, e) =>
            {
                // Tru scrollbar, giu can cot khi thay doi kich thuoc cua so.
                int rong = pnlCuon.ClientSize.Width -
                    pnlCuon.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth - 2;
                tblTrang.Width = Math.Max(760, rong);
            };

            // 1. Banner mau gradient tu mau HTML Tong quan.
            GradientHeader banner = new GradientHeader
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 11),
                Radius = 17
            };
            Label lblHero = Nhan("Tổng quan đơn vị quân đội trên địa bàn Hà Nội",
                17F, true, Color.White);
            lblHero.Dock = DockStyle.Fill;
            lblHero.Padding = new Padding(22, 1, 10, 0);
            lblHero.BackColor = Color.Transparent;
            banner.Controls.Add(lblHero);
            tblTrang.Controls.Add(banner, 0, 0);

            // 2. Truy cap nhanh nhu HTML (3 nut chinh).
            FlowLayoutPanel nhanh = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false,
                Margin = Padding.Empty,
                Padding = new Padding(0, 1, 0, 9),
                BackColor = Nen
            };
            btnBanDo = Nut("btnBanDo", "◆  Mở bản đồ GIS", XanhNgoc, Color.White);
            btnTraCuu = Nut("btnTraCuu", "⌕  Tra cứu đơn vị", XanhDuong, Color.White);
            btnTaiLai = Nut("btnTaiLai", "↻  Làm mới", Tim, Color.White);
            btnBanDo.Size = new Size(170, 38);
            btnTraCuu.Size = new Size(155, 38);
            btnTaiLai.Size = new Size(133, 38);
            foreach (Button b in new[] { btnBanDo, btnTraCuu, btnTaiLai })
            {
                b.Margin = new Padding(0, 2, 10, 2);
                nhanh.Controls.Add(b);
            }
            tblTrang.Controls.Add(nhanh, 0, 1);

            tblTrang.Controls.Add(TieuDeMuc("THỐNG KÊ TỔNG HỢP",
                "Dữ liệu trong phạm vi được cấp quyền"), 0, 2);

            // 3. 6 the chi so mau pastel, 3 cot x 2 hang.
            TableLayoutPanel chiSo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2,
                Margin = Padding.Empty,
                BackColor = Nen
            };
            for (int i = 0; i < 3; i++)
                chiSo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / 3F));
            chiSo.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            chiSo.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            lblTongDonVi = TheThongKe(chiSo, 0, 0, "TỔNG HỒ SƠ ĐƠN VỊ",
                "Dữ liệu hồ sơ được phép xem", "▦",
                Color.FromArgb(221, 250, 238), Color.FromArgb(0, 159, 120));
            lblCoToaDo = TheThongKe(chiSo, 1, 0, "ĐƠN VỊ CÓ TỌA ĐỘ",
                "Tọa độ WGS84 hợp lệ", "◎",
                Color.FromArgb(222, 238, 255), Color.FromArgb(36, 116, 225));
            lblThieuToaDo = TheThongKe(chiSo, 2, 0, "HỒ SƠ THIẾU TỌA ĐỘ",
                "Cần kiểm tra và bổ sung", "!",
                Color.FromArgb(255, 240, 214), Color.FromArgb(194, 111, 14));
            lblSoLop = TheThongKe(chiSo, 0, 1, "NHÓM LOẠI ĐƠN VỊ",
                "Phân nhóm theo thuộc tính hồ sơ", "▤",
                Color.FromArgb(239, 226, 255), Color.FromArgb(133, 83, 224));
            lblThePostGIS = TheThongKe(chiSo, 1, 1, "ĐỊA BÀN CÓ HỒ SƠ",
                "Số nhóm địa bàn trong dữ liệu", "⌖",
                Color.FromArgb(218, 247, 251), Color.FromArgb(14, 137, 179));
            lblTyLe = TheThongKe(chiSo, 2, 1, "VỊ TRÍ TRỤ SỞ THẬT",
                "Còn lại là điểm đại diện địa bàn", "%",
                Color.FromArgb(234, 251, 213), Color.FromArgb(82, 140, 23));
            tblTrang.Controls.Add(chiSo, 0, 3);

            tblTrang.Controls.Add(TieuDeMuc("PHÂN BỐ VÀ CHẤT LƯỢNG DỮ LIỆU",
                "Thống kê từ hồ sơ được cấp quyền"), 0, 4);

            TableLayoutPanel haiBieuDo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                BackColor = Nen
            };
            haiBieuDo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
            haiBieuDo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            haiBieuDo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Phan bo theo loai don vi/dia ban, tuong tu tabs trong HTML.
            RoundCard cardPhanBo = The(Trang, Color.FromArgb(178, 215, 246));
            cardPhanBo.Dock = DockStyle.Fill;
            cardPhanBo.Margin = new Padding(0, 0, 7, 0);
            cardPhanBo.Padding = new Padding(14, 12, 14, 11);
            TableLayoutPanel khungPhanBo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Trang,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty
            };
            khungPhanBo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            khungPhanBo.RowStyles.Add(new RowStyle(SizeType.Absolute, 49));
            khungPhanBo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            khungPhanBo.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            Panel dauPhanBo = new Panel { Dock = DockStyle.Fill, BackColor = Trang };
            Label tenPhanBo = Nhan("◈  PHÂN BỐ ĐƠN VỊ", 10.5F, true, Chu);
            tenPhanBo.Dock = DockStyle.Fill;
            Panel tab = new Panel
            {
                Width = 178,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(231, 241, 252),
                Padding = new Padding(3)
            };
            btnLoai = Nut("btnLoai", "Loại đơn vị", XanhDuong, Trang);
            btnDiaBan = Nut("btnDiaBan", "Địa bàn", Color.FromArgb(231, 241, 252), Chu);
            btnLoai.Dock = DockStyle.Left; btnLoai.Width = 98;
            btnDiaBan.Dock = DockStyle.Fill;
            tab.Controls.Add(btnDiaBan);
            tab.Controls.Add(btnLoai);
            dauPhanBo.Controls.Add(tenPhanBo);
            dauPhanBo.Controls.Add(tab);
            khungPhanBo.Controls.Add(dauPhanBo, 0, 0);

            flowPhanBo = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Trang,
                Margin = Padding.Empty,
                Padding = new Padding(0, 3, 6, 1)
            };
            khungPhanBo.Controls.Add(flowPhanBo, 0, 1);
            flowPhanBo.SizeChanged += (s, e) => CanDoRongBieuDo();
            lblGhiChuPhanBo = Nhan(
                "Biểu đồ chỉ hiển thị các nhóm nhiều hồ sơ nhất.", 8F, false, ChuPhu);
            lblGhiChuPhanBo.Dock = DockStyle.Fill;
            khungPhanBo.Controls.Add(lblGhiChuPhanBo, 0, 2);

            cardPhanBo.Controls.Add(khungPhanBo);
            haiBieuDo.Controls.Add(cardPhanBo, 0, 0);

            // Danh gia chat luong toa do nhu overview.html.
            RoundCard cardChatLuong = The(Trang, Color.FromArgb(215, 195, 246));
            cardChatLuong.Dock = DockStyle.Fill;
            cardChatLuong.Margin = new Padding(7, 0, 0, 0);
            cardChatLuong.Padding = new Padding(14, 11, 12, 10);
            TableLayoutPanel khungChatLuong = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Trang,
                Margin = Padding.Empty
            };
            khungChatLuong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            khungChatLuong.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            khungChatLuong.RowStyles.Add(new RowStyle(SizeType.Absolute, 143));
            khungChatLuong.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            khungChatLuong.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Label tieuDeChatLuong = Nhan("◉  CHẤT LƯỢNG DỮ LIỆU KHÔNG GIAN",
                9.8F, true, Color.FromArgb(105, 69, 185));
            tieuDeChatLuong.Dock = DockStyle.Fill;
            khungChatLuong.Controls.Add(tieuDeChatLuong, 0, 0);
            Panel hangChart = new Panel { Dock = DockStyle.Fill, BackColor = Trang };
            bieuDo = new DonutChart
            {
                Dock = DockStyle.Left,
                Width = 148,
                BackColor = Trang
            };
            TableLayoutPanel chuBieuDo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Trang,
                Margin = Padding.Empty
            };
            chuBieuDo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            chuBieuDo.RowStyles.Add(new RowStyle(SizeType.Absolute, 57));
            chuBieuDo.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
            chuBieuDo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            lblBieuDoTyLe = Nhan("Chưa có dữ liệu", 12F, true, XanhNgoc);
            lblBieuDoTyLe.Dock = DockStyle.Fill;
            lblChuThichCo = Nhan("Có tọa độ: —", 8.6F, false, Chu);
            lblChuThichCo.Dock = DockStyle.Fill;
            lblChuThichThieu = Nhan("Thiếu tọa độ: —", 8.6F, false, ChuPhu);
            lblChuThichThieu.Dock = DockStyle.Fill;
            chuBieuDo.Controls.Add(lblBieuDoTyLe, 0, 0);
            chuBieuDo.Controls.Add(lblChuThichCo, 0, 1);
            chuBieuDo.Controls.Add(lblChuThichThieu, 0, 2);
            hangChart.Controls.Add(chuBieuDo);
            hangChart.Controls.Add(bieuDo);
            khungChatLuong.Controls.Add(hangChart, 0, 1);
            TableLayoutPanel canhBao = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Trang,
                Margin = Padding.Empty
            };
            canhBao.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            for (int i = 0; i < 3; i++)
                canhBao.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 3F));
            Label lblTieuDeCo = Nhan("Có tọa độ hợp lệ", 8.5F, true, XanhNgoc);
            Label lblTieuDeThieu = Nhan("Thiếu tọa độ hợp lệ", 8.5F, true, Cam);
            lblTieuDeChatLuong = Nhan("Kiểm tra tọa độ WGS84; không thay thế topology.",
                8F, false, ChuPhu);
            foreach (Label lb in new[] { lblTieuDeCo, lblTieuDeThieu,
                lblTieuDeChatLuong }) lb.Dock = DockStyle.Fill;
            canhBao.Controls.Add(lblTieuDeCo, 0, 0);
            canhBao.Controls.Add(lblTieuDeThieu, 0, 1);
            canhBao.Controls.Add(lblTieuDeChatLuong, 0, 2);
            khungChatLuong.Controls.Add(canhBao, 0, 2);
            FlowLayoutPanel hanhDongChatLuong = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Trang,
                WrapContents = false,
                FlowDirection = FlowDirection.LeftToRight
            };
            Button xemBanDo = Nut("btnXemBanDoChatLuong", "◈  Xem trên bản đồ",
                XanhNgoc, Trang);
            xemBanDo.Size = new Size(144, 32);
            xemBanDo.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.BanDoGIS, "Bản đồ GIS");
            Button capNhat = Nut("btnCapNhatChatLuong", "↻  Cập nhật",
                XanhDuong, Trang);
            capNhat.Size = new Size(105, 32);
            capNhat.Click += BtnTaiLai_Click;
            hanhDongChatLuong.Controls.Add(xemBanDo);
            hanhDongChatLuong.Controls.Add(capNhat);
            khungChatLuong.Controls.Add(hanhDongChatLuong, 0, 3);
            cardChatLuong.Controls.Add(khungChatLuong);
            haiBieuDo.Controls.Add(cardChatLuong, 1, 0);
            tblTrang.Controls.Add(haiBieuDo, 0, 5);

            tblTrang.Controls.Add(TieuDeMuc("MỘT SỐ HỒ SƠ ĐƠN VỊ",
                "Hồ sơ thực tế được phép truy cập"), 0, 6);
            RoundCard cardHoSo = The(Trang, Vien);
            cardHoSo.Dock = DockStyle.Fill;
            cardHoSo.Margin = new Padding(0, 0, 0, 7);
            cardHoSo.Padding = new Padding(12);
            flowHoSo = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoScroll = false,
                BackColor = Trang,
                Margin = Padding.Empty
            };
            flowHoSo.SizeChanged += (s, e) => CanDoRongHoSo();
            lblTinhTrangHoSo = Nhan("Chưa có hồ sơ được cung cấp.", 9F, false, ChuPhu);
            lblTinhTrangHoSo.Dock = DockStyle.Bottom;
            lblTinhTrangHoSo.Height = 27;
            cardHoSo.Controls.Add(flowHoSo);
            cardHoSo.Controls.Add(lblTinhTrangHoSo);
            tblTrang.Controls.Add(cardHoSo, 0, 7);

            tblTrang.Controls.Add(TieuDeMuc("HỆ THỐNG VÀ HOẠT ĐỘNG",
                "Chỉ hiển thị thông tin BLL đã cung cấp"), 0, 8);

            // 5. Thong tin he thong + nhat ky gan day.
            TableLayoutPanel noiDungDuoi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                BackColor = Nen
            };
            noiDungDuoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            noiDungDuoi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            noiDungDuoi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            RoundCard heThong = The(Trang, Vien);
            heThong.Dock = DockStyle.Fill;
            heThong.Margin = new Padding(0, 0, 6, 0);
            heThong.Padding = new Padding(12, 9, 10, 10);
            TableLayoutPanel heThongTrong = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = Trang
            };
            heThongTrong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            int[] hangHT = { 35, 41, 31, 31, 31, 31 };
            foreach (int h in hangHT)
                heThongTrong.RowStyles.Add(new RowStyle(SizeType.Absolute, h));
            Label dauHeThong = Nhan("◉  Tình trạng hệ thống", 11F, true, Chu);
            dauHeThong.Dock = DockStyle.Fill;
            lblPostGIS = Nhan("PostGIS: Chưa kiểm tra", 9.4F, true, Cam);
            lblPostGIS.Dock = DockStyle.Fill;
            lblPostGIS.Cursor = Cursors.Hand;
            lblCRS = DongHeThong("CRS: Chưa xác định");
            lblCapNhat = DongHeThong("Cập nhật: Chưa có");
            lblNguon = DongHeThong("Nguồn: Chưa có dữ liệu");
            Label ghiChuHT = DongHeThong("Nhấn trạng thái PostGIS để kiểm tra kết nối.");
            ghiChuHT.ForeColor = ChuPhu;
            heThongTrong.Controls.Add(dauHeThong, 0, 0);
            heThongTrong.Controls.Add(lblPostGIS, 0, 1);
            heThongTrong.Controls.Add(lblCRS, 0, 2);
            heThongTrong.Controls.Add(lblCapNhat, 0, 3);
            heThongTrong.Controls.Add(lblNguon, 0, 4);

            heThongTrong.Controls.Add(ghiChuHT, 0, 5);
            heThong.Controls.Add(heThongTrong);
            noiDungDuoi.Controls.Add(heThong, 0, 0);

            RoundCard hoatDong = The(Trang, Vien);
            hoatDong.Dock = DockStyle.Fill;
            hoatDong.Margin = new Padding(6, 0, 0, 0);
            hoatDong.Padding = new Padding(12, 8, 12, 10);
            Label dauNhatKy = Nhan("☷  Hoạt động gần đây", 11F, true, Chu);
            dauNhatKy.Dock = DockStyle.Top;
            dauNhatKy.Height = 36;
            lblSoHoatDong = Nhan("Chưa có nhật ký được cung cấp.",
                8.4F, false, ChuPhu);
            lblSoHoatDong.Dock = DockStyle.Bottom;
            lblSoHoatDong.Height = 28;
            dgvHoatDong = new DataGridView
            {
                Name = "dgvHoatDong",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                BackgroundColor = Trang,
                GridColor = Color.FromArgb(222, 235, 248),
                EnableHeadersVisualStyles = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            dgvHoatDong.ColumnHeadersHeight = 35;
            dgvHoatDong.RowTemplate.Height = 31;
            dgvHoatDong.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(225, 246, 243);
            dgvHoatDong.ColumnHeadersDefaultCellStyle.ForeColor = Chu;
            dgvHoatDong.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvHoatDong.DefaultCellStyle.ForeColor = Chu;
            dgvHoatDong.DefaultCellStyle.BackColor = Trang;
            dgvHoatDong.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(203, 239, 232);
            dgvHoatDong.DefaultCellStyle.SelectionForeColor = Chu;
            dgvHoatDong.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(247, 251, 255);
            dgvHoatDong.Columns.Add(Cot("Thời gian", "ThoiGianHienThi", 23F));
            dgvHoatDong.Columns.Add(Cot("Hoạt động", "LoaiHoatDong", 28F));
            dgvHoatDong.Columns.Add(Cot("Nội dung", "NoiDung", 49F));
            hoatDong.Controls.Add(dgvHoatDong);
            hoatDong.Controls.Add(lblSoHoatDong);
            hoatDong.Controls.Add(dauNhatKy);
            noiDungDuoi.Controls.Add(hoatDong, 1, 0);
            tblTrang.Controls.Add(noiDungDuoi, 0, 9);

            // Dieu huong nhanh da nam duoi banner, khong lap lai.
            btnQuanLy = Nut("btnQuanLy", "Quản lý", XanhNgoc, Trang);
            btnPhanTich = Nut("btnPhanTich", "Phân tích", Tim, Trang);
            btnThongKe = Nut("btnThongKe", "Thống kê", XanhDuong, Trang);
            btnKiemTra = Nut("btnKiemTra", "Kiểm tra", Cam, Trang);
            btnThemDonVi = Nut("btnThemDonVi", "+ Thêm đơn vị", XanhNgoc, Trang);

            RoundCard chan = The(Color.FromArgb(226, 242, 254), Vien);
            chan.Dock = DockStyle.Fill;
            chan.Margin = new Padding(0, 0, 0, 3);
            lblTrangThai = Nhan("Chưa nạp dữ liệu tổng quan.",
                8.7F, false, ChuPhu);
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.Padding = new Padding(12, 0, 7, 0);
            chan.Controls.Add(lblTrangThai);
            tblTrang.Controls.Add(chan, 0, 10);

            // 7. Giu nguyen tat ca su kien cua UserControl cu.
            btnTaiLai.Click += BtnTaiLai_Click;
            btnBanDo.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.BanDoGIS, "Bản đồ GIS");
            btnTraCuu.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.TraCuuDonVi, "Tra cứu đơn vị");
            btnQuanLy.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.QuanLyDuLieu, "Quản lý dữ liệu");
            btnPhanTich.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.PhanTichKhongGian,
                    "Phân tích không gian");
            btnThongKe.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.ThongKeBaoCao,
                    "Thống kê, báo cáo");
            btnKiemTra.Click += (s, e) =>
                MoPhanHe(PhanHeDieuHuong.KiemTraDuLieu,
                    "Kiểm tra dữ liệu");
            btnThemDonVi.Click += BtnThemDonVi_Click;
            btnLoai.Click += (s, e) => DoiCheDoPhanBo(false);
            btnDiaBan.Click += (s, e) => DoiCheDoPhanBo(true);
            lblPostGIS.Click += LblPostGIS_Click;
            ChoPhepThemDonVi = false;
            XoaDuLieu();
            ResumeLayout(true);
            tblTrang.Width = Math.Max(760,
                pnlCuon.ClientSize.Width - pnlCuon.Padding.Horizontal - 20);
        }

        public bool DaNapDuLieu
        {
            get { return daNapDuLieu; }
        }

        public bool ChoPhepThemDonVi
        {
            get { return choPhepThemDonVi; }
            set
            {
                choPhepThemDonVi = value;
                btnThemDonVi.Enabled = value;
                btnThemDonVi.Cursor = value ? Cursors.Hand : Cursors.Default;
            }
        }

        // Du lieu phai duoc BLL tong hop va phan quyen truoc khi gui vao UI.
        public void NapDuLieu(DuLieuTongQuan duLieu)
        {
            if (duLieu == null)
                throw new ArgumentNullException("duLieu");
            KiemTraSoLuong(duLieu.TongSoDonVi, "TongSoDonVi");
            KiemTraSoLuong(duLieu.SoCoToaDo, "SoCoToaDo");
            KiemTraSoLuong(duLieu.SoThieuToaDo, "SoThieuToaDo");
            if (duLieu.SoLopBanDo.HasValue && duLieu.SoLopBanDo.Value < 0)
                throw new ArgumentException("Số lớp bản đồ không được âm.",
                    "duLieu");
            if (duLieu.TongSoDonVi.HasValue &&
                duLieu.SoCoToaDo.HasValue && duLieu.SoThieuToaDo.HasValue &&
                (decimal)duLieu.SoCoToaDo.Value + duLieu.SoThieuToaDo.Value !=
                duLieu.TongSoDonVi.Value)
                throw new ArgumentException(
                    "Số có tọa độ và thiếu tọa độ phải bằng tổng hồ sơ.",
                    "duLieu");

            List<HoatDongTongQuanItem> hoatDong =
                duLieu.HoatDongGanDay == null
                    ? new List<HoatDongTongQuanItem>()
                    : duLieu.HoatDongGanDay.Where(x => x != null).ToList();
            lblTongDonVi.Text = DinhDangSo(duLieu.TongSoDonVi);
            lblCoToaDo.Text = DinhDangSo(duLieu.SoCoToaDo);
            lblThieuToaDo.Text = DinhDangSo(duLieu.SoThieuToaDo);
            nhomLoai = duLieu.PhanBoTheoLoai == null
                ? new List<NhomTongQuanItem>()
                : duLieu.PhanBoTheoLoai.Where(x => x != null && x.SoLuong >= 0)
                    .OrderByDescending(x => x.SoLuong).ToList();
            nhomDiaBan = duLieu.PhanBoTheoDiaBan == null
                ? new List<NhomTongQuanItem>()
                : duLieu.PhanBoTheoDiaBan.Where(x => x != null && x.SoLuong >= 0)
                    .OrderByDescending(x => x.SoLuong).ToList();
            lblSoLop.Text = duLieu.SoNhomLoaiDonVi.HasValue
                ? duLieu.SoNhomLoaiDonVi.Value.ToString("N0", Vi) :
                (duLieu.PhanBoTheoLoai != null ? nhomLoai.Count.ToString("N0", Vi) : "—");
            lblThePostGIS.Text = duLieu.SoDiaBanCoHoSo.HasValue
                ? duLieu.SoDiaBanCoHoSo.Value.ToString("N0", Vi) :
                (duLieu.PhanBoTheoDiaBan != null ? nhomDiaBan.Count.ToString("N0", Vi) : "—");
            HienThiPhanBo();
            HienThiHoSo(duLieu.HoSoNoiBat);
            CapNhatTrangThaiPostGIS(duLieu.DaKetNoiPostGIS,
                duLieu.ChiTietKetNoi);

            // Donut va ti le chi duoc hien khi co du ca mau so / tu so.
            bool duDieuKien = duLieu.TongSoDonVi.HasValue &&
                duLieu.SoCoToaDo.HasValue &&
                duLieu.SoThieuToaDo.HasValue &&
                duLieu.TongSoDonVi.Value > 0;

            bool coChatLuong = duLieu.SoViTriThat.HasValue && duLieu.SoDiemDaiDien.HasValue &&
                duLieu.SoViTriThat.Value + duLieu.SoDiemDaiDien.Value > 0;
            if (coChatLuong)
            {
                // Goc nhin trac dia: "co toa do" chua du - can biet diem do la TRU SO THAT
                // hay chi la diem dai dien dia ban (trong tam xa/phuong).
                int that = duLieu.SoViTriThat.Value, daiDien = duLieu.SoDiemDaiDien.Value;
                double tyLe = 100d * that / (that + daiDien);
                string chuTyLe = tyLe.ToString("N1", Vi) + "%";
                lblTyLe.Text = chuTyLe;
                lblBieuDoTyLe.Text = chuTyLe + " là vị trí trụ sở thật";
                bieuDo.PhanTram = tyLe;
                lblChuThichCo.Text = "●  Trụ sở thật: " + that.ToString("N0", Vi);
                lblChuThichThieu.Text = "●  Điểm đại diện: " + daiDien.ToString("N0", Vi) +
                    (duLieu.SoThieuToaDo.HasValue && duLieu.SoThieuToaDo.Value > 0 ? " · thiếu tọa độ: " + DinhDangSo(duLieu.SoThieuToaDo) : "");
            }
            else if (duDieuKien)
            {
                double tyLe = 100d * duLieu.SoCoToaDo.Value /
                    duLieu.TongSoDonVi.Value;
                string chuTyLe = tyLe.ToString("N1", Vi) + "%";
                lblTyLe.Text = chuTyLe;
                lblBieuDoTyLe.Text = chuTyLe + " hồ sơ có tọa độ";
                bieuDo.PhanTram = tyLe;
                lblChuThichCo.Text = "●  Có tọa độ: " +
                    DinhDangSo(duLieu.SoCoToaDo);
                lblChuThichThieu.Text = "●  Thiếu tọa độ: " +
                    DinhDangSo(duLieu.SoThieuToaDo);
            }
            else
            {
                lblTyLe.Text = "—";
                lblBieuDoTyLe.Text = "Chưa đủ dữ liệu để tính tỷ lệ";
                lblChuThichCo.Text = "●  Có tọa độ: " +
                    DinhDangSo(duLieu.SoCoToaDo);
                lblChuThichThieu.Text = "●  Thiếu tọa độ: " +
                    DinhDangSo(duLieu.SoThieuToaDo);
                bieuDo.PhanTram = null;
            }

            lblCRS.Text = string.IsNullOrWhiteSpace(duLieu.HeQuyChieu)
                ? "CRS: Chưa xác định" : "CRS: " + duLieu.HeQuyChieu.Trim();
            lblCapNhat.Text = duLieu.ThoiDiemCapNhat.HasValue
                ? "Cập nhật: " + duLieu.ThoiDiemCapNhat.Value
                    .ToString("dd/MM/yyyy HH:mm", Vi)
                : "Cập nhật: Chưa có";
            lblNguon.Text = string.IsNullOrWhiteSpace(duLieu.NguonDuLieu)
                ? "Nguồn: BLL / dữ liệu được cung cấp"
                : "Nguồn: " + duLieu.NguonDuLieu.Trim();
            chuThich.SetToolTip(lblNguon, lblNguon.Text);
            chuThich.SetToolTip(lblCRS, lblCRS.Text);

            dgvHoatDong.DataSource = null;
            dgvHoatDong.DataSource =
                new BindingList<HoatDongTongQuanItem>(hoatDong
                    .OrderByDescending(x => x.ThoiDiem).ToList());
            dgvHoatDong.ClearSelection();
            dgvHoatDong.CurrentCell = null;
            lblSoHoatDong.Text = hoatDong.Count == 0
                ? "Không có nhật ký được cung cấp."
                : "Đã nhận " + hoatDong.Count.ToString("N0", Vi) + " hoạt động.";
            daNapDuLieu = true;
            dangTai = false;
            soLanNap++;
            btnTaiLai.Enabled = true;
            CapNhatThongBao("Đã nhận dữ liệu tổng quan từ tầng nghiệp vụ.");
        }

        public void XoaDuLieu()
        {
            soLanNap++;
            daNapDuLieu = false;
            dangTai = false;
            lblTongDonVi.Text = "—";
            lblCoToaDo.Text = "—";
            lblThieuToaDo.Text = "—";
            lblSoLop.Text = "—";
            lblThePostGIS.Text = "—";
            nhomLoai.Clear();
            nhomDiaBan.Clear();
            HienThiPhanBo();
            HienThiHoSo(null);
            lblTyLe.Text = "—";
            bieuDo.PhanTram = null;
            lblBieuDoTyLe.Text = "Chưa có dữ liệu để tính tỷ lệ";
            lblChuThichCo.Text = "●  Có tọa độ: —";
            lblChuThichThieu.Text = "●  Thiếu tọa độ: —";
            CapNhatTrangThaiPostGIS(null);
            lblCRS.Text = "CRS: Chưa xác định";
            lblCapNhat.Text = "Cập nhật: Chưa có";
            lblNguon.Text = "Nguồn: Chưa có dữ liệu";
            chuThich.SetToolTip(lblNguon, lblNguon.Text);
            chuThich.SetToolTip(lblCRS, lblCRS.Text);
            dgvHoatDong.DataSource = null;
            lblSoHoatDong.Text = "Chưa có nhật ký được cung cấp.";
            btnTaiLai.Enabled = true;
            CapNhatThongBao("Chưa nạp dữ liệu tổng quan.");
        }

        public void DatTrangThaiDangTai(bool dangThucHien)
        {
            dangTai = dangThucHien;
            btnTaiLai.Enabled = !dangTai;
            if (dangTai)
                CapNhatThongBao("Đang chờ dữ liệu tổng quan từ tầng nghiệp vụ.");
        }

        public void CapNhatTrangThaiPostGIS(bool? daKetNoi,
            string chiTiet = null)
        {
            string trangThai = !daKetNoi.HasValue ? "Chưa kiểm tra"
                : daKetNoi.Value ? "Đã kết nối" : "Không kết nối";
            lblPostGIS.Text = "●  PostGIS: " + trangThai;
            Color mau = !daKetNoi.HasValue ? Cam
                : daKetNoi.Value ? Color.FromArgb(0, 153, 111)
                : Color.FromArgb(219, 75, 97);
            lblPostGIS.ForeColor = mau;
            chuThich.SetToolTip(lblPostGIS,
                string.IsNullOrWhiteSpace(chiTiet)
                    ? "Nhấn để yêu cầu kiểm tra kết nối PostGIS." : chiTiet);
        }

        public void CapNhatThongBao(string thongBao)
        {
            lblTrangThai.Text = string.IsNullOrWhiteSpace(thongBao)
                ? "Sẵn sàng."
                : thongBao.Trim().Replace('\r', ' ').Replace('\n', ' ');
        }

        private void MoPhanHe(PhanHeDieuHuong phanHe, string ten)
        {
            EventHandler<SuKienDieuHuongArgs> handler = YeuCauMoPhanHe;
            if (handler == null)
            {
                ThongBaoChuaTichHop("chuyển đến " + ten);
                return;
            }
            try
            {
                handler(this, new SuKienDieuHuongArgs(phanHe, ten));
            }
            catch (Exception ex) { HienLoi("Lỗi điều hướng", ex); }
        }

        private void BtnTaiLai_Click(object sender, EventArgs e)
        {
            if (dangTai) return;
            EventHandler handler = YeuCauTaiLaiTongQuan;
            if (handler == null)
            {
                ThongBaoChuaTichHop("tải lại tổng quan");
                return;
            }
            try
            {
                int lanTruoc = soLanNap;
                handler(this, EventArgs.Empty);
                if (!dangTai && lanTruoc == soLanNap)
                    CapNhatThongBao("Đã gửi yêu cầu tải lại; đang chờ BLL trả kết quả.");
            }
            catch (Exception ex) { HienLoi("Lỗi làm mới", ex); }

        }

        private void BtnThemDonVi_Click(object sender, EventArgs e)
        {
            if (!choPhepThemDonVi) return;
            EventHandler handler = YeuCauThemDonVi;
            if (handler == null)
            {
                ThongBaoChuaTichHop("thêm đơn vị");
                return;
            }
            try { handler(this, EventArgs.Empty); }
            catch (Exception ex) { HienLoi("Lỗi thêm đơn vị", ex); }
        }

        private void LblPostGIS_Click(object sender, EventArgs e)
        {
            EventHandler handler = YeuCauKiemTraPostGIS;
            if (handler == null)
            {
                ThongBaoChuaTichHop("kiểm tra PostGIS");
                return;
            }
            try { handler(this, EventArgs.Empty); }
            catch (Exception ex) { HienLoi("Lỗi kiểm tra kết nối", ex); }
        }

        private void ThongBaoChuaTichHop(string chucNang)
        {
            MessageBox.Show(this,
                "Chức năng " + chucNang + " chưa được FormMain đăng ký xử lý.",
                "Chưa tích hợp", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void HienLoi(string tieuDe, Exception ex)
        {
            CapNhatThongBao("Không xử lý được yêu cầu.");
            MessageBox.Show(this, ex.Message, tieuDe,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DoiCheDoPhanBo(bool xemDiaBan)
        {
            dangXemDiaBan = xemDiaBan;
            Color nenThuong = Color.FromArgb(231, 241, 252);
            btnLoai.BackColor = xemDiaBan ? nenThuong : XanhDuong;
            btnLoai.ForeColor = xemDiaBan ? Chu : Trang;
            btnDiaBan.BackColor = xemDiaBan ? XanhDuong : nenThuong;
            btnDiaBan.ForeColor = xemDiaBan ? Trang : Chu;
            HienThiPhanBo();
        }

        private void HienThiPhanBo()
        {
            if (flowPhanBo == null || flowPhanBo.IsDisposed) return;
            List<NhomTongQuanItem> ds = dangXemDiaBan ? nhomDiaBan : nhomLoai;
            flowPhanBo.SuspendLayout();
            try
            {
                flowPhanBo.Controls.Clear();
                if (ds.Count == 0)
                {
                    Label thongBao = Nhan("Chưa có dữ liệu phân bố từ BLL/PostGIS.",
                        9F, false, ChuPhu);
                    thongBao.AutoSize = false;
                    thongBao.Size = new Size(350, 76);
                    flowPhanBo.Controls.Add(thongBao);
                }
                else
                {
                    long max = Math.Max(1, ds.Max(x => x.SoLuong));
                    long tong = ds.Sum(x => Math.Max(0, x.SoLuong));
                    for (int i = 0; i < Math.Min(7, ds.Count); i++)
                    {
                        NhomTongQuanItem item = ds[i];
                        Panel hang = new Panel
                        {
                            BackColor = Trang,
                            Width = Math.Max(200, flowPhanBo.ClientSize.Width - 16),
                            Height = 46,
                            Margin = new Padding(0, 2, 0, 3)
                        };
                        Label ten = Nhan((i + 1) + ". " +
                            (string.IsNullOrWhiteSpace(item.Ten) ? "Chưa phân loại" : item.Ten),
                            8.7F, true, Chu);
                        ten.Dock = DockStyle.Fill;
                        Label so = Nhan(item.SoLuong.ToString("N0", Vi) +
                            " · " + (tong > 0 ? 100d * item.SoLuong / tong : 0d)
                            .ToString("N1", Vi) + "%", 8.2F, true, mauBieuDo[i % mauBieuDo.Length]);
                        so.Dock = DockStyle.Right;
                        so.Width = 118;
                        so.TextAlign = ContentAlignment.MiddleRight;
                        Panel dong = new Panel
                        {
                            Dock = DockStyle.Top,
                            Height = 25,
                            BackColor = Trang
                        };
                        dong.Controls.Add(ten);
                        dong.Controls.Add(so);
                        ProgressGradient track = new ProgressGradient
                        {
                            Dock = DockStyle.Bottom,
                            Height = 11,
                            Fore = mauBieuDo[i % mauBieuDo.Length],
                            PhanTram = 100d * item.SoLuong / max
                        };
                        hang.Controls.Add(dong);
                        hang.Controls.Add(track);
                        flowPhanBo.Controls.Add(hang);
                    }
                }
            }
            finally { flowPhanBo.ResumeLayout(true); }
            lblGhiChuPhanBo.Text = dangXemDiaBan
                ? "Tối đa 7 địa bàn có nhiều hồ sơ; tỷ lệ trên các nhóm được nhận."
                : "Tối đa 7 loại đơn vị; tỷ lệ trên các nhóm được nhận.";
        }

        private void CanDoRongBieuDo()
        {
            if (flowPhanBo == null || flowPhanBo.IsDisposed) return;
            int w = Math.Max(200, flowPhanBo.ClientSize.Width - 18);
            foreach (Control c in flowPhanBo.Controls)
                if (c.Width != w) c.Width = w;
        }

        private void HienThiHoSo(IEnumerable<HoSoNoiBatTongQuanItem> ds)
        {
            if (flowHoSo == null || flowHoSo.IsDisposed) return;
            List<HoSoNoiBatTongQuanItem> items = ds == null
                ? new List<HoSoNoiBatTongQuanItem>()
                : ds.Where(x => x != null).Take(4).ToList();
            flowHoSo.SuspendLayout();
            try
            {
                flowHoSo.Controls.Clear();
                if (items.Count == 0)
                {
                    lblTinhTrangHoSo.Text =
                        "Chưa có hồ sơ được cung cấp từ BLL/PostGIS.";
                    return;
                }
                for (int i = 0; i < items.Count; i++)
                {
                    HoSoNoiBatTongQuanItem item = items[i];
                    Color[] nen = { Color.FromArgb(230, 255, 246),
                        Color.FromArgb(231, 244, 255), Color.FromArgb(246, 237, 255),
                        Color.FromArgb(255, 245, 225) };
                    RoundCard card = The(nen[i % nen.Length], Vien);
                    card.Width = Math.Max(210, (flowHoSo.ClientSize.Width - 24) / 2);
                    card.Height = 108;
                    card.Margin = new Padding(3, 2, 5, 7);
                    card.Padding = new Padding(11, 7, 8, 7);
                    TableLayoutPanel cot = new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 1,
                        RowCount = 3,
                        BackColor = card.BackColor,
                        Margin = Padding.Empty
                    };
                    cot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                    cot.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
                    cot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                    cot.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
                    Label ma = Nhan(item.MaDonVi ?? "CHƯA CÓ MÃ",
                        8.5F, true, mauBieuDo[i % mauBieuDo.Length]);
                    Label ten = Nhan(item.TenDonVi ?? "Chưa có tên đơn vị",
                        9.7F, true, Chu);
                    Label moTa = Nhan((item.LoaiDonVi ?? "Chưa phân loại") +
                        "  ·  " + (item.DiaBan ?? "Chưa có địa bàn"),
                        8F, false, ChuPhu);
                    foreach (Label l in new[] { ma, ten, moTa })
                    {
                        l.Dock = DockStyle.Fill;
                        l.AutoEllipsis = true;
                    }
                    cot.Controls.Add(ma, 0, 0);
                    cot.Controls.Add(ten, 0, 1);
                    cot.Controls.Add(moTa, 0, 2);

                    card.Controls.Add(cot);
                    flowHoSo.Controls.Add(card);
                }
                lblTinhTrangHoSo.Text =
                    "Đang hiển thị " + items.Count + " hồ sơ được phép xem.";
            }
            finally { flowHoSo.ResumeLayout(true); }
            CanDoRongHoSo();
        }

        private void CanDoRongHoSo()
        {
            if (flowHoSo == null || flowHoSo.IsDisposed) return;
            int w = Math.Max(210, (flowHoSo.ClientSize.Width - 25) / 2);
            foreach (Control c in flowHoSo.Controls)
                if (c.Width != w) c.Width = w;
        }

        private sealed class ProgressGradient : Control
        {
            private double phanTram;
            public Color Fore { get; set; }
            public double PhanTram
            {
                get { return phanTram; }
                set { phanTram = Math.Max(0, Math.Min(100, value)); Invalidate(); }
            }
            public ProgressGradient()
            {
                DoubleBuffered = true;
                Fore = XanhNgoc;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (Width <= 2 || Height <= 2) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath bg = BoTron(new Rectangle(0, 0,
                    Width - 1, Height - 1), 5))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(223, 235, 248)))
                    e.Graphics.FillPath(b, bg);
                int filled = (int)Math.Round((Width - 1) * PhanTram / 100d);
                if (filled < 2) return;
                using (GraphicsPath path = BoTron(new Rectangle(0, 0,
                    filled, Height - 1), 5))
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    new Rectangle(0, 0, Math.Max(filled, 2), Height),
                    Fore, ControlPaint.Light(Fore, 0.45f),
                    LinearGradientMode.Horizontal))
                    e.Graphics.FillPath(brush, path);
            }
        }

        private static void KiemTraSoLuong(long? soLuong, string ten)
        {
            if (soLuong.HasValue && soLuong.Value < 0)
                throw new ArgumentException("Số lượng không được âm: " + ten, ten);
        }

        private static string DinhDangSo(long? soLuong)
        {
            return soLuong.HasValue ? soLuong.Value.ToString("N0", Vi) : "—";
        }

        private static Label Nhan(string text, float co, bool dam, Color mau)
        {
            return new Label
            {
                Text = text,
                ForeColor = mau,
                AutoEllipsis = true,
                Font = new Font("Segoe UI", co,
                    dam ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label DongHeThong(string text)
        {
            Label l = Nhan(text, 8.8F, false, Chu);
            l.Dock = DockStyle.Fill;
            l.AutoEllipsis = true;
            return l;
        }

        private static RoundCard The(Color nen, Color mauVien)
        {
            return new RoundCard
            {
                Radius = 15,
                BackColor = nen,
                LineColor = mauVien,
                Accent = Color.Transparent
            };
        }

        private static Control TieuDeMuc(string title, string ghiChu)
        {
            Panel hang = new Panel { Dock = DockStyle.Fill, BackColor = Nen };
            Label ten = Nhan("▏ " + title, 10F, true, Chu);
            ten.Dock = DockStyle.Left;
            ten.Width = 330;
            Label phu = Nhan(ghiChu, 8F, false, ChuPhu);
            phu.Dock = DockStyle.Fill;
            phu.TextAlign = ContentAlignment.MiddleRight;
            hang.Controls.Add(phu);
            hang.Controls.Add(ten);
            return hang;
        }

        private static Label TheThongKe(TableLayoutPanel cha, int cot, int hang,
            string title, string note, string icon, Color mauNen, Color accent)
        {
            RoundCard the = The(mauNen, Color.FromArgb(170, accent));
            the.Accent = accent;
            the.Dock = DockStyle.Fill;
            the.Margin = new Padding(cot == 0 ? 0 : 5, 0,
                cot == 2 ? 0 : 5, 11);
            the.Padding = new Padding(14, 9, 12, 9);
            Label iconNho = Nhan(icon, 17F, true, accent);
            iconNho.Dock = DockStyle.Right;
            iconNho.Width = 33;
            iconNho.TextAlign = ContentAlignment.TopCenter;
            Label ten = Nhan(title, 8F, true, Chu);
            ten.Dock = DockStyle.Top;
            ten.Height = 24;
            Label giaTri = Nhan("—", 21F, true, accent);
            giaTri.Dock = DockStyle.Top;
            giaTri.Height = 49;
            Label moTa = Nhan(note, 8.1F, false, ChuPhu);
            moTa.Dock = DockStyle.Fill;
            the.Controls.Add(moTa);
            the.Controls.Add(giaTri);
            the.Controls.Add(ten);
            the.Controls.Add(iconNho);
            cha.Controls.Add(the, cot, hang);
            return giaTri;
        }

        private static Button Nut(string name, string text, Color nen, Color mauChu)
        {
            Button b = new Button
            {
                Name = name,
                Text = text,
                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,
                BackColor = nen,
                ForeColor = mauChu,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8.7F, FontStyle.Bold)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(nen, 0.10F);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(nen, 0.08F);
            b.Resize += (s, e) => BoGoc(b, 12);
            return b;
        }

        private static DataGridViewTextBoxColumn Cot(string title,
            string property, float weight)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = "col" + property,
                HeaderText = title,
                DataPropertyName = property,
                FillWeight = weight,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private static void BoGoc(Control c, int radius)
        {
            if (c == null || c.IsDisposed || c.Width < 2 || c.Height < 2)
                return;
            using (GraphicsPath gp = BoTron(new Rectangle(0, 0,
                c.Width, c.Height), radius))
            {
                Region moi = new Region(gp);
                Region cu = c.Region;
                c.Region = moi;
                if (cu != null) cu.Dispose();
            }
        }

        private static GraphicsPath BoTron(Rectangle r, int radius)
        {
            int d = Math.Max(2, Math.Min(radius * 2,
                Math.Min(r.Width, r.Height)));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private sealed class RoundCard : Panel
        {
            public int Radius { get; set; }
            public Color LineColor { get; set; }
            public Color Accent { get; set; }

            public RoundCard()
            {
                DoubleBuffered = true;
                Radius = 15;
                LineColor = Vien;
                Accent = Color.Transparent;
                Resize += (s, e) => BoGoc(this, Radius);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                if (Width <= 2 || Height <= 2) return;
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath gp = BoTron(
                    new Rectangle(0, 0, Width - 1, Height - 1), Radius))
                using (Pen pen = new Pen(LineColor, 1F))
                    e.Graphics.DrawPath(pen, gp);
                if (Accent.A > 0)
                {
                    using (SolidBrush b = new SolidBrush(Accent))
                        e.Graphics.FillRectangle(b, 0, 17, 4,
                            Math.Max(0, Height - 34));
                    using (SolidBrush b = new SolidBrush(
                        Color.FromArgb(22, Accent)))
                        e.Graphics.FillEllipse(b, Width - 100, Height - 85,
                            123, 123);
                }
            }
        }

        private sealed class GradientHeader : Panel
        {
            public int Radius { get; set; }
            public GradientHeader()
            {
                DoubleBuffered = true;
                Radius = 17;
                Resize += (s, e) => BoGoc(this, Radius);
            }
            protected override void OnPaintBackground(PaintEventArgs e)
            {
                if (Width < 2 || Height < 2) return;
                Rectangle r = new Rectangle(0, 0, Width, Height);
                using (LinearGradientBrush br = new LinearGradientBrush(r,
                    XanhNgoc, Tim, LinearGradientMode.Horizontal))
                {
                    br.InterpolationColors = new ColorBlend
                    {
                        Colors = new[] { XanhNgoc,
                            Color.FromArgb(20, 162, 217), Tim },
                        Positions = new[] { 0F, 0.53F, 1F }
                    };
                    e.Graphics.FillRectangle(br, r);
                }
                using (Pen luoi = new Pen(Color.FromArgb(52, Color.White), 1F))
                {
                    int batDau = Math.Max(0, Width - 340);
                    for (int x = batDau; x < Width; x += 28)
                        e.Graphics.DrawLine(luoi, x, 0, x, Height);
                    for (int y = 0; y < Height; y += 27)
                        e.Graphics.DrawLine(luoi, batDau, y, Width, y);
                }
                using (SolidBrush nhan = new SolidBrush(
                    Color.FromArgb(230, 252, 90)))
                    e.Graphics.FillRectangle(nhan, 0, 15, 5,
                        Math.Max(0, Height - 30));
            }
        }

        private sealed class DonutChart : Control
        {
            private double? phanTram;
            public double? PhanTram
            {
                get { return phanTram; }
                set
                {
                    phanTram = value.HasValue
                        ? Math.Max(0d, Math.Min(100d, value.Value))
                        : (double?)null;
                    Invalidate();
                }
            }
            public DonutChart()
            {
                DoubleBuffered = true;
                Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                int co = Math.Max(40, Math.Min(Width, Height) - 35);
                Rectangle r = new Rectangle((Width - co) / 2,
                    (Height - co) / 2, co, co);
                using (Pen nen = new Pen(Color.FromArgb(233, 241, 250), 19F))
                    e.Graphics.DrawArc(nen, r, -90, 360);
                if (phanTram.HasValue)
                {
                    using (Pen p = new Pen(XanhNgoc, 19F))
                    {
                        p.StartCap = LineCap.Round;
                        p.EndCap = LineCap.Round;
                        float goc = (float)(phanTram.Value * 3.6d);
                        if (goc > 0.01F)
                            e.Graphics.DrawArc(p, r, -90, Math.Min(359.99F, goc));
                    }
                }
                string s = phanTram.HasValue
                    ? phanTram.Value.ToString("N1", Vi) + "%" : "—";
                TextRenderer.DrawText(e.Graphics, s, Font, r, Chu,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && chuThich != null)
                chuThich.Dispose();
            base.Dispose(disposing);
        }
    }
}
