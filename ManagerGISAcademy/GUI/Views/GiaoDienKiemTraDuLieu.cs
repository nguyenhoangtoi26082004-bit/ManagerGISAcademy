
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Views
{
    // Du lieu dau vao la ket qua kiem tra da duoc BLL cho phep xem.
    public sealed class CanhBaoKiemTraNative
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string LoaiLoi { get; set; }
        public string MucDo { get; set; }
        public string NoiDung { get; set; }
        public string KhuyenNghi { get; set; }
    }

    /// <summary>
    /// Trang "KIEM TRA CHAT LUONG DU LIEU" - thiet ke v2 (13/11/2026).
    /// Giu nguyen hop dong voi FormMain: su kien YeuCauChayKiemTra / YeuCauXemDonVi /
    /// YeuCauDinhVi, ham NapKetQua(...) va DatTomTat(...). Khong tu truy van CSDL.
    /// </summary>
    public sealed class UcKiemTraDuLieuNative : NativePage
    {
        private const string NghiemTrong = "Nghiêm trọng";
        private static readonly Color Do = Color.FromArgb(204, 70, 90);
        private static readonly Color DoNhat = Color.FromArgb(253, 232, 236);
        private static readonly Color Cam = Color.FromArgb(209, 127, 35);
        private static readonly Color CamNhat = Color.FromArgb(253, 241, 225);
        private static readonly Color XanhNhat = Color.FromArgb(226, 246, 241);
        private static readonly Color XanhDuongNhat = Color.FromArgb(230, 241, 252);

        private readonly List<CanhBaoKiemTraNative> data = new List<CanhBaoKiemTraNative>();
        private List<CanhBaoKiemTraNative> hienThi = new List<CanhBaoKiemTraNative>();
        private readonly BannerTrang banner;
        private readonly Button btnChay, btnXuat, btnXemHoSo, btnBanDo;
        private readonly Label lblTrangThai, hint;
        private readonly TheChiSoChung kpiHoSo, kpiDat, kpiNghiemTrong, kpiCanhBao;
        private readonly FlowLayoutPanel hangChip;
        private readonly List<Button> nutMucDo = new List<Button>();
        private readonly TextBox search;
        private readonly DataGridView grid;
        private readonly Panel pnlTrong;
        private readonly Label lblTrongTieuDe, lblTrongMoTa;
        private readonly Label lblCtTen, lblCtMoTa, lblCtGoiY;
        private readonly Timer timer, choToiDa;
        private readonly Stopwatch dongHo = new Stopwatch();
        private int mucDo;              // 0 tat ca, 1 nghiem trong, 2 canh bao
        private string loaiChon;        // null = tat ca loai loi
        private bool daChay, dangChay;

        public event Action YeuCauChayKiemTra;
        public event Action<string> YeuCauXemDonVi;
        public event Action<string> YeuCauDinhVi;

        public UcKiemTraDuLieuNative()
        {
            Name = "UcKiemTraDuLieuNative";
            timer = new Timer { Interval = 250 };
            timer.Tick += (s, e) => { timer.Stop(); Filter(); };
            // Neu FormMain bao loi (khong goi NapKetQua) -> mo lai nut sau 90 s.
            choToiDa = new Timer { Interval = 90000 };
            choToiDa.Tick += (s, e) => { choToiDa.Stop(); KetThucChay(false); };

            // 1. Banner
            banner = new BannerTrang
            {
                TieuDe = "KIỂM TRA CHẤT LƯỢNG DỮ LIỆU",
                PhuDe = "Rà soát thuộc tính (BLL) và chất lượng vị trí không gian (PostGIS) · không tự sửa CSDL",
                NhanPhai = "LẦN KIỂM TRA GẦN NHẤT",
                GiaTriPhai = "Chưa chạy"
            };
            AddRow(banner, 96);

            // 2. Thanh hanh dong
            Panel thanh = new Panel { BackColor = TienIchGiaoDienSang.Background, Padding = new Padding(0, 10, 0, 6) };
            btnChay = NutGiaoDien.Phang("▶  Chạy kiểm tra", TienIchGiaoDienSang.Teal, Color.White, 9.6F);
            btnChay.Dock = DockStyle.Left;
            btnChay.Width = 168;
            Panel cach = new Panel { Dock = DockStyle.Left, Width = 14, BackColor = TienIchGiaoDienSang.Background };
            lblTrangThai = TienIchGiaoDienSang.Text("● Chưa chạy kiểm tra", 9.2F, true, Color.FromArgb(160, 176, 194));
            lblTrangThai.Dock = DockStyle.Fill;
            btnXuat = NutGiaoDien.Phang("⬇  Xuất danh sách lỗi (CSV)", Color.White, TienIchGiaoDienSang.Ink, 9.2F);
            btnXuat.Dock = DockStyle.Right;
            btnXuat.Width = 228;
            btnXuat.FlatAppearance.BorderSize = 1;
            btnXuat.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            thanh.Controls.Add(lblTrangThai);
            thanh.Controls.Add(cach);
            thanh.Controls.Add(btnChay);
            thanh.Controls.Add(btnXuat);
            AddRow(thanh, 58);
            btnChay.Click += (s, e) => BatDauChay();
            btnXuat.Click += (s, e) => Export();

            // 3. Bon chi so
            TableLayoutPanel kpi = new TableLayoutPanel { ColumnCount = 4, RowCount = 1, BackColor = TienIchGiaoDienSang.Background };
            for (int i = 0; i < 4; i++) kpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            kpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            kpiHoSo = new TheChiSoChung("▤", "HỒ SƠ KIỂM TRA", VeBaoCao.Blue, XanhDuongNhat);
            kpiDat = new TheChiSoChung("✓", "HỒ SƠ ĐẠT", VeBaoCao.Teal, XanhNhat);
            kpiNghiemTrong = new TheChiSoChung("!", "NGHIÊM TRỌNG", Do, DoNhat);
            kpiCanhBao = new TheChiSoChung("△", "CẢNH BÁO", Cam, CamNhat);
            TheChiSoChung[] the = { kpiHoSo, kpiDat, kpiNghiemTrong, kpiCanhBao };
            for (int i = 0; i < 4; i++)
            {
                the[i].Dock = DockStyle.Fill;
                the[i].Margin = new Padding(i == 0 ? 0 : 6, 0, i == 3 ? 0 : 6, 0);
                kpi.Controls.Add(the[i], i, 0);
            }
            AddRow(kpi, 96);

            // 4. Chip loai loi (tao sau khi co ket qua)
            hangChip = new FlowLayoutPanel
            {
                BackColor = TienIchGiaoDienSang.Background,
                WrapContents = true,
                Padding = new Padding(0, 12, 0, 0)
            };
            AddRow(hangChip, 54);

            // 5. The danh sach: cong cu loc + bang + chi tiet
            NativeCard card = TienIchGiaoDienSang.Card();
            card.Padding = new Padding(16, 12, 16, 12);
            TableLayoutPanel trong = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, BackColor = Color.White };
            trong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            trong.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            trong.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            trong.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));

            TableLayoutPanel congCu = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = Color.White, Margin = Padding.Empty };
            congCu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            congCu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 330F));
            congCu.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            search = TienIchGiaoDienSang.Input();
            search.HandleCreated += (s, e) =>
                SendMessage(search.Handle, 0x1501, (IntPtr)1, "⌕  Lọc theo mã, tên đơn vị hoặc nội dung lỗi…");
            congCu.Controls.Add(search, 0, 0);
            KhungBoGocChung seg = new KhungBoGocChung { Dock = DockStyle.Fill, Padding = new Padding(4), Margin = new Padding(10, 6, 0, 6) };
            TableLayoutPanel segTrong = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 1, BackColor = Color.White, Margin = Padding.Empty };
            segTrong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            segTrong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            segTrong.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            segTrong.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            string[] tenMuc = { "Tất cả", "Nghiêm trọng", "Cảnh báo" };
            for (int i = 0; i < 3; i++)
            {
                int chiSo = i;
                Button b = NutGiaoDien.Phang(tenMuc[i], Color.White, TienIchGiaoDienSang.Muted, 8.8F);
                b.Dock = DockStyle.Fill;
                b.Margin = new Padding(1, 0, 1, 0);
                b.Click += (s, e) => { mucDo = chiSo; CapNhatNutMucDo(); Filter(); };
                nutMucDo.Add(b);
                segTrong.Controls.Add(b, i, 0);
            }
            seg.Controls.Add(segTrong);
            congCu.Controls.Add(seg, 1, 0);
            trong.Controls.Add(congCu, 0, 0);

            grid = TaoBang();
            pnlTrong = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            TableLayoutPanel tt = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, BackColor = Color.White };
            tt.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tt.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
            tt.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            tt.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tt.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tt.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            Label vong = TienIchGiaoDienSang.Text("✓", 22F, true, VeBaoCao.Teal);
            vong.Dock = DockStyle.None;
            vong.Size = new Size(64, 64);
            vong.Anchor = AnchorStyles.None;
            vong.TextAlign = ContentAlignment.MiddleCenter;
            vong.BackColor = XanhNhat;
            NutGiaoDien.BoTron(vong, 32);
            lblTrongTieuDe = TienIchGiaoDienSang.Text("Chưa chạy kiểm tra", 12F, true);
            lblTrongTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            lblTrongMoTa = TienIchGiaoDienSang.Text("Bấm \"Chạy kiểm tra\" để rà soát thuộc tính và vị trí của toàn bộ hồ sơ.",
                9F, false, TienIchGiaoDienSang.Muted);
            lblTrongMoTa.TextAlign = ContentAlignment.TopCenter;
            tt.Controls.Add(vong, 0, 1);
            tt.Controls.Add(lblTrongTieuDe, 0, 2);
            tt.Controls.Add(lblTrongMoTa, 0, 3);
            pnlTrong.Controls.Add(tt);
            Panel vungBang = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = Padding.Empty };
            vungBang.Controls.Add(grid);
            vungBang.Controls.Add(pnlTrong);
            pnlTrong.BringToFront();
            trong.Controls.Add(vungBang, 0, 1);

            // Chi tiet muc dang chon + hanh dong.
            Panel chiTiet = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(0, 12, 0, 0), Margin = Padding.Empty };
            chiTiet.Paint += (s, e) =>
            {
                using (Pen p = new Pen(TienIchGiaoDienSang.Border))
                    e.Graphics.DrawLine(p, 0, 4, chiTiet.Width, 4);
            };
            FlowLayoutPanel nut = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 340,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 16, 0, 0)
            };
            btnBanDo = NutGiaoDien.Phang("⌖  Xem trên bản đồ", TienIchGiaoDienSang.Teal, Color.White, 9F);
            btnBanDo.Size = new Size(170, 38);
            btnBanDo.Margin = Padding.Empty;
            btnXemHoSo = NutGiaoDien.Phang("▤  Xem hồ sơ", XanhDuongNhat, TienIchGiaoDienSang.Blue, 9F);
            btnXemHoSo.Size = new Size(140, 38);
            btnXemHoSo.Margin = new Padding(0, 0, 10, 0);
            nut.Controls.Add(btnBanDo);
            nut.Controls.Add(btnXemHoSo);
            Panel chu = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
            lblCtTen = TienIchGiaoDienSang.Text("Chọn một dòng để xem chi tiết và khuyến nghị xử lý.", 10F, true);
            lblCtTen.Dock = DockStyle.Top;
            lblCtTen.Height = 24;
            lblCtMoTa = TienIchGiaoDienSang.Text("", 9F, false, TienIchGiaoDienSang.Muted);
            lblCtMoTa.Dock = DockStyle.Top;
            lblCtMoTa.Height = 22;
            lblCtGoiY = TienIchGiaoDienSang.Text("", 9F, true, VeBaoCao.Teal);
            lblCtGoiY.Dock = DockStyle.Top;
            lblCtGoiY.Height = 24;
            chu.Controls.Add(lblCtGoiY);
            chu.Controls.Add(lblCtMoTa);
            chu.Controls.Add(lblCtTen);
            chiTiet.Controls.Add(chu);
            chiTiet.Controls.Add(nut);
            trong.Controls.Add(chiTiet, 0, 2);
            card.Controls.Add(trong);
            AddRow(card, 540);

            btnXemHoSo.Click += (s, e) => Raise(YeuCauXemDonVi);
            btnBanDo.Click += (s, e) => Raise(YeuCauDinhVi);

            hint = TienIchGiaoDienSang.Text("Chưa chạy kiểm tra · Dữ liệu lấy từ BLL / PostGIS · Không tự động chỉnh sửa CSDL",
                8.4F, false, TienIchGiaoDienSang.Muted);
            AddRow(hint, 36);

            search.TextChanged += (s, e) => Schedule();
            kpiHoSo.DatGiaTri("—", "chưa rà soát");
            kpiDat.DatGiaTri("—", "hồ sơ không có cảnh báo");
            kpiNghiemTrong.DatGiaTri("—", "cần ưu tiên xử lý");
            kpiCanhBao.DatGiaTri("—", "cần đối chiếu hồ sơ");
            CapNhatNutMucDo();
            CapNhatChiTiet();
            btnXuat.Enabled = false;
        }

        // =================================================
        // API CONG KHAI (FormMain goi)
        // =================================================

        public void NapKetQua(IEnumerable<CanhBaoKiemTraNative> items, int soHoSo)
        {
            if (IsDisposed) return;
            data.Clear();
            if (items != null) data.AddRange(items.Where(x => x != null));
            int tong = Math.Max(0, soHoSo);
            int coLoi = data.Where(x => !string.IsNullOrWhiteSpace(x.MaDonVi))
                .Select(x => x.MaDonVi.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            int dat = Math.Max(0, tong - coLoi);
            int nt = data.Count(LaNghiemTrong);
            kpiHoSo.DatGiaTri(tong.ToString("N0", VeBaoCao.Vi), "thuộc tính + vị trí");
            kpiDat.DatGiaTri(dat.ToString("N0", VeBaoCao.Vi), PhanTram(dat, tong) + " không có cảnh báo");
            kpiNghiemTrong.DatGiaTri(nt.ToString("N0", VeBaoCao.Vi), "cần ưu tiên xử lý");
            kpiCanhBao.DatGiaTri((data.Count - nt).ToString("N0", VeBaoCao.Vi), "cần đối chiếu hồ sơ");
            hint.Text = "Đã kiểm tra " + tong + " hồ sơ · " + data.Count +
                " mục cần rà soát · Chưa thay thế kiểm tra topology chuyên sâu";
            daChay = true;
            loaiChon = null;
            TaoChip();
            KetThucChay(true);
            Filter();
        }

        /// <summary>Thay dong ghi chu cuoi trang bang tom tat chat luong vi tri.</summary>
        public void DatTomTat(string noiDung)
        {
            if (IsDisposed || string.IsNullOrWhiteSpace(noiDung)) return;
            hint.Text = noiDung;
        }

        // =================================================
        // CHAY KIEM TRA
        // =================================================

        private void BatDauChay()
        {
            if (dangChay) return;
            Action h = YeuCauChayKiemTra;
            if (h == null) return;
            dangChay = true;
            btnChay.Enabled = false;
            NutGiaoDien.DoiMau(btnChay, Color.FromArgb(160, 205, 195), Color.White);
            lblTrangThai.Text = "● Đang kiểm tra hồ sơ và vị trí…";
            lblTrangThai.ForeColor = Cam;
            dongHo.Restart();
            choToiDa.Start();
            h();
        }

        private void KetThucChay(bool thanhCong)
        {
            choToiDa.Stop();
            dongHo.Stop();
            dangChay = false;
            btnChay.Enabled = true;
            NutGiaoDien.DoiMau(btnChay, TienIchGiaoDienSang.Teal, Color.White);
            btnChay.Text = daChay ? "↻  Chạy lại kiểm tra" : "▶  Chạy kiểm tra";
            if (thanhCong)
            {
                banner.GiaTriPhai = DateTime.Now.ToString("dd/MM/yyyy HH:mm", VeBaoCao.Vi);
                lblTrangThai.Text = "● Hoàn thành · " + data.Count.ToString("N0", VeBaoCao.Vi) + " mục cần rà soát" +
                    (dongHo.ElapsedMilliseconds > 0
                        ? " · " + (dongHo.ElapsedMilliseconds / 1000d).ToString("N1", VeBaoCao.Vi) + " giây" : "");
                lblTrangThai.ForeColor = VeBaoCao.Teal;
            }
            else
            {
                lblTrangThai.Text = "● Chưa nhận được kết quả – xem thanh trạng thái phía dưới";
                lblTrangThai.ForeColor = Do;
            }
        }

        // =================================================
        // LOC
        // =================================================

        private void TaoChip()
        {
            hangChip.SuspendLayout();
            foreach (Control c in hangChip.Controls.Cast<Control>().ToList()) c.Dispose();
            hangChip.Controls.Clear();
            List<KeyValuePair<string, int>> nhom = data
                .GroupBy(x => string.IsNullOrWhiteSpace(x.LoaiLoi) ? "Khác" : x.LoaiLoi.Trim(), StringComparer.CurrentCultureIgnoreCase)
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Count()))
                .OrderByDescending(k => k.Value).ThenBy(k => k.Key).ToList();
            ThemChip(null, "Tất cả", data.Count);
            foreach (KeyValuePair<string, int> k in nhom) ThemChip(k.Key, k.Key, k.Value);
            hangChip.ResumeLayout(true);
            // Hang chip co the xuong dong: noi chieu cao dong (hang thu 4 cua trang).
            int cao = hangChip.GetPreferredSize(new Size(Math.Max(300, hangChip.Width), 0)).Height;
            if (Body.RowStyles.Count > 3)
                Body.RowStyles[3].Height = Math.Max(54, cao + 4);
            CapNhatChip();
        }

        private void ThemChip(string khoa, string ten, int so)
        {
            Button b = NutGiaoDien.Phang(ten + "   " + so.ToString("N0", VeBaoCao.Vi), Color.White, TienIchGiaoDienSang.Muted, 8.8F);
            Size kt = TextRenderer.MeasureText(b.Text, b.Font);
            b.Size = new Size(kt.Width + 28, 32);
            b.Margin = new Padding(0, 0, 8, 6);
            b.Tag = khoa;
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            NutGiaoDien.BoTron(b, 16);
            b.Click += (s, e) => { loaiChon = khoa; CapNhatChip(); Filter(); };
            hangChip.Controls.Add(b);
        }

        private void CapNhatChip()
        {
            foreach (Button b in hangChip.Controls.OfType<Button>())
            {
                bool chon = string.Equals(b.Tag as string, loaiChon, StringComparison.CurrentCultureIgnoreCase);
                NutGiaoDien.DoiMau(b, chon ? VeBaoCao.Ink : Color.White, chon ? Color.White : TienIchGiaoDienSang.Muted);
                b.FlatAppearance.BorderColor = chon ? VeBaoCao.Ink : TienIchGiaoDienSang.Border;
            }
        }

        private void CapNhatNutMucDo()
        {
            for (int i = 0; i < nutMucDo.Count; i++)
            {
                bool chon = i == mucDo;
                Color nen = !chon ? Color.White : i == 1 ? Do : i == 2 ? Cam : TienIchGiaoDienSang.Teal;
                NutGiaoDien.DoiMau(nutMucDo[i], nen, chon ? Color.White : TienIchGiaoDienSang.Muted);
            }
        }

        private void Schedule()
        {
            if (IsDisposed) return;
            timer.Stop(); timer.Start();
        }

        private void Filter()
        {
            string key = search.Text.Trim();
            hienThi = data.Where(x =>
                (mucDo == 0 || (mucDo == 1) == LaNghiemTrong(x)) &&
                (loaiChon == null || string.Equals((x.LoaiLoi ?? "").Trim(), loaiChon, StringComparison.CurrentCultureIgnoreCase)) &&
                (key.Length == 0 || Contains(x.MaDonVi, key) || Contains(x.TenDonVi, key)
                    || Contains(x.NoiDung, key) || Contains(x.LoaiLoi, key)))
                .OrderBy(x => LaNghiemTrong(x) ? 0 : 1)
                .ThenBy(x => x.LoaiLoi).ThenBy(x => x.MaDonVi).ToList();
            grid.DataSource = new BindingList<CanhBaoKiemTraNative>(hienThi);
            grid.ClearSelection();
            grid.CurrentCell = null;

            bool trongRong = hienThi.Count == 0;
            pnlTrong.Visible = trongRong;
            grid.Visible = !trongRong;
            if (trongRong)
            {
                pnlTrong.BringToFront();
                if (!daChay)
                {
                    lblTrongTieuDe.Text = "Chưa chạy kiểm tra";
                    lblTrongMoTa.Text = "Bấm \"Chạy kiểm tra\" để rà soát thuộc tính và vị trí của toàn bộ hồ sơ.";
                }
                else if (data.Count == 0)
                {
                    lblTrongTieuDe.Text = "Dữ liệu đạt – không có cảnh báo";
                    lblTrongMoTa.Text = "Tất cả hồ sơ đều qua kiểm tra thuộc tính và vị trí.";
                }
                else
                {
                    lblTrongTieuDe.Text = "Không có mục nào khớp bộ lọc";
                    lblTrongMoTa.Text = "Đổi mức độ, loại lỗi hoặc xóa từ khóa lọc.";
                }
            }
            btnXuat.Enabled = hienThi.Count > 0;
            CapNhatChiTiet();
        }

        // =================================================
        // CHI TIET + HANH DONG
        // =================================================

        private CanhBaoKiemTraNative DangChon()
        {
            return grid.CurrentRow != null && grid.CurrentRow.Selected
                ? grid.CurrentRow.DataBoundItem as CanhBaoKiemTraNative : null;
        }

        private void CapNhatChiTiet()
        {
            CanhBaoKiemTraNative x = DangChon();
            bool co = x != null && !string.IsNullOrWhiteSpace(x.MaDonVi);
            btnXemHoSo.Enabled = btnBanDo.Enabled = co;
            NutGiaoDien.DoiMau(btnXemHoSo, co ? XanhDuongNhat : Color.FromArgb(240, 244, 249), co ? TienIchGiaoDienSang.Blue : Color.FromArgb(165, 180, 198));
            NutGiaoDien.DoiMau(btnBanDo, co ? TienIchGiaoDienSang.Teal : Color.FromArgb(240, 244, 249), co ? Color.White : Color.FromArgb(165, 180, 198));
            if (x == null)
            {
                lblCtTen.Text = hienThi.Count > 0 ? "Chọn một dòng để xem chi tiết và khuyến nghị xử lý." : "";
                lblCtMoTa.Text = "";
                lblCtGoiY.Text = "";
                return;
            }
            lblCtTen.Text = (x.TenDonVi ?? "—") + "  ·  " + (x.MaDonVi ?? "");
            lblCtMoTa.Text = (x.LoaiLoi ?? "") + ": " + (x.NoiDung ?? "");
            lblCtGoiY.Text = string.IsNullOrWhiteSpace(x.KhuyenNghi) ? "" : "➜  " + x.KhuyenNghi;
        }

        private void Raise(Action<string> eventHandler)
        {
            CanhBaoKiemTraNative item = DangChon();
            if (item != null && eventHandler != null &&
                !string.IsNullOrWhiteSpace(item.MaDonVi)) eventHandler(item.MaDonVi);
        }

        private void Export()
        {
            if (hienThi.Count == 0) return;
            var rows = new List<string[]> { new[] { "Mức độ", "Mã đơn vị", "Tên đơn vị",
                "Loại lỗi", "Nội dung", "Khuyến nghị" } };
            rows.AddRange(hienThi.Select(x => new[] { x.MucDo, x.MaDonVi, x.TenDonVi,
                x.LoaiLoi, x.NoiDung, x.KhuyenNghi }));
            TienIchGiaoDienSang.ExportCsv("KiemTraDuLieuGIS.csv", rows);
        }

        // =================================================
        // BANG
        // =================================================

        private DataGridView TaoBang()
        {
            DataGridView g = new DataGridView
            {
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
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(236, 243, 249),
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            };
            g.RowTemplate.Height = 36;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.ForeColor = TienIchGiaoDienSang.Muted;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.6F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9.2F);
            g.DefaultCellStyle.ForeColor = TienIchGiaoDienSang.Ink;
            g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.SelectionBackColor = XanhNhat;
            g.DefaultCellStyle.SelectionForeColor = TienIchGiaoDienSang.Ink;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 254);

            g.Columns.Add(Cot("Mức độ", "MucDo", 13));
            DataGridViewTextBoxColumn ma = Cot("Mã hồ sơ", "MaDonVi", 14);
            ma.DefaultCellStyle.ForeColor = TienIchGiaoDienSang.Muted;
            ma.DefaultCellStyle.Font = new Font("Segoe UI", 8.6F);
            g.Columns.Add(ma);
            g.Columns.Add(Cot("Tên đơn vị", "TenDonVi", 24));
            DataGridViewTextBoxColumn loai = Cot("Loại lỗi", "LoaiLoi", 18);
            loai.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            g.Columns.Add(loai);
            g.Columns.Add(Cot("Mô tả", "NoiDung", 31));

            // Cot "Muc do": ve nhan tron mau (do = nghiem trong, cam = canh bao).
            g.CellPainting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 0) return;
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                string md = e.Value as string ?? "";
                bool nt = string.Equals(md, NghiemTrong, StringComparison.CurrentCultureIgnoreCase);
                using (Font f = new Font("Segoe UI", 8.2F, FontStyle.Bold))
                {
                    Size kt = TextRenderer.MeasureText(md, f);
                    RectangleF r = new RectangleF(e.CellBounds.Left + 8, e.CellBounds.Top + (e.CellBounds.Height - 22) / 2f,
                        Math.Min(e.CellBounds.Width - 14, kt.Width + 14), 22);
                    GraphicsState truoc = e.Graphics.Save();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SolidBrush b = new SolidBrush(nt ? DoNhat : CamNhat))
                    using (GraphicsPath p = VeBaoCao.BoTron(r, 11)) e.Graphics.FillPath(b, p);
                    e.Graphics.Restore(truoc);
                    TextRenderer.DrawText(e.Graphics, md, f, Rectangle.Round(r), nt ? Do : Cam,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                e.Handled = true;
            };
            g.SelectionChanged += (s, e) => CapNhatChiTiet();
            g.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Raise(YeuCauDinhVi); };
            return g;
        }

        private static DataGridViewTextBoxColumn Cot(string tieuDe, string duLieu, float tyLe)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = tieuDe,
                DataPropertyName = duLieu,
                FillWeight = tyLe,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }

        private static bool LaNghiemTrong(CanhBaoKiemTraNative x)
        {
            return string.Equals(x.MucDo, NghiemTrong, StringComparison.CurrentCultureIgnoreCase);
        }

        private static string PhanTram(int a, int b)
        {
            return b > 0 ? (100d * a / b).ToString("N1", VeBaoCao.Vi) + "%" : "—";
        }

        private static bool Contains(string s, string text)
        {
            return TienIchGiaoDienSang.Vi.CompareInfo.IndexOf(s ?? "", text,
                System.Globalization.CompareOptions.IgnoreCase |
                System.Globalization.CompareOptions.IgnoreNonSpace) >= 0;
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer.Dispose();
                choToiDa.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
