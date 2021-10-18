using System.Collections.ObjectModel;
using System.Reactive;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class VersionsViewModel : ViewModelBase
	{
		private IVersionLoader versionLoader;
		private MVersionCollection versionCollection;
		public VersionsViewModel(IVersionLoader versionLoader, MVersionCollection? versionCollection)
		{
			this.versionLoader = versionLoader;
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			this.versionCollection = versionCollection;
			if (this.versionCollection == null) this.versionCollection = versionLoader.GetVersionMetadatas();
			Versions = new ObservableCollection<MVersionMetadata>(versionCollection);
			SelectedVersion = versionCollection.LatestReleaseVersion;
		}
		
		[Reactive] public ObservableCollection<MVersionMetadata> Versions { get; set; }
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		
		public ReactiveCommand<Unit, MVersionMetadata> OkCommand { get; }

		private MVersionMetadata OnOkCommandExecuted()
		{
			return SelectedVersion;
		}
	}
}