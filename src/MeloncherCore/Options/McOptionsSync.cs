using System;
using System.IO;
using System.Text;
using MeloncherCore.Launcher;
using MeloncherCore.Settings;

namespace MeloncherCore.Options
{
	internal class McOptionsSync
	{
		private readonly string[] _gameOptionsNoSyncKeys = {"resourcePacks", "incompatibleResourcePacks", "skin"};
		private readonly string _launcherGameOptionsPath;
		private readonly string _launcherOptifineOptionsPath;
		private readonly string[] _optifineOptionsNoSyncKeys = Array.Empty<string>();
		private readonly string _profileGameOptionsPath;
		private readonly string _profileOptifineOptionsPath;
		private readonly LauncherSettings? _launcherSettings;

		public McOptionsSync(ExtMinecraftPath path, LauncherSettings? launcherSettings)
		{
			_profileGameOptionsPath = Path.Combine(path.BasePath, "options.txt");
			_launcherGameOptionsPath = Path.Combine(path.RootPath, "options.txt");
			_profileOptifineOptionsPath = Path.Combine(path.BasePath, "optionsof.txt");
			_launcherOptifineOptionsPath = Path.Combine(path.RootPath, "optionsof.txt");
			_launcherSettings = launcherSettings;
		}

		private void save(string launcherOptionsPath, string profileOptionsPath, string[] noSyncKeys)
		{
			if (!File.Exists(launcherOptionsPath))
				File.WriteAllText(launcherOptionsPath, "", Encoding.Default);

			if (File.Exists(profileOptionsPath))
			{
				var sync = ExtGameOptionsFile.ReadFile(launcherOptionsPath, Encoding.Default);
				var options = ExtGameOptionsFile.ReadFile(profileOptionsPath, Encoding.Default);
				sync.Merge(options);
				foreach (var key in noSyncKeys) sync.SetRawValue(key, null!);
				sync.Downgrade();
				sync.Save(launcherOptionsPath, Encoding.Default);
			}
		}

		private void load(string launcherOptionsPath, string profileOptionsPath, string[] noSyncKeys)
		{
			if (!File.Exists(launcherOptionsPath))
			{
				File.WriteAllText(launcherOptionsPath, "", Encoding.Default);
				var sync = ExtGameOptionsFile.ReadFile(launcherOptionsPath, Encoding.Default);
				McOptionsUtils.SetDefaults(sync, _launcherSettings);
				sync.Save(launcherOptionsPath, Encoding.Default);
			}

			if (File.Exists(launcherOptionsPath))
			{
				var sync = ExtGameOptionsFile.ReadFile(launcherOptionsPath, Encoding.Default);
				if (File.Exists(profileOptionsPath))
				{
					var options = ExtGameOptionsFile.ReadFile(profileOptionsPath, Encoding.Default);
					foreach (var key in noSyncKeys)
						if (options.ContainsKey(key))
							sync.SetRawValue(key, options.GetRawValue(key)!);
				}

				sync.Save(profileOptionsPath, Encoding.Default);
			}
		}

		public void Save()
		{
			save(_launcherGameOptionsPath, _profileGameOptionsPath, _gameOptionsNoSyncKeys);
			save(_launcherOptifineOptionsPath, _profileOptifineOptionsPath, _optifineOptionsNoSyncKeys);
		}

		public void Load()
		{
			load(_launcherGameOptionsPath, _profileGameOptionsPath, _gameOptionsNoSyncKeys);
			load(_launcherOptifineOptionsPath, _profileOptifineOptionsPath, _optifineOptionsNoSyncKeys);
		}
	}
}