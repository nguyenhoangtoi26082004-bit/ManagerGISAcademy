
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using AxMapWinGIS;
using MapWinGIS;

namespace ManagerGISAcademy.GIS
{
    /// <summary>
    /// Cau noi MapWinGIS cho WinForms.
    /// Giai doan 4.3: toi uu tuong tac ban do.
    /// .NET Framework 4.8, C# 7.3, COM x86.
    ///
    /// - Seamless pan.
    /// - Khong animation khi zoom.
    /// - Tai su dung tile buffer neu API ho tro.
    /// - Zoom wheel buoc nho hon khi nap GeoTIFF.
    /// - Raster buffer 70% cho anh lon.
    /// - Giu GDAL cache, khong tao overviews tren UI.
    /// - Giu nguyen API public cua file cu.
    /// </summary>
    public sealed class TroGiupMapWinGIS :
        IGiaoTiepLopBanDo,
        IGiaoTiepThuTuLopBanDo
    {
        private readonly AxMap banDo;
        private readonly int maLuongGiaoDien;

        private const bool KeoBanDoLienMach = true;
        private const int TyLeBoDemRasterLon = 70;
        private const int NguongAnhLonTheoChieuRong = 16000;
        private const bool UuTienTocDoRaster = true;
        private const bool HienThoiGianVe = false;
        private const double TocDoLanChuotRaster = 0.94d;
        private const long NguongChamMs = 150;

        // ==================================================
        // 1. KHOI TAO
        // ==================================================

        public TroGiupMapWinGIS(AxMap dieuKhienBanDo)
        {
            if (dieuKhienBanDo == null)
                throw new ArgumentNullException("dieuKhienBanDo");

            if (dieuKhienBanDo.IsDisposed ||
                dieuKhienBanDo.Disposing)
                throw new ObjectDisposedException("dieuKhienBanDo");

            if (!dieuKhienBanDo.IsHandleCreated ||
                dieuKhienBanDo.InvokeRequired)
                throw new InvalidOperationException(
                    "AxMap phải khởi tạo trên luồng WinForms.");

            banDo = dieuKhienBanDo;
            maLuongGiaoDien =
                Thread.CurrentThread.ManagedThreadId;

            banDo.UseSeamlessPan = KeoBanDoLienMach;
            banDo.AnimationOnZooming = tkCustomState.csFalse;
            banDo.InertiaOnPanning = tkCustomState.csFalse;
            banDo.DisableWaitCursor = true;
            banDo.ShowRedrawTime = HienThoiGianVe;

            CauHinhBoDemTile();

            Debug.WriteLine(
                "[MapWinGIS] PROFILE | seamlessPan=" +
                banDo.UseSeamlessPan +
                " | rasterBuffer=" + TyLeBoDemRasterLon +
                "% | fastSampling=" + UuTienTocDoRaster);
        }

        public AxMap DieuKhienBanDo
        {
            get { return banDo; }
        }

        public int SoLopTrenBanDo
        {
            get
            {
                KiemTraSanSang();
                return banDo.NumLayers;
            }
        }

        // ==================================================
        // 2. NAP LOP TU TEP
        // ==================================================

        public int ThemLopTuTep(
            string duongDanDayDu,
            bool hienThi)
        {
            KiemTraSanSang();

            if (string.IsNullOrWhiteSpace(duongDanDayDu))
                throw new ArgumentException(
                    "Chưa chọn tệp bản đồ.", "duongDanDayDu");

            string tep = Path.GetFullPath(duongDanDayDu.Trim());

            if (!File.Exists(tep))
                throw new FileNotFoundException(
                    "Không tìm thấy tệp bản đồ.", tep);

            string moRong = Path.GetExtension(tep).ToLowerInvariant();

            bool laRaster =
                moRong == ".tif" || moRong == ".tiff";

            if (!laRaster && moRong != ".shp")
                throw new NotSupportedException(
                    "Chỉ hỗ trợ .shp, .tif và .tiff.");

            if (moRong == ".shp")
            {
                KiemTraTapTinKemTheo(tep, ".shx");
                KiemTraTapTinKemTheo(tep, ".dbf");
            }

            Stopwatch dongHo = Stopwatch.StartNew();
            int handle = -1;
            bool tuKhoaBanDo = false;
            Exception loi = null;

            try
            {
                if (banDo.IsLocked == tkLockMode.lmUnlock)
                {
                    banDo.LockWindow(tkLockMode.lmLock);
                    tuKhoaBanDo = true;
                }

                handle = banDo.AddLayerFromFilename(
                    tep,
                    tkFileOpenStrategy.fosAutoDetect,
                    hienThi);

                if (handle < 0)
                    throw new InvalidOperationException(
                        "Không mở được '" + Path.GetFileName(tep) +
                        "'. " + LayLoiBanDo());

                if (!CoLopNoiBo(handle))
                    throw new InvalidOperationException(
                        "LayerHandle không hợp lệ: " + handle);

                if (laRaster)
                {
                    DuaLopXuongDuoi(handle);
                    CauHinhCacheRaster(handle);

                    // Chi thiet lap sau khi raster mo thanh cong.
                    // Khong chen MouseWheel handler rieng.
                    CauHinhLanChuotRaster();
                }

                return handle;
            }
            catch (Exception ex)
            {
                loi = ex;

                if (handle >= 0)
                {
                    try
                    {
                        if (CoLopNoiBo(handle))
                            banDo.RemoveLayer(handle);
                    }
                    catch (Exception exXoa)
                    {
                        Debug.WriteLine(
                            "[MapWinGIS] ROLLBACK_REMOVE_ERROR | " +
                            exXoa.Message);
                    }
                }

                if (ex is COMException)
                    throw new InvalidOperationException(
                        "Lỗi COM khi nạp '" +
                        Path.GetFileName(tep) +
                        "': " + ex.Message, ex);

                throw;
            }
            finally
            {
                try
                {
                    if (tuKhoaBanDo &&
                        !banDo.IsDisposed &&
                        !banDo.Disposing)
                    {
                        banDo.LockWindow(tkLockMode.lmUnlock);
                    }
                }
                finally
                {
                    dongHo.Stop();

                    Debug.WriteLine(
                        "[MapWinGIS] LOAD | " +
                        Path.GetFileName(tep) +
                        " | ms=" + dongHo.ElapsedMilliseconds +
                        " | handle=" + handle +
                        " | " + (loi == null ? "OK" : loi.Message));

                    if (laRaster)
                        GhiChanDoanBoNho();
                }
            }
        }

        // ==================================================
        // 3. CAU HINH RASTER
        // ==================================================

        private void CauHinhCacheRaster(int layerHandle)
        {
            MapWinGIS.Image anh = banDo.get_Image(layerHandle);

            if (anh == null)
            {
                Debug.WriteLine(
                    "[MapWinGIS] RASTER_IMAGE_NULL | " + layerHandle);
                return;
            }

            // Giu GDAL cache giua cac lan ve.
            try
            {
                anh.ClearGDALCache = false;
            }
            catch (COMException ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] GDAL_CACHE_ERROR | " + ex.Message);
            }

            // Giu profile BufferSize=70% da thu nghiem.
            try
            {
                int tyLe =
                    anh.OriginalWidth >= NguongAnhLonTheoChieuRong
                    ? TyLeBoDemRasterLon : 100;

                if (anh.BufferSize != tyLe)
                    anh.BufferSize = tyLe;

                Debug.WriteLine(
                    "[MapWinGIS] RASTER_BUFFER | requested=" +
                    tyLe + "% | actual=" + anh.BufferSize + "%");
            }
            catch (COMException ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] RASTER_BUFFER_ERROR | " + ex.Message);
            }

