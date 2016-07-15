namespace AzureBus
{
    public class SubscriptionConfiguration : TopicConfiguration<ISubscriptionConfiguration>, ISubscriptionConfiguration
    {
        public string Subscription
        {
            get;
            private set;
        }

        public SubscriptionConfiguration()
        {
            this.Subscription = "default";
        }

        public ISubscriptionConfiguration WithSubscription(string subscription)
        {
            this.Subscription = subscription;
            return this;
        }
    }
}
