using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using MeloncherWPF.Infrastructure.Commands;
using System;
using System.ComponentModel;
using System.IO;
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
			ProgressValue = -1;
			IsNotStarted = true;
			PlayButtonCommand = new LambdaCommand(OnPlayButtonCommandExecuted, CanPlayButtonCommandExecute);
			TestConsole = "";

			mcLauncher = new McLauncher();
			mcLauncher.FileChanged += (e) =>
			{
				//FileChanged?.Invoke(e);
				TestLog(String.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount));
			};
			mcLauncher.ProgressChanged += (s, e) =>
			{
				//ProgressChanged?.Invoke(s, e);
				//TestLog(String.Format("{0}%", e.ProgressPercentage));
				ProgressValue = e.ProgressPercentage;
			};
		}

		void TestLog(string text)
		{
			TestConsole = text + "\n" + TestConsole;
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
			MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			MSession session = loginWindow.ShowLoginDialog();
			new Task(async () => {
				IsNotStarted = false;
				Title = "Meloncher " + McVersionName;
				
				//MSession asdsdaf = MSession.GetOfflineSession(Username);
				await mcLauncher.Launch(new McVersion(McVersionName, "Test", "Test-" + McVersionName), session, Offline, Optifine);
				ProgressValue = -1;
				IsNotStarted = true;
				Title = "Meloncher";
			}).Start();
		}
	}
}
