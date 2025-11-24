using Argo_Books.Charts;
using Argo_Books.Classes;
using Argo_Books.GridView;
using Argo_Books.Theme;
using Argo_Books.UI;
using Guna.UI2.WinForms;
using LiveChartsCore.SkiaSharpView.WinForms;
using Microsoft.Win32;
using Argo_Books.Properties;
using Argo_Books.ReportGenerator;
using Argo_Books.ReportGenerator.Menus;
using System.Runtime.InteropServices;

namespace Argo_Books.Theme
{
    /// <summary>
    /// Manages dark and light mode themes, and custom colors.
    /// This class provides methods for setting theme attributes on various UI controls and forms.
    /// </summary>
    internal static partial class ThemeManager
    {
        public enum ThemeType
        {
            Light,
            Dark,
            Windows
        }
        public static ThemeType CurrentTheme
        {
            get
            {
                if (Enum.TryParse(Properties.Settings.Default.ColorTheme, out ThemeType theme))
                {
                    return theme;
                }
                else
                {
                    // Default to Windows if parsing fails
                    return ThemeType.Windows;
                }
            }
            set
            {
                Properties.Settings.Default.ColorTheme = value.ToString();
            }
        }

        // Other methods
        public static bool IsDarkTheme()
        {
            return CurrentTheme == ThemeType.Dark ||
                   (CurrentTheme == ThemeType.Windows && IsWindowsThemeDark());
        }
        private static bool IsWindowsThemeDark()
        {
            int? value = (int?)Registry.GetValue(
                @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                "AppsUseLightTheme", -1);

            return value == 0;  // If value is 0, Windows is using dark theme
        }
        public static void SetThemeForControls(List<Control> list)
        {
            foreach (Control control in list)
            {
                if (control == null) { continue; }

                switch (control)
                {
                    case Form form:
                        form.BackColor = CustomColors.MainBackground;
                        break;

                    case LinkLabel linkLabel:
                        linkLabel.LinkColor = CustomColors.AccentBlue;
                        linkLabel.VisitedLinkColor = CustomColors.AccentBlue;
                        linkLabel.ActiveLinkColor = CustomColors.AccentBlue;
                        linkLabel.ForeColor = CustomColors.Text;
                        linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
                        break;

                    case Label label:
                        label.ForeColor = CustomColors.Text;
                        break;

                    case PictureBox pictureBox:
                        pictureBox.BackColor = Color.Transparent;
                        break;

                    case RichTextBox richTextBox:
                        richTextBox.BackColor = CustomColors.MainBackground;
                        richTextBox.ForeColor = CustomColors.Text;
                        break;

                    case FlowLayoutPanel flowLayoutPanel:
                        flowLayoutPanel.BackColor = CustomColors.MainBackground;
                        CustomizeScrollBar(flowLayoutPanel);
                        break;

                    case ListBox listBox:
                        listBox.BackColor = CustomColors.ControlBack;
                        listBox.ForeColor = CustomColors.Text;
                        break;

                    case Guna2Button guna2Button:
                        // Don't change the theme if the button is blue
                        if (IsButtonBlue(guna2Button))
                        {
                            // Except for the disabled state
                            guna2Button.DisabledState.FillColor = CustomColors.ControlDisabledBack;
                            continue;
                        }

                        guna2Button.FillColor = CustomColors.ControlBack;
                        guna2Button.ForeColor = CustomColors.Text;
                        guna2Button.BorderColor = CustomColors.ControlBorder;
                        guna2Button.DisabledState.FillColor = CustomColors.ControlDisabledBack;
                        guna2Button.HoverState.BorderColor = CustomColors.AccentBlue;
                        guna2Button.GotFocus += Guna2Button_GotFocus;
                        guna2Button.LostFocus += Guna2Button_LostFocus;
                        break;

                    case Guna2TextBox guna2TextBox:
                        guna2TextBox.FillColor = CustomColors.ControlBack;
                        guna2TextBox.HoverState.FillColor = CustomColors.ControlBack;
                        guna2TextBox.DisabledState.FillColor = CustomColors.ControlDisabledBack;
                        guna2TextBox.DisabledState.BorderColor = CustomColors.ControlBorder;
                        guna2TextBox.ForeColor = CustomColors.Text;
                        guna2TextBox.BorderColor = CustomColors.ControlBorder;
                        guna2TextBox.HoverState.BorderColor = CustomColors.AccentBlue;
                        guna2TextBox.FocusedState.BorderColor = CustomColors.AccentBlue;
                        guna2TextBox.FocusedState.FillColor = CustomColors.ControlBack;
                        guna2TextBox.PlaceholderForeColor = CustomColors.ControlBorder;
                        break;

                    case Guna2CustomCheckBox guna2CustomCheckBox:
                        guna2CustomCheckBox.CheckedState.BorderColor = CustomColors.AccentBlue;
                        guna2CustomCheckBox.CheckedState.FillColor = CustomColors.AccentBlue;
                        guna2CustomCheckBox.UncheckedState.BorderColor = CustomColors.ControlUncheckedBorder;
                        guna2CustomCheckBox.UncheckedState.FillColor = CustomColors.ControlBack;
                        break;

                    case Guna2CustomRadioButton guna2CustomRadioButton:
                        guna2CustomRadioButton.CheckedState.BorderColor = CustomColors.AccentBlue;
                        guna2CustomRadioButton.CheckedState.FillColor = CustomColors.AccentBlue;
                        guna2CustomRadioButton.UncheckedState.BorderColor = CustomColors.ControlUncheckedBorder;
                        guna2CustomRadioButton.UncheckedState.FillColor = CustomColors.ControlBack;
                        break;

                    case Guna2Panel guna2Panel:
                        guna2Panel.FillColor = CustomColors.MainBackground;
                        break;

                    case Panel panel:
                        panel.BackColor = CustomColors.MainBackground;
                        break;

                    case Guna2TrackBar guna2TrackBar:
                        guna2TrackBar.ThumbColor = CustomColors.AccentBlue;
                        break;

                    case Guna2NumericUpDown guna2NumericUpDown:
                        guna2NumericUpDown.FillColor = CustomColors.ControlBack;
                        guna2NumericUpDown.ForeColor = CustomColors.Text;
                        guna2NumericUpDown.BorderColor = CustomColors.ControlBorder;
                        guna2NumericUpDown.UpDownButtonFillColor = CustomColors.ControlBack;
                        guna2NumericUpDown.UpDownButtonForeColor = CustomColors.Text;
                        guna2NumericUpDown.UpDownButtonBorderVisible = true;
                        break;

                    case Guna2ComboBox guna2ComboBox:
                        guna2ComboBox.FillColor = CustomColors.ControlBack;
                        guna2ComboBox.ForeColor = CustomColors.Text;
                        guna2ComboBox.BorderColor = CustomColors.ControlBorder;
                        guna2ComboBox.HoverState.BorderColor = CustomColors.AccentBlue;
                        guna2ComboBox.FocusedState.BorderColor = CustomColors.AccentBlue;
                        guna2ComboBox.ItemsAppearance.BackColor = CustomColors.ControlBack;
                        break;

                    case Guna2DataGridView guna2DataGridView:
                        guna2DataGridView.Theme = CustomColors.DataGridViewTheme;
                        guna2DataGridView.BackgroundColor = CustomColors.ControlBack;

                        UpdateDataGridViewHeaderTheme(guna2DataGridView);
                        CustomizeScrollBar(guna2DataGridView);
                        guna2DataGridView.ClearSelection();
                        DataGridViewManager.UpdateRowColors(guna2DataGridView);
                        break;

                    case Guna2CircleButton guna2CircleButton:
                        guna2CircleButton.FillColor = CustomColors.ControlBack;
                        guna2CircleButton.HoverState.FillColor = CustomColors.ControlBack;
                        guna2CircleButton.HoverState.BorderColor = CustomColors.ControlBorder;
                        break;

                    case Guna2ToggleSwitch guna2ToggleSwitch:
                        guna2ToggleSwitch.CheckedState.FillColor = CustomColors.AccentBlue;
                        guna2ToggleSwitch.UncheckedState.FillColor = CustomColors.ControlBack;
                        break;

                    case Guna2DateTimePicker guna2DateTimePicker:
                        guna2DateTimePicker.FillColor = CustomColors.ControlBack;
                        guna2DateTimePicker.ForeColor = CustomColors.Text;
                        guna2DateTimePicker.BorderColor = CustomColors.ControlBorder;
                        guna2DateTimePicker.HoverState.BorderColor = CustomColors.AccentBlue;
                        break;

                    case Guna2GroupBox guna2GroupBox:
                        guna2GroupBox.ForeColor = Color.White;  // Always white since the border is blue
                        guna2GroupBox.FillColor = CustomColors.ControlBack;
                        guna2GroupBox.CustomBorderColor = CustomColors.AccentBlue;
                        break;

                    case Guna2ProgressBar guna2ProgressBar:
                        guna2ProgressBar.FillColor = CustomColors.ControlBack;
                        guna2ProgressBar.ProgressColor = CustomColors.AccentBlue;
                        break;

                    case CartesianChart cartesianChart:
                        ChartColors.ApplyTheme(cartesianChart);
                        break;

                    case PieChart pieChart:
                        ChartColors.ApplyTheme(pieChart);
                        break;

                    case GeoMap geoMap:
                        ChartColors.ApplyTheme(geoMap);
                        break;
                }

                // Recursively apply theme to any child controls
                if (control.HasChildren)
                {
                    List<Control> childControls = [];
                    foreach (Control childControl in control.Controls)
                    {
                        childControls.Add(childControl);
                    }
                    SetThemeForControls(childControls);
                }
            }
        }
        public static void UpdateThemeForPanel(List<Guna2Panel> listOfMenus)
        {
            foreach (Guna2Panel guna2Panel in listOfMenus)
            {
                guna2Panel.FillColor = CustomColors.PanelBtn;
                guna2Panel.BorderColor = CustomColors.ControlPanelBorder;

                Control.ControlCollection list;
                FlowLayoutPanel flowPanel = guna2Panel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                if (flowPanel != null)
                {
                    flowPanel.BackColor = CustomColors.PanelBtn;
                    list = flowPanel.Controls;
                }
                else
                {
                    list = guna2Panel.Controls;
                }

                foreach (Control control in list)
                {
                    if (control is Guna2Separator guna2Separator)
                    {
                        guna2Separator.FillColor = CustomColors.ControlPanelBorder;
                        guna2Separator.BackColor = CustomColors.PanelBtn;
                    }
                    else if (control is Guna2Button guna2Button)
                    {
                        guna2Button.FillColor = CustomColors.PanelBtn;
                        guna2Button.ForeColor = CustomColors.Text;
                        guna2Button.BorderColor = CustomColors.ControlBorder;
                        guna2Button.HoverState.BorderColor = CustomColors.ControlBorder;
                        guna2Button.HoverState.FillColor = CustomColors.PanelBtnHover;
                        guna2Button.PressedColor = CustomColors.PanelBtnHover;

                        foreach (Label label in guna2Button.Controls)
                        {
                            label.ForeColor = CustomColors.Text;
                            label.BackColor = CustomColors.PanelBtn;
                        }
                    }
                }
            }
        }
        public static void UpdateDataGridViewHeaderTheme(Guna2DataGridView dataGridView)
        {
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.Style.BackColor = CustomColors.HeaderBackground;
                column.HeaderCell.Style.SelectionBackColor = CustomColors.HeaderBackground;
            }
        }
        private static void Guna2Button_GotFocus(object sender, EventArgs e)
        {
            Guna2Button button = (Guna2Button)sender;
            button.BorderColor = CustomColors.AccentBlue;
        }
        private static void Guna2Button_LostFocus(object sender, EventArgs e)
        {
            Guna2Button button = (Guna2Button)sender;

            if (button.BorderThickness == 1 && !IsButtonBlue(button))
            {
                button.BorderColor = CustomColors.ControlBorder;
            }
        }
        private static readonly Dictionary<Control, Guna2VScrollBar> _scrollBars = [];
        public static void CustomizeScrollBar(Control control)
        {
            RemoveCustomScrollBar(control);

            // Add new scrollbar
            Guna2VScrollBar vScrollBar = new()
            {
                FillColor = CustomColors.MainBackground,
                ThumbColor = CustomColors.PanelBtnHover,
                BorderColor = CustomColors.ControlPanelBorder,
                ThumbSize = 35,
                HoverState = { ThumbColor = Color.Gray }
            };
            control.Controls.Add(vScrollBar);
            vScrollBar.BringToFront();
            vScrollBar.BindingContainer = control;

            _scrollBars[control] = vScrollBar;
        }
        public static void UpdateScrollBarForControl(ScrollableControl control)
        {
            if (_scrollBars.TryGetValue(control, out Guna2VScrollBar scrollBar))
            {
                scrollBar.Value = control.VerticalScroll.Value;
            }
        }
        public static void RemoveCustomScrollBar(Control control)
        {
            if (_scrollBars.TryGetValue(control, out Guna2VScrollBar? scrollBar))
            {
                scrollBar.Parent?.Controls.Remove(scrollBar);
                scrollBar.Dispose();
                _scrollBars.Remove(control);
            }
        }
        public static void SetThemeForForm(Form form)
        {
            form.InvokeIfRequired(() =>
            {
                FormThemeManager.RegisterForm(form);
                form.BackColor = CustomColors.MainBackground;
                List<Control> list = [];

                foreach (Control item in form.Controls)
                {
                    list.Add(item);
                }

                SetThemeForControls(list);
                UseImmersiveDarkMode(form.Handle, IsDarkTheme());
            });
        }
        public static void SetRightArrowImageBasedOnTheme(Guna2Button button)
        {
            button.Image = IsDarkTheme() ? Resources.RightArrowWhite : Resources.RightArrowBlack;
        }
        public static void UpdateOtherControls()
        {
            if (MainMenu_Form.Instance == null) { return; }

            if (MainMenu_Form.Instance.InvokeRequired)
            {
                MainMenu_Form.Instance.Invoke(new Action(UpdateOtherControls));
                return;
            }

            // Update charts and controls
            MainMenu_Form.Instance.LoadOrRefreshMainCharts();
            MainMenu_Form.Instance.LoadOrRefreshAnalyticsCharts();

            List<Guna2Panel> listOfPanels = MainMenu_Form.GetMenus();

            foreach (Guna2Panel panel in listOfPanels)
            {
                panel.FillColor = CustomColors.PanelBtn;
                panel.BorderColor = CustomColors.ControlPanelBorder;

                if (panel.Controls[0] is FlowLayoutPanel flowLayoutPanel)
                {
                    flowLayoutPanel.BackColor = CustomColors.MainBackground;
                }
            }

            UpdateThemeForPanel(listOfPanels);
            SetRightArrowImageBasedOnTheme(CustomControls.OpenRecentCompany_Button);

            // Update other controls
            SetThemeForControls([CustomControls.ControlsDropDown_Button, MainMenu_Form.DateRangePanel]);

            CustomTooltip.UpdateAllToolTipThemes();
            RightClickDataGridViewRowMenu.Delete_Button.ForeColor = CustomColors.AccentRed;

            // Update the ActionLabel in UndoRedoHistoryDropdown
            UndoRedoHistoryDropdown.ActionLabel.ForeColor = CustomColors.Text;
            UndoRedoHistoryDropdown.ActionLabel.BackColor = CustomColors.MainBackground;

            // Update the Rename_TextBox
            CustomControls.Rename_TextBox.HoverState.BorderColor = CustomColors.Text;
            CustomControls.Rename_TextBox.FocusedState.BorderColor = CustomColors.Text;
            CustomControls.Rename_TextBox.BorderColor = CustomColors.Text;

            // Update the SearchBox
            SearchBox.SearchResultBoxContainer.FillColor = CustomColors.ControlBack;
            SearchBox.SearchResultBox.FillColor = CustomColors.ControlBack;

            List<Guna2Button> searchResultButtons = SearchBox.SearchResultControls.OfType<Guna2Button>().ToList();

            foreach (Guna2Button button in searchResultButtons)
            {
                button.FillColor = CustomColors.ControlBack;
                button.BorderColor = CustomColors.ControlPanelBorder;
                button.ForeColor = CustomColors.Text;
            }

            CustomizeScrollBar(SearchBox.SearchResultBox);
            LoadingPanel.UpdateTheme();

            if (Tools.IsFormOpen<ReportGenerator_Form>())
            {
                ReportDataSelection_Form.Instance.UpdateScrollBarTheme();
                ReportLayoutDesigner_Form.Instance.UpdateTheme();
            }
        }

