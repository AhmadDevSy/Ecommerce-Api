using System.Text;
using System.Text.Json;
using Models;
using Data_Layer.Data;
using System.Net.Http.Headers;
using Enums;
using Models.DTO;
using Models.Enums;
using Business_Layer.DTO;
using Stripe.Climate;

namespace Business_Layer.Business;

public class Payment
{
    private EnRecordMode Mode;

    public string Id { get; init; }
    public int OrderId { get; init; }
    public decimal Amount { get; init; }
    public DateTime CreateDate { get; init; }
    public int UserId { get; init; }
    public PaymentStatus Status { get; protected set; }
    public bool IsLocked => this.Status != PaymentStatus.Pending;

    public PaymentDTO DTO => new PaymentDTO()
    {
        Id = this.Id,
        StatusId = (byte)this.Status,
        OrderId = this.OrderId,
        Amount = this.Amount,
        CreateDate = this.CreateDate,
        UserId = this.UserId
    };

    public Payment(Order order, string paymentId)
    {
        Id = paymentId;
        Status = PaymentStatus.Pending;
        CreateDate = DateTime.UtcNow;

        OrderId = order.Id;
        Amount = order.TotalPrice;
        UserId = order.UserId;

        Mode = EnRecordMode.Add;
    }

    private Payment(PaymentDTO dto)
    {
        Id = dto.Id;
        Status = (PaymentStatus)dto.StatusId;
        OrderId = dto.OrderId;
        Amount = dto.Amount;
        CreateDate = dto.CreateDate;
        UserId = dto.UserId;

        Mode = EnRecordMode.Update;
    }

    public static async Task<Payment> GetById(string id)
    {
        PaymentDTO dto = await PaymentData.GetByIdAsync(id);

        if (dto == null)
        {
            return null;
        }

        return new Payment(dto);
    }

    //public static async Task<List<PaymentDTO>> GetByOrderId(int orderId)
    //{
    //    return await PaymentData.GetByOrderId(orderId);
    //}

    public async ValueTask<bool> Save()
    {
        if (IsLocked)
        {
            return false;
        }

        switch (Mode)
        {
            case EnRecordMode.Add:
                {
                    if (await PaymentData.AddAsync(this.DTO))
                    {
                        Mode = EnRecordMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            case EnRecordMode.Update:
                {
                    return await PaymentData.UpdateAsync(this.DTO);
                }
        }

        return false;
    }

    public async ValueTask<bool> Complete()
    {
        if (IsLocked)
        {
            return false;
        }


        if (!await PaymentData.UpdateState(this.Id, (byte)PaymentStatus.Completed))
        {
            return false;
        }

        this.Status = PaymentStatus.Cancelled;
        return true;
    }

    public async ValueTask<bool> Cancel()
    {
        if (IsLocked)
        {
            return false;
        }

        Order order = await Order.GetById(this.OrderId);

        if (!await order.Cancel() || !await PaymentData.UpdateState(this.Id, (byte)PaymentStatus.Cancelled))
        {
            return false;

        }

        this.Status = PaymentStatus.Cancelled;
        return true;
    }



}