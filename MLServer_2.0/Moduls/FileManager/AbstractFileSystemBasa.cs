using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public abstract class AFileSystemBasa<T>
    {
        public ConcurrentQueue<T> FilesNameQueue;

        protected int Repit;
        protected int CompareSec;
        public Task MyTask { get; set; }

        protected CancellationTokenSource TokenRepitExit = new();
        protected CancellationToken CtTokenRepitExit = new();
        public virtual void CallBackQueue(T dan) { }
        public virtual void RunCommand(Action myfun, T dan)
        {
            try
            {
                myfun();
            }
            catch (IOException)
            {
                CallBackQueue(dan);
            }
        }

        public void AbortRepit() => TokenRepitExit.Cancel();
        public int GetCountFilesNameQueue() => FilesNameQueue?.Count() ?? 0;
    }
}
