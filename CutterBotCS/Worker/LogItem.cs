using System;

namespace CutterBotCS.Worker
{
    /// <summary>
    /// Log Item
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LogItem(string message, LogType logtype)
        {
            Message = message;  
            LogType = logtype;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Log Message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Log Type
        /// </summary>
        public LogType LogType { get; private set; }    

        /// <summary>
        /// Timestamp of Log
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// To String
        /// </summary>
        public override string ToString()
        {
            return String.Format("[{0}] {1}", Timestamp.ToString("yyyy/MM/dd HH:mm:ss"), Message);
        }
    }

    /// <summary>
    /// Log Type
    /// </summary>
    public enum LogType
    {
        Info,
        Warning,
        Error
    }
}
