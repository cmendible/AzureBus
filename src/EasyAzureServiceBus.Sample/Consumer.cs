namespace EasyAzureServiceBus.Sample
{
    using System;

    /// <summary>
    /// A sample message consumer
    /// </summary>
    public class Consumer : IConsume<SampleMessage>
    {
        /// <summary>
        /// Consumes the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Consume(SampleMessage message)
        {
            Console.WriteLine("\nReceiving message from Inventory...");
            Console.WriteLine(string.Format("Message received: Value = {0}", message.Value));
        }
    }
}
