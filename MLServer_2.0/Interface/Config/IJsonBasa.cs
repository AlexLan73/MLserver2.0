using System.Threading.Tasks;

using TypeDStringMemoryInfo1
    = System.Collections.Concurrent.ConcurrentDictionary<string,
                    System.Collections.Concurrent.ConcurrentDictionary<string,
                                            MLServer_2._0.Moduls.ClfFileType.MemoryInfo>>;

namespace MLServer_2._0.Interface.Config
{
    public interface IJsonBasa
    {
        
       Task AddFileMemInfo(TypeDStringMemoryInfo1 fileMemInfo);
        T LoadFileJso<T>(string filejson);
        void LoadFileJsoDbConfig();
        Task SaveFileFileMemInfo();
    }
}
