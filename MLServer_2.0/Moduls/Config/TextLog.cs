using Convert.Interface.Config;
using Convert.Logger;
using Convert.Moduls.Error;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class TextLog : IniProcessing, ITriggerTimeName
    {
        public List<DanTriggerTime> DateTimeTrigger;
        public TextLog(string filename, string field, ref Config0 config) : base(filename, field, ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем (Load) Class TextLog"));

            DateTimeTrigger = new List<DanTriggerTime>();
        }

        public sealed override bool Convert()
        {
            var result = ReadIni();

            if (result) return true;

            if (Config.NameTrigger.Count <= 0)
            {
                _ = ErrorBasa.FError(-23, Filename);
                return false;
            }

            foreach (var item in Ldata.Where(item => item.ToLower().Contains(Field)))
                DateTimeTrigger.Add(new DanTriggerTime(item));

            if (DateTimeTrigger.Count <= 0)
            {
                _ = ErrorBasa.FError(-213);
                return false;
            }

            for (var i = 0; i < DateTimeTrigger.Count; i++)
            {
                var z = DateTimeTrigger[i];
                z.Name = Config.NameTrigger.ContainsKey(z.Trigger) ? Config.NameTrigger[z.Trigger] : "";
                DateTimeTrigger[i] = new DanTriggerTime(z);
            }

            Config.DateTimeTrigger = new List<DanTriggerTime>(DateTimeTrigger);

            return false;
        }

        public List<DanTriggerTime> ReadInfoTimeTrigger(string sdata0, string sdata1)
        {
            return ReadInfoTimeTrigger(
                DateTime.ParseExact(sdata0, "dd.MM.yyyy HH:mm:ss.ff", CultureInfo.InvariantCulture),
                DateTime.ParseExact(sdata1, "dd.MM.yyyy HH:mm:ss.ff", CultureInfo.InvariantCulture)
                );
        }
        public List<DanTriggerTime> ReadInfoTimeTrigger(DateTime data0, DateTime data1) =>
            DateTimeTrigger.Where(x => x.DateTime >= data0 && x.DateTime < data1).ToList();
    }
}
