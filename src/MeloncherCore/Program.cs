using System;
using System.Threading.Tasks;

namespace MeloncherCore
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			// ServicePointManager.DefaultConnectionLimit = 512;
			// //Console.WriteLine(McOptionsUtils.GetDefaultScale());
			// Console.WriteLine("Hello Meloncher!");
			// DiscordRpcTools discordRpcTools = new();
			// discordRpcTools.SetStatus("Сидит в лаунчере", "");
			// Console.Write("Version: ");
			// var versionName = Console.ReadLine();
			// var offline = Confirm("Offline?");
			// var optifine = Confirm("Optifine?");
			// //var version = new McVersion(versionName, "Test", "Test-" + versionName);
			// var path = new ExtMinecraftPath();
			//
			// var launcher = new McLauncher(path);
			// // launcher.McDownloadProgressChanged += e =>
			// // {
			// // 	//FileChanged?.Invoke(e);
			// // 	//Console.WriteLine("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount);
			// // 	Console.WriteLine("Загрузка " + e.Type);
			// // };
			// // launcher.ProgressChanged += (s, e) =>
			// // {
			// // 	//ProgressChanged?.Invoke(s, e);
			// // 	Console.WriteLine("{0}%", e.ProgressPercentage);
			// // };
			// launcher.MinecraftOutput += e =>
			// {
			// 	// Console.WriteLine(McLogLine.Parse(e.Line));
			// 	Console.WriteLine(e.Line);
			// 	discordRpcTools.OnLog(e.Line);
			// };
			// var session = MSession.GetOfflineSession("tester123");
			// //var qwe = new MicrosoftAuth();
			// //var session = qwe.test();
			// //await launcher.Launch(version, session, offline, optifine);
			// //launcher.Version = version;
			// launcher.SetVersion(versionName);
			// launcher.Session = session;
			// launcher.UseOptifine = optifine;
			// discordRpcTools.SetStatus("Играет на версии " + versionName, "");
			// if (!offline) await launcher.Update();
			// await launcher.Launch();
		}

		private static bool Confirm(string text)
		{
			ConsoleKey response;
			do
			{
				Console.Write(text + " [y/N] ");
				response = Console.ReadKey(false).Key; // true is intercept key (dont show), false is show
				if (response != ConsoleKey.Enter)
					Console.WriteLine();
			} while (response != ConsoleKey.Y && response != ConsoleKey.N && response != ConsoleKey.Enter);

			return response == ConsoleKey.Y;
		}
	}
}