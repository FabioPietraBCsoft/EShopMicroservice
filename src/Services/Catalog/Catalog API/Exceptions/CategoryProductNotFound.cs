namespace Catalog_API.Exceptions
{
    public class CategoryProductNotFound : Exception
    {
        public CategoryProductNotFound() : base ("Categoria non trovata!")
        {

        }
    }
}
