namespace AzureBus
{
    using System;

    public interface ITopicConfiguration<T> : ITopicConfiguration
    {
        T WithTopicName(Func<Type, string> configure);
    }
}
