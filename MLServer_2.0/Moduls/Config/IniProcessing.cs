using Convert.Interface.Config;
using Convert.Moduls.Error;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class IniProcessing : IIniProcessing
    {
        #region Data
        public Dictionary<string, string> Data { get; set; }
        public string[] Fields { get; set; }
        protected string Filename;
        public List<string> Ldata { get; set; }
        public string Field { get; set; }
        public Config0 Config { get; set; }
        #endregion

        #region construct
        public IniProcessing(string filename, string[] fields, ref Config0 config)
        {
            Fields = fields;
            Install(filename, ref config);
            Config = config;
        }
        public IniProcessing(string filename, string field, ref Config0 config)
        {
            Field = field;
            Install(filename, ref config);
        }

        private void Install(string filename, ref Config0 config)
        {
            this.Filename = filename;
            Ldata = new List<string>();
            Data = new Dictionary<string, string>();
            Config = config;
        }
        #endregion

        #region convert
        public virtual bool Convert()
        {
            var dan = Fields.Select(item => Ldata.Find(x => x.ToLower().Contains(item))).ToList();

            foreach (var item in dan.Where(item => item != null))
                Data.Add(item.Split("=")[0].ToLower(), item.Split("=")[1]);

            if (dan.Count != Data.Count)
                _ = ErrorBasa.FError(-211);
            return false;
        }
        #endregion

        #region Read File
        public bool ReadIni()
        {
            if (!File.Exists(Filename))
            {
                var error = ErrorBasa.FError(-20, Filename);
                error.Wait();
                return true;
            }

            using (var sr = new StreamReader(Filename, System.Text.Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    Ldata.Add(line);
            }
            return false;
        }
        #endregion

    }
}
