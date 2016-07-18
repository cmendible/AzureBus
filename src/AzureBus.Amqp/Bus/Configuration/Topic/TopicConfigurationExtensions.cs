namespace AzureBus
{
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

        public static T WithTopicByMessageNamespace<T>(this ITopicConfiguration<T> configuration, string prefix) where T : class, ITopicConfiguration
        {
            return configuration.WithTopicName((t) => { return string.Format("{0}/{1}", prefix, t.Namespace.ToLowerInvariant()); });
        }
    }
}
