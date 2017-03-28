using System;
using System.Collections.ObjectModel;
using TalentPlus.Shared.Helpers;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
	public class MeView : BaseView
    {
        class MeActivityItem
        {
            public String Title { get; set; }
            public String Status { get; set; }
            public int Rating { get; set; }
            public MeActivityItem(String title, String status, int rating)
            {
                this.Title = title;
                this.Status = status;
                this.Rating = rating;
            }
        }

		private ActivitiesViewModel ViewModel
		{
			get;
			set;
		}

		private int AttentionNotifications { get; set; }
		private int TotalNotifications { get; set; }

		private Label AttentionNotificationsLabel { get; set; }
		private Label TotalNotificationsLabel { get; set; }

        ObservableCollection<MeActivityItem> MeActivityList = new ObservableCollection<MeActivityItem>();

        AbsoluteLayout absoluteActivityLayout;
        TPListView listViewSelectedActivities;

		public MeView()
		{
			ViewModel = ViewModelLocator.ActivitiesViewModel;
			BackgroundColor = Helpers.Color.Gray.ToFormsColor();

			AttentionNotifications = ViewModel.PendingFeedbacks.Count;
			TotalNotifications = ViewModel.PendingFeedbacks.Count+ViewModel.SelectedActivities.Count;

			#region Layout

			// Main Stack
			var stack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 0
			};

			// Second Stack
			var stack2 = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 0,
				Padding = 10
			};

			// Top Grid
			//var designedTopLayout = new Image { Source = "profile_top_header.png", Aspect = Aspect.AspectFit, HorizontalOptions = LayoutOptions.StartAndExpand };

			var absoluteLayout = new AbsoluteLayout { Padding = new Thickness(0) };
			//var image = new Image { Source = "profile_top_header.png", Aspect = Aspect.Fill };
			var image = new RoundedBox
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				CornerRadius = 15,
				RoundedSide = RoundedBox.RoundedSideType.Top
			};

			Grid grid = new Grid
			{
				//BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) }
                }
			};

			var userImageSource = "user_icon.png";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty(TalentPlusApp.CurrentUser.UserImage))
				userImageSource = TalentPlusApp.CurrentUser.UserImage;

			grid.Children.Add(new StackLayout
			{
				Children =
					{
						new TPCircleImage{Source = userImageSource, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 60, WidthRequest = 60, Aspect = Aspect.AspectFill }
					}
			}, 0, 0);

			var userName = "Me";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty(TalentPlusApp.CurrentUser.Name))
				userName = TalentPlusApp.CurrentUser.Name;

			grid.Children.Add(new StackLayout
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children =
					{
						new UnileverLabel{Text = userName, FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, TextColor = Helpers.Color.White.ToFormsColor() },
						//new Label{Text = "Global Brand Lead", Font = Font.SystemFontOfSize(NamedSize.Small), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, TextColor = Helpers.Color.White.ToFormsColor() },
					}
			}, 1, 0);

			var stackLayoutProfile = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Padding = new Thickness(0, 10),
				Children =
					{
						grid
					}
			};

			absoluteLayout.Children.Add(image, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteLayout.Children.Add(stackLayoutProfile, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			stack2.Children.Add(absoluteLayout);
			//stack2.Children.Add(grid);

			// Stats Grid
			var absoluteStatsLayout = new AbsoluteLayout { Padding = new Thickness(0) };
			//var imageStats = new Image { Source = "profile_top_footer.png", Aspect = Aspect.Fill };
			var imageStats = new RoundedBox
			{
				BackgroundColor = Helpers.Color.White.ToFormsColor(),
				CornerRadius = 15,
				RoundedSide = RoundedBox.RoundedSideType.Bottom
			};

			Grid statGrid = new Grid
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
			};

			AttentionNotificationsLabel = new UnileverLabel
			{
				Text = ViewModel.ActivitiesCompletedPercent+"%",
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.5,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start 
			};

			statGrid.Children.Add(new StackLayout
				{
					Children =
					{
						AttentionNotificationsLabel,
						new Label{Text = "Completed overall", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Helpers.Color.Gray.ToFormsColor() }
					}
				}, 0, 0);

			TotalNotificationsLabel = new UnileverLabel
			{
				Text = ViewModel.ActivitiesCompletedInSixMonths+"",
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.5,
				HorizontalOptions = LayoutOptions.Center, 
				VerticalOptions = LayoutOptions.Start
			};

			statGrid.Children.Add(new StackLayout
			{
				Children =
					{
						TotalNotificationsLabel,
						new Label{Text = "Last 6 months", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Helpers.Color.Gray.ToFormsColor() }
					}
			}, 1, 0);

			var stackLayoutProfileStats = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				Padding = new Thickness(0, 15),
				Children =
					{
						statGrid
					}
			};

			absoluteStatsLayout.Children.Add(imageStats, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteStatsLayout.Children.Add(stackLayoutProfileStats, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			stack2.Children.Add(absoluteStatsLayout);

			// Selected Activities
			var about = new Label
			{
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				FontFamily = "Arial",
				Text = "Activity",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				HorizontalOptions = LayoutOptions.Start,
				LineBreakMode = LineBreakMode.WordWrap
			};

			var stackAbout = new StackLayout
			{
				Padding = new Thickness(5, 20, 0, 10),
				Children = { about }
			};
			stack2.Children.Add(stackAbout);

			listViewSelectedActivities = new TPListView();
            listViewSelectedActivities.RowHeight = 80;

			listViewSelectedActivities.BackgroundColor = Xamarin.Forms.Color.Transparent;
            listViewSelectedActivities.ItemsSource = MeActivityList;// ViewModel.SelectedActivities;
            /*
			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Activity.ShortDescription");
			cell.SetBinding(TextCell.DetailProperty, "Activity.FullDescription");
			listViewSelectedActivities.ItemTemplate = cell;*/

            var cell = new DataTemplate(() =>
            {
                // Return an assembled ViewCell.
                var viewCell = new ViewCell
                {
                    View = GetMeActivityItem()
                };

                viewCell.Height = 80;
                return viewCell;
            });

            listViewSelectedActivities.ItemTemplate = cell;

			absoluteActivityLayout = new AbsoluteLayout { Padding = new Thickness(0) };
			var activityimage = new Image { Source = "activity_status_bg.png", Aspect = Aspect.Fill };

			absoluteActivityLayout.Children.Add(activityimage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteActivityLayout.Children.Add(listViewSelectedActivities, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

            stack2.Children.Add(absoluteActivityLayout);
            if (MeActivityList == null || MeActivityList.Count <= 0)
                absoluteActivityLayout.HeightRequest = 0;

			stack.Children.Add(new ScrollView { VerticalOptions = LayoutOptions.FillAndExpand, Content = stack2 });

			Content = stack;

			#endregion

			listViewSelectedActivities.ItemTapped += (sender, args) =>
			{
				if (listViewSelectedActivities.SelectedItem == null)
					return;
				//ViewModel.SelectedActivities.Remove((SelectedActivity)listView.SelectedItem);
				//ViewModel.EmptiedActivities = false;
				listViewSelectedActivities.SelectedItem = null;
			};

			ViewModel.PendingFeedbacks.CollectionChanged += NotificationsChanged;
			ViewModel.SelectedActivities.CollectionChanged += NotificationsChanged;
		}

		void NotificationsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			//AttentionNotifications = ViewModel.PendingFeedback.Count;
			//TotalNotifications = ViewModel.PendingFeedback.Count + ViewModel.SelectedActivities.Count;

			//AttentionNotificationsLabel.Text = "" + AttentionNotifications;
			//TotalNotificationsLabel.Text = "" + TotalNotifications;
		}

		/// <summary>
		/// Push the page of the activty's feedback
		/// </summary>
		/// <param name="activity"></param>
		public void PushFeedback(Activity activity)
		{
			TalentPlusApp.PendingNotification = false;
			this.Navigation.PushAsync(new FeedBackView(activity, ViewModel.GetSelectedActivity(activity.Id)));
		}

		public void Refresh()
		{
			ViewModel.LoadItemsCommand.Execute(null);
		}

        private StackLayout GetMeActivityItem()
        {
            var ItemImage = new DarkIceImage
            {
                Source = "activity_icon",
                Aspect = Aspect.AspectFit,
                WidthRequest = 30,
                HeightRequest = 80,
            };

            var ItemTextLabel = new DILabel
            {
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                YAlign = TextAlignment.End,
                HeightRequest = 40,
                IsDefaultLabel = true,
				//Lines = 2,
				TextColor = Helpers.Color.GreenNotif.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(DILabel)),
				FontAttributes = FontAttributes.Bold
            };
            ItemTextLabel.SetBinding(DILabel.TextProperty, "Title");

            var ItemTextDetailLabel = new DILabel
            {
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                YAlign = TextAlignment.Start,
                IsDefaultLabel = true,
                HeightRequest = 40,
                Lines = 2,
				TextColor = Helpers.Color.Gray.ToFormsColor(),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(DILabel)),
            };
            ItemTextDetailLabel.SetBinding(DILabel.TextProperty, "Status");

            var RatingControl = new RatingBarControl(25, false, 0, 0)
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                WidthRequest = 120,
            };

            RatingControl.SetBinding(RatingBarControl.RatingBarProperty, "Rating");

            var StackItemLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Padding = 10,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 80,
                Children = {
                    ItemImage,
					new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = 0,
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Children = {
                            ItemTextLabel,
					        ItemTextDetailLabel,
				        }
                    },
                    RatingControl
				}
            };

            return StackItemLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MeActivityList.Clear();
            for (int i = 0; i < ViewModel.SelectedActivities.Count; i++)
            {
                var status = "Current";
                int totalDays = 0;
                if (ViewModel.SelectedActivities[i].FinishTime < DateTime.Now)
                {
                    totalDays = (int)Math.Ceiling((DateTime.Now - ViewModel.SelectedActivities[i].FinishTime).TotalDays);
                    if (totalDays > 1)
                        status = totalDays + " days ago";
                    else
                        status = totalDays + " day ago";
                }

                MeActivityList.Add(new MeActivityItem(ViewModel.SelectedActivities[i].Activity.ShortDescription, status, (int)((ViewModel.SelectedActivities[i].Activity.ActivityStatistics.EaseAverage + ViewModel.SelectedActivities[i].Activity.ActivityStatistics.EffectivenessAverage) / 2)));
            }

            for (int i = 0; i < ViewModel.ActivityArchives.Count; i++)
            {
                var status = "Current";
                int totalDays = 0;
                if (ViewModel.SelectedActivities[i].FinishTime < DateTime.Now)
                {
                    totalDays = (int)Math.Ceiling((DateTime.Now - ViewModel.SelectedActivities[i].FinishTime).TotalDays);
                    if (totalDays > 1)
                        status = totalDays + " days ago";
                    else
                        status = totalDays + " day ago";
                }

                MeActivityList.Add(new MeActivityItem(ViewModel.ActivityArchives[i].Activity.ShortDescription, status, (int)((ViewModel.ActivityArchives[i].Activity.ActivityStatistics.EaseAverage + ViewModel.ActivityArchives[i].Activity.ActivityStatistics.EffectivenessAverage) / 2)));
            }

            listViewSelectedActivities.ItemsSource = MeActivityList;

            if (MeActivityList == null || MeActivityList.Count <= 0)
                absoluteActivityLayout.HeightRequest = 0;
            else
                absoluteActivityLayout.HeightRequest = 80 * MeActivityList.Count;
        }
    }
}
