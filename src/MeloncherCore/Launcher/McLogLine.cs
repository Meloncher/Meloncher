using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeloncherCore.Launcher
{
	public class McLogLine
	{
		public TimeSpan Time { get; set; }
		public string Thread { get; set; }
		public string Type { get; set; }
		public string Text { get; set; }
		public McLogLine(TimeSpan time, string thread, string type, string text)
		{
			Time = time;
			Thread = thread;
			Type = type;
			Text = text;
		}

		public static McLogLine Parse(string line)
		{
			TimeSpan time = new TimeSpan(0, 0, 0);
			string thread = null;
			string type = null;
			var var0 = line.Split("] [", 2);
			if (var0.Length == 2)
			{
				line = var0[1];
				var var1 = var0[0].Replace("[", "").Replace("]", "").Split(":");
				if (var1.Length == 3)
				{
					time = new TimeSpan(int.Parse(var1[0]), int.Parse(var1[1]), int.Parse(var1[2]));
				}
			}
			var var2 = line.Split("]: ", 2);
			if (var2.Length == 2)
			{
				line = var2[1];
				var var3 = var2[0].Replace("[", "").Replace("]", "").Split("/");
				if (var3.Length == 2)
				{
					thread = var3[0];
					type = var3[1];
				}
			}

			return new McLogLine(time, thread, type, line);
		}
	}
}
