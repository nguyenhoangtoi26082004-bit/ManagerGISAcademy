
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;

using AdoConnection = System.Data.Common.DbConnection;
using AdoCommand = System.Data.Common.DbCommand;
using AdoTransaction = System.Data.Common.DbTransaction;
using AdoDataReader = System.Data.Common.DbDataReader;

namespace ManagerGISAcademy.DAL
{
    // =====================================================
    // DANH MUC BAN CHQS CAP PHUONG/XA
    // =====================================================

    public sealed class DanhMucBanChqsCapXa2026
    {
        public string MaDvhc { get; set; }
        public string TenDonVi { get; set; }
        public string LoaiDonVi { get; set; }
        public string PhuongXa { get; set; }
        public string DonViCapTren { get; set; }

        public long? XaPhuongId { get; set; }
        public string MaXaPhuong { get; set; }
        public string TenXaPhuong { get; set; }

        public string TinhTrangXacMinh { get; set; }
        public string TrangThaiLienKet { get; set; }

        // Diem dai dien hanh chinh EPSG:32648.
        public double? XDaiDien32648 { get; set; }
        public double? YDaiDien32648 { get; set; }

        // Toa do WGS84 EPSG:4326.
        public double? KinhDoDaiDien { get; set; }
        public double? ViDoDaiDien { get; set; }

        public bool HinhHocDiemHopLe { get; set; }
        public string LoaiViTri { get; set; }

        // Thong tin hop nhat 2026 (ql_don_vi.v_ban_do_don_vi).
        // Null khi CSDL chua chay bo script HopNhat_2026.
        public ThongTinBanDoHopNhat BanDo { get; set; }

        public bool DaGhepDiaBan
        {
            get
            {
                return XaPhuongId.HasValue &&
                    string.Equals(
                        TrangThaiLienKet,
                        "DA_GHEP_MA_DIA_BAN",
                        StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool CoDiemDaiDien
        {
            get
            {
                return DaGhepDiaBan &&
                    HinhHocDiemHopLe &&
                    string.Equals(
                        LoaiViTri,
                        "DAI_DIEN_DIA_BAN",
                        StringComparison.OrdinalIgnoreCase) &&
                    XDaiDien32648.HasValue &&
                    YDaiDien32648.HasValue &&
                    KinhDoDaiDien.HasValue &&
                    ViDoDaiDien.HasValue &&
                    SoHuuHan(XDaiDien32648.Value) &&
                    SoHuuHan(YDaiDien32648.Value) &&
                    SoHuuHan(KinhDoDaiDien.Value) &&
                    SoHuuHan(ViDoDaiDien.Value) &&
                    KinhDoDaiDien.Value >= -180d &&
                    KinhDoDaiDien.Value <= 180d &&
                    ViDoDaiDien.Value >= -90d &&
                    ViDoDaiDien.Value <= 90d;
            }
        }

        private static bool SoHuuHan(double d)
        {
            return !double.IsNaN(d) &&
                   !double.IsInfinity(d);
        }
    }

    // =====================================================
    // THONG TIN BAN DO HOP NHAT 2026
    //
    // Logo theo loai, vi tri ve (da gian cach khi trung diem)
    // va do tin cay vi tri. Toa do VE chi dung de hien thi,
    // khong dung de do dac.
    // =====================================================

    public sealed class ThongTinBanDoHopNhat
    {
        public string TepAnhLogo { get; set; }
        public int? KichThuocLogoPx { get; set; }

        public bool LaViTriThuc { get; set; }
        public double? DoChinhXacM { get; set; }
        public string TenPhuongPhap { get; set; }

        public double? KinhDoVe { get; set; }
        public double? ViDoVe { get; set; }

        // Vi tri HIEN HANH that (vi_tri_don_vi), dung de do dac / hien thi.
        public double? KinhDoThuc { get; set; }
        public double? ViDoThuc { get; set; }

        public string MoTaViTri
        {
            get
            {
                string pp = string.IsNullOrWhiteSpace(TenPhuongPhap)
                    ? "Chưa rõ phương pháp"
                    : TenPhuongPhap.Trim();

                if (!LaViTriThuc)
                    return "Điểm đại diện địa bàn, chưa phải vị trí trụ sở (" +
                           pp + ").";

                return DoChinhXacM.HasValue
                    ? "Vị trí: " + pp + ", độ chính xác khoảng " +
                      DoChinhXacM.Value.ToString(
                          "0.##", CultureInfo.GetCultureInfo("vi-VN")) +
                      " m."
                    : "Vị trí: " + pp + ".";
            }
        }
    }

    // =====================================================
    // HO SO GIS CAC CAP TRONG DATABASE HQTCSDL
    // =====================================================

    public sealed class HoSoGisHqtc
    {
        public long Id { get; set; }

        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }

        public string TenCap { get; set; }
        public string TenLoai { get; set; }

        public string MaXaPhuong { get; set; }
        public string TenXaPhuong { get; set; }
        public string LoaiXaPhuong { get; set; }

        public double? X32648 { get; set; }
        public double? Y32648 { get; set; }

        public double? KinhDo { get; set; }
        public double? ViDo { get; set; }

        public bool HinhHocHopLe { get; set; }

        // Thong tin hop nhat 2026 (ql_don_vi.v_ban_do_don_vi).
        // Null khi CSDL chua chay bo script HopNhat_2026.
        public ThongTinBanDoHopNhat BanDo { get; set; }

        // Dia ban cua 43 ho so GIS trong HQTCSDL.
        // Gia tri "loai_xa_phuong" luu o CSDL co the KHONG dau:
        //   phuong, xa; hoac co dau: Phuong/Xa (Phường/Xã).
        // Chi dinh dang khi hien thi, khong cap nhat du lieu goc.
        public string DiaBan
        {
            get
            {
                string ten = (TenXaPhuong ?? "").Trim();

                if (ten.Length == 0)
                    return "Chưa xác định phường/xã";

                // Ten da co tien to dung thi giu nguyen.
                if (ten.StartsWith("Phường ", StringComparison.OrdinalIgnoreCase) ||
                    ten.StartsWith("Xã ", StringComparison.OrdinalIgnoreCase))
                    return ten;

                // Neu ten luu khong dau kem tien to, chuan hoa de khong lap.
                if (ten.StartsWith("Phuong ", StringComparison.OrdinalIgnoreCase))
                    return "Phường " + ten.Substring("Phuong ".Length).TrimStart();

                if (ten.StartsWith("Xa ", StringComparison.OrdinalIgnoreCase))
                    return "Xã " + ten.Substring("Xa ".Length).TrimStart();

                string loai = (LoaiXaPhuong ?? "").Trim().ToLowerInvariant();

                // QUAN TRONG: pgAdmin cua anh cho thay gia tri la 'phuong'.
                if (loai == "phuong" || loai == "phường" ||
                    loai == "p" || loai == "p.")
                    return "Phường " + ten;

                if (loai == "xa" || loai == "xã" ||
                    loai == "x" || loai == "x.")
                    return "Xã " + ten;

                // Du phong theo quy uoc ma dia ban da co trong HQTCSDL:
                // HN-Pxxx = phuong; HN-Xxxx = xa.
                string ma = (MaXaPhuong ?? "").Trim();

                if (ma.StartsWith("HN-P", StringComparison.OrdinalIgnoreCase))
                    return "Phường " + ten;

                if (ma.StartsWith("HN-X", StringComparison.OrdinalIgnoreCase))
                    return "Xã " + ten;

                // Neu khong co chung cu xac dinh loai, KHONG tu gan nham.
                return ten;
            }
        }

        public bool CoDiemBanDo
        {
            get
            {
                return HinhHocHopLe &&
                    X32648.HasValue &&
                    Y32648.HasValue &&
                    !double.IsNaN(X32648.Value) &&
                    !double.IsInfinity(X32648.Value) &&
                    !double.IsNaN(Y32648.Value) &&
                    !double.IsInfinity(Y32648.Value);
            }
        }
    }

    // =====================================================
    // DATA ACCESS LAYER
    // =====================================================

    public sealed class TruyXuatDonVi :
        ITruyXuatDonViQuanDoi
    {
        private const string TenBang =
            "public.don_vi_quan_doi";

        private const string CotDuLieu = @"
            u.phien_ban,  -- HQTCSDL: view tuong thich tinh tu xmin cac bang goc
            u.ma_don_vi,
            u.ten_don_vi,
            u.ten_viet_tat,
            u.loai_don_vi,
            u.quan_binh_chung,
            u.don_vi_cap_tren,
            u.quan_huyen,
            u.dia_chi,
            u.kinh_do,
            u.vi_do,
            u.x_vn2000,
            u.y_vn2000,
            u.chi_huy_truong,
            u.chinh_uy,
            u.so_dien_thoai,
            u.trang_thai_sscd,
            u.do_mat,
            u.nhiem_vu_chinh,
            u.vu_khi_trang_bi,

            ST_AsText(u.geom_4326) AS wkt,
            ST_AsText(u.geom_32648) AS wkt_32648,

            ST_SRID(u.geom_4326) AS srid_4326,
            ST_IsValid(u.geom_4326) AS valid_4326,
            ST_IsEmpty(u.geom_4326) AS empty_4326,
            ST_GeometryType(u.geom_4326) AS type_4326,

            ST_SRID(u.geom_32648) AS srid_32648,
            ST_IsValid(u.geom_32648) AS valid_32648,
            ST_IsEmpty(u.geom_32648) AS empty_32648,
            ST_GeometryType(u.geom_32648) AS type_32648";

        private readonly string chuoiKetNoi;
        private readonly DbProviderFactory nhaCungCap;

        // =========================================
        // 1. KHOI TAO
        // =========================================

        public TruyXuatDonVi()
            : this(
                KetNoiCoSoDuLieu.LayChuoiKetNoi(),
                KetNoiCoSoDuLieu.LayNhaCungCapNpgsql())
        {
        }

        public TruyXuatDonVi(
            string connectionString)
            : this(
                connectionString,
                KetNoiCoSoDuLieu.LayNhaCungCapNpgsql())
        {
        }

        public TruyXuatDonVi(
            string connectionString,
            DbProviderFactory factory)
        {
            if (string.IsNullOrWhiteSpace(
                connectionString))
            {
                throw new ArgumentException(
                    "Chuỗi kết nối PostGIS không được trống.",
                    "connectionString");
            }

            if (factory == null)
            {
                throw new ArgumentNullException(
                    "factory");
            }

            chuoiKetNoi = connectionString.Trim();
            nhaCungCap = factory;
        }

        // =========================================
        // 2. KET NOI HO SO GIS CHINH
        // =========================================

        private AdoConnection MoKetNoi()
        {
            AdoConnection cn =
                nhaCungCap.CreateConnection();

            if (cn == null)
            {
                throw new InvalidOperationException(
                    "Npgsql không tạo được kết nối.");
            }

            try
            {
                cn.ConnectionString = chuoiKetNoi;
                cn.Open();
                KetNoiCoSoDuLieu.ApDungVaiTro(cn);
                return cn;
            }
            catch
            {
                cn.Dispose();
                throw;
            }
        }

        // =========================================
        // 3. DOC DANH SACH HO SO CHINH
        // =========================================

        public IList<DonViQuanDoi> LayTatCaDonVi()
        {
            List<DonViQuanDoi> ketQua =
                new List<DonViQuanDoi>();

            using (AdoConnection cn = MoKetNoi())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT " + CotDuLieu +
                    " FROM " + TenBang +
                    " AS u ORDER BY u.ma_don_vi";

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                using (AdoDataReader reader =
                    cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ketQua.Add(
                            ChuyenTuBanGhi(reader));
                    }
                }
            }

            KiemTraRanhGioiHaNoi(ketQua);

            return ketQua;
        }

