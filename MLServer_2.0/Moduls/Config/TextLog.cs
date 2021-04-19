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
        private DanTriggerTime _convert;
        public List<DanTriggerTime> _dateTimeTrigger;
        private readonly string _nameModulConfig;
        public TextLog(string filename, string field, ILogger llogger, string nameModulConfig, ref Config0 config) 
                                : base(filename, field, llogger, ref config)
        {
            _dateTimeTrigger = new List<DanTriggerTime>();
            _convert = new DanTriggerTime();
            _nameModulConfig = nameModulConfig;
        }

        public sealed override bool Convert()
        {
            var result = ReadIni();

            if (result)  return true;

            if (Config.NameTrigger.Count <= 0)
            {
                 _ = ErrorBasa.FError(-23, Filename);
                return false;
//                var sResulT0 = new SResulT0(-23, $"В файле {Filename} нет данных о Trigger и времени", _nameModulConfig);
//                Task.Run(() => ILoger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Warning, sResulT0, EnumLogger.Monitor)));
//                return new ResultTd<bool, SResulT0>(false);
            }

            foreach (var item in Ldata.Where(item => item.ToLower().Contains(Field)))
                _dateTimeTrigger.Add(new DanTriggerTime(item));

            if (_dateTimeTrigger.Count <= 0)
            {
                _ = ErrorBasa.FError(-213);
                return false;
            }
//            return new ResultTd<bool, SResulT0>(new SResulT0(-213, " Нет данных в TextLog, время срабатывания триггера ", "==> Модуль ConfigProgramm "));

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
