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
        #region data
        private bool _isRun;
        private bool _isExitPrigram;
        private readonly string _filename;
        private readonly ConcurrentQueue<LoggerEvent> _cq = new();
        private readonly ConcurrentQueue<string> _strListWrite = new();

        private readonly CancellationTokenSource _tokenWriteAsync = new();
        private readonly CancellationTokenSource _tokenReadLogger = new();

        private readonly CancellationToken _ctWriteAsync;
        private readonly CancellationToken _ctReadLogger;
        private static LoggerManager _loggerManager;

        public (Task, Action)[] CurrentProcess = new (Task, Action)[2];
        #endregion

        #region constructor
        public LoggerManager(string filename)
        {
            if (!Directory.Exists(filename))
                Directory.CreateDirectory(filename);

            _filename = filename + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";

            _ctWriteAsync = _tokenWriteAsync.Token;
            _ctReadLogger = _tokenWriteAsync.Token;
            _isRun = true;
            _isExitPrigram = false;

            CurrentProcess[0] = (new Task(ReadLoggerInfo, _tokenReadLogger.Token), AbortReadLogger);
            CurrentProcess[1] = (new Task(action: () =>{_ = ProcessWriteAsync();}, _tokenWriteAsync.Token), AbortWriteAsync);
            CurrentProcess[0].Item1.Start();
            CurrentProcess[1].Item1.Start();

            _loggerManager = this;
            _ = AddLoggerAsync(new LoggerEvent(EnumError.Info, "Start programm convert " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
        }
        #endregion

        #region Exit function
        public void SetExitProgrammAsync() => _isExitPrigram = true;
        public void SetRun(bool t) => _isRun = t;
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
        #endregion

        #region Add data
        public static Task AddLoggerAsync(LoggerEvent e)
        {
            _loggerManager._cq.Enqueue(e);
            return Task.CompletedTask;
        }
        #endregion

        #region Procecc 
        public async Task ProcessWriteAsync()
        {
            _ctWriteAsync.ThrowIfCancellationRequested();
            while (_isRun || !_strListWrite.IsEmpty)
            {
                var text = "";
                while (true)
                {
                    _strListWrite.TryDequeue(out var st);
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

                    await Task.Delay(550, _ctWriteAsync);
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
                    Task.Delay(550, _ctWriteAsync).Wait(_ctWriteAsync);
            }
        }

        private async Task WriteTextAsync(string filePath, string  text)
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

                    s = dan.StringDan.Aggregate(s, (current, item) => current + item);

                    switch (dan.EnumLogger)
                    {
                        case EnumLogger.Monitor:
                            Console.WriteLine(s);
                            break;
                        case EnumLogger.File:
                            _strListWrite.Enqueue(s);
                            break;
                        case EnumLogger.MonitorFile:
                        {
                            s = dan.StringDan.Aggregate(s, (current, item) => current + item);
                            Console.WriteLine(s);
                            _strListWrite.Enqueue(s);
                            break;
                        }
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
            }
            Console.WriteLine(" !!!!  больше не ждем  ");
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, " Logger save !!!!  больше не ждем  "));
        }
        #endregion

    }
}

#region Singleton пример
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

#endregion

