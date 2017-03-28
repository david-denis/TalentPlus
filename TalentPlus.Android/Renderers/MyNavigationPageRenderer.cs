using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using TalentPlus.Android;
using Android.App;
using Android.Graphics.Drawables;
using Android.Widget;
using Android.Support.V7.App;

[assembly: ExportRenderer (typeof(TalentPlus.Shared.MyNavigationPage), typeof(MyNavigationPageRenderer))]
namespace TalentPlus.Android
{
	public class MyNavigationPageRenderer : NavigationRenderer
	{
		private TextView _txtTitle = null;
		private ImageButton _btnBack = null;

		~MyNavigationPageRenderer ()
		{
			UnsubscribeMessages ();
		}

		public MyNavigationPageRenderer () : base ()
		{
			
		}

		protected override System.Threading.Tasks.Task<bool> OnPopViewAsync (Page page, bool animated)
		{
			if (_btnBack != null && ChildCount <= 2 || page is TalentPlus.Shared.WhenView || page is TalentPlus.Shared.FeedBackView) {
				_btnBack.Visibility = global::Android.Views.ViewStates.Gone;
			}

			return base.OnPopViewAsync (page, animated);
		}

		protected override System.Threading.Tasks.Task<bool> OnPushAsync (Page view, bool animated)
		{
			if (ChildCount >= 1) {
				NavigationPage.SetHasBackButton (view, false);
				if (_btnBack != null) {
					_btnBack.Visibility = global::Android.Views.ViewStates.Visible;
				}
			}

			return base.OnPushAsync (view, animated);
		}

		protected override void OnElementChanged (ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged (e);

			if (e.NewElement != null) {
				var activity = (Activity)this.Context;
				var actionBar = activity.ActionBar;

				if (actionBar != null) {
					actionBar.NavigationMode = ActionBarNavigationMode.Standard;
					actionBar.SetDisplayShowHomeEnabled (false);
					actionBar.SetDisplayShowTitleEnabled (false);
					actionBar.SetDisplayHomeAsUpEnabled (true);

					//actionBar.SetHomeAsUpIndicator (new ColorDrawable (global::Android.Graphics.Color.Transparent));

					//actionBar.SetDisplayUseLogoEnabled (false);
					//actionBar.SetDisplayShowTitleEnabled (false);

					var bar = activity.LayoutInflater.Inflate (Resource.Layout.custom_actionbar, null);
					actionBar.SetCustomView (bar, new global::Android.App.ActionBar.LayoutParams (LayoutParams.MatchParent, LayoutParams.MatchParent));

					actionBar.SetDisplayShowCustomEnabled (true);

					_txtTitle = actionBar.CustomView.FindViewById (Resource.Id.txtTitle) as TextView;
					_btnBack = actionBar.CustomView.FindViewById (Resource.Id.ibtnMenu) as ImageButton;
					_btnBack.Click += _btnBack_Click;

					SubscribeMessages ();
				}
			}
		}

		void SubscribeMessages ()
		{
			MessagingCenter.Subscribe<string> (this, "ReloadActionBar", (string title) => {
				var activity = (Activity)this.Context;
				var actionBar = activity.ActionBar;
				if (actionBar != null) {
					var droidColor = TalentPlus.Shared.Helpers.Color.Primary.ToAndroidColor ();
					var parentOfParent = actionBar.CustomView.Parent.Parent as FrameLayout;
					parentOfParent.SetBackgroundColor (droidColor);
					actionBar.CustomView.SetBackgroundColor (droidColor);
				}
				//this.Element.BarBackgroundColor = TalentPlus.Shared.Helpers.Color.Primary.ToFormsColor ();
			});
			MessagingCenter.Subscribe<string> (this, "UpdatePageTitle", (string title) => {
				if (_txtTitle != null) {
					_txtTitle.Text = title;
				}
			});
			MessagingCenter.Subscribe<string> (this, "ForceHideBackButton", title => {
				if (_btnBack != null) {
					_btnBack.Visibility = global::Android.Views.ViewStates.Gone;
				}
			});
		}

		void UnsubscribeMessages ()
		{
			MessagingCenter.Unsubscribe<string> (this, "UpdatePageTitle");
			MessagingCenter.Unsubscribe<string> (this, "ReloadActionBar");
			MessagingCenter.Unsubscribe<string> (this, "ForceHideBackButton");
		}

		private void _btnBack_Click (object sender, EventArgs e)
		{
			this.Element.PopAsync (true);
		}

	}
}

