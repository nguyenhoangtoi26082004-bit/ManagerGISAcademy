
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Views
{
    // Giu nguyen hop dong su kien voi FormMain.cs, BLL va MapWinGIS.
    public enum LoaiPhanTichKhongGian
    {
        DoKhoangCach,
        TaoVungDem,
        TimDoiTuongGan
    }

    public sealed class YeuCauPhanTichArgs : EventArgs
    {
        public LoaiPhanTichKhongGian LoaiPhanTich { get; private set; }
        public string MaDoiTuongNguon { get; private set; }
        public decimal BanKinhMet { get; private set; }
        public int GioiHanKetQua { get; private set; }

        public YeuCauPhanTichArgs(
            LoaiPhanTichKhongGian loai, string ma,
            decimal banKinh, int gioiHan)
        {
            LoaiPhanTich = loai;
            MaDoiTuongNguon = ma ?? string.Empty;
            BanKinhMet = banKinh;
            GioiHanKetQua = gioiHan;
        }
    }

    public sealed class KetQuaPhanTichItem
    {
        public string MaDoiTuong { get; set; }
        public string TenDoiTuong { get; set; }
        public string TenLop { get; set; }
        public double? KhoangCachMet { get; set; }
        public string GhiChu { get; set; }
        public double? KinhDo { get; set; }
        public double? ViDo { get; set; }

        public string KhoangCachHienThi
        {
            get
            {
                if (!KhoangCachMet.HasValue ||
                    !LaSoHopLe(KhoangCachMet.Value) ||
                    KhoangCachMet.Value < 0d)
                    return "—";

                double m = KhoangCachMet.Value;
                CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
                return m >= 1000d
                    ? (m / 1000d).ToString("N2", vi) + " km"
                    : m.ToString("N1", vi) + " m";
            }
        }

        public bool CoToaDoWgs84HopLe
        {
            get
            {
                if (!KinhDo.HasValue || !ViDo.HasValue)
                    return false;
                double x = KinhDo.Value;
                double y = ViDo.Value;
                return LaSoHopLe(x) && LaSoHopLe(y) &&
                    x >= -180d && x <= 180d &&
                    y >= -90d && y <= 90d &&
                    !(x == 0d && y == 0d);
            }
        }

        private static bool LaSoHopLe(double d)
        {
            return !double.IsNaN(d) && !double.IsInfinity(d);
        }
    }

    public sealed class SuKienKetQuaPhanTichArgs : EventArgs
    {
        public KetQuaPhanTichItem DoiTuong { get; private set; }

        public SuKienKetQuaPhanTichArgs(KetQuaPhanTichItem doiTuong)
        {
            if (doiTuong == null)
                throw new ArgumentNullException("doiTuong");
            DoiTuong = doiTuong;
        }
    }

    /// <summary>
    /// Giao dien phan tich khong gian - thiet ke v2 (13/11/2026).
    /// Luong 3 buoc: (1) chon don vi trung tam -> (2) chon phep phan tich
    /// (4 the: K gan nhat / trong ban kinh / vung dem / do) -> (3) tham so
    /// theo ngu canh + MOT nut chay duy nhat. Ben phai: dai tom tat 4 chi so
    /// va bang ket qua co o loc, trang thai trong, chan bang hanh dong.
    /// Chi gui su kien; khong truc tiep truy van SQL hay ve MapWinGIS.
    /// </summary>
    public class GiaoDienPhanTich : UserControl
    {
        // ----- Bang mau (dong bo voi cac man hinh khac) -----
        private static readonly Color Nen = Color.FromArgb(237, 247, 255);
        private static readonly Color Trang = Color.White;
        private static readonly Color Chu = Color.FromArgb(26, 53, 89);
        private static readonly Color ChuPhu = Color.FromArgb(103, 126, 156);
        private static readonly Color Vien = Color.FromArgb(220, 233, 247);
        private static readonly Color NenO = Color.FromArgb(247, 250, 255);
        private static readonly Color XanhLa = Color.FromArgb(0, 184, 132);
        private static readonly Color XanhDam = Color.FromArgb(0, 151, 135);
        private static readonly Color XanhLaNhat = Color.FromArgb(227, 248, 241);
        private static readonly Color XanhDuong = Color.FromArgb(45, 150, 240);
        private static readonly Color XanhDuongDam = Color.FromArgb(21, 105, 180);
        private static readonly Color XanhDuongNhat = Color.FromArgb(230, 242, 254);
        private static readonly Color Tim = Color.FromArgb(126, 90, 242);
        private static readonly Color Do = Color.FromArgb(205, 65, 84);
        private static readonly Color DoNhat = Color.FromArgb(255, 233, 238);
        private static readonly Color Cam = Color.FromArgb(222, 132, 20);
        private static readonly Color XamCham = Color.FromArgb(170, 187, 207);
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

        private static readonly decimal[] BanKinhNhanh = { 500m, 1000m, 2000m, 5000m };

        private readonly Panel pnlNguonNgoai;
        private FlowLayoutPanel thanhNguonTuFormMain;
        private readonly NumericUpDown nudBanKinh;
        private readonly NumericUpDown nudGioiHan;
        private NumericUpDown nudK;
        private readonly TheLuaChon btnK;
        private readonly TheLuaChon btnDo;
        private readonly TheLuaChon btnVungDem;
        private readonly TheLuaChon btnTimGan;
        private readonly Button btnChay;
        private readonly Button btnXoa;
        private readonly Button btnDinhVi;
        private readonly Button btnXuatCsv;
        private readonly Label lblDoiTuong;
        private readonly Label lblKhoangCach;
        private readonly Label lblVungDem;
        private readonly Label lblHuongDan;
        private readonly Label lblTrangThai;
        private readonly Label lblNguon;
        private readonly Label lblSoKetQua;
        private readonly Label lblChiTiet;
        private readonly Label lblTheTrangThai;
        private readonly Label lblTheDonVi;
        private readonly Label lblTheSoKetQua;
        private readonly Label lblTheBanKinh;
        private readonly TextBox txtLocKetQua;
        private readonly DataGridView dgvKetQua;
        private readonly Panel pnlTrong;
        private readonly Panel pnlBangDuLieu;
        private readonly Panel pnlSoLieu;
        private readonly ONhap oNguon;
        private readonly Panel oLamMoi;
        private readonly ONhap oK;
        private readonly Panel hangK;
        private readonly Panel hangBanKinh;
        private readonly FlowLayoutPanel hangChip;
        private readonly Panel hangGioiHan;
        private readonly Label hangGhiChuDo;
        private readonly List<Button> nutChip = new List<Button>();
        private readonly ToolTip goiY = new ToolTip();

        private readonly List<KetQuaPhanTichItem> ketQua =
            new List<KetQuaPhanTichItem>();
        private List<KetQuaPhanTichItem> hienThi =
            new List<KetQuaPhanTichItem>();

        private LoaiPhanTichKhongGian cheDo =
            LoaiPhanTichKhongGian.TaoVungDem;
        private bool cheDoK = true;
        private string maNguon = string.Empty;
        private string tenNguon = string.Empty;
        private bool dangXuLy;
        private bool dangGanBang;
        private bool daNhanDanhSach;
        private bool choPhepXuatCsv;
        private int soLanCapNhat;

        public event EventHandler<YeuCauPhanTichArgs>
            YeuCauThucHienPhanTich;
        public event EventHandler YeuCauXoaKetQua;
        public event EventHandler<SuKienKetQuaPhanTichArgs>
            YeuCauDinhViKetQua;
        public event EventHandler<SuKienKetQuaPhanTichArgs>
            YeuCauChonKetQua;
        /// <summary>Nut chay o che do "K don vi gan nhat" (FormMain xu ly KNN).</summary>
        public event EventHandler YeuCauTimKGanNhat;

        public GiaoDienPhanTich()
        {
            Name = "UcPhanTich";
            Dock = DockStyle.Fill;
            BackColor = Nen;
            ForeColor = Chu;
            Font = new Font("Segoe UI", 9F);
            DoubleBuffered = true;
            SuspendLayout();

            // =============================================
            // 1. KHUNG TRANG
            // =============================================
            Panel khung = new Panel
            {
                Name = "pnlPhanTichLight",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                AutoScrollMinSize = new Size(900, 600),
                BackColor = Nen,
                Padding = new Padding(12, 11, 12, 12)
            };

            TableLayoutPanel trang = BangTrong(1, 2, Nen);
            trang.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            trang.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            trang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            khung.Controls.Add(trang);
            Controls.Add(khung);

            GradientBanner banner = new GradientBanner
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 12),
                Radius = 16,
                MauDau = Color.FromArgb(0, 190, 146),
                MauGiua = Color.FromArgb(18, 157, 225),
                MauCuoi = Color.FromArgb(116, 85, 242),
                Buoc = new[] { "1  Chọn đơn vị", "2  Chọn phép phân tích", "3  Chạy" }
            };
            Label lblTieuDe = Nhan("◎  Phân tích không gian", 17F, true, Color.White);
            lblTieuDe.Dock = DockStyle.Left;
            lblTieuDe.Width = 360;
            lblTieuDe.Padding = new Padding(20, 0, 0, 0);
            lblTieuDe.BackColor = Color.Transparent;
            banner.Controls.Add(lblTieuDe);
            trang.Controls.Add(banner, 0, 0);

            TableLayoutPanel haiCot = BangTrong(2, 1, Nen);
            haiCot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 404F));
            haiCot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            haiCot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            trang.Controls.Add(haiCot, 0, 1);

            // =============================================
            // 2. COT TRAI: 3 BUOC THIET LAP
            // =============================================
            RoundPanel cardTrai = The(Trang, new Padding(18, 14, 18, 14));
            cardTrai.Margin = new Padding(0, 0, 12, 0);
            cardTrai.Dock = DockStyle.Fill;
            haiCot.Controls.Add(cardTrai, 0, 0);

            TableLayoutPanel trai = BangTrong(1, 12, Trang);
            trai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));   // 0 buoc 1
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));   // 1 chon nguon
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));   // 2 goi y nguon
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));   // 3
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));   // 4 buoc 2
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 126F));  // 5 4 the
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 10F));   // 6
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));   // 7 buoc 3
            trai.RowStyles.Add(new RowStyle(SizeType.AutoSize));        // 8 tham so
            trai.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // 9 so lieu
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));   // 10 nut chay
            trai.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));   // 11 huong dan
            cardTrai.Controls.Add(trai);

            trai.Controls.Add(new TieuDeBuoc(1, "Đơn vị trung tâm"), 0, 0);

            // Hang chon nguon: combo (tu FormMain) + nut lam moi (tu FormMain).
            pnlNguonNgoai = new Panel { Dock = DockStyle.Fill, BackColor = Trang, Margin = new Padding(0, 2, 0, 2) };
            oNguon = new ONhap { Dock = DockStyle.Fill, Padding = new Padding(8, 7, 6, 4) };
            oLamMoi = new Panel { Dock = DockStyle.Right, Width = 46, BackColor = Trang, Padding = new Padding(8, 0, 0, 0) };
            pnlNguonNgoai.Controls.Add(oNguon);
            pnlNguonNgoai.Controls.Add(oLamMoi);
            trai.Controls.Add(pnlNguonNgoai, 0, 1);

            lblDoiTuong = Nhan("Chưa chọn đơn vị trung tâm.", 8.3F, false, ChuPhu);
            lblDoiTuong.Dock = DockStyle.Fill;
            lblDoiTuong.Padding = new Padding(2, 0, 0, 0);
            trai.Controls.Add(lblDoiTuong, 0, 2);

            trai.Controls.Add(new TieuDeBuoc(2, "Phép phân tích"), 0, 4);

            TableLayoutPanel luoiThe = BangTrong(2, 2, Trang);
            luoiThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            luoiThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            luoiThe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            luoiThe.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            btnK = new TheLuaChon("K đơn vị gần nhất", "Xếp theo khoảng cách (KNN)");
            btnTimGan = new TheLuaChon("Trong bán kính", "Liệt kê đơn vị trong R mét");
            btnVungDem = new TheLuaChon("Vùng đệm", "Vẽ vùng R mét lên bản đồ");
            btnDo = new TheLuaChon("Đo khoảng cách", "Đo trực tiếp trên bản đồ");
            btnK.Margin = new Padding(0, 0, 4, 4);
            btnTimGan.Margin = new Padding(4, 0, 0, 4);
            btnVungDem.Margin = new Padding(0, 4, 4, 0);
            btnDo.Margin = new Padding(4, 4, 0, 0);
            luoiThe.Controls.Add(btnK, 0, 0);
            luoiThe.Controls.Add(btnTimGan, 1, 0);
            luoiThe.Controls.Add(btnVungDem, 0, 1);
            luoiThe.Controls.Add(btnDo, 1, 1);
            trai.Controls.Add(luoiThe, 0, 5);

            trai.Controls.Add(new TieuDeBuoc(3, "Tham số"), 0, 7);

            // Tham so theo ngu canh: chi hien dong can cho phep da chon.
            FlowLayoutPanel thamSo = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Trang,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            oK = new ONhap { Dock = DockStyle.Right, Width = 120, Padding = new Padding(8, 8, 6, 4) };
            hangK = HangThamSo("Số đơn vị gần nhất (K)", oK);

            nudBanKinh = So("nudBanKinh", 100000m, 1000m);
            nudBanKinh.Increment = 100m;
            // NumericUpDown dinh dang theo culture he thong (en-US: "2,000")
            // -> tat dau phan cach de khong lech voi nut chay ("2.000 M").
            nudBanKinh.ThousandsSeparator = false;
            hangBanKinh = HangThamSo("Bán kính (m)", BocO(nudBanKinh));

            hangChip = new FlowLayoutPanel
            {
                Height = 32,
                WrapContents = false,
                BackColor = Trang,
                Margin = new Padding(0, 0, 0, 4),
                Padding = new Padding(0, 2, 0, 0)
            };
            foreach (decimal r in BanKinhNhanh)
            {
                decimal giaTri = r;
                Button chip = Nut(r >= 1000m ? (r / 1000m).ToString("0.#", Vi) + " km" : r.ToString("0") + " m",
                    XanhDuongNhat, XanhDuongDam, 8.2F);
                chip.Size = new Size(66, 26);
                chip.Margin = new Padding(0, 0, 6, 0);
                chip.Tag = giaTri;
                chip.Click += (s, e) =>
                {
                    if (giaTri > nudBanKinh.Maximum) nudBanKinh.Maximum = giaTri;
                    nudBanKinh.Value = giaTri;
                };
                nutChip.Add(chip);
                hangChip.Controls.Add(chip);
            }

            nudGioiHan = So("nudGioiHan", 1000m, 20m);
            hangGioiHan = HangThamSo("Số kết quả tối đa", BocO(nudGioiHan));

            hangGhiChuDo = Nhan(
                "Không cần tham số. Bấm nút bên dưới, sau đó nhấp lần lượt các điểm trên bản đồ GIS; nhấp đúp để kết thúc.",
                8.6F, false, ChuPhu);
            hangGhiChuDo.Height = 48;
            hangGhiChuDo.TextAlign = ContentAlignment.TopLeft;
            hangGhiChuDo.Margin = new Padding(0, 2, 0, 4);

            thamSo.Controls.Add(hangK);
            thamSo.Controls.Add(hangBanKinh);
            thamSo.Controls.Add(hangChip);
            thamSo.Controls.Add(hangGioiHan);
            thamSo.Controls.Add(hangGhiChuDo);
            thamSo.SizeChanged += (s, e) =>
            {
                int w = Math.Max(200, thamSo.ClientSize.Width);
                foreach (Control c in thamSo.Controls) c.Width = w;
            };
            trai.Controls.Add(thamSo, 0, 8);

            // So lieu do / vung dem gan nhat (an khi chua co).
            Panel oSoLieu = new Panel { Dock = DockStyle.Fill, BackColor = Trang, Padding = new Padding(0, 8, 0, 8) };
            RoundPanel soLieu = The(XanhDuongNhat, new Padding(12, 6, 10, 6));
            soLieu.Dock = DockStyle.Top;
            soLieu.Height = 56;
            lblKhoangCach = Nhan("", 8.6F, false, Chu);
            lblVungDem = Nhan("", 8.6F, false, Chu);
            lblKhoangCach.Dock = DockStyle.Top;
            lblKhoangCach.Height = 22;
            lblVungDem.Dock = DockStyle.Top;
            lblVungDem.Height = 22;
            soLieu.Controls.Add(lblVungDem);
            soLieu.Controls.Add(lblKhoangCach);
            oSoLieu.Controls.Add(soLieu);
            pnlSoLieu = soLieu;
            trai.Controls.Add(oSoLieu, 0, 9);

            btnChay = Nut("▶  CHẠY PHÂN TÍCH", XanhDam, Color.White, 10F);
            btnChay.Dock = DockStyle.Fill;
            btnChay.Margin = new Padding(0, 2, 0, 4);
            trai.Controls.Add(btnChay, 0, 10);

            lblHuongDan = Nhan("", 8.3F, false, ChuPhu);
            lblHuongDan.Dock = DockStyle.Fill;
            lblHuongDan.TextAlign = ContentAlignment.MiddleCenter;
            trai.Controls.Add(lblHuongDan, 0, 11);

            // =============================================
            // 3. COT PHAI: TOM TAT + BANG KET QUA
            // =============================================
            TableLayoutPanel phai = BangTrong(1, 2, Nen);
            phai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 84F));
            phai.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            haiCot.Controls.Add(phai, 1, 0);

            RoundPanel tomTat = The(Trang, new Padding(6, 12, 6, 10));
            tomTat.Dock = DockStyle.Fill;
            tomTat.Margin = new Padding(0, 0, 0, 12);
            TableLayoutPanel chiSo = BangTrong(4, 1, Trang);
            for (int i = 0; i < 4; i++)
                chiSo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            chiSo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            chiSo.CellPaint += (s, e) =>
            {
                if (e.Column == 0) return;
                using (Pen p = new Pen(Vien))
                    e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top + 4,
                        e.CellBounds.Left, e.CellBounds.Bottom - 4);
            };
            lblTheTrangThai = ChiSo(chiSo, 0, "TRẠNG THÁI", "● Chưa chạy");
            lblTheDonVi = ChiSo(chiSo, 1, "ĐƠN VỊ TRUNG TÂM", "Chưa chọn");
            lblTheSoKetQua = ChiSo(chiSo, 2, "KẾT QUẢ", "—");
            lblTheBanKinh = ChiSo(chiSo, 3, "THAM SỐ", "—");
            lblTheTrangThai.ForeColor = XamCham;
            tomTat.Controls.Add(chiSo);
            phai.Controls.Add(tomTat, 0, 0);

            RoundPanel cardPhai = The(Trang, new Padding(16, 12, 16, 12));
            cardPhai.Dock = DockStyle.Fill;
            phai.Controls.Add(cardPhai, 0, 1);

            // Dau bang: tieu de + so luong + o loc.
            TableLayoutPanel dauBang = BangTrong(4, 1, Trang);
            dauBang.Dock = DockStyle.Top;
            dauBang.Height = 48;
            dauBang.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            dauBang.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            dauBang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            dauBang.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 270F));
            dauBang.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Label lblTieuDeBang = Nhan("Kết quả phân tích", 12F, true, Chu);
            lblTieuDeBang.AutoSize = true;
            lblTieuDeBang.Anchor = AnchorStyles.Left;
            lblTieuDeBang.Margin = new Padding(0, 0, 8, 6);
            dauBang.Controls.Add(lblTieuDeBang, 0, 0);

            RoundPanel huyHieu = The(XanhDuongNhat, new Padding(10, 3, 10, 3));
            huyHieu.Radius = 11;
            huyHieu.Size = new Size(64, 24);
            huyHieu.Anchor = AnchorStyles.Left;
            huyHieu.Margin = new Padding(0, 0, 0, 6);
            lblSoKetQua = Nhan("0", 8.5F, true, XanhDuongDam);
            lblSoKetQua.Dock = DockStyle.Fill;
            lblSoKetQua.TextAlign = ContentAlignment.MiddleCenter;
            huyHieu.Controls.Add(lblSoKetQua);
            dauBang.Controls.Add(huyHieu, 1, 0);

            ONhap oLoc = new ONhap { Dock = DockStyle.Fill, Padding = new Padding(8, 8, 8, 4), Margin = new Padding(0, 2, 0, 8) };
            Label kinhLup = Nhan("⌕", 11F, true, XamCham);
            kinhLup.Dock = DockStyle.Left;
            kinhLup.Width = 22;
            kinhLup.TextAlign = ContentAlignment.TopLeft;
            kinhLup.Padding = new Padding(0, 0, 0, 0);
            txtLocKetQua = new TextBox
            {
                Name = "txtLocKetQua",
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = NenO,
                ForeColor = Chu,
                Font = new Font("Segoe UI", 9.5F)
            };
            txtLocKetQua.HandleCreated += (s, e) =>
                SendMessage(txtLocKetQua.Handle, EM_SETCUEBANNER, (IntPtr)1,
                    "Lọc theo tên hoặc mã đơn vị…");
            oLoc.Controls.Add(txtLocKetQua);
            oLoc.Controls.Add(kinhLup);
            dauBang.Controls.Add(oLoc, 3, 0);

            // Bang ket qua.
            dgvKetQua = new DataGridView
            {
                Name = "dgvKetQua",
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Trang,
                GridColor = Color.FromArgb(236, 243, 251),
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            };
            dgvKetQua.RowTemplate.Height = 36;
            dgvKetQua.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvKetQua.ColumnHeadersHeight = 36;
            dgvKetQua.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 247, 254);
            dgvKetQua.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(241, 247, 254);
            dgvKetQua.ColumnHeadersDefaultCellStyle.ForeColor = ChuPhu;
            dgvKetQua.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvKetQua.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
            dgvKetQua.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgvKetQua.DefaultCellStyle.BackColor = Trang;
            dgvKetQua.DefaultCellStyle.ForeColor = Chu;
            dgvKetQua.DefaultCellStyle.Font = new Font("Segoe UI", 9.2F);
            dgvKetQua.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgvKetQua.DefaultCellStyle.SelectionBackColor = XanhLaNhat;
            dgvKetQua.DefaultCellStyle.SelectionForeColor = Chu;
            dgvKetQua.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255);

            DataGridViewTextBoxColumn cotStt = Cot("#", null, 6F);
            cotStt.Name = "colSTT";
            cotStt.MinimumWidth = 38;
            cotStt.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
            cotStt.DefaultCellStyle.ForeColor = ChuPhu;
            dgvKetQua.Columns.Add(cotStt);
            dgvKetQua.Columns.Add(Cot("Tên đơn vị", "TenDoiTuong", 37F));
            DataGridViewTextBoxColumn cotMa = Cot("Mã", "MaDoiTuong", 19F);
            cotMa.DefaultCellStyle.ForeColor = ChuPhu;
            cotMa.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            dgvKetQua.Columns.Add(cotMa);
            DataGridViewTextBoxColumn cotLop = Cot("Quân chủng", "TenLop", 18F);
            cotLop.DefaultCellStyle.ForeColor = Tim;
            cotLop.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            dgvKetQua.Columns.Add(cotLop);
            DataGridViewTextBoxColumn cotKc = Cot("Khoảng cách", "KhoangCachHienThi", 20F);
            cotKc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            cotKc.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            cotKc.DefaultCellStyle.ForeColor = XanhDam;
            cotKc.DefaultCellStyle.SelectionForeColor = XanhDam;
            cotKc.DefaultCellStyle.Font = new Font("Segoe UI", 9.2F, FontStyle.Bold);
            dgvKetQua.Columns.Add(cotKc);
            dgvKetQua.CellFormatting += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 0)
                {
                    e.Value = (e.RowIndex + 1).ToString(Vi);
                    e.FormattingApplied = true;
                }
            };

            // Trang thai trong.
            pnlTrong = new Panel { Dock = DockStyle.Fill, BackColor = Trang };
            TableLayoutPanel manHinhTrong = BangTrong(1, 5, Trang);
            manHinhTrong.Dock = DockStyle.Fill;
            manHinhTrong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            manHinhTrong.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            manHinhTrong.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));
            manHinhTrong.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            manHinhTrong.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            manHinhTrong.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            RoundPanel vong = The(XanhDuongNhat, Padding.Empty);
            vong.Radius = 32;
            vong.Size = new Size(64, 64);
            vong.Anchor = AnchorStyles.None;
            Label lblVong = Nhan("◎", 22F, true, XanhDuong);
            lblVong.Dock = DockStyle.Fill;
            lblVong.TextAlign = ContentAlignment.MiddleCenter;
            vong.Controls.Add(lblVong);
            Label lblTrongTen = Nhan("Chưa có kết quả", 12F, true, Chu);
            lblTrongTen.TextAlign = ContentAlignment.MiddleCenter;
            lblTrongTen.Dock = DockStyle.Fill;
            Label lblTrongPhu = Nhan(
                "Chọn đơn vị trung tâm, chọn phép phân tích rồi bấm nút chạy ở cột bên trái.",
                9F, false, ChuPhu);
            lblTrongPhu.Dock = DockStyle.Fill;
            lblTrongPhu.TextAlign = ContentAlignment.TopCenter;
            manHinhTrong.Controls.Add(vong, 0, 1);
            manHinhTrong.Controls.Add(lblTrongTen, 0, 2);
            manHinhTrong.Controls.Add(lblTrongPhu, 0, 3);
            pnlTrong.Controls.Add(manHinhTrong);

            pnlBangDuLieu = new Panel { Dock = DockStyle.Fill, BackColor = Trang };
            pnlBangDuLieu.Controls.Add(dgvKetQua);
            pnlBangDuLieu.Controls.Add(pnlTrong);
            pnlTrong.BringToFront();

            // Chan bang: dong chi tiet + trang thai (ca chieu ngang), duoi la hang nut.
            Panel chanBang = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 94,
                BackColor = Trang,
                Padding = new Padding(0, 10, 0, 0)
            };
            chanBang.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Vien))
                    e.Graphics.DrawLine(p, 0, 2, chanBang.Width, 2);
            };
            FlowLayoutPanel hanhDong = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 0),
                BackColor = Trang
            };
            btnDinhVi = Nut("⌖  ĐỊNH VỊ TRÊN BẢN ĐỒ", XanhLa, Color.White, 8.4F);
            btnXuatCsv = Nut("↓  XUẤT CSV", XanhDuongNhat, XanhDuongDam, 8.4F);
            btnXoa = Nut("✕  XÓA", DoNhat, Do, 8.4F);
            btnDinhVi.Size = new Size(186, 36);
            btnXuatCsv.Size = new Size(112, 36);
            btnXoa.Size = new Size(86, 36);
            btnDinhVi.Margin = new Padding(0);
            btnXuatCsv.Margin = new Padding(0, 0, 8, 0);
            btnXoa.Margin = new Padding(0, 0, 8, 0);
            hanhDong.Controls.Add(btnDinhVi);
            hanhDong.Controls.Add(btnXuatCsv);
            hanhDong.Controls.Add(btnXoa);

            Panel thongTinChan = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Trang };
            lblChiTiet = Nhan("Chọn một dòng để xem thông tin.", 9F, true, Chu);
            lblChiTiet.Dock = DockStyle.Top;
            lblChiTiet.Height = 22;
            lblTrangThai = Nhan("Sẵn sàng.", 8.2F, false, ChuPhu);
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.TopLeft;
            lblNguon = Nhan("", 8F, false, XamCham);
            lblNguon.Dock = DockStyle.Right;
            lblNguon.Width = 190;
            lblNguon.TextAlign = ContentAlignment.TopRight;
            Panel dongTrangThai = new Panel { Dock = DockStyle.Fill, BackColor = Trang };
            dongTrangThai.Controls.Add(lblTrangThai);
            dongTrangThai.Controls.Add(lblNguon);
            thongTinChan.Controls.Add(dongTrangThai);
            thongTinChan.Controls.Add(lblChiTiet);

            // WinForms neo (dock) tu control co chi so CAO nhat ve 0:
            // khung Top/Bottom phai co chi so cao hon khung Fill, neu khong
            // khung Fill chiem het va bi Top/Bottom de len (mat hang tieu de cot).
            chanBang.Controls.Add(hanhDong);
            chanBang.Controls.Add(thongTinChan);

            cardPhai.Controls.Add(pnlBangDuLieu);
            cardPhai.Controls.Add(chanBang);
            cardPhai.Controls.Add(dauBang);
            cardPhai.Controls.SetChildIndex(pnlBangDuLieu, 0);
            cardPhai.Controls.SetChildIndex(chanBang, 1);
            cardPhai.Controls.SetChildIndex(dauBang, 2);

            // =============================================
            // 4. SU KIEN GIAO DIEN (du lieu thuc do FormMain xu ly)
            // =============================================
            btnK.Click += (s, e) => ChonCheDoK();
            btnTimGan.Click += (s, e) => ChonCheDo(LoaiPhanTichKhongGian.TimDoiTuongGan);
            btnVungDem.Click += (s, e) => ChonCheDo(LoaiPhanTichKhongGian.TaoVungDem);
            btnDo.Click += (s, e) => ChonCheDo(LoaiPhanTichKhongGian.DoKhoangCach);
            btnChay.Click += (s, e) => GuiYeuCauHienTai();
            btnXoa.Click += BtnXoa_Click;
            btnDinhVi.Click += (s, e) => GuiYeuCauDinhVi();
            btnXuatCsv.Click += (s, e) => XuatCsv();
            nudBanKinh.ValueChanged += (s, e) => CapNhatGiaoDienCheDo();
            nudGioiHan.ValueChanged += (s, e) => CapNhatGiaoDienCheDo();
            txtLocKetQua.TextChanged += (s, e) => LocKetQua(null);
            dgvKetQua.SelectionChanged += (s, e) =>
            {
                if (!dangGanBang) CapNhatChiTiet();
            };
            dgvKetQua.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) GuiYeuCauDinhVi();
            };
            goiY.SetToolTip(btnK, "Tìm K đơn vị gần đơn vị trung tâm nhất (PostGIS KNN <->).");
            goiY.SetToolTip(btnTimGan, "Liệt kê đơn vị nằm trong bán kính R (ST_DWithin).");
            goiY.SetToolTip(btnVungDem, "Tạo vùng đệm R mét và vẽ lên bản đồ (ST_Buffer).");
            goiY.SetToolTip(btnDo, "Mở công cụ đo khoảng cách trên bản đồ GIS.");

            ChonCheDoK();
            XoaKetQuaHienThi();
            ResumeLayout(true);
        }

        // =================================================
        // NHAN THANH CHON NGUON TU FORMMAIN
        // =================================================

        // FormMain.GanXuLyPhanTichGon tao 1 FlowLayoutPanel gom: combo don vi,
        // o so K, nut "Tim K gan nhat", nut "Lam moi nguon" roi Add vao day.
        // Ta TACH tung control ra dat dung cho trong bo cuc 3 buoc; giu nguyen
        // doi tuong (FormMain van bat/tat Enabled va gan su kien tren chung).
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            if (e.Control == null || pnlNguonNgoai == null ||
                e.Control.Parent != this)
                return;

            FlowLayoutPanel thanh = e.Control as FlowLayoutPanel;
            if (thanh == null ||
                thanh.Controls.Find("cboNguonPhanTich", true).Length == 0)
                return;

            thanhNguonTuFormMain = thanh;
            ComboBox cbo = thanh.Controls.OfType<ComboBox>().FirstOrDefault();
            NumericUpDown soK = thanh.Controls.OfType<NumericUpDown>().FirstOrDefault();
            Button[] nut = thanh.Controls.OfType<Button>().ToArray();

            if (cbo != null)
            {
                cbo.Dock = DockStyle.Fill;
                cbo.Margin = Padding.Empty;
                cbo.FlatStyle = FlatStyle.Flat;
                cbo.BackColor = NenO;
                cbo.ForeColor = Chu;
                cbo.Font = new Font("Segoe UI", 9.6F);
                oNguon.Controls.Add(cbo);
            }
            if (soK != null)
            {
                nudK = soK;
                soK.Dock = DockStyle.Fill;
                soK.Margin = Padding.Empty;
                soK.BorderStyle = BorderStyle.None;
                soK.BackColor = NenO;
                soK.ForeColor = Chu;
                soK.TextAlign = HorizontalAlignment.Right;
                soK.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                oK.Controls.Add(soK);
                soK.ValueChanged += (s, ev) => CapNhatGiaoDienCheDo();
            }
            // nut[0] = "TIM K GAN NHAT": thay bang nut chay chung -> an di.
            // nut[1] = "LAM MOI NGUON": thu gon thanh nut bieu tuong canh combo.
            if (nut.Length >= 2)
            {
                Button lamMoi = nut[1];
                lamMoi.Text = "↻";
                lamMoi.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                lamMoi.Dock = DockStyle.Fill;
                lamMoi.Margin = Padding.Empty;
                DoiMauNut(lamMoi, XanhLaNhat, XanhDam);
                lamMoi.SizeChanged += (s, ev) => BoTron(lamMoi, 10);
                goiY.SetToolTip(lamMoi, "Làm mới danh sách đơn vị nguồn");
                oLamMoi.Controls.Add(lamMoi);
            }
            thanh.Visible = false;
            thanh.Dock = DockStyle.None;
            thanh.Size = new Size(1, 1);
            CapNhatGiaoDienCheDo();
        }

        // =================================================
        // CHE DO PHAN TICH
        // =================================================

        private void ChonCheDoK()
        {
            cheDoK = true;
            CapNhatGiaoDienCheDo();
        }

        private void ChonCheDo(LoaiPhanTichKhongGian loai)
        {
            cheDoK = false;
            cheDo = loai;
            CapNhatGiaoDienCheDo();
        }

        private void CapNhatGiaoDienCheDo()
        {
            if (btnK == null || hangK == null) return;
            bool laDo = !cheDoK && cheDo == LoaiPhanTichKhongGian.DoKhoangCach;
            bool laBanKinh = !cheDoK && !laDo;

            btnK.DangChon = cheDoK;
            btnTimGan.DangChon = !cheDoK && cheDo == LoaiPhanTichKhongGian.TimDoiTuongGan;
            btnVungDem.DangChon = !cheDoK && cheDo == LoaiPhanTichKhongGian.TaoVungDem;
            btnDo.DangChon = laDo;

            hangK.Visible = cheDoK;
            hangBanKinh.Visible = laBanKinh;
            hangChip.Visible = laBanKinh;
            hangGioiHan.Visible = laBanKinh;
            hangGhiChuDo.Visible = laDo;

            nudBanKinh.Enabled = laBanKinh && !dangXuLy;
            nudGioiHan.Enabled = laBanKinh && !dangXuLy;
            foreach (Button chip in nutChip)
            {
                bool chon = chip.Tag is decimal && (decimal)chip.Tag == nudBanKinh.Value;
                DoiMauNut(chip, chon ? XanhDuong : XanhDuongNhat,
                    chon ? Color.White : XanhDuongDam);
                chip.Enabled = laBanKinh && !dangXuLy;
            }

            int k = nudK != null ? decimal.ToInt32(nudK.Value) : 10;
            string r = nudBanKinh.Value.ToString("N0", Vi);
            if (cheDoK)
            {
                btnChay.Text = "▶  TÌM " + k + " ĐƠN VỊ GẦN NHẤT";
                lblHuongDan.Text = "Kết quả xếp từ gần đến xa và được vẽ lên bản đồ.";
                lblTheBanKinh.Text = "K = " + k;
            }
            else if (cheDo == LoaiPhanTichKhongGian.TimDoiTuongGan)
            {
                btnChay.Text = "⌕  TÌM TRONG BÁN KÍNH " + r + " M";
                lblHuongDan.Text = "Tối đa " + nudGioiHan.Value.ToString("N0", Vi) + " đơn vị, xếp theo khoảng cách.";
                lblTheBanKinh.Text = "R = " + r + " m";
            }
            else if (cheDo == LoaiPhanTichKhongGian.TaoVungDem)
            {
                btnChay.Text = "◆  TẠO VÙNG ĐỆM " + r + " M";
                lblHuongDan.Text = "Vùng đệm và các đơn vị bên trong sẽ hiện trên bản đồ.";
                lblTheBanKinh.Text = "R = " + r + " m";
            }
            else
            {
                btnChay.Text = "⌖  MỞ CÔNG CỤ ĐO TRÊN BẢN ĐỒ";
                lblHuongDan.Text = "Ứng dụng sẽ chuyển sang trang Bản đồ GIS.";
                lblTheBanKinh.Text = "—";
            }
            // O so lieu chi cao bang so dong dang co noi dung.
            // (Khong doc lai .Visible: khi khung cha dang an, getter luon tra false.)
            bool coKc = lblKhoangCach.Text.Length > 0;
            bool coVd = lblVungDem.Text.Length > 0;
            lblKhoangCach.Visible = coKc;
            lblVungDem.Visible = coVd;
            int soDong = (coKc ? 1 : 0) + (coVd ? 1 : 0);
            pnlSoLieu.Visible = soDong > 0;
            if (soDong > 0) pnlSoLieu.Height = 14 + 22 * soDong;
        }

        // =================================================
        // API CONG KHAI (FormMain goi)
        // =================================================

        public string MaDoiTuongDangChon
        {
            get { return maNguon; }
        }

        public KetQuaPhanTichItem KetQuaDangChon
        {
            get { return DoiTuongKetQuaDangChon(); }
        }

        public bool DaNhanKetQuaTimGan
        {
            get { return daNhanDanhSach; }
        }

        public bool ChoPhepXuatCsv
        {
            get { return choPhepXuatCsv; }
            set
            {
                choPhepXuatCsv = value;
                CapNhatNut();
            }
        }

        public void GanDoiTuongDangChon(string ma, string ten)
        {
            maNguon = (ma ?? string.Empty).Trim();
            tenNguon = (ten ?? string.Empty).Trim();
            lblDoiTuong.Text = maNguon.Length == 0
                ? "Chưa chọn đơn vị trung tâm."
                : "Mã " + maNguon + " · kết quả sẽ không tính chính đơn vị này";
            lblTheDonVi.Text = maNguon.Length == 0
                ? "Chưa chọn"
                : string.IsNullOrWhiteSpace(tenNguon) ? maNguon : tenNguon;
        }

        public void DatTrangThaiDangXuLy(bool dangThucHien)
        {
            dangXuLy = dangThucHien;
            btnK.Enabled = !dangXuLy;
            btnDo.Enabled = !dangXuLy;
            btnVungDem.Enabled = !dangXuLy;
            btnTimGan.Enabled = !dangXuLy;
            btnChay.Enabled = !dangXuLy;
            btnXoa.Enabled = !dangXuLy;
            if (dangXuLy)
                DatTrangThai("Đang xử lý…", Cam);
            else if (daNhanDanhSach)
                DatTrangThai("Hoàn thành", XanhLa);
            else
                DatTrangThai("Chưa chạy", XamCham);
            CapNhatGiaoDienCheDo();
            CapNhatNut();
            if (dangXuLy)
                CapNhatThongBao("Đang chờ kết quả phân tích từ PostGIS…");
        }

        public void CapNhatThongBao(string thongBao)
        {
            lblTrangThai.Text = string.IsNullOrWhiteSpace(thongBao)
                ? "Sẵn sàng."
                : thongBao.Trim().Replace('\r', ' ').Replace('\n', ' ');
        }

        public void CapNhatKetQuaDo(double? khoangCachMet)
        {
            soLanCapNhat++;
            DatTrangThaiDangXuLy(false);
            if (!khoangCachMet.HasValue ||
                !HopLe(khoangCachMet.Value) ||
                khoangCachMet.Value < 0)
            {
                lblKhoangCach.Text = string.Empty;
                CapNhatGiaoDienCheDo();
                return;
            }
            string kc = DinhDangKhoangCach(khoangCachMet.Value);
            lblKhoangCach.Text = "⌖  Khoảng cách đo: " + kc;
            lblNguon.Text = "Nguồn: MapWinGIS";
            lblTheSoKetQua.Text = kc;
            DatTrangThai("Đã đo", XanhLa);
            CapNhatGiaoDienCheDo();
        }

        public void CapNhatKetQuaVungDem(
            double? dienTichM2, int? soDoiTuong)
        {
            soLanCapNhat++;
            DatTrangThaiDangXuLy(false);
            string soDv = soDoiTuong.HasValue ? soDoiTuong.Value + " đơn vị" : null;
            if (!dienTichM2.HasValue ||
                !HopLe(dienTichM2.Value) ||
                dienTichM2.Value < 0d)
            {
                lblVungDem.Text = soDv == null
                    ? string.Empty
                    : "◆  Vùng đệm: " + soDv + "; chưa tính được diện tích.";
            }
            else
            {
                double m2 = dienTichM2.Value;
                string dt = m2 >= 1e6
                    ? (m2 / 1e6).ToString("N2", Vi) + " km²"
                    : m2.ToString("N0", Vi) + " m²";
                lblVungDem.Text = "◆  Vùng đệm " + nudBanKinh.Value.ToString("N0", Vi) +
                    " m: " + dt + (soDv == null ? string.Empty : " · " + soDv);
                lblNguon.Text = "Nguồn: PostGIS";
            }
            if (soDv != null) lblTheSoKetQua.Text = soDv;
            CapNhatGiaoDienCheDo();
        }

        public void NapKetQuaTimGan(
            IEnumerable<KetQuaPhanTichItem> duLieu,
            string nguon = null)
        {
            ketQua.Clear();
            if (duLieu != null)
                ketQua.AddRange(duLieu.Where(x => x != null));
            ketQua.Sort((a, b) =>
            {
                double x = a.KhoangCachMet.HasValue &&
                    HopLe(a.KhoangCachMet.Value)
                    ? a.KhoangCachMet.Value : double.MaxValue;
                double y = b.KhoangCachMet.HasValue &&
                    HopLe(b.KhoangCachMet.Value)
                    ? b.KhoangCachMet.Value : double.MaxValue;
                int k = x.CompareTo(y);
                return k != 0 ? k : string.Compare(a.TenDoiTuong,
                    b.TenDoiTuong,
                    StringComparison.CurrentCultureIgnoreCase);
            });
            daNhanDanhSach = true;
            soLanCapNhat++;
            DatTrangThaiDangXuLy(false);
            lblNguon.Text = string.IsNullOrWhiteSpace(nguon)
                ? "Nguồn: BLL/PostGIS" : "Nguồn: " + nguon.Trim();
            LocKetQua(null);
            CapNhatThongBao("Đã nhận " + ketQua.Count +
                " kết quả. Chọn một dòng để xem chi tiết; nhấp đúp để định vị.");
        }

        public void XoaKetQuaHienThi()
        {
            soLanCapNhat++;
            ketQua.Clear();
            hienThi.Clear();
            daNhanDanhSach = false;
            DatTrangThaiDangXuLy(false);
            dangGanBang = true;
            try
            {
                dgvKetQua.DataSource = null;
                txtLocKetQua.Clear();
            }
            finally
            {
                dangGanBang = false;
            }
            lblKhoangCach.Text = string.Empty;
            lblVungDem.Text = string.Empty;
            lblSoKetQua.Text = "0";
            lblTheSoKetQua.Text = "—";
            DatTrangThai("Chưa chạy", XamCham);
            lblChiTiet.Text = "Chọn một dòng để xem thông tin.";
            lblNguon.Text = string.Empty;
            pnlTrong.Visible = true;
            dgvKetQua.Visible = false;
            pnlTrong.BringToFront();
            CapNhatGiaoDienCheDo();
            CapNhatNut();
            CapNhatThongBao("Sẵn sàng.");
        }

        // =================================================
        // BANG KET QUA
        // =================================================

        private void LocKetQua(string maCanChon)
        {
            if (dangGanBang) return;
            string q = ChuanHoa(txtLocKetQua.Text);
            hienThi = ketQua.Where(x =>
                q.Length == 0 ||
                ChuanHoa((x.MaDoiTuong ?? "") + " " +
                    (x.TenDoiTuong ?? "") + " " +
                    (x.TenLop ?? "") + " " +
                    (x.GhiChu ?? "")).Contains(q)).ToList();
            dangGanBang = true;
            try
            {
                dgvKetQua.DataSource = null;
                dgvKetQua.DataSource =
                    new BindingList<KetQuaPhanTichItem>(hienThi);
                dgvKetQua.ClearSelection();
                dgvKetQua.CurrentCell = null;
                if (hienThi.Count > 0)
                {
                    int index = hienThi.FindIndex(x =>
                        string.Equals(x.MaDoiTuong, maCanChon,
                            StringComparison.OrdinalIgnoreCase));
                    if (index < 0) index = 0;
                    dgvKetQua.CurrentCell = dgvKetQua.Rows[index].Cells[1];
                    dgvKetQua.Rows[index].Selected = true;
                }
            }
            finally
            {
                dangGanBang = false;
            }
            lblSoKetQua.Text = !daNhanDanhSach
                ? "0"
                : hienThi.Count == ketQua.Count
                    ? ketQua.Count.ToString(Vi)
                    : hienThi.Count + "/" + ketQua.Count;
            if (daNhanDanhSach)
                lblTheSoKetQua.Text = hienThi.Count.ToString("N0", Vi) + " đơn vị";
            pnlTrong.Visible = hienThi.Count == 0;
            dgvKetQua.Visible = hienThi.Count > 0;
            if (pnlTrong.Visible) pnlTrong.BringToFront();
            if (dgvKetQua.Visible) dgvKetQua.BringToFront();
            CapNhatChiTiet();
        }

        private KetQuaPhanTichItem DoiTuongKetQuaDangChon()
        {
            return dgvKetQua.CurrentRow != null &&
                dgvKetQua.CurrentRow.Selected
                ? dgvKetQua.CurrentRow.DataBoundItem as KetQuaPhanTichItem
                : null;
        }

        private void CapNhatChiTiet()
        {
            KetQuaPhanTichItem item = DoiTuongKetQuaDangChon();
            lblChiTiet.Text = item == null
                ? "Chọn một dòng để xem thông tin."
                : "Đã chọn: " + (item.TenDoiTuong ?? "—") +
                  "  ·  " + item.KhoangCachHienThi;
            CapNhatNut();
            EventHandler<SuKienKetQuaPhanTichArgs> handler =
                YeuCauChonKetQua;
            if (handler != null && item != null &&
                item.CoToaDoWgs84HopLe)
                handler(this, new SuKienKetQuaPhanTichArgs(item));
        }

        private void CapNhatNut()
        {
            if (btnDinhVi == null || btnXuatCsv == null)
                return;
            KetQuaPhanTichItem item = DoiTuongKetQuaDangChon();
            btnDinhVi.Enabled = item != null &&
                item.CoToaDoWgs84HopLe && !dangXuLy;
            btnXuatCsv.Enabled = choPhepXuatCsv &&
                daNhanDanhSach && hienThi.Count > 0 && !dangXuLy;
            btnXoa.Enabled = !dangXuLy;
            LamMoNutTat(btnDinhVi, XanhLa, Color.White);
            LamMoNutTat(btnXuatCsv, XanhDuongNhat, XanhDuongDam);
            LamMoNutTat(btnXoa, DoNhat, Do);
        }

        // Nut phang WinForms khong tu doi mau khi Enabled=false -> lam nhat.
        private static void LamMoNutTat(Button b, Color nen, Color chu)
        {
            if (b.Enabled) DoiMauNut(b, nen, chu);
            else DoiMauNut(b, Color.FromArgb(240, 244, 249), Color.FromArgb(165, 180, 198));
        }

        // =================================================
        // GUI YEU CAU
        // =================================================

        private void GuiYeuCauHienTai()
        {
            if (dangXuLy) return;
            if (!cheDoK)
            {
                GuiYeuCau(cheDo);
                return;
            }
            if (maNguon.Length == 0)
            {
                MessageBox.Show(this, "Hãy chọn đơn vị trung tâm trước.",
                    "Thiếu đơn vị nguồn", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            EventHandler handler = YeuCauTimKGanNhat;
            if (handler == null)
            {
                ThongBaoChuaTichHop("tìm K đơn vị gần nhất");
                return;
            }
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                DatTrangThaiDangXuLy(false);
                MessageBox.Show(this, ex.Message, "Lỗi phân tích",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuiYeuCauDinhVi()
        {
            KetQuaPhanTichItem item = DoiTuongKetQuaDangChon();
            if (item == null || !item.CoToaDoWgs84HopLe)
                return;
            EventHandler<SuKienKetQuaPhanTichArgs> handler =
                YeuCauDinhViKetQua;
            if (handler == null)
            {
                ThongBaoChuaTichHop("định vị trên bản đồ");
                return;
            }
            try
            {
                handler(this, new SuKienKetQuaPhanTichArgs(item));
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi định vị",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GuiYeuCau(LoaiPhanTichKhongGian loai)
        {
            if (dangXuLy) return;
            if (loai != LoaiPhanTichKhongGian.DoKhoangCach &&
                maNguon.Length == 0)
            {
                MessageBox.Show(this, "Hãy chọn đơn vị trung tâm trước.",
                    "Thiếu đơn vị nguồn", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            EventHandler<YeuCauPhanTichArgs> handler =
                YeuCauThucHienPhanTich;
            if (handler == null)
            {
                ThongBaoChuaTichHop("phân tích không gian");
                return;
            }
            try
            {
                int truoc = soLanCapNhat;
                handler(this, new YeuCauPhanTichArgs(loai,
                    maNguon, nudBanKinh.Value,
                    decimal.ToInt32(nudGioiHan.Value)));
                if (truoc == soLanCapNhat && !dangXuLy)
                    CapNhatThongBao(loai == LoaiPhanTichKhongGian.DoKhoangCach
                        ? "Đã mở công cụ đo trên bản đồ GIS."
                        : "Đã gửi yêu cầu; đang chờ kết quả.");
            }
            catch (Exception ex)
            {
                DatTrangThaiDangXuLy(false);
                MessageBox.Show(this, ex.Message, "Lỗi phân tích",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            EventHandler handler = YeuCauXoaKetQua;
            if (handler == null)
            {
                ThongBaoChuaTichHop("xóa kết quả");
                return;
            }
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message,
                    "Lỗi xóa kết quả", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void XuatCsv()
        {
            if (!choPhepXuatCsv || !daNhanDanhSach ||
                hienThi.Count == 0)
                return;
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Title = "Xuất kết quả phân tích";
                dialog.Filter = "Tệp CSV (*.csv)|*.csv";
                dialog.DefaultExt = "csv";
                dialog.AddExtension = true;
                dialog.FileName = "KetQuaPhanTich.csv";
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;
                try
                {
                    using (StreamWriter writer = new StreamWriter(
                        dialog.FileName, false, new UTF8Encoding(true)))
                    {
                        writer.WriteLine(
                            "STT,Mã đơn vị,Tên đơn vị,Lớp dữ liệu,Khoảng cách (m),Ghi chú");
                        int stt = 0;
                        foreach (KetQuaPhanTichItem item in hienThi)
                        {
                            stt++;
                            writer.WriteLine(string.Join(",", new string[]
                            {
                                stt.ToString(CultureInfo.InvariantCulture),
                                Csv(item.MaDoiTuong),
                                Csv(item.TenDoiTuong),
                                Csv(item.TenLop),
                                Csv(item.KhoangCachMet.HasValue &&
                                    HopLe(item.KhoangCachMet.Value)
                                    ? item.KhoangCachMet.Value.ToString(
                                        "0.###", CultureInfo.InvariantCulture)
                                    : string.Empty),
                                Csv(item.GhiChu)
                            }));
                        }
                    }
                    CapNhatThongBao("Đã xuất " + hienThi.Count +
                        " kết quả ra CSV.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Lỗi xuất CSV",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DatTrangThai(string chu, Color mau)
        {
            if (lblTheTrangThai == null) return;
            lblTheTrangThai.Text = "● " + chu;
            lblTheTrangThai.ForeColor = mau;
        }

        // =================================================
        // HAM TAO CONTROL
        // =================================================

        private static TableLayoutPanel BangTrong(int cot, int hang, Color nen)
        {
            return new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = cot,
                RowCount = hang,
                BackColor = nen,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
        }

        private static Label ChiSo(TableLayoutPanel cha, int cot,
            string tieuDe, string giaTri)
        {
            Panel o = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Trang,
                Margin = Padding.Empty,
                Padding = new Padding(16, 2, 10, 0)
            };
            Label nhanTieuDe = Nhan(tieuDe, 7.6F, true, ChuPhu);
            nhanTieuDe.Dock = DockStyle.Top;
            nhanTieuDe.Height = 20;
            Label nhanGiaTri = Nhan(giaTri, 11F, true, Chu);
            nhanGiaTri.Dock = DockStyle.Fill;
            nhanGiaTri.TextAlign = ContentAlignment.TopLeft;
            o.Controls.Add(nhanGiaTri);
            o.Controls.Add(nhanTieuDe);
            cha.Controls.Add(o, cot, 0);
            return nhanGiaTri;
        }

        private static Panel HangThamSo(string nhan, Control o)
        {
            Panel hang = new Panel
            {
                Height = 42,
                BackColor = Trang,
                Margin = new Padding(0, 0, 0, 4)
            };
            Label l = Nhan(nhan, 9.4F, false, Chu);
            l.Dock = DockStyle.Fill;
            o.Dock = DockStyle.Right;
            o.Width = 120;
            hang.Controls.Add(l);
            hang.Controls.Add(o);
            return hang;
        }

        private static ONhap BocO(NumericUpDown nud)
        {
            ONhap o = new ONhap { Padding = new Padding(8, 8, 6, 4) };
            nud.Dock = DockStyle.Fill;
            o.Controls.Add(nud);
            return o;
        }

        private static RoundPanel The(Color mauNen, Padding padding)
        {
            return new RoundPanel
            {
                Radius = 15,
                BackColor = mauNen,
                Padding = padding,
                Margin = Padding.Empty
            };
        }

        private static Label Nhan(string text, float coChu,
            bool dam, Color mau)
        {
            return new Label
            {
                Text = text,
                AutoEllipsis = true,
                ForeColor = mau,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", coChu,
                    dam ? FontStyle.Bold : FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Button Nut(string chu, Color nen, Color mauChu, float coChu)
        {
            Button b = new Button
            {
                Text = chu,
                BackColor = nen,
                ForeColor = mauChu,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", coChu, FontStyle.Bold),
                Margin = new Padding(2),
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.SizeChanged += (s, e) => BoTron(b, 10);
            return b;
        }

        private static void DoiMauNut(Button b, Color nen, Color chu)
        {
            b.BackColor = nen;
            b.ForeColor = chu;
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(nen, 0.15f);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(nen, 0.05f);
        }

        private static NumericUpDown So(string ten, decimal max, decimal value)
        {
            return new NumericUpDown
            {
                Name = ten,
                Dock = DockStyle.Fill,
                Minimum = 1m,
                Maximum = max,
                Value = value,
                ThousandsSeparator = true,
                TextAlign = HorizontalAlignment.Right,
                BackColor = NenO,
                ForeColor = Chu,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BorderStyle = BorderStyle.None,
                Margin = Padding.Empty
            };
        }

        private static DataGridViewTextBoxColumn Cot(string tieuDe,
            string duLieu, float tyLe)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn
            {
                HeaderText = tieuDe,
                Name = "col" + (duLieu ?? tieuDe),
                FillWeight = tyLe,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            if (duLieu != null) c.DataPropertyName = duLieu;
            return c;
        }

        private static void BoTron(Control control, int radius)
        {
            if (control == null || control.IsDisposed ||
                control.Width < 2 || control.Height < 2)
                return;
            using (GraphicsPath path = DuongBoTron(
                new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                Region region = new Region(path);
                Region cu = control.Region;
                control.Region = region;
                if (cu != null) cu.Dispose();
            }
        }

        private static GraphicsPath DuongBoTron(Rectangle rect, int radius)
        {
            int d = Math.Max(2, Math.Min(radius * 2,
                Math.Min(rect.Width, rect.Height)));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(rect.Left, rect.Top, d, d, 180, 90);
            p.AddArc(rect.Right - d, rect.Top, d, d, 270, 90);
            p.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            p.AddArc(rect.Left, rect.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static bool HopLe(double x)
        {
            return !double.IsNaN(x) && !double.IsInfinity(x);
        }

        private static string DinhDangKhoangCach(double x)
        {
            return x >= 1000d
                ? (x / 1000d).ToString("N2", Vi) + " km"
                : x.ToString("N1", Vi) + " m";
        }

        private static string ChuanHoa(string value)
        {
            string s = (value ?? string.Empty).Trim().ToLowerInvariant()
                .Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            StringBuilder b = new StringBuilder();
            foreach (char c in s)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                    b.Append(c);
            }
            return b.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string Csv(string value)
        {
            string s = value ?? string.Empty;
            string first = s.TrimStart(' ', '\t', '\r', '\n');
            if (first.Length > 0 && "=+-@".IndexOf(first[0]) >= 0)
                s = "'" + s;
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        }

        private void ThongBaoChuaTichHop(string ten)
        {
            MessageBox.Show(this,
                "FormMain chưa đăng ký chức năng " + ten + ".",
                "Chưa tích hợp", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg,
            IntPtr wParam, string lParam);

        // =================================================
        // CONTROL TU VE
        // =================================================

        private sealed class RoundPanel : Panel
        {
            private int radius = 15;
            public int Radius
            {
                get { return radius; }
                set { radius = Math.Max(2, value); BoTron(this, radius); }
            }

            public RoundPanel()
            {
                DoubleBuffered = true;
                Resize += (s, e) => BoTron(this, Radius);
            }
        }

        /// <summary>O nhap bo goc: nen nhat + vien mong, control con khong vien.</summary>
        private sealed class ONhap : Panel
        {
            public ONhap()
            {
                DoubleBuffered = true;
                BackColor = Trang;
                Height = 38;
                ResizeRedraw = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
                using (GraphicsPath p = DuongBoTron(r, 9))
                using (SolidBrush b = new SolidBrush(NenO))
                using (Pen pen = new Pen(ContainsFocus ? XanhLa : Vien, ContainsFocus ? 1.6f : 1f))
                {
                    e.Graphics.FillPath(b, p);
                    e.Graphics.DrawPath(pen, p);
                }
            }

            protected override void OnControlAdded(ControlEventArgs e)
            {
                base.OnControlAdded(e);
                e.Control.GotFocus += (s, ev) => Invalidate();
                e.Control.LostFocus += (s, ev) => Invalidate();
            }
        }

        /// <summary>Tieu de buoc: vong so mau xanh + chu dam.</summary>
        private sealed class TieuDeBuoc : Control
        {
            private readonly int so;

            public TieuDeBuoc(int so, string chu)
            {
                this.so = so;
                Text = chu;
                Dock = DockStyle.Fill;
                Margin = Padding.Empty;
                BackColor = Trang;
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                int y = (Height - 22) / 2;
                using (SolidBrush b = new SolidBrush(XanhLa))
                    g.FillEllipse(b, 0, y, 22, 22);
                using (Font f = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                    TextRenderer.DrawText(g, so.ToString(CultureInfo.InvariantCulture), f,
                        new Rectangle(0, y, 22, 22), Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(g, Text, Font, new Rectangle(30, 0, Width - 30, Height), Chu,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        /// <summary>
        /// The lua chon phep phan tich (ke thua Button de giu Enabled/Click):
        /// tieu de dam + mo ta 1 dong; dang chon -> vien xanh, nen xanh nhat, dau tich.
        /// </summary>
        private sealed class TheLuaChon : Button
        {
            private bool dangChon;
            private bool diChuot;
            public string MoTa { get; private set; }

            public bool DangChon
            {
                get { return dangChon; }
                set { if (dangChon != value) { dangChon = value; Invalidate(); } }
            }

            public TheLuaChon(string tieuDe, string moTa)
            {
                Text = tieuDe;
                MoTa = moTa;
                Dock = DockStyle.Fill;
                Cursor = Cursors.Hand;
                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                BackColor = Trang;
                Font = new Font("Segoe UI", 9.6F, FontStyle.Bold);
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            }

            protected override bool ShowFocusCues { get { return false; } }

            protected override void OnMouseEnter(EventArgs e) { diChuot = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { diChuot = false; Invalidate(); base.OnMouseLeave(e); }
            protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(Parent != null ? Parent.BackColor : Trang);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle r = new Rectangle(1, 1, Width - 3, Height - 3);
                Color nen = dangChon ? XanhLaNhat : (diChuot && Enabled ? Color.FromArgb(246, 250, 255) : Trang);
                Color vien = dangChon ? XanhLa : (diChuot && Enabled ? Color.FromArgb(160, 205, 235) : Vien);
                using (GraphicsPath p = DuongBoTron(r, 11))
                using (SolidBrush b = new SolidBrush(nen))
                using (Pen pen = new Pen(vien, dangChon ? 1.8f : 1.2f))
                {
                    g.FillPath(b, p);
                    g.DrawPath(pen, p);
                }
                Color chu = Enabled ? Chu : Color.FromArgb(160, 175, 195);
                Color phu = Enabled ? ChuPhu : Color.FromArgb(185, 196, 210);
                int le = 12;
                int phai = dangChon ? 34 : 10;
                int giua = Height / 2;
                TextRenderer.DrawText(g, Text, Font,
                    new Rectangle(le, giua - 22, Width - le - phai, 22), chu,
                    TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis);
                using (Font f = new Font("Segoe UI", 8.2F))
                    TextRenderer.DrawText(g, MoTa, f,
                        new Rectangle(le, giua + 1, Width - le - 8, 20), phu,
                        TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);
                if (dangChon)
                {
                    Rectangle c = new Rectangle(Width - 28, 9, 18, 18);
                    using (SolidBrush b = new SolidBrush(XanhLa))
                        g.FillEllipse(b, c);
                    using (Pen p = new Pen(Color.White, 2f))
                    {
                        p.StartCap = p.EndCap = LineCap.Round;
                        g.DrawLines(p, new[]
                        {
                            new Point(c.Left + 5, c.Top + 9),
                            new Point(c.Left + 8, c.Top + 12),
                            new Point(c.Left + 13, c.Top + 6)
                        });
                    }
                }
            }
        }

        private sealed class GradientBanner : Panel
        {
            public int Radius { get; set; }
            public Color MauDau { get; set; }
            public Color MauGiua { get; set; }
            public Color MauCuoi { get; set; }
            public string[] Buoc { get; set; }

            public GradientBanner()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
                Resize += (s, e) => BoTron(this, Radius);
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                if (Width <= 0 || Height <= 0) return;
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Rectangle rect = new Rectangle(0, 0, Width, Height);
                using (LinearGradientBrush nen = new LinearGradientBrush(
                    rect, MauDau, MauCuoi, LinearGradientMode.Horizontal))
                {
                    nen.InterpolationColors = new ColorBlend
                    {
                        Colors = new[] { MauDau, MauGiua, MauCuoi },
                        Positions = new[] { 0f, 0.52f, 1f }
                    };
                    g.FillRectangle(nen, rect);
                }
                using (SolidBrush nhan = new SolidBrush(Color.FromArgb(230, 249, 81)))
                    g.FillRectangle(nhan, 0, 12, 5, Math.Max(0, Height - 24));

                // Cac "chip" buoc thuc hien o ben phai banner.
                if (Buoc == null || Buoc.Length == 0) return;
                using (Font f = new Font("Segoe UI", 8.8F, FontStyle.Bold))
                using (SolidBrush chip = new SolidBrush(Color.FromArgb(48, Color.White)))
                {
                    int x = Width - 18;
                    int h = 28;
                    int y = (Height - h) / 2;
                    for (int i = Buoc.Length - 1; i >= 0; i--)
                    {
                        Size sz = TextRenderer.MeasureText(Buoc[i], f);
                        int w = sz.Width + 22;
                        x -= w;
                        if (x < 380) break;
                        using (GraphicsPath p = DuongBoTron(new Rectangle(x, y, w, h), 14))
                            g.FillPath(chip, p);
                        TextRenderer.DrawText(g, Buoc[i], f, new Rectangle(x, y, w, h), Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                        x -= 8;
                    }
                }
            }
        }
    }
}
