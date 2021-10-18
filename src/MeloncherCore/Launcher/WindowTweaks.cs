using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MeloncherCore.Launcher
{
	internal class WindowTweaks
	{
		private const int SW_SHOWMAXIMIZED = 3;

		//assorted constants needed
		public static int GWL_STYLE = -16;
		public static int WS_CHILD = 0x40000000; //child window
		public static int WS_BORDER = 0x00800000; //window with border
		public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
		public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar

		private readonly Process process;

		public WindowTweaks(Process process)
		{
			this.process = process;
		}

		public async Task<IntPtr> GetHWnd()
		{
			for (var i = 0; i < 20; i++)
			{
				if (process.MainWindowHandle.ToInt32() != 0) return process.MainWindowHandle;
				await Task.Delay(1000);
			}

			return process.MainWindowHandle;
		}

		public async Task Maximize()
		{
			var hWnd = await GetHWnd();
			ShowWindow(hWnd, SW_SHOWMAXIMIZED);
		}

		public async Task Borderless()
		{
			var hWnd = await GetHWnd();
			var style = GetWindowLong(hWnd, GWL_STYLE);
			SetWindowLong(hWnd, GWL_STYLE, style & ~WS_CAPTION);
			ShowWindow(hWnd, SW_SHOWMAXIMIZED);
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern bool DrawMenuBar(IntPtr hWnd);
	}
}