namespace EasyAzureServiceBus
{
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;
    using System;
    using System.Linq;
    using System.Collections.Concurrent;
    using System.Configuration;

    /// <summary>
    /// Microsoft ServiceBus Abstraction.
    /// </summary>
    public class AzureBus : IBus
    {
        private readonly string connectionString;
        private readonly NamespaceManager namespaceManager;
        private readonly ConcurrentDictionary<string, SubscriptionClient> subscribeActions = new ConcurrentDictionary<string, SubscriptionClient>();
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

            if (!topicClients.ContainsKey(topicName))
            {
                topicClients.GetOrAdd(topicName, TopicClient.CreateFromConnectionString(this.connectionString, topicName));
            }

            TopicClient topicClient = topicClients[topicName];

            topicClient.Send(new BrokeredMessage(message) { MessageId = message.GetHashCode().ToString() });
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

            if (!namespaceManager.SubscriptionExists(topicName, subscriptionId))
            {
                SubscriptionDescription dataCollectionTopic = namespaceManager.CreateSubscription(topicName, subscriptionId);
            }

            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(
                this.connectionString, 
                topicName, 
                subscriptionId, 
                ReceiveMode.PeekLock);

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = true;
            options.MaxConcurrentCalls = 2;

            client.OnMessage(message =>
                {
                    onMessage.Invoke(message.GetBody<T>());
                }, 
                options);

            this.subscribeActions.GetOrAdd(subscriptionId, client);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            subscribeActions.ToList().ForEach((s) => s.Value.Close());
            topicClients.ToList().ForEach((s) => s.Value.Close());
        }

        /// <summary>
        /// Creates the topic if not exists.
        /// </summary>
        /// <typeparam name="T">type of the message</typeparam>
        /// <returns>The topic name</returns>
        private string CreateTopicIfNotExists<T>()
        {
            string topicName = typeof(T).FullName.ToLowerInvariant();

            if (!namespaceManager.TopicExists(topicName))
            {
                TopicDescription topicDescription = new TopicDescription(topicName)
                    {
                        RequiresDuplicateDetection = Settings.Default.RequiresDuplicateDetection,
                        DuplicateDetectionHistoryTimeWindow = Settings.Default.DuplicateDetectionHistoryTimeWindow
                    };

                namespaceManager.CreateTopic(topicDescription);
            }

            return topicName;
        }
    }
}
