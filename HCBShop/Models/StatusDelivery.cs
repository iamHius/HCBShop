using System.ComponentModel.DataAnnotations;

namespace HCBShop.Models
{
    public class StatusDelivery
    {
        [Key]
        public int StatusId { get; set; }
        [Required]
        [StringLength(50)]
        public string? StatusName { get; set; }
        [StringLength(300)]
        public string? StatusDescription { get; set; }   

    }
}
