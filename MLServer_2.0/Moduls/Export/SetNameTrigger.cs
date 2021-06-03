using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.FileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Export
{
    public class SetNameTrigger
    {
        #region data
        private Config0 _config;
        public Task RunTask = null;
        public readonly string PatternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)F";
        private readonly string _ext;
        private readonly string _pathConvert;
        private readonly FileMove _renameFile;
        #endregion

        #region constructor
        public SetNameTrigger(ref Config0 config, string typeconvert)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class SetNameTrigger"));

            _config = config;
            var typeconvert1 = typeconvert.ToUpper();
            _pathConvert = _config.MPath.OutputDir + "\\" + typeconvert1 + "\\";
            _ext = "." + _config.ClexportParams[typeconvert1]["ext"];

            if (_config.FMem.Count == 0)
            {
                var x0 = _config.DbConfig
                    .Where(x => x.Key.ToLower()
                        .Contains("_m2_"))
                    .Select(y => y.Value);
                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        _config.FMem.AddOrUpdate("M2_" + key, value, (_, _) => value);

                x0 = _config.DbConfig
                    .Where(x => x.Key.ToLower()
                        .Contains("_m1_"))
                    .Select(y => y.Value);
                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        _config.FMem.AddOrUpdate("M1_" + key, value, (_, _) => value);

            }

            _renameFile = new FileMove(_pathConvert, _pathConvert);
            _renameFile.Run();
        }
        #endregion

        #region Public Function Run
        public async Task Run()
        {
            _config.IsRun.IsExportRename = true;

            while (_config.IsRun.IsExport || _findFileDirClf().Count > 0)
            {
                var ls = _findFileDirClf();
                if (ls.Count <= 0)
                {
                    await Task.Delay(1000);
                    continue;
                }
                var lsM1 = ls.Where(x => x.ToLower().Contains("_m1_") && x.ToUpper().Contains(")F")).ToList();
                var lsM2 = ls.Where(x => x.ToLower().Contains("_m2_") && x.ToUpper().Contains(")F")).ToList();
                Task waitM1 = null;
                Task waitM2 = null;

                if (lsM1.Count > 0)
                    waitM1 = Task.Run(() => _rename_m1(lsM1));

                if (lsM2.Count > 0)
                    waitM2 = Task.Run(() => _rename_m2(lsM2));

                waitM1?.Wait();
                waitM2?.Wait();
                await Task.Delay(1000);
            }
            _config.IsRun.IsExportRename = false;
            while (_renameFile.GetCountFilesNameQueue() > 0)
                Thread.Sleep(300);

            _renameFile.AbortRepit();

        }



        #endregion

        #region Find files
        private List<string> _findFileDirClf()
        {
            var ss = "*" + _ext;
            var path = _pathConvert;
            var x01 = Directory.GetFiles(path, ss);
            return (from item in x01
                    where Regex.Matches(item, PatternFile, RegexOptions.IgnoreCase).Count == 1
                    select Path.GetFileName(item))
                        .ToList();
        }
        private async Task _rename_m1(List<string> ls1)
        {
            await Task.Run(() =>
              {
//                  var z = _config.FMem;
                  var ls = new List<string>(ls1);
                  foreach (var item in ls)
                  {
                      var file = Path.GetFileName(item);

                      var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                      var ext = Path.GetExtension(file);
                      string _fmemory = Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                      string _m1F = "M1_" + _fmemory;
                      string _newFile = "";

                      if (_config.FMem.ContainsKey(_m1F))
                          _newFile = file0[0] + "_M1_" + _config.FMem[_m1F].StartEndMem + ext;
                      else
                          continue;

                      try
                      {
                          var filePatch = _pathConvert + item;
                          using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                          { }
                      }
                      catch (IOException)
                      {
                          continue;
                      }

//                      file = file.ToUpper().Replace(")F", ")_F");
                      _renameFile.Add(item, _newFile);

                  }
              });
        }
        private async Task _rename_m2(IReadOnlyCollection<string> ls1)
        {
            await Task.Run(() =>
            {
                var z = _config.FMem;

                List<string> ls = new(ls1);
                foreach (var item in ls)
                {
                    var file = Path.GetFileName(item).ToUpper();

                    var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                    var ext = Path.GetExtension(file);
                    string _fmemory = Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                    string _m1F = "M2_" + _fmemory;
                    string _newFile = "";

                    if (_config.FMem.ContainsKey(_m1F))
                        _newFile = file0[0] + "_M2_" + _config.FMem[_m1F].StartEndMem;
                    else
                        continue;


                    try
                    {
                        var filePatch = _pathConvert + item;
                        using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                        { }
                    }
                    catch (IOException)
                    {
                        continue;
                    }

//                    var i = file.IndexOf(")F", StringComparison.Ordinal);
//                    var file0 = file.Substring(0, i + 1) + "_";
//                    var file1 = file[(i + 1)..].Split(".");
                    var fnum = "";
                    var m2Fmem = "M2_" + _fmemory;
                    if (_config.FMem.ContainsKey(m2Fmem))
                        fnum = _config.FMem[m2Fmem].GetNameTrigger();
                    var s0 = _newFile + fnum + ext;
                    _renameFile.Add(item, s0);
                }
            });
        }
        #endregion

    }
}
