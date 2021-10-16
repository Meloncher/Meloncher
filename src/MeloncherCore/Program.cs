using CmlLib.Core.Auth;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MeloncherCore
{
	class Program
	{
		static async Task Main(string[] args)
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			//Console.WriteLine(McOptionsUtils.GetDefaultScale());
			Console.WriteLine("Hello Meloncher!");
			DiscrodRPCTools discrodRPCTools = new DiscrodRPCTools();
			discrodRPCTools.SetStatus("Сидит в лаунчере", "");
			Console.Write("Version: ");
			string versionName = Console.ReadLine();
			bool offline = Confirm("Offline?");
			bool optifine = Confirm("Optifine?");
			//var version = new McVersion(versionName, "Test", "Test-" + versionName);
			var path = new ExtMinecraftPath();

			var launcher = new McLauncher(path);
			launcher.FileChanged += (e) =>
			{
				//FileChanged?.Invoke(e);
				//Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
				Console.WriteLine("Загрузка " + e.Type);
			};
			launcher.ProgressChanged += (s, e) =>
			{
				//ProgressChanged?.Invoke(s, e);
				Console.WriteLine("{0}%", e.ProgressPercentage);
			};
			launcher.MinecraftOutput += (e) =>
			{
				// Console.WriteLine(McLogLine.Parse(e.Line));
				Console.WriteLine(e.Line);
				discrodRPCTools.OnLog(e.Line);
			};
			var session = MSession.GetOfflineSession("tester123");
			//var qwe = new MicrosoftAuth();
			//var session = qwe.test();
			//await launcher.Launch(version, session, offline, optifine);
			//launcher.Version = version;
			launcher.SetVersion(versionName);
			launcher.Session = session;
			launcher.Offline = offline;
			launcher.UseOptifine = optifine;
			discrodRPCTools.SetStatus("Играет на версии " + versionName, "");
			await launcher.Update();
			await launcher.Launch();
		}

		static bool Confirm(string text)
		{
			ConsoleKey response;
			do
			{
				Console.Write(text + " [y/N] ");
				response = Console.ReadKey(false).Key;   // true is intercept key (dont show), false is show
				if (response != ConsoleKey.Enter)
					Console.WriteLine();

			} while (response != ConsoleKey.Y && response != ConsoleKey.N && response != ConsoleKey.Enter);
			return response == ConsoleKey.Y;
		}
	}
}
