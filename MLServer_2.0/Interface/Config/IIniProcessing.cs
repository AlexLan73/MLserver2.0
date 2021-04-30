﻿using Convert.Moduls.Config;
using System.Collections.Generic;

namespace Convert.Interface.Config
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
