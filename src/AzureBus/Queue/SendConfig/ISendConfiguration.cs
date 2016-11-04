namespace AzureBus.Queue.SendConfig
{
    using System;
    using System.Collections.Generic;

    public interface ISendConfiguration
    {
        Func<object, string> GetMessageId { get; }

        ISendConfiguration WithMessageId(Func<object, string> configure);
    }
}
