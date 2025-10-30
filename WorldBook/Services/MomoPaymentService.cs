using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using WorldBook.Config;

namespace WorldBook.Services
{
    public class MomoPaymentService
    {
        private readonly MomoOptions _options;
        private readonly HttpClient _httpClient;

        public MomoPaymentService(IOptions<MomoOptions> options)
        {
            _options = options.Value;
            _httpClient = new HttpClient();
        }

        public async Task<MomoPaymentResponse?> CreatePaymentAsync(string orderId, string amount, string orderInfo)
        {
            string requestId = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();

            var rawData =
                $"accessKey={_options.AccessKey}&amount={amount}&extraData=&ipnUrl={_options.IpnUrl}&orderId={orderId}" +
                $"&orderInfo={orderInfo}&partnerCode={_options.PartnerCode}&redirectUrl={_options.RedirectUrl}" +
                $"&requestId={requestId}&requestType={_options.RequestType}";

            string signature = MomoSecurity.SignSHA256(rawData, _options.SecretKey);

            var request = new MomoPaymentRequest
            {
                PartnerCode = _options.PartnerCode,
                AccessKey = _options.AccessKey,
                PartnerName = "MoMo Payment",
                StoreId = "WorldBook",
                RequestId = requestId,
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = _options.RedirectUrl,
                IpnUrl = _options.IpnUrl,
                RequestType = _options.RequestType,
                Signature = signature
            };

            var jsonBody = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_options.Endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<MomoPaymentResponse>(responseString);
        }
    }
}
