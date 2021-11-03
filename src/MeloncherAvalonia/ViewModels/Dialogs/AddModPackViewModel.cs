using System.Collections.Generic;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using MeloncherAvalonia.Views.Dialogs;
using MeloncherCore.ModPack;
using MeloncherCore.Version;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddModPackViewModel : ViewModelBase
	{
		private readonly MVersionCollection? _versionCollection;
		private readonly VersionTools _versionTools;

		public AddModPackViewModel(VersionTools versionTools, MVersionCollection? versionCollection)
		{
			_versionTools = versionTools;
			_versionCollection = versionCollection;
		}


		[Reactive] public MVersionMetadata SelectedVersionMetadata { get; set; }
		[Reactive] public string Name { get; set; }
		public Interaction<VersionsViewModel, MVersionMetadata?> ShowSelectVersionDialog { get; } = new();

		private async Task OpenVersionsWindowCommand()
		{
			var dialog = new VersionSelectorDialog
			{
				DataContext = new VersionsViewModel("AddModPackDialogHost", _versionTools, _versionCollection)
			};
			var result = await DialogHost.DialogHost.Show(dialog, "AddModPackDialogHost");
			if (result is MVersionMetadata mVersionMetadata) SelectedVersionMetadata = mVersionMetadata;
		}

		private void OkCommand()
		{
			var keyValuePair = new KeyValuePair<string, ModPackInfo>(Name, new ModPackInfo(SelectedVersionMetadata.GetVersion().Id));
			DialogHost.DialogHost.GetDialogSession("MainDialogHost")?.Close(keyValuePair);
		}
	}
}