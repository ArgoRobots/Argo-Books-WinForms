using Guna.UI2.WinForms;
using Sales_Tracker.Charts;
using Sales_Tracker.DataClasses;
using Sales_Tracker.Language;
using Sales_Tracker.ReportGenerator.Elements;
using Sales_Tracker.Theme;
using Sales_Tracker.UI;

namespace Sales_Tracker.ReportGenerator.Menus
{
    /// <summary>
    /// First step in report generation - allows users to select charts, transactions, and apply filters.
    /// </summary>
    public partial class ReportDataSelection_Form : Form
    {
        // Properties
        private static ReportDataSelection_Form _instance;
        private int _initialFormWidth;
        private int _initialLeftPanelWidth;
        private int _initialRightPanelWidth;
        private CustomCheckListBox ChartSelection_CheckedListBox;
        private readonly List<MainMenu_Form.ChartDataType> _chartTypeOrder = [];
        private bool _isUpdating;
        private int _previousTemplateIndex = 0;
        private static ReportConfiguration? ReportConfig => ReportGenerator_Form.Instance.CurrentReportConfiguration;

        // Getters
        public static ReportDataSelection_Form Instance => _instance;

        // Init.
        public ReportDataSelection_Form()
        {
            InitializeComponent();
            _instance = this;

            // This fixes a bug. I have a similar setup on ReportPreviewExport_Form but it doesn't have this issue. I'm not sure why.
            DateRange_GroupBox.Anchor = AnchorStyles.None;
            DateRange_GroupBox.Height = Right_Panel.Height - Right_Panel.Padding.Bottom - DateRange_GroupBox.Top;
            DateRange_GroupBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;

            AddEventHandlers();
            InitializeDatePresets();
            InitChartSelectionControl();
            SetupChartSelection();
            SetupDateRangeControls();
            SetupTemplates();
            StoreInitialSizes();
            ScaleControls();
        }
        private void AddEventHandlers()
        {
            Today_Label.Click += (_, _) => Today_RadioButton.Checked = true;
            Yesterday_Label.Click += (_, _) => Yesterday_RadioButton.Checked = true;
            Last7Days_Label.Click += (_, _) => Last7Days_RadioButton.Checked = true;
            Last30Days_Label.Click += (_, _) => Last30Days_RadioButton.Checked = true;
            ThisMonth_Label.Click += (_, _) => ThisMonth_RadioButton.Checked = true;
            LastMonth_Label.Click += (_, _) => LastMonth_RadioButton.Checked = true;
            ThisQuarter_Label.Click += (_, _) => ThisQuarter_RadioButton.Checked = true;
            LastQuarter_Label.Click += (_, _) => LastQuarter_RadioButton.Checked = true;
            YearToDate_Label.Click += (_, _) => YearToDate_RadioButton.Checked = true;
            LastYear_Label.Click += (_, _) => LastYear_RadioButton.Checked = true;
            AllTime_Label.Click += (_, _) => AllTime_RadioButton.Checked = true;
            Custom_Label.Click += (_, _) => Custom_RadioButton.Checked = true;
        }
        private void InitChartSelectionControl()
        {
            ChartSelection_CheckedListBox = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(10)
            };

            ChartSelection_CheckedListBox.SetBounds(
                ChartSelection_GroupBox.Padding.Left + 10,  // Left
                ChartSelection_GroupBox.Padding.Top + ChartSelection_GroupBox.CustomBorderThickness.Top + 10,  // Top (allowing for group box title)
                ChartSelection_GroupBox.ClientSize.Width - (ChartSelection_GroupBox.Padding.Horizontal + 20),  // Width
                ChartSelection_GroupBox.ClientSize.Height - (ChartSelection_GroupBox.Padding.Vertical + 120)   // Height
            );

            ThemeManager.CustomizeScrollBar(ChartSelection_CheckedListBox);
            ChartSelection_CheckedListBox.ItemCheck += new ItemCheckEventHandler(ChartSelection_CheckedListBox_ItemCheck);
            ChartSelection_CheckedListBox.AccessibleDescription = AccessibleDescriptionManager.DoNotTranslate;

            ChartSelection_GroupBox.Controls.Add(ChartSelection_CheckedListBox);
        }
        private void SetupChartSelection()
        {
            string title;

            // Overview Section
            title = LanguageManager.TranslateString("Overview");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.SalesVsExpenses, TranslatedChartTitles.SalesVsExpenses);
            AddChartItem(MainMenu_Form.ChartDataType.TotalProfits, TranslatedChartTitles.TotalProfits);
            AddChartItem(MainMenu_Form.ChartDataType.TotalTransactions, TranslatedChartTitles.TotalTransactions);
            AddChartItem(MainMenu_Form.ChartDataType.AverageTransactionValue, TranslatedChartTitles.AverageTransactionValue);

