using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>Mot nhom thong ke (dung chung man hinh + ban in).</summary>
    public sealed class NhomThongKe
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public double TyLe { get; set; }           // 0..100
        public string TyLeHienThi { get; set; }
    }

    /// <summary>Du lieu dau vao cua bao cao (chup lai tai thoi diem in).</summary>
    public sealed class DuLieuBaoCaoThongKe
    {
        public string TieuChi { get; set; }
        public DateTime ThoiGian { get; set; }
        public int Tong { get; set; }
        public int CoToaDo { get; set; }
        public int? ViTriThat { get; set; }        // null = khong doc duoc v_thong_ke_vi_tri
        public int? DiemDaiDien { get; set; }
        public int? DatMuoiMet { get; set; }
        public List<NhomThongKe> Nhom { get; set; }
        public List<DonViQuanDoi> DanhSach { get; set; }
    }

    /// <summary>
    /// Ve bieu do dung chung cho man hinh (Graphics cua control) va ban in
    /// (Graphics cua may in, don vi 1/100 inch). Dung DrawString (khong dung
    /// TextRenderer) de chu dung ti le tren ca hai.
    /// </summary>
    internal static class VeBaoCao
    {
        public static readonly Color Ink = Color.FromArgb(22, 49, 74);
        public static readonly Color Sub = Color.FromArgb(102, 122, 139);
        public static readonly Color Line = Color.FromArgb(220, 231, 241);
        public static readonly Color Track = Color.FromArgb(237, 243, 249);
        public static readonly Color Teal = Color.FromArgb(0, 162, 133);
        public static readonly Color Blue = Color.FromArgb(39, 126, 205);
        public static readonly Color Violet = Color.FromArgb(109, 90, 205);
        public static readonly Color Amber = Color.FromArgb(209, 127, 35);
        public static readonly Color Gray = Color.FromArgb(154, 170, 187);
        public static readonly CultureInfo Vi = CultureInfo.GetCultureInfo("vi-VN");

        public static readonly Color[] MauNhom =
        {
            Teal, Blue, Violet, Amber, Color.FromArgb(7, 150, 174),
            Color.FromArgb(230, 107, 152), Color.FromArgb(108, 171, 40), Gray
        };

        /// <summary>Gop nhom cho bieu do: toi da 8 thanh (7 + "Khac").</summary>
        public static List<NhomThongKe> NhomBieuDo(IList<NhomThongKe> tatCa, int tong)
        {
            if (tatCa == null) return new List<NhomThongKe>();
            if (tatCa.Count <= 8) return tatCa.ToList();
            List<NhomThongKe> kq = tatCa.Take(7).ToList();
            List<NhomThongKe> conLai = tatCa.Skip(7).ToList();
            int n = conLai.Sum(x => x.SoLuong);
            double tl = tong > 0 ? 100d * n / tong : 0d;
            kq.Add(new NhomThongKe
            {
                Ten = "Khác (" + conLai.Count + " nhóm)",
                SoLuong = n,
                TyLe = tl,
                TyLeHienThi = tl.ToString("N1", Vi) + "%"
            });
            return kq;
        }

        public static void BieuDoThanh(Graphics g, RectangleF r, IList<NhomThongKe> nhom,
            Font chu, Font chuDam, Font chuNho)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (nhom == null || nhom.Count == 0)
            {
                using (SolidBrush b = new SolidBrush(Sub))
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString("Chưa có dữ liệu", chu, b, r, sf);
                return;
            }
            int max = Math.Max(1, nhom.Max(x => x.SoLuong));
            float hang = r.Height / Math.Max(nhom.Count, 6);
            float cotTen = Math.Min(r.Width * 0.36f, 220f * r.Width / 600f + 40f);
            float cotSo = Math.Max(70f, r.Width * 0.15f);
            float rongThanh = Math.Max(10f, r.Width - cotTen - cotSo - 12f);
            float caoThanh = Math.Max(5f, Math.Min(12f, hang * 0.38f));
            using (StringFormat trai = new StringFormat { LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center, FormatFlags = StringFormatFlags.NoWrap })
            using (SolidBrush ink = new SolidBrush(Ink))
            using (SolidBrush sub = new SolidBrush(Sub))
            using (SolidBrush track = new SolidBrush(Track))
            {
                for (int i = 0; i < nhom.Count; i++)
                {
                    float y = r.Top + i * hang;
                    RectangleF ten = new RectangleF(r.Left, y, cotTen, hang);
                    g.DrawString(nhom[i].Ten, chu, ink, ten, trai);
                    RectangleF tr = new RectangleF(r.Left + cotTen + 6, y + (hang - caoThanh) / 2, rongThanh, caoThanh);
                    using (GraphicsPath p = BoTron(tr, caoThanh / 2)) g.FillPath(track, p);
                    float w = Math.Max(caoThanh, rongThanh * nhom[i].SoLuong / max);
                    Color mau = nhom[i].Ten.StartsWith("Khác (", StringComparison.Ordinal) ? Gray : MauNhom[i % 7];
                    using (SolidBrush b = new SolidBrush(mau))
                    using (GraphicsPath p = BoTron(new RectangleF(tr.Left, tr.Top, w, caoThanh), caoThanh / 2))
                        g.FillPath(b, p);
                    RectangleF so = new RectangleF(r.Right - cotSo, y, cotSo, hang);
                    string sl = nhom[i].SoLuong.ToString("N0", Vi);
                    SizeF kt = g.MeasureString(" " + nhom[i].TyLeHienThi, chuNho);
                    g.DrawString(nhom[i].TyLeHienThi, chuNho, sub, so, phai);
                    g.DrawString(sl, chuDam, ink, new RectangleF(so.Left, so.Top, so.Width - kt.Width, so.Height), phai);
                }
            }
        }

        /// <summary>Bieu do vanh: vi tri that (teal) / diem dai dien (amber).</summary>
        public static void BieuDoVanh(Graphics g, RectangleF o, int thuc, int daiDien,
            Font chuLon, Font chuNho)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            int tong = Math.Max(0, thuc + daiDien);
            float day = o.Width * 0.16f;
            RectangleF vanh = new RectangleF(o.Left + day / 2, o.Top + day / 2, o.Width - day, o.Height - day);
            using (Pen nen = new Pen(Amber, day))
                g.DrawEllipse(nen, vanh);
            if (tong > 0 && thuc > 0)
            {
                float goc = 360f * thuc / tong;
                using (Pen p = new Pen(Teal, day))
                    g.DrawArc(p, vanh, -90f, goc);
            }
            if (tong == 0)
            {
                using (Pen p = new Pen(Track, day)) g.DrawEllipse(p, vanh);
            }
            string pt = tong > 0 ? (100d * thuc / tong).ToString("N1", Vi) + "%" : "—";
            using (SolidBrush ink = new SolidBrush(Ink))
            using (SolidBrush sub = new SolidBrush(Sub))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far })
            using (StringFormat sf2 = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near })
            {
                RectangleF tren = new RectangleF(o.Left, o.Top, o.Width, o.Height / 2 + o.Height * 0.06f);
                RectangleF duoi = new RectangleF(o.Left, o.Top + o.Height / 2 + o.Height * 0.04f, o.Width, o.Height / 2);
                g.DrawString(pt, chuLon, ink, tren, sf);
                g.DrawString("đã xác minh", chuNho, sub, duoi, sf2);
            }
        }

        public static GraphicsPath BoTron(RectangleF r, float ban)
        {
            GraphicsPath p = new GraphicsPath();
            float d = Math.Max(0.5f, Math.Min(ban * 2, Math.Min(r.Width, r.Height)));
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    /// <summary>
    /// Tai lieu in A4 doc cua bao cao thong ke: dung cho ca "Xuat PDF"
    /// (qua may in ao Microsoft Print to PDF) va "In bao cao" (xem truoc + in).
    /// Trang 1: tieu de, 4 chi so, bieu do phan bo, chat luong vi tri, bang
    /// tong hop; tiep theo: phu luc danh sach don vi (tu sang trang, lap tieu de cot).
    /// </summary>
    public sealed class BaoCaoThongKeIn : PrintDocument
    {
        private const float HangBang = 19f;      // 1/100 inch
        private const float HangPhuLuc = 16.5f;
        private const float CaoChanTrang = 26f;

        private readonly DuLieuBaoCaoThongKe d;
        private readonly List<DonViQuanDoi> phuLuc;
        private List<Trang> trang;
        private int chiSoTrang;
        private int trangCuoi;

        private sealed class Trang
        {
            public bool LaTrangDau;
            public int NhomTu, NhomDen;        // [tu, den)
            public bool TieuDePhuLuc;
            public int DvTu, DvDen;
        }

        public BaoCaoThongKeIn(DuLieuBaoCaoThongKe duLieu)
        {
            d = duLieu ?? throw new ArgumentNullException("duLieu");
            phuLuc = (d.DanhSach ?? new List<DonViQuanDoi>())
                .OrderBy(x => x.LoaiDonVi ?? "", StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(x => x.TenDonVi ?? "", StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            DocumentName = "Báo cáo thống kê đơn vị " + d.ThoiGian.ToString("dd-MM-yyyy HH-mm", VeBaoCao.Vi);
            DefaultPageSettings.Margins = new Margins(50, 50, 45, 45);
            DefaultPageSettings.Landscape = false;
            try
            {
                PaperSize a4 = PrinterSettings.PaperSizes.Cast<PaperSize>()
                    .FirstOrDefault(p => p.Kind == PaperKind.A4);
                if (a4 != null) DefaultPageSettings.PaperSize = a4;
            }
            catch (Exception) { /* may in khong liet ke kho giay: giu mac dinh */ }
        }

        // ---------------- chia trang (thuan so hoc, khong can Graphics) ----------------

        private List<Trang> ChiaTrang(float caoVung)
        {
            List<Trang> ds = new List<Trang>();
            float conLaiTrangDau = caoVung - CaoChanTrang - CaoPhanDau();
            int nhomTong = d.Nhom == null ? 0 : d.Nhom.Count;
            int nhom = 0, dv = 0;

            // Trang dau: bang tong hop bat dau sau bieu do.
            Trang t = new Trang { LaTrangDau = true };
            int vua = Math.Max(0, (int)((conLaiTrangDau - 34f - HangBang) / HangBang));
            t.NhomTu = 0; t.NhomDen = Math.Min(nhomTong, vua);
            nhom = t.NhomDen;
            ds.Add(t);

            float caoTrangSau = caoVung - CaoChanTrang;
            while (nhom < nhomTong)
            {
                int n = Math.Max(1, (int)((caoTrangSau - 34f - HangBang) / HangBang));
                Trang tt = new Trang { NhomTu = nhom, NhomDen = Math.Min(nhomTong, nhom + n) };
                nhom = tt.NhomDen;
                ds.Add(tt);
            }
            bool batDauPhuLuc = true;
            while (dv < phuLuc.Count)
            {
                float cao = caoTrangSau - (batDauPhuLuc ? 52f : 8f) - HangPhuLuc;
                int n = Math.Max(1, (int)(cao / HangPhuLuc));
                Trang tt = new Trang
                {
                    NhomTu = 0, NhomDen = 0,
                    TieuDePhuLuc = batDauPhuLuc,
                    DvTu = dv, DvDen = Math.Min(phuLuc.Count, dv + n)
                };
                dv = tt.DvDen;
                batDauPhuLuc = false;
                ds.Add(tt);
            }
            return ds;
        }

        // Tieu de (78) + meta (6) + chi so (64) + muc 1 (26 + 214) + muc 2 (26 + 150)
        private static float CaoPhanDau() { return 78f + 14f + 64f + 18f + 26f + 214f + 14f + 26f + 150f + 14f; }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            base.OnBeginPrint(e);
            // Chon lai kho A4 theo may in THUC SU se dung (co the da doi sau khi khoi tao).
            try
            {
                PaperSize a4 = PrinterSettings.PaperSizes.Cast<PaperSize>()
                    .FirstOrDefault(p => p.Kind == PaperKind.A4);
                if (a4 != null) DefaultPageSettings.PaperSize = a4;
            }
            catch (Exception) { }
            Rectangle mb = DefaultPageSettings.Bounds;
            Margins m = DefaultPageSettings.Margins;
            float caoVung = (DefaultPageSettings.Landscape ? mb.Width : mb.Height) - m.Top - m.Bottom;
            trang = ChiaTrang(caoVung);
            chiSoTrang = 0;
            trangCuoi = trang.Count;
            if (PrinterSettings.PrintRange == PrintRange.SomePages)
            {
                chiSoTrang = Math.Max(0, Math.Min(trang.Count - 1, PrinterSettings.FromPage - 1));
                trangCuoi = Math.Max(chiSoTrang + 1, Math.Min(trang.Count, PrinterSettings.ToPage));
            }
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            base.OnPrintPage(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            RectangleF vung = e.MarginBounds;
            Trang t = trang[chiSoTrang];
            float y = vung.Top;

            using (Font f8 = new Font("Segoe UI", 8f))
            using (Font f85 = new Font("Segoe UI", 8.5f))
            using (Font f9 = new Font("Segoe UI", 9f))
            using (Font f9b = new Font("Segoe UI", 9f, FontStyle.Bold))
            using (Font f11b = new Font("Segoe UI", 11f, FontStyle.Bold))
            using (Font f16b = new Font("Segoe UI", 16f, FontStyle.Bold))
            using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
            using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
            using (Pen vien = new Pen(VeBaoCao.Line, 1f))
            {
                if (t.LaTrangDau)
                {
                    // --- Dai tieu de ---
                    RectangleF dai = new RectangleF(vung.Left, y, vung.Width, 70f);
                    using (LinearGradientBrush lg = new LinearGradientBrush(dai, Color.FromArgb(0, 169, 138), Color.FromArgb(44, 104, 201), LinearGradientMode.Horizontal))
                    using (GraphicsPath p = VeBaoCao.BoTron(dai, 8f))
                        g.FillPath(lg, p);
                    using (SolidBrush chuTrang = new SolidBrush(Color.White))
                    using (Font fTieuDe = new Font("Segoe UI", 13.5f, FontStyle.Bold))
                    {
                        g.DrawString("BÁO CÁO THỐNG KÊ ĐƠN VỊ QUÂN ĐỘI TRÊN ĐỊA BÀN HÀ NỘI",
                            fTieuDe, chuTrang, dai.Left + 16, dai.Top + 13);
                        g.DrawString("Hệ thống quản lý đơn vị quân đội · Tiêu chí thống kê: " + d.TieuChi +
                            " · Lập lúc " + d.ThoiGian.ToString("dd/MM/yyyy HH:mm", VeBaoCao.Vi),
                            f85, chuTrang, dai.Left + 17, dai.Top + 42);
                    }
                    y += 78f + 14f;

                    // --- 4 chi so ---
                    string[] tieuDe = { "TỔNG ĐƠN VỊ", "CÓ TỌA ĐỘ WGS84", "VỊ TRÍ ĐÃ XÁC MINH", "NHÓM THỐNG KÊ" };
                    string[] giaTri =
                    {
                        d.Tong.ToString("N0", VeBaoCao.Vi),
                        d.CoToaDo.ToString("N0", VeBaoCao.Vi),
                        d.ViTriThat.HasValue ? d.ViTriThat.Value.ToString("N0", VeBaoCao.Vi) : "—",
                        (d.Nhom == null ? 0 : d.Nhom.Count).ToString("N0", VeBaoCao.Vi)
                    };
                    string[] moTa =
                    {
                        "hồ sơ được cấp quyền",
                        PhanTram(d.CoToaDo, d.Tong) + " tổng số",
                        d.ViTriThat.HasValue ? PhanTram(d.ViTriThat.Value, d.Tong) + " tổng số" : "chưa đọc được",
                        "theo " + (d.TieuChi ?? "").ToLower(VeBaoCao.Vi)
                    };
                    Color[] mau = { VeBaoCao.Teal, VeBaoCao.Blue, VeBaoCao.Amber, VeBaoCao.Violet };
                    float w = (vung.Width - 3 * 10f) / 4f;
                    for (int i = 0; i < 4; i++)
                    {
                        RectangleF o = new RectangleF(vung.Left + i * (w + 10f), y, w, 64f);
                        using (GraphicsPath p = VeBaoCao.BoTron(o, 6f)) g.DrawPath(vien, p);
                        using (SolidBrush b = new SolidBrush(mau[i]))
                            g.FillRectangle(b, o.Left, o.Top + 10, 3.5f, o.Height - 20);
                        g.DrawString(tieuDe[i], f8, sub, o.Left + 11, o.Top + 7);
                        using (SolidBrush b = new SolidBrush(mau[i]))
                            g.DrawString(giaTri[i], f16b, b, o.Left + 9, o.Top + 19);
                        g.DrawString(moTa[i], f8, sub, new RectangleF(o.Left + 11, o.Top + 46, o.Width - 14, 16), KhongXuongDong());
                    }
                    y += 64f + 18f;

                    // --- Muc 1: phan bo ---
                    g.DrawString("1. Phân bố theo " + (d.TieuChi ?? "").ToLower(VeBaoCao.Vi), f11b, ink, vung.Left, y);
                    y += 26f;
                    List<NhomThongKe> bd = VeBaoCao.NhomBieuDo(d.Nhom, d.Tong);
                    VeBaoCao.BieuDoThanh(g, new RectangleF(vung.Left + 4, y, vung.Width - 8, 210f), bd, f9, f9b, f8);
                    y += 214f + 14f;

                    // --- Muc 2: chat luong vi tri ---
                    g.DrawString("2. Chất lượng vị trí trụ sở", f11b, ink, vung.Left, y);
                    y += 26f;
                    if (d.ViTriThat.HasValue && d.DiemDaiDien.HasValue)
                    {
                        using (Font f13b = new Font("Segoe UI", 13f, FontStyle.Bold))
                            VeBaoCao.BieuDoVanh(g, new RectangleF(vung.Left + 30, y + 4, 140f, 140f),
                                d.ViTriThat.Value, d.DiemDaiDien.Value, f13b, f8);
                        float lx = vung.Left + 210f;
                        DongChuGiai(g, lx, y + 18, vung.Right - lx, VeBaoCao.Teal, "Vị trí thật (đo đạc / số hóa / mã hóa địa chỉ)", d.ViTriThat.Value, f9, f9b, ink);
                        DongChuGiai(g, lx, y + 44, vung.Right - lx, VeBaoCao.Amber, "Điểm đại diện trong ranh giới xã/phường", d.DiemDaiDien.Value, f9, f9b, ink);
                        DongChuGiai(g, lx, y + 70, vung.Right - lx, VeBaoCao.Gray, "Trong đó đạt sai số ≤ 10 m", d.DatMuoiMet ?? 0, f9, f9b, ink);
                        g.DrawString("Điểm đại diện chưa phải trụ sở thật; cần định vị lại bằng GNSS hoặc số hóa trên ảnh vệ tinh.",
                            f8, sub, new RectangleF(lx, y + 100, vung.Right - lx, 34));
                    }
                    else
                    {
                        g.DrawString("Không đọc được thống kê vị trí (ql_don_vi.v_thong_ke_vi_tri).", f9, sub, vung.Left, y + 6);
                    }
                    y += 150f + 14f;
                }

                // --- Muc 3: bang tong hop ---
                if (t.NhomDen > t.NhomTu)
                {
                    g.DrawString(t.NhomTu == 0 ? "3. Bảng tổng hợp" : "3. Bảng tổng hợp (tiếp)", f11b, ink, vung.Left, y);
                    y += 26f;
                    float[] cot = { 0.07f, 0.53f, 0.15f, 0.25f };
                    string[] td = { "#", "Nhóm", "Số hồ sơ", "Tỷ lệ" };
                    y = HangTieuDe(g, vung, y, cot, td, new[] { false, false, true, true }, f8, sub);
                    for (int i = t.NhomTu; i < t.NhomDen; i++)
                    {
                        NhomThongKe n = d.Nhom[i];
                        string[] gt = { (i + 1).ToString(VeBaoCao.Vi), n.Ten, n.SoLuong.ToString("N0", VeBaoCao.Vi), n.TyLeHienThi };
                        y = HangDuLieu(g, vung, y, HangBang, cot, gt, new[] { false, false, true, true }, f9, ink, vien, i % 2 == 1);
                    }
                    y += 10f;
                }

                // --- Phu luc: danh sach don vi ---
                if (t.DvDen > t.DvTu)
                {
                    if (t.TieuDePhuLuc)
                    {
                        g.DrawString("Phụ lục. Danh sách đơn vị (" + phuLuc.Count.ToString("N0", VeBaoCao.Vi) + " hồ sơ)", f11b, ink, vung.Left, y);
                        y += 22f;
                        g.DrawString("Sắp xếp theo loại đơn vị, tên đơn vị. Tọa độ WGS84 (EPSG:4326).", f8, sub, vung.Left, y);
                        y += 22f;
                    }
                    float[] cot = { 0.045f, 0.145f, 0.31f, 0.15f, 0.155f, 0.10f, 0.095f };
                    string[] td = { "#", "Mã", "Tên đơn vị", "Loại", "Phường/xã", "Kinh độ", "Vĩ độ" };
                    bool[] phai = { false, false, false, false, false, true, true };
                    y = HangTieuDe(g, vung, y, cot, td, phai, f8, sub);
                    for (int i = t.DvTu; i < t.DvDen; i++)
                    {
                        DonViQuanDoi dv = phuLuc[i];
                        bool coTd = NghiepVuDonVi.CoToaDoWgs84(dv);
                        string[] gt =
                        {
                            (i + 1).ToString(VeBaoCao.Vi), dv.MaDonVi ?? "", dv.TenDonVi ?? "", dv.LoaiDonVi ?? "",
                            dv.PhuongXa ?? dv.QuanHuyen ?? "",
                            coTd ? dv.KinhDo.ToString("0.00000", CultureInfo.InvariantCulture) : "—",
                            coTd ? dv.ViDo.ToString("0.00000", CultureInfo.InvariantCulture) : "—"
                        };
                        y = HangDuLieu(g, vung, y, HangPhuLuc, cot, gt, phai, f8, ink, vien, i % 2 == 1);
                    }
                }

                // --- Chan trang ---
                float yc = vung.Bottom - 14f;
                g.DrawLine(vien, vung.Left, yc - 4, vung.Right, yc - 4);
                g.DrawString("Hệ thống quản lý đơn vị quân đội trên địa bàn Hà Nội · Nguồn: BLL / PostGIS (HQTCSDL)",
                    f8, sub, vung.Left, yc);
                using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Far })
                    g.DrawString("Trang " + (chiSoTrang + 1) + "/" + trang.Count, f8, sub,
                        new RectangleF(vung.Left, yc, vung.Width, 16), sf);
            }

            chiSoTrang++;
            e.HasMorePages = chiSoTrang < trangCuoi;
        }

        private static void DongChuGiai(Graphics g, float x, float y, float w, Color mau,
            string ten, int so, Font f, Font fb, Brush ink)
        {
            using (SolidBrush b = new SolidBrush(mau))
            using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(x, y + 4, 10, 10), 2.5f))
                g.FillPath(b, p);
            g.DrawString(ten, f, ink, x + 16, y);
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Far })
                g.DrawString(so.ToString("N0", VeBaoCao.Vi), fb, ink, new RectangleF(x, y, w, 18), sf);
        }

        private static float HangTieuDe(Graphics g, RectangleF vung, float y, float[] cot,
            string[] td, bool[] phai, Font f, Brush sub)
        {
            using (SolidBrush nen = new SolidBrush(Color.FromArgb(242, 247, 252)))
                g.FillRectangle(nen, vung.Left, y, vung.Width, HangBang);
            float x = vung.Left;
            for (int i = 0; i < cot.Length; i++)
            {
                float w = vung.Width * cot[i];
                using (StringFormat sf = DinhDang(phai[i]))
                    g.DrawString(td[i], f, sub, new RectangleF(x + 4, y, w - 8, HangBang), sf);
                x += w;
            }
            return y + HangBang;
        }

        private static float HangDuLieu(Graphics g, RectangleF vung, float y, float cao, float[] cot,
            string[] gt, bool[] phai, Font f, Brush ink, Pen vien, bool le)
        {
            if (le)
                using (SolidBrush nen = new SolidBrush(Color.FromArgb(250, 252, 254)))
                    g.FillRectangle(nen, vung.Left, y, vung.Width, cao);
            float x = vung.Left;
            for (int i = 0; i < cot.Length; i++)
            {
                float w = vung.Width * cot[i];
                using (StringFormat sf = DinhDang(phai[i]))
                    g.DrawString(gt[i], f, ink, new RectangleF(x + 4, y, w - 8, cao), sf);
                x += w;
            }
            g.DrawLine(vien, vung.Left, y + cao, vung.Right, y + cao);
            return y + cao;
        }

        private static StringFormat DinhDang(bool phai)
        {
            return new StringFormat
            {
                Alignment = phai ? StringAlignment.Far : StringAlignment.Near,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap
            };
        }

        private static StringFormat KhongXuongDong()
        {
            return new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap };
        }

        private static string PhanTram(int a, int b)
        {
            return b > 0 ? (100d * a / b).ToString("N1", VeBaoCao.Vi) + "%" : "—";
        }
    }
}
