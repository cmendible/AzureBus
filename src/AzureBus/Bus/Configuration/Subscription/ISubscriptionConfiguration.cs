using System;

namespace AzureBus
{
    public interface ISubscriptionConfiguration : ITopicConfiguration<ISubscriptionConfiguration>
    {
        string Subscription { get; }
        int MaxConcurrentCalls { get; }
        Microsoft.ServiceBus.Messaging.ReceiveMode ReceiveMode { get; }
        TimeSpan AutoRenewTimeout { get;}

        ISubscriptionConfiguration WithMWithAutoRenewTimeout(TimeSpan autoRenewTimeout);
        ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls);
        ISubscriptionConfiguration WithReceiveAndDelete();
        ISubscriptionConfiguration WithSubscription(string subscription);
    }
}
