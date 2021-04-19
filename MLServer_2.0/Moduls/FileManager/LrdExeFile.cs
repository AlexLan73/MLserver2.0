using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Config;
using MLServer_2._0.Moduls.Error;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.FileManager
{
    public class LrdExeFile: ExeFileInfo
    {
        public FileDelete FileDelete;
        private readonly ILogger _logger;

        public LrdExeFile(string exefile, string filenamr, string command,  ILogger logger, ref Config0 config)
                     : base(exefile, filenamr, command)
        {
            FileDelete = new FileDelete(ref config);
            _logger = logger;
        }

        public bool Run()
        {
            FileDelete.Run();

            var result = ExeInfo();

            Console.WriteLine($"  Код завершения программы {result.CodeError}  ");
            if (result.CodeError != 0)
            {
                    Console.WriteLine(" !!!  Бардак!! ");
            }

            FileDelete.SetExitRepit();

            while (FileDelete.GetCountFilesName() >0)
            {
                Console.WriteLine($"Удаляем файлы, ожидаем завершение, осталось -> {FileDelete.GetCountFilesName()}");
                Thread.Sleep(1000);
            }
            FileDelete.AbortRepit();

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-5);
            return false;

//            var sResulT0 = new SResulT0(-3, $" С конвертацией в lrf_dec  ", " Ошибка в обработке первоначальных данных");
//            Task.Run(() => _logger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.Monitor)));
//            return new ResultTd<bool, SResulT0>(sResulT0);

        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;

            Console.WriteLine(line);
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
