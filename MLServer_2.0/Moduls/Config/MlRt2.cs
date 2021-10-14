using Convert.Logger;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;


// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class MlRt2 : IniProcessing
    {
        public ConcurrentDictionary<string, string> NameTrigger;

        public MlRt2(string filename, string[] fields, ref Config0 config)
                                    : base(filename, fields, ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Обработка файла MlRt2 \n MlRt2 file processing"));

            NameTrigger = new ConcurrentDictionary<string, string>();
        }

        public override bool Convert()
        {
            var result = ReadIni();

            if (result) return true;

            string ConvTriger(string s) => (s.Contains("Trigger"))
                                                ? "Trigger " + s.Replace("Trigger", "")
                                                : s;

            var dan = Fields.Select(item => Ldata.FindAll(x => x.ToLower().Contains(item))).ToList();

            if (dan.Count == 0)
                return true;

            foreach (var item in dan[0].Where(item => item != null))
            {
                var s1 = item.Split("=")[1];
                NameTrigger.AddOrUpdate(ConvTriger(item.Split("=")[0]), s1, (_, _) => s1);
            }

            if (dan.Count < 2)
                return false;

            var temp = new List<string>();
            for (var i = 1; i < dan.Count; i++)
                temp.AddRange(dan[i]);

            foreach (var item in temp)
            {
                for (var i = 1; i < Fields.Length; i++)
                {
                    var z0 = item.Split("=")[0].Trim().ToLower();
                    var z1 = item.Split("=")[1].Trim();

                    if (Fields[i] == z0 && Fields[i].Length == z0.Length)
                        Data.Add(z0, z1);
                }
            }

            foreach (var (key, val) in Data)
                Config.Fields.AddOrUpdate(key, val, (_, _) => val);

            Config.NameTrigger = new ConcurrentDictionary<string, string>(NameTrigger);

            return false;
        }
    }
}

