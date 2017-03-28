using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class LoginPage : BaseView
	{
		public string Username = "";
		public string Password = "";

#if __IOS__
		private Entry UserNameEntry;
		private Entry PasswordEntry;
#else
		//private TalentPlus.Shared.Helpers.CustomEntry UserNameEntry;
		//private TalentPlus.Shared.Helpers.CustomEntry PasswordEntry;
		private Entry UserNameEntry;
		private Entry PasswordEntry;
#endif

		public bool ButtonClicked = false;

		TaskCompletionSource<object> ContinueClicked;

		public LoginPage(TaskCompletionSource<object> continueClicked)
		{
			BackgroundColor = Helpers.Color.Blue.ToFormsColor();
			ContinueClicked = continueClicked;

			var layout = new AbsoluteLayout {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
            };

            var backImage = new Image
            {
                Source = "splash_background",
                Aspect = Aspect.Fill,
            };

            AbsoluteLayout.SetLayoutFlags(backImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(backImage, new Rectangle(0, 0, 1, 1));
            layout.Children.Add(backImage);
			/*
			var label = new UnileverLabel
			{
				Text = "Unilever",
				FontSize = 35,
				TextColor = Color.White,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				XAlign = TextAlignment.Center, // Center the text in the blue box.
				YAlign = TextAlignment.Center, // Center the text in the blue box.
			};

            AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(label, new Rectangle(0.5, 0.18, 1, 0.3));
            layout.Children.Add(label);
*/
			var topLogoImage = new Image
			{
				Source = "logo_top",
				Aspect = Aspect.AspectFit,
			};
			AbsoluteLayout.SetLayoutFlags(topLogoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(topLogoImage, new Rectangle(0.5, 0.12, 0.8, 0.3));
			layout.Children.Add(topLogoImage);

            var UserNameBackButton = new Button
            {
                BackgroundColor = Color.White,
                BorderColor = Color.White,
                Text = "",
                BorderRadius = 10,
                BorderWidth = 2,                
            };
            AbsoluteLayout.SetLayoutFlags(UserNameBackButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserNameBackButton, new Rectangle(0.5, 0.5, 0.9, 0.08));
            layout.Children.Add(UserNameBackButton);

			#if __IOS__
            UserNameEntry = new Entry { Placeholder = "Username", BackgroundColor = Color.Transparent };
			#else
			//UserNameEntry = new TalentPlus.Shared.Helpers.CustomEntry { Placeholder = "Username", BackgroundColor = Color.Transparent };
			UserNameEntry = new Entry { Placeholder = "Username", BackgroundColor = Color.Transparent };
			#endif

            AbsoluteLayout.SetLayoutFlags(UserNameEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(UserNameEntry, new Rectangle(0.5, 0.5, 0.9, 0.08));
            layout.Children.Add(UserNameEntry);

            var PasswordBackButton = new Button
            {
                BackgroundColor = Color.White,
                BorderColor = Color.White,
                Text = "",
                BorderRadius = 10,
                BorderWidth = 2,
            };
            AbsoluteLayout.SetLayoutFlags(PasswordBackButton, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PasswordBackButton, new Rectangle(0.5, 0.6, 0.9, 0.08));
            layout.Children.Add(PasswordBackButton);

			#if __IOS__
			PasswordEntry = new Entry { Placeholder = "Password", IsPassword = true, BackgroundColor = Color.Transparent, };
			#else
			//PasswordEntry = new TalentPlus.Shared.Helpers.CustomEntry { Placeholder = "Password", IsPassword = true, BackgroundColor = Color.Transparent, };
			PasswordEntry = new Entry { Placeholder = "Password", IsPassword = true, BackgroundColor = Color.Transparent, };
			#endif

            AbsoluteLayout.SetLayoutFlags(PasswordEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(PasswordEntry, new Rectangle(0.5, 0.6, 0.9, 0.08));
            layout.Children.Add(PasswordEntry);

			var button = new Button { Text = "Login", TextColor = Helpers.Color.Primary.ToFormsColor(), BackgroundColor = Color.White, BorderColor = Color.White, BorderWidth = 2, BorderRadius = 10, };

			button.Clicked += button_Clicked;

            AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0.5, 0.7, 0.9, 0.08));
            layout.Children.Add(button);

            var LogoImage = new Image
            {
                Source = "logo",
                Aspect = Aspect.AspectFit,
            };

            AbsoluteLayout.SetLayoutFlags(LogoImage, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(LogoImage, new Rectangle(0.5, 0.95, 0.8, 0.15));
            layout.Children.Add(LogoImage);

			Content = new ScrollView { Content = layout };
		}

		public bool isBUttonClicked()
		{
			while (!ButtonClicked) ;
			ButtonClicked = false;
			return true;
		}

		void button_Clicked(object sender, EventArgs e)
		{
			if (ButtonClicked)
			{
				return;
			}
			ButtonClicked = true;
			Username = UserNameEntry.Text.ToLower();
			Password = PasswordEntry.Text;
			ContinueClicked.TrySetResult(null);
		}

		public void ChangeCompletionTask(TaskCompletionSource<object> continueClicked)
		{
			ContinueClicked = continueClicked;
		}

		public void AnimateLoading()
		{
			LoadingViewFlag = true;
		}

		public void StopAnimateLoading()
		{
			LoadingViewFlag = false;
		}

	}
}
