
using System;

namespace ManagerGISAcademy.DTO
{
    /// <summary>
    /// Cấu hình đầu vào cho chức năng phân tích vùng đệm đơn vị.
    /// DTO chỉ lưu dữ liệu; việc kiểm tra tham số và phân quyền
    /// do SpatialAnalysisBLL thực hiện, truy vấn PostGIS do DAL thực hiện.
    /// Tương thích C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class CauHinhVungDem
    {
        /// <summary>
        /// Mã đơn vị làm tâm vùng đệm; phải tồn tại và được phép xem.
        /// </summary>
        public string MaDonViTam { get; set; }

        /// <summary>
        /// Bán kính vùng đệm tính bằng mét (không phải km hay độ).
        /// SpatialAnalysisBLL hiện giới hạn lớn hơn 0 và tối đa 35.000 mét.
        /// </summary>
        public double BanKinhMet { get; set; }

        /// <summary>
        /// Số bản ghi tối đa trả về khi tìm đơn vị trong bán kính.
        /// SpatialAnalysisBLL hiện chấp nhận từ 1 đến 500.
        /// </summary>
        public int GioiHanKetQua { get; set; }

        /// <summary>
        /// Tên hiển thị để gắn cho lớp kết quả trên giao diện sau này.
        /// Không phải tên bảng hoặc tên lớp trong PostGIS.
        /// </summary>
        public string TenLopKetQua { get; set; }

        /// <summary>
        /// Tùy chọn hiển thị hình học vùng đệm trên bản đồ.
        /// Phần hiển thị MapWinGIS sẽ sử dụng khi được tích hợp.
        /// </summary>
        public bool HienThiVungDem { get; set; }

        /// <summary>
        /// Tùy chọn hiển thị danh sách đơn vị trong vùng đệm.
        /// Không thay đổi phạm vi dữ liệu mà DAL/BLL được phép trả về.
        /// </summary>
        public bool HienThiDanhSachDonVi { get; set; }

        public CauHinhVungDem()
        {
            MaDonViTam = string.Empty;
            BanKinhMet = 5000d;
            GioiHanKetQua = 200;
            TenLopKetQua = "Vùng đệm đơn vị";
            HienThiVungDem = true;
            HienThiDanhSachDonVi = true;
        }

        /// <summary>
        /// Tạo cấu hình cho một mã đơn vị và bán kính cụ thể.
        /// Dữ liệu đầu vào sẽ được BLL kiểm tra trước khi truy vấn.
        /// </summary>
        public CauHinhVungDem(
            string maDonViTam,
            double banKinhMet,
            int gioiHanKetQua = 200)
            : this()
        {
            MaDonViTam = maDonViTam;
            BanKinhMet = banKinhMet;
            GioiHanKetQua = gioiHanKetQua;
        }
    }
}
