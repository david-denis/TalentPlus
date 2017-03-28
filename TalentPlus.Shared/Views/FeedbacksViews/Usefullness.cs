using System;

using Xamarin.Forms;
using TalentPlus.Shared.Helpers;
using System.Timers;
using System.Threading.Tasks;

namespace TalentPlus.Shared
{
	public class Usefullness : FeedbackViewContent
	{
		#region PRIVATE MEMBERS
		RatingBarControl RatingControl { get; set; }
		String activityId;
		bool buttonClicked;
		#endregion

		public Usefullness(Activity activity)
		{
			activityId = (activity == null) ? "" : activity.Id;
			Spacing = 0;
			VerticalOptions = LayoutOptions.FillAndExpand;

			UnileverLabel question = new UnileverLabel
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.2,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				XAlign = TextAlignment.Center,
			};

			var fs = new FormattedString();
			fs.Spans.Add(new Span
			{
				Text = "Did you find the \"",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = activity.ShortDescription,
				ForegroundColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = "\" activity useful?",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});

			question.FormattedText = fs;

			RatingControl = new RatingBarControl(50, true, 0, 10, true)
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand
			};

			RatingControl.StarRated += async (s, e) =>
			{
				await Task.Delay(1000);
				validateButton_Clicked(null, null);
				RatingControl.IsChangeable = true;
			};

			var stackLayout = new StackLayout
			{
				//BackgroundColor = Xamarin.Forms.Color.Fuschia,
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(10, 0),
			};

			var hintImage = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				Source = "icon_info_gray.png",
				Scale = 0.7
			};

			var hintText = new UnileverLabel
			{
				Text = "Tap a star to rate this activity",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center
			};

			stackLayout.Children.Add(new StackLayout { Padding = new Thickness(0, 10), Children = { question } });
			stackLayout.Children.Add(RatingControl);
			stackLayout.Children.Add(new StackLayout { Padding = new Thickness(20, 30, 20, 10), Spacing = 0, VerticalOptions = LayoutOptions.End, Children = { hintImage, hintText } });

			var notDoneButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
			};
			notDoneButton.Clicked += notDoneButton_Clicked;

			var notDoneLabel = new UnileverLabel
			{
				Text = "I haven't done it yet",
				TextColor = Xamarin.Forms.Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel))
			};

			notDoneLabel.Tapped += () =>
			{
				notDoneButton_Clicked(null, null);
			};

			var notDoItButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
			};
			notDoItButton.Clicked += notDoItButton_Clicked;

			var notDoItLabel = new UnileverLabel
			{
				Text = "Not going to do it",
				TextColor = Xamarin.Forms.Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel))
			};

			notDoItLabel.Tapped += () =>
			{
				notDoItButton_Clicked(null, null);
			};

			var notDoneAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(notDoneButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(notDoneButton, new Rectangle(0, 0, 1, 1));
			notDoneAbsStack.Children.Add(notDoneButton);

			AbsoluteLayout.SetLayoutFlags(notDoneLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(notDoneLabel, new Rectangle(0, 0, 1, 1));
			notDoneAbsStack.Children.Add(notDoneLabel);

			var notDoItAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(notDoItButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(notDoItButton, new Rectangle(0, 0, 1, 1));
			notDoItAbsStack.Children.Add(notDoItButton);

			AbsoluteLayout.SetLayoutFlags(notDoItLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(notDoItLabel, new Rectangle(0, 0, 1, 1));
			notDoItAbsStack.Children.Add(notDoItLabel);

			var ActionLayout = new StackLayout
			{
				//BackgroundColor = Xamarin.Forms.Color.Purple,
				Spacing = 10,
				Padding = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand,
				//HeightRequest = 180,
				Children = {
					//submitFeedbackAbsStack,
                    notDoneAbsStack,
					notDoItAbsStack
                }
			};

			Children.Add(stackLayout);
			Children.Add(ActionLayout);
		}

		void validateButton_Clicked(object sender, EventArgs e)
		{
			this.OnValidatedFeedback(EventArgs.Empty, RatingControl.Rating);
		}

		async void notDoneButton_Clicked(object sender, EventArgs e)
		{
			if (buttonClicked) { return; }
			buttonClicked = true;
			TalentDb.isNeedActivityAgain = true;
			//await Navigation.PopToRootAsync ();
			
			Activity activity = ViewModelLocator.ActivitiesViewModel.GetActivity(activityId);
			if (activity == null) {
				activity = await ViewModelLocator.ActivitiesViewModel.GetActivityFromDb(activityId);
			}

			if (activity != null){
				var whenView = new WhenView (activity);
				whenView.PostponedActivity += async (s, arg) =>
				{
					OnProperlyQuit(EventArgs.Empty);
				};
				await this.Navigation.PushAsync (whenView);
			}
			
			buttonClicked = false;
		}

		async void notDoItButton_Clicked(object sender, EventArgs e)
		{
			if (buttonClicked) { return; }
			buttonClicked = true;
			await TalentPlusApp.RootPage.overview.NotDoingActivity(activityId);
			OnProperlyQuit(EventArgs.Empty);
			//Utility.ForceHideBackButton ();
			await Navigation.PopToRootAsync ();
			buttonClicked = false;
		}

		protected virtual void OnProperlyQuit(EventArgs e)
		{
			EventHandler handler = ProperlyQuit;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler ProperlyQuit;

		/*
		void averageButton_Clicked(object sender, EventArgs e)
		{
			this.OnValidatedFeedback(EventArgs.Empty);
		}

		void noButton_Clicked(object sender, EventArgs e)
		{
			this.OnValidatedFeedback(EventArgs.Empty);
		}

		void yesButton_Clicked(object sender, EventArgs e)
		{
			this.OnValidatedFeedback(EventArgs.Empty);
		}*/
	}
}
