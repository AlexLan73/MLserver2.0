using Convert.Logger;
using Convert.Moduls.Error;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class CarNameDan
    {
        public ConcurrentDictionary<string, string> BasaParams = new();
        public ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ClexportParams = new();
    }

    public class CarNameParams
    {
        public ConcurrentDictionary<string, CarNameDan> Params = new();

        public void Add(string carname, CarNameDan dan) => Params.AddOrUpdate(carname, dan, (_, _) => dan);

        public CarNameDan GetCarNameParams(string carname)
        {
            return Params.ContainsKey(carname) ? Params[carname] : new CarNameDan();
        }
    }


    public class MlServerJson
    {
        private const string CarName = "car name";
        private const string Clexport = "clexport";
        private const string Lrfdec = "lrf_dec";
        private const string Error = "error";
        private readonly Config0 _config;

        private readonly string[] _fieldes = new[] { CarName, Clexport, Error };
        private ConcurrentDictionary<string, string> BasaParams { get; set; }
        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ClexportParams { get; set; }

        private readonly CarNameParams _carParams = new();
        private List<string> _lErrorConvert;
        public MlServerJson(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Обработка файла (File processing) MlServerJson"));
            _lErrorConvert = new();
            BasaParams = new ConcurrentDictionary<string, string>();
            ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();

            _config = config;
        }

        public bool FormDanJson(string filename)
        {
            if (!File.Exists(filename))
                return true;

            var jsonString = File.ReadAllText(filename);

            JObject googleSearch = JObject.Parse(jsonString);

            var zJson = googleSearch.Children().ToList();

            var lsName = zJson.Select(item => (string)((JProperty)item).Name).ToList();

            var danJsonBasa = _fieldes.Select(item => lsName.Find(x => x.ToLower().Contains(item))).Where(z => z != null).ToList();

            foreach (var item in danJsonBasa)
                lsName.Remove(item);

            try
            {
                foreach (var item in lsName)
                    BasaParams.AddOrUpdate(item, (string)googleSearch[item], (_, _) => (string)googleSearch[item]);
            }
            catch (Exception)
            {
                return true;
            }

            if (danJsonBasa.Find(x => x.ToLower() == Clexport) != null)
                JsonClexport(googleSearch[Clexport]?.Children().ToList());

            if (danJsonBasa.Find(x => x.ToLower() == CarName) != null)
                JsonCarClexport(googleSearch["Car name"]?.Children().ToList());

            if (danJsonBasa.Find(x => x.ToLower() == Error) != null)
                JsonError(googleSearch[Error]);

            return false;
        }

        private void JsonError(IEnumerable<JToken> xx)
        {
            foreach (JValue item in xx)
                _lErrorConvert.Add((string)item.Value);
        }

        private void JsonCarClexport(IEnumerable<JToken> carxx)
        {
            foreach (var item in carxx.Children())
            {
                ConcurrentDictionary<string, ConcurrentDictionary<string, string>> clexportParams = new();
                ConcurrentDictionary<string, string> basaParams = new();

                var key0 = (string)((JProperty)item.Parent)?.Name;

                var val0 = ((JProperty)item.Parent)?.Value;

                if (val0 != null)
                    foreach (var item0 in val0.Children())
                    {
                        if (((JProperty)item0).Name == Clexport)
                        {
                            var x0 = val0[Clexport]?.Children().ToList();
                            if (x0 != null)
                                foreach (var item1 in x0)
                                {
                                    var name = ((JProperty)item1).Name;
                                    var _zz2 = (((JProperty)item1).Value).ToString();
                                    var htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(_zz2);
                                    var xx = new ConcurrentDictionary<string, string>(htmlAttributes);
                                    ClexportParams.AddOrUpdate(name, xx, (_, _) => xx);
                                }
                        }

                        if (((JProperty)item0).Name != Lrfdec) continue;

                        var x1 = val0[Lrfdec];
                        basaParams.AddOrUpdate(Lrfdec, (string)x1, (_, _) => (string)x1);
                    }

                _carParams.Add(key0, new CarNameDan()
                {
                    BasaParams = new ConcurrentDictionary<string, string>(basaParams),
                    ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(clexportParams)
                });
            }
        }

        private void JsonClexport(IReadOnlyCollection<JToken> clexportLs)
        {
            if (clexportLs.Count <= 0)
                return;

            foreach (JToken item in clexportLs)
            {
                var name = ((JProperty)item).Name;
                var zz2 = (((JProperty)item).Value).ToString();
                var htmlAttributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(zz2);
                var xx = new ConcurrentDictionary<string, string>(htmlAttributes);

                ClexportParams.AddOrUpdate(name, xx, (_, _) => xx);
            }
        }

        public void CarSetParam(string nameCar = "")
        {
            if (nameCar != "")
            {
                var paramsCar = _carParams.GetCarNameParams(nameCar);
                if (paramsCar.BasaParams.Count > 0)
                {
                    foreach (var (key, val) in paramsCar.BasaParams)
                        BasaParams.AddOrUpdate(key, val, (_, _) => val);
                }

                if (paramsCar.ClexportParams.IsEmpty) return;

                foreach (var (key, val) in paramsCar.ClexportParams)
                    ClexportParams.AddOrUpdate(key, val, (_, _) => val);

            }
            if (BasaParams.IsEmpty)
            {
                _ = ErrorBasa.FError(-272, _config.MPath.MlServerJson);
                return;
            }
            else
            {
                if (!BasaParams.ContainsKey("lrf_dec"))
                {
                    _ = ErrorBasa.FError(-272, _config.MPath.MlServerJson);
                    return;
                }
                BasaParams = new ConcurrentDictionary<string, string>(BasaParams);
            }

            if (ClexportParams.IsEmpty)
                _ = ErrorBasa.FError(-272, _config.MPath.MlServerJson);
            else
                ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(ClexportParams);
        }
        public void IniciallMLServer(string namecar = "")
        {
            if (FormDanJson(_config.MPath.MlServerJson))
            {
                var error = ErrorBasa.FError(-27, _config.MPath.MlServerJson);
                error.Wait();
                return;
            }

            CarSetParam(namecar);

            _config.BasaParams = new ConcurrentDictionary<string, string>(BasaParams);
            _config.ClexportParams = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(ClexportParams);
        }
    }
}
