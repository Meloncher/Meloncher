using CmlLib.Core.Auth;
using MeloncherCore.Account;
using MeloncherCore.Launcher;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MeloncherWPF.ViewModels
{
	class AccountsViewModel : INotifyPropertyChanged
	{

		public AccountsViewModel()
		{
			Accounts = new ObservableCollection<MSession>();
			var dispatcher = Dispatcher.CurrentDispatcher;
			new Task(async () =>
			{
				AccountStorage accountStorage = new AccountStorage(new ExtMinecraftPath("D:\\MeloncherNetTest"));
				await accountStorage.ReadFile();
				var list = accountStorage.GetList();
				list.ForEach((acc) =>
				{
					Action del = () => Accounts.Add(acc);
					dispatcher.Invoke(del);
				});
			}).Start();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<MSession> Accounts { get; set; }

	}
}
