﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CmlLib.Utils;

namespace MeloncherCore.Options
{
	internal class ExtGameOptionsFile : GameOptionsFile
	{
		// Overrides
		public ExtGameOptionsFile(Dictionary<string, string> options, string path) : base(options, path)
		{
		}

		public ExtGameOptionsFile(Dictionary<string, string> options) : base(options)
		{
		}

		public ExtGameOptionsFile()
		{
		}

		public new static ExtGameOptionsFile ReadFile(string filepath, Encoding encoding)
		{
			var fileinfo = new FileInfo(filepath);
			if (fileinfo.Length > MaxOptionFileLength)
				throw new IOException("File is too big");

			var optionDict = new Dictionary<string, string?>();

			using (var fs = fileinfo.OpenRead())
			using (var reader = new StreamReader(fs, encoding))
			{
				string? line;
				while ((line = reader.ReadLine()) != null)
					if (!line.Contains(":"))
					{
						optionDict[line] = null;
					}
					else
					{
						var keyvalue = FromKeyValueString(line);
						optionDict[keyvalue.Key] = keyvalue.Value;
					}
			}

			return new ExtGameOptionsFile(optionDict, filepath);
		}

		public new void Save(string path, Encoding encoding)
		{
			if (File.Exists(path)) File.Delete(path);
			var dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
			using (var fs = File.OpenWrite(path))
			using (var writer = new StreamWriter(fs, encoding))
			{
				foreach (var keyvalue in this)
					if (keyvalue.Value != null)
					{
						var line = keyvalue.Key + ":" + keyvalue.Value;
						writer.WriteLine(line);
					}
			}
		}

		// New Funcs
		public void Merge(GameOptionsFile gof, string[] noSyncKeys = null)
		{
			foreach (var pair in gof)
			{
				if (noSyncKeys != null && noSyncKeys.Contains(pair.Key)) continue;
				SetRawValue(pair.Key, pair.Value);
			}
		}

		public void Downgrade()
		{
			SetRawValue("version", null);
			if (ContainsKey("lang"))
			{
				var lang = GetValueAsString("lang");
				var lang_split = lang.Split("_");
				if (lang_split.Length == 2)
				{
					lang = lang_split[0] + "_" + lang_split[1].ToUpper();
					SetValue("lang", lang);
				}
			}

			foreach (var keyPair in this)
				if (keyPair.Key.StartsWith("key_") && McKeycodes.Contains(keyPair.Value))
					SetValue(keyPair.Key, McKeycodes.Get(keyPair.Value));
		}
	}
}