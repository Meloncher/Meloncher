using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class AddAccountViewModel : ViewModelBase
	{
		public AddAccountViewModel()
		{
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
		}

		[Reactive] public string Username { get; set; }
		[Reactive] public string Password { get; set; }

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