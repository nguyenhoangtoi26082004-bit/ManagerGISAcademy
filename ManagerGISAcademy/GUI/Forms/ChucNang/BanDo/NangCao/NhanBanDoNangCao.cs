using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MapWinGIS;

using WinImage = System.Drawing.Image;
using GisImage = MapWinGIS.Image;
using WinLabel = System.Windows.Forms.Label;
using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Ban do GIS nang cap.
    /// Giu nguyen cac chuc nang MapWinGIS, PostGIS va do dac.
    /// Thanh zoom doc duoc dat duoi 5 nut cong cu, cach 12 px.
    /// C# 7.3 - .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {
        // =====================================================
        // 12. CHON / PAN / DO DAC
        // =====================================================

        private void Bd11_ChonCheDo(bool chon)
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            ThoatDoDacKhiChuyenCongCu();
            DatCheDoBanDo(chon);

            Bd11_DanhDauChon(chon);
            Bd11_AnThanhTrung();
        }

        private void Bd11_DanhDauChon(bool chon)
        {
            if (bd11NutChon == null || bd11NutKeo == null)
                return;

            bd11NutChon.BackColor =
                chon ? bd11Xanh : bd11Nen;

            bd11NutKeo.BackColor =
                chon ? bd11Nen : bd11Xanh;
        }

        private void Bd11_BatDo(int kieu)
        {
            BatCheDoDoDac(kieu);

            bd11NutChon.BackColor = bd11Nen;
            bd11NutKeo.BackColor = bd11Nen;

            Bd11_AnThanhTrung();

            if (pnlKetQuaDoDac != null &&
                pnlKetQuaDoDac.Visible)
                pnlKetQuaDoDac.BringToFront();

            pnlMapTools.BringToFront();
        }

        private static bool Bd11_ChuaNutDo(Control panel)
        {
            foreach (Control c in panel.Controls)
            {
                Button b = c as Button;

                if (b == null)
                    continue;

                string ten = b.Text ?? "";

                if (ten.IndexOf(
                    "CÔNG CỤ ĐO",
                    StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ten.IndexOf(
                        "ẨN CÔNG CỤ",
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        private void Bd11_AnThanhTrung()
        {
            if (pnlCheDoBanDo != null)
                pnlCheDoBanDo.Visible = false;

            if (pnlThanhDoDac != null)
                pnlThanhDoDac.Visible = false;

            if (btnNapLop != null)
                btnNapLop.Visible = false;

            // Khong an zoom: luon can no duoi nhom nut.
            Bd11_DatZoomBenDuoi();

            if (pnlMapCanvas == null)
                return;

            foreach (Control c in pnlMapCanvas.Controls)
            {
                Panel p = c as Panel;

                if (p == null ||
                    p == pnlMapTools ||
                    p == bd11KhungZoom ||
                    p == pnlKetQuaDoDac ||
                    p == pnlRightCard)
                    continue;

                if (Bd11_ChuaNutDo(p))
                    p.Visible = false;
            }
        }

        // =====================================================
        // 13. CAN CHINH THANH CONG CU VA THANH ZOOM
        // =====================================================

        private void Bd11_CanChinh()
        {
            if (bd11DangCan ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed)
                return;

            bd11DangCan = true;

            try
            {
                pnlMapTools.SetBounds(17, 16, 49, 235);
                pnlMapTools.BringToFront();

                // Zoom luon nam duoi 5 nut.
                Bd11_DatZoomBenDuoi();

                if (pnlChuGiaiBanDo != null &&
                    !pnlChuGiaiBanDo.IsDisposed)
                {
                    DatViTriChuGiai();
                    pnlChuGiaiBanDo.BringToFront();
                }
            }
            finally
            {
                bd11DangCan = false;
            }
        }

        private void Bd11_KhiVeBanDo()
        {
            if (!bd11DaKhoiTao || IsDisposed || Disposing)
                return;

            Bd11_AnThanhTrung();
            Bd11_CanChinh();
            Bd11_DatZoomBenDuoi();
            Bd11_DatMauNenBanDo();
            Bd11_GanNhanDonVi();

            if (bd11CanVeKhiTroLai)
                Bd11_YeuCauVeLaiBanDo();
        }

        private void Bd11_YeuCauVeLaiBanDo()
        {
            bd11CanVeKhiTroLai = true;

            if (bd11ChoVeLai ||
                !IsHandleCreated ||
                IsDisposed ||
                Disposing ||
                phanHeHienTai != "Bản đồ GIS")
                return;

            bd11ChoVeLai = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    bd11ChoVeLai = false;

                    if (IsDisposed ||
                        Disposing ||
                        banDoMapWinGIS == null ||
                        banDoMapWinGIS.IsDisposed ||
                        phanHeHienTai != "Bản đồ GIS")
                        return;

                    try
                    {
                        banDoMapWinGIS.Redraw();
                        bd11CanVeKhiTroLai = false;
                    }
                    catch (Exception ex)
                    {
                        GhiNhatKyBanDo(
                            "BD11_REDRAW_FAILED | " + ex.Message);
                    }
                }));
            }
            catch (InvalidOperationException)
            {
                bd11ChoVeLai = false;
            }
        }

        private void Bd11_GanNhanDonVi()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null ||
                handleLopDiemPostGIS < 0)
                return;

            if (!bd11NhanCanKiemTra &&
                bd11HandleNhan == handleLopDiemPostGIS)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS))
                    return;

                Shapefile sf =
                    banDoMapWinGIS.get_Shapefile(
                        handleLopDiemPostGIS);

                if (sf == null ||
                    sf.NumShapes == 0 ||
                    sf.Labels == null)
                    return;

                Labels nhan = sf.Labels;

                if (bd11HandleNhan == handleLopDiemPostGIS &&
                    bd11SoDiemNhan == sf.NumShapes)
                {
                    bd11NhanCanKiemTra = false;

                    if (nhan.Visible != bd11HienNhan)
                    {
                        nhan.Visible = bd11HienNhan;
                        Bd11_YeuCauVeLaiBanDo();
                    }

                    return;
                }

                int cotTen = Bd11_TimCotTen(sf);

                if (cotTen < 0)
                {
                    GhiNhatKyBanDo(
                        "BD11_NO_UNIT_NAME_FIELD");

                    bd11HandleNhan = handleLopDiemPostGIS;
                    bd11SoDiemNhan = sf.NumShapes;
                    bd11NhanCanKiemTra = false;
                    return;
                }

                Bd11_LamDepKyHieuDonVi(sf);

                bool daKhoaCuaSo = false;
                int soNhan = 0;

                System.Diagnostics.Stopwatch dongHo =
                    System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    if (banDoMapWinGIS.IsLocked ==
                        tkLockMode.lmUnlock)
                    {
                        banDoMapWinGIS.LockWindow(
                            tkLockMode.lmLock);

                        daKhoaCuaSo = true;
                    }

                    nhan.Synchronized = false;
                    nhan.Clear();

                    for (int i = 0; i < sf.NumShapes; i++)
                    {
                        string ten = Convert.ToString(
                            sf.get_CellValue(cotTen, i));

                        if (string.IsNullOrWhiteSpace(ten))
                            continue;

                        Shape hinh = sf.get_Shape(i);

                        if (hinh == null)
                            continue;

                        MapWinGIS.Point p =
                            hinh.get_Point(0);

                        if (p == null)
                            continue;

                        nhan.AddLabel(
                            ten.Trim(),
                            p.x,
                            p.y,
                            0.0,
                            -1);

                        soNhan++;
                    }

                    nhan.Visible = bd11HienNhan;
                    nhan.AvoidCollisions = true;
                    nhan.CollisionBuffer = 7;
                    nhan.AutoOffset = false;
                    nhan.OffsetX = 14.0;
                    nhan.OffsetY = 0.0;
                    nhan.RemoveDuplicates = false;

                    nhan.FontName = "Segoe UI";
                    nhan.FontSize = 9;
                    nhan.FontBold = true;

                    nhan.FontColor = unchecked(
                        (uint)ColorTranslator.ToOle(
                            Color.White));

                    nhan.FrameVisible = true;

                    nhan.FrameBackColor = unchecked(
                        (uint)ColorTranslator.ToOle(
                            Color.FromArgb(13, 27, 44)));

                    nhan.FrameTransparency = 234;

                    nhan.FrameOutlineColor = unchecked(
                        (uint)ColorTranslator.ToOle(
                            Color.FromArgb(48, 118, 117)));

                    nhan.FrameOutlineWidth = 1;
                    nhan.FramePaddingX = 5;
                    nhan.FramePaddingY = 3;
                    nhan.ScaleLabels = false;

                    bool tuDong =
                        uiCheDoHienNhan == null ||
                        uiCheDoHienNhan.IsDisposed ||
                        uiCheDoHienNhan.SelectedIndex <= 0;

                    nhan.DynamicVisibility = tuDong;

                    if (tuDong)
                    {
                        nhan.MinVisibleScale = 1d;
                        nhan.MaxVisibleScale =
                            UiMaxTyLeNhanTuDong;
                    }

                    bd11HandleNhan = handleLopDiemPostGIS;
                    bd11SoDiemNhan = sf.NumShapes;
                    bd11NhanCanKiemTra = false;

                    uiHandleNhanDaTao = -1;
                }
                finally
                {
                    if (daKhoaCuaSo &&
                        !banDoMapWinGIS.IsDisposed)
                    {
                        banDoMapWinGIS.LockWindow(
                            tkLockMode.lmUnlock);
                    }

                    dongHo.Stop();

                    System.Diagnostics.Debug.WriteLine(
                        "[MapWinGIS] LABEL_BUILD | ms=" +
                        dongHo.ElapsedMilliseconds +
                        " | count=" + soNhan +
                        " | layer=" + handleLopDiemPostGIS);
                }

                Bd11_YeuCauVeLaiBanDo();

                GhiNhatKyBanDo(
                    "BD11_LABELS_CACHED: " +
                    soNhan + "/" + sf.NumShapes);
            }
            catch (Exception ex)
            {
                bd11NhanCanKiemTra = true;

                GhiNhatKyBanDo(
                    "BD11_LABELS_ERROR | " + ex);
            }
        }

        // =====================================================
        // 14. GIAI PHONG TAI NGUYEN
        // =====================================================

        private void Bd11_KhiDongForm(
            object sender,
            FormClosedEventArgs e)
        {
            Bd11_HuyHenKhoiTao();

            if (bd11HenZoomDoc != null)
            {
                bd11HenZoomDoc.Stop();
                bd11HenZoomDoc.Tick -=
                    Bd11_ZoomDocHenGio;

                bd11HenZoomDoc.Dispose();
                bd11HenZoomDoc = null;
            }

            if (bd11HenDongBo != null)
            {
                bd11HenDongBo.Stop();
                bd11HenDongBo.Dispose();
                bd11HenDongBo = null;
            }

            if (bd11MenuDo != null)
                bd11MenuDo.Dispose();

            if (bd11MenuLop != null)
                bd11MenuLop.Dispose();

            foreach (PictureBox pb in bd11DanhSachIcon)
            {
                if (pb == null)
                    continue;

                WinImage anh = pb.Image;
                pb.Image = null;

                if (!pb.IsDisposed)
                    pb.Dispose();

                if (anh != null)
                    anh.Dispose();
            }

            bd11DanhSachIcon.Clear();
            bd11ChuThich.Dispose();
        }
    }
}