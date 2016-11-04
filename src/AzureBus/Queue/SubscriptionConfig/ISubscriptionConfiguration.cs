namespace AzureBus.Queue.SubscriptionConfig
{
    using System;

    public interface ISubscriptionConfiguration
    {
        //bool AutoComplete { get; }
        int MaxConcurrentCalls { get; }
        //TimeSpan AutoRenewTimeout { get; }

        ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls);
        //ISubscriptionConfiguration WithAutoComplete();
        //ISubscriptionConfiguration WithAutoRenewTimeout(TimeSpan ts);
    }
}
