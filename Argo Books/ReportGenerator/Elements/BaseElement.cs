using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.ReportGenerator;
using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Enums;
using Argo_Books.Language;
using Argo_Books.ReportGenerator.Menus;

namespace Argo_Books.ReportGenerator.Elements
{
    /// <summary>
    /// Base class for all report elements.
    /// </summary>
    public abstract class BaseElement
    {
        // Properties
        private const byte leftMargin = 10;
        public static byte RightMargin { get; } = 40;
        public static byte NumericUpDownWidth { get; } = 100;
        public static byte ColorPickerWidth { get; } = 50;
        public abstract byte MinimumSize { get; }

        // Debounce timers for text input controls
        private static readonly Dictionary<Control, (System.Windows.Forms.Timer timer, string lastValue)> _textInputDebouncers = [];

        /// <summary>
        /// Gets the user-friendly display name for this element type.
        /// </summary>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Unique identifier for the element.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Position and size of the element on the report canvas.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Z-order for layering elements.
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// Whether the element is visible in the report.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets the height of each row for the element in the designer.
        /// </summary>
        public static byte ControlRowHeight { get; } = 55;
        public static byte CheckBoxRowHeight { get; } = 40;
        public static byte ControlHeight { get; } = 45;

        // Abstract methods
        /// <summary>
        /// Gets the element type.
        /// </summary>
        public abstract ReportElementType GetElementType();

        /// <summary>
        /// Creates a copy of this element.
        /// </summary>
        public abstract BaseElement Clone();

        /// <summary>
        /// Renders the element to a graphics context for the report.
        /// </summary>
        public abstract void RenderElement(Graphics graphics, ReportConfiguration config, float renderScale);

        /// <summary>
        /// Renders an error message centered within the element's bounds.
        /// Used as a fallback when element rendering fails.
        /// </summary>
        protected void RenderError(Graphics graphics)
        {
            string message = LanguageManager.TranslateString("Error rendering element");

            using Font font = new("Arial", 10f);
            using SolidBrush brush = new(Color.Red);
            using StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            graphics.DrawString(message, font, brush, Bounds, format);
        }

        public Panel CachedPropertyPanel { get; private set; }
        private bool _controlsCreated = false;
        private readonly Dictionary<string, Control> _controlCache = [];
        public Dictionary<string, Action> UpdateActionCache { get; } = [];

        /// <summary>
        /// Creates or retrieves cached property controls. Only creates controls once.
        /// </summary>
        public virtual int CreatePropertyControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            // Create controls only once
            if (!_controlsCreated)
            {
                // Set size first so child controls can calculate their positions correctly
                int panelWidth = container.Width - 5;

                CachedPropertyPanel = new Panel
                {
                    Size = new Size(panelWidth, container.Height),
                    AutoScroll = true,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
                };

                // Create common controls if element doesn't handle its own
                if (!HandlesOwnCommonControls)
                {
                    Dictionary<string, Control> commonControls = CreateCommonPropertyControls(
                        CachedPropertyPanel,
                        this,
                        yPosition,
                        onPropertyChanged,
                        out Dictionary<string, Action> commonUpdateActions);

                    foreach (KeyValuePair<string, Control> kvp in commonControls)
                    {
                        _controlCache[kvp.Key] = kvp.Value;
                    }
                    foreach (KeyValuePair<string, Action> kvp in commonUpdateActions)
                    {
                        UpdateActionCache[kvp.Key] = kvp.Value;
                    }

                    yPosition += ControlRowHeight * 4;
                }

                // Let derived classes create their specific controls
                yPosition = CreateElementSpecificControls(CachedPropertyPanel, yPosition, onPropertyChanged);

                _controlsCreated = true;
            }

