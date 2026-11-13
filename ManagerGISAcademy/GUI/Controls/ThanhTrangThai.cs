
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Controls
{
    /// <summary>
    /// Mức độ của thông báo trên thanh trạng thái.
    /// </summary>
    public enum MucDoTrangThai
    {
        ThongTin,
        ThanhCong,
        CanhBao,
        Loi
    }

    /// <summary>
    /// Thanh trạng thái dùng chung cho FormMain.
    /// Chỉ hiển thị dữ liệu được cung cấp.
    /// Không tự kết nối PostGIS, không trực tiếp gọi MapWinGIS
    /// và không tự chuyển đổi hệ quy chiếu.
    ///
    /// Tương thích C# 7.3 và .NET Framework 4.8.
    /// Không cần file StatusBarView.Designer.cs.
    /// </summary>
    public class ThanhTrangThai : UserControl
    {
        // ============================================
        // 1. KHAI BÁO ĐIỀU KHIỂN
        // ============================================

        private readonly StatusStrip thanhTrangThai;

        private readonly ToolStripStatusLabel lblThongBao;
        private readonly ToolStripStatusLabel lblKhoangTrong;
        private readonly ToolStripStatusLabel lblPostGIS;
        private readonly ToolStripStatusLabel lblSoDoiTuong;
        private readonly ToolStripStatusLabel lblTyLe;
        private readonly ToolStripStatusLabel lblToaDoPhang;
        private readonly ToolStripStatusLabel lblToaDoWgs84;
        private readonly ToolStripStatusLabel lblHeQuyChieu;

        // ============================================
        // 2. MÀU SẮC GIAO DIỆN
        // ============================================

        private readonly Color mauThongTin =
            Color.FromArgb(203, 213, 225);

        private readonly Color mauThanhCong =
            Color.FromArgb(110, 231, 183);

        private readonly Color mauCanhBao =
            Color.FromArgb(251, 191, 36);

        private readonly Color mauLoi =
            Color.FromArgb(253, 164, 175);

        // ============================================
        // 3. SỰ KIỆN KIỂM TRA POSTGIS
        // ============================================

        public event EventHandler YeuCauKiemTraPostGIS;

        // ============================================
        // 4. HÀM KHỞI TẠO
        // ============================================

        public ThanhTrangThai()
        {
            Name = "StatusBarView";

            Dock = DockStyle.Bottom;
            Height = 34;

            MinimumSize = new Size(0, 34);

            BackColor =
                Color.FromArgb(11, 20, 32);

            Font = new Font("Segoe UI", 9F);

            // ========================================
            // 4.1. THANH TRẠNG THÁI
            // ========================================

            thanhTrangThai = new StatusStrip();

            thanhTrangThai.Name = "thanhTrangThai";

            thanhTrangThai.Dock = DockStyle.Fill;

            thanhTrangThai.AutoSize = false;

            thanhTrangThai.Height = 34;

            thanhTrangThai.SizingGrip = false;

            thanhTrangThai.ShowItemToolTips = true;

            thanhTrangThai.BackColor = BackColor;

            thanhTrangThai.ForeColor = mauThongTin;

            thanhTrangThai.Font =
                new Font("Segoe UI", 9F);

            thanhTrangThai.Padding =
                new Padding(10, 2, 10, 2);

            // ========================================
            // 4.2. THÔNG BÁO HỆ THỐNG
            // ========================================

            lblThongBao = TaoNhan(
                "lblThongBao",
                "Sẵn sàng",
                mauThongTin);

            lblThongBao.Spring = true;

            lblThongBao.TextAlign =
                ContentAlignment.MiddleLeft;

            lblThongBao.ToolTipText =
                "Thông báo hoạt động của hệ thống";

            // ========================================
            // 4.3. KHOẢNG CÁCH
            // ========================================

            lblKhoangTrong = TaoNhan(
                "lblKhoangTrong",
                "",
                mauThongTin);

            lblKhoangTrong.AutoSize = false;

            lblKhoangTrong.Width = 4;

            // ========================================
            // 4.4. TRẠNG THÁI POSTGIS
            // ========================================

            lblPostGIS = TaoNhan(
                "lblPostGIS",
                "PostGIS: Chưa kiểm tra",
                mauCanhBao);

            lblPostGIS.IsLink = true;

            lblPostGIS.ToolTipText =
                "Nhấn để yêu cầu kiểm tra kết nối PostGIS";

            lblPostGIS.Click += LblPostGIS_Click;

            // ========================================
            // 4.5. SỐ ĐỐI TƯỢNG
            // ========================================

            lblSoDoiTuong = TaoNhan(
                "lblSoDoiTuong",
                "Đối tượng: —",
                mauThongTin);

            lblSoDoiTuong.ToolTipText =
                "Số đối tượng đang được hiển thị";

            // ========================================
            // 4.6. TỶ LỆ BẢN ĐỒ
            // ========================================

            lblTyLe = TaoNhan(
                "lblTyLe",
                "Tỷ lệ: —",
                mauThongTin);

            lblTyLe.ToolTipText =
                "Mẫu số tỷ lệ bản đồ do mô-đun GIS cung cấp";

            // ========================================
            // 4.7. TỌA ĐỘ PHẲNG X/Y
            // ========================================

            lblToaDoPhang = TaoNhan(
                "lblToaDoPhang",
                "X/Y: —",
                mauThongTin);

            lblToaDoPhang.ToolTipText =
                "Tọa độ phẳng của con trỏ, đơn vị mét";

            // ========================================
            // 4.8. TỌA ĐỘ WGS84
            // ========================================

            lblToaDoWgs84 = TaoNhan(
                "lblToaDoWgs84",
                "WGS84: —",
                mauThongTin);

            lblToaDoWgs84.ToolTipText =
                "Kinh độ và vĩ độ WGS84 của con trỏ";

            // ========================================
            // 4.9. HỆ QUY CHIẾU
            // ========================================

            lblHeQuyChieu = TaoNhan(
                "lblHeQuyChieu",
                "CRS: Chưa xác định",
                mauThongTin);

            lblHeQuyChieu.ToolTipText =
                "Hệ quy chiếu đang sử dụng của bản đồ";

            // ========================================
            // 4.10. GHÉP CÁC THÀNH PHẦN
            // ========================================

            thanhTrangThai.Items.AddRange(
                new ToolStripItem[]
                {
                    lblThongBao,
                    lblKhoangTrong,
                    lblPostGIS,
                    lblSoDoiTuong,
                    lblTyLe,
                    lblToaDoPhang,
                    lblToaDoWgs84,
                    lblHeQuyChieu
                });

            Controls.Add(thanhTrangThai);

            CapNhatTrangThaiPostGIS(null);

            CapNhatBoCuc();
        }

        // ============================================
        // 5. HÀM TẠO NHÃN DÙNG CHUNG
        // ============================================

        private static ToolStripStatusLabel TaoNhan(
            string ten,
            string noiDung,
            Color mau)
        {
            ToolStripStatusLabel nhan =
                new ToolStripStatusLabel();

            nhan.Name = ten;

            nhan.Text = noiDung;

            nhan.ForeColor = mau;

            nhan.TextAlign =
                ContentAlignment.MiddleCenter;

            nhan.Margin =
                new Padding(3, 2, 8, 2);

            return nhan;
        }

        // ============================================
        // 6. CẬP NHẬT THÔNG BÁO HỆ THỐNG
        // ============================================

        public void CapNhatThongBao(
            string noiDung,
            MucDoTrangThai mucDo =
                MucDoTrangThai.ThongTin)
        {
            string thongBao =
                string.IsNullOrWhiteSpace(noiDung)
                    ? "Sẵn sàng"
                    : noiDung
                        .Replace('\r', ' ')
                        .Replace('\n', ' ')
                        .Trim();

            lblThongBao.Text = thongBao;

            lblThongBao.ToolTipText = thongBao;

            switch (mucDo)
            {
                case MucDoTrangThai.ThanhCong:

                    lblThongBao.ForeColor =
                        mauThanhCong;

                    break;

                case MucDoTrangThai.CanhBao:

                    lblThongBao.ForeColor =
                        mauCanhBao;

                    break;

                case MucDoTrangThai.Loi:

                    lblThongBao.ForeColor =
                        mauLoi;

                    break;

                default:

                    lblThongBao.ForeColor =
                        mauThongTin;

                    break;
            }
        }

        // ============================================
        // 7. HÀM CẬP NHẬT TRẠNG THÁI NGẮN GỌN
        // ============================================

        public void CapNhatTrangThai(string noiDung)
        {
            CapNhatThongBao(noiDung);
        }

        // ============================================
        // 8. CẬP NHẬT TRẠNG THÁI POSTGIS
        // ============================================

        /// <summary>
        /// null: chưa kiểm tra.
        /// true: đã kết nối.
        /// false: không kết nối.
        /// Phương thức này không tự kiểm tra CSDL.
        /// </summary>

        public void CapNhatTrangThaiPostGIS(
            bool? daKetNoi,
            string chiTiet = null)
        {
            if (!daKetNoi.HasValue)
            {
                lblPostGIS.Text =
                    "PostGIS: Chưa kiểm tra";

                lblPostGIS.ForeColor =
                    mauCanhBao;
            }
            else if (daKetNoi.Value)
            {
                lblPostGIS.Text =
                    "PostGIS: Đã kết nối";

                lblPostGIS.ForeColor =
                    mauThanhCong;
            }
            else
            {
                lblPostGIS.Text =
                    "PostGIS: Không kết nối";

                lblPostGIS.ForeColor =
                    mauLoi;
            }

            lblPostGIS.ToolTipText =
                string.IsNullOrWhiteSpace(chiTiet)
                    ? "Nhấn để yêu cầu kiểm tra kết nối PostGIS"
                    : chiTiet;
        }

        // ============================================
        // 9. CẬP NHẬT TỌA ĐỘ WGS84
        // ============================================

        /// <summary>
        /// Thứ tự truyền tham số:
        /// kinh độ trước, vĩ độ sau.
        ///
        /// Đơn vị là độ.
        /// Không truyền tọa độ pixel hoặc tọa độ phẳng.
        /// </summary>

        public void CapNhatToaDo(
            double kinhDo,
            double viDo)
        {
            if (!LaSoHuuHan(kinhDo) ||
                !LaSoHuuHan(viDo) ||
                kinhDo < -180 ||
                kinhDo > 180 ||
                viDo < -90 ||
                viDo > 90)
            {
                lblToaDoWgs84.Text =
                    "WGS84: —";

                return;
            }

            lblToaDoWgs84.Text =
                string.Format(
                    CultureInfo.InvariantCulture,
                    "KĐ: {0:0.000000}°  VĐ: {1:0.000000}°",
                    kinhDo,
                    viDo);
        }

        // ============================================
        // 10. CẬP NHẬT TỌA ĐỘ PHẲNG X/Y
        // ============================================

        /// <summary>
        /// X/Y phải là tọa độ phẳng theo CRS thực tế.
        /// Đơn vị mét.
        ///
        /// Không tự giả định rằng X/Y là VN-2000
        /// hoặc EPSG:32648 nếu chưa xác minh CRS.
        /// </summary>

        public void CapNhatToaDoPhang(
            double x,
            double y)
        {
            if (!LaSoHuuHan(x) ||
                !LaSoHuuHan(y))
            {
                lblToaDoPhang.Text =
                    "X/Y: —";

                return;
            }

            lblToaDoPhang.Text =
                string.Format(
                    CultureInfo.GetCultureInfo("vi-VN"),
                    "X: {0:N2} m  Y: {1:N2} m",
                    x,
                    y);
        }

        // ============================================
        // 11. XÓA HIỂN THỊ TỌA ĐỘ
        // ============================================

        public void XoaToaDo()
        {
            lblToaDoWgs84.Text =
                "WGS84: —";

            lblToaDoPhang.Text =
                "X/Y: —";
        }

        // ============================================
        // 12. CẬP NHẬT HỆ QUY CHIẾU
        // ============================================

        /// <summary>
        /// Ví dụ: EPSG:32648.
        /// Phải đọc CRS thực tế từ lớp bản đồ.
        /// </summary>

        public void CapNhatHeToaDo(
            string heQuyChieu)
        {
            lblHeQuyChieu.Text =
                string.IsNullOrWhiteSpace(heQuyChieu)
                    ? "CRS: Chưa xác định"
                    : "CRS: " + heQuyChieu.Trim();

            lblHeQuyChieu.ToolTipText =
                lblHeQuyChieu.Text;
        }

        // ============================================
        // 13. CẬP NHẬT TỶ LỆ BẢN ĐỒ
        // ============================================

        /// <summary>
        /// Truyền mẫu số tỷ lệ.
        /// Ví dụ 50000 sẽ hiển thị 1:50.000.
        /// Giá trị do mô-đun GIS cung cấp.
        /// </summary>

        public void CapNhatTyLeBanDo(
            double? mauSoTyLe)
        {
            if (!mauSoTyLe.HasValue ||
                !LaSoHuuHan(mauSoTyLe.Value) ||
                mauSoTyLe.Value <= 0)
            {
                lblTyLe.Text =
                    "Tỷ lệ: —";

                return;
            }

            lblTyLe.Text =
                string.Format(
                    CultureInfo.GetCultureInfo("vi-VN"),
                    "Tỷ lệ: 1:{0:N0}",
                    mauSoTyLe.Value);
        }

        // ============================================
        // 14. CẬP NHẬT SỐ ĐỐI TƯỢNG
        // ============================================

        /// <summary>
        /// Số đối tượng đang hiển thị trên bản đồ.
        /// Không phải số lượng lớp dữ liệu.
        /// Giá trị 0 là hợp lệ.
        /// </summary>

        public void CapNhatSoDoiTuong(
            int? soLuong)
        {
            if (!soLuong.HasValue ||
                soLuong.Value < 0)
            {
                lblSoDoiTuong.Text =
                    "Đối tượng: —";

                return;
            }

            lblSoDoiTuong.Text =
                "Đối tượng: " +
                soLuong.Value.ToString(
                    "N0",
                    CultureInfo.GetCultureInfo("vi-VN"));
        }

        // ============================================
        // 15. XÓA THÔNG TIN BẢN ĐỒ
        // ============================================

        public void XoaThongTinBanDo()
        {
            XoaToaDo();

            CapNhatHeToaDo(null);

            CapNhatTyLeBanDo(null);

            CapNhatSoDoiTuong(null);
        }

        // ============================================
        // 16. KIỂM TRA SỐ HỢP LỆ
        // ============================================

        private static bool LaSoHuuHan(
            double giaTri)
        {
            return
                !double.IsNaN(giaTri) &&
                !double.IsInfinity(giaTri);
        }

        // ============================================
        // 17. YÊU CẦU KIỂM TRA POSTGIS
        // ============================================

        private void LblPostGIS_Click(
            object sender,
            EventArgs e)
        {
            EventHandler suKien =
                YeuCauKiemTraPostGIS;

            if (suKien != null)
            {
                suKien(
                    this,
                    EventArgs.Empty);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "Chưa đăng ký chức năng kiểm tra PostGIS " +
                    "từ FormMain hoặc tầng nghiệp vụ.",
                    "Chưa tích hợp",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        // ============================================
        // 18. CẬP NHẬT BỐ CỤC KHI ĐỔI KÍCH THƯỚC
        // ============================================

        protected override void OnResize(
            EventArgs e)
        {
            base.OnResize(e);

            CapNhatBoCuc();
        }

        private void CapNhatBoCuc()
        {
            if (lblPostGIS == null)
            {
                return;
            }

            int rong = ClientSize.Width;

            lblPostGIS.Visible =
                rong >= 440;

            lblSoDoiTuong.Visible =
                rong >= 1050;

            lblTyLe.Visible =
                rong >= 950;

            lblToaDoPhang.Visible =
                rong >= 1260;

            lblToaDoWgs84.Visible =
                rong >= 760;

            lblHeQuyChieu.Visible =
                rong >= 620;
        }
    }
}
