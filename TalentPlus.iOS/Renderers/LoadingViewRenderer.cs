using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using System.Drawing;
using System.ComponentModel;
using TalentPlus.Shared;
using Foundation;

[assembly:ExportRenderer(typeof(BaseView), typeof(TalentPlus.iOS.LoadingViewRenderer))]
namespace TalentPlus.iOS
{
	public class LoadingViewRenderer : PageRenderer
	{
		#region PRIVATE VARIABLES
		UIView _popupView = null;
		UIView MainUIView = null;
		BaseView MainBaseView = null;
		#endregion

		public LoadingViewRenderer ()
		{
		}

		protected override void OnElementChanged (VisualElementChangedEventArgs e)
		{
			base.OnElementChanged (e);

			var page = e.NewElement as BaseView;
			MainBaseView = page;
			MainUIView = NativeView;

			page.PropertyChanged += (object sender, PropertyChangedEventArgs args) => OnElementPropertyChanged(sender, args);
		}

		private void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == BaseView.LoadingProperty.PropertyName && MainBaseView != null && MainUIView != null) {
				bool isShow = MainBaseView.LoadingViewFlag;
				if (isShow == true) {
					_popupView = new UIView ();
					_popupView.Frame = new RectangleF (0, 0, (float)MainUIView.Frame.Width, (float)MainUIView.Frame.Height);
					_popupView.Alpha = 0.5f;
					_popupView.BackgroundColor = UIColor.Black;

					var label = new UILabel (new RectangleF ((float)MainUIView.Frame.Width / 2 - 100, (float)MainUIView.Frame.Height / 2 - 20, 200, 40));
					label.AdjustsFontSizeToFitWidth = true;
					label.TextColor = UIColor.White;
					label.TextAlignment = UITextAlignment.Center;
					label.Text = "Loading...";

					_popupView.Add (label);

					MainUIView.Add (_popupView);
					MainUIView.BringSubviewToFront (_popupView);
				} else {
					if (_popupView != null) {
						_popupView.RemoveFromSuperview ();
					}
				}
			}
			if (e.PropertyName == BaseView.HideBackProperty.PropertyName) {
				ViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear (animated);
			if (MainBaseView.Title == "Search Results") {
				ViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
			}
		}
	}
}

