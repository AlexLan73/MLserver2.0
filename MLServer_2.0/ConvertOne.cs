using MLServer_2._0.Logger;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.Config;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0
{
    public class ConvertOne
    {
        #region data
        private Config0 _config;
//        private Task<bool> _resulClrExport = null;
        #endregion

        public ConvertOne(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class ConvertOne"));
            _config = config;
        }

        public bool Run()
        {
            ///////////////
            // ReSharper disable once InvalidXmlDocComment
            ///    проверка есть ли файлы *.CLF в каталоге CLF если  есть то копируем в основной каталог

            Task<bool> resConvertSours = null;
            Task<bool> resulRename = null;
            TestClfMoveWorkDir();

            if (Directory.GetDirectories(_config.MPath.WorkDir, "!D*").Length > 0)
            {
                //  запускаем конвертацию сырых данных
                var convertSource = new ConvertSource(ref _config);
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "запускаем конвертации сырых данных"));
                resConvertSours = convertSource.Run();
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "завершение конвертации сырых данных"));
            }
            else
            {
                if (Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length > 0)
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "запустить переименование CLF файлов и перенос в каталог CLF."));
                    //  запустить переименование.
                    resulRename = Task<bool>.Factory.StartNew(() =>{ return new RenameFileClfMoveBasa(ref _config).Run(); });
                }
            }

            resConvertSours?.Wait();
            resulRename?.Wait();
//            _resulClrExport?.Wait();
//            StopProcessing();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Завершение class ConvertOne.Run() "));
            return false;
        }

        private void TestClfMoveWorkDir()
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

        private  void StopProcessing()
        {
            LoggerManager.DisposeStatic();
            Thread.Sleep(600);
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
    Thread.Sleep(600);

    foreach (var item in _logger.CurrentProcess
        .Where(x => x.Item1.Status != TaskStatus.Canceled))
        item.Item2();

}
    }
}

/*

 
 
 */