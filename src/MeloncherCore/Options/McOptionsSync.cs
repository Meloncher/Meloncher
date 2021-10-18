using System.IO;
using System.Text;
using MeloncherCore.Launcher;

namespace MeloncherCore.Options
{
	internal class McOptionsSync
	{
		private readonly string[] noSyncKeys = {"resourcePacks", "incompatibleResourcePacks", "skin"};
		private readonly string profileOpts;
		private readonly string syncOpts;

		public McOptionsSync(ExtMinecraftPath path)
		{
			profileOpts = Path.Combine(path.BasePath, "options.txt");
			syncOpts = Path.Combine(path.RootPath, "options.txt");
		}

		public void Save()
		{
			if (!File.Exists(syncOpts))
				File.WriteAllText(syncOpts, "", Encoding.Default);

			if (File.Exists(profileOpts))
			{
				var sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				var options = ExtGameOptionsFile.ReadFile(profileOpts, Encoding.Default);
				sync.Merge(options);
				foreach (var key in noSyncKeys) sync.SetRawValue(key, null);
				sync.Downgrade();
				sync.Save(syncOpts, Encoding.Default);
			}
		}

		public void Load()
		{
			if (!File.Exists(syncOpts))
			{
				File.WriteAllText(syncOpts, "", Encoding.Default);
				var sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				McOptionsUtils.SetDefaults(sync);
				sync.Save(syncOpts, Encoding.Default);
			}

			if (File.Exists(syncOpts))
			{
				var sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				if (File.Exists(profileOpts))
				{
					var options = ExtGameOptionsFile.ReadFile(profileOpts, Encoding.Default);
					foreach (var key in noSyncKeys)
						if (options.ContainsKey(key))
							sync.SetRawValue(key, options.GetRawValue(key));
				}

				sync.Save(profileOpts, Encoding.Default);
			}
		}
	}
}