using Convert.Moduls.Config;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class FileDelete : AFileSystemBasa<TypeDanFromFile0>
    {
        private readonly Config0 _config;
        private readonly string _workDir;
        private bool _isExitRepitFilesNameQueue;
        public FileDelete(ref Config0 config)
        {
            _config = config;
            _workDir = _config.MPath.WorkDir;
            _isExitRepitFilesNameQueue = false;
            FilesNameQueue = new System.Collections.Concurrent.ConcurrentQueue<TypeDanFromFile0>();
            CtTokenRepitExit = TokenRepitExit.Token;
        }

        public void Add(string namefile0)
        {
            TypeDanFromFile0 dan = new TypeDanFromFile0(_workDir + "\\" + namefile0);
            FilesNameQueue.Enqueue(dan);
        }
        public override void CallBackQueue(TypeDanFromFile0 dan)
        {
            dan.CalcSecDateTime();
            dan.IsStartTest = _isExitRepitFilesNameQueue;
            if (dan.IsRun)
                return;
            FilesNameQueue.Enqueue(dan);
        }

        private void DeleteDirsSourse()
        {
            foreach (var (item1, item2) in Directory.GetDirectories(_config.MPath.WorkDir, "!D*")
                .Select(item => ((string, int))new(item, Directory.GetFiles(item, "D?F*.").Count()))
                .ToList())
            {

                if (item2 != 0) continue;
                try
                {
                    Directory.Delete(item1, true);
                }
                catch (Exception) { /*Console.WriteLine(e);*/  }
            }
        }

        public void Run()
        {
            MyTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);

                    while (GetCountFilesNameQueue() > 0)
                    {
                        TypeDanFromFile0 _value;
                        FilesNameQueue.TryDequeue(out _value);
                        if (_value != null)
                        {
                            try
                            {
                                if (CtTokenRepitExit.IsCancellationRequested)
                                    CtTokenRepitExit.ThrowIfCancellationRequested();
                            }
                            catch (Exception)
                            {
                                break;
                            }

                            RunCommand(() => { File.Delete(_value.NameFile0); }, _value);
                        }

                        DeleteDirsSourse();
                    }
                }
            });
        }
        public int GetCountFilesName() => FilesNameQueue != null ? FilesNameQueue.Count : 0;
        public void SetExitRepit()
        {
            _isExitRepitFilesNameQueue = true;
        }
    }
}

