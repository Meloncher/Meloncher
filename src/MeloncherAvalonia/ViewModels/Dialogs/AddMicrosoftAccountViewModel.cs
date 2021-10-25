using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddMicrosoftAccountViewModel : ViewModelBase
	{
		public AddMicrosoftAccountViewModel()
		{
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
		}

		[Reactive] public string RedirectUrl { get; set; }

		public ReactiveCommand<Unit, string> OkCommand { get; }

		private string OnOkCommandExecuted()
		{
			return RedirectUrl;
		}
	}
}