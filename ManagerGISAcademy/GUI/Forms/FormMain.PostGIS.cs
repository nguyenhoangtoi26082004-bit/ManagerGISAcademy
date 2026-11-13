
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;
using MapWinGIS;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Ve cac diem PostGIS len MapWinGIS va giu diem
    /// o tren cac lop nen.
    /// Dong thoi dang ky phan he Kiem tra du lieu WebView2.
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {
        private int handleLopDiemPostGIS = -1;
        private bool dangNapLopDiemPostGIS;
        private bool daDangKyNapLopPostGIS;

        // Tranh truy van DB + tao lai shapefile khi bam tab GIS lien tiep.
        // Goi truc tiep NapLaiLopDiemPostGIS() van luon nap moi du lieu.
        private const int PostgisKhoangCachNapLaiGiay = 20;
        private DateTime postgisLanNapThanhCongUtc = DateTime.MinValue;

        // Menu PostGIS duoc chen vao menu lop ban do.
        private System.Windows.Forms.Timer postgisHenGanMenu;
        private ToolStripMenuItem postgisMucLamMoi;
        private ToolStripMenuItem postgisMucKiemTraLogo;
        private DateTime postgisBatDauGanMenuUtc;

        // Anh ky hieu cho lop diem don vi.
        private const int PostgisLogoKichThuoc = 18;
        private MapWinGIS.Image postgisAnhLogoDonVi;
        private bool postgisDaBaoThieuLogo;
        private bool postgisDaBaoLoiLogo;

        // =====================================================
        // 1. KHOI TAO
        // =====================================================

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);


            if (daDangKyNapLopPostGIS ||
                IsDisposed ||
                Disposing)
                return;

            daDangKyNapLopPostGIS = true;

            btnNavMap.Click += Postgis_KhiChonBanDo;

            btnNapLop.Click +=
                (s, args) => DuaLopDiemLenTren();

            FormClosed += Postgis_KhiDongForm;
            Postgis_BatHenGanMenu();

            if (IsHandleCreated)
            {
                BeginInvoke(
                    new MethodInvoker(
                        NapLaiLopDiemPostGIS));
            }
        }

        private void Postgis_KhiChonBanDo(object sender, EventArgs e)
        {
            if (dangNapLopDiemPostGIS || IsDisposed || Disposing)
                return;

            try
            {
                bool daCoLop =
                    handleLopDiemPostGIS >= 0 &&
                    ketNoiMapWinGIS != null &&
                    ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS);

                if (daCoLop &&
                    postgisLanNapThanhCongUtc != DateTime.MinValue &&
                    (DateTime.UtcNow - postgisLanNapThanhCongUtc)
                        .TotalSeconds < PostgisKhoangCachNapLaiGiay)
                {
                    DuaLopDiemLenTren();
                    return;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "POSTGIS_CACHE_CHECK | " + ex.Message);
            }

            NapLaiLopDiemPostGIS();
        }

        // =====================================================
        // 2. MENU LOP BAN DO
        // =====================================================

        private void Postgis_BatHenGanMenu()
        {
            if (postgisHenGanMenu != null)
                return;

            postgisBatDauGanMenuUtc = DateTime.UtcNow;

            postgisHenGanMenu = new System.Windows.Forms.Timer
            {
                Interval = 500
            };

            postgisHenGanMenu.Tick += Postgis_ThuGanMenu;
            postgisHenGanMenu.Start();

            Postgis_ThuGanMenu(this, EventArgs.Empty);
        }

        private void Postgis_ThuGanMenu(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing ||
                (DateTime.UtcNow - postgisBatDauGanMenuUtc)
                    .TotalSeconds >= 20)
            {
                Postgis_DungHenGanMenu();
                return;
            }

            if (bd11MenuLop == null || bd11MenuLop.IsDisposed)
                return;

            if (postgisMucLamMoi == null ||
                postgisMucLamMoi.IsDisposed)
            {
                postgisMucLamMoi = new ToolStripMenuItem(
                    "Làm mới dữ liệu điểm PostGIS");

                postgisMucLamMoi.Click += Postgis_LamMoiThuCong;

                bd11MenuLop.Items.Insert(
                    Math.Min(5, bd11MenuLop.Items.Count),
                    postgisMucLamMoi);
            }

            if (postgisMucKiemTraLogo == null ||
                postgisMucKiemTraLogo.IsDisposed)
            {
                postgisMucKiemTraLogo = new ToolStripMenuItem(
                    "Kiểm tra ký hiệu logo đơn vị");

                postgisMucKiemTraLogo.Click +=
                    Postgis_KiemTraKyHieu;

                bd11MenuLop.Items.Insert(
                    Math.Min(6, bd11MenuLop.Items.Count),
                    postgisMucKiemTraLogo);
            }

            Postgis_DungHenGanMenu();
        }

        private void Postgis_LamMoiThuCong(
            object sender,
            EventArgs e)
        {
            NapLaiLopDiemPostGIS();
        }

        // =====================================================
        // 3. KIEM TRA KY HIEU LOGO
        // =====================================================

        private void Postgis_KiemTraKyHieu(
            object sender,
            EventArgs e)
        {
            string jpg = Postgis_TimLogoDonVi();

            string png = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ManagerGISAcademy",
                "Cache",
                "LogoDonVi_MapWinGIS_18px.png");

            string thongTinLop =
                "Chưa nạp được lớp điểm PostGIS.";

            try
            {
                if (banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed &&
                    handleLopDiemPostGIS >= 0 &&
                    ketNoiMapWinGIS != null &&
                    ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS))
                {
                    Shapefile sf =
                        banDoMapWinGIS.get_Shapefile(
                            handleLopDiemPostGIS);

                    if (sf != null)
                    {
                        ShapeDrawingOptions opt =
                            sf.DefaultDrawingOptions;

                        thongTinLop =
                            "Handle: " + handleLopDiemPostGIS +
                            "\r\nSố điểm: " + sf.NumShapes +
                            "\r\nPointType: " + opt.PointType +
                            "\r\nPicture: " +
                            (opt.Picture == null
                                ? "NULL" : "Đã có ảnh") +
                            "\r\nSố nhóm ký hiệu: " +
                            (sf.Categories == null
                                ? 0 : sf.Categories.Count) +
                            "\r\nLogo dự kiến: " +
                            PostgisLogoKichThuoc + " px";
                    }
                }
            }
            catch (Exception ex)
            {
                thongTinLop =
                    "Lỗi kiểm tra lớp: " + ex.Message;
            }

            MessageBox.Show(
                this,
                "Ảnh nguồn: " +
                (jpg ?? "KHÔNG TÌM THẤY") +
                "\r\nPNG cache: " +
                (File.Exists(png) ? png : "CHƯA TẠO") +
                "\r\n\r\n" + thongTinLop +
                "\r\n\r\nNếu PointType=ptSymbolPicture " +
                "mà vẫn thấy ngôi sao, hãy thử ẩn lớp " +
                "'Đơn vị từ PostGIS' để kiểm tra có lớp " +
                "khác vẽ đè hay không.",
                "Kiểm tra ký hiệu đơn vị",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void Postgis_DungHenGanMenu()
        {
            if (postgisHenGanMenu == null)
                return;

            postgisHenGanMenu.Stop();
            postgisHenGanMenu.Tick -= Postgis_ThuGanMenu;
            postgisHenGanMenu.Dispose();
            postgisHenGanMenu = null;
        }

        private void Postgis_KhiDongForm(
            object sender,
            FormClosedEventArgs e)
        {
            Postgis_DungHenGanMenu();

            btnNavMap.Click -= Postgis_KhiChonBanDo;
            FormClosed -= Postgis_KhiDongForm;

            if (postgisMucLamMoi != null &&
                !postgisMucLamMoi.IsDisposed)
            {
                postgisMucLamMoi.Click -= Postgis_LamMoiThuCong;
            }

            if (postgisMucKiemTraLogo != null &&
                !postgisMucKiemTraLogo.IsDisposed)
            {
                postgisMucKiemTraLogo.Click -=
                    Postgis_KiemTraKyHieu;
            }

            if (postgisAnhLogoDonVi != null)
            {
                try
                {
                    postgisAnhLogoDonVi.Close();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[POSTGIS_LOGO_CLOSE] " + ex.Message);
                }

                postgisAnhLogoDonVi = null;
            }
        }

        // =====================================================
        // 4. TIM DUONG DAN LOGO
        // =====================================================

        private string Postgis_TimLogoDonVi()
        {
            DirectoryInfo thuMuc = new DirectoryInfo(
                AppDomain.CurrentDomain.BaseDirectory);

            for (int i = 0; i < 8 && thuMuc != null; i++)
            {
                string path = Path.Combine(
                    thuMuc.FullName,
                    "Resources",
                    "Images",
                    "LogoDonVi.jpg");

                if (File.Exists(path))
                    return path;

                thuMuc = thuMuc.Parent;
            }

            return null;
        }

        // =====================================================
        // 5. DOC JPG, TAO PNG 18 PX VA NAP VAO MAPWINGIS
        // =====================================================

        private MapWinGIS.Image Postgis_LayAnhLogoDonVi()
        {
            if (postgisAnhLogoDonVi != null)
                return postgisAnhLogoDonVi;

            string logoJpg = Postgis_TimLogoDonVi();

            if (string.IsNullOrWhiteSpace(logoJpg))
            {
                if (!postgisDaBaoThieuLogo)
                {
                    postgisDaBaoThieuLogo = true;

                    GhiNhatKyBanDo(
                        "POSTGIS_LOGO_MISSING | " +
                        "Resources\\Images\\LogoDonVi.jpg");

                    MessageBox.Show(
                        this,
                        "Không tìm thấy file:\r\n" +
                        "Resources\\Images\\LogoDonVi.jpg\r\n\r\n" +
                        "Hãy kiểm tra tên ảnh và thư mục dự án.",
                        "Thiếu ảnh ký hiệu",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return null;
            }

            try
            {
                string thuMucCache = Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "ManagerGISAcademy",
                    "Cache");

                Directory.CreateDirectory(thuMucCache);

                string logoPng = Path.Combine(
                    thuMucCache,
                    "LogoDonVi_MapWinGIS_18px.png");

                using (Bitmap jpg = new Bitmap(logoJpg))
                using (Bitmap catNen = Postgis_LamTrongNenTrang(jpg))
                using (Bitmap nho = new Bitmap(
                    PostgisLogoKichThuoc,
                    PostgisLogoKichThuoc,
                    PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(nho))
                    {
                        g.Clear(Color.Transparent);

                        g.SmoothingMode = SmoothingMode.AntiAlias;

                        g.InterpolationMode =
                            InterpolationMode.HighQualityBicubic;

                        g.PixelOffsetMode =
                            PixelOffsetMode.HighQuality;

                        g.CompositingQuality =
                            CompositingQuality.HighQuality;

                        // Giu nguyen ty le logo, khong lam meo.
                        float tyLe = Math.Min(
                            (float)(PostgisLogoKichThuoc - 2) /
                                Math.Max(1, catNen.Width),
                            (float)(PostgisLogoKichThuoc - 2) /
                                Math.Max(1, catNen.Height));

                        int rong = Math.Max(
                            1,
                            (int)Math.Round(catNen.Width * tyLe));

                        int cao = Math.Max(
                            1,
                            (int)Math.Round(catNen.Height * tyLe));

                        int x =
                            (PostgisLogoKichThuoc - rong) / 2;

                        int y =
                            (PostgisLogoKichThuoc - cao) / 2;

                        g.DrawImage(
                            catNen,
                            new Rectangle(x, y, rong, cao));
                    }

                    nho.Save(logoPng, ImageFormat.Png);
                }

                MapWinGIS.Image anh = new MapWinGIS.Image();

                bool moThanhCong = anh.Open(
                    logoPng,
                    ImageType.USE_FILE_EXTENSION,
                    true,
                    null);

                if (!moThanhCong)
                {
                    string loi =
                        anh.get_ErrorMsg(anh.LastErrorCode);

                    anh.Close();

                    GhiNhatKyBanDo(
                        "POSTGIS_LOGO_OPEN | " + loi);

                    if (!postgisDaBaoLoiLogo)
                    {
                        postgisDaBaoLoiLogo = true;

                        MessageBox.Show(
                            this,
                            "Không mở được logo PNG:\r\n" + loi,
                            "Lỗi ký hiệu đơn vị",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    return null;
                }

                postgisAnhLogoDonVi = anh;

                System.Diagnostics.Debug.WriteLine(
                    "[POSTGIS_LOGO] JPG=" + logoJpg +
                    " | PNG=" + logoPng);

                return anh;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "POSTGIS_LOGO_CREATE_FAILED | " + ex);

                if (!postgisDaBaoLoiLogo)
                {
                    postgisDaBaoLoiLogo = true;

                    MessageBox.Show(
                        this,
                        "Lỗi mở logo:\r\n" + ex.Message,
                        "Lỗi tạo ký hiệu đơn vị",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                return null;
            }
        }

        // =====================================================
        // 6. XU LY NEN TRANG JPG
        // =====================================================

        private static Bitmap Postgis_LamTrongNenTrang(
            Bitmap anhGoc)
        {
            int w = anhGoc.Width;
            int h = anhGoc.Height;

            Bitmap anh = new Bitmap(
                w, h, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(anh))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(anhGoc, 0, 0, w, h);
            }

            BitmapData khoa = null;

            try
            {
                khoa = anh.LockBits(
                    new Rectangle(0, 0, w, h),
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb);

                int stride = khoa.Stride;
                byte[] pixel = new byte[stride * h];

                System.Runtime.InteropServices.Marshal.Copy(
                    khoa.Scan0,
                    pixel,
                    0,
                    pixel.Length);

                bool[] daXet = new bool[w * h];
                Queue<int> doi = new Queue<int>();

                // Xoa nen trang lien thong tu vien anh.
                for (int x = 0; x < w; x++)
                {
                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x, 0, w, h, stride);

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x, h - 1, w, h, stride);
                }

                for (int y = 0; y < h; y++)
                {
                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        0, y, w, h, stride);

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        w - 1, y, w, h, stride);
                }

                while (doi.Count > 0)
                {
                    int p = doi.Dequeue();

                    int x = p % w;
                    int y = p / w;

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x - 1, y, w, h, stride);

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x + 1, y, w, h, stride);

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x, y - 1, w, h, stride);

                    Postgis_ThemNenTrang(
                        pixel, daXet, doi,
                        x, y + 1, w, h, stride);
                }

                int minX = w;
                int minY = h;
                int maxX = -1;
                int maxY = -1;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int a = y * stride + x * 4 + 3;

                        if (pixel[a] == 0)
                            continue;

                        if (x < minX) minX = x;
                        if (x > maxX) maxX = x;
                        if (y < minY) minY = y;
                        if (y > maxY) maxY = y;
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(
                    pixel,
                    0,
                    khoa.Scan0,
                    pixel.Length);

                anh.UnlockBits(khoa);
                khoa = null;

                if (maxX < 0)
                    return anh;

                minX = Math.Max(0, minX - 1);
                minY = Math.Max(0, minY - 1);

                maxX = Math.Min(w - 1, maxX + 1);
                maxY = Math.Min(h - 1, maxY + 1);

                Bitmap cat = anh.Clone(
                    Rectangle.FromLTRB(
                        minX, minY,
                        maxX + 1, maxY + 1),
                    PixelFormat.Format32bppArgb);

                anh.Dispose();
                return cat;
            }
            finally
            {
                if (khoa != null)
                    anh.UnlockBits(khoa);
            }
        }

        private static void Postgis_ThemNenTrang(
            byte[] pixel,
            bool[] daXet,
            Queue<int> doi,
            int x,
            int y,
            int w,
            int h,
            int stride)
        {
            if (x < 0 || y < 0 || x >= w || y >= h)
                return;

            int viTri = y * w + x;

            if (daXet[viTri])
                return;

            daXet[viTri] = true;

            int offset = y * stride + x * 4;

            int b = pixel[offset];
            int g = pixel[offset + 1];
            int r = pixel[offset + 2];
            int a = pixel[offset + 3];

            bool mauTrang =
                r >= 225 &&
                g >= 225 &&
                b >= 225 &&
                Math.Max(r, Math.Max(g, b)) -
                Math.Min(r, Math.Min(g, b)) <= 25;

            if (a > 10 && !mauTrang)
                return;

            pixel[offset + 3] = 0;

            doi.Enqueue(viTri);
        }

        // =====================================================
        // 7. AP DUNG LOGO CHO SHAPEFILE
        // =====================================================

        private bool Postgis_ApDungKyHieuLogo(Shapefile lopDiem)
        {
            if (lopDiem == null)
                return false;

            ShapeDrawingOptions kyHieu =
                lopDiem.DefaultDrawingOptions;

            MapWinGIS.Image logo = Postgis_LayAnhLogoDonVi();

            if (logo == null)
            {
                // Phuong an du phong neu khong mo duoc logo.
                kyHieu.PointSize = 11f;

                kyHieu.FillColor = unchecked((uint)
                    ColorTranslator.ToOle(
                        Color.FromArgb(16, 185, 129)));

                return false;
            }

            Postgis_DatAnhChoKieuVe(kyHieu, logo);

            // Neu lop co cac nhom ky hieu rieng,
            // ap dung logo vao tat ca cac nhom.
            if (lopDiem.Categories != null)
            {
                for (int i = 0;
                     i < lopDiem.Categories.Count;
                     i++)
                {
                    ShapefileCategory nhom =
                        lopDiem.Categories.get_Item(i);

                    if (nhom != null &&
                        nhom.DrawingOptions != null)
                    {
                        Postgis_DatAnhChoKieuVe(
                            nhom.DrawingOptions,
                            logo);
                    }
                }
            }

            lopDiem.CollisionMode =
                tkCollisionMode.AllowCollisions;

            bool daGan =
                lopDiem.DefaultDrawingOptions.PointType ==
                    tkPointSymbolType.ptSymbolPicture &&
                lopDiem.DefaultDrawingOptions.Picture != null;

            System.Diagnostics.Debug.WriteLine(
                "[POSTGIS_LOGO] Gan marker=" + daGan +
                " | Shapes=" + lopDiem.NumShapes +
                " | Categories=" +
                (lopDiem.Categories == null
                    ? 0 : lopDiem.Categories.Count));

            return daGan;
        }

        private static void Postgis_DatAnhChoKieuVe(
            ShapeDrawingOptions kyHieu,
            MapWinGIS.Image logo)
        {
            kyHieu.PointType =
                tkPointSymbolType.ptSymbolPicture;

            kyHieu.Picture = logo;

            kyHieu.PointSize =
                PostgisLogoKichThuoc;

            kyHieu.AlignPictureByBottom = false;
        }

        // =====================================================
        // 8. NAP DU LIEU POSTGIS VA TAO LOP DIEM
        // =====================================================

        public void NapLaiLopDiemPostGIS()
        {
            if (dangNapLopDiemPostGIS ||
                IsDisposed ||
                Disposing)
                return;

            if (banDoMapWinGIS == null ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                statusGeneral.Text =
                    "● MapWinGIS chưa sẵn sàng nạp điểm.";

                return;
            }

            if (nghiepVuDonVi == null ||
                !nghiepVuDonVi.DaCauHinh ||
                !CoQuyen(ThaoTacDonViQuanDoi.Xem))
            {
                XoaLopDiemPostGISCu();
                CapNhatNhanBanDo();

                statusGeneral.Text =
                    "● Chưa có quyền xem hồ sơ " +
                    "hoặc chưa cấu hình BLL.";

                return;
            }

            bool laLanDau =
                handleLopDiemPostGIS < 0;

            // Giai doan 3: moi don vi da duoc ve bang lop LOGO
            // (v_ban_do_don_vi). Lop diem ho so nay trung vi tri nen
            // MAC DINH AN; van giu trong danh sach lop de bat khi can.
            bool hienThi = false;
            int handleMoi = -1;

            dangNapLopDiemPostGIS = true;

            System.Diagnostics.Stopwatch dongHo =
                System.Diagnostics.Stopwatch.StartNew();

            long miligiayLayDuLieu = 0L;

            try
            {
                if (!laLanDau &&
                    ketNoiMapWinGIS.CoLop(
                        handleLopDiemPostGIS))
                {
                    hienThi =
                        ketNoiMapWinGIS.LayHienThiLop(
                            handleLopDiemPostGIS);
                }

                System.Diagnostics.Stopwatch dongHoCSDL =
                    System.Diagnostics.Stopwatch.StartNew();

                List<DonViQuanDoi> danhSach =
                    nghiepVuDonVi.LayTatCaDonVi();

                dongHoCSDL.Stop();

                miligiayLayDuLieu =
                    dongHoCSDL.ElapsedMilliseconds;

                if (danhSach == null)
                {
                    throw new InvalidOperationException(
                        "BLL trả về danh sách đơn vị null.");
                }

                Shapefile lopDiem = new Shapefile();

                if (!lopDiem.CreateNewWithShapeID(
                    "",
                    ShpfileType.SHP_POINT))
                {
                    throw new InvalidOperationException(
                        "Không tạo được Shapefile: " +
                        lopDiem.get_ErrorMsg(
                            lopDiem.LastErrorCode));
                }

                if (!lopDiem.GeoProjection.CopyFrom(
                    banDoMapWinGIS.GeoProjection))
                {
                    throw new InvalidOperationException(
                        "Không đồng bộ được hệ quy chiếu " +
                        "của lớp điểm.");
                }

                int cotMa = lopDiem.EditAddField(
                    "MA_DON_VI",
                    FieldType.STRING_FIELD,
                    0,
                    50);

                int cotTen = lopDiem.EditAddField(
                    "TEN_DON_VI",
                    FieldType.STRING_FIELD,
                    0,
                    254);

                if (cotMa < 0 || cotTen < 0)
                {
                    throw new InvalidOperationException(
                        "Không tạo được các cột thuộc tính.");
                }

                int soBoQua = 0;

                foreach (DonViQuanDoi donVi in danhSach)
                {
                    if (donVi == null ||
                        !NghiepVuDonVi.CoToaDoWgs84(donVi))
                    {
                        soBoQua++;
                        continue;
                    }

                    double x = 0d;
                    double y = 0d;

                    bool daChuyen =
                        banDoMapWinGIS.DegreesToProj(
                            donVi.KinhDo,
                            donVi.ViDo,
                            ref x,
                            ref y);

                    if (!daChuyen ||
                        double.IsNaN(x) ||
                        double.IsNaN(y) ||
                        double.IsInfinity(x) ||
                        double.IsInfinity(y))
                    {
                        soBoQua++;
                        continue;
                    }

                    Shape diem = new Shape();

                    if (!diem.Create(ShpfileType.SHP_POINT))
                    {
                        throw new InvalidOperationException(
                            "Không tạo được điểm " +
                            donVi.MaDonVi);
                    }

                    if (diem.AddPoint(x, y) < 0)
                    {
                        throw new InvalidOperationException(
                            "Không ghi được tọa độ " +
                            donVi.MaDonVi);
                    }

                    int hang = lopDiem.EditAddShape(diem);

                    if (hang < 0)
                    {
                        throw new InvalidOperationException(
                            "Không chèn được điểm " +
                            donVi.MaDonVi);
                    }

                    string ma =
                        donVi.MaDonVi ?? string.Empty;

                    string ten =
                        donVi.TenDonVi ?? string.Empty;

                    if (ma.Length > 50)
                        ma = ma.Substring(0, 50);

                    if (ten.Length > 254)
                        ten = ten.Substring(0, 254);

                    if (!lopDiem.EditCellValue(
                            cotMa, hang, ma) ||
                        !lopDiem.EditCellValue(
                            cotTen, hang, ten))
                    {
                        throw new InvalidOperationException(
                            "Không ghi được thuộc tính của " +
                            ma);
                    }
                }

                if (lopDiem.NumShapes == 0)
                {
                    XoaLopDiemPostGISCu();
                    CapNhatNhanBanDo();

                    statusGeneral.Text =
                        "● Không có hồ sơ với tọa độ hợp lệ.";

                    postgisLanNapThanhCongUtc =
                        DateTime.UtcNow;

                    return;
                }

                // Logo duoc gan sau khi AddLayer.
                bool daDungLogo = false;

                bool tuKhoaBanDo = false;

                try
                {
                    if (banDoMapWinGIS.IsLocked ==
                        tkLockMode.lmUnlock)
                    {
                        banDoMapWinGIS.LockWindow(
                            tkLockMode.lmLock);

                        tuKhoaBanDo = true;
                    }

                    handleMoi =
                        banDoMapWinGIS.AddLayer(
                            lopDiem,
                            hienThi);

                    if (handleMoi < 0 ||
                        !ketNoiMapWinGIS.CoLop(handleMoi))
                    {
                        throw new InvalidOperationException(
                            "MapWinGIS không nhận được " +
                            "lớp điểm PostGIS.");
                    }

                    // Lay Shapefile THUC TE sau AddLayer.
                    Shapefile lopThucTe =
                        banDoMapWinGIS.get_Shapefile(
                            handleMoi);

                    daDungLogo =
                        Postgis_ApDungKyHieuLogo(lopThucTe);

                    ketNoiMapWinGIS.DuaLopLenTrenCung(
                        handleMoi);

                    // Chi xoa lop cu khi lop moi thanh cong.
                    XoaLopDiemPostGISCu();

                    handleLopDiemPostGIS = handleMoi;
                    handleMoi = -1;
                }
                finally
                {
                    if (tuKhoaBanDo &&
                        !banDoMapWinGIS.IsDisposed)
                    {
                        banDoMapWinGIS.LockWindow(
                            tkLockMode.lmUnlock);
                    }
                }

                LopMapItem mucLop = new LopMapItem
                {
                    Handle = handleLopDiemPostGIS,

                    Ten =
                        "Đơn vị quân đội - hồ sơ quản lý (" +
                        lopDiem.NumShapes +
                        " điểm)"
                };

                bool dangCapNhatCu = dangCapNhatLop;

                dangCapNhatLop = true;

                try
                {
                    cacLop.Add(mucLop);
                    danhSachLop.Items.Add(mucLop, hienThi);
                }
                finally
                {
                    dangCapNhatLop = dangCapNhatCu;
                }

                if (laLanDau)
                {
                    try
                    {
                        ketNoiMapWinGIS.PhongToanCanh();
                    }
                    catch (Exception exZoom)
                    {
                        GhiNhatKyBanDo(
                            "POSTGIS_FIRST_ZOOM_FAILED | " +
                            exZoom);
                    }
                }

                // Gan nhan nhu code cu.
                Bd11_DatLaiCacheNhan();
                Bd11_GanNhanDonVi();

                // Ap dung logo lan nua sau khi tao nhan.
                // Tranh module khac ghi de style.
                if (handleLopDiemPostGIS >= 0)
                {
                    Shapefile lopThucTe =
                        banDoMapWinGIS.get_Shapefile(
                            handleLopDiemPostGIS);

                    daDungLogo =
                        Postgis_ApDungKyHieuLogo(lopThucTe);
                }

                Bd11_YeuCauVeLaiBanDo();
                HenCapNhatChuGiai();
                CapNhatNhanBanDo();

                postgisLanNapThanhCongUtc =
                    DateTime.UtcNow;

                statusGeneral.Text =
                    "● Đã hiển thị " +
                    lopDiem.NumShapes +
                    " điểm từ PostGIS." +
                    (daDungLogo
                        ? " Ký hiệu: logo đơn vị."
                        : " Chưa áp dụng được logo đơn vị.") +
                    (soBoQua > 0
                        ? " Bỏ qua " +
                          soBoQua +
                          " hồ sơ sai tọa độ."
                        : string.Empty);
            }
            catch (Exception ex)
            {
                if (handleMoi >= 0)
                {
                    try
                    {
                        if (ketNoiMapWinGIS != null &&
                            ketNoiMapWinGIS.CoLop(handleMoi))
                        {
                            ketNoiMapWinGIS.XoaLop(
                                handleMoi);
                        }
                    }
                    catch
                    {
                        // Giu nguyen loi ban dau.
                    }
                }

                GhiNhatKyBanDo(
                    "POSTGIS_LAYER_FAILED | " + ex);

                statusGeneral.Text =
                    "● Lỗi hiển thị điểm PostGIS: " +
                    ex.Message;

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không nạp được lớp điểm PostGIS",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                dangNapLopDiemPostGIS = false;
                dongHo.Stop();

                System.Diagnostics.Debug.WriteLine(
                    "[PostGIS] POINT_LAYER_LOAD | db_ms=" +
                    miligiayLayDuLieu +
                    " | total_ms=" +
                    dongHo.ElapsedMilliseconds +
                    " | handle=" +
                    handleLopDiemPostGIS);
            }
        }

        // =====================================================
        // 9. DUA LOP DIEM LEN TREN
        // =====================================================

        private void DuaLopDiemLenTren()
        {
            if (dangNapLopDiemPostGIS ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                handleLopDiemPostGIS < 0)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(
                    handleLopDiemPostGIS))
                    return;

                int viTri =
                    ketNoiMapWinGIS.LayViTriLop(
                        handleLopDiemPostGIS);

                if (viTri >= 0 &&
                    viTri < banDoMapWinGIS.NumLayers - 1)
                {
                    ketNoiMapWinGIS.DuaLopLenTrenCung(
                        handleLopDiemPostGIS);
                }

                // Ap dung lai logo neu module khac doi style.
                Shapefile lopDangHienThi =
                    banDoMapWinGIS.get_Shapefile(
                        handleLopDiemPostGIS);

                if (lopDangHienThi != null)
                {
                    Postgis_ApDungKyHieuLogo(
                        lopDangHienThi);
                }

                Bd11_YeuCauVeLaiBanDo();
                HenCapNhatChuGiai();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "POSTGIS_LAYER_ORDER_FAILED | " + ex);

                statusGeneral.Text =
                    "● Không sắp xếp được lớp điểm: " +
                    ex.Message;
            }
        }

        // =====================================================
        // 10. XOA LOP DIEM CU
        // =====================================================

        private void XoaLopDiemPostGISCu()
        {
            if (handleLopDiemPostGIS < 0)
                return;

            int handleCu = handleLopDiemPostGIS;

            if (ketNoiMapWinGIS != null &&
                ketNoiMapWinGIS.CoLop(handleCu))
            {
                ketNoiMapWinGIS.XoaLop(handleCu);
            }

            bool dangCapNhatCu = dangCapNhatLop;
            dangCapNhatLop = true;

            try
            {
                for (int i =
                    danhSachLop.Items.Count - 1;
                    i >= 0;
                    i--)
                {
                    LopMapItem muc =
                        danhSachLop.Items[i] as LopMapItem;

                    if (muc != null &&
                        muc.Handle == handleCu)
                    {
                        danhSachLop.Items.RemoveAt(i);
                    }
                }

                cacLop.RemoveAll(muc =>
                    muc != null &&
                    muc.Handle == handleCu);
            }
            finally
            {
                dangCapNhatLop = dangCapNhatCu;
            }

            handleLopDiemPostGIS = -1;
            Bd11_DatLaiCacheNhan();
        }
    }
}
