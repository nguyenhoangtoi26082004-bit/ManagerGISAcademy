using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Nut tai khoan o goc phai (Dang nhap / avatar + menu), phien dang nhap,
    /// va khoa phan he "Quan tri he thong" cho tai khoan Quan tri.
    /// Khach (chua dang nhap) chay voi vai tro gis_xem.
    /// </summary>
    public partial class FormMain
    {
        private TheTaiKhoan tkThe;
        private ContextMenuStrip tkMenu;
        private bool tkDangXuLy;
        private const int TkRong = 206, TkCao = 42;

        /// <summary>Program goi sau khi biet CSDL da co he thong tai khoan (script 89).</summary>
        public void CapNhatTrangThaiTaiKhoan()
        {
            if (IsDisposed) return;
            if (InvokeRequired) { BeginInvoke(new MethodInvoker(CapNhatTrangThaiTaiKhoan)); return; }
            TaiKhoan_KhoiTao();
            if (tkThe == null) return;
            tkThe.Visible = PhienDangNhap.HeThongSanSang;
            tkThe.Gan(PhienDangNhap.HienTai);
            TaiKhoan_CanViTri();
            if (PhienDangNhap.HeThongSanSang && !PhienDangNhap.DaDangNhap)
                statusGeneral.Text = "● Đang xem với quyền KHÁCH (chỉ xem). Đăng nhập ở góc phải để làm việc theo vai trò.";
        }

        private void TaiKhoan_KhoiTao()
        {
            if (tkThe != null && !tkThe.IsDisposed) return;
            tkThe = new TheTaiKhoan { Size = new Size(TkRong, TkCao), Visible = false };
            tkThe.Click += (s, e) =>
            {
                if (!PhienDangNhap.DaDangNhap) TaiKhoan_MoDangNhap(false);
                else TaiKhoan_HienMenu();
            };
            Controls.Add(tkThe);
            Resize += (s, e) => TaiKhoan_CanViTri();
            SizeChanged += (s, e) => TaiKhoan_CanViTri();
            PhienDangNhap.DaThayDoi += () =>
            {
                if (IsDisposed) return;
                if (InvokeRequired) BeginInvoke(new MethodInvoker(() => tkThe.Gan(PhienDangNhap.HienTai)));
                else tkThe.Gan(PhienDangNhap.HienTai);
            };
        }

        /// <summary>
        /// Ban do: nut nam tren map canvas, ben phai nut "Them don vi" (UiCanHeaderNoiTrenBanDo chua cho).
        /// Trang nghiep vu: nut noi tren form, goc phai tren cua vung noi dung (trong dai banner).
        /// </summary>
        private void TaiKhoan_CanViTri()
        {
            if (tkThe == null || tkThe.IsDisposed || !tkThe.Visible) { BannerTrang.ChoTrongPhai = 0; return; }
            try
            {
                bool banDo = phanHeHienTai == "Bản đồ GIS" && pnlMapCanvas != null && !pnlMapCanvas.IsDisposed && pnlMapCanvas.Visible;
                if (banDo)
                {
                    if (tkThe.Parent != pnlMapCanvas) pnlMapCanvas.Controls.Add(tkThe);
                    tkThe.SetBounds(Math.Max(15, pnlMapCanvas.ClientSize.Width - 15 - TkRong), 11, TkRong, TkCao);
                    BannerTrang.ChoTrongPhai = 0;
                }
                else
                {
                    if (tkThe.Parent != this) Controls.Add(tkThe);
                    Control vung = tblContent != null && !tblContent.IsDisposed ? (Control)tblContent : this;
                    Point goc = PointToClient(vung.PointToScreen(Point.Empty));
                    tkThe.SetBounds(goc.X + vung.Width - TkRong - 44, goc.Y + 36, TkRong, TkCao);
                    BannerTrang.ChoTrongPhai = TkRong + 24;
                }
                tkThe.BringToFront();
            }
            catch (Exception ex) { GhiNhatKyBanDo("TAI_KHOAN_VI_TRI | " + ex.Message); }
        }

        private void TaiKhoan_MoDangNhap(bool dangKy, Action sauKhiQuanTri = null)
        {
            if (tkDangXuLy) return;
            using (FormDangNhap f = new FormDangNhap(dangKy))
            {
                if (f.ShowDialog(this) != DialogResult.OK || f.TaiKhoan == null) return;
                TaiKhoan_ApDung(f.TaiKhoan, sauKhiQuanTri);
            }
        }

        /// <summary>Gan phien + SET ROLE theo vai tro tai khoan, nap lai quyen va du lieu.</summary>
        private void TaiKhoan_ApDung(TaiKhoanDangNhap tk, Action sauKhiQuanTri)
        {
            PhienDangNhap.Gan(tk);
            QuanTri_DoiVaiTro(tk.VaiTro);
            string chao = "● Xin chào " + tk.HoTen + " · vai trò " + tk.TenVaiTro +
                (tk.LanDangNhapTruoc.HasValue ? " · lần trước: " + tk.LanDangNhapTruoc.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "");
            BeginInvoke(new MethodInvoker(() =>
            {
                statusGeneral.Text = chao;
                if (tk.LaQuanTri && sauKhiQuanTri != null) sauKhiQuanTri();
                if (phanHeHienTai == "Quản trị hệ thống" && !tk.LaQuanTri) ChonPhanHe(btnNavMap, "Bản đồ GIS");
            }));
            GhiNhatKyBanDo("TAI_KHOAN_DANG_NHAP | " + tk.TenDangNhap + " | " + tk.VaiTro);
        }

        private async void TaiKhoan_DangXuat()
        {
            TaiKhoanDangNhap tk = PhienDangNhap.HienTai;
            if (tk == null) return;
            if (MessageBox.Show(this, "Đăng xuất tài khoản " + tk.TenDangNhap + "?\nỨng dụng sẽ chuyển về quyền khách (chỉ xem).",
                    "Đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            tkDangXuLy = true;
            try
            {
                try { await Task.Run(() => new TruyXuatTaiKhoan().DangXuat(tk.TenDangNhap)); }
                catch (Exception ex) { GhiNhatKyBanDo("TAI_KHOAN_DANG_XUAT_LOG | " + ex.Message); }
                PhienDangNhap.Gan(null);
                if (phanHeHienTai == "Quản trị hệ thống") ChonPhanHe(btnNavMap, "Bản đồ GIS");
                QuanTri_DoiVaiTro("gis_xem");
                statusGeneral.Text = "● Đã đăng xuất – đang xem với quyền khách (chỉ xem).";
                GhiNhatKyBanDo("TAI_KHOAN_DANG_XUAT | " + tk.TenDangNhap);
            }
            finally { tkDangXuLy = false; }
        }

        private void TaiKhoan_HienMenu()
        {
            TaiKhoanDangNhap tk = PhienDangNhap.HienTai;
            if (tk == null) return;
            if (tkMenu != null) tkMenu.Dispose();
            tkMenu = new ContextMenuStrip { ShowImageMargin = false, Font = new Font("Segoe UI", 9.5F) };
            ToolStripLabel dau = new ToolStripLabel(tk.HoTen + "\n@" + tk.TenDangNhap + "  ·  " + tk.TenVaiTro +
                (string.IsNullOrEmpty(tk.DonViCongTac) ? "" : "\n" + tk.DonViCongTac))
            { Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(22, 49, 74), Margin = new Padding(8, 8, 8, 6) };
            tkMenu.Items.Add(dau);
            tkMenu.Items.Add(new ToolStripSeparator());
            if (tk.LaQuanTri)
                tkMenu.Items.Add("⚙  Quản trị hệ thống & phân quyền", null, (s, e) => ChonPhanHe(btnNavAdmin, "Quản trị hệ thống"));
            tkMenu.Items.Add("🔑  Đổi mật khẩu…", null, (s, e) => TaiKhoan_DoiMatKhau());
            tkMenu.Items.Add(new ToolStripSeparator());
            ToolStripItem ra = tkMenu.Items.Add("⎋  Đăng xuất", null, (s, e) => TaiKhoan_DangXuat());
            ra.ForeColor = Color.FromArgb(200, 55, 70);
            tkMenu.Show(tkThe, new Point(tkThe.Width - tkMenu.PreferredSize.Width, tkThe.Height + 4));
        }

        private void TaiKhoan_DoiMatKhau()
        {
            TaiKhoanDangNhap tk = PhienDangNhap.HienTai;
            if (tk == null) return;
            string ten = tk.TenDangNhap;
            using (FormMatKhau f = new FormMatKhau("Đổi mật khẩu", "Tài khoản " + ten + " · mật khẩu được băm bcrypt trong CSDL", true,
                       (cu, moi) => new TruyXuatTaiKhoan().DoiMatKhau(ten, cu, moi)))
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                    MessageBox.Show(this, "Đã đổi mật khẩu.", "Đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Chan vao "Quan tri he thong" neu chua dang nhap bang tai khoan Quan tri.
        /// true = duoc vao.
        /// </summary>
        private bool TaiKhoan_KiemTraQuyenQuanTri()
        {
            if (!PhienDangNhap.HeThongSanSang) return true;                 // CSDL cu: giu hanh vi truoc day
            if (PhienDangNhap.DaDangNhap && PhienDangNhap.HienTai.LaQuanTri) return true;
            string ly = PhienDangNhap.DaDangNhap
                ? "Tài khoản " + PhienDangNhap.HienTai.TenDangNhap + " có vai trò " + PhienDangNhap.HienTai.TenVaiTro +
                  ".\nChức năng quản trị hệ thống chỉ dành cho tài khoản QUẢN TRỊ.\n\nĐăng nhập bằng tài khoản khác?"
                : "Quản trị hệ thống (phân quyền, duyệt, sao lưu…) yêu cầu ĐĂNG NHẬP bằng tài khoản Quản trị.\n\nMở màn hình đăng nhập?";
            if (MessageBox.Show(this, ly, "Cần đăng nhập", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                TaiKhoan_MoDangNhap(false, () => ChonPhanHe(btnNavAdmin, "Quản trị hệ thống"));
            return false;
        }

        // =====================================================
        // NUT TAI KHOAN (ve tay)
        // =====================================================
        private sealed class TheTaiKhoan : Control
        {
            private TaiKhoanDangNhap tk;
            private bool diChuot;

            public TheTaiKhoan()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                         ControlStyles.ResizeRedraw, true);
                BackColor = Color.White;
                Cursor = Cursors.Hand;
                Font = new Font("Segoe UI", 9.5F);
            }

            public void Gan(TaiKhoanDangNhap t) { tk = t; Invalidate(); }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                if (Width < 4 || Height < 4) return;
                // Bo goc bang Region: nen trong suot khong hoat dong tren ban do ActiveX.
                using (GraphicsPath p = TienIchIn.BoTron(new RectangleF(0, 0, Width, Height), Height / 2f))
                {
                    Region cu = Region;
                    Region = new Region(p);
                    if (cu != null) cu.Dispose();
                }
            }

            protected override void OnMouseEnter(EventArgs e) { diChuot = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { diChuot = false; Invalidate(); base.OnMouseLeave(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                RectangleF r = new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f);
                Color mauVt = tk == null ? Color.FromArgb(0, 150, 122)
                    : tk.VaiTro == "gis_quan_tri" ? Color.FromArgb(0, 150, 122)
                    : tk.VaiTro == "gis_bien_tap" ? Color.FromArgb(39, 106, 205) : Color.FromArgb(209, 127, 35);
                using (GraphicsPath p = TienIchIn.BoTron(r, r.Height / 2))
                {
                    Color nen = tk == null ? (diChuot ? Color.FromArgb(0, 136, 110) : Color.FromArgb(0, 150, 122))
                                           : (diChuot ? Color.FromArgb(241, 248, 252) : Color.White);
                    using (SolidBrush b = new SolidBrush(nen)) g.FillPath(b, p);
                    using (Pen pen = new Pen(tk == null ? Color.FromArgb(0, 128, 104) : Color.FromArgb(205, 218, 230), 1.1f)) g.DrawPath(pen, p);
                }
                RectangleF tron = new RectangleF(5, 5, Height - 10, Height - 10);
                if (tk == null)
                {
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(50, 255, 255, 255))) g.FillEllipse(b, tron);
                    using (SolidBrush trang = new SolidBrush(Color.White))
                    {
                        float cx = tron.X + tron.Width / 2;
                        g.FillEllipse(trang, cx - 5, tron.Y + 6, 10, 10);
                        g.FillPie(trang, cx - 10, tron.Y + 18, 20, 18, 180, 180);
                    }
                    using (Font f1 = new Font("Segoe UI", 10F, FontStyle.Bold))
                    using (Font f2 = new Font("Segoe UI", 7.6F))
                    {
                        TextRenderer.DrawText(g, "ĐĂNG NHẬP", f1, new Rectangle((int)tron.Right + 8, 4, Width - (int)tron.Right - 16, 20), Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                        TextRenderer.DrawText(g, "Khách · chỉ xem  ·  Đăng ký", f2, new Rectangle((int)tron.Right + 8, 22, Width - (int)tron.Right - 16, 16), Color.FromArgb(220, 255, 255, 255), TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    }
                    return;
                }
                using (SolidBrush b = new SolidBrush(mauVt)) g.FillEllipse(b, tron);
                using (Font fc = new Font("Segoe UI", 9F, FontStyle.Bold))
                    TextRenderer.DrawText(g, tk.ChuVietTat, fc, Rectangle.Round(tron), Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                using (Font f1 = new Font("Segoe UI", 9.4F, FontStyle.Bold))
                using (Font f2 = new Font("Segoe UI", 7.8F, FontStyle.Bold))
                {
                    int x = (int)tron.Right + 8, w = Width - x - 22;
                    TextRenderer.DrawText(g, tk.HoTen, f1, new Rectangle(x, 4, w, 19), Color.FromArgb(22, 49, 74), TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                    TextRenderer.DrawText(g, "● " + tk.TenVaiTro.ToUpperInvariant(), f2, new Rectangle(x, 22, w, 15), mauVt, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
                using (Pen mui = new Pen(Color.FromArgb(120, 140, 160), 1.6f))
                {
                    float mx = Width - 16, my = Height / 2f;
                    g.DrawLines(mui, new[] { new PointF(mx - 4, my - 2), new PointF(mx, my + 2), new PointF(mx + 4, my - 2) });
                }
            }
        }
    }
}
