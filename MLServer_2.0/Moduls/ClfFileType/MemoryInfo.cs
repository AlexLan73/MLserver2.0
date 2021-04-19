using MLServer_2._0.Moduls.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
    }
}
