using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Argo_Books.UI
{
    public class CustomCircleButton : Button
    {
        private Image _buttonImage;
        private Color _fillColor = Color.LightGray;
        private Color _pressedColor = Color.DarkGray;
        private bool _isPressed = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image ButtonImage
        {
            get => _buttonImage;
            set
            {
                _buttonImage = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color FillColor
        {
            get => _fillColor;
            set
            {
                _fillColor = value;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color PressedColor
        {
            get => _pressedColor;
            set
            {
                _pressedColor = value;
                Invalidate();
            }
        }

        public CustomCircleButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.DoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);

            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            BackColor = Color.Transparent;
            Text = string.Empty;
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            // Calculate circle bounds - leave 1px margin to avoid edge clipping
            Rectangle bounds = new(1, 1, Width - 3, Height - 3);

            // Fill circle
            Color currentFillColor = _isPressed ? _pressedColor : _fillColor;
            using (SolidBrush brush = new(currentFillColor))
            {
                e.Graphics.FillEllipse(brush, bounds);
            }

            // Draw image if available
            if (_buttonImage != null)
            {
                float scale = DpiHelper.GetRelativeDpiScale();
                int imageSize = (int)(32 * scale);

                // Perfect center calculation
                int x = (Width - imageSize) / 2;
                int y = (Height - imageSize) / 2;

                Rectangle imageRect = new(x, y, imageSize, imageSize);
                e.Graphics.DrawImage(_buttonImage, imageRect);
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isPressed = true;
            Invalidate();
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            Invalidate();
            base.OnMouseEnter(e);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Make the button circular
            using GraphicsPath path = new();
            path.AddEllipse(1, 1, Width - 3, Height - 3);
            Region = new Region(path);
        }
    }
}