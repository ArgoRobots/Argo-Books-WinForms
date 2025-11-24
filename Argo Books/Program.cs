using Argo_Books.Startup;

namespace Argo_Books
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            ApplicationStartup.InitializeApplication();

            // Handle .ArgoSales file association - opens file if launched via double-click
            if (ApplicationStartup.TryOpenCompanyFromCommandLine(args))
            {
                Application.Run();
                return;
            }

            // Auto-open the most recent company if this is after an update
            if (ApplicationStartup.TryAutoOpenRecentCompanyAfterUpdate())
            {
                Application.Run();
                return;
            }

            // Otherwise, normal application startup
            Startup_Form startupForm = new(args);
            startupForm.Show();
            Application.Run();
        }
    }
}