using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>
    /// Trang "QUAN TRI HE THONG" (thiet ke v2): 5 the
    ///   1. Phan quyen &amp; duyet  - doi vai tro phien (SET ROLE + RLS), duyet yeu cau
    ///   2. Nhat ky              - nhat ky ung dung + nhat ky trigger bang goc
    ///   3. Sao luu              - pg_dump / kiem tra / khoi phuc sang CSDL moi
    ///   4. Kiem thu CSDL        - ql_kiem_thu.fn_chay_kiem_thu()
    ///   5. Hieu nang            - EXPLAIN ANALYZE co/khong chi muc, kich thuoc, chi muc
    /// </summary>
    public sealed class GiaoDienQuanTri : UserControl
    {
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");
        private static readonly Color Nen = TienIchGiaoDienSang.Background;

        /// <summary>FormMain doi vai tro phien: null = tai khoan ung dung goc.</summary>
        public event Action<string> YeuCauDoiVaiTro;

        private readonly TruyXuatQuanTri dal = new TruyXuatQuanTri();
        private readonly BannerTrang banner;
        private readonly List<Button> nutThe = new List<Button>();
        private readonly Panel[] trang = new Panel[6];
        private TrangQuanLyTaiKhoan trangTaiKhoan;

        /// <summary>So dang ky tai khoan dang cho duyet (FormMain hien huy hieu).</summary>
        public event Action<int> SoTaiKhoanChoDuyet;
        private readonly Label lblTrangThai;
        private int theChon = -1;

        // the 1
        private readonly TheVaiTro[] theVaiTro = new TheVaiTro[3];
        private readonly DataGridView gridYeuCau, gridKhacBiet;
        private readonly Button btnDuyet, btnTuChoi, btnLocCho, btnLocTatCa;
        private readonly TextBox txtGhiChu;
        private readonly Label lblYcTieuDe, lblYcMoTa;
        private bool locChoDuyet = true;
        private string vaiTroHieuLuc = "";

        // the 2
        private readonly DataGridView gridNhatKy, gridNhatKyChiTiet;
        private readonly TextBox txtLocNhatKy;
        private List<DongNhatKy> nhatKy = new List<DongNhatKy>();
        private readonly Timer henLoc = new Timer { Interval = 250 };

        // the 3
        private readonly DataGridView gridSaoLuu;
        private readonly Label lblThuMuc;
        private readonly TextBox txtNhatKySaoLuu;
        private string thuMucSaoLuu;

        // the 4
        private readonly DataGridView gridKiemThu;
        private readonly Label lblTomTatKiemThu;

        // the 5
        private readonly DataGridView gridHieuNang, gridKichThuoc, gridChiMuc;

        public GiaoDienQuanTri()
        {
            Name = "UcQuanTriHeThong";
            Tag = "v2";   // FormMain khong "trang tri lai" trang nay
            Dock = DockStyle.Fill;
            BackColor = Nen;
            Font = new Font("Segoe UI", 9F);
            DoubleBuffered = true;

            Panel khung = new Panel { Dock = DockStyle.Fill, BackColor = Nen, Padding = new Padding(18, 15, 18, 14) };
            Controls.Add(khung);

            banner = new BannerTrang
            {
                Dock = DockStyle.Top,
                Height = 96,
                TieuDe = "QUẢN TRỊ HỆ THỐNG",
                PhuDe = "Phân quyền RLS · duyệt thay đổi · nhật ký · sao lưu · kiểm thử và hiệu năng CSDL",
                NhanPhai = "TÀI KHOẢN ĐĂNG NHẬP",
                GiaTriPhai = "—"
            };

            // Thanh the
            Panel thanhThe = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Nen, Padding = new Padding(0, 10, 0, 8) };
            KhungBoGocChung khungThe = new KhungBoGocChung { Dock = DockStyle.Left, Width = 900, Padding = new Padding(4) };
            TableLayoutPanel bangThe = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1, BackColor = Color.White, Margin = Padding.Empty };
            string[] tenThe = { "Duyệt thay đổi", "Nhật ký thay đổi", "Sao lưu & khôi phục", "Kiểm thử CSDL", "Hiệu năng", "Tài khoản & phân quyền" };
            for (int i = 0; i < 6; i++)
            {
                bangThe.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, i == 0 ? 20F : 16F));
                int so = i;
                Button b = NutGiaoDien.Phang(tenThe[i], Color.White, TienIchGiaoDienSang.Muted, 9F);
                b.Dock = DockStyle.Fill;
                b.Margin = new Padding(1, 0, 1, 0);
                b.Click += (s, e) => ChonThe(so);
                nutThe.Add(b);
                bangThe.Controls.Add(b, i == 5 ? 0 : i + 1, 0);   // the Tai khoan dung dau
            }
            bangThe.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            khungThe.Controls.Add(bangThe);
            lblTrangThai = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblTrangThai.Dock = DockStyle.Fill;
            lblTrangThai.TextAlign = ContentAlignment.MiddleRight;
            thanhThe.Controls.Add(lblTrangThai);
            thanhThe.Controls.Add(khungThe);

            Panel noiDung = new Panel { Dock = DockStyle.Fill, BackColor = Nen };
            for (int i = 0; i < 6; i++)
            {
                trang[i] = new Panel { Dock = DockStyle.Fill, BackColor = Nen, Visible = false };
                noiDung.Controls.Add(trang[i]);
            }

            khung.Controls.Add(noiDung);
            khung.Controls.Add(thanhThe);
            khung.Controls.Add(banner);

            // ============ THE 1: PHAN QUYEN & DUYET ============
            TableLayoutPanel t1 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Nen };
            t1.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            t1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableLayoutPanel hangVaiTro = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = Nen, Margin = new Padding(0, 0, 0, 12) };
            string[] ma = { "gis_quan_tri", "gis_bien_tap", "gis_xem" };
            string[] tieuDe = { "Quản trị", "Biên tập", "Chỉ xem" };
            string[] moTa =
            {
                "Xem toàn bộ (kể cả Tuyệt mật) · sửa/xóa trực tiếp · duyệt yêu cầu",
                "Xem đến mức Mật · thêm/sửa/định vị → gửi vào hàng đợi chờ duyệt",
                "Chỉ xem đến mức Nội bộ · các nút sửa/xóa tự khóa theo GRANT"
            };
            Color[] mau = { VeBaoCao.Teal, VeBaoCao.Blue, VeBaoCao.Amber };
            for (int i = 0; i < 3; i++)
            {
                hangVaiTro.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
                string vt = ma[i];
                theVaiTro[i] = new TheVaiTro(tieuDe[i], moTa[i], mau[i]) { Dock = DockStyle.Fill, Margin = new Padding(i == 0 ? 0 : 6, 0, i == 2 ? 0 : 6, 0) };
                theVaiTro[i].Click += (s, e) => DoiVaiTro(vt);
                hangVaiTro.Controls.Add(theVaiTro[i], i, 0);
            }
            hangVaiTro.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            t1.Controls.Add(hangVaiTro, 0, 0);

            NativeCard cardYc = TienIchGiaoDienSang.Card();
            cardYc.Padding = new Padding(16, 12, 16, 12);
            cardYc.Dock = DockStyle.Fill;
            Panel dauYc = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.White };
            Label tdYc = TienIchGiaoDienSang.Text("Yêu cầu thay đổi chờ duyệt", 11.5F, true);
            tdYc.Dock = DockStyle.Left; tdYc.Width = 300;
            FlowLayoutPanel locYc = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 330, FlowDirection = FlowDirection.RightToLeft, WrapContents = false, BackColor = Color.White, Padding = new Padding(0, 4, 0, 0) };
            Button btnTaiYc = NutGiaoDien.Phang("↻", Color.White, TienIchGiaoDienSang.Ink, 10F);
            btnTaiYc.Size = new Size(38, 32); btnTaiYc.FlatAppearance.BorderSize = 1; btnTaiYc.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            btnLocTatCa = NutGiaoDien.Phang("Tất cả", Color.White, TienIchGiaoDienSang.Muted, 8.8F);
            btnLocTatCa.Size = new Size(90, 32);
            btnLocCho = NutGiaoDien.Phang("Chờ duyệt", VeBaoCao.Teal, Color.White, 8.8F);
            btnLocCho.Size = new Size(100, 32);
            btnTaiYc.Margin = new Padding(8, 0, 0, 0); btnLocTatCa.Margin = new Padding(4, 0, 0, 0); btnLocCho.Margin = Padding.Empty;
            locYc.Controls.Add(btnTaiYc); locYc.Controls.Add(btnLocTatCa); locYc.Controls.Add(btnLocCho);
            dauYc.Controls.Add(locYc); dauYc.Controls.Add(tdYc);

            TableLayoutPanel chiaYc = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.White };
            chiaYc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 56F));
            chiaYc.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44F));
            chiaYc.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gridYeuCau = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridYeuCau, "#", "Id", 8);
            BangGiaoDien.Cot(gridYeuCau, "Trạng thái", "TrangThaiHienThi", 17);
            BangGiaoDien.Cot(gridYeuCau, "Loại", "LoaiHienThi", 16, false, true);
            BangGiaoDien.Cot(gridYeuCau, "Đơn vị", "TenDonVi", 33);
            BangGiaoDien.Cot(gridYeuCau, "Người gửi", "NguoiGuiHienThi", 13);
            BangGiaoDien.Cot(gridYeuCau, "Thời gian", "ThoiGianGuiHienThi", 17);
            BangGiaoDien.VeNhanTron(gridYeuCau, 1, MauTrangThai);
            gridYeuCau.Margin = new Padding(0, 0, 10, 0);
            chiaYc.Controls.Add(gridYeuCau, 0, 0);

            Panel ctYc = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 251, 255), Padding = new Padding(14, 10, 14, 12), Margin = new Padding(10, 0, 0, 0) };
            lblYcTieuDe = TienIchGiaoDienSang.Text("Chọn một yêu cầu để xem thay đổi.", 10.5F, true);
            lblYcTieuDe.Dock = DockStyle.Top; lblYcTieuDe.Height = 26;
            lblYcMoTa = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblYcMoTa.Dock = DockStyle.Top; lblYcMoTa.Height = 22;
            gridKhacBiet = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridKhacBiet, "Trường", "Item1", 30, false, true);
            BangGiaoDien.Cot(gridKhacBiet, "Trước", "Item2", 35);
            BangGiaoDien.Cot(gridKhacBiet, "Sau / đề nghị", "Item3", 35);
            gridKhacBiet.Columns[2].DefaultCellStyle.ForeColor = VeBaoCao.Teal;
            Panel chanYc = new Panel { Dock = DockStyle.Bottom, Height = 86, BackColor = Color.FromArgb(248, 251, 255), Padding = new Padding(0, 8, 0, 0) };
            txtGhiChu = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5F), BorderStyle = BorderStyle.FixedSingle };
            txtGhiChu.HandleCreated += (s, e) => GoiY(txtGhiChu, "Ghi chú duyệt / lý do từ chối…");
            FlowLayoutPanel nutYc = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, FlowDirection = FlowDirection.RightToLeft, BackColor = Color.FromArgb(248, 251, 255), Padding = new Padding(0, 6, 0, 0) };
            btnDuyet = NutGiaoDien.Phang("✓  Duyệt và áp dụng", VeBaoCao.Teal, Color.White, 9F);
            btnDuyet.Size = new Size(170, 36); btnDuyet.Margin = Padding.Empty;
            btnTuChoi = NutGiaoDien.Phang("✕  Từ chối", BangGiaoDien.DoNhat, BangGiaoDien.Do, 9F);
            btnTuChoi.Size = new Size(110, 36); btnTuChoi.Margin = new Padding(0, 0, 8, 0);
            nutYc.Controls.Add(btnDuyet); nutYc.Controls.Add(btnTuChoi);
            chanYc.Controls.Add(nutYc); chanYc.Controls.Add(txtGhiChu);
            ctYc.Controls.Add(gridKhacBiet); ctYc.Controls.Add(chanYc); ctYc.Controls.Add(lblYcMoTa); ctYc.Controls.Add(lblYcTieuDe);
            chiaYc.Controls.Add(ctYc, 1, 0);
            cardYc.Controls.Add(chiaYc); cardYc.Controls.Add(dauYc);
            t1.Controls.Add(cardYc, 0, 1);
            trang[0].Controls.Add(t1);

            btnLocCho.Click += (s, e) => { locChoDuyet = true; CapNhatNutLocYc(); TaiYeuCau(); };
            btnLocTatCa.Click += (s, e) => { locChoDuyet = false; CapNhatNutLocYc(); TaiYeuCau(); };
            btnTaiYc.Click += (s, e) => TaiYeuCau();
            gridYeuCau.SelectionChanged += (s, e) => HienChiTietYeuCau();
            btnDuyet.Click += (s, e) => XuLyYeuCau(true);
            btnTuChoi.Click += (s, e) => XuLyYeuCau(false);

            // ============ THE 2: NHAT KY ============
            NativeCard cardNk = TienIchGiaoDienSang.Card();
            cardNk.Padding = new Padding(16, 12, 16, 12); cardNk.Dock = DockStyle.Fill;
            Panel dauNk = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
            Label tdNk = TienIchGiaoDienSang.Text("Nhật ký thay đổi dữ liệu", 11.5F, true);
            tdNk.Dock = DockStyle.Left; tdNk.Width = 280;
            Button btnTaiNk = NutGiaoDien.Phang("↻  Tải lại", Color.White, TienIchGiaoDienSang.Ink, 9F);
            btnTaiNk.Dock = DockStyle.Right; btnTaiNk.Width = 100; btnTaiNk.FlatAppearance.BorderSize = 1; btnTaiNk.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            KhungBoGocChung oLoc = new KhungBoGocChung { Dock = DockStyle.Right, Width = 320, Padding = new Padding(10, 9, 8, 4), Margin = Padding.Empty };
            txtLocNhatKy = new TextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9.5F) };
            txtLocNhatKy.HandleCreated += (s, e) => GoiY(txtLocNhatKy, "⌕  Lọc theo mã đơn vị, bảng, tài khoản…");
            oLoc.Controls.Add(txtLocNhatKy);
            Panel cachNk = new Panel { Dock = DockStyle.Right, Width = 10, BackColor = Color.White };
            dauNk.Controls.Add(tdNk); dauNk.Controls.Add(oLoc); dauNk.Controls.Add(cachNk); dauNk.Controls.Add(btnTaiNk);
            TableLayoutPanel chiaNk = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.White };
            chiaNk.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            chiaNk.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            chiaNk.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gridNhatKy = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridNhatKy, "Thời điểm", "ThoiDiemHienThi", 22);
            BangGiaoDien.Cot(gridNhatKy, "Thao tác", "ThaoTac", 11);
            BangGiaoDien.Cot(gridNhatKy, "Mã đơn vị", "MaDonVi", 21, false, true);
            BangGiaoDien.Cot(gridNhatKy, "Bảng", "Bang", 18);
            BangGiaoDien.Cot(gridNhatKy, "Nguồn", "Nguon", 16);
            BangGiaoDien.Cot(gridNhatKy, "Tài khoản", "TaiKhoan", 14);
            BangGiaoDien.VeNhanTron(gridNhatKy, 1, s => s == "Thêm" ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal)
                : s == "Xóa" ? Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do) : Tuple.Create(BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue));
            gridNhatKy.Margin = new Padding(0, 0, 10, 0);
            gridNhatKyChiTiet = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridNhatKyChiTiet, "Trường", "Item1", 30, false, true);
            BangGiaoDien.Cot(gridNhatKyChiTiet, "Trước", "Item2", 35);
            BangGiaoDien.Cot(gridNhatKyChiTiet, "Sau", "Item3", 35);
            gridNhatKyChiTiet.Columns[2].DefaultCellStyle.ForeColor = VeBaoCao.Teal;
            gridNhatKyChiTiet.Margin = new Padding(10, 0, 0, 0);
            chiaNk.Controls.Add(gridNhatKy, 0, 0);
            chiaNk.Controls.Add(gridNhatKyChiTiet, 1, 0);
            cardNk.Controls.Add(chiaNk); cardNk.Controls.Add(dauNk);
            trang[1].Controls.Add(cardNk);
            btnTaiNk.Click += (s, e) => TaiNhatKy();
            henLoc.Tick += (s, e) => { henLoc.Stop(); LocNhatKy(); };
            txtLocNhatKy.TextChanged += (s, e) => { henLoc.Stop(); henLoc.Start(); };
            gridNhatKy.SelectionChanged += (s, e) => HienChiTietNhatKy();

            // ============ THE 3: SAO LUU ============
            NativeCard cardSl = TienIchGiaoDienSang.Card();
            cardSl.Padding = new Padding(16, 12, 16, 12); cardSl.Dock = DockStyle.Fill;
            Panel dauSl = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = Color.White };
            Label tdSl = TienIchGiaoDienSang.Text("Sao lưu & khôi phục CSDL (pg_dump · định dạng custom)", 11.5F, true);
            tdSl.Dock = DockStyle.Top; tdSl.Height = 28;
            lblThuMuc = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblThuMuc.Dock = DockStyle.Top; lblThuMuc.Height = 22;
            FlowLayoutPanel nutSl = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 0, BackColor = Color.White };
            dauSl.Controls.Add(lblThuMuc); dauSl.Controls.Add(tdSl);
            FlowLayoutPanel hanhDongSl = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, BackColor = Color.White, WrapContents = false, Padding = new Padding(0, 4, 0, 6) };
            Button btnSaoLuu = NutGiaoDien.Phang("⬇  Sao lưu ngay", VeBaoCao.Teal, Color.White, 9F);
            btnSaoLuu.Size = new Size(150, 36);
            Button btnKiemTraSl = NutGiaoDien.Phang("✓  Kiểm tra tệp", BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue, 9F);
            btnKiemTraSl.Size = new Size(140, 36);
            Button btnKhoiPhuc = NutGiaoDien.Phang("↺  Khôi phục sang CSDL mới", BangGiaoDien.CamNhat, BangGiaoDien.Cam, 9F);
            btnKhoiPhuc.Size = new Size(220, 36);
            Button btnXoaSl = NutGiaoDien.Phang("✕  Xóa tệp", BangGiaoDien.DoNhat, BangGiaoDien.Do, 9F);
            btnXoaSl.Size = new Size(110, 36);
            Button btnDoiThuMuc = NutGiaoDien.Phang("Đổi thư mục…", Color.White, TienIchGiaoDienSang.Ink, 9F);
            btnDoiThuMuc.Size = new Size(120, 36); btnDoiThuMuc.FlatAppearance.BorderSize = 1; btnDoiThuMuc.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            Button btnMoThuMuc = NutGiaoDien.Phang("Mở thư mục", Color.White, TienIchGiaoDienSang.Ink, 9F);
            btnMoThuMuc.Size = new Size(110, 36); btnMoThuMuc.FlatAppearance.BorderSize = 1; btnMoThuMuc.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            foreach (Button b in new[] { btnSaoLuu, btnKiemTraSl, btnKhoiPhuc, btnXoaSl, btnDoiThuMuc, btnMoThuMuc })
            {
                b.Margin = new Padding(0, 0, 8, 0);
                hanhDongSl.Controls.Add(b);
            }
            TableLayoutPanel chiaSl = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.White };
            chiaSl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            chiaSl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            chiaSl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            gridSaoLuu = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridSaoLuu, "Tệp sao lưu", "Ten", 58, false, true);
            BangGiaoDien.Cot(gridSaoLuu, "Kích thước", "KichThuoc", 18, true);
            BangGiaoDien.Cot(gridSaoLuu, "Thời gian", "ThoiGianHienThi", 24);
            gridSaoLuu.Margin = new Padding(0, 0, 10, 0);
            txtNhatKySaoLuu = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
                BorderStyle = BorderStyle.None, BackColor = Color.FromArgb(17, 34, 48), ForeColor = Color.FromArgb(178, 236, 214),
                Font = new Font("Consolas", 9F), Margin = new Padding(10, 0, 0, 0)
            };
            chiaSl.Controls.Add(gridSaoLuu, 0, 0);
            chiaSl.Controls.Add(txtNhatKySaoLuu, 1, 0);
            cardSl.Controls.Add(chiaSl); cardSl.Controls.Add(hanhDongSl); cardSl.Controls.Add(dauSl);
            trang[2].Controls.Add(cardSl);
            btnSaoLuu.Click += async (s, e) => await SaoLuu();
            btnKiemTraSl.Click += async (s, e) => await KiemTraSaoLuu();
            btnKhoiPhuc.Click += async (s, e) => await KhoiPhuc();
            btnXoaSl.Click += (s, e) => XoaSaoLuu();
            btnDoiThuMuc.Click += (s, e) => DoiThuMuc();
            btnMoThuMuc.Click += (s, e) => { if (Directory.Exists(thuMucSaoLuu)) Process.Start("explorer.exe", "\"" + thuMucSaoLuu + "\""); };

            // ============ THE 4: KIEM THU ============
            NativeCard cardKt = TienIchGiaoDienSang.Card();
            cardKt.Padding = new Padding(16, 12, 16, 12); cardKt.Dock = DockStyle.Fill;
            Panel dauKt = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.White };
            Button btnChayKt = NutGiaoDien.Phang("▶  Chạy bộ kiểm thử", VeBaoCao.Teal, Color.White, 9.4F);
            btnChayKt.Dock = DockStyle.Left; btnChayKt.Width = 180;
            Panel cachKt = new Panel { Dock = DockStyle.Left, Width = 14, BackColor = Color.White };
            lblTomTatKiemThu = TienIchGiaoDienSang.Text("Bộ kiểm thử ràng buộc, trigger, hàm không gian, dữ liệu và phân quyền RLS. Ca có ghi dữ liệu tự hoàn tác.", 9F, false, TienIchGiaoDienSang.Muted);
            lblTomTatKiemThu.Dock = DockStyle.Fill;
            dauKt.Controls.Add(lblTomTatKiemThu); dauKt.Controls.Add(cachKt); dauKt.Controls.Add(btnChayKt);
            gridKiemThu = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridKiemThu, "#", "Stt", 5);
            BangGiaoDien.Cot(gridKiemThu, "Kết quả", "KetQua", 12);
            BangGiaoDien.Cot(gridKiemThu, "Nhóm", "Nhom", 14, false, true);
            BangGiaoDien.Cot(gridKiemThu, "Ca kiểm thử", "Ten", 32);
            BangGiaoDien.Cot(gridKiemThu, "Chi tiết", "ChiTiet", 37);
            BangGiaoDien.VeNhanTron(gridKiemThu, 1, s => s == "Đạt" ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal) : Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do));
            cardKt.Controls.Add(gridKiemThu); cardKt.Controls.Add(dauKt);
            trang[3].Controls.Add(cardKt);
            btnChayKt.Click += async (s, e) => await ChayKiemThu();

            // ============ THE 5: HIEU NANG ============
            TableLayoutPanel t5 = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, BackColor = Nen };
            t5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            t5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            t5.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));
            t5.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));
            NativeCard cardHn = TienIchGiaoDienSang.Card();
            cardHn.Padding = new Padding(16, 12, 16, 12); cardHn.Dock = DockStyle.Fill; cardHn.Margin = new Padding(0, 0, 0, 12);
            Panel dauHn = new Panel { Dock = DockStyle.Top, Height = 48, BackColor = Color.White };
            Button btnDoHn = NutGiaoDien.Phang("▶  Đo EXPLAIN ANALYZE", VeBaoCao.Teal, Color.White, 9.2F);
            btnDoHn.Dock = DockStyle.Left; btnDoHn.Width = 190;
            Button btnLamMoiTk = NutGiaoDien.Phang("↻  Làm mới thống kê (MV + ANALYZE)", BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue, 9F);
            btnLamMoiTk.Dock = DockStyle.Right; btnLamMoiTk.Width = 270;
            Label tdHn = TienIchGiaoDienSang.Text("   So sánh thời gian có / không dùng chỉ mục (lấy nhỏ nhất 3 lần)", 9F, false, TienIchGiaoDienSang.Muted);
            tdHn.Dock = DockStyle.Fill;
            dauHn.Controls.Add(tdHn); dauHn.Controls.Add(btnLamMoiTk); dauHn.Controls.Add(btnDoHn);
            gridHieuNang = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridHieuNang, "Truy vấn", "Ten", 26, false, true);
            BangGiaoDien.Cot(gridHieuNang, "Kỹ thuật", "MoTa", 26);
            BangGiaoDien.Cot(gridHieuNang, "Có chỉ mục (ms)", "MsCoChiMuc", 13, true);
            BangGiaoDien.Cot(gridHieuNang, "Không chỉ mục (ms)", "MsKhongChiMuc", 14, true);
            BangGiaoDien.Cot(gridHieuNang, "Nhanh hơn", "NhanhHon", 10, true, true);
            BangGiaoDien.Cot(gridHieuNang, "Nút gốc kế hoạch", "KeHoach", 11);
            gridHieuNang.Columns[4].DefaultCellStyle.ForeColor = VeBaoCao.Teal;
            cardHn.Controls.Add(gridHieuNang); cardHn.Controls.Add(dauHn);
            t5.Controls.Add(cardHn, 0, 0); t5.SetColumnSpan(cardHn, 2);

            NativeCard cardKtb = TienIchGiaoDienSang.Card();
            cardKtb.Padding = new Padding(16, 12, 16, 12); cardKtb.Dock = DockStyle.Fill; cardKtb.Margin = new Padding(0, 0, 6, 0);
            Label tdKtb = TienIchGiaoDienSang.Text("Kích thước bảng", 11F, true); tdKtb.Dock = DockStyle.Top; tdKtb.Height = 30;
            gridKichThuoc = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridKichThuoc, "Bảng", "TenBang", 36, false, true);
            BangGiaoDien.Cot(gridKichThuoc, "Lược đồ", "LuocDo", 18);
            BangGiaoDien.Cot(gridKichThuoc, "Số dòng", "SoDong", 14, true);
            BangGiaoDien.Cot(gridKichThuoc, "Chỉ mục", "ChiMuc", 15, true);
            BangGiaoDien.Cot(gridKichThuoc, "Tổng", "Tong", 15, true);
            cardKtb.Controls.Add(gridKichThuoc); cardKtb.Controls.Add(tdKtb);
            t5.Controls.Add(cardKtb, 0, 1);

            NativeCard cardCm = TienIchGiaoDienSang.Card();
            cardCm.Padding = new Padding(16, 12, 16, 12); cardCm.Dock = DockStyle.Fill; cardCm.Margin = new Padding(6, 0, 0, 0);
            Label tdCm = TienIchGiaoDienSang.Text("Chỉ mục (GiST / B-tree)", 11F, true); tdCm.Dock = DockStyle.Top; tdCm.Height = 30;
            gridChiMuc = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridChiMuc, "Chỉ mục", "TenChiMuc", 38, false, true);
            BangGiaoDien.Cot(gridChiMuc, "Bảng", "Bang", 32);
            BangGiaoDien.Cot(gridChiMuc, "Kiểu", "Kieu", 11);
            BangGiaoDien.Cot(gridChiMuc, "Lượt quét", "SoLanQuet", 13, true);
            BangGiaoDien.VeNhanTron(gridChiMuc, 2, s => s == "GIST" ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal) : Tuple.Create(BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue));
            cardCm.Controls.Add(gridChiMuc); cardCm.Controls.Add(tdCm);
            t5.Controls.Add(cardCm, 1, 1);
            trang[4].Controls.Add(t5);
            btnDoHn.Click += async (s, e) => await DoHieuNang();
            btnLamMoiTk.Click += async (s, e) => await LamMoiThongKe();

            thuMucSaoLuu = DocThuMucSaoLuu();
            // ============ THE 6: TAI KHOAN & PHAN QUYEN ============
            trangTaiKhoan = new TrangQuanLyTaiKhoan();
            trangTaiKhoan.SoChoDuyetThayDoi += n =>
            {
                nutThe[5].Text = n > 0 ? "Tài khoản  ⚑" + n : "Tài khoản & phân quyền";
                Action<int> h = SoTaiKhoanChoDuyet;
                if (h != null) h(n);
            };
            trang[5].Controls.Add(trangTaiKhoan);

            ChonThe(PhienDangNhap.HeThongSanSang ? 5 : 0);
            if (theChon != 0) LamMoiVaiTro();
        }

        // =================================================
        // CONG KHAI
        // =================================================

        /// <summary>FormMain goi sau khi doi vai tro (hoac khi mo trang).</summary>
        public async void LamMoiVaiTro()
        {
            try
            {
                vaiTroHieuLuc = await Task.Run(() => dal.LayVaiTroHieuLuc());
            }
            catch (Exception ex)
            {
                vaiTroHieuLuc = "";
                lblTrangThai.Text = "Không đọc được vai trò: " + ex.Message;
            }
            string ten = TruyXuatQuanTri.TenVaiTro(vaiTroHieuLuc);
            banner.GiaTriPhai = PhienDangNhap.HienTai != null
                ? PhienDangNhap.HienTai.HoTen + " · " + PhienDangNhap.HienTai.TenVaiTro
                : ten;
            string[] ma = { "gis_quan_tri", "gis_bien_tap", "gis_xem" };
            bool laGoc = Array.IndexOf(ma, vaiTroHieuLuc) < 0;
            for (int i = 0; i < 3; i++)
                theVaiTro[i].DangDung = vaiTroHieuLuc == ma[i] || (laGoc && i == 0);
            bool quanTri = laGoc || vaiTroHieuLuc == "gis_quan_tri";
            btnDuyet.Enabled = btnTuChoi.Enabled = quanTri;
            CapNhatNutDuyet();
            TaiYeuCau();
        }

        // =================================================
        // THE
        // =================================================

        private void ChonThe(int i)
        {
            if (theChon == i) return;
            theChon = i;
            for (int k = 0; k < 6; k++)
            {
                bool chon = k == i;
                NutGiaoDien.DoiMau(nutThe[k], chon ? VeBaoCao.Teal : Color.White, chon ? Color.White : TienIchGiaoDienSang.Muted);
                trang[k].Visible = chon;
            }
            trang[i].BringToFront();
            if (i == 0) LamMoiVaiTro();
            else if (i == 1 && nhatKy.Count == 0) TaiNhatKy();
            else if (i == 2) TaiDanhSachSaoLuu();
            else if (i == 4 && gridKichThuoc.Rows.Count == 0) TaiKichThuoc();
            else if (i == 5) trangTaiKhoan.TaiLai();
        }

        private void DatTrangThai(string s) { lblTrangThai.Text = s ?? ""; }

        // =================================================
        // 1. VAI TRO + DUYET
        // =================================================

        private void DoiVaiTro(string vt)
        {
            if (PhienDangNhap.HeThongSanSang)
            {
                // Vai tro do TAI KHOAN quyet dinh; khong cho tu chuyen vai tro nua.
                DatTrangThai("Vai trò phiên do tài khoản đăng nhập quyết định – cấp quyền ở thẻ \"Tài khoản & phân quyền\".");
                return;
            }
            if (vt == vaiTroHieuLuc) return;
            Action<string> h = YeuCauDoiVaiTro;
            if (h != null) h(vt);
        }

        private void CapNhatNutLocYc()
        {
            NutGiaoDien.DoiMau(btnLocCho, locChoDuyet ? VeBaoCao.Teal : Color.White, locChoDuyet ? Color.White : TienIchGiaoDienSang.Muted);
            NutGiaoDien.DoiMau(btnLocTatCa, !locChoDuyet ? VeBaoCao.Teal : Color.White, !locChoDuyet ? Color.White : TienIchGiaoDienSang.Muted);
        }

        private async void TaiYeuCau()
        {
            try
            {
                bool cho = locChoDuyet;
                List<YeuCauThayDoi> ds = await Task.Run(() => dal.LayYeuCau(cho));
                gridYeuCau.DataSource = new BindingList<YeuCauThayDoi>(ds);
                gridYeuCau.ClearSelection();
                int soCho = ds.Count(x => x.TrangThai == "CHO_DUYET");
                DatTrangThai(soCho > 0 ? "● " + soCho + " yêu cầu đang chờ duyệt" : "Không có yêu cầu chờ duyệt");
                HienChiTietYeuCau();
            }
            catch (Exception ex)
            {
                DatTrangThai("Lỗi tải yêu cầu: " + ex.Message);
            }
        }

        private YeuCauThayDoi YeuCauDangChon()
        {
            return gridYeuCau.CurrentRow != null && gridYeuCau.CurrentRow.Selected
                ? gridYeuCau.CurrentRow.DataBoundItem as YeuCauThayDoi : null;
        }

        private void HienChiTietYeuCau()
        {
            YeuCauThayDoi y = YeuCauDangChon();
            if (y == null)
            {
                lblYcTieuDe.Text = gridYeuCau.Rows.Count == 0 ? "Không có yêu cầu nào." : "Chọn một yêu cầu để xem thay đổi.";
                lblYcMoTa.Text = "";
                gridKhacBiet.DataSource = null;
                CapNhatNutDuyet();
                return;
            }
            lblYcTieuDe.Text = "#" + y.Id + " · " + y.LoaiHienThi + " · " + y.TenDonVi;
            lblYcMoTa.Text = y.MaDonVi + " · gửi bởi " + y.NguoiGuiHienThi + " lúc " + y.ThoiGianGuiHienThi +
                (string.IsNullOrEmpty(y.GhiChuDuyet) ? "" : " · " + y.TrangThaiHienThi + ": " + y.GhiChuDuyet);
            List<Tuple<string, string, string>> kb;
            if (y.Loai == "DINH_VI")
            {
                Dictionary<string, string> cu = PhangJson(y.DuLieuCu), ts = PhangJson(y.ThamSo);
                kb = new List<Tuple<string, string, string>>
                {
                    Tuple.Create("Kinh độ / Vĩ độ", Lay(cu, "kinh_do") + " / " + Lay(cu, "vi_do"), "SRID " + Lay(ts, "srid") + ": " + Lay(ts, "e") + " / " + Lay(ts, "n")),
                    Tuple.Create("Phương pháp", "", Lay(ts, "phuong_phap")),
                    Tuple.Create("Độ chính xác (m)", "", Lay(ts, "do_chinh_xac")),
                    Tuple.Create("Nguồn", "", Lay(ts, "nguon")),
                    Tuple.Create("Ghi chú", "", Lay(ts, "ghi_chu"))
                };
            }
            else
            {
                kb = SoSanh(y.DuLieuCu, y.DuLieuMoi);
            }
            gridKhacBiet.DataSource = kb;
            CapNhatNutDuyet();
        }

        private void CapNhatNutDuyet()
        {
            YeuCauThayDoi y = YeuCauDangChon();
            bool quanTri = string.IsNullOrEmpty(vaiTroHieuLuc) || vaiTroHieuLuc == "gis_quan_tri" ||
                           Array.IndexOf(new[] { "gis_bien_tap", "gis_xem" }, vaiTroHieuLuc) < 0;
            bool duoc = quanTri && y != null && y.TrangThai == "CHO_DUYET";
            btnDuyet.Enabled = btnTuChoi.Enabled = duoc;
            NutGiaoDien.DoiMau(btnDuyet, duoc ? VeBaoCao.Teal : Color.FromArgb(232, 238, 245), duoc ? Color.White : Color.FromArgb(160, 176, 194));
            NutGiaoDien.DoiMau(btnTuChoi, duoc ? BangGiaoDien.DoNhat : Color.FromArgb(232, 238, 245), duoc ? BangGiaoDien.Do : Color.FromArgb(160, 176, 194));
        }

        private async void XuLyYeuCau(bool duyet)
        {
            YeuCauThayDoi y = YeuCauDangChon();
            if (y == null) return;
            string ghiChu = txtGhiChu.Text.Trim();
            if (!duyet && ghiChu.Length == 0)
            {
                MessageBox.Show(this, "Hãy nhập lý do từ chối vào ô ghi chú.", "Từ chối yêu cầu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtGhiChu.Focus();
                return;
            }
            if (duyet && MessageBox.Show(this, "Áp dụng yêu cầu #" + y.Id + " (" + y.LoaiHienThi + " · " + y.TenDonVi + ") vào CSDL?",
                    "Duyệt yêu cầu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                string kq = await Task.Run(() => duyet ? dal.Duyet(y.Id, ghiChu) : dal.TuChoi(y.Id, ghiChu));
                txtGhiChu.Clear();
                DatTrangThai(kq);
                if (kq != null && kq.StartsWith("LỖI", StringComparison.Ordinal))
                    MessageBox.Show(this, kq, "Không áp dụng được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TaiYeuCau();
                if (duyet && DaDuyetThanhCong != null) DaDuyetThanhCong();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Bao FormMain nap lai ban do/du lieu sau khi duyet.</summary>
        public event Action DaDuyetThanhCong;

        // =================================================
        // 2. NHAT KY
        // =================================================

        private async void TaiNhatKy()
        {
            DatTrangThai("Đang tải nhật ký…");
            try
            {
                nhatKy = await Task.Run(() => dal.LayNhatKy(2000));
                LocNhatKy();
                DatTrangThai("Đã tải " + nhatKy.Count + " dòng nhật ký (mới nhất trước)");
            }
            catch (Exception ex) { DatTrangThai("Lỗi tải nhật ký: " + ex.Message); }
        }

        private void LocNhatKy()
        {
            string k = txtLocNhatKy.Text.Trim();
            List<DongNhatKy> ds = k.Length == 0 ? nhatKy : nhatKy.Where(x =>
                Chua(x.MaDonVi, k) || Chua(x.Bang, k) || Chua(x.TaiKhoan, k) || Chua(x.ThaoTac, k) || Chua(x.Nguon, k)).ToList();
            gridNhatKy.DataSource = new BindingList<DongNhatKy>(ds);
            gridNhatKy.ClearSelection();
            HienChiTietNhatKy();
        }

        private void HienChiTietNhatKy()
        {
            DongNhatKy d = gridNhatKy.CurrentRow != null && gridNhatKy.CurrentRow.Selected
                ? gridNhatKy.CurrentRow.DataBoundItem as DongNhatKy : null;
            if (d == null) { gridNhatKyChiTiet.DataSource = null; return; }
            List<Tuple<string, string, string>> kb = (d.DuLieuCu.Length > 0 || d.DuLieuMoi.Length > 0)
                ? SoSanh(d.DuLieuCu, d.DuLieuMoi)
                : new List<Tuple<string, string, string>>
                {
                    Tuple.Create("Bảng", "", d.Bang),
                    Tuple.Create("Thao tác", "", d.ThaoTac),
                    Tuple.Create("Tài khoản CSDL", "", d.TaiKhoan),
                    Tuple.Create("Ứng dụng", "", d.UngDung),
                    Tuple.Create("Ghi chú", "", "Nhật ký trigger chỉ lưu thông tin giao dịch")
                };
            gridNhatKyChiTiet.DataSource = kb;
        }

        // =================================================
        // 3. SAO LUU
        // =================================================

        private static string TepCauHinhThuMuc
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ManagerGISAcademy", "thu-muc-sao-luu.txt"); }
        }

        private static string DocThuMucSaoLuu()
        {
            try
            {
                if (File.Exists(TepCauHinhThuMuc))
                {
                    string t = File.ReadAllText(TepCauHinhThuMuc).Trim();
                    if (Directory.Exists(t)) return t;
                }
            }
            catch { }
            string[] ung = { @"D:\aNAM5\HK1\HeQuanTriCSDLKG\BTLHQTCSDLKG\_QUAN_LY_PHAT_TRIEN\06_SAO_LUU_CSDL" };
            foreach (string u in ung) if (Directory.Exists(u)) return u;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManagerGISAcademy", "SaoLuu");
        }

        private void TaiDanhSachSaoLuu()
        {
            lblThuMuc.Text = "Thư mục: " + thuMucSaoLuu + "   ·   tài khoản pg_dump: " + TaiKhoanSaoLuu();
            gridSaoLuu.DataSource = new BindingList<TepSaoLuu>(TruyXuatQuanTri.LietKeSaoLuu(thuMucSaoLuu));
        }

        private static string TaiKhoanSaoLuu()
        {
            string h, p, c, u;
            try { TruyXuatQuanTri.LayThongSoMayChu(out h, out p, out c, out u); return u + "@" + h + ":" + p + "/" + c; }
            catch { return "?"; }
        }

        private void GhiLog(string s)
        {
            txtNhatKySaoLuu.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + s + Environment.NewLine);
        }

        private TepSaoLuu SaoLuuDangChon()
        {
            return gridSaoLuu.CurrentRow != null ? gridSaoLuu.CurrentRow.DataBoundItem as TepSaoLuu : null;
        }

        private async Task SaoLuu()
        {
            string h, p, c, u;
            TruyXuatQuanTri.LayThongSoMayChu(out h, out p, out c, out u);
            Directory.CreateDirectory(thuMucSaoLuu);
            string tep = Path.Combine(thuMucSaoLuu, c + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".backup");
            GhiLog("pg_dump -Fc " + c + " → " + Path.GetFileName(tep));
            DatTrangThai("Đang sao lưu…");
            Stopwatch dh = Stopwatch.StartNew();
            try
            {
                Tuple<int, string> kq = await Task.Run(() => TruyXuatQuanTri.ChayCongCu("pg_dump.exe",
                    string.Format("-w -h {0} -p {1} -U {2} -Fc -f \"{3}\" {4}", h, p, u, tep, c), 600));
                if (kq.Item1 != 0) { GhiLog("LỖI: " + kq.Item2); DatTrangThai("Sao lưu thất bại"); return; }
                GhiLog("Xong trong " + (dh.ElapsedMilliseconds / 1000d).ToString("0.0", Vi) + " s · " +
                       (new FileInfo(tep).Length / 1048576d).ToString("N2", Vi) + " MB");
                DatTrangThai("Đã sao lưu " + Path.GetFileName(tep));
                TaiDanhSachSaoLuu();
            }
            catch (Exception ex) { GhiLog("LỖI: " + ex.Message); }
        }

        private async Task KiemTraSaoLuu()
        {
            TepSaoLuu t = SaoLuuDangChon();
            if (t == null) return;
            GhiLog("pg_restore --list " + t.Ten);
            try
            {
                Tuple<int, string> kq = await Task.Run(() => TruyXuatQuanTri.ChayCongCu("pg_restore.exe", "--list \"" + t.DuongDan + "\"", 120));
                if (kq.Item1 != 0) { GhiLog("TỆP HỎNG: " + kq.Item2); return; }
                string[] dong = kq.Item2.Split('\n');
                int bang = dong.Count(x => x.Contains(" TABLE ") && !x.Contains("TABLE DATA"));
                int duLieu = dong.Count(x => x.Contains("TABLE DATA"));
                int ham = dong.Count(x => x.Contains(" FUNCTION "));
                int cs = dong.Count(x => x.Contains(" POLICY "));
                GhiLog("Tệp hợp lệ: " + dong.Count(x => x.Length > 0 && !x.StartsWith(";")) + " mục · " + bang + " bảng · " +
                       duLieu + " khối dữ liệu · " + ham + " hàm · " + cs + " chính sách RLS");
            }
            catch (Exception ex) { GhiLog("LỖI: " + ex.Message); }
        }

        private async Task KhoiPhuc()
        {
            TepSaoLuu t = SaoLuuDangChon();
            if (t == null) return;
            string h, p, c, u;
            TruyXuatQuanTri.LayThongSoMayChu(out h, out p, out c, out u);
            string moi = "HQTCSDL_KP_" + DateTime.Now.ToString("yyyyMMdd_HHmm");
            if (MessageBox.Show(this,
                    "Khôi phục \"" + t.Ten + "\" sang CSDL MỚI \"" + moi + "\".\n\n" +
                    "CSDL đang dùng (" + c + ") KHÔNG bị thay đổi. Sau khi kiểm tra, có thể đổi Database trong chuỗi kết nối để dùng bản khôi phục.\n\nTiếp tục?",
                    "Khôi phục sao lưu", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            GhiLog("CREATE DATABASE " + moi);
            try
            {
                Tuple<int, string> k1 = await Task.Run(() => TruyXuatQuanTri.ChayCongCu("psql.exe",
                    string.Format("-w -h {0} -p {1} -U {2} -d postgres -c \"CREATE DATABASE \\\"{3}\\\"\"", h, p, u, moi), 120));
                if (k1.Item1 != 0) { GhiLog("LỖI tạo CSDL: " + k1.Item2); return; }
                GhiLog("pg_restore → " + moi + " (có thể mất 1–2 phút)…");
                Tuple<int, string> k2 = await Task.Run(() => TruyXuatQuanTri.ChayCongCu("pg_restore.exe",
                    string.Format("-w -h {0} -p {1} -U {2} -d \"{3}\" --no-owner \"{4}\"", h, p, u, moi, t.DuongDan), 900));
                GhiLog(k2.Item1 == 0 ? "Khôi phục xong: " + moi : "Khôi phục xong kèm cảnh báo (mã " + k2.Item1 + "): " + Rut(k2.Item2, 600));
                DatTrangThai("Đã khôi phục sang " + moi);
            }
            catch (Exception ex) { GhiLog("LỖI: " + ex.Message); }
        }

        private void XoaSaoLuu()
        {
            TepSaoLuu t = SaoLuuDangChon();
            if (t == null) return;
            if (MessageBox.Show(this, "Xóa vĩnh viễn tệp sao lưu \"" + t.Ten + "\" (" + t.KichThuoc + ")?\nThao tác này không hoàn tác được.",
                    "Xóa tệp sao lưu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try { File.Delete(t.DuongDan); GhiLog("Đã xóa " + t.Ten); TaiDanhSachSaoLuu(); }
            catch (Exception ex) { GhiLog("LỖI xóa: " + ex.Message); }
        }

        private void DoiThuMuc()
        {
            using (FolderBrowserDialog d = new FolderBrowserDialog { Description = "Chọn thư mục lưu bản sao lưu CSDL", SelectedPath = thuMucSaoLuu })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                thuMucSaoLuu = d.SelectedPath;
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(TepCauHinhThuMuc));
                    File.WriteAllText(TepCauHinhThuMuc, thuMucSaoLuu);
                }
                catch { }
                TaiDanhSachSaoLuu();
            }
        }

        // =================================================
        // 4. KIEM THU
        // =================================================

        private async Task ChayKiemThu()
        {
            lblTomTatKiemThu.Text = "Đang chạy bộ kiểm thử…";
            lblTomTatKiemThu.ForeColor = BangGiaoDien.Cam;
            Stopwatch dh = Stopwatch.StartNew();
            try
            {
                List<KetQuaKiemThu> ds = await Task.Run(() => dal.ChayKiemThu());
                gridKiemThu.DataSource = new BindingList<KetQuaKiemThu>(ds);
                gridKiemThu.ClearSelection();
                int dat = ds.Count(x => x.Dat);
                lblTomTatKiemThu.Text = (dat == ds.Count ? "✓ " : "✕ ") + dat + "/" + ds.Count + " ca đạt · " +
                    (dh.ElapsedMilliseconds / 1000d).ToString("0.00", Vi) + " giây · dữ liệu kiểm thử đã tự hoàn tác";
                lblTomTatKiemThu.ForeColor = dat == ds.Count ? VeBaoCao.Teal : BangGiaoDien.Do;
            }
            catch (Exception ex)
            {
                lblTomTatKiemThu.Text = "Lỗi chạy kiểm thử: " + ex.Message;
                lblTomTatKiemThu.ForeColor = BangGiaoDien.Do;
            }
        }

        // =================================================
        // 5. HIEU NANG
        // =================================================

        private async Task DoHieuNang()
        {
            DatTrangThai("Đang đo EXPLAIN ANALYZE…");
            try
            {
                List<KetQuaHieuNang> ds = await Task.Run(() => dal.DoHieuNang());
                gridHieuNang.DataSource = new BindingList<KetQuaHieuNang>(ds);
                gridHieuNang.ClearSelection();
                DatTrangThai("Đã đo " + ds.Count + " truy vấn");
                TaiKichThuoc();
            }
            catch (Exception ex) { DatTrangThai("Lỗi đo hiệu năng: " + ex.Message); }
        }

        private async void TaiKichThuoc()
        {
            try
            {
                List<KichThuocBang> kt = await Task.Run(() => dal.LayKichThuocBang());
                List<ThongTinChiMuc> cm = await Task.Run(() => dal.LayChiMuc());
                gridKichThuoc.DataSource = new BindingList<KichThuocBang>(kt);
                gridChiMuc.DataSource = new BindingList<ThongTinChiMuc>(cm);
                gridKichThuoc.ClearSelection(); gridChiMuc.ClearSelection();
            }
            catch (Exception ex) { DatTrangThai("Lỗi đọc kích thước: " + ex.Message); }
        }

        private async Task LamMoiThongKe()
        {
            try { DatTrangThai(await Task.Run(() => dal.LamMoiThongKe())); TaiKichThuoc(); }
            catch (Exception ex) { DatTrangThai("Lỗi: " + ex.Message); }
        }

        // =================================================
        // TIEN ICH
        // =================================================

        private static readonly HashSet<string> TruongBo = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { "geom_4326", "geom_32648", "phien_ban", "ngay_cap_nhat", "x_vn2000", "y_vn2000", "id" };

        private static readonly Dictionary<string, string> TenTruong = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "ma_don_vi", "Mã đơn vị" }, { "ten_don_vi", "Tên đơn vị" }, { "ten_viet_tat", "Tên viết tắt" },
            { "loai_don_vi", "Loại đơn vị" }, { "quan_binh_chung", "Quân/Binh chủng" }, { "don_vi_cap_tren", "Đơn vị cấp trên" },
            { "quan_huyen", "Phường/xã" }, { "dia_chi", "Địa chỉ" }, { "kinh_do", "Kinh độ" }, { "vi_do", "Vĩ độ" },
            { "chi_huy_truong", "Chỉ huy trưởng" }, { "chinh_uy", "Chính ủy" }, { "so_dien_thoai", "Số điện thoại" },
            { "trang_thai_sscd", "Trạng thái SSCĐ" }, { "do_mat", "Độ mật" }, { "nhiem_vu_chinh", "Nhiệm vụ chính" },
            { "vu_khi_trang_bi", "Vũ khí trang bị" }, { "vi_tri_phuong_phap", "Phương pháp vị trí" },
            { "vi_tri_do_chinh_xac_m", "Độ chính xác (m)" }, { "vi_tri_nguon", "Nguồn vị trí" }
        };

        private static List<Tuple<string, string, string>> SoSanh(string jsonCu, string jsonMoi)
        {
            Dictionary<string, string> cu = PhangJson(jsonCu), moi = PhangJson(jsonMoi);
            List<Tuple<string, string, string>> kq = new List<Tuple<string, string, string>>();
            foreach (string k in cu.Keys.Union(moi.Keys))
            {
                if (TruongBo.Contains(k)) continue;
                string a = Lay(cu, k), b = Lay(moi, k);
                if (jsonCu.Length > 0 && jsonMoi.Length > 0 && a == b) continue;   // chi hien truong thay doi
                if (a.Length == 0 && b.Length == 0) continue;
                string ten;
                kq.Add(Tuple.Create(TenTruong.TryGetValue(k, out ten) ? ten : k, a, b));
            }
            if (kq.Count == 0) kq.Add(Tuple.Create("(không có trường thay đổi)", "", ""));
            return kq;
        }

        private static Dictionary<string, string> PhangJson(string json)
        {
            Dictionary<string, string> d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(json)) return d;
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.ValueKind != JsonValueKind.Object) return d;
                    foreach (JsonProperty p in doc.RootElement.EnumerateObject())
                    {
                        string v;
                        switch (p.Value.ValueKind)
                        {
                            case JsonValueKind.Null: v = ""; break;
                            case JsonValueKind.String: v = p.Value.GetString(); break;
                            case JsonValueKind.Number:
                                double so;
                                v = p.Value.TryGetDouble(out so) ? so.ToString("0.######", CultureInfo.InvariantCulture) : p.Value.GetRawText();
                                break;
                            default: v = p.Value.GetRawText(); break;
                        }
                        d[p.Name] = v ?? "";
                    }
                }
            }
            catch { }
            return d;
        }

        private static string Lay(Dictionary<string, string> d, string k)
        {
            string v;
            return d.TryGetValue(k, out v) ? v : "";
        }

        private static bool Chua(string s, string k)
        {
            return CultureInfo.GetCultureInfo("vi-VN").CompareInfo.IndexOf(s ?? "", k,
                CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) >= 0;
        }

        private static string Rut(string s, int n) { return s == null || s.Length <= n ? s : s.Substring(0, n) + "…"; }

        private static Tuple<Color, Color> MauTrangThai(string s)
        {
            switch (s)
            {
                case "Chờ duyệt": return Tuple.Create(BangGiaoDien.CamNhat, BangGiaoDien.Cam);
                case "Đã duyệt": return Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal);
                case "Từ chối": return Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do);
                default: return Tuple.Create(Color.FromArgb(238, 238, 242), Color.FromArgb(100, 100, 110));
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private static void GoiY(TextBox t, string chu)
        {
            SendMessage(t.Handle, 0x1501, (IntPtr)1, chu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) henLoc.Dispose();
            base.Dispose(disposing);
        }

        // =================================================
        // THE VAI TRO (tu ve)
        // =================================================

        private sealed class TheVaiTro : Control
        {
            private readonly string tieuDe, moTa;
            private readonly Color mau;
            private bool dangDung, diChuot;

            public bool DangDung { get { return dangDung; } set { dangDung = value; Invalidate(); } }

            public TheVaiTro(string tieuDe, string moTa, Color mau)
            {
                this.tieuDe = tieuDe; this.moTa = moTa; this.mau = mau;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = TienIchGiaoDienSang.Background;
                Cursor = PhienDangNhap.HeThongSanSang ? Cursors.Default : Cursors.Hand;
            }

            protected override void OnMouseEnter(EventArgs e) { diChuot = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { diChuot = false; Invalidate(); base.OnMouseLeave(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                RectangleF r = new RectangleF(1, 1, Width - 3, Height - 3);
                Color nen = dangDung ? Color.FromArgb(Math.Min(255, mau.R / 10 + 230), Math.Min(255, mau.G / 10 + 230), Math.Min(255, mau.B / 10 + 230)) : Color.White;
                using (GraphicsPath p = VeBaoCao.BoTron(r, 14))
                using (SolidBrush b = new SolidBrush(nen))
                using (Pen pen = new Pen(dangDung ? mau : (diChuot ? Color.FromArgb(170, 200, 225) : Color.FromArgb(228, 236, 245)), dangDung ? 2f : 1.2f))
                {
                    g.FillPath(b, p);
                    g.DrawPath(pen, p);
                }
                using (SolidBrush b = new SolidBrush(mau))
                    g.FillEllipse(b, 18, 18, 14, 14);
                using (Font ft = new Font("Segoe UI", 13F, FontStyle.Bold))
                using (Font fm = new Font("Segoe UI", 9F))
                using (Font fb = new Font("Segoe UI", 8F, FontStyle.Bold))
                using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
                using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
                {
                    g.DrawString(tieuDe, ft, ink, 40, 11);
                    g.DrawString(moTa, fm, sub, new RectangleF(18, 46, Width - 36, Height - 80));
                    string nhan = dangDung ? "●  VAI TRÒ CỦA BẠN" : (PhienDangNhap.HeThongSanSang ? "Cấp cho tài khoản ở thẻ Tài khoản" : "Bấm để chuyển sang vai trò này");
                    using (SolidBrush b = new SolidBrush(dangDung ? mau : VeBaoCao.Sub))
                        g.DrawString(nhan, dangDung ? fb : fm, b, 18, Height - 30);
                }
            }
        }
    }
}
