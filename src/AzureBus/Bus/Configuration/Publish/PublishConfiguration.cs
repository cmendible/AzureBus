namespace AzureBus
{
    using System;

    public class PublishConfiguration : TopicConfiguration<IPublishConfiguration>, IPublishConfiguration
    {
        public Func<object, string> GetMessageId
        {
            get;
            private set;
        }

        public PublishConfiguration()
        {
            this.GetMessageId = (message) => { return message.GetHashCode().ToString(); };
        }

        public IPublishConfiguration WithMessageId(Func<object, string> configure)
        {
            this.GetMessageId = configure;
            return this;
        }
    }
}
