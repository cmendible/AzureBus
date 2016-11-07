namespace AzureBus
{
    using System;

    public interface IBusConfiguration
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
        Func<ISubscriptionConfiguration> SubscriptionConfiguration { get; }

        /// <summary>
        /// Sets the connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The BusConfiguration instance.</returns>
        IBusConfiguration WithConnectionString(string connectionString);

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
