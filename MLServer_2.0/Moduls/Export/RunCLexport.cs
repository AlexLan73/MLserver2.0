using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;
using MLServer_2._0.Moduls.FileManager;

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

            var sWrite = $"  Код завершения программы {result.CodeError}  ";
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + sWrite));

            if (result.CodeError != 0)
            {
                sWrite = " !!!  Бардак!! ";
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + sWrite));
            }

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-8, "№-"+result.CodeError+ " "+_nameFile);
            return true;
        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " RunCLexport =>  " + line));
        }

    }
}
