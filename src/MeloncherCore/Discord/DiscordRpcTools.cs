using DiscordRPC;
using MeloncherCore.Launcher;

namespace MeloncherCore.Discord
{
	public class DiscordRpcTools
	{
		private readonly DiscordRpcClient _client;

		public DiscordRpcTools()
		{
			_client = new DiscordRpcClient("895992198171078666");
			_client.Initialize();
		}

		public void SetStatus(string details, string state)
		{
			_client.SetPresence(new RichPresence
			{
				Details = details,
				State = state,
				Assets = new Assets
				{
					LargeImageKey = "melon",
					LargeImageText = "Meloncher"
				}
			});
		}

		public void OnLog(string line)
		{
			var logLine = McLogLine.Parse(line);
			if (logLine.Text.StartsWith("Connecting to "))
			{
				var var0 = logLine.Text.Replace("Connecting to ", "").Split(", ");
				if (var0.Length == 2)
				{
					string server;
					if (var0[1] == "25565") server = var0[0];
					else server = var0[0] + ":" + var0[1];
					SetStatus("Играет на сервере " + server, "");
				}
			}
			else if (logLine.Text.StartsWith("Starting integrated minecraft server"))
			{
				SetStatus("Играет в одиночном мире", "");
			}
			else if (logLine.Text.StartsWith("Stopping server"))
			{
				SetStatus("Главное меню", "");
			}
		}
	}
}