using Microsoft.AspNetCore.Mvc;
using OrderApi.Entities;
using OrderApi.Services;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet("{orderId}/{productId}")]
        public ActionResult<OrderItem> GetOrderItemByIds(int orderId, int productId)
        {
            var orderItem = _orderItemService.GetOrderItemByIds(orderId, productId);

            if (orderItem == null)
            {
                return NotFound();
            }

            return Ok(orderItem);
        }

        [HttpPost]
        public ActionResult<OrderItem> AddOrderItem(OrderItem orderItem)
        {
            _orderItemService.AddOrderItem(orderItem);
            return CreatedAtAction(nameof(GetOrderItemByIds), new { orderId = orderItem.OrderId, productId = orderItem.ProductId }, orderItem);
        }

        [HttpPut]
        public IActionResult UpdateOrderItem(OrderItem orderItem)
        {
            _orderItemService.UpdateOrderItem(orderItem);
            return NoContent();
        }

        [HttpDelete("{orderId}/{productId}")]
        public IActionResult DeleteOrderItem(int orderId, int productId)
        {
            _orderItemService.DeleteOrderItem(orderId, productId);
            return NoContent();
        }
    }
}