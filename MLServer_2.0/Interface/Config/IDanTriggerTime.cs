using System;

namespace MLServer_2._0.Interface.Config
{
    public interface IDanTriggerTime
    {
        DateTime DateTime { get; set; }
        string Trigger { get; set; }
        string Work { get; set; }
        string Name { get; set; }
    }
}
