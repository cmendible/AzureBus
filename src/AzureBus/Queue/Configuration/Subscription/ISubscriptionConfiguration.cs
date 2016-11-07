namespace AzureBus.Queue
{
    using System;

    public interface ISubscriptionConfiguration : IQueueNameConfiguration<ISubscriptionConfiguration>
    {
        int MaxConcurrentCalls { get; }

        ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls);
    }
}
