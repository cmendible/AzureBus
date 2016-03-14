namespace AzureBus
{
    using System;

    public interface IAzureBusConfiguration
    {
        string ConnectionString { get; }
        ILogger Logger { get; }
        Func<IPublishConfiguration> PublishConfiguration { get; }
        Func<ISubscribtionConfiguration> SubscribtionConfiguration { get; }

        IAzureBusConfiguration WithConnectionString(string connectionString);
        IAzureBusConfiguration WithLogger(ILogger logger);

        IAzureBusConfiguration WithPublishConfiguration(Action<IPublishConfiguration> configure);
        IAzureBusConfiguration WithSubscribtionConfiguration(Action<ISubscribtionConfiguration> configuration);
    }
}
