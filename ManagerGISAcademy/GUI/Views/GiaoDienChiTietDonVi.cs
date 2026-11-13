
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    // Trang chi tiet thuan WinForms, bo cuc mau tu HTML chi tiet don vi light.
    public sealed class UcChiTietDonViNative : NativePage
    {
        private readonly Dictionary<string, Label> values = new Dictionary<string, Label>();
        private readonly Label heading, locationHint;
        // Hien day du noi dung khi gia tri dai bi cat (AutoEllipsis).
        private readonly ToolTip tipGiaTri = new ToolTip();
        private DonViQuanDoi current;
        public event Action<string> YeuCauSua;
        public event Action<string> YeuCauDinhVi;
        public event Action YeuCauDong;

        public UcChiTietDonViNative()
        {
            Name = "UcChiTietDonViNative";
            Disposed += (s, e) => tipGiaTri.Dispose();

            // Cuon trang thi Body doi vi tri; Windows khong tu an
            // tooltip dang hien nen no "treo" de len o khac.
            Body.LocationChanged += (s, e) => AnTooltip();
            AddRow(TienIchGiaoDienSang.Banner("CHI TIẾT HỒ SƠ ĐƠN VỊ",
                "Thông tin đơn vị quân đội · dữ liệu được truy xuất từ PostGIS"), 93);
            NativeCard identity = TienIchGiaoDienSang.Card(Color.FromArgb(229, 253, 246));
            heading = TienIchGiaoDienSang.Text("Chưa chọn hồ sơ", 16F, true, TienIchGiaoDienSang.Ink);
            identity.Controls.Add(heading);
            AddRow(identity, 79);
            // Hang nut dat ngay duoi ten don vi (truoc day o cuoi trang
            // ~1130 px, bi thanh trang thai che o man hinh 768 px).
            NativeCard footer = TienIchGiaoDienSang.Card();
            FlowLayoutPanel buttons = TienIchGiaoDienSang.Buttons(
                TienIchGiaoDienSang.Button("⌖ Xem trên bản đồ", TienIchGiaoDienSang.Teal, 174),
                TienIchGiaoDienSang.Button("✎ Sửa hồ sơ", TienIchGiaoDienSang.Blue, 141),
                TienIchGiaoDienSang.Button("⧉ Sao chép tọa độ", TienIchGiaoDienSang.Purple, 176),
                TienIchGiaoDienSang.Button("× Đóng", TienIchGiaoDienSang.Amber, 108));
            // Card mac dinh padding 16 + margin 10: o 67 px chi con
            // ~25 px cho nut cao 39 px (bi cat doc). Giam padding,
            // tang chieu cao hang va tat cuon cua nhom nut.
            footer.Padding = new Padding(12, 6, 12, 6);
            buttons.AutoScroll = false;
            footer.Controls.Add(buttons);
            AddRow(footer, 82);
            AddRow(TienIchGiaoDienSang.SectionTitle("THÔNG TIN CƠ BẢN"), 41);
            AddFields(new[] {
                Tuple.Create("Mã đơn vị", "MaDonVi"),
                Tuple.Create("Tên đầy đủ", "TenDonVi"),
                Tuple.Create("Tên viết tắt", "TenVietTat"),
                Tuple.Create("Loại đơn vị", "LoaiDonVi"),
                Tuple.Create("Quân / Binh chủng", "QuanBinhChung"),
                Tuple.Create("Đơn vị cấp trên", "DonViCapTren")
            }, 3);
            AddRow(TienIchGiaoDienSang.SectionTitle("ĐỊA BÀN VÀ ĐỊA CHỈ"), 41);
            AddFields(new[] {
                Tuple.Create("Quận / Huyện", "QuanHuyen"),
                Tuple.Create("Địa chỉ", "DiaChi")
            }, 2, 1);
            AddRow(TienIchGiaoDienSang.SectionTitle("TỌA ĐỘ KHÔNG GIAN"), 41);
            AddFields(new[] {
                Tuple.Create("WGS84 · Kinh độ", "KinhDo"),
                Tuple.Create("WGS84 · Vĩ độ", "ViDo"),
                Tuple.Create("VN-2000 · X", "XVn2000"),
                Tuple.Create("VN-2000 · Y", "YVn2000")
            }, 2);
            locationHint = TienIchGiaoDienSang.Text(
                "Lưu ý: giá trị tọa độ lấy từ hồ sơ. Không tự chuyển hệ quy chiếu.",
                8.5F, false, TienIchGiaoDienSang.Muted);
            AddRow(locationHint, 33);
            AddRow(TienIchGiaoDienSang.SectionTitle("CHỈ HUY VÀ TÌNH TRẠNG"), 41);
            AddFields(new[] {
                Tuple.Create("Chỉ huy trưởng", "ChiHuyTruong"),
                Tuple.Create("Chính ủy", "ChinhUy"),
                Tuple.Create("Số điện thoại", "SoDienThoai"),
                Tuple.Create("Trạng thái sẵn sàng", "TrangThaiSSCD")
            }, 2);
            Button[] a = new Button[4];
            for (int i = 0; i < 4; i++) a[i] = buttons.Controls[i] as Button;
            a[0].Click += (s, e) =>
            {
                if (current != null && NghiepVuDonVi.CoToaDoWgs84(current)
                    && YeuCauDinhVi != null) YeuCauDinhVi(current.MaDonVi);
            };
            a[1].Click += (s, e) =>
            {
                if (current != null && YeuCauSua != null) YeuCauSua(current.MaDonVi);
            };
            a[2].Click += (s, e) => CopyCoordinates();
            a[3].Click += (s, e) => { if (YeuCauDong != null) YeuCauDong(); };
        }

        private void AddFields(Tuple<string, string>[] fields, int rows, int cols = 2)
        {
            NativeCard card = TienIchGiaoDienSang.Card();
            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = cols,
                RowCount = rows,
                BackColor = Color.White
            };
            for (int c = 0; c < cols; c++)
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / cols));
            for (int i = 0; i < rows; i++)
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rows));
            Color[] back = { Color.FromArgb(235, 252, 247), Color.FromArgb(238, 247, 255),
                Color.FromArgb(247, 239, 255), Color.FromArgb(255, 248, 231) };
            for (int i = 0; i < fields.Length; i++)
            {
                NativeCard field = TienIchGiaoDienSang.Card(back[i % back.Length]);
                field.Margin = new Padding(4);
                field.Padding = new Padding(11, 6, 8, 5);
                TableLayoutPanel inside = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = field.BackColor,
                    ColumnCount = 1,
                    RowCount = 2
                };
                inside.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                inside.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
                inside.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                inside.Controls.Add(TienIchGiaoDienSang.Text(fields[i].Item1, 8.2F,
                    true, TienIchGiaoDienSang.Muted), 0, 0);
                Label value = TienIchGiaoDienSang.Text("—", 10.1F, true);
                inside.Controls.Add(value, 0, 1);
                field.Controls.Add(inside);
                grid.Controls.Add(field, i % cols, i / cols);
                values[fields[i].Item2] = value;
            }
            card.Controls.Add(grid);
            AddRow(card, rows * 80 + 13);
        }

        public void NapDuLieu(DonViQuanDoi d)
        {
            current = d;
            heading.Text = d == null ? "Chưa chọn hồ sơ" :
                (string.IsNullOrWhiteSpace(d.TenDonVi) ? "Hồ sơ đơn vị" : d.TenDonVi);
            Put("MaDonVi", d == null ? null : d.MaDonVi);
            Put("TenDonVi", d == null ? null : d.TenDonVi);
            Put("TenVietTat", d == null ? null : d.TenVietTat);
            Put("LoaiDonVi", d == null ? null : d.LoaiDonVi);
            Put("QuanBinhChung", d == null ? null : d.QuanBinhChung);
            Put("DonViCapTren", d == null ? null : d.DonViCapTren);
            Put("QuanHuyen", d == null ? null : d.QuanHuyen);
            Put("DiaChi", d == null ? null : d.DiaChi);
            Put("ChiHuyTruong", d == null ? null : d.ChiHuyTruong);
            Put("ChinhUy", d == null ? null : d.ChinhUy);
            Put("SoDienThoai", d == null ? null : d.SoDienThoai);
            Put("TrangThaiSSCD", d == null ? null : d.TrangThaiSSCD);
            bool valid = d != null && NghiepVuDonVi.CoToaDoWgs84(d);
            Put("KinhDo", valid ? d.KinhDo.ToString("F6", CultureInfo.InvariantCulture) + "° E" : null);
            Put("ViDo", valid ? d.ViDo.ToString("F6", CultureInfo.InvariantCulture) + "° N" : null);
            Put("XVn2000", d == null ? null : d.XVn2000.ToString("N2", TienIchGiaoDienSang.Vi));
            Put("YVn2000", d == null ? null : d.YVn2000.ToString("N2", TienIchGiaoDienSang.Vi));
            locationHint.Text = valid ? "● Hồ sơ có tọa độ WGS84 hợp lệ"
                : "○ Hồ sơ chưa có tọa độ WGS84 hợp lệ";
        }
        private void Put(string key, string text)
        {
            Label label;
            if (values.TryGetValue(key, out label))
            {
                label.Text = string.IsNullOrWhiteSpace(text) ? "—" : text;
                tipGiaTri.SetToolTip(label, string.IsNullOrWhiteSpace(text) ? null : text);
            }
        }
        private void AnTooltip()
        {
            // Tat roi bat lai: an moi tooltip dang hien, van hoat dong
            // binh thuong o lan re chuot tiep theo.
            tipGiaTri.Active = false;
            tipGiaTri.Active = true;
        }

        private void CopyCoordinates()
        {
            if (current == null || !NghiepVuDonVi.CoToaDoWgs84(current)) return;
            string coordinates = string.Format(CultureInfo.InvariantCulture,
                "WGS84: {0:F6}, {1:F6}", current.KinhDo, current.ViDo);
            try { Clipboard.SetText(coordinates); }
            catch (System.Runtime.InteropServices.ExternalException) { }
        }
    }
}
