using SharedLibrary.Models;

namespace BasketApi.Models;

public class ShoppingCart : AuditableEntity
{
    public string CustomerId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
}
