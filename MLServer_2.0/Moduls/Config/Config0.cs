using System.Collections.Concurrent;
using System.Collections.Generic;
using MLServer_2._0.Moduls.ClfFileType;
using TypeDStringMemoryInfo = System.Collections.Concurrent.ConcurrentDictionary<string,
        System.Collections.Concurrent.ConcurrentDictionary<string,  MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;
using System;
using System.Threading;
using System.Threading.Tasks;

//using LXmld = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;

namespace MLServer_2._0.Moduls.Config
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

        public string SiglogFileInfo { get; set; }
        public string VSysVarPath { get; set; }
        public string VSysVarType { get; set; }

        private Timer _timer1sec;
        public event EventHandler Time1Sec;

        #endregion
        public Config0()
        {
            IsRun = new IsRun();
            BasaParams = new ConcurrentDictionary<string, string>();
            ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            Fields = new ConcurrentDictionary<string, string>();
            DateTimeTrigger = new List<DanTriggerTime>();
            NameTrigger = new ConcurrentDictionary<string, string>();
            FileMemInfo = new TypeDStringMemoryInfo();
            DbConfig = new();
            Time1Sec += Config0_Time1Sec;

            _timer1sec = new Timer(fTime1Sec, null, 0, 1000);
        }

        private void Config0_Time1Sec(object sender, EventArgs e)   {}
        private void fTime1Sec(Object stateInfo) 
        { 
            Time1Sec(this, null); 
        }
        public void StopTime()
        {
            Time1Sec -= Config0_Time1Sec;
            _timer1sec.Dispose();
        }
    }
}


/*
 
         public class Config //: IInterface
        {

            public ConcurrentDictionary<int, string> DError;

            public async Task ErrorRunAsync(int error)
            {

                await Task.Factory.StartNew((object nError) =>
                {
                    WriteLine($"  kod- {(int) nError} ");
                }, error);
            }
            public async Task ForErrorRunAsync(int nnn)
            {

                await Task.Factory.StartNew((object nError) =>
                {
                    for (int i = 0; i < (int)nnn; i++)
                    {
                        ErrorRunAsync(i);

                    }
                }, nnn);
            }

        }
        public static async Task Main(string[] args)
        {
            Config _config = new Config();
            _config.ErrorRunAsync(-10);
            _config.ForErrorRunAsync(20);

 
 */