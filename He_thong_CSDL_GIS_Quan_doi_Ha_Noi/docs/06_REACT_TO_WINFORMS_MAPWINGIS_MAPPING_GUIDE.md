# 06. HƯỚNG DẪN ÁNH XẠ GIAO DIỆN REACT SANG C# WINFORMS + MAPWINGIS
## Chuyển Đổi Từng Khung Giao Diện, Nút Bấm & Sự Kiện Sang Windows Forms
### Môn học: Hệ Quản Trị Cơ Sở Dữ Liệu Không Gian (Spatial DBMS)

---

## 1. NGUYÊN TẮC ÁNH XẠ TỔNG THỂ (LAYOUT ARCHITECTURE MAPPING)

Giao diện Web React hiện tại được thiết kế theo phong cách **Trung tâm chỉ huy tác chiến (Military Dark Command Center)** gồm 4 vùng chính:
1. **Header (Đỉnh màn hình)** $\rightarrow$ Ánh xạ sang WinForms: `Panel` (Dock = Top) chứa Logo, Tiêu đề, Ô tra cứu nhanh và Nút Thêm đơn vị.
2. **Sidebar (Bên trái)** $\rightarrow$ Ánh xạ sang WinForms: `Panel` (Dock = Left) chứa 7 nút chuyển đổi phân hệ chức năng.
3. **Khu vực trung tâm (Bản đồ & Màn hình nghiệp vụ)** $\rightarrow$ Ánh xạ sang WinForms: Điều khiển `AxMapWinGIS.AxMap` (`axMap1`, Dock = Fill) kết hợp các `Panel` / `UserControl` hiển thị đè hoặc ẩn/hiện theo Tab.
4. **Status Bar (Đáy màn hình)** $\rightarrow$ Ánh xạ sang WinForms: `StatusStrip` (Dock = Bottom) hiển thị tọa độ thời gian thực $X, Y$ mét (VN-2000).

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ [HEADER PANEL] Logo MTA | HỆ THỐNG CSDL GIS QUẢN LÝ ĐƠN VỊ HÀ NỘI | [Tìm] [PostGIS] [+] │
├──────────────┬──────────────────────────────────────────────────────────┬──────────────┤
│ [SIDEBAR]    │ [MAPWINGIS axMap1 - DOCK = FILL]                         │ [DRAWER PNL] │
│              │                                                          │              │
│ 1. Tổng quan │  ┌─[TOOLBAR]─┐                                           │ Chi tiết hồ  │
│ 2. Bản đồ GIS│  │ [+] ZoomIn│                                           │ sơ đơn vị    │
│ 3. Tra cứu   │  │ [-] ZoomOu│         ĐA GIÁC ĐỊA GIỚI HÀ NỘI           │ tác chiến:   │
│ 4. Quản lý   │  │ [⊡] Extent│      (30 Quận, Huyện, Thị xã)             │ - Binh chủng │
│ 5. Phân tích │  │ [i] Ident │                                           │ - Trực ban   │
│ 6. Thống kê  │  │ [📏] Đo cự│   ● Điểm đơn vị đóng quân (Marker Icon)   │ - Tọa độ VN2k│
│ 7. Ktra Topo │  │ [◎] Buffer│                                           │ - SSCĐ Cấp 1 │
│              │  │ [≣] Lớp   │   ◯ Vòng tròn vùng đệm phòng thủ (Buffer) │              │
│              │  └───────────┘                                           │ [Vùng đệm]   │
│              │                                                          │ [Sửa đơn vị] │
├──────────────┴──────────────────────────────────────────────────────────┴──────────────┤
│ [STATUS STRIP] Sẵn sàng | TP. Hà Nội | X: 586.230 m, Y: 2.326.450 m | CRS: EPSG:32648  │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. BẢNG ÁNH XẠ CHI TIẾT TỪNG COMPONENT SANG WINFORMS CONTROLS

