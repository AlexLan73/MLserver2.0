using Convert.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class Analysis
    {
        public string RezDirAnalis { get; set; }
        private Config0 _config;

        private readonly string[] _dirs = new[] { "\\VEHICLE_CFG\\Analysis\\", "\\VEHICLE_CFG\\Analysis\\Archive\\" };

        public Analysis(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class Analysis"));
            _config = config;
            RezDirAnalis = "";
        }
        public string Convert()
        {
            var filename = _config.Fields["filename"].Split(".ltl")[0];

            var filenameOld = filename.Split("_#")[0];

            var compilationtimestamp = _config.Fields.ContainsKey("compilationtimestamp")
                                                        ? _config.Fields["compilationtimestamp"]
                                                        : "";

            var pathBasa = _config.MPath.Common + "\\Configuration\\" + filenameOld;

            if (compilationtimestamp != "")
                RezDirAnalis = FindDirectAnalis(pathBasa, _dirs, filename, compilationtimestamp);

            if (RezDirAnalis == "")
            {
                if (Directory.Exists(pathBasa))
                {
                    var allfiles = Directory.GetFiles(pathBasa, "*.analysis.zip");
                    if (allfiles.Length > 0)
                        RezDirAnalis = _findDirectAnalis(allfiles[0].Split(".zip")[0]);

                }
            }

            if (RezDirAnalis == "") return "";

            _ = LoggerManager.AddLoggerAsync
                (new LoggerEvent(EnumError.Info, $"Конфигурацию берем из {RezDirAnalis}", EnumLogger.Monitor));

            return RezDirAnalis;
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

        private string FindDirectAnalis(string pathBasa, IEnumerable<string> dirs, string filename, string compilationtimestamp)
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
