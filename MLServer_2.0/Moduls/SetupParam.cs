using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using System.Collections.Concurrent;
using System.Collections.Generic;


// ReSharper disable once CheckNamespace
namespace Convert.Moduls
{
    public class SetupParam
    {
        #region Data
        private Config0 _config;

        private Dictionary<string, string> _nameFile;

        private static string[] FileDanMlRt => new[] { "filename", "carname", "sernum" };
        private readonly string[] _fildsMlRt2 = { "trigger", "compilationtimestamp" };

        private MlServerJson _mLServerJson;

        #endregion
        public SetupParam(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Запуск (Start) class SetupParam "));
            this._config = config;
            _inicial01();

        }

        private void _inicial01()
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info,
                    new[]{ "SetupParam \n"
                    , "инициализация параметров: \n"
                    , " - для ml_rt, ml_rt2, TextLog \n"
                    , " lrd = -S 20 -L 512 -n -k -v -i \n"
                    , "_mdf = -v -~ -o -t -l \"file_clf\" -MB -O  \"my_dir\"    \"SystemChannel=Binlog_GL.ini\"" }));

            _nameFile = new Dictionary<string, string>
            {
                {"ml_rt",   _config.MPath.WorkDir + "\\ml_rt.ini"},
                {"ml_rt2",  _config.MPath.WorkDir + "\\ml_rt2.ini"},
                {"TextLog", _config.MPath.WorkDir + "\\TextLog.txt"}
            };

            string _lrd = " -S 20 -L 512 -n -k -v -i ";
            string _mdf = " -v -~ -o -t -l \"file_clf\" -MB -O  \"my_dir\" SystemChannel=Binlog_GL.ini";

            ConcurrentDictionary<string, string> mdf0 = new();
            mdf0.AddOrUpdate("commanda", _mdf, (_, _) => _mdf);
            mdf0.AddOrUpdate("ext", "mdf", (_, _) => "mdf");

            _config.BasaParams.AddOrUpdate("lrf_dec", _lrd, (_, _) => _lrd);
            _config.ClexportParams.AddOrUpdate("MDF", mdf0, (_, _) => mdf0);
        }

        public bool IniciaPathJson()
        {
            //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   
            //     Настроить загрузку данных 

            _ = LoggerManager.AddLoggerAsync(
                new LoggerEvent(EnumError.Info, new[] { "SetupParam \n", "Грузим файл конфигурации DbConfig \n" +
                                                                         "Loading the DbConfig configuration file" }));

            JsonBasa.LoadFileJsoDbConfig();

            var mlrt = new MlRt(_nameFile["ml_rt"], FileDanMlRt, ref _config);
            var resul = mlrt.Convert();
            if (resul)
                return true;

            _mLServerJson = new MlServerJson(ref _config);
            _mLServerJson.IniciallMLServer(_config.Fields.ContainsKey("carname") ? _config.Fields["carname"] : "");

            new MlRt2(_nameFile["ml_rt2"], _fildsMlRt2, ref _config).Convert();

            new TextLog(_nameFile["TextLog"], "trigger", ref _config).Convert();

            var analis = new Analysis(ref _config).Convert();
            if (analis == "")
                _ = ErrorBasa.FError(-25);

            _config.MPath.Analis = analis;

            new ParsingXml(ref _config).Convert();

            return false;
        }

    }
}
