using System;

namespace ManagerGISAcademy.DTO
{
    /// <summary>
    /// Một dòng của bảng public.nhat_ky_don_vi_quan_doi
    /// (do trigger trg_nhat_ky_don_vi ghi; ứng dụng chỉ đọc).
    /// </summary>
    public sealed class NhatKyThayDoi
    {
        public long Id { get; set; }

        /// <summary>I = Thêm, U = Sửa, D = Xóa.</summary>
        public string ThaoTac { get; set; }

        public string MaDonVi { get; set; }

        /// <summary>Tên đơn vị (lấy từ bản mới, hoặc bản cũ nếu đã xóa).</summary>
        public string TenDonVi { get; set; }

        public string TaiKhoanDb { get; set; }

        public string UngDung { get; set; }

        public DateTime ThoiDiem { get; set; }

        /// <summary>Tên cột CSDL có giá trị khác nhau giữa bản cũ và mới.</summary>
        public string[] TruongThayDoi { get; set; }

        public string TenThaoTac
        {
            get
            {
                switch ((ThaoTac ?? "").Trim())
                {
                    case "I": return "Thêm";
                    case "U": return "Sửa";
                    case "D": return "Xóa";
                    default: return ThaoTac;
                }
            }
        }

        public NhatKyThayDoi()
        {
            TruongThayDoi = new string[0];
        }
    }

    /// <summary>Giá trị cũ / mới của một trường trong một lần thay đổi.</summary>
    public sealed class ChiTietThayDoi
    {
        public string TenCot { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriMoi { get; set; }
    }
}
