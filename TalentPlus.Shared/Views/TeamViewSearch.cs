using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Ioc;
using XLabs.Platform.Device;
using TalentPlus.Shared.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TalentPlus.Shared
{
	public class TeamViewSearch : BaseView
	{
		class TeamUserInfo : INotifyPropertyChanged
		{
			#region INotifyPropertyChanged implementation

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion

			public String UserEmail { get; set; }
			public String UserImage { get; set; }
			public String UserName { get; set; }

			private String userOption;

			public String UserOption {
				get {
					return userOption;
				}
				set {
					userOption = value;
					OnPropertyChanged ("UserOption");
				}
			}

			public TeamUserInfo(String userEmail, String userImage, String userName, String userOption)
			{
				UserEmail = userEmail;
				UserImage = userImage;
				UserName = userName;
				UserOption = userOption;
			}

			protected virtual void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		/// <summary>
		/// Fake UserList
		/// </summary>
		private ObservableCollection<TeamUserInfo> SearchedUsersList
		{
			get { return searcheduserList; }
			set { searcheduserList = value; OnPropertyChanged("SearchedUsersList"); }
		}

		private ObservableCollection<TeamUserInfo> searcheduserList = new ObservableCollection<TeamUserInfo>();
		private TPListView lvSearchUsers;
		private UsersViewModel ViewModel;
        private StackLayout SelectedUserLayout;
		private Button backButton;
		private SearchBar searchBar;

		private async void OnUserIncludeTapped(object sender, SelectedItemChangedEventArgs e)
		{
			lvSearchUsers.ItemSelected -= OnUserIncludeTapped;
			if (e.SelectedItem == null)
			{
				lvSearchUsers.ItemSelected += OnUserIncludeTapped;
				return;
			}

			var ItemData = e.SelectedItem as TeamUserInfo;
			if (ItemData == null)
			{
				lvSearchUsers.ItemSelected += OnUserIncludeTapped;
				return;
			}

			await SelectListItem(ItemData.UserEmail);

			lvSearchUsers.SelectedItem = null;
			lvSearchUsers.ItemSelected += OnUserIncludeTapped;
		}

		public TeamViewSearch(String searchText, Activity activity)
		{
			Title = "Search Results";
			ViewModel = ViewModelLocator.UsersViewModel;

			BindingContext = activity;
			#region Layout

			searchBar = new SearchBar
			{
				Placeholder = "Search",
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Xamarin.Forms.Color.White,
				Text = searchText,
			};

			lvSearchUsers = new TPListView();
			lvSearchUsers.ItemsSource = SearchedUsersList;
			lvSearchUsers.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell 
				{
					View = GetSelectedItemView(),
				};

				viewCell.Height = 70;
				return viewCell;
			});
			lvSearchUsers.ItemSelected += OnUserIncludeTapped;

			ScrollView scrollView = new ScrollView
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				IsClippedToBounds = true,
				Content = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0,
					Padding = 0,
					Children = {
						lvSearchUsers,
					}
				}
			};

			backButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Back to My Team",
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
				BorderWidth = 0,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};

			backButton.Clicked += (object sender, EventArgs e) => {
				if (!ViewModel.IsSearchPage) { return; }
				ViewModel.IsSearchPage = false;
				Navigation.PopAsync();
			};

			// Build the page.
			this.Content = new StackLayout
			{				
				Padding = new Thickness(0, 0, 0, 5),
				Spacing = 0,
				BackgroundColor = Xamarin.Forms.Color.White,
				Children = 
                {
					new StackLayout{
						Padding = 0,
						Spacing = 5,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = TalentPlus.Shared.Helpers.Color.UniLeverMidGray.ToFormsColor(),
						HeightRequest = 60,
						Children = {
							new StackLayout{
								Padding = 5,
								Spacing = 0,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								VerticalOptions = LayoutOptions.FillAndExpand,
								Children = {
									searchBar,
								}
							}
						}
					},
					new Label{
						HorizontalOptions = LayoutOptions.FillAndExpand,
						HeightRequest = 40,
						Text = "   Your Search Results",
						BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
						TextColor = Helpers.Color.Black.ToFormsColor(),
						YAlign = TextAlignment.Center,
					},
		            scrollView,
					new StackLayout{
						Spacing = 0,
						Padding = new Thickness(10, 0, 10, 0),
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.EndAndExpand,
						HeightRequest = 50,
						Children = {
							backButton
						}
					}
				}
			};

			#endregion
			searchBar.SearchButtonPressed += (object sender, EventArgs e) => {
				ReAlignUserLayout();
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			ViewModel.IsSearchPage = true;

			ViewModel.LoadItemsCommand.Execute(null);

			ReAlignUserLayout ();
		}

		private async void ReAlignUserLayout()
		{
			SearchedUsersList.Clear();

			String userOptionImage = "chevron.png";

			var email = searchBar.Text;

			List<UserSuggestion> searchedMemberList = await UserHelper.SearchUsers (email);
			foreach (UserSuggestion user in searchedMemberList) {
				SearchedUsersList.Add(new TeamUserInfo(user.Email, user.UserImage, user.Name, userOptionImage));
			}
		}

		private StackLayout GetSelectedItemView()
		{			
			var MainItemView = new StackLayout {
				Padding = 5,
				Spacing = 5,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 60,
			};

			var UserImageView = new TPCircleImage {
				WidthRequest = 60,
				HeightRequest = 60,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			UserImageView.SetBinding (Image.SourceProperty, "UserImage");

			var UserNameLabel = new Label {
				TextColor = Xamarin.Forms.Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
			};
			UserNameLabel.SetBinding (Label.TextProperty, "UserName");

			var UserOptionImage = new DarkIceImage {
				WidthRequest = 40,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
			};
			UserOptionImage.SetBinding (Image.SourceProperty, "UserOption");
			UserOptionImage.SetBinding (DarkIceImage.TagInfoProperty, "UserEmail");

			MainItemView.Children.Add (UserImageView);
			MainItemView.Children.Add (new StackLayout{
				Padding = 0,
				Spacing = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Orientation = StackOrientation.Vertical,
				Children = {
					UserNameLabel
				}
			});
			MainItemView.Children.Add (UserOptionImage);

			return MainItemView;
		}

		private async Task SelectListItem(String info)
		{
			TeamUserInfo userInfo = SearchedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();

			LoadingViewFlag = true;
			HideBackButtonFlag = true;
			var isSuccessfullyAdded = await Helpers.UserHelper.AddTeamUserByEmail(info);
			LoadingViewFlag = false;

			if (isSuccessfullyAdded != null && isSuccessfullyAdded == AddUserToTeamResult.Added)
			{
				try
				{
					ViewModel.LoadItemsCommand.Execute(null);
					var searchedMember = ViewModel.GetTeamMemberFromEmail(info);

					await DisplayAlert("Success", userInfo.UserName + " has been added to your team", "OK");
					SearchedUsersList.Remove(userInfo);
				}
				catch (Exception ex)
				{
					DisplayAlert("Failure", "Failed to add a colleague to the activity", "OK");
				}
			}
			else
			{
				await DisplayAlert("Failure", "Failed to add a colleague to the activity", "OK");
			}

			lvSearchUsers.ItemsSource = SearchedUsersList;
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			ViewModel.IsSearchPage = false;
		}
	}
}
