
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>
    /// Giao dien tra cuu GIS V2.1.
    /// C# 7.3 / .NET Framework 4.8.
    ///
    /// - Danh sach ket qua ben trai.
    /// - So do khong gian lon ben phai.
    /// - Khong hien thi khung "Ho so dang chon".
    /// - Co zoom, pan, vua khung, tooltip, chon diem.
    /// - Giu cac su kien dinh vi FormMain hien tai.
    /// - Khong thay doi du lieu PostGIS.
    /// </summary>
    public sealed class GiaoDienTraCuuDonVi : UserControl
    {
        // =====================================================
        // 1. BANG MAU
        // =====================================================

        private static readonly Color Nen =
            Color.FromArgb(244, 248, 253);

        private static readonly Color Chu =
            Color.FromArgb(27, 51, 77);

        private static readonly Color Phu =
            Color.FromArgb(105, 124, 146);

        private static readonly Color Vien =
            Color.FromArgb(218, 229, 240);

        private static readonly Color Xanh =
            Color.FromArgb(37, 99, 235);

        private static readonly Color Ngoc =
            Color.FromArgb(0, 173, 148);

        private static readonly Color Tim =
            Color.FromArgb(124, 77, 226);

        private static readonly Color Do =
            Color.FromArgb(235, 71, 104);

        private static readonly Color Vang =
            Color.FromArgb(242, 178, 50);

        private static readonly CultureInfo Vi =
            CultureInfo.GetCultureInfo("vi-VN");

        private const int KichThuocTrang = 4;

        // =====================================================
        // 2. DU LIEU
        // =====================================================

        private readonly List<BanGhi> duLieu =
            new List<BanGhi>();

        private List<BanGhi> ketQua =
            new List<BanGhi>();

        private BanGhi dangChon;

        private int trangHienTai;
        private int nhomHienTai;

        private bool dangCapNhatBoLoc;
        private bool daNapDuLieu;

        // =====================================================
        // 3. CONTROLS
        // =====================================================

        private readonly TextBox txtTim;

        private readonly ComboBox cboLoai;
        private readonly ComboBox cboDiaBan;

        private readonly CheckBox chkCoDiem;

        private readonly Label lblTrangThai;
        private readonly Label lblKetQua;
        private readonly Label lblPhanTrang;
        private readonly Label lblThongTinBanDo;

        private readonly FlowLayoutPanel dsThe;

        private readonly Button btnTruoc;
        private readonly Button btnSau;

        private readonly Button btnXemGIS;
        private readonly Button btnChiTiet;

        private readonly Button[] nutNhom =
            new Button[5];

        private readonly SoDoKhongGian soDo;

        private readonly System.Windows.Forms.Timer henTim;

        private readonly ToolTip meo =
            new ToolTip();

        // =====================================================
        // 4. SU KIEN KET NOI FORMMAIN
        // =====================================================

        public event Action<string>
            YeuCauDinhViDonVi;

        public event Action<string>
            YeuCauXemChiTietDonVi;

        public event Action
            YeuCauLamMoi;

        public event Action<string, double, double, string>
            YeuCauDinhViDiemDaiDien;

        public event Action<long, double, double, string>
            YeuCauDinhViHoSoHqtc;

        public bool DaNapDuLieu
        {
            get { return daNapDuLieu; }
        }

        // =====================================================
        // 5. KHOI TAO
        // =====================================================

        public GiaoDienTraCuuDonVi()
        {
            Name = "GiaoDienTraCuuDonVi";

            Dock = DockStyle.Fill;

            BackColor = Nen;
            ForeColor = Chu;

            Font = new Font("Segoe UI", 9F);

            DoubleBuffered = true;
            AutoScroll = false;

            SuspendLayout();

            henTim = new System.Windows.Forms.Timer
            {
                Interval = 260
            };

            henTim.Tick += (s, e) =>
            {
                henTim.Stop();
                ApDungBoLoc();
            };

            // =================================================
            // 5.1. BO CUC CHINH
            // =================================================

            TableLayoutPanel trang =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 1,
                    RowCount = 4,

                    Margin = Padding.Empty,

                    Padding = new Padding(
                        15, 9, 15, 10),

                    BackColor = Nen
                };

            trang.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            trang.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 98F));

            trang.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 140F));

            trang.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 45F));

            trang.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            Controls.Add(trang);

            // =================================================
            // 5.2. HEADER
            // =================================================

            Panel header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Nen,
                Margin = Padding.Empty
            };

            Panel headerTrai = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Nen
            };

            Label breadcrumb = Nhan(
                "HỆ QUẢN TRỊ CSDL KHÔNG GIAN  /  TRA CỨU",
                8F,
                true,
                Phu);

            breadcrumb.Dock = DockStyle.Top;
            breadcrumb.Height = 21;

            Label title = Nhan(
                "Tra cứu đơn vị quân đội",
                20F,
                true,
                Chu);

            title.Dock = DockStyle.Top;
            title.Height = 41;

            Label subtitle = Nhan(
                "Tìm kiếm thông tin và khám phá vị trí đơn vị trên bản đồ GIS.",
                9.4F,
                false,
                Phu);

            subtitle.Dock = DockStyle.Top;
            subtitle.Height = 26;

            headerTrai.Controls.Add(subtitle);
            headerTrai.Controls.Add(title);
            headerTrai.Controls.Add(breadcrumb);

            RoundedPanel trangThai = The(
                Color.FromArgb(226, 249, 240),
                Color.FromArgb(183, 233, 217));

            trangThai.Dock = DockStyle.Right;
            trangThai.Width = 210;

            trangThai.Margin =
                new Padding(0, 0, 0, 10);

            trangThai.Padding =
                new Padding(9, 3, 8, 3);

            lblTrangThai = Nhan(
                "● Chưa nạp dữ liệu",
                8.3F,
                true,
                Color.FromArgb(0, 125, 104));

            lblTrangThai.Dock = DockStyle.Fill;

            trangThai.Controls.Add(lblTrangThai);

            header.Controls.Add(headerTrai);
            header.Controls.Add(trangThai);

            trang.Controls.Add(header, 0, 0);

            // =================================================
            // 6. KHUNG TIM KIEM
            // =================================================

            RoundedPanel khungTim = The(
                Color.White,
                Vien);

            khungTim.Margin =
                new Padding(0, 0, 0, 7);

            khungTim.Padding =
                new Padding(15, 9, 15, 8);

            TableLayoutPanel khungTimTrong =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 1,
                    RowCount = 3,

                    BackColor = Color.White,
                    Margin = Padding.Empty
                };

            khungTimTrong.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            khungTimTrong.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 20F));

            khungTimTrong.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 53F));

            khungTimTrong.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            Label nhanTim = Nhan(
                "TÌM KIẾM NHANH",
                8.5F,
                true,
                Phu);

            nhanTim.Dock = DockStyle.Fill;

            khungTimTrong.Controls.Add(
                nhanTim, 0, 0);

            // =================================================
            // 6.1. O TIM KIEM LON
            // =================================================

            TableLayoutPanel dongTim =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 2,
                    RowCount = 1,

                    Margin = Padding.Empty,
                    BackColor = Color.White
                };

            dongTim.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 80F));

            dongTim.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 20F));

            dongTim.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            RoundedPanel oTim = The(
                Color.FromArgb(249, 252, 255),
                Color.FromArgb(170, 198, 228));

            oTim.Margin =
                new Padding(0, 0, 9, 4);

            oTim.Padding =
                new Padding(13, 9, 12, 8);

            Label iconTim = Nhan(
                "⌕",
                19F,
                true,
                Xanh);

            iconTim.Dock = DockStyle.Left;
            iconTim.Width = 34;

            txtTim = new TextBox
            {
                Name = "txtTuKhoa",

                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,

                Font = new Font(
                    "Segoe UI", 11F),

                BackColor =
                    Color.FromArgb(249, 252, 255),

                ForeColor = Chu,
                MaxLength = 150
            };

            txtTim.HandleCreated += (s, e) =>
            {
                DatGoiY(
                    txtTim,
                    "Nhập tên đơn vị, mã hồ sơ hoặc địa bàn...");
            };

            oTim.Controls.Add(txtTim);
            oTim.Controls.Add(iconTim);

            Button btnTim = Nut(
                "⌕  Tìm kiếm",
                Xanh,
                Color.White,
                155,
                43);

            btnTim.Dock = DockStyle.Fill;

            btnTim.Margin =
                new Padding(0, 0, 0, 4);

            dongTim.Controls.Add(oTim, 0, 0);
            dongTim.Controls.Add(btnTim, 1, 0);

            khungTimTrong.Controls.Add(
                dongTim, 0, 1);

            // =================================================
            // 6.2. BO LOC
            // =================================================

            FlowLayoutPanel dongLoc =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    FlowDirection =
                        FlowDirection.LeftToRight,

                    WrapContents = false,
                    AutoScroll = true,

                    Margin = Padding.Empty,
                    Padding = new Padding(0, 2, 0, 0),

                    BackColor = Color.White
                };

            Label lblLoc = Nhan(
                "Bộ lọc:",
                9F,
                true,
                Chu);

            lblLoc.Width = 67;
            lblLoc.Height = 32;

            cboLoai = Combo();
            cboDiaBan = Combo();

            cboLoai.Width = 186;
            cboDiaBan.Width = 193;

            cboLoai.Margin =
                new Padding(0, 4, 10, 0);

            cboDiaBan.Margin =
                new Padding(0, 4, 10, 0);

            chkCoDiem = new CheckBox
            {
                Text = "Có điểm bản đồ",

                AutoSize = false,

                Width = 147,
                Height = 32,

                Margin =
                    new Padding(2, 3, 8, 0),

                Font = new Font(
                    "Segoe UI", 8.8F),

                ForeColor = Chu
            };

            Button btnXoa = Nut(
                "↶ Xóa bộ lọc",
                Color.FromArgb(255, 241, 245),
                Do,
                116,
                30);

            btnXoa.Margin =
                new Padding(0, 4, 8, 0);

            Button btnTaiLai = Nut(
                "↻ Tải lại",
                Color.FromArgb(242, 236, 255),
                Tim,
                89,
                30);

            btnTaiLai.Margin =
                new Padding(0, 4, 0, 0);

            dongLoc.Controls.Add(lblLoc);
            dongLoc.Controls.Add(cboLoai);
            dongLoc.Controls.Add(cboDiaBan);
            dongLoc.Controls.Add(chkCoDiem);
            dongLoc.Controls.Add(btnXoa);
            dongLoc.Controls.Add(btnTaiLai);

            khungTimTrong.Controls.Add(
                dongLoc, 0, 2);

            khungTim.Controls.Add(khungTimTrong);

            trang.Controls.Add(khungTim, 0, 1);

            // =================================================
            // 7. LOC NHANH THEO NHOM
            // =================================================

            FlowLayoutPanel thanhNhom =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    FlowDirection =
                        FlowDirection.LeftToRight,

                    WrapContents = false,
                    AutoScroll = true,

                    Margin = Padding.Empty,

                    Padding =
                        new Padding(0, 4, 0, 4),

                    BackColor = Nen
                };

            Label lblNhom = Nhan(
                "TÌM THEO NHÓM",
                8.2F,
                true,
                Phu);

            lblNhom.Width = 121;
            lblNhom.Height = 31;

            thanhNhom.Controls.Add(lblNhom);

            string[] tenNhom =
            {
                "Tất cả",
                "Ban CHQS",
                "Đơn vị các cấp",
                "Học viện",
                "Bộ tư lệnh"
            };

            int[] rongNhom =
            {
                94,
                131,
                158,
                106,
                116
            };

            for (int i = 0; i < nutNhom.Length; i++)
            {
                int index = i;

                Button nut = Nut(
                    tenNhom[i],
                    Color.White,
                    Chu,
                    rongNhom[i],
                    30);

                nut.Margin =
                    new Padding(0, 1, 8, 0);

                nut.Click += (s, e) =>
                {
                    nhomHienTai = index;

                    CapNhatMauNhom();
                    ApDungBoLoc();
                };

                nutNhom[i] = nut;

                thanhNhom.Controls.Add(nut);
            }

            trang.Controls.Add(thanhNhom, 0, 2);

            // =================================================
            // 8. HAI COT CHINH
            // =================================================

            TableLayoutPanel cotChinh =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 2,
                    RowCount = 1,

                    BackColor = Nen,
                    Margin = Padding.Empty
                };

            cotChinh.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 44F));

            cotChinh.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 56F));

            cotChinh.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            trang.Controls.Add(cotChinh, 0, 3);

            // =================================================
            // 9. DANH SACH KET QUA BEN TRAI
            // =================================================

            RoundedPanel khungKetQua = The(
                Color.White,
                Vien);

            khungKetQua.Margin =
                new Padding(0, 0, 7, 0);

            khungKetQua.Padding =
                new Padding(12, 10, 12, 9);

            Panel dauDanhSach = new Panel
            {
                Dock = DockStyle.Top,
                Height = 43,
                BackColor = Color.White
            };

            Label titleKetQua = Nhan(
                "Kết quả tra cứu",
                13F,
                true,
                Chu);

            titleKetQua.Dock = DockStyle.Left;
            titleKetQua.Width = 205;

            lblKetQua = Nhan(
                "0 hồ sơ",
                9F,
                true,
                Ngoc);

            lblKetQua.Dock = DockStyle.Right;
            lblKetQua.Width = 104;

            lblKetQua.TextAlign =
                ContentAlignment.MiddleRight;

            dauDanhSach.Controls.Add(titleKetQua);
            dauDanhSach.Controls.Add(lblKetQua);

            dsThe = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,

                WrapContents = false,
                AutoScroll = true,

                FlowDirection =
                    FlowDirection.TopDown,

                BackColor = Color.White,

                Margin = Padding.Empty,
                Padding = new Padding(0, 3, 1, 0)
            };

            dsThe.Resize += (s, e) =>
                CanThe();

            // =================================================
            // 9.1. PHAN TRANG
            // =================================================

            Panel chanDanhSach = new Panel
            {
                Dock = DockStyle.Bottom,

                Height = 39,

                BackColor = Color.White,
                Padding = new Padding(0, 5, 0, 0)
            };

            btnTruoc = Nut(
                "‹",
                Color.FromArgb(234, 241, 250),
                Chu,
                40,
                29);

            btnSau = Nut(
                "›",
                Xanh,
                Color.White,
                40,
                29);

            btnTruoc.Dock = DockStyle.Right;
            btnSau.Dock = DockStyle.Right;

            btnTruoc.Margin = Padding.Empty;
            btnSau.Margin = Padding.Empty;

            lblPhanTrang = Nhan(
                "Chưa có kết quả",
                8.3F,
                false,
                Phu);

            lblPhanTrang.Dock = DockStyle.Fill;

            chanDanhSach.Controls.Add(lblPhanTrang);
            chanDanhSach.Controls.Add(btnTruoc);
            chanDanhSach.Controls.Add(btnSau);

            khungKetQua.Controls.Add(dsThe);
            khungKetQua.Controls.Add(chanDanhSach);
            khungKetQua.Controls.Add(dauDanhSach);

            cotChinh.Controls.Add(khungKetQua, 0, 0);

            // =================================================
            // 10. SO DO KHONG GIAN BEN PHAI
            // =================================================

            RoundedPanel khungSoDo = The(
                Color.White,
                Vien);

            khungSoDo.Margin =
                new Padding(7, 0, 0, 0);

            khungSoDo.Padding =
                new Padding(12, 10, 12, 10);

            TableLayoutPanel noiDungSoDo =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 1,
                    RowCount = 4,

                    Margin = Padding.Empty,
                    BackColor = Color.White
                };

            noiDungSoDo.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            noiDungSoDo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 48));

            noiDungSoDo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 34));

            noiDungSoDo.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            noiDungSoDo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 55));

            // =================================================
            // 10.1. TIEU DE SO DO
            // =================================================

            Panel dauSoDo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Label titleSoDo = Nhan(
                "Sơ đồ phân bố đơn vị",
                13F,
                true,
                Chu);

            titleSoDo.Dock = DockStyle.Top;
            titleSoDo.Height = 27;

            Label phuDe = Nhan(
                "Phân bố tọa độ PostGIS  ·  EPSG:32648",
                8.4F,
                false,
                Phu);

            phuDe.Dock = DockStyle.Fill;

            dauSoDo.Controls.Add(phuDe);
            dauSoDo.Controls.Add(titleSoDo);

            noiDungSoDo.Controls.Add(dauSoDo, 0, 0);

            // =================================================
            // 10.2. THANH CONG CU SO DO
            // =================================================

            Panel thanhBanDo = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor =
                    Color.FromArgb(246, 250, 254)
            };

            Label lblHuongDan = Nhan(
                "Kéo chuột để di chuyển · Lăn chuột để thu/phóng",
                8.1F,
                false,
                Phu);

            lblHuongDan.Dock = DockStyle.Fill;

            Button btnVua = Nut(
                "Vừa khung",
                Color.White,
                Chu,
                88,
                27);

            Button btnCong = Nut(
                "+",
                Color.White,
                Xanh,
                32,
                27);

            Button btnTru = Nut(
                "−",
                Color.White,
                Xanh,
                32,
                27);

            btnVua.Dock = DockStyle.Right;
            btnCong.Dock = DockStyle.Right;
            btnTru.Dock = DockStyle.Right;

            btnVua.Margin = Padding.Empty;
            btnCong.Margin = Padding.Empty;
            btnTru.Margin = Padding.Empty;

            thanhBanDo.Controls.Add(lblHuongDan);
            thanhBanDo.Controls.Add(btnVua);
            thanhBanDo.Controls.Add(btnCong);
            thanhBanDo.Controls.Add(btnTru);

            noiDungSoDo.Controls.Add(thanhBanDo, 0, 1);

            // =================================================
            // 10.3. SO DO CHIEM PHAN LON DIEN TICH
            // =================================================

            RoundedPanel khungNenBanDo = The(
                Color.FromArgb(235, 245, 252),
                Color.FromArgb(215, 230, 243));

            khungNenBanDo.Margin =
                new Padding(0, 6, 0, 6);

            khungNenBanDo.Padding =
                new Padding(5);

            soDo = new SoDoKhongGian
            {
                Dock = DockStyle.Fill
            };

            soDo.YeuCauChon += (d) =>
                ChonBanGhi(d, true, false);

            soDo.YeuCauMoBanDo += (d) =>
            {
                ChonBanGhi(d, true, false);
                DinhViBanGhi(d);
            };

            khungNenBanDo.Controls.Add(soDo);

            noiDungSoDo.Controls.Add(
                khungNenBanDo, 0, 2);

            // =================================================
            // 10.4. NUT THAO TAC NAM DUOI BAN DO
            // =================================================

            TableLayoutPanel thanhThaoTac =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 2,
                    RowCount = 1,

                    Margin = Padding.Empty,
                    BackColor = Color.White
                };

            thanhThaoTac.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 52F));

            thanhThaoTac.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 48F));

            thanhThaoTac.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            btnXemGIS = Nut(
                "⌖  Xem trên bản đồ GIS",
                Xanh,
                Color.White,
                185,
                38);

            btnChiTiet = Nut(
                "▤  Xem chi tiết hồ sơ",
                Tim,
                Color.White,
                170,
                38);

            btnXemGIS.Dock = DockStyle.Fill;
            btnChiTiet.Dock = DockStyle.Fill;

            btnXemGIS.Margin =
                new Padding(0, 8, 6, 5);

            btnChiTiet.Margin =
                new Padding(6, 8, 0, 5);

            thanhThaoTac.Controls.Add(
                btnXemGIS, 0, 0);

            thanhThaoTac.Controls.Add(
                btnChiTiet, 1, 0);

            noiDungSoDo.Controls.Add(
                thanhThaoTac, 0, 3);

            khungSoDo.Controls.Add(noiDungSoDo);

            cotChinh.Controls.Add(khungSoDo, 1, 0);

            // =================================================
            // 11. SU KIEN
            // =================================================

            txtTim.TextChanged += (s, e) =>
                HenLoc();

            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    henTim.Stop();
                    ApDungBoLoc();
                }
            };

            cboLoai.SelectedIndexChanged +=
                (s, e) => HenLoc();

            cboDiaBan.SelectedIndexChanged +=
                (s, e) => HenLoc();

            chkCoDiem.CheckedChanged +=
                (s, e) => HenLoc();

            btnTim.Click += (s, e) =>
            {
                henTim.Stop();
                ApDungBoLoc();
            };

            btnXoa.Click += (s, e) =>
                XoaBoLoc();

            btnTaiLai.Click += (s, e) =>
            {
                Action h = YeuCauLamMoi;

                if (h != null)
                    h();
            };

            btnTruoc.Click += (s, e) =>
                DoiTrang(-1);

            btnSau.Click += (s, e) =>
                DoiTrang(1);

            btnXemGIS.Click += (s, e) =>
                DinhViBanGhi(dangChon);

            btnChiTiet.Click += (s, e) =>
                MoChiTiet();

            btnVua.Click += (s, e) =>
                soDo.VuaKhung();

            btnCong.Click += (s, e) =>
                soDo.ThuPhong(1.35);

            btnTru.Click += (s, e) =>
                soDo.ThuPhong(1d / 1.35d);

            cboLoai.Items.Add(
                "Tất cả loại đơn vị");

            cboDiaBan.Items.Add(
                "Tất cả phường/xã");

            cboLoai.SelectedIndex = 0;
            cboDiaBan.SelectedIndex = 0;

            lblThongTinBanDo = lblHuongDan;

            CapNhatMauNhom();
            CapNhatTrangThaiChon();

            ResumeLayout(true);
        }

        // =====================================================
        // 12. TEXTBOX PLACEHOLDER
        // =====================================================

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            string text);

        private static void DatGoiY(
            TextBox textbox,
            string goiY)
        {
            if (!textbox.IsHandleCreated)
                return;

            SendMessage(
                textbox.Handle,
                0x1501,
                new IntPtr(1),
                goiY);
        }

        // =====================================================
        // 13. NAP DU LIEU - GIU CAC OVERLOAD
        // =====================================================

        public void NapDuLieu(
            IEnumerable<DonViQuanDoi> hoSo)
        {
            NapNoiBo(
                hoSo,
                null,
                null,
                true);
        }

        public void NapDuLieu(
            IEnumerable<DonViQuanDoi> hoSo,
            IEnumerable<DanhMucBanChqsCapXa2026> danhMuc)
        {
            NapNoiBo(
                hoSo,
                danhMuc,
                null,
                false);
        }

        public void NapDuLieu(
            IEnumerable<DonViQuanDoi> hoSo,
            IEnumerable<DanhMucBanChqsCapXa2026> danhMuc,
            IEnumerable<HoSoGisHqtc> hoSoHqtc)
        {
            NapNoiBo(
                hoSo,
                danhMuc,
                hoSoHqtc,
                false);
        }

        // =====================================================
        // 14. NAP DU LIEU NOI BO
        // =====================================================

        private void NapNoiBo(
            IEnumerable<DonViQuanDoi> hoSo,
            IEnumerable<DanhMucBanChqsCapXa2026> danhMuc,
            IEnumerable<HoSoGisHqtc> hoSoHqtc,
            bool choNguonCu)
        {
            if (IsDisposed || Disposing)
                return;

            List<BanGhi> moi =
                new List<BanGhi>();

            // =================================================
            // 14.1. NGUON CU - CHI SU DUNG KHI NAP 1 NGUON
            // =================================================

            if (choNguonCu)
            {
                foreach (DonViQuanDoi d in
                    hoSo ??
                    Enumerable.Empty<DonViQuanDoi>())
                {
                    if (d == null ||
                        string.IsNullOrWhiteSpace(d.MaDonVi) ||
                        LaBanChqsQuanHuyen(d.TenDonVi))
                    {
                        continue;
                    }

                    bool gps =
                        NghiepVuDonVi.CoToaDoWgs84(d);

                    moi.Add(new BanGhi
                    {
                        MaDonVi = d.MaDonVi.Trim(),

                        TenDonVi = d.TenDonVi ?? "",

                        LoaiDonVi =
                            d.LoaiDonVi ?? "Chưa phân loại",

                        DiaBan = d.DiaBanHienThi,

                        TuKhoaThem =
                            (d.TenVietTat ?? "") + " " +
                            (d.DiaChi ?? ""),

                        LaCu = true,

                        CoDiem = gps,

                        KinhDo = gps
                            ? (double?)d.KinhDo
                            : null,

                        ViDo = gps
                            ? (double?)d.ViDo
                            : null,

                        LoaiViTri = gps
                            ? "Tọa độ hồ sơ WGS84"
                            : "Chưa có tọa độ"
                    });
                }
            }

            // =================================================
            // 14.2. BAN CHQS PHUONG/XA MOI
            // =================================================

            HashSet<string> maDvhc =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (DanhMucBanChqsCapXa2026 d in
                danhMuc ??
                Enumerable.Empty<DanhMucBanChqsCapXa2026>())
            {
                if (d == null ||
                    !d.DaGhepDiaBan ||
                    string.IsNullOrWhiteSpace(d.MaDvhc) ||
                    LaBanChqsQuanHuyen(d.TenDonVi))
                {
                    continue;
                }

                string ma = d.MaDvhc.Trim();

                if (!maDvhc.Add(ma))
                    continue;

                bool coDiem =
                    d.CoDiemDaiDien &&
                    ToaDoHopLe(
                        d.XDaiDien32648,
                        d.YDaiDien32648);

                moi.Add(new BanGhi
                {
                    MaDonVi = "HN-BCHQS-" + ma,  // trung ma ho so HQTCSDL
                    MaDvhc = ma,

                    TenDonVi = d.TenDonVi ?? "",

                    LoaiDonVi =
                        string.IsNullOrWhiteSpace(d.LoaiDonVi)
                        ? "Ban Chỉ huy quân sự"
                        : d.LoaiDonVi.Trim(),

                    DiaBan =
                        string.IsNullOrWhiteSpace(d.PhuongXa)
                        ? (d.TenXaPhuong ??
                           "Chưa xác định phường/xã")
                        : d.PhuongXa.Trim(),

                    TuKhoaThem =
                        (d.MaXaPhuong ?? "") + " " +
                        (d.DonViCapTren ?? ""),

                    LaBanChqs = true,
                    CoDiem = coDiem,

                    X = coDiem
                        ? d.XDaiDien32648
                        : null,

                    Y = coDiem
                        ? d.YDaiDien32648
                        : null,

                    LoaiViTri = coDiem
                        ? "Điểm đại diện địa bàn"
                        : "Chưa có điểm bản đồ"
                });
            }

            // =================================================
            // 14.3. CAC HO SO GIS CAP TREN
            // =================================================

            HashSet<long> idHoSo =
                new HashSet<long>();

            foreach (HoSoGisHqtc d in
                hoSoHqtc ??
                Enumerable.Empty<HoSoGisHqtc>())
            {
                if (d == null ||
                    string.IsNullOrWhiteSpace(d.TenDonVi) ||
                    LaBanChqsQuanHuyen(d.TenDonVi))
                {
                    continue;
                }

                if (!idHoSo.Add(d.Id))
                    continue;

                bool coDiem =
                    d.CoDiemBanDo &&
                    ToaDoHopLe(
                        d.X32648,
                        d.Y32648);

                moi.Add(new BanGhi
                {
                    MaDonVi =
                        string.IsNullOrWhiteSpace(d.MaDonVi)
                        ? "HQT-" + d.Id.ToString(
                            CultureInfo.InvariantCulture)
                        : d.MaDonVi.Trim(),

                    IdHoSo = d.Id,

                    TenDonVi = d.TenDonVi.Trim(),

                    LoaiDonVi =
                        !string.IsNullOrWhiteSpace(d.TenLoai)
                        ? d.TenLoai.Trim()
                        : (d.TenCap ?? "Chưa phân loại"),

                    TenCap = d.TenCap ?? "",

                    DiaBan =
                        string.IsNullOrWhiteSpace(d.DiaBan)
                        ? "Chưa xác minh phường/xã"
                        : d.DiaBan.Trim(),

                    TuKhoaThem =
                        (d.TenCap ?? "") + " " +
                        (d.TenXaPhuong ?? "") + " " +
                        (d.MaXaPhuong ?? ""),

                    LaHqtc = true,
                    CoDiem = coDiem,

                    X = coDiem
                        ? d.X32648
                        : null,

                    Y = coDiem
                        ? d.Y32648
                        : null,

                    KinhDo = d.KinhDo,
                    ViDo = d.ViDo,

                    LoaiViTri = coDiem
                        ? "Điểm hồ sơ GIS"
                        : "Chưa có điểm bản đồ"
                });
            }

            // =================================================
            // 14.4. CAP NHAT DANH SACH
            // =================================================

            string loaiCu =
                cboLoai.SelectedItem as string;

            string diaCu =
                cboDiaBan.SelectedItem as string;

            duLieu.Clear();

            duLieu.AddRange(
                moi
                .Where(d =>
                    !string.IsNullOrWhiteSpace(d.MaDonVi))
                .GroupBy(
                    d => d.MaDonVi,
                    StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First()));

            daNapDuLieu = true;
            dangCapNhatBoLoc = true;

            cboLoai.BeginUpdate();
            cboDiaBan.BeginUpdate();

            try
            {
                cboLoai.Items.Clear();
                cboDiaBan.Items.Clear();

                cboLoai.Items.Add(
                    "Tất cả loại đơn vị");

                cboDiaBan.Items.Add(
                    "Tất cả phường/xã");

                foreach (string s in duLieu
                    .Select(d => d.LoaiDonVi)
                    .Where(s =>
                        !string.IsNullOrWhiteSpace(s))
                    .Distinct(
                        StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(
                        s => s,
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    cboLoai.Items.Add(s);
                }

                foreach (string s in duLieu
                    .Select(d => d.DiaBan)
                    .Where(s =>
                        !string.IsNullOrWhiteSpace(s))
                    .Distinct(
                        StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(
                        s => s,
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    cboDiaBan.Items.Add(s);
                }

                cboLoai.SelectedIndex =
                    loaiCu != null &&
                    cboLoai.Items.Contains(loaiCu)
                    ? cboLoai.Items.IndexOf(loaiCu)
                    : 0;

                cboDiaBan.SelectedIndex =
                    diaCu != null &&
                    cboDiaBan.Items.Contains(diaCu)
                    ? cboDiaBan.Items.IndexOf(diaCu)
                    : 0;
            }
            finally
            {
                cboLoai.EndUpdate();
                cboDiaBan.EndUpdate();

                dangCapNhatBoLoc = false;
            }

            lblTrangThai.Text =
                "● " +
                duLieu.Count.ToString("N0", Vi) +
                " hồ sơ đã nạp";

            CapNhatMauNhom();
            ApDungBoLoc();
        }

        // =====================================================
        // 15. KIEM TRA TOA DO
        // =====================================================

        private static bool ToaDoHopLe(
            double? x,
            double? y)
        {
            return
                x.HasValue &&
                y.HasValue &&

                !double.IsNaN(x.Value) &&
                !double.IsNaN(y.Value) &&

                !double.IsInfinity(x.Value) &&
                !double.IsInfinity(y.Value) &&

                x.Value > 100000d &&
                x.Value < 900000d &&

                y.Value > 0d &&
                y.Value < 10000000d;
        }

        // =====================================================
        // 16. LOAI DU LIEU BAN CHQS CAP HUYEN CU
        // =====================================================

        private static bool LaBanChqsQuanHuyen(
            string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;

            string a = s.Trim().ToLowerInvariant();

            bool bch =
                a.StartsWith("ban chỉ huy quân sự") ||
                a.StartsWith("ban chqs");

            return bch &&
                (
                    a.Contains(" quân sự quận ") ||
                    a.Contains(" quân sự huyện ") ||
                    a.Contains(" quân sự thị xã ") ||
                    a.StartsWith("ban chqs quận ") ||
                    a.StartsWith("ban chqs huyện ") ||
                    a.StartsWith("ban chqs thị xã ")
                );
        }

        // =====================================================
        // 17. TIM KIEM
        // =====================================================

        public void TimKiemTheoTuKhoa(
            string tuKhoa)
        {
            if (IsDisposed || Disposing)
                return;

            txtTim.Text = tuKhoa ?? "";

            henTim.Stop();
            ApDungBoLoc();
        }

        private void HenLoc()
        {
            if (dangCapNhatBoLoc ||
                IsDisposed ||
                Disposing)
            {
                return;
            }

            henTim.Stop();
            henTim.Start();
        }

        private static bool Chua(
            string text,
            string key)
        {
            return key.Length == 0 ||
                Vi.CompareInfo.IndexOf(
                    text ?? "",
                    key,
                    CompareOptions.IgnoreCase |
                    CompareOptions.IgnoreNonSpace) >= 0;
        }

        // =====================================================
        // 18. LOC NHANH
        // =====================================================

        private bool ThuocNhom(BanGhi d)
        {
            switch (nhomHienTai)
            {
                case 1:
                    return d.LaBanChqs;

                case 2:
                    return !d.LaBanChqs;

                case 3:
                    return
                        Chua(d.TenDonVi, "Học viện") ||
                        Chua(d.LoaiDonVi, "Học viện");

                case 4:
                    return
                        Chua(d.TenDonVi, "Bộ tư lệnh") ||
                        Chua(d.LoaiDonVi, "Bộ tư lệnh");

                default:
                    return true;
            }
        }

        private void CapNhatMauNhom()
        {
            if (nutNhom[0] == null)
                return;

            int[] so =
            {
                duLieu.Count,

                duLieu.Count(
                    d => d.LaBanChqs),

                duLieu.Count(
                    d => !d.LaBanChqs),

                duLieu.Count(d =>
                    Chua(d.TenDonVi, "Học viện") ||
                    Chua(d.LoaiDonVi, "Học viện")),

                duLieu.Count(d =>
                    Chua(d.TenDonVi, "Bộ tư lệnh") ||
                    Chua(d.LoaiDonVi, "Bộ tư lệnh"))
            };

            string[] ten =
            {
                "Tất cả",
                "Ban CHQS",
                "Đơn vị các cấp",
                "Học viện",
                "Bộ tư lệnh"
            };

            for (int i = 0; i < nutNhom.Length; i++)
            {
                Button b = nutNhom[i];

                b.Text =
                    ten[i] + " · " +
                    so[i].ToString("N0", Vi);

                b.BackColor =
                    i == nhomHienTai
                    ? Color.FromArgb(219, 249, 239)
                    : Color.White;

                b.ForeColor =
                    i == nhomHienTai
                    ? Color.FromArgb(0, 128, 107)
                    : Chu;
            }
        }

        // =====================================================
        // 19. AP DUNG BO LOC
        // =====================================================

        private void ApDungBoLoc()
        {
            if (dangCapNhatBoLoc ||
                IsDisposed ||
                Disposing)
            {
                return;
            }

            string key = txtTim.Text.Trim();

            string loai =
                cboLoai.SelectedIndex > 0
                ? cboLoai.SelectedItem as string
                : null;

            string dia =
                cboDiaBan.SelectedIndex > 0
                ? cboDiaBan.SelectedItem as string
                : null;

            ketQua = duLieu
                .Where(d =>
                    ThuocNhom(d) &&

                    (!chkCoDiem.Checked || d.CoDiem) &&

                    (
                        loai == null ||
                        string.Equals(
                            d.LoaiDonVi,
                            loai,
                            StringComparison.CurrentCultureIgnoreCase)
                    ) &&

                    (
                        dia == null ||
                        string.Equals(
                            d.DiaBan,
                            dia,
                            StringComparison.CurrentCultureIgnoreCase)
                    ) &&

                    (
                        Chua(d.MaDonVi, key) ||
                        Chua(d.TenDonVi, key) ||
                        Chua(d.LoaiDonVi, key) ||
                        Chua(d.DiaBan, key) ||
                        Chua(d.TuKhoaThem, key)
                    ))
                .OrderBy(
                    d => d.TenDonVi,
                    StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            trangHienTai = 0;

            dangChon =
                ketQua.FirstOrDefault();

            lblKetQua.Text =
                ketQua.Count.ToString("N0", Vi) +
                " hồ sơ";

            VeTrang();
            CapNhatTrangThaiChon();

            soDo.NapDiem(
                duLieu,
                ketQua,
                dangChon);
        }

        // =====================================================
        // 20. XOA BO LOC
        // =====================================================

        private void XoaBoLoc()
        {
            dangCapNhatBoLoc = true;

            try
            {
                txtTim.Clear();

                if (cboLoai.Items.Count > 0)
                    cboLoai.SelectedIndex = 0;

                if (cboDiaBan.Items.Count > 0)
                    cboDiaBan.SelectedIndex = 0;

                chkCoDiem.Checked = false;

                nhomHienTai = 0;
            }
            finally
            {
                dangCapNhatBoLoc = false;
            }

            henTim.Stop();

            CapNhatMauNhom();
            ApDungBoLoc();
        }

        // =====================================================
        // 21. PHAN TRANG
        // =====================================================

        private int SoTrang
        {
            get
            {
                return
                    (ketQua.Count +
                     KichThuocTrang - 1) /
                    KichThuocTrang;
            }
        }

        private void DoiTrang(int chenh)
        {
            int moi = trangHienTai + chenh;

            if (moi < 0 || moi >= SoTrang)
                return;

            trangHienTai = moi;

            dangChon = ketQua
                .Skip(
                    trangHienTai * KichThuocTrang)
                .FirstOrDefault();

            VeTrang();
            CapNhatTrangThaiChon();
            soDo.Chon(dangChon);
        }

        // =====================================================
        // 22. VE DANH SACH THE
        // =====================================================

        private void VeTrang()
        {
            dsThe.SuspendLayout();

            try
            {
                while (dsThe.Controls.Count > 0)
                {
                    Control c = dsThe.Controls[0];

                    dsThe.Controls.RemoveAt(0);
                    c.Dispose();
                }

                foreach (BanGhi d in ketQua
                    .Skip(
                        trangHienTai * KichThuocTrang)
                    .Take(KichThuocTrang))
                {
                    dsThe.Controls.Add(
                        TaoTheKetQua(d));
                }

                if (ketQua.Count == 0)
                {
                    Label rong = Nhan(
                        "Không tìm thấy đơn vị phù hợp.\n" +
                        "Hãy thay đổi từ khóa hoặc bộ lọc.",
                        10F,
                        false,
                        Phu);

                    rong.Width = 290;
                    rong.Height = 95;

                    rong.TextAlign =
                        ContentAlignment.MiddleCenter;

                    dsThe.Controls.Add(rong);
                }
            }
            finally
            {
                dsThe.ResumeLayout(true);
            }

            int dau = ketQua.Count == 0
                ? 0
                : trangHienTai * KichThuocTrang + 1;

            int cuoi = Math.Min(
                ketQua.Count,
                (trangHienTai + 1) *
                KichThuocTrang);

            lblPhanTrang.Text =
                "Hiển thị " + dau +
                "–" + cuoi +
                " / " + ketQua.Count +
                " · Trang " +
                (SoTrang == 0 ? 0 : trangHienTai + 1) +
                "/" + SoTrang;

            btnTruoc.Enabled =
                trangHienTai > 0;

            btnSau.Enabled =
                trangHienTai + 1 < SoTrang;

            CanThe();
        }

        private void CanThe()
        {
            if (dsThe == null ||
                dsThe.IsDisposed)
            {
                return;
            }

            int rong = Math.Max(
                180,
                dsThe.ClientSize.Width -
                SystemInformation.VerticalScrollBarWidth -
                5);

            foreach (Control c in dsThe.Controls)
            {
                c.Width = rong;
            }
        }

        // =====================================================
        // 23. TAO THE DON VI
        // =====================================================

        private Panel TaoTheKetQua(BanGhi d)
        {
            bool chon =
                object.ReferenceEquals(
                    dangChon, d);

            Color mau =
                d.LaBanChqs ? Ngoc : Xanh;

            RoundedPanel card = The(
                chon
                ? Color.FromArgb(233, 243, 255)
                : Color.White,

                chon
                ? Color.FromArgb(114, 169, 247)
                : Vien);

            card.Dock = DockStyle.None;

            card.Width = 340;
            card.Height = 78;

            card.Margin =
                new Padding(0, 0, 0, 7);

            card.Padding =
                new Padding(9, 7, 8, 6);

            card.Cursor = Cursors.Hand;
            card.Tag = d;

            Panel icon = new Panel
            {
                Dock = DockStyle.Left,
                Width = 39,
                BackColor = Color.Transparent
            };

            icon.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (Brush b =
                    new SolidBrush(mau))
                {
                    e.Graphics.FillEllipse(
                        b, 3, 10, 29, 29);
                }

                using (Font f = new Font(
                    "Segoe UI Symbol",
                    10F,
                    FontStyle.Bold))
                {
                    TextRenderer.DrawText(
                        e.Graphics,

                        d.LaBanChqs
                        ? "✦"
                        : "▦",

                        f,

                        new Rectangle(
                            3, 10, 29, 29),

                        Color.White,

                        TextFormatFlags.HorizontalCenter |
                        TextFormatFlags.VerticalCenter);
                }
            };

            Panel text = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            Label ten = Nhan(
                d.TenDonVi,
                9.3F,
                true,
                Chu);

            ten.Dock = DockStyle.Top;
            ten.Height = 25;

            Label ma = Nhan(
                d.MaDonVi +
                " · " + d.LoaiDonVi,
                8F,
                false,
                Phu);

            ma.Dock = DockStyle.Top;
            ma.Height = 19;

            Label dia = Nhan(
                "● " + d.DiaBan,
                8.2F,
                true,
                d.CoDiem ? Ngoc : Do);

            dia.Dock = DockStyle.Fill;

            text.Controls.Add(dia);
            text.Controls.Add(ma);
            text.Controls.Add(ten);

            card.Controls.Add(text);
            card.Controls.Add(icon);

            EventHandler click = (s, e) =>
                ChonBanGhi(d, false, true);

            foreach (Control c in new Control[]
            {
                card, icon, text, ten, ma, dia
            })
            {
                c.Click += click;

                c.DoubleClick += (s, e) =>
                {
                    ChonBanGhi(d, false, false);
                    MoChiTiet();
                };

                meo.SetToolTip(c, d.TenDonVi);
            }

            return card;
        }

        // =====================================================
        // 24. CHON BAN GHI
        // =====================================================

        private void ChonBanGhi(
            BanGhi d,
            bool tuBanDo,
            bool phongToDiem)
        {
            if (d == null ||
                !ketQua.Contains(d))
            {
                return;
            }

            dangChon = d;

            int index = ketQua.IndexOf(d);

            trangHienTai =
                index / KichThuocTrang;

            VeTrang();
            CapNhatTrangThaiChon();

            if (phongToDiem && ToaDoHopLe(d.X, d.Y))
            {
                soDo.TapTrungVao(d);
            }
            else
            {
                soDo.Chon(d);
            }
        }

        // =====================================================
        // 25. TRANG THAI CHON - KHONG CO KHUNG HO SO
        // =====================================================

        private void CapNhatTrangThaiChon()
        {
            BanGhi d = dangChon;

            btnXemGIS.Enabled =
                d != null && d.CoDiem;

            btnChiTiet.Enabled =
                d != null;

            lblThongTinBanDo.Text =
                d == null
                ? "Chọn một điểm hoặc một đơn vị để thao tác"
                : d.CoDiem
                    ? "Đã chọn một vị trí · Có thể mở trên bản đồ GIS"
                    : "Đơn vị này chưa có vị trí bản đồ hợp lệ";
        }

        // =====================================================
        // 26. DINH VI TREN MAPWINGIS
        // =====================================================

        private void DinhViBanGhi(BanGhi d)
        {
            if (d == null || !d.CoDiem)
                return;

            if (d.LaBanChqs &&
                ToaDoHopLe(d.X, d.Y))
            {
                Action<string, double, double, string> h =
                    YeuCauDinhViDiemDaiDien;

                if (h != null)
                {
                    h(
                        d.MaDvhc,
                        d.X.Value,
                        d.Y.Value,
                        d.TenDonVi);

                    return;
                }
            }

            if (d.LaHqtc &&
                ToaDoHopLe(d.X, d.Y))
            {
                Action<long, double, double, string> h =
                    YeuCauDinhViHoSoHqtc;

                if (h != null)
                {
                    h(
                        d.IdHoSo,
                        d.X.Value,
                        d.Y.Value,
                        d.TenDonVi);

                    return;
                }
            }

            if (d.LaCu)
            {
                Action<string> h =
                    YeuCauDinhViDonVi;

                if (h != null)
                {
                    h(d.MaDonVi);
                    return;
                }
            }

            MessageBox.Show(
                this,
                "Chưa kết nối sự kiện định vị " +
                "giữa giao diện Tra cứu và FormMain.",
                "Định vị GIS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // =====================================================
        // 27. XEM CHI TIET HO SO
        // =====================================================

        private void MoChiTiet()
        {
            BanGhi d = dangChon;

            if (d == null)
                return;

            if (d.LaCu &&
                YeuCauXemChiTietDonVi != null)
            {
                YeuCauXemChiTietDonVi(
                    d.MaDonVi);

                return;
            }

            using (Form dlg = new Form())
            {
                dlg.Text =
                    "Thông tin đơn vị quân đội";

                dlg.StartPosition =
                    FormStartPosition.CenterParent;

                dlg.FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;

                dlg.ShowInTaskbar = false;

                dlg.ClientSize =
                    new Size(725, 475);

                dlg.BackColor = Nen;

                dlg.Font =
                    new Font("Segoe UI", 9.5F);

                Panel head = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 84,

                    BackColor =
                        Color.FromArgb(20, 86, 110),

                    Padding =
                        new Padding(23, 10, 18, 10)
                };

                Label title = Nhan(
                    d.TenDonVi,
                    15F,
                    true,
                    Color.White);

                title.Dock = DockStyle.Fill;

                head.Controls.Add(title);

                Panel foot = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 62,

                    BackColor = Color.White,

                    Padding =
                        new Padding(20, 12, 20, 10)
                };

                Button dong = Nut(
                    "Đóng",
                    Color.FromArgb(104, 120, 140),
                    Color.White,
                    105,
                    35);

                Button dinhVi = Nut(
                    "⌖ Xem trên bản đồ GIS",
                    Xanh,
                    Color.White,
                    189,
                    35);

                dong.Dock = DockStyle.Right;
                dinhVi.Dock = DockStyle.Right;

                dinhVi.Enabled = d.CoDiem;

                dong.Click += (s, e) =>
                {
                    dlg.DialogResult =
                        DialogResult.Cancel;

                    dlg.Close();
                };

                dinhVi.Click += (s, e) =>
                {
                    dlg.DialogResult =
                        DialogResult.Yes;

                    dlg.Close();
                };

                foot.Controls.Add(dong);
                foot.Controls.Add(dinhVi);

                Panel body = new Panel
                {
                    Dock = DockStyle.Fill,
                    AutoScroll = true,

                    BackColor = Color.White,

                    Padding =
                        new Padding(25, 15, 25, 15)
                };

                TableLayoutPanel bang =
                    new TableLayoutPanel
                    {
                        Dock = DockStyle.Top,

                        ColumnCount = 2,
                        RowCount = 0,

                        AutoSize = true,

                        AutoSizeMode =
                            AutoSizeMode.GrowAndShrink,

                        BackColor = Color.White
                    };

                bang.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Absolute, 165));

                bang.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent, 100));

                DongHoSo(
                    bang, "Mã hồ sơ", d.MaDonVi);

                DongHoSo(
                    bang, "Tên đơn vị", d.TenDonVi);

                DongHoSo(
                    bang, "Loại đơn vị", d.LoaiDonVi);

                if (!string.IsNullOrWhiteSpace(d.TenCap))
                {
                    DongHoSo(
                        bang, "Cấp đơn vị", d.TenCap);
                }

                DongHoSo(
                    bang, "Địa bàn", d.DiaBan);

                DongHoSo(
                    bang, "Loại vị trí", d.LoaiViTri);

                if (ToaDoHopLe(d.X, d.Y))
                {
                    DongHoSo(
                        bang,
                        "X / EPSG:32648",
                        d.X.Value.ToString("N2", Vi) + " m");

                    DongHoSo(
                        bang,
                        "Y / EPSG:32648",
                        d.Y.Value.ToString("N2", Vi) + " m");
                }

                if (d.KinhDo.HasValue &&
                    d.ViDo.HasValue)
                {
                    DongHoSo(
                        bang,
                        "Kinh độ WGS84",
                        d.KinhDo.Value.ToString(
                            "F6",
                            CultureInfo.InvariantCulture));

                    DongHoSo(
                        bang,
                        "Vĩ độ WGS84",
                        d.ViDo.Value.ToString(
                            "F6",
                            CultureInfo.InvariantCulture));
                }

                if (d.LaBanChqs)
                {
                    DongHoSo(
                        bang,
                        "Lưu ý",
                        "Điểm đại diện phường/xã không " +
                        "phải tọa độ trụ sở đã xác minh.");
                }

                body.Controls.Add(bang);

                dlg.Controls.Add(body);
                dlg.Controls.Add(foot);
                dlg.Controls.Add(head);

                dlg.CancelButton = dong;

                DialogResult result =
                    dlg.ShowDialog(FindForm());

                if (result == DialogResult.Yes)
                {
                    DinhViBanGhi(d);
                }
            }
        }

        private static void DongHoSo(
            TableLayoutPanel bang,
            string nhan,
            string giaTri)
        {
            int row = bang.RowCount++;

            bang.RowStyles.Add(
                new RowStyle(SizeType.AutoSize));

            Label a = Nhan(
                nhan, 9.2F, false, Phu);

            a.AutoSize = true;
            a.Dock = DockStyle.Fill;

            a.Margin =
                new Padding(0, 9, 12, 9);

            Label b = Nhan(
                string.IsNullOrWhiteSpace(giaTri)
                ? "Chưa cập nhật"
                : giaTri,
                9.6F,
                true,
                Chu);

            b.AutoSize = true;
            b.Dock = DockStyle.Fill;

            b.MaximumSize =
                new Size(450, 0);

            b.Margin =
                new Padding(0, 8, 0, 9);

            bang.Controls.Add(a, 0, row);
            bang.Controls.Add(b, 1, row);
        }

        // =====================================================
        // 28. BAN GHI NOI BO
        // =====================================================

        private sealed class BanGhi
        {
            public string MaDonVi;
            public string MaDvhc;

            public string TenDonVi;
            public string LoaiDonVi;
            public string TenCap;

            public string DiaBan;
            public string TuKhoaThem;
            public string LoaiViTri;

            public long IdHoSo;

            public bool LaBanChqs;
            public bool LaHqtc;
            public bool LaCu;
            public bool CoDiem;

            public double? X;
            public double? Y;

            public double? KinhDo;
            public double? ViDo;
        }

        // =====================================================
        // 29. SO DO KHONG GIAN PHONG TO
        // =====================================================

        private sealed class SoDoKhongGian : Control
        {
            private List<BanGhi> tatCa =
                new List<BanGhi>();

            private HashSet<BanGhi> daLoc =
                new HashSet<BanGhi>();

            private BanGhi selected;
            private BanGhi hover;

            private double centerX;
            private double centerY;

            private double spanX = 20000d;
            private double spanY = 20000d;

            private double zoom = 1d;

            private bool dangKeo;
            private bool daDiChuyen;

            private Point diemBatDau;

            private double centerXBatDau;
            private double centerYBatDau;

            private readonly ToolTip toolTip =
                new ToolTip();

            public event Action<BanGhi>
                YeuCauChon;

            public event Action<BanGhi>
                YeuCauMoBanDo;

            // =================================================
            // 29.1. KHOI TAO SO DO
            // =================================================

            public SoDoKhongGian()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);

                BackColor =
                    Color.FromArgb(235, 245, 252);

                ForeColor = Chu;

                Cursor = Cursors.Hand;

                toolTip.InitialDelay = 300;
                toolTip.ReshowDelay = 100;
                toolTip.AutoPopDelay = 7000;
            }

            // =================================================
            // 29.2. NAP CAC DIEM TOA DO
            // =================================================

            public void NapDiem(
                IEnumerable<BanGhi> nguon,
                IEnumerable<BanGhi> ketQua,
                BanGhi chon)
            {
                tatCa = (nguon ??
                    Enumerable.Empty<BanGhi>())
                    .Where(d =>
                        d != null &&
                        ToaDoHopLe(d.X, d.Y))
                    .ToList();

                daLoc = new HashSet<BanGhi>(
                    (ketQua ??
                        Enumerable.Empty<BanGhi>())
                    .Where(d => d != null));

                selected = chon;
                hover = null;

                VuaKhung();
            }

            // =================================================
            // 29.3. VUA KHUNG TOAN BO DIEM
            // =================================================

            public void VuaKhung()
            {
                zoom = 1d;

                if (tatCa.Count == 0)
                {
                    centerX = 0d;
                    centerY = 0d;

                    spanX = 20000d;
                    spanY = 20000d;

                    Invalidate();
                    return;
                }

                List<BanGhi> diemLoc =
                    tatCa.Where(d =>
                        daLoc.Contains(d)).ToList();

                // Neu khong co ket qua loc,
                // giu pham vi tat ca diem de tham chieu.
                List<BanGhi> phamVi =
                    diemLoc.Count > 0
                    ? diemLoc
                    : tatCa;

                double minX =
                    phamVi.Min(d => d.X.Value);

                double maxX =
                    phamVi.Max(d => d.X.Value);

                double minY =
                    phamVi.Min(d => d.Y.Value);

                double maxY =
                    phamVi.Max(d => d.Y.Value);

                centerX =
                    (minX + maxX) / 2d;

                centerY =
                    (minY + maxY) / 2d;

                spanX = Math.Max(
                    1000d,
                    (maxX - minX) * 1.18d);

                spanY = Math.Max(
                    1000d,
                    (maxY - minY) * 1.18d);

                Invalidate();
            }

            // =================================================
            // 29.4. THU PHONG
            // =================================================

            public void ThuPhong(double heSo)
            {
                if (heSo <= 0d ||
                    double.IsNaN(heSo) ||
                    double.IsInfinity(heSo))
                {
                    return;
                }

                zoom = Math.Max(
                    0.5d,
                    Math.Min(
                        80d,
                        zoom * heSo));

                Invalidate();
            }

            // =================================================
            // 29.5. CHON DIEM
            // =================================================

            public void Chon(BanGhi d)
            {
                selected = d;
                Invalidate();
            }

            // =================================================
            // 29.6. TAP TRUNG VAO DIEM
            // =================================================

            public void TapTrungVao(BanGhi d)
            {
                if (d == null ||
                    !ToaDoHopLe(d.X, d.Y))
                {
                    return;
                }

                selected = d;

                centerX = d.X.Value;
                centerY = d.Y.Value;

                zoom = Math.Max(zoom, 3d);

                Invalidate();
            }

            // =================================================
            // 29.7. KHUNG VE
            // =================================================

            private RectangleF VungVe
            {
                get
                {
                    return new RectangleF(
                        21F,
                        30F,

                        Math.Max(
                            1F, Width - 42F),

                        Math.Max(
                            1F, Height - 67F));
                }
            }

            // =================================================
            // 29.8. TY LE VE
            // =================================================

            private double TyLe
            {
                get
                {
                    RectangleF r = VungVe;

                    return Math.Min(
                        r.Width / spanX,
                        r.Height / spanY) * zoom;
                }
            }

            // =================================================
            // 29.9. CHUYEN TOA DO SANG PIXEL
            // =================================================

            private PointF TinhPixel(BanGhi d)
            {
                RectangleF r = VungVe;

                double scale = TyLe;

                return new PointF(
                    (float)(
                        r.Left +
                        r.Width / 2d +
                        (d.X.Value - centerX) * scale),

                    (float)(
                        r.Top +
                        r.Height / 2d -
                        (d.Y.Value - centerY) * scale));
            }

            // =================================================
            // 29.10. VE SO DO
            // =================================================

            protected override void OnPaint(
                PaintEventArgs e)
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;

                g.SmoothingMode =
                    SmoothingMode.AntiAlias;

                g.TextRenderingHint =
                    System.Drawing.Text.TextRenderingHint
                    .ClearTypeGridFit;

                g.Clear(BackColor);

                RectangleF r = VungVe;

                // =============================================
                // VE LUOI
                // =============================================

                using (Pen luoi = new Pen(
                    Color.FromArgb(215, 230, 241),
                    1F))
                {
                    luoi.DashStyle = DashStyle.Dot;

                    for (int i = 0; i <= 8; i++)
                    {
                        float x =
                            r.Left +
                            r.Width * i / 8F;

                        float y =
                            r.Top +
                            r.Height * i / 8F;

                        g.DrawLine(
                            luoi,
                            x, r.Top,
                            x, r.Bottom);

                        g.DrawLine(
                            luoi,
                            r.Left, y,
                            r.Right, y);
                    }
                }

                // =============================================
                // VE HUONG BAC
                // =============================================

                using (Font f = new Font(
                    "Segoe UI", 8F,
                    FontStyle.Bold))
                using (Brush b =
                    new SolidBrush(Phu))
                {
                    g.DrawString(
                        "N ↑",
                        f,
                        b,
                        r.Left + 4,
                        r.Top - 21);
                }

                // =============================================
                // KHONG CO DU LIEU
                // =============================================

                if (tatCa.Count == 0)
                {
                    VeThongBaoTrong(g);

                    VeChuThich(g);
                    return;
                }

                GraphicsState oldState =
                    g.Save();

                g.SetClip(r);

                // =============================================
                // VE CAC DIEM KHONG THUOC BO LOC
                // =============================================

                foreach (BanGhi d in tatCa)
                {
                    if (daLoc.Contains(d))
                        continue;

                    PointF p = TinhPixel(d);

                    using (Brush b = new SolidBrush(
                        Color.FromArgb(183, 197, 211)))
                    {
                        g.FillEllipse(
                            b,
                            p.X - 2.3F,
                            p.Y - 2.3F,
                            4.6F,
                            4.6F);
                    }
                }

                // =============================================
                // VE CAC DIEM THUOC BO LOC
                // =============================================

                foreach (BanGhi d in tatCa)
                {
                    if (!daLoc.Contains(d))
                        continue;

                    if (object.ReferenceEquals(
                        d, selected))
                    {
                        continue;
                    }

                    PointF p = TinhPixel(d);

                    Color mau =
                        d.LaBanChqs
                        ? Ngoc
                        : Xanh;

                    using (Brush b =
                        new SolidBrush(mau))
                    using (Pen vien = new Pen(
                        Color.White, 1.1F))
                    {
                        g.FillEllipse(
                            b,
                            p.X - 4F,
                            p.Y - 4F,
                            8F,
                            8F);

                        g.DrawEllipse(
                            vien,
                            p.X - 4F,
                            p.Y - 4F,
                            8F,
                            8F);
                    }
                }

                // =============================================
                // VE DIEM DANG CHON
                // =============================================

                if (selected != null &&
                    ToaDoHopLe(
                        selected.X,
                        selected.Y))
                {
                    PointF p =
                        TinhPixel(selected);

                    using (Brush nenVong =
                        new SolidBrush(
                            Color.FromArgb(65, Vang)))
                    using (Pen ring =
                        new Pen(Vang, 3F))
                    using (Brush giua =
                        new SolidBrush(Tim))
                    using (Pen vien =
                        new Pen(Color.White, 2F))
                    {
                        g.FillEllipse(
                            nenVong,
                            p.X - 17F,
                            p.Y - 17F,
                            34F,
                            34F);

                        g.DrawEllipse(
                            ring,
                            p.X - 11F,
                            p.Y - 11F,
                            22F,
                            22F);

                        g.FillEllipse(
                            giua,
                            p.X - 5F,
                            p.Y - 5F,
                            10F,
                            10F);

                        g.DrawEllipse(
                            vien,
                            p.X - 5F,
                            p.Y - 5F,
                            10F,
                            10F);
                    }
                }

                g.Restore(oldState);

                // =============================================
                // CHU THICH VA THONG TIN
                // =============================================

                VeChuThich(g);
            }

            // =================================================
            // 29.11. THONG BAO KHI KHONG CO DIEM
            // =================================================

            private void VeThongBaoTrong(Graphics g)
            {
                RectangleF r = VungVe;

                string s =
                    "Chưa có điểm không gian hợp lệ";

                using (Font f = new Font(
                    "Segoe UI", 10F))
                using (Brush b =
                    new SolidBrush(Phu))
                {
                    SizeF size =
                        g.MeasureString(s, f);

                    g.DrawString(
                        s,
                        f,
                        b,

                        r.Left +
                        (r.Width - size.Width) / 2F,

                        r.Top +
                        (r.Height - size.Height) / 2F);
                }
            }

            // =================================================
            // 29.12. CHU THICH SO DO
            // =================================================

            private void VeChuThich(Graphics g)
            {
                int y =
                    Math.Max(0, Height - 31);

                using (Font f = new Font(
                    "Segoe UI", 8F))
                using (Brush b =
                    new SolidBrush(Phu))
                {
                    string text =
                        "● Ban CHQS   ● Đơn vị các cấp" +
                        "   ·   " +
                        daLoc.Count(d =>
                            d.CoDiem &&
                            ToaDoHopLe(d.X, d.Y)) +
                        "/" +
                        tatCa.Count +
                        " điểm";

                    g.DrawString(
                        text,
                        f,
                        b,
                        12F,
                        y);
                }

                using (Brush b =
                    new SolidBrush(Ngoc))
                {
                    g.FillEllipse(
                        b,
                        12F,
                        y - 11F,
                        7F,
                        7F);
                }

                using (Brush b =
                    new SolidBrush(Xanh))
                {
                    g.FillEllipse(
                        b,
                        98F,
                        y - 11F,
                        7F,
                        7F);
                }
            }

            // =================================================
            // 29.13. TIM DIEM GAN CON TRO
            // =================================================

            private BanGhi TimDiemGan(
                Point p,
                double banKinh)
            {
                BanGhi best = null;
                double min = banKinh;

                foreach (BanGhi d in tatCa)
                {
                    if (!daLoc.Contains(d))
                        continue;

                    PointF q = TinhPixel(d);

                    double dx = q.X - p.X;
                    double dy = q.Y - p.Y;

                    double kc = Math.Sqrt(
                        dx * dx + dy * dy);

                    if (kc < min)
                    {
                        min = kc;
                        best = d;
                    }
                }

                return best;
            }

            // =================================================
            // 29.14. BAT DAU KEO BAN DO
            // =================================================

            protected override void OnMouseDown(
                MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button != MouseButtons.Left)
                    return;

                dangKeo = true;
                daDiChuyen = false;

                diemBatDau = e.Location;

                centerXBatDau = centerX;
                centerYBatDau = centerY;

                Capture = true;
            }

            // =================================================
            // 29.15. DI CHUYEN BAN DO
            // =================================================

            protected override void OnMouseMove(
                MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (dangKeo)
                {
                    int dx =
                        e.X - diemBatDau.X;

                    int dy =
                        e.Y - diemBatDau.Y;

                    if (Math.Abs(dx) > 4 ||
                        Math.Abs(dy) > 4)
                    {
                        daDiChuyen = true;
                    }

                    if (daDiChuyen &&
                        TyLe > 0d)
                    {
                        centerX =
                            centerXBatDau -
                            dx / TyLe;

                        centerY =
                            centerYBatDau +
                            dy / TyLe;

                        Cursor = Cursors.SizeAll;
                        Invalidate();
                    }

                    return;
                }

                BanGhi d =
                    TimDiemGan(e.Location, 11d);

                Cursor =
                    d != null
                    ? Cursors.Hand
                    : Cursors.Default;

                if (object.ReferenceEquals(
                    d, hover))
                {
                    return;
                }

                hover = d;

                toolTip.SetToolTip(
                    this,

                    d == null
                    ? ""
                    : d.TenDonVi +
                      "\n" + d.DiaBan +
                      "\nNhấn để chọn; nhấp đúp để xem trên bản đồ GIS.");
            }

            // =================================================
            // 29.16. THA CHUOT / CHON DIEM
            // =================================================

            protected override void OnMouseUp(
                MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (!dangKeo)
                    return;

                dangKeo = false;
                Capture = false;

                Cursor = Cursors.Default;

                if (daDiChuyen)
                {
                    daDiChuyen = false;
                    return;
                }

                if (e.Button != MouseButtons.Left)
                    return;

                BanGhi d =
                    TimDiemGan(e.Location, 12d);

                if (d == null)
                    return;

                selected = d;
                Invalidate();

                Action<BanGhi> h =
                    YeuCauChon;

                if (h != null)
                    h(d);
            }

            // =================================================
            // 29.17. MO BAN DO GIS KHI NHAP DUP
            // =================================================

            protected override void OnMouseDoubleClick(
                MouseEventArgs e)
            {
                base.OnMouseDoubleClick(e);

                if (e.Button != MouseButtons.Left)
                    return;

                BanGhi d =
                    TimDiemGan(e.Location, 13d);

                if (d == null)
                    return;

                selected = d;
                Invalidate();

                Action<BanGhi> h =
                    YeuCauMoBanDo;

                if (h != null)
                    h(d);
            }

            // =================================================
            // 29.18. LAN CHUOT DE ZOOM
            // =================================================

            protected override void OnMouseWheel(
                MouseEventArgs e)
            {
                base.OnMouseWheel(e);

                ThuPhong(
                    e.Delta > 0
                    ? 1.25d
                    : 1d / 1.25d);
            }

            // =================================================
            // 29.19. GIAI PHONG TAI NGUYEN
            // =================================================

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    toolTip.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        // =====================================================
        // 30. CAC HAM TAO CONTROL
        // =====================================================

        private static Label Nhan(
            string text,
            float co,
            bool dam,
            Color mau)
        {
            return new Label
            {
                Text = text,

                AutoEllipsis = true,

                BackColor = Color.Transparent,
                ForeColor = mau,

                TextAlign =
                    ContentAlignment.MiddleLeft,

                Font = new Font(
                    "Segoe UI",
                    co,

                    dam
                    ? FontStyle.Bold
                    : FontStyle.Regular)
            };
        }

        private static ComboBox Combo()
        {
            return new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList,

                FlatStyle = FlatStyle.Flat,

                Font =
                    new Font("Segoe UI", 9F),

                BackColor = Color.White,
                ForeColor = Chu,

                IntegralHeight = false,
                MaxDropDownItems = 14
            };
        }

        private static Button Nut(
            string text,
            Color nen,
            Color chu,
            int rong,
            int cao)
        {
            Button b = new Button
            {
                Text = text,

                Width = rong,
                Height = cao,

                BackColor = nen,
                ForeColor = chu,

                FlatStyle = FlatStyle.Flat,
                UseVisualStyleBackColor = false,

                Font = new Font(
                    "Segoe UI",
                    8.7F,
                    FontStyle.Bold),

                Cursor = Cursors.Hand,

                Margin =
                    new Padding(0, 1, 8, 1)
            };

            b.FlatAppearance.BorderSize = 0;

            b.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(nen, 0.10F);

            b.HandleCreated += (s, e) =>
                BoNut(b);

            b.Resize += (s, e) =>
                BoNut(b);

            return b;
        }

        private static void BoNut(Button b)
        {
            if (b.Width < 5 || b.Height < 5)
                return;

            using (GraphicsPath gp = Goc(
                new Rectangle(
                    0, 0,
                    b.Width,
                    b.Height),
                8))
            {
                Region cu = b.Region;

                b.Region = new Region(gp);

                if (cu != null)
                    cu.Dispose();
            }
        }

        private static RoundedPanel The(
            Color nen,
            Color vien)
        {
            return new RoundedPanel
            {
                Dock = DockStyle.Fill,

                BackColor = nen,
                MauVien = vien
            };
        }

        // =====================================================
        // 31. GOC BO TRON
        // =====================================================

        private static GraphicsPath Goc(
            Rectangle r,
            int radius)
        {
            int d = Math.Max(
                2,
                Math.Min(
                    radius * 2,
                    Math.Min(r.Width, r.Height)));

            GraphicsPath gp =
                new GraphicsPath();

            gp.AddArc(
                r.Left,
                r.Top,
                d, d,
                180, 90);

            gp.AddArc(
                r.Right - d,
                r.Top,
                d, d,
                270, 90);

            gp.AddArc(
                r.Right - d,
                r.Bottom - d,
                d, d,
                0, 90);

            gp.AddArc(
                r.Left,
                r.Bottom - d,
                d, d,
                90, 90);

            gp.CloseFigure();

            return gp;
        }

        // =====================================================
        // 32. PANEL BO GOC
        // =====================================================

        private sealed class RoundedPanel : Panel
        {
            public Color MauVien { get; set; }

            public RoundedPanel()
            {
                DoubleBuffered = true;
                MauVien = Vien;

                Resize += (s, e) =>
                {
                    if (Width < 5 || Height < 5)
                        return;

                    using (GraphicsPath gp = Goc(
                        new Rectangle(
                            0, 0,
                            Width,
                            Height),
                        12))
                    {
                        Region cu = Region;

                        Region = new Region(gp);

                        if (cu != null)
                            cu.Dispose();
                    }
                };
            }

            protected override void OnPaint(
                PaintEventArgs e)
            {
                base.OnPaint(e);

                if (Width < 5 || Height < 5)
                    return;

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (GraphicsPath gp = Goc(
                    new Rectangle(
                        0, 0,
                        Width - 1,
                        Height - 1),
                    12))
                using (Pen p =
                    new Pen(MauVien, 1F))
                {
                    e.Graphics.DrawPath(p, gp);
                }
            }
        }

        // =====================================================
        // 33. GIAI PHONG TAI NGUYEN
        // =====================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (henTim != null)
                {
                    henTim.Stop();
                    henTim.Dispose();
                }

                meo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
