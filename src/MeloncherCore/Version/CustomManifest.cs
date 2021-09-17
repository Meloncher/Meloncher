using CmlLib.Core.VersionLoader;
using System.Collections.Generic;

namespace MeloncherCore.Version
{
    class CustomManifest
    {
        IVersionLoader versionLoader;
        public CustomManifest(IVersionLoader versionLoader)
        {
            this.versionLoader = versionLoader;
    
        }
        public Dictionary<string, List<McVersion>> Test()
        {
            var metas = versionLoader.GetVersionMetadatas();
            Dictionary<string, List<McVersion>> list = new Dictionary<string, List<McVersion>>();
            foreach (var meta in metas)
            {
                if (!list.ContainsKey(meta.MType.ToString()))
                {
                    list.Add(meta.MType.ToString(), new List<McVersion>());
                }
                string profileType = meta.MType.ToString();
                if (profileType == "Release")
                {
                    var spl = meta.Name.Split(".");
                    if (spl.Length >= 2)
                    {
                        profileType = "Release-" + spl[0] + "." + spl[1];
                    }
                }
                list[meta.MType.ToString()].Add(new McVersion(meta.Name, meta.MType.ToString(), profileType));
            }
            return list;
        }
    }
}
