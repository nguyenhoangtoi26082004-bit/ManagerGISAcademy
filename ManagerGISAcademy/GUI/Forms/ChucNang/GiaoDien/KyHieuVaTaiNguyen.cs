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

        private void UiCapNhatCheDoNhanTheoYeuCau()
        {
            uiCheDoNhanDaApDung = -1;
            UiKiemTraKieuNhan();
        }

        private void UiBatHenGiamNhan()
        {
            if (uiHenNhan != null)
                return;

            uiHenNhan = new System.Windows.Forms.Timer
            {
                Interval = 6000
            };

            uiHenNhan.Tick += UiHenNhanKhiCan;
            uiHenNhan.Start();

            UiKiemTraKieuNhan();
        }

        private void UiHenNhanKhiCan(
            object sender,
            EventArgs e)
        {
            if (IsDisposed ||
                Disposing ||
                phanHeHienTai != "Bản đồ GIS" ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                uiCheDoHienNhan == null ||
                uiCheDoHienNhan.IsDisposed ||
                handleLopDiemPostGIS < 0)
                return;

            int cheDo = Math.Max(
                0,
                uiCheDoHienNhan.SelectedIndex);

            if (uiHandleNhanDaTao == handleLopDiemPostGIS &&
                uiCheDoNhanDaApDung == cheDo &&
                bd11HandleNhan == handleLopDiemPostGIS &&
                uiSoDiemDaTao == bd11SoDiemNhan)
            {
                return;
            }

            UiKiemTraKieuNhan();
        }

        private void UiYeuCauVeLaiNhan()
        {
            if (uiChoVeLaiNhan ||
                !IsHandleCreated ||
                IsDisposed ||
                Disposing)
                return;

            uiChoVeLaiNhan = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    uiChoVeLaiNhan = false;

                    if (IsDisposed ||
                        Disposing ||
                        phanHeHienTai != "Bản đồ GIS" ||
                        banDoMapWinGIS == null ||
                        banDoMapWinGIS.IsDisposed)
                        return;

                    banDoMapWinGIS.Redraw();
                }));
            }
            catch (InvalidOperationException)
            {
                uiChoVeLaiNhan = false;
            }
        }
        private void UiKiemTraKieuNhan()
        {
            if (phanHeHienTai != "Bản đồ GIS" ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                handleLopDiemPostGIS < 0 ||
                uiCheDoHienNhan == null ||
                uiCheDoHienNhan.IsDisposed)
                return;

            try
            {
                MapWinGIS.Shapefile sf =
                    banDoMapWinGIS.get_Shapefile(
                        handleLopDiemPostGIS);

                if (sf == null ||
                    sf.Labels == null ||
                    sf.NumShapes < 1)
                    return;

                MapWinGIS.Labels nhan = sf.Labels;

                int cheDo = Math.Max(
                    0,
                    uiCheDoHienNhan.SelectedIndex);

                bool tuDong = cheDo == 0;

                if (uiHandleNhanDaTao == handleLopDiemPostGIS &&
                    uiSoDiemDaTao == sf.NumShapes &&
                    uiSoNhanDaTao == nhan.Count &&
                    uiCheDoNhanDaApDung == cheDo &&
                    nhan.DynamicVisibility == tuDong &&
                    (!tuDong ||
                     Math.Abs(
                         nhan.MaxVisibleScale -
                         UiMaxTyLeNhanTuDong) < 0.5d))
                    return;

                nhan.FontName = "Segoe UI";
                nhan.FontSize = 8;
                nhan.FontBold = true;

                nhan.FontColor = unchecked(
                    (uint)ColorTranslator.ToOle(Color.White));

                nhan.FrameVisible = true;

                nhan.FrameBackColor = unchecked(
                    (uint)ColorTranslator.ToOle(
                        Color.FromArgb(21, 54, 44)));

                nhan.FrameTransparency = 100;

                nhan.FrameOutlineColor = unchecked(
                    (uint)ColorTranslator.ToOle(uiXanh));

                nhan.FrameOutlineWidth = 1;
                nhan.FramePaddingX = 4;
                nhan.FramePaddingY = 2;
                nhan.AvoidCollisions = true;
                nhan.CollisionBuffer = 16;
                nhan.OffsetX = 12.0;
                nhan.ScaleLabels = false;
                nhan.DynamicVisibility = tuDong;

                if (tuDong)
                {
                    nhan.MinVisibleScale = 1.0d;
                    nhan.MaxVisibleScale =
                        UiMaxTyLeNhanTuDong;
                }

                uiHandleNhanDaTao = handleLopDiemPostGIS;
                uiSoDiemDaTao = sf.NumShapes;
                uiSoNhanDaTao = nhan.Count;
                uiCheDoNhanDaApDung = cheDo;

                UiYeuCauVeLaiNhan();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "[UI_LABEL_STYLE] " + ex.Message);
            }
        }

        private void UiTaoNutDo()
        {
        }

        private void UiDoiThanhDo()
        {
            if (pnlThanhDoDac != null)
                pnlThanhDoDac.Visible = false;
        }

        private void UiDongThanhDo()
        {
            if (pnlThanhDoDac != null)
                pnlThanhDoDac.Visible = false;
        }
        private Bitmap UiLayBieuTuong(
    string ten,
    Color mau)
        {
            string key = ten + "_" + mau.ToArgb();

            Bitmap cache;

            if (uiBieuTuong.TryGetValue(key, out cache))
                return cache;

            Bitmap img = new Bitmap(
                24,
                24,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(img))
            using (Pen p = new Pen(mau, 1.8F))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                p.LineJoin = LineJoin.Round;
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                switch (ten)
                {
                    case "dashboard":
                        g.DrawRectangle(p, 4, 4, 6, 6);
                        g.DrawRectangle(p, 14, 4, 6, 6);
                        g.DrawRectangle(p, 4, 14, 6, 6);
                        g.DrawRectangle(p, 14, 14, 6, 6);
                        break;

                    case "map":
                        g.DrawPolygon(p, new[]
                        {
                            new WinPoint(3, 6),
                            new WinPoint(9, 3),
                            new WinPoint(15, 6),
                            new WinPoint(21, 3),
                            new WinPoint(21, 18),
                            new WinPoint(15, 21),
                            new WinPoint(9, 18),
                            new WinPoint(3, 21)
                        });

                        g.DrawLine(p, 9, 3, 9, 18);
                        g.DrawLine(p, 15, 6, 15, 21);
                        break;

                    case "search":
                        g.DrawEllipse(p, 4, 4, 12, 12);
                        g.DrawLine(p, 15, 15, 21, 21);
                        break;

                    case "folder":
                        g.DrawLines(p, new[]
                        {
                            new WinPoint(3, 8),
                            new WinPoint(3, 20),
                            new WinPoint(21, 20),
                            new WinPoint(21, 8),
                            new WinPoint(12, 8),
                            new WinPoint(10, 5),
                            new WinPoint(3, 5),
                            new WinPoint(3, 8)
                        });
                        break;

                    case "analysis":
                        g.DrawEllipse(p, 4, 4, 16, 16);
                        g.DrawEllipse(p, 9, 9, 6, 6);
                        g.DrawLine(p, 12, 2, 12, 6);
                        g.DrawLine(p, 2, 12, 6, 12);
                        g.DrawLine(p, 18, 12, 22, 12);
                        break;

                    case "chart":
                        g.DrawLine(p, 4, 4, 4, 21);
                        g.DrawLine(p, 4, 21, 21, 21);
                        g.DrawRectangle(p, 8, 12, 3, 6);
                        g.DrawRectangle(p, 14, 8, 3, 10);
                        break;

                    case "server":
                        // Bieu tuong may chu CSDL (3 dia xep chong).
                        g.DrawEllipse(p, 5, 3, 14, 5);
                        g.DrawLine(p, 5, 5, 5, 19);
                        g.DrawLine(p, 19, 5, 19, 19);
                        g.DrawArc(p, 5, 9, 14, 5, 0, 180);
                        g.DrawArc(p, 5, 15, 14, 5, 0, 180);
                        break;

                    case "shield":
                        g.DrawPolygon(p, new[]
                        {
                            new WinPoint(12, 3),
                            new WinPoint(20, 6),
                            new WinPoint(19, 15),
                            new WinPoint(12, 21),
                            new WinPoint(5, 15),
                            new WinPoint(4, 6)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(8, 12),
                            new WinPoint(11, 15),
                            new WinPoint(16, 9)
                        });
                        break;

                    case "extent":
                        g.DrawLines(p, new[]
                        {
                            new WinPoint(9, 4),
                            new WinPoint(4, 4),
                            new WinPoint(4, 9)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(15, 4),
                            new WinPoint(20, 4),
                            new WinPoint(20, 9)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(4, 15),
                            new WinPoint(4, 20),
                            new WinPoint(9, 20)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(15, 20),
                            new WinPoint(20, 20),
                            new WinPoint(20, 15)
                        });
                        break;

                    case "info":
                        g.DrawEllipse(p, 3, 3, 18, 18);
                        g.DrawLine(p, 12, 10, 12, 17);
                        g.DrawEllipse(p, 11.5F, 6.5F, 1F, 1F);
                        break;

                    case "hand":
                        g.DrawLines(p, new[]
                        {
                            new WinPoint(5, 12),
                            new WinPoint(5, 7),
                            new WinPoint(8, 7),
                            new WinPoint(8, 4),
                            new WinPoint(11, 4),
                            new WinPoint(11, 3),
                            new WinPoint(14, 3),
                            new WinPoint(14, 5),
                            new WinPoint(17, 5),
                            new WinPoint(17, 9),
                            new WinPoint(20, 9),
                            new WinPoint(20, 15),
                            new WinPoint(16, 21),
                            new WinPoint(9, 21),
                            new WinPoint(3, 15),
                            new WinPoint(3, 12),
                            new WinPoint(5, 12)
                        });
                        break;

                    case "ruler":
                        g.DrawPolygon(p, new[]
                        {
                            new WinPoint(3, 16),
                            new WinPoint(16, 3),
                            new WinPoint(21, 8),
                            new WinPoint(8, 21)
                        });

                        g.DrawLine(p, 7, 12, 10, 15);
                        g.DrawLine(p, 11, 8, 14, 11);
                        break;

                    case "layers":
                        g.DrawPolygon(p, new[]
                        {
                            new WinPoint(12, 3),
                            new WinPoint(22, 8),
                            new WinPoint(12, 13),
                            new WinPoint(2, 8)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(3, 12),
                            new WinPoint(12, 17),
                            new WinPoint(21, 12)
                        });

                        g.DrawLines(p, new[]
                        {
                            new WinPoint(3, 16),
                            new WinPoint(12, 21),
                            new WinPoint(21, 16)
                        });
                        break;
                }
            }

            uiBieuTuong.Add(key, img);
            return img;
        }
        private sealed class UiLogoSacNet : PictureBox
        {
            public UiLogoSacNet()
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (Image == null ||
                    ClientSize.Width < 1 ||
                    ClientSize.Height < 1)
                    return;

                Graphics g = e.Graphics;

                g.SmoothingMode =
                    SmoothingMode.HighQuality;

                g.CompositingQuality =
                    CompositingQuality.HighQuality;

                g.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

                g.PixelOffsetMode =
                    PixelOffsetMode.HighQuality;

                float sx =
                    (float)ClientSize.Width / Image.Width;

                float sy =
                    (float)ClientSize.Height / Image.Height;

                float scale = Math.Min(sx, sy);

                int w = Math.Max(
                    1,
                    (int)Math.Round(Image.Width * scale));

                int h = Math.Max(
                    1,
                    (int)Math.Round(Image.Height * scale));

                int x = (ClientSize.Width - w) / 2;
                int y = (ClientSize.Height - h) / 2;

                g.DrawImage(
                    Image,
                    new Rectangle(x, y, w, h),
                    0,
                    0,
                    Image.Width,
                    Image.Height,
                    GraphicsUnit.Pixel);
            }
        }

        private sealed class UiThanhZoomMoi : Control
        {
            private int giaTri;
            private bool dangKeo;

            public int Minimum { get; set; }
            public int Maximum { get; set; }

            public event Action<int> ValueCommitted;

            public int Value
            {
                get
                {
                    return giaTri;
                }
                set
                {
                    int moi = Math.Max(
                        Minimum,
                        Math.Min(Maximum, value));

                    if (moi == giaTri)
                        return;

                    giaTri = moi;
                    Invalidate();
                }
            }

            public UiThanhZoomMoi()
            {
                Minimum = 0;
                Maximum = 10;

                TabStop = true;
                BackColor = Color.White;
                Cursor = Cursors.Hand;

                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw,
                    true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                float x = Width / 2F;
                float top = 42F;

                float bottom =
                    Math.Max(top + 10, Height - 42F);

                using (Pen p = new Pen(
                    Color.FromArgb(93, 122, 104), 1.6F))
                {
                    g.DrawLine(p, x - 5, 19, x + 5, 19);
                    g.DrawLine(p, x, 14, x, 24);

                    g.DrawLine(
                        p,
                        x - 5,
                        Height - 19,
                        x + 5,
                        Height - 19);
                }

                float rate =
                    (float)(Value - Minimum) /
                    Math.Max(1, Maximum - Minimum);

                float y =
                    bottom - rate * (bottom - top);

                using (Pen p = new Pen(
                    Color.FromArgb(216, 229, 218), 3F))
                {
                    g.DrawLine(p, x, top, x, bottom);
                }

                using (Pen p = new Pen(
                    Color.FromArgb(16, 145, 111), 3F))
                {
                    g.DrawLine(p, x, y, x, bottom);
                }

                using (SolidBrush b = new SolidBrush(
                    Color.FromArgb(16, 145, 111)))
                {
                    g.FillEllipse(
                        b, x - 6, y - 6, 12, 12);
                }
            }

            private void DatTheoY(int y)
            {
                double bottom =
                    Math.Max(52D, Height - 42D);

                double rate =
                    (bottom - y) /
                    Math.Max(1D, bottom - 42D);

                rate = Math.Max(
                    0D,
                    Math.Min(1D, rate));

                Value = Minimum +
                    (int)Math.Round(
                        rate * (Maximum - Minimum));
            }

            private void Chot()
            {
                Action<int> handler = ValueCommitted;

                if (handler != null)
                    handler(Value);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);

                if (e.Button != MouseButtons.Left)
                    return;

                Focus();

                if (e.Y < 34)
                {
                    Value++;
                    Chot();
                    return;
                }

                if (e.Y > Height - 34)
                {
                    Value--;
                    Chot();
                    return;
                }

                dangKeo = true;
                Capture = true;
                DatTheoY(e.Y);
            }
            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);

                if (dangKeo)
                    DatTheoY(e.Y);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);

                if (e.Button != MouseButtons.Left ||
                    !dangKeo)
                    return;

                DatTheoY(e.Y);

                dangKeo = false;
                Capture = false;

                Chot();
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.KeyCode == Keys.Up ||
                    e.KeyCode == Keys.Add)
                {
                    Value++;
                }
                else if (e.KeyCode == Keys.Down ||
                         e.KeyCode == Keys.Subtract)
                {
                    Value--;
                }
                else
                {
                    return;
                }

                e.Handled = true;
                Chot();
            }
        }

        private void UiGiaiPhong(
            object sender,
            FormClosedEventArgs e)
        {
            Application.Idle -= UiChoCongCuBanDo;

            if (uiSidebarTayNam != null &&
                !uiSidebarTayNam.IsDisposed)
            {
                uiSidebarTayNam.MouseDown -=
                    UiSidebarBatDauKeo;

                uiSidebarTayNam.MouseMove -=
                    UiSidebarDangDiChuyen;

                uiSidebarTayNam.MouseUp -=
                    UiSidebarThaChuot;
            }

            if (uiDaGanCanZoomDoc)
            {
                if (pnlMapTools != null &&
                    !pnlMapTools.IsDisposed)
                {
                    pnlMapTools.LocationChanged -=
                        UiKhiDoiViTriCongCuZoom;

                    pnlMapTools.SizeChanged -=
                        UiKhiDoiViTriCongCuZoom;
                }

                if (bd11KhungZoom != null &&
                    !bd11KhungZoom.IsDisposed)
                {
                    bd11KhungZoom.LocationChanged -=
                        UiKhiDoiViTriCongCuZoom;

                    bd11KhungZoom.SizeChanged -=
                        UiKhiDoiViTriCongCuZoom;

                    bd11KhungZoom.VisibleChanged -=
                        UiKhiDoiViTriCongCuZoom;
                }

                uiDaGanCanZoomDoc = false;
            }
            if (pnlBrand != null &&
    !pnlBrand.IsDisposed)
            {
                pnlBrand.Paint -= UiVeNenThuongHieu;
            }

            if (pnlNoiDungChuGiai != null &&
                !pnlNoiDungChuGiai.IsDisposed)
            {
                pnlNoiDungChuGiai.Resize -=
                    UiKhiChuGiaiDoiKichThuoc;
            }

            if (uiHenNhan != null)
            {
                uiHenNhan.Stop();
                uiHenNhan.Tick -= UiHenNhanKhiCan;
                uiHenNhan.Dispose();
                uiHenNhan = null;
            }

            if (pnlRightCard != null &&
                !pnlRightCard.IsDisposed)
            {
                pnlRightCard.VisibleChanged -=
                    UiKhiDoiHienThiBangLop;
            }

            if (uiLogo != null &&
                !uiLogo.IsDisposed)
            {
                uiLogo.Image = null;
                uiLogo.Dispose();
            }

            if (uiAnhLogo != null)
            {
                uiAnhLogo.Dispose();
                uiAnhLogo = null;
            }

            foreach (Bitmap icon in uiBieuTuong.Values)
            {
                icon.Dispose();
            }

            uiBieuTuong.Clear();

            if (uiNutMoBangLop != null &&
                !uiNutMoBangLop.IsDisposed)
            {
                uiNutMoBangLop.Image = null;
            }

            if (uiAnhNutMoBangLop != null)
            {
                uiAnhNutMoBangLop.Dispose();
                uiAnhNutMoBangLop = null;
            }

            uiMeo.Dispose();
            uiFontMenu.Dispose();
            uiFontMenuChon.Dispose();
            uiFontPhu.Dispose();
            uiFontTenHeThong.Dispose();
            uiFontTieuDeMotDong.Dispose();
            uiFontTieuDeHaiDong.Dispose();

            uiDaTrangTri.Clear();
            uiDaBoGoc.Clear();
        }

    }
}