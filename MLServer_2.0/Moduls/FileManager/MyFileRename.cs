using MLServer_2._0.Logger;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.FileManager
{
    public class MyFileRename : AFileSystemBasa<TypeDanFromFile1>
    {
        public MyFileRename(int repit, int comparesec)
        {
            FilesNameQueue = new System.Collections.Concurrent.ConcurrentQueue<TypeDanFromFile1>();
            _repit = repit;
            _compareSec = comparesec;
            ctTokenRepitExit = tokenRepitExit.Token;

            Task.Factory.StartNew(TestQueue);
        }
        public void Add(string namefile0, string namefile1)
        {
            TypeDanFromFile1 dan = new TypeDanFromFile1(namefile0, namefile1, _repit, _compareSec);

            FilesNameQueue.Enqueue(dan);
        }
        public async void TestQueue()
        {
            bool _isRun = true;
            string _sWrite = "";
            while (_isRun)
            {
                var xx = FilesNameQueue.ToList().Where(x => x.Count > 3 | x.SecWait > 60).Select(x => (x.NameFile1, x.Count, x.SecWait));
                _sWrite = $" count {FilesNameQueue.Count}    --------------------------------------------------";
                Console.WriteLine(_sWrite);
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + _sWrite));

                foreach (var item in xx)
                {
                    _sWrite = $" path-> {item.NameFile1}  Count {item.Count} , SecWait {item.SecWait}";
                    Console.WriteLine(_sWrite);
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + _sWrite));
                }
                Thread.Sleep(500);
                Console.WriteLine("  цикл TestQueue ожидаем ");
                try
                {
                    if (ctTokenRepitExit.IsCancellationRequested)
                        ctTokenRepitExit.ThrowIfCancellationRequested();
                }
                catch (Exception)
                {
                    break;
                }
            }
        }
        public override void CallBackQueue(TypeDanFromFile1 dan)
        {
            dan.CalcSecDateTime();
            if (dan.IsRun)
                return;
            FilesNameQueue.Enqueue(dan);
        }
        public async void Run()
        {
            MyTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    TypeDanFromFile1 _value;
                    FilesNameQueue.TryDequeue(out _value);
                    if (_value != null)
                    {
                        //RunCommand(() => { File.Move(_value.NameFile0, _value.NameFile1); }, _value);
                    }
                    Thread.Sleep(300);
                    Console.WriteLine(" ===!=!=!=!=!=!=!=========   Ожидаем  ");
                    try
                    {
                        if (ctTokenRepitExit.IsCancellationRequested) ctTokenRepitExit.ThrowIfCancellationRequested();
                    }
                    catch (Exception) { break; }

                }
            });
        }
    }
}
