namespace AzureBus
{
    public static class AzureBusConfigurationExtensions
    {
        public static IBus CreateBus(this IAzureBusConfiguration configuration)
        {
            return new AzureBus(configuration);
        }
    }
}
