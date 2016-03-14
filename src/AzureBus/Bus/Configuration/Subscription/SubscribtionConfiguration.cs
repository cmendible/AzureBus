namespace AzureBus
{
    public class SubscribtionConfiguration : TopicConfiguration<ISubscribtionConfiguration>, ISubscribtionConfiguration
    {
        public int MaxConcurrentCalls
        {
            get;
            private set;
        }

        public Microsoft.ServiceBus.Messaging.ReceiveMode ReceiveMode
        {
            get;
            private set;
        }

        public SubscribtionConfiguration()
        {
            this.ReceiveMode = Microsoft.ServiceBus.Messaging.ReceiveMode.PeekLock;
            this.MaxConcurrentCalls = 1;
        }

        public ISubscribtionConfiguration WithReceiveAndDelete()
        {
            this.ReceiveMode = Microsoft.ServiceBus.Messaging.ReceiveMode.ReceiveAndDelete;
            return this;
        }

        public ISubscribtionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls)
        {
            this.MaxConcurrentCalls = maxConcurrentCalls;
            return this;
        }
    }
}
