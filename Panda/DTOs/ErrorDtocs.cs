namespace Panda.DTOs
{
    public class ValidationErrorItem
    {
        public string Field { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ValidationErrorResponse
    {
        public List<ValidationErrorItem> Errors { get; set; } = new();
    }
}
