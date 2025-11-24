namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Represents a product with details such as ID, name, country of origin, company of origin, and ItemType.
    /// </summary>
    public class Product
    {
        public enum TypeOption
        {
            Product,
            Service
        }

        // Getters and setters
        public string ProductID { get; set; }
        public string Name { get; set; }
        public string CountryOfOrigin { get; set; }
        public string CompanyOfOrigin { get; set; }
        public TypeOption? ItemType { get; set; }

        // Default constructor required for deserialization
        public Product() { }

        public Product(string productID, string productName, string countryOfOrigin, string companyOfOrigin, TypeOption? type)
        {
            ProductID = productID;
            Name = productName;
            CountryOfOrigin = countryOfOrigin;
            CompanyOfOrigin = companyOfOrigin;
            ItemType = type;
        }
    }
}