using InventoryApi.Data;
using InventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryApi.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDbContext _context;

        public InventoryRepository(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StockItem entity)
        {
            await _context.Stock.AddAsync(entity);
        }

        public void Delete(StockItem entity)
        {
            _context.Stock.Remove(entity);
        }

        public async Task<IEnumerable<StockItem>> GetAllAsync()
        {
            return await _context.Stock.AsNoTracking().ToListAsync();
        }

        public async Task<StockItem?> GetByIdAsync(Guid id)
        {
            return await _context.Stock.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StockItem?> GetByProductIdAsync(Guid productId)
        {
            return await _context.Stock.FirstOrDefaultAsync(s => s.ProductId == productId);
        }

        public void Update(StockItem entity)
        {
            _context.Stock.Update(entity);
        }

        public async Task ReserveAsync(StockItem item, int quantity)
        {
            item.QuantityAvailable -= quantity;
            _context.Stock.Update(item);
            await Task.CompletedTask;
        }
    }
}
