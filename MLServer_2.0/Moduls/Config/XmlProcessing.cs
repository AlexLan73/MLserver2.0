#nullable enable
using MLServer_2._0.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using LXmld = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, string>>;

namespace MLServer_2._0.Moduls.Config
{
    public class XmlProcessing
    {
        #region Data
        public LXmld? Dxml { get; private set; }    //  Данные для последующей обработки
        public Dictionary<string, string>? VSysVar { get; private set; } // Данные vSysVar
        public Task XmLstream { get; private set; } // контроль потока
        #endregion

        /// <summary>
        /// <param name="filename"></param>  Путь к файлу который нужно разобрать 
        /// <param name="masTega"></param>   Маска параметров
        /// </summary>
        /// 
        public XmlProcessing(string filename, string[] masTega, string selectnodes = "AnalysisPackage/DatabaseList/Database")
        {
            _ = LoggerManager.AddLoggerAsync(new LoggerEvent(EnumError.Info, "Загружаем Class XmlProcessing"));
            XmLstream = new TaskFactory().StartNew(() =>
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filename);

//                var rezultat = (from XmlNode elem in doc.SelectNodes(selectnodes)
//                    select elem.Cast<XmlNode>()
//                        .ToDictionary(elem1 => elem1.Name.ToLower(), elem1 => elem1.InnerText.ToLower())).ToList();

                var rezultat = new LXmld();
                foreach (XmlNode elem in doc.SelectNodes(selectnodes)!)
                {
                    rezultat.Add(elem.Cast<XmlNode>()
                        .ToDictionary(elem1 => elem1.Name.ToLower(), elem1 => elem1.InnerText.ToLower()));
                }

                if (rezultat.Count == 0)
                    return;

                Dxml = new LXmld();

                foreach (var item0 in rezultat)
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();

                    foreach (var key1 in masTega)
                    {
                        var val = key1 switch
                        {
                            "path" => item0.ContainsKey(key1) ? item0[key1] : "path",
                            "bustype" => item0.ContainsKey(key1) ? item0[key1] : "bt_bustype",
                            "channel" => item0.ContainsKey(key1) ? item0[key1] : "-1",
                            "type" => item0.ContainsKey(key1) ? item0[key1] : "type",
                            _ => ""
                        };

                        d.Add(key1, val);
                    }

                    if (d["type"] == "vsysvar")
                        VSysVar = new Dictionary<string, string>(d);
                    else
                        Dxml.Add(d);
                }
            });
        }
    }
}

