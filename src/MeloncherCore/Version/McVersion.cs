using CmlLib.Core.Version;

namespace MeloncherCore.Version
{
	public class McVersion
	{
		public McVersion(string name, string type, string profileName, MVersion mVersion)
		{
			Name = name;
			Type = type;
			ProfileName = profileName;
			MVersion = mVersion;
		}

		public string Name { get; set; }
		public string Type { get; set; }
		public string ProfileName { get; set; }

		public MVersion MVersion { get; set; }
	}
}