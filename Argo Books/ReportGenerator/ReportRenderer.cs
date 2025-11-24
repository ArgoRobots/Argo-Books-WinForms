using Argo_Books.Classes;
using Argo_Books.ReportGenerator.Elements;
using Argo_Books.ReportGenerator.Menus;
using SkiaSharp;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Handles the rendering of reports from ReportConfiguration to various output formats.
    /// </summary>
    public class ReportRenderer(ReportConfiguration config, ExportSettings exportSettings)
    {
        // Properties
        private readonly ReportConfiguration _config = config ?? throw new ArgumentNullException(nameof(config));
        private readonly ExportSettings _exportSettings = exportSettings ?? throw new ArgumentNullException(nameof(exportSettings));
        public static byte HeaderHeight { get; } = 50;
        public static byte FooterHeight { get; } = 30;
        public static byte SeparatorHeight { get; } = 5;
        public static float RenderScale { get; } = 3;

        // Init.
        /// <summary>
        /// Renders the report to a Bitmap for preview purposes.
        /// </summary>
        public Bitmap RenderToPreview(int maxWidth = 800, int maxHeight = 600)
        {
            Size pageSize = PageDimensions.GetDimensions(_config.PageSize, _config.PageOrientation);

            // Calculate preview scale to fit within max dimensions
            float scaleX = (float)maxWidth / pageSize.Width;
            float scaleY = (float)maxHeight / pageSize.Height;
            float scale = Math.Min(scaleX, scaleY);

            int previewWidth = (int)(pageSize.Width * scale);
            int previewHeight = (int)(pageSize.Height * scale);

            Bitmap bitmap = new(previewWidth, previewHeight);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.ScaleTransform(scale, scale);
                RenderReport(graphics, pageSize, _config);
            }

            return bitmap;
        }

        /// <summary>
        /// Renders and exports the report to the specified file.
        /// </summary>
        public void ExportReport()
        {
            if (_exportSettings.Format == ExportFormat.PDF)
            {
                ExportToPdf();
            }
            else
            {
                ExportToImage();
            }
        }

        // Private export methods
        private void ExportToImage()
        {
            Size pageSize = PageDimensions.GetDimensions(_config.PageSize, _config.PageOrientation);

            float scaleFactor = RenderScale;
            int exportWidth = (int)(pageSize.Width * scaleFactor);
            int exportHeight = (int)(pageSize.Height * scaleFactor);

            using Bitmap originalBitmap = new(exportWidth, exportHeight);
            using (Graphics graphics = Graphics.FromImage(originalBitmap))
            {
                graphics.ScaleTransform(scaleFactor, scaleFactor);
                RenderReport(graphics, pageSize, _config);
            }

            // Reduce quality based on setting
            using Bitmap finalBitmap = ReduceBitmapQuality(originalBitmap, _exportSettings.Quality);
            SaveBitmap(finalBitmap);
        }
        private void ExportToPdf()
        {
            Size pageSize = PageDimensions.GetDimensions(_config.PageSize, _config.PageOrientation);

            string? directory = Path.GetDirectoryName(_exportSettings.FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = Directories.GetNewFileNameIfItAlreadyExists(_exportSettings.FilePath);
            _exportSettings.FilePath = fileName;

            float scaleFactor = RenderScale;
            int exportWidth = (int)(pageSize.Width * scaleFactor);
            int exportHeight = (int)(pageSize.Height * scaleFactor);

            using Bitmap originalBitmap = new(exportWidth, exportHeight);
            using (Graphics graphics = Graphics.FromImage(originalBitmap))
            {
                graphics.ScaleTransform(scaleFactor, scaleFactor);
                RenderReport(graphics, pageSize, _config);
            }

            // Reduce quality based on setting
            using Bitmap finalBitmap = ReduceBitmapQuality(originalBitmap, _exportSettings.Quality);

            // Create PDF document using SkiaSharp
            using SKDocument document = SKDocument.CreatePdf(fileName);
            using SKCanvas canvas = document.BeginPage(pageSize.Width, pageSize.Height);

            // Convert to SkiaSharp bitmap
            using SKBitmap skBitmap = BitmapToSKBitmap(finalBitmap);

            // Draw scaled to fit the page
            canvas.DrawBitmap(skBitmap, SKRect.Create(0, 0, pageSize.Width, pageSize.Height));

            document.EndPage();
            document.Close();
        }

        /// <summary>
        /// Reduces bitmap quality based on quality setting by resampling to lower resolution.
        /// </summary>
        private static Bitmap ReduceBitmapQuality(Bitmap originalBitmap, int quality)
        {
            // Map quality (1-100) to scale factor (0.2 to 1.0)
            // quality=1 -> 20%, quality=50 -> 60%, quality=100 -> 100%
            float qualityScale = 0.2f + (quality / 100f * 0.8f);

            int newWidth = (int)(originalBitmap.Width * qualityScale);
            int newHeight = (int)(originalBitmap.Height * qualityScale);

            // If scale is essentially 1.0, clone the original
            if (qualityScale >= 0.95f)
            {
                return new Bitmap(originalBitmap);
            }

            // Create reduced size bitmap
            Bitmap reducedBitmap = new(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(reducedBitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                g.DrawImage(originalBitmap, 0, 0, newWidth, newHeight);
            }

            return reducedBitmap;
        }

        // Private rendering methods
        public static void SetupGraphics(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        }
        public static void RenderReport(Graphics graphics, Size pageSize, ReportConfiguration config, bool showGrid = false)
        {
            SetupGraphics(graphics);
            graphics.Clear(config.BackgroundColor);

            if (config.ShowHeader)
            {
                RenderHeader(graphics, pageSize, config);
            }

            if (config.ShowFooter)
            {
                RenderFooter(graphics, pageSize, config);
            }

            if (showGrid)
            {
                DrawGrid(graphics, config, ReportLayoutDesigner_Form.GridSize);
            }

            // Render all report elements using their own render methods
            List<BaseElement> elements = config.GetElementsByZOrder();
            foreach (BaseElement element in elements.Where(e => e.IsVisible))
            {
                element.RenderElement(graphics, config, RenderScale);
            }
        }
        private static void RenderHeader(Graphics graphics, Size pageSize, ReportConfiguration config)
        {
            Rectangle headerRect = new(
                config.PageMargins.Left,
                config.PageMargins.Top,
                pageSize.Width - config.PageMargins.Left - config.PageMargins.Right,
                HeaderHeight
            );

            using (Font titleFont = new("Segoe UI", 18, FontStyle.Bold))
            using (SolidBrush titleBrush = new(Color.Black))
            {
                StringFormat titleFormat = new()
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

                graphics.DrawString(config.Title, titleFont, titleBrush, headerRect, titleFormat);
            }

            // Draw separator line
            using Pen pen = new(Color.LightGray, 1);
            graphics.DrawLine(pen,
                headerRect.Left, headerRect.Bottom + SeparatorHeight,
                headerRect.Right, headerRect.Bottom + SeparatorHeight);
        }
        private static void RenderFooter(Graphics graphics, Size pageSize, ReportConfiguration config)
        {
            Rectangle footerRect = new(
                config.PageMargins.Left,
                pageSize.Height - config.PageMargins.Bottom - FooterHeight,
                pageSize.Width - config.PageMargins.Left - config.PageMargins.Right,
                FooterHeight
            );

            using Font footerFont = new("Segoe UI", 9);
            using SolidBrush footerBrush = new(Color.Gray);

            // Left side - generation date
            StringFormat leftFormat = new() { Alignment = StringAlignment.Near };
            graphics.DrawString($"Generated: {DateTime.Now:yyyy-MM-dd hh:mm tt}",
                footerFont, footerBrush, footerRect, leftFormat);

            // Right side - page number (if enabled)
            if (config.ShowPageNumbers)
            {
                StringFormat rightFormat = new() { Alignment = StringAlignment.Far };
                graphics.DrawString($"Page {config.CurrentPageNumber}", footerFont, footerBrush, footerRect, rightFormat);
            }
        }
        public static void DrawGrid(Graphics g, ReportConfiguration config, int gridSize)
        {
            Size pageSize = PageDimensions.GetDimensions(config.PageSize, config.PageOrientation);

            // Calculate content area (excluding header and footer)
            int topY = 0;
            int bottomY = pageSize.Height;

            if (config.ShowHeader)
            {
                topY = config.PageMargins.Top + HeaderHeight + SeparatorHeight;
            }

            if (config.ShowFooter)
            {
                bottomY = pageSize.Height - config.PageMargins.Bottom - FooterHeight;
            }

            using Pen pen = new(Color.LightGray);
            pen.DashStyle = DashStyle.Solid;

            // Draw vertical lines in content area
            for (int x = 0; x < pageSize.Width; x += gridSize)
            {
                g.DrawLine(pen, x, topY, x, bottomY);
            }

            // Draw horizontal lines in content area
            for (int y = topY; y < bottomY; y += gridSize)
            {
                g.DrawLine(pen, 0, y, pageSize.Width, y);
            }
        }
        private void SaveBitmap(Bitmap bitmap)
        {
            string? directory = Path.GetDirectoryName(_exportSettings.FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string fileName = Directories.GetNewFileNameIfItAlreadyExists(_exportSettings.FilePath);
            _exportSettings.FilePath = fileName;

            ImageFormat format = _exportSettings.Format switch
            {
                ExportFormat.PNG => ImageFormat.Png,
                ExportFormat.JPG => ImageFormat.Jpeg,
                _ => ImageFormat.Png
            };

            if (_exportSettings.Format == ExportFormat.JPG)
            {
                // Always use high JPEG compression quality (95)
                // The quality slider controls bitmap size reduction only
                EncoderParameters encoderParams = new(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 95L);
                ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
                bitmap.Save(fileName, jpegCodec, encoderParams);
            }
            else
            {
                bitmap.Save(fileName, format);
            }
        }
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Find the codec matching the mimeType
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo? codec = codecs.FirstOrDefault(c => c.MimeType == mimeType);

            return codec ?? throw new ArgumentException($"Encoder for MIME type '{mimeType}' not found.");
        }
        private static SKBitmap BitmapToSKBitmap(Bitmap bitmap)
        {
            using MemoryStream stream = new();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;
            return SKBitmap.Decode(stream);
        }
    }
}