using CmlLib.Core.Version;

namespace MeloncherCore.Version
{
	public class McVersion
	{
		public McVersion(MVersion mVersion, ProfileType profileType, string profileName)
		{
			// Name = name;
			// VersionType = versionType;
			MVersion = mVersion;
			ProfileType = profileType;
			ProfileName = profileName;
		}

		public MVersion MVersion { get; set; }

		// public string Name { get; set; }
		// public string VersionType { get; set; }
		public ProfileType ProfileType { get; set; }
		public string ProfileName { get; set; }
	}

	public enum ProfileType
	{
		Vanilla,
		Custom
	}
}