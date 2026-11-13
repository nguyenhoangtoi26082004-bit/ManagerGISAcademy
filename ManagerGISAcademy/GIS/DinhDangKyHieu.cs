
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using AxMapWinGIS;
using MapWinGIS;

namespace ManagerGISAcademy.GIS
{
    /// <summary>
    /// Hình dạng ký hiệu điểm dạng chuẩn của MapWinGIS.
    /// </summary>
    public enum HinhDangMarker
    {
        HinhTron,
        HinhVuong,
        HinhThoi,
        TamGiac,
        NgoiSao,
        DauCong,
        LaCo
    }

    /// <summary>
    /// Cấu hình hiển thị ký hiệu điểm (không chứa tọa độ hay dữ liệu GIS).
    /// DoDuc: 0 = trong suốt, 255 = hoàn toàn đục.
    /// </summary>
    public sealed class CauHinhMarker
    {
        public HinhDangMarker HinhDang { get; set; }
        public Color MauNen { get; set; }
        public Color MauVien { get; set; }
        public float KichThuoc { get; set; }
        public float DoDayVien { get; set; }
        public byte DoDuc { get; set; }

        public CauHinhMarker()
        {
            HinhDang = HinhDangMarker.HinhTron;
            MauNen = Color.FromArgb(5, 150, 105);
            MauVien = Color.White;
            KichThuoc = 14F;
            DoDayVien = 1.5F;
            DoDuc = 255;
        }

        public CauHinhMarker SaoChep()
        {
            return new CauHinhMarker
            {
                HinhDang = HinhDang,
                MauNen = MauNen,
                MauVien = MauVien,
                KichThuoc = KichThuoc,
                DoDayVien = DoDayVien,
                DoDuc = DoDuc
            };
        }
    }

    /// <summary>
    /// Định dạng ký hiệu điểm cho các lớp Shapefile đã có trên AxMap.
    /// Không tạo điểm, không thay hình học, không chuyển đổi CRS và không truy vấn CSDL.
    ///
    /// Các thao tác dùng LayerHandle (không phải vị trí lớp).
    /// Phải gọi trên luồng WinForms sở hữu điều khiển AxMap.
    /// Tương thích C# 7.3 / .NET Framework 4.8 / MapWinGIS ActiveX.
    /// </summary>
    public sealed class DinhDangKyHieu
    {
        private readonly AxMap banDo;
        private readonly int maLuongGiaoDien;

        public DinhDangKyHieu(AxMap dieuKhienBanDo)
        {
            if (dieuKhienBanDo == null)
                throw new ArgumentNullException("dieuKhienBanDo");
            if (dieuKhienBanDo.IsDisposed)
                throw new ObjectDisposedException("dieuKhienBanDo");
            if (dieuKhienBanDo.IsHandleCreated && dieuKhienBanDo.InvokeRequired)
                throw new InvalidOperationException(
                    "Hãy tạo MarkerStyler trên luồng giao diện đang sở hữu AxMap.");

            banDo = dieuKhienBanDo;
            maLuongGiaoDien = Thread.CurrentThread.ManagedThreadId;
        }

        /// <summary>
        /// Thuận tiện khi MapWinGISHelper đã được khởi tạo.
        /// </summary>
        public DinhDangKyHieu(TroGiupMapWinGIS helper)
            : this(helper == null
                ? throw new ArgumentNullException("helper")
                : helper.DieuKhienBanDo)
        {
        }

        public AxMap DieuKhienBanDo
        {
            get { return banDo; }
        }

        /// <summary>
        /// Kiểm tra lớp có phải Shapefile chứa điểm hoặc đa điểm (kể cả Z/M).
        /// </summary>
        public bool LaLopDiem(int layerHandle)
        {
            KiemTraSanSang();
            if (!TonTaiHandle(layerHandle))
                return false;

            Shapefile sf = banDo.get_Shapefile(layerHandle);
            return sf != null && LaKieuDiem(sf.ShapefileType);
        }

        /// <summary>
        /// Áp dụng một mẫu cho lớp điểm đã nạp trên bản đồ.
        /// Mặc định chỉ đổi DefaultDrawingOptions. Những đối tượng đang được
        /// gán danh mục riêng sẽ giữ phong cách riêng của chúng.
        /// Đặt ghiDeDanhMuc = true để áp dụng cùng mẫu cho tất cả danh mục.
        /// </summary>
        public void ApDungChoLopDiem(
            int layerHandle,
            CauHinhMarker cauHinh,
            bool ghiDeDanhMuc = false)
        {
            KiemTraSanSang();
            KiemTraCauHinh(cauHinh);
            Shapefile sf = LayShapefileDiem(layerHandle);

            // Sao lưu toàn bộ tùy chọn định sửa để phục hồi nếu phát sinh lỗi.
            List<TrangThaiTuyChon> truocKhiSua = new List<TrangThaiTuyChon>();
            ThemBanSao(truocKhiSua, sf.DefaultDrawingOptions);

            if (ghiDeDanhMuc)
            {
                ShapefileCategories danhMuc = sf.Categories;
                if (danhMuc != null)
                {
                    for (int i = 0; i < danhMuc.Count; i++)
                    {
                        ShapefileCategory muc = danhMuc.get_Item(i);
                        if (muc != null)
                            ThemBanSao(truocKhiSua, muc.DrawingOptions);
                    }
                }
            }

            try
            {
                foreach (TrangThaiTuyChon muc in truocKhiSua)
                    ApDungChoTuyChon(muc.TuyChon, cauHinh);

                banDo.Redraw();
            }
            catch
            {
                // Cố gắng trả lại trạng thái trước thao tác; giữ nguyên lỗi gốc.
                foreach (TrangThaiTuyChon muc in truocKhiSua)
                {
                    try { muc.TuyChon.Deserialize(muc.TrangThaiCu); }
                    catch { /* Khôi phục thất bại sẽ được kiểm tra khi chạy. */ }
                }
                try { banDo.Redraw(); }
                catch { /* Không ghi đè lỗi gốc. */ }
                throw;
            }
        }

