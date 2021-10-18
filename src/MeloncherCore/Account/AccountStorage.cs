using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using Newtonsoft.Json;

namespace MeloncherCore.Account
{
	public class AccountStorage : ICollection<MSession>, INotifyCollectionChanged
	{
		private Dictionary<string, MSession> sessionList = new();
		private readonly string storagePath;

		public AccountStorage(ExtMinecraftPath path)
		{
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
			LoadFile();
		}

		public IEnumerator<MSession> GetEnumerator()
		{
			foreach (var item in sessionList) yield return item.Value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var item in sessionList) yield return item;
		}

		public void Add(MSession session)
		{
			sessionList.Add(session.Username, session);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public MSession Get(string key)
		{
			return sessionList[key];
		}

		public void Clear()
		{
			sessionList = new Dictionary<string, MSession>();
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public bool Contains(MSession item)
		{
			return sessionList.ContainsKey(item.Username);
		}

		public void CopyTo(MSession[] array, int arrayIndex)
		{
			sessionList.Values.CopyTo(array, arrayIndex);
		}

		public bool Remove(MSession item)
		{
			var remove = sessionList.Remove(item.Username);
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
				sessionList = JsonConvert.DeserializeObject<Dictionary<string, MSession>>(jsonStr);
			}
			catch (Exception _)
			{
				sessionList = new Dictionary<string, MSession>();
			}
			
		}

		private void SaveFile()
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