namespace AzureBus
{
    using System;

    public class TopicConfiguration<T> : ITopicConfiguration<T> where T : class, ITopicConfiguration
    {
        public Func<Type, string> GetTopicName
        {
            get;
            private set;
        }
        public TopicConfiguration()
        {
            this.GetTopicName = (type) => { return type.FullName.ToLowerInvariant(); };
        }

        public T WithDuplicateDetection(bool enable, TimeSpan? timeWindow = null)
        {
            return this as T;
        }

        public T WithTopicName(Func<Type, string> configure)
        {
            this.GetTopicName = configure;
            return this as T;
        }
    }
}
