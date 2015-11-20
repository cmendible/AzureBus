namespace EasyAzureServiceBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;

    /// <summary>
    /// Microsoft ServiceBus Abstraction.
    /// </summary>
    public class AzureBus : IBus
    {
        private readonly string connectionString;
        private readonly NamespaceManager namespaceManager;
        private readonly ConcurrentDictionary<string, IEnumerable<Delegate>> subscriptionActions = new ConcurrentDictionary<string, IEnumerable<Delegate>>();
        private readonly ConcurrentDictionary<string, SubscriptionClient> subscriptionClients = new ConcurrentDictionary<string, SubscriptionClient>();
        private readonly ConcurrentDictionary<string, TopicClient> topicClients = new ConcurrentDictionary<string, TopicClient>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBus"/> class.
        /// </summary>
        internal AzureBus()
            : this(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBus"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        internal AzureBus(string connectionString)
        {
            this.connectionString = connectionString;
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(this.connectionString);
        }

        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        public void Publish<T>(T message) where T : class
        {
            string topicName = this.CreateTopicIfNotExists<T>();

            if (!this.topicClients.ContainsKey(topicName))
            {
                this.topicClients.GetOrAdd(topicName, TopicClient.CreateFromConnectionString(this.connectionString, topicName));
            }

            TopicClient topicClient = this.topicClients[topicName];

            Type messageType = typeof(T);

            BrokeredMessage envelope = new BrokeredMessage(JsonConvert.SerializeObject(message));
            envelope.Properties["Message.Type.AssemblyQualifiedName"] = messageType.AssemblyQualifiedName;
            envelope.Properties["Message.Type.Assembly"] = messageType.Assembly.FullName;
            envelope.Properties["Message.Type.FullName"] = messageType.FullName;
            envelope.Properties["Message.Type.Namespace"] = messageType.Namespace;

            envelope.MessageId = message.GetHashCode().ToString();

            topicClient.Send(envelope);
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
            string topicName = this.CreateTopicIfNotExists<T>();
            string realSubscriptionId = subscriptionId.ToLowerInvariant();

            if (!this.namespaceManager.SubscriptionExists(topicName, realSubscriptionId))
            {
                SubscriptionDescription dataCollectionTopic = this.namespaceManager.CreateSubscription(topicName, realSubscriptionId);
            }

            string descriptor = topicName + ":" + realSubscriptionId;
            subscriptionActions.AddOrUpdate(descriptor, new Delegate[] { onMessage }, (key, oldValue) => oldValue.Concat(new Delegate[] { onMessage }));

            Func<SubscriptionClient> clientSetup = () =>
                {
                    SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(
                        this.connectionString,
                        topicName,
                        realSubscriptionId,
                        ReceiveMode.PeekLock);

                    OnMessageOptions options = new OnMessageOptions();
                    options.AutoComplete = true;
                    options.MaxConcurrentCalls = 1;

                    client.OnMessage(envelope =>
                        {
                            string messageTypeFullName = envelope.Properties["Message.Type.FullName"].ToString();

                            IEnumerable<Delegate> actions = subscriptionActions[descriptor]
                                .Where(a => a.GetType().GetGenericArguments().First().FullName == messageTypeFullName);

                            if (actions.Any())
                            {
                                Type messageType = actions.First().GetType().GetGenericArguments().First();

                                foreach (Delegate action in actions)
                                {
                                    object message = JsonConvert.DeserializeObject(envelope.GetBody<string>(), messageType);

                                    action.DynamicInvoke(message);
                                }
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
        private string CreateTopicIfNotExists<T>()
        {
            string topicName = !Settings.Default.TopicPerMessage ? typeof(T).Namespace.ToLowerInvariant() : typeof(T).FullName.ToLowerInvariant();

            if (!this.namespaceManager.TopicExists(topicName))
            {
                TopicDescription topicDescription = new TopicDescription(topicName)
                    {
                        RequiresDuplicateDetection = Settings.Default.RequiresDuplicateDetection,
                        DuplicateDetectionHistoryTimeWindow = Settings.Default.DuplicateDetectionHistoryTimeWindow
                    };

                this.namespaceManager.CreateTopic(topicDescription);
            }

            return topicName;
        }
    }
}
