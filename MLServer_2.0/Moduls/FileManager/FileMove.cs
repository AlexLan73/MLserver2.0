using Convert.Logger;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class FileMove : AFileSystemBasa<TypeDanFromFile1>
    {
        private readonly string _workDir;
        private readonly string _outputDir;

        private bool _isExitRepitFilesNameQueue;
        public FileMove(string workDir, string outputDir)
        {
            _workDir = workDir;
            _outputDir = outputDir;
            _isExitRepitFilesNameQueue = false;
            FilesNameQueue = new System.Collections.Concurrent.ConcurrentQueue<TypeDanFromFile1>();
            CtTokenRepitExit = TokenRepitExit.Token;
        }

        public void Add(string namefile0, string namefile1)
        {
            TypeDanFromFile1 dan = new TypeDanFromFile1(_workDir + "\\" + namefile0, _outputDir + "\\" + namefile1);
            FilesNameQueue.Enqueue(dan);
        }
        public override void CallBackQueue(TypeDanFromFile1 dan)
        {
            dan.CalcSecDateTime();
            dan.IsStartTest = _isExitRepitFilesNameQueue;
            if (dan.IsRun)
                return;
            FilesNameQueue.Enqueue(dan);
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
                        TypeDanFromFile1 _value;
                        FilesNameQueue.TryDequeue(out _value);

                        if (_value != null)
                        {
                            if (!File.Exists(_value.NameFile0))
                                continue;

                            //                            Console.WriteLine($" ==--> {_value.NameFile0} ");
                            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $" FileMove -> {_value.NameFile0} "));

                            try
                            {
                                if (CtTokenRepitExit.IsCancellationRequested) CtTokenRepitExit.ThrowIfCancellationRequested();
                            }
                            catch (Exception)
                            {
                                break;
                            }
                            RunCommand(() =>
                            {
                                File.Move(_value.NameFile0, _value.NameFile1, true);
                            }, _value);
                        }
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
