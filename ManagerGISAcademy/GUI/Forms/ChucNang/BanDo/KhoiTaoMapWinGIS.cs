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
        private DateTime banDoLanCapNhatToaDoChuot =
            DateTime.MinValue;

        private void KhoiTaoMapWinGIS()
        {
            if (dieuKhienBanDo != null)
                return;

            AxMap moi = null;

            GhiNhatKyBanDo("MAP_INIT_BEGIN");

            try
            {
                moi = new AxMap();

                ((ISupportInitialize)moi).BeginInit();

                moi.Dock = DockStyle.Fill;
                pnlMapCanvas.Controls.Add(moi);

                ((ISupportInitialize)moi).EndInit();
                // Tat toa do mac dinh o phia tren ban do.
                moi.ShowCoordinates =
                    MapWinGIS.tkCoordinatesDisplay.cdmNone;
                // Cho phep theo doi con tro chuot.
                moi.SendMouseMove = true;
                moi.Tiles.Visible = false;
                moi.Tiles.UseServer = false;
                moi.UseSeamlessPan = false;
                moi.MouseWheelSpeed = 0.90d;

                MapWinGIS.GeoProjection heToaDo =
                    new MapWinGIS.GeoProjection();

                if (!heToaDo.ImportFromEPSG(EpsgBanDo))
                {
                    throw new InvalidOperationException(
                        "Không thiết lập được EPSG:" + EpsgBanDo);
                }

                moi.GrabProjectionFromData = false;
                moi.GeoProjection = heToaDo;
                moi.ExtentsChanged += BanDo_ExtentsChanged;
                moi.MouseMoveEvent += BanDo_CapNhatToaDoDuoi_MouseMove;
                ketNoiMapWinGIS = new TroGiupMapWinGIS(moi);
                banDoMapWinGIS = moi;

                GanDieuKhienBanDo(moi, "EPSG:" + EpsgBanDo);
                pnlMapTools.BringToFront();

                GhiNhatKyBanDo(
                    "MAP_INIT_OK | EPSG=" + EpsgBanDo +
                    " | tiles=false");

                statusGeneral.Text =
                    "● MapWinGIS đã khởi tạo.";
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("MAP_INIT_FAILED | " + ex);

                if (moi != null)
                {
                    try
                    {
                        moi.ExtentsChanged -= BanDo_ExtentsChanged;
                    }
                    catch { }

                    try
                    {
                        pnlMapCanvas.Controls.Remove(moi);
                    }
                    catch { }

                    try
                    {
                        moi.Dispose();
                    }
                    catch { }
                }

                banDoMapWinGIS = null;
                ketNoiMapWinGIS = null;
                dieuKhienBanDo = null;

                lblMapHeaderTag.Text = "● Lỗi MapWinGIS";
                lblEmptyTitle.Text = "KHÔNG KHỞI TẠO ĐƯỢC MAPWINGIS";
                lblEmptyDescription.Text = ex.Message;
                pnlMapEmptyState.Visible = true;

                CanGiuaThongBao();

                statusGeneral.Text =
                    "● Lỗi MapWinGIS: " + ex.Message;
            }
        }

        private void BanDo_ExtentsChanged(object sender, EventArgs e)
        {
            DateTime hienTai = DateTime.UtcNow;

            if ((hienTai - lanGhiPhamViCuoi).TotalMilliseconds < 1000)
                return;

            lanGhiPhamViCuoi = hienTai;

            GhiNhatKyBanDo("EXTENTS_CHANGED");
        }

        private void GhiNhatKyBanDo(string noiDung)
        {
            try
            {
                string thuMuc =
                    Path.GetDirectoryName(tepNhatKyBanDo);

                if (!Directory.Exists(thuMuc))
                    Directory.CreateDirectory(thuMuc);

                File.AppendAllText(
                    tepNhatKyBanDo,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    " | " + noiDung + Environment.NewLine);

            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }

        public void GanDieuKhienBanDo(
            Control banDo,
            string heQuyChieu)
        {
            if (banDo == null)
                throw new ArgumentNullException("banDo");

            if (banDo.IsDisposed)
                throw new ObjectDisposedException("banDo");

            if (dieuKhienBanDo != null && dieuKhienBanDo != banDo)
            {
                throw new InvalidOperationException(
                    "Đã tồn tại một điều khiển bản đồ.");
            }

            dieuKhienBanDo = banDo;
            banDo.Dock = DockStyle.Fill;

            if (!pnlMapCanvas.Controls.Contains(banDo))
                pnlMapCanvas.Controls.Add(banDo);

            banDo.Visible = true;
            banDo.SendToBack();

            pnlMapTools.BringToFront();
            pnlMapEmptyState.Visible = false;

            statusProjection.Text = string.IsNullOrWhiteSpace(heQuyChieu)
                ? "CRS: Chưa xác định"
                : "CRS: " + heQuyChieu;

            CapNhatNhanBanDo();
        }

        private void CapNhatNhanBanDo()
        {
            lblMapHeading.Text = "KHÔNG GIAN BẢN ĐỒ";

            lblMapHeaderTag.Text = dieuKhienBanDo == null
                ? "● Chưa gắn MapWinGIS"
                : "● MapWinGIS đã khởi tạo";

            lblMapHeaderTag.ForeColor = dieuKhienBanDo == null
                ? Color.FromArgb(243, 186, 96)
                : Color.FromArgb(52, 211, 153);

            lblRightTag.Text = cacLop.Count + " lớp";

            lblMapFooter.Text = cacLop.Count == 0
                ? "Chưa có lớp dữ liệu; chọn LỚP + để nạp."
                : "Đã nạp " + cacLop.Count + " lớp bản đồ.";

            pnlMapEmptyState.Visible = dieuKhienBanDo == null;
        }

        private void KhoiTaoNutNapLop()
        {
            pnlMapTools.Height = 269;

            btnNapLop = new Button
            {
                Name = "btnNapLop",
                Text = "LỚP +",
                Width = 48,
                Height = 48,
                Left = 1,
                Top = 210,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold)
            };

            btnNapLop.FlatAppearance.BorderSize = 0;

            pnlMapTools.Controls.Add(btnNapLop);

            toolTip.SetToolTip(
                btnNapLop,
                "Nạp tệp Shapefile hoặc GeoTIFF");
        }

        private void KhoiTaoDanhSachLop()
        {
            pnlRightBody.Controls.Clear();

            Label tieuDe = new Label
            {
                Dock = DockStyle.Top,
                Height = 52,
                Text = "DANH SÁCH LỚP ĐÃ NẠP",
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                ForeColor = Color.White
            };

            danhSachLop = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(23, 37, 55),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            danhSachLop.ItemCheck += DanhSachLop_ItemCheck;

            Label ghiChu = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 78,

                Text = "Chọn LỚP + để nạp tệp .shp hoặc .tif. " +
                    "Tích vào ô để bật hoặc tắt lớp.",

                Padding = new Padding(16, 10, 12, 3),
                ForeColor = Color.FromArgb(148, 163, 184)
            };

            pnlRightBody.Controls.Add(danhSachLop);
            pnlRightBody.Controls.Add(ghiChu);
            pnlRightBody.Controls.Add(tieuDe);
        }
        private void NapLopTuTep()
        {
            if (ketNoiMapWinGIS == null)
            {
                MessageBox.Show(
                    this,
                    "MapWinGIS chưa được khởi tạo.",
                    "Bản đồ GIS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (dangThaoTacBanDo)
                return;

            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Chọn lớp dữ liệu GIS";

                dialog.Filter =
                    "Dữ liệu GIS (*.shp;*.tif;*.tiff)|*.shp;*.tif;*.tiff" +
                    "|Tất cả tệp (*.*)|*.*";

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                dangThaoTacBanDo = true;
                btnNapLop.Enabled = false;

                try
                {
                    GhiNhatKyBanDo(
                        "ADD_LAYER_BEGIN | " +
                        Path.GetFileName(dialog.FileName));

                    int handleDaThem =
                        ketNoiMapWinGIS.ThemLopTuTep(
                            dialog.FileName,
                            true);

                    LopMapItem lop = new LopMapItem
                    {
                        Handle = handleDaThem,

                        Ten = Path.GetFileNameWithoutExtension(
                            dialog.FileName)
                    };

                    cacLop.Add(lop);
                    danhSachLop.Items.Add(lop, true);

                    GhiNhatKyBanDo(
                        "ADD_LAYER_OK | handle=" + handleDaThem);

                    ketNoiMapWinGIS.PhongToanCanh();

                    CapNhatNhanBanDo();
                    TaiLaiDuLieuPhanHe();

                    statusGeneral.Text = "● Đã nạp lớp: " + lop.Ten;
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo("ADD_LAYER_FAILED | " + ex);

                    MessageBox.Show(
                        this,
                        "Không thể nạp hoặc hiển thị lớp:\n" + ex.Message,
                        "Lỗi lớp bản đồ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    dangThaoTacBanDo = false;

                    if (!IsDisposed && !Disposing)
                        btnNapLop.Enabled = true;
                }
            }
        }

        private void DanhSachLop_ItemCheck(
            object sender,
            ItemCheckEventArgs e)
        {
            if (ketNoiMapWinGIS == null ||
                danhSachLop == null ||
                dangCapNhatLop ||
                dangThaoTacBanDo ||
                e.Index < 0 ||
                e.Index >= danhSachLop.Items.Count)
            {
                return;
            }

            LopMapItem lop =
                danhSachLop.Items[e.Index] as LopMapItem;

            if (lop == null || e.NewValue == e.CurrentValue)
                return;

            bool hienThiMoi = e.NewValue == CheckState.Checked;
            bool daDoiLop = false;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(lop.Handle))
                {
                    throw new InvalidOperationException(
                        "Lớp không còn tồn tại trên bản đồ.");
                }

                bool hienThiCu =
                    ketNoiMapWinGIS.LayHienThiLop(lop.Handle);

                if (hienThiCu != hienThiMoi)
                {
                    ketNoiMapWinGIS.DatHienThiLop(
                        lop.Handle,
                        hienThiMoi);

                    daDoiLop = true;
                }
            }
            catch (Exception ex)
            {
                e.NewValue = e.CurrentValue;

                statusGeneral.Text =
                    "● Lỗi bật/tắt lớp: " + ex.Message;

                GhiNhatKyBanDo(
                    "LAYER_VISIBILITY_ERROR | " + ex);

                return;
            }

            if (!daDoiLop)
                return;

            Bd11_YeuCauVeLaiBanDo();
            HenCapNhatChuGiai();

            try
            {
                EventHandler handler = YeuCauThayDoiLop;

                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "LAYER_CHANGED_NOTIFY_ERROR | " + ex);
            }
        }

        private void DoiHienThiBangLop()
        {
            pnlRightCard.Visible = !pnlRightCard.Visible;

            tblContent.ColumnStyles[0].Width =
                pnlRightCard.Visible ? 71F : 100F;

            tblContent.ColumnStyles[1].Width =
                pnlRightCard.Visible ? 29F : 0F;
        }

        public void CapNhatTrangThaiPostGIS(bool? daKetNoi)
        {
            daKetNoiPostGIS = daKetNoi;

            string trangThai = !daKetNoi.HasValue
                ? "Chưa kiểm tra"
                : daKetNoi.Value
                ? "Đã kết nối"
                : "Không kết nối";

            lblHeaderDbStatus.Text = "● PostGIS: " + trangThai;
            lblConnectState.Text = "● " + trangThai;

            Color mau = !daKetNoi.HasValue
                ? Color.FromArgb(243, 186, 96)
                : daKetNoi.Value
                ? Color.FromArgb(52, 211, 153)
                : Color.FromArgb(248, 113, 113);

            lblHeaderDbStatus.ForeColor = mau;
            lblConnectState.ForeColor = mau;
        }

        private void BanDo_CapNhatToaDoDuoi_MouseMove(
            object sender,
            AxMapWinGIS._DMapEvents_MouseMoveEvent e)
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                statusCoordinates == null)
                return;

            // Giới hạn tần suất cập nhật để tránh giật.
            DateTime bayGio = DateTime.UtcNow;

            if ((bayGio - banDoLanCapNhatToaDoChuot)
                .TotalMilliseconds < 70)
                return;

            banDoLanCapNhatToaDoChuot = bayGio;

            double kinhDo = 0d;
            double viDo = 0d;

            // Chuyển tọa độ pixel sang WGS84.
            if (banDoMapWinGIS.PixelToDegrees(
                e.x,
                e.y,
                ref kinhDo,
                ref viDo) &&
                !double.IsNaN(kinhDo) &&
                !double.IsNaN(viDo) &&
                !double.IsInfinity(kinhDo) &&
                !double.IsInfinity(viDo) &&
                kinhDo >= -180d &&
                kinhDo <= 180d &&
                viDo >= -90d &&
                viDo <= 90d)
            {
                CapNhatToaDo(kinhDo, viDo);
            }
        }

        public void CapNhatToaDo(double kinhDo, double viDo)
        {
            statusCoordinates.Text = string.Format(
                CultureInfo.InvariantCulture,
                "WGS84: {0:0.000000}°E; {1:0.000000}°N",
                kinhDo,
                viDo);
        }

        public void BatDieuKhienLop(bool choPhep)
        {
            dangCapNhatLop = true;

            chkBorders.Enabled = choPhep;
            chkWater.Enabled = choPhep;
            chkRoads.Enabled = choPhep;
            chkUnits.Enabled = choPhep;

            dangCapNhatLop = false;
        }

        private void CanGiuaThongBao()
        {
            if (pnlMapCanvas == null || pnlMapEmptyState == null)
                return;

            pnlMapEmptyState.Left = Math.Max(
                5,
                (pnlMapCanvas.ClientSize.Width -
                 pnlMapEmptyState.Width) / 2);

            pnlMapEmptyState.Top = Math.Max(
                5,
                (pnlMapCanvas.ClientSize.Height -
                 pnlMapEmptyState.Height) / 2);
        }

        private sealed class LopMapItem
        {
            public int Handle { get; set; }
            public string Ten { get; set; }

            public override string ToString()
            {
                return Ten ?? string.Empty;
            }
        }
    }
}