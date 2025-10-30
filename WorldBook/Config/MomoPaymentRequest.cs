namespace WorldBook.Config
{
    public class MomoPaymentRequest
    {
        public string PartnerCode { get; set; }
        public string AccessKey { get; set; }
        public string PartnerName { get; set; } = "Test Payment";
        public string StoreId { get; set; } = "MomoTestStore";
        public string RequestId { get; set; }
        public string Amount { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public string RedirectUrl { get; set; }
        public string IpnUrl { get; set; }
        public string RequestType { get; set; }
        public string Signature { get; set; }
        public string Lang { get; set; } = "vi";
        public string ExtraData { get; set; } = "";
    }
}
