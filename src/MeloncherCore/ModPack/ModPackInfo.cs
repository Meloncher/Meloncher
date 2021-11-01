using MeloncherCore.Account;
using Newtonsoft.Json;

namespace MeloncherCore.ModPack
{
	public class ModPackInfo
	{
		[JsonProperty("VersionName")] public string VersionName { get; set; }
	}
}