        /// <summary>
        /// Định dạng trực tiếp một đối tượng ShapeDrawingOptions.
        /// Có thể dùng cho DefaultDrawingOptions hoặc DrawingOptions của
        /// một ShapefileCategory cụ thể. Hàm này không gọi Redraw().
        /// </summary>
        public static void ApDungChoTuyChon(
            ShapeDrawingOptions tuyChon,
            CauHinhMarker cauHinh)
        {
            if (tuyChon == null)
                throw new ArgumentNullException("tuyChon");
            KiemTraCauHinh(cauHinh);

            // SetDefaultPointSymbol thiết lập đồng bộ kiểu và hình dạng.
            tuyChon.SetDefaultPointSymbol(ChuyenHinhDang(cauHinh.HinhDang));
            tuyChon.PointSize = cauHinh.KichThuoc;
            tuyChon.FillType = tkFillType.ftStandard;
            tuyChon.FillVisible = true;
            tuyChon.FillColor = ChuyenMauOle(cauHinh.MauNen);
            tuyChon.FillTransparency = cauHinh.DoDuc;
            tuyChon.LineVisible = cauHinh.DoDayVien > 0F;
            tuyChon.LineColor = ChuyenMauOle(cauHinh.MauVien);
            tuyChon.LineTransparency = 255F;
            tuyChon.LineWidth = cauHinh.DoDayVien;
        }

        /// <summary>
        /// Mẫu mặc định: điểm tròn màu xanh, nền phù hợp giao diện tối.
        /// </summary>
        public static CauHinhMarker TaoMauMacDinh()
        {
            return new CauHinhMarker();
        }

        /// <summary>
        /// Mẫu làm nổi bật (nên áp dụng cho lớp điểm tạm riêng, tránh tô lại toàn lớp).
        /// </summary>
        public static CauHinhMarker TaoMauNoiBat()
        {
            return new CauHinhMarker
            {
                HinhDang = HinhDangMarker.NgoiSao,
                MauNen = Color.FromArgb(251, 191, 36),
                MauVien = Color.FromArgb(15, 23, 42),
                KichThuoc = 22F,
                DoDayVien = 2F,
                DoDuc = 255
            };
        }

        /// <summary>
        /// Mẫu thông báo cần chú ý trên lớp điểm.
        /// </summary>
        public static CauHinhMarker TaoMauCanhBao()
        {
            return new CauHinhMarker
            {
                HinhDang = HinhDangMarker.TamGiac,
                MauNen = Color.FromArgb(220, 80, 80),
                MauVien = Color.White,
                KichThuoc = 17F,
                DoDayVien = 1.5F,
                DoDuc = 255
            };
        }

        /// <summary>
        /// Mẫu địa điểm chung, không mã hóa loại đối tượng hay thông tin nghiệp vụ.
        /// </summary>
        public static CauHinhMarker TaoMauDiaDiem()
        {
            return new CauHinhMarker
            {
                HinhDang = HinhDangMarker.HinhThoi,
                MauNen = Color.FromArgb(59, 130, 246),
                MauVien = Color.White,
                KichThuoc = 15F,
                DoDayVien = 1.5F,
                DoDuc = 255
            };
        }

        private Shapefile LayShapefileDiem(int layerHandle)
        {
            if (!TonTaiHandle(layerHandle))
                throw new KeyNotFoundException(
                    "Không có LayerHandle này trên bản đồ: " + layerHandle);

            Shapefile sf = banDo.get_Shapefile(layerHandle);
            if (sf == null)
                throw new InvalidOperationException(
                    "LayerHandle không trỏ tới lớp Shapefile. " +
                    "Không thể định dạng marker của lớp raster.");

            if (!LaKieuDiem(sf.ShapefileType))
                throw new InvalidOperationException(
                    "Lớp hiện tại không phải lớp điểm/đa điểm. " +
                    "MarkerStyler chỉ áp dụng cho ký hiệu điểm.");

            if (sf.DefaultDrawingOptions == null)
                throw new InvalidOperationException(
                    "Không đọc được DefaultDrawingOptions của lớp điểm.");
            return sf;
        }

