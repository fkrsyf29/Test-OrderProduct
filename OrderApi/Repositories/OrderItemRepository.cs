using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Entities;
using System;

namespace OrderApi.Repositories
{
    public interface IOrderItemRepository
    {
        OrderItem GetOrderItemByIds(int orderId, int productId);
        void AddOrderItem(OrderItem orderItem);
        void UpdateOrderItem(OrderItem orderItem);
        void DeleteOrderItem(int orderId, int productId);
    }

    // Repositories/OrderItemRepository.cs
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly DataContext _context;

        public OrderItemRepository(DataContext context)
        {
            _context = context;
        }

        public OrderItem GetOrderItemByIds(int orderId, int productId)
        {
            return _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
        }

        public void UpdateOrderItem(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteOrderItem(int orderId, int productId)
        {
            var orderItem = _context.OrderItems.FirstOrDefault(oi => oi.OrderId == orderId && oi.ProductId == productId);

            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                _context.SaveChanges();
            }
        }
    }
}
