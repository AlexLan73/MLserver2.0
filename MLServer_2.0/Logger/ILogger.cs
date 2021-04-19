using System;
using MLServer_2._0.Moduls;

namespace MLServer_2._0.Logger
{
    public interface ILogger
    {
        public void AddLoggerInfoAsync(LoggerEvent e);
        public void SetExitProgrammAsync();
        public void SetRun(bool t);
        public void Dispose();
    }

    public class LoggerEvent//: EventArgs
    {
        public LoggerEvent(DateTime dateTime, EnumError enumError, string[] stringDan, EnumLogger enumLogger)
        {
            DateTime = dateTime;
            EnumError = enumError;
            StringDan = stringDan;
            EnumLogger = enumLogger;
        }
        public LoggerEvent(EnumError enumError, string[] stringDan, EnumLogger enumLogger)
        {
            DateTime = DateTime.Now;
            EnumError = enumError;
            StringDan = stringDan;
            EnumLogger = enumLogger;
        }
        public LoggerEvent(EnumError enumError,  string stringDan, EnumLogger enumLogger=EnumLogger.MonitorFile)
        {
            DateTime = DateTime.Now;
            EnumError = enumError;
            StringDan = new []{ stringDan };
            EnumLogger = enumLogger;
        }

        public LoggerEvent(EnumError enumErro, SResulT0 souese, EnumLogger enumLogger)
        {

            DateTime = DateTime.Now;
            StringDan = new[] {$"код ошибки - {souese.Error}; название -{souese.NameError} ", $" =>=> {souese.NameRazdel} "};
            EnumError = enumErro;
            EnumLogger = enumLogger;
        }


        public DateTime DateTime { get; set; }
        public string[] StringDan { get; set; }
        public EnumLogger EnumLogger { get; set; }
        public EnumError EnumError { get; set; }
    }
}
