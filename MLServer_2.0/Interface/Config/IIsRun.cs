// ReSharper disable once CheckNamespace
namespace Convert.Interface.Config
{
    public interface IIsRun
    {
        bool IsSource { get; set; }
        bool IsClr { get; set; }
        bool IsRename { get; set; }
    }
}
