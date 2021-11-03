using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddMicrosoftAccountViewModel : ViewModelBase
	{
		[Reactive] public string RedirectUrl { get; set; }

		private void OkCommand()
		{
			DialogHost.DialogHost.GetDialogSession("AccountSelectorDialogHost")?.Close(RedirectUrl);
		}
	}
}