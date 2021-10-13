using System.Collections.Concurrent;
using System;
using System.Reflection;

namespace ManagerThread
{
    public static class ThreadManager
    {
        //        public static ConcurrentDictionary<Guid, Task> CManagerTast = new ConcurrentDictionary<int, Task>();
        public static ConcurrentDictionary<Guid, (Task, DanX, string)> CManagerTask = new ConcurrentDictionary<Guid, (Task, DanX, string)>();
        public static ConcurrentDictionary<Guid, (Task, Action, string)> CManagerTaskAction = new ConcurrentDictionary<Guid, (Task, Action, string)>();

//        public static void Add(Task t) => CManagerTast.AddOrUpdate(t.Id, t, (x, y) => t);

        public static void DelXX(Guid i)
        {
            if (CManagerTask.ContainsKey(i))
            {
                (Task, DanX, string) z;
                CManagerTask.TryRemove(i, out z);
            }
        }
        public static void DelX(Guid i)
        {
            if (CManagerTask.ContainsKey(i))
            {
                var _x = CManagerTask[i];
                _x.Item2?.ClearDanX();
                (Task, DanX, string) z;
                CManagerTask.TryRemove(i, out z);
            }
        }

        public static void Add(Guid id, Task task, string s)
        {
            if (CManagerTask.ContainsKey(id))
            {
                var z = CManagerTask[id];

                _ = CManagerTask.AddOrUpdate(id, (task, z.Item2, z.Item3 == "" ? s : z.Item3)
                                    , (x, y) => (task, z.Item2, z.Item3 == "" ? s : z.Item3));
            }
            else
            {
                _ = CManagerTask.AddOrUpdate(id, (task, null, s), (x, y) => (task, null, s));
            }
        }
        public static void Add(Guid id, DanX action, string s)
        {
            if (CManagerTask.ContainsKey(id))
            {
                var z = CManagerTask[id];

                _ = CManagerTask.AddOrUpdate(id, (z.Item1, action, s), (x, y) => (z.Item1, action, s));
            }
            else
            {
                _ = CManagerTask.AddOrUpdate(id, (null, action, s), (x, y) => (null, action, s));
            }

        }
        public static List<Guid> FindNotRunTask()
        {
            var _countTask = CManagerTask
                    .Where(x => x.Value.Item1 != null ? x.Value.Item1.Status == TaskStatus.Running : false)
                    .Select(y => y.Key)
                    .ToList();
            return _countTask;
        }


        public static void DelXXA(Guid i)
        {
            if (CManagerTaskAction.ContainsKey(i))
            {
                (Task, Action, string) z;
                CManagerTaskAction.TryRemove(i, out z);
            }
        }
        public static void DelXA(Guid i)
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
        public static void AddA(Guid id, Task task, string s)
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
        public static void AddA(Guid id, Action action, string s)
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
        public static List<Guid> FindNotRunTaskA()
        {
            var _countTask = CManagerTaskAction
                    .Where(x => x.Value.Item1 != null ? x.Value.Item1.Status == TaskStatus.Running : false)
                    .Select(y => y.Key)
                    .ToList();
            return _countTask;
        }

    }
}