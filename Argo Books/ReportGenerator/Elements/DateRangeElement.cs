using Argo_Books.ReportGenerator;
using Guna.UI2.WinForms;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Date range element for displaying report date filters.
    /// </summary>
    public class DateRangeElement : BaseElement
    {
        // Properties
        public string DateFormat { get; set; } = "yyyy-MM-dd";
        public Color TextColor { get; set; } = Color.Gray;
        public float FontSize { get; set; } = 10f;
        public FontStyle FontStyle { get; set; } = FontStyle.Italic;
        public string FontFamily { get; set; } = "Segoe UI";
        public StringAlignment HAlignment { get; set; } = StringAlignment.Center;
        public StringAlignment VAlignment { get; set; } = StringAlignment.Center;

        // Overrides
        public override byte MinimumSize => 40;
        public override string DisplayName => LanguageManager.TranslateString("date range");
        public override ReportElementType GetElementType() => ReportElementType.DateRange;
        public override BaseElement Clone()
        {
            return new DateRangeElement
            {
                Id = Id,
                Bounds = Bounds,
                ZOrder = ZOrder,
                IsVisible = IsVisible,
                DateFormat = DateFormat,
                TextColor = TextColor,
                FontSize = FontSize,
                FontStyle = FontStyle,
                FontFamily = FontFamily,
                HAlignment = HAlignment,
                VAlignment = VAlignment
            };
        }
        public override void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale)
        {
            try
            {
                if (config?.Filters == null) { return; }

                DateTime? startDate = config.Filters.StartDate;
                DateTime? endDate = config.Filters.EndDate;

                string dateText = LanguageManager.TranslateString("Period") + ": ";
                if (startDate.HasValue && endDate.HasValue)
                {
                    dateText += $"{startDate.Value.ToString(DateFormat)} to {endDate.Value.ToString(DateFormat)}";
                }
                else
                {
                    dateText += LanguageManager.TranslateString("Not specified");
                }

                using Font font = new(FontFamily, FontSize, FontStyle);
                using SolidBrush brush = new(TextColor);

                StringFormat format = new()
                {
                    Alignment = HAlignment,
                    LineAlignment = VAlignment,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                graphics.DrawString(dateText, font, brush, Bounds, format);
            }
            catch
            {
                RenderError(graphics);
            }
        }
        protected override int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Get undo manager for recording property changes
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Date format
            text = LanguageManager.TranslateString("Format") + ":";
            AddPropertyLabel(container, text, yPosition);
            string[] dateFormats = ["yyyy-MM-dd", "MM/dd/yyyy", "dd/MM/yyyy", "MMM dd, yyyy", "MMMM dd, yyyy",
                            "dd-MMM-yyyy", "yyyy/MM/dd", "dd.MM.yyyy"];
            Guna2ComboBox formatCombo = AddPropertyComboBox(container, DateFormat, yPosition, dateFormats,
                value =>
                {
                    if (DateFormat != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(DateFormat),
                            DateFormat,
                            value,
                            onPropertyChanged));
                        DateFormat = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("DateFormat", formatCombo, () => formatCombo.SelectedItem = DateFormat);
            yPosition += ControlRowHeight;

            // Font Family
            text = LanguageManager.TranslateString("Font") + ":";
            Label fontLabel = AddPropertyLabel(container, text, yPosition);

            Guna2TextBox fontTextBox = AddPropertySearchBox(
                container,
                FontFamily,
                yPosition,
                GetFontSearchResults,
                value =>
                {
                    if (FontFamily != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(FontFamily),
                            FontFamily,
                            value,
                            onPropertyChanged));
                        FontFamily = value;
                        onPropertyChanged();
                    }
                },
                fontLabel);

            CacheControl("FontFamily", fontTextBox, () => fontTextBox.Text = FontFamily);
            yPosition += ControlRowHeight;

            // Font Size
            text = LanguageManager.TranslateString("Size") + ":";
            AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown fontSizeNumeric = AddPropertyNumericUpDown(container, (decimal)FontSize, yPosition,
                value =>
                {
                    float newFontSize = (float)value;
                    if (Math.Abs(FontSize - newFontSize) > 0.01f)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(FontSize),
                            FontSize,
                            newFontSize,
                            onPropertyChanged));
                        FontSize = newFontSize;
                        onPropertyChanged();
                    }
                }, 6, 72);
            CacheControl("FontSize", fontSizeNumeric, () => fontSizeNumeric.Value = (decimal)FontSize);
            yPosition += ControlRowHeight;

            // Font Style (Bold, Italic, Underline)
            text = LanguageManager.TranslateString("Style") + ":";
            AddPropertyLabel(container, text, yPosition);

            // Create the font style buttons
            const int buttonWidth = 35;
            const int buttonHeight = 30;
            const int spacing = 5;
            const int totalButtonWidth = (buttonWidth * 3) + (spacing * 2);  // 3 buttons + 2 gaps
            int xPosition = container.ClientSize.Width - RightMargin - totalButtonWidth;
            int buttonY = yPosition + 2;

            // Bold button
            Guna2Button boldButton = CreateFontStyleButton("B", FontStyle.Bold, xPosition, buttonY, buttonWidth, buttonHeight);
            boldButton.Checked = FontStyle.HasFlag(FontStyle.Bold);
            boldButton.Tag = "Bold";
            container.Controls.Add(boldButton);
            xPosition += buttonWidth + spacing;

            // Italic button
            Guna2Button italicButton = CreateFontStyleButton("I", FontStyle.Italic, xPosition, buttonY, buttonWidth, buttonHeight);
            italicButton.Checked = FontStyle.HasFlag(FontStyle.Italic);
            italicButton.Tag = "Italic";
            container.Controls.Add(italicButton);
            xPosition += buttonWidth + spacing;

            // Underline button
            Guna2Button underlineButton = CreateFontStyleButton("U", FontStyle.Underline, xPosition, buttonY, buttonWidth, buttonHeight);
            underlineButton.Checked = FontStyle.HasFlag(FontStyle.Underline);
            underlineButton.Tag = "Underline";
            container.Controls.Add(underlineButton);

            // Store the buttons in cache for later updates
            CacheControl("BoldButton", boldButton, () => boldButton.Checked = FontStyle.HasFlag(FontStyle.Bold));
            CacheControl("ItalicButton", italicButton, () => italicButton.Checked = FontStyle.HasFlag(FontStyle.Italic));
            CacheControl("UnderlineButton", underlineButton, () => underlineButton.Checked = FontStyle.HasFlag(FontStyle.Underline));

            // Attach event handlers to update the FontStyle property
            FontStyle currentFontStyle = FontStyle;

            boldButton.CheckedChanged += (s, e) =>
            {
                FontStyle newStyle = currentFontStyle;
                if (boldButton.Checked)
                {
                    newStyle |= FontStyle.Bold;
                }
                else
                {
                    newStyle &= ~FontStyle.Bold;
                }

                if (currentFontStyle != newStyle)
                {
                    undoRedoManager?.RecordAction(new PropertyChangeAction(
                        this,
                        nameof(FontStyle),
                        FontStyle,
                        newStyle,
                        onPropertyChanged));
                    FontStyle = newStyle;
                    currentFontStyle = newStyle;
                    onPropertyChanged();
                }
            };

            italicButton.CheckedChanged += (s, e) =>
            {
                FontStyle newStyle = currentFontStyle;
                if (italicButton.Checked)
                {
                    newStyle |= FontStyle.Italic;
                }
                else
                {
                    newStyle &= ~FontStyle.Italic;
                }

                if (currentFontStyle != newStyle)
                {
                    undoRedoManager?.RecordAction(new PropertyChangeAction(
                        this,
                        nameof(FontStyle),
                        FontStyle,
                        newStyle,
                        onPropertyChanged));
                    FontStyle = newStyle;
                    currentFontStyle = newStyle;
                    onPropertyChanged();
                }
            };

            underlineButton.CheckedChanged += (s, e) =>
            {
                FontStyle newStyle = currentFontStyle;
                if (underlineButton.Checked)
                {
                    newStyle |= FontStyle.Underline;
                }
                else
                {
                    newStyle &= ~FontStyle.Underline;
                }

                if (currentFontStyle != newStyle)
                {
                    undoRedoManager?.RecordAction(new PropertyChangeAction(
                        this,
                        nameof(FontStyle),
                        FontStyle,
                        newStyle,
                        onPropertyChanged));
                    FontStyle = newStyle;
                    currentFontStyle = newStyle;
                    onPropertyChanged();
                }
            };

            yPosition += ControlRowHeight;

            // Text Color
            text = LanguageManager.TranslateString("Text Color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel colorPanel = AddColorPicker(container, yPosition, TextColor,
                newColor =>
                {
                    if (TextColor != newColor)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(TextColor),
                            TextColor,
                            newColor,
                            onPropertyChanged));
                        TextColor = newColor;
                        onPropertyChanged();
                    }
                });
            CacheControl("TextColor", colorPanel, () => colorPanel.BackColor = TextColor);
            yPosition += ControlRowHeight;

            // Horizontal Alignment
            text = LanguageManager.TranslateString("H-Align");
            AddPropertyLabel(container, text, yPosition);
            Guna2ComboBox hAlignCombo = AddPropertyComboBox(
                container,
                AlignmentHelper.ToDisplayText(HAlignment),
                yPosition,
                AlignmentHelper.HorizontalOptions,
                value =>
                {
                    StringAlignment newAlignment = AlignmentHelper.FromDisplayText(value);
                    if (HAlignment != newAlignment)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(HAlignment),
                            HAlignment,
                            newAlignment,
                            onPropertyChanged));
                        HAlignment = newAlignment;
                        onPropertyChanged();
                    }
                });
            CacheControl("HAlignment", hAlignCombo, () => hAlignCombo.SelectedItem = HAlignment.ToString());
            yPosition += ControlRowHeight;

            // Vertical Alignment
            text = LanguageManager.TranslateString("V-Align") + ":";
            AddPropertyLabel(container, text, yPosition);
            Guna2ComboBox vAlignCombo = AddPropertyComboBox(
                container,
                AlignmentHelper.ToDisplayText(VAlignment, isVertical: true),
                yPosition,
                AlignmentHelper.VerticalOptions,
                value =>
                {
                    StringAlignment newAlignment = AlignmentHelper.FromDisplayText(value);
                    if (VAlignment != newAlignment)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(VAlignment),
                            VAlignment,
                            newAlignment,
                            onPropertyChanged));
                        VAlignment = newAlignment;
                        onPropertyChanged();
                    }
                });
            CacheControl("VAlignment", vAlignCombo,
                () => vAlignCombo.SelectedItem = AlignmentHelper.ToDisplayText(VAlignment, isVertical: true));
            yPosition += ControlRowHeight;

            return yPosition;
        }
    }
}