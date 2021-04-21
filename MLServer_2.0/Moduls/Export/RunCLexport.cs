using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;
using MLServer_2._0.Moduls.FileManager;
using System;

namespace MLServer_2._0.Moduls.Export
{
    public class RunCLexport: ExeFileInfo
    {
        #region Data
        private readonly string _nameFile;
        #endregion
        public RunCLexport(string exefile, string filenamr, string command)
                :base(exefile, filenamr, command)
        {
            _nameFile = filenamr;
        }

        public bool Run()
        {
            var result = ExeInfo();

            string _sWrite = $"  Код завершения программы {result.CodeError}  ";
            Console.WriteLine(_sWrite);
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + _sWrite));

            if (result.CodeError != 0)
            {
                _sWrite = " !!!  Бардак!! ";
                Console.WriteLine(_sWrite);
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + _sWrite));
            }

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-8, "№-"+result.CodeError+ " "+_nameFile);
            return true;
        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            Console.WriteLine(line);
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + line));
        }

    }
}
