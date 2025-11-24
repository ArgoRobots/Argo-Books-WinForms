using System.ComponentModel;

namespace Argo_Books.UI
{
    /// <summary>
    /// A custom panel that arranges child controls in a centered, flow layout with adjustable spacing.
    /// The layout adapts to control resizing and dynamically centers controls horizontally within each row.
    /// </summary>
    public partial class CenteredFlowLayoutPanel : Panel
    {
        // Properties
        private byte _spacing = 10;

        // Getters and setters
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public byte Spacing
        {
            get => _spacing;
            set
            {
                if (_spacing != value)
                {
                    _spacing = value;
                    Invalidate();  // Redraw the control
                }
            }
        }

        // Init.
        public CenteredFlowLayoutPanel()
        {
            // Enable double buffering to reduce flicker
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            SetFlowLayoutPanel();
        }
        private void SetFlowLayoutPanel()
        {
            HorizontalScroll.Maximum = 0;
            HorizontalScroll.Enabled = false;
            HorizontalScroll.Visible = false;
            AutoScroll = true;
        }

        // Methods
        private void CenterControls()
        {
            if (Controls.Count == 0) { return; }

            int y = AutoScrollPosition.Y;
            int rowHeight = 0;
            int totalControls = Controls.Count;

            int clientWidth = ClientSize.Width;
            if (VerticalScroll.Visible)
            {
                clientWidth -= SystemInformation.VerticalScrollBarWidth;
            }

            int startIndex = 0;

            while (startIndex < totalControls)
            {
                int currentRowWidth = 0;
                int endIndex = startIndex;

                // Calculate how many controls fit in the current row
                while (endIndex < totalControls)
                {
                    Control control = Controls[endIndex];
                    int controlWidth = control.Width + _spacing;

                    if (currentRowWidth + controlWidth - _spacing > clientWidth && currentRowWidth > 0)
                    {
                        break;
                    }

                    currentRowWidth += controlWidth;
                    endIndex++;
                }

                // Center the row
                int x = Math.Max(0, (clientWidth - (currentRowWidth - _spacing)) / 2);

                // Layout the controls in the current row
                for (int i = startIndex; i < endIndex; i++)
                {
                    Control control = Controls[i];
                    control.Location = new Point(x, y);
                    x += control.Width + _spacing;
                    rowHeight = Math.Max(rowHeight, control.Height);
                }

                // Move to the next row
                y += rowHeight + _spacing;
                rowHeight = 0;
                startIndex = endIndex;
            }

            // Prevents the horizontal scrollbar from flashing in some cases
            SetFlowLayoutPanel();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CenterControls();
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            CenterControls();
        }
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            CenterControls();
        }
    }
}