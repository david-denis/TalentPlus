using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Xamarin;
using System.Reflection;

namespace TalentPlus.Shared
{
	public class TeamView : BaseView, IDisposable
	{
		private const string EXPAND_IMAGE = "arrow_right_icon.png";
		private const string COLLAPSE_IMAGE = "cross_icon.png";

		private const double IMAGE_SIZE = 20.0f;

		private UsersViewModel ViewModel
		{
			get;
			set;
		}

		//private TPListView listNeverUsers = new TPListView();

        private ObservableCollection<TeamMember> NeverEngagedMembers = new ObservableCollection<TeamMember>();
        private ObservableCollection<TeamMember> OneWeekEngagedMembers = new ObservableCollection<TeamMember>();
        private ObservableCollection<TeamMember> OneDayEngagedMembers = new ObservableCollection<TeamMember>();

        private ObservableCollection<SelectedActivity> SelectedActivities = new ObservableCollection<SelectedActivity>();

		private ObservableCollection<string> NotExpendableMember = new ObservableCollection<string>();

        private Dictionary<String, String> ExpandHelperList = new Dictionary<String, String>();

        private StackLayout NeverStackLayout;
        private StackLayout OneWeekStackLayout;
        private StackLayout OneDayStackLayout;

        private AbsoluteLayout NeverEngagedStack;
        private AbsoluteLayout OneWeekEngagedStack;
        private AbsoluteLayout OneDayEngagedStack;

		public TeamView ()
		{
			Title = "My Team";

			ViewModel = ViewModelLocator.UsersViewModel;

			ViewModel.BusyStateChanged += SetUpPages;
		}

		~TeamView()
		{
			Dispose();
		}

		public void Dispose()
		{
			ViewModel.BusyStateChanged -= SetUpPages;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (ViewModel == null)
				return;

			if ((ViewModel.CanLoadMore || ViewModel.TeamMembers == null || ViewModel.TeamMembers.Count == 0) && !ViewModel.IsBusy)
				ViewModel.LoadItemsCommand.Execute(null);

			//LoadDataAndUI ();

			/*
			ViewModel.BusyStateChanged += (object sender, EventArgs e) => {
				LoadDataAndUI();
			};*/
        }

		private void SetUpPages(object sender, EventArgs e)
		{
			if (ViewModel.IsBusy)
			{
				return;
			}
			LoadDataAndUI();
		}

        private void LoadDataAndUI()
        {
			var selectedActivities = ViewModelLocator.ActivitiesViewModel.SelectedActivities;
            foreach (SelectedActivity activity in selectedActivities)
            {
                SelectedActivities.Add(activity);
            }            

            NeverEngagedMembers.Clear();
            OneWeekEngagedMembers.Clear();
            OneDayEngagedMembers.Clear();

            // Get Branches
            foreach (TeamMember member in ViewModel.TeamMembers)
            {
                if (member.LastInteraction == null || member.LastInteraction.HasValue == false)
                    NeverEngagedMembers.Add(member);
                else
                {
                    double days = (DateTime.Now - member.LastInteraction.Value).TotalDays;
                    if (days < 7)
                        OneDayEngagedMembers.Add(member);
                    else
                        OneWeekEngagedMembers.Add(member);
                }
            }

            BuildUI();
        }

