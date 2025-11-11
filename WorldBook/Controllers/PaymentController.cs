using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using WorldBook.Services;
using WorldBook.Config;
using WorldBook.Models;
using WorldBook.Repositories.Interfaces;
using CloudinaryDotNet.Actions;

namespace WorldBook.Controllers
{
    [Route("api/v1/user/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IOptions<MomoOptions> _options;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentController(IOptions<MomoOptions> options, IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _options = options;
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet("momo")]
        public async Task<IActionResult> CreatePayment([FromQuery] string orderId, [FromQuery] string amount)
        {
            try
            {
                var momo = _options.Value;
                string requestId = Guid.NewGuid().ToString();
                var momoOrderId = $"{orderId}_PAY_{DateTime.Now:yyyyMMddHHmmss}";
                string orderInfo = momoOrderId;
                string extraData = "";

                // 🔧 FIX: Parse amount đúng cách - không nhân 100
                // amount từ query string là "10000", cần đảm bảo nó là số hợp lệ
                if (!long.TryParse(amount, out long parsedAmount))
                {
                    return BadRequest(new { message = "Invalid amount format" });
                }

                // 🔧 QUAN TRỌNG: amount phải là string nhưng giá trị đúng (không nhân 100)
                string amountStr = parsedAmount.ToString(); // "10000" = "10000" VND

                Console.WriteLine($"📊 MoMo Payment Request: orderId={momoOrderId}, amount={amountStr}");

                // 1️⃣ Raw data phải đúng thứ tự và amount đúng giá trị
                string rawData =
                    $"accessKey={momo.AccessKey}&amount={amountStr}&extraData={extraData}&ipnUrl={momo.IpnUrl}&orderId={momoOrderId}&orderInfo={orderInfo}&partnerCode={momo.PartnerCode}&redirectUrl={momo.RedirectUrl}&requestId={requestId}&requestType={momo.RequestType}";

                // 2️⃣ Tạo chữ ký
                var signature = CreateSignature(rawData, momo.SecretKey);

                // 3️⃣ JSON gửi lên MoMo (đúng format)
                var requestBody = new
                {
                    partnerCode = momo.PartnerCode,
                    partnerName = "WorldBook",
                    storeId = "WorldBook",
                    requestId = requestId,
                    amount = amountStr,  // 🔧 String nhưng giá trị đúng
                    orderId = momoOrderId,
                    orderInfo = orderInfo,
                    redirectUrl = momo.RedirectUrl,
                    ipnUrl = momo.IpnUrl,
                    lang = "vi",
                    extraData = extraData,
                    requestType = momo.RequestType,
                    signature = signature
                };

                Console.WriteLine($"📤 Sending to MoMo: {JsonConvert.SerializeObject(requestBody, Formatting.Indented)}");

                using var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(momo.Endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"📥 MoMo Response: {responseContent}");

                var json = JsonConvert.DeserializeObject<JObject>(responseContent);

                if (json?["payUrl"] != null)
                {
                    // Redirect sang MoMo nếu thành công
                    return Redirect(json["payUrl"].ToString());
                }

                // Nếu lỗi thì show luôn kết quả
                return BadRequest(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ MoMo Payment Error: {ex.Message}");
                return BadRequest(new { message = $"Payment error: {ex.Message}" });
            }
        }


        private string CreateSignature(string rawData, string secretKey)
        {
            var encoding = new UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(secretKey);
            byte[] messageBytes = encoding.GetBytes(rawData);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
                return BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            }
        }
    }

}

