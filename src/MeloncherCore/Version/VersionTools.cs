using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;

namespace MeloncherCore.Version
{
	public class VersionTools
	{
		public VersionTools(IVersionLoader versionLoader)
		{
			VersionLoader = versionLoader;
		}

		private IVersionLoader VersionLoader { get; }

		public McVersion getMcVersion(string versionName)
		{
			var mVersion = VersionLoader.GetVersionMetadatas().GetVersion(versionName);
			return getMcVersion(mVersion);
		}

		public McVersion getMcVersion(MVersion mVersion)
		{
			var assid = mVersion.AssetId;
			var type = mVersion.TypeStr;

			var profileName = assid;
			if (profileName == "pre-1.6") profileName = "legacy";
			if (profileName == null)
			{
				var parentName = mVersion.ParentVersionId;
				if (parentName != null)
				{
					var parVer = VersionLoader.GetVersionMetadatas().GetVersion(parentName);
					profileName = parVer.AssetId;
				}
			}

			if (profileName == null) profileName = "unknown";
			//System.Console.WriteLine("Profile name = " + profileName.ToString());

			//if (type == "release")
			//{
			//	var verSplit = mcVerName.Split(".");
			//	if (verSplit.Length >= 2 && verSplit[0] == "1")
			//	{
			//		profileName = "1." + verSplit[1];
			//	}
			//} else if (type != "snapshot")
			//{
			//	profileName = "other";
			//}

			return new McVersion(mVersion.Id, type, profileName);
		}
	}
}