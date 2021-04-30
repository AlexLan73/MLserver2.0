using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.FileManager
{
    public class InfoExe
    {
        public int CodeError { get; set; }
        public string PathNameFile { get; set; }
        public int Id { get; set; }     //  id процесса
        public bool Is { get; set; }    //  завершение процесса True - error!!
        public List<string> LInfo = new List<string>();

        public InfoExe(string pathnamefile)
        {
            PathNameFile = pathnamefile;
            Id = -1;
            Is = false;

        }
        public InfoExe(int codeerror, int id, bool @is, List<string> lInfo)
        {
            CodeError = codeerror;
            Id = id;
            Is = @is;
            LInfo = new List<string>(lInfo);
        }
        public InfoExe(string file, int id, bool @is, List<string> lInfo)
        {
            PathNameFile = file;
            Id = id;
            Is = @is;
            LInfo = new List<string>(lInfo);
        }
    }
}
