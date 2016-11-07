namespace AzureBus.Queue
{
    using System;

    public interface IQueueNameConfiguration<T> : IQueueNameConfiguration
    {
        T WithQueueName(Func<Type, string> configure);
    }
}
