using Convert.Logger;
using System.Collections.Generic;
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class ExeFileInfo
    {
        protected List<string> Lines = new();

        private readonly string _filenamr;
        private readonly string _command;
        private readonly string _exefile;


        public ExeFileInfo(string exefile, string filenamr, string command)
        {
            _exefile = exefile;
            _filenamr = filenamr;
            _command = command;
        }
        public virtual void CallBackFun()
        {
        }

        public virtual void CallBackFun(string line)
        {
            if (line.Length <= 0) return;
            Lines.Add(line);
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $" ExeFileInfo =->  {line} "));
        }

        public virtual InfoExe ExeInfo()
        {
            int error;
            using (var runProcess = new Process())
            {
                runProcess.StartInfo.FileName = _exefile;
                runProcess.StartInfo.Arguments = _command + " " + _filenamr;
                runProcess.StartInfo.UseShellExecute = false;
                runProcess.StartInfo.RedirectStandardOutput = true;
                runProcess.Start();
                string line;
                while ((line = runProcess.StandardOutput.ReadLine()) != null)
                {
                    CallBackFun(line);
                }
                runProcess.WaitForExit();
                error = runProcess.ExitCode;
            }
            CallBackFun();
            return new InfoExe(error, 0, false, Lines);
        }


    }
}
