namespace AzureBus
{
    /// <summary>
    /// Simplifies Service Bus instance creation.
    /// </summary>
    public static class AzureCloud
    {
        /// <summary>
        /// Creates an BusConfiguration object.
        /// </summary>
        /// <returns>An BusConfiguration object.</returns>
        public static IBusConfiguration ConfigureBus()
        {
            return new BusConfiguration();
        }
    }
}