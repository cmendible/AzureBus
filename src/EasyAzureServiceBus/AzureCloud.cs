namespace EasyAzureServiceBus
{
    /// <summary>
    /// Simplifies Service Bus instance creation.
    /// </summary>
    public static class AzureCloud
    {
        /// <summary>
        /// Creates a Service Bus instance.
        /// </summary>
        /// <returns></returns>
        public static IBus CreateBus()
        {
            return new AzureBus();
        }

        /// <summary>
        /// Creates a Service Bus instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static IBus CreateBus(string connectionString)
        {
            return new AzureBus(connectionString);
        }

        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <returns></returns>
        public static IQueue CreateQueue()
        {
            return new AzureQueue();
        }

        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static IQueue CreateQueue(string connectionString)
        {
            return new AzureQueue(connectionString);
        }
    }
}
