using MeloncherCore.Launcher;
using Newtonsoft.Json;
using System.ComponentModel;
using System.IO;

namespace MeloncherCore.Settings
{
	public class LauncherSettings : INotifyPropertyChanged
	{
		[JsonProperty("use_optifine")] public bool UseOptifine { get; set; }
		[JsonProperty("window_mode")] public WindowMode WindowMode { get; set; }
		[JsonProperty("selected_version")] public string SelectedVersion { get; set; }
		[JsonProperty("selected_account")] public string SelectedAccount { get; set; }

		string storagePath;

		public event PropertyChangedEventHandler PropertyChanged;
		public static LauncherSettings Create(ExtMinecraftPath path)
		{
			var storagePath = Path.Combine(path.RootPath, "meloncher_settings.json");
			var jsonObj = "{}";
			if (File.Exists(storagePath)) jsonObj = File.ReadAllText(storagePath);
			var ls = JsonConvert.DeserializeObject<LauncherSettings>(jsonObj);
			ls.storagePath = storagePath;
			ls.PropertyChanged += (object sender, PropertyChangedEventArgs e) => { ls.SaveFile(); };
			return ls;
		}

		private void SaveFile()
		{
			var jsonStr = JsonConvert.SerializeObject(this);
			_ = File.WriteAllTextAsync(storagePath, jsonStr);
		}
	}

	public enum WindowMode
	{
		WINDOWED,
		FULLSCREEN,
		BORDERLESS
	}
}
