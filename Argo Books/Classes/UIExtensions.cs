namespace Argo_Books.Classes
{
    // Thread-safe UI extensions
    public static class UIExtensions
    {
        /// <summary>
        /// Executes an action on the UI thread if required, otherwise it executes immediately.
        /// </summary>
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Executes a function on the UI thread if required and returns the result, otherwise it executes immediately.
        /// </summary>
        /// <returns>The result of the function execution.</returns>
        public static T InvokeIfRequired<T>(this Control control, Func<T> func)
        {
            if (control.InvokeRequired)
            {
                return control.Invoke(func);
            }
            else
            {
                return func();
            }
        }

        /// <summary>
        /// Extension method to disable control scrolling and forward to parent panel.
        /// </summary>
        public static void DisableScrollAndForwardToPanel(this Control control)
        {
            // Create message filter for this specific control
            ScrollMessageFilter filter = new(control);
            Application.AddMessageFilter(filter);

            // Remove filter when control is disposed
            control.Disposed += (s, e) => Application.RemoveMessageFilter(filter);
        }
    }
}