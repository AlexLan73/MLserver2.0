using MLServer_2._0.Logger;
using System;
using System.IO;
using System.Linq;

namespace MLServer_2._0.Moduls.Config
{
    public class FindCommand
    {
        #region data
        private string PathBegin { get; set; }
        private const string Common = "#COMMON";
        #endregion

        #region constructor
        public FindCommand(string pathBegin)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Поиск каталога #COMMON "));
            PathBegin = new FileInfo(pathBegin).DirectoryName;
        }
        #endregion

        #region Find
        private void findDir(ref string sdir)
        {
            if (sdir.Length == 0)
                return;

            var dirs = Directory.GetDirectories(sdir);
            var path1 = dirs.FirstOrDefault(x => x.ToLower().Contains(Common.ToLower()));
            if (path1 != null)
                return;

            var paths = sdir.Split("\\");
            var s = "";

            for (var i = 0; i < paths.Length - 1; i++)
                s += paths[i] + "\\";

            sdir = s.Length == 0 ? "" : s.Substring(0, s.Length - 1);

            findDir(ref sdir);
        }

        public string FindCommon(string path = "")
        {
            var path0 = path == "" ? PathBegin : path;
            path0 = path0.Replace("//", "\\");
            var i = path0.IndexOf(Common, StringComparison.Ordinal);
            if (i > 0)
                return path0.Substring(0, i)+ Common;

            
            if (Directory.Exists(path0))
            {
                findDir(ref path0);
                path0 += "\\" + Common;
            }
            else
                path0 = "";

            return path0;
        }
        #endregion

    }
}
