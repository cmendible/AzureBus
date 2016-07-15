namespace AzureBus
{
    using System;

    public interface IBusConfiguration
    {
        /// <summary>
        /// Gets the Url for the ServiceBus AMQP endpoint.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        string Url { get; }

        /// <summary>
        /// Username for the connection 
        /// </summary>
        /// <returns></returns>
        string Username { get; }

        /// <summary>
        /// Key or password for the connection 
        /// </summary>
        /// <returns></returns>
        string Key { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        IAzureBusLogger Logger { get; }

        /// <summary>
        /// Gets the publish configuration.
        /// </summary>
        /// <value>
        /// The publish configuration.
        /// </value>
        Func<IPublishConfiguration> PublishConfiguration { get; }

        /// <summary>
        /// Gets the subscribtion configuration.
        /// </summary>
        /// <value>
        /// The subscribtion configuration.
        /// </value>
        Func<ISubscriptionConfiguration> SubscribtionConfiguration { get; }

        /// <summary>
        /// Sets the Url for the ServiceBus AMQP endpoint.
        /// </summary>
        /// <param name="url">The Url for the ServiceBus AMQP endpoint.</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        IBusConfiguration WithUrl(string url);

        /// <summary>
        /// Set the Username
        /// </summary>
        /// <param name="username">the username</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        IBusConfiguration WithUsername(string username);

        /// <summary>
        /// Set the Key
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        IBusConfiguration WithKey(string key);

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>The BusConfiguration instance.</returns>
        IBusConfiguration WithLogger(IAzureBusLogger logger);

        /// <summary>
        /// Sets the publish configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>The BusConfiguration instance.</returns>
        IBusConfiguration WithPublishConfiguration(Action<IPublishConfiguration> configure);

        /// <summary>
        /// Sets the subscription configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>The BusConfiguration instance.</returns>
        IBusConfiguration WithSubscriptionConfiguration(Action<ISubscriptionConfiguration> configure);
    }
}
