using Convert.Interface.Config;
using System;
using System.Globalization;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class DanTriggerTime : IDanTriggerTime
    {
        #region Data
        public DateTime DateTime { get; set; }
        public string Trigger { get; set; }
        public string Work { get; set; }
        public string Name { get; set; }

        #endregion

        #region constructor
        public DanTriggerTime(string dan)
        {
            Convert(dan);
        }

        public DanTriggerTime(IDanTriggerTime sourse)
        {
            DateTime = sourse.DateTime;
            Trigger = sourse.Trigger;
            Work = sourse.Work;
            Name = sourse.Name;
        }

        public DanTriggerTime()
        {
        }
        public DanTriggerTime Convert(string dan)
        {
            var d0 = dan.Split(",")[1];
            var ds0 = d0.Split(": ");
            var data = ds0[0].Trim();
            var trigger0 = ds0[1].Trim();
            var triggers = trigger0.Split(" ");
            Trigger = triggers[0] + " " + triggers[1];
            Work = triggers[2].Trim();

            DateTime = DateTime.ParseExact(data, "dd.MM.yyyy HH:mm:ss.ff", CultureInfo.InvariantCulture);
            return this;
        }
        #endregion

    }
}
