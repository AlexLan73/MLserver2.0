using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.Config
{
    public class MlRt : IniProcessing
    {
        private readonly string _nameModulConfig;
        public MlRt(string filename, string[] fields, ILogger ilogger, string nameModulConfig, ref Config0 config) 
                                    : base(filename, fields, ilogger, ref config)
        {
            _nameModulConfig = nameModulConfig;
        }

        public override bool Convert()
        {
            var result = ReadIni();

            if (result)
                return true;

            if (base.Convert())
            {
                if (!Data.ContainsKey("filename"))
                {
                    _ = ErrorBasa.FError(-211);
                    return false;

//                    var sResulT0 = new SResulT0(-211, $"В файле {Filename} нет поля => filename", _nameModulConfig);
//                    Task.Run(() => ILoger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.Monitor)));
///                    return new ResultTd<bool, SResulT0>(sResulT0);
                }
            }

            foreach ((var key, var val) in Data)
                Config.Fields.AddOrUpdate(key, val, (_, _) => val);

            return false;
        }
    }
}


/*
 
             if (!_nameFile["ml_rt"].Item2)
            {
                var sResulT0 = new SResulT0(-20, $"Нет файла {_nameFile["ml_rt"].Item1}", nameModulConfig);
                Task.Run(() => iLoger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.Monitor)));
                return new ResultTd<bool, SResulT0>(sResulT0);
            }

            var mlrt = new MlRt(_nameFile["ml_rt"].Item1, _fileDanMlRt);
            if (mlrt.Convert())
            {
                if (!mlrt.Data.ContainsKey("filename"))
                {
                    var sResulT0 = new SResulT0(-211, $"В файле {_nameFile["ml_rt"].Item1} нет поля => filename", nameModulConfig);
                        Task.Run(() => iLoger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.Monitor)));
                        return new ResultTd<bool, SResulT0>(sResulT0);
                }
            }

            foreach (var item in _fileDanMlRt)
                if (mlrt.Data.ContainsKey(item))
                    _dGlobal.Config.AddOrUpdate(item, mlrt.Data[item], (_, _) => mlrt.Data[item]);

 
 */