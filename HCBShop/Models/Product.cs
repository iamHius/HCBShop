using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCBShop.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [StringLength(150)]
        //[Index("Ix_ProductName", Order = 1, IsUnique = true)]
        public string ProductName { get; set; }
        [StringLength(300)]
        public string? ProductDescription { get; set; } = null;
        [Required]
        [Column(TypeName = "decimal(8,3)")]
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; } = 1;
        public string? ProductImage { get; set; } = null;
        public bool Trending {get; set; } = false;
        public bool SecurityPolicy {get; set; } = false;
        public bool DeliveryPolicy {get; set; } = false;
        public bool Sale { get; set; } = false;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
            
        //}
    }
}
