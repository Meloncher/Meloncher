#nullable enable
using System.ComponentModel;
using System.IO;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using Newtonsoft.Json;

namespace MeloncherCore.Settings
{
	public class LauncherSettings : INotifyPropertyChanged
	{
		private string? _storagePath;
		[JsonProperty("use_optifine")] public bool UseOptifine { get; set; } = true;
		[JsonProperty("window_mode")] public WindowMode WindowMode { get; set; } = WindowMode.Windowed;
		[JsonProperty("selected_version")] public string? SelectedVersion { get; set; }

		[JsonProperty("selected_profile_type")] public ProfileType SelectedProfileType { get; set; } = ProfileType.Vanilla;
		[JsonProperty("selected_account")] public string? SelectedAccount { get; set; }
		[JsonProperty("maximum_ram_mb")] public int MaximumRamMb { get; set; } = 1024;
		[JsonProperty("glass_background")] public bool GlassBackground { get; set; } = true;

		public event PropertyChangedEventHandler? PropertyChanged;

		public static LauncherSettings New(ExtMinecraftPath path)
		{
			var storagePath = Path.Combine(path.RootPath, "meloncher_settings.json");
			var jsonObj = "{}";
			if (File.Exists(storagePath)) jsonObj = File.ReadAllText(storagePath);
			var ls = JsonConvert.DeserializeObject<LauncherSettings>(jsonObj);
			ls._storagePath = storagePath;
			ls.PropertyChanged += (_, _) => { ls.SaveFile(); };
			return ls;
		}

		private void SaveFile()
		{
			var jsonStr = JsonConvert.SerializeObject(this);
			if (_storagePath != null) File.WriteAllTextAsync(_storagePath, jsonStr);
		}
	}

	public enum WindowMode
	{
		Windowed,
		Fullscreen,
		Borderless
	}
}