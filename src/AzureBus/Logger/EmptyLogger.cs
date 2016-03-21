namespace AzureBus.Loggers
{
    using System;

    public class EmptyLogger : IAzureBusLogger
    {
        public void Debug(object message)
        {
        }

        public void DebugFormat(string format, params object[] args)
        {
        }

        public void Error(object message)
        {
        }

        public void Error(object message, Exception exception)
        {
        }

        public void ErrorFormat(string format, params object[] args)
        {
        }

        public void Fatal(object message)
        {
        }

        public void Fatal(object message, Exception exception)
        {
        }

        public void FatalFormat(string format, params object[] args)
        {
        }

        public void Info(object message)
        {
        }

        public void InfoFormat(string format, params object[] args)
        {
        }

    }
}