            // Financial Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Financial");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.TotalRevenue, TranslatedChartTitles.TotalRevenue);
            AddChartItem(MainMenu_Form.ChartDataType.TotalExpenses, TranslatedChartTitles.TotalExpenses);
            AddChartItem(MainMenu_Form.ChartDataType.RevenueDistribution, TranslatedChartTitles.RevenueDistribution);
            AddChartItem(MainMenu_Form.ChartDataType.ExpensesDistribution, TranslatedChartTitles.ExpensesDistribution);

            // Geographic Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Geographic Analysis");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.WorldMap, TranslatedChartTitles.WorldMap);
            AddChartItem(MainMenu_Form.ChartDataType.CountriesOfOrigin, TranslatedChartTitles.CountriesOfOrigin);
            AddChartItem(MainMenu_Form.ChartDataType.CountriesOfDestination, TranslatedChartTitles.CountriesOfDestination);
            AddChartItem(MainMenu_Form.ChartDataType.CompaniesOfOrigin, TranslatedChartTitles.CompaniesOfOrigin);

            // Performance Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Performance");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.GrowthRates, TranslatedChartTitles.GrowthRates);

            // Operational Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Operational");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.AccountantsTransactions, TranslatedChartTitles.AccountantsTransactions);
            AddChartItem(MainMenu_Form.ChartDataType.AverageShippingCosts, TranslatedChartTitles.AverageShippingCosts);

            // Returns Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Returns Analysis");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.ReturnsOverTime, TranslatedChartTitles.ReturnsOverTime);
            AddChartItem(MainMenu_Form.ChartDataType.ReturnReasons, TranslatedChartTitles.ReturnReasons);
            AddChartItem(MainMenu_Form.ChartDataType.ReturnFinancialImpact, TranslatedChartTitles.ReturnFinancialImpact);
            AddChartItem(MainMenu_Form.ChartDataType.ReturnsByCategory, TranslatedChartTitles.ReturnsByCategory);
            AddChartItem(MainMenu_Form.ChartDataType.ReturnsByProduct, TranslatedChartTitles.ReturnsByProduct);
            AddChartItem(MainMenu_Form.ChartDataType.PurchaseVsSaleReturns, TranslatedChartTitles.PurchaseVsSaleReturns);

            // Lost Products Section
            ChartSelection_CheckedListBox.AddSpacer(10);
            title = LanguageManager.TranslateString("Lost Products");
            ChartSelection_CheckedListBox.AddSection(title);
            AddChartItem(MainMenu_Form.ChartDataType.LossesOverTime, TranslatedChartTitles.LossesOverTime);
            AddChartItem(MainMenu_Form.ChartDataType.LossReasons, TranslatedChartTitles.LossReasons);
            AddChartItem(MainMenu_Form.ChartDataType.LossFinancialImpact, TranslatedChartTitles.LossFinancialImpact);
            AddChartItem(MainMenu_Form.ChartDataType.LossesByCategory, TranslatedChartTitles.LossesByCategory);
            AddChartItem(MainMenu_Form.ChartDataType.LossesByProduct, TranslatedChartTitles.LossesByProduct);
            AddChartItem(MainMenu_Form.ChartDataType.PurchaseVsSaleLosses, TranslatedChartTitles.PurchaseVsSaleLosses);

