using Data_Layer.Data;
using Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class OrderItem
    {
        public int Id { get; init; }
        public decimal ProductId { get; init; }
        public decimal Price { get; init; }
        public int Count { get; init; }
        public int OrderId { get; init; }
        public int PromoCodeId { get; init; }

        private OrderItem()
        {

        }

        public static async Task<List<OrderItemDTO>> GetByOrderId(int orderId)
        {
            return await OrderItemData.GetByOrderId(orderId);
        }
    }
}
