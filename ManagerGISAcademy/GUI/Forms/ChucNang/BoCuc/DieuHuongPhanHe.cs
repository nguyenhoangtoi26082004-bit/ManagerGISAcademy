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
        private void DangKySuKien()
        {
            Load += FormMain_Load;
            Shown += FormMain_Shown;

            btnNavOverview.Click += (s, e) =>
                ChonPhanHe(btnNavOverview, "Tổng quan");

            btnNavMap.Click += (s, e) =>
                ChonPhanHe(btnNavMap, "Bản đồ GIS");

            btnNavSearch.Click += (s, e) =>
                MoTraCuu(null);

            btnNavManage.Click += (s, e) =>
                ChonPhanHe(btnNavManage, "Quản lý dữ liệu");

            btnNavAnalysis.Click += (s, e) =>
                ChonPhanHe(btnNavAnalysis, "Phân tích không gian");

            btnNavReports.Click += (s, e) =>
                ChonPhanHe(btnNavReports, "Thống kê, báo cáo");

            btnNavValidate.Click += (s, e) =>
                ChonPhanHe(btnNavValidate, "Kiểm tra dữ liệu");

            // Muc menu thu 8 (tao bang ma, khong sua Designer).
            QuanTri_TaoNutDieuHuong();

            btnQuickSearch.Click += (s, e) => Header_TimNgay();

            btnAddUnit.Click += (s, e) =>
                MoThemDonVi();

            txtQuickSearch.KeyDown += TxtQuickSearch_KeyDown;
            txtQuickSearch.TextChanged += Header_TextChanged;
            txtQuickSearch.VisibleChanged += (s, e) =>
            {
                if (!txtQuickSearch.Visible) Header_DongGoiY();
            };
            Move += (s, e) => Header_DongGoiY();
            Resize += (s, e) => Header_DongGoiY();
            Deactivate += (s, e) => Header_DongGoiY();

            btnZoomIn.Click += (s, e) =>
                ThucHienBanDo(
                    "Phóng to",
                    () => ketNoiMapWinGIS.PhongTo());

            btnZoomOut.Click += (s, e) =>
                ThucHienBanDo(
                    "Thu nhỏ",
                    () => ketNoiMapWinGIS.ThuNho());

            btnFullExtent.Click += (s, e) =>
                ThucHienBanDo(
                    "Toàn cảnh",
                    () => ketNoiMapWinGIS.PhongToanCanh());

            btnMapLayers.Click += (s, e) =>
                DoiHienThiBangLop();

            btnNapLop.Click += (s, e) =>
                NapLopTuTep();

            pnlMapCanvas.Resize += (s, e) =>
                CanGiuaThongBao();
        }

        private void ChonPhanHe(Button nut, string ten)
        {
            if (IsDisposed || Disposing) return;
            // Quan tri he thong: bat buoc dang nhap bang tai khoan Quan tri.
            if (ten == "Quản trị hệ thống" && !TaiKhoan_KiemTraQuyenQuanTri()) return;
            Header_DongGoiY();

            // Dong trang chi tiet/them-sua dang mo truoc khi chuyen tab.
            Native_DongTrangHoSo(false);

            if (nutDangChon != null)
            {
                nutDangChon.BackColor = mauMenu;
                nutDangChon.ForeColor = Color.FromArgb(207, 220, 232);
            }
            nutDangChon = nut;
            if (nutDangChon != null)
            {
                nutDangChon.BackColor = mauDangChon;
                nutDangChon.ForeColor = Color.White;
            }

            phanHeHienTai = ten;
            lblPageTitle.Text = ten;
            lblStageBadge.Text = "HỆ THỐNG GIS";

            // GD5.7.4: Chi ban do GIS giu thanh tim nhanh phia tren.
            // Sau phan he nghiep vu nhan toan bo chieu cao cua Header.
            Main_CapNhatBoCucToanTrang(
                string.Equals(ten, "Bản đồ GIS", StringComparison.Ordinal));

            if (ten != "Bản đồ GIS")
                Main_AnTieuDePhanTichBiTrung();
            else
            {
                if (tblWorkspaceHeading != null && !tblWorkspaceHeading.IsDisposed)
                {
                    tblWorkspaceHeading.Height = 84;
                    tblWorkspaceHeading.Visible = true;
                }
                if (lblPageTitle != null) lblPageTitle.Visible = true;
                if (lblPageSubtitle != null) lblPageSubtitle.Visible = true;
                if (lblStageBadge != null) lblStageBadge.Visible = true;
            }

            tblContent.SuspendLayout();
            try
            {
                if (ten == "Bản đồ GIS")
                {
                    tblContent.Controls.Clear();
                    tblContent.ColumnStyles[0].Width = 71F;
                    tblContent.ColumnStyles[1].Width = 29F;
                    pnlRightCard.Visible = true;
                    tblContent.Controls.Add(pnlMapCard, 0, 0);
                    tblContent.Controls.Add(pnlRightCard, 1, 0);
                    lblPageSubtitle.Text =
                        "Hiển thị bản đồ, quản lý lớp và thao tác không gian.";
                    CapNhatNhanBanDo();
                }
                else
                {
                    MainNav_ChuanBiKhungWeb();
                    HienThiNoiDung(ten);
                }
            }
            finally { tblContent.ResumeLayout(true); }

            if (ten != "Bản đồ GIS")
            {
                Main_AnTieuDePhanTichBiTrung();
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (!IsDisposed && !Disposing &&
                            phanHeHienTai != "Bản đồ GIS")
                            Main_AnTieuDePhanTichBiTrung();
                    }));
                }
            }
            statusGeneral.Text = "● Đang xem: " + ten;
            TaiKhoan_CanViTri();
            if (IsHandleCreated) BeginInvoke(new MethodInvoker(TaiKhoan_CanViTri));
        }

        private bool MainNav_MoWebTrucTiepLanDau(string ten)
        {
            return false;
        }

        private bool MainNav_HienWebDangKhoiTao(string ten)
        {
            return false;
        }

        private void MainNav_TheoDoiKhoiTaoWeb()
        {
            if (mainNavTheoDoi == null)
            {
                mainNavTheoDoi = new System.Windows.Forms.Timer();
                mainNavTheoDoi.Interval = 150;
                mainNavTheoDoi.Tick += MainNav_TickKhoiTaoWeb;
            }

            mainNavTheoDoi.Start();
        }

        private void MainNav_TickKhoiTaoWeb(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing ||
                phanHeHienTai == "Bản đồ GIS")
            {
                mainNavTheoDoi.Stop();
                return;
            }

            string ten = phanHeHienTai;

            if (!LaPhanHeWebHoatDong(ten))
            {
                mainNavTheoDoi.Stop();

                if (tblContent.Controls.Contains(pnlPhanHeHost))
                    HienThiNoiDung(ten);

                return;
            }

            if (ChuyenNgaySangWebDaSanSang(ten))
            {
                mainNavTheoDoi.Stop();
                return;
            }
        }

        private void MainNav_ChuanBiKhungWeb()
        {
            if (tblContent.Controls.Contains(pnlPhanHeHost))
                return;

            tblContent.SuspendLayout();

            try
            {
                tblContent.Controls.Clear();

                tblContent.ColumnStyles[0].Width = 100F;
                tblContent.ColumnStyles[1].Width = 0F;

                pnlRightCard.Visible = false;

                tblContent.Controls.Add(pnlPhanHeHost, 0, 0);
                tblContent.SetColumnSpan(pnlPhanHeHost, 2);
            }
            finally
            {
                tblContent.ResumeLayout(true);
            }
        }

        private void MainNav_AnChuDangMoWeb(
            object sender,
            ControlEventArgs e)
        {
            if (e.Control == null || e.Control.IsDisposed)
                return;

            MainNav_AnChuCho(e.Control);
        }

        private void MainNav_AnChuCho(Control control)
        {
            Label nhan = control as Label;

            if (nhan != null &&
                !string.IsNullOrEmpty(nhan.Text) &&
                nhan.Text.IndexOf(
                    "mở giao diện Web",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                nhan.Text = string.Empty;
                nhan.Visible = false;
            }

            foreach (Control con in control.Controls)
                MainNav_AnChuCho(con);
        }

        private void MainNav_XoaChuChoTrongHost()
        {
            foreach (Control control in pnlPhanHeHost.Controls)
                MainNav_AnChuCho(control);
        }

        private void HienThiNoiDung(string ten)
        {
            Control view;
            if (!cacManHinh.TryGetValue(ten, out view) ||
                view == null || view.IsDisposed)
            {
                switch (ten)
                {
                    case "Tổng quan":
                        {
                            GiaoDienTongQuan v = new GiaoDienTongQuan();
                            v.YeuCauThemDonVi += (a, b) => MoThemDonVi();
                            v.YeuCauMoPhanHe +=
                                (a, b) => DieuHuongTuTongQuan(b.PhanHe);
                            v.YeuCauTaiLaiTongQuan +=
                                (a, b) => Native_TaiMoiDuLieuPhanHe();
                            v.YeuCauKiemTraPostGIS += (a, b) =>
                                MessageBox.Show(this, lblHeaderDbStatus.Text,
                                    "Trạng thái PostGIS", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            view = v;
                            break;
                        }
                    case "Tra cứu đơn vị":
                        {
                            GiaoDienTraCuuDonVi v = new GiaoDienTraCuuDonVi();
                            v.YeuCauDinhViDonVi += ma => DinhViDonVi(ma);
                            v.YeuCauXemChiTietDonVi += ma => MoXemChiTiet(ma);
                            v.YeuCauLamMoi += () => Native_TaiMoiDuLieuPhanHe();
                            view = v;
                            break;
                        }
                    case "Quản lý dữ liệu":
                        {
                            UcQuanLyDuLieuNative v = new UcQuanLyDuLieuNative();
                            v.YeuCauThem += () => MoThemDonVi();
                            v.YeuCauTaiLai += () => Native_TaiMoiDuLieuPhanHe();
                            v.YeuCauSua += ma => MoSuaDonVi(ma);
                            v.YeuCauXoa += ma => XoaDonVi(ma);
                            v.YeuCauChiTiet += ma => MoXemChiTiet(ma);
                            v.YeuCauDinhVi += ma => DinhViDonVi(ma);
                            v.YeuCauNhatKy += ma => MoNhatKyThayDoi(ma);
                            v.YeuCauCongCuGis += (lenh, ts) =>
                            {
                                try
                                {
                                    switch (lenh)
                                    {
                                        case "nhap_toa_do": CongCu_MoNhapToaDo(); break;
                                        case "xuat_gis": CongCu_MoXuatDuLieu(); break;
                                        case "phieu_ho_so": CongCu_InPhieuHoSo(ts); break;
                                        case "lich_su":
                                            string[] p = (ts ?? "").Split(new[] { '|' }, 2);
                                            CongCu_MoLichSu(p[0], p.Length > 1 ? p[1] : p[0]);
                                            break;
                                    }
                                }
                                catch (Exception exCc) { CongCu_BaoLoi(exCc); }
                            };
                            view = v;
                            break;
                        }
                    case "Phân tích không gian":
                        {
                            GiaoDienPhanTich v = new GiaoDienPhanTich();
                            v.Tag = "v2";
                            GanXuLyPhanTichGon(v);
                            view = v;
                            break;
                        }
                    case "Thống kê, báo cáo":
                        {
                            UcThongKeBaoCaoNative v = new UcThongKeBaoCaoNative();
                            v.Tag = "v2";
                            v.YeuCauLamMoi += () => Native_TaiMoiDuLieuPhanHe();
                            view = v;
                            break;
                        }
                    case "Kiểm tra dữ liệu":
                        {
                            UcKiemTraDuLieuNative v = new UcKiemTraDuLieuNative();
                            v.Tag = "v2";
                            Native_GanKiemTraDuLieu(v);
                            view = v;
                            break;
                        }
                    case "Quản trị hệ thống":
                        {
                            GiaoDienQuanTri v = new GiaoDienQuanTri();
                            v.YeuCauDoiVaiTro += vt => QuanTri_DoiVaiTro(vt);
                            v.DaDuyetThanhCong += () => QuanTri_NapLaiSauThayDoi();
                            view = v;
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("ten",
                            "Không tìm thấy phân hệ: " + ten);
                }
                cacManHinh[ten] = view;
            }

            // Cache UserControl: chi tao 1 lan, khong tao lai moi lan chuyen tab.
            pnlPhanHeHost.SuspendLayout();
            try
            {
                pnlPhanHeHost.Controls.Clear();
                Form oldForm = view as Form;
                if (oldForm != null)
                {
                    oldForm.TopLevel = false;
                    oldForm.FormBorderStyle = FormBorderStyle.None;
                    oldForm.Dock = DockStyle.Fill;
                    pnlPhanHeHost.Controls.Add(oldForm);
                    oldForm.Show();
                }
                else
                {
                    view.Dock = DockStyle.Fill;
                    pnlPhanHeHost.Controls.Add(view);
                    view.Show();
                }
            }
            finally { pnlPhanHeHost.ResumeLayout(true); }

            TaiLaiDuLieuPhanHe();
        }

        private bool LaPhanHeWebHoatDong(string ten)
        {
            // Giai doan chuyen doi: giu file HTML cu de rollback,
            // nhung FormMain khong mo HTML cho 7 phan he nua.
            return false;
        }

        private bool ChuyenNgaySangWebDaSanSang(string ten)
        {
            return false;
        }

        private void DieuHuongTuTongQuan(PhanHeDieuHuong phanHe)
        {
            switch (phanHe)
            {
                case PhanHeDieuHuong.TongQuan:
                    ChonPhanHe(btnNavOverview, "Tổng quan");
                    break;

                case PhanHeDieuHuong.BanDoGIS:
                    ChonPhanHe(btnNavMap, "Bản đồ GIS");
                    break;

                case PhanHeDieuHuong.TraCuuDonVi:
                    MoTraCuu(null);
                    break;

                case PhanHeDieuHuong.QuanLyDuLieu:
                    ChonPhanHe(btnNavManage, "Quản lý dữ liệu");
                    break;

                case PhanHeDieuHuong.PhanTichKhongGian:
                    ChonPhanHe(btnNavAnalysis, "Phân tích không gian");
                    break;

                case PhanHeDieuHuong.ThongKeBaoCao:
                    ChonPhanHe(btnNavReports, "Thống kê, báo cáo");
                    break;

                case PhanHeDieuHuong.KiemTraDuLieu:
                    ChonPhanHe(btnNavValidate, "Kiểm tra dữ liệu");
                    break;
            }
        }

        private void MoTraCuu(string tuKhoa)
        {
            if (tuKhoa != null && string.IsNullOrWhiteSpace(tuKhoa))
            {
                txtQuickSearch.Focus();
                return;
            }
            ChonPhanHe(btnNavSearch, "Tra cứu đơn vị");
            GiaoDienTraCuuDonVi v = TimManHinh<GiaoDienTraCuuDonVi>("Tra cứu đơn vị");
            if (v != null && !string.IsNullOrWhiteSpace(tuKhoa))
                v.TimKiemTheoTuKhoa(tuKhoa);
        }
    }
}