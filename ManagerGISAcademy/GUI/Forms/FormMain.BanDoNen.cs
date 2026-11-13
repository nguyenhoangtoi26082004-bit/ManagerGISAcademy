
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MapWinGIS;
using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Quan ly 4 che do ban do nen:
    /// 0 - GeoTIFF ngoai tuyen
    /// 1 - OpenStreetMap truc tuyen
    /// 2 - Khong co nen
    /// 3 - GeoServer WMTS
    ///
    /// C# 7.3 / .NET Framework 4.8 / MapWinGIS.
    /// </summary>
    public partial class FormMain
    {
        /// <summary>
        /// Thu muc anh nen ngoai tuyen (GeoTIFF EPSG:32648), tim theo thu tu:
        /// 1) bien moi truong MANAGERGISACADEMY_BASEMAPS;
        /// 2) &lt;thu muc chay&gt;\Basemaps;
        /// 3) DuLieu\Basemaps trong repo (di nguoc len tu thu muc chay);
        /// 4) thu muc phat trien cu. Khong co thi ung dung dung OSM truc tuyen.
        /// </summary>
        private static readonly string ThuMucNenMacDinh = TimThuMucNen();

        private static string TimThuMucNen()
        {
            try
            {
                string bien = Environment.GetEnvironmentVariable("MANAGERGISACADEMY_BASEMAPS");
                if (!string.IsNullOrWhiteSpace(bien) && Directory.Exists(bien)) return bien;
                string goc = AppDomain.CurrentDomain.BaseDirectory;
                string canh = Path.Combine(goc, "Basemaps");
                if (Directory.Exists(canh)) return canh;
                DirectoryInfo d = new DirectoryInfo(goc);
                for (int i = 0; i < 6 && d != null; i++, d = d.Parent)
                {
                    string repo = Path.Combine(d.FullName, "DuLieu", "Basemaps");
                    if (Directory.Exists(repo)) return repo;
                }
            }
            catch { }
            return @"D:\aNAM5\HK1\HeQuanTriCSDLKG\MapWinGIS_TOI\Basemaps";
        }

        private const string TepNenHaiMet =
            "HANOI_SATELLITE_2M_TILED_32648.tif";

        private const string TepNenNhe =
            "HANOI_SATELLITE_32648.tif";

        private const string NenVeTinh =
            "Ảnh vệ tinh Hà Nội (ngoại tuyến)";

        private const string NenOSM =
            "OpenStreetMap (trực tuyến)";

        private const string NenTrong = "Không có nền";

        private const string NenTileNoiBo =
            "Ảnh vệ tinh Hà Nội (Tile Server)";

        private const int CheDoNenTileNoiBo = 3;
        private const int MaNhaCungCapOSMNoiBo = 11301;

        // Du phong neu MapWinGIS khong nhan tile qua proxy localhost.
        private const int MaNhaCungCapOSMTrucTiep = 11303;

        private const string BdNen_OSMUrlTrucTiep =
            "https://tile.openstreetmap.org/{zoom}/{x}/{y}.png";

        private ComboBox cboChonBanDoNen;
        private Button btnChonTepBanDoNen;
        private Button btnChanDoanRaster;
        // Cho phep tai lai tile dang xem ma khong doi he toa do ban do.
        private Button btnTaiLaiTileNen;
        private Label lblNguonOSM;

        private int handleAnhNenTuDong = -1;
        private bool dangChuyenBanDoNen;
        private bool daKhoiTaoBanDoNen;
        private bool dangNapAnhNen;
        private bool bdNenDangKhoiDongTuDong;

        private int cheDoBanDoNenDangDung = -1;
        private string duongDanAnhNenDangNap;

        private MayChuTrungGianOSM mayChuOSM;

        private System.Windows.Forms.Timer bdNenHenKiemTraOSM;

        // Tu dong ket noi lai su kien va phuc hoi Tile
        // khi cac partial FormMain thay doi giao dien.
        private System.Windows.Forms.Timer bdNenHenDongBoChon;
        private ComboBox bdNenComboDaGanSuKien;
        private DateTime bdNenThoiDiemTuSuaCuoi = DateTime.MinValue;
        private int bdNenSoLanKhoiPhucLienTiep;
        private string bdNenLoiChuyenGanNhat = "";

        private int bdNenSoYeuCauOSMLucBat;
        private bool bdNenDangXuLyChonTuNguoiDung;
        private int bdNenSoTileOSMThanhCongLucBat;
        private int bdNenSoTileOSMThatBaiLucBat;
        private bool bdNenDaGanTheoDoiPhamVi;
        private bool bdNenDangDungOSMTrucTiep;
        private double bdNenDungLuongOSMRamBanDau;
        private double bdNenDungLuongOSMDiskBanDau;

        private string ThuMucCauHinhNen
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "ManagerGISAcademy");
            }
        }

        private string DuongDanCauHinhBanDoNen
        {
            get
            {
                return Path.Combine(
                    ThuMucCauHinhNen,
                    "duong-dan-ban-do-nen.txt");
            }
        }

        private string DuongDanCheDoNen
        {
            get
            {
                return Path.Combine(
                    ThuMucCauHinhNen,
                    "che-do-ban-do-nen.txt");
            }
        }

        // =====================================================
        // 1. KHOI DONG
        // =====================================================

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Shown += FormMain_BanDoNen_Shown;

            // Timer du phong OSM phai ton tai ke ca khi
            // ComboBox duoc tao san boi partial FormMain khac.
            if (bdNenHenKiemTraOSM == null)
            {
                bdNenHenKiemTraOSM =
                    new System.Windows.Forms.Timer();

                bdNenHenKiemTraOSM.Interval = 6500;
                bdNenHenKiemTraOSM.Tick +=
                    BdNen_HenKiemTraOSM_Tick;
            }

            // Timer UI chay cung thread WinForms.
            // Khong thao tac AxMap tu thread khac.
            if (bdNenHenDongBoChon == null)
            {
                bdNenHenDongBoChon =
                    new System.Windows.Forms.Timer();

                bdNenHenDongBoChon.Interval = 900;
                bdNenHenDongBoChon.Tick +=
                    BdNen_HenDongBoChon_Tick;

                bdNenHenDongBoChon.Start();
            }

            FormClosed += (s, args) =>
            {
                if (bdNenHenKiemTraOSM != null)
                {
                    bdNenHenKiemTraOSM.Stop();
                    bdNenHenKiemTraOSM.Dispose();
                    bdNenHenKiemTraOSM = null;
                }

                if (bdNenHenDongBoChon != null)
                {
                    bdNenHenDongBoChon.Stop();
                    bdNenHenDongBoChon.Dispose();
                    bdNenHenDongBoChon = null;
                }

                if (bdNenComboDaGanSuKien != null &&
                    !bdNenComboDaGanSuKien.IsDisposed)
                {
                    bdNenComboDaGanSuKien.SelectedIndexChanged -=
                        BdNen_CboChon_SelectedIndexChanged;

                    bdNenComboDaGanSuKien.SelectionChangeCommitted -=
                        BdNen_CboChon_SelectionChangeCommitted;

                    bdNenComboDaGanSuKien.DropDownClosed -=
                        BdNen_CboChon_DropDownClosed;
                }

                bdNenComboDaGanSuKien = null;

                if (bdNenDaGanTheoDoiPhamVi && banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.ExtentsChanged -= BdNen_KhiPhamViThayDoi;
                    bdNenDaGanTheoDoiPhamVi = false;
                }

                if (mayChuOSM != null)
                {
                    mayChuOSM.Dispose();
                    mayChuOSM = null;
                }
            };
        }


        private void FormMain_BanDoNen_Shown(
            object sender,
            EventArgs e)
        {
            if (daKhoiTaoBanDoNen || IsDisposed || Disposing)
                return;

            daKhoiTaoBanDoNen = true;

            if (!IsHandleCreated)
                return;

            // Cho WinForms du mot nhip de ve giao dien truoc khi nap nen.
            // Timer nay chay tren UI thread; khong day ActiveX sang Task.Run.
            var henNapNen = new System.Windows.Forms.Timer
            {
                Interval = 180
            };

            EventHandler xuLyNapNen = null;

            xuLyNapNen = (s, args) =>
            {
                henNapNen.Stop();
                henNapNen.Tick -= xuLyNapNen;
                henNapNen.Dispose();

                if (IsDisposed || Disposing || !IsHandleCreated)
                    return;

                var dongHo =
                    System.Diagnostics.Stopwatch.StartNew();

                int cheDo = -1;

                bdNenDangKhoiDongTuDong = true;

                try
                {
                    if (banDoMapWinGIS == null ||
                        banDoMapWinGIS.IsDisposed ||
                        ketNoiMapWinGIS == null)
                    {
                        throw new InvalidOperationException(
                            "MapWinGIS chưa được khởi tạo.");
                    }

                    TaoBoChonBanDoNen();

                    // AxMap tu nap tile khi Extents thay doi. Theo doi trang thai
                    // de phan biet cache, yeu cau dang tai va loi mang thuc te.
                    if (!bdNenDaGanTheoDoiPhamVi)
                    {
                        banDoMapWinGIS.ExtentsChanged += BdNen_KhiPhamViThayDoi;
                        bdNenDaGanTheoDoiPhamVi = true;
                    }

                    cheDo = BdNen_DocCheDoKhoiDong();

                    GhiNhatKyBanDo(
                        "BASEMAP_STARTUP | selected=" + cheDo +
                        " | tileConfig=" + BdTile_CoCauHinh());

                    ChuyenBanDoNen(cheDo);
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "BASEMAP_INIT_FAILED | " + ex);

                    statusGeneral.Text =
                        "● Không nạp được nền: " + ex.Message;
                }
                finally
                {
                    bdNenDangKhoiDongTuDong = false;

                    dongHo.Stop();

                    string thongTin =
                        "[STARTUP] BASEMAP_LOAD | mode=" + cheDo +
                        " | ms=" + dongHo.ElapsedMilliseconds;

                    System.Diagnostics.Debug.WriteLine(thongTin);
                    GhiNhatKyBanDo(thongTin);
                }
            };

            henNapNen.Tick += xuLyNapNen;
            henNapNen.Start();
        }


        // =====================================================
        // 2. CAU HINH CHE DO NEN
        // =====================================================

        private int BdNen_DocCheDoKhoiDong()
        {
            try
            {
                if (File.Exists(DuongDanCheDoNen))
                {
                    string noiDung =
                        File.ReadAllText(DuongDanCheDoNen).Trim();

                    int cheDo;

                    if (int.TryParse(noiDung, out cheDo) &&
                        cheDo >= 0 &&
                        cheDo <= 3)
                    {
                        if (cheDo != CheDoNenTileNoiBo ||
                            BdTile_CoCauHinh())
                        {
                            return cheDo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BASEMAP_READ_MODE_FAILED | " + ex.Message);
            }

            return BdTile_CoCauHinh()
                ? CheDoNenTileNoiBo
                : 0;
        }

        private void BdNen_LuuCheDo(int cheDo)
        {
            if (bdNenDangKhoiDongTuDong)
                return;

            try
            {
                Directory.CreateDirectory(ThuMucCauHinhNen);

                File.WriteAllText(
                    DuongDanCheDoNen,
                    cheDo.ToString(CultureInfo.InvariantCulture),
                    Encoding.UTF8);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "BASEMAP_SAVE_MODE_FAILED | " + ex.Message);
            }
        }

        private string TimTepAnhNen()
        {
            try
            {
                if (File.Exists(DuongDanCauHinhBanDoNen))
                {
                    string daChon =
                        File.ReadAllText(
                            DuongDanCauHinhBanDoNen).Trim();

                    if (File.Exists(daChon))
                        return daChon;
                }
            }
            catch (Exception)
            {
            }

            string tep = Path.Combine(
                ThuMucNenMacDinh,
                TepNenHaiMet);

            if (File.Exists(tep))
                return tep;

            tep = Path.Combine(
                ThuMucNenMacDinh,
                TepNenNhe);

            return File.Exists(tep) ? tep : null;
        }

        // =====================================================
        // 3. GIAO DIEN CHON NEN
        // =====================================================

        private void TaoBoChonBanDoNen()
        {
            if (cboChonBanDoNen != null &&
                !cboChonBanDoNen.IsDisposed)

            {
                // Khong bo qua gan su kien
                // neu ComboBox da ton tai.
                BdNen_GanSuKienChoCombo();
                return;
            }

            Panel khung = new Panel
            {
                Dock = DockStyle.Top,
                Height = 188,
                BackColor = Color.FromArgb(11, 20, 34),
                Padding = new Padding(12, 8, 12, 8)
            };

            Label nhan = new Label
            {
                Text = "CHỌN BẢN ĐỒ NỀN",
                Left = 12,
                Top = 8,
                Height = 18,
                Width = 240,
                ForeColor = Color.FromArgb(52, 211, 153),
                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold)
            };

            cboChonBanDoNen = new ComboBox
            {
                Left = 12,
                Top = 30,
                Height = 28,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 27, 43),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            cboChonBanDoNen.Items.AddRange(
                new object[]
                {
                    NenVeTinh,
                    NenOSM,
                    NenTrong,
                    NenTileNoiBo
                });

            btnChonTepBanDoNen = new Button
            {
                Text = "+ ĐỔI ẢNH VỆ TINH (.TIF)",
                Left = 12,
                Top = 68,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnChonTepBanDoNen
                .FlatAppearance.BorderSize = 0;

            btnChanDoanRaster = new Button
            {
                Text = "KIỂM TRA ẢNH NỀN (CHẨN ĐOÁN)",
                Left = 12,
                Top = 108,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 27, 43),
                ForeColor = Color.FromArgb(56, 189, 248),
                Font = new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnChanDoanRaster
                .FlatAppearance.BorderSize = 0;

            btnTaiLaiTileNen = new Button
            {
                Name = "btnTaiLaiTileNen",
                Text = "↻ TẢI LẠI TILE BẢN ĐỒ",
                Left = 12,
                Top = 145,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 115, 112),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnTaiLaiTileNen.FlatAppearance.BorderSize = 0;

            khung.Controls.Add(nhan);
            khung.Controls.Add(cboChonBanDoNen);
            khung.Controls.Add(btnChonTepBanDoNen);
            khung.Controls.Add(btnChanDoanRaster);
            khung.Controls.Add(btnTaiLaiTileNen);

            Action canChinh = () =>
            {
                int rong =
                    Math.Max(140, khung.ClientSize.Width - 24);

                cboChonBanDoNen.Width = rong;
                btnChonTepBanDoNen.Width = rong;
                btnChanDoanRaster.Width = rong;
                btnTaiLaiTileNen.Width = rong;
            };

            khung.Resize += (s, args) => canChinh();

            pnlRightBody.Controls.Add(khung);
            khung.BringToFront();

            canChinh();

            lblNguonOSM = new Label
            {
                AutoSize = true,
                Text = "© OpenStreetMap contributors",
                BackColor = Color.FromArgb(9, 17, 28),
                ForeColor = Color.FromArgb(148, 163, 184),
                Padding = new Padding(4),
                Font = new Font("Segoe UI", 8F),
                Visible = false
            };

            pnlMapCanvas.Controls.Add(lblNguonOSM);

            pnlMapCanvas.Resize +=
                (s, args) => DatViTriGhiNguonOSM();

            DatViTriGhiNguonOSM();

            lblNguonOSM.BringToFront();
            pnlMapTools.BringToFront();

            // Gan su kien bang handler co ten
            // de tranh trung hoac roi su kien.
            BdNen_GanSuKienChoCombo();

            btnChonTepBanDoNen.Click +=
                (s, args) => ChonTepAnhNenMoi();

            btnChanDoanRaster.Click +=
                (s, args) => ChanDoanAnhNen();

            btnTaiLaiTileNen.Click +=
                (s, args) => BdNen_TaiLaiTileDangXem();

            ContextMenuStrip menuNen =
                new ContextMenuStrip();

            ToolStripMenuItem itemChanDoan =
                new ToolStripMenuItem(
                    "Chẩn đoán nguồn bản đồ nền");

            itemChanDoan.Click +=
                (s, args) => ChanDoanAnhNen();

            menuNen.Items.Add(itemChanDoan);
            ToolStripMenuItem itemTaiLaiTile =
                new ToolStripMenuItem("Tải lại tile bản đồ đang xem");
            itemTaiLaiTile.Click += (s, args) => BdNen_TaiLaiTileDangXem();
            menuNen.Items.Add(itemTaiLaiTile);

            cboChonBanDoNen.ContextMenuStrip = menuNen;

            btnChonTepBanDoNen.MouseUp += (s, args) =>
            {
                if (args.Button == MouseButtons.Right)
                    ChanDoanAnhNen();
            };

            BdNen_DatLuaChonNen(0);
        }

        // =====================================================
        // 3A. DONG BO COMBOBOX VA TRANG THAI TILE MAPWINGIS
        // =====================================================

        private void BdNen_HenKiemTraOSM_Tick(object sender, EventArgs e)
        {
            if (bdNenHenKiemTraOSM != null)
                bdNenHenKiemTraOSM.Stop();
            BdNen_KiemTraSauKhiBatOSM();
        }

        // KHONG redraw lien tuc khi keo/zoom: MapWinGIS tu nap tile.
        // Hen chan doan mot lan sau khi pham vi da on dinh.
        private void BdNen_KhiPhamViThayDoi(object sender, EventArgs e)
        {
            if (IsDisposed || Disposing || cheDoBanDoNenDangDung != 1 ||
                bdNenDangDungOSMTrucTiep || dangChuyenBanDoNen ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                mayChuOSM == null || bdNenHenKiemTraOSM == null)
                return;

            // Ghi lai moc DEM tile cua pham vi moi, khong lay thong ke cu
            // tu pham vi truoc de ket luan proxy da tai thanh cong.
            bdNenSoYeuCauOSMLucBat = mayChuOSM.SoYeuCau;
            bdNenSoTileOSMThanhCongLucBat = mayChuOSM.SoThanhCong;
            bdNenSoTileOSMThatBaiLucBat = mayChuOSM.SoThatBai;

            bdNenHenKiemTraOSM.Stop();
            bdNenHenKiemTraOSM.Interval = 9000;
            bdNenHenKiemTraOSM.Start();
        }

        // Chi goi theo thao tac cua nguoi dung. Khong nap hang loat tile.
        private void BdNen_TaiLaiTileDangXem()
        {
            if (IsDisposed || Disposing || dangChuyenBanDoNen ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return;

            if (cheDoBanDoNenDangDung != 1 &&
                cheDoBanDoNenDangDung != CheDoNenTileNoiBo)
            {
                statusGeneral.Text = "● Chế độ nền hiện tại không dùng tile trực tuyến.";
                return;
            }

            try
            {
                Tiles tiles = banDoMapWinGIS.Tiles;
                if (tiles.ProjectionStatus ==
                    tkTilesProjectionStatus.tpsEmptyOrInvalid)
                {
                    statusGeneral.Text =
                        "● Không thể tải tile do phép chiếu bản đồ chưa hợp lệ.";
                    return;
                }

                // Neu truoc do da chuyen sang HTTPS truc tiep, uu tien
                // quay ve proxy ung dung co User-Agent rieng va cache 7 ngay.
                if (cheDoBanDoNenDangDung == 1 &&
                    bdNenDangDungOSMTrucTiep)
                {
                    if (tiles.Visible)
                        tiles.Visible = false;

                    BatOpenStreetMap();
                    bdNenDangDungOSMTrucTiep = false;
                    BdNen_CapNhatNguonHienThi(1);
                }

                tiles.UseServer = true;
                tiles.Visible = true;

                if (mayChuOSM != null && cheDoBanDoNenDangDung == 1)
                {
                    bdNenSoYeuCauOSMLucBat = mayChuOSM.SoYeuCau;
                    bdNenSoTileOSMThanhCongLucBat = mayChuOSM.SoThanhCong;
                    bdNenSoTileOSMThatBaiLucBat = mayChuOSM.SoThatBai;
                }

                banDoMapWinGIS.Redraw3(tkRedrawType.RedrawAll, true);
                GhiNhatKyBanDo("BASEMAP_MANUAL_RELOAD | mode=" +
                    cheDoBanDoNenDangDung + " | provider=" + tiles.ProviderId +
                    " | zoom=" + tiles.CurrentZoom +
                    " | projection=" + tiles.ProjectionStatus);

                if (cheDoBanDoNenDangDung == 1 &&
                    bdNenHenKiemTraOSM != null)
                {
                    bdNenHenKiemTraOSM.Stop();
                    bdNenHenKiemTraOSM.Interval = 9000;
                    bdNenHenKiemTraOSM.Start();
                }
                statusGeneral.Text = "● Đã yêu cầu tải lại các tile đang xem.";
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("BASEMAP_MANUAL_RELOAD_FAILED | " + ex);
                statusGeneral.Text = "● Lỗi tải lại nền: " + ex.Message;
            }
        }

        private void BdNen_GanSuKienChoCombo()
        {
            ComboBox combo = cboChonBanDoNen;
            if (combo == null || combo.IsDisposed)
                return;

            if (ReferenceEquals(bdNenComboDaGanSuKien, combo))
                return;

            if (bdNenComboDaGanSuKien != null &&
                !bdNenComboDaGanSuKien.IsDisposed)
            {
                bdNenComboDaGanSuKien.SelectedIndexChanged -=
                    BdNen_CboChon_SelectedIndexChanged;
                bdNenComboDaGanSuKien.SelectionChangeCommitted -=
                    BdNen_CboChon_SelectionChangeCommitted;
                bdNenComboDaGanSuKien.DropDownClosed -=
                    BdNen_CboChon_DropDownClosed;
            }

            bdNenComboDaGanSuKien = combo;
            combo.SelectedIndexChanged += BdNen_CboChon_SelectedIndexChanged;
            combo.SelectionChangeCommitted +=
                BdNen_CboChon_SelectionChangeCommitted;
            combo.DropDownClosed += BdNen_CboChon_DropDownClosed;

            GhiNhatKyBanDo("BASEMAP_COMBO_EVENTS_ATTACHED");
        }

        private void BdNen_CboChon_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            BdNen_XuLySuKienChonNen(sender);
        }

        private void BdNen_CboChon_SelectionChangeCommitted(
            object sender, EventArgs e)
        {
            BdNen_XuLySuKienChonNen(sender);
        }

        private void BdNen_CboChon_DropDownClosed(
            object sender, EventArgs e)
        {
            BdNen_XuLySuKienChonNen(sender);
        }

        private void BdNen_XuLySuKienChonNen(object sender)
        {
            if (!ReferenceEquals(sender, cboChonBanDoNen) ||
                cboChonBanDoNen == null ||
                cboChonBanDoNen.IsDisposed ||
                cboChonBanDoNen.SelectedIndex < 0 ||
                dangChuyenBanDoNen ||
                bdNenDangKhoiDongTuDong ||
                bdNenDangXuLyChonTuNguoiDung)
                return;

            int yeuCau = cboChonBanDoNen.SelectedIndex;
            if (BdNen_CheDoDangKhopThucTe(yeuCau))
                return;

            bdNenSoLanKhoiPhucLienTiep = 0;
            GhiNhatKyBanDo(
                "BASEMAP_COMBO_CHANGED | requested=" + yeuCau +
                " | applied=" + cheDoBanDoNenDangDung);
            BdNen_YeuCauChuyenTuNguoiDung();
        }

        private bool BdNen_CheDoDangKhopThucTe(int cheDo)
        {
            if (cheDo != cheDoBanDoNenDangDung ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return false;

            try
            {
                Tiles tile = banDoMapWinGIS.Tiles;
                if (tile == null) return false;

                if (cheDo == 1)
                    return tile.Visible && tile.UseServer &&
                        (tile.ProviderId == MaNhaCungCapOSMNoiBo ||
                         tile.ProviderId == MaNhaCungCapOSMTrucTiep);

                if (cheDo == CheDoNenTileNoiBo)
                    return tile.Visible && tile.UseServer &&
                        tile.ProviderId == BdTile_MaNhaCungCap;

                if (cheDo == 2)
                    return !tile.Visible;

                if (cheDo == 0)
                    return !tile.Visible && handleAnhNenTuDong >= 0 &&
                        ketNoiMapWinGIS != null &&
                        ketNoiMapWinGIS.CoLop(handleAnhNenTuDong) &&
                        ketNoiMapWinGIS.LayHienThiLop(handleAnhNenTuDong);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("BASEMAP_CHECK_STATE_FAILED | " + ex.Message);
            }

            return false;
        }

        // Khi module khac bo qua/doi ComboBox, timer se gan lai event.
        // Neu OSM duoc chon nhung chua bat, thuc hien chuyen that su.
        // Gioi han so lan thu de tranh lap vo han neu co file khac tat Tiles.

        private void BdNen_HenDongBoChon_Tick(object sender, EventArgs e)
        {
            if (!daKhoiTaoBanDoNen || IsDisposed || Disposing ||
                bdNenDangKhoiDongTuDong || dangChuyenBanDoNen ||
                bdNenDangXuLyChonTuNguoiDung ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed)
                return;

            BdNen_GanSuKienChoCombo();
            if (cboChonBanDoNen == null || cboChonBanDoNen.IsDisposed ||
                cboChonBanDoNen.DroppedDown)
                return;

            int yeuCau = cboChonBanDoNen.SelectedIndex;
            if (yeuCau < 0 || yeuCau > 3)
                return;

            if (BdNen_CheDoDangKhopThucTe(yeuCau))
            {
                bdNenSoLanKhoiPhucLienTiep = 0;
                return;
            }

            if (DateTime.UtcNow - bdNenThoiDiemTuSuaCuoi <
                TimeSpan.FromSeconds(3))
                return;

            if (bdNenSoLanKhoiPhucLienTiep >= 3)
            {
                // Khong lap lien tuc neu phan he khac lien tuc tat Tiles.
                return;
            }

            bdNenThoiDiemTuSuaCuoi = DateTime.UtcNow;
            bdNenSoLanKhoiPhucLienTiep++;
            GhiNhatKyBanDo(
                "BASEMAP_AUTO_RECONCILE | request=" + yeuCau +
                " | applied=" + cheDoBanDoNenDangDung +
                " | attempt=" + bdNenSoLanKhoiPhucLienTiep);
            BdNen_YeuCauChuyenTuNguoiDung();
        }

        private void BdNen_YeuCauChuyenTuNguoiDung()
        {
            if (bdNenDangXuLyChonTuNguoiDung ||
                dangChuyenBanDoNen ||
                bdNenDangKhoiDongTuDong ||
                cboChonBanDoNen == null ||
                cboChonBanDoNen.IsDisposed ||
                cboChonBanDoNen.SelectedIndex < 0)
                return;

            int yeuCau = cboChonBanDoNen.SelectedIndex;
            if (BdNen_CheDoDangKhopThucTe(yeuCau))
                return;

            bdNenDangXuLyChonTuNguoiDung = true;

            try
            {
                GhiNhatKyBanDo(
                    "BASEMAP_SELECTION_REQUEST | requested=" + yeuCau +
                    " | previous=" + cheDoBanDoNenDangDung);

                // Khong chi tin vao gia tri combo: buoc chuyen thuc su.
                ChuyenBanDoNen(yeuCau);

                if (cheDoBanDoNenDangDung != yeuCau)
                {
                    GhiNhatKyBanDo(
                        "BASEMAP_SELECTION_NOT_APPLIED | requested=" + yeuCau +
                        " | actual=" + cheDoBanDoNenDangDung);
                    BdNen_KhoiPhucLuaChonThucTe();
                    return;
                }

                if (yeuCau == 1 || yeuCau == CheDoNenTileNoiBo)
                {
                    Tiles tiles = banDoMapWinGIS.Tiles;
                    int mongDoi = yeuCau == 1
                        ? MaNhaCungCapOSMNoiBo
                        : BdTile_MaNhaCungCap;
                    BdNen_KiemTraTileDaChuyen(
                        mongDoi,
                        yeuCau == 1 ? "OpenStreetMap" : "GeoServer WMTS");
                    GhiNhatKyBanDo(
                        "BASEMAP_TILE_STATE | provider=" + tiles.ProviderId +
                        " | visible=" + tiles.Visible +
                        " | useServer=" + tiles.UseServer +
                        " | zoom=" + tiles.CurrentZoom +
                        " | projection=" + tiles.ProjectionStatus);
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("BASEMAP_SELECTION_FAILED | " + ex);
                BdNen_KhoiPhucLuaChonThucTe();
                if (statusGeneral != null)
                    statusGeneral.Text = "● Lỗi chuyển nền: " + ex.Message;
            }
            finally
            {
                bdNenDangXuLyChonTuNguoiDung = false;
            }
        }

        private void BdNen_KhoiPhucLuaChonThucTe()
        {
            if (cheDoBanDoNenDangDung >= 0)
                BdNen_DatLuaChonNen(cheDoBanDoNenDangDung);
            else if (cboChonBanDoNen != null &&
                     !cboChonBanDoNen.IsDisposed)
            {
                bool khoaCu = dangChuyenBanDoNen;
                dangChuyenBanDoNen = true;
                try { cboChonBanDoNen.SelectedIndex = -1; }
                finally { dangChuyenBanDoNen = khoaCu; }
            }
        }

        // =====================================================
        // TU CHUYEN SANG OSM HTTPS NEU PROXY KHONG CO TILE
        // =====================================================
        private void BdNen_KiemTraSauKhiBatOSM()
        {
            if (cheDoBanDoNenDangDung != 1 ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                mayChuOSM == null)
                return;

            try
            {
                Tiles t = banDoMapWinGIS.Tiles;
                int yeuCauMoi = mayChuOSM.SoYeuCau -
                    bdNenSoYeuCauOSMLucBat;
                int thanhCongMoi = mayChuOSM.SoThanhCong -
                    bdNenSoTileOSMThanhCongLucBat;
                int thatBaiMoi = mayChuOSM.SoThatBai -
                    bdNenSoTileOSMThatBaiLucBat;
                double ram = 0d;
                double disk = 0d;
                try
                {
                    ram = t.get_CacheSize2(
                        tkCacheType.RAM, t.ProviderId, -1);
                    disk = t.get_CacheSize2(
                        tkCacheType.Disk, t.ProviderId, -1);
                }
                catch (Exception cacheError)
                {
                    GhiNhatKyBanDo(
                        "OSM_CACHE_DIAG_FAILED | " + cacheError.Message);
                }

                GhiNhatKyBanDo(
                    "OSM_DIAG | provider=" + t.ProviderId +
                    " | visible=" + t.Visible +
                    " | server=" + t.UseServer +
                    " | zoom=" + t.CurrentZoom +
                    " | projection=" + t.ProjectionStatus +

                    " | requests=" + yeuCauMoi +
                    " | proxyOK=" + thanhCongMoi +
                    " | proxyFAIL=" + thatBaiMoi +
                    " | ramMB=" + ram.ToString("F3", CultureInfo.InvariantCulture) +
                    " | diskMB=" + disk.ToString("F3", CultureInfo.InvariantCulture) +
                    " | proxyError=" + mayChuOSM.LoiGanNhat);

                if (t.ProjectionStatus ==
                    tkTilesProjectionStatus.tpsEmptyOrInvalid)
                {
                    statusGeneral.Text =
                        "● OSM: phép chiếu AxMap không tương thích, " +
                        "xem chẩn đoán nền.";
                    return;
                }

                // Da chuyen sang OSM HTTPS truc tiep, khong chuyen nguoc.
                if (bdNenDangDungOSMTrucTiep)
                {
                    if (t.ProviderId != MaNhaCungCapOSMTrucTiep ||
                        !t.Visible || !t.UseServer)
                    {
                        statusGeneral.Text =
                            "● OSM: provider trực tiếp bị thay đổi từ nơi khác.";
                    }
                    else if (ram <= bdNenDungLuongOSMRamBanDau &&
                             disk <= bdNenDungLuongOSMDiskBanDau)
                    {
                        statusGeneral.Text =
                            "● OSM HTTPS đã bật nhưng chưa có tile mới. " +
                            "Kiểm tra Tiles.Proxy, phép chiếu và nhật ký tile.";
                    }
                    else
                    {
                        statusGeneral.Text =
                            "● OpenStreetMap HTTPS: đã nhận dữ liệu tile.";
                    }
                    return;
                }

                if (t.ProviderId != MaNhaCungCapOSMNoiBo ||
                    !t.Visible || !t.UseServer)
                {
                    statusGeneral.Text =
                        "● OSM proxy bị vô hiệu hóa; thử nguồn HTTPS trực tiếp.";
                }
                else if (thanhCongMoi > 0)
                {
                    // Da nhan tile: giu nguyen provider va bo nho dem.
                    // Redraw khong ep tai lai tile da nhan.
                    banDoMapWinGIS.Redraw3(
                        tkRedrawType.RedrawAll, false);
                    statusGeneral.Text =
                        "● OSM proxy đã trả " + thanhCongMoi +
                        " tile; tiếp tục sử dụng nền hiện tại.";
                    return;
                }

                // KHONG coi '0 yeu cau' la loi proxy: ban do co the
                // da dung tile cache, hoac MapWinGIS chua phat sinh request.
                // Tu dong doi sang provider HTTPS trong truong hop nay lam
                // mat nen da co va co the gap loi User-Agent tren may chu OSM.
                if (yeuCauMoi <= 0)
                {
                    GhiNhatKyBanDo("OSM_NO_NEW_REQUEST | keep proxy; " +
                        "check tile zoom/projection or cached tiles");
                    if (phanHeHienTai == "Bản đồ GIS")
                        statusGeneral.Text =
                            "● OSM chưa có yêu cầu tile mới; có thể đang dùng cache.";
                    return;
                }

                // Co request nhung chua thanh cong / that bai: van dang tai.
                // Khong doi provider giua chung de tranh roi tile.
                if (thatBaiMoi <= 0)
                {
                    GhiNhatKyBanDo("OSM_REQUESTS_PENDING | " + yeuCauMoi);
                    return;
                }

                // Chi thu HTTPS truc tiep khi proxy thuc su tra loi that bai.
                // Neu mang chan dich vu OSM, ca hai nguon co the deu trang.
                BdNen_BatOSMTrucTiep();
                BdNen_KiemTraTileDaChuyen(
                    MaNhaCungCapOSMTrucTiep,
                    "OpenStreetMap HTTPS");
                BdNen_CapNhatNguonHienThi(1);

                try
                {
                    bdNenDungLuongOSMRamBanDau =
                        t.get_CacheSize2(tkCacheType.RAM,
                            MaNhaCungCapOSMTrucTiep, -1);
                    bdNenDungLuongOSMDiskBanDau =
                        t.get_CacheSize2(tkCacheType.Disk,
                            MaNhaCungCapOSMTrucTiep, -1);
                }
                catch (Exception cacheError)
                {
                    GhiNhatKyBanDo(
                        "OSM_DIRECT_CACHE_DIAG_FAILED | " + cacheError.Message);
                }

                if (bdNenHenKiemTraOSM != null)
                    bdNenHenKiemTraOSM.Start();

                statusGeneral.Text =
                    "● OSM proxy không có tile, đã chuyển sang HTTPS trực tiếp.";
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("OSM_FALLBACK_FAILED | " + ex);
                statusGeneral.Text =
                    "● Không khởi chạy được OSM dự phòng: " + ex.Message;
            }
        }

        private void BdNen_BatOSMTrucTiep()
        {
            Tiles t = banDoMapWinGIS.Tiles;
            int index = t.Providers.get_IndexByProviderId(
                MaNhaCungCapOSMTrucTiep);

            if (index >= 0 && !string.Equals(
                t.Providers.get_UrlPattern(index),
                BdNen_OSMUrlTrucTiep,

                StringComparison.OrdinalIgnoreCase))
            {
                // Khong xoa provider dang su dung.
                if (t.Visible) t.Visible = false;
                if (!t.Providers.Remove(
                    MaNhaCungCapOSMTrucTiep, false))
                {
                    throw new InvalidOperationException(
                        "Không thay được URL provider OSM HTTPS.");
                }
                index = -1;
            }

            if (index < 0)
            {
                bool them = t.Providers.Add(
                    MaNhaCungCapOSMTrucTiep,
                    "OpenStreetMap HTTPS trực tiếp",
                    BdNen_OSMUrlTrucTiep,
                    tkTileProjection.SphericalMercator,
                    0, 19);
                if (!them)
                    throw new InvalidOperationException(
                        "Không đăng ký được OSM HTTPS: " +
                        t.Providers.get_ErrorMsg(
                            t.Providers.LastErrorCode));
            }

            BdTile_CauHinhCache(t);
            if (t.Visible) t.Visible = false;
            if (t.ProviderId != MaNhaCungCapOSMTrucTiep)
                t.ProviderId = MaNhaCungCapOSMTrucTiep;
            t.UseServer = true;
            t.Visible = true;

            if (t.ProjectionStatus ==
                tkTilesProjectionStatus.tpsEmptyOrInvalid)
                throw new InvalidOperationException(
                    "Phép chiếu AxMap không tương thích tile OSM.");

            bdNenDangDungOSMTrucTiep = true;
            banDoMapWinGIS.Redraw3(
                tkRedrawType.RedrawAll, true);
            GhiNhatKyBanDo(
                "OSM_DIRECT_ENABLED | provider=" + t.ProviderId +
                " | URL=" + BdNen_OSMUrlTrucTiep);
        }

        private void DatViTriGhiNguonOSM()
        {
            if (lblNguonOSM == null ||
                pnlMapCanvas == null)
                return;

            lblNguonOSM.Left = Math.Max(
                0,
                pnlMapCanvas.ClientSize.Width -
                lblNguonOSM.Width - 8);

            lblNguonOSM.Top = Math.Max(
                0,
                pnlMapCanvas.ClientSize.Height -
                lblNguonOSM.Height - 8);
        }

        private void BdNen_DatLuaChonNen(int cheDo)
        {
            if (cboChonBanDoNen == null ||
                cboChonBanDoNen.IsDisposed ||
                cheDo < 0 ||
                cheDo >= cboChonBanDoNen.Items.Count)
                return;

            bool dangChuyenCu = dangChuyenBanDoNen;
            dangChuyenBanDoNen = true;

            try
            {
                cboChonBanDoNen.SelectedIndex = cheDo;
            }
            finally
            {
                dangChuyenBanDoNen = dangChuyenCu;
            }
        }

        // =====================================================
        // 4. CHUYEN DOI 4 LOAI NEN
        // =====================================================


        private void ChuyenBanDoNen(int cheDo)
        {
            if (dangChuyenBanDoNen)
                return;

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null)
                throw new InvalidOperationException("MapWinGIS chưa sẵn sàng để chuyển nền.");

            if (cheDo < 0 || cheDo > 3)
                throw new ArgumentOutOfRangeException("cheDo");

            Tiles t = banDoMapWinGIS.Tiles;
            if (t == null)
                throw new InvalidOperationException("MapWinGIS Tiles chưa được khởi tạo.");

            int truoc = cheDoBanDoNenDangDung;
            bool oldVisible = t.Visible;
            bool oldServer = t.UseServer;

            int oldProvider = t.ProviderId;

            string loi = null;
            bool ok = false;

            dangChuyenBanDoNen = true;

            if (bdNenHenKiemTraOSM != null)
                bdNenHenKiemTraOSM.Stop();

            try
            {
                if (cheDo == 0)
                {
                    if (handleAnhNenTuDong < 0 ||
                        !ketNoiMapWinGIS.CoLop(
                            handleAnhNenTuDong))
                    {
                        string tep = TimTepAnhNen();

                        if (string.IsNullOrWhiteSpace(tep))
                        {
                            throw new FileNotFoundException(
                                "Chưa tìm thấy GeoTIFF ngoại tuyến. " +
                                "Hãy chọn ảnh nền .tif hợp lệ.");
                        }

                        NapAnhNen(tep);
                    }

                    DatHienThiAnhNen(true);
                    BdNen_AnRasterNenKhac(handleAnhNenTuDong);

                    TatOpenStreetMap();
                }
                else if (cheDo == 1)
                {
                    // Gan CRS neu AxMap dang de trong, giu he UTM 48N.
                    BdNen_DamBaoPhepChieuThucTe();

                    // Chuyen sang provider OSM 11301.
                    if (t.Visible)
                        t.Visible = false;

                    BatOpenStreetMap();

                    bdNenDangDungOSMTrucTiep = false;
                    bdNenSoYeuCauOSMLucBat =
                        mayChuOSM == null ? 0 : mayChuOSM.SoYeuCau;
                    bdNenSoTileOSMThanhCongLucBat =
                        mayChuOSM == null ? 0 : mayChuOSM.SoThanhCong;
                    bdNenSoTileOSMThatBaiLucBat =
                        mayChuOSM == null ? 0 : mayChuOSM.SoThatBai;

                    DatHienThiAnhNen(false);
                    BdNen_AnRasterNenKhac(-1);

                    BdNen_KiemTraTileDaChuyen(
                        11301,
                        "OpenStreetMap");
                }
                else if (cheDo == CheDoNenTileNoiBo)
                {
                    BdNen_DamBaoPhepChieuThucTe();

                    // Chuyen sang provider GeoServer 11302.
                    if (t.Visible)
                        t.Visible = false;

                    BdTile_Bat();

                    DatHienThiAnhNen(false);
                    BdNen_AnRasterNenKhac(-1);

                    BdNen_KiemTraTileDaChuyen(
                        BdTile_MaNhaCungCap,
                        "GeoServer WMTS");
                }
                else
                {
                    TatOpenStreetMap();
                    DatHienThiAnhNen(false);
                    BdNen_AnRasterNenKhac(-1);
                }
                if (cheDo == 1 ||
                    cheDo == CheDoNenTileNoiBo)
                {
                    // Khi khoi dong, Tiles.Visible da kich hoat
                    // viec tai tile. Khong yeu cau nap lai lan hai.
                    banDoMapWinGIS.Redraw3(
                        tkRedrawType.RedrawAll,
                        !bdNenDangKhoiDongTuDong);
                }
                else
                {
                    banDoMapWinGIS.Redraw();
                }

                cheDoBanDoNenDangDung = cheDo;
                bdNenLoiChuyenGanNhat = "";
                bdNenSoLanKhoiPhucLienTiep = 0;

                BdNen_DatLuaChonNen(cheDo);
                BdNen_LuuCheDo(cheDo);
                BdNen_CapNhatNguonHienThi(cheDo);

                CapNhatNhanBanDo();

                if (cheDo == 1 &&
                    bdNenHenKiemTraOSM != null)
                {
                    bdNenHenKiemTraOSM.Interval = 9000;
                    bdNenHenKiemTraOSM.Start();
                }

                GhiNhatKyBanDo(
                    "BASEMAP_SWITCH_OK | mode=" + cheDo +
                    " | provider=" + t.ProviderId +
                    " | visible=" + t.Visible +
                    " | server=" + t.UseServer +
                    " | zoom=" + t.CurrentZoom +
                    " | projection=" + t.ProjectionStatus);

                ok = true;
            }
            catch (Exception ex)
            {
                loi = ex.Message;
                bdNenLoiChuyenGanNhat = ex.ToString();

                GhiNhatKyBanDo(
                    "BASEMAP_SWITCH_FAILED | mode=" +
                    cheDo + " | " + ex);

                try
                {
                    if (t.Visible)
                        t.Visible = false;

                    if (t.ProviderId != oldProvider)
                        t.ProviderId = oldProvider;

                    t.UseServer = oldServer;
                    t.Visible = oldVisible;

                    banDoMapWinGIS.Redraw3(
                        tkRedrawType.RedrawAll,
                        true);
                }
                catch (Exception rollback)
                {
                    GhiNhatKyBanDo(
                        "BASEMAP_ROLLBACK_FAILED | " +
                        rollback);
                }

                if (truoc == 0)
                {
                    try
                    {
                        DatHienThiAnhNen(true);
                    }
                    catch (Exception)
                    {
                    }
                }

                BdNen_KhoiPhucLuaChonThucTe();

                if (truoc >= 0)
                    BdNen_CapNhatNguonHienThi(truoc);

                if (!bdNenDangKhoiDongTuDong)
                {
                    MessageBox.Show(
                        this,
                        "Chuyển bản đồ nền thất bại:\r\n" +
                        ex.Message +
                        "\r\n\r\nNhấp chuột phải vào ô chọn " +
                        "bản đồ nền để xem chẩn đoán.",
                        "Lỗi chuyển bản đồ nền",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                dangChuyenBanDoNen = false;
            }

            if (!ok && loi != null)
            {
                statusGeneral.Text =
                    "● Lỗi chuyển nền: " + loi;
                GhiNhatKyBanDo(
                    "BASEMAP_AFTER_FAILURE | applied=" +
                    cheDoBanDoNenDangDung +
                    " | provider=" + t.ProviderId +
                    " | visible=" + t.Visible +
                    " | useServer=" + t.UseServer);
            }
        }

        // =====================================================
        // 5. AN CAC LOP RASTER NEN CU
        // =====================================================

        private void BdNen_AnRasterNenKhac(int handleGiu)
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            for (int i = 0;
                i < banDoMapWinGIS.NumLayers; i++)
            {
                int h =
                    banDoMapWinGIS.get_LayerHandle(i);

                if (h < 0 || h == handleGiu)

                    continue;

                try
                {
                    MapWinGIS.Image image =
                        banDoMapWinGIS.get_Image(h);

                    if (image == null)
                        continue;

                    string ten =
                        banDoMapWinGIS.get_LayerName(h) ?? "";

                    string tep =
                        banDoMapWinGIS.get_LayerFilename(h) ?? "";

                    bool laRasterNen =
                        h == handleAnhNenTuDong ||
                        ten.IndexOf(
                            "HANOI_SATELLITE",
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        tep.IndexOf(
                            "HANOI_SATELLITE",
                            StringComparison.OrdinalIgnoreCase) >= 0;

                    if (laRasterNen &&
                        banDoMapWinGIS.get_LayerVisible(h))
                    {
                        banDoMapWinGIS.set_LayerVisible(
                            h,
                            false);

                        GhiNhatKyBanDo(
                            "BASEMAP_HIDE_OLD_RASTER | " +
                            h + " | " + ten);
                    }
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "BASEMAP_RASTER_CHECK_FAIL | " +
                        h + " | " + ex.Message);
                }
            }
        }

        // Chi gan CRS khi AxMap hoan toan chua co phep chieu.
        // Khong thay doi CRS da duoc thiet lap, khong chuyen toa do diem.
        private void BdNen_DamBaoPhepChieuThucTe()
        {
            if (banDoMapWinGIS == null ||
               banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            GeoProjection dangCo =
                banDoMapWinGIS.GeoProjection;

            // IsEmpty la thuoc tinh, khong phai phuong thuc.
            if (dangCo != null && !dangCo.IsEmpty)
            {
                return;
            }

            GeoProjection gp = new GeoProjection();

            if (!gp.ImportFromEPSG(32648))
            {
                throw new InvalidOperationException(
                    "Không khởi tạo được hệ tọa độ " +
                    "EPSG:32648 cho MapWinGIS.");
            }

            banDoMapWinGIS.GeoProjection = gp;

            GhiNhatKyBanDo(
                "BASEMAP_MAP_PROJECTION_INITIALIZED | " +
                "EPSG:32648");
        }

        private void BdNen_KiemTraTileDaChuyen(
            int providerMongDoi,
            string tenNguon)
        {
            Tiles tiles = banDoMapWinGIS.Tiles;

            bool providerHopLe =
                tiles.ProviderId == providerMongDoi ||
                (providerMongDoi == MaNhaCungCapOSMNoiBo &&
                 tiles.ProviderId == MaNhaCungCapOSMTrucTiep);

            if (!tiles.Visible ||
                !tiles.UseServer ||
                !providerHopLe)
            {
                throw new InvalidOperationException(
                    tenNguon +
                    " chưa kích hoạt thành công. Provider=" +
                    tiles.ProviderId +
                    ", Visible=" + tiles.Visible +
                    ", UseServer=" + tiles.UseServer);
            }

            if (tiles.ProjectionStatus ==
                tkTilesProjectionStatus.tpsEmptyOrInvalid)
            {
                throw new InvalidOperationException(
                    "Phép chiếu bản đồ không tương thích " +
                    "với tile EPSG:3857. " +
                    "Cần kiểm tra phép chiếu của AxMap.");
            }
        }

        private void BdNen_CapNhatNguonHienThi(int cheDo)
        {
            if (lblNguonOSM != null &&
                !lblNguonOSM.IsDisposed)
            {
                lblNguonOSM.Visible =
                    cheDo == 1 ||
                    cheDo == CheDoNenTileNoiBo;

                if (cheDo == 1)
                {
                    lblNguonOSM.Text =
                        bdNenDangDungOSMTrucTiep
                        ? "© OpenStreetMap contributors (HTTPS)"
                        : "© OpenStreetMap contributors";
                }
                else if (cheDo == CheDoNenTileNoiBo)
                {
                    lblNguonOSM.Text =
                        "Ảnh vệ tinh Hà Nội – GeoServer WMTS";
                }

                if (lblNguonOSM.Visible)
                    lblNguonOSM.BringToFront();
            }

            string ten =
                cheDo == 0 ? "GeoTIFF ngoại tuyến" :
                cheDo == 1 ? "OpenStreetMap" :
                cheDo == CheDoNenTileNoiBo
                    ? "GeoServer WMTS"
                    : "Không có nền";

            statusGeneral.Text =
                "● Bản đồ nền: " + ten;

            if (btnTaiLaiTileNen != null && !btnTaiLaiTileNen.IsDisposed)
                btnTaiLaiTileNen.Enabled =
                    cheDo == 1 || cheDo == CheDoNenTileNoiBo;

            if (pnlMapTools != null &&
                !pnlMapTools.IsDisposed)
            {
                pnlMapTools.BringToFront();
            }
        }

        private void DatHienThiAnhNen(bool hienThi)
        {
            if (ketNoiMapWinGIS == null ||
                handleAnhNenTuDong < 0 ||
                !ketNoiMapWinGIS.CoLop(
                    handleAnhNenTuDong))
                return;

            if (ketNoiMapWinGIS.LayHienThiLop(
                handleAnhNenTuDong) != hienThi)
            {
                ketNoiMapWinGIS.DatHienThiLop(
                    handleAnhNenTuDong,
                    hienThi);
            }
            if (danhSachLop != null)
            {
                for (int i = 0;

                    i < danhSachLop.Items.Count; i++)
                {
                    LopMapItem muc =
                        danhSachLop.Items[i] as LopMapItem;

                    if (muc == null ||
                        muc.Handle != handleAnhNenTuDong)
                        continue;

                    if (danhSachLop.GetItemChecked(i) != hienThi)
                    {
                        bool cu = dangCapNhatLop;
                        dangCapNhatLop = true;

                        try
                        {
                            danhSachLop.SetItemChecked(
                                i,
                                hienThi);
                        }
                        finally
                        {
                            dangCapNhatLop = cu;
                        }
                    }

                    break;
                }
            }

            BdNen_GhiTrangThaiNen();
        }

        // =====================================================
        // 6. NAP ANH GEOTIFF NGOAI TUYEN
        // =====================================================

        private void NapAnhNen(string duongDan)
        {
            if (dangNapAnhNen)
                return;

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                ketNoiMapWinGIS == null)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa sẵn sàng.");
            }

            if (string.IsNullOrWhiteSpace(duongDan))
            {
                throw new ArgumentException(
                    "Chưa có đường dẫn GeoTIFF.",
                    "duongDan");
            }

            string tep =
                Path.GetFullPath(duongDan.Trim());

            if (!File.Exists(tep))
            {
                throw new FileNotFoundException(
                    "Không tìm thấy ảnh nền.",
                    tep);

            }

            if (handleAnhNenTuDong >= 0 &&
                ketNoiMapWinGIS.CoLop(
                    handleAnhNenTuDong))
            {
                string cu = duongDanAnhNenDangNap;

                if (string.IsNullOrWhiteSpace(cu))
                {
                    MapWinGIS.Image anhCu =
                        banDoMapWinGIS.get_Image(
                            handleAnhNenTuDong);

                    if (anhCu != null)
                        cu = anhCu.Filename;
                }

                if (!string.IsNullOrWhiteSpace(cu) &&
                    string.Equals(
                        Path.GetFullPath(cu),
                        tep,
                        StringComparison.OrdinalIgnoreCase))
                {
                    DatHienThiAnhNen(true);
                    return;
                }
            }

            dangNapAnhNen = true;

            int handleCu = handleAnhNenTuDong;
            int handleMoi = -1;
            bool tuKhoa = false;

            bool laLanDauNgoaiTuyen =
                cheDoBanDoNenDangDung < 0 &&
                handleCu < 0 &&
                banDoMapWinGIS.NumLayers == 0;

            try
            {
                if (banDoMapWinGIS.IsLocked ==
                    tkLockMode.lmUnlock)
                {
                    banDoMapWinGIS.LockWindow(
                        tkLockMode.lmLock);

                    tuKhoa = true;
                }

                handleMoi =
                    ketNoiMapWinGIS.ThemLopTuTep(
                        tep,
                        true);

                if (handleMoi < 0)
                {
                    throw new InvalidOperationException(
                        "Không nạp được GeoTIFF.");
                }

                LopMapItem muc = new LopMapItem
                {
                    Handle = handleMoi,
                    Ten = Path.GetFileNameWithoutExtension(tep)
                };

                bool cu = dangCapNhatLop;
                dangCapNhatLop = true;

                try
                {
                    if (handleCu >= 0 &&
                        handleCu != handleMoi &&
                        ketNoiMapWinGIS.CoLop(handleCu))
                    {
                        ketNoiMapWinGIS.XoaLop(handleCu);

                        for (int i =
                            danhSachLop.Items.Count - 1;
                            i >= 0; i--)
                        {
                            LopMapItem x =
                                danhSachLop.Items[i]
                                    as LopMapItem;

                            if (x != null &&
                                x.Handle == handleCu)
                            {
                                danhSachLop.Items.RemoveAt(i);
                            }
                        }

                        cacLop.RemoveAll(

                            x => x != null &&
                                 x.Handle == handleCu);
                    }

                    cacLop.Add(muc);
                    danhSachLop.Items.Add(muc, true);

                    handleAnhNenTuDong = handleMoi;
                    duongDanAnhNenDangNap = tep;
                }
                finally
                {
                    dangCapNhatLop = cu;
                }

                if (laLanDauNgoaiTuyen)
                    ketNoiMapWinGIS.PhongDenLop(handleMoi);

                DuaLopDiemLenTren();
                CapNhatNhanBanDo();
            }
            finally
            {
                if (tuKhoa &&
                    banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.LockWindow(
                        tkLockMode.lmUnlock);
                }

                dangNapAnhNen = false;
            }
        }

        private void BdNen_GhiCacLopRaster()
        {
        }

        private void BdNen_GhiTrangThaiNen()
        {
        }

        // =====================================================
        // 7. CHAN DOAN BAN DO NEN
        // =====================================================

        private void ChanDoanAnhNen()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                MessageBox.Show(
                    this,
                    "MapWinGIS chưa sẵn sàng.",
                    "Chẩn đoán nền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            try
            {
                Tiles t = banDoMapWinGIS.Tiles;

                string tenCombo =
                    cboChonBanDoNen == null
                    ? "(chưa tạo)"
                    : cboChonBanDoNen.Text;

                StringBuilder sb =
                    new StringBuilder();

                sb.AppendLine(
                    "CHẨN ĐOÁN CHUYỂN BẢN ĐỒ NỀN");

                sb.AppendLine(
                    "--------------------------------------------");

                sb.AppendLine(
                    "ComboBox: " + tenCombo);

                sb.AppendLine(
                    "Chế độ đã áp dụng: " +
                    cheDoBanDoNenDangDung);
                sb.AppendLine("Sự kiện ComboBox đã gắn: " +
                    (cboChonBanDoNen != null &&
                     ReferenceEquals(bdNenComboDaGanSuKien,
                         cboChonBanDoNen)));
                sb.AppendLine("ComboBox khớp chế độ/tile: " +
                    (cboChonBanDoNen != null &&
                     BdNen_CheDoDangKhopThucTe(
                         cboChonBanDoNen.SelectedIndex)));
                sb.AppendLine("Lỗi chuyển nền gần nhất: " +
                    (string.IsNullOrEmpty(bdNenLoiChuyenGanNhat)
                        ? "(không có)" : bdNenLoiChuyenGanNhat));

                sb.AppendLine(
                    "Tile ProviderId: " + t.ProviderId);

                sb.AppendLine(
                    "Tile ProviderName: " + t.ProviderName);
                sb.AppendLine(
                    "OSM source: " + (bdNenDangDungOSMTrucTiep
                        ? "HTTPS trực tiếp (11303)"
                        : "Proxy nội bộ (11301)"));
                sb.AppendLine("MapWinGIS tile proxy: " + t.Proxy);

                sb.AppendLine(
                    "Tile Visible: " + t.Visible);

                sb.AppendLine(
                    "Tile UseServer: " + t.UseServer);

                sb.AppendLine(
                    "Tile CurrentZoom: " + t.CurrentZoom);

                sb.AppendLine(
                    "Tile ZoomRange: " +
                    t.MinZoom + "-" + t.MaxZoom);

                sb.AppendLine(
                    "Tile ProjectionStatus: " +
                    t.ProjectionStatus);

                sb.AppendLine(
                    "Map Projection: " +
                    banDoMapWinGIS.Projection);

                sb.AppendLine(
                    "Map Layers: " +
                    banDoMapWinGIS.NumLayers);

                sb.AppendLine(
                    "Tile Cache: " +
                    t.DiskCacheFilename);

                if (mayChuOSM != null)
                {
                    sb.AppendLine(
                        "OSM local URL: " +
                        mayChuOSM.DuongDanTiles);

                    sb.AppendLine(
                        "OSM proxy requests: " +
                        mayChuOSM.SoYeuCau);


                    sb.AppendLine(
                        "OSM proxy OK: " +
                        mayChuOSM.SoThanhCong);

                    sb.AppendLine(
                        "OSM proxy FAILED: " +
                        mayChuOSM.SoThatBai);

                    sb.AppendLine(
                        "OSM last error: " +
                        mayChuOSM.LoiGanNhat);
                    sb.AppendLine("OSM new requests in current view: " +
                        (mayChuOSM.SoYeuCau - bdNenSoYeuCauOSMLucBat));
                    sb.AppendLine("OSM new successes in current view: " +
                        (mayChuOSM.SoThanhCong - bdNenSoTileOSMThanhCongLucBat));
                    sb.AppendLine("OSM new failures in current view: " +
                        (mayChuOSM.SoThatBai - bdNenSoTileOSMThatBaiLucBat));
                }

                sb.AppendLine();
                sb.AppendLine("CÁC LỚP RASTER ĐANG CÓ:");

                int soRaster = 0;

                for (int i = 0;
                    i < banDoMapWinGIS.NumLayers; i++)
                {
                    int handle =
                        banDoMapWinGIS.get_LayerHandle(i);

                    if (handle < 0)
                        continue;

                    MapWinGIS.Image image =
                        banDoMapWinGIS.get_Image(handle);

                    if (image == null)
                        continue;

                    soRaster++;

                    sb.AppendLine(
                        "[" + handle + "] " +
                        banDoMapWinGIS.get_LayerName(handle) +
                        " | Visible=" +
                        banDoMapWinGIS.get_LayerVisible(handle));
                }

                if (soRaster == 0)
                {
                    sb.AppendLine(
                        "(không có Image layer)");
                }

                string chiTiet = sb.ToString();

                GhiNhatKyBanDo(
                    "BASEMAP_DIAG | " +
                    chiTiet.Replace(
                        Environment.NewLine,
                        " | "));

                MessageBox.Show(
                    this,
                    chiTiet,
                    "Kiểm tra bản đồ nền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    ex.ToString(),
                    "Lỗi chẩn đoán nền",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // =====================================================
        // 8. CHON FILE GEOTIFF
        // =====================================================

        private void ChonTepAnhNenMoi()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title =

                    "Chọn ảnh nền GeoTIFF cho Hà Nội";

                dlg.Filter =
                    "GeoTIFF (*.tif;*.tiff)|*.tif;*.tiff";

                dlg.InitialDirectory =
                    Directory.Exists(ThuMucNenMacDinh)
                    ? ThuMucNenMacDinh
                    : Environment.GetFolderPath(
                        Environment.SpecialFolder.MyDocuments);

                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return;

                FileInfo tep =
                    new FileInfo(dlg.FileName);

                try
                {
                    NapAnhNen(tep.FullName);
                    TatOpenStreetMap();

                    if (lblNguonOSM != null)
                        lblNguonOSM.Visible = false;

                    banDoMapWinGIS.Redraw();

                    Directory.CreateDirectory(
                        ThuMucCauHinhNen);

                    File.WriteAllText(
                        DuongDanCauHinhBanDoNen,
                        tep.FullName);

                    cheDoBanDoNenDangDung = 0;

                    BdNen_DatLuaChonNen(0);
                    BdNen_LuuCheDo(0);

                    statusGeneral.Text =
                        "● Đã chọn ảnh nền: " + tep.Name;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        this,
                        ex.Message,
                        "Không nạp được ảnh nền",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =====================================================
        // 9. BAT / TAT OPENSTREETMAP
        // =====================================================

        private void BatOpenStreetMap()
        {
            if (mayChuOSM == null)
            {
                string cache = Path.Combine(
                    ThuMucCauHinhNen,
                    "osm-cache-7ngay");

                mayChuOSM = new MayChuTrungGianOSM(
                    cache,
                    noiDung => GhiNhatKyBanDo(
                        "OSM_PROXY | " + noiDung));
            }

            Tiles tiles = banDoMapWinGIS.Tiles;

            if (tiles == null)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa khởi tạo Tiles.");
            }

            // Kiem tra TCP loopback khong tai them tile.
            mayChuOSM.KiemTraSanSang();

            string url = mayChuOSM.DuongDanTiles;

            int chiSo =
                tiles.Providers.get_IndexByProviderId(
                    MaNhaCungCapOSMNoiBo);

            // Port cua proxy thay doi theo moi lan chay.
            if (chiSo >= 0 &&
                !string.Equals(
                    tiles.Providers.get_UrlPattern(chiSo),
                    url,
                    StringComparison.Ordinal))
            {
                if (tiles.Visible &&
                    tiles.ProviderId == MaNhaCungCapOSMNoiBo)
                {
                    tiles.Visible = false;


                }

                if (!tiles.Providers.Remove(
                    MaNhaCungCapOSMNoiBo,
                    false))
                {
                    throw new InvalidOperationException(
                        "Không thể thay provider OSM cũ 11301.");
                }

                chiSo = -1;
            }

            if (chiSo < 0)
            {
                bool thanhCong = tiles.Providers.Add(
                    MaNhaCungCapOSMNoiBo,
                    "OpenStreetMap - qua may chu cuc bo",
                    url,
                    tkTileProjection.SphericalMercator,
                    0,
                    19);

                if (!thanhCong)
                {
                    throw new InvalidOperationException(
                        "Không đăng ký được OpenStreetMap: " +
                        tiles.Providers.get_ErrorMsg(
                            tiles.Providers.LastErrorCode));
                }
            }

            // Dung cache chung voi Tile Server ve tinh.
            BdTile_CauHinhCache(tiles);

            if (tiles.Visible &&
                tiles.ProviderId != MaNhaCungCapOSMNoiBo)
            {
                tiles.Visible = false;
            }

            if (tiles.ProviderId != MaNhaCungCapOSMNoiBo)
                tiles.ProviderId = MaNhaCungCapOSMNoiBo;

            if (!tiles.UseServer)
                tiles.UseServer = true;

            if (!tiles.Visible)
                tiles.Visible = true;

            if (tiles.ProjectionStatus ==
                tkTilesProjectionStatus.tpsEmptyOrInvalid)
            {
                throw new InvalidOperationException(
                    "MapWinGIS không chuyển được OSM EPSG:3857 " +
                    "sang hệ tọa độ của bản đồ.");
            }

            // UTM khong hien thi cac tile WebMercator o muc zoom qua xa.
            if (tiles.CurrentZoom < 6)
            {
                try { BdTile_PhongToHaNoi(); }
                catch (Exception zoomError)
                {
                    GhiNhatKyBanDo(
                        "OSM_INITIAL_ZOOM_FAILED | " + zoomError.Message);
                }
            }

            if (tiles.ProviderId != MaNhaCungCapOSMNoiBo ||
                !tiles.Visible ||
                !tiles.UseServer)
            {
                throw new InvalidOperationException(
                    "MapWinGIS chưa bật được provider OSM 11301.");
            }

            DuaLopDiemLenTren();
        }


        private void TatOpenStreetMap()
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            Tiles tiles = banDoMapWinGIS.Tiles;

            if (tiles.Visible)

                tiles.Visible = false;

            if (tiles.UseServer)
                tiles.UseServer = false;
        }

        // =====================================================
        // 10. MAY CHU TRUNG GIAN OSM
        // =====================================================

        private sealed class MayChuTrungGianOSM : IDisposable
        {
            private const string UserAgentOSM =
                "ManagerGISAcademy/1.0 (desktop GIS application)";

            private readonly string thuMucCache;
            private readonly Action<string> ghiNhatKy;
            private readonly TcpListener mayLangNghe;

            private readonly SemaphoreSlim gioiHanTai =
                new SemaphoreSlim(2, 2);

            private readonly string maPhien =
                Guid.NewGuid().ToString("N");
            private volatile bool daDung;

            private int soYeuCau;
            private int soThanhCong;
            private int soThatBai;

            private string loiGanNhat = "";

            public int SoYeuCau
            {
                get { return Volatile.Read(ref soYeuCau); }
            }

            public int SoThanhCong
            {
                get { return Volatile.Read(ref soThanhCong); }
            }

            public int SoThatBai
            {
                get { return Volatile.Read(ref soThatBai); }
            }

            public string LoiGanNhat
            {
                get { return loiGanNhat; }
            }

            public int Cong { get; private set; }

            public string DuongDanTiles
            {
                get
                {
                    return "http://127.0.0.1:" +
                           Cong + "/" + maPhien +
                           "/{zoom}/{x}/{y}.png";
                }
            }

            public MayChuTrungGianOSM(
                string cache,
                Action<string> ghi)
            {
                thuMucCache = cache;
                ghiNhatKy = ghi;

                Directory.CreateDirectory(cache);

                mayLangNghe = new TcpListener(
                    IPAddress.Loopback,
                    0);

                mayLangNghe.Start(16);

                Cong =
                    ((IPEndPoint)
                        mayLangNghe.LocalEndpoint).Port;

                if (ghiNhatKy != null)
                {
                    ghiNhatKy(
                        "OSM_PROXY_STARTED | port=" + Cong);
                }

                Task.Run((Action)VongLapNhanYeuCau);
            }

            // ---------------------------------------------
            // 10.1. Kiem tra local proxy /health
            // ---------------------------------------------

            public void KiemTraSanSang()
            {
                string url =
                    "http://127.0.0.1:" +
                    Cong + "/" + maPhien + "/health";

                HttpWebRequest req =
                    (HttpWebRequest)WebRequest.Create(url);

                req.Proxy = null;
                req.Timeout = 3000;
                req.ReadWriteTimeout = 3000;
                req.KeepAlive = false;

                try
                {
                    using (HttpWebResponse res =
                        (HttpWebResponse)req.GetResponse())
                    {
                        if (res.StatusCode != HttpStatusCode.OK)
                        {
                            throw new IOException(
                                "HTTP " + (int)res.StatusCode);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "Proxy OpenStreetMap 127.0.0.1:" +
                        Cong + " không phản hồi. " +
                        ex.Message,
                        ex);
                }
            }

            // ---------------------------------------------
            // 10.2. Lang nghe yeu cau
            // ---------------------------------------------

            private void VongLapNhanYeuCau()
            {
                while (!daDung)
                {
                    TcpClient ketNoi;

                    try
                    {
                        ketNoi =
                            mayLangNghe.AcceptTcpClient();
                    }
                    catch
                    {
                        break;
                    }

                    Task.Run(
                        () => XuLyKetNoi(ketNoi));
                }
            }

            private void XuLyKetNoi(TcpClient ketNoi)
            {
                using (ketNoi)
                {
                    try
                    {
                        ketNoi.ReceiveTimeout = 8000;
                        ketNoi.SendTimeout = 8000;
                        ketNoi.NoDelay = true;

                        using (NetworkStream luong =
                            ketNoi.GetStream())
                        {
                            string yeuCau;

                            using (StreamReader doc =
                                new StreamReader(
                                    luong,
                                    Encoding.ASCII,
                                    false,
                                    1024,
                                    true))
                            {
                                yeuCau = doc.ReadLine();

                                string dong;
                                int soDong = 0;

                                while ((dong = doc.ReadLine()) != null &&
                                       dong.Length > 0 &&
                                       ++soDong < 60)
                                {
                                }
                            }

                            if (yeuCau != null &&
                                yeuCau.StartsWith(
                                    "GET /" + maPhien +
                                    "/health ",
                                    StringComparison.Ordinal))
                            {
                                PhanHoi(
                                    luong,
                                    200,
                                    Encoding.ASCII.GetBytes("OK"),
                                    "text/plain");

                                return;
                            }


                            int z, x, y;

                            if (!DocDiaChiTile(
                                yeuCau,
                                out z,
                                out x,
                                out y))
                            {
                                PhanHoi(
                                    luong,
                                    404,
                                    null,
                                    "text/plain");

                                return;
                            }

                            Interlocked.Increment(
                                ref soYeuCau);

                            byte[] anh =
                                LayTile(z, x, y);


                            if (anh == null)
                            {
                                Interlocked.Increment(
                                    ref soThatBai);

                                PhanHoi(
                                    luong,
                                    502,
                                    null,
                                    "text/plain");
                            }
                            else
                            {
                                Interlocked.Increment(
                                    ref soThanhCong);

                                PhanHoi(
                                    luong,
                                    200,
                                    anh,
                                    "image/png");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        loiGanNhat = ex.Message;

                        Debug.WriteLine(
                            "[OSM_PROXY_FAILED] " + ex);
                    }
                }
            }

            // ---------------------------------------------
            // 10.3. Phan tich URL /z/x/y.png
            // ---------------------------------------------

            private bool DocDiaChiTile(
                string yeuCau,
                out int z,
                out int x,
                out int y)
            {
                z = x = y = 0;

                if (string.IsNullOrEmpty(yeuCau) ||
                    !yeuCau.StartsWith(
                        "GET /",

                        StringComparison.Ordinal))
                    return false;

                string[] phan = yeuCau.Split(' ');

                if (phan.Length < 2)
                    return false;

                string[] duong =
                    phan[1].Trim('/').Split('/');

                if (duong.Length != 4 ||
                    duong[0] != maPhien ||
                    !duong[3].EndsWith(
                        ".png",
                        StringComparison.OrdinalIgnoreCase))
                    return false;

                string chuoiY = duong[3].Substring(
                    0,
                    duong[3].Length - 4);

                if (!int.TryParse(
                        duong[1],
                        out z) ||
                    !int.TryParse(
                        duong[2],
                        out x) ||
                    !int.TryParse(
                        chuoiY,
                        out y) ||
                    z < 0 ||
                    z > 19)
                    return false;

                int gioiHan = 1 << z;

                return x >= 0 &&
                       y >= 0 &&
                       x < gioiHan &&
                       y < gioiHan;
            }

            // ---------------------------------------------
            // 10.4. Lay tile tu cache / OpenStreetMap
            // ---------------------------------------------

            private byte[] LayTile(
                int z,
                int x,
                int y)
            {
                string tep = Path.Combine(
                    thuMucCache,
                    z.ToString(),
                    x.ToString(),
                    y + ".png");

                byte[] cache = DocCache(
                    tep,
                    true);

                if (cache != null)
                    return cache;

                if (!gioiHanTai.Wait(15000))
                    return DocCache(tep, false);

                try
                {
                    cache = DocCache(tep, true);

                    if (cache != null)
                        return cache;

                    string url = string.Format(
                        CultureInfo.InvariantCulture,
                        "https://tile.openstreetmap.org/{0}/{1}/{2}.png",
                        z,
                        x,
                        y);

                    try
                    {
                        // Ho tro TLS 1.2 tren .NET Framework.
                        if ((ServicePointManager.SecurityProtocol &
                            SecurityProtocolType.Tls12) == 0)
                        {
                            ServicePointManager.SecurityProtocol |=
                                SecurityProtocolType.Tls12;
                        }

                        HttpWebRequest req =
                            (HttpWebRequest)WebRequest.Create(url);

                        req.UserAgent = UserAgentOSM;
                        req.Timeout = 10000;
                        req.ReadWriteTimeout = 10000;
                        req.AllowAutoRedirect = false;

                        using (HttpWebResponse res =
                            (HttpWebResponse)req.GetResponse())
                        using (Stream input =
                            res.GetResponseStream())
                        using (MemoryStream memory =
                            new MemoryStream())
                        {
                            if (res.StatusCode != HttpStatusCode.OK ||
                                input == null ||
                                res.ContentLength > 4 * 1024 * 1024)
                            {
                                return DocCache(tep, false);
                            }

                            byte[] buf = new byte[8192];
                            int count;

                            while ((count =
                                input.Read(
                                    buf,
                                    0,
                                    buf.Length)) > 0)
                            {
                                memory.Write(
                                    buf,
                                    0,
                                    count);

                                if (memory.Length >
                                    4 * 1024 * 1024)
                                {
                                    throw new IOException(
                                        "Tile vượt quá 4 MB.");
                                }
                            }

                            byte[] anh = memory.ToArray();

                            if (anh.Length < 8 ||
                                anh[0] != 137 ||
                                anh[1] != 80 ||
                                anh[2] != 78 ||
                                anh[3] != 71)
                            {
                                throw new IOException(
                                    "Máy chủ không trả về PNG hợp lệ.");
                            }

                            try
                            {
                                Directory.CreateDirectory(
                                    Path.GetDirectoryName(tep));

                                File.WriteAllBytes(tep, anh);
                            }
                            catch
                            {
                            }

                            return anh;
                        }
                    }
                    catch (Exception ex)
                    {
                        loiGanNhat =
                            ex.GetType().Name +
                            ": " + ex.Message;

                        Debug.WriteLine(
                            "[OSM_PROXY_HTTP_FAILED] " +
                            url + " | " + loiGanNhat);

                        return DocCache(tep, false);
                    }
                }
                finally
                {
                    gioiHanTai.Release();
                }
            }

            // ---------------------------------------------
            // 10.5. Cache anh OSM
            // ---------------------------------------------

            private static byte[] DocCache(
                string tep,
                bool canConHan)
            {
                try
                {
                    if (!File.Exists(tep))
                        return null;

                    if (canConHan &&
                        DateTime.UtcNow -
                        File.GetLastWriteTimeUtc(tep) >=
                        TimeSpan.FromDays(7))
                    {
                        return null;
                    }

                    return File.ReadAllBytes(tep);
                }
                catch
                {
                    return null;
                }
            }

            // ---------------------------------------------
            // 10.6. Phan hoi HTTP
            // ---------------------------------------------

            private static void PhanHoi(
                Stream luong,
                int ma,
                byte[] anh,
                string contentType)
            {
                string trangThai =
                    ma == 200 ? "200 OK" :
                    ma == 502 ? "502 Bad Gateway" :
                    "404 Not Found";

                int doDai =
                    anh == null ? 0 : anh.Length;

                string dau =
                    "HTTP/1.1 " + trangThai + "\r\n" +
                    "Content-Type: " + contentType + "\r\n" +
                    "Content-Length: " + doDai + "\r\n" +
                    "Connection: close\r\n\r\n";

                byte[] header =
                    Encoding.ASCII.GetBytes(dau);

                luong.Write(
                    header,
                    0,
                    header.Length);

                if (doDai > 0)
                {
                    luong.Write(
                        anh,
                        0,
                        doDai);
                }

                luong.Flush();
            }

            public void Dispose()
            {
                daDung = true;
                mayLangNghe.Stop();
            }
        }
    }
}
