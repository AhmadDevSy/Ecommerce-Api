using Models;
using Stripe;
using Stripe.Checkout;


namespace Business_Layer.Services
{
    public class StripePaymentService
    {
        public async Task<CheckoutSessionCreateResult> CreateCheckoutSessionAsync(string successUrl, string cancelUrl, decimal amount)
        {

            var options = new SessionCreateOptions
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(amount * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Money Transfer"
                            },
                        },
                        Quantity = 1,
                    },
                }
            };

            var service = new SessionService();
            var result = new CheckoutSessionCreateResult();

            try
            {
                var session = await service.CreateAsync(options);
                result.SessionId = session.Id;
                result.SessionUrl = session.Url;
                result.Success = true;
            }
            catch (Exception)
            {
                result.Success = false;
            }

            return result;
        }

        public async Task<RefundResult> RefundPaymentAsync(string paymentIntentId)
        {
            try
            {
                var options = new RefundCreateOptions
                {
                    PaymentIntent = paymentIntentId,
                };

                var service = new RefundService();
                Refund refund = await service.CreateAsync(options);

                // التحقق من حالة الإرجاع
                if (refund.Status == "succeeded")
                {
                    return new RefundResult
                    {
                        Success = true,
                        RefundId = refund.Id
                    };
                }

                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = $"Refund status: {refund.Status}"
                };
            }
            catch (StripeException e)
            {
                // تسجيل الخطأ (Logger)
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }
    }


    public class RefundResult
    {
        public bool Success { get; set; }
        public string RefundId { get; set; }
        public string ErrorMessage { get; set; }
    }
}