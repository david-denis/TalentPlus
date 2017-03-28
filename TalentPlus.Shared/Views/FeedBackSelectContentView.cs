using System;
using System.Collections.Generic;
using Xamarin.Forms;
using XLabs.Forms.Mvvm;
using TalentPlus.Shared.Helpers;
using Xamarin;

namespace TalentPlus.Shared
{
	public class FeedBackSelectContentView : BaseView
	{
		private StackLayout PageLayout = null;
        private FeedbackPost Post;

        private Activity activity;

		bool buttonClicked = false;

		public FeedBackSelectContentView (Activity activity, SelectedActivity selectedActivity, FeedbackPost post)
		{
            Post = post;
            this.activity = activity;
			
			Post.Activity = activity;
			Post.ActivityId = activity.Id;
			Post.Date = DateTime.Now;

			Title = "Provide Feedback";
			Padding = new Thickness(0, 10, 0, 0);

			BackgroundColor = Helpers.Color.White.ToFormsColor();

			var viewModel = new CameraViewModel ();
			BindingContext = viewModel;
			viewModel.ImageSelected += SubmitPictureFeedback;
			viewModel.VideoSelected += SubmitVideoFeedback;

			UnileverLabel description = new UnileverLabel
			{
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel)) * 1.2,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				XAlign = TextAlignment.Center,
			};

