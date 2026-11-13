using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using WinImage = System.Drawing.Image;
using WinPoint = System.Drawing.Point;

namespace ManagerGISAcademy.GUI.Forms
{
    public partial class FormMain
    {
        private void UiChoCongCuBanDo(
    object sender,
    EventArgs e)
        {
            if (IsDisposed || Disposing)
            {
                Application.Idle -= UiChoCongCuBanDo;
                return;
            }

            if (uiDaKhoiTaoCongCu)
            {
                Application.Idle -= UiChoCongCuBanDo;
                return;
            }

            if (!daKhoiTaoThongTinBanDo ||
                !daKhoiTaoDoDac ||
                !daKhoiTaoChuGiai ||
                pnlCheDoBanDo == null ||
                pnlThanhDoDac == null ||
                pnlChuGiaiBanDo == null ||
                bd11KhungZoom == null ||
                bd11Zoom == null)
            {
                return;
            }

            Application.Idle -= UiChoCongCuBanDo;
            uiDaKhoiTaoCongCu = true;

            UiDatZoomDuoiCongCu();

            pnlCheDoBanDo.Visible = false;
            pnlThanhDoDac.Visible = false;

            if (!chuGiaiThuGon)
                DoiThuGonChuGiai();

            UiCanCongCuBanDo();
            UiBatHenGiamNhan();
        }

