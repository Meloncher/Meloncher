using Newtonsoft.Json;

namespace MeloncherCore.Settings
{
	public class LauncherSettings
	{
		[JsonProperty("use_optifine")] public bool UseOptifine { get; set; }
	}
}
