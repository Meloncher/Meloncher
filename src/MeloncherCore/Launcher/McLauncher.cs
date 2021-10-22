using System;
using System.ComponentModel;
using System.IO;
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
		public McLauncher(ExtMinecraftPath minecraftPath)
		{
			_minecraftPath = minecraftPath;
		}

		private readonly ExtMinecraftPath _minecraftPath;

		public McVersion? Version { get; set; }
		public MSession Session { get; set; } = MSession.GetOfflineSession("Player");
		// public bool Offline { get; set; }
		public bool UseOptifine { get; set; } = true;
		public int MaximumRamMb { get; set; } = 2048;
		public WindowMode WindowMode { get; set; } = WindowMode.Windowed;

		public event McDownloadFileChangedHandler? FileChanged;
		public event ProgressChangedEventHandler? ProgressChanged;
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
			launcher.FileChanged += e => FileChanged?.Invoke(new McDownloadFileChangedEventArgs(e.FileKind.ToString()));
			launcher.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e);

			try
			{
				await launcher.CheckAndDownloadAsync(Version.MVersion);

				if (UseOptifine)
				{
					var optifineInstaller = new OptifineInstallerBobcat();
					optifineInstaller.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e);

					var isLatestInstalled = await optifineInstaller.IsLatestInstalled(Version.Name, _minecraftPath);
					if (!isLatestInstalled)
					{
						ProgressChanged?.Invoke(null, new ProgressChangedEventArgs(0, null));
						FileChanged?.Invoke(new McDownloadFileChangedEventArgs("Optifine"));
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
			launcher.FileChanged += e => FileChanged?.Invoke(new McDownloadFileChangedEventArgs(e.FileKind.ToString()));
			launcher.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e);
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

			var process = await launcher.CreateProcessAsync(launchOption);

			sync.Load();
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.Start();
			new Task(() =>
			{
				while (!process.StandardOutput.EndOfStream)
				{
					var line = process.StandardOutput.ReadLine();
					MinecraftOutput?.Invoke(new MinecraftOutputEventArgs(line));
				}
			}).Start();
			if (WindowMode == WindowMode.Borderless)
			{
				var wt = new WindowTweaks(process);
				_ = wt.Borderless();
			}

			await process.WaitForExitAsync();
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

			version.JavaBinaryPath = Path.Combine(path.Runtime, javaVersion, "bin", MJava.GetDefaultBinaryName());
		}
	}
}