
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ManagerGISAcademy.BLL;

using AdoConnection = System.Data.Common.DbConnection;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Truy xuất và kiểm tra chất lượng hình học
    /// của dữ liệu đơn vị quân đội trong PostGIS.
    ///
    /// Các chức năng:
    /// 1. Kiểm tra tọa độ và hình học.
    /// 2. Kiểm tra SRID.
    /// 3. Kiểm tra sai lệch giữa hai hệ tọa độ.
    /// 4. Tìm các cặp vị trí gần trùng.
    ///
    /// Không tự động sửa hoặc xóa dữ liệu.
    ///
    /// Tương thích:
    /// C# 7.3 - .NET Framework 4.8.
    /// PostgreSQL/PostGIS.
    /// </summary>
    public sealed class TruyXuatKiemTraTopology
        : ITruyXuatKiemTraTopology
    {
        private const int GioiHanDocToiDa = 5001;
        private const int ThoiGianTruyVanGiay = 45;
        private const int SridWgs84 = 4326;
        private const int SridBanDo = 32648;

        // =====================================================
        // 1. KIỂM TRA HÌNH HỌC VÀ TỌA ĐỘ
        // =====================================================

        public IList<ThongTinTopologyDonVi>
            LayThongTinHinhHoc(int gioiHan)
        {
            KiemTraGioiHan(gioiHan);

            const string sql = @"
                SELECT
                    u.ma_don_vi,
                    u.kinh_do,
                    u.vi_do,

                    (u.geom_4326 IS NOT NULL)
                        AS co_geom_4326,

                    (u.geom_32648 IS NOT NULL)
                        AS co_geom_32648,

                    CASE
                        WHEN u.geom_4326 IS NOT NULL
                        THEN ST_SRID(u.geom_4326)
                    END AS srid_4326,

                    CASE
                        WHEN u.geom_32648 IS NOT NULL
                        THEN ST_SRID(u.geom_32648)
                    END AS srid_32648,

                    CASE
                        WHEN u.geom_4326 IS NOT NULL
                        THEN
                            ST_GeometryType(u.geom_4326)
                            = 'ST_Point'
                    END AS la_diem_4326,

                    CASE
                        WHEN u.geom_32648 IS NOT NULL
                        THEN
                            ST_GeometryType(u.geom_32648)
                            = 'ST_Point'
                    END AS la_diem_32648,

                    CASE
                        WHEN u.geom_4326 IS NOT NULL
                        THEN ST_IsValid(u.geom_4326)
                    END AS hop_le_4326,

                    CASE
                        WHEN u.geom_32648 IS NOT NULL
                        THEN ST_IsValid(u.geom_32648)
                    END AS hop_le_32648,

                    CASE
                        WHEN u.kinh_do BETWEEN -180 AND 180
                         AND u.vi_do BETWEEN -90 AND 90
                         AND u.geom_32648 IS NOT NULL
                         AND ST_SRID(u.geom_32648) = 32648
                         AND ST_GeometryType(u.geom_32648)
                             = 'ST_Point'
                         AND ST_IsValid(u.geom_32648)
                        THEN
                            ST_Distance(
                                ST_Transform(
                                    ST_SetSRID(
                                        ST_MakePoint(
                                            u.kinh_do,
                                            u.vi_do
                                        ),
                                        4326
                                    ),
                                    32648
                                ),
                                u.geom_32648
                            )
                    END AS sai_lech_toa_do_met,

                    CASE
                        WHEN u.geom_4326 IS NOT NULL
                         AND u.geom_32648 IS NOT NULL
                         AND ST_SRID(u.geom_4326) = 4326
                         AND ST_SRID(u.geom_32648) = 32648
                         AND ST_GeometryType(u.geom_4326)
                             = 'ST_Point'
                         AND ST_GeometryType(u.geom_32648)
                             = 'ST_Point'
                         AND ST_IsValid(u.geom_4326)
                         AND ST_IsValid(u.geom_32648)
                        THEN
                            ST_Distance(
                                ST_Transform(
                                    u.geom_4326,
                                    32648
                                ),
                                u.geom_32648
                            )
                    END AS sai_lech_hai_hinh_hoc_met

                FROM public.don_vi_quan_doi AS u
                ORDER BY u.ma_don_vi
                LIMIT @gioi_han;
            ";

            var danhSach =
                new List<ThongTinTopologyDonVi>();

            using (AdoConnection ketNoi =
                KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand lenh =
                TaoLenh(ketNoi, sql))
            {
                ThemThamSo(
                    lenh,
                    "@gioi_han",
                    DbType.Int32,
                    gioiHan
                );

                using (DbDataReader doc =
                    lenh.ExecuteReader())
                {
                    while (doc.Read())
                    {
                        danhSach.Add(
                            new ThongTinTopologyDonVi
                            {
                                MaDonVi = LayChuoi(doc, 0),

                                KinhDo = LaySoThuc(doc, 1),
                                ViDo = LaySoThuc(doc, 2),

                                CoGeom4326 = LayBool(doc, 3),
                                CoGeom32648 = LayBool(doc, 4),

                                SridGeom4326 =
                                    LaySoNguyen(doc, 5),

                                SridGeom32648 =
                                    LaySoNguyen(doc, 6),

                                Geom4326LaDiem =
                                    LayBoolNullable(doc, 7),

                                Geom32648LaDiem =
                                    LayBoolNullable(doc, 8),

                                Geom4326HopLe =
                                    LayBoolNullable(doc, 9),

                                Geom32648HopLe =
                                    LayBoolNullable(doc, 10),

                                SaiLechToaDoMet =
                                    LaySoThuc(doc, 11),

                                SaiLechHaiHinhHocMet =
                                    LaySoThuc(doc, 12)
                            }
                        );
                    }
                }
            }

            return danhSach;
        }

        // =====================================================
        // 2. TÌM CẶP VỊ TRÍ GẦN TRÙNG
        // =====================================================

        public IList<CapViTriGanTrung>
            TimCapViTriGanTrung(
                double nguongKhoangCachMet,
                int gioiHan)
        {
            KiemTraGioiHan(gioiHan);

            KiemTraKhoangCach(
                nguongKhoangCachMet
            );

            /*
             * NÂNG CẤP:
             *
             * Không tạo cột geometry trung gian bằng CASE
             * trước khi thực hiện phép JOIN.
             *
             * Sử dụng trực tiếp cột geom_32648 và toán tử &&
             * để PostgreSQL có cơ hội dùng chỉ mục GiST.
             *
             * Bước 1:
             * Lọc nhanh các bounding box gần nhau.
             *
             * Bước 2:
             * Xác nhận SRID, kiểu và tính hợp lệ.
             *
             * Bước 3:
             * Tính khoảng cách thật bằng ST_DWithin.
             *
             * CASE bảo đảm không gọi ST_DWithin
             * cho cặp hình học có SRID không phù hợp.
             */

            const string sql = @"
                SELECT
                    a.ma_don_vi AS ma_don_vi_a,
                    b.ma_don_vi AS ma_don_vi_b,

                    ST_Distance(
                        a.geom_32648,
                        b.geom_32648
                    ) AS khoang_cach_met

                FROM public.don_vi_quan_doi AS a

                JOIN public.don_vi_quan_doi AS b
                    ON b.ma_don_vi > a.ma_don_vi

                   AND b.geom_32648 &&
                       ST_Expand(
                           a.geom_32648,
                           @nguong_met
                       )

                WHERE a.geom_32648 IS NOT NULL
                  AND b.geom_32648 IS NOT NULL

                  AND CASE
                        WHEN
                            ST_SRID(a.geom_32648) = 32648
                            AND ST_SRID(b.geom_32648) = 32648

                            AND ST_GeometryType(
                                a.geom_32648
                            ) = 'ST_Point'

                            AND ST_GeometryType(
                                b.geom_32648
                            ) = 'ST_Point'

                            AND ST_IsValid(a.geom_32648)
                            AND ST_IsValid(b.geom_32648)

                        THEN ST_DWithin(
                            a.geom_32648,
                            b.geom_32648,
                            @nguong_met
                        )

                        ELSE FALSE
                      END

                ORDER BY
                    khoang_cach_met,
                    a.ma_don_vi,
                    b.ma_don_vi

                LIMIT @gioi_han;
            ";

            var danhSach =
                new List<CapViTriGanTrung>();

            using (AdoConnection ketNoi =
                KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand lenh =
                TaoLenh(ketNoi, sql))
            {
                ThemThamSo(
                    lenh,
                    "@nguong_met",
                    DbType.Double,
                    nguongKhoangCachMet
                );

                ThemThamSo(
                    lenh,
                    "@gioi_han",
                    DbType.Int32,
                    gioiHan
                );

                using (DbDataReader doc =
                    lenh.ExecuteReader())
                {
                    while (doc.Read())
                    {
                        danhSach.Add(
                            new CapViTriGanTrung
                            {
                                MaDonViA =
                                    LayChuoi(doc, 0),

                                MaDonViB =
                                    LayChuoi(doc, 1),

                                KhoangCachMet =
                                    Convert.ToDouble(
                                        doc.GetValue(2)
                                    )
                            }
                        );
                    }
                }
            }

            return danhSach;
        }

        // =====================================================
        // 3. TẠO LỆNH SQL
        // =====================================================

        private static DbCommand TaoLenh(
            AdoConnection ketNoi,
            string sql)
        {
            if (ketNoi == null)
                throw new ArgumentNullException(
                    "ketNoi"
                );

            if (string.IsNullOrWhiteSpace(sql))
                throw new ArgumentException(
                    "Nội dung SQL không được trống.",
                    "sql"
                );

            DbCommand lenh =
                ketNoi.CreateCommand();

            if (lenh == null)
                throw new InvalidOperationException(
                    "Không tạo được lệnh SQL."
                );

            lenh.CommandText = sql;
            lenh.CommandType = CommandType.Text;
            lenh.CommandTimeout = ThoiGianTruyVanGiay;

            return lenh;
        }

        // =====================================================
        // 4. THÊM THAM SỐ SQL AN TOÀN
        // =====================================================

        private static void ThemThamSo(
            DbCommand lenh,
            string ten,
            DbType kieu,
            object giaTri)
        {
            if (lenh == null)
                throw new ArgumentNullException(
                    "lenh"
                );

            DbParameter thamSo =
                lenh.CreateParameter();

            if (thamSo == null)
                throw new InvalidOperationException(
                    "Không tạo được tham số SQL."
                );

            thamSo.ParameterName = ten;
            thamSo.DbType = kieu;

            thamSo.Value =
                giaTri ?? DBNull.Value;

            lenh.Parameters.Add(thamSo);
        }

        // =====================================================
        // 5. KIỂM TRA GIỚI HẠN BẢN GHI
        // =====================================================

        private static void KiemTraGioiHan(
            int gioiHan)
        {
            if (gioiHan < 1 ||
                gioiHan > GioiHanDocToiDa)
            {
                throw new ArgumentOutOfRangeException(
                    "gioiHan",
                    "Giới hạn truy vấn phải từ 1 đến 5001."
                );
            }
        }

        // =====================================================
        // 6. KIỂM TRA KHOẢNG CÁCH
        // =====================================================

        private static void KiemTraKhoangCach(
            double khoangCachMet)
        {
            if (double.IsNaN(khoangCachMet) ||
                double.IsInfinity(khoangCachMet) ||
                khoangCachMet <= 0d ||
                khoangCachMet >
                    NghiepVuKiemTraTopology.NguongMetToiDa)
            {
                throw new ArgumentOutOfRangeException(
                    "khoangCachMet",
                    "Khoảng cách phải lớn hơn 0 " +
                    "và không vượt ngưỡng tối đa của BLL."
                );
            }
        }

        // =====================================================
        // 7. ĐỌC GIÁ TRỊ CHUỖI
        // =====================================================

        private static string LayChuoi(
            DbDataReader doc,
            int cot)
        {
            return doc.IsDBNull(cot)
                ? null
                : Convert.ToString(
                    doc.GetValue(cot)
                );
        }

        // =====================================================
        // 8. ĐỌC SỐ THỰC
        // =====================================================

        private static double? LaySoThuc(
            DbDataReader doc,
            int cot)
        {
            if (doc.IsDBNull(cot))
                return null;

            return Convert.ToDouble(
                doc.GetValue(cot)
            );
        }

        // =====================================================
        // 9. ĐỌC SỐ NGUYÊN
        // =====================================================

        private static int? LaySoNguyen(
            DbDataReader doc,
            int cot)
        {
            if (doc.IsDBNull(cot))
                return null;

            return Convert.ToInt32(
                doc.GetValue(cot)
            );
        }

        // =====================================================
        // 10. ĐỌC GIÁ TRỊ BOOLEAN
        // =====================================================

        private static bool LayBool(
            DbDataReader doc,
            int cot)
        {
            return !doc.IsDBNull(cot) &&
                Convert.ToBoolean(
                    doc.GetValue(cot)
                );
        }

        // =====================================================
        // 11. ĐỌC BOOLEAN CÓ THỂ NULL
        // =====================================================

        private static bool? LayBoolNullable(
            DbDataReader doc,
            int cot)
        {
            if (doc.IsDBNull(cot))
                return null;

            return Convert.ToBoolean(
                doc.GetValue(cot)
            );
        }
    }
}
