using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Convert.Logger
{
    public class LoggerManager : ILogger, IDisposable
    {
        #region data
        private bool _isExitPrigram;
        private string _filename;
        private readonly ConcurrentQueue<LoggerEvent> _cq = new();
        private readonly ConcurrentQueue<string> _strListWrite = new();

        private readonly CancellationTokenSource _tokenWriteAsync = new();
        private readonly CancellationTokenSource _tokenReadLogger = new();

        private readonly CancellationToken _ctWriteAsync;
        private readonly CancellationToken _ctReadLogger;
        private static LoggerManager _loggerManager;
        private readonly Task _readDanTask;
        private readonly Task _writeDanTask;
        #endregion

        #region constructor
        public LoggerManager(string filename)
        {
            if (!Directory.Exists(filename))
                Directory.CreateDirectory(filename);

            _filename = filename + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
//            NewNameFile(filename);
            _ctWriteAsync = _tokenWriteAsync.Token;
            _ctReadLogger = _tokenReadLogger.Token;
            _isExitPrigram = false;

            _readDanTask = Task.Run(ReadLoggerInfo, _tokenReadLogger.Token);
            _writeDanTask = Task.Run(ProcessWriteAsync, _tokenWriteAsync.Token);

            _loggerManager = this;
            _ = AddLoggerAsync(new LoggerEvent(EnumError.Info, "Start programm convert " + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
        }
        #endregion

        #region Exit function
        public void SetExitProgrammAsync() => _isExitPrigram = true;
        public void AbortReadLogger() => _tokenReadLogger.Cancel();
        public void AbortWriteAsync() => _tokenWriteAsync.Cancel();

        public void Dispose()
        {

            while (_cq.Count > 0 || _strListWrite.Count > 0)
            {
                Task.Delay(250);
            }
            SetExitProgrammAsync();
            AbortReadLogger();
            AbortWriteAsync();

            try
            {
                _readDanTask?.Wait(2500);
            }
            catch (Exception){}
            try
            {
                _writeDanTask?.Wait(2500);
            }
            catch (Exception){}
        }
        public static void DisposeStatic()
        {
            _loggerManager.Dispose();
        }
        #endregion

        #region Add data
//        public static void NewNameFile(string filename) => 
//            _loggerManager._filename = filename + "\\LOG\\" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";

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
            while (true) 
            {
                var text = "";
                while (true)
                {
                    _strListWrite.TryDequeue(out var st);
                    if (st != null)
                    {
                        text += "\n" + st;
                        if (text.Length * 2 >= 4096 * 12)
                        {
                            await WriteTextAsync(_filename, text);
                            break;
                        }
                        if (_strListWrite.Count > 0)
                            continue;
                    }

                    //                    if (!_strListWrite.IsEmpty) continue;

                    if (_isExitPrigram && _strListWrite.IsEmpty)
                    {
                        var xwait = WriteTextAsync(_filename, text);
                        xwait.Wait();
                        return;
                    }

                    try
                    {

                        if (_ctWriteAsync.IsCancellationRequested)
                        {
                            while (_strListWrite.Count > 0)
                            {
                                _strListWrite.TryDequeue(out st);
                                if (st != null)
                                {
                                    text += "\n" + st;
                                }
                            }
                            var xwait = WriteTextAsync(_filename, text);
                            xwait.Wait();
                            _ctWriteAsync.ThrowIfCancellationRequested();
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    await Task.Delay(550);
                }

                try
                {
                    if (_ctWriteAsync.IsCancellationRequested)
                    {
                        var xwait = WriteTextAsync(_filename, text);
                        xwait.Wait();
                        _ctWriteAsync.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                if (_strListWrite.IsEmpty)
                    Task.Delay(550).Wait(_ctWriteAsync);
            }
        }

        private async Task WriteTextAsync(string filePath, string text)
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
            while (true)
            {

                while (_cq.Count > 0)
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
                        return;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

                if (_isExitPrigram && _cq.IsEmpty)
                {
                    return;
                }
                Thread.Sleep(500);
            }
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

