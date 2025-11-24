using Argo_Books.Theme;
using System.Reflection;

namespace Argo_Books.Theme
{
    /// <summary>
    /// Allows the theme of all open forms to be updated without having to reinitiate the form 
    /// or manually calling the 'UpdateTheme' method for every form.
    /// </summary>
    public static class FormThemeManager
    {
        private static readonly List<Form> _registeredForms = [];

        public static void RegisterForm(Form form)
        {
            if (!_registeredForms.Contains(form))
            {
                _registeredForms.Add(form);
                form.FormClosed += (s, e) => _registeredForms.Remove(form);
            }
        }

        /// <summary>
        /// Updates the theme of all registered and open forms.
        /// </summary>
        public static void UpdateAllForms()
        {
            foreach (Form form in _registeredForms)
            {
                if (form != null && !form.IsDisposed)
                {
                    MethodInfo updateMethod = form.GetType().GetMethod("UpdateTheme",
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (updateMethod != null)
                    {
                        // Form has UpdateTheme(), so use it
                        updateMethod.Invoke(form, null);
                    }
                    else
                    {
                        // Form doesn't have UpdateTheme(), so apply theme directly
                        ThemeManager.SetThemeForForm(form);
                    }
                }
            }
        }
    }
}