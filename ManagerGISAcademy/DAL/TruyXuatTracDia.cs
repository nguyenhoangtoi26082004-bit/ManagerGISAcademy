using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

namespace ManagerGISAcademy.DAL
{
    // =====================================================
    // DTO trac dia / GIS
    // =====================================================

    public sealed class DonViRutGon
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string Loai { get; set; }
        public string DiaBan { get; set; }
        public double X { get; set; }       // E EPSG:32648
        public double Y { get; set; }       // N EPSG:32648
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public override string ToString() { return Ten + "   ·   " + Ma; }
    }

    public sealed class KetQuaKiemTraToaDo
    {
        public bool HopLe { get; set; }
        public string ThongBao { get; set; }
        public double? KinhDo { get; set; }
        public double? ViDo { get; set; }
        public double? E { get; set; }
        public double? N { get; set; }
        public string TenXaPhuong { get; set; }
        public string TenDonVi { get; set; }
        public double? KhoangDichM { get; set; }
    }

    public sealed class DongLichSuViTri
    {
        public int Stt { get; set; }
        public DateTime HieuLucTu { get; set; }
        public DateTime? HieuLucDen { get; set; }
        public bool LaHienHanh { get; set; }
        public string PhuongPhap { get; set; }
        public string TenPhuongPhap { get; set; }
        public decimal DoChinhXacM { get; set; }
        public bool LaViTriThuc { get; set; }
        public string Nguon { get; set; }
        public string NguoiXacMinh { get; set; }
        public string GhiChu { get; set; }
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public double E { get; set; }
        public double N { get; set; }
        public double CachHienHanhM { get; set; }

        public string ThoiGian { get { return HieuLucTu.ToString("dd/MM/yyyy HH:mm"); } }
        public string TrangThai { get { return LaHienHanh ? "Hiện hành" : "Lịch sử"; } }
        public string SaiSo { get { return "±" + DoChinhXacM.ToString("0.##", CultureInfo.GetCultureInfo("vi-VN")) + " m"; } }
        public string ToaDo { get { return ViDo.ToString("0.000000", CultureInfo.InvariantCulture) + ", " + KinhDo.ToString("0.000000", CultureInfo.InvariantCulture); } }
        public string Cach { get { return LaHienHanh ? "—" : CachHienHanhM.ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " m"; } }
    }

    public sealed class KhuonVien
    {
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenKhuVuc { get; set; }
        public double DienTichM2 { get; set; }
        public double ChuViM { get; set; }
        public bool ChuaTruSo { get; set; }
        public string Wkt { get; set; }
    }

    public sealed class KetQuaLuuKhuonVien
    {
        public long Id { get; set; }
        public double DienTichM2 { get; set; }
        public double ChuViM { get; set; }
        public bool ChuaTruSo { get; set; }
    }

    public sealed class KetQuaTuyen
    {
        public string Wkt { get; set; }
        public double DaiM { get; set; }
        public double Phut { get; set; }
        public int SoDoan { get; set; }
        public string TenDuong { get; set; }
        public string MaDich { get; set; }
        public string TenDich { get; set; }
    }

    public sealed class VungTiepCan
    {
        public int Phut { get; set; }
        public string Wkt { get; set; }
        public double DienTichKm2 { get; set; }
    }

    public sealed class DiemMatCat
    {
        public int Stt { get; set; }
        public double KhoangCachM { get; set; }
        public double? DoCaoM { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    // =====================================================
    // DAL
    // =====================================================

    public sealed class TruyXuatTracDia
    {
        // ---------- danh sach don vi (chon nhanh) ----------

        public List<DonViRutGon> LayDanhSachDonVi()
        {
            List<DonViRutGon> ds = new List<DonViRutGon>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, @"SELECT ma_don_vi, ten_don_vi, loai_don_vi, quan_huyen, x_vn2000, y_vn2000, kinh_do, vi_do
                                             FROM public.don_vi_quan_doi ORDER BY ten_don_vi"))
            using (DbDataReader rd = cmd.ExecuteReader())
                while (rd.Read())
                    ds.Add(new DonViRutGon
                    {
                        Ma = Chuoi(rd, 0), Ten = Chuoi(rd, 1), Loai = Chuoi(rd, 2), DiaBan = Chuoi(rd, 3),
                        X = So(rd, 4) ?? 0, Y = So(rd, 5) ?? 0, KinhDo = So(rd, 6) ?? 0, ViDo = So(rd, 7) ?? 0
                    });
            return ds;
        }

        // ---------- nhap toa do ----------

        public KetQuaKiemTraToaDo KiemTraToaDo(string maDonVi, int srid, double e, double n)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT * FROM ql_don_vi.fn_kiem_tra_nhap_toa_do(@ma, @srid, @e, @n)"))
            {
                ThamSo(cmd, "@ma", DbType.String, maDonVi);
                ThamSo(cmd, "@srid", DbType.Int32, srid);
                ThamSo(cmd, "@e", DbType.Double, e);
                ThamSo(cmd, "@n", DbType.Double, n);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read()) return new KetQuaKiemTraToaDo { HopLe = false, ThongBao = "Không có kết quả" };
                    return new KetQuaKiemTraToaDo
                    {
                        HopLe = !rd.IsDBNull(0) && rd.GetBoolean(0),
                        ThongBao = Chuoi(rd, 1),
                        KinhDo = So(rd, 2), ViDo = So(rd, 3), E = So(rd, 4), N = So(rd, 5),
                        TenXaPhuong = Chuoi(rd, 6), TenDonVi = Chuoi(rd, 7), KhoangDichM = So(rd, 8)
                    };
                }
            }
        }

        // ---------- lich su vi tri ----------

        public List<DongLichSuViTri> LichSuViTri(string maDonVi)
        {
            List<DongLichSuViTri> ds = new List<DongLichSuViTri>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT * FROM ql_don_vi.fn_lich_su_vi_tri(@ma)"))
            {
                ThamSo(cmd, "@ma", DbType.String, maDonVi);
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        ds.Add(new DongLichSuViTri
                        {
                            Stt = Convert.ToInt32(rd.GetValue(0)),
                            HieuLucTu = rd.GetDateTime(1),
                            HieuLucDen = rd.IsDBNull(2) ? (DateTime?)null : rd.GetDateTime(2),
                            LaHienHanh = !rd.IsDBNull(3) && rd.GetBoolean(3),
                            PhuongPhap = Chuoi(rd, 4), TenPhuongPhap = Chuoi(rd, 5),
                            DoChinhXacM = rd.IsDBNull(6) ? 0 : Convert.ToDecimal(rd.GetValue(6)),
                            LaViTriThuc = !rd.IsDBNull(7) && rd.GetBoolean(7),
                            Nguon = Chuoi(rd, 8), NguoiXacMinh = Chuoi(rd, 9), GhiChu = Chuoi(rd, 10),
                            KinhDo = So(rd, 11) ?? 0, ViDo = So(rd, 12) ?? 0, E = So(rd, 13) ?? 0, N = So(rd, 14) ?? 0,
                            CachHienHanhM = So(rd, 15) ?? 0
                        });
            }
            return ds;
        }

        // ---------- khuon vien ----------

        public List<KhuonVien> LayKhuonVien()
        {
            List<KhuonVien> ds = new List<KhuonVien>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ma_don_vi, ten_don_vi, ten_khu_vuc, dien_tich_m2, chu_vi_m, chua_tru_so, wkt FROM ql_don_vi.v_khuon_vien_don_vi"))
            using (DbDataReader rd = cmd.ExecuteReader())
                while (rd.Read())
                    ds.Add(new KhuonVien
                    {
                        MaDonVi = Chuoi(rd, 0), TenDonVi = Chuoi(rd, 1), TenKhuVuc = Chuoi(rd, 2),
                        DienTichM2 = So(rd, 3) ?? 0, ChuViM = So(rd, 4) ?? 0,
                        ChuaTruSo = !rd.IsDBNull(5) && rd.GetBoolean(5), Wkt = Chuoi(rd, 6)
                    });
            return ds;
        }

        public KetQuaLuuKhuonVien LuuKhuonVien(string maDonVi, string wkt32648, string ten, string ghiChu)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT * FROM ql_don_vi.fn_luu_khuon_vien(@ma, @wkt, @ten, @gc)"))
            {
                ThamSo(cmd, "@ma", DbType.String, maDonVi);
                ThamSo(cmd, "@wkt", DbType.String, wkt32648);
                ThamSo(cmd, "@ten", DbType.String, ten);
                ThamSo(cmd, "@gc", DbType.String, ghiChu);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    rd.Read();
                    return new KetQuaLuuKhuonVien
                    {
                        Id = Convert.ToInt64(rd.GetValue(0)), DienTichM2 = So(rd, 1) ?? 0,
                        ChuViM = So(rd, 2) ?? 0, ChuaTruSo = !rd.IsDBNull(3) && rd.GetBoolean(3)
                    };
                }
            }
        }

        public int XoaKhuonVien(string maDonVi)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_don_vi.fn_xoa_khuon_vien(@ma)"))
            {
                ThamSo(cmd, "@ma", DbType.String, maDonVi);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ---------- dinh tuyen (pgRouting) ----------

        public bool CoMangDuong()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT to_regproc('ql_ban_do_nen.fn_dinh_tuyen') IS NOT NULL"))
                return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public KetQuaTuyen DinhTuyen(double x1, double y1, double x2, double y2)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT wkt, dai_m, phut, so_doan, ten_duong FROM ql_ban_do_nen.fn_dinh_tuyen(@x1, @y1, @x2, @y2)"))
            {
                cmd.CommandTimeout = 60;
                ThamSo(cmd, "@x1", DbType.Double, x1); ThamSo(cmd, "@y1", DbType.Double, y1);
                ThamSo(cmd, "@x2", DbType.Double, x2); ThamSo(cmd, "@y2", DbType.Double, y2);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read() || rd.IsDBNull(0)) return null;
                    return new KetQuaTuyen
                    {
                        Wkt = Chuoi(rd, 0), DaiM = So(rd, 1) ?? 0, Phut = So(rd, 2) ?? 0,
                        SoDoan = rd.IsDBNull(3) ? 0 : Convert.ToInt32(rd.GetValue(3)), TenDuong = Chuoi(rd, 4)
                    };
                }
            }
        }

        public KetQuaTuyen DenQuanYGanNhat(double x, double y)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ma_don_vi, ten_don_vi, wkt, dai_m, phut FROM ql_ban_do_nen.fn_den_quan_y_gan_nhat(@x, @y)"))
            {
                cmd.CommandTimeout = 90;
                ThamSo(cmd, "@x", DbType.Double, x); ThamSo(cmd, "@y", DbType.Double, y);
                using (DbDataReader rd = cmd.ExecuteReader())
                {
                    if (!rd.Read() || rd.IsDBNull(2)) return null;
                    return new KetQuaTuyen
                    {
                        MaDich = Chuoi(rd, 0), TenDich = Chuoi(rd, 1), Wkt = Chuoi(rd, 2),
                        DaiM = So(rd, 3) ?? 0, Phut = So(rd, 4) ?? 0
                    };
                }
            }
        }

        public List<VungTiepCan> VungTiepCan(double x, double y, int[] phut)
        {
            List<VungTiepCan> ds = new List<VungTiepCan>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT phut, wkt, dien_tich_km2 FROM ql_ban_do_nen.fn_vung_tiep_can(@x, @y, string_to_array(@p, ',')::int[])"))
            {
                cmd.CommandTimeout = 120;
                ThamSo(cmd, "@x", DbType.Double, x); ThamSo(cmd, "@y", DbType.Double, y);
                ThamSo(cmd, "@p", DbType.String, string.Join(",", phut));
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        if (!rd.IsDBNull(1))
                            ds.Add(new VungTiepCan { Phut = Convert.ToInt32(rd.GetValue(0)), Wkt = Chuoi(rd, 1), DienTichKm2 = So(rd, 2) ?? 0 });
            }
            return ds;
        }

        // ---------- dia hinh (DEM) ----------

        public bool CoDem()
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT to_regproc('ql_ban_do_nen.fn_mat_cat') IS NOT NULL"))
                return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public double? DoCao(double x, double y)
        {
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT ql_ban_do_nen.fn_do_cao(@x, @y)"))
            {
                ThamSo(cmd, "@x", DbType.Double, x); ThamSo(cmd, "@y", DbType.Double, y);
                object o = cmd.ExecuteScalar();
                return o == null || o == DBNull.Value ? (double?)null : Convert.ToDouble(o);
            }
        }

        public List<DiemMatCat> MatCat(double x1, double y1, double x2, double y2, int soDiem)
        {
            List<DiemMatCat> ds = new List<DiemMatCat>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, "SELECT stt, khoang_cach_m, do_cao_m, x, y FROM ql_ban_do_nen.fn_mat_cat(@x1, @y1, @x2, @y2, @n)"))
            {
                cmd.CommandTimeout = 60;
                ThamSo(cmd, "@x1", DbType.Double, x1); ThamSo(cmd, "@y1", DbType.Double, y1);
                ThamSo(cmd, "@x2", DbType.Double, x2); ThamSo(cmd, "@y2", DbType.Double, y2);
                ThamSo(cmd, "@n", DbType.Int32, soDiem);
                using (DbDataReader rd = cmd.ExecuteReader())
                    while (rd.Read())
                        ds.Add(new DiemMatCat
                        {
                            Stt = Convert.ToInt32(rd.GetValue(0)), KhoangCachM = So(rd, 1) ?? 0,
                            DoCaoM = So(rd, 2), X = So(rd, 3) ?? 0, Y = So(rd, 4) ?? 0
                        });
            }
            return ds;
        }

        /// <summary>
        /// WGS84 -> VN-2000 mui 3°, KTT 105°00' (EPSG:5897, 7 tham so Bursa-Wolf cua Bo TN&amp;MT).
        /// Tra ve {X = Bac, Y = Dong} theo quy uoc trac dia Viet Nam, hoac null.
        /// </summary>
        public double[] DoiVn2000(double kinhDo, double viDo)
        {
            const string sql = @"SELECT ST_Y(g), ST_X(g) FROM (SELECT ST_Transform(ST_SetSRID(ST_MakePoint(@lon, @lat), 4326), 5897) g) s";
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            {
                ThamSo(cmd, "@lon", DbType.Double, kinhDo);
                ThamSo(cmd, "@lat", DbType.Double, viDo);
                using (DbDataReader rd = cmd.ExecuteReader())
                    return rd.Read() && !rd.IsDBNull(0) ? new[] { rd.GetDouble(0), rd.GetDouble(1) } : null;
            }
        }

        // ---------- xuat du lieu ----------

        /// <summary>FeatureCollection GeoJSON (EPSG:4326) cho mot lop: "don_vi" | "khuon_vien" | "xa_phuong".</summary>
        public string XuatGeoJson(string lop)
        {
            string sql;
            switch (lop)
            {
                case "khuon_vien":
                    sql = @"SELECT json_build_object('type','FeatureCollection','name','khuon_vien_don_vi','features', coalesce(json_agg(json_build_object(
                              'type','Feature','geometry', ST_AsGeoJSON(ST_Transform(ST_GeomFromText(wkt, 32648), 4326), 7)::json,
                              'properties', json_build_object('ma_don_vi', ma_don_vi, 'ten_don_vi', ten_don_vi, 'ten_khu_vuc', ten_khu_vuc,
                                  'dien_tich_m2', dien_tich_m2, 'chu_vi_m', chu_vi_m))), '[]'::json))::text
                            FROM ql_don_vi.v_khuon_vien_don_vi";
                    break;
                case "xa_phuong":
                    sql = @"SELECT json_build_object('type','FeatureCollection','name','xa_phuong_ha_noi','features', json_agg(json_build_object(
                              'type','Feature','geometry', ST_AsGeoJSON(ST_Transform(ST_SimplifyPreserveTopology(geom, 5), 4326), 6)::json,
                              'properties', json_build_object('ma', ma_xa_phuong, 'ten', ten_xa_phuong, 'loai', loai))))::text
                            FROM ql_don_vi.dm_xa_phuong";
                    break;
                default:
                    sql = @"SELECT json_build_object('type','FeatureCollection','name','don_vi_quan_doi','features', json_agg(json_build_object(
                              'type','Feature','geometry', ST_AsGeoJSON(b.geom_4326, 7)::json,
                              'properties', json_build_object('ma_don_vi', b.ma_don_vi, 'ten_don_vi', b.ten_don_vi,
                                  'loai_don_vi', b.loai_don_vi, 'quan_binh_chung', b.quan_binh_chung, 'phuong_xa', b.quan_huyen,
                                  'kinh_do', round(b.kinh_do::numeric, 7), 'vi_do', round(b.vi_do::numeric, 7),
                                  'phuong_phap', v.phuong_phap, 'do_chinh_xac_m', v.do_chinh_xac_m))))::text
                            FROM public.don_vi_quan_doi b
                            LEFT JOIN ql_don_vi.vi_tri_don_vi v ON v.don_vi_id = b.id AND v.la_hien_hanh";
                    break;
            }
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            {
                cmd.CommandTimeout = 120;
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        /// <summary>Danh sach (ten, mo ta, kml-geometry) de ghep tep KML.</summary>
        public List<string[]> XuatKml(string lop)
        {
            string sql;
            switch (lop)
            {
                case "khuon_vien":
                    sql = "SELECT ten_don_vi, ma_don_vi || ' · ' || dien_tich_m2 || ' m²', ST_AsKML(ST_Transform(ST_GeomFromText(wkt, 32648), 4326), 7) FROM ql_don_vi.v_khuon_vien_don_vi";
                    break;
                case "xa_phuong":
                    sql = "SELECT ten_xa_phuong, ma_xa_phuong, ST_AsKML(ST_Transform(ST_SimplifyPreserveTopology(geom, 5), 4326), 6) FROM ql_don_vi.dm_xa_phuong";
                    break;
                default:
                    sql = "SELECT ten_don_vi, ma_don_vi || ' · ' || coalesce(loai_don_vi, '') || ' · ' || coalesce(quan_huyen, ''), ST_AsKML(geom_4326, 7) FROM public.don_vi_quan_doi";
                    break;
            }
            List<string[]> ds = new List<string[]>();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
                while (rd.Read())
                    ds.Add(new[] { Chuoi(rd, 0), Chuoi(rd, 1), Chuoi(rd, 2) });
            return ds;
        }

        /// <summary>Bang (cot, gia tri, WKT 32648) de ghi Shapefile / CSV.</summary>
        public DataTable XuatBang(string lop)
        {
            string sql;
            switch (lop)
            {
                case "khuon_vien":
                    sql = "SELECT ma_don_vi, ten_don_vi, ten_khu_vuc, dien_tich_m2::float8 AS dien_tich, chu_vi_m::float8 AS chu_vi, wkt FROM ql_don_vi.v_khuon_vien_don_vi";
                    break;
                case "xa_phuong":
                    sql = "SELECT ma_xa_phuong AS ma, ten_xa_phuong AS ten, loai, ST_AsText(ST_SimplifyPreserveTopology(geom, 2)) AS wkt FROM ql_don_vi.dm_xa_phuong";
                    break;
                default:
                    sql = @"SELECT b.ma_don_vi, b.ten_don_vi, b.loai_don_vi AS loai, b.quan_binh_chung AS qbc, b.quan_huyen AS phuong_xa,
                                   b.kinh_do, b.vi_do, v.phuong_phap AS pp, v.do_chinh_xac_m::float8 AS sai_so_m, ST_AsText(b.geom_32648) AS wkt
                            FROM public.don_vi_quan_doi b LEFT JOIN ql_don_vi.vi_tri_don_vi v ON v.don_vi_id = b.id AND v.la_hien_hanh";
                    break;
            }
            DataTable t = new DataTable();
            using (DbConnection cn = KetNoiCoSoDuLieu.MoKetNoi())
            using (DbCommand cmd = Lenh(cn, sql))
            using (DbDataReader rd = cmd.ExecuteReader())
                t.Load(rd);
            return t;
        }

        // ---------- tien ich ----------

        private static DbCommand Lenh(DbConnection cn, string sql)
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

        private static string Chuoi(DbDataReader rd, int i) { return rd.IsDBNull(i) ? "" : Convert.ToString(rd.GetValue(i)); }
        private static double? So(DbDataReader rd, int i) { return rd.IsDBNull(i) ? (double?)null : Convert.ToDouble(rd.GetValue(i)); }
    }
}
