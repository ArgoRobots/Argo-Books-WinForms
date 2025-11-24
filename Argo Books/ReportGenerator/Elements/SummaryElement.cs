using Argo_Books.DataClasses;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Summary element for displaying statistics and key metrics.
    /// </summary>
    public class SummaryElement : BaseElement
    {
        // Properties
        public TransactionType TransactionType { get; set; } = TransactionType.Revenue;
        public bool IncludeReturns { get; set; } = true;
        public bool IncludeLosses { get; set; } = true;
        public bool ShowTotalSales { get; set; } = true;
        public bool ShowTotalTransactions { get; set; } = true;
        public bool ShowAverageValue { get; set; } = true;
        public bool ShowGrowthRate { get; set; } = true;
        public Color BackgroundColor { get; set; } = Color.WhiteSmoke;
        public int BorderThickness { get; set; } = 1;
        public Color BorderColor { get; set; } = Color.LightGray;
        public string FontFamily { get; set; } = "Segoe UI";
        public float FontSize { get; set; } = 10f;
        public FontStyle FontStyle { get; set; } = FontStyle.Regular;
        public Color TextColor { get; set; } = Color.Black;
        public StringAlignment Alignment { get; set; } = StringAlignment.Near;
        public StringAlignment VerticalAlignment { get; set; } = StringAlignment.Near;

        private class SummaryStatistics
        {
            public decimal TotalSales { get; set; }
            public decimal TotalPurchases { get; set; }
            public decimal CombinedTotal { get; set; }
            public int TransactionCount { get; set; }
            public decimal AverageValue { get; set; }
            public decimal GrowthRate { get; set; }
        }

        // Overrides
        public override byte MinimumSize => 40;
        public override string DisplayName => LanguageManager.TranslateString("summary");
        public override ReportElementType GetElementType() => ReportElementType.Summary;
        public override BaseElement Clone()
        {
            return new SummaryElement
            {
                TransactionType = TransactionType,
                IncludeReturns = IncludeReturns,
                IncludeLosses = IncludeLosses,
                Id = Id,
                Bounds = Bounds,
                ZOrder = ZOrder,
                IsVisible = IsVisible,
                ShowTotalSales = ShowTotalSales,
                ShowTotalTransactions = ShowTotalTransactions,
                ShowAverageValue = ShowAverageValue,
                ShowGrowthRate = ShowGrowthRate,
                BackgroundColor = BackgroundColor,
                BorderThickness = BorderThickness,
                BorderColor = BorderColor,
                FontFamily = FontFamily,
                FontSize = FontSize,
                FontStyle = FontStyle,
                TextColor = TextColor,
                Alignment = Alignment,
                VerticalAlignment = VerticalAlignment
            };
        }
        public override void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale)
        {
            Region originalClip = graphics.Clip;

            try
            {
                // Draw background
                using SolidBrush bgBrush = new(BackgroundColor);
                graphics.FillRectangle(bgBrush, Bounds);

                // Draw border
                if (BorderColor != Color.Transparent && BorderThickness > 0)
                {
                    using Pen borderPen = new(BorderColor, BorderThickness);

                    // Adjust rectangle to account for border thickness to prevent clipping
                    Rectangle borderRect = Bounds;
                    if (BorderThickness > 1)
                    {
                        int offset = BorderThickness / 2;
                        borderRect = new Rectangle(
                            Bounds.X + offset,
                            Bounds.Y + offset,
                            Bounds.Width - BorderThickness,
                            Bounds.Height - BorderThickness
                        );
                    }

                    graphics.DrawRectangle(borderPen, borderRect);
                }

                // Add clipping region to prevent overflow
                graphics.SetClip(Bounds);

                // Calculate statistics
                SummaryStatistics stats = CalculateStatistics(config);

                // Draw summary content with actual values
                using Font titleFont = new(FontFamily, FontSize + 1, FontStyle.Bold | FontStyle);
                using Font valueFont = new(FontFamily, FontSize, FontStyle);
                using SolidBrush textBrush = new(TextColor);

                int titleHeight = titleFont.Height;
                int lineHeight = valueFont.Height;
                int padding = 10;

                // Calculate starting position based on alignment
                int contentHeight = CalculateContentHeight(titleFont, valueFont);
                int startY = VerticalAlignment switch
                {
                    StringAlignment.Near => Bounds.Y + padding,
                    StringAlignment.Center => Bounds.Y + (Bounds.Height - contentHeight) / 2,
                    StringAlignment.Far => Bounds.Bottom - contentHeight - padding,
                    _ => Bounds.Y + padding
                };

                StringFormat format = new()
                {
                    Alignment = Alignment,
                    LineAlignment = StringAlignment.Near,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisCharacter
                };

                // Check if there's enough space for title
                if (startY + titleHeight <= Bounds.Bottom - padding)
                {
                    RectangleF textBounds = new(Bounds.X + padding, startY, Bounds.Width - (padding * 2), titleHeight);
                    string text = LanguageManager.TranslateString("Summary Statistics");
                    graphics.DrawString(text, titleFont, textBrush, textBounds, format);
                    startY += titleHeight;
                }
                else
                {
                    return;  // Not enough space
                }

                if (ShowTotalSales && startY + lineHeight <= Bounds.Bottom - padding)
                {
                    string totalText = TransactionType switch
                    {
                        TransactionType.Expenses => LanguageManager.TranslateString("Total Expenses") + $": {FormatCurrency(stats.TotalPurchases)}",
                        TransactionType.Revenue => LanguageManager.TranslateString("Total Revenue") + $": {FormatCurrency(stats.TotalSales)}",
                        _ => LanguageManager.TranslateString("Total: error")
                    };
                    RectangleF textBounds = new(Bounds.X + padding, startY, Bounds.Width - (padding * 2), lineHeight);
                    graphics.DrawString(totalText, valueFont, textBrush, textBounds, format);
                    startY += lineHeight;
                }

                if (ShowTotalTransactions && startY + lineHeight <= Bounds.Bottom - padding)
                {
                    RectangleF textBounds = new(Bounds.X + padding, startY, Bounds.Width - (padding * 2), lineHeight);
                    string text = LanguageManager.TranslateString("Transactions");
                    graphics.DrawString($"{text}: {stats.TransactionCount:N0}", valueFont, textBrush, textBounds, format);
                    startY += lineHeight;
                }

                if (ShowAverageValue && startY + lineHeight <= Bounds.Bottom - padding)
                {
                    RectangleF textBounds = new(Bounds.X + padding, startY, Bounds.Width - (padding * 2), lineHeight);
                    string text = LanguageManager.TranslateString("Average Value");
                    graphics.DrawString($"{text}: {FormatCurrency(stats.AverageValue)}", valueFont, textBrush, textBounds, format);
                    startY += lineHeight;
                }

                if (ShowGrowthRate && startY + lineHeight <= Bounds.Bottom - padding)
                {
                    string growthSymbol = stats.GrowthRate >= 0 ? "↑" : "↓";
                    RectangleF textBounds = new(Bounds.X + padding, startY, Bounds.Width - (padding * 2), lineHeight);
                    string text = LanguageManager.TranslateString("Growth Rate");
                    graphics.DrawString($"{text}: {growthSymbol} {Math.Abs(stats.GrowthRate):F1}%", valueFont, textBrush, textBounds, format);
                }
            }
            catch
            {
                RenderError(graphics);
            }
            finally
            {
                graphics.Clip = originalClip;
            }
        }
        private int CalculateContentHeight(Font titleFont, Font valueFont)
        {
            int height = titleFont.Height;  // Title height
            int lineHeight = valueFont.Height;

            if (ShowTotalSales) { height += lineHeight; }
            if (ShowTotalTransactions) { height += lineHeight; }
            if (ShowAverageValue) { height += lineHeight; }
            if (ShowGrowthRate) { height += lineHeight; }

            return height;
        }
        private SummaryStatistics CalculateStatistics(ReportConfiguration config)
        {
            SummaryStatistics stats = new();

            if (config?.Filters == null)
            {
                return stats;
            }

            DateTime startDate = config.Filters.StartDate ?? DateTime.MinValue;
            DateTime endDate = config.Filters.EndDate ?? DateTime.MaxValue;

            try
            {
                // Calculate sales if needed
                if (TransactionType == TransactionType.Revenue)
                {
                    (decimal Total, int Count) = CalculateGridViewTotal(
                        MainMenu_Form.Instance.Sale_DataGridView,
                        startDate,
                        endDate,
                        IncludeReturns);

                    stats.TotalSales = Total;
                    stats.TransactionCount += Count;
                }

                // Calculate purchases if needed
                if (TransactionType == TransactionType.Expenses)
                {
                    (decimal Total, int Count) = CalculateGridViewTotal(
                        MainMenu_Form.Instance.Purchase_DataGridView,
                        startDate,
                        endDate,
                        IncludeReturns);

                    stats.TotalPurchases = Total;
                    if (TransactionType == TransactionType.Expenses)
                    {
                        stats.TransactionCount += Count;
                    }
                }

                // Calculate derived values
                stats.CombinedTotal = stats.TotalSales + stats.TotalPurchases;
                stats.AverageValue = stats.TransactionCount > 0 ? stats.CombinedTotal / stats.TransactionCount : 0;

                // Calculate growth rate if needed
                if (ShowGrowthRate)
                {
                    stats.GrowthRate = CalculateGrowthRate(config, startDate, endDate, stats.CombinedTotal);
                }
            }
            catch
            {
                // Return default stats on error
            }

            return stats;
        }

        private static (decimal Total, int Count) CalculateGridViewTotal(
            DataGridView gridView,
            DateTime startDate,
            DateTime endDate,
            bool includeReturns)
        {
            decimal total = 0;
            int count = 0;

            foreach (DataGridViewRow row in gridView.Rows)
            {
                if (row.Cells[ReadOnlyVariables.Date_column].Value != null &&
                    DateTime.TryParse(row.Cells[ReadOnlyVariables.Date_column].Value.ToString(), out DateTime rowDate))
                {
                    if (rowDate >= startDate && rowDate <= endDate)
                    {
                        if (row.Cells[ReadOnlyVariables.Total_column].Value != null &&
                            decimal.TryParse(row.Cells[ReadOnlyVariables.Total_column].Value.ToString(), out decimal amount))
                        {
                            bool isReturn = amount < 0;
                            if (!isReturn || includeReturns)
                            {
                                total += Math.Abs(amount);
                                count++;
                            }
                        }
                    }
                }
            }

            return (total, count);
        }

        private decimal CalculateGrowthRate(
            ReportConfiguration config,
            DateTime startDate,
            DateTime endDate,
            decimal currentPeriodTotal)
        {
            TimeSpan periodLength = endDate - startDate;
            DateTime prevStart = startDate.AddDays(-periodLength.TotalDays);
            DateTime prevEnd = startDate.AddDays(-1);

            decimal previousPeriodTotal = 0;

            // Get previous period sales
            if (config.Filters.TransactionType == TransactionType.Revenue)
            {
                (decimal Total, _) = CalculateGridViewTotal(
                    MainMenu_Form.Instance.Sale_DataGridView,
                    prevStart,
                    prevEnd,
                    IncludeReturns);
                previousPeriodTotal += Total;
            }

            // Get previous period purchases
            if (config.Filters.TransactionType == TransactionType.Expenses)
            {
                (decimal Total, _) = CalculateGridViewTotal(
                    MainMenu_Form.Instance.Purchase_DataGridView,
                    prevStart,
                    prevEnd,
                    IncludeReturns);
                previousPeriodTotal += Total;
            }

            if (previousPeriodTotal > 0)
            {
                return ((currentPeriodTotal - previousPeriodTotal) / previousPeriodTotal) * 100;
            }

            return 0;
        }

        private static string FormatCurrency(decimal amount)
        {
            // Format based on size
            if (amount >= 1000000)
            {
                return $"${amount / 1000000:F1}M";
            }
            else if (amount >= 1000)
            {
                return $"${amount / 1000:F1}K";
            }
            else
            {
                return $"${amount:F2}";
            }
        }
        protected override int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Get undo manager for recording property changes
            UndoRedoManager? undoRedoManager = ReportLayoutDesigner_Form.Instance?.GetUndoRedoManager();
            string text;

            // Transaction type
            AddPropertyLabel(container, "Data Type:", yPosition);
            Guna2ComboBox typeCombo = AddPropertyComboBox(container, TransactionType.ToString(), yPosition,
                Enum.GetNames<TransactionType>(),
                value =>
                {
                    TransactionType newType = Enum.Parse<TransactionType>(value);
                    if (TransactionType != newType)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(TransactionType),
                            TransactionType,
                            newType,
                            onPropertyChanged));
                        TransactionType = newType;
                        onPropertyChanged();
                    }

                    UpdateAllControlValues();  // This will update the "Show Expenses/Revenue" label text
                });
            CacheControl("TransactionType", typeCombo, () => typeCombo.SelectedItem = TransactionType.ToString());
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
            container.Controls.Add(boldButton);
            xPosition += buttonWidth + spacing;

            // Italic button
            Guna2Button italicButton = CreateFontStyleButton("I", FontStyle.Italic, xPosition, buttonY, buttonWidth, buttonHeight);
            italicButton.Checked = FontStyle.HasFlag(FontStyle.Italic);
            container.Controls.Add(italicButton);
            xPosition += buttonWidth + spacing;

            // Underline button
            Guna2Button underlineButton = CreateFontStyleButton("U", FontStyle.Underline, xPosition, buttonY, buttonWidth, buttonHeight);
            underlineButton.Checked = FontStyle.HasFlag(FontStyle.Underline);
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

            // Background Color
            text = LanguageManager.TranslateString("Background color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel bgColorPanel = AddColorPicker(container, yPosition, BackgroundColor,
                newColor =>
                {
                    if (BackgroundColor != newColor)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(BackgroundColor),
                            BackgroundColor,
                            newColor,
                            onPropertyChanged));
                        BackgroundColor = newColor;
                        onPropertyChanged();
                    }
                });
            CacheControl("BackgroundColor", bgColorPanel, () => bgColorPanel.BackColor = BackgroundColor);
            yPosition += ControlRowHeight;

            // Border Thickness
            text = LanguageManager.TranslateString("Border thickness") + ":";
            AddPropertyLabel(container, text, yPosition, false, NumericUpDownWidth);
            Guna2NumericUpDown borderThicknessNumeric = AddPropertyNumericUpDown(container, BorderThickness, yPosition,
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
            CacheControl("BorderThickness", borderThicknessNumeric, () => borderThicknessNumeric.Value = BorderThickness);
            yPosition += ControlRowHeight;

            // Border Color
            text = LanguageManager.TranslateString("Border color") + ":";
            AddPropertyLabel(container, text, yPosition, false, ColorPickerWidth);
            Panel borderColorPanel = AddColorPicker(container, yPosition, BorderColor,
                color =>
                {
                    if (BorderColor != color)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(BorderColor),
                            BorderColor,
                            color,
                            onPropertyChanged));
                        BorderColor = color;
                        onPropertyChanged();
                    }
                });
            CacheControl("BorderColor", borderColorPanel, () => borderColorPanel.BackColor = BorderColor);
            yPosition += ControlRowHeight;

            // Horizontal Alignment
            text = LanguageManager.TranslateString("H-Align") + ":";
            AddPropertyLabel(container, text, yPosition);
            Guna2ComboBox alignCombo = AddPropertyComboBox(
                container,
                AlignmentHelper.ToDisplayText(Alignment),
                yPosition,
                AlignmentHelper.HorizontalOptions,
                value =>
                {
                    StringAlignment newAlignment = AlignmentHelper.FromDisplayText(value);
                    if (Alignment != newAlignment)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(Alignment),
                            Alignment,
                            newAlignment,
                            onPropertyChanged));
                        Alignment = newAlignment;
                        onPropertyChanged();
                    }
                });
            CacheControl("Alignment", alignCombo,
                () => alignCombo.SelectedItem = AlignmentHelper.ToDisplayText(Alignment));
            yPosition += ControlRowHeight;

            // Vertical Alignment
            text = LanguageManager.TranslateString("V-Align") + ":";
            AddPropertyLabel(container, text, yPosition);
            Guna2ComboBox vAlignCombo = AddPropertyComboBox(
                container,
                AlignmentHelper.ToDisplayText(VerticalAlignment, isVertical: true),
                yPosition,
                AlignmentHelper.VerticalOptions,
                value =>
                {
                    StringAlignment newVAlignment = AlignmentHelper.FromDisplayText(value);
                    if (VerticalAlignment != newVAlignment)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(VerticalAlignment),
                            VerticalAlignment,
                            newVAlignment,
                            onPropertyChanged));
                        VerticalAlignment = newVAlignment;
                        onPropertyChanged();
                    }
                });
            CacheControl("VerticalAlignment", vAlignCombo,
                () => vAlignCombo.SelectedItem = AlignmentHelper.ToDisplayText(VerticalAlignment, isVertical: true));
            yPosition += ControlRowHeight;

            // Include returns checkbox
            text = LanguageManager.TranslateString("Include Returns");
            Guna2CustomCheckBox returnsCheck = AddPropertyCheckBoxWithLabel(container, text, IncludeReturns, yPosition,
                value =>
                {
                    if (IncludeReturns != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(IncludeReturns),
                            IncludeReturns,
                            value,
                            onPropertyChanged));
                        IncludeReturns = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("IncludeReturns", returnsCheck, () => returnsCheck.Checked = IncludeReturns);
            yPosition += CheckBoxRowHeight;

            // Include losses checkbox
            text = LanguageManager.TranslateString("Include Losses");
            Guna2CustomCheckBox lossesCheck = AddPropertyCheckBoxWithLabel(container, text, IncludeLosses, yPosition,
                value =>
                {
                    if (IncludeLosses != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(IncludeLosses),
                            IncludeLosses,
                            value,
                            onPropertyChanged));
                        IncludeLosses = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("IncludeLosses", lossesCheck, () => lossesCheck.Checked = IncludeLosses);
            yPosition += CheckBoxRowHeight;

            // Total Sales checkbox - label should reflect transaction type
            text = TransactionType == TransactionType.Revenue
                ? LanguageManager.TranslateString("Show Revenue")
                : LanguageManager.TranslateString("Show Expenses");

            Guna2CustomCheckBox salesCheck = AddPropertyCheckBoxWithLabel(container, text, ShowTotalSales, yPosition,
                value =>
                {
                    if (ShowTotalSales != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowTotalSales),
                            ShowTotalSales,
                            value,
                            onPropertyChanged));
                        ShowTotalSales = value;
                        onPropertyChanged();
                    }
                });

            // Find the label that was just added (it's added right after the checkbox)
            Label salesLabel = container.Controls.OfType<Label>()
                .FirstOrDefault(l => l.Location.Y == yPosition);

            CacheControl("ShowTotalSales", salesCheck, () => salesCheck.Checked = ShowTotalSales);
            if (salesLabel != null)
            {
                CacheControl("ShowTotalSalesLabel", salesLabel, () =>
                {
                    salesLabel.Text = TransactionType == TransactionType.Revenue
                        ? LanguageManager.TranslateString("Show Revenue")
                        : LanguageManager.TranslateString("Show Expenses");
                });
            }
            yPosition += CheckBoxRowHeight;

            // Total Transactions checkbox
            text = LanguageManager.TranslateString("Show Transactions");
            Guna2CustomCheckBox transactionsCheck = AddPropertyCheckBoxWithLabel(container, text, ShowTotalTransactions, yPosition,
                value =>
                {
                    if (ShowTotalTransactions != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowTotalTransactions),
                            ShowTotalTransactions,
                            value,
                            onPropertyChanged));
                        ShowTotalTransactions = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowTotalTransactions", transactionsCheck, () => transactionsCheck.Checked = ShowTotalTransactions);
            yPosition += CheckBoxRowHeight;

            // Average Value checkbox
            text = LanguageManager.TranslateString("Show Average Value");
            Guna2CustomCheckBox avgCheck = AddPropertyCheckBoxWithLabel(container, text, ShowAverageValue, yPosition,
                value =>
                {
                    if (ShowAverageValue != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowAverageValue),
                            ShowAverageValue,
                            value,
                            onPropertyChanged));
                        ShowAverageValue = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowAverageValue", avgCheck, () => avgCheck.Checked = ShowAverageValue);
            yPosition += CheckBoxRowHeight;

            // Growth Rate checkbox
            text = LanguageManager.TranslateString("Show Growth Rate");
            Guna2CustomCheckBox growthCheck = AddPropertyCheckBoxWithLabel(container, text, ShowGrowthRate, yPosition,
                value =>
                {
                    if (ShowGrowthRate != value)
                    {
                        undoRedoManager?.RecordAction(new PropertyChangeAction(
                            this,
                            nameof(ShowGrowthRate),
                            ShowGrowthRate,
                            value,
                            onPropertyChanged));
                        ShowGrowthRate = value;
                        onPropertyChanged();
                    }
                });
            CacheControl("ShowGrowthRate", growthCheck, () => growthCheck.Checked = ShowGrowthRate);
            yPosition += CheckBoxRowHeight + 10;

            return yPosition;
        }
    }
}