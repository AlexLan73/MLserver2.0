////////
// ReSharper disable once InvalidXmlDocComment
/// Пример     https://metanit.com/sharp/patterns/2.3.php
///////

using System;
using System.Collections.Concurrent;

namespace MLServer_2._0.Moduls.Singleton
{
    public class DanGlobal
    {
        private static readonly Lazy<DanGlobal> Lazy =
            new Lazy<DanGlobal>(() => new DanGlobal());

        private DanGlobal()
        {
        }

        public static DanGlobal GetInstance()=> Lazy.Value;
//        public ConcurrentDictionary<string, object> Analysis = new ConcurrentDictionary<string, object>();
//        public ConcurrentDictionary<string, string> PathDan = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, object> Config = new ConcurrentDictionary<string, object>();

    }
}

/*
 public class Singleton
{
    private static readonly Lazy<Singleton> lazy = 
        new Lazy<Singleton>(() => new Singleton());
 
    public string Name { get; private set; }
         
    private Singleton()
    {
        Name = System.Guid.NewGuid().ToString();
    }
     
    public static Singleton GetInstance()
    {
        return lazy.Value;
    }
}
 */