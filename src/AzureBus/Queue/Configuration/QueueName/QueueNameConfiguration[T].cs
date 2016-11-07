namespace AzureBus.Queue
{
    using System;

    public class QueueNameConfiguration<T> : IQueueNameConfiguration<T> where T : class, IQueueNameConfiguration
    {
        public QueueNameConfiguration()
        {
            this.GetQueueName = (type) => { return type.FullName.ToLowerInvariant(); };
        }

        public Func<Type, string> GetQueueName
        {
            get;
            private set;
        }

        public T WithQueueName(Func<Type, string> configure)
        {
            this.GetQueueName = configure;
            return this as T;
        }
    }
}
