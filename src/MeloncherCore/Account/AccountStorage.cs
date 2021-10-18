using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using Newtonsoft.Json;

namespace MeloncherCore.Account
{
	public class AccountStorage : ICollection<MSession>, INotifyCollectionChanged
	{
		private List<MSession> sessionList = new();
		private readonly string storagePath;

		public AccountStorage(ExtMinecraftPath path)
		{
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
			LoadFile();
		}

		public IEnumerator<MSession> GetEnumerator()
		{
			foreach (var item in sessionList) yield return item;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (var item in sessionList) yield return item;
		}

		public void Add(MSession session)
		{
			sessionList.Add(session);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}

		public void Clear()
		{
			sessionList = new List<MSession>();
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
			var remove = sessionList.Remove(item);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
			return remove;
		}

		public int Count => sessionList.Count;
		public bool IsReadOnly => false;

		public event NotifyCollectionChangedEventHandler CollectionChanged;

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

		public void RemoveAt(int index)
		{
			sessionList.RemoveAt(index);
			CollectionChanged.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			SaveFile();
		}
	}
}