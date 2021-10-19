using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using MeloncherCore.Launcher;

namespace MeloncherAvalonia
{
	internal class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		public static void Main(string[] args)
		{
			try
			{
				BuildAvaloniaApp()
					.StartWithClassicDesktopLifetime(args);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				string mcpath = ExtMinecraftPath.GetOSDefaultPath();
				if (!Directory.Exists(mcpath))
					Directory.CreateDirectory(mcpath);
				using (StreamWriter writer = new(Path.Combine(mcpath, "crash.log")))  
				{  
					writer.WriteLine(e);
				}  
				throw;
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		private static AppBuilder BuildAvaloniaApp()
		{
			return AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToTrace()
				.UseReactiveUI();
		}
	}
}