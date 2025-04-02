namespace MuseoLibrary.ApplicationDomain.DTOs
{
    public class ResponseDto <T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
    }
}
