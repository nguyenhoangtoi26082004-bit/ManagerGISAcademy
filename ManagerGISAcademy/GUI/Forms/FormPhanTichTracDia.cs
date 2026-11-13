using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    // =====================================================================
    // 1. LICH SU VI TRI (dong thoi gian)
    // =====================================================================

    public sealed class FormLichSuViTri : HopThoaiV2
    {
        private readonly List<DongLichSuViTri> ds;
        public event Action<List<DongLichSuViTri>> YeuCauHienTrenBanDo;

        public FormLichSuViTri(string maDonVi, string tenDonVi, List<DongLichSuViTri> lichSu)
            : base("Lịch sử vị trí trụ sở", tenDonVi + " · " + maDonVi, new Size(1060, 560))
        {
            ds = lichSu ?? new List<DongLichSuViTri>();
            DongLichSuViTri hh = ds.FirstOrDefault(x => x.LaHienHanh);
            int thuc = ds.Count(x => x.LaViTriThuc);
            string tom = ds.Count + " lần định vị · " + thuc + " vị trí thật · " +
                (hh == null ? "chưa có vị trí hiện hành" : "hiện hành: " + hh.TenPhuongPhap + " " + hh.SaiSo);
            Label lblTom = Nhan(tom, 9.6F, true, VeBaoCao.Teal);
            lblTom.Dock = DockStyle.Top; lblTom.Height = 32;

            NativeCard card = TienIchGiaoDienSang.Card();
            card.Dock = DockStyle.Fill; card.Padding = new Padding(10);
            DataGridView g = BangGiaoDien.Tao();
            BangGiaoDien.Cot(g, "#", "Stt", 4);
            BangGiaoDien.Cot(g, "Từ thời điểm", "ThoiGian", 13);
            BangGiaoDien.Cot(g, "Trạng thái", "TrangThai", 10);
            BangGiaoDien.Cot(g, "Phương pháp", "TenPhuongPhap", 22, false, true);
            BangGiaoDien.Cot(g, "Sai số", "SaiSo", 8, true);
            BangGiaoDien.Cot(g, "Vĩ độ, kinh độ", "ToaDo", 16);
            BangGiaoDien.Cot(g, "Cách vị trí hiện hành", "Cach", 11, true);
            BangGiaoDien.Cot(g, "Nguồn", "Nguon", 16);
            BangGiaoDien.VeNhanTron(g, 2, s => s == "Hiện hành" ? Tuple.Create(BangGiaoDien.XanhNhat, VeBaoCao.Teal) : Tuple.Create(Color.FromArgb(238, 241, 246), TienIchGiaoDienSang.Muted));
            g.DataSource = new BindingList<DongLichSuViTri>(ds);
            card.Controls.Add(g);
            Than.Controls.Add(card);
            Than.Controls.Add(lblTom);
            Shown += (s, e) => g.ClearSelection();

            ThemNut("⌖  Hiện đường dịch chuyển trên bản đồ", VeBaoCao.Teal, Color.White, 280, (s, e) =>
            {
                if (YeuCauHienTrenBanDo != null) YeuCauHienTrenBanDo(ds);
            });
            ThemNut("Xuất CSV", Color.White, TienIchGiaoDienSang.Ink, 100, (s, e) => XuatCsv(maDonVi));
            ThemNut("Đóng", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => Close());
        }

        private void XuatCsv(string ma)
        {
            using (SaveFileDialog d = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "lich_su_vi_tri_" + ma + ".csv" })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                StringBuilder b = new StringBuilder("stt;tu_thoi_diem;hien_hanh;phuong_phap;sai_so_m;kinh_do;vi_do;e_utm48;n_utm48;cach_hien_hanh_m;nguon;nguoi_xac_minh;ghi_chu\n");
                foreach (DongLichSuViTri x in ds)
                    b.AppendLine(string.Join(";", x.Stt, x.HieuLucTu.ToString("yyyy-MM-dd HH:mm:ss"), x.LaHienHanh, x.PhuongPhap,
                        x.DoChinhXacM.ToString(CultureInfo.InvariantCulture), x.KinhDo.ToString("0.0000000", CultureInfo.InvariantCulture),
                        x.ViDo.ToString("0.0000000", CultureInfo.InvariantCulture), x.E.ToString("0.000", CultureInfo.InvariantCulture),
                        x.N.ToString("0.000", CultureInfo.InvariantCulture), x.CachHienHanhM.ToString("0.0", CultureInfo.InvariantCulture),
                        "\"" + x.Nguon + "\"", "\"" + x.NguoiXacMinh + "\"", "\"" + x.GhiChu + "\""));
                File.WriteAllText(d.FileName, b.ToString(), new UTF8Encoding(true));
            }
        }
    }

    // =====================================================================
    // 2. MAT CAT DIA HINH + TAM NHIN THONG (LOS)
    // =====================================================================

    public sealed class FormMatCat : HopThoaiV2
    {
        private const double BanKinhTraiDat = 6371000d;
        private readonly List<DiemMatCat> diem;
        private readonly BieuDoMatCat bieuDo;
        private readonly NumericUpDown nudH1, nudH2, nudK;
        private readonly Label lblKetQua, lblThongSo;
        private readonly string ten1, ten2;

        public FormMatCat(List<DiemMatCat> matCat, string tenDiem1, string tenDiem2)
            : base("Mặt cắt địa hình & tầm nhìn thông", "DEM SRTM 1″ (~30 m) · có hiệu chỉnh độ cong Trái Đất và khúc xạ khí quyển", new Size(1080, 640))
        {
            diem = matCat ?? new List<DiemMatCat>();
            ten1 = tenDiem1; ten2 = tenDiem2;

            FlowLayoutPanel thamSo = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, BackColor = TienIchGiaoDienSang.Background, WrapContents = false, Padding = new Padding(0, 6, 0, 0) };
            nudH1 = So(10); nudH2 = So(2); nudK = new NumericUpDown { DecimalPlaces = 2, Minimum = 0m, Maximum = 1m, Increment = 0.01m, Value = 0.13m, Width = 70, Font = new Font("Segoe UI", 9.6F) };
            thamSo.Controls.Add(NhanCo("Cao anten điểm A (m)")); thamSo.Controls.Add(nudH1);
            thamSo.Controls.Add(NhanCo("Cao điểm B (m)")); thamSo.Controls.Add(nudH2);
            thamSo.Controls.Add(NhanCo("Hệ số khúc xạ k")); thamSo.Controls.Add(nudK);
            lblKetQua = Nhan("", 11F, true, VeBaoCao.Teal);
            lblKetQua.Dock = DockStyle.Top; lblKetQua.Height = 34;
            lblThongSo = Nhan("", 9F, false, TienIchGiaoDienSang.Muted);
            lblThongSo.Dock = DockStyle.Top; lblThongSo.Height = 24;
            NativeCard card = TienIchGiaoDienSang.Card();
            card.Dock = DockStyle.Fill; card.Padding = new Padding(8);
            bieuDo = new BieuDoMatCat { Dock = DockStyle.Fill, Ten1 = ten1, Ten2 = ten2 };
            card.Controls.Add(bieuDo);
            Than.Controls.Add(card);
            Than.Controls.Add(lblThongSo);
            Than.Controls.Add(lblKetQua);
            Than.Controls.Add(thamSo);

            ThemNut("Xuất CSV mặt cắt", Color.White, TienIchGiaoDienSang.Ink, 150, (s, e) => XuatCsv());
            ThemNut("Lưu ảnh PNG", Color.White, TienIchGiaoDienSang.Ink, 120, (s, e) => LuuAnh());
            ThemNut("Đóng", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => Close());
            EventHandler tinh = (s, e) => TinhLos();
            nudH1.ValueChanged += tinh; nudH2.ValueChanged += tinh; nudK.ValueChanged += tinh;
            TinhLos();
        }

        private static NumericUpDown So(decimal v)
        {
            return new NumericUpDown { DecimalPlaces = 1, Minimum = 0m, Maximum = 500m, Value = v, Width = 80, Font = new Font("Segoe UI", 9.6F), Margin = new Padding(0, 3, 22, 0) };
        }

        private static Label NhanCo(string s)
        {
            Label l = Nhan(s, 9F, true);
            l.AutoSize = true; l.Dock = DockStyle.None; l.Margin = new Padding(0, 8, 6, 0);
            return l;
        }

        private void TinhLos()
        {
            List<DiemMatCat> co = diem.Where(x => x.DoCaoM.HasValue).ToList();
            if (co.Count < 2)
            {
                lblKetQua.Text = "Không có dữ liệu độ cao trên tuyến (ngoài vùng DEM).";
                lblKetQua.ForeColor = BangGiaoDien.Do;
                return;
            }
            double L = diem.Last().KhoangCachM;
            double rHieuDung = BanKinhTraiDat / (1d - (double)nudK.Value);
            double zA = co.First().DoCaoM.Value + (double)nudH1.Value;
            double zB = co.Last().DoCaoM.Value + (double)nudH2.Value;
            double? chanTai = null, chanCao = null; double khoangHo = double.MaxValue;
            List<double> phinh = new List<double>();
            foreach (DiemMatCat p in diem)
            {
                double d = p.KhoangCachM;
                double b = d * (L - d) / (2d * rHieuDung);        // do phinh Trai Dat (m)
                phinh.Add(b);
                if (!p.DoCaoM.HasValue || d <= 0 || d >= L) continue;
                double tia = zA + (zB - zA) * d / L;
                double dat = p.DoCaoM.Value + b;
                double ho = tia - dat;
                if (ho < khoangHo) khoangHo = ho;
                if (ho < 0 && !chanTai.HasValue) { chanTai = d; chanCao = p.DoCaoM.Value; }
            }
            bieuDo.Dat(diem, phinh, zA, zB, chanTai);
            CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
            if (chanTai.HasValue)
            {
                lblKetQua.Text = "✕  KHÔNG nhìn thấy: tia bị địa hình che tại " + (chanTai.Value / 1000d).ToString("N2", vi) +
                                 " km (cao " + chanCao.Value.ToString("N0", vi) + " m)";
                lblKetQua.ForeColor = BangGiaoDien.Do;
            }
            else
            {
                lblKetQua.Text = "✓  NHÌN THẤY (thông tuyến) · khoảng hở nhỏ nhất " + khoangHo.ToString("N1", vi) + " m";
                lblKetQua.ForeColor = VeBaoCao.Teal;
            }
            lblThongSo.Text = "Chiều dài " + (L / 1000d).ToString("N2", vi) + " km · cao nhất " + co.Max(x => x.DoCaoM.Value).ToString("N0", vi) +
                " m · thấp nhất " + co.Min(x => x.DoCaoM.Value).ToString("N0", vi) + " m · độ phình Trái Đất giữa tuyến " +
                (phinh.Count > 0 ? phinh.Max().ToString("N2", vi) : "0") + " m · " + diem.Count + " điểm lấy mẫu";
        }

        private void XuatCsv()
        {
            using (SaveFileDialog d = new SaveFileDialog { Filter = "CSV (*.csv)|*.csv", FileName = "mat_cat_dia_hinh.csv" })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                StringBuilder b = new StringBuilder("stt;khoang_cach_m;do_cao_m;e_utm48;n_utm48\n");
                foreach (DiemMatCat p in diem)
                    b.AppendLine(p.Stt + ";" + p.KhoangCachM.ToString("0.0", CultureInfo.InvariantCulture) + ";" +
                        (p.DoCaoM.HasValue ? p.DoCaoM.Value.ToString("0.0", CultureInfo.InvariantCulture) : "") + ";" +
                        p.X.ToString("0.0", CultureInfo.InvariantCulture) + ";" + p.Y.ToString("0.0", CultureInfo.InvariantCulture));
                File.WriteAllText(d.FileName, b.ToString(), new UTF8Encoding(true));
            }
        }

        private void LuuAnh()
        {
            using (SaveFileDialog d = new SaveFileDialog { Filter = "PNG (*.png)|*.png", FileName = "mat_cat_dia_hinh.png" })
            {
                if (d.ShowDialog(this) != DialogResult.OK) return;
                using (Bitmap bmp = new Bitmap(Math.Max(800, bieuDo.Width), Math.Max(400, bieuDo.Height)))
                {
                    bieuDo.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                    bmp.Save(d.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
        }

        /// <summary>Bieu do mat cat: dia hinh (+ do phinh), tia nhin A-B, diem bi che.</summary>
        private sealed class BieuDoMatCat : Control
        {
            private List<DiemMatCat> d = new List<DiemMatCat>();
            private List<double> phinh = new List<double>();
            private double zA, zB;
            private double? chan;
            public string Ten1 { get; set; }
            public string Ten2 { get; set; }

            public BieuDoMatCat()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = Color.White;
            }

            public void Dat(List<DiemMatCat> ds, List<double> p, double a, double b, double? c)
            {
                d = ds; phinh = p; zA = a; zB = b; chan = c; Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                List<DiemMatCat> co = d.Where(x => x.DoCaoM.HasValue).ToList();
                if (co.Count < 2) return;
                RectangleF vung = new RectangleF(64, 24, Width - 90, Height - 70);
                double L = d.Last().KhoangCachM;
                double zMin = Math.Min(co.Min(x => x.DoCaoM.Value), Math.Min(zA, zB));
                double zMax = Math.Max(co.Max(x => x.DoCaoM.Value) + (phinh.Count > 0 ? phinh.Max() : 0), Math.Max(zA, zB));
                double dz = Math.Max(10, zMax - zMin);
                zMin -= dz * 0.08; zMax += dz * 0.12;
                Func<double, float> X = k => vung.Left + (float)(k / Math.Max(1, L) * vung.Width);
                Func<double, float> Y = z => vung.Bottom - (float)((z - zMin) / (zMax - zMin) * vung.Height);
                using (Font f = new Font("Segoe UI", 8.5F))
                using (Pen luoi = new Pen(Color.FromArgb(236, 242, 248)))
                using (SolidBrush chu = new SolidBrush(VeBaoCao.Sub))
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        double z = zMin + (zMax - zMin) * i / 5;
                        float y = Y(z);
                        g.DrawLine(luoi, vung.Left, y, vung.Right, y);
                        g.DrawString(z.ToString("N0") + " m", f, chu, 4, y - 8);
                        double k = L * i / 5;
                        float x = X(k);
                        g.DrawLine(luoi, x, vung.Top, x, vung.Bottom);
                        g.DrawString((k / 1000d).ToString("0.0") + " km", f, chu, x - 18, vung.Bottom + 6);
                    }
                }
                // Dia hinh + do phinh
                List<PointF> dat = new List<PointF>();
                for (int i = 0; i < d.Count; i++)
                    if (d[i].DoCaoM.HasValue)
                        dat.Add(new PointF(X(d[i].KhoangCachM), Y(d[i].DoCaoM.Value + (i < phinh.Count ? phinh[i] : 0))));
                if (dat.Count > 1)
                {
                    List<PointF> vungDat = new List<PointF>(dat) { new PointF(dat.Last().X, vung.Bottom), new PointF(dat.First().X, vung.Bottom) };
                    using (LinearGradientBrush lg = new LinearGradientBrush(vung, Color.FromArgb(150, 196, 161, 108), Color.FromArgb(150, 120, 170, 96), LinearGradientMode.Vertical))
                        g.FillPolygon(lg, vungDat.ToArray());
                    using (Pen p = new Pen(Color.FromArgb(120, 84, 40), 1.8f)) g.DrawLines(p, dat.ToArray());
                }
                // Tia nhin
                PointF a = new PointF(X(0), Y(zA)), b = new PointF(X(L), Y(zB));
                using (Pen tia = new Pen(chan.HasValue ? BangGiaoDien.Do : VeBaoCao.Teal, 2.2f) { DashStyle = DashStyle.Dash })
                    g.DrawLine(tia, a, b);
                using (SolidBrush ba = new SolidBrush(VeBaoCao.Blue))
                {
                    g.FillEllipse(ba, a.X - 5, a.Y - 5, 10, 10);
                    g.FillEllipse(ba, b.X - 5, b.Y - 5, 10, 10);
                }
                using (Font fb = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
                using (SolidBrush nenNhan = new SolidBrush(Color.FromArgb(235, 255, 255, 255)))
                {
                    SizeF ka = g.MeasureString("A · " + (Ten1 ?? ""), fb);
                    g.FillRectangle(nenNhan, a.X + 3, a.Y - 24, ka.Width + 6, ka.Height + 2);
                    g.DrawString("A · " + (Ten1 ?? ""), fb, ink, a.X + 6, a.Y - 22);
                    SizeF kt = g.MeasureString("B · " + (Ten2 ?? ""), fb);
                    g.FillRectangle(nenNhan, b.X - kt.Width - 9, b.Y - 24, kt.Width + 6, kt.Height + 2);
                    g.DrawString("B · " + (Ten2 ?? ""), fb, ink, b.X - kt.Width - 6, b.Y - 22);
                }
                if (chan.HasValue)
                {
                    float x = X(chan.Value);
                    using (Pen p = new Pen(BangGiaoDien.Do, 1.5f) { DashStyle = DashStyle.Dot }) g.DrawLine(p, x, vung.Top, x, vung.Bottom);
                    using (Font f = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                    using (SolidBrush b2 = new SolidBrush(BangGiaoDien.Do))
                        g.DrawString("bị che", f, b2, x + 4, vung.Top + 2);
                }
            }
        }
    }
}
