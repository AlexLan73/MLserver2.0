using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls
{
    internal class RecoverOriginalFiles
    {
        #region Data
        private readonly string _pathWork;
        #endregion

        #region constructor
        public RecoverOriginalFiles(string pathWork)
        {
            _pathWork = pathWork;
        }
        #endregion
        #region moduls
        public void Run()
        {
            var _dirs = Directory.GetDirectories(_pathWork, "~!D*", SearchOption.TopDirectoryOnly);
        }
        #endregion
    }
}
