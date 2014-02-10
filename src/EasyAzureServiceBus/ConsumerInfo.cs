namespace EasyAzureServiceBus
{
    using System;

    /// <summary>
    /// Describes a meesage consumer
    /// </summary>
    [Serializable]
    public class ConsumerInfo
    {
        /// <summary>
        /// The concrete type
        /// </summary>
        public readonly Type ConcreteType;

        /// <summary>
        /// The interface type
        /// </summary>
        public readonly Type InterfaceType;

        /// <summary>
        /// The message type
        /// </summary>
        public readonly Type MessageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerInfo"/> class.
        /// </summary>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="messageType">Type of the message.</param>
        public ConsumerInfo(Type concreteType, Type interfaceType, Type messageType)
        {
            ConcreteType = concreteType;
            InterfaceType = interfaceType;
            MessageType = messageType;
        }
    }
}