using CmlLib.Core.Auth;
using MeloncherCore.Account;
using MeloncherCore.Launcher;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherAvalonia.ViewModels
{
	public class AccountsViewModel : ViewModelBase
	{
		public AccountsViewModel()
		{
			DeleteAccountCommand = ReactiveCommand.Create(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = ReactiveCommand.Create(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = ReactiveCommand.Create(OnAddMojangCommandExecuted);
			AddOfflineCommand = ReactiveCommand.Create(OnAddOfflineCommandExecuted);
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			ShowAddAccountDialog = new Interaction<AddAccountViewModel, AddAccountData?>();
			var path = new ExtMinecraftPath();
			accountStorage = new AccountStorage(path);
			accountStorage.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => Accounts = new ObservableCollection<MSession>(accountStorage);
			Accounts = new ObservableCollection<MSession>(accountStorage);
		}

		[Reactive] public ObservableCollection<MSession> Accounts { get; set; }
		[Reactive] public int selectedAccount { get; set; } = 0;
		public Interaction<AddAccountViewModel, AddAccountData?> ShowAddAccountDialog { get; }

		private AccountStorage accountStorage;

		public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }
		private void OnDeleteAccountCommandExecuted()
		{
			accountStorage.RemoveAt(selectedAccount);
		}

		public ReactiveCommand<Unit, Unit> AddMicrosoftCommand { get; }
		private void OnAddMicrosoftCommandExecuted()
		{
			//MicrosoftLoginWindow loginWindow = new MicrosoftLoginWindow();
			//MSession session = loginWindow.ShowLoginDialog();
			//accountStorage.Add(session);
		}

		public ReactiveCommand<Unit, MSession> OkCommand { get; }
		private MSession OnOkCommandExecuted()
		{
			if (Accounts.Count > selectedAccount) return Accounts[selectedAccount];
			return null;
		}

		public ReactiveCommand<Unit, Task> AddMojangCommand { get; }
		private async Task OnAddMojangCommandExecuted()
		{
			var dialog = new AddAccountViewModel();
			var result = await ShowAddAccountDialog.Handle(dialog);
			if (result != null)
			{
				var login = new MLogin();
				var resp = login.Authenticate(result.Username, result.Password);
				if (resp.Session != null) accountStorage.Add(resp.Session);
			}
		}

		public ReactiveCommand<Unit, Task> AddOfflineCommand { get; }
		private async Task OnAddOfflineCommandExecuted()
		{
			var dialog = new AddAccountViewModel();
			var result = await ShowAddAccountDialog.Handle(dialog);
			if (result != null)
			{
				MSession session = MSession.GetOfflineSession(result.Username);
				accountStorage.Add(session);
			}
		}
	}
}
