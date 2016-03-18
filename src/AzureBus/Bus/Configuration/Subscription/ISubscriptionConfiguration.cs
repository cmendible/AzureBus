namespace AzureBus
{
    public interface ISubscriptionConfiguration : ITopicConfiguration<ISubscriptionConfiguration>
    {
        int MaxConcurrentCalls { get; }
        Microsoft.ServiceBus.Messaging.ReceiveMode ReceiveMode { get; }

        ISubscriptionConfiguration WithReceiveAndDelete();
        ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls);
    }
}
