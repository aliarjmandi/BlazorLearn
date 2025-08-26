// BlazorLearn/Data/Models/Order.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorLearn.Data.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(32)]
        public string OrderNumber { get; set; } = default!;

        [Required]
        public Guid PersonId { get; set; }

        public Guid? CartId { get; set; }
        public Guid? AddressId { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        public byte PaymentStatus { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; private set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Person Person { get; set; } = default!;
        public Cart? Cart { get; set; }
        public Address? Address { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
