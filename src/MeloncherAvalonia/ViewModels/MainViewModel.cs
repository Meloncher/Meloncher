using System;
using System.ComponentModel;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Account;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using MeloncherCore.Launcher.Events;
using MeloncherCore.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		DiscrodRPCTools discrodRPCTools = new DiscrodRPCTools();

		private McLauncher mcLauncher;
		private IVersionLoader versionLoader;
		private MVersionCollection versionCollection;
		private AccountStorage accountStorage;
		private string loadingType = "";

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			PlayButtonCommand = ReactiveCommand.Create(OnPlayButtonCommandExecuted);
			OpenAccountsWindowCommand = ReactiveCommand.Create(OnOpenAccountsWindowCommandExecuted);
			OpenVersionsWindowCommand = ReactiveCommand.Create(OnOpenVersionsWindowCommandExecuted);

			var path = new ExtMinecraftPath();
			versionLoader = new DefaultVersionLoader(path);
			try
			{
				versionCollection = versionLoader.GetVersionMetadatas();
			}
			catch (Exception e)
			{
				versionCollection = new LocalVersionLoader(path).GetVersionMetadatas();
			}

			accountStorage = new AccountStorage(path);
			mcLauncher = new McLauncher(path);
			LauncherSettings = LauncherSettings.Create(path);
			SelectedVersion = versionCollection.LatestReleaseVersion;
			if (LauncherSettings.SelectedVersion != null) SelectedVersion = versionCollection.GetVersionMetadata(LauncherSettings.SelectedVersion);
			if (LauncherSettings.SelectedAccount != null) SelectedSession = accountStorage.Get(LauncherSettings.SelectedAccount);
			if (SelectedSession == null) SelectedSession = MSession.GetOfflineSession("Player");

			mcLauncher.FileChanged += OnMcLauncherOnFileChanged;
			mcLauncher.ProgressChanged += OnMcLauncherOnProgressChanged;
			mcLauncher.MinecraftOutput += (e) => { discrodRPCTools.OnLog(e.Line); };

			discrodRPCTools.SetStatus("Сидит в лаунчере", "");

			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "SelectedVersion") LauncherSettings.SelectedVersion = SelectedVersion?.Name;
				if (e.PropertyName == "SelectedSession") LauncherSettings.SelectedAccount = SelectedSession?.Username;
			};
		}
		
		void OnMcLauncherOnFileChanged(McDownloadFileChangedEventArgs e)
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
		}
		void OnMcLauncherOnProgressChanged(object _, ProgressChangedEventArgs e)
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
		}

		[Reactive] public string Title { get; set; } = "Meloncher";
		[Reactive] public int ProgressValue { get; set; } = 0;
		[Reactive] public string ProgressText { get; set; } = null;
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsStarted { get; set; } = false;
		[Reactive] public LauncherSettings LauncherSettings { get; set; }

		public Interaction<AccountsViewModel, MSession?> ShowSelectAccountDialog { get; } = new Interaction<AccountsViewModel, MSession?>();
		public Interaction<VersionsViewModel, MVersionMetadata?> ShowSelectVersionDialog { get; } = new Interaction<VersionsViewModel, MVersionMetadata?>();
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		[Reactive] public MSession? SelectedSession { get; set; }

		public ReactiveCommand<Unit, Task> OpenAccountsWindowCommand { get; }
		public ReactiveCommand<Unit, Task> OpenVersionsWindowCommand { get; }
		public ReactiveCommand<Unit, Unit> PlayButtonCommand { get; }

		private async Task OnOpenAccountsWindowCommandExecuted()
		{
			var dialog = new AccountsViewModel(accountStorage);
			var result = await ShowSelectAccountDialog.Handle(dialog);
			if (result != null)
			{
				SelectedSession = result;
			}
		}

		private async Task OnOpenVersionsWindowCommandExecuted()
		{
			var dialog = new VersionsViewModel(versionLoader, versionCollection);
			var result = await ShowSelectVersionDialog.Handle(dialog);
			if (result != null)
			{
				SelectedVersion = result;
			}
		}

		private void OnPlayButtonCommandExecuted()
		{
			MSession session = SelectedSession;
			new Task(async () =>
			{
				IsStarted = true;
				ProgressHidden = false;
				Title = "Meloncher " + SelectedVersion?.Name;
				discrodRPCTools.SetStatus("Играет на версии " + SelectedVersion?.Name, "");
				mcLauncher.UseOptifine = LauncherSettings.UseOptifine;
				mcLauncher.WindowMode = LauncherSettings.WindowMode;
				mcLauncher.SetVersion(SelectedVersion.GetVersion());
				mcLauncher.Session = session;
				try
				{
					await mcLauncher.Update();
				}
				catch (Exception e)
				{
					mcLauncher.Offline = true;
				}

				ProgressValue = 0;
				ProgressText = null;
				ProgressHidden = true;
				await mcLauncher.Launch();
				IsStarted = false;
				Title = "Meloncher";
				discrodRPCTools.SetStatus("Сидит в лаунчере", "");
			}).Start();
		}
	}
}