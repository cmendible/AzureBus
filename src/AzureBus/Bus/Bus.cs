namespace AzureBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Microsoft ServiceBus Abstraction.
    /// </summary>
    public class Bus : IBus
    {
        private readonly NamespaceManager namespaceManager;
        private readonly ConcurrentDictionary<string, SubscriptionClient> subscriptionClients = new ConcurrentDictionary<string, SubscriptionClient>();
        private readonly ConcurrentDictionary<string, TopicClient> topicClients = new ConcurrentDictionary<string, TopicClient>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Bus" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal Bus(IBusConfiguration configuration)
        {
            this.Configuration = configuration;
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(configuration.ConnectionString);
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

            string topic = this.CreateTopicIfNotExists<T>(configuration);

            if (!this.topicClients.ContainsKey(topic))
            {
                this.topicClients.GetOrAdd(topic, TopicClient.CreateFromConnectionString(this.Configuration.ConnectionString, topic));
            }

            TopicClient topicClient = this.topicClients[topic];

            Type messageType = typeof(T);

            BrokeredMessage envelope = new BrokeredMessage(JsonConvert.SerializeObject(message));
            envelope.Properties["Message.Type.AssemblyQualifiedName"] = messageType.AssemblyQualifiedName;
            envelope.Properties["Message.Type.FullName"] = messageType.FullName;

            configuration.SetMessageMetadata(messageType, envelope.Properties);
            
            envelope.MessageId = configuration.GetMessageId(message);

            topicClient.Send(envelope);

            this.Configuration.Logger.InfoFormat(
                "Message of type {0} was sent to Topic {1} with MessageId {2}",
                messageType.FullName, 
                topic,
                envelope.MessageId);
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

            string topic = this.CreateTopicIfNotExists<T>(configuration);

            string realSubscriptionId = configuration.Subscription.ToLowerInvariant();

            if (!this.namespaceManager.SubscriptionExists(topic, realSubscriptionId))
            {
                SqlFilter filter = new SqlFilter(string.Format("[Message.Type.FullName] = '{0}'", messageType.FullName));
                SubscriptionDescription dataCollectionTopic = this.namespaceManager.CreateSubscription(topic, realSubscriptionId, filter);
            }

            string descriptor = topic + ":" + realSubscriptionId;
            SubscriptionClient client = this.subscriptionClients.GetOrAdd(
                descriptor, 
                (d) => 
                    {
                        return SubscriptionClient.CreateFromConnectionString(
                            this.Configuration.ConnectionString,
                            topic,
                            realSubscriptionId,
                            configuration.ReceiveMode);
                    });

            OnMessageOptions options = new OnMessageOptions()
                {
                    AutoComplete = false,
                    MaxConcurrentCalls = configuration.MaxConcurrentCalls
                };

            
            client.OnMessage(envelope =>
                {
                    try
                    {
                        this.Configuration.Logger.InfoFormat(
                            "Message was received on Subscription {0} Topic {1} with MessageId {2}",
                            realSubscriptionId,
                            topic,
                            envelope.MessageId);

                        object message = JsonConvert.DeserializeObject(envelope.GetBody<string>(), messageType);
                        onMessage(message as T);
                        envelope.Complete();
                    }
                    catch (Exception ex)
                    {
                        this.Configuration.Logger.Fatal(ex);
                        envelope.Abandon();
                    }
                },
                options);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.subscriptionClients.ToList().ForEach((s) => s.Value.Close());
            this.topicClients.ToList().ForEach((s) => s.Value.Close());
        }

        /// <summary>
        /// Creates the topic if not exists.
        /// </summary>
        /// <typeparam name="T">type of the message</typeparam>
        /// <returns>The topic name</returns>
        private string CreateTopicIfNotExists<T>(ITopicConfiguration configuration)
        {
            string topicName = configuration.GetTopicName(typeof(T));

            if (!this.namespaceManager.TopicExists(topicName))
            {
                TopicDescription topicDescription = new TopicDescription(topicName)
                    {
                        RequiresDuplicateDetection = configuration.RequiresDuplicateDetection,
                        DuplicateDetectionHistoryTimeWindow = configuration.DuplicateDetectionHistoryTimeWindow
                    };

                this.namespaceManager.CreateTopic(topicDescription);
                this.Configuration.Logger.InfoFormat("Topic with name {0} was created.", topicName);
            }

            return topicName;
        }
    }
}