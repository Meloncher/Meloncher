using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MeloncherCore.Launcher;
using Newtonsoft.Json;

namespace MeloncherCore.ModPack
{
	public class ModPackStorage : IDictionary<string, ModPackInfo>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		private readonly ExtMinecraftPath _path;
		private readonly Dictionary<string, ModPackInfo> _dictionary = new();

		public ModPackStorage(ExtMinecraftPath path)
		{
			_path = path;
			Load();
			ObservableKeys = new ObservableCollection<string>(_dictionary.Keys);
			ObservableValues = new ObservableCollection<ModPackInfo>(_dictionary.Values);
			CollectionChanged += (sender, args) =>
			{
				ObservableKeys = new ObservableCollection<string>(_dictionary.Keys);
				ObservableValues = new ObservableCollection<ModPackInfo>(_dictionary.Values);
			};
		}

		private void Load()
		{
			_dictionary.Clear();
			var profilesDir = Path.Combine(_path.RootPath, "profiles", "custom");
			if (!Directory.Exists(profilesDir)) return;
			var directories = Directory.GetDirectories(profilesDir);
			foreach (var directory in directories)
			{
				var jsonPath = Path.Combine(directory, "modpack.json");
				if (!File.Exists(jsonPath)) continue;
				var jsonStr = File.ReadAllText(jsonPath);
				var modPackInfo = JsonConvert.DeserializeObject<ModPackInfo>(jsonStr);
				var dirName = new DirectoryInfo(directory).Name;
				_dictionary.Add(dirName, modPackInfo);
			}
		}

		public void Add(string key, ModPackInfo value)
		{
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (!Directory.Exists(modPackDir)) Directory.CreateDirectory(modPackDir);
			var jsonStr = JsonConvert.SerializeObject(value);
			File.WriteAllText(Path.Combine(modPackDir, "modpack.json"), jsonStr);
			Load();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		
		public bool Remove(string key)
		{
			if (!ContainsKey(key)) return false;
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (Directory.Exists(modPackDir)) Directory.Delete(modPackDir, true);
			Load();
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			return true;
		}
		public IEnumerator<KeyValuePair<string, ModPackInfo>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<string, ModPackInfo> item)
		{
			var (key, value) = item;
			Add(key, value);
		}

		public void Clear()
		{
			// _dictionary.Clear();
		}

		public bool Contains(KeyValuePair<string, ModPackInfo> item)
		{
			return _dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<string, ModPackInfo>[] array, int arrayIndex)
		{
			
		}

		public bool Remove(KeyValuePair<string, ModPackInfo> item)
		{
			return Remove(item.Key);
		}

		public int Count => _dictionary.Count;
		public bool IsReadOnly => false;

		public bool ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
		}

		public bool TryGetValue(string key, out ModPackInfo value)
		{
			return _dictionary.TryGetValue(key, out value);
		}

		public ModPackInfo this[string key]
		{
			get => _dictionary[key];
			set => _dictionary[key] = value;
		}

		public ObservableCollection<string> ObservableKeys { get; set; }
		public ObservableCollection<ModPackInfo> ObservableValues { get; set; }
		public ICollection<string> Keys => _dictionary.Keys;
		public ICollection<ModPackInfo> Values => _dictionary.Values;
		public event NotifyCollectionChangedEventHandler? CollectionChanged;
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}