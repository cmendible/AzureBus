namespace AzureBus
{
    using System;

    public static class TopicConfigurationExtensions
    {
        public static T WithTopicByMessage<T>(this ITopicConfiguration<T> configuration) where T : class, ITopicConfiguration
        {
            return configuration.WithTopicName((t) => { return t.FullName.ToLowerInvariant(); });
        }

        public static T WithTopicByMessageNamespace<T>(this ITopicConfiguration<T> configuration) where T : class, ITopicConfiguration
        {
            return configuration.WithTopicName((t) => { return t.Namespace.ToLowerInvariant(); });
        }
    }
}
