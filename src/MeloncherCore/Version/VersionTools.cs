using CmlLib.Core;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Version
{
	public class VersionTools
	{

		IVersionLoader VersionLoader { get; set; }
		public VersionTools(IVersionLoader versionLoader)
		{
			VersionLoader = versionLoader;
		}

		public McVersion getMcVersionByName(string mcVerName)
		{
			MVersion mVersion = VersionLoader.GetVersionMetadatas().GetVersion(mcVerName);
			var assid = mVersion.AssetId;
			var type = mVersion.TypeStr;

			var profileName = assid;
			if (profileName == "pre-1.6") profileName = "legacy";

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


			return new McVersion(mcVerName, type, profileName);
		} 
	}
}
