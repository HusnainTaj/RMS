using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RMS.Models;

namespace RMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        public DbSet<Supplier> Supplier { get; set; } = default!;
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Promotion> Promotions { get; set; } = null!;

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        //public DbSet<EmployeeBonus> EmployeeBonuses { get; set; }


        // ENUMED
        //public DbSet<PaymentMethod> PaymentMethods { get; set; }
        //public DbSet<PaymentStatus> PaymentStatuses { get; set; }

        //public DbSet<OrderStatus> OrderStatuses { get; set; } = null!;
        //public DbSet<OrderType> OrderTypes { get; set; } = null!;

        //public DbSet<ReservationStatus> ReservationStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OrderItem>(e=>
            {
                e.ToTable("OrderItems", tb => tb.HasTrigger("DecrementStockOnOrder"));
            });
        }
    }
}