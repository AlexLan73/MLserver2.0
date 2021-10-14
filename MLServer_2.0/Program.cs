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
            WriteLine("----   Ver programs 2.1      ----------------------------------------");
            WriteLine("----    params:              ----------------------------------------");
            WriteLine("----     1. Path start convert.exe           ------------------------");
            WriteLine("----     2. Work directory                   ------------------------");
            WriteLine("----     3. rename:dir (directorys) create DBConfig          --------");
            WriteLine("----     4. out: directory where to put conversion files     --------");
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
                ThreadManager.TestTask();
                return;
            }

            if (inputArguments.DArgs.ContainsKey("RenameDir"))
            {
                CreateDbConfig createDbConfig = new CreateDbConfig(inputArguments.DArgs);
                createDbConfig.Run();
                ThreadManager.TestTask();
                return;
            }

            if (inputArguments.DArgs.ContainsKey("MDFRenameDir"))
            {
                MDFClassRenameDir mDFClassRenameDir = new MDFClassRenameDir(inputArguments.DArgs);
                mDFClassRenameDir.Run();
                ThreadManager.TestTask();
                return;
            }

            BasaClassConvert basaClassConvert = new BasaClassConvert(inputArguments.DArgs);
            basaClassConvert.Run();
            ThreadManager.TestTask();
            WriteLine("Все,  Halast))");
        }

    }
}

//"\\mlmsrv\MLServer\#COMMON\Dll\convert.exe" - ~"E:\MLserver\data\PS03SED"  ~test rename:\\mlmsrv\MLServer\PS03SED\