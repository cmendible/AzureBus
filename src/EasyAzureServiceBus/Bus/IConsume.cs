namespace EasyAzureServiceBus
{
    /// <summary>
    /// Message consumer contract.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsume<in T> where T : class
    {
        /// <summary>
        /// Consumes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Consume(T message);
    }
}