        private async void BuildUI()
        {
            bool isExistData = false;

			SearchBar searchBar = new SearchBar
			{
				Placeholder = "Search",
				VerticalOptions = LayoutOptions.Center,
				BackgroundColor = Xamarin.Forms.Color.White,
			};

			searchBar.SearchButtonPressed += async (sender, args) =>
			{
				await Navigation.PushAsync(new TeamViewSearch(searchBar.Text, BindingContext as Activity));
			};

            StackLayout MainStack = new StackLayout
            {
				Padding = new Thickness(0, 0, 0, 5),
				Spacing = 0,
				BackgroundColor = Xamarin.Forms.Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            
			MainStack.Children.Add (new StackLayout{
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
			});

            // Add Never Engaged Layout
            if (NeverEngagedMembers != null && NeverEngagedMembers.Count > 0)
            {
                isExistData = true;
                var NeverEngagedLabel = new Label
                {
                    TextColor = Helpers.Color.Black.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
					FontAttributes = Xamarin.Forms.FontAttributes.Bold,
                    Text = "Never Engaged",
                };
				MainStack.Children.Add(new StackLayout{
					Spacing = 0,
					Padding = new Thickness(10, 0, 10, 0),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
					Children = {
						NeverEngagedLabel,
					}
				});

                NeverEngagedStack = new AbsoluteLayout
                {
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

				//var NeverBackImage = new Image
				//{
				//	Source = "activity_status_bg",
				//	Aspect = Aspect.Fill,
				//};

				var NeverBackImage = new RoundedBox
				{
					BackgroundColor = Helpers.Color.White.ToFormsColor(),
					CornerRadius = 10,
					RoundedSide = RoundedBox.RoundedSideType.All
				};

                AbsoluteLayout.SetLayoutFlags(NeverBackImage, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NeverBackImage, new Rectangle(0, 0, 1, 1));
                NeverEngagedStack.Children.Add(NeverBackImage);

                NeverStackLayout = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0
                };

                foreach (TeamMember member in NeverEngagedMembers)
                {
                    String plusId = "";

					StackLayout activityLayout = new StackLayout
					{
						Padding = new Thickness(0, 5),
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
					};

					var ArchiveActivities = await TalentPlus.Shared.Helpers.ActivityHelper.GetLatestActivityArchiveByUser(member.TeamUserId);
					// Fake Data
					//ArchiveActivities.Add(new ActivityArchive() { Activity = new Activity { ShortDescription = "aaa" }, Feedback = 3, FinishTime = new DateTime(2015, 3, 3), });

					if (ArchiveActivities != null && ArchiveActivities.Count > 0)
					{
						foreach (ActivityArchive activity in ArchiveActivities)
						{
							var ActivityLayout = GetActivityStack(member, activity);
							activityLayout.Children.Add(ActivityLayout);
							activityLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
						}
					}
					else
					{
						NotExpendableMember.Add(member.TeamUserId);
					}

                    var MemberLayout = GetMemberLayout(member, out plusId);
                    NeverStackLayout.Children.Add(MemberLayout);


					if (member != NeverEngagedMembers[NeverEngagedMembers.Count - 1])
					{
						NeverStackLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
					}

                    NeverStackLayout.Children.Add(activityLayout);
                    activityLayout.IsVisible = false;

					ExpandHelperList.Add(plusId, activityLayout.Id.ToString());
                }

                AbsoluteLayout.SetLayoutFlags(NeverStackLayout, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(NeverStackLayout, new Rectangle(0, 0, 1, 1));
                NeverEngagedStack.Children.Add(NeverStackLayout);

                MainStack.Children.Add(NeverEngagedStack);

				//NeverEngagedStack.HeightRequest = 60 * NeverEngagedMembers.Count;
            }

            // Add One Week Engaged Layout
            if (OneWeekEngagedMembers != null && OneWeekEngagedMembers.Count > 0)
            {
                isExistData = true;
                var OneWeekEngagedLabel = new Label
                {
					TextColor = Helpers.Color.Black.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
					FontAttributes = Xamarin.Forms.FontAttributes.Bold,
					Text = "Engaged 1 week ago",
                };
				MainStack.Children.Add(new StackLayout{
					Spacing = 0,
					Padding = new Thickness(10, 0, 10, 0),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
					Children = {
						OneWeekEngagedLabel,
					}
				});

                OneWeekEngagedStack = new AbsoluteLayout
                {
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

				//var OneWeekBackImage = new Image
				//{
				//	Source = "activity_status_bg",
				//	Aspect = Aspect.Fill,
				//};

				var OneWeekBackImage = new RoundedBox
				{
					BackgroundColor = Helpers.Color.White.ToFormsColor(),
					CornerRadius = 10,
					RoundedSide = RoundedBox.RoundedSideType.All
				};

                AbsoluteLayout.SetLayoutFlags(OneWeekBackImage, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(OneWeekBackImage, new Rectangle(0, 0, 1, 1));
                OneWeekEngagedStack.Children.Add(OneWeekBackImage);

                OneWeekStackLayout = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0
                };

                foreach (TeamMember member in OneWeekEngagedMembers)
                {
                    String plusId = "";

					StackLayout activityLayout = new StackLayout
					{
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
					};

					var ArchiveActivities = await TalentPlus.Shared.Helpers.ActivityHelper.GetLatestActivityArchiveByUser(member.TeamUserId);

					var currentActivitiesWithUser = ViewModel.GetCurrentActivitiesWithUser(member.TeamUserId);
					foreach (ActivityArchive currentActivity in currentActivitiesWithUser)
					{
						ArchiveActivities.Add(currentActivity);
					}

					// Fake Data
					//ArchiveActivities.Add(new ActivityArchive() { Activity = new Activity { ShortDescription = "aaa" }, Feedback = 3, FinishTime = new DateTime(2015, 3, 3), });

					if (ArchiveActivities != null && ArchiveActivities.Count > 0)
					{
						foreach (ActivityArchive activity in ArchiveActivities)
						{
							var ActivityLayout = GetActivityStack(member, activity);
							activityLayout.Children.Add(ActivityLayout);
							activityLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
						}
					}
					else
					{
						NotExpendableMember.Add(member.TeamUserId);
					}

                    var MemberLayout = GetMemberLayout(member, out plusId);
                    OneWeekStackLayout.Children.Add(MemberLayout);

					if (member != OneWeekEngagedMembers[OneWeekEngagedMembers.Count - 1])
					{
						OneWeekStackLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
					}

                    OneWeekStackLayout.Children.Add(activityLayout);
                    activityLayout.IsVisible = false;

                    ExpandHelperList.Add(plusId, activityLayout.Id.ToString());
                }

                AbsoluteLayout.SetLayoutFlags(OneWeekStackLayout, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(OneWeekStackLayout, new Rectangle(0, 0, 1, 1));
                OneWeekEngagedStack.Children.Add(OneWeekStackLayout);

                MainStack.Children.Add(OneWeekEngagedStack);

				//OneWeekEngagedStack.HeightRequest = 60 * OneWeekEngagedMembers.Count;
            }

            // Add One Day Engaged Layout
            if (OneDayEngagedMembers != null && OneDayEngagedMembers.Count > 0)
            {
                isExistData = true;
                var OneDayEngagedLabel = new Label
                {
					TextColor = Helpers.Color.Black.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
					FontAttributes = Xamarin.Forms.FontAttributes.Bold,
					Text = "Engaged 1 day ago",
                };
				MainStack.Children.Add(new StackLayout{
					Spacing = 0,
					Padding = new Thickness(10, 0, 10, 0),
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
					Children = {
						OneDayEngagedLabel,
					}
				});

                OneDayEngagedStack = new AbsoluteLayout
                {
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

				//var OneDayBackImage = new Image
				//{
				//	Source = "activity_status_bg",
				//	Aspect = Aspect.Fill,
				//};

				var OneDayBackImage = new RoundedBox
				{
					BackgroundColor = Helpers.Color.White.ToFormsColor(),
					CornerRadius = 10,
					RoundedSide = RoundedBox.RoundedSideType.All,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand
				};

                AbsoluteLayout.SetLayoutFlags(OneDayBackImage, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(OneDayBackImage, new Rectangle(0, 0, 1, 1));
                OneDayEngagedStack.Children.Add(OneDayBackImage);

                OneDayStackLayout = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
					Spacing = 0
                };

                foreach (TeamMember member in OneDayEngagedMembers)
                {
                    String plusId = "";

					StackLayout activityLayout = new StackLayout
					{
						Padding = 0,
						Spacing = 0,
						HorizontalOptions = LayoutOptions.FillAndExpand,
					};

					var ArchiveActivities = await TalentPlus.Shared.Helpers.ActivityHelper.GetLatestActivityArchiveByUser(member.TeamUserId);

					var currentActivitiesWithUser = ViewModel.GetCurrentActivitiesWithUser(member.TeamUserId);
					foreach (ActivityArchive currentActivity in currentActivitiesWithUser)
					{
						ArchiveActivities.Add(currentActivity);
					}

					// Fake Data
					//ArchiveActivities.Add(new ActivityArchive() { Activity = new Activity { ShortDescription = "aaa" }, Feedback = 3, FinishTime = new DateTime(2015, 3, 3), });

					if (ArchiveActivities != null && ArchiveActivities.Count > 0)
					{
						foreach (ActivityArchive activity in ArchiveActivities)
						{
							var ActivityLayout = GetActivityStack(member, activity);
							activityLayout.Children.Add(ActivityLayout);
							activityLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
						}
					}
					else
					{
						NotExpendableMember.Add(member.TeamUserId);
					}

                    var MemberLayout = GetMemberLayout(member, out plusId);
                    OneDayStackLayout.Children.Add(MemberLayout);

					if (member != OneDayEngagedMembers[OneDayEngagedMembers.Count - 1])
					{
						OneDayStackLayout.Children.Add(new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, Color = Helpers.Color.Gray.ToFormsColor() });
					}

                    OneDayStackLayout.Children.Add(activityLayout);
                    activityLayout.IsVisible = false;

                    ExpandHelperList.Add(plusId, activityLayout.Id.ToString());
                }

                AbsoluteLayout.SetLayoutFlags(OneDayStackLayout, AbsoluteLayoutFlags.All);
                AbsoluteLayout.SetLayoutBounds(OneDayStackLayout, new Rectangle(0, 0, 1, 1));
                OneDayEngagedStack.Children.Add(OneDayStackLayout);

                MainStack.Children.Add(OneDayEngagedStack);

				//OneDayEngagedStack.HeightRequest = 60 * OneDayEngagedMembers.Count;
            }
            
            if (isExistData == false)
            {
                var NoMemberLabel = new UnileverLabel
                {
                    Text = "No members",
					TextColor = Helpers.Color.Primary.ToFormsColor(),
                    XAlign = TextAlignment.Center,
                    YAlign = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)),
                };

                MainStack.Children.Add(NoMemberLabel);
            }

            // Build the page.
            this.Content = new ScrollView { Content = MainStack };
        }

        private StackLayout GetMemberLayout(TeamMember member, out String plusId)
        {
            StackLayout MemberLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 60,
                Padding = 5,
                Spacing = 5,
            };

            var UserImage = new TPCircleImage
            {
                Source = member.TeamUser.UserImage,
                WidthRequest = 50,
				Aspect = Aspect.AspectFit,
            };
            MemberLayout.Children.Add(UserImage);

            var NameStack = new StackLayout{
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 0
            };

            var UserName = new Label {
                Text = member.TeamUser.Name,
                TextColor = Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				//HeightRequest = 25,
                YAlign = TextAlignment.Center,
            };
            NameStack.Children.Add(UserName);

            var UserRole = new Label
            {
                Text = member.TeamUser.Role,
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
				//HeightRequest = 25,
                YAlign = TextAlignment.Center,
            };
            NameStack.Children.Add(UserRole);
            MemberLayout.Children.Add(NameStack);

            //var ExpandButton = new Button//new TalentPlus.Shared.Helpers.DarkIceImage
            //{
            //    Image = "small_icons_chevron_down_green",
            //    WidthRequest = 40,
            //    HorizontalOptions = LayoutOptions.End,
            //    VerticalOptions = LayoutOptions.FillAndExpand,
            //    BorderColor = Color.Transparent,
            //    BackgroundColor = Color.Transparent,
            //    BorderRadius = 0,
            //    BorderWidth = 0,
            //    //Aspect = Aspect.AspectFit,                
            //};

            //var ExpandButton = new SvgColorIcon
            //{
            //    SvgPath = "plus_icon.svg",
            //    SvgPathForSelectedState = "cross_icon.svg",
            //    SvgAssembly = typeof(TalentPlusApp).GetTypeInfo().Assembly,
            //    HorizontalOptions = LayoutOptions.Center,
            //    VerticalOptions = LayoutOptions.Center,
            //    BackgroundColor = Xamarin.Forms.Color.Transparent,
            //    HeightRequest = 40,
            //    WidthRequest = 40,
            //    IconColor = Helpers.Color.Primary.ToIntColor(),
            //};

            var ExpandButton = new TalentPlus.Shared.Helpers.DarkIceImage
            {
                Source = EXPAND_IMAGE,
				WidthRequest = IMAGE_SIZE,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
				FilterColor = Helpers.Color.Primary.ToFormsColor(),
            };

			//ExpandButton.TagInfo = ExpandButton.Id.ToString();
			//ExpandButton.TappedWithInfo += (id) =>

            ExpandButton.Tapped = () =>
            {
                ExpandButton_Clicked(ExpandButton, null);
            };

            //ExpandButton.Clicked += (sender, e1) =>
            //{
            //    ExpandButton_Clicked(sender, e1);
            //};

			if (!NotExpendableMember.Contains(member.TeamUserId))
			{
				MemberLayout.Children.Add(ExpandButton);
			}

			plusId = ExpandButton.Id.ToString();

            return MemberLayout;
        }
        
        private StackLayout GetActivityStack(TeamMember member, ActivityArchive activity)
        {
            StackLayout ActivityLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,                
                HeightRequest = 60,
                Padding = new Thickness(10, 0, 10, 0),
                Spacing = 0,
				BackgroundColor = Helpers.Color.Gray.ToFormsColor(),
            };

            var ActivityImage = new Image
            {
                Source = "activity_icon",
                WidthRequest = 50,
                HeightRequest = 30,
                Aspect = Aspect.AspectFit,
                VerticalOptions = LayoutOptions.Center,
            };
            ActivityLayout.Children.Add(ActivityImage);

            var NameStack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 0
            };

