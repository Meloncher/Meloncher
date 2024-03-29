﻿using System;
using CmlLib.Core;

namespace MeloncherCore.Launcher
{
	public class ExtMinecraftPath : MinecraftPath
	{
		public new static readonly string
			MacDefaultPath = Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/meloncher",
			LinuxDefaultPath = Environment.GetEnvironmentVariable("HOME") + "/.meloncher",
			WindowsDefaultPath = Environment.GetEnvironmentVariable("appdata") + "\\.meloncher";

		public ExtMinecraftPath() : this(GetOSDefaultPath())
		{
		}

		public ExtMinecraftPath(string rootPath) : this(rootPath, null)
		{
		}

		public ExtMinecraftPath(string rootPath, string profileType, string profileName) : this(rootPath, "profiles/" + profileType + "/" + profileName)
		{
		}

		public ExtMinecraftPath(string rootPath, string profilePath)
		{
			if (string.IsNullOrEmpty(profilePath))
			{
				BasePath = NormalizePath(rootPath + "/" + profilePath);
				Resource = NormalizePath(BasePath + "/resources");
			}

			RootPath = NormalizePath(rootPath);
			MinecraftPath = NormalizePath(rootPath + "/minecraft");

			Library = NormalizePath(MinecraftPath + "/libraries");
			Versions = NormalizePath(MinecraftPath + "/versions");

			Runtime = NormalizePath(MinecraftPath + "/runtime");
			Assets = NormalizePath(MinecraftPath + "/assets");
			Natives = NormalizePath(MinecraftPath + "/natives");
			Dir(rootPath);
		}

		public ExtMinecraftPath(string rootPath, string basePath, string minecraftPath, string libraryPath, string versionsPath, string resourcePath, string runtimePath, string assetsPath, string nativesPath)
		{
			RootPath = rootPath;
			BasePath = basePath;
			MinecraftPath = minecraftPath;
			Library = libraryPath;
			Versions = versionsPath;
			Resource = resourcePath;
			Runtime = runtimePath;
			Assets = assetsPath;
			Natives = nativesPath;
			Dir(rootPath);
		}

		public string Natives { get; set; }
		public string RootPath { get; set; }
		public string MinecraftPath { get; set; }

		public new static string GetOSDefaultPath()
		{
			return MRule.OSName switch
			{
				"osx" => MacDefaultPath,
				"linux" => LinuxDefaultPath,
				"windows" => WindowsDefaultPath,
				_ => Environment.CurrentDirectory
			};
		}

		public ExtMinecraftPath CloneWithProfile(string profileType, string profileName)
		{
			return CloneWithProfile("profiles/" + profileType + "/" + profileName);
		}

		public ExtMinecraftPath CloneWithProfile(string profilePath)
		{
			return CloneWithProfileAbsolute(NormalizePath(RootPath + "/" + profilePath));
		}

		public ExtMinecraftPath CloneWithProfileAbsolute(string profilePath)
		{
			return new ExtMinecraftPath(RootPath, profilePath, MinecraftPath, Library, Versions, NormalizePath(profilePath + "/resources"), Runtime, Assets, Natives);
		}

		public override string GetNativePath(string id)
		{
			return NormalizePath($"{Natives}/{id}");
		}
	}
}