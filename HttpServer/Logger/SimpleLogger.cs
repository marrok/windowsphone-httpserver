using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HttpServer.Logger
{
    public delegate void LoggerDelegate(String message);

    public enum LogLevel
    {
        INFO = 0,
        ERROR,
        WARN,
        OFF
    };

    public sealed class SimpleLogger : IDisposable
    {
        private static volatile SimpleLogger _instance;
        private Semaphore _semaphore = new Semaphore(1, 1);

        private LogLevel _currentLevel;
        private LoggerDelegate _loggerDelegate;
        private LoggerBuffer _buffer;

        private SimpleLogger(LogLevel logLevel, LoggerDelegate loggerDelegate, int maxLogEntries)
        {
            _currentLevel = logLevel;
            _loggerDelegate = loggerDelegate;
            _buffer = new LoggerBuffer(maxLogEntries);
        }

        public static SimpleLogger GetLogger()
        {
            return _instance;
        }

        private void Log(LogLevel logLevel, String message, params string[] messageParams)
        {
            if (logLevel >= _currentLevel && _loggerDelegate != null)
            {
                try
                {
                    _semaphore.WaitOne();
                    var messageBuilder = new StringBuilder();
                    messageBuilder.AppendFormat("[{0}]: ", logLevel);
                    messageBuilder.AppendFormat(message, messageParams);
                    messageBuilder.AppendLine();

                    _buffer.AddMessage(messageBuilder.ToString());
                    _loggerDelegate(_buffer.GetBufferedMessages());
                }
                finally
                {
                    _semaphore.Release(1);
                }
            }
        }

        internal static SimpleLogger InitializeLogger(LogLevel logLevel, LoggerDelegate loggerDelegate, int maxLogEntries)
        {
            if (_instance == null)
            {
                _instance = new SimpleLogger(logLevel, loggerDelegate, maxLogEntries);
            }
            return _instance;
        }

        internal void ChangeLogLevel(LogLevel newLogLevel)
        {
            try
            {
                _semaphore.WaitOne();
                _currentLevel = newLogLevel;
            }
            finally
            {
                _semaphore.Release(1);
            }
        }

        public void Info(String message, params string[] messageParams)
        {
            Log(LogLevel.INFO, message, messageParams);
        }

        public void Warning(String message, params string[] messageParams)
        {
            Log(LogLevel.WARN, message, messageParams);
        }

        public void Error(String message, params string[] messageParams)
        {
            Log(LogLevel.ERROR, message, messageParams);
        }

        private class LoggerBuffer
        {
            private StringBuilder _buffer;
            private readonly int _maxEntries;
            private readonly Queue<int> _entriesQueue;

            internal LoggerBuffer(int maxEntries)
            {
                _buffer = new StringBuilder();
                _maxEntries = maxEntries;
                _entriesQueue = new Queue<int>(maxEntries);
            }

            internal void AddMessage(String message)
            {
                _buffer.Insert(0, message);
                _entriesQueue.Enqueue(message.Length);

                if (_entriesQueue.Count > _maxEntries)
                {
                    var lastLogEntryLength = _entriesQueue.Dequeue();
                    _buffer.Remove(_buffer.Length - lastLogEntryLength, lastLogEntryLength);
                }
            }

            internal String GetBufferedMessages()
            {
                return _buffer.ToString();
            }
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
