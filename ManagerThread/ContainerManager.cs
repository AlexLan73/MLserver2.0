using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerThread
{
    public class ContainerManager
    {
        private static readonly Lazy<ContainerManager> Lazy = new(() => new ContainerManager());
        public static ContainerManager GetInstance() => Lazy.Value;
        private ContainerManager()
        {
        }
        

    }
}
