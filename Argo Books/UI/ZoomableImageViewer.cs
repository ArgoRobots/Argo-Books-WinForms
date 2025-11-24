using System.Drawing.Drawing2D;
using Timer = System.Windows.Forms.Timer;

namespace Argo_Books.UI
{
    /// <summary>
    /// Adds zoom and pan functionality to an existing PictureBox control.
    /// Attaches event handlers to enable mouse wheel zoom-to-cursor and drag-to-pan.
    /// Includes rubber band effect when panning beyond boundaries.
    /// </summary>
    public class ZoomableImageViewer : IDisposable
    {
        // Target control
        private readonly PictureBox _pictureBox;
        private readonly VScrollBar _vScrollBar;
        private readonly HScrollBar _hScrollBar;

        // Original image
        private Image _originalImage;

        // Zoom and pan state
        private float _zoomFactor = 1.0f;
        private PointF _imageOffset = PointF.Empty;
        private bool _isPanning = false;
        private Point _panStartPoint;
        private PointF _panStartOffset;

        // Rubber band animation
        private Timer _rubberBandTimer;
        private PointF _rubberBandStartOffset;
        private PointF _rubberBandTargetOffset;
        private DateTime _rubberBandStartTime;
        private const int RubberBandDurationMs = 300;
        private const float RubberBandResistance = 0.3f;  // How much resistance when over-panning

        // Configuration
        private const float MinZoom = 0.1f;
        private const float MaxZoom = 10.0f;
        private const float ZoomStep = 1.25f;  // 25% zoom per wheel step
        private const float ButtonZoomStep = 0.25f;  // 25% for button clicks

        // Events
        public event EventHandler ZoomChanged;
        public event EventHandler ImageChanged;

        // Properties
        public Image Image
        {
            get => _originalImage;
            set
            {
                _originalImage?.Dispose();
                _originalImage = value;
                UpdatePictureBoxImage();
                FitToWindow();
                ImageChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public float ZoomFactor
        {
            get => _zoomFactor;
            set
            {
                _zoomFactor = Math.Max(MinZoom, Math.Min(MaxZoom, value));
                UpdateScrollBars();
                UpdatePictureBoxImage();
                ZoomChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public string ZoomPercentageText => $"{(int)(_zoomFactor * 100)}%";

        // Constructor
        public ZoomableImageViewer(PictureBox pictureBox, VScrollBar vScrollBar = null, HScrollBar hScrollBar = null)
        {
            _pictureBox = pictureBox ?? throw new ArgumentNullException(nameof(pictureBox));
            _vScrollBar = vScrollBar;
            _hScrollBar = hScrollBar;

            InitializeRubberBandTimer();
            AttachEventHandlers();
            ConfigurePictureBox();
        }
        private void ConfigurePictureBox()
        {
            _pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            _pictureBox.BorderStyle = BorderStyle.FixedSingle;

            // Store original image if one exists
            if (_pictureBox.Image != null)
            {
                _originalImage = _pictureBox.Image;
            }
        }
        private void InitializeRubberBandTimer()
        {
            _rubberBandTimer = new Timer
            {
                Interval = 16  // ~60 FPS
            };
            _rubberBandTimer.Tick += RubberBandTimer_Tick;
        }
        private void AttachEventHandlers()
        {
            // Detach first to prevent duplicates
            DetachEventHandlers();

            // Picture box events
            _pictureBox.Paint += PictureBox_Paint;
            _pictureBox.MouseDown += PictureBox_MouseDown;
            _pictureBox.MouseMove += PictureBox_MouseMove;
            _pictureBox.MouseUp += PictureBox_MouseUp;
            _pictureBox.MouseWheel += PictureBox_MouseWheel;
            _pictureBox.MouseEnter += PictureBox_MouseEnter;
            _pictureBox.Resize += PictureBox_Resize;

            // Scroll bar events if provided
            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll += VScrollBar_Scroll;
            }

            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll += HScrollBar_Scroll;
            }
        }
        private void DetachEventHandlers()
        {
            _pictureBox.Paint -= PictureBox_Paint;
            _pictureBox.MouseDown -= PictureBox_MouseDown;
            _pictureBox.MouseMove -= PictureBox_MouseMove;
            _pictureBox.MouseUp -= PictureBox_MouseUp;
            _pictureBox.MouseWheel -= PictureBox_MouseWheel;
            _pictureBox.MouseEnter -= PictureBox_MouseEnter;
            _pictureBox.Resize -= PictureBox_Resize;

            if (_vScrollBar != null)
            {
                _vScrollBar.Scroll -= VScrollBar_Scroll;
            }

            if (_hScrollBar != null)
            {
                _hScrollBar.Scroll -= HScrollBar_Scroll;
            }
        }

        // Public zoom methods
        public void ZoomIn()
        {
            Point center = new(_pictureBox.Width / 2, _pictureBox.Height / 2);
            float newZoom = _zoomFactor * (1 + ButtonZoomStep);
            ZoomAtPoint(center, newZoom);
        }
        public void ZoomOut()
        {
            Point center = new(_pictureBox.Width / 2, _pictureBox.Height / 2);
            float newZoom = _zoomFactor * (1 - ButtonZoomStep);
            ZoomAtPoint(center, newZoom);
        }
        public void FitToWindow()
        {
            if (_originalImage == null) { return; }

            float scaleX = (float)_pictureBox.Width / _originalImage.Width;
            float scaleY = (float)_pictureBox.Height / _originalImage.Height;
            _zoomFactor = Math.Min(scaleX, scaleY) * 0.95f;  // 95% to show borders

            CenterImage();
            UpdateScrollBars();
            UpdatePictureBoxImage();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
        public void ActualSize()
        {
            _zoomFactor = 1.0f;
            CenterImage();
            UpdateScrollBars();
            UpdatePictureBoxImage();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
        public void SetZoom(float zoomFactor)
        {
            Point center = new(_pictureBox.Width / 2, _pictureBox.Height / 2);
            ZoomAtPoint(center, zoomFactor);
        }

        // Event handlers
        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (_originalImage == null) { return; }

            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Clear background
            g.Clear(_pictureBox.BackColor);

            // Calculate scaled dimensions
            float scaledWidth = _originalImage.Width * _zoomFactor;
            float scaledHeight = _originalImage.Height * _zoomFactor;

            // Draw image
            RectangleF destRect = new(
                _imageOffset.X,
                _imageOffset.Y,
                scaledWidth,
                scaledHeight
            );

            g.DrawImage(_originalImage, destRect,
                new RectangleF(0, 0, _originalImage.Width, _originalImage.Height),
                GraphicsUnit.Pixel);
        }
        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _originalImage != null)
            {
                // Stop any active rubber band animation
                _rubberBandTimer.Stop();

                _isPanning = true;
                _panStartPoint = e.Location;
                _panStartOffset = _imageOffset;
                _pictureBox.Cursor = Cursors.Hand;
            }
        }
        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && _originalImage != null)
            {
                float deltaX = e.X - _panStartPoint.X;
                float deltaY = e.Y - _panStartPoint.Y;

                _imageOffset.X = _panStartOffset.X + deltaX;
                _imageOffset.Y = _panStartOffset.Y + deltaY;

                // Apply rubber band resistance when over-panning
                ApplyRubberBandResistance(ref _imageOffset);

                UpdateScrollBars();
                _pictureBox.Invalidate();
            }
        }
        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPanning = false;
                _pictureBox.Cursor = Cursors.Default;

