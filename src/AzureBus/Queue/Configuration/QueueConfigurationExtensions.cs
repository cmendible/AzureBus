namespace AzureBus.Queue
{
    public static class QueueConfigurationExtensions
    {
        public static IQueue CreateQueue(this IQueueConfiguration configuration)
        {
            return new AzureQueue(configuration);
        }
    }
}
