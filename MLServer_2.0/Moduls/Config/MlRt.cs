using MLServer_2._0.Logger;
using MLServer_2._0.Moduls.Error;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.Config
{
    public class MlRt : IniProcessing
    {
        public MlRt(string filename, string[] fields, ref Config0 config) 
                                    : base(filename, fields, ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Обработка файла MlRt"));
        }

        public override bool Convert()
        {
            var result = ReadIni();

            if (result)
                return true;

            if (base.Convert())
            {
                if (!Data.ContainsKey("filename"))
                {
                    _ = ErrorBasa.FError(-211);
                    return false;
                }
            }

            foreach ((var key, var val) in Data)
                Config.Fields.AddOrUpdate(key, val, (_, _) => val);

            return false;
        }
    }
}


