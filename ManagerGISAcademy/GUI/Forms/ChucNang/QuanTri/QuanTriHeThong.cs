using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.GUI.Views;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Phan he "Quan tri he thong": nut menu thu 8, doi vai tro phien
    /// (SET ROLE -> RLS + GRANT), thong bao khi thay doi bi xep hang duyet.
    /// </summary>
    public partial class FormMain
    {
        private Button btnNavAdmin;
        private bool quanTriDangDoiVaiTro;

        private void QuanTri_TaoNutDieuHuong()
        {
            if (btnNavAdmin != null || flowNavigation == null) return;
            btnNavAdmin = new Button
            {
                Name = "btnNavAdmin",
                BackColor = Color.FromArgb(9, 17, 28),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(203, 213, 225),
                Margin = new Padding(0, 0, 0, 6),
                Size = new Size(210, 44),
                Text = "  Quản trị hệ thống",
                TextAlign = ContentAlignment.MiddleLeft,
                UseVisualStyleBackColor = false,
                TabIndex = 7
            };
            btnNavAdmin.FlatAppearance.BorderSize = 0;
            flowNavigation.Controls.Add(btnNavAdmin);
            int viTri = flowNavigation.Controls.GetChildIndex(btnNavValidate);
            flowNavigation.Controls.SetChildIndex(btnNavAdmin, viTri + 1);
            btnNavAdmin.Click += (s, e) => ChonPhanHe(btnNavAdmin, "Quản trị hệ thống");
        }

        /// <summary>Doi vai tro PostgreSQL cho moi ket noi moi va nap lai quyen + du lieu.</summary>
        private async void QuanTri_DoiVaiTro(string vaiTro)
        {
            if (quanTriDangDoiVaiTro) return;
            quanTriDangDoiVaiTro = true;
            string cu = KetNoiCoSoDuLieu.VaiTroPhien;
            try
            {
                KetNoiCoSoDuLieu.VaiTroPhien = vaiTro;
                statusGeneral.Text = "● Đang chuyển vai trò " + TruyXuatQuanTri.TenVaiTro(vaiTro) + "…";
                Tuple<ChinhSachQuyenPostGIS, NghiepVuDonVi> kq = await Task.Run(() =>
                {
                    ChinhSachQuyenPostGIS cs = ChinhSachQuyenPostGIS.DocQuyenTuCoSoDuLieu();
                    if (!cs.ChoPhepXem)
                        throw new InvalidOperationException("Vai trò này không có quyền SELECT trên hồ sơ đơn vị.");
                    NghiepVuDonVi nv = new NghiepVuDonVi(
                        new TruyXuatDonVi(KetNoiCoSoDuLieu.LayChuoiKetNoi()), cs);
                    return Tuple.Create(cs, nv);
                });
                if (IsDisposed) return;

                GanNghiepVuDonVi(kq.Item2, kq.Item1);
                QuanTri_NapLaiSauThayDoi();

                GiaoDienQuanTri trang = TimManHinh<GiaoDienQuanTri>("Quản trị hệ thống");
                if (trang != null && !trang.IsDisposed) trang.LamMoiVaiTro();

                statusGeneral.Text = "● Vai trò phiên: " + TruyXuatQuanTri.TenVaiTro(vaiTro) +
                    " · Thêm " + (kq.Item1.ChoPhepThem ? "✓" : "✕") +
                    " · Sửa " + (kq.Item1.ChoPhepSua ? "✓" : "✕") +
                    " · Xóa " + (kq.Item1.ChoPhepXoa ? "✓" : "✕") +
                    (vaiTro == "gis_bien_tap" ? " · thay đổi sẽ chờ quản trị duyệt" : "");
                GhiNhatKyBanDo("QUAN_TRI_DOI_VAI_TRO | " + (vaiTro ?? "(goc)"));
            }
            catch (Exception ex)
            {
                KetNoiCoSoDuLieu.VaiTroPhien = cu;
                MessageBox.Show(this, ex.Message, "Không chuyển được vai trò",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                statusGeneral.Text = "● Giữ nguyên vai trò " + TruyXuatQuanTri.TenVaiTro(cu ?? "gis_quan_tri");
            }
            finally
            {
                quanTriDangDoiVaiTro = false;
            }
        }

        /// <summary>Nap lai logo / vong sai so / cac trang sau khi doi vai tro hoac duyet.</summary>
        private void QuanTri_NapLaiSauThayDoi()
        {
            try
            {
                AnKhungThongTinDiem();
                Native_XoaLopPhu169();
                Native_TaiMoiDuLieuPhanHe();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("QUAN_TRI_NAP_LAI_FAILED | " + ex.Message);
            }
        }

        /// <summary>Vai tro Bien tap: bao ro thay doi dang CHO DUYET (CSDL chua doi).</summary>
        private void QuanTri_ThongBaoNeuChoDuyet(string thaoTac)
        {
            if (KetNoiCoSoDuLieu.VaiTroPhien != "gis_bien_tap") return;
            statusGeneral.Text = "● " + thaoTac + ": đã gửi yêu cầu, chờ quản trị duyệt.";
            MessageBox.Show(this,
                thaoTac + " đã được gửi vào HÀNG ĐỢI DUYỆT.\n\n" +
                "Dữ liệu gốc chưa thay đổi. Quản trị viên duyệt tại:\n" +
                "Quản trị hệ thống → Phân quyền & duyệt.",
                "Đang chờ duyệt", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>8 thay doi gan nhat tu nhat ky CSDL cho trang Tong quan (rong neu vai tro khong duoc doc).</summary>
        private System.Collections.Generic.List<HoatDongTongQuanItem> TongQuan_HoatDongGanDay(
            System.Collections.Generic.IList<ManagerGISAcademy.DTO.DonViQuanDoi> ds)
        {
            var kq = new System.Collections.Generic.List<HoatDongTongQuanItem>();
            try
            {
                var ten = new System.Collections.Generic.Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (ds != null)
                    foreach (var d in ds)
                        if (!string.IsNullOrEmpty(d.MaDonVi) && !ten.ContainsKey(d.MaDonVi)) ten[d.MaDonVi] = d.TenDonVi;
                foreach (DongNhatKy n in new TruyXuatQuanTri().LayNhatKy(8))
                {
                    string bang;
                    switch (n.Bang)
                    {
                        case "don_vi_quan_doi": bang = "hồ sơ đơn vị"; break;
                        case "vi_tri_don_vi": bang = "vị trí trụ sở"; break;
                        case "ho_so_nghiep_vu": bang = "hồ sơ nghiệp vụ"; break;
                        case "khu_vuc_don_vi": bang = "khuôn viên"; break;
                        default: bang = n.Bang; break;
                    }
                    string tenDv;
                    string doiTuong = string.IsNullOrEmpty(n.MaDonVi) ? "" :
                        " · " + (ten.TryGetValue(n.MaDonVi, out tenDv) && !string.IsNullOrEmpty(tenDv) ? tenDv : n.MaDonVi);
                    kq.Add(new HoatDongTongQuanItem
                    {
                        ThoiDiem = n.ThoiDiem,
                        LoaiHoatDong = n.ThaoTac + " " + bang,
                        NoiDung = (string.IsNullOrEmpty(n.TaiKhoan) ? "" : n.TaiKhoan) + doiTuong
                    });
                }
            }
            catch (Exception ex) { GhiNhatKyBanDo("OVERVIEW_ACTIVITY_FAILED | " + ex.Message); }
            return kq;
        }
    }
}