namespace Argo_Books.DataClasses
{
    /// <summary>
    /// Represents a category containing a list of products.
    /// </summary>
    public class Category
    {

        // Getters and setters
        public string Name { get; set; }
        public List<Product> ProductList { get; set; } = [];

        // Default constructor required for deserialization
        public Category() { }

        public Category(string name)
        {
            Name = name;
        }

        public void AddProduct(Product product)
        {
            ProductList.Add(product);
        }
    }
}