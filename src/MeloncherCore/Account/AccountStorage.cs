using CmlLib.Core.Auth;
using MeloncherCore.Launcher;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Account
{
	public class AccountStorage
	{
		ExtMinecraftPath path;
		string storagePath;
		List<MSession> array = new List<MSession> { };
		public AccountStorage(ExtMinecraftPath path)
		{
			this.path = path;
			storagePath = Path.Combine(path.RootPath, "meloncher_accounts.json");
		}

		public async Task ReadFile()
		{
			var jsonStr = "[]";
			if (File.Exists(storagePath)) jsonStr = await File.ReadAllTextAsync(storagePath);
			array = JsonConvert.DeserializeObject<List<MSession>>(jsonStr);

			//using (StreamReader file = File.OpenText(storagePath))
			//using (JsonTextReader reader = new JsonTextReader(file))
			//{
			//	array = (JArray)JToken.ReadFrom(reader);
			//}
		}
		public async Task SaveFile()
		{
			var jsonStr = JsonConvert.SerializeObject(array);
			await File.WriteAllTextAsync(storagePath, jsonStr);
		}
		public void Add(MSession session)
		{
			array.Add(session);
		}
		public void RemoveAt(int index)
		{
			array.RemoveAt(index);
		}
		public List<MSession> GetList()
		{
			return array;
		}
	}
}
