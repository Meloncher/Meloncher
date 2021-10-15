using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;

namespace MeloncherAvalonia.ViewModels
{
	public class AddMicrosoftAccountViewModel : ViewModelBase
	{
		[Reactive] public string RedirectUrl { get; set; }

		public AddMicrosoftAccountViewModel()
		{
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
		}

		public ReactiveCommand<Unit, string> OkCommand { get; }

		private string OnOkCommandExecuted()
		{
			return RedirectUrl;
		}
	}
}
