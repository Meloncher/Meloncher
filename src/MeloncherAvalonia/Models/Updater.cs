using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace MeloncherAvalonia.Models
{
	public class Updater
	{
		private const int CurrentVersion = 25;
		private readonly WebClient _client = new();
		private UpdaterJson? _updaterJson;

		public UpdaterJson? CheckUpdates()
		{
			try
			{
				var jsonString = _client.DownloadString("https://raw.githubusercontent.com/Meloncher/Meloncher/master/updater.json");
				_updaterJson = JsonConvert.DeserializeObject<UpdaterJson>(jsonString);
				if (_updaterJson.Version > CurrentVersion) return _updaterJson;
			}
			catch (Exception)
			{
				// ignored
			}

			return null;
		}

		public bool Update()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				try
				{
					return updateWindows();
				}
				catch (Exception)
				{
					// ignored
				}

			return false;
		}

		private bool updateWindows()
		{
			if (_updaterJson == null) return false;
			var filename = Path.GetTempFileName() + ".exe";
			_client.DownloadFile(_updaterJson.DirectLink, filename);
			var proc = new Process();
			proc.StartInfo.FileName = filename;
			proc.StartInfo.Arguments = _updaterJson.LaunchArgs;
			proc.Start();
			return true;
		}
	}

	public class UpdaterJson
	{
		[JsonProperty("version")] public int Version { get; set; }

		[JsonProperty("direct_link")] public string DirectLink { get; set; }

		[JsonProperty("description")] public string Description { get; set; }

		[JsonProperty("launch_args")] public string LaunchArgs { get; set; }
	}
}