using System;
using Xamarin.Forms;
using System.Collections.Generic;
using XLabs.Forms.Controls;
using System.Threading.Tasks;


#if __ANDROID__
using TalentPlusAndroid;
#endif

namespace TalentPlus.Shared
{
	public class HomeTabbedView : MyNavigationPage
	{
		public static readonly BindableProperty DrawerWidthProperty = BindableProperty.Create<HomeTabbedView, int>(p => p.DrawerWidth, default(int));

		public int DrawerWidth
		{
			get { return (int)GetValue(DrawerWidthProperty); }
			set { SetValue(DrawerWidthProperty, value); }
		}

		private HomeViewModel ViewModel 
		{
			get { return BindingContext as HomeViewModel; }
		}

		public static Dictionary<MenuType, MyNavigationPage> pages;

		TabbedIconPage tabbedPage;

		//private MeView meview;
		public OverviewView overview;
		public ActivitiesView activities;
		private TeamView team;
		private SettingsPage settings;

#if __IOS__
		private ExploreThemesView exploreThemeView;
#endif

		private bool FirstAppear = true;

		public HomeTabbedView ()
		{
			BackgroundColor = Color.White;
			pages = new Dictionary<MenuType, MyNavigationPage>();
			BindingContext = new HomeViewModel();

			overview = new OverviewView() { Title = "Overview", Icon = "overview.png" };
			activities = new ActivitiesView() { Title = "Activities", Icon = "activities.png" };
			team = new TeamView() { Title = "My Team", Icon = "team.png" };
			settings = new SettingsPage() { Title = "Settings", Icon = "small_icons_menu_settings.png" };

#if __IOS__
			exploreThemeView = new ExploreThemesView(activities, ViewModelLocator.ActivitiesViewModel) { Title = "Explore Themes", Icon = "themes.png" };
#endif
			tabbedPage = new TabbedIconPage
			{
				BackgroundColor = Color.White,
				Children =
				{
					overview,
					activities,
					#if __IOS__
						exploreThemeView,
					#endif
					team,
					settings
				},
				Title = "Test test"
			};


            this.PushAsync(tabbedPage);

			//this.Children.Add(overview);
			//this.Children.Add(activities);
			//this.Children.Add(team);
			//this.Children.Add(settings);
		}

		public async Task OverviewRefresh()
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				try
				{
					if (tabbedPage.CurrentPage is OverviewView)
					{
						overview.OnAppearingActions();
					}
				}
				catch (Exception ex)
				{
					Console.Write(ex);
				}
			});
		}

		public async Task ActivityInProgress(string activityId)
		{
			ActivitiesView.BlackActivityIdList.Add(activityId);
			ActivitiesView.IsNeedReload = true;
			activities.SetUpPages(null, null);
		}

		public async Task RemoveActivity(Activity activity)
		{
			ActivitiesView.IsNeedReload = true;
			activities.SetUpPages(null, null);
		}

		public async Task RefreshActivities()
		{
			ActivitiesView.IsNeedReload = true;
			activities.SetUpPages(null, null);
		}

		public async Task ActivityFinished(Activity activity)
		{
			ActivitiesView.BlackActivityIdList.Remove(activity.Id);
			ActivitiesView.IsNeedReload = true;

			activities.SetUpPages(null, null);
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (FirstAppear)
			{
				TalentPlusApp.TalentApp.StopPotentialLoadings();
				FirstAppear = false;

				TalentPlus.Shared.Helpers.Utility.SetScreenTitle (overview.Title);
			}
		}
	}

}

