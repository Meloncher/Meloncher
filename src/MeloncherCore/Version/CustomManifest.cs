using System.Collections.Generic;
using CmlLib.Core.VersionLoader;

namespace MeloncherCore.Version
{
	internal class CustomManifest
	{
		private readonly IVersionLoader versionLoader;

		public CustomManifest(IVersionLoader versionLoader)
		{
			this.versionLoader = versionLoader;
		}

		public Dictionary<string, List<McVersion>> Test()
		{
			var metas = versionLoader.GetVersionMetadatas();
			var list = new Dictionary<string, List<McVersion>>();
			foreach (var meta in metas)
			{
				if (!list.ContainsKey(meta.MType.ToString())) list.Add(meta.MType.ToString(), new List<McVersion>());
				var profileType = meta.MType.ToString();
				if (profileType == "Release")
				{
					var spl = meta.Name.Split(".");
					if (spl.Length >= 2) profileType = "Release-" + spl[0] + "." + spl[1];
				}

				list[meta.MType.ToString()].Add(new McVersion(meta.Name, meta.MType.ToString(), profileType));
			}

			return list;
		}
	}
}