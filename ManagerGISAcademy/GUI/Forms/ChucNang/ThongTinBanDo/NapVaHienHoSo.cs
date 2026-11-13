
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using SpatialDrawer = ManagerGISAcademy.GIS.VeDuLieuKhongGian;
using DonViQuanDoiBLL = ManagerGISAcademy.BLL.NghiepVuDonVi;
using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Panel chi tiet va luong mo ho so tu ban do.
    /// Tuong thich C# 7.3 / .NET Framework 4.8.
    /// Hai lop logo HQTCSDL duoc nhan dien trong LogoDonViQuanDoi.cs.
    /// </summary>
    public partial class FormMain
    {
        // 1. Luong cu: chi cho phep khi lop legacy van hien thi.
        private void TtDv_YeuCauXemHoSo(UngVienDonViBanDo diem, int x, int y)
        {
            if (diem == null || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed || diem.HandleLop < 0)
                return;

            bool lopHien;
            try { lopHien = banDoMapWinGIS.get_LayerVisible(diem.HandleLop); }
            catch { return; }
            if (!lopHien) return; // Tuyet doi khong mo lai lop 51 diem cu.

            if (nghiepVuDonVi == null || !nghiepVuDonVi.DaCauHinh ||
                !CoQuyen(ThaoTacDonViQuanDoi.Xem, diem.Ma))
            {
                statusGeneral.Text = "● Không có quyền xem hồ sơ này.";
                return;
            }

            logoDangHienHoSoMoi = false;
            TtDv_DonHoSoDangHienThi(false);
            ttDvDiemDangHienThi = diem;
            TtDv_VeDanhDau(diem);
            ttDvYeuCauDangCho = new YeuCauHoSoBanDo
            {
                Phien = ++ttDvPhienTraCuu,
                Diem = diem,
                PixelX = x,
                PixelY = y
            };
            statusGeneral.Text = "● Đang đọc hồ sơ: " + diem.Ma;
            if (!ttDvDangTaiHoSo) TtDv_XuLyYeuCauHoSo();
        }

        // 2. Tai ho so legacy khong chan luong logo moi.
        private async void TtDv_XuLyYeuCauHoSo()
        {
            if (ttDvDangTaiHoSo) return;
            ttDvDangTaiHoSo = true;
            try
            {
                while (ttDvYeuCauDangCho != null && !IsDisposed && !Disposing)
                {
                    YeuCauHoSoBanDo yc = ttDvYeuCauDangCho;
                    ttDvYeuCauDangCho = null;
                    try
                    {
                        if (!TtDv_YeuCauConHieuLuc(yc)) continue;
                        DonViQuanDoiBLL bll = nghiepVuDonVi;
                        DonViQuanDoi donVi = await Task.Run(() =>
                            bll.LayDonViTheoMa(yc.Diem.Ma));
                        if (logoDangHienHoSoMoi ||
                            !TtDv_YeuCauConHieuLuc(yc) ||
                            !ReferenceEquals(bll, nghiepVuDonVi)) continue;
                        if (donVi == null)
                        {
                            statusGeneral.Text = "● Không tìm thấy hồ sơ tương ứng.";
                            continue;
                        }
                        if (!CoQuyen(ThaoTacDonViQuanDoi.Xem, yc.Diem.Ma))
                            continue;
                        ttDvDiemDangHienThi = yc.Diem;
                        TtDv_VeDanhDau(yc.Diem);
                        HienThiThongTinDiem(donVi, yc.PixelX, yc.PixelY);
                    }
                    catch (Exception ex)
                    {
                        GhiNhatKyBanDo("LEGACY_INFO_FAILED | " + ex);
                        if (!logoDangHienHoSoMoi && !IsDisposed)
                            statusGeneral.Text = "● Lỗi mở hồ sơ: " + ex.Message;
                    }
                }
            }
            finally { ttDvDangTaiHoSo = false; }
        }

        private bool TtDv_YeuCauConHieuLuc(YeuCauHoSoBanDo yc)
        {
            if (yc == null || yc.Diem == null ||
                yc.Phien != ttDvPhienTraCuu || logoDangHienHoSoMoi ||
                IsDisposed || Disposing ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null ||
                handleLopDiemPostGIS != yc.Diem.HandleLop ||
                !ketNoiMapWinGIS.CoLop(yc.Diem.HandleLop) ||
                !ketNoiMapWinGIS.LayHienThiLop(yc.Diem.HandleLop))
                return false;
            Shapefile sf = banDoMapWinGIS.get_Shapefile(yc.Diem.HandleLop);
            if (sf == null || yc.Diem.ChiSoHinh < 0 ||
                yc.Diem.ChiSoHinh >= sf.NumShapes) return false;
            int cot = sf.get_FieldIndexByName("MA_DON_VI");
            return cot >= 0 && string.Equals(
                (Convert.ToString(sf.get_CellValue(cot, yc.Diem.ChiSoHinh)) ?? "").Trim(),
                yc.Diem.Ma, StringComparison.OrdinalIgnoreCase);
        }

        private void TtDv_VeDanhDau(UngVienDonViBanDo diem)
        {
            if (diem == null || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return;
            bool tuKhoa = false;
            try
            {
                if (ttDvBoVe == null) ttDvBoVe = new SpatialDrawer(banDoMapWinGIS);
                if (banDoMapWinGIS.IsLocked == tkLockMode.lmUnlock)
                {
                    banDoMapWinGIS.LockWindow(tkLockMode.lmLock);
                    tuKhoa = true;
                }
                ttDvBoVe.LamMoiLopVe(TtDv_MaLopDanhDau);
                ttDvBoVe.VeVongTronPixel(TtDv_MaLopDanhDau,
                    new ToaDoVe(diem.X, diem.Y), 17d,
                    Color.FromArgb(255, 201, 70), false, 3, 255);
            }
            catch (Exception ex) { GhiNhatKyBanDo("LEGACY_MARK_FAILED | " + ex); }
            finally
            {
                if (tuKhoa && !banDoMapWinGIS.IsDisposed)
                    banDoMapWinGIS.LockWindow(tkLockMode.lmUnlock);
            }
        }

        private void TtDv_XoaDanhDau()
        {
            if (ttDvBoVe == null || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return;
            try { ttDvBoVe.XoaLopVe(TtDv_MaLopDanhDau); }
            catch (Exception ex) { GhiNhatKyBanDo("LEGACY_MARK_CLEAR_FAILED | " + ex); }
        }

        // 3. Mo panel cho 126 Ban CHQS + 43 ho so moi.
        private void TtDv_HienThiHoSoLogo(DonViQuanDoi donVi, double x,
            double y, bool laDiemDaiDien)
        {
            if (donVi == null || IsDisposed || Disposing) return;
            if (pnlMapCard == null || pnlMapCard.IsDisposed) return;

            ++ttDvPhienTraCuu; // huy bat ky ket qua tai legacy nao dang cho
            ttDvYeuCauDangCho = null;
            ttDvDiemDangHienThi = null;
            logoDangHienHoSoMoi = true;
            logoDiemChonX = x;
            logoDiemChonY = y;
            logoDiemChonLaDaiDien = laDiemDaiDien;

            if (pnlThongTinDiem == null || pnlThongTinDiem.IsDisposed)
                TaoKhungThongTinDiem();

            HienThiThongTinDiem(donVi, 0, 0);
            if (ttDvBtnHoSo != null)
            {
                // Don vi xem tu lop logo: nut nay dung de dinh vi lai tru so.
                ttDvBtnHoSo.Text = "⌖  ĐỊNH VỊ LẠI TRỤ SỞ";
                ttDvBtnHoSo.Enabled = CoQuyen(ThaoTacDonViQuanDoi.Sua, donVi.MaDonVi);
            }
            if (ttDvBtnDinhTam != null)
                ttDvBtnDinhTam.Enabled = true;
            if (ttDvCotHoSo != null)
            {
                ttDvCotHoSo.Visible = true;
                ttDvCotHoSo.BringToFront();
            }
            if (pnlThongTinDiem != null)
            {
                pnlThongTinDiem.Visible = true;
                pnlThongTinDiem.BringToFront();
            }
            if (pnlCheDoBanDo != null) pnlCheDoBanDo.BringToFront();
            if (pnlMapTools != null) pnlMapTools.BringToFront();
            statusGeneral.Text = "● Đang xem: " + donVi.TenDonVi +
                (laDiemDaiDien ? " · Điểm đại diện địa bàn (không phải trụ sở)." :
                                    " · Điểm hồ sơ GIS.");
        }

        // 4. UI giu phong cach hien tai cua du an.
        private static Color TtDv_Mau(int r, int g, int b)
        { return Color.FromArgb(r, g, b); }

        private static GraphicsPath TtDv_DuongBoGoc(Rectangle r, int banKinh)
        {
            int d = Math.Max(2, Math.Min(banKinh * 2, Math.Min(r.Width, r.Height)));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.Left, r.Top, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Top, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        private static void TtDv_BoGoc(Control control, int banKinh)
        {
            if (control == null) return;
            Action capNhat = () =>
            {
                if (control.IsDisposed || control.Width < 3 || control.Height < 3) return;
                using (GraphicsPath p = TtDv_DuongBoGoc(
                    new Rectangle(0, 0, control.Width, control.Height), banKinh))
                {
                    Region cu = control.Region;
                    control.Region = new Region(p);
                    if (cu != null) cu.Dispose();
                }
            };
            control.SizeChanged += (s, e) => capNhat();
            capNhat();
        }

        private static void TtDv_DatKieuNut(Button nut, Color mau)
        {
            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;
            nut.UseVisualStyleBackColor = false;
            nut.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            nut.Cursor = Cursors.Hand;
            nut.FlatAppearance.MouseOverBackColor = ControlPaint.Light(mau, 0.13F);
            nut.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(mau, 0.10F);
            TtDv_BoGoc(nut, 10);
        }

        private void TaoKhungThongTinDiem()
        {
            if (pnlThongTinDiem != null && !pnlThongTinDiem.IsDisposed) return;
            ttDvCotHoSo = new Panel
            {
                Name = "ttDvCotHoSo",
                Dock = DockStyle.Right,
                Width = 425,
                BackColor = TtDv_Mau(249, 252, 255),
                Visible = false
            };
            pnlThongTinDiem = new Panel
            {
                Name = "pnlThongTinDiemLight",
                Dock = DockStyle.Fill,
                BackColor = TtDv_Mau(249, 252, 255),
                Visible = false
            };
            Panel dau = new Panel
            {
                Dock = DockStyle.Top,
                Height = 129,
                BackColor = TtDv_Mau(18, 165, 157)
            };
            dau.Paint += (s, e) =>
            {
                if (dau.Width <= 0 || dau.Height <= 0) return;
                using (LinearGradientBrush b = new LinearGradientBrush(
                    dau.ClientRectangle, TtDv_Mau(0, 173, 148),
                    TtDv_Mau(104, 91, 235), LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(b, dau.ClientRectangle);
            };
            Label nhan = new Label
            {
                Text = "✦   THÔNG TIN ĐƠN VỊ",
                Left = 18,
                Top = 12,
                Width = 300,
                Height = 24,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.8F, FontStyle.Bold)
            };
            lblTenDiem = new Label
            {
                Left = 18,
                Top = 48,
                Width = 355,
                Height = 66,
                Text = "THÔNG TIN ĐƠN VỊ",
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Button dong = new Button
            {
                Text = "×",
                Width = 30,
                Height = 30,
                Top = 9,
                BackColor = TtDv_Mau(69, 158, 174),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold)
            };
            dong.FlatAppearance.BorderSize = 0;
            TtDv_BoGoc(dong, 8);
            dong.Click += (s, e) => AnKhungThongTinDiem();
            dau.Controls.Add(nhan);
            dau.Controls.Add(lblTenDiem);
            dau.Controls.Add(dong);

            Panel chan = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 142,
                BackColor = Color.White,
                Padding = new Padding(14, 9, 14, 9)
            };
            ttDvBtnHoSo = new Button
            {
                Text = "▣  HỒ SƠ ĐẦY ĐỦ",
                Left = 14,
                Top = 9,
                Height = 38,
                Enabled = false
            };
            TtDv_DatKieuNut(ttDvBtnHoSo, TtDv_Mau(12, 178, 152));
            ttDvBtnHoSo.Click += (s, e) =>
            {
                if (logoDangHienHoSoMoi)
                {
                    // The cua lop logo: mo hop thoai dinh vi lai tru so.
                    string maDinhVi = maDonViDangChon;
                    if (!string.IsNullOrWhiteSpace(maDinhVi))
                        DinhVi_MoHopThoai(maDinhVi);
                    return;
                }
                string ma = maDonViDangChon;
                if (string.IsNullOrWhiteSpace(ma)) return;
                AnKhungThongTinDiem();
                MoXemChiTiet(ma);
            };
            ttDvBtnSaoChep = new Button
            {
                Text = "▤  SAO CHÉP TỌA ĐỘ",
                Top = 9,
                Height = 38,
                Enabled = false
            };
            TtDv_DatKieuNut(ttDvBtnSaoChep, TtDv_Mau(88, 113, 234));
            ttDvBtnSaoChep.Click += (s, e) => TtDv_SaoChepToaDo();
            ttDvBtnDinhTam = new Button
            {
                Text = "⌖  ĐƯA ĐIỂM VỀ GIỮA BẢN ĐỒ",
                Left = 14,
                Top = 54,
                Height = 35,
                Enabled = false
            };
            TtDv_DatKieuNut(ttDvBtnDinhTam, TtDv_Mau(45, 145, 194));
            ttDvBtnDinhTam.Click += (s, e) =>
            {
                if (logoDangHienHoSoMoi) Logo_DinhTamDiemDangChon();
                else TtDv_DinhTamDonVi();
            };
            chan.Controls.Add(ttDvBtnHoSo);
            chan.Controls.Add(ttDvBtnSaoChep);
            chan.Controls.Add(ttDvBtnDinhTam);

            // Hang nut cong cu GIS cho don vi dang xem.
            Button[] ccNutThe =
            {
                new Button { Text = "⟲  Lịch sử vị trí", Top = 96, Height = 33 },
                new Button { Text = "▱  Khuôn viên", Top = 96, Height = 33 },
                new Button { Text = "⎙  Phiếu hồ sơ", Top = 96, Height = 33 }
            };
            Color[] ccMauThe = { TtDv_Mau(209, 127, 35), TtDv_Mau(0, 140, 112), TtDv_Mau(52, 72, 98) };
            for (int i = 0; i < ccNutThe.Length; i++)
            {
                TtDv_DatKieuNut(ccNutThe[i], ccMauThe[i]);
                ccNutThe[i].Font = new Font("Segoe UI", 8.4F, FontStyle.Bold);
                chan.Controls.Add(ccNutThe[i]);
            }
            meoThongTinDiem.SetToolTip(ccNutThe[0], "Xem các lần định vị trụ sở (thời gian, phương pháp, sai số) và vẽ đường dịch chuyển.");
            meoThongTinDiem.SetToolTip(ccNutThe[1], "Số hóa đa giác khuôn viên doanh trại trên ảnh vệ tinh (vai trò Quản trị).");
            meoThongTinDiem.SetToolTip(ccNutThe[2], "Phiếu hồ sơ A4: thông tin, tọa độ WGS84/UTM/VN-2000, ảnh vị trí, lịch sử định vị · in hoặc PDF.");
            ccNutThe[0].Click += (s, e) =>
            {
                string ma = maDonViDangChon;
                if (!string.IsNullOrWhiteSpace(ma)) CongCu_MoLichSu(ma, lblTenDiem.Text);
            };
            ccNutThe[1].Click += (s, e) =>
            {
                string ma = maDonViDangChon;
                if (string.IsNullOrWhiteSpace(ma)) return;
                if (!CoQuyen(ThaoTacDonViQuanDoi.Sua, ma))
                {
                    MessageBox.Show(this, "Vai trò hiện tại không được sửa dữ liệu không gian.", "Khuôn viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string ten = lblTenDiem.Text;
                AnKhungThongTinDiem();
                CongCu_BatSoHoaKhuonVien(ma, ten);
            };
            ccNutThe[2].Click += (s, e) => CongCu_InPhieuHoSo(maDonViDangChon);

            flpThongTinDiem = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                WrapContents = false,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                BackColor = TtDv_Mau(249, 252, 255),
                Padding = new Padding(14, 12, 10, 12)
            };
            pnlThongTinDiem.Controls.Add(flpThongTinDiem);
            pnlThongTinDiem.Controls.Add(chan);
            pnlThongTinDiem.Controls.Add(dau);
            Action canChinh = () =>
            {
                dong.Left = Math.Max(0, dau.ClientSize.Width - 42);
                lblTenDiem.Width = Math.Max(55, dau.ClientSize.Width - 38);
                int rong = Math.Max(100, chan.ClientSize.Width - 28);
                ttDvBtnHoSo.Width = Math.Max(35, (rong - 9) / 2);
                ttDvBtnSaoChep.Left = ttDvBtnHoSo.Right + 9;
                ttDvBtnSaoChep.Width = Math.Max(35, rong - ttDvBtnHoSo.Width - 9);
                ttDvBtnDinhTam.Width = rong;
                int rong3 = Math.Max(30, (rong - 14) / 3);
                for (int i = 0; i < ccNutThe.Length; i++)
                {
                    ccNutThe[i].Left = 14 + i * (rong3 + 7);
                    ccNutThe[i].Width = i == 2 ? rong - 2 * (rong3 + 7) : rong3;
                }
                TtDv_CanChinhDongThongTin();
            };
            pnlThongTinDiem.Resize += (s, e) => canChinh();
            flpThongTinDiem.ClientSizeChanged +=
                (s, e) => TtDv_CanChinhDongThongTin();
            ttDvCotHoSo.Controls.Add(pnlThongTinDiem);
            pnlMapCard.Controls.Add(ttDvCotHoSo);
            canChinh();
            GioiHanKhungThongTin();
        }

        private void HienThiThongTinDiem(DonViQuanDoi donVi, int x, int y)
        {
            if (donVi == null) return;
            if (pnlThongTinDiem == null || pnlThongTinDiem.IsDisposed)
                TaoKhungThongTinDiem();
            ttDvHoSoDangHienThi = donVi;
            maDonViDangChon = donVi.MaDonVi;
            lblTenDiem.Text = GiaTriHienThi(donVi.TenDonVi);
            meoThongTinDiem.SetToolTip(lblTenDiem, GiaTriHienThi(donVi.TenDonVi));
            bool coWgs = DonViQuanDoiBLL.CoToaDoWgs84(donVi);
            bool coDiemMoi = logoDangHienHoSoMoi;
            bool coDiem = coDiemMoi || ttDvDiemDangHienThi != null;
            ttDvBtnHoSo.Text = coDiemMoi ? "⌖  ĐỊNH VỊ LẠI TRỤ SỞ" : "▣  HỒ SƠ ĐẦY ĐỦ";
            ttDvBtnHoSo.Enabled = !string.IsNullOrWhiteSpace(donVi.MaDonVi) &&
                (!coDiemMoi || CoQuyen(ThaoTacDonViQuanDoi.Sua, donVi.MaDonVi));
            ttDvBtnSaoChep.Enabled = coWgs;
            ttDvBtnDinhTam.Enabled = coDiem;
            flpThongTinDiem.SuspendLayout();
            try
            {
                TtDv_XoaCacDongThongTin();
                TtDv_ThemTrangThaiHoSo(donVi, coWgs);
                TtDv_ThemTieuDeNhom("THÔNG TIN CƠ BẢN");
                TtDv_ThemHaiThe("Mã đơn vị", donVi.MaDonVi, 0,
                    "Loại đơn vị", donVi.LoaiDonVi, 1);
                TtDv_ThemTheRong("Tên đầy đủ", donVi.TenDonVi, 0);
                TtDv_ThemHaiThe("Tên viết tắt", donVi.TenVietTat, 2,
                    "Quân / Binh chủng", donVi.QuanBinhChung, 3);
                TtDv_ThemTheRong("Đơn vị cấp trên", donVi.DonViCapTren, 4);
                TtDv_ThemTheRong("Trạng thái", donVi.TrangThaiSSCD, 3);
                TtDv_ThemTieuDeNhom("ĐỊA BÀN VÀ ĐỊA CHỈ");
                TtDv_ThemTheRong("Phường / xã", donVi.DiaBanHienThi, 0);
                TtDv_ThemTheRong("Địa chỉ", donVi.DiaChi, 1);
                if (!coDiemMoi)
                {
                    TtDv_ThemTieuDeNhom("LÃNH ĐẠO VÀ CHỈ HUY");
                    TtDv_ThemTheRong("Chỉ huy trưởng / Tư lệnh", donVi.ChiHuyTruong, 1);
                    TtDv_ThemTheRong("Chính ủy / Chính trị viên", donVi.ChinhUy, 2);
                }
                TtDv_ThemTieuDeNhom("VỊ TRÍ KHÔNG GIAN");
                TtDv_ThemHaiThe("Kinh độ (WGS84)", coWgs
                    ? donVi.KinhDo.ToString("F6", CultureInfo.InvariantCulture) + "° E"
                    : "Chưa cập nhật", 1,
                    "Vĩ độ (WGS84)", coWgs
                    ? donVi.ViDo.ToString("F6", CultureInfo.InvariantCulture) + "° N"
                    : "Chưa cập nhật", 2);
                // Logo co the duoc ve lech (gian cach khi trung diem):
                // X/Y hien thi phai la vi tri GOC tinh tu kinh/vi do.
                double xHienThi = coDiemMoi ? logoDiemChonX : ttDvDiemDangHienThi.X;
                double yHienThi = coDiemMoi ? logoDiemChonY : ttDvDiemDangHienThi.Y;
                double xGoc, yGoc;
                if (coDiemMoi && coWgs &&
                    Native_Wgs84SangToaDoBanDo(donVi.KinhDo, donVi.ViDo,
                        out xGoc, out yGoc))
                {
                    xHienThi = xGoc;
                    yHienThi = yGoc;
                }
                TtDv_ThemHaiThe("X (hệ bản đồ)", coDiem
                    ? xHienThi.ToString("F2", CultureInfo.InvariantCulture)
                    : "Chưa cập nhật", 0,
                    "Y (hệ bản đồ)", coDiem
                    ? yHienThi.ToString("F2", CultureInfo.InvariantCulture)
                    : "Chưa cập nhật", 3);
                if (coDiemMoi)
                {
                    TtDv_ThemTieuDeNhom("NGUỒN DỮ LIỆU");
                    TtDv_ThemTheRong("Nguồn hồ sơ", donVi.NguonDuLieu, 1);
                    // GhiChu do lop logo gan: phuong phap + do chinh xac
                    // tu CSDL hop nhat 2026.
                    TtDv_ThemTheRong("Tính chất vị trí",
                        !string.IsNullOrWhiteSpace(donVi.GhiChu)
                            ? donVi.GhiChu
                            : logoDiemChonLaDaiDien
                                ? "Điểm đại diện hành chính; KHÔNG PHẢI tọa độ trụ sở."
                                : "Vị trí điểm hồ sơ GIS trên bản đồ.", 4);
                }
            }
            finally { flpThongTinDiem.ResumeLayout(true); }
            GioiHanKhungThongTin();
            pnlThongTinDiem.Visible = true;
            ttDvCotHoSo.Visible = true;
            ttDvCotHoSo.BringToFront(); // SUA LOI: Map canvas che panel.
            pnlThongTinDiem.BringToFront();
            if (pnlCheDoBanDo != null) pnlCheDoBanDo.BringToFront();
            if (pnlMapTools != null) pnlMapTools.BringToFront();
            meoThongTinDiem.Active = true;
            statusGeneral.Text = "● Đang xem đơn vị: " + GiaTriHienThi(donVi.TenDonVi);
        }
    }
}
