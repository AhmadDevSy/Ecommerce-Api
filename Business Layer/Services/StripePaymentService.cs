using Stripe;
using Stripe.Checkout;


namespace Business_Layer.Services
{
    public class StripePaymentService
    {
        public async Task<string> CreateCheckoutSessionAsync(string successUrl, string cancelUrl, decimal amount)
        {

            var options = new SessionCreateOptions
            {
                //PaymentMethodTypes = new List<string> { "card" },
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
            var session = await service.CreateAsync(options);
            return session.Url;
        }
    }
}
