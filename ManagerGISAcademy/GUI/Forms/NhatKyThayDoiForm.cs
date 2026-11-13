using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ManagerGISAcademy.DAL;
using ManagerGISAcademy.DTO;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Màn xem nhật ký thay đổi hồ sơ đơn vị (chỉ đọc).
    /// Trên: danh sách lần Thêm/Sửa/Xóa, mới nhất trước.
    /// Dưới: từng trường giá trị cũ → mới của dòng đang chọn.
    /// </summary>
    public sealed class NhatKyThayDoiForm : Form
    {
        private const int GioiHanMacDinh = 500;

        private static readonly Dictionary<string, string> TenCot =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ma_don_vi", "Mã đơn vị" },
                { "ten_don_vi", "Tên đơn vị" },
                { "ten_viet_tat", "Tên viết tắt" },
                { "loai_don_vi", "Loại đơn vị" },
                { "quan_binh_chung", "Quân/Binh chủng" },
                { "don_vi_cap_tren", "Đơn vị cấp trên" },
                { "quan_huyen", "Địa bàn" },
                { "dia_chi", "Địa chỉ" },
                { "kinh_do", "Kinh độ" },
                { "vi_do", "Vĩ độ" },
                { "chi_huy_truong", "Chỉ huy trưởng" },
                { "chinh_uy", "Chính ủy" },
                { "so_dien_thoai", "Số điện thoại" },
                { "trang_thai_sscd", "Trạng thái SSCĐ" },
                { "do_mat", "Độ mật" },
                { "nhiem_vu_chinh", "Nhiệm vụ chính" },
                { "vu_khi_trang_bi", "Vũ khí trang bị" }
            };

        private readonly TruyXuatNhatKy truyXuat = new TruyXuatNhatKy();

        private readonly TextBox txtMa;
        private readonly ComboBox cboThaoTac;
        private readonly Label lblTongKet;
        private readonly DataGridView bangNhatKy;
        private readonly DataGridView bangChiTiet;
        private readonly Label lblChiTiet;

        public NhatKyThayDoiForm(string maDonVi)
        {
            Text = "Nhật ký thay đổi hồ sơ đơn vị";
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MinimizeBox = false;
            Font = new Font("Segoe UI", 9.5F);
            BackColor = Color.White;
            ClientSize = new Size(980, 620);
            MinimumSize = new Size(720, 480);

            // Esc dong hop thoai.
            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true;
                    Close();
                }
            };

            // ---- Thanh loc ----
            FlowLayoutPanel loc = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 46,
                Padding = new Padding(10, 9, 10, 0),
                WrapContents = false,
                BackColor = Color.FromArgb(244, 249, 250)
            };

            loc.Controls.Add(new Label
            {
                Text = "Mã đơn vị:",
                AutoSize = true,
                Margin = new Padding(0, 6, 4, 0)
            });

            txtMa = new TextBox
            {
                Width = 170,
                Text = maDonVi ?? string.Empty,
                Margin = new Padding(0, 2, 14, 0)
            };
            loc.Controls.Add(txtMa);

            loc.Controls.Add(new Label
            {
                Text = "Thao tác:",
                AutoSize = true,
                Margin = new Padding(0, 6, 4, 0)
            });

            cboThaoTac = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 110,
                Margin = new Padding(0, 2, 14, 0)
            };
            cboThaoTac.Items.AddRange(new object[] { "Tất cả", "Thêm", "Sửa", "Xóa" });
            cboThaoTac.SelectedIndex = 0;
            loc.Controls.Add(cboThaoTac);

            Button btnTai = new Button
            {
                Text = "↻ Tải nhật ký",
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 166, 145),
                ForeColor = Color.White,
                Margin = new Padding(0, 0, 14, 0),
                Cursor = Cursors.Hand
            };
            btnTai.FlatAppearance.BorderSize = 0;
            btnTai.Click += (s, e) => TaiNhatKy();
            loc.Controls.Add(btnTai);

            lblTongKet = new Label
            {
                AutoSize = true,
                ForeColor = Color.FromArgb(78, 99, 110),
                Margin = new Padding(0, 6, 0, 0)
            };
            loc.Controls.Add(lblTongKet);

            // ---- Bang nhat ky ----
            bangNhatKy = TaoBang();
            bangNhatKy.Columns.Add(TaoCot("Thời điểm", 15F));
            bangNhatKy.Columns.Add(TaoCot("Thao tác", 8F));
            bangNhatKy.Columns.Add(TaoCot("Mã đơn vị", 12F));
            bangNhatKy.Columns.Add(TaoCot("Tên đơn vị", 25F));
            bangNhatKy.Columns.Add(TaoCot("Trường thay đổi", 28F));
            bangNhatKy.Columns.Add(TaoCot("Tài khoản CSDL", 12F));
            bangNhatKy.CellFormatting += BangNhatKy_ToMau;
            bangNhatKy.SelectionChanged += (s, e) => HienChiTiet();

            // ---- Bang chi tiet ----
            lblChiTiet = new Label
            {
                Dock = DockStyle.Top,
                Height = 30,
                Padding = new Padding(10, 7, 0, 0),
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 54, 69),
                BackColor = Color.FromArgb(244, 249, 250),
                Text = "Chọn một dòng để xem giá trị cũ → mới"
            };

            bangChiTiet = TaoBang();
            bangChiTiet.Columns.Add(TaoCot("Trường", 20F));
            bangChiTiet.Columns.Add(TaoCot("Giá trị cũ", 40F));
            bangChiTiet.Columns.Add(TaoCot("Giá trị mới", 40F));

            SplitContainer chia = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterWidth = 6,
                BackColor = Color.FromArgb(218, 232, 238)
            };
            chia.Panel1.Controls.Add(bangNhatKy);
            chia.Panel2.Controls.Add(bangChiTiet);
            chia.Panel2.Controls.Add(lblChiTiet);

            Controls.Add(chia);
            Controls.Add(loc);

            Shown += (s, e) =>
            {
                chia.SplitterDistance = (int)(chia.Height * 0.58);
                TaiNhatKy();
            };

            txtMa.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TaiNhatKy();
                    e.SuppressKeyPress = true;
                }
            };
            cboThaoTac.SelectedIndexChanged += (s, e) =>
            {
                if (Visible) TaiNhatKy();
            };
        }

        private void TaiNhatKy()
        {
            Cursor = Cursors.WaitCursor;

            try
            {
                if (!truyXuat.BangNhatKyTonTai())
                {
                    bangNhatKy.Rows.Clear();
                    bangChiTiet.Rows.Clear();
                    lblTongKet.Text =
                        "CSDL chưa có bảng nhật ký - hãy chạy Database\\04_NhatKyVaRangBuoc.sql.";
                    return;
                }

                string[] ma = { null, "I", "U", "D" };

                IList<NhatKyThayDoi> ds = truyXuat.LayNhatKy(
                    txtMa.Text,
                    ma[Math.Max(0, cboThaoTac.SelectedIndex)],
                    GioiHanMacDinh);

                bangNhatKy.SuspendLayout();
                bangNhatKy.Rows.Clear();

                foreach (NhatKyThayDoi n in ds)
                {
                    string truong =
                        n.ThaoTac == "U"
                            ? (n.TruongThayDoi.Length == 0
                                ? "(không đổi dữ liệu nghiệp vụ)"
                                : string.Join(", ", n.TruongThayDoi.Select(TenHienThi)))
                            : (n.ThaoTac == "I" ? "(tạo mới hồ sơ)" : "(xóa hồ sơ)");

                    int i = bangNhatKy.Rows.Add(
                        n.ThoiDiem.ToString("dd/MM/yyyy HH:mm:ss"),
                        n.TenThaoTac,
                        n.MaDonVi,
                        n.TenDonVi,
                        truong,
                        n.TaiKhoanDb);

                    bangNhatKy.Rows[i].Tag = n;
                }

                bangNhatKy.ResumeLayout();

                // Rows.Add chon san dong dau TRUOC khi gan Tag, nen
                // SelectionChanged luc do khong co du lieu: hien lai.
                HienChiTiet();

                lblTongKet.Text =
                    ds.Count + " lần thay đổi" +
                    (ds.Count >= GioiHanMacDinh
                        ? " (chỉ hiện " + GioiHanMacDinh + " lần mới nhất)"
                        : "") +
                    " · Thêm " + ds.Count(x => x.ThaoTac == "I") +
                    " · Sửa " + ds.Count(x => x.ThaoTac == "U") +
                    " · Xóa " + ds.Count(x => x.ThaoTac == "D");

                if (bangNhatKy.Rows.Count == 0)
                {
                    bangChiTiet.Rows.Clear();
                    lblChiTiet.Text = "Không có thay đổi nào khớp bộ lọc.";
                }
            }
            catch (Exception ex)
            {
                lblTongKet.Text = "Không đọc được nhật ký: " + ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void HienChiTiet()
        {
            NhatKyThayDoi n =
                bangNhatKy.CurrentRow == null
                    ? null
                    : bangNhatKy.CurrentRow.Tag as NhatKyThayDoi;

            bangChiTiet.Rows.Clear();

            if (n == null)
                return;

            lblChiTiet.Text =
                n.TenThaoTac + " " + n.MaDonVi + " lúc " +
                n.ThoiDiem.ToString("dd/MM/yyyy HH:mm:ss") +
                " bởi " + n.TaiKhoanDb;

            try
            {
                foreach (ChiTietThayDoi c in truyXuat.LayChiTiet(n.Id))
                {
                    bangChiTiet.Rows.Add(
                        TenHienThi(c.TenCot),
                        c.GiaTriCu ?? "",
                        c.GiaTriMoi ?? "");
                }
            }
            catch (Exception ex)
            {
                lblChiTiet.Text = "Không đọc được chi tiết: " + ex.Message;
            }
        }

        private static string TenHienThi(string cot)
        {
            string ten;
            return cot != null && TenCot.TryGetValue(cot, out ten) ? ten : cot;
        }

        private void BangNhatKy_ToMau(
            object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 1 || e.Value == null)
                return;

            switch (e.Value.ToString())
            {
                case "Thêm": e.CellStyle.ForeColor = Color.FromArgb(0, 146, 126); break;
                case "Sửa": e.CellStyle.ForeColor = Color.FromArgb(37, 125, 221); break;
                case "Xóa": e.CellStyle.ForeColor = Color.FromArgb(183, 69, 75); break;
            }
        }

        private static DataGridView TaoBang()
        {
            DataGridView g = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 32,
                ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ShowCellToolTips = true
            };

            g.RowTemplate.Height = 28;
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(25, 54, 69);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(215, 246, 238);
            g.DefaultCellStyle.SelectionForeColor = Color.FromArgb(9, 73, 65);
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 251);
            return g;
        }

        private static DataGridViewTextBoxColumn TaoCot(string tieuDe, float tyLe)
        {
            return new DataGridViewTextBoxColumn
            {
                HeaderText = tieuDe,
                FillWeight = tyLe,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
        }
    }
}
