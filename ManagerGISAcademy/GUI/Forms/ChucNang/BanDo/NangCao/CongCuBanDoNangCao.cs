using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MapWinGIS;

using WinImage = System.Drawing.Image;
using GisImage = MapWinGIS.Image;
using WinLabel = System.Windows.Forms.Label;
using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Ban do GIS nang cap.
    /// Giu nguyen cac chuc nang MapWinGIS, PostGIS va do dac.
    /// Thanh zoom doc duoc dat duoi 5 nut cong cu, cach 12 px.
    /// C# 7.3 - .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {
        private bool bd11DaDangKy;
        private bool bd11DaKhoiTao;
        private bool bd11DangCan;
        private bool bd11HienNhan = true;
        private bool bd11DangDongBoMenu;
        private bool bd11DangDoiNenTrang;
        private bool bd11DaThuKhoiPhucRanh;

        private System.Windows.Forms.Timer bd11HenKhoiTao;
        private System.Windows.Forms.Timer bd11HenDongBo;
        private DateTime bd11BatDau;

        private Button bd11NutChon;
        private Button bd11NutKeo;
        private Button bd11NutDo;

        private Panel bd11KhungZoom;
        private TrackBar bd11Zoom;
        private int bd11ZoomCu = 5;
        private bool bd11DangKeoZoom;
        private bool bd11DangDongBoZoom;
        private System.Windows.Forms.Timer bd11HenZoomDoc;

        private ContextMenuStrip bd11MenuDo;
        private ContextMenuStrip bd11MenuLop;
        private ToolStripMenuItem bd11MucHienNhan;
        private ToolStripMenuItem bd11MucNenTrang;
        private ToolStripMenuItem bd11MucHienTinh;
        private ToolStripMenuItem bd11MucHienHuyen;
        private ToolStripMenuItem bd11MucPhongTinh;
        private ToolStripMenuItem bd11MucPhongHuyen;

        private int bd11HandleNhan = -1;
        private int bd11SoDiemNhan = -1;
        private int bd11HandleKyHieu = -1;
        private bool bd11NhanCanKiemTra = true;

        private bool bd11ChoVeLai;
        private bool bd11CanVeKhiTroLai;

        private int bd11HandleTinh = -1;
        private int bd11HandleHuyen = -1;
        private int bd11HandleRasterDaKiemTra = -1;

        private string bd11ThuMucIcons;
        private PictureBox bd11BieuTuongLop;

        private readonly List<PictureBox> bd11DanhSachIcon =
            new List<PictureBox>();

        private readonly ToolTip bd11ChuThich = new ToolTip();

        private readonly Color bd11Nen =
            Color.FromArgb(12, 27, 47);

        private readonly Color bd11Xanh =
            Color.FromArgb(0, 148, 112);

        private readonly Color bd11MauTinh =
            Color.FromArgb(35, 225, 184);

        private readonly Color bd11MauHuyen =
            Color.FromArgb(245, 202, 107);

        private int bd11HandleAnhGoc = -1;
        private bool bd11CoTrangThaiAnhGoc;
        private bool bd11UseTransparencyGoc;
        private uint bd11MauAnhGoc1;
        private uint bd11MauAnhGoc2;

        // =====================================================
        // 1. CAU HINH DUONG DAN
        // =====================================================

        private string Bd11_ThuMucCauHinh
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "ManagerGISAcademy");
            }
        }

        private string Bd11_TepRanhTinh
        {
            get
            {
                return Path.Combine(
                    Bd11_ThuMucCauHinh,
                    "ranh-gioi-ha-noi.txt");
            }
        }

        private string Bd11_TepRanhHuyen
        {
            get
            {
                return Path.Combine(
                    Bd11_ThuMucCauHinh,
                    "ranh-gioi-quan-huyen.txt");
            }
        }

        // =====================================================
        // 2. KHOI TAO BAN DO
        // =====================================================

        private void BdPro_KhoiTaoGiaoDien()
        {
            if (bd11DaDangKy || DesignMode)
                return;

            bd11DaDangKy = true;
            Shown += Bd11_KhiMoForm;
            FormClosed += Bd11_KhiDongForm;
        }

        private void Bd11_KhiMoForm(object sender, EventArgs e)
        {
            bd11BatDau = DateTime.Now;

            bd11HenKhoiTao = new System.Windows.Forms.Timer
            {
                Interval = 350
            };

            bd11HenKhoiTao.Tick += Bd11_ThuKhoiTao;
            bd11HenKhoiTao.Start();

            Bd11_ThuKhoiTao(this, EventArgs.Empty);
        }

        private void Bd11_ThuKhoiTao(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || bd11DaKhoiTao)
            {
                Bd11_HuyHenKhoiTao();
                return;
            }

            if (pnlMapCanvas == null ||
                pnlMapTools == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null ||
                !daKhoiTaoThongTinBanDo ||
                !daKhoiTaoDoDac ||
                pnlCheDoBanDo == null ||
                pnlThanhDoDac == null)
            {
                if ((DateTime.Now - bd11BatDau).TotalSeconds > 25)
                {
                    Bd11_HuyHenKhoiTao();
                    GhiNhatKyBanDo(
                        "BD11_TIMEOUT: modules not ready");
                }

                return;
            }

            Bd11_HuyHenKhoiTao();

            try
            {
                bd11ThuMucIcons = Bd11_TimThuMucIcons();

                Bd11_TaoThanhDoc();
                Bd11_TaoZoomDoc();
                Bd11_TaoMenu();

                Bd11_DatMauNenBanDo();
                Bd11_AnThanhTrung();
                Bd11_CanChinh();

                if (pnlRightCard != null && pnlRightCard.Visible)
                {
                    DoiHienThiBangLop();
                }

                pnlMapCanvas.Resize += (s, a) =>
                {
                    Bd11_CanChinh();
                };

                btnNavMap.Click += (s, a) =>
                {
                    if (!IsHandleCreated || IsDisposed || Disposing)
                        return;

                    BeginInvoke(
                        new MethodInvoker(Bd11_KhiVeBanDo));
                };

                bd11DaKhoiTao = true;

                bd11HenDongBo = new System.Windows.Forms.Timer
                {
                    Interval = 8000
                };

                bd11HenDongBo.Tick += (s, a) =>
                {
                    if (IsDisposed || Disposing)
                        return;

                    if (!bd11DaThuKhoiPhucRanh &&
                        daKhoiTaoBanDoNen)
                    {
                        bd11DaThuKhoiPhucRanh = true;
                        Bd11_KhoiPhucHaiLopRanhGioi();
                    }

                    if (phanHeHienTai != "Bản đồ GIS")
                        return;

                    if (handleLopDiemPostGIS >= 0 &&
                        (bd11NhanCanKiemTra ||
                         bd11HandleNhan != handleLopDiemPostGIS))
                    {
                        Bd11_GanNhanDonVi();
                    }

                    Bd11_KiemTraRasterMotLan();
                };

                bd11HenDongBo.Start();

                Bd11_GanNhanDonVi();
                Bd11_KiemTraRasterMotLan();

                // Thanh zoom duoc can sau khi cong cu san sang.
                Bd11_DatZoomBenDuoi();

                GhiNhatKyBanDo("BD11_PERFORMANCE_READY");
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("BD11_INIT_ERROR | " + ex);

                statusGeneral.Text =
                    "● Lỗi khởi tạo bản đồ: " + ex.Message;
            }
        }

        private void Bd11_HuyHenKhoiTao()
        {
            if (bd11HenKhoiTao == null)
                return;

            bd11HenKhoiTao.Stop();
            bd11HenKhoiTao.Tick -= Bd11_ThuKhoiTao;
            bd11HenKhoiTao.Dispose();
            bd11HenKhoiTao = null;
        }

        // =====================================================
        // 3. TAI ICON PNG GOC
        // =====================================================

        private string Bd11_TimThuMucIcons()
        {
            try
            {
                DirectoryInfo thuMuc = new DirectoryInfo(
                    AppDomain.CurrentDomain.BaseDirectory);

                for (int i = 0; i < 10 && thuMuc != null; i++)
                {
                    string duongDan = Path.Combine(
                        thuMuc.FullName, "Resources", "Icons");

                    if (Directory.Exists(duongDan))
                        return duongDan;

                    thuMuc = thuMuc.Parent;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_ICON_FOLDER_ERROR | " + ex.Message);
            }

            return null;
        }

        private WinImage Bd11_TaoAnhTuIcon(
            string tenFile, int rong, int cao)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(bd11ThuMucIcons))
                    return null;

                string duongDan = Path.Combine(
                    bd11ThuMucIcons, tenFile);

                if (!File.Exists(duongDan))
                    return null;

                using (WinImage goc = WinImage.FromFile(duongDan))
                {
                    Bitmap bmp = new Bitmap(rong, cao);

                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.Clear(Color.Transparent);

                        g.InterpolationMode =
                            System.Drawing.Drawing2D
                                .InterpolationMode.HighQualityBicubic;

                        g.SmoothingMode =
                            System.Drawing.Drawing2D
                                .SmoothingMode.AntiAlias;

                        g.DrawImage(
                            goc, new Rectangle(0, 0, rong, cao));
                    }

                    return bmp;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_ICON_LOAD_ERROR | " + ex.Message);
                return null;
            }
        }

        private PictureBox Bd11_GanIconNeuCo(
            Button nut, string tenFile, string textMacDinh)
        {
            if (nut == null || nut.IsDisposed)
                return null;

            WinImage anh = Bd11_TaoAnhTuIcon(tenFile, 22, 22);

            if (anh == null)
            {
                nut.Text = textMacDinh;
                return null;
            }

            PictureBox bieuTuong = null;

            try
            {
                nut.Image = null;
                nut.Text = string.Empty;

                bieuTuong = new PictureBox
                {
                    Size = new Size(26, 26),
                    BackColor = Color.Transparent,
                    Image = anh,
                    SizeMode = PictureBoxSizeMode.CenterImage,
                    Cursor = Cursors.Hand,
                    TabStop = false
                };

                Action canGiua = () =>
                {
                    if (bieuTuong.IsDisposed)
                        return;

                    bieuTuong.Location = new WinPoint(
                        Math.Max(0,
                            (nut.ClientSize.Width - bieuTuong.Width) / 2),
                        Math.Max(0,
                            (nut.ClientSize.Height - bieuTuong.Height) / 2));
                };

                bieuTuong.Click += (s, e) =>
                {
                    if (!nut.IsDisposed && nut.Enabled && nut.Visible)
                        nut.PerformClick();
                };

                nut.Resize += (s, e) => canGiua();
                nut.Controls.Add(bieuTuong);

                canGiua();
                bieuTuong.BringToFront();

                bd11ChuThich.SetToolTip(
                    bieuTuong, bd11ChuThich.GetToolTip(nut));

                bd11DanhSachIcon.Add(bieuTuong);
                return bieuTuong;
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BD11_ICON_ATTACH_ERROR | " + ex.Message);

                if (bieuTuong != null)
                {
                    if (bieuTuong.Parent != null)
                        bieuTuong.Parent.Controls.Remove(bieuTuong);

                    bieuTuong.Image = null;
                    bieuTuong.Dispose();
                }

                anh.Dispose();
                nut.Text = textMacDinh;
                return null;
            }
        }

        // =====================================================
        // 4. THANH 5 NUT CONG CU
        // =====================================================

        private void Bd11_KieuNut(
            Button nut, string kyHieu, int y, string moTa)
        {
            nut.AutoSize = false;
            nut.AutoEllipsis = false;
            nut.Text = kyHieu;
            nut.TextAlign = ContentAlignment.MiddleCenter;

            nut.SetBounds(5, y, 39, 38);
            nut.Margin = Padding.Empty;
            nut.Padding = Padding.Empty;

            nut.Font = new Font(
                "Segoe UI Symbol", 12F, FontStyle.Regular);

            nut.FlatStyle = FlatStyle.Flat;
            nut.FlatAppearance.BorderSize = 0;
            nut.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(31, 75, 81);
            nut.FlatAppearance.MouseDownBackColor = bd11Xanh;

            nut.ForeColor = Color.White;
            nut.BackColor = bd11Nen;
            nut.Cursor = Cursors.Hand;
            nut.TabStop = false;
            nut.UseVisualStyleBackColor = false;

            bd11ChuThich.SetToolTip(nut, moTa);
        }

        private Button Bd11_TaoNut(
            string kyHieu, int y, string moTa)
        {
            Button nut = new Button();
            Bd11_KieuNut(nut, kyHieu, y, moTa);
            pnlMapTools.Controls.Add(nut);
            return nut;
        }

        private void Bd11_TaoThanhDoc()
        {
            pnlMapTools.SuspendLayout();

            try
            {
                // Vi tri thong nhat voi ToiUuBanDo.cs.
                pnlMapTools.SetBounds(17, 16, 49, 235);
                pnlMapTools.BackColor =
                    Color.FromArgb(7, 20, 36);
                pnlMapTools.BorderStyle = BorderStyle.FixedSingle;

                if (btnNapLop != null)
                {
                    btnNapLop.Visible = false;
                    toolTip.SetToolTip(btnNapLop, "");
                }

                toolTip.SetToolTip(btnMapLayers, "");

                // Hai nut zoom rieng van giu de tuong thich.
                Bd11_KieuNut(btnZoomIn, "+", 4, "Phóng to");
                Bd11_KieuNut(btnZoomOut, "−", 44, "Thu nhỏ");

                // Chi hien thi 5 nut sau.
                Bd11_KieuNut(
                    btnFullExtent, "□", 6, "Toàn cảnh");

                bd11NutChon = Bd11_TaoNut(
                    "◎", 51, "Chọn điểm đơn vị");

                bd11NutKeo = Bd11_TaoNut(
                    "✥", 96, "Di chuyển bản đồ");

                bd11NutDo = Bd11_TaoNut(
                    "⌁", 141, "Đo khoảng cách / diện tích");

                Bd11_KieuNut(
                    btnMapLayers, "≡", 186, "Quản lý lớp bản đồ");

                Bd11_GanIconNeuCo(
                    btnZoomIn, "zoom_in.png", "+");

                Bd11_GanIconNeuCo(
                    btnZoomOut, "zoom_out.png", "−");

                btnZoomIn.Visible = false;
                btnZoomOut.Visible = false;
                btnZoomIn.Enabled = false;
                btnZoomOut.Enabled = false;

                Bd11_GanIconNeuCo(
                    btnFullExtent, "full_extent.png", "□");

                Bd11_GanIconNeuCo(
                    bd11NutChon, "info.png", "◎");

                Bd11_GanIconNeuCo(
                    bd11NutKeo, "pan.png", "✥");

                Bd11_GanIconNeuCo(
                    bd11NutDo, "measure.png", "⌁");

                bd11BieuTuongLop = Bd11_GanIconNeuCo(
                    btnMapLayers, "open_layer.png", "≡");

                bd11NutChon.Click +=
                    (s, e) => Bd11_ChonCheDo(true);

                bd11NutKeo.Click +=
                    (s, e) => Bd11_ChonCheDo(false);

                bd11NutDo.Click += (s, e) =>
                {
                    if (bd11MenuDo != null)
                    {
                        bd11MenuDo.Show(
                            bd11NutDo, new WinPoint(38, 0));
                    }
                };

                Bd11_DanhDauChon(true);
                pnlMapTools.BringToFront();
            }
            finally
            {
                pnlMapTools.ResumeLayout(true);
            }
        }
    }
}