| Thành phần React Web | Control WinForms tương ứng | Thuộc tính & Thiết lập quan trọng | Sự kiện (Event) tương ứng trong C# |
| :--- | :--- | :--- | :--- |
| **`Header.tsx`** | `Panel` (`pnlHeader`) | `Dock = Top`, `Height = 55`, `BackColor = Color.FromArgb(11, 19, 36)` | `pnlHeader_Paint` |
| Logo Học viện KTQS | `PictureBox` (`picLogo`) | `SizeMode = PictureBoxSizeMode.Zoom`, tải ảnh `MTA_Logo.png` | `picLogo_Click` |
| Tiêu đề hệ thống | `Label` (`lblTitle`) | `Font = Bold 11pt`, `ForeColor = Color.White` | - |
| Ô tra cứu nhanh | `TextBox` (`txtQuickSearch`) | `BackColor = Color.FromArgb(9, 15, 29)`, `ForeColor = Color.White` | `txtQuickSearch_KeyDown` (Enter) |
| Nút "Tìm" | `Button` (`btnQuickSearch`) | `FlatStyle = Flat`, `BackColor = Color.FromArgb(5, 150, 105)` | `btnQuickSearch_Click` |
| Nút "PostGIS Sẵn sàng" | `Button` (`btnPostgisStatus`) | `Text = "• PostGIS | Sẵn sàng"`, `ForeColor = Color.Lime` | `btnPostgisStatus_Click` (Mở FormPostgis) |
| Nút "+ Thêm đơn vị" | `Button` (`btnThemDonVi`) | `BackColor = Color.FromArgb(5, 150, 105)`, `ForeColor = White` | `btnThemDonVi_Click` (Mở FormThemSua) |
| **`Sidebar.tsx`** | `Panel` (`pnlSidebar`) | `Dock = Left`, `Width = 200`, `BackColor = Color.FromArgb(10, 16, 29)` | - |
| 7 Nút chức năng menu | 7 `Button` (`btnTabOverview`, `btnTabMap`, ...) | `FlatStyle = Flat`, `TextAlign = MiddleLeft`, `Height = 40` | `btnTab_Click` (Kích hoạt Tab tương ứng) |
| **`GisMap.tsx`** | `AxMapWinGIS.AxMap` (`axMap1`) | `Dock = Fill`, `TileProvider = OpenStreetMap` hoặc `BingMaps` | `axMap1_MouseDown`, `MouseMove`, `SelectBoxFinal` |
| Toolbar GIS bên trái map | `Panel` hoặc `ToolStrip` (`pnlMapToolbar`) | Đặt đè góc trên bên trái `axMap1`, `BackColor = Semi-transparent` | Các nút: `ZoomIn`, `ZoomOut`, `Measure`, `Buffer` |
| Thước tỷ lệ Scale Bar | Thuộc tính tích hợp của MapWinGIS | `axMap1.ShowScaleBar = true; axMap1.ScaleBarUnits = tkScaleBarUnits.Kilometers;` | Tự động tính tỷ lệ theo độ phóng to |
| **`UnitDetailDrawer.tsx`** | `Panel` (`pnlChiTietDonVi`) | `Dock = Right`, `Width = 340`, `Visible = false` (Mở trượt khi click đơn vị) | `btnDongChiTiet_Click`, `btnTaoVungDemNhanh_Click` |
| **`StatusBar.tsx`** | `StatusStrip` (`statusStrip1`) | `Dock = Bottom`, gồm 4 `ToolStripStatusLabel` | Cập nhật trong `axMap1_MouseMove` |

---

## 3. CODE C# WINFORMS TRIỂN KHAI TỪNG KHỐI CHỨC NĂNG

