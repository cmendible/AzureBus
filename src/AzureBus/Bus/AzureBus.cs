namespace AzureBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Microsoft ServiceBus Abstraction.
    /// </summary>
    public class AzureBus : IBus
    {
        private readonly NamespaceManager namespaceManager;
        private readonly ConcurrentDictionary<string, IEnumerable<Delegate>> subscriptionActions = new ConcurrentDictionary<string, IEnumerable<Delegate>>();
        private readonly ConcurrentDictionary<string, SubscriptionClient> subscriptionClients = new ConcurrentDictionary<string, SubscriptionClient>();
        private readonly ConcurrentDictionary<string, TopicClient> topicClients = new ConcurrentDictionary<string, TopicClient>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBus" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        internal AzureBus(IAzureBusConfiguration configuration)
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
        public IAzureBusConfiguration Configuration
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
            envelope.Properties["Message.Type.Assembly"] = messageType.Assembly.FullName;
            envelope.Properties["Message.Type.FullName"] = messageType.FullName;
            envelope.Properties["Message.Type.Namespace"] = messageType.Namespace;

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
        /// <param name="subscriptionId">
        /// A unique identifier for the subscription.
        /// </param>
        /// <param name="onMessage">
        /// The action to run when a message arrives.
        /// </param>
        public void Subscribe<T>(string subscriptionId, Action<T> onMessage)
        {
            this.Subscribe<T>(subscriptionId, onMessage, (c) => { });
        }

        /// <summary>
        /// Subscribes to a stream of messages that match a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to subscribe to</typeparam>
        /// <param name="subscriptionId">
        /// A unique identifier for the subscription.
        /// </param>
        /// <param name="onMessage">
        /// The action to run when a message arrives.
        /// </param>
        public void Subscribe<T>(string subscriptionId, Action<T> onMessage, Action<ISubscribtionConfiguration> configure)
        {
            ISubscribtionConfiguration configuration = this.Configuration.SubscribtionConfiguration();
            configure(configuration);

            string topic = this.CreateTopicIfNotExists<T>(configuration);

            string realSubscriptionId = subscriptionId.ToLowerInvariant();

            if (!this.namespaceManager.SubscriptionExists(topic, realSubscriptionId))
            {
                SubscriptionDescription dataCollectionTopic = this.namespaceManager.CreateSubscription(topic, realSubscriptionId);
            }

            string descriptor = topic + ":" + realSubscriptionId;
            subscriptionActions.AddOrUpdate(descriptor, new Delegate[] { onMessage }, (key, oldValue) => oldValue.Concat(new Delegate[] { onMessage }));

            Func<SubscriptionClient> clientSetup = () =>
                {
                    SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(
                        this.Configuration.ConnectionString,
                        topic,
                        realSubscriptionId,
                        configuration.ReceiveMode);

                    OnMessageOptions options = new OnMessageOptions()
                        {
                            AutoComplete = false,
                            MaxConcurrentCalls = configuration.MaxConcurrentCalls
                        };

                    client.OnMessage(envelope =>
                        {
                            this.Configuration.Logger.InfoFormat(
                                "Message was received on Subscription {0} Topic {1} with MessageId {2}",
                                realSubscriptionId,
                                topic,
                                envelope.MessageId);

                            Type messageType = typeof(T);

                            IEnumerable<Delegate> actions = subscriptionActions[descriptor]
                                .Where(a => a.GetType().GetGenericArguments().First().FullName == messageType.FullName);

                            if (actions.Any())
                            {
                                foreach (Delegate action in actions)
                                {
                                    object message = JsonConvert.DeserializeObject(envelope.GetBody<string>(), messageType);

                                    action.DynamicInvoke(message);
                                }
                                envelope.Complete();
                            }
                            else
                            {
                                this.Configuration.Logger.InfoFormat("No action was configured for type {0}", messageType.FullName);
                            }
                        },
                        options);

                    return client;
                };

            this.subscriptionClients.GetOrAdd(descriptor, clientSetup.Invoke());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.subscriptionClients.ToList().ForEach((s) => s.Value.Close());
            this.topicClients.ToList().ForEach((s) => s.Value.Close());
            this.subscriptionActions.Clear();
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