using DiscordRPC;
using DiscordRPC.Logging;
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
	}
}
