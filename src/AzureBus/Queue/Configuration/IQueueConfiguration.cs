namespace AzureBus.Queue.Configuration
{
    using queueSubscription = Queue.SubscriptionConfig;
    using System;

    public interface IQueueConfiguration
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        string ConnectionString { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        IAzureBusLogger Logger { get; }
    
        /// <summary>
        /// Gets the send configuration.
        /// </summary>
        /// <value>
        /// The send configuration.
        /// </value>
        Func<AzureBus.Queue.SendConfig.ISendConfiguration> SendConfiguration { get; } 

        /// <summary>
        /// Gets the subscribtion configuration.
        /// </summary>
        /// <value>
        /// The subscribtion configuration.
        /// </value>
        Func<queueSubscription.ISubscriptionConfiguration> SubscriptionConfiguration { get; }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The QueueConfiguration instance.</returns>
        IQueueConfiguration WithConnectionString(string connectionString);

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>The QueueConfiguration instance.</returns>
        IQueueConfiguration WithLogger(IAzureBusLogger logger);
    
        /// <summary>
        /// Sets the send configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>
        /// The QueueConfiguration instance.
        /// </returns>
        IQueueConfiguration WithSendConfiguration(Action<AzureBus.Queue.SendConfig.ISendConfiguration> configure);

        /// <summary>
        /// Sets the subscription configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>
        /// The QueueConfiguration instance.
        /// </returns>
        IQueueConfiguration WithSubscriptionConfiguration(Action<queueSubscription.ISubscriptionConfiguration> configure);


    }
}
