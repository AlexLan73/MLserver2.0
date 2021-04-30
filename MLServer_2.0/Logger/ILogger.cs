using Convert.Moduls;
using System;

// ReSharper disable once CheckNamespace
namespace Convert.Logger
{
    public interface ILogger
    {
        public void SetExitProgrammAsync();
        public void Dispose();
    }

    public class LoggerEvent//: EventArgs
    {
        public LoggerEvent(EnumError enumError, string[] stringDan, EnumLogger enumLogger = EnumLogger.MonitorFile)
        {
            DateTime = DateTime.Now;
            EnumError = enumError;
            StringDan = stringDan;
            EnumLogger = enumLogger;
        }
        public LoggerEvent(EnumError enumError, string stringDan, EnumLogger enumLogger = EnumLogger.MonitorFile)
        {
            DateTime = DateTime.Now;
            EnumError = enumError;
            StringDan = new[] { stringDan };
            EnumLogger = enumLogger;
        }

        public DateTime DateTime { get; set; }
        public string[] StringDan { get; set; }
        public EnumLogger EnumLogger { get; set; }
        public EnumError EnumError { get; set; }
    }
}
