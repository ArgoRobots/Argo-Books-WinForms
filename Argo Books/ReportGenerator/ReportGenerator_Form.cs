using Argo_Books.Classes;
using Argo_Books.DataClasses;
using Argo_Books.ReportGenerator;
using Argo_Books.Theme;
using Argo_Books.UI;
using Argo_Books.Language;
using Argo_Books.Properties;
using Argo_Books.ReportGenerator.Menus;
using Argo_Books.Theme;

namespace Argo_Books.ReportGenerator
{
    /// <summary>
    /// Main container form for the Data Report Generation System.
    /// Manages navigation between child forms and coordinates the report creation process.
    /// </summary>
    public partial class ReportGenerator_Form : BaseForm
    {
        // Properties
        private static ReportGenerator_Form _instance;
        private ReportDataSelection_Form _dataSelectionForm;
        private ReportLayoutDesigner_Form _layoutDesignerForm;
        private ReportPreviewExport_Form _previewExportForm;
        private int _currentStep = 0;
        private const byte _totalSteps = 3;

        // Getters
        public static ReportGenerator_Form Instance => _instance;
        public ReportConfiguration CurrentReportConfiguration { get; private set; }

        public enum ReportStep
        {
            DataSelection = 0,
            LayoutDesigner = 1,
            PreviewExport = 2
        }

        // Init.
        public ReportGenerator_Form()
        {
            InitializeComponent();
            _instance = this;

            CurrentReportConfiguration = new ReportConfiguration();
            InitializeChildForms();

            SetCurrentStep(ReportStep.DataSelection);
            UpdateNavigationButtons();
            UpdateProgressIndicator();

            UpdateTheme();
            SetAccessibleDescriptions();
            LanguageManager.UpdateLanguageForControl(this);
            AlignControlsAfterLanguageChange();
            AnimateButtons();

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels,
                RightClickElementMenu.Panel,
                UndoRedoHistoryDropdown.Panel,
                SearchBox.SearchResultBoxContainer);

            Application.AddMessageFilter(panelCloseFilter);

            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        public static void AlignControlsAfterLanguageChange()
        {
            ReportDataSelection_Form.Instance.AlignControlsAfterLanguageChange();
            PageSettings_Form.Instance?.AlignControlsAfterLanguageChange();
        }
        private void InitializeChildForms()
        {
            _dataSelectionForm = new();
            _layoutDesignerForm = new();
            _previewExportForm = new();

            SetupChildForm(_dataSelectionForm);
            SetupChildForm(_layoutDesignerForm);
            SetupChildForm(_previewExportForm);
        }
        private void SetupChildForm(Form childForm)
        {
            if (childForm == null) { return; }

            childForm.TopLevel = false;
            childForm.Dock = DockStyle.Fill;
            childForm.Visible = false;

            // Add to main content panel
            Content_Panel.Controls.Add(childForm);
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);

            ReportLayoutDesigner_Form.Instance.ToolsContainer_Panel.BackColor = CustomColors.ControlBack;
            ReportLayoutDesigner_Form.Instance.PropertiesContainer_Panel.BackColor = CustomColors.ControlBack;

            ReportPreviewExport_Form.Instance.OpenAfterExport_CheckBox.UncheckedState.FillColor = CustomColors.MainBackground;

            ThemeManager.MakeGButtonBluePrimary(Previous_Button);
            ThemeManager.MakeGButtonBluePrimary(Next_Button);
            ThemeManager.MakeGButtonBlueSecondary(Cancel_Button);

            ReportLayoutDesigner_Form designer = ReportLayoutDesigner_Form.Instance;

