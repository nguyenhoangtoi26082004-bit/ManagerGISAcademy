using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.DAL
{
    /// <summary>
    /// Đọc nhật ký thay đổi hồ sơ đơn vị (public.nhat_ky_don_vi_quan_doi).
    /// Chỉ SELECT: tài khoản ứng dụng không có quyền ghi/xóa nhật ký.
    /// So sánh bản cũ/mới bằng JSONB ngay trong PostgreSQL.
    /// </summary>
    public sealed class TruyXuatNhatKy
    {
        public const int GioiHanToiDa = 2000;

        // Cot ky thuat, khong hien khi so sanh thay doi.
        private const string CotBoQua =
            "('id','ngay_cap_nhat','x_vn2000','y_vn2000')";

        /// <summary>Đã chạy script 04 (có bảng nhật ký) hay chưa.</summary>
        public bool BangNhatKyTonTai()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT to_regclass('public.nhat_ky_don_vi_quan_doi') IS NOT NULL";
                cmd.CommandTimeout = 15;
                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }

        /// <summary>
        /// Lịch sử mới nhất trước. maDonVi null/rỗng = mọi đơn vị.
        /// thaoTac null = mọi loại; ngược lại "I", "U" hoặc "D".
        /// </summary>
        public IList<NhatKyThayDoi> LayNhatKy(
            string maDonVi,
            string thaoTac,
            int gioiHan)
        {
            if (gioiHan < 1 || gioiHan > GioiHanToiDa)
                throw new ArgumentOutOfRangeException(
                    "gioiHan", "Giới hạn từ 1 đến " + GioiHanToiDa + ".");

            string sql = @"
                SELECT
                    n.id,
                    n.thao_tac,
                    n.ma_don_vi,
                    COALESCE(n.du_lieu_moi ->> 'ten_don_vi',
                             n.du_lieu_cu ->> 'ten_don_vi') AS ten_don_vi,
                    n.tai_khoan_db,
                    n.ung_dung,
                    n.thoi_diem,
                    CASE WHEN n.thao_tac = 'U' THEN
                        ARRAY(
                            SELECT k
                            FROM jsonb_object_keys(n.du_lieu_moi) AS k
                            WHERE k NOT IN " + CotBoQua + @"
                              AND (n.du_lieu_cu -> k)
                                  IS DISTINCT FROM (n.du_lieu_moi -> k)
                            ORDER BY k)
                    ELSE ARRAY[]::text[] END AS truong_thay_doi
                FROM public.nhat_ky_don_vi_quan_doi AS n
                WHERE (CAST(@ma AS text) IS NULL
                       OR n.ma_don_vi ILIKE '%' || CAST(@ma AS text) || '%')
                  AND (CAST(@tt AS text) IS NULL
                       OR n.thao_tac = CAST(@tt AS text))
                ORDER BY n.thoi_diem DESC, n.id DESC
                LIMIT @lim";

            List<NhatKyThayDoi> ketQua = new List<NhatKyThayDoi>();

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = 30;

                ThemThamSo(cmd, "@ma", DbType.String,
                    string.IsNullOrWhiteSpace(maDonVi) ? null : maDonVi.Trim());
                ThemThamSo(cmd, "@tt", DbType.String,
                    string.IsNullOrWhiteSpace(thaoTac) ? null : thaoTac.Trim());
                ThemThamSo(cmd, "@lim", DbType.Int32, gioiHan);

                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ketQua.Add(new NhatKyThayDoi
                        {
                            Id = Convert.ToInt64(rd.GetValue(0)),
                            ThaoTac = DocChuoi(rd, 1),
                            MaDonVi = DocChuoi(rd, 2),
                            TenDonVi = DocChuoi(rd, 3),
                            TaiKhoanDb = DocChuoi(rd, 4),
                            UngDung = DocChuoi(rd, 5),
                            ThoiDiem = rd.IsDBNull(6)
                                ? DateTime.MinValue
                                : Convert.ToDateTime(rd.GetValue(6)).ToLocalTime(),
                            TruongThayDoi = rd.IsDBNull(7)
                                ? new string[0]
                                : (string[])rd.GetValue(7)
                        });
                    }
                }
            }

            return ketQua;
        }

        /// <summary>
        /// Từng trường cũ → mới của một dòng nhật ký.
        /// Thêm: chỉ có giá trị mới; Xóa: chỉ có giá trị cũ;
        /// Sửa: chỉ các trường thực sự khác nhau.
        /// </summary>
        public IList<ChiTietThayDoi> LayChiTiet(long idNhatKy)
        {
            string sql = @"
                SELECT
                    k,
                    n.du_lieu_cu ->> k AS cu,
                    n.du_lieu_moi ->> k AS moi
                FROM public.nhat_ky_don_vi_quan_doi AS n
                CROSS JOIN LATERAL jsonb_object_keys(
                    COALESCE(n.du_lieu_moi, n.du_lieu_cu)) AS k
                WHERE n.id = @id
                  AND k NOT IN " + CotBoQua + @"
                  AND (n.thao_tac <> 'U'
                       OR (n.du_lieu_cu -> k)
                          IS DISTINCT FROM (n.du_lieu_moi -> k))
                ORDER BY k";

            List<ChiTietThayDoi> ketQua = new List<ChiTietThayDoi>();

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandTimeout = 30;
                ThemThamSo(cmd, "@id", DbType.Int64, idNhatKy);

                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        ketQua.Add(new ChiTietThayDoi
                        {
                            TenCot = DocChuoi(rd, 0),
                            GiaTriCu = DocChuoi(rd, 1),
                            GiaTriMoi = DocChuoi(rd, 2)
                        });
                    }
                }
            }

            return ketQua;
        }

        private static string DocChuoi(DbDataReader rd, int i)
        {
            return rd.IsDBNull(i) ? null : Convert.ToString(rd.GetValue(i));
        }

        private static void ThemThamSo(
            DbCommand cmd, string ten, DbType kieu, object giaTri)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = ten;
            p.DbType = kieu;
            p.Value = giaTri ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
