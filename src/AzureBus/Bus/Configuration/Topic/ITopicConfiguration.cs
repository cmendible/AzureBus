namespace AzureBus
{
    using System;

    public interface ITopicConfiguration
    {
        Func<Type, string> GetTopicName { get; }
        TimeSpan DuplicateDetectionHistoryTimeWindow { get; }
        bool RequiresDuplicateDetection { get; }
    }
}
