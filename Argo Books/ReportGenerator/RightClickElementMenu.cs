using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using Argo_Books.ReportGenerator.Menus;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Manages the right-click context menu for report elements in the layout designer.
    /// </summary>
    internal static class RightClickElementMenu
    {
        // Properties
        public static Guna2Panel Panel { get; private set; }
        private static Guna2Button _delete_Button;
        private static Guna2Button _duplicate_Button;
        private static Guna2Button _selectAll_Button;
        private static Guna2Button _bringToFront_Button;
        private static Guna2Button _sendToBack_Button;

        // Constructor
        public static void Construct()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(CustomControls.PanelButtonHeight * scale);
            int scaledSpaceForPanel = (int)(CustomControls.SpaceForPanel * scale);

            int calculatedHeight = 5 * scaledButtonHeight + scaledSpaceForPanel;

            Panel = CustomControls.ConstructPanelForMenu(
                new Size(CustomControls.PanelWidth, calculatedHeight),
                "rightClickElement_Panel");

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)Panel.Controls[0];

            // Duplicate button
            _duplicate_Button = CustomControls.ConstructBtnForMenu("Duplicate", CustomControls.PanelBtnWidth, flowPanel);
            _duplicate_Button.Click += DuplicateElement;
            CustomControls.ConstructKeyShortcut("Ctrl+D", _duplicate_Button);

            // Select All button
            _selectAll_Button = CustomControls.ConstructBtnForMenu("Select all", CustomControls.PanelBtnWidth, flowPanel);
            _selectAll_Button.Click += SelectAllElements;
            CustomControls.ConstructKeyShortcut("Ctrl+A", _selectAll_Button);

            // Bring to Front button
            _bringToFront_Button = CustomControls.ConstructBtnForMenu("Bring to front", CustomControls.PanelBtnWidth, flowPanel);
            _bringToFront_Button.Click += BringToFront;

            // Send to Back button
            _sendToBack_Button = CustomControls.ConstructBtnForMenu("Send to back", CustomControls.PanelBtnWidth, flowPanel);
            _sendToBack_Button.Click += SendToBack;

            // Delete button
            _delete_Button = CustomControls.ConstructBtnForMenu("Delete", CustomControls.PanelBtnWidth, flowPanel);
            _delete_Button.ForeColor = CustomColors.AccentRed;
            _delete_Button.Click += DeleteElement;
            CustomControls.ConstructKeyShortcut("Del", _delete_Button);
        }

        // Event handlers
        private static void DuplicateElement(object sender, EventArgs e)
        {
            ReportLayoutDesigner_Form.Instance.DuplicateSelected();
            Hide();
        }
        private static void DeleteElement(object sender, EventArgs e)
        {
            ReportLayoutDesigner_Form.Instance.DeleteSelected();
            Hide();
        }
        private static void SelectAllElements(object sender, EventArgs e)
        {
            ReportLayoutDesigner_Form.Instance.SelectAllElements();
            Hide();
        }
        private static void BringToFront(object sender, EventArgs e)
        {
            ReportLayoutDesigner_Form.Instance.BringElementToFront();
            Hide();
        }
        private static void SendToBack(object sender, EventArgs e)
        {
            ReportLayoutDesigner_Form.Instance.SendElementToBack();
            Hide();
        }

        // Public methods
        /// <summary>
        /// Shows the right-click menu at the specified location.
        /// </summary>
        public static void Show(Point location, Form parentForm, bool hasSelection)
        {
            if (Panel == null)
            {
                Construct();
            }

            FlowLayoutPanel flowPanel = (FlowLayoutPanel)Panel.Controls[0];
            flowPanel.Controls.Clear();

            // Add buttons based on selection state
            if (hasSelection)
            {
                flowPanel.Controls.Add(_selectAll_Button);
                flowPanel.Controls.Add(_duplicate_Button);
                flowPanel.Controls.Add(_bringToFront_Button);
                flowPanel.Controls.Add(_sendToBack_Button);
                flowPanel.Controls.Add(_delete_Button);
            }
            else
            {
                flowPanel.Controls.Add(_selectAll_Button);
            }

            CustomControls.SetRightClickMenuHeight(Panel);

            // Adjust position to keep menu within form boundaries
            Point adjustedLocation = new(
                Math.Max(0, Math.Min(location.X, parentForm.ClientSize.Width - Panel.Width)),
                Math.Max(0, Math.Min(location.Y, parentForm.ClientSize.Height - Panel.Height))
            );

            // Position the menu
            Panel.Location = adjustedLocation;

            // Add to parent form
            parentForm.Controls.Add(Panel);
            Panel.BringToFront();
        }

        /// <summary>
        /// Hides the right-click menu.
        /// </summary>
        public static void Hide()
        {
            Panel?.Parent?.Controls.Remove(Panel);
        }
    }
}