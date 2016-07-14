namespace AzureBus.Sample
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Sample message
    /// </summary>
    [DataContract]
    public class SampleMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleMessage"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public SampleMessage(string value, bool activo)
        {
            this.Value = value;
            this.Activo = activo;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public string Value { get; private set; }

        [DataMember]
        public bool Activo { get; private set; }
    }

    /// <summary>
    /// Sample message
    /// </summary>
    [DataContract]
    public class AnotherSampleMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleMessage"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public AnotherSampleMessage(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DataMember]
        public string Value { get; private set; }
    }
}
