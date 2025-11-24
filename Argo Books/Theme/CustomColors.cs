using Guna.UI2.WinForms.Enums;
using Argo_Books.Theme;

namespace Argo_Books.Theme
{
    /// <summary>
    /// Manages the custom colors, providing different color settings for dark and light themes.
    /// </summary>
    public static class CustomColors
    {
        // Read-only accent colors that don't change with theme
        public static readonly Color AccentBlue = Color.FromArgb(25, 117, 197),
            AccentRed = Color.FromArgb(238, 89, 81),
            PastelBlue = Color.FromArgb(102, 153, 204),
            PastelGreen = Color.FromArgb(153, 204, 102);

        public static Color ControlBack { get; private set; }
        public static Color ControlDisabledBack { get; private set; }
        public static Color ControlBorder { get; private set; }
        public static Color ControlUncheckedBorder { get; private set; }
        public static Color ControlPanelBorder { get; private set; }
        public static Color Text { get; private set; }
        public static Color GrayText { get; private set; }
        public static Color LinkColor { get; private set; }
        public static Color DebugText { get; private set; }
        public static Color MouseHover { get; private set; }
        public static Color PanelBtn { get; private set; }
        public static Color PanelBtnHover { get; private set; }
        public static Color MainBackground { get; private set; }
        public static Color HeaderBackground { get; private set; }
        public static Color ToolbarBackground { get; private set; }
        public static Color ContentPanelBackground { get; private set; }
        public static DataGridViewPresetThemes DataGridViewTheme { get; private set; }
        public static Color AccentGreen { get; private set; }
        public static Color AccentGray { get; private set; }

        // Return-related colors
        public static Color ReturnedItemBackground { get; private set; }
        public static Color ReturnedItemSelection { get; private set; }
        public static Color ReturnedItemText { get; private set; }
        public static Color PartiallyReturnedItemBackground { get; private set; }
        public static Color PartiallyReturnedItemSelection { get; private set; }
        public static Color PartiallyReturnedItemText { get; private set; }

        // Loss-related colors
        public static Color LostItemBackground { get; private set; }
        public static Color LostItemSelection { get; private set; }
        public static Color LostItemText { get; private set; }
        public static Color PartiallyLostItemBackground { get; private set; }
        public static Color PartiallyLostItemSelection { get; private set; }
        public static Color PartiallyLostItemText { get; private set; }

        /// <summary>
        /// Sets all color values based on the current theme. These colors are used for UI controls throughout the application.
        /// </summary>
        public static void SetColors()
        {
            if (ThemeManager.IsDarkTheme())
            {
                SetDarkThemeColors();
            }
            else
            {
                SetLightThemeColors();
            }
        }
        private static void SetDarkThemeColors()
        {
            // Control colors
            ControlBack = Color.FromArgb(62, 62, 66);
            ControlDisabledBack = Color.FromArgb(45, 45, 48);   
            ControlBorder = Color.FromArgb(130, 130, 130);
            ControlUncheckedBorder = Color.FromArgb(125, 137, 149);
            ControlPanelBorder = Color.FromArgb(110, 110, 110);

            // Text colors
            Text = Color.White;
            GrayText = Color.FromArgb(160, 160, 160);
            LinkColor = Color.FromArgb(71, 157, 250);
            DebugText = Color.FromArgb(0, 255, 255);

            // File colors
            MouseHover = Color.FromArgb(77, 77, 77);

            // Panel colors
            PanelBtn = Color.FromArgb(62, 62, 66);
            PanelBtnHover = Color.FromArgb(90, 90, 94);

            // Background colors
            MainBackground = Color.FromArgb(40, 40, 40);
            HeaderBackground = Color.FromArgb(30, 30, 30);
            ToolbarBackground = Color.FromArgb(20, 20, 20);
            ContentPanelBackground = Color.FromArgb(50, 50, 50);

            // DataGridView colors
            DataGridViewTheme = DataGridViewPresetThemes.Dark;

            // Return colors
            ReturnedItemBackground = Color.FromArgb(80, 45, 45);
            ReturnedItemSelection = Color.FromArgb(100, 60, 60);
            ReturnedItemText = Color.FromArgb(255, 180, 180);
            PartiallyReturnedItemBackground = Color.FromArgb(100, 60, 20);
            PartiallyReturnedItemSelection = Color.FromArgb(120, 70, 30);
            PartiallyReturnedItemText = Color.FromArgb(255, 200, 150);

            // Loss colors - darker than return colors for better distinction
            LostItemBackground = Color.FromArgb(70, 30, 30);
            LostItemSelection = Color.FromArgb(90, 40, 40);
            LostItemText = Color.FromArgb(255, 160, 160);
            PartiallyLostItemBackground = Color.FromArgb(80, 50, 20);
            PartiallyLostItemSelection = Color.FromArgb(100, 65, 30);
            PartiallyLostItemText = Color.FromArgb(255, 220, 180);

            // Accent colors
            AccentGreen = Color.FromArgb(168, 233, 203);
            AccentGray = Color.FromArgb(150, 150, 150);
        }
        private static void SetLightThemeColors()
        {
            // Control colors
            ControlBack = Color.FromArgb(220, 220, 220);
            ControlDisabledBack = Color.LightGray;
            ControlBorder = Color.FromArgb(150, 150, 150);
            ControlUncheckedBorder = Color.FromArgb(125, 137, 149);
            ControlPanelBorder = Color.FromArgb(50, 50, 50);

            // Text colors
            Text = Color.Black;
            GrayText = Color.Gray;
            LinkColor = Color.FromArgb(71, 157, 250);
            DebugText = Color.FromArgb(0, 100, 150);

            // File colors
            MouseHover = Color.FromArgb(229, 243, 255);

            // Panel colors
            PanelBtn = Color.FromArgb(246, 246, 246);
            PanelBtnHover = Color.FromArgb(214, 214, 214);

            // Background colors
            MainBackground = Color.FromArgb(240, 240, 240);
            HeaderBackground = Color.FromArgb(250, 250, 250);
            ToolbarBackground = Color.FromArgb(204, 204, 204);
            ContentPanelBackground = Color.FromArgb(250, 250, 250);

            // DataGridView colors
            DataGridViewTheme = DataGridViewPresetThemes.White;

            // Return colors
            ReturnedItemBackground = Color.FromArgb(255, 215, 215);
            ReturnedItemSelection = Color.FromArgb(240, 220, 220);
            ReturnedItemText = Color.FromArgb(140, 70, 70);
            PartiallyReturnedItemBackground = Color.FromArgb(255, 235, 200);
            PartiallyReturnedItemSelection = Color.FromArgb(240, 220, 180);
            PartiallyReturnedItemText = Color.FromArgb(160, 90, 40);

            // Loss colors - darker than return colors for better distinction
            LostItemBackground = Color.FromArgb(139, 69, 69);
            LostItemSelection = Color.FromArgb(160, 82, 82);
            LostItemText = Color.White;
            PartiallyLostItemBackground = Color.FromArgb(205, 133, 63);
            PartiallyLostItemSelection = Color.FromArgb(222, 150, 80);
            PartiallyLostItemText = Color.White;

            // Accent colors
            AccentGreen = Color.FromArgb(60, 160, 120);
            AccentGray = Color.FromArgb(128, 128, 128);
        }
    }
}