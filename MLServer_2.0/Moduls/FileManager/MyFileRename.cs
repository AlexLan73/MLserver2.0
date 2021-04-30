using Convert.Logger;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Convert.Moduls.FileManager
{
    public class MyFileRename : AFileSystemBasa<TypeDanFromFile1>
    {
        public MyFileRename(int repit, int comparesec)
        {
            FilesNameQueue = new System.Collections.Concurrent.ConcurrentQueue<TypeDanFromFile1>();
            Repit = repit;
            CompareSec = comparesec;
            CtTokenRepitExit = TokenRepitExit.Token;

            Task.Factory.StartNew(TestQueue);
        }
        public void Add(string namefile0, string namefile1)
        {
            TypeDanFromFile1 dan = new TypeDanFromFile1(namefile0, namefile1, Repit, CompareSec);

            FilesNameQueue.Enqueue(dan);
        }
        public void TestQueue()
        {
            while (true)
            {
                var xx = FilesNameQueue.ToList().Where(x => x.Count > 3 | x.SecWait > 60).Select(x => (x.NameFile1, x.Count, x.SecWait));
                var sWrite = $" count {FilesNameQueue.Count}    --------------------------------------------------";
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + sWrite));

                foreach (var item in xx)
                {
                    sWrite = $" path-> {item.NameFile1}  Count {item.Count} , SecWait {item.SecWait}";
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + sWrite));
                }
                Thread.Sleep(500);
                Console.WriteLine("  цикл TestQueue ожидаем ");
                try
                {
                    if (CtTokenRepitExit.IsCancellationRequested)
                        CtTokenRepitExit.ThrowIfCancellationRequested();
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
        public void Run()
        {
            MyTask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    FilesNameQueue.TryDequeue(out var value);
                    if (value != null)
                    {
                        //RunCommand(() => { File.Move(_value.NameFile0, _value.NameFile1); }, _value);
                    }
                    Thread.Sleep(300);
                    Console.WriteLine(" ===!=!=!=!=!=!=!=========   Ожидаем  ");
                    try
                    {
                        if (CtTokenRepitExit.IsCancellationRequested) CtTokenRepitExit.ThrowIfCancellationRequested();
                    }
                    catch (Exception) { break; }

                }
            });
        }
    }
}