            // Lay mau uu tien toc do.
            try
            {
                tkInterpolationMode cheDo =
                    UuTienTocDoRaster
                    ? tkInterpolationMode.imNone
                    : tkInterpolationMode.imBilinear;

                anh.UpsamplingMode = cheDo;
                anh.DownsamplingMode = cheDo;

                Debug.WriteLine(
                    "[MapWinGIS] SAMPLING | up=" +
                    anh.UpsamplingMode +
                    " | down=" + anh.DownsamplingMode);
            }
            catch (COMException ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] SAMPLING_ERROR | " + ex.Message);
            }

            // Chan doan pyramid/overviews.
            try
            {
                Debug.WriteLine(
                    "[MapWinGIS] RASTER | overviews=" +
                    anh.NumOverviews +
                    " | px=" + anh.OriginalWidth +
                    "x" + anh.OriginalHeight +
                    " | source=" + anh.SourceType);

                if (anh.NumOverviews == 0)
                {
                    Debug.WriteLine(
                        "[MapWinGIS] WARNING | " +
                        "GeoTIFF chưa có overviews. " +
                        "Nên tạo pyramid bằng GDAL khi " +
                        "ứng dụng đã đóng.");
                }
            }
            catch (COMException ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] RASTER_INFO_ERROR | " + ex.Message);
            }
        }

        // ==================================================
        // 4. TAI SU DUNG BO DEM TILE
        // ==================================================

        private void CauHinhBoDemTile()
        {
            try
            {
                // ReuseTileBuffer co trong API MapWinGIS moi.
                // Reflection de khong yeu cau AxInterop cu phai
                // khai bao thuoc tinh nay luc bien dich.
                System.Reflection.PropertyInfo thuocTinh =
                    banDo.GetType().GetProperty("ReuseTileBuffer");

                if (thuocTinh == null ||
                    !thuocTinh.CanWrite ||
                    thuocTinh.PropertyType != typeof(bool))
                {
                    Debug.WriteLine(
                        "[MapWinGIS] TILE_BUFFER | unsupported");
                    return;
                }

                thuocTinh.SetValue(banDo, true, null);

                Debug.WriteLine(
                    "[MapWinGIS] TILE_BUFFER | reuse=true");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] TILE_BUFFER_FAILED | " +
                    ex.GetType().Name);
            }
        }

        // ==================================================
        // 5. BUOC ZOOM BANG BANH XE CHUOT
        // ==================================================

        private void CauHinhLanChuotRaster()
        {
            try
            {
                if (Math.Abs(
                    banDo.MouseWheelSpeed -
                    TocDoLanChuotRaster) > 0.0001d)
                {
                    banDo.MouseWheelSpeed =
                        TocDoLanChuotRaster;
                }

                Debug.WriteLine(
                    "[MapWinGIS] RASTER_WHEEL | speed=" +
                    banDo.MouseWheelSpeed);
            }
            catch (COMException ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] RASTER_WHEEL_FAILED | " +
                    ex.Message);
            }
        }

        // ==================================================
        // 6. CHAN DOAN RASTER
        // ==================================================

        public string LayChanDoanRaster(int layerHandle)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);

            MapWinGIS.Image anh =
                banDo.get_Image(layerHandle);

            if (anh == null)
                return "Lớp không phải raster Image.";

            return
                "Nguồn: " +
                (anh.Filename ?? "(không xác định)") +
                "\r\nKích thước: " +
                anh.OriginalWidth + " x " +
                anh.OriginalHeight + " pixel" +
                "\r\nOverviews: " +
                anh.NumOverviews +
                "\r\nBộ đệm kết xuất: " +
                anh.BufferSize + "%" +
                "\r\nLoại nguồn: " +
                anh.SourceType +
                "\r\nLấy mẫu khi thu nhỏ: " +
                anh.DownsamplingMode +
                "\r\nLấy mẫu khi phóng to: " +
                anh.UpsamplingMode;
        }

        private static void GhiChanDoanBoNho()
        {
            try
            {
                using (Process tienTrinh =
                    Process.GetCurrentProcess())
                {
                    long boNhoRieng =
                        tienTrinh.PrivateMemorySize64 /
                        (1024L * 1024L);

                    long boNhoDangDung =
                        tienTrinh.WorkingSet64 /
                        (1024L * 1024L);

                    Debug.WriteLine(
                        "[MapWinGIS] MEMORY | private=" +
                        boNhoRieng +
                        "MB | working=" +
                        boNhoDangDung +
                        "MB | x64=" +
                        Environment.Is64BitProcess);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    "[MapWinGIS] MEMORY_INFO_ERROR | " +
                    ex.Message);
            }
        }

        // ==================================================
        // 7. KIEM TRA LOP
        // ==================================================

        public bool CoLop(int layerHandle)
        {
            KiemTraSanSang();
            return CoLopNoiBo(layerHandle);
        }

        public bool LayHienThiLop(int layerHandle)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);
            return banDo.get_LayerVisible(layerHandle);
        }

        public void DatHienThiLop(
            int layerHandle,
            bool hienThi)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);

            if (banDo.get_LayerVisible(layerHandle) == hienThi)
                return;

            banDo.set_LayerVisible(layerHandle, hienThi);

            if (banDo.get_LayerVisible(layerHandle) != hienThi)
                throw new InvalidOperationException(
                    "Không thay đổi được trạng thái lớp " +
                    layerHandle);
        }

        // ==================================================
        // 8. TEN LOP
        // ==================================================

        public void DatTenLop(
            int layerHandle,
            string tenLop)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);

            if (string.IsNullOrWhiteSpace(tenLop))
                throw new ArgumentException(
                    "Tên lớp trống.", "tenLop");

            string ten = tenLop.Trim();

            banDo.set_LayerName(layerHandle, ten);

            if (!string.Equals(
                banDo.get_LayerName(layerHandle),
                ten,
                StringComparison.Ordinal))
                throw new InvalidOperationException(
                    "Không đổi được tên lớp " + layerHandle);
        }

        public string LayTenLop(int layerHandle)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);
            return banDo.get_LayerName(layerHandle);
        }

        // ==================================================
        // 9. XOA LOP
        // ==================================================

        public void XoaLop(int layerHandle)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);

            banDo.RemoveLayer(layerHandle);

            if (CoLopNoiBo(layerHandle))
                throw new InvalidOperationException(
                    "Lớp vẫn tồn tại sau khi xóa: " +
                    layerHandle);
        }

        // ==================================================
        // 10. THU TU LOP
        // ==================================================

        public void DuaLopLenMotBac(int layerHandle)
        {
            KiemTraSanSang();

            int viTri = LayViTriLopNoiBo(layerHandle);

            if (viTri >= banDo.NumLayers - 1)
                throw new InvalidOperationException(
                    "Lớp đã ở trên cùng.");

            bool thanhCong = banDo.MoveLayerUp(viTri);

            if (!thanhCong &&
                LayViTriLopNoiBo(layerHandle) != viTri + 1)
                throw new InvalidOperationException(
                    "Không đưa được lớp lên trên.");

            if (LayViTriLopNoiBo(layerHandle) != viTri + 1)
                throw new InvalidOperationException(
                    "Vị trí lớp chưa được cập nhật.");
        }

        public void DuaLopXuongMotBac(int layerHandle)
        {
            KiemTraSanSang();

            int viTri = LayViTriLopNoiBo(layerHandle);

            if (viTri <= 0)
                throw new InvalidOperationException(
                    "Lớp đã ở dưới cùng.");

            bool thanhCong = banDo.MoveLayerDown(viTri);

            if (!thanhCong &&
                LayViTriLopNoiBo(layerHandle) != viTri - 1)
                throw new InvalidOperationException(
                    "Không đưa được lớp xuống dưới.");

            if (LayViTriLopNoiBo(layerHandle) != viTri - 1)
                throw new InvalidOperationException(
                    "Vị trí lớp chưa được cập nhật.");
        }

        public void DuaLopXuongDuoi(int layerHandle)
        {
            KiemTraSanSang();

            int viTri = LayViTriLopNoiBo(layerHandle);

            if (viTri == 0)
                return;

            bool thanhCong = banDo.MoveLayerBottom(viTri);

            if (!thanhCong &&
                LayViTriLopNoiBo(layerHandle) != 0)
                throw new InvalidOperationException(
                    "Không đưa được lớp xuống dưới.");

            if (LayViTriLopNoiBo(layerHandle) != 0)
                throw new InvalidOperationException(
                    "Vị trí lớp nền không đúng.");
        }

        public void DuaLopLenTrenCung(int layerHandle)
        {
            KiemTraSanSang();

            int viTri = LayViTriLopNoiBo(layerHandle);
            int viTriTrenCung = banDo.NumLayers - 1;

            if (viTri == viTriTrenCung)
                return;

            bool thanhCong = banDo.MoveLayerTop(viTri);

            if (!thanhCong &&
                LayViTriLopNoiBo(layerHandle) != viTriTrenCung)
                throw new InvalidOperationException(
                    "Không đưa được lớp lên trên cùng.");

            if (LayViTriLopNoiBo(layerHandle) != viTriTrenCung)
                throw new InvalidOperationException(
                    "Vị trí lớp điểm không đúng.");
        }

        // ==================================================
        // 11. THAO TAC BAN DO
        // ==================================================

        public void PhongDenLop(int layerHandle)
        {
            KiemTraSanSang();
            KiemTraLopTonTai(layerHandle);
            banDo.ZoomToLayer(layerHandle);
        }

        public void VeLaiBanDo()
        {
            KiemTraSanSang();

            Stopwatch dongHo = Stopwatch.StartNew();

            banDo.Redraw();

            dongHo.Stop();

            GhiThoiGianCham(
                "REDRAW_CALL",
                dongHo.ElapsedMilliseconds);
        }

        public void PhongTo(double heSo = 0.3)
        {
            KiemTraSanSang();
            KiemTraHeSoPhong(heSo);

            Stopwatch dongHo = Stopwatch.StartNew();

            banDo.ZoomIn(heSo);

            dongHo.Stop();

            GhiThoiGianCham(
                "ZOOM_IN_CALL",
                dongHo.ElapsedMilliseconds);
        }

        public void ThuNho(double heSo = 0.3)
        {
            KiemTraSanSang();
            KiemTraHeSoPhong(heSo);

            Stopwatch dongHo = Stopwatch.StartNew();

            banDo.ZoomOut(heSo);

            dongHo.Stop();

            GhiThoiGianCham(
                "ZOOM_OUT_CALL",
                dongHo.ElapsedMilliseconds);
        }

        public void PhongToanCanh()
        {
            KiemTraSanSang();

            if (banDo.NumLayers == 0)
                throw new InvalidOperationException(
                    "Chưa có lớp bản đồ.");

            banDo.ZoomToMaxExtents();
        }

        // ==================================================
        // 12. TRUY VAN HANDLE
        // ==================================================

        public int LayViTriLop(int layerHandle)
        {
            KiemTraSanSang();
            return LayViTriLopNoiBo(layerHandle);
        }

        public IList<int> LayDanhSachLayerHandle()
        {
            KiemTraSanSang();

            List<int> danhSach = new List<int>();

            for (int i = 0; i < banDo.NumLayers; i++)
                danhSach.Add(banDo.get_LayerHandle(i));

            return danhSach.AsReadOnly();
        }

        // ==================================================
        // 13. HAM NOI BO
        // ==================================================

        private static void GhiThoiGianCham(
            string thaoTac,
            long thoiGianMs)
        {
            if (thoiGianMs >= NguongChamMs)
                Debug.WriteLine(
                    "[MapWinGIS] SLOW | " +
                    thaoTac +
                    " | callMs=" +
                    thoiGianMs);
        }

        private static void KiemTraTapTinKemTheo(
            string tepShp,
            string phanMoRong)
        {
            string tapTin =
                Path.ChangeExtension(tepShp, phanMoRong);

            if (!File.Exists(tapTin))
                throw new FileNotFoundException(
                    "Shapefile thiếu " + phanMoRong +
                    ". Hãy đặt .shp, .shx, .dbf cùng thư mục.",
                    tapTin);
        }

        private bool CoLopNoiBo(int layerHandle)
        {
            if (layerHandle < 0)
                return false;

            for (int i = 0; i < banDo.NumLayers; i++)
            {
                if (banDo.get_LayerHandle(i) == layerHandle)
                    return true;
            }

            return false;
        }

        private void KiemTraLopTonTai(int layerHandle)
        {
            if (!CoLopNoiBo(layerHandle))
                throw new KeyNotFoundException(
                    "Không tìm thấy LayerHandle = " +
                    layerHandle);
        }

        private int LayViTriLopNoiBo(int layerHandle)
        {
            KiemTraLopTonTai(layerHandle);

            int viTri =
                banDo.get_LayerPosition(layerHandle);

            if (viTri < 0 ||
                viTri >= banDo.NumLayers ||
                banDo.get_LayerHandle(viTri) != layerHandle)
                throw new InvalidOperationException(
                    "Không xác định được vị trí lớp " +
                    layerHandle);

            return viTri;
        }

        private void KiemTraSanSang()
        {
            if (Thread.CurrentThread.ManagedThreadId !=
                maLuongGiaoDien)
                throw new InvalidOperationException(
                    "MapWinGIS phải thao tác trên luồng WinForms.");

            if (banDo.IsDisposed || banDo.Disposing)
                throw new ObjectDisposedException("banDo");

            if (!banDo.IsHandleCreated || banDo.InvokeRequired)
                throw new InvalidOperationException(
                    "AxMap chưa sẵn sàng trên luồng giao diện.");
        }

        private static void KiemTraHeSoPhong(double heSo)
        {
            if (double.IsNaN(heSo) ||
                double.IsInfinity(heSo) ||
                heSo <= 0d ||
                heSo >= 1d)
                throw new ArgumentOutOfRangeException(
                    "heSo",
                    "Hệ số phóng phải nằm trong khoảng (0;1).");
        }

        private string LayLoiBanDo()
        {
            try
            {
                int maLoi = banDo.LastErrorCode;

                if (maLoi == 0)
                    return "Không có mô tả lỗi chi tiết.";

                return "Mã lỗi " +
                    maLoi + ": " +
                    banDo.get_ErrorMsg(maLoi);
            }
            catch (Exception)
            {
                return "Không đọc được mô tả lỗi từ MapWinGIS.";
            }
        }
    }
}
