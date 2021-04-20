using MLServer_2._0.Moduls.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MLServer_2._0.Moduls.ClfFileType
{
    public class MemoryInfo
    {
        public string FMemory { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
        public List<DanTriggerTime> TriggerInfo { get; private set; }
        [JsonConstructor]
        public MemoryInfo(string fMemory, DateTime start, DateTime end, List<DanTriggerTime> triggerInfo)
        {
            this.FMemory = fMemory;
            Start = start;
            End = end;
            TriggerInfo = new List<DanTriggerTime>(triggerInfo);
        }
        public MemoryInfo(string name, DateTime start, DateTime end)
        {
            FMemory = name;
            Start = start;
            End = end;
            TriggerInfo = new List<DanTriggerTime>();
        }

        public string GetNameTrigger()
        {
            if (TriggerInfo.Count <= 0)
                return "";

            string s = "_(x)";
            string s0 = "";
            foreach (var item in TriggerInfo.Select(x => x.Trigger.Split(" ")[1]).ToList().Distinct().ToArray())
                s0 += s.Replace("x", item);

            return s0.Length > 0 ? "_Trigger" + s0 : "";
        }

    }
}
