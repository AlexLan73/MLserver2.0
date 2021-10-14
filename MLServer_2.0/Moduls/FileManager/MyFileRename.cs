using Convert.Logger;
using MLServer_2._0;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
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
            Guid _guid = Guid.NewGuid();
            bool _isGuid = true;
            void SetFalse() => _isGuid = false;
            ThreadManager.Add(_guid, SetFalse, " MyFileRename.TestQueue() ");

            while (_isGuid)
            {
                var xx = FilesNameQueue.ToList().Where(x => x.Count > 3 | x.SecWait > 60).Select(x => (x.NameFile1, x.Count, x.SecWait));
                var sWrite = $" count {FilesNameQueue.Count}    --------------------------------------------------";
                _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + sWrite));

                foreach (var (nameFile1, count, secWait) in xx)
                {
                    sWrite = $" path-> {nameFile1}  Count {count} , SecWait {secWait}";
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " MyFileRename =>  " + sWrite));
                }
                Thread.Sleep(500);
                Console.WriteLine("  цикл TestQueue ожидаем ( TestQueue loop awaiting ) ");
                try
                {
                    if (CtTokenRepitExit.IsCancellationRequested)
                    {
                        ThreadManager.DelRecInDict(_guid);
                        CtTokenRepitExit.ThrowIfCancellationRequested(); 
                    }
                }
                catch (Exception)
                {
                    break;
                }
            }
            ThreadManager.DelRecInDict(_guid);
        }
        public override void CallBackQueue(TypeDanFromFile1 dan)
        {
            dan.CalcSecDateTime();
            if (dan.IsRun)
                return;
            FilesNameQueue.Enqueue(dan);
        }
        //public void Run()
        //{
        //    MyTask = Task.Factory.StartNew(() =>
        //    {
        //        while (true)
        //        {
        //            FilesNameQueue.TryDequeue(out var value);
        //            if (value != null)
        //            {
        //                //RunCommand(() => { File.Move(_value.NameFile0, _value.NameFile1); }, _value);
        //            }
        //            Thread.Sleep(300);
        //            Console.WriteLine(" ===!=!=!=!=!=!=!=========   Ожидаем  ");
        //            try
        //            {
        //                if (CtTokenRepitExit.IsCancellationRequested) CtTokenRepitExit.ThrowIfCancellationRequested();
        //            }
        //            catch (Exception) { break; }

        //        }
        //    });
        //}
    }
}
