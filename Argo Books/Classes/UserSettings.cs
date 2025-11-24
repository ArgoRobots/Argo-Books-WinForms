using Argo_Books.DataClasses;
using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using Argo_Books;
using Argo_Books.Classes;
using Argo_Books.Language;
using Argo_Books.ReportGenerator;
using Argo_Books.Settings;
using Argo_Books.Settings.Menus;
using Argo_Books.Theme;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Manages user settings, including saving, updating, and resetting.
    /// </summary>
    internal class UserSettings
    {
        /// <summary>
        /// Saves user settings and returns whether the operation was successful.
        /// </summary>
        /// <returns>True if all settings were successfully saved, false if cancelled or failed.</returns>
        public static async Task<bool> SaveUserSettingsAsync()
        {
            Properties.Settings settings = Properties.Settings.Default;
            General_Form form = General_Form.Instance;

            // Handle currency change
            string oldCurrency = DataFileManager.GetValue(AppDataSettings.DefaultCurrencyType);
            if (oldCurrency != form.Currency_TextBox.Text)
            {
                bool currencyUpdateSuccess = await UpdateCurrencyAsync(oldCurrency);
                if (!currencyUpdateSuccess)
                {
                    return false;  // Currency update was cancelled or failed
                }
            }

            // Handle theme change - Use SelectedIndex instead of Text to work with translations
            if (settings.ColorTheme != GetThemeNameFromIndex(form.ColorTheme_ComboBox.SelectedIndex))
            {
                UpdateTheme();
            }

            // Update checkbox settings
            UpdateSetting("Show tooltips", settings.ShowTooltips, form.ShowTooltips_CheckBox.Checked,
                value => settings.ShowTooltips = value);
            UpdateSetting("Show debug info", settings.ShowDebugInfo, form.ShowDebugInfo_CheckBox.Checked,
                value => settings.ShowDebugInfo = value);
            UpdateSetting("Send anonymous information", settings.SendAnonymousInformation, form.SendAnonymousInformation_CheckBox.Checked,
                value => settings.SendAnonymousInformation = value);
            UpdateSetting("Purchase receipts", settings.PurchaseReceipts, form.PurchaseReceipts_CheckBox.Checked,
                value => settings.PurchaseReceipts = value);
            UpdateSetting("Sale receipts", settings.SaleReceipts, form.SalesReceipts_CheckBox.Checked,
                value => settings.SaleReceipts = value);
            UpdateSetting("AI search", settings.AISearchEnabled, form.EnableAISearch_CheckBox.Checked,
              value => settings.AISearchEnabled = value);
            UpdateSetting("File encryption", settings.EncryptFiles, Security_Form.Instance.EncryptFiles_CheckBox.Checked,
                value => settings.EncryptFiles = value);

            // Handle animate buttons
            if (settings.AnimateButtons != form.AnimateButtons_CheckBox.Checked)
            {
                UpdateSetting("Animate buttons", settings.AnimateButtons, form.AnimateButtons_CheckBox.Checked,
                    value => settings.AnimateButtons = value);

                Settings_Form.Instance.AnimateButtons();
                ReportGenerator_Form.Instance?.AnimateButtons();
                Export_Form.Instance?.AnimateButtons();
                Log_Form.Instance?.AnimateButtons();
                MainMenu_Form.Instance.AnimateButtons();
                Receipts_Form.Instance?.AnimateButtons();
            }

            // Handle animate charts
            if (settings.AnimateCharts != form.AnimateCharts_CheckBox.Checked)
            {
                UpdateSetting("Animate charts", settings.AnimateCharts, form.AnimateCharts_CheckBox.Checked,
                    value => settings.AnimateCharts = value);

                MainMenu_Form.Instance.AnimateCharts();
            }

            // Handle company logo visibility
            if (settings.ShowCompanyLogo != form.ShowCompanyLogo_CheckBox.Checked)
            {
                UpdateSetting("Show company logo", settings.ShowCompanyLogo, form.ShowCompanyLogo_CheckBox.Checked,
                    value => settings.ShowCompanyLogo = value);

                CompanyLogo.SetCompanyLogo();
            }

            return true;  // All settings were successfully saved
        }

        /// <summary>
        /// Gets theme name from ComboBox index to work with translations.
        /// </summary>
        private static string GetThemeNameFromIndex(int index)
        {
            ThemeManager.ThemeType[] themes = Enum.GetValues<ThemeManager.ThemeType>();
            if (index >= 0 && index < themes.Length)
            {
                return themes[index].ToString();
            }
            return ThemeManager.ThemeType.Windows.ToString();  // Default fallback
        }
        public static void UpdateSetting(string settingName, bool currentValue, bool newValue, Action<bool> setter)
        {
            if (currentValue != newValue)
            {
                setter(newValue);
                string message = LanguageManager.TranslateString($"Changed the '{settingName}' setting");
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.SettingsThatHaveChangedInFile, 2, message);
            }
        }
        private static void UpdateTheme()
        {
            int selectedIndex = General_Form.Instance.ColorTheme_ComboBox.SelectedIndex;
            ThemeManager.ThemeType[] themes = Enum.GetValues<ThemeManager.ThemeType>();

            if (selectedIndex >= 0 && selectedIndex < themes.Length)
            {
                ThemeManager.CurrentTheme = themes[selectedIndex];
            }
            else
            {
                ThemeManager.CurrentTheme = ThemeManager.ThemeType.Windows;  // Default fallback
            }

            CustomColors.SetColors();
            FormThemeManager.UpdateAllForms();
            ThemeManager.UpdateOtherControls();

            // Remove previous messages that mention theme changes
            string message = "Changed the color theme to";
            MainMenu_Form.SettingsThatHaveChangedInFile.RemoveAll(x => x.Contains(message));

            // Get the theme name for logging (use enum name, not translated text)
            string themeName = ThemeManager.CurrentTheme.ToString();
            CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.SettingsThatHaveChangedInFile, 2, $"{message} {themeName}");
        }
        private static async Task<bool> UpdateCurrencyAsync(string oldCurrency)
        {
            CancellationTokenSource cancellationTokenSource = new();
            LoadingPanel.ShowLoadingScreen(Settings_Form.Instance, "Converting currency data...", false, cancellationTokenSource);

            // Check for internet connection
            if (!await InternetConnectionManager.CheckInternetAndShowMessageAsync("currency conversion", true))
            {
                Log.Write(1, "Currency conversion cancelled - no internet connection");
                LoadingPanel.HideLoadingScreen(Settings_Form.Instance);
                return false;
            }

            string newCurrency = General_Form.Instance.Currency_TextBox.Text;

            try
            {
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                // Initial delay to ensure loading panel appears and starts animating
                await Task.Delay(100, cancellationToken);

                // Update basic currency settings
                DataFileManager.SetValue(AppDataSettings.DefaultCurrencyType, newCurrency);
                MainMenu_Form.CurrencySymbol = Currency.GetSymbol();
                MainMenu_Form.IsProgramLoading = true;

                Guna2DataGridView purchaseDataGridView = MainMenu_Form.Instance.Purchase_DataGridView;
                Guna2DataGridView salesDataGridView = MainMenu_Form.Instance.Sale_DataGridView;
                List<DataGridViewRow> purchaseRows = [];
                List<DataGridViewRow> saleRows = [];

                // Backup original cell values
                Dictionary<DataGridViewRow, Dictionary<string, string>> originalValues = [];

                // Collect all rows in single UI thread calls
                TaskCompletionSource<bool> collectRowsTcs = new();

                purchaseDataGridView.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        foreach (DataGridViewRow row in purchaseDataGridView.Rows)
                        {
                            purchaseRows.Add(row);
                        }
                        foreach (DataGridViewRow row in salesDataGridView.Rows)
                        {
                            saleRows.Add(row);
                        }
                        collectRowsTcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        collectRowsTcs.SetException(ex);
                    }
                }));

                await collectRowsTcs.Task;

                // Store original row tags for rollback
                Dictionary<DataGridViewRow, object> originalRowTags = [];
                foreach (DataGridViewRow row in purchaseRows.Concat(saleRows))
                {
                    originalRowTags[row] = row.Tag;
                }

                // Check for cancellation after collecting rows
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Write(1, "Currency conversion cancelled by user");
                    return false;
                }

                // Process all currency updates on background thread
                bool allSuccessful = await Task.Run(() =>
                {
                    // Process purchase rows
                    foreach (DataGridViewRow row in purchaseRows)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!UpdateCurrencyValueInRowData(row, newCurrency))
                        {
                            return false;  // Stop on failure
                        }
                    }

                    // Process sale rows  
                    foreach (DataGridViewRow row in saleRows)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (!UpdateCurrencyValueInRowData(row, newCurrency))
                        {
                            return false;  // Stop on failure
                        }
                    }

                    return true;
                }, cancellationToken);

                if (!allSuccessful)
                {
                    // Revert currency
                    DataFileManager.SetValue(AppDataSettings.DefaultCurrencyType, oldCurrency);
                    MainMenu_Form.CurrencySymbol = Currency.GetSymbol();

                    // Reload data from files to restore original values
                    MainMenu_Form.Instance.BeginInvoke(new Action(() =>
                    {
                        foreach (KeyValuePair<DataGridViewRow, object> kvp in originalRowTags)
                        {
                            kvp.Key.Tag = kvp.Value;
                        }

                        // Refresh display with original values
                        purchaseDataGridView.Refresh();
                        salesDataGridView.Refresh();
                        MainMenu_Form.Instance.UpdateTotalLabels();
                    }));

                    Log.Write(1, "Currency conversion cancelled - changes reverted");
                    return false;
                }

                // Check for cancellation after processing
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Write(1, "Currency conversion cancelled by user");
                    return false;
                }

                // Update UI elements in single operation
                TaskCompletionSource<bool> uiUpdateTcs = new();

                MainMenu_Form.Instance.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        // Refresh DataGridViews to show updated values
                        purchaseDataGridView.Refresh();
                        salesDataGridView.Refresh();

                        // Update totals and charts
                        MainMenu_Form.Instance.UpdateTotalLabels();
                        MainMenu_Form.Instance.LoadOrRefreshMainCharts();
                        MainMenu_Form.Instance.UpdateChartCurrencyFormats();

                        uiUpdateTcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteWithFormat(1, "Error in UI updates: {0}", ex.Message);
                        uiUpdateTcs.SetException(ex);
                    }
                }));

                // Wait for UI updates to complete
                await uiUpdateTcs.Task;

                // Save the updated data
                MainMenu_Form.SaveDataGridViewToFileAsJson(purchaseDataGridView, MainMenu_Form.SelectedOption.Purchases);
                MainMenu_Form.SaveDataGridViewToFileAsJson(salesDataGridView, MainMenu_Form.SelectedOption.Sales);

                // Remove previous messages that mention currency changes
                string message = "Changed the currency from";
                MainMenu_Form.SettingsThatHaveChangedInFile.RemoveAll(x => x.Contains(message));

                // Add the new currency change message
                string fullMessage = $"{message} {oldCurrency} to {newCurrency}";
                CustomMessage_Form.AddThingThatHasChangedAndLogMessage(MainMenu_Form.SettingsThatHaveChangedInFile, 2, fullMessage);

                Log.WriteWithFormat(2, "Currency conversion completed: {0} to {1}", oldCurrency, newCurrency);
                return true;  // Success
            }
            catch (Exception ex)
            {
                Log.Error_WriteToFile($"Error during currency conversion: {ex.Message}");

                // Revert currency settings on error
                DataFileManager.SetValue(AppDataSettings.DefaultCurrencyType, oldCurrency);
                MainMenu_Form.CurrencySymbol = Currency.GetSymbol();

                CustomMessageBox.Show(
                    "Currency Conversion Error",
                    "An error occurred during currency conversion. Settings have been reverted to the previous currency.",
                    CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok
                );
                return false;
            }
            finally
            {
                MainMenu_Form.IsProgramLoading = false;
                LoadingPanel.HideLoadingScreen(Settings_Form.Instance);
            }
        }
        private static bool UpdateCurrencyValueInRowData(DataGridViewRow row, string defaultCurrency)
        {
            try
            {
                if (row.Tag is (string, TagData tagData))
                {
                    // Skip conversion if currencies match
                    if (defaultCurrency == tagData.OriginalCurrency)
                    {
                        UpdateRowWithOriginalValues(row, tagData);
                    }
                    else
                    {
                        // Only get exchange rate when needed
                        string rowDate = row.Cells[ReadOnlyVariables.Date_column].Value.ToString();
                        decimal USDToDefault = Currency.GetExchangeRate("USD", defaultCurrency, rowDate, false);
                        if (USDToDefault == -1) { return false; }

                        UpdateRowWithConvertedValues(row, tagData, USDToDefault);
                    }
                }
                else if (row.Tag is (List<string> itemList, TagData tagData1))
                {
                    // Skip conversion if currencies match
                    if (defaultCurrency == tagData1.OriginalCurrency)
                    {
                        UpdateMultiItemRowWithOriginalValues(row, tagData1, itemList);
                    }
                    else
                    {
                        // Only get exchange rate when needed
                        string rowDate = row.Cells[ReadOnlyVariables.Date_column].Value.ToString();
                        decimal USDToDefault = Currency.GetExchangeRate("USD", defaultCurrency, rowDate, false);
                        if (USDToDefault == -1) { return false; }

                        UpdateMultiItemRowWithConvertedValues(row, tagData1, itemList, USDToDefault);
                    }
                }
                else if (row.Tag is TagData tagData2)
                {
                    // Only get exchange rate when needed
                    string rowDate = row.Cells[ReadOnlyVariables.Date_column].Value.ToString();
                    decimal USDToDefault = Currency.GetExchangeRate("USD", defaultCurrency, rowDate, false);
                    if (USDToDefault == -1) { return false; }

                    UpdateRowWithConvertedValues(row, tagData2, USDToDefault);
                }
                else
                {
                    string id = row.Cells[ReadOnlyVariables.ID_column].Value.ToString();
                    Log.WriteWithFormat(1, "Error updating currency for row #{0}", id);

                    CustomMessageBoxResult result = CustomMessageBox.ShowWithFormat(
                        "Currency Conversion Error",
                        "An error occurred while updating currency for row #{0} because the row data is invalid. ",
                        CustomMessageBoxIcon.Error, CustomMessageBoxButtons.Ok,
                        id
                    );

                    return false;
                }

                return true;  // Success
            }
            catch (Exception ex)
            {
                // Log but don't crash the entire operation
                Log.WriteWithFormat(1, "Error updating currency for row: {0}", ex.Message);
                return false;
            }
        }
        private static void UpdateRowWithOriginalValues(DataGridViewRow row, TagData tagData)
        {
            row.Cells[ReadOnlyVariables.PricePerUnit_column].Value = tagData.OriginalPricePerUnit.ToString("N2");
            row.Cells[ReadOnlyVariables.Shipping_column].Value = tagData.OriginalShipping.ToString("N2");
            row.Cells[ReadOnlyVariables.Tax_column].Value = tagData.OriginalTax.ToString("N2");
            row.Cells[ReadOnlyVariables.Fee_column].Value = tagData.OriginalFee.ToString("N2");
            row.Cells[ReadOnlyVariables.ChargedDifference_column].Value = tagData.OriginalChargedDifference.ToString("N2");
            row.Cells[ReadOnlyVariables.Total_column].Value = tagData.OriginalChargedOrCredited.ToString("N2");
        }
        private static void UpdateRowWithConvertedValues(DataGridViewRow row, TagData tagData, decimal USDToDefault)
        {
            row.Cells[ReadOnlyVariables.PricePerUnit_column].Value = (tagData.PricePerUnitUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Shipping_column].Value = (tagData.ShippingUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Tax_column].Value = (tagData.TaxUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Fee_column].Value = (tagData.FeeUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.ChargedDifference_column].Value = (tagData.ChargedDifferenceUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Total_column].Value = (tagData.ChargedOrCreditedUSD * USDToDefault).ToString("N2");
        }
        private static void UpdateMultiItemRowWithOriginalValues(DataGridViewRow row, TagData tagData, List<string> itemList)
        {
            row.Cells[ReadOnlyVariables.Shipping_column].Value = tagData.OriginalShipping.ToString("N2");
            row.Cells[ReadOnlyVariables.Tax_column].Value = tagData.OriginalTax.ToString("N2");
            row.Cells[ReadOnlyVariables.Fee_column].Value = tagData.OriginalFee.ToString("N2");
            row.Cells[ReadOnlyVariables.ChargedDifference_column].Value = tagData.OriginalChargedDifference.ToString("N2");
            row.Cells[ReadOnlyVariables.Total_column].Value = tagData.OriginalChargedOrCredited.ToString("N2");

            UpdateItemList(row, itemList, useOriginalPrice: true, tagData);
        }
        private static void UpdateMultiItemRowWithConvertedValues(DataGridViewRow row, TagData tagData, List<string> itemList, decimal USDToDefault)
        {
            row.Cells[ReadOnlyVariables.Shipping_column].Value = (tagData.ShippingUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Tax_column].Value = (tagData.TaxUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Fee_column].Value = (tagData.FeeUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.ChargedDifference_column].Value = (tagData.ChargedDifferenceUSD * USDToDefault).ToString("N2");
            row.Cells[ReadOnlyVariables.Total_column].Value = (tagData.ChargedOrCreditedUSD * USDToDefault).ToString("N2");

            UpdateItemList(row, itemList, useOriginalPrice: false, tagData, USDToDefault);
        }
        private static void UpdateItemList(DataGridViewRow row, List<string> itemList, bool useOriginalPrice, TagData tagData, decimal USDToDefault = 1m)
        {
            bool hasReceipt = itemList.Last().StartsWith(ReadOnlyVariables.Receipt_text);
            List<string> valuesList = [];

            for (int i = 0; i < itemList.Count - (hasReceipt ? 1 : 0); i++)
            {
                string[] values = itemList[i].Split(',');

                if (useOriginalPrice)
                {
                    decimal originalItemPrice = decimal.Parse(values[5]);
                    values[5] = originalItemPrice.ToString("N2");
                }
                else
                {
                    decimal itemPricePerUnitUSD = decimal.Parse(values[6]);
                    values[5] = (itemPricePerUnitUSD * USDToDefault).ToString("N2");
                }

                valuesList.Add(string.Join(",", values));
            }

            if (hasReceipt)
            {
                valuesList.Add(itemList.Last());
            }

            row.Tag = (valuesList, tagData);
        }

        /// <summary>
        /// Resets all user settings to their default values.
        /// </summary>
        public static void ResetAllToDefault()
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();

            General_Form.Instance.UpdateControls();
            Security_Form.Instance.UpdateControls();
        }
    }
}