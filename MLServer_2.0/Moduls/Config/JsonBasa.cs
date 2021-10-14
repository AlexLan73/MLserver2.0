using Convert.Logger;
using Convert.Moduls.ClfFileType;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TypeDStringMemoryInfo1 = System.Collections.Concurrent.ConcurrentDictionary<string,
        System.Collections.Concurrent.ConcurrentDictionary<string, Convert.Moduls.ClfFileType.MemoryInfo>>;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class JsonBasa
    {
        #region data
        private readonly Config0 _config;
        private static JsonBasa _jsonBasa;
        #endregion

        #region Constructor
        public JsonBasa(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class JsonBasa"));

            this._config = config;
            _jsonBasa = this;
        }
        #endregion

        #region Func -> ADD
        public static Task AddFileMemInfo(TypeDStringMemoryInfo1 fileMemInfo)
        {
            ThreadPool.QueueUserWorkItem(_ =>
                    {
                        var d = new TypeDStringMemoryInfo1(fileMemInfo);
                        foreach (var (key, value) in d)
                            _jsonBasa._config.FileMemInfo.AddOrUpdate(key, value, (_, _) => value);
                    });
            return Task.CompletedTask;
        }
        #endregion

        #region Save File   
        public static async Task SaveFileAsync<T>(T dan, string namefile)
        {
            var json = JsonConvert.SerializeObject(dan, Formatting.Indented);
            await File.WriteAllTextAsync(namefile, json);
        }
        public static async Task SaveFileFileMemInfo()
        {
            await File.WriteAllTextAsync(_jsonBasa._config.MPath.DbConfig,
                JsonConvert.SerializeObject(_jsonBasa._config.FileMemInfo, Formatting.Indented));

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Сохранить данные в (Sawe dan in) DbConfig.json"));
        }
        #endregion

        #region Load File   
        public static T LoadFileJso<T>(string filejson)
        {
            return !File.Exists(filejson)
                ? default
                : JsonConvert.DeserializeObject<T>(File.ReadAllText(filejson));
        }

        public static void LoadFileJsoDbConfig()
        {
            if (File.Exists(_jsonBasa._config.MPath.DbConfig))
            {
                var dbConfig = LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(_jsonBasa._config.MPath.DbConfig);
                _jsonBasa._config.DbConfig = dbConfig ?? new TypeDStringMemoryInfo1();

                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Чтение данных из (Read dan from ) DbConfig.json"));
            }
            else
                _jsonBasa._config.DbConfig = new TypeDStringMemoryInfo1();
        }

        public static  ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>  LoadFileJsoDbConfig(string path)
        {
            return LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(path);
        }

        #endregion

    }
}
