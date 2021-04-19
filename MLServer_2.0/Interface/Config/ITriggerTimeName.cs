using System;
using System.Collections.Generic;
using MLServer_2._0.Moduls.Config;

namespace MLServer_2._0.Interface.Config
{
    public interface ITriggerTimeName
    {
        List<DanTriggerTime> ReadInfoTimeTrigger(string sdata0, string sdata1);
        List<DanTriggerTime> ReadInfoTimeTrigger(DateTime data0, DateTime data1);

    }
}