        private void UiHenCanCongCuBanDo()
        {
            if (uiChoCanCongCu ||
                !IsHandleCreated ||
                IsDisposed ||
                Disposing)
                return;

            uiChoCanCongCu = true;

            try
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    uiChoCanCongCu = false;

                    if (!IsDisposed &&
                        !Disposing &&
                        phanHeHienTai == "Bản đồ GIS")
                    {
                        UiCanCongCuBanDo();
                    }
                }));
            }
            catch (InvalidOperationException)
            {
                uiChoCanCongCu = false;
            }
        }

        private void UiCanCongCuBanDo()
        {
            if (uiDangCanBanDo ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed)
                return;

            uiDangCanBanDo = true;

            try
            {
                if (phanHeHienTai == "Bản đồ GIS" &&
                    pnlRightCard.Parent != pnlMapCanvas)
                {
                    UiDatBangLopNoi();
                }

                UiDuaThongTinVaoChuGiai();

                if (pnlMapTools != null &&
                    !pnlMapTools.IsDisposed)
                {
                    pnlMapTools.BorderStyle =
                        BorderStyle.None;

                    pnlMapTools.BackColor = uiThe;
                    pnlMapTools.AutoSize = false;
                    pnlMapTools.Dock = DockStyle.None;

                    pnlMapTools.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    pnlMapTools.Padding = Padding.Empty;

                    int xCongCu = Math.Max(
                        14,
                        pnlMapCanvas.ClientSize.Width - 35 - 12);

                    pnlMapTools.SetBounds(
                        xCongCu, 78, 35, 184);

                    UiBoGoc(pnlMapTools, 8);

                    Button[] nut =
                    {
                        btnFullExtent,
                        bd11NutChon,
                        bd11NutKeo,
                        bd11NutDo,
                        btnMapLayers,
                        CongCu_LayNut()
                    };

                    string[] icon =
                    {
                        "extent",
                        "info",
                        "hand",
                        "ruler",
                        "layers",
                        "gis-tools"
                    };

                    string[] tip =
                    {
                        "Toàn cảnh",
                        "Chọn đơn vị",
                        "Di chuyển bản đồ",
                        "Đo khoảng cách / diện tích",
                        "Hiện / ẩn lớp",
                        "Công cụ GIS: tọa độ, khuôn viên, tìm đường, địa hình, in và xuất"
                    };

                    string[] kyHieuDuPhong =
                    {
                        "□",
                        "◎",
                        "✥",
                        "⌁",
                        "≡",
                        "⚒"
                    };

                    for (int i = 0; i < nut.Length; i++)
                    {
                        Button b = nut[i];

                        if (b == null || b.IsDisposed)
                            continue;

                        bool chon =
                            icon[i] == uiCongCuDangChon ||
                            (icon[i] == "layers" &&
                             uiBangLopMo);

                        b.Image = null;
                        b.Padding = Padding.Empty;
                        b.AutoSize = false;
                        b.Dock = DockStyle.None;

                        b.Anchor =
                            AnchorStyles.Top | AnchorStyles.Left;

                        b.MinimumSize = Size.Empty;
                        b.MaximumSize = Size.Empty;

                        b.SetBounds(
                            4,
                            4 + i * 30,
                            27,
                            26);

                        b.BackColor = chon
                            ? Color.FromArgb(226, 244, 232)
                            : uiThe;

                        b.ForeColor = uiChu;

                        b.TextAlign =
                            ContentAlignment.MiddleCenter;

                        UiLamDepNut(b, 6);

                        bool coAnhGoc = false;

                        foreach (Control con in b.Controls)
                        {
                            PictureBox anhGoc =
                                con as PictureBox;

                            if (anhGoc == null ||
                                anhGoc.IsDisposed)
                                continue;

                            bool coAnh =
                                anhGoc.Image != null;

                            anhGoc.Visible = coAnh;

                            if (!coAnh)
                                continue;

                            coAnhGoc = true;
                            anhGoc.Dock = DockStyle.None;

                            anhGoc.Anchor =
                                AnchorStyles.Top | AnchorStyles.Left;

                            anhGoc.SizeMode =
                                PictureBoxSizeMode.Zoom;

                            anhGoc.Size =
                                new Size(17, 17);

                            anhGoc.Location = new WinPoint(
                                Math.Max(
                                    0,
                                    (b.ClientSize.Width -
                                     anhGoc.Width) / 2),

                                Math.Max(
                                    0,
                                    (b.ClientSize.Height -
                                     anhGoc.Height) / 2));

                            anhGoc.BringToFront();

                            uiMeo.SetToolTip(
                                anhGoc, tip[i]);
                        }

                        b.Text = coAnhGoc
                            ? string.Empty
                            : kyHieuDuPhong[i];

                        uiMeo.SetToolTip(b, tip[i]);
                    }

                    btnZoomIn.Visible = false;
                    btnZoomOut.Visible = false;

                    if (btnNapLop != null)
                        btnNapLop.Visible = false;

                    pnlMapTools.BringToFront();
                }

                UiDatZoomDuoiCongCu();

                if (uiDaKhoiTaoCongCu &&
                    !uiDaGanTheoDoiCongCu)
                {
                    uiDaGanTheoDoiCongCu = true;

                    if (bd11NutChon != null)
                    {
                        bd11NutChon.Click += (s, e) =>
                        {
                            uiCongCuDangChon = "info";
                            UiHenCapNhatTrang();
                        };
                    }

                    if (bd11NutKeo != null)
                    {
                        bd11NutKeo.Click += (s, e) =>
                        {
                            uiCongCuDangChon = "hand";
                            UiHenCapNhatTrang();
                        };
                    }

                    if (bd11MenuDo != null)
                    {
                        bd11MenuDo.ItemClicked += (s, e) =>
                        {
                            uiCongCuDangChon = "ruler";
                            UiHenCapNhatTrang();
                        };
                    }

                    if (btnMapLayers != null)
                    {
                        btnMapLayers.Click += (s, e) =>
                        {
                            UiHenCapNhatTrang();
                        };
                    }
                }

                UiCanBangLop();
                UiDatZoomDuoiCongCu();

                if (pnlChuGiaiBanDo != null &&
                    !pnlChuGiaiBanDo.IsDisposed &&
                    pnlChuGiaiBanDo.Visible)
                {
                    UiBoGoc(pnlChuGiaiBanDo, 9);

                    if (chuGiaiThuGon)
                    {
                        pnlChuGiaiBanDo.Width = Math.Min(
                            280,
                            Math.Max(
                                210,
                                pnlMapCanvas.ClientSize.Width - 100));

                        pnlChuGiaiBanDo.Height = 36;
                    }

                    int khoangPhai =
                        uiBangLopMo ? 345 : 18;

                    pnlChuGiaiBanDo.Left = Math.Max(
                        65,
                        pnlMapCanvas.ClientSize.Width -
                        pnlChuGiaiBanDo.Width -
                        khoangPhai);

                    const int khoangDuoi = 68;

                    pnlChuGiaiBanDo.Top = Math.Max(
                        90,
                        pnlMapCanvas.ClientSize.Height -
                        pnlChuGiaiBanDo.Height -
                        khoangDuoi);

                    pnlChuGiaiBanDo.BringToFront();
                }
            }
            finally
            {
                uiDangCanBanDo = false;
            }
        }
        private void UiDatZoomDuoiCongCu()
        {
            if (uiDangCanZoomDoc ||
                pnlMapCanvas == null ||
                pnlMapCanvas.IsDisposed ||
                pnlMapTools == null ||
                pnlMapTools.IsDisposed ||
                bd11KhungZoom == null ||
                bd11KhungZoom.IsDisposed)
                return;

            if (!uiDaGanCanZoomDoc)
            {
                uiDaGanCanZoomDoc = true;

                pnlMapTools.LocationChanged +=
                    UiKhiDoiViTriCongCuZoom;

                pnlMapTools.SizeChanged +=
                    UiKhiDoiViTriCongCuZoom;

                bd11KhungZoom.LocationChanged +=
                    UiKhiDoiViTriCongCuZoom;

                bd11KhungZoom.SizeChanged +=
                    UiKhiDoiViTriCongCuZoom;

                bd11KhungZoom.VisibleChanged +=
                    UiKhiDoiViTriCongCuZoom;
            }

            uiDangCanZoomDoc = true;

            try
            {
                int xCongCu = Math.Max(
                    14,
                    pnlMapCanvas.ClientSize.Width - 35 - 12);

                Rectangle congCu = new Rectangle(
                    xCongCu, 78, 35, 184);

                if (pnlMapTools.Bounds != congCu)
                    pnlMapTools.Bounds = congCu;

                if (bd11KhungZoom.Parent != pnlMapCanvas)
                    pnlMapCanvas.Controls.Add(bd11KhungZoom);

                const int khoangCach = 9;
                const int leDuoi = 12;
                const int caoMongMuon = 115;
                const int caoToiThieu = 75;

                int x = pnlMapTools.Left;
                int y = pnlMapTools.Bottom + khoangCach;

                if (uiNutMoBangLop != null &&
                    !uiNutMoBangLop.IsDisposed &&
                    uiNutMoBangLop.Visible)
                {
                    y = uiNutMoBangLop.Bottom + khoangCach;
                }

                int choConLai =
                    pnlMapCanvas.ClientSize.Height -
                    y - leDuoi;

                if (choConLai < caoToiThieu)
                {
                    if (bd11KhungZoom.Visible)
                        bd11KhungZoom.Visible = false;

                    return;
                }

                int cao = Math.Min(
                    caoMongMuon, choConLai);

                Rectangle khung = new Rectangle(
                    x, y, 35, cao);

                if (bd11KhungZoom.Bounds != khung)
                    bd11KhungZoom.Bounds = khung;

                bd11KhungZoom.Dock = DockStyle.None;

                bd11KhungZoom.Anchor =
                    AnchorStyles.Top | AnchorStyles.Left;

                bd11KhungZoom.BackColor = uiThe;
                bd11KhungZoom.BorderStyle = BorderStyle.None;

                UiBoGoc(bd11KhungZoom, 8);

                if (bd11Zoom != null &&
                    !bd11Zoom.IsDisposed)
                {
                    bd11Zoom.Dock = DockStyle.None;

                    bd11Zoom.Anchor =
                        AnchorStyles.Top | AnchorStyles.Left;

                    bd11Zoom.BackColor = uiThe;

                    Rectangle vungThanh = new Rectangle(
                        3,
                        19,
                        29,
                        Math.Max(40, cao - 37));

                    if (bd11Zoom.Bounds != vungThanh)
                        bd11Zoom.Bounds = vungThanh;
                }

                if (!bd11KhungZoom.Visible)
                    bd11KhungZoom.Visible = true;

                bd11KhungZoom.BringToFront();
                pnlMapTools.BringToFront();
            }
            finally
            {
                uiDangCanZoomDoc = false;
            }
        }

        private void UiKhiDoiViTriCongCuZoom(
            object sender,
            EventArgs e)
        {
            if (!uiDangCanZoomDoc &&
                !IsDisposed &&
                !Disposing)
            {
                UiDatZoomDuoiCongCu();
            }
        }
    }
}