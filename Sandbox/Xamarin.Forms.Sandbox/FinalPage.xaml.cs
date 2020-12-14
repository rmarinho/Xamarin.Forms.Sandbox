using System;
using static System.Diagnostics.Debug;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sandbox
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FinalPage : ContentPage
	{
		public FinalPage()
		{
			InitializeComponent();
		}

		void LifeCycleEffect_Loaded(object sender, EventArgs e) =>
			log.Text += $"\n {Message(sender, true)}";

		void LifeCycleEffect_Unloaded(object sender, EventArgs e) =>
			WriteLine($"\n {Message(sender, false)}");


		string Message(object sender, bool isLoaded)
		{
			if (sender is Label)
				return $"Label {IsLoadedText(isLoaded)}";
			if (sender is StackLayout)
				return $"StackLayout {IsLoadedText(isLoaded)}";

			return "Objeto não indentificado";

			
		}
		static string IsLoadedText(bool isLoaded)
		{
			switch (isLoaded)
			{
				case true:
					return "Carregada";
				case false:
					return "Descarregada";
				default:
					return "";
			}
		}
	}
}
