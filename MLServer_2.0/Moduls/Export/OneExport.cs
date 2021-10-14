using Convert.Logger;
using Convert.Moduls.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Export
{
    public class OneExport : IDisposable
    {
        #region data
        private Config0 _config;
        private readonly string _patternFile;
        private ConcurrentDictionary<string, Task> _dirClfRun;
        private readonly string _commandExport;
        private readonly string _outDir;
        private DateTime _startDateTime;
        private int _timeWait;
        public int ErrorRun { get; private set; }
        public Task TaskRun { get; set; }
        private SetNameTrigger _setNameTrigger;
        private Task _waitNameTrigger;
        private string _ext;
        #endregion

        #region Constructor
        public OneExport(ref Config0 config, (string, string, string) typeExport)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем ( Load ) Class OneExport"));

            _config = config;
            _commandExport = typeExport.Item2;
            _ext = typeExport.Item3;
            _outDir = _config.MPath.OutputDir + "\\" + typeExport.Item1;
            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\).clf";
            _dirClfRun = new ConcurrentDictionary<string, Task>();
            _config.Time1Sec += _config_Time1Sec;
            _startDateTime = DateTime.Now;
        }

        private void _config_Time1Sec(object sender, EventArgs e)
        {
            _timeWait = (int)(DateTime.Now - _startDateTime).TotalSeconds;
        }
        #endregion

        #region Run поток
        private List<string> _findFileDirClf() => Directory.GetFiles(_config.MPath.Clf, "*.clf")
                        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(z => Path.GetFileName(z))
                        .ToList();
        private List<string> _newFileWorkDir() => Directory.GetFiles(_config.MPath.WorkDir, "*.clf")
                        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(z => Path.GetFileName(z)).ToList();
        private int _countSourseFiles()
        {
            var directSour = Directory.GetDirectories(_config.MPath.WorkDir, "!D*");
            //  Нет директорий с сырыми данными закончилась ВСЯ обработка сырых данных
            if (!directSour.Any() && !(_config.IsRun.IsRename || _config.IsRun.IsClr || _config.IsRun.IsSource))
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, "  Нет директорий с сырыми данными закончилась ВСЯ обработка сырых данных  Error - 0 "));
                return 0; 
            }

            int _count = 0;

            try
            {
                _count = directSour.Select(item => ((string, int))new(item, Directory.GetFiles(item, "D?F*.").Length))
                        .ToList()
                        .Sum(x => x.Item2);
            }
            catch (Exception)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, "  Error in _countSourseFiles() to Repit  =>  _countSourseFiles()"));
                _countSourseFiles();
            }

            return _count;
        }

        private int _runTestStartProcess()
        {
            // ReSharper disable once InvalidXmlDocComment
            /// <summary> *** ПРОВЕРКА ПЕРЕД НАЧАЛОМ СТАРТА ПРОЦЕССА КОПИРОВАНИЯ И ПЕРЕИМЕНОВАНИЯ ***
            ///     Проверяем следующие параметры
            ///     1: Есть ли файлы в CLF
            ///         1.1. Есть - запускаем процесс
            ///         1.2. Нет -> проверяем есть ли данные clf в корневом каталоге
            ///             1.3. Есть - проверяем запущенна конвертация или переименование 
            ///                         IsRun.Sourse, _config.IsRun.IsRename  true 
            ///                          + запускаем проверку на ожидание действий доп 120 сек
            ///             1.4. Нет - Проверяем есть ли сырые данные
            ///                 1.5. - нет выходим
            ///                 1.6. - есть проверяем IsRun.Sourse=true  
            ///                                 + запускаем проверку на ожидание действий доп 120 сек 
            ///                                 переходим на пункт 1.
            /// </summary>
            _startDateTime = DateTime.Now;
            var isTestRunSourse = true;
            while (_timeWait < 120)
            {
                // 1.1.
                if (_findFileDirClf().Count > 0)
                    return 1;
                // 1.2. и 1.3.
                if (_newFileWorkDir().Count > 0 && (_config.IsRun.IsSource || _config.IsRun.IsRename))
                    _startDateTime = DateTime.Now;      // Процесс работает и пока таймер не запускаем
                else
                {   //  1.4.
                    if (_countSourseFiles() <= 0 && !(_config.IsRun.IsRename || _config.IsRun.IsClr || _config.IsRun.IsSource))
                    {
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, "  _runTestStartProcess() ->  Сход -1"));
                        return -1;     //  1.5.
                    }

                    //  1.6.
                    if ((_config.IsRun.IsSource || _config.IsRun.IsRename) && isTestRunSourse)
                    {
                        _startDateTime = DateTime.Now;
                        isTestRunSourse = false;
                    }
                }
                Task.Delay(500);
            }
            //  сход по времени
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, " _runTestStartProcess()  --  Сход по времени \n"));
            return -1;
        }
        public void Run()
        {
            TaskRun = Task.Run(async () =>
            {
                _config.IsRun.IsExport = false;

                switch (_runTestStartProcess())
                {
                    case 0:
                        //                        Console.WriteLine("Error  - 0");
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, " OneExport.Run() =>  Error - 0"));
                        ErrorRun = 0;
                        return;

                    case < 0:
                        //                        Console.WriteLine("Error  -1");
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Warning, " OneExport.Run() =>  Error - -1"));
                        ErrorRun = -1;
                        return;

                    case > 0:
                        //                        Console.WriteLine("Все нормально! ");
                        _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " OneExport.Run() =>Start  Ok "));
                        break;
                }

                _config.IsRun.IsExport = true;

                _setNameTrigger = new SetNameTrigger(ref _config, _ext);
                _waitNameTrigger = Task.Run(() => _setNameTrigger.Run());

                _startDateTime = DateTime.Now;

                while (_timeWait < 120)
                {
                    var newFileDirClf = NewFileDirClf();
                    if (newFileDirClf.Any())
                    {//  Есть новые файлы
                        _startConvert(newFileDirClf);
                        _startDateTime = DateTime.Now;
                        continue;
                    }
                    await Task.Delay(250);

                    if (CompareFilesClf() > 0) continue;

                    var countWorkClf = _newFileWorkDir().Count;
                    //  Есть данные уйти на повторение
                    if (countWorkClf > 0 && _config.IsRun.IsRename)
                        continue;
                    //  процессы не работают файлов нет
                    //                        return 1;

                    if (countWorkClf == 0 && !_config.IsRun.IsSource && !_config.IsRun.IsRename)
                        // Новых файдов нет CLF в корневом нет
                        //  _config.IsRun.IsSource, _config.IsRun.IsRename - отработали
                        break;

                    if (_countSourseFiles() <= 0 && countWorkClf == 0 && _config.IsRun.IsRename)
                        _startDateTime = DateTime.Now;
                }

                foreach (var item in _dirClfRun)
                {
                    item.Value.Wait();
                }
                _config.IsRun.IsExport = false;
                _waitNameTrigger?.Wait();
            });
        }

        private void _startConvert(IEnumerable<string> newFiles)
        {
            foreach (var item in newFiles)
            {
                if (_dirClfRun.ContainsKey(item))
                    continue;

                var tast = Task.Factory.StartNew(info1 =>
                {
                    Directory.SetCurrentDirectory(_config.MPath.Mlserver);

                    var file = _config.MPath.Clf + "\\" + (string)info1;
                    var maska = _commandExport.Replace("file_clf", file);
                    maska = maska.Replace("my_dir", _outDir);

                    var runCLexport = new RunCLexport(_config.MPath.CLexport, maska, "");
                    runCLexport.Run();
                }, item);

                _dirClfRun.AddOrUpdate(item, tast, (_, _) => tast);
            }
        }

        private int CompareFilesClf()
        {
            var countfiles = Directory.GetFiles(_config.MPath.Clf, "*.clf")
                        .Where(input => Regex.Matches(input, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(Path.GetFileName)
                        .ToList().Count;
            var x = _dirClfRun.Count();

            return countfiles > x ? 1 : countfiles == x ? 0 : -1;
        }
        private List<string> NewFileDirClf()
        {
            var files = Directory.GetFiles(_config.MPath.Clf, "*.clf")
                        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(Path.GetFileName)
                        .ToList();

            (new List<string>(_dirClfRun.Keys)).ForEach(x => files.Remove(x));

            return files;
        }
        public void Dispose()
        {

        }
        #endregion
    }
}
