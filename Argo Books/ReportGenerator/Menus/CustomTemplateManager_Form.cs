using Argo_Books.Classes;
using Argo_Books.ReportGenerator;
using Argo_Books.Theme;
using Argo_Books.UI;
using Argo_Books.Classes;
using Argo_Books.Language;
using Argo_Books.Theme;

namespace Argo_Books.ReportGenerator.Menus
{
    /// <summary>
    /// Form for managing custom report templates.
    /// </summary>
    public partial class CustomTemplateManager_Form : Form
    {
        // Properties
        private List<string> _templateNames = [];
        public string SelectedTemplateName { get; private set; }

        // Init.
        public CustomTemplateManager_Form()
        {
            InitializeComponent();

            UpdateTheme();
            LoadTemplates();
            UpdateButtonStates();
            LanguageManager.UpdateLanguageForControl(this);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void UpdateTheme()
        {
            ThemeManager.MakeGButtonBluePrimary(Load_Button);
            ThemeManager.SetThemeForForm(this);
        }
        private void LoadTemplates()
        {
            _templateNames = CustomTemplateStorage.GetCustomTemplateNames();

            Templates_DataGridView.Rows.Clear();
            foreach (string templateName in _templateNames)
            {
                Templates_DataGridView.Rows.Add(templateName);
            }

            if (_templateNames.Count == 0)
            {
                NoTemplates_Label.Visible = true;
            }
            else
            {
                NoTemplates_Label.Visible = false;
            }
            NoTemplates_Label.BackColor = CustomColors.ControlBack;
            NoTemplates_Label.ForeColor = CustomColors.Text;
        }
        private void UpdateButtonStates()
        {
            int selectedCount = Templates_DataGridView.SelectedRows.Count;

            // Load, Rename, and Export only work with a single selection
            Load_Button.Enabled = selectedCount == 1;
            Rename_Button.Enabled = selectedCount == 1;
            Export_Button.Enabled = selectedCount == 1;

            // Delete works with one or more selections
            Delete_Button.Enabled = selectedCount > 0;
        }

        // Form event handlers
        private void CustomTemplateManager_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Templates_DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
        }
        private void Templates_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Load_Button_Click(sender, e);
            }
        }
        private void Load_Button_Click(object sender, EventArgs e)
        {
            if (Templates_DataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedIndex = Templates_DataGridView.SelectedRows[0].Index;
            SelectedTemplateName = _templateNames[selectedIndex];
            DialogResult = DialogResult.OK;
            Close();
        }
        private void Delete_Button_Click(object sender, EventArgs e)
        {
            if (Templates_DataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            // Get all selected template names
            List<string> selectedTemplateNames = [];
            foreach (DataGridViewRow row in Templates_DataGridView.SelectedRows)
            {
                selectedTemplateNames.Add(_templateNames[row.Index]);
            }

            // Confirm deletion
            if (selectedTemplateNames.Count == 1)
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Delete Template",
                    "Are you sure you want to delete the template '{0}'?\nThis action cannot be undone.",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    selectedTemplateNames[0]);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return;
                }
            }
            else
            {
                CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                    "Delete Templates",
                    "Are you sure you want to delete {0} templates?\nThis action cannot be undone.",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo,
                    selectedTemplateNames.Count);

                if (result != CustomMessageBoxResult.Yes)
                {
                    return;
                }
            }

            int successCount = 0;
            int failCount = 0;

            foreach (string templateName in selectedTemplateNames)
            {
                if (CustomTemplateStorage.DeleteTemplate(templateName))
                {
                    successCount++;

                    // Clear the designer if this template was loaded
                    ReportLayoutDesigner_Form.Instance?.ClearIfTemplateDeleted(templateName);
                }
                else
                {
                    failCount++;
                }
            }

            LoadTemplates();
            UpdateButtonStates();
            ReportDataSelection_Form.Instance.RefreshTemplates();

            // Show error message if any deletions failed
            if (failCount > 0)
            {
                if (failCount == 1)
                {
                    CustomMessageBox.Show(
                        "Delete Failed",
                        "Failed to delete 1 template.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok);
                }
                else
                {
                    CustomMessageBox.ShowWithFormat(
                        "Delete Failed",
                        "Failed to delete {0} templates.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok,
                        failCount);
                }
            }
        }
        private void Rename_Button_Click(object sender, EventArgs e)
        {
            if (Templates_DataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedIndex = Templates_DataGridView.SelectedRows[0].Index;
            string oldTemplateName = _templateNames[selectedIndex];

            // Show SaveTemplate_Form dialog for new name
            SaveTemplate_Form renameForm = new()
            {
                CurrentTemplateName = oldTemplateName,
                IsRenameMode = true
            };

            if (renameForm.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            string newTemplateName = renameForm.TemplateName;

            // Check if user entered empty name or same name
            if (string.IsNullOrWhiteSpace(newTemplateName) || newTemplateName == oldTemplateName)
            {
                return;
            }

            // Check if a template with the new name already exists
            if (_templateNames.Contains(newTemplateName))
            {
                CustomMessageBox.ShowWithFormat(
                    "Template Exists",
                    "A template named '{0}' already exists.\nPlease choose a different name.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    newTemplateName);
                return;
            }

            // Rename the template
            if (CustomTemplateStorage.RenameTemplate(oldTemplateName, newTemplateName))
            {
                LoadTemplates();
                UpdateButtonStates();
                ReportDataSelection_Form.Instance.RefreshTemplates(newTemplateName);

                // Select the renamed template
                int index = _templateNames.IndexOf(newTemplateName);
                if (index >= 0)
                {
                    Templates_DataGridView.ClearSelection();
                    Templates_DataGridView.Rows[index].Selected = true;
                }
            }
            else
            {
                CustomMessageBox.ShowWithFormat(
                    "Rename Failed",
                    "Failed to rename template '{0}' to '{1}'.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    oldTemplateName,
                    newTemplateName);
            }
        }
        private void Export_Button_Click(object sender, EventArgs e)
        {
            if (Templates_DataGridView.SelectedRows.Count == 0)
            {
                return;
            }

            int selectedIndex = Templates_DataGridView.SelectedRows[0].Index;
            string templateName = _templateNames[selectedIndex];

            // Show save file dialog
            using SaveFileDialog saveDialog = new()
            {
                Filter = $"Argo Sales Template (*{ArgoFiles.ArgoTemplateFileExtension})|*{ArgoFiles.ArgoTemplateFileExtension}",
                DefaultExt = ArgoFiles.ArgoTemplateFileExtension,
                FileName = templateName,
                Title = "Export Template",
                InitialDirectory = Directories.Desktop_dir
            };

            if (saveDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // Export template with images using TAR archive
                bool success = CustomTemplateStorage.ExportTemplateWithImages(templateName, saveDialog.FileName);

                if (!success)
                {
                    CustomMessageBox.ShowWithFormat(
                        "Export Failed",
                        "Could not export template '{0}'.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok,
                        templateName);
                    return;
                }

                CustomMessageBox.ShowWithFormat(
                    "Export Successful",
                    "Template '{0}' has been exported successfully.",
                    CustomMessageBoxIcon.Info,
                    CustomMessageBoxButtons.Ok,
                    templateName);
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWithFormat(
                    "Export Failed",
                    "Failed to export template: {0}",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    ex.Message);
            }
        }
        private void Import_Button_Click(object sender, EventArgs e)
        {
            // Show open file dialog
            using OpenFileDialog openDialog = new()
            {
                Filter = $"Argo Sales Template (*{ArgoFiles.ArgoTemplateFileExtension})|*{ArgoFiles.ArgoTemplateFileExtension}",
                DefaultExt = ArgoFiles.ArgoTemplateFileExtension,
                Title = "Import Template",
                InitialDirectory = Directories.Desktop_dir,
                Multiselect = false
            };

            if (openDialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                // Get template name without importing
                string templateName = CustomTemplateStorage.GetTemplateNameFromFile(openDialog.FileName);

                if (string.IsNullOrEmpty(templateName))
                {
                    CustomMessageBox.Show(
                        "Import Failed",
                        "The selected file is not a valid Argo Sales Template.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok);
                    return;
                }

                // Check if trying to import a template with a built-in template name
                string finalTemplateName = templateName;
                if (ReportTemplates.IsBuiltInTemplate(templateName))
                {
                    // Generate a unique name
                    List<string> allTemplateNames = ReportTemplates.GetAvailableTemplates();
                    string suggestedName = Tools.AddNumberForAStringThatAlreadyExists(templateName, allTemplateNames);

                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Reserved Template Name",
                        "The template '{0}' uses a name reserved for a built-in template.\nWould you like to import it as '{1}' instead?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.YesNo,
                        templateName,
                        suggestedName);

                    if (result != CustomMessageBoxResult.Yes)
                    {
                        return;
                    }

                    finalTemplateName = suggestedName;
                }

                // Check if a template with this name already exists
                bool templateExists = _templateNames.Contains(finalTemplateName);
                bool shouldOverwrite = true;

                if (templateExists)
                {
                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Template Exists",
                        "A template named '{0}' already exists. Do you want to overwrite it?",
                        CustomMessageBoxIcon.Question,
                        CustomMessageBoxButtons.YesNo,
                        finalTemplateName);

                    if (result != CustomMessageBoxResult.Yes)
                    {
                        shouldOverwrite = false;
                        return;
                    }
                }

                // Import template with images from TAR archive
                string newTemplateName = (finalTemplateName != templateName) ? finalTemplateName : null;
                string importedTemplateName = CustomTemplateStorage.ImportTemplateWithImages(openDialog.FileName, shouldOverwrite, newTemplateName);

                if (string.IsNullOrEmpty(importedTemplateName))
                {
                    CustomMessageBox.Show(
                        "Import Failed",
                        "Failed to import the template.",
                        CustomMessageBoxIcon.Error,
                        CustomMessageBoxButtons.Ok);
                    return;
                }

                // Reload templates and refresh UI
                LoadTemplates();
                UpdateButtonStates();
                ReportDataSelection_Form.Instance.RefreshTemplates(importedTemplateName);

                // Select the imported template
                int index = _templateNames.IndexOf(importedTemplateName);
                if (index >= 0)
                {
                    Templates_DataGridView.ClearSelection();
                    Templates_DataGridView.Rows[index].Selected = true;
                }

                CustomMessageBox.ShowWithFormat(
                    "Import Successful",
                    "Template '{0}' has been imported successfully.",
                    CustomMessageBoxIcon.Info,
                    CustomMessageBoxButtons.Ok,
                    importedTemplateName);
            }
            catch (Exception ex)
            {
                CustomMessageBox.ShowWithFormat(
                    "Import Failed",
                    "Failed to import template: {0}",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    ex.Message);
            }
        }
        private void Close_Button_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
