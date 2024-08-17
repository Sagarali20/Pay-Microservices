namespace PaymentService.Helpers.ExceptionHandler
{
    public class ExceptionErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
