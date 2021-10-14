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
    public class BasaClassConvert
    {
        private Dictionary<string, string> _dArgs;
        public BasaClassConvert(Dictionary<string, string> dArgs)
        {
            _dArgs = dArgs;
        }
        public void Run()
        {
            LoggerManager logger = new(_dArgs["WorkDir"] + "\\Log");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Входные данные проверенные \n " +
                                                                             " Input data verified"));

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
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " - Инициализация параметров закончилась \n " +
                                                                             " - Parameter initialization finished"));

            //////////////////////////////////////////////////////////
            // ReSharper disable once InvalidXmlDocComment
            //  Выбрать режим конвертации
            //      1. Есть исходные данные тогда по полной схеме
            //          1.1. Запускаем процесс ConvertOne  IsRun.Sourse = true
            //          1.2. Анализ запуска модуля конвертации Если -> IsRun.Sourse = true есди false Запускаем процесс 3.
            //              1.2.1. Ждем пустой каталог \CLF\ когда при пустом каталоге появится первый файл запускаем процесс конвертации
            //              1.2.2. Если есть файлы соответствующие маске car_Mx_(2000-01-01_00-00-00)_(2000-01-01_00-00-01).clf запускаем процесс конвертации
            //          1.3. Запущенный процесс конвертации IsRun.Sourse -проверка на false (признак завершения ) 
            //      2. Исходных данных нет
            //          2.1. Проверка есть ли clf данные
            //              2.2.1. Есть ли clf файлы и  файл DbConfig.json
            //              - проверяем файлы clf в каталоге \CLF\ и наличие DbConfig.json если есть идем дальше
            //              - если нет копируем в корневой каталог (если есть файлы) и запускаем переименование
            //      3. Запускаем процес конвертации
            //      ---  обратить внимание на каталог формирования данных

            if (Directory.GetDirectories(_dArgs["WorkDir"], "!D*").Length > 0)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Запуск-> Обработка сырых данных \n" +
                                                                                 " Launch-> Raw Data Processing"));

                ConvertOne convertOne = new ConvertOne(ref config);
                convertOne.Run();

                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Завершение обработки \n " +
                                                                                 " Completion of processing"));

            }
            //  Существует каталог CLF с файлами + наличие файла файла DbConfig.json 
            //  Нет clf файлов в корневом каталоге
            else
            {
                if (File.Exists(_dArgs["WorkDir"] + "\\clf.json"))
                    File.Delete(_dArgs["WorkDir"] + "\\clf.json");

                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Режим Конвертации -> CLF -> MDF (...) \n" +
                                                                                 " Conversion Mode -> CLF -> MDF (...)"));

                if (!File.Exists(_dArgs["WorkDir"] + "\\DbConfig.json"))
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " нет файла DbConfig.json создаем его \n" +
                                                                                     " no DbConfig.json file create it"));
                    ConvertOne convertOne = new(ref config);
                    convertOne.Run();
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение \n" +
                                                                                     " Processing completion"));
                }

                if (Directory.Exists(config.MPath.Clf)
                    && (Directory.GetFiles(config.MPath.Clf, "*.clf").Length > 0)
                    && (Directory.GetFiles(config.MPath.WorkDir, "*.clf").Length == 0)
                    && File.Exists(config.MPath.WorkDir + "\\DbConfig.json"))
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Конвертируем -> CLF -> MDF (...)  \n" +
                                                                                     "Conversion Mode->CLF->MDF(...)"));

                    var converExport = new ConverExport(ref config);
                    converExport.Run();
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение \n" +
                                                                                     " Processing completion"));
                }
                else if (!Directory.Exists(config.MPath.Clf)
                    && (Directory.GetFiles(config.MPath.WorkDir, "*.clf").Length > 0))
                {
                    ConvertOne convertOne = new(ref config);
                    convertOne.Run();
                    var converExport = new ConverExport(ref config);
                    converExport.Run();

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение \n" +
                                                                                     " Processing completion"));
                }
            }
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Exit programm - " + DateTime.Now));

            logger.Dispose();
            Console.WriteLine("Все - основной раздел \n EXIT ))");

        }
    }
}
