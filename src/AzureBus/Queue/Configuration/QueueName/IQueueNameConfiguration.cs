namespace AzureBus.Queue
{
    using System;

    public interface IQueueNameConfiguration
    {
        Func<Type, string> GetQueueName { get; }
    }
}
