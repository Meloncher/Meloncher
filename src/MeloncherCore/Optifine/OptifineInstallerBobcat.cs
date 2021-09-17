using CmlLib.Core.VersionLoader;
using MeloncherCore.Launcher;
using MeloncherCore.Optifine.Bobcat;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MeloncherCore.Optifine
{
	class OptifineInstallerBobcat
	{
		private const string OptifineBmclUrl = "https://bmclapi2.bangbang93.com/optifine/versionlist/";
		private OptifineDownloadVersionModel[] ParseVersions(string res)
		{
			var jarr = JArray.Parse(res);
			var ofVerList = new List<OptifineDownloadVersionModel>(jarr.Count);
			foreach (var item in jarr)
			{
				var obj = item.ToObject<OptifineDownloadVersionModel>();
				if (obj != null)
					ofVerList.Add(obj);
			}
			return ofVerList.ToArray();
		}
		private async Task<OptifineDownloadVersionModel[]> ParseVersions()
		{
			string res;
			using (var wc = new WebClient())
			{
				res = await wc.DownloadStringTaskAsync(OptifineBmclUrl);
			}
			return ParseVersions(res);
		}
		
		private string getPatch(OptifineDownloadVersionModel ofVer)
		{
			if (ofVer.Patch.StartsWith("pre")) 
				return ofVer.Type;
			else 
				return ofVer.Type + "_" + ofVer.Patch;
		}

		private int getPre(OptifineDownloadVersionModel ofVer)
		{
			if (ofVer.Patch.StartsWith("pre")) 
				return Int32.Parse(ofVer.Patch.Replace("pre", ""));
			else 
				return 1337;
		}

		private OptifineDownloadVersionModel? GetLatestOptifineVersion(OptifineDownloadVersionModel[] ofVers, string mcVersionName)
		{
			OptifineDownloadVersionModel latestOfVer = null;
			foreach (var ofVer in ofVers)
			{
				if (ofVer.McVersion == mcVersionName || ofVer.McVersion == mcVersionName + ".0")
				{
					if (latestOfVer == null || IsNewer(getPatch(latestOfVer), getPatch(ofVer), getPre(latestOfVer), getPre(ofVer))) latestOfVer = ofVer;
				}
			}
			return latestOfVer;
		}

		private async Task<OptifineDownloadVersionModel?> GetLatestOptifineVersion(string mcVersionName) {
			var versions = await ParseVersions();
			return GetLatestOptifineVersion(versions, mcVersionName);
		}

		private bool IsNewer(string patch, string patchNew, int pre, int preNew)
		{
			int comparePatch = String.Compare(patchNew, patch);
			if (comparePatch == 1) return true;
			else if (comparePatch == 0 && preNew > pre) return true;
			return false;
		}

		public string? GetLatestInstalled(string mcVersionName, ExtMinecraftPath path)
		{
			var versionLoader = new LocalVersionLoader(path);
			string latestName = null;
			string latestPatch = null;
			int latestPre = -1;
			foreach (var mtd in versionLoader.GetVersionMetadatas())
			{
				var name = mtd.Name;
				if (name.StartsWith(mcVersionName + "-Optifine_")) {
					var nameSplit = name.Split("_");
					if (nameSplit.Length == 4 || nameSplit.Length == 5)
					{
						var patch = nameSplit[1] + "_" + nameSplit[2] + "_" + nameSplit[3];
						int pre = 1337;
						if (nameSplit.Length == 5) pre = Int32.Parse(nameSplit[4].Replace("pre", ""));
						if (latestName == null || IsNewer(latestPatch, patch, latestPre, pre))
						{
							latestName = name;
							latestPatch = patch;
							latestPre = pre;
						}
					}
				}
			}
			return latestName;
		}

		public async Task<string> IsLatestInstalled(string mcVersionName, ExtMinecraftPath path)
		{
			var latest = await GetLatestOptifineVersion(mcVersionName);
			var name = mcVersionName + "-Optifine_" + latest.Type + "_" + latest.Patch;
			var versionLoader = new LocalVersionLoader(path);
			foreach (var mtd in versionLoader.GetVersionMetadatas())
			{
				if (mtd.Name == name) return name;
			}
			return null;
		}

		public async Task<string> installOptifine(string mcVersionName, ExtMinecraftPath path, string javaPath)
		{
			var latest = await GetLatestOptifineVersion(mcVersionName);
			Console.WriteLine(latest.FileName);
			return await installOptifine(latest, path, javaPath);
		}
		public async Task<string> installOptifine(OptifineDownloadVersionModel ofVer, ExtMinecraftPath path, string javaPath)
		{
			var installerPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			using (var wc = new WebClient())
			{
				var url = new Uri("https://optifine.net/download?f=" + ofVer.FileName);
				wc.DownloadProgressChanged += (sender, e) => {
					Console.WriteLine("Крч джарник оптифайна качается: " + e.ProgressPercentage + "%");
				};
				await wc.DownloadFileTaskAsync(url, installerPath);
				Console.WriteLine("Ура Оптифайн сканчался!");
			}

			var ofInstaller = new OptifineInstaller();
			ofInstaller.OptifineJarPath = installerPath;
			ofInstaller.OptifineDownloadVersion = ofVer;
			ofInstaller.JavaExecutablePath = javaPath;
			//ofInstaller.RootPath = path.MinecraftPath;
			ofInstaller.MinecraftPath = path; 
			return ofInstaller.Install();
		}
	}
}
