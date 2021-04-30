using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Convert.Interface.Config
{
    public interface IInputArguments
    {
        string ExeFile { get; set; }
        string WorkDir { get; set; }
        string OutputDir { get; set; }
    }
    public interface IInputArgumentsDop : IInputArguments
    {
        Dictionary<string, string> DArgs { get; set; }
    }

    public interface IMasPaths : IInputArguments
    {
        string Common { get; set; }
        string Dll { get; set; }
        string Siglogconfig { get; set; }
        string Namesiglog { get; set; }
        string Mlserver { get; set; }
        string Analis { get; set; }
        string MlServerJson { get; set; }
        string LrfDec { get; set; }
        string FileType { get; set; }
        string CLexport { get; set; }
        string Clf { get; set; }
        string Log { get; set; }
        string DbConfig { get; set; }
    }
}
