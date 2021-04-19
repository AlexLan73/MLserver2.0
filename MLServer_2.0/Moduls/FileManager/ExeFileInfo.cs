using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MLServer_2._0.Moduls.FileManager
{
    public class ExeFileInfo
    {
        protected List<string> Lines = new();

        private readonly string _filenamr;
        private readonly string _command;
        private readonly string _exefile;

        private ConcurrentDictionary<string, InfoExe> _danPool = new ConcurrentDictionary<string, InfoExe>();

        public ExeFileInfo(string exefile, string filenamr, string command)
        {
            _exefile = exefile;
            _filenamr = filenamr;
            _command = command;
        }
        public int GetDan() => _danPool.Count;

        private void ThreadProc(object stateInfo)
        {
            var z = (InfoExe)stateInfo;
            z.Id = Thread.CurrentThread.ManagedThreadId;
            var z0 = new InfoExe(z.PathNameFile, z.Id, true, new List<string>());
            _danPool.AddOrUpdate(z0.PathNameFile, z0, (_, _) => z0);
        }
        public virtual void CallBackFun()
        {

        }
        public virtual void CallBackFun(string line)
        {
            if (line.Length > 0)
            {
                Console.WriteLine(line);
                Lines.Add(line);
            }
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


        /*      
                public void Test001()
                {
                    InfoExe _info = new InfoExe("01");
                    _danPool.AddOrUpdate(_info.PathNameFile, _info, (_, _) => _info);

                    ThreadPool.QueueUserWorkItem(ThreadProc, _info);
                    Thread.Sleep(3000);
                }
        */

    }
}
