using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class WhenView : BaseView, IDisposable
	{
		private WhenViewModel ViewModel;
		private double sliderVal;

		~WhenView()
		{
			Dispose ();
		}

		#region IDisposable implementation

		public void Dispose ()
		{

		}

		#endregion

		public WhenView (Activity activity)
		{
			Title = "When";
            this.Icon = "overview.png";

			ViewModel = new WhenViewModel(activity);
			BindingContext = activity;
            
			#region Layout
			var question = new UnileverLabel
			{
				Text = "When would you like to do this activity?",
				TextColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};

			var feedbackImage = new TalentPlus.Shared.Helpers.DarkIceImage{
				HorizontalOptions = LayoutOptions.Center,
				Source = "activities.png",
				WidthRequest = 20,
				HeightRequest = 20,
			};

			var feedbackHintLabel = new UnileverLabel
			{
				Text = "Please select one option",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
			};

            var WhenStack = GetWhenStack();

			// Build the page.
			this.Content = new StackLayout
			{
                Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0, Device.OnPlatform<int>(20, 0, 0), 0, 0),
                Spacing = 5,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = 
                {
					question,
					WhenStack,
					feedbackImage,
					feedbackHintLabel
				}
			};

			#endregion

			(BindingContext as Activity).SelectedTime = DateTime.Today;
		}

		private StackLayout GetWhenStack()
		{
			var TodayButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Today",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Helpers.Color.White.ToFormsColor(),
			};
			TodayButton.Clicked += async (sender, e) => { await ChangeOption(1); };

			var TomorrowButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Tomorrow",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Helpers.Color.White.ToFormsColor(),
			};
			TomorrowButton.Clicked += async (sender, e) => { await ChangeOption(2); };

			var NextWeekButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Next Week",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Helpers.Color.White.ToFormsColor(),
			};
			NextWeekButton.Clicked += async (sender, e) => { await ChangeOption(3); };

			var TwoWeeksButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Two Weeks",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Helpers.Color.White.ToFormsColor(),
			};
			TwoWeeksButton.Clicked += async (sender, e) => { await ChangeOption(4); };

			var OneMonthButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "One Month",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				TextColor = Helpers.Color.White.ToFormsColor(),
			};
			OneMonthButton.Clicked += async (sender, e) => { await ChangeOption(5); };

			var whenStackLayout = new StackLayout{
				Spacing = 10,
				Padding = new Thickness(5, 0, 5, 20),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					TodayButton,
					TomorrowButton,
					NextWeekButton,
					TwoWeeksButton,
					OneMonthButton,
				}
			};

			return whenStackLayout;
		}
		bool ButtonClicked { get; set; }
		private async Task ChangeOption(int option)
		{
			if (ButtonClicked) { return; }
			ButtonClicked = true;
			sliderVal = option - 1;
			await OnContinueClicked (null, null);
			ButtonClicked = false;
		}

        private async Task OnContinueClicked(object sender, EventArgs e)
        {
			if (TalentDb.isNeedActivityAgain == false) {
				var whoView = new WhoView(BindingContext as Activity, sliderVal);
				await Navigation.PushAsync (whoView);
			} else {
				TalentDb.isNeedActivityAgain = false;
				OnPostponedActivity(EventArgs.Empty);
				await ViewModelLocator.ActivitiesViewModel.RemovePendingFeedBack((BindingContext as Activity).Id);
				//TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
				LoadingViewFlag = true;
				await ViewModel.ValidateSlider(sliderVal, "", "", true);
				LoadingViewFlag = false;

				await Navigation.PopToRootAsync();
			}	
        }

		protected override void OnAppearing()
		{
			base.OnAppearing();
			ViewModel.LoadItemsCommand.Execute(null);
		}

		protected virtual void OnPostponedActivity(EventArgs e)
		{
			EventHandler handler = PostponedActivity;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler PostponedActivity;
	}
}
