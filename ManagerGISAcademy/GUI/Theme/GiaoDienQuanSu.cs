
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Theme
{
    /// <summary>
    /// Kiểu nút thống nhất trong toàn bộ giao diện.
    /// </summary>
    public enum KieuNutMilitary
    {
        Chinh,
        Phu,
        ThanhCong,
        CanhBao,
        NguyHiem
    }

    /// <summary>
    /// Kiểu chữ hiển thị trên Label.
    /// </summary>
    public enum KieuNhanMilitary
    {
        NoiDung,
        TieuDeTrang,
        TieuDeNhom,
        ChuPhu,
        ChiSo
    }

    /// <summary>
    /// Mức độ thông báo, chỉ quy định màu hiển thị.
    /// </summary>
    public enum MucDoMilitary
    {
        ThongTin,
        ThanhCong,
        CanhBao,
        Loi
    }

    /// <summary>
    /// Bộ giao diện dùng chung cho ManagerGISAcademy.
    /// Chỉ cấu hình màu sắc, phông chữ và kiểu điều khiển WinForms.
    /// Không truy vấn PostGIS, không gọi MapWinGIS và không xử lý nghiệp vụ.
    /// Tương thích C# 7.3 / .NET Framework 4.8.
    ///
    /// Chú ý:
    /// - Các phương thức không thay đổi Dock, Size, Location, Name và sự kiện.
    /// - Chỉ gọi hàm định dạng cho những control cần dùng bộ giao diện này.
    /// - Mỗi control nhận Font riêng; không chia sẻ Font để rồi Dispose nhầm.
    /// </summary>
    public static class GiaoDienQuanSu
    {
        // ============================================================
        // 1. BẢNG MÀU CHUNG
        // ============================================================

        public static readonly Color NenChinh = Color.FromArgb(17, 24, 39);
        public static readonly Color NenTieuDe = Color.FromArgb(11, 19, 36);
        public static readonly Color NenDieuHuong = Color.FromArgb(17, 29, 46);
        public static readonly Color NenThe = Color.FromArgb(22, 31, 46);
        public static readonly Color NenDauThe = Color.FromArgb(24, 41, 61);
        public static readonly Color NenNhapLieu = Color.FromArgb(30, 41, 59);
        public static readonly Color NenDongXenKe = Color.FromArgb(30, 41, 59);
        public static readonly Color NenTieuDeBang = Color.FromArgb(35, 52, 74);
        public static readonly Color MauVien = Color.FromArgb(49, 65, 85);

        public static readonly Color ChuChinh = Color.FromArgb(226, 232, 240);
        public static readonly Color ChuPhu = Color.FromArgb(148, 163, 184);
        public static readonly Color ChuTrang = Color.White;

        public static readonly Color XanhLaChinh = Color.FromArgb(5, 150, 105);
        public static readonly Color XanhLaNhat = Color.FromArgb(110, 231, 183);
        public static readonly Color XanhDuong = Color.FromArgb(30, 64, 175);
        public static readonly Color VangCanhBao = Color.FromArgb(251, 191, 36);
        public static readonly Color DoLoi = Color.FromArgb(253, 164, 175);
        public static readonly Color DoNguyHiem = Color.FromArgb(153, 27, 27);
        public static readonly Color NutPhu = Color.FromArgb(51, 65, 85);
        public static readonly Color NenDangChon = Color.FromArgb(34, 91, 76);

        public const string TenPhongChu = "Segoe UI";
        public const float CoChuMacDinh = 9F;

        // ============================================================
        // 2. PHÔNG CHỮ
        // ============================================================

        /// <summary>
        /// Tạo Font mới. Control dùng Font này sẽ quản lý vòng đời của nó.
        /// Không trả về một Font tĩnh để chia sẻ giữa nhiều cửa sổ.
        /// </summary>
        public static Font TaoFont(
            float coChu = CoChuMacDinh,
            FontStyle kieu = FontStyle.Regular)
        {
            if (float.IsNaN(coChu) || float.IsInfinity(coChu) || coChu <= 0F)
            {
                throw new ArgumentOutOfRangeException("coChu");
            }

            return new Font(TenPhongChu, coChu, kieu, GraphicsUnit.Point);
        }

        // ============================================================
        // 3. FORM VÀ USERCONTROL
        // ============================================================

        public static void DinhDangForm(Form form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            form.BackColor = NenChinh;
            form.ForeColor = ChuChinh;
            GanFontNeuCan(form, CoChuMacDinh, FontStyle.Regular);
        }

        public static void DinhDangUserControl(UserControl control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            control.BackColor = NenChinh;
            control.ForeColor = ChuChinh;
            GanFontNeuCan(control, CoChuMacDinh, FontStyle.Regular);
        }

        // ============================================================
        // 4. PANEL, THẺ VÀ GROUPBOX
        // ============================================================

        public static void DinhDangPanel(Panel panel, bool laThanhTieuDe = false)
        {
            if (panel == null)
                throw new ArgumentNullException("panel");

            panel.BackColor = laThanhTieuDe ? NenTieuDe : NenChinh;
            panel.ForeColor = ChuChinh;
        }

        public static void DinhDangThe(Panel panel, bool laDauThe = false)
        {
            if (panel == null)
                throw new ArgumentNullException("panel");

            panel.BackColor = laDauThe ? NenDauThe : NenThe;
            panel.ForeColor = ChuChinh;
        }

        public static void DinhDangNhom(GroupBox nhom)
        {
            if (nhom == null)
                throw new ArgumentNullException("nhom");

            nhom.BackColor = NenThe;
            nhom.ForeColor = XanhLaNhat;
            GanFontNeuCan(nhom, 9F, FontStyle.Bold);
        }

        // ============================================================
        // 5. NHÃN VĂN BẢN
        // ============================================================

        public static void DinhDangNhan(
            Label nhan,
            KieuNhanMilitary kieu = KieuNhanMilitary.NoiDung)
        {
            if (nhan == null)
                throw new ArgumentNullException("nhan");

            switch (kieu)
            {
                case KieuNhanMilitary.TieuDeTrang:
                    nhan.ForeColor = ChuTrang;
                    GanFontNeuCan(nhan, 16F, FontStyle.Bold);
                    break;

                case KieuNhanMilitary.TieuDeNhom:
                    nhan.ForeColor = XanhLaNhat;
                    GanFontNeuCan(nhan, 10F, FontStyle.Bold);
                    break;

                case KieuNhanMilitary.ChuPhu:
                    nhan.ForeColor = ChuPhu;
                    GanFontNeuCan(nhan, 8.5F, FontStyle.Regular);
                    break;

                case KieuNhanMilitary.ChiSo:
                    nhan.ForeColor = XanhLaNhat;
                    GanFontNeuCan(nhan, 20F, FontStyle.Bold);
                    break;

                case KieuNhanMilitary.NoiDung:
                    nhan.ForeColor = ChuChinh;
                    GanFontNeuCan(nhan, CoChuMacDinh, FontStyle.Regular);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("kieu");
            }
        }

        // ============================================================
        // 6. NÚT BẤM
        // ============================================================

        public static void DinhDangNut(
            Button nut,
            KieuNutMilitary kieu = KieuNutMilitary.Chinh)
        {
            if (nut == null)
                throw new ArgumentNullException("nut");

            Color mau;
            switch (kieu)
            {
                case KieuNutMilitary.Chinh:
                case KieuNutMilitary.ThanhCong:
                    mau = XanhLaChinh;
                    break;
                case KieuNutMilitary.Phu:
                    mau = NutPhu;
                    break;
                case KieuNutMilitary.CanhBao:
                    mau = Color.FromArgb(146, 64, 14);
                    break;
                case KieuNutMilitary.NguyHiem:
                    mau = DoNguyHiem;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("kieu");
            }

            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = LamSang(mau, 18);
            nut.FlatAppearance.MouseDownBackColor = LamSang(mau, 8);
            nut.BackColor = mau;
            nut.ForeColor = ChuTrang;
            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
            GanFontNeuCan(nut, 9F, FontStyle.Bold);
        }

        /// <summary>
        /// Chỉ đổi hình thức, không phát sự kiện điều hướng.
        /// </summary>
        public static void DinhDangNutDieuHuong(Button nut, bool dangChon)
        {
            if (nut == null)
                throw new ArgumentNullException("nut");

            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 76, 87);
            nut.FlatAppearance.MouseDownBackColor = NenDangChon;
            nut.BackColor = dangChon ? NenDangChon : NenDieuHuong;
            nut.ForeColor = dangChon ? ChuTrang : ChuChinh;
            nut.Cursor = Cursors.Hand;
            nut.UseVisualStyleBackColor = false;
            nut.TextAlign = ContentAlignment.MiddleLeft;
            GanFontNeuCan(nut, 9F, FontStyle.Bold);
        }

        // ============================================================
        // 7. ĐIỀU KHIỂN NHẬP LIỆU
        // ============================================================

        public static void DinhDangOChu(TextBox oNhap)
        {
            if (oNhap == null)
                throw new ArgumentNullException("oNhap");

            oNhap.BackColor = NenNhapLieu;
            oNhap.ForeColor = ChuTrang;
            oNhap.BorderStyle = BorderStyle.FixedSingle;
            GanFontNeuCan(oNhap, 9.5F, FontStyle.Regular);
        }

        public static void DinhDangOChu(RichTextBox oNhap)
        {
            if (oNhap == null)
                throw new ArgumentNullException("oNhap");

            oNhap.BackColor = NenNhapLieu;
            oNhap.ForeColor = ChuTrang;
            oNhap.BorderStyle = BorderStyle.FixedSingle;
            GanFontNeuCan(oNhap, 9.5F, FontStyle.Regular);
        }

        public static void DinhDangComboBox(ComboBox combo)
        {
            if (combo == null)
                throw new ArgumentNullException("combo");

            // Giữ nguyên DropDownStyle vì có màn hình cần cho nhập tự do.
            combo.BackColor = NenNhapLieu;
            combo.ForeColor = ChuTrang;
            combo.FlatStyle = FlatStyle.Flat;
            GanFontNeuCan(combo, 9.5F, FontStyle.Regular);
        }

        public static void DinhDangOSo(NumericUpDown oSo)
        {
            if (oSo == null)
                throw new ArgumentNullException("oSo");

            // Giữ nguyên Minimum/Maximum/Value/DecimalPlaces.
            oSo.BackColor = NenNhapLieu;
            oSo.ForeColor = ChuTrang;
            oSo.BorderStyle = BorderStyle.FixedSingle;
            GanFontNeuCan(oSo, 9.5F, FontStyle.Regular);
        }

        public static void DinhDangCheckBox(CheckBox oChon)
        {
            if (oChon == null)
                throw new ArgumentNullException("oChon");

            oChon.ForeColor = ChuChinh;
            oChon.UseVisualStyleBackColor = false;
            GanFontNeuCan(oChon, 9F, FontStyle.Regular);
        }

        public static void DinhDangRadioButton(RadioButton oChon)
        {
            if (oChon == null)
                throw new ArgumentNullException("oChon");

            oChon.ForeColor = ChuChinh;
            oChon.UseVisualStyleBackColor = false;
            GanFontNeuCan(oChon, 9F, FontStyle.Regular);
        }

        // ============================================================
        // 8. BẢNG DỮ LIỆU
        // ============================================================

        /// <summary>
        /// Định dạng DataGridView. Mặc định là bảng chỉ xem.
        /// Không xóa cột, DataSource, sự kiện hoặc lựa chọn hiện tại.
        /// Nếu chiDoc=false, giữ nguyên trạng thái ReadOnly sẵn có.
        /// </summary>
        public static void DinhDangBang(DataGridView bang, bool chiDoc = true)
        {
            if (bang == null)
                throw new ArgumentNullException("bang");

            bang.BackgroundColor = NenThe;
            bang.GridColor = MauVien;
            bang.BorderStyle = BorderStyle.None;
            bang.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            bang.EnableHeadersVisualStyles = false;
            bang.RowHeadersVisible = false;
            bang.ColumnHeadersHeight = 40;
            bang.RowTemplate.Height = 35;

            bang.ColumnHeadersDefaultCellStyle.BackColor = NenTieuDeBang;
            bang.ColumnHeadersDefaultCellStyle.ForeColor = ChuTrang;
            bang.ColumnHeadersDefaultCellStyle.SelectionBackColor = NenTieuDeBang;
            bang.ColumnHeadersDefaultCellStyle.SelectionForeColor = ChuTrang;
            bang.ColumnHeadersDefaultCellStyle.Font = TaoFont(9F, FontStyle.Bold);

            bang.DefaultCellStyle.BackColor = NenThe;
            bang.DefaultCellStyle.ForeColor = ChuTrang;
            bang.DefaultCellStyle.SelectionBackColor = NenDangChon;
            bang.DefaultCellStyle.SelectionForeColor = ChuTrang;
            bang.DefaultCellStyle.Font = TaoFont(9F, FontStyle.Regular);
            bang.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            bang.AlternatingRowsDefaultCellStyle.BackColor = NenDongXenKe;
            bang.AlternatingRowsDefaultCellStyle.ForeColor = ChuTrang;
            bang.AlternatingRowsDefaultCellStyle.SelectionBackColor = NenDangChon;
            bang.AlternatingRowsDefaultCellStyle.SelectionForeColor = ChuTrang;

            if (chiDoc)
            {
                bang.ReadOnly = true;
                bang.AllowUserToAddRows = false;
                bang.AllowUserToDeleteRows = false;
                bang.AllowUserToResizeRows = false;
                bang.MultiSelect = false;
                bang.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            }
        }

        // ============================================================
        // 9. THANH TRẠNG THÁI VÀ MÀU THÔNG BÁO
        // ============================================================

        public static void DinhDangThanhTrangThai(StatusStrip thanh)
        {
            if (thanh == null)
                throw new ArgumentNullException("thanh");

            thanh.BackColor = NenTieuDe;
            thanh.ForeColor = ChuChinh;
            thanh.SizingGrip = false;
            thanh.ShowItemToolTips = true;
            thanh.Font = TaoFont(9F, FontStyle.Regular);
        }

        public static Color LayMauThongBao(MucDoMilitary mucDo)
        {
            switch (mucDo)
            {
                case MucDoMilitary.ThongTin:
                    return ChuChinh;
                case MucDoMilitary.ThanhCong:
                    return XanhLaNhat;
                case MucDoMilitary.CanhBao:
                    return VangCanhBao;
                case MucDoMilitary.Loi:
                    return DoLoi;
                default:
                    throw new ArgumentOutOfRangeException("mucDo");
            }
        }

        public static void DinhDangThongBao(
            Label nhan,
            MucDoMilitary mucDo,
            string noiDung = null)
        {
            if (nhan == null)
                throw new ArgumentNullException("nhan");

            nhan.ForeColor = LayMauThongBao(mucDo);
            if (noiDung != null)
                nhan.Text = noiDung;
        }

        public static void DinhDangThongBao(
            ToolStripStatusLabel nhan,
            MucDoMilitary mucDo,
            string noiDung = null)
        {
            if (nhan == null)
                throw new ArgumentNullException("nhan");

            nhan.ForeColor = LayMauThongBao(mucDo);
            if (noiDung != null)
                nhan.Text = noiDung;
        }

        // ============================================================
        // 10. HÀM HỖ TRỢ NỘI BỘ
        // ============================================================

        private static Color LamSang(Color mau, int muc)
        {
            return Color.FromArgb(
                mau.A,
                Math.Min(255, mau.R + muc),
                Math.Min(255, mau.G + muc),
                Math.Min(255, mau.B + muc));
        }

        private static void GanFontNeuCan(
            Control control,
            float coChu,
            FontStyle kieu)
        {
            Font fontCu = control.Font;
            if (fontCu != null &&
                string.Equals(fontCu.FontFamily.Name, TenPhongChu,
                    StringComparison.OrdinalIgnoreCase) &&
                Math.Abs(fontCu.SizeInPoints - coChu) < 0.05F &&
                fontCu.Style == kieu)
            {
                return;
            }

            // Font mới gắn riêng cho control; không Dispose font đang có,
            // vì font cũ có thể đang dùng ở control hoặc Form khác.
            control.Font = TaoFont(coChu, kieu);
        }
    }
}
