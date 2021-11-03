using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using MeloncherAvalonia.Views.Dialogs;
using MeloncherCore.Account;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AccountsViewModel : ViewModelBase
	{
		private readonly AccountStorage _accountStorage;

		public AccountsViewModel(AccountStorage accountStorage, McAccount? selectedSession)
		{
			SelectedSession = selectedSession;
			_accountStorage = accountStorage;
			accountStorage.CollectionChanged += (_, _) => Accounts = new ObservableCollection<McAccount>(accountStorage);
			Accounts = new ObservableCollection<McAccount>(accountStorage);
		}

		[Reactive] public ObservableCollection<McAccount> Accounts { get; set; }
		[Reactive] public McAccount? SelectedSession { get; set; }
		public Interaction<AddAccountViewModel, AddAccountData?> ShowAddAccountDialog { get; } = new();
		public Interaction<AddMicrosoftAccountViewModel, string?> ShowAddMicrosoftAccountDialog { get; } = new();

		private void DeleteAccountCommand()
		{
			_accountStorage.Remove(SelectedSession);
		}

		private async Task AddMicrosoftCommand()
		{
			var lh = new MicrosoftLoginHandler();
			Process.Start(new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = lh.CreateOAuthUrl()
			});

			var dialog = new AddMicrosoftAccountDialog
			{
				DataContext = new AddMicrosoftAccountViewModel()
			};
			var result = await DialogHost.DialogHost.Show(dialog, "AccountSelectorDialogHost");
			if (result is string url)
				if (lh.CheckOAuthLoginSuccess(url))
					_accountStorage.Add(lh.LoginFromOAuth());
		}

		private void OkCommand()
		{
			DialogHost.DialogHost.GetDialogSession("MainDialogHost")?.Close(SelectedSession);
		}

		private async Task AddMojangCommand()
		{
			var dialog = new AddAccountDialog
			{
				DataContext = new AddAccountViewModel(true)
			};
			var result = await DialogHost.DialogHost.Show(dialog, "AccountSelectorDialogHost");
			if (result is AddAccountData addAccountData)
				if (addAccountData.Username != null && addAccountData.Password != null)
				{
					var resp = new MLogin().Authenticate(addAccountData.Username, addAccountData.Password);
					if (resp.Session != null) _accountStorage.Add(new McAccount(resp.Session));
				}
		}

		private async Task AddOfflineCommand()
		{
			var dialog = new AddAccountDialog
			{
				DataContext = new AddAccountViewModel(false)
			};
			var result = await DialogHost.DialogHost.Show(dialog, "AccountSelectorDialogHost");
			if (result is AddAccountData addAccountData)
				if (addAccountData.Username != null)
					_accountStorage.Add(new McAccount(addAccountData.Username));
		}
	}
}