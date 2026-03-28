using InventoryApi.Models;
using InventoryApi.Repositories;
using InventoryApi.Data;
using Microsoft.Extensions.Logging;

namespace InventoryApi.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _repo;
        private readonly InventoryDbContext _db;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IInventoryRepository repo, InventoryDbContext db, ILogger<InventoryService> logger)
        {
            _repo = repo;
            _db = db;
            _logger = logger;
        }

        public async Task<StockItem> CreateAsync(StockItem item)
        {
            await _repo.AddAsync(item);
            await _db.SaveChangesAsync();
            return item;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            _repo.Delete(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<StockItem>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<StockItem?> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<StockItem?> GetByProductIdAsync(Guid productId)
        {
            return await _repo.GetByProductIdAsync(productId);
        }

        public async Task<bool> ReserveAsync(Guid productId, Guid orderId, int quantity)
        {
            var item = await _repo.GetByProductIdAsync(productId);
            if (item == null || item.QuantityAvailable < quantity) return false;

            await _repo.ReserveAsync(item, quantity);
            await _db.SaveChangesAsync();

            // emit event via logger (placeholder for outbox)
            _logger.LogInformation("Inventory reserved for Order {OrderId}: Product {ProductId} x{Quantity}", orderId, productId, quantity);

            return true;
        }

        public async Task<bool> UpdateAsync(Guid id, StockItem item)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;
            existing.ProductId = item.ProductId;
            existing.QuantityAvailable = item.QuantityAvailable;
            _repo.Update(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
