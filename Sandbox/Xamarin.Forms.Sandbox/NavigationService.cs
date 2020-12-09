using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Forms.Sandbox
{
	public class NavigationService
	{
		bool isNavigateAllowed;

		static readonly Lazy<NavigationService> navigation =
			new Lazy<NavigationService>(() => new NavigationService(), false);

		public static NavigationService Current => navigation.Value;

		Shell Shell => Shell.Current;

		Page CurrentPage => Shell.CurrentPage;

		public Dictionary<string, Func<Task<bool>>> InterceptNavigation { get; } = new Dictionary<string, Func<Task<bool>>>();

		NavigationService()
		{
			RegisterRoutes();
			Shell.Navigated += OnShellNavigated;
			Shell.Navigating += OnShellNavigating;

			static void RegisterRoutes()
			{
				Routing.RegisterRoute(nameof(MainViewModel), typeof(Page1));
				Routing.RegisterRoute(nameof(StartViewModel), typeof(Page2));
				Routing.RegisterRoute(nameof(SecretViewModel), typeof(SecretPage));
				Routing.RegisterRoute(nameof(FinalViewModel), typeof(FinalPage));
			}
		}

		async void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
		{
			var key = (CurrentPage.BindingContext as BaseViewModel).Key;
			InterceptNavigation.TryGetValue(key, out var task);
			if (task is { }) // task != null
			{
				var deferral = e.GetDeferral();
				var result = await task();

				if (!result)
					e.Cancel();

				deferral.Complete();
			}

			isNavigateAllowed = !e.Cancelled;
		}

		void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
		{
			//Preferences.Set("LastKnownUrl", e.Current.Location.OriginalString);
		}

		public async Task GoToAsync(string url, object args = null)
		{
			await Shell.GoToAsync(url);
			if (url == ".." || url.Contains("\\") || url.Contains("/"))
			{
				await (CurrentPage.BindingContext as BaseViewModel).BackAsync(args).ConfigureAwait(false);
				return;
			}

			await CreateVMAndInit(url, args).ConfigureAwait(false);
		}

		public async Task GoToAsync(string url, IDictionary<string, object> @params)
		{
			await Shell.GoToAsync(url, @params);
			await CreateVMAndInit(url).ConfigureAwait(false);
		}

		public async Task GoToAsync(ShellNavigationState state, object args = null)
		{
			await Shell.GoToAsync(state);
			await CreateVMAndInit(state.Location.OriginalString.Split('/').Last(), args).ConfigureAwait(false);
		}

		Task CreateVMAndInit(string url, object args = null)
		{
			if (!isNavigateAllowed)
				return Task.CompletedTask;

			var vm = CurrentPage.BindingContext as BaseViewModel;

			if (vm is null || vm is ShellViewModel)
			{
				vm = CreateViewModel(url);
				CurrentPage.BindingContext = vm;
			}

			return vm.InitAsync(args);
		}

		BaseViewModel CreateViewModel(string url)
		{
			var typeName = $"Xamarin.Forms.Sandbox.{url}";
			var viewModel = (BaseViewModel)Activator.CreateInstance(Type.GetType(typeName));
			return viewModel;
		}
	}
}
