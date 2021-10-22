using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.VersionLoader;
using MeloncherCore.Account;
using MeloncherCore.Launcher;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class AccountsViewModel : ViewModelBase
	{
		private readonly AccountStorage _accountStorage;

		public AccountsViewModel(AccountStorage accountStorage, MSession? selectedSession)
		{
			SelectedSession = selectedSession;
			DeleteAccountCommand = ReactiveCommand.Create(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = ReactiveCommand.Create(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = ReactiveCommand.Create(OnAddMojangCommandExecuted);
			AddOfflineCommand = ReactiveCommand.Create(OnAddOfflineCommandExecuted);
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			_accountStorage = accountStorage;
			accountStorage.CollectionChanged += (_, _) => Accounts = new ObservableCollection<MSession>(accountStorage);
			Accounts = new ObservableCollection<MSession>(accountStorage);
		}

		public AccountsViewModel()
		{
		}

		[Reactive] public ObservableCollection<MSession> Accounts { get; set; }
		[Reactive] public MSession? SelectedSession { get; set; }
		public Interaction<AddAccountViewModel, AddAccountData?> ShowAddAccountDialog { get; } = new();
		public Interaction<AddMicrosoftAccountViewModel, string?> ShowAddMicrosoftAccountDialog { get; } = new();

		public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }

		public ReactiveCommand<Unit, Task> AddMicrosoftCommand { get; }

		public ReactiveCommand<Unit, MSession> OkCommand { get; }

		public ReactiveCommand<Unit, Task> AddMojangCommand { get; }

		public ReactiveCommand<Unit, Task> AddOfflineCommand { get; }

		private void OnDeleteAccountCommandExecuted()
		{
			_accountStorage.Remove(SelectedSession);
		}

		private async Task OnAddMicrosoftCommandExecuted()
		{
			var lh = new LoginHandler();
			var psi = new ProcessStartInfo();
			psi.UseShellExecute = true;
			psi.FileName = lh.CreateOAuthUrl();
			Process.Start(psi);
			var dialog = new AddMicrosoftAccountViewModel();
			var result = await ShowAddMicrosoftAccountDialog.Handle(dialog);
			if (result != null)
			{
				if (lh.CheckOAuthLoginSuccess(result.ToString()))
					_accountStorage.Add(lh.LoginFromOAuth());
			}
		}

		private MSession OnOkCommandExecuted()
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
				if (resp.Session != null) _accountStorage.Add(resp.Session);
			}
		}

		private async Task OnAddOfflineCommandExecuted()
		{
			var dialog = new AddAccountViewModel(false);
			var result = await ShowAddAccountDialog.Handle(dialog);
			if (result != null)
			{
				MSession session = MSession.GetOfflineSession(result.Username);
				_accountStorage.Add(session);
			}
		}
	}
}