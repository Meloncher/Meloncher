using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MeloncherCore.Optifine.Bobcat
{
	public class FileInfo
	{
		[JsonIgnore] public string Name { get; set; }
		[JsonProperty("path")] public string Path { get; set; }

		[JsonProperty("sha1")] public string Sha1 { get; set; }

		[JsonProperty("size")] public long Size { get; set; }

		[JsonProperty("url")] public string Url { get; set; }
	}

	public class GameDownloadInfo
	{
		[JsonProperty("client")] public FileInfo Client { get; set; }

		[JsonProperty("server")] public FileInfo Server { get; set; }
	}

	public class Asset
	{
		[JsonProperty("id")] public string Id { get; set; }

		[JsonProperty("sha1")] public string Sha1 { get; set; }

		[JsonProperty("size")] public long Size { get; set; }

		[JsonProperty("totalSize")] public long TotalSize { get; set; }

		[JsonProperty("url")] public string Url { get; set; }
	}

	public class Arguments
	{
		[JsonProperty("game")] public List<object> Game { get; set; }

		[JsonProperty("jvm")] public List<object> Jvm { get; set; }
	}

	public class Extract
	{
		[JsonProperty("exclude")] public List<string> Exclude { get; set; }
	}

	public class Downloads
	{
		[JsonProperty("artifact")] public FileInfo Artifact { get; set; }
		[JsonProperty("classifiers")] public Dictionary<string, FileInfo> Classifiers { get; set; }
	}

	public class Library
	{
		[JsonProperty("downloads")] public Downloads Downloads { get; set; }

		[JsonProperty("name")] public string Name { get; set; }

		[JsonProperty("extract")] public Extract Extract { get; set; }

		[JsonProperty("natives")] public Dictionary<string, string> Natives { get; set; }

		[JsonProperty("rules")] public List<JvmRules> Rules { get; set; }

		[JsonProperty("checksums")] public List<string> CheckSums { get; set; }

		[JsonProperty("serverreq")] public bool ServerRequired { get; set; }

		[JsonProperty("clientreq")] public bool ClientRequired { get; set; } = true;

		[JsonProperty("url")] public string Url { get; set; }
	}

	public class Client
	{
		[JsonProperty("argument")] public string Argument { get; set; }

		[JsonProperty("file")] public FileInfo File { get; set; }

		[JsonProperty("type")] public string Type { get; set; }
	}

	public class Logging
	{
		[JsonProperty("client")] public Client Client { get; set; }
	}

	public class RawVersionModel
	{
		[JsonProperty("minecraftArguments")]
		public string MinecraftArguments { get; set; }

		[JsonProperty("arguments")]
		public Arguments Arguments { get; set; }

		[JsonProperty("assetIndex")]
		public Asset AssetIndex { get; set; }

		[JsonProperty("assets")]
		public string AssetsVersion { get; set; }

		[JsonProperty("downloads")]
		public GameDownloadInfo Downloads { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("javaVersion")] public JavaVersionModel JavaVersion { get; set; }

		[JsonProperty("inheritsFrom")]
		public string InheritsFrom { get; set; }

		[JsonProperty("libraries")]
		public List<Library> Libraries { get; set; }

		[JsonProperty("logging")]
		public Logging Logging { get; set; }

		[JsonProperty("mainClass")]
		public string MainClass { get; set; }

		[JsonProperty("minimumLauncherVersion")]
		public int MinimumLauncherVersion { get; set; }

		[JsonProperty("releaseTime")]
		public DateTime ReleaseTime { get; set; }

		[JsonProperty("time")]
		public DateTime Time { get; set; }

		[JsonProperty("type")]
		public string BuildType { get; set; }

		[JsonProperty("jar")]
		public string JarFile { get; set; }
	}

	public class JavaVersionModel
	{
		[JsonProperty("component")] public string Component { get; set; }

		[JsonProperty("majorVersion")] public int MajorVersion { get; set; }
	}

	public class JvmRules
	{
		[JsonProperty("action")]
		public string Action { get; set; }

		[JsonProperty("os")]
		public Dictionary<string, string> OperatingSystem { get; set; }
	}
}