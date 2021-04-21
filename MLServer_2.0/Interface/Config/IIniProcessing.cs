using System.Collections.Generic;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.Config;

namespace MLServer_2._0.Interface.Config
{
    public interface IIniProcessing
    {
        string Field { get; set; }
        string[] Fields { get; set; }
        List<string> Ldata { get; set; }
        Dictionary<string, string> Data { get; set; }
        bool Convert();
        Config0 Config { get; set; }
    }
}
