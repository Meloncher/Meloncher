﻿#nullable enable
using System;
using System.ComponentModel;
using System.Globalization;
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

		[JsonProperty("selected_profile_type")]
		public ProfileType SelectedProfileType { get; set; } = ProfileType.Vanilla;

		[JsonProperty("selected_account")] public string? SelectedAccount { get; set; }
		[JsonProperty("maximum_ram_mb")] public int MaximumRamMb { get; set; } = 1024;
		[JsonProperty("glass_background")] public bool GlassBackground { get; set; } = true;

		[JsonProperty("jvm_arguments")] public string JvmArguments { get; set; } = "-XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=16M";

		[JsonProperty("language")] public Language Language { get; set; } = GetSystemLang();
		[JsonProperty("hide_launcher")] public bool HideLauncher { get; set; } = true;
		[JsonProperty("fast_launcher")] public bool FastLaunch { get; set; } = false;

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

		private static Language GetSystemLang()
		{
			string sysLang = CultureInfo.InstalledUICulture.Name;
			if (sysLang.Equals("ru-RU")) return Language.Russian;
			if (sysLang.Equals("uk-UA")) return Language.Russian;
			return Language.English;
		}
	}

	public enum WindowMode
	{
		Windowed,
		Fullscreen,
		Borderless
	}

	public enum Language
	{
		English,
		Russian
	}
}