using HCBShop.Areas.Identity.Data;
using HCBShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HCBShop.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillDetails> BillDetails { get; set; }
    public DbSet<StatusDelivery> StatusDeliveries {  get; set; } 


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        foreach(var entityType in builder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if(tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6)); 
            }
        }
    }
}
