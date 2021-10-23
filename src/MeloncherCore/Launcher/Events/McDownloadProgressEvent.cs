using System;

namespace MeloncherCore.Launcher.Events
{
	public delegate void McDownloadProgressEventHandler(McDownloadProgressEventArgs e);

	public class McDownloadProgressEventArgs : EventArgs
	{
		public McDownloadProgressEventArgs(string type, int progressPercentage, bool isChecking)
		{
			Type = type;
			ProgressPercentage = progressPercentage;
			IsChecking = isChecking;
		}

		public string Type { get; }
		public int ProgressPercentage { get; }
		public bool IsChecking { get; }
	}
}