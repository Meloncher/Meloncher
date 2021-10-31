using System;
using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;

namespace MeloncherCore.Version
{
	public class VersionTools
	{
		private readonly MinecraftPath _minecraftPath;

		public VersionTools(MinecraftPath minecraftPath)
		{
			_minecraftPath = minecraftPath;
		}

		public MVersionCollection GetVersionMetadatas()
		{
			return new ExtVersionLoader(_minecraftPath).GetVersionMetadatas();
		}

		public MVersion? GetVersion(string versionName)
		{
			try
			{
				return GetVersionMetadatas().GetVersion(versionName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public McVersion? GetMcVersion(string versionName)
		{
			return GetMcVersion(GetVersion(versionName));
		}

		public McVersion? GetMcVersion(MVersion? mVersion)
		{
			if (mVersion == null) return null;

			var profileName = mVersion.AssetId;
			if (profileName == null)
			{
				var parentName = mVersion.ParentVersionId;
				if (parentName != null)
				{
					var parVer = GetVersion(parentName);
					if (parVer != null) profileName = parVer.AssetId;
				}
			}

			if (profileName != null)
			{
				var split = profileName.Split(".");
				if (split.Length == 3) profileName = split[0] + "." + split[1];
			}

			if (profileName == "pre-1.6") profileName = "legacy";
			profileName ??= "unknown";

			return new McVersion(mVersion, ProfileType.Vanilla, profileName);
		}
	}
}