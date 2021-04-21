using MLServer_2._0.Logger;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace MLServer_2._0
{
    public class ConvertOne
    {
        #region data
        private Config0 _config;
        private readonly LoggerManager _logger;
        private readonly JsonBasa _jsonBasa;
        private Task<bool> _resulClrExport = null;
        private ConverExport _converExport = null;
        #endregion

        /*
                public ConvertOne(Dictionary<string, string> args)
                {
                    _logger =  new LoggerManager(args["WorkDir"] + "\\Log");

                    _config = new Config0();
                    _jsonBasa = new JsonBasa(ref _config);
                    _config.MPath = new MasPaths(args, _logger);
                    _jsonBasa.LoadFileJsoDbConfig();
                    //--------
                }
        */
        public ConvertOne(LoggerManager logger, ref Config0 config, JsonBasa jsonbasa)
        {
            _logger = logger;
            _config = config;
            _jsonBasa = jsonbasa;
        }

        public bool Run()
        {
//            var resul = _config.MPath.FormPath();
//            if (resul)
//            {
//                var __error = ErrorBasa.FError(-4);
//                __error.Wait();
//            }

//            SetupParam _setupParam = new(ref _config, _logger, _jsonBasa);
////            var _mpathTask = Task<ResultTd<bool, SResulT0>>.Factory.StartNew(_setupParam.IniciaPathJson);
//            var _mpathTask = Task<bool>.Factory.StartNew(_setupParam.IniciaPathJson);

            //Task<ResultTd<bool, SResulT0>> resulRename = new();
            ///////////////
            ///    проверка есть ли файлы *.CLF в каталоге CLF если  есть то копируем в основной каталог

            ConvertSource _convertSource;
            Task<bool> _resConvertSours = null;
            Task<bool> _resulRename = null;
            testClfMoveWorkDir();

            if (Directory.GetDirectories(_config.MPath.WorkDir, "!D*").Length > 0)
            {
                //  запускаем конвертацию сырых данных
                _convertSource = new ConvertSource(_logger, _jsonBasa, ref _config);
                _resConvertSours = _convertSource.Run();
            }
            else
            {
                if (Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length > 0)
                {
                    //  запустить переименование.
                    _resulRename = Task<bool>.Factory.StartNew(() =>{ return new RenameFileClfMoveBasa(_logger, _jsonBasa, ref _config).Run(); });
                }
            }

            RunClrExsport();
            _resConvertSours?.Wait();
            _resulRename?.Wait();
            _resulClrExport?.Wait();
            stopProcessing();
            return false;
        }

        public virtual bool RunClrExsport()
        {
//            _converExport = new ConverExport(_logger, ref _config);
//            _converExport.Run();
            return true;
        }

        private void testClfMoveWorkDir()
        {
            if (Directory.Exists(_config.MPath.Clf))
            {
                var files = Directory.GetFiles(_config.MPath.Clf);
                foreach (var item in files)
                {
                    var fileOut = _config.MPath.WorkDir + "\\" + Path.GetFileName(item);
                    File.Move(item, fileOut, true);
                }
            }
        }

        private void stopProcessing()
        {
            Task.WaitAll();
            _logger.Dispose();
            Thread.Sleep(600);

            foreach (var item in _logger.CurrentProcess
                .Where(x => x.Item1.Status != TaskStatus.Canceled))
                    item.Item2();

        }
    }
}

    


/*

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


//            var resultIniciall = configProgramm.Iniciall();
//            StopProcessing();

WriteLine("Hello World!");
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
    }
}

/*

 
 
 */