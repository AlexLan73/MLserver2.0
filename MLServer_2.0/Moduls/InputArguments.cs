// ReSharper disable all StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using MLServer_2._0.Interface.Config;


namespace MLServer_2._0.Moduls
{
    public class InputArguments : IInputArgumentsDop
    {
        #region Data
        private List<string> _args { get; set; }
        public Dictionary<string, string> DArgs { get; set; }
        public string ExeFile { get; set; }
        public string WorkDir { get; set; }
        public string OutputDir { get; set; }

        #endregion
        public InputArguments(string[] args)
        {
            _args = new List<string>(args);
            DArgs = new Dictionary<string, string>();
        }

        public ResultTd1<bool, SResulT0> Parser()
        {
            var _z = _args.Where(x => x.Length <= 3).ToList();
            foreach (var item in _z)
                _args.Remove(item);

            Action<string, string> fdict = (s0, s1) =>
            {
                string _t = "";
                try
                {
                    _t = _args.First(x => x.ToLower().Contains(s0));
                    if (_t != null)
                    {
                        DArgs.Add(s1, _t);
                        _args.Remove(s0);
                    }
                }
                catch (Exception)
                {
                }
            };

            fdict("~test", "test");
            fdict("~!d", "~d");

            var _out = _args.FirstOrDefault(x => x.Contains("out:"));
            OutputDir = _out != null ? _out.Split("out:")[1] : WorkDir;
            if (Directory.Exists(OutputDir))
            {
                DArgs.Add("OutputDir", OutputDir);
                _args.Remove(_out);
            }

            var _rename0 = _args.FirstOrDefault(x => x.Contains("rename:"));
            string _rename = _rename0 != null ? _rename0.Split("rename:")[1].Trim() : "";
            if (_rename != "" && Directory.Exists(_rename))
            {
                DArgs.Add("RenameDir", _rename);
                _args.Remove(_rename0);
            }

            var _exe = _args.FirstOrDefault(x => x.Contains(".e1xe") && DArgs.ContainsKey("test"));
            

            DArgs.Add("ExeFile", _exe == null ? Environment.CurrentDirectory : _exe);
            if (_exe != null)
                _args.Remove(_exe);
            Console.WriteLine($"!!!!!! === {DArgs["ExeFile"]}");

            var _dir = _args.Where(x => Directory.Exists(x)).ToArray();
            if (_dir.Length == 1)
            {
                DArgs.Add("WorkDir", _dir[0]);
                if (!DArgs.ContainsKey("OutputDir"))
                    DArgs.Add("OutputDir", _dir[0]);

                return new ResultTd1<bool, SResulT0>(false);
            }
            return new ResultTd1<bool, SResulT0>(new SResulT0(-2, $"Error comand string ", "Modul InputArguments"));
        }
    }
}
