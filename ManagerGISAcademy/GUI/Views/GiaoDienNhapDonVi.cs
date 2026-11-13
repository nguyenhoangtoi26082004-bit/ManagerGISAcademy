using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    // Giu nguyen hop dong du lieu voi FormMain / BLL / DAL.
    public sealed class HoSoNhapNative
    {
        public bool LaChinhSua { get; set; }
        public string MaDonVi { get; set; }
        public string TenDonVi { get; set; }
        public string TenVietTat { get; set; }
        public string LoaiDonVi { get; set; }
        public string QuanBinhChung { get; set; }
        public string DonViCapTren { get; set; }
        public string QuanHuyen { get; set; }
        public string DiaChi { get; set; }
        public double? KinhDo { get; set; }
        public double? ViDo { get; set; }
        public string ChiHuyTruong { get; set; }
        public string ChinhUy { get; set; }
        public string SoDienThoai { get; set; }
        public string TrangThaiSSCD { get; set; }
        public string NhiemVuChinh { get; set; }
        public string VuKhiTrangBi { get; set; }
    }

    // Giao dien nhap ho so Native. Khong tu ghi vao PostGIS.
    public sealed class UcNhapDonViNative : NativePage
    {
        private readonly Dictionary<string, Control> fields =
            new Dictionary<string, Control>(StringComparer.OrdinalIgnoreCase);

        private readonly Label message;
        private bool editMode;

        public event Action<HoSoNhapNative> YeuCauLuu;
        public event Action YeuCauHuy;
        public event Action YeuCauChonViTri;

        // Danh muc theo Nghi quyet 1656/NQ-UBTVQH15 (2025).
        // Goi y duoc su dung de lua chon, khong tu doan dia chi tu toa do.
        private const string PhuongHaNoi =
            "Hoàn Kiếm|Cửa Nam|Ba Đình|Ngọc Hà|Giảng Võ|Hai Bà Trưng|Vĩnh Tuy|Bạch Mai|" +
            "Đống Đa|Kim Liên|Văn Miếu - Quốc Tử Giám|Láng|Ô Chợ Dừa|Hồng Hà|Lĩnh Nam|" +
            "Hoàng Mai|Vĩnh Hưng|Tương Mai|Định Công|Hoàng Liệt|Yên Sở|Thanh Xuân|" +
            "Khương Đình|Phương Liệt|Cầu Giấy|Nghĩa Đô|Yên Hòa|Tây Hồ|Phú Thượng|" +
            "Tây Tựu|Phú Diễn|Xuân Đỉnh|Đông Ngạc|Thượng Cát|Từ Liêm|Xuân Phương|" +
            "Tây Mỗ|Đại Mỗ|Long Biên|Bồ Đề|Việt Hưng|Phúc Lợi|Hà Đông|Dương Nội|" +
            "Yên Nghĩa|Phú Lương|Kiến Hưng|Thanh Liệt|Chương Mỹ|Sơn Tây|Tùng Thiện";

        private const string XaHaNoi =
            "Thanh Trì|Đại Thanh|Nam Phù|Ngọc Hồi|Thượng Phúc|Thường Tín|Chương Dương|" +
            "Hồng Vân|Phú Xuyên|Phượng Dực|Chuyên Mỹ|Đại Xuyên|Thanh Oai|Bình Minh|" +
            "Tam Hưng|Dân Hòa|Vân Đình|Ứng Thiên|Hòa Xá|Ứng Hòa|Mỹ Đức|Hồng Sơn|" +
            "Phúc Sơn|Hương Sơn|Phú Nghĩa|Xuân Mai|Trần Phú|Hòa Phú|Quảng Bị|" +
            "Minh Châu|Quảng Oai|Vật Lại|Cổ Đô|Bất Bạt|Suối Hai|Ba Vì|Yên Bài|" +
            "Đoài Phương|Phúc Thọ|Phúc Lộc|Hát Môn|Thạch Thất|Hạ Bằng|Tây Phương|" +
            "Hòa Lạc|Yên Xuân|Quốc Oai|Hưng Đạo|Kiều Phú|Phú Cát|Hoài Đức|" +
            "Dương Hòa|Sơn Đồng|An Khánh|Đan Phượng|Ô Diên|Liên Minh|Gia Lâm|" +
            "Thuận An|Bát Tràng|Phù Đổng|Thư Lâm|Đông Anh|Phúc Thịnh|Thiên Lộc|" +
            "Vĩnh Thanh|Mê Linh|Yên Lãng|Tiến Thắng|Quang Minh|Sóc Sơn|Đa Phúc|" +
            "Nội Bài|Trung Giã|Kim Anh";

        public UcNhapDonViNative()
        {
            Name = "UcNhapDonViNative";

            AddRow(NativeLightUi.Banner(
                "THÊM / CẬP NHẬT HỒ SƠ ĐƠN VỊ",
                "Tra cứu, xác minh địa danh mới và cập nhật hồ sơ quân đội · WGS84 / PostGIS"), 100);

            AddRow(NativeLightUi.SectionTitle(
                "THÔNG TIN ĐỊNH DANH", "Dấu (*) là thông tin bắt buộc"), 44);

            AddFields(new[]
            {
                Tuple.Create("Mã đơn vị (*)", "MaDonVi"),
                Tuple.Create("Tên đơn vị (*)", "TenDonVi"),
                Tuple.Create("Tên viết tắt", "TenVietTat"),
                Tuple.Create("Loại đơn vị", "LoaiDonVi"),
                Tuple.Create("Quân / Binh chủng", "QuanBinhChung"),
                Tuple.Create("Đơn vị cấp trên", "DonViCapTren")
            });

            AddRow(NativeLightUi.SectionTitle(
                "ĐỊA BÀN HÀ NỘI", "Danh mục 126 phường/xã sau sắp xếp 2025"), 44);

            AddFields(new[]
            {
                Tuple.Create("Phường / Xã (*)", "QuanHuyen"),
                Tuple.Create("Địa chỉ chi tiết", "DiaChi")
            });

            AddRow(NativeLightUi.SectionTitle(
                "TỌA ĐỘ KHÔNG GIAN", "WGS84 (EPSG:4326)"), 44);

            AddFields(new[]
            {
                Tuple.Create("Kinh độ WGS84 (E)", "KinhDo"),
                Tuple.Create("Vĩ độ WGS84 (N)", "ViDo")
            });

            Panel hangChonToaDo = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = NativeLightUi.Background,
                Margin = new Padding(0, 2, 0, 10)
            };

            Button nutChonToaDo = NativeLightUi.Button(
                "⌖  CHỌN VỊ TRÍ TRÊN BẢN ĐỒ", NativeLightUi.Teal, 264);

            nutChonToaDo.Dock = DockStyle.Left;
            nutChonToaDo.Height = 42;
            nutChonToaDo.Click += (s, e) =>
            {
                Action xuLy = YeuCauChonViTri;
                if (xuLy == null)
                {
                    BaoTrangThai("Chưa kết nối chọn vị trí với FormMain.", true);
                    return;
                }
                xuLy();
            };

            Label ghiChuToaDo = new Label
            {
                Text = "Nhấp bản đồ để tự điền kinh độ, vĩ độ; các thông tin khác được giữ nguyên.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = NativeLightUi.Muted,
                Font = new Font("Segoe UI", 8.5F),
                AutoEllipsis = true
            };

            hangChonToaDo.Controls.Add(ghiChuToaDo);
            hangChonToaDo.Controls.Add(nutChonToaDo);
            AddRow(hangChonToaDo, 62);

            AddRow(NativeLightUi.SectionTitle(
                "LÃNH ĐẠO VÀ THÔNG TIN BỔ SUNG",
                "Chỉ nhập quân hàm, họ tên đã được xác minh"), 44);

            AddFields(new[]
            {
                Tuple.Create("Chỉ huy trưởng / Giám đốc", "ChiHuyTruong"),
                Tuple.Create("Chính ủy / Chính trị viên", "ChinhUy"),
                Tuple.Create("Số điện thoại", "SoDienThoai"),
                Tuple.Create("Trạng thái SSCĐ", "TrangThaiSSCD"),
                Tuple.Create("Nhiệm vụ chính", "NhiemVuChinh"),
                Tuple.Create("Vũ khí trang bị", "VuKhiTrangBi")
            });

            NativeCard footer = NativeLightUi.Card();
            TableLayoutPanel controls = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Padding = Padding.Empty
            };
            controls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            controls.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            controls.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            message = NativeLightUi.Text(
                "Chưa có thay đổi cần lưu.", 8.8F, false, NativeLightUi.Muted);
            controls.Controls.Add(message, 0, 0);

            Button nutLuu = NativeLightUi.Button(
                "✓  KIỂM TRA VÀ LƯU", NativeLightUi.Teal, 200);
            Button nutHuy = NativeLightUi.Button(
                "×  HỦY / QUAY LẠI", NativeLightUi.Purple, 170);
            FlowLayoutPanel buttons = NativeLightUi.Buttons(nutLuu, nutHuy);
            controls.Controls.Add(buttons, 0, 1);
            footer.Controls.Add(controls);
            AddRow(footer, 122);

            nutLuu.Click += (s, e) => ValidateAndSend();
            nutHuy.Click += (s, e) =>
            {
                Action xuLy = YeuCauHuy;
                if (xuLy != null) xuLy();
            };
        }

        private static bool LaDanhMuc(string key)
        {
            return key == "LoaiDonVi" ||
                   key == "QuanBinhChung" ||
                   key == "QuanHuyen" ||
                   key == "TrangThaiSSCD";
        }

        private Control TaoDauVao(string key)
        {
            if (!LaDanhMuc(key))
            {
                TextBox t = NativeLightUi.Input();
                t.Name = "nhap_" + key;
                return t;
            }

            ComboBox c = new ComboBox
            {
                Name = "chon_" + key,
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 7, 7, 7),
                DropDownStyle = ComboBoxStyle.DropDown,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.4F),
                BackColor = Color.White,
                ForeColor = NativeLightUi.Ink,
                IntegralHeight = false,
                DropDownHeight = 270,
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems
            };

            string[] ds = null;
            if (key == "LoaiDonVi")
                ds = new[] {
                    "Bộ Tư lệnh", "Học viện", "Nhà trường", "Binh chủng",
                    "Lữ đoàn", "Sư đoàn", "Trung đoàn", "Tiểu đoàn",
                    "Ban Chỉ huy quân sự", "Cơ quan", "Đơn vị khác"
                };
            else if (key == "QuanBinhChung")
                ds = new[] {
                    "Lục quân", "Phòng không - Không quân", "Hải quân",
                    "Thông tin liên lạc", "Tăng thiết giáp", "Pháo binh",
                    "Đặc công", "Công binh", "Hóa học", "Hậu cần - Kỹ thuật",
                    "Công nghệ thông tin", "Cơ quan / Nhà trường", "Khác"
                };
            else if (key == "TrangThaiSSCD")
                ds = new[] { "Chưa xác minh", "Đang hoạt động", "Tạm ngừng", "Đã giải thể" };
            else if (key == "QuanHuyen")
                ds = LayDanhMucPhuongXaHaNoi();

            if (ds != null) c.Items.AddRange(ds);
            return c;
        }

        private static string[] LayDanhMucPhuongXaHaNoi()
        {
            var danhSach = new List<string>(126);
            foreach (string ten in PhuongHaNoi.Split('|'))
                danhSach.Add("Phường " + ten);
            foreach (string ten in XaHaNoi.Split('|'))
                danhSach.Add("Xã " + ten);
            return danhSach.ToArray();
        }

        private void AddFields(Tuple<string, string>[] definitions)
        {
            int rowCount = (definitions.Length + 1) / 2;
            TableLayoutPanel rows = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = rowCount,
                BackColor = NativeLightUi.Background,
                Margin = Padding.Empty
            };
            rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            rows.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            for (int r = 0; r < rowCount; r++)
                rows.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));

            for (int i = 0; i < definitions.Length; i++)
            {
                string key = definitions[i].Item2;
                NativeCard card = NativeLightUi.Card(Color.White);
                card.Margin = new Padding(
                    i % 2 == 0 ? 0 : 6,
                    0,
                    i % 2 == 1 ? 0 : 6,
                    8);
                card.Padding = new Padding(11, 6, 11, 6);

                TableLayoutPanel field = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    BackColor = Color.White,
                    Padding = Padding.Empty
                };
                field.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                field.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
                field.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                field.Controls.Add(NativeLightUi.Text(
                    definitions[i].Item1, 8.5F, true, NativeLightUi.Muted), 0, 0);

                Control input = TaoDauVao(key);
                field.Controls.Add(input, 0, 1);
                card.Controls.Add(field);
                rows.Controls.Add(card, i % 2, i / 2);
                fields[key] = input;
            }
            AddRow(rows, rowCount * 90);
        }

        public void NapDuLieu(DonViQuanDoi d, bool laChinhSua)
        {
            editMode = laChinhSua;
            Put("MaDonVi", d == null ? null : d.MaDonVi);
            Put("TenDonVi", d == null ? null : d.TenDonVi);
            Put("TenVietTat", d == null ? null : d.TenVietTat);
            Put("LoaiDonVi", d == null ? null : d.LoaiDonVi);
            Put("QuanBinhChung", d == null ? null : d.QuanBinhChung);
            Put("DonViCapTren", d == null ? null : d.DonViCapTren);
            Put("QuanHuyen", d == null ? null : d.QuanHuyen);
            Put("DiaChi", d == null ? null : d.DiaChi);
            Put("KinhDo", d == null ? null :
                d.KinhDo.ToString("R", CultureInfo.InvariantCulture));
            Put("ViDo", d == null ? null :
                d.ViDo.ToString("R", CultureInfo.InvariantCulture));
            Put("ChiHuyTruong", d == null ? null : d.ChiHuyTruong);
            Put("ChinhUy", d == null ? null : d.ChinhUy);
            Put("SoDienThoai", d == null ? null : d.SoDienThoai);
            Put("TrangThaiSSCD", d == null ? null : d.TrangThaiSSCD);
            Put("NhiemVuChinh", d == null ? null :
                string.Join("; ", d.NhiemVuChinh ?? new string[0]));
            Put("VuKhiTrangBi", d == null ? null :
                string.Join("; ", d.VuKhiTrangBi ?? new string[0]));

            TextBox ma = fields["MaDonVi"] as TextBox;
            if (ma != null) ma.ReadOnly = laChinhSua;

            BaoTrangThai(laChinhSua
                ? "Đang cập nhật hồ sơ; kiểm tra lại Phường/Xã và thông tin lãnh đạo."
                : "Đang thêm hồ sơ mới; chọn địa danh và kiểm tra tọa độ trước khi lưu.", false);
        }

        public void CapNhatToaDoWgs84(double kinhDo, double viDo)
        {
            if (double.IsNaN(kinhDo) || double.IsInfinity(kinhDo) ||
                double.IsNaN(viDo) || double.IsInfinity(viDo) ||
                kinhDo < -180d || kinhDo > 180d ||
                viDo < -90d || viDo > 90d)
            {
                BaoTrangThai("Tọa độ từ bản đồ không hợp lệ.", true);
                return;
            }

            Put("KinhDo", kinhDo.ToString("F8", CultureInfo.InvariantCulture));
            Put("ViDo", viDo.ToString("F8", CultureInfo.InvariantCulture));
            BaoTrangThai("Đã lấy tọa độ WGS84. Vui lòng kiểm tra và lưu hồ sơ.", false);
        }

        /// <summary>
        /// FormMain goi khi BLL/CSDL tu choi luu, de dong trang thai
        /// tren form khop voi ket qua that.
        /// </summary>
        public void BaoLoiLuu(string text)
        {
            if (IsDisposed || Disposing) return;
            BaoTrangThai(text, true);
        }

        private void BaoTrangThai(string text, bool loi)
        {
            if (message == null) return;
            message.Text = text;
            message.ForeColor = loi
                ? Color.FromArgb(183, 69, 75)
                : NativeLightUi.Teal;
        }

        private void Put(string key, string value)
        {
            Control control;
            if (fields.TryGetValue(key, out control))
                control.Text = value ?? string.Empty;
        }

        private string Read(string key)
        {
            return (fields[key].Text ?? string.Empty).Trim();
        }

        private void ValidateAndSend()
        {
            if (string.IsNullOrWhiteSpace(Read("MaDonVi")) ||
                string.IsNullOrWhiteSpace(Read("TenDonVi")))
            {
                BaoTrangThai("Cần nhập mã đơn vị và tên đơn vị.", true);
                return;
            }

            // Khong tu dong chuyen ten quan/huyen cu thanh phuong/xa:
            // can doi chieu dia chi va ranh gioi moi truoc khi cap nhat.
            if (string.IsNullOrWhiteSpace(Read("QuanHuyen")))
            {
                BaoTrangThai("Vui lòng chọn hoặc nhập Phường/Xã mới đã xác minh.", true);
                return;
            }

            double longitude;
            double latitude;
            if (!ParseCoordinate(Read("KinhDo"), out longitude) ||
                !ParseCoordinate(Read("ViDo"), out latitude) ||
                longitude < -180d || longitude > 180d ||
                latitude < -90d || latitude > 90d)
            {
                BaoTrangThai("Tọa độ WGS84 chưa hợp lệ hoặc chưa nhập đủ.", true);
                return;
            }

            HoSoNhapNative model = new HoSoNhapNative
            {
                LaChinhSua = editMode,
                MaDonVi = Read("MaDonVi"),
                TenDonVi = Read("TenDonVi"),
                TenVietTat = Read("TenVietTat"),
                LoaiDonVi = Read("LoaiDonVi"),
                QuanBinhChung = Read("QuanBinhChung"),
                DonViCapTren = Read("DonViCapTren"),
                QuanHuyen = Read("QuanHuyen"), // DTO/DB cu; UI la Phuong/Xa.
                DiaChi = Read("DiaChi"),
                KinhDo = longitude,
                ViDo = latitude,
                ChiHuyTruong = Read("ChiHuyTruong"),
                ChinhUy = Read("ChinhUy"),
                SoDienThoai = Read("SoDienThoai"),
                TrangThaiSSCD = Read("TrangThaiSSCD"),
                NhiemVuChinh = Read("NhiemVuChinh"),
                VuKhiTrangBi = Read("VuKhiTrangBi")
            };

            Action<HoSoNhapNative> xuLy = YeuCauLuu;
            if (xuLy == null)
            {
                BaoTrangThai("Chưa kết nối hành động lưu với FormMain / BLL.", true);
                return;
            }

            // Dat trang thai TRUOC khi goi: xuLy chay dong bo, neu loi
            // FormMain se goi BaoLoiLuu; neu thanh cong trang se dong.
            BaoTrangThai("Đang lưu hồ sơ...", false);
            xuLy(model);
        }

        private static bool ParseCoordinate(string text, out double result)
        {
            return double.TryParse(text, NumberStyles.Float,
                       CultureInfo.InvariantCulture, out result) ||
                   double.TryParse(text, NumberStyles.Float,
                       NativeLightUi.Vi, out result);
        }
    }
}