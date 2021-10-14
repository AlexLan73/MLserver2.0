using Convert.Logger;
using Convert.Moduls.ClfFileType;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TypeDStringMemoryInfo = System.Collections.Concurrent.ConcurrentDictionary<string,
        System.Collections.Concurrent.ConcurrentDictionary<string, Convert.Moduls.ClfFileType.MemoryInfo>>;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class Config0
    {
        #region data
        public IsRun IsRun;
        public MasPaths MPath;

        public ConcurrentDictionary<string, string> BasaParams;
        public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ClexportParams;
        public ConcurrentDictionary<string, string> Fields;
        public List<DanTriggerTime> DateTimeTrigger;
        public ConcurrentDictionary<string, string> NameTrigger { get; set; }

        public TypeDStringMemoryInfo FileMemInfo;
        public ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>> DbConfig;
        public ConcurrentDictionary<string, MemoryInfo> FMem { get; set; }

        public string SiglogFileInfo { get; set; }
        public string VSysVarPath { get; set; }
        public string VSysVarType { get; set; }

        private Timer _timer1sec;
        public event EventHandler Time1Sec;

        #endregion
        public Config0()
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем (Load) Class Config0"));

            IsRun = new IsRun();
            BasaParams = new ConcurrentDictionary<string, string>();
            ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            Fields = new ConcurrentDictionary<string, string>();
            DateTimeTrigger = new List<DanTriggerTime>();
            NameTrigger = new ConcurrentDictionary<string, string>();
            FileMemInfo = new TypeDStringMemoryInfo();
            FMem = new ConcurrentDictionary<string, MemoryInfo>();
            DbConfig = new();
            Time1Sec += Config0_Time1Sec;

            _timer1sec = new Timer(FTime1Sec, null, 0, 1000);
        }

        private void Config0_Time1Sec(object sender, EventArgs e) { }
        private void FTime1Sec(object stateInfo)
        {
            Time1Sec?.Invoke(this, null!);
        }
        public void StopTime()
        {
            Time1Sec -= Config0_Time1Sec;
            _timer1sec.Dispose();
        }
    }
}

