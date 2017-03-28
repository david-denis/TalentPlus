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
	public class WhoView : BaseView
	{
		class TeamUserInfo : INotifyPropertyChanged
		{
			#region INotifyPropertyChanged implementation

			public event PropertyChangedEventHandler PropertyChanged;

			#endregion

			public String UserEmail { get; set; }
			public String UserImage { get; set; }
			public String UserName { get; set; }
			public String UserRole { get; set; }

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

			public TeamUserInfo(String userEmail, String userImage, String userName, String userRole, String userOption)
			{
				UserEmail = userEmail;
				UserImage = userImage;
				UserName = userName;
				UserRole = userRole;
				UserOption = userOption;
			}

			protected virtual void OnPropertyChanged(string propertyName)
			{
				if (PropertyChanged == null)
					return;

				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private WhoViewSearch _whoViewSearch = null;
		/// <summary>
		/// Fake UserList
		/// </summary>
		private ObservableCollection<TeamUserInfo> NeverEngagedUsersList
		{
			get { return neverEngageduserList; }
			set { neverEngageduserList = value; OnPropertyChanged("NeverEngagedUsersList"); }
		}
		private ObservableCollection<TeamUserInfo> neverEngageduserList = new ObservableCollection<TeamUserInfo>();

		private ObservableCollection<TeamUserInfo> OneWeekEngagedUsersList
		{
			get { return oneWeekEngageduserList; }
			set { oneWeekEngageduserList = value; OnPropertyChanged("OneWeekEngagedUsersList"); }
		}
		private ObservableCollection<TeamUserInfo> oneWeekEngageduserList = new ObservableCollection<TeamUserInfo>();

		private ObservableCollection<TeamUserInfo> OneDayEngagedUsersList
		{
			get { return oneDayEngageduserList; }
			set { oneDayEngageduserList = value; OnPropertyChanged("OneDayEngagedUsersList"); }
		}
		private ObservableCollection<TeamUserInfo> oneDayEngageduserList = new ObservableCollection<TeamUserInfo>();

		private StackLayout NeverStackLayout;
		private StackLayout OneWeekStackLayout;
		private StackLayout OneDayStackLayout;

		private TPListView NeverlistUsers;
		private TPListView OneWeeklistUsers;
		private TPListView OneDaylistUsers;

		private TPListView _selectedUsersList;

		private Label NeverEngagedLabel;
		private Label OneWeekEngagedLabel;
		private Label OneDayEngagedLabel;

		/// <summary>
		/// Fake UserList
		/// </summary>
		public ObservableCollection<User> SelectedUsers
		{
			get { return selectedUsers; }
			set { selectedUsers = value; OnPropertyChanged("SelectedUsers"); }
		}
		private ObservableCollection<User> selectedUsers = new ObservableCollection<User>();

		private UsersViewModel ViewModel;
		private WhenViewModel WhenModel;
		private double sliderVal;

		private StackLayout SelectedUserLayout;

		private Button continueButton;
		private SearchBar searchBar;

		public WhoView(Activity activity, double sliderVal)
		{
			Title = "Pick who to join you";
			ViewModel = ViewModelLocator.UsersViewModel;
			this.sliderVal = sliderVal;

			BindingContext = activity;
			(BindingContext as Activity).SelectedUsers = new ObservableCollection<User>();

			BuildUI ();
		}

		private void BuildUI()
		{
			#region Layout

			searchBar = new SearchBar
			{
				Placeholder = "Search",
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Xamarin.Forms.Color.White,
			};

			SelectedUserLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = 5,
				Spacing = 10,
				HorizontalOptions = LayoutOptions.Fill,
				HeightRequest = 50,
			};

			for (int i = 0; i < 5; i++)
			{
				SelectedUserLayout.Children.Add(
					new TPCircleImage{
						WidthRequest = 40,
						HeightRequest = 40,
						Source = "temp_user.png",
						Aspect = Aspect.AspectFit,
					}
				);
			}

			var SelectedUsersScrollView = new ScrollView{
				Orientation = ScrollOrientation.Horizontal,
				Content = SelectedUserLayout,
				HorizontalOptions = LayoutOptions.Fill,
				IsClippedToBounds = true,
			};

			// SELECTED USERS BAR
			_selectedUsersList = new TPListView();

			var selectedUsersCell = new DataTemplate(typeof(RotatedListImageCell));
			//selectedUsersCell.SetBinding(Image.SourceProperty, "UserImage");
			_selectedUsersList.ItemTemplate = selectedUsersCell;
			_selectedUsersList.ItemsSource = ViewModel.TeamMembers;

			NeverEngagedLabel = new Label{
				Text = "Never Engaged",
				TextColor = Xamarin.Forms.Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
			};

			OneWeekEngagedLabel = new Label{
				Text = "Engaged One Week Ago",
				TextColor = Xamarin.Forms.Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
			};

			OneDayEngagedLabel = new Label{
				Text = "Engaged One Day Ago",
				TextColor = Xamarin.Forms.Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
			};

			NeverlistUsers = new TPListView();
			NeverlistUsers.ItemsSource = NeverEngagedUsersList;
			NeverlistUsers.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell {
					View = GetSelectedItemView(),
				};

				viewCell.Height = 70;
				return viewCell;
			});
			NeverlistUsers.ItemSelected += OnUserIncludeTapped;

			NeverStackLayout = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Padding = 0,
				Children = {
					NeverlistUsers
				}
			};

			OneWeeklistUsers = new TPListView();
			OneWeeklistUsers.ItemsSource = OneWeekEngagedUsersList;
			OneWeeklistUsers.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell {
					View = GetSelectedItemView(),
				};

				viewCell.Height = 70;
				return viewCell;
			});
			OneWeeklistUsers.ItemSelected += OnUserIncludeTapped;

			OneWeekStackLayout = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Padding = 0,
				Children = {
					OneWeeklistUsers
				}
			};

			OneDaylistUsers = new TPListView();
			OneDaylistUsers.ItemsSource = OneDayEngagedUsersList;
			OneDaylistUsers.ItemTemplate = new DataTemplate (() => {
				var viewCell = new ViewCell {
					View = GetSelectedItemView(),
				};

				viewCell.Height = 70;
				return viewCell;
			});
			OneDaylistUsers.ItemSelected += OnUserIncludeTapped;

			OneDayStackLayout = new StackLayout{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Padding = 0,
				Children = {
					OneDaylistUsers
				}
			};

			CustomScrollView scrollView = new CustomScrollView
			{
				HorizontalOptions = LayoutOptions.Fill,
				//VerticalOptions = LayoutOptions.Fill,
				Orientation = ScrollOrientation.Vertical,
				IsClippedToBounds = true,
				Content = new StackLayout{
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0,
					Padding = 0,
					Children = {
						new StackLayout{
							Spacing = 0,
							Padding = new Thickness(10, 0, 10, 0),
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
							Children = {
								NeverEngagedLabel,
							}
						},
						NeverStackLayout,
						new StackLayout{
							Spacing = 0,
							Padding = new Thickness(10, 0, 10, 0),
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
							Children = {
								OneWeekEngagedLabel,
							}
						},
						OneWeekStackLayout,
						new StackLayout{
							Spacing = 0,
							Padding = new Thickness(10, 0, 10, 0),
							Orientation = StackOrientation.Horizontal,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
							Children = {
								OneDayEngagedLabel,
							}
						},
						OneDayStackLayout,
					}
				}
			};

			continueButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Continue",
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
				BorderWidth = 0,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 10,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
			};

			continueButton.Clicked += OnContinueClicked;

			string minimumText = "";
			int requiredPeople = (BindingContext as Activity).RequiredPeople;
			if (requiredPeople <= 1)
				minimumText = "";
			else
				minimumText = "Please choose a minimum of " + (requiredPeople - 1) + " colleagues";

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
					new StackLayout{
						Padding = 0,
						Spacing = 5,
						BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
						Orientation = StackOrientation.Horizontal,
						//VerticalOptions = LayoutOptions.FillAndExpand,
						HeightRequest = 40,
						Children = {
							new Label{
								BackgroundColor = Xamarin.Forms.Color.Transparent,
								WidthRequest = 80,
								TextColor = TalentPlus.Shared.Helpers.Color.Black.ToFormsColor(),
								Text = "My Team",
								XAlign = TextAlignment.Center,
								YAlign = TextAlignment.Center,
								VerticalOptions = LayoutOptions.FillAndExpand,
								HorizontalOptions = LayoutOptions.StartAndExpand,
							},
							SelectedUsersScrollView,
						}
					},
					new Label{
						HorizontalOptions = LayoutOptions.FillAndExpand,
						HeightRequest = 40,
						Text = minimumText,
						TextColor = Helpers.Color.UniLeverDarkGray.ToFormsColor(),
						XAlign = TextAlignment.Center,
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
							continueButton
						}
					}
				}
				};

			#endregion

			searchBar.SearchButtonPressed += OnSearchClicked;

		}


		public void ClearAllViews()
		{
			if (_selectedUsersList != null) {
				_selectedUsersList.MakeDisposed = true;
				_selectedUsersList = null;
			}

			if (NeverlistUsers != null) {
				NeverlistUsers.MakeDisposed = true;
				NeverlistUsers.ItemSelected -= OnUserIncludeTapped;
				NeverlistUsers = null;
			}

			if (OneDaylistUsers != null) {
				OneDaylistUsers.MakeDisposed = true;
				OneDaylistUsers.ItemSelected -= OnUserIncludeTapped;
				OneDaylistUsers = null;
			}

			if (OneWeeklistUsers != null) {
				OneWeeklistUsers.MakeDisposed = true;
				OneWeeklistUsers.ItemSelected -= OnUserIncludeTapped;
				OneWeeklistUsers = null;
			}
		}

		private async void OnUserIncludeTapped(object sender, SelectedItemChangedEventArgs e) {
			RemoveUserIncludeTapListeners();
			if (e.SelectedItem == null) { AddUserIncludeTapListeners(); return; }

			var ItemData = e.SelectedItem as TeamUserInfo;
			if (ItemData == null) { AddUserIncludeTapListeners(); return; }

			await SelectListItem(ItemData.UserEmail);

			NeverlistUsers.SelectedItem = null;
			OneDaylistUsers.SelectedItem = null;
			OneWeeklistUsers.SelectedItem = null;

			AddUserIncludeTapListeners();
		}

		private void RemoveUserIncludeTapListeners() {
			NeverlistUsers.ItemSelected -= OnUserIncludeTapped;
			OneDaylistUsers.ItemSelected -= OnUserIncludeTapped;
			OneWeeklistUsers.ItemSelected -= OnUserIncludeTapped;
		}

		private void AddUserIncludeTapListeners()
		{
			NeverlistUsers.ItemSelected += OnUserIncludeTapped;
			OneDaylistUsers.ItemSelected += OnUserIncludeTapped;
			OneWeeklistUsers.ItemSelected += OnUserIncludeTapped;
		}

		async void OnSearchClicked(object sender, EventArgs e)
		{
			searchBar.SearchButtonPressed -= OnSearchClicked;
			_whoViewSearch = new WhoViewSearch (searchBar.Text, SelectedUsers, BindingContext as Activity, this);
			await Navigation.PushAsync(_whoViewSearch);
			searchBar.SearchButtonPressed += OnSearchClicked;
		}

		private async void OnValidate(object sender, EventArgs e)
		{
			WhenModel = new WhenViewModel(BindingContext as Activity);
			WhenModel.LoadItemsCommand.Execute (null);

			await Validate ();
		}

		private async Task Validate()
		{
			LoadingViewFlag = true;
			await WhenModel.ValidateSlider(sliderVal);

			ClearAllViews ();
			LoadingViewFlag = false;
			await Navigation.PopToRootAsync();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			ViewModel.LoadItemsCommand.Execute(null);
			ReAlignUserLayout ();

			ViewModel.BusyStateChanged += (object sender, EventArgs e) => {
				ReAlignUserLayout();
			};

			ChangeContinueStatus ();

			if (SelectedUsers != null)
			{
				for (int i = 0; i < SelectedUsers.Count; i++)
				{
					AddUserToSelectedView (SelectedUsers [i]);
				}
			}
		}

		private void ReAlignUserLayout()
		{
			NeverEngagedUsersList.Clear();
			OneWeekEngagedUsersList.Clear ();
			OneDayEngagedUsersList.Clear ();

			int totalDays = 0;
			String userOptionImage = "";
			foreach (TeamMember member in ViewModel.TeamMembers)
			{
				if (member.TeamUser == null)
					continue;
				
				var selUserList = SelectedUsers.Where (x => x.Email.Equals (member.TeamUser.Email)).FirstOrDefault ();
				if (selUserList != null)
					userOptionImage = "cross_icon.png";
				else
					userOptionImage = "arrow_right_icon.png";

				int type = 0;
				if (member.LastInteraction == null) {
					type = 0;
				} else {
					if (member.LastInteraction.Value < DateTime.Now) {
						totalDays = (int)Math.Ceiling((DateTime.Now - member.LastInteraction.Value).TotalDays);
						if (totalDays == 0)
							totalDays = 1;

						if (totalDays >= 7)
							type = 1;
						else
							type = 2;
					}
				}

				if (type == 0)
					NeverEngagedUsersList.Add(new TeamUserInfo(member.TeamUser.Email, member.TeamUser.UserImage, member.TeamUser.Name, member.TeamUser.Role, userOptionImage));
				else if (type == 1)
					OneWeekEngagedUsersList.Add(new TeamUserInfo(member.TeamUser.Email, member.TeamUser.UserImage, member.TeamUser.Name, member.TeamUser.Role, userOptionImage));
				else
					OneDayEngagedUsersList.Add(new TeamUserInfo(member.TeamUser.Email, member.TeamUser.UserImage, member.TeamUser.Name, member.TeamUser.Role, userOptionImage));
			}

			if (NeverEngagedUsersList.Count <= 0) {
				NeverEngagedLabel.IsVisible = false;
				NeverStackLayout.HeightRequest = 0;
			} else {
				NeverEngagedLabel.IsVisible = true;
				NeverStackLayout.HeightRequest = 60 * NeverEngagedUsersList.Count;
			}

			if (OneWeekEngagedUsersList.Count <= 0) {
				OneWeekEngagedLabel.IsVisible = false;
				OneWeekStackLayout.HeightRequest = 0;
			} else {
				OneWeekEngagedLabel.IsVisible = true;
				OneWeekStackLayout.HeightRequest = 60 * OneWeekEngagedUsersList.Count;
			}

			if (OneDayEngagedUsersList.Count <= 0) {
				OneDayEngagedLabel.IsVisible = false;
				OneDayStackLayout.HeightRequest = 0;
			} else {
				OneDayEngagedLabel.IsVisible = true;
				OneDayStackLayout.HeightRequest = 60 * OneDayEngagedUsersList.Count;
			}
		}

        private async void OnContinueClicked(object sender, EventArgs e)
        {
			continueButton.Clicked -= OnContinueClicked;
            //ParentView.MoveToNextPage(2);
			Activity activity = BindingContext as Activity;
			if (activity != null) {
				if (activity.RequiredPeople == 1 && SelectedUsers.Count <= 0)
				{
					OnValidate(null, null);
				}
				else
				{
					_whoViewSearch = null;

					var sendMsgView = new SendMessageView(BindingContext as Activity, sliderVal, this);
					await Navigation.PushAsync(sendMsgView);
				}
			}
			continueButton.Clicked += OnContinueClicked;
        }

		private async void AddUserToSelectedList(User user)
		{
			if (SelectedUsers.Contains(user))
			{
				await DisplayAlert("Failure", "Colleague already selected!", "OK");
				return;
			}
			else
			{
				SelectedUsers.Add(user);

				if ((BindingContext as Activity).SelectedUsers == null)
				{
					(BindingContext as Activity).SelectedUsers = new ObservableCollection<User>();
				}
                (BindingContext as Activity).SelectedUsers.Add(user);

				ChangeContinueStatus ();
			}

			AddUserToSelectedView (user);
		}

		private void AddUserToSelectedView(User user)
		{
			var SelectedUserImage = new TPCircleImage {
				Source = user.UserImage,
				WidthRequest = 40,
				HeightRequest = 40,
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var stack = new RelativeLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = 0,
				WidthRequest = 40,
				HeightRequest = 40,
			};

			stack.Children.Add(SelectedUserImage,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return parent.Width;
					}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return parent.Height;
					}));

			var addLayotutButton = new DarkIceImage {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				TagInfo = user.Email,
			};
			bool userInfoClicked = false;
			addLayotutButton.TappedWithInfo += async (String tag) => {
				if (userInfoClicked) { return; }
				userInfoClicked = true;
				#if __ANDROID__
				int id = -1;
				List<User> userList = SelectedUsers.Where(u => u.Email.Equals(tag)).ToList();
				if (userList != null && userList.Count > 0)
				{
					await DisplayAlert("Info", "User Name: " + userList[0].Name + "\n" + "User Email: " + userList[0].Email, "OK");
				}
				#else
				bool bResult = await DisplayAlert("Remove User", "Do you want to remove this user?", "Yes", "No");
				if (bResult == false)
				return;
				#endif
				userInfoClicked = false;
			};

			stack.Children.Add(addLayotutButton,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.Constant(0),
				widthConstraint: Constraint.RelativeToParent((parent) =>
					{
						return parent.Width;
					}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
					{
						return parent.Height;
					}));

			if (SelectedUsers != null && SelectedUsers.Count <= 5 && SelectedUsers.Count > 0) {
				SelectedUserLayout.Children.RemoveAt (SelectedUsers.Count - 1);
				SelectedUserLayout.Children.Insert (SelectedUsers.Count - 1, stack);
			}
			else
				SelectedUserLayout.Children.Add (stack);
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
				HeightRequest = 30,
				YAlign = TextAlignment.Center,
			};
			UserNameLabel.SetBinding (Label.TextProperty, "UserName");

			var UserRoleLabel = new Label {
				TextColor = Xamarin.Forms.Color.Gray,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				HeightRequest = 30,
				YAlign = TextAlignment.Center,
			};
			UserRoleLabel.SetBinding (Label.TextProperty, "UserRole");

			var UserOptionImage = new DarkIceImage {
				WidthRequest = 20,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.End,
				FilterColor = TalentPlus.Shared.Helpers.Color.Primary.ToFormsColor()
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
					UserNameLabel,
					UserRoleLabel
				}
			});
			MainItemView.Children.Add (UserOptionImage);

			return MainItemView;
		}

		private void ChangeContinueStatus()
		{
			int requiredPeople = (BindingContext as Activity).RequiredPeople - 1;
			if (SelectedUsers != null && SelectedUsers.Count >= requiredPeople) {
				continueButton.IsEnabled = true;
				continueButton.BackgroundColor = Helpers.Color.Primary.ToFormsColor ();
			} else {
				continueButton.IsEnabled = false;
				continueButton.BackgroundColor = Helpers.Color.UniLeverDarkGray.ToFormsColor ();
			}
		}

		private async Task SelectListItem(String info)
		{
				User selectedUserinfo = SelectedUsers.Where(x => x.Email.Equals(info)).FirstOrDefault();
				if (selectedUserinfo != null)
				{
					bool bResult = await DisplayAlert("Remove User", "Do you want to remove this user?", "Yes", "No");
					if (bResult == false)
						return;

					int id = -1;
					List<User> curSelecteduserList = SelectedUsers.Where(u => u.Email.Equals(info)).ToList();
					id = SelectedUsers.IndexOf(curSelecteduserList[0]);

					if (id >= 0)
					{
						SelectedUsers.RemoveAt (id);
						SelectedUserLayout.Children.RemoveAt (id);

						if (SelectedUsers.Count < 5)
						{
							SelectedUserLayout.Children.Add(
								new TPCircleImage{
									WidthRequest = 40,
									HeightRequest = 40,
									Source = "temp_user.png",
									Aspect = Aspect.AspectFit,
								}
							);
						}

						(BindingContext as Activity).SelectedUsers.Remove(curSelecteduserList[0]);
					}

					ChangeContinueStatus ();

					var userInfo = NeverEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "arrow_right_icon.png";
					}

					userInfo = OneWeekEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "arrow_right_icon.png";
					}

					userInfo = OneDayEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "arrow_right_icon.png";
					}
				}
				else{
					var teamMember = ViewModel.TeamMembers.Where(x => x.TeamUser.Email.Equals(info)).FirstOrDefault();

					if (teamMember != null)
					{
						AddUserToSelectedList(teamMember.TeamUser);
					}

					var userInfo = NeverEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "cross_icon.png";
					}

					userInfo = OneWeekEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "cross_icon.png";
					}

					userInfo = OneDayEngagedUsersList.Where(x => x.UserEmail.Equals(info)).FirstOrDefault();
					if (userInfo != null)
					{
						userInfo.UserOption = "cross_icon.png";
					}
				}

				NeverlistUsers.ItemsSource = NeverEngagedUsersList;
				OneWeeklistUsers.ItemsSource = OneWeekEngagedUsersList;
				OneDaylistUsers.ItemsSource = OneDayEngagedUsersList;
		}
	}
}
