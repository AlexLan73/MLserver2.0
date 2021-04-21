using MLServer_2._0.Interface.Config;

namespace MLServer_2._0.Moduls.Config
{
    public class IsRun : IIsRun
    {
        public IsRun()
        {
            IsSource = false;
            IsClr = false;
            IsRename = false;
            IsExport = false;
            IsExportRename = false;
        }
        public bool IsSource { get; set; }
        public bool IsClr { get; set; }
        public bool IsRename { get; set; }
        public bool IsExport { get; set; }
        public bool IsExportRename { get; set; }

    }
}