                // Check if we need to snap back
                if (_originalImage != null && IsOutOfBounds(_imageOffset))
                {
                    StartRubberBandAnimation(_imageOffset, GetConstrainedOffset(_imageOffset));
                }
            }
        }
        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (_originalImage == null) { return; }

            float zoomMultiplier = e.Delta > 0 ? ZoomStep : 1f / ZoomStep;
            float newZoom = _zoomFactor * zoomMultiplier;

            ZoomAtPoint(e.Location, newZoom);
        }
        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            _pictureBox.Focus();
        }
        private void PictureBox_Resize(object sender, EventArgs e)
        {
            if (_originalImage != null)
            {
                ConstrainOffset();
                UpdateScrollBars();
                _pictureBox.Invalidate();
            }
        }
        private void VScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _imageOffset.Y = -e.NewValue;
            _pictureBox.Invalidate();
        }
        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            _imageOffset.X = -e.NewValue;
            _pictureBox.Invalidate();
        }

        // Rubber band animation
        private void RubberBandTimer_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - _rubberBandStartTime).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / RubberBandDurationMs);

            // Ease out cubic for smooth deceleration
            double easedProgress = 1 - Math.Pow(1 - progress, 3);

            _imageOffset.X = Lerp(_rubberBandStartOffset.X, _rubberBandTargetOffset.X, (float)easedProgress);
            _imageOffset.Y = Lerp(_rubberBandStartOffset.Y, _rubberBandTargetOffset.Y, (float)easedProgress);

            _pictureBox.Invalidate();
            UpdateScrollBars();

            if (progress >= 1.0)
            {
                _rubberBandTimer.Stop();
                _imageOffset = _rubberBandTargetOffset;
                _pictureBox.Invalidate();
            }
        }
        private void StartRubberBandAnimation(PointF from, PointF to)
        {
            _rubberBandStartOffset = from;
            _rubberBandTargetOffset = to;
            _rubberBandStartTime = DateTime.Now;
            _rubberBandTimer.Start();
        }
        private static float Lerp(float start, float end, float t)
        {
            return start + (end - start) * t;
        }

        // Private helper methods
        private void ZoomAtPoint(Point mousePos, float newZoom)
        {
            if (_originalImage == null) { return; }

            float oldZoom = _zoomFactor;
            _zoomFactor = Math.Max(MinZoom, Math.Min(MaxZoom, newZoom));

            if (Math.Abs(_zoomFactor - oldZoom) < 0.001f) { return; }

            // Calculate the point on the image that the mouse is over
            float imageX = (mousePos.X - _imageOffset.X) / oldZoom;
            float imageY = (mousePos.Y - _imageOffset.Y) / oldZoom;

            // Calculate new offset to keep the same point under the mouse
            _imageOffset.X = mousePos.X - imageX * _zoomFactor;
            _imageOffset.Y = mousePos.Y - imageY * _zoomFactor;

            ConstrainOffset();
            UpdateScrollBars();
            UpdatePictureBoxImage();
            ZoomChanged?.Invoke(this, EventArgs.Empty);
        }
        private void CenterImage()
        {
            if (_originalImage == null) { return; }

            float scaledWidth = _originalImage.Width * _zoomFactor;
            float scaledHeight = _originalImage.Height * _zoomFactor;

            _imageOffset = new PointF(
                (_pictureBox.Width - scaledWidth) / 2,
                (_pictureBox.Height - scaledHeight) / 2
            );

            ConstrainOffset();
        }
        private void ConstrainOffset()
        {
            _imageOffset = GetConstrainedOffset(_imageOffset);
        }
        private PointF GetConstrainedOffset(PointF offset)
        {
            if (_originalImage == null) { return offset; }

            float scaledWidth = _originalImage.Width * _zoomFactor;
            float scaledHeight = _originalImage.Height * _zoomFactor;
            PointF constrained = offset;

            // If image is smaller than view, center it
            if (scaledWidth <= _pictureBox.Width)
            {
                constrained.X = (_pictureBox.Width - scaledWidth) / 2;
            }
            else
            {
                // Constrain panning
                constrained.X = Math.Min(0, constrained.X);
                constrained.X = Math.Max(_pictureBox.Width - scaledWidth, constrained.X);
            }

            if (scaledHeight <= _pictureBox.Height)
            {
                constrained.Y = (_pictureBox.Height - scaledHeight) / 2;
            }
            else
            {
                // Constrain panning
                constrained.Y = Math.Min(0, constrained.Y);
                constrained.Y = Math.Max(_pictureBox.Height - scaledHeight, constrained.Y);
            }

            return constrained;
        }
        private bool IsOutOfBounds(PointF offset)
        {
            PointF constrained = GetConstrainedOffset(offset);
            return Math.Abs(offset.X - constrained.X) > 0.1f ||
                   Math.Abs(offset.Y - constrained.Y) > 0.1f;
        }
        private void ApplyRubberBandResistance(ref PointF offset)
        {
            if (_originalImage == null) { return; }

            PointF constrained = GetConstrainedOffset(offset);

            // Calculate how much we're over-panning
            float overX = offset.X - constrained.X;
            float overY = offset.Y - constrained.Y;

            // Apply resistance to the overshoot
            if (Math.Abs(overX) > 0.1f)
            {
                offset.X = constrained.X + overX * RubberBandResistance;
            }

            if (Math.Abs(overY) > 0.1f)
            {
                offset.Y = constrained.Y + overY * RubberBandResistance;
            }
        }
        private void UpdateScrollBars()
        {
            if (_originalImage == null)
            {
                if (_vScrollBar != null) { _vScrollBar.Visible = false; }
                if (_hScrollBar != null) { _hScrollBar.Visible = false; }
                return;
            }

            float scaledWidth = _originalImage.Width * _zoomFactor;
            float scaledHeight = _originalImage.Height * _zoomFactor;

            // Vertical scroll bar
            if (_vScrollBar != null)
            {
                if (scaledHeight > _pictureBox.Height)
                {
                    _vScrollBar.Visible = true;
                    _vScrollBar.Maximum = (int)(scaledHeight - _pictureBox.Height);
                    _vScrollBar.LargeChange = Math.Max(1, _pictureBox.Height / 10);
                    _vScrollBar.SmallChange = Math.Max(1, _pictureBox.Height / 20);
                    _vScrollBar.Value = Math.Max(0, Math.Min(_vScrollBar.Maximum, (int)-_imageOffset.Y));
                }
                else
                {
                    _vScrollBar.Visible = false;
                }
            }

            // Horizontal scroll bar
            if (_hScrollBar != null)
            {
                if (scaledWidth > _pictureBox.Width)
                {
                    _hScrollBar.Visible = true;
                    _hScrollBar.Maximum = (int)(scaledWidth - _pictureBox.Width);
                    _hScrollBar.LargeChange = Math.Max(1, _pictureBox.Width / 10);
                    _hScrollBar.SmallChange = Math.Max(1, _pictureBox.Width / 20);
                    _hScrollBar.Value = Math.Max(0, Math.Min(_hScrollBar.Maximum, (int)-_imageOffset.X));
                }
                else
                {
                    _hScrollBar.Visible = false;
                }
            }
        }
        private void UpdatePictureBoxImage()
        {
            // Force repaint
            _pictureBox.Invalidate();
        }

        // Cleanup
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rubberBandTimer?.Stop();
                _rubberBandTimer?.Dispose();
                DetachEventHandlers();
                _originalImage?.Dispose();
            }
        }
    }
}