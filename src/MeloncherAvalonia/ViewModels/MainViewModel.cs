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
using MeloncherAvalonia.Views;
using Avalonia.Controls;
using System.Reactive.Linq;

namespace MeloncherAvalonia.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
		
		[Reactive] public string Title { get; set; } = "Meloncher";
		[Reactive] public bool Optifine { get; set; } = true;
		[Reactive] public bool Offline { get; set; } = false;
		[Reactive] public int ProgressValue { get; set; } = 0;
		[Reactive] public string ProgressText { get; set; } = null;
		[Reactive] public bool ProgressHidden { get; set; } = true;
		[Reactive] public bool IsNotStarted { get; set; } = true;

		private McLauncher mcLauncher;
		private IVersionLoader versionLoader;
		DiscrodRPCTools discrodRPCTools = new DiscrodRPCTools();

		public Interaction<AccountsViewModel, MSession?> ShowSelectAccountDialog { get; }

		public MainViewModel()
		{
			ServicePointManager.DefaultConnectionLimit = 512;
			ShowSelectAccountDialog = new Interaction<AccountsViewModel, MSession?>();
			PlayButtonCommand = ReactiveCommand.Create(OnPlayButtonCommandExecuted);
			OpenAccountsWindowCommand = ReactiveCommand.Create(OnOpenAccountsWindowCommandExecuted);

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
			
			discrodRPCTools.SetStatus("Сидит в лаунчере", "");

			var mdts = versionLoader.GetVersionMetadatas();
			Versions = new ObservableCollection<MVersionMetadata>(mdts);
			SelectedVersion = mdts.LatestReleaseVersion;
		}
		[Reactive] public ObservableCollection<MVersionMetadata> Versions { get; set; }
		[Reactive] public MVersionMetadata ?SelectedVersion { get; set; }
		[Reactive] public MSession? SelectedSession { get; set; }

		public ReactiveCommand<Unit, Task> OpenAccountsWindowCommand { get; }
		private async Task OnOpenAccountsWindowCommandExecuted()
		{
			var dialog = new AccountsViewModel();
			var result = await ShowSelectAccountDialog.Handle(dialog);
			if (result != null)
			{
				SelectedSession = result;
			}
		}

		public ReactiveCommand<Unit, Unit> PlayButtonCommand { get; }
		private void OnPlayButtonCommandExecuted()
		{
			MSession session = SelectedSession;
			if (session == null) session = MSession.GetOfflineSession("Player");
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
