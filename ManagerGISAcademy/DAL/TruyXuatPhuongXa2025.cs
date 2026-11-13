using System;
using System.Data;
using System.Data.Common;

using AdoConnection = System.Data.Common.DbConnection;
using AdoCommand = System.Data.Common.DbCommand;
using AdoReader = System.Data.Common.DbDataReader;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Ket qua xac dinh phuong/xa tu toa do WGS84.
    /// </summary>
    public sealed class KetQuaPhuongXa2025
    {
        public bool TimThay { get; internal set; }

        public string MaPhuongXa { get; internal set; }

        public string TenPhuongXa { get; internal set; }

        public string Nguon { get; internal set; }

        public string ThongBao { get; internal set; }
    }

    /// <summary>
    /// GD5.11 - Tra cuu phuong/xa Ha Noi theo toa do.
    ///
    /// Nen tang:
    /// - C# 7.3
    /// - .NET Framework 4.8
    /// - PostgreSQL / PostGIS
    ///
    /// Chuc nang:
    /// 1. Kiem tra toa do WGS84.
    /// 2. Mo ket noi PostgreSQL hien tai.
    /// 3. Kiem tra bang ranh gioi.
    /// 4. Kiem tra bo 126 phuong/xa.
    /// 5. Tim dia danh bang ST_Covers.
    /// 6. Phat hien diem thuoc nhieu da giac.
    ///
    /// Khong them, sua, xoa du lieu PostGIS.
    /// </summary>
    public sealed class TruyXuatPhuongXa2025
    {
        private const string TenBang =
            "public.phuong_xa_hanoi_2025";

        // ==========================================
        // 1. XAC DINH PHUONG / XA TU WGS84
        // ==========================================

        public KetQuaPhuongXa2025 XacDinh(
            double kinhDo,
            double viDo)
        {
            if (double.IsNaN(kinhDo) ||
                double.IsInfinity(kinhDo) ||
                double.IsNaN(viDo) ||
                double.IsInfinity(viDo) ||
                kinhDo < -180d ||
                kinhDo > 180d ||
                viDo < -90d ||
                viDo > 90d)
            {
                return KhongTimThay(
                    "Tọa độ WGS84 không hợp lệ.");
            }

            // SUA LOI:
            // Su dung lop ket noi hien tai cua du an.
            // Khong su dung DAL.DbConnection cu.

            using (AdoConnection ketNoi =
                KetNoiCoSoDuLieu.MoKetNoi())
            {
                if (ketNoi == null ||
                    ketNoi.State != ConnectionState.Open)
                {
                    return KhongTimThay(
                        "Không mở được kết nối PostgreSQL.");
                }

                // Kiem tra bang ranh gioi.
                if (!BangDaTonTai(ketNoi))
                {
                    return KhongTimThay(
                        "Chưa có bảng ranh giới " +
                        TenBang +
                        " trong cơ sở dữ liệu PostGIS.");
                }

                // Kiem tra tinh day du
                // cua bo ranh gioi 126 phuong/xa.
                string loiDuLieu =
                    KiemTraBoRanhGioi(ketNoi);

                if (!string.IsNullOrEmpty(loiDuLieu))
                {
                    return KhongTimThay(loiDuLieu);
                }

                // Tim phuong/xa chua diem WGS84.
                return TimPhuongXaTheoDiem(
                    ketNoi,
                    kinhDo,
                    viDo);
            }
        }

        // ==========================================
        // 2. KIEM TRA BANG TON TAI
        // ==========================================

        private static bool BangDaTonTai(
            AdoConnection ketNoi)
        {
            using (AdoCommand lenh =
                ketNoi.CreateCommand())
            {
                lenh.CommandText =
                    "SELECT to_regclass(" +
                    "'public.phuong_xa_hanoi_2025')" +
                    " IS NOT NULL";

                lenh.CommandType =
                    CommandType.Text;

                lenh.CommandTimeout = 10;

                object giaTri =
                    lenh.ExecuteScalar();

                return giaTri != null &&
                       giaTri != DBNull.Value &&
                       Convert.ToBoolean(giaTri);
            }
        }

        // ==========================================
        // 3. KIEM TRA BO RANH GIOI
        // ==========================================

        private static string KiemTraBoRanhGioi(
            AdoConnection ketNoi)
        {
            const string sql = @"
                SELECT
                    COUNT(*) AS tong,

                    COUNT(*) FILTER (
                        WHERE loai_dvhc = 'Phường'
                    ) AS so_phuong,

                    COUNT(*) FILTER (
                        WHERE loai_dvhc = 'Xã'
                    ) AS so_xa,

                    COUNT(*) FILTER (
                        WHERE geom IS NULL
                           OR ST_IsEmpty(geom)
                           OR NOT ST_IsValid(geom)
                    ) AS hinh_hoc_loi,

                    COUNT(*) FILTER (
                        WHERE NULLIF(
                            BTRIM(nguon), ''
                        ) IS NULL
                    ) AS thieu_nguon,

                    COUNT(
                        DISTINCT LOWER(
                            BTRIM(loai_dvhc) || '|' ||
                            BTRIM(ten_dvhc)
                        )
                    ) AS ten_phan_biet

                FROM public.phuong_xa_hanoi_2025";

            using (AdoCommand lenh =
                ketNoi.CreateCommand())
            {
                lenh.CommandText = sql;

                lenh.CommandType =
                    CommandType.Text;

                lenh.CommandTimeout = 15;

                using (AdoReader doc =
                    lenh.ExecuteReader())
                {
                    if (!doc.Read())
                    {
                        return
                            "Không kiểm tra được dữ liệu ranh giới.";
                    }

                    long tong =
                        Convert.ToInt64(doc.GetValue(0));

                    long soPhuong =
                        Convert.ToInt64(doc.GetValue(1));

                    long soXa =
                        Convert.ToInt64(doc.GetValue(2));

                    long hinhHocLoi =
                        Convert.ToInt64(doc.GetValue(3));

                    long thieuNguon =
                        Convert.ToInt64(doc.GetValue(4));

                    long tenPhanBiet =
                        Convert.ToInt64(doc.GetValue(5));

                    // Ha Noi sau sap xep:
                    // 51 phuong + 75 xa = 126 don vi.

                    if (tong != 126 ||
                        soPhuong != 51 ||
                        soXa != 75)
                    {
                        return
                            "Bộ ranh giới chưa đủ 126 đơn vị. " +
                            "Hiện có " + tong + " vùng, " +
                            soPhuong + " phường và " +
                            soXa + " xã. " +
                            "Chưa tự động xác định địa danh.";
                    }

                    if (hinhHocLoi > 0)
                    {
                        return
                            "Phát hiện " +
                            hinhHocLoi +
                            " đa giác không hợp lệ hoặc rỗng.";
                    }

                    if (thieuNguon > 0)
                    {
                        return
                            "Có " +
                            thieuNguon +
                            " ranh giới chưa khai báo nguồn dữ liệu.";
                    }

                    if (tenPhanBiet != 126)
                    {
                        return
                            "Bộ ranh giới có tên phường/xã " +
                            "trùng lặp hoặc chưa hợp lệ.";
                    }
                }
            }

            return null;
        }

        // ==========================================
        // 4. TIM PHUONG / XA THEO TOA DO
        // ==========================================

        private static KetQuaPhuongXa2025 TimPhuongXaTheoDiem(
            AdoConnection ketNoi,
            double kinhDo,
            double viDo)
        {
            const string sql = @"
                SELECT
                    ma_dvhc,
                    loai_dvhc,
                    ten_dvhc,
                    nguon

                FROM public.phuong_xa_hanoi_2025

                WHERE ST_Covers(
                    geom,
                    ST_SetSRID(
                        ST_MakePoint(
                            @kinh_do,
                            @vi_do
                        ),
                        4326
                    )
                )

                ORDER BY ma_dvhc

                LIMIT 2";

            using (AdoCommand lenh =
                ketNoi.CreateCommand())
            {
                lenh.CommandText = sql;

                lenh.CommandType =
                    CommandType.Text;

                lenh.CommandTimeout = 15;

                ThemThamSo(
                    lenh,
                    "@kinh_do",
                    DbType.Double,
                    kinhDo);

                ThemThamSo(
                    lenh,
                    "@vi_do",
                    DbType.Double,
                    viDo);

                using (AdoReader doc =
                    lenh.ExecuteReader())
                {
                    if (!doc.Read())
                    {
                        return KhongTimThay(
                            "Tọa độ không thuộc đa giác nào " +
                            "trong bộ ranh giới đã nạp.");
                    }

                    string ma =
                        Convert.ToString(
                            doc.GetValue(0));

                    string loai =
                        Convert.ToString(
                            doc.GetValue(1));

                    string ten =
                        Convert.ToString(
                            doc.GetValue(2));

                    string nguon =
                        Convert.ToString(
                            doc.GetValue(3));

                    // ST_Covers co the tra ve
                    // nhieu vung neu diem nam
                    // ngay tren duong ranh gioi.

                    if (doc.Read())
                    {
                        return KhongTimThay(
                            "Tọa độ thuộc nhiều vùng ranh giới. " +
                            "Cần xác minh thủ công.");
                    }

                    return new KetQuaPhuongXa2025
                    {
                        TimThay = true,

                        MaPhuongXa = ma,

                        TenPhuongXa =
                            loai.Trim() + " " + ten.Trim(),

                        Nguon = nguon,

                        ThongBao =
                            "Đã xác định địa danh " +
                            "từ dữ liệu ranh giới PostGIS."
                    };
                }
            }
        }

        // ==========================================
        // 5. TAO THAM SO SQL
        // ==========================================

        private static void ThemThamSo(
            AdoCommand lenh,
            string ten,
            DbType kieu,
            object giaTri)
        {
            DbParameter thamSo =
                lenh.CreateParameter();

            if (thamSo == null)
            {
                throw new InvalidOperationException(
                    "Không tạo được tham số SQL: " + ten);
            }

            thamSo.ParameterName = ten;

            thamSo.DbType = kieu;

            thamSo.Value =
                giaTri ?? DBNull.Value;

            lenh.Parameters.Add(thamSo);
        }

        // ==========================================
        // 6. KET QUA KHONG TIM THAY
        // ==========================================

        private static KetQuaPhuongXa2025 KhongTimThay(
            string thongBao)
        {
            return new KetQuaPhuongXa2025
            {
                TimThay = false,

                MaPhuongXa = null,

                TenPhuongXa = null,

                Nguon = null,

                ThongBao = thongBao
            };
        }
    }
}   