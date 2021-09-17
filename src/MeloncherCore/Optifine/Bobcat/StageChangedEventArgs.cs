using System;

namespace MeloncherCore.Optifine.Bobcat
{
	public class StageChangedEventArgs : EventArgs
	{
		public string CurrentStage { get; set; }
		public double Progress { get; set; }
	}
}