			var fs = new FormattedString();
			fs.Spans.Add(new Span
			{
				Text = "Share your insights about the \"",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = activity.ShortDescription,
				ForegroundColor = Helpers.Color.Primary.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});
			fs.Spans.Add(new Span
			{
				Text = "\" activity",
				ForegroundColor = Helpers.Color.Black.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel)),
				FontAttributes = FontAttributes.Bold
			});

			description.FormattedText = fs;

			#region Comment Button
			var addCommentButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				HeightRequest = 50,
			};

			addCommentButton.Clicked += addCommentButton_Clicked;

			var addCommentLabel = new UnileverLabel
			{
				Text = "Add a comment",
				//FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				TextColor = Xamarin.Forms.Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel))
			};

			addCommentLabel.Tapped += () =>
			{
				addCommentButton_Clicked(null, null);
			};

			var addCommentImage = new DarkIceImage
			{
				Source = "ico_pencil_white.png",
				Aspect = Aspect.AspectFit
			};

			addCommentImage.Tapped += () =>
			{
				addCommentButton_Clicked(null, null);
			};

			var addCommentAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(addCommentButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addCommentButton, new Rectangle(0, 0, 1, 1));
			addCommentAbsStack.Children.Add(addCommentButton);

			AbsoluteLayout.SetLayoutFlags(addCommentLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addCommentLabel, new Rectangle(0, 0, 1, 1));
			addCommentAbsStack.Children.Add(addCommentLabel);

			AbsoluteLayout.SetLayoutFlags(addCommentImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addCommentImage, new Rectangle(0.05, 0.2, 0.1, 0.90));
			addCommentAbsStack.Children.Add(addCommentImage);
			#endregion

			#region Image Button
			var addImageButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				HeightRequest = 50,
			};

			addImageButton.Clicked += addImageButton_Clicked;

			var addImageLabel = new UnileverLabel
			{
				Text = "Add an Image",
				//FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				TextColor = Xamarin.Forms.Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel))
			};

			addImageLabel.Tapped += () =>
			{
				addImageButton_Clicked(null, null);
			};

			var addImageImage = new DarkIceImage
			{
				Source = "ico_camera_white.png",
				Aspect = Aspect.AspectFit
			};

			addImageImage.Tapped += () =>
			{
				addImageButton_Clicked(null, null);
			};

			var addImageAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(addImageButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addImageButton, new Rectangle(0, 0, 1, 1));
			addImageAbsStack.Children.Add(addImageButton);

			AbsoluteLayout.SetLayoutFlags(addImageLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addImageLabel, new Rectangle(0, 0, 1, 1));
			addImageAbsStack.Children.Add(addImageLabel);

			AbsoluteLayout.SetLayoutFlags(addImageImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addImageImage, new Rectangle(0.05, 0.2, 0.1, 0.90));
			addImageAbsStack.Children.Add(addImageImage);
			#endregion

			#region Video Button
			var addVideoButton = new Button
			{
				BackgroundColor = Helpers.Color.Primary.ToFormsColor(),
				Text = "",
				BorderWidth = 2,
				BorderColor = Helpers.Color.Primary.ToFormsColor(),
				BorderRadius = 5,
				HeightRequest = 50,
			};

			addVideoButton.Clicked += addVideoButton_Clicked;

			var addVideoLabel = new UnileverLabel
			{
				Text = "Add a short video",
				//FontAttributes = Xamarin.Forms.FontAttributes.Bold,
				TextColor = Xamarin.Forms.Color.White,
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(UnileverLabel))
			};

			addVideoLabel.Tapped += () =>
			{
				addVideoButton_Clicked(null, null);
			};

			var addVideoImage = new DarkIceImage
			{
				Source = "ico_video_white.png",
				Aspect = Aspect.AspectFit
			};

			addVideoImage.Tapped += () =>
			{
				addVideoButton_Clicked(null, null);
			};

			var addVideoAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(addVideoButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addVideoButton, new Rectangle(0, 0, 1, 1));
			addVideoAbsStack.Children.Add(addVideoButton);

			AbsoluteLayout.SetLayoutFlags(addVideoLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addVideoLabel, new Rectangle(0, 0, 1, 1));
			addVideoAbsStack.Children.Add(addVideoLabel);

			AbsoluteLayout.SetLayoutFlags(addVideoImage, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(addVideoImage, new Rectangle(0.05, 0.2, 0.1, 0.90));
			addVideoAbsStack.Children.Add(addVideoImage);
			#endregion

			var ButtonLayout = new StackLayout {
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				//HeightRequest = 80,
				Spacing = 15,
				Padding = 0,
				Children = {
					addCommentAbsStack,
					addImageAbsStack,
					addVideoAbsStack,
				}
			};

			var stackLayoutCard = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				VerticalOptions = LayoutOptions.Start,
				Padding = new Thickness(10, 0),
			};

			var hintImage = new Image
			{
				HorizontalOptions = LayoutOptions.Center,
				Source = "thumbs_up.png",
				Scale = 1
			};

			var hintText = new UnileverLabel
			{
				Text = "Your feedback helps others",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(UnileverLabel)),
				HorizontalOptions = LayoutOptions.Center
			};

			stackLayoutCard.Children.Add(new StackLayout { Padding = new Thickness(20, 0), Children = { description } });
			stackLayoutCard.Children.Add(new StackLayout { Padding = new Thickness(20, 20, 20, 10), Spacing = 0, VerticalOptions = LayoutOptions.End, Children = { hintImage, hintText } });
			stackLayoutCard.Children.Add(ButtonLayout);

			//var UserListView = new TPListView
			//{
			//	HasUnevenRows = false,
			//};

			//var cell = new DataTemplate(typeof(TextCell));
			//cell.SetBinding(TextCell.TextProperty, "Name");
			//UserListView.ItemTemplate = cell;

			//if (selectedActivity != null)
			//	UserListView.ItemsSource = selectedActivity.InvolvedUsers;

			var noInsightButton = new Button
			{
				BackgroundColor = Xamarin.Forms.Color.White,
				Text = "",
				BorderWidth = 2,
				BorderColor = Xamarin.Forms.Color.White,
				BorderRadius = 10,
				HeightRequest = 50,
			};
			noInsightButton.Clicked += validateButton_Clicked;

			var noInsightLabel = new UnileverLabel
			{
				Text = "Continue without my insight >",
				TextColor = Helpers.Color.Gray.ToFormsColor(),
				XAlign = TextAlignment.Center,
				YAlign = TextAlignment.Center,
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(UnileverLabel))
			};

			noInsightLabel.Tapped += () =>
			{
				validateButton_Clicked(null, null);
			};

			var noInsightAbsStack = new AbsoluteLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 50,
			};

			AbsoluteLayout.SetLayoutFlags(noInsightButton, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(noInsightButton, new Rectangle(0, 0, 1, 1));
			noInsightAbsStack.Children.Add(noInsightButton);
			
			AbsoluteLayout.SetLayoutFlags(noInsightLabel, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(noInsightLabel, new Rectangle(0, 0, 1, 1));
			noInsightAbsStack.Children.Add(noInsightLabel);

			var ActionLayout = new StackLayout
			{
				BackgroundColor = Xamarin.Forms.Color.Transparent,
				Spacing = 10,
				Padding = 10,
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				Children = {
                    noInsightAbsStack
                }
			};

			PageLayout = new StackLayout
			{
				Padding = new Thickness(0, 10, 0, 0),
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = 
                {
					stackLayoutCard,
					ActionLayout
					//UserListView
				}
			};

			Content = new ScrollView 
			{
				Content = PageLayout
			};
		}

		async void addCommentButton_Clicked(object sender, EventArgs e)
		{
			if (buttonClicked) { return; }
			buttonClicked = true;
			var textPage = new TextPage();
			textPage.Post = this.Post;
			textPage.activity = this.activity;

			textPage.ProperlyQuit += (s, arg) =>
			{
				OnProperlyQuit(EventArgs.Empty);
			};

			await Navigation.PushAsync(textPage);
			buttonClicked = false;
		}

		async void addImageButton_Clicked(object sender, EventArgs e)
		{			
			/*var cameraPage = ViewFactory.CreatePage<CameraViewModel, Page>();
			((CameraPage)cameraPage).Post = this.Post;
			((CameraPage)cameraPage).activity = this.activity;
			Navigation.PushAsync(cameraPage as Page);*/
			if (buttonClicked) { return; }
			buttonClicked = true;

			var result = await DisplayActionSheet("Choose source", "Cancel", null, new string[2]{"Camera", "Photo Library"});

			if(result == null || result.Equals("Cancel")){
				buttonClicked = false;
				return;
			}
			else if (result.Equals("Camera"))
			{
				var viewModel = BindingContext as CameraViewModel;
				viewModel.TakePictureCommand.Execute(null);
			}
			else{
				var viewModel = BindingContext as CameraViewModel;
				viewModel.SelectPictureCommand.Execute(null);

			}
			buttonClicked = false;
		}

		async void addVideoButton_Clicked(object sender, EventArgs e)
		{
			/*var videoPage = ViewFactory.CreatePage<VideoViewModel, Page>();
			((VideoPage)videoPage).Post = this.Post;
			((VideoPage)videoPage).activity = this.activity;
			Navigation.PushAsync(videoPage as Page);*/

			if (buttonClicked) { return; }
			buttonClicked = true;

			var result = await DisplayActionSheet("Choose source", "Cancel", null, new string[2]{"Camera", "Video Library"});
			if(result == null || result.Equals("Cancel")) {
				buttonClicked = false;
				return;
			}
			else if (result.Equals("Camera"))
			{
				var viewModel = BindingContext as CameraViewModel;
				viewModel.TakeVideoCommand.Execute(null);
			}
			else{
				var viewModel = BindingContext as CameraViewModel;
				viewModel.SelectVideoCommand.Execute(null);
			}
			buttonClicked = false;
		}

		async void validateButton_Clicked(object sender, EventArgs e)
		{
			if (LoadingViewFlag) { return; }
			LoadingViewFlag = true;
			HideBackButtonFlag = true;
			ActivitiesView.IsNeedReload = true;
			await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
			OnProperlyQuit(EventArgs.Empty);
			await Navigation.PopToRootAsync();
			LoadingViewFlag = false;
		}

		private async void SubmitPictureFeedback()
		{
			bool IsSuccess = false;

			LoadingViewFlag = true;
			HideBackButtonFlag = true;

			try
			{		 
				ActivitiesView.IsNeedReload = true;
				//Post.ImageUrl = (BindingContext as CameraViewModel).ImagePath;
				Post.ImageUrl = await Helpers.Utility.UploadPhoto((BindingContext as CameraViewModel).ImageBytes);

				await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
				IsSuccess = true;
			}
			catch (Exception ex)
			{
				Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "CameraPage.SubmitPictureFeedback()" }
					});
			}

			LoadingViewFlag = false;

			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);


			if (IsSuccess) {
				OnProperlyQuit(EventArgs.Empty);
				await Navigation.PopToRootAsync ();
				//TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
				//TalentPlus.Shared.Helpers.Utility.ForceHideBackButton ();
			}
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
		}

		private async void SubmitVideoFeedback()
		{
			bool IsSuccess = false;

			LoadingViewFlag = true;
			HideBackButtonFlag = true;

			try
			{
				ActivitiesView.IsNeedReload = true;

				var thing = BindingContext as CameraViewModel;
				Post.VideoUrl = "";
				Post.VideoId = await Helpers.Utility.UploadVideo(thing.ImageBytes);
				Post.VideoStatus = VideoStatus.Processing;

				//Post.VideoUrl = (BindingContext as VideoViewModel).VideoInfo;
				await TalentDb.SaveOrUpdateItem<FeedbackPost>(Post);
				IsSuccess = true;
			}
			catch (Exception ex)
			{
				Insights.Report(ex, new Dictionary<string, string>
					{
						{ "Where", "VideoPage.SubmitVideoFeedback()" },
					});
			}

			LoadingViewFlag = false;

			await TalentPlusApp.RootPage.overview.FeedbackSubmitted(activity.Id);

			if (IsSuccess) {
				OnProperlyQuit(EventArgs.Empty);
				await Navigation.PopToRootAsync();
			}
			TalentPlus.Shared.Helpers.Utility.RefreshTabBar ();
		}

		protected virtual void OnProperlyQuit(EventArgs e)
		{
			EventHandler handler = ProperlyQuit;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		public event EventHandler ProperlyQuit;

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}
	}
}
