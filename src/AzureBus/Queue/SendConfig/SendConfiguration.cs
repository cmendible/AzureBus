namespace AzureBus.Queue.SendConfig
{
    using System;
    using System.Collections.Generic;

    public class SendConfiguration : ISendConfiguration
    {
        public Func<object, string> GetMessageId
        {
            get;
            private set;
        }

        public SendConfiguration()
        {
            this.GetMessageId = (message) => { return Guid.NewGuid().ToString(); };
        }

        public ISendConfiguration WithMessageId(Func<object, string> configure)
        {
            this.GetMessageId = configure;
            return this;
        }

      

    }
}