        // =========================================
        // 4. DOC 126 BAN CHQS VA DIEM DAI DIEN
        // =========================================

        public IList<DanhMucBanChqsCapXa2026>
            LayDanhMucBanChqsCapXa()
        {
            // Uu tien doc kem logo / vi tri ve tu CSDL hop nhat 2026.
            // CSDL chua hop nhat (thieu view) thi doc nhu cu.
            string sqlHopNhat = SqlBchqs
                .Replace(ChoCotHopNhat, "," + CotHopNhat("m"))
                .Replace(ChoJoinHopNhat,
                    "LEFT JOIN ql_don_vi.v_ban_do_don_vi AS m " +
                    "ON m.ma_don_vi = 'HN-BCHQS-' || btrim(b.ma_dvhc::text)");

            try
            {
                return DocDanhMucBchqs(sqlHopNhat);
            }
            catch (DbException ex) when (LaLoiThieuDoiTuongCsdl(ex))
            {
                return DocDanhMucBchqs(SqlBchqs);
            }
        }

        private const string SqlBchqs = @"
                SELECT
                    b.ma_dvhc,
                    b.ten_don_vi,
                    b.loai_don_vi,
                    b.phuong_xa,
                    b.don_vi_cap_tren,
                    b.xa_phuong_id,
                    b.ma_xa_phuong,
                    b.ten_xa_phuong,
                    b.tinh_trang_xac_minh,
                    b.trang_thai_lien_ket,

                    p.x_32648 AS diem_x,
                    p.y_32648 AS diem_y,
                    p.kinh_do AS diem_kinh_do,
                    p.vi_do AS diem_vi_do,
                    p.loai_vi_tri AS diem_loai,

