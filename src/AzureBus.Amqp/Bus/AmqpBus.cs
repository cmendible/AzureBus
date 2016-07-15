namespace AzureBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Xml;
    using Amqp;
    using Amqp.Framing;
    using Newtonsoft.Json;

    /// <summary>
    /// Microsoft ServiceBus Abstraction.
    /// </summary>
    public class AmqpBus : IBus
    {
        private readonly Session session;
        private readonly ConcurrentDictionary<string, ReceiverLink> subscriptionClients = new ConcurrentDictionary<string, ReceiverLink>();
        private readonly ConcurrentDictionary<string, SenderLink> topicClients = new ConcurrentDictionary<string, SenderLink>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AmqpBus" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal AmqpBus(IBusConfiguration configuration)
        {
            this.Configuration = configuration;

            var user = System.Web.HttpUtility.UrlEncode(this.Configuration.Username);
            var key = System.Web.HttpUtility.UrlEncode(this.Configuration.Key);
            var url = this.Configuration.Url;

            // Create the connection string
            var connnectionString = string.Format("amqps://{0}:{1}@{2}/", user, key, url);

            var connection = new Connection(new Address(connnectionString));

            // Create the session
            this.session = new Session(connection);
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public IBusConfiguration Configuration
        {
            get;
            private set;
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        public void Publish<T>(T message) where T : class
        {
            this.Publish<T>(message, (c) => { });
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        /// <param name="configure">The configure action.</param>
        public void Publish<T>(T message, Action<IPublishConfiguration> configure) where T : class
        {
            IPublishConfiguration configuration = this.Configuration.PublishConfiguration();
            configure(configuration);

            var topic = configuration.GetTopicName(typeof(T));

            if (!this.topicClients.ContainsKey(topic))
            {
                this.topicClients.GetOrAdd(topic, new SenderLink(this.session, Guid.NewGuid().ToString(), topic));
            }

            var topicClient = this.topicClients[topic];

            Type messageType = typeof(T);

            var envelop = new Message(JsonConvert.SerializeObject(message));
            envelop.Properties = new Properties() { MessageId = configuration.GetMessageId(message) };
            envelop.ApplicationProperties = new ApplicationProperties();
            envelop.ApplicationProperties["Message.Type.AssemblyQualifiedName"] = messageType.AssemblyQualifiedName; ;
            envelop.ApplicationProperties["Message.Type.FullName"] = messageType.FullName;

            var metadata = new Dictionary<string, object>();
            configuration.SetMessageMetadata(message, metadata);

            foreach (var keyvalue in metadata)
            {
                envelop.ApplicationProperties[keyvalue.Key] = keyvalue.Value;
            }

            topicClient.Send(envelop);

            this.Configuration.Logger.InfoFormat(
                "Message of type {0} was sent to Topic {1} with MessageId {2}",
                messageType.FullName,
                topic,
                envelop.Properties.MessageId);
        }

        /// <summary>
        /// Subscribes to a stream of messages that match a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to subscribe to</typeparam>
        /// <param name="subscriptionId">The subscription identifier.</param>
        /// <param name="onMessage">The action to run when a message arrives.</param>
        public void Subscribe<T>(string subscriptionId, Action<T> onMessage) where T : class
        {
            this.Subscribe<T>(onMessage, (c) => { c.WithSubscription(subscriptionId); });
        }

        /// <summary>
        /// Subscribes to a stream of messages that match a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to subscribe to</typeparam>
        /// <param name="onMessage">The action to run when a message arrives.</param>
        /// <param name="configure">The configure action.</param>
        public void Subscribe<T>(Action<T> onMessage, Action<ISubscriptionConfiguration> configure) where T : class
        {
            Type messageType = typeof(T);

            ISubscriptionConfiguration configuration = this.Configuration.SubscribtionConfiguration();
            configure(configuration);

            string realSubscriptionId = configuration.Subscription.ToLowerInvariant();
            var topic = configuration.GetTopicName(typeof(T));

            string descriptor = topic + ":" + realSubscriptionId;
            var client = this.subscriptionClients.GetOrAdd(
                descriptor,
                (d) =>
                    {
                        return new ReceiverLink(
                            this.session,
                            realSubscriptionId,
                            string.Format("{0}/Subscriptions/{1}", topic, realSubscriptionId));
                    });

            //// Start listening
            client.Start(
                new Random().Next(),
                (receiver, envelope) =>
                {
                    try
                    {
                        this.Configuration.Logger.InfoFormat(
                                "Message was received on Subscription {0} Topic {1} with MessageId {2}",
                                realSubscriptionId,
                                topic,
                                envelope.Properties.MessageId);

                        string body = string.Empty;
                        var rawBody = envelope.GetBody<object>();
                        if (rawBody is byte[])
                        {
                            using (var reader = XmlDictionaryReader.CreateBinaryReader(
                                new MemoryStream(rawBody as byte[]),
                                null,
                                XmlDictionaryReaderQuotas.Max))
                            {
                                var doc = new XmlDocument();
                                doc.Load(reader);
                                body = doc.InnerText;
                            }
                        }
                        else
                        {
                            body = rawBody.ToString();
                        }

                        object message = JsonConvert.DeserializeObject(body, messageType);
                        onMessage(message as T);

                        client.Accept(envelope);
                    }
                    catch (Exception ex)
                    {
                        this.Configuration.Logger.Fatal(ex);
                        client.Reject(envelope);
                    }
                });
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.subscriptionClients.ToList().ForEach((s) => s.Value.Close());
            this.topicClients.ToList().ForEach((s) => s.Value.Close());
        }
    }
}