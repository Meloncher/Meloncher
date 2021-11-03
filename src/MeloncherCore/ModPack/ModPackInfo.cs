using Newtonsoft.Json;

namespace MeloncherCore.ModPack
{
	public class ModPackInfo
	{
		public ModPackInfo(string versionName)
		{
			VersionName = versionName;
		}

		[JsonProperty("VersionName")] public string VersionName { get; set; }
	}
}