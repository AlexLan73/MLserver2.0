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
        private ConcurrentDictionary<string, bool> _dirClfRun;
        private readonly string _typeExport;
        private readonly string _commandExport;
        private readonly string _outDir;
        private Barrier _barrier;
        private readonly int _timeComp;
        private  DateTime _startDateTime;
        private int _timeWait;
        #endregion

        #region Constructor
        public OneExport(ILogger ilogger, ref Config0 config, (string, string) typeExport)
        {
            _iLogger = ilogger;
            _config = config;
            _typeExport = typeExport.Item1;
            _commandExport = typeExport.Item2;
            _outDir = _config.MPath.OutputDir + "\\" + _typeExport;

            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\).clf";
            _dirClfRun = new ConcurrentDictionary<string, bool>();
            _config.Time1Sec += _config_Time1Sec;
            _timeComp = 60;                             // о
            _startDateTime = DateTime.Now;


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
        public int Run(Barrier barrier)
        {
//            _barrier = (Barrier)obj;
            _barrier = barrier;

            switch (_runTestStartProcess())
            {
                case 0:
                    Console.WriteLine("Error  - 0");

                    return 0;

                case <0:
                    Console.WriteLine("Error  -1");
                    return -1;

                case >0:
                    Console.WriteLine("Все нормально! ");
                    break;

                default:

                    return 0;
            }

            _startDateTime = DateTime.Now;
//            bool _isTestRunSourse = true;
            while (_timeWait < 120)
            {
                List<string> _newFiles = newFileDirCLF();
                if (_newFiles.Count() > 0)
                {//  Есть новые файлы
                    _startConvert(_newFiles);
                    _startDateTime = DateTime.Now;
                }
                else 
                {
                    int _countWorkClf = _newFileWorkDir().Count;
                    //  Есть данные уйти на повторение
                    if (_countWorkClf > 0 && _config.IsRun.IsRename)
                        continue;

//                    if ((!(_config.IsRun.IsSource || _config.IsRun.IsRename)) && _countWorkClf==0)
                    //  процессы не работают файлов нет
//                        return 1;

                    if(_countWorkClf==0 && !_config.IsRun.IsSource && !_config.IsRun.IsRename)
                        // Новых файдов нет CLF в корневом нет
                        //  _config.IsRun.IsSource, _config.IsRun.IsRename - отработали
                        break;

                    if(_countSourseFiles() <= 0 && _countWorkClf==0 && _config.IsRun.IsRename)
                        _startDateTime = DateTime.Now;
                }
            }
            int dd = 1;
            return 1;
        }

        private void _startConvert(List<string> newFiles)
        {
            foreach (var item in newFiles)
            {
                if (_dirClfRun.ContainsKey(item))
                    continue;
                //                ThreadPool.QueueUserWorkItem((object info) => 
                //                {
                //Mlserver
                    Directory.SetCurrentDirectory(_config.MPath.Mlserver);
                    var dir_ = Directory.GetCurrentDirectory();

                    string _file = _config.MPath.Clf + "\\" + item;
                    string _maska = _commandExport.Replace("file_clf", _file);
                    _maska = _maska.Replace("my_dir", _outDir);
//                    Console.WriteLine(_maska);
                    RunCLexport _runCLexport = new RunCLexport(_config.MPath.CLexport, _maska, "");
                    _runCLexport.Run();
  //              }, item);
                _dirClfRun.AddOrUpdate(item, true, (_, _) => true);
            }
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