                    CASE
                        WHEN p.geom IS NOT NULL
                         AND NOT ST_IsEmpty(p.geom)
                         AND ST_IsValid(p.geom)
                         AND ST_SRID(p.geom) = 32648
                         AND ST_GeometryType(p.geom) =
                             'ST_Point'
                        THEN TRUE
                        ELSE FALSE
                    END AS diem_hop_le
                    /*COT_HOP_NHAT*/

                FROM ql_don_vi.v_bchqs_cap_xa_2026 AS b

                LEFT JOIN
                    ql_don_vi.v_bchqs_diem_dai_dien_2026 AS p
                    ON b.ma_dvhc = p.ma_dvhc

                /*JOIN_HOP_NHAT*/

                ORDER BY
                    b.ten_don_vi,
                    b.ma_dvhc";

        private IList<DanhMucBanChqsCapXa2026>
            DocDanhMucBchqs(string sql)
        {
            List<DanhMucBanChqsCapXa2026> items =
                new List<DanhMucBanChqsCapXa2026>();

            using (AdoConnection cn =
                KetNoiCoSoDuLieu.MoKetNoiDanhMuc())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 45;

                using (AdoDataReader rd =
                    cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int i =
                            rd.GetOrdinal("xa_phuong_id");

                        DanhMucBanChqsCapXa2026 d =
                            new DanhMucBanChqsCapXa2026
                            {
                                MaDvhc =
                                    DocChuoi(
                                        rd,
                                        "ma_dvhc"),

                                TenDonVi =
                                    DocChuoi(
                                        rd,
                                        "ten_don_vi"),

                                LoaiDonVi =
                                    DocChuoi(
                                        rd,
                                        "loai_don_vi"),

                                PhuongXa =
                                    DocChuoi(
                                        rd,
                                        "phuong_xa"),

                                DonViCapTren =
                                    DocChuoi(
                                        rd,
                                        "don_vi_cap_tren"),

                                XaPhuongId =
                                    rd.IsDBNull(i)
                                    ? (long?)null
                                    : Convert.ToInt64(
                                        rd.GetValue(i)),

                                MaXaPhuong =
                                    DocChuoi(
                                        rd,
                                        "ma_xa_phuong"),

                                TenXaPhuong =
                                    DocChuoi(
                                        rd,
                                        "ten_xa_phuong"),

                                TinhTrangXacMinh =
                                    DocChuoi(
                                        rd,
                                        "tinh_trang_xac_minh"),

                                TrangThaiLienKet =
                                    DocChuoi(
                                        rd,
                                        "trang_thai_lien_ket"),

                                XDaiDien32648 =
                                    DocSoThucNullable(
                                        rd,
                                        "diem_x"),

                                YDaiDien32648 =
                                    DocSoThucNullable(
                                        rd,
                                        "diem_y"),

                                KinhDoDaiDien =
                                    DocSoThucNullable(
                                        rd,
                                        "diem_kinh_do"),

                                ViDoDaiDien =
                                    DocSoThucNullable(
                                        rd,
                                        "diem_vi_do"),

                                LoaiViTri =
                                    DocChuoi(
                                        rd,
                                        "diem_loai"),

                                HinhHocDiemHopLe =
                                    !rd.IsDBNull(
                                        rd.GetOrdinal(
                                            "diem_hop_le")) &&
                                    Convert.ToBoolean(
                                        rd["diem_hop_le"])
                            };

                        d.BanDo = DocThongTinHopNhat(rd);

                        items.Add(d);
                    }
                }
            }

