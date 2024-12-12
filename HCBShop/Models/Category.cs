using System.ComponentModel.DataAnnotations;

namespace HCBShop.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(150)]
        public string CategoryName {  get; set; }
        public int? CategoryOrder { get; set; }
        public string? CategoryImage { get; set; }

    }
}
