using Guna.UI2.WinForms;
using Sales_Tracker.Charts;
using Sales_Tracker.Classes;
using Sales_Tracker.DataClasses;
using Sales_Tracker.GridView;
using Sales_Tracker.ImportSpreadsheet;
using Sales_Tracker.Language;
using Sales_Tracker.Properties;
using Sales_Tracker.Rentals;
using Sales_Tracker.ReportGenerator;
using Sales_Tracker.Settings;
using Sales_Tracker.Startup;
using Sales_Tracker.Startup.Menus;
using Sales_Tracker.Theme;
using System.Drawing.Drawing2D;
using System.Globalization;
using Timer = System.Windows.Forms.Timer;

namespace Sales_Tracker.UI
{
    /// <summary>
    /// Manages the construction and customization of various UI controls, including menus, buttons, panels, and validation styling. 
    /// This class helps streamline UI setup and ensures consistency across different control types.
    /// </summary>
    internal static class CustomControls
    {
        /// <summary>
        /// Constructs various UI controls used throughout the application, including FileMenu, HelpMenu, and more.
        /// </summary>
        public static void ConstructControls()
        {
            CascadingMenu.Init();

            // Main menu controls
            ConstructRecentlyOpenedMenu();
            ConstructFileMenu();
            ConstructHelpMenu();
            ConstructControlsDropDownButton();
            ConstructControlsDropDownMenu();

            // Other controls
            RightClickDataGridViewRowMenu.Construct();
            RightClickChartMenu.Construct();
            CompanyLogo.ConstructRightClickMenu();
            ColumnVisibilityPanel.Construct();
            RightClickElementMenu.Construct();
            UndoRedoHistoryDropdown.Construct();

            // Set language
            LanguageManager.UpdateLanguageForControl(RecentlyOpenedMenu);
            LanguageManager.UpdateLanguageForControl(FileMenu);
            LanguageManager.UpdateLanguageForControl(HelpMenu);
            LanguageManager.UpdateLanguageForControl(ControlsDropDown_Button);
            LanguageManager.UpdateLanguageForControl(ControlDropDown_Panel);
            LanguageManager.UpdateLanguageForControl(RightClickDataGridViewRowMenu.Panel);
            LanguageManager.UpdateLanguageForControl(RightClickChartMenu.Panel);
            LanguageManager.UpdateLanguageForControl(CompanyLogo.CompanyLogoRightClick_Panel);
            LanguageManager.UpdateLanguageForControl(RightClickElementMenu.Panel);
            LanguageManager.UpdateLanguageForControl(UndoRedoHistoryDropdown.Panel);
        }

        // Properties
        private static readonly byte _spaceForSeparator = 11, _offsetForKeyboardShortcutOrArrow = 15;
        public enum KeyPressValidation
        {
            OnlyNumbersAndDecimalAndMinus,
            OnlyNumbersAndDecimal,
            OnlyNumbers,
            OnlyLetters,
            None
        }

        // Getters
        public static short PanelWidth { get; } = 350;
        public static short PanelBtnWidth { get; } = 340;
        public static byte PanelButtonHeight { get; } = 35;
        public static byte SpaceForPanel { get; } = 10;
        public static byte SpaceBetweenControls { get; } = 8;

        // Construct things for menus
        public static Guna2Panel ConstructPanelForMenu(Size size, string name)
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            // Only scale width, height is pre-calculated with scaled values
            Size scaledSize = new((int)(size.Width * scale), size.Height);

            Guna2Panel panel = new()
            {
                BorderStyle = DashStyle.Solid,
                BorderColor = CustomColors.ControlPanelBorder,
                BorderThickness = 1,
                BorderRadius = 4,
                FillColor = CustomColors.PanelBtn,
                Size = scaledSize,
                Name = name
            };

            int scaledHalf = (int)(SpaceForPanel * scale / 2);
            FlowLayoutPanel flowLayoutPanel = new()
            {
                BackColor = CustomColors.PanelBtn,
                Size = new Size(scaledSize.Width - (int)(SpaceForPanel * scale), scaledSize.Height - (int)(SpaceForPanel * scale)),
                Location = new Point(scaledHalf, scaledHalf)
            };
            panel.Controls.Add(flowLayoutPanel);
            return panel;
        }
        public static Guna2Separator ConstructSeparator(int width, Control control)
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledWidth = (int)(width * scale);
            int scaledMargin = (int)(5 * scale);

