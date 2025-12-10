using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RH7Accounting.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        // Foreign key
        [Required]
        public int CustomerId { get; set; }

        // Navigation property
        public Customer? Customer { get; set; }

        // Invoice items
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SubTotal { get; set; }

        [Range(0, 100)]
        public decimal TaxRate { get; set; }

        public decimal TaxAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Total { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue, Cancelled

        public DateTime? PaidDate { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class InvoiceItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        // Navigation property
        public Invoice? Invoice { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }

        [Range(0, 100)]
        public decimal DiscountPercent { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
