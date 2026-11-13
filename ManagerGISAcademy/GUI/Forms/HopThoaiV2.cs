using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Khung hop thoai phong cach v2: dai tieu de gradient, than (Than),
    /// chan (ChanNut) chua nut. Cac hop thoai GIS moi ke thua lop nay.
    /// </summary>
    public class HopThoaiV2 : Form
    {
        protected readonly Panel Than;
        protected readonly FlowLayoutPanel ChanNut;
        private readonly DaiTieuDe dai;

        public HopThoaiV2(string tieuDe, string phuDe, Size kichThuoc)
        {
            Text = tieuDe;
            Font = new Font("Segoe UI", 9F);
            BackColor = TienIchGiaoDienSang.Background;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            ClientSize = kichThuoc;
            MinimumSize = new Size(Math.Min(640, kichThuoc.Width), Math.Min(420, kichThuoc.Height));
            DoubleBuffered = true;

            dai = new DaiTieuDe { Dock = DockStyle.Top, Height = 70, TieuDe = tieuDe, PhuDe = phuDe };
            Than = new Panel { Dock = DockStyle.Fill, BackColor = TienIchGiaoDienSang.Background, Padding = new Padding(16, 12, 16, 6) };
            ChanNut = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 58,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(16, 11, 16, 8)
            };
            ChanNut.Paint += (s, e) =>
            {
                using (Pen p = new Pen(TienIchGiaoDienSang.Border)) e.Graphics.DrawLine(p, 0, 0, ChanNut.Width, 0);
            };
            Controls.Add(Than);
            Controls.Add(ChanNut);
            Controls.Add(dai);
        }

        protected void DatPhuDe(string s) { dai.PhuDe = s; }

        /// <summary>Them nut vao chan (tu phai sang trai theo thu tu goi).</summary>
        protected Button ThemNut(string chu, Color nen, Color mauChu, int rong, EventHandler bam)
        {
            Button b = NutGiaoDien.Phang(chu, nen, mauChu, 9.2F);
            b.Size = new Size(rong, 36);
            b.Margin = new Padding(8, 0, 0, 0);
            if (nen == Color.White)
            {
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            }
            if (bam != null) b.Click += bam;
            ChanNut.Controls.Add(b);
            return b;
        }

        protected static Label Nhan(string chu, float co = 9F, bool dam = false, Color? mau = null)
        {
            Label l = TienIchGiaoDienSang.Text(chu, co, dam, mau);
            l.AutoEllipsis = true;
            return l;
        }

        /// <summary>Nhan dat vi tri tu do (khong Dock) cho bo cuc SetBounds.</summary>
        protected static Label NhanTd(string chu, float co = 9F, bool dam = false, Color? mau = null)
        {
            Label l = Nhan(chu, co, dam, mau);
            l.Dock = DockStyle.None;
            return l;
        }

        private sealed class DaiTieuDe : Control
        {
            private string tieuDe = "", phuDe = "";
            public string TieuDe { get { return tieuDe; } set { tieuDe = value ?? ""; Invalidate(); } }
            public string PhuDe { get { return phuDe; } set { phuDe = value ?? ""; Invalidate(); } }

            public DaiTieuDe()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                Rectangle r = ClientRectangle;
                using (LinearGradientBrush lg = new LinearGradientBrush(r, Color.FromArgb(0, 169, 138), Color.FromArgb(44, 104, 201), LinearGradientMode.Horizontal))
                    g.FillRectangle(lg, r);
                using (SolidBrush vang = new SolidBrush(Color.FromArgb(230, 249, 81)))
                    g.FillRectangle(vang, 0, 14, 4, Math.Max(0, Height - 28));
                using (Font f1 = new Font("Segoe UI", 14F, FontStyle.Bold))
                using (Font f2 = new Font("Segoe UI", 9F))
                using (SolidBrush t = new SolidBrush(Color.White))
                using (SolidBrush t2 = new SolidBrush(Color.FromArgb(225, Color.White)))
                {
                    g.DrawString(tieuDe, f1, t, 18, 10);
                    g.DrawString(phuDe, f2, t2, new RectangleF(20, 42, Width - 30, 22));
                }
            }
        }
    }

    /// <summary>Hop thoai chon nhanh 1 don vi (tim theo ten / ma, khong dau).</summary>
    public sealed class FormChonDonVi : HopThoaiV2
    {
        private readonly TextBox txtTim;
        private readonly ListBox ds;
        private readonly System.Collections.Generic.List<ManagerGISAcademy.DAL.DonViRutGon> tatCa;
        private readonly Font fontDam = new Font("Segoe UI", 10F, FontStyle.Bold);
        private readonly Font fontNho = new Font("Segoe UI", 8.5F);
        public ManagerGISAcademy.DAL.DonViRutGon DaChon { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { fontDam.Dispose(); fontNho.Dispose(); }
            base.Dispose(disposing);
        }

        public FormChonDonVi(string tieuDe, System.Collections.Generic.List<ManagerGISAcademy.DAL.DonViRutGon> danhSach)
            : base(tieuDe, "Gõ tên hoặc mã đơn vị · Enter hoặc nhấp đúp để chọn", new Size(620, 560))
        {
            tatCa = danhSach ?? new System.Collections.Generic.List<ManagerGISAcademy.DAL.DonViRutGon>();
            KhungBoGocChung o = new KhungBoGocChung { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 10, 8, 4) };
            txtTim = new TextBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10F) };
            o.Controls.Add(txtTim);
            Panel cach = new Panel { Dock = DockStyle.Top, Height = 10, BackColor = TienIchGiaoDienSang.Background };
            ds = new ListBox
            {
                Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 10F),
                IntegralHeight = false, DrawMode = DrawMode.OwnerDrawFixed, ItemHeight = 44
            };
            ds.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;
                ManagerGISAcademy.DAL.DonViRutGon d = (ManagerGISAcademy.DAL.DonViRutGon)ds.Items[e.Index];
                bool chon = (e.State & DrawItemState.Selected) != 0;
                using (SolidBrush nen = new SolidBrush(chon ? Color.FromArgb(226, 246, 241) : (e.Index % 2 == 0 ? Color.White : Color.FromArgb(250, 252, 254))))
                    e.Graphics.FillRectangle(nen, e.Bounds);
                TextRenderer.DrawText(e.Graphics, d.Ten, fontDam, new Rectangle(e.Bounds.X + 12, e.Bounds.Y + 3, e.Bounds.Width - 20, 22), TienIchGiaoDienSang.Ink, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(e.Graphics, d.Ma + "   ·   " + d.Loai + "   ·   " + d.DiaBan, fontNho, new Rectangle(e.Bounds.X + 12, e.Bounds.Y + 23, e.Bounds.Width - 20, 18), TienIchGiaoDienSang.Muted, TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            };
            Than.Controls.Add(ds);
            Than.Controls.Add(cach);
            Than.Controls.Add(o);
            ThemNut("Chọn", Views.VeBaoCao.Teal, Color.White, 110, (s, e) => Chon());
            ThemNut("Hủy", Color.White, TienIchGiaoDienSang.Ink, 90, (s, e) => { DialogResult = DialogResult.Cancel; Close(); });
            txtTim.TextChanged += (s, e) => Loc();
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Down && ds.Items.Count > 0) { ds.Focus(); ds.SelectedIndex = 0; e.Handled = true; }
                if (e.KeyCode == Keys.Enter) { Chon(); e.Handled = e.SuppressKeyPress = true; }
            };
            ds.DoubleClick += (s, e) => Chon();
            ds.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) Chon(); };
            Shown += (s, e) => txtTim.Focus();
            Loc();
        }

        private void Loc()
        {
            string k = txtTim.Text.Trim();
            System.Globalization.CompareInfo ci = System.Globalization.CultureInfo.GetCultureInfo("vi-VN").CompareInfo;
            System.Globalization.CompareOptions op = System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreNonSpace;
            ds.BeginUpdate();
            ds.Items.Clear();
            foreach (ManagerGISAcademy.DAL.DonViRutGon d in tatCa)
                if (k.Length == 0 || ci.IndexOf(d.Ten ?? "", k, op) >= 0 || ci.IndexOf(d.Ma ?? "", k, op) >= 0)
                    ds.Items.Add(d);
            ds.EndUpdate();
            if (ds.Items.Count > 0) ds.SelectedIndex = 0;
        }

        private void Chon()
        {
            DaChon = ds.SelectedItem as ManagerGISAcademy.DAL.DonViRutGon;
            if (DaChon == null) return;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
