using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using WinLabel = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    /// <summary>
    /// Chu giai ban do GIS hien dai.
    ///
    /// - Nhan biet ranh gioi Ha Noi, quan/huyen.
    /// - Hien thi diem quan doi va anh ve tinh.
    /// - Bat/tat lop truc tiep tu chu giai.
    /// - Dong bo danh sach lop MapWinGIS.
    /// - Khong sua du lieu PostGIS.
    /// - Gop chu giai vao bang LOP BAN DO.
    ///
    /// C# 7.3 / .NET Framework 4.8.
    /// </summary>
    public partial class FormMain
    {

        // =====================================================
        // 4. THU GON / MO RONG CHU GIAI
        // =====================================================

        private void DoiThuGonChuGiai()
        {
            if (pnlChuGiaiBanDo == null ||
                pnlChuGiaiBanDo.IsDisposed)
            {
                return;
            }

            chuGiaiThuGon =
                !chuGiaiThuGon;

            pnlNoiDungChuGiai.Visible =
                !chuGiaiThuGon;

            btnThuGonChuGiai.Text =
                chuGiaiThuGon ? "+" : "−";

            DatViTriChuGiai();

            if (!chuGiaiThuGon)
            {
                CapNhatChuGiaiBanDo();
            }

            pnlChuGiaiBanDo.BringToFront();
        }

        // Căn vị trí chú giải trong bảng lớp.
        private void DatViTriChuGiai()
        {
            CgCanLaiBangGop();
        }

        private void CgKhiBangGopDoiKichThuoc(
            object sender,
            EventArgs e)
        {
            CgCanLaiBangGop();
        }

        private void CgKhiChuGiaiBiDich(
            object sender,
            EventArgs e)
        {
            CgCanLaiBangGop();
        }

        // =====================================================
        // 5. CAN LAI HAI KHU VUC THANH MOT BANG
        // =====================================================

        // GD5.3 V3: chia khung theo phan noi dung dang hien thi.
        // Khong de chu giai truot xuong day khi danh sach lop chi co it muc.
        // Khi chu giai mo rong, cac the duoc cuon trong FlowLayoutPanel.
        private void CgCanLaiBangGop()
        {
            if (cgDangCanBangGop ||
                pnlRightBody == null || pnlRightBody.IsDisposed ||
                bangDanhSachLop == null || bangDanhSachLop.IsDisposed ||
                pnlChuGiaiBanDo == null || pnlChuGiaiBanDo.IsDisposed ||
                pnlChuGiaiBanDo.Parent != pnlRightBody)
                return;

            cgDangCanBangGop = true;
            try
            {
                int w = pnlRightBody.ClientSize.Width;
                int h = pnlRightBody.ClientSize.Height;
                if (w < 1 || h < 1)
                    return;

                // Khong cho phan chu giai tran xuong thanh trang thai duoi ban do.
                // Dung toa do man hinh vi hai panel khong cung Parent.
                if (pnlMapCanvas != null && !pnlMapCanvas.IsDisposed &&
                    pnlMapCanvas.IsHandleCreated && pnlRightBody.IsHandleCreated &&
                    pnlMapCanvas.ClientSize.Height > 0)
                {
                    Rectangle mapOnScreen =
                        pnlMapCanvas.RectangleToScreen(pnlMapCanvas.ClientRectangle);
                    int topPanel = pnlRightBody.PointToScreen(Point.Empty).Y;
                    int gioiHanDuoi = mapOnScreen.Bottom - topPanel - 8;
                    if (gioiHanDuoi > 0)
                        h = Math.Min(h, gioiHanDuoi);
                }

                // Cho mot lop khoang 350 px (bo loc, nut cong cu, the lop).
                // Them lop thi tang phan danh sach, nhung van giu cho chu giai.
                int soLop = 1;
                if (danhSachLop != null && !danhSachLop.IsDisposed)
                    soLop = Math.Max(1, danhSachLop.Items.Count);

                int caoDanhSachDeXuat = 350 +
                    Math.Min(5, Math.Max(0, soLop - 1)) * 66;

                // Toi thieu 44 px cho thanh tieu de chu giai khi thu gon.
                // Khi mo rong, giu toi thieu 150 px neu du cho, de cuon noi dung.
                int caoChuGiaiToiThieu = chuGiaiThuGon
                    ? Math.Min(44, h)
                    : Math.Min(150, Math.Max(44, h / 3));

                int caoDanhSach = Math.Min(
                    caoDanhSachDeXuat,
                    Math.Max(0, h - caoChuGiaiToiThieu));

                int caoChuGiai = chuGiaiThuGon
                    ? Math.Min(44, Math.Max(0, h - caoDanhSach))
                    : Math.Max(0, h - caoDanhSach);

                Rectangle vungDanhSach = new Rectangle(
                    0, 0, w, caoDanhSach);
                Rectangle vungChuGiai = new Rectangle(
                    0, caoDanhSach, w, caoChuGiai);

                if (bangDanhSachLop.Bounds != vungDanhSach)
                    bangDanhSachLop.Bounds = vungDanhSach;

                if (pnlChuGiaiBanDo.Bounds != vungChuGiai)
                    pnlChuGiaiBanDo.Bounds = vungChuGiai;

                // Giu nguyen giao dien the Ban do Ha Noi va cac su kien.
                if (uiTheBanDo != null &&
                    !uiTheBanDo.IsDisposed &&
                    uiTheBanDo.Parent == pnlNoiDungChuGiai)
                {
                    uiTheBanDo.BackColor = Color.FromArgb(228, 244, 255);

                    if (uiTieuDeBanDo != null)
                        uiTieuDeBanDo.ForeColor = Color.FromArgb(23, 70, 103);

                    if (uiMoTaBanDo != null)
                        uiMoTaBanDo.ForeColor = Color.FromArgb(110, 75, 193);

                    if (uiNhanTuyChon != null)
                        uiNhanTuyChon.ForeColor = Color.FromArgb(56, 106, 138);

                    if (uiCheDoHienNhan != null)
                    {
                        uiCheDoHienNhan.BackColor = Color.White;
                        uiCheDoHienNhan.ForeColor = Color.FromArgb(23, 74, 107);
                    }
                }

                ChuGiai_CanTieuDe();
            }
            finally
            {
                cgDangCanBangGop = false;
            }
        }

        // =====================================================
        // 6. NHAN BIET LOAI LOP
        // =====================================================

        private int ChuGiai_XacDinhLoaiLop(
            LopMapItem lop)
        {
            if (lop == null)
                return CG_LOP_KHAC;

            if (lop.Handle ==
                handleLopDiemPostGIS)
            {
                return CG_DON_VI;
            }

            if (lop.Handle ==
                handleAnhNenTuDong)
            {
                return CG_RASTER;
            }

            string ten =
                (lop.Ten ?? "").Trim();

            if (ten.Equals(
                "Ranh giới Hà Nội",
                StringComparison.OrdinalIgnoreCase))
            {
                return CG_RANH_TINH;
            }

            if (ten.Equals(
                "Ranh giới quận/huyện",
                StringComparison.OrdinalIgnoreCase))
            {
                return CG_RANH_HUYEN;
            }

            return CG_LOP_KHAC;
        }

        // =====================================================
        // 7. LAY DU LIEU LOP THUC
        // =====================================================
        private void CapNhatChuGiaiBanDo()
        {
            if (!daKhoiTaoChuGiai ||
                dangVeChuGiai ||
                flpCacLopChuGiai == null ||
                flpCacLopChuGiai.IsDisposed ||
                ketNoiMapWinGIS == null ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed)
            {
                return;
            }

            dangVeChuGiai = true;

            flpCacLopChuGiai.SuspendLayout();

            try
            {
                while (
                    flpCacLopChuGiai.Controls.Count > 0)
                {
                    Control cu =
                        flpCacLopChuGiai.Controls[0];

                    flpCacLopChuGiai.Controls
                        .RemoveAt(0);

                    cu.Dispose();
                }

                int tongLop = 0;
                int lopDangBat = 0;

                foreach (LopMapItem lop in cacLop)
                {
                    if (lop == null)
                        continue;

                    bool coLop = false;
                    bool hienThi = false;

                    try
                    {
                        coLop =
                            ketNoiMapWinGIS.CoLop(
                                lop.Handle);

                        if (coLop)
                        {
                            hienThi =
                                ketNoiMapWinGIS
                                .LayHienThiLop(
                                    lop.Handle);
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    if (!coLop)
                        continue;

                    int loai =
                        ChuGiai_XacDinhLoaiLop(
                            lop);

                    string ten =
                        lop.Ten ?? "Lớp GIS";

                    string moTa =
                        "Lớp dữ liệu GIS";

                    Color mau =
                        Color.FromArgb(
                            96, 165, 250);

                    switch (loai)
                    {
                        case CG_DON_VI:
                            ten = "Đơn vị quân đội";

                            moTa =
                                LayThongTinSoDiem() +
                                " · PostGIS";

                            mau = mauDiemDonVi;
                            break;

                        case CG_RASTER:
                            ten = "Ảnh vệ tinh Hà Nội";

                            moTa =
                                "Ảnh raster · Ngoại tuyến";

                            mau = vangChuGiai;
                            break;

                        case CG_RANH_TINH:
                            ten = "Ranh giới Hà Nội";

                            moTa =
                                "Nét liền · Cấp tỉnh";

                            mau = mauRanhTinh;
                            break;

                        case CG_RANH_HUYEN:
                            ten = "Ranh giới quận/huyện";

                            moTa =
                                "Nét đứt · Cấp huyện";

                            mau = mauRanhHuyen;
                            break;
                    }

                    Panel the =
                        TaoTheChuGiai(
                            lop.Handle,
                            loai,
                            ten,
                            moTa,
                            mau,
                            hienThi);

                    flpCacLopChuGiai.Controls.Add(
                        the);

                    tongLop++;

                    if (hienThi)
                        lopDangBat++;
                }

                // OpenStreetMap là nguồn tile,
                // không phải LayerHandle.

                try
                {
                    if (banDoMapWinGIS.Tiles != null &&
                        banDoMapWinGIS.Tiles.Visible)
                    {
                        flpCacLopChuGiai.Controls.Add(
                            TaoTheChuGiai(
                                -1,
                                CG_OSM,
                                "OpenStreetMap",
                                "Bản đồ nền trực tuyến",
                                Color.FromArgb(
                                    125, 195, 255),
                                true));

                        tongLop++;
                        lopDangBat++;
                    }
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo(
                        "MAP_LEGEND_TILES_ERROR | " +
                        ex.Message);
                }

                if (tongLop == 0)
                {
                    WinLabel thongBao = new WinLabel
                    {
                        Width = 230,
                        Height = 72,

                        Text =
                            "Chưa có lớp dữ liệu.\r\n" +
                            "Hãy nạp bản đồ để xem chú giải.",

                        TextAlign =
                            ContentAlignment.MiddleCenter,

                        ForeColor = chuPhuChuGiai,

                        Font = new Font(
                            "Segoe UI", 8.5F)
                    };

                    flpCacLopChuGiai.Controls.Add(
                        thongBao);
                }

                if (lblThongKeChuGiai != null)
                {
                    lblThongKeChuGiai.Text =
                        string.Format(
                            "{0}/{1} lớp đang bật   •   EPSG:{2}",
                            lopDangBat,
                            tongLop,
                            EpsgBanDo);
                }

                if (lblSoLopChuGiai != null)
                {
                    lblSoLopChuGiai.Text =
                        lopDangBat + "/" + tongLop;

                    lblSoLopChuGiai.ForeColor =
                        lopDangBat > 0
                            ? xanhChuGiai
                            : chuPhuChuGiai;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "MAP_LEGEND_REFRESH_ERROR | " +
                    ex);
            }
            finally
            {
                flpCacLopChuGiai.ResumeLayout(
                    true);

                dangVeChuGiai = false;
            }

            CanRongTheChuGiai();
            CgCanLaiBangGop();
        }
    }
}