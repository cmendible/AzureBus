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

        public TimeSpan DuplicateDetectionHistoryTimeWindow
        {
            get;
            private set;
        }

        public bool RequiresDuplicateDetection
        {
            get;
            private set;
        }

        public TopicConfiguration()
        {
            this.RequiresDuplicateDetection = true;
            this.DuplicateDetectionHistoryTimeWindow = new TimeSpan(1, 0, 0, 0);
            this.GetTopicName = (type) => { return type.FullName.ToLowerInvariant(); };
        }

        public T WithDuplicateDetection(bool enable, TimeSpan? timeWindow = null)
        {
            this.RequiresDuplicateDetection = enable;
            this.DuplicateDetectionHistoryTimeWindow = timeWindow.HasValue ? timeWindow.Value : new TimeSpan(1, 0, 0, 0);
            return this as T;
        }

        public T WithTopicName(Func<Type, string> configure)
        {
            this.GetTopicName = configure;
            return this as T;
        }
    }
}
