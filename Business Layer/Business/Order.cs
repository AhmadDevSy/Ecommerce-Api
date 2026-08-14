
using System.Text;
using System.Text.Json;
using Models;
using Data_Layer.Data;
using System.Net.Http.Headers;
using Enums;
using Models.DTO;
using Models.Enums;
using Business_Layer.DTO;

namespace Business_Layer.Business;

public class Order
{
    public int Id { get; init; }
    public decimal TotalPrice { get; init; }
    public int UserId { get; init; }
    public OrderStatus Status { get; protected set; }
    public DateTime CreatedDate { get; init; }
    public bool IsLocked => this.Status != OrderStatus.Pending;
    public OrderDTO DTO => new OrderDTO()
    {
        Id = this.Id,
        StatusId = (byte)this.Status,
        CreatedDate = this.CreatedDate,
        UserId = this.UserId,
        TotalPrice = this.TotalPrice
    };


    private Order(OrderDTO dto)
    {
        Id = dto.Id;
        Status = (OrderStatus)dto.StatusId;
        CreatedDate = dto.CreatedDate;
        UserId = dto.UserId;
        TotalPrice = dto.TotalPrice;
    }


    public static async Task<CreateOrderOperation> Create(int cartId)
    {
        CreateOrderDatabaseOperation op = await OrderData.Create(cartId);
        Order order = null;

        if (op.OrderDto != null)
        {
            order = new Order(op.OrderDto);
        }

        return new CreateOrderOperation()
        {
            Order = order,
            Result = op.Result
        };
    }

    public static async Task<Order> GetById(int orderId)
    {
        OrderDTO dto = await OrderData.GetById(orderId);

        if (dto == null)
        {
            return null;
        }
        else
        {
            return new Order(dto);
        }
    }

    public static async Task<List<OrderDTO>> GetByUserId(int userId)
    {
        return await OrderData.GetByUserId(userId);
    }

    public async Task<bool> Cancel()
    {
        if (this.Status != OrderStatus.Pending)
        {
            return false;
        }

        if (!await OrderData.UpdateState(this.Id, (byte)OrderStatus.Cancelled))
        {
            return false;
        }

        this.Status = OrderStatus.Cancelled;
        return true;
    }

    public async Task<bool> Complete()
    {
        if (this.Status != OrderStatus.Pending)
        {
            return false;
        }

        if (!await OrderData.UpdateState(this.Id, (byte)OrderStatus.Completed))
        {
            return false;
        }

        this.Status = OrderStatus.Completed;
        return true;
    }
}