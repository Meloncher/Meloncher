using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using MeloncherWPF.Infrastructure.Commands;
using MeloncherWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MeloncherWPF.ViewModels
{
	internal class MainWindowViewModel : ViewModelBase
	{
		private string _Title = "Meloncher";
		public string Title
		{
			get => _Title;
			set => Set(ref _Title, value);
		}

		private string _Version = "1.12.2";
		public string Verison
		{
			get => _Version;
			set => Set(ref _Version, value);
		}

		private string _Username = "Steve";
		public string Username
		{
			get => _Username;
			set => Set(ref _Username, value);
		}

		private bool _Optifine = true;
		public bool Optifine
		{
			get => _Optifine;
			set => Set(ref _Optifine, value);
		}

		private bool _Offline = false;
		public bool Offline
		{
			get => _Offline;
			set => Set(ref _Offline, value);
		}

		private int _ProgressValue = 0;
		public int ProgressValue
		{
			get => _ProgressValue;
			set => Set(ref _ProgressValue, value);
		}

		private McLauncher mcLauncher = new McLauncher();
		private bool isStarted = false;

		public ICommand PlayButtonCommand { get; }
		private bool CanPlayButtonCommandExecute(object p)
		{
			return !isStarted;
		}
		private void OnPlayButtonCommandExecuted(object p)
		{
			mcLauncher = new McLauncher();
			new Task(async () => {
				isStarted = true;
				await mcLauncher.Launch(new McVersion(Verison, "Test", "Test-" + Verison), MSession.GetOfflineSession(Username), Offline, Optifine);
				isStarted = false;
			}).Start();
		}

		public MainWindowViewModel()
		{
			PlayButtonCommand = new LambdaCommand(OnPlayButtonCommandExecuted, CanPlayButtonCommandExecute);
		}
	}
}
