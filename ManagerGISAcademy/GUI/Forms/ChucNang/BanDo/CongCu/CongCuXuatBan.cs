using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>Xuat ban: in ban do A4/A3, phieu ho so don vi, xuat du lieu GIS.</summary>
    public partial class FormMain
    {
        private string CongCu_NguoiLap()
        {
            string vt = KetNoiCoSoDuLieu.VaiTroPhien;
            return "Vai trò: " + (string.IsNullOrEmpty(vt) ? "Quản trị (mặc định)" : TruyXuatQuanTri.TenVaiTro(vt));
        }

        /// <summary>Chup ban do theo pham vi (EPSG:32648) ra Bitmap rong 'rongPx'.</summary>
        private Bitmap CongCu_ChupBanDo(double[] pv, int rongPx)
        {
            rongPx = Math.Max(50, Math.Min(5000, rongPx));
            int caoPx = Math.Max(50, (int)Math.Round(rongPx * (pv[3] - pv[1]) / (pv[2] - pv[0])));
            Bitmap bmp = new Bitmap(rongPx, caoPx);
            Extents e = new Extents();
            e.SetBounds(pv[0], pv[1], 0, pv[2], pv[3], 0);
            // Luoi tren man hinh co nhan theo khung nhin -> an khi chup (ban in tu ve luoi mep khung).
            int[] an = { ccLopLuoi, ccLopLuoiNhan };
            bool[] hien = an.Select(h => Logo_CoHandle(h) && banDoMapWinGIS.get_LayerVisible(h)).ToArray();
            for (int i = 0; i < an.Length; i++) if (hien[i]) banDoMapWinGIS.set_LayerVisible(an[i], false);
            try
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    IntPtr hdc = g.GetHdc();
                    try { banDoMapWinGIS.SnapShotToDC(hdc, e, rongPx); }
                    finally { g.ReleaseHdc(hdc); }
                }
            }
            finally
            {
                for (int i = 0; i < an.Length; i++) if (hien[i]) banDoMapWinGIS.set_LayerVisible(an[i], true);
            }
            return bmp;
        }

        private List<MucChuGiai> CongCu_ChuGiai()
        {
            List<MucChuGiai> ds = new List<MucChuGiai>
            {
                new MucChuGiai("Trụ sở đơn vị quân đội", Color.FromArgb(200, 30, 45), 4),
                new MucChuGiai("Ban CHQS cấp xã / phường", Color.FromArgb(39, 126, 205), 0),
            };
            if (Logo_CoHandle(ccLopKhuonVien)) ds.Add(new MucChuGiai("Khuôn viên doanh trại", Color.FromArgb(0, 140, 112), 1));
            if (Logo_CoHandle(ccLopTuyen)) ds.Add(new MucChuGiai("Tuyến đường đề xuất", Color.FromArgb(39, 126, 205), 2));
            if (Logo_CoHandle(ccLopVung)) ds.Add(new MucChuGiai("Vùng tiếp cận 5/10/15 phút", Color.FromArgb(109, 90, 205), 1));
            if (Logo_CoHandle(ccLopMatCat)) ds.Add(new MucChuGiai("Tuyến mặt cắt địa hình", Color.FromArgb(209, 127, 35), 3));
            if (Logo_CoHandle(ccLopLichSu)) ds.Add(new MucChuGiai("Vị trí trụ sở qua các lần định vị", Color.FromArgb(209, 127, 35), 0));
            if (Logo_CoHandle(ccLopNhap)) ds.Add(new MucChuGiai("Điểm tọa độ vừa nhập", Color.FromArgb(39, 126, 205), 0));
            ds.Add(new MucChuGiai("Lưới tọa độ km UTM 48N", Color.FromArgb(40, 70, 120), 5));
            return ds;
        }

        private void CongCu_MoInBanDo()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            if (phanHeHienTai != "Bản đồ GIS") ChonPhanHe(btnNavMap, "Bản đồ GIS");
            Extents e = banDoMapWinGIS.Extents;
            double[] nhin = { e.xMin, e.yMin, e.xMax, e.yMax };
            using (FormInBanDo f = new FormInBanDo(nhin, CongCu_ChupBanDo, CongCu_ChuGiai(), CongCu_NguoiLap()))
                f.ShowDialog(this);
        }

        private void CongCu_MoXuatDuLieu()
        {
            using (FormXuatDuLieuGis f = new FormXuatDuLieuGis(CongCu_GhiShapefile))
                f.ShowDialog(this);
        }

        /// <summary>Ghi DataTable (cot "wkt" EPSG:32648) ra Shapefile bang MapWinGIS (chay tren UI thread).</summary>
        private void CongCu_GhiShapefile(string tep, string lop, DataTable t)
        {
            ShpfileType kieu = lop == "don_vi" ? ShpfileType.SHP_POINT : ShpfileType.SHP_POLYGON;
            foreach (string ext in new[] { ".shp", ".shx", ".dbf", ".prj", ".cpg" })
            {
                string f = Path.ChangeExtension(tep, ext);
                if (File.Exists(f)) File.Delete(f);
            }
            Shapefile sf = new Shapefile();
            if (!sf.CreateNew("", kieu)) throw new InvalidOperationException("Không tạo được shapefile: " + sf.ErrorMsg[sf.LastErrorCode]);
            try
            {
                GeoProjection gp = new GeoProjection();
                gp.ImportFromEPSG(32648);
                sf.GeoProjection = gp;
                int cotWkt = t.Columns.IndexOf("wkt");
                List<int> cotDl = new List<int>();
                List<int> chiSoTruong = new List<int>();
                HashSet<string> daDung = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int c = 0; c < t.Columns.Count; c++)
                {
                    if (c == cotWkt) continue;
                    DataColumn col = t.Columns[c];
                    string ten = col.ColumnName.Length > 10 ? col.ColumnName.Substring(0, 10) : col.ColumnName;
                    for (int i = 1; !daDung.Add(ten); i++) ten = ten.Substring(0, Math.Min(ten.Length, 8)) + "_" + i;
                    FieldType ft; int doRong, chinhXac;
                    Type ty = col.DataType;
                    if (ty == typeof(double) || ty == typeof(float) || ty == typeof(decimal)) { ft = FieldType.DOUBLE_FIELD; doRong = 18; chinhXac = 7; }
                    else if (ty == typeof(int) || ty == typeof(long) || ty == typeof(short)) { ft = FieldType.INTEGER_FIELD; doRong = 10; chinhXac = 0; }
                    else
                    {
                        ft = FieldType.STRING_FIELD; chinhXac = 0;
                        int max = 1;
                        foreach (DataRow r in t.Rows) if (r[c] != DBNull.Value) max = Math.Max(max, Encoding.UTF8.GetByteCount(Convert.ToString(r[c])));
                        doRong = Math.Min(254, max);
                    }
                    int idx = sf.EditAddField(ten, ft, chinhXac, doRong);
                    if (idx < 0) throw new InvalidOperationException("Không thêm được trường " + ten);
                    cotDl.Add(c); chiSoTruong.Add(idx);
                }
                foreach (DataRow r in t.Rows)
                {
                    string wkt = cotWkt >= 0 ? Convert.ToString(r[cotWkt]) : null;
                    if (string.IsNullOrWhiteSpace(wkt)) continue;
                    Shape s = new Shape();
                    if (!s.ImportFromWKT(wkt)) continue;
                    int si = sf.NumShapes;
                    if (!sf.EditInsertShape(s, ref si)) continue;
                    for (int i = 0; i < cotDl.Count; i++)
                    {
                        object v = r[cotDl[i]];
                        if (v == DBNull.Value) continue;
                        if (v is decimal || v is float) v = Convert.ToDouble(v);
                        if (v is long || v is short) v = Convert.ToInt32(v);
                        sf.EditCellValue(chiSoTruong[i], si, v);
                    }
                }
                if (!sf.SaveAs(tep, null)) throw new InvalidOperationException("Không ghi được shapefile: " + sf.ErrorMsg[sf.LastErrorCode]);
            }
            finally { sf.Close(); }
            File.WriteAllText(Path.ChangeExtension(tep, ".cpg"), "UTF-8", new UTF8Encoding(false));
        }

        // =====================================================
        // PHIEU HO SO DON VI
        // =====================================================

        private async void CongCu_InPhieuHoSo(string ma)
        {
            if (string.IsNullOrWhiteSpace(ma)) return;
            statusGeneral.Text = "● Đang chuẩn bị phiếu hồ sơ " + ma + "…";
            try
            {
                NghiepVuDonVi nv = nghiepVuDonVi;
                DonViQuanDoi dv = await Task.Run(() => nv.LayDonViTheoMa(ma));
                if (dv == null)
                {
                    MessageBox.Show(this, "Không tìm thấy hồ sơ " + ma + " trong CSDL (hoặc vai trò hiện tại không được xem do độ mật).",
                        "Phiếu hồ sơ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                TruyXuatTracDia dal = new TruyXuatTracDia();
                List<DongLichSuViTri> ls = await Task.Run(() => dal.LichSuViTri(ma));
                List<KhuonVien> kvs = ccKhuonVien ?? await Task.Run(() => dal.LayKhuonVien());
                DuLieuPhieuHoSo d = new DuLieuPhieuHoSo
                {
                    DonVi = dv,
                    LichSu = ls,
                    KhuonVien = kvs.FirstOrDefault(k => k.MaDonVi == ma),
                    NguoiIn = CongCu_NguoiLap()
                };
                DongLichSuViTri hh = ls.FirstOrDefault(x => x.LaHienHanh);
                double x0, y0;
                if (hh != null) { x0 = hh.E; y0 = hh.N; }
                else if (!Native_Wgs84SangToaDoBanDo(dv.KinhDo, dv.ViDo, out x0, out y0)) { x0 = 0; y0 = 0; }
                d.E = x0; d.N = y0;
                if (Math.Abs(dv.KinhDo) > 0.0001 && Math.Abs(dv.ViDo) > 0.0001)
                    d.Vn2000 = await Task.Run(() => dal.DoiVn2000(dv.KinhDo, dv.ViDo));
                if (x0 > 0 && banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed)
                {
                    double r = d.KhuonVien != null ? Math.Max(300, Math.Sqrt(d.KhuonVien.DienTichM2) * 1.2) : 300;
                    d.PhamViAnhM = 2 * r;
                    // Dua ban do toi don vi de nen anh ve tinh / OSM tai du o muc zoom nay, roi moi chup.
                    if (phanHeHienTai != "Bản đồ GIS") ChonPhanHe(btnNavMap, "Bản đồ GIS");
                    CongCu_PhongToi(x0 - r * 1.6, y0 - r * 1.6, x0 + r * 1.6, y0 + r * 1.6);
                    banDoMapWinGIS.Redraw();
                    statusGeneral.Text = "● Đang tải nền bản đồ quanh trụ sở để chụp ảnh vị trí…";
                    await Task.Delay(2600);
                    d.AnhBanDo = CongCu_ChupBanDo(new[] { x0 - r, y0 - r, x0 + r, y0 + r }, 1000);
                }
                statusGeneral.Text = "● Phiếu hồ sơ " + dv.TenDonVi + " sẵn sàng.";
                try
                {
                    using (PhieuHoSoIn doc = new PhieuHoSoIn(d))
                        TienIchIn.XemTruocVaIn(this, doc, "Phiếu hồ sơ – " + dv.TenDonVi,
                            () => new PhieuHoSoIn(d), "PhieuHoSo_" + ma + "_" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
                }
                finally { if (d.AnhBanDo != null) d.AnhBanDo.Dispose(); }
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }
    }
}
