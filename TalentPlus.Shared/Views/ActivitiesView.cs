using System;
using System.Collections.Generic;
using Xamarin;
using Xamarin.Forms;

namespace TalentPlus.Shared
{
    public class ActivitiesView : CarouselPage, IDisposable
    {
		private ActivitiesViewModel ViewModel
		{
			get { return BindingContext as ActivitiesViewModel; }
		}

		private List<ActivityDetailsView> ActivitiesDetailsView = new List<ActivityDetailsView>();

		/// <summary>
		/// Page when no activities are available
		/// </summary>
		private ContentPage NoActivitiesPage = new ContentPage();

		public static List<string> BlackActivityIdList = new List<string>();
		public static bool IsNeedReload = true;

		public static bool IsReloading = false;
	
		private const int WINDOW_SIZE = 1;
		private int CurrentIndex = 0;

		~ActivitiesView()
		{
			Dispose();
		}

		public void Dispose()
		{
//			MessagingCenter.Unsubscribe<CustomizePopupLayout> (this, "HideImage");
//			MessagingCenter.Unsubscribe<ActivityDetailsView> (this, "ShowImage");
//				
			ViewModel.BusyStateChanged -= SetUpPages;
			//ViewModel.Activities.CollectionChanged -= SetUpPages;
			CurrentPageChanged -= ActivitiesView_CurrentPageChanged;
			foreach (var view in ActivitiesDetailsView)
			{
				view.Dispose();
			}
		}

		public ActivitiesView()
		{
			//this.Title = "Pick Your Activity";

			BindingContext = ViewModelLocator.ActivitiesViewModel;

			//Set up the pages of the Carousel when the Activities are loaded for the for the first time or the list of activities is changed
			ViewModel.BusyStateChanged += SetUpPages;
			//ViewModel.Activities.CollectionChanged += SetUpPages;

//			var yesButton = new TalentPlus.Shared.Helpers.CustomImageButton("Yes", "\u2714")
//			{				//yesButton = new TalentPlus.Shared.Helpers.CustomImageButton("Yes", "star_icon.svg")
//				HorizontalOptions = LayoutOptions.FillAndExpand,
//			}; 
//
//			yesButton.Tapped += async () => {
//				var whenView = new WhenView (new Activity());
//				await this.Navigation.PushAsync (whenView);
//			};


			NoActivitiesPage.Content = new StackLayout
			{
				Children = {
					new Label {
						Text = "No activities available.",
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center
					}
					//,yesButton

				}
			};

			Children.Add(new ContentPage
			{
				Content = new StackLayout
				{
					Children = {
						new Label {
							Text = "Loading...",
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Center
						}
					}
				}
			});

			//Load feedback contents when current page change
			CurrentPageChanged += ActivitiesView_CurrentPageChanged;
		}

		protected void ActivitiesView_CurrentPageChanged(object sender, EventArgs e)
		{
			if (IsReloading)
			{
				return;
			}
			int newIndex = this.Children.IndexOf(this.CurrentPage);

  			CurrentIndex = newIndex;

			#if __IOS__
			UnloadContents();
			#endif
			LoadContents();

			//TalentPlus.Shared.Helpers.Utility.SetScreenTitle("Pick your activity");
			//if (CurrentPage is ActivityDetailsView)
			//{
			//	//await ((ActivityDetailsView)CurrentPage).LoadFeedbackPosts();

			//	//this.Title = ((Activity)CurrentPage.BindingContext).ShortDescription;
			//	//TalentPlus.Shared.Helpers.Utility.SetScreenTitle (this.Title);
			//}
		}

		private void LoadContents()
		{
			int lowIndex = CurrentIndex - WINDOW_SIZE >= 0 ? CurrentIndex - WINDOW_SIZE : 0;
			int highIndex = CurrentIndex + WINDOW_SIZE <= this.Children.Count - 1 ? CurrentIndex + WINDOW_SIZE : this.Children.Count - 1;

			for (int i = lowIndex; i <= highIndex; i++)
			{
				if (this.Children[i] is ActivityDetailsView)
				{
					ActivityDetailsView custom = (ActivityDetailsView)this.Children[i];
					custom.LoadContent();
				}
			}
		}

		private void UnloadContents()
		{
			int lowIndex = CurrentIndex - WINDOW_SIZE >= 0 ? CurrentIndex - WINDOW_SIZE : 0;
			int highIndex = CurrentIndex + WINDOW_SIZE <= this.Children.Count - 1 ? CurrentIndex + WINDOW_SIZE : this.Children.Count - 1;

			if (lowIndex - 1 >= 0)
			{
				if (this.Children[lowIndex - 1] is ActivityDetailsView)
				{
					ActivityDetailsView custom = (ActivityDetailsView)this.Children[lowIndex - 1];
					custom.UnloadContent();
				}
			}

			if (highIndex + 1 <= this.Children.Count - 1)
			{
				if (this.Children[highIndex + 1] is ActivityDetailsView)
				{
					ActivityDetailsView custom = (ActivityDetailsView)this.Children[highIndex + 1];
					custom.UnloadContent();
				}
			}
		}

		private void UnloadAllContents()
		{
			foreach (ContentPage child in this.Children)
			{
				if (child is ActivityDetailsView)
				{
					ActivityDetailsView custom = (ActivityDetailsView)child;
					custom.UnloadContent();
				}
			}
		}

		/// <summary>
		/// Remove the page with the activityId from the Carousel
		/// </summary>
		/// <param name="activityId"></param>
		public void RemovePage(string activityId)
		{
			ActivityDetailsView activityDetailsView = GetActivityDetailsViewFromActivityId(activityId);
			if (activityDetailsView != null && Children.Contains(activityDetailsView))
			{
				if (Children.Count == 1)
				{
					Children.Add(NoActivitiesPage);
				}
				Children.Remove(activityDetailsView);
			}
		}

