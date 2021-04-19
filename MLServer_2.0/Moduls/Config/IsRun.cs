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
        }
        public IsRun(bool issource, bool isclr)
        {
            IsSource = issource;
            IsClr = isclr;
        }
        public bool IsSource { get; set; }
        public bool IsClr { get; set; }
        public bool IsRename { get; set; }
    }
}
