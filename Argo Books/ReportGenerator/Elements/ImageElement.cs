using Argo_Books.Classes;
using Argo_Books.ReportGenerator;
using Guna.UI2.WinForms;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;
using SkiaSharp;
using Svg.Skia;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Image scaling modes for display.
    /// </summary>
    public enum ImageScaleMode
    {
        Stretch,  // Stretch to fill bounds (may distort)
        Fit,      // Fit within bounds (maintain aspect ratio)
        Fill,     // Fill bounds (maintain aspect ratio, may crop)
        Center    // Center at original size
    }

    /// <summary>
    /// Image element for displaying PNG, JPEG, and SVG images.
    /// </summary>
    public class ImageElement : BaseElement, IDisposable
    {
        // Properties
        public string ImagePath { get; set; } = "";
        public ImageScaleMode ScaleMode { get; set; } = ImageScaleMode.Fit;
        public Color BackgroundColor { get; set; } = Color.Transparent;
        public Color BorderColor { get; set; } = Color.Transparent;
        public int BorderThickness { get; set; } = 1;
        public int CornerRadius_Percent { get; set; } = 0;
        public byte Opacity { get; set; } = 255;

        // Cached image data
        private Image _cachedImage;
        private SKSvg _cachedSvg;
        private string _cachedImagePath;

        /// <summary>
        /// Calculates the actual corner radius in pixels based on the percentage and bounds.
        /// </summary>
        private int GetActualCornerRadius()
        {
            if (CornerRadius_Percent <= 0) { return 0; }

            // Calculate radius as percentage of the smaller dimension
            // At 100%, radius = half of smaller side (creates a circle)
            int minDimension = Math.Min(Bounds.Width, Bounds.Height);
            return (int)(minDimension * (CornerRadius_Percent / 200.0));
        }

        // Overrides
        public override byte MinimumSize => 40;
        public override string DisplayName => LanguageManager.TranslateString("image");
        public override ReportElementType GetElementType() => ReportElementType.Image;
        public override BaseElement Clone()
        {
            return new ImageElement
            {
                Id = Id,
                Bounds = Bounds,
                ZOrder = ZOrder,
                IsVisible = IsVisible,
                ImagePath = ImagePath,
                ScaleMode = ScaleMode,
                BackgroundColor = BackgroundColor,
                BorderColor = BorderColor,
                BorderThickness = BorderThickness,
                CornerRadius_Percent = CornerRadius_Percent,
                Opacity = Opacity
            };
        }
        public override void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale)
        {
            try
            {
                int actualRadius = GetActualCornerRadius();

                // Draw background if not transparent
                if (BackgroundColor != Color.Transparent)
                {
                    using SolidBrush bgBrush = new(BackgroundColor);
                    if (actualRadius > 0)
                    {
                        using GraphicsPath path = GetRoundedRectanglePath(Bounds, actualRadius);
                        graphics.FillPath(bgBrush, path);
                    }
                    else
                    {
                        graphics.FillRectangle(bgBrush, Bounds);
                    }
                }

                // Load and render image
                if (!string.IsNullOrEmpty(ImagePath))
                {
                    string resolvedPath = ResolveImagePath(ImagePath);

                    if (File.Exists(resolvedPath))
                    {
                        string extension = Path.GetExtension(resolvedPath).ToLowerInvariant();

                        if (extension == ".svg")
                        {
                            RenderSvgImage(graphics, resolvedPath);
                        }
                        else if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
                        {
                            RenderBitmapImage(graphics, resolvedPath);
                        }
                    }
                    else
                    {
                        RenderPlaceholder(graphics);
                    }
                }
                else
                {
                    RenderPlaceholder(graphics);
                }

                // Draw border if specified
                if (BorderColor != Color.Transparent && BorderThickness > 0)
                {
                    using Pen borderPen = new(BorderColor, BorderThickness);
                    if (actualRadius > 0)
                    {
                        using GraphicsPath path = GetRoundedRectanglePath(Bounds, actualRadius);
                        graphics.DrawPath(borderPen, path);
                    }
                    else
                    {
                        graphics.DrawRectangle(borderPen, Bounds);
                    }
                }
            }
            catch
            {
                RenderError(graphics);
            }
        }

        private void RenderBitmapImage(Graphics graphics, string resolvedPath)
        {
            // Load image if not cached or path changed
            if (_cachedImage == null || _cachedImagePath != resolvedPath)
            {
                _cachedImage?.Dispose();
                _cachedImage = Image.FromFile(resolvedPath);
                _cachedImagePath = resolvedPath;
            }

            if (_cachedImage == null) { return; }

            // Save graphics state
            GraphicsState state = graphics.Save();

            try
            {
                int actualRadius = GetActualCornerRadius();

                // Set up clipping if using rounded corners
                if (actualRadius > 0)
                {
                    using GraphicsPath path = GetRoundedRectanglePath(Bounds, actualRadius);
                    graphics.SetClip(path);
                }

                // Apply opacity
                if (Opacity < 255)
                {
                    ColorMatrix colorMatrix = new()
                    {
                        Matrix33 = Opacity / 255f
                    };
                    ImageAttributes imageAttributes = new();
                    imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    Rectangle destRect = CalculateImageBounds();
                    graphics.DrawImage(_cachedImage, destRect, 0, 0, _cachedImage.Width, _cachedImage.Height, GraphicsUnit.Pixel, imageAttributes);
                }
                else
                {
                    Rectangle destRect = CalculateImageBounds();
                    graphics.DrawImage(_cachedImage, destRect);
                }
            }
            finally
            {
                graphics.Restore(state);
            }
        }
        private void RenderSvgImage(Graphics graphics, string resolvedPath)
        {
            // Load SVG if not cached or path changed
            if (_cachedSvg == null || _cachedImagePath != resolvedPath)
            {
                _cachedSvg = new SKSvg();
                _cachedSvg.Load(resolvedPath);
                _cachedImagePath = resolvedPath;
            }

            if (_cachedSvg?.Picture == null) { return; }

            // Save graphics state
            GraphicsState state = graphics.Save();

            try
            {
                int actualRadius = GetActualCornerRadius();

                // Set up clipping if using rounded corners
                if (actualRadius > 0)
                {
                    using GraphicsPath path = GetRoundedRectanglePath(Bounds, actualRadius);
                    graphics.SetClip(path);
                }

                // Calculate scale and position for SVG
                Rectangle destRect = CalculateSvgBounds();

                // Create a bitmap to render the SVG
                using SKBitmap bitmap = new(destRect.Width, destRect.Height);
                using SKCanvas canvas = new(bitmap);

                canvas.Clear(SKColors.Transparent);

                // Calculate scale to fit SVG in destination rectangle
                float scaleX = destRect.Width / _cachedSvg.Picture.CullRect.Width;
                float scaleY = destRect.Height / _cachedSvg.Picture.CullRect.Height;

                float scale = ScaleMode switch
                {
                    ImageScaleMode.Fit => Math.Min(scaleX, scaleY),
                    ImageScaleMode.Fill => Math.Max(scaleX, scaleY),
                    ImageScaleMode.Stretch => 1f,
                    _ => Math.Min(scaleX, scaleY)
                };

                if (ScaleMode == ImageScaleMode.Stretch)
                {
                    canvas.Scale(scaleX, scaleY);
                }
                else
                {
                    canvas.Scale(scale, scale);
                }

                // Center the SVG in the destination rectangle
                float offsetX = (destRect.Width / scale - _cachedSvg.Picture.CullRect.Width) / 2;
                float offsetY = (destRect.Height / scale - _cachedSvg.Picture.CullRect.Height) / 2;
                canvas.Translate(offsetX, offsetY);

                // Draw SVG
                if (Opacity < 255)
                {
                    using SKPaint paint = new();
                    paint.Color = new SKColor(255, 255, 255, Opacity);
                    canvas.DrawPicture(_cachedSvg.Picture, paint);
                }
                else
                {
                    canvas.DrawPicture(_cachedSvg.Picture);
                }

                canvas.Flush();

                // Convert SKBitmap to GDI+ Image
                using SKImage skImage = SKImage.FromBitmap(bitmap);
                using SKData data = skImage.Encode();
                using MemoryStream stream = new(data.ToArray());
                using Image renderedImage = Image.FromStream(stream);

                graphics.DrawImage(renderedImage, destRect);
            }
            finally
            {
                graphics.Restore(state);
            }
        }
        private Rectangle CalculateImageBounds()
        {
            if (_cachedImage == null || ScaleMode == ImageScaleMode.Stretch)
            {
                return Bounds;
            }

            float imageAspect = (float)_cachedImage.Width / _cachedImage.Height;
            float boundsAspect = (float)Bounds.Width / Bounds.Height;

            return ScaleMode switch
            {
                ImageScaleMode.Fit => CalculateFitBounds(imageAspect, boundsAspect),
                ImageScaleMode.Fill => CalculateFillBounds(imageAspect, boundsAspect),
                ImageScaleMode.Center => CalculateCenterBounds(),
                _ => Bounds
            };
        }
        private Rectangle CalculateSvgBounds()
        {
            if (_cachedSvg?.Picture == null || ScaleMode == ImageScaleMode.Stretch)
            {
                return Bounds;
            }

            float imageAspect = _cachedSvg.Picture.CullRect.Width / _cachedSvg.Picture.CullRect.Height;
            float boundsAspect = (float)Bounds.Width / Bounds.Height;

            return ScaleMode switch
            {
                ImageScaleMode.Fit => CalculateFitBounds(imageAspect, boundsAspect),
                ImageScaleMode.Fill => CalculateFillBounds(imageAspect, boundsAspect),
                ImageScaleMode.Center => CalculateCenterBounds(),
                _ => Bounds
            };
        }
        private Rectangle CalculateFitBounds(float imageAspect, float boundsAspect)
        {
            int width, height;

            if (imageAspect > boundsAspect)
            {
                // Image is wider than bounds
                width = Bounds.Width;
                height = (int)(Bounds.Width / imageAspect);
            }
            else
            {
                // Image is taller than bounds
                width = (int)(Bounds.Height * imageAspect);
                height = Bounds.Height;
            }

            int x = Bounds.X + (Bounds.Width - width) / 2;
            int y = Bounds.Y + (Bounds.Height - height) / 2;

            return new Rectangle(x, y, width, height);
        }
        private Rectangle CalculateFillBounds(float imageAspect, float boundsAspect)
        {
            int width, height;

            if (imageAspect > boundsAspect)
            {
                // Image is wider than bounds
                width = (int)(Bounds.Height * imageAspect);
                height = Bounds.Height;
            }
            else
            {
                // Image is taller than bounds
                width = Bounds.Width;
                height = (int)(Bounds.Width / imageAspect);
            }

            int x = Bounds.X + (Bounds.Width - width) / 2;
            int y = Bounds.Y + (Bounds.Height - height) / 2;

            return new Rectangle(x, y, width, height);
        }
        private Rectangle CalculateCenterBounds()
        {
            if (_cachedImage == null) { return Bounds; }

            int x = Bounds.X + (Bounds.Width - _cachedImage.Width) / 2;
            int y = Bounds.Y + (Bounds.Height - _cachedImage.Height) / 2;

            return new Rectangle(x, y, _cachedImage.Width, _cachedImage.Height);
        }
        private static GraphicsPath GetRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new();
            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
        private void RenderPlaceholder(Graphics graphics)
        {
            // Draw background
            using SolidBrush baseBgBrush = new(Color.FromArgb(240, 240, 240));
            graphics.FillRectangle(baseBgBrush, Bounds);

            using Font font = new("Segoe UI", 10);
            using SolidBrush textBrush = new(Color.Gray);

            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            string message = string.IsNullOrEmpty(ImagePath)
                ? LanguageManager.TranslateString("No image selected")
                : LanguageManager.TranslateString("Image not found");

            graphics.DrawString(message, font, textBrush, Bounds, format);

            // Draw border
            using Pen borderPen = new(Color.Black, 1);
            graphics.DrawRectangle(borderPen, Bounds);
        }
        protected override int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Get undo manager for recording property changes
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Image path with browse button
            text = LanguageManager.TranslateString("Image");
            AddPropertyLabel(container, text, yPosition);

            Guna2Button browseButton = new()
            {
                Text = LanguageManager.TranslateString("Select Image"),
                Size = new Size(container.Width - 95, ControlHeight),
                Location = new Point(85, yPosition),
                BorderRadius = 2,
                BorderThickness = 1,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            container.Controls.Add(browseButton);
            CacheControl("BrowseButton", browseButton, null);

            // Display the selected file name below the button
            Label pathLabel = new()
            {
                Text = !string.IsNullOrEmpty(ImagePath)
                    ? Path.GetFileName(ImagePath)
                    : LanguageManager.TranslateString("No image selected"),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(85, yPosition + ControlHeight + 2),
                AutoSize = false,
                Width = container.Width - 95,  // Match button width
                Height = 25,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            container.Controls.Add(pathLabel);
            CacheControl("PathLabel", pathLabel, () =>
                pathLabel.Text = !string.IsNullOrEmpty(ImagePath)
                    ? Path.GetFileName(ImagePath)
                    : LanguageManager.TranslateString("No image selected"));

            // Update browse button click handler to update the label
            browseButton.Click += (s, e) =>
            {
                using OpenFileDialog openDialog = new();
                openDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.svg|PNG Files|*.png|JPEG Files|*.jpg;*.jpeg|SVG Files|*.svg|All Files|*.*";
                openDialog.Title = "Select Image";

                if (!string.IsNullOrEmpty(ImagePath) && File.Exists(ImagePath))
                {
                    openDialog.InitialDirectory = Path.GetDirectoryName(ImagePath);
                }

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = openDialog.FileName;

                    // Copy the image to the template images directory
                    string newPath = CopyImageToTemplateDirectory(selectedPath);

                    if (ImagePath != newPath)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ImagePath),
                            ImagePath,
                            newPath,
                            () =>
                            {
                                // Clear cached image when path changes
                                _cachedImage?.Dispose();
                                _cachedImage = null;
                                _cachedSvg = null;
                                _cachedImagePath = null;
                                pathLabel.Text = Path.GetFileName(ImagePath);
                                onPropertyChanged();
                            }));

                        ImagePath = newPath;
                        _cachedImage?.Dispose();
                        _cachedImage = null;
                        _cachedSvg = null;
                        _cachedImagePath = null;

                        // Update the label immediately
                        pathLabel.Text = Path.GetFileName(ImagePath);
                        onPropertyChanged();
                    }
                }
            };

            yPosition += ControlHeight + 35;  // Account for button height plus label

            // Scale mode
            text = LanguageManager.TranslateString("Scale") + ":";
            AddPropertyLabel(container, text, yPosition);
            Guna2ComboBox scaleCombo = AddPropertyComboBox(container, ScaleMode.ToString(), yPosition,
                Enum.GetNames<ImageScaleMode>(),
                value =>
                {
                    ImageScaleMode newScaleMode = Enum.Parse<ImageScaleMode>(value);
                    if (ScaleMode != newScaleMode)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ScaleMode),
                            ScaleMode,
                            newScaleMode,
                            onPropertyChanged));
                        ScaleMode = newScaleMode;
                        onPropertyChanged();
                    }
                });
            CacheControl("ScaleMode", scaleCombo, () => scaleCombo.SelectedItem = ScaleMode.ToString());
            yPosition += ControlRowHeight;

            // Opacity
            text = LanguageManager.TranslateString("Opacity") + ":";
            AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown opacityNumeric = AddPropertyNumericUpDown(container, Opacity, yPosition,
                value =>
                {
                    byte newOpacity = (byte)value;
                    if (Opacity != newOpacity)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(Opacity),
                            Opacity,
                            newOpacity,
                            onPropertyChanged));
                        Opacity = newOpacity;
                        onPropertyChanged();
                    }
                }, 0, 255);
            CacheControl("Opacity", opacityNumeric, () => opacityNumeric.Value = Opacity);
            yPosition += ControlRowHeight;

            // Corner radius
            text = LanguageManager.TranslateString("Border radius");
            AddPropertyLabel(container, text + " %:", yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown radiusNumeric = AddPropertyNumericUpDown(container, CornerRadius_Percent, yPosition,
                value =>
                {
                    int newRadius = (int)value;
                    if (CornerRadius_Percent != newRadius)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(CornerRadius_Percent),
                            CornerRadius_Percent,
                            newRadius,
                            onPropertyChanged));
                        CornerRadius_Percent = newRadius;
                        onPropertyChanged();
                    }
                }, 0, 100);
            CacheControl("CornerRadius_Percent", radiusNumeric, () => radiusNumeric.Value = CornerRadius_Percent);
            yPosition += ControlRowHeight;

            // Border thickness
            text = LanguageManager.TranslateString("Border thickness") + ":";
            AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown thicknessNumeric = AddPropertyNumericUpDown(container, BorderThickness, yPosition,
                value =>
                {
                    int newThickness = (int)value;
                    if (BorderThickness != newThickness)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(BorderThickness),
                            BorderThickness,
                            newThickness,
                            onPropertyChanged));
                        BorderThickness = newThickness;
                        onPropertyChanged();
                    }
                }, 0, 20);
            CacheControl("BorderThickness", thicknessNumeric, () => thicknessNumeric.Value = BorderThickness);
            yPosition += ControlRowHeight;

            // Border color
            text = LanguageManager.TranslateString("Border Color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel borderColorPanel = AddColorPicker(container, yPosition, BorderColor,
                newColor =>
                {
                    if (BorderColor != newColor)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(BorderColor),
                            BorderColor,
                            newColor,
                            onPropertyChanged));
                        BorderColor = newColor;
                        onPropertyChanged();
                    }
                });
            CacheControl("BorderColor", borderColorPanel, () => borderColorPanel.BackColor = BorderColor);
            yPosition += ControlRowHeight;

            // Background color
            text = LanguageManager.TranslateString("Background Color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel bgColorPanel = AddColorPicker(container, yPosition, BackgroundColor,
                color =>
                {
                    if (BackgroundColor != color)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(BackgroundColor),
                            BackgroundColor,
                            color,
                            onPropertyChanged));
                        BackgroundColor = color;
                        onPropertyChanged();
                    }
                });
            CacheControl("BackgroundColor", bgColorPanel, () => bgColorPanel.BackColor = BackgroundColor);
            yPosition += ControlRowHeight;

            return yPosition;
        }

        /// <summary>
        /// Gets the directory where template images are stored.
        /// </summary>
        private static string GetImagesDirectory()
        {
            if (!Directory.Exists(Directories.ReportTemplateImages_dir))
            {
                Directory.CreateDirectory(Directories.ReportTemplateImages_dir);
            }
            return Directories.ReportTemplateImages_dir;
        }

        /// <summary>
        /// Copies an image file to the template images directory and returns the new path.
        /// </summary>
        private static string CopyImageToTemplateDirectory(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath) || !File.Exists(sourcePath))
            {
                return sourcePath;
            }

            try
            {
                string imagesDir = GetImagesDirectory();
                string fileName = Path.GetFileName(sourcePath);

                // Generate unique filename if file already exists
                string destPath = Path.Combine(imagesDir, fileName);
                int counter = 1;
                string nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                while (File.Exists(destPath))
                {
                    fileName = $"{nameWithoutExt}_{counter}{extension}";
                    destPath = Path.Combine(imagesDir, fileName);
                    counter++;
                }

                // Copy the file
                File.Copy(sourcePath, destPath, false);

                // Return just the filename (relative path)
                return fileName;
            }
            catch
            {
                // If copy fails, return the original path
                return sourcePath;
            }
        }

        /// <summary>
        /// Resolves an image path. If it's a relative path (just filename),
        /// looks for it in the template images directory.
        /// </summary>
        private static string ResolveImagePath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return imagePath;
            }

            // If it's already an absolute path and exists, use it
            if (Path.IsPathRooted(imagePath) && File.Exists(imagePath))
            {
                return imagePath;
            }

            // Try to find it in the template images directory
            string imagesDir = GetImagesDirectory();
            string possiblePath = Path.Combine(imagesDir, imagePath);

            if (File.Exists(possiblePath))
            {
                return possiblePath;
            }

            // Return original path if not found
            return imagePath;
        }

        // Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedImage?.Dispose();
                _cachedImage = null;
                _cachedSvg = null;
            }
        }
        ~ImageElement()
        {
            Dispose(false);
        }
    }
}