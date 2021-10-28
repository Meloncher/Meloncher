using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Installer;
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
		private bool _downloadProgressIsChecking = true;
		private int _downloadProgressPercentage;

		private string _downloadProgressType = "unknown";
		public McProcess? McProcess;

		public McLauncher(ExtMinecraftPath minecraftPath)
		{
			_minecraftPath = minecraftPath;
		}

		public McVersion? Version { get; set; }

		public MSession Session { get; set; } = MSession.GetOfflineSession("Player");

		// public bool Offline { get; set; }
		public bool UseOptifine { get; set; } = true;
		public int MaximumRamMb { get; set; } = 2048;
		public WindowMode WindowMode { get; set; } = WindowMode.Windowed;

		public event McDownloadProgressEventHandler? McDownloadProgressChanged;

		private void InvokeMcDownloadProgressEvent()
		{
			McDownloadProgressChanged?.Invoke(new McDownloadProgressEventArgs(_downloadProgressType, _downloadProgressPercentage, _downloadProgressIsChecking));
		}

		public event MinecraftOutputEventHandler? MinecraftOutput;

		public void SetVersion(string mcVerName)
		{
			var verTools = new VersionTools(_minecraftPath);
			Version = verTools.GetMcVersion(mcVerName);
		}

		public void SetVersion(MVersion mVersion)
		{
			var verTools = new VersionTools(_minecraftPath);
			Version = verTools.GetMcVersion(mVersion);
		}

		public async Task<bool> Update()
		{
			if (Version == null) return false;
			var path = _minecraftPath.CloneWithProfile("vanilla", Version.ProfileName);
			var launcher = new CMLauncher(path);
			launcher.FileChanged += args =>
			{
				var type = args.FileKind.ToString();
				if (_downloadProgressType != type) _downloadProgressPercentage = 0;

				_downloadProgressType = type;
				if (_downloadProgressPercentage == 0) _downloadProgressIsChecking = true;
				InvokeMcDownloadProgressEvent();
			};
			launcher.ProgressChanged += (_, args) =>
			{
				_downloadProgressPercentage = args.ProgressPercentage;
				_downloadProgressIsChecking = false;
				InvokeMcDownloadProgressEvent();
			};

			try
			{
				await launcher.CheckAndDownloadAsync(Version.MVersion);

				if (UseOptifine)
				{
					var optifineInstaller = new OptifineInstallerBobcat();
					optifineInstaller.ProgressChanged += (sender, args) =>
					{
						_downloadProgressPercentage = args.ProgressPercentage;
						_downloadProgressIsChecking = false;
						InvokeMcDownloadProgressEvent();
					};

					var isLatestInstalled = await optifineInstaller.IsLatestInstalled(Version.Name, _minecraftPath);
					if (!isLatestInstalled)
					{
						_downloadProgressPercentage = 0;
						_downloadProgressType = "Optifine";
						_downloadProgressIsChecking = true;
						InvokeMcDownloadProgressEvent();
						await optifineInstaller.InstallOptifine(Version.Name, _minecraftPath, Version.MVersion.JavaBinaryPath);
					}
				}
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public async Task<bool> Launch()
		{
			if (Version == null) return false;
			var path = _minecraftPath.CloneWithProfile("vanilla", Version.ProfileName);
			var launcher = new CMLauncher(path)
			{
				VersionLoader = new LocalVersionLoader(_minecraftPath),
				FileDownloader = null
			};
			var launchOption = new MLaunchOption
			{
				StartVersion = Version.MVersion,
				MaximumRamMb = MaximumRamMb,
				Session = Session,
				VersionType = "Meloncher",
				GameLauncherName = "Meloncher"
			};

			if (WindowMode == WindowMode.Fullscreen) launchOption.FullScreen = true;

			var sync = new McOptionsSync(path);
			FixJavaBinaryPath(_minecraftPath, launchOption.StartVersion);

			if (UseOptifine)
			{
				var optifineInstaller = new OptifineInstallerBobcat();
				var ofVerName = optifineInstaller.GetLatestInstalled(Version.Name, _minecraftPath);
				if (ofVerName != null)
				{
					launchOption.StartVersion = await (await launcher.VersionLoader.GetVersionMetadatasAsync()).GetVersionAsync(ofVerName);
					FixJavaBinaryPath(_minecraftPath, launchOption.StartVersion);
				}
			}

			McProcess = new McProcess(await launcher.CreateProcessAsync(launchOption));
			McProcess.MinecraftOutput += args => MinecraftOutput?.Invoke(args);
			sync.Load();
			McProcess.Start();
			if (WindowMode == WindowMode.Borderless)
			{
				var wt = new WindowTweaks(McProcess.Process);
				_ = wt.Borderless();
			}

			await McProcess.WaitForExitAsync();
			sync.Save();
			return true;
		}

		private void FixJavaBinaryPath(MinecraftPath path, MVersion version)
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