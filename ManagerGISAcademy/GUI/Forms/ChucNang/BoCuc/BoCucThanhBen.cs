
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Forms
{
    // Giao dien thanh dieu huong.
    // .NET Framework 4.8 / C# 7.3.
    //
    // Khong override OnHandleCreated,
    // OnActivated hoac OnVisibleChanged.
    //
    // Goi Sidebar_KhoiTao() sau
    // UiKhoiTaoGiaoDien() trong
    // FormMain.ToiUuBanDo.cs.

    public partial class FormMain
    {
        private readonly Color sidebarNenMoi =
            Color.FromArgb(14, 49, 40);

        private readonly Color sidebarNenDuoi =
            Color.FromArgb(9, 36, 32);

        private readonly Color sidebarChuMoi =
            Color.FromArgb(207, 236, 225);

        private readonly Font sidebarFontMoi =
            new Font("Segoe UI", 9.5F);

        private readonly Font sidebarFontDamMoi =
            new Font(
                "Segoe UI",
                9.5F,
                FontStyle.Bold);

        private readonly Font sidebarFontNhoMoi =
            new Font(
                "Segoe UI",
                8.0F,
                FontStyle.Bold);

        private readonly Dictionary<Button, float>
            sidebarHoverMoi =
            new Dictionary<Button, float>();

        private readonly HashSet<Button>
            sidebarDangHoverMoi =
            new HashSet<Button>();

        private bool sidebarKhoiTaoMoi;

        private Label sidebarNhanMenuMoi;

        private Timer sidebarTimerMoi;

        private SidebarPostgisMoi sidebarTrangThaiMoi;

        // ==========================================
        // 1. LAY BAY NUT DIEU HUONG
        // ==========================================

        private Button[] Sidebar_CacNutMoi()
        {
            return new Button[]
            {
                btnNavOverview,
                btnNavMap,
                btnNavSearch,
                btnNavManage,
                btnNavAnalysis,
                btnNavReports,
                btnNavValidate,
                btnNavAdmin
            };
        }

        // ==========================================
        // 2. KHOI TAO GIAO DIEN SIDEBAR
        // ==========================================

        private void Sidebar_KhoiTao()
        {
            if (sidebarKhoiTaoMoi ||
                IsDisposed ||
                Disposing ||
                pnlSidebar == null ||
                flowNavigation == null)
            {
                return;
            }

            sidebarKhoiTaoMoi = true;

            pnlSidebar.SuspendLayout();
            flowNavigation.SuspendLayout();

            try
            {
                pnlSidebar.BackColor =
                    sidebarNenMoi;

                pnlSidebarTop.BackColor =
                    sidebarNenMoi;

                flowNavigation.BackColor =
                    sidebarNenMoi;

                pnlSidebarBottom.BackColor =
                    sidebarNenDuoi;

                // Giu lai logo cua he thong.

                if (pnlBrand != null)
                {
                    pnlBrand.BackColor =
                        sidebarNenMoi;
                }

                // Them nhan dieu huong vao
                // FlowLayoutPanel, khong che logo.

                sidebarNhanMenuMoi = new Label
                {
                    Name = "sidebarNhanMenuMoi",

                    Text =
                        "ĐIỀU HƯỚNG      •      08 PHÂN HỆ",

                    Height = 29,

                    TextAlign =
                        ContentAlignment.MiddleLeft,

                    Font = sidebarFontNhoMoi,

                    ForeColor =
                        Color.FromArgb(
                            115, 211, 170),

                    BackColor = sidebarNenMoi,

                    AutoEllipsis = true,

                    Margin =
                        new Padding(0, 0, 0, 7)
                };

                flowNavigation.Controls.Add(
                    sidebarNhanMenuMoi);

                flowNavigation.Controls.SetChildIndex(
                    sidebarNhanMenuMoi,
                    0);

                // Timer hieu ung hover.

                sidebarTimerMoi = new Timer
                {
                    Interval = 35
                };

                sidebarTimerMoi.Tick +=
                    Sidebar_TickMoi;

                // Trang tri cac nut da co san.

                foreach (Button b in
                    Sidebar_CacNutMoi())
                {
                    if (b == null ||
                        b.IsDisposed)
                    {
                        continue;
                    }

                    sidebarHoverMoi[b] = 0f;

                    b.FlatStyle =
                        FlatStyle.Flat;

                    b.FlatAppearance.BorderSize = 0;

                    b.UseVisualStyleBackColor = false;

                    b.Cursor = Cursors.Hand;

                    // UiCanMenu() van co the
                    // gan Height = 43.
                    //
                    // MinimumSize giu nut 50px
                    // ma khong tao vong lap Resize.

                    b.MinimumSize =
                        new Size(0, 50);

                    b.Height = 50;

                    b.Paint +=
                        Sidebar_VeNutMoi;

                    b.MouseEnter +=
                        Sidebar_ChuotVaoMoi;

                    b.MouseLeave +=
                        Sidebar_ChuotRaMoi;

                    b.Click +=
                        Sidebar_ChonNutMoi;
                }

                // The PostGIS duoi cung.
                // Khong thay the trang thai C#.

                pnlSidebarBottom.Height = 94;

                pnlSidebarBottom.Padding =
                    new Padding(12, 7, 12, 11);

                sidebarTrangThaiMoi =
                    new SidebarPostgisMoi();

                sidebarTrangThaiMoi.Dock =
                    DockStyle.Fill;

                pnlSidebarBottom.Controls.Add(
                    sidebarTrangThaiMoi);

                sidebarTrangThaiMoi.BringToFront();

                pnlSidebarBottom.Visible =
                    pnlSidebar.Width >= 210;

                if (lblHeaderDbStatus != null)
                {
                    lblHeaderDbStatus.TextChanged +=
                        Sidebar_DongBoDbMoi;

                    Sidebar_DongBoDbMoi(
                        null,
                        EventArgs.Empty);
                }

                pnlSidebar.Resize +=
                    Sidebar_CanLaiMoi;

                flowNavigation.Resize +=
                    Sidebar_CanLaiMoi;

                FormClosed +=
                    Sidebar_GiaiPhongMoi;

                Sidebar_DieuChinhMoi();
            }
            finally
            {
                flowNavigation.ResumeLayout(true);

                pnlSidebar.ResumeLayout(true);
            }

            Sidebar_VeLaiMoi();
        }

        // ==========================================
        // 3. CAN CHINH KHI THU GON SIDEBAR
        // ==========================================

        private void Sidebar_DieuChinhMoi()
        {
            if (!sidebarKhoiTaoMoi ||
                pnlSidebar == null ||
                pnlSidebar.IsDisposed ||
                flowNavigation == null ||
                flowNavigation.IsDisposed)
            {
                return;
            }

            bool gon =
                pnlSidebar.Width < 210;

            if (sidebarNhanMenuMoi != null &&
                !sidebarNhanMenuMoi.IsDisposed)
            {
                sidebarNhanMenuMoi.Visible =
                    !gon;

                int width =
                    flowNavigation.ClientSize.Width -
                    flowNavigation.Padding.Horizontal - 3;

                sidebarNhanMenuMoi.Width =
                    Math.Max(20, width);
            }

            // UiCanSidebarTheoChieuRong cua project
            // tu quan ly Visible cua phan duoi.

            Sidebar_VeLaiMoi();
        }

        private void Sidebar_CanLaiMoi(
            object sender,
            EventArgs e)
        {
            Sidebar_DieuChinhMoi();
        }

        // ==========================================
        // 4. DONG BO TRANG THAI POSTGIS
        // ==========================================

        private void Sidebar_DongBoDbMoi(
            object sender,
            EventArgs e)
        {
            if (sidebarTrangThaiMoi == null ||
                sidebarTrangThaiMoi.IsDisposed)
            {
                return;
            }

            string s =
                lblHeaderDbStatus == null
                    ? ""
                    : lblHeaderDbStatus.Text;

            sidebarTrangThaiMoi.DaKetNoi =
                s.IndexOf(
                    "Đã kết nối",
                    StringComparison.OrdinalIgnoreCase
                ) >= 0 &&

                s.IndexOf(
                    "Không",
                    StringComparison.OrdinalIgnoreCase
                ) < 0;
        }

        // ==========================================
        // 5. HIEU UNG CHUOT
        // ==========================================

        private void Sidebar_ChuotVaoMoi(
            object sender,
            EventArgs e)
        {
            Button b =
                sender as Button;

            if (b == null)
                return;

            sidebarDangHoverMoi.Add(b);

            if (sidebarTimerMoi != null)
            {
                sidebarTimerMoi.Start();
            }
        }

        private void Sidebar_ChuotRaMoi(
            object sender,
            EventArgs e)
        {
            Button b =
                sender as Button;

            if (b == null)
                return;

            sidebarDangHoverMoi.Remove(b);

            if (sidebarTimerMoi != null)
            {
                sidebarTimerMoi.Start();
            }
        }

        private void Sidebar_TickMoi(
            object sender,
            EventArgs e)
        {
            bool canChay = false;

            foreach (Button b in
                Sidebar_CacNutMoi())
            {
                if (b == null ||
                    b.IsDisposed ||
                    !sidebarHoverMoi.ContainsKey(b))
                {
                    continue;
                }

                float old =
                    sidebarHoverMoi[b];

                float target =
                    sidebarDangHoverMoi.Contains(b)
                        ? 1f
                        : 0f;

                float next =
                    old + (target - old) * .32f;

                if (Math.Abs(next - target) < .02f)
                {
                    next = target;
                }

                if (next != old)
                {
                    sidebarHoverMoi[b] = next;

                    b.Invalidate();
                }

                if (next != target)
                {
                    canChay = true;
                }
            }

            if (!canChay &&
                sidebarTimerMoi != null)
            {
                sidebarTimerMoi.Stop();
            }
        }

        // ==========================================
        // 6. CAP NHAT NUT DANG CHON
        // ==========================================

        private void Sidebar_ChonNutMoi(
            object sender,
            EventArgs e)
        {
            if (IsDisposed ||
                Disposing ||
                !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(
                (MethodInvoker)
                    Sidebar_VeLaiMoi);
        }

        private void Sidebar_VeLaiMoi()
        {
            foreach (Button b in
                Sidebar_CacNutMoi())
            {
                if (b != null &&
                    !b.IsDisposed)
                {
                    b.Invalidate();
                }
            }
        }

        // ==========================================
        // 7. HAM VE BO GOC
        // ==========================================

        private static GraphicsPath
            Sidebar_BoGocMoi(
                Rectangle r,
                int radius)
        {
            GraphicsPath path =
                new GraphicsPath();

            if (r.Width <= 1 ||
                r.Height <= 1)
            {
                path.AddRectangle(r);

                return path;
            }

            int d = Math.Max(
                2,
                Math.Min(
                    radius * 2,
                    Math.Min(
                        r.Width,
                        r.Height)));

            path.AddArc(
                r.Left,
                r.Top,
                d,
                d,
                180,
                90);

            path.AddArc(
                r.Right - d,
                r.Top,
                d,
                d,
                270,
                90);

            path.AddArc(
                r.Right - d,
                r.Bottom - d,
                d,
                d,
                0,
                90);

            path.AddArc(
                r.Left,
                r.Bottom - d,
                d,
                d,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        // ==========================================
        // 8. VE CAC NUT DIEU HUONG
        // ==========================================

        private void Sidebar_VeNutMoi(
            object sender,
            PaintEventArgs e)
        {
            Button b =
                sender as Button;

            if (b == null ||
                b.Width < 20 ||
                b.Height < 20)
            {
                return;
            }

            Button[] buttons =
                Sidebar_CacNutMoi();

            int index =
                Array.IndexOf(buttons, b);

            if (index < 0)
                return;

            string[] labels =
            {
                "Tổng quan",
                "Bản đồ GIS",
                "Tra cứu đơn vị",
                "Quản lý dữ liệu",
                "Phân tích không gian",
                "Thống kê, báo cáo",
                "Kiểm tra dữ liệu",
                "Quản trị hệ thống"
            };

            bool selected =
                b == nutDangChon;

            bool collapsed =
                pnlSidebar.Width < 210;

            float hover =
                sidebarHoverMoi.ContainsKey(b)
                    ? sidebarHoverMoi[b]
                    : 0f;

            Graphics g =
                e.Graphics;

            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            g.TextRenderingHint =
                System.Drawing.Text
                    .TextRenderingHint
                    .ClearTypeGridFit;

            // Nen dac tranh chu va icon
            // mac dinh ve chong len giao dien.

            g.Clear(sidebarNenMoi);

            Rectangle surface =
                new Rectangle(
                    1,
                    1,
                    b.Width - 3,
                    b.Height - 3);

            using (GraphicsPath shape =
                Sidebar_BoGocMoi(
                    surface, 12))
            {
                if (selected)
                {
                    using (Brush fill =
                        new LinearGradientBrush(
                            surface,

                            Color.FromArgb(
                                3, 174, 139),

                            Color.FromArgb(
                                38, 213, 170),

                            LinearGradientMode.Horizontal))
                    {
                        g.FillPath(fill, shape);
                    }

                    using (Pen border =
                        new Pen(
                            Color.FromArgb(
                                95, 226, 255, 220)))
                    {
                        g.DrawPath(border, shape);
                    }
                }
                else if (hover > 0)
                {
                    using (Brush fill =
                        new SolidBrush(
                            Color.FromArgb(
                                (int)(45 * hover),
                                91, 223, 179)))
                    {
                        g.FillPath(fill, shape);
                    }
                }
            }

            if (selected)
            {
                using (Brush mark =
                    new SolidBrush(
                        Color.FromArgb(
                            216, 255, 126)))
                {
                    g.FillRectangle(
                        mark,
                        2,
                        12,
                        4,
                        Math.Max(
                            3,
                            b.Height - 24));
                }
            }

            // Ve nen bieu tuong.

            int box =
                collapsed ? 36 : 33;

            int bx =
                collapsed
                    ? Math.Max(
                        2,
                        (b.Width - box) / 2)
                    : 13;

            int by =
                Math.Max(
                    1,
                    (b.Height - box) / 2);

            Rectangle tile =
                new Rectangle(
                    bx,
                    by,
                    box,
                    box);

            using (GraphicsPath path =
                Sidebar_BoGocMoi(
                    tile, 9))
            using (Brush fill =
                new SolidBrush(
                    selected
                        ? Color.FromArgb(
                            58, Color.White)
                        : Color.FromArgb(
                            36 + (int)(22 * hover),
                            103, 213, 171)))
            {
                g.FillPath(fill, path);
            }

            // Dung lai icon cua project.

            if (b.Image != null)
            {
                int iconSize =
                    Math.Min(
                        22,
                        box - 7);

                Rectangle rect =
                    new Rectangle(
                        bx + (box - iconSize) / 2,

                        by + (box - iconSize) / 2,

                        iconSize,
                        iconSize);

                g.DrawImage(
                    b.Image,
                    rect);
            }

            // Ve ten phan he khi sidebar mo.

            if (!collapsed)
            {
                RectangleF tx =
                    new RectangleF(
                        bx + box + 12,
                        0,

                        Math.Max(
                            10,
                            b.Width - bx - box - 39),

                        b.Height);

                using (StringFormat sf =
                    new StringFormat())
                using (Brush brush =
                    new SolidBrush(
                        selected
                            ? Color.White
                            : sidebarChuMoi))
                {
                    sf.Alignment =
                        StringAlignment.Near;

                    sf.LineAlignment =
                        StringAlignment.Center;

                    sf.Trimming =
                        StringTrimming.EllipsisCharacter;

                    sf.FormatFlags =
                        StringFormatFlags.NoWrap;

                    g.DrawString(
                        labels[index],

                        selected
                            ? sidebarFontDamMoi
                            : sidebarFontMoi,

                        brush,
                        tx,
                        sf);
                }

                if (selected)
                {
                    using (Brush dot =
                        new SolidBrush(
                            Color.FromArgb(
                                215, 255, 184)))
                    {
                        g.FillEllipse(
                            dot,
                            b.Width - 18,
                            b.Height / 2 - 3,
                            6,
                            6);
                    }
                }
            }
        }

        // ==========================================
        // 9. GIAI PHONG TAI NGUYEN
        // ==========================================

        private void Sidebar_GiaiPhongMoi(
            object sender,
            FormClosedEventArgs e)
        {
            if (sidebarTimerMoi != null)
            {
                sidebarTimerMoi.Stop();

                sidebarTimerMoi.Dispose();

                sidebarTimerMoi = null;
            }

            if (lblHeaderDbStatus != null &&
                !lblHeaderDbStatus.IsDisposed)
            {
                lblHeaderDbStatus.TextChanged -=
                    Sidebar_DongBoDbMoi;
            }

            sidebarFontMoi.Dispose();

            sidebarFontDamMoi.Dispose();

            sidebarFontNhoMoi.Dispose();
        }

        // ==========================================
        // 10. THE TRANG THAI POSTGIS
        // ==========================================

        private sealed class SidebarPostgisMoi :
            Control
        {
            private bool ketNoi;

            public bool DaKetNoi
            {
                get
                {
                    return ketNoi;
                }

                set
                {
                    if (ketNoi == value)
                        return;

                    ketNoi = value;

                    Invalidate();
                }
            }

            public SidebarPostgisMoi()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint |
                    ControlStyles.ResizeRedraw,
                    true);

                BackColor =
                    Color.FromArgb(9, 36, 32);
            }

            protected override void OnPaint(
                PaintEventArgs e)
            {
                base.OnPaint(e);

                if (Width < 25 ||
                    Height < 25)
                {
                    return;
                }

                Graphics g =
                    e.Graphics;

                g.SmoothingMode =
                    SmoothingMode.AntiAlias;

                Rectangle r =
                    new Rectangle(
                        1,
                        1,
                        Width - 3,
                        Height - 3);

                using (GraphicsPath path =
                    Sidebar_BoGocMoi(
                        r, 12))
                using (Brush fill =
                    new LinearGradientBrush(
                        r,

                        Color.FromArgb(
                            26, 87, 67),

                        Color.FromArgb(
                            18, 59, 52),

                        LinearGradientMode.Horizontal))
                using (Pen edge =
                    new Pen(
                        Color.FromArgb(
                            56, 151, 113)))
                {
                    g.FillPath(
                        fill,
                        path);

                    g.DrawPath(
                        edge,
                        path);
                }

                using (Font head =
                    new Font(
                        "Segoe UI",
                        8.3F,
                        FontStyle.Bold))
                using (Font sub =
                    new Font(
                        "Segoe UI",
                        7.6F))
                {
                    TextRenderer.DrawText(
                        g,

                        "CƠ SỞ DỮ LIỆU POSTGIS",

                        head,

                        new Rectangle(
                            13,
                            10,
                            Width - 39,
                            18),

                        Color.White,

                        TextFormatFlags.Left |
                        TextFormatFlags.EndEllipsis |
                        TextFormatFlags.VerticalCenter);

                    TextRenderer.DrawText(
                        g,

                        ketNoi
                            ? "● Đã kết nối · EPSG:32648"
                            : "● Chưa kết nối",

                        sub,

                        new Rectangle(
                            13,
                            29,
                            Width - 27,
                            18),

                        ketNoi
                            ? Color.FromArgb(
                                95, 245, 183)
                            : Color.FromArgb(
                                255, 191, 117),

                        TextFormatFlags.Left |
                        TextFormatFlags.EndEllipsis |
                        TextFormatFlags.VerticalCenter);
                }
            }
        }
    }
}
