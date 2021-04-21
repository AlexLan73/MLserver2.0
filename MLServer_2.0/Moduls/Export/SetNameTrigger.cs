using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.ClfFileType;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.FileManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TypeDStringMemoryInfo = System.Collections.Concurrent.ConcurrentDictionary<string,
        System.Collections.Concurrent.ConcurrentDictionary<string, MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;

//public ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>> DbConfig;

namespace MLServer_2._0.Moduls.Export
{
    public class SetNameTrigger
    {
        #region data
        private Config0 _config;
        private readonly ILogger _ilogger;
        public Task RunTask = null;
        public string _patternFile;
        private string _typeconvert;
        private string _ext;
        private string _pathConvert;
//        private ConcurrentDictionary<string, string> _mem1 = new ConcurrentDictionary<string, string>();
        private readonly FileMove _renameFile;
        #endregion
        public SetNameTrigger(ILogger ilogger, ref Config0 config, string typeconvert)
        {
            _config = config;
            _ilogger = ilogger;
            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)F";
            _typeconvert = typeconvert.ToUpper();
            _pathConvert = _config.MPath.OutputDir + "\\" + _typeconvert + "\\";
            _ext = "."+_config.ClexportParams[_typeconvert]["ext"];

            if (_config.FMem.Count == 0)
            {
                var _x0 = _config.DbConfig.Where(x => x.Key.ToLower().Contains("_m2_")).Select(y => y.Value);//
                foreach (var item in _x0)
                    foreach (var item1 in item)
                        _config.FMem.AddOrUpdate("M2_" + item1.Key, item1.Value, (_, _) => item1.Value);
            }

            _renameFile = new FileMove(_pathConvert, _pathConvert);
            _renameFile.Run();
        }

        private string _numTrigger(MemoryInfo meminfo)
        {
            string s = "_(x)";
            var zx= meminfo.TriggerInfo.Select(x => x.Trigger.Split(" ")[1]).ToList().Distinct().ToArray(); ;
            string ss0 = "";
            foreach (var item in zx)
                ss0 += s.Replace("x", item);
            return ss0;
        }
        //private List<string> _findFileDirClf() => Directory.GetFiles(_pathConvert, "*" + _ext)
        //        .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
        //        .Select(z => Path.GetFileName(z))
        //        .ToList();
        private List<string> _findFileDirClf()
        {
            List<string> ls = new List<string>();
            string ss = "*" + _ext;
            string path = _pathConvert;
            var x01 = Directory.GetFiles(path, ss);
            foreach (var item in x01)
            {
                if (Regex.Matches(item, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                {
                    ls.Add(Path.GetFileName(item));
                }
            }
            var x02 = x01.Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1);
            var x03 = x02.Select(z => Path.GetFileName(z)).ToList();
            return ls;
        }

        private async Task _rename_m1(List<string> ls)
        {
            await Task.Run(() =>
              {
                  var _ls = new List<string>(ls);
                  foreach (var item in _ls)
                  {
                      string _file = Path.GetFileName(item);
                      try
                      {
                          string filePatch = _pathConvert + item;
                          using (var fs = File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                          { }
                      }
                      catch (IOException )
                      {
                          continue;
                      }

                      _file = _file.ToUpper().Replace(")F", ")_F");
                    _renameFile.Add(item, _file);

                  }
              });
        }
        private async Task _rename_m2(List<string> ls)
        {
            await Task.Run(() =>
            {
                List<string> _ls = new(ls);
                foreach (var item in _ls)
                {
                    string _file = Path.GetFileName(item).ToUpper();
                    try
                    {
                        string filePatch = _pathConvert + item;
                        using (var fs = File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                        { }
                    }
                    catch (IOException)
                    {
                        continue;
                    }

                    int i = _file.IndexOf(")F");
                    var _file0 = _file.Substring(0, i+1)+"_";
                    var _file1 = _file.Substring(i + 1).Split(".");
                    string _fnum = "";
                    string _m2fmem = "M2_" + _file1[0];
                    if (_config.FMem.ContainsKey(_m2fmem))
                        _fnum = _config.FMem[_m2fmem].GetNameTrigger();
                    string s0 = _file0 + _file1[0] + _fnum +"."+ _file1[1];
                    _renameFile.Add(item, s0);

                }
            });
        }

        public async Task Run()
        {
            _config.IsRun.IsExportRename = true;

            var xxx = _findFileDirClf().Count;
            while (_config.IsRun.IsExport || _findFileDirClf().Count>0)
            {
                var ls = _findFileDirClf();
                if(ls.Count<=0)
                {
                    await Task.Delay(1000);
                    continue;
                }
                var lsM1 = ls.Where(x => x.ToLower().Contains("_m1_") && x.ToUpper().Contains(")F")).ToList();
                var lsM2 = ls.Where(x => x.ToLower().Contains("_m2_") && x.ToUpper().Contains(")F")).ToList();
                Task _waitM1 = null;
                Task _waitM2 = null;

                if (lsM1.Count > 0)
                    _waitM1 = Task.Run(() => _rename_m1(lsM1));

                if (lsM2.Count > 0)
                    _waitM2 = Task.Run(() => _rename_m2(lsM2));

                _waitM1?.Wait();
                _waitM2?.Wait();
                await Task.Delay(1000);
            }
            _config.IsRun.IsExportRename = false;
            while (_renameFile.GetCountFilesNameQueue() > 0)
                Thread.Sleep(300);

            _renameFile.AbortRepit();

        }
    }
}
