
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerGISAcademy.BLL
{
    /// <summary>
    /// Một đơn vị cùng khoảng cách đến điểm hoặc đơn vị đang phân tích.
    /// Khoảng cách tính bằng mét trong EPSG:32648; Wkt4326 dùng để hiển thị.
    /// DTO kết quả này không thay thế ManagerGISAcademy.DTO.DonViQuanDoi.
    /// </summary>
    public sealed class DonViTheoKhoangCach
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string QuanBinhChung { get; set; }
        public double KhoangCachMet { get; set; }
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public string Wkt4326 { get; set; }
    }

    /// <summary>
    /// Kết quả vùng đệm: hình học WKT trong EPSG:4326 và danh sách đơn vị
    /// trong bán kính. Không tạo polygon hoặc thực hiện phép Buffer trên GUI.
    /// </summary>
    public sealed class KetQuaVungDemDonVi
    {
        public string MaDonViTam { get; private set; }
        public double BanKinhMet { get; private set; }
        public string WktVungDem4326 { get; private set; }
        public IList<DonViTheoKhoangCach> DanhSachDonVi { get; private set; }

        internal KetQuaVungDemDonVi(string maDonViTam, double banKinhMet,
            string wktVungDem4326, IList<DonViTheoKhoangCach> danhSachDonVi)
        {
            MaDonViTam = maDonViTam;
            BanKinhMet = banKinhMet;
            WktVungDem4326 = wktVungDem4326;
            DanhSachDonVi = new List<DonViTheoKhoangCach>(danhSachDonVi).AsReadOnly();
        }
    }

    /// <summary>
    /// Hợp đồng truy vấn không gian. SpatialAnalysisDAL.cs triển khai giao diện này.
    /// Phép chiếu EPSG:32648 = WGS84 / UTM múi 48 Bắc, KHÔNG phải VN-2000.
    /// </summary>
    public interface ITruyXuatPhanTichKhongGian
    {
        IList<DonViTheoKhoangCach> TimDonViTrongBanKinh(
            string maDonViTam, double banKinhMet, int gioiHan);

        IList<DonViTheoKhoangCach> TimKDonViGanNhat(
            double kinhDo, double viDo, int soLuong);

        double? TinhKhoangCachHaiDonVi(string maDonViA, string maDonViB);

        string LayWktVungDem(string maDonViTam, double banKinhMet);
    }

    /// <summary>
    /// Tầng nghiệp vụ phân tích không gian: kiểm tra tham số, kiểm tra quyền
    /// và gọi DAL. Không phụ thuộc Npgsql, PostGIS API, WinForms hay MapWinGIS.
    ///
    /// Bảo mật: kiểm tra quyền Xem ở BLL là lớp bảo vệ bổ sung; bản thân DAL
    /// phải dùng tài khoản PostgreSQL/RLS chỉ nhìn thấy bản ghi được cho phép.
    /// Tuyệt đối không dùng tài khoản postgres/superuser cho người dùng cuối.
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class SpatialAnalysisBLL
    {
        public const double BanKinhToiDaMet = 35000.0;
        public const int SoLuongGanNhatToiDa = 100;
        public const int GioiHanTrongBanKinhToiDa = 500;

        private ITruyXuatPhanTichKhongGian truyXuat;
        private IPhanQuyenDonViQuanDoi phanQuyen;

        public SpatialAnalysisBLL()
        {
            // Chưa cấu hình thì không thể đọc dữ liệu; không cấp quyền mặc định.
        }

        public SpatialAnalysisBLL(ITruyXuatPhanTichKhongGian dal,
            IPhanQuyenDonViQuanDoi quyen)
        {
            CauHinh(dal, quyen);
        }

        public bool DaCauHinh
        {
            get { return truyXuat != null && phanQuyen != null; }
        }

        public void CauHinh(ITruyXuatPhanTichKhongGian dal,
            IPhanQuyenDonViQuanDoi quyen)
        {
            if (dal == null)
                throw new ArgumentNullException("dal");
            if (quyen == null)
                throw new ArgumentNullException("quyen");
            truyXuat = dal;
            phanQuyen = quyen;
        }

        /// <summary>
        /// Trả polygon vùng đệm theo WKT EPSG:4326 và các đơn vị lân cận
        /// được quyền xem, KHÔNG bao gồm đơn vị ở tâm.
        /// </summary>
        public KetQuaVungDemDonVi PhanTichVungDem(
            string maDonViTam, double banKinhMet = 5000.0, int gioiHan = 200)
        {
            string ma = KiemTraMa(maDonViTam);
            KiemTraBanKinh(banKinhMet);
            KiemTraGioiHan(gioiHan);
            BaoDamDuocXem(ma);

            string wkt = truyXuat.LayWktVungDem(ma, banKinhMet);
            if (string.IsNullOrWhiteSpace(wkt))
                throw new KeyNotFoundException(
                    "Không tìm thấy đơn vị trung tâm có hình học EPSG:32648 hợp lệ.");

            IList<DonViTheoKhoangCach> ketQua =
                truyXuat.TimDonViTrongBanKinh(ma, banKinhMet, gioiHan);
            List<DonViTheoKhoangCach> duocXem = LocTheoQuyen(ketQua);

            return new KetQuaVungDemDonVi(ma, banKinhMet, wkt, duocXem);
        }

        /// <summary>
        /// Tìm các đơn vị nằm trong bán kính tính từ một đơn vị đã có trong CSDL.
        /// Khoảng cách và bán kính theo mét, không phải độ vĩ/kinh.
        /// </summary>
        public List<DonViTheoKhoangCach> TimDonViTrongBanKinh(
            string maDonViTam, double banKinhMet, int gioiHan = 200)
        {
            string ma = KiemTraMa(maDonViTam);
            KiemTraBanKinh(banKinhMet);
            KiemTraGioiHan(gioiHan);
            BaoDamDuocXem(ma);
            return LocTheoQuyen(truyXuat.TimDonViTrongBanKinh(ma, banKinhMet, gioiHan));
        }

        /// <summary>
        /// Tìm K đơn vị gần một điểm WGS84 (kinh độ, vĩ độ).
        /// Phạm vi tọa độ bị giới hạn ở múi UTM 48 Bắc.
        /// </summary>
        public List<DonViTheoKhoangCach> TimKDonViGanNhat(
            double kinhDo, double viDo, int soLuong = 10)
        {
            KiemTraToaDoWgs84(kinhDo, viDo);
            if (soLuong < 1 || soLuong > SoLuongGanNhatToiDa)
                throw new ArgumentOutOfRangeException("soLuong",
                    "Số lượng phải từ 1 đến " + SoLuongGanNhatToiDa + ".");
            BaoDamCauHinh();
            // Sau lọc quyền, số kết quả có thể ít hơn soLuong.
            return LocTheoQuyen(truyXuat.TimKDonViGanNhat(kinhDo, viDo, soLuong));
        }

        /// <summary>
        /// Khoảng cách hai đơn vị tính theo EPSG:32648 (mét).
        /// Trả null nếu không tìm thấy một trong hai hình học.
        /// </summary>
        public double? TinhKhoangCachHaiDonVi(string maDonViA, string maDonViB)
        {
            string a = KiemTraMa(maDonViA);
            string b = KiemTraMa(maDonViB);
            BaoDamDuocXem(a);
            BaoDamDuocXem(b);
            double? khoangCach = truyXuat.TinhKhoangCachHaiDonVi(a, b);
            if (khoangCach.HasValue &&
                (!LaSoHuuHan(khoangCach.Value) || khoangCach.Value < 0))
                throw new InvalidOperationException("DAL trả về khoảng cách không hợp lệ.");
            return khoangCach;
        }

        private List<DonViTheoKhoangCach> LocTheoQuyen(
            IList<DonViTheoKhoangCach> danhSach)
        {
            if (danhSach == null)
                throw new InvalidOperationException("DAL trả về danh sách null.");

            List<DonViTheoKhoangCach> ketQua = new List<DonViTheoKhoangCach>();
            foreach (DonViTheoKhoangCach donVi in danhSach)
            {
                if (donVi == null || string.IsNullOrWhiteSpace(donVi.MaDonVi))
                    continue;
                if (!phanQuyen.DuocPhep(ThaoTacDonViQuanDoi.Xem, donVi.MaDonVi))
                    continue;
                if (!LaSoHuuHan(donVi.KhoangCachMet) || donVi.KhoangCachMet < 0)
                    throw new InvalidOperationException("DAL trả về khoảng cách không hợp lệ.");
                ketQua.Add(donVi);
            }
            return ketQua.OrderBy(x => x.KhoangCachMet)
                .ThenBy(x => x.MaDonVi, StringComparer.OrdinalIgnoreCase).ToList();
        }

        private void BaoDamCauHinh()
        {
            if (!DaCauHinh)
                throw new InvalidOperationException(
                    "SpatialAnalysisBLL chưa được cấu hình DAL và phân quyền.");
        }

        private void BaoDamDuocXem(string maDonVi)
        {
            BaoDamCauHinh();
            if (!phanQuyen.DuocPhep(ThaoTacDonViQuanDoi.Xem, maDonVi))
                throw new UnauthorizedAccessException(
                    "Tài khoản không có quyền xem hồ sơ cần phân tích.");
        }

        private static string KiemTraMa(string maDonVi)
        {
            if (string.IsNullOrWhiteSpace(maDonVi))
                throw new ArgumentException("Mã đơn vị không được để trống.", "maDonVi");
            string ma = maDonVi.Trim();
            if (ma.Length > 50)
                throw new ArgumentOutOfRangeException("maDonVi", "Mã đơn vị tối đa 50 ký tự.");
            return ma;
        }

        private static void KiemTraBanKinh(double banKinhMet)
        {
            if (!LaSoHuuHan(banKinhMet) ||
                banKinhMet <= 0 || banKinhMet > BanKinhToiDaMet)
                throw new ArgumentOutOfRangeException("banKinhMet",
                    "Bán kính phải lớn hơn 0 và không vượt quá 35.000 mét.");
        }

        private static void KiemTraGioiHan(int gioiHan)
        {
            if (gioiHan < 1 || gioiHan > GioiHanTrongBanKinhToiDa)
                throw new ArgumentOutOfRangeException("gioiHan",
                    "Giới hạn phải từ 1 đến 500 kết quả.");
        }

        private static void KiemTraToaDoWgs84(double kinhDo, double viDo)
        {
            if (!LaSoHuuHan(kinhDo) || !LaSoHuuHan(viDo) ||
                kinhDo < 102.0 || kinhDo > 108.0 ||
                viDo < 0.0 || viDo > 84.0)
                throw new ArgumentOutOfRangeException("kinhDo",
                    "Tọa độ WGS84 phải ở múi UTM 48 Bắc (102-108 độ Đông, 0-84 độ Bắc).");
        }

        private static bool LaSoHuuHan(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
