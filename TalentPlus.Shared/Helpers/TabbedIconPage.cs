using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using TalentPlus.Shared.Helpers;

namespace TalentPlus.Shared
{
	public class TabbedIconPage : TabbedPage
	{
		public int FIXED_APPEARING_BASED_ON_PLATFORM = 2;

		private bool isFirstLoad = true;
		 
		public int NumberOfAppearing { get; set; }

		//public event EventHandler<int> TabbedIconPageHandler;

		public static readonly BindableProperty CurrentTabIndexProperty =
			BindableProperty.Create("CurrentTabIndex", typeof(int), typeof(TabbedIconPage), 0);
		
		public int CurrentTabIndex
		{
			get { return (int)GetValue(CurrentTabIndexProperty); }
			set { SetValue(CurrentTabIndexProperty, value); }
		}

		public TabbedIconPage () : base ()
		{
			NumberOfAppearing = 0;
		}

		public void SwitchToOverview ()
		{
			if (Children.Count != 0) {
				CurrentTabIndex = 0;
				this.CurrentPage = Children [CurrentTabIndex];
				this.Title = this.CurrentPage.Title;
			}
		}

		public void SwitchToSettings ()
		{
			if (Children.Count != 0) {
				CurrentTabIndex = Children.Count - 1;
				this.CurrentPage = Children [CurrentTabIndex];
			}
		}

		public void SwitchToActivities()
		{
			if (Children.Count != 0 && Children.Count > 2)
			{
				CurrentTabIndex = 1;
				this.CurrentPage = Children[CurrentTabIndex];
			}
		}

#if __IOS__
		public void SwitchToThemes()
		{
			if (Children.Count != 0 && Children.Count > 3)
			{
				this.CurrentPage = Children[2];
			}
		}
#endif

		protected override void OnCurrentPageChanged ()
		{
			base.OnCurrentPageChanged ();
			#if __IOS__
			if (this.CurrentPage.Title != "Activities") {
				this.Title = this.CurrentPage.Title;
			} else {
				this.Title = "Pick an Activity";
			}
            #endif
			
            #if __ANDROID__

			this.Title = this.CurrentPage.Title;
			Utility.SetScreenTitle (this.CurrentPage.Title);

			#endif


		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			Utility.ForceHideBackButton();

			if (!isFirstLoad) {
				NumberOfAppearing = FIXED_APPEARING_BASED_ON_PLATFORM;
				#if __ANDROID__
				if(TalentPlusApp.JustResume)
				{
					SwitchToOverview ();
				}
				#endif
				//SelectTabbedHandler ();
			} else {
				#if __IOS__
				this.Title = this.CurrentPage.Title;
				#endif
			}
			isFirstLoad = false;
		}

//		protected void SelectTabbedHandler ()
//		{
//			if (TabbedIconPageHandler != null) {
//				TabbedIconPageHandler (this, 0);
//			}
//		}
	}
}
