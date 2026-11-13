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
        // 8. SAP XEP VA XOA LOP
        // =====================================================

        private void Bd11_SapXepRanhGioi()
        {
            if (ketNoiMapWinGIS == null)
                return;

            bool coTinh =
                bd11HandleTinh >= 0 &&
                ketNoiMapWinGIS.CoLop(bd11HandleTinh);

            bool coHuyen =
                bd11HandleHuyen >= 0 &&
                ketNoiMapWinGIS.CoLop(bd11HandleHuyen);

            if (coTinh && coHuyen)
            {
                int soBuoc = 0;

                while (soBuoc < 100)
                {
                    int viTriTinh =
                        ketNoiMapWinGIS.LayViTriLop(
                            bd11HandleTinh);

                    int viTriHuyen =
                        ketNoiMapWinGIS.LayViTriLop(
                            bd11HandleHuyen);

                    if (viTriTinh > viTriHuyen)
                        break;

                    ketNoiMapWinGIS.DuaLopLenMotBac(
                        bd11HandleTinh);

                    soBuoc++;
                }
            }

            DuaLopDiemLenTren();
        }

        private void Bd11_XoaRanhGioiCu(int handleCu)
        {
            if (ketNoiMapWinGIS.CoLop(handleCu))
                ketNoiMapWinGIS.XoaLop(handleCu);

            Bd11_XoaKhoiDanhSach(handleCu);
        }

        private void Bd11_XoaKhoiDanhSach(int handle)
        {
            bool trangThaiCu = dangCapNhatLop;
            dangCapNhatLop = true;

            try
            {
                for (int i = danhSachLop.Items.Count - 1;
                     i >= 0; i--)
                {
                    LopMapItem muc =
                        danhSachLop.Items[i] as LopMapItem;

                    if (muc != null && muc.Handle == handle)
                        danhSachLop.Items.RemoveAt(i);
                }

                cacLop.RemoveAll(
                    x => x != null && x.Handle == handle);
            }
            finally
            {
                dangCapNhatLop = trangThaiCu;
            }
        }

        // =====================================================
        // 9. LUU VA KHOI PHUC RANH GIOI
        // =====================================================

        private void Bd11_LuuDuongDanRanhGioi(
            bool laCapTinh, string duongDan)
        {
            try
            {
                Directory.CreateDirectory(Bd11_ThuMucCauHinh);

                string tep = laCapTinh
                    ? Bd11_TepRanhTinh
                    : Bd11_TepRanhHuyen;

                File.WriteAllText(tep, duongDan);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_SAVE_BOUNDARY_PATH | " + ex.Message);
            }
        }

        private void Bd11_KhoiPhucHaiLopRanhGioi()
        {
            Bd11_KhoiPhucMotLop(true);
            Bd11_KhoiPhucMotLop(false);
        }

        private void Bd11_KhoiPhucMotLop(bool laCapTinh)
        {
            try
            {
                int handle = laCapTinh
                    ? bd11HandleTinh
                    : bd11HandleHuyen;

                if (handle >= 0 &&
                    ketNoiMapWinGIS.CoLop(handle))
                    return;

                string tep = laCapTinh
                    ? Bd11_TepRanhTinh
                    : Bd11_TepRanhHuyen;

                if (!File.Exists(tep))
                    return;

                string duongDan = File.ReadAllText(tep).Trim();

                if (!File.Exists(duongDan))
                {
                    GhiNhatKyBanDo(
                        "BD11_BOUNDARY_MISSING | " + duongDan);
                    return;
                }

                Bd11_NapRanhGioi(laCapTinh, duongDan);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_BOUNDARY_RESTORE | " + ex.Message);
            }
        }

        private void Bd11_CapNhatMenuRanhGioi()
        {
            Bd11_CapNhatMenuMotCap(
                true, bd11MucHienTinh, bd11MucPhongTinh);

            Bd11_CapNhatMenuMotCap(
                false, bd11MucHienHuyen, bd11MucPhongHuyen);
        }

        private void Bd11_CapNhatMenuMotCap(
            bool laCapTinh,
            ToolStripMenuItem mucHien,
            ToolStripMenuItem mucPhong)
        {
            int handle = laCapTinh
                ? bd11HandleTinh
                : bd11HandleHuyen;

            bool coLop = false;

            try
            {
                coLop =
                    handle >= 0 &&
                    ketNoiMapWinGIS != null &&
                    ketNoiMapWinGIS.CoLop(handle);
            }
            catch
            {
                coLop = false;
            }

            if (mucHien != null)
            {
                mucHien.Enabled = coLop;
                bd11DangDongBoMenu = true;

                try
                {
                    mucHien.Checked =
                        coLop &&
                        ketNoiMapWinGIS.LayHienThiLop(handle);
                }
                finally
                {
                    bd11DangDongBoMenu = false;
                }
            }

            if (mucPhong != null)
                mucPhong.Enabled = coLop;
        }

        private void Bd11_DoiHienThiRanhGioi(bool laCapTinh)
        {
            if (ketNoiMapWinGIS == null)
                return;

            int handle = laCapTinh
                ? bd11HandleTinh
                : bd11HandleHuyen;

            ToolStripMenuItem muc = laCapTinh
                ? bd11MucHienTinh
                : bd11MucHienHuyen;

            if (handle < 0 || muc == null)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                    return;

                bool hienThi = muc.Checked;

                bool daDoi =
                    ketNoiMapWinGIS.LayHienThiLop(handle) !=
                    hienThi;

                if (daDoi)
                    ketNoiMapWinGIS.DatHienThiLop(
                        handle, hienThi);

                bool trangThaiCu = dangCapNhatLop;
                dangCapNhatLop = true;

                try
                {
                    for (int i = 0;
                         i < danhSachLop.Items.Count; i++)
                    {
                        LopMapItem item =
                            danhSachLop.Items[i] as LopMapItem;

                        if (item == null || item.Handle != handle)
                            continue;

                        if (danhSachLop.GetItemChecked(i) != hienThi)
                            danhSachLop.SetItemChecked(
                                i, hienThi);

                        break;
                    }
                }
                finally
                {
                    dangCapNhatLop = trangThaiCu;
                }

                if (daDoi)
                {
                    Bd11_YeuCauVeLaiBanDo();
                    HenCapNhatChuGiai();
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_BOUNDARY_VISIBILITY | " + ex.Message);
            }
        }

        private void Bd11_PhongDenRanhGioi(bool laCapTinh)
        {
            int handle = laCapTinh
                ? bd11HandleTinh
                : bd11HandleHuyen;

            try
            {
                if (ketNoiMapWinGIS == null ||
                    handle < 0 ||
                    !ketNoiMapWinGIS.CoLop(handle))
                    return;

                ketNoiMapWinGIS.PhongDenLop(handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this, ex.Message,
                    "Không phóng đến được lớp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // 10. AN / HIEN MAU NEN RASTER
        // =====================================================

        private void Bd11_DoiNenTrangRaster(bool bat)
        {
            if (bd11DangDoiNenTrang)
                return;

            bd11DangDoiNenTrang = true;

            try
            {
                if (handleAnhNenTuDong < 0 ||
                    ketNoiMapWinGIS == null ||
                    !ketNoiMapWinGIS.CoLop(handleAnhNenTuDong))
                    throw new InvalidOperationException(
                        "Chưa có lớp ảnh GeoTIFF.");

                GisImage anh =
                    banDoMapWinGIS.get_Image(handleAnhNenTuDong);

                if (anh == null)
                    throw new InvalidOperationException(
                        "Lớp nền không trả về Image.");

                if (bat)
                {
                    if (!bd11CoTrangThaiAnhGoc ||
                        bd11HandleAnhGoc != handleAnhNenTuDong)
                    {
                        bd11HandleAnhGoc = handleAnhNenTuDong;
                        bd11UseTransparencyGoc =
                            anh.UseTransparencyColor;
                        bd11MauAnhGoc1 = anh.TransparencyColor;
                        bd11MauAnhGoc2 = anh.TransparencyColor2;
                        bd11CoTrangThaiAnhGoc = true;
                    }

                    uint mauTrang = unchecked(
                        (uint)ColorTranslator.ToOle(Color.White));

                    anh.TransparencyColor = mauTrang;
                    anh.TransparencyColor2 = mauTrang;
                    anh.UseTransparencyColor = true;
                }
                else if (bd11CoTrangThaiAnhGoc &&
                         bd11HandleAnhGoc == handleAnhNenTuDong)
                {
                    anh.TransparencyColor = bd11MauAnhGoc1;
                    anh.TransparencyColor2 = bd11MauAnhGoc2;
                    anh.UseTransparencyColor =
                        bd11UseTransparencyGoc;
                }

                Bd11_YeuCauVeLaiBanDo();

                statusGeneral.Text = bat
                    ? "● Đã thử làm trong suốt màu trắng."
                    : "● Đã khôi phục màu raster.";
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_RASTER_TRANSPARENCY | " + ex);

                if (bat && bd11MucNenTrang != null)
                    bd11MucNenTrang.Checked = false;

                statusGeneral.Text =
                    "● Lỗi màu nền: " + ex.Message;
            }
            finally
            {
                bd11DangDoiNenTrang = false;
            }
        }

        private void Bd11_DatMauNenBanDo()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            Color mauNen = Color.FromArgb(25, 37, 49);

            if (banDoMapWinGIS.BackColor != mauNen)
                banDoMapWinGIS.BackColor = mauNen;

            if (pnlMapCanvas != null &&
                !pnlMapCanvas.IsDisposed &&
                pnlMapCanvas.BackColor != mauNen)
                pnlMapCanvas.BackColor = mauNen;
        }

        private void Bd11_KiemTraRasterMotLan()
        {
            if (handleAnhNenTuDong < 0 ||
                handleAnhNenTuDong == bd11HandleRasterDaKiemTra ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !ketNoiMapWinGIS.CoLop(handleAnhNenTuDong))
                return;

            try
            {
                GisImage anh =
                    banDoMapWinGIS.get_Image(handleAnhNenTuDong);

                if (anh == null)
                    return;

                bd11HandleRasterDaKiemTra = handleAnhNenTuDong;

                GhiNhatKyBanDo(
                    "BD11_RASTER_OVERVIEWS | " +
                    anh.NumOverviews + " | " + anh.Filename);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_RASTER_INFO_FAILED | " + ex.Message);
            }
        }

        private void Bd11_ThongTinHieuNangRaster()
        {
            if (handleAnhNenTuDong < 0 ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !ketNoiMapWinGIS.CoLop(handleAnhNenTuDong))
            {
                MessageBox.Show(
                    this,
                    "Chưa có ảnh nền GeoTIFF đang nạp.",
                    "Kiểm tra hiệu năng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                GisImage anh =
                    banDoMapWinGIS.get_Image(handleAnhNenTuDong);

                if (anh == null)
                    return;

                string duongDan = anh.Filename ?? "";
                int soMuc = anh.NumOverviews;

                string noiDung =
                    "Ảnh nền: " + duongDan + "\r\n" +
                    "Số mức ảnh tổng quan: " +
                    soMuc + "\r\n\r\n";

                if (soMuc == 0)
                {
                    noiDung +=
                        "Ảnh chưa có mức tổng quan.\r\n" +
                        "Điều này có thể làm chậm quá trình thu/phóng.\r\n\r\n" +
                        "Đóng ứng dụng rồi chạy trong OSGeo4W Shell:\r\n\r\n" +
                        "gdaladdo -ro -r average \"" +
                        duongDan + "\" 2 4 8 16 32\r\n\r\n" +
                        "Sau đó mở lại ứng dụng.";
                }
                else
                {
                    noiDung +=
                        "Ảnh đã có mức tổng quan.\r\n" +
                        "Nếu vẫn chậm, cần kiểm tra kích thước raster, " +
                        "ổ đĩa và phương thức đọc ảnh.";
                }

                MessageBox.Show(
                    this, noiDung,
                    "Tình trạng ảnh vệ tinh",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_RASTER_CHECK_ERROR | " + ex);

                MessageBox.Show(
                    this, ex.Message,
                    "Không đọc được thông tin raster",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // 11. KY HIEU VA NHAN DON VI
        // =====================================================

        private void Bd11_LamDepKyHieuDonVi(Shapefile sf)
        {
            if (sf == null ||
                bd11HandleKyHieu == handleLopDiemPostGIS ||
                sf.DefaultDrawingOptions == null)
                return;

            try
            {
                ShapeDrawingOptions kieu = sf.DefaultDrawingOptions;

                kieu.PointType =
                    tkPointSymbolType.ptSymbolStandard;
                kieu.PointShape =
                    tkPointShapeType.ptShapeStar;
                kieu.PointSidesCount = 5;
                kieu.PointSize = 12F;

                kieu.FillColor = unchecked(
                    (uint)ColorTranslator.ToOle(
                        Color.FromArgb(18, 181, 146)));

                kieu.LineVisible = true;

                kieu.LineColor = unchecked(
                    (uint)ColorTranslator.ToOle(
                        Color.FromArgb(250, 206, 79)));

                kieu.LineWidth = 1.5F;
                bd11HandleKyHieu = handleLopDiemPostGIS;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_MARKER_ERROR | " + ex.Message);
            }
        }

        private void Bd11_DatLaiCacheNhan()
        {
            bd11HandleNhan = -1;
            bd11SoDiemNhan = -1;
            bd11HandleKyHieu = -1;
            bd11NhanCanKiemTra = true;

            uiHandleNhanDaTao = -1;
        }

        private int Bd11_TimCotTen(Shapefile sf)
        {
            string[] tenCot =
            {
                "TEN_DON_VI", "TEN_DV", "TEN_DONVI",
                "TenDonVi", "TEN_DAY_DU", "TEN_DVI"
            };

            for (int j = 0; j < tenCot.Length; j++)
            {
                for (int i = 0; i < sf.NumFields; i++)
                {
                    MapWinGIS.Field truong = sf.get_Field(i);

                    if (truong != null &&
                        string.Equals(
                            truong.Name,
                            tenCot[j],
                            StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }

            return -1;
        }
    }
}