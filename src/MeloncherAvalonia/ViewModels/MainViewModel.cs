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
using MeloncherCore.Options;
using MeloncherCore.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		private readonly DiscordRpcTools _discordRpcTools = new();

		private readonly McLauncher _mcLauncher;
		private readonly IVersionLoader _versionLoader;
		private readonly MVersionCollection _versionCollection;
		private readonly AccountStorage _accountStorage;
		private string _loadingType = "";
		private readonly ExtMinecraftPath _path;

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			PlayButtonCommand = ReactiveCommand.Create(OnPlayButtonCommandExecuted);
			OpenAccountsWindowCommand = ReactiveCommand.Create(OnOpenAccountsWindowCommandExecuted);
			OpenVersionsWindowCommand = ReactiveCommand.Create(OnOpenVersionsWindowCommandExecuted);
			OpenSettingsWindowCommand = ReactiveCommand.Create(OnOpenSettingsWindowCommandExecuted);

			_path = new ExtMinecraftPath();
			_versionLoader = new DefaultVersionLoader(_path);
			try
			{
				_versionCollection = _versionLoader.GetVersionMetadatas();
			}
			catch (Exception e)
			{
				_versionCollection = new LocalVersionLoader(_path).GetVersionMetadatas();
			}

			_accountStorage = new AccountStorage(_path);
			_mcLauncher = new McLauncher(_path);
			LauncherSettings = LauncherSettings.New(_path);
			SelectedVersion = _versionCollection.LatestReleaseVersion;
			if (LauncherSettings.SelectedVersion != null) SelectedVersion = _versionCollection.GetVersionMetadata(LauncherSettings.SelectedVersion);
			if (LauncherSettings.SelectedAccount != null) SelectedSession = _accountStorage.Get(LauncherSettings.SelectedAccount);
			if (SelectedSession == null) SelectedSession = MSession.GetOfflineSession("Player");

			_mcLauncher.FileChanged += OnMcLauncherOnFileChanged;
			_mcLauncher.ProgressChanged += OnMcLauncherOnProgressChanged;
			_mcLauncher.MinecraftOutput += e => { _discordRpcTools.OnLog(e.Line); };

			_discordRpcTools.SetStatus("Сидит в лаунчере", "");

			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "SelectedVersion") LauncherSettings.SelectedVersion = SelectedVersion?.Name;
				if (e.PropertyName == "SelectedSession") LauncherSettings.SelectedAccount = SelectedSession?.Username;
			};
		}

		private void OnMcLauncherOnFileChanged(McDownloadFileChangedEventArgs e)
		{
			_loadingType = e.Type;
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

		private void OnMcLauncherOnProgressChanged(object _, ProgressChangedEventArgs e)
		{
			ProgressValue = e.ProgressPercentage;
			switch (_loadingType)
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
		[Reactive] public int ProgressValue { get; set; }
		[Reactive] public string? ProgressText { get; set; }
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsStarted { get; set; }
		[Reactive] public LauncherSettings LauncherSettings { get; set; }

		public Interaction<AccountsViewModel, MSession?> ShowSelectAccountDialog { get; } = new();
		public Interaction<VersionsViewModel, MVersionMetadata?> ShowSelectVersionDialog { get; } = new();
		public Interaction<SettingsViewModel, SettingsAction?> ShowSettingsDialog { get; } = new();
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		[Reactive] public MSession? SelectedSession { get; set; }

		public ReactiveCommand<Unit, Task> OpenAccountsWindowCommand { get; }
		public ReactiveCommand<Unit, Task> OpenVersionsWindowCommand { get; }
		public ReactiveCommand<Unit, Unit> PlayButtonCommand { get; }
		
		public ReactiveCommand<Unit, Task> OpenSettingsWindowCommand { get; }
		private async Task OnOpenSettingsWindowCommandExecuted()
		{
			var dialog = new SettingsViewModel(LauncherSettings);
			var result = await ShowSettingsDialog.Handle(dialog);
			if (result != null)
			{
				if (result == SettingsAction.Import)
				{
					var importer = new McOptionsImporter(_path);
					importer.Import();
					var messageBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow("Готово", "Готово!");
					messageBoxStandardWindow.Show();
				}
			}
		}

		private async Task OnOpenAccountsWindowCommandExecuted()
		{
			var dialog = new AccountsViewModel(_accountStorage);
			var result = await ShowSelectAccountDialog.Handle(dialog);
			if (result != null)
			{
				SelectedSession = result;
			}
		}

		private async Task OnOpenVersionsWindowCommandExecuted()
		{
			var dialog = new VersionsViewModel(_versionLoader, _versionCollection);
			var result = await ShowSelectVersionDialog.Handle(dialog);
			if (result != null)
			{
				SelectedVersion = result;
			}
		}

		private void OnPlayButtonCommandExecuted()
		{
			new Task(async () =>
			{
				IsStarted = true;
				ProgressHidden = false;
				Title = "Meloncher " + SelectedVersion?.Name;
				_discordRpcTools.SetStatus("Играет на версии " + SelectedVersion?.Name, "");
				_mcLauncher.UseOptifine = LauncherSettings.UseOptifine;
				_mcLauncher.WindowMode = LauncherSettings.WindowMode;
				_mcLauncher.MaximumRamMb = LauncherSettings.MaximumRamMb;
				_mcLauncher.SetVersion(await SelectedVersion?.GetVersionAsync()!);
				_mcLauncher.Session = SelectedSession;
				try
				{
					await _mcLauncher.Update();
				}
				catch (Exception)
				{
					_mcLauncher.Offline = true;
				}

				ProgressValue = 0;
				ProgressText = null;
				ProgressHidden = true;
				await _mcLauncher.Launch();
				IsStarted = false;
				Title = "Meloncher";
				_discordRpcTools.SetStatus("Сидит в лаунчере", "");
			}).Start();
		}
	}
}