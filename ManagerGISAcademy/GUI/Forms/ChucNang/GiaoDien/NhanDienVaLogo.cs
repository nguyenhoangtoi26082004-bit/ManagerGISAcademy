using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinImage = System.Drawing.Image;
using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {

        private void UiCanThanhHeader()
        {
            if (uiHeaderDangNoiTrenBanDo ||
                uiDangCanHeader ||
                pnlHeader == null ||
                pnlHeader.IsDisposed ||
                tblHeaderActions == null ||
                tblHeaderActions.IsDisposed ||
                tblHeaderActions.ColumnStyles.Count < 4)
                return;

            uiDangCanHeader = true;

            try
            {
                int w = tblHeaderActions.ClientSize.Width;

                if (w <= 0)
                    w = pnlHeader.ClientSize.Width;

                if (w <= 0)
                    return;

                tblHeaderActions.Padding =
                    new Padding(9, 0, 12, 0);

                const int rongPostGIS = 188;
                const int rongThem = 171;

                int rongTimKiem = Math.Min(
                    345,
                    Math.Max(
                        190,
                        w - rongPostGIS - rongThem - 35));

                tblHeaderActions.SuspendLayout();

                try
                {
                    tblHeaderActions.ColumnStyles[0].SizeType =
                        SizeType.Absolute;

                    tblHeaderActions.ColumnStyles[0].Width =
                        rongTimKiem;

                    tblHeaderActions.ColumnStyles[1].SizeType =
                        SizeType.Percent;

                    tblHeaderActions.ColumnStyles[1].Width = 100F;

                    tblHeaderActions.ColumnStyles[2].SizeType =
                        SizeType.Absolute;

                    tblHeaderActions.ColumnStyles[2].Width =
                        rongPostGIS;

                    tblHeaderActions.ColumnStyles[3].SizeType =
                        SizeType.Absolute;

                    tblHeaderActions.ColumnStyles[3].Width =
                        rongThem;

                    if (uiHopTimKiem != null &&
                        !uiHopTimKiem.IsDisposed)
                    {
                        uiHopTimKiem.Dock = DockStyle.Fill;
                        uiHopTimKiem.Margin =
                            new Padding(0, 15, 0, 15);
                    }
                }
                finally
                {
                    tblHeaderActions.ResumeLayout(true);
                }

                UiCanHopTimMoi();
                UiCanChuTieuDe();
            }
            finally
            {
                uiDangCanHeader = false;
            }
        }
        private void UiCanChuTieuDe()
        {
            if (pnlBrand == null ||
                pnlBrand.IsDisposed ||
                lblBrandTitle == null ||
                lblBrandTitle.IsDisposed)
                return;

            bool gon = pnlSidebar.Width < 210;
            int kichThuocLogo = gon ? 40 : 68;

            int chieuCaoHeader =
                pnlHeader != null ? pnlHeader.Height : 72;

            int yLogo = Math.Max(
                0,
                (chieuCaoHeader - kichThuocLogo) / 2);

            int xTen = gon ? 55 : 92;

            int rongTen = Math.Max(
                120,
                pnlBrand.ClientSize.Width - xTen -
                (gon ? 6 : 28));

            lblBrandTitle.AutoSize = false;

            lblBrandTitle.Text =
                "HỆ THỐNG QUẢN LÝ\r\n" +
                "ĐƠN VỊ QUÂN ĐỘI\r\n" +
                "TRÊN ĐỊA BÀN HÀ NỘI";

            lblBrandTitle.Font = uiFontTenHeThong;
            lblBrandTitle.ForeColor = Color.White;
            lblBrandTitle.BackColor = Color.Transparent;
            lblBrandTitle.TextAlign =
                ContentAlignment.MiddleLeft;
            lblBrandTitle.AutoEllipsis = false;

            lblBrandTitle.SetBounds(
                xTen, 2, rongTen, 68);

            lblBrandTitle.Visible = !gon;

            if (!gon)
                lblBrandTitle.BringToFront();

            lblBrandSubtitle.Text = string.Empty;
            lblBrandSubtitle.Visible = false;
            lblSidebarTitle.Visible = false;
            lblSidebarHint.Visible = false;

            if (uiLogo != null && !uiLogo.IsDisposed)
            {
                uiLogo.SetBounds(
                    gon ? 7 : 10,
                    yLogo,
                    kichThuocLogo,
                    kichThuocLogo);

                uiLogo.BringToFront();
            }

            if (lblBrandLogo != null &&
                !lblBrandLogo.IsDisposed)
            {
                lblBrandLogo.SetBounds(
                    gon ? 7 : 10,
                    yLogo,
                    kichThuocLogo,
                    kichThuocLogo);
            }
        }

        private void UiVeNenThuongHieu(
            object sender,
            PaintEventArgs e)
        {
            Panel p = sender as Panel;

            if (p == null || p.Width < 1 || p.Height < 1)
                return;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (var nen = new LinearGradientBrush(
                p.ClientRectangle,
                Color.FromArgb(14, 74, 55),
                uiThanh,
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(
                    nen, p.ClientRectangle);
            }

            using (var song = new Pen(
                Color.FromArgb(85, 42, 124, 94), 1.5F))
            {
                e.Graphics.DrawArc(
                    song,
                    -38,
                    p.Height - 61,
                    p.Width + 80,
                    76,
                    2,
                    176);
            }

            using (var gach = new Pen(
                Color.FromArgb(92, 146, 195, 160), 1F))
            {
                e.Graphics.DrawLine(
                    gach, 84, 8, 84, p.Height - 8);

                e.Graphics.DrawLine(
                    gach,
                    14,
                    p.Height - 1,
                    p.Width - 14,
                    p.Height - 1);
            }
        }

        private void UiTaoLogo()
        {
            if (uiLogo != null && !uiLogo.IsDisposed)
                return;

            uiLogo = new UiLogoSacNet
            {
                Name = "uiLogo",
                Size = new Size(68, 68),
                Location = new WinPoint(10, 2),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            string root =
                AppDomain.CurrentDomain.BaseDirectory;

            string[] thu =
            {
                Path.Combine(
                    root, "Resources", "Images", "logohocvien.png"),

                Path.Combine(
                    root, "Images", "logohocvien.png"),

                Path.GetFullPath(Path.Combine(
                    root, "..", "..",
                    "Resources", "Images", "logohocvien.png")),

                Path.GetFullPath(Path.Combine(
                    root, "..", "..", "..",
                    "Resources", "Images", "logohocvien.png"))
            };

            long soDiemAnhTotNhat = -1;

            foreach (string path in thu)
            {
                try
                {
                    if (!File.Exists(path))
                        continue;

                    using (WinImage img = WinImage.FromFile(path))
                    {
                        long soDiemAnh =
                            (long)img.Width * img.Height;

                        if (soDiemAnh <= soDiemAnhTotNhat)
                            continue;

                        using (Bitmap copy = new Bitmap(img))
                        {
                            Bitmap anhMoi =
                                UiTaoLogoNenTrong(copy);

                            if (uiAnhLogo != null)
                                uiAnhLogo.Dispose();

                            uiAnhLogo = anhMoi;
                            soDiemAnhTotNhat = soDiemAnh;

                            System.Diagnostics.Debug.WriteLine(
                                "[UI_LOGO] Source=" + path +
                                " Size=" + img.Width +
                                "x" + img.Height);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[UI_LOGO] " + ex.Message);
                }
            }

            uiLogo.Image = uiAnhLogo;

            if (soDiemAnhTotNhat > 0 &&
                soDiemAnhTotNhat < 256L * 256L)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[UI_LOGO] Anh nguon qua nho. " +
                    "Nen su dung PNG 512x512 hoac lon hon.");
            }

            pnlBrand.Controls.Add(uiLogo);
            uiLogo.BringToFront();

            uiLogo.Visible = uiAnhLogo != null;
            lblBrandLogo.Visible = uiAnhLogo == null;

            if (uiAnhLogo == null)
            {
                lblBrandLogo.Text = "GIS";
                lblBrandLogo.SetBounds(10, 2, 68, 68);
                lblBrandLogo.BackColor = uiXanh;
                lblBrandLogo.ForeColor = Color.White;
                lblBrandLogo.TextAlign =
                    ContentAlignment.MiddleCenter;

                UiBoGoc(lblBrandLogo, 11);
            }
        }
        private Bitmap UiTaoLogoNenTrong(Bitmap banDau)
        {
            const int kichThuocToiDa = 768;

            double tyLe = Math.Min(
                1.0,
                Math.Min(
                    (double)kichThuocToiDa / banDau.Width,
                    (double)kichThuocToiDa / banDau.Height));

            int w = Math.Max(
                1,
                (int)Math.Round(banDau.Width * tyLe));

            int h = Math.Max(
                1,
                (int)Math.Round(banDau.Height * tyLe));

            Bitmap anh = new Bitmap(
                w,
                h,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(anh))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode =
                    PixelOffsetMode.HighQuality;
                g.CompositingQuality =
                    CompositingQuality.HighQuality;

                g.DrawImage(
                    banDau,
                    new Rectangle(0, 0, w, h));
            }

            System.Drawing.Imaging.BitmapData khoa = null;

            try
            {
                khoa = anh.LockBits(
                    new Rectangle(0, 0, w, h),
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int buoc = khoa.Stride;
                byte[] diemAnh = new byte[buoc * h];

                Marshal.Copy(
                    khoa.Scan0,
                    diemAnh,
                    0,
                    diemAnh.Length);

                bool[] daTham = new bool[w * h];
                Queue<int> hangDoi = new Queue<int>();

                for (int x = 0; x < w; x++)
                {
                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x, 0, w, h, buoc);

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x, h - 1, w, h, buoc);
                }

                for (int y = 0; y < h; y++)
                {
                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        0, y, w, h, buoc);

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        w - 1, y, w, h, buoc);
                }

                while (hangDoi.Count > 0)
                {
                    int viTri = hangDoi.Dequeue();
                    int x = viTri % w;
                    int y = viTri / w;

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x - 1, y, w, h, buoc);

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x + 1, y, w, h, buoc);

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x, y - 1, w, h, buoc);

                    UiThemDiemNenLogo(
                        diemAnh, daTham, hangDoi,
                        x, y + 1, w, h, buoc);
                }

                int minX = w;
                int minY = h;
                int maxX = -1;
                int maxY = -1;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int pos = y * buoc + x * 4;

                        if (diemAnh[pos + 3] <= 12)
                            continue;

                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }

                Marshal.Copy(
                    diemAnh,
                    0,
                    khoa.Scan0,
                    diemAnh.Length);

                if (maxX < minX || maxY < minY)
                    return anh;

                int le = Math.Max(
                    1,
                    Math.Min(w, h) / 100);

                minX = Math.Max(0, minX - le);
                minY = Math.Max(0, minY - le);
                maxX = Math.Min(w - 1, maxX + le);
                maxY = Math.Min(h - 1, maxY + le);

                anh.UnlockBits(khoa);
                khoa = null;

                Rectangle vungLogo = Rectangle.FromLTRB(
                    minX, minY, maxX + 1, maxY + 1);

                Bitmap ketQua = new Bitmap(
                    vungLogo.Width,
                    vungLogo.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(ketQua))
                {
                    g.Clear(Color.Transparent);

                    g.DrawImage(
                        anh,
                        new Rectangle(
                            0, 0,
                            ketQua.Width,
                            ketQua.Height),
                        vungLogo,
                        GraphicsUnit.Pixel);
                }

                anh.Dispose();
                return ketQua;
            }
            finally
            {
                if (khoa != null)
                    anh.UnlockBits(khoa);
            }
        }

        private static void UiThemDiemNenLogo(
            byte[] diemAnh,
            bool[] daTham,
            Queue<int> hangDoi,
            int x,
            int y,
            int w,
            int h,
            int buoc)
        {
            if (x < 0 || y < 0 || x >= w || y >= h)
                return;

            int viTri = y * w + x;

            if (daTham[viTri])
                return;

            daTham[viTri] = true;

            int pos = y * buoc + x * 4;

            int b = diemAnh[pos];
            int g = diemAnh[pos + 1];
            int r = diemAnh[pos + 2];
            int alpha = diemAnh[pos + 3];

            bool mauNenTrang =
                r >= 216 &&
                g >= 216 &&
                b >= 216 &&
                Math.Max(r, Math.Max(g, b)) -
                Math.Min(r, Math.Min(g, b)) <= 22;

            if (alpha > 12 && !mauNenTrang)
                return;

            diemAnh[pos + 3] = 0;
            hangDoi.Enqueue(viTri);
        }

        private void UiThemDiemBienLogo(
            Bitmap bitmap,
            bool[,] daKiemTra,
            Queue<WinPoint> choKiemTra,
            int x,
            int y)
        {
            if (bitmap == null ||
                daKiemTra == null ||
                choKiemTra == null)
                return;

            if (x < 0 || y < 0 ||
                x >= bitmap.Width ||
                y >= bitmap.Height ||
                daKiemTra[x, y])
                return;

            daKiemTra[x, y] = true;

            Color p = bitmap.GetPixel(x, y);

            if (p.A > 0 &&
                p.R >= 238 &&
                p.G >= 238 &&
                p.B >= 238)
            {
                choKiemTra.Enqueue(new WinPoint(x, y));
            }
        }
    }
}