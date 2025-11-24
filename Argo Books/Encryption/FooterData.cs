namespace Argo_Books.Encryption
{
    /// <summary>
    /// Represents the footer data structure for company files.
    /// </summary>
    public class FooterData
    {
        public List<string> Accountants { get; set; } = [];
        public string Version { get; set; }
        public bool IsEncrypted { get; set; }
        public string Password { get; set; }
    }
}