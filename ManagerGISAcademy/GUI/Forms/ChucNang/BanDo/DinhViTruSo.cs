using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using MapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Chuc nang "Dinh vi lai tru so" + lop "vong tron sai so".
    ///
    /// Luong: the thong tin (logo) -> nut "DINH VI LAI TRU SO" -> hop thoai
    /// FormDinhViTruSo -> (tuy chon) nhap 1 diem tren ban do -> luu qua ham
    /// CSDL ql_don_vi.fn_dinh_vi_tru_so -> nap lai du lieu va logo.
    ///
    /// Vong tron sai so: ban kinh = do_chinh_xac_m quanh vi tri THAT
    /// (khong ve cho diem dai dien). Ban do EPSG:32648 nen don vi la met.
    /// </summary>
    public partial class FormMain
    {
        private const string DinhViKeyVongSaiSo = "HQTCSDL_VONG_SAI_SO";
        private const double DinhViBanKinhVeToiDaM = 1000d;

        private int dinhViLopVongSaiSo = -1;
        private bool dinhViDangChonDiem;
        private FormDinhViTruSo dinhViHopThoai;
        private tkCursorMode dinhViCheDoChuotTruoc = tkCursorMode.cmPan;

        // =====================================================
        // 1. MO HOP THOAI
        // =====================================================

        private void DinhVi_MoHopThoai(string maDonVi)
        {
            if (!CoQuyen(ThaoTacDonViQuanDoi.Sua, maDonVi))
            {
                MessageBox.Show(this, "Tài khoản chưa được cấp quyền định vị lại trụ sở.",
                    "Từ chối truy cập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ViTriHienHanh viTri;
            try
            {
                viTri = new TruyXuatViTri().LayViTriHienHanh(maDonVi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không đọc được vị trí hiện hành",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (viTri == null)
            {
                MessageBox.Show(this,
                    "Không tìm thấy vị trí hiện hành của đơn vị " + maDonVi + ".",
                    "Định vị lại trụ sở", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DinhVi_DongHopThoai();
            try
            {
                dinhViHopThoai = new FormDinhViTruSo(viTri);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Không mở được hộp thoại",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DinhVi_HienHopThoai();
        }

        private void DinhVi_HienHopThoai()
        {
            FormDinhViTruSo hop = dinhViHopThoai;
            if (hop == null || hop.IsDisposed || IsDisposed || Disposing) return;

            DialogResult kq = hop.ShowDialog(this);

            if (kq == DialogResult.Retry && hop.YeuCauChonTrenBanDo)
            {
                DinhVi_BatChonDiem();   // giu hop thoai de mo lai sau khi nhap diem
                return;
            }

            if (kq == DialogResult.OK && hop.KetQua != null && hop.KetQua.ViTriId < 0)
            {
                // Vai tro Bien tap: ham CSDL tra -id yeu cau, chua doi vi tri.
                string maCho = hop.MaDonVi;
                DinhVi_DongHopThoai();
                statusGeneral.Text = "● Đã gửi yêu cầu định vị lại " + maCho + " (#" + (-hop.KetQua.ViTriId) + "), chờ duyệt.";
                QuanTri_ThongBaoNeuChoDuyet("Định vị lại trụ sở " + maCho);
                return;
            }

            if (kq == DialogResult.OK && hop.KetQua != null)
            {
                KetQuaDinhVi r = hop.KetQua;
                string ma = hop.MaDonVi;
                statusGeneral.Text =
                    "● Đã định vị lại " + ma + ": " + r.TenXaPhuong +
                    ", ±" + r.DoChinhXacM.ToString("0.##", CultureInfo.InvariantCulture) + " m, dịch " +
                    r.KhoangDichM.ToString("0", CultureInfo.InvariantCulture) + " m" +
                    (r.DaDoiXa ? " (đã cập nhật phường/xã)." : ".");
                GhiNhatKyBanDo("DINH_VI_TRU_SO_OK | " + ma + " | vi_tri=" + r.ViTriId +
                    " | dich_m=" + r.KhoangDichM.ToString("0.0", CultureInfo.InvariantCulture));
                DinhVi_DongHopThoai();
                DinhVi_TaiLaiSauKhiLuu();
                return;
            }

            statusGeneral.Text = "● Đã đóng hộp thoại định vị, không thay đổi vị trí.";
            DinhVi_DongHopThoai();
        }

        private void DinhVi_DongHopThoai()
        {
            if (dinhViHopThoai != null && !dinhViHopThoai.IsDisposed)
                dinhViHopThoai.Dispose();
            dinhViHopThoai = null;
        }

        private void DinhVi_TaiLaiSauKhiLuu()
        {
            try
            {
                AnKhungThongTinDiem();
                // Xoa lop logo/vong sai so cu, nap lai du lieu -> ve lai.
                Native_XoaLopPhu169();
                Native_TaiMoiDuLieuPhanHe();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("DINH_VI_RELOAD_FAILED | " + ex);
                statusGeneral.Text = "● Đã lưu vị trí, nhưng chưa nạp lại được bản đồ: " + ex.Message;
            }
        }

        // =====================================================
        // 2. CHE DO NHAP DIEM TREN BAN DO
        // =====================================================

        private void DinhVi_BatChonDiem()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
            {
                DinhVi_HienHopThoai();
                return;
            }

            dinhViCheDoChuotTruoc = banDoMapWinGIS.CursorMode;
            banDoMapWinGIS.CursorMode = tkCursorMode.cmNone;
            banDoMapWinGIS.MapCursor = tkCursor.crsrCross;
            banDoMapWinGIS.SendMouseDown = true;
            banDoMapWinGIS.SendMouseUp = true;
            Logo_DangKyNhanDiem();
            dinhViDangChonDiem = true;

            statusGeneral.Text =
                "● ĐỊNH VỊ TRỤ SỞ: nhấp chuột TRÁI vào cổng chính trên bản đồ " +
                "(cuộn chuột để phóng to). Chuột PHẢI để quay lại hộp thoại.";
        }

        private void DinhVi_TatChonDiem()
        {
            dinhViDangChonDiem = false;
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            banDoMapWinGIS.CursorMode = dinhViCheDoChuotTruoc;
            banDoMapWinGIS.MapCursor = tkCursor.crsrArrow;
        }

        /// <summary>
        /// Goi tu su kien MouseUp cua ban do. Tra ve true neu da xu ly
        /// (dang o che do dinh vi), de lop logo khong mo the thong tin.
        /// </summary>
        private bool DinhVi_XuLyNhanChuot(int nut, int px, int py)
        {
            if (!dinhViDangChonDiem) return false;

            if (nut == 2)
            {
                DinhVi_TatChonDiem();
                statusGeneral.Text = "● Đã hủy chọn điểm trên bản đồ.";
                BeginInvoke(new MethodInvoker(DinhVi_HienHopThoai));
                return true;
            }

            if (nut != 1) return true;

            double x = 0d, y = 0d;
            banDoMapWinGIS.PixelToProj(px, py, ref x, ref y);
            DinhVi_TatChonDiem();

            if (dinhViHopThoai != null && !dinhViHopThoai.IsDisposed)
                dinhViHopThoai.DatDiemTuBanDo(x, y);   // ban do: EPSG:32648
            statusGeneral.Text = "● Đã lấy điểm trên bản đồ; kiểm tra lại trong hộp thoại rồi lưu.";

            // Mo lai hop thoai NGOAI su kien COM cua ban do.
            BeginInvoke(new MethodInvoker(DinhVi_HienHopThoai));
            return true;
        }

        // =====================================================
        // 3. LOP VONG TRON SAI SO
        // =====================================================

        private void DinhVi_XoaVongSaiSo()
        {
            Logo_XoaLop(ref dinhViLopVongSaiSo, DinhViKeyVongSaiSo);
        }

        private void DinhVi_VeVongSaiSo()
        {
            DinhVi_XoaVongSaiSo();
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                banDoMapWinGIS.GeoProjection == null) return;

            List<Tuple<double, double, double>> vong = new List<Tuple<double, double, double>>();

            if (nativeHoSoGisHqtcTraCuu != null)
                foreach (HoSoGisHqtc d in nativeHoSoGisHqtcTraCuu)
                    if (d != null)
                        DinhVi_ThemVong(vong, d.BanDo,
                            d.BanDo != null && d.BanDo.KinhDoThuc.HasValue ? d.BanDo.KinhDoThuc : d.KinhDo,
                            d.BanDo != null && d.BanDo.ViDoThuc.HasValue ? d.BanDo.ViDoThuc : d.ViDo);

            if (nativeDanhMucBchqsTraCuu != null)
                foreach (DanhMucBanChqsCapXa2026 d in nativeDanhMucBchqsTraCuu)
                    if (d != null && d.BanDo != null)
                        DinhVi_ThemVong(vong, d.BanDo, d.BanDo.KinhDoThuc, d.BanDo.ViDoThuc);

            if (vong.Count == 0) return;

            try
            {
                Shapefile sf = new Shapefile();
                if (!sf.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON))
                    throw new InvalidOperationException("Không tạo được lớp vòng sai số.");
                sf.GeoProjection = banDoMapWinGIS.GeoProjection.Clone();

                foreach (Tuple<double, double, double> v in vong)
                {
                    Shape s = new Shape();
                    if (!s.Create(ShpfileType.SHP_POLYGON)) continue;
                    const int soDinh = 48;
                    // Chieu kim dong ho (vong ngoai shapefile), dong vong.
                    for (int i = 0; i <= soDinh; i++)
                    {
                        double goc = -2d * Math.PI * i / soDinh;
                        s.AddPoint(v.Item1 + v.Item3 * Math.Cos(goc),
                                   v.Item2 + v.Item3 * Math.Sin(goc));
                    }
                    sf.EditAddShape(s);
                }

                ShapeDrawingOptions o = sf.DefaultDrawingOptions;
                o.FillVisible = true;
                o.FillColor = Logo_MauOle(Color.FromArgb(12, 160, 140));
                o.FillTransparency = 38f;     // 0..255
                o.LineVisible = true;
                o.LineColor = Logo_MauOle(Color.FromArgb(12, 130, 115));
                o.LineWidth = 1f;
                o.LineStipple = tkDashStyle.dsDash;

                int h = banDoMapWinGIS.AddLayer(sf, true);
                if (h < 0) return;
                banDoMapWinGIS.set_LayerName(h, DinhViKeyVongSaiSo);
                banDoMapWinGIS.set_LayerKey(h, DinhViKeyVongSaiSo);
                banDoMapWinGIS.set_LayerSkipOnSaving(h, true);
                dinhViLopVongSaiSo = h;
                GhiNhatKyBanDo("DINH_VI_VONG_SAI_SO | so_vong=" + vong.Count);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("DINH_VI_VONG_SAI_SO_FAILED | " + ex);
            }
        }

        private void DinhVi_ThemVong(List<Tuple<double, double, double>> ds,
            ThongTinBanDoHopNhat bd, double? kinhDo, double? viDo)
        {
            if (bd == null || !bd.LaViTriThuc || !bd.DoChinhXacM.HasValue ||
                !kinhDo.HasValue || !viDo.HasValue) return;
            double r = bd.DoChinhXacM.Value;
            if (r <= 0 || r > DinhViBanKinhVeToiDaM) return;   // bo diem qua kem
            double x, y;
            if (!Native_Wgs84SangToaDoBanDo(kinhDo.Value, viDo.Value, out x, out y)) return;
            ds.Add(Tuple.Create(x, y, r));
        }
    }
}
