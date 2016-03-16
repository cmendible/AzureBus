namespace AzureBus
{
    public static class BusConfigurationExtensions
    {
        public static IBus CreateBus(this IBusConfiguration configuration)
        {
            return new Bus(configuration);
        }
    }
}
