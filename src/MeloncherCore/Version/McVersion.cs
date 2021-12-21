using CmlLib.Core.Version;

namespace MeloncherCore.Version
{
	public class McVersion
	{
		public McVersion(string name, string versionName, ProfileType profileType, string profileName)
		{
			Name = name;
			VersionName = name;
			ProfileType = profileType;
			ProfileName = profileName;
		}

		public string VersionName { get; set; }

		public string Name { get; set; }

		// public string Name { get; set; }
		// public string VersionType { get; set; }
		public ProfileType ProfileType { get; set; }
		public string ProfileName { get; set; }
	}
}