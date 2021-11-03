using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using MeloncherCore.ModPack;
using MeloncherCore.Version;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddModPackViewModel : ViewModelBase
	{
		private readonly VersionTools _versionTools;
		private readonly MVersionCollection? _versionCollection;

		public AddModPackViewModel(VersionTools versionTools, MVersionCollection? versionCollection)
		{
			_versionTools = versionTools;
			_versionCollection = versionCollection;
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
		}

		public ReactiveCommand<Unit, KeyValuePair<string, ModPackInfo>> OkCommand { get; }

		private async Task OpenVersionsWindowCommand()
		{
			var dialog = new VersionsViewModel(_versionTools, _versionCollection);
			var result = await ShowSelectVersionDialog.Handle(dialog);
			if (result != null) SelectedVersionMetadata = result;
		}

		private KeyValuePair<string, ModPackInfo> OnOkCommandExecuted()
		{
			return new KeyValuePair<string, ModPackInfo>(Name, new ModPackInfo(SelectedVersionMetadata.GetVersion().Id));
		}


		[Reactive] public MVersionMetadata SelectedVersionMetadata { get; set; }
		[Reactive] public string Name { get; set; }
		public Interaction<VersionsViewModel, MVersionMetadata?> ShowSelectVersionDialog { get; } = new();
	}
}