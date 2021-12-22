using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using CmlLib.Core;
using MeloncherCore.Launcher;
using MeloncherCore.Options;
using Newtonsoft.Json;

namespace MeloncherCore.ModPack
{
	public class ModPackStorage : INotifyPropertyChanged
	{
		private readonly Dictionary<string, ModPackInfo> _dictionary = new();
		private readonly ExtMinecraftPath _path;

		public ModPackStorage(ExtMinecraftPath path)
		{
			_path = path;
			Load();
		}

		public ObservableCollection<string> Keys { get; set; }
		public ObservableCollection<ModPackInfo> Values { get; set; }

		public int Count => _dictionary.Count;
		public event PropertyChangedEventHandler? PropertyChanged;

		public ModPackInfo Get(string key)
		{
			return _dictionary[key];
		}

		public void Rename(string key, string newKey)
		{
			if (key == newKey) return;
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (!Directory.Exists(modPackDir)) return;
			var newModPackDir = Path.Combine(_path.RootPath, "profiles", "custom", newKey);
			Directory.Move(modPackDir, newModPackDir);
			Load();
		}

		public void Edit(string key, ModPackInfo value)
		{
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (!Directory.Exists(modPackDir)) return;
			var jsonStr = JsonConvert.SerializeObject(value);
			File.WriteAllText(Path.Combine(modPackDir, "modpack.json"), jsonStr);
			Load();
		}

		public void Add(string key, ModPackInfo value)
		{
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (!Directory.Exists(modPackDir)) Directory.CreateDirectory(modPackDir);
			var jsonStr = JsonConvert.SerializeObject(value);
			File.WriteAllText(Path.Combine(modPackDir, "modpack.json"), jsonStr);
			var profileMcPath = _path.CloneWithProfile("custom", key);
			new McOptionsSync(profileMcPath, null).Load();
			Load();
		}

		public bool Remove(string key)
		{
			if (!ContainsKey(key)) return false;
			var modPackDir = Path.Combine(_path.RootPath, "profiles", "custom", key);
			if (Directory.Exists(modPackDir)) Directory.Delete(modPackDir, true);
			Load();
			return true;
		}

		public bool ContainsKey(string key)
		{
			return _dictionary.ContainsKey(key);
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

			Keys = new ObservableCollection<string>(_dictionary.Keys);
			Values = new ObservableCollection<ModPackInfo>(_dictionary.Values);
		}
	}
}