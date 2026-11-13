// Phan tach tu FormMain.cs (GD5.7.6), dung partial class.
// Khong khai bao lop FormMain moi, khong thay doi FormMain.Designer.cs.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using ManagerGISAcademy.GUI.Views;
using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private void ThucHienBanDo(string ten, Action thaoTac)
        {
            if (ten == "Phóng to" || ten == "Thu nhỏ")
            {
                BanDo_YeuCauZoomBuoc(
                    ten == "Phóng to" ? 1 : -1,
                    false);

                return;
            }

            if (ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.NumLayers == 0)
            {
                statusGeneral.Text =
                    "● Bản đồ chưa có lớp dữ liệu.";

                return;
            }

            if (dangThaoTacBanDo)
                return;

            if (henXuLyZoom != null)
                henXuLyZoom.Stop();

            soBuocZoomDangCho = 0;
            dangThaoTacBanDo = true;

            btnZoomIn.Enabled = false;
            btnZoomOut.Enabled = false;
            btnFullExtent.Enabled = false;

            try
            {
                GhiNhatKyBanDo("MAP_ACTION_BEGIN | " + ten);

                thaoTac();

                GhiNhatKyBanDo("MAP_ACTION_OK | " + ten);

                EventHandler handler = YeuCauToanCanhBanDo;

                if (handler != null)
                    handler(this, EventArgs.Empty);

                statusGeneral.Text = "● Bản đồ: " + ten;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("MAP_ACTION_FAILED | " + ex);

                statusGeneral.Text =
                    "● Lỗi bản đồ: " + ex.Message;
            }
            finally
            {
                dangThaoTacBanDo = false;

                if (!IsDisposed && !Disposing)
                {
                    btnZoomIn.Enabled = true;
                    btnZoomOut.Enabled = true;
                    btnFullExtent.Enabled = true;
                }
            }
        }

        private bool BanDo_YeuCauZoomBuoc(
    int soBuoc,
    bool thucHienNgay)
        {
            if (soBuoc == 0)
                return true;

            if (IsDisposed ||
                Disposing ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.NumLayers == 0)
            {
                return false;
            }

            if (dangThaoTacBanDo)
                return false;

            soBuocZoomDangCho = Math.Max(
                -ZoomGioiHanBuocCho,
                Math.Min(
                    ZoomGioiHanBuocCho,
                    soBuocZoomDangCho + soBuoc));

            if (henXuLyZoom == null)
            {
                henXuLyZoom = new System.Windows.Forms.Timer();
                henXuLyZoom.Interval = ZoomGomYeuCauMs;
                henXuLyZoom.Tick += BanDo_XuLyZoomDangCho;
            }

            henXuLyZoom.Stop();

            if (thucHienNgay)
                BanDo_XuLyZoomDangCho(this, EventArgs.Empty);
            else
                henXuLyZoom.Start();

            return true;
        }

        private void BanDo_XuLyZoomDangCho(object sender, EventArgs e)
        {
            if (henXuLyZoom != null)
                henXuLyZoom.Stop();

            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                soBuocZoomDangCho = 0;
                return;
            }

            if (dangThaoTacBanDo)
            {
                if (henXuLyZoom != null)
                    henXuLyZoom.Start();

                return;
            }

            int soBuoc = soBuocZoomDangCho;
            soBuocZoomDangCho = 0;

            if (soBuoc == 0 ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.NumLayers == 0)
            {
                return;
            }

            dangThaoTacBanDo = true;

            System.Diagnostics.Stopwatch dongHo =
                System.Diagnostics.Stopwatch.StartNew();

            try
            {
                MapWinGIS.Extents cu = banDoMapWinGIS.Extents;

                if (cu == null)
                    return;

                double rong = cu.xMax - cu.xMin;
                double cao = cu.yMax - cu.yMin;

                if (rong <= 0d || cao <= 0d ||
                    double.IsNaN(rong) || double.IsNaN(cao) ||
                    double.IsInfinity(rong) || double.IsInfinity(cao))
                {
                    return;
                }

                double heSo = Math.Pow(ZoomHeSoMoiBuoc, soBuoc);
                double rongMoi = rong * heSo;
                double caoMoi = cao * heSo;

                double metMoiPixel = rongMoi /
                    Math.Max(1, banDoMapWinGIS.ClientSize.Width);

                if (metMoiPixel < 0.5d ||
                    metMoiPixel > 200000d ||
                    double.IsNaN(metMoiPixel) ||
                    double.IsInfinity(metMoiPixel))
                {
                    return;
                }

                double tamX = (cu.xMin + cu.xMax) / 2d;
                double tamY = (cu.yMin + cu.yMax) / 2d;

                MapWinGIS.Extents moi = new MapWinGIS.Extents();

                moi.SetBounds(
                    tamX - rongMoi / 2d,
                    tamY - caoMoi / 2d,
                    0d,
                    tamX + rongMoi / 2d,
                    tamY + caoMoi / 2d,
                    0d);

                banDoMapWinGIS.Extents = moi;
                lanZoomCuoi = DateTime.UtcNow;

                EventHandler handler = soBuoc > 0
                    ? YeuCauPhongToBanDo
                    : YeuCauThuNhoBanDo;

                if (handler != null)
                    handler(this, EventArgs.Empty);

                statusGeneral.Text = soBuoc > 0
                    ? "● Bản đồ: Phóng to"
                    : "● Bản đồ: Thu nhỏ";

                System.Diagnostics.Debug.WriteLine(
                    "[MapWinGIS] ZOOM_COMMIT | steps=" + soBuoc +
                    " | factor=" +
                    heSo.ToString("F4", CultureInfo.InvariantCulture) +
                    " | mpp=" +
                    metMoiPixel.ToString("F3", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("ZOOM_COMMIT_FAILED | " + ex);

                statusGeneral.Text =
                    "● Lỗi thu phóng bản đồ: " + ex.Message;
            }
            finally
            {
                dongHo.Stop();

                System.Diagnostics.Debug.WriteLine(
                    "[MapWinGIS] ZOOM_COMMIT_TIME | ms=" +
                    dongHo.ElapsedMilliseconds +
                    " | note=Extents_assignment_not_total_raster_render");

                dangThaoTacBanDo = false;
            }
        }
    }
}