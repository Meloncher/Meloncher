using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using MeloncherCore.Account;
using MeloncherCore.Launcher;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class AccountsViewModel : ViewModelBase
	{
		private AccountStorage accountStorage;

		public AccountsViewModel()
		{
			DeleteAccountCommand = ReactiveCommand.Create(OnDeleteAccountCommandExecuted);
			AddMicrosoftCommand = ReactiveCommand.Create(OnAddMicrosoftCommandExecuted);
			AddMojangCommand = ReactiveCommand.Create(OnAddMojangCommandExecuted);
			AddOfflineCommand = ReactiveCommand.Create(OnAddOfflineCommandExecuted);
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			var path = new ExtMinecraftPath();
			accountStorage = new AccountStorage(path);
			accountStorage.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => Accounts = new ObservableCollection<MSession>(accountStorage);
			Accounts = new ObservableCollection<MSession>(accountStorage);
		}

		[Reactive] public ObservableCollection<MSession> Accounts { get; set; }
		[Reactive] public int selectedAccount { get; set; } = 0;
		public Interaction<AddAccountViewModel, AddAccountData?> ShowAddAccountDialog { get; } = new Interaction<AddAccountViewModel, AddAccountData?>();
		public Interaction<AddMicrosoftAccountViewModel, string?> ShowAddMicrosoftAccountDialog { get; } = new Interaction<AddMicrosoftAccountViewModel, string?>();

		public ReactiveCommand<Unit, Unit> DeleteAccountCommand { get; }

		public ReactiveCommand<Unit, Task> AddMicrosoftCommand { get; }

		public ReactiveCommand<Unit, MSession> OkCommand { get; }

		public ReactiveCommand<Unit, Task> AddMojangCommand { get; }

		public ReactiveCommand<Unit, Task> AddOfflineCommand { get; }

		private void OnDeleteAccountCommandExecuted()
		{
			accountStorage.RemoveAt(selectedAccount);
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
					accountStorage.Add(lh.LoginFromOAuth());
			}
		}

		private MSession OnOkCommandExecuted()
		{
			if (Accounts.Count > selectedAccount) return Accounts[selectedAccount];
			return null;
		}

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