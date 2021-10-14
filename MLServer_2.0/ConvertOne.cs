using Convert.Logger;
using Convert.Moduls;
using Convert.Moduls.Config;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert
{
    public class ConvertOne
    {
        #region data
        private Config0 _config;
        //        private Task<bool> _resulClrExport = null;
        #endregion

        public ConvertOne(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class ConvertOne \n Load Class ConvertOne"));
            _config = config;
            if (!Directory.Exists(_config.MPath.Clf))
                Directory.CreateDirectory(_config.MPath.Clf);
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
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "запускаем конвертации сырых данных \n start converting raw data"));
                resConvertSours = convertSource.Run();
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "запускаем конвертации сырых данных \n start converting raw data"));
            }
            else
            {
                if (Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length > 0)
                {
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "запустить переименование CLF файлов и перенос в каталог CLF. \n " +
                                                                                     "start renaming CLF files and transferring to the CLF directory."));
                    //  запустить переименование.
                    resulRename = Task<bool>.Factory.StartNew(() => { return new RenameFileClfMoveBasa(ref _config).Run(); });
                }
            }

            resConvertSours?.Wait();
            resulRename?.Wait();
            //            _resulClrExport?.Wait();
            //            StopProcessing();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Завершение class ConvertOne.Run() \n Completing class ConvertOne.Run ()"));
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
    }
}
