using CmlLib.Core.Version;
using MeloncherCore.Launcher;

namespace MeloncherCore.Version
{
	public class McVersion
	{
		public McVersion(string name, string versionName, McClientType clientType, ProfileType profileType, string profileName)
		{
			Name = name;
			VersionName = versionName;
			ClientType = clientType;
			ProfileType = profileType;
			ProfileName = profileName;
		}
		
		public string Name { get; set; }
		public string VersionName { get; set; }

		public McClientType ClientType { get; set; }
		public ProfileType ProfileType { get; set; }
		public string ProfileName { get; set; }
	}
}