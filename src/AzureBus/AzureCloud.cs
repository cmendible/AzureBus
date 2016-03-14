namespace AzureBus
{
    using System;

    /// <summary>
    /// Simplifies Service Bus instance creation.
    /// </summary>
    public static class AzureCloud
    {
        /// <summary>
        /// Creates an AzureBusConfiguration object.
        /// </summary>
        /// <returns>An AzureBusConfiguration object.</returns>
        public static IAzureBusConfiguration ConfigureBus()
        {
            return new AzureBusConfiguration();
        }

        /// <summary>
        /// Creates a Service Bus instance.
        /// </summary>
        /// <returns></returns>
        public static IBus CreateBus()
        {
            return new AzureBusConfiguration()
                .CreateBus();
        }

        /// <summary>
        /// Creates a Service Bus instance.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        [Obsolete("Use the new ConfigureBus method")]
        public static IBus CreateBus(string connectionString)
        {
            return new AzureBusConfiguration()
                .WithConnectionString(connectionString)
                .CreateBus();
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
