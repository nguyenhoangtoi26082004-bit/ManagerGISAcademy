
using System;

namespace ManagerGISAcademy.DTO
{
    /// <summary>
    /// Thông tin địa bàn quận/huyện dùng giữa GUI, BLL và DAL.
    ///
    /// Trong CSDL hiện tại, tên địa bàn lấy từ cột quan_huyen của
    /// public.don_vi_quan_doi; chưa có bảng ranh giới quan_huyen_hanoi.
    /// Vì vậy, thông tin mã hành chính và đa giác có thể chưa được cung cấp.
    ///
    /// DTO không truy vấn CSDL, không thực hiện phân quyền hay phép toán GIS.
    /// Tương thích C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public sealed class QuanHuyen
    {
        /// <summary>
        /// Mã hành chính chính thức nếu có nguồn dữ liệu địa giới đáng tin cậy.
        /// DAL hiện tại không tự suy ra mã từ tên.
        /// </summary>
        public string MaQuanHuyen { get; set; }

        /// <summary>Tên quận/huyện, lấy từ dữ liệu đã lưu.</summary>
        public string TenQuanHuyen { get; set; }

        /// <summary>
        /// Tên tỉnh/thành phố khi có dữ liệu phân vùng hành chính riêng.
        /// Chưa được xác định từ bảng don_vi_quan_doi hiện tại.
        /// </summary>
        public string TinhThanhPho { get; set; }

        /// <summary>
        /// Số bản ghi trong địa bàn mà tài khoản CSDL được phép truy cập.
        /// Không phải tổng số thực tế nếu chính sách quyền giới hạn dữ liệu.
        /// </summary>
        public long SoLuongDonVi { get; set; }

        /// <summary>
        /// WKT của ranh giới Polygon/MultiPolygon thuộc EPSG:4326 (nếu có).
        /// Bản DAL hiện tại chưa tải đa giác vì CSDL chưa tạo bảng địa giới.
        /// </summary>
        public string WktRanhGioi4326 { get; set; }

        /// <summary>SRID của hình học ranh giới, nếu có.</summary>
        public int? SridRanhGioi { get; set; }

        /// <summary>Diện tích (km2), nếu đã được tính từ nguồn phù hợp.</summary>
        public double? DienTichKm2 { get; set; }

        /// <summary>Đã có ranh giới mang đúng hệ quy chiếu hay chưa.</summary>
        public bool CoRanhGioi4326
        {
            get
            {
                return !string.IsNullOrWhiteSpace(WktRanhGioi4326)
                    && SridRanhGioi.HasValue
                    && SridRanhGioi.Value == 4326;
            }
        }
    }
}
