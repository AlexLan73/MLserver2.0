using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.ClfFileType;
using MLServer_2._0.Moduls.Config;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private ConcurrentDictionary<string, string> _mem1 = new ConcurrentDictionary<string, string>();

        #endregion
        public SetNameTrigger(ILogger ilogger, ref Config0 config, string typeconvert)
        {
            _config = config;
            _ilogger = ilogger;
            _patternFile = @"_M\d_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)_\(\d{4}-\d\d-\d\d_\d\d-\d\d-\d\d\)F";
            _typeconvert = typeconvert;
            _pathConvert = _config.MPath.OutputDir + "\\" + _typeconvert + "\\";
            _ext = "."+_config.ClexportParams[_typeconvert]["ext"];

            var _x0 = _config.DbConfig.Where(x => x.Key.ToLower().Contains("_m2_")).Select(y=> y.Value);//
            foreach (var item in _x0)
            {
                foreach (var item1 in item)
                {
                    string s0 = _numTrigger(item1.Value);
                    var ww = s0.Length >0? "_Trigger"+ s0 : "";
                    _mem1.AddOrUpdate(item1.Key, ww, (_,_) => ww);
                }
            }
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
        private List<string> _findFileDirClf() => Directory.GetFiles(_pathConvert, "*"+_ext)
                .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                .Select(z => Path.GetFileName(z))
                .ToList();

        private async Task _rename_m1(List<string> ls)
        {
            await Task.Run(() =>
              {
                  var _ls = new List<string>(ls);
                  foreach (var item in _ls)
                  {
                      string _file = Path.GetFileName(item);

                      _file = _file.ToUpper().Replace(")F", ")_F");

                      var t = _pathConvert + _file;
                  }
              });
        }
        private async Task _rename_m2(List<string> ls)
        {
            await Task.Run(() =>
            {
                List<string> _ls = new List<string>(ls);
                foreach (var item in _ls)
                {
                    string _file = Path.GetFileName(item).ToUpper();

                    int i = _file.IndexOf(")F");
                    var _file0 = _file.Substring(0, i+1)+"_";
                    var _file1 = _file.Substring(i+1).Split(".");
                    string _fnum = _mem1[_file1[0]];
                    string s0 = _file0 + _file1[0] + _fnum +"."+ _file1[1];
                    var f0 = _pathConvert + item;
                }
            });
        }

        public async void Run()
        {
            var ls = _findFileDirClf();
            var lsM1 = ls.Where(x => x.ToLower().Contains("_m1_") && x.ToUpper().Contains(")F")).ToList();
            var lsM2 = ls.Where(x => x.ToLower().Contains("_m2_") && x.ToUpper().Contains(")F")).ToList();
            if (lsM2.Count > 0)
            {
                var _z = Task.Run(()=> _rename_m2(lsM2)); 
                _z.Wait();

            }
            //            if (lsM1.Count > 0)
            //            {
            //                var _z = Task.Run(()=> _rename_m1(lsM1)); 
            //                _z.Wait();
            //            }
        }
    }
}
