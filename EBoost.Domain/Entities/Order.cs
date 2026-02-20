using EBoost.Domain.Entities;
using EBoost.Domain.Enums;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    //public int Stock { get; set; }

    public string ShippingFullName { get; set; } = null!;
    public string ShippingPhone { get; set; } = null!;
    public string ShippingStreet { get; set; } = null!;
    public string ShippingCity { get; set; } = null!;
    public string ShippingState { get; set; } = null!;
    public string ShippingPostalCode { get; set; } = null!;
    public string ShippingCountry { get; set; } = null!;



    public PaymentMethod PaymentMethod { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? TransactionId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }




    public decimal SubTotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal GrandTotal { get; set; }
    public decimal GrandCost { get; set; }

}
