using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using MeloncherCore.Account;
using MeloncherCore.Launcher;
using MeloncherCore.Version;
using MeloncherWPF.Infrastructure.Commands;
using MeloncherWPF.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace MeloncherWPF.ViewModels
{
	internal class MainViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string TestConsole { get; set; } = "";
		public string Title { get; set; } = "Meloncher";
		public string McVersionName { get; set; } = "1.12.2";
		public string Username { get; set; } = "Steve";
		public bool Optifine { get; set; } = true;
		public bool Offline { get; set; } = false;
		public int ProgressValue { get; set; } = -1;
		public bool IsNotStarted { get; set; } = true;

		private McLauncher mcLauncher;
		public MainViewModel()
		{
			PlayButtonCommand = new LambdaCommand(OnPlayButtonCommandExecuted);
			OpenAccountsWindowCommand = new LambdaCommand(OnOpenAccountsWindowCommandExecuted);
			DeleteAccountCommand = new LambdaCommand(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = new LambdaCommand(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = new LambdaCommand(OnAddMojangCommandExecuted);
			AddOfflineCommand = new LambdaCommand(OnAddOfflineCommandExecuted);
			//mcLauncher = new McLauncher(new ExtMinecraftPath("D:\\MeloncherNetTest"));
			mcLauncher = new McLauncher(new ExtMinecraftPath("Data"));
			mcLauncher.FileChanged += (e) =>
			{
				TestLog(String.Format("[{0}] {1} - {2}/{3}", e.FileKind.ToString(), e.FileName, e.ProgressedFileCount, e.TotalFileCount));
			};
			mcLauncher.ProgressChanged += (s, e) =>
			{
				ProgressValue = e.ProgressPercentage;
			};
			testAccountsViewModel();
		}

		public void testAccountsViewModel()
		{
			
			var dispatcher = Dispatcher.CurrentDispatcher;
			new Task(async () =>
			{
				accountStorage = new AccountStorage(mcLauncher.MinecraftPath);
				await accountStorage.ReadFile();
				dispatcher.Invoke(reloadAccList);
			}).Start();
		}

		private void reloadAccList() 
		{
			Accounts = new ObservableCollection<MSession>();
			var list = accountStorage.GetList();
			list.ForEach((acc) =>
			{
				Accounts.Add(acc);
			});
		}

		private AccountStorage accountStorage;
		public ObservableCollection<MSession> Accounts { get; set; }
		public int selectedAccount { get; set; } = 0;

		public ICommand DeleteAccountCommand { get; }
		private void OnDeleteAccountCommandExecuted(Object p)
		{
			accountStorage.RemoveAt(selectedAccount);
			reloadAccList();
			_ = accountStorage.SaveFile();
		}

		public ICommand AddMicrosoftCommand { get; }
		private void OnAddMicrosoftCommandExecuted(object p)
		{
			MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			MSession session = loginWindow.ShowLoginDialog();
			accountStorage.Add(session);
			reloadAccList();
			_ = accountStorage.SaveFile();
		}

		public ICommand AddMojangCommand { get; }
		private void OnAddMojangCommandExecuted(object p)
		{
			var dialog = new AddAccountWindow();
			if (dialog.ShowDialog() == true)
			{
				var login = new MLogin();
				var resp = login.Authenticate(dialog.ResponseLogin, dialog.ResponsePass);
				MSession session = resp.Session;
				accountStorage.Add(session);
				reloadAccList();
				_ = accountStorage.SaveFile();
			}
		}

		public ICommand AddOfflineCommand { get; }
		private void OnAddOfflineCommandExecuted(object p)
		{
			var dialog = new AddAccountWindow();
			if (dialog.ShowDialog() == true)
			{
				MSession session = MSession.GetOfflineSession(dialog.ResponseLogin);
				accountStorage.Add(session);
				reloadAccList();
				_ = accountStorage.SaveFile();
			}
		}

		void TestLog(string text)
		{
			TestConsole = text + "\n" + TestConsole;
		}

		public ICommand OpenAccountsWindowCommand { get; }
		private void OnOpenAccountsWindowCommandExecuted(object p)
		{
			//accountsWindow.Activate();
		}

		public ICommand PlayButtonCommand { get; }
		private void OnPlayButtonCommandExecuted(object p)
		{
			//MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			//MSession session = loginWindow.ShowLoginDialog();
			MSession session = MSession.GetOfflineSession("Gomosek228");
			if (Accounts.Count > 0)
				session = Accounts[selectedAccount];
			new Task(async () => {

				//var path = new ExtMinecraftPath("D:\\MeloncherNetTest", $"profiles\\qwe");
				//AccountStorage accountStorage = new AccountStorage(mcLauncher.MinecraftPath);
				//await accountStorage.ReadFile();
				//accountStorage.Add(session);
				//await accountStorage.SaveFile();

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
