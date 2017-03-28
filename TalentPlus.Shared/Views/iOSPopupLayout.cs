using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
	public class iOSPopupLayout : RelativeLayout
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

		public async Task<bool> ShowPopup (View popupView)
		{
			await this.DismissPopup ();
			this._popup = popupView;

			this._content.InputTransparent = true;
			var backdrop = new RelativeLayout {
				BackgroundColor = Color.FromRgba (0, 0, 0, 0.4),
				Opacity = 0,
				GestureRecognizers = { new TapGestureRecognizer () }
			};

			backdrop.Children.Add (_popup,
				Constraint.RelativeToParent (p => (this.Width / 2) - (this._popup.WidthRequest / 2)),
				Constraint.RelativeToParent (p => (this.Height / 2) - (this._popup.HeightRequest / 1.5f)),
				Constraint.RelativeToParent (p => this._popup.WidthRequest),
				Constraint.RelativeToParent (p => this._popup.HeightRequest));

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

			backdrop.GestureRecognizers.Add (new TapGestureRecognizer (async (View view) => {
				await DismissPopup ();
				MessagingCenter.Send (this, "HideImage");
			}));

			return await _backdrop.FadeTo (1);
		}

		public async Task DismissPopup ()
		{
			if (this._popup != null) {
				await Task.WhenAll (_popup.FadeTo (0), _backdrop.FadeTo (0));

				_backdrop.Children.Remove (_popup);
				this.Children.Remove (_backdrop);
				this._popup = null;
			}

			this._content.InputTransparent = false;
		}
	}

	public class DroidImagePopup : RelativeLayout
	{
		private Image _image = null;
		private PopupLayout _popupLayout;

		public DroidImagePopup(PopupLayout popupLayout, string imageUrl) : base()
		{
			BackgroundColor = Color.FromRgba (0, 0, 0, 0.5);
			_popupLayout = popupLayout;

			_image = new Image () {
				Source = imageUrl,
				Aspect = Aspect.AspectFit,
				WidthRequest = _popupLayout.Width,
				HeightRequest = _popupLayout.Height
			};

			WidthRequest = popupLayout.Width;
			HeightRequest = popupLayout.Height;
		}

		public void Show()
		{
			_popupLayout.Content.InputTransparent = true;

			this.Children.Add (_image,
				Constraint.RelativeToParent (p => (_popupLayout.Width / 2) - (_image.WidthRequest / 2)),
				Constraint.RelativeToParent (p => (_popupLayout.Height / 2) - (_image.HeightRequest / 1.7f)),
				Constraint.RelativeToParent (p => _image.WidthRequest),
				Constraint.RelativeToParent (p => _image.HeightRequest));

			this.GestureRecognizers.Add (new TapGestureRecognizer(async (View view, object obj ) => {

				if (this != null) {

					await Task.WhenAll (this.FadeTo (0), this.FadeTo (0));
					this.Children.Remove (_image);
				}

				_popupLayout.DismissPopup();
				_popupLayout.Content.InputTransparent = false;
			}));

			_popupLayout.ShowPopup (this);

		}


	}

}

