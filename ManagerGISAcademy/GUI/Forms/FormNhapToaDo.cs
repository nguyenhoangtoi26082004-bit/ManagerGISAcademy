using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>Mot dong toa do nhap (sau khi doc tep va kiem tra).</summary>
    public sealed class DongNhapToaDo
    {
        public int Stt { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double C1 { get; set; }          // E / kinh do (theo he da chon)
        public double C2 { get; set; }          // N / vi do
        public string PhuongPhap { get; set; }
        public decimal? DoChinhXac { get; set; }
        public string Nguon { get; set; }
        public string GhiChu { get; set; }
        public bool HopLe { get; set; }
        public string ThongBao { get; set; }
        public string TenXa { get; set; }
        public double? KhoangDichM { get; set; }
        public double? E { get; set; }
        public double? N { get; set; }
        /// <summary>Tep dat ten cot "x"/"y" (quy uoc VN-2000: X = Bac).</summary>
        public bool Tag_XY { get; set; }

        public string ToaDo { get { return C1.ToString("0.######", CultureInfo.InvariantCulture) + " ; " + C2.ToString("0.######", CultureInfo.InvariantCulture); } }
        public string KetQua { get { return ThongBao == null ? "Chưa kiểm tra" : HopLe ? "Hợp lệ" : "Lỗi"; } }
        public string Doi { get { return KhoangDichM.HasValue ? KhoangDichM.Value.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " m" : "—"; } }
    }

    /// <summary>
    /// Nhap toa do tru so hang loat tu tep GNSS / CSV / TXT / KML / GPX.
    /// Moi diem kiem tra bang ql_don_vi.fn_kiem_tra_nhap_toa_do (PostGIS);
    /// ghi bang fn_dinh_vi_tru_so (giu lich su, tu tinh lai phuong/xa,
    /// vai tro Bien tap -> xep hang cho duyet).
    /// </summary>
    public sealed class FormNhapToaDo : HopThoaiV2
    {
        private static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");
        private readonly ComboBox cboHe, cboPhuongPhap;
        private readonly NumericUpDown nudSaiSo;
        private readonly TextBox txtNguon;
        private readonly CheckBox chkXBac;
        private readonly Label lblTep, lblTomTat;
        private readonly DataGridView grid;
        private readonly Button btnGhi, btnBanDo, btnKiemTra;
        private readonly List<DongNhapToaDo> dong = new List<DongNhapToaDo>();
        private IList<PhuongPhapDinhVi> dsPhuongPhap = new List<PhuongPhapDinhVi>();
        private string tenTep = "";

        /// <summary>Danh sach diem (E, N 32648, nhan) de ve lop xem truoc.</summary>
        public event Action<List<Tuple<double, double, string, bool>>> YeuCauHienTrenBanDo;
        /// <summary>Da ghi it nhat 1 diem -> FormMain nap lai ban do.</summary>
        public event Action DaGhi;

        private static readonly int[] Srid = { 4326, 5897, 3405, 32648 };

        public FormNhapToaDo()
            : base("Nhập tọa độ trụ sở hàng loạt", "Tệp GNSS / CSV / TXT / KML / GPX · kiểm tra bằng PostGIS trước khi ghi · giữ lịch sử vị trí", new Size(1080, 680))
        {
            // ---------- thiet lap ----------
            TableLayoutPanel tl = new TableLayoutPanel { Dock = DockStyle.Top, Height = 104, ColumnCount = 6, RowCount = 2, BackColor = TienIchGiaoDienSang.Background };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));

            Button btnTep = NutGiaoDien.Phang("📂  Chọn tệp…", VeBaoCao.Blue, Color.White, 9.2F);
            btnTep.Dock = DockStyle.Fill; btnTep.Margin = new Padding(0, 6, 10, 6);
            lblTep = Nhan("Chưa chọn tệp. Cột nhận dạng: ma_don_vi, x/e/kinh_do, y/n/vi_do, [phuong_phap], [do_chinh_xac], [nguon], [ghi_chu]", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblTep.Dock = DockStyle.Fill;
            tl.Controls.Add(btnTep, 0, 0);
            tl.Controls.Add(lblTep, 1, 0);
            tl.SetColumnSpan(lblTep, 3);
            chkXBac = new CheckBox { Text = "Cột X là tọa độ Bắc (VN-2000)", Checked = true, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 8.8F), ForeColor = TienIchGiaoDienSang.Ink };
            tl.Controls.Add(chkXBac, 4, 0);
            tl.SetColumnSpan(chkXBac, 2);

            Label l1 = Nhan("Hệ tọa độ của tệp", 9F, true); l1.Dock = DockStyle.Fill;
            cboHe = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.4F), Margin = new Padding(0, 11, 10, 0) };
            cboHe.Items.AddRange(new object[]
            {
                "WGS84 – kinh độ, vĩ độ (EPSG:4326)",
                "VN-2000 TM-3 KTT 105°00′ (EPSG:5897)",
                "VN-2000 UTM múi 48 (EPSG:3405)",
                "WGS84 UTM 48N – E, N (EPSG:32648)"
            });
            cboHe.SelectedIndex = 0;
            Label l2 = Nhan("Phương pháp", 9F, true); l2.Dock = DockStyle.Fill;
            cboPhuongPhap = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.4F), Margin = new Padding(0, 11, 10, 0) };
            Label l3 = Nhan("Sai số (m)", 9F, true); l3.Dock = DockStyle.Fill;
            nudSaiSo = new NumericUpDown { Dock = DockStyle.Fill, DecimalPlaces = 2, Minimum = 0.01m, Maximum = 5000m, Value = 5m, Font = new Font("Segoe UI", 9.4F), Margin = new Padding(0, 11, 0, 0) };
            tl.Controls.Add(l1, 0, 1); tl.Controls.Add(cboHe, 1, 1);
            tl.Controls.Add(l2, 2, 1); tl.Controls.Add(cboPhuongPhap, 3, 1);
            tl.Controls.Add(l3, 4, 1); tl.Controls.Add(nudSaiSo, 5, 1);

            Panel hangNguon = new Panel { Dock = DockStyle.Top, Height = 42, BackColor = TienIchGiaoDienSang.Background, Padding = new Padding(0, 4, 0, 6) };
            Label l4 = Nhan("Nguồn số liệu", 9F, true); l4.Dock = DockStyle.Left; l4.Width = 150;
            txtNguon = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9.4F), Text = "Đo GNSS ngày " + DateTime.Now.ToString("dd/MM/yyyy") };
            hangNguon.Controls.Add(txtNguon); hangNguon.Controls.Add(l4);

            lblTomTat = Nhan("", 9F, true, VeBaoCao.Teal);
            lblTomTat.Dock = DockStyle.Bottom; lblTomTat.Height = 28;

            NativeCard card = TienIchGiaoDienSang.Card();
            card.Dock = DockStyle.Fill; card.Padding = new Padding(10);
            grid = BangGiaoDien.Tao();
            BangGiaoDien.Cot(grid, "#", "Stt", 5);
            BangGiaoDien.Cot(grid, "Kết quả", "KetQua", 11);
            BangGiaoDien.Cot(grid, "Mã đơn vị", "MaDonVi", 14, false, true);
            BangGiaoDien.Cot(grid, "Tên đơn vị", "TenDonVi", 22);
            BangGiaoDien.Cot(grid, "Tọa độ gốc", "ToaDo", 18);
            BangGiaoDien.Cot(grid, "Phường/xã", "TenXa", 13);
            BangGiaoDien.Cot(grid, "Dời", "Doi", 8, true);
            BangGiaoDien.Cot(grid, "Ghi chú kiểm tra", "ThongBao", 22);
            BangGiaoDien.VeNhanTron(grid, 1, s => s == "Hợp lệ" ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal)
                : s == "Lỗi" ? Tuple.Create(BangGiaoDien.DoNhat, BangGiaoDien.Do) : Tuple.Create(BangGiaoDien.CamNhat, BangGiaoDien.Cam));
            card.Controls.Add(grid);

            Than.Controls.Add(card);
            Than.Controls.Add(lblTomTat);
            Than.Controls.Add(hangNguon);
            Than.Controls.Add(tl);

            btnGhi = ThemNut("✓  Ghi các điểm hợp lệ", VeBaoCao.Teal, Color.White, 200, async (s, e) => await Ghi());
            btnBanDo = ThemNut("⌖  Xem trên bản đồ", BangGiaoDien.XanhDuongNhat, VeBaoCao.Blue, 160, (s, e) => HienBanDo());
            btnKiemTra = ThemNut("↻  Kiểm tra lại", Color.White, TienIchGiaoDienSang.Ink, 130, async (s, e) => await KiemTra());
            ThemNut("Tải tệp mẫu CSV", Color.White, TienIchGiaoDienSang.Ink, 140, (s, e) => LuuMau());
            ThemNut("Đóng", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => Close());
            btnGhi.Enabled = btnBanDo.Enabled = btnKiemTra.Enabled = false;

            btnTep.Click += async (s, e) => await ChonTep();
            cboHe.SelectedIndexChanged += (s, e) => chkXBac.Enabled = cboHe.SelectedIndex == 1 || cboHe.SelectedIndex == 2;
            chkXBac.Enabled = false;
            Shown += async (s, e) => await NapPhuongPhap();
        }

        private async Task NapPhuongPhap()
        {
            try
            {
                dsPhuongPhap = await Task.Run(() => new TruyXuatViTri().LayPhuongPhap());
                cboPhuongPhap.Items.Clear();
                foreach (PhuongPhapDinhVi p in dsPhuongPhap.Where(x => x.LaViTriThuc)) cboPhuongPhap.Items.Add(p);
                int i = dsPhuongPhap.Where(x => x.LaViTriThuc).ToList().FindIndex(x => x.Ma == "GNSS_CAM_TAY");
                cboPhuongPhap.SelectedIndex = i >= 0 ? i : 0;
            }
            catch (Exception ex) { lblTomTat.Text = "Không đọc được danh mục phương pháp: " + ex.Message; lblTomTat.ForeColor = BangGiaoDien.Do; }
        }

        // ---------------- doc tep ----------------

        private async Task ChonTep()
        {
            using (OpenFileDialog d = new OpenFileDialog
            {
                Title = "Chọn tệp tọa độ",
                Filter = "Tệp tọa độ (*.csv;*.txt;*.kml;*.gpx)|*.csv;*.txt;*.kml;*.gpx|Tất cả (*.*)|*.*"
            })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    dong.Clear();
                    tenTep = Path.GetFileName(d.FileName);
                    string duoi = Path.GetExtension(d.FileName).ToLowerInvariant();
                    if (duoi == ".kml") DocKml(d.FileName);
                    else if (duoi == ".gpx") DocGpx(d.FileName);
                    else DocCsv(d.FileName);
                    lblTep.Text = tenTep + "  ·  " + dong.Count + " điểm";
                    lblTep.ForeColor = TienIchGiaoDienSang.Ink;
                    await KiemTra();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Không đọc được tệp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private static string ChuanHoa(string s)
        {
            string t = (s ?? "").Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            StringBuilder b = new StringBuilder();
            foreach (char c in t)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && (char.IsLetterOrDigit(c) || c == '_')) b.Append(c);
            return b.ToString();
        }

        private static bool DocSo(string s, out double v)
        {
            s = (s ?? "").Trim().Trim('"');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v)) return true;
            return double.TryParse(s, NumberStyles.Float, Vi, out v);
        }

        private void DocCsv(string tep)
        {
            string[] hang = File.ReadAllLines(tep, Encoding.UTF8).Where(x => x.Trim().Length > 0).ToArray();
            if (hang.Length < 2) throw new InvalidDataException("Tệp cần dòng tiêu đề và ít nhất 1 dòng dữ liệu.");
            char[] ung = { ';', '\t', ',', '|' };
            char sep = ung.OrderByDescending(c => hang[0].Count(x => x == c)).First();
            string[] td = hang[0].Split(sep).Select(ChuanHoa).ToArray();
            Func<string[], int> tim = ten => Array.FindIndex(td, x => ten.Contains(x));
            int iMa = tim(new[] { "ma", "madonvi", "ma_don_vi", "code", "name", "ten", "id" });
            int iX = tim(new[] { "x", "e", "easting", "kinhdo", "kinh_do", "lon", "lng", "longitude" });
            int iY = tim(new[] { "y", "n", "northing", "vido", "vi_do", "lat", "latitude" });
            int iPp = tim(new[] { "phuongphap", "phuong_phap", "method" });
            int iSs = tim(new[] { "dochinhxac", "do_chinh_xac", "saiso", "sai_so", "accuracy" });
            int iNg = tim(new[] { "nguon", "source" });
            int iGc = tim(new[] { "ghichu", "ghi_chu", "note" });
            if (iMa < 0 || iX < 0 || iY < 0)
                throw new InvalidDataException("Không tìm thấy cột mã đơn vị và 2 cột tọa độ. Tiêu đề đọc được: " + string.Join(" | ", td));
            bool laXy = td[iX] == "x" && td[iY] == "y";   // VN-2000: X la Bac
            for (int i = 1; i < hang.Length; i++)
            {
                string[] c = hang[i].Split(sep);
                if (c.Length <= Math.Max(iMa, Math.Max(iX, iY))) continue;
                double a, b;
                DongNhapToaDo d = new DongNhapToaDo { Stt = dong.Count + 1, MaDonVi = c[iMa].Trim().Trim('"') };
                if (!DocSo(c[iX], out a) || !DocSo(c[iY], out b)) { d.HopLe = false; d.ThongBao = "Tọa độ không phải số"; dong.Add(d); continue; }
                d.C1 = a; d.C2 = b;
                d.Tag_XY = laXy;
                if (iPp >= 0 && iPp < c.Length) d.PhuongPhap = c[iPp].Trim().Trim('"').ToUpperInvariant();
                decimal ss;
                if (iSs >= 0 && iSs < c.Length && decimal.TryParse(c[iSs].Trim().Trim('"'), NumberStyles.Float, CultureInfo.InvariantCulture, out ss)) d.DoChinhXac = ss;
                if (iNg >= 0 && iNg < c.Length) d.Nguon = c[iNg].Trim().Trim('"');
                if (iGc >= 0 && iGc < c.Length) d.GhiChu = c[iGc].Trim().Trim('"');
                dong.Add(d);
            }
        }

        private void DocKml(string tep)
        {
            XmlDocument x = new XmlDocument();
            x.Load(tep);
            XmlNamespaceManager ns = new XmlNamespaceManager(x.NameTable);
            ns.AddNamespace("k", x.DocumentElement.NamespaceURI);
            string p = string.IsNullOrEmpty(x.DocumentElement.NamespaceURI) ? "//Placemark" : "//k:Placemark";
            foreach (XmlNode pm in x.SelectNodes(p, ns))
            {
                XmlNode ten = pm.SelectSingleNode(string.IsNullOrEmpty(x.DocumentElement.NamespaceURI) ? "name" : "k:name", ns);
                XmlNode toa = pm.SelectSingleNode(string.IsNullOrEmpty(x.DocumentElement.NamespaceURI) ? ".//Point/coordinates" : ".//k:Point/k:coordinates", ns);
                if (ten == null || toa == null) continue;
                string[] c = toa.InnerText.Trim().Split(',');
                double lon, lat;
                if (c.Length < 2 || !DocSo(c[0], out lon) || !DocSo(c[1], out lat)) continue;
                dong.Add(new DongNhapToaDo { Stt = dong.Count + 1, MaDonVi = ten.InnerText.Trim(), C1 = lon, C2 = lat });
            }
            cboHe.SelectedIndex = 0;   // KML luon WGS84
        }

        private void DocGpx(string tep)
        {
            XmlDocument x = new XmlDocument();
            x.Load(tep);
            foreach (XmlNode w in x.GetElementsByTagName("wpt"))
            {
                double lat, lon;
                if (!DocSo(w.Attributes["lat"].Value, out lat) || !DocSo(w.Attributes["lon"].Value, out lon)) continue;
                XmlNode ten = w.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.LocalName == "name");
                if (ten == null) continue;
                dong.Add(new DongNhapToaDo { Stt = dong.Count + 1, MaDonVi = ten.InnerText.Trim(), C1 = lon, C2 = lat });
            }
            cboHe.SelectedIndex = 0;
        }

        // ---------------- kiem tra ----------------

        private PhuongPhapDinhVi PhuongPhapCua(DongNhapToaDo d)
        {
            string ma = string.IsNullOrWhiteSpace(d.PhuongPhap) ? ((PhuongPhapDinhVi)cboPhuongPhap.SelectedItem)?.Ma : d.PhuongPhap;
            return dsPhuongPhap.FirstOrDefault(p => string.Equals(p.Ma, ma, StringComparison.OrdinalIgnoreCase));
        }

        private void TinhEN(DongNhapToaDo d, out int srid, out double e, out double n)
        {
            srid = Srid[Math.Max(0, cboHe.SelectedIndex)];
            e = d.C1; n = d.C2;
            if ((srid == 5897 || srid == 3405) && chkXBac.Checked && d.Tag_XY) { e = d.C2; n = d.C1; }
        }

        private async Task KiemTra()
        {
            if (dong.Count == 0) return;
            btnGhi.Enabled = btnBanDo.Enabled = btnKiemTra.Enabled = false;
            lblTomTat.Text = "Đang kiểm tra " + dong.Count + " điểm bằng PostGIS…";
            lblTomTat.ForeColor = BangGiaoDien.Cam;
            TruyXuatTracDia dal = new TruyXuatTracDia();
            List<DongNhapToaDo> ds = dong.ToList();
            int[] he = new int[ds.Count]; double[] ee = new double[ds.Count]; double[] nn = new double[ds.Count];
            for (int i = 0; i < ds.Count; i++) TinhEN(ds[i], out he[i], out ee[i], out nn[i]);
            decimal saiSoMacDinh = nudSaiSo.Value;
            await Task.Run(() =>
            {
                for (int i = 0; i < ds.Count; i++)
                {
                    DongNhapToaDo d = ds[i];
                    if (d.ThongBao == "Tọa độ không phải số") continue;
                    try
                    {
                        KetQuaKiemTraToaDo k = dal.KiemTraToaDo(d.MaDonVi, he[i], ee[i], nn[i]);
                        d.HopLe = k.HopLe; d.ThongBao = k.ThongBao; d.TenDonVi = k.TenDonVi;
                        d.TenXa = k.TenXaPhuong; d.KhoangDichM = k.KhoangDichM; d.E = k.E; d.N = k.N;
                    }
                    catch (Exception ex) { d.HopLe = false; d.ThongBao = ex.Message; }
                }
            });
            // Sai so phai >= muc danh nghia cua phuong phap (trigger CSDL se chan).
            foreach (DongNhapToaDo d in dong.Where(x => x.HopLe))
            {
                PhuongPhapDinhVi pp = PhuongPhapCua(d);
                decimal ss = d.DoChinhXac ?? saiSoMacDinh;
                if (pp == null) { d.HopLe = false; d.ThongBao = "Phương pháp \"" + d.PhuongPhap + "\" không có trong danh mục"; }
                else if (ss < pp.DoChinhXacDanhNghiaM) { d.HopLe = false; d.ThongBao = "Sai số " + ss + " m nhỏ hơn mức của " + pp.Ma + " (" + pp.DoChinhXacDanhNghiaM + " m)"; }
            }
            grid.DataSource = new BindingList<DongNhapToaDo>(dong);
            grid.ClearSelection();
            int ok = dong.Count(x => x.HopLe);
            lblTomTat.Text = "✓ " + ok + " hợp lệ   ·   ✕ " + (dong.Count - ok) + " lỗi   ·   tổng " + dong.Count + " điểm";
            lblTomTat.ForeColor = ok == dong.Count ? VeBaoCao.Teal : (ok == 0 ? BangGiaoDien.Do : BangGiaoDien.Cam);
            btnGhi.Enabled = ok > 0;
            btnBanDo.Enabled = dong.Any(x => x.E.HasValue);
            btnKiemTra.Enabled = true;
        }

        private void HienBanDo()
        {
            Action<List<Tuple<double, double, string, bool>>> h = YeuCauHienTrenBanDo;
            if (h == null) return;
            h(dong.Where(x => x.E.HasValue && x.N.HasValue)
                  .Select(x => Tuple.Create(x.E.Value, x.N.Value, x.MaDonVi, x.HopLe)).ToList());
        }

        // ---------------- ghi ----------------

        private async Task Ghi()
        {
            List<DongNhapToaDo> hopLe = dong.Where(x => x.HopLe).ToList();
            if (hopLe.Count == 0) return;
            if (MessageBox.Show(this, "Ghi vị trí mới cho " + hopLe.Count + " đơn vị?\nVị trí cũ được giữ trong lịch sử; phường/xã tự cập nhật theo tọa độ mới.",
                    "Xác nhận ghi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            btnGhi.Enabled = false;
            string nguonChung = txtNguon.Text.Trim();
            if (nguonChung.Length == 0) nguonChung = "Nhập hàng loạt từ " + tenTep;
            decimal ssMacDinh = nudSaiSo.Value;
            List<Tuple<DongNhapToaDo, int, double, double, string, decimal>> viec = hopLe.Select(d =>
            {
                int srid; double e, n; TinhEN(d, out srid, out e, out n);
                return Tuple.Create(d, srid, e, n, PhuongPhapCua(d).Ma, d.DoChinhXac ?? ssMacDinh);
            }).ToList();
            int thanhCong = 0, choDuyet = 0, loi = 0;
            await Task.Run(() =>
            {
                TruyXuatViTri dal = new TruyXuatViTri();
                foreach (var v in viec)
                {
                    try
                    {
                        KetQuaDinhVi r = dal.DinhViTruSo(v.Item1.MaDonVi, v.Item2, v.Item3, v.Item4, v.Item5, v.Item6,
                            string.IsNullOrWhiteSpace(v.Item1.Nguon) ? nguonChung : v.Item1.Nguon,
                            Environment.UserName, (v.Item1.GhiChu ?? "") + " [nhập từ " + tenTep + "]");
                        if (r.ViTriId < 0) { choDuyet++; v.Item1.ThongBao = "Đã gửi yêu cầu #" + (-r.ViTriId) + ", chờ duyệt"; }
                        else { thanhCong++; v.Item1.ThongBao = "Đã ghi · " + r.TenXaPhuong; }
                    }
                    catch (Exception ex) { loi++; v.Item1.HopLe = false; v.Item1.ThongBao = "Lỗi ghi: " + ex.Message; }
                }
            });
            grid.Refresh();
            lblTomTat.Text = "Đã ghi " + thanhCong + " · chờ duyệt " + choDuyet + " · lỗi " + loi;
            lblTomTat.ForeColor = loi == 0 ? VeBaoCao.Teal : BangGiaoDien.Cam;
            if (thanhCong + choDuyet > 0 && DaGhi != null) DaGhi();
        }

        private void LuuMau()
        {
            using (SaveFileDialog d = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "mau_nhap_toa_do.csv" })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                File.WriteAllText(d.FileName,
                    "ma_don_vi;kinh_do;vi_do;phuong_phap;do_chinh_xac;nguon;ghi_chu\n" +
                    "HN-HVQP-001;105.801877;21.042855;GNSS_CAM_TAY;5;Đo GNSS cổng chính;Mẫu\n" +
                    "HN-HVKTQS-001;105.785668;21.048374;SO_HOA_ANH;3;Ảnh vệ tinh 2 m;Mẫu\n", new UTF8Encoding(true));
            }
        }
    }
}
