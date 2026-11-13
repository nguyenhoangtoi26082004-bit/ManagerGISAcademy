
using System;
using System.Diagnostics;
using System.IO;
using MapWinGIS;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Tile Server GeoWebCache/XYZ cho ban do Ha Noi.
    ///
    /// - Giu nguyen MapWinGIS, EPSG:32648
    ///   va ID provider hien tai.
    /// - Tai su dung provider va bo nho dem.
    /// - Khong cau hinh lai moi lan doi nen.
    /// - Khong truy cap COM tu Task.Run.
    /// - Khoi phuc trang thai tile neu bat that bai.
    ///
    /// C# 7.3 - .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {
        // OSM dang su dung ID 11301.
        // Khong doi ID cua Tile Server nay.
        private const int BdTile_MaNhaCungCap = 11302;

        private const double BdTile_GioiHanRamMB = 80d;
        private const double BdTile_GioiHanDiaMB = 600d;

        private bool bdTileDaCauHinhCache;
        private bool bdTileDangBat;
        private bool bdTileDaDangKyProvider;

        private string bdTileMauUrlDaDangKy;
        private string bdTileDuongDanDaDoc;

        private DateTime bdTileNgaySuaDaDocUtc =
            DateTime.MinValue;

        private string bdTileMauUrlDaDoc;

        // =====================================================
        // 1. DUONG DAN CAU HINH
        // =====================================================

        private string BdTile_DuongDanCauHinh
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder
                            .LocalApplicationData),
                    "ManagerGISAcademy",
                    "tile-server-url.txt");
            }
        }

        private bool BdTile_CoCauHinh()
        {
            try
            {
                FileInfo info = new FileInfo(
                    BdTile_DuongDanCauHinh);

                return info.Exists &&
                    info.Length > 0L;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        // =====================================================
        // 2. DOC CAU HINH CO CACHE
        // =====================================================

        private string BdTile_DocDuongDan()
        {
            string tep = BdTile_DuongDanCauHinh;

            FileInfo info = new FileInfo(tep);

            if (!info.Exists ||
                info.Length <= 0L)
            {
                bdTileMauUrlDaDoc = null;

                throw new FileNotFoundException(
                    "Chưa cấu hình Tile Server. " +
                    "Tạo tệp tile-server-url.txt trong " +
                    "%LOCALAPPDATA%\\ManagerGISAcademy " +
                    "rồi ghi mẫu URL XYZ thực tế " +
                    "của máy chủ.",
                    tep);
            }

            // File khong thay doi:
            // su dung cau hinh da doc.
            if (bdTileMauUrlDaDoc != null &&
                string.Equals(
                    bdTileDuongDanDaDoc,
                    tep,
                    StringComparison.OrdinalIgnoreCase) &&
                bdTileNgaySuaDaDocUtc ==
                    info.LastWriteTimeUtc)
            {
                return bdTileMauUrlDaDoc;
            }

            string mauUrl = null;

            foreach (string dong in File.ReadLines(tep))
            {
                string chu = dong.Trim();

                if (chu.Length == 0 ||
                    chu.StartsWith("#"))
                {
                    continue;
                }

                mauUrl = chu;
                break;
            }

            if (string.IsNullOrWhiteSpace(mauUrl) ||
                mauUrl.IndexOf(
                    "{zoom}",
                    StringComparison.Ordinal) < 0 ||
                mauUrl.IndexOf(
                    "{x}",
                    StringComparison.Ordinal) < 0 ||
                mauUrl.IndexOf(
                    "{y}",
                    StringComparison.Ordinal) < 0)
            {
                throw new FormatException(
                    "Mẫu URL Tile Server phải chứa " +
                    "đủ {zoom}, {x} và {y}; " +
                    "không dùng URL WMS GetMap.");
            }

            string diaChiThu = mauUrl
                .Replace("{zoom}", "10")
                .Replace("{x}", "1")
                .Replace("{y}", "1");

            Uri uri;

            if (!Uri.TryCreate(
                    diaChiThu,
                    UriKind.Absolute,
                    out uri) ||
                (uri.Scheme != Uri.UriSchemeHttp &&
                 uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new FormatException(
                    "URL Tile Server phải là " +
                    "HTTP hoặc HTTPS hợp lệ.");
            }

            if (!string.IsNullOrEmpty(uri.UserInfo))
            {
                throw new FormatException(
                    "Không nhúng tên đăng nhập " +
                    "hoặc mật khẩu trực tiếp " +
                    "trong URL Tile Server.");
            }

            bdTileDuongDanDaDoc = tep;

            bdTileNgaySuaDaDocUtc =
                info.LastWriteTimeUtc;

            bdTileMauUrlDaDoc = mauUrl;

            return mauUrl;
        }

        // =====================================================
        // 3. KIEM TRA TRANG THAI TILE
        // =====================================================

        private bool BdTile_DangBat()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated)
            {
                return false;
            }

            Tiles tiles = banDoMapWinGIS.Tiles;

            return tiles != null &&
                tiles.Visible &&
                tiles.UseServer &&
                tiles.ProviderId ==
                    BdTile_MaNhaCungCap;
        }

        // =====================================================
        // 4. DAT PHAM VI BAN DO HA NOI
        // =====================================================

        private void BdTile_PhongToHaNoi()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            double x = 0d;
            double y = 0d;

            if (!banDoMapWinGIS.DegreesToProj(
                    105.85d,
                    21.03d,
                    ref x,
                    ref y))
            {
                throw new InvalidOperationException(
                    "Không chuyển được tâm Hà Nội " +
                    "sang EPSG:32648.");
            }

            Extents phamVi = new Extents();

            const double banKinhMet = 65000d;

            phamVi.SetBounds(
                x - banKinhMet,
                y - banKinhMet,
                0d,
                x + banKinhMet,
                y + banKinhMet,
                0d);

            banDoMapWinGIS.Extents = phamVi;
        }

        // =====================================================
        // 5. CAU HINH BO NHO DEM MOT LAN
        // =====================================================

        private void BdTile_CauHinhCache(Tiles tiles)
        {
            string thuMuc = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder
                        .LocalApplicationData),
                "ManagerGISAcademy");

            Directory.CreateDirectory(thuMuc);

            string cacheChung = Path.Combine(
                thuMuc, "hanoi-gis-all-providers.db3");

            // Tranh doi cache lien tuc khi chuyen OSM <-> ve tinh.
            // Neu thanh phan khac doi cache, tu khoi phuc cache chung.
            bool dungCache = string.Equals(
                tiles.DiskCacheFilename,
                cacheChung,
                StringComparison.OrdinalIgnoreCase);

            if (bdTileDaCauHinhCache && dungCache)
                return;

            if (!dungCache)
                tiles.DiskCacheFilename = cacheChung;

            // RAM cache.
            tiles.set_UseCache(
                tkCacheType.RAM,
                true);

            tiles.set_DoCaching(
                tkCacheType.RAM,
                true);

            // Disk cache.
            tiles.set_UseCache(
                tkCacheType.Disk,
                true);

            tiles.set_DoCaching(
                tkCacheType.Disk,
                true);

            // Gioi han RAM cho tien trinh x86.
            tiles.set_MaxCacheSize(
                tkCacheType.RAM,
                BdTile_GioiHanRamMB);

            tiles.set_MaxCacheSize(
                tkCacheType.Disk,
                BdTile_GioiHanDiaMB);

            bdTileDaCauHinhCache = true;
        }

        // =====================================================
        // 6. BAT TILE SERVER TOI UU
        // =====================================================

        private void BdTile_Bat()
        {
            if (bdTileDangBat)
            {
                return;
            }

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated ||
                banDoMapWinGIS.InvokeRequired)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa sẵn sàng " +
                    "trên UI thread.");
            }

            // Kiem tra cau hinh truoc khi
            // thay doi bat ky thuoc tinh tile.
            string mauUrl = BdTile_DocDuongDan();

            Tiles tiles = banDoMapWinGIS.Tiles;

            if (tiles == null)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa khởi tạo Tiles.");
            }

            // Khong nhan provider ID la bang chung URL da dung.
            // MapWinGIS co the con provider cu cung ID trong RAM.
            int chiSoHienCo =
                tiles.Providers.get_IndexByProviderId(
                    BdTile_MaNhaCungCap);

            bool urlDung =
                chiSoHienCo >= 0 &&
                string.Equals(
                    tiles.Providers.get_UrlPattern(
                        chiSoHienCo),
                    mauUrl,
                    StringComparison.Ordinal);

            if (BdTile_DangBat() &&
                urlDung &&
                tiles.CurrentZoom >= 8 &&
                tiles.CurrentZoom <= 18)
            {
                bdTileDaDangKyProvider = true;
                bdTileMauUrlDaDangKy = mauUrl;
                return;
            }

            bool hienThiCu = tiles.Visible;
            bool dungMayChuCu = tiles.UseServer;
            int providerCu = tiles.ProviderId;

            bool providerMoi = false;

            Stopwatch dongHo =
                Stopwatch.StartNew();

            bdTileDangBat = true;

            try
            {
                // Neu provider ton tai nhung URL sai,
                // xoa provider cu va cache cua rieng provider 11302.
                // Khong dong cham provider OSM 11301.
                int chiSo =
                    tiles.Providers.get_IndexByProviderId(
                        BdTile_MaNhaCungCap);

                if (chiSo >= 0 &&
                    !string.Equals(
                        tiles.Providers.get_UrlPattern(chiSo),
                        mauUrl,
                        StringComparison.Ordinal))
                {
                    if (tiles.Visible &&
                        tiles.ProviderId == BdTile_MaNhaCungCap)
                    {
                        tiles.Visible = false;
                    }

                    if (!tiles.Providers.Remove(
                        BdTile_MaNhaCungCap, true))
                    {
                        throw new InvalidOperationException(
                            "Không thể thay provider vệ tinh cũ (11302).");
                    }

                    chiSo = -1;
                    bdTileDaDangKyProvider = false;
                    bdTileMauUrlDaDangKy = null;
                }

                if (chiSo < 0)
                {
                    bool thanhCong =
                        tiles.Providers.Add(
                            BdTile_MaNhaCungCap,
                            "Ảnh vệ tinh Hà Nội - " +
                            "Tile Server nội bộ",
                            mauUrl,
                            tkTileProjection
                                .SphericalMercator,
                            8,
                            18);

                    if (!thanhCong)
                    {
                        throw new InvalidOperationException(
                            "MapWinGIS không đăng ký " +
                            "được Tile Server: " +
                            tiles.Providers.get_ErrorMsg(
                                tiles.Providers.LastErrorCode));
                    }

                    providerMoi = true;

                    bdTileDaDangKyProvider = true;
                    bdTileMauUrlDaDangKy = mauUrl;
                }
                else if (!bdTileDaDangKyProvider)
                {
                    // Provider da duoc dang ky
                    // boi bo phan khac cua ung dung.
                    bdTileDaDangKyProvider = true;
                    bdTileMauUrlDaDangKy = mauUrl;
                }

                // Cache dung chung mot tep,
                // phan biet theo provider ID.
                BdTile_CauHinhCache(tiles);

                // An khung tile cua provider cu
                // truoc khi doi nguon.
                if (tiles.Visible &&
                    tiles.ProviderId != BdTile_MaNhaCungCap)
                {
                    tiles.Visible = false;
                }

                if (tiles.ProviderId != BdTile_MaNhaCungCap)
                {
                    tiles.ProviderId = BdTile_MaNhaCungCap;
                }

                if (!tiles.UseServer)
                {
                    tiles.UseServer = true;
                }

                if (!tiles.Visible)
                {
                    tiles.Visible = true;
                }

                // GeoServer chi phuc vu tu zoom 8.
                // Neu map qua rong, can lai tam Ha Noi.
                if (banDoMapWinGIS.NumLayers == 0 ||
                    tiles.CurrentZoom < 8 ||
                    tiles.CurrentZoom > 18)
                {
                    try
                    {
                        BdTile_PhongToHaNoi();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(
                            "[BASEMAP] CENTER_HANOI_FAILED | " +
                            ex.Message);
                    }
                }

                if (tiles.ProviderId != BdTile_MaNhaCungCap ||
                    !tiles.Visible ||
                    !tiles.UseServer)
                {
                    throw new InvalidOperationException(
                        "MapWinGIS chưa kích hoạt được provider 11302.");
                }

                if (tiles.ProjectionStatus ==
                    tkTilesProjectionStatus.tpsEmptyOrInvalid)
                {
                    throw new InvalidOperationException(
                        "MapWinGIS không chuyển được tile EPSG:3857 " +
                        "sang hệ tọa độ của bản đồ. " +
                        "Kiểm tra GeoProjection của AxMap.");
                }

                // Giu lop diem don vi tren nen ve tinh.
                DuaLopDiemLenTren();

                Debug.WriteLine(
                    "[MapWinGIS] TILE_ENABLED | ms=" +
                    dongHo.ElapsedMilliseconds +
                    " | newProvider=" + providerMoi +
                    " | provider=" +
                    tiles.ProviderId);

                GhiNhatKyBanDo(
                    "TILE_SERVER_ENABLED | provider=" +
                    tiles.ProviderId);
            }
            catch
            {
                // Neu kich hoat that bai,
                // thu khoi phuc trang thai tile cu.
                try
                {
                    if (tiles.ProviderId != providerCu)
                    {
                        tiles.ProviderId = providerCu;
                    }

                    if (tiles.UseServer != dungMayChuCu)
                    {
                        tiles.UseServer = dungMayChuCu;
                    }

                    if (tiles.Visible != hienThiCu)
                    {
                        tiles.Visible = hienThiCu;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(
                        "[MapWinGIS] TILE_ROLLBACK_FAILED | " +
                        ex.GetType().Name);
                }

                throw;
            }
            finally
            {
                dongHo.Stop();
                bdTileDangBat = false;
            }
        }
    }
}
