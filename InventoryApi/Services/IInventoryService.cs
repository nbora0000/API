using InventoryApi.Models;

namespace InventoryApi.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<StockItem>> GetAllAsync();
        Task<StockItem?> GetByIdAsync(Guid id);
        Task<StockItem?> GetByProductIdAsync(Guid productId);
        Task<StockItem> CreateAsync(StockItem item);
        Task<bool> UpdateAsync(Guid id, StockItem item);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ReserveAsync(Guid productId, Guid orderId, int quantity);
    }
}
