using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
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
			DeleteAccountCommand = ReactiveCommand.Create(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = ReactiveCommand.Create(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = ReactiveCommand.Create(OnAddMojangCommandExecuted);
			AddOfflineCommand = ReactiveCommand.Create(OnAddOfflineCommandExecuted);
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			_accountStorage = accountStorage;
			accountStorage.CollectionChanged += (_, _) => Accounts = new ObservableCollection<McAccount>(accountStorage);
			Accounts = new ObservableCollection<McAccount>(accountStorage);
		}

		public AccountsViewModel()
		{
		}

		[Reactive] public ObservableCollection<McAccount> Accounts { get; set; }
		[Reactive] public McAccount? SelectedSession { get; set; }
		public Interaction<AddAccountViewModel, AddAccountData?> ShowAddAccountDialog { get; } = new();
		public Interaction<AddMicrosoftAccountViewModel, string?> ShowAddMicrosoftAccountDialog { get; } = new();

		public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }

		public ReactiveCommand<Unit, Task> AddMicrosoftCommand { get; }

		public ReactiveCommand<Unit, McAccount> OkCommand { get; }

		public ReactiveCommand<Unit, Task> AddMojangCommand { get; }

		public ReactiveCommand<Unit, Task> AddOfflineCommand { get; }

		private void OnDeleteAccountCommandExecuted()
		{
			_accountStorage.Remove(SelectedSession);
		}

		private async Task OnAddMicrosoftCommandExecuted()
		{
			var lh = new MicrosoftLoginHandler();
			Process.Start(new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = lh.CreateOAuthUrl()
			});
			var dialog = new AddMicrosoftAccountViewModel();
			var result = await ShowAddMicrosoftAccountDialog.Handle(dialog);
			if (result != null)
				if (lh.CheckOAuthLoginSuccess(result))
					_accountStorage.Add(lh.LoginFromOAuth());
		}

		private McAccount OnOkCommandExecuted()
		{
			return SelectedSession;
		}

		private async Task OnAddMojangCommandExecuted()
		{
			var dialog = new AddAccountViewModel(true);
			var result = await ShowAddAccountDialog.Handle(dialog);
			if (result != null)
			{
				var login = new MLogin();
				var resp = login.Authenticate(result.Username, result.Password);
				if (resp.Session != null) _accountStorage.Add(new McAccount(resp.Session));
			}
		}

		private async Task OnAddOfflineCommandExecuted()
		{
			var dialog = new AddAccountViewModel(false);
			var result = await ShowAddAccountDialog.Handle(dialog);
			if (result != null)
			{
				MSession session = MSession.GetOfflineSession(result.Username);
				_accountStorage.Add(new McAccount(session));
			}
		}
	}
}