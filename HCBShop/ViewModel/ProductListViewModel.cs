using HCBShop.Models;

namespace HCBShop.ViewModel
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();
        public PagingInfo PagingInfo { get; set; } = new PagingInfo();
    }
}
