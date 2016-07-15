namespace AzureBus
{
    using System;
    using System.Linq;
    using Loggers;

    /// <summary>
    /// Helper to create and configure an Bus instance
    /// </summary>
    internal class BusConfiguration : IBusConfiguration
    {
        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string Url
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
        /// Gets the publish configuration.
        /// </summary>
        /// <value>
        /// The publish configuration.
        /// </value>
        public Func<IPublishConfiguration> PublishConfiguration
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
        public Func<ISubscriptionConfiguration> SubscribtionConfiguration
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }

        public string Key
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusConfiguration"/> class.
        /// </summary>
        internal BusConfiguration()
        {
            this.Logger = new EmptyLogger();

            this.PublishConfiguration = () => { return new PublishConfiguration(); };
            this.SubscribtionConfiguration = () => { return new SubscriptionConfiguration(); };
        }

        /// <summary>
        /// Sets the Url for the ServiceBus AMQP endpoint.
        /// </summary>
        /// <param name="url">The Url for the ServiceBus AMQP endpoint.</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithUrl(string url)
        {
            this.Url = url;
            return this;
        }

        /// <summary>
        /// Set the Username
        /// </summary>
        /// <param name="username">the username</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithUsername(string username)
        {
            this.Username = username;
            return this;
        }

        /// <summary>
        /// Set the Key
        /// </summary>
        /// <param name="key">the key</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithKey(string key)
        {
            this.Key = key;
            return this;
        }

        /// <summary>
        /// Sets the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithLogger(IAzureBusLogger logger)
        {
            this.Logger = logger;
            return this;
        }

        /// <summary>
        /// Sets the publish configuration.
        /// </summary>
        /// <param name="configure">The configure action</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithPublishConfiguration(Action<IPublishConfiguration> configure)
        {
            this.PublishConfiguration = () => 
                {
                    IPublishConfiguration configuration = new PublishConfiguration();
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
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithSubscriptionConfiguration(Action<ISubscriptionConfiguration> configure)
        {
            this.SubscribtionConfiguration = () =>
            {
                ISubscriptionConfiguration configuration = new SubscriptionConfiguration();
                configure(configuration);
                return configuration;
            };

            return this;
        }

        public IBusConfiguration WithConnectionString(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}