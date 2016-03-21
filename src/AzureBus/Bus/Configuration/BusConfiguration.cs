﻿namespace AzureBus
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

        public IAzureBusLogger Logger
        {
            get;
            private set;
        }

        public Func<IPublishConfiguration> PublishConfiguration
        {
            get;
            private set;
        }

        public Func<ISubscriptionConfiguration> SubscribtionConfiguration
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
            this.SubscribtionConfiguration = () => { return new SubscriptionConfiguration(); };
        }

        /// <summary>
        /// Sets the connection string
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>An BusConfiguration object.</returns>
        public IBusConfiguration WithConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        public IBusConfiguration WithLogger(IAzureBusLogger logger)
        {
            this.Logger = logger;
            return this;
        }

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
    }
}