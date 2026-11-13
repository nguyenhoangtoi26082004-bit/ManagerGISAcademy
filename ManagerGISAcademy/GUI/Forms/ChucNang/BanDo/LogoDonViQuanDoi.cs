
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using GdiImage = System.Drawing.Image;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// 169 logo HQTCSDL: 126 Ban CHQS + 43 ho so cac cap.
    /// C# 7.3 / .NET Framework 4.8 / MapWinGIS.
    /// MouseDown + MouseUp: nhan logo -> tim MA_DON_VI -> mo panel.
    /// Khong su dung nguon 51 diem cu de truy xuat ho so moi.
    /// </summary>
    public partial class FormMain
    {
        private const string LogoKeyBchqs = "HQTCSDL_LOGO_BCHQS";
        private const string LogoKeyHoSo = "HQTCSDL_LOGO_HOSO";
        private const string LogoKeyChon = "HQTCSDL_LOGO_CHON";
        private const int LogoKichThuocBchqs = 18;
        private const int LogoKichThuocHoSo = 18;
        private const int LogoKichThuocChon = 26;
        private const string LogoCacheVersion = "v15_kyhieu_v4";
        private const double LogoBchqsHienTen = 7000d;
        private const double LogoBchqsAnTen = 11000d;
        private const double LogoHoSoHienTen = 30000d;
        private const double LogoHoSoAnTen = 40000d;
        private const string LogoFontNhan = "Segoe UI";
        private const int LogoCoChuNhan = 9;
        private const int LogoDemTrongSuot = 220;
        private const int LogoDemNgang = 5;
        private const int LogoDemDoc = 3;
        private const int LogoKhoangCachNhanBchqs = 14;
        private const int LogoKhoangCachNhanHoSo = 14;

        private int logoLopBanChqs = -1;
        private int logoLopHoSo = -1;
        private int logoLopDangChon = -1;
        private MapWinGIS.Image logoAnhThuong;
        private MapWinGIS.Image logoAnhDangChon;
        private System.Windows.Forms.Timer logoDongBoTimer;
        private bool logoDangXuLy;
        private bool logoDaGanSuKienDongBo;
        private bool? logoTenBchqs;
        private bool? logoTenHoSo;
        private int logoDemTickGiamSat;
        private int logoLanTruocTrongKhungBchqs = -1;
        private int logoLanTruocTrongKhungHoSo = -1;

        // Trang thai click va ho so moi (NapVaHienHoSo.cs su dung).
        private bool logoDaDangKyNhanDiem;
        private bool logoDaNhanChuotXuong;
        private int logoChuotXuongX;
        private int logoChuotXuongY;
        private bool logoDangHienHoSoMoi;
        private bool logoDiemChonLaDaiDien;
        private double logoDiemChonX;
        private double logoDiemChonY;

        private sealed class LogoDiem
        {
            public double X;
            public double Y;
            public string Ma;
            public string Ten;
            public string DiaBan;

            // Hop nhat 2026: logo theo loai, kich thuoc rieng,
            // diem dai dien ve mo + vong net dut de phan biet.
            public string TepAnh;
            public int KichThuoc;
            public bool LaDaiDien;
        }

        // Anh logo theo (tep, kich thuoc, mo) da nap vao MapWinGIS.
        private readonly Dictionary<string, MapWinGIS.Image> logoAnhTheoLoai =
            new Dictionary<string, MapWinGIS.Image>(StringComparer.OrdinalIgnoreCase);

        // ============================================================
        // 1. ANH LOGO: 24 PX, RIENG DIEM CHON 38 PX.
        // ============================================================
        private static string Logo_TimAnhGoc()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 7 && dir != null; i++, dir = dir.Parent)
            {
                string[] paths =
                {
                    Path.Combine(dir.FullName, "Resources", "Images", "KiHieuDonVi.png"),
                    Path.Combine(dir.FullName, "Resources", "KiHieuDonVi.png"),
                    Path.Combine(dir.FullName, "KiHieuDonVi.png")
                };
                foreach (string path in paths)
                    if (File.Exists(path)) return path;
            }
            throw new FileNotFoundException(
                "Không tìm thấy KiHieuDonVi.png trong Resources\\Images.");
        }

        /// <summary>
        /// Tim anh logo theo ten tep trong Resources\Logos.
        /// Chi lay ten tep (bo duong dan) de chan "..\" tu du lieu CSDL.
        /// Khong co anh -> null (dung ky hieu mac dinh).
        /// </summary>
        private static string Logo_TimAnhTheoTep(string tepAnh)
        {
            if (string.IsNullOrWhiteSpace(tepAnh)) return null;
            string ten = Path.GetFileName(tepAnh.Trim());
            if (string.IsNullOrEmpty(ten)) return null;
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            for (int i = 0; i < 7 && dir != null; i++, dir = dir.Parent)
            {
                string path = Path.Combine(dir.FullName, "Resources", "Logos", ten);
                if (File.Exists(path)) return path;
            }
            return null;
        }

        private static string Logo_TaoAnhNho(int kichThuoc,
            string tepAnh = null, bool mo = false)
        {
            if (kichThuoc < 16 || kichThuoc > 64)
                throw new ArgumentOutOfRangeException("kichThuoc");
            string src = Logo_TimAnhTheoTep(tepAnh) ?? Logo_TimAnhGoc();
            string cache = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ManagerGISAcademy", "MapIcons");
            Directory.CreateDirectory(cache);
            string ver = File.GetLastWriteTimeUtc(src).Ticks.ToString("x",
                CultureInfo.InvariantCulture);
            string path = Path.Combine(cache,
                Path.GetFileNameWithoutExtension(src) + "_" + kichThuoc + "px_" +
                (mo ? "daidien_" : "") + LogoCacheVersion + "_" + ver + ".png");
            if (File.Exists(path))
            {
                try
                {
                    using (GdiImage old = GdiImage.FromFile(path))
                        if (old.Width == kichThuoc && old.Height == kichThuoc)
                            return path;
                }
                catch { }
                File.Delete(path);
            }
            using (GdiImage source = GdiImage.FromFile(src))
            using (Bitmap bmp = new Bitmap(kichThuoc, kichThuoc,
                PixelFormat.Format32bppArgb))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                // Ky hieu v2: moi diem CUNG kich thuoc. Diem dai dien
                // (khong phai vi tri tru so) chi khac o do MO 60%,
                // khong thu nho, khong them vong -> ban do deu, gon.
                float khung = kichThuoc;
                float scale = Math.Min(khung / source.Width, khung / source.Height);
                int w = Math.Max(1, (int)Math.Round(source.Width * scale));
                int h = Math.Max(1, (int)Math.Round(source.Height * scale));
                Rectangle dich = new Rectangle((kichThuoc - w) / 2,
                    (kichThuoc - h) / 2, w, h);
                if (!mo)
                {
                    g.DrawImage(source, dich,
                        new Rectangle(0, 0, source.Width, source.Height),
                        GraphicsUnit.Pixel);
                }
                else
                {
                    using (ImageAttributes thuocTinh = new ImageAttributes())
                    {
                        ColorMatrix maTran = new ColorMatrix { Matrix33 = 0.6f };
                        thuocTinh.SetColorMatrix(maTran);
                        g.DrawImage(source, dich, 0, 0, source.Width, source.Height,
                            GraphicsUnit.Pixel, thuocTinh);
                    }
                }
                bmp.Save(path, ImageFormat.Png);
            }
            return path;
        }

        private MapWinGIS.Image Logo_LayAnhTheoTep(int kichThuoc,
            string tepAnh, bool mo)
        {
            kichThuoc = Math.Max(16, Math.Min(64, kichThuoc));
            string khoa = (tepAnh ?? "") + "|" + kichThuoc + "|" + (mo ? "1" : "0");
            MapWinGIS.Image old;
            if (logoAnhTheoLoai.TryGetValue(khoa, out old) && old != null)
                return old;
            string path = Logo_TaoAnhNho(kichThuoc, tepAnh, mo);
            MapWinGIS.Image image = new MapWinGIS.Image();
            if (!image.Open(path, ImageType.USE_FILE_EXTENSION, true, null))
                throw new InvalidOperationException("MapWinGIS không mở logo: " + path +
                    " | " + image.get_ErrorMsg(image.LastErrorCode));
            logoAnhTheoLoai[khoa] = image;
            return image;
        }

        private MapWinGIS.Image Logo_LayAnh(int kichThuoc)
        {
            bool selected = kichThuoc == LogoKichThuocChon;
            MapWinGIS.Image old = selected ? logoAnhDangChon : logoAnhThuong;
            if (old != null) return old;
            string path = Logo_TaoAnhNho(kichThuoc);
            MapWinGIS.Image image = new MapWinGIS.Image();
            if (!image.Open(path, ImageType.USE_FILE_EXTENSION, true, null))
                throw new InvalidOperationException("MapWinGIS không mở logo: " + path +
                    " | " + image.get_ErrorMsg(image.LastErrorCode));
            if (selected) logoAnhDangChon = image;
            else logoAnhThuong = image;
            return image;
        }

        private static string Logo_GioiHan(string s, int max)
        {
            s = (s ?? "").Trim();
            return s.Length <= max ? s : s.Substring(0, max);
        }
        private static string Logo_ChuanHoaTen(string ten)
        {
            string s = (ten ?? "").Normalize(NormalizationForm.FormD);
            StringBuilder b = new StringBuilder();
            foreach (char c in s)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) ==
                    UnicodeCategory.NonSpacingMark) continue;
                b.Append(c == 'đ' || c == 'Đ' ? 'd' : char.ToLowerInvariant(c));
            }
            return b.ToString();
        }
        private static uint Logo_MauOle(Color c)
        { return unchecked((uint)ColorTranslator.ToOle(c)); }

        // ============================================================
        // 2. LOP VA HANDLE
        // ============================================================
        private bool Logo_CoHandle(int h)
        {
            if (h < 0 || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return false;
            for (int i = 0; i < banDoMapWinGIS.NumLayers; i++)
                if (banDoMapWinGIS.get_LayerHandle(i) == h) return true;
            return false;
        }

        private bool Logo_LopConTonTai(int h, string key, int soDiem)
        {
            if (!Logo_CoHandle(h)) return false;
            try
            {
                if (!string.Equals(banDoMapWinGIS.get_LayerName(h), key,
                    StringComparison.Ordinal)) return false;
                Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                return sf != null && sf.NumShapes == soDiem;
            }
            catch { return false; }
        }

        private bool Native_CacLopLogoConTonTai()
        {
            return nativeDanhMucBchqsTraCuu != null &&
                nativeHoSoGisHqtcTraCuu != null &&
                Logo_LopConTonTai(logoLopBanChqs, LogoKeyBchqs,
                    nativeDanhMucBchqsTraCuu.Count) &&
                Logo_LopConTonTai(logoLopHoSo, LogoKeyHoSo,
                    nativeHoSoGisHqtcTraCuu.Count);
        }

        private void Logo_XoaLop(ref int handle, string key)
        {
            int old = handle;
            handle = -1;
            if (!Logo_CoHandle(old)) return;
            try
            {
                if (banDoMapWinGIS.get_LayerName(old) == key)
                    banDoMapWinGIS.RemoveLayer(old);
            }
            catch (Exception ex) { GhiNhatKyBanDo("LOGO_REMOVE_FAILED | " + ex); }
        }

        // ============================================================
        // 3. NHAN TEN THEO ZOOM
        // ============================================================
        private void Logo_CauHinhNhan(Shapefile sf, string key,
            IList<LogoDiem> danhSach)
        {
            if (sf == null || sf.NumShapes == 0 || key == LogoKeyChon) return;
            if (sf.get_FieldIndexByName("TEN_DON_VI") < 0) return;
            Labels labels = sf.Labels;
            if (labels == null) return;
            bool laHoSo = key == LogoKeyHoSo;
            try
            {
                labels.Visible = false;
                labels.DynamicVisibility = false;
                labels.ScaleLabels = false;
                labels.UseVariableSize = false;
                labels.UseGdiPlus = true;
                labels.FontName = LogoFontNhan;
                labels.FontSize = LogoCoChuNhan;
                labels.FontBold = true;
                labels.FontColor = Logo_MauOle(Color.FromArgb(26, 51, 66));
                labels.FontTransparency = 255;
                labels.HaloVisible = true;
                labels.HaloColor = Logo_MauOle(Color.White);
                labels.HaloSize = 1;
                labels.FrameVisible = true;
                labels.FrameType = tkLabelFrameType.lfRoundedRectangle;
                labels.FrameBackColor = Logo_MauOle(Color.White);
                labels.FrameTransparency = LogoDemTrongSuot;
                labels.FrameOutlineColor = Logo_MauOle(Color.FromArgb(192, 208, 219));
                labels.FrameOutlineWidth = 1;
                labels.FramePaddingX = LogoDemNgang;
                labels.FramePaddingY = LogoDemDoc;
                labels.AutoOffset = false;
                labels.Alignment = tkLabelAlignment.laCenterRight;
                labels.OffsetX = laHoSo ? LogoKhoangCachNhanHoSo : LogoKhoangCachNhanBchqs;
                labels.OffsetY = 0;
                labels.AvoidCollisions = true;
                labels.CollisionBuffer = 4;
                labels.RemoveDuplicates = false;
                labels.VerticalPosition = laHoSo
                    ? tkVerticalPosition.vpAboveAllLayers
                    : tkVerticalPosition.vpAboveParentLayer;
                labels.Clear();
                labels.Generate("[TEN_DON_VI]", tkLabelPositioning.lpCenter, true);
                if (danhSach != null)
                {
                    int n = Math.Min(labels.Count, danhSach.Count);
                    for (int i = 0; i < n; i++)
                    {
                        if (danhSach[i] == null) continue;
                        MapWinGIS.Label lab = labels.get_Label(i, 0);
                        if (lab != null) lab.Text = danhSach[i].Ten ?? "";
                    }
                }
                labels.Synchronized = labels.Count == sf.NumShapes;
                labels.Visible = false;
            }
            catch (Exception ex)
            { GhiNhatKyBanDo("LOGO_LABEL_FAILED | " + key + " | " + ex); }
        }

        private void Logo_DamBaoNhan(int h, string key)
        {
            if (!Logo_CoHandle(h)) return;
            try
            {
                Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                if (sf != null && sf.Labels != null && sf.Labels.Count != sf.NumShapes)
                    Logo_CauHinhNhan(sf, key, null);
            }
            catch (Exception ex)
            { GhiNhatKyBanDo("LOGO_LABEL_RESTORE | " + ex.Message); }
        }

        private double Logo_LayDoRongKhungNhin()
        {
            try
            {
                if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                    return double.MaxValue;
                MapWinGIS.Extents e = banDoMapWinGIS.Extents;
                double w = e == null ? 0 : Math.Abs(e.xMax - e.xMin);
                return double.IsNaN(w) || double.IsInfinity(w) || w <= 0
                    ? double.MaxValue : w;
            }
            catch { return double.MaxValue; }
        }

        private static bool Logo_TinhHienTen(double width, bool? previous,
            double show, double hide)
        {
            if (width == double.MaxValue) return false;
            if (!previous.HasValue) return width <= show;
            return previous.Value ? width < hide : width <= show;
        }

        private bool Logo_DatHienTenChoLop(int h, string key, bool show)
        {
            if (!Logo_CoHandle(h)) return false;
            try
            {
                Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                if (sf == null || sf.Labels == null) return false;
                if (sf.Labels.Count != sf.NumShapes) Logo_CauHinhNhan(sf, key, null);
                if (sf.Labels.Visible == show) return false;
                sf.Labels.Visible = show;
                return true;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LOGO_LABEL_TOGGLE | " + ex.Message);
                return false;
            }
        }

        private void Logo_CapNhatHienThiTenTheoZoom(bool batBuoc)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            double width = Logo_LayDoRongKhungNhin();
            bool b = Logo_TinhHienTen(width, logoTenBchqs,
                LogoBchqsHienTen, LogoBchqsAnTen);
            bool h = Logo_TinhHienTen(width, logoTenHoSo,
                LogoHoSoHienTen, LogoHoSoAnTen);
            bool changed = !logoTenBchqs.HasValue || !logoTenHoSo.HasValue ||
                logoTenBchqs.Value != b || logoTenHoSo.Value != h;
            logoTenBchqs = b;
            logoTenHoSo = h;
            if (!batBuoc && !changed) return;
            bool a = Logo_DatHienTenChoLop(logoLopBanChqs, LogoKeyBchqs, b);
            bool c = Logo_DatHienTenChoLop(logoLopHoSo, LogoKeyHoSo, h);
            if (a || c) banDoMapWinGIS.Redraw();
        }

        // ============================================================
        // 4. TAO SHAPEFILE TU DANH SACH DA CO
        // ============================================================
        private int Logo_TaoLop(string key, IList<LogoDiem> points, int size)
        {
            if (points == null || points.Count == 0) return -1;
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.GeoProjection == null)
                throw new InvalidOperationException("AxMap chưa có hệ quy chiếu.");
            MapWinGIS.Image picture = Logo_LayAnh(size);
            Shapefile sf = new Shapefile();
            if (!sf.CreateNewWithShapeID("", ShpfileType.SHP_POINT))
                throw new InvalidOperationException("Không tạo được Shapefile " + key);
            sf.GeoProjection = banDoMapWinGIS.GeoProjection.Clone();
            if ((!sf.EditingShapes || !sf.EditingTable) &&
                !sf.StartEditingShapes(true, null))
                throw new InvalidOperationException("Không mở được EditingShapes.");
            int ma = sf.EditAddField("MA_DON_VI", FieldType.STRING_FIELD, 0, 60);
            int ten = sf.EditAddField("TEN_DON_VI", FieldType.STRING_FIELD, 0, 250);
            int dia = sf.EditAddField("DIA_BAN", FieldType.STRING_FIELD, 0, 150);
            if (ma < 0 || ten < 0 || dia < 0)
                throw new InvalidOperationException("Không tạo được cột Shapefile.");
            foreach (LogoDiem d in points)
            {
                if (d == null || double.IsNaN(d.X) || double.IsInfinity(d.X) ||
                    double.IsNaN(d.Y) || double.IsInfinity(d.Y))
                    throw new InvalidOperationException("Tọa độ logo không hợp lệ.");
                Shape shape = new Shape();
                if (!shape.Create(ShpfileType.SHP_POINT))
                    throw new InvalidOperationException("Không tạo được shape điểm.");
                MapWinGIS.Point p = new MapWinGIS.Point { x = d.X, y = d.Y };
                int idx = 0;
                if (!shape.InsertPoint(p, ref idx))
                    throw new InvalidOperationException("Không chèn được tọa độ logo.");
                int row = sf.EditAddShape(shape);
                if (row < 0 ||
                    !sf.EditCellValue(ma, row, Logo_GioiHan(d.Ma, 60)) ||
                    !sf.EditCellValue(ten, row, Logo_GioiHan(d.Ten, 250)) ||
                    !sf.EditCellValue(dia, row, Logo_GioiHan(d.DiaBan, 150)))
                    throw new InvalidOperationException("Không ghi được dữ liệu logo.");
            }
            if (sf.NumShapes != points.Count)
                throw new InvalidOperationException("Số shape logo không khớp nguồn.");
            ShapeDrawingOptions opt = sf.DefaultDrawingOptions;
            opt.PointType = tkPointSymbolType.ptSymbolPicture;
            opt.Picture = picture;
            opt.PictureScaleX = 1.0;
            opt.PictureScaleY = 1.0;
            opt.PointSize = (float)size;
            opt.PointRotation = 0.0;
            opt.AlignPictureByBottom = false;
            Logo_GanNhomKyHieu(sf, points, size);
            sf.CollisionMode = tkCollisionMode.AllowCollisions;
            int handle = banDoMapWinGIS.AddLayer(sf, true);
            if (handle < 0) throw new InvalidOperationException("Không thêm được lớp " + key);
            try
            {
                banDoMapWinGIS.set_LayerName(handle, key);
                banDoMapWinGIS.set_LayerKey(handle, key);
                banDoMapWinGIS.set_LayerDynamicVisibility(handle, false);
                banDoMapWinGIS.set_LayerVisible(handle, true);
                banDoMapWinGIS.set_LayerSkipOnSaving(handle, true);
                Shapefile mapSf = banDoMapWinGIS.get_Shapefile(handle);
                if (mapSf == null)
                    throw new InvalidOperationException("Không đọc được lớp " + key);
                if (key != LogoKeyChon) Logo_CauHinhNhan(mapSf, key, points);
                GhiNhatKyBanDo("HQTCSDL_LAYER_CREATED | " + key +
                    " | Shapes=" + mapSf.NumShapes);
                return handle;
            }
            catch
            {
                try { if (Logo_CoHandle(handle)) banDoMapWinGIS.RemoveLayer(handle); }
                catch { }
                throw;
            }
        }

        /// <summary>
        /// Moi to hop (tep logo, kich thuoc, diem dai dien) la mot
        /// ShapefileCategory co anh rieng. Diem khong co thong tin
        /// hop nhat giu DefaultDrawingOptions (ky hieu mac dinh).
        /// Shape thu i tuong ung points[i] (EditAddShape them tuan tu).
        /// </summary>
        private void Logo_GanNhomKyHieu(Shapefile sf, IList<LogoDiem> points,
            int kichThuocLop)
        {
            Dictionary<string, int> nhom =
                new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < points.Count; i++)
            {
                LogoDiem d = points[i];
                if (d == null || (string.IsNullOrWhiteSpace(d.TepAnh) && !d.LaDaiDien))
                    continue;
                int kichThuoc = d.KichThuoc > 0 ? d.KichThuoc : kichThuocLop;
                string khoa = (d.TepAnh ?? "") + "|" + kichThuoc + "|" +
                    (d.LaDaiDien ? "daidien" : "thuc");
                int chiSo;
                if (!nhom.TryGetValue(khoa, out chiSo))
                {
                    ShapefileCategory cat = sf.Categories.Add("LOGO|" + khoa);
                    if (cat == null)
                        throw new InvalidOperationException("Không tạo được nhóm ký hiệu " + khoa);
                    ShapeDrawingOptions o = cat.DrawingOptions;
                    o.PointType = tkPointSymbolType.ptSymbolPicture;
                    o.Picture = Logo_LayAnhTheoTep(kichThuoc, d.TepAnh, d.LaDaiDien);
                    o.PictureScaleX = 1.0;
                    o.PictureScaleY = 1.0;
                    o.PointSize = kichThuoc;
                    o.PointRotation = 0.0;
                    o.AlignPictureByBottom = false;
                    cat.DrawingOptions = o;
                    chiSo = sf.Categories.Count - 1;
                    nhom[khoa] = chiSo;
                }
                sf.set_ShapeCategory(i, chiSo);
            }
        }

        private void Logo_DuaLopLenTren(int handle)
        {
            if (!Logo_CoHandle(handle)) return;
            int pos = banDoMapWinGIS.get_LayerPosition(handle);
            if (pos >= 0 && pos < banDoMapWinGIS.NumLayers - 1)
                banDoMapWinGIS.MoveLayerTop(pos);
        }

        private void Logo_SapXepThuTuLop()
        {
            if (!Logo_CoHandle(logoLopBanChqs) || !Logo_CoHandle(logoLopHoSo))
                return;
            int top = banDoMapWinGIS.NumLayers - 1;
            bool selected = Logo_CoHandle(logoLopDangChon);
            bool correct = selected
                ? banDoMapWinGIS.get_LayerPosition(logoLopDangChon) == top &&
                  banDoMapWinGIS.get_LayerPosition(logoLopHoSo) == top - 1 &&
                  banDoMapWinGIS.get_LayerPosition(logoLopBanChqs) == top - 2
                : banDoMapWinGIS.get_LayerPosition(logoLopHoSo) == top &&
                  banDoMapWinGIS.get_LayerPosition(logoLopBanChqs) == top - 1;
            if (correct) return;
            Logo_DuaLopLenTren(logoLopBanChqs);
            Logo_DuaLopLenTren(logoLopHoSo);
            if (selected) Logo_DuaLopLenTren(logoLopDangChon);
        }

        private void Logo_AnLop51Cu()
        {
            if (!Native_CacLopLogoConTonTai() || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed) return;
            // Lop handleLopDiemPostGIS la ho so QUAN LY (Sua/Xoa duoc),
            // khong con la "lop 51 cu": khong an lop nay nua, de ban do
            // khop voi luoi Quan ly du lieu.
            for (int i = 0; i < banDoMapWinGIS.NumLayers; i++)
            {
                int h = banDoMapWinGIS.get_LayerHandle(i);
                if (h < 0 || h == logoLopBanChqs || h == logoLopHoSo ||
                    h == logoLopDangChon || h == handleLopDiemPostGIS) continue;
                try
                {
                    Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                    if (sf == null || sf.NumShapes != 51) continue;
                    bool hasFields = sf.get_FieldIndexByName("MA_DON_VI") >= 0 &&
                        sf.get_FieldIndexByName("TEN_DON_VI") >= 0;
                    string name = Logo_ChuanHoaTen(banDoMapWinGIS.get_LayerName(h));
                    if (!hasFields && !name.Contains("don vi quan doi")) continue;
                    if (banDoMapWinGIS.get_LayerVisible(h))
                        banDoMapWinGIS.set_LayerVisible(h, false);
                }
                catch (Exception ex)
                { GhiNhatKyBanDo("LOGO_HIDE_SCAN_FAILED | " + ex.Message); }
            }
        }

        private int Logo_DemDiemTrongKhung(int h)
        {
            if (!Logo_CoHandle(h)) return 0;
            try
            {
                Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                MapWinGIS.Extents ext = banDoMapWinGIS.Extents;
                if (sf == null || ext == null) return 0;
                int n = 0;
                for (int i = 0; i < sf.NumShapes; i++)
                {
                    Shape shape = sf.get_Shape(i);
                    if (shape == null || shape.numPoints == 0) continue;
                    MapWinGIS.Point p = shape.get_Point(0);
                    if (p.x >= ext.xMin && p.x <= ext.xMax &&
                        p.y >= ext.yMin && p.y <= ext.yMax) n++;
                }
                return n;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LOGO_COUNT_FAILED | " + ex.Message);
                return -1;
            }
        }

        private void Logo_KiemTraDiemTrenBanDo()
        {
            if (!Native_CacLopLogoConTonTai()) return;
            int b = Logo_DemDiemTrongKhung(logoLopBanChqs);
            int h = Logo_DemDiemTrongKhung(logoLopHoSo);
            if (b == logoLanTruocTrongKhungBchqs &&
                h == logoLanTruocTrongKhungHoSo) return;
            logoLanTruocTrongKhungBchqs = b;
            logoLanTruocTrongKhungHoSo = h;
            GhiNhatKyBanDo("HQTCSDL_MAP_CHECK | BCHQS=" + b + "/" +
                nativeDanhMucBchqsTraCuu.Count + " | HOSO=" + h + "/" +
                nativeHoSoGisHqtcTraCuu.Count);
        }

        private void Logo_DamBaoThuTuVaAnCu()
        {
            if (!Native_CacLopLogoConTonTai()) return;
            if (!banDoMapWinGIS.get_LayerVisible(logoLopBanChqs))
                banDoMapWinGIS.set_LayerVisible(logoLopBanChqs, true);
            if (!banDoMapWinGIS.get_LayerVisible(logoLopHoSo))
                banDoMapWinGIS.set_LayerVisible(logoLopHoSo, true);
            banDoMapWinGIS.set_LayerDynamicVisibility(logoLopBanChqs, false);
            banDoMapWinGIS.set_LayerDynamicVisibility(logoLopHoSo, false);
            Logo_DamBaoNhan(logoLopBanChqs, LogoKeyBchqs);
            Logo_DamBaoNhan(logoLopHoSo, LogoKeyHoSo);
            Logo_SapXepThuTuLop();
            Logo_AnLop51Cu();
            Logo_CapNhatHienThiTenTheoZoom(true);
        }

        // ============================================================
        // 5. SU KIEN NHAN LOGO. KHONG PHU THUOC CHE DO CHON.
        // ============================================================
        private void Logo_DangKyNhanDiem()
        {
            if (logoDaDangKyNhanDiem || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed) return;
            banDoMapWinGIS.SendMouseDown = true;
            banDoMapWinGIS.SendMouseUp = true;
            // Su dung lambda de tuong thich interop AxMapWinGIS dang tham chieu.
            banDoMapWinGIS.MouseDownEvent += (s, e) =>
            {
                if (e.button != 1) { logoDaNhanChuotXuong = false; return; }
                logoChuotXuongX = e.x;
                logoChuotXuongY = e.y;
                logoDaNhanChuotXuong = true;
            };
            banDoMapWinGIS.MouseUpEvent += (s, e) =>
            {
                // Che do "Dinh vi lai tru so": chuot phai = huy.
                if (e.button == 2 && CongCu_XuLyNhanChuot(2, e.x, e.y)) return;
                if (e.button == 2 && DinhVi_XuLyNhanChuot(2, e.x, e.y)) return;
                if (e.button != 1 || !logoDaNhanChuotXuong) return;
                logoDaNhanChuotXuong = false;
                // Nhap - khong phai thao tac keo ban do.
                if (Math.Abs(e.x - logoChuotXuongX) > 7 ||
                    Math.Abs(e.y - logoChuotXuongY) > 7) return;
                // Che do "Dinh vi lai tru so": chuot trai = lay diem.
                if (CongCu_XuLyNhanChuot(1, e.x, e.y)) return;
                if (DinhVi_XuLyNhanChuot(1, e.x, e.y)) return;
                Logo_XuLyNhanLogo(e.x, e.y);
            };
            logoDaDangKyNhanDiem = true;
            GhiNhatKyBanDo("HQTCSDL_LOGO_CLICK_REGISTERED");
        }

        private LogoDiem Logo_TimDiemGanPixel(int px, int py,
            out bool laBanChqs)
        {
            laBanChqs = false;
            if (!Native_CacLopLogoConTonTai()) return null;
            LogoDiem best = null;
            double bestDistance = 18d; // Hit test 24px, de nhan tren man hinh DPI cao.
            int[] handles = { logoLopHoSo, logoLopBanChqs };
            for (int t = 0; t < handles.Length; t++)
            {
                int h = handles[t];
                if (!Logo_CoHandle(h) || !banDoMapWinGIS.get_LayerVisible(h)) continue;
                Shapefile sf = banDoMapWinGIS.get_Shapefile(h);
                if (sf == null) continue;
                int col = sf.get_FieldIndexByName("MA_DON_VI");
                if (col < 0) continue;
                for (int i = 0; i < sf.NumShapes; i++)
                {
                    Shape shape = sf.get_Shape(i);
                    if (shape == null || shape.numPoints < 1) continue;
                    MapWinGIS.Point p = shape.get_Point(0);
                    if (p == null) continue;
                    double sx = 0d, sy = 0d;
                    banDoMapWinGIS.ProjToPixel(p.x, p.y, ref sx, ref sy);
                    double dx = sx - px, dy = sy - py;
                    double distance = Math.Sqrt(dx * dx + dy * dy);
                    if (distance > bestDistance) continue;
                    string ma = (Convert.ToString(sf.get_CellValue(col, i)) ?? "").Trim();
                    if (ma.Length == 0) continue;
                    int tenCol = sf.get_FieldIndexByName("TEN_DON_VI");
                    string ten = tenCol < 0 ? "" : Convert.ToString(sf.get_CellValue(tenCol, i));
                    bestDistance = distance;
                    best = new LogoDiem { X = p.x, Y = p.y, Ma = ma, Ten = ten };
                    laBanChqs = h == logoLopBanChqs;
                }
            }
            return best;
        }

        private void Logo_XuLyNhanLogo(int px, int py)
        {
            if (IsDisposed || Disposing || phanHeHienTai != "Bản đồ GIS" ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            try
            {
                bool laBchqs;
                LogoDiem hit = Logo_TimDiemGanPixel(px, py, out laBchqs);
                // Nhan vao ban do trong: khong lam gi, khong dong panel.
                if (hit == null) return;
                DonViQuanDoi hoSo;
                if (laBchqs)
                {
                    DanhMucBanChqsCapXa2026 d = nativeDanhMucBchqsTraCuu
                        .FirstOrDefault(v => v != null && string.Equals(
                            (v.MaDvhc ?? "").Trim(), hit.Ma,
                            StringComparison.OrdinalIgnoreCase));
                    if (d == null || !CoQuyen(ThaoTacDonViQuanDoi.Xem, d.MaDvhc))
                        return;
                    hoSo = new DonViQuanDoi
                    {
                        MaDonVi = "HN-BCHQS-" + d.MaDvhc,  // trung ma ho so HQTCSDL
                        MaDvhc = d.MaDvhc,
                        MaPhuongXa = d.MaXaPhuong,
                        TenDonVi = d.TenDonVi,
                        LoaiDonVi = d.LoaiDonVi ?? "Ban Chỉ huy quân sự",
                        DonViCapTren = d.DonViCapTren,
                        PhuongXa = d.PhuongXa,
                        DaXacMinhDiaBan = d.DaGhepDiaBan,
                        LaDanhMucBanChqs = true,
                        NguonDuLieu = "HQTCSDL · Danh mục Ban CHQS phường/xã",
                        // Vi tri hien hanh that (sau khi dinh vi lai), neu co.
                        KinhDo = (d.BanDo != null ? d.BanDo.KinhDoThuc : null)
                                 ?? d.KinhDoDaiDien ?? 0d,
                        ViDo = (d.BanDo != null ? d.BanDo.ViDoThuc : null)
                               ?? d.ViDoDaiDien ?? 0d,
                        GhiChu = d.BanDo != null
                            ? d.BanDo.MoTaViTri
                            : "Vị trí là điểm đại diện địa bàn, không phải trụ sở."
                    };
                }
                else
                {
                    HoSoGisHqtc d = nativeHoSoGisHqtcTraCuu
                        .FirstOrDefault(v => v != null && string.Equals(
                            (v.MaDonVi ?? "").Trim(), hit.Ma,
                            StringComparison.OrdinalIgnoreCase));
                    if (d == null || !CoQuyen(ThaoTacDonViQuanDoi.Xem, d.MaDonVi))
                        return;
                    hoSo = new DonViQuanDoi
                    {
                        MaDonVi = d.MaDonVi,
                        TenDonVi = d.TenDonVi,
                        LoaiDonVi = string.IsNullOrWhiteSpace(d.TenLoai)
                            ? d.TenCap : d.TenLoai,
                        DonViCapTren = d.TenCap,
                        MaPhuongXa = d.MaXaPhuong,
                        PhuongXa = d.DiaBan,
                        DaXacMinhDiaBan = !string.IsNullOrWhiteSpace(d.TenXaPhuong),
                        NguonDuLieu = "HQTCSDL · Hồ sơ GIS các cấp",
                        // Toa do GOC (khong phai vi tri ve da gian cach).
                        KinhDo = d.KinhDo ?? 0d,
                        ViDo = d.ViDo ?? 0d,
                        GhiChu = d.BanDo != null ? d.BanDo.MoTaViTri : null
                    };
                }
                // Chi tao logo 38px tai diem chon, khong zoom bat ngo.
                try { Native_DongBoLogoDinhVi(hit.X, hit.Y); }
                catch (Exception ex) { GhiNhatKyBanDo("LOGO_SELECT_MARK_FAILED | " + ex); }
                TtDv_HienThiHoSoLogo(hoSo, hit.X, hit.Y, laBchqs);
                GhiNhatKyBanDo("HQTCSDL_LOGO_CLICK | " +
                    (laBchqs ? "BCHQS" : "HOSO") + " | " + hit.Ma);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("HQTCSDL_CLICK_FAILED | " + ex);
                statusGeneral.Text = "● Không mở được thông tin điểm: " + ex.Message;
            }
        }

        private void Logo_DinhTamDiemDangChon()
        {
            if (!logoDangHienHoSoMoi || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed) return;
            try
            {
                MapWinGIS.Extents ext = new MapWinGIS.Extents();
                ext.SetBounds(logoDiemChonX - 1200d, logoDiemChonY - 850d, 0d,
                    logoDiemChonX + 1200d, logoDiemChonY + 850d, 0d);
                banDoMapWinGIS.Extents = ext;
                banDoMapWinGIS.Redraw();
            }
            catch (Exception ex)
            { GhiNhatKyBanDo("LOGO_CENTER_FAILED | " + ex); }
        }

        // ============================================================
        // 6. GIAM SAT LOP, ZOOM VA HOOK CHUOT.
        // ============================================================
        private void Logo_BatGiamSat()
        {
            Logo_DangKyNhanDiem();
            if (logoDongBoTimer == null)
            {
                logoDongBoTimer = new System.Windows.Forms.Timer { Interval = 450 };
                logoDongBoTimer.Tick += (s, e) =>
                {
                    if (logoDangXuLy || IsDisposed || Disposing ||
                        banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                        phanHeHienTai != "Bản đồ GIS") return;
                    logoDangXuLy = true;
                    try
                    {
                        if (Native_CacLopLogoConTonTai())
                        {
                            Logo_CapNhatHienThiTenTheoZoom(false);
                            if (++logoDemTickGiamSat >= 6)
                            {
                                logoDemTickGiamSat = 0;
                                Logo_DamBaoThuTuVaAnCu();
                                Logo_KiemTraDiemTrenBanDo();
                                // May man: map co the reset SendMouse... sau khi thay ban do.
                                banDoMapWinGIS.SendMouseDown = true;
                                banDoMapWinGIS.SendMouseUp = true;
                            }
                        }
                        else if (nativeDanhMucBchqsTraCuu != null &&
                                 nativeHoSoGisHqtcTraCuu != null)
                            Native_DongBoLogoTatCa();
                    }
                    catch (Exception ex)
                    { GhiNhatKyBanDo("LOGO_MONITOR_FAILED | " + ex.Message); }
                    finally { logoDangXuLy = false; }
                };
            }
            if (!logoDaGanSuKienDongBo)
            {
                logoDaGanSuKienDongBo = true;
                Disposed += (s, e) =>
                {
                    if (logoDongBoTimer != null)
                    {
                        logoDongBoTimer.Stop();
                        logoDongBoTimer.Dispose();
                        logoDongBoTimer = null;
                    }
                };
            }
            if (!logoDongBoTimer.Enabled) logoDongBoTimer.Start();
        }

        // ============================================================
        // 7. HIEN THI LOGO DIEM DANG CHON
        // ============================================================
        private void Native_DongBoLogoDinhVi(double x, double y)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            List<LogoDiem> list = new List<LogoDiem>
            {
                new LogoDiem { X = x, Y = y, Ma = "DANG_CHON",
                    Ten = "Điểm đang chọn", DiaBan = "" }
            };
            int handle = Logo_TaoLop(LogoKeyChon, list, LogoKichThuocChon);
            Logo_XoaLop(ref logoLopDangChon, LogoKeyChon);
            logoLopDangChon = handle;
            Logo_SapXepThuTuLop();
            Logo_CapNhatHienThiTenTheoZoom(true);
            banDoMapWinGIS.Redraw();
        }

        // ============================================================
        // 8. DONG BO CAC LOP 126 + 43
        // ============================================================
        private bool Native_DongBoLogoTatCa()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                nativeDanhMucBchqsTraCuu == null ||
                nativeHoSoGisHqtcTraCuu == null) return false;
            if (Native_CacLopLogoConTonTai())
            {
                Logo_DamBaoThuTuVaAnCu();
                Logo_BatGiamSat();
                Logo_KiemTraDiemTrenBanDo();
                return true;
            }
            List<LogoDiem> bchqs = new List<LogoDiem>();
            List<LogoDiem> hoSo = new List<LogoDiem>();
            // Vi tri VE: uu tien geom_hien_thi cua CSDL hop nhat (da gian
            // cach cac don vi trung diem); khong co thi dung vi tri goc.
            foreach (DanhMucBanChqsCapXa2026 d in nativeDanhMucBchqsTraCuu)
            {
                double x, y;
                ThongTinBanDoHopNhat bd = d == null ? null : d.BanDo;
                double? kinhDo = bd != null && bd.KinhDoVe.HasValue && bd.ViDoVe.HasValue
                    ? bd.KinhDoVe : (d == null ? null : d.KinhDoDaiDien);
                double? viDo = bd != null && bd.KinhDoVe.HasValue && bd.ViDoVe.HasValue
                    ? bd.ViDoVe : (d == null ? null : d.ViDoDaiDien);
                if (d == null || !d.CoDiemDaiDien ||
                    !kinhDo.HasValue || !viDo.HasValue ||
                    !Native_Wgs84SangToaDoBanDo(kinhDo.Value,
                        viDo.Value, out x, out y))
                {
                    GhiNhatKyBanDo("HQTCSDL_BCHQS_COORD_FAILED | " +
                        (d == null ? "NULL" : d.MaDvhc));
                    return false;
                }
                bchqs.Add(new LogoDiem
                {
                    X = x, Y = y, Ma = d.MaDvhc, Ten = d.TenDonVi, DiaBan = d.PhuongXa,
                    TepAnh = bd == null ? null : bd.TepAnhLogo,
                    KichThuoc = bd == null || !bd.KichThuocLogoPx.HasValue
                        ? 0 : bd.KichThuocLogoPx.Value,
                    // Danh muc BCHQS cu luon la diem dai dien.
                    LaDaiDien = bd == null || !bd.LaViTriThuc
                });
            }
            foreach (HoSoGisHqtc d in nativeHoSoGisHqtcTraCuu)
            {
                double x, y;
                ThongTinBanDoHopNhat bd = d == null ? null : d.BanDo;
                double? kinhDo = bd != null && bd.KinhDoVe.HasValue && bd.ViDoVe.HasValue
                    ? bd.KinhDoVe : (d == null ? null : d.KinhDo);
                double? viDo = bd != null && bd.KinhDoVe.HasValue && bd.ViDoVe.HasValue
                    ? bd.ViDoVe : (d == null ? null : d.ViDo);
                if (d == null || !d.CoDiemBanDo ||
                    !kinhDo.HasValue || !viDo.HasValue ||
                    !Native_Wgs84SangToaDoBanDo(kinhDo.Value,
                        viDo.Value, out x, out y))
                {
                    GhiNhatKyBanDo("HQTCSDL_HOSO_COORD_FAILED | " +
                        (d == null ? "NULL" : d.MaDonVi));
                    return false;
                }
                hoSo.Add(new LogoDiem
                {
                    X = x, Y = y, Ma = d.MaDonVi, Ten = d.TenDonVi, DiaBan = d.DiaBan,
                    TepAnh = bd == null ? null : bd.TepAnhLogo,
                    KichThuoc = bd == null || !bd.KichThuocLogoPx.HasValue
                        ? 0 : bd.KichThuocLogoPx.Value,
                    // CSDL cu khong co thong tin -> ve nhu truoc (khong mo).
                    LaDaiDien = bd != null && !bd.LaViTriThuc
                });
            }
            if (bchqs.Count == 0 || hoSo.Count == 0) return false;
            int newB = -1, newH = -1;
            try
            {
                newB = Logo_TaoLop(LogoKeyBchqs, bchqs, LogoKichThuocBchqs);
                newH = Logo_TaoLop(LogoKeyHoSo, hoSo, LogoKichThuocHoSo);
                if (!Logo_LopConTonTai(newB, LogoKeyBchqs, bchqs.Count) ||
                    !Logo_LopConTonTai(newH, LogoKeyHoSo, hoSo.Count))
                    throw new InvalidOperationException("Lớp logo mới thiếu shape.");
                Logo_XoaLop(ref logoLopBanChqs, LogoKeyBchqs);
                Logo_XoaLop(ref logoLopHoSo, LogoKeyHoSo);
                logoLopBanChqs = newB;
                logoLopHoSo = newH;
                logoTenBchqs = null;
                logoTenHoSo = null;
                logoDemTickGiamSat = 0;
                logoLanTruocTrongKhungBchqs = -1;
                logoLanTruocTrongKhungHoSo = -1;
                // Vong tron sai so ve TRUOC khi sap xep -> nam duoi logo.
                DinhVi_VeVongSaiSo();
                try { CongCu_SauKhiVeLogo(); }
                catch (Exception exCc) { GhiNhatKyBanDo("CONG_CU_SAU_LOGO_FAILED | " + exCc.Message); }
                Logo_DamBaoThuTuVaAnCu();
                Logo_BatGiamSat();
                Logo_KiemTraDiemTrenBanDo();
                GhiNhatKyBanDo("HQTCSDL_NEW_LAYERS | BCHQS=" + bchqs.Count +
                    " | HOSO=" + hoSo.Count + " | TOTAL=" + (bchqs.Count + hoSo.Count));
                banDoMapWinGIS.Redraw();
                return true;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("HQTCSDL_LOGO_SYNC_FAILED | " + ex);
                if (newB >= 0 && newB != logoLopBanChqs)
                    Logo_XoaLop(ref newB, LogoKeyBchqs);
                if (newH >= 0 && newH != logoLopHoSo)
                    Logo_XoaLop(ref newH, LogoKeyHoSo);
                return false;
            }
        }

        private void Native_XoaToanBoLogo()
        {
            if (logoDongBoTimer != null) logoDongBoTimer.Stop();
            DinhVi_XoaVongSaiSo();
            Logo_XoaLop(ref logoLopBanChqs, LogoKeyBchqs);
            Logo_XoaLop(ref logoLopHoSo, LogoKeyHoSo);
            Logo_XoaLop(ref logoLopDangChon, LogoKeyChon);
            logoTenBchqs = null;
            logoTenHoSo = null;
            logoDemTickGiamSat = 0;
            logoLanTruocTrongKhungBchqs = -1;
            logoLanTruocTrongKhungHoSo = -1;
            logoDangHienHoSoMoi = false;
        }
    }
}
