namespace AzureBus
{
    using System;

    public interface IBusConfiguration
    {
        string ConnectionString { get; }
        IAzureBusLogger Logger { get; }
        Func<IPublishConfiguration> PublishConfiguration { get; }
        Func<ISubscriptionConfiguration> SubscribtionConfiguration { get; }

        IBusConfiguration WithConnectionString(string connectionString);
        IBusConfiguration WithLogger(IAzureBusLogger logger);

        IBusConfiguration WithPublishConfiguration(Action<IPublishConfiguration> configure);
        IBusConfiguration WithSubscriptionConfiguration(Action<ISubscriptionConfiguration> configuration);
    }
}
