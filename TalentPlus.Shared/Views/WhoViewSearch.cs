using System;
using System.Collections.ObjectModel;
using Xamarin;
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
	public class WhoViewSearch : BaseView
	{
		class TeamUserInfo : INotifyPropertyChanged
		{
			#region INotifyPropertyChanged implementation

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion

			public string UserEmail { get; set; }

			public string UserImage { get; set; }

			public string UserName { get; set; }

			private string userOption;

			public string UserOption {
				get {
					return userOption;
				}
				set {
					userOption = value;
					//OnPropertyChanged ("UserOption");
				}
			}

			public TeamUserInfo (string userEmail, string userImage, string userName, string userOption)
			{
				UserEmail = userEmail;
				UserImage = userImage;
				UserName = userName;
				UserOption = userOption;
			}

			protected virtual void OnPropertyChanged (string propertyName)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
			}
		}

		/// <summary>
		/// Fake UserList
		/// </summary>
		private ObservableCollection<TeamUserInfo> SearchedUsersList 
		{
			get { return searcheduserList; }

			set 
			{
				searcheduserList = value;
				//OnPropertyChanged ("SearchedUsersList");
			}
		}

		private ObservableCollection<TeamUserInfo> searcheduserList = new ObservableCollection<TeamUserInfo> ();
		private ObservableCollection<User> selectedUsers = new ObservableCollection<User> ();

		private UsersViewModel ViewModel;

		private StackLayout _layoutSelectedUser;

		private WhoView _viewParentWho;

		private Button _btnBack;
		private SearchBar _searchBar;
		private TPListView _lvSearchUsers;

		/// <summary>
		/// Fake UserList
		/// </summary>
		public ObservableCollection<User> SelectedUsers 
		{
			get { return selectedUsers; }
		
			set 
			{
				selectedUsers = value;
				//OnPropertyChanged ("SelectedUsers");
			}
		}



		public WhoViewSearch (String searchText, ObservableCollection<User> whoviewSelUsers, Activity activity, WhoView parentView)
		{
			Title = "Search Results";
			_viewParentWho = parentView;
			ViewModel = ViewModelLocator.UsersViewModel;

			BindingContext = activity;

			SelectedUsers = whoviewSelUsers;

			//SelectedUsers = new ObservableCollection<User> ();

			//for (int i = 0; i < whoviewSelUsers.Count; i++) {
			//	SelectedUsers.Add (whoviewSelUsers [i]);
			//}

			BuildUI (searchText);
		}

		private async void OnUserIncludeTapped (object sender, SelectedItemChangedEventArgs e)
		{
			_lvSearchUsers.ItemSelected -= OnUserIncludeTapped;
			if (e.SelectedItem == null) {
				_lvSearchUsers.ItemSelected += OnUserIncludeTapped;
				return;
			}

			var ItemData = e.SelectedItem as TeamUserInfo;
			if (ItemData == null) {
				_lvSearchUsers.ItemSelected += OnUserIncludeTapped;
				return;
			}

			await SelectListItem (ItemData.UserEmail);

			_lvSearchUsers.SelectedItem = null;
			_lvSearchUsers.ItemSelected += OnUserIncludeTapped;
		}

		private void BuildUI (String searchText)
		{
			#region Layout

			_searchBar = new SearchBar {
				Placeholder = "Search",
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Xamarin.Forms.Color.White,
				Text = searchText,
			};

			_layoutSelectedUser = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = 5,
				Spacing = 10,
				HorizontalOptions = LayoutOptions.Fill,
				HeightRequest = 50,
			};

			for (int i = 0; i < 5; i++) {
				_layoutSelectedUser.Children.Add (
					new TPCircleImage {
						WidthRequest = 40,
						HeightRequest = 40,
						Source = "temp_user.png",
						Aspect = Aspect.AspectFit,
					}
				);
			}

			var scrollSelectedUsers = new ScrollView {
				Orientation = ScrollOrientation.Horizontal,
				Content = _layoutSelectedUser,
				HorizontalOptions = LayoutOptions.Fill,
				IsClippedToBounds = true,
			};

			// SELECTED USERS BAR
			var _lstSelectedUsers = new TPListView ();

			var selectedUsersCell = new DataTemplate (typeof(RotatedListImageCell));
			_lstSelectedUsers.ItemTemplate = selectedUsersCell;
			_lstSelectedUsers.ItemsSource = ViewModel.TeamMembers;

			_lvSearchUsers = new TPListView ();
			_lvSearchUsers.ItemsSource = SearchedUsersList;
			_lvSearchUsers.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell {
					View = GetSelectedItemView (),
				};

				viewCell.Height = 70;
				return viewCell;
			});
			_lvSearchUsers.ItemSelected += OnUserIncludeTapped;

			ScrollView scrollView       = new ScrollView {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Fill,
				IsClippedToBounds = true,
				Content = new StackLayout {
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0,
					Padding = 0,
					Children = {
						_lvSearchUsers,
					}
				}
			};

			_btnBack = new Button {
				BackgroundColor = Helpers.Color.Primary.ToFormsColor (),
				Text = "Back to My Team",
				TextColor = Helpers.Color.White.ToFormsColor (),
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Button)),
				BorderWidth = 0,
				BorderColor = Helpers.Color.Primary.ToFormsColor (),
				BorderRadius = 10,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};

			_btnBack.Clicked += (object sender, EventArgs e) => {
				
				_lvSearchUsers.ItemsSource = null;
				_lstSelectedUsers.ItemsSource = null;

				_lvSearchUsers.ItemSelected -= OnUserIncludeTapped;

				_lvSearchUsers.MakeDisposed = true;
				_lstSelectedUsers.MakeDisposed = true;


				//this.Content = null;
				scrollView = null;
				_lstSelectedUsers = null;
				_layoutSelectedUser= null;
				_lvSearchUsers = null;
				Navigation.PopAsync ();
			};

