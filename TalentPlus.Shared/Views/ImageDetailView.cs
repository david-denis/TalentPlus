using System;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
	public class CustomizePopupLayout : RelativeLayout
	{
		private View _content;
		private View _popup;
		private RelativeLayout _backdrop;

		public View Content {
			get { return _content; }

			set {
				if (_content != null) {
					Children.Remove (this._content);
				}

				_content = value;
				Children.Add (this._content, () => this.Bounds);
			}
		}

		public bool IsPopupActive {
			get { return _popup != null; }
		}

		public async System.Threading.Tasks.Task<bool> ShowPopup (View popupView)
		{
			this.DismissPopup ();
			this._popup = popupView;

			this._content.InputTransparent = true;
			var backdrop = new RelativeLayout {
				BackgroundColor = Color.FromRgba (0, 0, 0, 0.4),
				Opacity = 0,
				GestureRecognizers = { new TapGestureRecognizer () }
			};

			backdrop.Children.Add (_popup,
				Constraint.RelativeToParent (p => 0),
				Constraint.RelativeToParent (p => 0),
				//Constraint.RelativeToParent (p => this._popup.WidthRequest),
				Constraint.RelativeToParent (p => p.Width));
				//Constraint.RelativeToParent (p => this._popup.HeightRequest));

			this._backdrop = backdrop;

			Children.Add (backdrop,
				Constraint.Constant (0),
				Constraint.Constant (0),
				Constraint.RelativeToParent (p => p.Width),
				Constraint.RelativeToParent (p => p.Height)
			);

			if (Device.OS == TargetPlatform.Android) {
				this.Children.Remove (this._content);
				this.Children.Add (this._content, () => this.Bounds);
			}

			this.UpdateChildrenLayout ();

			return await _backdrop.FadeTo (1);
		}

		public async System.Threading.Tasks.Task DismissPopup ()
		{
			if (this._popup != null) {
				await System.Threading.Tasks.Task.WhenAll (_popup.FadeTo (0), _backdrop.FadeTo (0));

				_backdrop.Children.Remove (_popup);
				this.Children.Remove (_backdrop);
				this._popup = null;
			}

			this._content.InputTransparent = false;
		}
	}
	//
	//	public class ImageDetailView : BaseView
	//	{
	//		public ImageDetailView ()
	//		{
	//			var button = new Button {
	//				Text = "Open popup"
	//			};
	//
	//			var popup = new PopupLayout {
	//				Content = new StackLayout {
	//					VerticalOptions = LayoutOptions.Center,
	//					Children = {
	//						button
	//					}
	//				}
	//			};
	//
	//			var label = new Label () {
	//				HeightRequest = 100, WidthRequest = 200
	//			};
	//
	//			button.Clicked += async (sender, e) => {
	//				button.IsEnabled = false;
	//				popup.ShowPopup (label);
	//				for (var i = 0; i < 5; i++) {
	//					label.Text = string.Format ("Disappearing in {0}s...", 5 - i);
	//					await System.Threading.Tasks.Task.Delay (1000);
	//				}
	//				popup.DismissPopup ();
	//				button.IsEnabled = true;
	//			};
	//
	//			// The root page of your application
	//			Content = popup;
	//		}
	//	}

	public class ImageDetailView : ContentView
	{
		public ImageDetailView (FeedbackPost post)
		{
			var stackCenter = new StackLayout () {
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = {
					new Image () {
						Source = post.ImageUrl,
						Aspect = Aspect.AspectFit,
						VerticalOptions = LayoutOptions.CenterAndExpand,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						BackgroundColor = Color.Red
					},
	
				},
			};
	
			stackCenter.GestureRecognizers.Add (new TapGestureRecognizer ((View view) => {
				
			}));
	
			this.BackgroundColor = new Color (0, 0, 0, 0.5);
			this.Opacity = 0.7f;
	
			this.Content = stackCenter;
		}
	}
}

