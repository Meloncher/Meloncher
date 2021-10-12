using System;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Account;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	internal class MainWindowViewModel : ViewModelBase
	{
		[Reactive] public string Title { get; set; } = "Meloncher";
		[Reactive] public string Username { get; set; } = "Steve";
		[Reactive] public bool Optifine { get; set; } = true;
		[Reactive] public bool Offline { get; set; } = false;
		[Reactive] public int ?ProgressValue { get; set; }
		[Reactive] public string ?ProgressText { get; set; }
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsNotStarted { get; set; } = true;

		private McLauncher mcLauncher;
		private IVersionLoader versionLoader;
		DiscrodRPCTools discrodRPCTools = new DiscrodRPCTools();
		public MainWindowViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			PlayButtonCommand = ReactiveCommand.Create(OnPlayButtonCommandExecuted);
			DeleteAccountCommand = ReactiveCommand.Create(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = ReactiveCommand.Create(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = ReactiveCommand.Create(OnAddMojangCommandExecuted);
			AddOfflineCommand = ReactiveCommand.Create(OnAddOfflineCommandExecuted);
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
			mcLauncher.MinecraftOutput += (e) =>
			{
				discrodRPCTools.OnLog(e.Line);
			};
			accountStorage = new AccountStorage(mcLauncher.MinecraftPath);
			accountStorage.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => Accounts = new ObservableCollection<MSession>(accountStorage);
			Accounts = new ObservableCollection<MSession>(accountStorage);
			discrodRPCTools.SetStatus("Сидит в лаунчере", "");

			var mdts = versionLoader.GetVersionMetadatas();
			Versions = new ObservableCollection<MVersionMetadata>(mdts);
			SelectedVersion = mdts.LatestReleaseVersion;
		}

		private AccountStorage accountStorage;
		[Reactive] public ObservableCollection<MSession> Accounts { get; set; }
		[Reactive] public ObservableCollection<MVersionMetadata> Versions { get; set; }
		[Reactive] public MVersionMetadata ?SelectedVersion { get; set; }
		[Reactive] public int selectedAccount { get; set; } = 0;

		public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
		private void OnDeleteAccountCommandExecuted()
		{
			accountStorage.RemoveAt(selectedAccount);
		}

		public ReactiveCommand<Unit, Unit> AddMicrosoftCommand { get; }
		private void OnAddMicrosoftCommandExecuted()
		{
			Title = "Test";
			//MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			//MSession session = loginWindow.ShowLoginDialog();
			//accountStorage.Add(session);
		}

		public ReactiveCommand<Unit, Unit> AddMojangCommand { get; }
		private void OnAddMojangCommandExecuted()
		{
			//var dialog = new AddAccountWindow();
			//if (dialog.ShowDialog() == true)
			//{
			//	var login = new MLogin();
			//	var resp = login.Authenticate(dialog.ResponseLogin, dialog.ResponsePass);
			//	MSession session = resp.Session;
			//	accountStorage.Add(session);
			//}
		}

		public ReactiveCommand<Unit, Unit> AddOfflineCommand { get; }
		private void OnAddOfflineCommandExecuted()
		{
			//var dialog = new AddAccountWindow();
			//if (dialog.ShowDialog() == true)
			//{
			//	MSession session = MSession.GetOfflineSession(dialog.ResponseLogin);
			//	accountStorage.Add(session);
			//}
		}

		public ReactiveCommand<Unit, Unit> PlayButtonCommand { get; }
		private void OnPlayButtonCommandExecuted()
		{
			MSession session = (Accounts.Count > 0) ? Accounts[selectedAccount] : MSession.GetOfflineSession("Player");
			new Task(async () =>
			{
				IsNotStarted = false;
				ProgressHidden = false;
				Title = "Meloncher " + SelectedVersion?.Name;
				discrodRPCTools.SetStatus("Играет на версии " + SelectedVersion.Name, "");
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
				discrodRPCTools.SetStatus("Сидит в лаунчере", "");
			}).Start();
		}
	}
}
