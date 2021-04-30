using Convert.Moduls.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Convert.Moduls.ClfFileType
{
    public class MemoryInfo
    {
        #region data
        public string FMemory { get; private set; }
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }
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
        }
        public MemoryInfo(string name, DateTime start, DateTime end)
        {
            FMemory = name;
            Start = start;
            End = end;
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
        #endregion

    }
}
