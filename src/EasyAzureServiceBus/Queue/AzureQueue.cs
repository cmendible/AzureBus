namespace EasyAzureServiceBus
{
    using System;
    using System.Collections.Concurrent;
    using System.Configuration;
    using System.Linq;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Microsoft QueueClient Abstraction.
    /// </summary>
    public class AzureQueue : IQueue
    {
        private readonly string connectionString;
        private readonly NamespaceManager namespaceManager;
        private readonly ConcurrentDictionary<string, QueueClient> queueClients = new ConcurrentDictionary<string, QueueClient>();
        private readonly ConcurrentDictionary<string, QueueClient> subscriptions = new ConcurrentDictionary<string, QueueClient>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueue"/> class.
        /// </summary>
        internal AzureQueue()
            : this(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureQueue"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        internal AzureQueue(string connectionString)
        {
            this.connectionString = connectionString;
            this.namespaceManager = NamespaceManager.CreateFromConnectionString(this.connectionString);
        }

        /// <summary>
        /// Sends a message to the Queue.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        public void Send<T>(T message) where T : class
        {
            string queueName = this.CreateQueueIfNotExists<T>();

            if (!this.queueClients.ContainsKey(queueName))
            {
                this.queueClients.GetOrAdd(queueName, QueueClient.CreateFromConnectionString(connectionString, queueName));
            }

            QueueClient queueClient = this.queueClients[queueName];

            queueClient.Send(new BrokeredMessage(message) { MessageId = message.GetHashCode().ToString() });
        }

        /// <summary>
        /// Subscribes to a stream of messages that match a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to subscribe to</typeparam>
        /// <param name="onMessage">
        /// The action to run when a message arrives.
        /// </param>
        public void Subscribe<T>(Action<T> onMessage)
        {
            string queueName = this.CreateQueueIfNotExists<T>();

            QueueClient queueClient = QueueClient.CreateFromConnectionString(connectionString, queueName);

            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = true;
            options.MaxConcurrentCalls = 1;

            queueClient.OnMessage(message =>
                {
                    onMessage.Invoke(message.GetBody<T>());
                },
                options);

            this.subscriptions.GetOrAdd(queueName, queueClient);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.queueClients.ToList().ForEach((s) => s.Value.Close());
            this.subscriptions.ToList().ForEach((s) => s.Value.Close());
        }

        /// <summary>
        /// Creates the topic if not exists.
        /// </summary>
        /// <typeparam name="T">type of the message</typeparam>
        /// <returns>The topic name</returns>
        private string CreateQueueIfNotExists<T>()
        {
            string queueName = string.Format("{0}", typeof(T).FullName.ToLowerInvariant());

            if (!this.namespaceManager.QueueExists(queueName))
            {
                this.namespaceManager.CreateQueue(queueName);
            }

            return queueName;
        }
    }
}
