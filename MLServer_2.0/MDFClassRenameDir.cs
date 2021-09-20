using Convert.Logger;
using Convert.Moduls;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using MLServer_2._0.Moduls.MDFRename;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLServer_2._0
{
    public class MDFClassRenameDir
    {
        private Dictionary<string, string> _dArgs;
        public MDFClassRenameDir(Dictionary<string, string> dArgs)
        {
            _dArgs= dArgs;
        }
        public void Run()
        {
            LoggerManager logger = new(_dArgs["WorkDir"] + "\\Log");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Входные данные проверенные"));

            var errorBasa = new ErrorBasa();
            Config0 config = new();
            var jsonBasa = new JsonBasa(ref config);
            config.MPath = new MasPaths(_dArgs);

            var resul = config.MPath.FormPath();
            if (resul)
            {
                var error = ErrorBasa.FError(-4);
                error.Wait();
            }

            SetupParam _setupParam = new(ref config);
            _setupParam.IniciaPathJson();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " - Инициализация параметров закончилась "));

            ConcurrentDictionary<string, int> _pathFileMDF = new ConcurrentDictionary<string, int>();
            DateTime _dataStart = new DateTime(2020, 11, 2);
            DateTime _dataEnd = new DateTime(2021, 06, 03, 16, 0, 0);

            var _findMdf = new FindDirMDF(_dArgs["MDFRenameDir"], ref _pathFileMDF, _dataStart, _dataEnd);
            var _waitFindDir = _findMdf.Run();
            _waitFindDir.Wait();

            var _keyPaths = _pathFileMDF.Keys;

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "########## - Запуск переименования ########", EnumLogger.Monitor));

            List<Task> _runReanme = new List<Task>();
            foreach (var item in _keyPaths)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $"##  -> {item}   ##", EnumLogger.Monitor));
                _runReanme.Add(new RenameMDF(item).Run());
            }

            while (_runReanme.Count > 0)
            {
                try
                {
                    Console.WriteLine($"#__ осталось дождаться -{_runReanme.Count}   __#");

                    var x = _runReanme[0];
                    x.Wait();
                    _runReanme.RemoveAt(0);
                }
                catch (Exception)
                {
                    _runReanme.RemoveAt(0);
                }
            }

            logger.Dispose();
            Console.WriteLine("Все   - режим MDFRenameDir ))");

        }
    }
}
