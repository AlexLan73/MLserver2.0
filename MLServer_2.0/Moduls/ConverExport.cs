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
        #region data
        private Config0 _config;
        private readonly string _patternFile;
        private ConcurrentDictionary<string, OneExport> _allRun;
        private ConcurrentDictionary<string, bool> _dirClfRun;

        #endregion
        public ConverExport(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Создаем class ConverExport"));

            //            _iLogger = ilogger;
            _config = config;
            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\).clf";
            _dirClfRun = new ConcurrentDictionary<string, bool>();
            _allRun = new ConcurrentDictionary<string, OneExport>();
        }

        public void Run()
        {

            copy_siglog();
            foreach (var item in _config.ClexportParams)
            {
                _allRun.AddOrUpdate(item.Key, new OneExport(ref _config, (item.Key, item.Value["commanda"], item.Value["ext"]))
                    , (_, _) => new OneExport(ref _config, (item.Key, item.Value["commanda"], item.Value["ext"])));

                _allRun[item.Key].Run();
            }

            foreach (var item in _allRun)
            {
                item.Value.TaskRun.Wait();
            }
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
                try
                {
                    Directory.Delete(_pathConvert, true);
                }
                catch (Exception e)
                {
                    // ignor
                }

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
