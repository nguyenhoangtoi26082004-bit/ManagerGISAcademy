using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Dang nhap / Dang ky. Mat khau chi di qua tham so toi ham CSDL
    /// (ql_he_thong.fn_dang_nhap / fn_dang_ky) – ung dung khong luu, khong ghi log mat khau.
    /// DialogResult.OK + TaiKhoan != null khi dang nhap thanh cong.
    /// </summary>
    public sealed class FormDangNhap : Form
    {
        private static readonly Color Teal = Color.FromArgb(0, 150, 122), Xanh = Color.FromArgb(39, 106, 205),
            Muc = Color.FromArgb(22, 49, 74), Xam = Color.FromArgb(104, 122, 142), Vien = Color.FromArgb(208, 220, 232),
            NenO = Color.FromArgb(248, 251, 253), Do = Color.FromArgb(200, 55, 70);

        public TaiKhoanDangNhap TaiKhoan { get; private set; }

        private readonly Panel trangDn, trangDk;
        private readonly Button tabDn, tabDk, btnDangNhap, btnDangKy;
        private readonly TextBox txtTen, txtMk, kTen, kHoTen, kDonVi, kEmail, kMk, kMk2, kLyDo;
        private readonly CheckBox chkNho;
        private readonly Label lblDn, lblDk, lblGoiYDk, lblDoManh;
        private readonly Panel thanhDoManh;
        private int doManh;
        private static readonly string TepNho = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ManagerGISAcademy", "ten-dang-nhap.txt");

        public FormDangNhap(bool moTrangDangKy = false)
        {
            Text = "Đăng nhập hệ thống";
            Font = new Font("Segoe UI", 9.5F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false; MinimizeBox = false;
            ShowIcon = false; ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(880, 590);
            BackColor = Color.White;
            DoubleBuffered = true;
            KeyPreview = true;

            // ---------- Dai thuong hieu ben trai ----------
            DaiThuongHieu trai = new DaiThuongHieu { Dock = DockStyle.Left, Width = 330 };
            Panel phai = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(44, 26, 44, 18) };
            Controls.Add(phai);
            Controls.Add(trai);

            // ---------- The chuyen Dang nhap / Dang ky ----------
            Panel thanhTab = new Panel { Dock = DockStyle.Top, Height = 46, BackColor = Color.White };
            KhungBoGocChung khungTab = new KhungBoGocChung { Location = new Point(0, 4), Size = new Size(300, 40), Padding = new Padding(4), BackColor = Color.White };
            tabDn = NutTab("Đăng nhập");
            tabDk = NutTab("Đăng ký tài khoản");
            tabDn.SetBounds(4, 4, 140, 32);
            tabDk.SetBounds(148, 4, 148, 32);
            khungTab.Controls.Add(tabDn); khungTab.Controls.Add(tabDk);
            thanhTab.Controls.Add(khungTab);

            trangDn = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            trangDk = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Visible = false, AutoScroll = true };
            phai.Controls.Add(trangDk);
            phai.Controls.Add(trangDn);
            phai.Controls.Add(thanhTab);

            // ================= TRANG DANG NHAP =================
            int y = 26;
            trangDn.Controls.Add(NhanAt("Chào mừng trở lại", 18F, true, Muc, 0, y, 460, 36)); y += 40;
            trangDn.Controls.Add(NhanAt("Đăng nhập để làm việc theo vai trò được cấp. Khách chỉ được xem bản đồ.", 9.5F, false, Xam, 0, y, 450, 22)); y += 44;
            txtTen = ONhap(trangDn, "Tên đăng nhập", 0, ref y, false, "vd: nguyen.van.a");
            txtMk = ONhap(trangDn, "Mật khẩu", 0, ref y, true, null);
            chkNho = new CheckBox { Text = "Ghi nhớ tên đăng nhập trên máy này", ForeColor = Xam, Location = new Point(0, y - 4), AutoSize = true };
            trangDn.Controls.Add(chkNho); y += 26;
            lblDn = NhanAt("", 9.2F, false, Do, 0, y, 450, 40); trangDn.Controls.Add(lblDn); y += 44;
            btnDangNhap = NutChinh("ĐĂNG NHẬP", Teal, 0, y, 450); trangDn.Controls.Add(btnDangNhap); y += 58;
            LinkLabel lienKetDk = Lien("Chưa có tài khoản?  Đăng ký ngay  →", 0, y); trangDn.Controls.Add(lienKetDk);
            LinkLabel lienKetKhach = Lien("Tiếp tục với quyền khách (chỉ xem)", 0, y + 26); lienKetKhach.LinkColor = Xam; trangDn.Controls.Add(lienKetKhach);

            // ================= TRANG DANG KY =================
            y = 6;
            trangDk.Controls.Add(NhanAt("Tạo tài khoản mới", 18F, true, Muc, 0, y, 460, 36)); y += 40;
            lblGoiYDk = NhanAt("Đăng ký sẽ được quản trị viên duyệt và cấp vai trò trước khi đăng nhập.", 9.2F, false, Xam, 0, y, 450, 36);
            trangDk.Controls.Add(lblGoiYDk); y += 40;
            kHoTen = ONhapCot(trangDk, "Họ và tên *", 0, y, 219, false);
            kTen = ONhapCot(trangDk, "Tên đăng nhập *", 231, y, 219, false); y += 62;
            kDonVi = ONhapCot(trangDk, "Đơn vị công tác", 0, y, 219, false);
            kEmail = ONhapCot(trangDk, "Email", 231, y, 219, false); y += 62;
            kMk = ONhapCot(trangDk, "Mật khẩu * (≥ 8 ký tự, có chữ và số)", 0, y, 219, true);
            kMk2 = ONhapCot(trangDk, "Nhập lại mật khẩu *", 231, y, 219, true); y += 60;
            thanhDoManh = new Panel { Location = new Point(0, y), Size = new Size(219, 5), BackColor = Color.FromArgb(232, 238, 244) };
            thanhDoManh.Paint += (s, e) =>
            {
                Color m = doManh <= 1 ? Do : doManh == 2 ? Color.FromArgb(230, 150, 30) : doManh == 3 ? Color.FromArgb(120, 180, 40) : Teal;
                using (SolidBrush b = new SolidBrush(m)) e.Graphics.FillRectangle(b, 0, 0, thanhDoManh.Width * doManh / 4, 5);
            };
            trangDk.Controls.Add(thanhDoManh);
            lblDoManh = NhanAt("", 8.5F, false, Xam, 231, y - 6, 219, 18); trangDk.Controls.Add(lblDoManh); y += 14;
            kLyDo = ONhapCot(trangDk, "Lý do / nhiệm vụ cần dùng hệ thống", 0, y, 450, false); y += 62;
            lblDk = NhanAt("", 9.2F, false, Do, 0, y, 450, 38); trangDk.Controls.Add(lblDk); y += 40;
            btnDangKy = NutChinh("GỬI ĐĂNG KÝ", Xanh, 0, y, 450); trangDk.Controls.Add(btnDangKy); y += 54;
            LinkLabel lienKetDn = Lien("←  Đã có tài khoản? Đăng nhập", 0, y); trangDk.Controls.Add(lienKetDn);

            // ---------- Su kien ----------
            tabDn.Click += (s, e) => ChuyenTrang(false);
            tabDk.Click += (s, e) => ChuyenTrang(true);
            lienKetDk.LinkClicked += (s, e) => ChuyenTrang(true);
            lienKetDn.LinkClicked += (s, e) => ChuyenTrang(false);
            lienKetKhach.LinkClicked += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnDangNhap.Click += async (s, e) => await DangNhap();
            btnDangKy.Click += async (s, e) => await DangKy();
            kMk.TextChanged += (s, e) => DanhGiaMatKhau();
            kTen.TextChanged += (s, e) =>
            {
                int c = kTen.SelectionStart;
                string t = kTen.Text.ToLowerInvariant();
                if (t != kTen.Text) { kTen.Text = t; kTen.SelectionStart = Math.Min(c, t.Length); }
            };
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) { DialogResult = DialogResult.Cancel; Close(); }
                if (e.KeyCode == Keys.Enter && !(ActiveControl is LinkLabel))
                {
                    e.Handled = e.SuppressKeyPress = true;
                    if (trangDn.Visible) btnDangNhap.PerformClick();
                    else if (ActiveControl != kLyDo) SelectNextControl(ActiveControl, true, true, true, true);
                    else btnDangKy.PerformClick();
                }
            };

            try
            {
                if (File.Exists(TepNho))
                {
                    txtTen.Text = File.ReadAllText(TepNho).Trim();
                    chkNho.Checked = txtTen.Text.Length > 0;
                }
            }
            catch { }

            Shown += async (s, e) =>
            {
                ChuyenTrang(moTrangDangKy);
                try
                {
                    bool coQt = await Task.Run(() => new TruyXuatTaiKhoan().CoQuanTriHoatDong());
                    if (!coQt)
                    {
                        lblGoiYDk.Text = "⚑  Hệ thống chưa có quản trị viên: tài khoản đăng ký ĐẦU TIÊN sẽ được cấp vai trò QUẢN TRỊ và dùng được ngay.";
                        lblGoiYDk.ForeColor = Color.FromArgb(176, 106, 20);
                        if (!moTrangDangKy) { lblDn.ForeColor = Color.FromArgb(176, 106, 20); lblDn.Text = "Chưa có tài khoản quản trị nào – hãy đăng ký tài khoản đầu tiên."; }
                    }
                }
                catch (Exception ex) { lblDn.Text = "Không kết nối được CSDL: " + ex.Message; }
            };
        }

        // =====================================================
        // XU LY
        // =====================================================

        private void ChuyenTrang(bool dangKy)
        {
            trangDk.Visible = dangKy;
            trangDn.Visible = !dangKy;
            DatTab(tabDn, !dangKy);
            DatTab(tabDk, dangKy);
            Text = dangKy ? "Đăng ký tài khoản" : "Đăng nhập hệ thống";
            AcceptButton = null;
            if (dangKy) kHoTen.Focus();
            else if (txtTen.Text.Length > 0) txtMk.Focus(); else txtTen.Focus();
        }

        private static void DatTab(Button b, bool chon)
        {
            b.BackColor = chon ? Teal : Color.White;
            b.ForeColor = chon ? Color.White : Xam;
            b.FlatAppearance.MouseOverBackColor = chon ? Teal : Color.FromArgb(236, 246, 243);
        }

        private async Task DangNhap()
        {
            string ten = txtTen.Text.Trim().ToLowerInvariant(), mk = txtMk.Text;
            if (ten.Length == 0 || mk.Length == 0) { BaoDn("Nhập tên đăng nhập và mật khẩu.", true); return; }
            DatBan(true, btnDangNhap, "ĐANG KIỂM TRA…");
            try
            {
                KetQuaTaiKhoan kq = await Task.Run(() => new TruyXuatTaiKhoan().DangNhap(ten, mk));
                if (!kq.ThanhCong || kq.TaiKhoan == null)
                {
                    BaoDn(kq.ThongBao, true);
                    txtMk.SelectAll(); txtMk.Focus();
                    return;
                }
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(TepNho));
                    if (chkNho.Checked) File.WriteAllText(TepNho, kq.TaiKhoan.TenDangNhap);
                    else if (File.Exists(TepNho)) File.Delete(TepNho);
                }
                catch { }
                TaiKhoan = kq.TaiKhoan;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex) { BaoDn("Lỗi kết nối CSDL: " + ex.Message, true); }
            finally { if (!IsDisposed) DatBan(false, btnDangNhap, "ĐĂNG NHẬP"); txtMk.Text = TaiKhoan == null ? txtMk.Text : ""; }
        }

        private async Task DangKy()
        {
            string ten = kTen.Text.Trim().ToLowerInvariant();
            if (kHoTen.Text.Trim().Length < 3) { BaoDk("Nhập họ và tên (ít nhất 3 ký tự).", true); kHoTen.Focus(); return; }
            if (!System.Text.RegularExpressions.Regex.IsMatch(ten, "^[a-z0-9._]{4,40}$"))
            { BaoDk("Tên đăng nhập 4–40 ký tự: chữ thường không dấu, số, dấu chấm, gạch dưới.", true); kTen.Focus(); return; }
            if (kMk.Text != kMk2.Text) { BaoDk("Hai lần nhập mật khẩu không khớp.", true); kMk2.Focus(); return; }
            if (doManh < 2) { BaoDk("Mật khẩu quá yếu: cần ≥ 8 ký tự, có cả chữ và số.", true); kMk.Focus(); return; }
            DatBan(true, btnDangKy, "ĐANG GỬI…");
            try
            {
                string hoTen = kHoTen.Text.Trim(), mk = kMk.Text, dv = kDonVi.Text.Trim(), em = kEmail.Text.Trim(), ld = kLyDo.Text.Trim();
                KetQuaTaiKhoan kq = await Task.Run(() => new TruyXuatTaiKhoan().DangKy(ten, hoTen, mk, dv, em, ld));
                if (!kq.ThanhCong) { BaoDk(kq.ThongBao, true); return; }
                kMk.Text = kMk2.Text = "";
                txtTen.Text = ten;
                txtMk.Text = "";
                ChuyenTrang(false);
                BaoDn("✓  " + kq.ThongBao, false);
            }
            catch (Exception ex) { BaoDk("Lỗi kết nối CSDL: " + ex.Message, true); }
            finally { if (!IsDisposed) DatBan(false, btnDangKy, "GỬI ĐĂNG KÝ"); }
        }

        private void DanhGiaMatKhau()
        {
            string m = kMk.Text;
            int d = 0;
            if (m.Length >= 8) d++;
            if (System.Text.RegularExpressions.Regex.IsMatch(m, "[A-Za-z]") && System.Text.RegularExpressions.Regex.IsMatch(m, "[0-9]")) d++;
            if (m.Length >= 12) d++;
            if (System.Text.RegularExpressions.Regex.IsMatch(m, "[^A-Za-z0-9]") && System.Text.RegularExpressions.Regex.IsMatch(m, "[A-Z]")) d++;
            if (m.Length < 8 || d < 2) d = Math.Min(d, 1);
            doManh = m.Length == 0 ? 0 : Math.Max(1, d);
            string[] ten = { "", "Yếu", "Trung bình", "Khá", "Mạnh" };
            lblDoManh.Text = m.Length == 0 ? "" : "Độ mạnh: " + ten[doManh];
            thanhDoManh.Invalidate();
        }

        private void BaoDn(string s, bool loi) { lblDn.ForeColor = loi ? Do : Teal; lblDn.Text = s ?? ""; }
        private void BaoDk(string s, bool loi) { lblDk.ForeColor = loi ? Do : Teal; lblDk.Text = s ?? ""; }

        private void DatBan(bool ban, Button b, string chu)
        {
            b.Enabled = !ban;
            b.Text = chu;
            UseWaitCursor = ban;
        }

        // =====================================================
        // DUNG GIAO DIEN
        // =====================================================

        private static Label NhanAt(string chu, float co, bool dam, Color mau, int x, int y, int w, int h)
        {
            return new Label
            {
                Text = chu, Font = new Font("Segoe UI", co, dam ? FontStyle.Bold : FontStyle.Regular), ForeColor = mau,
                Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.Transparent, UseMnemonic = false
            };
        }

        private static Button NutTab(string chu)
        {
            Button b = new Button { Text = chu, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), Cursor = Cursors.Hand, TabStop = false };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private static Button NutChinh(string chu, Color mau, int x, int y, int w)
        {
            Button b = new Button
            {
                Text = chu, BackColor = mau, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold), Cursor = Cursors.Hand,
                Location = new Point(x, y), Size = new Size(w, 44), UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(mau, 0.15f);
            b.Paint += (s, e) => { };
            return b;
        }

        private static LinkLabel Lien(string chu, int x, int y)
        {
            LinkLabel l = new LinkLabel
            {
                Text = chu, AutoSize = true, Location = new Point(x, y), LinkColor = Xanh, ActiveLinkColor = Teal,
                LinkBehavior = LinkBehavior.HoverUnderline, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            return l;
        }

        /// <summary>O nhap 1 cot (nhan tren, khung bo goc duoi), tang y.</summary>
        private TextBox ONhap(Panel cha, string nhan, int x, ref int y, bool matKhau, string goiY)
        {
            TextBox t = ONhapCot(cha, nhan, x, y, 450, matKhau);
            if (!string.IsNullOrEmpty(goiY)) GoiY(t, goiY);
            y += 66;
            return t;
        }

        private TextBox ONhapCot(Panel cha, string nhan, int x, int y, int w, bool matKhau)
        {
            cha.Controls.Add(NhanAt(nhan, 8.8F, true, Muc, x, y, w, 18));
            KhungBoGocChung k = new KhungBoGocChung { Location = new Point(x, y + 20), Size = new Size(w, 36), Padding = new Padding(10, 9, matKhau ? 34 : 8, 4), BackColor = NenO };
            TextBox t = new TextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10.5F), BackColor = NenO, ForeColor = Muc, UseSystemPasswordChar = matKhau, MaxLength = matKhau ? 72 : 200 };
            k.Controls.Add(t);
            if (matKhau)
            {
                Label mat = new Label
                {
                    Text = "👁", Font = new Font("Segoe UI Emoji", 10F), ForeColor = Xam, Cursor = Cursors.Hand,
                    Size = new Size(26, 26), Location = new Point(w - 32, 5), TextAlign = ContentAlignment.MiddleCenter, BackColor = NenO
                };
                mat.Click += (s, e) => { t.UseSystemPasswordChar = !t.UseSystemPasswordChar; mat.ForeColor = t.UseSystemPasswordChar ? Xam : Teal; t.Focus(); };
                k.Controls.Add(mat);
                mat.BringToFront();
            }
            cha.Controls.Add(k);
            return t;
        }

        private static void GoiY(TextBox t, string goiY)
        {
            // Placeholder don gian (EM_SETCUEBANNER).
            t.HandleCreated += (s, e) => SendMessage(t.Handle, 0x1501, (IntPtr)1, goiY);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, string l);

        // =====================================================
        // DAI THUONG HIEU BEN TRAI
        // =====================================================
        private sealed class DaiThuongHieu : Control
        {
            private readonly Image logo;

            public DaiThuongHieu()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                try
                {
                    string p = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "logohocvien.png");
                    if (File.Exists(p)) using (Bitmap a = new Bitmap(p)) logo = CatHuyHieu(a);
                }
                catch { logo = null; }
            }

            /// <summary>
            /// Anh goc la hinh chu nhat nen trang, huy hieu tron o giua.
            /// Tim khung bao phan khong phai nen trang, cat thanh hinh VUONG
            /// can giua de ve vao khung tron khong bi meo / lech.
            /// </summary>
            private static Bitmap CatHuyHieu(Bitmap a)
            {
                int x0 = a.Width, y0 = a.Height, x1 = -1, y1 = -1;
                for (int y = 0; y < a.Height; y += 2)
                    for (int x = 0; x < a.Width; x += 2)
                    {
                        Color c = a.GetPixel(x, y);
                        if (c.A < 40 || (c.R > 235 && c.G > 235 && c.B > 235)) continue;
                        if (x < x0) x0 = x; if (x > x1) x1 = x;
                        if (y < y0) y0 = y; if (y > y1) y1 = y;
                    }
                if (x1 < 0) return new Bitmap(a);
                int canh = Math.Max(x1 - x0, y1 - y0) + 4;
                int cx = (x0 + x1) / 2, cy = (y0 + y1) / 2;
                Rectangle nguon = new Rectangle(cx - canh / 2, cy - canh / 2, canh, canh);
                Bitmap kq = new Bitmap(canh, canh);
                using (Graphics g = Graphics.FromImage(kq))
                {
                    g.Clear(Color.White);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(a, new Rectangle(0, 0, canh, canh), nguon, GraphicsUnit.Pixel);
                }
                return kq;
            }

            protected override void Dispose(bool disposing) { if (disposing && logo != null) logo.Dispose(); base.Dispose(disposing); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                Rectangle r = ClientRectangle;
                using (LinearGradientBrush lg = new LinearGradientBrush(r, Color.FromArgb(0, 128, 104), Color.FromArgb(28, 70, 150), 60f))
                    g.FillRectangle(lg, r);
                // Hoa van luoi toa do mo (goi y GIS).
                using (Pen p = new Pen(Color.FromArgb(22, 255, 255, 255), 1f))
                {
                    for (int x = 0; x < Width; x += 34) g.DrawLine(p, x, 0, x, Height);
                    for (int y = 0; y < Height; y += 34) g.DrawLine(p, 0, y, Width, y);
                }
                using (Pen p = new Pen(Color.FromArgb(40, 255, 255, 255), 1.4f))
                    for (int i = 0; i < 4; i++) g.DrawEllipse(p, Width - 120 - i * 26, Height - 150 - i * 26, 160 + i * 52, 160 + i * 52);

                int cx = Width / 2;
                if (logo != null)
                {
                    // Vong sang ngoai + nen trang tron, huy hieu ve ben trong (cat tron), dung tam.
                    const int d = 112, le = 5;
                    RectangleF vong = new RectangleF(cx - d / 2f, 46, d, d);
                    using (SolidBrush hao = new SolidBrush(Color.FromArgb(45, 255, 255, 255)))
                        g.FillEllipse(hao, vong.X - 6, vong.Y - 6, vong.Width + 12, vong.Height + 12);
                    using (SolidBrush nen = new SolidBrush(Color.White)) g.FillEllipse(nen, vong);
                    RectangleF anh = new RectangleF(vong.X + le, vong.Y + le, d - 2 * le, d - 2 * le);
                    using (GraphicsPath tron = new GraphicsPath())
                    {
                        tron.AddEllipse(anh);
                        GraphicsState st = g.Save();
                        g.SetClip(tron);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(logo, anh);
                        g.Restore(st);
                    }
                    using (Pen vien = new Pen(Color.FromArgb(230, 249, 81), 2f)) g.DrawEllipse(vien, vong);
                }
                using (Font f1 = new Font("Segoe UI", 13.5F, FontStyle.Bold))
                using (Font f2 = new Font("Segoe UI", 9.5F))
                using (Font f3 = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (Font f4 = new Font("Segoe UI", 8.6F))
                using (SolidBrush trang = new SolidBrush(Color.White))
                using (SolidBrush mo = new SolidBrush(Color.FromArgb(215, 255, 255, 255)))
                using (SolidBrush vang = new SolidBrush(Color.FromArgb(230, 249, 81)))
                using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center })
                {
                    g.DrawString("HỆ THỐNG QUẢN LÝ\nĐƠN VỊ QUÂN ĐỘI", f1, trang, new RectangleF(16, 170, Width - 32, 56), giua);
                    g.DrawString("trên địa bàn Hà Nội · CSDL không gian PostGIS", f2, mo, new RectangleF(16, 228, Width - 32, 22), giua);
                    g.FillRectangle(vang, cx - 28, 258, 56, 3);

                    string[,] vt =
                    {
                        { "Quản trị", "Phân quyền tài khoản, duyệt thay đổi, sao lưu" },
                        { "Biên tập", "Thêm / sửa hồ sơ, gửi yêu cầu chờ duyệt" },
                        { "Chỉ xem", "Tra cứu bản đồ trong phạm vi độ mật cho phép" },
                    };
                    float y = 290;
                    for (int i = 0; i < 3; i++)
                    {
                        using (SolidBrush cham = new SolidBrush(i == 0 ? Color.FromArgb(230, 249, 81) : i == 1 ? Color.FromArgb(140, 220, 255) : Color.FromArgb(200, 255, 255, 255)))
                            g.FillEllipse(cham, 34, y + 5, 9, 9);
                        g.DrawString(vt[i, 0], f3, trang, 52, y);
                        g.DrawString(vt[i, 1], f4, mo, new RectangleF(52, y + 18, Width - 70, 34));
                        y += 58;
                    }
                    g.DrawString("🔒  Mật khẩu băm bcrypt trong PostgreSQL · sai 5 lần khóa 15 phút", f4, mo, new RectangleF(20, Height - 46, Width - 40, 36), giua);
                }
            }
        }
    }

    /// <summary>Doi mat khau (nguoi dung) hoac dat lai mat khau tam (quan tri).</summary>
    public sealed class FormMatKhau : HopThoaiV2
    {
        private readonly TextBox txtCu, txtMoi, txtMoi2;
        private readonly Label lblLoi;
        public string MatKhauCu { get { return txtCu == null ? null : txtCu.Text; } }
        public string MatKhauMoi { get { return txtMoi.Text; } }
        private readonly Func<string, string, string> xuLy;

        /// <param name="xuLyLuu">(cu, moi) -> "OK" hoac thong bao loi; chay tren luong nen.</param>
        public FormMatKhau(string tieuDe, string phuDe, bool canMatKhauCu, Func<string, string, string> xuLyLuu)
            : base(tieuDe, phuDe, new Size(460, canMatKhauCu ? 400 : 330))
        {
            xuLy = xuLyLuu;
            MinimumSize = Size;
            int y = 4;
            Func<string, TextBox> o = nhan =>
            {
                Label l = NhanTd(nhan, 9F, true); l.SetBounds(0, y, 400, 18); Than.Controls.Add(l);
                KhungBoGocChung k = new KhungBoGocChung { Location = new Point(0, y + 20), Size = new Size(410, 36), Padding = new Padding(10, 9, 8, 4), BackColor = Color.FromArgb(248, 251, 253) };
                TextBox t = new TextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10.5F), BackColor = k.BackColor, UseSystemPasswordChar = true, MaxLength = 72 };
                k.Controls.Add(t); Than.Controls.Add(k); y += 64;
                return t;
            };
            if (canMatKhauCu) txtCu = o("Mật khẩu hiện tại");
            txtMoi = o("Mật khẩu mới (≥ 8 ký tự, có chữ và số)");
            txtMoi2 = o("Nhập lại mật khẩu mới");
            lblLoi = NhanTd("", 9F, false, Color.FromArgb(200, 55, 70)); lblLoi.SetBounds(0, y, 410, 36); Than.Controls.Add(lblLoi);
            Button luu = ThemNut("Lưu mật khẩu", VeBaoCao.Teal, Color.White, 140, async (s, e) => await Luu());
            ThemNut("Hủy", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            AcceptButton = luu;
            Shown += (s, e) => (txtCu ?? txtMoi).Focus();
        }

        private async Task Luu()
        {
            if (txtMoi.Text != txtMoi2.Text) { lblLoi.Text = "Hai lần nhập mật khẩu mới không khớp."; return; }
            string cu = txtCu == null ? null : txtCu.Text, moi = txtMoi.Text;
            UseWaitCursor = true;
            try
            {
                string kq = await Task.Run(() => xuLy(cu, moi));
                if (kq == "OK") { DialogResult = DialogResult.OK; Close(); }
                else lblLoi.Text = kq;
            }
            catch (Exception ex) { lblLoi.Text = ex.Message; }
            finally { UseWaitCursor = false; }
        }
    }
}
