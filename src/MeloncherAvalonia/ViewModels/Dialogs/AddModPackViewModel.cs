using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CmlLib.Core.Version;
using MeloncherAvalonia.Views.Dialogs;
using MeloncherCore.Launcher;
using MeloncherCore.ModPack;
using MeloncherCore.Version;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels.Dialogs
{
	public class AddModPackViewModel : ViewModelBase
	{
		private readonly MVersionCollection? _versionCollection;
		private readonly VersionTools _versionTools;

		public AddModPackViewModel(VersionTools versionTools, MVersionCollection? versionCollection, KeyValuePair<string, ModPackInfo> modPackInfo)
		{
			_versionTools = versionTools;
			_versionCollection = versionCollection;
			Name = modPackInfo.Key;
			SelectedVersionMetadata = null;
			var metadatas = versionTools.GetVersionMetadatas();
			if (metadatas.Contains(modPackInfo.Value.VersionName))
			{
				SelectedVersionMetadata = metadatas.GetVersionMetadata(modPackInfo.Value.VersionName);
			}
		}

		public AddModPackViewModel(VersionTools versionTools, MVersionCollection? versionCollection)
		{
			_versionTools = versionTools;
			_versionCollection = versionCollection;
		}

		[Reactive] public MVersionMetadata? SelectedVersionMetadata { get; set; }
		[Reactive] public McClientType ClientType { get; set; } = McClientType.Vanilla;
		[Reactive] public string? Name { get; set; }

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
			var keyValuePair = new KeyValuePair<string, ModPackInfo>(Name, new ModPackInfo(SelectedVersionMetadata.GetVersion().Id, ClientType));
			DialogHost.DialogHost.GetDialogSession("MainDialogHost")?.Close(keyValuePair);
		}
	}
}