using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using DynamicData;
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
			UpdateList();
			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "VersionType") UpdateList();
			};
			SelectedVersion = versionCollection.LatestReleaseVersion;
		}
		
		[Reactive] public ObservableCollection<MVersionMetadata> Versions { get; set; }
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		[Reactive] public int VersionType { get; set; } = 0;

		private void UpdateList()
		{
			var lst = new List<MVersionMetadata>(versionCollection);
			foreach (var metadata in versionCollection)
			{
				if (metadata.Name.ToLower().Contains("optifine")) lst.Remove(metadata);
				var mtdType = metadata.MType;
				if (mtdType == MVersionType.Custom) mtdType = metadata.GetVersion().Type;
				if (VersionType == 0 && mtdType != MVersionType.Release) lst.Remove(metadata);
				if (VersionType == 1 && mtdType != MVersionType.Snapshot) lst.Remove(metadata);
				if (VersionType == 2 && mtdType != MVersionType.OldAlpha && mtdType != MVersionType.OldBeta) lst.Remove(metadata);
			}
			lst.Sort(Comparer<MVersionMetadata>.Create((metadata1, metadata2) =>
			{
				// var mtd1Type = metadata1.MType;
				// var mtd2Type = metadata2.MType;
				// if (mtd1Type == MVersionType.Custom) mtd1Type = metadata1.GetVersion().Type;
				// if (mtd2Type == MVersionType.Custom) mtd2Type = metadata2.GetVersion().Type;
				//
				// if (mtd1Type != mtd2Type) return mtd2Type.CompareTo(mtd1Type);

				// if (mtd1Type == MVersionType.Release)
				if (VersionType == 0)
				{
					var spl1 = metadata1.Name.Split(".");
					var spl2 = metadata2.Name.Split(".");
					if (spl1.Length >= 2 && spl2.Length >= 2)
					{
						int minor1 = int.Parse(spl1[1]);
						int minor2 = int.Parse(spl2[1]);
						if (minor1 != minor2) return minor2.CompareTo(minor1);
					}
					return metadata2.Name.CompareTo(metadata1.Name);
				}
				return versionCollection.IndexOf(metadata1).CompareTo(versionCollection.IndexOf(metadata2));
			}));
			Versions = new ObservableCollection<MVersionMetadata>(lst);
		}
		
		public ReactiveCommand<Unit, MVersionMetadata> OkCommand { get; }

		private MVersionMetadata OnOkCommandExecuted()
		{
			return SelectedVersion;
		}
	}
}