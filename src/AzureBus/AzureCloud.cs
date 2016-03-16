namespace AzureBus
{
    using System;

    /// <summary>
    /// Simplifies Service Bus instance creation.
    /// </summary>
    public static class AzureCloud
    {
        /// <summary>
        /// Creates an BusConfiguration object.
        /// </summary>
        /// <returns>An BusConfiguration object.</returns>
        public static IBusConfiguration ConfigureBus()
        {
            return new BusConfiguration();
        }

        /// <summary>
        /// Creates a Service Bus instance.
        /// </summary>
        /// <returns></returns>
        public static IBus CreateBus()
        {
            return new BusConfiguration()
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
            return new BusConfiguration()
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
