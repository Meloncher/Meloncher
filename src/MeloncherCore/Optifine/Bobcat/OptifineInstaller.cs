﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmlLib.Core;
using Newtonsoft.Json;
using SharpCompress.Archives;

namespace MeloncherCore.Optifine.Bobcat
{
	public class OptifineInstaller
	{
		public string JavaExecutablePath { get; set; }

		public string OptifineJarPath { get; set; }
		public MinecraftPath MinecraftPath { get; set; }
		public string CustomId { get; set; }
		public OptifineDownloadVersionModel OptifineDownloadVersion { get; set; }

		public string Install()
		{
			return InstallTaskAsync().Result;
		}

		public event EventHandler<StageChangedEventArgs> StageChangedEventDelegate;

		public virtual void InvokeStatusChangedEvent(string currentStage, double progress)
		{
			StageChangedEventDelegate?.Invoke(this, new StageChangedEventArgs
			{
				CurrentStage = currentStage,
				Progress = progress
			});
		}

		private bool isNewArguments(string mcVer)
		{
			var var0 = mcVer.Split(".");
			if (var0.Length >= 2)
			{
				var var1 = var0[1];
				int var2;
				if (int.TryParse(var1, out var2))
					if (var2 > 12)
						return true;
			}

			return false;
		}

		public async Task<string> InstallTaskAsync()
		{
			InvokeStatusChangedEvent("Start installing Optifine", 0);
			var mcVersion = OptifineDownloadVersion.McVersion;
			var edition = OptifineDownloadVersion.Type;
			var release = OptifineDownloadVersion.Patch;
			var editionRelease = $"{edition}_{release}";
			var id = string.IsNullOrEmpty(CustomId)
				? $"{mcVersion}-Optifine_{editionRelease}"
				: CustomId;
			var versionPath = Path.Combine(MinecraftPath.Versions, id);
			var di = new DirectoryInfo(versionPath);

			if (!di.Exists)
				di.Create();

			InvokeStatusChangedEvent("Read Optifine data", 20);
			using var archive = ArchiveFactory.Open(OptifineJarPath);
			var entries = archive.Entries;

			var launchWrapperVersion = "1.7";

			foreach (var entry in entries)
			{
				if (!entry.Key.Equals("launchwrapper-of.txt", StringComparison.OrdinalIgnoreCase)) continue;
				await using var stream = entry.OpenEntryStream();
				using var sr = new StreamReader(stream, Encoding.UTF8);
				launchWrapperVersion = await sr.ReadToEndAsync();
			}

			var launchWrapperEntry =
				entries.First(x => x.Key.Equals($"launchwrapper-of-{launchWrapperVersion}.jar"));

			InvokeStatusChangedEvent("Build version assembly", 40);

			var versionModel = new RawVersionModel
			{
				Id = id,
				InheritsFrom = mcVersion,
				ReleaseTime = DateTime.Now,
				Time = DateTime.Now,
				BuildType = "release",
				Libraries = new List<Library>
				{
					new()
					{
						Name = $"optifine:launchwrapper-of:{launchWrapperVersion}"
					},
					new()
					{
						Name = $"optifine:Optifine:{OptifineDownloadVersion.McVersion}_{editionRelease}"
					}
				},
				MainClass = "net.minecraft.launchwrapper.Launch",
				MinimumLauncherVersion = 21
			};

			if (isNewArguments(mcVersion))
				versionModel.Arguments = new Arguments
				{
					Game = new List<object>
					{
						"--tweakClass",
						"optifine.OptiFineTweaker"
					},
					Jvm = new List<object>()
				};
			else
				versionModel.MinecraftArguments = "--username ${auth_player_name} --version ${version_name} --gameDir ${game_directory} --assetsDir ${assets_root} --assetIndex ${assets_index_name} --uuid ${auth_uuid} --accessToken ${auth_access_token} --userType ${user_type} --versionType ${version_type} --tweakClass optifine.OptiFineTweaker";


			var versionJsonPath = MinecraftPath.GetVersionJsonPath(id);

			var jsonStr = JsonConvert.SerializeObject(versionModel, JsonHelper.CamelCasePropertyNamesSettings);
			await File.WriteAllTextAsync(versionJsonPath, jsonStr).ConfigureAwait(false);

			var librariesPath = Path.Combine(MinecraftPath.Library, "optifine",
				"launchwrapper-of",
				launchWrapperVersion);
			var libDi = new DirectoryInfo(librariesPath);

			InvokeStatusChangedEvent("Write Optifine data", 60);

			if (!libDi.Exists)
				libDi.Create();

			var launchWrapperPath = Path.Combine(librariesPath,
				$"launchwrapper-of-{launchWrapperVersion}.jar");
			if (!File.Exists(launchWrapperPath))
			{
				await using var launchWrapperFs = File.OpenWrite(launchWrapperPath);
				launchWrapperEntry.WriteTo(launchWrapperFs);
			}

			var gameJarPath = MinecraftPath.GetVersionJarPath(OptifineDownloadVersion.McVersion);
			var optifineLibPath = Path.Combine(MinecraftPath.Library, "optifine", "Optifine",
				$"{OptifineDownloadVersion.McVersion}_{editionRelease}",
				$"Optifine-{OptifineDownloadVersion.McVersion}_{editionRelease}.jar");

			var optifineLibPathDi = new DirectoryInfo(Path.GetDirectoryName(optifineLibPath)!);
			if (!optifineLibPathDi.Exists)
				optifineLibPathDi.Create();

			InvokeStatusChangedEvent("Execute the installation script", 80);

			var ps = new ProcessStartInfo(JavaExecutablePath)
			{
				ArgumentList =
				{
					"-cp",
					OptifineJarPath,
					"optifine.Patcher",
					Path.GetFullPath(gameJarPath),
					OptifineJarPath,
					Path.GetFullPath(optifineLibPath)
				},
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};

			var p = Process.Start(ps);
			if (p == null)
				throw new NullReferenceException();

			p.BeginOutputReadLine();
			p.BeginErrorReadLine();

			void LogReceivedEvent(object sender, DataReceivedEventArgs args)
			{
				InvokeStatusChangedEvent(args.Data ?? "loading...", 85);
			}

			p.OutputDataReceived += LogReceivedEvent;

			var errList = new List<string>();
			p.ErrorDataReceived += (sender, args) =>
			{
				LogReceivedEvent(sender, args);

				if (!string.IsNullOrEmpty(args.Data))
					errList.Add(args.Data);
			};

			await p.WaitForExitAsync();
			InvokeStatusChangedEvent("Installation is almost complete", 90);

			if (errList.Any())
				throw new NullReferenceException();

			InvokeStatusChangedEvent("Optifine installation is complete", 100);

			return id;
		}
	}
}