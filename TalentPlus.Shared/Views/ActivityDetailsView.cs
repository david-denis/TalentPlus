using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TalentPlus.Shared
{
    public class ActivityDetailsView : ContentPage, IDisposable
    {
        public const double USER_ICON_WIDTH = 50;
		public readonly double SCREEN_STANDARD_WIDTH = Device.OnPlatform (320, 360, 360);

        private ActivitiesViewModel ViewModel;

        private StackLayout stack2;

		private bool first;
		private double doActivityViewHeight = 0;

		public static bool IsSubscribed = false;
#if __ANDROID__

        public PopupLayout PopupPhoto;

#else
		
		public iOSPopupLayout PopupPhoto;

#endif

		~ActivityDetailsView ()
        {
			Dispose ();
        }

		public void Dispose ()
        {
            yesButton.Tapped -= OnYesClicked;
            noButton.Tapped -= OnNoClicked;
            MessagingCenter.Unsubscribe<string> (string.Empty, "ReloadActionBar");
        }

        private bool FeedbackPostsLoaded = false;

        private StackLayout StreamStack = null;
		private TalentPlus.Shared.Helpers.VideoPlayerView videoView = null;

        private WrappedTruncatedLabel labelFormattedTrunc;
        private Label labelFormattedFull;
		//private Grid topTipGrid;
		//private bool LastDisplayedLeft = false;

		private Dictionary<string, bool> DisplayedFeedbacks = new Dictionary<string, bool> ();

		private ObservableCollection<FeedbackPost> feedbackPosts = new ObservableCollection<FeedbackPost> ();

		private StackLayout RecentUsersStack = null;
        private Grid RecentImageStack = null;

		//private AbsoluteLayout absoluteTopTipLayout;
		private StackLayout stackTitleLayout;
		private StackLayout topTipStack;
		private StackLayout activityDoingStack;
		private StackLayout contentStack;
		private RelativeLayout stack;

		private TalentPlus.Shared.Helpers.RatingBarControl EffectRating;
		private TalentPlus.Shared.Helpers.RatingBarControl EaseRating;

        private double topTipStackHeight = 0;
        private bool IsTooltipHeightInitialized = false;
        private bool IsTooltipExpanded = false;

		public bool IsLoaded = false;

		private double yScrollPosition = 0;

        /// <summary>
        /// gets or sets the feed items
        /// </summary>
		private ObservableCollection<FeedbackPost> FeedbackPosts {
            get { return feedbackPosts; }
			set {
                feedbackPosts = value;
				OnPropertyChanged ("FeedbackPosts");
            }
        }

		private ObservableCollection<User> recentUsersList = new ObservableCollection<User> ();

		private ObservableCollection<User> RecentUsersList {
            get { return recentUsersList; }
			set {
                recentUsersList = value;
				OnPropertyChanged ("RecentUsersList");
            }
        }

        Activity Item;
        ClippedScrollView mainScrollView;
        TalentPlus.Shared.Helpers.CustomImageButton yesButton;
        TalentPlus.Shared.Helpers.CustomImageButton noButton;
        TalentPlus.Shared.Helpers.CustomImageButton laterButton;

		ActivitiesView ActivityParentView;

		public ActivityDetailsView (ActivitiesView parentView, Activity item, ActivitiesViewModel vm, bool preLoad = true)
        {
			ActivityParentView = parentView;

            ViewModel = vm;
            BindingContext = item;
			BackgroundColor = Helpers.Color.White.ToFormsColor ();

            Item = item;

			if (preLoad)
			{
				LoadView();
				//LoadFeedbackPosts();
			}

			first = true;
		}

		public void LoadContent()
		{
			this.LoadView();
			this.LoadFeedbackPosts();
			//mainScrollView.IsClippedToBounds = true;
		}

		public void UnloadContent()
		{
			this.Content = null;
		}

		public void LoadView()
		{
			if (IsLoaded)
			{
				if (this.Content == null)
				{
#if __IOS__
					this.Content = stack;
#else
					this.Content = contentStack;
#endif
				}
				return;
			}
			IsLoaded = true;

            #region Layout

            // Main Stack
			stack = new RelativeLayout {
                //Orientation = StackOrientation.Vertical
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0
            };

            // Stack for Activity Content
			stack2 = new StackLayout {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 15,
                Padding = 0
            };

            //======================= Title ============================

			this.BuildTitle();

			//======================= Description ============================

			this.BuildDescription();

			//======================= Action info ============================

			this.BuildInfos();

			//======================= Recent users ============================

			this.BuildRecentUsersStack();

			//======================= Ratings ============================

			this.BuildEaseOfUseRating();

			this.BuildEffectivenessRating();

			//======================= TopTip ============================

			this.BuildTopTip();

			//======================= Feedback ============================

			this.BuildStreamStack();

            //======================= Activity buttons ============================

			this.BuildActivityButtons();

            //======================= Add all stacks together ============================

#if __IOS__
			videoView = new TalentPlus.Shared.Helpers.VideoPlayerView {
				HeightRequest = 1,
				WidthRequest = 1,
				//BackgroundColor = Color.Transparent,
			};

			activityDoingStack.Children.Add (videoView);
#endif

			mainScrollView = new ClippedScrollView {
                Content = stack2,
                HorizontalOptions = LayoutOptions.Fill,
                Orientation = ScrollOrientation.Vertical,
				IsClippedToBounds = true
            };

			mainScrollView.Scrolled += (object sender, ScrolledEventArgs e) => 
			{
				if (e.ScrollY != 0) {
					yScrollPosition = e.ScrollY;
				}
			};

			contentStack = new StackLayout {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Padding = 0,
                Children = {
					stackTitleLayout,
					mainScrollView,
					#if __ANDROID__
					activityDoingStack
					#endif
				}
            };

			#if __IOS__
			stack.Children.Add (contentStack,
				xConstraint: Constraint.Constant (0),
				yConstraint: Constraint.Constant (0),
				widthConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Width;
				}),
				heightConstraint: Constraint.RelativeToParent ((parent) => {
					return parent.Height;
				}));


			stack.Children.Add(activityDoingStack,
				xConstraint: Constraint.Constant(0),
				yConstraint: Constraint.RelativeToParent((parent) =>
				{
					if (first) {
						doActivityViewHeight = parent.Height - 100;
						first = false;
					}
					return doActivityViewHeight;
				}),
				widthConstraint: Constraint.RelativeToParent((parent) =>
				{
					return parent.Width;
				}),
				heightConstraint: Constraint.RelativeToParent((parent) =>
				{
					return 100;
				}));
				#endif


			//#if __ANDROID__

//			PopupPhoto = new PopupLayout () {
//				Content = contentStack,
//			};

//#else

//			PopupPhoto = new iOSPopupLayout () {
//				Content = contentStack
//			};
					
//#endif

//			var device = XLabs.Ioc.Resolver.Resolve<XLabs.Platform.Device.IDevice> ();

//			PopupPhoto.WidthRequest = device.Display.Width;
//			PopupPhoto.HeightRequest = device.Display.Height;
		
#if __IOS__
			Content = stack;
#else
			Content = contentStack;
#endif
            #endregion
            
        }

		private void activityDoingStack_LayoutChanged (object sender, EventArgs e)
        {
			stack2.Padding = new Thickness (0, 5, 0, ((StackLayout)sender).Height + 10);
        }

		private void BuildTitle()
		{
			stackTitleLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Color.White,
				Spacing = 0,
				Padding = new Thickness(10, 10, 0, 10), //this is weird, where does the right padding come from?
			};

			var activityNavLeftImage = new Label
			{
				Text = "\u276e",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.3,
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				WidthRequest = 30,
				HorizontalOptions = LayoutOptions.StartAndExpand
			};
			stackTitleLayout.Children.Add(activityNavLeftImage);

			var NavlefttapGestureRecognizer = new TapGestureRecognizer();
			NavlefttapGestureRecognizer.Tapped += MoveToPrevPage;
			activityNavLeftImage.GestureRecognizers.Add(NavlefttapGestureRecognizer);

			var activityTitle = new UnileverLabel
			{
				FontSize = Convert.ToSingle(Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel))),
				Text = Item.ShortDescription,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				YAlign = TextAlignment.Center,
				XAlign = TextAlignment.Center,
				TextColor = Color.Black
			};
			stackTitleLayout.Children.Add(activityTitle);

			var activityNavRightImage = new Label
			{
				Text = "\u276f",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.3,
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				WidthRequest = 30,
				HorizontalOptions = LayoutOptions.EndAndExpand,
			};
			stackTitleLayout.Children.Add(activityNavRightImage);
			var NavRighttapGestureRecognizer = new TapGestureRecognizer();
			NavRighttapGestureRecognizer.Tapped += MoveToNextPage;
			activityNavRightImage.GestureRecognizers.Add(NavRighttapGestureRecognizer);
		}

		private void BuildDescription()
		{
			var activityDescription = new UnileverLabel
			{
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)) * 1.2,
				Text = Item.FullDescription,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				TextColor = Color.Gray,
			};

			var descriptionContainer = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 0),
				Children = {
						activityDescription
					}
			};

			stack2.Children.Add(descriptionContainer);
		}
		bool themeButtonClicked = false;
		private void BuildInfos()
		{
			var businessResourcesGrid = new FastGrid
			{
				VerticalOptions = LayoutOptions.End,
				HorizontalOptions = LayoutOptions.Center,
				ColumnDefinitions = {
					new ColumnDefinition ()
				},
				RowDefinitions = {
					new RowDefinition ()
				},
				Padding = 0,
				HeightRequest = 30
			};

			businessResourcesGrid.Children.Add(new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.End,
				Children = {
					new Image{ Source = "clock.png", Scale = 0.6 }, //Image to change (CLOCK)
					new Label {
						Text = Helpers.Utility.TimeSpanToText (TimeSpan.FromSeconds (Item.RequiredTime)),
						FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
						VerticalOptions = LayoutOptions.Center,
						TextColor = Color.Black
					}
				},
			}, 0, 0);

			var peopleString = " people";
			if (Item.RequiredPeople == 1) { peopleString = " person"; }

			businessResourcesGrid.Children.Add(new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Start,
				Children = {
					new Image{ Source = "person.png", Scale = 0.6 }, //Image to change (TEAM)
					new Label {
						Text = "" + Item.RequiredPeople + peopleString,
						FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
						VerticalOptions = LayoutOptions.Center,
						TextColor = Color.Black
					}
				}
			}, 1, 0);

			var themeShortDescriptionContainer = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 0,
				Children = {
					new Image{ Source = "star_nofilled.png", Scale = 0.6 }, //Image to change (TEAM)
					new Label {
						Text = Item.Theme.ShortDescription,
						FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
						VerticalOptions = LayoutOptions.Center,
						TextColor = Color.Black
					}
				}
			};

			var themeTapGestureRecognizer = new TapGestureRecognizer();
			themeTapGestureRecognizer.Tapped += async (s, e) =>
			{
				if (themeButtonClicked) { return; }
				themeButtonClicked = true;
				if (themeShortDescriptionContainer != null)
				{
#if __ANDROID__
					var exploreThemeView = new ExploreThemesView(ActivityParentView, ViewModel);
					await Navigation.PushAsync(exploreThemeView);
#else
					var homeView = this.Parent.Parent as TabbedIconPage;
					homeView.SwitchToThemes();
#endif
				}
				themeButtonClicked = false;
			};

			themeShortDescriptionContainer.GestureRecognizers.Add(themeTapGestureRecognizer);

			var infosStack = new StackLayout
			{
				Spacing = 5,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children = { businessResourcesGrid, themeShortDescriptionContainer },
				HeightRequest = 60
			};

			stack2.Children.Add(infosStack);
		}

		private void BuildRecentUsersStack()
		{
			RecentUsersStack = new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Fill,
				Spacing = 10,
				BackgroundColor = Color.FromHex("EFECF4"),
				Children = {
					new UnileverLabel {
						Text = "Recent", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.Black, HeightRequest = 50, XAlign = TextAlignment.Start, YAlign = TextAlignment.Center,
						#if __IOS__
						FontSize = Device.GetNamedSize (NamedSize.Small, typeof(UnileverLabel)),
						#endif
					}
				}
			};

			RecentImageStack = new FastGrid
			{
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				ColumnDefinitions = {
					new ColumnDefinition { Width = GridLength.Auto }
				},
				Padding = 0
			};
			RecentUsersStack.Children.Add(RecentImageStack);

			PopulateRecentUsersStack();

			stack2.Children.Add(RecentUsersStack);
		}

		private int EaseOfUseAverage()
		{
			int easeOfUserAverage = Convert.ToInt32(Math.Ceiling(Item.ActivityStatistics.EaseAverage));

			if (easeOfUserAverage <= 0 || easeOfUserAverage > 5)
			{
				easeOfUserAverage = 5;
			}

			return easeOfUserAverage;
		}

		private void BuildEaseOfUseRating()
		{
			EaseRating = new TalentPlus.Shared.Helpers.RatingBarControl(24, false, EaseOfUseAverage()) { Orientation = StackOrientation.Horizontal };
			/*Item.ActivityStatistics.EaseAverage*/

			var easeofUseStack = new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.Transparent,
				Children = {
					new UnileverLabel {
						Text = "Ease of use", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.Black, WidthRequest = 150, XAlign = TextAlignment.Start, YAlign = TextAlignment.Center,
						#if __IOS__
						FontSize = Device.GetNamedSize (NamedSize.Small, typeof(UnileverLabel)),
						#endif
					},
					EaseRating,
				}
			};

			stack2.Children.Add(easeofUseStack);
		}

		private int EffectivenessAverage()
		{
			int effectivenessAverage = Convert.ToInt32(Math.Ceiling(Item.ActivityStatistics.EffectivenessAverage));

			if (effectivenessAverage <= 0 || effectivenessAverage > 5)
			{
				effectivenessAverage = 5;
			}

			return effectivenessAverage;
		}

		private void BuildEffectivenessRating()
		{
			EffectRating = new TalentPlus.Shared.Helpers.RatingBarControl(24, false, EffectivenessAverage()) { Orientation = StackOrientation.Horizontal };

			var effectRatingStack = new StackLayout
			{
				Padding = new Thickness(10, 0),
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.Transparent,
				Children = {
					new UnileverLabel {
						Text = "Effectiveness", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center, TextColor = Color.Black, WidthRequest = 150, XAlign = TextAlignment.Start, YAlign = TextAlignment.Center,
						#if __IOS__
						FontSize = Device.GetNamedSize (NamedSize.Small, typeof(UnileverLabel)),
						#endif
					},
					EffectRating,
				}
			};

			stack2.Children.Add(effectRatingStack);
		}

		private void BuildTopTip()
		{
			StackLayout topStickStackContainer = null;

			if (!string.IsNullOrEmpty(Item.TopTip))
			{
				topTipStack = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Padding = 10,
					BackgroundColor = Helpers.Color.Secondary.ToFormsColor(),
				};

				FastGrid topTipGrid = new FastGrid
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					HorizontalOptions = LayoutOptions.Fill,
					Padding = 0,
					ColumnDefinitions = {
						new ColumnDefinition { Width = new GridLength (1, GridUnitType.Star) },
						new ColumnDefinition { Width = new GridLength (9, GridUnitType.Star) }
					}
				};

				topTipStack.Children.Add(topTipGrid);

				var tapGestureRecognizer = new TapGestureRecognizer
				{
					Command = new Command(() =>
					{ 
						if (topTipStack != null)
						{
							if (IsTooltipExpanded == false)
							{
								IsTooltipExpanded = true;
								labelFormattedTrunc.IsVisible = false;
								labelFormattedFull.IsVisible = true;
								topTipStack.HeightRequest = labelFormattedFull.Height;
							}
							else
							{
								IsTooltipExpanded = false;
								labelFormattedTrunc.IsVisible = true;
								labelFormattedFull.IsVisible = false;
								topTipStack.HeightRequest = labelFormattedTrunc.Height;
							}
								mainScrollView.ScrollToAsync(0, yScrollPosition, false);
						}
					}),
					NumberOfTapsRequired = 1
				};

				var toolTipLeftImage = new Image
				{
					Source = "small_icons_tip_white.png",
					HorizontalOptions = LayoutOptions.Start,
					VerticalOptions = LayoutOptions.Start,
					HeightRequest = 36,
				};

				topTipGrid.Children.Add(toolTipLeftImage, 0, 0);

				toolTipLeftImage.GestureRecognizers.Add(tapGestureRecognizer);

				labelFormattedTrunc = new WrappedTruncatedLabel
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					Lines = 2,
				};

				labelFormattedFull = new Label
				{
					VerticalOptions = LayoutOptions.FillAndExpand,
					IsVisible = false
				};

				var fs = new FormattedString();
				fs.Spans.Add(new Span
				{
					Text = "Top tip: ",
					ForegroundColor = Helpers.Color.White.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
					FontAttributes = FontAttributes.Bold
				});
				fs.Spans.Add(new Span
				{
					Text = Item.TopTip,
					ForegroundColor = Helpers.Color.White.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
				});
				labelFormattedTrunc.FormattedText = fs;
				labelFormattedFull.FormattedText = fs;

				var labelTipStack = new StackLayout
				{
					Padding = 0,
					Spacing = 0,
					VerticalOptions = LayoutOptions.StartAndExpand,
					Children = {
						labelFormattedTrunc,
						labelFormattedFull
					}
				};


				topTipGrid.Children.Add(labelTipStack, 1, 0);

				labelFormattedTrunc.GestureRecognizers.Add(tapGestureRecognizer);
				labelFormattedFull.GestureRecognizers.Add(tapGestureRecognizer);

				topTipStack.GestureRecognizers.Add(tapGestureRecognizer);

				topStickStackContainer = new StackLayout
				{
					Spacing = 0,
					Padding = new Thickness(10, 0),
					Children = {
						topTipStack
					}
				};

				stack2.Children.Add(topStickStackContainer);
			}
			else
			{
				topStickStackContainer = new StackLayout
				{
					Spacing = 0,
					Padding = new Thickness(0, 0)
				};

				topTipStack = new StackLayout
				{
					Orientation = StackOrientation.Vertical,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Padding = 0,
				};
				topTipStack.HeightRequest = 0;
				topTipStack.SizeChanged += (object sender, EventArgs e) =>
				{
					topStickStackContainer.HeightRequest = 0;
				};

				topStickStackContainer.Children.Add(topTipStack);

				stack2.Children.Add(topStickStackContainer);
			}

            if (topTipStack != null)
            {
                MessagingCenter.Subscribe<string>(string.Empty, "ReloadActionBar", (title) =>
                    {   
						if(topTipStack != null)
						{
                        	topTipStack.BackgroundColor = Shared.Helpers.Color.Secondary.ToFormsColor();
						}

                    });
            }
		}

		private void BuildStreamStack()
		{
			StreamStack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = 0
			};

			var feedBackStackContainer = new StackLayout
			{
				Spacing = 0,
				Padding = new Thickness(10, 0),
				Children = {
						StreamStack
					}
			};

			stack2.Children.Add(feedBackStackContainer);
		}

		private void BuildActivityButtons()
		{
			//var stack3 = new StackLayout
			//{
			//	Orientation = StackOrientation.Vertical,
			//	VerticalOptions = LayoutOptions.EndAndExpand,
			//	Padding = 0
			//};

			activityDoingStack = new StackLayout
			{
				Spacing = 3,
				Padding = 10,
				HeightRequest = 90,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.EndAndExpand,
				BackgroundColor = Helpers.Color.Primary.ToFormsColorWithAlpha(0xCC),
			};

			var activityDoingMessageStack = new StackLayout
			{
				BackgroundColor = Color.Transparent,
				Spacing = 5,
				Padding = 0,
				HeightRequest = 30,
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			var acceptHintLabel = new UnileverLabel
			{
				Text = "Do this activity?",
				TextColor = Color.White,
				//XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			activityDoingMessageStack.Children.Add(new Image
			{
				Source = "activities_white.png",
				WidthRequest = 30,
			});

			activityDoingMessageStack.Children.Add(acceptHintLabel);
			activityDoingStack.Children.Add(activityDoingMessageStack);

			var buttonsGrid = new FastGrid
			{
				Padding = new Thickness(5, 5, 5, 5),
				BackgroundColor = Color.Transparent,//Helpers.Color.GreenNotif.ToFormsColor().MultiplyAlpha(0xAA),
				VerticalOptions = LayoutOptions.StartAndExpand,
				RowDefinitions = {
					new RowDefinition { Height = 40 }
				},
				ColumnDefinitions = {
					new ColumnDefinition { Width = new GridLength (0.7, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (0.7, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength (0.7, GridUnitType.Star) }
				}
			};

			yesButton = new TalentPlus.Shared.Helpers.CustomImageButton("Yes", "\u2714")
			{				//yesButton = new TalentPlus.Shared.Helpers.CustomImageButton("Yes", "star_icon.svg")
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};

			buttonsGrid.Children.Add(yesButton, 0, 0);

			laterButton = new TalentPlus.Shared.Helpers.CustomImageButton("Later", "🕗")
			{				//laterButton = new TalentPlus.Shared.Helpers.CustomImageButton("Later", "time_icon.svg")
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			buttonsGrid.Children.Add(laterButton, 2, 0);

			noButton = new TalentPlus.Shared.Helpers.CustomImageButton("No", "\u2716")
			{				//noButton = new TalentPlus.Shared.Helpers.CustomImageButton("No", "cross_icon.svg")
				HorizontalOptions = LayoutOptions.FillAndExpand,
			};
			buttonsGrid.Children.Add(noButton, 1, 0);

			yesButton.Tapped += OnYesClicked;
			noButton.Tapped += OnNoClicked;
			laterButton.Tapped += MoveToNextPage;

			activityDoingStack.Children.Add(buttonsGrid);
			//stack3.Children.Add(activityDoingStack);

#if __IOS__
			activityDoingStack.SizeChanged += activityDoingStack_LayoutChanged;
#endif
		}

		protected override void OnAppearing ()
        {
			base.OnAppearing ();

           
                    
			//#if __ANDROID__
			if (activityDoingStack != null) {
				activityDoingStack.BackgroundColor = Helpers.Color.Primary.ToFormsColorWithAlpha(0xCC);
				yesButton.RefreshColorFilter();
				noButton.RefreshColorFilter();
				laterButton.RefreshColorFilter();
			}
			//#endif

			//LoadFeedbackPosts ();
        }

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();
		}

		protected override void OnSizeAllocated (double widht, double height)
        {
			base.OnSizeAllocated (widht, height);

			if (topTipStack != null && IsTooltipHeightInitialized == false) {
                IsTooltipHeightInitialized = true;
				//topTipStackHeight = topTipStack.Height;
                topTipStack.HeightRequest = 36;
            }
        }

		private async void StartActivityWithId(WhenViewModel page, int delay)
		{
			DateTime time = DateTime.Now.AddSeconds (delay);
			await ViewModel.AddSelectedActivity (page.GetActivity().Id, time, "", "");

			//Start Reminder Service
			var remiderService = DependencyService.Get<IReminderService> ();
			#if __ANDROID__
			if (remiderService != null)
				remiderService.Remind (time, "Activity Reminder", page.GetActivity().ShortDescription, page.GetActivity().Id, page.GetActivity().AlarmId);
			#else
			if (remiderService != null)
			remiderService.Remind (delay, "Activity Reminder", page.GetActivity().ShortDescription, page.GetActivity().Id, page.GetActivity().AlarmId);
			#endif

			OnAcceptedActivityDetails (EventArgs.Empty);
		}

		public async Task Start (WhenViewModel page, int delay)
        {
			DateTime time = DateTime.Now.AddSeconds (delay);
			String subject = "";
			String message = "";

			if (page != null) {
				subject = page.Subject;
				message = page.Message;
			}

			await ViewModel.AddSelectedActivity (((Activity)BindingContext).Id, time, subject, message);

            //Start Reminder Service
			var remiderService = DependencyService.Get<IReminderService> ();
#if __ANDROID__
            if (remiderService != null)
				remiderService.Remind (time, "Activity Reminder", ((Activity)BindingContext).ShortDescription, ((Activity)BindingContext).Id, ((Activity)BindingContext).AlarmId);
#else
			if (remiderService != null)
				remiderService.Remind (delay, "Activity Reminder", ((Activity)BindingContext).ShortDescription, ((Activity)BindingContext).Id, ((Activity)BindingContext).AlarmId);
#endif

			OnAcceptedActivityDetails (EventArgs.Empty);
        }

        //		void mainScrollView_Scrolled (object sender, ScrolledEventArgs e)
        //		{
        //			//Console.WriteLine ((ScrollView)sender);
        //		}

		void FeedbackPosts_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //			var device = XLabs.Ioc.Resolver.Resolve<XLabs.Platform.Device.IDevice> ();
            //			var screenWidthInInch = device.Display.Width / device.Display.WidthRequestInInches (1);
            //
            //			Console.WriteLine ("WidthByInch:{0}, Width:{1}, Height:{2}, Xdpi:{3}, Ydpi:{4}", screenWidthInInch, device.Display.Width, device.Display.Height, device.Display.Xdpi, device.Display.Ydpi); 

            StreamStack.IsVisible = true;

			//_remainingWidth = GetRemainingWidth ();

			RecentUsersList.Clear ();
            int i = 0;
            bool needSeparator = true;
			foreach (FeedbackPost post in FeedbackPosts) {
				if (!DisplayedFeedbacks.ContainsKey (post.Id)) {
					if (post == FeedbackPosts [FeedbackPosts.Count - 1]) {
                        needSeparator = false;
                    }
					FeedbackPostFormat (post, needSeparator);
					DisplayedFeedbacks.Add (post.Id, true);
                }
				if (i < 5 && !post.VIPContent) {
					RecentUsersList.Add (new User { Name = post.UserName, UserImage = post.UserImage });
                    i++;
                }
            }

			PopulateRecentUsersStack();

            //StreamStack.ForceLayout ();
        }

		private void UpdateStarRating()
		{
			EaseRating.UpdateRating(EaseOfUseAverage());
			EffectRating.UpdateRating(EffectivenessAverage());
		}

		private void PopulateRecentUsersStack ()
        {
			RecentImageStack.Children.Clear ();

			var isAllFake = true;

			if (RecentUsersList.Count == 0)
			{
				RecentUsersStack.IsVisible = false;
				return;
			}
			for (int i = 0; i < 5; i++) {
				if (i > (RecentUsersList.Count - 1)) {
					RecentImageStack.Children.Add (new Image {
                        Source = "user_icon.png",
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        WidthRequest = 40,
                        HeightRequest = 40,
                        Opacity = 0.2
                    }, i, 0);
				} else {
					if (string.IsNullOrEmpty (RecentUsersList [i].UserImage) || RecentUsersList [i].UserImage == "user_icon.png") {
						RecentImageStack.Children.Add (new Image {
							Source = "user_icon.png",
							WidthRequest = 40,
							HeightRequest = 40,
							HorizontalOptions = LayoutOptions.End,
							VerticalOptions = LayoutOptions.CenterAndExpand,
							Opacity = 0.2
						}, i, 0);
					} else {
						isAllFake = false;
						RecentImageStack.Children.Add (new TPCircleImage {
							Source = RecentUsersList [i].UserImage,
                            HorizontalOptions = LayoutOptions.End,
                            VerticalOptions = LayoutOptions.CenterAndExpand,
							Aspect = Xamarin.Forms.Aspect.AspectFill,
                            WidthRequest = 40,
                            HeightRequest = 40
                        }, i, 0);
                    }
                }
            }
			if (!isAllFake)
			{
				RecentUsersStack.IsVisible = true;
			}
        }

//		private double GetRemainingWidth ()
//		{
//			double ratio = (Dpi / SCREEN_STANDARD_WIDTH);

//			double diffRatio = 1;

//#if __ANDROID__
//			double widthRatio = ScreenWidth / (SCREEN_STANDARD_WIDTH * 2);
//			double densRatio = Dpi / 320;
//			diffRatio = widthRatio / densRatio;
//#endif

//			var remainingValue = Dpi - ((USER_ICON_WIDTH + (PADDING_VIEW * 4)) * ratio);
//			Console.WriteLine ("remainingValue:" + remainingValue);
//			return (remainingValue / ratio) * diffRatio;
//		}

		private void FeedbackPostFormat (FeedbackPost post, bool needSeparator)
            {
			if (string.IsNullOrEmpty(post.Description) && string.IsNullOrEmpty(post.ImageUrl) && string.IsNullOrEmpty(post.VideoUrl))
			{
				return;
			}
			var rowLayout = new StackLayout {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0,
                Spacing = 10
            };

			var contentLayout = new StackLayout {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = 0
            };

            var opacity = 1.0;
			if (post.UserImage == "user_icon.png") {
                opacity = 0.2;
            }

			rowLayout.Children.Add (new TPCircleImage {
                Source = post.UserImage,
                HeightRequest = USER_ICON_WIDTH,
                WidthRequest = USER_ICON_WIDTH,
                Aspect = Aspect.AspectFill,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Opacity = opacity
            });

			rowLayout.Children.Add (new StackLayout {
                HorizontalOptions = LayoutOptions.Fill,
                Spacing = 0,
				Children = {
					new UnileverLabel {
						Text = post.UserName,
						VerticalOptions = LayoutOptions.Start,
						FontSize = Device.GetNamedSize (NamedSize.Medium, typeof(UnileverLabel)),
						TextColor = Color.Black
					},
					new UnileverLabel {
						Text = post.Date.ToString (),
						VerticalOptions = LayoutOptions.Start,
						FontSize = Device.GetNamedSize (NamedSize.Small, typeof(UnileverLabel)),
						TextColor = Color.Gray
					}
				}
            });

            Image image = null;
            Label lblDescription = null;

			if (!string.IsNullOrEmpty (post.Description)) {
				lblDescription = new Label {
                    Text = post.Description,
                    TextColor = Color.Gray,
					FontSize = Device.GetNamedSize (NamedSize.Small, typeof(Label)),
                    //WidthRequest = _remainingWidth,
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Start
                };
				contentLayout.Children.Add(lblDescription);
            }

			if (!string.IsNullOrEmpty (post.ImageUrl)) {
				image = new Image () {
                    Source = post.ImageUrl,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Aspect = Aspect.AspectFit,
                    //WidthRequest = _remainingWidth
                };

//				if (string.IsNullOrEmpty (post.VideoUrl)) {
//					image.GestureRecognizers.Add (new TapGestureRecognizer ((View imageView) => {

//#if __ANDROID__

//						var popup = new DroidImagePopup (PopupPhoto, post.ImageUrl);
//						popup.Show ();

//#else
//						double ratio = imageView.Height / imageView.Width;

//						PopupPhoto.ShowPopup (new Image () {
//							Source = post.ImageUrl,
//							VerticalOptions = LayoutOptions.FillAndExpand,
//							HorizontalOptions = LayoutOptions.FillAndExpand,
//							Aspect = Aspect.AspectFill,
//							HeightRequest = SCREEN_STANDARD_WIDTH * ratio,
//							WidthRequest = SCREEN_STANDARD_WIDTH,
//						});				

//						//MessagingCenter.Send(this, "ShowImage");
//#endif

//					}));
//				}
            }

			if (!String.IsNullOrEmpty (post.VideoUrl)) {
                image.Aspect = Aspect.AspectFill;
				var VideoPlayImage = new TalentPlus.Shared.Helpers.DarkIceImage {
                    Source = "playurl.png",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    //HeightRequest = 50,
                    //WidthRequest = 50,
                    Aspect = Aspect.AspectFit,
                };

				VideoPlayImage.Tapped = () => {
#if __ANDROID__
					Navigation.PushAsync (new VideoPlayView (post.VideoUrl));
#else
					videoView.VideoURI = post.VideoUrl;
					videoView.StartPlay = !videoView.StartPlay;
#endif
                };

				var VideoStack = new AbsoluteLayout {
                    //VerticalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Color.Transparent,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    //WidthRequest = _remainingWidth,
                    //HeightRequest = _remainingWidth,
                    Padding = 0
                };

                //image.Aspect = Aspect.AspectFill;

				VideoStack.Children.Add (image);
				AbsoluteLayout.SetLayoutFlags (image, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (image, new Rectangle (0, 0, 1, 1));

				AbsoluteLayout.SetLayoutFlags (VideoPlayImage, AbsoluteLayoutFlags.All);
				AbsoluteLayout.SetLayoutBounds (VideoPlayImage, new Rectangle (0.5, 0.5, 0.5, 0.5));
				VideoStack.Children.Add (VideoPlayImage);

				contentLayout.Children.Add (VideoStack);
			} else {
				if (image != null) {
					contentLayout.Children.Add (image);
                }
            }

            //rowLayout.Children.Add (contentLayout);
			StreamStack.Children.Add (rowLayout);
			StreamStack.Children.Add (contentLayout);
			if (needSeparator) {
				StreamStack.Children.Add (new BoxView () {
					Color = Helpers.Color.Gray.ToFormsColor (),
					HeightRequest = 1,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Opacity = 0.2
				});
            }
        }

		async void OnNoClicked ()
        {
			yesButton.Tapped -= OnYesClicked;
			noButton.Tapped -= OnNoClicked;
			var negativeFeedbackView = new ActivityNegativeFeedbackView (BindingContext as Activity);
			await this.Navigation.PushAsync (negativeFeedbackView);
			yesButton.Tapped += OnYesClicked;
			noButton.Tapped += OnNoClicked;
        }

		public string GetActivityId ()
        {
            return ((Activity)BindingContext).Id;
        }

		private bool alreadyRefreshing = false;
		public async Task RefreshFeedback()
		{
			if (alreadyRefreshing || !IsLoaded) { return; }
			alreadyRefreshing = true;

			var updatedActivity = await TalentDb.GetActivity (((Activity)BindingContext).Id);

			if (updatedActivity != null) {
				BindingContext = updatedActivity;

				if (((Activity)BindingContext).FeedbackPosts != null && ((Activity)BindingContext).FeedbackPosts.Count > 0) {
					FeedbackPosts.Clear (); //TODO: consider not reloading everything, but just adding new one

					foreach (FeedbackPost post in ((Activity)BindingContext).FeedbackPosts) {
						if (post != null) {
							if (!FeedbackPosts.Contains (post)) {
								FeedbackPosts.Add (post);
							}
						}
					}
					FeedbackPosts_CollectionChanged (null, null);
				}
			}
			UpdateStarRating();
			
			alreadyRefreshing = false;
		}

		public void LoadFeedbackPosts()
		{
			if (!FeedbackPostsLoaded)
			{
				if (((Activity)BindingContext).FeedbackPosts != null && ((Activity)BindingContext).FeedbackPosts.Count > 0)
				{
					var labelAdded = false;
					foreach (FeedbackPost post in ((Activity)BindingContext).FeedbackPosts)
					{
						if (post != null)
						{
							if (!labelAdded && post.VIPContent)
							{
								AddFeaturedContentLabel();
								labelAdded = true;
							}
							if (!FeedbackPosts.Contains(post))
							{
								FeedbackPosts.Add(post);
							}
						}
					}
					FeedbackPostsLoaded = true;
					FeedbackPosts_CollectionChanged(null, null);
				}
			}
		}

		private void AddFeaturedContentLabel()
		{
			var featuredLabel = new IlustrativeLabel
			{
				Text = "FEATURED CONTENT",
				FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				XAlign = TextAlignment.Start,
				YAlign = TextAlignment.Start,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};
			StreamStack.Children.Add(featuredLabel);
		}

        /// <summary>
        /// Callback for YesButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		async void OnYesClicked ()
        {
			yesButton.Tapped -= OnYesClicked;
			noButton.Tapped -= OnNoClicked;
			(BindingContext as Activity).SelectedUsers = new List<User> ();
			var whenView = new WhenView (BindingContext as Activity);
			await this.Navigation.PushAsync (whenView);
			yesButton.Tapped += OnYesClicked;
			noButton.Tapped += OnNoClicked;
        }

		void MoveToPrevPage (object s, EventArgs e)
        {
			ActivityParentView.MoveToPrevPage ((BindingContext as Activity).Id);
        }

		void MoveToNextPage (object s = null, EventArgs e = null)
        {
			ActivityParentView.MoveToNextPage((BindingContext as Activity).Id);
        }

		void MoveToNextPage ()
        {
			ActivityParentView.MoveToNextPage((BindingContext as Activity).Id);
        }

        /// <summary>
        /// Event when the activity has been completely accepted
        /// </summary>
        /// <param name="e"></param>
		protected virtual void OnAcceptedActivityDetails (EventArgs e)
        {
            EventHandler handler = AcceptedActivityDetails;
			if (handler != null) {
				handler (this, e);
            }
        }

        public event EventHandler AcceptedActivityDetails;
    }
}