            ThemeManager.CustomizeScrollBar(ChartSelection_CheckedListBox.ContainerPanel);
        }
        private void AddChartItem(MainMenu_Form.ChartDataType chartType, string displayName)
        {
            _chartTypeOrder.Add(chartType);
            ChartSelection_CheckedListBox.Add(displayName, false);
        }
        private void SetupDateRangeControls()
        {
            // Set initial date range
            PerformUpdate(() =>
            {
                // Set default selection to "This month"
                DatePreset? thisMonthPreset = _datePresets.FirstOrDefault(p => p.Name == "This month");
                if (thisMonthPreset != null)
                {
                    thisMonthPreset.RadioButton.Checked = true;
                    (DateTime start, DateTime end) = thisMonthPreset.GetDateRange();
                    StartDate_DateTimePicker.Value = start;
                    EndDate_DateTimePicker.Value = end;
                }

                SetCustomDateRangeControlsVisibility(false);
            });
        }
        private void SetupTemplates()
        {
            Template_ComboBox.Items.Clear();

            // Add templates from ReportTemplates class
            List<string> templates = ReportTemplates.GetAvailableTemplates();
            foreach (string templateName in templates)
            {
                Template_ComboBox.Items.Add(templateName);
            }

            // Select "Custom Report" by default
            PerformUpdate(() =>
            {
                Template_ComboBox.SelectedIndex = 0;
                ReportTitle_TextBox.Text = ReportTemplates.TemplateNames.Custom;
                _previousTemplateIndex = 0;
            });
        }
        public void RefreshTemplates(string selectTemplateName = null)
        {
            int previousIndex = Template_ComboBox.SelectedIndex;
            SetupTemplates();

            // If a specific template name is provided, select it
            if (!string.IsNullOrEmpty(selectTemplateName))
            {
                int newIndex = -1;
                for (int i = 0; i < Template_ComboBox.Items.Count; i++)
                {
                    if (Template_ComboBox.Items[i].ToString() == selectTemplateName)
                    {
                        newIndex = i;
                        break;
                    }
                }

                if (newIndex >= 0)
                {
                    // Update previous index BEFORE setting selected index to prevent reload
                    _previousTemplateIndex = newIndex;
                    Template_ComboBox.SelectedIndex = newIndex;
                    return;
                }
            }

            // Otherwise, try to restore previous selection if still valid
            if (previousIndex >= 0 && previousIndex < Template_ComboBox.Items.Count)
            {
                Template_ComboBox.SelectedIndex = previousIndex;
                _previousTemplateIndex = previousIndex;
            }
            else
            {
                Template_ComboBox.SelectedIndex = 0;
                _previousTemplateIndex = 0;
            }
        }
        private void StoreInitialSizes()
        {
            _initialFormWidth = Width;
            _initialLeftPanelWidth = Left_Panel.Width;
            _initialRightPanelWidth = Right_Panel.Width;
        }
        private void ScaleControls()
        {
            DpiHelper.ScaleComboBox(Template_ComboBox);
            DpiHelper.ScaleGroupBox(ChartSelection_GroupBox);
            DpiHelper.ScaleGroupBox(Template_GroupBox);
            DpiHelper.ScaleGroupBox(ReportSettings_GroupBox);
            DpiHelper.ScaleGroupBox(DateRange_GroupBox);
        }
        public void AlignControlsAfterLanguageChange()
        {
            ReportTitle_TextBox.Left = ReportTitle_Label.Right + 10;

            if (ReportTitle_TextBox.Right > ReportSettings_GroupBox.ClientSize.Width - ReportSettings_GroupBox.Padding.Right - 10)
            {
                ReportTitle_TextBox.Width = ReportSettings_GroupBox.ClientSize.Width - ReportSettings_GroupBox.Padding.Right - 10 - ReportTitle_TextBox.Left;
            }

            // Determine which label is wider
            int maxRight = Math.Max(StartDate_Label.Right, EndDate_Label.Right);

            // Align both DateTimePickers to the widest label
            StartDate_DateTimePicker.Left = maxRight + 10;
            EndDate_DateTimePicker.Left = maxRight + 10;

        }

        /// <summary>
        /// Manages date presets and their associated radio buttons.
        /// </summary>
        private class DatePreset
        {
            public Guna2CustomRadioButton RadioButton { get; set; }
            public string Name { get; set; }
            public Func<(DateTime start, DateTime end)> GetDateRange { get; set; }
        }

        private List<DatePreset> _datePresets;

        /// <summary>
        /// Initialize date presets list in constructor or setup method.
        /// </summary>
        private void InitializeDatePresets()
        {
            DateTime today = DateTime.Today;

            _datePresets =
            [
                new DatePreset
                {
                    RadioButton = Today_RadioButton,
                    Name = "Today",
                    GetDateRange = () => (today, today)
                },
                new DatePreset
                {
                    RadioButton = Yesterday_RadioButton,
                    Name = "Yesterday",
                    GetDateRange = () => (today.AddDays(-1), today.AddDays(-1))
                },
                new DatePreset
                {
                    RadioButton = Last7Days_RadioButton,
                    Name = "Last 7 days",
                    GetDateRange = () => (today.AddDays(-7), today)
                },
                new DatePreset
                {
                    RadioButton = Last30Days_RadioButton,
                    Name = "Last 30 days",
                    GetDateRange = () => (today.AddDays(-30), today)
                },
                new DatePreset
                {
                    RadioButton = ThisMonth_RadioButton,
                    Name = "This month",
                    GetDateRange = () => (new DateTime(today.Year, today.Month, 1), today)
                },
                new DatePreset
                {
                    RadioButton = LastMonth_RadioButton,
                    Name = "Last month",
                    GetDateRange = () =>
                    {
                        DateTime firstDay = today.AddMonths(-1);
                        DateTime start = new(firstDay.Year, firstDay.Month, 1);
                        return (start, start.AddMonths(1).AddDays(-1));
                    }
                },
                new DatePreset
                {
                    RadioButton = ThisQuarter_RadioButton,
                    Name = "This quarter",
                    GetDateRange = () =>
                    {
                        int quarter = (today.Month - 1) / 3;
                        return (new DateTime(today.Year, quarter * 3 + 1, 1), today);
                    }
                },
                new DatePreset
                {
                    RadioButton = LastQuarter_RadioButton,
                    Name = "Last quarter",
                    GetDateRange = () =>
                    {
                        int quarter = ((today.Month - 1) / 3) - 1;
                        int year = today.Year;
                        if (quarter < 0) { quarter = 3; year--; }
                        DateTime start = new(year, quarter * 3 + 1, 1);
                        return (start, start.AddMonths(3).AddDays(-1));
                    }
                },
                new DatePreset
                {
                    RadioButton = YearToDate_RadioButton,
                    Name = "Year to date",
                    GetDateRange = () => (new DateTime(today.Year, 1, 1), today)
                },
                new DatePreset
                {
                    RadioButton = LastYear_RadioButton,
                    Name = "Last year",
                    GetDateRange = () => (new DateTime(today.Year - 1, 1, 1), new DateTime(today.Year - 1, 12, 31))
                },
                new DatePreset
                {
                    RadioButton = AllTime_RadioButton,
                    Name = "All time",
                    GetDateRange = () => (GetEarliestTransactionDate(), today)
                }
            ];
        }
        public void UpdateScrollBarTheme()
        {
            ThemeManager.CustomizeScrollBar(ChartSelection_CheckedListBox.ContainerPanel);
        }

        // Helper methods
        private MainMenu_Form.ChartDataType GetChartTypeFromIndex(int index)
        {
            if (index >= 0 && index < _chartTypeOrder.Count)
            {
                return _chartTypeOrder[index];
            }
            return MainMenu_Form.ChartDataType.TotalRevenue;
        }
        private List<MainMenu_Form.ChartDataType> GetSelectedChartTypes()
        {
            List<MainMenu_Form.ChartDataType> selectedCharts = [];

            for (int i = 0; i < ChartSelection_CheckedListBox.CheckedIndices.Count; i++)
            {
                int checkedIndex = ChartSelection_CheckedListBox.CheckedIndices[i];
                selectedCharts.Add(GetChartTypeFromIndex(checkedIndex));
            }

            return selectedCharts;
        }
        public void SwitchToCustomTemplate()
        {
            if (Template_ComboBox.SelectedIndex > 0)
            {
                PerformUpdate(() =>
                {
                    Template_ComboBox.SelectedIndex = 0;
                    _previousTemplateIndex = 0;
                    if (ReportConfig != null)
                    {
                        ReportConfig.Title = ReportTemplates.TemplateNames.Custom;
                    }
                });
            }
        }
        public void UpdateLanguageForChartSelectionControl()
        {
            if (_isUpdating) { return; }

            PerformUpdate(() =>
            {
                for (int i = 0; i < ChartSelection_CheckedListBox.Count; i++)
                {
                    MainMenu_Form.ChartDataType chartType = GetChartTypeFromIndex(i);
                    string displayName = TranslatedChartTitles.GetChartDisplayName(chartType);
                    ChartSelection_CheckedListBox.Items[i] = displayName;
                }
            });
        }

        // Form event handlers
        private void ReportDataSelection_Form_VisibleChanged(object sender, EventArgs e)
        {
            if (!_isUpdating && !Visible)
            {
                OnStepDeactivated();
            }
        }
        private void ReportDataSelection_Form_Resize(object sender, EventArgs e)
        {
            if (_initialFormWidth == 0) { return; }

            // Calculate the form's width change ratio
            float widthRatio = (float)Width / _initialFormWidth;

            // Calculate new panel widths while maintaining proportion
            Left_Panel.Width = (int)(_initialLeftPanelWidth * widthRatio);
            Right_Panel.Width = (int)(_initialRightPanelWidth * widthRatio);

            // Position the right panel
            Right_Panel.Left = Left_Panel.Width;
        }

        // Event handlers
        private void SelectAll_Button_Click(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            PerformUpdate(() =>
            {
                for (int i = 0; i < ChartSelection_CheckedListBox.Items.Count; i++)
                {
                    ChartSelection_CheckedListBox.SetItemChecked(i, true);
                }

                // Update the report configuration with current selections
                if (ReportConfig != null)
                {
                    ReportConfig.Filters.SelectedChartTypes = GetSelectedChartTypes();
                    UpdateElementsFromChartSelection();
                }

                NotifyParentValidationChanged();
            });
        }
        private void SelectNone_Button_Click(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            PerformUpdate(() =>
            {
                for (int i = 0; i < ChartSelection_CheckedListBox.Items.Count; i++)
                {
                    ChartSelection_CheckedListBox.SetItemChecked(i, false);
                }

                // Update the report configuration with current selections
                if (ReportConfig != null)
                {
                    ReportConfig.Filters.SelectedChartTypes = GetSelectedChartTypes();
                    UpdateElementsFromChartSelection();
                }

                NotifyParentValidationChanged();
            });
        }
        private void OpenTemplates_Button_Click(object sender, EventArgs e)
        {
            using CustomTemplateManager_Form form = new();

            if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(form.SelectedTemplateName))
            {
                // Load the selected template
                LoadCustomTemplate(form.SelectedTemplateName);

                // Show ReportDataSelection_Form
                ReportGenerator_Form.Instance.Next_Button.PerformClick();
            }
        }
        private void LoadCustomTemplate(string templateName)
        {
            ReportConfiguration config = CustomTemplateStorage.LoadTemplate(templateName);

            if (config == null)
            {
                CustomMessageBox.ShowWithFormat(
                    "Load Failed",
                    "Failed to load template '{0}'.",
                    CustomMessageBoxIcon.Error,
                    CustomMessageBoxButtons.Ok,
                    templateName);
                return;
            }

            // Apply the loaded configuration
            if (ReportConfig != null)
            {
                ApplyLoadedConfiguration(config);

                // Update the template combo box to show the loaded template name
                int templateIndex = GetTemplateIndex(templateName);
                if (templateIndex >= 0)
                {
                    PerformUpdate(() =>
                    {
                        Template_ComboBox.SelectedIndex = templateIndex;
                        _previousTemplateIndex = templateIndex;
                    });
                }
            }
        }
        private int GetTemplateIndex(string templateName)
        {
            for (int i = 0; i < Template_ComboBox.Items.Count; i++)
            {
                if (Template_ComboBox.Items[i].ToString() == templateName)
                {
                    return i;
                }
            }
            return -1;
        }
        private void ApplyLoadedConfiguration(ReportConfiguration config)
        {
            PerformUpdate(() =>
            {
                // Update title
                ReportTitle_TextBox.Text = config.Title;
                ReportConfig.Title = config.Title;

                // Update page settings
                ReportConfig.PageSize = config.PageSize;
                ReportConfig.PageOrientation = config.PageOrientation;
                ReportConfig.ShowHeader = config.ShowHeader;
                ReportConfig.ShowFooter = config.ShowFooter;
                ReportConfig.ShowPageNumbers = config.ShowPageNumbers;
                ReportConfig.BackgroundColor = config.BackgroundColor;
                ReportConfig.PageMargins = config.PageMargins;

                // Update filters
                ReportConfig.Filters.TransactionType = config.Filters.TransactionType;
                ReportConfig.Filters.IncludeReturns = config.Filters.IncludeReturns;
                ReportConfig.Filters.IncludeLosses = config.Filters.IncludeLosses;
                ReportConfig.Filters.DatePresetName = config.Filters.DatePresetName;
                ReportConfig.Filters.StartDate = config.Filters.StartDate;
                ReportConfig.Filters.EndDate = config.Filters.EndDate;
                ReportConfig.Filters.SelectedChartTypes = [.. config.Filters.SelectedChartTypes];

                // Update date preset selection
                ApplyDatePresetByName(config.Filters.DatePresetName);

                // Copy elements
                ReportConfig.Elements.Clear();
                foreach (BaseElement element in config.Elements)
                {
                    ReportConfig.Elements.Add(element);
                }
            });

            // Notify changes
            NotifyParentValidationChanged();
            ReportLayoutDesigner_Form.Instance?.OnConfigurationLoaded();
        }
        private void ChartSelection_CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_isUpdating) { return; }

            // If a non-custom template is selected and user makes changes, switch to custom
            if (Template_ComboBox.SelectedIndex > 0)
            {
                PerformUpdate(() =>
                {
                    Template_ComboBox.SelectedIndex = 0;  // Switch to Custom Report
                    _previousTemplateIndex = 0;
                    if (ReportConfig != null)
                    {
                        ReportConfig.Title = ReportTemplates.TemplateNames.Custom;
                    }
                });
            }

            // Update the report configuration with current selections
            if (ReportConfig != null)
            {
                // Update the selected chart types in filters
                ReportConfig.Filters.SelectedChartTypes = GetSelectedChartTypes();

                UpdateElementsFromChartSelection();
            }

            NotifyParentValidationChanged();
        }
        private void Template_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            int templateIndex = Template_ComboBox.SelectedIndex;

            // Check if template is actually changing
            if (templateIndex == _previousTemplateIndex)
            {
                return;
            }

            // Check for unsaved changes before switching templates
            if (ReportLayoutDesigner_Form.Instance != null && ReportLayoutDesigner_Form.HasUnsavedChanges)
            {
                bool shouldContinue = ReportLayoutDesigner_Form.Instance.PromptToSaveChanges();

                if (!shouldContinue)
                {
                    // User cancelled, revert to previous selection
                    PerformUpdate(() =>
                    {
                        Template_ComboBox.SelectedIndex = _previousTemplateIndex;
                    });
                    return;
                }
            }

            // Apply the new template
            if (templateIndex == 0)
            {
                // Custom Report selected - don't apply any template
                if (ReportConfig != null)
                {
                    PerformUpdate(() =>
                    {
                        ReportTitle_TextBox.Text = ReportTemplates.TemplateNames.Custom;
                        ReportConfig.Title = ReportTemplates.TemplateNames.Custom;
                    });
                }
            }
            else
            {
                ApplyTemplateByIndex(templateIndex);
            }

            // Update the previous template index
            _previousTemplateIndex = templateIndex;
        }
        private void DateRange_ValueChanged(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }

            ValidateDateRange();

            // When user manually changes dates, set to Custom preset
            if (ReportConfig != null)
            {
                ReportConfig.Filters.DatePresetName = ReportTemplates.DatePresetNames.Custom;
                ReportConfig.Filters.StartDate = StartDate_DateTimePicker.Value;
                ReportConfig.Filters.EndDate = EndDate_DateTimePicker.Value;
            }

            SwitchToCustomTemplate();
            NotifyParentValidationChanged();
        }
        private void ReportTitle_TextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_isUpdating)
            {
                NotifyParentValidationChanged();
            }
        }
        private void DatePreset_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (_isUpdating) { return; }
            if (sender is not Guna2CustomRadioButton radioButton || !radioButton.Checked) { return; }

            PerformUpdate(() =>
            {
                if (radioButton == Custom_RadioButton)
                {
                    SetCustomDateRangeControlsVisibility(true);
                    if (ReportConfig != null)
                    {
                        ReportConfig.Filters.DatePresetName = ReportTemplates.DatePresetNames.Custom;
                    }
                }
                else
                {
                    SetCustomDateRangeControlsVisibility(false);

                    DatePreset? selectedPreset = _datePresets.FirstOrDefault(p => p.RadioButton == radioButton);
                    if (selectedPreset != null)
                    {
                        (DateTime start, DateTime end) = selectedPreset.GetDateRange();
                        StartDate_DateTimePicker.Value = start;
                        EndDate_DateTimePicker.Value = end;

                        // Store the preset name in the config
                        if (ReportConfig != null)
                        {
                            ReportConfig.Filters.DatePresetName = selectedPreset.Name;
                            ReportConfig.Filters.StartDate = start;
                            ReportConfig.Filters.EndDate = end;
                        }
                    }
                }

                NotifyParentValidationChanged();
            });
        }

        // Event handler helper methods
        private static DateTime GetEarliestTransactionDate()
        {
            DateTime earliestDate = DateTime.Today;

            // Check sales transactions for earliest date
            foreach (DataGridViewRow row in MainMenu_Form.Instance.Sale_DataGridView.Rows)
            {
                if (row.Cells[ReadOnlyVariables.Date_column].Value != null &&
                    DateTime.TryParse(row.Cells[ReadOnlyVariables.Date_column].Value.ToString(), out DateTime date))
                {
                    if (date < earliestDate)
                    {
                        earliestDate = date;
                    }
                }
            }

            // Check purchase transactions for earliest date
            foreach (DataGridViewRow row in MainMenu_Form.Instance.Purchase_DataGridView.Rows)
            {
                if (row.Cells[ReadOnlyVariables.Date_column].Value != null &&
                    DateTime.TryParse(row.Cells[ReadOnlyVariables.Date_column].Value.ToString(), out DateTime date))
                {
                    if (date < earliestDate)
                    {
                        earliestDate = date;
                    }
                }
            }

            // If no transactions found, default to reasonable date
            if (earliestDate == DateTime.Today)
            {
                earliestDate = new DateTime(2000, 1, 1);
            }

            return earliestDate;
        }
        private void ValidateDateRange()
        {
            if (!HasValidDateRange())
            {
                SetDateTimeControlsBorderColor(CustomColors.AccentRed);

                CustomMessageBoxResult result = CustomMessageBox.Show(
                    "Invalid Date Range",
                    "The 'Start' date cannot be later than the 'End' date.\nWould you like to swap the dates automatically?",
                    CustomMessageBoxIcon.Question,
                    CustomMessageBoxButtons.YesNo);

                if (result == CustomMessageBoxResult.Yes)
                {
                    // Swap the dates
                    (StartDate_DateTimePicker.Value, EndDate_DateTimePicker.Value) = (EndDate_DateTimePicker.Value, StartDate_DateTimePicker.Value);

                    SetDateTimeControlsBorderColor(CustomColors.ControlBorder);
                }
            }
            else
            {
                SetDateTimeControlsBorderColor(CustomColors.ControlBorder);
            }
        }
        private void SetDateTimeControlsBorderColor(Color color)
        {
            StartDate_DateTimePicker.BorderColor = color;
            EndDate_DateTimePicker.BorderColor = color;
        }
        private void ApplyTemplateByIndex(int index)
        {
            if (index < 0) { return; }

            _isUpdating = true;
            try
            {
                ReportConfiguration template = ReportTemplates.CreateFromTemplateIndex(index);

                // Apply the template configuration to the parent's report config
                if (ReportConfig != null)
                {
                    ReportConfig.HasManualChartLayout = false;

                    // Clear existing elements
                    ReportConfig.Elements.Clear();

                    // Copy all template elements
                    foreach (BaseElement element in template.Elements)
                    {
                        ReportConfig.AddElement(element.Clone());
                    }

                    // Copy page settings
                    ReportConfig.PageSize = template.PageSize;
                    ReportConfig.PageOrientation = template.PageOrientation;
                    ReportConfig.ShowHeader = template.ShowHeader;
                    ReportConfig.ShowFooter = template.ShowFooter;
                    ReportConfig.ShowPageNumbers = template.ShowPageNumbers;
                    ReportConfig.BackgroundColor = template.BackgroundColor;
                    ReportConfig.PageMargins = template.PageMargins;

                    // Copy filters from template
                    ReportConfig.Filters.SelectedChartTypes = [.. template.Filters.SelectedChartTypes];
                    ReportConfig.Filters.TransactionType = template.Filters.TransactionType;
                    ReportConfig.Filters.StartDate = template.Filters.StartDate;
                    ReportConfig.Filters.EndDate = template.Filters.EndDate;
                    ReportConfig.Filters.IncludeReturns = template.Filters.IncludeReturns;
                    ReportConfig.Filters.IncludeLosses = template.Filters.IncludeLosses;

                    // Copy template metadata
                    ReportConfig.Title = template.Title;
                }

                // Apply the template settings to the form controls
                PerformUpdate(() =>
                {
                    // Update chart selection
                    for (int i = 0; i < ChartSelection_CheckedListBox.Items.Count; i++)
                    {
                        MainMenu_Form.ChartDataType chartType = GetChartTypeFromIndex(i);
                        bool isSelected = template.Filters.SelectedChartTypes.Contains(chartType);
                        ChartSelection_CheckedListBox.SetItemChecked(i, isSelected);
                    }

                    // Update report title
                    ReportTitle_TextBox.Text = template.Title;

                    // Update date range
                    if (!string.IsNullOrEmpty(template.Filters.DatePresetName))
                    {
                        ApplyDatePresetByName(template.Filters.DatePresetName);
                    }
                    else
                    {
                        // Fallback to custom dates if no preset specified
                        DateTime startDate = template.Filters.StartDate ?? DateTime.Now.AddMonths(-1);
                        DateTime endDate = template.Filters.EndDate ?? DateTime.Now;

                        StartDate_DateTimePicker.Value = startDate;
                        EndDate_DateTimePicker.Value = endDate;
                        Custom_RadioButton.Checked = true;
                        SetCustomDateRangeControlsVisibility(true);
                    }
                });

                NotifyParentValidationChanged();

                // Notify layout designer that configuration has been loaded
                ReportLayoutDesigner_Form.Instance?.OnConfigurationLoaded();
            }
            finally
            {
                _isUpdating = false;
            }
        }
        private void ApplyDatePresetByName(string presetName)
        {
            DatePreset? preset = _datePresets.FirstOrDefault(p =>
                p.Name.Equals(presetName, StringComparison.OrdinalIgnoreCase));

            if (preset != null)
            {
                preset.RadioButton.Checked = true;
                (DateTime start, DateTime end) = preset.GetDateRange();
                StartDate_DateTimePicker.Value = start;
                EndDate_DateTimePicker.Value = end;
                SetCustomDateRangeControlsVisibility(false);
            }
            else
            {
                // If preset not found, default to "This month"
                DatePreset? defaultPreset = _datePresets.FirstOrDefault(p => p.Name == "This month");
                if (defaultPreset != null)
                {
                    defaultPreset.RadioButton.Checked = true;
                    (DateTime start, DateTime end) = defaultPreset.GetDateRange();
                    StartDate_DateTimePicker.Value = start;
                    EndDate_DateTimePicker.Value = end;
                    SetCustomDateRangeControlsVisibility(false);
                }
            }
        }
        private void SetCustomDateRangeControlsVisibility(bool visible)
        {
            StartDate_Label.Visible = visible;
            StartDate_DateTimePicker.Visible = visible;
            EndDate_Label.Visible = visible;
            EndDate_DateTimePicker.Visible = visible;
        }

        /// <summary>
        /// Updates the ReportConfig.Elements collection based on current chart selections.
        /// Adds/removes charts and repositions all charts.
        /// </summary>
        private void UpdateElementsFromChartSelection()
        {
            if (ReportConfig == null) { return; }

            List<MainMenu_Form.ChartDataType> selectedCharts = GetSelectedChartTypes();
            List<ChartElement> existingCharts = ReportConfig.GetElementsOfType<ChartElement>();

            // Remove charts that are no longer selected
            List<ChartElement> chartsToRemove = existingCharts
                .Where(chart => !selectedCharts.Contains(chart.ChartType))
                .ToList();

            foreach (ChartElement chart in chartsToRemove)
            {
                ReportConfig.Elements.Remove(chart);
            }

            // Clear selection for removed charts in the layout designer
            if (chartsToRemove.Count > 0)
            {
                ReportLayoutDesigner_Form.Instance.ClearSelectionForRemovedElements();
            }

            // Find which chart types need to be added
            List<MainMenu_Form.ChartDataType> existingChartTypes = existingCharts
                .Where(chart => selectedCharts.Contains(chart.ChartType))
                .Select(chart => chart.ChartType)
                .ToList();

            List<MainMenu_Form.ChartDataType> chartsToAdd = selectedCharts
                .Where(chartType => !existingChartTypes.Contains(chartType))
                .ToList();

            // Add new chart elements
            foreach (MainMenu_Form.ChartDataType chartType in chartsToAdd)
            {
                ChartElement newChartElement = new()
                {
                    ChartType = chartType,
                    Bounds = new Rectangle(50, 50, 350, 250)
                };
                ReportConfig.AddElement(newChartElement);
            }

            // Only auto-arrange if user hasn't manually positioned/resized any charts
            if (!ReportConfig.HasManualChartLayout)
            {
                // Now reposition and resize charts
                List<ChartElement> allCharts = ReportConfig.GetElementsOfType<ChartElement>();

                if (allCharts.Count > 0)
                {
                    // Get page dimensions
                    Size pageSize = PageDimensions.GetDimensions(
                        ReportConfig.PageSize,
                        ReportConfig.PageOrientation
                    );

                    // Calculate available space
                    int margin = ReportConfig.PageMargins?.Left ?? 40;
                    int headerHeight = ReportConfig.ShowHeader ? 80 : 0;
                    int footerHeight = ReportConfig.ShowFooter ? 50 : 0;

                    int availableWidth = pageSize.Width - (margin * 2);
                    int availableHeight = pageSize.Height - headerHeight - footerHeight - (margin * 2);

                    const int spacing = 20;
                    int startX = margin;
                    int startY = headerHeight + margin;

                    int totalCharts = allCharts.Count;

                    // Dynamically calculate columns based on chart count
                    int columns;
                    if (totalCharts == 1)
                    {
                        columns = 1;
                    }
                    else if (totalCharts <= 4)
                    {
                        columns = 2;
                    }
                    else if (totalCharts <= 9)
                    {
                        columns = 3;
                    }
                    else if (totalCharts <= 16)
                    {
                        columns = 4;
                    }
                    else
                    {
                        columns = 5;
                    }

                    // Calculate rows needed
                    int rows = (int)Math.Ceiling((double)totalCharts / columns);

                    // Calculate chart dimensions to fit within available space
                    int chartWidth = (availableWidth - (spacing * (columns - 1))) / columns;
                    int chartHeight = (availableHeight - (spacing * (rows - 1))) / rows;

                    // Enforce aspect ratio of 2:1
                    int maxHeightForAspectRatio = chartWidth / 2;
                    if (chartHeight > maxHeightForAspectRatio)
                    {
                        chartHeight = maxHeightForAspectRatio;
                    }

                    // Position all charts
                    int index = 0;
                    foreach (ChartElement chart in allCharts)
                    {
                        int row = index / columns;
                        int col = index % columns;

                        int x = startX + (col * (chartWidth + spacing));
                        int y = startY + (row * (chartHeight + spacing));

                        chart.Bounds = new Rectangle(x, y, chartWidth, chartHeight);
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// Synchronizes the chart selection CheckedListBox with the current SelectedChartTypes in ReportConfig.
        /// This is called when charts are deleted from the layout designer.
        /// </summary>
        public void SyncChartSelectionFromConfig()
        {
            if (ReportConfig == null) { return; }

            PerformUpdate(() =>
            {
                for (int i = 0; i < ChartSelection_CheckedListBox.Items.Count; i++)
                {
                    MainMenu_Form.ChartDataType chartType = GetChartTypeFromIndex(i);
                    bool shouldBeChecked = ReportConfig.Filters.SelectedChartTypes.Contains(chartType);

                    if (ChartSelection_CheckedListBox.GetItemChecked(i) != shouldBeChecked)
                    {
                        ChartSelection_CheckedListBox.SetItemChecked(i, shouldBeChecked);
                    }
                }
            });
        }

        // Form implementation methods
        private bool HasValidDateRange()
        {
            return StartDate_DateTimePicker.Value <= EndDate_DateTimePicker.Value;
        }
        public bool IsValidForNextStep()
        {
            bool hasTitle = !string.IsNullOrWhiteSpace(ReportTitle_TextBox.Text);

            return HasValidDateRange() && hasTitle;
        }

        /// <summary>
        /// Called when the form becomes inactive (user navigates away from this step).
        /// </summary>
        public void OnStepDeactivated()
        {
            if (ReportConfig == null) { return; }

            // Update filters
            ReportConfig.Filters.StartDate = StartDate_DateTimePicker.Value;
            ReportConfig.Filters.EndDate = EndDate_DateTimePicker.Value;
            ReportConfig.Filters.SelectedChartTypes = GetSelectedChartTypes();

            // Update template
            if (Template_ComboBox.SelectedIndex > 0)
            {
                ReportConfig.Title = Template_ComboBox.SelectedItem?.ToString();
            }

            // Update title if provided
            if (!string.IsNullOrWhiteSpace(ReportTitle_TextBox.Text))
            {
                ReportConfig.Title = ReportTitle_TextBox.Text;
            }
        }

        // Helper Methods for Base Functionality
        /// <summary>
        /// Notifies the parent form that validation state has changed
        /// </summary>
        private static void NotifyParentValidationChanged()
        {
            ReportGenerator_Form.Instance.OnChildFormValidationChanged();
        }

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