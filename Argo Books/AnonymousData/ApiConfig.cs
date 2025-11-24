using Argo_Books.Classes;
using Argo_Books.Encryption;

namespace Argo_Books.AnonymousData
{
    public static class ApiConfig
    {
        public static string? ApiKey => DotEnv.Get("UPLOAD_API_KEY");
        public static string UserAgent => $"ArgoSalesTracker/{Tools.GetVersionNumber()}";
        public static string ServerUrl => "https://argorobots.com/upload_data.php";
    }
}