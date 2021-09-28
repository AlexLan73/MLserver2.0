using System.Collections.Concurrent;

namespace ManagerThread
{
    public static class ThreadManager
    {
//        public static ConcurrentDictionary<Guid, Task> CManagerTast = new ConcurrentDictionary<int, Task>();
        public static ConcurrentDictionary<Guid, (Task, DanX, string)> CManagerTask = new ConcurrentDictionary<Guid, (Task, DanX, string)>();

        //        public static void Add(Task t)=> CManagerTast.AddOrUpdate(t.Id, t, (x, y) => t);

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

                _ = CManagerTask.AddOrUpdate(id, (task, z.Item2, z.Item3==""?s: z.Item3)
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

        //        public static void Add(Task t)=> CManagerTast.AddOrUpdate(t.Id, t, (x, y) => t);
        //public static void Add(int t, DanX action)
        //{
        //    _ = CManagerTask.AddOrUpdate(t, (null, action), (x, y) => (null, action));
        //}
        //public static void Add(Task task)
        //{
        //    _ = CManagerTask.AddOrUpdate(task.Id, (task, task.), (x, y) => (task, _x.Item2));

        //    var _x = CManagerTask[99999];
        //    _ = CManagerTask.AddOrUpdate(t, (task, _x.Item2), (x, y) => (task, _x.Item2));
        //    (Task, DanX) z;
        //    CManagerTask.TryRemove(99999, out z);
        //}


        public static List<Guid> FindNotRunTask()
        {
            var _countTask = CManagerTask
                    .Where(x=> x.Value.Item1!=null? x.Value.Item1.Status == TaskStatus.Running:false)
                    .Select(y=>y.Key)
                    .ToList();
//            _countTask.ForEach(z=> Console.WriteLine(z));
            return _countTask;
        }

    }
}