namespace AzureBus.Queue
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Loggers;

    /// <summary>
    /// Helper to create and configure an Queue instance
    /// </summary>
    internal class QueueConfiguration : IQueueConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string ConnectionString
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public IAzureBusLogger Logger
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the send configuration.
        /// </summary>
        /// <value>
        /// The send configuration.
        /// </value>
        public Func<ISendConfiguration> SendConfiguration
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the subscribtion configuration.
        /// </summary>
        /// <value>
        /// The subscribtion configuration.
        /// </value>
        public Func<ISubscriptionConfiguration> SubscriptionConfiguration
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusConfiguration"/> class.
        /// </summary>
        internal QueueConfiguration()
        {
            this.ConnectionString = ConfigurationManager.AppSettings.AllKeys.Contains("Microsoft.ServiceBus.ConnectionString") ? ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"] : string.Empty;
            this.Logger = new EmptyLogger();

            this.SendConfiguration = () => { return new SendConfiguration(); };
            this.SubscriptionConfiguration = () => { return new SubscriptionConfiguration(); };
        }

        /// <summary>
        /// Sets the connection string
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IQueueConfiguration WithConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// The QueueConfiguration instance.
        /// </returns>
        public IQueueConfiguration WithLogger(IAzureBusLogger logger)
        {
            this.Logger = logger;
            return this;
        }

        /// <summary>
        /// Sets the send configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>
        /// The QueueConfiguration instance.
        /// </returns>
        public IQueueConfiguration WithSendConfiguration(Action<ISendConfiguration> configure)
        {
            this.SendConfiguration = () =>
            {
                ISendConfiguration configuration = new SendConfiguration();
                configure(configuration);
                return configuration;
            };

            return this;
        }

        /// <summary>
        /// Sets the subscription configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>
        /// The QueueConfiguration instance.
        /// </returns>
        public IQueueConfiguration WithSubscriptionConfiguration(Action<ISubscriptionConfiguration> configure)
        {
            this.SubscriptionConfiguration = () =>
            {
                ISubscriptionConfiguration configuration = new SubscriptionConfiguration();
                configure(configuration);
                return configuration;
            };

            return this;
        }
    }
}