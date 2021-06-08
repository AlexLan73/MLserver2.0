using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.MDFRename
{
    public interface IFindDirMDF
    {

    }
    public class FindDirMDF: IFindDirMDF
    {
        #region data
        public ConcurrentDictionary<string, int> PathFiledbConfig { get; set; }
        private int _numberTasks;
        private Barrier _barrier;
        private string path;
        private readonly DateTime _dtstart;
        private readonly DateTime _dtend;
        #endregion
        public FindDirMDF(string path, ref ConcurrentDictionary<string, int> _pathFileMDF, DateTime dtstart, DateTime dtend)
        {
            PathFiledbConfig = _pathFileMDF;
            this.path = path;
            _dtstart = dtstart;
            _dtend = dtend;
        
        }

        public async Task Run()
        {
            string[] globPaths = Directory.GetDirectories(path);

            _numberTasks = globPaths.Length;
            _barrier = new Barrier(_numberTasks + 1);

            for (int i = 0; i < globPaths.Length; i++)
                ThreadPool.QueueUserWorkItem(F0, (_barrier, globPaths[i]));
            await Task.Delay(1);
            _barrier.SignalAndWait();

        }
        private void F0(object state)
        {
            //            (Barrier, string) barrier, path = state;
            (Barrier, string) idat = ((Barrier, string))state;

            ConcurrentDictionary<string, bool> _dir0 = new ConcurrentDictionary<string, bool>();
            _dir0.AddOrUpdate(idat.Item2, true, (_, _) => true);

            //            while (_dir0.Count > 0 && _dir0.Count(x => !x.Key.ToLower().Contains("dbconfig.json")) > 0)
            while (_dir0.Count > 0)
            {
                foreach (var item in _dir0.Keys)
                {
                    //                    if (!_dir0.ContainsKey(item)) continue;
                    Console.WriteLine($" FindDirClf - {item}  ");

                    _dir0.TryRemove(item, out _);
                    if (!Directory.Exists(item))
                        continue;

                    string _path = item + "\\DbConfig.json";
                    if (File.Exists(_path))
                    {

                        if (!PathFiledbConfig.ContainsKey(_path) && (Directory.Exists(item + "\\MDF")))
                        {

                            string[] _sdt0 = item.Split("\\");
                            int _countx = _sdt0.Count();
                            string _std1 = _sdt0[_countx - 1];
                            DateTime dt = DateTime.ParseExact(_std1, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);

                            if (dt >= _dtstart && dt < _dtend && Directory.GetFiles(item + "\\MDF").Length>1)
                            {
                                PathFiledbConfig.AddOrUpdate(_path, 0, (_, _) => 0);
                            }
                        }
                        continue;
                    }

                    foreach (var item0 in Directory.GetDirectories(item))
                        _dir0.AddOrUpdate(item0, true, (_, _) => true);
                }
            }

            idat.Item1.RemoveParticipant();
        }


    }
}

/*
         #region data
        public ConcurrentDictionary<string, int> PathFiledbConfig { get; set; }
        private int _numberTasks;
        private Barrier _barrier;
        private string path;
        #endregion
        public FindDirClf(string path, ConcurrentDictionary<string, int> _pathFileCLF)
        {
            PathFiledbConfig = _pathFileCLF;
            this.path = path;
        }

        public async Task Run()
        {
            string[] globPaths = Directory.GetDirectories(path);

            _numberTasks = globPaths.Length;
            _barrier = new Barrier(_numberTasks + 1);

            for (int i = 0; i < globPaths.Length; i++)
                 ThreadPool.QueueUserWorkItem(F0, (_barrier, globPaths[i]));
            await Task.Delay(1);
            _barrier.SignalAndWait();

        }
        private void F0(object state)
        {
//            (Barrier, string) barrier, path = state;
            (Barrier, string) idat = ((Barrier, string))state;

            ConcurrentDictionary<string, bool> _dir0 = new ConcurrentDictionary<string, bool>();
            _dir0.AddOrUpdate(idat.Item2, true, (_, _) => true);

//            while (_dir0.Count > 0 && _dir0.Count(x => !x.Key.ToLower().Contains("dbconfig.json")) > 0)
            while (_dir0.Count > 0 )
            {
                foreach (var item in _dir0.Keys)
                {
//                    if (!_dir0.ContainsKey(item)) continue;
                    Console.WriteLine($" FindDirClf - {item}  ");

                    _dir0.TryRemove(item, out _);
                    string _path = item + "\\DbConfig.json";
                    if (File.Exists(_path))
                    {

                        if (!PathFiledbConfig.ContainsKey(_path))
                        {
                            if(Directory.GetFiles(item+"\\CLF", "*.clf").Length>0)
                                PathFiledbConfig.AddOrUpdate(_path, 0, (_, _) => 0);
                            else
                                PathFiledbConfig.AddOrUpdate(_path, -1, (_, _) => -1);
                        }
                            
                        continue;
                    }

                    foreach (var item0 in Directory.GetDirectories(item))
                        _dir0.AddOrUpdate(item0, true, (_, _) => true);
                }
            }

            idat.Item1.RemoveParticipant();
        }



 */

/*
                         string[] _sdt0 = item.Split("\\");
                        int _countx = _sdt0.Count();
                        string _std1 = _sdt0[_countx - 1];
                        DateTime dt = DateTime.ParseExact(_std1, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture);
//                        DateTime _dataStart = new DateTime(2021, 04, 27);
//                        DateTime _dataEnd = new DateTime(2021, 06, 3, 16, 0, 0);
                        DateTime _dataStart = new DateTime(2020, 11, 2);
                        DateTime _dataEnd = new DateTime(2020, 12, 21, 16, 0, 0);

                        if (dt>=_dataStart && dt<_dataEnd)
                        {
                            if (!PathFiledbConfig.ContainsKey(_path))
                            {

 */