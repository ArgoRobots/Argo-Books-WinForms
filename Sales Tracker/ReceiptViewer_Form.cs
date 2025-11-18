using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using Timer = System.Windows.Forms.Timer;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for viewing and previewing receipt images and PDFs.
    /// </summary>
    public partial class ReceiptViewer_Form : BaseForm
    {
        // Properties
        private readonly string _currentFilePath;
        private SKBitmap _skBitmap;
        private float _zoomFactor = 1.0f;
        private float _effectiveMaxZoom = 5.0f;
        private const float _zoomIncrement = 0.05f;
        private const float _minZoom = 0.1f;
        private const float _maxZoom = 5.0f;

        // Transform properties
        private float _offsetX = 0;
        private float _offsetY = 0;

        // Panning properties
        private bool _isPanning = false;
        private Point _panStartPoint;
        private float _startOffsetX;
        private float _startOffsetY;

        // Rubber band animation
        private Timer _rubberBandTimer;
        private SKPoint _rubberBandStartOffset;
        private SKPoint _rubberBandTargetOffset;
        private DateTime _rubberBandStartTime;
        private const int RubberBandDurationMs = 300;
        private const float RubberBandResistance = 0.3f;  // How much resistance when over-panning

        // SkiaSharp control
        private SKGLControl _skControl;

        // Supported file formats
        private static readonly HashSet<string> SupportedImageFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tiff", ".tif", ".webp"
        };
        private static readonly HashSet<string> SupportedDocumentFormats = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf"
        };

        // Init.
        public ReceiptViewer_Form(string filePath)
        {
            InitializeComponent();

            WebViewer.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                {
                    WebViewer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                }
            };

            _currentFilePath = filePath;
            InitializeSkiaSharp();
            InitializeRubberBandTimer();
            LoadReceipt();
            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void InitializeSkiaSharp()
        {
            // Remove the PictureBox if it exists
            if (ReceiptPictureBox != null)
            {
                ImagePanel.Controls.Remove(ReceiptPictureBox);
                ReceiptPictureBox.Dispose();
                ReceiptPictureBox = null;
            }

            // Create SKGLControl for hardware acceleration
            _skControl = new SKGLControl
            {
                Dock = DockStyle.Fill,
                Visible = false
            };

            _skControl.PaintSurface += OnSkControlPaintSurface;
            _skControl.MouseDown += OnSkControlMouseDown;
            _skControl.MouseMove += OnSkControlMouseMove;
            _skControl.MouseUp += OnSkControlMouseUp;
            _skControl.MouseWheel += OnSkControlMouseWheel;
            _skControl.MouseEnter += (s, e) => _skControl.Focus();

            ImagePanel.Controls.Add(_skControl);
        }
        private void InitializeRubberBandTimer()
        {
            _rubberBandTimer = new Timer
            {
                Interval = 16  // ~60 FPS
            };
            _rubberBandTimer.Tick += RubberBandTimer_Tick;
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ImagePanel.BackColor = CustomColors.ControlBack;
            ControlsPanel.BackColor = CustomColors.ToolbarBackground;
        }
        private void SetAccessibleDescriptions()
        {
            Zoom_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }

        // SkiaSharp Paint Event
        private void OnSkControlPaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.LightGray);

            if (_skBitmap == null) { return; }

            // Apply transformations
            canvas.Save();

            // Center the image and apply zoom
            float centerX = _skControl.Width / 2f;
            float centerY = _skControl.Height / 2f;

            // Calculate scaled dimensions
            float scaledWidth = _skBitmap.Width * _zoomFactor;
            float scaledHeight = _skBitmap.Height * _zoomFactor;

            // Calculate position to draw the image (centered + offset)
            float drawX = centerX - scaledWidth / 2f + _offsetX;
            float drawY = centerY - scaledHeight / 2f + _offsetY;

            // Draw the bitmap with high quality
            using (SKPaint paint = new()
            {
                IsAntialias = true
            })
            {
                SKRect destRect = SKRect.Create(drawX, drawY, scaledWidth, scaledHeight);
                canvas.DrawBitmap(_skBitmap, destRect, paint);
            }

            canvas.Restore();
        }

        // Mouse Events for SkiaSharp Control
        private void OnSkControlMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && CanPan())
            {
                _rubberBandTimer.Stop();
                _isPanning = true;
                _panStartPoint = e.Location;
                _startOffsetX = _offsetX;
                _startOffsetY = _offsetY;
                _skControl.Cursor = Cursors.SizeAll;
            }
        }
        private void OnSkControlMouseMove(object sender, MouseEventArgs e)
        {
            if (_isPanning && e.Button == MouseButtons.Left)
            {
                int deltaX = e.X - _panStartPoint.X;
                int deltaY = e.Y - _panStartPoint.Y;

                SKPoint newOffset = new(_startOffsetX + deltaX, _startOffsetY + deltaY);
                newOffset = ApplyRubberBandResistance(newOffset);

                _offsetX = newOffset.X;
                _offsetY = newOffset.Y;

                _skControl.Invalidate();
            }
            else if (CanPan())
            {
                _skControl.Cursor = Cursors.Hand;
            }
            else
            {
                _skControl.Cursor = Cursors.Default;
            }
        }
        private void OnSkControlMouseUp(object sender, MouseEventArgs e)
        {
            if (_isPanning)
            {
                _isPanning = false;
                _skControl.Cursor = CanPan() ? Cursors.Hand : Cursors.Default;

                // Check if we need to snap back
                if (IsOutOfBounds())
                {
                    SKPoint constrained = ConstrainOffset(new SKPoint(_offsetX, _offsetY));
                    StartRubberBandAnimation(new SKPoint(_offsetX, _offsetY), constrained);
                }
            }
        }
        private void OnSkControlMouseWheel(object sender, MouseEventArgs e)
        {
            if (_skBitmap == null) { return; }

            // Get mouse position relative to control
            Point mousePos = e.Location;

            // Determine zoom direction
            bool zoomIn = e.Delta > 0;
            float oldZoomFactor = _zoomFactor;

            if (zoomIn && _zoomFactor < _effectiveMaxZoom)
            {
                _zoomFactor = Math.Min(_zoomFactor + _zoomIncrement, _effectiveMaxZoom);
            }
            else if (!zoomIn && _zoomFactor > _minZoom)
            {
                _zoomFactor = Math.Max(_zoomFactor - _zoomIncrement, _minZoom);
            }

            if (Math.Abs(_zoomFactor - oldZoomFactor) > 0.001f)
            {
                // Calculate zoom point relative to center
                float centerX = _skControl.Width / 2f;
                float centerY = _skControl.Height / 2f;

                // Calculate the point under the mouse in image coordinates
                float mouseImageX = (mousePos.X - centerX - _offsetX) / oldZoomFactor;
                float mouseImageY = (mousePos.Y - centerY - _offsetY) / oldZoomFactor;

                // Calculate new offset to keep the same point under the mouse
                _offsetX = mousePos.X - centerX - mouseImageX * _zoomFactor;
                _offsetY = mousePos.Y - centerY - mouseImageY * _zoomFactor;

                // Constrain the offset
                SKPoint constrained = ConstrainOffset(new SKPoint(_offsetX, _offsetY));
                _offsetX = constrained.X;
                _offsetY = constrained.Y;

                Zoom_Label.Text = $"{_zoomFactor * 100:F0}%";
                _skControl.Invalidate();
            }
        }

        // Helper Methods
        private bool CanPan()
        {
            if (_skBitmap == null) { return false; }

            float scaledWidth = _skBitmap.Width * _zoomFactor;
            float scaledHeight = _skBitmap.Height * _zoomFactor;

            return scaledWidth > _skControl.Width || scaledHeight > _skControl.Height;
        }
        private bool IsOutOfBounds()
        {
            SKPoint current = new(_offsetX, _offsetY);
            SKPoint constrained = ConstrainOffset(current);
            return Math.Abs(current.X - constrained.X) > 1 || Math.Abs(current.Y - constrained.Y) > 1;
        }
        private SKPoint ConstrainOffset(SKPoint offset)
        {
            if (_skBitmap == null) { return offset; }

            float scaledWidth = _skBitmap.Width * _zoomFactor;
            float scaledHeight = _skBitmap.Height * _zoomFactor;
            float controlWidth = _skControl.Width;
            float controlHeight = _skControl.Height;

            float x = offset.X;
            float y = offset.Y;

            // If image is larger than control, limit panning
            if (scaledWidth > controlWidth)
            {
                float maxX = (scaledWidth - controlWidth) / 2f;
                x = Math.Max(-maxX, Math.Min(maxX, x));
            }
            else
            {
                x = 0;  // Center if smaller
            }

            if (scaledHeight > controlHeight)
            {
                float maxY = (scaledHeight - controlHeight) / 2f;
                y = Math.Max(-maxY, Math.Min(maxY, y));
            }
            else
            {
                y = 0;  // Center if smaller
            }

            return new SKPoint(x, y);
        }
        private SKPoint ApplyRubberBandResistance(SKPoint proposedOffset)
        {
            if (_skBitmap == null) { return proposedOffset; }

            SKPoint constrained = ConstrainOffset(proposedOffset);
            float overX = proposedOffset.X - constrained.X;
            float overY = proposedOffset.Y - constrained.Y;
            SKPoint result = proposedOffset;

            if (Math.Abs(overX) > 1)
            {
                result.X = constrained.X + overX * RubberBandResistance;
            }

            if (Math.Abs(overY) > 1)
            {
                result.Y = constrained.Y + overY * RubberBandResistance;
            }

            return result;
        }

        // Rubber Band Animation
        private void StartRubberBandAnimation(SKPoint from, SKPoint to)
        {
            _rubberBandStartOffset = from;
            _rubberBandTargetOffset = to;
            _rubberBandStartTime = DateTime.Now;
            _rubberBandTimer.Start();
        }
        private void RubberBandTimer_Tick(object sender, EventArgs e)
        {
            double elapsed = (DateTime.Now - _rubberBandStartTime).TotalMilliseconds;
            double progress = Math.Min(1.0, elapsed / RubberBandDurationMs);

            // Ease out cubic
            double easedProgress = 1 - Math.Pow(1 - progress, 3);

            _offsetX = Lerp(_rubberBandStartOffset.X, _rubberBandTargetOffset.X, (float)easedProgress);
            _offsetY = Lerp(_rubberBandStartOffset.Y, _rubberBandTargetOffset.Y, (float)easedProgress);

            _skControl.Invalidate();

            if (progress >= 1.0)
            {
                _rubberBandTimer.Stop();
                _offsetX = _rubberBandTargetOffset.X;
                _offsetY = _rubberBandTargetOffset.Y;
            }
        }
        private static float Lerp(float start, float end, float t)
        {
            return start + (end - start) * t;
        }

        // Form Events
        private void ReceiptViewer_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void ReceiptViewer_Form_Resize(object sender, EventArgs e)
        {
            if (_skBitmap != null && _skControl.Visible)
            {
                // Constrain offset after resize
                SKPoint constrained = ConstrainOffset(new SKPoint(_offsetX, _offsetY));
                _offsetX = constrained.X;
                _offsetY = constrained.Y;
                _skControl.Invalidate();
            }
        }
        private void ReceiptViewer_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            _skBitmap?.Dispose();
            _skControl?.Dispose();
            _rubberBandTimer?.Dispose();
            components?.Dispose();
        }

        // Button Events
        private void ZoomIn_Button_Click(object sender, EventArgs e)
        {
            if (_zoomFactor < _effectiveMaxZoom)
            {
                _zoomFactor = Math.Min(_zoomFactor + _zoomIncrement, _effectiveMaxZoom);
                ApplyZoom();
            }
        }
        private void ZoomOut_Button_Click(object sender, EventArgs e)
        {
            if (_zoomFactor > _minZoom)
            {
                _zoomFactor = Math.Max(_zoomFactor - _zoomIncrement, _minZoom);
                ApplyZoom();
            }
        }
        private void ResetZoom_Button_Click(object sender, EventArgs e)
        {
            _zoomFactor = 1.0f;
            _offsetX = 0;
            _offsetY = 0;
            ApplyZoom();
        }
        private void FitToWindow_Button_Click(object sender, EventArgs e)
        {
            FitToWindow();
        }
        private void Export_Button_Click(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new();
            dialog.FileName = Path.GetFileName(_currentFilePath);
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.Title = "Export Receipt";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.Copy(_currentFilePath, dialog.FileName, true);
                    CustomMessageBox.Show("Export successful", "Receipt exported successfully.",
                        CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
                }
                catch
                {
                    CustomMessageBox.Show("Export failed", "Failed to export receipt",
                        CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                }
            }
        }

        // Loading Methods
        private void LoadReceipt()
        {
            if (!File.Exists(_currentFilePath))
            {
                CustomMessageBox.Show("File not found", "The receipt file could not be found.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            string extension = Path.GetExtension(_currentFilePath);
            FileInfo fileInfo = new(_currentFilePath);

            Text = LanguageManager.TranslateString("Receipt Manager") +
                $" • {Path.GetFileName(_currentFilePath)} • {Tools.ConvertBytesToReadableSize(fileInfo.Length)}";

            try
            {
                if (SupportedImageFormats.Contains(extension))
                {
                    LoadImageFile();
                }
                else if (SupportedDocumentFormats.Contains(extension))
                {
                    LoadDocumentFile();
                }
                else
                {
                    CustomMessageBox.ShowWithFormat("Unsupported format",
                        "The file format '{0}' is not supported for viewing.",
                        CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok,
                        extension);
                }
            }
            catch
            {
                CustomMessageBox.Show("Error loading receipt",
                    "An error occurred while loading the receipt",
                   CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
            }
        }
        private void LoadImageFile()
        {
            // Load image using SkiaSharp
            using (FileStream stream = File.OpenRead(_currentFilePath))
            {
                _skBitmap = SKBitmap.Decode(stream);
            }

            if (_skBitmap == null)
            {
                CustomMessageBox.Show("Error loading image", "Failed to decode image file",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok);
                return;
            }

            _skControl.Visible = true;
            WebViewer.Visible = false;

            SetZoomControlsVisible(true);
            FitToWindow();
        }
        private async void LoadDocumentFile()
        {
            await WebViewer.EnsureCoreWebView2Async();
            WebViewer.Source = new Uri(_currentFilePath);
            WebViewer.Visible = true;
            _skControl.Visible = false;

            SetZoomControlsVisible(false);
        }

        // Zoom Methods
        private void SetZoomControlsVisible(bool visible)
        {
            ZoomIn_Button.Visible = visible;
            ZoomOut_Button.Visible = visible;
            ResetZoom_Button.Visible = visible;
            FitToWindow_Button.Visible = visible;
            Zoom_Label.Visible = visible;
        }
        private void ApplyZoom()
        {
            // Constrain offset after zoom
            SKPoint constrained = ConstrainOffset(new SKPoint(_offsetX, _offsetY));
            _offsetX = constrained.X;
            _offsetY = constrained.Y;

            Zoom_Label.Text = $"{_zoomFactor * 100:F0}%";
            _skControl.Invalidate();
        }
        private void FitToWindow()
        {
            if (_skBitmap == null) { return; }

            float scaleWidth = (float)_skControl.Width / _skBitmap.Width;
            float scaleHeight = (float)_skControl.Height / _skBitmap.Height;
            _zoomFactor = Math.Min(scaleWidth, scaleHeight);
            _zoomFactor = Math.Max(_zoomFactor, _minZoom);

            _effectiveMaxZoom = Math.Max(_maxZoom, _zoomFactor);

            _offsetX = 0;
            _offsetY = 0;

            ApplyZoom();
        }

        // Helper Methods
        public static bool IsFormatSupported(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) { return false; }
            string extension = Path.GetExtension(filePath);
            return SupportedImageFormats.Contains(extension) || SupportedDocumentFormats.Contains(extension);
        }
        public static string GetSupportedFormatsDescription()
        {
            string imageFormats = string.Join(", ", SupportedImageFormats.Select(f => f.ToUpper()));
            string documentFormats = string.Join(", ", SupportedDocumentFormats.Select(f => f.ToUpper()));
            return $"Images: {imageFormats}\nDocuments: {documentFormats}";
        }
    }
}