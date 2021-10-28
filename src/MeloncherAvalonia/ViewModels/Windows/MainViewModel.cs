using System;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using CmlLib.Core.Auth;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using MeloncherAvalonia.Models;
using MeloncherAvalonia.ViewModels.Dialogs;
using MeloncherCore.Account;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using MeloncherCore.Launcher.Events;
using MeloncherCore.Options;
using MeloncherCore.Settings;
using MeloncherCore.Version;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Windows
{
	public class MainViewModel : ViewModelBase
	{
		private readonly AccountStorage _accountStorage;
		private readonly DiscordRpcTools _discordRpcTools = new();
		private readonly LauncherSettings _launcherSettings;

		private readonly McLauncher _mcLauncher;
		private readonly ExtMinecraftPath _path;
		private readonly MVersionCollection _versionCollection;
		private readonly IVersionLoader _versionLoader;
		private readonly VersionTools _versionTools;

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			PlayButtonCommand = ReactiveCommand.Create(OnPlayButtonCommandExecuted);
			OpenAccountsWindowCommand = ReactiveCommand.Create(OnOpenAccountsWindowCommandExecuted);
			OpenVersionsWindowCommand = ReactiveCommand.Create(OnOpenVersionsWindowCommandExecuted);
			OpenSettingsWindowCommand = ReactiveCommand.Create(OnOpenSettingsWindowCommandExecuted);

			_path = new ExtMinecraftPath();
			_versionTools = new VersionTools(_path);
			_versionCollection = _versionTools.GetVersionMetadatas();
			// _versionCollection = new LocalVersionLoader(_path).GetVersionMetadatas();

			_accountStorage = new AccountStorage(_path);
			_mcLauncher = new McLauncher(_path);
			_launcherSettings = LauncherSettings.New(_path);
			SelectedVersion = _versionCollection.LatestReleaseVersion;
			if (_launcherSettings.SelectedVersion != null)
				try
				{
					SelectedVersion = _versionCollection.GetVersionMetadata(_launcherSettings.SelectedVersion);
				}
				catch (Exception)
				{
					// ignored
				}

			if (_launcherSettings.SelectedAccount != null) SelectedAccount = _accountStorage.Get(_launcherSettings.SelectedAccount);

			_mcLauncher.McDownloadProgressChanged += OnMcLauncherOnMcDownloadProgressChanged;
			// _mcLauncher.ProgressChanged += OnMcLauncherOnProgressChanged;
			_mcLauncher.MinecraftOutput += e => { _discordRpcTools.OnLog(e.Line); };

			_discordRpcTools.SetStatus("Сидит в лаунчере", "");

			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "SelectedVersion") _launcherSettings.SelectedVersion = SelectedVersion?.Name;
				if (e.PropertyName == "SelectedAccount") _launcherSettings.SelectedAccount = SelectedAccount?.GameSession.Username;
			};
		}

		[Reactive] public string Title { get; set; } = "Meloncher";
		[Reactive] public int ProgressValue { get; set; }
		[Reactive] public string? ProgressText { get; set; }
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsStarted { get; private set; }
		[Reactive] public bool IsLaunched { get; private set; }

		public Interaction<AccountsViewModel, McAccount?> ShowSelectAccountDialog { get; } = new();
		public Interaction<VersionsViewModel, MVersionMetadata?> ShowSelectVersionDialog { get; } = new();
		public Interaction<SettingsViewModel, SettingsAction?> ShowSettingsDialog { get; } = new();
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		[Reactive] public McAccount? SelectedAccount { get; set; } = new(MSession.GetOfflineSession("Player"));

		public ReactiveCommand<Unit, Task> OpenAccountsWindowCommand { get; }
		public ReactiveCommand<Unit, Task> OpenVersionsWindowCommand { get; }
		public ReactiveCommand<Unit, Unit> PlayButtonCommand { get; }

		public ReactiveCommand<Unit, Task> OpenSettingsWindowCommand { get; }

		public async Task CheckUpdates()
		{
			Updater updater = new();
			var updaterJson = updater.CheckUpdates();
			if (updaterJson != null)
			{
				var res = await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
				{
					ButtonDefinitions = ButtonEnum.YesNo,
					ContentHeader = "Обновить сейчас?",
					ContentTitle = "Обновление",
					ContentMessage = updaterJson.Description,
					WindowStartupLocation = WindowStartupLocation.CenterScreen,
					Topmost = true
				}).Show();
				if (res == ButtonResult.Yes)
				{
					if (updater.Update())
						Environment.Exit(0);
					else
						await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
						{
							ButtonDefinitions = ButtonEnum.Ok,
							ContentTitle = "Обновление",
							ContentMessage = "Ошибка при обновлении",
							WindowStartupLocation = WindowStartupLocation.CenterScreen,
							Topmost = true
						}).Show();
				}
			}
		}

		private void OnMcLauncherOnMcDownloadProgressChanged(McDownloadProgressEventArgs e)
		{
			ProgressValue = e.ProgressPercentage;
			if (e.IsChecking)
				ProgressText = e.Type switch
				{
					"Resource" => "Проверка Ресурсов...",
					"Runtime" => "Проверка Java...",
					"Library" => "Проверка Библиотек...",
					"Minecraft" => "Проверка Minecraft...",
					"Optifine" => "Проверка Optifine...",
					_ => "Проверка Файлов..."
				};
			else
				ProgressText = e.Type switch
				{
					"Resource" => "Загрузка Ресурсов...",
					"Runtime" => "Загрузка Java...",
					"Library" => "Загрузка Библиотек...",
					"Minecraft" => "Загрузка Minecraft...",
					"Optifine" => "Загрузка Optifine...",
					_ => "Загрузка..."
				};
		}

		private async Task OnOpenSettingsWindowCommandExecuted()
		{
			var dialog = new SettingsViewModel(_launcherSettings);
			var result = await ShowSettingsDialog.Handle(dialog);
			if (result != null)
				if (result == SettingsAction.Import)
				{
					var importer = new McOptionsImporter(_path);
					importer.Import();
					var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Готово", "Готово!");
					await messageBoxStandardWindow.Show();
				}
		}

		private async Task OnOpenAccountsWindowCommandExecuted()
		{
			var dialog = new AccountsViewModel(_accountStorage, SelectedAccount);
			var result = await ShowSelectAccountDialog.Handle(dialog);
			if (result != null) SelectedAccount = result;
		}

		private async Task OnOpenVersionsWindowCommandExecuted()
		{
			var dialog = new VersionsViewModel(_versionTools, _versionCollection, SelectedVersion);
			var result = await ShowSelectVersionDialog.Handle(dialog);
			if (result != null) SelectedVersion = result;
		}

		private void OnPlayButtonCommandExecuted()
		{
			new Task(async () =>
			{
				IsStarted = true;
				ProgressHidden = false;
				Title = "Meloncher " + SelectedVersion?.Name;
				_discordRpcTools.SetStatus("Играет на версии " + SelectedVersion?.Name, "");
				_mcLauncher.UseOptifine = _launcherSettings.UseOptifine;
				_mcLauncher.WindowMode = _launcherSettings.WindowMode;
				_mcLauncher.MaximumRamMb = _launcherSettings.MaximumRamMb;
				if (SelectedVersion != null) _mcLauncher.Version = _versionTools.GetMcVersion(SelectedVersion.Name);

				if (SelectedAccount != null)
				{
					if (!SelectedAccount.Validate())
						if (SelectedAccount.Refresh())
							_accountStorage.SaveFile();
					_mcLauncher.Session = SelectedAccount.GameSession;
				}

				await _mcLauncher.Update();

				ProgressValue = 0;
				ProgressText = null;
				ProgressHidden = true;
				IsLaunched = true;
				await _mcLauncher.Launch();
				IsStarted = false;
				IsLaunched = false;
				Title = "Meloncher";
				_discordRpcTools.SetStatus("Сидит в лаунчере", "");
			}).Start();
		}
	}
}