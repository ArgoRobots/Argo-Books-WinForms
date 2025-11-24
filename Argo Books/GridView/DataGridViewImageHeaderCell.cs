using Argo_Books;

namespace Argo_Books.GridView
{
    /// <summary>
    /// A custom DataGridView column header cell that combines an image and text in the header,
    /// with hover and click functionalities for the image. The image can display a message box
    /// when clicked, making it interactive for guiding users with additional information.
    /// </summary>
    internal class DataGridViewImageHeaderCell(Image image, string headerText, string messageBoxText) : DataGridViewColumnHeaderCell
    {
        // Properties
        private bool _isImageHovered = false;
        private static readonly Color _hoverBackgroundColor = Color.FromArgb(187, 187, 187);
        private const byte _padding = 3;
        private readonly Image _headerImage = new Bitmap(image, new Size(28, 28));
        private readonly short _imageOffsetX = -20;
        private readonly byte _imageOffsetY = 5;
        private readonly string _messageBoxText = messageBoxText;

        // Getters and setters
        public string HeaderText { get; set; } = headerText;

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object? value, object? formattedValue, string? errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            // Exclude default text painting
            paintParts &= ~DataGridViewPaintParts.ContentForeground;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            if (_headerImage != null)
            {
                Size imgSize = new(25, 25);
                Point imgLocation = new(cellBounds.Right - imgSize.Width + _imageOffsetX - _padding, cellBounds.Y + _imageOffsetY);

                // Draw background color if image is hovered
                if (_isImageHovered)
                {
                    graphics.FillRectangle(new SolidBrush(_hoverBackgroundColor), new Rectangle(imgLocation.X - _padding, imgLocation.Y - _padding, imgSize.Width + _padding * 2, imgSize.Height + _padding * 2));
                }

                graphics.DrawImage(_headerImage, new Rectangle(imgLocation, imgSize));
            }

            if (!string.IsNullOrEmpty(HeaderText))
            {
                // Define text formatting options
                StringFormat stringFormat = new()
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                // Define the area where the text will be drawn
                RectangleF textBounds = new(
                    cellBounds.X + 5,
                    cellBounds.Y,
                    cellBounds.Width - (_headerImage?.Width ?? 0) + _imageOffsetX - 5,
                    cellBounds.Height
                );

                // Use the cell style's ForeColor for text drawing, so it updates when the theme changes
                using Brush textBrush = new SolidBrush(cellStyle.ForeColor);
                graphics.DrawString(HeaderText, cellStyle.Font, textBrush, textBounds, stringFormat);
            }
        }
        protected override void OnMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_headerImage != null)
            {
                Rectangle cellBounds = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, -1, true);
                Size imgSize = new(28, 28);
                Rectangle imgBounds = new(
                    cellBounds.Right - imgSize.Width + _imageOffsetX - _padding,
                    cellBounds.Y + _imageOffsetY - _padding,
                    imgSize.Width + _padding * 2,
                    imgSize.Height + _padding * 2
                );

                Point mouseLocation = new(e.X + cellBounds.X, e.Y + cellBounds.Y);

                bool isCurrentlyHovered = imgBounds.Contains(mouseLocation);
                if (isCurrentlyHovered != _isImageHovered)
                {
                    _isImageHovered = isCurrentlyHovered;
                    DataGridView.InvalidateCell(this);
                }
            }
        }
        protected override void OnMouseLeave(int rowIndex)
        {
            base.OnMouseLeave(rowIndex);
            if (_isImageHovered)
            {
                _isImageHovered = false;
                DataGridView.InvalidateCell(this);
            }
        }
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (_headerImage != null)
            {
                Rectangle cellBounds = DataGridView.GetCellDisplayRectangle(e.ColumnIndex, -1, true);
                Size imgSize = new(28, 28);
                Rectangle imgBounds = new(
                    cellBounds.Right - imgSize.Width + _imageOffsetX - _padding,
                    cellBounds.Y + _imageOffsetY - _padding,
                    imgSize.Width + _padding * 2,
                    imgSize.Height + _padding * 2
                );

                Point clickLocation = new(e.X + cellBounds.X, e.Y + cellBounds.Y);

                if (imgBounds.Contains(clickLocation))
                {
                    string translatedMessage = Language.LanguageManager.TranslateString(_messageBoxText);
                    CustomMessageBox.Show(
                        "Argo Books", translatedMessage,
                        CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                }
            }
        }
    }
}