//			this.Content = _btnBack;
//			return;

			// Build the page.
			this.Content = new StackLayout {				
				Padding = new Thickness (0, 0, 0, 5),
				Spacing = 0,
				BackgroundColor = Xamarin.Forms.Color.White,
				Children = {
					new StackLayout {
						Padding = 0,
						Spacing = 5,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = TalentPlus.Shared.Helpers.Color.UniLeverMidGray.ToFormsColor (),
						HeightRequest = 60,
						Children = {
							new StackLayout {
								Padding = 5,
								Spacing = 0,
								HorizontalOptions = LayoutOptions.FillAndExpand,
								VerticalOptions = LayoutOptions.FillAndExpand,
								Children = {
									_searchBar,
								}
							}
						}
					},
					new StackLayout {
						Padding = 0,
						Spacing = 5,
						BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor (),
						Orientation = StackOrientation.Horizontal,
						//VerticalOptions = LayoutOptions.FillAndExpand,
						HeightRequest = 40,
						Children = {
							new Label {
								BackgroundColor = Xamarin.Forms.Color.Transparent,
								WidthRequest = 80,
								TextColor = TalentPlus.Shared.Helpers.Color.Black.ToFormsColor (),
								Text = "My Team",
								XAlign = TextAlignment.Center,
								YAlign = TextAlignment.Center,
								VerticalOptions = LayoutOptions.FillAndExpand,
								HorizontalOptions = LayoutOptions.StartAndExpand,
							},
							scrollSelectedUsers,
						}
					},
					new Label {
						HorizontalOptions = LayoutOptions.FillAndExpand,
						HeightRequest = 40,
						Text = "   Your Search Results",
						BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor (),
						TextColor = Helpers.Color.Black.ToFormsColor (),
						YAlign = TextAlignment.Center,
					},
					scrollView,
					new StackLayout {
						Spacing = 0,
						Padding = new Thickness (10, 0, 10, 0),
						Orientation = StackOrientation.Horizontal,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.EndAndExpand,
						HeightRequest = 50,
						Children = {
							_btnBack
						}
					}
				}
			};

			#endregion
