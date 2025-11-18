using Sales_Tracker.Language;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.ComponentModel;

namespace Sales_Tracker.ReportGenerator.Menus
{
    /// <summary>
    /// Dialog for saving a custom report template.
    /// </summary>
    public partial class SaveTemplate_Form : Form
    {
        // Properties
        public string TemplateName { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CurrentTemplateName { get; set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRenameMode { get; set; }

        // Init.
        public SaveTemplate_Form()
        {
            InitializeComponent();

            UpdateTheme();
            AddEventHandlers();
            TextBoxManager.Attach(TemplateName_TextBox);
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void AddEventHandlers()
        {
            UpdateExisting_RadioButton.Click += (_, _) => UpdateExisting_RadioButton.Checked = true;
            SaveAsNew_RadioButton.Click += (_, _) => SaveAsNew_RadioButton.Checked = true;
        }
        private void UpdateTheme()
        {
            ThemeManager.MakeGButtonBluePrimary(Save_Button);
            ThemeManager.SetThemeForForm(this);
        }

        // Form event handlers
        private void SaveTemplate_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);

            // If in rename mode, only show the template name input
            if (IsRenameMode)
            {
                UpdateExisting_RadioButton.Visible = false;
                UpdateExisting_Label.Visible = false;
                SaveAsNew_RadioButton.Visible = false;
                SaveAsNew_Label.Visible = false;
                TemplateName_TextBox.Text = CurrentTemplateName;
                TemplateName_TextBox.Enabled = true;
                CenterTemplateNameTextBoxAndLabel();
                Text = LanguageManager.TranslateString("Rename Template");
            }
            // If there's a current template, show the update option
            else if (!string.IsNullOrEmpty(CurrentTemplateName))
            {
                UpdateExisting_RadioButton.Visible = true;
                UpdateExisting_Label.Visible = true;
                SaveAsNew_RadioButton.Visible = true;
                SaveAsNew_Label.Visible = true;
                UpdateExisting_Label.Text = LanguageManager.TranslateString("Update existing template") + ": '" + CurrentTemplateName + "'";
                UpdateExisting_RadioButton.Checked = true;
                SaveAsNew_RadioButton.Checked = false;
                TemplateName_TextBox.Text = CurrentTemplateName;
                TemplateName_TextBox.Enabled = false;
            }
            else
            {
                UpdateExisting_RadioButton.Visible = false;
                UpdateExisting_Label.Visible = false;
                SaveAsNew_RadioButton.Visible = false;
                SaveAsNew_Label.Visible = false;
                CenterTemplateNameTextBoxAndLabel();
                TemplateName_TextBox.Enabled = true;
            }

            TemplateName_TextBox.Focus();
        }
        private void CenterTemplateNameTextBoxAndLabel()
        {
            TemplateName_TextBox.Top = (ClientSize.Height - TemplateName_TextBox.Height) / 2;
            TemplateName_Label.Top = TemplateName_TextBox.Top - TemplateName_Label.Height - 5;
        }
        // Event handlers
        private void UpdateExisting_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (UpdateExisting_RadioButton.Checked)
            {
                TemplateName_TextBox.Text = CurrentTemplateName;
                TemplateName_TextBox.Enabled = false;
            }
        }
        private void SaveAsNew_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (SaveAsNew_RadioButton.Checked)
            {
                TemplateName_TextBox.Text = "";
                TemplateName_TextBox.Enabled = true;
                TemplateName_TextBox.Focus();
            }
        }
        private void Save_Button_Click(object sender, EventArgs e)
        {
            // In rename mode, just return the new name
            if (IsRenameMode)
            {
                string templateName = TemplateName_TextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(templateName))
                {
                    CustomMessageBox.Show(
                        "Invalid Template Name",
                        "Please enter a name for the template.",
                        CustomMessageBoxIcon.Exclamation,
                        CustomMessageBoxButtons.Ok);
                    return;
                }

                // Check if trying to use a built-in template name
                if (ReportTemplates.IsBuiltInTemplate(templateName))
                {
                    CustomMessageBox.ShowWithFormat(
                        "Reserved Template Name",
                        "The template name '{0}' is reserved for a built-in template.\nPlease choose a different name.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok,
                        templateName);
                    return;
                }

                TemplateName = templateName;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            // Check if we're updating an existing template
            if (UpdateExisting_RadioButton.Visible && UpdateExisting_RadioButton.Checked)
            {
                TemplateName = CurrentTemplateName;
                DialogResult = DialogResult.OK;
                Close();
                return;
            }

            // Saving as new template
            string newTemplateName = TemplateName_TextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(newTemplateName))
            {
                CustomMessageBox.Show(
                    "Invalid Template Name",
                    "Please enter a name for the template.",
                    CustomMessageBoxIcon.Exclamation,
                    CustomMessageBoxButtons.Ok);
                return;
            }

            // Check if trying to use a built-in template name
            if (ReportTemplates.IsBuiltInTemplate(newTemplateName))
            {
                CustomMessageBox.ShowWithFormat(
                    "Reserved Template Name",
                    "The template name '{0}' is reserved for a built-in template.\nPlease choose a different name.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    newTemplateName);
                return;
            }

            // Check if template already exists
            if (CustomTemplateStorage.TemplateExists(newTemplateName))
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Template Exists",
                    "A template named '{0}' already exists.\nDo you want to overwrite it?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    newTemplateName);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return;
                }
            }

            TemplateName = newTemplateName;
            DialogResult = DialogResult.OK;
            Close();
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void TemplateName_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Save_Button_Click(sender, e);
            }
        }
    }
}
