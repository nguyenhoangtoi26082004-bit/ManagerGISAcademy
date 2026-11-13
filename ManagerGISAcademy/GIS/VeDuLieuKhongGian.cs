
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using AxMapWinGIS;
using MapWinGIS;

namespace ManagerGISAcademy.GIS
{
    /// <summary>
    /// Một vị trí trong hệ tọa độ hiện tại của AxMap (không mặc định là WGS84).
    /// Với bản đồ EPSG:32648, X/Y là tọa độ phẳng theo mét.
    /// </summary>
    public struct ToaDoVe
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public ToaDoVe(double x, double y)
        {
            if (!LaSoHuuHan(x) || !LaSoHuuHan(y))
                throw new ArgumentOutOfRangeException("x", "Tọa độ phải là số hữu hạn.");
            X = x;
            Y = y;
        }

        internal static bool LaSoHuuHan(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }

    /// <summary>
    /// Thông tin đọc-only của một lớp vẽ tạm do SpatialDrawer sở hữu.
    /// DrawingHandle không phải LayerHandle của LayerManager.
    /// </summary>
    public sealed class ThongTinLopVe
    {
        public string MaLop { get; private set; }
        public int DrawingHandle { get; private set; }
        public int SoThaoTacVe { get; private set; }

        internal ThongTinLopVe(string maLop, int handle, int soThaoTac)
        {
            MaLop = maLop;
            DrawingHandle = handle;
            SoThaoTacVe = soThaoTac;
        }
    }

    /// <summary>
    /// Vẽ đồ họa tạm trên AxMap bằng drawing layer, không tạo/sửa Shapefile.
    /// Dữ liệu đầu vào phải cùng CRS với bản đồ (ngoại trừ hàm chuyển WGS84).
    /// Tất cả thao tác phải chạy trên luồng WinForms sở hữu AxMap.
    /// C# 7.3 / .NET Framework 4.8 / MapWinGIS ActiveX.
    /// </summary>
    public sealed class VeDuLieuKhongGian : IDisposable
    {
        private sealed class LopVeNoiBo
        {
            public string MaLop;
            public int Handle;
            public int SoThaoTac;
        }

        private readonly AxMap banDo;
        private readonly int maLuongGiaoDien;
        private readonly Dictionary<string, LopVeNoiBo> cacLop;
        private bool daGiaiPhong;

        public VeDuLieuKhongGian(AxMap dieuKhienBanDo)
        {
            if (dieuKhienBanDo == null)
                throw new ArgumentNullException("dieuKhienBanDo");
            if (dieuKhienBanDo.IsDisposed)
                throw new ObjectDisposedException("dieuKhienBanDo");
            if (dieuKhienBanDo.IsHandleCreated && dieuKhienBanDo.InvokeRequired)
                throw new InvalidOperationException("Phải tạo SpatialDrawer trên luồng sở hữu AxMap.");

            banDo = dieuKhienBanDo;
            maLuongGiaoDien = Thread.CurrentThread.ManagedThreadId;
            cacLop = new Dictionary<string, LopVeNoiBo>(StringComparer.OrdinalIgnoreCase);
        }

        public VeDuLieuKhongGian(TroGiupMapWinGIS helper)
            : this(helper == null
                ? throw new ArgumentNullException("helper")
                : helper.DieuKhienBanDo)
        {
        }

        public AxMap DieuKhienBanDo { get { return banDo; } }
        public int SoLopVe { get { KiemTraLuongVaDoiTuong(); return cacLop.Count; } }

        public IList<ThongTinLopVe> LayDanhSachLopVe()
        {
            KiemTraLuongVaDoiTuong();
            return cacLop.Values
                .Select(x => new ThongTinLopVe(x.MaLop, x.Handle, x.SoThaoTac))
                .ToList().AsReadOnly();
        }

        public bool CoLopVe(string maLop)
        {
            KiemTraLuongVaDoiTuong();
            return cacLop.ContainsKey(KiemTraMaLop(maLop));
        }

        /// <summary>
        /// Tạo drawing layer trong tọa độ không gian của bản đồ.
        /// Nếu mã đã được tạo bởi đối tượng này, trả về handle đang quản lý.
        /// Không đăng ký drawing layer vào LayerManager (chỉ quản lý data layer).
        /// </summary>
        public int TaoLopVe(string maLop)
        {
            KiemTraSanSang();
            string ma = KiemTraMaLop(maLop);
            LopVeNoiBo lop;
            if (cacLop.TryGetValue(ma, out lop))
                return lop.Handle;

            int handle = banDo.NewDrawing(tkDrawReferenceList.dlSpatiallyReferencedList);
            if (handle < 0)
                throw new InvalidOperationException("MapWinGIS không tạo được lớp vẽ tạm: " + ma);

            lop = new LopVeNoiBo { MaLop = ma, Handle = handle, SoThaoTac = 0 };
            cacLop.Add(ma, lop);
            return handle;
        }

