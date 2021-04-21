// R-eSharper disable all StringLiteralTypo
//  строка запуска c:\mlserver\#common\DLL\lrf_dec.exe -S 20 -L 512 -n -k -v -i C:\MLserver\PS18LIM\log\2020-11-04_18-36-34
////  https://docs.microsoft.com/ru-ru/dotnet/core/deploying/single-file  создание публикации

//     ! "E:\MLserver\data\PS18SED\log\2020-08-19_12-59-54
//      \\mlmsrv\MLServer\PS18SED\log\2020-09-01_12-54-14
///    !  E:\MLserver\data\PS14SED\2021-03-05_14-18-26\CLF
///    ! "E:\MLserver\data\PS14SED\2021-03-05_14-18-26"
///    E:\MLserver\data\PS33SED\log\2020-09-15_15-43-43 — копия
///    
///   "out:E:\OutTest"   "rename:\\mlmsrv\MLServer\PTA10SUV"
//  "rename:E:\MLserver\data\PS18SED\log\2020-09-03_06-00-36"

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using MLServer_2._0.Moduls.Export;
using static System.Console;

namespace MLServer_2._0
{

    class Program
    {
//        static LoggerManager _logger;

        static void Main(string[] args)
        {


            var _inputArguments = new InputArguments(args);
            var resultError = _inputArguments.Parser();

            //////////////////////////////////////////////////////////
            ///     resultError -> true  если истина то лшибка
            /////////////////////////////////////////////////////////
            if (resultError)
            {
                WriteLine(" --->   Ошибка в командной строке");
                WriteLine($"   код ошибки {resultError.Error.Error}");
                WriteLine($"   название {resultError.Error.NameError}");
                WriteLine($"   раздел {resultError.Error.NameRazdel}");
                if (resultError.Error.Id != null)
                    WriteLine($"   код ID {resultError.Error.Id.Value}");
                Environment.Exit(-1);
            }

            LoggerManager _logger = new(_inputArguments.DArgs["WorkDir"] + "\\Log");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Входные данные проверенные"));

//            ErrorBasa _errorBasa = new ErrorBasa(_logger);
            ErrorBasa _errorBasa = new ErrorBasa();
            Config0 _config = new();
            JsonBasa _jsonBasa = new(ref _config);
            _config.MPath = new MasPaths(_inputArguments.DArgs);

//            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Поиск каталога #COMMON "));

            var resul = _config.MPath.FormPath();
            if (resul)
            {
                var __error = ErrorBasa.FError(-4);
                __error.Wait();
            }

            SetupParam _setupParam = new(ref _config, _jsonBasa);
            var z = _setupParam.IniciaPathJson();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " - Инициализация параметров закончилась "));

            //            _logger.Dispose();
//            Thread.Sleep(6000);


            if (_inputArguments.DArgs.ContainsKey("RenameDir"))
            {
                _ = LoggerManager.AddLoggerAsync(
                    new LoggerEvent(EnumError.Info, " - Включен режим переименования clf файлов и создание DbConfig.json  "));

                var _files = new FindDirClf(_inputArguments.DArgs["RenameDir"]).Run();
                foreach (var item in _files)
                {
                    if (File.Exists(item + "\\clf.json"))
                        File.Delete(item + "\\clf.json");

                    if (File.Exists(item + "\\DbConfig.json"))
                        continue;

                    _inputArguments.DArgs["WorkDir"] = item;
                    _inputArguments.DArgs["OutputDir"] = item;

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, new []{ "  Включен режим переименования:\n "
                                                                    , $" Работаем с каталогом - {item}" }));

                    ConvertOne _convertOne = new(ref _config, _jsonBasa);
                    _convertOne.Run();
                }
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Перебрали все каталоги " ));
                _logger.Dispose();
            }
            else
            {
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

                
                if(Directory.GetDirectories(_inputArguments.DArgs["WorkDir"], "!D*").Length > 0)
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Запуск-> Обработка сырых данных "));

                    //  Class ConvertOne + Convert
                    ConvertOne _convertOne = new ConvertOne(ref _config, _jsonBasa);
                    _convertOne.Run();

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Завершение обработки"));

                }
                //  Существует каталог CLF с файлами + наличие файла файла DbConfig.json 
                //  Нет clf файлов в корневом каталоге
                else
                {
                    if (File.Exists(_inputArguments.DArgs["WorkDir"] + "\\clf.json"))
                        File.Delete(_inputArguments.DArgs["WorkDir"] + "\\clf.json");

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Режим Конвертации -> CLF -> MDF (...) "));

                    if (!File.Exists(_inputArguments.DArgs["WorkDir"] + "\\DbConfig.json"))
                    {
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " нет файла DbConfig.json создаем его "));
                        ConvertOne _convertOne = new(ref _config, _jsonBasa);
                        _convertOne.Run();
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение"));
                    }

                    if (Directory.Exists(_config.MPath.Clf)
                        && (Directory.GetFiles(_config.MPath.Clf, "*.clf").Length > 0)
                        && (Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length == 0)
                        && File.Exists(_config.MPath.WorkDir + "\\DbConfig.json"))
                    {
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Конвертируем -> CLF -> MDF (...)  "));

                        ConverExport _converExport = new ConverExport(ref _config);
                        _converExport.Run();
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение"));
                    }
                }
            }
            _logger.Dispose();

            WriteLine("Все ))");
        }

    }
}

