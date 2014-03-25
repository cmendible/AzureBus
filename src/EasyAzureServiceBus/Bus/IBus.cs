namespace EasyAzureServiceBus
{
    using System;

    /// <summary>
    /// Provides a simple Publish/Subscribe API for a message bus.
    /// </summary>
    public interface IBus : IDisposable
    {
        /// <summary>
        /// Publishes a message.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        void Publish<T>(T message) where T : class;

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
        void Subscribe<T>(string subscriptionId, Action<T> onMessage);
    }
}
