using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MeloncherCore.Launcher
{
	internal class WindowTweaks
	{
		private const int SW_SHOWMAXIMIZED = 3;
		private const int GWL_STYLE = -16;
		private const int WS_BORDER = 0x00800000; //window with border
		private const int WS_DLGFRAME = 0x00400000; //window with double border but no title
		private const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar

		private readonly Process _process;

		public WindowTweaks(Process process)
		{
			this._process = process;
		}

		private async Task<IntPtr> GetHWnd()
		{
			for (var i = 0; i < 20; i++)
			{
				if (_process.MainWindowHandle.ToInt32() != 0) return _process.MainWindowHandle;
				await Task.Delay(1000);
			}

			return _process.MainWindowHandle;
		}

		public async Task Maximize()
		{
			var hWnd = await GetHWnd().ConfigureAwait(false);
			ShowWindow(hWnd, SW_SHOWMAXIMIZED);
		}

		public async Task Borderless()
		{
			var hWnd = await GetHWnd().ConfigureAwait(false);
			var style = GetWindowLong(hWnd, GWL_STYLE);
			SetWindowLong(hWnd, GWL_STYLE, style & ~WS_CAPTION);
			ShowWindow(hWnd, SW_SHOWMAXIMIZED);
		}

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		[DllImport("user32.dll")]
		private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll")]
		private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll")]
		private static extern bool DrawMenuBar(IntPtr hWnd);
	}
}