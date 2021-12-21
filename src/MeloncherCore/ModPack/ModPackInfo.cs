using MeloncherCore.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MeloncherCore.ModPack
{
	public class ModPackInfo
	{
		public ModPackInfo(string versionName, McClientType clientType)
		{
			VersionName = versionName;
			ClientType = clientType;
		}

		[JsonProperty("VersionName")] 
		public string VersionName { get; set; }

		[JsonProperty("ClientType"), JsonConverter(typeof(StringEnumConverter))]
		public McClientType ClientType { get; set; }
	}
}