            if (ThemeManager.IsDarkTheme())
            {
                ReportDataSelection_Form.Instance.OpenTemplates_Button.Image = Resources.OpenFolderWhite;

                designer.Undo_Button.Image = Resources.UndoWhite;
                designer.UndoDropdown_Button.Image = Resources.DropDownWhite;
                designer.Redo_Button.Image = Resources.RedoWhite;
                designer.RedoDropdown_Button.Image = Resources.DropDownWhite;

                designer.AlignLeft_Button.Image = Resources.ALignLeftWhite;
                designer.AlignCenter_Button.Image = Resources.AlignCenterWhite;
                designer.AlignRight_Button.Image = Resources.AlignRightWhite;

                designer.AlignTop_Button.Image = Resources.AlignTopWhite;
                designer.AlignMiddle_Button.Image = Resources.AlignMiddleWhite;
                designer.AlignBottom_Button.Image = Resources.AlignBottomWhite;

                designer.DistributeHorizontally_Button.Image = Resources.DistributeHorizontallyWhite;
                designer.DistributeVertically_Button.Image = Resources.DistributeVerticallyWhite;

                designer.MakeSameWidth_Button.Image = Resources.SameWidthWhite;
                designer.MakeSameHeight_Button.Image = Resources.SameHeightWhite;
                designer.MakeSameSize_Button.Image = Resources.SameSizeWhite;

                designer.SaveTemplate_Button.Image = Resources.SaveWhite;
            }
            else
            {
                ReportDataSelection_Form.Instance.OpenTemplates_Button.Image = Resources.OpenFolderGray;

                designer.Undo_Button.Image = Resources.UndoBlack;
                designer.UndoDropdown_Button.Image = Resources.DropDownBlack;
                designer.Redo_Button.Image = Resources.RedoBlack;
                designer.RedoDropdown_Button.Image = Resources.DropDownBlack;

                designer.AlignLeft_Button.Image = Resources.ALignLeftBlack;
                designer.AlignCenter_Button.Image = Resources.AlignCenterBlack;
                designer.AlignRight_Button.Image = Resources.AlignRightBlack;

                designer.AlignTop_Button.Image = Resources.AlignTopBlack;
                designer.AlignMiddle_Button.Image = Resources.AlignMiddleBlack;
                designer.AlignBottom_Button.Image = Resources.AlignBottomBlack;

                designer.DistributeHorizontally_Button.Image = Resources.DistributeHorizontallyBlack;
                designer.DistributeVertically_Button.Image = Resources.DistributeVerticallyBlack;

                designer.MakeSameWidth_Button.Image = Resources.SameWidthBlack;
                designer.MakeSameHeight_Button.Image = Resources.SameHeightBlack;
                designer.MakeSameSize_Button.Image = Resources.SameSizeBlack;

                designer.SaveTemplate_Button.Image = Resources.SaveGray;
            }
        }
        private void SetAccessibleDescriptions()
        {
            StepTitle_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotCache;
            ProgressValue_Label.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }
        public void AnimateButtons()
        {
            CustomControls.AnimateButtons([Previous_Button, Next_Button, Cancel_Button]);
        }

