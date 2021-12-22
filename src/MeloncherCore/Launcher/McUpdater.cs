using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Installer;
using CmlLib.Core.Installer.FabricMC;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Launcher.Events;
using MeloncherCore.Optifine;
using MeloncherCore.Version;

namespace MeloncherCore.Launcher
{
	public class McUpdater
	{
		private readonly ExtMinecraftPath _minecraftPath;

		public McUpdater(ExtMinecraftPath minecraftPath)
		{
			_minecraftPath = minecraftPath;
		}

		public event McDownloadProgressEventHandler? McDownloadProgressChanged;

		public async Task<bool> UpdateMinecraft(McVersion mcVersion, bool fastLaunch)
		{
			var path = _minecraftPath.CloneWithProfile(mcVersion.ProfileType.ToString().ToLower(), mcVersion.ProfileName);
			var launcher = new CMLauncher(path);

			if (fastLaunch)
			{
				var mcJarPath = Path.Combine(path.Versions, mcVersion.VersionName, mcVersion.VersionName + ".jar");
				var mcJsonPath = Path.Combine(path.Versions, mcVersion.VersionName, mcVersion.VersionName + ".json");
				if (File.Exists(mcJarPath) && File.Exists(mcJsonPath)) return true;
			}

			var downloadProgressIsChecking = true;
			var downloadProgressPercentage = 0;
			var downloadProgressType = "unknown";

			launcher.FileChanged += args =>
			{
				var type = args.FileKind.ToString();
				if (downloadProgressType != type) downloadProgressPercentage = 0;

				downloadProgressType = type;
				if (downloadProgressPercentage == 0) downloadProgressIsChecking = true;
				McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
			};
			launcher.ProgressChanged += (_, args) =>
			{
				downloadProgressPercentage = args.ProgressPercentage;
				downloadProgressIsChecking = false;
				McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
			};

			try
			{
				var mVersion = await (await new DefaultVersionLoader(_minecraftPath).GetVersionMetadatasAsync()).GetVersionAsync(mcVersion.VersionName);
				await launcher.CheckAndDownloadAsync(mVersion);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> UpdateCustomClient(McVersion mcVersion)
		{
			var path = _minecraftPath.CloneWithProfile(mcVersion.ProfileType.ToString().ToLower(), mcVersion.ProfileName);
			var launcher = new CMLauncher(path);
			try
			{
				var mVersion = await (await new DefaultVersionLoader(_minecraftPath).GetVersionMetadatasAsync()).GetVersionAsync(mcVersion.VersionName);
				
				if (mcVersion.ClientType == McClientType.Optifine)
				{
					var optifineInstaller = new OptifineInstallerBobcat();
					optifineInstaller.ProgressChanged += (_, args) =>
					{
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs("Optifine", args.ProgressPercentage, false));
					};

					var isLatestInstalled = await optifineInstaller.IsLatestInstalled(mVersion.Id, _minecraftPath);
					if (!isLatestInstalled)
					{
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs("Optifine", 0, true));
						FixJavaBinaryPath(_minecraftPath, mVersion);
						await optifineInstaller.InstallOptifine(mVersion.Id, _minecraftPath, mVersion.JavaBinaryPath);
					}
				}
				if (mcVersion.ClientType == McClientType.Fabric)
				{
					var fabricVersionLoader = new FabricVersionLoader();
					var fabricVersions = await fabricVersionLoader.GetVersionMetadatasAsync();
					var fabricVersionMetadata = (from mVersionMetadata in fabricVersions where mVersionMetadata.Name.EndsWith(mcVersion.VersionName) select mVersionMetadata).FirstOrDefault();
					if (fabricVersionMetadata != null)
					{
						await fabricVersionMetadata.SaveAsync(path);
						await launcher.CheckAndDownloadAsync(await fabricVersionMetadata.GetVersionAsync());
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			
			return true;
		}
		
		public static void FixJavaBinaryPath(MinecraftPath path, MVersion version)
		{
			if (!string.IsNullOrEmpty(version.JavaBinaryPath) && File.Exists(version.JavaBinaryPath))
				return;

			var javaVersion = version.JavaVersion;
			if (string.IsNullOrEmpty(javaVersion))
				javaVersion = "jre-legacy";
			var bin = "bin";
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) bin = "jre.bundle/Contents/Home/bin";
			version.JavaBinaryPath = Path.Combine(path.Runtime, javaVersion, bin, MJava.GetDefaultBinaryName());
		}
	}
}