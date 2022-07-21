using coteo.Domain.Entities;

namespace coteo.Domain.Repositories.Abstract
{
    public interface IOrdersRepository
    {
        IQueryable<Order> GetOrders();
        Order? GetOrderById(string id);
        void SaveOrder(Order entity);
        void DeleteOrder(string id);
    }
}