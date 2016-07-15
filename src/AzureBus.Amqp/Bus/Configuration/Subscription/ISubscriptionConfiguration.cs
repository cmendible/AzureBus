namespace AzureBus
{
    public interface ISubscriptionConfiguration : ITopicConfiguration<ISubscriptionConfiguration>
    {
        string Subscription { get; }

        ISubscriptionConfiguration WithSubscription(string subscription);
    }
}
