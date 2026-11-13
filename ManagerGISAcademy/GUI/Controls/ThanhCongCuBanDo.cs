
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Controls
{
    /// <summary>
    /// Các chế độ thao tác trên bản đồ.
    /// Mô-đun GIS quyết định cách xử lý thực tế.
    /// </summary>
    public enum CheDoMapToolbar
    {
        Khong,
        DiChuyen,
        ChonDoiTuong,
        DoKhoangCach,
        TaoVungDem
    }

    /// <summary>
    /// Thanh công cụ bản đồ dùng chung.
    /// Không truy vấn PostGIS hoặc xử lý MapWinGIS trực tiếp.
    /// Tương thích WinForms .NET Framework và C# 7.3.
    /// Không cần file MapToolbar.Designer.cs.
    /// </summary>
    public class ThanhCongCuBanDo : UserControl
    {
        // ============================================
        // 1. KHAI BÁO CÁC ĐIỀU KHIỂN
        // ============================================

        private readonly Panel pnlNen;

        private readonly Button btnPhongTo;
        private readonly Button btnThuNho;
        private readonly Button btnToanCanh;
        private readonly Button btnDiChuyen;
        private readonly Button btnChonDoiTuong;
        private readonly Button btnDoKhoangCach;
        private readonly Button btnXoaKetQuaDo;
        private readonly Button btnTaoVungDem;
        private readonly Button btnQuanLyLop;

        private readonly ToolTip toolTip;

        // ============================================
        // 2. MÀU SẮC GIAO DIỆN
        // ============================================

        private readonly Color mauNut =
            Color.FromArgb(31, 54, 75);

        private readonly Color mauDuocChon =
            Color.FromArgb(5, 150, 105);

        private CheDoMapToolbar cheDoDangChon =
            CheDoMapToolbar.Khong;

        private bool choPhepTuongTac = true;
        private bool hienThiNutVungDem = true;

        // ============================================
        // 3. CÁC SỰ KIỆN GỬI ĐẾN FORMMAIN
        // ============================================

        public event EventHandler YeuCauPhongToBanDo;

        public event EventHandler YeuCauThuNhoBanDo;

        public event EventHandler YeuCauToanCanhBanDo;

        public event EventHandler YeuCauDiChuyenBanDo;

        public event EventHandler YeuCauChonDoiTuong;

        public event EventHandler YeuCauDoKhoangCach;

        public event EventHandler YeuCauXoaKetQuaDo;

        public event EventHandler YeuCauTaoVungDem;

        public event EventHandler YeuCauMoBangLop;

        // ============================================
        // 4. HÀM KHỞI TẠO
        // ============================================

        public ThanhCongCuBanDo()
        {
            Name = "MapToolbar";

            Size = new Size(54, 409);
            MinimumSize = new Size(54, 409);

            BackColor = Color.FromArgb(23, 37, 55);

            Font = new Font("Segoe UI", 9F);
            TabStop = false;

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            // Chú thích khi di chuột.
            toolTip = new ToolTip();

            toolTip.AutoPopDelay = 7000;
            toolTip.InitialDelay = 350;
            toolTip.ReshowDelay = 100;

            // Nền thanh công cụ.
            pnlNen = new Panel();

            pnlNen.Name = "pnlNen";
            pnlNen.Dock = DockStyle.Fill;

            pnlNen.BackColor =
                Color.FromArgb(23, 37, 55);

            pnlNen.Paint += PnlNen_Paint;

            // ========================================
            // TẠO 9 NÚT CÔNG CỤ
            // ========================================

            btnPhongTo = TaoNut(
                "btnPhongTo",
                "+",
                "Phóng to bản đồ");

            btnThuNho = TaoNut(
                "btnThuNho",
                "−",
                "Thu nhỏ bản đồ");

            btnToanCanh = TaoNut(
                "btnToanCanh",
                "□",
                "Hiển thị toàn cảnh bản đồ");

            btnDiChuyen = TaoNut(
                "btnDiChuyen",
                "✥",
                "Chế độ di chuyển bản đồ");

            btnChonDoiTuong = TaoNut(
                "btnChonDoiTuong",
                "i",
                "Chọn đối tượng để xem thông tin");

            btnDoKhoangCach = TaoNut(
                "btnDoKhoangCach",
                "↔",
                "Đo khoảng cách trên bản đồ");

            btnXoaKetQuaDo = TaoNut(
                "btnXoaKetQuaDo",
                "×",
                "Xóa kết quả đo");

            btnTaoVungDem = TaoNut(
                "btnTaoVungDem",
                "◯",
                "Tạo vùng đệm cho đối tượng được chọn");

            btnQuanLyLop = TaoNut(
                "btnQuanLyLop",
                "≡",
                "Hiện hoặc ẩn bảng lớp dữ liệu");

            // ========================================
            // BỐ TRÍ CÁC NÚT THEO CHIỀU DỌC
            // ========================================

            Button[] cacNut = LayDanhSachNut();

            for (int i = 0; i < cacNut.Length; i++)
            {
                cacNut[i].SetBounds(
                    5,
                    7 + i * 44,
                    44,
                    39);

                pnlNen.Controls.Add(cacNut[i]);
            }

            Controls.Add(pnlNen);

            // ========================================
            // ĐĂNG KÝ SỰ KIỆN CÁC NÚT
            // ========================================

            btnPhongTo.Click += (sender, args) =>
                PhatYeuCau(
                    YeuCauPhongToBanDo,
                    "phóng to bản đồ");

            btnThuNho.Click += (sender, args) =>
                PhatYeuCau(
                    YeuCauThuNhoBanDo,
                    "thu nhỏ bản đồ");

            btnToanCanh.Click += (sender, args) =>
                PhatYeuCau(
                    YeuCauToanCanhBanDo,
                    "hiển thị toàn cảnh");

            btnDiChuyen.Click += (sender, args) =>
                ChonCheDo(
                    CheDoMapToolbar.DiChuyen,
                    YeuCauDiChuyenBanDo,
                    "di chuyển bản đồ");

            btnChonDoiTuong.Click += (sender, args) =>
                ChonCheDo(
                    CheDoMapToolbar.ChonDoiTuong,
                    YeuCauChonDoiTuong,
                    "chọn đối tượng");

            btnDoKhoangCach.Click += (sender, args) =>
                ChonCheDo(
                    CheDoMapToolbar.DoKhoangCach,
                    YeuCauDoKhoangCach,
                    "đo khoảng cách");

            btnXoaKetQuaDo.Click += (sender, args) =>
                PhatYeuCau(
                    YeuCauXoaKetQuaDo,
                    "xóa kết quả đo");

            btnTaoVungDem.Click += (sender, args) =>
                ChonCheDo(
                    CheDoMapToolbar.TaoVungDem,
                    YeuCauTaoVungDem,
                    "tạo vùng đệm");

            btnQuanLyLop.Click += (sender, args) =>
                PhatYeuCau(
                    YeuCauMoBangLop,
                    "hiện/ẩn bảng lớp");

            CapNhatNutDangChon();
        }

        // ============================================
        // 5. CHẾ ĐỘ ĐANG SỬ DỤNG
        // ============================================

        public CheDoMapToolbar CheDoDangChon
        {
            get
            {
                return cheDoDangChon;
            }
        }

        // ============================================
        // 6. BẬT / TẮT TƯƠNG TÁC
        // ============================================

        public bool ChoPhepTuongTac
        {
            get
            {
                return choPhepTuongTac;
            }
            set
            {
                choPhepTuongTac = value;

                foreach (Button nut in LayDanhSachNut())
                {
                    nut.Enabled = value;
                }

                if (!value)
                {
                    DatCheDoTuBenNgoai(
                        CheDoMapToolbar.Khong);
                }
            }
        }

        // ============================================
        // 7. HIỆN / ẨN NÚT VÙNG ĐỆM
        // ============================================

        public bool HienThiNutVungDem
        {
            get
            {
                return hienThiNutVungDem;
            }
            set
            {
                hienThiNutVungDem = value;

                btnTaoVungDem.Visible = value;

                SapXepLaiNut();

                if (!value &&
                    cheDoDangChon ==
                    CheDoMapToolbar.TaoVungDem)
                {
                    DatCheDoTuBenNgoai(
                        CheDoMapToolbar.Khong);
                }
            }
        }

        // ============================================
        // 8. ĐỒNG BỘ CHẾ ĐỘ TỪ MODULE GIS
        // ============================================

        /// <summary>
        /// Cập nhật nút đang chọn từ bên ngoài.
        /// Không phát sự kiện điều khiển bản đồ.
        /// </summary>

        public void DatCheDoTuBenNgoai(
            CheDoMapToolbar cheDo)
        {
            cheDoDangChon = cheDo;

            CapNhatNutDangChon();
        }

        // ============================================
        // 9. LẤY DANH SÁCH NÚT
        // ============================================

        private Button[] LayDanhSachNut()
        {
            return new Button[]
            {
                btnPhongTo,
                btnThuNho,
                btnToanCanh,
                btnDiChuyen,
                btnChonDoiTuong,
                btnDoKhoangCach,
                btnXoaKetQuaDo,
                btnTaoVungDem,
                btnQuanLyLop
            };
        }

        // ============================================
        // 10. HÀM TẠO NÚT
        // ============================================

        private Button TaoNut(
            string ten,
            string kyHieu,
            string moTa)
        {
            Button nut = new Button();

            nut.Name = ten;
            nut.Text = kyHieu;

            nut.AccessibleName = moTa;
            nut.AccessibleDescription = moTa;

            nut.Size = new Size(44, 39);

            nut.BackColor = mauNut;
            nut.ForeColor = Color.White;

            nut.FlatStyle = FlatStyle.Flat;

            nut.FlatAppearance.BorderSize = 0;

            nut.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(47, 78, 99);

            nut.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(28, 109, 87);

            nut.UseVisualStyleBackColor = false;

            nut.Font = new Font(
                "Segoe UI Symbol",
                15F,
                FontStyle.Bold);

            nut.Cursor = Cursors.Hand;
            nut.TabStop = true;

            toolTip.SetToolTip(nut, moTa);

            return nut;
        }

        // ============================================
        // 11. SẮP XẾP LẠI NÚT
        // ============================================

        private void SapXepLaiNut()
        {
            int index = 0;

            foreach (Button nut in LayDanhSachNut())
            {
                if (nut == btnTaoVungDem &&
                    !hienThiNutVungDem)
                {
                    continue;
                }

                nut.SetBounds(
                    5,
                    7 + index * 44,
                    44,
                    39);

                index++;
            }

            int chieuCaoMoi = 13 + index * 44;

            MinimumSize =
                new Size(54, chieuCaoMoi);

            Height = chieuCaoMoi;
        }

        // ============================================
        // 12. CẬP NHẬT MÀU CHẾ ĐỘ ĐANG CHỌN
        // ============================================

        private void CapNhatNutDangChon()
        {
            if (btnDiChuyen == null)
            {
                return;
            }

            btnDiChuyen.BackColor =
                cheDoDangChon ==
                CheDoMapToolbar.DiChuyen
                    ? mauDuocChon
                    : mauNut;

            btnChonDoiTuong.BackColor =
                cheDoDangChon ==
                CheDoMapToolbar.ChonDoiTuong
                    ? mauDuocChon
                    : mauNut;

            btnDoKhoangCach.BackColor =
                cheDoDangChon ==
                CheDoMapToolbar.DoKhoangCach
                    ? mauDuocChon
                    : mauNut;

            btnTaoVungDem.BackColor =
                cheDoDangChon ==
                CheDoMapToolbar.TaoVungDem
                    ? mauDuocChon
                    : mauNut;
        }

        // ============================================
        // 13. PHÁT YÊU CẦU CHỨC NĂNG
        // ============================================

        private void PhatYeuCau(
            EventHandler suKien,
            string tenChucNang)
        {
            if (suKien == null)
            {
                ThongBaoChuaKetNoi(tenChucNang);
                return;
            }

            try
            {
                suKien(
                    this,
                    EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Không thực hiện được chức năng " +
                    tenChucNang +
                    ":\n" +
                    ex.Message,
                    "Lỗi công cụ bản đồ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 14. KÍCH HOẠT CHẾ ĐỘ BẢN ĐỒ
        // ============================================

        private void ChonCheDo(
            CheDoMapToolbar cheDo,
            EventHandler suKien,
            string tenChucNang)
        {
            if (suKien == null)
            {
                ThongBaoChuaKetNoi(tenChucNang);
                return;
            }

            try
            {
                suKien(
                    this,
                    EventArgs.Empty);

                DatCheDoTuBenNgoai(cheDo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Không kích hoạt được chế độ " +
                    tenChucNang +
                    ":\n" +
                    ex.Message,
                    "Lỗi công cụ bản đồ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ============================================
        // 15. THÔNG BÁO CHƯA TÍCH HỢP
        // ============================================

        private void ThongBaoChuaKetNoi(
            string tenChucNang)
        {
            MessageBox.Show(
                this,
                "Công cụ " +
                tenChucNang +
                " chưa được FormMain hoặc mô-đun GIS " +
                "đăng ký xử lý.",
                "Chưa tích hợp MapWinGIS",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ============================================
        // 16. VẼ VIỀN THANH CÔNG CỤ
        // ============================================

        private void PnlNen_Paint(
            object sender,
            PaintEventArgs e)
        {
            Rectangle khung =
                pnlNen.ClientRectangle;

            if (khung.Width < 2 ||
                khung.Height < 2)
            {
                return;
            }

            using (Pen but = new Pen(
                Color.FromArgb(53, 75, 95)))
            {
                e.Graphics.DrawRectangle(
                    but,
                    0,
                    0,
                    khung.Width - 1,
                    khung.Height - 1);
            }
        }

        // ============================================
        // 17. GIẢI PHÓNG TÀI NGUYÊN
        // ============================================

        protected override void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                toolTip.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
