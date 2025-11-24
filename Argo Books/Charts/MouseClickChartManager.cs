using Argo_Books.UI;
using Argo_Books;

namespace Argo_Books.Charts
{
    /// <summary>
    /// Manages mouse click detection for LiveCharts controls and invokes specific actions.
    /// </summary>
    public static class MouseClickChartManager
    {
        private static readonly HashSet<Control> _registeredCharts = [];
        private static Action<Control, Point> _onRightClick;
        private static CustomMessageFilter _messageFilter;

        /// <summary>
        /// Initializes the click manager for mixed chart types.
        /// </summary>
        public static void InitCharts(Control[] charts)
        {
            Initialize(charts, RightClickChartMenu.Show);
        }
        private static void Initialize(Control[] charts, Action<Control, Point> onRightClick)
        {
            // Add new charts to the collection
            foreach (Control chart in charts)
            {
                _registeredCharts.Add(chart);
            }

            _onRightClick = onRightClick;

            // Initialize message filter if not already done
            if (_messageFilter == null)
            {
                _messageFilter = new CustomMessageFilter();
                Application.AddMessageFilter(_messageFilter);
            }
        }

        /// <summary>
        /// Custom IMessageFilter implementation that detects mouse clicks and determines
        /// whether a left or right click occurred on a LiveCharts control.
        /// </summary>
        public class CustomMessageFilter : IMessageFilter
        {
            /// <summary>
            /// Filters Windows messages before they are dispatched to controls.
            /// Detects left and right mouse button down events and invokes the appropriate action.
            /// </summary>
            public bool PreFilterMessage(ref Message m)
            {
                // Detect left or right mouse button down events
                if (m.Msg != 0x0201 && m.Msg != 0x0204)
                {
                    return false;
                }

                bool isRightClick = m.Msg == 0x0204;
                Point mousePosition = Control.MousePosition;

                // Check if any other form is over the click position
                if (IsAnotherFormAtPosition(mousePosition))
                {
                    return false;
                }

                // Create a list of controls to ignore
                List<Control> controlsList =
                [
                    CustomControls.FileMenu,
                    CustomControls.RecentlyOpenedMenu,
                    CustomControls.HelpMenu,
                    CustomControls.ControlDropDown_Panel,
                    RightClickChartMenu.Panel,
                    TextBoxManager.RightClickTextBox_Panel
                ];
                Control mainPanel = DateRange_Form.Instance?.Main_Panel;
                if (mainPanel != null && MainMenu_Form.Instance.Controls.Contains(mainPanel))
                {
                    controlsList.Add(mainPanel);
                }

                // Ignore the click if it was over any of the controls
                foreach (Control control in controlsList)
                {
                    if (control.Parent == null)
                    {
                        continue;
                    }

                    // Check if the mouse click was within the bounds of the control
                    Point localMousePosition = control.PointToClient(mousePosition);
                    if (control.ClientRectangle.Contains(localMousePosition))
                    {
                        return false;
                    }
                }

                // Check if the click happened on any of the registered charts
                foreach (Control chart in _registeredCharts)
                {
                    if (!chart.Visible)
                    {
                        // Skip charts that are not visible or not supported chart types
                        continue;
                    }

                    Point localMousePosition = chart.Parent.PointToClient(mousePosition);

                    // Check if the click is within the bounds of the chart
                    if (chart.Bounds.Contains(localMousePosition))
                    {
                        if (isRightClick)
                        {
                            // Trigger the right click action with chart and mouse position
                            _onRightClick?.Invoke(chart, mousePosition);
                        }
                        break;
                    }
                }
                return false;
            }

            /// <summary>
            /// Checks if there is another form at the specified position.
            /// </summary>
            private static bool IsAnotherFormAtPosition(Point screenPosition)
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (form != MainMenu_Form.Instance && form.Visible)
                    {
                        // Check if the form contains the screen position
                        Point clientPoint = form.PointToClient(screenPosition);
                        if (form.ClientRectangle.Contains(clientPoint))
                        {
                            return true;  // Another form is at this position
                        }
                    }
                }

                return false;
            }
        }
    }
}