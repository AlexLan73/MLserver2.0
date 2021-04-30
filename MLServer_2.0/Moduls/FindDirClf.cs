using Convert.Logger;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls
{
    public class FindDirClf
    {
        #region data
        private readonly int _numberTasks;
        private readonly Barrier _barrier;
        private ConcurrentDictionary<string, bool> _dir0 = new ConcurrentDictionary<string, bool>();
        #endregion
        public FindDirClf(string path)
        {
            _numberTasks = 10;
            _barrier = new Barrier(_numberTasks + 1);

            _dir0.AddOrUpdate(path, true, (_, _) => true);
        }

        public string[] Run()
        {
            for (int i = 0; i < _numberTasks; i++)
                ThreadPool.QueueUserWorkItem(F0, _barrier);

            _barrier.SignalAndWait();

            var dirx = _dir0.Keys
                                .Select(x => x.ToLower())
                                .ToArray()
                                .Where(x => Directory.GetFiles(x).Length > 0)
                                .Select(x => x.Split("\\clf")[0])
                                .ToArray()
                                .Where(x => !File.Exists(x + "\\DbConfig.json"))
                                .ToArray();
            return dirx;

        }
        private void F0(object state)
        {
            var barrier = (Barrier)state;
            while (_dir0.Count > 0 && _dir0.Count(x => !x.Key.ToLower().Contains("\\clf")) > 0)
            {
                foreach (var item in _dir0.Keys.Where(x => !x.ToLower().Contains("\\clf")))
                {
                    if (!_dir0.ContainsKey(item)) continue;

                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, $" FindDirClf - {item}  "));

                    _dir0.TryRemove(item, out _);
                    foreach (var item0 in Directory.GetDirectories(item))
                        _dir0.AddOrUpdate(item0, true, (_, _) => true);
                }
            }

            barrier.RemoveParticipant();
        }
    }
}