            return items;
        }

        // =========================================
        // 5. DOC 43 HO SO GIS CAC CAP
        // =========================================

        public IList<HoSoGisHqtc> LayHoSoGisHqtc()
        {
            // CSDL hop nhat 2026: doc tu view ban do thong nhat,
            // bo Ban CHQS xa (da co lop rieng). Thieu view -> doc nhu cu.
            string sqlHopNhat = SqlHoSoGis
                .Replace(ChoCotHopNhat, "," + CotHopNhat("v"))
                .Replace(
                    "FROM ql_don_vi.v_don_vi_ban_do AS v",
                    "FROM ql_don_vi.v_ban_do_don_vi AS v " +
                    "WHERE v.ma_loai IS DISTINCT FROM 'BCHQS_XA'");

            try
            {
                return DocHoSoGisHqtc(sqlHopNhat);
            }
            catch (DbException ex) when (LaLoiThieuDoiTuongCsdl(ex))
            {
                return DocHoSoGisHqtc(SqlHoSoGis);
            }
        }

        private const string SqlHoSoGis = @"
                SELECT
                    v.id,
                    v.ma_don_vi,
                    v.ten_don_vi,
                    v.ten_cap,
                    v.ten_loai,
                    v.ma_xa_phuong,
                    v.ten_xa_phuong,
                    v.loai_xa_phuong,

                    CASE
                        WHEN v.geom IS NOT NULL
                         AND NOT ST_IsEmpty(v.geom)
                         AND ST_IsValid(v.geom)
                         AND ST_SRID(v.geom) = 32648
                         AND ST_GeometryType(v.geom) =
                             'ST_Point'

                        THEN ST_X(v.geom)
                        ELSE NULL
                    END AS x_32648,

                    CASE
                        WHEN v.geom IS NOT NULL
                         AND NOT ST_IsEmpty(v.geom)
                         AND ST_IsValid(v.geom)
                         AND ST_SRID(v.geom) = 32648
                         AND ST_GeometryType(v.geom) =
                             'ST_Point'
                        THEN ST_Y(v.geom)
                        ELSE NULL
                    END AS y_32648,

                    CASE
                        WHEN v.geom IS NOT NULL
                         AND NOT ST_IsEmpty(v.geom)
                         AND ST_IsValid(v.geom)
                         AND ST_SRID(v.geom) = 32648
                         AND ST_GeometryType(v.geom) =
                             'ST_Point'
                        THEN ST_X(
                            ST_Transform(v.geom, 4326))
                        ELSE NULL
                    END AS kinh_do,

                    CASE
                        WHEN v.geom IS NOT NULL
                         AND NOT ST_IsEmpty(v.geom)
                         AND ST_IsValid(v.geom)
                         AND ST_SRID(v.geom) = 32648
                         AND ST_GeometryType(v.geom) =
                             'ST_Point'
                        THEN ST_Y(
                            ST_Transform(v.geom, 4326))
                        ELSE NULL
                    END AS vi_do,

                    CASE
                        WHEN v.geom IS NOT NULL
                         AND NOT ST_IsEmpty(v.geom)
                         AND ST_IsValid(v.geom)
                         AND ST_SRID(v.geom) = 32648
                         AND ST_GeometryType(v.geom) =
                             'ST_Point'
                        THEN TRUE
                        ELSE FALSE
                    END AS hinh_hoc_hop_le
                    /*COT_HOP_NHAT*/

                FROM ql_don_vi.v_don_vi_ban_do AS v

                ORDER BY
                    v.ten_don_vi,
                    v.id";

        private IList<HoSoGisHqtc> DocHoSoGisHqtc(string sql)
        {
            List<HoSoGisHqtc> ketQua =
                new List<HoSoGisHqtc>();

            using (AdoConnection cn =
                KetNoiCoSoDuLieu.MoKetNoiDanhMuc())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 45;

                using (AdoDataReader rd =
                    cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int idColumn =
                            rd.GetOrdinal("id");

                        if (rd.IsDBNull(idColumn))
                            continue;

                        HoSoGisHqtc d =
                            new HoSoGisHqtc
                            {
                                Id = Convert.ToInt64(
                                    rd.GetValue(idColumn)),

                                MaDonVi =
                                    DocChuoi(
                                        rd,
                                        "ma_don_vi"),

                                TenDonVi =
                                    DocChuoi(
                                        rd,
                                        "ten_don_vi"),

                                TenCap =
                                    DocChuoi(
                                        rd,
                                        "ten_cap"),

                                TenLoai =
                                    DocChuoi(
                                        rd,
                                        "ten_loai"),

                                MaXaPhuong =
                                    DocChuoi(
                                        rd,
                                        "ma_xa_phuong"),

                                TenXaPhuong =
                                    DocChuoi(
                                        rd,
                                        "ten_xa_phuong"),

                                LoaiXaPhuong =
                                    DocChuoi(
                                        rd,
                                        "loai_xa_phuong"),

                                X32648 =
                                    DocSoThucNullable(
                                        rd,
                                        "x_32648"),

                                Y32648 =
                                    DocSoThucNullable(
                                        rd,
                                        "y_32648"),

                                KinhDo =
                                    DocSoThucNullable(
                                        rd,
                                        "kinh_do"),

                                ViDo =
                                    DocSoThucNullable(
                                        rd,
                                        "vi_do"),

                                HinhHocHopLe =
                                    !rd.IsDBNull(
                                        rd.GetOrdinal(
                                            "hinh_hoc_hop_le")) &&
                                    Convert.ToBoolean(
                                        rd["hinh_hoc_hop_le"])
                            };

                        d.BanDo = DocThongTinHopNhat(rd);

                        ketQua.Add(d);
                    }
                }
            }

            return ketQua;
        }

        // =========================================
        // 5B. COT BAN DO HOP NHAT 2026 (DUNG CHUNG)
        // =========================================

        private const string ChoCotHopNhat = "/*COT_HOP_NHAT*/";
        private const string ChoJoinHopNhat = "/*JOIN_HOP_NHAT*/";

        private static string CotHopNhat(string bi)
        {
            // geom_hien_thi chi de VE (da gian cach khi trung diem).
            return @"
                    " + bi + @".tep_anh            AS hn_tep_anh,
                    " + bi + @".kich_thuoc_px      AS hn_kich_thuoc_px,
                    " + bi + @".la_vi_tri_thuc     AS hn_la_vi_tri_thuc,
                    " + bi + @".do_chinh_xac_m::float8 AS hn_do_chinh_xac_m,
                    " + bi + @".ten_phuong_phap    AS hn_ten_phuong_phap,
                    ST_X(ST_Transform(" + bi + @".geom_hien_thi, 4326)) AS hn_kinh_do_ve,
                    ST_Y(ST_Transform(" + bi + @".geom_hien_thi, 4326)) AS hn_vi_do_ve,
                    " + bi + @".kinh_do              AS hn_kinh_do_thuc,
                    " + bi + @".vi_do                AS hn_vi_do_thuc";
        }

        private static bool CoCot(AdoDataReader rd, string ten)
        {
            for (int i = 0; i < rd.FieldCount; i++)
            {
                if (string.Equals(rd.GetName(i), ten,
                    StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private static ThongTinBanDoHopNhat DocThongTinHopNhat(
            AdoDataReader rd)
        {
            if (!CoCot(rd, "hn_tep_anh"))
                return null;

            string tep = DocChuoi(rd, "hn_tep_anh");

            // LEFT JOIN khong khop: don vi chua co trong CSDL hop nhat.
            if (string.IsNullOrWhiteSpace(tep))
                return null;

            int iKichThuoc = rd.GetOrdinal("hn_kich_thuoc_px");
            int iThuc = rd.GetOrdinal("hn_la_vi_tri_thuc");

            return new ThongTinBanDoHopNhat
            {
                TepAnhLogo = tep.Trim(),
                KichThuocLogoPx = rd.IsDBNull(iKichThuoc)
                    ? (int?)null
                    : Convert.ToInt32(rd.GetValue(iKichThuoc)),
                LaViTriThuc = !rd.IsDBNull(iThuc) &&
                    Convert.ToBoolean(rd.GetValue(iThuc)),
                DoChinhXacM = DocSoThucNullable(rd, "hn_do_chinh_xac_m"),
                TenPhuongPhap = DocChuoi(rd, "hn_ten_phuong_phap"),
                KinhDoVe = DocSoThucNullable(rd, "hn_kinh_do_ve"),
                ViDoVe = DocSoThucNullable(rd, "hn_vi_do_ve"),
                KinhDoThuc = DocSoThucNullable(rd, "hn_kinh_do_thuc"),
                ViDoThuc = DocSoThucNullable(rd, "hn_vi_do_thuc")
            };
        }

        /// <summary>
        /// 42P01: chua co bang/view; 42703: chua co cot.
        /// Dung de lui ve truy van cu khi CSDL chua hop nhat.
        /// Doc SqlState qua reflection de khong phu thuoc kieu Npgsql.
        /// </summary>
        private static bool LaLoiThieuDoiTuongCsdl(DbException ex)
        {
            PropertyInfo p = ex.GetType().GetProperty("SqlState");
            string ma = p == null ? null : p.GetValue(ex, null) as string;

            return ma == "42P01" || ma == "42703";
        }

        // =========================================
        // 6. DOC HO SO CU THEO MA
        // =========================================

        public DonViQuanDoi LayTheoMa(
            string maDonVi)
        {
            string ma = KiemTraMa(maDonVi);

            using (AdoConnection cn = MoKetNoi())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT " + CotDuLieu +
                    " FROM " + TenBang +
                    " AS u " +
                    "WHERE u.ma_don_vi = @ma LIMIT 1";

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                ThemThamSo(
                    cmd,
                    "@ma",
                    DbType.String,
                    ma);

                using (AdoDataReader reader =
                    cmd.ExecuteReader())
                {
                    return reader.Read()
                        ? ChuyenTuBanGhi(reader)
                        : null;
                }
            }
        }

        // =========================================
        // 7. THEM DON VI GIS CU
        // =========================================

        public bool ThemDonVi(
            DonViQuanDoi donVi)
        {
            KiemTraDuLieuGhi(donVi);

            const string sql = @"
                INSERT INTO public.don_vi_quan_doi
                (
                    ma_don_vi,
                    ten_don_vi,
                    ten_viet_tat,
                    loai_don_vi,
                    quan_binh_chung,
                    don_vi_cap_tren,
                    quan_huyen,
                    dia_chi,
                    kinh_do,
                    vi_do,
                    x_vn2000,
                    y_vn2000,
                    chi_huy_truong,
                    chinh_uy,
                    so_dien_thoai,
                    trang_thai_sscd,
                    do_mat,
                    nhiem_vu_chinh,
                    vu_khi_trang_bi,
                    geom_4326,
                    geom_32648
                )
                VALUES
                (
                    @ma,
                    @ten,
                    @ten_tat,
                    @loai,
                    @binh_chung,
                    @cap_tren,
                    @quan_huyen,
                    @dia_chi,
                    @kinh_do,
                    @vi_do,

                    ST_X(
                        ST_Transform(
                            ST_SetSRID(
                                ST_MakePoint(
                                    @kinh_do,
                                    @vi_do),
                                4326),
                            32648)
                    ),

                    ST_Y(
                        ST_Transform(
                            ST_SetSRID(
                                ST_MakePoint(
                                    @kinh_do,
                                    @vi_do),
                                4326),
                            32648)
                    ),

                    @chi_huy,

                    @chinh_uy,
                    @dien_thoai,
                    @sscd,
                    @do_mat,
                    @nhiem_vu,
                    @trang_bi,

                    ST_SetSRID(
                        ST_MakePoint(
                            @kinh_do,
                            @vi_do),
                        4326),

                    ST_Transform(
                        ST_SetSRID(
                            ST_MakePoint(
                                @kinh_do,
                                @vi_do),
                            4326),
                        32648)
                )";

            using (AdoConnection cn = MoKetNoi())
            using (AdoTransaction tx =
                cn.BeginTransaction())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                GanThamSoDonVi(cmd, donVi);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
        }

        // =========================================
        // 8. SUA DON VI GIS CU
        // =========================================

        public bool SuaDonVi(
            DonViQuanDoi donVi)
        {
            KiemTraDuLieuGhi(donVi);

            const string sql = @"
                UPDATE public.don_vi_quan_doi
                SET
                    ten_don_vi = @ten,
                    ten_viet_tat = @ten_tat,
                    loai_don_vi = @loai,
                    quan_binh_chung = @binh_chung,
                    don_vi_cap_tren = @cap_tren,
                    quan_huyen = @quan_huyen,
                    dia_chi = @dia_chi,
                    kinh_do = @kinh_do,
                    vi_do = @vi_do,

                    x_vn2000 = ST_X(
                        ST_Transform(
                            ST_SetSRID(
                                ST_MakePoint(
                                    @kinh_do,
                                    @vi_do),
                                4326),
                            32648)
                    ),

                    y_vn2000 = ST_Y(
                        ST_Transform(
                            ST_SetSRID(
                                ST_MakePoint(
                                    @kinh_do,
                                    @vi_do),
                                4326),
                            32648)
                    ),

                    chi_huy_truong = @chi_huy,
                    chinh_uy = @chinh_uy,
                    so_dien_thoai = @dien_thoai,
                    trang_thai_sscd = @sscd,
                    do_mat = @do_mat,
                    nhiem_vu_chinh = @nhiem_vu,
                    vu_khi_trang_bi = @trang_bi,

                    geom_4326 = ST_SetSRID(
                        ST_MakePoint(
                            @kinh_do,
                            @vi_do),
                        4326),

                    geom_32648 = ST_Transform(
                        ST_SetSRID(
                            ST_MakePoint(
                                @kinh_do,
                                @vi_do),
                            4326),
                        32648),

                    ngay_cap_nhat = CURRENT_TIMESTAMP

                WHERE ma_don_vi = @ma
                  -- Khoa lac quan: phien_ban (xmin cac bang goc)
                  -- doi sau moi lan ghi. Nguoi khac da sua -> 0 dong.
                  AND (
                      CAST(@phien_ban AS text) IS NULL
                      OR phien_ban = CAST(@phien_ban AS text)
                  )";

            using (AdoConnection cn = MoKetNoi())
            using (AdoTransaction tx =
                cn.BeginTransaction())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                GanThamSoDonVi(cmd, donVi);

                ThemThamSo(
                    cmd,
                    "@phien_ban",
                    DbType.String,
                    RongThanhNull(donVi.PhienBan));

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
        }

        // =========================================
        // 9. XOA DON VI GIS CU
        // =========================================

        public bool XoaDonVi(
            string maDonVi)
        {
            string ma = KiemTraMa(maDonVi);

            using (AdoConnection cn = MoKetNoi())
            using (AdoTransaction tx =
                cn.BeginTransaction())
            using (AdoCommand cmd = cn.CreateCommand())
            {
                cmd.Transaction = tx;

                cmd.CommandText =
                    "DELETE FROM " + TenBang +
                    " WHERE ma_don_vi = @ma";

                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 30;

                ThemThamSo(
                    cmd,
                    "@ma",
                    DbType.String,
                    ma);

                if (cmd.ExecuteNonQuery() != 1)
                {
                    tx.Rollback();
                    return false;
                }

                tx.Commit();
                return true;
            }
        }

        // =========================================
        // 10. CHUYEN DATAREADER THANH DTO
        // =========================================

        private static DonViQuanDoi ChuyenTuBanGhi(
            AdoDataReader doc)
        {
            DonViQuanDoi dv = new DonViQuanDoi
            {
                MaDonVi =
                    DocChuoi(doc, "ma_don_vi"),

                TenDonVi =
                    DocChuoi(doc, "ten_don_vi"),

                TenVietTat =
                    DocChuoi(doc, "ten_viet_tat"),

                LoaiDonVi =
                    DocChuoi(doc, "loai_don_vi"),

                QuanBinhChung =
                    DocChuoi(doc, "quan_binh_chung"),

                DonViCapTren =
                    DocChuoi(doc, "don_vi_cap_tren"),

                QuanHuyen =
                    DocChuoi(doc, "quan_huyen"),

                DiaChi =
                    DocChuoi(doc, "dia_chi"),

                KinhDo =
                    DocSo(doc, "kinh_do"),

                ViDo =
                    DocSo(doc, "vi_do"),

                XVn2000 =
                    DocSo(doc, "x_vn2000"),

                YVn2000 =
                    DocSo(doc, "y_vn2000"),

                ChiHuyTruong =
                    DocChuoi(doc, "chi_huy_truong"),

                ChinhUy =
                    DocChuoi(doc, "chinh_uy"),

                SoDienThoai =
                    DocChuoi(doc, "so_dien_thoai"),

                TrangThaiSSCD =
                    DocChuoi(doc, "trang_thai_sscd"),

                DoMat =
                    DocChuoi(doc, "do_mat"),

                NhiemVuChinh =
                    TachDanhSach(
                        DocChuoi(doc, "nhiem_vu_chinh")),

                VuKhiTrangBi =
                    TachDanhSach(
                        DocChuoi(doc, "vu_khi_trang_bi")),

                Wkt =
                    DocChuoi(doc, "wkt"),

                PhienBan =
                    DocChuoi(doc, "phien_ban")
            };

            KiemTraHinhHoc(dv, doc);

            return dv;
        }

        // =========================================
        // 11. KIEM TRA HINH HOC
        // =========================================

        private static void KiemTraHinhHoc(
            DonViQuanDoi dv,
            AdoDataReader doc)

        {
            int? srid =
                DocSoNguyenNullable(
                    doc, "srid_4326");

            bool? hopLe =
                DocBoolNullable(
                    doc, "valid_4326");

            bool? rong =
                DocBoolNullable(
                    doc, "empty_4326");

            string loai =
                DocChuoi(
                    doc, "type_4326");

            if (srid.HasValue &&
                srid.Value != 4326)
            {
                ThemCanhBao(
                    dv,
                    "Hình học geom_4326 có SRID = " +
                    srid.Value +
                    ", yêu cầu EPSG:4326.");
            }

            if (hopLe.HasValue && !hopLe.Value)
            {
                ThemCanhBao(
                    dv,
                    "Hình học geom_4326 không hợp lệ.");
            }

            if (rong.HasValue && rong.Value)
            {
                ThemCanhBao(
                    dv,
                    "Hình học geom_4326 rỗng.");
            }

            string wkt32648 =
                DocChuoi(
                    doc, "wkt_32648");

            int? sridPhang =
                DocSoNguyenNullable(
                    doc, "srid_32648");

            bool? validPhang =
                DocBoolNullable(
                    doc, "valid_32648");

            bool? rongPhang =
                DocBoolNullable(
                    doc, "empty_32648");

            string loaiPhang =
                DocChuoi(
                    doc, "type_32648");

            if (string.IsNullOrWhiteSpace(wkt32648))
            {
                ThemCanhBao(
                    dv,
                    "Thiếu hình học geom_32648 trong PostGIS.");
            }
            else
            {
                if (sridPhang.HasValue &&
                    sridPhang.Value != 32648)
                {
                    ThemCanhBao(
                        dv,
                        "Hình học geom_32648 có SRID = " +
                        sridPhang.Value +
                        ", yêu cầu EPSG:32648.");
                }

                if (validPhang.HasValue &&
                    !validPhang.Value)
                {
                    ThemCanhBao(
                        dv,
                        "Hình học geom_32648 không hợp lệ.");
                }

                if (rongPhang.HasValue &&
                    rongPhang.Value)
                {
                    ThemCanhBao(
                        dv,
                        "Hình học geom_32648 rỗng.");
                }

                if (!string.Equals(
                    loaiPhang,
                    "ST_Point",
                    StringComparison.OrdinalIgnoreCase))
                {
                    ThemCanhBao(
                        dv,
                        "Hình học geom_32648 " +
                        "không phải kiểu POINT.");
                }
            }

            double x;
            double y;

            if (srid == 4326 &&
                hopLe == true &&
                rong == false &&
                string.Equals(
                    loai,
                    "ST_Point",
                    StringComparison.OrdinalIgnoreCase) &&
                ThuToaDoPoint(
                    dv.Wkt,
                    out x,
                    out y) &&
                CoToaDoWgs84(
                    dv.KinhDo,
                    dv.ViDo))
            {
                double saiLech = KhoangCachMet(
                    x,
                    y,
                    dv.KinhDo,
                    dv.ViDo);

                if (saiLech > 2.0)
                {
                    ThemCanhBao(
                        dv,
                        "Tọa độ thuộc tính và geom_4326 lệch " +
                        saiLech.ToString(
                            "N1",
                            CultureInfo.GetCultureInfo(
                                "vi-VN")) +
                        " m.");
                }
            }

            if (sridPhang == 32648 &&
                validPhang == true &&
                rongPhang == false &&
                string.Equals(
                    loaiPhang,
                    "ST_Point",
                    StringComparison.OrdinalIgnoreCase) &&
                ThuToaDoPoint(
                    wkt32648,
                    out x,
                    out y))
            {
                if (dv.XVn2000 == 0d &&
                    dv.YVn2000 == 0d)
                {
                    ThemCanhBao(
                        dv,
                        "Thiếu tọa độ phẳng X/Y " +
                        "mặc dù có geom_32648.");
                }
                else if (
                    LaSoHuuHan(dv.XVn2000) &&
                    LaSoHuuHan(dv.YVn2000))
                {
                    double saiLech = Math.Sqrt(
                        Math.Pow(
                            x - dv.XVn2000, 2) +
                        Math.Pow(
                            y - dv.YVn2000, 2));

                    if (saiLech > 2.0)
                    {
                        ThemCanhBao(
                            dv,
                            "Tọa độ X/Y và geom_32648 lệch " +
                            saiLech.ToString(
                                "N1",
                                CultureInfo.GetCultureInfo(
                                    "vi-VN")) +
                            " m.");
                    }
                }
            }
        }

        private static void ThemCanhBao(
            DonViQuanDoi dv,
            string noiDung)
        {
            if (dv.CanhBaoKhongGian == null)
            {
                dv.CanhBaoKhongGian =
                    new List<string>();
            }

            if (!dv.CanhBaoKhongGian.Contains(
                noiDung))
            {
                dv.CanhBaoKhongGian.Add(noiDung);
            }
        }

        // =========================================
        // 12. KIEM TRA RANH GIOI HA NOI
        // =========================================

        private void KiemTraRanhGioiHaNoi(
            IList<DonViQuanDoi> danhSach)
        {
            if (danhSach == null ||
                danhSach.Count == 0)
            {
                return;
            }

            try
            {
                using (AdoConnection cn = MoKetNoi())
                {
                    using (AdoCommand cmd =
                        cn.CreateCommand())
                    {
                        cmd.CommandText =
                            "SELECT to_regclass(" +
                            "'public.ranh_gioi_ha_noi')" +
                            " IS NOT NULL";

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;

                        if (!Convert.ToBoolean(
                            cmd.ExecuteScalar()))
                        {
                            ThemCanhBaoChoDanhSach(
                                danhSach,
                                "Chưa kiểm tra tọa độ trong " +
                                "ranh giới Hà Nội: thiếu bảng " +
                                "public.ranh_gioi_ha_noi.");

                            return;
                        }
                    }

                    long soRanhGioi;
                    long soLoi;

                    using (AdoCommand cmd =
                        cn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT
                                COUNT(*) AS tong,
                                COUNT(*) FILTER (
                                    WHERE geom IS NULL
                                       OR ST_IsEmpty(geom)
                                       OR NOT ST_IsValid(geom)

                                       OR ST_SRID(geom) <> 32648
                                       OR ST_GeometryType(geom)
                                           NOT IN (
                                               'ST_Polygon',
                                               'ST_MultiPolygon'
                                           )
                                ) AS so_loi
                            FROM public.ranh_gioi_ha_noi";

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;

                        using (AdoDataReader rd =
                            cmd.ExecuteReader())
                        {
                            if (!rd.Read())
                            {
                                throw new InvalidOperationException(
                                    "Không đọc được ranh giới Hà Nội.");
                            }

                            soRanhGioi =
                                Convert.ToInt64(
                                    rd.GetValue(0));

                            soLoi =
                                Convert.ToInt64(
                                    rd.GetValue(1));
                        }
                    }

                    if (soRanhGioi == 0 ||
                        soLoi > 0)
                    {
                        ThemCanhBaoChoDanhSach(
                            danhSach,
                            "Chưa kiểm tra tọa độ trong " +
                            "ranh giới Hà Nội: " +
                            (
                                soRanhGioi == 0
                                ? "bảng ranh giới không có dữ liệu."
                                : "lớp ranh giới có " +
                                  soLoi +
                                  " hình học không hợp lệ."
                            ));

                        return;
                    }

                    HashSet<string> maNgoai =
                        new HashSet<string>(
                            StringComparer.OrdinalIgnoreCase);

                    using (AdoCommand cmd =
                        cn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT u.ma_don_vi
                            FROM public.don_vi_quan_doi AS u
                            WHERE u.geom_32648 IS NOT NULL
                              AND NOT ST_IsEmpty(u.geom_32648)
                              AND ST_IsValid(u.geom_32648)
                              AND ST_SRID(u.geom_32648) = 32648
                              AND ST_GeometryType(u.geom_32648) =
                                  'ST_Point'
                              AND NOT EXISTS (
                                  SELECT 1
                                  FROM public.ranh_gioi_ha_noi AS r
                                  WHERE ST_Covers(
                                      r.geom,
                                      u.geom_32648
                                  )
                              )";

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 30;

                        using (AdoDataReader rd =
                            cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                if (!rd.IsDBNull(0))
                                {
                                    maNgoai.Add(
                                        Convert.ToString(
                                            rd.GetValue(0)));
                                }
                            }
                        }
                    }

                    foreach (DonViQuanDoi dv in danhSach)
                    {
                        if (dv != null &&
                            !string.IsNullOrWhiteSpace(
                                dv.MaDonVi) &&
                            maNgoai.Contains(dv.MaDonVi))
                        {
                            ThemCanhBao(
                                dv,
                                "Tọa độ ngoài phạm vi ranh giới " +
                                "Hà Nội (ST_Covers = false).");
                        }
                    }
                }
            }
            catch (DbException ex)
            {
                ThemCanhBaoChoDanhSach(
                    danhSach,
                    "Chưa kiểm tra ranh giới Hà Nội: " +
                    ex.Message);
            }
        }

        private static void ThemCanhBaoChoDanhSach(
            IEnumerable<DonViQuanDoi> danhSach,
            string canhBao)
        {
            foreach (DonViQuanDoi dv in danhSach)
            {
                if (dv != null)
                {
                    ThemCanhBao(
                        dv,
                        canhBao);
                }
            }
        }

        // =========================================
        // 13. HAM DOC POINT TU WKT
        // =========================================

        private static bool ThuToaDoPoint(
            string wkt,
            out double x,
            out double y)
        {
            x = 0d;
            y = 0d;

            if (string.IsNullOrWhiteSpace(wkt) ||
                !wkt.TrimStart().StartsWith(
                    "POINT",
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            int dau = wkt.IndexOf('(');
            int cuoi = wkt.IndexOf(')', dau + 1);

            if (dau < 0 || cuoi <= dau + 1)
                return false;

            string[] so = wkt
                .Substring(
                    dau + 1,
                    cuoi - dau - 1)
                .Trim()
                .Split(
                    (char[])null,
                    StringSplitOptions.RemoveEmptyEntries);

            return so.Length >= 2 &&
                double.TryParse(
                    so[0],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out x) &&
                double.TryParse(
                    so[1],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out y) &&
                LaSoHuuHan(x) &&
                LaSoHuuHan(y);
        }

        // =========================================
        // 14. KHOANG CACH WGS84
        // =========================================

        private static double KhoangCachMet(
            double lon1,
            double lat1,
            double lon2,
            double lat2)
        {
            const double r = 6371008.8;
            double a = Math.PI / 180.0;

            double dLat = (lat2 - lat1) * a;
            double dLon = (lon2 - lon1) * a;

            double v =
                Math.Pow(
                    Math.Sin(dLat / 2), 2) +
                Math.Cos(lat1 * a) *
                Math.Cos(lat2 * a) *
                Math.Pow(
                    Math.Sin(dLon / 2), 2);

            return 2 * r *
                Math.Asin(
                    Math.Min(
                        1.0,
                        Math.Sqrt(
                            Math.Max(0.0, v))));
        }

        private static bool CoToaDoWgs84(
            double lon,
            double lat)
        {
            return LaSoHuuHan(lon) &&
                LaSoHuuHan(lat) &&
                lon >= -180d &&
                lon <= 180d &&
                lat >= -90d &&
                lat <= 90d &&
                !(lon == 0d && lat == 0d);
        }

        private static bool LaSoHuuHan(
            double value)
        {
            return !double.IsNaN(value) &&
                !double.IsInfinity(value);
        }

        // =========================================
        // 15. HAM DOC DATAREADER
        // =========================================

        private static string DocChuoi(
            AdoDataReader doc,
            string cot)
        {
            int i = doc.GetOrdinal(cot);

            return doc.IsDBNull(i)
                ? null
                : Convert.ToString(
                    doc.GetValue(i));
        }

        private static double DocSo(
            AdoDataReader doc,
            string cot)
        {
            int i = doc.GetOrdinal(cot);

            return doc.IsDBNull(i)
                ? 0d
                : Convert.ToDouble(
                    doc.GetValue(i));
        }

        private static double? DocSoThucNullable(
            AdoDataReader doc,
            string cot)
        {
            int i = doc.GetOrdinal(cot);

            if (doc.IsDBNull(i))
                return null;

            double v = Convert.ToDouble(
                doc.GetValue(i),
                CultureInfo.InvariantCulture);

            return LaSoHuuHan(v)
                ? (double?)v
                : null;
        }

        private static int? DocSoNguyenNullable(
            AdoDataReader doc,
            string cot)
        {
            int i = doc.GetOrdinal(cot);

            return doc.IsDBNull(i)
                ? (int?)null
                : Convert.ToInt32(
                    doc.GetValue(i));
        }

        private static bool? DocBoolNullable(
            AdoDataReader doc,
            string cot)
        {
            int i = doc.GetOrdinal(cot);

            return doc.IsDBNull(i)
                ? (bool?)null
                : Convert.ToBoolean(
                    doc.GetValue(i));
        }

        // =========================================
        // 16. GAN THAM SO SQL
        // =========================================

        private static void GanThamSoDonVi(
            AdoCommand cmd,
            DonViQuanDoi dv)
        {
            ThemThamSo(
                cmd, "@ma",
                DbType.String,
                dv.MaDonVi.Trim());

            ThemThamSo(
                cmd, "@ten",
                DbType.String,
                dv.TenDonVi.Trim());

            ThemThamSo(
                cmd, "@ten_tat",
                DbType.String,
                RongThanhNull(dv.TenVietTat));

            ThemThamSo(
                cmd, "@loai",
                DbType.String,
                dv.LoaiDonVi.Trim());

            ThemThamSo(
                cmd, "@binh_chung",
                DbType.String,
                dv.QuanBinhChung.Trim());

            ThemThamSo(
                cmd, "@cap_tren",
                DbType.String,
                dv.DonViCapTren.Trim());

            ThemThamSo(
                cmd, "@quan_huyen",
                DbType.String,
                dv.QuanHuyen.Trim());

            ThemThamSo(
                cmd, "@dia_chi",
                DbType.String,
                dv.DiaChi.Trim());

            ThemThamSo(
                cmd, "@kinh_do",
                DbType.Double,
                dv.KinhDo);

            ThemThamSo(
                cmd, "@vi_do",
                DbType.Double,
                dv.ViDo);

            ThemThamSo(
                cmd, "@chi_huy",
                DbType.String,
                RongThanhNull(dv.ChiHuyTruong));

            ThemThamSo(
                cmd, "@chinh_uy",
                DbType.String,
                RongThanhNull(dv.ChinhUy));

            ThemThamSo(
                cmd, "@dien_thoai",
                DbType.String,
                dv.SoDienThoai.Trim());

            ThemThamSo(
                cmd, "@sscd",
                DbType.String,
                RongThanhNull(dv.TrangThaiSSCD));

            ThemThamSo(
                cmd, "@do_mat",
                DbType.String,
                RongThanhNull(dv.DoMat));

            ThemThamSo(
                cmd, "@nhiem_vu",
                DbType.String,
                NoiDanhSach(dv.NhiemVuChinh));

            ThemThamSo(
                cmd, "@trang_bi",
                DbType.String,
                NoiDanhSach(dv.VuKhiTrangBi));
        }

        private static void ThemThamSo(
            AdoCommand cmd,
            string ten,
            DbType kieu,
            object giaTri)
        {
            DbParameter p = cmd.CreateParameter();

            if (p == null)
            {
                throw new InvalidOperationException(
                    "Không tạo được tham số: " + ten);
            }

            p.ParameterName = ten;
            p.DbType = kieu;
            p.Value = giaTri ?? DBNull.Value;

            cmd.Parameters.Add(p);
        }

        private static string RongThanhNull(
            string giaTri)
        {
            return string.IsNullOrWhiteSpace(giaTri)
                ? null
                : giaTri.Trim();
        }

        private static string NoiDanhSach(
            string[] danhSach)
        {
            if (danhSach == null)
                return null;

            string[] cacDong = danhSach
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToArray();

            return cacDong.Length == 0
                ? null
                : string.Join("\n", cacDong);
        }

        private static string[] TachDanhSach(
            string noiDung)
        {
            return string.IsNullOrWhiteSpace(noiDung)
                ? new string[0]
                : noiDung.Split(
                    new[]
                    {
                        "\r\n",
                        "\n",
                        "\r"
                    },
                    StringSplitOptions.RemoveEmptyEntries);
        }

        // =========================================
        // 17. KIEM TRA MA VA DU LIEU GHI
        // =========================================

        private static string KiemTraMa(
            string maDonVi)
        {
            if (string.IsNullOrWhiteSpace(maDonVi))
            {
                throw new ArgumentException(
                    "Mã đơn vị không được trống.",
                    "maDonVi");
            }

            string ma = maDonVi.Trim();

            if (ma.Length > 50)
            {
                throw new ArgumentOutOfRangeException(
                    "maDonVi",
                    "Mã đơn vị tối đa 50 ký tự.");
            }

            return ma;
        }

        private static void KiemTraDuLieuGhi(
            DonViQuanDoi dv)
        {
            if (dv == null)
            {
                throw new ArgumentNullException("dv");
            }

            KiemTraMa(dv.MaDonVi);

            if (!string.IsNullOrWhiteSpace(dv.GhiChu))
            {
                throw new NotSupportedException(
                    "Chưa có cột ghi_chu trong CSDL; " +
                    "không thể lưu GhiChu.");
            }

            BatBuoc(
                dv.TenDonVi,
                255,
                "Tên đơn vị");

            BatBuoc(
                dv.LoaiDonVi,
                100,
                "Loại đơn vị");

            BatBuoc(
                dv.QuanBinhChung,
                100,
                "Quân/Binh chủng");

            BatBuoc(
                dv.DonViCapTren,
                255,
                "Đơn vị cấp trên");

            BatBuoc(
                dv.QuanHuyen,
                100,
                "Địa bàn");

            BatBuoc(
                dv.DiaChi,
                10000,
                "Địa chỉ");

            BatBuoc(
                dv.SoDienThoai,
                50,
                "Số điện thoại");

            TuyChon(
                dv.TenVietTat,
                100,
                "Tên viết tắt");

            TuyChon(
                dv.ChiHuyTruong,
                150,
                "Chỉ huy trưởng");

            TuyChon(
                dv.ChinhUy,
                150,
                "Chính ủy");

            TuyChon(
                dv.TrangThaiSSCD,
                100,
                "Trạng thái SSCĐ");

            TuyChon(
                dv.DoMat,
                50,
                "Độ mật");

            if (!CoToaDoWgs84(
                    dv.KinhDo,
                    dv.ViDo) ||
                dv.KinhDo < 102d ||
                dv.KinhDo > 108d ||
                dv.ViDo < 0d ||
                dv.ViDo > 84d)
            {
                throw new ArgumentException(
                    "Cần tọa độ WGS84 hợp lệ " +
                    "trong UTM 48 Bắc " +
                    "(102-108°E, 0-84°N).");
            }
        }

        private static void BatBuoc(
            string giaTri,
            int toiDa,
            string ten)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                throw new ArgumentException(
                    ten + " không được để trống.");
            }

            TuyChon(
                giaTri,
                toiDa,
                ten);
        }

        private static void TuyChon(
            string giaTri,
            int toiDa,
            string ten)
        {
            if (giaTri != null &&
                giaTri.Trim().Length > toiDa)
            {
                throw new ArgumentException(
                    ten + " không được quá " +
                    toiDa + " ký tự.");
            }
        }
    }
}
    