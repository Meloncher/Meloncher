using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace MeloncherAvalonia.ViewModels
{
	public class AddAccountViewModel : ViewModelBase
	{
		[Reactive] public string Username { get; set; }
		[Reactive] public string Password { get; set; }
		public AddAccountViewModel()
		{
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
		}

		public ReactiveCommand<Unit, AddAccountData> OkCommand { get; }

		private AddAccountData OnOkCommandExecuted()
		{
			return new AddAccountData(Username, Password);
		}
	}

	public class AddAccountData
	{
		public AddAccountData(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public string Username { get; set; }
		public string Password { get; set; }

	}
}
