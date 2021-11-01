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
	public class AccountStorage : ICollection<McAccount>, INotifyCollectionChanged
	{
		private readonly string storagePath;
		private Dictionary<string, McAccount> sessionList = new();

		public AccountStorage(ExtMinecraftPath path)
		{
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
			LoadFile();
		}

		public IEnumerator<McAccount> GetEnumerator()
		{
			foreach (var item in sessionList) yield return item.Value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(McAccount session)
		{
			sessionList.Add(session.GameSession.Username, session);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public void Clear()
		{
			sessionList = new Dictionary<string, McAccount>();
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public bool Contains(McAccount item)
		{
			return sessionList.ContainsKey(item.GameSession.Username);
		}

		public void CopyTo(McAccount[] array, int arrayIndex)
		{
			sessionList.Values.CopyTo(array, arrayIndex);
		}

		public bool Remove(McAccount item)
		{
			var remove = sessionList.Remove(item.GameSession.Username);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
			return remove;
		}

		public int Count => sessionList.Count;
		public bool IsReadOnly => false;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public McAccount? Get(string key)
		{
			return sessionList.ContainsKey(key) ? sessionList[key] : null;
		}

		private void LoadFile()
		{
			var jsonStr = "{}";
			if (File.Exists(storagePath)) jsonStr = File.ReadAllText(storagePath);
			try
			{
				sessionList = JsonConvert.DeserializeObject<Dictionary<string, McAccount>>(jsonStr);
			}
			catch (Exception)
			{
				sessionList = new Dictionary<string, McAccount>();
			}

			foreach (var keyValuePair in sessionList)
				if (keyValuePair.Value.GameSession == null)
					sessionList.Remove(keyValuePair.Key);
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