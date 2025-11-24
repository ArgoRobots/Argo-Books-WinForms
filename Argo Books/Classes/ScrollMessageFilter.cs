using Guna.UI2.WinForms;

namespace Argo_Books.Classes
{
    /// <summary>
    /// Message filter that intercepts mouse wheel events over a control and forwards them to the parent scrollable panel.
    /// </summary>
    public class ScrollMessageFilter(Control control) : IMessageFilter
    {
        private const int WM_MOUSEWHEEL = 0x020A;
        private readonly Control _control = control;

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_MOUSEWHEEL)
            {
                // Check if mouse is over our control or its children
                Point mousePos = Control.MousePosition;
                if (IsMouseOverControl(_control, mousePos))
                {
                    // Find parent scrollable panel
                    Panel? panel = _control.Parent as Panel;
                    while (panel != null && !panel.AutoScroll)
                    {
                        panel = panel.Parent as Panel;
                    }

                    if (panel != null)
                    {
                        // Extract delta from message
                        int delta = (short)(m.WParam.ToInt64() >> 16 & 0xFFFF);

                        // Forward scroll to panel
                        int newValue = panel.VerticalScroll.Value - delta / 120 * SystemInformation.MouseWheelScrollLines * 20;
                        panel.VerticalScroll.Value = Math.Clamp(newValue,
                            panel.VerticalScroll.Minimum,
                            panel.VerticalScroll.Maximum);
                        panel.PerformLayout();

                        // Find the Guna2VScrollBar in the panel and update it
                        if (panel.Parent != null)
                        {
                            foreach (Control ctrl in panel.Parent.Controls)
                            {
                                if (ctrl is Guna2VScrollBar scrollBar)
                                {
                                    scrollBar.Value = Math.Clamp(newValue,
                                        scrollBar.Minimum,
                                        scrollBar.Maximum - scrollBar.LargeChange + 1);
                                    break;
                                }
                            }
                        }

                        return true;  // Block the message
                    }
                }
            }
            return false;
        }
        private static bool IsMouseOverControl(Control control, Point screenPoint)
        {
            Point clientPoint = control.PointToClient(screenPoint);
            if (control.ClientRectangle.Contains(clientPoint))
            {
                return true;
            }

            // Check all children recursively
            foreach (Control child in control.Controls)
            {
                if (IsMouseOverControl(child, screenPoint))
                {
                    return true;
                }
            }

            return false;
        }
    }
}