            Guna2Separator seperator = new()
            {
                FillColor = CustomColors.ControlBorder,
                BackColor = CustomColors.PanelBtn,
                Size = new Size(scaledWidth, 1),
                Margin = new Padding(0, scaledMargin, 0, scaledMargin)
            };
            control.Controls.Add(seperator);
            return seperator;
        }
        public static Guna2Button ConstructBtnForMenu(string text, int width, Control control, bool scaleFontSize = false)
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            int scaledWidth = (int)(width * scale);
            int scaledHeight = (int)(PanelButtonHeight * scale);

            float fontSize = scaleFontSize && scale < 1
                ? 10 * scale
                : 10;

            Guna2Button menuBtn = new()
            {
                Size = new Size(scaledWidth, scaledHeight),
                FillColor = CustomColors.PanelBtn,
                ForeColor = CustomColors.Text,
                TextAlign = HorizontalAlignment.Left,
                Font = new Font("Segoe UI", fontSize),
                Text = LanguageManager.TranslateString(text),
                Name = FormatControlName(text, "_Button"),
                Margin = new Padding(0),
                BorderColor = CustomColors.ControlBorder,
                PressedColor = CustomColors.PanelBtnHover,
                HoverState = {
                    FillColor = CustomColors.PanelBtnHover,
                    BorderColor = CustomColors.ControlBorder
                },
            };

            menuBtn.MouseEnter += (sender, e) =>
            {
                Guna2Button btn = (Guna2Button)sender;
                Label label = btn.Controls.OfType<Label>().FirstOrDefault();

                if (label != null)
                {
                    label.BackColor = CustomColors.PanelBtnHover;
                }
                btn.BorderThickness = 1;
            };
            menuBtn.MouseLeave += (sender, e) =>
            {
                Guna2Button btn = (Guna2Button)sender;
                Label label = btn.Controls.OfType<Label>().FirstOrDefault();

                if (label != null)
                {
                    label.BackColor = CustomColors.PanelBtn;
                }
                btn.BorderThickness = 0;
            };

            control.Controls.Add(menuBtn);
            return menuBtn;
        }
        public static void ConstructKeyShortcut(string text, Control control)
        {
            Label KeyShortcut = new()
            {
                ForeColor = CustomColors.Text,
                BackColor = CustomColors.PanelBtn,
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = true,
                Name = FormatControlName(text, "_Label"),
                Text = text,
                AccessibleDescription = AccessibleDescriptionManager.AlignRight,
                Anchor = AnchorStyles.Top
            };

            KeyShortcut.MouseEnter += (sender, e) =>
            {
                Label label = (Label)sender;
                label.BackColor = CustomColors.PanelBtnHover;
                Guna2Button btn = (Guna2Button)label.Parent;
                btn.FillColor = CustomColors.PanelBtnHover;
                btn.BorderThickness = 1;
            };
            KeyShortcut.MouseLeave += (sender, e) =>
            {
                Label label = (Label)sender;
                label.BackColor = CustomColors.PanelBtn;
                Guna2Button btn = (Guna2Button)label.Parent;
                btn.FillColor = CustomColors.PanelBtn;
                btn.BorderThickness = 0;
            };
            KeyShortcut.MouseUp += (sender, e) =>
            {
                Label label = (Label)sender;
                Guna2Button btn = (Guna2Button)label.Parent;
                btn.PerformClick();
            };

            control.Controls.Add(KeyShortcut);

            // Position it after adding so AutoSize is calculated
            // Scale the offset for the larger button
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledOffset = (int)(_offsetForKeyboardShortcutOrArrow * scale);

            KeyShortcut.Left = control.Width - KeyShortcut.Width - scaledOffset;
            KeyShortcut.Top = (control.Height - KeyShortcut.Height) / 2;  // Center vertically
        }

        // FileMenu
        public static Guna2Panel FileMenu { get; set; }
        public static Guna2Panel RecentlyOpenedMenu { get; set; }
        public static Guna2Button OpenRecentCompany_Button { get; set; }
        private static void ConstructFileMenu()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(PanelButtonHeight * scale);
            int scaledSeparatorSpace = (int)(_spaceForSeparator * scale);
            int scaledSpaceForPanel = (int)(SpaceForPanel * scale);

            // Calculate height using scaled values: 10 buttons + 2 separators + panel padding
            int calculatedHeight = 10 * scaledButtonHeight + scaledSeparatorSpace * 2 + scaledSpaceForPanel;

            FileMenu = ConstructPanelForMenu(new Size(PanelWidth, calculatedHeight), "fileMenu_Panel");
            FlowLayoutPanel flowPanel = (FlowLayoutPanel)FileMenu.Controls[0];

            Guna2Button menuBtn = ConstructBtnForMenu("Create new company", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                // Save current company
                if (!ArgoCompany.AskUserToSave())
                {
                    return;
                }

                Tools.OpenForm(new Startup_Form(["autoClickButton"]));
            };

            menuBtn = ConstructBtnForMenu("Open company", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                ArgoCompany.OpenCompanyWhenACompanyIsAlreadyOpen();
            };

            menuBtn = ConstructBtnForMenu("Open recent company", PanelBtnWidth, flowPanel);
            float arrowScale = DpiHelper.GetRelativeDpiScale();
            menuBtn.ImageSize = new Size((int)(11 * arrowScale), (int)(11 * arrowScale));
            int scaledOffset = (int)(_offsetForKeyboardShortcutOrArrow * arrowScale);
            menuBtn.ImageOffset = new Point((menuBtn.Width - menuBtn.ImageSize.Width - scaledOffset) - (menuBtn.Width / 2), 0);
            menuBtn.Tag = RecentlyOpenedMenu;
            ThemeManager.SetRightArrowImageBasedOnTheme(menuBtn);
            menuBtn.Click += (sender, e) =>
            {
                Guna2Button btn = (Guna2Button)sender;
                SetRecentlyOpenedMenu();
                CascadingMenu.OpenMenu();
            };
            menuBtn.MouseEnter += (sender, e) =>
            {
                Guna2Button btn = (Guna2Button)sender;
                SetRecentlyOpenedMenu();
            };
            menuBtn.MouseLeave += CascadingMenu.CloseMenu;
            OpenRecentCompany_Button = menuBtn;

            ConstructSeparator(PanelBtnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu("Save", PanelBtnWidth, flowPanel);
            menuBtn.Name = "Save";
            menuBtn.Click += (sender, e) =>
            {
                SaveAll();
            };
            ConstructKeyShortcut("Ctrl+S", menuBtn);

            menuBtn = ConstructBtnForMenu("Save as...", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                ArgoCompany.SaveAs();
            };
            ConstructKeyShortcut("Ctrl+Shift+S", menuBtn);

            menuBtn = ConstructBtnForMenu("Export as...", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                Tools.OpenForm(new Export_Form());
            };
            ConstructKeyShortcut("Ctrl+E", menuBtn);

            menuBtn = ConstructBtnForMenu("Export receipts", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                Tools.OpenForm(new Receipts_Form());
            };

            menuBtn = ConstructBtnForMenu("Generate report", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                Tools.OpenForm(new ReportGenerator_Form());
            };

            menuBtn = ConstructBtnForMenu("Import spreadsheet", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                bool importSpreadsheetTutorial = DataFileManager.GetBoolValue(GlobalAppDataSettings.ImportSpreadsheetTutorial);
                if (importSpreadsheetTutorial)
                {
                    Tools.OpenForm(new Setup_Form());
                }
                else
                {
                    Tools.OpenForm(new ImportSpreadsheet_Form());
                }
            };

            ConstructSeparator(PanelBtnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu("Show company in folder", PanelBtnWidth, flowPanel);
            menuBtn.Click += (sender, e) =>
            {
                Tools.ShowFileInFolder(Directories.ArgoCompany_file);
            };
        }
        private static void ConstructRecentlyOpenedMenu()
        {
            RecentlyOpenedMenu = ConstructPanelForMenu(new Size(PanelWidth, 100), "recentlyOpenedMenu_Panel");
            RecentlyOpenedMenu.MouseEnter += (_, _) => CascadingMenu.KeepMenuOpen();
        }
        private static void SetRecentlyOpenedMenu()
        {
            if (MainMenu_Form.Instance.Controls.Contains(RecentlyOpenedMenu))
            {
                CascadingMenu.KeepMenuOpen();
                return;
            }

            FlowLayoutPanel flowPanel = RecentlyOpenedMenu.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
            flowPanel.Controls.Clear();

            List<string> validCompanyDirs = ArgoCompany.GetValidRecentCompanyPaths(true);

            if (validCompanyDirs.Count == 0)
            {
                LabelManager.AddNoRecentlyOpenedCompanies(flowPanel, PanelBtnWidth, PanelButtonHeight);
            }
            else
            {
                // Construct buttons
                foreach (string companyDir in validCompanyDirs)
                {
                    string text = Path.GetFileNameWithoutExtension(companyDir);
                    Guna2Button menuBtn = ConstructBtnForMenu(text, PanelBtnWidth, flowPanel, scaleFontSize: true);
                    menuBtn.Tag = companyDir;
                    menuBtn.MouseEnter += (sender, e) => CascadingMenu.KeepMenuOpen();
                    menuBtn.Click += (sender, e) =>
                    {
                        Guna2Button button = (Guna2Button)sender;
                        string path = button.Tag.ToString();
                        ArgoCompany.OpenCompanyWhenACompanyIsAlreadyOpenFromPath(path);
                    };
                }
            }

            SetRightClickMenuHeight(RecentlyOpenedMenu);

            // Use scaled button height for positioning
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(PanelButtonHeight * scale);

            RecentlyOpenedMenu.Location = new Point(FileMenu.Right,
                FileMenu.Top + scaledButtonHeight * 2);

            MainMenu_Form.Instance.Controls.Add(RecentlyOpenedMenu);
            RecentlyOpenedMenu.BringToFront();
        }
        public static void SaveAll(bool showLabel = true)
        {
            if (!showLabel)
            {
                if (ArgoCompany.AreAnyChangesMade())
                {
                    ArgoCompany.SaveAll();
                }
                return;
            }

            Label label = MainMenu_Form.Instance.Saved_Label;
            label.Visible = true;

            if (ArgoCompany.AreAnyChangesMade())
            {
                label.ForeColor = CustomColors.AccentGreen;
                label.Text = "Saving...";
                ArgoCompany.SaveAll();
                label.Text = "Saved";
            }
            else
            {
                label.ForeColor = CustomColors.Text;
                label.Text = "No changes found";
            }

            Timer timer = new() { Interval = 3000 };
            timer.Tick += (_, _) =>
            {
                label.Visible = false;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        // HelpMenu
        public static Guna2Panel HelpMenu { get; set; }
        private static void ConstructHelpMenu()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(PanelButtonHeight * scale);
            int scaledSeparatorSpace = (int)(_spaceForSeparator * scale);
            int scaledSpaceForPanel = (int)(SpaceForPanel * scale);

            // Calculate height using scaled values: 8 buttons + 1 separator + panel padding
            int calculatedHeight = 8 * scaledButtonHeight + scaledSeparatorSpace + scaledSpaceForPanel;

            HelpMenu = ConstructPanelForMenu(new Size(PanelWidth, calculatedHeight), "helpMenu_Panel");
            FlowLayoutPanel flowPanel = (FlowLayoutPanel)HelpMenu.Controls[0];

            Guna2Button menuBtn = ConstructBtnForMenu("Settings", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenForm(new Settings_Form());
            };

            menuBtn = ConstructBtnForMenu("What's new", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenLink("https://argorobots.com/whats-new/index.php");
            };

            menuBtn = ConstructBtnForMenu("Documentaion", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenLink("https://argorobots.com/documentation/index.php");
            };

            menuBtn = ConstructBtnForMenu("Show logs", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenForm(new Log_Form());
            };
            ConstructKeyShortcut("Ctrl+L", menuBtn);

            ConstructSeparator(PanelBtnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu("Contact us", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenLink("https://argorobots.com/contact-us/index.php");
            };

            menuBtn = ConstructBtnForMenu("Community", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenLink("https://argorobots.com/community/index.php");
            };

            menuBtn = ConstructBtnForMenu("About", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                Tools.OpenLink("https://argorobots.com/about-us/index.php");
            };

            menuBtn = ConstructBtnForMenu("Clear cache", PanelBtnWidth, flowPanel);
            menuBtn.Click += (_, _) =>
            {
                ArgoCompany.ClearCache();
            };
        }

        // Control dropdown
        public static Guna2Button ControlsDropDown_Button { get; private set; }
        public static Guna2Panel ControlDropDown_Panel { get; private set; }
        private static void ConstructControlsDropDownButton()
        {
            float scale = DpiHelper.GetRelativeDpiScale();

            ControlsDropDown_Button = new Guna2Button
            {
                BackColor = Color.Transparent,
                BorderColor = CustomColors.ControlBorder,
                FillColor = CustomColors.PanelBtn,
                ForeColor = CustomColors.Text,
                BorderRadius = 3,
                BorderThickness = 2,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI Semibold", 10, FontStyle.Bold),
                Image = Resources.DownArrowFullGray,
                ImageAlign = HorizontalAlignment.Right,
                ImageSize = new Size((int)(8 * scale), (int)(8 * scale)),
                Size = new Size((int)(225 * scale), (int)(60 * scale)),
                TabIndex = 13,
                Text = "Controls",
                TextOffset = new Point((int)(-10 * scale), 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            ControlsDropDown_Button.Click += ControlsDropDownButton_Click;
        }
        private static void ControlsDropDownButton_Click(object sender, EventArgs e)
        {
            if (MainMenu_Form.Instance.Controls.Contains(ControlDropDown_Panel))
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
            }
            else
            {
                ControlDropDown_Panel.Location = new Point(
                    ControlsDropDown_Button.Right - ControlDropDown_Panel.Width,
                    MainMenu_Form.Instance.MainTop_Panel.Top + MainMenu_Form.Instance.MainTop_Panel.Height);

                MainMenu_Form.Instance.Controls.Add(ControlDropDown_Panel);
                ControlDropDown_Panel.BringToFront();
                ControlDropDown_Panel.Focus();
            }
        }
        private static void ConstructControlsDropDownMenu()
        {
            float scale = DpiHelper.GetRelativeDpiScale();
            int btnWidth = (int)(290 * scale);
            int btnHeight = (int)(50 * scale);

            // Calculate proper height using scaled values - now 8 buttons + 7 separators
            int calculatedHeight = btnHeight * 8 + (int)(15 * scale) + (int)(_spaceForSeparator * scale) * 7;

            ControlDropDown_Panel = ConstructPanelForMenu(new Size((int)(300 * scale), calculatedHeight), "controlDropDown_Panel");
            FlowLayoutPanel flowPanel = (FlowLayoutPanel)ControlDropDown_Panel.Controls[0];

            Guna2Button menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.Accountants_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Accountants_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.Companies_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Companies_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.Customers_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Customers_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.Categories_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Categories_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.Products_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Products_Form(ProductType.Purchase));
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.ManageRentals_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new Rentals_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.AddSale_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new AddSale_Form());
            };

            ConstructSeparator(btnWidth, flowPanel);

            menuBtn = ConstructBtnForMenu(MainMenu_Form.Instance.AddPurchase_Button.Text, btnWidth, flowPanel);
            menuBtn.Height = btnHeight;
            menuBtn.Click += (_, _) =>
            {
                MainMenu_Form.Instance.Controls.Remove(ControlDropDown_Panel);
                Tools.OpenForm(new AddPurchase_Form());
            };
        }

        // Init hover effect for Guna2ImageButton
        public static void InitHoverEffectForImageButton(Guna2ImageButton control)
        {
            control.MouseEnter += Control_MouseEnter;
            control.MouseLeave += Control_MouseLeave;
        }
        private static void Control_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Guna2ImageButton button)
            {
                button.BackColor = CustomColors.MouseHover;
            }
        }
        private static void Control_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Guna2ImageButton button)
            {
                button.BackColor = CustomColors.MainBackground;
            }
        }

        // Rename
        public static Guna2TextBox Rename_TextBox { get; set; }
        public static void ConstructRightClickRename()
        {
            Rename_TextBox = new Guna2TextBox
            {
                Font = new Font("Segoe UI", 10),
                MaxLength = 30,
                ForeColor = CustomColors.Text,
                FillColor = CustomColors.ControlBack,
                BorderStyle = DashStyle.Solid,
                TextOffset = new Point(-3, 0),
                BorderThickness = 1,
                ShortcutsEnabled = false
            };
            if (ThemeManager.IsDarkTheme())
            {
                Rename_TextBox.BorderColor = Color.White;
                Rename_TextBox.HoverState.BorderColor = Color.White;
                Rename_TextBox.FocusedState.BorderColor = Color.White;
            }
            else
            {
                Rename_TextBox.BorderColor = Color.Black;
                Rename_TextBox.HoverState.BorderColor = Color.Black;
                Rename_TextBox.FocusedState.BorderColor = Color.Black;
            }

            Rename_TextBox.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape)
                {
                    Rename();

                    // Remove Windows "ding" noise when user presses enter
                    e.SuppressKeyPress = true;
                }
            };
            TextBoxManager.Attach(Rename_TextBox);
        }

        // Other methods
        public static void SetRightClickMenuHeight(Guna2Panel panel)
        {
            FlowLayoutPanel flowPanel = panel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
            int controlCount = flowPanel.Controls.Cast<Control>().Count(c => c.Visible);

            float scale = DpiHelper.GetRelativeDpiScale();
            int scaledButtonHeight = (int)(PanelButtonHeight * scale);
            int scaledSpaceForPanel = (int)(SpaceForPanel * scale);

            panel.Height = controlCount * scaledButtonHeight + scaledSpaceForPanel;
            flowPanel.Height = controlCount * scaledButtonHeight;
        }
        private static string FormatControlName(string text, string suffix)
        {
            // Capitalize first letter of each word
            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string titleCaseText = textInfo.ToTitleCase(text);

            // Remove spaces and ellipses
            string cleanText = titleCaseText.Replace(" ", "").Replace("...", "");

            // Add suffix
            return cleanText + suffix;
        }

        // Validity
        public static void SetGTextBoxToValid(Guna2TextBox textBox)
        {
            textBox.BorderColor = CustomColors.ControlBorder;
            textBox.FocusedState.BorderColor = CustomColors.AccentBlue;
        }
        public static void SetGTextBoxToInvalid(Guna2TextBox textBox)
        {
            textBox.BorderColor = Color.Red;
            textBox.FocusedState.BorderColor = Color.Red;
        }
        public static bool IsGTextBoxValid(Guna2TextBox textBox)
        {
            return textBox.BorderColor != Color.Red;
        }

        // Animate buttons
        /// <summary>
        /// Sets the Animated property for a list of buttons if the setting is enabled.
        /// </summary>
        public static void AnimateButtons(IEnumerable<Guna2Button> buttons)
        {
            foreach (Guna2Button button in buttons)
            {
                button.Animated = Properties.Settings.Default.AnimateButtons;
            }
        }

        // Close all panels
        public static void Rename()
        {
            if (Tools.IsFormOpen<MainMenu_Form>())
            {
                MainMenu_Form.Instance.RenameCompany();
            }
            else if (Tools.IsFormOpen<GetStarted_Form>())
            {
                GetStarted_Form.Instance.RenameCompany();
            }
        }
        public static void DeselectAllMenuButtons(Guna2Panel panel)
        {
            foreach (Control control in panel.Controls[0].Controls)
            {
                if (control is Guna2Button button && button.BorderThickness == 1)
                {
                    button.BorderThickness = 0;
                }
            }
        }
    }
}