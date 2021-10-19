using System;
using System.IO;
using System.Text;
using CmlLib.Core;
using MeloncherCore.Launcher;

namespace MeloncherCore.Options
{
	public class McOptionsImporter
	{
		private readonly string[] _gameOptionsNoSyncKeys = {"resourcePacks", "incompatibleResourcePacks", "skin"};
		private readonly string[] _optifineOptionsNoSyncKeys = Array.Empty<string>();
		private readonly string _importGameOptionsPath;
		private readonly string _importOptifineOptionsPath;
		private readonly string _launcherGameOptionsPath;
		private readonly string _launcherOptifineOptionsPath;

		public McOptionsImporter(ExtMinecraftPath path)
		{
			_importGameOptionsPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "options.txt");
			_importOptifineOptionsPath = Path.Combine(MinecraftPath.GetOSDefaultPath(), "optionsof.txt");
			_launcherGameOptionsPath = Path.Combine(path.RootPath, "options.txt");
			_launcherOptifineOptionsPath = Path.Combine(path.RootPath, "optionsof.txt");
		}

		public void Import()
		{
			save(_launcherGameOptionsPath, _importGameOptionsPath, _gameOptionsNoSyncKeys);
			save(_launcherOptifineOptionsPath, _importOptifineOptionsPath, _optifineOptionsNoSyncKeys);
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
	}
}