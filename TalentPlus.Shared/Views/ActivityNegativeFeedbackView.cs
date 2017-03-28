using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class ActivityNegativeFeedbackView : BaseView
	{
		bool Alreadyclicked { get; set; }
		private Activity activity;
		private ScrollView scrollContent;

		public ActivityNegativeFeedbackView (Activity act)
		{
			activity = act;
			BindingContext = activity;

			Alreadyclicked = false;

			BuildContent ();
		}

		private void BuildContent()
		{
			BackgroundColor = Color.FromRgba(255, 255, 255, 200);

			var NegativeStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Padding = 20,
				Spacing = 10
			};
					
			foreach (var declinedOption in activity.ActivityDeclinedOptions)
			{
				RoundedBox roundedBox = new RoundedBox {
					CornerRadius = 3,
					BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				};

				var label = new Label {
					VerticalOptions = LayoutOptions.Center,
					XAlign = TextAlignment.Center,
					Text = declinedOption.Description,
					TextColor = Helpers.Color.White.ToFormsColor(),
				};

				StackLayout labelStack = new StackLayout {
					Padding = new Thickness(5, 10, 5, 10),
					Children = {
						label
					},
				};

				var grid = new Grid {
					Children = {
						roundedBox,
						labelStack
					}
				};

				var tapGesture = new TapGestureRecognizer ();
				tapGesture.Tapped += (object sender, EventArgs e) => OnValidate(declinedOption);

				grid.GestureRecognizers.Add (tapGesture);

				NegativeStack.Children.Add(grid);
			}

			var fs = new FormattedString();
			fs.Spans.Add(new Span
				{
					Text = activity.FeedbackQuestion,
					ForegroundColor = Helpers.Color.Black.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				});
			
			var question = new UnileverLabel
			{
				//Text = activity.FeedbackQuestion,
				//TextColor = Helpers.Color.Primary.ToFormsColor(),
				//FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
				FormattedText = fs,
				HorizontalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};

			var feedbackImage = new Image{
				HorizontalOptions = LayoutOptions.Center,
				Source = "activities.png",
				WidthRequest = 30,
				HeightRequest = 30,
			};

			var feedbackHintLabel = new UnileverLabel
			{
				Text = "Your feedback helps others",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,

			};

			// Build the page.
			scrollContent = new ScrollView { 
				Content = new StackLayout
            	{
	                Orientation = StackOrientation.Vertical,
	                Spacing = 10,
					Padding = new Thickness(0, Device.OnPlatform<int>(20, 0, 0), 0, 30),
	                HorizontalOptions = LayoutOptions.FillAndExpand,
	                VerticalOptions = LayoutOptions.CenterAndExpand,
	                Children =
	                {
	                    question,
	                    NegativeStack,
						feedbackImage,
						feedbackHintLabel
	                }
				}
            };

			this.Content = scrollContent;
		}

		private async void OnValidate(ActivityDeclinedOption option)
		{
			if (Alreadyclicked) 
			{ return; }
			Alreadyclicked = true;
			// don't need to wait

			if (option.Type == ActivityDelcinedOptionType.Open)
			{
				var textPage = new ActivityDeclinedOtherView();
				textPage.Answer = option;
				textPage.NegativeView = this;
				await Navigation.PushAsync(textPage);
				return;
			}
			LoadingViewFlag = true;
			HideBackButtonFlag = true;
			await RemoveActivityAfterClick(option);
			//TalentDb.SaveOrUpdateItemInThread<NegativeFeedbackPost>(new NegativeFeedbackPost() { Activity = option.Activity, ActivityId = option.ActivityId, AnswerId = option.Id, Value = "", Time = DateTime.Now });
			await TalentDb.SaveOrUpdateItem<NegativeFeedbackPost>(new NegativeFeedbackPost() { Activity = option.Activity, ActivityId = option.ActivityId, AnswerId = option.Id, Value = "", Time = DateTime.Now });

			//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();
			await this.Navigation.PopToRootAsync();
			LoadingViewFlag = false;
		}

		public async Task RemoveActivityAfterClick(ActivityDeclinedOption option)
		{
			UserActivityOrder order = option.Activity.Order;
			order.Hidden = true;

			RemoveActivityFromViewModel();
			await TalentDb.SaveOrUpdateItem<UserActivityOrder>(order);
			await TalentPlusApp.RootPage.RemoveActivity(option.Activity);
		}

		private void RemoveActivityFromViewModel()
		{
			var vm = ViewModelLocator.ActivitiesViewModel;

			if (vm.Activities.Contains(activity))
			{
				vm.Activities.Remove(activity);
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Alreadyclicked = false;
		}
	}
}
