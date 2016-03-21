﻿namespace AzureBus
{
    using System;
    using System.Collections.Generic;

    public class PublishConfiguration : TopicConfiguration<IPublishConfiguration>, IPublishConfiguration
    {
        public Func<object, string> GetMessageId
        {
            get;
            private set;
        }

        public Action<Type, IDictionary<string, object>> SetMessageMetadata
        {
            get;
            private set;
        }

        public PublishConfiguration()
        {
            this.GetMessageId = (message) => { return Guid.NewGuid().ToString(); };

            this.SetMessageMetadata = (type, metadata) => { };
        }

        public IPublishConfiguration WithMessageId(Func<object, string> configure)
        {
            this.GetMessageId = configure;
            return this;
        }

        public IPublishConfiguration WithMessageMetadata(Action<Type, IDictionary<string, object>> configure)
        {
            this.SetMessageMetadata = configure;
            return this;
        }
    }
}
