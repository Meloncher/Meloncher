using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using DynamicData;
using MeloncherCore.Version;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MeloncherAvalonia.ViewModels
{
	public class VersionsViewModel : ViewModelBase
	{
		private readonly VersionTools _versionTools;
		private readonly MVersionCollection _versionCollection;

		public VersionsViewModel(VersionTools versionTools, MVersionCollection? versionCollection, MVersionMetadata? selectedVersion)
		{
			SelectedVersion = selectedVersion;
			_versionTools = versionTools;
			OkCommand = ReactiveCommand.Create(OnOkCommandExecuted);
			versionCollection ??= _versionTools.GetVersionMetadatas();
			_versionCollection = versionCollection;
			UpdateList();
			PropertyChanged += (_, e) =>
			{
				if (e.PropertyName == "VersionType") UpdateList();
			};
		}

		public VersionsViewModel()
		{
		}

		[Reactive] public ObservableCollection<MVersionMetadata> Versions { get; set; }
		[Reactive] public MVersionMetadata? SelectedVersion { get; set; }
		[Reactive] public int VersionType { get; set; }

		private void UpdateList()
		{
			var lst = new List<MVersionMetadata>(_versionCollection);
			foreach (var metadata in _versionCollection)
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
				if (VersionType == 0)
				{
					var spl1 = metadata1.Name.Split(".");
					var spl2 = metadata2.Name.Split(".");
					if (spl1.Length >= 2 && spl2.Length >= 2)
					{
						int minor1;
						int minor2;
						int.TryParse(spl1[1], out minor1);
						int.TryParse(spl2[1], out minor2);
						if (minor1 != minor2) return minor2.CompareTo(minor1);
					}

					return string.Compare(metadata2.Name, metadata1.Name, StringComparison.Ordinal);
				}

				return _versionCollection.IndexOf(metadata1).CompareTo(_versionCollection.IndexOf(metadata2));
			}));
			Versions = new ObservableCollection<MVersionMetadata>(lst);
		}

		public ReactiveCommand<Unit, MVersionMetadata?> OkCommand { get; }

		private MVersionMetadata? OnOkCommandExecuted()
		{
			return SelectedVersion;
		}
	}
}