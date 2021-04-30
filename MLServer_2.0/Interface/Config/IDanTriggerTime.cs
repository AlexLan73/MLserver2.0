using System;

// ReSharper disable once CheckNamespace
namespace Convert.Interface.Config
{
    public interface IDanTriggerTime
    {
        DateTime DateTime { get; set; }
        string Trigger { get; set; }
        string Work { get; set; }
        string Name { get; set; }
    }
}
