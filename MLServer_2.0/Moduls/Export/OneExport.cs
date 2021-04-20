using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace MLServer_2._0.Moduls.Export
{
    public class OneExport:IDisposable
    {
        #region data
        private readonly ILogger _iLogger;
        private Config0 _config;
        private readonly string _patternFile;
//        private ConcurrentDictionary<string, bool> _dirClfRun;
        private ConcurrentDictionary<string, Task> _dirClfRun;
        private readonly string _typeExport;
        private readonly string _commandExport;
        private readonly string _outDir;
        private readonly int _timeComp;
        private  DateTime _startDateTime;
        private int _timeWait;
        private List<string> _newFiles;
        public int ErrorRun { get; private set; } = 0;
        public Task TaskRun { get; set; } = null;
        private SetNameTrigger _setNameTrigger;
        private Task _waitNameTrigger=null;
        private string _ext;
        #endregion

        #region Constructor
        public OneExport(ILogger ilogger, ref Config0 config, (string, string, string) typeExport)
        {
            _iLogger = ilogger;
            _config = config;
            _typeExport = typeExport.Item1;
            _commandExport = typeExport.Item2;
            _ext = typeExport.Item3;
            _outDir = _config.MPath.OutputDir + "\\" + _typeExport;

            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\).clf";
//            _dirClfRun = new ConcurrentDictionary<string, bool>();
            _dirClfRun = new ConcurrentDictionary<string, Task>();
            _config.Time1Sec += _config_Time1Sec;
            _timeComp = 60;                             // о
            _startDateTime = DateTime.Now;
            _newFiles = new List<string>();

           

        }

        private void _config_Time1Sec(object sender, EventArgs e)
        {
           _timeWait = (int) (DateTime.Now - _startDateTime).TotalSeconds;
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
        private int _countSourseFiles()=> Directory.GetDirectories(_config.MPath.WorkDir, "!D*")
                        .Select(item => ((string, int)) new(item, Directory.GetFiles(item, "D?F*.").Length))
                        .ToList()
                        .Sum(x => x.Item2);


        private int _runTestStartProcess()
        {
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
            bool _isTestRunSourse = true;
            while (_timeWait <120)
            {
                // 1.1.
                if (_findFileDirClf().Count > 0)
                    return 1;
                // 1.2. и 1.3.
                if (_newFileWorkDir().Count > 0 && (_config.IsRun.IsSource || _config.IsRun.IsRename))
                        _startDateTime = DateTime.Now;      // Процесс работает и пока таймер не запускаем
                else
                {   //  1.4.
                    if (_countSourseFiles() <= 0)
                         return -1;     //  1.5.

                    //  1.6.
                    if ((_config.IsRun.IsSource || _config.IsRun.IsRename) && _isTestRunSourse)
                    { 
                        _startDateTime = DateTime.Now;
                        _isTestRunSourse = false;
                    }
                }
             }
            //  сход по времени
            return -1;
        }
//        public int Run(object obj)
        public void Run()
        {
            TaskRun = Task.Run(async ()=> 
            {
                _config.IsRun.IsExport = false;

                switch (_runTestStartProcess())
                {
                    case 0:
                        Console.WriteLine("Error  - 0");
                        ErrorRun = 0;
                        return;

                    case < 0:
                        Console.WriteLine("Error  -1");
                        ErrorRun = -1;
                        return;

                    case > 0:
                        Console.WriteLine("Все нормально! ");
                        break;

                    default:
                        ErrorRun = 0;
                        return;
                }

                _config.IsRun.IsExport = true;

                _setNameTrigger = new SetNameTrigger(_iLogger, ref _config, _ext);
                _waitNameTrigger = _setNameTrigger.Run();

                _startDateTime = DateTime.Now;
                //            bool _isTestRunSourse = true;

                while (_timeWait < 120)
                {
                    List<string> _newFiles = newFileDirCLF();
                    if (_newFiles.Count() > 0)
                    {//  Есть новые файлы
                        _startConvert(_newFiles);
                        //                    await Task.Delay(250);
                        _startDateTime = DateTime.Now;

                        continue;
                    }
                    await Task.Delay(250);
                    if(compareFilesClf()<=0)
                    {
                        int _countWorkClf = _newFileWorkDir().Count;
                        //  Есть данные уйти на повторение
                        if (_countWorkClf > 0 && _config.IsRun.IsRename)
                            continue;

                        //                    if ((!(_config.IsRun.IsSource || _config.IsRun.IsRename)) && _countWorkClf==0)
                        //  процессы не работают файлов нет
                        //                        return 1;

                        if (_countWorkClf == 0 && !_config.IsRun.IsSource && !_config.IsRun.IsRename)
                            // Новых файдов нет CLF в корневом нет
                            //  _config.IsRun.IsSource, _config.IsRun.IsRename - отработали
                            break;

                        if (_countSourseFiles() <= 0 && _countWorkClf == 0 && _config.IsRun.IsRename)
                            _startDateTime = DateTime.Now;
                    }
                }

                foreach (var item in _dirClfRun)
                {
                    item.Value.Wait();
                }

                _config.IsRun.IsExport = false;

  
                _waitNameTrigger?.Wait();
            });
        }

        private void _startConvert(List<string> newFiles)
        {
            _newFiles = new List<string>(newFiles);
            foreach (var item in newFiles)
            {
                if (_dirClfRun.ContainsKey(item))
                    continue;

                Task _tast = Task.Factory.StartNew((object info1) => 
                {
                    Directory.SetCurrentDirectory(_config.MPath.Mlserver);
                    var dir_ = Directory.GetCurrentDirectory();

                    string _file = _config.MPath.Clf + "\\" + (string)info1;
                    string _maska = _commandExport.Replace("file_clf", _file);
                    _maska = _maska.Replace("my_dir", _outDir);
                    RunCLexport _runCLexport = new RunCLexport(_config.MPath.CLexport, _maska, "");
                    _runCLexport.Run();
                }, item);
//                var xx = ThreadPool.QueueUserWorkItem((object info) => 
//                {                //Mlserver
//                }, item);

                _dirClfRun.AddOrUpdate(item, _tast, (_, _) => _tast);
            }
        }

        private int compareFilesClf()
        {
            var Countfiles = Directory.GetFiles(_config.MPath.Clf, "*.clf")
                        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(z => Path.GetFileName(z))
                        .ToList().Count;
            var _x = _dirClfRun.Count();

            return Countfiles > _x ? 1 : Countfiles == _x ? 0 : -1;
        }
        private List<string> newFileDirCLF()
        {
            var files = Directory.GetFiles(_config.MPath.Clf, "*.clf")
                        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                        .Select(z => Path.GetFileName(z))
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

/*
             //            _outDir = _config.MPath.OutputDir;

            //            _lTypePath = new ConcurrentDictionary<string, string>();
            //            foreach (var item in _config.ClexportParams)
            //                _lTypePath.AddOrUpdate(item.Key, _outDir + "\\" + item.Key, (_, _) => _outDir + "\\" + item.Key);
 

            while (true)
            {
                var xx = newFileDirCLF();
                foreach (var item in xx)
                {
                    if(!_dirClfRun.ContainsKey(item))
                        _dirClfRun.AddOrUpdate(item, true, (_,_)=> true );
                }
                var xx1 = newFileWorkDir().Count();
            }
            //            copy_siglog();

            if (_config.IsRun.IsSource)
            { // преобразует из сырых данных в clf

            }
            else
            { // Или закончил преобразовать или данные уже дыли.

                var filesClfWork = Directory.GetFiles(_config.MPath.WorkDir, "*.clf").ToArray();

                if (1 > 0) //files.Count()
                {
                    //"PS18SED_M1_(2020-09-02_04-33-34)_(2020-09-02_05-07-01).clf"
                    //                    string pattern = @"_M\d_(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d)_(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d).clf";
//                    foreach (var item in files)
//                    {
//                        var xxx = item;
////                        string nameS = Path.GetFileName(item);
//                    }

//                    int k = 1;

//                    foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
//                        Console.WriteLine("{0} (duplicates '{1}') at position {2}",
//                                          match.Value, match.Groups[1].Value, match.Index);

                }
                else
                {

                }

            }

 

 */