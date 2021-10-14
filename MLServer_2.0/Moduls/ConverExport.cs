using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using Convert.Moduls.Export;
using System;
using System.Collections.Concurrent;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls
{
    public class ConverExport
    {
        #region data
        private Config0 _config;
        private ConcurrentDictionary<string, OneExport> _allRun;

        #endregion
        public ConverExport(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Создаем (Creact) class ConverExport"));

            _config = config;

            if (!Directory.Exists(_config.MPath.Clf))
                Directory.CreateDirectory(_config.MPath.Clf);

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

            foreach (var (key, _) in _config.ClexportParams)
            {
                var pathConvert = _config.MPath.OutputDir + "\\" + key;
                try
                {
                    Directory.Delete(pathConvert, true);
                }
                catch (Exception)
                {
                    // ignor
                }

                DirectoryInfo dirInfo = new(pathConvert);
                if (!dirInfo.Exists)
                    dirInfo.Create();
                pathConvert += "\\siglog_config.ini";

                try
                {
                    using (StreamWriter sw = new StreamWriter(pathConvert, false, System.Text.Encoding.Default))
                    {
                        sw.Write(_config.SiglogFileInfo);
                    }
                }
                catch (Exception)
                {
                    _ = ErrorBasa.FError(-201, pathConvert);
                    return;
                }
            }
        }

    }
}
