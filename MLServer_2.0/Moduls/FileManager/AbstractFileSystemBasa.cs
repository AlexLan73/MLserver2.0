using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.FileManager
{
    public abstract class AFileSystemBasa<T>    //: IDisposable
    {
        public ConcurrentQueue<T> FilesNameQueue;

        protected int _repit;
        protected int _compareSec;
        public Task MyTask { get; set; }

        protected CancellationTokenSource tokenRepitExit = new();
        protected CancellationToken ctTokenRepitExit = new();
        public virtual void CallBackQueue(T dan) { }
        public virtual void RunCommand(Action myfun, T _dan)
        {
            try
            {
                myfun();
            }
            catch (IOException e)
            {
                CallBackQueue(_dan);
            }
        }

        public void AbortRepit() => tokenRepitExit.Cancel();
        public int GetCountFilesNameQueue() => FilesNameQueue != null ? FilesNameQueue.Count() : 0;
    }
}
