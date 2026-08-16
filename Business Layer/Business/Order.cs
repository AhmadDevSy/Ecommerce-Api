
using System.Text;
using System.Text.Json;
using Data_Layer.Data;
using System.Net.Http.Headers;
using Business_Layer.DTO;
using Business_Layer.Services;
using Models.DTO;
using Enums;
using Models.Results;
using Business_Layer.Interfaces;

namespace Business_Layer.Business;

public class Order : IOwnable
{
    public int Id { get; init; }
    public decimal TotalPrice { get; init; }
    public int UserId { get; init; }
    public EnOrderStatus Status { get; protected set; }
    public DateTime CreatedDate { get; init; }
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
        Status = (EnOrderStatus)dto.StatusId;
        CreatedDate = dto.CreatedDate;
        UserId = dto.UserId;
        TotalPrice = dto.TotalPrice;
    }


    public static async Task<CreateOrderOperation> Create(int cartId)
    {
        CreateOrderDatabaseResult op = await OrderData.Create(cartId);
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
        if (!await OrderData.UpdateState(this.Id, (byte)EnOrderStatus.Cancelled))
        {
            return false;
        }

        this.Status = EnOrderStatus.Cancelled;
        return true;
    }

    public async Task<bool> Complete()
    {
        if (!await OrderData.UpdateState(this.Id, (byte)EnOrderStatus.Completed))
        {
            return false;
        }

        this.Status = EnOrderStatus.Completed;
        return true;
    }
}