using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Stripe;
using Dapper;
using System.Data;

namespace Stripe_POC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IDbConnection connection; 

        public PaymentController(IConfiguration config, IDbConnection dbConnection)
        {
            _config = config;
            connection = dbConnection;
        }



        //Creating intent for stripe payment
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] decimal amount)
        {


            try
            {

            var orderId = Guid.NewGuid();

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),   //needs to be smallest currency unit (e.g., paise for INR , cents for USD)
                Currency = "inr",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },
                Metadata = new Dictionary<string, string>
        {
            { "orderId", orderId.ToString() }
        }
            };

                var client = new StripeClient(_config["Stripe:SecretKey"]);
                var service = new PaymentIntentService(client);
            var intent = await service.CreateAsync(options);


            await connection.ExecuteAsync(
                "INSERT INTO Orders (OrderId, Amount, Status) VALUES (@OrderId, @Amount, 'Created')",
                new { OrderId = orderId, Amount = amount });

            await connection.ExecuteAsync(
                "INSERT INTO StripePayments (OrderId, PaymentIntentId, Status) VALUES (@OrderId, @PaymentIntentId, @Status)",
                new { OrderId = orderId, PaymentIntentId = intent.Id, Status = intent.Status });

            return Ok(new
            {
                clientSecret = intent.ClientSecret,
                publishableKey = _config["Stripe:PublishableKey"]
            });

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]
                );


                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    var orderId = intent.Metadata["orderId"];

                    await connection.ExecuteAsync(
                        "UPDATE Orders SET Status = 'Paid' WHERE OrderId = @OrderId",
                        new { OrderId = Guid.Parse(orderId) });

                    await connection.ExecuteAsync(
                        "UPDATE StripePayments SET Status = 'Success' WHERE PaymentIntentId = @Id",
                        new { Id = intent.Id });
                }

                if (stripeEvent.Type == "payment_intent.payment_failed")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    var orderId = intent.Metadata["orderId"];

                    await connection.ExecuteAsync(
                        "UPDATE Orders SET Status = 'Failed' WHERE OrderId = @OrderId",
                        new { OrderId = Guid.Parse(orderId) });
                }

                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}