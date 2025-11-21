using Guna.UI2.WinForms;
using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.GridView
{
    /// <summary>
    /// Provides a right-click panel for toggling column visibility in MainMenu_Form DataGridViews.
    /// </summary>
    public static class ColumnVisibilityPanel
    {
        // Properties
        private static Guna2DataGridView _dataGridView;
        private static readonly Dictionary<string, Guna2CustomCheckBox> _checkBoxes = [];
        private static MainMenu_Form.Column[] _currentColumnHeaders = [];
        private static readonly byte _spacingBetweenCheckBoxes = 40, _titleHeight = 60;

        // UI Components
        private static Guna2Button _apply_Button;
        private static Guna2Button _cancel_Button;
        private static Label _title_Label;

        public static Guna2Panel Panel { get; private set; }

        // Init.
        public static void Construct()
        {
            Panel = new Guna2Panel()
            {
                BorderRadius = 4,
                BorderThickness = 1,
                BorderColor = CustomColors.ControlPanelBorder
            };

            // Title Label
            _title_Label = new Label
            {
                Text = LanguageManager.TranslateString("Column visibility"),
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Top = 8,
                Name = "Title_Label"
            };
            Panel.Controls.Add(_title_Label);

            // Apply Button
            _apply_Button = new Guna2Button
            {
                Text = LanguageManager.TranslateString("Apply"),
                Size = new Size(130, 45),
                Font = new Font("Segoe UI", 10)
            };
            _apply_Button.Click += ApplyButton_Click;
            Panel.Controls.Add(_apply_Button);

            // Cancel Button
            _cancel_Button = new Guna2Button
            {
                Text = LanguageManager.TranslateString("Cancel"),
                Size = new Size(130, 45),
                Font = new Font("Segoe UI", 10)
            };
            _cancel_Button.Click += CancelButton_Click;
            Panel.Controls.Add(_cancel_Button);
        }

        // Methods
        public static void Show(Guna2DataGridView dataGridView, DataGridViewCellMouseEventArgs e)
        {
            if (MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Purchases &&
                MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Sales &&
                MainMenu_Form.Instance.Selected != MainMenu_Form.SelectedOption.Rentals)
            {
                return;
            }

            _dataGridView = dataGridView;
            _currentColumnHeaders = GetColumnHeadersForDataGridView();

            DataGridView.HitTestInfo info = _dataGridView.HitTest(e.X, e.Y);
            if (info.Type != DataGridViewHitTestType.ColumnHeader)
            {
                return;
            }

            Form parentForm = _dataGridView.FindForm();
            int formHeight = parentForm.ClientSize.Height;
            int formWidth = parentForm.ClientSize.Width;

            // Convert cell-relative coordinates to DataGridView-relative coordinates
            Rectangle columnHeaderBounds = _dataGridView.GetColumnDisplayRectangle(e.ColumnIndex, false);
            int dataGridViewX = columnHeaderBounds.X + e.X;
            // Y is already correct for column headers since they're at the top

            // Create a new MouseEventArgs with the corrected coordinates
            MouseEventArgs correctedMouseArgs = new(MouseButtons.Right, 1, dataGridViewX, e.Y, 0);

            DataGridViewManager.SetHorizontalPosition(_dataGridView, Panel, correctedMouseArgs, formWidth);
            DataGridViewManager.SetVerticalPosition(_dataGridView, Panel, info, formHeight);

            ConstructColumnCheckBoxes();

            // Dynamically adjust panel size based on number of columns
            int newHeight = Math.Max(250, _titleHeight + _currentColumnHeaders.Length * _spacingBetweenCheckBoxes + 80);  // 80 for buttons and padding
            Panel.Size = new Size(400, newHeight);

            // Adjust control positions
            _title_Label.Left = (Panel.Width - _title_Label.Width) / 2;
            _apply_Button.Location = new Point(Panel.Width - _apply_Button.Width - CustomControls.SpaceBetweenControls, Panel.Height - _apply_Button.Height - CustomControls.SpaceBetweenControls);
            _cancel_Button.Location = new Point(_apply_Button.Left - CustomControls.SpaceBetweenControls - _cancel_Button.Width, _apply_Button.Top);

            ThemeManager.SetThemeForControls([Panel]);
            ThemeManager.MakeGButtonBluePrimary(_apply_Button);
            ThemeManager.MakeGButtonBlueSecondary(_cancel_Button);

            parentForm.Controls.Add(Panel);
            Panel.BringToFront();
        }
        public static void Hide()
        {
            Panel?.Parent?.Controls.Remove(Panel);
        }
        private static MainMenu_Form.Column[] GetColumnHeadersForDataGridView()
        {
            return
            [
                MainMenu_Form.Column.ID,
                MainMenu_Form.Column.Country,
                MainMenu_Form.Column.Company,
                MainMenu_Form.Column.Accountant,
                MainMenu_Form.Column.Note,
                MainMenu_Form.Column.HasReceipt
            ];
        }
        private static void ClearCheckBoxes()
        {
            Panel.Controls.Clear();
            Panel.Controls.Add(_title_Label);
            Panel.Controls.Add(_apply_Button);
            Panel.Controls.Add(_cancel_Button);

            _checkBoxes.Clear();
        }
        private static void ConstructColumnCheckBoxes()
        {
            ClearCheckBoxes();

            int yPosition = _titleHeight;
            byte checkBoxHeight = 20;

            foreach (MainMenu_Form.Column columnHeader in _currentColumnHeaders)
            {
                string columnName = columnHeader.ToString();

                // Find the column to get its display name
                DataGridViewColumn column = DataGridViewManager.FindColumnByName(_dataGridView, columnName);
                if (column == null) { continue; }

                // Use the actual column header text instead of enum name
                string displayName = column.HeaderText;

                // Create checkbox
                Guna2CustomCheckBox checkBox = new()
                {
                    Size = new Size(checkBoxHeight, checkBoxHeight),
                    Location = new Point(20, yPosition),
                    Animated = true
                };
                Panel.Controls.Add(checkBox);

                // Set initial checked state
                checkBox.Checked = column?.Visible ?? true;

                // Create label
                Label label = new()
                {
                    Text = displayName,
                    Font = new Font("Segoe UI", 10),
                    Left = 45,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                label.Click += (sender, e) =>
                {
                    checkBox.Checked = !checkBox.Checked;
                };
                Panel.Controls.Add(label);
                label.Top = yPosition + checkBoxHeight / 2 - label.Height / 2;

                _checkBoxes[columnName] = checkBox;

                yPosition += _spacingBetweenCheckBoxes;
            }
        }

        // Event Handlers
        private static void ApplyButton_Click(object sender, EventArgs e)
        {
            foreach (MainMenu_Form.Column columnHeader in _currentColumnHeaders)
            {
                string columnName = columnHeader.ToString();

                if (_checkBoxes.TryGetValue(columnName, out Guna2CustomCheckBox? value))
                {
                    DataGridViewColumn column = DataGridViewManager.FindColumnByName(_dataGridView, columnName);
                    if (column != null)
                    {
                        column.Visible = value.Checked;
                    }
                }
            }

            Hide();
        }
        private static void CancelButton_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}