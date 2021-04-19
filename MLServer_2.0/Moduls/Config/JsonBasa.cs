﻿using MLServer_2._0.Interface.Config;
using MLServer_2._0.Moduls.ClfFileType;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TypeDStringMemoryInfo1 = System.Collections.Concurrent.ConcurrentDictionary<string,
                                    System.Collections.Concurrent.ConcurrentDictionary<string,
                                        MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;



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
        }

        #endregion

        #region Load File   
        public T LoadFileJso<T>(string filejson)
        {
//            var zz = JsonConvert.DeserializeObject<T>(File.ReadAllText(filejson));
            return !File.Exists(filejson) 
                ? default 
                : JsonConvert.DeserializeObject<T>(File.ReadAllText(filejson));
        }

        //public async void LoadFileJsoDbConfig() =>
        //    ThreadPool.QueueUserWorkItem(_ =>
        //    {
        //        var dbConfig = LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(config.MPath.DbConfig);
        //        config.DbConfig = dbConfig == null
        //            ? new()
        //            : dbConfig;
        //    });
        public void LoadFileJsoDbConfig() {
            if(File.Exists(_config.MPath.DbConfig))
            {
                var dbConfig = LoadFileJso<ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>>>(_config.MPath.DbConfig);
                _config.DbConfig = dbConfig == null
                        ? new()
                        : dbConfig;
            }
            else
            {
                _config.DbConfig = new();
            }
        }
        #endregion

    }
}
