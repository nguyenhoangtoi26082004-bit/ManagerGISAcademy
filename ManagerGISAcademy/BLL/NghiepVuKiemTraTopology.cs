
using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerGISAcademy.BLL
{
    /// <summary>
    /// Các nhóm lỗi phù hợp với lớp đơn vị dạng điểm (Point).
    /// Không áp dụng kiểm tra tự giao, chồng lấn polygon cho lớp điểm.
    /// </summary>
    public enum LoaiLoiTopology
    {
        ThieuToaDo,
        ToaDoKhongHopLe,
        NgoaiPhamViUtm48,
        ThieuHinhHoc4326,
        ThieuHinhHoc32648,
        SaiSrid4326,
        SaiSrid32648,
        SaiKieuHinhHoc,
        HinhHocKhongHopLe,
        SaiLechToaDo,
        HaiHinhHocKhongDongBo,
        ViTriGanTrung
    }

    public enum MucDoLoiTopology
    {
        CanhBao,
        Loi
    }

    /// <summary>
    /// Một bản ghi kiểm tra đã được DAL xác định các thuộc tính hình học
    /// bằng PostGIS. Null trong các trường bool?/double? nghĩa là chưa có
    /// kết quả đo, không phải đã kiểm tra và hợp lệ.
    /// </summary>
    public sealed class ThongTinTopologyDonVi
    {
        public string MaDonVi { get; set; }
        public double? KinhDo { get; set; }
        public double? ViDo { get; set; }

        public bool CoGeom4326 { get; set; }
        public bool CoGeom32648 { get; set; }
        public int? SridGeom4326 { get; set; }
        public int? SridGeom32648 { get; set; }
        public bool? Geom4326LaDiem { get; set; }
        public bool? Geom32648LaDiem { get; set; }
        public bool? Geom4326HopLe { get; set; }
        public bool? Geom32648HopLe { get; set; }

        // ST_Distance(ST_Transform(ST_SetSRID(ST_MakePoint(kinh_do,
        // vi_do),4326),32648), geom_32648), đơn vị mét.
        public double? SaiLechToaDoMet { get; set; }

        // ST_Distance(ST_Transform(geom_4326,32648), geom_32648), mét.
        public double? SaiLechHaiHinhHocMet { get; set; }
    }

    /// <summary>
    /// Hai đơn vị có các điểm gần nhau theo ST_DWithin trong EPSG:32648.
    /// DAL phải trả từng cặp một lần, ví dụ so sánh ma_don_vi A &lt; B.
    /// </summary>
    public sealed class CapViTriGanTrung
    {
        public string MaDonViA { get; set; }
        public string MaDonViB { get; set; }
        public double KhoangCachMet { get; set; }
    }

    /// <summary>
    /// Hợp đồng cho TopologyCheckerDAL.cs sẽ xây dựng ở bước kế tiếp.
    /// DAL phải chỉ đọc các dòng được quyền xem từ PostgreSQL/RLS,
    /// sử dụng câu lệnh có tham số, không lấy toàn bộ dữ liệu rồi mới
    /// coi lọc quyền tại BLL là biện pháp bảo vệ duy nhất.
    /// </summary>
    public interface ITruyXuatKiemTraTopology
    {
        IList<ThongTinTopologyDonVi> LayThongTinHinhHoc(int gioiHan);

        IList<CapViTriGanTrung> TimCapViTriGanTrung(
            double nguongKhoangCachMet, int gioiHan);
    }

    public sealed class LoiKiemTraTopology
    {
        public string MaDonVi { get; private set; }
        public string MaDonViLienQuan { get; private set; }
        public LoaiLoiTopology LoaiLoi { get; private set; }
        public MucDoLoiTopology MucDo { get; private set; }
        public string NoiDung { get; private set; }
        public double? GiaTriMet { get; private set; }

        internal LoiKiemTraTopology(
            string maDonVi,
            string maDonViLienQuan,
            LoaiLoiTopology loaiLoi,
            MucDoLoiTopology mucDo,
            string noiDung,
            double? giaTriMet)
        {
            MaDonVi = maDonVi;
            MaDonViLienQuan = maDonViLienQuan;
            LoaiLoi = loaiLoi;
            MucDo = mucDo;
            NoiDung = noiDung;
            GiaTriMet = giaTriMet;
        }
    }

    public sealed class KetQuaKiemTraTopology
    {
        public int SoDonViDaKiemTra { get; private set; }
        public int SoDonViCoViPham { get; private set; }
        public int TongSoLoi { get; private set; }
        public int TongSoCanhBao { get; private set; }
        public bool CoTheChuaDayDu { get; private set; }
        public IList<LoiKiemTraTopology> DanhSachViPham { get; private set; }

        internal KetQuaKiemTraTopology(
            int soDonViDaKiemTra,
            bool coTheChuaDayDu,
            IList<LoiKiemTraTopology> danhSach)
        {
            SoDonViDaKiemTra = soDonViDaKiemTra;
            CoTheChuaDayDu = coTheChuaDayDu;

            List<LoiKiemTraTopology> banSao =
                new List<LoiKiemTraTopology>(danhSach);
            DanhSachViPham = banSao.AsReadOnly();

            TongSoLoi = banSao.Count(
                x => x.MucDo == MucDoLoiTopology.Loi);

            TongSoCanhBao = banSao.Count(
                x => x.MucDo == MucDoLoiTopology.CanhBao);

            SoDonViCoViPham = banSao
                .SelectMany(x => string.IsNullOrWhiteSpace(x.MaDonViLienQuan)
                    ? new[] { x.MaDonVi }
                    : new[] { x.MaDonVi, x.MaDonViLienQuan })
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
        }
    }

    /// <summary>
    /// Nghiệp vụ kiểm tra tính toàn vẹn dữ liệu không gian của lớp điểm
    /// public.don_vi_quan_doi. Không truy vấn SQL, không dùng MapWinGIS
    /// và không thay đổi dữ liệu trong cơ sở dữ liệu.
    ///
    /// Phân biệt EPSG:32648 (WGS84/UTM 48N) với VN-2000.
    /// Tương thích C# 7.3 và .NET Framework 4.8.
    /// </summary>
    public sealed class NghiepVuKiemTraTopology
    {
        public const int GioiHanDonViToiDa = 5000;
        public const int GioiHanCapGanTrungToiDa = 5000;
        public const double NguongMetToiDa = 1000d;

        private ITruyXuatKiemTraTopology truyXuat;
        private IPhanQuyenDonViQuanDoi phanQuyen;

        public NghiepVuKiemTraTopology()
        {
            // Chưa cấu hình thì không được phép đọc dữ liệu CSDL.
        }

        public NghiepVuKiemTraTopology(
            ITruyXuatKiemTraTopology khoDuLieu,
            IPhanQuyenDonViQuanDoi chinhSachQuyen)
        {
            CauHinh(khoDuLieu, chinhSachQuyen);
        }

        public bool DaCauHinh
        {
            get { return truyXuat != null && phanQuyen != null; }
        }

        public void CauHinh(
            ITruyXuatKiemTraTopology khoDuLieu,
            IPhanQuyenDonViQuanDoi chinhSachQuyen)
        {
            if (khoDuLieu == null)
                throw new ArgumentNullException("khoDuLieu");

            if (chinhSachQuyen == null)
                throw new ArgumentNullException("chinhSachQuyen");

            truyXuat = khoDuLieu;
            phanQuyen = chinhSachQuyen;
        }

        /// <summary>
        /// Kiểm tra các điểm, SRID và đồng bộ hai geometry. Nếu DAL trả
        /// quá giới hạn, cờ CoTheChuaDayDu = true. Các dòng không được phép
        /// xem sẽ không xuất hiện trong kết quả; DAL cần lọc quyền ở SQL.
        /// </summary>
        public KetQuaKiemTraTopology KiemTraDuLieuKhongGian(
            int gioiHanDonVi = 2000,
            double nguongSaiLechMet = 2d,
            double nguongGanTrungMet = 2d,
            int gioiHanCapGanTrung = 2000)
        {
            KiemTraGioiHan(
                gioiHanDonVi,
                GioiHanDonViToiDa,
                "gioiHanDonVi");

            KiemTraGioiHan(
                gioiHanCapGanTrung,
                GioiHanCapGanTrungToiDa,
                "gioiHanCapGanTrung");

            KiemTraNguong(
                nguongSaiLechMet,
                "nguongSaiLechMet");

            KiemTraNguong(
                nguongGanTrungMet,
                "nguongGanTrungMet");

            DamBaoDaCauHinh();

            // Xin thêm một dòng để nhận biết kết quả đã bị cắt ngắn.
            IList<ThongTinTopologyDonVi> nguon =
                truyXuat.LayThongTinHinhHoc(gioiHanDonVi + 1);

            if (nguon == null)
                throw new InvalidOperationException(
                    "DAL trả null cho danh sách thông tin hình học.");

            if (nguon.Count > gioiHanDonVi + 1)
                throw new InvalidOperationException(
                    "DAL trả vượt quá giới hạn dòng đã yêu cầu.");

            bool chuaDayDu = nguon.Count > gioiHanDonVi;

            List<LoiKiemTraTopology> danhSach =
                new List<LoiKiemTraTopology>();

            int soDonViDuocXem = 0;

            foreach (ThongTinTopologyDonVi thongTin
                in nguon.Take(gioiHanDonVi))
            {
                if (thongTin == null ||
                    string.IsNullOrWhiteSpace(thongTin.MaDonVi))
                    continue;

                if (!DuocXem(thongTin.MaDonVi))
                    continue;

                soDonViDuocXem++;

                danhSach.AddRange(
                    KiemTraMotDonVi(
                        thongTin,
                        nguongSaiLechMet));
            }

            IList<CapViTriGanTrung> cacCap =
                truyXuat.TimCapViTriGanTrung(
                    nguongGanTrungMet,
                    gioiHanCapGanTrung + 1);

            if (cacCap == null)
                throw new InvalidOperationException(
                    "DAL trả null cho danh sách cặp vị trí gần trùng.");

            if (cacCap.Count > gioiHanCapGanTrung + 1)
                throw new InvalidOperationException(
                    "DAL trả vượt quá giới hạn cặp điểm đã yêu cầu.");

            if (cacCap.Count > gioiHanCapGanTrung)
                chuaDayDu = true;

            foreach (CapViTriGanTrung cap
                in cacCap.Take(gioiHanCapGanTrung))
            {
                if (cap == null ||
                    string.IsNullOrWhiteSpace(cap.MaDonViA) ||
                    string.IsNullOrWhiteSpace(cap.MaDonViB))
                    continue;

                if (string.Equals(
                    cap.MaDonViA,
                    cap.MaDonViB,
                    StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!DuocXem(cap.MaDonViA) ||
                    !DuocXem(cap.MaDonViB))
                    continue;

                if (!LaSoHuuHan(cap.KhoangCachMet) ||
                    cap.KhoangCachMet < 0d)
                    throw new InvalidOperationException(
                        "DAL trả về khoảng cách cặp điểm không hợp lệ.");

                if (cap.KhoangCachMet <= nguongGanTrungMet)
                {
                    danhSach.Add(TaoViPham(
                        cap.MaDonViA,
                        cap.MaDonViB,
                        LoaiLoiTopology.ViTriGanTrung,
                        MucDoLoiTopology.CanhBao,
                        "Hai vị trí cách nhau không quá ngưỡng kiểm tra; " +
                        "cần đối chiếu trước khi xác định là dữ liệu trùng.",
                        cap.KhoangCachMet));
                }
            }

            return new KetQuaKiemTraTopology(
                soDonViDuocXem,
                chuaDayDu,
                danhSach);
        }

        /// <summary>
        /// Kiểm tra một bản ghi do bên gọi cung cấp mà không truy cập CSDL.
        /// Không dùng để xác nhận người gọi có quyền đọc hồ sơ trong CSDL.
        /// </summary>
        public static List<LoiKiemTraTopology> KiemTraMotDonVi(
            ThongTinTopologyDonVi thongTin,
            double nguongSaiLechMet = 2d)
        {
            if (thongTin == null)
                throw new ArgumentNullException("thongTin");

            KiemTraNguong(
                nguongSaiLechMet,
                "nguongSaiLechMet");

            List<LoiKiemTraTopology> loi =
                new List<LoiKiemTraTopology>();

            string ma = thongTin.MaDonVi;

            bool coKinhDo = thongTin.KinhDo.HasValue;
            bool coViDo = thongTin.ViDo.HasValue;

            if (!coKinhDo ||
                !coViDo ||
                (coKinhDo && coViDo &&
                 thongTin.KinhDo.Value == 0d &&
                 thongTin.ViDo.Value == 0d))
            {
                loi.Add(TaoViPham(
                    ma,
                    null,
                    LoaiLoiTopology.ThieuToaDo,
                    MucDoLoiTopology.Loi,
                    "Thiếu kinh độ/vĩ độ WGS84 hoặc cặp (0,0) chưa được xác định.",
                    null));
            }
            else
            {
                double kinhDo = thongTin.KinhDo.Value;
                double viDo = thongTin.ViDo.Value;

                if (!LaSoHuuHan(kinhDo) ||
                    !LaSoHuuHan(viDo) ||
                    kinhDo < -180d ||
                    kinhDo > 180d ||
                    viDo < -90d ||
                    viDo > 90d)
                {
                    loi.Add(TaoViPham(
                        ma,
                        null,
                        LoaiLoiTopology.ToaDoKhongHopLe,
                        MucDoLoiTopology.Loi,
                        "Kinh độ hoặc vĩ độ ngoài miền WGS84 hoặc không phải số hữu hạn.",
                        null));
                }
                else if (kinhDo < 102d ||
                         kinhDo > 108d ||
                         viDo < 0d ||
                         viDo > 84d)
                {
                    loi.Add(TaoViPham(
                        ma,
                        null,
                        LoaiLoiTopology.NgoaiPhamViUtm48,
                        MucDoLoiTopology.CanhBao,
                        "Vị trí nằm ngoài phạm vi kinh/vĩ độ đang quy định " +
                        "cho dữ liệu UTM múi 48 Bắc.",
                        null));
                }
            }

            if (!thongTin.CoGeom4326)
            {
                loi.Add(TaoViPham(
                    ma,
                    null,
                    LoaiLoiTopology.ThieuHinhHoc4326,
                    MucDoLoiTopology.Loi,
                    "Chưa có geom_4326.",
                    null));
            }
            else
            {
                KiemTraHinhHoc(
                    ma,
                    "geom_4326",
                    4326,
                    thongTin.SridGeom4326,
                    thongTin.Geom4326LaDiem,
                    thongTin.Geom4326HopLe,
                    loi);
            }

            if (!thongTin.CoGeom32648)
            {
                loi.Add(TaoViPham(
                    ma,
                    null,
                    LoaiLoiTopology.ThieuHinhHoc32648,
                    MucDoLoiTopology.Loi,
                    "Chưa có geom_32648.",
                    null));
            }
            else
            {
                KiemTraHinhHoc(
                    ma,
                    "geom_32648",
                    32648,
                    thongTin.SridGeom32648,
                    thongTin.Geom32648LaDiem,
                    thongTin.Geom32648HopLe,
                    loi);
            }

            if (thongTin.SaiLechToaDoMet.HasValue)
            {
                KiemTraSaiLech(
                    ma,
                    thongTin.SaiLechToaDoMet.Value,
                    nguongSaiLechMet,
                    LoaiLoiTopology.SaiLechToaDo,
                    "Tọa độ WGS84 trong thuộc tính không khớp hình học lưu trữ.",
                    loi);
            }

            if (thongTin.SaiLechHaiHinhHocMet.HasValue)
            {
                KiemTraSaiLech(
                    ma,
                    thongTin.SaiLechHaiHinhHocMet.Value,
                    nguongSaiLechMet,
                    LoaiLoiTopology.HaiHinhHocKhongDongBo,
                    "geom_4326 và geom_32648 không đồng bộ sau phép chuyển hệ.",
                    loi);
            }

            return loi;
        }

        private static void KiemTraHinhHoc(
            string ma,
            string tenCot,
            int sridDung,
            int? sridThucTe,
            bool? laDiem,
            bool? hopLe,
            List<LoiKiemTraTopology> danhSach)
        {
            if (!sridThucTe.HasValue ||
                sridThucTe.Value != sridDung)
            {
                danhSach.Add(TaoViPham(
                    ma,
                    null,
                    sridDung == 4326
                        ? LoaiLoiTopology.SaiSrid4326
                        : LoaiLoiTopology.SaiSrid32648,
                    MucDoLoiTopology.Loi,
                    tenCot +
                    " có SRID không đúng hoặc chưa được DAL cung cấp; " +
                    "yêu cầu EPSG:" + sridDung + ".",
                    null));
            }

            if (laDiem.HasValue && !laDiem.Value)
            {
                danhSach.Add(TaoViPham(
                    ma,
                    null,
                    LoaiLoiTopology.SaiKieuHinhHoc,
                    MucDoLoiTopology.Loi,
                    tenCot + " không phải kiểu POINT.",
                    null));
            }

            if (hopLe.HasValue && !hopLe.Value)
            {
                danhSach.Add(TaoViPham(
                    ma,
                    null,
                    LoaiLoiTopology.HinhHocKhongHopLe,
                    MucDoLoiTopology.Loi,
                    tenCot + " không hợp lệ theo ST_IsValid.",
                    null));
            }
        }

        private static void KiemTraSaiLech(
            string ma,
            double doLechMet,
            double nguongMet,
            LoaiLoiTopology loai,
            string noiDung,
            List<LoiKiemTraTopology> danhSach)
        {
            if (!LaSoHuuHan(doLechMet) ||
                doLechMet < 0d)
            {
                throw new InvalidOperationException(
                    "DAL cung cấp giá trị sai lệch hình học không hợp lệ.");
            }

            if (doLechMet > nguongMet)
            {
                danhSach.Add(TaoViPham(
                    ma,
                    null,
                    loai,
                    MucDoLoiTopology.Loi,
                    noiDung,
                    doLechMet));
            }
        }

        private static LoiKiemTraTopology TaoViPham(
            string ma,
            string maLienQuan,
            LoaiLoiTopology loai,
            MucDoLoiTopology mucDo,
            string noiDung,
            double? giaTriMet)
        {
            return new LoiKiemTraTopology(
                ma,
                maLienQuan,
                loai,
                mucDo,
                noiDung,
                giaTriMet);
        }

        private bool DuocXem(string maDonVi)
        {
            return phanQuyen.DuocPhep(
                ThaoTacDonViQuanDoi.Xem,
                maDonVi);
        }

        private void DamBaoDaCauHinh()
        {
            if (!DaCauHinh)
            {
                throw new InvalidOperationException(
                    "TopologyCheckerBLL chưa được cấu hình DAL và phân quyền.");
            }
        }

        private static void KiemTraGioiHan(
            int giaTri,
            int toiDa,
            string ten)
        {
            if (giaTri < 1 || giaTri > toiDa)
            {
                throw new ArgumentOutOfRangeException(
                    ten,
                    "Giới hạn phải từ 1 đến " + toiDa + ".");
            }
        }

        private static void KiemTraNguong(
            double giaTri,
            string ten)
        {
            if (!LaSoHuuHan(giaTri) ||
                giaTri <= 0d ||
                giaTri > NguongMetToiDa)
            {
                throw new ArgumentOutOfRangeException(
                    ten,
                    "Ngưỡng phải lớn hơn 0 và không vượt quá 1.000 mét.");
            }
        }

        private static bool LaSoHuuHan(double giaTri)
        {
            return !double.IsNaN(giaTri) &&
                   !double.IsInfinity(giaTri);
        }
    }
}
