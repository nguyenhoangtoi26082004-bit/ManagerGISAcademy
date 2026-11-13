using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using AxMapWinGIS;
using MapWinGIS;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // C# 7.3 / .NET Framework 4.8 / MapWinGIS.
    public partial class FormMain
    {
        private bool daDangKyKhoiTaoDoDac;
        private bool daKhoiTaoDoDac;
        private bool daHoanTatDoDac;
        private int cheDoDoDac; // 0: tắt, 1: khoảng cách, 2: diện tích.
        private Panel pnlThanhDoDac;
        private Panel pnlKetQuaDoDac;
        private Button btnDoKhoangCach;
        private Button btnDoDienTich;
        private Button btnHoanTatDo;
        private Button btnXoaPhepDo;
        private Label lblKetQuaDoDac;
        private Label lblHuongDanDoDac;
        private readonly ToolTip meoDoDac = new ToolTip();
        private readonly Color mauDoDacNen = Color.FromArgb(17, 32, 49);
        private readonly Color mauDoDacThuong = Color.FromArgb(40, 61, 81);
        private readonly Color mauDoDacChon = Color.FromArgb(13, 139, 107);

        private Panel dd26VungKetQua;
        private Label dd26TieuDe;
        private Label dd26TrangThai;
        private ComboBox dd26DonVi;
        private Button dd26BoDiem;
        private Button dd26SaoChep;
        private System.Windows.Forms.Timer dd26HenKhoiTao;
        private int dd26SoLanCho;
        private bool dd26DangDoiCongCu;
        private bool dd26DangDoiDonVi;
        private int dd26SoDiem;
        private double dd26GiaTri;
        private bool dd26KetQuaHopLe;

        // Giữ các điểm vào được FormMain.BanDoNangCap.cs sử dụng.
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (DesignMode) return;
            BdPro_KhoiTaoGiaoDien();
            if (daDangKyKhoiTaoDoDac) return;
            daDangKyKhoiTaoDoDac = true;
            Shown += FormMain_DoDac_Shown;
            FormClosed += FormMain_DoDac_FormClosed;
        }

        private void FormMain_DoDac_Shown(object sender, EventArgs e)
        {
            if (daKhoiTaoDoDac || dd26HenKhoiTao != null) return;
            dd26SoLanCho = 0;
            dd26HenKhoiTao = new System.Windows.Forms.Timer { Interval = 250 };
            dd26HenKhoiTao.Tick += FormMain_DoDac_KiemTraKhoiTao;
            dd26HenKhoiTao.Start();
            FormMain_DoDac_KiemTraKhoiTao(this, EventArgs.Empty);
        }

        private void FormMain_DoDac_KiemTraKhoiTao(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || daKhoiTaoDoDac)
            {
                Dd26_DungHenKhoiTao();
                return;
            }
            if (++dd26SoLanCho > 120)
            {
                Dd26_DungHenKhoiTao();
                GhiNhatKyBanDo("GIS_MEASUREMENT_INIT_TIMEOUT");
                return;
            }
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated || pnlMapCanvas == null ||
                pnlMapCard == null || pnlCheDoBanDo == null ||
                btnCheDoChonDiem == null || btnCheDoDiChuyen == null ||
                !daKhoiTaoThongTinBanDo) return;

            Dd26_DungHenKhoiTao();
            try
            {
                TaoThanhCongCuDoDac();
                TaoBangKetQuaDoDac();
                banDoMapWinGIS.MeasuringChanged -= BanDo_DoDac_MeasuringChanged;
                banDoMapWinGIS.MeasuringChanged += BanDo_DoDac_MeasuringChanged;
                btnCheDoChonDiem.Click -= Dd26_KhiChuyenCongCu;
                btnCheDoChonDiem.Click += Dd26_KhiChuyenCongCu;
                btnCheDoDiChuyen.Click -= Dd26_KhiChuyenCongCu;
                btnCheDoDiChuyen.Click += Dd26_KhiChuyenCongCu;
                lblPageTitle.TextChanged -= Dd26_KhiChuyenTrang;
                lblPageTitle.TextChanged += Dd26_KhiChuyenTrang;
                pnlMapCanvas.Resize -= Dd26_KhiDoiKichThuoc;
                pnlMapCanvas.Resize += Dd26_KhiDoiKichThuoc;
                daKhoiTaoDoDac = true;
                CanChinhViTriDoDac();
                CapNhatMauNutDoDac();
                GhiNhatKyBanDo("GIS_MEASUREMENT_INIT_OK | panel=bottom | controls=ready");
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("GIS_MEASUREMENT_INIT_FAILED | " + ex);
                statusGeneral.Text = "● Không khởi tạo được công cụ đo: " + ex.Message;
            }
        }

        private void Dd26_DungHenKhoiTao()
        {
            if (dd26HenKhoiTao == null) return;
            dd26HenKhoiTao.Stop();
            dd26HenKhoiTao.Tick -= FormMain_DoDac_KiemTraKhoiTao;
            dd26HenKhoiTao.Dispose();
            dd26HenKhoiTao = null;
        }

        private void FormMain_DoDac_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dd26_DungHenKhoiTao();
            cheDoDoDac = 0;
            try
            {
                if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed)
                    banDoMapWinGIS.MeasuringChanged -= BanDo_DoDac_MeasuringChanged;
            }
            catch (Exception) { /* ActiveX có thể đã được giải phóng. */ }
            if (btnCheDoChonDiem != null) btnCheDoChonDiem.Click -= Dd26_KhiChuyenCongCu;
            if (btnCheDoDiChuyen != null) btnCheDoDiChuyen.Click -= Dd26_KhiChuyenCongCu;
            if (lblPageTitle != null) lblPageTitle.TextChanged -= Dd26_KhiChuyenTrang;
            if (pnlMapCanvas != null) pnlMapCanvas.Resize -= Dd26_KhiDoiKichThuoc;
            meoDoDac.Dispose();
        }

        private void Dd26_KhiChuyenCongCu(object sender, EventArgs e)
        {
            ThoatDoDacKhiChuyenCongCu();
        }

        private void Dd26_KhiChuyenTrang(object sender, EventArgs e)
        {
            if (lblPageTitle.Text != "Bản đồ GIS") ThoatDoDacKhiChuyenCongCu();
        }

        private void Dd26_KhiDoiKichThuoc(object sender, EventArgs e)
        {
            CanChinhViTriDoDac();
        }

        private void TaoThanhCongCuDoDac()
        {
            if (pnlThanhDoDac != null) return;
            pnlThanhDoDac = new Panel
            {
                Width = 330,
                Height = 40,
                BackColor = mauDoDacNen,
                BorderStyle = BorderStyle.FixedSingle
            };
            TableLayoutPanel bang = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(3),
                Margin = Padding.Empty
            };
            bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            bang.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            btnDoKhoangCach = TaoNutDoDac("ĐO KHOẢNG CÁCH");
            btnDoDienTich = TaoNutDoDac("ĐO DIỆN TÍCH");
            btnHoanTatDo = TaoNutDoDac("HOÀN TẤT");
            btnXoaPhepDo = TaoNutDoDac("ĐO LẠI");
            btnDoKhoangCach.Click += (s, e) => BatCheDoDoDac(1);
            btnDoDienTich.Click += (s, e) => BatCheDoDoDac(2);
            btnHoanTatDo.Click += (s, e) => HoanTatPhepDo();
            btnXoaPhepDo.Click += (s, e) => XoaPhepDoHienTai();
            bang.Controls.Add(btnDoKhoangCach, 0, 0);
            bang.Controls.Add(btnDoDienTich, 1, 0);
            pnlThanhDoDac.Controls.Add(bang);
            pnlMapCanvas.Controls.Add(pnlThanhDoDac);
            meoDoDac.SetToolTip(btnDoKhoangCach, "Đo tuyến qua ít nhất hai điểm.");
            meoDoDac.SetToolTip(btnDoDienTich, "Đo vùng qua ít nhất ba đỉnh; vẽ biên không tự cắt.");
            meoDoDac.SetToolTip(btnHoanTatDo, "Hoàn tất phép đo đang thực hiện.");
            meoDoDac.SetToolTip(btnXoaPhepDo, "Xóa phép đo hiện tại và bắt đầu lại cùng loại.");
        }

        private Button TaoNutDoDac(string noiDung)
        {
            Button nut = new Button
            {
                Text = noiDung,
                Dock = DockStyle.Fill,
                Margin = new Padding(3),
                FlatStyle = FlatStyle.Flat,
                BackColor = mauDoDacThuong,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 110, 91);
            return nut;
        }

        private void TaoBangKetQuaDoDac()
        {
            if (pnlKetQuaDoDac != null) return;
            // Vùng chứa ngoài AxMap: bản đồ và các công cụ của nó
            // tự thu chiều cao; nhãn bản đồ không nằm dưới bảng kết quả.
            dd26VungKetQua = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 174,
                Visible = false,
                BackColor = mauDoDacNen
            };
            pnlKetQuaDoDac = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = mauDoDacNen,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };
            TableLayoutPanel bang = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(8, 4, 8, 4),
                Margin = Padding.Empty
            };
            bang.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            bang.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            bang.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            bang.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            bang.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            bang.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Panel dau = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            Button dong = new Button
            {
                Text = "×",
                Dock = DockStyle.Right,
                Width = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = mauDoDacThuong,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            dong.FlatAppearance.BorderSize = 0;
            dong.Click += (s, e) => Dd26_DongCongCu();
            dd26TieuDe = new Label
            {
                Text = "KẾT QUẢ ĐO",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            dau.Controls.Add(dd26TieuDe);
            dau.Controls.Add(dong);
            lblKetQuaDoDac = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Chưa có kết quả đo",
                ForeColor = Color.FromArgb(52, 211, 153),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            lblHuongDanDoDac = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(188, 205, 221),
                Font = new Font("Segoe UI", 8.5F),
                TextAlign = ContentAlignment.MiddleLeft,
                AutoEllipsis = true
            };
            TableLayoutPanel tuyChon = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty
            };
            tuyChon.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110F));
            tuyChon.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165F));
            tuyChon.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tuyChon.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tuyChon.Controls.Add(new Label
            {
                Text = "Đơn vị kết quả:",
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);
            dd26DonVi = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = mauDoDacThuong,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            dd26DonVi.SelectedIndexChanged += (s, e) =>
            {
                if (!dd26DangDoiDonVi) Dd26_HienKetQuaDaDoc();
            };
            dd26TrangThai = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(188, 205, 221),
                TextAlign = ContentAlignment.MiddleRight,
                AutoEllipsis = true
            };
            tuyChon.Controls.Add(dd26DonVi, 1, 0);
            tuyChon.Controls.Add(dd26TrangThai, 2, 0);
            TableLayoutPanel thaoTac = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty
            };
            for (int i = 0; i < 4; i++)
                thaoTac.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            thaoTac.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            dd26BoDiem = TaoNutDoDac("BỎ ĐIỂM CUỐI");
            dd26SaoChep = TaoNutDoDac("SAO CHÉP KẾT QUẢ");
            dd26BoDiem.Click += (s, e) => Dd26_BoDiemCuoi();
            dd26SaoChep.Click += (s, e) => Dd26_SaoChepKetQua();
            // Tái sử dụng chính hai nút cũ, đưa vào bảng luôn truy cập được.
            thaoTac.Controls.Add(dd26BoDiem, 0, 0);
            thaoTac.Controls.Add(btnHoanTatDo, 1, 0);
            thaoTac.Controls.Add(btnXoaPhepDo, 2, 0);
            thaoTac.Controls.Add(dd26SaoChep, 3, 0);
            bang.Controls.Add(dau, 0, 0);
            bang.Controls.Add(lblKetQuaDoDac, 0, 1);
            bang.Controls.Add(lblHuongDanDoDac, 0, 2);
            bang.Controls.Add(tuyChon, 0, 3);
            bang.Controls.Add(thaoTac, 0, 4);
            pnlKetQuaDoDac.Controls.Add(bang);
            dd26VungKetQua.Controls.Add(pnlKetQuaDoDac);
            pnlMapCard.Controls.Add(dd26VungKetQua);
            pnlMapCanvas.BringToFront();
            meoDoDac.SetToolTip(dong, "Thoát đo và trở về chọn điểm. Phím Esc khi đang ở bản đồ.");
            meoDoDac.SetToolTip(dd26BoDiem, "Bỏ đỉnh cuối khi đang đo; cũng có thể dùng chuột phải.");
            meoDoDac.SetToolTip(dd26SaoChep, "Sao chép kết quả đã hoàn tất theo đơn vị đang chọn.");
        }

        private void CanChinhViTriDoDac()
        {
            if (pnlMapCanvas == null || pnlThanhDoDac == null) return;
            int rong = Math.Max(1, pnlMapCanvas.ClientSize.Width - 16);
            pnlThanhDoDac.Width = Math.Min(330, rong);
            pnlThanhDoDac.Left = Math.Max(0,
                (pnlMapCanvas.ClientSize.Width - pnlThanhDoDac.Width) / 2);
            pnlThanhDoDac.Top = 8;
            if (pnlCheDoBanDo != null && pnlCheDoBanDo.Visible)
                pnlThanhDoDac.Top = pnlCheDoBanDo.Bottom + 6;
            if (pnlMapTools != null) pnlMapTools.BringToFront();
        }

        private void Dd26_HienBangKetQua(bool hien)
        {
            if (pnlKetQuaDoDac == null || dd26VungKetQua == null) return;
            if (hien)
            {
                pnlKetQuaDoDac.Visible = true;
                dd26VungKetQua.Visible = true;
                // FormMain.BanDoNangCap có thể BringToFront bảng con;
                // vùng chứa Dock Bottom vẫn giữ thứ tự với vùng bản đồ.
                pnlMapCanvas.BringToFront();
            }
            else
            {
                dd26VungKetQua.Visible = false;
                pnlKetQuaDoDac.Visible = false;
            }
        }

        private void Dd26_NapDonVi()
        {
            dd26DangDoiDonVi = true;
            try
            {
                dd26DonVi.Items.Clear();
                dd26DonVi.Items.Add("Tự động");
                if (cheDoDoDac == 1)
                    dd26DonVi.Items.AddRange(new object[] { "Mét (m)", "Kilômét (km)" });
                else
                    dd26DonVi.Items.AddRange(new object[] { "Mét vuông (m²)", "Hécta (ha)", "Kilômét vuông (km²)" });
                dd26DonVi.SelectedIndex = 0;
            }
            finally { dd26DangDoiDonVi = false; }
        }

        private void Dd26_CauHinhNetDo(Measuring congCuDo)
        {
            congCuDo.Persistent = false;
            congCuDo.LineColor = unchecked((uint)ColorTranslator.ToOle(Color.FromArgb(255, 188, 68)));
            congCuDo.LineWidth = 3F;
            congCuDo.PointsVisible = true;
            congCuDo.PointLabelsVisible = true;
            congCuDo.ShowLength = true;
            congCuDo.ShowTotalLength = true;
            congCuDo.LengthUnits = tkLengthDisplayMode.ldmMetric;
            congCuDo.AreaUnits = tkAreaDisplayMode.admMetric;
            congCuDo.LengthPrecision = 2;
            congCuDo.AreaPrecision = 2;
            congCuDo.FillColor = unchecked((uint)ColorTranslator.ToOle(Color.FromArgb(52, 211, 153)));
            congCuDo.FillTransparency = 70;
        }

        private void BatCheDoDoDac(int kieuDo)
        {
            if (!daKhoiTaoDoDac || banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed || (kieuDo != 1 && kieuDo != 2)) return;

            // Da loai bo co che bieu mau nhap lieu cu.
            // Che do do dac hoat dong doc lap voi ThemSuaDonVi.

            dd26DangDoiCongCu = true;
            try
            {
                DatCheDoBanDo(false);
                AnKhungThongTinDiem();
                Measuring congCuDo = banDoMapWinGIS.Measuring;
                congCuDo.Clear();
                cheDoDoDac = kieuDo;
                daHoanTatDoDac = false;
                dd26SoDiem = 0;
                dd26GiaTri = 0d;
                dd26KetQuaHopLe = false;
                congCuDo.MeasuringType = kieuDo == 1
                    ? tkMeasuringType.MeasureDistance : tkMeasuringType.MeasureArea;
                Dd26_CauHinhNetDo(congCuDo);
                Dd26_NapDonVi();
                // Thu chiều cao bản đồ trước khi người dùng nhập đỉnh đầu.
                Dd26_HienBangKetQua(true);
                banDoMapWinGIS.CursorMode = tkCursorMode.cmMeasure;
                Dd26_HienKetQuaDaDoc();
                banDoMapWinGIS.Focus();
                GhiNhatKyBanDo("GIS_MEASUREMENT_START | type=" + kieuDo);
            }
            catch (Exception ex)
            {
                cheDoDoDac = 0;
                daHoanTatDoDac = false;
                dd26KetQuaHopLe = false;
                Dd26_HienBangKetQua(false);
                try { DatCheDoBanDo(true); } catch (Exception) { }
                CapNhatMauNutDoDac();
                GhiNhatKyBanDo("GIS_MEASUREMENT_START_FAILED | " + ex);
                statusGeneral.Text = "● Không bật được công cụ đo: " + ex.Message;
            }
            finally { dd26DangDoiCongCu = false; }
        }

        private void BanDo_DoDac_MeasuringChanged(object sender, _DMapEvents_MeasuringChangedEvent e)
        {
            if (dd26DangDoiCongCu || IsDisposed || Disposing ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                lblPageTitle.Text != "Bản đồ GIS") return;
            try
            {
                // Đồng bộ cả phép đo do phân hệ Phân tích không gian mở.
                // Không Clear ở đây: đỉnh người dùng vừa nhập phải được giữ lại.
                if (cheDoDoDac == 0)
                {
                    if (banDoMapWinGIS.CursorMode != tkCursorMode.cmMeasure) return;
                    cheDoDoDac = banDoMapWinGIS.Measuring.MeasuringType == tkMeasuringType.MeasureArea ? 2 : 1;
                    daHoanTatDoDac = false;
                    Dd26_NapDonVi();
                    // Đợi callback COM kết thúc rồi mới đổi bố cục AxMap.
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        if (IsDisposed || Disposing || cheDoDoDac == 0 ||
                            lblPageTitle.Text != "Bản đồ GIS") return;
                        AnKhungThongTinDiem();
                        Dd26_HienBangKetQua(true);
                    }));
                }
                daHoanTatDoDac = e.action == tkMeasuringAction.MeasuringStopped ||
                    banDoMapWinGIS.Measuring.IsStopped;
                CapNhatKetQuaDoDac();
            }
            catch (Exception ex)
            {
                dd26KetQuaHopLe = false;
                CapNhatMauNutDoDac();
                GhiNhatKyBanDo("GIS_MEASUREMENT_EVENT_FAILED | " + ex);
                statusGeneral.Text = "● Không đọc được kết quả đo: " + ex.Message;
            }
        }

        private void CapNhatKetQuaDoDac()
        {
            if (cheDoDoDac == 0 || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            Measuring congCuDo = banDoMapWinGIS.Measuring;
            dd26SoDiem = congCuDo.PointCount;
            daHoanTatDoDac = daHoanTatDoDac || congCuDo.IsStopped;
            dd26GiaTri = Math.Abs(cheDoDoDac == 1 ? congCuDo.Length : congCuDo.Area);
            dd26KetQuaHopLe = dd26SoDiem >= (cheDoDoDac == 1 ? 2 : 3) &&
                !double.IsNaN(dd26GiaTri) && !double.IsInfinity(dd26GiaTri) && dd26GiaTri > 0d;
            Dd26_HienKetQuaDaDoc();
        }

        private void Dd26_HienKetQuaDaDoc()
        {
            if (cheDoDoDac == 0 || lblKetQuaDoDac == null) return;
            bool laKhoangCach = cheDoDoDac == 1;
            dd26TieuDe.Text = laKhoangCach ? "ĐO KHOẢNG CÁCH" : "ĐO DIỆN TÍCH";
            lblKetQuaDoDac.Text = dd26KetQuaHopLe
                ? (laKhoangCach ? "Khoảng cách: " : "Diện tích: ") + Dd26_DinhDangKetQua()
                : "Chọn ít nhất " + (laKhoangCach ? "2 điểm khác nhau." : "3 đỉnh tạo thành một vùng.");
            if (daHoanTatDoDac && !dd26KetQuaHopLe)
                lblHuongDanDoDac.Text = "Phép đo chưa có kết quả hợp lệ. Chọn ĐO LẠI để thực hiện lại.";
            else if (daHoanTatDoDac)
                lblHuongDanDoDac.Text = "Đã hoàn tất. Có thể đổi đơn vị, sao chép kết quả hoặc chọn ĐO LẠI.";
            else
                lblHuongDanDoDac.Text = "Nhấp để thêm đỉnh; chuột phải để bỏ đỉnh; đúp chuột hoặc HOÀN TẤT để kết thúc.";
            dd26TrangThai.Text = dd26SoDiem + " điểm · " + (daHoanTatDoDac ? "Đã kết thúc" : "Đang đo");
            CapNhatMauNutDoDac();
            statusGeneral.Text = "● " + lblKetQuaDoDac.Text;
        }

        private void HoanTatPhepDo()
        {
            if (cheDoDoDac == 0 || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            try
            {
                CapNhatKetQuaDoDac();
                if (!dd26KetQuaHopLe)
                {
                    statusGeneral.Text = cheDoDoDac == 1
                        ? "● Cần ít nhất hai điểm khác nhau để hoàn tất đo khoảng cách."
                        : "● Cần ít nhất ba đỉnh tạo thành vùng có diện tích lớn hơn 0.";
                    return;
                }
                if (!daHoanTatDoDac) banDoMapWinGIS.Measuring.FinishMeasuring();
                daHoanTatDoDac = true;
                CapNhatKetQuaDoDac();
                GhiNhatKyBanDo("GIS_MEASUREMENT_FINISHED | type=" + cheDoDoDac +
                    " | points=" + dd26SoDiem + " | value=" + dd26GiaTri.ToString("R", CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("GIS_MEASUREMENT_FINISH_FAILED | " + ex);
                statusGeneral.Text = "● Không hoàn tất được phép đo: " + ex.Message;
            }
        }

        private void Dd26_BoDiemCuoi()
        {
            if (cheDoDoDac == 0 || daHoanTatDoDac || banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            try
            {
                if (banDoMapWinGIS.Measuring.PointCount > 0)
                {
                    if (!banDoMapWinGIS.Measuring.UndoPoint())
                    {
                        statusGeneral.Text = "● Không bỏ được điểm cuối.";
                        return;
                    }
                    daHoanTatDoDac = false;
                }
                CapNhatKetQuaDoDac();
                banDoMapWinGIS.Focus();
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("GIS_MEASUREMENT_UNDO_FAILED | " + ex);
                statusGeneral.Text = "● Không bỏ được điểm: " + ex.Message;
            }
        }

        private void XoaPhepDoHienTai()
        {
            if (banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed) return;
            dd26DangDoiCongCu = true;
            try
            {
                banDoMapWinGIS.Measuring.Clear();
                daHoanTatDoDac = false;
                dd26SoDiem = 0;
                dd26GiaTri = 0d;
                dd26KetQuaHopLe = false;
                if (cheDoDoDac != 0)
                {
                    banDoMapWinGIS.CursorMode = tkCursorMode.cmMeasure;
                    Dd26_HienKetQuaDaDoc();
                    banDoMapWinGIS.Focus();
                }
                CapNhatMauNutDoDac();
                statusGeneral.Text = "● Đã xóa phép đo. Nhấp bản đồ để bắt đầu lại.";
                GhiNhatKyBanDo("GIS_MEASUREMENT_CLEARED");
            }
            catch (Exception ex)
            {
                dd26KetQuaHopLe = false;
                CapNhatMauNutDoDac();
                GhiNhatKyBanDo("GIS_MEASUREMENT_CLEAR_FAILED | " + ex);
                statusGeneral.Text = "● Không xóa được phép đo: " + ex.Message;
            }
            finally { dd26DangDoiCongCu = false; }
        }

        private void Dd26_SaoChepKetQua()
        {
            if (cheDoDoDac == 0 || !daHoanTatDoDac || !dd26KetQuaHopLe) return;
            try
            {
                string noiDung = (cheDoDoDac == 1 ? "ĐO KHOẢNG CÁCH" : "ĐO DIỆN TÍCH") +
                    "\r\nKết quả: " + Dd26_DinhDangKetQua() +
                    "\r\nSố điểm: " + dd26SoDiem +
                    "\r\nTrạng thái: Đã hoàn tất";
                Clipboard.SetText(noiDung);
                statusGeneral.Text = "● Đã sao chép kết quả đo.";
            }
            catch (Exception ex)
            {
                statusGeneral.Text = "● Không sao chép được kết quả: " + ex.Message;
            }
        }

        private void ThoatDoDacKhiChuyenCongCu()
        {
            if (cheDoDoDac == 0) return;
            dd26DangDoiCongCu = true;
            cheDoDoDac = 0;
            daHoanTatDoDac = false;
            dd26SoDiem = 0;
            dd26GiaTri = 0d;
            dd26KetQuaHopLe = false;
            try
            {
                if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.Measuring.Clear();
                }
            }
            catch (Exception ex) { GhiNhatKyBanDo("GIS_MEASUREMENT_EXIT_FAILED | " + ex); }
            finally
            {
                try
                {
                    if (banDoMapWinGIS != null && !banDoMapWinGIS.IsDisposed &&
                        banDoMapWinGIS.CursorMode == tkCursorMode.cmMeasure)
                    {
                        DatCheDoBanDo(true);
                        Bd11_DanhDauChon(true);
                    }
                }
                catch (Exception ex) { GhiNhatKyBanDo("GIS_MEASUREMENT_CURSOR_FAILED | " + ex); }
                Dd26_HienBangKetQua(false);
                CapNhatMauNutDoDac();
                dd26DangDoiCongCu = false;
            }
        }

        private void Dd26_DongCongCu()
        {
            ThoatDoDacKhiChuyenCongCu();
            statusGeneral.Text = "● Đã thoát công cụ đo.";
        }

        private void CapNhatMauNutDoDac()
        {
            if (btnDoKhoangCach == null || btnDoDienTich == null) return;
            btnDoKhoangCach.BackColor = cheDoDoDac == 1 ? mauDoDacChon : mauDoDacThuong;
            btnDoDienTich.BackColor = cheDoDoDac == 2 ? mauDoDacChon : mauDoDacThuong;
            bool dangDo = cheDoDoDac != 0;
            btnHoanTatDo.Enabled = dangDo && !daHoanTatDoDac && dd26KetQuaHopLe;
            btnXoaPhepDo.Enabled = dangDo && (dd26SoDiem > 0 || daHoanTatDoDac);
            if (dd26BoDiem != null) dd26BoDiem.Enabled = dangDo && !daHoanTatDoDac && dd26SoDiem > 0;
            if (dd26SaoChep != null) dd26SaoChep.Enabled = dangDo && daHoanTatDoDac && dd26KetQuaHopLe;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Không chặn Enter/Backspace khi nhập ô tìm kiếm hoặc biểu mẫu.
            bool trongVungDo = cheDoDoDac != 0 && lblPageTitle.Text == "Bản đồ GIS" &&
                ((banDoMapWinGIS != null && banDoMapWinGIS.ContainsFocus) ||
                 (pnlKetQuaDoDac != null && pnlKetQuaDoDac.ContainsFocus));
            if (trongVungDo)
            {
                if (dd26DonVi.DroppedDown) return base.ProcessCmdKey(ref msg, keyData);
                if (keyData == Keys.Escape) { Dd26_DongCongCu(); return true; }
                if (keyData == Keys.Back) { Dd26_BoDiemCuoi(); return true; }
                if (keyData == Keys.Enter && !dd26DonVi.ContainsFocus)
                {
                    HoanTatPhepDo();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string Dd26_DinhDangKetQua()
        {
            if (!dd26KetQuaHopLe) return "Chưa xác định";
            int chon = dd26DonVi.SelectedIndex;
            CultureInfo vanHoa = CultureInfo.GetCultureInfo("vi-VN");
            if (cheDoDoDac == 1)
            {
                if (chon == 1) return dd26GiaTri.ToString("N2", vanHoa) + " m";
                if (chon == 2) return (dd26GiaTri / 1000d).ToString("N3", vanHoa) + " km";
                return DinhDangKhoangCach(dd26GiaTri);
            }
            if (chon == 1) return dd26GiaTri.ToString("N2", vanHoa) + " m²";
            if (chon == 2) return (dd26GiaTri / 10000d).ToString("N3", vanHoa) + " ha";
            if (chon == 3) return (dd26GiaTri / 1000000d).ToString("N3", vanHoa) + " km²";
            return DinhDangDienTich(dd26GiaTri);
        }

        private static string DinhDangKhoangCach(double met)
        {
            if (double.IsNaN(met) || double.IsInfinity(met) || met < 0d) return "Chưa xác định";
            CultureInfo vanHoa = CultureInfo.GetCultureInfo("vi-VN");
            return met >= 1000d ? (met / 1000d).ToString("N3", vanHoa) + " km"
                : met.ToString("N2", vanHoa) + " m";
        }

        private static string DinhDangDienTich(double metVuong)
        {
            if (double.IsNaN(metVuong) || double.IsInfinity(metVuong) || metVuong < 0d) return "Chưa xác định";
            CultureInfo vanHoa = CultureInfo.GetCultureInfo("vi-VN");
            if (metVuong >= 1000000d) return (metVuong / 1000000d).ToString("N3", vanHoa) + " km²";
            if (metVuong >= 10000d) return (metVuong / 10000d).ToString("N3", vanHoa) + " ha";
            return metVuong.ToString("N2", vanHoa) + " m²";
        }
    }
}