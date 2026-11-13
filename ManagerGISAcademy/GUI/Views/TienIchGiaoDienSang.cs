using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ManagerGISAcademy.GUI.Views
{
    // Bo dieu khien dung chung cho cac phan he WinForms.
    // Giai doan 5.1: giao dien Military GIS Modern Light.
    internal static class TienIchGiaoDienSang
    {
        public static readonly Color Background =
            Color.FromArgb(244, 248, 252);

        public static readonly Color Ink =
            Color.FromArgb(22, 49, 67);

        public static readonly Color Muted =
            Color.FromArgb(102, 122, 139);

        public static readonly Color Border =
            Color.FromArgb(215, 227, 237);

        public static readonly Color Teal =
            Color.FromArgb(0, 162, 133);

        public static readonly Color Blue =
            Color.FromArgb(39, 126, 205);

        public static readonly Color Purple =
            Color.FromArgb(109, 90, 205);

        public static readonly Color Amber =
            Color.FromArgb(209, 127, 35);

        public static readonly CultureInfo Vi =
            CultureInfo.GetCultureInfo("vi-VN");

        // =========================================
        // NHAN VAN BAN
        // =========================================

        public static Label Text(
            string text,
            float size = 9F,
            bool bold = false,
            Color? color = null)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                AutoEllipsis = true,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = color ?? Ink,
                Font = new Font(
                    "Segoe UI",
                    size,
                    bold ? FontStyle.Bold : FontStyle.Regular)
            };
        }

        // =========================================
        // NUT BAM
        // =========================================

        public static Button Button(
            string text,
            Color color,
            int width = 145)
        {
            Button b = new Button
            {
                Text = text,
                Width = width,
                Height = 39,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 10, 2),
                Font = new Font(
                    "Segoe UI Semibold",
                    9F,
                    FontStyle.Bold)
            };

            b.FlatAppearance.BorderSize = 0;

            b.FlatAppearance.MouseOverBackColor =
                ControlPaint.Light(color, 0.12F);

            b.FlatAppearance.MouseDownBackColor =
                ControlPaint.Dark(color, 0.08F);

            b.Resize += (s, e) => Round(b, 9);

            b.EnabledChanged += (s, e) =>
                b.Invalidate();

            Round(b, 9);

            return b;
        }

        public static FlowLayoutPanel Buttons(
            params Control[] controls)
        {
            FlowLayoutPanel f = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection =
                    FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White,
                Padding = new Padding(3, 8, 0, 3),
                Margin = Padding.Empty
            };

            f.Controls.AddRange(controls);

            return f;
        }

        // =========================================
        // THE NOI DUNG
        // =========================================

        public static NativeCard Card(
            Color? background = null)
        {
            return new NativeCard
            {
                Dock = DockStyle.Fill,
                BackColor =
                    background ?? Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(16),
                LineColor = Border
            };
        }

        // =========================================
        // TIEU DE PHAN MUC
        // =========================================

        public static Control SectionTitle(
            string text,
            string note = null)
        {
            TableLayoutPanel p = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Background,
                ColumnCount = 2,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            p.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 62F));

            p.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 38F));

            p.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            Label heading = Text(
                "▏ " + text,
                10F,
                true);

            heading.Margin = Padding.Empty;

            p.Controls.Add(heading, 0, 0);

            Label subtitle = Text(
                note ?? "",
                8.2F,
                false,
                Muted);

            subtitle.TextAlign =
                ContentAlignment.MiddleRight;

            subtitle.Margin = Padding.Empty;

            p.Controls.Add(subtitle, 1, 0);

            return p;
        }

        // =========================================
        // BANNER TIEU DE
        // =========================================

        public static Control Banner(
            string heading,
            string subtitle)
        {
            NativeBanner panel = new NativeBanner
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(23, 13, 14, 11)
            };

            TableLayoutPanel inside =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent,
                    ColumnCount = 1,
                    RowCount = 2
                };

            inside.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            inside.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 60F));

            inside.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 40F));

            inside.Controls.Add(
                Text(
                    heading,
                    16.5F,
                    true,
                    Color.White),
                0, 0);

            inside.Controls.Add(
                Text(
                    subtitle,
                    9F,
                    false,
                    Color.FromArgb(235, 249, 250)),
                0, 1);

            panel.Controls.Add(inside);

            return panel;
        }

        // =========================================
        // NHAP LIEU
        // =========================================

        public static TextBox Input()
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 7, 7, 7),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = Ink,
                Font = new Font("Segoe UI", 9.4F)
            };
        }

        public static ComboBox Select()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(4, 7, 7, 7),
                DropDownStyle =
                    ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.4F)
            };
        }

        // =========================================
        // BO GOC TEXTBOX VA COMBOBOX
        // =========================================

        internal static void ApplyRoundedFields(
            Control root)
        {
            if (root == null || root.IsDisposed)
                return;

            List<Control> inputs =
                new List<Control>();

            CollectFields(root, inputs);

            foreach (Control input in inputs)
            {
                WrapField(input);
            }
            PolishControls(root);
        }

        // Format buttons and table borders without changing their events.
        private static readonly HashSet<Button> styledButtons = new HashSet<Button>();

        internal static void PolishControls(Control root)
        {
            if (root == null || root.IsDisposed) return;
            Button button = root as Button;
            if (button != null && !styledButtons.Contains(button))
            {
                styledButtons.Add(button);
                button.Resize += (s, e) => Round(button, 8);
                button.Disposed += (s, e) => styledButtons.Remove(button);
                Round(button, 8);
            }
            DataGridView grid = root as DataGridView;
            if (grid != null)
            {
                grid.BorderStyle = BorderStyle.FixedSingle;
                grid.GridColor = Border;
            }
            foreach (Control child in root.Controls)
                PolishControls(child);
        }
        private static void CollectFields(
                    Control parent,
                    List<Control> inputs)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is TextBox || child is ComboBox || child is NumericUpDown)
                {
                    if (!(child.Parent
                        is NativeRoundedField))
                    {
                        inputs.Add(child);
                    }
                }
                else if (!(child is DataGridView))
                {
                    CollectFields(child, inputs);
                }
            }
        }

        private static void WrapField(
            Control input)
        {
            Control parent = input.Parent;
            // Keep manually positioned inputs attached to their original parent.
            if (!(parent is TableLayoutPanel) && input.Dock == DockStyle.None)
            {
                input.Resize += (s, e) => Round(input, 8);
                Round(input, 8);
                return;
            }

            if (parent == null ||
                parent.IsDisposed ||
                parent is NativeRoundedField)
            {
                return;
            }

            TableLayoutPanel table =
                parent as TableLayoutPanel;

            int column = -1;
            int row = -1;
            int columnSpan = 1;
            int rowSpan = 1;

            if (table != null)
            {
                column = table.GetColumn(input);
                row = table.GetRow(input);

                columnSpan =
                    table.GetColumnSpan(input);

                rowSpan =
                    table.GetRowSpan(input);
            }

            DockStyle dock = input.Dock;
            Rectangle bounds = input.Bounds;
            Padding margin = input.Margin;

            int tabIndex = input.TabIndex;

            int index =
                parent.Controls.GetChildIndex(input);

            NativeRoundedField frame =
                new NativeRoundedField(input)
                {
                    Dock = dock,
                    Bounds = bounds,
                    Margin = new Padding(margin.Left, 1, margin.Right, 1),
                    TabIndex = tabIndex,
                    TabStop = false
                };

            parent.SuspendLayout();

            try
            {
                parent.Controls.Remove(input);

                if (table != null &&
                    column >= 0 &&
                    row >= 0)
                {
                    table.Controls.Add(
                        frame,
                        column,
                        row);

                    table.SetColumnSpan(
                        frame,
                        columnSpan);

                    table.SetRowSpan(
                        frame,
                        rowSpan);
                }
                else
                {
                    parent.Controls.Add(frame);

                    parent.Controls.SetChildIndex(
                        frame,
                        Math.Min(
                            index,
                            parent.Controls.Count - 1));
                }

                frame.AttachInput(input);
            }
            finally
            {
                parent.ResumeLayout(true);
            }
        }
        // =========================================
        // THE CHI SO
        // =========================================

        public static Label Metric(
            TableLayoutPanel parent,
            int column,
            string title,
            string note,
            Color accent,
            Color back)
        {
            NativeCard card = Card(back);

            card.LineColor =
                Color.FromArgb(115, accent);

            card.Margin = new Padding(
                column == 0 ? 0 : 6,
                0,
                column == parent.ColumnCount - 1
                    ? 0 : 6,
                10);

            TableLayoutPanel inner =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = back,
                    ColumnCount = 1,
                    RowCount = 3
                };

            inner.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            inner.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 25));

            inner.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            inner.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 20));

            inner.Controls.Add(
                Text(title, 8.4F, true),
                0, 0);

            Label number = Text(
                "—",
                23F,
                true,
                accent);

            inner.Controls.Add(number, 0, 1);

            inner.Controls.Add(
                Text(
                    note,
                    8F,
                    false,
                    Muted),
                0, 2);

            card.Controls.Add(inner);

            parent.Controls.Add(
                card,
                column,
                0);

            return number;
        }

        // =========================================
        // BANG DU LIEU
        // =========================================

        public static DataGridView Table()
        {
            DataGridView d = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,

                ReadOnly = true,

                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,

                RowHeadersVisible = false,

                AutoGenerateColumns = false,
                MultiSelect = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                CellBorderStyle =
                    DataGridViewCellBorderStyle.SingleHorizontal,

                GridColor =
                    Color.FromArgb(227, 236, 243),

                EnableHeadersVisualStyles = false,

                ColumnHeadersHeight = 43,
                ShowCellToolTips = true,

                AutoSizeRowsMode =
                    DataGridViewAutoSizeRowsMode.None,

                ColumnHeadersHeightSizeMode =
                    DataGridViewColumnHeadersHeightSizeMode
                        .DisableResizing
            };

            d.RowTemplate.Height = 38;

            d.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(237, 245, 250);

            d.ColumnHeadersDefaultCellStyle.ForeColor =
                Ink;

            d.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold);

            d.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            d.ColumnHeadersDefaultCellStyle.Padding =
                new Padding(10, 0, 5, 0);

            d.DefaultCellStyle.Font =
                new Font("Segoe UI", 9F);

            d.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleLeft;

            d.DefaultCellStyle.Padding =
                new Padding(10, 0, 6, 0);

            d.DefaultCellStyle.ForeColor = Ink;

            d.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(218, 245, 235);

            d.DefaultCellStyle.SelectionForeColor = Ink;

            d.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 251, 254);

            return d;
        }

        public static void Column(
            DataGridView grid,
            string heading,
            string name,
            float weight)
        {
            grid.Columns.Add(
                new DataGridViewTextBoxColumn
                {
                    HeaderText = heading,
                    DataPropertyName = name,
                    FillWeight = weight,
                    ReadOnly = true,

                    SortMode =
                        DataGridViewColumnSortMode
                            .NotSortable
                });
        }

        // =========================================
        // XUAT CSV UTF-8
        // =========================================

        public static void ExportCsv(
            string fileName,
            IEnumerable<string[]> rows)
        {
            using (SaveFileDialog dialog =
                new SaveFileDialog())
            {
                dialog.Filter =
                    "CSV UTF-8 (*.csv)|*.csv";

                dialog.FileName = fileName;

                if (dialog.ShowDialog() !=
                    DialogResult.OK)
                {
                    return;
                }

                using (StreamWriter writer =
                    new StreamWriter(
                        dialog.FileName,
                        false,
                        new UTF8Encoding(true)))
                {
                    foreach (string[] row in rows)
                    {
                        writer.WriteLine(
                            string.Join(
                                ",",
                                row.Select(EscapeCsv)));
                    }
                }
            }
        }

        private static string EscapeCsv(
            string value)
        {
            return "\"" +
                (value ?? "").Replace("\"", "\"\"") +
                "\"";
        }

        // =========================================
        // HINH HOC BO GOC
        // =========================================

        internal static void Round(
            Control control,
            int r)
        {
            if (control.IsDisposed ||
                control.Width < 3 ||
                control.Height < 3)
            {
                return;
            }

            using (GraphicsPath path =
                Shape(
                    new Rectangle(
                        0,
                        0,
                        control.Width,
                        control.Height),
                    r))
            {
                Region old = control.Region;

                control.Region = new Region(path);

                if (old != null)
                {
                    old.Dispose();
                }
            }
        }

        internal static GraphicsPath Shape(
            Rectangle rectangle,
            int r)
        {
            int d = Math.Max(
                2,
                Math.Min(
                    r * 2,
                    Math.Min(
                        rectangle.Width,
                        rectangle.Height)));

            GraphicsPath path =
                new GraphicsPath();

            path.AddArc(
                rectangle.Left,
                rectangle.Top,
                d,
                d,
                180,
                90);

            path.AddArc(
                rectangle.Right - d,
                rectangle.Top,
                d,
                d,
                270,
                90);

            path.AddArc(
                rectangle.Right - d,
                rectangle.Bottom - d,
                d,
                d,
                0,
                90);

            path.AddArc(
                rectangle.Left,
                rectangle.Bottom - d,
                d,
                d,
                90,
                90);

            path.CloseFigure();

            return path;
        }
    }

    // =========================================
    // TƯƠNG THÍCH MÃ CŨ: NativeLightUi
    // =========================================
    // File đã đổi tên thành TienIchGiaoDienSang.cs và nhiều nơi trong
    // dự án sử dụng TienIchGiaoDienSang. Các partial FormMain và View cũ
    // vẫn có thể gọi NativeLightUi mà không phải sửa hàng loạt file.
    // KHÔNG khai báo lại lớp này ở file nào khác.
    internal static class NativeLightUi
    {
        public static readonly Color Background = TienIchGiaoDienSang.Background;
        public static readonly Color Ink = TienIchGiaoDienSang.Ink;
        public static readonly Color Muted = TienIchGiaoDienSang.Muted;
        public static readonly Color Border = TienIchGiaoDienSang.Border;
        public static readonly Color Teal = TienIchGiaoDienSang.Teal;
        public static readonly Color Blue = TienIchGiaoDienSang.Blue;
        public static readonly Color Purple = TienIchGiaoDienSang.Purple;
        public static readonly Color Amber = TienIchGiaoDienSang.Amber;
        public static readonly CultureInfo Vi = TienIchGiaoDienSang.Vi;

        public static Label Text(string text, float size = 9F,
            bool bold = false, Color? color = null)
        {
            return TienIchGiaoDienSang.Text(text, size, bold, color);
        }

        public static Button Button(string text, Color color, int width = 145)
        {
            return TienIchGiaoDienSang.Button(text, color, width);
        }

        public static FlowLayoutPanel Buttons(params Control[] controls)
        {
            return TienIchGiaoDienSang.Buttons(controls);
        }

        public static NativeCard Card(Color? background = null)
        {
            return TienIchGiaoDienSang.Card(background);
        }

        public static Control SectionTitle(string text, string note = null)
        {
            return TienIchGiaoDienSang.SectionTitle(text, note);
        }

        public static Control Banner(string heading, string subtitle)
        {
            return TienIchGiaoDienSang.Banner(heading, subtitle);
        }

        public static TextBox Input()
        {
            return TienIchGiaoDienSang.Input();
        }

        public static ComboBox Select()
        {
            return TienIchGiaoDienSang.Select();
        }

        internal static void ApplyRoundedFields(Control root)
        {
            TienIchGiaoDienSang.ApplyRoundedFields(root);
        }

        internal static void PolishControls(Control root)
        {
            TienIchGiaoDienSang.PolishControls(root);
        }

        public static Label Metric(TableLayoutPanel parent, int column,
            string title, string note, Color accent, Color back)
        {
            return TienIchGiaoDienSang.Metric(
                parent, column, title, note, accent, back);
        }

        public static DataGridView Table()
        {
            return TienIchGiaoDienSang.Table();
        }

        public static void Column(DataGridView grid, string heading,
            string name, float weight)
        {
            TienIchGiaoDienSang.Column(grid, heading, name, weight);
        }

        public static void ExportCsv(string fileName,
            IEnumerable<string[]> rows)
        {
            TienIchGiaoDienSang.ExportCsv(fileName, rows);
        }

        internal static void Round(Control control, int radius)
        {
            TienIchGiaoDienSang.Round(control, radius);
        }

        internal static GraphicsPath Shape(Rectangle rectangle, int radius)
        {
            return TienIchGiaoDienSang.Shape(rectangle, radius);
        }
    }
    // =========================================
    // KHUNG TRANG NATIVE
    // =========================================

    public class NativePage : UserControl
    {
        private readonly Panel scroll;

        protected readonly TableLayoutPanel Body;

        protected NativePage()
        {
            Dock = DockStyle.Fill;

            BackColor =
                TienIchGiaoDienSang.Background;

            Font = new Font(
                "Segoe UI",
                9F);

            scroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,

                BackColor =
                    TienIchGiaoDienSang.Background,

                Padding =
                    new Padding(18, 15, 18, 16)
            };

            Body = new TableLayoutPanel
            {
                Dock = DockStyle.Top,

                AutoSize = true,

                AutoSizeMode =
                    AutoSizeMode.GrowAndShrink,

                ColumnCount = 1,
                RowCount = 0,

                BackColor =
                    TienIchGiaoDienSang.Background,

                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            Body.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            scroll.Controls.Add(Body);

            Controls.Add(scroll);

            scroll.Resize += (s, e) =>
            {
                int width =
                    scroll.ClientSize.Width -
                    scroll.Padding.Horizontal -
                    SystemInformation.VerticalScrollBarWidth -
                    4;

                if (width > 100)
                {
                    Body.Width =
                        Math.Max(740, width);
                }
            };
        }

        protected void AddRow(
            Control child,
            int height)
        {
            int row = Body.RowCount++;

            Body.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute,
                    height));

            child.Dock = DockStyle.Fill;

            Body.Controls.Add(
                child,
                0,
                row);

            // Giu nguyen control nhap lieu va event.
            TienIchGiaoDienSang.ApplyRoundedFields(child);
        }
    }

    // =========================================
    // KHUNG INPUT BO GOC
    // =========================================

    internal sealed class NativeRoundedField : Panel
    {
        private Control input;
        private bool hovered;

        internal NativeRoundedField(
            Control source)
        {
            Name =
                "nativeRounded_" + source.Name;

            DoubleBuffered = true;
            ResizeRedraw = true;

            BackColor = Color.White;

            Padding =
                new Padding(12, 4, 12, 4);
        }

        internal void AttachInput(
            Control field)
        {
            input = field;

            field.Dock = DockStyle.None;

            field.Margin = Padding.Empty;

            field.BackColor = Color.White;

            field.ForeColor =
                TienIchGiaoDienSang.Ink;

            field.Font =
                new Font("Segoe UI", 9.4F);

            field.TabIndex = 0;

            TextBox text = field as TextBox;

            if (text != null)
            {
                text.BorderStyle =
                    BorderStyle.None;
            }

            ComboBox combo =
                field as ComboBox;

            if (combo != null)
            {
                combo.FlatStyle =
                    FlatStyle.Flat;
            }

            NumericUpDown numeric = field as NumericUpDown;
            if (numeric != null) numeric.BorderStyle = BorderStyle.None;
            Controls.Add(field);

            field.GotFocus +=
                (s, e) => Invalidate();

            field.LostFocus +=
                (s, e) => Invalidate();

            field.MouseEnter += (s, e) =>
            {
                hovered = true;
                Invalidate();
            };

            field.MouseLeave += (s, e) =>
            {
                hovered = false;
                Invalidate();
            };

            field.Resize +=
                (s, e) => Invalidate();

            Resize +=
                (s, e) => LayoutInput();

            SizeChanged +=
                (s, e) => Invalidate();

            Click +=
                (s, e) => field.Focus();

            LayoutInput();
        }

        private void LayoutInput()
        {
            if (input == null ||
                input.IsDisposed ||
                ClientSize.Width < 25)
            {
                return;
            }

            int width = Math.Max(
                20,
                ClientSize.Width -
                Padding.Horizontal);

            int availableHeight = Math.Max(
                8,
                ClientSize.Height -
                Padding.Vertical);

            int height = Math.Min(
                availableHeight,
                Math.Max(
                    20,
                    input.PreferredSize.Height));

            input.SetBounds(
                Padding.Left,
                Math.Max(
                    Padding.Top,
                    (ClientSize.Height - height) / 2),
                width,
                height);
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width < 4 ||
                Height < 4)
            {
                return;
            }

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (GraphicsPath shape =
                TienIchGiaoDienSang.Shape(
                    new Rectangle(
                        0,
                        0,
                        Width - 1,
                        Height - 1),
                    9))
            using (Pen border = new Pen(
                ContainsFocus
                    ? TienIchGiaoDienSang.Teal
                    : hovered
                        ? Color.FromArgb(
                            134, 195, 190)
                        : TienIchGiaoDienSang.Border,
                ContainsFocus ? 1.8F : 1F))
            {
                e.Graphics.DrawPath(
                    border,
                    shape);
            }
        }
    }

    // =========================================
    // THE NOI DUNG BO GOC
    // =========================================

    internal sealed class NativeCard : Panel
    {
        public Color LineColor { get; set; }

        public NativeCard()
        {
            DoubleBuffered = true;

            LineColor =
                TienIchGiaoDienSang.Border;

            Resize += (s, e) =>
                TienIchGiaoDienSang.Round(this, 12);
        }

        protected override void OnPaint(
            PaintEventArgs e)
        {
            base.OnPaint(e);

            if (Width <= 3 ||
                Height <= 3)
            {
                return;
            }

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (GraphicsPath path =
                TienIchGiaoDienSang.Shape(
                    new Rectangle(
                        0,
                        0,
                        Width - 1,
                        Height - 1),
                    12))
            using (Pen p =
                new Pen(LineColor))
            {
                e.Graphics.DrawPath(
                    p,
                    path);
            }
        }
    }

    // =========================================
    // BANNER CHUYEN SAC
    // =========================================

    internal sealed class NativeBanner : Panel
    {
        public NativeBanner()
        {
            DoubleBuffered = true;

            Resize += (s, e) =>
                TienIchGiaoDienSang.Round(this, 13);
        }

        protected override void OnPaintBackground(
            PaintEventArgs e)
        {
            if (Width <= 2 ||
                Height <= 2)
            {
                return;
            }

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    ClientRectangle,
                    Color.FromArgb(0, 164, 140),
                    Color.FromArgb(52, 104, 181),
                    LinearGradientMode.Horizontal))
            {
                brush.InterpolationColors =
                    new ColorBlend
                    {
                        Colors = new[]
                        {
                            Color.FromArgb(
                                0, 164, 140),

                            Color.FromArgb(
                                17, 151, 174),

                            Color.FromArgb(
                                52, 104, 181)
                        },

                        Positions = new[]
                        {
                            0F,
                            .55F,
                            1F
                        }
                    };

                e.Graphics.FillRectangle(
                    brush,
                    ClientRectangle);
            }

            // Hoa tiet ban do / dinh vi o mep phai.
            using (Pen line = new Pen(
                Color.FromArgb(
                    33,
                    Color.White),
                1F))
            {
                float centerX =
                    Width - 35F;

                float centerY =
                    Height * .5F;

                for (int radius = 28;
                    radius <= 132;
                    radius += 26)
                {
                    e.Graphics.DrawEllipse(
                        line,
                        centerX - radius,
                        centerY - radius,
                        radius * 2,
                        radius * 2);
                }

                e.Graphics.DrawLine(
                    line,
                    Width - 265,
                    centerY,
                    Width,
                    centerY);
            }
        }
    }
}