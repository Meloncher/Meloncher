using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Settings
{ 
    public class LauncherSettings
    {
        [JsonProperty("use_optifine")] public bool UseOptifine { get; set; }
    }
}
