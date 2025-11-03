using BuildingBolcks.Exceptions;

namespace Catalog_API.Exceptions
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid Id): base("Prodotto", Id)
        {

        }
    }
}
