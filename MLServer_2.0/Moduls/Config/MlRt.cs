using Convert.Logger;
using Convert.Moduls.Error;

// ReSharper disable once CheckNamespace
namespace Convert.Moduls.Config
{
    public class MlRt : IniProcessing
    {

        public MlRt(string filename, string[] fields, ref Config0 config)
                                    : base(filename, fields, ref config)
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Обработка файла MlRt \n MlRt file processing"));
        }

        public override bool Convert()
        {
            if (ReadIni())
                return true;

            if (base.Convert())
            {
                if (!Data.ContainsKey("filename"))
                {
                    _ = ErrorBasa.FError(-211);
                    return false;
                }
            }

            foreach (var (key, val) in Data)
                Config.Fields.AddOrUpdate(key, val, (_, _) => val);

            return false;
        }
    }
}


