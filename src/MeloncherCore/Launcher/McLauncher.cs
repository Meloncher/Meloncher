using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Optifine;
using MeloncherCore.Options;
using MeloncherCore.Version;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace MeloncherCore.Launcher
{
	public class McLauncher
	{
		public ExtMinecraftPath MinecraftPath { get; set; }
		public McLauncher(ExtMinecraftPath MinecraftPath)
		{
			this.MinecraftPath = MinecraftPath;
		}

		public event DownloadFileChangedHandler FileChanged;
		public event ProgressChangedEventHandler ProgressChanged;

		public async Task Launch(McVersion version, MSession session, bool offline, bool optifine)
		{
			ProgressChanged?.Invoke(null, new ProgressChangedEventArgs(50, null));
			//var path = new ExtMinecraftPath("D:\\MeloncherNetTest", $"profiles\\versions\\{version.ProfileName}");
			var path = MinecraftPath.CloneWithProfile("versions", version.ProfileName);
			//var path = new ExtMinecraftPath("Data", $"profiles\\versions\\{version.ProfileName}");
			//var path = new MinecraftPath("D:\\MeloncherNetTest\\testvanillalike");
			var launcher = new CMLauncher(path);
			if (offline)
			{
				launcher.VersionLoader = new LocalVersionLoader(MinecraftPath);
				launcher.FileDownloader = null;
			}

			launcher.FileChanged += (e) => FileChanged?.Invoke(e);
			launcher.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e);
			var launchOption = new MLaunchOption
			{
				StartVersion = launcher.GetVersion(version.Name),
				MaximumRamMb = 1024,
				Session = session,
				VersionType = "Meloncher",
				GameLauncherName = "Meloncher"
			};

			var sync = new McOptionsSync(path);

			if (!offline) await launcher.CheckAndDownloadAsync(launchOption.StartVersion);

			FixJavaBinaryPath(MinecraftPath, launchOption.StartVersion);

			if (optifine)
			{
				var optifineInstaller = new OptifineInstallerBobcat();
				optifineInstaller.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e);
				string ofVerName = null;
				if (offline)
				{
					ofVerName = optifineInstaller.GetLatestInstalled(version.Name, MinecraftPath);
				} else
				{
					ofVerName = await optifineInstaller.IsLatestInstalled(version.Name, MinecraftPath);
					if (ofVerName == null) {
						FileChanged?.Invoke(new DownloadFileChangedEventArgs(MFile.Others, this, "Optifine", 1, 1));
						ofVerName = await optifineInstaller.installOptifine(version.Name, MinecraftPath, launchOption.StartVersion.JavaBinaryPath);
					} 
				}

				if (ofVerName != null) {
					launchOption.StartVersion = new LocalVersionLoader(MinecraftPath).GetVersionMetadatas().GetVersion(ofVerName);		
					FixJavaBinaryPath(MinecraftPath, launchOption.StartVersion);
				}
			}

			var process = await launcher.CreateProcessAsync(launchOption);
			sync.Load();
			process.Start();
			var wt = new WindowTweaks(process);
			_ = wt.Borderless();
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