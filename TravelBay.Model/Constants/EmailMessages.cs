namespace TravelBay.Model.Constants
{
    /// <summary>
    /// RabbitMQ contract for emails sent by the Python worker (TravelBay.Worker/app/messages.py).
    /// A separate queue from the AI agents, so a long agent run never delays an email.
    /// </summary>
    public static class EmailMessages
    {
        public const string QueueName = "travelbay.emails";

        public const string SendPasswordResetCode = "SendPasswordResetCode";
    }
}
