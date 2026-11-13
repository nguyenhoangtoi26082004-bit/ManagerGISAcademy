using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using ManagerGISAcademy.GUI.Views;
using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private void MoThemDonVi()
        {
            if (!CoQuyen(ThaoTacDonViQuanDoi.Them))
            {
                ThongBaoKhongCoQuyen("thêm đơn vị");
                return;
            }

            ChonPhanHe(
                btnNavManage,
                "Quản lý dữ liệu");

            Native_MoTrangNhapDonVi(null, false);
        }

        private void MoSuaDonVi(string maDonVi)
        {
            if (!CoQuyen(
                ThaoTacDonViQuanDoi.Sua,
                maDonVi))
            {
                ThongBaoKhongCoQuyen("sửa đơn vị");
                return;
            }

            try
            {
                DonViQuanDoi dv =
                    nghiepVuDonVi.LayDonViTheoMa(maDonVi);

                if (dv == null)
                    throw new InvalidOperationException(
                        "Không tìm thấy hồ sơ cần sửa.");

                ChonPhanHe(
                    btnNavManage,
                    "Quản lý dữ liệu");

                Native_MoTrangNhapDonVi(dv, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không mở được hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void Native_HienTrangHoSo(Control trang)
        {
            if (trang == null)
                return;

            Native_DongTrangHoSo(false);

            nativeManHinhHoSoDangMo = trang;

            pnlPhanHeHost.SuspendLayout();

            try
            {
                pnlPhanHeHost.Controls.Clear();

                trang.Dock = DockStyle.Fill;

                pnlPhanHeHost.Controls.Add(trang);

                trang.Show();
            }
            finally
            {
                pnlPhanHeHost.ResumeLayout(true);
            }
        }

        private void Native_DongTrangHoSo(
            bool hienLaiQuanLy)
        {
            Control cu = nativeManHinhHoSoDangMo;

            nativeManHinhHoSoDangMo = null;

            if (cu != null)
            {
                if (pnlPhanHeHost.Controls.Contains(cu))
                    pnlPhanHeHost.Controls.Remove(cu);

                if (!cu.IsDisposed)
                    cu.Dispose();
            }

            if (hienLaiQuanLy &&
                !IsDisposed &&
                !Disposing &&
                phanHeHienTai == "Quản lý dữ liệu")
            {
                HienThiNoiDung("Quản lý dữ liệu");
            }
        }

        // ==========================================
        // MO TRANG NHAP VA KET NOI CHON TOA DO
        // ==========================================

        private void Native_MoTrangNhapDonVi(
            DonViQuanDoi banGoc,
            bool chinhSua)
        {
            UcNhapDonViNative trang =
                new UcNhapDonViNative();

            trang.NapDuLieu(banGoc, chinhSua);

            // Giu phien ban luc MO ho so (khong phai luc luu)
            // de phat hien nguoi khac sua xen giua.
            string phienBanLucMo =
                chinhSua && banGoc != null
                    ? banGoc.PhienBan
                    : null;

            trang.YeuCauHuy +=
                () => Native_DongTrangHoSo(true);

            trang.YeuCauLuu +=
                duLieu => Native_XuLyLuuDonVi(
                    duLieu, phienBanLucMo, trang);

            // Su kien moi cho chuc nang chon toa do tren ban do.
            trang.YeuCauChonViTri +=
                () => Native_BatChonViTriChoHoSo(trang);

            Native_HienTrangHoSo(trang);
        }

        // ==========================================
        // LUU HO SO QUA BLL / POSTGIS
        // ==========================================

        private void Native_XuLyLuuDonVi(
            HoSoNhapNative d,
            string phienBanLucMo,
            UcNhapDonViNative trang)
        {
            if (d == null)
                return;

            try
            {
                ThaoTacDonViQuanDoi thaoTac =
                    d.LaChinhSua
                        ? ThaoTacDonViQuanDoi.Sua
                        : ThaoTacDonViQuanDoi.Them;

                if (!CoQuyen(thaoTac, d.MaDonVi))
                    throw new UnauthorizedAccessException(
                        "Không có quyền lưu hồ sơ.");

                if (!d.KinhDo.HasValue ||
                    !d.ViDo.HasValue)
                    throw new ArgumentException(
                        "Chưa nhập đủ tọa độ WGS84.");

                // Bao ton cac thuoc tinh khong co tren form
                // khi cap nhat ho so da ton tai.
                DonViQuanDoi banGoc =
                    d.LaChinhSua
                        ? nghiepVuDonVi.LayDonViTheoMa(d.MaDonVi)
                        : null;

                if (d.LaChinhSua && banGoc == null)
                    throw new InvalidOperationException(
                        "Không tìm thấy hồ sơ gốc.");

                DonViQuanDoi dv =
                    banGoc ?? new DonViQuanDoi();

                dv.MaDonVi = d.MaDonVi;
                dv.TenDonVi = d.TenDonVi;
                dv.TenVietTat = d.TenVietTat;
                dv.LoaiDonVi = d.LoaiDonVi;
                dv.QuanBinhChung = d.QuanBinhChung;
                dv.DonViCapTren = d.DonViCapTren;
                dv.QuanHuyen = d.QuanHuyen;
                dv.DiaChi = d.DiaChi;
                dv.KinhDo = d.KinhDo.Value;
                dv.ViDo = d.ViDo.Value;
                dv.ChiHuyTruong = d.ChiHuyTruong;
                dv.ChinhUy = d.ChinhUy;
                dv.SoDienThoai = d.SoDienThoai;
                dv.TrangThaiSSCD = d.TrangThaiSSCD;

                dv.NhiemVuChinh =
                    TachDanhSach(d.NhiemVuChinh);

                dv.VuKhiTrangBi =
                    TachDanhSach(d.VuKhiTrangBi);

                // Ban goc vua tai lai mang phien ban MOI NHAT;
                // thay bang phien ban luc mo de BLL/DAL so sanh.
                if (d.LaChinhSua)
                    dv.PhienBan = phienBanLucMo;

                bool thanhCong =
                    d.LaChinhSua
                        ? nghiepVuDonVi.SuaDonVi(dv)
                        : nghiepVuDonVi.ThemDonVi(dv);

                if (!thanhCong)
                    throw new InvalidOperationException(
                        "CSDL chưa xác nhận lưu thành công.");

                Native_DanhDauDuLieuCanTaiLai();

                // Khong Dispose UserControl ngay trong callback.
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (IsDisposed || Disposing)
                            return;

                        Native_DongTrangHoSo(true);

                        Native_DongBoLopDiemSauGhi();

                        statusGeneral.Text =
                            "● Đã lưu hồ sơ " + d.MaDonVi;

                        // Vai tro Bien tap: CSDL chi xep hang yeu cau.
                        QuanTri_ThongBaoNeuChoDuyet(
                            (d.LaChinhSua ? "Sửa" : "Thêm") + " hồ sơ " + d.MaDonVi);
                    }));
                }
            }
            catch (XungDotPhienBanException ex)
            {
                if (trang != null)
                    trang.BaoLoiLuu("Chưa lưu: hồ sơ đã bị người khác sửa.");

                DialogResult taiLai = MessageBox.Show(
                    this,
                    ex.Message +
                    "\n\nTải lại hồ sơ mới nhất ngay bây giờ?" +
                    "\n(Các thay đổi chưa lưu trên form sẽ mất.)",
                    "Hồ sơ đã thay đổi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button1);

                if (taiLai == DialogResult.Yes && IsHandleCreated)
                {
                    string ma = ex.MaDonVi;

                    // Khong thay trang ngay trong callback cua no.
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (!IsDisposed && !Disposing)
                            MoSuaDonVi(ma);
                    }));
                }
            }
            catch (Exception ex)
            {
                if (trang != null)
                    trang.BaoLoiLuu("Chưa lưu: " + ex.Message);

                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không lưu được hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private static string[] TachDanhSach(
            string noiDung)
        {
            return (noiDung ?? "")
                .Split(
                    new[] { ';', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => x.Length > 0)
                .ToArray();
        }

        // ==========================================
        // XOA DON VI
        // ==========================================

        private void XoaDonVi(string maDonVi)
        {
            if (!CoQuyen(
                ThaoTacDonViQuanDoi.Xoa,
                maDonVi))
            {
                ThongBaoKhongCoQuyen("xóa đơn vị");
                return;
            }

            if (string.IsNullOrWhiteSpace(maDonVi))
                return;

            DialogResult xacNhan =
                MessageBox.Show(
                    this,
                    "Bạn có chắc muốn xóa hồ sơ có mã:\n" +
                    maDonVi +
                    "\n\nThao tác này có thể không hoàn tác được.",
                    "Xác nhận xóa đơn vị",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

            if (xacNhan != DialogResult.Yes)
                return;

            try
            {
                if (!nghiepVuDonVi.XoaDonVi(maDonVi))
                {
                    throw new InvalidOperationException(
                        "CSDL không xác nhận xóa thành công.");
                }

                Native_TaiMoiDuLieuPhanHe();

                Native_DongBoLopDiemSauGhi();

                statusGeneral.Text =
                    "● Đã xóa hồ sơ " + maDonVi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Lỗi xóa hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // HIEN THI CHI TIET DON VI
        // ==========================================

        private void MoXemChiTiet(string maDonVi)
        {
            if (!CoQuyen(
                ThaoTacDonViQuanDoi.Xem,
                maDonVi))
            {
                ThongBaoKhongCoQuyen("xem hồ sơ");
                return;
            }

            try
            {
                DonViQuanDoi dv =
                    nghiepVuDonVi.LayDonViTheoMa(maDonVi);

                if (dv == null)
                    throw new InvalidOperationException(
                        "Không tìm thấy hồ sơ.");

                ChonPhanHe(
                    btnNavManage,
                    "Quản lý dữ liệu");

                UcChiTietDonViNative v =
                    new UcChiTietDonViNative();

                v.NapDuLieu(dv);

                v.YeuCauSua +=
                    ma => MoSuaDonVi(ma);

                v.YeuCauDinhVi +=
                    ma => DinhViDonVi(ma);

                v.YeuCauDong +=
                    () => Native_DongTrangHoSo(true);

                Native_HienTrangHoSo(v);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Lỗi xem hồ sơ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // DINH VI HO SO TREN BAN DO
        // ==========================================

        private void DinhViDonVi(string maDonVi)
        {
            if (!CoQuyen(
                ThaoTacDonViQuanDoi.Xem,
                maDonVi))
            {
                ThongBaoKhongCoQuyen("định vị");
                return;
            }

            try
            {
                DonViQuanDoi dv =
                    nghiepVuDonVi.LayDonViTheoMa(maDonVi);

                if (dv == null ||
                    !NghiepVuDonVi.CoToaDoWgs84(dv))
                {
                    throw new InvalidOperationException(
                        "Hồ sơ chưa có tọa độ WGS84 hợp lệ.");
                }

                if (banDoMapWinGIS == null)
                {
                    throw new InvalidOperationException(
                        "MapWinGIS chưa được khởi tạo.");
                }

                // Tu phan he khac: bam nut Ban do THAT de chay du cac
                // su kien da gan vao btnNavMap.Click (ve lai lop phu
                // HQTCSDL, dong bo chu giai, nap lop diem). Chi goi
                // ChonPhanHe se bo sot chung va mat cac lop HQTCSDL.
                bool daOBanDo = string.Equals(
                    phanHeHienTai, "Bản đồ GIS", StringComparison.Ordinal);

                // Khi ve lai lop phu HQTCSDL (timer 600 ms) khong
                // tu thu ve toan canh, de giu khung nhin dinh vi.
                nativeDaCanKhung169 = true;

                if (!daOBanDo && btnNavMap != null && btnNavMap.CanSelect)
                    btnNavMap.PerformClick();
                else if (!daOBanDo)
                    ChonPhanHe(btnNavMap, "Bản đồ GIS");

                double x = 0d;
                double y = 0d;

                if (!banDoMapWinGIS.DegreesToProj(
                    dv.KinhDo,
                    dv.ViDo,
                    ref x,
                    ref y))
                {
                    throw new InvalidOperationException(
                        "Không chuyển được tọa độ WGS84 sang hệ quy chiếu bản đồ.");
                }

                MapWinGIS.Extents phamVi =
                    new MapWinGIS.Extents();

                phamVi.SetBounds(
                    x - 1000d,
                    y - 1000d,
                    0d,
                    x + 1000d,
                    y + 1000d,
                    0d);

                banDoMapWinGIS.Extents = phamVi;

                // Bat lop ho so quan ly va to sang diem vua dinh vi.
                Native_DanhDauDiemDonVi(dv.MaDonVi);

                // Cac su kien cua nut Ban do co phan chay sau
                // (BeginInvoke). Lam lai mot lan sau chung de giu
                // khung nhin va diem duoc to sang.
                if (!daOBanDo && IsHandleCreated)
                {
                    string ma = dv.MaDonVi;

                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (IsDisposed || Disposing ||
                            banDoMapWinGIS == null ||
                            banDoMapWinGIS.IsDisposed)
                        {
                            return;
                        }

                        banDoMapWinGIS.Extents = phamVi;
                        Native_DanhDauDiemDonVi(ma);
                    }));
                }

                CapNhatToaDo(
                    dv.KinhDo,
                    dv.ViDo);

                statusGeneral.Text =
                    "● Đã định vị hồ sơ " + dv.MaDonVi;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Không thể định vị",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ==========================================
        // NHAT KY THAY DOI
        // ==========================================

        private void MoNhatKyThayDoi(string maDonVi)
        {
            if (!CoQuyen(ThaoTacDonViQuanDoi.Xem, maDonVi))
            {
                ThongBaoKhongCoQuyen("xem nhật ký thay đổi");
                return;
            }

            using (NhatKyThayDoiForm f =
                new NhatKyThayDoiForm(maDonVi))
            {
                f.ShowDialog(this);
            }
        }

        // ==========================================
        // DONG BO LOP DIEM VOI CSDL SAU KHI GHI
        // ==========================================

        /// <summary>
        /// Nap lai lop diem ho so quan ly sau Them/Sua/Xoa de ban do
        /// khop voi luoi (so diem, vi tri). Loi ban do khong lam hong
        /// thao tac ghi da thanh cong.
        /// </summary>
        private void Native_DongBoLopDiemSauGhi()
        {
            try
            {
                NapLaiLopDiemPostGIS();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("SYNC_POINT_LAYER_AFTER_WRITE | " + ex);
            }
        }

        /// <summary>
        /// Bat lop ho so quan ly (dong bo o danh sach lop) va chon
        /// dung diem cua don vi de MapWinGIS to mau lua chon.
        /// </summary>
        private void Native_DanhDauDiemDonVi(string maDonVi)
        {
            try
            {
                if (banDoMapWinGIS == null ||
                    banDoMapWinGIS.IsDisposed ||
                    ketNoiMapWinGIS == null ||
                    handleLopDiemPostGIS < 0 ||
                    !ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS))
                {
                    return;
                }

                if (!ketNoiMapWinGIS.LayHienThiLop(handleLopDiemPostGIS))
                    ChuGiai_DoiHienThiLop(handleLopDiemPostGIS, true);

                MapWinGIS.Shapefile lop =
                    banDoMapWinGIS.get_Shapefile(handleLopDiemPostGIS);

                if (lop == null)
                    return;

                int cotMa = lop.get_FieldIndexByName("MA_DON_VI");

                if (cotMa < 0)
                    return;

                lop.SelectNone();

                for (int i = 0; i < lop.NumShapes; i++)
                {
                    string ma = (Convert.ToString(
                        lop.get_CellValue(cotMa, i)) ?? "").Trim();

                    if (string.Equals(ma, maDonVi,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        lop.set_ShapeSelected(i, true);
                        break;
                    }
                }

                banDoMapWinGIS.Redraw();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("HIGHLIGHT_UNIT_POINT | " + ex);
            }
        }

        private void ThongBaoKhongCoQuyen(
            string hanhDong)
        {
            MessageBox.Show(
                this,
                "Tài khoản chưa được cấp quyền " +
                hanhDong + ".",
                "Từ chối truy cập",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}