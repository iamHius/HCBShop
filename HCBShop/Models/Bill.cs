using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HCBShop.Areas.Identity.Data;

namespace HCBShop.Models
{
    public class Bill
    {
        [Key]
        public int BillId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string? UserId { get; set; }
        [ForeignKey("StatusDelivery")]
        public int? StatusId {  get; set; }
        public DateTime? DateBooking { get; set; }
        public DateTime? DateShip { get; set; }
        [StringLength(24)]
        [Required]
        public string? UserPhone { get; set; }
        [StringLength(30)]
        [EmailAddress]
        public string? UserEmail { get; set; } 
        [Required]
        [StringLength(300)]
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? Note { get; set; }


        public ApplicationUser? User { get; set; }
        public StatusDelivery? StatusDelivery { get; set; }

    }
}
