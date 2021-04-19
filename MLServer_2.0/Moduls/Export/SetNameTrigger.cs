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


namespace MLServer_2._0.Moduls.Export
{
    public class SetNameTrigger
    {
        #region data
        private Config0 _config;
        private readonly ILogger _ilogger;
        public Task RunTask = null;
        private readonly string _patternFile;
        private readonly string _typeconvert;
        private readonly string _ext;
        private readonly string _pathConvert;
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
                foreach (var (key, value) in item)
                {
                    var s0 = _numTrigger(value);
                    var ww = s0.Length >0? "_Trigger"+ s0 : "";
                    _mem1.AddOrUpdate(key, ww, (_,_) => ww);
                }
            }
        }

        private string _numTrigger(MemoryInfo meminfo)
        {
            var s = "_(x)";
            var zx= meminfo.TriggerInfo.Select(x => x.Trigger.Split(" ")[1]).ToList().Distinct().ToArray(); ;
            return zx.Aggregate("", (current, item) => current + s.Replace("x", item));
        }
        private List<string> _findFileDirClf() => Directory.GetFiles(_pathConvert, "*"+_ext)
                .Where(x => Regex.Matches(x, _patternFile, RegexOptions.IgnoreCase).Count == 1)
                .Select(Path.GetFileName)
                .ToList();

        private async Task _rename_m1(IReadOnlyCollection<string> ls1)
        {
            await Task.Run(() =>
            {
                var ls = new List<string>(ls1);
                foreach (var t in from item in ls 
                                select Path.GetFileName(item) into file 
                                select file.ToUpper().Replace(")F", ")_F") into _file 
                                select _pathConvert + _file)
                {
                }
            });
        }
        private async Task _rename_m2(List<string> ls1)
        {
            await Task.Run(() =>
            {
                List<string> _ls = new List<string>(ls1);
                foreach (var f0 in from item in _ls 
                                    let file = Path.GetFileName(item).ToUpper() 
                                    let i = file.IndexOf(")F", StringComparison.Ordinal) 
                                    let file0 = file.Substring(0, i+1)+"_" 
                                    let file1 = file.Substring(i+1).Split(".") 
                                    let fnum = _mem1[file1[0]] 
                                    let s0 = file0 + file1[0] + fnum +"."+ file1[1] 
                                    select _pathConvert + item)
                {
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
