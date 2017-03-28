using System;
using System.Collections.Generic;
using Xamarin.Forms;
#if __ANDROID__
using Android.App;
#endif
namespace TalentPlus.Shared
{
    public class NewActivitiesView : CarouselPage, IDisposable
    {
        private bool _isLoading = false;

        private int _numberOfFirstLoad = 5;
        private int _maxNumberOfLoad = 3;

#if __ANDROID__
        ProgressDialog p = null;
#endif

        public bool LoadingViewFlag
        {

            set
            {
                if (_isLoading == value)
                {
                    return;
                }
                _isLoading = value;


#if __ANDROID__
                if (_isLoading == true)
                {
                    if (p == null)
                    {
						p = new ProgressDialog(Forms.Context);
                        p.SetMessage("Loading...");
                        p.SetCancelable(false);
                    }
                    p.Show();

                }
                else
                {
                    if (p != null)
                    {
                        p.Cancel();
                        //p.Dismiss();
                        //p = null;
                    }
                }
#endif
            }
        }

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

        ~NewActivitiesView()
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

        public NewActivitiesView()
        {
            LoadingViewFlag = true;
            //this.Title = "Pick Your Activity";

            BindingContext = ViewModelLocator.ActivitiesViewModel;

            //Set up the pages of the Carousel when the Activities are loaded for the for the first time or the list of activities is changed
            ViewModel.BusyStateChanged += SetUpPages;
            //ViewModel.Activities.CollectionChanged += SetUpPages;

            NoActivitiesPage.Content = new StackLayout
            {
                Children = {
					new Label {
						Text = "No activities available.",
						VerticalOptions = LayoutOptions.Center,
						HorizontalOptions = LayoutOptions.Center
					}
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

            LoadingViewFlag = false;
        }

        private bool _isLoad = false;

        protected async void ActivitiesView_CurrentPageChanged(object sender, EventArgs e)
        {
            //get the current page index
            var currentPage = CurrentPage as ActivityDetailsView;
            if (currentPage != null)
            {

                int index = ActivitiesDetailsView.IndexOf(currentPage);
                Console.WriteLine("You are at: {0}", index);
				if ((index + 2) == _numberOfFirstLoad && !_isLoad)
                {
                    LoadingViewFlag = true;
                }
            }

            TalentPlus.Shared.Helpers.Utility.SetScreenTitle("Pick your activity");

            //			if (CurrentPage is ActivityDetailsView) {
            //				//await ((ActivityDetailsView)CurrentPage).LoadFeedbackPosts();
            //
            //				//this.Title = ((Activity)CurrentPage.BindingContext).ShortDescription;
            //				//TalentPlus.Shared.Helpers.Utility.SetScreenTitle (this.Title);
            //			}
        }

        private async System.Threading.Tasks.Task LoadRemainingActivities()
        {
            if (_isLoad)
            {
                p.ShowEvent -= p_ShowEvent;
                return;
            }

            for (int i = _numberOfFirstLoad; i < ActivitiesDetailsView.Count; i++)
            {
                Console.WriteLine("Loading activity: {0}", i);
                if (!ActivitiesDetailsView[i].IsLoad)
                {
                    await ActivitiesDetailsView[i].LoadView();
                    await ActivitiesDetailsView[i].LoadFeedbackPosts();
                    Console.WriteLine("Loaded activity: {0}", i);
                }
            }

            _isLoad = true;

        }

        private async void p_ShowEvent(object sender, EventArgs e)
        {
			await System.Threading.Tasks.Task.Delay(1000);
            await LoadRemainingActivities();
            LoadingViewFlag = false;
        }

        /// <summary>
        /// Remove the page with the activityId from the Carousel
        /// </summary>
        /// <param name="activityId"></param>
        private void RemovePage(string activityId)
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
        private void AddPage(Activity activity, bool preLoad = false)
        {
            ActivityDetailsView activityDetailsView = GetActivityDetailsViewFromActivityId(activity.Id);
            if (activityDetailsView != null && !Children.Contains(activityDetailsView))
            {
                Children.Add(activityDetailsView);
                if (Children.Contains(NoActivitiesPage))
                {
                    Children.Remove(NoActivitiesPage);
                }
            }
            else
            {
                var activityDetailView = new ActivityDetailsView(this, activity as Activity, ViewModel, preLoad);

                activityDetailView.AcceptedActivityDetails += (senderDetails, argsDetails) =>
                {
                    MessagingCenter.Send<string>(((ActivityDetailsView)senderDetails).GetActivityId(), "activityInProgress");
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

            Children.Clear();

            if (ViewModel.Activities.Count == 0)
            {
                Children.Add(NoActivitiesPage);
                return;
            }

            //load first 5 activities
            if (ActivitiesDetailsView.Count >= Children.Count && ViewModel.Activities.Count > 0)
            {
                for (int i = 0; i < ViewModel.Activities.Count; i++)
                {
                    var activity = ViewModel.Activities[i];
                    if ((GetActivityDetailsViewFromActivityId(activity.Id) == null) ||
                        (GetActivityDetailsViewFromActivityId(activity.Id) != null && !Children.Contains(GetActivityDetailsViewFromActivityId(activity.Id))))
                    {
                        if (BlackActivityIdList.Contains(activity.Id) == false)
                            AddPage(activity, i < _numberOfFirstLoad);
                    }
                }

                #if __ANDROID__
                    p.ShowEvent += p_ShowEvent;
                #endif
            }


            //			if (ActivitiesDetailsView.Count >= Children.Count) {
            //				foreach (Activity activity in ViewModel.Activities) {
            //					if ((GetActivityDetailsViewFromActivityId (activity.Id) == null) ||
            //					    (GetActivityDetailsViewFromActivityId (activity.Id) != null && !Children.Contains (GetActivityDetailsViewFromActivityId (activity.Id)))) {
            //						if (BlackActivityIdList.Contains (activity.Id) == false)
            //							AddPage (activity);
            //					}
            //				}
            //			}
        }

        private ActivityDetailsView GetActivityDetailsViewFromActivityId(string activityId)
        {
            return ActivitiesDetailsView.Find(a => a.GetActivityId() == activityId);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy || ViewModel.Activities.Count > 0 || ViewModel.EmptiedActivities)
            {
                if (IsNeedReload == true)
                {
                    SetUpPages(null, null);
                }

                IsNeedReload = false;
                return;
            }

            ViewModel.LoadItemsCommand.Execute(null);

            MessagingCenter.Subscribe<ActivityDetailsView>(this, "ShowImage", (ActivityDetailsView view) =>
            {
                //NavigationPage.SetHasNavigationBar(this, false);
            });

            MessagingCenter.Subscribe<iOSPopupLayout>(this, "HideImage", (iOSPopupLayout view) =>
            {
                NavigationPage.SetHasNavigationBar(this, true);
            });
        }

        public void Refresh()
        {
            ViewModel.LoadItemsCommand.Execute(null);
        }

        public void MoveToNextPage(string activityId)
        {
            for (int i = 0; i < ViewModel.Activities.Count; i++)
            {
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
                }
            }

        }

        public void MoveToPrevPage(string activityId)
        {
            for (int i = 0; i < ViewModel.Activities.Count; i++)
            {
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
    }
}
