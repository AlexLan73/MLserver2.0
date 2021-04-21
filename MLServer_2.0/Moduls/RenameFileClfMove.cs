using System.IO;
using MLServer_2._0.Moduls.ClfFileType;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.FileManager;
using System;
using System.Collections.Generic;
using System.Threading;
using MLServer_2._0.Logger;

namespace MLServer_2._0.Moduls
{
    public class RenameFileClfMoveBasa
    {
        protected Config0 Config;
        public RenameFileClfMoveBasa(ref Config0 config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class RenameFileClfMoveBasa"));
            Config = config;
    }

        public virtual bool GetReturn()
        {
            return Directory.GetFiles(Config.MPath.WorkDir, "*.clf").Length <= 0;
        }

        public bool Run()
        {
            string[] nameFileClf;
            if (GetReturn())
                return false;

            Config.IsRun.IsRename = true;
            var renameFile = new FileMove(Config.MPath.WorkDir, Config.MPath.Clf);
            renameFile.Run();

            List<ClfFileInfo> taskClfInfo = new();
            Dictionary<(string, long, DateTime), bool> dFileClf = new();

            while (true)
            {
                nameFileClf = Directory.GetFiles(Config.MPath.WorkDir, "*.clf");
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
                            (string, long, DateTime) xkey = (taskClfInfo[i].FileName, taskClfInfo[i].FileSize, taskClfInfo[i].FileDate);
                            if (dFileClf.ContainsKey(xkey))
                                dFileClf.Remove(xkey, out _);

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

                            taskClfInfo.Add(new ClfFileInfo(item, ref renameFile,  ref Config));
                            dFileClf.Add(xkey, true);
                        }
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(300);
                    }
    
                }
            }

            _ = JsonBasa.AddFileMemInfo(Config.FileMemInfo);

            while (renameFile.GetCountFilesNameQueue() > 0)
                Thread.Sleep(300);

            renameFile.AbortRepit();

            Config.IsRun.IsRename =  false;
            _ = JsonBasa.SaveFileFileMemInfo();
            return false;
        }
    }

    public class RenameFileClfMove : RenameFileClfMoveBasa
    {
        public RenameFileClfMove(ref Config0 config):base(ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class RenameFileClfMove"));

        }

        public override bool GetReturn() 
        {  return!((Directory.GetFiles(Config.MPath.WorkDir, "*.clf").Length > 0) || Config.IsRun.IsSource); 
        }
    }
}
