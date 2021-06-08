using Convert.Moduls.ClfFileType;
using Convert.Moduls.FileManager;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Convert.Moduls.Config;

namespace MLServer_2._0.Moduls.MDFRename
{
    public interface IRenameMDF
    {

    }
    public class RenameMDF: IRenameMDF
    {
        #region data
        private readonly string _pathDbConfig;
        private readonly string _workDir;
        //        private ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>> FMem;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, MemoryInfo>> DbConfig;
        private ConcurrentDictionary<string, MemoryInfo> FMem;
        private FileMove _renameFile;
        #endregion

        #region cunstructor
        public RenameMDF(string pathDbConfig)
        {
            _pathDbConfig = pathDbConfig;
            _workDir = _pathDbConfig.ToLower().Split("\\db")[0]+"\\MDF";
            DbConfig = JsonBasa.LoadFileJsoDbConfig(_pathDbConfig);
            FMem = new ConcurrentDictionary<string, MemoryInfo>();
        }

        #endregion

        #region Run

        public async Task Run()
        {
                var x0 = DbConfig
                    .Where(x => x.Key.ToLower()
                        .Contains("_m2_"))
                    .Select(y => y.Value);
                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        FMem.AddOrUpdate("M2_" + key, value, (_, _) => value);

                x0 = DbConfig
                    .Where(x => x.Key.ToLower()
                        .Contains("_m1_"))
                    .Select(y => y.Value);
                foreach (var item in x0)
                    foreach (var (key, value) in item)
                        FMem.AddOrUpdate("M1_" + key, value, (_, _) => value);

            _renameFile = new FileMove(_workDir, _workDir);
            _renameFile.Run();

            string[] _filesMDF = Directory.GetFiles(_workDir, "*.mdf");
            var lsM1 = _filesMDF.Where(x => x.ToLower().Contains("_m1_")).ToList();
            var lsM2 = _filesMDF.Where(x => x.ToLower().Contains("_m2_")).ToList();
            Task waitM1 = null;
            Task waitM2 = null;

            if (lsM1.Count > 0) //_ = _rename_m1(lsM1);
                waitM1 = Task.Run(() => _rename_m1(lsM1));

            if (lsM2.Count > 0) //_ = _rename_m2(lsM2);
                waitM2 = Task.Run(() => _rename_m2(lsM2));

            Task.WaitAll(waitM1, waitM2);

            return ;
        }

        private async Task _rename_m1(List<string> ls1)
        {
            await Task.Run(() =>
            {
                var ls = new List<string>(ls1);
                foreach (var item in ls)
                {
                    var file = Path.GetFileName(item);

                    try
                    {
                        var filePatch = _workDir + "\\" + file;
                        using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                        { }
                    }
                    catch (IOException)
                    {
                        continue;
                    }

                    var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                    string _m1F = "M1_" + Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                    string _newFile = "";

                    if (FMem.ContainsKey(_m1F))
                        _newFile = file0[0] + "_M1_" + FMem[_m1F].StartEndMem + Path.GetExtension(file);
                    else
                        continue;

                    _renameFile.Add(file, _newFile);

                }
            });
        }
        private async Task _rename_m2(IReadOnlyCollection<string> ls1)
        {
            await Task.Run(() =>
            {

                List<string> ls = new(ls1);
                foreach (var item in ls)
                {
                    var file = Path.GetFileName(item).ToUpper();

                    try
                    {
                        var filePatch = _workDir + "\\" + file;
                        using (File.Open(filePatch, FileMode.Open, FileAccess.Read, FileShare.None))
                        { }
                    }
                    catch (IOException)
                    {
                        continue;
                    }

                    var file0 = Path.GetFileNameWithoutExtension(file).Split("_", 3);
                    string m2Fmem = "M2_" + Regex.Match(file0[2], @"F\d{3,5}", RegexOptions.IgnoreCase).Value;
                    string _newFile = "";

                    if (FMem.ContainsKey(m2Fmem))
                        _newFile = file0[0] + "_M2_" + FMem[m2Fmem].StartEndMem;
                    else
                        continue;

                    var fnum = "";
                    if (FMem.ContainsKey(m2Fmem))
                        fnum = FMem[m2Fmem].GetNameTrigger();

                    _newFile = _newFile + fnum + Path.GetExtension(file);
                    _renameFile.Add(file, _newFile);
                }
            });
        }
        #endregion
    }
}