            // Only add CachedPropertyPanel if it's not already in the container
            if (CachedPropertyPanel != null && CachedPropertyPanel.Parent != container)
            {
                List<Control> controlsToRemove = container.Controls
                   .Cast<Control>()
                   .Where(c => c != LoadingPanel.BlankLoadingPanelInstance)
                   .ToList();

                foreach (Control ctrl in controlsToRemove)
                {
                    container.Controls.Remove(ctrl);
                }

                // Update panel size in case container size changed
                int panelWidth = container.Width > 0 ? container.Width - 5 : 340;
                CachedPropertyPanel.Size = new Size(panelWidth, container.Height);
                container.Controls.Add(CachedPropertyPanel);
            }

            UpdateAllControlValues();

            return yPosition;
        }

        /// <summary>
        /// Creates element-specific property controls. Override this in derived classes.
        /// Store controls and update actions in _controlCache and _updateActionCache.
        /// </summary>
        protected virtual int CreateElementSpecificControls(Panel container, int yPosition, Action onPropertyChanged)
        {
            return yPosition;
        }

        /// <summary>
        /// Updates all cached control values without recreating them.
        /// </summary>
        public void UpdateAllControlValues()
        {
            foreach (Action updateAction in UpdateActionCache.Values)
            {
                updateAction?.Invoke();
            }
        }

        /// <summary>
        /// Adds a control and its update action to the cache.
        /// </summary>
        protected void CacheControl(string key, Control control, Action updateAction)
        {
            _controlCache[key] = control;
            if (updateAction != null)
            {
                UpdateActionCache[key] = updateAction;
            }
        }

        /// <summary>
        /// Clears the cached property panel, forcing recreation on next CreatePropertyControls call.
        /// Used when language changes to recreate controls with new translations.
        /// </summary>
        public void ClearPropertyControlCache()
        {
            _controlsCreated = false;
            CachedPropertyPanel?.Dispose();
            CachedPropertyPanel = null;
            _controlCache.Clear();
            UpdateActionCache.Clear();
        }

        /// <summary>
        /// Indicates if element handles its own common controls.
        /// </summary>
        public virtual bool HandlesOwnCommonControls => false;

