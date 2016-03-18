namespace AzureBus
{
    using System;

    public static class PublishConfigurationExtensions
    {
        public static IPublishConfiguration WithHashCodeAsMessageId(this IPublishConfiguration configuration)
        {
            return configuration.WithMessageId((message) => { return message.GetHashCode().ToString(); });
        }
    }
}
