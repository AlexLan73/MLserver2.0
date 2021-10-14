using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MLServer_2._0
{
    public class ThreadTest
    {
        public ThreadTest() => Is = true;
        public bool Is { get; private set; }
        public void ClearIx() => Is = false;
    }

    internal static class ThreadManager
    {
//        public static ConcurrentDictionary<Guid, (Task, Action, string)> CManagerTaskAction = new();
        public static ConcurrentDictionary<Guid, (Task, Action, string)> CManagerTaskAction = new();
        public static void Add(Guid id, Task task, string s)
        {
            if (CManagerTaskAction.ContainsKey(id))
            {
                var z = CManagerTaskAction[id];

                _ = CManagerTaskAction.AddOrUpdate(id, (task, z.Item2, z.Item3 == "" ? s : z.Item3),
                                (x, y) => (task, z.Item2, z.Item3 == "" ? s : z.Item3));
            }
            else
            {
                _ = CManagerTaskAction.AddOrUpdate(id, (task, null, s), (x, y) => (task, null, s));
            }

        }

        public static void Add(Guid id, Action action, string s)
        {
            if (CManagerTaskAction.ContainsKey(id))
            {
                var z = CManagerTaskAction[id];

                _ = CManagerTaskAction.AddOrUpdate(id, (z.Item1, action, s), (x, y) => (z.Item1, action, s));
            }
            else
            {
                _ = CManagerTaskAction.AddOrUpdate(id, (null, action, s), (x, y) => (null, action, s));
            }
        }

        public static void DelRecInDict(Guid i)     //  для удаления записи из словаря когда процес закончен
        {
            if (CManagerTaskAction.ContainsKey(i))
            {
                (Task, Action, string) z;
                CManagerTaskAction.TryRemove(i, out z);
            }
        }
        public static void DelProcesAndInDict(Guid i)     // остановить процес и удалить из словаря
        {
            if (CManagerTaskAction.ContainsKey(i))
            {
                var _x = CManagerTaskAction[i];
                if (_x.Item2 != null)
                {
                    CManagerTaskAction[i].Item2.Invoke();
                }

                (Task, Action, string) z;
                CManagerTaskAction.TryRemove(i, out z);
            }
        }
        public static List<Guid> FindNotRunTask()
        {
            var _countTask = CManagerTaskAction
                    .Where(x => x.Value.Item1 != null ? x.Value.Item1.Status == TaskStatus.Running : false)
                    .Select(y => y.Key)
                    .ToList();
            return _countTask;
        }
        
        public static void TestTask() 
        {
            List<Guid> _lcount = ThreadManager.FindNotRunTask();
            while (_lcount.Count > 0)
            {
                Console.WriteLine($" № {_lcount.Count}  ___  Wait  ____");
                foreach (Guid id in _lcount)
                {
                    try
                    {
                        var _x = ThreadManager.CManagerTaskAction[id];
                        Console.WriteLine($" № {id} ->  { _x.Item3 } ");
                        ThreadManager.DelProcesAndInDict(id);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($" № {id} -> Error ((  ");
                    }
                }
                _lcount = ThreadManager.FindNotRunTask();
            }
        }

    }
}
