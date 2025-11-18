using Ookii.Dialogs.WinForms;
using Sales_Tracker.AnonymousData;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Excel;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;
using System.Diagnostics;

namespace Sales_Tracker
{
    /// <summary>
    /// Form for exporting company data and creating backups.
    /// </summary>
    public partial class Export_Form : BaseForm
    {
        // Properties
        private static Export_Form _instance;
        private int _originalDirectoryLabelY, _originalDirectoryTextBoxY, _originalWarningDirLabelY, _originalWarningDirPictureBoxY;
        private static Action _validationCallback;
        private static bool _isProgramLoading;

        // Getters
        public static Export_Form Instance => _instance;

        // Init.
        public Export_Form()
        {
            InitializeComponent();
            _instance = this;

            _isProgramLoading = true;
            DpiHelper.ScaleComboBox(FileType_ComboBox);
            StoreOriginalPositions();
            AddEventHandlersToTextBoxes();
            SetControls();
            UpdateTheme();
            SetAccessibleDescriptions();
            AnimateButtons();
            _isProgramLoading = false;
            LanguageManager.UpdateLanguageForControl(this);

            PanelCloseFilter panelCloseFilter = new(this, ClosePanels, TextBoxManager.RightClickTextBox_Panel);
            Application.AddMessageFilter(panelCloseFilter);
            LoadingPanel.ShowBlankLoadingPanel(this);
        }
        private void StoreOriginalPositions()
        {
            _originalDirectoryLabelY = Directory_Label.Location.Y;
            _originalDirectoryTextBoxY = Directory_TextBox.Location.Y;
            _originalWarningDirLabelY = WarningDir_Label.Location.Y;
            _originalWarningDirPictureBoxY = WarningDir_PictureBox.Location.Y;
        }
        private void SetControls()
        {
            if (Properties.Settings.Default.ExportDirectory == "")
            {
                Properties.Settings.Default.ExportDirectory = Directories.Desktop_dir;
                Properties.Settings.Default.Save();
            }

            Name_TextBox.Text = Directories.CompanyName + " " + Tools.FormatDate(DateTime.Today);
            Directory_TextBox.Text = Properties.Settings.Default.ExportDirectory;
            FileType_ComboBox.SelectedIndex = 0;

            string defaultCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
            SetDefaultCurrency(defaultCurrency);

            SetExportSpreadsheetControls();
        }
        private void SetDefaultCurrency(string currencyType)
        {
            foreach (string currency in Currency.GetCurrencyTypes())
            {
                // Check if the currency type matches the item (exact match or starts with)
                if (currency.Equals(currencyType, StringComparison.OrdinalIgnoreCase) ||
                    currency.StartsWith(currencyType, StringComparison.OrdinalIgnoreCase))
                {
                    Currency_TextBox.Text = currency;
                    return;
                }
            }

            // If no match found, default to USD
            Currency_TextBox.Text = "USD";
        }
        private void UpdateTheme()
        {
            ThemeManager.SetThemeForForm(this);
            ThemeManager.MakeGButtonBluePrimary(Export_Button);

            if (ThemeManager.IsDarkTheme())
            {
                ThreeDots_Button.Image = Resources.ThreeDotsWhite;
            }
            else
            {
                ThreeDots_Button.Image = Resources.ThreeDotsBlack;
            }
        }
        private void SetAccessibleDescriptions()
        {
            Name_TextBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
            Directory_TextBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;
        }
        private void AddEventHandlersToTextBoxes()
        {
            byte searchBoxMaxHeight = 255;

            TextBoxManager.Attach(Name_TextBox);
            TextBoxManager.Attach(Directory_TextBox);
            TextBoxManager.Attach(Currency_TextBox);

            // Set the validation callback before attaching SearchBox
            _validationCallback = ValidateInputs;
            SearchBox.SetValidationCallback(_validationCallback);

            SearchBox.Attach(Currency_TextBox, this, Currency.GetSearchResults, searchBoxMaxHeight, false, false, false, false);
        }
        public void AnimateButtons()
        {
            CustomControls.AnimateButtons([Export_Button]);
        }

