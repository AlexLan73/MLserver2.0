using Convert.Moduls.Config;
using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Convert.Interface.Config
{
    public interface ITriggerTimeName
    {
        List<DanTriggerTime> ReadInfoTimeTrigger(string sdata0, string sdata1);
        List<DanTriggerTime> ReadInfoTimeTrigger(DateTime data0, DateTime data1);

    }
}
