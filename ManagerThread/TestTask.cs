using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerThread
{
    public class DanX
    {
        public DanX()=>Is = true;
        
        public bool Is {  get; set; }
        public void ClearDanX()=>Is = false;
        
    }
    public class TestTask
    {
        private readonly int _ncount;

        public TestTask(int ncount=20)
        {
            _ncount = ncount;
        }
        public void Run()
        {
            for (int n = 0; n <= _ncount; n++)
            {
                Guid idGuid = Guid.NewGuid();
                if (n % 2 == 0)
                {
                    Action<Guid, int> p = (__idGuid, i) =>
                    {
                        Thread.Sleep(2);
                        Guid _idG = __idGuid;
                        bool _is = true;
                        DanX danX = new DanX();
                        ThreadManager.Add(_idG, danX, " _o11__ "+i.ToString());
                        while (_is)
                        {
                            Thread.Sleep(100);
                            _is = danX.Is;
                        }

                    };
                    var _x01 = Task<bool>.Run(() => p(new Guid(idGuid.ToByteArray()), n));
                    ThreadManager.Add(new Guid(idGuid.ToByteArray()), _x01, " _o12__ " + n.ToString());

                }
                else
                {
                    Action<Guid, int> p1 = (__idGuid, i) =>
                    {
                        Guid _idG = __idGuid;
                        bool _is = true;
                        DanX danX = new DanX();
                        ThreadManager.Add(_idG, danX, " _o21_ " + i.ToString());
                        for (int i0 = 0; i0 < 100; i0++)
                        {
                            Thread.Sleep(200);
                            _is = danX.Is;

                        }
                        ThreadManager.DelXX(_idG);
                    };
                    var _x02 = Task<bool>.Run(() => p1(new Guid(idGuid.ToByteArray()), n));
                    ThreadManager.Add(new Guid(idGuid.ToByteArray()), _x02, " _o22__ " + n.ToString());
                }
                ThreadManager.FindNotRunTask();
            }

            Thread.Sleep(1000);
            List<Guid> _lcount = ThreadManager.FindNotRunTask();
            bool _iscount = _lcount.Count > 0;
            int kk = 0;
            while (_iscount)
            {

                Console.WriteLine($" {kk} -> {_lcount.Count}  ___  Wait  ____");
                foreach (Guid id in _lcount)
                {
                    try
                    {
                        var _x = ThreadManager.CManagerTask[id];
                        Console.WriteLine($" № {id} ->  { _x.Item3 } ");

                    }
                    catch (Exception)
                    {
                    }
                }

                Thread.Sleep(500);
                _lcount = ThreadManager.FindNotRunTask();
                _iscount = _lcount.Count > 0;

                kk++;
                if (kk > 20 && _lcount.Count>0)
                {
                    ThreadManager.DelX(_lcount[0]);
                }
            }
        }
    }
}
