using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // C# 7.3 / .NET Framework 4.8. Tái sử dụng danhSachLop và bộ chọn nền.
    public partial class FormMain
    {
        private void Lp26_PhongDenLop(int handle)
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null ||
                dangThaoTacBanDo)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                {
                    HenLamMoiDanhSachLop();
                    statusGeneral.Text = "● Lớp không còn trên bản đồ.";
                    return;
                }

                MapWinGIS.Shapefile lop =
                    banDoMapWinGIS.get_GetObject(handle) as MapWinGIS.Shapefile;

                if (lop != null && lop.NumShapes == 0)
                {
                    statusGeneral.Text =
                        "● Lớp chưa có đối tượng để phóng đến.";
                    return;
                }

                banDoMapWinGIS.ZoomToLayer(handle);

                bool hien = ketNoiMapWinGIS.LayHienThiLop(handle);

                statusGeneral.Text =
                    "● Đã đến phạm vi lớp" +
                    (hien ? "." : "; lớp đang ẩn.");

                GhiNhatKyBanDo(
                    "LAYER_ZOOM | handle=" +
                    handle + " | visible=" + hien);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LAYER_ZOOM_FAILED | " + ex);
                statusGeneral.Text =
                    "● Không phóng được đến lớp: " + ex.Message;
            }
        }

        private void DoiTrangThaiLopTuThe(int handle)
        {
            if (dangLamMoiGiaoDienLop ||
                dangCapNhatLop ||
                dangThaoTacBanDo ||
                ketNoiMapWinGIS == null ||
                danhSachLop == null)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                {
                    HenLamMoiDanhSachLop();
                    return;
                }

                bool hienTai = ketNoiMapWinGIS.LayHienThiLop(handle);

                if (handle == handleAnhNenTuDong)
                {
                    // Nền cục bộ đi qua bộ chuyển nền, tránh chồng tile.
                    ChuyenBanDoNen(hienTai ? 2 : 0);
                    HenLamMoiDanhSachLop();
                    HenCapNhatChuGiai();
                    return;
                }

                for (int i = 0; i < danhSachLop.Items.Count; i++)
                {
                    LopMapItem muc = danhSachLop.Items[i] as LopMapItem;

                    if (muc == null || muc.Handle != handle)
                        continue;

                    danhSachLop.SetItemChecked(i, !hienTai);
                    HenLamMoiDanhSachLop();
                    return;
                }
            }
            catch (Exception ex)
            {
                HenLamMoiDanhSachLop();
                statusGeneral.Text =
                    "● Không đổi được trạng thái lớp: " + ex.Message;
            }
        }

        private static bool LaLopPostGIS(string ten)
        {
            return !string.IsNullOrWhiteSpace(ten) &&
                ten.StartsWith(
                    "Đơn vị từ PostGIS",
                    StringComparison.OrdinalIgnoreCase);
        }

        private static bool LaLopRaster(string ten)
        {
            if (string.IsNullOrWhiteSpace(ten))
                return false;

            string chuoi = ten.ToUpperInvariant();

            return chuoi.Contains("SATELLITE") ||
                   chuoi.EndsWith(".TIF") ||
                   chuoi.EndsWith(".TIFF") ||
                   chuoi.Contains("RASTER");
        }

        private static string LayTenHienThiLop(string ten)
        {
            if (LaLopPostGIS(ten))
                return "Đơn vị quân đội";

            if (!string.IsNullOrWhiteSpace(ten) &&
                ten.StartsWith(
                    "HANOI_SATELLITE",
                    StringComparison.OrdinalIgnoreCase))
                return "Ảnh vệ tinh Hà Nội";

            return string.IsNullOrWhiteSpace(ten)
                ? "Lớp dữ liệu"
                : ten;
        }

        private static string LayMoTaHienThiLop(string ten)
        {
            if (LaLopPostGIS(ten))
            {
                int dau = ten.IndexOf('(');
                int cuoi = ten.IndexOf(')', dau + 1);

                if (dau >= 0 && cuoi > dau)
                {
                    return ten.Substring(
                        dau + 1,
                        cuoi - dau - 1) + " · PostGIS";
                }

                return "Lớp điểm · PostGIS";
            }

            return LaLopRaster(ten)
                ? "Ảnh bản đồ"
                : "Dữ liệu GIS";
        }

        private void CanChinhChieuRongTheLop()
        {
            if (khungTheLop == null ||
                khungTheLop.IsDisposed)
                return;

            int rong = Math.Max(
                1,
                khungTheLop.ClientSize.Width -
                khungTheLop.Padding.Horizontal -
                SystemInformation.VerticalScrollBarWidth - 2);

            foreach (Control c in khungTheLop.Controls)
            {
                if (c.Width != rong)
                    c.Width = rong;
            }
        }

        private void CapNhatThongKeDanhSachLop()
        {
            if (!daSapXepLop || danhSachLop == null)
                return;

            int tong = danhSachLop.Items.Count;
            int bat = danhSachLop.CheckedItems.Count;

            lblTrangThaiDanhSachLop.Text =
                bat + "/" + tong +
                " bật · " + lp26SoLopTrongBoLoc + " phù hợp";

            lblTongSoLopGiaoDien.Text =
                tong + " lớp";
        }

        private void HenLamMoiDanhSachLop()
        {
            if (!daSapXepLop ||
                lp26ChoLamMoi ||
                dangLamMoiGiaoDienLop ||
                !IsHandleCreated ||
                IsDisposed ||
                Disposing)
                return;

            lp26ChoLamMoi = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    lp26ChoLamMoi = false;

                    if (!IsDisposed && !Disposing)
                        LamMoiDanhSachLop();
                }));
            }
            catch (InvalidOperationException)
            {
                lp26ChoLamMoi = false;
            }
        }

        private void Lp26_HenDongBo(object sender, EventArgs e)
        {
            HenLamMoiDanhSachLop();
        }

        private void Lp26_KhiThemLop(
            object sender,
            AxMapWinGIS._DMapEvents_LayerAddedEvent e)
        {
            HenLamMoiDanhSachLop();
        }

        private void Lp26_KhiXoaLop(
            object sender,
            AxMapWinGIS._DMapEvents_LayerRemovedEvent e)
        {
            lp26HandleThongTin = -1;
            HenLamMoiDanhSachLop();
        }

        private void Lp26_KhiDoiDanhDau(
            object sender,
            ItemCheckEventArgs e)
        {
            if (!dangCapNhatLop)
                HenLamMoiDanhSachLop();
        }

        private void Lp26_KhiMoBangLop(object sender, EventArgs e)
        {
            if (pnlRightCard.Visible)
                HenLamMoiDanhSachLop();
        }

        private void DangKyDongBoDanhSachLop()
        {
            if (lp26DaGanSuKien)
                return;

            lp26DaGanSuKien = true;

            btnNavMap.Click += Lp26_HenDongBo;
            btnNapLop.Click += Lp26_HenDongBo;
            cboChonBanDoNen.SelectedIndexChanged += Lp26_HenDongBo;
            btnChonTepBanDoNen.Click += Lp26_HenDongBo;

            danhSachLop.ItemCheck += Lp26_KhiDoiDanhDau;
            pnlRightCard.VisibleChanged += Lp26_KhiMoBangLop;

            banDoMapWinGIS.LayerAdded += Lp26_KhiThemLop;
            banDoMapWinGIS.LayerRemoved += Lp26_KhiXoaLop;
        }
    }
}