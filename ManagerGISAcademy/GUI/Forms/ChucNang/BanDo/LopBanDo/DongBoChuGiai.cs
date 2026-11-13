using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WinLabel = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Chu giai ban do GIS hien dai.
    ///
    /// - Nhan biet ranh gioi Ha Noi, quan/huyen.
    /// - Hien thi diem quan doi va anh ve tinh.
    /// - Bat/tat lop truc tiep tu chu giai.
    /// - Dong bo danh sach lop MapWinGIS.
    /// - Khong sua du lieu PostGIS.
    /// - Gop chu giai vao bang LOP BAN DO.
    ///
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {

        // =====================================================
        // 11. BAT/TAT LOP TREN CHU GIAI
        // =====================================================
        private void ChuGiai_DoiHienThiLop(
            int handle,
            bool hienThi)
        {
            if (ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                    return;

                ketNoiMapWinGIS.DatHienThiLop(
                    handle,
                    hienThi);

                // Đồng bộ CheckedListBox quản lý lớp.

                bool trangThaiCu =
                    dangCapNhatLop;

                dangCapNhatLop = true;

                try
                {
                    for (int i = 0;
                         i < danhSachLop.Items.Count;
                         i++)
                    {
                        LopMapItem item =
                            danhSachLop.Items[i]
                            as LopMapItem;

                        if (item == null ||
                            item.Handle != handle)
                        {
                            continue;
                        }

                        if (danhSachLop.GetItemChecked(i)
                            != hienThi)
                        {
                            danhSachLop.SetItemChecked(
                                i,
                                hienThi);
                        }

                        break;
                    }
                }
                finally
                {
                    dangCapNhatLop =
                        trangThaiCu;
                }

                banDoMapWinGIS.Redraw();

                HenCapNhatChuGiai();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "MAP_LEGEND_TOGGLE_ERROR | " +
                    ex);

                MessageBox.Show(
                    this,

                    "Không thay đổi được trạng thái lớp.\r\n" +
                    ex.Message,

                    "Chú giải bản đồ",

                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                HenCapNhatChuGiai();
            }
        }

        // =====================================================
        // 12. CAN CHIEU RONG THE CHU GIAI
        // =====================================================

        private void CanRongTheChuGiai()
        {
            if (flpCacLopChuGiai == null ||
                flpCacLopChuGiai.IsDisposed)
            {
                return;
            }

            int rong = Math.Max(
                100,
                flpCacLopChuGiai.ClientSize.Width -
                SystemInformation.VerticalScrollBarWidth -
                3);

            foreach (Control item in
                flpCacLopChuGiai.Controls)
            {
                if (item.Width != rong)
                {
                    item.Width = rong;
                }
            }
        }

        // =====================================================
        // 13. DONG BO VOI GIAO DIEN HIEN CO
        // =====================================================

        // =====================================================
        // GD5.4: CAN TAM BAN DO THEO BANG LOP BEN PHAI
        // =====================================================

        private void Cg54DangKyDichTam()
        {
            if (cg54DaDangKyDichTam ||
                pnlRightCard == null || pnlRightCard.IsDisposed)
                return;

            cg54DaDangKyDichTam = true;
            pnlRightCard.VisibleChanged += Cg54KhiBangLopThayDoi;
            pnlRightCard.SizeChanged += Cg54KhiBangLopThayDoi;
            pnlRightCard.LocationChanged += Cg54KhiBangLopThayDoi;

            // Xu ly ca truong hop bang lop da mo tu luc khoi dong.
            Cg54HenDichTam();
        }

        private void Cg54KhiBangLopThayDoi(object sender, EventArgs e)
        {
            Cg54HenDichTam();
        }

        private void Cg54HenDichTam()
        {
            if (cg54DangChoDichTam || !IsHandleCreated ||
                IsDisposed || Disposing)
                return;

            cg54DangChoDichTam = true;
            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    cg54DangChoDichTam = false;
                    if (!IsDisposed && !Disposing)
                        Cg54DichTamTheoBangLop();
                }));
            }
            catch (InvalidOperationException)
            {
                cg54DangChoDichTam = false;
            }
        }

        private void Cg54DichTamTheoBangLop()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated ||
                banDoMapWinGIS.ClientSize.Width < 1)
                return;

            try
            {
                bool bangDangMo = false;
                double pixelCanDich = 0.0;

                if (pnlRightCard != null && !pnlRightCard.IsDisposed &&
                    pnlRightCard.Visible && pnlRightCard.IsHandleCreated)
                {
                    Rectangle khungBanDo = banDoMapWinGIS.RectangleToScreen(
                        banDoMapWinGIS.ClientRectangle);
                    Rectangle khungBangLop = pnlRightCard.RectangleToScreen(
                        pnlRightCard.ClientRectangle);
                    Rectangle giao = Rectangle.Intersect(khungBanDo, khungBangLop);

                    // Chi xu ly bang lop dang che phia ben phai cua ban do.
                    if (giao.Width > 40 &&
                        giao.Left > khungBanDo.Left + khungBanDo.Width / 3)
                    {
                        bangDangMo = true;
                        pixelCanDich = Math.Min(170.0, giao.Width * 0.5);
                    }
                }

                // Khong dich lai moi lan control ve lai / Resize.
                if (bangDangMo == cg54DaDichTam)
                    return;

                double pixelDich = bangDangMo
                    ? pixelCanDich : -cg54SoPixelDaDich;
                if (Math.Abs(pixelDich) < 1.0)
                    return;

                MapWinGIS.Extents hienTai = banDoMapWinGIS.Extents;
                if (hienTai == null)
                    return;

                double chieuRongBanDo = hienTai.xMax - hienTai.xMin;
                if (chieuRongBanDo <= 0.0 ||
                    double.IsNaN(chieuRongBanDo) ||
                    double.IsInfinity(chieuRongBanDo))
                    return;

                // Tang toa do X: doi tuong di sang trai tren man hinh.
                // Giu nguyen do cao va chieu rong Extents => khong doi zoom.
                double dx = pixelDich * chieuRongBanDo /
                    banDoMapWinGIS.ClientSize.Width;

                MapWinGIS.Extents moi = new MapWinGIS.Extents();
                moi.SetBounds(
                    hienTai.xMin + dx,
                    hienTai.yMin,
                    0.0,
                    hienTai.xMax + dx,
                    hienTai.yMax,
                    0.0);

                banDoMapWinGIS.Extents = moi;
                cg54DaDichTam = bangDangMo;
                cg54SoPixelDaDich = bangDangMo ? pixelCanDich : 0.0;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("MAP_PANEL_RECENTER_ERROR | " + ex.Message);
            }
        }

        private void DangKyDongBoChuGiai()
        {
            danhSachLop.ItemCheck +=
                (s, e) => HenCapNhatChuGiai();

            btnNavMap.Click +=
                (s, e) => HenCapNhatChuGiai();

            btnNapLop.Click +=
                (s, e) => HenCapNhatChuGiai();

            cboChonBanDoNen.SelectedIndexChanged +=
                (s, e) => HenCapNhatChuGiai();

            btnChonTepBanDoNen.Click +=
                (s, e) => HenCapNhatChuGiai();

            if (btnLamMoiDanhSachLop != null)
            {
                btnLamMoiDanhSachLop.Click +=
                    (s, e) => HenCapNhatChuGiai();
            }
        }

        private void HenCapNhatChuGiai()
        {
            if (!daKhoiTaoChuGiai ||
                dangChoCapNhatChuGiai ||
                !IsHandleCreated ||
                IsDisposed ||
                Disposing)
            {
                return;
            }

            dangChoCapNhatChuGiai = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    dangChoCapNhatChuGiai = false;

                    if (!IsDisposed && !Disposing)
                    {
                        CapNhatChuGiaiBanDo();
                    }
                }));
            }
            catch (InvalidOperationException)
            {
                dangChoCapNhatChuGiai = false;
            }
        }
    }
}