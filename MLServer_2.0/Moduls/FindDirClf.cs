using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace MLServer_2._0.Moduls
{
    public class FindDirClf
    {
        #region data
        private readonly int _numberTasks;
        private readonly Barrier _barrier;
        static private ConcurrentDictionary<string, bool> _dir0 = new ConcurrentDictionary<string, bool>();
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
                ThreadPool.QueueUserWorkItem(F0,  _barrier);

            _barrier.SignalAndWait();

            //            var _dirAll = _dir0.Keys.Select(x => x.ToLower().Split("\\clf")[0]).ToArray();
            //            var _dirx = _dirAll.Where(x => !File.Exists(x + "\\DbConfig.json")).ToArray();

//            var dirAll0 = _dir0.Keys.Select(x => x.ToLower()).ToArray();
//            var dirAll1 = dirAll0.Where(x => Directory.GetFiles(x).Length > 0);
//            var dirAll = dirAll1.Select(x => x.Split("\\clf")[0]).ToArray();
//            var dirx = dirAll.Where(x => !File.Exists(x + "\\DbConfig.json")).ToArray();

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
            Barrier barrier = (Barrier)state;
            while (_dir0.Count > 0 && _dir0.Count(x => !x.Key.ToLower().Contains("\\clf")) > 0)
            {
                foreach (var item in _dir0.Keys.Where(x => !x.ToLower().Contains("\\clf")))
                {
                    if (_dir0.ContainsKey(item))
                    {
                        Console.WriteLine(item);
                        _dir0.TryRemove(item, out _);
                        foreach (var item0 in Directory.GetDirectories(item))
                            _dir0.AddOrUpdate(item0, true, (_, _) => true);
                    }
                }
            }

            barrier.RemoveParticipant();
        }
    }
}
