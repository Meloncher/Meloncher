using DiscordRPC;
using DiscordRPC.Logging;
using MeloncherCore.Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Discord
{
	public class DiscrodRPCTools
	{
		public DiscordRpcClient client;
		public DiscrodRPCTools()
		{
			client = new DiscordRpcClient("895992198171078666");
			client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
			client.OnReady += (sender, e) =>
			{
				Console.WriteLine("Received Ready from user {0}", e.User.Username);
			};

			client.OnPresenceUpdate += (sender, e) =>
			{
				Console.WriteLine("Received Update! {0}", e.Presence);
			};
			client.Initialize();
			
		}
		public void SetStatus(string details, string state)
		{
			client.SetPresence(new RichPresence()
			{
				Details = details,
				State = state,
				Assets = new Assets()
				{
					LargeImageKey = "melon",
					LargeImageText = "Meloncher",
					//SmallImageKey = "image_small"
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
