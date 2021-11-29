using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia.Controls;
using CmlLib.Core.Version;
using MeloncherAvalonia.Models;
using MeloncherAvalonia.ViewModels.Dialogs;
using MeloncherAvalonia.Views.Dialogs;
using MeloncherCore.Account;
using MeloncherCore.Discord;
using MeloncherCore.Launcher;
using MeloncherCore.Launcher.Events;
using MeloncherCore.Logs;
using MeloncherCore.ModPack;
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
		private readonly VersionTools _versionTools;
		private readonly McLogs _mcLogs = new();

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;

			_path = new ExtMinecraftPath();
			_versionTools = new VersionTools(_path);
			_versionCollection = _versionTools.GetVersionMetadatas();

			_accountStorage = new AccountStorage(_path);
			_mcLauncher = new McLauncher(_path);
			_launcherSettings = LauncherSettings.New(_path);
			ModPackStorage = new ModPackStorage(_path);
			// SelectedVersion = _versionCollection.LatestReleaseVersion;
			SelectedVersion = _versionTools.GetMcVersion(_versionCollection.LatestReleaseVersion.GetVersion());
			if (_launcherSettings.SelectedVersion != null)
				try
				{
					SelectedVersion = _launcherSettings.SelectedProfileType switch
					{
						ProfileType.Vanilla => _versionTools.GetMcVersion(_launcherSettings.SelectedVersion),
						ProfileType.Custom => _versionTools.GetMcVersion(_launcherSettings.SelectedVersion, ModPackStorage),
						_ => SelectedVersion
					};
					// SelectedVersion = _versionCollection.GetVersionMetadata(_launcherSettings.SelectedVersion);
				}
				catch (Exception)
				{
					// ignored
				}

			if (_launcherSettings.SelectedAccount != null) SelectedAccount = _accountStorage.Get(_launcherSettings.SelectedAccount);

			_mcLauncher.McDownloadProgressChanged += OnMcLauncherOnMcDownloadProgressChanged;
			// _mcLauncher.ProgressChanged += OnMcLauncherOnProgressChanged;
			_mcLauncher.MinecraftOutput += e =>
			{
				_mcLogs.Parse(e.Line);
				McLogLines = new ObservableCollection<McLogLine>(_mcLogs.Lines);
			};

			_discordRpcTools.SetStatus("Using Meloncher", "");

			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "SelectedVersion" && SelectedVersion != null)
				{
					_launcherSettings.SelectedVersion = SelectedVersion.Name;
					_launcherSettings.SelectedProfileType = SelectedVersion.ProfileType;
				}

				if (e.PropertyName == "SelectedAccount") _launcherSettings.SelectedAccount = SelectedAccount?.GameSession.Username;
				if (e.PropertyName == "SelectedModPack")
				{
					var mcVersion = _versionTools.GetMcVersion(SelectedModPack, ModPackStorage);
					if (mcVersion != null) SelectedVersion = mcVersion;
				}
			};

			TransparencyLevelHint = _launcherSettings.GlassBackground ? WindowTransparencyLevel.Blur : WindowTransparencyLevel.None;
			_launcherSettings.PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == "GlassBackground") TransparencyLevelHint = _launcherSettings.GlassBackground ? WindowTransparencyLevel.Blur : WindowTransparencyLevel.None;
			};
			
			var launcherProfilesPath = Path.Combine(_path.MinecraftPath, "launcher_profiles.json");
			if (!File.Exists(launcherProfilesPath))
			{
				if (!Directory.Exists(_path.MinecraftPath)) Directory.CreateDirectory(_path.MinecraftPath);
				var text = "{\"clientToken\":\"\",\"launcherVersion\":{\"format\":21,\"name\":\"Meloncher\",\"profilesFormat\":2 },\"profiles\":{},\"settings\":{}}";
				_ = File.WriteAllTextAsync(launcherProfilesPath, text);
			}
		}

		[Reactive] public ModPackStorage ModPackStorage { get; set; }
		[Reactive] public string? SelectedModPack { get; set; }
		[Reactive] public string Logs { get; set; } = "";
		[Reactive] public int SelectedTabIndex { get; set; }

		[Reactive] public string Title { get; set; } = "Meloncher";
		[Reactive] public WindowTransparencyLevel TransparencyLevelHint { get; set; }
		[Reactive] public int ProgressValue { get; set; }
		[Reactive] public string? ProgressText { get; set; }
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsStarted { get; private set; }
		[Reactive] public bool IsLaunched { get; private set; }
		[Reactive] public McVersion? SelectedVersion { get; set; }
		[Reactive] public McAccount? SelectedAccount { get; set; } = new("Player");
		[Reactive] public ObservableCollection<McLogLine> McLogLines { get; set; } = new();

		public async Task CheckUpdates()
		{
			Updater updater = new();
			var updaterJson = updater.CheckUpdates();
			if (updaterJson != null)
			{
				var res = await MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
				{
					ButtonDefinitions = ButtonEnum.YesNo,
					ContentHeader = "Update now?",
					ContentTitle = "Update",
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
							ContentTitle = "Update",
							ContentMessage = "Update error",
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
					"Resource" => "Checking Resources...",
					"Runtime" => "Checking Java...",
					"Library" => "Checking Libraries...",
					"Minecraft" => "Checking Minecraft...",
					"Optifine" => "Checking Optifine...",
					_ => "Checking Files..."
				};
			else
				ProgressText = e.Type switch
				{
					"Resource" => "Downloading Resources...",
					"Runtime" => "Downloading Java...",
					"Library" => "Downloading Libraries...",
					"Minecraft" => "Downloading Minecraft...",
					"Optifine" => "Downloading Optifine...",
					_ => "Downloading..."
				};
		}

		private async Task OpenAddModPackWindowCommand()
		{
			var dialog = new AddModPackDialog
			{
				DataContext = new AddModPackViewModel(_versionTools, _versionCollection)
			};
			var result = await DialogHost.DialogHost.Show(dialog);
			if (result is KeyValuePair<string, ModPackInfo>(var key, var value)) ModPackStorage.Add(key, value);
		}

		private void RemoveSelectedModPackCommand()
		{
			if (SelectedModPack != null) ModPackStorage.Remove(SelectedModPack);
		}

		private async Task EditSelectedModPackCommand()
		{
			if (SelectedModPack == null) return; 
			var dialog = new AddModPackDialog
			{
				DataContext = new AddModPackViewModel(_versionTools, _versionCollection, new KeyValuePair<string, ModPackInfo>(SelectedModPack, ModPackStorage.Get(SelectedModPack)))
			};
			var result = await DialogHost.DialogHost.Show(dialog);
			if (result is KeyValuePair<string, ModPackInfo>(var key, var value))
			{
				ModPackStorage.Edit(SelectedModPack, value);
				ModPackStorage.Rename(SelectedModPack, key);
				SelectedModPack = key;
			}
		}

		private void SelectModPackCommand()
		{
			SelectedTabIndex = 0;
			var mcVersion = _versionTools.GetMcVersion(SelectedModPack, ModPackStorage);
			if (mcVersion != null) SelectedVersion = mcVersion;
		}

		private void OpenModPackFolderCommand()
		{
			var modPackPath = Path.Combine(_path.RootPath, "profiles", "custom");
			if (SelectedModPack != null) modPackPath = Path.Combine(modPackPath, SelectedModPack);
			Process.Start("explorer.exe", modPackPath);
		}

		private async Task OpenSettingsWindowCommand()
		{
			var dialog = new SettingsDialog
			{
				DataContext = new SettingsViewModel(_launcherSettings)
			};
			var result = await DialogHost.DialogHost.Show(dialog);
			if (result is SettingsAction.Import)
			{
				var importer = new McOptionsImporter(_path);
				importer.Import();
				var messageBoxStandardWindow = MessageBoxManager.GetMessageBoxStandardWindow("Готово", "Готово!");
				await messageBoxStandardWindow.Show();
			}
		}

		private async Task OpenAccountsWindowCommand()
		{
			var dialog = new AccountSelectorDialog
			{
				DataContext = new AccountsViewModel(_accountStorage, SelectedAccount)
			};
			var result = await DialogHost.DialogHost.Show(dialog);
			if (result is McAccount mcAccount) SelectedAccount = mcAccount;
		}

		private async Task OpenVersionsWindowCommand()
		{
			var dialog = new VersionSelectorDialog
			{
				DataContext = new VersionsViewModel("MainDialogHost", _versionTools, _versionCollection)
			};
			var result = await DialogHost.DialogHost.Show(dialog);
			if (result is MVersionMetadata mVersionMetadata) SelectedVersion = _versionTools.GetMcVersion(mVersionMetadata.GetVersion());
		}

		private void PlayButtonCommand()
		{
			new Task(async () =>
			{
				IsStarted = true;
				ProgressHidden = false;
				Title = "Meloncher " + SelectedVersion?.Name;
				_discordRpcTools.SetStatus("Playing on " + SelectedVersion?.Name, "");
				_mcLauncher.UseOptifine = _launcherSettings.UseOptifine;
				_mcLauncher.WindowMode = _launcherSettings.WindowMode;
				_mcLauncher.MaximumRamMb = _launcherSettings.MaximumRamMb;
				_mcLauncher.JvmArguments = _launcherSettings.JvmArguments;
				// if (SelectedVersion != null) _mcLauncher.Version = _versionTools.GetMcVersion(SelectedVersion.MVersion.Id);
				if (SelectedVersion != null) _mcLauncher.Version = SelectedVersion;
				// var test = new DefaultVersionLoader(_path).GetVersionMetadatas().GetVersion("1.12.2");
				// _mcLauncher.Version = new McVersion(test, ProfileType.Custom, "TestModPack");

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
				_discordRpcTools.SetStatus("Using Meloncher", "");
			}).Start();
		}
	}
}