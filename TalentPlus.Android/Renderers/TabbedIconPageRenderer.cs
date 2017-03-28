using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TalentPlus.Shared;
using TalentPlusAndroid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using System.ComponentModel;
using TalentPlus.Android;

[assembly: ExportRenderer (typeof(TabbedIconPage), typeof(TabbedIconPageRenderer))]
namespace TalentPlusAndroid
{
	public class TabbedIconPageRenderer : TabbedRenderer
	{
		private Dictionary<Int32, Int32> icons = new Dictionary<Int32, Int32> ();
		private Dictionary<Int32, string> iconsTitles = new Dictionary<Int32, string> ();
		private Dictionary<Int32, string> titles = new Dictionary<Int32, string> ();
		private Dictionary<Int32, bool> viewSet = new Dictionary<Int32, bool> ();

		private int _tabWidth = 0;
		private Android.App.Activity _activity;
		private TabbedIconPage _iconPage;
		//private int _selectedTabIndex = 0;

		//This flag is used in the case when the app is not completely closed, and the user return back.
		//private bool isFirstDesign = true;

		protected override void DispatchDraw (Android.Graphics.Canvas canvas)
		{
			if (_iconPage.NumberOfAppearing > 0 && viewSet != null) {
				_iconPage.NumberOfAppearing--;
				viewSet.Clear ();
			}

			setIconsAndText ();
            
			base.DispatchDraw (canvas);
		}
		//		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		//		{
		//			base.OnElementPropertyChanged (sender, e);
		//		}
		protected override void OnElementChanged (ElementChangedEventArgs<TabbedPage> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				this.Element.PropertyChanged += Element_PropertyChanged;
				_iconPage = e.NewElement as TabbedIconPage;


				if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat) {
					if (TalentPlusApp.JustResume) {
						_iconPage.FIXED_APPEARING_BASED_ON_PLATFORM = 1;
					} else {
						_iconPage.FIXED_APPEARING_BASED_ON_PLATFORM = 3;
					}
				}
				getIconsAndText ();

				MessagingCenter.Subscribe<string> (this, "ReloadTabBar", (page) => {

//					if (MainActivity.JustResume && _iconPage.FIXED_APPEARING_BASED_ON_PLATFORM == 1) {
//						MainActivity.JustResume = false;
//						if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat) {
//							_iconPage.FIXED_APPEARING_BASED_ON_PLATFORM = 3;
//						} else {
//							_iconPage.FIXED_APPEARING_BASED_ON_PLATFORM = 2;
//						}
//					}

					if (viewSet != null) {
						this.PostInvalidate ();
						viewSet.Clear ();
					}
				});
			}
		}

		private void Element_PropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			if (_iconPage != null && e.PropertyName == "CurrentPage"
			    && _activity.ActionBar.SelectedNavigationIndex >= 0
			    && _activity.ActionBar.SelectedNavigationIndex != _iconPage.CurrentTabIndex) {
				_iconPage.CurrentTabIndex = _activity.ActionBar.SelectedNavigationIndex;
				//TalentPlus.Shared.Helpers.Utility.RefreshActionBar ();
			}

			Console.WriteLine ("PropertyName: " + e.PropertyName);
		}

		private void getIconsAndText ()
		{
			if (Element == null)
				return;

			var displaymetrics = Resources.DisplayMetrics;
			int screenWidth = displaymetrics.WidthPixels;

			_activity = this.Context as Android.App.Activity;
            
			_tabWidth = screenWidth / Element.Children.Count - 1;

			int id = 0;
			foreach (var t in Element.Children) {
				if (icons.ContainsKey (id))
					continue;
				if (t.Icon != "" && t.Icon != null) {
					iconsTitles.Add (id, t.Icon);
					icons.Add (id, IconResourceIdFromString (t.Icon));
				}
				id++;
			}
			id = 0;
			foreach (var t in Element.Children) {
				if (titles.ContainsKey (id))
					continue;
				if (t.Title != "" && t.Title != null) {
					titles.Add (id, t.Title);
				}
				id++;
			}
		}

		private int IconResourceIdFromString (String name)
		{
			name = name.ToLower ()
                .Replace (".png", "")
                .Replace (".jpg", "")
                .Replace (".jpeg", "")
                .Replace (".gif", "")
                .Replace (".ico", "");
			Type type = typeof(Resource.Drawable);
			foreach (var p in type.GetFields()) {
				if (p.Name.ToLower () == name)
					return (int)p.GetValue (null);
			}
			return 0;
		}

		//private int TitleResourceIdFromString(String name)
		//{
		//	name = name.ToLower();
		//	Type type = typeof(Resource.Drawable);
		//	foreach (var p in type.GetFields())
		//	{
		//		if (p.Name.ToLower() == name)
		//			return (int)p.GetValue(null);
		//	}
		//	return 0;
		//}

		private void setIconsAndText ()
		{
			if (_activity != null && _activity.ActionBar != null) {
				for (int i = 0; i < _activity.ActionBar.TabCount; i++) {
					if (!icons.ContainsKey (i))
						continue;
					var tab = _activity.ActionBar.GetTabAt (i);

					if (!viewSet.ContainsKey (i)) {
						tab.SetCustomView (Resource.Layout.tab_layout);

						var tabView = tab.CustomView as LinearLayout;
						//title.SetMaxWidth(5);
						//image.SetMaxWidth(5);
						if (tabView != null) {
							tabView.Selected = (_iconPage.CurrentTabIndex == i);
							tabView.LayoutParameters = new LinearLayout.LayoutParams (_tabWidth, LinearLayout.LayoutParams.WrapContent);
							var image = tabView.FindViewById (Resource.Id.tabIconImageView) as ImageView;
							image.SetImageResource (IconResourceIdFromString (iconsTitles [i] + "_logo"));

							var title = tabView.FindViewById (Resource.Id.tabIconTextView) as TextView;
							title.SetText (titles [i], Android.Widget.TextView.BufferType.Normal);

							Android.Views.View tabContainerView = (Android.Views.View)tabView.Parent;
							if (tabContainerView != null) {
								//tabContainerView.SetBackgroundColor(Android.Graphics.Color.ParseColor("#f2f2f2"));
								tabContainerView.SetMinimumWidth (5);
								tabContainerView.SetMinimumHeight (100);
								tabView.SetMinimumHeight (100);
							}

							title.SetMaxWidth (_tabWidth);
							image.SetMaxWidth (_tabWidth);

							//SetTabsMaxWidth(screenWidth, activity.ActionBar.TabCount, tabPadding, title, image);
						}

						viewSet.Add (i, true);
					}
				}
			}
		}

		//		private void SetTabsMaxWidth(int screenWidth, int tabsCount, int tabPadding, TextView title, ImageView image)
		//		{
		//
		//			Console.WriteLine ("Max Width: " + maxWidth);
		//			if (tabPadding != -1)
		//			{
		//				title.SetMaxWidth(maxWidth);
		//				image.SetMaxWidth(maxWidth);
		//			}
		//		}


	}
}