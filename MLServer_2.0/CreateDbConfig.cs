using Convert;
using Convert.Logger;
using Convert.Moduls;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLServer_2._0
{
    public class CreateDbConfig
    {
        private Dictionary<string, string> _dArgs;
        private LoggerManager logger=null;
        public CreateDbConfig(Dictionary<string, string> dArgs)
        {
            _dArgs = dArgs;
        }
        public void Run()
        {
            logger = new(_dArgs["RenameDir"] + "\\Log");
            var files = new FindDirClf(_dArgs["RenameDir"]).Run();
            foreach (var item in files)
            {
                _dArgs["WorkDir"] = item;
                _dArgs["OutputDir"] = item;

                if (File.Exists(item + "\\clf.json"))
                    File.Delete(item + "\\clf.json");

                if (File.Exists(item + "\\DbConfig.json"))
                    continue;

//                LoggerManager.NewNameFile(item);

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

                _ = LoggerManager.AddLoggerAsync(
                    new LoggerEvent(EnumError.Info, " - Включен режим переименования clf файлов и создание DbConfig.json  "));

                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, new[]{ "  Включен режим переименования:\n "
                                                                    , $" Работаем с каталогом - {item}" }));

                ConvertOne convertOne = new(ref config);
                convertOne.Run();
            }
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Перебрали все каталоги "));

            logger.Dispose();
            Console.WriteLine("Все  - режим Rename))");
        }
    }
}