        // Form event handlers
        private void Export_Form_Shown(object sender, EventArgs e)
        {
            ExportCompany_Label.Focus();
            LoadingPanel.HideBlankLoadingPanel(this);
        }

        // Event handlers
        private void Name_TextBox_TextChanged(object sender, EventArgs e)
        {
            bool isNameValid = !("/#%&*|;".Any(Name_TextBox.Text.Contains) || Name_TextBox.Text == "");

            if (isNameValid)
            {
                CustomControls.SetGTextBoxToValid(Name_TextBox);
                WarningName_Label.Visible = false;
                WarningName_PictureBox.Visible = false;
            }
            else
            {
                CustomControls.SetGTextBoxToInvalid(Name_TextBox);
                WarningName_Label.Visible = true;
                WarningName_PictureBox.Visible = true;
            }

            ValidateInputs();
        }
        private void Directory_TextBox_TextChanged(object sender, EventArgs e)
        {
            bool isDirectoryValid = !("/#%&*|;".Any(Directory_TextBox.Text.Contains) ||
                Directory_TextBox.Text == "" ||
                !Directory.Exists(Directory_TextBox.Text.TrimEnd('\\')));

            if (isDirectoryValid)
            {
                Directory_TextBox.BorderColor = CustomColors.ControlBorder;
                Directory_TextBox.FocusedState.BorderColor = CustomColors.ControlBorder;
                WarningDir_Label.Visible = false;
                WarningDir_PictureBox.Visible = false;
            }
            else
            {
                Directory_TextBox.BorderColor = Color.Red;
                Directory_TextBox.FocusedState.BorderColor = Color.Red;
                WarningDir_Label.Visible = true;
                WarningDir_PictureBox.Visible = true;
            }

            ValidateInputs();
        }
        private void FileType_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetExportSpreadsheetControls();
        }
        private void ExportReceipts_Label_Click(object sender, EventArgs e)
        {
            ExportReceipts_CheckBox.Checked = !ExportReceipts_CheckBox.Checked;
        }
        private void ThreeDots_Button_Click(object sender, EventArgs e)
        {
            // Select folder
            VistaFolderBrowserDialog dialog = new();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Directory_TextBox.Text = dialog.SelectedPath + @"\";
            }
        }
        private async void Export_Button_Click(object sender, EventArgs e)
        {
            if (!HasAnyDataToExport())
            {
                CustomMessageBox.Show(
                    LanguageManager.TranslateString("No Data to Export"),
                    LanguageManager.TranslateString("There is no data available to export. Please add some transactions, products, companies, or accountants before exporting."),
                    CustomMessageBoxIcon.Info, CustomMessageBoxButtons.Ok);
                return;
            }

            string loadingText = GetExportLoadingText();
            LoadingPanel.ShowLoadingScreen(this, loadingText);

            // Capture all UI values on the UI thread
            int selectedIndex = FileType_ComboBox.SelectedIndex;
            string directoryPath = Directory_TextBox.Text;
            string fileName = Name_TextBox.Text;
            bool exportReceipts = ExportReceipts_CheckBox.Checked && ExportReceipts_CheckBox.Visible;
            string currency = Currency_TextBox.Text;

            // Run the export operation
            await Task.Run(() => Export(selectedIndex, directoryPath, fileName, exportReceipts, currency));
        }

        // Methods
        private void ValidateInputs()
        {
            if (_isProgramLoading) { return; }

            Export_Button.Enabled = Name_TextBox.BorderColor != Color.Red
                && Directory_TextBox.BorderColor != Color.Red
                && !string.IsNullOrWhiteSpace(Currency_TextBox.Text) && Currency_TextBox.Tag?.ToString() != "0";
        }
        private string GetExportLoadingText()
        {
            bool shouldExportReceipts = ExportReceipts_CheckBox.Checked && ExportReceipts_CheckBox.Visible;
            string fileType = FileType_ComboBox.Text;

            if (shouldExportReceipts)
            {
                string template = LanguageManager.TranslateString("Exporting {0} and receipts...");
                return string.Format(template, fileType);
            }
            else
            {
                string template = LanguageManager.TranslateString("Exporting {0}...");
                return string.Format(template, fileType);
            }
        }
        private void SetExportSpreadsheetControls()
        {
            bool isExcelSelected = FileType_ComboBox.SelectedIndex == 1;

            if (isExcelSelected)
            {
                SetSpreadsheetOptionControlVisibility(true);

                // Move directory controls down to make room for the GroupBox
                byte space = 30;
                Directory_Label.Top = _originalDirectoryLabelY + space;
                Directory_TextBox.Top = _originalDirectoryTextBoxY + space;
                ThreeDots_Button.Top = _originalDirectoryTextBoxY + space;
                WarningDir_Label.Top = _originalWarningDirLabelY + space;
                WarningDir_PictureBox.Top = _originalWarningDirPictureBoxY + space;
            }
            else
            {
                SetSpreadsheetOptionControlVisibility(false);

                // Move directory controls back to original positions
                Directory_Label.Top = _originalDirectoryLabelY;
                Directory_TextBox.Top = _originalDirectoryTextBoxY;
                ThreeDots_Button.Top = _originalDirectoryTextBoxY;
                WarningDir_Label.Top = _originalWarningDirLabelY;
                WarningDir_PictureBox.Top = _originalWarningDirPictureBoxY;
            }
        }
        private void SetSpreadsheetOptionControlVisibility(bool visible)
        {
            ExportReceipts_CheckBox.Visible = visible;
            ExportReceipts_Label.Visible = visible;
            Currency_Label.Visible = visible;
            Currency_TextBox.Visible = visible;
        }
        private void Export(int selectedIndex, string directoryPath, string fileName, bool exportReceipts, string currency)
        {
            string filePath = Path.Combine(directoryPath, fileName);
            Stopwatch stopwatch = Stopwatch.StartNew();

            switch (selectedIndex)
            {
                case 0:  // .zip backup
                    Directories.CreateBackup(filePath);
                    TrackExport(stopwatch, filePath + ArgoFiles.ZipExtension, ExportType.Backup);

                    string backupMessage = GetBackupSuccessMessage();
                    FinalizeExport(backupMessage);
                    break;

                case 1:  // .xlsx spreadsheet
                    string exportFolder = Path.Combine(directoryPath, fileName);

                    // Create export folder
                    exportFolder = Directories.GetNewDirectoryNameIfItAlreadyExists(exportFolder);
                    Directory.CreateDirectory(exportFolder);

                    // Create xlsx file inside the export folder
                    string xlsxPath = Path.Combine(exportFolder, fileName + ArgoFiles.XlsxFileExtension);
                    ExportExcelSheetManager.ExportSpreadsheet(xlsxPath, currency);

                    TrackExport(stopwatch, xlsxPath, ExportType.XLSX);

                    string successMessage = ExportReceiptsAndGetSpreadsheetSuccessMessage(exportReceipts, exportFolder);
                    FinalizeExport(successMessage);
                    break;
            }
        }
        private static string GetBackupSuccessMessage()
        {
            string template = LanguageManager.TranslateString("Successfully backed up '{0}'");
            return string.Format(template, Directories.CompanyName);
        }
        private static string ExportReceiptsAndGetSpreadsheetSuccessMessage(bool exportReceipts, string exportFolder)
        {
            string baseTemplate = LanguageManager.TranslateString("Successfully created spreadsheet for '{0}'");
            string successMessage = string.Format(baseTemplate, Directories.CompanyName);

            // Export receipts if checkbox is checked
            if (exportReceipts)
            {
                try
                {
                    ExportReceiptsToFolder(Path.Combine(exportFolder, "Receipts"));
                    string receiptsText = LanguageManager.TranslateString(" and exported receipts");
                    successMessage += receiptsText;
                }
                catch (Exception ex)
                {
                    string errorTemplate = LanguageManager.TranslateString(", but failed to export receipts: {0}");
                    string errorText = string.Format(errorTemplate, ex.Message);
                    successMessage += errorText;
                }
            }

            return successMessage;
        }
        private static void ExportReceiptsToFolder(string folder)
        {
            // Create receipts folder
            Directory.CreateDirectory(folder);

            List<string> allReceipts = [];

            // Collect receipt paths from sales
            foreach (DataGridViewRow row in MainMenu_Form.Instance.Sale_DataGridView.Rows)
            {
                string receiptPath = GetReceiptPathFromRow(row);
                if (!string.IsNullOrEmpty(receiptPath) && File.Exists(receiptPath))
                {
                    allReceipts.Add(receiptPath);
                }
            }

            // Collect receipt paths from purchases
            foreach (DataGridViewRow row in MainMenu_Form.Instance.Purchase_DataGridView.Rows)
            {
                string receiptPath = GetReceiptPathFromRow(row);
                if (!string.IsNullOrEmpty(receiptPath) && File.Exists(receiptPath))
                {
                    allReceipts.Add(receiptPath);
                }
            }

            // Copy receipts to the export folder
            foreach (string receiptPath in allReceipts)
            {
                string fileName = Path.GetFileName(receiptPath);
                string destinationPath = Path.Combine(folder, fileName);
                File.Copy(receiptPath, destinationPath, true);
            }
        }
        private static string GetReceiptPathFromRow(DataGridViewRow row)
        {
            if (row.Tag == null) { return ""; }

            // Handle different tag formats based on Receipts_Form.cs pattern
            switch (row.Tag)
            {
                case (string dir, TagData):
                    return ReceiptManager.ProcessReceiptTextFromRowTag(dir);

                case (List<string> items, TagData) when items.Count > 0:
                    string receipt = items[^1];
                    return ReceiptManager.ProcessReceiptTextFromRowTag(receipt);

                default:
                    return "";
            }
        }
        private static void TrackExport(Stopwatch stopwatch, string filePath, ExportType exportType)
        {
            stopwatch.Stop();
            string readableSize = "0 Bytes";

            if (File.Exists(filePath))
            {
                FileInfo fileInfo = new(filePath);
                long fileSizeBytes = fileInfo.Length;
                readableSize = Tools.ConvertBytesToReadableSize(fileSizeBytes);
            }

            Dictionary<ExportDataField, object> exportData = new()
            {
                { ExportDataField.ExportType, exportType },
                { ExportDataField.DurationMS, stopwatch.ElapsedMilliseconds },
                { ExportDataField.FileSize, readableSize }
            };

            AnonymousDataManager.AddExportData(exportData);
        }
        private void FinalizeExport(string message)
        {
            Invoke(() =>
            {
                LoadingPanel.HideLoadingScreen(this);
                string title = LanguageManager.TranslateString("Successfully exported");
                CustomMessageBox.Show(title, message, CustomMessageBoxIcon.Success, CustomMessageBoxButtons.Ok);
            });
        }
        private static bool HasAnyDataToExport()
        {
            MainMenu_Form main = MainMenu_Form.Instance;

            return main.Purchase_DataGridView.Rows.Count > 0 ||
                   main.Sale_DataGridView.Rows.Count > 0 ||
                   main.CategoryPurchaseList.Count > 0 ||
                   main.CategorySaleList.Count > 0 ||
                   main.CompanyList.Count > 0 ||
                   main.AccountantList.Count > 0;
        }
        private void ClosePanels()
        {
            TextBoxManager.HideRightClickPanel();
        }
    }
}