using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Label = System.Windows.Forms.Label;

namespace ManagerGISAcademy.GUI.Forms
{
    // C# 7.3 / .NET Framework 4.8. Tái sử dụng danhSachLop và bộ chọn nền.
    public partial class FormMain
    {
        private bool Lp26_SanSangLuuThuTu()
        {
            return daSapXepLop && !IsDisposed && !Disposing &&
                !dangThaoTacBanDo && !dangCapNhatLop && !dangLamMoiGiaoDienLop &&
                ketNoiMapWinGIS != null && banDoMapWinGIS != null &&
                !banDoMapWinGIS.IsDisposed && banDoMapWinGIS.IsHandleCreated;
        }

        private static string Lp26_TepThuTuLop()
        {
            return Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
                "ManagerGISAcademy", "thu-tu-lop.xml");
        }

        private List<int> Lp26_LayThuTuHienTai()
        {
            List<int> thuTu = new List<int>(ketNoiMapWinGIS.LayDanhSachLayerHandle());
            thuTu.Reverse();
            return thuTu;
        }

        private string Lp26_KhoaLopDeLuu(int handle)
        {
            if (handle == handleAnhNenTuDong) return "";
            string ten = banDoMapWinGIS.get_LayerName(handle) ?? "";
            if (handle == handleLopDiemPostGIS || LaLopPostGIS(ten))
                return "postgis:don-vi-quan-doi";
            string tep = banDoMapWinGIS.get_LayerFilename(handle);
            if (!string.IsNullOrWhiteSpace(tep) && Path.IsPathRooted(tep))
                return "tep:" + Path.GetFullPath(tep).Normalize(NormalizationForm.FormC);
            if (string.IsNullOrWhiteSpace(ten)) return "";
            object duLieu = banDoMapWinGIS.get_GetObject(handle);
            string loai = duLieu is MapWinGIS.Image ? "anh" :
                duLieu is MapWinGIS.Grid ? "luoi" : "vector";
            return "tam:" + loai + ":" + ten.Trim().Normalize(NormalizationForm.FormC);
        }

        private Dictionary<string, int> Lp26_LopCoKhoaDuyNhat(List<int> thuTu)
        {
            Dictionary<string, int> ketQua = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> trung = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (int handle in thuTu)
            {
                string khoa = Lp26_KhoaLopDeLuu(handle);
                if (khoa.Length == 0) continue;
                if (ketQua.ContainsKey(khoa)) trung.Add(khoa);
                else ketQua.Add(khoa, handle);
            }
            foreach (string khoa in trung) ketQua.Remove(khoa);
            return ketQua;
        }

        private void Lp26_LuuThuTuLop()
        {
            if (!Lp26_SanSangLuuThuTu()) return;
            string tepTam = null;
            try
            {
                List<int> thuTu = Lp26_LayThuTuHienTai();
                Dictionary<string, int> lop = Lp26_LopCoKhoaDuyNhat(thuTu);
                if (lop.Count == 0)
                {
                    statusGeneral.Text = "● Chưa có lớp dữ liệu có nguồn riêng để lưu thứ tự.";
                    return;
                }
                if (lop.Count > 2000)
                    throw new InvalidOperationException("Số lớp vượt giới hạn của tệp thứ tự.");
                Dictionary<int, string> khoaTheoHandle = new Dictionary<int, string>();
                foreach (KeyValuePair<string, int> muc in lop) khoaTheoHandle.Add(muc.Value, muc.Key);
                string tep = Lp26_TepThuTuLop();
                Directory.CreateDirectory(Path.GetDirectoryName(tep));
                tepTam = tep + "." + Guid.NewGuid().ToString("N") + ".tmp";
                XmlWriterSettings thietLap = new XmlWriterSettings
                {
                    Indent = true,
                    Encoding = new UTF8Encoding(false)
                };
                using (XmlWriter ghi = XmlWriter.Create(tepTam, thietLap))
                {
                    ghi.WriteStartDocument();
                    ghi.WriteStartElement("thu-tu-lop");
                    ghi.WriteAttributeString("phien-ban", "1");
                    foreach (int handle in thuTu)
                    {
                        string khoa;
                        if (!khoaTheoHandle.TryGetValue(handle, out khoa)) continue;
                        ghi.WriteStartElement("lop");
                        ghi.WriteAttributeString("khoa", khoa);
                        ghi.WriteEndElement();
                    }
                    ghi.WriteEndElement();
                    ghi.WriteEndDocument();
                }
                // Chỉ thay bản đang lưu sau khi ghi xong tệp tạm.
                if (File.Exists(tep)) File.Replace(tepTam, tep, null);
                else File.Move(tepTam, tep);
                int boQua = thuTu.Count - lop.Count - (thuTu.Contains(handleAnhNenTuDong) ? 1 : 0);
                statusGeneral.Text = "● Đã lưu thứ tự " + lop.Count + " lớp" +
                    (boQua > 0 ? "; bỏ qua " + boQua + " lớp trùng hoặc thiếu thông tin nguồn." : ".");
                GhiNhatKyBanDo("LAYER_ORDER_SAVED | count=" + lop.Count + " | skipped=" + boQua);
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LAYER_ORDER_SAVE_FAILED | " + ex);
                statusGeneral.Text = "● Không lưu được thứ tự lớp: " + ex.Message;
            }
            finally
            {
                if (tepTam != null)
                {
                    try { if (File.Exists(tepTam)) File.Delete(tepTam); }
                    catch (Exception ex) { GhiNhatKyBanDo("LAYER_ORDER_TEMP_CLEANUP_FAILED | " + ex.Message); }
                }
            }
        }

