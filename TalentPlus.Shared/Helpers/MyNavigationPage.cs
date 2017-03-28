using System;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class MyNavigationPage : NavigationPage
	{
		~MyNavigationPage()
		{
			MessagingCenter.Unsubscribe<string> (this, "ReloadActionBar");
				
		}

		public MyNavigationPage () : base ()
		{
			LoadTheme ();
			SubcribeActionBar ();
		}

		public MyNavigationPage (Page root) : base (root)
		{
			LoadTheme ();
			SubcribeActionBar ();
		}

		protected override void OnAppearing ()
		{
			LoadTheme ();

			base.OnAppearing ();
		}

		private void SubcribeActionBar()
		{
			MessagingCenter.Subscribe<string> (this, "ReloadActionBar", (string title) => {
				this.BarBackgroundColor = Helpers.Color.Primary.ToFormsColor ();

			});
		}

		private void LoadTheme ()
		{
			//this.Icon = "header_menu";
			this.BarBackgroundColor = Helpers.Color.Primary.ToFormsColor ();
			this.BarTextColor = Xamarin.Forms.Color.White;

//			if (ToolbarItems.Count == 0) {
//				ToolbarItems.Add (new ToolbarItem ("info", "info", delegate {
//
//				}));
//			}
		}
	}
}