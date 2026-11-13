using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // C# 7.3 / .NET Framework 4.8. Tái sử dụng danhSachLop và bộ chọn nền.
    public partial class FormMain
    {
        private void TaoKhungTheLop()
        {
            khungTheLop = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = nenGiaoDienLop,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Margin = Padding.Empty,
                Padding = new Padding(0, 2, 0, 0)
            };
            khungTheLop.ClientSizeChanged += (s, e) => CanChinhChieuRongTheLop();
            bangDanhSachLop.Controls.Add(khungTheLop, 0, 5);
        }

        private void TaoChanDanhSachLop()
        {
            Panel chan = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = nenGiaoDienLop
            };
            lblTrangThaiDanhSachLop = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = mauChuPhuLop,
                Font = lp26FontMoTa,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            lblTongSoLopGiaoDien = new Label
            {
                Dock = DockStyle.Right,
                Width = 65,
                ForeColor = mauXanhNgocLop,
                Font = lp26FontTen,
                TextAlign = ContentAlignment.MiddleRight
            };
            meoGiaoDienLop.SetToolTip(lblTrangThaiDanhSachLop,
                "Danh sách xếp từ lớp trên xuống lớp dưới. Dùng ▲ / ▼ để đổi thứ tự.");
            chan.Controls.Add(lblTrangThaiDanhSachLop);
            chan.Controls.Add(lblTongSoLopGiaoDien);
            bangDanhSachLop.Controls.Add(chan, 0, 6);
        }

        private void LamMoiDanhSachLop()
        {
            if (!daSapXepLop || dangLamMoiGiaoDienLop || IsDisposed || Disposing ||
                khungTheLop == null || ketNoiMapWinGIS == null || danhSachLop == null ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            dangLamMoiGiaoDienLop = true;
            bool trangThaiCu = dangCapNhatLop;
            dangCapNhatLop = true;
            try
            {
                DongBoDanhSachLopGoc();
                if (lp26HandleThongTin >= 0 && !ketNoiMapWinGIS.CoLop(lp26HandleThongTin))
                    lp26HandleThongTin = -1;
                VeLaiCacTheLop();
                CapNhatThongKeDanhSachLop();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("MILITARY_LAYER_REFRESH_FAILED | " + ex);
                statusGeneral.Text = "● Không đồng bộ được danh sách lớp: " + ex.Message;
            }
            finally
            {
                dangCapNhatLop = trangThaiCu;
                dangLamMoiGiaoDienLop = false;
            }
        }

        private void DongBoDanhSachLopGoc()
        {
            Dictionary<int, LopMapItem> mucTheoHandle = new Dictionary<int, LopMapItem>();
            foreach (LopMapItem muc in cacLop)
                if (muc != null) mucTheoHandle[muc.Handle] = muc;
            foreach (object doiTuong in danhSachLop.Items)
            {
                LopMapItem muc = doiTuong as LopMapItem;
                if (muc != null && !mucTheoHandle.ContainsKey(muc.Handle))
                    mucTheoHandle.Add(muc.Handle, muc);
            }
            LopMapItem daChon = danhSachLop.SelectedItem as LopMapItem;
            List<LopMapItem> thuTu = new List<LopMapItem>();
            // Chỉ đọc lớp thật, từ trên xuống theo thứ tự vẽ của AxMap.
            // Tile nền dùng bộ chọn nền riêng, không tạo lớp giả trong danh sách.
            for (int viTri = banDoMapWinGIS.NumLayers - 1; viTri >= 0; viTri--)
            {
                int handle = banDoMapWinGIS.get_LayerHandle(viTri);
                if (handle < 0) continue;
                LopMapItem muc;
                if (!mucTheoHandle.TryGetValue(handle, out muc))
                {
                    string ten = banDoMapWinGIS.get_LayerName(handle);
                    if (string.IsNullOrWhiteSpace(ten))
                        ten = Path.GetFileNameWithoutExtension(banDoMapWinGIS.get_LayerFilename(handle));
                    muc = new LopMapItem { Handle = handle, Ten = string.IsNullOrWhiteSpace(ten) ? "Lớp dữ liệu" : ten };
                    cacLop.Add(muc);
                }
                thuTu.Add(muc);
            }
            danhSachLop.BeginUpdate();
            try
            {
                danhSachLop.Items.Clear();
                foreach (LopMapItem muc in thuTu)
                {
                    int index = danhSachLop.Items.Add(muc, ketNoiMapWinGIS.LayHienThiLop(muc.Handle));
                    if (daChon != null && daChon.Handle == muc.Handle) danhSachLop.SelectedIndex = index;
                }
            }
            finally { danhSachLop.EndUpdate(); }
        }

        private void VeLaiCacTheLop()
        {
            if (khungTheLop == null || khungTheLop.IsDisposed) return;
            System.Drawing.Point cuonCu = khungTheLop.AutoScrollPosition;
            string tuKhoa = Lp26_ChuanHoaTen(lp26TimLop == null ? "" : lp26TimLop.Text);
            lp26SoLopTrongBoLoc = 0;
            khungTheLop.SuspendLayout();
            try
            {
                while (khungTheLop.Controls.Count > 0)
                {
                    Control cu = khungTheLop.Controls[0];
                    Lp26_BoChuThich(cu);
                    khungTheLop.Controls.Remove(cu);
                    cu.Dispose();
                }
                foreach (object doiTuong in danhSachLop.Items)
                {
                    LopMapItem muc = doiTuong as LopMapItem;
                    if (muc == null) continue;
                    int index = danhSachLop.Items.IndexOf(muc);
                    bool hienThi = danhSachLop.GetItemChecked(index);
                    if (lp26ChiLopDangBat != null && lp26ChiLopDangBat.Checked && !hienThi) continue;
                    string tenTim = Lp26_ChuanHoaTen((muc.Ten ?? "") + " " +
                        LayTenHienThiLop(muc.Ten) +
                        (muc.Handle == handleAnhNenTuDong ? " Ảnh vệ tinh ngoại tuyến" : ""));
                    if (tuKhoa.Length > 0 && tenTim.IndexOf(tuKhoa, StringComparison.Ordinal) < 0) continue;
                    khungTheLop.Controls.Add(TaoTheLop(muc, hienThi));
                    lp26SoLopTrongBoLoc++;
                }
                if (lp26SoLopTrongBoLoc == 0)
                    khungTheLop.Controls.Add(new Label
                    {
                        Width = 240,
                        Height = 65,
                        ForeColor = mauChuPhuLop,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = lp26FontMoTa,
                        Text = danhSachLop.Items.Count == 0 ? "Chưa có lớp dữ liệu.\nDùng nút nạp lớp để thêm dữ liệu."
                            : "Không có lớp phù hợp.\nThử xóa từ khóa hoặc bỏ bộ lọc lớp đang bật."
                    });
            }
            finally { khungTheLop.ResumeLayout(true); }
            CanChinhChieuRongTheLop();
            khungTheLop.AutoScrollPosition = new System.Drawing.Point(Math.Abs(cuonCu.X), Math.Abs(cuonCu.Y));
            int handleCanCuon = lp26HandleCanCuonDen;
            lp26HandleCanCuonDen = -1;
            if (handleCanCuon >= 0)
            {
                foreach (Control the in khungTheLop.Controls)
                {
                    if (!(the.Tag is int) || (int)the.Tag != handleCanCuon) continue;
                    khungTheLop.ScrollControlIntoView(the);
                    break;
                }
            }
        }

        private static string Lp26_ChuanHoaTen(string ten)
        {
            string tach = (ten ?? "").Trim().ToLowerInvariant().Replace('đ', 'd').Normalize(NormalizationForm.FormD);
            StringBuilder ketQua = new StringBuilder();
            bool vuaGapKhoangTrang = false;
            foreach (char c in tach)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark) continue;
                if (char.IsWhiteSpace(c))
                {
                    if (!vuaGapKhoangTrang) ketQua.Append(' ');
                    vuaGapKhoangTrang = true;
                }
                else { ketQua.Append(c); vuaGapKhoangTrang = false; }
            }
            return ketQua.ToString().Normalize(NormalizationForm.FormC);
        }

        private void Lp26_BoChuThich(Control control)
        {
            meoGiaoDienLop.SetToolTip(control, null);
            foreach (Control con in control.Controls) Lp26_BoChuThich(con);
        }

        private Panel TaoTheLop(LopMapItem muc, bool hienThi)
        {
            bool laPostGIS = muc.Handle == handleLopDiemPostGIS || LaLopPostGIS(muc.Ten);
            object duLieu = banDoMapWinGIS.get_GetObject(muc.Handle);
            bool laRaster = duLieu is MapWinGIS.Image || duLieu is MapWinGIS.Grid;
            bool laAnhNen = muc.Handle == handleAnhNenTuDong;
            int viTri = ketNoiMapWinGIS.LayViTriLop(muc.Handle);
            int thuTu = banDoMapWinGIS.NumLayers - viTri;
            bool dangXemThongTin = muc.Handle == lp26HandleThongTin;
            Color mau = laPostGIS ? mauXanhNgocLop :
                laRaster ? Color.FromArgb(228, 145, 55) : lp26MauXanhDuong;
            Color nenThe = laPostGIS ? Color.FromArgb(223, 251, 240) :
                laRaster ? Color.FromArgb(255, 244, 225) : Color.FromArgb(229, 244, 255);
            Color vienThe = laPostGIS ? Color.FromArgb(138, 225, 188) :
                laRaster ? Color.FromArgb(245, 198, 129) : Color.FromArgb(147, 201, 242);
            Panel the = new Panel
            {
                Width = 260,
                Height = dangXemThongTin ? 344 : 90,
                Margin = new Padding(0, 0, 0, 8),
                BackColor = nenThe,
                Tag = muc.Handle
            };
            Lp26_BoGoc(the, 11);
            the.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath duong = Lp26_DuongBoGoc(
                    new Rectangle(0, 0, the.Width - 1, the.Height - 1), 11))
                using (Pen pen = new Pen(vienThe, 1F))
                    e.Graphics.DrawPath(pen, duong);
            };
            Panel vach = new Panel
            {
                Left = 0,
                Top = 7,
                Width = 4,
                Height = 76,
                BackColor = mau
            };
            Lp26_BoGoc(vach, 2);
            Label kyHieu = new Label
            {
                Left = 10,
                Top = 15,
                Width = 25,
                Height = 27,
                Text = laPostGIS ? "◆" : laRaster ? "▧" : "▱",
                ForeColor = mau,
                Font = lp26FontKyHieu,
                TextAlign = ContentAlignment.MiddleCenter
            };
            Label ten = new Label
            {
                Left = 40,
                Top = 8,
                Width = 170,
                Height = 20,
                Text = thuTu + ". " + (laAnhNen ? "Ảnh vệ tinh ngoại tuyến" : LayTenHienThiLop(muc.Ten)),
                ForeColor = lp26MauChuChinh,
                Font = lp26FontTen,
                AutoEllipsis = true
            };
            Label moTa = new Label
            {
                Left = 40,
                Top = 31,
                Width = 170,
                Height = 19,
                Text = (hienThi ? "Đang bật · " : "Đang ẩn · ") +
                    (laRaster ? "Ảnh bản đồ" : LayMoTaHienThiLop(muc.Ten)),
                ForeColor = mauChuPhuLop,
                Font = lp26FontMoTa,
                AutoEllipsis = true
            };
            Button batTat = Lp26_TaoNut(hienThi ? "✓" : "");
            batTat.Size = new Size(27, 27);
            batTat.Top = 15;
            Lp26_DatMauNut(batTat,
                hienThi ? Color.FromArgb(9, 171, 132) : Color.FromArgb(208, 223, 235),
                hienThi ? Color.White : lp26MauChuChinh,
                hienThi ? Color.FromArgb(14, 193, 149) : Color.FromArgb(185, 210, 228));
            batTat.Click += (s, e) => DoiTrangThaiLopTuThe(muc.Handle);
            Button phong = Lp26_TaoNut("PHÓNG ĐẾN");
            phong.Left = 40;
            phong.Top = 55;
            phong.Height = 23;
            Lp26_DatMauNut(phong, Color.FromArgb(207, 236, 255),
                lp26MauXanhDuong, Color.FromArgb(182, 223, 250));
            phong.Click += (s, e) => Lp26_PhongDenLop(muc.Handle);
            Button len = Lp26_TaoNut("▲");
            len.AccessibleName = "Đưa lớp lên một bậc";
            len.Size = new Size(27, 23);
            len.Top = 55;
            Lp26_DatMauNut(len, Color.FromArgb(222, 248, 236),
                mauXanhNgocLop, Color.FromArgb(199, 236, 218));
            len.Enabled = Lp26_CoTheDoiThuTu(muc.Handle, viTri, true);
            len.Click += (s, e) => Lp26_DoiThuTuLop(muc.Handle, true);
            Button xuong = Lp26_TaoNut("▼");
            xuong.AccessibleName = "Đưa lớp xuống một bậc";
            xuong.Size = new Size(27, 23);
            xuong.Top = 55;
            Lp26_DatMauNut(xuong, Color.FromArgb(222, 248, 236),
                mauXanhNgocLop, Color.FromArgb(199, 236, 218));
            xuong.Enabled = Lp26_CoTheDoiThuTu(muc.Handle, viTri, false);
            xuong.Click += (s, e) => Lp26_DoiThuTuLop(muc.Handle, false);
            Button thongTin = Lp26_TaoNut(dangXemThongTin ? "−" : "i");
            thongTin.AccessibleName = dangXemThongTin ? "Thu gọn thông tin lớp" : "Xem thông tin lớp";
            thongTin.Size = new Size(27, 23);
            thongTin.Top = 55;
            Lp26_DatMauNut(thongTin,
                dangXemThongTin ? Color.FromArgb(215, 197, 250) : Color.FromArgb(233, 222, 255),
                lp26MauTim, Color.FromArgb(215, 197, 250));
            thongTin.Click += (s, e) => Lp26_DoiKhungThongTinLop(muc.Handle);
            the.Controls.AddRange(new Control[] { vach, kyHieu, ten, moTa, batTat, phong, thongTin, len, xuong });
            TextBox chiTiet = null;
            Button saoChep = null;
            if (dangXemThongTin)
            {
                chiTiet = new TextBox
                {
                    Left = 10,
                    Top = 86,
                    Height = 210,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    WordWrap = true,
                    ShortcutsEnabled = true,
                    BackColor = Color.White,
                    ForeColor = lp26MauChuChinh,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = lp26FontMoTa,
                    AccessibleName = "Thông tin lớp bản đồ",
                    Text = Lp26_LayThongTinLop(muc.Handle)
                };
                saoChep = Lp26_TaoNut("SAO CHÉP THÔNG TIN");
                saoChep.Left = 10;
                saoChep.Top = 303;
                saoChep.Height = 26;
                Lp26_DatMauNut(saoChep, Color.FromArgb(35, 146, 200),
                    Color.White, Color.FromArgb(18, 164, 215));
                saoChep.Click += (s, e) => Lp26_SaoChepThongTinLop(chiTiet.Text);
                the.Controls.Add(chiTiet);
                the.Controls.Add(saoChep);
            }
            Action canChinh = () =>
            {
                batTat.Left = Math.Max(0, the.ClientSize.Width - 35);
                ten.Width = moTa.Width = Math.Max(1, batTat.Left - 47);
                xuong.Left = Math.Max(0, the.ClientSize.Width - 35);
                len.Left = Math.Max(0, xuong.Left - 31);
                thongTin.Left = Math.Max(0, len.Left - 31);
                phong.Width = Math.Max(1, thongTin.Left - phong.Left - 4);
                vach.Height = Math.Max(1, the.ClientSize.Height - 14);
                if (chiTiet != null) chiTiet.Width = Math.Max(1, the.ClientSize.Width - 20);
                if (saoChep != null) saoChep.Width = Math.Max(1, the.ClientSize.Width - 20);
            };
            the.Resize += (s, e) => canChinh();
            canChinh();
            meoGiaoDienLop.SetToolTip(ten, muc.Ten + "\nThứ tự " + thuTu + " từ trên xuống.");
            meoGiaoDienLop.SetToolTip(moTa, laAnhNen
                ? "Ảnh nền ngoại tuyến được giữ dưới các lớp dữ liệu. Dùng bộ chọn nền phía trên để đổi nguồn nền."
                : muc.Ten);
            meoGiaoDienLop.SetToolTip(len, "Đưa lên một bậc theo thứ tự toàn bộ lớp, kể cả lớp đang bị lọc.");
            meoGiaoDienLop.SetToolTip(xuong, "Đưa xuống một bậc theo thứ tự toàn bộ lớp, kể cả lớp đang bị lọc.");
            meoGiaoDienLop.SetToolTip(thongTin, dangXemThongTin
                ? "Thu gọn thông tin lớp." : "Xem nguồn dữ liệu, hệ tọa độ, số đối tượng hoặc kích thước ảnh và phạm vi lớp.");
            meoGiaoDienLop.SetToolTip(batTat, hienThi ? "Ẩn lớp" : "Bật lớp");
            meoGiaoDienLop.SetToolTip(phong, "Đến phạm vi dữ liệu; giữ nguyên trạng thái bật/tắt của lớp.");
            return the;
        }

        private void Lp26_DoiKhungThongTinLop(int handle)
        {
            if (!Lp26_SanSangLuuThuTu()) return;
            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                {
                    statusGeneral.Text = "● Lớp không còn trên bản đồ.";
                    HenLamMoiDanhSachLop();
                    return;
                }
                lp26HandleThongTin = lp26HandleThongTin == handle ? -1 : handle;
                if (lp26HandleThongTin >= 0) lp26HandleCanCuonDen = handle;
                HenLamMoiDanhSachLop();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LAYER_INFO_OPEN_FAILED | " + ex);
                statusGeneral.Text = "● Không mở được thông tin lớp: " + ex.Message;
            }
        }

        private static string Lp26_MoTaHeToaDo(MapWinGIS.GeoProjection heToaDo)
        {
            if (heToaDo == null) return "Chưa đọc được";
            if (heToaDo.IsEmpty) return "Chưa khai báo";
            int ma;
            string ten = heToaDo.Name ?? "";
            string maHienThi = heToaDo.TryAutoDetectEpsg(out ma) && ma > 0
                ? "EPSG:" + ma : "Chưa nhận diện được mã EPSG";
            return string.IsNullOrWhiteSpace(ten) ? maHienThi : maHienThi + " · " + ten;
        }

        private static string Lp26_SoThongTin(double giaTri, string dinhDang)
        {
            if (double.IsNaN(giaTri) || double.IsInfinity(giaTri)) return "Chưa xác định";
            return giaTri.ToString(dinhDang, CultureInfo.GetCultureInfo("vi-VN"));
        }

        private static string Lp26_TenHinhHoc(MapWinGIS.ShpfileType kieu)
        {
            string ten = kieu.ToString();
            string coBan = ten.Contains("MULTIPOINT") ? "Đa điểm" :
                ten.Contains("POLYLINE") ? "Đường" :
                ten.Contains("POLYGON") ? "Vùng" :
                ten.Contains("POINT") ? "Điểm" :
                ten.Contains("MULTIPATCH") ? "Bề mặt ba chiều" : "Chưa có kiểu hình học";
            if (ten.EndsWith("Z", StringComparison.Ordinal)) coBan += " · Có độ cao";
            else if (ten.EndsWith("M", StringComparison.Ordinal)) coBan += " · Có giá trị đo M";
            return coBan;
        }

        private string Lp26_LayThongTinLop(int handle)
        {
            StringBuilder noiDung = new StringBuilder();
            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle)) return "Lớp không còn trên bản đồ.";
                string ten = banDoMapWinGIS.get_LayerName(handle) ?? "";
                object duLieu = banDoMapWinGIS.get_GetObject(handle);
                MapWinGIS.Shapefile doiTuong = duLieu as MapWinGIS.Shapefile;
                MapWinGIS.Image anh = duLieu as MapWinGIS.Image;
                MapWinGIS.Grid luoi = duLieu as MapWinGIS.Grid;
                MapWinGIS.GeoProjection heLop = null;
                MapWinGIS.Extents phamVi = null;
                CultureInfo vi = CultureInfo.GetCultureInfo("vi-VN");
                noiDung.AppendLine("Lớp: " + LayTenHienThiLop(ten));
                noiDung.AppendLine("Thứ tự từ trên xuống: " +
                    (banDoMapWinGIS.NumLayers - ketNoiMapWinGIS.LayViTriLop(handle)));
                noiDung.AppendLine("Trạng thái: " + (ketNoiMapWinGIS.LayHienThiLop(handle) ? "Đang bật" : "Đang ẩn"));
                bool laNguonNgoai = false;
                if (doiTuong != null)
                {
                    laNguonNgoai = banDoMapWinGIS.get_OgrLayer(handle) != null;
                    noiDung.AppendLine("Loại dữ liệu: Lớp đối tượng · " + Lp26_TenHinhHoc(doiTuong.ShapefileType));
                    noiDung.AppendLine((laNguonNgoai ? "Số đối tượng đang nạp: " : "Số đối tượng: ") +
                        doiTuong.NumShapes.ToString("N0", vi));
                    noiDung.AppendLine("Số trường thuộc tính: " + doiTuong.NumFields.ToString("N0", vi));
                    heLop = doiTuong.GeoProjection;
                    if (doiTuong.NumShapes > 0) phamVi = doiTuong.Extents;
                }
                else if (anh != null)
                {
                    noiDung.AppendLine("Loại dữ liệu: Ảnh bản đồ");
                    noiDung.AppendLine("Kích thước ảnh gốc: " + anh.OriginalWidth.ToString("N0", vi) +
                        " × " + anh.OriginalHeight.ToString("N0", vi) + " pixel");
                    noiDung.AppendLine("Kích thước pixel theo hệ tọa độ ảnh: " +
                        Lp26_SoThongTin(Math.Abs(anh.OriginalDX), "G10") + " × " +
                        Lp26_SoThongTin(Math.Abs(anh.OriginalDY), "G10"));
                    int soMuc = anh.NumOverviews;
                    noiDung.AppendLine("Số mức ảnh tổng quan: " + (soMuc >= 0 ? soMuc.ToString("N0", vi) : "Chưa xác định"));
                    heLop = anh.GeoProjection;
                    phamVi = anh.Extents;
                }
                else if (luoi != null)
                {
                    noiDung.AppendLine("Loại dữ liệu: Lưới dữ liệu");
                    var thongSo = luoi.Header;
                    if (thongSo != null)
                    {
                        noiDung.AppendLine("Số cột × số hàng: " + thongSo.NumberCols.ToString("N0", vi) +
                            " × " + thongSo.NumberRows.ToString("N0", vi));
                        noiDung.AppendLine("Kích thước ô theo hệ tọa độ lưới: " +
                            Lp26_SoThongTin(Math.Abs(thongSo.dX), "G10") + " × " +
                            Lp26_SoThongTin(Math.Abs(thongSo.dY), "G10"));
                        heLop = thongSo.GeoProjection;
                    }
                    phamVi = luoi.Extents;
                }
                else noiDung.AppendLine("Loại dữ liệu: Chưa đọc được thông tin chi tiết");
                string tep = banDoMapWinGIS.get_LayerFilename(handle);
                string nguon = !string.IsNullOrWhiteSpace(tep) && Path.IsPathRooted(tep) ? tep :
                    (handle == handleLopDiemPostGIS || LaLopPostGIS(ten))
                        ? "PostGIS · Dữ liệu đơn vị" : laNguonNgoai
                            ? "Nguồn dữ liệu ngoài đang nạp" : "Lớp dữ liệu đang nạp trong bộ nhớ";
                noiDung.AppendLine("Nguồn: " + nguon);
                noiDung.AppendLine();
                noiDung.AppendLine("Hệ tọa độ lớp đang nạp: " + Lp26_MoTaHeToaDo(heLop));
                noiDung.AppendLine("Hệ tọa độ bản đồ: " + Lp26_MoTaHeToaDo(banDoMapWinGIS.GeoProjection));
                noiDung.AppendLine();
                if (phamVi == null) noiDung.AppendLine("Phạm vi: Chưa có dữ liệu để xác định.");
                else
                {
                    string dinhDang = heLop == null || heLop.IsEmpty ? "G12" : heLop.IsGeographic ? "N6" : "N3";
                    noiDung.AppendLine("PHẠM VI THEO HỆ TỌA ĐỘ LỚP");
                    noiDung.AppendLine("X nhỏ nhất: " + Lp26_SoThongTin(phamVi.xMin, dinhDang));
                    noiDung.AppendLine("Y nhỏ nhất: " + Lp26_SoThongTin(phamVi.yMin, dinhDang));
                    noiDung.AppendLine("X lớn nhất: " + Lp26_SoThongTin(phamVi.xMax, dinhDang));
                    noiDung.AppendLine("Y lớn nhất: " + Lp26_SoThongTin(phamVi.yMax, dinhDang));
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LAYER_INFO_READ_FAILED | handle=" + handle + " | " + ex);
                noiDung.AppendLine("Chưa đọc đủ thông tin: " + ex.Message);
            }
            return noiDung.ToString().TrimEnd();
        }

        private void Lp26_SaoChepThongTinLop(string noiDung)
        {
            if (string.IsNullOrWhiteSpace(noiDung)) return;
            try
            {
                Clipboard.SetText(noiDung);
                statusGeneral.Text = "● Đã sao chép thông tin lớp bản đồ.";
            }
            catch (Exception ex)
            {
                statusGeneral.Text = "● Chưa sao chép được thông tin lớp: " + ex.Message;
            }
        }
    }
}