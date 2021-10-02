using System;

namespace MeloncherCore.Launcher.Events
{
    public delegate void McDownloadFileChangedHandler(McDownloadFileChangedEventArgs e);

    public class McDownloadFileChangedEventArgs : EventArgs
    {
        public McDownloadFileChangedEventArgs(string type)
        {
            Type = type;
        }
        public string Type { get; private set; }
    }
}
