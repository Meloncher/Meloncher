using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Optifine;
using MeloncherCore.Options;
using MeloncherCore.Version;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace MeloncherCore.Launcher
{
	public class McLauncher
	{
		public McLauncher() { }

		public event DownloadFileChangedHandler FileChanged;
		public event ProgressChangedEventHandler ProgressChanged;

		public async Task Launch(McVersion version, MSession session, bool offline, bool optifine)
		{
			ProgressChanged?.Invoke(null, new ProgressChangedEventArgs(50, null));
			//var path = new ExtMinecraftPath("D:\\MeloncherNetTest", $"D:\\MeloncherNetTest\\profiles\\versions\\{version.ProfileName}");
			var path = new ExtMinecraftPath("Data", $"Data\\profiles\\versions\\{version.ProfileName}");
			//var path = new MinecraftPath("D:\\MeloncherNetTest\\testvanillalike");
			var launcher = new CMLauncher(path);
			if (offline)
			{
				launcher.VersionLoader = new LocalVersionLoader(path);
				launcher.FileDownloader = null;
			}

			launcher.FileChanged += (e) =>
			{
				FileChanged?.Invoke(e);
				Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
			};
			launcher.ProgressChanged += (s, e) =>
			{
				ProgressChanged?.Invoke(s, e);
				Console.WriteLine("{0}%", e.ProgressPercentage);
			};
			var launchOption = new MLaunchOption
			{
				StartVersion = launcher.GetVersion(version.Name),
				MaximumRamMb = 1024,
				Session = session,
				VersionType = "Meloncher",
				GameLauncherName = "Meloncher"
			};

			var sync = new Sync(path);

			if (!offline) await launcher.CheckAndDownloadAsync(launchOption.StartVersion);

			FixJavaBinaryPath(path, launchOption.StartVersion);

			if (optifine)
			{
				var optifineInstaller = new OptifineInstallerBobcat();
				string ofVerName = null;
				if (offline)
				{
					ofVerName = optifineInstaller.GetLatestInstalled(version.Name, path);
				} else
				{
					ofVerName = await optifineInstaller.IsLatestInstalled(version.Name, path);
					if (ofVerName == null) {
						ofVerName = await optifineInstaller.installOptifine(version.Name, path, launchOption.StartVersion.JavaBinaryPath);
					} 
				}

				if (ofVerName != null) {
					launchOption.StartVersion = new LocalVersionLoader(path).GetVersionMetadatas().GetVersion(ofVerName);		
					FixJavaBinaryPath(path, launchOption.StartVersion);
				}
			}

			var process = await launcher.CreateProcessAsync(launchOption);
			sync.Load();
			process.Start();
			var wt = new WindowTweaks(process);
			wt.Borderless();
			process.WaitForExit();
			sync.Save();
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