### 3.1. Header: Tra Cứu Nhanh & Mở Hộp Thoại Thêm Mới
```csharp
// GUI/FormMain.cs
private void btnQuickSearch_Click(object sender, EventArgs e)
{
    string keyword = txtQuickSearch.Text.Trim();
    if (string.IsNullOrEmpty(keyword)) return;

    // Chuyển sang chế độ tìm kiếm và nạp kết quả
    HienThiTabTraCuu();
    formTraCuu.ThucHienTimKiem(keyword);
}

// Bắt phím Enter trên ô TextBox
private void txtQuickSearch_KeyDown(object sender, KeyEventArgs e)
{
    if (e.KeyCode == Keys.Enter)
    {
        btnQuickSearch.PerformClick();
        e.SuppressKeyPress = true; // Tránh tiếng 'ding' của Windows
    }
}

// Nút "+ Thêm đơn vị"
private void btnThemDonVi_Click(object sender, EventArgs e)
{
    using (var formThem = new FormThemSuaDonVi())
    {
        if (formThem.ShowDialog() == DialogResult.OK)
        {
            // Tải lại lớp điểm trên bản đồ MapWinGIS để thấy đơn vị mới nạp
            LoadLopDonViLenMap();
            MessageBox.Show("Đã lưu đơn vị mới vào CSDL PostGIS thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
```

---

### 3.2. Sidebar: Chuyển Đổi Qua Lại Giữa 7 Phân Hệ Chức Năng
Trong WinForms, để chuyển đổi mượt mà giữa các Tab mà không mở quá nhiều cửa sổ con, ta sử dụng **1 Panel chứa MapWinGIS** và các **UserControl** tương ứng cho từng màn hình:

```csharp
// GUI/FormMain.cs
private Button _currentActiveBtn = null;

private void SetActiveTab(Button clickedBtn, UserControl targetControl)
{
    // 1. Đổi màu nút được chọn (xanh lục quân sự giống web)
    if (_currentActiveBtn != null)
    {
        _currentActiveBtn.BackColor = Color.FromArgb(10, 16, 29); // Màu tối thường
        _currentActiveBtn.ForeColor = Color.Gainsboro;
    }
    _currentActiveBtn = clickedBtn;
    _currentActiveBtn.BackColor = Color.FromArgb(5, 150, 105); // Màu xanh lục Active
    _currentActiveBtn.ForeColor = Color.White;

    // 2. Nếu chọn "Bản đồ GIS", ta đưa axMap1 lên trước
    if (clickedBtn == btnTabBanDo)
    {
        pnlMapContainer.BringToFront();
        pnlMapContainer.Visible = true;
    }
    else
    {
        // Hiển thị UserControl tương ứng (Tổng quan, Tra cứu, Quản lý dữ liệu...)
        pnlContentContainer.Controls.Clear();
        targetControl.Dock = DockStyle.Fill;
        pnlContentContainer.Controls.Add(targetControl);
        pnlContentContainer.BringToFront();
    }
}

private void btnTabBanDo_Click(object sender, EventArgs e)
{
    SetActiveTab(btnTabBanDo, null);
}

private void btnTabTraCuu_Click(object sender, EventArgs e)
{
    SetActiveTab(btnTabTraCuu, ucTraCuuDonVi);
}

private void btnTabPhanTich_Click(object sender, EventArgs e)
{
    SetActiveTab(btnTabPhanTich, ucPhanTichKhongGian);
}
```

---

### 3.3. Thanh Công Cụ Trên Bản Đồ (Map Toolbar & Toolstrip Actions)
Thanh công cụ bên trái bản đồ (`GisMap.tsx`) được cài đặt bằng các nút bấm gọi trực tiếp API của MapWinGIS:

```csharp
// 1. Phóng to (Zoom In)
private void btnZoomIn_Click(object sender, EventArgs e)
{
    axMap1.ZoomIn();
}

// 2. Thu nhỏ (Zoom Out)
private void btnZoomOut_Click(object sender, EventArgs e)
{
    axMap1.ZoomOut();
}

// 3. Toàn cảnh lãnh thổ Hà Nội (Full Extent)
private void btnFullExtent_Click(object sender, EventArgs e)
{
    // Zoom đến tọa độ trung tâm và bao trọn Hà Nội
    axMap1.ZoomToKnownExtents(tkKnownExtents.keVietnam);
    // Hoặc Zoom to Layer ranh giới Hà Nội:
    // axMap1.ZoomToLayer(_layerRanhGioiHanoiHandle);
}

// 4. Bật thước đo cự ly tác chiến (Measure Tool)
private void btnMeasureDistance_Click(object sender, EventArgs e)
{
    axMap1.CursorMode = tkCursorMode.cmMeasure;
    axMap1.Measuring.MeasuringType = tkMeasuringType.mtDistance;
    MessageBox.Show("Nhấp các điểm trên bản đồ để đo khoảng cách cơ động. Nhấp đúp để kết thúc.", "Thước đo khoảng cách");
}

// 5. Bật chế độ xem thông tin (Identify Tool)
private void btnIdentify_Click(object sender, EventArgs e)
{
    axMap1.CursorMode = tkCursorMode.cmIdentify;
}

// 6. Xóa phép đo & Làm sạch vùng chọn (Reset / Clear)
private void btnClearMap_Click(object sender, EventArgs e)
{
    // Xóa layer Buffer tạm thời nếu có
    if (_layerBufferHandle != -1)
    {
        axMap1.RemoveLayer(_layerBufferHandle);
        _layerBufferHandle = -1;
    }
    // Tắt thước đo
    axMap1.Measuring.Clear();
    axMap1.CursorMode = tkCursorMode.cmPan; // Trở về chế độ kéo rê bản đồ
    pnlChiTietDonVi.Visible = false;        // Đóng panel chi tiết
    axMap1.Redraw();
}
```

---

### 3.4. Status Bar: Hiển Thị Tọa Độ Thời Gian Thực $X, Y$ (VN-2000) và WGS-84
Trong React, ta bắt sự kiện `map.on('mousemove')`. Trong C# MapWinGIS, ta bắt sự kiện `axMap1_MouseMove`:

```csharp
private void axMap1_MouseMoveEvent(object sender, _DMapEvents_MouseMoveEvent e)
{
    // 1. Lấy tọa độ kinh độ/vĩ độ (WGS84) từ pixel chuột
    double lon = 0, lat = 0;
    axMap1.PixelToProj(e.x, e.y, ref lon, ref lat);

    // 2. Sử dụng lớp CoordinateConverter chuyển đổi sang tọa độ phẳng VN-2000 (m)
    double utmX = 0, utmY = 0;
    CoordinateConverter.Wgs84ToUtm48N(lat, lon, out utmX, out utmY);

    // 3. Cập nhật lên StatusStrip dưới đáy form
    lblCursorCoord.Text = string.Format("X: {0:N0} m, Y: {1:N0} m ({2:F4}°N, {3:F4}°E)", 
                                        utmX, utmY, lat, lon);
    lblCRS.Text = "CRS: EPSG:32648 (VN-2000 UTM 48N) | mét";
}
```

---

### 3.5. Bảng Trượt Xem Chi Tiết Đơn Vị (`UnitDetailDrawer.tsx` $\rightarrow$ `pnlChiTietDonVi`)
Khi người dùng click chọn 1 Marker trên bản đồ hoặc chọn 1 dòng trên `DataGridView`:

```csharp
// Khi click vào điểm trên MapWinGIS
private void axMap1_ShapeIdentified(object sender, _DMapEvents_ShapeIdentifiedEvent e)
{
    if (e.layerHandle == _layerDonViHandle)
    {
        var sf = axMap1.get_Shapefile(_layerDonViHandle);
        string maDV = sf.get_CellValue(0, e.shapeIndex).ToString();

        // Lấy thông tin từ BLL và hiển thị lên Panel trượt bên phải
        HienThiChiTietDonVi(maDV);
    }
}

private void HienThiChiTietDonVi(string maDonVi)
{
    var bll = new DonViQuanDoiBLL();
    DonViQuanDoi dv = bll.LayChiTietDonVi(maDonVi);
    if (dv == null) return;

    // Gán dữ liệu lên các Label của Panel Chi Tiết
    lblDetailMaDV.Text = dv.MaDonVi;
    lblDetailTenDV.Text = dv.TenDonVi;
    lblDetailBinhChung.Text = dv.QuanBinhChung;
    lblDetailDiaChi.Text = dv.DiaChi;
    lblDetailChiHuy.Text = dv.ChiHuyTruong;
    lblDetailSDT.Text = dv.SoDienThoai;
    lblDetailSSCD.Text = dv.TrangThaiSSCD;
    lblDetailCoord.Text = string.Format("X: {0:N0} m | Y: {1:N0} m", dv.XVn2000, dv.YVn2000);

    // Mở Panel bên phải
    pnlChiTietDonVi.Visible = true;
    pnlChiTietDonVi.BringToFront();

    // Nút kích hoạt nhanh Vùng đệm 10km quanh đơn vị này
    btnTaoVungDem10km.Tag = dv.MaDonVi;
}
```