        /// <summary>
        /// Vẽ một điểm. Kích thước sử dụng pixel; X/Y là tọa độ bản đồ.
        /// </summary>
        public void VeDiem(string maLop, ToaDoVe diem, Color mau,
            int kichThuocPixel = 12, byte doDuc = 255)
        {
            KiemTraSanSang();
            KiemTraMau(mau);
            KiemTraPixel(kichThuocPixel, "kichThuocPixel", 1, 200);
            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            banDo.DrawPointEx(lop.Handle, diem.X, diem.Y,
                kichThuocPixel, MauOle(mau), doDuc);
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Vẽ một đoạn thẳng trong tọa độ không gian của bản đồ.
        /// </summary>
        public void VeDoanThang(string maLop, ToaDoVe dau, ToaDoVe cuoi,
            Color mau, int doDayPixel = 2, byte doDuc = 255)
        {
            KiemTraSanSang();
            KiemTraMau(mau);
            KiemTraPixel(doDayPixel, "doDayPixel", 1, 100);
            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            banDo.DrawLineEx(lop.Handle, dau.X, dau.Y,
                cuoi.X, cuoi.Y, doDayPixel, MauOle(mau), doDuc);
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Vẽ đường gấp khúc bằng các đoạn nối tiếp, không tạo geometry PostGIS.
        /// Chỉ dùng tập điểm không quá 20.000 đỉnh để tránh chặn luồng UI.
        /// </summary>
        public void VeDuongGapKhuc(string maLop, IEnumerable<ToaDoVe> cacDiem,
            Color mau, int doDayPixel = 3, byte doDuc = 255)
        {
            KiemTraSanSang();
            KiemTraMau(mau);
            KiemTraPixel(doDayPixel, "doDayPixel", 1, 100);
            List<ToaDoVe> diem = LayDiemHopLe(cacDiem, 2);
            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            uint color = MauOle(mau);
            for (int i = 1; i < diem.Count; i++)
            {
                banDo.DrawLineEx(lop.Handle,
                    diem[i - 1].X, diem[i - 1].Y,
                    diem[i].X, diem[i].Y, doDayPixel, color, doDuc);
            }
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Vẽ đa giác đơn, không có lỗ. Không kiểm tra topology và không tính diện tích.
        /// Có thể dùng để HIỂN THỊ vùng đệm đã được BLL/GIS tính trước,
        /// nhưng không được hiểu đây là phép tính Buffer.
        /// </summary>
        public void VeDaGiac(string maLop, IEnumerable<ToaDoVe> cacDinh,
            Color mauNen, Color mauVien, bool toMau = true,
            byte doDucNen = 80, int doDayVienPixel = 2)
        {
            KiemTraSanSang();
            KiemTraMau(mauNen);
            KiemTraMau(mauVien);
            KiemTraPixel(doDayVienPixel, "doDayVienPixel", 1, 100);
            List<ToaDoVe> dinh = LayDiemHopLe(cacDinh, 3);

            // Bỏ điểm cuối trùng điểm đầu; phần viền được đóng riêng.
            if (dinh.Count > 3 && CungToaDo(dinh[0], dinh[dinh.Count - 1]))
                dinh.RemoveAt(dinh.Count - 1);
            if (dinh.Count < 3 || dinh.Distinct(new SoSanhDiem()).Count() < 3)
                throw new ArgumentException("Đa giác cần ít nhất ba đỉnh khác nhau.", "cacDinh");

            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            if (toMau)
            {
                double[] xs = dinh.Select(x => x.X).ToArray();
                double[] ys = dinh.Select(x => x.Y).ToArray();
                object xObj = xs;
                object yObj = ys;
                banDo.DrawPolygonEx(lop.Handle, ref xObj, ref yObj,
                    dinh.Count, MauOle(mauNen), true, doDucNen);
            }

            uint vien = MauOle(mauVien);
            for (int i = 0; i < dinh.Count; i++)
            {
                ToaDoVe dau = dinh[i];
                ToaDoVe cuoi = dinh[(i + 1) % dinh.Count];
                banDo.DrawLineEx(lop.Handle, dau.X, dau.Y, cuoi.X, cuoi.Y,
                    doDayVienPixel, vien, 255);
            }
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Vẽ hình tròn tại tọa độ bản đồ, BÁN KÍNH LÀ PIXEL, không phải mét.
        /// Không dùng phương thức này để tạo vùng đệm địa lý.
        /// </summary>
        public void VeVongTronPixel(string maLop, ToaDoVe tam,
            double banKinhPixel, Color mau, bool toMau = false,
            short doDayVienPixel = 2, byte doDuc = 200)
        {
            KiemTraSanSang();
            KiemTraMau(mau);
            if (!ToaDoVe.LaSoHuuHan(banKinhPixel) ||
                banKinhPixel <= 0 || banKinhPixel > 10000)
                throw new ArgumentOutOfRangeException("banKinhPixel");
            if (doDayVienPixel < 1 || doDayVienPixel > 100)
                throw new ArgumentOutOfRangeException("doDayVienPixel");

            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            banDo.DrawWideCircleEx(lop.Handle, tam.X, tam.Y,
                banKinhPixel, MauOle(mau), toMau, doDayVienPixel, doDuc);
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Thêm một nhãn văn bản vào lớp vẽ tạm. Hàm DrawLabelEx trả -1 nếu lỗi.
        /// </summary>
        public void VeNhan(string maLop, string noiDung, ToaDoVe viTri,
            double gocXoayDo = 0)
        {
            KiemTraSanSang();
            if (string.IsNullOrWhiteSpace(noiDung))
                throw new ArgumentException("Nội dung nhãn không được rỗng.", "noiDung");
            if (noiDung.Length > 300)
                throw new ArgumentOutOfRangeException("noiDung", "Nhãn tối đa 300 ký tự.");
            if (!ToaDoVe.LaSoHuuHan(gocXoayDo) || gocXoayDo < -360 || gocXoayDo > 360)
                throw new ArgumentOutOfRangeException("gocXoayDo");

            LopVeNoiBo lop = LayHoacTaoLop(maLop);
            int nhanHandle = banDo.DrawLabelEx(lop.Handle, noiDung.Trim(),
                viTri.X, viTri.Y, gocXoayDo);
            if (nhanHandle < 0)
                throw new InvalidOperationException("MapWinGIS không thêm được nhãn bản đồ.");
            lop.SoThaoTac++;
            banDo.Redraw();
        }

        /// <summary>
        /// Chuyển WGS84 (kinh độ, vĩ độ) sang hệ tọa độ bản đồ đã được khai báo.
        /// Không phỏng đoán hệ quy chiếu và không thay CRS của AxMap.
        /// </summary>
        public ToaDoVe ChuyenWgs84SangBanDo(double kinhDo, double viDo)
        {
            KiemTraSanSang();
            if (!ToaDoVe.LaSoHuuHan(kinhDo) || kinhDo < -180 || kinhDo > 180)
                throw new ArgumentOutOfRangeException("kinhDo");
            if (!ToaDoVe.LaSoHuuHan(viDo) || viDo < -90 || viDo > 90)
                throw new ArgumentOutOfRangeException("viDo");

            double x = 0;
            double y = 0;
            if (!banDo.DegreesToProj(kinhDo, viDo, ref x, ref y) ||
                !ToaDoVe.LaSoHuuHan(x) || !ToaDoVe.LaSoHuuHan(y))
                throw new InvalidOperationException(
                    "Không chuyển được WGS84 sang tọa độ bản đồ. Hãy kiểm tra CRS của AxMap.");
            return new ToaDoVe(x, y);
        }

        /// <summary>Hàm tiện ích: chuyển WGS84 trước rồi vẽ điểm.</summary>
        public void VeDiemWgs84(string maLop, double kinhDo, double viDo,
            Color mau, int kichThuocPixel = 12, byte doDuc = 255)
        {
            ToaDoVe diem = ChuyenWgs84SangBanDo(kinhDo, viDo);
            VeDiem(maLop, diem, mau, kichThuocPixel, doDuc);
        }

        /// <summary>
        /// Xóa riêng một drawing layer do SpatialDrawer quản lý.
        /// Không xóa data layer trong LayerManager và không gọi ClearDrawings().
        /// </summary>
        public bool XoaLopVe(string maLop)
        {
            KiemTraSanSang();
            string ma = KiemTraMaLop(maLop);
            LopVeNoiBo lop;
            if (!cacLop.TryGetValue(ma, out lop)) return false;

            banDo.ClearDrawing(lop.Handle);
            cacLop.Remove(ma);
            banDo.Redraw();
            return true;
        }

        /// <summary>
        /// Xóa hình cũ và tạo drawing layer mới cùng tên.
        /// DrawingHandle thay đổi sau khi gọi hàm này.
        /// </summary>
        public int LamMoiLopVe(string maLop)
        {
            KiemTraSanSang();
            string ma = KiemTraMaLop(maLop);
            XoaLopVe(ma);
            return TaoLopVe(ma);
        }

        /// <summary>
        /// Chỉ xóa những drawing layer do đối tượng này tạo.
        /// Các lớp khác trên bản đồ không bị tác động.
        /// </summary>
        public void XoaTatCaLopVe()
        {
            KiemTraSanSang();
            List<string> maLop = cacLop.Keys.ToList();
            foreach (string ma in maLop)
                XoaLopVe(ma);
        }

        public void VeLaiBanDo()
        {
            KiemTraSanSang();
            banDo.Redraw();
        }

        private LopVeNoiBo LayHoacTaoLop(string maLop)
        {
            string ma = KiemTraMaLop(maLop);
            LopVeNoiBo lop;
            if (!cacLop.TryGetValue(ma, out lop))
            {
                TaoLopVe(ma);
                lop = cacLop[ma];
            }
            return lop;
        }

        private static string KiemTraMaLop(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
                throw new ArgumentException("Mã lớp vẽ tạm không được rỗng.", "maLop");
            string ma = maLop.Trim();
            if (ma.Length > 100)
                throw new ArgumentOutOfRangeException("maLop", "Mã lớp tối đa 100 ký tự.");
            return ma;
        }

        private static List<ToaDoVe> LayDiemHopLe(
            IEnumerable<ToaDoVe> danhSach, int itNhat)
        {
            if (danhSach == null)
                throw new ArgumentNullException("danhSach");
            List<ToaDoVe> ketQua = new List<ToaDoVe>();
            foreach (ToaDoVe diem in danhSach)
            {
                if (!ToaDoVe.LaSoHuuHan(diem.X) || !ToaDoVe.LaSoHuuHan(diem.Y))
                    throw new ArgumentException("Danh sách có tọa độ không hợp lệ.", "danhSach");
                if (ketQua.Count >= 20000)
                    throw new ArgumentException("Tối đa 20.000 đỉnh cho lớp vẽ tạm.", "danhSach");
                ketQua.Add(diem);
            }
            if (ketQua.Count < itNhat)
                throw new ArgumentException("Danh sách không đủ số đỉnh.", "danhSach");
            return ketQua;
        }

        private static bool CungToaDo(ToaDoVe a, ToaDoVe b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        private sealed class SoSanhDiem : IEqualityComparer<ToaDoVe>
        {
            public bool Equals(ToaDoVe a, ToaDoVe b) { return CungToaDo(a, b); }
            public int GetHashCode(ToaDoVe p)
            {
                unchecked { return (p.X.GetHashCode() * 397) ^ p.Y.GetHashCode(); }
            }
        }

        private static void KiemTraMau(Color mau)
        {
            if (mau.IsEmpty)
                throw new ArgumentException("Màu vẽ không hợp lệ.", "mau");
        }

        private static void KiemTraPixel(int pixel, string ten, int min, int max)
        {
            if (pixel < min || pixel > max)
                throw new ArgumentOutOfRangeException(ten,
                    "Kích thước phải nằm trong khoảng " + min + " đến " + max + " pixel.");
        }

        private static uint MauOle(Color mau)
        {
            return unchecked((uint)ColorTranslator.ToOle(Color.FromArgb(mau.R, mau.G, mau.B)));
        }

        private void KiemTraLuongVaDoiTuong()
        {
            if (daGiaiPhong)
                throw new ObjectDisposedException("SpatialDrawer");
            if (Thread.CurrentThread.ManagedThreadId != maLuongGiaoDien)
                throw new InvalidOperationException(
                    "Chỉ được sử dụng SpatialDrawer trên luồng giao diện WinForms.");
        }

        private void KiemTraSanSang()
        {
            KiemTraLuongVaDoiTuong();
            if (banDo.IsDisposed || banDo.Disposing)
                throw new ObjectDisposedException("banDo");
            if (!banDo.IsHandleCreated || banDo.InvokeRequired)
                throw new InvalidOperationException(
                    "AxMap chưa sẵn sàng trên luồng giao diện. Hãy gọi sau khi Form hiển thị.");
        }

        /// <summary>
        /// Xóa các drawing layer đang quản lý khi còn AxMap hợp lệ.
        /// Không Dispose AxMap (thuộc quyền sở hữu của Form/UcBanDo).
        /// Hãy gọi Dispose trên luồng WinForms trước khi đóng Form.
        /// </summary>
        public void Dispose()
        {
            if (daGiaiPhong) return;
            KiemTraLuongVaDoiTuong();
            if (!banDo.IsDisposed && !banDo.Disposing && banDo.IsHandleCreated)
            {
                List<string> ds = cacLop.Keys.ToList();
                foreach (string ma in ds)
                    XoaLopVe(ma);
            }
            cacLop.Clear();
            daGiaiPhong = true;
            GC.SuppressFinalize(this);
        }
    }
}
