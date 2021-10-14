using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.FileManager;
using MLServer_2._0;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls
{
    public class ConvertSource
    {
        #region data
        private Config0 _config;
        private ConverExport _converExport;
        private Task _converExportTask;
        #endregion
        public ConvertSource(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Создаем class ConvertSource"));
            _config = config;
            Func<string, string> dirCreate = (NameDir) =>
            {
                var s = _config.MPath.WorkDir + $"\\{NameDir}";

                if (!Directory.Exists(s))
                    Directory.CreateDirectory(s);

                return s;
            };
            if (!Directory.Exists(_config.MPath.Clf))
                Directory.CreateDirectory(_config.MPath.Clf);
        }

        protected string[] FilesSourse() => Directory.GetDirectories(_config.MPath.WorkDir, "!D*");

        private IEnumerable<(string, int)> FilesCountDirs()
        {
            return FilesSourse()
                .Select(item => ((string, int))new(item, Directory.GetFiles(item, "D?F*.").Length))
                .ToList();
        }

        protected void TestFilesNullByte(string[] direct)
        {
            if (direct == null)
                return;

            var testByte = new List<Task>();
            foreach (var item in direct)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $" ConvertSource -> {item}"));

                testByte.Add(Task.Factory.StartNew(() =>
                {
                    var files = Directory.GetFiles(item);
                    foreach (var file0 in files)
                    {
                        var file = new FileInfo(file0);
                        if (file.Length > 0) continue;
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                }));
            }

            foreach (var item in testByte)
                item.Wait();
        }
        private void DeleteDirsSourse()
        {
            foreach (var (item1, item2) in FilesCountDirs())
            {
                if (item2 != 0) continue;
                try
                {
                    Directory.Delete(item1, true);
                }
                catch (Exception) { /*Console.WriteLine(e);*/  }
            }
        }

        public  virtual Task<bool> Run()
        {
            _config.IsRun.IsSource = true;

            _converExport = new ConverExport(ref _config);

            var resultat = false;

            TestFilesNullByte(Directory.GetDirectories(_config.MPath.WorkDir, "!D*"));

            var resulRename = Task<bool>.Factory.StartNew(() => { return new RenameFileClfMove(ref _config).Run(); });

            _converExportTask = Task.Run(() => _converExport.Run());

            Func<Guid, bool> action = (_guid) =>
            {
                bool _isGuid = true;
                void SetFalse() => _isGuid = false;
                ThreadManager.Add(_guid, SetFalse, " ConvertSource ");

                while ((FilesSourse().Length > 0) && _isGuid)
                {
                    //Console.WriteLine($"  кол-во файлов  ---  FilesSourse().Count()");
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $"  кол-во файлов (number of files) ---  {FilesSourse().Count()}"));
                    resultat = new LrdExeFile(_config.MPath.LrfDec,
                                                _config.MPath.WorkDir,
                                                _config.BasaParams["lrf_dec"], ref _config).Run();
                    DeleteDirsSourse();

                }
                ThreadManager.DelRecInDict(_guid);
                return resultat;

            };

            var res = Task<bool>.Factory.StartNew(() => action(Guid.NewGuid()));

            //var res = Task<bool>.Factory.StartNew(() =>
            //{
            //    bool _is = true;
            //    void SetFalse() => _is = false;
            //    ThreadManager.AddA(_idG, SetFalse, " _o11__ " + i.ToString());

            //    while (FilesSourse().Length > 0 && _is)
            //    {
            //        //Console.WriteLine($"  кол-во файлов  ---  FilesSourse().Count()");
            //        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $"  кол-во файлов (number of files) ---  {FilesSourse().Count()}"));
            //        resultat = new LrdExeFile(_config.MPath.LrfDec,
            //                                    _config.MPath.WorkDir,
            //                                    _config.BasaParams["lrf_dec"], ref _config).Run();
            //        DeleteDirsSourse();

            //    }
            //    return resultat;
            //});

            res.Wait();
            // Console.WriteLine(" *****   ******  конвертация сырых данных завершена  ***** ");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " *****   ******  конвертация сырых данных завершена  ***** "));
            _config.IsRun.IsSource = false;
            resulRename.Wait();
            _converExportTask.Wait();

            // Console.WriteLine(" ***** ## ******  Переименование и перемецение CLF файлов завершена  ***** ");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " ***** ## ******  Переименование и перемецение CLF файлов завершена  ***** "));

            return resulRename; //.Result;
        }

    }
}

