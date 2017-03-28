using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;

namespace TalentPlus.Shared
{
	public class FeedBackView : BaseView
	{
		private List<FeedbackViewContent> CurrentLayout = new List<FeedbackViewContent>();		
		private int CurrentLayoutIndex = 0;
		private StackLayout PageLayout = null;
        private SelectedActivity SelectedActivity;
        FeedbackPost Feedback = new FeedbackPost();

		public FeedBackView(Activity activity, SelectedActivity selectedActivity)
		{
			BindingContext = activity;
            SelectedActivity = selectedActivity;

			BackgroundColor = Color.White;

			Title = "Feedback";
			Padding = new Thickness(0, 10, 0, 0);

			var usefullnessView = new Usefullness(activity);
			usefullnessView.ProperlyQuit += selectContentView_ProperlyQuit;

			CurrentLayout.Add(usefullnessView);
			CurrentLayout.Add(new Easiness(activity));
			//CurrentLayout.Add(new Camera());

			//Label header = new Label
			//{
			//	Text = activity.ShortDescription,
			//	Font = Font.SystemFontOfSize(NamedSize.Small),
			//	HorizontalOptions = LayoutOptions.Center
			//};

			PageLayout = new StackLayout
			{
				//Padding = new Thickness(0, 10, 0, 0),
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = 
                {
					CurrentLayout[CurrentLayoutIndex]
				}
			};

			Content = new ScrollView 
			{
				Content = PageLayout
			};

			CurrentLayout[CurrentLayoutIndex].ValidatedFeedback += CurrentLayout_ValidatedFeedback;
			TalentDb.isNeedActivityAgain = false;
		}

		void CurrentLayout_ValidatedFeedback(object sender, EventArgs e)
		{
            if (CurrentLayoutIndex == 0)
            {
                Feedback.EaseRating = CurrentLayout[CurrentLayoutIndex].Rating;
				PageLayout.Children.Remove(CurrentLayout[CurrentLayoutIndex]);
				CurrentLayoutIndex += 1;
				CurrentLayout[CurrentLayoutIndex].ValidatedFeedback += CurrentLayout_ValidatedFeedback;
				PageLayout.Children.Add(CurrentLayout[CurrentLayoutIndex]);
				return;
            }
            else if (CurrentLayoutIndex == 1)
            {
                Feedback.EffectivenessRating = CurrentLayout[CurrentLayoutIndex].Rating;
            }

			/*
			var cameraPage = ViewFactory.CreatePage<CameraViewModel, Page>();

			this.Navigation.PushAsync(cameraPage as Page);*/

			FeedBackSelectContentView selectContentView = new FeedBackSelectContentView(BindingContext as Activity, SelectedActivity, Feedback);

			this.Navigation.PushAsync(selectContentView);

			selectContentView.ProperlyQuit += selectContentView_ProperlyQuit;
			
		}

		void selectContentView_ProperlyQuit(object sender, EventArgs e)
		{
			OnProperlyQuit(EventArgs.Empty);
		}

		protected virtual void OnProperlyQuit (EventArgs e)
        {
			EventHandler handler = ProperlyQuit;
			if (handler != null) {
				handler (this, e);
            }
        }

		public event EventHandler ProperlyQuit;

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}
