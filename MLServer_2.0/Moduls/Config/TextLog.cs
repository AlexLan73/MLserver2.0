using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;

namespace MLServer_2._0.Moduls.Config
{
    public class TextLog : IniProcessing, ITriggerTimeName
    {
        public List<DanTriggerTime> _dateTimeTrigger;
        public TextLog(string filename, string field, ref Config0 config) : base(filename, field, ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class TextLog"));

            _dateTimeTrigger = new List<DanTriggerTime>();
        }

        public sealed override bool Convert()
        {
            var result = ReadIni();

            if (result)  return true;

            if (Config.NameTrigger.Count <= 0)
            {
                 _ = ErrorBasa.FError(-23, Filename);
                return false;
            }

            foreach (var item in Ldata.Where(item => item.ToLower().Contains(Field)))
                _dateTimeTrigger.Add(new DanTriggerTime(item));

            if (_dateTimeTrigger.Count <= 0)
            {
                _ = ErrorBasa.FError(-213);
                return false;
            }

            for (var i = 0; i < _dateTimeTrigger.Count; i++)
            {
                var z = _dateTimeTrigger[i];
                z.Name = Config.NameTrigger.ContainsKey(z.Trigger) ? Config.NameTrigger[z.Trigger] : "";
                _dateTimeTrigger[i] = new DanTriggerTime(z);
            }

            Config.DateTimeTrigger = new List<DanTriggerTime>(_dateTimeTrigger);

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
            _dateTimeTrigger.Where(x => x.DateTime >= data0 && x.DateTime < data1).ToList();
    }
}
