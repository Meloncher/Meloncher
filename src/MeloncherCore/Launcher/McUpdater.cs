using System;
using System.Threading.Tasks;
using CmlLib.Core;
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

		public async Task<bool> Update(McVersion mcVersion, bool optifine)
		{
			if (mcVersion == null) return false;
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
				await launcher.CheckAndDownloadAsync(mcVersion.MVersion);

				if (optifine)
				{
					var optifineInstaller = new OptifineInstallerBobcat();
					optifineInstaller.ProgressChanged += (_, args) =>
					{
						downloadProgressPercentage = args.ProgressPercentage;
						downloadProgressIsChecking = false;
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
					};

					var isLatestInstalled = await optifineInstaller.IsLatestInstalled(mcVersion.MVersion.Id, _minecraftPath);
					if (!isLatestInstalled)
					{
						downloadProgressPercentage = 0;
						downloadProgressType = "Optifine";
						downloadProgressIsChecking = true;
						McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(downloadProgressType, downloadProgressPercentage, downloadProgressIsChecking));
						await optifineInstaller.InstallOptifine(mcVersion.MVersion.Id, _minecraftPath, mcVersion.MVersion.JavaBinaryPath);
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