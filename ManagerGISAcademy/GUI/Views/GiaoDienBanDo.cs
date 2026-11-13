
using System;
using System.Drawing;
using System.Windows.Forms;

using ManagerGISAcademy.GUI.Controls;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>
    /// Giao dien ban do GIS va tra cuu don vi.
    /// WinForms .NET Framework 4.8.
    ///
    /// - Chi chua AxMap cua FormMain.
    /// - Khong tao ban do thu hai.
    /// - Khong ket noi CSDL truc tiep.
    /// - Su dung thanh tim kiem cua FormMain.
    /// - Giu cac su kien va cong cu hien co.
    /// </summary>
    public class GiaoDienBanDo : UserControl
    {
        // ==================================================
        // 1. CAC PANEL
        // ==================================================

        private readonly Panel pnlDau;
        private readonly Panel pnlNoiDung;
        private readonly Panel pnlKhungBanDo;
        private readonly Panel pnlTrong;

        private readonly Panel pnlThongBaoTrong;
        private readonly Panel pnlChan;

        // ==================================================
        // 2. NHAN THONG TIN
        // ==================================================

        private readonly Label lblTieuDe;
        private readonly Label lblTinhTrang;
        private readonly Label lblBieuTuong;

        private readonly Label lblTieuDeTrong;
        private readonly Label lblHuongDan;

        private readonly Label lblTrangThai;
        private readonly Label lblSoLop;

        // ==================================================
        // 3. CONG CU VA HO SO
        // ==================================================

        private readonly ThanhCongCuBanDo thanhCongCu;

        private readonly BangChiTietDonVi bangChiTiet;

        private Control dieuKhienBanDo;

        private bool daChoPhepCongCu;

        // ==================================================
        // 4. CAC SU KIEN BAN DO
        // ==================================================

        public event EventHandler YeuCauPhongToBanDo;
        public event EventHandler YeuCauThuNhoBanDo;

        public event EventHandler YeuCauToanCanhBanDo;
        public event EventHandler YeuCauDiChuyenBanDo;

        public event EventHandler YeuCauChonDoiTuong;

        public event EventHandler YeuCauDoKhoangCach;
        public event EventHandler YeuCauXoaKetQuaDo;

        public event EventHandler YeuCauTaoVungDem;
        public event EventHandler YeuCauMoBangLop;

        // ==================================================
        // 5. CAC SU KIEN HO SO
        // ==================================================

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauXemHoSo;

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauHienThiDonViTrenBanDo;

        public event EventHandler<SuKienUnitDetailArgs>
            YeuCauChinhSuaDonVi;

        public event EventHandler
            YeuCauDongBangChiTiet;

        // ==================================================
        // 6. KHOI TAO
        // ==================================================

        public GiaoDienBanDo()
        {
            Name = "UcBanDo";

            Dock = DockStyle.Fill;

            MinimumSize = new Size(450, 320);

            BackColor = Color.FromArgb(17, 24, 39);

            ForeColor = Color.White;

            Font = new Font("Segoe UI", 9F);

            SuspendLayout();

            // ==================================================
            // 6.1. THANH TIEU DE
            // ==================================================

            pnlDau = new Panel
            {
                Dock = DockStyle.Top,

                Height = 56,

                BackColor =
                    Color.FromArgb(22, 36, 54)
            };

            lblTieuDe = new Label
            {
                Dock = DockStyle.Fill,

                AutoEllipsis = true,

                Text = "BẢN ĐỒ GIS  ·  TRA CỨU ĐƠN VỊ",

                Padding = new Padding(
                    17, 0, 5, 0),

                TextAlign =
                    ContentAlignment.MiddleLeft,

                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold)
            };

            lblTinhTrang = new Label
            {
                Dock = DockStyle.Right,

                Width = 205,

                Text = "● Chưa nạp bản đồ",

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor =
                    Color.FromArgb(251, 191, 36)
            };

            pnlDau.Controls.Add(lblTieuDe);
            pnlDau.Controls.Add(lblTinhTrang);

            // ==================================================
            // 6.2. THANH TRANG THAI
            // ==================================================

            pnlChan = new Panel
            {
                Dock = DockStyle.Bottom,

                Height = 35,

                BackColor =
                    Color.FromArgb(15, 23, 42)
            };

            lblSoLop = new Label
            {
                Dock = DockStyle.Right,

                Width = 135,

                Text = "Lớp dữ liệu: 0",

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor =
                    Color.FromArgb(148, 163, 184)
            };

            lblTrangThai = new Label
            {
                Dock = DockStyle.Fill,

                AutoEllipsis = true,

                Text = "Chưa gắn điều khiển MapWinGIS.",

                Padding =
                    new Padding(13, 0, 4, 0),

                TextAlign =
                    ContentAlignment.MiddleLeft,

                ForeColor =
                    Color.FromArgb(203, 213, 225)
            };

            pnlChan.Controls.Add(lblTrangThai);
            pnlChan.Controls.Add(lblSoLop);

            // ==================================================
            // 6.3. VUNG NOI DUNG
            // ==================================================

            pnlNoiDung = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor =
                    Color.FromArgb(17, 24, 39)
            };

            pnlKhungBanDo = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor =
                    Color.FromArgb(20, 34, 50)
            };

            pnlTrong = new Panel
            {
                Dock = DockStyle.Fill,

                BackColor =
                    Color.FromArgb(20, 34, 50)
            };

            // ==================================================
            // 6.4. THONG BAO KHI CHUA CO BAN DO
            // ==================================================

            pnlThongBaoTrong = new Panel
            {
                Size = new Size(390, 172),

                BackColor =
                    Color.FromArgb(28, 47, 66)
            };

            lblBieuTuong = new Label
            {
                Dock = DockStyle.Top,

                Height = 64,

                Text = "GIS",

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor =
                    Color.FromArgb(110, 231, 183),

                Font = new Font(
                    "Segoe UI",
                    25F,
                    FontStyle.Bold)
            };

            lblTieuDeTrong = new Label
            {
                Dock = DockStyle.Top,

                Height = 37,

                Text = "CHƯA CÓ BẢN ĐỒ",

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold)
            };

            lblHuongDan = new Label
            {
                Dock = DockStyle.Fill,

                Text =
                    "Bản đồ GIS và thanh tìm kiếm " +
                    "sẽ được khởi tạo từ FormMain.",

                Padding = new Padding(
                    18, 0, 18, 8),

                TextAlign =
                    ContentAlignment.MiddleCenter,

                ForeColor =
                    Color.FromArgb(170, 188, 201)
            };

            pnlThongBaoTrong.Controls.Add(
                lblHuongDan);

            pnlThongBaoTrong.Controls.Add(
                lblTieuDeTrong);

            pnlThongBaoTrong.Controls.Add(
                lblBieuTuong);

            pnlTrong.Controls.Add(
                pnlThongBaoTrong);

            pnlTrong.Resize += (s, e) =>
                CanGiuaThongBaoTrong();

            pnlKhungBanDo.Controls.Add(pnlTrong);

            // ==================================================
            // 6.5. THANH CONG CU BAN DO
            // ==================================================

            thanhCongCu = new ThanhCongCuBanDo
            {
                Name = "thanhCongCu",

                Dock = DockStyle.Left,

                Width = 54,

                ChoPhepTuongTac = false
            };

            // ==================================================
            // 6.6. PANEL CHI TIET DON VI
            // ==================================================

            bangChiTiet = new BangChiTietDonVi
            {
                Name = "bangChiTiet",

                Dock = DockStyle.Right,

                Width = 360,

                Visible = false,

                ChoPhepChinhSua = false
            };

            // ==================================================
            // 6.7. HOAN THIEN BO CUC
            // ==================================================

            pnlNoiDung.Controls.Add(
                pnlKhungBanDo);

            pnlNoiDung.Controls.Add(
                bangChiTiet);

            pnlNoiDung.Controls.Add(
                thanhCongCu);

            Controls.Add(pnlNoiDung);
            Controls.Add(pnlChan);
            Controls.Add(pnlDau);

            DangKySuKien();

            CanGiuaThongBaoTrong();

            ResumeLayout(true);
        }

        // ==================================================
        // 7. DANG KY SU KIEN
        // ==================================================

        private void DangKySuKien()
        {
            thanhCongCu.YeuCauPhongToBanDo +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauPhongToBanDo,
                        "phóng to");

            thanhCongCu.YeuCauThuNhoBanDo +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauThuNhoBanDo,
                        "thu nhỏ");

            thanhCongCu.YeuCauToanCanhBanDo +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauToanCanhBanDo,
                        "toàn cảnh");

            thanhCongCu.YeuCauDiChuyenBanDo +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauDiChuyenBanDo,
                        "di chuyển");

            thanhCongCu.YeuCauChonDoiTuong +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauChonDoiTuong,
                        "chọn đối tượng");

            thanhCongCu.YeuCauDoKhoangCach +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauDoKhoangCach,
                        "đo khoảng cách");

            thanhCongCu.YeuCauXoaKetQuaDo +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauXoaKetQuaDo,
                        "xóa kết quả đo");

            thanhCongCu.YeuCauTaoVungDem +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauTaoVungDem,
                        "tạo vùng đệm");

            thanhCongCu.YeuCauMoBangLop +=
                (s, e) =>
                    ChuyenYeuCauCongCu(
                        YeuCauMoBangLop,
                        "mở bảng lớp");

            bangChiTiet.YeuCauXemHoSo +=
                (s, e) =>
                    ChuyenYeuCauDonVi(
                        YeuCauXemHoSo,
                        e,
                        "xem hồ sơ");

            bangChiTiet.YeuCauHienThiTrenBanDo +=
                (s, e) =>
                    ChuyenYeuCauDonVi(
                        YeuCauHienThiDonViTrenBanDo,
                        e,
                        "định vị đơn vị");

            bangChiTiet.YeuCauChinhSua +=
                (s, e) =>
                    ChuyenYeuCauDonVi(
                        YeuCauChinhSuaDonVi,
                        e,
                        "chỉnh sửa đơn vị");

            bangChiTiet.YeuCauDong += (s, e) =>
            {
                AnChiTietDonVi();

                EventHandler handler =
                    YeuCauDongBangChiTiet;

                if (handler != null)
                {
                    handler(
                        this,
                        EventArgs.Empty);
                }
            };
        }

        // ==================================================
        // 8. TRANG THAI BAN DO
        // ==================================================

        public bool DaGanBanDo
        {
            get
            {
                return dieuKhienBanDo != null &&
                       !dieuKhienBanDo.IsDisposed;
            }
        }

        public Control DieuKhienBanDo
        {
            get { return dieuKhienBanDo; }
        }

        public CheDoMapToolbar CheDoCongCu
        {
            get { return thanhCongCu.CheDoDangChon; }
        }

        // ==================================================
        // 9. GAN MAPWINGIS TU FORMMAIN
        // ==================================================

        public void GanDieuKhienBanDo(
            Control banDo,
            string heQuyChieu = null)
        {
            if (banDo == null)
            {
                throw new ArgumentNullException(
                    "banDo");
            }

            if (banDo.IsDisposed)
            {
                throw new ObjectDisposedException(
                    "banDo");
            }

            if (ReferenceEquals(banDo, this) ||
                ContainsControlCha(banDo))
            {
                throw new ArgumentException(
                    "Không được gắn control cha.",
                    "banDo");
            }

            if (dieuKhienBanDo != null &&
                !ReferenceEquals(
                    dieuKhienBanDo, banDo))
            {
                throw new InvalidOperationException(
                    "Hãy tháo điều khiển bản đồ " +
                    "trước khi thay MapWinGIS.");
            }

            if (banDo.Parent != null &&
                banDo.Parent != pnlKhungBanDo)
            {
                throw new InvalidOperationException(
                    "Hãy tháo AxMap khỏi parent cũ trước.");
            }

            pnlKhungBanDo.SuspendLayout();

            try
            {
                dieuKhienBanDo = banDo;

                banDo.Dock = DockStyle.Fill;

                if (!pnlKhungBanDo.Controls.Contains(banDo))
                {
                    pnlKhungBanDo.Controls.Add(
                        banDo);
                }

                pnlTrong.Visible = false;

                banDo.Visible = true;
                banDo.BringToFront();
            }
            finally
            {
                pnlKhungBanDo.ResumeLayout(true);
            }

            lblTinhTrang.Text =
                "● Đã gắn bản đồ";

            lblTinhTrang.ForeColor =
                Color.FromArgb(110, 231, 183);

            CapNhatTrangThai(
                string.IsNullOrWhiteSpace(heQuyChieu)
                ? "Bản đồ sẵn sàng. Tra cứu đơn vị ở thanh tìm kiếm phía trên."
                : "Đã gắn bản đồ | CRS: " +
                  heQuyChieu.Trim());

            CapNhatChoPhepCongCu();
        }

        // ==================================================
        // 10. THAO DIEU KHIEN BAN DO
        // ==================================================

        public Control ThaoDieuKhienBanDo()
        {
            Control cu = dieuKhienBanDo;

            if (cu != null &&
                !cu.IsDisposed)
            {
                pnlKhungBanDo.Controls.Remove(cu);
            }

            dieuKhienBanDo = null;

            daChoPhepCongCu = false;

            thanhCongCu.ChoPhepTuongTac = false;

            pnlTrong.Visible = true;

            CanGiuaThongBaoTrong();

            lblTinhTrang.Text =
                "● Chưa nạp bản đồ";

            lblTinhTrang.ForeColor =
                Color.FromArgb(251, 191, 36);

            CapNhatSoLop(0);

            CapNhatTrangThai(
                "Chưa gắn điều khiển MapWinGIS.");

            return cu;
        }

        // ==================================================
        // 11. KIEM TRA CONTROL CHA
        // ==================================================

        private bool ContainsControlCha(
            Control control)
        {
            for (Control c = Parent;
                 c != null;
                 c = c.Parent)
            {
                if (ReferenceEquals(c, control))
                {
                    return true;
                }
            }

            return false;
        }

        // ==================================================
        // 12. BAT / TAT CONG CU
        // ==================================================

        public void BatCongCuBanDo(bool choPhep)
        {
            daChoPhepCongCu = choPhep;

            CapNhatChoPhepCongCu();
        }

        private void CapNhatChoPhepCongCu()
        {
            thanhCongCu.ChoPhepTuongTac =
                daChoPhepCongCu &&
                DaGanBanDo;
        }

        public void HienThiNutVungDem(bool hienThi)
        {
            thanhCongCu.HienThiNutVungDem = hienThi;
        }

        public void CapNhatCheDoCongCu(
            CheDoMapToolbar cheDo)
        {
            thanhCongCu.DatCheDoTuBenNgoai(cheDo);
        }

        // ==================================================
        // 13. HIEN THI PANEL CHI TIET
        // ==================================================

        public void HienThiChiTietDonVi(
            ThongTinDonViPanel thongTin)
        {
            if (thongTin == null ||
                string.IsNullOrWhiteSpace(
                    thongTin.MaDonVi))
            {
                AnChiTietDonVi();
                return;
            }

            bangChiTiet.GanDuLieu(thongTin);

            bangChiTiet.Visible = true;

            bangChiTiet.BringToFront();
        }

        public void AnChiTietDonVi()
        {
            bangChiTiet.Visible = false;

            bangChiTiet.XoaDuLieu();
        }

        public void ChoPhepChinhSuaDonVi(bool choPhep)
        {
            bangChiTiet.ChoPhepChinhSua =
                choPhep;
        }

        // ==================================================
        // 14. CAP NHAT THONG TIN
        // ==================================================

        public void CapNhatSoLop(int soLop)
        {
            if (soLop < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "soLop");
            }

            lblSoLop.Text =
                "Lớp dữ liệu: " + soLop;
        }

        public void CapNhatTrangThai(string thongBao)
        {
            lblTrangThai.Text =
                string.IsNullOrWhiteSpace(thongBao)
                ? "Sẵn sàng"
                : thongBao.Trim();
        }

        // ==================================================
        // 15. CHUYEN YEU CAU CONG CU VE FORMMAIN
        // ==================================================

        private void ChuyenYeuCauCongCu(
            EventHandler handler,
            string ten)
        {
            if (handler == null)
            {
                CapNhatTrangThai(
                    "Chưa tích hợp công cụ: " + ten);

                MessageBox.Show(
                    this,
                    "Công cụ " + ten +
                    " chưa được kết nối.",
                    "Chưa tích hợp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            handler(this, EventArgs.Empty);
        }

        // ==================================================
        // 16. CHUYEN YEU CAU HO SO VE FORMMAIN
        // ==================================================

        private void ChuyenYeuCauDonVi(
            EventHandler<SuKienUnitDetailArgs> handler,
            SuKienUnitDetailArgs e,
            string ten)
        {
            if (handler == null)
            {
                CapNhatTrangThai(
                    "Chưa tích hợp: " + ten);

                MessageBox.Show(
                    this,
                    "Chức năng " + ten +
                    " chưa được FormMain đăng ký.",
                    "Chưa tích hợp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            handler(this, e);
        }

        // ==================================================
        // 17. CAN GIUA THONG BAO TRONG
        // ==================================================

        private void CanGiuaThongBaoTrong()
        {
            if (pnlTrong == null ||
                pnlThongBaoTrong == null)
            {
                return;
            }

            pnlThongBaoTrong.Left = Math.Max(
                0,
                (pnlTrong.ClientSize.Width -
                 pnlThongBaoTrong.Width) / 2);

            pnlThongBaoTrong.Top = Math.Max(
                0,
                (pnlTrong.ClientSize.Height -
                 pnlThongBaoTrong.Height) / 2);
        }
    }
}
