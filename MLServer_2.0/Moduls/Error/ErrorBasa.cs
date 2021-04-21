using MLServer_2._0.Logger;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace MLServer_2._0.Moduls.Error
{
    public delegate void DelegErrorNun(int errorcod);
    public delegate void DelegErrorNunMessag(int errorcod, string message = "");

    public class STypeError1
    {
        public STypeError1(int nerror, string nameerror)
        {
            NError = nerror;
            NaneError = nameerror;

        }
        public int NError { get; set; }
        public string NaneError { get; set; }

    }

    public class STypeError2 : STypeError1
    {
        public STypeError2(int nerror, string nameerror, string section) : base(nerror, nameerror)
        {
            Section = section;
        }
        public string Section { get; set; }
        public string Set(string messages)
        {
            return $"{NaneError.Replace("%file%", messages)} - {Section}";
        }
        public override string ToString()
        {
            return $" {NaneError} -  {Section} ";
        }

    }

    public class ErrorBasa
    {
        #region data
        private ConcurrentDictionary<int, (object, object, EnumError)> DError = new ConcurrentDictionary<int, (object, object, EnumError)>();
        private static ErrorBasa _errorBasa = null;
        private DelegErrorNun en;
        private DelegErrorNunMessag enm;
        private const string NameModulConfig = "Модуль SetupParam ";

        #region CodeError
        private const string _err1 = "Проблема с входными параметрами";
        private const string _err_1 = "Мало аргументов, < 2  Модуль InputArguments";
        private const string _err_2 = "Не существует файл %file% Модуль InputArguments";
        private const string _err_3 = "Нет рабочей директории %file%  Модуль InputArguments ";
        private const string _err_4 = " ==>> #### Ошибка формирования: путей, json, ml_rt  ####";
        private const string _err_5 = " С конвертацией в lrf_dec  Ошибка в обработке первоначальных данных";
        private const string _err_7 = " Проблема с чтением информации из  %file%  ";
        private const string _err_8 = " Error в запуске CLexport  %file%  ";
        private const string _err_20 = "Нет файла %file% ";
        private const string _err_201 = "Проблема с записью  %file% ";
        private const string _err_202 = "Проблема с записью  %file% \n отсутствует { _xml.VSysVarPath } ";
        private const string _err_23 = "В файле %file% нет данных о Trigger и времени" + NameModulConfig;
        private const string _err_231 = "В файле %file% нет поля => filename";
        private const string _err_24 = "Нет каталога #COMMON " + NameModulConfig;
        private const string _err_211 = " Нет соответствия запрашиваемых данных и полученных ==> Модуль конфигурации инициал - ция" + NameModulConfig;
        private const string _err_212 = " Нет данных в ml_rt2 " + NameModulConfig;
        private const string _err_213 = " Нет данных в TextLog, время срабатывания триггера " + NameModulConfig;
        private const string _err_25 = "Нет файла(директории) с Analis " + NameModulConfig;
        private const string _err_26 = "Нет файла для анализа  %file% ";
        private const string _err_261 = "Нет нужных данных в файле %file%  для анализа ";
        private const string _err_27 = "Error c файлом конфигурации %file% ";
        private const string _err_271 = "Error c файлом конфигурации Нет конфиг.MDF(...)  %file%  ";
        private const string _err_272 = "Error c файлом конфигурации Нет конфиг. LRF_DEC(...)  %file%  ";
        private const string _err_31 = "не правильно отработала программа FileType  ClfFileInfo";
        private const string _err_32 = "нет строки '$Car: [' в FileType  ClfFileInfo";
        private const string _err_34 = "нет информации о Memory: в файле %file%";


        #endregion
        #endregion

        public ErrorBasa()
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class ErrorBasa"));

            _errorBasa = this;
            en = _errorBasa.ErrorNun;
            enm = _errorBasa.ErrorNunMessag;
            inicial();
        }

        private void inicial()
        {
            DError.AddOrUpdate(1, (_err1, en, EnumError.Info), (_, _) => (_err1, en, EnumError.Error));
            DError.AddOrUpdate(-1, (_err_1, en, EnumError.Error), (_, _) => (_err_1, en, EnumError.Error));
            DError.AddOrUpdate(-2, (new STypeError2(-2, _err_2, NameModulConfig), enm, EnumError.Error),
                          (_, _) => (new STypeError2(-2, _err_2, NameModulConfig), enm, EnumError.Error));
            DError.AddOrUpdate(-3, (new STypeError2(-3, _err_3, NameModulConfig), enm, EnumError.Error),
                          (_, _) => (new STypeError2(-3, _err_3, NameModulConfig), enm, EnumError.Error));
            DError.AddOrUpdate(-4, (_err_4, en, EnumError.Error), (_, _) => (_err_4, en, EnumError.Error));
            DError.AddOrUpdate(-5, (_err_5, en, EnumError.Warning), (_, _) => (_err_5, en, EnumError.Warning));
            DError.AddOrUpdate(-7, (new STypeError2(-7, _err_7, " Ошибка в обработке -> ClfFileInfo "), enm, EnumError.Warning),
                           (_, _) => (new STypeError2(-7, _err_7, " Ошибка в обработке -> ClfFileInfo "), enm, EnumError.Warning));
            DError.AddOrUpdate(-8, (new STypeError2(-8, _err_8, " error CLexport "), enm, EnumError.Warning),
                           (_, _) => (new STypeError2(-8, _err_8, " error CLexport "), enm, EnumError.Warning));
            DError.AddOrUpdate(-20, (new STypeError2(-20, _err_20, NameModulConfig), enm, EnumError.Error),
                          (_, _) => (new STypeError2(-20, _err_20, NameModulConfig), enm, EnumError.Error));
            DError.AddOrUpdate(-201, (new STypeError2(-201, _err_201, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-201, _err_201, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-202, (new STypeError2(-202, _err_202, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-202, _err_202, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-23, (new STypeError2(-23, _err_23, NameModulConfig), enm, EnumError.Warning), 
                          (_, _) => (new STypeError2(-23, _err_23, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-231, (new STypeError2(-231, _err_231, NameModulConfig), enm, EnumError.Warning), 
                           (_, _) => (new STypeError2(-231, _err_231, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-24, (_err_24, en, EnumError.Error), (_, _) => (_err_24, en, EnumError.Error));
            DError.AddOrUpdate(-25, (_err_25, en, EnumError.Warning), (_, _) => (_err_25, en, EnumError.Warning));
            DError.AddOrUpdate(-211, (_err_211, en, EnumError.Error), (_, _) => (_err_211, en, EnumError.Error));
            DError.AddOrUpdate(-212, (_err_212, en, EnumError.Warning), (_, _) => (_err_212, en, EnumError.Warning));
            DError.AddOrUpdate(-213, (_err_213, en, EnumError.Warning), (_, _) => (_err_213, en, EnumError.Warning));
            DError.AddOrUpdate(-26, (new STypeError2(-26, _err_26, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-26, _err_26, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-261, (new STypeError2(-261, _err_261, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-261, _err_261, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-27, (new STypeError2(-27, _err_27, NameModulConfig), enm, EnumError.Error),
                          (_, _) => (new STypeError2(-27, _err_27, NameModulConfig), enm, EnumError.Error));
            DError.AddOrUpdate(-271, (new STypeError2(-271, _err_271, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-271, _err_271, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-272, (new STypeError2(-272, _err_272, NameModulConfig), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-272, _err_272, NameModulConfig), enm, EnumError.Warning));
            DError.AddOrUpdate(-31, (_err_31, en, EnumError.Warning), (_, _) => (_err_31, en, EnumError.Warning));
            DError.AddOrUpdate(-32, (_err_32, en, EnumError.Warning), (_, _) => (_err_32, en, EnumError.Warning));
            DError.AddOrUpdate(-34, (new STypeError2(-34, _err_34, "ClfFileInfo"), enm, EnumError.Warning),
                          (_, _) => (new STypeError2(-34, _err_34, "ClfFileInfo"), enm, EnumError.Warning));

        }
        public void ErrorNun(int cod)
        {
            var _info = _errorBasa.DError[cod];
            var _typeerror = _info.Item3;

            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(_typeerror, (string)_info.Item1));

            if (_typeerror == EnumError.Error)
            {
                LoggerManager.DisposeStatic();
                Thread.Sleep(1000);
                Environment.Exit(cod); 
            }
        }
        public void ErrorNunMessag(int cod, string message = "")
        {
            switch (DError[cod].Item1.GetType().Name)
            {
                case "STypeError2":
//                    _iLogger.AddLoggerInfoAsync(new LoggerEvent(_errorBasa.DError[cod].Item3, ((STypeError2)DError[cod].Item1).Set(message)));
                    _ = LoggerManager.AddLoggerAsync(new LoggerEvent(_errorBasa.DError[cod].Item3, ((STypeError2)DError[cod].Item1).Set(message)));
                    break;
            }

        }
        struct S01
        {
            public S01(int i, object m)
            {
                Cod = i;
                Mes = m;
            }
            public int Cod { get; set; }
            public object Mes { get; set; }
        }
        public static async Task FError(int cod, string message = "")
        {
            await Task.Factory.StartNew((object x0) =>
            {
                var z1 = (S01)x0;
                var _info = _errorBasa.DError[(int)z1.Cod];
                switch (_info.Item2.GetType().Name)
                {
                    case "DelegErrorNun":
//                        var _deleg = (DelegErrorNun)_info.Item2; _deleg((int)z1.Cod);
                        ((DelegErrorNun)_info.Item2)((int)z1.Cod);
                        break;
                    case "DelegErrorNunMessag": ((DelegErrorNunMessag)_info.Item2)(z1.Cod, (string)z1.Mes); break;
                }
            }, new S01(cod, message));
        }
    }
}
