using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;

namespace TalentPlus.Shared.Helpers
{
	public class CustomImageButton : AbsoluteLayout
	{
		public Action Tapped { get; set; }

		private TalentPlus.Shared.Helpers.DarkIceImage _backImage;
		private UnileverLabel _buttonText;


		public CustomImageButton (String text, String icon)
			: base ()
		{
			this.VerticalOptions = LayoutOptions.Fill;
			this.HorizontalOptions = LayoutOptions.Fill;

			this.BackgroundColor = Xamarin.Forms.Color.Transparent;
			/*
            var backImage = new DarkIceImage
            {
                Aspect = Aspect.Fill,
                Source = icon,
            };*/

			var backImageButton = new Button {
				BackgroundColor = Helpers.Color.White.ToFormsColor (),
				BorderColor = Helpers.Color.White.ToFormsColor (),
				BorderWidth = 2,
				BorderRadius = 5,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			//var backImage = new Image
			//{
			//	Aspect = Aspect.AspectFit,
			//	Source = icon,
			//	WidthRequest = 40,
			//};

//            var backImage = new SvgColorIcon
//            {
//                SvgPath = icon,
//                SvgAssembly = typeof(TalentPlusApp).GetTypeInfo().Assembly,
//                HorizontalOptions = LayoutOptions.Center,
//                VerticalOptions = LayoutOptions.Center,
//                BackgroundColor = Xamarin.Forms.Color.Transparent,
//                IconColor = Helpers.Color.Primary.ToIntColor(),
//                //				HeightRequest = 5,
//                //				WidthRequest =5,
//            };


//			var backImage = new Label {
//				Text = icon,
//				FontFamily = "ZapfDingbatsITC",
//				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
//				TextColor = Helpers.Color.Primary.ToFormsColor (),
//				FontAttributes = Xamarin.Forms.FontAttributes.Bold,
//				VerticalOptions = LayoutOptions.FillAndExpand,
//				HorizontalOptions = LayoutOptions.FillAndExpand,
//				XAlign = TextAlignment.Center,
//				YAlign = TextAlignment.Center,
//			};

			var imageHeight = Device.GetNamedSize (NamedSize.Medium, typeof(Label));

			string iconSource = "clock.png";

			switch (icon) {
			case "\u2714":	
				iconSource = "check_icon.png";
				break;

			case "🕗":	
				iconSource = "clock.png";
				break;

			case "\u2716":	
				iconSource = "cross_icon.png";
				break;
			default:
				break;
			}

			_backImage = new TalentPlus.Shared.Helpers.DarkIceImage {
				Source = iconSource,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = imageHeight,
				WidthRequest = imageHeight,
				FilterColor = Helpers.Color.Primary.ToFormsColor ()
			};

			_buttonText = new UnileverLabel {
				Text = text,
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(UnileverLabel)),
				//#if __IOS__
				//#else
				//FontSize = 14,//Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
				//#endif
				TextColor = Helpers.Color.Primary.ToFormsColor (),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};

			AbsoluteLayout.SetLayoutFlags (backImageButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (backImageButton, new Rectangle (0, 0, 1, 1));
			this.Children.Add (backImageButton);

			AbsoluteLayout.SetLayoutFlags (_backImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (_backImage, new Rectangle (0.05, 0, 0.2, 1));
			//AbsoluteLayout.SetLayoutBounds (backImage, new Rectangle (0.05, 0.5, 0.2, 0.4));
			this.Children.Add (_backImage);

			AbsoluteLayout.SetLayoutFlags (_buttonText, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (_buttonText, new Rectangle (0, 0, 1, 1));
			this.Children.Add (_buttonText);

			backImageButton.Clicked += (sender, e) => {
				if (Tapped != null)
					Tapped.Invoke ();
			};

			_buttonText.Tapped += () => {
				if (Tapped != null)
					Tapped.Invoke ();
			};
		}

		public void RefreshColorFilter()
		{
			if (_backImage != null) {
				_backImage.FilterColor = Helpers.Color.Primary.ToFormsColor (); 
				_buttonText.TextColor =  Helpers.Color.Primary.ToFormsColor (); 
			}
		}
	}

	public class CustomImageButtonNotifications : AbsoluteLayout
	{
		public Action Tapped { get; set; }

		private UnileverLabel _buttonText;

		public CustomImageButtonNotifications (String text, String icon, Color textColor)
			: base ()
		{
			this.VerticalOptions = LayoutOptions.Fill;
			this.BackgroundColor = Xamarin.Forms.Color.Transparent;

			var backImage = new DarkIceImage {
				Aspect = Aspect.Fill,
				Source = icon,
				FilterColor = Helpers.Color.Primary.ToFormsColor ()
			};

			_buttonText = new UnileverLabel {
				Text = text,
#if __IOS__
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(UnileverLabel)),
#else
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
#endif
				TextColor = textColor.ToFormsColor (),
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};

			AbsoluteLayout.SetLayoutFlags (backImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (backImage, new Rectangle (0, 0, 1, 1));
			this.Children.Add (backImage);

			AbsoluteLayout.SetLayoutFlags (_buttonText, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (_buttonText, new Rectangle (0, 0, 1, 1));
			this.Children.Add (_buttonText);

			backImage.Tapped += () => {
				if (Tapped != null)
					Tapped.Invoke ();
			};

			_buttonText.Tapped += () => {
				if (Tapped != null)
					Tapped.Invoke ();
			};
		}
	}
}
