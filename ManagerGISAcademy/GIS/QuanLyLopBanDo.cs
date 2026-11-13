
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagerGISAcademy.GIS
{
    public enum KieuLopBanDo
    {
        ChuaXacDinh = 0,
        Vector = 1,
        Raster = 2
    }

    public enum LoaiThayDoiLop
    {
        Them = 0,
        Xoa = 1,
        HienThi = 2,
        DoiTen = 3,
        DoiThuTu = 4,
        DongBo = 5
    }

    /// <summary>
    /// Hop dong thao tac ban do; giu nguyen de khong gay loi cho cac file cu.
    /// </summary>
    public interface IGiaoTiepLopBanDo
    {
        int ThemLopTuTep(string duongDanDayDu, bool hienThi);
        bool CoLop(int layerHandle);
        bool LayHienThiLop(int layerHandle);
        void DatHienThiLop(int layerHandle, bool hienThi);
        void DatTenLop(int layerHandle, string tenLop);
        void XoaLop(int layerHandle);
        void DuaLopLenMotBac(int layerHandle);
        void DuaLopXuongMotBac(int layerHandle);
        void PhongDenLop(int layerHandle);
        void VeLaiBanDo();
    }

    /// <summary>
    /// Giao dien MO RONG tuy chon. Khong thay doi interface cu.
    /// Dung de dong bo thu tu voi cac lop nap truc tiep tu FormMain.
    /// </summary>
    public interface IGiaoTiepThuTuLopBanDo
    {
        int LayViTriLop(int layerHandle);
        IList<int> LayDanhSachLayerHandle();
    }

    public sealed class ThongTinLopBanDo
    {
        public int LayerHandle { get; private set; }
        public string TenLop { get; private set; }
        public string DuongDanNguon { get; private set; }
        public string NhomLop { get; private set; }
        public KieuLopBanDo KieuLop { get; private set; }
        public bool HienThi { get; private set; }
        public int ThuTuHienThi { get; private set; }

        internal ThongTinLopBanDo(int handle, string ten, string duongDan,
            string nhom, KieuLopBanDo kieu, bool hienThi, int thuTu)
        {
            LayerHandle = handle;
            TenLop = ten;
            DuongDanNguon = duongDan;
            NhomLop = nhom;
            KieuLop = kieu;
            HienThi = hienThi;
            ThuTuHienThi = thuTu;
        }

        internal void CapNhatHienThi(bool hienThi) { HienThi = hienThi; }
        internal void CapNhatTen(string ten) { TenLop = ten; }
        internal void CapNhatThuTu(int viTri) { ThuTuHienThi = viTri; }

        internal ThongTinLopBanDo SaoChep()
        {
            return new ThongTinLopBanDo(LayerHandle, TenLop,
                DuongDanNguon, NhomLop, KieuLop, HienThi, ThuTuHienThi);
        }
    }

    public sealed class SuKienThayDoiLopArgs : EventArgs
    {
        public LoaiThayDoiLop LoaiThayDoi { get; private set; }
        public ThongTinLopBanDo Lop { get; private set; }

        public SuKienThayDoiLopArgs(LoaiThayDoiLop loai,
            ThongTinLopBanDo lop)
        {
            if (lop == null) throw new ArgumentNullException("lop");
            LoaiThayDoi = loai;
            Lop = lop.SaoChep();
        }
    }

    /// <summary>
    /// Quan ly danh sach va trang thai cac lop GIS theo LayerHandle thuc.
    /// Khong nham LayerHandle voi vi tri lop; khong goi SQL/MapWinGIS truc tiep.
    /// Goi tren luong WinForms vi adapter MapWinGIS yeu cau luong UI.
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class QuanLyLopBanDo
    {
        private readonly List<ThongTinLopBanDo> danhSachLop =
            new List<ThongTinLopBanDo>();
        private IGiaoTiepLopBanDo boDieuKhien;

        public event EventHandler<SuKienThayDoiLopArgs> LopThayDoi;

        public QuanLyLopBanDo() { }

        public QuanLyLopBanDo(IGiaoTiepLopBanDo boDieuKhienBanDo)
        {
            GanBoDieuKhien(boDieuKhienBanDo);
        }

        public bool DaGanBoDieuKhien
        {
            get { return boDieuKhien != null; }
        }

        public int SoLop
        {
            get { return danhSachLop.Count; }
        }

        public int SoLopDangHienThi
        {
            get { return danhSachLop.Count(lop => lop.HienThi); }
        }

        public void GanBoDieuKhien(IGiaoTiepLopBanDo moi)
        {
            if (moi == null) throw new ArgumentNullException("moi");
            if (danhSachLop.Count > 0 &&
                !object.ReferenceEquals(boDieuKhien, moi))
                throw new InvalidOperationException(
                    "Hãy xóa các lớp đang quản lý trước khi đổi bản đồ.");
            boDieuKhien = moi;
        }

        public void ThaoBoDieuKhien()
        {
            if (danhSachLop.Count != 0)
                throw new InvalidOperationException(
                    "Còn lớp đang quản lý. Hãy xóa trước khi tháo bản đồ.");
            boDieuKhien = null;
        }

        public IList<ThongTinLopBanDo> LayDanhSachLop()
        {
            return danhSachLop.Select(lop => lop.SaoChep())
                .ToList().AsReadOnly();
        }

        public ThongTinLopBanDo LayThongTinLop(int layerHandle)
        {
            ThongTinLopBanDo lop = TimLop(layerHandle);
            return lop == null ? null : lop.SaoChep();
        }

        public bool CoQuanLyLop(int layerHandle)
        {
            return TimLop(layerHandle) != null;
        }

        public int ThemLopTuTep(string duongDan, string tenLop = null,
            bool hienThi = true, string nhomLop = null)
        {
            DamBaoDaGanBanDo();
            if (string.IsNullOrWhiteSpace(duongDan))
                throw new ArgumentException("Chưa chọn tệp bản đồ.", "duongDan");

            string tep = Path.GetFullPath(duongDan.Trim());
            if (!File.Exists(tep))
                throw new FileNotFoundException("Không tìm thấy tệp.", tep);

            if (danhSachLop.Any(x => x.DuongDanNguon != null &&
                string.Equals(x.DuongDanNguon, tep,
                StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Tệp này đã được nạp.");

            string ten = ChuanHoaTen(tenLop,
                Path.GetFileNameWithoutExtension(tep));
            int handle = boDieuKhien.ThemLopTuTep(tep, hienThi);
            if (handle < 0 || !boDieuKhien.CoLop(handle))
                throw new InvalidOperationException(
                    "Bộ kết nối không trả về lớp hợp lệ.");

            // Khong xoa handle neu do bo dieu khien tra ve bi trung:
            // co the day la lop da co tren AxMap.
            if (CoQuanLyLop(handle))
                throw new InvalidOperationException(
                    "LayerHandle đã thuộc một lớp đang quản lý.");

            try
            {
                boDieuKhien.DatTenLop(handle, ten);
            }
            catch
            {
                try { boDieuKhien.XoaLop(handle); }
                catch { /* Giu nguyen exception goc. */ }
                throw;
            }

            ThongTinLopBanDo lop = new ThongTinLopBanDo(
                handle, ten, tep, ChuanHoaNhom(nhomLop),
                NhanBietKieuLop(tep), hienThi, danhSachLop.Count);
            danhSachLop.Add(lop);
            SapXepTheoBanDo();
            VeLaiVaThongBao(LoaiThayDoiLop.Them, lop);
            return handle;
        }

        /// <summary>
        /// Dang ky lop da nap ben ngoai LayerManager (vi du tu FormMain).
        /// Sau khi dang ky, LayerManager co the xoa lop nay tren AxMap.
        /// </summary>
        public void DangKyLopDaCo(int layerHandle, string tenLop,
            KieuLopBanDo kieuLop = KieuLopBanDo.ChuaXacDinh,
            string nhomLop = null)
        {
            DamBaoDaGanBanDo();
            if (CoQuanLyLop(layerHandle))
                throw new InvalidOperationException("Lớp đã được đăng ký.");
            if (!boDieuKhien.CoLop(layerHandle))
                throw new KeyNotFoundException("LayerHandle không có trên bản đồ.");

            string ten = ChuanHoaTen(tenLop, "Lớp " + layerHandle);
            bool hienThi = boDieuKhien.LayHienThiLop(layerHandle);
            boDieuKhien.DatTenLop(layerHandle, ten);

            ThongTinLopBanDo lop = new ThongTinLopBanDo(
                layerHandle, ten, null, ChuanHoaNhom(nhomLop),
                kieuLop, hienThi, danhSachLop.Count);
            danhSachLop.Add(lop);
            SapXepTheoBanDo();
            VeLaiVaThongBao(LoaiThayDoiLop.Them, lop);
        }

        public bool DatHienThiLop(int layerHandle, bool hienThi)
        {
            DamBaoDaGanBanDo();
            ThongTinLopBanDo lop = LayLopBatBuoc(layerHandle);
            bool thucTe = boDieuKhien.LayHienThiLop(layerHandle);
            if (thucTe == hienThi && lop.HienThi == hienThi)
                return false;

            if (thucTe != hienThi)
                boDieuKhien.DatHienThiLop(layerHandle, hienThi);

            lop.CapNhatHienThi(hienThi);
            VeLaiVaThongBao(thucTe == hienThi
                ? LoaiThayDoiLop.DongBo : LoaiThayDoiLop.HienThi, lop);
            return true;
        }

        public bool BatTatLop(int layerHandle)
        {
            DamBaoDaGanBanDo();
            LayLopBatBuoc(layerHandle);
            return DatHienThiLop(layerHandle,
                !boDieuKhien.LayHienThiLop(layerHandle));
        }

        public bool DoiTenLop(int layerHandle, string tenLopMoi)
        {
            DamBaoDaGanBanDo();
            ThongTinLopBanDo lop = LayLopBatBuoc(layerHandle);
            string ten = ChuanHoaTen(tenLopMoi, null);
            if (string.Equals(lop.TenLop, ten, StringComparison.Ordinal))
                return false;

            boDieuKhien.DatTenLop(layerHandle, ten);
            lop.CapNhatTen(ten);
            VeLaiVaThongBao(LoaiThayDoiLop.DoiTen, lop);
            return true;
        }

        public bool DuaLopLenTren(int layerHandle)
        {
            DamBaoDaGanBanDo();
            ThongTinLopBanDo lop = LayLopBatBuoc(layerHandle);
            SapXepTheoBanDo();
            if (DaODauCuoi(layerHandle, true)) return false;

            boDieuKhien.DuaLopLenMotBac(layerHandle);
            CapNhatThuTuSauKhiDiChuyen(layerHandle, true);
            VeLaiVaThongBao(LoaiThayDoiLop.DoiThuTu, lop);
            return true;
        }

        public bool DuaLopXuongDuoi(int layerHandle)
        {
            DamBaoDaGanBanDo();
            ThongTinLopBanDo lop = LayLopBatBuoc(layerHandle);
            SapXepTheoBanDo();
            if (DaODauCuoi(layerHandle, false)) return false;

            boDieuKhien.DuaLopXuongMotBac(layerHandle);
            CapNhatThuTuSauKhiDiChuyen(layerHandle, false);
            VeLaiVaThongBao(LoaiThayDoiLop.DoiThuTu, lop);
            return true;
        }

        public void PhongDenLop(int layerHandle)
        {
            DamBaoDaGanBanDo();
            LayLopBatBuoc(layerHandle);
            boDieuKhien.PhongDenLop(layerHandle);
        }

        public void XoaLop(int layerHandle)
        {
            DamBaoDaGanBanDo();
            ThongTinLopBanDo lop = LayLopBatBuoc(layerHandle);
            ThongTinLopBanDo banChup = lop.SaoChep();

            if (boDieuKhien.CoLop(layerHandle))
                boDieuKhien.XoaLop(layerHandle);

            danhSachLop.Remove(lop);
            SapXepTheoBanDo();
            VeLaiVaThongBao(LoaiThayDoiLop.Xoa, banChup);
        }

        public void XoaTatCaLop()
        {
            DamBaoDaGanBanDo();
            while (danhSachLop.Count > 0)
                XoaLop(danhSachLop[danhSachLop.Count - 1].LayerHandle);
        }

        public void DongBoTrangThaiTuBanDo()
        {
            DamBaoDaGanBanDo();
            List<ThongTinLopBanDo> daXoa = new List<ThongTinLopBanDo>();
            List<ThongTinLopBanDo> daDoi = new List<ThongTinLopBanDo>();

            for (int i = danhSachLop.Count - 1; i >= 0; i--)
            {
                ThongTinLopBanDo lop = danhSachLop[i];
                if (!boDieuKhien.CoLop(lop.LayerHandle))
                {
                    daXoa.Add(lop.SaoChep());
                    danhSachLop.RemoveAt(i);
                    continue;
                }
                bool hienThi = boDieuKhien.LayHienThiLop(lop.LayerHandle);
                if (lop.HienThi != hienThi)
                {
                    lop.CapNhatHienThi(hienThi);
                    daDoi.Add(lop);
                }
            }

            // Co the co lop khac khong duoc dang ky. Dong bo thu tu thuc te.
            Dictionary<int, int> viTriCu = danhSachLop.ToDictionary(
                x => x.LayerHandle, x => x.ThuTuHienThi);
            SapXepTheoBanDo();

            foreach (ThongTinLopBanDo lop in daXoa)
                ThongBaoThayDoi(LoaiThayDoiLop.Xoa, lop);
            foreach (ThongTinLopBanDo lop in daDoi)
                ThongBaoThayDoi(LoaiThayDoiLop.DongBo, lop);
            foreach (ThongTinLopBanDo lop in danhSachLop)
            {
                if (viTriCu[lop.LayerHandle] != lop.ThuTuHienThi)
                    ThongBaoThayDoi(LoaiThayDoiLop.DoiThuTu, lop);
            }
        }

        public void VeLaiBanDo()
        {
            DamBaoDaGanBanDo();
            boDieuKhien.VeLaiBanDo();
        }

        public static KieuLopBanDo NhanBietKieuLop(string duongDan)
        {
            if (string.IsNullOrWhiteSpace(duongDan))
                return KieuLopBanDo.ChuaXacDinh;

            string ext = Path.GetExtension(duongDan).ToLowerInvariant();
            switch (ext)
            {
                case ".shp":
                case ".geojson":
                    return KieuLopBanDo.Vector;
                case ".tif":
                case ".tiff":
                case ".img":
                case ".asc":
                case ".vrt":
                    return KieuLopBanDo.Raster;
                default:
                    return KieuLopBanDo.ChuaXacDinh;
            }
        }

        private void DamBaoDaGanBanDo()
        {
            if (boDieuKhien == null)
                throw new InvalidOperationException(
                    "Chưa gắn MapWinGISHelper vào LayerManager.");
        }

        private ThongTinLopBanDo TimLop(int handle)
        {
            return danhSachLop.FirstOrDefault(x => x.LayerHandle == handle);
        }

        private ThongTinLopBanDo LayLopBatBuoc(int handle)
        {
            ThongTinLopBanDo lop = TimLop(handle);
            if (lop == null)
                throw new KeyNotFoundException(
                    "LayerHandle không thuộc LayerManager: " + handle);
            return lop;
        }

        private bool DaODauCuoi(int handle, bool duaLen)
        {
            IGiaoTiepThuTuLopBanDo thuTu =
                boDieuKhien as IGiaoTiepThuTuLopBanDo;
            if (thuTu != null)
            {
                int viTri = thuTu.LayViTriLop(handle);
                int tong = thuTu.LayDanhSachLayerHandle().Count;
                return duaLen ? viTri >= tong - 1 : viTri <= 0;
            }
            int i = danhSachLop.FindIndex(x => x.LayerHandle == handle);
            return duaLen ? i == danhSachLop.Count - 1 : i == 0;
        }

        private void CapNhatThuTuSauKhiDiChuyen(int handle, bool duaLen)
        {
            IGiaoTiepThuTuLopBanDo thuTu =
                boDieuKhien as IGiaoTiepThuTuLopBanDo;
            if (thuTu != null)
            {
                SapXepTheoBanDo();
                return;
            }

            int viTri = danhSachLop.FindIndex(x => x.LayerHandle == handle);
            ThongTinLopBanDo lop = danhSachLop[viTri];
            danhSachLop.RemoveAt(viTri);
            danhSachLop.Insert(duaLen ? viTri + 1 : viTri - 1, lop);
            for (int i = 0; i < danhSachLop.Count; i++)
                danhSachLop[i].CapNhatThuTu(i);
        }

        private void SapXepTheoBanDo()
        {
            IGiaoTiepThuTuLopBanDo thuTu =
                boDieuKhien as IGiaoTiepThuTuLopBanDo;
            if (thuTu == null)
            {
                for (int i = 0; i < danhSachLop.Count; i++)
                    danhSachLop[i].CapNhatThuTu(i);
                return;
            }

            // Chi goi sau khi lop da duoc tao, hoac sau khi lop bi xoa.
            foreach (ThongTinLopBanDo lop in danhSachLop)
                lop.CapNhatThuTu(thuTu.LayViTriLop(lop.LayerHandle));

            danhSachLop.Sort((a, b) =>
                a.ThuTuHienThi.CompareTo(b.ThuTuHienThi));
        }

        private static string ChuanHoaTen(string ten, string macDinh)
        {
            string ketQua = string.IsNullOrWhiteSpace(ten)
                ? (macDinh ?? string.Empty) : ten.Trim();
            if (ketQua.Length == 0)
                throw new ArgumentException("Tên lớp không được trống.", "ten");
            return ketQua;
        }

        private static string ChuanHoaNhom(string nhom)
        {
            return string.IsNullOrWhiteSpace(nhom)
                ? "Chưa phân nhóm" : nhom.Trim();
        }

        private void VeLaiVaThongBao(LoaiThayDoiLop loai,
            ThongTinLopBanDo lop)
        {
            try { boDieuKhien.VeLaiBanDo(); }
            finally { ThongBaoThayDoi(loai, lop); }
        }

        private void ThongBaoThayDoi(LoaiThayDoiLop loai,
            ThongTinLopBanDo lop)
        {
            EventHandler<SuKienThayDoiLopArgs> handler = LopThayDoi;
            if (handler != null)
                handler(this, new SuKienThayDoiLopArgs(loai, lop));
        }
    }
}
