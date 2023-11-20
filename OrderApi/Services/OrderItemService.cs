using OrderApi.Entities;
using OrderApi.Repositories;

namespace OrderApi.Services
{
    public interface IOrderItemService
    {
        OrderItem GetOrderItemByIds(int orderId, int productId);
        void AddOrderItem(OrderItem orderItem);
        void UpdateOrderItem(OrderItem orderItem);
        void DeleteOrderItem(int orderId, int productId);
    }

    // Services/OrderItemService.cs
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderItemService(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }

        public OrderItem GetOrderItemByIds(int orderId, int productId)
        {
            return _orderItemRepository.GetOrderItemByIds(orderId, productId);
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _orderItemRepository.AddOrderItem(orderItem);
        }

        public void UpdateOrderItem(OrderItem orderItem)
        {
            _orderItemRepository.UpdateOrderItem(orderItem);
        }

        public void DeleteOrderItem(int orderId, int productId)
        {
            _orderItemRepository.DeleteOrderItem(orderId, productId);
        }
    }
}
