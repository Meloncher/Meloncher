using System;
using System.Linq;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Installer.FabricMC;
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

		public async Task<bool> Update(McVersion mcVersion, McClientType mcClientType)
		{
			var path = _minecraftPath.CloneWithProfile(mcVersion.ProfileType.ToString().ToLower(), mcVersion.ProfileName);
			var launcher = new CMLauncher(path);

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

				if (mcClientType == McClientType.Optifine)
				{
					var optifineInstaller = new OptifineInstallerBobcat();
					optifineInstaller.ProgressChanged += (_, args) =>
					{
						downloadProgressPercentage = args.ProgressPercentage;
						downloadProgressIsChecking = false;
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
					};

					var isLatestInstalled = await optifineInstaller.IsLatestInstalled(mVersion.Id, _minecraftPath);
					if (!isLatestInstalled)
					{
						downloadProgressPercentage = 0;
						downloadProgressType = "Optifine";
						downloadProgressIsChecking = true;
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
						await optifineInstaller.InstallOptifine(mVersion.Id, _minecraftPath, mVersion.JavaBinaryPath);
					}
				}

				if (mcClientType == McClientType.Fabric)
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
	}
}