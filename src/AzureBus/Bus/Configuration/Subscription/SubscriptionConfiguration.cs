using System;

namespace AzureBus
{
    public class SubscriptionConfiguration : TopicConfiguration<ISubscriptionConfiguration>, ISubscriptionConfiguration
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

        public string Subscription
        {
            get;
            private set;
        }

        public TimeSpan AutoRenewTimeout
        {
            get;
            private set;
        }

        public SubscriptionConfiguration()
        {
            this.ReceiveMode = Microsoft.ServiceBus.Messaging.ReceiveMode.PeekLock;
            this.MaxConcurrentCalls = 1;
            this.Subscription = "default";
        }

        public ISubscriptionConfiguration WithReceiveAndDelete()
        {
            this.ReceiveMode = Microsoft.ServiceBus.Messaging.ReceiveMode.ReceiveAndDelete;
            return this;
        }

        public ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls)
        {
            this.MaxConcurrentCalls = maxConcurrentCalls;
            return this;
        }

        public ISubscriptionConfiguration WithSubscription(string subscription)
        {
            this.Subscription = subscription;
            return this;
        }

        public ISubscriptionConfiguration WithMWithAutoRenewTimeout(TimeSpan autoRenewTimeout)
        {
            TimeSpan AutoRenewTimeout = autoRenewTimeout;
            return this;
        }
    }
}