        public static Dictionary<string, Control> CreateCommonPropertyControls(
            Panel container,
            BaseElement element,
            int yPosition,
            Action onPropertyChanged,
            out Dictionary<string, Action> updateActions)
        {
            Dictionary<string, Control> controls = [];
            updateActions = [];
            string text;

            // Get the UndoRedoManager instance
            UndoRedoManager undoRedoManager = ReportLayoutDesigner_Form.Instance.GetUndoRedoManager();

            // X position
            AddPropertyLabel(container, "X:", yPosition, false, NumericUpDownWidth);
            int oldX = element?.Bounds.X ?? 0;
            Guna2NumericUpDown xNumeric = AddPropertyNumericUpDown(container, oldX, yPosition,
                value =>
                {
                    if (element != null)
                    {
                        Rectangle oldBounds = element.Bounds;
                        Rectangle newBounds = element.Bounds;
                        newBounds.X = (int)value;

                        // Record undo action if value actually changed
                        if (oldBounds.X != newBounds.X)
                        {
                            undoRedoManager.RecordAction(new PropertyChangeAction(
                                element,
                                nameof(Bounds),
                                oldBounds,
                                newBounds,
                                () =>
                                {
                                    container.Invalidate();
                                    onPropertyChanged();
                                }));

                            element.Bounds = newBounds;
                            container.Invalidate();
                            onPropertyChanged();
                        }
                    }
                }, 0, 9999);
            controls["X"] = xNumeric;
            updateActions["X"] = () => xNumeric.Value = element?.Bounds.X ?? 0;
            yPosition += ControlRowHeight;

            // Y position
            AddPropertyLabel(container, "Y:", yPosition, false, NumericUpDownWidth);
            int oldY = element?.Bounds.Y ?? 0;
            Guna2NumericUpDown yNumeric = AddPropertyNumericUpDown(container, oldY, yPosition,
                value =>
                {
                    if (element != null)
                    {
                        Rectangle oldBounds = element.Bounds;
                        Rectangle newBounds = element.Bounds;
                        newBounds.Y = (int)value;

                        if (oldBounds.Y != newBounds.Y)
                        {
                            undoRedoManager.RecordAction(new PropertyChangeAction(
                                element,
                                nameof(Bounds),
                                oldBounds,
                                newBounds,
                                () =>
                                {
                                    container.Invalidate();
                                    onPropertyChanged();
                                }));

                            element.Bounds = newBounds;
                            container.Invalidate();
                            onPropertyChanged();
                        }
                    }
                }, 0, 9999);
            controls["Y"] = yNumeric;
            updateActions["Y"] = () => yNumeric.Value = element?.Bounds.Y ?? 0;
            yPosition += ControlRowHeight;

            // Width
            text = LanguageManager.TranslateString("Width");
            AddPropertyLabel(container, text + ":", yPosition, false, NumericUpDownWidth);
            int oldWidth = element?.Bounds.Width ?? 100;
            Guna2NumericUpDown widthNumeric = AddPropertyNumericUpDown(container, oldWidth, yPosition,
                value =>
                {
                    if (element != null)
                    {
                        Rectangle oldBounds = element.Bounds;
                        Rectangle newBounds = element.Bounds;
                        newBounds.Width = (int)value;

                        if (oldBounds.Width != newBounds.Width)
                        {
                            undoRedoManager.RecordAction(new PropertyChangeAction(
                                element,
                                nameof(Bounds),
                                oldBounds,
                                newBounds,
                                () =>
                                {
                                    container.Invalidate();
                                    onPropertyChanged();
                                }));

                            element.Bounds = newBounds;
                            container.Invalidate();
                            onPropertyChanged();
                        }
                    }
                }, 10, 9999);
            controls["Width"] = widthNumeric;
            updateActions["Width"] = () => widthNumeric.Value = element?.Bounds.Width ?? 100;
            yPosition += ControlRowHeight;

            // Height
            text = LanguageManager.TranslateString("Height");
            AddPropertyLabel(container, text + ":", yPosition, false, NumericUpDownWidth);
            int oldHeight = element?.Bounds.Height ?? 100;
            Guna2NumericUpDown heightNumeric = AddPropertyNumericUpDown(container, oldHeight, yPosition,
                value =>
                {
                    if (element != null)
                    {
                        Rectangle oldBounds = element.Bounds;
                        Rectangle newBounds = element.Bounds;
                        newBounds.Height = (int)value;

                        if (oldBounds.Height != newBounds.Height)
                        {
                            undoRedoManager.RecordAction(new PropertyChangeAction(
                                element,
                                nameof(Bounds),
                                oldBounds,
                                newBounds,
                                () =>
                                {
                                    container.Invalidate();
                                    onPropertyChanged();
                                }));

                            element.Bounds = newBounds;
                            container.Invalidate();
                            onPropertyChanged();
                        }
                    }
                }, 10, 9999);
            controls["Height"] = heightNumeric;
            updateActions["Height"] = () => heightNumeric.Value = element?.Bounds.Height ?? 100;

            return controls;
        }

        /// <summary>
        /// Adds a property label to the container with overflow handling.
        /// </summary>
        public static Label AddPropertyLabel(Panel container, string text, int yPosition, bool bold = false, int controlWidth = 170)
        {
            int maxLabelWidth = container.ClientSize.Width - RightMargin - controlWidth;

            Font labelFont = new("Segoe UI", 9, bold ? FontStyle.Bold : FontStyle.Regular);

            // Measure text with current font
            using (Graphics g = container.CreateGraphics())
            {
                Size textSize = TextRenderer.MeasureText(text, labelFont);

                // If text is too wide, try smaller font first
                if (textSize.Width + 10 > maxLabelWidth)
                {
                    labelFont = new Font("Segoe UI", 8, bold ? FontStyle.Bold : FontStyle.Regular);
                    textSize = TextRenderer.MeasureText(text, labelFont);
                    maxLabelWidth = textSize.Width;
                }
                else
                {
                    // Set the label width to the text size if it fits
                    maxLabelWidth = textSize.Width;
                }
            }

            Label label = new()
            {
                Text = text,
                Font = labelFont,
                ForeColor = CustomColors.Text,
                Location = new Point(leftMargin, yPosition + 8),
                Size = new Size(maxLabelWidth, 24),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleLeft
            };

            container.Controls.Add(label);
            return label;
        }

