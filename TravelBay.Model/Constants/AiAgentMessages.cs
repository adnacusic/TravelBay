namespace TravelBay.Model.Constants
{
    /// <summary>
    /// RabbitMQ contract between the API (publisher) and the Python worker (consumer).
    /// The worker (TravelBay.Worker/app/messages.py) uses the same queue name and commands.
    /// </summary>
    public static class AiAgentMessages
    {
        public const string QueueName = "travelbay.ai-agents";

        public const string GenerateKeywords = "GenerateKeywords";
        public const string FindImages = "FindImages";
    }
}
