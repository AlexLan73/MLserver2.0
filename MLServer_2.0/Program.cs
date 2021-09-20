// ReSharper disable once InvalidXmlDocComment
// R-eSharper disable all StringLiteralTypo
//  строка запуска c:\mlserver\#common\DLL\lrf_dec.exe -S 20 -L 512 -n -k -v -i C:\MLserver\PS18LIM\log\2020-11-04_18-36-34
////  https://docs.microsoft.com/ru-ru/dotnet/core/deploying/single-file  создание публикации

using Convert.Logger;
using Convert.Moduls;
using Convert.Moduls.Config;
using Convert.Moduls.Error;
using MLServer_2._0;
using MLServer_2._0.Moduls;
using MLServer_2._0.Moduls.MDFRename;

// ReSharper disable once InvalidXmlDocComment
///   "out:E:\OutTest"   "rename:\\mlmsrv\MLServer\PTA10SUV"
//  "rename:E:\MLserver\data\PS18SED\log\2020-09-03_06-00-36"

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Console;

// ReSharper disable once CheckNamespace
namespace Convert
{

    class Program
    {
        //        static LoggerManager _logger;

        static void Main(string[] args)
        {
            WriteLine("---------------------------------------------------------------------");
            WriteLine("---------------------------------------------------------------------");
            WriteLine("----   Версия программы 2.0  ----------------------------------------");
            WriteLine("----    параметры:           ----------------------------------------");
            WriteLine("----     1. Путь где лежит запускаемый файл  ------------------------");
            WriteLine("----     2. Рабочий каталог                  ------------------------");
            WriteLine("----     3. rename:Каталог (каталоги) создаем DBConfig  -------------");
            WriteLine("----     4. out:Каталог куда нужно вывести конвертацию  -------------");
            WriteLine("---------------------------------------------------------------------");
            WriteLine("---------------------------------------------------------------------");

            var inputArguments = new InputArguments(args);
            var resultError = inputArguments.Parser();

            //////////////////////////////////////////////////////////
            // ReSharper disable once InvalidXmlDocComment
            ///     resultError -> true  если истина то лшибка
            /////////////////////////////////////////////////////////
            if (resultError)
            {
                WriteLine(" --->   Ошибка в командной строке");
                WriteLine($"   код ошибки {resultError.Error.Error}");
                WriteLine($"   название {resultError.Error.NameError}");
                WriteLine($"   раздел {resultError.Error.NameRazdel}");
                if (resultError.Error.Id != null)
                    WriteLine($"   код ID {resultError.Error.Id.Value}");
                Environment.Exit(-1);
            }
            if (inputArguments.DArgs.ContainsKey("~d"))
            {
                new RecoverOriginalFiles(inputArguments.DArgs["WorkDir"]).Run();
                return;
            }

            if (inputArguments.DArgs.ContainsKey("RenameDir"))
            {
                CreateDbConfig createDbConfig = new CreateDbConfig(inputArguments.DArgs);
                createDbConfig.Run();
                return;
            }

            if (inputArguments.DArgs.ContainsKey("MDFRenameDir"))
            {
                MDFClassRenameDir mDFClassRenameDir = new MDFClassRenameDir(inputArguments.DArgs);
                mDFClassRenameDir.Run();
                return;
            }

            BasaClassConvert basaClassConvert = new BasaClassConvert(inputArguments.DArgs);
            basaClassConvert.Run();

            WriteLine("Все ))");
        }

    }
}

//"\\mlmsrv\MLServer\#COMMON\Dll\convert.exe" - ~"E:\MLserver\data\PS03SED"  ~test rename:\\mlmsrv\MLServer\PS03SED\