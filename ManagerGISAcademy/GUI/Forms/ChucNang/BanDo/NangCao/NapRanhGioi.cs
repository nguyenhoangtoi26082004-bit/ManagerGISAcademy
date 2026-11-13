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
        // 5. THANH ZOOM DOC DUOI 5 NUT
        // =====================================================

        private void Bd11_TaoZoomDoc()
        {
            // Chi tao mot thanh zoom, giu TrackBar hien co.
            bd11KhungZoom = new Panel
            {
                Name = "bd11KhungZoom",
                Size = new Size(49, 155),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Visible = true
            };

            WinLabel dau = new WinLabel
            {
                Text = "+",
                Dock = DockStyle.Top,
                Height = 18,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(31, 56, 47)
            };

            WinLabel cuoi = new WinLabel
            {
                Text = "−",
                Dock = DockStyle.Bottom,
                Height = 18,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(31, 56, 47)
            };

            bd11Zoom = new TrackBar
            {
                Orientation = Orientation.Vertical,
                Minimum = 0,
                Maximum = 10,
                Value = 5,
                TickStyle = TickStyle.None,
                Size = new Size(38, 102),
                Location = new WinPoint(3, 19),
                BackColor = bd11KhungZoom.BackColor
            };

            bd11ZoomCu = bd11Zoom.Value;

            bd11HenZoomDoc = new System.Windows.Forms.Timer
            {
                Interval = 180
            };

            bd11HenZoomDoc.Tick += Bd11_ZoomDocHenGio;

            bd11Zoom.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    bd11DangKeoZoom = true;
                    bd11HenZoomDoc.Stop();
                }
            };

            bd11Zoom.ValueChanged += (s, e) =>
            {
                if (!bd11DaKhoiTao || bd11DangDongBoZoom)
                    return;

                bd11HenZoomDoc.Stop();

                if (!bd11DangKeoZoom)
                    bd11HenZoomDoc.Start();
            };

            bd11Zoom.MouseUp += (s, e) =>
            {
                if (e.Button != MouseButtons.Left)
                    return;

                bd11DangKeoZoom = false;
                Bd11_ChotZoomDoc();
            };

            bd11Zoom.MouseCaptureChanged += (s, e) =>
            {
                if (!bd11DangKeoZoom)
                    return;

                bd11DangKeoZoom = false;
                Bd11_ChotZoomDoc();
            };

            bd11Zoom.KeyUp +=
                (s, e) => Bd11_ChotZoomDoc();

            bd11Zoom.Leave += (s, e) =>
            {
                if (!bd11DangKeoZoom)
                    Bd11_ChotZoomDoc();
            };

            bd11KhungZoom.Controls.Add(bd11Zoom);
            bd11KhungZoom.Controls.Add(dau);
            bd11KhungZoom.Controls.Add(cuoi);

            pnlMapCanvas.Controls.Add(bd11KhungZoom);
            Bd11_DatZoomBenDuoi();
        }

        /// <summary>
        /// Can thanh zoom doc ngay duoi 5 nut cong cu.
        /// Khi resize, tu tinh lai toa do va chieu cao.
        /// </summary>
        private void Bd11_DatZoomBenDuoi()
        {
            if (bd11KhungZoom == null ||
                bd11KhungZoom.IsDisposed ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed ||
                pnlMapTools == null ||
                pnlMapTools.IsDisposed)
                return;

            int trai = pnlMapTools.Left;
            int tren = pnlMapTools.Bottom + 12;

            int choTrong =
                pnlMapCanvas.ClientSize.Height - tren - 12;

            // Khong de zoom bi cat neu cua so qua thap.
            if (choTrong < 110)
            {
                bd11KhungZoom.Visible = false;
                return;
            }

            int cao = Math.Min(155, choTrong);

            bd11KhungZoom.SetBounds(
                trai, tren, pnlMapTools.Width, cao);

            if (bd11Zoom != null && !bd11Zoom.IsDisposed)
            {
                bd11Zoom.SetBounds(
                    3, 19, 38, Math.Max(52, cao - 37));
            }

            bd11KhungZoom.Visible = true;
            bd11KhungZoom.BringToFront();
        }

        private void Bd11_ZoomDocHenGio(
            object sender, EventArgs e)
        {
            if (bd11HenZoomDoc != null)
                bd11HenZoomDoc.Stop();

            if (!bd11DangKeoZoom)
                Bd11_ChotZoomDoc();
        }

        private void Bd11_ChotZoomDoc()
        {
            if (bd11HenZoomDoc != null)
                bd11HenZoomDoc.Stop();

            if (!bd11DaKhoiTao ||
                bd11DangDongBoZoom ||
                bd11Zoom == null ||
                bd11Zoom.IsDisposed ||
                IsDisposed ||
                Disposing)
                return;

            int soBuoc = bd11Zoom.Value - bd11ZoomCu;

            if (soBuoc == 0)
                return;

            bool daTiepNhan =
                BanDo_YeuCauZoomBuoc(soBuoc, true);

            if (daTiepNhan)
            {
                bd11ZoomCu = bd11Zoom.Value;
            }
            else
            {
                bd11DangDongBoZoom = true;

                try
                {
                    bd11Zoom.Value = Math.Max(
                        bd11Zoom.Minimum,
                        Math.Min(bd11Zoom.Maximum, bd11ZoomCu));
                }
                finally
                {
                    bd11DangDongBoZoom = false;
                }
            }
        }

        // =====================================================
        // 6. MENU DO DAC VA MENU LOP
        // =====================================================

        private void Bd11_TaoMenu()
        {
            bd11MenuDo = new ContextMenuStrip
            {
                BackColor = bd11Nen,
                ForeColor = Color.White,
                ShowImageMargin = false
            };

            bd11MenuDo.Items.Add(
                "Đo khoảng cách", null,
                (s, e) => Bd11_BatDo(1));

            bd11MenuDo.Items.Add(
                "Đo diện tích", null,
                (s, e) => Bd11_BatDo(2));

            bd11MenuDo.Items.Add(new ToolStripSeparator());

            bd11MenuDo.Items.Add(
                "Hoàn tất", null,
                (s, e) => HoanTatPhepDo());

            bd11MenuDo.Items.Add(
                "Xóa phép đo", null,
                (s, e) => XoaPhepDoHienTai());

            bd11MenuLop = new ContextMenuStrip
            {
                BackColor = bd11Nen,
                ForeColor = Color.White,
                ShowImageMargin = false
            };

            bd11MenuLop.Items.Add(
                "Mở / đóng bảng lớp", null,
                (s, e) => DoiHienThiBangLop());

            bd11MenuLop.Items.Add(
                "Nạp lớp dữ liệu...", null,
                (s, e) => NapLopTuTep());

            bd11MenuLop.Items.Add(new ToolStripSeparator());

            bd11MucHienNhan = new ToolStripMenuItem(
                "Hiện tên đơn vị")
            {
                CheckOnClick = true,
                Checked = true
            };

            bd11MucHienNhan.CheckedChanged += (s, e) =>
            {
                bd11HienNhan = bd11MucHienNhan.Checked;

                if (banDoMapWinGIS == null ||
                    banDoMapWinGIS.IsDisposed ||
                    handleLopDiemPostGIS < 0)
                    return;

                Shapefile sf =
                    banDoMapWinGIS.get_Shapefile(
                        handleLopDiemPostGIS);

                if (sf != null &&
                    sf.Labels != null &&
                    sf.Labels.Visible != bd11HienNhan)
                {
                    sf.Labels.Visible = bd11HienNhan;
                    Bd11_YeuCauVeLaiBanDo();
                }
            };

            bd11MenuLop.Items.Add(bd11MucHienNhan);

            bd11MenuLop.Items.Add(
                "Làm mới nhãn đơn vị", null,
                (s, e) =>
                {
                    Bd11_DatLaiCacheNhan();
                    Bd11_GanNhanDonVi();
                });

            bd11MenuLop.Items.Add(
                "Kiểm tra hiệu năng ảnh nền", null,
                (s, e) => Bd11_ThongTinHieuNangRaster());

            bd11MenuLop.Items.Add(new ToolStripSeparator());

            bd11MenuLop.Items.Add(
                "Nạp ranh giới Hà Nội (.shp)...", null,
                (s, e) => Bd11_ChonRanhGioi(true));

            bd11MucHienTinh = new ToolStripMenuItem(
                "Hiện ranh giới Hà Nội")
            {
                CheckOnClick = true,
                Checked = false,
                Enabled = false
            };

            bd11MucHienTinh.CheckedChanged += (s, e) =>
            {
                if (!bd11DangDongBoMenu)
                    Bd11_DoiHienThiRanhGioi(true);
            };

            bd11MenuLop.Items.Add(bd11MucHienTinh);

            bd11MucPhongTinh = new ToolStripMenuItem(
                "Phóng đến ranh giới Hà Nội")
            {
                Enabled = false
            };

            bd11MucPhongTinh.Click +=
                (s, e) => Bd11_PhongDenRanhGioi(true);

            bd11MenuLop.Items.Add(bd11MucPhongTinh);
            bd11MenuLop.Items.Add(new ToolStripSeparator());

            bd11MenuLop.Items.Add(
                "Nạp ranh giới quận/huyện (.shp)...", null,
                (s, e) => Bd11_ChonRanhGioi(false));

            bd11MucHienHuyen = new ToolStripMenuItem(
                "Hiện ranh giới quận/huyện")
            {
                CheckOnClick = true,
                Checked = false,
                Enabled = false
            };

            bd11MucHienHuyen.CheckedChanged += (s, e) =>
            {
                if (!bd11DangDongBoMenu)
                    Bd11_DoiHienThiRanhGioi(false);
            };

            bd11MenuLop.Items.Add(bd11MucHienHuyen);

            bd11MucPhongHuyen = new ToolStripMenuItem(
                "Phóng đến lớp quận/huyện")
            {
                Enabled = false
            };

            bd11MucPhongHuyen.Click +=
                (s, e) => Bd11_PhongDenRanhGioi(false);

            bd11MenuLop.Items.Add(bd11MucPhongHuyen);
            bd11MenuLop.Items.Add(new ToolStripSeparator());

            bd11MucNenTrang = new ToolStripMenuItem(
                "Thử ẩn màu trắng ảnh vệ tinh")
            {
                CheckOnClick = true,
                Checked = false
            };

            bd11MucNenTrang.CheckedChanged += (s, e) =>
            {
                if (!bd11DangDoiNenTrang)
                    Bd11_DoiNenTrangRaster(
                        bd11MucNenTrang.Checked);
            };

            bd11MenuLop.Items.Add(bd11MucNenTrang);

            bd11MenuLop.Opening +=
                (s, e) => Bd11_CapNhatMenuRanhGioi();

            btnMapLayers.ContextMenuStrip = bd11MenuLop;

            if (bd11BieuTuongLop != null)
                bd11BieuTuongLop.ContextMenuStrip = bd11MenuLop;
        }

        // =====================================================
        // 7. NAP SHAPEFILE RANH GIOI
        // =====================================================

        private void Bd11_ChonRanhGioi(bool laCapTinh)
        {
            using (OpenFileDialog hopThoai = new OpenFileDialog())
            {
                hopThoai.Title = laCapTinh
                    ? "Chọn ranh giới Hà Nội"
                    : "Chọn ranh giới quận/huyện";

                hopThoai.Filter = "Shapefile (*.shp)|*.shp";
                hopThoai.Multiselect = false;
                hopThoai.CheckFileExists = true;

                if (hopThoai.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    string duongDan = hopThoai.FileName;

                    Bd11_NapRanhGioi(laCapTinh, duongDan);
                    Bd11_LuuDuongDanRanhGioi(
                        laCapTinh, duongDan);
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "BD11_BOUNDARY_LOAD_ERROR | " + ex);

                    MessageBox.Show(
                        this, ex.Message,
                        "Không nạp được ranh giới",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void Bd11_KiemTraShapefile(string duongDan)
        {
            if (!File.Exists(duongDan))
                throw new FileNotFoundException(
                    "Không tìm thấy Shapefile.", duongDan);

            string[] phanMoRong =
            {
                ".shp", ".shx", ".dbf", ".prj"
            };

            foreach (string duoi in phanMoRong)
            {
                string tep = Path.ChangeExtension(duongDan, duoi);

                if (!File.Exists(tep))
                    throw new FileNotFoundException(
                        "Thiếu thành phần Shapefile: " +
                        Path.GetFileName(tep));
            }

            string prj = Path.ChangeExtension(
                duongDan, ".prj");

            GeoProjection nguon = new GeoProjection();

            if (!nguon.ReadFromFile(prj) || nguon.IsEmpty)
                throw new InvalidOperationException(
                    "Không đọc được hệ tọa độ từ " +
                    Path.GetFileName(prj));

            GeoProjection mongDoi = new GeoProjection();

            if (!mongDoi.ImportFromEPSG(32648))
                throw new InvalidOperationException(
                    "Không khởi tạo được EPSG:32648.");

            if (!nguon.get_IsSame(mongDoi))
                throw new InvalidOperationException(
                    "Hệ tọa độ Shapefile chưa khớp EPSG:32648.\r\n\r\n" +
                    "Hệ tọa độ đọc được: " + nguon.Name +
                    "\r\n\r\nHãy dùng QGIS xuất lại lớp sang " +
                    "EPSG:32648 trước khi nạp.");
        }

        private void Bd11_NapRanhGioi(
            bool laCapTinh, string duongDan)
        {
            if (ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                throw new InvalidOperationException(
                    "MapWinGIS chưa sẵn sàng.");

            Bd11_KiemTraShapefile(duongDan);

            int handleMoi = -1;
            int handleCu = laCapTinh
                ? bd11HandleTinh
                : bd11HandleHuyen;

            bool daDangKyLop = false;
            bool daChotLop = false;

            try
            {
                handleMoi =
                    ketNoiMapWinGIS.ThemLopTuTep(duongDan, true);

                Shapefile shp =
                    banDoMapWinGIS.get_Shapefile(handleMoi);

                if (shp == null)
                    throw new InvalidOperationException(
                        "Không đọc được dữ liệu Shapefile.");

                string loai = shp.ShapefileType.ToString();

                if (loai.IndexOf(
                    "POLYGON",
                    StringComparison.OrdinalIgnoreCase) < 0)
                    throw new InvalidOperationException(
                        "Lớp ranh giới phải có kiểu hình học Polygon.");

                if (shp.NumShapes <= 0)
                    throw new InvalidOperationException(
                        "Shapefile không có đối tượng.");

                if (shp.GeoProjection == null ||
                    shp.GeoProjection.IsEmpty)
                    throw new InvalidOperationException(
                        "Lớp sau khi nạp không có CRS.");

                GeoProjection chuan = new GeoProjection();

                if (!chuan.ImportFromEPSG(32648) ||
                    !shp.GeoProjection.get_IsSame(chuan))
                    throw new InvalidOperationException(
                        "CRS của lớp sau khi nạp không khớp EPSG:32648.");

                Bd11_DinhDangRanhGioi(shp, laCapTinh);

                LopMapItem mucMoi = new LopMapItem
                {
                    Handle = handleMoi,
                    Ten = laCapTinh
                        ? "Ranh giới Hà Nội"
                        : "Ranh giới quận/huyện"
                };

                bool trangThaiCu = dangCapNhatLop;
                dangCapNhatLop = true;

                try
                {
                    cacLop.Add(mucMoi);
                    danhSachLop.Items.Add(mucMoi, true);
                    daDangKyLop = true;
                }
                finally
                {
                    dangCapNhatLop = trangThaiCu;
                }

                if (laCapTinh)
                    bd11HandleTinh = handleMoi;
                else
                    bd11HandleHuyen = handleMoi;

                daChotLop = true;

                if (handleCu >= 0 && handleCu != handleMoi)
                {
                    try
                    {
                        Bd11_XoaRanhGioiCu(handleCu);
                    }
                    catch (Exception exCu)
                    {
                        GhiNhatKyBanDo(
                            "BD11_REMOVE_OLD_BOUNDARY | " +
                            exCu.Message);
                    }
                }

                try
                {
                    Bd11_SapXepRanhGioi();
                    Bd11_CapNhatMenuRanhGioi();
                    Bd11_YeuCauVeLaiBanDo();
                    HenCapNhatChuGiai();
                }
                catch (Exception exVe)
                {
                    GhiNhatKyBanDo(
                        "BD11_BOUNDARY_DISPLAY | " +
                        exVe.Message);
                }

                statusGeneral.Text = laCapTinh
                    ? "● Đã nạp ranh giới Hà Nội."
                    : "● Đã nạp ranh giới quận/huyện.";

                GhiNhatKyBanDo(
                    "BD11_BOUNDARY_LOADED | " + duongDan);
            }
            catch
            {
                if (!daChotLop && handleMoi >= 0)
                {
                    if (daDangKyLop)
                        Bd11_XoaKhoiDanhSach(handleMoi);

                    try
                    {
                        if (ketNoiMapWinGIS.CoLop(handleMoi))
                            ketNoiMapWinGIS.XoaLop(handleMoi);
                    }
                    catch (Exception ex)
                    {
                        GhiNhatKyBanDo(
                            "BD11_BOUNDARY_ROLLBACK | " +
                            ex.Message);
                    }
                }

                throw;
            }
        }

        private void Bd11_DinhDangRanhGioi(
            Shapefile shp, bool laCapTinh)
        {
            if (shp == null)
                return;

            Color mau = laCapTinh
                ? bd11MauTinh
                : bd11MauHuyen;

            float doDay = laCapTinh ? 2.5F : 1.2F;

            tkDashStyle net = laCapTinh
                ? tkDashStyle.dsSolid
                : tkDashStyle.dsDash;

            Bd11_GanKieuDuong(
                shp.DefaultDrawingOptions, mau, doDay, net);

            if (shp.Categories != null)
            {
                for (int i = 0; i < shp.Categories.Count; i++)
                {
                    ShapefileCategory nhom =
                        shp.Categories.get_Item(i);

                    if (nhom != null &&
                        nhom.DrawingOptions != null)
                    {
                        Bd11_GanKieuDuong(
                            nhom.DrawingOptions, mau, doDay, net);
                    }
                }
            }
        }

        private void Bd11_GanKieuDuong(
            ShapeDrawingOptions kieu,
            Color mau,
            float doDay,
            tkDashStyle kieuNet)
        {
            if (kieu == null)
                return;

            kieu.FillVisible = false;
            kieu.LineVisible = true;

            kieu.LineColor = unchecked(
                (uint)ColorTranslator.ToOle(mau));

            kieu.LineWidth = doDay;
            kieu.LineTransparency = 255F;
            kieu.LineStipple = kieuNet;
        }
    }
}