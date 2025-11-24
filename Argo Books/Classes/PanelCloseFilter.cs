namespace Argo_Books.Classes
{
    /// <summary>
    /// Monitors mouse clicks on a Form and calls a method when clicking outside excluded controls.
    /// </summary>
    public class PanelCloseFilter(Form form, Action closeAction, params Control[] excludedControls) : IMessageFilter
    {
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_RBUTTONDOWN = 0x0204;
        private readonly Form _form = form;
        private readonly Action _closeAction = closeAction;
        private readonly List<Control> _excludedControls = [.. excludedControls];

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN)
            {
                if (_form == null || _form.IsDisposed)
                {
                    Application.RemoveMessageFilter(this);
                    return false;
                }
                else
                {
                    // Don't trigger if a dialog or another form is active
                    Form activeForm = Form.ActiveForm;
                    if (activeForm != null && activeForm != _form)
                    {
                        return false;
                    }

                    // Check if click is inside any excluded control
                    foreach (Control control in _excludedControls)
                    {
                        if (control != null && control.Visible && control.Parent != null && !control.IsDisposed)
                        {
                            // Convert control bounds to screen coordinates
                            Rectangle screenBounds = control.RectangleToScreen(control.ClientRectangle);

                            if (screenBounds.Contains(Control.MousePosition))
                            {
                                return false;
                            }
                        }
                    }

                    _closeAction?.Invoke();
                }
            }
            return false;
        }
    }
}