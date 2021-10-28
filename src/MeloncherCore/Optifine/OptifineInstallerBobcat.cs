using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Optifine.Bobcat;
using Newtonsoft.Json.Linq;

namespace MeloncherCore.Optifine
{
	internal class OptifineInstallerBobcat
	{
		private const string OptifineBmclUrl = "https://bmclapi2.bangbang93.com/optifine/versionlist/";
		public event ProgressChangedEventHandler ProgressChanged;

		private OptifineDownloadVersionModel?[] ParseVersions(string res)
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

		private async Task<OptifineDownloadVersionModel?[]> ParseVersions()
		{
			string res;
			using (var wc = new WebClient())
			{
				res = await wc.DownloadStringTaskAsync(OptifineBmclUrl);
			}

			return ParseVersions(res);
		}

		private string getPatch(OptifineDownloadVersionModel? ofVer)
		{
			if (ofVer.Patch.StartsWith("pre"))
				return ofVer.Type;
			return ofVer.Type + "_" + ofVer.Patch;
		}

		private int getPre(OptifineDownloadVersionModel? ofVer)
		{
			if (ofVer.Patch.StartsWith("pre"))
				return int.Parse(ofVer.Patch.Replace("pre", ""));
			return 1337;
		}

		private OptifineDownloadVersionModel? GetLatestOptifineVersion(OptifineDownloadVersionModel?[] ofVers, string mcVersionName)
		{
			OptifineDownloadVersionModel? latestOfVer = null;
			foreach (var ofVer in ofVers)
				if (ofVer?.McVersion == mcVersionName || ofVer?.McVersion == mcVersionName + ".0")
					if (latestOfVer == null || IsNewer(getPatch(latestOfVer), getPatch(ofVer), getPre(latestOfVer), getPre(ofVer)))
						latestOfVer = ofVer;
			return latestOfVer;
		}

		private async Task<OptifineDownloadVersionModel?> GetLatestOptifineVersion(string mcVersionName)
		{
			var versions = await ParseVersions().ConfigureAwait(false);
			return GetLatestOptifineVersion(versions, mcVersionName);
		}

		private bool IsNewer(string patch, string patchNew, int pre, int preNew)
		{
			var comparePatch = string.CompareOrdinal(patchNew, patch);
			if (comparePatch == 1) return true;
			if (comparePatch == 0 && preNew > pre) return true;
			return false;
		}

		public string? GetLatestInstalled(string mcVersionName, MinecraftPath path)
		{
			var versionLoader = new LocalVersionLoader(path);
			string latestName = null;
			string latestPatch = null;
			var latestPre = -1;
			foreach (var mtd in versionLoader.GetVersionMetadatas())
			{
				var name = mtd.Name;
				if (name.StartsWith(mcVersionName + "-Optifine_"))
				{
					var nameSplit = name.Split("_");
					if (nameSplit.Length == 4 || nameSplit.Length == 5)
					{
						var patch = nameSplit[1] + "_" + nameSplit[2] + "_" + nameSplit[3];
						var pre = 1337;
						if (nameSplit.Length == 5) pre = int.Parse(nameSplit[4].Replace("pre", ""));
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

		public async Task<bool> IsLatestInstalled(string mcVersionName, MinecraftPath path)
		{
			var latest = await GetLatestOptifineVersion(mcVersionName);
			if (latest == null) return false;
			var name = mcVersionName + "-Optifine_" + latest.Type + "_" + latest.Patch;
			var versionLoader = new LocalVersionLoader(path);
			return (await versionLoader.GetVersionMetadatasAsync()).Any(mtd => mtd.Name == name);
		}

		public async Task<string?> InstallOptifine(string mcVersionName, MinecraftPath path, string javaPath)
		{
			var latest = await GetLatestOptifineVersion(mcVersionName).ConfigureAwait(false);
			if (latest == null) return null;
			return await InstallOptifine(latest, path, javaPath).ConfigureAwait(false);
		}

		public async Task<string> InstallOptifine(OptifineDownloadVersionModel ofVer, MinecraftPath path, string javaPath)
		{
			var installerPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			using (var wc = new WebClient())
			{
				var url = new Uri("https://optifine.net/download?f=" + ofVer.FileName);
				wc.DownloadProgressChanged += (sender, e) => { ProgressChanged?.Invoke(sender, e); };
				await wc.DownloadFileTaskAsync(url, installerPath);
			}

			var ofInstaller = new OptifineInstaller();
			ofInstaller.OptifineJarPath = installerPath;
			ofInstaller.OptifineDownloadVersion = ofVer;
			ofInstaller.JavaExecutablePath = javaPath;
			ofInstaller.MinecraftPath = path;
			return ofInstaller.Install();
		}
	}
}