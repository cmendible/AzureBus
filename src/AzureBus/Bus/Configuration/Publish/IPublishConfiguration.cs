namespace AzureBus
{
    using System;

    public interface IPublishConfiguration : ITopicConfiguration<IPublishConfiguration>
    {
        Func<object, string> GetMessageId { get; }

        IPublishConfiguration WithDuplicateDetection(bool enable, TimeSpan? timeWindow = null);
        IPublishConfiguration WithMessageId(Func<object, string> configure);
    }
}
