using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using MeloncherCore.Launcher;
using Newtonsoft.Json;

namespace MeloncherCore.Account
{
	public class AccountStorage : ICollection<MinecraftAccount>, INotifyCollectionChanged
	{
		private Dictionary<string, MinecraftAccount> sessionList = new();
		private readonly string storagePath;

		public AccountStorage(ExtMinecraftPath path)
		{
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
			LoadFile();
		}

		public IEnumerator<MinecraftAccount> GetEnumerator()
		{
			foreach (var item in sessionList) yield return item.Value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var item in sessionList) yield return item;
		}

		public void Add(MinecraftAccount session)
		{
			sessionList.Add(session.GameSession.Username, session);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public MinecraftAccount? Get(string key)
		{
			return sessionList.ContainsKey(key) ? sessionList[key] : null;
		}

		public void Clear()
		{
			sessionList = new Dictionary<string, MinecraftAccount>();
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public bool Contains(MinecraftAccount item)
		{
			return sessionList.ContainsKey(item.GameSession.Username);
		}

		public void CopyTo(MinecraftAccount[] array, int arrayIndex)
		{
			sessionList.Values.CopyTo(array, arrayIndex);
		}

		public bool Remove(MinecraftAccount item)
		{
			var remove = sessionList.Remove(item.GameSession.Username);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
			return remove;
		}

		public int Count => sessionList.Count;
		public bool IsReadOnly => false;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private void LoadFile()
		{
			var jsonStr = "{}";
			if (File.Exists(storagePath)) jsonStr = File.ReadAllText(storagePath);
			try
			{
				sessionList = JsonConvert.DeserializeObject<Dictionary<string, MinecraftAccount>>(jsonStr);
			}
			catch (Exception)
			{
				sessionList = new Dictionary<string, MinecraftAccount>();
			}
		}

		public void SaveFile()
		{
			var jsonStr = JsonConvert.SerializeObject(sessionList);
			_ = File.WriteAllTextAsync(storagePath, jsonStr);
		}

		public void RemoveAt(int index)
		{
			var key = sessionList.Keys.ElementAt(index);
			sessionList.Remove(key);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}
	}
}