using CmlLib.Core;
using MeloncherCore.Launcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Options
{
	class Sync
	{
		private string profileOpts;
		private string syncOpts;
		private string[] noSyncKeys = new string[] { "resourcePacks", "incompatibleResourcePacks", "skin" };

		public Sync(ExtMinecraftPath path)
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
