
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using MapWinGIS;

using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    // ============================================================
    // HQTCSDL - NAP DU LIEU HO SO
    //
    // C# 7.3 / .NET Framework 4.8 / MapWinGIS
    //
    // PHIEN BAN: DONG BO TRA CUU - QUAN LY DU LIEU
    //
    // Nguon hien hanh:
    // - Danh muc Ban CHQS phuong/xa
    // - Ho so GIS quan doi cac cap
    //
    // Ho so BLL cu van giu cho cac nghiep vu hien co,
    // nhung KHONG cong vao tong so don vi hien hanh.
    //
    // Khong thay doi bang PostGIS.
    // Khong thay doi logic anh logo trong file rieng.
    // ============================================================

    public partial class FormMain
    {
        // ========================================================
        // 1. BO NHO DANH MUC BAN CHQS CAP XA
        // ========================================================

        private List<DanhMucBanChqsCapXa2026>
            nativeDanhMucBchqsTraCuu;

        private DateTime nativeLanTaiDanhMucBchqs =
            DateTime.MinValue;

        private bool nativePhaiTaiLaiDanhMucBchqs = true;

        private string nativeLoiTaiDanhMucBchqs;

        // ========================================================
        // 2. BO NHO HO SO GIS CAC CAP
        // ========================================================

        private List<HoSoGisHqtc>
            nativeHoSoGisHqtcTraCuu;

        private DateTime nativeLanTaiHoSoHqtc =
            DateTime.MinValue;

        private bool nativePhaiTaiLaiHoSoHqtc = true;

        private string nativeLoiTaiHoSoHqtc;

        private int nativeSoHoSoHqtcDoc;

        // ========================================================
        // 3. CAC LOP BAN DO
        // ========================================================

        private int nativeLopVeDiemDaiDien = -1;

        private int nativeLopVeBchqs169 = -1;

        private int nativeLopVeHoSo169 = -1;

        private bool nativeDaDangKyBanDo169;

        private bool nativeDaVeLopPhu169;

        private bool nativeDaCanKhung169;

        private System.Windows.Forms.Timer
            nativeHenDongBoLop169;

        private int nativeSoLanDongBo169;

        private const string Native_KeyBchqs169 =
            "HQTCSDL:BCHQS169";

        private const string Native_KeyHoSo169 =
            "HQTCSDL:HOSO169";

        private const string Native_KeyDiemChon169 =
            "HQTCSDL:DIEMCHON169";

        // ========================================================
        // 4. NAP DU LIEU TAT CA PHAN HE
        // ========================================================

        private void TaiLaiDuLieuPhanHe()
        {
            // ----------------------------------------------------
            // 4.1. KIEM TRA CAU HINH VA QUYEN XEM
            // ----------------------------------------------------

            if (nghiepVuDonVi == null ||
                !nghiepVuDonVi.DaCauHinh ||
                !CoQuyen(ThaoTacDonViQuanDoi.Xem))
            {
                nativeBoNhoHoSo = null;

                nativeDanhMucBchqsTraCuu = null;
                nativePhaiTaiLaiHoSo = true;

                nativePhaiTaiLaiDanhMucBchqs = true;
                nativeLoiTaiDanhMucBchqs = null;

                nativeHoSoGisHqtcTraCuu = null;
                nativePhaiTaiLaiHoSoHqtc = true;
                nativeLoiTaiHoSoHqtc = null;

                nativeSoHoSoHqtcDoc = 0;

                Native_XoaLopPhu169();

                // Xoa du lieu khoi man hinh Quan ly.
                UcQuanLyDuLieuNative qlRong =
                    TimManHinh<UcQuanLyDuLieuNative>(
                        "Quản lý dữ liệu");

                if (qlRong != null && !qlRong.IsDisposed)
                {
                    qlRong.NapDuLieu(
                        new DonViQuanDoi[0],
                        new DanhMucBanChqsCapXa2026[0],
                        new HoSoGisHqtc[0]);
                }

                // Xoa du lieu khoi man hinh Tra cuu.
                GiaoDienTraCuuDonVi tcRong =
                    TimManHinh<GiaoDienTraCuuDonVi>(
                        "Tra cứu đơn vị");

                if (tcRong != null && !tcRong.IsDisposed)
                {
                    tcRong.NapDuLieu(
                        new DonViQuanDoi[0],
                        new DanhMucBanChqsCapXa2026[0],
                        new HoSoGisHqtc[0]);
                }

                // Bao cao cu van dung du lieu BLL.
                UcThongKeBaoCaoNative bcRong =
                    TimManHinh<UcThongKeBaoCaoNative>(
                        "Thống kê, báo cáo");

                if (bcRong != null && !bcRong.IsDisposed)
                {
                    bcRong.NapDuLieu(
                        new DonViQuanDoi[0]);
                }

                // Cap nhat trang thai PostGIS.
                GiaoDienTongQuan tqRong =
                    TimManHinh<GiaoDienTongQuan>(
                        "Tổng quan");

                if (tqRong != null && !tqRong.IsDisposed)
                {
                    tqRong.CapNhatTrangThaiPostGIS(
                        daKetNoiPostGIS);
                }

                statusGeneral.Text =
                    "● Chưa cấu hình nghiệp vụ hoặc quyền xem.";

                return;
            }

            // ----------------------------------------------------
            // 4.2. DANG KY SU KIEN MO BAN DO
            // ----------------------------------------------------

            if (!nativeDaDangKyBanDo169 &&
                btnNavMap != null)
            {
                btnNavMap.Click += (s, e) =>
                {
                    if (!IsDisposed &&
                        !Disposing &&
                        IsHandleCreated)
                    {
                        nativeDaVeLopPhu169 = false;

                        BeginInvoke(
                            new Action(
                                Native_HenDongBoLop169));
                    }
                };

                nativeDaDangKyBanDo169 = true;
            }

            try
            {
                // ------------------------------------------------
                // 4.3. KIEM TRA CACHE
                // ------------------------------------------------

                bool canTaiHoSo =
                    nativePhaiTaiLaiHoSo ||
                    nativeBoNhoHoSo == null ||
                    DateTime.UtcNow - nativeLanTaiHoSo >
                    nativeThoiHanBoNho;

                bool canTaiDanhMuc =
                    nativePhaiTaiLaiDanhMucBchqs ||
                    nativeDanhMucBchqsTraCuu == null ||
                    DateTime.UtcNow - nativeLanTaiDanhMucBchqs >
                    nativeThoiHanBoNho;

                bool canTaiHoSoHqtc =
                    nativePhaiTaiLaiHoSoHqtc ||
                    nativeHoSoGisHqtcTraCuu == null ||
                    DateTime.UtcNow - nativeLanTaiHoSoHqtc >
                    nativeThoiHanBoNho;

                string loiHoSo = null;

                // ------------------------------------------------
                // 4.4. NAP HO SO CHINH TU BLL
                //
                // Nguon nay duoc giu cho cac nghiep vu
                // quan ly ho so, thong ke, bao cao cu.
                //
                // KHONG cong vao tong 169 don vi hien hanh.
                // ------------------------------------------------

                if (canTaiHoSo)
                {
                    try
                    {
                        List<DonViQuanDoi> hoSo =
                            nghiepVuDonVi.LayTatCaDonVi();

                        if (hoSo == null)
                        {
                            throw new InvalidOperationException(
                                "BLL trả về danh sách hồ sơ null.");
                        }

                        nativeBoNhoHoSo = hoSo
                            .Where(d => d != null)
                            .ToList();

                        nativeLanTaiHoSo = DateTime.UtcNow;

                        nativePhaiTaiLaiHoSo = false;
                    }
                    catch (Exception ex)
                    {
                        nativeBoNhoHoSo =
                            new List<DonViQuanDoi>();

                        nativeLanTaiHoSo = DateTime.UtcNow;

                        nativePhaiTaiLaiHoSo = true;

                        loiHoSo = ex.Message;

                        GhiNhatKyBanDo(
                            "MAIN_RECORD_LOAD_FAILED | " +
                            ex);
                    }
                }

                // ------------------------------------------------
                // 4.5. NAP DANH MUC BAN CHQS PHUONG/XA
                //
                // Chi nhan don vi cap xa hien hanh.
                // Loai bo Ban CHQS quan/huyen cu.
                // Loc theo quyen xem.
                // Loai bo ban ghi trung ma DVHC.
                // ------------------------------------------------

                if (canTaiDanhMuc)
                {
                    try
                    {
                        TruyXuatDonVi dalDanhMuc =
                            new TruyXuatDonVi();

                        IList<DanhMucBanChqsCapXa2026>
                            tatCa =
                            dalDanhMuc.LayDanhMucBanChqsCapXa();

                        nativeDanhMucBchqsTraCuu =
                            (tatCa ??
                                new List<DanhMucBanChqsCapXa2026>())
                            .Where(d =>
                                Native_LaBanChqsPhuongXa(d) &&
                                CoQuyen(
                                    ThaoTacDonViQuanDoi.Xem,
                                    d.MaDvhc.Trim()))
                            .GroupBy(
                                d => d.MaDvhc.Trim(),
                                StringComparer.OrdinalIgnoreCase)
                            .Select(g => g.First())
                            .OrderBy(
                                d => d.TenDonVi,
                                StringComparer.CurrentCultureIgnoreCase)
                            .ToList();

                        nativeLoiTaiDanhMucBchqs = null;

                        if (tatCa != null &&
                            tatCa.Count > 0 &&
                            nativeDanhMucBchqsTraCuu.Count == 0)
                        {
                            nativeLoiTaiDanhMucBchqs =
                                "Đọc được " +
                                tatCa.Count +
                                " mục từ HQTCSDL nhưng không có " +
                                "bản ghi phường/xã hợp lệ " +
                                "trong phạm vi quyền xem.";
                        }
                    }
                    catch (Exception ex)
                    {
                        nativeDanhMucBchqsTraCuu =
                            new List<DanhMucBanChqsCapXa2026>();

                        nativeLoiTaiDanhMucBchqs =
                            ex.Message;

                        GhiNhatKyBanDo(
                            "BCHQS_CATALOG_LOAD_FAILED | " +
                            ex);
                    }

                    nativeLanTaiDanhMucBchqs =
                        DateTime.UtcNow;

                    nativePhaiTaiLaiDanhMucBchqs = false;
                }

                // ------------------------------------------------
                // 4.6. NAP HO SO GIS QUAN DOI CAC CAP
                //
                // Nguon HQTCSDL:
                //   ql_don_vi.v_don_vi_ban_do
                //
                // Du lieu duoc DAL xu ly.
                //
                // - Hoc vien
                // - Truong si quan
                // - Bo tu lenh
                // - Quan/binh chung
                // - Su doan
                // - Lu doan
                // - Trung doan
                // - Cac don vi khac co trong view
                // ------------------------------------------------

                if (canTaiHoSoHqtc)
                {
                    try
                    {
                        TruyXuatDonVi dal =
                            new TruyXuatDonVi();

                        IList<HoSoGisHqtc> tatCa =
                            dal.LayHoSoGisHqtc();

                        if (tatCa == null)
                        {
                            throw new InvalidOperationException(
                                "DAL trả về danh sách " +
                                "hồ sơ HQTCSDL null.");
                        }

                        nativeSoHoSoHqtcDoc =
                            tatCa.Count;

                        nativeHoSoGisHqtcTraCuu =
                            tatCa
                            .Where(d =>
                                d != null &&
                                !string.IsNullOrWhiteSpace(
                                    d.MaDonVi) &&
                                CoQuyen(
                                    ThaoTacDonViQuanDoi.Xem,
                                    d.MaDonVi.Trim()))
                            .GroupBy(d => d.Id)
                            .Select(g => g.First())
                            .OrderBy(
                                d => d.TenDonVi,
                                StringComparer.CurrentCultureIgnoreCase)
                            .ToList();

                        nativeLoiTaiHoSoHqtc = null;
                    }
                    catch (Exception ex)
                    {
                        nativeSoHoSoHqtcDoc = 0;

                        nativeHoSoGisHqtcTraCuu =
                            new List<HoSoGisHqtc>();

                        nativeLoiTaiHoSoHqtc =
                            ex.Message;

                        GhiNhatKyBanDo(
                            "HQTCSDL_43_LOAD_FAILED | " +
                            ex);
                    }

                    nativeLanTaiHoSoHqtc =
                        DateTime.UtcNow;

                    nativePhaiTaiLaiHoSoHqtc = false;
                }

                // ------------------------------------------------
                // 4.7. CAP NHAT PHIEN BAN DU LIEU
                // ------------------------------------------------

                if (canTaiHoSo ||
                    canTaiDanhMuc ||
                    canTaiHoSoHqtc)
                {
                    nativePhienBanDuLieu++;

                    nativeDaVeLopPhu169 = false;
                }

                // ------------------------------------------------
                // 4.8. TONG HOP CAC DANH SACH
                // ------------------------------------------------

                List<DonViQuanDoi> ds =
                    nativeBoNhoHoSo ??
                    new List<DonViQuanDoi>();

                List<DanhMucBanChqsCapXa2026> danhMuc =
                    nativeDanhMucBchqsTraCuu ??
                    new List<DanhMucBanChqsCapXa2026>();

                List<HoSoGisHqtc> hoSoHqtc =
                    nativeHoSoGisHqtcTraCuu ??
                    new List<HoSoGisHqtc>();

                // ------------------------------------------------
                // 4.9. NAP TRANG TRA CUU DON VI
                //
                // Hai nguon hien hanh:
                //   danhMuc + hoSoHqtc
                //
                // Khong nap ds cu vao Tra cuu.
                // ------------------------------------------------

                GiaoDienTraCuuDonVi tc =
                    TimManHinh<GiaoDienTraCuuDonVi>(
                        "Tra cứu đơn vị");

                if (tc != null && !tc.IsDisposed)
                {
                    tc.YeuCauDinhViDiemDaiDien -=
                        Native_DinhViDiemDaiDien;

                    tc.YeuCauDinhViDiemDaiDien +=
                        Native_DinhViDiemDaiDien;

                    tc.YeuCauDinhViHoSoHqtc -=
                        Native_DinhViHoSoHqtc;

                    tc.YeuCauDinhViHoSoHqtc +=
                        Native_DinhViHoSoHqtc;
                }

                if (Native_CanNapTrang(tc))
                {
                    tc.NapDuLieu(
                        new DonViQuanDoi[0],
                        danhMuc,
                        hoSoHqtc);

                    Native_DanhDauTrangDaNap(tc);
                }

                // ------------------------------------------------
                // 4.10. NAP TRANG QUAN LY DU LIEU
                //
                // SUA LOI 177 KHAC 169
                //
                // Truoc day:
                //   ql.NapDuLieu(ds);
                //
                // Bay gio:
                //   ql.NapDuLieu(ds, danhMuc, hoSoHqtc);
                //
                // ds chi de doi chieu ho so co quyen ghi.
                // Danh sach hien thi la hai nguon HQTCSDL.
                // ------------------------------------------------

                UcQuanLyDuLieuNative ql =
                    TimManHinh<UcQuanLyDuLieuNative>(
                        "Quản lý dữ liệu");

                if (ql != null && !ql.IsDisposed)
                {
                    // Dinh vi diem dai dien Ban CHQS.
                    ql.YeuCauDinhViDiemDaiDien -=
                        Native_DinhViDiemDaiDien;

                    ql.YeuCauDinhViDiemDaiDien +=
                        Native_DinhViDiemDaiDien;

                    // Dinh vi ho so GIS cac cap.
                    ql.YeuCauDinhViHoSoHqtc -=
                        Native_DinhViHoSoHqtc;

                    ql.YeuCauDinhViHoSoHqtc +=
                        Native_DinhViHoSoHqtc;
                }

                if (Native_CanNapTrang(ql))
                {
                    // Su dung cung nguon voi Tra cuu.
                    ql.NapDuLieu(
                        ds,
                        danhMuc,
                        hoSoHqtc);

                    Native_DanhDauTrangDaNap(ql);
                }

                // ------------------------------------------------
                // 4.11. THONG KE BAO CAO
                //
                // Tam giu nguyen logic bao cao BLL.
                // Khong tac dong den thong ke cu.
                // ------------------------------------------------

                UcThongKeBaoCaoNative bc =
                    TimManHinh<UcThongKeBaoCaoNative>(
                        "Thống kê, báo cáo");

                if (Native_CanNapTrang(bc))
                {
                    bc.NapDuLieu(ds);

                    // Chat luong vi tri (v_thong_ke_vi_tri): vi tri that / diem dai dien.
                    try
                    {
                        TongHopViTri th = new TruyXuatViTri().LayTongHopViTri();
                        bc.NapChatLuongViTri(th.ViTriTot, th.CanDoLai, th.DiemDaiDien);
                    }
                    catch (Exception exViTri)
                    {
                        GhiNhatKyBanDo("REPORT_POSITION_STATS_FAILED | " + exViTri.Message);
                        bc.NapChatLuongViTri(null, null, null);
                    }

                    Native_DanhDauTrangDaNap(bc);
                }

                // ------------------------------------------------
                // 4.12. TONG QUAN
                //
                // Giu nguyen cau truc DuLieuTongQuan.
                // Du lieu tong quan van thuoc nghiep vu BLL.
                // ------------------------------------------------

                GiaoDienTongQuan tq =
                    TimManHinh<GiaoDienTongQuan>(
                        "Tổng quan");

                if (Native_CanNapTrang(tq))
                {
                    int coToaDo =
                        ds.Count(
                            NghiepVuDonVi.CoToaDoWgs84);

                    List<NhomTongQuanItem> theoLoai =
                        ds.GroupBy(
                            d => string.IsNullOrWhiteSpace(
                                d.LoaiDonVi)
                                ? "Chưa phân loại"
                                : d.LoaiDonVi.Trim(),
                            StringComparer.CurrentCultureIgnoreCase)
                        .Select(g => new NhomTongQuanItem
                        {
                            Ten = g.Key,
                            SoLuong = g.LongCount()
                        })
                        .OrderByDescending(x => x.SoLuong)
                        .ThenBy(x => x.Ten)
                        .ToList();

                    List<NhomTongQuanItem> theoDiaBan =
                        ds.GroupBy(
                            d => string.IsNullOrWhiteSpace(
                                d.QuanHuyen)
                                ? "Chưa xác định"
                                : d.QuanHuyen.Trim(),
                            StringComparer.CurrentCultureIgnoreCase)
                        .Select(g => new NhomTongQuanItem
                        {
                            Ten = g.Key,
                            SoLuong = g.LongCount()
                        })
                        .OrderByDescending(x => x.SoLuong)
                        .ThenBy(x => x.Ten)
                        .ToList();

                    tq.ChoPhepThemDonVi =
                        CoQuyen(
                            ThaoTacDonViQuanDoi.Them);

                    TongHopViTri thTq = null;
                    try { thTq = new TruyXuatViTri().LayTongHopViTri(); }
                    catch (Exception exTq) { GhiNhatKyBanDo("OVERVIEW_POSITION_STATS_FAILED | " + exTq.Message); }

                    tq.NapDuLieu(
                        new DuLieuTongQuan
                        {
                            TongSoDonVi = ds.Count,
                            SoViTriThat = thTq == null ? (int?)null : thTq.ViTriTot + thTq.CanDoLai,
                            SoDiemDaiDien = thTq == null ? (int?)null : thTq.DiemDaiDien,

                            SoCoToaDo = coToaDo,

                            SoThieuToaDo =
                                ds.Count - coToaDo,

                            SoLopBanDo = cacLop.Count,

                            SoNhomLoaiDonVi =
                                theoLoai.Count,

                            SoDiaBanCoHoSo =
                                theoDiaBan.Count,

                            PhanBoTheoLoai =
                                theoLoai,

                            PhanBoTheoDiaBan =
                                theoDiaBan,

                            HoatDongGanDay = TongQuan_HoatDongGanDay(ds),

                            // Uu tien don vi chu luc (khong phai Ban CHQS cap xa).
                            HoSoNoiBat =
                                ds
                                .OrderBy(d => NghiepVuDonVi.LaBanChqsCapXa(d) ? 1 : 0)
                                .ThenBy(
                                    d => d.TenDonVi)
                                .Take(4)
                                .Select(
                                    d =>
                                        new HoSoNoiBatTongQuanItem
                                        {
                                            MaDonVi =
                                                d.MaDonVi,

                                            TenDonVi =
                                                d.TenDonVi,

                                            LoaiDonVi =
                                                d.LoaiDonVi,

                                            DiaBan =
                                                d.QuanHuyen
                                        })
                                .ToList(),

                            DaKetNoiPostGIS =
                                daKetNoiPostGIS,

                            HeQuyChieu =
                                banDoMapWinGIS == null
                                ? null
                                : "EPSG:" + EpsgBanDo,

                            NguonDuLieu =
                                "PostGIS - hồ sơ được phép xem",

                            ThoiDiemCapNhat =
                                DateTime.Now
                        });

                    Native_DanhDauTrangDaNap(tq);
                }

                // ------------------------------------------------
                // 4.13. THONG BAO KET QUA NAP DU LIEU
                // ------------------------------------------------

                int coDiemDaiDien =
                    danhMuc.Count(
                        d => d.CoDiemDaiDien);

                int coDiemHoSoHqtc =
                    hoSoHqtc.Count(
                        d => d.CoDiemBanDo);

                int tongHienHanh =
                    danhMuc.Count +
                    hoSoHqtc.Count;

                string thongBao =
                    "● HQTCSDL: " +
                    tongHienHanh +
                    " đơn vị hiện hành; " +
                    danhMuc.Count +
                    " Ban CHQS phường/xã (" +
                    coDiemDaiDien +
                    " điểm đại diện), " +
                    hoSoHqtc.Count +
                    " hồ sơ GIS các cấp (" +
                    coDiemHoSoHqtc +
                    " điểm đơn vị).";

                if (nativeSoHoSoHqtcDoc !=
                    hoSoHqtc.Count &&
                    string.IsNullOrWhiteSpace(
                        nativeLoiTaiHoSoHqtc))
                {
                    thongBao +=
                        " Đã đọc " +
                        nativeSoHoSoHqtcDoc +
                        " hồ sơ GIS; số còn lại " +
                        "không thuộc quyền xem.";
                }

                if (!string.IsNullOrWhiteSpace(loiHoSo))
                {
                    thongBao +=
                        " Lỗi nguồn hồ sơ chính: " +
                        loiHoSo;
                }

                if (!string.IsNullOrWhiteSpace(
                    nativeLoiTaiDanhMucBchqs))
                {
                    thongBao +=
                        " Lỗi danh mục: " +
                        nativeLoiTaiDanhMucBchqs;
                }

                if (!string.IsNullOrWhiteSpace(
                    nativeLoiTaiHoSoHqtc))
                {
                    thongBao +=
                        " Lỗi đọc hồ sơ HQTCSDL: " +
                        nativeLoiTaiHoSoHqtc;
                }

                statusGeneral.Text = thongBao;

                // ------------------------------------------------
                // 4.14. DONG BO BAN DO NEU DANG HIEN THI
                // ------------------------------------------------

                if (phanHeHienTai == "Bản đồ GIS" &&
                    IsHandleCreated)
                {
                    BeginInvoke(
                        new Action(
                            Native_HenDongBoLop169));
                }
            }
            catch (Exception ex)
            {
                nativePhaiTaiLaiHoSo = true;

                nativePhaiTaiLaiDanhMucBchqs = true;

                nativePhaiTaiLaiHoSoHqtc = true;

                statusGeneral.Text =
                    "● Lỗi nạp dữ liệu: " +
                    ex.Message;

                GhiNhatKyBanDo(
                    "NATIVE_DATA_LOAD_FAILED | " +
                    ex);
            }
        }

        // ========================================================
        // 5. KIEM TRA BAN CHQS PHUONG/XA
        //
        // Khong chap nhan Ban CHQS quan/huyen cu.
        // ========================================================

        private static bool Native_LaBanChqsPhuongXa(
            DanhMucBanChqsCapXa2026 muc)
        {
            if (muc == null ||
                string.IsNullOrWhiteSpace(
                    muc.MaDvhc))
            {
                return false;
            }

            string diaBan =
                (muc.PhuongXa ?? "").Trim();

            string ten =
                (muc.TenDonVi ?? "").Trim();

            bool laPhuongXa =
                diaBan.StartsWith(
                    "Phường ",
                    StringComparison.CurrentCultureIgnoreCase)
                ||
                diaBan.StartsWith(
                    "Xã ",
                    StringComparison.CurrentCultureIgnoreCase);

            bool tenDungCap =
                ten.IndexOf(
                    "phường ",
                    StringComparison.CurrentCultureIgnoreCase) >= 0
                ||
                ten.IndexOf(
                    "xã ",
                    StringComparison.CurrentCultureIgnoreCase) >= 0;

            bool tenCapCu =
                ten.IndexOf(
                    "huyện ",
                    StringComparison.CurrentCultureIgnoreCase) >= 0
                ||
                ten.IndexOf(
                    "quận ",
                    StringComparison.CurrentCultureIgnoreCase) >= 0
                ||
                ten.IndexOf(
                    "thị xã ",
                    StringComparison.CurrentCultureIgnoreCase) >= 0;

            return
                laPhuongXa &&
                tenDungCap &&
                !tenCapCu;
        }

        // ========================================================
        // 6. DINH VI BAN CHQS THEO DIEM DAI DIEN
        //
        // Diem dai dien la vi tri hanh chinh,
        // KHONG mac dinh la toa do tru so quan doi.
        // ========================================================

        private void Native_DinhViDiemDaiDien(
            string maDvhc,
            double x32648,
            double y32648,
            string tenDonVi)
        {
            if (IsDisposed || Disposing)
                return;

            if (string.IsNullOrWhiteSpace(maDvhc) ||
                !CoQuyen(
                    ThaoTacDonViQuanDoi.Xem,
                    maDvhc.Trim()))
            {
                statusGeneral.Text =
                    "● Không có quyền xem địa bàn này.";

                return;
            }

            DanhMucBanChqsCapXa2026 d =
                nativeDanhMucBchqsTraCuu == null
                ? null
                : nativeDanhMucBchqsTraCuu
                    .FirstOrDefault(
                        item =>
                            item != null &&
                            string.Equals(
                                item.MaDvhc,
                                maDvhc,
                                StringComparison.OrdinalIgnoreCase));

            if (d == null ||
                !d.CoDiemDaiDien ||
                !d.XDaiDien32648.HasValue ||
                !d.YDaiDien32648.HasValue ||
                Math.Abs(
                    d.XDaiDien32648.Value -
                    x32648) > 0.01 ||
                Math.Abs(
                    d.YDaiDien32648.Value -
                    y32648) > 0.01)
            {
                statusGeneral.Text =
                    "● Vị trí đại diện đã thay đổi. " +
                    "Hãy nạp lại dữ liệu.";

                return;
            }

            if (!d.KinhDoDaiDien.HasValue ||
                !d.ViDoDaiDien.HasValue)
            {
                statusGeneral.Text =
                    "● Hồ sơ thiếu tọa độ WGS84 " +
                    "của điểm đại diện.";

                return;
            }

            Native_HienThiDiemTraCuu(
                d.KinhDoDaiDien.Value,
                d.ViDoDaiDien.Value,
                tenDonVi ?? d.TenDonVi,
                "điểm đại diện địa bàn phường/xã, " +
                "không phải trụ sở Ban CHQS");
        }

        // ========================================================
        // 7. DINH VI HO SO GIS CAC CAP
        // ========================================================

        private void Native_DinhViHoSoHqtc(
            long id,
            double x32648,
            double y32648,
            string tenDonVi)
        {
            if (IsDisposed || Disposing)
                return;

            HoSoGisHqtc d =
                nativeHoSoGisHqtcTraCuu == null
                ? null
                : nativeHoSoGisHqtcTraCuu
                    .FirstOrDefault(
                        item =>
                            item != null &&
                            item.Id == id);

            if (d == null ||
                string.IsNullOrWhiteSpace(d.MaDonVi) ||
                !CoQuyen(
                    ThaoTacDonViQuanDoi.Xem,
                    d.MaDonVi.Trim()) ||
                !d.CoDiemBanDo ||
                !d.X32648.HasValue ||
                !d.Y32648.HasValue ||
                Math.Abs(
                    d.X32648.Value -
                    x32648) > 0.01 ||
                Math.Abs(
                    d.Y32648.Value -
                    y32648) > 0.01)
            {
                statusGeneral.Text =
                    "● Hồ sơ hoặc quyền xem đã thay đổi. " +
                    "Hãy nạp lại dữ liệu.";

                return;
            }

            if (!d.KinhDo.HasValue ||
                !d.ViDo.HasValue)
            {
                statusGeneral.Text =
                    "● Hồ sơ thiếu tọa độ WGS84 " +
                    "để định vị.";

                return;
            }

            Native_HienThiDiemTraCuu(
                d.KinhDo.Value,
                d.ViDo.Value,
                tenDonVi ?? d.TenDonVi,
                "vị trí điểm của hồ sơ GIS");
        }

        // ========================================================
        // 8. HIEN THI DIEM TREN MAPWINGIS
        //
        // Su dung toa do WGS84 lam dau vao.
        // AxMap chuyen sang CRS hien tai.
        // ========================================================

        private void Native_HienThiDiemTraCuu(
            double kinhDo,
            double viDo,
            string tenDonVi,
            string moTaViTri)
        {
            if (!Native_LaWgs84HopLe(
                kinhDo, viDo))
            {
                statusGeneral.Text =
                    "● Tọa độ WGS84 không hợp lệ.";

                return;
            }

            try
            {
                ChonPhanHe(
                    btnNavMap,
                    "Bản đồ GIS");

                if (!IsHandleCreated)
                {
                    statusGeneral.Text =
                        "● Giao diện bản đồ chưa sẵn sàng.";

                    return;
                }

                BeginInvoke(new Action(() =>
                {
                    if (IsDisposed || Disposing)
                        return;

                    try
                    {
                        if (banDoMapWinGIS == null ||
                            banDoMapWinGIS.IsDisposed)
                        {
                            statusGeneral.Text =
                                "● MapWinGIS chưa khởi tạo.";

                            return;
                        }

                        double x;
                        double y;

                        if (!Native_Wgs84SangToaDoBanDo(
                            kinhDo,
                            viDo,
                            out x,
                            out y))
                        {
                            statusGeneral.Text =
                                "● Không thể chuyển WGS84 " +
                                "sang hệ tọa độ AxMap.";

                            return;
                        }

                        // Dong bo lop phu hien hanh.
                        Native_VeLopPhu169();

                        // An lop du lieu cu neu da
                        // tao thanh cong lop hien hanh.
                        Native_AnLop51CuNeuDungNguon();

                        // Xoa drawing tam da chon truoc do.
                        Native_XoaLopVeTheoKey(
                            ref nativeLopVeDiemDaiDien,
                            Native_KeyDiemChon169);

                        // Khoang xem dung don vi phang cua
                        // ban do hien tai.
                        Extents ext = new Extents();

                        ext.SetBounds(
                            x - 1400d,
                            y - 1000d,
                            0d,
                            x + 1400d,
                            y + 1000d,
                            0d);

                        banDoMapWinGIS.Extents = ext;

                        // Hien thi logo tai diem dang chon.
                        // Ham nam trong LogoDonViQuanDoi.cs.
                        Native_DongBoLogoDinhVi(x, y);

                        banDoMapWinGIS.Redraw();

                        statusGeneral.Text =
                            "● " +
                            tenDonVi +
                            " — " +
                            moTaViTri +
                            ". Đã định vị trên bản đồ.";
                    }
                    catch (Exception ex)
                    {
                        GhiNhatKyBanDo(
                            "HQTCSDL_MAP_POINT_FAILED | " +
                            ex);

                        statusGeneral.Text =
                            "● Không thể định vị: " +
                            ex.Message;
                    }
                }));
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "HQTCSDL_MAP_NAV_FAILED | " +
                    ex);

                statusGeneral.Text =
                    "● Không thể mở Bản đồ GIS: " +
                    ex.Message;
            }
        }

        // ========================================================
        // 9. KIEM TRA TOA DO WGS84
        // ========================================================

        private static bool Native_LaWgs84HopLe(
            double lon,
            double lat)
        {
            return
                !double.IsNaN(lon) &&
                !double.IsInfinity(lon) &&
                !double.IsNaN(lat) &&
                !double.IsInfinity(lat) &&
                lon >= -180d &&
                lon <= 180d &&
                lat >= -90d &&
                lat <= 90d &&
                !(lon == 0d && lat == 0d);
        }

        // ========================================================
        // 10. CHUYEN TOA DO WGS84 SANG MAP CRS
        // ========================================================

        private bool Native_Wgs84SangToaDoBanDo(
            double lon,
            double lat,
            out double x,
            out double y)
        {
            x = 0d;
            y = 0d;

            if (!Native_LaWgs84HopLe(lon, lat) ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return false;
            }

            // CRS hien tai do AxMap quan ly.
            // Khong tu suy doan tu EPSG trong nhan UI.
            bool ok =
                banDoMapWinGIS.DegreesToProj(
                    lon,
                    lat,
                    ref x,
                    ref y);

            return
                ok &&
                !double.IsNaN(x) &&
                !double.IsInfinity(x) &&
                !double.IsNaN(y) &&
                !double.IsInfinity(y);
        }

        // ========================================================
        // 11. HEN DONG BO LOP 169 DIEM
        // ========================================================

        private void Native_HenDongBoLop169()
        {
            if (IsDisposed ||
                Disposing ||
                !IsHandleCreated)
            {
                return;
            }

            if (nativeHenDongBoLop169 == null)
            {
                nativeHenDongBoLop169 =
                    new System.Windows.Forms.Timer();

                nativeHenDongBoLop169.Interval = 600;

                nativeHenDongBoLop169.Tick +=
                    (sender, args) =>
                    {
                        if (IsDisposed ||
                            Disposing ||
                            phanHeHienTai != "Bản đồ GIS")
                        {
                            nativeHenDongBoLop169.Stop();

                            return;
                        }

                        nativeSoLanDongBo169++;

                        if (!nativeDaVeLopPhu169 ||
                            !Native_LopPhu169ConTonTai())
                        {
                            Native_VeLopPhu169();
                        }

                        if (nativeDaVeLopPhu169)
                        {
                            Native_AnLop51CuNeuDungNguon();
                        }

                        if (nativeSoLanDongBo169 >= 12)
                        {
                            nativeHenDongBoLop169.Stop();
                        }
                    };

                Disposed += (sender, args) =>
                {
                    if (nativeHenDongBoLop169 != null)
                    {
                        nativeHenDongBoLop169.Stop();

                        nativeHenDongBoLop169.Dispose();

                        nativeHenDongBoLop169 = null;
                    }
                };
            }

            nativeSoLanDongBo169 = 0;

            nativeHenDongBoLop169.Stop();

            nativeHenDongBoLop169.Start();
        }

        // ========================================================
        // 12. XOA DRAWING THEO KEY
        // ========================================================

        private void Native_XoaLopVeTheoKey(
            ref int handle,
            string key)
        {
            if (handle < 0)
                return;

            int cu = handle;

            handle = -1;

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            try
            {
                if (string.Equals(
                    banDoMapWinGIS.get_DrawingKey(cu),
                    key,
                    StringComparison.Ordinal))
                {
                    banDoMapWinGIS.ClearDrawing(cu);
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "HQTCSDL_CLEAR_DRAWING | " +
                    ex.Message);
            }
        }

        // ========================================================
        // 13. XOA LOP PHU TAM
        //
        // Khong xoa du lieu PostGIS.
        // ========================================================

        private void Native_XoaLopPhu169()
        {
            if (nativeHenDongBoLop169 != null)
            {
                nativeHenDongBoLop169.Stop();
            }

            nativeDaVeLopPhu169 = false;

            Native_XoaToanBoLogo();

            Native_XoaLopVeTheoKey(
                ref nativeLopVeBchqs169,
                Native_KeyBchqs169);

            Native_XoaLopVeTheoKey(
                ref nativeLopVeHoSo169,
                Native_KeyHoSo169);

            Native_XoaLopVeTheoKey(
                ref nativeLopVeDiemDaiDien,
                Native_KeyDiemChon169);
        }

        // ========================================================
        // 14. AN LOP 51 DON VI CU
        //
        // Lop cu la DATA layer, khong phai drawing.
        // Chi an theo ten lop phu hop.
        // Khong xoa CSDL.
        // ========================================================

        private void Native_AnLop51CuNeuDungNguon()
        {
            if (!nativeDaVeLopPhu169 ||
                !Native_CacLopLogoConTonTai() ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            int demNhanDien = 0;
            int demDaAn = 0;

            try
            {
                for (int i = 0;
                     i < banDoMapWinGIS.NumLayers;
                     i++)
                {
                    int handle =
                        banDoMapWinGIS.get_LayerHandle(i);

                    // Khong an lop ho so quan ly (Sua/Xoa duoc).
                    if (handle < 0 ||
                        handle == handleLopDiemPostGIS)
                        continue;

                    string ten =
                        (banDoMapWinGIS.get_LayerName(handle)
                            ?? "")
                        .Trim()
                        .ToLowerInvariant();

                    // Khong an cac lop HQTCSDL moi.
                    if (ten.Contains("hqtc") ||
                        ten.Contains("phường/xã") ||
                        ten.Contains("phuong/xa"))
                    {
                        continue;
                    }

                    bool laLopDonViCu =
                        ten.Contains("đơn vị quân đội") ||
                        ten.Contains("don vi quan doi") ||
                        ten.Contains("don_vi_quan_doi");

                    if (!laLopDonViCu)
                        continue;

                    demNhanDien++;

                    if (banDoMapWinGIS.get_LayerVisible(
                        handle))
                    {
                        banDoMapWinGIS.set_LayerVisible(
                            handle,
                            false);

                        demDaAn++;

                        GhiNhatKyBanDo(
                            "HQTCSDL_HIDE_OLD_LAYER | " +
                            ten +
                            " | handle=" +
                            handle);
                    }
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "HQTCSDL_HIDE_OLD_LAYER_FAILED | " +
                    ex);
            }

            if (demNhanDien == 0)
            {
                GhiNhatKyBanDo(
                    "HQTCSDL_OLD_LAYER_NOT_FOUND | " +
                    "Cần kiểm tra tên lớp dữ liệu cũ " +
                    "trong hệ thống bản đồ.");
            }
        }

        // ========================================================
        // 15. KIEM TRA LOP PHU CON TON TAI
        // ========================================================

        private bool Native_LopPhu169ConTonTai()
        {
            return Native_CacLopLogoConTonTai();
        }

        // ========================================================
        // 16. VE LOP PHU HQTCSDL
        //
        // Hai lop:
        //   Ban CHQS phuong/xa
        //   Ho so GIS quan doi cac cap
        //
        // Logo va nhan do LogoDonViQuanDoi.cs xu ly.
        // ========================================================

        private void Native_VeLopPhu169()
        {
            if (IsDisposed ||
                Disposing ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                nativeDanhMucBchqsTraCuu == null ||
                nativeHoSoGisHqtcTraCuu == null)
            {
                return;
            }

            // Neu ca hai lop logo da ton tai
            // thi khong tao lai shapefile.
            if (Native_CacLopLogoConTonTai())
            {
                nativeDaVeLopPhu169 = true;

                Native_AnLop51CuNeuDungNguon();

                return;
            }

            bool tuKhoa = false;

            try
            {
                // Tim anh goc truoc khi thao tac map.
                Logo_TimAnhGoc();

                if (banDoMapWinGIS.IsLocked ==
                    tkLockMode.lmUnlock)
                {
                    banDoMapWinGIS.LockWindow(
                        tkLockMode.lmLock);

                    tuKhoa = true;
                }

                // Ham xu ly logo trong partial FormMain
                // cua file LogoDonViQuanDoi.cs.
                nativeDaVeLopPhu169 =
                    Native_DongBoLogoTatCa();

                if (!nativeDaVeLopPhu169)
                {
                    statusGeneral.Text =
                        "● Chưa tạo đủ lớp ký hiệu HQTCSDL. " +
                        "Kiểm tra nhật ký hoặc tọa độ.";

                    return;
                }

                int bchqs =
                    nativeDanhMucBchqsTraCuu.Count;

                int hoSo =
                    nativeHoSoGisHqtcTraCuu.Count;

                // ------------------------------------------------
                // 16.1. XAC DINH PHAM VI TOAN BO CAC DIEM
                // ------------------------------------------------

                if (!nativeDaCanKhung169)
                {
                    double minX = double.MaxValue;
                    double minY = double.MaxValue;

                    double maxX = double.MinValue;
                    double maxY = double.MinValue;

                    int soCoViTri = 0;

                    // --------------------------------------------
                    // BAN CHQS PHUONG/XA
                    // --------------------------------------------

                    foreach (
                        DanhMucBanChqsCapXa2026 d
                        in nativeDanhMucBchqsTraCuu)
                    {
                        if (d == null ||
                            !d.CoDiemDaiDien ||
                            !d.KinhDoDaiDien.HasValue ||
                            !d.ViDoDaiDien.HasValue)
                        {
                            continue;
                        }

                        double x;
                        double y;

                        if (!Native_Wgs84SangToaDoBanDo(
                            d.KinhDoDaiDien.Value,
                            d.ViDoDaiDien.Value,
                            out x,
                            out y))
                        {
                            continue;
                        }

                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);

                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);

                        soCoViTri++;
                    }

                    // --------------------------------------------
                    // HO SO GIS CAC CAP
                    // --------------------------------------------

                    foreach (
                        HoSoGisHqtc d
                        in nativeHoSoGisHqtcTraCuu)
                    {
                        if (d == null ||
                            !d.CoDiemBanDo ||
                            !d.KinhDo.HasValue ||
                            !d.ViDo.HasValue)
                        {
                            continue;
                        }

                        double x;
                        double y;

                        if (!Native_Wgs84SangToaDoBanDo(
                            d.KinhDo.Value,
                            d.ViDo.Value,
                            out x,
                            out y))
                        {
                            continue;
                        }

                        minX = Math.Min(minX, x);
                        maxX = Math.Max(maxX, x);

                        minY = Math.Min(minY, y);
                        maxY = Math.Max(maxY, y);

                        soCoViTri++;
                    }

                    // --------------------------------------------
                    // 16.2. CAN KHUNG TOAN BO DIEM
                    // --------------------------------------------

                    if (soCoViTri ==
                        bchqs + hoSo &&
                        minX < double.MaxValue)
                    {
                        double dx =
                            Math.Max(
                                2000d,
                                (maxX - minX) * 0.08d);

                        double dy =
                            Math.Max(
                                2000d,
                                (maxY - minY) * 0.08d);

                        Extents tatCa = new Extents();

                        tatCa.SetBounds(
                            minX - dx,
                            minY - dy,
                            0d,
                            maxX + dx,
                            maxY + dy,
                            0d);

                        banDoMapWinGIS.Extents = tatCa;

                        nativeDaCanKhung169 = true;
                    }
                }

                // ------------------------------------------------
                // 16.3. AN LOP CU
                // ------------------------------------------------

                Native_AnLop51CuNeuDungNguon();

                // ------------------------------------------------
                // 16.4. THONG BAO
                // ------------------------------------------------

                statusGeneral.Text =
                    "● HQTCSDL: " +
                    bchqs +
                    " Ban CHQS phường/xã + " +
                    hoSo +
                    " đơn vị các cấp; ký hiệu theo loại đơn vị " +
                    "(mờ = điểm đại diện, chưa phải trụ sở).";

                GhiNhatKyBanDo(
                    "HQTCSDL_LOGO_DONE | BCHQS=" +
                    bchqs +
                    " | HOSO=" +
                    hoSo);
            }
            catch (Exception ex)
            {
                nativeDaVeLopPhu169 = false;

                GhiNhatKyBanDo(
                    "HQTCSDL_LOGO_MAP_FAILED | " +
                    ex);

                statusGeneral.Text =
                    "● Không thể vẽ logo HQTCSDL: " +
                    ex.Message;
            }
            finally
            {
                if (tuKhoa &&
                    banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.LockWindow(
                        tkLockMode.lmUnlock);
                }

                if (banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.Redraw();
                }
            }
        }

        // ========================================================
        // 17. KIEM TRA DIEM TRONG KHUNG
        // ========================================================

        private static bool Native_TrongKhung(
            Extents ex,
            double x,
            double y)
        {
            return
                ex != null &&
                x >= ex.xMin &&
                x <= ex.xMax &&
                y >= ex.yMin &&
                y <= ex.yMax;
        }

        // ========================================================
        // 18. CHUYEN MAU COLOR SANG OLE
        // ========================================================

        private static uint MauOleDaiDien(Color c)
        {
            return unchecked(
                (uint)ColorTranslator.ToOle(c));
        }

        // ========================================================
        // 19. KIEM TRA CO CAN NAP LAI TRANG
        // ========================================================

        private bool Native_CanNapTrang(
            Control control)
        {
            if (control == null ||
                control.IsDisposed)
            {
                return false;
            }

            int daNap;

            return
                !nativePhienBanTrang.TryGetValue(
                    control,
                    out daNap)
                ||
                daNap != nativePhienBanDuLieu;
        }

        // ========================================================
        // 20. DANH DAU PHIEN BAN DA NAP
        // ========================================================

        private void Native_DanhDauTrangDaNap(
            Control control)
        {
            if (control != null &&
                !control.IsDisposed)
            {
                nativePhienBanTrang[control] =
                    nativePhienBanDuLieu;
            }
        }

        // ========================================================
        // 21. DANH DAU CACHE CAN TAI LAI
        // ========================================================

        private void Native_DanhDauDuLieuCanTaiLai()
        {
            headerBoNho = null;

            headerLanTai = DateTime.MinValue;

            // Ho so BLL.
            nativePhaiTaiLaiHoSo = true;

            nativeBoNhoHoSo = null;

            nativeLanTaiHoSo = DateTime.MinValue;

            // Danh muc Ban CHQS.
            nativePhaiTaiLaiDanhMucBchqs = true;

            nativeDanhMucBchqsTraCuu = null;

            nativeLanTaiDanhMucBchqs =
                DateTime.MinValue;

            nativeLoiTaiDanhMucBchqs = null;

            // Ho so GIS cac cap.
            nativePhaiTaiLaiHoSoHqtc = true;

            nativeHoSoGisHqtcTraCuu = null;

            nativeLanTaiHoSoHqtc =
                DateTime.MinValue;

            nativeLoiTaiHoSoHqtc = null;

            nativeSoHoSoHqtcDoc = 0;

            // Ban do.
            nativeDaVeLopPhu169 = false;

            nativeDaCanKhung169 = false;

            // Xoa logo tam, khong xoa du lieu database.
            Native_XoaToanBoLogo();
        }

        // ========================================================
        // 22. TAI MOI TAT CA PHAN HE
        // ========================================================

        private void Native_TaiMoiDuLieuPhanHe()
        {
            Native_DanhDauDuLieuCanTaiLai();

            TaiLaiDuLieuPhanHe();
        }

        // ========================================================
        // 23. CAP NHAT TONG QUAN
        // ========================================================

        private void CapNhatTongQuan()
        {
            GiaoDienTongQuan tongQuan =
                TimManHinh<GiaoDienTongQuan>(
                    "Tổng quan");

            if (tongQuan != null)
            {
                tongQuan.CapNhatTrangThaiPostGIS(
                    daKetNoiPostGIS);
            }
        }
    }
}
