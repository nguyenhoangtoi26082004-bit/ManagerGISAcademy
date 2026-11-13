using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Views
{
    // =====================================================
    // Cac khung tu ve dung chung cho trang thiet ke v2
    // (Kiem tra du lieu; cung phong cach trang Thong ke va bao cao).
    // =====================================================

    /// <summary>Banner gradient: tieu de + phu de ben trai, nhan + gia tri ben phai.</summary>
    internal sealed class BannerTrang : Control
    {
        private string tieuDe = "", phuDe = "", nhanPhai = "", giaTriPhai = "—";

        public string TieuDe { get { return tieuDe; } set { tieuDe = value ?? ""; Invalidate(); } }
        public string PhuDe { get { return phuDe; } set { phuDe = value ?? ""; Invalidate(); } }
        public string NhanPhai { get { return nhanPhai; } set { nhanPhai = value ?? ""; Invalidate(); } }
        public string GiaTriPhai { get { return giaTriPhai; } set { giaTriPhai = value ?? ""; Invalidate(); } }

        /// <summary>Cho trong ben phai (px) danh cho nut tai khoan noi o goc phai man hinh.</summary>
        public static int ChoTrongPhai;

        public BannerTrang()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = TienIchGiaoDienSang.Background;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 6);
            Color c0 = Color.FromArgb(0, 169, 138), c1 = Color.FromArgb(28, 143, 208), c2 = Color.FromArgb(44, 104, 201);
            using (LinearGradientBrush lg = new LinearGradientBrush(r, c0, c2, LinearGradientMode.Horizontal))
            using (GraphicsPath p = VeBaoCao.BoTron(r, 16))
            {
                lg.InterpolationColors = new ColorBlend { Colors = new[] { c0, c1, c2 }, Positions = new[] { 0f, 0.6f, 1f } };
                g.FillPath(lg, p);
                g.SetClip(p);
                using (Pen vong = new Pen(Color.FromArgb(28, Color.White), 1.4f))
                    for (int i = 1; i <= 5; i++)
                        g.DrawEllipse(vong, r.Right - 150 - i * 26, r.Top + r.Height / 2 - i * 26, i * 52, i * 52);
                g.ResetClip();
            }
            using (SolidBrush vang = new SolidBrush(Color.FromArgb(230, 249, 81)))
                g.FillRectangle(vang, 0, 18, 5, Math.Max(0, Height - 42));
            using (Font f1 = new Font("Segoe UI", 19F, FontStyle.Bold))
            using (Font f2 = new Font("Segoe UI", 9.6F))
            using (Font f3 = new Font("Segoe UI", 8.6F))
            using (Font f4 = new Font("Segoe UI", 12F, FontStyle.Bold))
            using (SolidBrush trang = new SolidBrush(Color.White))
            using (SolidBrush trangMo = new SolidBrush(Color.FromArgb(225, Color.White)))
            using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far })
            {
                g.DrawString(tieuDe, f1, trang, 26, 14);
                g.DrawString(phuDe, f2, trangMo, 28, 52);
                g.DrawString(nhanPhai, f3, trangMo, new RectangleF(0, 22, Width - 26 - ChoTrongPhai, 20), phai);
                g.DrawString(giaTriPhai, f4, trang, new RectangleF(0, 40, Width - 26 - ChoTrongPhai, 26), phai);
            }
        }
    }

    /// <summary>The chi so: o bieu tuong mau + tieu de + gia tri lon + mo ta.</summary>
    internal sealed class TheChiSoChung : Control
    {
        private readonly string icon, tieuDe;
        private readonly Color mau, mauNhat;
        private string giaTri = "—", moTa = "";

        public TheChiSoChung(string icon, string tieuDe, Color mau, Color mauNhat)
        {
            this.icon = icon; this.tieuDe = tieuDe; this.mau = mau; this.mauNhat = mauNhat;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            BackColor = TienIchGiaoDienSang.Background;
        }

        public void DatGiaTri(string gt, string mt) { giaTri = gt ?? "—"; moTa = mt ?? ""; Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(BackColor);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            RectangleF r = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
            using (GraphicsPath p = VeBaoCao.BoTron(r, 14))
            using (SolidBrush b = new SolidBrush(Color.White))
            using (Pen pen = new Pen(Color.FromArgb(228, 236, 245)))
            {
                g.FillPath(b, p);
                g.DrawPath(pen, p);
            }
            RectangleF o = new RectangleF(r.Left + 16, r.Top + (r.Height - 46) / 2, 46, 46);
            using (SolidBrush b = new SolidBrush(mauNhat))
            using (GraphicsPath p = VeBaoCao.BoTron(o, 12)) g.FillPath(b, p);
            using (Font fi = new Font("Segoe UI", 15F, FontStyle.Bold))
            using (SolidBrush b = new SolidBrush(mau))
            using (StringFormat c = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                g.DrawString(icon, fi, b, o, c);
            float x = o.Right + 14, w = r.Right - x - 10;
            using (Font ft = new Font("Segoe UI", 8.2F, FontStyle.Bold))
            using (Font fv = new Font("Segoe UI", 20F, FontStyle.Bold))
            using (Font fm = new Font("Segoe UI", 8.4F))
            using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
            using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
            using (StringFormat sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                float y = r.Top + (r.Height - 70) / 2;
                g.DrawString(tieuDe, ft, sub, new RectangleF(x, y, w, 18), sf);
                g.DrawString(giaTri, fv, ink, new RectangleF(x - 2, y + 15, w, 36), sf);
                g.DrawString(moTa, fm, sub, new RectangleF(x, y + 51, w, 18), sf);
            }
        }
    }

    /// <summary>Khung trang bo goc vien mong (chua nhom nut / o nhap).</summary>
    internal sealed class KhungBoGocChung : Panel
    {
        public int BanKinh { get; set; }

        public KhungBoGocChung()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BanKinh = 11;
            BackColor = Color.White;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Color nenCha = Parent != null ? Parent.BackColor : TienIchGiaoDienSang.Background;
            e.Graphics.Clear(nenCha);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f), BanKinh))
            using (SolidBrush b = new SolidBrush(BackColor))
            using (Pen pen = new Pen(TienIchGiaoDienSang.Border))
            {
                e.Graphics.FillPath(b, p);
                e.Graphics.DrawPath(pen, p);
            }
        }
    }

    /// <summary>Bang du lieu phong cach v2 (dong cao, tieu de nhat, khong vien doc).</summary>
    internal static class BangGiaoDien
    {
        public static DataGridView Tao()
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
            g.RowTemplate.Height = 34;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.ForeColor = TienIchGiaoDienSang.Muted;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.6F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9.1F);
            g.DefaultCellStyle.ForeColor = TienIchGiaoDienSang.Ink;
            g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 246, 241);
            g.DefaultCellStyle.SelectionForeColor = TienIchGiaoDienSang.Ink;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 254);
            return g;
        }

        public static DataGridViewTextBoxColumn Cot(DataGridView g, string tieuDe, string duLieu, float tyLe,
            bool phai = false, bool dam = false)
        {
            DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn
            {
                HeaderText = tieuDe,
                DataPropertyName = duLieu,
                Name = "col" + (duLieu ?? tieuDe),
                FillWeight = tyLe,
                ReadOnly = true,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            if (phai)
            {
                c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            if (dam) c.DefaultCellStyle.Font = new Font("Segoe UI", 9.1F, FontStyle.Bold);
            g.Columns.Add(c);
            return c;
        }

        /// <summary>Ve o dang "nhan tron" (pill) cho mot cot; mau theo gia tri.</summary>
        public static void VeNhanTron(DataGridView g, int cot, Func<string, Tuple<Color, Color>> mau)
        {
            g.CellPainting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != cot) return;
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                string chu = Convert.ToString(e.FormattedValue);
                if (string.IsNullOrEmpty(chu)) { e.Handled = true; return; }
                Tuple<Color, Color> m = mau(chu);
                using (Font f = new Font("Segoe UI", 8.2F, FontStyle.Bold))
                {
                    Size kt = TextRenderer.MeasureText(chu, f);
                    RectangleF r = new RectangleF(e.CellBounds.Left + 8, e.CellBounds.Top + (e.CellBounds.Height - 22) / 2f,
                        Math.Min(e.CellBounds.Width - 14, kt.Width + 14), 22);
                    GraphicsState truoc = e.Graphics.Save();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SolidBrush b = new SolidBrush(m.Item1))
                    using (GraphicsPath p = VeBaoCao.BoTron(r, 11)) e.Graphics.FillPath(b, p);
                    e.Graphics.Restore(truoc);
                    TextRenderer.DrawText(e.Graphics, chu, f, Rectangle.Round(r), m.Item2,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }
                e.Handled = true;
            };
        }

        public static readonly Color XanhNhat = Color.FromArgb(226, 246, 241);
        public static readonly Color DoNhat = Color.FromArgb(253, 232, 236);
        public static readonly Color Do = Color.FromArgb(204, 70, 90);
        public static readonly Color CamNhat = Color.FromArgb(253, 241, 225);
        public static readonly Color Cam = Color.FromArgb(209, 127, 35);
        public static readonly Color XanhDuongNhat = Color.FromArgb(230, 241, 252);
    }

    internal static class NutGiaoDien
    {
        public static Button Phang(string chu, Color nen, Color mauChu, float co)
        {
            Button b = new Button
            {
                Text = chu,
                BackColor = nen,
                ForeColor = mauChu,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", co, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                UseMnemonic = false   // "&" hien dung (Phan quyen & duyet...)
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(nen, 0.12f);
            return b;
        }

        public static void DoiMau(Button b, Color nen, Color chu)
        {
            b.BackColor = nen;
            b.ForeColor = chu;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(nen, 0.12f);
        }

        /// <summary>Bo tron theo Region (chip / nut vien thuoc).</summary>
        public static void BoTron(Control c, int banKinh)
        {
            Action ap = () =>
            {
                if (c.Width < 2 || c.Height < 2) return;
                using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(0, 0, c.Width, c.Height), banKinh))
                {
                    Region cu = c.Region;
                    c.Region = new Region(p);
                    if (cu != null) cu.Dispose();
                }
            };
            c.SizeChanged += (s, e) => ap();
            ap();
        }
    }
}
