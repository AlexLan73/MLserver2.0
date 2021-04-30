using Convert.Interface.Config;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
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
