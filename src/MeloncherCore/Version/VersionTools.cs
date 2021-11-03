using System;
using CmlLib.Core;
using CmlLib.Core.Version;
using MeloncherCore.ModPack;

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
			var mVersion = GetVersion(versionName);
			return mVersion != null ? GetMcVersion(mVersion) : null;
		}

		public McVersion? GetMcVersion(string? modPackName, ModPackStorage modPackStorage)
		{
			if (string.IsNullOrEmpty(modPackName)) return null;
			if (!modPackStorage.ContainsKey(modPackName)) return null;
			var mVersion = GetVersion(modPackStorage[modPackName].VersionName);
			return mVersion != null ? new McVersion(modPackName, mVersion, ProfileType.Custom, modPackName) : null;
		}

		public McVersion GetMcVersion(MVersion mVersion)
		{
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

			return new McVersion(mVersion.Id, mVersion, ProfileType.Vanilla, profileName);
		}
	}
}