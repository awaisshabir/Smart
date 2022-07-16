using System;

namespace Smart.Api.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
    }
}
