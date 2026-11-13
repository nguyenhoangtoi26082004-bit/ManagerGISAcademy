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
        private void Native_GanKiemTraDuLieu(UcKiemTraDuLieuNative giaoDien)
        {
            if (giaoDien == null) return;
            giaoDien.YeuCauXemDonVi += ma => MoXemChiTiet(ma);
            giaoDien.YeuCauDinhVi += ma => DinhViDonVi(ma);
            giaoDien.YeuCauChayKiemTra += async () =>
            {
                if (giaoDien.IsDisposed || IsDisposed || Disposing) return;
                if (!CoQuyen(ThaoTacDonViQuanDoi.Xem))
                {
                    MessageBox.Show(this, "Chưa có quyền kiểm tra hồ sơ.",
                        "Phân quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                NghiepVuDonVi bll = nghiepVuDonVi;
                if (bll == null || !bll.DaCauHinh) return;
                statusGeneral.Text = "● Đang kiểm tra chất lượng dữ liệu...";
                try
                {
                    string tomTat = null;
                    Tuple<int, List<CanhBaoKiemTraNative>> result =
                        await Task.Run(() =>
                        {
                            List<DonViQuanDoi> hoSo = bll.LayTatCaDonVi();
                            if (hoSo == null)
                                throw new InvalidOperationException("BLL trả về danh sách null.");
                            List<CanhBaoKiemTraNative> canhBao =
                                new List<CanhBaoKiemTraNative>();
                            foreach (DonViQuanDoi dv in hoSo)
                            {
                                if (dv == null || string.IsNullOrWhiteSpace(dv.MaDonVi))
                                    continue;
                                KetQuaKiemTraDonVi kiemTra = bll.KiemTraDuLieu(dv, true);
                                if (kiemTra.DanhSachLoi != null)
                                {
                                    foreach (string noiDung in kiemTra.DanhSachLoi)
                                        canhBao.Add(TaoCanhBaoDuLieu(dv, noiDung));
                                }
                                if (NghiepVuDonVi.LaThieuSoTrucBan(dv))
                                {
                                    canhBao.Add(TaoCanhBaoDuLieu(
                                        dv, "Chưa có số điện thoại trực ban.",
                                        "Thông tin liên lạc", "Cảnh báo"));
                                }
                                if (string.IsNullOrWhiteSpace(dv.Wkt))
                                {
                                    canhBao.Add(TaoCanhBaoDuLieu(
                                        dv, "Thiếu hình học geom_4326 trong PostGIS.",
                                        "Hình học", "Nghiêm trọng"));
                                }
                                else if (!dv.Wkt.TrimStart().StartsWith("POINT",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    canhBao.Add(TaoCanhBaoDuLieu(
                                        dv, "Hình học không phải POINT.",
                                        "Kiểu hình học", "Cảnh báo"));
                                }
                            }
                            // Kiem tra KHONG GIAN o tang CSDL (v_kiem_tra_vi_tri):
                            // diem dai dien, trung vi tri, do chinh xac, ngoai xa.
                            try
                            {
                                TruyXuatViTri dalViTri = new TruyXuatViTri();
                                foreach (LoiViTri loi in dalViTri.LayLoiViTri())
                                    canhBao.Add(TaoCanhBaoViTri(loi));
                                TongHopViTri th = dalViTri.LayTongHopViTri();
                                if (th.Tong > 0)
                                {
                                    int thuc = th.ViTriTot + th.CanDoLai;
                                    tomTat =
                                        "Chất lượng vị trí: " + thuc + "/" + th.Tong +
                                        " vị trí thật (" + (100.0 * thuc / th.Tong).ToString("0.#", TienIchGiaoDienSang.Vi) +
                                        "%) · đạt ≤ 10 m: " + th.ViTriTot +
                                        " · cần đo lại: " + th.CanDoLai +
                                        " · điểm đại diện: " + th.DiemDaiDien +
                                        " · đã kiểm chứng: " + th.DaKiemChung;
                                }
                            }
                            catch (Exception exViTri)
                            {
                                GhiNhatKyBanDo("QUALITY_SPATIAL_FAILED | " + exViTri.Message);
                                tomTat = "Không đọc được kiểm tra không gian: " + exViTri.Message;
                            }
                            return Tuple.Create(hoSo.Count, canhBao);
                        });
                    if (giaoDien.IsDisposed || IsDisposed || Disposing) return;
                    giaoDien.NapKetQua(result.Item2, result.Item1);
                    giaoDien.DatTomTat(tomTat);
                    statusGeneral.Text = "● Đã kiểm tra " + result.Item1 +
                        " hồ sơ, " + result.Item2.Count + " cảnh báo (thuộc tính + không gian).";
                }
                catch (Exception ex)
                {
                    if (!IsDisposed && !Disposing)
                        statusGeneral.Text = "● Lỗi kiểm tra: " + ex.Message;
                    GhiNhatKyBanDo("NATIVE_QUALITY_FAILED | " + ex);
                }
            };
        }

        /// <summary>Chuyen mot loi vi tri cua CSDL sang dong canh bao tieng Viet.</summary>
        private static CanhBaoKiemTraNative TaoCanhBaoViTri(LoiViTri loi)
        {
            string loai, khuyenNghi;
            switch (loi.LoaiLoi)
            {
                case "DIEM_DAI_DIEN":
                    loai = "Vị trí: điểm đại diện";
                    khuyenNghi = "Mở thẻ đơn vị trên bản đồ → \"Định vị lại trụ sở\", lấy điểm tại cổng chính.";
                    break;
                case "TRUNG_VI_TRI":
                    loai = "Vị trí: trùng tọa độ";
                    khuyenNghi = "Định vị lại từng đơn vị để tách các điểm đang chồng nhau.";
                    break;
                case "DO_CHINH_XAC_THAP":
                    loai = "Vị trí: độ chính xác thấp";
                    khuyenNghi = "Số hóa lại trên ảnh vệ tinh (~3 m) hoặc đo GNSS.";
                    break;
                case "NGOAI_XA_KHAI_BAO":
                    loai = "Vị trí: ngoài xã khai báo";
                    khuyenNghi = "Kiểm tra lại tọa độ hoặc phường/xã của hồ sơ.";
                    break;
                case "THIEU_VI_TRI":
                    loai = "Vị trí: thiếu";
                    khuyenNghi = "Bổ sung vị trí trụ sở.";
                    break;
                default:
                    loai = "Vị trí";
                    khuyenNghi = "Rà soát vị trí.";
                    break;
            }

            return new CanhBaoKiemTraNative
            {
                MaDonVi = loi.MaDonVi,
                TenDonVi = loi.TenDonVi,
                LoaiLoi = loai,
                MucDo = loi.MucDo == "NGHIEM_TRONG" ? "Nghiêm trọng" : "Cảnh báo",
                NoiDung = loi.MoTa,
                KhuyenNghi = khuyenNghi
            };
        }

        private static CanhBaoKiemTraNative TaoCanhBaoDuLieu(
            DonViQuanDoi dv,
            string loi,
            string loaiLoi = null,
            string mucDo = null)
        {
            string moTa = loi ?? "Không xác định";
            string thuong = moTa.ToLowerInvariant();

            if (loaiLoi == null)
            {
                if (thuong.Contains("tọa độ") ||
                    thuong.Contains("kinh độ") ||
                    thuong.Contains("vĩ độ") ||
                    thuong.Contains("x/y phẳng"))
                {
                    loaiLoi = "Tọa độ";
                }
                else if (thuong.Contains("trống") ||
                    thuong.Contains("chưa nhập"))
                {
                    loaiLoi = "Thiếu thuộc tính";
                }
                else if (thuong.Contains("ký tự"))
                {
                    loaiLoi = "Độ dài thuộc tính";
                }
                else
                {
                    loaiLoi = "Thuộc tính";
                }
            }

            if (mucDo == null)
            {
                mucDo =
                    thuong.Contains("không được để trống") ||
                    thuong.Contains("số hữu hạn") ||
                    thuong.Contains("yêu cầu tọa độ") ||
                    thuong.Contains("ngoài phạm vi")
                    ? "Nghiêm trọng"
                    : "Cảnh báo";
            }

            return new CanhBaoKiemTraNative
            {
                MaDonVi = dv.MaDonVi,
                TenDonVi = dv.TenDonVi,
                LoaiLoi = loaiLoi,
                MucDo = mucDo,
                NoiDung = moTa,
                KhuyenNghi =
                    "Đối chiếu hồ sơ gốc và kiểm tra lại dữ liệu; " +
                    "không tự ý sửa trên CSDL khi chưa được cấp quyền."
            };
        }
    }
}