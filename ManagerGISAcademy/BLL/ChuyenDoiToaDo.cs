
using System;
using System.Globalization;

namespace ManagerGISAcademy.BLL
{
    /// <summary>
    /// Một cặp tọa độ kèm mã EPSG.
    /// EPSG:4326: X = kinh độ, Y = vĩ độ (đơn vị độ).
    /// EPSG:32648 và EPSG:3857: X/Y là tọa độ phẳng (đơn vị mét).
    /// </summary>
    public struct ToaDoQuyChieu
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public int Epsg { get; private set; }

        public ToaDoQuyChieu(double x, double y, int epsg)
        {
            X = x;
            Y = y;
            Epsg = epsg;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "EPSG:{0} | X={1:0.########} | Y={2:0.########}",
                Epsg, X, Y);
        }
    }

    /// <summary>
    /// Chuyển đổi tọa độ thuần C# giữa:
    /// - EPSG:4326: WGS 84, kinh độ/vĩ độ (độ).
    /// - EPSG:32648: WGS 84 / UTM zone 48N (mét).
    /// - EPSG:3857: WGS 84 / Pseudo-Mercator (mét).
    ///
    /// Không chuyển đổi sang VN-2000: VN-2000 dùng datum khác WGS84,
    /// cần thông số chuyển datum và mã hệ tọa độ cụ thể.
    /// Không đọc/ghi PostGIS, không sử dụng WinForms hoặc MapWinGIS.
    /// Dùng được trong BLL - C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public static class ChuyenDoiToaDo
    {
        public const int Wgs84 = 4326;
        public const int Utm48N = 32648;
        public const int WebMercator = 3857;

        private const double BanKinhTrucLon = 6378137.0;
        private const double DoDet = 1.0 / 298.257223563;
        private const double BinhPhuongTamSai = DoDet * (2.0 - DoDet);
        private const double TamSaiPhu = BinhPhuongTamSai / (1.0 - BinhPhuongTamSai);
        private const double HeSoUtm = 0.9996;
        private const double DichDongUtm = 500000.0;
        private const double KinhTuyenTrucUtm48 = 105.0;
        private const double ViDoMercatorToiDa = 85.0511287798066;
        private const double GioiHanMercator = 20037508.342789244;

        /// <summary>
        /// Hàm chung. Không làm tròn tọa độ để tránh mất độ chính xác.
        /// Nguồn và đích phải thuộc 4326, 32648 hoặc 3857.
        /// </summary>
        public static ToaDoQuyChieu ChuyenDoi(
            double x, double y, int epsgNguon, int epsgDich)
        {
            KiemTraEpsg(epsgNguon);
            KiemTraEpsg(epsgDich);
            KiemTraToaDoNguon(x, y, epsgNguon);

            if (epsgNguon == epsgDich)
                return new ToaDoQuyChieu(x, y, epsgDich);

            ToaDoQuyChieu diaLy;
            switch (epsgNguon)
            {
                case Wgs84:
                    diaLy = new ToaDoQuyChieu(x, y, Wgs84);
                    break;
                case Utm48N:
                    diaLy = TinhUtm48NSangWgs84(x, y);
                    break;
                default:
                    diaLy = TinhMercatorSangWgs84(x, y);
                    break;
            }

            switch (epsgDich)
            {
                case Wgs84:
                    return diaLy;
                case Utm48N:
                    return TinhWgs84SangUtm48N(diaLy.X, diaLy.Y);
                default:
                    return TinhWgs84SangMercator(diaLy.X, diaLy.Y);
            }
        }

        /// <summary>Đầu vào theo thứ tự kinh độ, vĩ độ (độ).</summary>
        public static ToaDoQuyChieu Wgs84SangUtm48N(double kinhDo, double viDo)
        {
            return ChuyenDoi(kinhDo, viDo, Wgs84, Utm48N);
        }

        /// <summary>Đầu vào X = Easting, Y = Northing (mét).</summary>
        public static ToaDoQuyChieu Utm48NSangWgs84(double x, double y)
        {
            return ChuyenDoi(x, y, Utm48N, Wgs84);
        }

        public static ToaDoQuyChieu Wgs84SangWebMercator(double kinhDo, double viDo)
        {
            return ChuyenDoi(kinhDo, viDo, Wgs84, WebMercator);
        }

        public static ToaDoQuyChieu WebMercatorSangWgs84(double x, double y)
        {
            return ChuyenDoi(x, y, WebMercator, Wgs84);
        }

        /// <summary>
        /// Kiểm tra giá trị có thể biểu diễn trong CRS, không xác minh vị trí
        /// của đơn vị hoặc tính đúng đắn của nguồn dữ liệu.
        /// </summary>
        public static bool ToaDoHopLe(double x, double y, int epsg)
        {
            try
            {
                KiemTraEpsg(epsg);
                KiemTraToaDoNguon(x, y, epsg);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static ToaDoQuyChieu TinhWgs84SangUtm48N(double kinhDo, double viDo)
        {
            // UTM 48N: múi 102°E–108°E; bán cầu Bắc, vĩ độ 0°–84°.
            // Hạn chế này ngăn chiếu sai múi hoặc nhầm UTM bán cầu Nam.
            if (kinhDo < 102.0 || kinhDo > 108.0 || viDo < 0.0 || viDo > 84.0)
                throw new ArgumentOutOfRangeException("kinhDo",
                    "Điểm nằm ngoài phạm vi UTM múi 48 Bắc (102–108°E; 0–84°N).");

            double phi = SangRadian(viDo);
            double lambda = SangRadian(kinhDo - KinhTuyenTrucUtm48);
            double sinPhi = Math.Sin(phi);
            double cosPhi = Math.Cos(phi);
            double tanPhi = Math.Tan(phi);

            double t = tanPhi * tanPhi;
            double c = TamSaiPhu * cosPhi * cosPhi;
            double a = cosPhi * lambda;
            double n = BanKinhTrucLon /
                Math.Sqrt(1.0 - BinhPhuongTamSai * sinPhi * sinPhi);
            double m = CungKinhTuyen(phi);

            double a2 = a * a;
            double a3 = a2 * a;
            double a4 = a2 * a2;
            double a5 = a4 * a;
            double a6 = a3 * a3;

            double x = DichDongUtm + HeSoUtm * n *
                (a + (1.0 - t + c) * a3 / 6.0 +
                 (5.0 - 18.0 * t + t * t + 72.0 * c - 58.0 * TamSaiPhu) * a5 / 120.0);

            double y = HeSoUtm * (m + n * tanPhi *
                (a2 / 2.0 + (5.0 - t + 9.0 * c + 4.0 * c * c) * a4 / 24.0 +
                 (61.0 - 58.0 * t + t * t + 600.0 * c - 330.0 * TamSaiPhu) * a6 / 720.0));

            return new ToaDoQuyChieu(x, y, Utm48N);
        }

        private static ToaDoQuyChieu TinhUtm48NSangWgs84(double x, double y)
        {
            // EPSG:32648 không có false northing 10.000.000 m của UTM Nam.
            double e2 = BinhPhuongTamSai;
            double e4 = e2 * e2;
            double e6 = e4 * e2;
            double m = y / HeSoUtm;
            double mu = m / (BanKinhTrucLon *
                (1.0 - e2 / 4.0 - 3.0 * e4 / 64.0 - 5.0 * e6 / 256.0));
            double can = Math.Sqrt(1.0 - e2);
            double e1 = (1.0 - can) / (1.0 + can);
            double e1_2 = e1 * e1;
            double e1_3 = e1_2 * e1;
            double e1_4 = e1_2 * e1_2;

            double phi1 = mu +
                (3.0 * e1 / 2.0 - 27.0 * e1_3 / 32.0) * Math.Sin(2.0 * mu) +
                (21.0 * e1_2 / 16.0 - 55.0 * e1_4 / 32.0) * Math.Sin(4.0 * mu) +
                (151.0 * e1_3 / 96.0) * Math.Sin(6.0 * mu) +
                (1097.0 * e1_4 / 512.0) * Math.Sin(8.0 * mu);

            double sinPhi1 = Math.Sin(phi1);
            double cosPhi1 = Math.Cos(phi1);
            double tanPhi1 = Math.Tan(phi1);
            double t1 = tanPhi1 * tanPhi1;
            double c1 = TamSaiPhu * cosPhi1 * cosPhi1;
            double heSo = 1.0 - e2 * sinPhi1 * sinPhi1;
            double n1 = BanKinhTrucLon / Math.Sqrt(heSo);
            double r1 = BanKinhTrucLon * (1.0 - e2) /
                Math.Pow(heSo, 1.5);
            double d = (x - DichDongUtm) / (n1 * HeSoUtm);
            double d2 = d * d;
            double d3 = d2 * d;
            double d4 = d2 * d2;
            double d5 = d4 * d;
            double d6 = d3 * d3;

            double viDoRad = phi1 - (n1 * tanPhi1 / r1) *
                (d2 / 2.0 -
                 (5.0 + 3.0 * t1 + 10.0 * c1 - 4.0 * c1 * c1 - 9.0 * TamSaiPhu) * d4 / 24.0 +
                 (61.0 + 90.0 * t1 + 298.0 * c1 + 45.0 * t1 * t1 -
                  252.0 * TamSaiPhu - 3.0 * c1 * c1) * d6 / 720.0);

            double kinhDoRad = SangRadian(KinhTuyenTrucUtm48) +
                (d - (1.0 + 2.0 * t1 + c1) * d3 / 6.0 +
                 (5.0 - 2.0 * c1 + 28.0 * t1 - 3.0 * c1 * c1 +
                  8.0 * TamSaiPhu + 24.0 * t1 * t1) * d5 / 120.0) / cosPhi1;

            double kinhDo = SangDo(kinhDoRad);
            double viDo = SangDo(viDoRad);
            if (!LaSoHuuHan(kinhDo) || !LaSoHuuHan(viDo) ||
                kinhDo < 101.999999 || kinhDo > 108.000001 ||
                viDo < -0.000001 || viDo > 84.000001)
                throw new ArgumentException(
                    "X/Y không đại diện một điểm hợp lệ trong UTM 48N.");

            return new ToaDoQuyChieu(kinhDo, viDo, Wgs84);
        }

        private static double CungKinhTuyen(double phi)
        {
            double e2 = BinhPhuongTamSai;
            double e4 = e2 * e2;
            double e6 = e4 * e2;
            return BanKinhTrucLon *
                ((1.0 - e2 / 4.0 - 3.0 * e4 / 64.0 - 5.0 * e6 / 256.0) * phi -
                 (3.0 * e2 / 8.0 + 3.0 * e4 / 32.0 + 45.0 * e6 / 1024.0) * Math.Sin(2.0 * phi) +
                 (15.0 * e4 / 256.0 + 45.0 * e6 / 1024.0) * Math.Sin(4.0 * phi) -
                 (35.0 * e6 / 3072.0) * Math.Sin(6.0 * phi));
        }

        private static ToaDoQuyChieu TinhWgs84SangMercator(double kinhDo, double viDo)
        {
            if (Math.Abs(viDo) > ViDoMercatorToiDa)
                throw new ArgumentOutOfRangeException("viDo",
                    "EPSG:3857 chỉ hỗ trợ vĩ độ khoảng ±85,05112878°.");

            double x = BanKinhTrucLon * SangRadian(kinhDo);
            double y = Math.Abs(viDo) < 1e-12 ? 0.0 :
                BanKinhTrucLon * Math.Log(Math.Tan(Math.PI / 4.0 + SangRadian(viDo) / 2.0));
            return new ToaDoQuyChieu(x, y, WebMercator);
        }

        private static ToaDoQuyChieu TinhMercatorSangWgs84(double x, double y)
        {
            double kinhDo = SangDo(x / BanKinhTrucLon);
            double viDo = SangDo(2.0 * Math.Atan(Math.Exp(y / BanKinhTrucLon)) - Math.PI / 2.0);
            return new ToaDoQuyChieu(kinhDo, viDo, Wgs84);
        }

        private static void KiemTraEpsg(int epsg)
        {
            if (epsg != Wgs84 && epsg != Utm48N && epsg != WebMercator)
                throw new ArgumentOutOfRangeException("epsg",
                    "Chỉ hỗ trợ EPSG:4326, EPSG:32648 và EPSG:3857.");
        }

        private static void KiemTraToaDoNguon(double x, double y, int epsg)
        {
            if (!LaSoHuuHan(x) || !LaSoHuuHan(y))
                throw new ArgumentException("Tọa độ phải là số hữu hạn.");

            if (epsg == Wgs84 &&
                (x < -180.0 || x > 180.0 || y < -90.0 || y > 90.0))
                throw new ArgumentOutOfRangeException("x",
                    "WGS84: kinh độ trong [-180,180], vĩ độ trong [-90,90].");

            if (epsg == Utm48N &&
                (x < 100000.0 || x > 900000.0 || y < 0.0 || y > 9350000.0))
                throw new ArgumentOutOfRangeException("x",
                    "UTM 48N: X/Y nằm ngoài khoảng tọa độ hợp lý của múi chiếu.");

            if (epsg == WebMercator &&
                (Math.Abs(x) > GioiHanMercator + 0.001 ||
                 Math.Abs(y) > GioiHanMercator + 0.001))
                throw new ArgumentOutOfRangeException("x",
                    "Web Mercator: tọa độ nằm ngoài phạm vi EPSG:3857 thông dụng.");
        }

        private static bool LaSoHuuHan(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static double SangRadian(double doGoc)
        {
            return doGoc * (Math.PI / 180.0);
        }

        private static double SangDo(double radian)
        {
            return radian * (180.0 / Math.PI);
        }
    }
}
