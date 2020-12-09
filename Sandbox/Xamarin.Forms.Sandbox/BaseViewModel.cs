using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using static System.String;

namespace Xamarin.Forms.Sandbox
{
	sealed class ShellViewModel : BaseViewModel
	{
		bool isLogged;

		public bool IsLogged
		{
			get => isLogged;
			set
			{
				if (SetProperty(ref isLogged, value))
					OnPropertyChanged(nameof(IsNotLogged));
			}
		}

		bool isAdm;

		public bool IsAdm
		{
			get => isAdm;
			set => SetProperty(ref isAdm, value);
		}

		public bool IsNotLogged => !IsLogged;

		public Command LogInCommand { get; }

		public AsyncCommand LogOutCommand { get; }

		public ShellViewModel()
		{
			LogInCommand = new Command(LogInCommandExecute);
			LogOutCommand = new AsyncCommand(LogOutCommandExecute);
		}

		Task LogOutCommandExecute()
		{
			CurrentShell.FlyoutBehavior = FlyoutBehavior.Disabled;
			CurrentShell.FlyoutIsPresented = false;
			IsLogged = false;
			return CurrentShell.GoToAsync("///login");
		}

		void LogInCommandExecute()
		{
			IsLogged = true;
			CurrentShell.FlyoutBehavior = FlyoutBehavior.Flyout;
		}
	}

	sealed class MainViewModel : BaseViewModel
	{
		bool isFromCommand;

		public MainViewModel()
		{
			Navigation.InterceptNavigation[Key] = (async () =>
			{
				if (!isFromCommand)
					return true;

				var result = await Shell.Current.DisplayAlert("Tem ctz?", "Quer sair dessa tela?", "Sim", "Não");
				isFromCommand = false;
				return result;
			});

			NavigateCommand = new AsyncCommand(NavigateCommandExecute);
		}

		Task NavigateCommandExecute()
		{
			isFromCommand = true;
			return Navigation.GoToAsync(nameof(StartViewModel));
		}
	}

	sealed class StartViewModel : BaseViewModel
	{

	}

	sealed class SecretViewModel : BaseViewModel
	{
		string name;

		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		string password;

		public string Password
		{
			get => password;
			set => SetProperty(ref password, value);
		}

		public SecretViewModel()
		{
			NavigateCommand = new AsyncCommand(NavigateCommandExecute);
			Navigation.InterceptNavigation[Key] = async () =>
			{
				if (IsNullOrEmpty(Name) || IsNullOrEmpty(Password))
				{
					await Shell.Current.DisplayAlert("Erro", "Tem que preencher tudo!", "Ok");
					return false;
				}
				return true;
			};
		}

		Task NavigateCommandExecute() => Navigation.GoToAsync(nameof(FinalViewModel), new Dictionary<string, object>
		{
			{nameof(Name), Name },
			{nameof(Password), Password }
		});
	}

	[QueryProperty(nameof(Name), nameof(Name))]
	[QueryProperty(nameof(Pass), "Password")]
	sealed class FinalViewModel : BaseViewModel
	{
		string name;
		public string Name
		{
			get => name;
			set => SetProperty(ref name, value);
		}

		string pass;
		public string Pass
		{
			get => pass;
			set => SetProperty(ref pass, value);
		}
	}

	sealed class FlyoutDimensionsViewModel : BaseViewModel
	{
		public Command HeightCommand { get; }
		public Command WidthCommand { get; }
		public Command DefaultCommand { get; }

		readonly (double height, double width) defaultValues;

		public FlyoutDimensionsViewModel()
		{
			HeightCommand = new Command<string>(HeightCommandExecute);
			WidthCommand = new Command<string>(WidthCommandExecute);
			defaultValues = (Shell.GetFlyoutHeight(CurrentShell), Shell.GetFlyoutWidth(CurrentShell));
			DefaultCommand = new Command(DefaultCommandExecute);
		}

		void DefaultCommandExecute()
		{
			Shell.SetFlyoutHeight(CurrentShell, defaultValues.height);
			Shell.SetFlyoutWidth(CurrentShell, defaultValues.width);
		}

		void WidthCommandExecute(string w)
		{
			if (double.TryParse(w, out var width))
				Shell.SetFlyoutWidth(CurrentShell, width);
			else
				CurrentShell.DisplayAlert("PANIC!", "O valor precisar ser \"double\"! ", "Ok");
		}

		void HeightCommandExecute(string h)
		{
			if (double.TryParse(h, out var height))
				Shell.SetFlyoutHeight(CurrentShell, height);
			else
				CurrentShell.DisplayAlert("PANIC!", "O valor precisar ser \"double\"! ", "Ok");
		}
	}

	public abstract class BaseViewModel : INotifyPropertyChanged
	{

		protected Shell CurrentShell => Shell.Current;

		public string Key { get; } = Guid.NewGuid().ToString();

		protected NavigationService Navigation => NavigationService.Current;

		bool isBusy = false;
		public bool IsBusy
		{
			get => isBusy;
			set => SetProperty(ref isBusy, value);
		}

		string title = string.Empty;
		public string Title
		{
			get => title;
			set => SetProperty(ref title, value);
		}

		public AsyncCommand NavigateCommand { get; protected set; }

		protected bool SetProperty<T>(ref T backingStore, T value,
			[CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(backingStore, value))
				return false;

			backingStore = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		public virtual Task InitAsync(object args = null) => Task.CompletedTask;

		public virtual Task BackAsync(object args = null) => Task.CompletedTask;


		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			var changed = PropertyChanged;

			changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
