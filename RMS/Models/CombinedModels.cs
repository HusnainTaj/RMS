using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RMS.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(20)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; } = null!;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
    public class Stock
    {
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; } = null!;

        [ForeignKey("SupplierId")]
        public Supplier Supplier { get; set; } = null!;
    }
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string ImageURL { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
    }
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Text { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        public int ItemId { get; set; }

        [ForeignKey("CustomerId")]
        public AppUser Customer { get; set; } = null!;

        [ForeignKey("ItemId")]
        public MenuItem Item { get; set; } = null!;
    }
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Currency)]

        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Required]
        public PaymentMethod Method { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Refunded
    }
    public enum PaymentMethod
    {
        Cash,
        Card
    }
    //public class PaymentMethod
    //{
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(255)]
    //    public string Name { get; set; } = null!;
    //}
    //public class PaymentStatus
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(255)]
    //    public string Name { get; set; } = null!;
    //}
   
    public class Promotion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Code { get; set; } = null!;  

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 100)]
        public decimal DiscountPercentage {  get; set; }

        [Required]
        public DateTime Start { get; set; }

        [Required]
        public DateTime End { get; set; }

        //public ICollection<Order> Orders { get; set; } = new List<Order>();
    }


    public enum OrderType
    {
        Delivery,
        DineIn,
        TakeAway
    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed, 
        Cancelled
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        [Required]
        public OrderType Type { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        public int? PromotionId { get; set; }
        public int? ReservationId { get; set; }

        [ForeignKey("CustomerId")]
        public AppUser Customer { get; set; } = null!;

        [ForeignKey("PromotionId")]
        public Promotion? Promotion { get; set; }

        [ForeignKey("ReservationId")]
        public Reservation? Reservation { get; set; }

        public Payment Payment { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool Reviewed { get; set; } = false;

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; } = null!;

    }
    //public class OrderType
    //{
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(255)]
    //    public string Name { get; set; } = null!;   
    //}
    //public class OrderStatus
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(255)]
    //    public string Name { get; set; } = null!;
    //}
    public class Table
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Capacity { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("TableId")]
        public Table Table { get; set; } = null!;

        [ForeignKey("OrderId")]
        public Order Order { get; set; } = null!;

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Available;
    }

    public enum ReservationStatus
    {
        Reserved,
        Available
    }

    //public class ReservationStatus
    //{
    //    [Key]
    //    public int Id { get; set; }

    //    [Required]
    //    [StringLength(255)]
    //    public string Name { get; set; } = null!;
    //}
    public class EmployeeBonus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(8, 2)")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(255)]
        public string Reason { get; set; } = null!;

        [Required]
        public string EmployeeId { get; set; } = null!;

        [ForeignKey("EmployeeId")]
        public AppUser Employee { get; set; } = null!;
    }
}
