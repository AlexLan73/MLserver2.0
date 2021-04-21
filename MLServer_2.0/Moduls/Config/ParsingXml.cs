
using MLServer_2._0.Logger;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MLServer_2._0.Moduls.Error;
namespace MLServer_2._0.Moduls.Config
{
    public class ParsingXml
    {

        private readonly ILogger _iLoger;
        private readonly Config0 _config;
        private string[] masTega1 = { "path", "bustype", "channel", "type" };


        public ParsingXml(ILogger ilogger, ref Config0 config)
        {
            _iLoger = ilogger;
            _config = config;
            _config.VSysVarPath = "";
            _config.VSysVarType = "";
        }

        public void Convert()
        {
            var filename = _config.MPath.Analis + "\\Analysis.gla";
            if (!File.Exists(filename))
            {
                _ = ErrorBasa.FError(-26, filename);
                return;
            }

            XmlProcessing processing = new(filename, masTega1);
            processing.XmLstream.Wait();

            if (processing.Dxml != null && (processing.Dxml.Count == 0 || processing.VSysVar.Count == 0))
            {
                _ = ErrorBasa.FError(-261, filename);
                return;
            }

            filename = _config.MPath.Siglogconfig;
            IList<Dictionary<string, string>> lDxml = processing.Dxml;
            var s = "";
            foreach (var item in lDxml)
            {
                var i = lDxml.IndexOf(item) + 1;

                s += $"[DB{i}] \n";
                s += "Path=" + filename + "\\" + item["path"] + "\n";
                s += "Network=" + item["type"] + "\n";
                s += "Bus=" + item["bustype"] + "\n";
                s += "Channels=" + item["channel"] + "\n";
            }

            var s0 = "";
            using (var sr = new StreamReader(_config.MPath.Siglogconfig))
                s0 += sr.ReadToEnd();
            s0 += s;

//            Task.Run(() => _iLoger.AddLoggerInfoAsync(
///                new LoggerEvent(EnumError.Info, s0, EnumLogger.Monitor)));

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, s0, EnumLogger.Monitor));


            _config.SiglogFileInfo = s0;

            _config.VSysVarPath = processing.VSysVar != null && processing.VSysVar.ContainsKey("path") ? processing.VSysVar["path"] : "";
            _config.VSysVarType = processing.VSysVar != null && processing.VSysVar.ContainsKey("type") ? processing.VSysVar["type"] : "";
        }
    }
}