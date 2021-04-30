// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public interface ITypeDanFromFile1 : ITypeDanFromFile0
    {
        string NameFile1 { get; set; }
    }
    public class TypeDanFromFile1 : TypeDanFromFile0, ITypeDanFromFile1
    {
        public string NameFile1 { get; set; }


        public TypeDanFromFile1(string namefile0, string namefile1, int repit = 20, int compareSec = 120) :
            base(namefile0, repit, compareSec)
        {
            NameFile1 = namefile1;
        }
        public TypeDanFromFile1(ITypeDanFromFile1 sourse) : base(sourse)
        {
            NameFile1 = sourse.NameFile1;
        }

    }
}
