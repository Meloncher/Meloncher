using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using MeloncherCore.Launcher.Events;

namespace MeloncherCore.Launcher
{
	public class McProcess
	{
		public McProcess(Process process)
		{
			Process = process;
		}

		public Process Process { get; set; }
		private ObservableCollection<McLogLine> LogLines { get; set; } = new();
		public event MinecraftOutputEventHandler? MinecraftOutput;

		public void Start()
		{
			Process.StartInfo.RedirectStandardOutput = true;
			Process.StartInfo.RedirectStandardError = true;
			Process.Start();
			new Task(() =>
			{
				while (!Process.StandardOutput.EndOfStream)
				{
					var line = Process.StandardOutput.ReadLine();
					MinecraftOutput?.Invoke(new MinecraftOutputEventArgs(line));
				}
			}).Start();
		}

		public async Task WaitForExitAsync()
		{
			await Process.WaitForExitAsync();
		}
	}
}