        private bool TonTaiHandle(int layerHandle)
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

        private static bool LaKieuDiem(ShpfileType kieu)
        {
            switch (kieu)
            {
                case ShpfileType.SHP_POINT:
                case ShpfileType.SHP_POINTZ:
                case ShpfileType.SHP_POINTM:
                case ShpfileType.SHP_MULTIPOINT:
                case ShpfileType.SHP_MULTIPOINTZ:
                case ShpfileType.SHP_MULTIPOINTM:
                    return true;
                default:
                    return false;
            }
        }

        private static tkDefaultPointSymbol ChuyenHinhDang(HinhDangMarker hinh)
        {
            switch (hinh)
            {
                case HinhDangMarker.HinhTron:
                    return tkDefaultPointSymbol.dpsCircle;
                case HinhDangMarker.HinhVuong:
                    return tkDefaultPointSymbol.dpsSquare;
                case HinhDangMarker.HinhThoi:
                    return tkDefaultPointSymbol.dpsDiamond;
                case HinhDangMarker.TamGiac:
                    return tkDefaultPointSymbol.dpsTriangleUp;
                case HinhDangMarker.NgoiSao:
                    return tkDefaultPointSymbol.dpsStar;
                case HinhDangMarker.DauCong:
                    return tkDefaultPointSymbol.dpsCross;
                case HinhDangMarker.LaCo:
                    return tkDefaultPointSymbol.dpsFlag;
                default:
                    throw new ArgumentOutOfRangeException("hinh");
            }
        }

        /// <summary>
        /// MapWinGIS dùng màu OLE (COLORREF), không phải giá trị Color.ToArgb().
        /// </summary>
        private static uint ChuyenMauOle(Color mau)
        {
            Color khongAlpha = Color.FromArgb(mau.R, mau.G, mau.B);
            return unchecked((uint)ColorTranslator.ToOle(khongAlpha));
        }

        private static void KiemTraCauHinh(CauHinhMarker cauHinh)
        {
            if (cauHinh == null)
                throw new ArgumentNullException("cauHinh");
            if (!Enum.IsDefined(typeof(HinhDangMarker), cauHinh.HinhDang))
                throw new ArgumentOutOfRangeException("cauHinh.HinhDang");
            if (cauHinh.MauNen.IsEmpty || cauHinh.MauVien.IsEmpty)
                throw new ArgumentException("Chưa xác định màu ký hiệu.", "cauHinh");
            if (float.IsNaN(cauHinh.KichThuoc) ||
                float.IsInfinity(cauHinh.KichThuoc) ||
                cauHinh.KichThuoc < 2F || cauHinh.KichThuoc > 80F)
                throw new ArgumentOutOfRangeException("cauHinh.KichThuoc",
                    "Kích thước marker phải từ 2 đến 80 điểm ảnh.");
            if (float.IsNaN(cauHinh.DoDayVien) ||
                float.IsInfinity(cauHinh.DoDayVien) ||
                cauHinh.DoDayVien < 0F || cauHinh.DoDayVien > 15F)
                throw new ArgumentOutOfRangeException("cauHinh.DoDayVien",
                    "Độ dày viền phải từ 0 đến 15 điểm ảnh.");
        }

        private static void ThemBanSao(
            List<TrangThaiTuyChon> danhSach,
            ShapeDrawingOptions tuyChon)
        {
            if (tuyChon == null)
                throw new InvalidOperationException(
                    "Không đọc được tùy chọn ký hiệu của danh mục.");
            string trangThai = tuyChon.Serialize();
            if (string.IsNullOrEmpty(trangThai))
                throw new InvalidOperationException(
                    "Không thể sao lưu tùy chọn ký hiệu trước khi cập nhật.");
            danhSach.Add(new TrangThaiTuyChon(tuyChon, trangThai));
        }

        private void KiemTraSanSang()
        {
            if (Thread.CurrentThread.ManagedThreadId != maLuongGiaoDien)
                throw new InvalidOperationException(
                    "MarkerStyler chỉ được gọi trên luồng giao diện WinForms.");
            if (banDo.IsDisposed || banDo.Disposing)
                throw new ObjectDisposedException("banDo");
            if (!banDo.IsHandleCreated)
                throw new InvalidOperationException(
                    "AxMap chưa tạo handle. Hãy gọi sau khi Form đã hiển thị.");
            if (banDo.InvokeRequired)
                throw new InvalidOperationException(
                    "Lệnh MapWinGIS phải chạy trên luồng sở hữu AxMap.");
        }

        private sealed class TrangThaiTuyChon
        {
            public ShapeDrawingOptions TuyChon { get; private set; }
            public string TrangThaiCu { get; private set; }

            public TrangThaiTuyChon(ShapeDrawingOptions tuyChon, string trangThai)
            {
                TuyChon = tuyChon;
                TrangThaiCu = trangThai;
            }
        }
    }
}
