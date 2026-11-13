
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    // ============================================================
    // HQTCSDL - QUAN LY DU LIEU DON VI QUAN DOI
    // PHIEN BAN 10.2 - THONG KE VI TRI BAN DO
    //
    // C# 7.3 / .NET Framework 4.8 / WinForms
    //
    // - Giu giao dien 5 cot va 7 nut chuc nang.
    // - Giu bo loc loai don vi va phuong/xa.
    // - Thong nhat nguon HQTCSDL.
    // - Khong cong them 51 ho so cu.
    // - Thay "CHO XAC MINH TRU SO" bang "CO DIEM BAN DO".
    // - Dem vi tri thuc te, khong hardcode 169.
    // - Phan biet diem dai dien voi diem ho so GIS.
    // - Giu cac event ket noi FormMain.
    // - Khong ghi thay doi xuong PostgreSQL.
    // ============================================================

    public sealed class UcQuanLyDuLieuNative : NativePage
    {
        // ========================================================
        // 1. DU LIEU
        // ========================================================

        private readonly List<DanhMucBanChqsCapXa2026> danhMuc =
            new List<DanhMucBanChqsCapXa2026>();

        private readonly List<HoSoGisHqtc> hoSoHqtc =
            new List<HoSoGisHqtc>();

        private readonly Dictionary<string, DonViQuanDoi>
            hoSoDuocQuanLy =
                new Dictionary<string, DonViQuanDoi>(
                    StringComparer.OrdinalIgnoreCase);

        private const string TatCaLoai =
            "Tất cả loại đơn vị";

        private const string TatCaDiaBan =
            "Tất cả phường / xã";

        private const string ChuaXacDinh =
            "Chưa xác định phường/xã";

        private static readonly Color XanhChu =
            Color.FromArgb(25, 54, 69);

        private static readonly Color XanhNgoc =
            Color.FromArgb(0, 166, 145);

        private static readonly Color XanhDuong =
            Color.FromArgb(37, 125, 221);

        private static readonly Color Tim =
            Color.FromArgb(113, 83, 216);

        private static readonly Color Vien =
            Color.FromArgb(218, 232, 238);

        // ========================================================
        // 2. CONTROLS
        // ========================================================

        private readonly TextBox keyword;
        private readonly ComboBox type;
        private readonly ComboBox area;

        private readonly DataGridView table;

        private readonly Label total;
        private readonly Label shown;

        // The thong ke "CO DIEM BAN DO".
        private readonly Label mapped;

        private readonly Label bangThongTin;
        private readonly Label bangChon;

        private readonly Button btnChiTiet;
        private readonly Button btnSua;
        private readonly Button btnDinhVi;
        private readonly Button btnXoa;

        private readonly System.Windows.Forms.Timer delayed;

        private bool dangCapNhatBoLoc;

        // ========================================================
        // 3. SU KIEN KET NOI FORMMAIN
        // ========================================================

        public event Action YeuCauThem;
        public event Action YeuCauTaiLai;

        public event Action<string> YeuCauSua;
        public event Action<string> YeuCauXoa;
        public event Action<string> YeuCauChiTiet;
        public event Action<string> YeuCauDinhVi;
        /// <summary>(lenh, tham so): nhap_toa_do | xuat_gis | phieu_ho_so (ma) | lich_su (ma|ten).</summary>
        public event Action<string, string> YeuCauCongCuGis;

        /// <summary>Mở nhật ký thay đổi; null = mọi đơn vị.</summary>
        public event Action<string> YeuCauNhatKy;

        public event Action<string, double, double, string>
            YeuCauDinhViDiemDaiDien;

        public event Action<long, double, double, string>
            YeuCauDinhViHoSoHqtc;

        // ========================================================
        // 4. KHOI TAO
        // ========================================================

        public UcQuanLyDuLieuNative()
        {
            Name = "UcQuanLyDuLieuNative";

            CauHinhTrangKhongCuon();

            delayed = new System.Windows.Forms.Timer
            {
                Interval = 230
            };

            delayed.Tick += (s, e) =>
            {
                delayed.Stop();
                LocDuLieu();
            };

            // ====================================================
            // 4.1. BANNER
            // ====================================================

            AddRow(TaoBanner(), 70);

            // ====================================================
            // 4.2. BO LOC
            // ====================================================

            NativeCard filter =
                TienIchGiaoDienSang.Card();

            filter.Margin =
                new Padding(0, 0, 0, 5);

            filter.Padding =
                new Padding(12, 6, 12, 6);

            TableLayoutPanel filterGrid =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RowCount = 2,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty,
                    BackColor = Color.White
                };

            filterGrid.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50F));

            filterGrid.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25F));

            filterGrid.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25F));

            filterGrid.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 22F));

            filterGrid.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            filterGrid.Controls.Add(
                TienIchGiaoDienSang.Text(
                    "TÌM KIẾM MÃ / TÊN ĐƠN VỊ",
                    8.2F,
                    true),
                0, 0);

            filterGrid.Controls.Add(
                TienIchGiaoDienSang.Text(
                    "LOẠI ĐƠN VỊ",
                    8.2F,
                    true),
                1, 0);

            filterGrid.Controls.Add(
                TienIchGiaoDienSang.Text(
                    "PHƯỜNG / XÃ",
                    8.2F,
                    true),
                2, 0);

            keyword = TienIchGiaoDienSang.Input();
            type = TienIchGiaoDienSang.Select();
            area = TienIchGiaoDienSang.Select();

            filterGrid.Controls.Add(keyword, 0, 1);
            filterGrid.Controls.Add(type, 1, 1);
            filterGrid.Controls.Add(area, 2, 1);

            filter.Controls.Add(filterGrid);

            AddRow(filter, 78);

            // ====================================================
            // 4.3. BA THE THONG KE
            // ====================================================

            TableLayoutPanel metrics =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RowCount = 1,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty
                };

            for (int i = 0; i < 3; i++)
            {
                metrics.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent,
                        100F / 3F));
            }

            metrics.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent,
                    100F));

            // The 1: Tong so don vi hien hanh.
            total = TaoChiSoGon(
                metrics,
                0,
                "TỔNG ĐƠN VỊ",
                XanhNgoc,
                Color.FromArgb(224, 253, 240));

            // The 2: So don vi khop bo loc.
            shown = TaoChiSoGon(
                metrics,
                1,
                "KẾT QUẢ BỘ LỌC",
                XanhDuong,
                Color.FromArgb(224, 240, 255));

            // The 3: So don vi co diem tren ban do.
            mapped = TaoChiSoGon(
                metrics,
                2,
                "CÓ ĐIỂM BẢN ĐỒ",
                Tim,
                Color.FromArgb(239, 229, 255));

            ToolTip thongTinViTri = new ToolTip();

            thongTinViTri.SetToolTip(
                mapped,
                "Đếm số đơn vị có điểm bản đồ.\n" +
                "Ban CHQS phường/xã: điểm đại diện địa bàn.\n" +
                "Hồ sơ GIS: điểm của đơn vị.\n" +
                "Không đồng nghĩa đã xác minh trụ sở thực tế.");

            AddRow(metrics, 66);

            // ====================================================
            // 4.4. TIEU DE BANG
            // ====================================================

            AddRow(
                TienIchGiaoDienSang.SectionTitle(
                    "DANH SÁCH ĐƠN VỊ QUÂN ĐỘI",
                    " "),
                29);

            // ====================================================
            // 4.5. KHUNG CHINH
            // ====================================================

            NativeCard card =
                TienIchGiaoDienSang.Card();

            card.Margin = Padding.Empty;
            card.Padding =
                new Padding(9, 8, 9, 7);

            TableLayoutPanel tableHost =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty,
                    BackColor = Color.White
                };

            tableHost.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            tableHost.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 34F));

            tableHost.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            tableHost.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 54F));

            // ====================================================
            // 4.6. THANH THONG TIN BANG
            // ====================================================

            Panel tableToolbar = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                BackColor = Color.White
            };

            bangThongTin = new Label
            {
                Dock = DockStyle.Left,
                Width = 320,
                AutoSize = false,
                Text = "ĐANG HIỂN THỊ 0 HỒ SƠ",
                ForeColor = XanhChu,
                Font = new Font(
                    "Segoe UI",
                    8.8F,
                    FontStyle.Bold),
                TextAlign =
                    ContentAlignment.MiddleLeft,
                Padding = new Padding(4, 0, 0, 0)
            };

            bangChon = new Label
            {
                Dock = DockStyle.Right,
                Width = 340,
                AutoSize = false,
                Text = "Chọn một dòng để thao tác",
                ForeColor =
                    Color.FromArgb(110, 130, 143),
                Font = new Font(
                    "Segoe UI",
                    8.4F),
                TextAlign =
                    ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 5, 0)
            };

            tableToolbar.Controls.Add(bangThongTin);
            tableToolbar.Controls.Add(bangChon);

            tableHost.Controls.Add(tableToolbar, 0, 0);

            // ====================================================
            // 4.7. KHUNG BANG
            // ====================================================

            Panel tableFrame = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = new Padding(1),
                BackColor = Vien,
                BorderStyle = BorderStyle.None
            };

            table = TienIchGiaoDienSang.Table();

            table.Dock = DockStyle.Fill;
            table.Margin = Padding.Empty;

            table.BackgroundColor = Color.White;
            table.BorderStyle = BorderStyle.None;

            table.AutoGenerateColumns = false;
            table.Columns.Clear();

            table.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            table.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.None;

            table.RowTemplate.Height = 39;
            table.ColumnHeadersHeight = 42;

            table.ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode
                    .DisableResizing;

            table.RowHeadersVisible = false;

            table.AllowUserToAddRows = false;
            table.AllowUserToDeleteRows = false;
            table.AllowUserToResizeRows = false;
            table.AllowUserToOrderColumns = false;

            table.ReadOnly = true;
            table.MultiSelect = false;

            table.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            table.ScrollBars = ScrollBars.Vertical;
            table.ShowCellToolTips = true;

            table.EnableHeadersVisualStyles = false;

            table.CellBorderStyle =
                DataGridViewCellBorderStyle.Single;

            table.ColumnHeadersBorderStyle =
                DataGridViewHeaderBorderStyle.None;

            table.GridColor =
                Color.FromArgb(228, 236, 241);

            // ====================================================
            // HEADER
            // ====================================================

            table.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(238, 247, 249);

            table.ColumnHeadersDefaultCellStyle.ForeColor =
                XanhChu;

            table.ColumnHeadersDefaultCellStyle
                .SelectionBackColor =
                Color.FromArgb(238, 247, 249);

            table.ColumnHeadersDefaultCellStyle
                .SelectionForeColor = XanhChu;

            table.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    8.8F,
                    FontStyle.Bold);

            table.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            table.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(10, 0, 2, 0);

            // ====================================================
            // HANG DU LIEU
            // ====================================================

            table.DefaultCellStyle.Font =
                new Font("Segoe UI", 8.8F);

            table.DefaultCellStyle.ForeColor =
                Color.FromArgb(37, 61, 73);

            table.DefaultCellStyle.BackColor =
                Color.White;

            table.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(215, 246, 238);

            table.DefaultCellStyle.SelectionForeColor =
                Color.FromArgb(9, 73, 65);

            table.DefaultCellStyle.Padding =
                new Padding(10, 0, 4, 0);

            table.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            table.DefaultCellStyle.WrapMode =
                DataGridViewTriState.False;

            table.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 251, 251);

            // ====================================================
            // NAM COT CO GIAN
            // ====================================================

            table.Columns.Add(
                TaoCotBang(
                    "MÃ ĐƠN VỊ",
                    "MaDonVi",
                    13F,
                    92));

            table.Columns.Add(
                TaoCotBang(
                    "TÊN ĐƠN VỊ",
                    "TenDonVi",
                    35F,
                    230));

            table.Columns.Add(
                TaoCotBang(
                    "LOẠI ĐƠN VỊ",
                    "LoaiDonVi",
                    17F,
                    112));

            table.Columns.Add(
                TaoCotBang(
                    "ĐỊA BÀN",
                    "DiaBan",
                    17F,
                    112));

            table.Columns.Add(
                TaoCotBang(
                    "TRẠNG THÁI",
                    "TrangThai",
                    18F,
                    143));

            table.CellFormatting +=
                Bang_DinhDangTrangThai;

            table.CellToolTipTextNeeded +=
                Bang_HienThiNoiDungDayDu;

            // Bat lai tooltip (da tat khi nap lai luoi) khi chuot
            // vao mot o that; luoi trong thi khong co o nao de vao.
            table.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && !table.ShowCellToolTips)
                    table.ShowCellToolTips = true;
            };

            tableFrame.Controls.Add(table);
            tableHost.Controls.Add(tableFrame, 0, 1);

            // ====================================================
            // 4.8. NUT CHUC NANG
            // ====================================================

            Button btnThem =
                TienIchGiaoDienSang.Button(
                    "+ Thêm đơn vị",
                    TienIchGiaoDienSang.Teal,
                    120);

            Button btnTaiLai =
                TienIchGiaoDienSang.Button(
                    "↻ Làm mới",
                    TienIchGiaoDienSang.Purple,
                    100);

            Button btnXuat =
                TienIchGiaoDienSang.Button(
                    "⇩ Xuất CSV",
                    TienIchGiaoDienSang.Purple,
                    114);

            Button btnNhatKy =
                TienIchGiaoDienSang.Button(
                    "☷ Nhật ký",
                    TienIchGiaoDienSang.Purple,
                    100);

            // Loi tat cong cu GIS: nhap toa do, xuat GIS, phieu ho so.
            Button btnGis =
                TienIchGiaoDienSang.Button(
                    "⚒ GIS ▾",
                    Color.FromArgb(52, 72, 98),
                    86);
            ContextMenuStrip menuGis = new ContextMenuStrip { ShowImageMargin = false, Font = new Font("Segoe UI", 9.5F) };
            Action<string, string> lenhGis = (lenh, ma) =>
            {
                Action<string, string> h = YeuCauCongCuGis;
                if (h != null) h(lenh, ma);
            };
            menuGis.Items.Add("Nhập tọa độ trụ sở GNSS / CSV / KML…", null, (s, e) => lenhGis("nhap_toa_do", null));
            menuGis.Items.Add("Xuất dữ liệu GIS (GeoJSON · KML · SHP · CSV)…", null, (s, e) => lenhGis("xuat_gis", null));
            ToolStripItem mucPhieu = menuGis.Items.Add("In phiếu hồ sơ đơn vị đang chọn…", null, (s, e) =>
            {
                Dong r = DongDuocChon();
                if (r != null) lenhGis("phieu_ho_so", r.MaDonVi);
            });
            ToolStripItem mucLichSu = menuGis.Items.Add("Lịch sử vị trí đơn vị đang chọn…", null, (s, e) =>
            {
                Dong r = DongDuocChon();
                if (r != null) lenhGis("lich_su", r.MaDonVi + "|" + r.TenDonVi);
            });
            menuGis.Opening += (s, e) => mucPhieu.Enabled = mucLichSu.Enabled = DongDuocChon() != null;
            btnGis.Click += (s, e) => menuGis.Show(btnGis, new System.Drawing.Point(0, -menuGis.PreferredSize.Height - 2));

            btnChiTiet =
                TienIchGiaoDienSang.Button(
                    "▤ Chi tiết",
                    TienIchGiaoDienSang.Blue,
                    98);

            btnSua =
                TienIchGiaoDienSang.Button(
                    "✎ Chỉnh sửa",
                    TienIchGiaoDienSang.Blue,
                    108);

            btnDinhVi =
                TienIchGiaoDienSang.Button(
                    "⌖ Định vị",
                    TienIchGiaoDienSang.Teal,
                    94);

            btnXoa =
                TienIchGiaoDienSang.Button(
                    "× Xóa",
                    TienIchGiaoDienSang.Amber,
                    78);

            TableLayoutPanel actionBar =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Margin = Padding.Empty,
                    Padding = new Padding(0, 5, 0, 0),
                    BackColor = Color.White
                };

            actionBar.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 44F));

            actionBar.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 56F));

            actionBar.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            FlowLayoutPanel leftActions =
                TaoNhomNut(false);

            FlowLayoutPanel rightActions =
                TaoNhomNut(true);

            foreach (Button b in new[]
            {
                btnThem,
                btnTaiLai,
                btnXuat,
                btnNhatKy,
                btnGis
            })
            {
                b.Height = 33;
                b.Margin = new Padding(0, 1, 7, 1);
                leftActions.Controls.Add(b);
            }

            foreach (Button b in new[]
            {
                btnXoa,
                btnDinhVi,
                btnSua,
                btnChiTiet
            })
            {
                b.Height = 33;
                b.Margin = new Padding(7, 1, 0, 1);
                rightActions.Controls.Add(b);
            }

            actionBar.Controls.Add(leftActions, 0, 0);
            actionBar.Controls.Add(rightActions, 1, 0);

            tableHost.Controls.Add(actionBar, 0, 2);

            // Cua so hep: hai nhom nut khong vua mot hang thi
            // xep thanh hai hang (khong de nut Xoa tran / bi cat).
            RowStyle hangNut = tableHost.RowStyles[2];
            bool haiHang = false;

            actionBar.RowStyles.Add(
                new RowStyle(SizeType.Percent, 50F));

            EventHandler canChinhNut = (s, e) =>
            {
                int canTrai = leftActions.Controls
                    .Cast<Control>()
                    .Sum(c => c.Width + c.Margin.Horizontal);

                int canPhai = rightActions.Controls
                    .Cast<Control>()
                    .Sum(c => c.Width + c.Margin.Horizontal);

                bool canHaiHang =
                    actionBar.ClientSize.Width > 0 &&
                    actionBar.ClientSize.Width <
                        canTrai + canPhai + 24;

                // Mot hang: cot trai rong DUNG bang nhom nut trai
                // (khong dung ty le co dinh 44% - gay cat nut o 1360px).
                // Hai hang: moi nhom chiem ca chieu ngang.
                SizeType kieuTrai = canHaiHang
                    ? SizeType.Percent : SizeType.Absolute;
                float rongTrai = canHaiHang ? 100F : canTrai + 4;

                if (canHaiHang == haiHang &&
                    actionBar.ColumnStyles[0].SizeType == kieuTrai &&
                    Math.Abs(actionBar.ColumnStyles[0].Width - rongTrai) < 0.5F)
                {
                    return;
                }

                haiHang = canHaiHang;

                actionBar.SuspendLayout();
                tableHost.SuspendLayout();

                try
                {
                    actionBar.RowCount = haiHang ? 2 : 1;

                    actionBar.ColumnStyles[0].SizeType = kieuTrai;
                    actionBar.ColumnStyles[0].Width = rongTrai;

                    actionBar.ColumnStyles[1].SizeType = haiHang
                        ? SizeType.Absolute : SizeType.Percent;
                    actionBar.ColumnStyles[1].Width = haiHang ? 0F : 100F;

                    actionBar.RowStyles[0].Height = haiHang ? 50F : 100F;

                    actionBar.SetCellPosition(
                        rightActions,
                        haiHang
                            ? new TableLayoutPanelCellPosition(0, 1)
                            : new TableLayoutPanelCellPosition(1, 0));

                    // Giu RightToLeft o ca hai che do: nut "Xoa"
                    // (nguy hiem) luon nam cuoi ben phai.
                    hangNut.Height = haiHang ? 92F : 54F;
                }
                finally
                {
                    actionBar.ResumeLayout(true);
                    tableHost.ResumeLayout(true);
                }
            };

            actionBar.Resize += canChinhNut;

            card.Controls.Add(tableHost);

            AddRow(card, 1);

            RowStyle hangBang =
                Body.RowStyles[Body.RowCount - 1];

            hangBang.SizeType = SizeType.Percent;
            hangBang.Height = 100F;

            // ====================================================
            // 4.9. SU KIEN
            // ====================================================

            btnThem.Click += (s, e) =>
                MoThemDonVi();

            btnTaiLai.Click += (s, e) =>
                TaiLaiTatCa();

            btnXuat.Click += (s, e) =>
                XuatCsv();

            btnNhatKy.Click += (s, e) =>
            {
                // Dang chon ho so quan ly thi loc theo ma do,
                // nguoc lai xem nhat ky cua moi don vi.
                Dong row = DongDuocChon();

                Action<string> handler = YeuCauNhatKy;

                if (handler != null)
                {
                    handler(
                        row != null && row.LaHoSoChinh
                            ? row.MaDonVi
                            : null);
                }
            };

            btnChiTiet.Click += (s, e) =>
                MoChiTiet();

            btnSua.Click += (s, e) =>
                GuiMaHoSo(YeuCauSua);

            btnXoa.Click += (s, e) =>
                GuiMaHoSo(YeuCauXoa);

            btnDinhVi.Click += (s, e) =>
                DinhViBanGhi();

            table.SelectionChanged += (s, e) =>
                CapNhatNut();

            table.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                    MoChiTiet();
            };

            keyword.TextChanged += (s, e) =>
                HenLoc();

            type.SelectedIndexChanged += (s, e) =>
                HenLoc();

            area.SelectedIndexChanged += (s, e) =>
                HenLoc();

            keyword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    delayed.Stop();
                    LocDuLieu();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            };

            CapNhatBoLoc();
        }

        // ========================================================
        // 5. NAP DU LIEU - TUONG THICH PHIEN BAN CU
        //
        // Khong dua 51 ho so cu vao danh sach hien hanh.
        // ========================================================

        public void NapDuLieu(
            IEnumerable<DonViQuanDoi> items)
        {
            if (IsDisposed || Disposing)
                return;

            danhMuc.Clear();
            hoSoHqtc.Clear();
            hoSoDuocQuanLy.Clear();

            CapNhatBoLoc();
        }

        // ========================================================
        // 6. NAP DU LIEU HIEN HANH TU FORMMAIN
        //
        // Nguon 1: Danh muc Ban CHQS phuong/xa (chi doc).
        // Nguon 2: Ho so GIS quan doi cac cap (chi doc).
        // Nguon 3: Ho so chinh don_vi_quan_doi (Sua/Xoa).
        // ========================================================

        public void NapDuLieu(
            IEnumerable<DonViQuanDoi> hoSoCu,
            IEnumerable<DanhMucBanChqsCapXa2026> danhMucMoi,
            IEnumerable<HoSoGisHqtc> hoSoMoi)
        {
            if (IsDisposed || Disposing)
                return;

            danhMuc.Clear();
            hoSoHqtc.Clear();
            hoSoDuocQuanLy.Clear();

            if (hoSoCu != null)
            {
                foreach (DonViQuanDoi d in hoSoCu)
                {
                    if (d == null ||
                        string.IsNullOrWhiteSpace(d.MaDonVi))
                    {
                        continue;
                    }

                    string ma = d.MaDonVi.Trim();

                    if (!hoSoDuocQuanLy.ContainsKey(ma))
                        hoSoDuocQuanLy.Add(ma, d);
                }
            }

            if (danhMucMoi != null)
            {
                danhMuc.AddRange(
                    danhMucMoi
                        .Where(d =>
                            d != null &&
                            !string.IsNullOrWhiteSpace(
                                d.MaDvhc))
                        .GroupBy(
                            d => d.MaDvhc.Trim(),
                            StringComparer.OrdinalIgnoreCase)
                        .Select(g => g.First())
                        .OrderBy(
                            d => d.TenDonVi,
                            StringComparer.CurrentCultureIgnoreCase));
            }

            if (hoSoMoi != null)
            {
                hoSoHqtc.AddRange(
                    hoSoMoi
                        .Where(d =>
                            d != null &&
                            !string.IsNullOrWhiteSpace(
                                d.MaDonVi))
                        .GroupBy(d => d.Id)
                        .Select(g => g.First())
                        .OrderBy(
                            d => d.TenDonVi,
                            StringComparer.CurrentCultureIgnoreCase));
            }

            CapNhatBoLoc();
        }

        // ========================================================
        // 7. CAP NHAT BO LOC
        // ========================================================

        private void CapNhatBoLoc()
        {
            if (IsDisposed || Disposing)
                return;

            string prevType = type.SelectedItem as string;
            string prevArea = area.SelectedItem as string;

            List<Dong> tatCa = LayDuLieuHienHanh();

            dangCapNhatBoLoc = true;

            type.BeginUpdate();
            area.BeginUpdate();

            try
            {
                type.Items.Clear();
                area.Items.Clear();

                type.Items.Add(TatCaLoai);
                area.Items.Add(TatCaDiaBan);

                foreach (string s in tatCa
                    .Select(x => x.LoaiDonVi)
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct(
                        StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(
                        x => x,
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    type.Items.Add(s);
                }

                foreach (string s in tatCa
                    .Select(x => x.DiaBan)
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(x))
                    .Distinct(
                        StringComparer.CurrentCultureIgnoreCase)
                    .OrderBy(
                        x => x,
                        StringComparer.CurrentCultureIgnoreCase))
                {
                    area.Items.Add(s);
                }

                type.SelectedIndex =
                    prevType != null &&
                    type.Items.Contains(prevType)
                    ? type.Items.IndexOf(prevType)
                    : 0;

                area.SelectedIndex =
                    prevArea != null &&
                    area.Items.Contains(prevArea)
                    ? area.Items.IndexOf(prevArea)
                    : 0;
            }
            finally
            {
                type.EndUpdate();
                area.EndUpdate();
                dangCapNhatBoLoc = false;
            }

            LocDuLieu();
        }

        // ========================================================
        // 8. LAY HAI NGUON DU LIEU HIEN HANH
        // ========================================================

        private List<Dong> LayDuLieuHienHanh()
        {
            List<Dong> rows =
                new List<Dong>(
                    danhMuc.Count +
                    hoSoHqtc.Count +
                    hoSoDuocQuanLy.Count);

            HashSet<string> maDaHien =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            // Giai doan 3: moi Ban CHQS da co ho so trong HQTCSDL
            // voi ma "HN-BCHQS-<ma DVHC>" -> dong nay sua/xoa duoc,
            // va khong hien lan nua o nguon 3 (het dem trung 220).
            foreach (DanhMucBanChqsCapXa2026 d in danhMuc)
            {
                if (d == null)
                    continue;

                string maHoSo =
                    "HN-BCHQS-" + (d.MaDvhc ?? "").Trim();

                bool coHoSoGhi =
                    hoSoDuocQuanLy.ContainsKey(maHoSo);

                if (coHoSoGhi)
                    maDaHien.Add(maHoSo);

                rows.Add(new Dong(d, coHoSoGhi ? maHoSo : null));
            }

            foreach (HoSoGisHqtc d in hoSoHqtc)
            {
                if (d == null)
                    continue;

                bool coHoSoGhi =
                    !string.IsNullOrWhiteSpace(d.MaDonVi) &&
                    hoSoDuocQuanLy.ContainsKey(
                        d.MaDonVi.Trim());

                if (coHoSoGhi)
                    maDaHien.Add(d.MaDonVi.Trim());

                rows.Add(new Dong(d, coHoSoGhi));
            }

            // Nguon 3: ho so chinh public.don_vi_quan_doi
            // (doc/ghi qua BLL). Day la nguon duy nhat
            // Sua/Xoa duoc, nen phai hien tren luoi;
            // bo qua ma da hien qua nguon HQTCSDL.
            foreach (DonViQuanDoi d in hoSoDuocQuanLy.Values)
            {
                if (d != null &&
                    !maDaHien.Contains(d.MaDonVi.Trim()))
                {
                    rows.Add(new Dong(d));
                }
            }

            return rows;
        }

        // ========================================================
        // 9. LOC DU LIEU VA CAP NHAT BA THE THONG KE
        // ========================================================

        private void HenLoc()
        {
            if (dangCapNhatBoLoc ||
                IsDisposed ||
                Disposing)
            {
                return;
            }

            delayed.Stop();
            delayed.Start();
        }

        private void LocDuLieu()
        {
            if (dangCapNhatBoLoc ||
                IsDisposed ||
                Disposing)
            {
                return;
            }

            string key = keyword.Text.Trim();

            string loai =
                type.SelectedIndex > 0
                    ? type.SelectedItem as string
                    : null;

            string diaBan =
                area.SelectedIndex > 0
                    ? area.SelectedItem as string
                    : null;

            List<Dong> tatCa = LayDuLieuHienHanh();

            List<Dong> rows = tatCa
                .Where(d =>
                {
                    bool khopLoai =
                        loai == null ||
                        string.Equals(
                            d.LoaiDonVi,
                            loai,
                            StringComparison
                                .CurrentCultureIgnoreCase);

                    bool khopDiaBan =
                        diaBan == null ||
                        string.Equals(
                            d.DiaBan,
                            diaBan,
                            StringComparison
                                .CurrentCultureIgnoreCase);

                    bool khopTuKhoa =
                        key.Length == 0 ||
                        Khop(d.MaDonVi, key) ||
                        Khop(d.TenDonVi, key) ||
                        Khop(d.LoaiDonVi, key) ||
                        Khop(d.DiaBan, key) ||
                        Khop(d.MaXaPhuong, key) ||
                        Khop(d.TenCap, key);

                    return
                        khopLoai &&
                        khopDiaBan &&
                        khopTuKhoa;
                })
                .OrderBy(
                    d => d.TenDonVi,
                    StringComparer.CurrentCultureIgnoreCase)
                .ThenBy(
                    d => d.MaDonVi,
                    StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            // ====================================================
            // THE 1: TONG SO DON VI
            // ====================================================

            total.Text = tatCa.Count.ToString(
                "N0",
                TienIchGiaoDienSang.Vi);

            // ====================================================
            // THE 2: KET QUA TIM KIEM / BO LOC
            // ====================================================

            shown.Text = rows.Count.ToString(
                "N0",
                TienIchGiaoDienSang.Vi);

            // ====================================================
            // THE 3: CO DIEM BAN DO
            //
            // Ban CHQS:
            //    CoDiemDaiDien = true.
            //
            // Ho so GIS cac cap:
            //    CoDiemBanDo = true.
            //
            // Khong nham lan voi toa do tru so da xac minh.
            // ====================================================

            int soCoDiemBanDo =
                tatCa.Count(d => d.CoDiemBanDo);

            mapped.Text = soCoDiemBanDo.ToString(
                "N0",
                TienIchGiaoDienSang.Vi);

            // ====================================================
            // THANH THONG TIN BANG
            // ====================================================

            bangThongTin.Text =
                "ĐANG HIỂN THỊ " +
                rows.Count.ToString(
                    "N0",
                    TienIchGiaoDienSang.Vi) +
                " / " +
                tatCa.Count.ToString(
                    "N0",
                    TienIchGiaoDienSang.Vi) +
                " HỒ SƠ";

            // Ghi nho dong dang chon.
            Dong daChon = DongDuocChon();

            string maDaChon =
                daChon == null
                    ? null
                    : daChon.MaDonVi;

            table.SuspendLayout();

            try
            {
                // An tooltip cua dong vua bi xoa: DataGridView giu van
                // ban tooltip cu va hien lai ngay khi bat lai (ke ca
                // bat lai bang BeginInvoke). Vi vay tat o day va CHI bat
                // lai khi chuot thuc su vao mot o (CellMouseEnter).
                table.ShowCellToolTips = false;

                table.DataSource = null;

                table.DataSource =
                    new BindingList<Dong>(rows);

                table.ClearSelection();
                table.CurrentCell = null;

                if (!string.IsNullOrWhiteSpace(maDaChon))
                {
                    foreach (DataGridViewRow r in table.Rows)
                    {
                        Dong d =
                            r.DataBoundItem as Dong;

                        if (d != null &&
                            string.Equals(
                                d.MaDonVi,
                                maDaChon,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            r.Selected = true;
                            table.CurrentCell = r.Cells[0];
                            break;
                        }
                    }
                }
            }
            finally
            {
                table.ResumeLayout(true);
            }

            CapNhatNut();
        }

        // ========================================================
        // 10. TIM KIEM TIENG VIET
        // ========================================================

        private static bool Khop(
            string value,
            string key)
        {
            return TienIchGiaoDienSang.Vi
                .CompareInfo.IndexOf(
                    value ?? "",
                    key,
                    CompareOptions.IgnoreCase |
                    CompareOptions.IgnoreNonSpace) >= 0;
        }

        // ========================================================
        // 11. TAO COT BANG
        // ========================================================

        private static DataGridViewTextBoxColumn TaoCotBang(
            string tieuDe,
            string tenThuocTinh,
            float tyLe,
            int doRongToiThieu)
        {
            DataGridViewTextBoxColumn col =
                new DataGridViewTextBoxColumn
                {
                    Name = tenThuocTinh,
                    HeaderText = tieuDe,
                    DataPropertyName = tenThuocTinh,

                    SortMode =
                        DataGridViewColumnSortMode.NotSortable,

                    AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill,

                    FillWeight = tyLe,
                    MinimumWidth = doRongToiThieu,
                    ReadOnly = true
                };

            col.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment =
                        DataGridViewContentAlignment.MiddleLeft,
                    NullValue = ""
                };

            return col;
        }

        // ========================================================
        // 12. MAU SAC TRANG THAI
        // ========================================================

        private void Bang_DinhDangTrangThai(
            object sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                table.Columns[e.ColumnIndex].Name !=
                    "TrangThai")
            {
                return;
            }

            string value =
                Convert.ToString(e.Value) ?? "";

            if (value.Contains("Điểm đại diện"))
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(0, 146, 126);
            }
            else if (value.Contains("Điểm đơn vị"))
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(37, 125, 221);
            }
            else if (value.Contains("● Hồ sơ quản lý"))
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(113, 83, 216);
            }
            else
            {
                e.CellStyle.ForeColor =
                    Color.FromArgb(169, 105, 23);
            }
        }

        // ========================================================
        // 13. TOOLTIP
        // ========================================================

        private void Bang_HienThiNoiDungDayDu(
            object sender,
            DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0 ||
                e.ColumnIndex < 0 ||
                e.RowIndex >= table.Rows.Count)
            {
                return;
            }

            Dong dong =
                table.Rows[e.RowIndex]
                    .DataBoundItem as Dong;

            if (dong == null)
                return;

            if (table.Columns[e.ColumnIndex].Name ==
                "TrangThai")
            {
                if (dong.LaDanhMuc)
                {
                    e.ToolTipText =
                        "Điểm đại diện địa bàn phường/xã.\n" +
                        "Không phải vị trí trụ sở đã xác minh.";
                }
                else if (dong.LaHoSoChinh)
                {
                    e.ToolTipText =
                        "Hồ sơ quản lý (public.don_vi_quan_doi).\n" +
                        "Được Sửa / Xóa; mọi thay đổi ghi nhật ký.";
                }
                else
                {
                    e.ToolTipText =
                        dong.CoDiemBanDo
                        ? "Có điểm hồ sơ GIS trên bản đồ."
                        : "Chưa có điểm GIS hợp lệ.";
                }

                return;
            }

            object value =
                table.Rows[e.RowIndex]
                    .Cells[e.ColumnIndex].Value;

            e.ToolTipText =
                Convert.ToString(value) ?? "";
        }

        // ========================================================
        // 14. DONG DUOC CHON
        // ========================================================

        private Dong DongDuocChon()
        {
            return
                table.CurrentRow != null &&
                table.CurrentRow.Selected
                ? table.CurrentRow.DataBoundItem as Dong
                : null;
        }

        // ========================================================
        // 15. CAP NHAT NUT
        // ========================================================

        private void CapNhatNut()
        {
            Dong row = DongDuocChon();

            if (row == null)
            {
                bangChon.Text =
                    "Chọn một dòng để thao tác";

                btnChiTiet.Enabled = false;
                btnSua.Enabled = false;
                btnXoa.Enabled = false;
                btnDinhVi.Enabled = false;

                return;
            }

            bangChon.Text =
                "Đang chọn: " + row.MaDonVi +
                (row.CoTheChinhSua ? "" : " (chỉ đọc)");

            btnChiTiet.Enabled = true;

            // Nut ve tuy bien khong hien ro trang thai Disabled,
            // nen de bat va GuiMaHoSo giai thich khi dong chi doc.
            btnSua.Enabled = true;
            btnXoa.Enabled = true;

            btnDinhVi.Enabled = row.CoDiemBanDo;
        }

        // ========================================================
        // 16. THEM, NAP LAI, SUA, XOA
        // ========================================================

        private void MoThemDonVi()
        {
            Action handler = YeuCauThem;

            if (handler != null)
                handler();
        }

        private void TaiLaiTatCa()
        {
            Action handler = YeuCauTaiLai;

            if (handler != null)
                handler();
        }

        private void GuiMaHoSo(
            Action<string> handler)
        {
            Dong row = DongDuocChon();

            if (row == null ||
                string.IsNullOrWhiteSpace(row.MaDonVi))
            {
                return;
            }

            // Khong im lang khi bam vao dong chi doc.
            if (!row.CoTheChinhSua)
            {
                MessageBox.Show(
                    this,
                    "Bản ghi " + row.MaDonVi + " thuộc nguồn chỉ đọc " +
                    "(danh mục Ban CHQS / hồ sơ GIS HQTCSDL), " +
                    "không sửa hoặc xóa được tại đây.\n\n" +
                    "Hãy chọn dòng có trạng thái \"Hồ sơ quản lý\".",
                    "Bản ghi chỉ đọc",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            if (handler != null)
                handler(row.MaDonVi);
        }

        // ========================================================
        // 17. DINH VI BAN GHI
        // ========================================================

        private void DinhViBanGhi()
        {
            Dong row = DongDuocChon();

            if (row == null)
                return;

            // Ho so chinh: dinh vi theo ma qua FormMain/BLL.
            if (row.LaHoSoChinh)
            {
                Action<string> dinhVi = YeuCauDinhVi;

                if (dinhVi != null)
                    dinhVi(row.MaDonVi);
                else
                    ThongBaoChuaLienKetBanDo();

                return;
            }

            // Ban CHQS phuong/xa.
            if (row.LaDanhMuc &&
                row.BanChqs != null &&
                row.BanChqs.CoDiemDaiDien &&
                row.BanChqs.XDaiDien32648.HasValue &&
                row.BanChqs.YDaiDien32648.HasValue)
            {
                Action<string, double, double, string>
                    handler = YeuCauDinhViDiemDaiDien;

                if (handler != null)
                {
                    handler(
                        row.BanChqs.MaDvhc,
                        row.BanChqs.XDaiDien32648.Value,
                        row.BanChqs.YDaiDien32648.Value,
                        row.BanChqs.TenDonVi);
                }
                else
                {
                    ThongBaoChuaLienKetBanDo();
                }

                return;
            }

            // Ho so GIS quan doi cac cap.
            if (row.LaHoSoHqtc &&
                row.HoSoHqtc != null &&
                row.HoSoHqtc.CoDiemBanDo &&
                row.HoSoHqtc.X32648.HasValue &&
                row.HoSoHqtc.Y32648.HasValue)
            {
                Action<long, double, double, string>
                    handler = YeuCauDinhViHoSoHqtc;

                if (handler != null)
                {
                    handler(
                        row.HoSoHqtc.Id,
                        row.HoSoHqtc.X32648.Value,
                        row.HoSoHqtc.Y32648.Value,
                        row.HoSoHqtc.TenDonVi);
                }
                else
                {
                    ThongBaoChuaLienKetBanDo();
                }

                return;
            }

            MessageBox.Show(
                this,
                "Bản ghi chưa có tọa độ hợp lệ để định vị.",
                "Thông tin vị trí",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ThongBaoChuaLienKetBanDo()
        {
            MessageBox.Show(
                this,
                "Chưa liên kết sự kiện định vị " +
                "từ Quản lý dữ liệu tới FormMain.",
                "Định vị bản đồ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        // ========================================================
        // 18. CHI TIET DON VI
        // ========================================================

        private void MoChiTiet()
        {
            Dong row = DongDuocChon();

            if (row == null)
                return;

            if (row.CoTheChinhSua &&
                YeuCauChiTiet != null)
            {
                YeuCauChiTiet(row.MaDonVi);
                return;
            }

            MoChiTietChiDoc(row);
        }

        private void MoChiTietChiDoc(Dong row)
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Thông tin đơn vị quân đội";

                dlg.StartPosition =
                    FormStartPosition.CenterParent;

                dlg.FormBorderStyle =
                    FormBorderStyle.FixedDialog;

                dlg.MinimizeBox = false;
                dlg.MaximizeBox = false;
                dlg.ShowInTaskbar = false;

                dlg.ClientSize = new Size(700, 480);

                dlg.Font = new Font("Segoe UI", 9.5F);

                dlg.BackColor =
                    Color.FromArgb(245, 249, 252);

                Panel header = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 85,
                    BackColor = XanhNgoc,
                    Padding = new Padding(22, 12, 20, 12)
                };

                Label title = new Label
                {
                    Dock = DockStyle.Fill,
                    AutoSize = false,

                    Text =
                        "THÔNG TIN HỒ SƠ ĐƠN VỊ\n" +
                        row.TenDonVi,

                    ForeColor = Color.White,

                    Font = new Font(
                        "Segoe UI",
                        12.5F,
                        FontStyle.Bold),

                    TextAlign =
                        ContentAlignment.MiddleLeft
                };

                header.Controls.Add(title);

                Panel footer = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 67,
                    BackColor = Color.White,
                    Padding =
                        new Padding(16, 14, 16, 12)
                };

                Button btnDong =
                    TienIchGiaoDienSang.Button(
                        "Đóng",
                        TienIchGiaoDienSang.Blue,
                        100);

                btnDong.Dock = DockStyle.Right;
                btnDong.DialogResult = DialogResult.OK;

                footer.Controls.Add(btnDong);

                TextBox txt = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.White,
                    ForeColor = XanhChu,
                    Font = new Font("Segoe UI", 10F),
                    Text = TaoNoiDungChiTiet(row)
                };

                Panel content = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding =
                        new Padding(23, 22, 23, 18),
                    BackColor = Color.White
                };

                content.Controls.Add(txt);

                dlg.Controls.Add(content);
                dlg.Controls.Add(footer);
                dlg.Controls.Add(header);

                dlg.AcceptButton = btnDong;

                dlg.ShowDialog(this);
            }
        }

        private static string TaoNoiDungChiTiet(Dong row)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("THÔNG TIN CHUNG");
            sb.AppendLine();

            sb.AppendLine(
                "Mã đơn vị: " + row.MaDonVi);

            sb.AppendLine(
                "Tên đơn vị: " + row.TenDonVi);

            sb.AppendLine(
                "Loại đơn vị: " + row.LoaiDonVi);

            sb.AppendLine(
                "Địa bàn: " + row.DiaBan);

            sb.AppendLine(
                "Mã phường/xã: " +
                GiaTri(row.MaXaPhuong));

            sb.AppendLine();
            sb.AppendLine("THÔNG TIN KHÔNG GIAN");
            sb.AppendLine();

            if (row.LaDanhMuc)
            {
                sb.AppendLine(
                    "Nguồn: Danh mục Ban CHQS phường/xã");

                sb.AppendLine(
                    "Loại vị trí: Điểm đại diện hành chính");

                sb.AppendLine(
                    "Tọa độ trụ sở: Chưa xác minh");

                if (row.BanChqs != null)
                {
                    sb.AppendLine(
                        "Trạng thái liên kết: " +
                        GiaTri(
                            row.BanChqs.TrangThaiLienKet));

                    sb.AppendLine(
                        "Trạng thái xác minh: " +
                        GiaTri(
                            row.BanChqs.TinhTrangXacMinh));
                }
            }
            else
            {
                sb.AppendLine(
                    "Nguồn: Hồ sơ GIS các cấp HQTCSDL");

                sb.AppendLine(
                    "Cấp đơn vị: " +
                    GiaTri(row.TenCap));

                sb.AppendLine(
                    "Trạng thái: " +
                    row.TrangThai);

                if (row.HoSoHqtc != null &&
                    row.HoSoHqtc.KinhDo.HasValue &&
                    row.HoSoHqtc.ViDo.HasValue)
                {
                    sb.AppendLine(
                        "Kinh độ WGS84: " +
                        row.HoSoHqtc.KinhDo.Value
                            .ToString(
                                "F6",
                                CultureInfo.InvariantCulture));

                    sb.AppendLine(
                        "Vĩ độ WGS84: " +
                        row.HoSoHqtc.ViDo.Value
                            .ToString(
                                "F6",
                                CultureInfo.InvariantCulture));
                }
            }

            sb.AppendLine();
            sb.AppendLine("Lưu ý:");

            sb.AppendLine(
                "Có điểm trên bản đồ không đồng nghĩa " +
                "đã xác minh tọa độ trụ sở thực tế.");

            return sb.ToString();
        }

        private static string GiaTri(string value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? "Chưa có thông tin"
                : value.Trim();
        }

        // ========================================================
        // 19. XUAT CSV
        // ========================================================

        private void XuatCsv()
        {
            List<string[]> rows =
                new List<string[]>();

            rows.Add(new[]
            {
                "Mã đơn vị",
                "Tên đơn vị",
                "Loại đơn vị",
                "Địa bàn",
                "Trạng thái",
                "Nguồn dữ liệu",
                "Mã phường/xã",
                "Vị trí bản đồ",
                "Loại vị trí"
            });

            foreach (DataGridViewRow r in table.Rows)
            {
                Dong d = r.DataBoundItem as Dong;

                if (d == null)
                    continue;

                rows.Add(new[]
                {
                    d.MaDonVi,
                    d.TenDonVi,
                    d.LoaiDonVi,
                    d.DiaBan,
                    d.TrangThai,

                    d.LaDanhMuc
                        ? "Danh mục Ban CHQS cấp xã"
                        : "Hồ sơ GIS các cấp",

                    d.MaXaPhuong,

                    d.CoDiemBanDo
                        ? "Có"
                        : "Chưa có",

                    d.LaDanhMuc
                        ? "Điểm đại diện hành chính"
                        : "Điểm hồ sơ GIS"
                });
            }

            TienIchGiaoDienSang.ExportCsv(
                "DanhSachDonViQuanDoi_HienHanh.csv",
                rows);
        }

        // ========================================================
        // 20. BANNER
        // ========================================================

        private static Control TaoBanner()
        {
            Panel banner = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 65,
                Margin = new Padding(0, 0, 0, 5),
                Padding = Padding.Empty,
                BackColor = XanhNgoc
            };

            Label title = new Label
            {
                Text =
                    "QUẢN LÝ DỮ LIỆU ĐƠN VỊ QUÂN ĐỘI",

                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    17F,
                    FontStyle.Bold),

                TextAlign =
                    ContentAlignment.MiddleLeft,

                Padding =
                    new Padding(24, 0, 8, 0)
            };

            banner.Paint += (s, e) =>
            {
                Rectangle r =
                    banner.ClientRectangle;

                if (r.Width <= 0 ||
                    r.Height <= 0)
                {
                    return;
                }

                e.Graphics.SmoothingMode =
                    SmoothingMode.AntiAlias;

                using (LinearGradientBrush brush =
                    new LinearGradientBrush(
                        r,
                        Color.FromArgb(0, 164, 143),
                        Color.FromArgb(55, 104, 185),
                        LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, r);
                }

                using (Pen pen = new Pen(
                    Color.FromArgb(45, 255, 255, 255),
                    1F))
                {
                    int right = r.Right - 34;
                    int cy = r.Height / 2;

                    for (int i = 0; i < 4; i++)
                    {
                        int radius = 30 + i * 24;

                        e.Graphics.DrawEllipse(
                            pen,
                            right - radius,
                            cy - radius,
                            radius * 2,
                            radius * 2);
                    }
                }
            };

            banner.Controls.Add(title);

            return banner;
        }

        // ========================================================
        // 21. BO CUC TRANG
        // ========================================================

        private void CauHinhTrangKhongCuon()
        {
            Panel container =
                Controls.OfType<Panel>()
                    .FirstOrDefault(
                        p => p.Controls.Contains(Body));

            if (container != null)
            {
                container.AutoScroll = false;
                container.Padding =
                    new Padding(15, 10, 15, 10);
            }

            Body.AutoSize = false;
            Body.Dock = DockStyle.Fill;
            Body.Margin = Padding.Empty;
            Body.Padding = Padding.Empty;
        }

        // ========================================================
        // 22. TAO THE THONG KE
        // ========================================================

        private static Label TaoChiSoGon(
            TableLayoutPanel parent,
            int col,
            string title,
            Color accent,
            Color background)
        {
            NativeCard card =
                TienIchGiaoDienSang.Card(background);

            card.LineColor =
                Color.FromArgb(115, accent);

            card.Margin = new Padding(
                col == 0 ? 0 : 5,
                0,
                col == 2 ? 0 : 5,
                5);

            card.Padding =
                new Padding(11, 5, 8, 5);

            TableLayoutPanel inner =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty,
                    BackColor = background
                };

            inner.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 19F));

            inner.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F));

            inner.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F));

            inner.Controls.Add(
                TienIchGiaoDienSang.Text(
                    title,
                    8.1F,
                    true),
                0, 0);

            Label number =
                TienIchGiaoDienSang.Text(
                    "—",
                    19F,
                    true,
                    accent);

            inner.Controls.Add(number, 0, 1);

            card.Controls.Add(inner);
            parent.Controls.Add(card, col, 0);

            return number;
        }

        // ========================================================
        // 23. NHOM NUT
        // ========================================================

        private static FlowLayoutPanel TaoNhomNut(
            bool canPhai)
        {
            return new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,

                FlowDirection = canPhai
                    ? FlowDirection.RightToLeft
                    : FlowDirection.LeftToRight,

                WrapContents = false,
                AutoScroll = true,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.White
            };
        }

        // ========================================================
        // 24. DOI TUONG DONG BANG
        // ========================================================

        private sealed class Dong
        {
            public string MaDonVi { get; private set; }
            public string TenDonVi { get; private set; }
            public string LoaiDonVi { get; private set; }

            public string DiaBan { get; private set; }
            public string MaXaPhuong { get; private set; }
            public string TenCap { get; private set; }

            public string TrangThai { get; private set; }

            public bool CoDiemBanDo { get; private set; }
            public bool CoTheChinhSua { get; private set; }

            public bool LaDanhMuc { get; private set; }
            public bool LaHoSoHqtc { get; private set; }
            public bool LaHoSoChinh { get; private set; }

            public DanhMucBanChqsCapXa2026 BanChqs
            {
                get;
                private set;
            }

            public HoSoGisHqtc HoSoHqtc
            {
                get;
                private set;
            }

            // ====================================================
            // 24.1. BAN CHQS PHUONG XA
            // ====================================================

            public Dong(
                DanhMucBanChqsCapXa2026 d,
                string maHoSoGhi)
            {
                BanChqs = d;
                HoSoHqtc = null;

                LaDanhMuc = true;
                LaHoSoHqtc = false;

                // Co ho so HQTCSDL tuong ung -> sua/xoa theo ma ho so.
                CoTheChinhSua =
                    !string.IsNullOrWhiteSpace(maHoSoGhi);

                MaDonVi = CoTheChinhSua
                    ? maHoSoGhi.Trim()
                    : "BCHQS-" + (d.MaDvhc ?? "").Trim();

                TenDonVi = d.TenDonVi ?? "";

                LoaiDonVi =
                    string.IsNullOrWhiteSpace(d.LoaiDonVi)
                        ? "Ban Chỉ huy quân sự"
                        : d.LoaiDonVi.Trim();

                DiaBan = ChuanHoaDiaBan(
                    d.PhuongXa,
                    null);

                MaXaPhuong = d.MaXaPhuong ?? "";

                TenCap = "Cấp xã";

                // Diem dai dien dia ban.
                // Khong dong nghia toa do tru so.
                CoDiemBanDo = d.CoDiemDaiDien;

                TrangThai = CoDiemBanDo
                    ? "● Điểm đại diện"
                    : "○ Chưa có điểm";
            }

            // ====================================================
            // 24.2. HO SO GIS CAC CAP
            // ====================================================

            public Dong(
                HoSoGisHqtc d,
                bool coHoSoGhi)
            {
                BanChqs = null;
                HoSoHqtc = d;

                LaDanhMuc = false;
                LaHoSoHqtc = true;

                MaDonVi = d.MaDonVi ?? "";
                TenDonVi = d.TenDonVi ?? "";

                LoaiDonVi =
                    string.IsNullOrWhiteSpace(d.TenLoai)
                        ? (d.TenCap ?? "Chưa phân loại")
                        : d.TenLoai.Trim();

                DiaBan = d.DiaBan;

                if (string.IsNullOrWhiteSpace(DiaBan))
                {
                    DiaBan = ChuaXacDinh;
                }

                MaXaPhuong = d.MaXaPhuong ?? "";
                TenCap = d.TenCap ?? "";

                // Diem ho so GIS cac cap.
                CoDiemBanDo = d.CoDiemBanDo;

                CoTheChinhSua = coHoSoGhi;

                TrangThai = CoDiemBanDo
                    ? "● Điểm đơn vị"
                    : "○ Chưa có điểm";
            }

            // ====================================================
            // 24.3. HO SO CHINH public.don_vi_quan_doi
            // Sua/Xoa/Dinh vi qua BLL (quyen do FormMain kiem tra).
            // ====================================================

            public Dong(DonViQuanDoi d)
            {
                BanChqs = null;
                HoSoHqtc = null;

                LaDanhMuc = false;
                LaHoSoHqtc = false;
                LaHoSoChinh = true;

                MaDonVi = (d.MaDonVi ?? "").Trim();
                TenDonVi = d.TenDonVi ?? "";

                LoaiDonVi =
                    string.IsNullOrWhiteSpace(d.LoaiDonVi)
                        ? "Chưa phân loại"
                        : d.LoaiDonVi.Trim();

                DiaBan =
                    d.DaXacMinhDiaBan &&
                    !string.IsNullOrWhiteSpace(d.PhuongXa)
                        ? ChuanHoaDiaBan(d.PhuongXa, null)
                        : (string.IsNullOrWhiteSpace(d.QuanHuyen)
                            ? ChuaXacDinh
                            : d.QuanHuyen.Trim());

                MaXaPhuong = d.MaPhuongXa ?? "";

                TenCap = string.IsNullOrWhiteSpace(d.DonViCapTren)
                    ? "Hồ sơ quản lý"
                    : "Trực thuộc: " + d.DonViCapTren.Trim();

                CoDiemBanDo =
                    ManagerGISAcademy.BLL.NghiepVuDonVi
                        .CoToaDoWgs84(d);

                CoTheChinhSua = true;

                TrangThai = CoDiemBanDo
                    ? "● Hồ sơ quản lý"
                    : "○ Hồ sơ chưa có điểm";
            }

            private static string ChuanHoaDiaBan(
                string ten,
                string loai)
            {
                string s = (ten ?? "").Trim();

                if (s.Length == 0)
                    return ChuaXacDinh;

                if (s.StartsWith(
                    "Phường ",
                    StringComparison.OrdinalIgnoreCase) ||
                    s.StartsWith(
                    "Xã ",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return s;
                }

                string l = (loai ?? "").Trim();

                if (string.Equals(
                    l,
                    "phường",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return "Phường " + s;
                }

                if (string.Equals(
                    l,
                    "xã",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return "Xã " + s;
                }

                return s;
            }
        }

        // ========================================================
        // 25. GIAI PHONG TAI NGUYEN
        // ========================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing && delayed != null)
            {
                delayed.Stop();
                delayed.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