        private List<string> Lp26_DocThuTuDaLuu(string tep)
        {
            XmlDocument taiLieu = new XmlDocument { XmlResolver = null };
            XmlReaderSettings thietLap = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = null,
                MaxCharactersInDocument = 4 * 1024 * 1024
            };
            using (XmlReader doc = XmlReader.Create(tep, thietLap)) taiLieu.Load(doc);
            XmlElement goc = taiLieu.DocumentElement;
            if (goc == null || goc.Name != "thu-tu-lop" || goc.GetAttribute("phien-ban") != "1")
                throw new InvalidDataException("Tệp thứ tự lớp không đúng định dạng.");
            List<string> ketQua = new List<string>();
            HashSet<string> daGap = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XmlNode nut in goc.ChildNodes)
            {
                XmlElement muc = nut as XmlElement;
                if (muc == null || muc.Name != "lop") continue;
                string khoa = muc.GetAttribute("khoa");
                if (string.IsNullOrWhiteSpace(khoa) || !daGap.Add(khoa) || ketQua.Count >= 2000)
                    throw new InvalidDataException("Danh sách trong tệp thứ tự lớp không hợp lệ.");
                ketQua.Add(khoa);
            }
            if (ketQua.Count == 0) throw new InvalidDataException("Tệp thứ tự chưa có lớp dữ liệu.");
            return ketQua;
        }

        private static List<int> Lp26_TaoThuTuKhoiPhuc(
            List<int> hienTai, List<int> daGhep, int handleNen)
        {
            List<int> ketQua = new List<int>(hienTai);
            HashSet<int> lopDaGhep = new HashSet<int>(daGhep);
            int chiSo = 0;
            // Chỉ thay các ô của lớp đã ghép; lớp mới giữ vị trí tương đối.
            for (int i = 0; i < ketQua.Count; i++)
                if (lopDaGhep.Contains(ketQua[i])) ketQua[i] = daGhep[chiSo++];
            if (handleNen >= 0 && ketQua.Remove(handleNen)) ketQua.Add(handleNen);
            return ketQua;
        }

        private void Lp26_ApDungThuTu(List<int> trenXuong)
        {
            if (trenXuong.Count != banDoMapWinGIS.NumLayers ||
                new HashSet<int>(trenXuong).Count != trenXuong.Count)
                throw new InvalidOperationException("Danh sách lớp đã thay đổi trong lúc sắp xếp.");
            foreach (int handle in trenXuong)
                if (!ketNoiMapWinGIS.CoLop(handle))
                    throw new InvalidOperationException("Một lớp không còn trên bản đồ.");

            // Cố định lần lượt từ đáy lên; handle vẫn giữ nguyên khi đổi chỗ.
            for (int dich = 0; dich < trenXuong.Count; dich++)
            {
                int handle = trenXuong[trenXuong.Count - 1 - dich];
                int viTri = ketNoiMapWinGIS.LayViTriLop(handle);

                if (viTri < dich)
                    throw new InvalidOperationException("Không xác định được thứ tự lớp.");

                while (viTri > dich)
                {
                    ketNoiMapWinGIS.DuaLopXuongMotBac(handle);
                    viTri--;
                }
            }
        }

        private void Lp26_DatThuTuCoHoanTac(List<int> dich, List<int> banDau)
        {
            if (banDoMapWinGIS.IsLocked != MapWinGIS.tkLockMode.lmUnlock)
                throw new InvalidOperationException("Bản đồ đang cập nhật, hãy thử lại sau.");

            bool tuKhoa = false;
            dangThaoTacBanDo = true;

            try
            {
                banDoMapWinGIS.LockWindow(MapWinGIS.tkLockMode.lmLock);
                tuKhoa = true;

                try
                {
                    Lp26_ApDungThuTu(dich);
                }
                catch (Exception loiApDung)
                {
                    try
                    {
                        Lp26_ApDungThuTu(banDau);
                    }
                    catch (Exception loiHoanTac)
                    {
                        throw new AggregateException(
                            "Không áp dụng được thứ tự; việc trả lại thứ tự cũ cũng thất bại.",
                            loiApDung,
                            loiHoanTac);
                    }

                    throw new InvalidOperationException(
                        "Không áp dụng được thứ tự đã lưu; đã trả lại thứ tự trước đó.",
                        loiApDung);
                }
            }
            finally
            {
                try
                {
                    if (tuKhoa)
                        banDoMapWinGIS.LockWindow(MapWinGIS.tkLockMode.lmUnlock);
                }
                finally
                {
                    dangThaoTacBanDo = false;
                }
            }
        }

        private void Lp26_KhoiPhucThuTuLop()
        {
            if (!Lp26_SanSangLuuThuTu()) return;

            try
            {
                string tep = Lp26_TepThuTuLop();

                if (!File.Exists(tep))
                {
                    statusGeneral.Text =
                        "● Chưa có thứ tự đã lưu. Hãy sắp xếp lớp rồi bấm LƯU THỨ TỰ.";
                    return;
                }

                List<string> daLuu = Lp26_DocThuTuDaLuu(tep);
                List<int> banDau = Lp26_LayThuTuHienTai();
                Dictionary<string, int> lop = Lp26_LopCoKhoaDuyNhat(banDau);
                List<int> daGhep = new List<int>();

                foreach (string khoa in daLuu)
                {
                    int handle;

                    if (lop.TryGetValue(khoa, out handle))
                        daGhep.Add(handle);
                }

                if (daGhep.Count == 0)
                {
                    statusGeneral.Text =
                        "● Chưa có lớp phù hợp. Hãy nạp lại các lớp đã lưu rồi bấm KHÔI PHỤC.";
                    return;
                }

                List<int> dich = Lp26_TaoThuTuKhoiPhuc(
                    banDau,
                    daGhep,
                    handleAnhNenTuDong);

                Lp26_DatThuTuCoHoanTac(dich, banDau);

                int thieu = daLuu.Count - daGhep.Count;

                statusGeneral.Text =
                    "● Đã khôi phục thứ tự " + daGhep.Count + " lớp" +
                    (thieu > 0
                        ? "; bỏ qua " + thieu + " lớp chưa nạp hoặc trùng nguồn."
                        : ".");

                GhiNhatKyBanDo(
                    "LAYER_ORDER_RESTORED | matched=" +
                    daGhep.Count + " | missing=" + thieu);

                try
                {
                    ketNoiMapWinGIS.VeLaiBanDo();
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo("LAYER_ORDER_REDRAW_FAILED | " + ex);
                    statusGeneral.Text =
                        "● Đã khôi phục thứ tự; chưa vẽ lại được bản đồ: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo("LAYER_ORDER_RESTORE_FAILED | " + ex);
                statusGeneral.Text =
                    "● Không khôi phục được thứ tự lớp: " + ex.Message;
            }
            finally
            {
                HenLamMoiDanhSachLop();
                HenCapNhatChuGiai();
            }
        }

        private bool Lp26_CoTheDoiThuTu(
            int handle,
            int viTri,
            bool duaLen)
        {
            if (handle < 0 ||
                handle == handleAnhNenTuDong ||
                banDoMapWinGIS == null ||
                banDoMapWinGIS.IsDisposed ||
                viTri < 0 ||
                viTri >= banDoMapWinGIS.NumLayers ||
                banDoMapWinGIS.get_LayerHandle(viTri) != handle)
                return false;

            int dich = viTri + (duaLen ? 1 : -1);

            if (dich < 0 || dich >= banDoMapWinGIS.NumLayers)
                return false;

            int handleKeBen = banDoMapWinGIS.get_LayerHandle(dich);

            // Không cho lớp dữ liệu vượt xuống dưới ảnh nền ngoại tuyến.
            return handleKeBen >= 0 &&
                   handleKeBen != handleAnhNenTuDong;
        }

        private void Lp26_DoiThuTuLop(int handle, bool duaLen)
        {
            if (IsDisposed || Disposing || dangThaoTacBanDo ||
                dangCapNhatLop || dangLamMoiGiaoDienLop ||
                ketNoiMapWinGIS == null || danhSachLop == null ||
                banDoMapWinGIS == null || banDoMapWinGIS.IsDisposed ||
                !banDoMapWinGIS.IsHandleCreated)
                return;

            try
            {
                if (!ketNoiMapWinGIS.CoLop(handle))
                {
                    statusGeneral.Text = "● Lớp không còn trên bản đồ.";
                    return;
                }

                int viTriCu = ketNoiMapWinGIS.LayViTriLop(handle);

                if (!Lp26_CoTheDoiThuTu(handle, viTriCu, duaLen))
                {
                    statusGeneral.Text = handle == handleAnhNenTuDong
                        ? "● Ảnh nền ngoại tuyến được giữ dưới các lớp dữ liệu."
                        : "● Lớp đã ở giới hạn sắp xếp.";
                    return;
                }

                int viTriMoi = viTriCu + (duaLen ? 1 : -1);

                string ten = LayTenHienThiLop(
                    banDoMapWinGIS.get_LayerName(handle));

                // Không nạp lại dữ liệu và không thay đổi trạng thái bật/tắt.
                dangThaoTacBanDo = true;

                try
                {
                    if (duaLen)
                        ketNoiMapWinGIS.DuaLopLenMotBac(handle);
                    else
                        ketNoiMapWinGIS.DuaLopXuongMotBac(handle);

                    if (ketNoiMapWinGIS.LayViTriLop(handle) != viTriMoi)
                        throw new InvalidOperationException(
                            "Thứ tự lớp chưa được cập nhật.");
                }
                finally
                {
                    dangThaoTacBanDo = false;
                }

                for (int i = 0; i < danhSachLop.Items.Count; i++)
                {
                    LopMapItem muc = danhSachLop.Items[i] as LopMapItem;

                    if (muc == null || muc.Handle != handle)
                        continue;

                    danhSachLop.SelectedIndex = i;
                    break;
                }

                lp26HandleCanCuonDen = handle;

                GhiNhatKyBanDo(
                    "LAYER_REORDER | handle=" + handle +
                    " | from=" + viTriCu +
                    " | to=" + viTriMoi);

                statusGeneral.Text =
                    "● Đã đưa " + ten +
                    (duaLen ? " lên một bậc." : " xuống một bậc.");

                try
                {
                    ketNoiMapWinGIS.VeLaiBanDo();
                }
                catch (Exception ex)
                {
                    GhiNhatKyBanDo("LAYER_REORDER_REDRAW_FAILED | " + ex);
                    statusGeneral.Text =
                        "● Đã đổi thứ tự lớp; chưa vẽ lại được bản đồ: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                GhiNhatKyBanDo(
                    "LAYER_REORDER_FAILED | handle=" + handle + " | " + ex);

                statusGeneral.Text =
                    "● Không đổi được thứ tự lớp: " + ex.Message;
            }
            finally
            {
                HenLamMoiDanhSachLop();
                HenCapNhatChuGiai();
            }
        }
    }
}