            var ActivityName = new Label
            {
                Text = activity.Activity.ShortDescription,
				TextColor = Helpers.Color.GreenNotif.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				//HeightRequest = 25,
                YAlign = TextAlignment.Center,
            };
            NameStack.Children.Add(ActivityName);

            var status = "Current";
            int totalDays = 0;
            if (activity.FinishTime < DateTime.Now)
            {
                totalDays = (int)Math.Ceiling((DateTime.Now - activity.FinishTime).TotalDays);
                if (totalDays == 0)
                    totalDays = 1;

                if (totalDays > 1)
                    status = totalDays + " days ago";
                else
                    status = totalDays + " day ago";
            }

            var StatusLabel = new Label
            {
                Text = status,
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				//HeightRequest = 25,
                YAlign = TextAlignment.Center,
            };
            NameStack.Children.Add(StatusLabel);
            ActivityLayout.Children.Add(NameStack);

            var RatingBarCont = new TalentPlus.Shared.Helpers.RatingBarControl(20, false, (int)activity.Feedback, 5)
            {
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Orientation =  StackOrientation.Horizontal,
            };
            ActivityLayout.Children.Add(RatingBarCont);

            return ActivityLayout;
        }

        private void ExpandButton_Clicked(object sender, EventArgs e1)
        {
			var expandButton = sender as TalentPlus.Shared.Helpers.DarkIceImage;
            String id = expandButton.Id.ToString();
            String stackId = "";
            try
            {
                stackId = ExpandHelperList.Where(a => a.Key.Equals(id)).FirstOrDefault().Value;
            }
            catch (Exception ex)
            {
				Insights.Report(ex, new Dictionary<string, string>
				{
					{ "Where", "TeamView.ExpandButton_Clicked()" }
				});
            }

            if (String.IsNullOrEmpty(stackId))
                return;

            if (OneDayStackLayout != null && OneDayEngagedMembers != null && OneDayEngagedMembers.Count > 0)
            {
                foreach (View view in OneDayStackLayout.Children)
                {
                    var idValue = view.Id.ToString();
                    if (idValue.Equals(stackId))
                    {
                        StackLayout layout = view as StackLayout;
                        if (layout.IsVisible == true)
                        {
                            layout.IsVisible = false;
                            //expandButton.Selected = true;
							expandButton.Source = EXPAND_IMAGE;
							//OneDayEngagedStack.HeightRequest = 60 * OneDayEngagedMembers.Count;
                        }
                        else
                        {
                            layout.IsVisible = true;
							expandButton.Source = COLLAPSE_IMAGE;
                            //expandButton.Image = "small_icons_up_chevron_green";
							//OneDayEngagedStack.HeightRequest = 60 * (OneDayEngagedMembers.Count + layout.Children.Count);
                        }
                    }
                }
            }

            if (OneWeekStackLayout != null && OneWeekEngagedMembers != null && OneWeekEngagedMembers.Count > 0)
            {
                foreach (View view in OneWeekStackLayout.Children)
                {
                    var idValue = view.Id.ToString();
                    if (idValue.Equals(stackId))
                    {
                        StackLayout layout = view as StackLayout;
                        if (layout.IsVisible == true)
                        {
                            layout.IsVisible = false;
							expandButton.Source = EXPAND_IMAGE;
                            //expandButton.Image = "small_icons_chevron_down_green";
							//OneWeekEngagedStack.HeightRequest = 60 * OneWeekEngagedMembers.Count;
                        }
                        else
                        {
                            layout.IsVisible = true;
							expandButton.Source = COLLAPSE_IMAGE;
                            //expandButton.Image = "small_icons_up_chevron_green";
							//OneWeekEngagedStack.HeightRequest = 60 * (OneWeekEngagedMembers.Count + layout.Children.Count);
                        }
                    }
                }
            }

            if (NeverStackLayout != null && NeverEngagedMembers != null && NeverEngagedMembers.Count > 0)
            {
                foreach (View view in NeverStackLayout.Children)
                {
                    var idValue = view.Id.ToString();
                    if (idValue.Equals(stackId))
                    {
                        StackLayout layout = view as StackLayout;
                        if (layout.IsVisible == true)
                        {
                            layout.IsVisible = false;
							expandButton.Source = EXPAND_IMAGE;
                            //expandButton.Image = "small_icons_chevron_down_green";
							//NeverEngagedStack.HeightRequest = 60 * NeverEngagedMembers.Count;
                        }
                        else
                        {
                            layout.IsVisible = true;
							expandButton.Source = COLLAPSE_IMAGE;
                            //expandButton.Image = "small_icons_up_chevron_green";
							//NeverEngagedStack.HeightRequest = 60 * (NeverEngagedMembers.Count + layout.Children.Count);
                        }
                    }
                }
            }
        }
	}
}
