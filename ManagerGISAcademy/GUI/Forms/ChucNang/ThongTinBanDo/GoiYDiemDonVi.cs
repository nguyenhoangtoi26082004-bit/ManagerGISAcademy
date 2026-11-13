using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using AxMapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;
using MapWinGIS;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private void BanDo_ChonDiem_MouseDownEvent(
            object sender,
            _DMapEvents_MouseDownEvent e)
        {
            ttDvDangNhanChuot = false;

            if (e.button != 1 ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null)
                return;

            TtDv_DongMenuUngVien();

            if (!dangOCheDoChonDiem ||
                banDoMapWinGIS.CursorMode != tkCursorMode.cmNone)
            {
                GhiNhatKyBanDo(
                    "UNIT_CLICK_SKIPPED | select=" +
                    dangOCheDoChonDiem +
                    " | cursor=" +
                    banDoMapWinGIS.CursorMode);
                return;
            }

            GhiNhatKyBanDo(
                "UNIT_CLICK_DOWN | x=" + e.x +
                " | y=" + e.y);

            ttDvViTriNhanChuot =
                new System.Drawing.Point(e.x, e.y);

            ttDvDangNhanChuot = true;
        }

        private void TtDv_BanDo_MouseUpEvent(
            object sender,
            _DMapEvents_MouseUpEvent e)
        {
            bool daNhan = ttDvDangNhanChuot;
            ttDvDangNhanChuot = false;

            if (!daNhan ||
                e.button != 1 ||
                IsDisposed ||
                Disposing ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !dangOCheDoChonDiem ||
                banDoMapWinGIS.CursorMode != tkCursorMode.cmNone)
                return;

            double dx = e.x - ttDvViTriNhanChuot.X;
            double dy = e.y - ttDvViTriNhanChuot.Y;

            // Không chọn điểm sau một thao tác kéo.
            if (dx * dx + dy * dy > 36d)
                return;

            GhiNhatKyBanDo(
                "UNIT_CLICK_UP | x=" + e.x +
                " | y=" + e.y +
                " | layer=" + handleLopDiemPostGIS);

            try
            {
                // Kiem tra TRUOC khi don trang thai: lop diem ho so
                // dang an (mac dinh tu giai doan 3) thi lop LOGO tu
                // xu ly nhan chuot; khong xoa the thong tin no vua mo.
                if (handleLopDiemPostGIS < 0 ||
                    ketNoiMapWinGIS == null ||
                    !ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS) ||
                    !ketNoiMapWinGIS.LayHienThiLop(handleLopDiemPostGIS))
                {
                    return;
                }

                TtDv_DonHoSoDangHienThi(false);

                Shapefile lopDiem =
                    banDoMapWinGIS.get_Shapefile(
                        handleLopDiemPostGIS);

                if (lopDiem == null ||
                    lopDiem.NumShapes == 0)
                {
                    AnKhungThongTinDiem();
                    statusGeneral.Text =
                        "● Lớp PostGIS chưa có điểm.";
                    return;
                }

                List<UngVienDonViBanDo> ds =
                    TtDv_TimUngVien(
                        lopDiem, e.x, e.y, 22d);

                GhiNhatKyBanDo(
                    "UNIT_CLICK_HITS | count=" + ds.Count +
                    " | shapes=" + lopDiem.NumShapes);

                ds.RemoveAll(d =>
                    !CoQuyen(
                        ThaoTacDonViQuanDoi.Xem,
                        d.Ma));

                if (ds.Count == 0)
                {
                    AnKhungThongTinDiem();
                    statusGeneral.Text =
                        "● Không có đơn vị được phép xem trong phạm vi chọn.";
                    return;
                }

                if (ds.Count == 1)
                {
                    TtDv_YeuCauXemHoSo(
                        ds[0], e.x, e.y);
                }
                else
                {
                    TtDv_MoMenuUngVien(
                        ds, e.x, e.y);
                }
            }
            catch (Exception ex)
            {
                AnKhungThongTinDiem();

                GhiNhatKyBanDo(
                    "UNIT_CLICK_FAILED | " + ex);

                statusGeneral.Text =
                    "● Lỗi chọn đơn vị: " + ex.Message;
            }
        }

        private List<UngVienDonViBanDo> TtDv_TimUngVien(
            Shapefile lopDiem,
            double xChuot,
            double yChuot,
            double banKinh)
        {
            List<UngVienDonViBanDo> ds =
                new List<UngVienDonViBanDo>();

            int cotMa =
                lopDiem.get_FieldIndexByName("MA_DON_VI");

            int cotTen =
                lopDiem.get_FieldIndexByName("TEN_DON_VI");

            if (cotMa < 0)
            {
                throw new InvalidOperationException(
                    "Không tìm thấy cột MA_DON_VI.");
            }

            double gioiHan = banKinh * banKinh;

            for (int i = 0; i < lopDiem.NumShapes; i++)
            {
                Shape hinh = lopDiem.get_Shape(i);
                if (hinh == null)
                    continue;

                MapWinGIS.Point diem =
                    hinh.get_Point(0);

                if (diem == null)
                    continue;

                double px = 0d;
                double py = 0d;

                banDoMapWinGIS.ProjToPixel(
                    diem.x,
                    diem.y,
                    ref px,
                    ref py);

                if (double.IsNaN(px) ||
                    double.IsInfinity(px) ||
                    double.IsNaN(py) ||
                    double.IsInfinity(py))
                    continue;

                double dx = px - xChuot;
                double dy = py - yChuot;
                double kc2 = dx * dx + dy * dy;

                if (kc2 > gioiHan)
                    continue;

                string ma =
                    (Convert.ToString(
                        lopDiem.get_CellValue(cotMa, i)) ?? "")
                        .Trim();

                if (ma.Length == 0)
                    continue;

                string ten =
                    cotTen < 0
                        ? ma
                        : (Convert.ToString(
                            lopDiem.get_CellValue(cotTen, i)) ?? "")
                            .Trim();

                ds.Add(new UngVienDonViBanDo
                {
                    HandleLop = handleLopDiemPostGIS,
                    ChiSoHinh = i,
                    Ma = ma,
                    Ten = ten,
                    X = diem.x,
                    Y = diem.y,
                    KhoangCachBinhPhuong = kc2
                });
            }

            ds.Sort((a, b) =>
            {
                int soSanh =
                    a.KhoangCachBinhPhuong
                     .CompareTo(b.KhoangCachBinhPhuong);

                return soSanh != 0
                    ? soSanh
                    : StringComparer.OrdinalIgnoreCase
                        .Compare(a.Ma, b.Ma);
            });

            return ds;
        }

        private int TimDiemGanNhatTheoPixel(
            Shapefile lopDiem,
            double xChuot,
            double yChuot,
            double banKinh)
        {
            List<UngVienDonViBanDo> ds =
                TtDv_TimUngVien(
                    lopDiem,
                    xChuot,
                    yChuot,
                    banKinh);

            return ds.Count == 0
                ? -1
                : ds[0].ChiSoHinh;
        }

        private void TtDv_MoMenuUngVien(
            List<UngVienDonViBanDo> ds,
            int x,
            int y)
        {
            TtDv_DongMenuUngVien();

            if (ds == null ||
                ds.Count == 0 ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            ttDvCacUngVien =
                new List<UngVienDonViBanDo>(ds);

            ttDvViTriChonX = x;
            ttDvViTriChonY = y;

            int rong = Math.Min(
                376,
                Math.Max(
                    165,
                    pnlMapCanvas.ClientSize.Width - 16));

            int caoTheoSoDiem =
                157 + Math.Min(ds.Count, 5) * 60;

            int cao = Math.Min(
                452,
                Math.Min(
                    caoTheoSoDiem,
                    Math.Max(
                        190,
                        pnlMapCanvas.ClientSize.Height - 16)));

            Panel bang = new Panel
            {
                Name = "ttDvBangChonUngVien",
                Width = rong,
                Height = cao,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            ttDvBangUngVien = bang;
            TtDv_BoGoc(bang, 12);

            bang.Paint += (s, e) =>
            {
                if (bang.ClientSize.Width < 4 ||
                    bang.ClientSize.Height < 4)
                    return;

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (GraphicsPath p =
                    TtDv_DuongBoGoc(
                        new Rectangle(
                            0, 0,
                            bang.Width - 1,
                            bang.Height - 1),
                        12))
                using (Pen vien =
                    new Pen(Color.FromArgb(191, 218, 231)))
                {
                    e.Graphics.DrawPath(vien, p);
                }
            };

            Panel dau = new Panel
            {
                Name = "ttDvTieuDeBangChon",
                Left = 1,
                Top = 1,
                Width = rong - 2,
                Height = 47,
                BackColor = Color.FromArgb(15, 120, 106),
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            Label tieuDe = new Label
            {
                Text = "CHỌN ĐƠN VỊ GẦN VỊ TRÍ NHẤP",
                Left = 12,
                Top = 7,
                Width = Math.Max(80, rong - 63),
                Height = 19,
                ForeColor = Color.White,
                Font = ttDvFontNhomLight,
                AutoEllipsis = true
            };

            Label soLuong = new Label
            {
                Text = ds.Count +
                    " đơn vị trong phạm vi chọn",
                Left = 12,
                Top = 26,
                Width = Math.Max(80, rong - 63),
                Height = 17,
                ForeColor = Color.FromArgb(211, 253, 239),
                Font = ttDvFontGhiChuLight
            };

            Button dong = new Button
            {
                Text = "×",
                Left = rong - 42,
                Top = 7,
                Width = 28,
                Height = 28,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(28, 145, 127),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = ttDvFontGiaTriLight,
                TabStop = false
            };

            dong.FlatAppearance.BorderSize = 0;

            dong.Click +=
                (s, e) => TtDv_DongMenuUngVien();

            dau.Controls.Add(tieuDe);
            dau.Controls.Add(soLuong);
            dau.Controls.Add(dong);

            Label lblTim = new Label
            {
                Left = 12,
                Top = 54,
                Width = Math.Max(90, rong - 24),
                Height = 17,
                Text = "TÌM THEO TÊN HOẶC MÃ ĐƠN VỊ",
                ForeColor = Color.FromArgb(82, 114, 134),
                Font = ttDvFontNhanLight
            };

            ttDvTimUngVien = new TextBox
            {
                Name = "ttDvTimUngVien",
                Left = 12,
                Top = 73,
                Width = rong - 24,
                Height = 27,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ttDvFontDongThongTin,
                BackColor = Color.FromArgb(247, 252, 255),
                ForeColor = Color.FromArgb(28, 58, 80)
            };

            ttDvTimUngVien.TextChanged +=
                (s, e) => TtDv_LamMoiBangUngVien();

            ttDvTimUngVien.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    TtDv_DongMenuUngVien();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    UngVienDonViBanDo dauTien =
                        TtDv_TimUngVienDauTien();

                    if (dauTien != null)
                        TtDv_ChonUngVienTuBang(dauTien);
                }
            };

            ttDvDanhSachUngVien = new FlowLayoutPanel
            {
                Name = "ttDvDanhSachUngVien",
                Left = 9,
                Top = 109,
                Width = rong - 18,
                Height = Math.Max(38, cao - 144),
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                Margin = Padding.Empty,
                Padding = new Padding(1, 0, 0, 0),
                BackColor = Color.White,
                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            ttDvThongKeUngVien = new Label
            {
                Left = 12,
                Top = cao - 30,
                Width = rong - 24,
                Height = 23,
                Anchor = AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(92, 123, 144),
                Font = ttDvFontGhiChuLight
            };

            bang.Controls.Add(dau);
            bang.Controls.Add(lblTim);
            bang.Controls.Add(ttDvTimUngVien);
            bang.Controls.Add(ttDvDanhSachUngVien);
            bang.Controls.Add(ttDvThongKeUngVien);

            pnlMapCanvas.Controls.Add(bang);
            bang.BringToFront();

            TtDv_LamMoiBangUngVien();
            TtDv_CanViTriBangUngVien();

            ttDvTimUngVien.Focus();

            statusGeneral.Text =
                "● Có " + ds.Count +
                " đơn vị gần vị trí nhấp; nhập tên hoặc mã để lọc.";
        }

        private static string TtDv_ChuanHoaTimKiem(
            string chuoi)
        {
            if (string.IsNullOrWhiteSpace(chuoi))
                return string.Empty;

            string dau = chuoi
                .Trim()
                .ToLowerInvariant()
                .Replace('đ', 'd')
                .Normalize(NormalizationForm.FormD);

            StringBuilder ketQua =
                new StringBuilder(dau.Length);

            foreach (char kyTu in dau)
            {
                UnicodeCategory loai =
                    CharUnicodeInfo.GetUnicodeCategory(kyTu);

                if (loai != UnicodeCategory.NonSpacingMark &&
                    loai != UnicodeCategory.SpacingCombiningMark)
                    ketQua.Append(kyTu);
            }

            return ketQua.ToString()
                .Normalize(NormalizationForm.FormC);
        }

        private List<UngVienDonViBanDo> TtDv_LocUngVien(
            string tuKhoa)
        {
            List<UngVienDonViBanDo> ketQua =
                new List<UngVienDonViBanDo>();

            if (ttDvCacUngVien == null)
                return ketQua;

            string khoa = TtDv_ChuanHoaTimKiem(tuKhoa);

            foreach (UngVienDonViBanDo diem in ttDvCacUngVien)
            {
                if (khoa.Length == 0 ||
                    TtDv_ChuanHoaTimKiem(diem.Ten)
                        .Contains(khoa) ||
                    TtDv_ChuanHoaTimKiem(diem.Ma)
                        .Contains(khoa))
                {
                    ketQua.Add(diem);
                }
            }

            return ketQua;
        }

        private UngVienDonViBanDo TtDv_TimUngVienDauTien()
        {
            if (ttDvTimUngVien == null)
                return null;

            List<UngVienDonViBanDo> ketQua =
                TtDv_LocUngVien(ttDvTimUngVien.Text);

            return ketQua.Count > 0
                ? ketQua[0]
                : null;
        }

        private void TtDv_LamMoiBangUngVien()
        {
            if (ttDvDanhSachUngVien == null ||
                ttDvDanhSachUngVien.IsDisposed ||
                ttDvTimUngVien == null ||
                ttDvTimUngVien.IsDisposed)
                return;

            List<UngVienDonViBanDo> ketQua =
                TtDv_LocUngVien(ttDvTimUngVien.Text);

            FlowLayoutPanel danhSach =
                ttDvDanhSachUngVien;

            danhSach.SuspendLayout();

            try
            {
                while (danhSach.Controls.Count > 0)
                {
                    Control cu = danhSach.Controls[0];
                    danhSach.Controls.RemoveAt(0);
                    cu.Dispose();
                }

                int rongDong = Math.Max(
                    125,
                    danhSach.ClientSize.Width -
                    SystemInformation.VerticalScrollBarWidth - 6);

                for (int i = 0; i < ketQua.Count; i++)
                {
                    UngVienDonViBanDo diem = ketQua[i];

                    Panel dong = new Panel
                    {
                        Width = rongDong,
                        Height = 58,
                        Margin = new Padding(0, 0, 0, 6),
                        BackColor = i == 0
                            ? Color.FromArgb(229, 250, 242)
                            : Color.FromArgb(246, 250, 253),
                        Cursor = Cursors.Hand
                    };

                    TtDv_BoGoc(dong, 8);

                    Label bieuTuong = new Label
                    {
                        Left = 8,
                        Top = 12,
                        Width = 34,
                        Height = 34,
                        Text = "⌖",
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.White,
                        ForeColor = Color.FromArgb(0, 155, 131),
                        Font = ttDvFontGiaTriLight,
                        Cursor = Cursors.Hand
                    };

                    Label ten = new Label
                    {
                        Left = 49,
                        Top = 6,
                        Width = Math.Max(40, rongDong - 59),
                        Height = 24,
                        Text = GiaTriHienThi(diem.Ten),
                        ForeColor = Color.FromArgb(28, 68, 90),
                        Font = ttDvFontGiaTriLight,
                        AutoEllipsis = true,
                        Cursor = Cursors.Hand
                    };

                    Label moTa = new Label
                    {
                        Left = 49,
                        Top = 31,
                        Width = Math.Max(40, rongDong - 59),
                        Height = 18,
                        Text =
                            (ttDvCacUngVien != null &&
                             ttDvCacUngVien.Count > 0 &&
                             object.ReferenceEquals(
                                 diem, ttDvCacUngVien[0])
                                ? "Gần nhất  ·  "
                                : "") +
                            "Mã: " + diem.Ma + "  ·  " +
                            Math.Sqrt(
                                diem.KhoangCachBinhPhuong)
                                .ToString(
                                    "0",
                                    CultureInfo.InvariantCulture) +
                            " px",
                        ForeColor = Color.FromArgb(99, 130, 149),
                        Font = ttDvFontGhiChuLight,
                        AutoEllipsis = true,
                        Cursor = Cursors.Hand
                    };

                    Action chon =
                        () => TtDv_ChonUngVienTuBang(diem);

                    dong.Click += (s, e) => chon();
                    bieuTuong.Click += (s, e) => chon();
                    ten.Click += (s, e) => chon();
                    moTa.Click += (s, e) => chon();

                    dong.Controls.Add(bieuTuong);
                    dong.Controls.Add(ten);
                    dong.Controls.Add(moTa);

                    danhSach.Controls.Add(dong);
                }

                if (ketQua.Count == 0)
                {
                    Label rong = new Label
                    {
                        Width = rongDong,
                        Height = 66,
                        Text = "Không tìm thấy đơn vị phù hợp.",
                        TextAlign = ContentAlignment.MiddleCenter,
                        ForeColor = Color.FromArgb(105, 127, 145),
                        Font = ttDvFontDongThongTin
                    };

                    danhSach.Controls.Add(rong);
                }

                if (ttDvThongKeUngVien != null)
                {
                    ttDvThongKeUngVien.Text =
                        "Hiển thị " + ketQua.Count +
                        "/" + ttDvCacUngVien.Count +
                        " đơn vị  ·  Enter: chọn gần nhất  ·  Esc: đóng";
                }
            }
            finally
            {
                danhSach.ResumeLayout(true);
            }
        }

        private void TtDv_ChonUngVienTuBang(
            UngVienDonViBanDo diem)
        {
            int x = ttDvViTriChonX;
            int y = ttDvViTriChonY;

            TtDv_DongMenuUngVien();

            if (diem == null ||
                IsDisposed ||
                Disposing ||
                !IsHandleCreated)
                return;

            BeginInvoke(new MethodInvoker(() =>
            {
                if (!IsDisposed &&
                    !Disposing &&
                    CoQuyen(
                        ThaoTacDonViQuanDoi.Xem,
                        diem.Ma))
                {
                    TtDv_YeuCauXemHoSo(diem, x, y);
                }
            }));
        }

        private void TtDv_CanViTriBangUngVien()
        {
            if (ttDvBangUngVien == null ||
                ttDvBangUngVien.IsDisposed ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !pnlMapCanvas.IsHandleCreated ||
                !banDoMapWinGIS.IsHandleCreated)
                return;

            System.Drawing.Point viTri =
                pnlMapCanvas.PointToClient(
                    banDoMapWinGIS.PointToScreen(
                        new System.Drawing.Point(
                            ttDvViTriChonX,
                            ttDvViTriChonY)));

            int rong = pnlMapCanvas.ClientSize.Width;
            int cao = pnlMapCanvas.ClientSize.Height;

            int x = viTri.X + 17;

            if (x + ttDvBangUngVien.Width > rong - 8)
                x = viTri.X - ttDvBangUngVien.Width - 17;

            int y = viTri.Y - 25;

            x = Math.Max(
                8,
                Math.Min(
                    x,
                    rong - ttDvBangUngVien.Width - 8));

            y = Math.Max(
                8,
                Math.Min(
                    y,
                    cao - ttDvBangUngVien.Height - 8));

            ttDvBangUngVien.Location =
                new System.Drawing.Point(x, y);

            ttDvBangUngVien.BringToFront();
        }

        private void TtDv_DongMenuUngVien()
        {
            ContextMenuStrip menu = ttDvMenuUngVien;
            ttDvMenuUngVien = null;

            if (menu != null)
            {
                menu.Close();
                menu.Dispose();
            }

            Panel bang = ttDvBangUngVien;

            ttDvBangUngVien = null;
            ttDvTimUngVien = null;
            ttDvDanhSachUngVien = null;
            ttDvThongKeUngVien = null;
            ttDvCacUngVien = null;

            if (bang != null && !bang.IsDisposed)
            {
                if (bang.Parent != null)
                    bang.Parent.Controls.Remove(bang);

                bang.Dispose();
            }
        }
    }
}