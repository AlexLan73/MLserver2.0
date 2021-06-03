using Convert.Moduls.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.ClfFileType
{
    public class MemoryInfo
    {
        #region data
        public string FMemory { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public string StartEndMem => StrStartStop();
        public List<DanTriggerTime> TriggerInfo { get; private set; }
        #endregion

        #region constructor
        [JsonConstructor]
        public MemoryInfo(string fMemory, DateTime start, DateTime end, List<DanTriggerTime> triggerInfo)
        {
            this.FMemory = fMemory;
            Start = start;
            End = end;
            TriggerInfo = new List<DanTriggerTime>(triggerInfo);
            StrStartStop();
        }
        public MemoryInfo(string name, DateTime start, DateTime end)
        {
            FMemory = name;
            Start = start;
            End = end;
            StrStartStop();
            TriggerInfo = new List<DanTriggerTime>();
        }
        #endregion

        #region GetTrigger
        public string GetNameTrigger()
        {
            if (TriggerInfo.Count <= 0)
                return "";

            const string s = "_(x)";
            var s0 = TriggerInfo
                                .Select(x => x.Trigger
                                .Split(" ")[1])
                                .ToList()
                                .Distinct()
                                .ToArray()
                                .Aggregate("", (current, item) => current + s.Replace("x", item));

            return s0.Length > 0 ? "_Trigger" + s0 : "";
        }
        private string StrStartStop()
        {
            //var file = Path.GetFileNameWithoutExtension(pathfile).Split("_", 3);
            //var ext = Path.GetExtension(pathfile);
            //FMemory = Regex.Match(file[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;

            return Start.ToString("(yyyy-MM-dd_HH-mm-ss)") + "_" + End.ToString("(yyyy-MM-dd_HH-mm-ss)")+ "_" + FMemory;
        }

        #endregion

    }
}
