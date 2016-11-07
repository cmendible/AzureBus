namespace AzureBus.Queue
{
    using System;

    public class SubscriptionConfiguration : QueueNameConfiguration<ISubscriptionConfiguration>, ISubscriptionConfiguration
    {
        public int MaxConcurrentCalls
        {
            get;
            private set;
        }

        public bool AutoComplete
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
            this.AutoComplete = false;
            this.MaxConcurrentCalls = 1;
            this.AutoRenewTimeout = new TimeSpan(0, 0, 10);
        }

        public ISubscriptionConfiguration WithAutoComplete()
        {
            this.AutoComplete = true;
            return this;
        }

        public ISubscriptionConfiguration WithMaxConcurrentCalls(int maxConcurrentCalls)
        {
            this.MaxConcurrentCalls = maxConcurrentCalls;
            return this;
        }

        public ISubscriptionConfiguration WithAutoRenewTimeout(TimeSpan ts)
        {
            this.AutoRenewTimeout = ts;
            return this;
        }
    }
}
