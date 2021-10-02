using MeloncherCore.Launcher;
using System.IO;
using System.Text;

namespace MeloncherCore.Options
{
	class McOptionsSync
	{
		private string profileOpts;
		private string syncOpts;
		private string[] noSyncKeys = new string[] { "resourcePacks", "incompatibleResourcePacks", "skin" };

		public McOptionsSync(ExtMinecraftPath path)
		{
			profileOpts = Path.Combine(path.BasePath, "options.txt");
			syncOpts = Path.Combine(path.RootPath, "options.txt");
		}
		public void Save()
		{
			if (!File.Exists(syncOpts))
				File.WriteAllText(syncOpts, "", Encoding.Default);

			if (File.Exists(profileOpts)) {
				ExtGameOptionsFile sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				ExtGameOptionsFile options = ExtGameOptionsFile.ReadFile(profileOpts, Encoding.Default);
				sync.Merge(options);
				foreach (string key in noSyncKeys)
				{
					sync.SetRawValue(key, null);
				}
				sync.Downgrade();
				sync.Save(syncOpts, Encoding.Default);
			}
			
		}
		public void Load()
		{
			if (!File.Exists(syncOpts)) {
				File.WriteAllText(syncOpts, "", Encoding.Default);
				ExtGameOptionsFile sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				McOptionsUtils.SetDefaults(sync);
				sync.Save(syncOpts, Encoding.Default);
			}

			if (File.Exists(syncOpts))
			{
				ExtGameOptionsFile sync = ExtGameOptionsFile.ReadFile(syncOpts, Encoding.Default);
				if (File.Exists(profileOpts))
				{
					ExtGameOptionsFile options = ExtGameOptionsFile.ReadFile(profileOpts, Encoding.Default);
					foreach (string key in noSyncKeys) {
						if (options.ContainsKey(key)) sync.SetRawValue(key, options.GetRawValue(key));
					}
				}
				sync.Save(profileOpts, Encoding.Default);
			}
		}
	}
}
