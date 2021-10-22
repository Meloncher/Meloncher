using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using MeloncherCore.Optifine.Bobcat;
using Newtonsoft.Json;

namespace MeloncherAvalonia.Models
{
	public class Updater
	{
		private const int CurrentVersion = 13;
		private WebClient _client = new();
		private UpdaterJson? _updaterJson;
		public bool CheckUpdates()
		{
			try
			{
				var jsonString = _client.DownloadString("https://raw.githubusercontent.com/Meloncher/Meloncher/master/updater.json");
				_updaterJson = JsonConvert.DeserializeObject<UpdaterJson>(jsonString);
				if (_updaterJson.Version > CurrentVersion) return true;
			}
			catch (Exception)
			{
				// ignored
			}

			return false;
		}

		public bool Update()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				try
				{
					return updateWindows();
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return false;
		}

		private bool updateWindows()
		{
			if (_updaterJson == null) return false;
			var filename = Path.GetTempFileName() + ".exe";
			_client.DownloadFile(_updaterJson.DirectLink, filename);
			Process.Start(filename);
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