using Guna.UI2.WinForms;

namespace Argo_Books.UI
{
    /// <summary>
    /// Manages keyboard shortcuts and navigation for menu controls.
    /// </summary>
    /// <remarks>
    /// Supports the following keyboard actions:
    /// - Down/Tab: Moves selection to next button
    /// - Up: Moves selection to previous button
    /// - Right: Opens cascading menu if available
    /// - Enter: Triggers the selected button's click event
    /// 
    /// - When reaching the end of the menu, selection wraps around to the beginning and vice versa.
    /// </remarks>
    public static class MenuKeyShortcutManager
    {
        // Properties
        private static Guna2Button buttonThatOpenedCascadingMenu;

        // Getter and setter
        public static Guna2Panel SelectedPanel { get; set; }

        // Main methods
        /// <summary>
        /// Manages navigation between menu buttons using arrow keys, tab, and enter.
        /// </summary>
        public static void HandlePanelKeyDown(Guna2Panel panel, Keys e)
        {
            Guna2Panel activePanel = SelectedPanel ?? panel;

            // Get the FlowLayoutPanel (which is always controls[0] if it exists)
            FlowLayoutPanel? flowPanel = activePanel.Controls.Count > 0 ? activePanel.Controls[0] as FlowLayoutPanel : null;
            if (flowPanel == null) { return; }

            // Get buttons from the FlowLayoutPanel
            IEnumerable<Guna2Button> buttons = flowPanel.Controls.OfType<Guna2Button>();
            if (!buttons.Any()) { return; }

            (Guna2Button? selectedButton, int selectedIndex) = GetSelectedButtonInMenu(buttons);

            if (selectedButton == null)
            {
                SelectFirstButton(buttons);
                return;
            }

            switch (e)
            {
                case Keys.Up:
                    HandleKeyUp(buttons, selectedButton, selectedIndex);
                    break;
                case Keys.Down:
                case Keys.Tab:
                    HandleKeyDown(buttons, selectedButton, selectedIndex);
                    break;
                case Keys.Left:
                    HandleKeyLeft(selectedButton, panel);
                    break;
                case Keys.Right:
                    HandleKeyRight(selectedButton);
                    break;
                case Keys.Enter:
                    selectedButton.PerformClick();
                    break;
            }
        }
        private static void HandleKeyUp(IEnumerable<Guna2Button> buttons, Guna2Button button, int index)
        {
            UnselectMenuButton(button);

            Guna2Button? targetButton = index > 0
                ? buttons.ElementAtOrDefault(index - 1)
                : buttons.LastOrDefault();

            if (targetButton != null)
            {
                SelectMenuButton(targetButton);
            }
        }
        private static void HandleKeyDown(IEnumerable<Guna2Button> buttons, Guna2Button button, int index)
        {
            UnselectMenuButton(button);

            Guna2Button? targetButton = index < buttons.Count() - 1
                ? buttons.ElementAtOrDefault(index + 1)
                : buttons.FirstOrDefault();

            if (targetButton != null)
            {
                SelectMenuButton(targetButton);
            }
        }
        private static void HandleKeyLeft(Guna2Button button, Guna2Panel panel)
        {
            if (SelectedPanel != null)
            {
                // Get buttons from the FlowLayoutPanel in the panel
                FlowLayoutPanel? flowPanel = panel.Controls.Count > 0 ? panel.Controls[0] as FlowLayoutPanel : null;
                if (flowPanel == null) { return; }

                IEnumerable<Guna2Button> buttons = flowPanel.Controls.OfType<Guna2Button>();
                if (!buttons.Any()) { return; }

                SelectedPanel = null;
                UnselectMenuButton(button);
                SelectMenuButton(buttonThatOpenedCascadingMenu);
                buttonThatOpenedCascadingMenu = null;
                CascadingMenu.RemoveCascadingMenus();
            }
        }
        private static void HandleKeyRight(Guna2Button button)
        {
            if (button.Tag is Guna2Panel panel)
            {
                // Get buttons from the FlowLayoutPanel in the panel
                FlowLayoutPanel? flowPanel = panel.Controls.Count > 0 ? panel.Controls[0] as FlowLayoutPanel : null;
                if (flowPanel == null) { return; }

                IEnumerable<Guna2Button> buttons = flowPanel.Controls.OfType<Guna2Button>();
                if (!buttons.Any()) { return; }

                buttonThatOpenedCascadingMenu = button;
                SelectedPanel = panel;

                UnselectMenuButton(button);
                SelectFirstButton(buttons);
            }
        }

        // Helper methods
        private static (Guna2Button?, int) GetSelectedButtonInMenu(IEnumerable<Guna2Button> buttons)
        {
            foreach ((Guna2Button button, int index) in buttons.Select((btn, idx) => (btn, idx)))
            {
                if (IsMenuButtonSelected(button))
                {
                    return (button, index);
                }
            }
            return (null, -1);
        }
        private static void SelectMenuButton(Guna2Button button)
        {
            button.BorderThickness = 1;

            if (button.Tag is Guna2Panel)
            {
                button.PerformClick();  // Open the cascading menu
            }
        }
        private static void SelectFirstButton(IEnumerable<Guna2Button> buttons)
        {
            Guna2Button? firstButton = buttons.FirstOrDefault();
            if (firstButton != null)
            {
                SelectMenuButton(firstButton);
            }
        }
        private static void UnselectMenuButton(Guna2Button button)
        {
            button.BorderThickness = 0;

            if (!CascadingMenu.IsThisACascadingMenu(SelectedPanel))
            {
                CascadingMenu.RemoveCascadingMenus();
            }
        }
        private static bool IsMenuButtonSelected(Guna2Button button)
        {
            return button.BorderThickness == 1;
        }
    }
}