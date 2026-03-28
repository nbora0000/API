using InventoryApi.Models;

namespace InventoryApi.Repositories
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<StockItem>> GetAllAsync();
        Task<StockItem?> GetByIdAsync(Guid id);
        Task<StockItem?> GetByProductIdAsync(Guid productId);
        Task AddAsync(StockItem entity);
        void Update(StockItem entity);
        void Delete(StockItem entity);
        Task ReserveAsync(StockItem item, int quantity);
    }
}
