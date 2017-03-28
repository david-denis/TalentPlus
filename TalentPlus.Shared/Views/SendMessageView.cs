using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using System.Collections.Generic;

namespace TalentPlus.Shared
{
	public class SendMessageView : BaseView
	{
		/// <summary>
		/// Fake UserList
		/// </summary>
		public ObservableCollection<User> UsersList
		{
			get { return userList; }
			set { userList = value; OnPropertyChanged("UsersList"); }
		}

		private ObservableCollection<User> userList = new ObservableCollection<User>();

		private WhenViewModel WhenModel;
		private double sliderVal;
		//WrappedTruncatedLabel recipients;
		private ScrollView recipients;
		private StackLayout SelectedUserLayout;

		private PlaceholderEditor SayText;
		private CustomEntry SubjectEntry;
		private WhoView _whoView;
        
		public SendMessageView(Activity activity, double sliderVal, WhoView whoView)
		{
			_whoView = whoView;
			Title = "Invite to Your Team";
			BackgroundColor = Helpers.Color.White.ToFormsColor();
			BindingContext = activity;
			this.sliderVal = sliderVal;

			#region Layout
			SelectedUserLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(10, 5, 10, 5),
				Spacing = 5,
				HorizontalOptions = LayoutOptions.Fill,
				HeightRequest = 40,
			};

			recipients = new ScrollView
			{
				Orientation = ScrollOrientation.Horizontal,
				Content = SelectedUserLayout,
				HorizontalOptions = LayoutOptions.Fill,
				IsClippedToBounds = true,
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
			};

			SayText = new PlaceholderEditor
			{
				Text = "",
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Placeholder = "Say something",
			};

			var editorStack = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = 0,
				Spacing = 0,
				Children = 
				{
					SayText
				}
			};

			SubjectEntry = new CustomEntry{
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Fill,
				Text = activity.ShortDescription,
				YAlign = TextAlignment.Center,
			};

            var MainStack = new StackLayout
			{
				Padding = 0,
				Spacing = 5,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = 
                {
					recipients,
					new StackLayout{
						Orientation = StackOrientation.Horizontal,
						HeightRequest = 35,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						VerticalOptions = LayoutOptions.Start,
						Padding = new Thickness(10, 5, 10, 5),
						Spacing = 2,
						Children = {
							new UnileverLabel{
								Text = "Subject:",
								TextColor = Helpers.Color.Gray.ToFormsColor(),
								FontSize = Device.OnPlatform<int>(16, 14, 14),
								YAlign = TextAlignment.Center,
								WidthRequest = 80,
								HorizontalOptions = LayoutOptions.Start,
							},
							SubjectEntry,
						}
					},
					new BoxView{
						HeightRequest = 1,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
					},
					editorStack
				}
			};

			// Build the page.
			this.Content = MainStack;

			#endregion
			ToolbarItems.Add(new ToolbarItem("Send", "", sendButton_Clicked));

			ValidateUsers ();
		}

        private void ValidateUsers()
        {
            Activity activity = BindingContext as Activity;

            if (activity == null || activity.SelectedUsers != null)
            {
				var i = 0;
                foreach (User user in activity.SelectedUsers)
                {
					var text = "";

					if (i != activity.SelectedUsers.Count - 1)
					{
						text = user.Name + ", ";
					}
					else
					{
						text = user.Name;
					}

					Label fsItem = new UnileverLabel {
						TextColor = Helpers.Color.Gray.ToFormsColor(),
						FontSize = Device.OnPlatform<int>(16, 14, 14),
						YAlign = TextAlignment.Center,
					};
					fsItem.Text = text;

					SelectedUserLayout.Children.Add (fsItem);
					i++;
				}
            }


			SayText.Text = "I would like to do the '" + activity.ShortDescription + "' with you. Are you free on " + activity.SelectedTime.ToString("D") + "?";
        }

		private async void sendButton_Clicked()
		{
			if (LoadingViewFlag) { return; }
			LoadingViewFlag = true;
			WhenModel = new WhenViewModel(BindingContext as Activity);
			WhenModel.LoadItemsCommand.Execute (null);

			await Validate ();
		}

		private async Task Validate()
		{
			await WhenModel.ValidateSlider(sliderVal, SubjectEntry.Text, SayText.Text);

			TalentPlusApp.BackBlocked = true;

			_whoView.ClearAllViews ();
			await Navigation.PushModalAsync(new GetReadyView(BindingContext as Activity, this));


			//Navigation.PopToRootAsync();

			LoadingViewFlag = false;
			Helpers.Utility.RefreshActionBar();
		}


		public async Task ClearEverything()
		{
			await Navigation.PopToRootAsync ();
		}
	}
}
