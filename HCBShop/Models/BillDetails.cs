using System.ComponentModel.DataAnnotations;

namespace HCBShop.Models
{
    public class BillDetails
    {
        [Key]
        public int BillDetailsId { get; set; }
        public int BillId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }  
        public float? Discount { get; set; }

    }
}
