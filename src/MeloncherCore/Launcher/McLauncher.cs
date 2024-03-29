﻿using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Launcher.Events;
using MeloncherCore.Optifine;
using MeloncherCore.Options;
using MeloncherCore.Settings;
using MeloncherCore.Version;

namespace MeloncherCore.Launcher
{
	public class McLauncher
	{
		private readonly ExtMinecraftPath _minecraftPath;
		private readonly LauncherSettings _launcherSettings;
		public McProcess? McProcess;

		public McLauncher(ExtMinecraftPath minecraftPath, LauncherSettings launcherSettings)
		{
			_minecraftPath = minecraftPath;
			_launcherSettings = launcherSettings;
		}

		public MSession Session { get; set; } = MSession.GetOfflineSession("Player");
		public event MinecraftOutputEventHandler? MinecraftOutput;

		public async Task<bool> Launch(McVersion mcVersion)
		{
			var path = _minecraftPath.CloneWithProfile(mcVersion.ProfileType.ToString().ToLower(), mcVersion.ProfileName);
			
			var mcJarPath = Path.Combine(path.Versions, mcVersion.VersionName, mcVersion.VersionName + ".jar");
			var mcJsonPath = Path.Combine(path.Versions, mcVersion.VersionName, mcVersion.VersionName + ".json");
			if (!File.Exists(mcJarPath) || !File.Exists(mcJsonPath)) return false;
			
			var launcher = new CMLauncher(path)
			{
				VersionLoader = new LocalVersionLoader(_minecraftPath),
				FileDownloader = null
			};
			MVersion mVersion = await launcher.GetVersionAsync(mcVersion.VersionName);
			var args = new List<string>
			{
				"-Xmx" + _launcherSettings.MaximumRamMb + "m",
				"-Xms" + _launcherSettings.MaximumRamMb + "m",
				_launcherSettings.JvmArguments
			};
			var launchOption = new MLaunchOption
			{
				StartVersion = mVersion,
				// MaximumRamMb = MaximumRamMb,
				JVMArguments = args.ToArray(),
				Session = Session,
				VersionType = "Meloncher",
				GameLauncherName = "Meloncher"
			};

			if (_launcherSettings.WindowMode == WindowMode.Fullscreen) launchOption.FullScreen = true;

			var sync = new McOptionsSync(path, _launcherSettings);
			McUpdater.FixJavaBinaryPath(_minecraftPath, launchOption.StartVersion);
			var javaBinaryPath = launchOption.StartVersion.JavaBinaryPath;

			if (mcVersion.ClientType == McClientType.Optifine)
			{
				var optifineInstaller = new OptifineInstallerBobcat();
				var ofVerName = optifineInstaller.GetLatestInstalled(mVersion.Id, _minecraftPath);
				if (ofVerName != null)
				{
					var ofJarPath = Path.Combine(path.Versions, ofVerName, ofVerName + ".jar");
					if (!File.Exists(ofJarPath) && File.Exists(mcJarPath)) File.Copy(mcJarPath, ofJarPath);
					launchOption.StartVersion = await launcher.GetVersionAsync(ofVerName);
					McUpdater.FixJavaBinaryPath(_minecraftPath, launchOption.StartVersion);
				}
			}
			if (mcVersion.ClientType == McClientType.Fabric)
			{
				var versionMetadatas = await launcher.VersionLoader.GetVersionMetadatasAsync();
				string? fabricVersionName = null;
				System.Version? fabricLoaderVersion = null;
				foreach (var mVersionMetadata in versionMetadatas)
				{
					var match = Regex.Match(mVersionMetadata.Name, "fabric-loader-(?<fabricVersion>[0-9]*\\.[0-9]*\\.[0-9]*)-(?<minecraftVersion>.*)");
					if (!match.Success || !string.Equals(mcVersion.VersionName, match.Groups["minecraftVersion"].Value)) continue;
					System.Version loaderVersion = System.Version.Parse(match.Groups["fabricVersion"].Value);
					if (loaderVersion.CompareTo(fabricLoaderVersion) <= 0) continue;
					fabricVersionName = mVersionMetadata.Name;
					fabricLoaderVersion = loaderVersion;
				}
				if (!string.IsNullOrEmpty(fabricVersionName))
				{
					var fabricJarPath = Path.Combine(path.Versions, fabricVersionName, fabricVersionName + ".jar");
					if (!File.Exists(fabricJarPath) && File.Exists(mcJarPath)) File.Copy(mcJarPath, fabricJarPath);
					launchOption.StartVersion = await (await launcher.VersionLoader.GetVersionMetadatasAsync()).GetVersionAsync(fabricVersionName);
					McUpdater.FixJavaBinaryPath(_minecraftPath, launchOption.StartVersion);
				}
			}

			McProcess = new McProcess(await launcher.CreateProcessAsync(launchOption));
			McProcess.MinecraftOutput += args => MinecraftOutput?.Invoke(args);
			if (mcVersion.ProfileType == ProfileType.Vanilla) sync.Load();
			McProcess.Start();
			if (_launcherSettings.WindowMode == WindowMode.Borderless)
			{
				var wt = new WindowTweaks(McProcess.Process);
				_ = wt.Borderless();
			}

			await McProcess.WaitForExitAsync();
			if (mcVersion.ProfileType == ProfileType.Vanilla) sync.Save();
			return true;
		}
	}
}