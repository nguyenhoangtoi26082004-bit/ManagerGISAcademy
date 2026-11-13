using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    // =====================================================================
    // 1. TIEN ICH IN: PDF (Microsoft Print to PDF) + xem truoc / chon may in
    // =====================================================================
    public static class TienIchIn
    {
        public const string MayPdf = "Microsoft Print to PDF";
        public static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

        public static bool CoMayPdf()
        {
            foreach (string p in PrinterSettings.InstalledPrinters)
                if (string.Equals(p, MayPdf, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        /// <summary>Dat kho giay theo ten (A4/A3) cho may in hien hanh; khong co thi tao kho tuy bien.</summary>
        public static void DatKhoGiay(PrintDocument doc, string kho, bool ngang)
        {
            PaperKind kind = kho == "A3" ? PaperKind.A3 : PaperKind.A4;
            PaperSize chon = null;
            try
            {
                foreach (PaperSize ps in doc.PrinterSettings.PaperSizes)
                    if (ps.Kind == kind) { chon = ps; break; }
            }
            catch { }
            if (chon == null)
                chon = kho == "A3" ? new PaperSize("A3", 1169, 1654) : new PaperSize("A4", 827, 1169);
            doc.DefaultPageSettings.PaperSize = chon;
            doc.DefaultPageSettings.Landscape = ngang;
            doc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        }

        /// <summary>Xuat PrintDocument ra tep PDF (hoi noi luu). Tra ve duong dan hoac null.</summary>
        public static string XuatPdf(IWin32Window chu, Func<PrintDocument> taoTaiLieu, string tenGoiY)
        {
            if (!CoMayPdf())
            {
                MessageBox.Show(chu, "Máy chưa bật máy in ảo \"Microsoft Print to PDF\".\n\nBật trong: Windows Features → Microsoft Print to PDF.",
                    "Xuất PDF", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            string tep;
            using (SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Tệp PDF (*.pdf)|*.pdf",
                FileName = tenGoiY,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                OverwritePrompt = true
            })
            {
                if (sfd.ShowDialog(chu) != DialogResult.OK) return null;
                tep = sfd.FileName;
            }
            Cursor cu = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (File.Exists(tep)) File.Delete(tep);
                using (PrintDocument doc = taoTaiLieu())
                {
                    string kho = doc.DefaultPageSettings.PaperSize.Kind == PaperKind.A3 ? "A3" : "A4";
                    bool ngang = doc.DefaultPageSettings.Landscape;
                    doc.PrinterSettings.PrinterName = MayPdf;
                    DatKhoGiay(doc, kho, ngang);
                    doc.PrinterSettings.PrintToFile = true;
                    doc.PrinterSettings.PrintFileName = tep;
                    doc.PrintController = new StandardPrintController();
                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = cu;
                MessageBox.Show(chu, "Không xuất được PDF: " + ex.Message, "Xuất PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally { Cursor.Current = cu; }

            if (File.Exists(tep) &&
                MessageBox.Show(chu, "Đã lưu:\n" + tep + "\n\nMở tệp PDF ngay?", "Xuất PDF",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                MoTep(chu, tep);
            return tep;
        }

        public static void MoTep(IWin32Window chu, string tep)
        {
            try { Process.Start(new ProcessStartInfo(tep) { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show(chu, ex.Message, "Không mở được", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        /// <summary>Xem truoc + nut "Chon may in va in...".</summary>
        public static void XemTruocVaIn(IWin32Window chu, PrintDocument doc, string tieuDe,
            Func<PrintDocument> taoPdf = null, string tenPdf = null)
        {
            using (PrintPreviewDialog xt = new PrintPreviewDialog
            {
                Document = doc,
                Text = tieuDe,
                ShowIcon = false,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            })
            {
                ToolStrip ts = xt.Controls.OfType<ToolStrip>().FirstOrDefault();
                if (ts != null)
                {
                    if (ts.Items.Count > 0) ts.Items[0].Visible = false;
                    ToolStripButton nutIn = new ToolStripButton("⎙  Chọn máy in và in…")
                    {
                        DisplayStyle = ToolStripItemDisplayStyle.Text,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                    };
                    nutIn.Click += (s, e) =>
                    {
                        using (PrintDialog pd = new PrintDialog { Document = doc, UseEXDialog = true, AllowSomePages = false })
                        {
                            if (pd.ShowDialog(xt) != DialogResult.OK) return;
                            try { doc.Print(); xt.Close(); }
                            catch (Exception ex) { MessageBox.Show(xt, ex.Message, "Lỗi in", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                        }
                    };
                    ts.Items.Insert(0, nutIn);
                    if (taoPdf != null)
                    {
                        ToolStripButton nutPdf = new ToolStripButton("⇩  Xuất PDF…")
                        {
                            DisplayStyle = ToolStripItemDisplayStyle.Text,
                            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                        };
                        nutPdf.Click += (s, e) => XuatPdf(xt, taoPdf, tenPdf ?? "TaiLieu.pdf");
                        ts.Items.Insert(1, nutPdf);
                    }
                }
                xt.ShowDialog(chu);
            }
        }

        // ---------- ve chung ----------
        public static GraphicsPath BoTron(RectangleF r, float k)
        {
            GraphicsPath p = new GraphicsPath();
            float d = Math.Min(k * 2, Math.Min(r.Width, r.Height));
            if (d <= 0) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static string Dms(double v, bool laVi)
        {
            string h = laVi ? (v >= 0 ? "N" : "S") : (v >= 0 ? "E" : "W");
            v = Math.Abs(v);
            int d = (int)v;
            double mf = (v - d) * 60;
            int m = (int)mf;
            double s = (mf - m) * 60;
            if (s >= 59.995) { s = 0; m++; }
            if (m >= 60) { m = 0; d++; }
            return d + "°" + m.ToString("00") + "′" + s.ToString("00.00", CultureInfo.InvariantCulture) + "″ " + h;
        }

        /// <summary>Do dai thuoc ty le "dep" (1-2-5 x 10^n) khong vuot qua max (m).</summary>
        public static double DoDaiDep(double max)
        {
            if (max <= 0) return 0;
            double p = Math.Pow(10, Math.Floor(Math.Log10(max)));
            foreach (double k in new[] { 5d, 2d, 1d }) if (k * p <= max) return k * p;
            return p;
        }

        public static string NhanKhoangCach(double m)
        {
            return m >= 1000 ? (m / 1000d).ToString("0.##", Vi) + " km" : m.ToString("0", Vi) + " m";
        }
    }

    // =====================================================================
    // 2. IN BAN DO A4 / A3
    // =====================================================================
    public sealed class MucChuGiai
    {
        public string Ten;
        public Color Mau;
        public int Kieu; // 0 diem tron, 1 vung, 2 duong, 3 duong net dut, 4 bieu tuong don vi, 5 luoi
        public MucChuGiai(string ten, Color mau, int kieu) { Ten = ten; Mau = mau; Kieu = kieu; }
    }

    public sealed class ThongSoInBanDo
    {
        public string KhoGiay = "A4";
        public bool Ngang = true;
        public string TieuDe = "BẢN ĐỒ PHÂN BỐ ĐƠN VỊ QUÂN ĐỘI TRÊN ĐỊA BÀN HÀ NỘI";
        public string PhuDe = "";
        public string NguoiLap = "";
        public int TyLe;                 // 0 = theo khung nhin hien tai
        public bool HienChuGiai = true, HienLuoi = true, HienThuocTyLe = true, HienMuiTenBac = true;
        public List<MucChuGiai> ChuGiai = new List<MucChuGiai>();
    }

    /// <summary>Bo cuc trang ban do (don vi 1/100 inch).</summary>
    public static class VeTrangBanDo
    {
        public const float Le = 38f;

        public static SizeF KichThuocTrang(ThongSoInBanDo t)
        {
            SizeF s = t.KhoGiay == "A3" ? new SizeF(1169, 1654) : new SizeF(827, 1169);
            return t.Ngang ? new SizeF(s.Height, s.Width) : s;
        }

        /// <summary>Khung ban do tren trang.</summary>
        public static RectangleF KhungBanDo(ThongSoInBanDo t)
        {
            SizeF s = KichThuocTrang(t);
            float k = t.KhoGiay == "A3" ? 1.25f : 1f;
            float dau = 78 * k, chan = 30 * k;
            float ben = t.HienChuGiai ? (t.Ngang ? 215 * k : 0) : 0;
            float duoi = t.HienChuGiai && !t.Ngang ? 200 * k : 0;
            float leLuoi = t.HienLuoi ? 20 * k : 0;
            return new RectangleF(Le + leLuoi, Le + dau + leLuoi,
                s.Width - 2 * Le - ben - 2 * leLuoi - (ben > 0 ? 12 * k : 0),
                s.Height - 2 * Le - dau - chan - duoi - 2 * leLuoi - (duoi > 0 ? 12 * k : 0));
        }

        /// <summary>Ty le thuc cua khung voi pham vi mat dat (m).</summary>
        public static double TinhTyLe(RectangleF khung, double rongDat)
        {
            double rongGiayM = khung.Width / 100d * 0.0254;
            return rongGiayM <= 0 ? 0 : rongDat / rongGiayM;
        }

        /// <summary>Pham vi mat dat (xmin,ymin,xmax,ymax) cho khung theo thong so va khung nhin hien tai.</summary>
        public static double[] PhamVi(ThongSoInBanDo t, double[] nhin)
        {
            RectangleF k = KhungBanDo(t);
            double cx = (nhin[0] + nhin[2]) / 2, cy = (nhin[1] + nhin[3]) / 2;
            double w, h, tl = k.Height / k.Width;
            if (t.TyLe > 0)
            {
                w = k.Width / 100d * 0.0254 * t.TyLe;
                h = w * tl;
            }
            else
            {
                w = nhin[2] - nhin[0]; h = nhin[3] - nhin[1];
                if (h / w > tl) w = h / tl; else h = w * tl;
            }
            return new[] { cx - w / 2, cy - h / 2, cx + w / 2, cy + h / 2 };
        }

        public static void Ve(Graphics g, ThongSoInBanDo t, Bitmap anh, double[] pv)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            SizeF s = KichThuocTrang(t);
            float k = t.KhoGiay == "A3" ? 1.25f : 1f;
            RectangleF khung = KhungBanDo(t);
            Color mucDam = Color.FromArgb(22, 49, 74), xam = Color.FromArgb(96, 116, 138), teal = Color.FromArgb(0, 140, 112);

            using (SolidBrush trang = new SolidBrush(Color.White)) g.FillRectangle(trang, 0, 0, s.Width, s.Height);

            // Vien ngoai doi (net dam + net manh).
            using (Pen p1 = new Pen(mucDam, 2.2f * k)) g.DrawRectangle(p1, Le - 8, Le - 8, s.Width - 2 * Le + 16, s.Height - 2 * Le + 16);
            using (Pen p2 = new Pen(mucDam, 0.6f)) g.DrawRectangle(p2, Le - 3, Le - 3, s.Width - 2 * Le + 6, s.Height - 2 * Le + 6);

            // Tieu de.
            using (Font f1 = new Font("Segoe UI", 15f * k, FontStyle.Bold))
            using (Font f2 = new Font("Segoe UI", 8.5f * k))
            using (SolidBrush b1 = new SolidBrush(mucDam))
            using (SolidBrush b2 = new SolidBrush(xam))
            using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                g.DrawString((t.TieuDe ?? "").ToUpper(TienIchIn.Vi), f1, b1, new RectangleF(Le, Le + 8 * k, s.Width - 2 * Le, 34 * k), giua);
                string phu = string.IsNullOrWhiteSpace(t.PhuDe)
                    ? "Hệ tọa độ WGS 84 / UTM múi 48N (EPSG:32648)  ·  Tỷ lệ 1:" + TinhTyLe(khung, pv[2] - pv[0]).ToString("N0", TienIchIn.Vi)
                    : t.PhuDe;
                g.DrawString(phu, f2, b2, new RectangleF(Le, Le + 44 * k, s.Width - 2 * Le, 20 * k), giua);
            }
            using (SolidBrush vang = new SolidBrush(teal)) g.FillRectangle(vang, s.Width / 2 - 60 * k, Le + 66 * k, 120 * k, 2.5f * k);

            // Anh ban do.
            if (anh != null) g.DrawImage(anh, khung);
            else using (SolidBrush nen = new SolidBrush(Color.FromArgb(238, 243, 247))) g.FillRectangle(nen, khung);

            if (t.HienLuoi) VeLuoi(g, khung, pv, k);
            using (Pen vien = new Pen(mucDam, 1.4f * k)) g.DrawRectangle(vien, khung.X, khung.Y, khung.Width, khung.Height);

            double tyLe = TinhTyLe(khung, pv[2] - pv[0]);
            if (t.HienChuGiai)
            {
                RectangleF ben = t.Ngang
                    ? new RectangleF(khung.Right + (t.HienLuoi ? 20 * k : 0) + 12 * k, khung.Y - (t.HienLuoi ? 20 * k : 0), s.Width - Le - (khung.Right + (t.HienLuoi ? 20 * k : 0) + 12 * k), khung.Height + (t.HienLuoi ? 40 * k : 0))
                    : new RectangleF(Le, khung.Bottom + (t.HienLuoi ? 20 * k : 0) + 12 * k, s.Width - 2 * Le, 200 * k);
                VeBenChuGiai(g, t, ben, tyLe, k);
            }
            else
            {
                if (t.HienMuiTenBac) VeMuiTenBac(g, new RectangleF(khung.Right - 52 * k, khung.Y + 10 * k, 42 * k, 62 * k), k, true);
                if (t.HienThuocTyLe) VeThuocTyLe(g, new RectangleF(khung.X + 12 * k, khung.Bottom - 40 * k, 190 * k, 30 * k), tyLe, k, true);
            }

            // Chan trang.
            using (Font fc = new Font("Segoe UI", 7f * k))
            using (SolidBrush bc = new SolidBrush(xam))
            {
                string trai = "Nguồn: CSDL HQTCSDL (PostgreSQL/PostGIS) · nền ảnh vệ tinh/OSM · ManagerGISAcademy";
                string phai = "Lập ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + (string.IsNullOrWhiteSpace(t.NguoiLap) ? "" : " · " + t.NguoiLap);
                g.DrawString(trai, fc, bc, Le + 2, s.Height - Le - 18 * k);
                SizeF sz = g.MeasureString(phai, fc);
                g.DrawString(phai, fc, bc, s.Width - Le - sz.Width - 2, s.Height - Le - 18 * k);
            }
        }

        private static void VeLuoi(Graphics g, RectangleF khung, double[] pv, float k)
        {
            double w = pv[2] - pv[0];
            double b = new[] { 100d, 200d, 500d, 1000d, 2000d, 5000d, 10000d, 20000d }.FirstOrDefault(x => w / x <= 9);
            if (b <= 0) b = 50000;
            Func<double, float> px = x => (float)(khung.X + (x - pv[0]) / w * khung.Width);
            Func<double, float> py = y => (float)(khung.Bottom - (y - pv[1]) / (pv[3] - pv[1]) * khung.Height);
            string dv = b >= 1000 ? "0" : "0.0";
            using (Pen p = new Pen(Color.FromArgb(150, 255, 255, 255), 0.8f * k) { DashStyle = DashStyle.Dash })
            using (Pen p2 = new Pen(Color.FromArgb(110, 22, 49, 74), 0.5f * k) { DashStyle = DashStyle.Dash })
            using (Pen vach = new Pen(Color.FromArgb(22, 49, 74), 0.9f * k))
            using (Font f = new Font("Segoe UI", 6.5f * k))
            using (SolidBrush br = new SolidBrush(Color.FromArgb(22, 49, 74)))
            using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (double x = Math.Ceiling(pv[0] / b) * b; x <= pv[2]; x += b)
                {
                    float X = px(x);
                    g.DrawLine(p, X, khung.Y, X, khung.Bottom);
                    g.DrawLine(p2, X, khung.Y, X, khung.Bottom);
                    g.DrawLine(vach, X, khung.Y - 6 * k, X, khung.Y);
                    g.DrawLine(vach, X, khung.Bottom, X, khung.Bottom + 6 * k);
                    string s = (x / 1000d).ToString(dv, CultureInfo.InvariantCulture);
                    g.DrawString(s, f, br, new RectangleF(X - 40 * k, khung.Y - 20 * k, 80 * k, 14 * k), giua);
                    g.DrawString(s, f, br, new RectangleF(X - 40 * k, khung.Bottom + 6 * k, 80 * k, 14 * k), giua);
                }
                for (double y = Math.Ceiling(pv[1] / b) * b; y <= pv[3]; y += b)
                {
                    float Y = py(y);
                    g.DrawLine(p, khung.X, Y, khung.Right, Y);
                    g.DrawLine(p2, khung.X, Y, khung.Right, Y);
                    g.DrawLine(vach, khung.X - 6 * k, Y, khung.X, Y);
                    g.DrawLine(vach, khung.Right, Y, khung.Right + 6 * k, Y);
                    string s = (y / 1000d).ToString(dv, CultureInfo.InvariantCulture);
                    GraphicsState st = g.Save();
                    g.TranslateTransform(khung.X - 12 * k, Y);
                    g.RotateTransform(-90);
                    g.DrawString(s, f, br, new RectangleF(-40 * k, -7 * k, 80 * k, 14 * k), giua);
                    g.Restore(st);
                    st = g.Save();
                    g.TranslateTransform(khung.Right + 12 * k, Y);
                    g.RotateTransform(-90);
                    g.DrawString(s, f, br, new RectangleF(-40 * k, -7 * k, 80 * k, 14 * k), giua);
                    g.Restore(st);
                }
            }
        }

        public static void VeMuiTenBac(Graphics g, RectangleF r, float k, bool coNen)
        {
            float cx = r.X + r.Width / 2, top = r.Y + 14 * k, bot = r.Bottom - 4 * k, w = Math.Min(r.Width, 30 * k) / 2;
            if (coNen)
                using (GraphicsPath bg = TienIchIn.BoTron(r, 6 * k))
                using (SolidBrush nen = new SolidBrush(Color.FromArgb(225, 255, 255, 255))) g.FillPath(nen, bg);
            PointF[] trai = { new PointF(cx, top), new PointF(cx - w, bot), new PointF(cx, bot - (bot - top) * 0.28f) };
            PointF[] phai = { new PointF(cx, top), new PointF(cx + w, bot), new PointF(cx, bot - (bot - top) * 0.28f) };
            using (SolidBrush den = new SolidBrush(Color.FromArgb(22, 49, 74))) g.FillPolygon(den, trai);
            using (SolidBrush trang = new SolidBrush(Color.White)) g.FillPolygon(trang, phai);
            using (Pen p = new Pen(Color.FromArgb(22, 49, 74), 0.9f * k)) { g.DrawPolygon(p, trai); g.DrawPolygon(p, phai); }
            using (Font f = new Font("Segoe UI", 9f * k, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(22, 49, 74)))
            using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center })
                g.DrawString("B", f, b, new RectangleF(r.X, r.Y - 1 * k, r.Width, 16 * k), giua);
        }

        public static void VeThuocTyLe(Graphics g, RectangleF r, double tyLe, float k, bool coNen)
        {
            if (tyLe <= 0) return;
            if (coNen)
                using (GraphicsPath bg = TienIchIn.BoTron(r, 5 * k))
                using (SolidBrush nen = new SolidBrush(Color.FromArgb(225, 255, 255, 255))) g.FillPath(nen, bg);
            float pad = 10 * k;
            double mPerUnit = tyLe * 0.0254 / 100d;             // m mat dat / 1 don vi (1/100 inch)
            double L = TienIchIn.DoDaiDep((r.Width - 2 * pad) * mPerUnit);
            float rong = (float)(L / mPerUnit);
            float x0 = r.X + pad, y0 = r.Y + r.Height * 0.52f, h = 5 * k;
            for (int i = 0; i < 4; i++)
                using (SolidBrush b = new SolidBrush(i % 2 == 0 ? Color.FromArgb(22, 49, 74) : Color.White))
                    g.FillRectangle(b, x0 + rong * i / 4, y0, rong / 4, h);
            using (Pen p = new Pen(Color.FromArgb(22, 49, 74), 0.8f * k)) g.DrawRectangle(p, x0, y0, rong, h);
            using (Font f = new Font("Segoe UI", 6.5f * k))
            using (SolidBrush b = new SolidBrush(Color.FromArgb(22, 49, 74)))
            using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center })
            {
                g.DrawString("0", f, b, new RectangleF(x0 - 20 * k, y0 - 13 * k, 40 * k, 12 * k), giua);
                g.DrawString(TienIchIn.NhanKhoangCach(L / 2), f, b, new RectangleF(x0 + rong / 2 - 40 * k, y0 - 13 * k, 80 * k, 12 * k), giua);
                g.DrawString(TienIchIn.NhanKhoangCach(L), f, b, new RectangleF(x0 + rong - 40 * k, y0 - 13 * k, 80 * k, 12 * k), giua);
                g.DrawString("1:" + tyLe.ToString("N0", TienIchIn.Vi), f, b, new RectangleF(x0, y0 + h + 2 * k, rong, 12 * k), giua);
            }
        }

        private static void VeBenChuGiai(Graphics g, ThongSoInBanDo t, RectangleF r, double tyLe, float k)
        {
            Color mucDam = Color.FromArgb(22, 49, 74), xam = Color.FromArgb(96, 116, 138);
            using (GraphicsPath p = TienIchIn.BoTron(r, 6 * k))
            {
                using (SolidBrush nen = new SolidBrush(Color.FromArgb(248, 251, 253))) g.FillPath(nen, p);
                using (Pen vien = new Pen(Color.FromArgb(205, 218, 230), 0.8f * k)) g.DrawPath(vien, p);
            }
            using (Font fTieu = new Font("Segoe UI", 8.5f * k, FontStyle.Bold))
            using (Font fMuc = new Font("Segoe UI", 7.5f * k))
            using (Font fNho = new Font("Segoe UI", 6.8f * k))
            using (SolidBrush bDam = new SolidBrush(mucDam))
            using (SolidBrush bXam = new SolidBrush(xam))
            {
                float pad = 12 * k;
                if (t.Ngang)
                {
                    float y = r.Y + pad;
                    if (t.HienMuiTenBac) { VeMuiTenBac(g, new RectangleF(r.X + r.Width / 2 - 22 * k, y, 44 * k, 64 * k), k, false); y += 74 * k; }
                    if (t.HienThuocTyLe) { VeThuocTyLe(g, new RectangleF(r.X + 4 * k, y, r.Width - 8 * k, 40 * k), tyLe, k, false); y += 48 * k; }
                    g.DrawString("CHÚ GIẢI", fTieu, bDam, r.X + pad, y); y += 20 * k;
                    foreach (MucChuGiai m in t.ChuGiai)
                    {
                        if (y > r.Bottom - 110 * k) break;
                        VeKyHieu(g, m, new RectangleF(r.X + pad, y + 1 * k, 22 * k, 12 * k), k);
                        g.DrawString(m.Ten, fMuc, bDam, new RectangleF(r.X + pad + 28 * k, y, r.Width - 2 * pad - 28 * k, 28 * k));
                        y += (g.MeasureString(m.Ten, fMuc, (int)(r.Width - 2 * pad - 28 * k)).Height > 16 * k ? 28 : 18) * k;
                    }
                    float yd = r.Bottom - 100 * k;
                    using (Pen sep = new Pen(Color.FromArgb(205, 218, 230), 0.8f * k)) g.DrawLine(sep, r.X + pad, yd - 6 * k, r.Right - pad, yd - 6 * k);
                    g.DrawString("THÔNG SỐ", fTieu, bDam, r.X + pad, yd); yd += 18 * k;
                    string ts = "Ellipsoid WGS 84\nPhép chiếu UTM, múi 48N\nKinh tuyến trục 105°E, k₀ = 0,9996\nLưới km: tọa độ E/N (km)\nKhổ " + t.KhoGiay + (t.Ngang ? " ngang" : " dọc");
                    g.DrawString(ts, fNho, bXam, new RectangleF(r.X + pad, yd, r.Width - 2 * pad, r.Bottom - yd - 4 * k));
                }
                else
                {
                    float x = r.X + pad, y = r.Y + pad;
                    if (t.HienMuiTenBac) VeMuiTenBac(g, new RectangleF(r.Right - 60 * k, y, 44 * k, 64 * k), k, false);
                    if (t.HienThuocTyLe) VeThuocTyLe(g, new RectangleF(r.Right - 260 * k, r.Bottom - 50 * k, 250 * k, 40 * k), tyLe, k, false);
                    g.DrawString("CHÚ GIẢI", fTieu, bDam, x, y); y += 20 * k;
                    float cot = (r.Width - 2 * pad - 70 * k) / 2;
                    int i = 0;
                    foreach (MucChuGiai m in t.ChuGiai)
                    {
                        float cx = x + (i % 2) * cot, cy = y + (i / 2) * 20 * k;
                        if (cy > r.Bottom - 30 * k) break;
                        VeKyHieu(g, m, new RectangleF(cx, cy + 1 * k, 22 * k, 12 * k), k);
                        g.DrawString(m.Ten, fMuc, bDam, new RectangleF(cx + 28 * k, cy, cot - 34 * k, 16 * k));
                        i++;
                    }
                    g.DrawString("WGS 84 / UTM múi 48N · Lưới km E/N · Khổ " + t.KhoGiay + " dọc", fNho, bXam, x, r.Bottom - 22 * k);
                }
            }
        }

        private static void VeKyHieu(Graphics g, MucChuGiai m, RectangleF r, float k)
        {
            switch (m.Kieu)
            {
                case 1:
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(90, m.Mau))) g.FillRectangle(b, r);
                    using (Pen p = new Pen(m.Mau, 1.2f * k)) g.DrawRectangle(p, r.X, r.Y, r.Width, r.Height);
                    break;
                case 2:
                case 3:
                case 5:
                    using (Pen p = new Pen(m.Mau, (m.Kieu == 2 ? 3f : 1.4f) * k))
                    {
                        if (m.Kieu != 2) p.DashStyle = m.Kieu == 5 ? DashStyle.Dot : DashStyle.Dash;
                        g.DrawLine(p, r.X, r.Y + r.Height / 2, r.Right, r.Y + r.Height / 2);
                    }
                    break;
                case 4:
                    {
                        float d = Math.Min(r.Height + 2 * k, r.Width);
                        RectangleF c = new RectangleF(r.X + (r.Width - d) / 2, r.Y + (r.Height - d) / 2, d, d);
                        using (SolidBrush b = new SolidBrush(m.Mau)) g.FillEllipse(b, c);
                        using (Pen p = new Pen(Color.FromArgb(255, 205, 60), 1.2f * k)) g.DrawEllipse(p, c);
                        using (SolidBrush sao = new SolidBrush(Color.FromArgb(255, 220, 70))) g.FillPolygon(sao, Sao(c.X + c.Width / 2, c.Y + c.Height / 2, d * 0.32f));
                        break;
                    }
                default:
                    {
                        float d = Math.Min(r.Height, r.Width);
                        RectangleF c = new RectangleF(r.X + (r.Width - d) / 2, r.Y, d, d);
                        using (SolidBrush b = new SolidBrush(m.Mau)) g.FillEllipse(b, c);
                        using (Pen p = new Pen(Color.White, 1.2f * k)) g.DrawEllipse(p, c);
                        break;
                    }
            }
        }

        private static PointF[] Sao(float cx, float cy, float R)
        {
            PointF[] p = new PointF[10];
            for (int i = 0; i < 10; i++)
            {
                double a = -Math.PI / 2 + i * Math.PI / 5;
                float r = i % 2 == 0 ? R : R * 0.42f;
                p[i] = new PointF(cx + (float)(r * Math.Cos(a)), cy + (float)(r * Math.Sin(a)));
            }
            return p;
        }
    }

    public sealed class BanDoIn : PrintDocument
    {
        private readonly ThongSoInBanDo ts;
        private readonly Func<double[], int, Bitmap> chup;
        private readonly double[] pv;

        public BanDoIn(ThongSoInBanDo thongSo, double[] phamVi, Func<double[], int, Bitmap> chupAnh)
        {
            ts = thongSo; pv = phamVi; chup = chupAnh;
            DocumentName = "Bản đồ " + thongSo.KhoGiay;
            TienIchIn.DatKhoGiay(this, ts.KhoGiay, ts.Ngang);
            OriginAtMargins = false;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            RectangleF khung = VeTrangBanDo.KhungBanDo(ts);
            int px = (int)Math.Min(4200, Math.Max(800, khung.Width / 100f * 200f)); // ~200 dpi
            Bitmap anh = null;
            try
            {
                anh = chup(pv, px);
                // May in co le cung (hard margin): dich goc ve mep giay.
                e.Graphics.TranslateTransform(-e.PageSettings.HardMarginX, -e.PageSettings.HardMarginY);
                VeTrangBanDo.Ve(e.Graphics, ts, anh, pv);
            }
            finally { if (anh != null) anh.Dispose(); }
            e.HasMorePages = false;
        }
    }

    public sealed class FormInBanDo : HopThoaiV2
    {
        private readonly ThongSoInBanDo ts = new ThongSoInBanDo();
        private readonly double[] nhin;
        private readonly Func<double[], int, Bitmap> chup;
        private readonly PictureBox xemTruoc;
        private readonly Label lblTyLe;
        private readonly Timer hen = new Timer { Interval = 350 };
        private Bitmap anhXemTruoc;
        private readonly TextBox txtTieuDe, txtPhuDe;
        private readonly ComboBox cboKho, cboHuong, cboTyLe;
        private readonly CheckBox chkChuGiai, chkLuoi, chkThuoc, chkBac;
        private static readonly int[] TyLeChuan = { 0, 5000, 10000, 25000, 50000, 100000, 200000 };

        public FormInBanDo(double[] khungNhin, Func<double[], int, Bitmap> chupAnh, List<MucChuGiai> chuGiai, string nguoiLap)
            : base("In bản đồ", "Chọn khổ giấy, tỷ lệ và thành phần trình bày · xem trước trực tiếp · xuất PDF hoặc in", new Size(1120, 720))
        {
            nhin = khungNhin; chup = chupAnh;
            ts.ChuGiai = chuGiai ?? new List<MucChuGiai>();
            ts.NguoiLap = nguoiLap;

            Panel trai = new Panel { Dock = DockStyle.Left, Width = 300, BackColor = Color.White, Padding = new Padding(16, 14, 16, 10) };
            trai.Paint += (s, e) => { using (Pen p = new Pen(TienIchGiaoDienSang.Border)) e.Graphics.DrawLine(p, trai.Width - 1, 0, trai.Width - 1, trai.Height); };
            Panel phai = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(226, 232, 240), Padding = new Padding(18) };
            xemTruoc = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Transparent };
            phai.Controls.Add(xemTruoc);
            Than.Padding = new Padding(0);
            Than.Controls.Add(phai);
            Than.Controls.Add(trai);

            int y = 4;
            Func<string, Label> nhom = t =>
            {
                Label l = NhanTd(t, 8F, true, VeBaoCao.Teal);
                l.SetBounds(16, y, 260, 18); trai.Controls.Add(l); y += 22; return l;
            };
            Func<string, Control, Control> dong = (t, c) =>
            {
                Label l = NhanTd(t, 9F, false, TienIchGiaoDienSang.Muted);
                l.SetBounds(16, y + 4, 92, 20); trai.Controls.Add(l);
                c.SetBounds(110, y, 172, 26); trai.Controls.Add(c); y += 34; return c;
            };
            nhom("TRANG GIẤY");
            cboKho = (ComboBox)dong("Khổ giấy", new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) });
            cboKho.Items.AddRange(new object[] { "A4", "A3" }); cboKho.SelectedIndex = 0;
            cboHuong = (ComboBox)dong("Hướng", new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) });
            cboHuong.Items.AddRange(new object[] { "Ngang", "Dọc" }); cboHuong.SelectedIndex = 0;
            cboTyLe = (ComboBox)dong("Tỷ lệ", new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9.5F) });
            cboTyLe.Items.Add("Theo khung nhìn hiện tại");
            foreach (int t in TyLeChuan.Skip(1)) cboTyLe.Items.Add("1 : " + t.ToString("N0", TienIchIn.Vi));
            cboTyLe.SelectedIndex = 0;
            lblTyLe = NhanTd("", 8.5F, false, TienIchGiaoDienSang.Muted);
            lblTyLe.SetBounds(16, y - 4, 266, 34); trai.Controls.Add(lblTyLe); y += 34;

            nhom("NỘI DUNG");
            Label lt = NhanTd("Tiêu đề", 9F, false, TienIchGiaoDienSang.Muted); lt.SetBounds(16, y, 260, 18); trai.Controls.Add(lt); y += 20;
            txtTieuDe = new TextBox { Text = ts.TieuDe, Font = new Font("Segoe UI", 9.5F) }; txtTieuDe.SetBounds(16, y, 266, 26); trai.Controls.Add(txtTieuDe); y += 32;
            Label lp = NhanTd("Phụ đề (để trống = hệ tọa độ + tỷ lệ)", 9F, false, TienIchGiaoDienSang.Muted); lp.SetBounds(16, y, 266, 18); trai.Controls.Add(lp); y += 20;
            txtPhuDe = new TextBox { Font = new Font("Segoe UI", 9.5F) }; txtPhuDe.SetBounds(16, y, 266, 26); trai.Controls.Add(txtPhuDe); y += 38;

            nhom("THÀNH PHẦN BẢN ĐỒ");
            Func<string, CheckBox> chk = t =>
            {
                CheckBox c = new CheckBox { Text = t, Checked = true, Font = new Font("Segoe UI", 9.5F), ForeColor = TienIchGiaoDienSang.Ink };
                c.SetBounds(16, y, 266, 24); trai.Controls.Add(c); y += 26; return c;
            };
            chkChuGiai = chk("Khung chú giải + thông số");
            chkLuoi = chk("Lưới tọa độ km + ghi chú mép khung");
            chkThuoc = chk("Thước tỷ lệ");
            chkBac = chk("Mũi tên chỉ hướng Bắc");

            ThemNut("⎙  In…", TienIchGiaoDienSang.Blue, Color.White, 110, (s, e) => InRa());
            ThemNut("⇩  Xuất PDF", VeBaoCao.Teal, Color.White, 130, (s, e) => XuatPdf());
            ThemNut("Đóng", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => Close());

            EventHandler doi = (s, e) => { hen.Stop(); hen.Start(); };
            foreach (Control c in new Control[] { cboKho, cboHuong, cboTyLe }) ((ComboBox)c).SelectedIndexChanged += doi;
            foreach (CheckBox c in new[] { chkChuGiai, chkLuoi, chkThuoc, chkBac }) c.CheckedChanged += doi;
            txtTieuDe.TextChanged += doi; txtPhuDe.TextChanged += doi;
            phai.Resize += doi;
            hen.Tick += (s, e) => { hen.Stop(); VeXemTruoc(); };
            Shown += (s, e) => VeXemTruoc();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { hen.Dispose(); if (anhXemTruoc != null) anhXemTruoc.Dispose(); }
            base.Dispose(disposing);
        }

        private void DocThongSo()
        {
            ts.KhoGiay = (string)cboKho.SelectedItem;
            ts.Ngang = cboHuong.SelectedIndex == 0;
            ts.TyLe = TyLeChuan[Math.Max(0, cboTyLe.SelectedIndex)];
            ts.TieuDe = txtTieuDe.Text.Trim();
            ts.PhuDe = txtPhuDe.Text.Trim();
            ts.HienChuGiai = chkChuGiai.Checked;
            ts.HienLuoi = chkLuoi.Checked;
            ts.HienThuocTyLe = chkThuoc.Checked;
            ts.HienMuiTenBac = chkBac.Checked;
        }

        private void VeXemTruoc()
        {
            try
            {
                DocThongSo();
                SizeF trang = VeTrangBanDo.KichThuocTrang(ts);
                Size vung = xemTruoc.ClientSize;
                if (vung.Width < 50 || vung.Height < 50) return;
                float k = Math.Min(vung.Width / trang.Width, vung.Height / trang.Height);
                k = Math.Max(0.2f, k);
                double[] pv = VeTrangBanDo.PhamVi(ts, nhin);
                RectangleF khung = VeTrangBanDo.KhungBanDo(ts);
                Bitmap bmp = new Bitmap(Math.Max(1, (int)(trang.Width * k)), Math.Max(1, (int)(trang.Height * k)));
                using (Bitmap anh = chup(pv, Math.Max(200, (int)(khung.Width * k * 1.5f))))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.ScaleTransform(k, k);
                    VeTrangBanDo.Ve(g, ts, anh, pv);
                }
                Bitmap cu = anhXemTruoc;
                anhXemTruoc = bmp;
                xemTruoc.Image = bmp;
                if (cu != null) cu.Dispose();
                double tyLe = VeTrangBanDo.TinhTyLe(khung, pv[2] - pv[0]);
                lblTyLe.Text = "Tỷ lệ thực trên giấy: 1:" + tyLe.ToString("N0", TienIchIn.Vi) + "\nPhạm vi: " +
                    TienIchIn.NhanKhoangCach(pv[2] - pv[0]) + " × " + TienIchIn.NhanKhoangCach(pv[3] - pv[1]);
                DatPhuDe("Khổ " + ts.KhoGiay + (ts.Ngang ? " ngang" : " dọc") + " · tỷ lệ 1:" + tyLe.ToString("N0", TienIchIn.Vi) + " · tâm = tâm khung nhìn hiện tại");
            }
            catch (Exception ex) { lblTyLe.Text = "Lỗi xem trước: " + ex.Message; }
        }

        private PrintDocument TaoTaiLieu()
        {
            DocThongSo();
            return new BanDoIn(ts, VeTrangBanDo.PhamVi(ts, nhin), chup);
        }

        private void XuatPdf()
        {
            TienIchIn.XuatPdf(this, TaoTaiLieu, "BanDo_" + ts.KhoGiay + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf");
        }

        private void InRa()
        {
            using (PrintDocument d = TaoTaiLieu())
                TienIchIn.XemTruocVaIn(this, d, "Xem trước bản đồ " + ts.KhoGiay, TaoTaiLieu,
                    "BanDo_" + ts.KhoGiay + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf");
        }
    }

    // =====================================================================
    // 3. PHIEU HO SO DON VI (A4 doc)
    // =====================================================================
    public sealed class DuLieuPhieuHoSo
    {
        public DonViQuanDoi DonVi;
        public List<DongLichSuViTri> LichSu = new List<DongLichSuViTri>();
        public KhuonVien KhuonVien;
        public Bitmap AnhBanDo;          // vuong, tam = tru so
        public double PhamViAnhM;        // canh anh (m)
        public double E, N;              // UTM 48N
        public double[] Vn2000;          // {X Bac, Y Dong} EPSG:5897
        public string NguoiIn;
    }

    public sealed class PhieuHoSoIn : PrintDocument
    {
        private readonly DuLieuPhieuHoSo d;
        private static readonly Color Dam = Color.FromArgb(22, 49, 74), Xam = Color.FromArgb(96, 116, 138),
            Vien = Color.FromArgb(214, 225, 235), Teal = Color.FromArgb(0, 150, 122), NenNhat = Color.FromArgb(246, 249, 252);

        public PhieuHoSoIn(DuLieuPhieuHoSo duLieu)
        {
            d = duLieu;
            DocumentName = "Phiếu hồ sơ " + (d.DonVi != null ? d.DonVi.MaDonVi : "");
            TienIchIn.DatKhoGiay(this, "A4", false);
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            e.Graphics.TranslateTransform(-e.PageSettings.HardMarginX, -e.PageSettings.HardMarginY);
            Ve(e.Graphics, d);
            e.HasMorePages = false;
        }

        private static string Gt(string s) { return string.IsNullOrWhiteSpace(s) ? "—" : s.Trim(); }

        public static void Ve(Graphics g, DuLieuPhieuHoSo d)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            DonViQuanDoi dv = d.DonVi ?? new DonViQuanDoi();
            const float L = 48, W = 827 - 2 * 48;
            CultureInfo vi = TienIchIn.Vi;
            using (SolidBrush nen = new SolidBrush(Color.White)) g.FillRectangle(nen, 0, 0, 827, 1169);

            using (Font fNho = new Font("Segoe UI", 7.2f))
            using (Font fNhan = new Font("Segoe UI", 7.2f))
            using (Font fGt = new Font("Segoe UI", 8.4f, FontStyle.Bold))
            using (Font fMuc = new Font("Segoe UI", 8.6f, FontStyle.Bold))
            using (Font fTen = new Font("Segoe UI", 15f, FontStyle.Bold))
            using (Font fDau = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (SolidBrush bDam = new SolidBrush(Dam))
            using (SolidBrush bXam = new SolidBrush(Xam))
            using (SolidBrush bTrang = new SolidBrush(Color.White))
            using (SolidBrush bTeal = new SolidBrush(Teal))
            using (Pen pVien = new Pen(Vien, 0.8f))
            using (StringFormat cat = new StringFormat { Trimming = StringTrimming.EllipsisCharacter })
            {
                // ---- Dai tieu de ----
                RectangleF dau = new RectangleF(L, 40, W, 96);
                using (GraphicsPath p = TienIchIn.BoTron(dau, 8))
                using (LinearGradientBrush lg = new LinearGradientBrush(dau, Color.FromArgb(0, 160, 132), Color.FromArgb(44, 104, 201), LinearGradientMode.Horizontal))
                    g.FillPath(lg, p);
                using (SolidBrush vang = new SolidBrush(Color.FromArgb(230, 249, 81))) g.FillRectangle(vang, L, 54, 4, 68);
                g.DrawString("PHIẾU HỒ SƠ ĐƠN VỊ", fDau, bTrang, L + 18, 52);
                g.DrawString(Gt(dv.TenDonVi), fTen, bTrang, new RectangleF(L + 16, 70, W - 170, 34), cat);
                g.DrawString(Gt(dv.MaDonVi) + "   ·   " + Gt(dv.LoaiDonVi), fNho, bTrang, L + 18, 108);
                string mat = string.IsNullOrWhiteSpace(dv.DoMat) ? "THƯỜNG" : dv.DoMat.ToUpper(vi);
                RectangleF the = new RectangleF(L + W - 176, 56, 160, 26);
                using (GraphicsPath p = TienIchIn.BoTron(the, 13))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(60, 255, 255, 255))) g.FillPath(b, p);
                using (StringFormat giua = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString("ĐỘ MẬT: " + mat, fDau, bTrang, the, giua);

                float y = 152;
                Action<string> muc = t =>
                {
                    g.FillRectangle(bTeal, L, y + 2, 3, 13);
                    g.DrawString(t, fMuc, bDam, L + 9, y);
                    y += 22;
                };

                // ---- 1. Thong tin chung (luoi 2 cot) ----
                muc("1. THÔNG TIN CHUNG");
                string[,] o =
                {
                    { "Tên viết tắt", Gt(dv.TenVietTat), "Quân / Binh chủng", Gt(dv.QuanBinhChung) },
                    { "Đơn vị cấp trên", Gt(dv.DonViCapTren), "Trạng thái SSCĐ", Gt(dv.TrangThaiSSCD) },
                    { "Phường / xã", Gt(dv.DiaBanHienThi), "Số điện thoại", Gt(dv.SoDienThoai) },
                    { "Chỉ huy trưởng / Tư lệnh", Gt(dv.ChiHuyTruong), "Chính ủy / CTV", Gt(dv.ChinhUy) },
                };
                float cw = W / 2;
                for (int i = 0; i < o.GetLength(0); i++)
                {
                    for (int c = 0; c < 2; c++)
                    {
                        RectangleF r = new RectangleF(L + c * cw, y, cw - (c == 0 ? 6 : 0), 36);
                        using (SolidBrush b = new SolidBrush(NenNhat)) g.FillRectangle(b, r);
                        g.DrawRectangle(pVien, r.X, r.Y, r.Width, r.Height);
                        g.DrawString(o[i, c * 2], fNhan, bXam, r.X + 8, r.Y + 3);
                        g.DrawString(o[i, c * 2 + 1], fGt, bDam, new RectangleF(r.X + 8, r.Y + 16, r.Width - 14, 18), cat);
                    }
                    y += 40;
                }
                {
                    RectangleF r = new RectangleF(L, y, W, 36);
                    using (SolidBrush b = new SolidBrush(NenNhat)) g.FillRectangle(b, r);
                    g.DrawRectangle(pVien, r.X, r.Y, r.Width, r.Height);
                    g.DrawString("Địa chỉ trụ sở", fNhan, bXam, r.X + 8, r.Y + 3);
                    g.DrawString(Gt(dv.DiaChi), fGt, bDam, new RectangleF(r.X + 8, r.Y + 16, r.Width - 14, 18), cat);
                    y += 50;
                }

                // ---- 2. Vi tri ----
                muc("2. VỊ TRÍ TRỤ SỞ VÀ KHUÔN VIÊN");
                float canh = 300;
                RectangleF anh = new RectangleF(L, y, canh, canh);
                if (d.AnhBanDo != null) g.DrawImage(d.AnhBanDo, anh);
                else using (SolidBrush b = new SolidBrush(NenNhat)) g.FillRectangle(b, anh);
                using (Pen tam = new Pen(Color.FromArgb(230, 60, 80), 1.6f))
                {
                    float cx = anh.X + canh / 2, cy = anh.Y + canh / 2;
                    g.DrawEllipse(tam, cx - 14, cy - 14, 28, 28);
                    g.DrawLine(tam, cx - 22, cy, cx - 8, cy); g.DrawLine(tam, cx + 8, cy, cx + 22, cy);
                    g.DrawLine(tam, cx, cy - 22, cx, cy - 8); g.DrawLine(tam, cx, cy + 8, cx, cy + 22);
                }
                using (Pen pk = new Pen(Dam, 1f)) g.DrawRectangle(pk, anh.X, anh.Y, anh.Width, anh.Height);
                if (d.PhamViAnhM > 0)
                {
                    double tyLe = d.PhamViAnhM / (canh / 100d * 0.0254);
                    VeTrangBanDo.VeThuocTyLe(g, new RectangleF(anh.X + 6, anh.Bottom - 40, 150, 34), tyLe, 0.85f, true);
                    VeTrangBanDo.VeMuiTenBac(g, new RectangleF(anh.Right - 36, anh.Y + 6, 30, 46), 0.7f, true);
                }

                DongLichSuViTri hh = d.LichSu.FirstOrDefault(x => x.LaHienHanh);
                bool coWgs = Math.Abs(dv.KinhDo) > 0.0001 && Math.Abs(dv.ViDo) > 0.0001;
                List<string[]> td = new List<string[]>
                {
                    new[] { "WGS 84 – vĩ độ φ", coWgs ? TienIchIn.Dms(dv.ViDo, true) + "\n" + dv.ViDo.ToString("0.000000", CultureInfo.InvariantCulture) + "°" : "—" },
                    new[] { "WGS 84 – kinh độ λ", coWgs ? TienIchIn.Dms(dv.KinhDo, false) + "\n" + dv.KinhDo.ToString("0.000000", CultureInfo.InvariantCulture) + "°" : "—" },
                    new[] { "UTM 48N (E ; N)", d.E > 0 ? d.E.ToString("N2", vi) + " m ; " + d.N.ToString("N2", vi) + " m" : "—" },
                    new[] { "VN-2000 KTT 105° (X Bắc ; Y Đông)", d.Vn2000 != null ? d.Vn2000[0].ToString("N2", vi) + " m ; " + d.Vn2000[1].ToString("N2", vi) + " m" : "—" },
                    new[] { "Phương pháp định vị", hh != null ? Gt(hh.TenPhuongPhap ?? hh.PhuongPhap) + " · " + hh.SaiSo : "—" },
                    new[] { "Khuôn viên", d.KhuonVien != null
                        ? (d.KhuonVien.DienTichM2 / 10000d).ToString("N2", vi) + " ha · chu vi " + d.KhuonVien.ChuViM.ToString("N0", vi) + " m"
                        : "Chưa số hóa" },
                };
                float x2 = L + canh + 14, w2 = W - canh - 14, y2 = y;
                foreach (string[] dong in td)
                {
                    bool hai = dong[1].Contains("\n");
                    float h = hai ? 46 : 36;
                    RectangleF r = new RectangleF(x2, y2, w2, h);
                    using (SolidBrush b = new SolidBrush(NenNhat)) g.FillRectangle(b, r);
                    g.DrawRectangle(pVien, r.X, r.Y, r.Width, r.Height);
                    g.DrawString(dong[0], fNhan, bXam, r.X + 8, r.Y + 3);
                    g.DrawString(dong[1], fGt, bDam, new RectangleF(r.X + 8, r.Y + 15, r.Width - 14, h - 16), cat);
                    y2 += h + 4;
                }
                g.DrawString("VN-2000: KTT Hà Nội 105°00′, múi 3°, k₀ = 0,9999, 7 tham số chuyển đổi (EPSG:5897). UTM: múi 48N, k₀ = 0,9996.", fNho, bXam, new RectangleF(x2, y2, w2, 26));
                y = Math.Max(y + canh, y2) + 16;

                // ---- 3. Lich su dinh vi ----
                muc("3. LỊCH SỬ ĐỊNH VỊ TRỤ SỞ");
                float[] cot = { 28, 110, 220, 70, 100, 0 };
                cot[5] = W - cot.Take(5).Sum();
                string[] tieu = { "#", "Thời gian", "Phương pháp", "Sai số", "Cách hiện hành", "Trạng thái / nguồn" };
                Action<string[], bool, bool> hang = (gt, laDau, chan) =>
                {
                    float x = L;
                    RectangleF rh = new RectangleF(L, y, W, 22);
                    if (laDau) using (SolidBrush b = new SolidBrush(Color.FromArgb(232, 244, 241))) g.FillRectangle(b, rh);
                    else if (chan) using (SolidBrush b = new SolidBrush(NenNhat)) g.FillRectangle(b, rh);
                    for (int i = 0; i < cot.Length; i++)
                    {
                        g.DrawString(gt[i], laDau ? fDau : fNho, laDau ? bDam : bDam, new RectangleF(x + 5, y + 4, cot[i] - 8, 16), cat);
                        x += cot[i];
                    }
                    g.DrawLine(pVien, L, y + 22, L + W, y + 22);
                    y += 22;
                };
                hang(tieu, true, false);
                List<DongLichSuViTri> ls = d.LichSu.Take(8).ToList();
                if (ls.Count == 0) hang(new[] { "", "Chưa có bản ghi vị trí", "", "", "", "" }, false, false);
                for (int i = 0; i < ls.Count; i++)
                {
                    DongLichSuViTri r = ls[i];
                    hang(new[] { r.Stt.ToString(), r.ThoiGian, Gt(r.TenPhuongPhap ?? r.PhuongPhap), r.SaiSo, r.Cach,
                        r.TrangThai + (string.IsNullOrWhiteSpace(r.Nguon) ? "" : " · " + r.Nguon) }, false, i % 2 == 1);
                }
                if (d.LichSu.Count > ls.Count)
                    g.DrawString("… và " + (d.LichSu.Count - ls.Count) + " bản ghi cũ hơn.", fNho, bXam, L, y + 2);
                y += 18;

                // ---- 4. Nhiem vu / trang bi / ghi chu ----
                List<string[]> them = new List<string[]>();
                if (dv.NhiemVuChinh != null && dv.NhiemVuChinh.Length > 0) them.Add(new[] { "Nhiệm vụ chính", string.Join("; ", dv.NhiemVuChinh) });
                if (!string.IsNullOrWhiteSpace(dv.GhiChu)) them.Add(new[] { "Ghi chú", dv.GhiChu.Trim() });
                if (!string.IsNullOrWhiteSpace(dv.NguonDuLieu)) them.Add(new[] { "Nguồn dữ liệu", dv.NguonDuLieu.Trim() });
                if (them.Count > 0 && y < 1169 - 150)
                {
                    muc("4. THÔNG TIN BỔ SUNG");
                    foreach (string[] t in them)
                    {
                        if (y > 1169 - 110) break;
                        g.DrawString(t[0], fNhan, bXam, L, y);
                        RectangleF r = new RectangleF(L + 110, y, W - 110, Math.Min(46, 1169 - 90 - y));
                        g.DrawString(t[1], fGt, bDam, r, cat);
                        y += Math.Min(46, g.MeasureString(t[1], fGt, (int)r.Width).Height) + 6;
                    }
                }

                // ---- Chan trang ----
                g.DrawLine(pVien, L, 1169 - 62, L + W, 1169 - 62);
                g.DrawString("In ngày " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + (string.IsNullOrWhiteSpace(d.NguoiIn) ? "" : " · " + d.NguoiIn) +
                    " · Nguồn: CSDL HQTCSDL (PostGIS) · ManagerGISAcademy", fNho, bXam, L, 1169 - 56);
                using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far })
                    g.DrawString("Trang 1/1", fNho, bXam, new RectangleF(L, 1169 - 56, W, 14), phai);
            }
        }
    }

    // =====================================================================
    // 4. XUAT DU LIEU GIS
    // =====================================================================
    public sealed class FormXuatDuLieuGis : HopThoaiV2
    {
        private readonly CheckBox chkDonVi, chkKhuonVien, chkXa;
        private readonly RadioButton rGeo, rKml, rShp, rCsv;
        private readonly TextBox txtThuMuc;
        private readonly CheckBox chkMo;
        private readonly ListBox nhatKy;
        private readonly Button btnXuat;
        private readonly Action<string, string, DataTable> ghiShp;   // (duongDan, lop, bang) - chay tren UI thread (COM STA)

        public FormXuatDuLieuGis(Action<string, string, DataTable> ghiShapefile)
            : base("Xuất dữ liệu GIS", "Chia sẻ sang QGIS / ArcGIS / Google Earth · dữ liệu đọc theo quyền (RLS) của vai trò hiện tại", new Size(760, 600))
        {
            ghiShp = ghiShapefile;
            int y = 4;
            Func<string, Label> nhom = t =>
            {
                Label l = NhanTd(t, 8F, true, VeBaoCao.Teal); l.SetBounds(0, y, 600, 18); Than.Controls.Add(l); y += 24; return l;
            };
            nhom("1. LỚP DỮ LIỆU");
            Func<string, string, CheckBox> chk = (t, m) =>
            {
                CheckBox c = new CheckBox { Text = t, Checked = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = TienIchGiaoDienSang.Ink };
                c.SetBounds(4, y, 330, 24); Than.Controls.Add(c);
                Label l = NhanTd(m, 8.5F, false, TienIchGiaoDienSang.Muted); l.SetBounds(340, y + 3, 370, 20); Than.Controls.Add(l);
                y += 28; return c;
            };
            chkDonVi = chk("Đơn vị quân đội (điểm)", "mã, tên, loại, QBC, phường/xã, tọa độ, phương pháp, sai số");
            chkKhuonVien = chk("Khuôn viên doanh trại (vùng)", "diện tích m², chu vi m");
            chkXa = chk("Ranh giới phường / xã Hà Nội (vùng)", "126 đơn vị hành chính cấp xã (2025), đã đơn giản hóa");
            chkXa.Checked = false;
            y += 10;
            nhom("2. ĐỊNH DẠNG");
            Func<string, string, int, int, RadioButton> rad = (t, m, cx, cy) =>
            {
                RadioButton r = new RadioButton { Text = t, Font = new Font("Segoe UI", 10F, FontStyle.Bold), ForeColor = TienIchGiaoDienSang.Ink };
                r.SetBounds(cx, cy, 330, 24); Than.Controls.Add(r);
                Label l = NhanTd(m, 8.5F, false, TienIchGiaoDienSang.Muted); l.SetBounds(cx + 20, cy + 24, 320, 18); Than.Controls.Add(l);
                return r;
            };
            rGeo = rad("GeoJSON", "WGS 84 (EPSG:4326) · web, QGIS, Leaflet", 4, y);
            rKml = rad("KML", "WGS 84 · Google Earth, điện thoại", 360, y);
            y += 50;
            rShp = rad("Shapefile (.shp/.dbf/.prj)", "UTM 48N (EPSG:32648) · ArcGIS, QGIS, MapInfo", 4, y);
            rCsv = rad("CSV + WKT", "UTF-8 · Excel, nhập lại PostGIS", 360, y);
            rGeo.Checked = true;
            y += 56;
            nhom("3. THƯ MỤC LƯU");
            txtThuMuc = new TextBox
            {
                Font = new Font("Segoe UI", 10F),
                Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ManagerGISAcademy_XuatGIS")
            };
            txtThuMuc.SetBounds(4, y, 560, 26); Than.Controls.Add(txtThuMuc);
            Button chon = NutGiaoDien.Phang("Chọn…", Color.White, TienIchGiaoDienSang.Ink, 9F);
            chon.FlatAppearance.BorderSize = 1; chon.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            chon.SetBounds(572, y - 1, 120, 28); Than.Controls.Add(chon);
            chon.Click += (s, e) =>
            {
                using (FolderBrowserDialog f = new FolderBrowserDialog { SelectedPath = txtThuMuc.Text, ShowNewFolderButton = true })
                    if (f.ShowDialog(this) == DialogResult.OK) txtThuMuc.Text = f.SelectedPath;
            };
            y += 34;
            chkMo = new CheckBox { Text = "Mở thư mục sau khi xuất", Checked = true, Font = new Font("Segoe UI", 9.5F), ForeColor = TienIchGiaoDienSang.Ink };
            chkMo.SetBounds(4, y, 300, 24); Than.Controls.Add(chkMo);
            y += 30;
            nhatKy = new ListBox { BorderStyle = BorderStyle.FixedSingle, Font = new Font("Consolas", 9F), IntegralHeight = false, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom };
            nhatKy.SetBounds(4, y, 688, 90); Than.Controls.Add(nhatKy);

            btnXuat = ThemNut("⇩  Xuất dữ liệu", VeBaoCao.Teal, Color.White, 150, async (s, e) => await Xuat());
            ThemNut("Đóng", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => Close());
        }

        private void Ghi(string s) { nhatKy.Items.Add(DateTime.Now.ToString("HH:mm:ss") + "  " + s); nhatKy.TopIndex = Math.Max(0, nhatKy.Items.Count - 1); }

        private async Task Xuat()
        {
            List<Tuple<string, string>> lop = new List<Tuple<string, string>>();
            if (chkDonVi.Checked) lop.Add(Tuple.Create("don_vi", "don_vi_quan_doi"));
            if (chkKhuonVien.Checked) lop.Add(Tuple.Create("khuon_vien", "khuon_vien_don_vi"));
            if (chkXa.Checked) lop.Add(Tuple.Create("xa_phuong", "xa_phuong_ha_noi"));
            if (lop.Count == 0) { MessageBox.Show(this, "Chọn ít nhất một lớp.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            string thuMuc = txtThuMuc.Text.Trim();
            try { Directory.CreateDirectory(thuMuc); }
            catch (Exception ex) { MessageBox.Show(this, "Không tạo được thư mục: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string hau = DateTime.Now.ToString("yyyyMMdd_HHmm");
            btnXuat.Enabled = false;
            UseWaitCursor = true;
            int ok = 0;
            try
            {
                TruyXuatTracDia dal = new TruyXuatTracDia();
                foreach (Tuple<string, string> l in lop)
                {
                    string goc = Path.Combine(thuMuc, l.Item2 + "_" + hau);
                    try
                    {
                        if (rGeo.Checked)
                        {
                            string json = await Task.Run(() => dal.XuatGeoJson(l.Item1));
                            File.WriteAllText(goc + ".geojson", json, new UTF8Encoding(false));
                            Ghi("✓ " + Path.GetFileName(goc) + ".geojson  (" + (json.Length / 1024) + " KB)");
                        }
                        else if (rKml.Checked)
                        {
                            List<string[]> ds = await Task.Run(() => dal.XuatKml(l.Item1));
                            File.WriteAllText(goc + ".kml", TaoKml(l.Item2, l.Item1, ds), new UTF8Encoding(false));
                            Ghi("✓ " + Path.GetFileName(goc) + ".kml  (" + ds.Count + " đối tượng)");
                        }
                        else
                        {
                            DataTable t = await Task.Run(() => dal.XuatBang(l.Item1));
                            if (rCsv.Checked)
                            {
                                GhiCsv(goc + ".csv", t);
                                Ghi("✓ " + Path.GetFileName(goc) + ".csv  (" + t.Rows.Count + " dòng)");
                            }
                            else
                            {
                                ghiShp(goc + ".shp", l.Item1, t);
                                Ghi("✓ " + Path.GetFileName(goc) + ".shp  (" + t.Rows.Count + " đối tượng, EPSG:32648)");
                            }
                        }
                        ok++;
                    }
                    catch (Exception ex) { Ghi("✕ " + l.Item2 + ": " + ex.Message); }
                }
            }
            finally
            {
                UseWaitCursor = false;
                btnXuat.Enabled = true;
            }
            Ghi("Hoàn tất " + ok + "/" + lop.Count + " lớp → " + thuMuc);
            if (ok > 0 && chkMo.Checked) TienIchIn.MoTep(this, thuMuc);
        }

        private static string TaoKml(string ten, string lop, List<string[]> ds)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.AppendLine("<kml xmlns=\"http://www.opengis.net/kml/2.2\"><Document>");
            sb.AppendLine("<name>" + SecurityElement.Escape(ten) + "</name>");
            // KML mau aabbggrr
            sb.AppendLine("<Style id=\"dv\"><IconStyle><color>ff3c46e6</color><scale>1.1</scale><Icon><href>http://maps.google.com/mapfiles/kml/shapes/placemark_circle.png</href></Icon></IconStyle><LabelStyle><scale>0.8</scale></LabelStyle></Style>");
            sb.AppendLine("<Style id=\"kv\"><LineStyle><color>ff708c00</color><width>2</width></LineStyle><PolyStyle><color>5584b800</color></PolyStyle></Style>");
            sb.AppendLine("<Style id=\"xa\"><LineStyle><color>ff8a6a2c</color><width>1.2</width></LineStyle><PolyStyle><fill>0</fill></PolyStyle></Style>");
            string kieu = lop == "khuon_vien" ? "kv" : lop == "xa_phuong" ? "xa" : "dv";
            foreach (string[] r in ds)
            {
                if (string.IsNullOrWhiteSpace(r[2])) continue;
                sb.Append("<Placemark><name>").Append(SecurityElement.Escape(r[0])).Append("</name><description>")
                  .Append(SecurityElement.Escape(r[1])).Append("</description><styleUrl>#").Append(kieu).Append("</styleUrl>")
                  .Append(r[2]).AppendLine("</Placemark>");
            }
            sb.AppendLine("</Document></kml>");
            return sb.ToString();
        }

        private static void GhiCsv(string tep, DataTable t)
        {
            Func<object, string> q = v =>
            {
                if (v == null || v == DBNull.Value) return "";
                string s = v is IFormattable ? ((IFormattable)v).ToString(null, CultureInfo.InvariantCulture) : v.ToString();
                return s.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0 ? "\"" + s.Replace("\"", "\"\"") + "\"" : s;
            };
            using (StreamWriter w = new StreamWriter(tep, false, new UTF8Encoding(true)))
            {
                w.WriteLine(string.Join(",", t.Columns.Cast<DataColumn>().Select(c => q(c.ColumnName))));
                foreach (DataRow r in t.Rows) w.WriteLine(string.Join(",", r.ItemArray.Select(q)));
            }
        }
    }
}
