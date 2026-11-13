namespace ManagerGISAcademy.GUI.Forms
{
    partial class FormMain
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlBrand;
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlSidebarTop;
        private System.Windows.Forms.Panel pnlSidebarBottom;
        private System.Windows.Forms.Panel pnlWorkspace;
        private System.Windows.Forms.Panel pnlWorkspaceHeadText;
        private System.Windows.Forms.Panel pnlMapCard;
        private System.Windows.Forms.Panel pnlMapCardHeader;
        private System.Windows.Forms.Panel pnlMapCanvas;
        private System.Windows.Forms.Panel pnlMapEmptyState;
        private System.Windows.Forms.Panel pnlMapTools;
        private System.Windows.Forms.Panel pnlPhanHeHost;
        private System.Windows.Forms.Panel pnlRightCard;
        private System.Windows.Forms.Panel pnlRightHeader;
        private System.Windows.Forms.Panel pnlRightBody;
        private System.Windows.Forms.Panel pnlRightBottom;

        private System.Windows.Forms.TableLayoutPanel tblHeaderActions;
        private System.Windows.Forms.TableLayoutPanel tblWorkspaceHeading;
        private System.Windows.Forms.TableLayoutPanel tblContent;

        private System.Windows.Forms.FlowLayoutPanel flowNavigation;

        private System.Windows.Forms.Label lblBrandLogo;
        private System.Windows.Forms.Label lblBrandTitle;
        private System.Windows.Forms.Label lblBrandSubtitle;
        private System.Windows.Forms.Label lblHeaderDbStatus;
        private System.Windows.Forms.Label lblSidebarTitle;
        private System.Windows.Forms.Label lblSidebarHint;
        private System.Windows.Forms.Label lblSidebarVersion;
        private System.Windows.Forms.Label lblPageTitle;
        private System.Windows.Forms.Label lblPageSubtitle;
        private System.Windows.Forms.Label lblStageBadge;
        private System.Windows.Forms.Label lblMapHeading;
        private System.Windows.Forms.Label lblMapHeaderTag;
        private System.Windows.Forms.Label lblEmptyIcon;
        private System.Windows.Forms.Label lblEmptyTitle;
        private System.Windows.Forms.Label lblEmptyDescription;
        private System.Windows.Forms.Label lblMapFooter;
        private System.Windows.Forms.Label lblRightTitle;
        private System.Windows.Forms.Label lblRightTag;
        private System.Windows.Forms.Label lblLayersCaption;
        private System.Windows.Forms.Label lblLayersHint;
        private System.Windows.Forms.Label lblConnectTitle;
        private System.Windows.Forms.Label lblConnectState;
        private System.Windows.Forms.Label lblConnectHint;

        private System.Windows.Forms.Button btnQuickSearch;
        private System.Windows.Forms.Button btnAddUnit;
        private System.Windows.Forms.Button btnNavOverview;
        private System.Windows.Forms.Button btnNavMap;
        private System.Windows.Forms.Button btnNavSearch;
        private System.Windows.Forms.Button btnNavManage;
        private System.Windows.Forms.Button btnNavAnalysis;
        private System.Windows.Forms.Button btnNavReports;
        private System.Windows.Forms.Button btnNavValidate;
        private System.Windows.Forms.Button btnZoomIn;
        private System.Windows.Forms.Button btnZoomOut;
        private System.Windows.Forms.Button btnFullExtent;
        private System.Windows.Forms.Button btnMapLayers;

        private System.Windows.Forms.TextBox txtQuickSearch;

        private System.Windows.Forms.CheckBox chkBorders;
        private System.Windows.Forms.CheckBox chkWater;
        private System.Windows.Forms.CheckBox chkRoads;
        private System.Windows.Forms.CheckBox chkUnits;

        private System.Windows.Forms.StatusStrip statusStrip;

        private System.Windows.Forms.ToolStripStatusLabel statusGeneral;
        private System.Windows.Forms.ToolStripStatusLabel statusSpacer;
        private System.Windows.Forms.ToolStripStatusLabel statusCoordinates;
        private System.Windows.Forms.ToolStripStatusLabel statusProjection;

        private System.Windows.Forms.ToolTip toolTip;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.tblHeaderActions = new System.Windows.Forms.TableLayoutPanel();
            this.txtQuickSearch = new System.Windows.Forms.TextBox();
            this.btnQuickSearch = new System.Windows.Forms.Button();
            this.lblHeaderDbStatus = new System.Windows.Forms.Label();
            this.btnAddUnit = new System.Windows.Forms.Button();
            this.pnlBrand = new System.Windows.Forms.Panel();
            this.lblBrandLogo = new System.Windows.Forms.Label();
            this.lblBrandTitle = new System.Windows.Forms.Label();
            this.lblBrandSubtitle = new System.Windows.Forms.Label();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.flowNavigation = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNavOverview = new System.Windows.Forms.Button();
            this.btnNavMap = new System.Windows.Forms.Button();
            this.btnNavSearch = new System.Windows.Forms.Button();
            this.btnNavManage = new System.Windows.Forms.Button();
            this.btnNavAnalysis = new System.Windows.Forms.Button();
            this.btnNavReports = new System.Windows.Forms.Button();
            this.btnNavValidate = new System.Windows.Forms.Button();
            this.pnlSidebarBottom = new System.Windows.Forms.Panel();
            this.lblSidebarVersion = new System.Windows.Forms.Label();
            this.pnlSidebarTop = new System.Windows.Forms.Panel();
            this.lblSidebarTitle = new System.Windows.Forms.Label();
            this.lblSidebarHint = new System.Windows.Forms.Label();
            this.pnlWorkspace = new System.Windows.Forms.Panel();
            this.tblContent = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMapCard = new System.Windows.Forms.Panel();
            this.pnlMapCanvas = new System.Windows.Forms.Panel();
            this.pnlMapEmptyState = new System.Windows.Forms.Panel();
            this.lblEmptyIcon = new System.Windows.Forms.Label();
            this.lblEmptyTitle = new System.Windows.Forms.Label();
            this.lblEmptyDescription = new System.Windows.Forms.Label();
            this.pnlMapTools = new System.Windows.Forms.Panel();
            this.btnZoomIn = new System.Windows.Forms.Button();
            this.btnZoomOut = new System.Windows.Forms.Button();
            this.btnFullExtent = new System.Windows.Forms.Button();
            this.btnMapLayers = new System.Windows.Forms.Button();
            this.lblMapFooter = new System.Windows.Forms.Label();
            this.pnlMapCardHeader = new System.Windows.Forms.Panel();
            this.lblMapHeading = new System.Windows.Forms.Label();
            this.lblMapHeaderTag = new System.Windows.Forms.Label();
            this.pnlRightCard = new System.Windows.Forms.Panel();
            this.pnlRightBody = new System.Windows.Forms.Panel();
            this.lblLayersCaption = new System.Windows.Forms.Label();
            this.chkBorders = new System.Windows.Forms.CheckBox();
            this.chkWater = new System.Windows.Forms.CheckBox();
            this.chkRoads = new System.Windows.Forms.CheckBox();
            this.chkUnits = new System.Windows.Forms.CheckBox();
            this.lblLayersHint = new System.Windows.Forms.Label();
            this.pnlRightBottom = new System.Windows.Forms.Panel();
            this.lblConnectTitle = new System.Windows.Forms.Label();
            this.lblConnectState = new System.Windows.Forms.Label();
            this.lblConnectHint = new System.Windows.Forms.Label();
            this.pnlRightHeader = new System.Windows.Forms.Panel();
            this.lblRightTitle = new System.Windows.Forms.Label();
            this.lblRightTag = new System.Windows.Forms.Label();
            this.tblWorkspaceHeading = new System.Windows.Forms.TableLayoutPanel();
            this.pnlWorkspaceHeadText = new System.Windows.Forms.Panel();
            this.lblPageTitle = new System.Windows.Forms.Label();
            this.lblPageSubtitle = new System.Windows.Forms.Label();
            this.lblStageBadge = new System.Windows.Forms.Label();
            this.pnlPhanHeHost = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusGeneral = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusSpacer = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCoordinates = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusProjection = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pnlHeader.SuspendLayout();
            this.tblHeaderActions.SuspendLayout();
            this.pnlBrand.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.flowNavigation.SuspendLayout();
            this.pnlSidebarBottom.SuspendLayout();
            this.pnlSidebarTop.SuspendLayout();
            this.pnlWorkspace.SuspendLayout();
            this.tblContent.SuspendLayout();
            this.pnlMapCard.SuspendLayout();
            this.pnlMapCanvas.SuspendLayout();
            this.pnlMapEmptyState.SuspendLayout();
            this.pnlMapTools.SuspendLayout();
            this.pnlMapCardHeader.SuspendLayout();
            this.pnlRightCard.SuspendLayout();
            this.pnlRightBody.SuspendLayout();
            this.pnlRightBottom.SuspendLayout();
            this.pnlRightHeader.SuspendLayout();
            this.tblWorkspaceHeading.SuspendLayout();
            this.pnlWorkspaceHeadText.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.pnlHeader.Controls.Add(this.tblHeaderActions);
            this.pnlHeader.Controls.Add(this.pnlBrand);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1364, 72);
            this.pnlHeader.TabIndex = 2;
            // 
            // tblHeaderActions
            // 
            this.tblHeaderActions.ColumnCount = 4;
            this.tblHeaderActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHeaderActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tblHeaderActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 195F));
            this.tblHeaderActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tblHeaderActions.Controls.Add(this.txtQuickSearch, 0, 0);
            this.tblHeaderActions.Controls.Add(this.btnQuickSearch, 1, 0);
            this.tblHeaderActions.Controls.Add(this.lblHeaderDbStatus, 2, 0);
            this.tblHeaderActions.Controls.Add(this.btnAddUnit, 3, 0);
            this.tblHeaderActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblHeaderActions.Location = new System.Drawing.Point(365, 0);
            this.tblHeaderActions.Name = "tblHeaderActions";
            this.tblHeaderActions.Padding = new System.Windows.Forms.Padding(7, 0, 14, 0);
            this.tblHeaderActions.RowCount = 1;
            this.tblHeaderActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHeaderActions.Size = new System.Drawing.Size(999, 72);
            this.tblHeaderActions.TabIndex = 0;
            // 
            // txtQuickSearch
            // 
            this.txtQuickSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.txtQuickSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQuickSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtQuickSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtQuickSearch.ForeColor = System.Drawing.Color.White;
            this.txtQuickSearch.Location = new System.Drawing.Point(11, 22);
            this.txtQuickSearch.Margin = new System.Windows.Forms.Padding(4, 22, 8, 20);
            this.txtQuickSearch.Name = "txtQuickSearch";
            this.txtQuickSearch.Size = new System.Drawing.Size(524, 25);
            this.txtQuickSearch.TabIndex = 0;
            // 
            // btnQuickSearch
            // 
            this.btnQuickSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.btnQuickSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuickSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQuickSearch.FlatAppearance.BorderSize = 0;
            this.btnQuickSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickSearch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnQuickSearch.ForeColor = System.Drawing.Color.White;
            this.btnQuickSearch.Location = new System.Drawing.Point(544, 17);
            this.btnQuickSearch.Margin = new System.Windows.Forms.Padding(1, 17, 9, 17);
            this.btnQuickSearch.Name = "btnQuickSearch";
            this.btnQuickSearch.Size = new System.Drawing.Size(82, 38);
            this.btnQuickSearch.TabIndex = 1;
            this.btnQuickSearch.Text = "TÌM";
            this.btnQuickSearch.UseVisualStyleBackColor = false;
            // 
            // lblHeaderDbStatus
            // 
            this.lblHeaderDbStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblHeaderDbStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeaderDbStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblHeaderDbStatus.Location = new System.Drawing.Point(638, 0);
            this.lblHeaderDbStatus.Name = "lblHeaderDbStatus";
            this.lblHeaderDbStatus.Size = new System.Drawing.Size(189, 72);
            this.lblHeaderDbStatus.TabIndex = 2;
            this.lblHeaderDbStatus.Text = "● PostGIS: Đang kiểm tra";
            this.lblHeaderDbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.btnAddUnit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAddUnit.FlatAppearance.BorderSize = 0;
            this.btnAddUnit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnAddUnit.ForeColor = System.Drawing.Color.White;
            this.btnAddUnit.Location = new System.Drawing.Point(835, 17);
            this.btnAddUnit.Margin = new System.Windows.Forms.Padding(5, 17, 0, 17);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(150, 38);
            this.btnAddUnit.TabIndex = 3;
            this.btnAddUnit.Text = "+ THÊM ĐƠN VỊ";
            this.btnAddUnit.UseVisualStyleBackColor = false;
            // 
            // pnlBrand
            // 
            this.pnlBrand.Controls.Add(this.lblBrandLogo);
            this.pnlBrand.Controls.Add(this.lblBrandTitle);
            this.pnlBrand.Controls.Add(this.lblBrandSubtitle);
            this.pnlBrand.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlBrand.Location = new System.Drawing.Point(0, 0);
            this.pnlBrand.Name = "pnlBrand";
            this.pnlBrand.Size = new System.Drawing.Size(365, 72);
            this.pnlBrand.TabIndex = 1;
            // 
            // lblBrandLogo
            // 
            this.lblBrandLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.lblBrandLogo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblBrandLogo.ForeColor = System.Drawing.Color.White;
            this.lblBrandLogo.Location = new System.Drawing.Point(18, 14);
            this.lblBrandLogo.Name = "lblBrandLogo";
            this.lblBrandLogo.Size = new System.Drawing.Size(44, 44);
            this.lblBrandLogo.TabIndex = 0;
            this.lblBrandLogo.Text = "GIS";
            this.lblBrandLogo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBrandTitle
            // 
            this.lblBrandTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold);
            this.lblBrandTitle.ForeColor = System.Drawing.Color.White;
            this.lblBrandTitle.Location = new System.Drawing.Point(74, 13);
            this.lblBrandTitle.Name = "lblBrandTitle";
            this.lblBrandTitle.Size = new System.Drawing.Size(278, 26);
            this.lblBrandTitle.TabIndex = 1;
            this.lblBrandTitle.Text = "HỆ THỐNG QUẢN LÝ GIS";
            // 
            // lblBrandSubtitle
            // 
            this.lblBrandSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblBrandSubtitle.Location = new System.Drawing.Point(75, 41);
            this.lblBrandSubtitle.Name = "lblBrandSubtitle";
            this.lblBrandSubtitle.Size = new System.Drawing.Size(279, 24);
            this.lblBrandSubtitle.TabIndex = 2;
            this.lblBrandSubtitle.Text = "Bộ Tư lệnh Thủ đô Hà Nội · Dữ liệu tác chiến";
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.pnlSidebar.Controls.Add(this.flowNavigation);
            this.pnlSidebar.Controls.Add(this.pnlSidebarBottom);
            this.pnlSidebar.Controls.Add(this.pnlSidebarTop);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 72);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(240, 655);
            this.pnlSidebar.TabIndex = 1;
            // 
            // flowNavigation
            // 
            this.flowNavigation.AutoScroll = true;
            this.flowNavigation.Controls.Add(this.btnNavOverview);
            this.flowNavigation.Controls.Add(this.btnNavMap);
            this.flowNavigation.Controls.Add(this.btnNavSearch);
            this.flowNavigation.Controls.Add(this.btnNavManage);
            this.flowNavigation.Controls.Add(this.btnNavAnalysis);
            this.flowNavigation.Controls.Add(this.btnNavReports);
            this.flowNavigation.Controls.Add(this.btnNavValidate);
            this.flowNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowNavigation.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowNavigation.Location = new System.Drawing.Point(0, 84);
            this.flowNavigation.Name = "flowNavigation";
            this.flowNavigation.Padding = new System.Windows.Forms.Padding(12, 7, 8, 5);
            this.flowNavigation.Size = new System.Drawing.Size(240, 506);
            this.flowNavigation.TabIndex = 0;
            this.flowNavigation.WrapContents = false;
            // 
            // btnNavOverview
            // 
            this.btnNavOverview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavOverview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavOverview.FlatAppearance.BorderSize = 0;
            this.btnNavOverview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavOverview.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavOverview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavOverview.Location = new System.Drawing.Point(12, 7);
            this.btnNavOverview.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavOverview.Name = "btnNavOverview";
            this.btnNavOverview.Size = new System.Drawing.Size(210, 44);
            this.btnNavOverview.TabIndex = 0;
            this.btnNavOverview.Text = "  Tổng quan";
            this.btnNavOverview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavOverview.UseVisualStyleBackColor = false;
            // 
            // btnNavMap
            // 
            this.btnNavMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavMap.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavMap.FlatAppearance.BorderSize = 0;
            this.btnNavMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavMap.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavMap.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavMap.Location = new System.Drawing.Point(12, 57);
            this.btnNavMap.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavMap.Name = "btnNavMap";
            this.btnNavMap.Size = new System.Drawing.Size(210, 44);
            this.btnNavMap.TabIndex = 1;
            this.btnNavMap.Text = "  Bản đồ GIS";
            this.btnNavMap.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavMap.UseVisualStyleBackColor = false;
            // 
            // btnNavSearch
            // 
            this.btnNavSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavSearch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavSearch.FlatAppearance.BorderSize = 0;
            this.btnNavSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavSearch.Location = new System.Drawing.Point(12, 107);
            this.btnNavSearch.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavSearch.Name = "btnNavSearch";
            this.btnNavSearch.Size = new System.Drawing.Size(210, 44);
            this.btnNavSearch.TabIndex = 2;
            this.btnNavSearch.Text = "  Tra cứu đơn vị";
            this.btnNavSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavSearch.UseVisualStyleBackColor = false;
            // 
            // btnNavManage
            // 
            this.btnNavManage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavManage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavManage.FlatAppearance.BorderSize = 0;
            this.btnNavManage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavManage.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavManage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavManage.Location = new System.Drawing.Point(12, 157);
            this.btnNavManage.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavManage.Name = "btnNavManage";
            this.btnNavManage.Size = new System.Drawing.Size(210, 44);
            this.btnNavManage.TabIndex = 3;
            this.btnNavManage.Text = "  Quản lý dữ liệu";
            this.btnNavManage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavManage.UseVisualStyleBackColor = false;
            // 
            // btnNavAnalysis
            // 
            this.btnNavAnalysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavAnalysis.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavAnalysis.FlatAppearance.BorderSize = 0;
            this.btnNavAnalysis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavAnalysis.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavAnalysis.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavAnalysis.Location = new System.Drawing.Point(12, 207);
            this.btnNavAnalysis.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavAnalysis.Name = "btnNavAnalysis";
            this.btnNavAnalysis.Size = new System.Drawing.Size(210, 44);
            this.btnNavAnalysis.TabIndex = 4;
            this.btnNavAnalysis.Text = "  Phân tích không gian";
            this.btnNavAnalysis.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavAnalysis.UseVisualStyleBackColor = false;
            // 
            // btnNavReports
            // 
            this.btnNavReports.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavReports.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavReports.FlatAppearance.BorderSize = 0;
            this.btnNavReports.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavReports.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavReports.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavReports.Location = new System.Drawing.Point(12, 257);
            this.btnNavReports.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavReports.Name = "btnNavReports";
            this.btnNavReports.Size = new System.Drawing.Size(210, 44);
            this.btnNavReports.TabIndex = 5;
            this.btnNavReports.Text = "  Thống kê, báo cáo";
            this.btnNavReports.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavReports.UseVisualStyleBackColor = false;
            // 
            // btnNavValidate
            // 
            this.btnNavValidate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.btnNavValidate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavValidate.FlatAppearance.BorderSize = 0;
            this.btnNavValidate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavValidate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNavValidate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.btnNavValidate.Location = new System.Drawing.Point(12, 307);
            this.btnNavValidate.Margin = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.btnNavValidate.Name = "btnNavValidate";
            this.btnNavValidate.Size = new System.Drawing.Size(210, 44);
            this.btnNavValidate.TabIndex = 6;
            this.btnNavValidate.Text = "  Kiểm tra dữ liệu";
            this.btnNavValidate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavValidate.UseVisualStyleBackColor = false;
            // 
            // pnlSidebarBottom
            // 
            this.pnlSidebarBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(13)))), ((int)(((byte)(23)))));
            this.pnlSidebarBottom.Controls.Add(this.lblSidebarVersion);
            this.pnlSidebarBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlSidebarBottom.Location = new System.Drawing.Point(0, 590);
            this.pnlSidebarBottom.Name = "pnlSidebarBottom";
            this.pnlSidebarBottom.Size = new System.Drawing.Size(240, 65);
            this.pnlSidebarBottom.TabIndex = 1;
            // 
            // lblSidebarVersion
            // 
            this.lblSidebarVersion.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSidebarVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSidebarVersion.Location = new System.Drawing.Point(0, 0);
            this.lblSidebarVersion.Name = "lblSidebarVersion";
            this.lblSidebarVersion.Padding = new System.Windows.Forms.Padding(17, 0, 0, 0);
            this.lblSidebarVersion.Size = new System.Drawing.Size(240, 65);
            this.lblSidebarVersion.TabIndex = 0;
            this.lblSidebarVersion.Text = "HỆ QUẢN TRỊ CSDL KHÔNG GIAN\r\nPhiên bản 4.6 (Dark Military)";
            this.lblSidebarVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlSidebarTop
            // 
            this.pnlSidebarTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.pnlSidebarTop.Controls.Add(this.lblSidebarTitle);
            this.pnlSidebarTop.Controls.Add(this.lblSidebarHint);
            this.pnlSidebarTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSidebarTop.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebarTop.Name = "pnlSidebarTop";
            this.pnlSidebarTop.Size = new System.Drawing.Size(240, 84);
            this.pnlSidebarTop.TabIndex = 2;
            // 
            // lblSidebarTitle
            // 
            this.lblSidebarTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSidebarTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblSidebarTitle.Location = new System.Drawing.Point(18, 22);
            this.lblSidebarTitle.Name = "lblSidebarTitle";
            this.lblSidebarTitle.Size = new System.Drawing.Size(205, 23);
            this.lblSidebarTitle.TabIndex = 0;
            this.lblSidebarTitle.Text = "DANH MỤC CHỨC NĂNG";
            // 
            // lblSidebarHint
            // 
            this.lblSidebarHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblSidebarHint.Location = new System.Drawing.Point(18, 48);
            this.lblSidebarHint.Name = "lblSidebarHint";
            this.lblSidebarHint.Size = new System.Drawing.Size(205, 23);
            this.lblSidebarHint.TabIndex = 1;
            this.lblSidebarHint.Text = "Điều hướng hệ thống tác chiến";
            // 
            // pnlWorkspace
            // 
            this.pnlWorkspace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(11)))), ((int)(((byte)(18)))));
            this.pnlWorkspace.Controls.Add(this.tblContent);
            this.pnlWorkspace.Controls.Add(this.tblWorkspaceHeading);
            this.pnlWorkspace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWorkspace.Location = new System.Drawing.Point(240, 72);
            this.pnlWorkspace.Name = "pnlWorkspace";
            this.pnlWorkspace.Size = new System.Drawing.Size(1124, 655);
            this.pnlWorkspace.TabIndex = 0;
            // 
            // tblContent
            // 
            this.tblContent.ColumnCount = 2;
            this.tblContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71F));
            this.tblContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29F));
            this.tblContent.Controls.Add(this.pnlMapCard, 0, 0);
            this.tblContent.Controls.Add(this.pnlRightCard, 1, 0);
            this.tblContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblContent.Location = new System.Drawing.Point(0, 84);
            this.tblContent.Name = "tblContent";
            this.tblContent.Padding = new System.Windows.Forms.Padding(18, 0, 18, 14);
            this.tblContent.RowCount = 1;
            this.tblContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 557F));
            this.tblContent.Size = new System.Drawing.Size(1124, 571);
            this.tblContent.TabIndex = 0;
            // 
            // pnlMapCard
            // 
            this.pnlMapCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(20)))), ((int)(((byte)(34)))));
            this.pnlMapCard.Controls.Add(this.pnlMapCanvas);
            this.pnlMapCard.Controls.Add(this.lblMapFooter);
            this.pnlMapCard.Controls.Add(this.pnlMapCardHeader);
            this.pnlMapCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMapCard.Location = new System.Drawing.Point(18, 0);
            this.pnlMapCard.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.pnlMapCard.Name = "pnlMapCard";
            this.pnlMapCard.Size = new System.Drawing.Size(762, 557);
            this.pnlMapCard.TabIndex = 0;
            // 
            // pnlMapCanvas
            // 
            this.pnlMapCanvas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(8)))), ((int)(((byte)(13)))), ((int)(((byte)(22)))));
            this.pnlMapCanvas.Controls.Add(this.pnlMapEmptyState);
            this.pnlMapCanvas.Controls.Add(this.pnlMapTools);
            this.pnlMapCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMapCanvas.Location = new System.Drawing.Point(0, 52);
            this.pnlMapCanvas.Name = "pnlMapCanvas";
            this.pnlMapCanvas.Size = new System.Drawing.Size(762, 473);
            this.pnlMapCanvas.TabIndex = 0;
            // 
            // pnlMapEmptyState
            // 
            this.pnlMapEmptyState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.pnlMapEmptyState.Controls.Add(this.lblEmptyIcon);
            this.pnlMapEmptyState.Controls.Add(this.lblEmptyTitle);
            this.pnlMapEmptyState.Controls.Add(this.lblEmptyDescription);
            this.pnlMapEmptyState.Location = new System.Drawing.Point(0, 0);
            this.pnlMapEmptyState.Name = "pnlMapEmptyState";
            this.pnlMapEmptyState.Size = new System.Drawing.Size(425, 185);
            this.pnlMapEmptyState.TabIndex = 0;
            // 
            // lblEmptyIcon
            // 
            this.lblEmptyIcon.Font = new System.Drawing.Font("Segoe UI", 25F, System.Drawing.FontStyle.Bold);
            this.lblEmptyIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblEmptyIcon.Location = new System.Drawing.Point(8, 12);
            this.lblEmptyIcon.Name = "lblEmptyIcon";
            this.lblEmptyIcon.Size = new System.Drawing.Size(409, 60);
            this.lblEmptyIcon.TabIndex = 0;
            this.lblEmptyIcon.Text = "GIS";
            this.lblEmptyIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyTitle
            // 
            this.lblEmptyTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblEmptyTitle.ForeColor = System.Drawing.Color.White;
            this.lblEmptyTitle.Location = new System.Drawing.Point(9, 82);
            this.lblEmptyTitle.Name = "lblEmptyTitle";
            this.lblEmptyTitle.Size = new System.Drawing.Size(407, 30);
            this.lblEmptyTitle.TabIndex = 1;
            this.lblEmptyTitle.Text = "SẴN SÀNG TÍCH HỢP MAPWINGIS";
            this.lblEmptyTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblEmptyDescription
            // 
            this.lblEmptyDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(188)))), ((int)(((byte)(201)))));
            this.lblEmptyDescription.Location = new System.Drawing.Point(26, 123);
            this.lblEmptyDescription.Name = "lblEmptyDescription";
            this.lblEmptyDescription.Size = new System.Drawing.Size(373, 48);
            this.lblEmptyDescription.TabIndex = 2;
            this.lblEmptyDescription.Text = "Khu vực hiển thị bản đồ raster vệ tinh và vector đơn vị.";
            this.lblEmptyDescription.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlMapTools
            // 
            this.pnlMapTools.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(9)))), ((int)(((byte)(17)))), ((int)(((byte)(28)))));
            this.pnlMapTools.Controls.Add(this.btnZoomIn);
            this.pnlMapTools.Controls.Add(this.btnZoomOut);
            this.pnlMapTools.Controls.Add(this.btnFullExtent);
            this.pnlMapTools.Controls.Add(this.btnMapLayers);
            this.pnlMapTools.Location = new System.Drawing.Point(16, 16);
            this.pnlMapTools.Name = "pnlMapTools";
            this.pnlMapTools.Size = new System.Drawing.Size(50, 212);
            this.pnlMapTools.TabIndex = 1;
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.btnZoomIn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomIn.FlatAppearance.BorderSize = 0;
            this.btnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomIn.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnZoomIn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnZoomIn.Location = new System.Drawing.Point(3, 5);
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(44, 46);
            this.btnZoomIn.TabIndex = 0;
            this.btnZoomIn.Text = "+";
            this.toolTip.SetToolTip(this.btnZoomIn, "Phóng to");
            this.btnZoomIn.UseVisualStyleBackColor = false;
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.btnZoomOut.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnZoomOut.FlatAppearance.BorderSize = 0;
            this.btnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnZoomOut.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.btnZoomOut.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnZoomOut.Location = new System.Drawing.Point(3, 55);
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(44, 46);
            this.btnZoomOut.TabIndex = 1;
            this.btnZoomOut.Text = "−";
            this.toolTip.SetToolTip(this.btnZoomOut, "Thu nhỏ");
            this.btnZoomOut.UseVisualStyleBackColor = false;
            // 
            // btnFullExtent
            // 
            this.btnFullExtent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.btnFullExtent.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFullExtent.FlatAppearance.BorderSize = 0;
            this.btnFullExtent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullExtent.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnFullExtent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnFullExtent.Location = new System.Drawing.Point(3, 105);
            this.btnFullExtent.Name = "btnFullExtent";
            this.btnFullExtent.Size = new System.Drawing.Size(44, 46);
            this.btnFullExtent.TabIndex = 2;
            this.btnFullExtent.Text = "□";
            this.toolTip.SetToolTip(this.btnFullExtent, "Toàn cảnh");
            this.btnFullExtent.UseVisualStyleBackColor = false;
            // 
            // btnMapLayers
            // 
            this.btnMapLayers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(27)))), ((int)(((byte)(43)))));
            this.btnMapLayers.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMapLayers.FlatAppearance.BorderSize = 0;
            this.btnMapLayers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMapLayers.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.btnMapLayers.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.btnMapLayers.Location = new System.Drawing.Point(3, 155);
            this.btnMapLayers.Name = "btnMapLayers";
            this.btnMapLayers.Size = new System.Drawing.Size(44, 46);
            this.btnMapLayers.TabIndex = 3;
            this.btnMapLayers.Text = "≡";
            this.toolTip.SetToolTip(this.btnMapLayers, "Hiện / ẩn bảng lớp");
            this.btnMapLayers.UseVisualStyleBackColor = false;
            // 
            // lblMapFooter
            // 
            this.lblMapFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblMapFooter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblMapFooter.Location = new System.Drawing.Point(0, 525);
            this.lblMapFooter.Name = "lblMapFooter";
            this.lblMapFooter.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblMapFooter.Size = new System.Drawing.Size(762, 32);
            this.lblMapFooter.TabIndex = 1;
            this.lblMapFooter.Text = "Hà Nội 2m Satellite Tiled Grid · GDAL Raster Cache hoạt động";
            this.lblMapFooter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlMapCardHeader
            // 
            this.pnlMapCardHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(22)))), ((int)(((byte)(36)))));
            this.pnlMapCardHeader.Controls.Add(this.lblMapHeading);
            this.pnlMapCardHeader.Controls.Add(this.lblMapHeaderTag);
            this.pnlMapCardHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMapCardHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlMapCardHeader.Name = "pnlMapCardHeader";
            this.pnlMapCardHeader.Size = new System.Drawing.Size(762, 52);
            this.pnlMapCardHeader.TabIndex = 2;
            // 
            // lblMapHeading
            // 
            this.lblMapHeading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMapHeading.ForeColor = System.Drawing.Color.White;
            this.lblMapHeading.Location = new System.Drawing.Point(18, 15);
            this.lblMapHeading.Name = "lblMapHeading";
            this.lblMapHeading.Size = new System.Drawing.Size(265, 24);
            this.lblMapHeading.TabIndex = 0;
            this.lblMapHeading.Text = "KHÔNG GIAN BẢN ĐỒ CHIẾN THUẬT";
            // 
            // lblMapHeaderTag
            // 
            this.lblMapHeaderTag.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblMapHeaderTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblMapHeaderTag.Location = new System.Drawing.Point(290, 17);
            this.lblMapHeaderTag.Name = "lblMapHeaderTag";
            this.lblMapHeaderTag.Size = new System.Drawing.Size(230, 22);
            this.lblMapHeaderTag.TabIndex = 1;
            this.lblMapHeaderTag.Text = "● MapWinGIS đã khởi tạo";
            // 
            // pnlRightCard
            // 
            this.pnlRightCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(20)))), ((int)(((byte)(34)))));
            this.pnlRightCard.Controls.Add(this.pnlRightBody);
            this.pnlRightCard.Controls.Add(this.pnlRightBottom);
            this.pnlRightCard.Controls.Add(this.pnlRightHeader);
            this.pnlRightCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightCard.Location = new System.Drawing.Point(793, 3);
            this.pnlRightCard.Name = "pnlRightCard";
            this.pnlRightCard.Size = new System.Drawing.Size(310, 551);
            this.pnlRightCard.TabIndex = 1;
            // 
            // pnlRightBody
            // 
            this.pnlRightBody.AutoScroll = true;
            this.pnlRightBody.Controls.Add(this.lblLayersCaption);
            this.pnlRightBody.Controls.Add(this.chkBorders);
            this.pnlRightBody.Controls.Add(this.chkWater);
            this.pnlRightBody.Controls.Add(this.chkRoads);
            this.pnlRightBody.Controls.Add(this.chkUnits);
            this.pnlRightBody.Controls.Add(this.lblLayersHint);
            this.pnlRightBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRightBody.Location = new System.Drawing.Point(0, 52);
            this.pnlRightBody.Name = "pnlRightBody";
            this.pnlRightBody.Size = new System.Drawing.Size(310, 359);
            this.pnlRightBody.TabIndex = 0;
            // 
            // lblLayersCaption
            // 
            this.lblLayersCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLayersCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblLayersCaption.Location = new System.Drawing.Point(17, 18);
            this.lblLayersCaption.Name = "lblLayersCaption";
            this.lblLayersCaption.Size = new System.Drawing.Size(260, 25);
            this.lblLayersCaption.TabIndex = 0;
            this.lblLayersCaption.Text = "DANH SÁCH LỚP";
            // 
            // chkBorders
            // 
            this.chkBorders.Enabled = false;
            this.chkBorders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkBorders.Location = new System.Drawing.Point(18, 55);
            this.chkBorders.Name = "chkBorders";
            this.chkBorders.Size = new System.Drawing.Size(263, 28);
            this.chkBorders.TabIndex = 1;
            this.chkBorders.Text = "Ranh giới hành chính";
            // 
            // chkWater
            // 
            this.chkWater.Enabled = false;
            this.chkWater.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkWater.Location = new System.Drawing.Point(18, 92);
            this.chkWater.Name = "chkWater";
            this.chkWater.Size = new System.Drawing.Size(263, 28);
            this.chkWater.TabIndex = 2;
            this.chkWater.Text = "Sông, hồ";
            // 
            // chkRoads
            // 
            this.chkRoads.Enabled = false;
            this.chkRoads.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkRoads.Location = new System.Drawing.Point(18, 129);
            this.chkRoads.Name = "chkRoads";
            this.chkRoads.Size = new System.Drawing.Size(263, 28);
            this.chkRoads.TabIndex = 3;
            this.chkRoads.Text = "Đường giao thông";
            // 
            // chkUnits
            // 
            this.chkUnits.Enabled = false;
            this.chkUnits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.chkUnits.Location = new System.Drawing.Point(18, 166);
            this.chkUnits.Name = "chkUnits";
            this.chkUnits.Size = new System.Drawing.Size(263, 28);
            this.chkUnits.TabIndex = 4;
            this.chkUnits.Text = "Điểm đơn vị quân đội";
            // 
            // lblLayersHint
            // 
            this.lblLayersHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblLayersHint.Location = new System.Drawing.Point(18, 205);
            this.lblLayersHint.Name = "lblLayersHint";
            this.lblLayersHint.Size = new System.Drawing.Size(268, 50);
            this.lblLayersHint.TabIndex = 5;
            this.lblLayersHint.Text = "Tích vào ô để bật hoặc tắt lớp trên bản đồ. Chọn LỚP + để nạp thêm.";
            // 
            // pnlRightBottom
            // 
            this.pnlRightBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(13)))), ((int)(((byte)(23)))));
            this.pnlRightBottom.Controls.Add(this.lblConnectTitle);
            this.pnlRightBottom.Controls.Add(this.lblConnectState);
            this.pnlRightBottom.Controls.Add(this.lblConnectHint);
            this.pnlRightBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlRightBottom.Location = new System.Drawing.Point(0, 411);
            this.pnlRightBottom.Name = "pnlRightBottom";
            this.pnlRightBottom.Size = new System.Drawing.Size(310, 140);
            this.pnlRightBottom.TabIndex = 1;
            // 
            // lblConnectTitle
            // 
            this.lblConnectTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConnectTitle.ForeColor = System.Drawing.Color.White;
            this.lblConnectTitle.Location = new System.Drawing.Point(17, 15);
            this.lblConnectTitle.Name = "lblConnectTitle";
            this.lblConnectTitle.Size = new System.Drawing.Size(270, 24);
            this.lblConnectTitle.TabIndex = 0;
            this.lblConnectTitle.Text = "TRẠNG THÁI HỆ THỐNG";
            // 
            // lblConnectState
            // 
            this.lblConnectState.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblConnectState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblConnectState.Location = new System.Drawing.Point(17, 48);
            this.lblConnectState.Name = "lblConnectState";
            this.lblConnectState.Size = new System.Drawing.Size(270, 24);
            this.lblConnectState.TabIndex = 1;
            this.lblConnectState.Text = "● PostGIS: Đang kiểm tra";
            // 
            // lblConnectHint
            // 
            this.lblConnectHint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblConnectHint.Location = new System.Drawing.Point(17, 78);
            this.lblConnectHint.Name = "lblConnectHint";
            this.lblConnectHint.Size = new System.Drawing.Size(270, 48);
            this.lblConnectHint.TabIndex = 2;
            this.lblConnectHint.Text = "PostgreSQL/PostGIS · EPSG:32648";
            // 
            // pnlRightHeader
            // 
            this.pnlRightHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(22)))), ((int)(((byte)(36)))));
            this.pnlRightHeader.Controls.Add(this.lblRightTitle);
            this.pnlRightHeader.Controls.Add(this.lblRightTag);
            this.pnlRightHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRightHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlRightHeader.Name = "pnlRightHeader";
            this.pnlRightHeader.Size = new System.Drawing.Size(310, 52);
            this.pnlRightHeader.TabIndex = 2;
            // 
            // lblRightTitle
            // 
            this.lblRightTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblRightTitle.ForeColor = System.Drawing.Color.White;
            this.lblRightTitle.Location = new System.Drawing.Point(17, 15);
            this.lblRightTitle.Name = "lblRightTitle";
            this.lblRightTitle.Size = new System.Drawing.Size(155, 24);
            this.lblRightTitle.TabIndex = 0;
            this.lblRightTitle.Text = "LỚP DỮ LIỆU";
            // 
            // lblRightTag
            // 
            this.lblRightTag.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblRightTag.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblRightTag.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblRightTag.Location = new System.Drawing.Point(226, 0);
            this.lblRightTag.Name = "lblRightTag";
            this.lblRightTag.Size = new System.Drawing.Size(84, 52);
            this.lblRightTag.TabIndex = 1;
            this.lblRightTag.Text = "5 lớp";
            this.lblRightTag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tblWorkspaceHeading
            // 
            this.tblWorkspaceHeading.ColumnCount = 2;
            this.tblWorkspaceHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblWorkspaceHeading.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 220F));
            this.tblWorkspaceHeading.Controls.Add(this.pnlWorkspaceHeadText, 0, 0);
            this.tblWorkspaceHeading.Controls.Add(this.lblStageBadge, 1, 0);
            this.tblWorkspaceHeading.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblWorkspaceHeading.Location = new System.Drawing.Point(0, 0);
            this.tblWorkspaceHeading.Name = "tblWorkspaceHeading";
            this.tblWorkspaceHeading.RowCount = 1;
            this.tblWorkspaceHeading.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 84F));
            this.tblWorkspaceHeading.Size = new System.Drawing.Size(1124, 84);
            this.tblWorkspaceHeading.TabIndex = 1;
            // 
            // pnlWorkspaceHeadText
            // 
            this.pnlWorkspaceHeadText.Controls.Add(this.lblPageTitle);
            this.pnlWorkspaceHeadText.Controls.Add(this.lblPageSubtitle);
            this.pnlWorkspaceHeadText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlWorkspaceHeadText.Location = new System.Drawing.Point(3, 3);
            this.pnlWorkspaceHeadText.Name = "pnlWorkspaceHeadText";
            this.pnlWorkspaceHeadText.Size = new System.Drawing.Size(898, 78);
            this.pnlWorkspaceHeadText.TabIndex = 0;
            // 
            // lblPageTitle
            // 
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.ForeColor = System.Drawing.Color.White;
            this.lblPageTitle.Location = new System.Drawing.Point(23, 14);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(590, 35);
            this.lblPageTitle.TabIndex = 0;
            this.lblPageTitle.Text = "Bản đồ GIS";
            // 
            // lblPageSubtitle
            // 
            this.lblPageSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblPageSubtitle.Location = new System.Drawing.Point(26, 52);
            this.lblPageSubtitle.Name = "lblPageSubtitle";
            this.lblPageSubtitle.Size = new System.Drawing.Size(590, 24);
            this.lblPageSubtitle.TabIndex = 1;
            this.lblPageSubtitle.Text = "Không gian hiển thị bản đồ, quản lý lớp và thao tác không gian.";
            // 
            // lblStageBadge
            // 
            this.lblStageBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(52)))), ((int)(((byte)(40)))));
            this.lblStageBadge.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStageBadge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblStageBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.lblStageBadge.Location = new System.Drawing.Point(914, 24);
            this.lblStageBadge.Margin = new System.Windows.Forms.Padding(10, 24, 22, 24);
            this.lblStageBadge.Name = "lblStageBadge";
            this.lblStageBadge.Size = new System.Drawing.Size(188, 36);
            this.lblStageBadge.TabIndex = 1;
            this.lblStageBadge.Text = "HỆ THỐNG GIS QUÂN SỰ";
            this.lblStageBadge.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlPhanHeHost
            // 
            this.pnlPhanHeHost.AutoScroll = true;
            this.pnlPhanHeHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(11)))), ((int)(((byte)(18)))));
            this.pnlPhanHeHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPhanHeHost.Location = new System.Drawing.Point(0, 0);
            this.pnlPhanHeHost.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPhanHeHost.Name = "pnlPhanHeHost";
            this.pnlPhanHeHost.Size = new System.Drawing.Size(200, 100);
            this.pnlPhanHeHost.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(13)))), ((int)(((byte)(23)))));
            this.statusStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(203)))), ((int)(((byte)(217)))));
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusGeneral,
            this.statusSpacer,
            this.statusCoordinates,
            this.statusProjection});
            this.statusStrip.Location = new System.Drawing.Point(0, 727);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1364, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 3;
            // 
            // statusGeneral
            // 
            this.statusGeneral.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.statusGeneral.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(211)))), ((int)(((byte)(153)))));
            this.statusGeneral.Name = "statusGeneral";
            this.statusGeneral.Size = new System.Drawing.Size(174, 17);
            this.statusGeneral.Text = "● Đang khởi tạo hệ thống GIS.";
            // 
            // statusSpacer
            // 
            this.statusSpacer.Name = "statusSpacer";
            this.statusSpacer.Size = new System.Drawing.Size(792, 17);
            this.statusSpacer.Spring = true;
            // 
            // statusCoordinates
            // 
            this.statusCoordinates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(189)))), ((int)(((byte)(248)))));
            this.statusCoordinates.Name = "statusCoordinates";
            this.statusCoordinates.Size = new System.Drawing.Size(189, 17);
            this.statusCoordinates.Text = "WGS84: 105.854444°E; 21.028511°N";
            // 
            // statusProjection
            // 
            this.statusProjection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.statusProjection.Name = "statusProjection";
            this.statusProjection.Size = new System.Drawing.Size(194, 17);
            this.statusProjection.Text = "CRS: EPSG:32648 (Hà Nội UTM 48N)";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(7)))), ((int)(((byte)(11)))), ((int)(((byte)(18)))));
            this.ClientSize = new System.Drawing.Size(1364, 749);
            this.Controls.Add(this.pnlWorkspace);
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.statusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1100, 690);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hệ thống quản lý đơn vị quân đội trên địa bàn Hà Nội";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load_1);
            this.pnlHeader.ResumeLayout(false);
            this.tblHeaderActions.ResumeLayout(false);
            this.tblHeaderActions.PerformLayout();
            this.pnlBrand.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.flowNavigation.ResumeLayout(false);
            this.pnlSidebarBottom.ResumeLayout(false);
            this.pnlSidebarTop.ResumeLayout(false);
            this.pnlWorkspace.ResumeLayout(false);
            this.tblContent.ResumeLayout(false);
            this.pnlMapCard.ResumeLayout(false);
            this.pnlMapCanvas.ResumeLayout(false);
            this.pnlMapEmptyState.ResumeLayout(false);
            this.pnlMapTools.ResumeLayout(false);
            this.pnlMapCardHeader.ResumeLayout(false);
            this.pnlRightCard.ResumeLayout(false);
            this.pnlRightBody.ResumeLayout(false);
            this.pnlRightBottom.ResumeLayout(false);
            this.pnlRightHeader.ResumeLayout(false);
            this.tblWorkspaceHeading.ResumeLayout(false);
            this.pnlWorkspaceHeadText.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}