        // Navigation methods
        public void SetCurrentStep(ReportStep step)
        {
            _currentStep = (int)step;

            HideAllChildForms();

            // Show the appropriate child form
            switch (step)
            {
                case ReportStep.DataSelection:
                    ShowChildForm(_dataSelectionForm);
                    StepTitle_Label.Text = LanguageManager.TranslateString("Data Selection");
                    break;

                case ReportStep.LayoutDesigner:
                    ShowChildForm(_layoutDesignerForm);
                    StepTitle_Label.Text = LanguageManager.TranslateString("Layout Designer");
                    break;

                case ReportStep.PreviewExport:
                    ShowChildForm(_previewExportForm);
                    StepTitle_Label.Text = LanguageManager.TranslateString("Preview && Export");
                    break;
            }

            UpdateNavigationButtons();
            UpdateProgressIndicator();
        }
        private void HideAllChildForms()
        {
            _dataSelectionForm?.Hide();
            _layoutDesignerForm?.Hide();
            _previewExportForm?.Hide();
        }
        private static void ShowChildForm(Form childForm)
        {
            childForm?.Show();
            childForm?.BringToFront();
        }
        private void UpdateNavigationButtons()
        {
            Previous_Button.Enabled = _currentStep > 0;

            bool canProceed = CanProceedToNextStep();
            Next_Button.Enabled = _currentStep < _totalSteps - 1 && canProceed;

            if (_currentStep == _totalSteps - 1)
            {
                Next_Button.Text = LanguageManager.TranslateString("Export");
                Next_Button.Enabled = canProceed;
            }
            else
            {
                Next_Button.Text = LanguageManager.TranslateString("Next");
            }
        }
        private bool CanProceedToNextStep()
        {
            return (ReportStep)_currentStep switch
            {
                ReportStep.DataSelection => _dataSelectionForm.IsValidForNextStep(),
                ReportStep.LayoutDesigner => ReportLayoutDesigner_Form.IsValidForNextStep(),
                ReportStep.PreviewExport => true,  // Always can finish from preview
                _ => false,
            };
        }
        private void UpdateProgressIndicator()
        {
            int progressPercentage = (int)((_currentStep + 1.0) / _totalSteps * 100);
            Progress_ProgressBar.Value = progressPercentage;
            ProgressValue_Label.Text = $"{_currentStep + 1} / {_totalSteps}";
        }

        // Form event handlers
        private void ReportGenerator_Form_Shown(object sender, EventArgs e)
        {
            LoadingPanel.HideBlankLoadingPanel(this);
        }
        private void ReportGenerator_Form_Resize(object sender, EventArgs e)
        {
            ClosePanels();
        }
        private void ReportGenerator_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!AskUserToSaveChanges())
            {
                e.Cancel = true;
                return;
            }

            CustomTemplateStorage.CleanupUnusedImages();

            // Clean up child forms
            _dataSelectionForm?.Dispose();
            _layoutDesignerForm?.Dispose();
            _previewExportForm?.Dispose();
        }

        // Event handlers
        private void Previous_Button_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
            {
                SetCurrentStep((ReportStep)(_currentStep - 1));
            }
        }
        private void Next_Button_Click(object sender, EventArgs e)
        {
            if (_currentStep < _totalSteps - 1)
            {
                SetCurrentStep((ReportStep)(_currentStep + 1));
            }
            else
            {
                FinishReportGeneration();
            }
        }
        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            if (AskUserToSaveChanges())
            {
                CustomTemplateStorage.CleanupUnusedImages();
                ReportLayoutDesigner_Form.HasUnsavedChanges = false;
                Close();
            }
        }

        // Validation and completion
        private void FinishReportGeneration()
        {
            try
            {
                // Final validation
                if (!_previewExportForm?.ValidateStep() ?? true)
                {
                    return;
                }

                // Export the report
                bool exportSuccess = _previewExportForm?.ExportReport() ?? false;

                if (exportSuccess)
                {
                    DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(
                    "Export Error",
                    $"An error occurred while exporting the report: {ex.Message}",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok
                );
            }
        }

        /// <summary>
        /// Asks the user to confirm closing the report generation process if there are unsaved changes.
        /// </summary>
        /// <returns>True if the user selects yes or there are no unsaved changes, otherwise False.</returns>
        private static bool AskUserToSaveChanges()
        {
            if (ReportLayoutDesigner_Form.HasUnsavedChanges)
            {
                CustomMessageBoxResult result = CustomMessageBox.Show(
                    "Cancel Report Generation",
                    "Are you sure you want to exit? Any unsaved changes will be lost.",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo
                );

                return result == CustomMessageBoxResult.Yes;
            }
            return true;
        }

        // Public methods for child Forms
        /// <summary>
        /// Called by child forms to notify that their validation state has changed.
        /// </summary>
        public void OnChildFormValidationChanged()
        {
            UpdateNavigationButtons();
        }

        // Other methods
        private static void ClosePanels()
        {
            RightClickElementMenu.Hide();
            UndoRedoHistoryDropdown.Remove();
            SearchBox.Close();
        }
    }
}