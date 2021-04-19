using System.IO;
using MLServer_2._0.Moduls.ClfFileType;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.FileManager;
using System;
using System.Collections.Generic;
using System.Threading;
using MLServer_2._0.Logger;
using MLServer_2._0.Interface.Config;

namespace MLServer_2._0.Moduls
{
    public class RenameFileClfMoveBasa
    {
        private readonly ILogger _iLogger;
        private readonly IJsonBasa _jsonBasa;
        protected Config0 _config;


        public RenameFileClfMoveBasa(ILogger ilogger, IJsonBasa jsonbasa, ref Config0 config)
        {
            _iLogger = ilogger;
            _jsonBasa = jsonbasa;
            _config = config;

    }

    public virtual bool GetReturn()
        {
            return Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length <= 0;
        }

        public bool Run()
        {
            var nameFileClf = Directory.GetFiles(_config.MPath.WorkDir, "*.clf");
            if (GetReturn())
                return false;

            _config.IsRun.IsRename = true;
            var renameFile = new FileMove(_config.MPath.WorkDir, _config.MPath.Clf);
            renameFile.Run();

            List<ClfFileInfo> taskClfInfo = new();
            Dictionary<(string, long, DateTime), bool> dFileClf = new();

            while (true)
            {
                nameFileClf = Directory.GetFiles(_config.MPath.WorkDir, "*.clf");
                if (GetReturn())
                    break; 

                if(nameFileClf.Length == 0)
                    Thread.Sleep(500);

                foreach (var item in nameFileClf)
                {
                    var i = 0;
                    while (i < taskClfInfo.Count)
                    {
                        if (taskClfInfo[i].IsError)
                        {
                            (string, long, DateTime) _xkey = (taskClfInfo[i].FileName, taskClfInfo[i].FileSize, taskClfInfo[i].FileDate);
                            if (dFileClf.ContainsKey(_xkey))
                                dFileClf.Remove(_xkey, out _);

                            taskClfInfo.RemoveAt(i);
                        }
                        else
                            i += 1;
                    }
                    try
                    {
                        FileInfo fileInf = new(item);

                        (string, long, DateTime) xkey = (item, fileInf.Length, fileInf.CreationTime);

                        if (!dFileClf.ContainsKey(xkey))
                        {
                            using (File.Open(item, FileMode.Open, FileAccess.Read, FileShare.None))
                            { }

                            taskClfInfo.Add(new ClfFileInfo(item, ref renameFile, _iLogger, ref _config));
                            dFileClf.Add(xkey, true);
                        }
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(300);
                    }
                }
            }

            _jsonBasa.AddFileMemInfo(_config.FileMemInfo);

            while (renameFile.GetCountFilesNameQueue() > 0)
                Thread.Sleep(300);

            renameFile.AbortRepit();

            _config.IsRun.IsRename =  false;
            _ = _jsonBasa.SaveFileFileMemInfo();
            return false;
        }
    }

    public class RenameFileClfMove : RenameFileClfMoveBasa
    {
        public RenameFileClfMove(ILogger ilogger, IJsonBasa jsonbasa, ref Config0 config):base(ilogger, jsonbasa, ref config)
        {

        }

        public override bool GetReturn() 
        {  return!((Directory.GetFiles(_config.MPath.WorkDir, "*.clf").Length > 0) || _config.IsRun.IsSource); 
        }
    }
}
