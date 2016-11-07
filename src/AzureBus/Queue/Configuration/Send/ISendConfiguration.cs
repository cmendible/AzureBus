namespace AzureBus.Queue
{
    using System;
    using System.Collections.Generic;

    public interface ISendConfiguration : IQueueNameConfiguration<ISendConfiguration>
    {
        Func<object, string> GetMessageId { get; }

        ISendConfiguration WithMessageId(Func<object, string> configure);
    }
}
