
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Controls
{
    /// <summary>
    /// Thanh đầu trang dùng chung của ứng dụng GIS.
    /// Chỉ hiển thị giao diện và phát sự kiện; không truy vấn PostGIS.
    /// Tương thích Windows Forms .NET Framework và C# 7.3.
    /// Không cần file HeaderBar.Designer.cs.
    /// </summary>
    public class HeaderBar : UserControl
    {
        private readonly Panel pnlHeader;
        private readonly PictureBox picLogo;
        private readonly Label lblTitle;
        private readonly Label lblSubTitle;
        private readonly TextBox txtQuickSearch;
        private readonly Button btnQuickSearch;
        private readonly Button btnPostgisStatus;
        private readonly Button btnThemDonVi;
        private readonly ToolTip toolTip;

        // FormMain (hoặc lớp điều phối) đăng ký các sự kiện này.
        public event EventHandler YeuCauTraCuuNhanh;
        public event EventHandler YeuCauThemDonVi;
        public event EventHandler YeuCauKiemTraPostGIS;

        public HeaderBar()
        {
            Name = "HeaderBar";
            Size = new Size(1200, 76);
            MinimumSize = new Size(430, 76);
            Dock = DockStyle.Top;
            BackColor = Color.FromArgb(11, 19, 36);
            Font = new Font("Segoe UI", 9F);

            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            toolTip = new ToolTip();

            pnlHeader = new Panel();
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.BackColor = BackColor;

            // ============================================
            // 1. BIỂU TƯỢNG HỆ THỐNG
            // ============================================

            picLogo = new PictureBox();
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(42, 42);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.BackColor = Color.FromArgb(24, 45, 68);

            // ============================================
            // 2. TIÊU ĐỀ HỆ THỐNG
            // ============================================

            lblTitle = new Label();
            lblTitle.Name = "lblTitle";
            lblTitle.Text =
                "HỆ THỐNG GIS QUẢN LÝ VÀ TRA CỨU ĐƠN VỊ";
            lblTitle.ForeColor = Color.White;
            lblTitle.Font = new Font(
                "Segoe UI", 11F, FontStyle.Bold);
            lblTitle.AutoEllipsis = true;
            lblTitle.TextAlign =
                ContentAlignment.MiddleLeft;

            lblSubTitle = new Label();
            lblSubTitle.Name = "lblSubTitle";
            lblSubTitle.Text =
                "HỆ QUẢN TRỊ CƠ SỞ DỮ LIỆU KHÔNG GIAN | HÀ NỘI";
            lblSubTitle.ForeColor =
                Color.FromArgb(148, 163, 184);
            lblSubTitle.Font =
                new Font("Segoe UI", 8F);
            lblSubTitle.AutoEllipsis = true;
            lblSubTitle.TextAlign =
                ContentAlignment.MiddleLeft;

            // ============================================
            // 3. Ô TÌM KIẾM NHANH
            // ============================================

            txtQuickSearch = new TextBox();
            txtQuickSearch.Name = "txtQuickSearch";
            txtQuickSearch.BorderStyle =
                BorderStyle.FixedSingle;
            txtQuickSearch.BackColor =
                Color.FromArgb(30, 41, 59);
            txtQuickSearch.ForeColor = Color.White;
            txtQuickSearch.Font =
                new Font("Segoe UI", 10F);
            txtQuickSearch.MaxLength = 200;

            // ============================================
            // 4. CÁC NÚT CHỨC NĂNG
            // ============================================

            btnQuickSearch = TaoNut(
                "btnQuickSearch",
                "TÌM",
                Color.FromArgb(5, 150, 105));

            btnPostgisStatus = TaoNut(
                "btnPostgisStatus",
                "PostGIS: Chưa kiểm tra",
                Color.FromArgb(30, 41, 59));

            btnThemDonVi = TaoNut(
                "btnThemDonVi",
                "+ THÊM ĐƠN VỊ",
                Color.FromArgb(4, 120, 87));

            // ============================================
            // 5. ĐĂNG KÝ SỰ KIỆN
            // ============================================

            picLogo.Paint += PicLogo_Paint;
            pnlHeader.Paint += PnlHeader_Paint;
            pnlHeader.Resize += PnlHeader_Resize;

            txtQuickSearch.KeyDown += TxtQuickSearch_KeyDown;

            btnQuickSearch.Click +=
                BtnQuickSearch_Click;

            btnPostgisStatus.Click +=
                BtnPostgisStatus_Click;

            btnThemDonVi.Click +=
                BtnThemDonVi_Click;

            // ============================================
            // 6. CHÚ THÍCH CHỨC NĂNG
            // ============================================

            toolTip.SetToolTip(
                picLogo,
                "Hệ thống quản lý cơ sở dữ liệu không gian");

            toolTip.SetToolTip(
                txtQuickSearch,
                "Nhập mã hoặc tên đơn vị, sau đó nhấn Enter.");

            toolTip.SetToolTip(
                btnQuickSearch,
                "Tra cứu đơn vị");

            toolTip.SetToolTip(
                btnThemDonVi,
                "Mở biểu mẫu thêm đơn vị");

            // ============================================
            // 7. GHÉP CÁC ĐIỀU KHIỂN
            // ============================================

            pnlHeader.Controls.Add(picLogo);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubTitle);
            pnlHeader.Controls.Add(txtQuickSearch);
            pnlHeader.Controls.Add(btnQuickSearch);
            pnlHeader.Controls.Add(btnPostgisStatus);
            pnlHeader.Controls.Add(btnThemDonVi);

            Controls.Add(pnlHeader);

            CapNhatTrangThaiPostGIS(null);
            SapXepDieuKhien();
        }

        // ============================================
        // 8. CÁC THUỘC TÍNH CÔNG KHAI
        // ============================================

        public string TuKhoaTraCuu
        {
            get
            {
                return txtQuickSearch.Text.Trim();
            }
            set
            {
                txtQuickSearch.Text =
                    value ?? string.Empty;
            }
        }

        public string TieuDeHeThong
        {
            get
            {
                return lblTitle.Text;
            }
            set
            {
                lblTitle.Text =
                    value ?? string.Empty;
            }
        }

        // Ảnh gán từ bên ngoài.
        // Bên gán chịu trách nhiệm quản lý tài nguyên ảnh.
        public Image HinhLogo
        {
            get
            {
                return picLogo.Image;
            }
            set
            {
                picLogo.Image = value;
                picLogo.Invalidate();
            }
        }

        public bool ChoPhepThemDonVi
        {
            get
            {
                return btnThemDonVi.Enabled;
            }
            set
            {
                btnThemDonVi.Enabled = value;
            }
        }

        // ============================================
        // 9. CẬP NHẬT TRẠNG THÁI POSTGIS
        // ============================================

        /// <summary>
        /// null: chưa kiểm tra.
        /// true: đã kết nối.
        /// false: mất kết nối.
        /// Chỉ cập nhật giao diện, không tự truy vấn CSDL.
        /// </summary>
        public void CapNhatTrangThaiPostGIS(
            bool? daKetNoi,
            string thongTin = null)
        {
            if (!daKetNoi.HasValue)
            {
                btnPostgisStatus.Text =
                    "PostGIS: Chưa kiểm tra";

                btnPostgisStatus.ForeColor =
                    Color.Gainsboro;
            }
            else if (daKetNoi.Value)
            {
                btnPostgisStatus.Text =
                    "PostGIS: Đã kết nối";

                btnPostgisStatus.ForeColor =
                    Color.FromArgb(110, 231, 183);
            }
            else
            {
                btnPostgisStatus.Text =
                    "PostGIS: Mất kết nối";

                btnPostgisStatus.ForeColor =
                    Color.FromArgb(253, 164, 175);
            }

            toolTip.SetToolTip(
                btnPostgisStatus,
                string.IsNullOrWhiteSpace(thongTin)
                    ? "Nhấn để yêu cầu kiểm tra kết nối PostGIS."
                    : thongTin);
        }

        // ============================================
        // 10. ĐƯA CON TRỎ VÀO Ô TRA CỨU
        // ============================================

        public void FocusTraCuu()
        {
            if (txtQuickSearch.Visible &&
                txtQuickSearch.Enabled &&
                txtQuickSearch.CanFocus)
            {
                txtQuickSearch.Focus();
                txtQuickSearch.SelectAll();
            }
        }

        // ============================================
        // 11. HÀM TẠO NÚT DÙNG CHUNG
        // ============================================

        private static Button TaoNut(
            string name,
            string text,
            Color mau)
        {
            Button nut = new Button();

            nut.Name = name;
            nut.Text = text;
            nut.BackColor = mau;
            nut.ForeColor = Color.White;

            nut.FlatStyle =
                FlatStyle.Flat;

            nut.FlatAppearance.BorderSize = 0;

            nut.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(38, 76, 91);

            nut.Font = new Font(
                "Segoe UI",
                8.5F,
                FontStyle.Bold);

            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
            nut.Height = 34;
            nut.TabStop = true;

            return nut;
        }

        // ============================================
        // 12. VẼ BIỂU TƯỢNG GIS
        // ============================================

        private void PicLogo_Paint(
            object sender,
            PaintEventArgs e)
        {
            // Nếu có logo thật thì PictureBox tự hiển thị.
            if (picLogo.Image != null)
            {
                return;
            }

            TextRenderer.DrawText(
                e.Graphics,
                "GIS",
                lblTitle.Font,
                picLogo.ClientRectangle,
                Color.FromArgb(110, 231, 183),
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine);
        }

        // ============================================
        // 13. VẼ ĐƯỜNG NGĂN CÁCH
        // ============================================

        private void PnlHeader_Paint(
            object sender,
            PaintEventArgs e)
        {
            if (pnlHeader.ClientSize.Height <= 0)
            {
                return;
            }

            using (Pen pen = new Pen(
                Color.FromArgb(37, 52, 72)))
            {
                int y =
                    pnlHeader.ClientSize.Height - 1;

                e.Graphics.DrawLine(
                    pen,
                    0,
                    y,
                    pnlHeader.ClientSize.Width,
                    y);
            }
        }

        // ============================================
        // 14. XỬ LÝ KHI THAY ĐỔI KÍCH THƯỚC
        // ============================================

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (pnlHeader != null &&
                btnThemDonVi != null)
            {
                SapXepDieuKhien();
            }
        }

        private void PnlHeader_Resize(
            object sender,
            EventArgs e)
        {
            SapXepDieuKhien();
        }

        // ============================================
        // 15. TỰ ĐỘNG SẮP XẾP GIAO DIỆN
        // ============================================

        private void SapXepDieuKhien()
        {
            if (pnlHeader == null ||
                picLogo == null ||
                lblTitle == null ||
                btnThemDonVi == null)
            {
                return;
            }

            int width =
                pnlHeader.ClientSize.Width;

            int height =
                pnlHeader.ClientSize.Height;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            int top = Math.Max(
                0,
                (height - 34) / 2);

            int right = width - 16;

            // Logo luôn nằm bên trái.
            picLogo.SetBounds(
                18,
                Math.Max(0, (height - 42) / 2),
                42,
                42);

            // Nút thêm đơn vị ở bên phải.
            btnThemDonVi.SetBounds(
                right - 150,
                top,
                150,
                34);

            right = btnThemDonVi.Left - 10;

            // Thanh trạng thái PostGIS chỉ xuất hiện
            // khi chiều rộng đủ lớn.
            bool hienTrangThai = width >= 1090;

            btnPostgisStatus.Visible =
                hienTrangThai;

            if (hienTrangThai)
            {
                btnPostgisStatus.SetBounds(
                    right - 180,
                    top,
                    180,
                    34);

                right =
                    btnPostgisStatus.Left - 10;
            }

            // Thu gọn vùng tra cứu trên cửa sổ nhỏ.
            bool hienTraCuu = width >= 790;

            txtQuickSearch.Visible =
                hienTraCuu;

            btnQuickSearch.Visible =
                hienTraCuu;

            if (hienTraCuu)
            {
                int searchWidth =
                    width >= 1250 ? 220 : 165;

                btnQuickSearch.SetBounds(
                    right - 58,
                    top,
                    58,
                    34);

                txtQuickSearch.SetBounds(
                    btnQuickSearch.Left -
                        searchWidth - 8,
                    top + 3,
                    searchWidth,
                    28);

                right = txtQuickSearch.Left - 12;
            }

            // Tính chiều rộng còn lại cho tiêu đề.
            int titleWidth =
                Math.Max(0, right - 78);

            lblTitle.SetBounds(
                77,
                12,
                titleWidth,
                31);

            lblSubTitle.SetBounds(
                78,
                46,
                titleWidth,
                20);

            lblSubTitle.Visible =
                titleWidth >= 280;
        }

        // ============================================
        // 16. XỬ LÝ PHÍM ENTER VÀ ESC
        // ============================================

        private void TxtQuickSearch_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                PhatYeuCauTraCuu();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;

                txtQuickSearch.Clear();
            }
        }

        // ============================================
        // 17. NÚT TÌM KIẾM
        // ============================================

        private void BtnQuickSearch_Click(
            object sender,
            EventArgs e)
        {
            PhatYeuCauTraCuu();
        }

        private void PhatYeuCauTraCuu()
        {
            if (string.IsNullOrWhiteSpace(
                TuKhoaTraCuu))
            {
                MessageBox.Show(
                    this,
                    "Vui lòng nhập mã hoặc tên đơn vị cần tra cứu.",
                    "Thiếu từ khóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                FocusTraCuu();
                return;
            }

            EventHandler suKien =
                YeuCauTraCuuNhanh;

            if (suKien != null)
            {
                suKien(this, EventArgs.Empty);
            }
            else
            {
                ThongBaoChuaKetNoi(
                    "tra cứu nhanh");
            }
        }

        // ============================================
        // 18. NÚT THÊM ĐƠN VỊ
        // ============================================

        private void BtnThemDonVi_Click(
            object sender,
            EventArgs e)
        {
            EventHandler suKien =
                YeuCauThemDonVi;

            if (suKien != null)
            {
                suKien(this, EventArgs.Empty);
            }
            else
            {
                ThongBaoChuaKetNoi(
                    "thêm đơn vị");
            }
        }

        // ============================================
        // 19. NÚT KIỂM TRA POSTGIS
        // ============================================

        private void BtnPostgisStatus_Click(
            object sender,
            EventArgs e)
        {
            EventHandler suKien =
                YeuCauKiemTraPostGIS;

            if (suKien != null)
            {
                suKien(this, EventArgs.Empty);
            }
            else
            {
                ThongBaoChuaKetNoi(
                    "kiểm tra kết nối PostGIS");
            }
        }

        // ============================================
        // 20. THÔNG BÁO CHƯA TÍCH HỢP
        // ============================================

        private void ThongBaoChuaKetNoi(
            string tenChucNang)
        {
            MessageBox.Show(
                this,
                "Chức năng " +
                tenChucNang +
                " chưa được FormMain đăng ký xử lý sự kiện.",
                "Chưa tích hợp",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        // ============================================
        // 21. GIẢI PHÓNG TÀI NGUYÊN
        // ============================================

        protected override void Dispose(bool disposing)
        {
            if (disposing && toolTip != null)
            {
                toolTip.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
