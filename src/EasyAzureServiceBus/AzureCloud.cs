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
    }
}