---

### 3.6. Phân Tích Vùng Đệm (Buffer Analysis - Gọi Stored Procedure PostGIS)
Ánh xạ nút "Tạo vùng đệm phòng thủ":

```csharp
private int _layerBufferHandle = -1;

private void btnTaoVungDem_Click(object sender, EventArgs e)
{
    string maDV = btnTaoVungDem10km.Tag.ToString();
    double banKinhMet = 10000; // 10 km

    var dal = new SpatialAnalysisDAL();
    // 1. Gọi hàm PostGIS lấy chuỗi WKT đa giác đường tròn
    string bufferWkt = dal.LayWktVungDem(maDV, banKinhMet);
    
    // 2. Lấy danh sách các đơn vị khác nằm trong tầm hỏa lực
    List<DonViTrongVungDemDTO> listBiAnhHuong = dal.LayDonViTrongVungDem(maDV, banKinhMet);

    // 3. Vẽ Polygon lên MapWinGIS
    var sfBuffer = new MapWinGIS.Shapefile();
    sfBuffer.CreateNewWithShapeID("", ShpfileType.SHP_POLYGON);
    sfBuffer.GeoProjection.SetWgs84();

    var shape = new MapWinGIS.Shape();
    shape.ImportFromWKT(bufferWkt);
    sfBuffer.EditAddShape(shape);

    // Cấu hình màu sắc vùng đệm (Vàng cam bán trong suốt 30%)
    sfBuffer.DefaultDrawingOptions.FillColor = (uint)ColorTranslator.ToOle(Color.FromArgb(245, 158, 11));
    sfBuffer.DefaultDrawingOptions.FillTransparency = 70; // Trong suốt
    sfBuffer.DefaultDrawingOptions.LineColor = (uint)ColorTranslator.ToOle(Color.DarkOrange);
    sfBuffer.DefaultDrawingOptions.LineWidth = 2;

    if (_layerBufferHandle != -1) axMap1.RemoveLayer(_layerBufferHandle);
    _layerBufferHandle = axMap1.AddLayer(sfBuffer, true);
    axMap1.Redraw();

    // 4. Hiển thị danh sách đơn vị bị bao phủ lên DataGridView kết quả
    dgvKetQuaBuffer.DataSource = listBiAnhHuong;
    MessageBox.Show(string.Format("Tìm thấy {0} đơn vị quân đội nằm trong vùng đệm 10km!", listBiAnhHuong.Count), "Kết quả phân tích GIS");
}
```

---

## 4. TỔNG KẾT BƯỚC ĐI DÀNH CHO BẠN KHI BẮT ĐẦU CODE C#
1. **Bước 1**: Mở Visual Studio $\rightarrow$ Tạo Project Windows Forms App $\rightarrow$ Kéo thả `Panel` Top, Left, Fill, Bottom y hệt cấu trúc ở Mục 1.
2. **Bước 2**: Thêm `AxMapWinGIS.AxMap` vào Panel Fill.
3. **Bước 3**: Copy các đoạn mã C# ở Mục 3 vào các file tương ứng trong thư mục `GUI/`, `DAL/`, `GIS/`.
4. **Bước 4**: Chạy thử phần mềm, kết nối vào PostgreSQL qua tệp `App.config` để trải nghiệm hệ thống Desktop GIS hoàn chỉnh cho bài tập lớn.
