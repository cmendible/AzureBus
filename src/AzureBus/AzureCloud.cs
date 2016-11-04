namespace AzureBus
{
    using AzureBus.Queue.Configuration;
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
        /// Creates an QueueConfiguration object.
        /// </summary>
        /// <returns>An QueueConfiguration object.</returns>
        public static IQueueConfiguration ConfigureQueue()
        {
            return new QueueConfiguration();
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

        /////// <summary>
        /////// Creates the queue.
        /////// </summary>
        /////// <returns></returns>
        ////public static IQueue CreateQueue()
        ////{
        ////    return new AzureQueue();
        ////}

        /// <summary>
        /// Creates the queue.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static Queue.IQueue CreateQueue(string connectionString)
        {
            return new QueueConfiguration()
                .WithConnectionString(connectionString)
                .CreateQueue();
            //return new AzureQueue(connectionString);
        }
    }
}
