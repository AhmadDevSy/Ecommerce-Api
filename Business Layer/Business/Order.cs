
using System.Text;
using System.Text.Json;
using Options;
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
    public OrderState State { get; protected set; }
    public DateTime CreatedDate { get; init; }

    public OrderDTO DTO => new OrderDTO()
    {
        Id = this.Id,
        StateId = (byte)this.State,
        CreatedDate = this.CreatedDate,
        UserId = this.UserId,
        TotalPrice = this.TotalPrice
    };


    private Order(OrderDTO dto)
    {
        Id = dto.Id;
        State = (OrderState)dto.StateId;
        CreatedDate = dto.CreatedDate;
        UserId = dto.UserId;
        TotalPrice = dto.TotalPrice;
    }


    public static async Task<CreateOrderOperation> Create(int userId)
    {
        CreateOrderDatabaseOperation op = await OrderData.Create(userId);
        Order order = null;

        switch (op.Result)
        {
            case EnCreateOrderResult.Success:
                {
                    if (op.OrderDto != null)
                    {
                        order = new Order(op.OrderDto);
                    }
                    else
                    {
                        op.Result = EnCreateOrderResult.UnExpected;
                    }
                }
                break;

            case EnCreateOrderResult.CartNotFound:
                {

                }
                break;

            case EnCreateOrderResult.CartIsEmpty:
                {

                }
                break;

            case EnCreateOrderResult.InvalidPromocode:
                {

                }
                break;

            default:
                {
                    op.Result = EnCreateOrderResult.UnExpected;
                }
                break;
        }

        return new CreateOrderOperation()
        {
            Order = order,
            Result = op.Result
        };


        //if (op.Result != EnCreateOrderResult.Success)
        //{
        //    if (await CartItemBusiness.RemoveExpiredPromocode())
        //    {
        //        operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the promo codes.";
        //    }
        //    else
        //    {
        //        operationResult.ErrorMessage = "Something went Wrong";
        //    }

        //    return operationResult;
        //}




        //bool BookedOrderSuccess = await CreateStoreOrder(order.Id);

        //if (!BookedOrderSuccess)
        //{
        //    if (await CartItemBusiness.SyncCartItemsWithStocks())
        //    {
        //        operationResult.ErrorMessage = "The quantity of products has been modified to match the quantity of the stocks.";
        //    }
        //    else
        //    {
        //        operationResult.ErrorMessage = "Something went Wrong";
        //    }

        //    return operationResult;
        //}

        //return op;
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
        if (this.State != OrderState.New)
        {
            return false;
        }

        return await OrderData.UpdateState(this.Id, (byte)OrderState.Cancelled);
    }

    public async Task<bool> Complete()
    {
        if (this.State != OrderState.New)
        {
            return false;
        }

        return await OrderData.UpdateState(this.Id, (byte)OrderState.Completed);
    }

}