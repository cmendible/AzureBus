namespace EasyAzureServiceBus
{
    using System;

    /// <summary>
    /// Provides a simple API to send messages to Azure queue.
    /// </summary>
    public interface IQueue : IDisposable
    {
        /// <summary>
        /// Sends a message to the Queue.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message to publish</param>
        void Send<T>(T message) where T : class;

        /// <summary>
        /// Subscribes to a stream of messages that match a .NET type.
        /// </summary>
        /// <typeparam name="T">The type to subscribe to</typeparam>
        /// <param name="onMessage">
        /// The action to run when a message arrives.
        /// </param>
        void Subscribe<T>(Action<T> onMessage);
    }
}
