using System;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;

namespace MeloncherCore.Version
{
	public class ExtVersionLoader : IVersionLoader
	{
		private readonly MinecraftPath _minecraftPath;

		public ExtVersionLoader(MinecraftPath path)
		{
			_minecraftPath = path;
		}

		public MVersionCollection GetVersionMetadatas()
		{
			var localVersionLoader = new LocalVersionLoader(_minecraftPath);

			var localVersions = localVersionLoader.GetVersionMetadatas();

			try
			{
				var mojangVersionLoader = new MojangVersionLoader();
				var mojangVersions = mojangVersionLoader.GetVersionMetadatas();
				localVersions.Merge(mojangVersions);
			}
			catch (Exception)
			{
				// ignored
			}

			return localVersions;
		}

		public async Task<MVersionCollection> GetVersionMetadatasAsync()
		{
			var localVersionLoader = new LocalVersionLoader(_minecraftPath);

			var localVersions = await localVersionLoader.GetVersionMetadatasAsync().ConfigureAwait(false);

			try
			{
				var mojangVersionLoader = new MojangVersionLoader();
				var mojangVersions = await mojangVersionLoader.GetVersionMetadatasAsync().ConfigureAwait(false);
				localVersions.Merge(mojangVersions);
			}
			catch (Exception)
			{
				// ignored
			}

			return localVersions;
		}
	}
}