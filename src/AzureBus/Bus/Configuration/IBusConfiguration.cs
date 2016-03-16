namespace AzureBus
{
    using System;

    public interface IBusConfiguration
    {
        string ConnectionString { get; }
        ILogger Logger { get; }
        Func<IPublishConfiguration> PublishConfiguration { get; }
        Func<ISubscribtionConfiguration> SubscribtionConfiguration { get; }

        IBusConfiguration WithConnectionString(string connectionString);
        IBusConfiguration WithLogger(ILogger logger);

        IBusConfiguration WithPublishConfiguration(Action<IPublishConfiguration> configure);
        IBusConfiguration WithSubscribtionConfiguration(Action<ISubscribtionConfiguration> configuration);
    }
}
