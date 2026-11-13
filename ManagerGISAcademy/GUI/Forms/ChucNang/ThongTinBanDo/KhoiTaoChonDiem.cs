using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using AxMapWinGIS;
using ManagerGISAcademy.BLL;
using ManagerGISAcademy.DTO;
using ManagerGISAcademy.GIS;
using MapWinGIS;

using SpatialDrawer = ManagerGISAcademy.GIS.VeDuLieuKhongGian;
using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private bool daDangKyThongTinBanDo;
        private bool daKhoiTaoThongTinBanDo;
        private bool dangOCheDoChonDiem = true;

        private Panel pnlCheDoBanDo;
        private Button btnCheDoChonDiem;
        private Button btnCheDoDiChuyen;

        private Panel pnlThongTinDiem;
        private Panel ttDvCotHoSo;

        private Label lblTenDiem;
        private FlowLayoutPanel flpThongTinDiem;
        private string maDonViDangChon;

        private const string TtDv_MaLopDanhDau =
            "thong-tin-don-vi";

        private bool ttDvDangNhanChuot;
        private System.Drawing.Point ttDvViTriNhanChuot;

        private long ttDvPhienTraCuu;
        private bool ttDvDangTaiHoSo;

        private YeuCauHoSoBanDo ttDvYeuCauDangCho;
        private ContextMenuStrip ttDvMenuUngVien;

        private Panel ttDvBangUngVien;
        private TextBox ttDvTimUngVien;
        private FlowLayoutPanel ttDvDanhSachUngVien;
        private Label ttDvThongKeUngVien;

        private List<UngVienDonViBanDo> ttDvCacUngVien;

        private int ttDvViTriChonX;
        private int ttDvViTriChonY;

        private SpatialDrawer ttDvBoVe;

        private DonViQuanDoi ttDvHoSoDangHienThi;
        private UngVienDonViBanDo ttDvDiemDangHienThi;

        private Button ttDvBtnHoSo;
        private Button ttDvBtnSaoChep;
        private Button ttDvBtnDinhTam;

        private System.Windows.Forms.Timer ttDvHenKhoiTao;
        private int ttDvSoLanChoMap;

        private readonly Font ttDvFontDongThongTin =
            new Font("Segoe UI", 8.5F);

        private readonly Font ttDvFontNhomLight =
            new Font("Segoe UI", 8.5F, FontStyle.Bold);

        private readonly Font ttDvFontNhanLight =
            new Font("Segoe UI", 7.8F, FontStyle.Bold);

        private readonly Font ttDvFontGiaTriLight =
            new Font("Segoe UI", 9F, FontStyle.Bold);

        private readonly Font ttDvFontHuyHieuLight =
            new Font("Segoe UI", 7.4F, FontStyle.Bold);

        private readonly Font ttDvFontGhiChuLight =
            new Font("Segoe UI", 8F);

        // ==========================================
        // DOI TUONG TRUNG GIAN
        // ==========================================

        private sealed class UngVienDonViBanDo
        {
            public int HandleLop;
            public int ChiSoHinh;
            public string Ma;
            public string Ten;
            public double X;
            public double Y;
            public double KhoangCachBinhPhuong;
        }

        private sealed class YeuCauHoSoBanDo
        {
            public long Phien;
            public UngVienDonViBanDo Diem;
            public int PixelX;
            public int PixelY;
        }

        private readonly ToolTip meoThongTinDiem =
            new ToolTip();

        // ==========================================
        // KHOI TAO
        // ==========================================

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (daDangKyThongTinBanDo)
                return;

            daDangKyThongTinBanDo = true;

            Shown += FormMain_ThongTinBanDo_Shown;

            FormClosed +=
                FormMain_ThongTinBanDo_FormClosed;
        }

        private void FormMain_ThongTinBanDo_Shown(
            object sender,
            EventArgs e)
        {
            if (daKhoiTaoThongTinBanDo ||
                IsDisposed ||
                Disposing)
                return;

            BeginInvoke(
                new MethodInvoker(KhoiTaoThongTinBanDo));
        }

        private void KhoiTaoThongTinBanDo()
        {
            if (daKhoiTaoThongTinBanDo ||
                IsDisposed ||
                Disposing)
                return;

            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated ||
                ketNoiMapWinGIS == null)
            {
                TtDv_ChoBanDoSanSang();
                return;
            }

            TtDv_DungHenKhoiTao();

            try
            {
                TaoThanhCheDoBanDo();
                TaoKhungThongTinDiem();

                banDoMapWinGIS.MouseDownEvent -=
                    BanDo_ChonDiem_MouseDownEvent;

                banDoMapWinGIS.MouseDownEvent +=
                    BanDo_ChonDiem_MouseDownEvent;

                banDoMapWinGIS.MouseUpEvent -=
                    TtDv_BanDo_MouseUpEvent;

                banDoMapWinGIS.MouseUpEvent +=
                    TtDv_BanDo_MouseUpEvent;

                banDoMapWinGIS.SendMouseDown = true;
                banDoMapWinGIS.SendMouseUp = true;

                ttDvBoVe =
                    new SpatialDrawer(banDoMapWinGIS);

                daKhoiTaoThongTinBanDo = true;

                DatCheDoBanDo(true);

                lblPageTitle.TextChanged += (s, ev) =>
                {
                    if (lblPageTitle.Text == "Bản đồ GIS")
                        return;

                    AnKhungThongTinDiem();
                };

                pnlMapCanvas.Resize += (s, ev) =>
                {
                    DatViTriThanhCheDo();
                    TtDv_CanViTriBangUngVien();

                    if (pnlThongTinDiem != null &&
                        pnlThongTinDiem.Visible)
                        GioiHanKhungThongTin();
                };

                pnlMapCard.ClientSizeChanged += (s, ev) =>
                {
                    if (pnlThongTinDiem != null &&
                        pnlThongTinDiem.Visible)
                        GioiHanKhungThongTin();
                };

                GhiNhatKyBanDo("UNIT_CLICK_INIT_OK");
            }
            catch (Exception ex)
            {
                daKhoiTaoThongTinBanDo = false;

                GhiNhatKyBanDo(
                    "UNIT_CLICK_INIT_FAILED | " + ex);

                statusGeneral.Text =
                    "● Không khởi tạo được chọn điểm: " +
                    ex.Message;
            }
        }

        private void TtDv_ChoBanDoSanSang()
        {
            if (ttDvHenKhoiTao != null ||
                IsDisposed ||
                Disposing)
                return;

            ttDvSoLanChoMap = 0;

            ttDvHenKhoiTao =
                new System.Windows.Forms.Timer
                {
                    Interval = 250
                };

            ttDvHenKhoiTao.Tick += (s, e) =>
            {
                if (IsDisposed ||
                    Disposing ||
                    daKhoiTaoThongTinBanDo)
                {
                    TtDv_DungHenKhoiTao();
                    return;
                }

                if (++ttDvSoLanChoMap >= 60)
                {
                    TtDv_DungHenKhoiTao();

                    GhiNhatKyBanDo(
                        "UNIT_CLICK_INIT_TIMEOUT");

                    statusGeneral.Text =
                        "● Bản đồ chưa sẵn sàng để chọn đơn vị.";
                    return;
                }

                KhoiTaoThongTinBanDo();
            };

            ttDvHenKhoiTao.Start();
        }

        private void TtDv_DungHenKhoiTao()
        {
            if (ttDvHenKhoiTao == null)
                return;

            ttDvHenKhoiTao.Stop();
            ttDvHenKhoiTao.Dispose();
            ttDvHenKhoiTao = null;
        }

        private void FormMain_ThongTinBanDo_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            ++ttDvPhienTraCuu;

            ttDvYeuCauDangCho = null;
            ttDvDangNhanChuot = false;

            TtDv_DungHenKhoiTao();
            TtDv_DongMenuUngVien();

            try
            {
                if (banDoMapWinGIS != null &&
                    !banDoMapWinGIS.IsDisposed)
                {
                    banDoMapWinGIS.MouseDownEvent -=
                        BanDo_ChonDiem_MouseDownEvent;

                    banDoMapWinGIS.MouseUpEvent -=
                        TtDv_BanDo_MouseUpEvent;
                }
            }
            catch (Exception)
            {
                // ActiveX co the da giai phong.
            }

            if (ttDvBoVe != null)
            {
                try
                {
                    ttDvBoVe.Dispose();
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "UNIT_HIGHLIGHT_DISPOSE_FAILED | " +
                        ex.Message);
                }

                ttDvBoVe = null;
            }

            meoThongTinDiem.Dispose();

            ttDvFontDongThongTin.Dispose();
            ttDvFontNhomLight.Dispose();
            ttDvFontNhanLight.Dispose();
            ttDvFontGiaTriLight.Dispose();
            ttDvFontHuyHieuLight.Dispose();
            ttDvFontGhiChuLight.Dispose();
        }

        // ==========================================
        // THANH CHE DO BAN DO
        // ==========================================

        private void TaoThanhCheDoBanDo()
        {
            if (pnlCheDoBanDo != null)
                return;

            pnlCheDoBanDo = new Panel
            {
                Width = 244,
                Height = 38,
                BackColor =
                    Color.FromArgb(19, 35, 52)
            };

            btnCheDoChonDiem = new Button
            {
                Text = "CHỌN ĐIỂM",
                Left = 4,
                Top = 4,
                Width = 114,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnCheDoDiChuyen = new Button
            {
                Text = "DI CHUYỂN",
                Left = 123,
                Top = 4,
                Width = 117,
                Height = 30,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnCheDoChonDiem.FlatAppearance.BorderSize = 0;
            btnCheDoDiChuyen.FlatAppearance.BorderSize = 0;

            btnCheDoChonDiem.Click +=
                (s, e) => DatCheDoBanDo(true);

            btnCheDoDiChuyen.Click +=
                (s, e) => DatCheDoBanDo(false);

            pnlCheDoBanDo.Controls.Add(
                btnCheDoChonDiem);

            pnlCheDoBanDo.Controls.Add(
                btnCheDoDiChuyen);

            pnlMapCanvas.Controls.Add(pnlCheDoBanDo);

            DatViTriThanhCheDo();

            pnlCheDoBanDo.BringToFront();
            pnlMapTools.BringToFront();
        }

        private void DatViTriThanhCheDo()
        {
            if (pnlCheDoBanDo == null ||
                pnlMapCanvas == null)
                return;

            pnlCheDoBanDo.Left = Math.Max(
                65,
                (pnlMapCanvas.ClientSize.Width -
                 pnlCheDoBanDo.Width) / 2);

            pnlCheDoBanDo.Top = 8;
        }

        private void DatCheDoBanDo(bool chonDiem)
        {
            if (banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
                return;

            if (cheDoDoDac != 0 &&
                !dd26DangDoiCongCu)
            {
                ThoatDoDacKhiChuyenCongCu();
            }

            TtDv_DongMenuUngVien();

            dangOCheDoChonDiem = chonDiem;
            ttDvDangNhanChuot = false;

            banDoMapWinGIS.SendMouseDown = true;
            banDoMapWinGIS.SendMouseUp = true;

            banDoMapWinGIS.CursorMode =
                chonDiem
                    ? tkCursorMode.cmNone
                    : tkCursorMode.cmPan;

            banDoMapWinGIS.MapCursor =
                tkCursor.crsrArrow;

            if (btnCheDoChonDiem != null)
            {
                btnCheDoChonDiem.BackColor =
                    chonDiem
                        ? Color.FromArgb(17, 128, 100)
                        : Color.FromArgb(48, 65, 83);
            }

            if (btnCheDoDiChuyen != null)
            {
                btnCheDoDiChuyen.BackColor =
                    chonDiem
                        ? Color.FromArgb(48, 65, 83)
                        : Color.FromArgb(17, 128, 100);
            }

            if (!chonDiem)
                AnKhungThongTinDiem();

            if (statusGeneral != null)
            {
                statusGeneral.Text = chonDiem
                    ? "● Chọn đơn vị: nhấp vào ký hiệu; nếu nhiều điểm gần nhau, chọn trong danh sách."
                    : "● Chế độ di chuyển: giữ chuột trái để kéo bản đồ.";
            }
        }
    }
}