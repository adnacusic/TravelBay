namespace TravelBay.Model.Exceptions
{
    /// <summary>Requested resource does not exist. The WebAPI ExceptionFilter maps this to HTTP 404.</summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
