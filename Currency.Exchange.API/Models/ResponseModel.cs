namespace Currency.Exchange.API.Models
{
    public class ResponseModel
    {
        public int StatusCode { get; set; } = 200;
        public string Status { get; set; } = "success";
        public string Error { get; set; } = string.Empty;
        public object Result { get; set; } = null;
    }
}
