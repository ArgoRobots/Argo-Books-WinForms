using Argo_Books.ReportGenerator;
using Argo_Books.UI;
using Argo_Books.Language;
using Argo_Books.Theme;

namespace Argo_Books.ReportGenerator.Menus
{
    /// <summary>
    /// Settings form for configuring page layout and display options.
    /// </summary>
    public partial class PageSettings_Form : Form
    {
        // Properties
        private static PageSettings_Form _instance;
        private bool _isUpdating;
        private static ReportConfiguration? ReportConfig => ReportGenerator_Form.Instance.CurrentReportConfiguration;

        // Getters
        public static PageSettings_Form Instance => _instance;

        // Init.
        public PageSettings_Form()
        {
            InitializeComponent();
            _instance = this;

            SetupPageSettings();
            ScaleControls();

            IncludeHeader_Label.Click += (s, e) => IncludeHeader_CheckBox.Checked = !IncludeHeader_CheckBox.Checked;
            IncludeFooter_Label.Click += (s, e) => IncludeFooter_CheckBox.Checked = !IncludeFooter_CheckBox.Checked;
            ShowPageNumbers_Label.Click += (s, e) => ShowPageNumbers_CheckBox.Checked = !ShowPageNumbers_CheckBox.Checked;
            ShowGrid_Label.Click += (s, e) => ShowGrid_CheckBox.Checked = !ShowGrid_CheckBox.Checked;

            UpdateTheme();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void UpdateTheme()
        {
            ThemeManager.MakeGButtonBluePrimary(Close_Button);
            ThemeManager.SetThemeForForm(this);
        }
        private void SetupPageSettings()
        {
            PerformUpdate(() =>
            {
                // Setup page size combo box
                PageSize_ComboBox.Items.Clear();
                PageSize_ComboBox.Items.Add("A4 (210 × 297 mm)");
                PageSize_ComboBox.Items.Add("Letter (8.5 × 11 in)");
                PageSize_ComboBox.Items.Add("Legal (8.5 × 14 in)");
                PageSize_ComboBox.Items.Add("Tabloid (11 × 17 in)");

                // Load from config or default to A4
                PageSize_ComboBox.SelectedIndex = ReportConfig?.PageSize switch
                {
                    PageSize.A4 => 0,
                    PageSize.Letter => 1,
                    PageSize.Legal => 2,
                    PageSize.Tabloid => 3,
                    _ => 0
                };

                // Setup orientation combo box
                PageOrientation_ComboBox.Items.Clear();
                PageOrientation_ComboBox.Items.Add("Portrait");
                PageOrientation_ComboBox.Items.Add("Landscape");

                PageOrientation_ComboBox.SelectedIndex = ReportConfig?.PageOrientation == PageOrientation.Landscape ? 1 : 0;

                // Setup header/footer checkboxes
                IncludeHeader_CheckBox.Checked = ReportConfig?.ShowHeader ?? true;
                IncludeFooter_CheckBox.Checked = ReportConfig?.ShowFooter ?? true;

                // Setup page numbers checkbox and input
                ShowPageNumbers_CheckBox.Checked = ReportConfig?.ShowPageNumbers ?? true;
                PageNumber_NumericUpDown.Value = ReportConfig?.CurrentPageNumber ?? 0;

                // Set initial enabled states based on footer and show page numbers
                ShowPageNumbers_CheckBox.Enabled = IncludeFooter_CheckBox.Checked;
                ShowPageNumbers_Label.Enabled = IncludeFooter_CheckBox.Checked;

                bool enableStartingNumber = IncludeFooter_CheckBox.Checked && ShowPageNumbers_CheckBox.Checked;
                PageNumber_NumericUpDown.Enabled = enableStartingNumber;
                StartingNumber_Label.Enabled = enableStartingNumber;

                ShowGrid_CheckBox.Checked = ReportLayoutDesigner_Form.Instance.ShowGrid;
                GridSize_NumericUpDown.Value = ReportLayoutDesigner_Form.GridSize;
            });
        }
        private void ScaleControls()
        {
            DpiHelper.ScaleComboBox(PageSize_ComboBox);
            DpiHelper.ScaleComboBox(PageOrientation_ComboBox);
        }
        public void AlignControlsAfterLanguageChange()
        {
            Title_Label.Left = (ClientSize.Width - Title_Label.Width) / 2;
        }

        // Form event handlers
        private void PageSettings_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void PageSettings_Changed(object sender, EventArgs e)
        {
            if (_isUpdating || ReportConfig == null) { return; }

            // Update config
            ReportConfig.PageSize = PageSize_ComboBox.SelectedIndex switch
            {
                0 => PageSize.A4,
                1 => PageSize.Letter,
                2 => PageSize.Legal,
                3 => PageSize.Tabloid,
                _ => PageSize.A4
            };

            ReportConfig.PageOrientation = PageOrientation_ComboBox.SelectedIndex == 1
                ? PageOrientation.Landscape
                : PageOrientation.Portrait;

            // Notify parent form to resize canvas and redraw
            ReportLayoutDesigner_Form.Instance.OnPageSettingsChanged();
        }
        private void IncludeHeader_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating || ReportConfig == null) { return; }

