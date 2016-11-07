namespace AzureBus
{
    using System;
    using System.Configuration;
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
        public Func<ISubscriptionConfiguration> SubscriptionConfiguration
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BusConfiguration"/> class.
        /// </summary>
        internal BusConfiguration()
        {
            this.ConnectionString = ConfigurationManager.AppSettings.AllKeys.Contains("Microsoft.ServiceBus.ConnectionString") ? ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"] : string.Empty;

            this.Logger = new EmptyLogger();

            this.PublishConfiguration = () => { return new PublishConfiguration(); };
            this.SubscriptionConfiguration = () => { return new SubscriptionConfiguration(); };
        }

        /// <summary>
        /// Sets the connection string
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// The BusConfiguration instance.
        /// </returns>
        public IBusConfiguration WithConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
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