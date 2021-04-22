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
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using static System.Console;

namespace MLServer_2._0
{

    class Program
    {
//        static LoggerManager _logger;

        static void Main(string[] args)
        {


            var inputArguments = new InputArguments(args);
            var resultError = inputArguments.Parser();

            //////////////////////////////////////////////////////////
            // ReSharper disable once InvalidXmlDocComment
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

            LoggerManager logger = new(inputArguments.DArgs["WorkDir"] + "\\Log");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Входные данные проверенные"));

            var errorBasa = new ErrorBasa();
            Config0 config = new();
            var jsonBasa = new JsonBasa(ref config);
            config.MPath = new MasPaths(inputArguments.DArgs);

//            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Поиск каталога #COMMON "));

            var resul = config.MPath.FormPath();
            if (resul)
            {
                var error = ErrorBasa.FError(-4);
                error.Wait();
            }

            SetupParam _setupParam = new(ref config);
            _setupParam.IniciaPathJson();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " - Инициализация параметров закончилась "));

            if (inputArguments.DArgs.ContainsKey("RenameDir"))
            {
                _ = LoggerManager.AddLoggerAsync(
                    new LoggerEvent(EnumError.Info, " - Включен режим переименования clf файлов и создание DbConfig.json  "));

                var files = new FindDirClf(inputArguments.DArgs["RenameDir"]).Run();
                foreach (var item in files)
                {
                    if (File.Exists(item + "\\clf.json"))
                        File.Delete(item + "\\clf.json");

                    if (File.Exists(item + "\\DbConfig.json"))
                        continue;

                    inputArguments.DArgs["WorkDir"] = item;
                    inputArguments.DArgs["OutputDir"] = item;

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, new []{ "  Включен режим переименования:\n "
                                                                    , $" Работаем с каталогом - {item}" }));

                    ConvertOne convertOne = new(ref config);
                    convertOne.Run();
                }
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Перебрали все каталоги " ));
//                logger.Dispose();
            }
            else
            {
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


                if (Directory.GetDirectories(inputArguments.DArgs["WorkDir"], "!D*").Length > 0)
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Запуск-> Обработка сырых данных "));

                    //  Class ConvertOne + Convert
                    ConvertOne convertOne = new ConvertOne(ref config);
                    convertOne.Run();

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Завершение обработки"));

                }
                //  Существует каталог CLF с файлами + наличие файла файла DbConfig.json 
                //  Нет clf файлов в корневом каталоге
                else
                {
                    if (File.Exists(inputArguments.DArgs["WorkDir"] + "\\clf.json"))
                        File.Delete(inputArguments.DArgs["WorkDir"] + "\\clf.json");

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Режим Конвертации -> CLF -> MDF (...) "));

                    if (!File.Exists(inputArguments.DArgs["WorkDir"] + "\\DbConfig.json"))
                    {
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " нет файла DbConfig.json создаем его "));
                        ConvertOne convertOne = new(ref config);
                        convertOne.Run();
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение"));
                    }

                    if (Directory.Exists(config.MPath.Clf)
                        && (Directory.GetFiles(config.MPath.Clf, "*.clf").Length > 0)
                        && (Directory.GetFiles(config.MPath.WorkDir, "*.clf").Length == 0)
                        && File.Exists(config.MPath.WorkDir + "\\DbConfig.json"))
                    {
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Конвертируем -> CLF -> MDF (...)  "));

                        var converExport = new ConverExport(ref config);
                        converExport.Run();
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Обработка завершение"));
                    }
                }
            }
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Exit programm - " + DateTime.Now));

            logger.Dispose();

            WriteLine("Все ))");
//            Thread.Sleep(60000);
        }

    }
}


//            Thread.Sleep(6000);
