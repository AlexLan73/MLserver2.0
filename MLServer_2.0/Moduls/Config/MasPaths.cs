using MLServer_2._0.Interface.Config;
using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.Config
{
    public class MasPaths : IMasPaths
    {
        public string Common { get; set; }
        public string Dll {
                            get => Common != "" ? Common + "\\DLL\\" : "";
                            set => _ = value; 
                          } 
        public string Siglogconfig { 
                                    get=> Common != "" ? Dll + "siglog_config.ini" : "";
                                    set => _ = value;
                                    }
        public string Namesiglog { get; set; }
        public string Mlserver { 
                                get => Common != "" ? Dll + "MLserver\\" : "";
                                set => _ = value; 
                                }
        public string MlServerJson
                                {
                                    get => Common != "" ? Dll + "mlserver.json" : "";
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
        private readonly ILogger _iLoger;

        public MasPaths(Dictionary<string, string> args, ILogger iLoger)
        {
            _iLoger = iLoger;
            ExeFile = args["ExeFile"];
            WorkDir = args["WorkDir"];
            OutputDir = args["OutputDir"];
        }

        public MasPaths()
        {
        }

//        public ResultTd<bool, SResulT0> FormPath()
        public bool FormPath()
        {
            var findCommand = new FindCommand(ExeFile);
            var common = findCommand.FindCommon();
            if (common == "")
            {
                var __error = ErrorBasa.FError(-24);
                __error.Wait();
                return true;

//                var sResulT0 = new SResulT0(-24, "Нет каталога #COMMON ", _nameModulConfig);
//                Task.Run(() => _iLoger.AddLoggerInfoAsync(new LoggerEvent(EnumError.Error, sResulT0, EnumLogger.MonitorFile)));
//                return new ResultTd<bool, SResulT0>(sResulT0);
            }

            Common = common;

            Namesiglog = "siglog_config.ini";

//            return new ResultTd<bool, SResulT0>(false);
            return false;

        }
    }
}

/*
             if (resul)
            {
                var __error = ErrorBasa.FError(2);
                __error.Wait();
            }

 
 */