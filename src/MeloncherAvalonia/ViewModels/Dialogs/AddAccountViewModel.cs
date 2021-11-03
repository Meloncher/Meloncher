using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddAccountViewModel : ViewModelBase
	{
		public AddAccountViewModel(bool needPassword)
		{
			NeedPassword = needPassword;
		}

		[Reactive] public string? Username { get; set; }
		[Reactive] public string? Password { get; set; }
		[Reactive] public bool NeedPassword { get; set; }

		private void OkCommand()
		{
			DialogHost.DialogHost.GetDialogSession("AccountSelectorDialogHost")?.Close(new AddAccountData(Username, Password));
		}
	}

	public class AddAccountData
	{
		public AddAccountData(string? username, string? password)
		{
			Username = username;
			Password = password;
		}

		public string? Username { get; set; }
		public string? Password { get; set; }
	}
}