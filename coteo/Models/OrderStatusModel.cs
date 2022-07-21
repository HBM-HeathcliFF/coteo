using coteo.Domain.Enum;

namespace coteo.Models
{
    public class OrderStatusModel
    {
        public string Id { get; set; }
        public OrderStatus Status { get; set; }
    }
}