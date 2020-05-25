namespace MiApp.Common.Models
{
    public class ResponseT<T> where T : class
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}