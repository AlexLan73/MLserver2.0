// ReSharper disable all StringLiteralTypo
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MLServer_2._0.Interface.Config;


namespace MLServer_2._0.Moduls
{
    public class InputArguments: IInputArgumentsDop
    {
        #region Data
        private string[] Args { get; set; }
        public Dictionary<string, string> DArgs { get; set; }
        public string ExeFile { get; set; }
        public string WorkDir { get; set; }
        public string OutputDir { get; set; }

        #endregion
        public InputArguments(string[] args)
        {
            Args = args;
            DArgs = new Dictionary<string, string>();
        }

        public ResultTd1<bool, SResulT0> Parser()
        {
            if (Args.Length < 2)
                return new ResultTd1<bool, SResulT0>(
                    new SResulT0(-1, "Мало аргументов, < 2", "Модуль InputArguments"));


            if (!File.Exists(Args[0]))
                return new ResultTd1<bool, SResulT0>(
                    new SResulT0(-2, $"Не существует файл {Args[0]}", "Модуль InputArguments"));
            DArgs.Add("ExeFile", Args[0]);
            ExeFile = Args[0];
            //_dGlobal.PathDan.AddOrUpdate("ExeFile", Args[0], (s, s1) => Args[0]);

            if (!Directory.Exists(Args[1]))
                return new ResultTd1<bool, SResulT0>(
                    new SResulT0(-3, $"Нет рабочей директории {Args[1]}", "Модуль InputArguments"));
            DArgs.Add("WorkDir", Args[1]);
            WorkDir = Args[1];
//            OutputkDir = WorkDir;
            
            var _out = Args.FirstOrDefault(x => x.Contains("out:"));
            OutputDir = _out != null ? _out.Split("out:")[1] : WorkDir;
            if (Directory.Exists(OutputDir))
                DArgs.Add("OutputDir", OutputDir);
            else
            {
                OutputDir = WorkDir;
                DArgs.Add("OutputDir", WorkDir); 
            }

            var _rename0 = Args.FirstOrDefault(x => x.Contains("rename:"));
            string _rename = _rename0 != null ? _rename0.Split("rename:")[1].Trim() : "";
            if (_rename !="" && Directory.Exists(_rename))
                DArgs.Add("RenameDir", _rename);

            Action<string, string> fdict = (s0, s1) =>
            {
                string _t = "";
                try
                {
                    _t = Args.First(x => x.ToLower().Contains(s0));
                    if (_t != null)
                        DArgs.Add(s1, _t);

                }
                catch (Exception)
                {
                }
            };

            fdict("~test", "test");
            fdict("~!d", "~d");

            return new ResultTd1<bool, SResulT0>(true);
        }
    }
}