//			searchBar.SearchButtonPressed += async (object sender, EventArgs e) => {
			_searchBar.SearchButtonPressed += (object sender, EventArgs e) => {
				ReAlignUserLayout ();
			};

			_lvSearchUsers.IsPullToRefreshEnabled = false;
			_lstSelectedUsers.IsPullToRefreshEnabled = false;
			_lstSelectedUsers.GroupHeaderTemplate = null;
			_lvSearchUsers.GroupHeaderTemplate = null;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();

			ViewModel.LoadItemsCommand.Execute (null);

			ReAlignUserLayout ();

			if (SelectedUsers != null) {
				for (int i = 0; i < SelectedUsers.Count; i++) {
					AddUserToSelectedView (SelectedUsers [i]);
				}
			}
		}

		private async void ReAlignUserLayout ()
		{
			SearchedUsersList.Clear ();

			String userOptionImage = "arrow_right_icon.png";

			var email = _searchBar.Text;

			List<UserSuggestion> searchedMemberList = await UserHelper.SearchUsers (email);
			foreach (UserSuggestion user in searchedMemberList) {
				var selectedUser = SelectedUsers.Where (u => u.Email.Equals (user.Email)).ToList ();
				if (selectedUser != null && selectedUser.Count > 0) {
					userOptionImage = "cross_icon.png";
				}

				SearchedUsersList.Add (new TeamUserInfo (user.Email, user.UserImage, user.Name, userOptionImage));
			}
		}

		private async void AddUserToSelectedList (User user)
		{
			if (SelectedUsers.Contains (user)) {
				await DisplayAlert ("Failure", "Colleague already selected!", "OK");
				return;
			} else {
				//await DisplayAlert("Success", "Colleague successfully added to the activity", "OK");
				SelectedUsers.Add (user);

				if ((BindingContext as Activity).SelectedUsers == null) {
					(BindingContext as Activity).SelectedUsers = new ObservableCollection<User> ();
				}
				(BindingContext as Activity).SelectedUsers.Add (user);
			}

			AddUserToSelectedView (user);
		}

		private void AddUserToSelectedView (User user)
		{
			var SelectedUserImage = new TPCircleImage {
				Source = user.UserImage,
				WidthRequest = 40,
				HeightRequest = 40,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var stack = new RelativeLayout {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = 0,
				WidthRequest = 40,
				HeightRequest = 40,
			};

			stack.Children.Add (SelectedUserImage,
				xConstraint: Constraint.Constant (0),
				yConstraint: Constraint.Constant (0),
				widthConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				heightConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Height;
				}));

			var addLayotutButton = new DarkIceImage {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				TagInfo = user.Email,
			};

			addLayotutButton.TappedWithInfo += async (String tag) => {
				#if __ANDROID__
				int id = -1;
				List<User> userList = SelectedUsers.Where (u => u.Email.Equals (tag)).ToList ();
				if (userList != null && userList.Count > 0) {
					await DisplayAlert ("Info", "User Name: " + userList [0].Name + "\n" + "User Email: " + userList [0].Email, "OK");
				}
				#else
				bool bResult = await DisplayAlert("Remove User", "Do you want to remove this user?", "Yes", "No");
				if (bResult == false)
				return;
				#endif
			};

			stack.Children.Add (addLayotutButton,
				xConstraint: Constraint.Constant (0),
				yConstraint: Constraint.Constant (0),
				widthConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				heightConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Height;
				}));

			if (SelectedUsers != null && SelectedUsers.Count <= 5 && SelectedUsers.Count > 0) {
				_layoutSelectedUser.Children.RemoveAt (SelectedUsers.Count - 1);
				_layoutSelectedUser.Children.Insert (SelectedUsers.Count - 1, stack);
			} else
				_layoutSelectedUser.Children.Add (stack);
		}

		private StackLayout GetSelectedItemView ()
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
				FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(Label)),
				VerticalOptions = LayoutOptions.FillAndExpand,
				YAlign = TextAlignment.Center,
			};
			UserNameLabel.SetBinding (Label.TextProperty, "UserName");

			var UserOptionImage = new DarkIceImage {
				WidthRequest = 40,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
				FilterColor = Helpers.Color.Primary.ToFormsColor (),
			};
			UserOptionImage.SetBinding (Image.SourceProperty, "UserOption");
			UserOptionImage.SetBinding (DarkIceImage.TagInfoProperty, "UserEmail");

			MainItemView.Children.Add (UserImageView);
			MainItemView.Children.Add (new StackLayout {
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

		private async Task SelectListItem (String info)
		{
			User selectedUserinfo = SelectedUsers.Where (x => x.Email.Equals (info)).FirstOrDefault ();
			if (selectedUserinfo != null) {
				bool bResult = await DisplayAlert ("Remove User", "Do you want to remove this user?", "Yes", "No");
				if (bResult == false)
					return;

				int id = -1;
				List<User> curSelecteduserList = SelectedUsers.Where (u => u.Email.Equals (info)).ToList ();
				id = SelectedUsers.IndexOf (curSelecteduserList [0]);

				if (id >= 0) {
					SelectedUsers.RemoveAt (id);
					_layoutSelectedUser.Children.RemoveAt (id);

					if (SelectedUsers.Count < 5) {
						_layoutSelectedUser.Children.Add (
							new TPCircleImage {
								WidthRequest = 40,
								HeightRequest = 40,
								Source = "temp_user.png",
								Aspect = Aspect.AspectFit,
							}
						);
					}

					(BindingContext as Activity).SelectedUsers.Remove (curSelecteduserList [0]);
				}

				TeamUserInfo userInfo = SearchedUsersList.Where (x => x.UserEmail.Equals (info)).FirstOrDefault ();
				if (userInfo != null) {
					userInfo.UserOption = "arrow_right_icon.png";
				}
			} else {
				TeamUserInfo userInfo = SearchedUsersList.Where (x => x.UserEmail.Equals (info)).FirstOrDefault ();

				LoadingViewFlag = true;
				var isSuccessfullyAdded = await Helpers.UserHelper.AddTeamUserByEmail (info);
				LoadingViewFlag = false;

				if (isSuccessfullyAdded != null && isSuccessfullyAdded == AddUserToTeamResult.Added)
				{
					try {
						ViewModel.LoadItemsCommand.Execute (null);
						var searchedMember = ViewModel.GetTeamMemberFromEmail (info);
						if (searchedMember != null) {
							AddUserToSelectedList (searchedMember.TeamUser);
						}
					} catch (Exception ex) {
						DisplayAlert ("Failure", "Failed to add a colleague to the activity", "OK");
						Insights.Report (ex, new Dictionary<string, string> {
							{ "WhoViewSearch.cs", "WhoViewSearch.SelectListItem()" }
						});
					}
				} else {
					await DisplayAlert ("Failure", "Failed to add a colleague to the activity", "OK");
				}
			}

			_lvSearchUsers.ItemsSource = SearchedUsersList;
			ReAlignUserLayout();
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
			_viewParentWho.SelectedUsers = this.SelectedUsers;
		}
	}
}
