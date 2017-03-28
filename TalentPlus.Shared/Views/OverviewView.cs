using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TalentPlus.Shared
{
	public class NotificationObject
	{
		public Label Title { get; set; }
		public Label Time { get; set; }
		public Image IconLeft { get; set; }
		//public SvgColorIcon IconRight { get; set; }
		public TalentPlus.Shared.Helpers.DarkIceImage IconRight { get; set; } 

		public NotificationType Type { get; set; }

		public Activity Activity { get; set; }

		public StackLayout OuterLayout { get; set; }
		public StackLayout InnerLayout { get; set; }
		public StackLayout AdditionalContentStack { get; set; }

		public StackLayout InvitationLayout { get; set; }

		public StackLayout Separator { get; set; }

		public string InviteId { get; set; }

		public Helpers.CustomImageButtonNotifications YesButton { get; set; }
		public Helpers.CustomImageButtonNotifications NoButton { get; set; }

		public NotificationObject(String title, Activity activity, DateTime receivedTime, NotificationType type, string inviteId = null, User user = null)
		{
			this.Title = new Label
			{
				Text = title,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 0.9,
				TextColor = Color.Black,
				FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.StartAndExpand,
				VerticalOptions = LayoutOptions.Center
			};

			this.IconLeft = new Image
			{
				Source = "icon_notifications.png",
				Aspect = Aspect.AspectFit,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Start,
				Scale = 0.6
			};

			this.Time = new Label
			{
				Text = receivedTime.ToString("HH:mm"),
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Start
			};

			//this.IconRight = new Image
			//{
			//	Source = "small_icons_cross_green.png",
			//	Aspect = Aspect.AspectFit,
			//	HorizontalOptions = LayoutOptions.EndAndExpand,
			//	VerticalOptions = LayoutOptions.Start
			//};

			var imageWidth = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 1.2; 
			var imageHeight = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 1.3;

			this.IconRight = new TalentPlus.Shared.Helpers.DarkIceImage
			{
				Source = "cross_icon.png", 
				Aspect = Aspect.AspectFit, 
				HorizontalOptions = LayoutOptions.End, 
				VerticalOptions = LayoutOptions.Start, 
				HeightRequest = imageHeight, 
				WidthRequest = imageWidth, 
				FilterColor  = Helpers.Color.Primary.ToFormsColor() 
			};

			//var imageWidth = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.2;
			//var imageHeight = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 1.3;

			//this.IconRight = new SvgColorIcon {
			//        SvgPath = "activities_icon.svg",
			//        SvgAssembly = typeof(TalentPlusApp).GetTypeInfo ().Assembly,
			//        HorizontalOptions = LayoutOptions.Center,
			//        VerticalOptions = LayoutOptions.Center,
			//        BackgroundColor = Xamarin.Forms.Color.Transparent,
			//        HeightRequest = imageHeight,
			//        WidthRequest = imageWidth,
			//        Aspect = Xamarin.Forms.Aspect.AspectFit,
			//        IconColor = Helpers.Color.Primary.ToIntColor (),
			//    };

			this.Activity = activity;

			this.Type = type;

			this.InviteId = inviteId;

			if (type == NotificationType.Invitation || type == NotificationType.Action)
			{
				IconLeft.Source = "icon_alerts.png";
				IconRight.Source = "arrow_right_icon.png";

				//IconRight.
				if (type == NotificationType.Invitation)
				{
					InvitationLayout = new StackLayout
					{
						Orientation = StackOrientation.Vertical,
						IsVisible = false,
						Children =
							{
								new BoxView { Color = Color.Transparent, HeightRequest = 10 },
								new BoxView() { Color = Helpers.Color.Gray.ToFormsColor(), HeightRequest = 1, Opacity = 1 },
								new BoxView { Color = Color.Transparent, HeightRequest = 5 },
							}
					};
				}
			}

			BuildLayout(user);
		}

		private void BuildLayout(User user)
		{
			InnerLayout = new StackLayout { Padding = new Thickness(10, 0), Orientation = StackOrientation.Horizontal };

			//var leftImageStack = new StackLayout
			//{
			//	Padding = 0,
			//	Spacing = 0,
			//	Children = { IconLeft },
			//	BackgroundColor = Color.Blue,
			//	HorizontalOptions = LayoutOptions.Start,
			//	VerticalOptions = LayoutOptions.Start
			//};

			InnerLayout.Children.Add(IconLeft);
			InnerLayout.Children.Add(new StackLayout
			{
				//BackgroundColor = Color.Red,
				Padding = new Thickness(0, 5, 0, 0),
				Spacing = 0,
				Children = { Title }
			});

			var rightStack = new StackLayout
			{
				Padding = new Thickness(0, 5, 0, 0),
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.EndAndExpand,
				Children = { Time, IconRight }
			};

			InnerLayout.Children.Add(rightStack);

			OuterLayout = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0, 0, 0, 20),
				Spacing = 0,
				Children = 
					{ 
						InnerLayout
					}
			};

			AdditionalContentStack = new StackLayout
			{
				Padding = new Thickness(0),
				Spacing = 0,
				VerticalOptions = LayoutOptions.Start
			};

			IconLeft.SizeChanged += (s, e) =>
			{
				Image image = s as Image;

				if (image != null && image.Width > 0)
				{
					AdditionalContentStack.Padding = new Thickness(image.Width + 15, 0, 0, 0);
					if (Separator != null)
					{
						Separator.Padding = new Thickness(image.Width + 15, 0, 0, 0);
					}
				}
			};

			if (Title.Text == "Pending Feedback")
			{
				AdditionalContentStack.Children.Add(new UnileverLabel
				{
					Text = "Please provide feedback on the activity. Your feedback help others.",
					TextColor = Helpers.Color.Gray.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
					VerticalOptions = LayoutOptions.Start
				});
				OuterLayout.Children.Add(AdditionalContentStack);
			}
			else if (Title.Text == "Sent Request")
			{
				AdditionalContentStack.Children.Add(new UnileverLabel
				{
					Text = "You have sent a request to " + user.Name + " to join you in this activity.",
					TextColor = Helpers.Color.Gray.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
					VerticalOptions = LayoutOptions.Start
				});
				OuterLayout.Children.Add(AdditionalContentStack);
			}
			else if (Title.Text == "Accepted Request")
			{
				AdditionalContentStack.Children.Add(new UnileverLabel
				{
					Text = user.Name + " accepted your invitation to join you in this activity.",
					TextColor = Helpers.Color.Gray.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
					VerticalOptions = LayoutOptions.Start
				});
				OuterLayout.Children.Add(AdditionalContentStack);
			}
			else if (Title.Text == "Declined Request")
			{
				AdditionalContentStack.Children.Add(new UnileverLabel
				{
					Text = user.Name + " declined your invitation to join you in this activity.",
					TextColor = Helpers.Color.Gray.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
					VerticalOptions = LayoutOptions.Start
				});
				OuterLayout.Children.Add(AdditionalContentStack);
			}

			if (Type == NotificationType.Invitation)
			{
				OuterLayout.Children.Add(AdditionalContentStack);

				var labelFormatted = new Label
				{
					VerticalOptions = LayoutOptions.StartAndExpand
				};
				var fs = new FormattedString();
				var userName = "Someone";
				var userImage = "user_icon.png";

				if (user != null)
				{
					userName = user.Name;
					userImage = user.UserImage;
				}

				fs.Spans.Add(new Span { Text = userName, ForegroundColor = Helpers.Color.Gray.ToFormsColor(), FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), FontAttributes = FontAttributes.Bold });
				fs.Spans.Add(new Span { Text = " needs you to help with the activity '" + Activity.ShortDescription + "'. Would you like to take part?", ForegroundColor = Helpers.Color.Gray.ToFormsColor(), FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) });
				labelFormatted.FormattedText = fs;

				var textStack = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Padding = new Thickness(10, 0),
					Children =
						{ 
							new TPCircleImage { HeightRequest = 30, WidthRequest = 30, Aspect = Aspect.AspectFit, Source = userImage, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.CenterAndExpand }, 
							new StackLayout { Padding = new Thickness(5, 0, 0, 0), Children = { labelFormatted } }
						}
				};

				YesButton = new Helpers.CustomImageButtonNotifications("YES", "button_notif_background", Helpers.Color.White)
				{
					HorizontalOptions = LayoutOptions.Start,
					VerticalOptions = LayoutOptions.Center,
					WidthRequest = Device.OnPlatform<int>(72, 100, 80),
					HeightRequest = Device.OnPlatform<int>(52, 70, 40),
					Padding = new Thickness(5, 0)
				};

				NoButton = new Helpers.CustomImageButtonNotifications("NO", "button_notif_background", Helpers.Color.White)
				{
					HorizontalOptions = LayoutOptions.Start,
					VerticalOptions = LayoutOptions.Center,
					WidthRequest = Device.OnPlatform<int>(72, 100, 80),
					HeightRequest = Device.OnPlatform<int>(52, 70, 40),
					Padding = new Thickness(5, 0)
				};

				var buttonsStack = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Padding = new Thickness(45, 10, 0, 0),
					Children =
						{ 
							YesButton,
							NoButton
						}
				};

				InvitationLayout.Children.Add(textStack);
				InvitationLayout.Children.Add(buttonsStack);

				AdditionalContentStack.Children.Add(new UnileverLabel
				{
					Text = "From " + userName,
					TextColor = Helpers.Color.Gray.ToFormsColor(),
					FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
					VerticalOptions = LayoutOptions.Start
				});
				OuterLayout.Children.Add(AdditionalContentStack);

				OuterLayout.Children.Add(InvitationLayout);
			}

			Separator = new StackLayout
			{
				Padding = new Thickness(0),
				Spacing = 0,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Children =
					{
						new BoxView() { Color = Helpers.Color.Gray.ToFormsColor(), HeightRequest = 1, Opacity = 1 }
					}
			};
		}
	}

	public class TaskStack
	{
		public AbsoluteLayout TopStack { get; set; }
		public Label TasksNumberLabel { get; set; }

		public int TasksNumber { get; set; }

		public TaskStack(Activity activity, int taskNumbers)
		{
			var absoluteLayout = new AbsoluteLayout
			{
				Padding = new Thickness(0),
			};

			//var image = new TalentPlus.Shared.Helpers.DarkIceImage
			//{
			//	Source = "activity_heading_up",
			//	Aspect = Aspect.Fill,
			//	ShouldUseInset = true,
			//	LeftInset = 10,
			//	TopInset = 10,
			//	RightInset = 10,
			//	BottomInset = 0
			//};
			//var image = new RoundedBox
			//{
			//	BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
			//	CornerRadius = 10,
			//	RoundedSide = RoundedBox.RoundedSideType.Top
			//};

			this.TasksNumber = taskNumbers;

			var stackLayout = new StackLayout
			{
				BackgroundColor = Helpers.Color.GrayBackground.ToFormsColor(),
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(10, 0)
			};

			UnileverLabel taskTitle = new UnileverLabel
			{
				Text = activity.ShortDescription,
				TextColor = Color.Black,
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};

			stackLayout.Children.Add(taskTitle);

			TasksNumberLabel = new Label
			{
				Text = "" + TasksNumber,
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 0.9,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.StartAndExpand
			};

			var tasksNumberLabelStack = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand, Children = { TasksNumberLabel }, Padding = 0 };

			var absoluteLayoutNumber = new AbsoluteLayout { Padding = new Thickness(0), HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
			var alertNumberTotalImage = new Image { Source = "alert_number.png", Aspect = Aspect.AspectFit, HeightRequest = (Device.GetNamedSize(NamedSize.Large, typeof(Label)) * 0.9) * 1.1 };

			absoluteLayoutNumber.Children.Add(alertNumberTotalImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteLayoutNumber.Children.Add(tasksNumberLabelStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			stackLayout.Children.Add(absoluteLayoutNumber);

			//absoluteLayout.Children.Add(image, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteLayout.Children.Add(stackLayout, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			TopStack = absoluteLayout;
		}
	}

	public enum NotificationType
	{
		Action,
		Invitation,
		Standard
	}

	public class OverviewView : BaseView
	{
		private ActivitiesViewModel ViewModel
		{
			get;
			set;
		}

		private int AttentionNotifications { get; set; }
		private int TotalNotifications { get; set; }

		private UnileverLabel LastCompletedLabel { get; set; }
		private UnileverLabel OveralCompletedLabel { get; set; }

		private UnileverLabel AttentionNotificationsLabel { get; set; }
		private UnileverLabel TotalNotificationsLabel { get; set; }

		private AbsoluteLayout TotalNotificationsAlertNumberAbsoluteLayout { get; set; }
		private AbsoluteLayout AttentionNotificationsAlertNumberAbsoluteLayout { get; set; }

		private ActivityInvitation SelectedInvitation { get; set; }
		private TPListView ListViewInvitations { get; set; }

		private StackLayout InvitationActionsLayout { get; set; }

		private Dictionary<string, TaskStack> TaskStacskObject { get; set; }
		private Dictionary<string, List<NotificationObject>> NotificationsObject { get; set; }

		private StackLayout ContentStack { get; set; }
		private TPCircleImage UserImageCircle { get; set; }
		private Grid UserProfileGrid { get; set; }

		private ObservableCollection<ActivityInvitation> SentInvitations { get; set; }
		private ObservableCollection<ActivityInvitation> ActivityInvitations { get; set; }

		private TalentPlus.Shared.Helpers.DarkIceImage _imgActivities;

		public OverviewView()
		{
			ViewModel = ViewModelLocator.ActivitiesViewModel;
			BackgroundColor = Color.White;

			TaskStacskObject = new Dictionary<string, TaskStack>();
			NotificationsObject = new Dictionary<string, List<NotificationObject>>();

			this.ActivityInvitations = new ObservableCollection<ActivityInvitation>();
			this.SentInvitations = new ObservableCollection<ActivityInvitation>();

			AttentionNotifications = ViewModel.PendingFeedbacks.Count;
			TotalNotifications = ViewModel.PendingFeedbacks.Count + ViewModel.SelectedActivities.Count;

			InvitationActionsLayout = new StackLayout { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.Start };

			#region Layout

			// Main Stack
			var stack = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 10,
				Padding = 0
			};

			// Second Stack
			var stack2 = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Spacing = 10,
				Padding = 0
			};

			//User profile Grid
			UserProfileGrid = new Grid
			{
				//BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 90,
				Padding = new Thickness(10),
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(7, GridUnitType.Star) }
                }
			};

			var userImageSource = "user_icon.png";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty(TalentPlusApp.CurrentUser.UserImage))
				userImageSource = TalentPlusApp.CurrentUser.UserImage;
			UserImageCircle = new TPCircleImage { Source = userImageSource, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 90, WidthRequest = 90, Aspect = Aspect.AspectFill };
			UserProfileGrid.Children.Add(UserImageCircle, 0, 0);

			var settingsButton = new TPButton
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "Settings",
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Button)),
				BorderWidth = 0,
				HeightRequest = 38,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.End
			};
			settingsButton.Clicked += (s, e) =>
			{
				var homeView = this.Parent as TabbedIconPage;
				homeView.SwitchToSettings();
			};

			var userName = "Me";
			if (TalentPlusApp.CurrentUser != null && !string.IsNullOrEmpty(TalentPlusApp.CurrentUser.Name))
				userName = TalentPlusApp.CurrentUser.Name;

			UserProfileGrid.Children.Add(new StackLayout
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness(5, 5, 0, 5),
				Children =
					{
						new UnileverLabel{Text = userName, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start, TextColor = Color.Black },
						settingsButton
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
						UserProfileGrid
					}
			};

			stack2.Children.Add(UserProfileGrid);

			// Stats Grid
			Grid statGrid = new Grid
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
			};

			OveralCompletedLabel = new UnileverLabel
			{
				Text = "",
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.5,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start
			};

			OveralCompletedLabel.BindingContext = ViewModel;
			OveralCompletedLabel.SetBinding(Label.TextProperty, "ActivitiesCompletedPercent", 0, null, "{0}%");

			statGrid.Children.Add(new StackLayout
			{
				Children =
					{
						OveralCompletedLabel,
						new Label{Text = "COMPLETED OVERALL", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Helpers.Color.Gray.ToFormsColor() }
					}
			}, 0, 0);

			LastCompletedLabel = new UnileverLabel
			{
				Text = "",
				TextColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.5,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			LastCompletedLabel.BindingContext = ViewModel;
			LastCompletedLabel.SetBinding(Label.TextProperty, "ActivitiesCompletedInSixMonths");

			var imageSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.5;

			_imgActivities = new TalentPlus.Shared.Helpers.DarkIceImage {
				Source = "activities_icon.png",
				HeightRequest = imageSize,
				WidthRequest = imageSize,
				Aspect = Xamarin.Forms.Aspect.AspectFit,
				FilterColor = Helpers.Color.Primary.ToFormsColor (),
			};

			var stackTotalNotifs = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Spacing = 0,
				Padding = 0,
				Children = 
				{
					 //Image { Source = "activities.png", Aspect = Aspect.AspectFit, HeightRequest = imageSize },
					_imgActivities,
					LastCompletedLabel
				}
			};

			statGrid.Children.Add(new StackLayout
			{
				Children =
					{
						stackTotalNotifs,
						new Label{Text = "LAST 6 MONTHS", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End, TextColor = Helpers.Color.Gray.ToFormsColor() }
					}
			}, 1, 0);

			var stackLayoutProfileStats = new StackLayout
			{
				VerticalOptions = LayoutOptions.Fill,
				HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex("EFECF4"),
				Padding = new Thickness(0, 5),
				Children =
					{
						statGrid
					}
			};

			stack2.Children.Add(stackLayoutProfileStats);

			// Top Grid
			Grid grid = new Grid
			{
				Padding = new Thickness(10, 20),
				VerticalOptions = LayoutOptions.Start,
				HorizontalOptions = LayoutOptions.Fill,
				ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
			};

			//ATTENTION NOTIFICATIONS FULL LAYOUT

			var attentionNotificationsAbsoluteLayout = new AbsoluteLayout { Padding = new Thickness(0, 0), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start };
			var attentionNotificationsImage = new Image { Source = "icon_alerts.png", Aspect = Aspect.AspectFit };

			AttentionNotificationsLabel = new UnileverLabel
			{
				Text = "" + AttentionNotifications,
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var attentionNotificationsLabelStack = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand, Children = { AttentionNotificationsLabel } };

			AttentionNotificationsAlertNumberAbsoluteLayout = new AbsoluteLayout { Padding = new Thickness(0), HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.StartAndExpand };
			var alertNumberImage = new Image { Source = "alert_number.png", Aspect = Aspect.AspectFit, HeightRequest = Device.GetNamedSize(NamedSize.Micro, typeof(UnileverLabel)) * 1.5 };

			AttentionNotificationsAlertNumberAbsoluteLayout.Children.Add(alertNumberImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			AttentionNotificationsAlertNumberAbsoluteLayout.Children.Add(attentionNotificationsLabelStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			var attentionNotificationsStack = new StackLayout { Padding = new Thickness(0, 0), Children = { AttentionNotificationsAlertNumberAbsoluteLayout } };

			attentionNotificationsAbsoluteLayout.Children.Add(attentionNotificationsImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			attentionNotificationsAbsoluteLayout.Children.Add(attentionNotificationsStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);


			grid.Children.Add(new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				Children =
					{
						attentionNotificationsAbsoluteLayout,
						new Label{Text = "requires response", FontFamily = "Arial", FontAttributes = Xamarin.Forms.FontAttributes.Bold, TextColor = Color.Black, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center }
					}
			}, 0, 0);

			//TOTAL NOTIFICATIONS FULL LAYOUT

			var totalNotificationsAbsoluteLayout = new AbsoluteLayout { Padding = new Thickness(0, 0), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start };
			var totalNotificationsImage = new Image { Source = "icon_notifications.png", Aspect = Aspect.AspectFit };

			TotalNotificationsLabel = new UnileverLabel
			{
				Text = "" + TotalNotifications,
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var totalNotificationsLabelStack = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand, Children = { TotalNotificationsLabel } };

			TotalNotificationsAlertNumberAbsoluteLayout = new AbsoluteLayout { Padding = new Thickness(0), HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.StartAndExpand };
			var alertNumberTotalImage = new Image { Source = "alert_number.png", Aspect = Aspect.AspectFit, HeightRequest = Device.GetNamedSize(NamedSize.Micro, typeof(UnileverLabel)) * 1.5 };

			TotalNotificationsAlertNumberAbsoluteLayout.Children.Add(alertNumberTotalImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			TotalNotificationsAlertNumberAbsoluteLayout.Children.Add(totalNotificationsLabelStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			var totalNotificationsStack = new StackLayout { Padding = new Thickness(0, 0), Children = { TotalNotificationsAlertNumberAbsoluteLayout } };

			totalNotificationsAbsoluteLayout.Children.Add(totalNotificationsImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			totalNotificationsAbsoluteLayout.Children.Add(totalNotificationsStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			grid.Children.Add(new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				Children =
					{
						totalNotificationsAbsoluteLayout,
						new Label{Text = "notifications", FontFamily = "Arial", FontAttributes = Xamarin.Forms.FontAttributes.Bold, TextColor = Color.Black, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center }
					}
			}, 1, 0);

			stack2.Children.Add(grid);

			ContentStack = new StackLayout
			{
				VerticalOptions = LayoutOptions.StartAndExpand,
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0),
				Spacing = 0
			};

			stack2.Children.Add(ContentStack);

			stack.Children.Add(new ScrollView { VerticalOptions = LayoutOptions.FillAndExpand, Content = stack2 });

			Content = stack;

			Refresh();

			#endregion

			MessagingCenter.Subscribe<SettingsPage>(this, "UserImageChanged", (page) =>
			{
				try
				{
					var myself = TalentPlusApp.CurrentUser;// await TalentDb.GetItem<User>(TalentPlusApp.CurrentUserId);
					if (myself != null)
					{
						UserProfileGrid.Children.Remove(UserImageCircle);
						UserImageCircle = new TPCircleImage { Source = myself.UserImage, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HeightRequest = 90, WidthRequest = 90, Aspect = Aspect.AspectFill };
						UserProfileGrid.Children.Add(UserImageCircle, 0, 0);
					}
				}
				catch (Exception ex)
				{

				}
			});
		}

		public async Task StartActivityWithId(WhenViewModel page, int delay)
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

		}

		public async Task FeedbackSubmitted(string activityId)
		{
			LoadingViewFlag = true;
			await ViewModel.RemovePendingFeedBackAndAddArchive(activityId);
			await ViewModel.RefreshCountItems ();

			OveralCompletedLabel.Text = ViewModel.ActivitiesCompletedPercent + "%";
			OveralCompletedLabel.TextColor = Helpers.Color.Primary.ToFormsColor();

			LastCompletedLabel.Text = ViewModel.ActivitiesCompletedInSixMonths + "";
			LastCompletedLabel.TextColor = Helpers.Color.Primary.ToFormsColor();

			await PutActivityBack(activityId);
			var detailsView = TalentPlusApp.RootPage.activities.GetActivityDetailsViewFromActivityId(activityId);
			if (detailsView != null)
			{
				await detailsView.RefreshFeedback();
			}
			LoadingViewFlag = false;
		}

		private async Task PutActivityBack(string activityId) {
			Activity activity = ViewModel.GetActivity(activityId);
			if (activity == null)
			{
				activity = await ViewModel.GetActivityFromDb(activityId);
				ViewModel.Activities.Add(activity); //TODO: this should add in the corrct place, not to the end
			}
			await TalentPlusApp.RootPage.ActivityFinished(activity);
		}

		public async Task NotDoingActivity(string activityId)
		{
			await ViewModel.RemovePendingFeedBack(activityId);

			Activity activity = ViewModel.GetActivity(activityId);
			if (activity == null)
			{
				activity = await ViewModel.GetActivityFromDb(activityId);
			}
			ViewModel.Activities.Add(activity);
			await TalentPlusApp.RootPage.ActivityFinished(activity);
		}

		async void noButton_Clicked()
		{
			SelectedInvitation.InvitationStatus = InvitationStatus.Declined;
			await TalentDb.SaveOrUpdateItem<ActivityInvitation>(SelectedInvitation);
			InvitationActionsLayout.IsVisible = false;
			ListViewInvitations.IsVisible = true;
		}

		async void yesButton_Clicked()
		{
			TimeSpan timeDiff = SelectedInvitation.FinishTime - DateTime.Now;
			SelectedInvitation.InvitationStatus = InvitationStatus.Accepted;
			await TalentDb.SaveOrUpdateItem<ActivityInvitation>(SelectedInvitation);
			await ViewModel.StartActivity(SelectedInvitation.Activity, Convert.ToInt32(timeDiff.TotalSeconds), "", "");
			InvitationActionsLayout.IsVisible = false;
			ListViewInvitations.IsVisible = true;
		}

		private void BuildPageContent()
		{
			ContentStack.Children.Clear();

			//Clear Dictionaries
			var buffer = new List<string>(NotificationsObject.Keys);

			foreach (var key in buffer)
			{
				NotificationsObject[key] = new List<NotificationObject>();
			}

			//Count Notifications
			TotalNotifications = 0;
			AttentionNotifications = 0;

			//Reorganise Content
			foreach (PendingFeedback pA in ViewModel.PendingFeedbacks)
			{
				if (NotificationsObject.ContainsKey(pA.ActivityId))
				{
					NotificationsObject[pA.ActivityId].Add(new NotificationObject("Pending Feedback", pA.Activity, pA.ReceiveTime, NotificationType.Action));
				}
				else
				{
					var list = new List<NotificationObject>();
					list.Add(new NotificationObject("Pending Feedback", pA.Activity, pA.ReceiveTime, NotificationType.Action));
					NotificationsObject.Add(pA.ActivityId, list);
				}
				AttentionNotifications += 1;
				TotalNotifications += 1;
			}

			foreach (ActivityInvitation aI in this.ActivityInvitations)
			{
				string userName = "Someone";

				if (aI.SenderUser != null && !string.IsNullOrEmpty(aI.SenderUser.Name))
				{
					userName = aI.SenderUser.Name;
				}

				if (NotificationsObject.ContainsKey(aI.ActivityId))
				{
					NotificationsObject[aI.ActivityId].Add(new NotificationObject("Received Request", aI.Activity, aI.ReceiveTime, NotificationType.Invitation, aI.Id, aI.SenderUser));
				}
				else
				{
					var list = new List<NotificationObject>();
					list.Add(new NotificationObject("Received Request", aI.Activity, aI.ReceiveTime, NotificationType.Invitation, aI.Id, aI.SenderUser));
					NotificationsObject.Add(aI.ActivityId, list);
				}
				AttentionNotifications += 1;
				TotalNotifications += 1;
			}


			foreach (ActivityInvitation sI in this.SentInvitations)
			{
				if (sI.Visible)
				{
					string userName = "Someone";

					if (sI.TargetUser != null && !string.IsNullOrEmpty(sI.TargetUser.Name))
					{
						userName = sI.TargetUser.Name;
					}
					var sentRequestStatus = "Sent Request";

					switch (sI.InvitationStatus)
					{
						case InvitationStatus.Pending:
							sentRequestStatus = "Sent Request";
							break;
						case InvitationStatus.Declined:
							sentRequestStatus = "Declined Request";
							break;
						case InvitationStatus.Accepted:
							sentRequestStatus = "Accepted Request";
							break;
						default:
							break;
					}

					if (NotificationsObject.ContainsKey(sI.ActivityId))
					{
						NotificationsObject[sI.ActivityId].Add(new NotificationObject(sentRequestStatus, sI.Activity, sI.ReceiveTime, NotificationType.Standard, sI.Id, sI.TargetUser));
					}
					else
					{
						var list = new List<NotificationObject>();
						list.Add(new NotificationObject(sentRequestStatus, sI.Activity, sI.ReceiveTime, NotificationType.Standard, sI.Id, sI.TargetUser));
						NotificationsObject.Add(sI.ActivityId, list);
					}
					TotalNotifications += 1;
				}
			}

			foreach (var item in NotificationsObject)
			{
				if (NotificationsObject[item.Key].Count > 0)
				{
					if (NotificationsObject[item.Key][0] != null && NotificationsObject[item.Key][0].Activity != null)
					{
						var taskStack = new TaskStack(NotificationsObject[item.Key][0].Activity, NotificationsObject[item.Key].Count);
						TaskStacskObject[item.Key] = taskStack;

						ContentStack.Children.Add(taskStack.TopStack);
						var fullNotifsStack = new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							VerticalOptions = LayoutOptions.StartAndExpand
						};

						NotificationObject last = NotificationsObject[item.Key][NotificationsObject[item.Key].Count - 1];

						foreach (var notification in NotificationsObject[item.Key])
						{
							fullNotifsStack.Children.Add(notification.OuterLayout);
							if (notification != last)
							{
								fullNotifsStack.Children.Add(notification.Separator);
							}

							if (notification.Type == NotificationType.Standard)
							{
								//var tapGestureRecognizer = new TapGestureRecognizer();
								//tapGestureRecognizer.Tapped += async (s, e) =>
								//{
								//	notification.OuterLayout.IsVisible = false;
								//	notification.Separator.IsVisible = false;
								//	taskStack.TasksNumber -= 1;
								//	taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber;
								//	if (taskStack.TasksNumber <= 0)
								//	{
								//		taskStack.TopStack.IsVisible = false;
								//	}
								//	DecreaseTotalNotifications();
								//	if (notification.InviteId != null)
								//	{
								//		ActivityInvitation invite = ViewModel.GetActivityInvitation(notification.InviteId);
								//		if (invite != null)
								//		{
								//			invite.Visible = false;
								//			await TalentDb.SaveOrUpdateItem<ActivityInvitation>(invite);
								//		}
								//	}
								//};
								//notification.IconRight.GestureRecognizers.Add(tapGestureRecognizer);
								notification.IconRight.Tapped += async () => 
								{ 
										notification.OuterLayout.IsVisible = false; 
										notification.Separator.IsVisible = false; 
										taskStack.TasksNumber -= 1; 
										taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber; 
										if (taskStack.TasksNumber <= 0) 
										{ 
											taskStack.TopStack.IsVisible = false; 
										} 
										DecreaseTotalNotifications (); 
										if (notification.InviteId != null)
										{ 
											ActivityInvitation invite = ViewModel.GetActivityInvitation(notification.InviteId); 
											if (invite != null) 
											{ 
												invite.Visible = false; 
												await TalentDb.SaveOrUpdateItem<ActivityInvitation>(invite); 
											} 
										} 
									};
							}
							else if (notification.Type == NotificationType.Invitation)
							{
								var tapGestureRecognizer = new TapGestureRecognizer();
								var tapAction = new Action(() =>
								{
									notification.InvitationLayout.IsVisible = !notification.InvitationLayout.IsVisible;
									if (notification.InvitationLayout.IsVisible)
									{
										//notification.IconRight.Text = "\u276e";
										notification.IconRight.Source = "arrow_left_icon.png";
									}
									else
									{
										//notification.IconRight.Text = "\u276f";
										notification.IconRight.Source = "arrow_right_icon.png";
									}
								});
								tapGestureRecognizer.Tapped += new EventHandler((obj, arg) => tapAction());

								notification.IconRight.Tapped += tapAction;

								notification.InnerLayout.GestureRecognizers.Add(tapGestureRecognizer);
								notification.OuterLayout.GestureRecognizers.Add(tapGestureRecognizer);

								notification.YesButton.Tapped += async () =>
								{
									if (LoadingViewFlag) { return; }
									LoadingViewFlag = true;
									SelectedInvitation = ViewModel.GetActivityInvitation(notification.InviteId);
									if (SelectedInvitation == null)
									{
										LoadingViewFlag = false;
										return;
									}
									TimeSpan timeDiff = SelectedInvitation.FinishTime - DateTime.Now;
									SelectedInvitation.InvitationStatus = InvitationStatus.Accepted;
									await TalentDb.SaveOrUpdateItem<ActivityInvitation>(SelectedInvitation);
									await ViewModel.StartActivity(SelectedInvitation.Activity, Convert.ToInt32(timeDiff.TotalSeconds), "", "", true);

									//Get the activity back
									Activity acceptedActivity = await ViewModel.GetActivityFromDb(SelectedInvitation.ActivityId);
									var order = acceptedActivity.Order;
									order.Hidden = false;
									await TalentDb.SaveOrUpdateItem<UserActivityOrder>(order);
									ViewModel.Activities.Add(acceptedActivity);

									//Hide Notif
									notification.OuterLayout.IsVisible = false;
									notification.Separator.IsVisible = false;
									taskStack.TasksNumber -= 1;
									taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber;
									if (taskStack.TasksNumber <= 0)
									{
										taskStack.TopStack.IsVisible = false;
									}
									DecreaseTotalNotifications();
									DecreaseAttentionNotifications();
									this.ActivityInvitations.Remove(SelectedInvitation);
									SelectedInvitation = null;
									LoadingViewFlag = false;
								};
								notification.NoButton.Tapped += async () =>
								{
									if (LoadingViewFlag) { return; }
									LoadingViewFlag = true;
									SelectedInvitation = ViewModel.GetActivityInvitation(notification.InviteId);
									if (SelectedInvitation == null)
									{
										LoadingViewFlag = false;
										return;
									}
									SelectedInvitation.InvitationStatus = InvitationStatus.Declined;
									await TalentDb.SaveOrUpdateItem<ActivityInvitation>(SelectedInvitation);

									Activity activity = await ViewModel.GetActivityFromDb(SelectedInvitation.ActivityId);
									ViewModel.Activities.Add(activity);

									await TalentPlusApp.RootPage.RefreshActivities();

									//Hide Notif
									notification.OuterLayout.IsVisible = false;
									notification.Separator.IsVisible = false;
									taskStack.TasksNumber -= 1;
									taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber;
									if (taskStack.TasksNumber <= 0)
									{
										taskStack.TopStack.IsVisible = false;
									}
									DecreaseTotalNotifications();
									DecreaseAttentionNotifications();
									SelectedInvitation = null;
									LoadingViewFlag = false;
								};
							}
							else
							{
								bool pendingFeedbackTapped = false;
								var tapGestureRecognizer = new TapGestureRecognizer();
								var tapAction = new Action(async () =>
								{
									if (pendingFeedbackTapped) { return; }
									pendingFeedbackTapped = true;
									await PushFeedback(notification.Activity, notification, taskStack);
									pendingFeedbackTapped = false;
								});

								tapGestureRecognizer.Tapped += (object sender, EventArgs e) => tapAction.Invoke();

								//notification.IconRight.GestureRecognizers.Add(tapGestureRecognizer);
								notification.IconRight.Tapped = tapAction; 

								notification.InnerLayout.GestureRecognizers.Add(tapGestureRecognizer);
								notification.AdditionalContentStack.GestureRecognizers.Add(tapGestureRecognizer);
								notification.OuterLayout.GestureRecognizers.Add(tapGestureRecognizer);
							}
						}

						ContentStack.Children.Add(fullNotifsStack);
						ContentStack.Children.Add(new BoxView { Color = Color.Transparent, HeightRequest = 10 });
					}
				}
			}

			if (TotalNotifications > 0)
			{
				TotalNotificationsAlertNumberAbsoluteLayout.IsVisible = true;
				TotalNotificationsLabel.Text = "" + TotalNotifications;
			}
			else
			{
				TotalNotificationsAlertNumberAbsoluteLayout.IsVisible = false;
			}

			if (AttentionNotifications > 0)
			{
				AttentionNotificationsAlertNumberAbsoluteLayout.IsVisible = true;
				AttentionNotificationsLabel.Text = "" + AttentionNotifications;
			}
			else
			{
				AttentionNotificationsAlertNumberAbsoluteLayout.IsVisible = false;
			}
		}

		private void RemoveNotificationsAndBrothers(NotificationObject notification, TaskStack taskStack, string activityId)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				notification.OuterLayout.IsVisible = false;
				notification.Separator.IsVisible = false;
				taskStack.TasksNumber -= 1;
				taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber;

				//RemoveOthersNotifications
				foreach (var sameGroupNotif in NotificationsObject[activityId])
				{
					if (sameGroupNotif != notification)
					{
						sameGroupNotif.OuterLayout.IsVisible = false;
						sameGroupNotif.Separator.IsVisible = false;
						taskStack.TasksNumber -= 1;
						taskStack.TasksNumberLabel.Text = "" + taskStack.TasksNumber;

						DecreaseTotalNotifications();
						DecreaseAttentionNotifications();
					}
				}

				if (taskStack.TasksNumber <= 0)
				{
					taskStack.TopStack.IsVisible = false;
				}
				DecreaseTotalNotifications();
				DecreaseAttentionNotifications();
			});
		}
        
		private AbsoluteLayout BuildTaskLayout(Activity activity)
		{
			var absoluteLayout = new AbsoluteLayout { Padding = new Thickness(0) };
			//var image = new Image { Source = "header_overview_task.png", Aspect = Aspect.Fill };
			var image = new RoundedBox
			{
				BackgroundColor = Helpers.Color.Purple.ToFormsColor(),
				CornerRadius = 10,
				RoundedSide = RoundedBox.RoundedSideType.Top
			};

			var stackLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.StartAndExpand,
				Padding = new Thickness(10, 0)
			};

			UnileverLabel taskTitle = new UnileverLabel
			{
				Text = activity.ShortDescription,
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			stackLayout.Children.Add(taskTitle);

			var tasksNumberLaber = new UnileverLabel
			{
				Text = "" + 0,
				TextColor = Helpers.Color.White.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			var tasksNumberLabelStack = new StackLayout { VerticalOptions = LayoutOptions.StartAndExpand, Children = { tasksNumberLaber } };

			var absoluteLayoutNumber = new AbsoluteLayout { Padding = new Thickness(0), HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
			var alertNumberTotalImage = new Image { Source = "task_number.png", Aspect = Aspect.AspectFit };

			absoluteLayoutNumber.Children.Add(alertNumberTotalImage, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteLayoutNumber.Children.Add(tasksNumberLabelStack, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			stackLayout.Children.Add(absoluteLayoutNumber);

			absoluteLayout.Children.Add(image, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);
			absoluteLayout.Children.Add(stackLayout, new Rectangle(0, 0, 1, 1), AbsoluteLayoutFlags.All);

			return absoluteLayout;
		}

		private void DecreaseTotalNotifications()
		{
			TotalNotifications -= 1;
			TotalNotificationsLabel.Text = "" + TotalNotifications;

			if (TotalNotifications <= 0)
			{
				TotalNotificationsAlertNumberAbsoluteLayout.IsVisible = false;
			}
		}

		private void DecreaseAttentionNotifications()
		{
			AttentionNotifications -= 1;
			AttentionNotificationsLabel.Text = "" + AttentionNotifications;

			if (AttentionNotifications <= 0)
			{
				AttentionNotificationsAlertNumberAbsoluteLayout.IsVisible = false;
			}
		}

		/// <summary>
		/// Push the page of the activty's feedback
		/// </summary>
		/// <param name="activity"></param>
		public async Task PushFeedback(Activity activity, NotificationObject notification, TaskStack taskStack)
		{
			TalentPlusApp.PendingNotification = false;

			FeedBackView fbView = new FeedBackView(activity, ViewModel.GetSelectedActivity(activity.Id));

			await this.Navigation.PushAsync(fbView);

			fbView.ProperlyQuit += (s, e) =>
			{
				RemoveNotificationsAndBrothers(notification, taskStack, activity.Id);
			};
		}

		public void Refresh()
		{
			LoadingViewFlag = true;
			ViewModel.LoadItemsCommand.Execute(null);
			LoadingViewFlag = false;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			OnAppearingActions();
		}

		public void OnAppearingActions()
		{
			#if __ANDROID__ 
	 
			RefreshPage ();
			
			if (_imgActivities != null)
			{ 
				_imgActivities.FilterColor = Helpers.Color.Primary.ToFormsColor (); 
			} 
	 
			#elif __IOS__ 
	 
			if (_imgActivities != null)
			{ 
				_imgActivities.FilterColor = Helpers.Color.Primary.ToFormsColor (); 
			}
	 
			RefreshPage (); 
	 
			#endif 
		}

		public void ClearContentStack()
		{
			ContentStack.Children.Clear();
		}

		public void RefreshPage()
		{
			if (ViewModel.DbActivityInvitations != null && ViewModel.DbActivityInvitations.Count > 0)
			{
				this.ActivityInvitations.Clear();
				this.SentInvitations.Clear();
				foreach (ActivityInvitation invite in ViewModel.DbActivityInvitations)
				{
					if (invite.SenderUserId != TalentPlusApp.CurrentUserId)
					{
						if (invite.InvitationStatus == InvitationStatus.Pending)
						{
							this.ActivityInvitations.Add(invite);
						}
					}
					else
					{
						this.SentInvitations.Add(invite);
					}
				}
			}

			BuildPageContent();

			OveralCompletedLabel.Text = ViewModel.ActivitiesCompletedPercent + "%";
			OveralCompletedLabel.TextColor = Helpers.Color.Primary.ToFormsColor ();

			LastCompletedLabel.Text = ViewModel.ActivitiesCompletedInSixMonths + "";
			LastCompletedLabel.TextColor = Helpers.Color.Primary.ToFormsColor ();
		}
	}
}