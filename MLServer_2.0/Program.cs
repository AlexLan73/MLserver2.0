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

            //            private Config0 _config;
            //            private readonly LoggerManager _logger;
            //            private readonly JsonBasa _jsonBasa;

            LoggerManager _logger = new(_inputArguments.DArgs["WorkDir"] + "\\Log");
            ErrorBasa _errorBasa = new ErrorBasa(_logger);
            Config0 _config = new();
            JsonBasa _jsonBasa = new(ref _config);
            _config.MPath = new MasPaths(_inputArguments.DArgs, _logger);
            _jsonBasa.LoadFileJsoDbConfig();

            var resul = _config.MPath.FormPath();
            if (resul)
            {
                var __error = ErrorBasa.FError(-4);
                __error.Wait();
            }

            SetupParam _setupParam = new(ref _config, _logger, _jsonBasa);
//            var _mpathTask = Task<bool>.Factory.StartNew(_setupParam.IniciaPathJson);
            var z = _setupParam.IniciaPathJson();


 //           SetNameTrigger _setNameTrigger = new SetNameTrigger(_logger, ref _config, "MDF");
//            var _wait = _setNameTrigger.Run();
//            _wait.Wait();
//            Thread.Sleep(6000);


            if (_inputArguments.DArgs.ContainsKey("RenameDir"))
            {
                var _files = new FindDirClf(_inputArguments.DArgs["RenameDir"]).Run();
                foreach (var item in _files)
                {
                    if (File.Exists(item + "\\clf.json"))
                        File.Delete(item + "\\clf.json");

                    if (File.Exists(item + "\\DbConfig.json"))
                        continue;

                    _inputArguments.DArgs["WorkDir"] = item;
                    _inputArguments.DArgs["OutputDir"] = item;

                    ConvertOne _convertOne = new(_logger, ref _config, _jsonBasa);
                    _convertOne.Run();
                }

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
                    int k = 1;
                    //  Class ConvertOne + Convert
                    ConvertOne _convertOne = new ConvertOne(_logger, ref _config, _jsonBasa);
                    _convertOne.Run();

                }
                //  Существует каталог CLF с файлами + налисие файла файла DbConfig.json 
                //  Нет clf файлов в корневом каталоге
                else
                {
                    if (File.Exists(_inputArguments.DArgs["WorkDir"] + "\\clf.json"))
                        File.Delete(_inputArguments.DArgs["WorkDir"] + "\\clf.json");

                    if (!File.Exists(_inputArguments.DArgs["WorkDir"] + "\\DbConfig.json"))
                    {
                        ConvertOne _convertOne = new(_logger, ref _config, _jsonBasa);
                        _convertOne.Run();

                    }

                    if (Directory.Exists(_config.MPath.Clf)
                        && (Directory.GetFiles(_config.MPath.Clf, "*.clf").Length > 0)
                        && (Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length == 0)
                        && File.Exists(_config.MPath.WorkDir + "\\DbConfig.json"))
                    {
                        ConverExport _converExport = new ConverExport(_logger, ref _config);
                        _converExport.Run();

                    }
                    else
                    {
                        //  стандартный запуск

                    }

                }

            }
//            var zzz = Directory.GetFiles(_inputArguments.DArgs["WorkDir"] + "\\", "*.clf").Length == 0;

            WriteLine("Все ))");
        }

    }
}

/*
  
             //            const string pathLog = @"E:\MLserver\Log";
            _logger = new LoggerManager(_inputArguments.DArgs["WorkDir"]+"\\Log");

            Task.Run(() => _logger.AddLoggerInfoAsync(
                new LoggerEvent(EnumError.Info, " ==>> #### Входные аргументы проверены  ####",
                    EnumLogger.MonitorFile)));

            JsonBasa _jsonBasa = new JsonBasa();
            SetupParam _setupParam = new(_inputArguments, _logger, _jsonBasa);
            var _mpathTask = Task<ResultTd<bool, SResulT0>>.Factory.StartNew(_setupParam.IniciaPathJson);

            _mpathTask.Wait();
            if (_mpathTask.Result)
            {
                Task.Run(() => _logger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, " ==>> #### Ошибка формирования: путей, json, ml_rt  ####", EnumLogger.MonitorFile)));
                Environment.Exit(-2);
            }

            //Task<ResultTd<bool, SResulT0>> resulRename = new();
            ///////////////
            ///    проверка есть ли файлы *.CLF в каталоге CLF если  есть то копируем в основной каталог

            ConvertSource _convertSource;
            Task<ResultTd<bool, SResulT0>> _resConvertSours = null;
            Task<ResultTd<bool, SResulT0>> _resulRename = null;
            TestClfMoveWorkDir();

 
  

            if (Directory.GetDirectories(ConfigAll.MPath.WorkDir, "!D*").Length > 0)
            {
                //  запускаем конвертацию сырых данных
                _convertSource = new ConvertSource(_logger, _jsonBasa);
                _resConvertSours =_convertSource.Run();
            }
            else
            {
                if (Directory.GetFiles(ConfigAll.MPath.WorkDir, "*.clf").Length > 0)
                {
                    //  запустить переименование.
                    _resulRename = Task<ResultTd<bool, SResulT0>>.Factory.StartNew(() => { return new RenameFileClfMoveBasa(_logger, _jsonBasa).Run(); });
//                    ConfigAll.IsRun.IsSource = false;
//                    var resulRename = Task<ResultTd<bool, SResulT0>>.Factory.StartNew(() => { return new RenameFileClfMove(_logger, _jsonBasa).Run(); });
                }
            }


            /////////////////////////////////////////////////

            Thread.Sleep(3000);

            _resConvertSours?.Wait();
            _resulRename?.Wait();
            int k1111 = 1;


        static void TestClfMoveWorkDir()
        {
            if (Directory.Exists(ConfigAll.MPath.Clf))
            {
                var files = Directory.GetFiles(ConfigAll.MPath.Clf);
                foreach (var item in files)
                {
                    var fileOut = ConfigAll.MPath.WorkDir + "\\" + Path.GetFileName(item);
                    File.Move(item, fileOut, true);
                }
            }
        }


        static void StopProcessing()
        {
            Task.WaitAll();
            _logger.Dispose();
            Thread.Sleep(600);

            foreach (var item in _logger.CurrentProcess
                .Where(x => x.Item1.Status != TaskStatus.Canceled))
                item.Item2();

        }


 */