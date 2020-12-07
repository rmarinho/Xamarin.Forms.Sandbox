using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sandbox
{
	public partial class MyShell
	{
		public MyShell()
		{
			InitializeComponent();
			this.FlyoutIsPresented = false;
			this.FlyoutBehavior = FlyoutBehavior.Disabled;
		}
	}
}