        // Make button blue
        public static void MakeGButtonBluePrimary(Guna2Button button)
        {
            button.BorderThickness = 0;
            button.FillColor = CustomColors.AccentBlue;
            button.ForeColor = Color.White;
            button.Font = new Font(button.Font, FontStyle.Bold);
        }
        public static void MakeGButtonBlueSecondary(Guna2Button button)
        {
            button.BorderThickness = 2;
            button.FillColor = Color.Transparent;
            button.BorderColor = CustomColors.AccentBlue;
            button.ForeColor = CustomColors.AccentBlue;
            button.Font = new Font(button.Font, FontStyle.Bold);
        }
        public static bool IsButtonBlue(Guna2Button button)
        {
            return button.FillColor == CustomColors.AccentBlue || button.ForeColor == CustomColors.AccentBlue;
        }

        // Set the form header theme
        // https://stackoverflow.com/questions/57124243/winforms-dark-title-bar-on-windows-10

        [LibraryImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
        private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        public static bool UseImmersiveDarkMode(IntPtr handle, bool useDarkMode)
        {
            if (Environment.OSVersion.Version.Major >= 10)
            {
                int attribute = 19;
                if (Environment.OSVersion.Version.Build >= 18985)
                {
                    attribute = 20;
                }

                int useImmersiveDarkMode = useDarkMode ? 1 : 0;
                return DwmSetWindowAttribute(handle, attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }
            return false;
        }
    }
}