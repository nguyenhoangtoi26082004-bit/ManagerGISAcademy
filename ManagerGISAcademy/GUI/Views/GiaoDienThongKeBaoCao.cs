
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Views
{
    /// <summary>
    /// Trang "THONG KE VA BAO CAO" - thiet ke v2 (13/11/2026).
    /// Khong tu truy van CSDL: FormMain nap danh sach ho so (NapDuLieu) va
    /// tong hop chat luong vi tri (NapChatLuongViTri).
    /// Bao cao: Xuat PDF (may in ao Microsoft Print to PDF) va In (xem truoc + chon may in).
    /// </summary>
    public sealed class UcThongKeBaoCaoNative : NativePage
    {
        private static readonly string[] TenTieuChi = { "Loại đơn vị", "Địa bàn", "Quân chủng" };

        private readonly List<DonViQuanDoi> source = new List<DonViQuanDoi>();
        private readonly BannerThongKe banner;
        private readonly List<Button> nutTieuChi = new List<Button>();
        private readonly TheChiSo kpiTong, kpiToaDo, kpiXacMinh, kpiNhom;
        private readonly TheBieuDoThanh bieuDo;
        private readonly TheChatLuong chatLuong;
        private readonly DataGridView summary;
        private readonly Label lblBangPhu;
        private readonly Label status;
        private readonly Button btnIn, btnPdf;
        private int tieuChi;
        private List<NhomThongKe> nhomHienTai = new List<NhomThongKe>();
        private int? viTriTot, viTriCanDoLai, diemDaiDien;
        private DateTime thoiGianNap = DateTime.Now;

        public event Action YeuCauLamMoi;

        public UcThongKeBaoCaoNative()
        {
            Name = "UcThongKeBaoCaoNative";

            // 1. Banner
            banner = new BannerThongKe();
            AddRow(banner, 96);

            // 2. Thanh cong cu: tieu chi (trai) + hanh dong (phai)
            Panel thanh = new Panel { BackColor = TienIchGiaoDienSang.Background, Padding = new Padding(0, 10, 0, 6) };
            FlowLayoutPanel trai = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 470,
                WrapContents = false,
                BackColor = TienIchGiaoDienSang.Background,
                Padding = Padding.Empty
            };
            Label cap = TienIchGiaoDienSang.Text("Tiêu chí", 9F, true, TienIchGiaoDienSang.Muted);
            cap.Dock = DockStyle.None;
            cap.AutoSize = false;
            cap.Size = new Size(62, 36);
            cap.Margin = new Padding(0, 0, 4, 0);
            trai.Controls.Add(cap);
            Panel seg = new KhungBoGoc { Size = new Size(372, 38), Padding = new Padding(3), Margin = Padding.Empty };
            FlowLayoutPanel segTrong = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, BackColor = Color.White, Margin = Padding.Empty, Padding = Padding.Empty };
            int[] rong = { 120, 110, 130 };
            for (int i = 0; i < TenTieuChi.Length; i++)
            {
                int chiSo = i;
                Button b = NutPhang(TenTieuChi[i], Color.White, TienIchGiaoDienSang.Muted, 9F);
                b.Size = new Size(rong[i] - 2, 32);
                b.Margin = new Padding(0, 0, 2, 0);
                b.Click += (s, e) => ChonTieuChi(chiSo);
                nutTieuChi.Add(b);
                segTrong.Controls.Add(b);
            }
            seg.Controls.Add(segTrong);
            trai.Controls.Add(seg);

            FlowLayoutPanel phai = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = TienIchGiaoDienSang.Background,
                Padding = Padding.Empty
            };
            btnPdf = NutPhang("⬇  Xuất PDF", TienIchGiaoDienSang.Teal, Color.White, 9.2F);
            btnPdf.Size = new Size(132, 38);
            btnIn = NutPhang("⎙  In báo cáo", TienIchGiaoDienSang.Blue, Color.White, 9.2F);
            btnIn.Size = new Size(140, 38);
            Button btnLamMoi = NutPhang("↻  Làm mới", Color.White, TienIchGiaoDienSang.Ink, 9.2F);
            btnLamMoi.Size = new Size(118, 38);
            btnLamMoi.FlatAppearance.BorderSize = 1;
            btnLamMoi.FlatAppearance.BorderColor = TienIchGiaoDienSang.Border;
            btnPdf.Margin = new Padding(0);
            btnIn.Margin = new Padding(0, 0, 8, 0);
            btnLamMoi.Margin = new Padding(0, 0, 8, 0);
            phai.Controls.Add(btnPdf);
            phai.Controls.Add(btnIn);
            phai.Controls.Add(btnLamMoi);
            thanh.Controls.Add(phai);
            thanh.Controls.Add(trai);
            AddRow(thanh, 58);
            btnLamMoi.Click += (s, e) => { if (YeuCauLamMoi != null) YeuCauLamMoi(); };
            btnPdf.Click += (s, e) => XuatPdf();
            btnIn.Click += (s, e) => InBaoCao();
            ToolTip goiY = new ToolTip();
            goiY.SetToolTip(btnPdf, "Lưu báo cáo A4 thành tệp PDF (máy in ảo Microsoft Print to PDF).");
            goiY.SetToolTip(btnIn, "Xem trước báo cáo, chọn máy in và in.");

            // 3. Bon chi so
            TableLayoutPanel kpi = new TableLayoutPanel { ColumnCount = 4, RowCount = 1, BackColor = TienIchGiaoDienSang.Background, Margin = Padding.Empty };
            for (int i = 0; i < 4; i++) kpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            kpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            kpiTong = new TheChiSo("Σ", "TỔNG ĐƠN VỊ", VeBaoCao.Teal, Color.FromArgb(226, 246, 241));
            kpiToaDo = new TheChiSo("⌖", "CÓ TỌA ĐỘ", VeBaoCao.Blue, Color.FromArgb(230, 241, 252));
            kpiXacMinh = new TheChiSo("✓", "VỊ TRÍ ĐÃ XÁC MINH", VeBaoCao.Amber, Color.FromArgb(253, 241, 225));
            kpiNhom = new TheChiSo("▦", "NHÓM THỐNG KÊ", VeBaoCao.Violet, Color.FromArgb(239, 235, 252));
            TheChiSo[] the = { kpiTong, kpiToaDo, kpiXacMinh, kpiNhom };
            for (int i = 0; i < 4; i++)
            {
                the[i].Dock = DockStyle.Fill;
                the[i].Margin = new Padding(i == 0 ? 0 : 6, 0, i == 3 ? 0 : 6, 0);
                kpi.Controls.Add(the[i], i, 0);
            }
            AddRow(kpi, 96);
            Panel cach1 = new Panel { BackColor = TienIchGiaoDienSang.Background };
            AddRow(cach1, 12);

            // 4. Bieu do phan bo + chat luong vi tri
            TableLayoutPanel hai = new TableLayoutPanel { ColumnCount = 2, RowCount = 1, BackColor = TienIchGiaoDienSang.Background, Margin = Padding.Empty };
            hai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 59F));
            hai.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 41F));
            hai.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            bieuDo = new TheBieuDoThanh { Dock = DockStyle.Fill, Margin = new Padding(0, 0, 6, 0) };
            chatLuong = new TheChatLuong { Dock = DockStyle.Fill, Margin = new Padding(6, 0, 0, 0) };
            hai.Controls.Add(bieuDo, 0, 0);
            hai.Controls.Add(chatLuong, 1, 0);
            AddRow(hai, 340);
            Panel cach2 = new Panel { BackColor = TienIchGiaoDienSang.Background };
            AddRow(cach2, 12);

            // 5. Bang tong hop
            NativeCard tableCard = TienIchGiaoDienSang.Card();
            tableCard.Padding = new Padding(16, 12, 16, 12);
            Panel dauBang = new Panel { Dock = DockStyle.Top, Height = 36, BackColor = Color.White };
            Label tieuDeBang = TienIchGiaoDienSang.Text("Bảng tổng hợp", 11.5F, true);
            tieuDeBang.Dock = DockStyle.Left;
            tieuDeBang.Width = 200;
            lblBangPhu = TienIchGiaoDienSang.Text("", 8.8F, false, TienIchGiaoDienSang.Muted);
            lblBangPhu.Dock = DockStyle.Fill;
            lblBangPhu.TextAlign = ContentAlignment.MiddleRight;
            dauBang.Controls.Add(lblBangPhu);
            dauBang.Controls.Add(tieuDeBang);
            summary = TaoBang();
            tableCard.Controls.Add(summary);
            tableCard.Controls.Add(dauBang);
            AddRow(tableCard, 380);

            status = TienIchGiaoDienSang.Text("Chưa có số liệu từ BLL / PostGIS", 8.5F, false,
                TienIchGiaoDienSang.Muted);
            AddRow(status, 36);

            ChonTieuChi(0);
        }


        // =================================================
        // NAP DU LIEU (FormMain goi)
        // =================================================

        public void NapDuLieu(IEnumerable<DonViQuanDoi> items)
        {
            if (IsDisposed) return;
            source.Clear();
            if (items != null) source.AddRange(items.Where(d => d != null));
            thoiGianNap = DateTime.Now;
            banner.ThoiGian = thoiGianNap.ToString("dd/MM/yyyy HH:mm", VeBaoCao.Vi);
            CapNhatChiSo();
            UpdateBreakdown();
            status.Text = "Nguồn: BLL / PostGIS (HQTCSDL)  ·  Cập nhật: " + banner.ThoiGian +
                "  ·  Báo cáo PDF gồm biểu đồ, bảng tổng hợp và phụ lục danh sách đơn vị.";
        }

        /// <summary>Tong hop tu ql_don_vi.v_thong_ke_vi_tri; null = khong doc duoc.</summary>
        public void NapChatLuongViTri(int? tot, int? canDoLai, int? daiDien)
        {
            viTriTot = tot;
            viTriCanDoLai = canDoLai;
            diemDaiDien = daiDien;
            CapNhatChiSo();
        }

        private int? ViTriThat
        {
            get
            {
                if (!viTriTot.HasValue || !viTriCanDoLai.HasValue) return null;
                return viTriTot.Value + viTriCanDoLai.Value;
            }
        }

        private void CapNhatChiSo()
        {
            int tong = source.Count;
            int coToaDo = source.Count(NghiepVuDonVi.CoToaDoWgs84);
            kpiTong.DatGiaTri(tong.ToString("N0", VeBaoCao.Vi), "hồ sơ được cấp quyền");
            kpiToaDo.DatGiaTri(coToaDo.ToString("N0", VeBaoCao.Vi),
                PhanTram(coToaDo, tong) + " có WGS84 hợp lệ");
            int? thuc = ViTriThat;
            if (thuc.HasValue)
                kpiXacMinh.DatGiaTri(thuc.Value.ToString("N0", VeBaoCao.Vi),
                    PhanTram(thuc.Value, tong) + " tổng số hồ sơ");
            else
                kpiXacMinh.DatGiaTri("—", "chưa đọc được thống kê vị trí");
            chatLuong.DatSoLieu(thuc, diemDaiDien, viTriTot);
        }

        // =================================================
        // TIEU CHI + NHOM
        // =================================================

        private void ChonTieuChi(int i)
        {
            tieuChi = i;
            for (int k = 0; k < nutTieuChi.Count; k++)
            {
                bool chon = k == i;
                nutTieuChi[k].BackColor = chon ? TienIchGiaoDienSang.Teal : Color.White;
                nutTieuChi[k].ForeColor = chon ? Color.White : TienIchGiaoDienSang.Muted;
                nutTieuChi[k].FlatAppearance.MouseOverBackColor = chon ? TienIchGiaoDienSang.Teal : Color.FromArgb(242, 247, 252);
            }
            UpdateBreakdown();
        }

        private string KhoaNhom(DonViQuanDoi d)
        {
            string k;
            if (tieuChi == 1) k = !string.IsNullOrWhiteSpace(d.QuanHuyen) ? d.QuanHuyen : d.PhuongXa;
            else if (tieuChi == 2) k = d.QuanBinhChung;
            else k = d.LoaiDonVi;
            if (!string.IsNullOrWhiteSpace(k)) return k.Trim();
            return tieuChi == 2 ? "Chưa khai báo quân chủng" : "Chưa phân loại";
        }

        private List<NhomThongKe> GetGroups()
        {
            int tong = source.Count;
            return source.GroupBy(KhoaNhom, StringComparer.CurrentCultureIgnoreCase)
                .Select(g =>
                {
                    double tl = tong > 0 ? 100d * g.Count() / tong : 0d;
                    return new NhomThongKe
                    {
                        Ten = g.Key,
                        SoLuong = g.Count(),
                        TyLe = tl,
                        TyLeHienThi = tl.ToString("N1", VeBaoCao.Vi) + "%"
                    };
                })
                .OrderByDescending(g => g.SoLuong).ThenBy(g => g.Ten).ToList();
        }

        private void UpdateBreakdown()
        {
            if (summary == null) return;
            nhomHienTai = GetGroups();
            string tc = TenTieuChi[tieuChi].ToLower(VeBaoCao.Vi);
            kpiNhom.DatGiaTri(nhomHienTai.Count.ToString("N0", VeBaoCao.Vi), "theo " + tc);
            summary.DataSource = new BindingList<NhomThongKe>(nhomHienTai);
            summary.ClearSelection();
            summary.CurrentCell = null;
            lblBangPhu.Text = nhomHienTai.Count + " nhóm · " + source.Count.ToString("N0", VeBaoCao.Vi) + " hồ sơ";
            bieuDo.TieuDe = "Phân bố theo " + tc;
            bieuDo.Nhom = VeBaoCao.NhomBieuDo(nhomHienTai, source.Count);
            bieuDo.Invalidate();
            bool coDuLieu = source.Count > 0;
            btnIn.Enabled = btnPdf.Enabled = coDuLieu;
            btnIn.BackColor = coDuLieu ? TienIchGiaoDienSang.Blue : Color.FromArgb(200, 214, 228);
            btnPdf.BackColor = coDuLieu ? TienIchGiaoDienSang.Teal : Color.FromArgb(200, 214, 228);
        }

        // =================================================
        // BAO CAO: PDF + IN
        // =================================================

        private DuLieuBaoCaoThongKe ChupDuLieu()
        {
            return new DuLieuBaoCaoThongKe
            {
                TieuChi = TenTieuChi[tieuChi],
                ThoiGian = DateTime.Now,
                Tong = source.Count,
                CoToaDo = source.Count(NghiepVuDonVi.CoToaDoWgs84),
                ViTriThat = ViTriThat,
                DiemDaiDien = diemDaiDien,
                DatMuoiMet = viTriTot,
                Nhom = nhomHienTai.ToList(),
                DanhSach = source.ToList()
            };
        }

        private void XuatPdf()
        {
            if (source.Count == 0) return;
            const string mayPdf = "Microsoft Print to PDF";
            bool coMayPdf = PrinterSettings.InstalledPrinters.Cast<string>()
                .Any(p => string.Equals(p, mayPdf, StringComparison.OrdinalIgnoreCase));
            if (!coMayPdf)
            {
                MessageBox.Show(this,
                    "Máy chưa bật máy in ảo \"Microsoft Print to PDF\".\n\n" +
                    "Hãy bật trong Windows Features, hoặc bấm \"In báo cáo\" rồi chọn một máy in PDF khác.",
                    "Không xuất được PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string tep;
            using (SaveFileDialog dlg = new SaveFileDialog
            {
                Title = "Xuất báo cáo PDF",
                Filter = "Tệp PDF (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                AddExtension = true,
                OverwritePrompt = true,
                FileName = "BaoCaoThongKe_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".pdf"
            })
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                tep = dlg.FileName;
            }
            Cursor cu = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (File.Exists(tep)) File.Delete(tep);
                using (BaoCaoThongKeIn doc = new BaoCaoThongKeIn(ChupDuLieu()))
                {
                    doc.PrinterSettings.PrinterName = mayPdf;
                    doc.PrinterSettings.PrintToFile = true;
                    doc.PrinterSettings.PrintFileName = tep;
                    doc.PrintController = new StandardPrintController();   // khong hien hop "Dang in..."
                    doc.Print();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = cu;
                MessageBox.Show(this, "Không xuất được PDF: " + ex.Message, "Lỗi xuất PDF",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                Cursor.Current = cu;
            }
            status.Text = "Đã xuất báo cáo PDF: " + tep;
            if (File.Exists(tep) &&
                MessageBox.Show(this, "Đã lưu báo cáo:\n" + tep + "\n\nMở tệp PDF ngay?", "Xuất PDF",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                try { Process.Start(new ProcessStartInfo(tep) { UseShellExecute = true }); }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Không mở được tệp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void InBaoCao()
        {
            if (source.Count == 0) return;
            using (BaoCaoThongKeIn doc = new BaoCaoThongKeIn(ChupDuLieu()))
            using (PrintPreviewDialog xemTruoc = new PrintPreviewDialog
            {
                Document = doc,
                Text = "Xem trước báo cáo thống kê",
                ShowIcon = false,
                WindowState = FormWindowState.Maximized,
                UseAntiAlias = true
            })
            {
                // Thay nut in mac dinh (in thang ra may mac dinh) bang nut
                // "Chon may in va in..." mo hop thoai may in cua Windows.
                ToolStrip ts = xemTruoc.Controls.OfType<ToolStrip>().FirstOrDefault();
                if (ts != null)
                {
                    if (ts.Items.Count > 0) ts.Items[0].Visible = false;
                    ToolStripButton nutIn = new ToolStripButton("⎙  Chọn máy in và in…")
                    {
                        DisplayStyle = ToolStripItemDisplayStyle.Text,
                        Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
                    };
                    nutIn.Click += (s, e) =>
                    {
                        using (PrintDialog pd = new PrintDialog
                        {
                            Document = doc,
                            UseEXDialog = true,
                            AllowSomePages = true,
                            AllowSelection = false
                        })
                        {
                            pd.PrinterSettings.FromPage = 1;
                            pd.PrinterSettings.ToPage = 99;
                            if (pd.ShowDialog(xemTruoc) != DialogResult.OK) return;
                            try
                            {
                                doc.Print();
                                status.Text = "Đã gửi báo cáo tới máy in: " + doc.PrinterSettings.PrinterName;
                                xemTruoc.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(xemTruoc, ex.Message, "Lỗi in", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    };
                    ts.Items.Insert(0, nutIn);
                    ts.Items.Insert(1, new ToolStripSeparator());
                }
                xemTruoc.ShowDialog(this);
            }
        }

        // =================================================
        // HAM TAO CONTROL
        // =================================================

        private static string PhanTram(int a, int b)
        {
            return b > 0 ? (100d * a / b).ToString("N1", VeBaoCao.Vi) + "%" : "—";
        }

        private static Button NutPhang(string chu, Color nen, Color mauChu, float co)
        {
            Button b = new Button
            {
                Text = chu,
                BackColor = nen,
                ForeColor = mauChu,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", co, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(nen, 0.12f);
            return b;
        }

        private DataGridView TaoBang()
        {
            DataGridView g = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                GridColor = Color.FromArgb(236, 243, 249),
                EnableHeadersVisualStyles = false,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None
            };
            g.RowTemplate.Height = 34;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 34;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(242, 247, 252);
            g.ColumnHeadersDefaultCellStyle.ForeColor = TienIchGiaoDienSang.Muted;
            g.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.6F, FontStyle.Bold);
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.Font = new Font("Segoe UI", 9.3F);
            g.DefaultCellStyle.ForeColor = TienIchGiaoDienSang.Ink;
            g.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 246, 241);
            g.DefaultCellStyle.SelectionForeColor = TienIchGiaoDienSang.Ink;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 252, 254);

            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSTT", HeaderText = "#", FillWeight = 6, MinimumWidth = 40, SortMode = DataGridViewColumnSortMode.NotSortable });
            g.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nhóm", DataPropertyName = "Ten", FillWeight = 46, SortMode = DataGridViewColumnSortMode.NotSortable });
            DataGridViewTextBoxColumn sl = new DataGridViewTextBoxColumn { HeaderText = "Số hồ sơ", DataPropertyName = "SoLuong", FillWeight = 13, SortMode = DataGridViewColumnSortMode.NotSortable };
            sl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            sl.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            sl.DefaultCellStyle.Font = new Font("Segoe UI", 9.3F, FontStyle.Bold);
            g.Columns.Add(sl);
            DataGridViewTextBoxColumn tl = new DataGridViewTextBoxColumn { HeaderText = "Tỷ lệ", DataPropertyName = "TyLeHienThi", FillWeight = 11, SortMode = DataGridViewColumnSortMode.NotSortable };
            tl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            tl.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            g.Columns.Add(tl);
            g.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTyTrong", HeaderText = "Tỷ trọng", FillWeight = 24, SortMode = DataGridViewColumnSortMode.NotSortable });

            g.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                if (e.ColumnIndex == 0) { e.Value = (e.RowIndex + 1).ToString(); e.FormattingApplied = true; }
                else if (e.ColumnIndex == 4) { e.Value = string.Empty; e.FormattingApplied = true; }
            };
            // Cot "Ty trong": ve thanh ngang nho theo ty le.
            g.CellPainting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex != 4) return;
                // Ve DU nen + vien o (khong chi nen) -> khong de lai vet o dong vien.
                e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                NhomThongKe n = g.Rows[e.RowIndex].DataBoundItem as NhomThongKe;
                if (n != null)
                {
                    Rectangle r = e.CellBounds;
                    RectangleF tr = new RectangleF(r.Left + 8, r.Top + r.Height / 2f - 4, r.Width - 20, 8);
                    // Graphics nay DUNG CHUNG cho ca bang: phai tra lai trang thai cu,
                    // neu khong duong ke dong / khung focus ve sau bi khu rang cua -> nhoe thanh vet.
                    GraphicsState truoc = e.Graphics.Save();
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SolidBrush b = new SolidBrush(VeBaoCao.Track))
                    using (GraphicsPath p = VeBaoCao.BoTron(tr, 4)) e.Graphics.FillPath(b, p);
                    float w = Math.Max(8f, tr.Width * (float)(n.TyLe / 100d));
                    using (SolidBrush b = new SolidBrush(VeBaoCao.Teal))
                    using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(tr.Left, tr.Top, w, tr.Height), 4)) e.Graphics.FillPath(b, p);
                    e.Graphics.Restore(truoc);
                }
                e.Handled = true;
            };
            return g;
        }

        // =================================================
        // CONTROL TU VE
        // =================================================

        /// <summary>Nen trang bo goc + vien mong (dung cho nhom nut tieu chi).</summary>
        private sealed class KhungBoGoc : Panel
        {
            public KhungBoGoc()
            {
                DoubleBuffered = true;
                ResizeRedraw = true;
                BackColor = TienIchGiaoDienSang.Background;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(0.5f, 0.5f, Width - 1.5f, Height - 1.5f), 11))
                using (SolidBrush b = new SolidBrush(Color.White))
                using (Pen pen = new Pen(TienIchGiaoDienSang.Border))
                {
                    e.Graphics.FillPath(b, p);
                    e.Graphics.DrawPath(pen, p);
                }
            }
        }

        /// <summary>Lop co so: the trang bo goc tu ve.</summary>
        private abstract class TheVe : Control
        {
            protected TheVe()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = TienIchGiaoDienSang.Background;
                Font = new Font("Segoe UI", 9F);
            }

            protected RectangleF VeNen(Graphics g, Color nen)
            {
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                RectangleF r = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
                using (GraphicsPath p = VeBaoCao.BoTron(r, 14))
                using (SolidBrush b = new SolidBrush(nen))
                using (Pen pen = new Pen(Color.FromArgb(228, 236, 245)))
                {
                    g.FillPath(b, p);
                    g.DrawPath(pen, p);
                }
                return r;
            }
        }

        private sealed class BannerThongKe : Control
        {
            private string thoiGian = "—";
            public string ThoiGian { get { return thoiGian; } set { thoiGian = value; Invalidate(); } }

            public BannerThongKe()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = TienIchGiaoDienSang.Background;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                RectangleF r = new RectangleF(0, 0, Width - 1, Height - 6);
                using (LinearGradientBrush lg = new LinearGradientBrush(r, Color.FromArgb(0, 169, 138), Color.FromArgb(44, 104, 201), LinearGradientMode.Horizontal))
                using (GraphicsPath p = VeBaoCao.BoTron(r, 16))
                {
                    lg.InterpolationColors = new ColorBlend
                    {
                        Colors = new[] { Color.FromArgb(0, 169, 138), Color.FromArgb(28, 143, 208), Color.FromArgb(44, 104, 201) },
                        Positions = new[] { 0f, 0.6f, 1f }
                    };
                    g.FillPath(lg, p);
                    // Hoa van vong tron mo ben phai.
                    g.SetClip(p);
                    using (Pen vong = new Pen(Color.FromArgb(28, Color.White), 1.4f))
                        for (int i = 1; i <= 5; i++)
                            g.DrawEllipse(vong, r.Right - 150 - i * 26, r.Top + r.Height / 2 - i * 26, i * 52, i * 52);
                    g.ResetClip();
                }
                using (SolidBrush vang = new SolidBrush(Color.FromArgb(230, 249, 81)))
                    g.FillRectangle(vang, 0, 18, 5, Math.Max(0, Height - 42));
                using (Font f1 = new Font("Segoe UI", 19F, FontStyle.Bold))
                using (Font f2 = new Font("Segoe UI", 9.6F))
                using (Font f3 = new Font("Segoe UI", 8.6F))
                using (Font f4 = new Font("Segoe UI", 12F, FontStyle.Bold))
                using (SolidBrush trang = new SolidBrush(Color.White))
                using (SolidBrush trangMo = new SolidBrush(Color.FromArgb(225, Color.White)))
                using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString("THỐNG KÊ VÀ BÁO CÁO", f1, trang, 26, 14);
                    g.DrawString("Tổng hợp hồ sơ đơn vị · phân bố theo loại, địa bàn, quân chủng · báo cáo PDF và in",
                        f2, trangMo, 28, 52);
                    g.DrawString("CẬP NHẬT", f3, trangMo, new RectangleF(0, 22, Width - 26, 20), phai);
                    g.DrawString(thoiGian, f4, trang, new RectangleF(0, 40, Width - 26, 26), phai);
                }
            }
        }

        private sealed class TheChiSo : TheVe
        {
            private readonly string icon, tieuDe;
            private readonly Color mau, mauNhat;
            private string giaTri = "—", moTa = "";

            public TheChiSo(string icon, string tieuDe, Color mau, Color mauNhat)
            {
                this.icon = icon; this.tieuDe = tieuDe; this.mau = mau; this.mauNhat = mauNhat;
            }

            public void DatGiaTri(string gt, string mt) { giaTri = gt; moTa = mt; Invalidate(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                RectangleF r = VeNen(g, Color.White);
                RectangleF o = new RectangleF(r.Left + 16, r.Top + (r.Height - 46) / 2, 46, 46);
                using (SolidBrush b = new SolidBrush(mauNhat))
                using (GraphicsPath p = VeBaoCao.BoTron(o, 12)) g.FillPath(b, p);
                using (Font fi = new Font("Segoe UI", 15F, FontStyle.Bold))
                using (SolidBrush b = new SolidBrush(mau))
                using (StringFormat c = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    g.DrawString(icon, fi, b, o, c);
                float x = o.Right + 14;
                float w = r.Right - x - 10;
                using (Font ft = new Font("Segoe UI", 8.2F, FontStyle.Bold))
                using (Font fv = new Font("Segoe UI", 20F, FontStyle.Bold))
                using (Font fm = new Font("Segoe UI", 8.4F))
                using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
                using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
                using (StringFormat sf = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    float y = r.Top + (r.Height - 70) / 2;
                    g.DrawString(tieuDe, ft, sub, new RectangleF(x, y, w, 18), sf);
                    g.DrawString(giaTri, fv, ink, new RectangleF(x - 2, y + 15, w, 36), sf);
                    g.DrawString(moTa, fm, sub, new RectangleF(x, y + 51, w, 18), sf);
                }
            }
        }

        private sealed class TheBieuDoThanh : TheVe
        {
            public string TieuDe { get; set; }
            public List<NhomThongKe> Nhom { get; set; }

            public TheBieuDoThanh() { TieuDe = "Phân bố"; Nhom = new List<NhomThongKe>(); }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                RectangleF r = VeNen(g, Color.White);
                using (Font ft = new Font("Segoe UI", 11.5F, FontStyle.Bold))
                using (Font fp = new Font("Segoe UI", 8.6F))
                using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
                using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
                using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString(TieuDe, ft, ink, r.Left + 18, r.Top + 14);
                    g.DrawString(Nhom.Count >= 8 && Nhom[Nhom.Count - 1].Ten.StartsWith("Khác (", StringComparison.Ordinal)
                        ? "Top 7 · còn lại gộp \"Khác\"" : "Số hồ sơ · tỷ lệ",
                        fp, sub, new RectangleF(r.Left, r.Top + 18, r.Width - 18, 20), phai);
                }
                using (Font f = new Font("Segoe UI", 9.2F))
                using (Font fb = new Font("Segoe UI", 9.4F, FontStyle.Bold))
                using (Font fn = new Font("Segoe UI", 8.2F))
                    VeBaoCao.BieuDoThanh(g, new RectangleF(r.Left + 18, r.Top + 50, r.Width - 36, r.Height - 64), Nhom, f, fb, fn);
            }
        }

        private sealed class TheChatLuong : TheVe
        {
            private int? thuc, daiDien, tot;

            public void DatSoLieu(int? viTriThat, int? diemDaiDien, int? datMuoiMet)
            {
                thuc = viTriThat; daiDien = diemDaiDien; tot = datMuoiMet; Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                RectangleF r = VeNen(g, Color.White);
                using (Font ft = new Font("Segoe UI", 11.5F, FontStyle.Bold))
                using (Font fp = new Font("Segoe UI", 8.6F))
                using (Font f = new Font("Segoe UI", 9.2F))
                using (Font fb = new Font("Segoe UI", 9.6F, FontStyle.Bold))
                using (Font fl = new Font("Segoe UI", 17F, FontStyle.Bold))
                using (SolidBrush ink = new SolidBrush(VeBaoCao.Ink))
                using (SolidBrush sub = new SolidBrush(VeBaoCao.Sub))
                using (StringFormat phai = new StringFormat { Alignment = StringAlignment.Far })
                {
                    g.DrawString("Chất lượng vị trí trụ sở", ft, ink, r.Left + 18, r.Top + 14);
                    if (!thuc.HasValue || !daiDien.HasValue)
                    {
                        g.DrawString("Chưa đọc được thống kê vị trí từ PostGIS.", f, sub,
                            new RectangleF(r.Left + 18, r.Top + 60, r.Width - 36, 60));
                        return;
                    }
                    float d = Math.Min(150f, Math.Min(r.Width - 36, r.Height - 160));
                    RectangleF o = new RectangleF(r.Left + (r.Width - d) / 2, r.Top + 48, d, d);
                    VeBaoCao.BieuDoVanh(g, o, thuc.Value, daiDien.Value, fl, fp);
                    float y = o.Bottom + 14;
                    Dong(g, r, ref y, VeBaoCao.Teal, "Vị trí thật (đo / số hóa / địa chỉ)", thuc.Value, f, fb, ink, phai);
                    Dong(g, r, ref y, VeBaoCao.Amber, "Điểm đại diện xã/phường", daiDien.Value, f, fb, ink, phai);
                    Dong(g, r, ref y, VeBaoCao.Gray, "Trong đó đạt sai số ≤ 10 m", tot ?? 0, f, fb, ink, phai);
                }
            }

            private static void Dong(Graphics g, RectangleF r, ref float y, Color mau, string ten, int so,
                Font f, Font fb, Brush ink, StringFormat phai)
            {
                using (SolidBrush b = new SolidBrush(mau))
                using (GraphicsPath p = VeBaoCao.BoTron(new RectangleF(r.Left + 20, y + 5, 10, 10), 3)) g.FillPath(b, p);
                g.DrawString(ten, f, ink, r.Left + 38, y);
                g.DrawString(so.ToString("N0", VeBaoCao.Vi), fb, ink, new RectangleF(r.Left, y, r.Width - 20, 22), phai);
                y += 24;
            }
        }
    }
}
