// Phan tach tu FormMain.cs (GD5.7.6), dung partial class.
// Khong khai bao lop FormMain moi, khong thay doi FormMain.Designer.cs.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AxMapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using ManagerGISAcademy.GUI.Views;
using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private struct ToaDoPhang
        {
            public double X, Y;

            public ToaDoPhang(double x, double y)
            {
                X = x;
                Y = y;
            }
        }

        private static uint MauOle(Color mau)
        {
            return unchecked((uint)ColorTranslator.ToOle(mau));
        }

        private void XoaVePhanTich()
        {
            bool coHinhCanXoa =
                veDiemDangChon >= 0 ||
                veDiemKetQua >= 0 ||
                veVungDem >= 0;

            if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed)
            {
                if (veDiemDangChon >= 0)
                    banDoMapWinGIS.ClearDrawing(veDiemDangChon);

                if (veDiemKetQua >= 0)
                    banDoMapWinGIS.ClearDrawing(veDiemKetQua);

                if (veVungDem >= 0)
                    banDoMapWinGIS.ClearDrawing(veVungDem);
            }

            veDiemDangChon = -1;
            veDiemKetQua = -1;
            veVungDem = -1;

            cacDiemPhanTich.Clear();

            if (coHinhCanXoa)
                Bd11_YeuCauVeLaiBanDo();
        }

        private bool ChieuWgs84(
            double kinhDo,
            double viDo,
            out double x,
            out double y)
        {
            x = 0;
            y = 0;

            return banDoMapWinGIS != null &&
                !double.IsNaN(kinhDo) &&
                !double.IsNaN(viDo) &&
                !double.IsInfinity(kinhDo) &&
                !double.IsInfinity(viDo) &&
                kinhDo >= -180 &&
                kinhDo <= 180 &&
                viDo >= -90 &&
                viDo <= 90 &&
                banDoMapWinGIS.DegreesToProj(
                    kinhDo,
                    viDo,
                    ref x,
                    ref y);
        }

        private List<List<ToaDoPhang>> TachVongWkt(string wkt)
        {
            var ds = new List<List<ToaDoPhang>>();

            if (string.IsNullOrWhiteSpace(wkt))
                return ds;

            string chu = wkt.Trim();

            if (chu.StartsWith(
                "SRID=",
                StringComparison.OrdinalIgnoreCase))
            {
                int tach = chu.IndexOf(';');

                if (tach < 0 ||
                    !chu.Substring(0, tach).Equals(
                        "SRID=4326",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new FormatException(
                        "WKT không có SRID=4326.");
                }

                chu = chu.Substring(tach + 1).Trim();
            }

            if (!chu.StartsWith(
                    "POLYGON",
                    StringComparison.OrdinalIgnoreCase) &&
                !chu.StartsWith(
                    "MULTIPOLYGON",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException(
                    "PostGIS không trả về vùng đệm dạng polygon.");
            }

            foreach (Match vong in Regex.Matches(
                chu,
                @"\(([^()]+)\)"))
            {
                string[] cacDinh =
                    vong.Groups[1].Value.Split(',');

                var duong = new List<ToaDoPhang>();

                foreach (string dinh in cacDinh)
                {
                    string[] so = Regex.Split(
                        dinh.Trim(),
                        @"\s+");

                    if (so.Length < 2)
                    {
                        throw new FormatException(
                            "Đỉnh WKT không đủ tọa độ.");
                    }

                    double kinhDo = double.Parse(
                        so[0],
                        CultureInfo.InvariantCulture);

                    double viDo = double.Parse(
                        so[1],
                        CultureInfo.InvariantCulture);

                    double x, y;

                    if (!ChieuWgs84(
                        kinhDo,
                        viDo,
                        out x,
                        out y))
                    {
                        throw new InvalidOperationException(
                            "Lỗi chuyển tọa độ vùng đệm sang EPSG:32648.");
                    }

                    duong.Add(new ToaDoPhang(x, y));
                }

                if (duong.Count >= 3)
                    ds.Add(duong);
            }

            if (ds.Count == 0)
            {
                throw new FormatException(
                    "Không đọc được đỉnh từ WKT vùng đệm.");
            }

            return ds;
        }

        private static double DienTichVong(List<ToaDoPhang> vong)
        {
            double tong = 0;

            for (int i = 0; i < vong.Count; i++)
            {
                ToaDoPhang a = vong[i];
                ToaDoPhang b = vong[(i + 1) % vong.Count];

                tong += a.X * b.Y - b.X * a.Y;
            }

            return Math.Abs(tong) / 2.0;
        }

        private double? VeKetQuaPhanTich(
            string wktVungDem,
            List<KetQuaPhanTichItem> danhSach,
            DonViQuanDoi tam,
            bool phongDenKetQua)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa sẵn sàng.");
            }

            List<List<ToaDoPhang>> cacVong =
                string.IsNullOrWhiteSpace(wktVungDem)
                    ? new List<List<ToaDoPhang>>()
                    : TachVongWkt(wktVungDem);

            var toaDoDiem = new List<ToaDoPhang>();
            var banGhiHopLe = new List<KetQuaPhanTichItem>();

            double minX = double.PositiveInfinity;
            double minY = double.PositiveInfinity;
            double maxX = double.NegativeInfinity;
            double maxY = double.NegativeInfinity;

            Action<double, double> themPhamVi = (x, y) =>
            {
                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
            };

            foreach (List<ToaDoPhang> vong in cacVong)
            {
                foreach (ToaDoPhang p in vong)
                    themPhamVi(p.X, p.Y);
            }

            if (danhSach != null)
            {
                foreach (KetQuaPhanTichItem item in danhSach)
                {
                    if (item == null || !item.CoToaDoWgs84HopLe)
                        continue;

                    double x, y;

                    if (!ChieuWgs84(
                        item.KinhDo.Value,
                        item.ViDo.Value,
                        out x,
                        out y))
                    {
                        continue;
                    }

                    toaDoDiem.Add(new ToaDoPhang(x, y));
                    banGhiHopLe.Add(item);
                    themPhamVi(x, y);
                }
            }

            ToaDoPhang? diemTam = null;
            if (tam != null && NghiepVuDonVi.CoToaDoWgs84(tam))
            {
                double x, y;

                if (ChieuWgs84(
                    tam.KinhDo,
                    tam.ViDo,
                    out x,
                    out y))
                {
                    diemTam = new ToaDoPhang(x, y);
                    themPhamVi(x, y);
                }
            }

            XoaVePhanTich();

            double dienTich = 0;

            try
            {
                if (cacVong.Count > 0)
                {
                    veVungDem = banDoMapWinGIS.NewDrawing(
                        MapWinGIS.tkDrawReferenceList
                            .dlSpatiallyReferencedList);

                    foreach (List<ToaDoPhang> vong in cacVong)
                    {
                        object xs = vong.Select(p => p.X).ToArray();
                        object ys = vong.Select(p => p.Y).ToArray();

                        banDoMapWinGIS.DrawPolygonEx(
                            veVungDem,
                            ref xs,
                            ref ys,
                            vong.Count,
                            MauOle(Color.FromArgb(18, 168, 138)),
                            true,
                            65);

                        banDoMapWinGIS.DrawWidePolygonEx(
                            veVungDem,
                            ref xs,
                            ref ys,
                            vong.Count,
                            MauOle(Color.FromArgb(20, 196, 164)),
                            false,
                            3,
                            240);

                        dienTich += DienTichVong(vong);
                    }
                }

                veDiemKetQua = banDoMapWinGIS.NewDrawing(
                    MapWinGIS.tkDrawReferenceList
                        .dlSpatiallyReferencedList);

                for (int i = 0; i < toaDoDiem.Count; i++)
                {
                    banDoMapWinGIS.DrawCircleEx(
                        veDiemKetQua,
                        toaDoDiem[i].X,
                        toaDoDiem[i].Y,
                        7,
                        MauOle(Color.White),
                        true,
                        235);

                    banDoMapWinGIS.DrawCircleEx(
                        veDiemKetQua,
                        toaDoDiem[i].X,
                        toaDoDiem[i].Y,
                        5,
                        MauOle(Color.FromArgb(255, 186, 67)),
                        true,
                        255);
                }

                if (diemTam.HasValue)
                {
                    banDoMapWinGIS.DrawCircleEx(
                        veDiemKetQua,
                        diemTam.Value.X,
                        diemTam.Value.Y,
                        10,
                        MauOle(Color.White),
                        true,
                        255);

                    banDoMapWinGIS.DrawCircleEx(
                        veDiemKetQua,
                        diemTam.Value.X,
                        diemTam.Value.Y,
                        7,
                        MauOle(Color.FromArgb(42, 205, 177)),
                        true,
                        255);
                }

                cacDiemPhanTich = banGhiHopLe;

                if (phongDenKetQua && !double.IsInfinity(minX))
                {
                    double rong = Math.Max(2000, maxX - minX);
                    double cao = Math.Max(2000, maxY - minY);
                    double pad = Math.Max(rong, cao) * 0.12;

                    MapWinGIS.Extents phamVi =
                        new MapWinGIS.Extents();

                    phamVi.SetBounds(
                        minX - pad,
                        minY - pad,
                        0,
                        maxX + pad,
                        maxY + pad,
                        0);

                    banDoMapWinGIS.Extents = phamVi;
                }

                Bd11_YeuCauVeLaiBanDo();
            }
            catch
            {
                XoaVePhanTich();
                throw;
            }

            return cacVong.Count > 0
                ? (double?)dienTich
                : null;
        }

        private void VeNoiBatKetQua(KetQuaPhanTichItem item)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return;

            bool coThayDoiHinhVe = veDiemDangChon >= 0;

            if (veDiemDangChon >= 0)
            {
                banDoMapWinGIS.ClearDrawing(veDiemDangChon);
                veDiemDangChon = -1;
            }

            if (item != null &&
                item.CoToaDoWgs84HopLe &&
                cacDiemPhanTich.Any(x =>
                    string.Equals(
                        x.MaDoiTuong,
                        item.MaDoiTuong,
                        StringComparison.OrdinalIgnoreCase)))
            {
                double x, y;

                if (ChieuWgs84(
                    item.KinhDo.Value,
                    item.ViDo.Value,
                    out x,
                    out y))
                {
                    veDiemDangChon = banDoMapWinGIS.NewDrawing(
                        MapWinGIS.tkDrawReferenceList
                            .dlSpatiallyReferencedList);
                    coThayDoiHinhVe = true;

                    banDoMapWinGIS.DrawCircleEx(
                        veDiemDangChon,
                        x, y, 14,
                        MauOle(Color.White),
                        true,
                        245);

                    banDoMapWinGIS.DrawCircleEx(
                        veDiemDangChon,
                        x, y, 10,
                        MauOle(Color.FromArgb(249, 88, 70)),
                        true,
                        255);

                    banDoMapWinGIS.DrawCircleEx(
                        veDiemDangChon,
                        x, y, 4,
                        MauOle(Color.White),
                        true,
                        255);
                }
            }

            if (coThayDoiHinhVe)
                Bd11_YeuCauVeLaiBanDo();
        }

        private static List<KetQuaPhanTichItem> ChuyenKetQuaPhanTichGon(
            IEnumerable<DonViTheoKhoangCach> danhSach)
        {
            return danhSach.Select(d => new KetQuaPhanTichItem
            {
                MaDoiTuong = d.MaDonVi,
                TenDoiTuong = d.TenDonVi,
                // Cot "Quan/Binh chung" tren bang ket qua (truoc day la hang so).
                TenLop = string.IsNullOrWhiteSpace(d.QuanBinhChung)
                    ? "—" : d.QuanBinhChung.Trim(),
                KhoangCachMet = d.KhoangCachMet,
                KinhDo = d.KinhDo,
                ViDo = d.ViDo,
                GhiChu = d.QuanBinhChung
            }).ToList();
        }
    }
}