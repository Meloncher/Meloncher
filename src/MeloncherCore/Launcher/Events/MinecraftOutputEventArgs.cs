using System;

namespace MeloncherCore.Launcher.Events
{
	public delegate void MinecraftOutputEventHandler(MinecraftOutputEventArgs e);

	public class MinecraftOutputEventArgs : EventArgs
	{
		public MinecraftOutputEventArgs(string line)
		{
			Line = line;
		}

		public string Line { get; }
	}
}