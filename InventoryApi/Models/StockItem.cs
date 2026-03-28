using System.ComponentModel.DataAnnotations;

namespace InventoryApi.Models
{
    public class StockItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityAvailable { get; set; }
    }
}