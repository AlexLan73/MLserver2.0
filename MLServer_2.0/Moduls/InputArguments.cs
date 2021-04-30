﻿// ReSharper disable all StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Reflection;
using Convert.Interface.Config;


namespace Convert.Moduls
{
    public class InputArguments : IInputArgumentsDop
    {
        #region Data
        private List<string> _Args { get; set; }
        public Dictionary<string, string> DArgs { get; set; }
        public string ExeFile { get; set; }
        public string WorkDir { get; set; }
        public string OutputDir { get; set; }

        #endregion
        public InputArguments(string[] args)
        {
            _Args = new List<string>(args);
            DArgs = new Dictionary<string, string>();
        }

        public ResultTd1<bool, SResulT0> Parser()
        {
            string _exe = null;
            string _fileName = null;

            Process[] localAll = Process.GetProcesses();
            int _count = localAll.Length - 1;

            while (_fileName == null && _count >= 0)
            {
                try
                {
//                    Console.WriteLine($"  №-{_count}   {localAll[_count].MainModule.FileName}");
                    _fileName = localAll[_count].MainModule.FileName.ToLower().Contains("dll\\convert.exe")
                                    ? localAll[_count].MainModule.FileName
                                    : null;
                }
                catch (Exception)
                { }
                _count--;
            }

            if (_fileName != null)
                Console.WriteLine($"  ------->>>  {_fileName}");

            /*            Process[] localByName = Process.GetProcessesByName("Convert")
                                            .Where(x => x.MainModule.FileName.Contains("#COMMON\\DLL\\Convert.exe")).ToArray();

                        if (localByName.Length > 0)
                        {
                            Console.WriteLine($"----->    {localByName[0]}");
                            Console.WriteLine($"----->    {localByName[0].MainModule.FileName}");
                            _fileName = localByName[0].MainModule.FileName;
                        }
            */

            var _z = _Args.Where(x => x.Length <= 3).ToList();
            foreach (var item in _z)
                _Args.Remove(item);

            Action<string, string> fdict = (s0, s1) =>
            {
                string _t = "";
                try
                {
                    _t = _Args.First(x => x.ToLower().Contains(s0));
                    if (_t != null)
                    {
                        DArgs.Add(s1, _t);
                        _Args.Remove(s0);
                    }
                }
                catch (Exception)
                {
                }
            };

            fdict("~test", "test");
            fdict("~!d", "~d");

            var _out = _Args.FirstOrDefault(x => x.Contains("out:"));
            OutputDir = _out != null ? _out.Split("out:")[1] : WorkDir;
            if (Directory.Exists(OutputDir))
            {
                DArgs.Add("OutputDir", OutputDir);
                _Args.Remove(_out);
            }

            var _rename0 = _Args.FirstOrDefault(x => x.Contains("rename:"));
            string _rename = _rename0 != null ? _rename0.Split("rename:")[1].Trim() : "";
            if (_rename != "" && Directory.Exists(_rename))
            {
                DArgs.Add("RenameDir", _rename);
                _Args.Remove(_rename0);
            }

            _exe = _Args.FirstOrDefault(x => x.Contains(".exe") && DArgs.ContainsKey("test"));

            DArgs.Add("ExeFile", _exe == null ? _fileName : _exe);
            if (_exe != null)
                _Args.Remove(_exe);

            if (DArgs["ExeFile"] == null)
                return new ResultTd1<bool, SResulT0>(new SResulT0(-3, $"Error not #COMMON\\DLL\\Convert.exe ", "Modul InputArguments"));

            var _dir = _Args.Where(x => Directory.Exists(x)).ToArray();
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