            ReportConfig.ShowHeader = IncludeHeader_CheckBox.Checked;

            // Notify parent form to redraw
            ReportLayoutDesigner_Form.Instance.OnPageSettingsChanged();
        }
        private void IncludeFooter_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating || ReportConfig == null) { return; }

            ReportConfig.ShowFooter = IncludeFooter_CheckBox.Checked;

            // Enable/disable page numbers section when footer is toggled
            ShowPageNumbers_CheckBox.Enabled = IncludeFooter_CheckBox.Checked;
            ShowPageNumbers_Label.Enabled = IncludeFooter_CheckBox.Checked;

            // Disable starting number if footer is off, or if show page numbers is off
            bool enableStartingNumber = IncludeFooter_CheckBox.Checked && ShowPageNumbers_CheckBox.Checked;
            PageNumber_NumericUpDown.Enabled = enableStartingNumber;
            StartingNumber_Label.Enabled = enableStartingNumber;

            // Notify parent form to redraw
            ReportLayoutDesigner_Form.Instance.OnPageSettingsChanged();
        }
        private void ShowPageNumbers_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating || ReportConfig == null) { return; }

            ReportConfig.ShowPageNumbers = ShowPageNumbers_CheckBox.Checked;

            // Enable/disable the starting number input (only if footer is also enabled)
            bool enableStartingNumber = ShowPageNumbers_CheckBox.Checked && IncludeFooter_CheckBox.Checked;
            PageNumber_NumericUpDown.Enabled = enableStartingNumber;
            StartingNumber_Label.Enabled = enableStartingNumber;

            // Notify parent form to redraw
            ReportLayoutDesigner_Form.Instance.OnPageSettingsChanged();
        }
        private void PageNumber_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating || ReportConfig == null) { return; }

            ReportConfig.CurrentPageNumber = (int)PageNumber_NumericUpDown.Value;

            // Notify parent form to redraw
            ReportLayoutDesigner_Form.Instance.OnPageSettingsChanged();
        }
        private void ShowGrid_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            ReportLayoutDesigner_Form.Instance.ShowGrid = ShowGrid_CheckBox.Checked;
            ReportLayoutDesigner_Form.Instance.OnGridSettingsChanged();
        }
        private void GridSize_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            ReportLayoutDesigner_Form.GridSize = (int)GridSize_NumericUpDown.Value;
            ReportLayoutDesigner_Form.Instance.OnGridSettingsChanged();
        }
        private void NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Remove Windows "ding" noise when user presses enter
            e.SuppressKeyPress = e.KeyCode == Keys.Enter;
        }
        private void Close_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Helper methods
        /// <summary>
        /// Safely updates UI controls without triggering events.
        /// </summary>
        private void PerformUpdate(Action updateAction)
        {
            if (updateAction == null) { return; }

            _isUpdating = true;
            try
            {
                updateAction();
            }
            finally
            {
                _isUpdating = false;
            }
        }
    }
}