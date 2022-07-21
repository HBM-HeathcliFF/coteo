using coteo.Domain.Entities;
using coteo.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace coteo.Domain.Repositories.EntityFramework
{
    public class EFOrdersRepository : IOrdersRepository
    {
        private readonly AppDbContext _context;

        public EFOrdersRepository(AppDbContext context)
        {
            _context = context;
        }

        public void DeleteOrder(string id)
        {
            _context.Orders.Remove(new Order() { Id = id });
        }

        public Order GetOrderById(string id)
        {
            return _context.Orders.FirstOrDefault(x => x.Id == id);
        }

        public IQueryable<Order> GetOrders()
        {
            return _context.Orders;
        }

        public void SaveOrder(Order entity)
        {
            if (entity.Id == default)
            {
                entity.Id = Guid.NewGuid().ToString();
                _context.Entry(entity).State = EntityState.Added;
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
    }
}