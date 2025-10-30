namespace WorldBook.Config
{
    public class MomoReturnResponse
    {
        public string orderId { get; set; }
        public string requestId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string? transId { get; set; }
        public string amount { get; set; }
        public string orderInfo { get; set; }
        public string signature { get; set; }
    }

    public class MomoIpnResponse
    {
        public string orderId { get; set; }
        public string requestId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string? transId { get; set; }
        public string amount { get; set; }
        public string orderInfo { get; set; }
        public string signature { get; set; }
    }
}
