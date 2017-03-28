using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
	public class DisclaimerPage : BaseView
	{
		private TaskCompletionSource<object> ClickedTask;

		private ScrollView MainScrollView;

		private bool IsViewResized = false;
		Button acceptButton { get; set; }
		Button declineButton { get; set; }

		public DisclaimerPage(TaskCompletionSource<object> clickedTask)
		{
			ClickedTask = clickedTask;

			StackLayout mainStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				Spacing = 0,
				Padding = 0
			};

			//var imageStats = new Image { Source = "profile_top_footer.png", Aspect = Aspect.Fill };
			var heading = new ContentView {
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				Padding = new Thickness(0, 20, 0, 10),
				Content = new Label { 
					Text = "Legal disclaimer",
					TextColor = Helpers.Color.White.ToFormsColor(),
					HorizontalOptions = LayoutOptions.CenterAndExpand,
					VerticalOptions = LayoutOptions.End,
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				}
			};


			MainScrollView = new ScrollView
			{
				//BackgroundColor = Color.Black,
				Padding = new Thickness(5, 0, 5, 0),
				VerticalOptions = LayoutOptions.FillAndExpand,
				IsClippedToBounds = true,
				Content = new Label
						{
							//VerticalOptions = LayoutOptions.StartAndExpand,
							TextColor = Helpers.Color.Gray.ToFormsColor(),
							VerticalOptions = LayoutOptions.FillAndExpand,
							Text = @"
By proceeding to upload your personal data (including comments, pictures, videos or any other media material that pertains to you) in this application you consent that we may process the personal data (including sensitive personal data) that we collect from you in accordance with Unilever’s internal policies on data protection and applicable laws. In particular, we may use the personal data provided by you for the purposes of being shared within the application itself with a global audience of Unilever personnel (including employees, contractors and agency staff). Please also note that this personal data may also be re-used in other Unilever sites and applications.

The personal data provided by you in this application will be accessed by or a transferred to trusted third parties, including, but not limited to Playgen, Vzaar to provide the following services:

·	Develop, support and maintain the application and its functionality
·   Enable use of videos by users
 
The personal data provided by you in this application may also be accessed, transferred to, and stored or processed at, a destination outside the European Economic Area ('EEA'). By submitting your personal data, you agree to this access, transfer, storing or processing.

If you wish for your personal data in this application to be deleted/amended, please contact the relevant individual below. You may also exercise your right of access by contacting the relevant individual below and request the personal data held about you in this application.
 
Please address any questions, comments and requests regarding our data processing practices to Kelly Needham, Global Talent Capability Manager, via Kelly.needham@unilever.com.
"
						}
			};

			var stackLayoutContent = new ContentView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				//HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(10, 0, 10, 0),
				Content = MainScrollView
			};

			MainScrollView.SizeChanged += MainScrollView_LayoutChanged;

			#region Buttons
			acceptButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Agree",
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)) * 1.1,
				BorderWidth = 0,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			acceptButton.Clicked += acceptButton_Clicked;

			declineButton = new Button
			{
				BackgroundColor = Helpers.Color.White.ToFormsColor(),
				Text = "Disagree",
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)) * 1.1,
				BorderWidth = 0,
				BorderColor = Helpers.Color.White.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			declineButton.Clicked += declineButton_Clicked;

			Grid grid = new Grid
			{
				VerticalOptions = LayoutOptions.End,
				RowDefinitions = 
                {
                    new RowDefinition { Height = GridLength.Auto }
                },
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star)  },
					new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) }
                },
				Padding = 10,
				BackgroundColor = Helpers.Color.Gray.ToFormsColor()
			};

			grid.Children.Add(declineButton, 0, 0);
			grid.Children.Add(acceptButton, 1, 0);

			
			#endregion

			mainStack.Children.Add(heading);
			//mainStack.Children.Add(scrollView);
			mainStack.Children.Add(stackLayoutContent);
			mainStack.Children.Add(grid);

			Content = mainStack;
		}

		void MainScrollView_LayoutChanged(object sender, EventArgs e)
		{
			if (!IsViewResized)
			{
				MainScrollView.HeightRequest = MainScrollView.ParentView.Height - 30;
				IsViewResized = true;
			}
		}

		async void acceptButton_Clicked(object sender, EventArgs e)
		{
			acceptButton.Clicked -= acceptButton_Clicked;
			declineButton.Clicked -= declineButton_Clicked;

			User myself = TalentPlusApp.CurrentUser;
			myself.AcceptedConditions = true;

			await TalentDb.SaveOrUpdateItem<User>(myself);
			TalentPlusApp.CurrentUser = myself;

			ClickedTask.TrySetResult (null);
		}

		async void declineButton_Clicked(object sender, EventArgs e)
		{
			await DisplayAlert("Disclaimer Alert", "You need to accept the legal disclaimer to be able to use the application", "Close");
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

