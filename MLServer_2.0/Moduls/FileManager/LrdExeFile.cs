using Convert.Logger;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class LrdExeFile : ExeFileInfo
    {
        public FileDelete FileDelete;

        public LrdExeFile(string exefile, string filenamr, string command, ref Config0 config)
                                                             : base(exefile, filenamr, command)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class LrdExeFile"));

            FileDelete = new FileDelete(ref config);
        }

        public bool Run()
        {
            FileDelete.Run();

            var result = ExeInfo();
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, new[] { " LrdExeFile:\n ", $"  Код завершения программы { result.CodeError }  " }));

            if (result.CodeError != 0)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "  LrdExeFile ->  !!!  Бардак!!  "));
            }

            FileDelete.SetExitRepit();

            while (FileDelete.GetCountFilesName() > 0)
            {
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, new[] { " LrdExeFile:\n "
                                                            , $"Удаляем файлы, ожидаем завершение, осталось -> {FileDelete.GetCountFilesName()}" }));

                Thread.Sleep(1000);
            }
            FileDelete.AbortRepit();

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-5);
            return false;

        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $"  LrdExeFile ->  {line}  "));

            if (!line.ToLower().Contains("file")) return;

            try
            {
                var s0 = line.Split('\'');
                Lines.Add(s0[1]);
                FileDelete.Add(s0[1]);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
