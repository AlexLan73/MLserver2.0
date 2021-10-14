using Convert.Interface.Config;
using Convert.Logger;
using Convert.Moduls.Error;
using System;
using System.Collections.Generic;
using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class MasPaths : IMasPaths
    {
        #region data
        public string Common { get; set; }
        public string Dll
        {
            get => Common != "" ? Common + "\\DLL\\" : "";
            set => _ = value;
        }
        public string Siglogconfig
        {
            get => Common != "" ? Dll + "siglog_config.ini" : "";
            set => _ = value;
        }
        public string Namesiglog { get; set; }
        public string Mlserver
        {
            get => Common != "" ? Dll + "MLserver\\" : "";
            set => _ = value;
        }
        public string MlServerJson
        {
            get => Common != "" ? Dll + "mlserverNew.json" : "";
            set => _ = value;
        }
        public string LrfDec
        {
            get => Common != "" ? Dll + "lrf_dec.exe" : "";
            set => _ = value;
        }
        public string FileType
        {
            get => Common != "" ? Dll + "fileType.exe" : "";
            set => _ = value;
        }
        public string CLexport
        {
            get => Common != "" ? Mlserver + "CLexport.exe" : "";
            set => _ = value;
        }
        public string Clf
        {
            get => WorkDir != "" ? WorkDir + "\\CLF" : "";
            set => _ = value;
        }
        public string Log
        {
            get => WorkDir != "" ? WorkDir + "\\LOG" : "";
            set => _ = value;
        }
        public string DbConfig
        {
            get => WorkDir != "" ? WorkDir + "\\DbConfig.json" : "";
            set => _ = value;
        }
        public string ExeFile { get; set; }
        public string WorkDir { get; set; }
        public string OutputDir { get; set; }
        public string Analis { get; set; }
        #endregion

        #region construct
        public MasPaths(IReadOnlyDictionary<string, string> args)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем класс MasPaths - конфиг. с путями \n" +
                                                                             "Load the MasPaths class - config. with paths"));

            ExeFile = args["ExeFile"];
            WorkDir = args["WorkDir"];
            OutputDir = args["OutputDir"];
        }

        public MasPaths()
        {
        }
        #endregion

        #region NewPathWork

        public void SetNewPathWork(string work, string outpath)
        {
            WorkDir = work;
            OutputDir = outpath;
//            var _clf = Clf;
//            var _log = Log;
//            var _db = DbConfig;
        }
        #endregion


    #region FormPath
    public bool FormPath()
        {
            var findCommand = new FindCommand(ExeFile);
            var common = findCommand.FindCommon();
            Console.WriteLine($"1. common =>  {common} ");

            if (common == "")
            {
                var error = ErrorBasa.FError(-24);
                error.Wait();
                return true;
            }
            Console.WriteLine($"2. common =>  {common} ");

            Common = common;

            Namesiglog = "siglog_config.ini";
            return false;
        }
        #endregion

    }
}
