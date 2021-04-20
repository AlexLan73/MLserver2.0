using System;
using System.IO;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.FileManager;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MLServer_2._0.Moduls.Error;

namespace MLServer_2._0.Moduls.ClfFileType
{
    public class ClfFileInfo : ExeFileInfo
    {
        #region data
        private readonly ILogger _ilogger;
        private readonly Config0 _config;
        private readonly string _filename;
        private readonly string _nameCar;
        public Task<bool> ClfInfoTask;

        private ConcurrentDictionary<string, MemoryInfo> _memInfo;
        private readonly FileMove _renameFile;
        public string NumMemory { get; private set; }
        public string FileName {get; set;}
        public bool IsError { get; set; }
        public long FileSize { get; set; }
        public DateTime FileDate { get; set; }


        #endregion
        public ClfFileInfo(string filename, ref FileMove renameFile, ILogger logger, ref Config0 config) 
                             : base(config.MPath.FileType, filename, "")
        {
            IsError = false;
            _config = config;
            _nameCar = _config.Fields["carname"];
            _ilogger = logger;
            _filename = filename;
            _renameFile = renameFile;
            FileInfo fileInf = new(_filename);
            FileDate = fileInf.CreationTime;
            FileSize = fileInf.Length;
            ClfInfoTask = Task<bool>.Factory.StartNew(Run);
        }

        private bool Run()
        {
            var result = ExeInfo();

            Console.WriteLine($"  Код завершения программы {result.CodeError}  ");
            if (result.CodeError != 0)
            {
                Console.WriteLine(" !!!  Бардак!! ");
            }

//            SResulT0 sResulT0;
            var nameCarx = Lines.Find(x => x.Contains("Car: ["));
            if (nameCarx==null || nameCarx.Length<=0)
            {
                _ = ErrorBasa.FError(-32);
                return false;

//                sResulT0 = new SResulT0(-32, "нет строки '$Car: [' в   FileType ", "  ClfFileInfo");
//                Task.Run(() => _ilogger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.MonitorFile)));
//                return new ResultTd<bool, SResulT0>(sResulT0);
            }
            var x0 = Lines.Find(x => x.Contains("Contents:"));
            var i0 = Lines.IndexOf(x0);
            var x1 = Lines.Find(x => x.Contains("--- File Comment"));
            var i1 = Lines.IndexOf(x1);
            var count = i1 - i0;
            var z0 = new string[count];
            if (count <= 1)
            {
                File.Delete(_filename);
                _ = ErrorBasa.FError(-34, _filename);
                return false;

//                sResulT0 = new SResulT0(-34, $"нет информации о Memory: в файле {_filename}  ", "  ClfFileInfo");
//                Task.Run(() => _ilogger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Warning, sResulT0, EnumLogger.MonitorFile)));
//                return new ResultTd<bool, SResulT0>(sResulT0);

            }
            Lines.CopyTo(i0, z0, 0, count);

            Func<string, string> formatDanFrom = (s) =>
            {
                var i = s.Length;
                s = i switch
                {
                    26 => s,
                    19 => s + ".00000000",
                    _ => s + "00000000"
                };
                return s.Substring(0, 26);
            };

            
            if (z0.Length <= 0)
            {
                IsError = true;
                _ = ErrorBasa.FError(-31);
                return false;

//                sResulT0 = new SResulT0(-31, "не правильно отработала программа FileType ", "  ClfFileInfo");
//                Task.Run(() => _ilogger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.MonitorFile)));
//                return new ResultTd<bool, SResulT0>(sResulT0);
            }

            List<string> danAll = new(z0);

            Func<List<string>, string, List<DateTime>> FParser = (all, s) => 
            { 
                return  all.FindAll(x => x.Contains(s))
                .Select(x => x.Split(s)[1]
                .Trim())
                .Select(z => DateTime.ParseExact(formatDanFrom(z), "dd.MM.yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture))
                .ToList();
            };

            var memory = danAll.FindAll(x => x.Contains("Memory F"))
                .Select(x => x.Split(" :")[0]
                .Split("Memory")[1]
                .Trim())
                .ToList();

            var trigger = FParser(danAll, "Trigger :");
            NumMemory = trigger.Count == 0 ? "_M1_" : "_M2_";

            var start = FParser(danAll, "Start:");
            var end = FParser(danAll, "End :");

            var newFileName = _nameCar + NumMemory
                                       + start[0].ToString("(yyyy-MM-dd_HH-mm-ss)") + "_"
                                       + end[^1].ToString("(yyyy-MM-dd_HH-mm-ss)")+".clf";

            _renameFile.Add(Path.GetFileName(Path.GetFileName(_filename)), newFileName);

            _memInfo = new ConcurrentDictionary<string, MemoryInfo>();
            for (var i = 0; i < memory.Count; i++)
            {
                MemoryInfo memoryInfo;
                var trigger0 = trigger.Where(x => x >= start[i] && x < end[i]).ToList();
                if (trigger0.Count > 0)
                {
                    List<DanTriggerTime> triggerInfo = new();

                    foreach (var allTrigger in trigger0.Select(item => _config.DateTimeTrigger
                        .FindAll(x => x.DateTime
                            .ToString("dd.MM.yyyy HH:mm:ss") == item.ToString("dd.MM.yyyy HH:mm:ss"))))
                    {
                        triggerInfo.AddRange(allTrigger);
                    }
                    memoryInfo = new MemoryInfo(memory[i], start[i], end[i], triggerInfo);
                }
                else
                    memoryInfo = new MemoryInfo(memory[i], start[i], end[i]);

                _memInfo.AddOrUpdate(memory[i], memoryInfo, (_, _) => memoryInfo);

                string fMem =( newFileName.ToLower().Contains("_m2_")? "M2_" : "M1_") + memoryInfo.FMemory;

                _config.FMem.AddOrUpdate(fMem, memoryInfo, (_, _) => memoryInfo);
            }

            _config.FileMemInfo.AddOrUpdate(newFileName, _memInfo, (_, _) => _memInfo);

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-7, _filename);
            return false;

//            sResulT0 = new SResulT0(-7, $" Проблема с чтением информации из {_filename}  ", " Ошибка в обработке -> ClfFileInfo ");
//            Task.Run(() => _ilogger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.Monitor)));
//            return new ResultTd<bool, SResulT0>(sResulT0);
        }

        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            Console.WriteLine(line);
            Lines.Add(line);
        }


    }
}


/*
             var _trigger = _danAll.FindAll(x => x.Contains("Trigger :"))
                .Select(x => x.Split("Trigger :")[1]
                .Trim())
                .Select(z => DateTime.ParseExact(formatDanFrom(z), "dd.MM.yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture))
                .ToList();
 

            var _start = _danAll.FindAll(x => x.Contains("Start:"))
                .Select(x => x.Split("Start:")[1]
                .Trim())
                .Select(z => DateTime.ParseExact(formatDanFrom(z), "dd.MM.yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture))
                .ToList();

            var _end = _danAll.FindAll(x => x.Contains("End :"))
                .Select(x => x.Split("End :")[1]
                .Trim())
                .Select(z => DateTime.ParseExact(formatDanFrom(z), "dd.MM.yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture))
                .ToList();


 */