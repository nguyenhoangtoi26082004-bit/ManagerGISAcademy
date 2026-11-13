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
        private void TxtQuickSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Header_DongGoiY();
                return;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            {
                if (headerPopup == null || !headerPopup.Visible)
                    return; // Cho phep mui ten khi popup khong mo.

                e.SuppressKeyPress = true;
                if (headerKetQua.Count > 0)
                {
                    int buoc = e.KeyCode == Keys.Down ? 1 : -1;
                    headerChiSoChon = (headerChiSoChon + buoc +
                        headerKetQua.Count) % headerKetQua.Count;
                    Header_CapNhatDongDangChon();
                }
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                string khoa = Header_ChuanHoa(txtQuickSearch.Text);
                if (headerPopup != null && headerPopup.Visible &&
                    headerKetQua.Count > 0 &&
                    headerChiSoChon >= 0 &&
                    headerChiSoChon < headerKetQua.Count &&
                    khoa == headerTuKhoaDaHienThi)
                {
                    Header_ChonDonVi(headerKetQua[headerChiSoChon]);
                }
                else
                {
                    Header_TimNgay();
                }
            }
        }

        private void Header_TextChanged(object sender, EventArgs e)
        {
            if (headerBoQuaThayDoi || IsDisposed || Disposing)
                return;

            if (headerHenGoiY == null)
            {
                headerHenGoiY = new System.Windows.Forms.Timer { Interval = 180 };
                headerHenGoiY.Tick += (s, args) =>
                {
                    headerHenGoiY.Stop();
                    Header_HienGoiY();
                };
            }
            headerHenGoiY.Stop();
            headerChiSoChon = 0;
            headerKetQua.Clear(); // Không cho Enter chọn kết quả của từ khóa cũ.
            headerTuKhoaDaHienThi = string.Empty;
            headerConTimKiem = !string.IsNullOrWhiteSpace(txtQuickSearch.Text);
            if (!headerConTimKiem)
            {
                Header_DongGoiY();
                return;
            }

            bool cacheConMoi = headerBoNho != null &&
                DateTime.UtcNow - headerLanTai <= headerThoiHanCache;
            headerHenGoiY.Interval = cacheConMoi ? 100 : 230;
            headerHenGoiY.Start();
        }

        private void Header_TimNgay()
        {
            if (headerHenGoiY != null) headerHenGoiY.Stop();
            if (string.IsNullOrWhiteSpace(txtQuickSearch.Text))
            {
                Header_DongGoiY();
                if (txtQuickSearch.CanFocus) txtQuickSearch.Focus();
                statusGeneral.Text = "● Nhập tên hoặc mã đơn vị để tìm nhanh.";
                return;
            }

            // Nút TÌM chỉ mở gợi ý, không chuyển sang UcTraCuu.
            headerConTimKiem = true;
            if (txtQuickSearch.CanFocus && !txtQuickSearch.Focused)
            {
                txtQuickSearch.Focus();
                txtQuickSearch.SelectionStart = txtQuickSearch.TextLength;
                txtQuickSearch.SelectionLength = 0;
            }
            Header_HienGoiY();
        }

        private async void Header_HienGoiY()
        {
            if (IsDisposed || Disposing || !IsHandleCreated ||
                !headerConTimKiem ||
                string.IsNullOrWhiteSpace(txtQuickSearch.Text))
                return;

            if (nghiepVuDonVi == null || !nghiepVuDonVi.DaCauHinh ||
                !CoQuyen(ThaoTacDonViQuanDoi.Xem))
            {
                Header_HienDanhSach(new List<DonViQuanDoi>(),
                    "Chưa có quyền xem dữ liệu đơn vị.");
                return;
            }

            bool cacheConMoi = headerBoNho != null &&
                DateTime.UtcNow - headerLanTai <= headerThoiHanCache;
            if (!cacheConMoi)
            {
                if (nativeBoNhoHoSo != null && !nativePhaiTaiLaiHoSo &&
                    DateTime.UtcNow - nativeLanTaiHoSo <= nativeThoiHanBoNho)
                {
                    headerBoNho = nativeBoNhoHoSo.ToList();
                    headerLanTai = DateTime.UtcNow;
                }
                else if (!headerDangTai)
                {
                    headerDangTai = true;
                    Header_HienDanhSach(new List<DonViQuanDoi>(),
                        "Đang đọc dữ liệu từ PostGIS...");
                    NghiepVuDonVi bll = nghiepVuDonVi;
                    try
                    {
                        List<DonViQuanDoi> duLieu = await Task.Run(
                            () => bll.LayTatCaDonVi());
                        if (IsDisposed || Disposing || !IsHandleCreated ||
                            !object.ReferenceEquals(bll, nghiepVuDonVi))
                            return;
                        headerBoNho = duLieu ?? new List<DonViQuanDoi>();
                        headerLanTai = DateTime.UtcNow;
                    }
                    catch (Exception ex)
                    {
                        if (!IsDisposed && !Disposing && IsHandleCreated &&
                            headerConTimKiem)
                            Header_HienDanhSach(new List<DonViQuanDoi>(),
                                "Không tải được dữ liệu: " + ex.Message);
                        return;
                    }
                    finally
                    {
                        headerDangTai = false;
                    }
                }
                else
                {
                    // Truy vấn đang chạy sẽ tự dùng từ khóa mới nhất khi xong.
                    return;
                }
            }

            if (headerBoNho == null || IsDisposed || Disposing ||
                !headerConTimKiem ||
                string.IsNullOrWhiteSpace(txtQuickSearch.Text))
                return;

            string khoa = Header_ChuanHoa(txtQuickSearch.Text);
            if (khoa.Length == 0) { Header_DongGoiY(); return; }

            List<DonViQuanDoi> ds = headerBoNho
                .Where(d => d != null && !string.IsNullOrWhiteSpace(d.MaDonVi) &&
                    CoQuyen(ThaoTacDonViQuanDoi.Xem, d.MaDonVi))
                .Select(d => new { DonVi = d, Diem = Header_DiemPhuHop(d, khoa) })
                .Where(x => x.Diem < 100)
                .OrderBy(x => x.Diem)
                .ThenBy(x => x.DonVi.TenDonVi ?? "",
                    StringComparer.CurrentCultureIgnoreCase)
                .Take(8)
                .Select(x => x.DonVi)
                .ToList();

            Header_HienDanhSach(ds, ds.Count == 0
                ? "Không tìm thấy đơn vị phù hợp." : null);
        }

        private static string Header_ChuanHoa(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            string nguon = s.Trim().ToLowerInvariant().Replace('đ', 'd')
                .Normalize(NormalizationForm.FormD);
            StringBuilder ketQua = new StringBuilder(nguon.Length);
            foreach (char c in nguon)
                if (CharUnicodeInfo.GetUnicodeCategory(c) !=
                    UnicodeCategory.NonSpacingMark)
                    ketQua.Append(c);
            return ketQua.ToString().Normalize(NormalizationForm.FormC);
        }

        private static int Header_DiemPhuHop(DonViQuanDoi d, string khoa)
        {
            if (d == null || khoa.Length == 0) return 100;
            string ma = Header_ChuanHoa(d.MaDonVi);
            string ten = Header_ChuanHoa(d.TenDonVi);
            string tat = Header_ChuanHoa(d.TenVietTat);
            if (ma == khoa) return 0;
            if (ten == khoa || tat == khoa) return 1;
            if (ten.StartsWith(khoa, StringComparison.Ordinal)) return 2;
            if (tat.StartsWith(khoa, StringComparison.Ordinal)) return 3;
            if (ma.StartsWith(khoa, StringComparison.Ordinal)) return 4;
            if (ten.Contains(" " + khoa)) return 5;
            if (ten.Contains(khoa)) return 6;
            if (tat.Contains(khoa) || ma.Contains(khoa)) return 7;
            return 100;
        }

        private void Header_HienDanhSach(
            List<DonViQuanDoi> danhSach, string thongBao)
        {
            if (IsDisposed || Disposing || !txtQuickSearch.Visible ||
                !txtQuickSearch.IsHandleCreated || !headerConTimKiem)
                return;

            Header_DamBaoPopup();
            headerKetQua = danhSach ?? new List<DonViQuanDoi>();
            headerChiSoChon = 0;
            headerTuKhoaDaHienThi = Header_ChuanHoa(txtQuickSearch.Text);
            headerDongKetQua.Clear();
            headerPopupDanhSach.SuspendLayout();
            try
            {
                while (headerPopupDanhSach.Controls.Count > 0)
                {
                    Control cu = headerPopupDanhSach.Controls[0];
                    headerPopupDanhSach.Controls.RemoveAt(0);
                    cu.Dispose();
                }

                if (headerKetQua.Count == 0)
                {
                    headerPopupDanhSach.Controls.Add(new Label
                    {
                        Text = thongBao ?? "Không có kết quả.",
                        Width = 349,
                        Height = 58,
                        ForeColor = Color.FromArgb(85, 109, 127),
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 9F),
                        Margin = Padding.Empty
                    });
                }
                else
                {
                    foreach (DonViQuanDoi donVi in headerKetQua)
                    {
                        Panel dong = new Panel
                        {
                            Width = 349,
                            Height = 55,
                            Margin = new Padding(0, 0, 0, 3),
                            Cursor = Cursors.Hand,
                            BackColor = Color.White
                        };
                        Label ten = new Label
                        {
                            Left = 11,
                            Top = 6,
                            Width = 325,
                            Height = 24,
                            Text = donVi.TenDonVi ?? donVi.MaDonVi,
                            ForeColor = Color.FromArgb(29, 61, 81),
                            Font = new Font("Segoe UI", 9.2F, FontStyle.Bold),
                            AutoEllipsis = true,
                            Cursor = Cursors.Hand
                        };
                        Label moTa = new Label
                        {
                            Left = 11,
                            Top = 30,
                            Width = 325,
                            Height = 18,
                            Text = "Mã: " + donVi.MaDonVi + "  ·  " +
                                (donVi.QuanHuyen ?? "Chưa cập nhật địa bàn"),
                            ForeColor = Color.FromArgb(103, 126, 141),
                            Font = new Font("Segoe UI", 8F),
                            AutoEllipsis = true,
                            Cursor = Cursors.Hand
                        };
                        Action chon = () => Header_ChonDonVi(donVi);
                        dong.Click += (s, e) => chon();
                        ten.Click += (s, e) => chon();
                        moTa.Click += (s, e) => chon();
                        dong.MouseEnter += (s, e) =>
                        {
                            int i = headerDongKetQua.IndexOf(dong);
                            if (i >= 0 && headerChiSoChon != i)
                            {
                                headerChiSoChon = i;
                                Header_CapNhatDongDangChon();
                            }
                        };
                        dong.Controls.Add(ten);
                        dong.Controls.Add(moTa);
                        headerPopupDanhSach.Controls.Add(dong);
                        headerDongKetQua.Add(dong);
                    }
                }
            }
            finally { headerPopupDanhSach.ResumeLayout(true); }

            int soDong = Math.Max(1, headerKetQua.Count);
            int chieuCaoDs = Math.Min(6, soDong) * 58;
            headerPopupDanhSach.Height = chieuCaoDs;
            headerPopupThongKe.Top = 46 + chieuCaoDs + 2;
            headerPopupThongKe.Text = headerKetQua.Count > 0
                ? headerKetQua.Count +
                  " gợi ý · ↑ ↓ để chọn · Enter để mở · Esc để đóng"
                : "Nhập tên hoặc mã đơn vị để tìm nhanh";

            int chieuCao = headerPopupThongKe.Bottom + 4;
            headerPopup.Size = new Size(385, chieuCao);
            Header_CapNhatDongDangChon();
            Header_DatViTriPopup();
            if (!headerPopup.Visible)
                headerPopup.Show(this);
            // Không gọi Focus(), Activate() hay Select() trên popup.
        }
        private void Header_DamBaoPopup()
        {
            if (headerPopup != null && !headerPopup.IsDisposed)
                return;

            // Form độc lập không kích hoạt, không tham gia ToolStripManager.
            // Tránh ToolStripDropDown bắt phím khi bộ gõ tiếng Việt đang hoạt động.
            headerPopup = new HeaderGoiYForm
            {
                Size = new Size(385, 143),
                BackColor = Color.FromArgb(210, 231, 229),
                Padding = new Padding(1)
            };
            headerPopupNoiDung = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            Label dau = new Label
            {
                Text = "GỢI Ý ĐƠN VỊ TỪ POSTGIS",
                Left = 12,
                Top = 7,
                Width = 360,
                Height = 30,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(12, 143, 119)
            };
            headerPopupDanhSach = new FlowLayoutPanel
            {
                Left = 6,
                Top = 46,
                Width = 373,
                Height = 58,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                BackColor = Color.White,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            headerPopupThongKe = new Label
            {
                Left = 10,
                Top = 108,
                Width = 367,
                Height = 27,
                ForeColor = Color.FromArgb(111, 137, 152),
                Font = new Font("Segoe UI", 7.8F)
            };
            headerPopupNoiDung.Controls.Add(dau);
            headerPopupNoiDung.Controls.Add(headerPopupDanhSach);
            headerPopupNoiDung.Controls.Add(headerPopupThongKe);
            headerPopup.Controls.Add(headerPopupNoiDung);

            if (headerBoLocClickNgoai == null)
            {
                headerBoLocClickNgoai = new HeaderGoiYOutsideClickFilter(this);
                Application.AddMessageFilter(headerBoLocClickNgoai);
            }
        }

        private void Header_DatViTriPopup()
        {
            if (headerPopup == null || headerPopup.IsDisposed ||
                txtQuickSearch == null || txtQuickSearch.IsDisposed)
                return;
            System.Drawing.Point duoi = txtQuickSearch.PointToScreen(
                new System.Drawing.Point(0, txtQuickSearch.Height + 5));
            Rectangle khuVuc = Screen.GetWorkingArea(duoi);
            int x = Math.Max(khuVuc.Left, Math.Min(duoi.X,
                khuVuc.Right - headerPopup.Width));
            int y = duoi.Y;
            if (y + headerPopup.Height > khuVuc.Bottom)
                y = txtQuickSearch.PointToScreen(System.Drawing.Point.Empty).Y
                    - headerPopup.Height - 5;
            y = Math.Max(khuVuc.Top, y);
            headerPopup.Location = new System.Drawing.Point(x, y);
        }

        private void Header_CapNhatDongDangChon()
        {
            for (int i = 0; i < headerDongKetQua.Count; i++)
            {
                Panel dong = headerDongKetQua[i];
                if (dong.IsDisposed) continue;
                dong.BackColor = i == headerChiSoChon
                    ? Color.FromArgb(224, 248, 240) : Color.White;
                if (i == headerChiSoChon && headerPopupDanhSach != null)
                    headerPopupDanhSach.ScrollControlIntoView(dong);
            }
        }

        private void Header_ChonDonVi(DonViQuanDoi donVi)
        {
            if (donVi == null || string.IsNullOrWhiteSpace(donVi.MaDonVi))
                return;
            if (Header_DiemPhuHop(donVi,
                Header_ChuanHoa(txtQuickSearch.Text)) >= 100)
                return; // Kết quả cũ trong lúc người dùng đang nhập thêm.
            if (!CoQuyen(ThaoTacDonViQuanDoi.Xem, donVi.MaDonVi))
            {
                Header_DongGoiY();
                ThongBaoKhongCoQuyen("xem hồ sơ");
                return;
            }
            headerBoQuaThayDoi = true;
            try { txtQuickSearch.Text = donVi.TenDonVi ?? donVi.MaDonVi; }
            finally { headerBoQuaThayDoi = false; }
            Header_DongGoiY();
            if (!IsHandleCreated || IsDisposed || Disposing) return;
            BeginInvoke(new MethodInvoker(() =>
            {
                if (IsDisposed || Disposing) return;
                DinhViDonVi(donVi.MaDonVi);
                Header_MoHoSoTrongLopDiem(donVi);
            }));
        }

        private void Header_MoHoSoTrongLopDiem(DonViQuanDoi donVi)
        {
            // Nếu lớp bản đồ chưa có điểm, vẫn giữ kết quả định vị WGS84.
            if (lblPageTitle.Text != "Bản đồ GIS" ||
                !daKhoiTaoThongTinBanDo || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed || handleLopDiemPostGIS < 0 ||
                ketNoiMapWinGIS == null ||
                !ketNoiMapWinGIS.CoLop(handleLopDiemPostGIS) ||
                !ketNoiMapWinGIS.LayHienThiLop(handleLopDiemPostGIS)) return;
            try
            {
                MapWinGIS.Shapefile lop =
                    banDoMapWinGIS.get_Shapefile(handleLopDiemPostGIS);
                if (lop == null) return;
                int cotMa = lop.get_FieldIndexByName("MA_DON_VI");
                if (cotMa < 0) return;
                for (int i = 0; i < lop.NumShapes; i++)
                {
                    string ma = (Convert.ToString(
                        lop.get_CellValue(cotMa, i)) ?? "").Trim();
                    if (!string.Equals(ma, donVi.MaDonVi,
                        StringComparison.OrdinalIgnoreCase)) continue;
                    MapWinGIS.Shape hinh = lop.get_Shape(i);
                    if (hinh == null) return;
                    MapWinGIS.Point diem = hinh.get_Point(0);
                    if (diem == null) return;
                    double px = 0d, py = 0d;
                    banDoMapWinGIS.ProjToPixel(diem.x, diem.y, ref px, ref py);
                    UngVienDonViBanDo ungVien = new UngVienDonViBanDo
                    {
                        HandleLop = handleLopDiemPostGIS,
                        ChiSoHinh = i,
                        Ma = donVi.MaDonVi,
                        Ten = donVi.TenDonVi,
                        X = diem.x,
                        Y = diem.y
                    };
                    TtDv_YeuCauXemHoSo(ungVien,
                        (int)Math.Round(px), (int)Math.Round(py));
                    return;
                }
                statusGeneral.Text =
                    "● Đã định vị; điểm chưa có trong lớp đang bật.";
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("HEADER_SEARCH_PROFILE_FAILED | " + ex);
                statusGeneral.Text = "● Đã định vị; chưa mở được bảng hồ sơ.";
            }
        }

        private void Header_DongGoiY()
        {
            headerConTimKiem = false;
            headerTuKhoaDaHienThi = string.Empty;
            if (headerHenGoiY != null) headerHenGoiY.Stop();
            if (headerPopup != null && !headerPopup.IsDisposed &&
                headerPopup.Visible)
                headerPopup.Hide(); // Không Dispose; tái sử dụng khi gõ lại.
        }

        private void Header_GiaiPhong()
        {
            if (headerBoLocClickNgoai != null)
            {
                Application.RemoveMessageFilter(headerBoLocClickNgoai);
                headerBoLocClickNgoai = null;
            }
            if (headerHenGoiY != null)
            {
                headerHenGoiY.Stop();
                headerHenGoiY.Dispose();
                headerHenGoiY = null;
            }
            if (headerPopup != null)
            {
                headerPopup.Dispose();
                headerPopup = null;
            }
            headerBoNho = null;
            headerKetQua.Clear();
        }

        private sealed class HeaderGoiYForm : Form
        {
            public HeaderGoiYForm()
            {
                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                AutoScaleMode = AutoScaleMode.None;
                ControlBox = false;
                MinimizeBox = false;
                MaximizeBox = false;
            }

            protected override bool ShowWithoutActivation
            {
                get { return true; }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                    cp.ExStyle |= 0x00000080; // WS_EX_TOOLWINDOW
                    return cp;
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x0021) // WM_MOUSEACTIVATE
                {
                    m.Result = new IntPtr(3); // MA_NOACTIVATE
                    return;
                }
                base.WndProc(ref m);
            }
        }

        private sealed class HeaderGoiYOutsideClickFilter : IMessageFilter
        {
            private readonly FormMain form;
            internal HeaderGoiYOutsideClickFilter(FormMain formMain)
            {
                form = formMain;
            }

            public bool PreFilterMessage(ref Message m)
            {
                if (form.IsDisposed || form.Disposing ||
                    form.headerPopup == null || form.headerPopup.IsDisposed ||
                    !form.headerPopup.Visible)
                    return false;

                if (m.Msg != 0x0201 && m.Msg != 0x0204 &&
                    m.Msg != 0x0207 && m.Msg != 0x00A1 &&
                    m.Msg != 0x00A4)
                    return false;
                System.Drawing.Point viTri = Cursor.Position;
                if (form.headerPopup.Bounds.Contains(viTri)) return false;
                if (!form.txtQuickSearch.IsDisposed &&
                    form.txtQuickSearch.RectangleToScreen(
                        form.txtQuickSearch.ClientRectangle).Contains(viTri))
                    return false;
                if (!form.btnQuickSearch.IsDisposed &&
                    form.btnQuickSearch.RectangleToScreen(
                        form.btnQuickSearch.ClientRectangle).Contains(viTri))
                    return false;
                form.Header_DongGoiY();
                return false; // Không nuốt sự kiện click bên ngoài.
            }
        }
    }
}