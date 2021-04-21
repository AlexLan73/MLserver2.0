using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;

namespace MLServer_2._0.Moduls.Config
{
    public class Analysis
    {
        public string RezDirAnalis { get; set; }
        private Config0 _config;

        private readonly string[] _dirs = new[] { "\\VEHICLE_CFG\\Analysis\\", "\\VEHICLE_CFG\\Analysis\\Archive\\" };

//        private readonly Dictionary<string, string> _fields;
        private readonly ILogger _iLoger;
//        private readonly IMasPaths _mPaths;

        public Analysis(ConcurrentDictionary<string, string> fields, IMasPaths paths, ILogger ilogger)
        {
//            _fields = new Dictionary<string, string>(fields);
//            _iLoger = ilogger;
//            _mPaths = paths;
//            RezDirAnalis = "";
        }
        public Analysis(ILogger ilogger, ref Config0 config)
        {
            _config = config;
            _iLoger = ilogger;

  //          _fields = _config.Fields; new Dictionary<string, string>(fields);
//            _mPaths = paths;
            RezDirAnalis = "";
        }
        public string Convert()
        {
            var filename = _config.Fields["filename"].Split(".ltl")[0];

            var filenameOld = filename.Split("_#")[0];

            var compilationtimestamp = _config.Fields.ContainsKey("compilationtimestamp") ? _config.Fields["compilationtimestamp"] : "";

            string pathBasa = _config.MPath.Common + "\\Configuration\\" + filenameOld;

            if (compilationtimestamp != "")
                RezDirAnalis = FindDirectAnalis(pathBasa, _dirs, filename, compilationtimestamp);

            if (RezDirAnalis == "")
            {
                var allfiles = Directory.GetFiles(pathBasa, "*.analysis.zip");
                if (allfiles.Length > 0)
                    RezDirAnalis = _findDirectAnalis(allfiles[0].Split(".zip")[0]);
            }

            if (RezDirAnalis != "")
            {
//                Task.Run(() => _iLoger.AddLoggerInfoAsync
//                            (new LoggerEvent(EnumError.Info, $"Конфигурацию берем из {RezDirAnalis}", EnumLogger.Monitor)));

                _ = LoggerManager.AddLoggerAsync
                            (new LoggerEvent(EnumError.Info, $"Конфигурацию берем из {RezDirAnalis}", EnumLogger.Monitor));

                return RezDirAnalis;
            }
            return "";
        }

        private string _findDirectAnalis(string pathAnalis)
        {
            if (Directory.Exists(pathAnalis))
                return pathAnalis;
            else
            {
                var pathAnalisZip = pathAnalis + ".zip";

                if (!File.Exists(pathAnalisZip)) return "";

                ZipFile.ExtractToDirectory(pathAnalisZip, pathAnalis);

                if (Directory.Exists(pathAnalis))
                    return pathAnalis;
            }

            return "";
        }

        private string FindDirectAnalis(string pathBasa, string[] dirs, string filename, string compilationtimestamp)
        {
            var sdt = DateTime.ParseExact(compilationtimestamp, "dd.MM.yyyy HH:mm:ss",
                            CultureInfo.InvariantCulture).ToString("yyyy-MM-dd_HH-mm-ss");

            foreach (var item in dirs)
            {
                var pathAnalisBasa = pathBasa + item;
                var pathAnalis = pathAnalisBasa + filename + "_" + sdt + ".analysis";
                var result = _findDirectAnalis(pathAnalis);
                if (result != "")
                    return result;
            }
            return "";
        }

    }
}
