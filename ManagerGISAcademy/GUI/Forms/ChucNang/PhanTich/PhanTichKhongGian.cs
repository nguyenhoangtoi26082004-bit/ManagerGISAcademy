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
        private void GanXuLyPhanTichGon(GiaoDienPhanTich giaoDien)
        {
            if (giaoDien == null)
                return;

            bool dangXuLy = false;

            FlowLayoutPanel thanh = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 59,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10, 5, 5, 3),
                BackColor = Color.FromArgb(23, 39, 58)
            };

            ComboBox chonNguon = new ComboBox
            {
                Name = "cboNguonPhanTich",
                Width = 370,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "TenDonVi",
                ValueMember = "MaDonVi",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 48, 68),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                Margin = new Padding(3, 6, 6, 3)
            };

            NumericUpDown soK = new NumericUpDown
            {
                Minimum = 1,
                Maximum = SpatialAnalysisBLL.SoLuongGanNhatToiDa,
                Value = 10,
                Width = 60,
                TextAlign = HorizontalAlignment.Center,
                BackColor = Color.FromArgb(30, 48, 68),
                ForeColor = Color.White,
                Margin = new Padding(3, 6, 5, 3)
            };
            Func<string, int, Button> taoNut = (ten, rong) =>
            {
                Button nut = new Button
                {
                    Text = ten,
                    Width = rong,
                    Height = 30,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(16, 185, 129),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                    Margin = new Padding(3, 3, 5, 3)
                };

                nut.FlatAppearance.BorderSize = 0;

                return nut;
            };

            Button timK = taoNut("TÌM K GẦN NHẤT", 140);
            Button lamMoi = taoNut("LÀM MỚI NGUỒN", 123);

            thanh.Controls.Add(chonNguon);
            thanh.Controls.Add(soK);
            thanh.Controls.Add(timK);
            thanh.Controls.Add(lamMoi);

            giaoDien.Controls.Add(thanh);
            thanh.BringToFront();

            giaoDien.ChoPhepXuatCsv =
                CoQuyen(ThaoTacDonViQuanDoi.Xem);

            NumericUpDown banKinh = giaoDien.Controls
                .Find("nudBanKinh", true)
                .OfType<NumericUpDown>()
                .FirstOrDefault();

            if (banKinh != null)
            {
                banKinh.Maximum =
                    (decimal)SpatialAnalysisBLL.BanKinhToiDaMet;

                if (banKinh.Value > banKinh.Maximum)
                    banKinh.Value = banKinh.Maximum;
            }

            NumericUpDown gioiHan = giaoDien.Controls
                .Find("nudGioiHan", true)
                .OfType<NumericUpDown>()
                .FirstOrDefault();

            if (gioiHan != null)
            {
                gioiHan.Maximum =
                    SpatialAnalysisBLL.GioiHanTrongBanKinhToiDa;

                if (gioiHan.Value > gioiHan.Maximum)
                    gioiHan.Value = gioiHan.Maximum;
            }

            Action capNhatNguon = () =>
            {
                DonViQuanDoi donVi =
                    chonNguon.SelectedItem as DonViQuanDoi;

                giaoDien.GanDoiTuongDangChon(
                    donVi == null ? null : donVi.MaDonVi,
                    donVi == null ? null : donVi.TenDonVi);
            };

            Action napNguon = () =>
            {
                if (dangXuLy)
                    return;

                string maCu =
                    (chonNguon.SelectedItem as DonViQuanDoi)?.MaDonVi;

                chonNguon.BeginUpdate();

                try
                {
                    chonNguon.Items.Clear();

                    if (!CoQuyen(ThaoTacDonViQuanDoi.Xem))
                    {
                        giaoDien.CapNhatThongBao(
                            "Chưa cấu hình BLL hoặc không có quyền xem dữ liệu.");

                        return;
                    }

                    List<DonViQuanDoi> ds =
                        nghiepVuDonVi.LayTatCaDonVi();

                    if (ds == null)
                    {
                        throw new InvalidOperationException(
                            "BLL trả về danh sách null.");
                    }

                    foreach (DonViQuanDoi dv in ds
                        .Where(d => d != null &&
                            !string.IsNullOrWhiteSpace(d.MaDonVi) &&
                            CoQuyen(
                                ThaoTacDonViQuanDoi.Xem,
                                d.MaDonVi))
                        .OrderBy(d => d.TenDonVi))
                    {
                        chonNguon.Items.Add(dv);
                    }

                    int viTri = -1;

                    for (int i = 0; i < chonNguon.Items.Count; i++)
                    {
                        DonViQuanDoi dv =
                            chonNguon.Items[i] as DonViQuanDoi;

                        if (dv != null &&
                            string.Equals(
                                dv.MaDonVi,
                                maCu,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            viTri = i;
                            break;
                        }
                    }

                    chonNguon.SelectedIndex = viTri >= 0
                        ? viTri
                        : chonNguon.Items.Count > 0 ? 0 : -1;

                    giaoDien.CapNhatThongBao(
                        "Đã nạp " + chonNguon.Items.Count +
                        " đơn vị nguồn được quyền xem.");
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "SPATIAL_SOURCE_FAILED | " + ex);

                    giaoDien.CapNhatThongBao(
                        "Lỗi tải đơn vị nguồn: " + ex.Message);
                }
                finally
                {
                    chonNguon.EndUpdate();
                    capNhatNguon();
                }
            };

            Action<bool> datDangXuLy = value =>
            {
                dangXuLy = value;

                if (giaoDien.IsDisposed)
                    return;

                giaoDien.DatTrangThaiDangXuLy(value);

                chonNguon.Enabled = !value;
                soK.Enabled = !value;
                timK.Enabled = !value;
                lamMoi.Enabled = !value;
            };

            Func<SpatialAnalysisBLL> taoBLL = () =>
            {
                if (!CoQuyen(ThaoTacDonViQuanDoi.Xem) ||
                    chinhSachQuyen == null)
                {
                    throw new UnauthorizedAccessException(
                        "Không có quyền xem dữ liệu để phân tích.");
                }

                return new SpatialAnalysisBLL(
                    new TruyXuatKhongGian(),
                    chinhSachQuyen);
            };

            chonNguon.SelectedIndexChanged +=
                (s, e) => capNhatNguon();

            lamMoi.Click +=
                (s, e) => napNguon();

            giaoDien.YeuCauThucHienPhanTich += async (s, e) =>
            {
            if (dangXuLy)
                return;

            if (e.LoaiPhanTich ==
                LoaiPhanTichKhongGian.DoKhoangCach)
            {
                try
                {
                    ChonPhanHe(btnNavMap, "Bản đồ GIS");

                    if (banDoMapWinGIS == null ||
                        banDoMapWinGIS.IsDisposed)
                    {
                        throw new InvalidOperationException(
                            "MapWinGIS chưa sẵn sàng.");
                    }

                    banDoMapWinGIS.Measuring.Clear();

                    banDoMapWinGIS.Measuring.MeasuringType =
                        MapWinGIS.tkMeasuringType.MeasureDistance;

                    banDoMapWinGIS.CursorMode =
                        MapWinGIS.tkCursorMode.cmMeasure;

                    statusGeneral.Text =
                        "● Nhấp các đỉnh trên bản đồ để đo.";
                }
                catch (Exception ex)
                {
                    if (!giaoDien.IsDisposed)
                        giaoDien.CapNhatThongBao(ex.Message);
                }

                return;
            }

            try
            {
                if (!CoQuyen(
                    ThaoTacDonViQuanDoi.Xem,
                    e.MaDoiTuongNguon))
                {
                    throw new UnauthorizedAccessException(
                        "Không được phép phân tích đơn vị trung tâm.");
                }

                SpatialAnalysisBLL bll = taoBLL();

                string ma = e.MaDoiTuongNguon;
                double met = (double)e.BanKinhMet;
                int gioiHanKetQua = e.GioiHanKetQua;

                datDangXuLy(true);

                List<DonViTheoKhoangCach> ds;
                    string moTa;
                    string wktVungDem = null;

                    if (e.LoaiPhanTich ==
                        LoaiPhanTichKhongGian.TaoVungDem)
                    {
                        KetQuaVungDemDonVi buffer =
                            await Task.Run(() =>
                                bll.PhanTichVungDem(
                                    ma,
                                    met,
                                    gioiHanKetQua));

                        ds = buffer.DanhSachDonVi.ToList();
                        wktVungDem = buffer.WktVungDem4326;
                        moTa = "PostGIS - ST_Buffer / ST_DWithin";
                    }
                    else if (e.LoaiPhanTich ==
                        LoaiPhanTichKhongGian.TimDoiTuongGan)
                    {
                        ds = await Task.Run(() =>
                            bll.TimDonViTrongBanKinh(
                                ma,
                                met,
                                gioiHanKetQua));

                        moTa = "PostGIS - ST_DWithin";
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Chế độ phân tích không được hỗ trợ.");
                    }

                    if (IsDisposed || giaoDien.IsDisposed)
                        return;

                    List<KetQuaPhanTichItem> duLieu =
                        ChuyenKetQuaPhanTichGon(ds);

                    giaoDien.NapKetQuaTimGan(duLieu, moTa);

                    try
                    {
                        DonViQuanDoi tam =
                            chonNguon.SelectedItem as DonViQuanDoi;

                        double? dienTich = VeKetQuaPhanTich(
                            wktVungDem,
                            duLieu,
                            tam,
                            true);

                        if (e.LoaiPhanTich ==
                            LoaiPhanTichKhongGian.TaoVungDem)
                        {
                            giaoDien.CapNhatKetQuaVungDem(
                                dienTich,
                                ds.Count);
                        }

                        VeNoiBatKetQua(giaoDien.KetQuaDangChon);

                        ChonPhanHe(btnNavMap, "Bản đồ GIS");

                        statusGeneral.Text =
                            "● Đã hiển thị " + ds.Count +
                            " đơn vị thuộc kết quả phân tích.";

                        giaoDien.CapNhatThongBao(
                            "Đã tìm " + ds.Count + " đơn vị trong " +
                            met.ToString("N0") +
                            " m; xem trên bản đồ GIS.");
                    }
                    catch (Exception exVe)
                    {
                        GhiNhatKyBanDo(
                            "SPATIAL_DRAW_FAILED | " + exVe);

                        giaoDien.CapNhatThongBao(
                            "Truy vấn thành công (" + ds.Count +
                            " đơn vị), nhưng lỗi vẽ bản đồ: " +
                            exVe.Message);
                    }
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "SPATIAL_RADIUS_FAILED | " + ex);

                    if (!IsDisposed && !giaoDien.IsDisposed)
                    {
                        giaoDien.CapNhatThongBao(
                            "Phân tích thất bại: " + ex.Message);
                    }
                }
                finally
                {
                    if (!IsDisposed && !giaoDien.IsDisposed)
                        datDangXuLy(false);
                }
            };

            // Giao dien v2: nut "Tim K" cu duoc an; nut chay chung cua giao
            // dien phat su kien YeuCauTimKGanNhat -> dung chung mot xu ly.
            EventHandler xuLyTimK = async (s, e) =>
            {
                if (dangXuLy)
                    return;

                DonViQuanDoi dv =
                    chonNguon.SelectedItem as DonViQuanDoi;

                if (dv == null ||
                    !NghiepVuDonVi.CoToaDoWgs84(dv))
                {
                    giaoDien.CapNhatThongBao(
                        "Chọn đơn vị có tọa độ WGS84 hợp lệ.");
                    return;
                }

                try
                {
                    if (!CoQuyen(
                        ThaoTacDonViQuanDoi.Xem,
                        dv.MaDonVi))
                    {
                        throw new UnauthorizedAccessException(
                            "Không có quyền xem đơn vị nguồn.");
                    }

                    SpatialAnalysisBLL bll = taoBLL();

                    int k = (int)soK.Value;
                    string maNguon = dv.MaDonVi;
                    double kinhDo = dv.KinhDo;
                    double viDo = dv.ViDo;

                    int lay = Math.Min(
                        k + 1,
                        SpatialAnalysisBLL.SoLuongGanNhatToiDa);

                    datDangXuLy(true);

                    List<DonViTheoKhoangCach> danhSach =
                        await Task.Run(() =>
                            bll.TimKDonViGanNhat(
                                kinhDo,
                                viDo,
                                lay));

                    if (IsDisposed || giaoDien.IsDisposed)
                        return;

                    List<DonViTheoKhoangCach> ketQua = danhSach
                        .Where(d => !string.Equals(
                            d.MaDonVi,
                            maNguon,
                            StringComparison.OrdinalIgnoreCase))
                        .Take(k)
                        .ToList();

                    List<KetQuaPhanTichItem> duLieu =
                        ChuyenKetQuaPhanTichGon(ketQua);

                    giaoDien.NapKetQuaTimGan(
                        duLieu,
                        "PostGIS - KNN");

                    try
                    {
                        VeKetQuaPhanTich(
                            null,
                            duLieu,
                            dv,
                            true);

                        VeNoiBatKetQua(giaoDien.KetQuaDangChon);

                        ChonPhanHe(btnNavMap, "Bản đồ GIS");

                        statusGeneral.Text =
                            "● Đã vẽ " + ketQua.Count +
                            " đơn vị gần nhất trên bản đồ.";

                        giaoDien.CapNhatThongBao(
                            "Đã tìm " + ketQua.Count +
                            " đơn vị gần nhất và hiển thị trên bản đồ.");
                    }
                    catch (Exception exVe)
                    {
                        GhiNhatKyBanDo(
                            "SPATIAL_KNN_DRAW_FAILED | " + exVe);

                        giaoDien.CapNhatThongBao(
                            "Đã tìm " + ketQua.Count +
                            " đơn vị nhưng lỗi vẽ: " +
                            exVe.Message);
                    }
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "SPATIAL_KNN_FAILED | " + ex);
                    if (!IsDisposed && !giaoDien.IsDisposed)
                    {
                        giaoDien.CapNhatThongBao(
                            "Tìm gần thất bại: " + ex.Message);
                    }
                }
                finally
                {
                    if (!IsDisposed && !giaoDien.IsDisposed)
                        datDangXuLy(false);
                }
            };
            timK.Click += xuLyTimK;
            giaoDien.YeuCauTimKGanNhat += xuLyTimK;

            giaoDien.YeuCauXoaKetQua += (s, e) =>
            {
                XoaVePhanTich();
                giaoDien.XoaKetQuaHienThi();
            };

            giaoDien.YeuCauChonKetQua += (s, e) =>
                VeNoiBatKetQua(e.DoiTuong);

            giaoDien.YeuCauDinhViKetQua += (s, e) =>
            {
                if (e.DoiTuong == null ||
                    string.IsNullOrWhiteSpace(e.DoiTuong.MaDoiTuong))
                {
                    return;
                }

                DinhViDonVi(e.DoiTuong.MaDoiTuong);
                VeNoiBatKetQua(e.DoiTuong);
            };

            napNguon();
        }
    }
}