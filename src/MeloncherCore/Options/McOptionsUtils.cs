using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MeloncherCore.Options
{
	class McOptionsUtils
	{
		public static int GetDefaultScale()
		{
			double scale = 1;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				scale = Int32.Parse((string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\ThemeManager", "LastLoadedDPI", "96")) / 96;

			return (int)Math.Round(2 * scale + 0.1); ;
		}
		public static void SetDefaults(ExtGameOptionsFile options)
		{
			options.SetValue("gamma", 0.5);
			options.SetValue("lang", "ru_RU");
			options.SetValue("guiScale", GetDefaultScale());
			options.SetValue("autoJump", false);
			options.SetValue("fov", 0.5);
			options.SetValue("skipMultiplayerWarning", true);
			options.SetValue("soundCategory_master", 0.5);
			options.SetValue("soundCategory_music", 0.1);
		}
	}
}
