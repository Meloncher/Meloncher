using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace MeloncherCore.Account
{
	public class AccountStorage : ICollection<MSession>, INotifyCollectionChanged
	{
		List<MSession> sessionList = new();
		string storagePath;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public AccountStorage(ExtMinecraftPath path)
		{
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
			LoadFile();
		}

		private void LoadFile()
		{
			var jsonStr = "[]";
			if (File.Exists(storagePath)) jsonStr = File.ReadAllText(storagePath);
			sessionList = JsonConvert.DeserializeObject<List<MSession>>(jsonStr);
		}
		private void SaveFile()
		{
			var jsonStr = JsonConvert.SerializeObject(sessionList);
			_ = File.WriteAllTextAsync(storagePath, jsonStr);
		}

		public IEnumerator<MSession> GetEnumerator()
		{
			foreach (var item in sessionList)
			{
				yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var item in sessionList)
			{
				yield return item;
			}
		}
		public void Add(MSession session)
		{
			sessionList.Add(session);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}
		public void RemoveAt(int index)
		{
			sessionList.RemoveAt(index);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}
		public void Clear()
		{
			sessionList = new();
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public bool Contains(MSession item)
		{
			return sessionList.Contains(item);
		}

		public void CopyTo(MSession[] array, int arrayIndex)
		{
			sessionList.CopyTo(array, arrayIndex);
		}

		public bool Remove(MSession item)
		{
			bool remove = sessionList.Remove(item);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
			return remove;
		}
		public int Count => sessionList.Count;
		public bool IsReadOnly => false;
	}
}
