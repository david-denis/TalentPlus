using System;
using Xamarin.Forms;
using System.Collections.Generic;
using XLabs.Forms.Controls;


#if __ANDROID__
using TalentPlusAndroid;
#endif

namespace TalentPlus.Shared
{
	public class HomeView : MasterDetailPage
	{
		public static readonly BindableProperty DrawerWidthProperty = BindableProperty.Create<HomeView, int>(p => p.DrawerWidth, default(int));

		public int DrawerWidth
		{
			get { return (int)GetValue(DrawerWidthProperty); }
			set { SetValue(DrawerWidthProperty, value); }
		}

		private HomeViewModel ViewModel 
		{
			get { return BindingContext as HomeViewModel; }
		}

		HomeMasterView master;
		public static Dictionary<MenuType, MyNavigationPage> pages;

		public HomeView ()
		{
			//DrawerWidth = 100;
			pages = new Dictionary<MenuType, MyNavigationPage>();
			BindingContext = new HomeViewModel ();

			Master = master = new HomeMasterView (ViewModel);

			var homeNav = new MyNavigationPage(master.PageSelection) {
				BarBackgroundColor = Helpers.Color.Gray.ToFormsColor(),
				BarTextColor = Color.Black
			};

			Detail = homeNav;

			pages.Add (MenuType.Activities, homeNav);

			master.PageSelectionChanged = async (menuType) =>
			{

				if (Detail != null && Device.OS == TargetPlatform.WinPhone)
				{
					await Detail.Navigation.PopToRootAsync();
				}

				MyNavigationPage newPage;
				if (pages.ContainsKey(menuType))
				{
					newPage = pages[menuType];
				}
				else
				{
					newPage = new MyNavigationPage(master.PageSelection)
					{
						BarBackgroundColor = Helpers.Color.Gray.ToFormsColor(),
						BarTextColor = Color.Black
					};

					pages.Add(menuType, newPage);
				}

				Detail = newPage;
				Detail.Title = master.PageSelection.Title;
				IsPresented = false;
			};

			this.Icon = "slideout.png";
		}

		public void RefreshCurrentPage()
		{
			master.RefreshCurrentPage ();
		}
	}


	public class HomeMasterView : BaseView
	{
		public Action<MenuType> PageSelectionChanged;
		private Page pageSelection;
		private MenuType menuType = MenuType.Activities;

		public Page PageSelection
		{
			get { return pageSelection; }
			set
			{
				pageSelection = value; 
				if (PageSelectionChanged != null)
					PageSelectionChanged (menuType);
			}
		}

		private bool FirstAppear = true;

		private MeView meview;
		private OverviewView overview;
		private ActivitiesView activities;
		private TeamView team;
		private SettingsPage settings;

		private TPListView listView;

		public HomeMasterView(HomeViewModel viewModel)
		{
			this.Icon = "slideout.png";
			this.Title = "Menu";
			BindingContext = viewModel;
      
			var layout = new StackLayout { Spacing = 0 };

			listView = new TPListView ();

			var cell = new DataTemplate(typeof(MenuCell));

			#if __ANDROID__
			cell.SetBinding(MenuCell.TextProperty, "Title");
			cell.SetBinding(MenuCell.ImageSourceProperty, "Icon");
            cell.SetBinding(MenuCell.TextColorProperty, "TextColor");
			#else
			cell.SetBinding(MenuCell.TextProperty, HomeViewModel.TitlePropertyName);
			cell.SetBinding(MenuCell.ImageSourceProperty, "Icon");
			#endif

			cell.SetValue(VisualElement.BackgroundColorProperty, Color.Red);

			listView.HasUnevenRows = true;
			listView.ItemTemplate = cell;

			listView.ItemsSource = viewModel.MenuItems;
			if (activities == null)
				activities = new ActivitiesView();

			PageSelection = activities;

			//Change to the correct page
			listView.ItemSelected += (sender, args) =>
			{
				var menuItem = listView.SelectedItem as HomeMenuItem;
				menuType = menuItem.MenuType;
				switch (menuItem.MenuType)
				{
					case MenuType.Me:
						if (meview == null)
							meview = new MeView();

						PageSelection = meview;
						break;
					case MenuType.Overview:
						if (overview == null)
							overview = new OverviewView();

						PageSelection = overview;
						break;
					case MenuType.Activities:
						if (activities == null)
						{
							if (activities != null)
								activities.Dispose();

							activities = new ActivitiesView();
						}

						PageSelection = activities;
						break;
					case MenuType.Team:
						if (team == null)
							team = new TeamView();

						PageSelection = team;
						break;
					case MenuType.Settings:
						if (settings == null)
							settings = new SettingsPage();

						PageSelection = settings;
						break;
				}

                viewModel.ChangeTextColor(menuType);
			};

			listView.SelectedItem = viewModel.MenuItems[2];

			//#region ProfileLayout
			//var absoluteLayout = new AbsoluteLayout { HeightRequest = 100 };
			//var image = new Image { Source = "pattern_header.png", Aspect = Aspect.AspectFill };

			//var stackLayoutProfile = new StackLayout
			//	{
			//		VerticalOptions = LayoutOptions.StartAndExpand,
			//		Orientation = StackOrientation.Horizontal,
			//		Padding = new Thickness(10, 0),
			//		Children =
			//		{
			//			new StackLayout 
			//			{ 
			//				VerticalOptions = LayoutOptions.Center,
			//				Padding = new Thickness(0, 0, 10, 0),
			//				HeightRequest = 70,
			//				WidthRequest = 70,
			//				Children =
			//				{
			//					new Image { Source = "user_icon.png", WidthRequest = 70, HeightRequest = 70 }
			//				} 
			//			},
			//			new UnileverLabel { Text = "Matthieu Lepinay", Font = Font.SystemFontOfSize(NamedSize.Large), TextColor = Color.White, VerticalOptions = LayoutOptions.CenterAndExpand }
			//		}
			//	};

			//absoluteLayout.Children.Add (image, new Rectangle (0, 0, 1, 1), AbsoluteLayoutFlags.All);
			//absoluteLayout.Children.Add(stackLayoutProfile, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			//#endregion

			//layout.Children.Add(absoluteLayout);
			layout.Children.Add(listView);

			//HomeView.MainPopup.Content = layout;

			Content = layout;

			MessagingCenter.Subscribe<ActivitiesView, string>(this, "activityInProgress", (page, activityId) =>
				{
					ActivitiesView.BlackActivityIdList.Add(activityId);
					ActivitiesView.IsNeedReload = true;
					if (activities != null)
						activities.Dispose();
					
					activities = new ActivitiesView();
					HomeView.pages.Remove(MenuType.Activities);

					var newPage = new MyNavigationPage(activities)
					{
						BarBackgroundColor = Helpers.Color.Gray.ToFormsColor(),
						BarTextColor = Color.Black
					};

					HomeView.pages.Add(MenuType.Activities, newPage);

					if (PageSelection is ActivitiesView)
					{
						PageSelection = activities;
					}
				});

		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (FirstAppear)
			{
				TalentPlusApp.TalentApp.StopPotentialLoadings();
				FirstAppear = false;
			}
		}

		public void RefreshCurrentPage()
		{
			if (PageSelection is ActivitiesView) {
				activities.Refresh ();
			} else if (pageSelection is OverviewView) {
				overview.Refresh ();
			}
		}
	}

}

