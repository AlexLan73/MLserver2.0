using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using MLServer_2._0.Moduls.Export;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls
{

    public class ConverExport
    {
        //class soo
        //{
        //    public soo(string name, DateTime dt)
        //    {
        //        namefile = name;
        //        this.dt = dt;
        //    }
        //    public string namefile { get; set; }
        //    public DateTime dt { get; set; }
        //}

        #region data
        private readonly ILogger _iLogger;
        private Config0 _config;
//        private string _outDir;
        private string _commandExport;
        private readonly string _patternFile;
        private ConcurrentDictionary<string, OneExport> _allRun;
        private ConcurrentDictionary<string, bool> _dirClfRun;
        private Barrier _barrier;

        #endregion
        public ConverExport(ILogger ilogger, ref Config0 config)
        {
            _iLogger = ilogger;
            _config = config;
            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\).clf";
            _dirClfRun = new ConcurrentDictionary<string, bool>();
            _allRun = new ConcurrentDictionary<string, OneExport>();


            Run();


//            _lTypePath = new ConcurrentDictionary<string, string>();
//            foreach (var item in _config.ClexportParams)
//                _lTypePath.AddOrUpdate(item.Key, _outDir + "\\" + item.Key, (_, _) => _outDir + "\\" + item.Key);
        }


        public void Run()
        {
            _barrier = new Barrier(_config.ClexportParams.Count + 1);
            foreach (var item in _config.ClexportParams)
            {
                _allRun.AddOrUpdate(item.Key, new OneExport(_iLogger, ref _config, (item.Key, item.Value))
                    , (_, _) => new OneExport(_iLogger, ref _config, (item.Key, item.Value)));

                Task.Factory.StartNew(()=> _allRun[item.Key].Run(_barrier));
            }

            _barrier.SignalAndWait();
        }
        private void copy_siglog()
        {   //  copy_siglog_vsysvar
            if (_config.VSysVarPath.ToLower().Contains(_config.VSysVarType.ToLower()))
            {
                var sourse = _config.MPath.Analis + "\\" + _config.VSysVarPath;
                try
                {
                    if (File.Exists(sourse))
                        File.Copy(sourse, _config.MPath.Mlserver + "siglog.vsysvar", true);
                }
                catch (Exception)
                {
                    _ = ErrorBasa.FError(-201, _config.MPath.Mlserver + "siglog.vsysvar");
                    return;
                }
            }
            else
            {
                _ = ErrorBasa.FError(-202, _config.MPath.Mlserver + "siglog.vsysvar");
                return;
            }

            foreach (var (key, val) in _config.ClexportParams)
            {
                string _pathConvert = _config.MPath.OutputDir + "\\" + key;
                DirectoryInfo dirInfo = new(_pathConvert);
                if (!dirInfo.Exists)
                        dirInfo.Create();
                _pathConvert += "\\siglog_config.ini";

                try
                {
                    using (StreamWriter sw = new StreamWriter(_pathConvert, false, System.Text.Encoding.Default))
                    {
                        sw.Write(_config.SiglogFileInfo);
                    }
                }
                catch (Exception)
                {
                    _ = ErrorBasa.FError(-201, _pathConvert);
                    return;
                }
            }
            return;
        }

    }
}
