using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Apex.MVVM;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Account;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using MeloncherWPF.Views.Windows;

namespace MeloncherWPF.ViewModels
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		private AccountStorage accountStorage;
		DiscordRpcTools _discordRpcTools = new DiscordRpcTools();

		private McLauncher mcLauncher;
		private IVersionLoader versionLoader;

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			PlayButtonCommand = new Command(OnPlayButtonCommandExecuted);
			DeleteAccountCommand = new Command(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = new Command(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = new Command(OnAddMojangCommandExecuted);
			AddOfflineCommand = new Command(OnAddOfflineCommandExecuted);
			var path = new ExtMinecraftPath();
			mcLauncher = new McLauncher(path);
			versionLoader = new DefaultVersionLoader(path);
			string loadingType = "";
			mcLauncher.FileChanged += (e) =>
			{
				loadingType = e.Type;
				//ProgressText = "Загрузка " + e.Type;
				if (ProgressValue == 0)
					switch (e.Type)
					{
						case "Resource":
							ProgressText = "Проверка Ресурсов...";
							break;
						case "Runtime":
							ProgressText = "Проверка Java...";
							break;
						case "Library":
							ProgressText = "Проверка Библиотек...";
							break;
						case "Minecraft":
							ProgressText = "Проверка Minecraft...";
							break;
						case "Optifine":
							ProgressText = "Проверка Optifine...";
							break;
						default:
							ProgressText = "Проверка Файлов...";
							break;
					}
			};
			mcLauncher.ProgressChanged += (s, e) =>
			{
				ProgressValue = e.ProgressPercentage;
				switch (loadingType)
				{
					case "Resource":
						ProgressText = "Загрузка Ресурсов...";
						break;
					case "Runtime":
						ProgressText = "Загрузка Java...";
						break;
					case "Library":
						ProgressText = "Загрузка Библиотек...";
						break;
					case "Minecraft":
						ProgressText = "Загрузка Minecraft...";
						break;
					case "Optifine":
						ProgressText = "Загрузка Optifine...";
						break;
					default:
						ProgressText = "Загрузка...";
						break;
				}
			};
			mcLauncher.MinecraftOutput += (e) => { _discordRpcTools.OnLog(e.Line); };
			accountStorage = new AccountStorage(mcLauncher.MinecraftPath);
			accountStorage.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => Accounts = new ObservableCollection<MSession>(accountStorage);
			Accounts = new ObservableCollection<MSession>(accountStorage);
			_discordRpcTools.SetStatus("Сидит в лаунчере", "");

			var mdts = versionLoader.GetVersionMetadatas();
			Versions = new ObservableCollection<MVersionMetadata>(mdts);
			SelectedVersion = mdts.LatestReleaseVersion;
		}

		public string Title { get; set; } = "Meloncher";
		public string McVersionName { get; set; } = "1.12.2";
		public string Username { get; set; } = "Steve";
		public bool Optifine { get; set; } = true;
		public bool Offline { get; set; } = false;
		public int ProgressValue { get; set; }
		public string ProgressText { get; set; }
		public bool ProgressHidden { get; set; } = true;
		public bool IsNotStarted { get; set; } = true;
		public ObservableCollection<MSession> Accounts { get; set; }
		public ObservableCollection<MVersionMetadata> Versions { get; set; }
		public MVersionMetadata SelectedVersion { get; set; }
		public int selectedAccount { get; set; } = 0;

		public Command DeleteAccountCommand { get; }

		public Command AddMicrosoftCommand { get; }

		public Command AddMojangCommand { get; }

		public Command AddOfflineCommand { get; }

		public ICommand PlayButtonCommand { get; }
		public event PropertyChangedEventHandler PropertyChanged;

		private void OnDeleteAccountCommandExecuted(Object p)
		{
			accountStorage.RemoveAt(selectedAccount);
		}

		private void OnAddMicrosoftCommandExecuted(object p)
		{
			MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			MSession session = loginWindow.ShowLoginDialog();
			accountStorage.Add(session);
		}

		private void OnAddMojangCommandExecuted(object p)
		{
			var dialog = new AddAccountWindow();
			if (dialog.ShowDialog() == true)
			{
				var login = new MLogin();
				var resp = login.Authenticate(dialog.ResponseLogin, dialog.ResponsePass);
				MSession session = resp.Session;
				accountStorage.Add(session);
			}
		}

		private void OnAddOfflineCommandExecuted(object p)
		{
			var dialog = new AddAccountWindow();
			if (dialog.ShowDialog() == true)
			{
				MSession session = MSession.GetOfflineSession(dialog.ResponseLogin);
				accountStorage.Add(session);
			}
		}

		private void OnPlayButtonCommandExecuted(object p)
		{
			MSession session = (Accounts.Count > 0) ? Accounts[selectedAccount] : MSession.GetOfflineSession("Player");
			new Task(async () =>
			{
				IsNotStarted = false;
				ProgressHidden = false;
				Title = "Meloncher " + McVersionName;
				_discordRpcTools.SetStatus("Играет на версии " + McVersionName, "");
				mcLauncher.Offline = Offline;
				mcLauncher.UseOptifine = Optifine;
				//mcLauncher.SetVersionByName(McVersionName);
				mcLauncher.SetVersion(SelectedVersion.GetVersion());
				//mcLauncher.Version = new McVersion(McVersionName, "Test", "Test-" + McVersionName);
				mcLauncher.Session = session;
				await mcLauncher.Update();
				ProgressValue = 0;
				ProgressText = null;
				ProgressHidden = true;
				await mcLauncher.Launch();
				IsNotStarted = true;
				Title = "Meloncher";
				_discordRpcTools.SetStatus("Сидит в лаунчере", "");
			}).Start();
		}
	}
}