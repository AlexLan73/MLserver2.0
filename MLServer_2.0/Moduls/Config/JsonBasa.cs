using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.ClfFileType;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TypeDStringMemoryInfo1 = System.Collections.Concurrent.ConcurrentDictionary<string,
        System.Collections.Concurrent.ConcurrentDictionary<string, MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;



namespace MLServer_2._0.Moduls.Config
{
    public class JsonBasa: IJsonBasa
    {
        #region data
        private readonly Config0 _config;
        #endregion

        #region Constructor
        public JsonBasa(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class JsonBasa"));

            this._config = config;
        }
        #endregion
        #region Func -> ADD
        public async Task AddFileMemInfo(TypeDStringMemoryInfo1 fileMemInfo) => 
            ThreadPool.QueueUserWorkItem(_ =>
                    {
                        var d = new TypeDStringMemoryInfo1(fileMemInfo);
                        foreach (var (key, value) in d)
                            _config.FileMemInfo.AddOrUpdate(key, value, (_, _) => value);
                    });
        #endregion

        #region Save File   
        public async Task SaveFileAsync<T>(T dan, string namefile)
        {
            var json = JsonConvert.SerializeObject(dan, Formatting.Indented);
            await File.WriteAllTextAsync(namefile, json);
        }
        public async Task SaveFileFileMemInfo()
        {
            await File.WriteAllTextAsync(_config.MPath.DbConfig, 
                JsonConvert.SerializeObject(_config.FileMemInfo, Formatting.Indented));

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Сохранить данные в DbConfig.json"));
        }
        #endregion

        #region Load File   
        public T LoadFileJso<T>(string filejson)
        {
            return !File.Exists(filejson) 
                ? default 
                : JsonConvert.DeserializeObject<T>(File.ReadAllText(filejson));
        }

        public void LoadFileJsoDbConfig() {
            if(File.Exists(_config.MPath.DbConfig))
            {
                var dbConfig = LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(_config.MPath.DbConfig);
                _config.DbConfig = dbConfig == null
                        ? new()
                        : dbConfig;

                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Чтение данных из DbConfig.json"));
            }
            else
                _config.DbConfig = new();
        }
        #endregion

    }
}
