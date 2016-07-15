namespace AzureBus
{
    using System;

    public interface ITopicConfiguration
    {
        Func<Type, string> GetTopicName { get; }
    }
}
