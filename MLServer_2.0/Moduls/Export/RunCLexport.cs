using MLServer_2._0.Moduls.Error;
using MLServer_2._0.Moduls.FileManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.Export
{
    public class RunCLexport: ExeFileInfo
    {
        #region Data
        private readonly string _nameFile;
        private readonly string _outDir;
        private readonly string _maska;
        #endregion
        public RunCLexport(string exefile, string filenamr, string command)
                :base(exefile, filenamr, command)
        {
            _nameFile = filenamr;
        }

        public bool Run()
        {
            var result = ExeInfo();

            Console.WriteLine($"  Код завершения программы {result.CodeError}  ");
            if (result.CodeError != 0)
            {
                Console.WriteLine(" !!!  Бардак!! ");
            }

            if (result.CodeError == 0) return false;

            _ = ErrorBasa.FError(-8, "№-"+result.CodeError+ " "+_nameFile);
            return true;
        }
        public override void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            Console.WriteLine(line);
        }

    }
}
