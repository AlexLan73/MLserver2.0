using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Logger
{
    public class LoggerManager:ILogger, IDisposable
    {
        private bool _isRun;
        private bool _isExitPrigram;
        private string _filename;
        private readonly string _director;
        private readonly ConcurrentQueue<LoggerEvent> _cq = new();
        private readonly ConcurrentQueue<string> _strListWrite = new();

        private readonly CancellationTokenSource _tokenWriteAsync = new();
        private readonly CancellationTokenSource _tokenReadLogger = new();

        private readonly CancellationToken _ctWriteAsync;
        private readonly CancellationToken _ctReadLogger;
        private static LoggerManager _loggerManager = null;

        //        public Task CurrentProcessWriteAsync;
        //        public Task [] CurrentProcess = new Task[2];
        public (Task, Action)[] CurrentProcess = new (Task, Action)[2];

        public LoggerManager(string filename)
        {
            _director = filename;

            if (!Directory.Exists(_director))
                Directory.CreateDirectory(_director);

            CreateNameFile();

            _ctWriteAsync = _tokenWriteAsync.Token;
            _ctReadLogger = _tokenWriteAsync.Token;
            _isRun = true;
            _isExitPrigram = false;

            CurrentProcess[0] = (new Task(ReadLoggerInfo, _tokenReadLogger.Token), AbortReadLogger);
            CurrentProcess[1] = (new Task(action: () =>{_ = ProcessWriteAsync();}, _tokenWriteAsync.Token), AbortWriteAsync);
            CurrentProcess[0].Item1.Start();
            CurrentProcess[1].Item1.Start();

            _loggerManager = this;
            AddLoggerAsync(new LoggerEvent(EnumError.Info, "Start programm convert " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));

        }

        public void SetExitProgrammAsync() => _isExitPrigram = true;
        public void SetRun(bool t) => _isRun = t;

        private void CreateNameFile()=>
            _filename = _director + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
        

//        public void AddLoggerInfoAsync(LoggerEvent e)        {            _cq.Enqueue(e);        }

        public static async Task AddLoggerAsync(LoggerEvent e) 
        {
            _loggerManager._cq.Enqueue(e);
        }


        public async Task ProcessWriteAsync()
        {
            _ctWriteAsync.ThrowIfCancellationRequested();

            while (_isRun || !_strListWrite.IsEmpty)
            {
                var text = "";

                while (true)
                {

                    _strListWrite.TryDequeue(out string st);

                    if (st != null)
                    {
                        text += "\n" + st;
                        if (text.Length * 2 >= 4096 * 4)
                        {
                            await WriteTextAsync(_filename, text);
                            break;
                        }
                    }

                    if (!_strListWrite.IsEmpty) continue;

                    if (_isExitPrigram)
                    {
                        await WriteTextAsync(_filename, text);
                        return;                                
                    }

                    try
                    {
                        
                        if (_ctWriteAsync.IsCancellationRequested)
                        {
                            await WriteTextAsync(_filename, text);
                            _ctWriteAsync.ThrowIfCancellationRequested(); 
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    Task.Delay(550).Wait();
                }

                try
                {
                    if (_ctWriteAsync.IsCancellationRequested)
                    {
                        await WriteTextAsync(_filename, text);
                        _ctWriteAsync.ThrowIfCancellationRequested(); 
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                if (_strListWrite.IsEmpty)
                    Task.Delay(550).Wait();
            }
        }

        async Task WriteTextAsync(string filePath, string  text)
        {
            var encodedText = Encoding.Unicode.GetBytes(text);

            if (File.Exists(filePath))
            {
                await using var sourceStream =
                    new FileStream(
                        filePath, FileMode.Append, FileAccess.Write, FileShare.None, 4096, useAsync: true);
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            }
            else
            {
                await using var sourceStream =
                    new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true);
                await sourceStream.WriteAsync(encodedText.AsMemory(0, encodedText.Length));
            }
            //            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);

        }

        public void ReadLoggerInfo()
        {
            while (_isRun || !_cq.IsEmpty)
            {

                while (_cq.Count>0)
                {
                    _cq.TryDequeue(out var dan);
                    if (dan == null) continue;
                    var s = dan.DateTime.ToString("yyyy-MM-dd_HH-mm-ss.fff") + "  " + dan.EnumError + "  ";

                    foreach (var item in dan.StringDan)
                    {
                        s += item;
//                        Console.WriteLine(item);
                    }

                    if (EnumLogger.Monitor == dan.EnumLogger )
                        Console.WriteLine(s);

                    if (EnumLogger.File == dan.EnumLogger )
                        _strListWrite.Enqueue(s);

                    if (EnumLogger.MonitorFile == dan.EnumLogger)
                    {
                        foreach (var item in dan.StringDan)
                            s += item;
                        Console.WriteLine(s);
                        _strListWrite.Enqueue(s);
                    }
                }
                try
                {
                    if (_ctReadLogger.IsCancellationRequested)
                    {
                        _ctReadLogger.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                Thread.Sleep(500);
//                Console.WriteLine(" ожидание  ");
            }
            Console.WriteLine(" !!!!  больше не ждем  ");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Logger save !!!!  больше не ждем  "));
        }

        public void AbortReadLogger() => _tokenReadLogger.Cancel();
        public void AbortWriteAsync() => _tokenWriteAsync.Cancel();

        public void Dispose()
        {
            SetRun(false);
            SetExitProgrammAsync();

            foreach (var item in CurrentProcess.Where(x => x.Item1.Status != TaskStatus.Canceled))
                item.Item2();
        }
        public static void DisposeStatic()
        {
            _loggerManager.Dispose();
        }

    }
}


/*
public class Singleton
{
    private static readonly Singleton instance = new Singleton();
 
    public string Date { get; private set; }
 
    private Singleton()
    {
        Date = System.DateTime.Now.TimeOfDay.ToString();
    }
 
    public static Singleton GetInstance()
    {
        return instance;
    }
} 

(new Thread(() =>
{
    Singleton singleton1 = Singleton.GetInstance();
    Console.WriteLine(singleton1.Date);
})).Start();
 
Singleton singleton2 = Singleton.GetInstance();
Console.WriteLine(singleton2.Date);
 */