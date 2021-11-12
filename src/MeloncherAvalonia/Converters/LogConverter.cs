using System;
using Avalonia.Data.Converters;
using MeloncherCore.Logs;

namespace MeloncherAvalonia.Converters
{
	public class LogConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is McLogLine mcLogLine)
			{
				return mcLogLine.Time + " " + mcLogLine.Text;
			}

			return null;
		}


		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}