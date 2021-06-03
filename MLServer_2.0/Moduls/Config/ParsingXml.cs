
using Convert.Logger;
using Convert.Moduls.Error;
using System.Collections.Generic;
using System.IO;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class ParsingXml
    {
        #region data
        private readonly Config0 _config;
        private readonly string[] _masTega1 = { "path", "bustype", "channel", "networkname"};
        #endregion

        #region construct
        public ParsingXml(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class ParsingXml"));

            _config = config;
            _config.VSysVarPath = "";
            _config.VSysVarType = "";
        }
        #endregion

        public void Convert()
        {
            var filename = _config.MPath.Analis + "\\Analysis.gla";
            if (!File.Exists(filename))
            {
                _ = ErrorBasa.FError(-26, filename);
                return;
            }

            XmlProcessing processing = new(filename, _masTega1);
            processing.XmLstream.Wait();

            if (processing.VSysVar != null
                && processing.Dxml != null
                && (processing.Dxml.Count == 0 || processing.VSysVar.Count == 0))
            {
                _ = ErrorBasa.FError(-261, filename);
                return;
            }


            filename = _config.MPath.Analis;
            IList<Dictionary<string, string>> lDxml = processing.Dxml;
            var s = "";
            if (lDxml != null)
                foreach (var item in lDxml)
                {
                    var i = lDxml.IndexOf(item) + 1;

                    s += $"[DB{i}] \n";
                    s += "Path=" + filename + "\\" + item["path"] + "\n";
                    s += "Network=" + item["networkname"] + "\n";
                    s += "Bus=" + item["bustype"] + "\n";
                    s += "Channels=" + item["channel"] + "\n";
                }

            var s0 = "";
            using (var sr = new StreamReader(_config.MPath.Siglogconfig))
                s0 += sr.ReadToEnd();
            s0 += s;

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, s0));

            _config.SiglogFileInfo = s0;

            _config.VSysVarPath = processing.VSysVar != null && processing.VSysVar.ContainsKey("path") ? processing.VSysVar["path"] : "";
            _config.VSysVarType = processing.VSysVar != null && processing.VSysVar.ContainsKey("type") ? processing.VSysVar["type"] : "";
        }
    }
}