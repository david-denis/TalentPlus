using System;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace TalentPlus.Shared
{
	public class IntroView : BaseView
	{
		private Action<bool> PageMoveEvent;

		#region Constructors

		public IntroView(string imageName, string header, string detail, Action<bool> pageMoveEvent = null)
		{
			PageMoveEvent = pageMoveEvent;

			var image = new Image()
			{
				Source = ImageSource.FromFile(Device.OnPlatform(iOS: imageName, Android: imageName, WinPhone: "Assets/" + imageName)),
				Aspect = Aspect.Fill
			};

			var titleLabel = new UnileverLabel {
				TextColor = Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				Text = header,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
			};

			//TODO: Determine size of text for Smaller phones (now iPhone5, Nexus5 tested)
			var MainLayout = new AbsoluteLayout {
				VerticalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.Transparent
			};

			Button btnGoBack = new Button {
				Text = "<  Go Back",
				TextColor = Helpers.Color.UniLeverBlue.ToFormsColor(),
				BackgroundColor = Color.White,
				FontSize = Device.OnPlatform(16, 16, 16)
			};

			Button btnGoNext = new Button {
				Text = "Let's Go!  >",
				TextColor = Color.White,
				BackgroundColor = Helpers.Color.UniLeverBlue.ToFormsColor(),
				FontSize = Device.OnPlatform(16, 16, 16)
			};

			AbsoluteLayout.SetLayoutFlags (image, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (image, new Rectangle (0, 0, 1, 1));
			MainLayout.Children.Add (image);

			AbsoluteLayout.SetLayoutFlags (titleLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (titleLabel, new Rectangle (0, Device.OnPlatform<double>(0.03, 0.02, 0.01), 1, 0.08));
			MainLayout.Children.Add (titleLabel);

			AbsoluteLayout.SetLayoutFlags (btnGoNext, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (btnGoNext, new Rectangle (0.9, Device.OnPlatform<double>(0.98, 0.98, 0.99), 0.4, 0.065));
			MainLayout.Children.Add (btnGoNext);

			AbsoluteLayout.SetLayoutFlags (btnGoBack, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds (btnGoBack, new Rectangle (0.1, Device.OnPlatform<double>(0.98, 0.98, 0.99), 0.4, 0.065));
			MainLayout.Children.Add (btnGoBack);

			btnGoBack.Clicked += OnGoBackClicked;
			btnGoNext.Clicked += OnGoNextClicked;

			Content = MainLayout;
		}

		#endregion

		#region Click Events
		private async void OnGoBackClicked (object sender, EventArgs e)
		{
			if (PageMoveEvent != null)
				PageMoveEvent.Invoke(false);
		}

		private async void OnGoNextClicked (object sender, EventArgs e)
		{
			if (PageMoveEvent != null)
				PageMoveEvent.Invoke(true);
		}
		#endregion
	}
}

