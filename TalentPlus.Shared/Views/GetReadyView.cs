using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
	public class GetReadyView : BaseView
	{
		protected bool IsClicked { get; set; }

		private SendMessageView _sendMessageView;


		public GetReadyView(Activity activity, SendMessageView sendMessageView)
		{
			_sendMessageView = sendMessageView;
			Helpers.Utility.RefreshActionBar();
			BackgroundColor = Helpers.Color.White.ToFormsColor();
			BindingContext = activity;
			Title = activity.ShortDescription;

			#region Layout

			var mainStackLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = 0,
				Spacing = 10,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			var headerLayout = new StackLayout{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 50,
				Orientation = StackOrientation.Vertical,
				Spacing = 0,
				Padding = new Thickness(0, Device.OnPlatform<int>(20, 0, 0), 0, 0),
				Children = {
					new UnileverLabel{
						Text = activity.ShortDescription,
						TextColor = Color.White,
						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.FillAndExpand,
						XAlign = TextAlignment.Center,
						YAlign = TextAlignment.Center,
					}
				}
			};
			mainStackLayout.Children.Add(headerLayout);

			var toptipLabel = new UnileverLabel
			{
				Text = "Your colleagues have been notified!",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Helpers.Color.Black.ToFormsColor(),
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				HeightRequest = 80,
			};
			mainStackLayout.Children.Add(toptipLabel);

			if (activity != null && activity.SelectedUsers != null && activity.SelectedUsers.Count > 0)
			{
				int i = 0, iAdded = 0;
				while (i < activity.SelectedUsers.Count)
				{
					var userLayout = new StackLayout{
						Padding = 0,
						Spacing = 5,
						Orientation = StackOrientation.Horizontal,
						HeightRequest = 40,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
					};

					iAdded = 0;
					while (iAdded < 6 && i < activity.SelectedUsers.Count)
					{
						var userImage = new TPCircleImage{
							Source = activity.SelectedUsers[i].UserImage,
							WidthRequest = 40,
							HeightRequest = 40,
							HorizontalOptions = LayoutOptions.CenterAndExpand,
							VerticalOptions = LayoutOptions.CenterAndExpand,
							Aspect = Aspect.AspectFill,
						};

						userLayout.Children.Add(userImage);
						iAdded++;
						i++;
					}

					mainStackLayout.Children.Add(userLayout);
				}
			}

			var emailImage = new Image{
				HorizontalOptions = LayoutOptions.Center,
				Source = "ico_mail.png",
				WidthRequest = 100,
				HeightRequest = 100,
			};
			mainStackLayout.Children.Add(emailImage);

			var ContinueButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Return to activities",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			ContinueButton.Clicked += backButton_Clicked;
			mainStackLayout.Children.Add(
				new StackLayout{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Padding = new Thickness(5, 5, 5, 10),
					Spacing = 0,
					Children = {
						ContinueButton
					}
				});

			var feedbackImage = new Image{
				HorizontalOptions = LayoutOptions.Center,
				Source = "ico_light.png",
				WidthRequest = 20,
				HeightRequest = 20,
			};
			mainStackLayout.Children.Add(feedbackImage);

			var feedbackHintLabel = new UnileverLabel
			{
				Text = "For your next activity try to involve people you don't usually interact with",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};
			mainStackLayout.Children.Add(feedbackHintLabel);

			// Build the page.
			this.Content = mainStackLayout;

			#endregion
		}


		protected async void backButton_Clicked(object sender, EventArgs e)
		{
			if (IsClicked)
				return;
			
			IsClicked = true;
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar();
			//MessagingCenter.Send<string>(sender.ToString(), "ReloadTabBar");
			Helpers.Utility.RefreshActionBar();

			TalentPlusApp.BackBlocked = false;
			//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();
			await Navigation.PopModalAsync();
			await _sendMessageView.ClearEverything ();
		}
	}
}
