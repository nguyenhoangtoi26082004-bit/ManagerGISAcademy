using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// "Cong cu GIS" tren ban do: nut thu 6 tren thanh cong cu + menu
    ///   Trac dia : nhap toa do GNSS/CSV/KML, lich su vi tri, luoi toa do UTM
    ///   Khuon vien: so hoa da giac doanh trai, hien lop khuon vien
    ///   Mang duong: tim duong, co so quan y gan nhat, vung tiep can (pgRouting)
    ///   Dia hinh : mat cat + tam nhin thong (DEM SRTM 30 m)
    ///   Xuat ban : in ban do A4/A3, xuat GeoJSON/KML/SHP/CSV
    /// Toa do ban do = EPSG:32648 (met).
    /// </summary>
    public partial class FormMain
    {
        private enum CcCheDo { None, KhuonVien, Tuyen, QuanY, TiepCan, MatCat }

        private const string CcKeyNhap = "CC_NHAP_TOA_DO", CcKeyPhac = "CC_PHAC_THAO", CcKeyPhacDiem = "CC_PHAC_DIEM",
            CcKeyKhuonVien = "CC_KHUON_VIEN", CcKeyTuyen = "CC_TUYEN", CcKeyVung = "CC_VUNG_TIEP_CAN",
            CcKeyMatCat = "CC_MAT_CAT", CcKeyLuoi = "CC_LUOI_TOA_DO", CcKeyLichSu = "CC_LICH_SU_VI_TRI", CcKeyDiemTam = "CC_DIEM_TAM";

        private Button ccNut;
        private ContextMenuStrip ccMenu;
        private ToolStripMenuItem ccMucLuoi, ccMucKhuonVien;
        private CcCheDo ccCheDo = CcCheDo.None;
        private readonly List<Tuple<double, double, string>> ccDiem = new List<Tuple<double, double, string>>();
        private string ccMaKhuonVien, ccTenKhuonVien;
        private tkCursorMode ccCheDoChuotTruoc = tkCursorMode.cmPan;
        private int ccLopNhap = -1, ccLopPhac = -1, ccLopPhacDiem = -1, ccLopKhuonVien = -1, ccLopTuyen = -1,
            ccLopVung = -1, ccLopMatCat = -1, ccLopLuoi = -1, ccLopLichSu = -1, ccLopDiemTam = -1;
        private bool ccHienLuoi, ccHienKhuonVien = true;
        private List<KhuonVien> ccKhuonVien;
        private System.Windows.Forms.Timer ccHenLuoi;
        private List<DonViRutGon> ccDanhSachDonVi;

        // =====================================================
        // 1. NUT + MENU
        // =====================================================

        /// <summary>Tao nut cong cu (goi 1 lan khi thanh cong cu ban do san sang).</summary>
        private Button CongCu_LayNut()
        {
            if (ccNut != null && !ccNut.IsDisposed) return ccNut;
            ccNut = new Button
            {
                Text = "⚒",
                Font = new Font("Segoe UI Symbol", 11F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TabStop = false,
                UseVisualStyleBackColor = false
            };
            ccNut.FlatAppearance.BorderSize = 0;
            pnlMapTools.Controls.Add(ccNut);
            CongCu_TaoMenu();
            ccNut.Click += (s, e) => ccMenu.Show(ccNut, new WinPoint(-ccMenu.PreferredSize.Width - 4, 0));
            return ccNut;
        }

        private void CongCu_TaoMenu()
        {
            ccMenu = new ContextMenuStrip { ShowImageMargin = false, ShowCheckMargin = true, Font = new Font("Segoe UI", 9.5F) };
            ccMenu.Renderer = new ToolStripProfessionalRenderer(new MauMenuV2());
            Action<string> tieuDe = t => ccMenu.Items.Add(new ToolStripLabel(t) { Font = new Font("Segoe UI", 8F, FontStyle.Bold), ForeColor = VeBaoCao.Teal, Margin = new Padding(6, 6, 0, 2) });
            Func<string, Action, ToolStripMenuItem> muc = (t, a) =>
            {
                ToolStripMenuItem m = new ToolStripMenuItem(t) { Padding = new Padding(4, 3, 4, 3) };
                m.Click += (s, e) => { try { a(); } catch (Exception ex) { CongCu_BaoLoi(ex); } };
                ccMenu.Items.Add(m);
                return m;
            };

            tieuDe("TRẮC ĐỊA");
            muc("Nhập tọa độ trụ sở GNSS / CSV / KML…", CongCu_MoNhapToaDo);
            muc("Lịch sử vị trí của đơn vị…", () => CongCu_ChonDonVi("Lịch sử vị trí", d => CongCu_MoLichSu(d.Ma, d.Ten)));
            ccMucLuoi = muc("Lưới tọa độ UTM 48N (km)", () => CongCu_DoiLuoi(!ccHienLuoi));
            ccMucLuoi.CheckOnClick = false;
            ccMenu.Items.Add(new ToolStripSeparator());
            tieuDe("KHUÔN VIÊN DOANH TRẠI");
            muc("Số hóa khuôn viên đơn vị…", () => CongCu_ChonDonVi("Số hóa khuôn viên", d => CongCu_BatSoHoaKhuonVien(d.Ma, d.Ten)));
            ccMucKhuonVien = muc("Hiện lớp khuôn viên", () => CongCu_DoiHienKhuonVien(!ccHienKhuonVien));
            ccMucKhuonVien.Checked = ccHienKhuonVien;
            muc("Xóa khuôn viên của đơn vị…", () => CongCu_ChonDonVi("Xóa khuôn viên", d => CongCu_XoaKhuonVien(d.Ma, d.Ten)));
            ccMenu.Items.Add(new ToolStripSeparator());
            tieuDe("MẠNG ĐƯỜNG BỘ (pgRouting)");
            muc("Tìm đường giữa 2 điểm / 2 đơn vị", () => CongCu_BatChon(CcCheDo.Tuyen, "TÌM ĐƯỜNG: nhấp điểm ĐẦU rồi điểm CUỐI (nhấp vào logo để chọn đơn vị)."));
            muc("Đường đến cơ sở quân y gần nhất", () => CongCu_BatChon(CcCheDo.QuanY, "QUÂN Y GẦN NHẤT: nhấp điểm xuất phát (hoặc logo đơn vị)."));
            muc("Vùng tiếp cận 5 / 10 / 15 phút", () => CongCu_BatChon(CcCheDo.TiepCan, "VÙNG TIẾP CẬN: nhấp điểm xuất phát."));
            ccMenu.Items.Add(new ToolStripSeparator());
            tieuDe("ĐỊA HÌNH (SRTM 30 m)");
            muc("Mặt cắt địa hình && tầm nhìn thông", () => CongCu_BatChon(CcCheDo.MatCat, "MẶT CẮT: nhấp điểm A (trạm) rồi điểm B (mục tiêu)."));
            ccMenu.Items.Add(new ToolStripSeparator());
            tieuDe("XUẤT BẢN");
            muc("In bản đồ A4 / A3 (PDF)…", CongCu_MoInBanDo);
            muc("Xuất dữ liệu GIS (GeoJSON · KML · SHP · CSV)…", CongCu_MoXuatDuLieu);
            ccMenu.Items.Add(new ToolStripSeparator());
            muc("Xóa các lớp kết quả tạm", CongCu_XoaLopTam);
        }

        private sealed class MauMenuV2 : ProfessionalColorTable
        {
            public override Color MenuItemSelected { get { return Color.FromArgb(226, 246, 241); } }
            public override Color MenuItemBorder { get { return Color.FromArgb(160, 220, 200); } }
            public override Color ToolStripDropDownBackground { get { return Color.White; } }
            public override Color MenuBorder { get { return Color.FromArgb(215, 227, 237); } }
        }

        private void CongCu_BaoLoi(Exception ex)
        {
            GhiNhatKyBanDo("CONG_CU_GIS_LOI | " + ex);
            statusGeneral.Text = "● Lỗi công cụ GIS: " + ex.Message;
            MessageBox.Show(this, ex.Message, "Công cụ GIS", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // =====================================================
        // 2. CHON DON VI / CHON DIEM TREN BAN DO
        // =====================================================

        private async void CongCu_ChonDonVi(string tieuDe, Action<DonViRutGon> xong)
        {
            try
            {
                if (ccDanhSachDonVi == null)
                    ccDanhSachDonVi = await Task.Run(() => new TruyXuatTracDia().LayDanhSachDonVi());
                using (FormChonDonVi f = new FormChonDonVi(tieuDe + " – chọn đơn vị", ccDanhSachDonVi))
                    if (f.ShowDialog(this) == DialogResult.OK && f.DaChon != null) xong(f.DaChon);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        private void CongCu_BatChon(CcCheDo cheDo, string huongDan)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            if (phanHeHienTai != "Bản đồ GIS") ChonPhanHe(btnNavMap, "Bản đồ GIS");
            CongCu_TatChon(false);
            ccCheDo = cheDo;
            ccDiem.Clear();
            ccCheDoChuotTruoc = banDoMapWinGIS.CursorMode;
            banDoMapWinGIS.CursorMode = tkCursorMode.cmNone;
            banDoMapWinGIS.MapCursor = tkCursor.crsrCross;
            banDoMapWinGIS.SendMouseDown = true;
            banDoMapWinGIS.SendMouseUp = true;
            Logo_DangKyNhanDiem();
            statusGeneral.Text = "● " + huongDan + "  ·  Chuột PHẢI để hủy / hoàn tất.";
        }

        private void CongCu_TatChon(bool xoaPhac)
        {
            if (ccCheDo == CcCheDo.None) return;
            ccCheDo = CcCheDo.None;
            if (xoaPhac)
            {
                Logo_XoaLop(ref ccLopPhac, CcKeyPhac);
                Logo_XoaLop(ref ccLopPhacDiem, CcKeyPhacDiem);
                Logo_XoaLop(ref ccLopDiemTam, CcKeyDiemTam);
            }
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            banDoMapWinGIS.CursorMode = ccCheDoChuotTruoc;
            banDoMapWinGIS.MapCursor = tkCursor.crsrArrow;
            banDoMapWinGIS.Redraw();
        }

        /// <summary>Goi tu MouseUp cua ban do. true = da xu ly (khong mo the logo).</summary>
        private bool CongCu_XuLyNhanChuot(int nut, int px, int py)
        {
            if (ccCheDo == CcCheDo.None) return false;
            if (nut == 2)
            {
                if (ccCheDo == CcCheDo.KhuonVien && ccDiem.Count > 0) { CongCu_MenuKhuonVien(px, py); return true; }
                CongCu_TatChon(true);
                statusGeneral.Text = "● Đã hủy thao tác công cụ GIS.";
                return true;
            }
            if (nut != 1) return true;

            double x = 0, y = 0;
            string nhan = null;
            bool laBchqs;
            LogoDiem hit = ccCheDo == CcCheDo.KhuonVien ? null : Logo_TimDiemGanPixel(px, py, out laBchqs);
            if (hit != null) { x = hit.X; y = hit.Y; nhan = hit.Ten; }
            else banDoMapWinGIS.PixelToProj(px, py, ref x, ref y);
            ccDiem.Add(Tuple.Create(x, y, nhan ?? ("Điểm " + (ccDiem.Count + 1))));

            switch (ccCheDo)
            {
                case CcCheDo.KhuonVien:
                    CongCu_VePhacKhuonVien();
                    break;
                case CcCheDo.Tuyen:
                case CcCheDo.MatCat:
                    CongCu_VeDiemTam();
                    if (ccDiem.Count == 1)
                        statusGeneral.Text = "● Đã chọn điểm đầu: " + ccDiem[0].Item3 + " · nhấp điểm thứ hai.";
                    else
                    {
                        CcCheDo cd = ccCheDo;
                        List<Tuple<double, double, string>> hai = ccDiem.ToList();
                        CongCu_TatChon(false);
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            if (cd == CcCheDo.Tuyen) CongCu_TimDuong(hai[0], hai[1]);
                            else CongCu_MatCat(hai[0], hai[1]);
                        }));
                    }
                    break;
                case CcCheDo.QuanY:
                case CcCheDo.TiepCan:
                    {
                        CcCheDo cd = ccCheDo;
                        Tuple<double, double, string> d = ccDiem[0];
                        CongCu_VeDiemTam();
                        CongCu_TatChon(false);
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            if (cd == CcCheDo.QuanY) CongCu_QuanYGanNhat(d);
                            else CongCu_TiepCan(d);
                        }));
                        break;
                    }
            }
            return true;
        }

        private void CongCu_VeDiemTam()
        {
            CongCu_VeLop(ref ccLopDiemTam, CcKeyDiemTam, ShpfileType.SHP_POINT,
                ccDiem.Select(d => "POINT(" + Dd(d.Item1) + " " + Dd(d.Item2) + ")").ToList(),
                Color.FromArgb(255, 255, 255), Color.FromArgb(230, 70, 90), 2f, 255, ccDiem.Select((d, i) => (i == 0 ? "A · " : "B · ") + d.Item3).ToList(), 12f);
        }

        // =====================================================
        // 3. LOP KET QUA (WKT -> shapefile trong bo nho)
        // =====================================================

        private static string Dd(double v) { return v.ToString("0.###", CultureInfo.InvariantCulture); }

        /// <summary>Ve lai mot lop tam tu danh sach WKT (EPSG:32648). trongSuotNen: 0 = trong suot .. 255 = dac.</summary>
        private void CongCu_VeLop(ref int handle, string key, ShpfileType kieu, IList<string> wkts,
            Color mauVien, Color mauNen, float rongVien, float trongSuotNen, IList<string> nhan = null,
            float coDiem = 0f, tkDashStyle net = tkDashStyle.dsSolid, Color? mauChu = null,
            IList<Color> mauRieng = null, bool nhanTrenDinh = false)
        {
            Logo_XoaLop(ref handle, key);
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed || wkts == null || wkts.Count == 0) return;
            Shapefile sf = new Shapefile();
            if (!sf.CreateNewWithShapeID("", kieu)) throw new InvalidOperationException("Không tạo được lớp " + key);
            if (banDoMapWinGIS.GeoProjection != null) sf.GeoProjection = banDoMapWinGIS.GeoProjection.Clone();
            for (int i = 0; i < wkts.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(wkts[i])) continue;
                Shape s = new Shape();
                if (!s.ImportFromWKT(wkts[i])) continue;
                int idx = sf.EditAddShape(s);
                if (nhan != null && i < nhan.Count && !string.IsNullOrEmpty(nhan[i]) && idx >= 0)
                {
                    MapWinGIS.Point c = kieu == ShpfileType.SHP_POINT ? s.get_Point(0) : s.Centroid;
                    if (c != null) sf.Labels.AddLabel(nhan[i], c.x, nhanTrenDinh ? s.Extents.yMax : c.y);
                }
                if (mauRieng != null && i < mauRieng.Count && idx >= 0)
                {
                    ShapefileCategory cat = sf.Categories.Add(key + "_" + i);
                    cat.DrawingOptions.FillColor = Logo_MauOle(mauRieng[i]);
                    cat.DrawingOptions.LineColor = Logo_MauOle(mauRieng[i]);
                    cat.DrawingOptions.FillTransparency = trongSuotNen;
                    cat.DrawingOptions.LineWidth = rongVien;
                    cat.DrawingOptions.FillVisible = kieu != ShpfileType.SHP_POLYLINE && trongSuotNen > 0;
                    sf.set_ShapeCategory(idx, sf.Categories.Count - 1);
                }
            }
            ShapeDrawingOptions o = sf.DefaultDrawingOptions;
            o.LineColor = Logo_MauOle(mauVien);
            o.LineWidth = rongVien;
            o.LineStipple = net;
            o.FillColor = Logo_MauOle(mauNen);
            o.FillTransparency = trongSuotNen;
            o.FillVisible = kieu != ShpfileType.SHP_POLYLINE && trongSuotNen > 0;
            if (kieu == ShpfileType.SHP_POINT)
            {
                o.PointSize = coDiem > 0 ? coDiem : 10f;
                o.PointShape = tkPointShapeType.ptShapeCircle;
                o.FillVisible = true;
                o.FillTransparency = 255;
                o.LineVisible = true;
            }
            if (sf.Labels.Count > 0)
            {
                sf.Labels.FontSize = 9;
                sf.Labels.FontBold = true;
                sf.Labels.FontColor = Logo_MauOle(mauChu ?? Color.FromArgb(22, 49, 74));
                sf.Labels.FrameVisible = true;
                sf.Labels.FrameBackColor = Logo_MauOle(Color.White);
                sf.Labels.FrameTransparency = 225;
                sf.Labels.FrameOutlineColor = Logo_MauOle(mauVien);
                sf.Labels.OffsetY = kieu == ShpfileType.SHP_POINT ? -16 : 0;
                sf.Labels.AvoidCollisions = false;
                sf.Labels.Visible = true;
            }
            int h = banDoMapWinGIS.AddLayer(sf, true);
            if (h < 0) return;
            banDoMapWinGIS.set_LayerName(h, key);
            banDoMapWinGIS.set_LayerKey(h, key);
            banDoMapWinGIS.set_LayerSkipOnSaving(h, true);
            handle = h;
            Logo_SapXepThuTuLop();
            banDoMapWinGIS.Redraw();
        }

        private void CongCu_XoaLopTam()
        {
            CongCu_TatChon(true);
            Logo_XoaLop(ref ccLopNhap, CcKeyNhap);
            Logo_XoaLop(ref ccLopTuyen, CcKeyTuyen);
            Logo_XoaLop(ref ccLopVung, CcKeyVung);
            Logo_XoaLop(ref ccLopMatCat, CcKeyMatCat);
            Logo_XoaLop(ref ccLopLichSu, CcKeyLichSu);
            Logo_XoaLop(ref ccLopLichSuDuong, CcKeyLichSu + "_DUONG");
            Logo_XoaLop(ref ccLopDiemTam, CcKeyDiemTam);
            if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed) banDoMapWinGIS.Redraw();
            statusGeneral.Text = "● Đã xóa các lớp kết quả tạm.";
        }

        /// <summary>Phong ban do toi khung bao cac WKT (them le 15%).</summary>
        private void CongCu_PhongToi(double xmin, double ymin, double xmax, double ymax)
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            double dx = Math.Max(400, xmax - xmin) * 0.15, dy = Math.Max(400, ymax - ymin) * 0.15;
            Extents ext = new Extents();
            ext.SetBounds(xmin - dx, ymin - dy, 0, xmax + dx, ymax + dy, 0);
            banDoMapWinGIS.Extents = ext;
        }

        private void CongCu_PhongToiLop(int handle)
        {
            if (!Logo_CoHandle(handle)) return;
            Extents e = banDoMapWinGIS.get_layerExtents(handle);
            if (e != null) CongCu_PhongToi(e.xMin, e.yMin, e.xMax, e.yMax);
        }

        /// <summary>Goi sau khi lop logo duoc ve lai: khoi phuc lop nen (khuon vien, luoi).</summary>
        private void CongCu_VeLaiLopNen()
        {
            try
            {
                if (ccHienKhuonVien && ccKhuonVien != null) CongCu_VeKhuonVien();
                if (ccHienLuoi) CongCu_VeLuoi();
            }
            catch (Exception ex) { GhiNhatKyBanDo("CONG_CU_VE_LAI_FAILED | " + ex.Message); }
        }

        // =====================================================
        // 4. NHAP TOA DO + LICH SU
        // =====================================================

        private void CongCu_MoNhapToaDo()
        {
            if (!CoQuyen(ManagerGISAcademy.BLL.ThaoTacDonViQuanDoi.Sua))
            {
                MessageBox.Show(this, "Vai trò hiện tại chỉ được xem, không được ghi vị trí.", "Nhập tọa độ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FormNhapToaDo f = new FormNhapToaDo();
            f.YeuCauHienTrenBanDo += ds =>
            {
                CongCu_VeLop(ref ccLopNhap, CcKeyNhap, ShpfileType.SHP_POINT,
                    ds.Select(d => "POINT(" + Dd(d.Item1) + " " + Dd(d.Item2) + ")").ToList(),
                    Color.White, Color.FromArgb(39, 126, 205), 1.5f, 255, ds.Select(d => d.Item3 + (d.Item4 ? "" : " (lỗi)")).ToList(), 11f);
                if (ds.Count > 0) CongCu_PhongToi(ds.Min(d => d.Item1), ds.Min(d => d.Item2), ds.Max(d => d.Item1), ds.Max(d => d.Item2));
            };
            f.DaGhi += () => QuanTri_NapLaiSauThayDoi();
            f.FormClosed += (s, e) => f.Dispose();
            f.Show(this);
        }

        private async void CongCu_MoLichSu(string ma, string ten)
        {
            try
            {
                List<DongLichSuViTri> ls = await Task.Run(() => new TruyXuatTracDia().LichSuViTri(ma));
                FormLichSuViTri f = new FormLichSuViTri(ma, ten, ls);
                f.YeuCauHienTrenBanDo += ds => CongCu_VeLichSu(ds, ten);
                f.FormClosed += (s, e) => f.Dispose();
                f.Show(this);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        private void CongCu_VeLichSu(List<DongLichSuViTri> ds, string ten)
        {
            if (ds == null || ds.Count == 0) return;
            if (phanHeHienTai != "Bản đồ GIS") ChonPhanHe(btnNavMap, "Bản đồ GIS");
            List<string> wkt = ds.Select(d => "POINT(" + Dd(d.E) + " " + Dd(d.N) + ")").ToList();
            List<string> nhan = ds.Select(d => d.Stt + ". " + d.HieuLucTu.ToString("dd/MM/yy") + (d.LaHienHanh ? " (hiện hành)" : "")).ToList();
            if (ds.Count > 1)
            {
                wkt.Add("LINESTRING(" + string.Join(", ", ds.Select(d => Dd(d.E) + " " + Dd(d.N))) + ")");
                nhan.Add(null);
            }
            // Diem (cam) + duong dich chuyen.
            CongCu_VeLop(ref ccLopLichSu, CcKeyLichSu, ShpfileType.SHP_POINT, wkt.Take(ds.Count).ToList(),
                Color.White, Color.FromArgb(209, 127, 35), 1.5f, 255, nhan, 11f);
            if (ds.Count > 1)
            {
                CongCu_VeLop(ref ccLopLichSuDuong, CcKeyLichSu + "_DUONG", ShpfileType.SHP_POLYLINE, new List<string> { wkt.Last() },
                    Color.FromArgb(209, 127, 35), Color.Empty, 2.4f, 0, null, 0, tkDashStyle.dsDash);
            }
            CongCu_PhongToi(ds.Min(d => d.E), ds.Min(d => d.N), ds.Max(d => d.E), ds.Max(d => d.N));
            statusGeneral.Text = "● Lịch sử vị trí " + ten + ": " + ds.Count + " lần định vị (điểm cam, nét đứt = đường dịch chuyển).";
        }

        // =====================================================
        // 5. LUOI TOA DO UTM
        // =====================================================

        private void CongCu_DoiLuoi(bool hien)
        {
            ccHienLuoi = hien;
            if (ccMucLuoi != null) ccMucLuoi.Checked = hien;
            if (ccHenLuoi == null)
            {
                ccHenLuoi = new System.Windows.Forms.Timer { Interval = 300 };
                ccHenLuoi.Tick += (s, e) => { ccHenLuoi.Stop(); if (ccHienLuoi) CongCu_VeLuoi(); };
                banDoMapWinGIS.ExtentsChanged += (s, e) => { if (ccHienLuoi) { ccHenLuoi.Stop(); ccHenLuoi.Start(); } };
            }
            if (hien) CongCu_VeLuoi();
            else { Logo_XoaLop(ref ccLopLuoi, CcKeyLuoi); Logo_XoaLop(ref ccLopLuoiNhan, CcKeyLuoi + "_NHAN"); banDoMapWinGIS.Redraw(); }
            statusGeneral.Text = hien ? "● Lưới tọa độ UTM 48N (WGS84, đơn vị km) – tự đổi bước lưới theo tỷ lệ." : "● Đã tắt lưới tọa độ.";
        }

        private void CongCu_VeLuoi()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            Extents e = banDoMapWinGIS.Extents;
            double w = e.xMax - e.xMin;
            double[] buoc = { 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000 };
            double b = buoc.FirstOrDefault(k => w / k <= 8);
            if (b <= 0) b = 50000;
            List<string> wkt = new List<string>();
            List<string> nhan = new List<string>();
            double dy = (e.yMax - e.yMin) * 0.03, dx = w * 0.02;
            string dv = b >= 1000 ? "0" : "0.0";
            for (double x = Math.Ceiling(e.xMin / b) * b; x <= e.xMax; x += b)
            {
                wkt.Add("LINESTRING(" + Dd(x) + " " + Dd(e.yMin) + ", " + Dd(x) + " " + Dd(e.yMax) + ")");
                nhan.Add(null);
                wkt.Add("POINT(" + Dd(x) + " " + Dd(e.yMin + dy * 3.2) + ")");
                nhan.Add("E " + (x / 1000d).ToString(dv, CultureInfo.InvariantCulture));
            }
            for (double y = Math.Ceiling(e.yMin / b) * b; y <= e.yMax; y += b)
            {
                wkt.Add("LINESTRING(" + Dd(e.xMin) + " " + Dd(y) + ", " + Dd(e.xMax) + " " + Dd(y) + ")");
                nhan.Add(null);
                wkt.Add("POINT(" + Dd(e.xMin + dx * 2.2) + " " + Dd(y) + ")");
                nhan.Add("N " + (y / 1000d).ToString(dv, CultureInfo.InvariantCulture));
            }
            // Duong luoi (polyline) + nhan (diem an).
            CongCu_VeLop(ref ccLopLuoi, CcKeyLuoi, ShpfileType.SHP_POLYLINE, wkt.Where(s => s.StartsWith("LINESTRING")).ToList(),
                Color.FromArgb(150, 40, 70, 120), Color.Empty, 1f, 0, null, 0, tkDashStyle.dsDot);
            List<string> diemNhan = new List<string>(); List<string> chuNhan = new List<string>();
            for (int i = 0; i < wkt.Count; i++) if (wkt[i].StartsWith("POINT")) { diemNhan.Add(wkt[i]); chuNhan.Add(nhan[i]); }
            CongCu_VeLop(ref ccLopLuoiNhan, CcKeyLuoi + "_NHAN", ShpfileType.SHP_POINT, diemNhan, Color.FromArgb(40, 70, 120), Color.FromArgb(40, 70, 120), 0.5f, 255, chuNhan, 1f, tkDashStyle.dsSolid, Color.FromArgb(40, 70, 120));
        }

        private int ccLopLuoiNhan = -1, ccLopLichSuDuong = -1;
        private bool ccDaNapKhuonVienLanDau;

        /// <summary>Goi ngay sau khi lop logo duoc tao lai (Native_DongBoLogoTatCa).</summary>
        private void CongCu_SauKhiVeLogo()
        {
            if (!ccHienKhuonVien) return;
            if (!ccDaNapKhuonVienLanDau)
            {
                ccDaNapKhuonVienLanDau = true;
                BeginInvoke(new MethodInvoker(async () => await CongCu_NapKhuonVien()));
                return;
            }
            if (ccKhuonVien != null && !Logo_CoHandle(ccLopKhuonVien)) CongCu_VeKhuonVien();
        }

        // =====================================================
        // 6. KHUON VIEN
        // =====================================================

        private async void CongCu_DoiHienKhuonVien(bool hien)
        {
            ccHienKhuonVien = hien;
            if (ccMucKhuonVien != null) ccMucKhuonVien.Checked = hien;
            if (!hien) { Logo_XoaLop(ref ccLopKhuonVien, CcKeyKhuonVien); banDoMapWinGIS.Redraw(); return; }
            await CongCu_NapKhuonVien();
        }

        private async Task CongCu_NapKhuonVien()
        {
            try
            {
                ccKhuonVien = await Task.Run(() => new TruyXuatTracDia().LayKhuonVien());
                CongCu_VeKhuonVien();
                statusGeneral.Text = "● Lớp khuôn viên: " + ccKhuonVien.Count + " doanh trại đã số hóa.";
            }
            catch (Exception ex) { GhiNhatKyBanDo("KHUON_VIEN_NAP_FAILED | " + ex.Message); }
        }

        private void CongCu_VeKhuonVien()
        {
            if (ccKhuonVien == null) return;
            CongCu_VeLop(ref ccLopKhuonVien, CcKeyKhuonVien, ShpfileType.SHP_POLYGON,
                ccKhuonVien.Select(k => k.Wkt).ToList(), Color.FromArgb(0, 140, 112), Color.FromArgb(0, 184, 132), 2f, 55,
                ccKhuonVien.Select(k => k.TenDonVi + "\n" + (k.DienTichM2 / 10000d).ToString("0.0#", CultureInfo.GetCultureInfo("vi-VN")) + " ha").ToList(),
                0, tkDashStyle.dsSolid, Color.FromArgb(0, 110, 90), null, true);
        }

        private void CongCu_BatSoHoaKhuonVien(string ma, string ten)
        {
            ccMaKhuonVien = ma; ccTenKhuonVien = ten;
            DonViRutGon d = ccDanhSachDonVi == null ? null : ccDanhSachDonVi.FirstOrDefault(x => x.Ma == ma);
            CongCu_BatChon(CcCheDo.KhuonVien, "SỐ HÓA KHUÔN VIÊN " + ten + ": nhấp lần lượt các góc tường rào; chuột PHẢI để hoàn tất / xóa điểm cuối / hủy.");
            double xt, yt;
            if (d != null) CongCu_PhongToi(d.X - 450, d.Y - 450, d.X + 450, d.Y + 450);
            else if (ttDvHoSoDangHienThi != null && ttDvHoSoDangHienThi.MaDonVi == ma &&
                     Native_Wgs84SangToaDoBanDo(ttDvHoSoDangHienThi.KinhDo, ttDvHoSoDangHienThi.ViDo, out xt, out yt))
                CongCu_PhongToi(xt - 450, yt - 450, xt + 450, yt + 450);
        }

        private void CongCu_VePhacKhuonVien()
        {
            List<string> w = new List<string>();
            if (ccDiem.Count >= 3)
                w.Add("POLYGON((" + string.Join(", ", ccDiem.Concat(new[] { ccDiem[0] }).Select(p => Dd(p.Item1) + " " + Dd(p.Item2))) + "))");
            else if (ccDiem.Count == 2)
                w.Add("LINESTRING(" + string.Join(", ", ccDiem.Select(p => Dd(p.Item1) + " " + Dd(p.Item2))) + ")");
            if (w.Count > 0)
                CongCu_VeLop(ref ccLopPhac, CcKeyPhac, ccDiem.Count >= 3 ? ShpfileType.SHP_POLYGON : ShpfileType.SHP_POLYLINE, w,
                    Color.FromArgb(230, 70, 90), Color.FromArgb(255, 200, 80), 2.2f, 70, null, 0, tkDashStyle.dsDash);
            CongCu_VeLop(ref ccLopPhacDiem, CcKeyPhacDiem, ShpfileType.SHP_POINT,
                ccDiem.Select(p => "POINT(" + Dd(p.Item1) + " " + Dd(p.Item2) + ")").ToList(),
                Color.FromArgb(230, 70, 90), Color.White, 2f, 255, null, 8f);
            string dt = "";
            if (ccDiem.Count >= 3)
            {
                double a = 0;
                for (int i = 0; i < ccDiem.Count; i++)
                {
                    var p = ccDiem[i]; var q = ccDiem[(i + 1) % ccDiem.Count];
                    a += p.Item1 * q.Item2 - q.Item1 * p.Item2;
                }
                dt = " · diện tích ≈ " + (Math.Abs(a) / 2d).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " m²";
            }
            statusGeneral.Text = "● Khuôn viên " + ccTenKhuonVien + ": " + ccDiem.Count + " đỉnh" + dt + " · chuột PHẢI để hoàn tất.";
        }

        private void CongCu_MenuKhuonVien(int px, int py)
        {
            ContextMenuStrip m = new ContextMenuStrip { ShowImageMargin = false, Font = new Font("Segoe UI", 9.5F) };
            ToolStripMenuItem luu = new ToolStripMenuItem("✓  Hoàn tất và lưu khuôn viên (" + ccDiem.Count + " đỉnh)") { Enabled = ccDiem.Count >= 3 };
            luu.Click += (s, e) => BeginInvoke(new MethodInvoker(CongCu_LuuKhuonVien));
            ToolStripMenuItem lui = new ToolStripMenuItem("↶  Xóa đỉnh cuối");
            lui.Click += (s, e) => { if (ccDiem.Count > 0) ccDiem.RemoveAt(ccDiem.Count - 1); CongCu_VePhacKhuonVien(); };
            ToolStripMenuItem huy = new ToolStripMenuItem("✕  Hủy số hóa");
            huy.Click += (s, e) => { CongCu_TatChon(true); statusGeneral.Text = "● Đã hủy số hóa khuôn viên."; };
            m.Items.Add(luu); m.Items.Add(lui); m.Items.Add(new ToolStripSeparator()); m.Items.Add(huy);
            m.Closed += (s, e) => BeginInvoke(new MethodInvoker(m.Dispose));
            m.Show(banDoMapWinGIS, new WinPoint(px, py));
        }

        private async void CongCu_LuuKhuonVien()
        {
            if (ccDiem.Count < 3) return;
            string wkt = "POLYGON((" + string.Join(", ", ccDiem.Concat(new[] { ccDiem[0] }).Select(p => Dd(p.Item1) + " " + Dd(p.Item2))) + "))";
            string ma = ccMaKhuonVien, ten = ccTenKhuonVien;
            CongCu_TatChon(true);
            try
            {
                KetQuaLuuKhuonVien kq = await Task.Run(() => new TruyXuatTracDia().LuuKhuonVien(ma, wkt, null, "Số hóa trên ảnh vệ tinh"));
                CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
                statusGeneral.Text = "● Đã lưu khuôn viên " + ten + ": " + (kq.DienTichM2 / 10000d).ToString("N2", vi) + " ha, chu vi " +
                    kq.ChuViM.ToString("N0", vi) + " m" + (kq.ChuaTruSo ? "" : " · CẢNH BÁO: trụ sở nằm ngoài khuôn viên");
                ccHienKhuonVien = true;
                if (ccMucKhuonVien != null) ccMucKhuonVien.Checked = true;
                await CongCu_NapKhuonVien();
                if (!kq.ChuaTruSo)
                    MessageBox.Show(this, "Đã lưu, nhưng điểm trụ sở hiện tại nằm NGOÀI khuôn viên vừa số hóa.\nNên dùng \"Định vị lại trụ sở\" để đặt điểm vào cổng chính.",
                        "Khuôn viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        private async void CongCu_XoaKhuonVien(string ma, string ten)
        {
            if (MessageBox.Show(this, "Chuyển khuôn viên của " + ten + " sang lưu trữ (không xóa vĩnh viễn)?", "Xóa khuôn viên",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                int n = await Task.Run(() => new TruyXuatTracDia().XoaKhuonVien(ma));
                statusGeneral.Text = n > 0 ? "● Đã chuyển khuôn viên " + ten + " sang lưu trữ." : "● " + ten + " chưa có khuôn viên.";
                await CongCu_NapKhuonVien();
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        // =====================================================
        // 7. MANG DUONG
        // =====================================================

        private async void CongCu_TimDuong(Tuple<double, double, string> a, Tuple<double, double, string> b)
        {
            statusGeneral.Text = "● Đang tìm đường bằng pgRouting (Dijkstra, chi phí = thời gian)…";
            try
            {
                KetQuaTuyen t = await Task.Run(() => new TruyXuatTracDia().DinhTuyen(a.Item1, a.Item2, b.Item1, b.Item2));
                if (t == null) { statusGeneral.Text = "● Không tìm được đường."; return; }
                CongCu_VeTuyen(t, a.Item3 + " → " + b.Item3);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        private async void CongCu_QuanYGanNhat(Tuple<double, double, string> a)
        {
            statusGeneral.Text = "● Đang tìm cơ sở quân y gần nhất theo mạng đường…";
            try
            {
                KetQuaTuyen t = await Task.Run(() => new TruyXuatTracDia().DenQuanYGanNhat(a.Item1, a.Item2));
                if (t == null) { statusGeneral.Text = "● Không tìm được cơ sở quân y nối mạng đường."; return; }
                CongCu_VeTuyen(t, a.Item3 + " → " + t.TenDich);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        private void CongCu_VeTuyen(KetQuaTuyen t, string moTa)
        {
            CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
            CongCu_VeLop(ref ccLopTuyen, CcKeyTuyen, ShpfileType.SHP_POLYLINE, new List<string> { t.Wkt },
                Color.FromArgb(39, 126, 205), Color.Empty, 5f, 0);
            CongCu_PhongToiLop(ccLopTuyen);
            string thongTin = moTa + ": " + (t.DaiM / 1000d).ToString("N2", vi) + " km · ≈ " + Math.Ceiling(t.Phut).ToString("N0", vi) + " phút" +
                (string.IsNullOrEmpty(t.TenDuong) ? "" : " · qua " + t.TenDuong);
            statusGeneral.Text = "● " + thongTin;
            MessageBox.Show(this, thongTin + "\n\nTốc độ giả định theo cấp đường OSM (cao tốc 80, trục chính 45–60, khu dân cư 22 km/h); " +
                "đường một chiều được tôn trọng.", "Kết quả định tuyến", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void CongCu_TiepCan(Tuple<double, double, string> a)
        {
            statusGeneral.Text = "● Đang tính vùng tiếp cận 5/10/15 phút (pgr_drivingDistance)…";
            try
            {
                List<VungTiepCan> ds = await Task.Run(() => new TruyXuatTracDia().VungTiepCan(a.Item1, a.Item2, new[] { 5, 10, 15 }));
                if (ds.Count == 0) { statusGeneral.Text = "● Không dựng được vùng tiếp cận."; return; }
                CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
                ds = ds.OrderByDescending(v => v.Phut).ToList();   // vung lon ve truoc, vung 5 phut nam tren
                Color[] mauVung = ds.Select(v => v.Phut <= 5 ? Color.FromArgb(0, 168, 120) : v.Phut <= 10 ? Color.FromArgb(240, 170, 40) : Color.FromArgb(220, 80, 90)).ToArray();
                CongCu_VeLop(ref ccLopVung, CcKeyVung, ShpfileType.SHP_POLYGON, ds.Select(v => v.Wkt).ToList(),
                    Color.FromArgb(109, 90, 205), Color.FromArgb(109, 90, 205), 1.6f, 45,
                    ds.Select(v => v.Phut + " phút · " + v.DienTichKm2.ToString("N1", vi) + " km²").ToList(),
                    0, tkDashStyle.dsSolid, null, mauVung, true);
                CongCu_PhongToiLop(ccLopVung);
                statusGeneral.Text = "● Vùng tiếp cận từ " + a.Item3 + ": " +
                    string.Join(" · ", ds.OrderBy(v => v.Phut).Select(v => v.Phut + " phút = " + v.DienTichKm2.ToString("N1", vi) + " km²"));
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }

        // =====================================================
        // 8. MAT CAT DIA HINH
        // =====================================================

        private async void CongCu_MatCat(Tuple<double, double, string> a, Tuple<double, double, string> b)
        {
            statusGeneral.Text = "● Đang lấy mặt cắt địa hình từ DEM SRTM…";
            try
            {
                double L = Math.Sqrt(Math.Pow(b.Item1 - a.Item1, 2) + Math.Pow(b.Item2 - a.Item2, 2));
                int n = (int)Math.Max(60, Math.Min(800, L / 30));
                List<DiemMatCat> ds = await Task.Run(() => new TruyXuatTracDia().MatCat(a.Item1, a.Item2, b.Item1, b.Item2, n));
                CongCu_VeLop(ref ccLopMatCat, CcKeyMatCat, ShpfileType.SHP_POLYLINE,
                    new List<string> { "LINESTRING(" + Dd(a.Item1) + " " + Dd(a.Item2) + ", " + Dd(b.Item1) + " " + Dd(b.Item2) + ")" },
                    Color.FromArgb(209, 127, 35), Color.Empty, 3f, 0, null, 0, tkDashStyle.dsDash);
                statusGeneral.Text = "● Mặt cắt " + (L / 1000d).ToString("N2", CultureInfo.GetCultureInfo("vi-VN")) + " km, " + n + " điểm lấy mẫu.";
                FormMatCat f = new FormMatCat(ds, a.Item3, b.Item3);
                f.FormClosed += (s, e) => f.Dispose();
                f.Show(this);
            }
            catch (Exception ex) { CongCu_BaoLoi(ex); }
        }
    }
}
