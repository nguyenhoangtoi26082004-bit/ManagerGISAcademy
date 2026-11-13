
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.BLL
{
    public enum ThaoTacDonViQuanDoi
    {
        Xem, Them, Sua, Xoa
    }

    public interface ITruyXuatDonViQuanDoi
    {
        IList<DonViQuanDoi> LayTatCaDonVi();
        DonViQuanDoi LayTheoMa(string maDonVi);
        bool ThemDonVi(DonViQuanDoi donVi);
        bool SuaDonVi(DonViQuanDoi donVi);
        bool XoaDonVi(string maDonVi);
    }

    public interface IPhanQuyenDonViQuanDoi
    {
        bool DuocPhep(
            ThaoTacDonViQuanDoi thaoTac,
            string maDonVi);
    }

    /// <summary>
    /// Hồ sơ đã bị người khác sửa sau khi mở (khóa lạc quan).
    /// GUI bắt riêng lỗi này để đề nghị tải lại bản mới nhất.
    /// </summary>
    public sealed class XungDotPhienBanException :
        InvalidOperationException
    {
        public string MaDonVi { get; private set; }

        public XungDotPhienBanException(string maDonVi)
            : base(
                "Hồ sơ " + maDonVi + " đã được người khác cập nhật " +
                "sau khi bạn mở. Để tránh ghi đè dữ liệu, " +
                "hãy tải lại hồ sơ mới nhất rồi nhập lại thay đổi.")
        {
            MaDonVi = maDonVi;
        }
    }

    public sealed class KetQuaKiemTraDonVi
    {
        private readonly List<string> loi =
            new List<string>();

        public bool HopLe
        {
            get { return loi.Count == 0; }
        }

        public IList<string> DanhSachLoi
        {
            get { return loi.AsReadOnly(); }
        }

        internal void ThemLoi(string thongBao)
        {
            if (!string.IsNullOrWhiteSpace(thongBao) &&
                !loi.Contains(thongBao))
            {
                loi.Add(thongBao);
            }
        }

        public string LayThongBao()
        {
            return HopLe
                ? "Dữ liệu hợp lệ."
                : string.Join(Environment.NewLine, loi);
        }
    }

    public sealed class TongQuanDonViQuanDoi
    {
        public int TongSoDonVi { get; internal set; }
        public int SoCoToaDoWgs84 { get; internal set; }
        public int SoThieuToaDoWgs84 { get; internal set; }
    }

    /// <summary>
    /// Nghiệp vụ quản lý đơn vị quân đội.
    /// Kiểm tra thuộc tính và nhận cảnh báo không gian từ DAL.
    /// </summary>
    public sealed class NghiepVuDonVi
    {
        private ITruyXuatDonViQuanDoi truyXuat;
        private IPhanQuyenDonViQuanDoi phanQuyen;

        public NghiepVuDonVi()
        {
        }

        public NghiepVuDonVi(
            ITruyXuatDonViQuanDoi dal,
            IPhanQuyenDonViQuanDoi quyen)
        {
            CauHinh(dal, quyen);
        }

        public bool DaCauHinh
        {
            get
            {
                return truyXuat != null &&
                       phanQuyen != null;
            }
        }

        public void CauHinh(
            ITruyXuatDonViQuanDoi dal,
            IPhanQuyenDonViQuanDoi quyen)
        {
            if (dal == null)
                throw new ArgumentNullException("dal");

            if (quyen == null)
                throw new ArgumentNullException("quyen");

            truyXuat = dal;
            phanQuyen = quyen;
        }

        // 1. ĐỌC DANH SÁCH VÀ PHÁT HIỆN TỌA ĐỘ TRÙNG

        public List<DonViQuanDoi> LayTatCaDonVi()
        {
            BaoDamCauHinh();

            IList<DonViQuanDoi> duLieu =
                truyXuat.LayTatCaDonVi();

            if (duLieu == null)
            {
                throw new InvalidOperationException(
                    "DAL không được trả danh sách null.");
            }

            List<DonViQuanDoi> ketQua = duLieu
                .Where(dv =>
                    dv != null &&
                    !string.IsNullOrWhiteSpace(dv.MaDonVi) &&
                    phanQuyen.DuocPhep(
                        ThaoTacDonViQuanDoi.Xem,
                        dv.MaDonVi))
                .ToList();

            // Chỉ kiểm tra trùng giữa những hồ sơ
            // mà tài khoản được phép xem.
            foreach (var nhom in ketQua
                .Where(CoToaDoWgs84)
                .GroupBy(dv =>
                    Math.Round(
                        dv.KinhDo, 7,
                        MidpointRounding.AwayFromZero)
                        .ToString(
                            "F7",
                            CultureInfo.InvariantCulture)
                    + ";" +
                    Math.Round(
                        dv.ViDo, 7,
                        MidpointRounding.AwayFromZero)
                        .ToString(
                            "F7",
                            CultureInfo.InvariantCulture)))
            {
                List<DonViQuanDoi> trung =
                    nhom.ToList();

                if (trung.Count < 2)
                    continue;

                foreach (DonViQuanDoi dv in trung)
                {
                    if (dv.CanhBaoKhongGian == null)
                    {
                        dv.CanhBaoKhongGian =
                            new List<string>();
                    }

                    string[] maKhac = trung
                        .Where(x =>
                            !object.ReferenceEquals(x, dv))
                        .Select(x => x.MaDonVi)
                        .Distinct(
                            StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    string thongBao =
                        "Trùng tọa độ WGS84 với hồ sơ: " +
                        string.Join(", ", maKhac) +
                        ". Cần xác minh vì nhiều đơn vị " +
                        "có thể cùng địa điểm.";

                    if (!dv.CanhBaoKhongGian.Contains(thongBao))
                        dv.CanhBaoKhongGian.Add(thongBao);
                }
            }

            return ketQua;
        }

        // 2. TRA CỨU ĐƠN VỊ

        public List<DonViQuanDoi> TraCuuDonVi(
            string tuKhoa,
            string loaiDonVi = null,
            string quanBinhChung = null,
            string quanHuyen = null,
            int gioiHan = 200)
        {
            if (gioiHan < 1 || gioiHan > 1000)
            {
                throw new ArgumentOutOfRangeException(
                    "gioiHan",
                    "Giới hạn từ 1 đến 1000.");
            }

            string q = ChuanHoaTimKiem(tuKhoa);
            string loai = ChuanHoaTimKiem(loaiDonVi);
            string binhChung =
                ChuanHoaTimKiem(quanBinhChung);
            string diaBan =
                ChuanHoaTimKiem(quanHuyen);

            return LayTatCaDonVi()
                .Where(dv =>
                    (
                        q.Length == 0 ||
                        ChuanHoaTimKiem(
                            (dv.MaDonVi ?? "") + " " +
                            (dv.TenDonVi ?? "") + " " +
                            (dv.TenVietTat ?? "") + " " +
                            (dv.DonViCapTren ?? "") + " " +
                            (dv.QuanHuyen ?? "")
                        ).Contains(q)
                    )
                    &&
                    (
                        loai.Length == 0 ||
                        ChuanHoaTimKiem(dv.LoaiDonVi) == loai
                    )
                    &&
                    (
                        binhChung.Length == 0 ||
                        ChuanHoaTimKiem(dv.QuanBinhChung)
                            == binhChung
                    )
                    &&
                    (
                        diaBan.Length == 0 ||
                        ChuanHoaTimKiem(dv.QuanHuyen)
                            == diaBan
                    ))
                .OrderBy(
                    dv => dv.TenDonVi ?? "",
                    StringComparer.CurrentCultureIgnoreCase)
                .Take(gioiHan)
                .ToList();
        }

        // 3. LẤY HỒ SƠ THEO MÃ

        public DonViQuanDoi LayDonViTheoMa(
            string maDonVi)
        {
            string ma = KiemTraMa(maDonVi);

            BaoDamQuyen(
                ThaoTacDonViQuanDoi.Xem, ma);

            DonViQuanDoi dv =
                truyXuat.LayTheoMa(ma);

            if (dv != null &&
                !string.Equals(
                    dv.MaDonVi, ma,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "DAL trả về sai mã đơn vị.");
            }

            return dv;
        }

        // 4. KIỂM TRA THUỘC TÍNH VÀ KHÔNG GIAN

        public KetQuaKiemTraDonVi KiemTraDuLieu(
            DonViQuanDoi dv,
            bool batBuocCoToaDo = false)
        {
            return KiemTraDuLieuNoiBo(
                dv,
                batBuocCoToaDo,
                true);
        }

        /// <summary>
        /// Nhan dien ho so Ban Chi huy quan su cap xa (danh muc BCHQS,
        /// ma "HN-BCHQS-..." hoac loai "Ban Chỉ huy quân sự cấp xã").
        /// </summary>
        public static bool LaBanChqsCapXa(DonViQuanDoi dv)
        {
            if (dv == null) return false;
            if (dv.LaDanhMucBanChqs) return true;
            string ma = (dv.MaDonVi ?? "").Trim();
            if (ma.StartsWith("HN-BCHQS-", StringComparison.OrdinalIgnoreCase)) return true;
            string loai = dv.LoaiDonVi ?? "";
            return loai.IndexOf("Ban Chỉ huy quân sự", StringComparison.CurrentCultureIgnoreCase) >= 0 &&
                   loai.IndexOf("xã", StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Don vi (khong phai Ban CHQS xa) chua co so truc ban -> canh bao
        /// chat luong du lieu (khong chan luu ho so).
        /// </summary>
        public static bool LaThieuSoTrucBan(DonViQuanDoi dv)
        {
            return dv != null && !LaBanChqsCapXa(dv) &&
                   string.IsNullOrWhiteSpace(dv.SoDienThoai);
        }

        private KetQuaKiemTraDonVi KiemTraDuLieuNoiBo(
            DonViQuanDoi dv,
            bool batBuocCoToaDo,
            bool baoGomCanhBaoKhongGian)
        {
            KetQuaKiemTraDonVi kq =
                new KetQuaKiemTraDonVi();

            if (dv == null)
            {
                kq.ThemLoi("Chưa nhập đơn vị.");
                return kq;
            }

            // Ban CHQS cap xa: co quan quan su dia phuong, KHONG thuoc
            // quan/binh chung nao; so truc ban thuong dung chung UBND xa.
            // -> hai truong nay khong bat buoc (van kiem tra do dai/dinh dang).
            bool laBanChqsXa = LaBanChqsCapXa(dv);

            KiemTraChuoi(
                dv.MaDonVi, 50,
                "Mã đơn vị", true, kq);

            KiemTraChuoi(
                dv.TenDonVi, 255,
                "Tên đơn vị", true, kq);

            KiemTraChuoi(
                dv.TenVietTat, 100,
                "Tên viết tắt", false, kq);

            KiemTraChuoi(
                dv.LoaiDonVi, 100,
                "Loại đơn vị", true, kq);

            KiemTraChuoi(
                dv.QuanBinhChung, 100,
                "Quân/Binh chủng", !laBanChqsXa, kq);

            KiemTraChuoi(
                dv.DonViCapTren, 255,
                "Đơn vị cấp trên", true, kq);

            KiemTraChuoi(
                dv.QuanHuyen, 100,
                "Địa bàn", true, kq);

            KiemTraChuoi(
                dv.DiaChi, 10000,
                "Địa chỉ", true, kq);

            KiemTraChuoi(
                dv.ChiHuyTruong, 150,
                "Chỉ huy trưởng", false, kq);

            KiemTraChuoi(
                dv.ChinhUy, 150,
                "Chính ủy", false, kq);

            KiemTraChuoi(
                dv.SoDienThoai, 50,
                // So truc ban don vi quan doi thuong khong cong khai -> KHONG
                // bat buoc khi luu; man Kiem tra du lieu nhac o muc "Canh bao"
                // (xem LaThieuSoTrucBan). Neu co nhap van kiem tra dinh dang.
                "Số điện thoại", false, kq);

            KiemTraChuoi(
                dv.TrangThaiSSCD, 100,
                "Trạng thái SSCĐ", false, kq);

            KiemTraChuoi(
                dv.DoMat, 50,
                "Độ mật", false, kq);

            // Luật nghiệp vụ: số trực ban chỉ gồm chữ số và
            // ký tự phân cách thông dụng, đủ 6-15 chữ số.
            if (!string.IsNullOrWhiteSpace(dv.SoDienThoai) &&
                !LaSoDienThoaiHopLe(dv.SoDienThoai))
            {
                kq.ThemLoi(
                    "Số điện thoại chỉ gồm chữ số, khoảng trắng, " +
                    "dấu + - . ( ) và có từ 6 đến 15 chữ số.");
            }

            // Luật nghiệp vụ: đơn vị không thể trực thuộc
            // chính nó (gây vòng lặp cây tổ chức).
            if (!string.IsNullOrWhiteSpace(dv.DonViCapTren) &&
                LaCungDonVi(dv.DonViCapTren, dv))
            {
                kq.ThemLoi(
                    "Đơn vị cấp trên không được là chính đơn vị này.");
            }

            if (!string.IsNullOrWhiteSpace(dv.GhiChu))
            {
                kq.ThemLoi(
                    "CSDL chưa có cột ghi_chu; " +
                    "không thể lưu GhiChu.");
            }

            if (!LaSoHuuHan(dv.KinhDo) ||
                !LaSoHuuHan(dv.ViDo))
            {
                kq.ThemLoi(
                    "Kinh độ và vĩ độ phải là số hữu hạn.");
            }
            else
            {
                bool thieu =
                    dv.KinhDo == 0d &&
                    dv.ViDo == 0d;

                if (thieu && batBuocCoToaDo)
                {
                    kq.ThemLoi(
                        "CSDL yêu cầu tọa độ WGS84 " +
                        "của đơn vị.");
                }

                if (!thieu &&
                    (
                        dv.KinhDo < 102d ||
                        dv.KinhDo > 108d ||
                        dv.ViDo < 0d ||
                        dv.ViDo > 84d
                    ))
                {
                    kq.ThemLoi(
                        "Tọa độ ngoài phạm vi " +
                        "WGS84/UTM múi 48 Bắc.");
                }
            }

            if (!LaSoHuuHan(dv.XVn2000) ||
                !LaSoHuuHan(dv.YVn2000))
            {
                kq.ThemLoi(
                    "Tọa độ phẳng phải là số hữu hạn.");
            }
            else if (
                (dv.XVn2000 == 0d) !=
                (dv.YVn2000 == 0d))
            {
                kq.ThemLoi(
                    "X/Y phẳng phải có đủ cả hai thành phần.");
            }

            // Cảnh báo được DAL đọc từ PostGIS:
            // hình học, SRID, độ lệch tọa độ,
            // vị trí ngoài ranh giới Hà Nội.
            if (baoGomCanhBaoKhongGian &&
                dv.CanhBaoKhongGian != null)
            {
                foreach (string loi in
                    dv.CanhBaoKhongGian)
                {
                    kq.ThemLoi(loi);
                }
            }

            return kq;
        }

        // 5. THÊM HỒ SƠ

        public bool ThemDonVi(DonViQuanDoi dv)
        {
            BaoDamCauHinh();
            KiemTraDeGhi(dv);

            BaoDamQuyen(
                ThaoTacDonViQuanDoi.Them,
                dv.MaDonVi.Trim());

            ChuanHoaChuoi(dv);

            if (truyXuat.LayTheoMa(dv.MaDonVi) != null)
            {
                throw new InvalidOperationException(
                    "Mã đơn vị đã tồn tại.");
            }

            return truyXuat.ThemDonVi(dv);
        }

        // 6. SỬA HỒ SƠ

        public bool SuaDonVi(DonViQuanDoi dv)
        {
            BaoDamCauHinh();
            KiemTraDeGhi(dv);

            BaoDamQuyen(
                ThaoTacDonViQuanDoi.Sua,
                dv.MaDonVi.Trim());

            ChuanHoaChuoi(dv);

            DonViQuanDoi hienTai =
                truyXuat.LayTheoMa(dv.MaDonVi);

            if (hienTai == null)
            {
                throw new KeyNotFoundException(
                    "Không tìm thấy đơn vị cần sửa.");
            }

            // Khóa lạc quan: phiên bản lúc mở phải trùng
            // phiên bản hiện tại trong CSDL.
            if (!string.IsNullOrWhiteSpace(dv.PhienBan) &&
                !string.IsNullOrWhiteSpace(hienTai.PhienBan) &&
                !string.Equals(
                    dv.PhienBan.Trim(),
                    hienTai.PhienBan.Trim(),
                    StringComparison.Ordinal))
            {
                throw new XungDotPhienBanException(
                    dv.MaDonVi);
            }

            bool thanhCong = truyXuat.SuaDonVi(dv);

            // DAL kiểm tra lại phiên bản trong chính câu UPDATE.
            // 0 dòng nghĩa là có người sửa xen giữa hai bước.
            if (!thanhCong &&
                !string.IsNullOrWhiteSpace(dv.PhienBan))
            {
                throw new XungDotPhienBanException(
                    dv.MaDonVi);
            }

            return thanhCong;
        }

        // 7. XÓA HỒ SƠ

        public bool XoaDonVi(string maDonVi)
        {
            string ma = KiemTraMa(maDonVi);

            BaoDamQuyen(
                ThaoTacDonViQuanDoi.Xoa, ma);

            DonViQuanDoi canXoa =
                truyXuat.LayTheoMa(ma);

            if (canXoa == null)
            {
                throw new KeyNotFoundException(
                    "Không tìm thấy đơn vị cần xóa.");
            }

            // Toàn vẹn cây tổ chức: không xóa đơn vị
            // đang là cấp trên của đơn vị khác.
            List<string> capDuoi =
                TimMaDonViCapDuoi(canXoa);

            if (capDuoi.Count > 0)
            {
                throw new InvalidOperationException(
                    "Không thể xóa \"" + canXoa.TenDonVi +
                    "\" vì còn " + capDuoi.Count +
                    " đơn vị trực thuộc (" +
                    string.Join(", ", capDuoi.Take(5)) +
                    (capDuoi.Count > 5 ? ", ..." : "") +
                    "). Hãy chuyển các đơn vị này sang " +
                    "cấp trên khác trước.");
            }

            return truyXuat.XoaDonVi(ma);
        }

        /// <summary>
        /// Mã các đơn vị có trường "Đơn vị cấp trên" trỏ tới
        /// đơn vị đã cho (theo mã, tên hoặc tên viết tắt).
        /// Kiểm tra trên toàn bộ dữ liệu, không lọc theo quyền xem,
        /// vì đây là ràng buộc toàn vẹn chứ không phải tra cứu.
        /// </summary>
        public List<string> TimMaDonViCapDuoi(
            DonViQuanDoi capTren)
        {
            BaoDamCauHinh();

            if (capTren == null)
                return new List<string>();

            IList<DonViQuanDoi> tatCa =
                truyXuat.LayTatCaDonVi()
                ?? new List<DonViQuanDoi>();

            return tatCa
                .Where(x =>
                    x != null &&
                    !string.IsNullOrWhiteSpace(x.MaDonVi) &&
                    !string.Equals(
                        x.MaDonVi, capTren.MaDonVi,
                        StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(x.DonViCapTren) &&
                    LaCungDonVi(x.DonViCapTren, capTren))
                .Select(x => x.MaDonVi.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // 8. THỐNG KÊ TỔNG QUAN

        public TongQuanDonViQuanDoi LayTongQuan()
        {
            List<DonViQuanDoi> ds =
                LayTatCaDonVi();

            int soCoToaDo =
                ds.Count(CoToaDoWgs84);

            return new TongQuanDonViQuanDoi
            {
                TongSoDonVi = ds.Count,
                SoCoToaDoWgs84 = soCoToaDo,
                SoThieuToaDoWgs84 =
                    ds.Count - soCoToaDo
            };
        }

        public static bool CoToaDoWgs84(
            DonViQuanDoi dv)
        {
            return dv != null &&
                LaSoHuuHan(dv.KinhDo) &&
                LaSoHuuHan(dv.ViDo) &&
                dv.KinhDo >= -180d &&
                dv.KinhDo <= 180d &&
                dv.ViDo >= -90d &&
                dv.ViDo <= 90d &&
                !(dv.KinhDo == 0d && dv.ViDo == 0d);
        }

        // 9. CÁC HÀM HỖ TRỢ NGHIỆP VỤ

        private void KiemTraDeGhi(
            DonViQuanDoi dv)
        {
            // Chỉ kiểm tra dữ liệu nhập mới.
            // Không chặn sửa hồ sơ do cảnh báo cũ
            // đã được phát hiện trước khi cập nhật.
            KetQuaKiemTraDonVi kq =
                KiemTraDuLieuNoiBo(dv, true, false);

            if (!kq.HopLe)
            {
                // Không truyền paramName: .NET sẽ nối thêm
                // "Parameter name: dv" vào thông báo cho người dùng.
                throw new ArgumentException(
                    kq.LayThongBao());
            }
        }

        private void BaoDamCauHinh()
        {
            if (!DaCauHinh)
            {
                throw new InvalidOperationException(
                    "BLL chưa được gắn DAL " +
                    "và chính sách quyền.");
            }
        }

        private void BaoDamQuyen(
            ThaoTacDonViQuanDoi thaoTac,
            string ma)
        {
            BaoDamCauHinh();

            if (!phanQuyen.DuocPhep(thaoTac, ma))
            {
                throw new UnauthorizedAccessException(
                    "Tài khoản không được phép " +
                    "thực hiện thao tác.");
            }
        }

        private static string KiemTraMa(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma))
            {
                throw new ArgumentException(
                    "Mã đơn vị không được trống.",
                    "ma");
            }

            ma = ma.Trim();

            if (ma.Length > 50)
            {
                throw new ArgumentOutOfRangeException(
                    "ma",
                    "Mã đơn vị tối đa 50 ký tự.");
            }

            return ma;
        }

        private static void KiemTraChuoi(
            string value,
            int max,
            string ten,
            bool batBuoc,
            KetQuaKiemTraDonVi kq)
        {
            if (batBuoc &&
                string.IsNullOrWhiteSpace(value))
            {
                kq.ThemLoi(
                    ten + " không được để trống.");
            }
            else if (value != null &&
                     value.Trim().Length > max)
            {
                kq.ThemLoi(
                    ten + " quá " + max + " ký tự.");
            }
        }

        private static void ChuanHoaChuoi(
            DonViQuanDoi dv)
        {
            dv.MaDonVi =
                (dv.MaDonVi ?? "").Trim();

            dv.TenDonVi =
                (dv.TenDonVi ?? "").Trim();

            dv.TenVietTat =
                (dv.TenVietTat ?? "").Trim();

            dv.LoaiDonVi =
                (dv.LoaiDonVi ?? "").Trim();

            dv.QuanBinhChung =
                (dv.QuanBinhChung ?? "").Trim();

            dv.DonViCapTren =
                (dv.DonViCapTren ?? "").Trim();

            dv.QuanHuyen =
                (dv.QuanHuyen ?? "").Trim();

            dv.DiaChi =
                (dv.DiaChi ?? "").Trim();

            dv.ChiHuyTruong =
                (dv.ChiHuyTruong ?? "").Trim();

            dv.ChinhUy =
                (dv.ChinhUy ?? "").Trim();

            dv.SoDienThoai =
                (dv.SoDienThoai ?? "").Trim();

            dv.TrangThaiSSCD =
                (dv.TrangThaiSSCD ?? "").Trim();

            dv.DoMat =
                (dv.DoMat ?? "").Trim();
        }

        /// <summary>
        /// So sánh tên/mã đơn vị cấp trên (chuỗi tự do) với
        /// mã, tên, tên viết tắt của một đơn vị, bỏ dấu, không
        /// phân biệt hoa thường.
        /// </summary>
        private static bool LaCungDonVi(
            string tenHoacMa,
            DonViQuanDoi dv)
        {
            string a = ChuanHoaTimKiem(tenHoacMa);

            if (a.Length == 0 || dv == null)
                return false;

            return a == ChuanHoaTimKiem(dv.MaDonVi) ||
                   a == ChuanHoaTimKiem(dv.TenDonVi) ||
                   (!string.IsNullOrWhiteSpace(dv.TenVietTat) &&
                    a == ChuanHoaTimKiem(dv.TenVietTat));
        }

        private static bool LaSoDienThoaiHopLe(string s)
        {
            int soChuSo = 0;

            foreach (char c in s.Trim())
            {
                if (c >= '0' && c <= '9')
                    soChuSo++;
                else if (" +-.()".IndexOf(c) < 0)
                    return false;
            }

            return soChuSo >= 6 && soChuSo <= 15;
        }

        private static bool LaSoHuuHan(double x)
        {
            return !double.IsNaN(x) &&
                   !double.IsInfinity(x);
        }

        private static string ChuanHoaTimKiem(
            string input)
        {
            string s = (input ?? "")
                .Trim()
                .ToLowerInvariant()
                .Replace('đ', 'd')
                .Normalize(NormalizationForm.FormD);

            StringBuilder result =
                new StringBuilder(s.Length);

            foreach (char c in s)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }

            return result.ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