		/// <summary>
		/// Add a page ActivityDetailView with the Activity provided to the Carousel
		/// </summary>
		/// <param name="activity"></param>
		public void AddPage(Activity activity)
		{
			ActivityDetailsView activityDetailsView = GetActivityDetailsViewFromActivityId(activity.Id);
			if (activityDetailsView != null && !Children.Contains(activityDetailsView))
			{
				//activityDetailsView.UnloadContent();
				Children.Add(activityDetailsView);
				if (Children.Contains(NoActivitiesPage))
				{
					Children.Remove(NoActivitiesPage);
				}
			}
			else
			{
				var activityDetailView = new ActivityDetailsView(this, activity as Activity, ViewModel, false);

				activityDetailView.AcceptedActivityDetails += async (senderDetails, argsDetails) =>
				{
					await TalentPlusApp.RootPage.ActivityInProgress(((ActivityDetailsView)senderDetails).GetActivityId());
				};

				ActivitiesDetailsView.Add(activityDetailView);
				Children.Add(activityDetailView);
			}
		}

		/// <summary>
		/// Add pages when there is a change to the activities list
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void SetUpPages(object sender, EventArgs e)
		{
			if (ViewModel.IsBusy)
				return;

			//UnloadAllContents();
			IsReloading = true;
			Children.Clear();
			IsReloading = false;

			if (ViewModel.Activities.Count == 0)
			{
				Children.Add(NoActivitiesPage);
				return;
			}
			if (ActivitiesDetailsView.Count >= Children.Count)
			{
				foreach (Activity activity in ViewModel.Activities)
				{
					if (BlackActivityIdList.Contains(activity.Id) == false && ( (GetActivityDetailsViewFromActivityId(activity.Id) == null) ||
						(GetActivityDetailsViewFromActivityId(activity.Id) != null && !Children.Contains(GetActivityDetailsViewFromActivityId(activity.Id)))))
					{
						AddPage(activity);
						//break;
					}
				}
			}

		}

		public ActivityDetailsView GetActivityDetailsViewFromActivityId(string activityId)
		{
			return ActivitiesDetailsView.Find(a => a.GetActivityId() == activityId);
		}

		protected override void OnAppearing()
		{			
			base.OnAppearing();

			if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy || ViewModel.Activities.Count > 0 || ViewModel.EmptiedActivities) {
				if (IsNeedReload == true) {
					SetUpPages (null, null);
				} 

				IsNeedReload = false;
				return;
			}



			ViewModel.LoadItemsCommand.Execute(null);

//			MessagingCenter.Subscribe<ActivityDetailsView> (this, "ShowImage", (ActivityDetailsView view) => {
//				//NavigationPage.SetHasNavigationBar(this, false);
//			});

			MessagingCenter.Subscribe<iOSPopupLayout> (this, "HideImage", (iOSPopupLayout view) => {
				NavigationPage.SetHasNavigationBar(this, true);
			});
		}

		public void Refresh()
		{
			ViewModel.LoadItemsCommand.Execute(null);
		}

		public void MoveToPage(string activityId)
		{
			try{
				for (int i = 0; i < ViewModel.Activities.Count; i++) {
					if (ViewModel.Activities[i].Id.Equals(activityId))
					{
//						if (i < ViewModel.Activities.Count - 1)
//						{
							var nextActivity = ViewModel.Activities[i];
							var activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
							if (activityDetailView != null)
							{
								SelectedItem = activityDetailView;
								CurrentPage = activityDetailView;
							}
							else
							{
								AddPage(nextActivity);
								activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
								if (activityDetailView != null)
								{
									SelectedItem = activityDetailView;
									CurrentPage = activityDetailView;
								}
							}
//						}
					}
				}
			}
			catch (Exception ex) {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "ActivitiesView.MoveToPage()" }
				});
			}
		}

		public void MoveToNextPage(string activityId)
		{
			try{
				for (int i = 0; i < ViewModel.Activities.Count; i++) {
					if (ViewModel.Activities[i].Id.Equals(activityId))
					{
						if (i < ViewModel.Activities.Count - 1)
						{
							var nextActivity = ViewModel.Activities[i + 1];
							var activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
							if (activityDetailView != null)
							{
								SelectedItem = activityDetailView;
								CurrentPage = activityDetailView;
							}
							else
							{
								AddPage(nextActivity);
								activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
								if (activityDetailView != null)
								{
									SelectedItem = activityDetailView;
									CurrentPage = activityDetailView;
								}
							}
						}
						break;
					}
				}
			}
			catch (Exception ex) {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "ActivitiesView.MoveToNextPage()" }
				});
			}
		}

		public void MoveToPrevPage(string activityId)
		{
			try{
				for (int i = 0; i < ViewModel.Activities.Count; i++) {
					if (ViewModel.Activities[i].Id.Equals(activityId))
					{
						if (i > 0)
						{
							var nextActivity = ViewModel.Activities[i - 1];
							var activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
							if (activityDetailView != null)
							{
								SelectedItem = activityDetailView;
								CurrentPage = activityDetailView;
							}
							else
							{
								AddPage(nextActivity);
								activityDetailView = GetActivityDetailsViewFromActivityId(nextActivity.Id);
								if (activityDetailView != null)
								{
									SelectedItem = activityDetailView;
									CurrentPage = activityDetailView;
								}
							}
						}
					}
				}
			}
			catch (Exception ex) {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "ActivitiesView.MoveToPrevPage()" }
				});
			}
		}
    }
}
