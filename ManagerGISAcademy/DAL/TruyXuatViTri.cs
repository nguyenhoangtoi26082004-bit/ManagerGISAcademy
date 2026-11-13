using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ManagerGISAcademy.DAL
{
    // =====================================================
    // DTO CHUC NANG "DINH VI LAI TRU SO"
    // =====================================================

    /// <summary>Mot dong danh muc ql_don_vi.dm_phuong_phap_dinh_vi.</summary>
    public sealed class PhuongPhapDinhVi
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public decimal DoChinhXacDanhNghiaM { get; set; }
        public bool LaViTriThuc { get; set; }

        public override string ToString()
        {
            return Ten + "  (≥ " + DoChinhXacDanhNghiaM.ToString("0.##") + " m)";
        }
    }

    /// <summary>Vi tri hien hanh cua mot don vi (v_ban_do_don_vi).</summary>
    public sealed class ViTriHienHanh
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public double E32648 { get; set; }
        public double N32648 { get; set; }
        public double XVn2000 { get; set; }   // Bac
        public double YVn2000 { get; set; }   // Dong
        public string DiaBan { get; set; }
        public string TenPhuongPhap { get; set; }
        public decimal? DoChinhXacM { get; set; }
        public bool LaViTriThuc { get; set; }
        public string NguonViTri { get; set; }
    }

    /// <summary>Ket qua quy doi 1 diem sang cac he toa do.</summary>
    public sealed class KetQuaQuyDoi
    {
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public double E32648 { get; set; }
        public double N32648 { get; set; }
        public double XVn2000 { get; set; }
        public double YVn2000 { get; set; }
        public string TenXaPhuong { get; set; }
        public bool TrongHaNoi { get; set; }
    }

    /// <summary>Ket qua sau khi luu vi tri moi.</summary>
    public sealed class KetQuaDinhVi
    {
        public long ViTriId { get; set; }
        public string TenXaPhuong { get; set; }
        public bool DaDoiXa { get; set; }
        public decimal DoChinhXacM { get; set; }
        public double KhoangDichM { get; set; }
    }

    /// <summary>Mot dong ql_don_vi.v_kiem_tra_vi_tri.</summary>
    public sealed class LoiViTri
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string LoaiLoi { get; set; }   // ma: DIEM_DAI_DIEN, TRUNG_VI_TRI...
        public string MucDo { get; set; }     // NGHIEM_TRONG / CANH_BAO
        public string MoTa { get; set; }
    }

    /// <summary>Tong hop chat luong vi tri toan he thong.</summary>
    public sealed class TongHopViTri
    {
        public int Tong { get; set; }
        public int ViTriTot { get; set; }        // vi tri that, <= 10 m
        public int CanDoLai { get; set; }        // vi tri that, > 10 m
        public int DiemDaiDien { get; set; }
        public int DaKiemChung { get; set; }
    }

    // =====================================================
    // DAL - goi ham CSDL, khong ghi bang goc
    // =====================================================

    public sealed class TruyXuatViTri
    {
        public const int SridWgs84 = 4326;
        public const int SridVn2000Tm3 = 5897;   // VN-2000 / TM-3 KTT 105°
        public const int SridUtm48N = 32648;     // WGS84 / UTM 48N

        public IList<PhuongPhapDinhVi> LayPhuongPhap()
        {
            const string sql = @"
                SELECT ma, ten, do_chinh_xac_danh_nghia_m, la_vi_tri_thuc
                FROM ql_don_vi.dm_phuong_phap_dinh_vi
                WHERE la_vi_tri_thuc
                ORDER BY do_chinh_xac_danh_nghia_m, ma";

            List<PhuongPhapDinhVi> ds = new List<PhuongPhapDinhVi>();

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = TaoLenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    ds.Add(new PhuongPhapDinhVi
                    {
                        Ma = rd.GetString(0),
                        Ten = rd.GetString(1),
                        DoChinhXacDanhNghiaM = Convert.ToDecimal(rd.GetValue(2)),
                        LaViTriThuc = Convert.ToBoolean(rd.GetValue(3))
                    });
                }
            }

            return ds;
        }

        public ViTriHienHanh LayViTriHienHanh(string maDonVi)
        {
            const string sql = @"
                SELECT ma_don_vi, ten_don_vi, kinh_do, vi_do,
                       x_32648, y_32648, x_vn2000, y_vn2000,
                       CASE loai_xa_phuong WHEN 'phuong' THEN 'Phường ' ELSE 'Xã ' END
                           || ten_xa_phuong AS dia_ban,
                       ten_phuong_phap, do_chinh_xac_m, la_vi_tri_thuc, nguon_vi_tri
                FROM ql_don_vi.v_ban_do_don_vi
                WHERE ma_don_vi = @ma";

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = TaoLenh(cn, sql))
            {
                ThamSo(cmd, "@ma", DbType.String, (maDonVi ?? "").Trim());

                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        return null;

                    return new ViTriHienHanh
                    {
                        MaDonVi = rd.GetString(0),
                        TenDonVi = rd.GetString(1),
                        KinhDo = rd.GetDouble(2),
                        ViDo = rd.GetDouble(3),
                        E32648 = rd.GetDouble(4),
                        N32648 = rd.GetDouble(5),
                        XVn2000 = rd.GetDouble(6),
                        YVn2000 = rd.GetDouble(7),
                        DiaBan = rd.IsDBNull(8) ? "" : rd.GetString(8),
                        TenPhuongPhap = rd.IsDBNull(9) ? "" : rd.GetString(9),
                        DoChinhXacM = rd.IsDBNull(10) ? (decimal?)null : Convert.ToDecimal(rd.GetValue(10)),
                        LaViTriThuc = !rd.IsDBNull(11) && Convert.ToBoolean(rd.GetValue(11)),
                        NguonViTri = rd.IsDBNull(12) ? "" : rd.GetString(12)
                    };
                }
            }
        }

        /// <summary>
        /// Quy doi (e, n) theo he goc sang moi he. Voi WGS84: e = kinh do,
        /// n = vi do. Voi VN-2000: e = Y (Dong), n = X (Bac).
        /// Toa do ngoai Ha Noi / dao truc -> CSDL bao loi tieng Viet.
        /// </summary>
        public KetQuaQuyDoi QuyDoi(int srid, double e, double n)
        {
            const string sql = @"
                SELECT kinh_do, vi_do, e_32648, n_32648, x_vn2000, y_vn2000,
                       ten_xa_phuong, trong_ha_noi
                FROM ql_don_vi.fn_quy_doi_toa_do(@srid, @e, @n)";

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = TaoLenh(cn, sql))
            {
                ThamSo(cmd, "@srid", DbType.Int32, srid);
                ThamSo(cmd, "@e", DbType.Double, e);
                ThamSo(cmd, "@n", DbType.Double, n);

                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read())
                        return null;

                    return new KetQuaQuyDoi
                    {
                        KinhDo = rd.GetDouble(0),
                        ViDo = rd.GetDouble(1),
                        E32648 = rd.GetDouble(2),
                        N32648 = rd.GetDouble(3),
                        XVn2000 = rd.GetDouble(4),
                        YVn2000 = rd.GetDouble(5),
                        TenXaPhuong = rd.IsDBNull(6) ? null : rd.GetString(6),
                        TrongHaNoi = !rd.IsDBNull(7) && Convert.ToBoolean(rd.GetValue(7))
                    };
                }
            }
        }

        public KetQuaDinhVi DinhViTruSo(
            string maDonVi, int srid, double e, double n,
            string maPhuongPhap, decimal? doChinhXacM,
            string nguon, string nguoiXacMinh, string ghiChu)
        {
            const string sql = @"
                SELECT vi_tri_id, ten_xa_phuong, da_doi_xa, do_chinh_xac_m, khoang_dich_m
                FROM ql_don_vi.fn_dinh_vi_tru_so(
                    @ma, @srid, @e, @n, @pp, @dcx, @nguon, @nguoi, @ghi_chu)";

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbTransaction tx = cn.BeginTransaction())
            using (DbCommand cmd = TaoLenh(cn, sql))
            {
                cmd.Transaction = tx;
                ThamSo(cmd, "@ma", DbType.String, (maDonVi ?? "").Trim());
                ThamSo(cmd, "@srid", DbType.Int32, srid);
                ThamSo(cmd, "@e", DbType.Double, e);
                ThamSo(cmd, "@n", DbType.Double, n);
                ThamSo(cmd, "@pp", DbType.String, maPhuongPhap);
                ThamSo(cmd, "@dcx", DbType.Decimal, doChinhXacM);
                ThamSo(cmd, "@nguon", DbType.String, nguon);
                ThamSo(cmd, "@nguoi", DbType.String, nguoiXacMinh);
                ThamSo(cmd, "@ghi_chu", DbType.String, ghiChu);

                KetQuaDinhVi kq = null;

                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        kq = new KetQuaDinhVi
                        {
                            ViTriId = Convert.ToInt64(rd.GetValue(0)),
                            TenXaPhuong = rd.IsDBNull(1) ? "" : rd.GetString(1),
                            DaDoiXa = Convert.ToBoolean(rd.GetValue(2)),
                            DoChinhXacM = Convert.ToDecimal(rd.GetValue(3)),
                            KhoangDichM = Convert.ToDouble(rd.GetValue(4))
                        };
                    }
                }

                if (kq == null)
                {
                    tx.Rollback();
                    throw new InvalidOperationException("CSDL không trả về kết quả định vị.");
                }

                tx.Commit();
                return kq;
            }
        }

        // =================================================
        // CHAT LUONG DU LIEU KHONG GIAN
        // =================================================

        public IList<LoiViTri> LayLoiViTri()
        {
            const string sql = @"
                SELECT ma_don_vi, ten_don_vi, loai_loi, muc_do, mo_ta
                FROM ql_don_vi.v_kiem_tra_vi_tri
                ORDER BY muc_do DESC, loai_loi, ma_don_vi";

            List<LoiViTri> ds = new List<LoiViTri>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = TaoLenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    ds.Add(new LoiViTri
                    {
                        MaDonVi = rd.IsDBNull(0) ? "" : rd.GetString(0),
                        TenDonVi = rd.IsDBNull(1) ? "" : rd.GetString(1),
                        LoaiLoi = rd.IsDBNull(2) ? "" : rd.GetString(2),
                        MucDo = rd.IsDBNull(3) ? "" : rd.GetString(3),
                        MoTa = rd.IsDBNull(4) ? "" : rd.GetString(4)
                    });
                }
            }
            return ds;
        }

        public TongHopViTri LayTongHopViTri()
        {
            const string sql = @"
                SELECT COALESCE(sum(tong), 0), COALESCE(sum(vi_tri_tot), 0),
                       COALESCE(sum(vi_tri_can_do_lai), 0), COALESCE(sum(diem_dai_dien), 0),
                       COALESCE(sum(da_kiem_chung), 0)
                FROM ql_don_vi.v_thong_ke_vi_tri";

            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = TaoLenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
            {
                if (!rd.Read()) return new TongHopViTri();
                return new TongHopViTri
                {
                    Tong = Convert.ToInt32(rd.GetValue(0)),
                    ViTriTot = Convert.ToInt32(rd.GetValue(1)),
                    CanDoLai = Convert.ToInt32(rd.GetValue(2)),
                    DiemDaiDien = Convert.ToInt32(rd.GetValue(3)),
                    DaKiemChung = Convert.ToInt32(rd.GetValue(4))
                };
            }
        }

        // -------------------------------------------------

        private static DbCommand TaoLenh(DbConnection cn, string sql)
        {
            DbCommand cmd = cn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 30;
            return cmd;
        }

        private static void ThamSo(DbCommand cmd, string ten, DbType kieu, object giaTri)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = ten;
            p.DbType = kieu;
            p.Value = giaTri ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
