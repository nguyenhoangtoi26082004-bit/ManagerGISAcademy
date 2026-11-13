using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Forms;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>
    /// The "Tai khoan & phan quyen" trong Quan tri he thong:
    /// duyet dang ky, cap vai tro, khoa/mo, dat lai mat khau, nhat ky dang nhap.
    /// Moi thao tac goi ham SECURITY DEFINER (script 89) - CSDL tu kiem tra quyen quan tri.
    /// </summary>
    public sealed class TrangQuanLyTaiKhoan : UserControl
    {
        private static readonly Color Nen = TienIchGiaoDienSang.Background;
        private readonly TruyXuatTaiKhoan dal = new TruyXuatTaiKhoan();
        private readonly DataGridView grid, gridDn;
        private readonly Label lblTomTat, lblTen, lblMoTa, lblTt;
        private readonly ComboBox cboVaiTro;
        private readonly Button btnDuyet, btnTuChoi, btnLuu, btnKhoa, btnDatLai, btnTai;
        private readonly TextBox txtLoc;
        private List<DongTaiKhoan> ds = new List<DongTaiKhoan>();
        private static readonly string[] MaVaiTro = { "gis_quan_tri", "gis_bien_tap", "gis_xem" };

        /// <summary>Bao FormMain so tai khoan cho duyet (cho huy hieu).</summary>
        public event Action<int> SoChoDuyetThayDoi;

        public TrangQuanLyTaiKhoan()
        {
            Dock = DockStyle.Fill;
            BackColor = Nen;
            Font = new Font("Segoe UI", 9F);

            TableLayoutPanel chia = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Nen };
            chia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            chia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            chia.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            Controls.Add(chia);

            // ---------------- Danh sach ----------------
            NativeCard trai = TienIchGiaoDienSang.Card();
            trai.Dock = DockStyle.Fill; trai.Padding = new Padding(16, 12, 16, 12); trai.Margin = new Padding(0, 0, 8, 0);
            Panel dau = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Color.White };
            Label td = TienIchGiaoDienSang.Text("Tài khoản người dùng", 11.5F, true); td.Dock = DockStyle.Left; td.Width = 230;
            btnTai = NutGiaoDien.Phang("↻", Color.White, TienIchGiaoDienSang.Ink, 10F);
            btnTai.Dock = DockStyle.Right; btnTai.Width = 38; btnTai.FlatAppearance.BorderSize = 1; btnTai.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            KhungBoGocChung khLoc = new KhungBoGocChung { Dock = DockStyle.Right, Width = 230, Padding = new Padding(10, 9, 8, 4) };
            txtLoc = new TextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 9.5F) };
            khLoc.Controls.Add(txtLoc);
            Panel cach = new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.White };
            dau.Controls.Add(td); dau.Controls.Add(khLoc); dau.Controls.Add(cach); dau.Controls.Add(btnTai);
            lblTomTat = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblTomTat.Dock = DockStyle.Top; lblTomTat.Height = 24;
            grid = BangGiaoDien.Tao();
            BangGiaoDien.Cot(grid, "Tên đăng nhập", "TenDangNhap", 18, false, true);
            BangGiaoDien.Cot(grid, "Họ tên", "HoTen", 22);
            BangGiaoDien.Cot(grid, "Vai trò", "TenVaiTro", 13);
            BangGiaoDien.Cot(grid, "Trạng thái", "TrangThaiHienThi", 15);
            BangGiaoDien.Cot(grid, "Đơn vị công tác", "DonViCongTac", 18);
            BangGiaoDien.Cot(grid, "Đăng nhập cuối", "DangNhapCuoiHienThi", 14);
            BangGiaoDien.VeNhanTron(grid, 3, MauTrangThai);
            BangGiaoDien.VeNhanTron(grid, 2, MauVaiTro);
            trai.Controls.Add(grid); trai.Controls.Add(lblTomTat); trai.Controls.Add(dau);
            chia.Controls.Add(trai, 0, 0);

            // ---------------- Chi tiet + thao tac ----------------
            TableLayoutPanel phai = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2, BackColor = Nen, Margin = new Padding(8, 0, 0, 0) };
            phai.RowStyles.Add(new RowStyle(SizeType.Absolute, 300F));
            phai.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            chia.Controls.Add(phai, 1, 0);

            NativeCard ct = TienIchGiaoDienSang.Card();
            ct.Dock = DockStyle.Fill; ct.Padding = new Padding(16, 12, 16, 12); ct.Margin = new Padding(0, 0, 0, 10);
            lblTen = TienIchGiaoDienSang.Text("Chọn một tài khoản", 11.5F, true); lblTen.Dock = DockStyle.Top; lblTen.Height = 28;
            lblMoTa = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted); lblMoTa.Dock = DockStyle.Top; lblMoTa.Height = 74;
            lblMoTa.TextAlign = ContentAlignment.TopLeft;
            Panel hangVt = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.White, Padding = new Padding(0, 6, 0, 2) };
            Label lvt = TienIchGiaoDienSang.Text("Vai trò", 9F, true); lvt.Dock = DockStyle.Left; lvt.Width = 70;
            cboVaiTro = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) };
            cboVaiTro.Items.AddRange(new object[] { "Quản trị – toàn quyền, duyệt, phân quyền", "Biên tập – thêm/sửa, gửi chờ duyệt", "Chỉ xem – tra cứu" });
            hangVt.Controls.Add(cboVaiTro); hangVt.Controls.Add(lvt);
            FlowLayoutPanel nut = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 96, BackColor = Color.White, Padding = new Padding(0, 8, 0, 0) };
            btnDuyet = Nut("✓  Duyệt đăng ký", VeBaoCao.Teal, Color.White, 150);
            btnTuChoi = Nut("✕  Từ chối", BangGiaoDien.DoNhat, BangGiaoDien.Do, 110);
            btnLuu = Nut("Lưu vai trò", VeBaoCao.Blue, Color.White, 120);
            btnKhoa = Nut("Khóa tài khoản", BangGiaoDien.CamNhat, BangGiaoDien.Cam, 130);
            btnDatLai = Nut("Đặt lại mật khẩu…", Color.White, TienIchGiaoDienSang.Ink, 140);
            btnDatLai.FlatAppearance.BorderSize = 1; btnDatLai.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            foreach (Button b in new[] { btnDuyet, btnTuChoi, btnLuu, btnKhoa, btnDatLai }) nut.Controls.Add(b);
            lblTt = TienIchGiaoDienSang.Text("", 8.8F, false, VeBaoCao.Teal); lblTt.Dock = DockStyle.Top; lblTt.Height = 22;
            ct.Controls.Add(lblTt); ct.Controls.Add(nut); ct.Controls.Add(hangVt); ct.Controls.Add(lblMoTa); ct.Controls.Add(lblTen);
            phai.Controls.Add(ct, 0, 0);

            NativeCard nk = TienIchGiaoDienSang.Card();
            nk.Dock = DockStyle.Fill; nk.Padding = new Padding(16, 12, 16, 12);
            Label tdNk = TienIchGiaoDienSang.Text("Nhật ký đăng nhập", 10.5F, true); tdNk.Dock = DockStyle.Top; tdNk.Height = 28;
            gridDn = BangGiaoDien.Tao();
            BangGiaoDien.Cot(gridDn, "Thời điểm", "ThoiDiemHienThi", 30);
            BangGiaoDien.Cot(gridDn, "Tài khoản", "TenDangNhap", 30, false, true);
            BangGiaoDien.Cot(gridDn, "Sự kiện", "KetQuaHienThi", 40);
            BangGiaoDien.VeNhanTron(gridDn, 2, s =>
                s.StartsWith("Sai") || s.Contains("khóa") || s.Contains("Khóa") || s.StartsWith("Chưa") || s.StartsWith("Từ chối")
                    ? Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do)
                    : s.StartsWith("Đăng nhập") || s.StartsWith("Duyệt")
                        ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal)
                        : Tuple.Create(BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue));
            nk.Controls.Add(gridDn); nk.Controls.Add(tdNk);
            phai.Controls.Add(nk, 0, 1);

            // ---------------- Su kien ----------------
            btnTai.Click += (s, e) => TaiLai();
            txtLoc.TextChanged += (s, e) => HienThi();
            txtLoc.HandleCreated += (s, e) => SendMessage(txtLoc.Handle, 0x1501, (IntPtr)1, "⌕  Lọc tên, họ tên, đơn vị…");
            grid.SelectionChanged += (s, e) => CapNhatChiTiet();
            btnDuyet.Click += async (s, e) => await CapNhat(true, "HOAT_DONG");
            btnLuu.Click += async (s, e) => await CapNhat(false, null);
            btnKhoa.Click += async (s, e) =>
            {
                DongTaiKhoan d = Chon();
                if (d == null) return;
                await CapNhat(false, d.TrangThai == "KHOA" ? "HOAT_DONG" : "KHOA");
            };
            btnTuChoi.Click += async (s, e) => await TuChoi();
            btnDatLai.Click += (s, e) => DatLaiMatKhau();
            CapNhatChiTiet();
        }

        private static Button Nut(string chu, Color nen, Color mau, int rong)
        {
            Button b = NutGiaoDien.Phang(chu, nen, mau, 9F);
            b.Size = new Size(rong, 36);
            b.Margin = new Padding(0, 0, 8, 8);
            return b;
        }

        private static Tuple<Color, Color> MauTrangThai(string s)
        {
            switch (s)
            {
                case "Chờ duyệt": return Tuple.Create(BangGiaoDien.CamNhat, BangGiaoDien.Cam);
                case "Đã khóa": case "Khóa tạm": return Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do);
                default: return Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal);
            }
        }

        private static Tuple<Color, Color> MauVaiTro(string s)
        {
            switch (s)
            {
                case "Quản trị": return Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal);
                case "Biên tập": return Tuple.Create(BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue);
                default: return Tuple.Create(Color.FromArgb(240, 242, 245), TienIchGiaoDienSang.Muted);
            }
        }

        // =====================================================

        public async void TaiLai()
        {
            lblTomTat.Text = "Đang tải…";
            try
            {
                Tuple<List<DongTaiKhoan>, List<DongDangNhap>> kq = await Task.Run(() =>
                    Tuple.Create(dal.LayDanhSach(), dal.NhatKyDangNhap(150)));
                ds = kq.Item1;
                gridDn.DataSource = new BindingList<DongDangNhap>(kq.Item2);
                gridDn.ClearSelection();
                HienThi();
                int cho = ds.Count(x => x.TrangThai == "CHO_DUYET");
                Action<int> h = SoChoDuyetThayDoi;
                if (h != null) h(cho);
            }
            catch (Exception ex)
            {
                ds = new List<DongTaiKhoan>();
                grid.DataSource = null;
                lblTomTat.Text = "Không tải được: " + ex.Message;
            }
        }

        private void HienThi()
        {
            string k = txtLoc.Text.Trim();
            System.Globalization.CompareInfo ci = System.Globalization.CultureInfo.GetCultureInfo("vi-VN").CompareInfo;
            System.Globalization.CompareOptions op = System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreNonSpace;
            List<DongTaiKhoan> loc = ds.Where(d => k.Length == 0 || ci.IndexOf(d.TenDangNhap ?? "", k, op) >= 0 ||
                                                 ci.IndexOf(d.HoTen ?? "", k, op) >= 0 || ci.IndexOf(d.DonViCongTac ?? "", k, op) >= 0).ToList();
            long idChon = Chon() == null ? -1 : Chon().Id;
            grid.DataSource = new BindingList<DongTaiKhoan>(loc);
            grid.ClearSelection();
            foreach (DataGridViewRow r in grid.Rows)
                if (((DongTaiKhoan)r.DataBoundItem).Id == idChon) { r.Selected = true; grid.CurrentCell = r.Cells[0]; }
            int cho = ds.Count(x => x.TrangThai == "CHO_DUYET");
            lblTomTat.Text = ds.Count + " tài khoản · " + ds.Count(x => x.VaiTro == "gis_quan_tri" && x.TrangThai == "HOAT_DONG") + " quản trị · " +
                             ds.Count(x => x.VaiTro == "gis_bien_tap") + " biên tập · " + ds.Count(x => x.VaiTro == "gis_xem") + " chỉ xem" +
                             (cho > 0 ? "   ·   ⚑ " + cho + " đăng ký chờ duyệt" : "");
            CapNhatChiTiet();
        }

        private DongTaiKhoan Chon()
        {
            return grid.CurrentRow != null && grid.CurrentRow.Selected ? grid.CurrentRow.DataBoundItem as DongTaiKhoan : null;
        }

        private void CapNhatChiTiet()
        {
            DongTaiKhoan d = Chon();
            bool co = d != null, cho = co && d.TrangThai == "CHO_DUYET";
            bool laToi = co && PhienDangNhap.HienTai != null && string.Equals(d.TenDangNhap, PhienDangNhap.HienTai.TenDangNhap, StringComparison.OrdinalIgnoreCase);
            lblTen.Text = co ? d.HoTen + "  (" + d.TenDangNhap + ")" + (laToi ? "  · bạn" : "") : "Chọn một tài khoản";
            lblMoTa.Text = !co ? "Chọn dòng bên trái để duyệt đăng ký, đổi vai trò, khóa / mở hoặc đặt lại mật khẩu." :
                "Đơn vị: " + (string.IsNullOrEmpty(d.DonViCongTac) ? "—" : d.DonViCongTac) +
                "   ·   Email: " + (string.IsNullOrEmpty(d.Email) ? "—" : d.Email) +
                "\nĐăng ký: " + d.NgayTaoHienThi + (string.IsNullOrEmpty(d.NguoiDuyet) ? "" : "   ·   Duyệt bởi: " + d.NguoiDuyet) +
                (string.IsNullOrEmpty(d.LyDoDangKy) ? "" : "\nLý do: " + d.LyDoDangKy) +
                (d.KhoaDen.HasValue ? "\nĐang khóa tạm đến " + d.KhoaDen.Value.ToString("HH:mm") + " (nhập sai mật khẩu)" : "");
            cboVaiTro.Enabled = co && !laToi;
            if (co) cboVaiTro.SelectedIndex = Math.Max(0, Array.IndexOf(MaVaiTro, cho ? "gis_xem" : d.VaiTro));
            btnDuyet.Visible = btnTuChoi.Visible = cho;
            btnLuu.Visible = btnKhoa.Visible = co && !cho;
            btnLuu.Enabled = btnKhoa.Enabled = !laToi;
            btnDatLai.Enabled = co && !cho;
            btnKhoa.Text = co && d.TrangThai == "KHOA" ? "Mở khóa" : "Khóa tài khoản";
            if (cho) cboVaiTro.SelectedIndex = 2;
        }

        private async Task CapNhat(bool duyet, string trangThaiMoi)
        {
            DongTaiKhoan d = Chon();
            if (d == null) return;
            string vt = MaVaiTro[Math.Max(0, cboVaiTro.SelectedIndex)];
            string tt = trangThaiMoi ?? (d.TrangThai == "KHOA" ? "KHOA" : "HOAT_DONG");
            if (vt == "gis_quan_tri" && d.VaiTro != "gis_quan_tri" &&
                MessageBox.Show(this, "Cấp vai trò QUẢN TRỊ cho " + d.HoTen + "?\nNgười này sẽ phân quyền, duyệt thay đổi và xem toàn bộ dữ liệu Tuyệt mật.",
                    "Xác nhận phân quyền", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            if (tt == "KHOA" && d.TrangThai != "KHOA" &&
                MessageBox.Show(this, "Khóa tài khoản " + d.TenDangNhap + "? Người dùng sẽ không đăng nhập được.",
                    "Khóa tài khoản", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                string kq = await Task.Run(() => dal.CapNhat(d.Id, vt, tt));
                Bao(kq == "OK" ? (duyet ? "✓ Đã duyệt " + d.TenDangNhap + " với vai trò " + TruyXuatTaiKhoan.TenVaiTro(vt) + "."
                                        : "✓ Đã cập nhật " + d.TenDangNhap + ": " + TruyXuatTaiKhoan.TenVaiTro(vt) + (tt == "KHOA" ? " · đã khóa" : "")) : kq, kq != "OK");
                if (kq == "OK") TaiLai();
            }
            catch (Exception ex) { Bao(ex.Message, true); }
        }

        private async Task TuChoi()
        {
            DongTaiKhoan d = Chon();
            if (d == null) return;
            if (MessageBox.Show(this, "Từ chối và xóa đăng ký của " + d.TenDangNhap + "?", "Từ chối đăng ký",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                string kq = await Task.Run(() => dal.TuChoi(d.Id));
                Bao(kq == "OK" ? "Đã từ chối đăng ký " + d.TenDangNhap + "." : kq, kq != "OK");
                if (kq == "OK") TaiLai();
            }
            catch (Exception ex) { Bao(ex.Message, true); }
        }

        private void DatLaiMatKhau()
        {
            DongTaiKhoan d = Chon();
            if (d == null) return;
            long id = d.Id;
            using (FormMatKhau f = new FormMatKhau("Đặt lại mật khẩu", "Mật khẩu tạm cho " + d.TenDangNhap + " – báo trực tiếp cho người dùng và yêu cầu đổi ngay",
                       false, (cu, moi) => dal.DatLaiMatKhau(id, moi)))
            {
                if (f.ShowDialog(this) == DialogResult.OK) { Bao("✓ Đã đặt lại mật khẩu cho " + d.TenDangNhap + ".", false); TaiLai(); }
            }
        }

        private void Bao(string s, bool loi)
        {
            lblTt.ForeColor = loi ? BangGiaoDien.Do : VeBaoCao.Teal;
            lblTt.Text = s;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, string l);
    }
}
