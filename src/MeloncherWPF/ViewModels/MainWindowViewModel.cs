using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using MeloncherWPF.Infrastructure.Commands;
using MeloncherWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MeloncherWPF.ViewModels
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{

		public class ConsoleWriterEventArgs : EventArgs
		{
			public string Value { get; private set; }
			public ConsoleWriterEventArgs(string value)
			{
				Value = value;
			}
		}

		public class ConsoleWriter : TextWriter
		{
			public override Encoding Encoding { get { return Encoding.UTF8; } }

			public override void Write(string value)
			{
				if (WriteEvent != null) WriteEvent(this, new ConsoleWriterEventArgs(value));
				base.Write(value);
			}

			public override void WriteLine(string value)
			{
				if (WriteLineEvent != null) WriteLineEvent(this, new ConsoleWriterEventArgs(value));
				base.WriteLine(value);
			}

			public event EventHandler<ConsoleWriterEventArgs> WriteEvent;
			public event EventHandler<ConsoleWriterEventArgs> WriteLineEvent;
		}

		public MainWindowViewModel()
		{
			Title = "Meloncher";
			McVersionName = "1.12.2";
			Username = "Steve";
			Optifine = true;
			Offline = false;
			ProgressValue = 0;
			IsNotStarted = true;
			PlayButtonCommand = new LambdaCommand(OnPlayButtonCommandExecuted, CanPlayButtonCommandExecute);
			TestConsole = "";

			using (var consoleWriter = new ConsoleWriter())
			{
				consoleWriter.WriteEvent += consoleWriter_WriteEvent;
				consoleWriter.WriteLineEvent += consoleWriter_WriteLineEvent;

				Console.SetOut(consoleWriter);
			}

			mcLauncher = new McLauncher();
		}

		void consoleWriter_WriteLineEvent(object sender, ConsoleWriterEventArgs e)
		{
			TestConsole =  e.Value + "\n" + TestConsole;
		}

		void consoleWriter_WriteEvent(object sender, ConsoleWriterEventArgs e)
		{
			TestConsole += e.Value;
		}

		public string TestConsole { get; set; }
		public string Title { get; set; }
		public string McVersionName { get; set; }
		public string Username { get; set; }
		public bool Optifine { get; set; }
		public bool Offline { get; set; }
		public int ProgressValue { get; set; }
		public bool IsNotStarted { get; set; }

		private McLauncher mcLauncher;

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand PlayButtonCommand { get; }
		private bool CanPlayButtonCommandExecute(object p)
		{
			//return !IsStarted;
			return true;
		}
		private void OnPlayButtonCommandExecuted(object p)
		{
			mcLauncher = new McLauncher();
			new Task(async () => {
				IsNotStarted = false;
				Title = "Meloncher " + McVersionName;
				await mcLauncher.Launch(new McVersion(McVersionName, "Test", "Test-" + McVersionName), MSession.GetOfflineSession(Username), Offline, Optifine);
				IsNotStarted = true;
				Title = "Meloncher";
			}).Start();
		}
	}
}
