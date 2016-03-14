namespace AzureBus
{
    public interface ISubscribtionConfiguration : ITopicConfiguration<ISubscribtionConfiguration>
    {
        int MaxConcurrentCalls { get; }
        Microsoft.ServiceBus.Messaging.ReceiveMode ReceiveMode { get; }

        ISubscribtionConfiguration WithReceiveAndDelete();
        ISubscribtionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls);
    }
}
