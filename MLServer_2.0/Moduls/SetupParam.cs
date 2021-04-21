using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TypeDStringMemoryInfo = System.Collections.Concurrent.ConcurrentDictionary<string,
                                System.Collections.Concurrent.ConcurrentDictionary<string,
                                    MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;


namespace MLServer_2._0.Moduls
{
    public class SetupParam
    {
        #region Data
        private Config0 _config;

        private Dictionary<string, string> _nameFile;

        private readonly ILogger _iLogger;
        private readonly IInputArgumentsDop _inputArguments;
        private readonly IJsonBasa _jsonBasa;
        private string[] _fileDanMlRt => new[] { "filename", "carname", "sernum" };
        private readonly string[] _fildsMlRt2 = { "trigger", "compilationtimestamp" };
        private const string NameModulConfig = "Модуль SetupParam ";

        private MlServerJson _mLServerJson;
        private ParsingXml _xml;
        private InputArguments inputArguments;

        #endregion

            #region Пока не удалять
            //string _fileJson = _workDir + "\\dbconfig.json";
            //if (File.Exists(_fileJson))
            //{
            //    string json = File.ReadAllText(_fileJson);
            //    DbConfig = JsonConvert.DeserializeObject<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(json);
            //}
            #endregion
            //            JsonBasa jsonBasa = new JsonBasa();
            //                jsonBasa.LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(_workDir + "\\dbconfig.json");

        public SetupParam(ref Config0 config, IJsonBasa jsonBasa)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Запуск class SetupParam "));
            _config = config;
            _jsonBasa = jsonBasa;

            _inicial01();

        }

        private void _inicial01()
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, 
                    new []{ "SetupParam \n"
                    , "инициализация параметров: \n"
                    , " - для ml_rt, ml_rt2, TextLog \n"
                    , " lrd = -S 20 -L 512 -n -k -v -i \n"
                    , "_mdf = -v -~ -o -t -l \"file_clf\" -MB -O  \"my_dir\" SystemChannel=Binlog_GL.ini" }));

            _nameFile = new Dictionary<string, string>
            {
                {"ml_rt",   _config.MPath.WorkDir + "\\ml_rt.ini"},
                {"ml_rt2",  _config.MPath.WorkDir + "\\ml_rt2.ini"},
                {"TextLog", _config.MPath.WorkDir + "\\TextLog.txt"}
            };

            string _lrd = " -S 20 -L 512 -n -k -v -i ";
            string _mdf = " -v -~ -o -t -l \"file_clf\" -MB -O  \"my_dir\" SystemChannel=Binlog_GL.ini";

            ConcurrentDictionary<string, string> _mdf0 = new();
            _mdf0.AddOrUpdate("commanda", _mdf, (_, _) => _mdf);
            _mdf0.AddOrUpdate("ext", "mdf", (_, _) => "mdf");

            _config.BasaParams.AddOrUpdate("lrf_dec", _lrd, (_, _) => _lrd);
            _config.ClexportParams.AddOrUpdate("MDF", _mdf0, (_, _) => _mdf0);
        }

        public bool IniciaPathJson()
        {
            //  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   
            //     Настроить загрузку данных 

            _ = LoggerManager.AddLoggerAsync(
                new LoggerEvent(EnumError.Info, new[] { "SetupParam \n","Грузим файл конфигурации DbConfig " }));

            _jsonBasa.LoadFileJsoDbConfig();

            var mlrt = new MlRt(_nameFile["ml_rt"], _fileDanMlRt, ref _config);
           var resul = mlrt.Convert();
           if (resul)
                return resul;

            _mLServerJson = new MlServerJson(ref _config);
            _mLServerJson.IniciallMLServer(_config.Fields.ContainsKey("carname") ? _config.Fields["carname"] : "");

            new MlRt2(_nameFile["ml_rt2"], _fildsMlRt2, ref _config).Convert();

            new TextLog(_nameFile["TextLog"], "trigger",  ref _config).Convert();

            string _analis = new Analysis(ref _config).Convert();
            if (_analis == "")
                _ = ErrorBasa.FError(-25);
            _config.MPath.Analis = _analis;

            new ParsingXml(ref _config).Convert();

            //////
            ///      добавить обработку  InicialAnalysis InicialXml
            //////

            return false;
        }

    }
}
