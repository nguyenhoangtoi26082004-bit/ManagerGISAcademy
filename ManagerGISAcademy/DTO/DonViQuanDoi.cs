
using System;
using System.Collections.Generic;

namespace ManagerGISAcademy.DTO
{
    /// <summary>
    /// Ho so don vi quan doi dung chung GUI, BLL, DAL.
    /// Tuong thich C# 7.3 / .NET Framework 4.8.
    ///
    /// Giu cac thuoc tinh cu de bao dam tuong thich.
    /// Bo sung thong tin dia ban phuong/xa va nguon du lieu.
    ///
    /// Cac thuoc tinh bo sung khong mac dinh la cot CSDL.
    /// </summary>
    public class DonViQuanDoi
    {
        // =============================================
        // 1. THONG TIN HO SO GOC
        // =============================================

        public string MaDonVi { get; set; }

        public string TenDonVi { get; set; }

        public string TenVietTat { get; set; }

        public string LoaiDonVi { get; set; }

        public string QuanBinhChung { get; set; }

        public string DonViCapTren { get; set; }

        // Truong du lieu ke thua.
        // Khong tu dong ghi de khi chua co anh xa.
        public string QuanHuyen { get; set; }

        public string DiaChi { get; set; }

        // =============================================
        // 2. DIA BAN HANH CHINH MOI
        // =============================================

        /// <summary>
        /// Ten phuong/xa hien hanh da duoc doi soat.
        /// </summary>
        public string PhuongXa { get; set; }

        /// <summary>
        /// Ma dia ban phuong/xa.
        /// </summary>
        public string MaPhuongXa { get; set; }

        /// <summary>
        /// Ma don vi hanh chinh cua danh muc Ban CHQS.
        /// </summary>
        public string MaDvhc { get; set; }

        /// <summary>
        /// Cho biet phuong/xa da duoc xac minh.
        /// </summary>
        public bool DaXacMinhDiaBan { get; set; }

        /// <summary>
        /// Phan biet danh muc BCHQS chi doc voi ho so chinh.
        /// </summary>
        public bool LaDanhMucBanChqs { get; set; }

        /// <summary>
        /// Ma nguon du lieu, do DAL/BLL gan.
        /// </summary>
        public string NguonDuLieu { get; set; }

        /// <summary>
        /// Dia ban dung de hien thi tren giao dien moi.
        /// Khong tu chuyen ten quan/huyen thanh phuong/xa.
        /// </summary>
        public string DiaBanHienThi
        {
            get
            {
                if (DaXacMinhDiaBan &&
                    !string.IsNullOrWhiteSpace(PhuongXa))
                {
                    return PhuongXa.Trim();
                }

                return "Chưa xác minh phường/xã";
            }
        }

        // =============================================
        // 3. TOA DO KHONG GIAN
        // =============================================

        // WGS84 / EPSG:4326
        public double KinhDo { get; set; }

        public double ViDo { get; set; }

        // Ten cot ke thua, gia tri phang do DAL xu ly.
        public double XVn2000 { get; set; }

        public double YVn2000 { get; set; }

        // =============================================
        // 4. THONG TIN NGHIEP VU
        // =============================================

        public string ChiHuyTruong { get; set; }

        public string ChinhUy { get; set; }

        public string SoDienThoai { get; set; }

        public string TrangThaiSSCD { get; set; }

        public string DoMat { get; set; }

        public string[] NhiemVuChinh { get; set; }

        public string[] VuKhiTrangBi { get; set; }

        public string GhiChu { get; set; }

        // =============================================
        // 5. HINH HOC VA CANH BAO
        // =============================================

        public string Wkt { get; set; }

        /// <summary>
        /// Phien ban ban ghi (cot he thong xmin cua PostgreSQL).
        /// Dung cho khoa lac quan: UPDATE chi thanh cong khi
        /// ban ghi chua bi nguoi khac sua tu luc mo ho so.
        /// Null = khong kiem tra (ho so moi hoac nguon khac).
        /// </summary>
        public string PhienBan { get; set; }

        public List<string> CanhBaoKhongGian
        {
            get;
            set;
        }

        // =============================================
        // 6. KHOI TAO
        // =============================================

        public DonViQuanDoi()
        {
            NhiemVuChinh = new string[0];

            VuKhiTrangBi = new string[0];

            CanhBaoKhongGian =
                new List<string>();

            LaDanhMucBanChqs = false;

            DaXacMinhDiaBan = false;

            NguonDuLieu = "HO_SO_CHINH";
        }
    }
}
