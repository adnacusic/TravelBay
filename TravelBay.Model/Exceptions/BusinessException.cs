namespace TravelBay.Model.Exceptions
{
    /// <summary>
    /// Request conflicts with the current state of the resource (e.g. an invalid state-machine
    /// transition). The WebAPI ExceptionFilter maps this to HTTP 409.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
}
