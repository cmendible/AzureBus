namespace AzureBus
{
    using System;
    using System.Collections.Generic;

    public interface IPublishConfiguration : ITopicConfiguration<IPublishConfiguration>
    {
        Func<object, string> GetMessageId { get; }
        Action<object, IDictionary<string, object>> SetMessageMetadata { get; }

        IPublishConfiguration WithMessageId(Func<object, string> configure);
        IPublishConfiguration WithMessageMetadata(Action<object, IDictionary<string, object>> configure);
    }
}
