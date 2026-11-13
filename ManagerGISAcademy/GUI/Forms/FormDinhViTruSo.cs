using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Hop thoai "Dinh vi lai tru so".
    ///
    /// Nhap toa do theo 3 he: WGS84 (do), VN-2000 TM-3 KTT 105° (EPSG:5897,
    /// quy uoc VN: X = Bac, Y = Dong), WGS84/UTM 48N (EPSG:32648).
    /// Toa do goc duoc luu NGUYEN nhu nhap (srid_goc, e_goc, n_goc); CSDL tu
    /// quy doi, kiem tra mien gia tri, xa/phuong va do chinh xac.
    ///
    /// "Chon tren ban do": hop thoai dong tam (YeuCauChonTrenBanDo = true),
    /// FormMain bat che do nhap diem roi goi DatDiemTuBanDo va mo lai.
    /// </summary>
    public sealed class FormDinhViTruSo : Form
    {
        private static readonly Color MauChinh = Color.FromArgb(12, 160, 140);
        private static readonly Color MauChu = Color.FromArgb(30, 45, 60);
        private static readonly Color MauPhu = Color.FromArgb(96, 112, 128);

        private readonly TruyXuatViTri dal = new TruyXuatViTri();
        private readonly ViTriHienHanh hienHanh;

        private readonly ComboBox cboHe = new ComboBox();
        private readonly Label lblO1 = new Label();
        private readonly Label lblO2 = new Label();
        private readonly TextBox txtO1 = new TextBox();
        private readonly TextBox txtO2 = new TextBox();
        private readonly Label lblQuyDoi = new Label();
        private readonly ComboBox cboPhuongPhap = new ComboBox();
        private readonly NumericUpDown numDoChinhXac = new NumericUpDown();
        private readonly TextBox txtNguon = new TextBox();
        private readonly TextBox txtNguoi = new TextBox();
        private readonly TextBox txtGhiChu = new TextBox();
        private readonly Button btnLuu = new Button();

        private KetQuaQuyDoi ketQuaKiemTra;

        public string MaDonVi { get; private set; }

        /// <summary>True khi nguoi dung bam "Chon tren ban do".</summary>
        public bool YeuCauChonTrenBanDo { get; private set; }

        /// <summary>Ket qua sau khi luu thanh cong (null neu chua luu).</summary>
        public KetQuaDinhVi KetQua { get; private set; }

        private sealed class HeToaDo
        {
            public int Srid;
            public string Ten;
            public string Nhan1;   // o thu nhat
            public string Nhan2;
            public bool O1LaBac;   // VN-2000: o 1 la X (Bac)
            public override string ToString() { return Ten; }
        }

        public FormDinhViTruSo(ViTriHienHanh viTri)
        {
            if (viTri == null) throw new ArgumentNullException("viTri");
            hienHanh = viTri;
            MaDonVi = viTri.MaDonVi;
            TaoGiaoDien();
            NapDanhMuc();
            DatToaDoTheoHe(viTri.KinhDo, viTri.ViDo, viTri.E32648, viTri.N32648,
                viTri.XVn2000, viTri.YVn2000);
            // Chua kiem tra toa do / chua co nguon -> chua cho luu.
            CapNhatNutLuu();
        }

        // =====================================================
        // 1. GIAO DIEN
        // =====================================================

        private void TaoGiaoDien()
        {
            Text = "Định vị lại trụ sở";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(560, 640);
            BackColor = Color.FromArgb(249, 252, 255);
            Font = new Font("Segoe UI", 9.5F);

            Panel dau = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = MauChinh };
            dau.Controls.Add(new Label
            {
                Text = hienHanh.TenDonVi,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoEllipsis = true,
                Left = 18, Top = 10, Width = 520, Height = 28
            });
            dau.Controls.Add(new Label
            {
                Text = hienHanh.MaDonVi + "  ·  Hiện tại: " + MoTaHienHanh(),
                ForeColor = Color.FromArgb(225, 255, 250),
                AutoEllipsis = true,
                Left = 18, Top = 40, Width = 520, Height = 24
            });

            TableLayoutPanel bang = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(18, 14, 18, 6),
                AutoScroll = true
            };
            bang.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            cboHe.DropDownStyle = ComboBoxStyle.DropDownList;
            cboHe.Items.Add(new HeToaDo { Srid = TruyXuatViTri.SridWgs84, Ten = "WGS84 – kinh độ, vĩ độ (độ thập phân)", Nhan1 = "Kinh độ (°E)", Nhan2 = "Vĩ độ (°N)" });
            cboHe.Items.Add(new HeToaDo { Srid = TruyXuatViTri.SridVn2000Tm3, Ten = "VN-2000 / TM-3 KTT 105° (EPSG:5897)", Nhan1 = "X – Bắc (m)", Nhan2 = "Y – Đông (m)", O1LaBac = true });
            cboHe.Items.Add(new HeToaDo { Srid = TruyXuatViTri.SridUtm48N, Ten = "WGS84 / UTM 48N (EPSG:32648)", Nhan1 = "E – Đông (m)", Nhan2 = "N – Bắc (m)" });
            cboHe.SelectedIndex = 0;
            cboHe.SelectedIndexChanged += (s, e) => DoiHeToaDo();

            ThemDong(bang, "Hệ tọa độ", cboHe);
            ThemDong(bang, lblO1, txtO1);
            ThemDong(bang, lblO2, txtO2);
            txtO1.TextChanged += (s, e) => DanhDauChuaKiemTra();
            txtO2.TextChanged += (s, e) => DanhDauChuaKiemTra();

            FlowLayoutPanel nutToaDo = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(0, 2, 0, 2) };
            Button btnBanDo = TaoNut("⌖  Chọn trên bản đồ", Color.FromArgb(45, 145, 194));
            btnBanDo.Click += (s, e) => { YeuCauChonTrenBanDo = true; DialogResult = DialogResult.Retry; };
            Button btnKiemTra = TaoNut("✓  Kiểm tra / quy đổi", Color.FromArgb(88, 113, 234));
            btnKiemTra.Click += (s, e) => KiemTra(true);
            nutToaDo.Controls.Add(btnBanDo);
            nutToaDo.Controls.Add(btnKiemTra);
            ThemDong(bang, "", nutToaDo);

            lblQuyDoi.AutoSize = false;
            lblQuyDoi.Height = 96;
            lblQuyDoi.Dock = DockStyle.Fill;
            lblQuyDoi.ForeColor = MauPhu;
            lblQuyDoi.BackColor = Color.White;
            lblQuyDoi.Padding = new Padding(8, 6, 8, 6);
            lblQuyDoi.Font = new Font("Consolas", 9F);
            ThemDong(bang, "Quy đổi", lblQuyDoi);

            cboPhuongPhap.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhuongPhap.SelectedIndexChanged += (s, e) => DoiPhuongPhap();
            ThemDong(bang, "Phương pháp", cboPhuongPhap);

            numDoChinhXac.DecimalPlaces = 2;
            numDoChinhXac.Increment = 0.5M;
            numDoChinhXac.Minimum = 0.05M;
            numDoChinhXac.Maximum = 5000M;
            numDoChinhXac.Width = 120;
            ThemDong(bang, "Độ chính xác (m)", numDoChinhXac);

            ThemDong(bang, "Nguồn (*)", txtNguon);
            txtNguon.TextChanged += (s, e) => CapNhatNutLuu();
            ThemDong(bang, "Người xác minh", txtNguoi);
            txtGhiChu.Multiline = true;
            txtGhiChu.Height = 48;
            ThemDong(bang, "Ghi chú", txtGhiChu);

            Label goiY = new Label
            {
                Text = "Lấy điểm tại CỔNG CHÍNH trụ sở. Nguồn ghi rõ: ảnh vệ tinh (ngày chụp), máy GNSS, " +
                       "tờ bản đồ... Vị trí cũ được giữ làm lịch sử; phường/xã tự cập nhật theo vị trí mới.",
                ForeColor = MauPhu,
                AutoSize = false,
                Height = 44,
                Dock = DockStyle.Fill
            };
            bang.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bang.Controls.Add(goiY, 0, bang.RowCount);
            bang.SetColumnSpan(goiY, 2);
            bang.RowCount++;

            FlowLayoutPanel chan = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(12, 10, 12, 10),
                BackColor = Color.White
            };
            Button btnHuy = TaoNut("Hủy", Color.FromArgb(150, 160, 170));
            btnHuy.DialogResult = DialogResult.Cancel;
            btnLuu.Text = "✔  Lưu vị trí mới";
            DatKieuNut(btnLuu, MauChinh);
            btnLuu.Click += (s, e) => Luu();
            chan.Controls.Add(btnHuy);
            chan.Controls.Add(btnLuu);
            CancelButton = btnHuy;

            Controls.Add(bang);
            Controls.Add(chan);
            Controls.Add(dau);

            DoiHeToaDo();
        }

        private static void ThemDong(TableLayoutPanel bang, string nhan, Control dieuKhien)
        {
            ThemDong(bang, new Label(), dieuKhien, nhan);
        }

        private static void ThemDong(TableLayoutPanel bang, Label nhan, Control dieuKhien, string chu = null)
        {
            if (chu != null) nhan.Text = chu;
            nhan.ForeColor = MauChu;
            nhan.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            nhan.TextAlign = ContentAlignment.MiddleLeft;
            nhan.Dock = DockStyle.Fill;
            if (!(dieuKhien is NumericUpDown) && !(dieuKhien is FlowLayoutPanel))
                dieuKhien.Dock = DockStyle.Fill;
            dieuKhien.Margin = new Padding(3, 5, 3, 5);
            int hang = bang.RowCount;
            bang.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            bang.Controls.Add(nhan, 0, hang);
            bang.Controls.Add(dieuKhien, 1, hang);
            bang.RowCount = hang + 1;
        }

        private static Button TaoNut(string chu, Color mau)
        {
            Button b = new Button { Text = chu };
            DatKieuNut(b, mau);
            return b;
        }

        private static void DatKieuNut(Button b, Color mau)
        {
            b.AutoSize = true;
            b.Height = 34;
            b.Padding = new Padding(10, 2, 10, 2);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.BackColor = mau;
            b.ForeColor = Color.White;
            b.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.Margin = new Padding(4, 0, 4, 0);
        }

        private string MoTaHienHanh()
        {
            string pp = string.IsNullOrWhiteSpace(hienHanh.TenPhuongPhap) ? "chưa rõ" : hienHanh.TenPhuongPhap;
            return hienHanh.LaViTriThuc
                ? pp + (hienHanh.DoChinhXacM.HasValue ? " ±" + hienHanh.DoChinhXacM.Value.ToString("0.##") + " m" : "") + " · " + hienHanh.DiaBan
                : "điểm đại diện " + hienHanh.DiaBan + " (chưa phải trụ sở)";
        }

        // =====================================================
        // 2. DANH MUC VA HE TOA DO
        // =====================================================

        private void NapDanhMuc()
        {
            foreach (PhuongPhapDinhVi p in dal.LayPhuongPhap())
                cboPhuongPhap.Items.Add(p);

            // Mac dinh: so hoa tren anh ve tinh.
            for (int i = 0; i < cboPhuongPhap.Items.Count; i++)
            {
                if (((PhuongPhapDinhVi)cboPhuongPhap.Items[i]).Ma == "SO_HOA_ANH")
                {
                    cboPhuongPhap.SelectedIndex = i;
                    break;
                }
            }
            if (cboPhuongPhap.SelectedIndex < 0 && cboPhuongPhap.Items.Count > 0)
                cboPhuongPhap.SelectedIndex = 0;
        }

        private HeToaDo HeDangChon { get { return (HeToaDo)cboHe.SelectedItem; } }

        private void DoiHeToaDo()
        {
            HeToaDo he = HeDangChon;
            lblO1.Text = he.Nhan1;
            lblO2.Text = he.Nhan2;
            KetQuaQuyDoi kq = ketQuaKiemTra;
            if (kq != null)
            {
                // Cung mot diem, chi doi cach the hien theo he moi.
                DatToaDoTheoHe(kq.KinhDo, kq.ViDo, kq.E32648, kq.N32648,
                    kq.XVn2000, kq.YVn2000);
                HienKetQuaQuyDoi(kq);
                CapNhatNutLuu();
            }
            else if (hienHanh != null && txtO1.Text.Length > 0)
            {
                DatToaDoTheoHe(hienHanh.KinhDo, hienHanh.ViDo, hienHanh.E32648,
                    hienHanh.N32648, hienHanh.XVn2000, hienHanh.YVn2000);
            }
        }

        private void DatToaDoTheoHe(double kinhDo, double viDo, double e32648,
            double n32648, double xVn, double yVn)
        {
            HeToaDo he = HeDangChon;
            KetQuaQuyDoi giuLai = ketQuaKiemTra;
            if (he.Srid == TruyXuatViTri.SridWgs84)
            {
                txtO1.Text = kinhDo.ToString("0.0000000", CultureInfo.InvariantCulture);
                txtO2.Text = viDo.ToString("0.0000000", CultureInfo.InvariantCulture);
            }
            else if (he.Srid == TruyXuatViTri.SridVn2000Tm3)
            {
                txtO1.Text = xVn.ToString("0.000", CultureInfo.InvariantCulture);
                txtO2.Text = yVn.ToString("0.000", CultureInfo.InvariantCulture);
            }
            else
            {
                txtO1.Text = e32648.ToString("0.000", CultureInfo.InvariantCulture);
                txtO2.Text = n32648.ToString("0.000", CultureInfo.InvariantCulture);
            }
            ketQuaKiemTra = giuLai;   // doi cach hien thi, khong doi diem
        }

        /// <summary>FormMain goi sau khi nguoi dung nhap 1 diem tren ban do (EPSG:32648).</summary>
        public void DatDiemTuBanDo(double e32648, double n32648)
        {
            YeuCauChonTrenBanDo = false;
            try
            {
                KetQuaQuyDoi kq = dal.QuyDoi(TruyXuatViTri.SridUtm48N, e32648, n32648);
                if (kq == null) return;
                ketQuaKiemTra = kq;
                DatToaDoTheoHe(kq.KinhDo, kq.ViDo, kq.E32648, kq.N32648, kq.XVn2000, kq.YVn2000);
                ketQuaKiemTra = kq;
                HienKetQuaQuyDoi(kq);
            }
            catch (Exception ex)
            {
                lblQuyDoi.ForeColor = Color.FromArgb(198, 40, 40);
                lblQuyDoi.Text = ex.Message;
            }
        }

        private void DoiPhuongPhap()
        {
            PhuongPhapDinhVi p = cboPhuongPhap.SelectedItem as PhuongPhapDinhVi;
            if (p == null) return;
            // Khong cho khai bao chinh xac hon kha nang cua phuong phap.
            numDoChinhXac.Minimum = Math.Max(0.05M, p.DoChinhXacDanhNghiaM);
            numDoChinhXac.Value = Math.Min(numDoChinhXac.Maximum, p.DoChinhXacDanhNghiaM);
        }

        // =====================================================
        // 3. KIEM TRA VA LUU
        // =====================================================

        private void DanhDauChuaKiemTra()
        {
            ketQuaKiemTra = null;
            lblQuyDoi.ForeColor = MauPhu;
            lblQuyDoi.Text = "Bấm \"Kiểm tra / quy đổi\" để xem điểm ở 3 hệ tọa độ và phường/xã.";
            CapNhatNutLuu();
        }

        private bool DocToaDo(out double e, out double n)
        {
            e = n = 0;
            double o1, o2;
            if (!DocSo(txtO1.Text, out o1) || !DocSo(txtO2.Text, out o2))
                return false;
            if (HeDangChon.O1LaBac) { n = o1; e = o2; }      // VN-2000: X = Bac
            else if (HeDangChon.Srid == TruyXuatViTri.SridWgs84) { e = o1; n = o2; }
            else { e = o1; n = o2; }
            return true;
        }

        private static bool DocSo(string s, out double v)
        {
            s = (s ?? "").Trim().Replace(" ", "").Replace(',', '.');
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
        }

        private bool KiemTra(bool baoLoi)
        {
            double e, n;
            if (!DocToaDo(out e, out n))
            {
                lblQuyDoi.ForeColor = Color.FromArgb(198, 40, 40);
                lblQuyDoi.Text = "Tọa độ chưa đúng định dạng số.";
                return false;
            }
            try
            {
                KetQuaQuyDoi kq = dal.QuyDoi(HeDangChon.Srid, e, n);
                ketQuaKiemTra = kq;
                HienKetQuaQuyDoi(kq);
                return kq != null && kq.TrongHaNoi && kq.TenXaPhuong != null;
            }
            catch (Exception ex)
            {
                ketQuaKiemTra = null;
                lblQuyDoi.ForeColor = Color.FromArgb(198, 40, 40);
                lblQuyDoi.Text = ex.Message;
                if (baoLoi) System.Media.SystemSounds.Beep.Play();
                return false;
            }
            finally { CapNhatNutLuu(); }
        }

        private void HienKetQuaQuyDoi(KetQuaQuyDoi kq)
        {
            if (kq == null) { lblQuyDoi.Text = "Không quy đổi được."; return; }
            double dich = Math.Sqrt(Math.Pow(kq.E32648 - hienHanh.E32648, 2) +
                                    Math.Pow(kq.N32648 - hienHanh.N32648, 2));
            CultureInfo c = CultureInfo.InvariantCulture;
            lblQuyDoi.ForeColor = kq.TenXaPhuong == null ? Color.FromArgb(198, 40, 40) : MauChu;
            lblQuyDoi.Text =
                "WGS84   : " + kq.KinhDo.ToString("0.000000", c) + "°E  " + kq.ViDo.ToString("0.000000", c) + "°N\n" +
                "VN-2000 : X " + kq.XVn2000.ToString("0.00", c) + "  Y " + kq.YVn2000.ToString("0.00", c) + "\n" +
                "UTM 48N : E " + kq.E32648.ToString("0.00", c) + "  N " + kq.N32648.ToString("0.00", c) + "\n" +
                "Địa bàn : " + (kq.TenXaPhuong ?? "NGOÀI các phường/xã Hà Nội") +
                "   ·   dịch " + dich.ToString("0", c) + " m so với hiện tại";
            CapNhatNutLuu();
        }

        private void CapNhatNutLuu()
        {
            btnLuu.Enabled = ketQuaKiemTra != null && ketQuaKiemTra.TenXaPhuong != null &&
                             txtNguon.Text.Trim().Length > 0;
            // Nut phang khong tu doi mau khi khoa -> doi nen cho de nhan biet.
            btnLuu.BackColor = btnLuu.Enabled ? MauChinh : Color.FromArgb(176, 190, 197);
            btnLuu.ForeColor = Color.White;
        }

        private void Luu()
        {
            if (!KiemTra(true)) return;
            PhuongPhapDinhVi p = cboPhuongPhap.SelectedItem as PhuongPhapDinhVi;
            if (p == null) return;
            if (txtNguon.Text.Trim().Length == 0)
            {
                MessageBox.Show(this, "Hãy ghi rõ nguồn vị trí.", "Thiếu nguồn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNguon.Focus();
                return;
            }
            double e, n;
            DocToaDo(out e, out n);
            try
            {
                Cursor = Cursors.WaitCursor;
                KetQua = dal.DinhViTruSo(MaDonVi, HeDangChon.Srid, e, n, p.Ma,
                    numDoChinhXac.Value, txtNguon.Text.Trim(),
                    txtNguoi.Text.Trim(), txtGhiChu.Text.Trim());
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không lưu được vị trí",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { Cursor = Cursors.Default; }
        }
    }
}