        /// <summary>
        /// Adds a text box property control aligned to the right.
        /// </summary>
        public static Guna2TextBox AddPropertyTextBox(Panel container, string value, int yPosition, Action<string> onChange)
        {
            const int controlWidth = 170;
            int xPosition = container.ClientSize.Width - leftMargin - controlWidth;

            Guna2TextBox textBox = new()
            {
                Text = value,
                Size = new Size(controlWidth, ControlHeight),
                Location = new Point(xPosition, yPosition),
                BorderRadius = 2,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Set up debounced text change handling
            SetupDebouncedTextChanged(textBox, value, onChange);

            container.Controls.Add(textBox);
            return textBox;
        }

        /// <summary>
        /// Adds a numeric up/down property control aligned to the right.
        /// </summary>
        public static Guna2NumericUpDown AddPropertyNumericUpDown(Panel container, decimal value, int yPosition, Action<decimal> onChange, decimal min = 0, decimal max = 9999)
        {
            int xPosition = container.ClientSize.Width - leftMargin - NumericUpDownWidth;

            Guna2NumericUpDown numericUpDown = new()
            {
                Size = new Size(NumericUpDownWidth, ControlHeight),
                Location = new Point(xPosition, yPosition),
                BorderRadius = 2,
                Font = new Font("Segoe UI", 9),
                Minimum = min,
                Maximum = max,
                Value = value,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            numericUpDown.KeyDown += (s, e) =>
            {
                // Remove Windows "ding" noise when user presses enter
                e.SuppressKeyPress = e.KeyCode == Keys.Enter;
            };

            numericUpDown.DisableScrollAndForwardToPanel();

            numericUpDown.ValueChanged += (s, e) => onChange(numericUpDown.Value);
            container.Controls.Add(numericUpDown);
            return numericUpDown;
        }

        /// <summary>
        /// Adds a combo box property control aligned to the right.
        /// </summary>
        protected static Guna2ComboBox AddPropertyComboBox(Panel container, string value, int yPosition, string[] items, Action<string> onChange)
        {
            const int controlWidth = 170;
            int xPosition = container.ClientSize.Width - leftMargin - controlWidth;

            Guna2ComboBox comboBox = new()
            {
                Size = new Size(controlWidth, ControlHeight),
                ItemHeight = 39,  // Needed to make the height equal to ControlHeight
                Location = new Point(xPosition, yPosition),
                BorderRadius = 2,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            comboBox.Items.AddRange(items);
            comboBox.SelectedItem = value;
            comboBox.SelectedIndexChanged += (s, e) =>
            {
                if (comboBox.SelectedItem != null)
                {
                    onChange(comboBox.SelectedItem.ToString());
                }
            };

            comboBox.DisableScrollAndForwardToPanel();

            container.Controls.Add(comboBox);
            return comboBox;
        }

        /// <summary>
        /// Adds a checkbox with an accompanying text label that can be clicked to toggle the checkbox.
        /// </summary>
        public static Guna2CustomCheckBox AddPropertyCheckBoxWithLabel(
            Panel container,
            string labelText,
            bool isChecked,
            int yPosition,
            Action<bool> onChange)
        {
            // Add the checkbox
            Guna2CustomCheckBox checkBox = new()
            {
                Location = new Point(leftMargin, yPosition + 6),
                Size = new Size(22, 22),
                Animated = true,
                Checked = isChecked  // This must be set after 'Animated' because Guna is bugged
            };

            checkBox.CheckedChanged += (s, e) => onChange(checkBox.Checked);
            container.Controls.Add(checkBox);

            // Add the label after the checkbox
            Label label = new()
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9),
                ForeColor = CustomColors.Text,
                Location = new Point(checkBox.Right + 5, yPosition),
                Padding = new Padding(5),
                AutoSize = true
            };

            // Clicking the label toggles the checkbox
            label.Click += (s, e) => { checkBox.Checked = !checkBox.Checked; };
            container.Controls.Add(label);

            return checkBox;
        }

        public Panel Tab_Panel { get; private set; }
        public static readonly string ColorPickerTag = "ColorPicker";

        /// <summary>
        /// Adds a color picker control with an optional label, aligned to the right.
        /// </summary>
        protected static Panel AddColorPicker(
            Panel container,
            int yPosition,
            Color currentColor,
            Action<Color> onColorChanged)
        {
            int xPosition = container.ClientSize.Width - leftMargin - ColorPickerWidth;

            Panel colorPreview = new()
            {
                BackColor = currentColor,
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(ColorPickerWidth, 30),
                Location = new Point(xPosition, yPosition + 8),
                Cursor = Cursors.Hand,
                Tag = ColorPickerTag,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            colorPreview.Click += (s, e) =>
            {
                ColorDialog colorDialog = new()
                {
                    Color = colorPreview.BackColor,
                    FullOpen = true
                };

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorPreview.BackColor = colorDialog.Color;
                    onColorChanged(colorDialog.Color);
                }
            };

            container.Controls.Add(colorPreview);

            return colorPreview;
        }

        /// <summary>
        /// Adds a searchable text box property control with dynamic width.
        /// </summary>
        protected static Guna2TextBox AddPropertySearchBox(
            Panel container,
            string value,
            int yPosition,
            Func<List<SearchResult>> getSearchResults,
            Action<string> onChange,
            Label label)
        {
            int xPosition;
            int controlWidth;

            // Dynamic width based on label
            xPosition = label.Right + 10;
            controlWidth = container.ClientSize.Width - xPosition - leftMargin;

            Guna2TextBox textBox = new()
            {
                Text = value,
                Size = new Size(controlWidth, ControlHeight),
                Location = new Point(xPosition, yPosition),
                BorderRadius = 2,
                Font = new Font("Segoe UI", 9),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Only trigger onChange when the value is valid (matches a search result)
            textBox.TextChanged += (s, e) =>
            {
                string currentValue = textBox.Text;
                List<SearchResult> validResults = getSearchResults();
                bool isValid = validResults.Any(r => r.Name.Equals(currentValue, StringComparison.OrdinalIgnoreCase));

                // Only call onChange if the value is valid
                if (isValid)
                {
                    onChange(currentValue);
                }
            };

            textBox.DisableScrollAndForwardToPanel();

            // Attach SearchBox functionality
            SearchBox.Attach(textBox, container, getSearchResults, 255, false, true, false, true,
                container.ClientSize.Width - leftMargin - RightMargin + 4,
                leftMargin);

            container.Controls.Add(textBox);
            return textBox;
        }
        /// <summary>
        /// Sets up debounced text change handling for a textbox to prevent recording undo on every keystroke.
        /// This is used for regular text boxes (not searchboxes).
        /// </summary>
        private static void SetupDebouncedTextChanged(Guna2TextBox textBox, string initialValue, Action<string> onChange)
        {
            string lastCommittedValue = initialValue;

            // Create debounce timer for this control
            System.Windows.Forms.Timer debounceTimer = new()
            {
                Interval = 500  // 500ms delay - same as PropertyChangeDebouncer
            };

            debounceTimer.Tick += (s, e) =>
            {
                debounceTimer.Stop();
                string currentValue = textBox.Text;

                // Call onChange if value changed
                if (currentValue != lastCommittedValue)
                {
                    lastCommittedValue = currentValue;
                    onChange(currentValue);
                }
            };

            // Store the timer and last value
            _textInputDebouncers[textBox] = (debounceTimer, lastCommittedValue);

            // Handle text changes
            textBox.TextChanged += (s, e) =>
            {
                debounceTimer.Stop();
                debounceTimer.Start();
            };

            // Handle focus loss - commit changes immediately when user leaves the field
            textBox.LostFocus += (s, e) =>
            {
                debounceTimer.Stop();
                string currentValue = textBox.Text;

                if (_textInputDebouncers.TryGetValue(textBox, out var debouncer))
                {
                    string lastValue = debouncer.lastValue;

                    // Call onChange if value changed
                    if (currentValue != lastValue)
                    {
                        _textInputDebouncers[textBox] = (debouncer.timer, currentValue);
                        onChange(currentValue);
                    }
                }
            };

            // Clean up timer when control is disposed
            textBox.Disposed += (s, e) =>
            {
                if (_textInputDebouncers.TryGetValue(textBox, out var debouncer))
                {
                    debouncer.timer.Stop();
                    debouncer.timer.Dispose();
                    _textInputDebouncers.Remove(textBox);
                }
            };
        }

        public static List<SearchResult> GetFontSearchResults()
        {
            string[] fonts = ["Arial", "Calibri", "Cambria", "Comic Sans MS", "Consolas",
                      "Courier New", "Georgia", "Impact", "Segoe UI", "Tahoma",
                      "Times New Roman", "Trebuchet MS", "Verdana"];

            return fonts.Select(font => new SearchResult(font, null, 0)).ToList();
        }

        /// <summary>
        /// Creates tab buttons for organizing properties.
        /// </summary>
        protected Panel CreateTabButtons(string[] tabNames, Action<int> onTabChanged)
        {
            int parentWidth = ReportLayoutDesigner_Form.Instance.PropertiesContainer_Panel.Width;

            Tab_Panel = new()
            {
                Location = new Point(0, 0),
                Size = new Size(parentWidth, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            int buttonWidth = (parentWidth - leftMargin) / tabNames.Length;
            for (int i = 0; i < tabNames.Length; i++)
            {
                int tabIndex = i;
                Guna2Button tabButton = new()
                {
                    Text = tabNames[i],
                    Size = new Size(buttonWidth - 2, 30),
                    Location = new Point(5 + (i * buttonWidth), 2),
                    BorderRadius = 4,
                    BorderThickness = 1,
                    Font = new Font("Segoe UI", 9),
                    Tag = i,
                    FillColor = i == 0 ? CustomColors.AccentBlue : CustomColors.ControlBack,  // Select general button by default
                    BorderColor = i == 0 ? CustomColors.AccentBlue : CustomColors.ControlBorder
                };

                tabButton.Click += (s, e) =>
                {
                    foreach (Control control in Tab_Panel.Controls)
                    {
                        if (control is Guna2Button btn)
                        {
                            if (btn.Tag is int tagValue)
                            {
                                btn.FillColor = tagValue == tabIndex ? CustomColors.AccentBlue : CustomColors.ControlBack;
                                btn.BorderColor = tagValue == tabIndex ? CustomColors.AccentBlue : CustomColors.ControlBorder;
                                btn.ForeColor = tagValue == tabIndex ? Color.White : CustomColors.Text;
                            }
                        }
                    }
                    onTabChanged(tabIndex);
                };

                Tab_Panel.Controls.Add(tabButton);
            }

            CachedPropertyPanel.Controls.Add(Tab_Panel);
            return Tab_Panel;
        }
        public static Guna2Button CreateFontStyleButton(
            string text,
            FontStyle fontStyle,
            int x,
            int y,
            int width,
            int height)
        {
            Guna2Button button = new()
            {
                Size = new Size(width, height),
                Location = new Point(x, y),
                Text = text,
                Font = new Font("Segoe UI", 9, fontStyle),
                BorderRadius = 2,
                ButtonMode = ButtonMode.ToogleButton,
                CheckedState =
                {
                    FillColor = CustomColors.AccentBlue,
                    ForeColor = Color.White
                }
            };

            return button;
        }
    }
}