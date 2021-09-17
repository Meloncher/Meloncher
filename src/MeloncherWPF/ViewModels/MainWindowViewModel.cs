using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using MeloncherWPF.Infrastructure.Commands;
using MeloncherWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MeloncherWPF.ViewModels
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{

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
		}
		
		public string Title { get; set; }
		public string McVersionName { get; set; }
		public string Username { get; set; }
		public bool Optifine { get; set; }
		public bool Offline { get; set; }
		public int ProgressValue { get; set; }
		public bool IsNotStarted { get; set; }

		private McLauncher mcLauncher = new McLauncher();

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
