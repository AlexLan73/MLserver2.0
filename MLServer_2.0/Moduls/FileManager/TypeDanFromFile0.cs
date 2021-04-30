using System;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public interface ITypeDanFromFile0
    {
        string NameFile0 { get; set; }
        bool IsRun { get; set; }
        int Count { get; set; }
        DateTime? StartTime { get; set; }
        int SecWait { get; set; }
        int Repit { get; set; }
        int CompareSec { get; set; }
        bool IsStartTest { get; set; }
    }
    public class TypeDanFromFile0 : ITypeDanFromFile0
    {
        public string NameFile0 { get; set; }
        public bool IsRun { get => Count > Repit && SecWait > CompareSec; set { _ = value; } }
        public int Count { get; set; }
        public DateTime? StartTime { get; set; }
        public int SecWait { get; set; }
        public int Repit { get; set; }
        public int CompareSec { get; set; }
        public bool IsStartTest { get; set; }


        public TypeDanFromFile0(string namefile0, int repit = 20, int compareSec = 120)
        {
            NameFile0 = namefile0;
            IsRun = false;
            Count = 0;
            SecWait = 0;
            Repit = repit;
            CompareSec = compareSec;
            IsStartTest = false;
        }
        public TypeDanFromFile0(ITypeDanFromFile0 sourse)
        {
            NameFile0 = sourse.NameFile0;
            IsRun = sourse.IsRun;
            Count = sourse.Count;
            StartTime = sourse.StartTime;
            SecWait = sourse.SecWait;
            Repit = sourse.Repit;
            IsStartTest = sourse.IsStartTest;
            CompareSec = sourse.CompareSec;

        }
        public virtual void CalcSecDateTime()
        {
            if (IsStartTest)
            {
                if (StartTime == null)
                    StartTime = DateTime.Now;
                else
                    SecWait += (DateTime.Now - StartTime).Value.Seconds;

                Count += 1;
            }
        }

    }
}
